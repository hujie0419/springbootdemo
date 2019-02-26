using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.DataAccess.Models;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Activity.Server.ServiceProxy;
using Tuhu.Service.Config;
using Tuhu.Service.Config.Models;
using Tuhu.Service.Config.Models.Requests;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models.Push;
using Tuhu.Service.UserAccount;
using Tuhu.ZooKeeper;

namespace Tuhu.Service.Activity.Server.Manager
{
    public class BargainManager
    {
        public static readonly ILog Logger = LogManager.GetLogger(typeof(BargainManager));
        //每日帮砍次数限制【配置文件读取】[ps:不包括帮自己砍的次数]
        public static readonly int TodayHelpBargainCountLimit = Convert.ToInt32(ConfigurationManager.AppSettings["TodayHelpBargainCountLimit"] ?? "1");

        private static readonly string ShareBargainNewUser = "ShareBargainNewUser";

        //测试账号
        private static readonly List<Guid> TestUserIdsConst = new List<Guid>() {
                new Guid("a3136282-251e-462a-af0c-627843f5c649"),//15026884502 dev
                new Guid("e0740835-4ca1-4275-95b5-6ab0ca45086a"),//15026884502 cn
                new Guid("8cf6135b-4f38-46ff-8ad6-d8f952ecc8b0"),//18221101981 dev
                new Guid("24129485-df4e-4676-93b0-859e0d71ae8b"),//18221101981 cn
                new Guid("cefc89e4-b91a-4f2d-b494-e3740a509f0e"),//18501613334 dev
                new Guid("4ffd1e57-3279-4e93-94b3-ddd2f2021558"),//18501613334 cn
            };

        #region 分享砍价

        /// <summary>
        ///     获取砍价列表（已废弃）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<PagedModel<BargainProductModel>> GetBargainProductList(
            GetBargainproductListRequest request)
        {
            var result = new PagedModel<BargainProductModel>()
            {
                Pager = new PagerModel()
                {
                    PageSize = request.PageSize,
                    CurrentPage = request.PageIndex
                }
            };
            var data = new List<BargainProductModel>();
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.BargainActivityName))
            {
                var Cache = await client.GetOrSetAsync(GlobalConstant.BargainActivityKey,
                    () => DalBargain.GetAllBargainProduct(),
                    GlobalConstant.BargainActivitySpan);
                if (Cache.Success && Cache.Value != null)
                {
                    data = Cache.Value.ToList();
                }
                else
                {
                    Logger.Warn($"砍价活动配置商品Redis缓存读取失败,GetAllBargainProduct");
                    data = (await DalBargain.GetAllBargainProduct()).ToList();
                }
            }

            if (request.UserId == null || request.UserId == Guid.Empty)
            {
                var dat = data.Where(g => g.EndDateTime > DateTime.Now);
                result.Pager.Total = dat.Count();
                result.Source = dat.OrderByDescending(g => g.Sequence).Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize);
            }
            else
            {
                var dat = await DalBargain.GetBargainProductByUser(request.UserId.Value);

                var dat2 = data.Where(g => !dat.Select(t => t.ActivityProductId).Contains(g.ActivityProductId))
                    .Union(dat)
                    .Where(g => g.EndDateTime > DateTime.Now ||
                                g.IsOver && !g.IsPurchased && g.EndDateTime.AddDays(1) > DateTime.Now || g.IsPurchased);
                result.Pager.Total = dat2.Count();
                result.Source = dat2.ToList().OrderBy(g => g.IsPurchased).ThenByDescending(g => g.IsOver)
                    .ThenByDescending(g => g.Sequence)
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize);
            }

            result.Source?.ForEach(g => g.ActivityId = GlobalConstant.BargainActivityId);
            return result;
        }

        public static async Task<BargainProductHistory> FetchBargainProductHistory(Guid userId, int apId, string pid)
        {
            bool readOnly = await GetBargainOwnerCache(userId);
            var dat = await DalBargain.FetchBargainProductHistory(userId, apId, pid, !readOnly);
            dat.ForEach(g =>
            {
                if (g.Rate > 1.2)
                {
                    g.Info = "完成了恐怖的暴击";
                }
                else if (g.Rate > 0.8)
                {
                    g.Info = "挥出了华丽的一刀";
                }
                else
                {
                    g.Info = "轻轻的摸了一下";
                }
            });
            var result = new BargainProductHistory()
            {
                UserId = userId,
                ActivityProductId = apId,
                TotalReduce = dat.FirstOrDefault()?.TotalReduce ?? 0,
                CurrentlyTimes = dat.Count(),
                BargainHistoryList = dat.ToList()
            };
            return result;
        }
        /// <summary>
        ///     受邀请人进行砍价
        /// </summary>
        /// <param name="idKey"></param>
        /// <param name="userId"></param>
        /// <param name="apId"></param>
        /// <returns></returns>
        public static async Task<BargainResult> AddBargainAction(Guid idKey, Guid userId, int apId)
        {
            using (var zklock = new ZooKeeperLock($"{idKey:D}"))
            {
                if (await zklock.WaitAsync(3000))
                {
                    var readOnly = !(await GetBargainOwnerCache(idKey));
                    var readOnly2 = !(await GetBargainProductCache());
                    // 获取发起的砍价信息
                    var data = await DalBargain.FetchCurrentBargainData(idKey, false);

                    if (data == null)
                    {
                        Logger.Info($" {idKey} {userId} {apId} ");
                        return new BargainResult
                        {
                            Code = 10,
                            Info = "商品不存在或已下架"
                        };
                    }

                    // 获取用户该次发起的帮他人砍记录
                    var actionInfo = await DalBargain.CheckBargainShare(userId, idKey, readOnly && readOnly2);

                    //针对一个活动产品，用户的所有砍价数据(自己砍和别人帮砍)
                    var allCut = await DalBargain.CheckBargainAllShare(userId, data.PID, data.BeginDateTime, data.EndDateTime);
                    //针对一个活动产品，用户的别人帮砍次数
                    int helpCut = 0;
                    if (allCut.Any())
                    {
                        helpCut = allCut.Where(g => g.UserId != g.OwnerId).ToList().Count;
                    }

                    //剩余需要砍的金额
                    var remnantMoney = data.OriginalPrice - data.FinalPrice - data.CurrentRedece;

                    //获取用户砍价次数
                    var readOnly3 = !(await GetBargainOwnerCache(userId));

                    var todayDate = DateTime.Now.ToString("yyyy-MM-dd 00:00:000");
                    //获取用户今天帮别人砍数据
                    var userBargainCount = await DalBargain.CheckUserBargainCount(userId, todayDate, readOnly3);

                    var result = new BargainResult
                    {
                        RemnantMoney = remnantMoney,
                        SimpleDisplayName = data.SimpleDisplayName,
                        HelpCutPriceTimes = data.HelpCutPriceTimes
                    };

                    //判断该砍价商品是否为新户帮砍商品
                    if (data.CutPricePersonLimit == 1 && data.OwnerId != userId)
                    {
                        //验证砍价新用户验证
                        if (!CheckBargainNewUser(userId))
                        {
                            result.Code = 8;
                            result.Info = "仅限新用户帮砍~";
                            return result;
                        }
                        //验证途虎新用户
                        var orderCount = (await GroupBuyingManager.CheckNewUser(userId)).Item2;
                        if (orderCount > 0)
                        {
                            result.Code = 8;
                            result.Info = "仅限新用户帮砍~";
                            return result;
                        }
                    }

                    if (data.OwnerId != userId && data.HelpCutPriceTimes > 0)
                    {
                        if (helpCut >= data.HelpCutPriceTimes)
                        {
                            result.Code = 9;
                            result.Info = "帮砍次数已满~";
                            return result;
                        }
                    }
                    if (data.IsOver || data.TotalCount <= data.CurrentCount)
                    {
                        result.Code = 2;
                        result.Info = "手慢咯，砍价已经成功啦~";
                    }
                    else if (data.EndDateTime < DateTime.Now)
                    {
                        result.Code = 3;
                        result.Info = "手慢啦，本商品砍价已结束~";
                    }
                    else if (data.CurrentStockCount < 1)
                    {
                        result.Code = 4;
                        result.Info = "手慢啦，砍价商品已抢完~";
                    }
                    else if (actionInfo.Count > 1 && data.OwnerId == userId)
                    {
                        result.Code = 6;
                        result.Reduce = actionInfo.Max(g => g.Item2);
                        result.Info = "分享人已进行过两次砍价~";
                    }
                    else if (actionInfo.Count > 0 && data.OwnerId != userId)
                    {
                        result.Code = 5;
                        result.Reduce = actionInfo.Max(g => g.Item2);
                        result.Info = "您已经为好友完成过砍价~";
                    }
                    else if (userBargainCount >= TodayHelpBargainCountLimit && data.OwnerId != userId)
                    {
                        var ignoreResult = CheckIgnoreNewUserSwitch("IsIgnoreTodayHelpBargainCountLimit");
                        if (!ignoreResult)
                        {
                            result.Code = 7;
                            result.Info = "您今天砍价次数已经用完，明天再来吧~";
                        }
                    }
                    else
                    {
                        //进行帮砍
                        result = await CreateUserBargainRecord(data, idKey, userId, apId);

                        #region 用户每日首次帮砍成功后，给帮砍的 userid 发一条实时推送
                        //int pushID = 3857;
                        //if (userBargainCount == 0 && userId != data.OwnerId && result.Code == 1)
                        //{
                        //    DailyFirstBarginPushAsync(userId, pushID);
                        //}
                        //else
                        //{
                        //    Logger.Info($"每日首次帮砍成功 推送不符合规则 DailyFirstBarginPush =》userId={userId}&pushId={pushID}&userBargainCount={userBargainCount}&OwnerId={data.OwnerId}&Code={result.Code}");
                        //}
                        #endregion

                        //帮砍后设为老用户
                        if (result.Code == 1)
                        {
                            SetBargainOldUser(userId);
                        }
                    }
                    return result;
                }
                else
                {
                    Logger.Error("砍价等待链接超时");
                    return new BargainResult
                    {
                        Code = 0
                    };
                }
            }
        }

        /// <summary>
        ///  用户每日首次帮砍成功后 推送
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="pushId">推送模板id</param>
        private static async void DailyFirstBarginPushAsync(Guid userId, int pushId)
        {
            try
            {
                var userInfo = await ActivityManager.FetchUserByUserIdByCacheAsync(userId);
                if (userInfo != null && userInfo.Success && userInfo.Result != null)
                {
                    string nickName = userInfo.Result.Nickname?.Trim() ?? "";
                    //加密 如果11位 那么隐藏中间 四位
                    if (nickName.Length == 11)
                    {
                        nickName = nickName.Substring(0, 3) + "****" + nickName.Substring(7, 4);
                    }
                    PushTemplateLog _PushTemplateLog = new PushTemplateLog()
                    {
                        Replacement = JsonConvert.SerializeObject(
                            new Dictionary<string, string>
                            {
                                    { "{{nickname}}",nickName},
                            })
                    };
                    using (var templatePushClient = new TemplatePushClient())
                    {
                        var result = templatePushClient.PushByUserIDAndBatchID(new List<string> { userId.ToString() }, pushId, _PushTemplateLog);
                        Logger.Info($" 每日首次帮砍成功 推送结果 UserFirstBarginDayPush =》userId={userId}&pushId={pushId}&result={result.Success}");
                    }
                }
                else
                {
                    Logger.Warn($"每日首次帮砍成功 推送失败 DailyFirstBarginPush =》userId={userId}&pushId={pushId}&Msg=FetchUserByUserIdByCacheAsync获取用户信息失败");
                }

            }
            catch (Exception ex)
            {
                Logger.Warn($"每日首次帮砍成功 推送失败 DailyFirstBarginPush =》userId={userId}&pushId={pushId}&ex={ex.Message}");
            }

        }

        /// <summary>
        ///     检查商品是否可购买 -- 废弃
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="apId"></param>
        /// <param name="pid"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static async Task<ShareBargainBaseResult> CheckBargainProductStatus(Guid ownerId, int apId, string pid,
            string deviceId)
        {
            //用户只可购买一个商品
            if (DateTime.Now >= GlobalConstant.TyreFestivalActivityBeginTime
                && DateTime.Now <= GlobalConstant.TyreFestivalActivityEndTime)
            {
                var userBargainOwnerActionCount = await DalBargain.CheckUserBargainOwnerActionCount(ownerId,
                    GlobalConstant.TyreFestivalActivityBeginTime, GlobalConstant.TyreFestivalActivityEndTime, false);
                if (userBargainOwnerActionCount > 0)
                {
                    return new ShareBargainBaseResult()
                    {
                        Code = 7,
                        Info = "活动期内只能领取一件商品哦"
                    };
                }
            }

            var val = await DalBargain.CheckBargainProductStatus(ownerId, apId, pid);

            if (val == null)
            {
                return new ShareBargainBaseResult()
                {
                    Code = 4,
                    Info = "您选中的宝贝还未完成砍价哦"
                };
            }

            var parameters = new List<BuyLimitModel>
            {
                new BuyLimitModel
                {
                    ModuleName = "sharebargain",
                    ModuleProductId = apId.ToString(),
                    LimitObjectId = ownerId.ToString("D"),
                    ObjectType = LimitObjectTypeEnum.UserId.ToString()
                }
            };
            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                parameters.Add(new BuyLimitModel
                {
                    ModuleName = "sharebargain",
                    ModuleProductId = apId.ToString(),
                    LimitObjectId = deviceId,
                    ObjectType = LimitObjectTypeEnum.DeviceId.ToString(),
                });
            }
            else
            {
                return new ShareBargainBaseResult
                {
                    Code = 4,
                    Info = "请填写设备号"
                };
            }

            switch (val.Code)
            {
                case 1:
                    val.Info = "可购买";
                    break;
                case 2:
                    val.Info = "商品已下架超过24小时，无法购买";
                    break;
                case 3:
                    val.Info = "您通过本活动购买过此商品，无法再次购买";
                    break;
                default:
                    val.Info = "您选中的宝贝还未完成砍价哦";
                    break;
            }


            if (val.Code == 1)
            {
                var checkResult = await LimitBuyManager.SelectBuyLimitInfo(parameters);

                if (checkResult.Exists(g => g.CurrentCount > 0))
                {
                    val.Code = 3;
                    val.Info = "您已通过本活动购买过此商品，无法再次购买";
                    Logger.Warn(
                        $"CheckBargainProductStatus-->ownerId:{ownerId:D}-->{JsonConvert.SerializeObject(checkResult)}");
                }
            }

            return new ShareBargainBaseResult
            {
                Code = val.Code,
                Info = val.Info
            };
        }

        /// <summary>
        ///     用户发起砍价（已废弃）
        /// </summary>
        /// <param name="ownerId">发起人的ID</param>
        /// <param name="apId">ActivityProductId</param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static async Task<BargainShareResult> AddShareBargain(Guid ownerId, int apId, string pid)
        {
            bool readOnly = await GetBargainOwnerCache(ownerId);
            //  判断用户是否已经发起砍价
            var dat = await DalBargain.CheckUserShareBargain(ownerId, apId, !readOnly);
            if (dat != null && dat.Code == 1)
            {
                dat.Info = "分享成功~";
                return dat;
            }

            //用户只可购买一个商品
            //if (DateTime.Now >= GlobalConstant.TyreFestivalActivityBeginTime
            //    && DateTime.Now <= GlobalConstant.TyreFestivalActivityEndTime)
            //{
            //    var userBargainOwnerActionCount = await DalBargain.CheckUserBargainOwnerActionCount(ownerId, false);
            //    if (userBargainOwnerActionCount > 0)
            //    {
            //        return new BargainShareResult()
            //        {
            //            Code = 5,
            //            Info = "每个用户只可购买一次商品"
            //        };
            //    }
            //}

            bool readOnly2 = await GetBargainProductCache();
            var data = await DalBargain.CheckBargainProduct(apId, pid, !readOnly2);
            if (data == 3)
            {
                return new BargainShareResult()
                {
                    Code = 3,
                    Info = "手慢啦，本商品砍价已结束~"
                };
            }
            else if (data == 4)
            {
                return new BargainShareResult()
                {
                    Code = 4,
                    Info = "手慢啦，砍价商品已抢完~"
                };
            }


            var result = await DalBargain.AddShareBargain(ownerId, apId, pid);
            if (result.Code == 1)
            {
                await SetBargainOwnerCache(ownerId);
                await SetBargainOwnerCache(result.IdKey ?? Guid.Empty);
            }

            return result;
        }
        /// <summary>
        ///     受邀人获取分享产品信息
        /// </summary>
        /// <param name="idKey"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static async Task<BargainShareProductModel> FetchShareBargainInfo(Guid idKey, Guid UserId)
        {
            var result = await DalBargain.FetchShareBargainInfo(idKey);
            if (result.OwnerId == UserId)
            {
                result.IsOwner = true;
            }

            if (UserId == null || UserId == Guid.Empty)
            {
                if (result.Code == 2)
                {
                    result.Info = "手慢啦，砍价商品已抢完~";
                }
                else if (result.Code == 3)
                {
                    result.Info = "手慢咯，砍价已经成功啦~";
                }
                else if (result.Code == 4)
                {
                    result.Info = "手慢啦，本商品砍价已结束~";
                }

                return result;
            }

            var dat = await DalBargain.CheckBargainShare(UserId, idKey, false);
            if (dat.Count > 0)
            {
                result.Code = 5;
                result.Info = "您已经为好友完成过砍价~";
            }

            return result;
        }
        /// <summary>
        ///     获取砍价配置
        /// </summary>
        /// <returns></returns>
        public static async Task<BargainGlobalConfigModel> GetBargainShareConfig()
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ShareBargainName))
            {
                var cacheResult =
                    await client.GetOrSetAsync("GlobalConfig", GetBargainGlobalConfig, TimeSpan.FromHours(2));
                if (cacheResult.Success && cacheResult.Value != null)
                {
                    return cacheResult.Value;
                }

                Logger.Warn($"获取砍价全局配置失败");
                return await GetBargainGlobalConfig();
            }
        }

        /// <summary>
        /// 刷新砍价所有缓存
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> RefreshShareBargainCache()
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ShareBargainName))
            {
                // 刷新首页砍价模块缓存
                var indexProduct = await GetBargainProductForIndexReal();
                await client.RemoveAsync("Index");
                var refreshkey = await client.SetAsync("Index", indexProduct, TimeSpan.FromHours(24));
                // 刷新全局配置缓存信息
                await client.RemoveAsync("GlobalConfig");
                //var refreshkey2 = await client.RemoveAsync(GlobalConstant.ShareBargainKey);

                //刷新砍价商品配置redis缓存通用key前缀
                var refreshResult = await CacheManager.CommonRefreshKeyPrefixAsync(GlobalConstant.ShareBargainName,
                             GlobalConstant.BargainProductSettingKey, TimeSpan.FromDays(1));
                if (!refreshResult)
                {
                    Logger.Warn($"RefreshShareBargainCache => CommonRefreshKeyPrefixAsync," +
                        $"刷新砍价商品配置redis缓存通用key前缀失败");
                }

                // 刷新砍价商品Hash缓存
                var data = await DalBargain.GetAllBargainProductItem();
                var refreshkey2 = await RefreshShareBargainCache(data);
                await client.RemoveAsync(GlobalConstant.ShareBargainSliceShowKey);
                if (!refreshkey2 || !refreshkey.Success)
                {
                    Logger.Warn("更新砍价商品列表缓存Redis缓存失败", refreshkey.Exception);
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// 清除所有砍价商品配置缓存,分享信息，设置新数据缓存
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private static async Task<bool> RefreshShareBargainCache(List<BargainProductItem> ids)
        {
            var data = await SelectBargainProductInfo(ids.Select(g => g.ActivityProductId.ToString()).ToList());
            using (var hashClient =
                CacheHelper.CreateHashClient(GlobalConstant.ShareBargainHashKey, TimeSpan.FromHours(24)))
            {
                //清除旧缓存数据
                var removeOldResult = hashClient.RemoveSelf();
                if (!removeOldResult.Success)
                {
                    //补偿清除一次
                    var removeRepeat = hashClient.RemoveSelf();
                    if (!removeRepeat.Success)
                    {
                        Logger.Warn($"RefreshShareBargainCache清除旧缓存数据失败,Message:{removeRepeat.Message}");
                    }
                }
                var dic = new Dictionary<string, object>();
                //设置新缓存数据
                data.ForEach(g => dic.Add($"{g.ActivityProductId}/{g.Pid}", g));
                var cacheData = new ReadOnlyDictionary<string, object>(dic);
                return (await hashClient.SetAsync<BargainProductNewModel>(cacheData)).Success;
            }
        }

        /// <summary>
        /// 设置砍价信息缓存
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private static async Task<bool> SetShareBargainCache(List<BargainProductItem> ids)
        {
            foreach (var item in ids)
            {
                //刷新活动参与人数缓存通用key
                var refreshResult = await CacheManager.CommonRefreshKeyPrefixAsync(GlobalConstant.ShareBargainName,
                        $"{GlobalConstant.BargainProductSettingKey}/{item.ActivityProductId}", TimeSpan.FromDays(1));
                if (!refreshResult)
                {
                    Logger.Warn($"SetShareBargainCache");
                }
            }

            //设置活动信息:配置、参与人数 缓存
            var data = await SelectBargainProductInfo(ids.Select(g => g.ActivityProductId.ToString()).ToList());
            using (var hashClient =
                CacheHelper.CreateHashClient(GlobalConstant.ShareBargainHashKey, TimeSpan.FromHours(24)))
            {
                var dic = new Dictionary<string, object>();
                //设置新缓存数据
                data.ForEach(g => dic.Add($"{g.ActivityProductId}/{g.Pid}", g));
                var cacheData = new ReadOnlyDictionary<string, object>(dic);
                return (await hashClient.SetAsync<BargainProductNewModel>(cacheData)).Success;
            }
        }

        public static async Task<IEnumerable<BargainProductModel>> SelectBargainProductById(Guid OwnerId, Guid UserId,
            List<BargainProductItem> ProductItems)
        {
            Guid? para = OwnerId;
            //if (UserId == Guid.Empty) para = null;
            bool readOnly = await GetBargainOwnerCache(OwnerId);
            bool readOnly2 = await GetBargainProductCache();
            var val = await DalBargain.SelectBargainProductById(para, ProductItems, !(readOnly || readOnly2));
            var result = new List<BargainProductModel>();
            if (val.Any())
            {
                if (UserId == Guid.Empty && OwnerId != Guid.Empty)
                {
                    val.First().IsOwner = false;
                    val.First().Bargained = false;
                }

                if (UserId != Guid.Empty && UserId != OwnerId)
                {
                    if (ProductItems.Count != 1)
                    {
                        return new List<BargainProductModel>();
                    }
                    else
                    {
                        val.First().IsOwner = false;
                        val.First().Bargained =
                            await DalBargain.CheckUserBargained(OwnerId, UserId, ProductItems.FirstOrDefault());
                    }
                }

                val.ForEach(g => g.ActivityId = GlobalConstant.BargainActivityId);
                ProductItems.ForEach(g => result.AddRange(val.Where(t => t.ActivityProductId == g.ActivityProductId)));
            }

            return result;
        }

        public static async Task<PagedModel<BargainProductItem>> SelectBargainProductItems(Guid UserId, int PageIndex,
            int PageSize)
        {
            var result = new PagedModel<BargainProductItem>()
            {
                Pager = new PagerModel()
                {
                    PageSize = PageSize,
                    CurrentPage = PageIndex
                }
            };
            var data = new List<BargainProductModel>();
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ShareBargainName))
            {
                var Cache = await client.GetOrSetAsync(GlobalConstant.ShareBargainKey,
                    () => DalBargain.SelectBargainProductItems(),
                    GlobalConstant.ShareBargainSpan);
                if (Cache.Success && Cache.Value != null)
                {
                    data = Cache.Value.ToList();
                }
                else
                {
                    Logger.Warn($"砍价活动配置商品Redis缓存读取失败,GetAllBargainProduct");
                    data = (await DalBargain.SelectBargainProductItems()).ToList();
                }
            }

            if (UserId == null || UserId == Guid.Empty)
            {
                var dat = data.Where(g => g.EndDateTime > DateTime.Now);
                result.Pager.Total = dat.Count();
                result.Source = dat?.OrderByDescending(g => g.Sequence).Select(g => new BargainProductItem()
                {
                    PID = g.PID,
                    ActivityProductId = g.ActivityProductId
                }).Skip((PageIndex - 1) * PageSize).Take(PageSize);
            }
            else
            {
                bool readOnly = await GetBargainOwnerCache(UserId);
                bool readOnly2 = await GetBargainProductCache();
                var dat = await DalBargain.SelectBargainProductItemsByUserId(UserId, !(readOnly || readOnly2));
                var dat1 = dat.Select(g => g.ActivityProductId);
                var dat2 = data.Where(g => !dat1.Contains(g.ActivityProductId))
                    .Union(dat)
                    .Where(g => g.ShowBeginTime < DateTime.Now || g.HasBargainHistory);

                var dat3 = new List<BargainProductItem>();

                //优先显示砍价完成、未购买的商品
                dat3.AddRange(dat2.Where(g => g.EndDateTime.AddDays(1) > DateTime.Now && g.IsOver &&
                                              g.IsPurchased == false)
                    .Select(g => new BargainProductItem()
                    {
                        PID = g.PID,
                        ActivityProductId = g.ActivityProductId
                    }));

                //过滤：不在上架时间内商品 排序：已购买商品 > 排序号 
                dat3.AddRange(dat2.Where(g => g.EndDateTime > DateTime.Now && g.ShowBeginTime < DateTime.Now)
                    .OrderBy(g => g.IsPurchased)
                    .ThenByDescending(g => g.Sequence)
                    .Select(g => new BargainProductItem()
                    {
                        PID = g.PID,
                        ActivityProductId = g.ActivityProductId
                    }).Where(g => !dat3.Select(t => t.ActivityProductId).Contains(g.ActivityProductId)));


                //过滤：未发起砍价商品 排序：商品已购买 > 砍价已完成
                dat3.AddRange(dat2.Where(g => g.EndDateTime < DateTime.Now && g.IsShare)
                    .OrderByDescending(g => g.IsPurchased)
                    .ThenByDescending(g => g.IsOver)
                    .Select(g => new BargainProductItem()
                    {
                        PID = g.PID,
                        ActivityProductId = g.ActivityProductId
                    }).Where(g => !dat3.Select(t => t.ActivityProductId).Contains(g.ActivityProductId)));

                result.Pager.Total = dat3.Count();
                result.Source = dat3?.Skip((PageIndex - 1) * PageSize).Take(PageSize);
            }

            return result;
        }

        public static async Task<BargainProductInfo> FetchBargainProductItemByIdKey(Guid idkey)
        {
            var readOnly = !(await GetBargainOwnerCache(idkey));
            return await DalBargain.FetchBargainProductItemByIdKey(idkey, readOnly);
        }


        /// <summary>
        /// 设置用户领取优惠券
        /// </summary>
        /// <param name="ownerId">发起人userId</param>
        /// <param name="IdKey">砍价流程id</param>
        /// <param name="apId">砍价商品pkid</param>
        /// <param name="pid">商品pid</param>
        /// <returns></returns>
        public static async Task<ShareBargainBaseResult> MarkUserReceiveCoupon(Guid ownerId, int apId, string pid,
            string deviceId)
        {
            //检查限制：活动期内，每人领取一个商品
            //用户只可购买一个商品
            if (DateTime.Now >= GlobalConstant.TyreFestivalActivityBeginTime
                && DateTime.Now <= GlobalConstant.TyreFestivalActivityEndTime)
            {
                var userBargainOwnerActionCount = await DalBargain.CheckUserBargainOwnerActionCount(ownerId,
                    GlobalConstant.TyreFestivalActivityBeginTime, GlobalConstant.TyreFestivalActivityEndTime, false);
                if (userBargainOwnerActionCount > 0)
                {
                    return new ShareBargainBaseResult()
                    {
                        Code = 9,
                        Info = "活动期内只能领取一件商品哦"
                    };
                }
            }

            //商品限制检查 
            var val = await DalBargain.CheckBargainProductStatus(ownerId, apId, pid);
            if (val == null)
            {
                return new ShareBargainBaseResult()
                {
                    Code = 4,
                    Info = "您选中的宝贝还未完成砍价哦"
                };
            }

            var parameters = new List<BuyLimitDetailModel>
            {
                new BuyLimitDetailModel
                {
                    ModuleName = "sharebargain",
                    ModuleProductId = apId.ToString(),
                    LimitObjectId = ownerId.ToString("D"),
                    ObjectType = LimitObjectTypeEnum.UserId.ToString(),
                    Reference = val.IdKey?.ToString(),
                    Remark = "优惠券砍价"
                }
            };
            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                parameters.Add(new BuyLimitDetailModel
                {
                    ModuleName = "sharebargain",
                    ModuleProductId = apId.ToString(),
                    LimitObjectId = deviceId,
                    ObjectType = LimitObjectTypeEnum.DeviceId.ToString(),
                    Reference = val.IdKey?.ToString(),
                    Remark = "优惠券砍价"
                });
            }

            switch (val.Code)
            {
                case 1:
                    val.Info = "优惠券领取成功啦";
                    break;
                case 2:
                    val.Info = "商品已下架超过24小时，无法领取";
                    break;
                case 3:
                    val.Info = "您通过本活动领取过此优惠券，无法再次领取";
                    break;
                default:
                    val.Info = "您选中的宝贝还未完成砍价哦";
                    break;
            }

            if (val.Code != 1)
                return new ShareBargainBaseResult { Code = val.Code, Info = val.Info };
            if (val.Code == 1)
            {
                var checkResult = await LimitBuyManager.SelectBuyLimitInfo(parameters.Select(g => new BuyLimitModel
                {
                    ModuleName = g.ModuleName,
                    ModuleProductId = g.ModuleProductId,
                    LimitObjectId = g.LimitObjectId,
                    ObjectType = g.ObjectType
                }).ToList());

                if (checkResult.Exists(g => g.CurrentCount > 0))
                {
                    Logger.Warn(
                        $"MarkUserReceiveCoupon-->ownerId:{ownerId:D}-->{JsonConvert.SerializeObject(checkResult)}");
                    return new ShareBargainBaseResult
                    {
                        Code = 3,
                        Info = "您通过本活动领取过此优惠券，无法再次领取"
                    };
                }
            }



            var data = await DalBargain.SelectProductInfo(apId);
            #region 接新优惠券服务
            var flag = false;
            var cacheFlag = await ConfigServiceProxy.GetOrSetRuntimeSwitchAsync("CreatePromotionNewForBigbrand");
            if (cacheFlag != null && cacheFlag.Result != null)
            {
                if (cacheFlag.Result.Value)
                    flag = true;
            }
            if (flag)
            {
                var createPromotionReponse = new CreatePromotion.Models.CreatePromotionModel
                {
                    Channel = "BargainActivity_ProductCoupon",
                    UserID = ownerId,
                    GetRuleGUID = Guid.Parse(data.PID),
                    Author = ownerId.ToString(),
                    Referer = $"砍价优惠券奖励/userid:{ownerId},apId:{apId}"
                };

                var createPromotionRes = await CreatePromotionServiceProxy.CreatePromotionNewAsync(createPromotionReponse);

                Logger.Info(
                    $"调用优惠券服务 CreatePromotion CreatePromotionNewAsync 出参：{JsonConvert.SerializeObject(createPromotionRes)} 入参：{JsonConvert.SerializeObject(createPromotionReponse)}");

                if (!createPromotionRes.Success || createPromotionRes.Result == null || createPromotionRes.Result.ErrorCode < 0)
                {
                    val.Code = 7;
                    val.Info = "优惠券发送失败";
                    return new ShareBargainBaseResult { Code = val.Code, Info = val.Info };
                    ;
                }
            }
            else
            {
                //发送优惠券给用户 

                var createPromotionReponse = new Member.Models.CreatePromotionModel
                {
                    Channel = "BargainActivity_ProductCoupon",
                    UserID = ownerId,
                    GetRuleGUID = Guid.Parse(data.PID),
                    Author = ownerId.ToString(),
                    Referer = $"砍价优惠券奖励/userid:{ownerId},apId:{apId}"
                };

                var createPromotionRes = await MemberServiceProxy.CreatePromotionNewAsync(createPromotionReponse);

                Logger.Info(
                    $"调用优惠券服务 CreatePromotionNewAsync 出参：{JsonConvert.SerializeObject(createPromotionRes)} 入参：{JsonConvert.SerializeObject(createPromotionReponse)}");

                if (!createPromotionRes.Success || createPromotionRes.Result == null || createPromotionRes.Result.ErrorCode < 0)
                {
                    val.Code = 7;
                    val.Info = "优惠券发送失败";
                    return new ShareBargainBaseResult { Code = val.Code, Info = val.Info };
                    ;
                }
            }

            var limitResult = await LimitBuyManager.AddBuyLimitInfo(parameters);
            if (limitResult.Code == 1)
            {
                Logger.Info($"MarkUserReceiveCoupon-->{limitResult.Msg}");
            }

            //标记已发送优惠券给用户
            var markRes = await DalBargain.MarkUserReceiveCoupon(ownerId, apId);
            if (!markRes)
            {
                val.Code = 8;
                val.Info = "优惠券发送失败";
                return new ShareBargainBaseResult { Code = val.Code, Info = val.Info };
                ;
            }

            #endregion
            return new ShareBargainBaseResult { Code = val.Code, Info = val.Info };
        }

        /// <summary>
        /// 用户砍价次数，某时间段内
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static async Task<int> GetUserBargainCountAtTimerange(Guid ownerId, DateTime beginTime, DateTime endTime)
        {
            var readOnly = await GetBargainOwnerCache(ownerId);
            var res = await DalBargain.GetUserBargainCountAtTimerange(ownerId, beginTime, endTime, !readOnly);
            return res;
        }

        #endregion

        #region 读写库变更记录缓存

        public static async Task SetBargainProductCache()
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.BargainProductName))
            {
                var result = await client.SetAsync("ShareBargainProductTable", true, TimeSpan.FromSeconds(10));
                if (!result.Success && result.Exception != null)
                {
                    string ex = result.Exception.Message;
                    Logger.Warn($"设置BargainProductTable是否变更缓存失败：{ex}");
                }
            }
        }

        public static async Task<bool> GetBargainProductCache()
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.BargainProductName))
            {
                var result = await client.GetAsync<bool>("ShareBargainProductTable");
                if (!result.Success && result.Exception != null)
                {
                    string ex = result.Exception.Message;
                    Logger.Warn($"获取BargainProductTable是否变更缓存失败：{ex}");
                }

                return result.Value;
            }
        }

        public static async Task SetBargainOwnerCache(Guid ownerId)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.BargainOwnerName))
            {
                var result = await client.SetAsync($"BargainOwnerAction/{ownerId:D}", true, TimeSpan.FromSeconds(10));
                if (!result.Success && result.Exception != null)
                {
                    string ex = result.Exception.Message;
                    Logger.Warn($"设置BargainOwnerAction/{ownerId}是否变更缓存失败：{ex}");
                }
            }
        }

        public static async Task<bool> GetBargainOwnerCache(Guid ownerId)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.BargainOwnerName))
            {
                var result = await client.GetAsync<bool>($"BargainOwnerAction/{ownerId:D}");
                if (!result.Success && result.Exception != null)
                {
                    string ex = result.Exception.Message;
                    Logger.Warn($"获取BargainOwnerAction/{ownerId}是否变更缓存失败：{ex}");
                }

                return result.Value;
            }
        }

        #endregion

        /// <summary>
        /// 砍价落地页 推送
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isOver"></param>
        /// <param name="apId"></param>
        /// <param name="userId"></param>
        /// <param name="idKey"></param>
        /// <returns></returns>
        public static async Task PushMessageByUserId(CurrentBargainData data, bool isOver, int apId, Guid userId, Guid idKey)
        {
            Logger.Info($"PushMessageByUserId={JsonConvert.SerializeObject(data)}& isOver={isOver} &apId ={apId} &userId={userId}&idKey={idKey.ToString()}");

            var pushLog = new PushTemplateLog()
            {
                Replacement = JsonConvert.SerializeObject(new Dictionary<string, string>()
                {
                    ["{{IdKey}}"] = idKey.ToString("D"),
                    ["{{productname}}"] = data.ProductName,
                })
            };
            int batchid = 0;

            if (data.OwnerId != userId)//其他用户帮砍
            {
                #region 推送给帮砍人  【固定 每次帮砍成功都会触发】
                var pushResult = await TemplatePushServiceProxy.PushByUserIDAndBatchIDAsync(new List<string>() { userId.ToString("D") }, (int)GlobalConstantForBargain.PushBatchid.帮砍成功, pushLog);
                #endregion

                //砍价进度 推送
                if (data.TotalCount - data.CurrentCount == 5)//砍价进度到还剩5人 
                {
                    batchid = (int)GlobalConstantForBargain.PushBatchid.砍价进度到还剩5人;
                }
                else if (data.TotalCount % 2 == 1 && data.CurrentCount == (data.TotalCount + 1) / 2)//砍价进度到50%
                {
                    batchid = (int)GlobalConstantForBargain.PushBatchid.砍价进度到百分之50;
                }
                else if (data.TotalCount % 2 == 0 && data.CurrentCount == (data.TotalCount) / 2)//砍价进度到50% 
                {
                    batchid = (int)GlobalConstantForBargain.PushBatchid.砍价进度到百分之50;
                }
                else if (isOver)//砍价成功
                {
                    batchid = (int)GlobalConstantForBargain.PushBatchid.砍价成功;
                }

                //推送给发起人
                if (batchid > 0)
                {
                    pushResult = await TemplatePushServiceProxy.PushByUserIDAndBatchIDAsync(new List<string>() { data.OwnerId.ToString("D") }, batchid, pushLog);
                }

                #region 库存不足 推送
                if ((data.TotalStockCount >= 10 && data.CurrentStockCount == 3) || data.CurrentStockCount == 0)
                {
                    if (data.CurrentStockCount == 0)
                    {
                        batchid = (int)GlobalConstantForBargain.PushBatchid.库存不足等于0;
                    }
                    else
                    {
                        batchid = (int)GlobalConstantForBargain.PushBatchid.库存不足等于3;
                    }
                    List<CurrentBargainData> bargainOwnerActionList = await GetValidBargainOwnerActionsByApidsAsync(apId, data.BeginDateTime, data.EndDateTime, 1, 0);
                    foreach (var boa in bargainOwnerActionList)
                    {
                        var pushTemplateLog = new PushTemplateLog()
                        {
                            Replacement = JsonConvert.SerializeObject(new Dictionary<string, string>()
                            {
                                ["{{IdKey}}"] = boa?.IdKey.ToString("D"),
                                ["{{productname}}"] = data.ProductName,
                            })
                        };
                        pushResult = await TemplatePushServiceProxy.PushByUserIDAndBatchIDAsync(new List<string>() { boa.OwnerId.ToString("D") }, batchid, pushTemplateLog);
                    }
                }
                #endregion
            }
        }

        public static async Task<bool> SetShareBargainStatus(Guid IdKey)
            => await DalBargain.SetShareBargainStatus(IdKey);

        #region [首页模块数据查询]

        /// <summary>
        ///     获取砍价首页商品
        /// </summary>
        /// <returns></returns>
        public static async Task<List<SimpleBargainProduct>> GetBargainProductForIndex()
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ShareBargainName))
            {
                var cacheResult =
                    await client.GetOrSetAsync("Index", () => GetBargainProductForIndexReal(), TimeSpan.FromHours(24));
                if (cacheResult.Success && cacheResult.Value.Any())
                {
                    return cacheResult.Value;
                }

                Logger.Warn($"分享砍价首页模块数据从缓存获取失败-->{cacheResult.RealKey}");
                return await GetBargainProductForIndexReal();
            }
        }

        /// <summary>
        ///     获取砍价首页商品
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private static async Task<List<SimpleBargainProduct>> GetBargainProductForIndexReal(int count = 2)
        {
            var result = new List<SimpleBargainProduct>();
            var data = await DalBargain.GetBargainProductForIndex();
            if (data.Any())
            {
                var countInfo =
                    await DalBargain.GetCurrentBargainCount(data.Select(g => g.ActivityProductId).ToList());
                using (var client = new ProductClient())
                {
                    var productInfo =
                        await client.SelectSkuProductListByPidsAsync(data.Select(g => g.Pid).Distinct().ToList());
                    if (productInfo.Success && productInfo.Result.Any())
                    {
                        foreach (var item in data)
                        {
                            item.CurrentCount =
                                countInfo.FirstOrDefault(g => g.Item1 == item.ActivityProductId)?.Item2 ?? 0;
                            var productItem = productInfo.Result.FirstOrDefault(g => g.Pid == item.Pid);
                            item.OnSale = productItem?.Onsale ?? false;
                            item.ImageUrl =
                                productItem?.ImageUrls?.FirstOrDefault(g => !string.IsNullOrWhiteSpace(g)) ??
                                "";
                        }
                    }
                }
                result = data.Where(g => g.OnSale).Take(count).ToList();
            }

            return result;
        }

        #endregion

        #region 砍价重构
        /// <summary>
        ///     首页获取砍价产品列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<PagedModel<BargainProductItem>> SelectBargainProductList(int pageIndex, int pageSize)
        {
            var key = $"BargainHome/{pageIndex}/{pageSize}";
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ShareBargainName))
            {
                var cacheResult = await client.GetOrSetAsync(key,
                    () => DalBargain.SelectBargainProductList(pageIndex, pageSize), TimeSpan.FromMinutes(1));
                if (cacheResult.Success && cacheResult.Value != null)
                {
                    return cacheResult.Value;
                }
                else
                {
                    Logger.Warn($"SelectBargainProductList-->获取Redis缓存失败-->{key}");
                    return await DalBargain.SelectBargainProductList(pageIndex, pageSize);
                }
            }
        }

        /// <summary>
        ///分页获取用户的砍价记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<PagedModel<BargainHistoryModel>> SelectBargainHistory(int pageIndex, int pageSize,
            Guid userId)
        {
            var data = await DalBargain.SelectBargainHistory(pageIndex, pageSize, userId);
            var activityProducts = data.Source.Where(g => !string.IsNullOrWhiteSpace(g.Pid) && g.ActivityProductId > 0)
                .Select(g =>
                    new BargainProductItem
                    {
                        ActivityProductId = g.ActivityProductId,
                        PID = g.Pid
                    })
                .ToList();
            var source = data.Source?.ToList() ?? new List<BargainHistoryModel>();
            if (activityProducts.Any())
            {
                var productInfo = await GetOrSetBargainHash(activityProducts);
                var userBargainData =
                    await GetBargainUserInfo(userId, activityProducts.Select(g => g.ActivityProductId).ToList());
                foreach (var item in source)
                {
                    var product = productInfo.FirstOrDefault(g => g.ActivityProductId == item.ActivityProductId);
                    var info = userBargainData.FirstOrDefault(g => g.ActivityProductId == item.ActivityProductId);
                    if (product != null)
                    {
                        item.ActivityPrice = product.ActivityPrice;
                        item.BeginTime = product.BeginTime;
                        item.CurrentCount = product.CurrentCount;
                        item.EndTime = product.EndTime;
                        item.ImageUrl = product.ImageUrl;
                        item.OnSale = product.OnSale;
                        item.OriginPrice = product.OriginPrice;
                        item.Pid = product.Pid;
                        item.ProductName = product.ProductName;
                        item.ProductType = product.ProductType;
                        item.RequiredTimes = product.RequiredTimes;
                        item.CurrentStockCount = product.CurrentStockCount;
                        item.ActivityId = GlobalConstant.BargainActivityId;
                        item.FinalTime = item.EndTime.AddHours(24);
                        item.IsOwner = info?.IsOwner ?? false;
                        //item.CurrentCount = info.CurrentCount;
                        item.IsShared = info?.IsShared ?? false;
                        item.IdKey = info?.IdKey ?? Guid.Empty;
                        item.SimpleDisplayName = product.SimpleDisplayName;
                        item.AppShareID = product.AppShareID;
                        item.WXShareTitle = product.WXShareTitle;
                        if (item.BargainStatus == 0)
                        {
                            if (item.EndTime < DateTime.Now)
                            {
                                item.BargainStatus = 2;
                            }
                            else if (item.CurrentStockCount < 1)
                            {
                                item.BargainStatus = 3;
                            }
                        }

                        if (item.BargainStatus == 1 && item.FinalTime < DateTime.Now)
                        {
                            item.BargainStatus = 3;
                        }
                    }
                }
            }

            data.Source = source.GroupBy(g => g.IdKey,
                (k, v) => v.OrderByDescending(t => t.Reduce)
                .FirstOrDefault())
                .Where(g => g != null && !string.IsNullOrWhiteSpace(g.Pid)).ToList();
            return data;
        }

        public static async Task<List<SliceShowInfoModel>> GetSliceShowInfo(int count)
        {
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ShareBargainName))
            {
                var cacheResult = await cacheClient.GetOrSetAsync(GlobalConstant.ShareBargainSliceShowKey,
                    () => GetSliceShowInfoReal(count), TimeSpan.FromHours(1));
                if (cacheResult.Success && cacheResult.Value.Any())
                {
                    return cacheResult.Value.OrderByDescending(g => g.FinishTime).Take(count).ToList();
                }
                else
                {
                    return await GetSliceShowInfoReal(count);
                }
            }
        }

        private static async Task<List<SliceShowInfoModel>> GetSliceShowInfoReal(int count)
        {
            var result = await DalBargain.GetSliceShowInfo(count);
            var sliceShowText = (await GetBargainGlobalConfig())?.SliceShowText;
            result.ForEach(g => g.SliceShowText = sliceShowText);
            return result;
        }

        private static async Task<bool> AddSliceShowInfo(SliceShowInfoModel data)
        {
            var originalData = await GetSliceShowInfo(10);
            if (originalData?.FirstOrDefault(g => g.UserId == data.UserId && g.ProductName == data.ProductName) != null)
            {
                Logger.Warn($"AddSliceShowInfo-->轮播信息已包含{data.UserId}/{data.ProductName}");
                return false;
            }

            data.SliceShowText = (await GetBargainShareConfig())?.SliceShowText;
            originalData.Add(data);
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ShareBargainName))
            {
                var cacheResult = await cacheClient.SetAsync(GlobalConstant.ShareBargainSliceShowKey, originalData,
                    TimeSpan.FromDays(1));
                return cacheResult.Success;
            }
        }

        /// <summary>
        /// 获取用户砍价商品详情
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productItems"></param>
        /// <returns></returns>
        public static async Task<List<BargainProductNewModel>> GetBargsinProductDetail(Guid userId,
            List<BargainProductItem> productItems)
        {
            var productInfo = await GetOrSetBargainHash(productItems);
            if (productInfo.Any() && userId != Guid.Empty)
            {
                var userBargainData =
                    await GetBargainUserInfo(userId, productItems.Select(g => g.ActivityProductId).ToList());
                if (userBargainData.Any())
                {
                    foreach (var item in productInfo)
                    {
                        var info = userBargainData.FirstOrDefault(g => g.ActivityProductId == item.ActivityProductId);
                        if (info != null)
                        {
                            item.BargainStatus = info.BargainStatus;
                            item.StartTime = info.StartTime;
                            item.Reduce = info.Reduce;
                            item.IsFinish = info.IsFinish;
                            item.IsOwner = info.IsOwner;
                            item.IsShared = info.IsShared;
                            item.IdKey = info.IdKey;
                            //item.CurrentCount = info.CurrentCount;
                            if (item.BargainStatus == 0)
                            {
                                if (item.EndTime < DateTime.Now)
                                {
                                    item.BargainStatus = 2;
                                }
                                else if (item.CurrentStockCount < 1)
                                {
                                    item.BargainStatus = 3;
                                }
                            }

                            if (item.BargainStatus == 1 && item.FinalTime < DateTime.Now)
                            {
                                item.BargainStatus = 2;
                            }

                        }
                    }
                }
            }

            return productInfo;
        }

        /// <summary>
        ///     用户创建砍价并砍价
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="apId"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static async Task<CreateBargainResult> CreateUserBargain(Guid userId, int apId, string pid, bool isPush = true)
        {
            using (var zklock = new ZooKeeperLock($"{userId:D}/{apId}"))
            {
                // zk 锁
                if (await zklock.WaitAsync(5000))
                {
                    var productIDs = new List<string>();
                    productIDs.Add(apId.ToString());
                    //砍价产品数据
                    var productData = await DalBargain.SelectBargainProductInfo(productIDs);
                    // 检查用户砍价数据
                    var checkResult = await DalBargain.CheckUserBargainRecord(userId, pid, apId);
                    var result = new CreateBargainResult
                    {
                        Code = 1,
                        Info = "创建成功~",
                        OwnerId = userId,
                        IdKey = Guid.Empty,
                        SimpleDisplayName = checkResult.FirstOrDefault()?.Item2 ?? default(string),
                        HelpCutPriceTimes = productData.FirstOrDefault()?.HelpCutPriceTimes ?? 0
                    };
                    if (productData == null || !productData.Any())
                    {
                        result.Code = 5;
                        result.Info = "商品还未上架~";
                        return result;
                    }

                    if (checkResult.Any())
                    {
                        result.Code = 2;
                        result.Info = "已发起过该商品砍价";
                        result.IdKey = checkResult.FirstOrDefault()?.Item1 ?? default(Guid);
                        result.RemnantMoney = checkResult.FirstOrDefault()?.Item3 ?? default(decimal);
                        result.Reduce = checkResult.Sum(g => g.Item4);//checkResult.FirstOrDefault()?.Item4 ?? default(decimal);
                        result.IsShared = checkResult.Count > 1;
                    }

                    // 检查砍价商品
                    var data = await DalBargain.CheckBargainProduct(apId, pid, false);
                    if (data == 3)
                    {
                        result.Code = 3;
                        result.Info = "手慢啦，本商品砍价已结束~";
                    }
                    else if (data == 4)
                    {
                        result.Code = 4;
                        result.Info = "手慢啦，砍价商品已抢完~";
                    }
                    else if (data == 5)
                    {
                        result.Code = 5;
                        result.Info = "商品还未上架~";
                    }
                    else if (result.Code == 1)
                    {
                        // 创建用户砍价数据
                        var createResult = await DalBargain.AddShareBargain(userId, apId, pid, true);
                        if (createResult.Code == 1 && createResult.IdKey != null)
                        {
                            await SetBargainOwnerCache(userId);
                            await SetBargainOwnerCache(createResult.IdKey.Value);
                            var bargainData = await DalBargain.FetchCurrentBargainData(createResult.IdKey.Value, false);
                            // 创建用户帮忙砍价数据
                            var bargainResult =
                                await CreateUserBargainRecord(bargainData, createResult.IdKey.Value, userId, apId);
                            result.SimpleDisplayName = bargainData.SimpleDisplayName;
                            if (bargainResult.Code == 0)
                            {
                                result.Code = 0;
                                result.Info = "服务器异常";
                            }
                            else
                            {

                                result.Code = 1;
                                result.IdKey = createResult.IdKey.Value;
                                result.Reduce = bargainResult.Reduce;
                                result.Info = bargainResult.Info;
                                result.Rate = bargainResult.Rate;
                                result.RemnantMoney = bargainResult.RemnantMoney;

                                #region 砍价推送落地页 - 发起砍价
                                if (isPush)
                                {
                                    var userids = new List<string>() { userId.ToString("D") };
                                    var pushResult = await TemplatePushServiceProxy.PushByUserIDAndBatchIDAsync(userids, (int)GlobalConstantForBargain.PushBatchid.发起砍价, new PushTemplateLog()
                                    {
                                        Replacement = JsonConvert.SerializeObject(new Dictionary<string, string>()
                                        {
                                            ["{{IdKey}}"] = createResult.IdKey.Value.ToString("D"),
                                            ["{{productname}}"] = bargainData.ProductName,
                                        })
                                    });
                                }

                                #endregion

                            }
                        }
                        else
                        {
                            result.Code = 0;
                            result.Info = "服务器异常";
                        }
                    }
                    return result;
                }
                else
                {
                    Logger.Error("砍价等待链接超时");
                    return new CreateBargainResult
                    {
                        Code = 0,
                        Info = "服务器异常"
                    };
                }
            }
        }

        /// <summary>
        /// 从hash缓存获取砍价商品配置信息、参与人数
        /// </summary>
        /// <param name="productItems"></param>
        /// <returns></returns>
        private static async Task<List<BargainProductNewModel>> GetOrSetBargainHash(
            List<BargainProductItem> productItems)
        {
            using (var hashClient =
                CacheHelper.CreateHashClient(GlobalConstant.ShareBargainHashKey, TimeSpan.FromHours(24)))
            {
                var keyList = productItems.Select(g => $"{g.ActivityProductId}/{g.PID}").ToList();
                var cacheResult = await hashClient.GetAsync<BargainProductNewModel>(keyList);
                var result = new List<BargainProductNewModel>();
                List<string> unfoundKey;
                if (cacheResult.Success && cacheResult.Value != null)
                {
                    unfoundKey = keyList.Except(cacheResult.Value.Keys)?.Select(g => g.Split('/').First())?.ToList() ??
                                 new List<string>();
                    result = cacheResult.Value.Values.ToList();
                }
                else
                {
                    unfoundKey = productItems.Select(g => g.ActivityProductId.ToString()).ToList();
                    Logger.Warn($"GetOrSetBargainHash==>fail==>{string.Join("/", unfoundKey)}");
                }

                if (unfoundKey.Any())
                {
                    var data = await SelectBargainProductInfo(unfoundKey);
                    if (data.Any())
                    {
                        result.AddRange(data);
                        var dic = new Dictionary<string, object>();
                        data.ForEach(g => dic.Add($"{g.ActivityProductId}/{g.Pid}", g));
                        var cacheData = new ReadOnlyDictionary<string, object>(dic);
                        await hashClient.SetAsync<BargainProductNewModel>(cacheData);
                    }
                }

                return result;
            }
        }

        private static async Task<List<BargainProductNewModel>> SelectBargainProductInfo(List<string> ids)
        {
            var result = new List<BargainProductNewModel>();
            if (!(bool)ids?.Any())
            {
                return result;
            }

            try
            {
                result = await DalBargain.SelectBargainProductInfo(ids);
                if (!result.Any())
                {
                    return result;
                }

                var globalConfig = await GetBargainShareConfig();
                var countInfo =
                    await DalBargain.GetCurrentBargainCount(result.Select(g => g.ActivityProductId).ToList());
                result.ForEach(t =>
                {
                    t.CurrentCount = countInfo.FirstOrDefault(g => g.Item1 == t.ActivityProductId)?.Item2 ?? 0;
                    t.AppShareID = string.IsNullOrWhiteSpace(t.AppShareID) ? globalConfig.AppDetailShareTag : t.AppShareID;
                    t.WXShareTitle = string.IsNullOrWhiteSpace(t.WXShareTitle)
                        ? globalConfig.WXAPPDetailShareText
                        : t.WXShareTitle;
                });

                using (var client = new ProductClient())
                {
                    var productInfo =
                        await client.SelectSkuProductListByPidsAsync(result.Select(g => g.Pid).Distinct().ToList());
                    if (productInfo.Success && productInfo.Result.Any())
                    {
                        foreach (var item in result)
                        {
                            var productItem = productInfo.Result.FirstOrDefault(g => g.Pid == item.Pid);
                            item.OnSale = productItem?.Onsale ?? false;
                            item.ProductName = item.ProductName ?? productItem?.DisplayName;
                            if (string.IsNullOrWhiteSpace(item.ImageUrl))
                            {
                                var img = productItem?.ImageUrls?.FirstOrDefault();
                                if (!string.IsNullOrWhiteSpace(img))
                                {
                                    item.ImageUrl = ImageHelper.GetImageUrl(img, 300, 300);
                                }
                            }

                        }
                    }
                }
                result.ForEach(g => g.ActivityId = GlobalConstant.BargainActivityId);
            }
            catch (Exception ex)
            {
                Logger.Error($"SelectBargainProductInfo异常，ex：{ex}");
            }
            return result;
        }

        /// <summary>
        /// 创建用户帮忙砍价数据
        /// </summary>
        /// <param name="bargainInfo"></param>
        /// <param name="idKey"></param>
        /// <param name="userId"></param>
        /// <param name="apId"></param>
        /// <returns></returns>
        private static async Task<BargainResult> CreateUserBargainRecord(CurrentBargainData bargainInfo, Guid idKey, Guid userId, int apId)
        {
            var result = new BargainResult();

            try
            {
                //计算本次砍去金额
                var val = ComputeCutAmount(bargainInfo);

                //是否砍价完成：最后一次
                var isover = bargainInfo.TotalCount - bargainInfo.CurrentCount == 1;

                //创建帮砍数据
                var createdResult = await DalBargain.AddBargainAction(userId, apId, bargainInfo.PKID, val, isover,
                    bargainInfo.CurrentCount + 1, bargainInfo.CurrentRedece + val);
                if (createdResult)
                {
                    //数据库更新成功 改变数据
                    bargainInfo.CurrentCount++;
                    bargainInfo.IsOver = isover;
                    if (isover)//帮砍成功，当前库存 -1
                    {
                        bargainInfo.CurrentStockCount--;
                    }
                    Logger.Info($"CreateUserBargainRecord => data.CurrentCount={ bargainInfo.CurrentCount}&data.IsOver={bargainInfo.IsOver}&data.CurrentStockCount={bargainInfo.CurrentStockCount}");

                    result.Code = 1;
                    result.Reduce = val;
                    result.Rate = bargainInfo.Average > 0 ? (double)(val / bargainInfo.Average) : 1;
                    result.Info = bargainInfo.SuccessfulHint.Replace(@"{{ReducePrice}}", val.ToString("#0.00"));
                    result.RemnantMoney = bargainInfo.OriginalPrice - bargainInfo.FinalPrice - bargainInfo.CurrentRedece - val;
                    result.SimpleDisplayName = bargainInfo.SimpleDisplayName;
                    result.Setp = bargainInfo.CurrentCount;
                    //刷新缓存
                    await SetShareBargainCache(new List<BargainProductItem>
                    {
                        new BargainProductItem {ActivityProductId = apId, PID = bargainInfo.PID}
                    });

                    await SetBargainOwnerCache(bargainInfo.OwnerId);
                    await SetBargainOwnerCache(idKey);
                    //砍价推送落地页 - 砍价进度 & 砍价成功 & 库存不足  【触发条件=》其他用户帮砍成功推送】
                    //await PushMessageByUserId(data, isover, apId,userId,idKey);
                    if (bargainInfo.OwnerId != userId)
                    {
                        Logger.Info($"CreateUserBargainRecord success =>userId={userId} 触发 ShareBargainPush MQ ");
                        TuhuNotification.SendNotification("notification.ShareBargainPush", new
                        {
                            currentBargainData = bargainInfo,
                            isOver = isover,
                            apId,
                            userId,
                            idKey
                        }, 5000);//加延时时间，存在读写延时问题
                    }
                    if (bargainInfo.CurrentStockCount == 1)
                    {
                        await SetBargainProductCache();
                    }

                    if (isover)
                    {
                        //砍价成功轮播信息
                        await AddSliceShowInfo(new SliceShowInfoModel
                        {
                            UserId = userId,
                            ProductName = bargainInfo.ProductName,
                            SimpleDisplayName = bargainInfo.SimpleDisplayName,
                            FinishTime = DateTime.Now
                        });
                    }
                }
                else
                {
                    result.Code = 0;
                    Logger.Warn($"CreateUserBargainRecord,创建帮砍数据失败,userid:{userId.ToString()},apid:{apId}");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"CreateUserBargainRecord,创建帮砍数据异常,userid:{userId.ToString()},apid:{apId},ex:{ex}");
            }

            return result;
        }

        /// <summary>
        /// 计算砍价金额
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static decimal ComputeCutAmount(CurrentBargainData data)
        {
            decimal resultAmount = 0;
            var remainCount = 0;
            decimal remainReduce = 0;

            //有设置前N个人砍M%目标价
            if (data.BigCutBeforeCount > 0 && data.BigCutPriceRate > 0)
            {
                //前N个人还剩余的砍价次数
                remainCount = data.BigCutBeforeCount - data.CurrentCount;
                //前M%金额还剩余的砍价金额
                remainReduce = (decimal)data.BigCutPriceRate / 100 * (data.OriginalPrice - data.FinalPrice) - data.CurrentRedece;
            }

            //没有设置前N个人砍M%目标价时,或者前N个人砍M%目标价的金额或次数已没有
            if (!(remainCount > 0 && remainReduce > 0))
            {
                //剩余的砍价次数
                remainCount = data.TotalCount - data.CurrentCount;
                //剩余的砍价金额
                remainReduce = data.OriginalPrice - data.FinalPrice - data.CurrentRedece;
            }

            //计算金额
            if (remainCount > 0 && remainReduce > 0)
            {
                if (remainCount == 1)//最后一次砍完
                {
                    resultAmount = remainReduce;
                }
                else
                {
                    Random random = new Random();
                    var rd = random.NextDouble() + 0.5;
                    resultAmount = decimal.Multiply(remainReduce / remainCount, (decimal)rd);
                    resultAmount = decimal.Round(resultAmount, 2, MidpointRounding.AwayFromZero);
                }
            }
            return resultAmount;
        }

        /// <summary>
        ///     获取用户砍价数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="apIds"></param>
        /// <returns></returns>
        private static async Task<List<BargainProductNewModel>> GetBargainUserInfo(Guid userId, List<int> apIds)
        {
            var userInfo = await DalBargain.GetBargainUserInfo(userId, apIds, false);
            var hasShared = await DalBargain.GetUserBargainCount(userId, apIds, false);
            if (userInfo.Any() && hasShared.Any())
            {
                foreach (var item in userInfo)
                {
                    item.IsOwner = true;
                    item.IsShared = hasShared.FirstOrDefault(g => g.ActivityProductId == item.ActivityProductId)
                                        ?.IsShared ?? false;
                    //item.CurrentCount = item.IsShared && item.CurrentCount > 1 ? item.CurrentCount - 1 : item.CurrentCount;
                }
            }

            return userInfo;
        }
        /// <summary>
        /// 获取砍价配置
        /// </summary>
        /// <returns></returns>
        private static async Task<BargainGlobalConfigModel> GetBargainGlobalConfig()
        {
            var data = await DalBargain.GetBackgroundStyle();
            if (data == null)
            {
                Logger.Warn("获取分享砍价活动全局配置失败");
                return null;
            }

            List<BargainRules> descJsonStu = JsonConvert.DeserializeObject<List<BargainRules>>(data.QAdATA);
            var result = new BargainGlobalConfigModel()
            {
                Style = data.Style,
                ImgUrl = data.ImgUrl,
                RulesCount = data.RulesCount,
                Title = data.Title,
                BargainRule = descJsonStu,
                WXAPPDetailShareText = data.WXAPPDetailShareText,
                WXAPPListShareImg = data.WXAPPListShareImg,
                WXAPPListShareText = data.WXAPPListShareText,
                APPListShareTag = data.APPListShareTag,
                AppDetailShareTag = data.AppDetailShareTag,
                SliceShowText = data.SliceShowText
            };
            return result;
        }

        /// <summary>
        ///     受邀人获取砍价结果
        /// </summary>
        /// <param name="idKey"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<InviteeBarginInfo> GetInviteeBargainInfo(Guid idKey, Guid userId)
        {
            var info = await FetchBargainProductItemByIdKey(idKey);
            var result = new InviteeBarginInfo();
            if (info == null)
            {
                Logger.Warn($"未找到{idKey:D}对应的砍价分享");
                return result;
            }

            result.PId = info.PID;
            result.ActivityProductId = info.ActivityProductId;
            if (info.OwnerId == userId)
            {
                result.IsOwner = true;
                result.BargainSucceed = true;
                result.InviteeIdkey = idKey;
            }
            else
            {
                var readOnly = !(await GetBargainOwnerCache(idKey));
                var readOnly2 = !(await GetBargainOwnerCache(userId));
                var intradayBargainCount =
                    await GetUserBargainCountAtTimerange(userId, DateTime.Now.Date, DateTime.Now.AddDays(10));
                var bargainSucceed = await DalBargain.CheckInviteeBargainResult(idKey, userId, readOnly);
                var inviteeIdkey = await DalBargain.GetInviteeIdKey(info.ActivityProductId, userId, readOnly2);
                result.IsOwner = false;
                result.BargainSucceed = bargainSucceed;
                result.IntradayBargainCount = intradayBargainCount;
                result.InviteeIdkey = inviteeIdkey;
            }

            //测试账号砍价次数返回0
            if (TestUserIdsConst.Contains(userId))
            {
                result.IntradayBargainCount = 0;
            }

            return result;
        }

        #endregion

        /// <summary>
        /// 获取未完成的 发起砍价记录
        /// </summary>
        /// <param name="apId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="status"></param>
        /// <param name="IsOver"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public static async Task<List<CurrentBargainData>> GetValidBargainOwnerActionsByApidsAsync(int apId, DateTime startDate, DateTime endDate, int status, int IsOver, bool readOnly = true)
        {
            var data = await DalBargain.GetValidBargainOwnerActionsByApidsAsync(apId, startDate, endDate, 1, 0);
            return data;
        }


        /// <summary>
        /// 获取砍价的配置  【时间】
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static async Task<List<BargainProductNewModel>> SelectBargainProductsByDateAsync(DateTime startDate, DateTime endDate)
        {
            var data = await DalBargain.SelectBargainProductsByDateAsync(startDate, endDate);
            return data;
        }

        #region 砍价标准版

        /// <summary>
        /// 用户发起砍价并自砍:验证黑名单,砍价状态,推送
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<CreateBargainResult> CreateBargainAndCutSelfAsync(CreateBargainAndCutSelfRequest request)
        {
            //验证黑名单
            var blackRequest = new BargainBlackListRequest()
            {
                UserId = request.UserId,
                DeviceId = request.DeviceId,
                Mobile = request.Mobile,
                Ip = request.Ip
            };
            var blackResult = CheckBlackList(blackRequest, "CreateBargainAndCutSelfAsync");
            if (!blackResult.Item1)
            {
                return new CreateBargainResult { Code = 6, Info = blackResult.Item2 };
            }

            var result = new CreateBargainResult();
            try
            {
                using (var zklock = new ZooKeeperLock($"{request.UserId:D}/{request.ActivityProductId}"))
                {
                    // zk 锁
                    if (await zklock.WaitAsync(5000))
                    {
                        // 检查用户是否发起过该砍价
                        var checkResult = await DalBargain.CheckUserBargainRecord(request.UserId, request.Pid, request.ActivityProductId);
                        result.SimpleDisplayName = checkResult.FirstOrDefault()?.Item2;//返回商品简称
                        result.OwnerId = request.UserId;

                        //检查砍价配置数据
                        var data = await DalBargain.CheckBargainProduct(request.ActivityProductId, request.Pid, false);
                        if (data != 1)
                        {
                            if (data == 3)
                            {
                                result.Code = 3;
                                result.Info = "手慢啦，本商品砍价已结束~";
                            }
                            else if (data == 4)
                            {
                                result.Code = 4;
                                result.Info = "手慢啦，砍价商品已抢完~";
                            }
                            else if (data == 5 || data == 0)
                            {
                                result.Code = 5;
                                result.Info = "商品还未上架~";
                            }
                            return result;
                        }
                        else
                        {
                            if (checkResult.Any())
                            {
                                result.Code = 2;
                                result.SimpleDisplayName = checkResult.FirstOrDefault()?.Item2;
                                result.Info = "已发起过该商品砍价";
                                result.IdKey = checkResult.FirstOrDefault()?.Item1 ?? default(Guid);
                                result.RemnantMoney = checkResult.FirstOrDefault()?.Item3 ?? default(decimal);
                                result.Reduce = checkResult.Sum(g => g.Item4);
                                result.IsShared = checkResult.Count > 1;
                                return result;
                            }

                            // 创建用户发起砍价数据
                            var createResult = await DalBargain.AddShareBargain(request.UserId, request.ActivityProductId, request.Pid, true);
                            if (createResult.Code == 1 && createResult.IdKey != null)
                            {
                                await SetBargainOwnerCache(request.UserId);
                                await SetBargainOwnerCache(createResult.IdKey.Value);

                                //获取发起砍价数据
                                var bargainData = await DalBargain.FetchCurrentBargainData(createResult.IdKey.Value, false);
                                if (bargainData == null)
                                {
                                    result.Code = 7;
                                    result.Info = "服务器异常";
                                    Logger.Warn($"CreateBargainAndCutSelfAsync帮砍成功后获取数据失败,userid:{request.UserId},apid:{request.ActivityProductId}");
                                    return result;
                                }

                                //创建用户帮忙砍价数据(自砍)
                                var bargainResult =
                                    await CreateUserBargainRecord(bargainData, createResult.IdKey.Value, request.UserId, request.ActivityProductId);
                                if (bargainResult.Code == 0)
                                {
                                    Logger.Warn($"CreateBargainAndCutSelfAsync用户发起砍价成功但自砍失败,userid:{request.UserId},apid:{request.ActivityProductId}");
                                    result.Code = 8;
                                    result.Info = "服务器异常";
                                }
                                else
                                {
                                    result = new CreateBargainResult
                                    {
                                        Code = 1,
                                        Info = bargainResult.Info,
                                        OwnerId = request.UserId,
                                        IdKey = createResult.IdKey.Value,
                                        Reduce = bargainResult.Reduce,
                                        Rate = bargainResult.Rate,
                                        RemnantMoney = bargainResult.RemnantMoney,
                                        SimpleDisplayName = bargainData.SimpleDisplayName,
                                        HelpCutPriceTimes = bargainData.HelpCutPriceTimes
                                    };

                                    #region 砍价推送落地页 - 发起砍价
                                    try
                                    {
                                        if (request.IsPush)
                                        {
                                            var userids = new List<string>() { request.UserId.ToString("D") };
                                            var pushResult = await TemplatePushServiceProxy.PushByUserIDAndBatchIDAsync(userids, (int)GlobalConstantForBargain.PushBatchid.发起砍价, new PushTemplateLog()
                                            {
                                                Replacement = JsonConvert.SerializeObject(new Dictionary<string, string>()
                                                {
                                                    ["{{IdKey}}"] = createResult.IdKey.Value.ToString("D"),
                                                    ["{{productname}}"] = bargainData.ProductName,
                                                })
                                            });
                                        }
                                    }
                                    catch (Exception exce)
                                    {
                                        Logger.Warn($"CreateBargainAndCutSelfAsync砍价推送落地页异常,IdKey:{createResult.IdKey.Value.ToString("D")},ex:{exce}");
                                    }
                                    #endregion

                                }
                            }
                            else
                            {
                                result.Code = 0;
                                result.Info = "服务器异常";
                                Logger.Warn($"CreateBargainAndCutSelfAsync用户发起砍价失败,userid:{request.UserId},apid:{request.ActivityProductId}");
                            }
                        }
                    }
                    else
                    {
                        Logger.Error("砍价等待链接超时");
                        result.Code = 0;
                        result.Info = "服务器异常";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"CreateBargainAndCutSelfAsync用户发起砍价异常，userid:{request.UserId},apid:{request.ActivityProductId},ex{ex}");
            }
            return result;
        }

        /// <summary>
        /// 检查用户是否可购买砍价商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<ShareBargainBaseResult> CheckBargainProductBuyStatusAsync(CheckBargainProductBuyStatusRequest request)
        {
            var result = new BargainShareResult();
            try
            {
                //验证砍价黑名单
                var blackRequest = new BargainBlackListRequest()
                {
                    UserId = request.OwnerId,
                    DeviceId = request.DeviceId,
                    Mobile = request.Mobile,
                    Ip = request.Ip
                };
                var blackResult = CheckBlackList(blackRequest, "CheckBargainProductBuyStatusAsync");
                if (!blackResult.Item1)
                {
                    return new ShareBargainBaseResult { Code = -1, Info = blackResult.Item2 };
                }

                //用户只可购买一个商品
                if (DateTime.Now >= GlobalConstant.TyreFestivalActivityBeginTime
                    && DateTime.Now <= GlobalConstant.TyreFestivalActivityEndTime)
                {
                    var userBargainOwnerActionCount = await DalBargain.CheckUserBargainOwnerActionCount(request.OwnerId,
                        GlobalConstant.TyreFestivalActivityBeginTime, GlobalConstant.TyreFestivalActivityEndTime, false);
                    if (userBargainOwnerActionCount > 0)
                    {
                        return new ShareBargainBaseResult()
                        {
                            Code = 7,
                            Info = "活动期内只能领取一件商品哦"
                        };
                    }
                }

                result = await DalBargain.CheckBargainProductStatus(request.OwnerId, request.ActivityProductId, request.Pid);
                if (result == null)
                {
                    return new ShareBargainBaseResult()
                    {
                        Code = 4,
                        Info = "您选中的宝贝还未完成砍价哦"
                    };
                }

                var parameters = new List<BuyLimitModel>
            {
                new BuyLimitModel
                {
                    ModuleName = "sharebargain",
                    ModuleProductId = request.ActivityProductId.ToString(),
                    LimitObjectId = request.OwnerId.ToString("D"),
                    ObjectType = LimitObjectTypeEnum.UserId.ToString()
                }
            };
                if (!string.IsNullOrWhiteSpace(request.DeviceId))
                {
                    parameters.Add(new BuyLimitModel
                    {
                        ModuleName = "sharebargain",
                        ModuleProductId = request.ActivityProductId.ToString(),
                        LimitObjectId = request.DeviceId,
                        ObjectType = LimitObjectTypeEnum.DeviceId.ToString(),
                    });
                }
                else
                {
                    return new ShareBargainBaseResult
                    {
                        Code = 4,
                        Info = "请填写设备号"
                    };
                }

                switch (result.Code)
                {
                    case 1:
                        result.Info = "可购买";
                        break;
                    case 2:
                        result.Info = "商品已下架超过24小时，无法购买";
                        break;
                    case 3:
                        result.Info = "您通过本活动购买过此商品，无法再次购买";
                        break;
                    default:
                        result.Info = "您选中的宝贝还未完成砍价哦";
                        break;
                }

                if (result.Code == 1)
                {
                    var checkResult = await LimitBuyManager.SelectBuyLimitInfo(parameters);

                    if (checkResult.Exists(g => g.CurrentCount > 0))
                    {
                        result.Code = 3;
                        result.Info = "您已通过本活动购买过此商品，无法再次购买";
                        Logger.Warn(
                            $"CheckBargainProductStatus-->ownerId:{request.OwnerId:D}-->{JsonConvert.SerializeObject(checkResult)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"CheckBargainProductBuyStatusAsync异常owerid:{request.OwnerId},apid:{request.ActivityProductId},ex:{ex}");
            }

            return new ShareBargainBaseResult
            {
                Code = result.Code,
                Info = result.Info
            };
        }

        /// <summary>
        /// 用户领取砍价优惠券
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<ShareBargainBaseResult> ReceiveBargainCouponAsync(ReceiveBargainCouponRequest request)
        {
            var result = new BargainShareResult();
            try
            {
                //验证砍价黑名单
                var blackRequest = new BargainBlackListRequest()
                {
                    UserId = request.OwnerId,
                    DeviceId = request.DeviceId,
                    Mobile = request.Mobile,
                    Ip = request.Ip
                };
                var blackResult = CheckBlackList(blackRequest, "ReceiveBargainCouponAsync");
                if (!blackResult.Item1)
                {
                    return new ShareBargainBaseResult { Code = -1, Info = blackResult.Item2 };
                }

                //检查限制：活动期内，每人领取一个商品
                //用户只可购买一个商品
                if (DateTime.Now >= GlobalConstant.TyreFestivalActivityBeginTime
                    && DateTime.Now <= GlobalConstant.TyreFestivalActivityEndTime)
                {
                    var userBargainOwnerActionCount = await DalBargain.CheckUserBargainOwnerActionCount(request.OwnerId,
                        GlobalConstant.TyreFestivalActivityBeginTime, GlobalConstant.TyreFestivalActivityEndTime, false);
                    if (userBargainOwnerActionCount > 0)
                    {
                        return new ShareBargainBaseResult()
                        {
                            Code = 9,
                            Info = "活动期内只能领取一件商品哦"
                        };
                    }
                }

                //商品限制检查 
                result = await DalBargain.CheckBargainProductStatus(request.OwnerId, request.ActivityProductId, request.Pid);
                if (result == null)
                {
                    return new ShareBargainBaseResult()
                    {
                        Code = 4,
                        Info = "您选中的宝贝还未完成砍价哦"
                    };
                }

                var parameters = new List<BuyLimitDetailModel>
            {
                new BuyLimitDetailModel
                {
                    ModuleName = "sharebargain",
                    ModuleProductId = request.ActivityProductId.ToString(),
                    LimitObjectId = request.OwnerId.ToString("D"),
                    ObjectType = LimitObjectTypeEnum.UserId.ToString(),
                    Reference = result.IdKey?.ToString(),
                    Remark = "优惠券砍价"
                }
            };
                if (!string.IsNullOrWhiteSpace(request.DeviceId))
                {
                    parameters.Add(new BuyLimitDetailModel
                    {
                        ModuleName = "sharebargain",
                        ModuleProductId = request.ActivityProductId.ToString(),
                        LimitObjectId = request.DeviceId,
                        ObjectType = LimitObjectTypeEnum.DeviceId.ToString(),
                        Reference = result.IdKey?.ToString(),
                        Remark = "优惠券砍价"
                    });
                }

                switch (result.Code)
                {
                    case 1:
                        result.Info = "优惠券领取成功啦";
                        break;
                    case 2:
                        result.Info = "商品已下架超过24小时，无法领取";
                        break;
                    case 3:
                        result.Info = "您通过本活动领取过此优惠券，无法再次领取";
                        break;
                    default:
                        result.Info = "您选中的宝贝还未完成砍价哦";
                        break;
                }

                if (result.Code != 1)
                    return new ShareBargainBaseResult { Code = result.Code, Info = result.Info };
                if (result.Code == 1)
                {
                    var checkResult = await LimitBuyManager.SelectBuyLimitInfo(parameters.Select(g => new BuyLimitModel
                    {
                        ModuleName = g.ModuleName,
                        ModuleProductId = g.ModuleProductId,
                        LimitObjectId = g.LimitObjectId,
                        ObjectType = g.ObjectType
                    }).ToList());

                    if (checkResult.Exists(g => g.CurrentCount > 0))
                    {
                        Logger.Warn(
                            $"MarkUserReceiveCoupon-->ownerId:{request.OwnerId:D}-->{JsonConvert.SerializeObject(checkResult)}");
                        return new ShareBargainBaseResult
                        {
                            Code = 3,
                            Info = "您通过本活动领取过此优惠券，无法再次领取"
                        };
                    }
                }

                var data = await DalBargain.SelectProductInfo(request.ActivityProductId);

                #region 接新优惠券服务
                var flag = false;
                var cacheFlag = await ConfigServiceProxy.GetOrSetRuntimeSwitchAsync("CreatePromotionNewForBigbrand");
                if (cacheFlag != null && cacheFlag.Result != null)
                {
                    if (cacheFlag.Result.Value)
                        flag = true;
                }
                if (flag)
                {
                    var createPromotionReponse = new CreatePromotion.Models.CreatePromotionModel
                    {
                        Channel = "BargainActivity_ProductCoupon",
                        UserID = request.OwnerId,
                        GetRuleGUID = Guid.Parse(data.PID),
                        Author = request.OwnerId.ToString(),
                        Referer = $"砍价优惠券奖励/userid:{request.OwnerId},apId:{request.ActivityProductId}"
                    };

                    var createPromotionRes = await CreatePromotionServiceProxy.CreatePromotionNewAsync(createPromotionReponse);

                    Logger.Info(
                        $"调用优惠券服务 CreatePromotion CreatePromotionNewAsync 出参：{JsonConvert.SerializeObject(createPromotionRes)} 入参：{JsonConvert.SerializeObject(createPromotionReponse)}");

                    if (!createPromotionRes.Success || createPromotionRes.Result == null || createPromotionRes.Result.ErrorCode < 0)
                    {
                        result.Code = 7;
                        result.Info = "优惠券发送失败";
                        return new ShareBargainBaseResult { Code = result.Code, Info = result.Info };
                        ;
                    }
                }
                else
                {
                    //发送优惠券给用户 

                    var createPromotionReponse = new Member.Models.CreatePromotionModel
                    {
                        Channel = "BargainActivity_ProductCoupon",
                        UserID = request.OwnerId,
                        GetRuleGUID = Guid.Parse(data.PID),
                        Author = request.OwnerId.ToString(),
                        Referer = $"砍价优惠券奖励/userid:{request.OwnerId},apId:{request.ActivityProductId}"
                    };

                    var createPromotionRes = await MemberServiceProxy.CreatePromotionNewAsync(createPromotionReponse);

                    Logger.Info(
                        $"调用优惠券服务 CreatePromotionNewAsync 出参：{JsonConvert.SerializeObject(createPromotionRes)} 入参：{JsonConvert.SerializeObject(createPromotionReponse)}");

                    if (!createPromotionRes.Success || createPromotionRes.Result == null || createPromotionRes.Result.ErrorCode < 0)
                    {
                        result.Code = 7;
                        result.Info = "优惠券发送失败";
                        return new ShareBargainBaseResult { Code = result.Code, Info = result.Info };
                        ;
                    }
                }

                var limitResult = await LimitBuyManager.AddBuyLimitInfo(parameters);
                if (limitResult.Code == 1)
                {
                    Logger.Info($"MarkUserReceiveCoupon-->{limitResult.Msg}");
                }

                //标记已发送优惠券给用户
                var markRes = await DalBargain.MarkUserReceiveCoupon(request.OwnerId, request.ActivityProductId);
                if (!markRes)
                {
                    result.Code = 8;
                    result.Info = "优惠券发送失败";
                    return new ShareBargainBaseResult { Code = result.Code, Info = result.Info };
                }

                #endregion

            }
            catch (Exception ex)
            {
                Logger.Error($"ReceiveBargainCouponAsync异常,OwnerId:{request.OwnerId},apid:{request.ActivityProductId}ex:{ex}");
            }
            return new ShareBargainBaseResult { Code = result.Code, Info = result.Info };
        }

        /// <summary>
        /// 用户帮砍
        /// </summary>
        /// <param name="idKey"></param>
        /// <param name="userId"></param>
        /// <param name="apId"></param>
        /// <returns></returns>
        public static async Task<BargainResult> HelpCutAsync(HelpCutRequest request)
        {
            var result = new BargainResult() { Code = 0 };

            try
            {
                //验证砍价黑名单
                var blackRequest = new BargainBlackListRequest()
                {
                    UserId = request.UserId,
                    DeviceId = request.DeviceId,
                    Mobile = request.Mobile,
                    Ip = request.Ip
                };
                var blackResult = CheckBlackList(blackRequest, "ReceiveBargainCouponAsync");
                if (!blackResult.Item1)
                {
                    return new BargainResult { Code = 11, Info = blackResult.Item2 };
                }

                using (var zklock = new ZooKeeperLock($"{request.IdKey:D}"))
                {
                    if (await zklock.WaitAsync(3000))
                    {
                        //获取发起的砍价信息
                        var bargainInfo = await DalBargain.FetchCurrentBargainData(request.IdKey, false);
                        if (bargainInfo == null)
                        {
                            Logger.Warn($"HelpCutAsync-FetchCurrentBargainData未查询到发起砍价信息，{request.IdKey} {request.UserId} {request.ActivityProductId} ");
                            return new BargainResult
                            {
                                Code = 10,
                                Info = "商品不存在或已下架"
                            };
                        }
                        else
                        {
                            result.SimpleDisplayName = bargainInfo.SimpleDisplayName;
                            result.HelpCutPriceTimes = bargainInfo.HelpCutPriceTimes;
                            if (bargainInfo.IsOver || bargainInfo.TotalCount <= bargainInfo.CurrentCount)
                            {
                                result.Code = 2;
                                result.Info = "手慢咯，砍价已经成功啦~";
                                return result;
                            }
                            else if (bargainInfo.EndDateTime < DateTime.Now)
                            {
                                result.Code = 3;
                                result.Info = "手慢啦，本商品砍价已结束~";
                                return result;
                            }
                            else if (bargainInfo.CurrentStockCount < 1)
                            {
                                result.Code = 4;
                                result.Info = "手慢啦，砍价商品已抢完~";
                                return result;
                            }
                            //判断该砍价商品是否为新户帮砍商品
                            else if (bargainInfo.CutPricePersonLimit == 1 && bargainInfo.OwnerId != request.UserId)
                            {
                                //测试账号忽略砍价模块新用户验证:15026884502
                                if (request.UserId != new Guid("a3136282-251e-462a-af0c-627843f5c649") && request.UserId != new Guid("e0740835-4ca1-4275-95b5-6ab0ca45086a"))
                                {
                                    //验证砍价新用户验证
                                    if (!CheckBargainNewUser(request.UserId))
                                    {
                                        result.Code = 8;
                                        result.Info = "仅限新用户帮砍~";
                                        return result;
                                    }
                                    //验证途虎新用户
                                    var orderCount = (await GroupBuyingManager.CheckNewUser(request.UserId)).Item2;
                                    if (orderCount > 0)
                                    {
                                        result.Code = 8;
                                        result.Info = "仅限新用户帮砍~";
                                        return result;
                                    }
                                }
                            }
                        }

                        //验证是否已经帮砍
                        var readOnly = !(await GetBargainOwnerCache(request.IdKey));
                        var readOnly2 = !(await GetBargainProductCache());
                        var userHelpCut = await DalBargain.CheckBargainShare(request.UserId, request.IdKey, readOnly && readOnly2);
                        if (userHelpCut.Count > 0 && bargainInfo.OwnerId != request.UserId)
                        {
                            result.Code = 5;
                            result.Reduce = userHelpCut.Max(g => g.Item2);
                            result.Info = "您已经为好友完成过砍价~";
                            return result;
                        }
                        else if (userHelpCut.Count > 1 && bargainInfo.OwnerId == request.UserId)
                        {
                            result.Code = 6;
                            result.Reduce = userHelpCut.Max(g => g.Item2);
                            result.Info = "分享人已进行过两次砍价~";
                            return result;
                        }

                        //一个活动产品，用户的所有砍价数据(自己砍和别人帮砍)
                        var allCut = await DalBargain.CheckBargainAllShare(request.UserId, bargainInfo.PID, bargainInfo.BeginDateTime, bargainInfo.EndDateTime);
                        //验证该次分享的已砍次数
                        if ((bool)allCut?.Any())
                        {
                            //用户的别人帮砍次数
                            var helpCut = allCut.Where(g => g.UserId != g.OwnerId).ToList().Count;
                            if (bargainInfo.OwnerId != request.UserId && bargainInfo.HelpCutPriceTimes > 0)
                            {
                                if (helpCut >= bargainInfo.HelpCutPriceTimes)
                                {
                                    result.Code = 9;
                                    result.Info = "帮砍次数已满~";
                                    return result;
                                }
                            }
                        }

                        var readOnly3 = !(await GetBargainOwnerCache(request.UserId));
                        var todayDate = DateTime.Now.ToString("yyyy-MM-dd 00:00:000");
                        //获取用户今天帮别人砍数据
                        var userBargainCount = await DalBargain.CheckUserBargainCount(request.UserId, todayDate, readOnly3);
                        if (userBargainCount >= TodayHelpBargainCountLimit && bargainInfo.OwnerId != request.UserId)
                        {
                            var ignoreResult = IsIgnoreDailyHelpCutCountVerify(request.UserId);
                            if (!ignoreResult)
                            {
                                result.Code = 7;
                                result.Info = "您今天砍价次数已经用完，明天再来吧~";
                                return result;
                            }
                            else
                            {
                                //进行帮砍
                                result = await CreateUserBargainRecord(bargainInfo, request.IdKey, request.UserId, request.ActivityProductId);

                                //帮砍后设为老用户
                                if (result.Code == 1)
                                {
                                    SetBargainOldUser(request.UserId);
                                }
                            }
                        }
                        else
                        {
                            //进行帮砍
                            result = await CreateUserBargainRecord(bargainInfo, request.IdKey, request.UserId, request.ActivityProductId);

                            //帮别人砍后设为老用户
                            if (result.Code == 1 && bargainInfo.OwnerId != request.UserId)
                            {
                                SetBargainOldUser(request.UserId);
                            }
                        }
                    }
                    else
                    {
                        Logger.Warn("HelpCutAsync,砍价等待链接超时");
                        result.Code = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"HelpCutAsync异常,userid:{request.UserId},idkey:{request.IdKey},ex:{ex}");
            }
            return result;
        }

        /// <summary>
        /// 验证砍价黑名单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static ShareBargainBaseResult CheckBargainBlackList(BargainBlackListRequest request)
        {
            var result = new ShareBargainBaseResult()
            {
                Code = 1
            };
            try
            {
                //验证砍价黑名单
                var blackRequest = new BargainBlackListRequest()
                {
                    UserId = request.UserId,
                    DeviceId = request.DeviceId,
                    Mobile = request.Mobile,
                    Ip = request.Ip,
                    ReceivePhone = request.ReceivePhone,
                    PayAccount = request.PayAccount
                };
                var blackResult = CheckBlackList(blackRequest, "CheckBargainBlackListAsync");
                if (!blackResult.Item1)
                {
                    result.Code = -1;
                    result.Info = blackResult.Item2;
                }
                else
                {
                    result.Code = 1;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"CheckBargainBlackList异常,userId:{request.UserId} deviceId:{request.DeviceId},ex:{ex}");
            }
            return result;
        }

        /// <summary>
        ///  获取砍价活动商品信息: 配置信息、参与人数、图文信息 (砍价商品详情页使用)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<GetShareBargainProductInfoResponse> GetShareBargainProductInfoAsync
            (GetShareBargainProductInfoRequest request)
        {
            var result = new GetShareBargainProductInfoResponse();

            try
            {
                //1.获取配置基本信息
                var productInfo = await GetProductSettingModelFromCacheAsync(request.ActivitiProductID);

                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ShareBargainName))
                {
                    //2.获取参与人数 - 帮砍成功时刷新帮砍的那个ActivitiProductID缓存
                    var participantCount = 0;
                    var participantCountCacheKey = $"{GetRefreshKeyPrefix($"{GlobalConstant.BargainProductSettingKey}/{request.ActivitiProductID}")}/{GlobalConstant.BargainProductParticipantCountKey}/{request.ActivitiProductID}";
                    var participantCountCache = await cacheClient.GetOrSetAsync(participantCountCacheKey,
                        async () => await DalBargain.GetBargainProductParticipantCountAsync(request.ActivitiProductID, false),
                        TimeSpan.FromDays(1));
                    if (participantCountCache.Success)
                    {
                        participantCount = participantCountCache.Value;
                    }
                    else
                    {
                        participantCount = await DalBargain.GetBargainProductParticipantCountAsync(request.ActivitiProductID, false);
                        Logger.Warn($"GetShareBargainProductInfoAsync => participantCountCache缓存失败,key:{participantCountCacheKey},Message{participantCountCache.Message}");
                    }

                    result = new GetShareBargainProductInfoResponse()
                    {
                        PKID = productInfo.PKID,
                        ActivityId = GlobalConstant.BargainActivityId,//砍价活动id固定值
                        ShowBeginTime = productInfo.ShowBeginTime,
                        BeginDateTime = productInfo.BeginDateTime,
                        EndDateTime = productInfo.EndDateTime,
                        ProductType = productInfo.ProductType,
                        PID = productInfo.PID,
                        ProductName = productInfo.ProductName,
                        SimpleDisplayName = productInfo.SimpleDisplayName,
                        OriginalPrice = productInfo.OriginalPrice,
                        FinalPrice = productInfo.FinalPrice,
                        TotalStockCount = productInfo.TotalStockCount,
                        CurrentStockCount = productInfo.CurrentStockCount,
                        CutPricePersonLimit = productInfo.CutPricePersonLimit,
                        Times = productInfo.Times,
                        HelpCutPriceTimes = productInfo.HelpCutPriceTimes,
                        BigCutBeforeCount = productInfo.BigCutBeforeCount,
                        BigCutPriceRate = productInfo.BigCutPriceRate,
                        Sequence = productInfo.Sequence,
                        Image1 = productInfo.Image1,
                        //WXShareTitle、APPShareId未配置则取全局配置
                        WXShareTitle = productInfo.WXShareTitle,
                        APPShareId = productInfo.APPShareId,
                        PageName = productInfo.PageName,
                        SuccessfulHint = productInfo.SuccessfulHint,
                        ParticipantCount = participantCount,
                        ProductImgList = new List<BargainProductImg>()
                    };

                    //3.商品详情图片
                    var productImgList = new List<BargainProductImg>() {
                    new BargainProductImg(){ ImgUrl=productInfo.ProductDetailImg1},
                    new BargainProductImg(){ ImgUrl=productInfo.ProductDetailImg2},
                    new BargainProductImg(){ ImgUrl=productInfo.ProductDetailImg3},
                    new BargainProductImg(){ ImgUrl=productInfo.ProductDetailImg4},
                    new BargainProductImg(){ ImgUrl=productInfo.ProductDetailImg5},
                };
                    productImgList = productImgList.Where(x => !string.IsNullOrWhiteSpace(x.ImgUrl))?.ToList();

                    //是商品类型的 没有配置则从产品库取 - 缓存1小时
                    if (result.ProductType == 1 && !(productImgList?.Count > 0))
                    {
                        var urlList = new List<string>();

                        //获取商品图文详情 - 缓存短时间
                        var imgUrlCacheKey = $"{GetRefreshKeyPrefix(GlobalConstant.BargainProductSettingKey)}/{GlobalConstant.BargainProductDescImgUrlKey}/{result.PID}";
                        var imgUrlCache = await cacheClient.GetOrSetAsync(imgUrlCacheKey,
                            async () => await GetProductDescriptionImgUrlList(result.PID),
                            TimeSpan.FromMinutes(30));
                        if (imgUrlCache.Success)
                        {
                            urlList = imgUrlCache.Value;
                        }
                        else
                        {
                            urlList = await GetProductDescriptionImgUrlList(result.PID);
                            Logger.Warn($"GetShareBargainProductInfoAsync => imgUrlCache缓存失败,key:{imgUrlCacheKey},Message{imgUrlCache.Message}");
                        }

                        result.ProductImgList = new List<BargainProductImg>();
                        foreach (var item in urlList ?? new List<string>())
                        {
                            result.ProductImgList.Add(new BargainProductImg() { ImgUrl = item });
                        }
                    }
                    else
                    {
                        result.ProductImgList = productImgList ?? new List<BargainProductImg>();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error($"GetShareBargainProductInfoAsync异常,APID:{request.ActivitiProductID}", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取商品图文详情  
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        private static async Task<List<string>> GetProductDescriptionImgUrlList(string pid)
        {
            var result = new List<string>();

            try
            {
                //获取商品信息:图文描述
                var productInfos = await ProductQueryServiceProxy.SelectSkuProductListByPidsAsync(new List<string>() { pid });
                var imgDescriptionHtml = productInfos.FirstOrDefault(x => string.Equals(x.Pid, pid, StringComparison.InvariantCultureIgnoreCase))
                                    ?.Description;
                if (string.IsNullOrWhiteSpace(imgDescriptionHtml))
                {
                    Logger.Warn($"GetProductDescriptionImgUrlList 未查询到商品图文详情,pid:{pid}");
                    return result;
                }

                //从html正则读取图片
                Regex regImg = new Regex(@"src=""(?<imgUrl>\S+.(jpg|jpge|gif|bmp|bnp|png))""", RegexOptions.IgnoreCase);
                // 搜索匹配的字符串
                MatchCollection matches = regImg.Matches(imgDescriptionHtml);

                // 取得匹配项列表 
                foreach (Match match in matches)
                {
                    result.Add(match.Groups["imgUrl"].Value);
                }
                result = result.Distinct()?.ToList();
            }
            catch (Exception ex)
            {
                Logger.Error($"GetProductDescriptionImgUrlList", ex);
            }

            return result;
        }

        /// <summary>
        /// 获取砍价商品配置信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<GetShareBargainSettingInfoResponse> GetShareBargainSettingInfoAsync(GetShareBargainSettingInfoRequest request)
        {
            var result = new GetShareBargainSettingInfoResponse();

            try
            {
                //获取配置基本信息
                var settingModel = await GetProductSettingModelFromCacheAsync(request.ActivitiProductID);
                result = new GetShareBargainSettingInfoResponse()
                {
                    PKID = settingModel.PKID,
                    ActivityId = GlobalConstant.BargainActivityId,//砍价活动id固定值
                    ShowBeginTime = settingModel.ShowBeginTime,
                    BeginDateTime = settingModel.BeginDateTime,
                    EndDateTime = settingModel.EndDateTime,
                    ProductType = settingModel.ProductType,
                    PID = settingModel.PID,
                    ProductName = settingModel.ProductName,
                    SimpleDisplayName = settingModel.SimpleDisplayName,
                    OriginalPrice = settingModel.OriginalPrice,
                    FinalPrice = settingModel.FinalPrice,
                    TotalStockCount = settingModel.TotalStockCount,
                    CurrentStockCount = settingModel.CurrentStockCount,
                    CutPricePersonLimit = settingModel.CutPricePersonLimit,
                    Times = settingModel.Times,
                    HelpCutPriceTimes = settingModel.HelpCutPriceTimes,
                    BigCutBeforeCount = settingModel.BigCutBeforeCount,
                    BigCutPriceRate = settingModel.BigCutPriceRate,
                    Sequence = settingModel.Sequence,
                    Image1 = settingModel.Image1,
                    WXShareTitle = settingModel.WXShareTitle,
                    APPShareId = settingModel.APPShareId,
                    PageName = settingModel.PageName,
                    SuccessfulHint = settingModel.SuccessfulHint,
                };

                var productImgList = new List<string>() {
                  settingModel.ProductDetailImg1,
                  settingModel.ProductDetailImg2,
                  settingModel.ProductDetailImg3,
                  settingModel.ProductDetailImg4,
                  settingModel.ProductDetailImg5,
                };
                productImgList = productImgList.Where(x => !string.IsNullOrWhiteSpace(x))?.ToList();
                result.ProductImgList = productImgList;
            }
            catch (Exception ex)
            {
                Logger.Error($"GetShareBargainSettingInfoAsync异常,ActivitiProductID:{request.ActivitiProductID}", ex);
            }

            return result;
        }

        /// <summary>
        /// 获取砍价商品配置信息 ,设置缓存。 在配置改动时刷新通用key前缀
        /// </summary>
        /// <param name="activityProductID"></param>
        /// <returns></returns>
        private static async Task<BargainProductSettingModel> GetProductSettingModelFromCacheAsync(int activityProductID)
        {
            var result = new BargainProductSettingModel();

            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ShareBargainName))
            {
                //获取砍价商品配置 - 缓存
                var cacheKey = $"{GetRefreshKeyPrefix(GlobalConstant.BargainProductSettingKey)}/{GlobalConstant.BargainProductSettingKey}/{activityProductID}";
                var productInfoCache = await cacheClient.GetOrSetAsync(cacheKey,
                    async () => await DalBargain.GetBargainProductInfoAsync(activityProductID, false),
                    TimeSpan.FromDays(1));
                if (productInfoCache.Success)
                {
                    result = productInfoCache.Value;
                }
                else
                {
                    result = await DalBargain.GetBargainProductInfoAsync(activityProductID, false);
                    Logger.Warn($"GetProductSettingModelFromCacheAsync => productInfoCache缓存失败,key:{cacheKey},Message{productInfoCache.Message}");
                }
            }

            if (!(result.PKID > 0))
            {
                Logger.Warn($"GetProductSettingModelFromCacheAsync获取配置信息为空.activityProductID:{activityProductID}");
            }

            return result ?? new BargainProductSettingModel();
        }

        #endregion

        /// <summary>
        /// 验证是否是砍价模块新用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="funcName"></param>
        /// <returns></returns>
        private static bool CheckBargainNewUser(Guid userId)
        {
            var result = true;
            try
            {
                //开关为true则不验证砍价新用户
                var isIgnore = CheckIgnoreNewUserSwitch();
                if (isIgnore)
                {
                    Logger.Warn("砍价-忽略帮砍新用户验证开关状态:true");
                    return true;
                }

                using (var client = new UserProfileClient())
                {
                    var clientResult = client.Get(userId, ShareBargainNewUser);
                    if (!clientResult.Success)
                    {
                        Logger.Warn($"CheckBargainNewUser获取用户是否是砍价模块新用户失败，ErrorMessage:{clientResult.ErrorMessage}");
                    }
                    else
                    {
                        int.TryParse(clientResult.Result, out int newUserFlag);
                        if (newUserFlag > 0)
                        {
                            result = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = true;
                Logger.Error($"CheckBargainNewUser获取用户是否是砍价模块新用户异常，ex:{ex}");
            }
            return result;
        }

        /// <summary>
        /// 忽略砍价新用户限制
        /// </summary>
        /// <returns></returns>
        private static bool CheckIgnoreNewUserSwitch(string switchStr = "IgnoreHelpCutNewUser")
        {
            var switchFlag = false;
            try
            {
                using (var switchClient = new ConfigClient())
                {
                    var switchResult = switchClient.GetOrSetRuntimeSwitch(switchStr);

                    if (!switchResult.Success)
                    {
                        Logger.Warn("CheckBargainNewUser-GetRuntimeSwitch服务调用失败");
                    }
                    else
                    {
                        if (switchResult.Result != null)
                        {
                            switchFlag = (bool)switchResult.Result?.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                switchFlag = false;
                Logger.Error($"CheckIgnoreNewUserSwitch-GetRuntimeSwitch服务调用异常,ex:{ex}");
            }
            return switchFlag;
        }

        /// <summary>
        /// 验证是否通过每日帮砍次数验证
        /// </summary>
        /// <returns></returns>
        private static bool IsIgnoreDailyHelpCutCountVerify(Guid userId)
        {
            //固定测试账号跳过限制 
            if (TestUserIdsConst.Contains(userId))
            {
                return true;
            }

            //验证忽略每日帮砍验证开关
            return CheckIgnoreNewUserSwitch("IsIgnoreTodayHelpBargainCountLimit");
        }

        /// <summary>
        /// 将用户设置为砍价老用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static bool SetBargainOldUser(Guid userId)
        {
            var result = false;
            try
            {
                using (var client = new UserProfileClient())
                {
                    var clientResult = client.Set(userId, ShareBargainNewUser, "1");
                    if (clientResult.Success)
                    {
                        result = true;
                    }
                    else
                    {
                        Logger.Warn($"SetBargainOldUser将用户设置砍价老用户失败，userId：{userId:D}，ErrorMessage:{clientResult.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"SetBargainOldUser将用户设置砍价老用户异常，userId：{userId:D}，ex:{ex}");
            }
            return result;
        }

        /// <summary>
        /// 验证砍价黑名单
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="deviceid"></param>
        /// <param name="mobile"></param>
        /// <param name="payAccount"></param>
        /// <param name="ip"></param>
        /// <returns>true表示验证通过,可以进行操作</returns>
        public static Tuple<bool, string> CheckBlackList(BargainBlackListRequest blackRequest, string funName)
        {
            var result = true;
            var resultStr = string.Empty;
            try
            {
                //获取电话号码
                blackRequest.Mobile = string.IsNullOrWhiteSpace(blackRequest.Mobile) ? GetUserMobileById(blackRequest.UserId) : blackRequest.Mobile;

                var request = new BlockListRequest()
                {
                    System = "ShareBargain",
                    BlockListItems = new List<BlockListItem>()
                };
                if (!(blackRequest.UserId == Guid.Empty))
                {
                    request.BlockListItems.Add(new BlockListItem()
                    {
                        BlockType = (int)BlockListType.UserId,
                        BlockValue = blackRequest.UserId.ToString()
                    });
                }

                if (!string.IsNullOrWhiteSpace(blackRequest.DeviceId))
                {
                    request.BlockListItems.Add(new BlockListItem()
                    {
                        BlockType = (int)BlockListType.DeviceId,
                        BlockValue = blackRequest.DeviceId
                    });
                }

                if (!string.IsNullOrWhiteSpace(blackRequest.Mobile))
                {
                    request.BlockListItems.Add(new BlockListItem()
                    {
                        BlockType = (int)BlockListType.Mobile,
                        BlockValue = blackRequest.Mobile
                    });
                }

                if (!string.IsNullOrWhiteSpace(blackRequest.PayAccount))
                {
                    request.BlockListItems.Add(new BlockListItem()
                    {
                        BlockType = (int)BlockListType.PayAccount,
                        BlockValue = blackRequest.PayAccount
                    });
                }

                if (!string.IsNullOrWhiteSpace(blackRequest.Ip))
                {
                    request.BlockListItems.Add(new BlockListItem()
                    {
                        BlockType = (int)BlockListType.Ip,
                        BlockValue = blackRequest.Ip
                    });
                }

                if (request.BlockListItems.Count == 0)
                {
                    return new Tuple<bool, string>(true, "");
                }
                using (var client = new BlockListConfigClient())
                {
                    var clientResult = client.BatchExistBlockListExactly(request);
                    if (clientResult.Success)
                    {
                        foreach (var item in clientResult.Result)
                        {
                            if (item.IsBlocked)
                            {
                                result = false;
                                switch (item.BlockType)
                                {
                                    case (int)BlockListType.UserId:
                                        resultStr = "系统检测到您当前账号存在异常，请稍后再试";
                                        break;
                                    case (int)BlockListType.DeviceId:
                                        resultStr = "系统检测到您当前登录设备存在异常，请稍后再试";
                                        break;
                                    case (int)BlockListType.Mobile:
                                        resultStr = "系统检测到您当前登录账号存在异常，请稍后再试";
                                        break;
                                    case (int)BlockListType.PayAccount:
                                        resultStr = "系统检测到您当前支付账号存在异常，请稍后再试";
                                        break;
                                    case (int)BlockListType.Ip:
                                        resultStr = "当前区域暂时缺货，请更换地址下单";
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            }
                            if (!result)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        result = true;
                        Logger.Warn($"{funName}验证黑名单调用接口失败，userid：{blackRequest.UserId},deviceid:{blackRequest.DeviceId},mobile:{blackRequest.Mobile}," +
                            $"payAccount:{blackRequest.PayAccount},ip:{ blackRequest.Ip},ErrorMessage:{clientResult.ErrorMessage}");
                    }
                    if (result)
                    {
                        var receiveResult = CheckBalckListReceivePhone(blackRequest.ReceivePhone);
                        if (!receiveResult)
                        {
                            result = false;
                            resultStr = "系统检测到您当前收货账号存在异常，请稍后再试";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = true;
                Logger.Error($"CheckBlackList验证砍价黑名单异常,ex:{ex}");
            }
            return new Tuple<bool, string>(result, resultStr);
        }

        /// <summary>
        /// 验证收货号码是否在黑名单
        /// </summary>
        /// <param name="receivePhone"></param>
        /// <returns></returns>
        private static bool CheckBalckListReceivePhone(string receivePhone)
        {
            var result = true;
            if (string.IsNullOrWhiteSpace(receivePhone))
            {
                return true;
            }
            var request = new BlockListItem()
            {
                BlockSystem = "ShareBargain",
                BlockType = (int)BlockListType.Mobile
            };
            try
            {
                using (var client = new BlockListConfigClient())
                {
                    var clientResult = client.ExistBlockListItem(request);
                    if (clientResult.Success)
                    {
                        if (!clientResult.Result)
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        result = true;
                        Logger.Warn($"验证黑名单调用接口失败，receivePhone：{receivePhone},ErrorMessage:{clientResult.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                result = true;
                Logger.Error($"CheckBalckListReceivePhone异常，receivePhone：{receivePhone},ex:{ex}");
            }
            return result;
        }

        /// <summary>
        /// 查询用户手机号
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static string GetUserMobileById(Guid userId)
        {
            var result = string.Empty;
            if (userId == Guid.Empty)
            {
                return result;
            }
            try
            {
                using (var client = new UserAccountClient())
                {
                    var clientResult = client.GetUserById(userId);
                    if (clientResult.Success)
                    {
                        result = clientResult.Result?.MobileNumber;
                    }
                    else
                    {
                        Logger.Warn($"GetUserMobileById用户用户信息失败ErrorMessage:{clientResult.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetUserMobileById异常，ex：{ex}");
            }
            return result;
        }

        /// <summary>
        /// 获取缓存key前缀
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public static string GetRefreshKeyPrefix(string redisKey)
        {
            try
            {
                using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ShareBargainName))
                {
                    var result = client.GetOrSet(redisKey, () => { return DateTime.Now.Ticks.ToString(); }
                    , TimeSpan.FromDays(1));
                    if (!result.Success || string.IsNullOrWhiteSpace(result.Value))
                    {
                        return DateTime.Now.Ticks.ToString();
                    }
                    else
                    {
                        return result.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"GetRefreshKeyPrefix异常;redisKey={redisKey}", ex);
                return DateTime.Now.Ticks.ToString();
            }

        }
    }
}
