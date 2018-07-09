using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Product;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models.Push;
using Tuhu.Service.UserAccount;
using Tuhu.ZooKeeper;

namespace Tuhu.Service.Activity.Server.Manager
{
    public class BargainManager
    {
        public static readonly ILog Logger = LogManager.GetLogger(typeof(BargainManager));

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
                    // 获取砍价数据
                    var data = await DalBargain.FetchCurrentBargainData(idKey, false);
                    // 获取砍价记录
                    var actionInfo = await DalBargain.CheckBargainShare(userId, idKey, readOnly && readOnly2);

                    if (data == null)
                    {
                        Logger.Info($" {idKey} {userId} {apId} ");
                        return new BargainResult
                        {
                            Code = 0,
                            Info = "服务器异常"
                        };
                    }

                    var remnantMoney = data.OriginalPrice - data.FinalPrice - data.CurrentRedece;

                    //获取用户砍价次数
                    var readOnly3 = !(await GetBargainOwnerCache(userId));

                    var todayDate = DateTime.Now.ToString("yyyy-MM-dd 00:00:000");
                    //获取用户今天砍价数据
                    var userBargainCount = await DalBargain.CheckUserBargainCount(userId, todayDate, readOnly3);

                    var result = new BargainResult
                    {
                        RemnantMoney = remnantMoney,
                        SimpleDisplayName = data.SimpleDisplayName
                    };
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
                    else if (userBargainCount >= 5 && data.OwnerId != userId)
                    {
                        result.Code = 7;
                        result.Info = "您今天砍价次数已经用完，明天再来吧~";
                    }
                    else
                    {
                        //进行帮砍
                        result = await CreateUserBargainRecord(data, idKey, userId, apId);
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
        ///     检查商品是否可购买
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

                Logger.Info($"获取砍价全局配置失败");
                return await GetBargainGlobalConfig();
            }
        }


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

        private static async Task<bool> RefreshShareBargainCache(List<BargainProductItem> ids)
        {
            var data = await SelectBargainProductInfo(ids.Select(g => g.ActivityProductId.ToString()).ToList());
            using (var hashClient =
                CacheHelper.CreateHashClient(GlobalConstant.ShareBargainHashKey, TimeSpan.FromHours(24)))
            {
                var dic = new Dictionary<string, object>();
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
                return new ShareBargainBaseResult {Code = val.Code, Info = val.Info};
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

            //发送优惠券给用户 
            using (var memberClient = new Tuhu.Service.Member.PromotionClient())
            {

                var createPromotionReponse = new Member.Models.CreatePromotionModel
                {
                    Channel = "BargainActivity_ProductCoupon",
                    UserID = ownerId,
                    GetRuleGUID = Guid.Parse(data.PID),
                    Author = ownerId.ToString(),
                    Referer = $"砍价优惠券奖励/userid:{ownerId},apId:{apId}"
                };

                Logger.Info(
                    $"调用优惠券服务 CreatePromotionNewAsync 入参：{JsonConvert.SerializeObject(createPromotionReponse)}");

                var createPromotionRes = await memberClient.CreatePromotionNewAsync(createPromotionReponse);

                Logger.Info(
                    $"调用优惠券服务 CreatePromotionNewAsync 出参：{JsonConvert.SerializeObject(createPromotionRes)} 入参：{JsonConvert.SerializeObject(createPromotionReponse)}");

                if (!createPromotionRes.Success || createPromotionRes.Result == null || createPromotionRes.Result.ErrorCode < 0)
                {
                    val.Code = 7;
                    val.Info = "优惠券发送失败";
                    return new ShareBargainBaseResult {Code = val.Code, Info = val.Info};
                    ;
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
                    return new ShareBargainBaseResult {Code = val.Code, Info = val.Info};
                    ;
                }
            }

            return new ShareBargainBaseResult {Code = val.Code, Info = val.Info};
            ;
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

        public static async Task PushMessageByUserId(CurrentBargainData data, bool isOver, int apId, Guid userId, Guid idKey)
        {
            var nickName = "";
            using (var client = new UserAccountClient())
            {
                var searchResule = await client.GetUserByIdAsync(userId);
                if (searchResule.Success)
                {
                    nickName = searchResule.Result?.Profile?.NickName ?? "";
                }
            }
            using (var client = new TemplatePushClient())
            {
                var target = new List<string>()
                {
                    userId.ToString("D")
                };
                var batchid = isOver ? 635 : 633;
                var result = await client.PushByUserIDAndBatchIDAsync(target, batchid, new PushTemplateLog()
                {
                    Replacement = JsonConvert.SerializeObject(new Dictionary<string, string>()
                    {
                        ["{{IdKey}}"] = idKey.ToString("D"),
                        ["{{Pid}}"] = data.PID,
                        ["{{AcitvityProductId}}"] = apId.ToString(),
                        ["{{ProductName}}"] = data.ProductName,
                        ["{{NickName}}"] = nickName,
                        ["{{ProductBriefName}}"] = data.SimpleDisplayName,
                        ["{{Price}}"] = data.OriginalPrice.ToString("#0.00"),
                        ["{{ActivityPrice}}"] = data.FinalPrice.ToString("#0.00")
                    })
                });
                result.ThrowIfException(true);
                if (!(result.Success && result.Result))
                {
                    Logger.Warn($"用户{userId}下商品{data.ProductName}砍价完成,推送失败", result.Exception);
                }
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

                result = data.Where(g => g.OnSale).OrderByDescending(g => g.CurrentCount).Take(count).ToList();
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
        ///     分页获取用户的砍价记录
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
        ///     获取用户砍价商品详情
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
        public static async Task<CreateBargainResult> CreateUserBargain(Guid userId, int apId, string pid)
        {

            using (var zklock = new ZooKeeperLock($"{userId:D}/{apId}"))
            {
                // zk 锁
                if (await zklock.WaitAsync(5000))
                {
                    // 检查用户砍价数据
                    var checkResult = await DalBargain.CheckUserBargainRecord(userId, pid, apId);
                    var result = new CreateBargainResult
                    {
                        Code = 1,
                        Info = "创建成功~",
                        OwnerId = userId,
                        IdKey = Guid.Empty,
                        SimpleDisplayName = checkResult.FirstOrDefault()?.Item2 ?? default(string)
                    };
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
            if (!ids.Any()) return new List<BargainProductNewModel>();
            var data = await DalBargain.SelectBargainProductInfo(ids);
            if (!data.Any()) return new List<BargainProductNewModel>();
            var globalConfig = await GetBargainShareConfig();
            var countInfo =
                await DalBargain.GetCurrentBargainCount(data.Select(g => g.ActivityProductId).ToList());
            data.ForEach(t =>
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
                    await client.SelectSkuProductListByPidsAsync(data.Select(g => g.Pid).Distinct().ToList());
                if (productInfo.Success && productInfo.Result.Any())
                {
                    foreach (var item in data)
                    {
                        var productItem = productInfo.Result.FirstOrDefault(g => g.Pid == item.Pid);
                        item.OnSale = productItem?.Onsale ?? false;
                        item.ProductName = productItem?.DisplayName ?? item.ProductName;
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

            data.ForEach(g => g.ActivityId = GlobalConstant.BargainActivityId);
            return data;
        }
        /// <summary>
        ///     创建用户帮忙砍价数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="idKey"></param>
        /// <param name="userId"></param>
        /// <param name="apId"></param>
        /// <returns></returns>
        private static async Task<BargainResult> CreateUserBargainRecord(CurrentBargainData data, Guid idKey,
            Guid userId, int apId)
        {
            var result = new BargainResult();
            var val = data.OriginalPrice - data.FinalPrice - data.CurrentRedece;
            Random random = new Random();
            var rd = random.NextDouble() + 0.5;
            if (data.TotalCount - data.CurrentCount > 1)
            {
                val = decimal.Multiply(val / (data.TotalCount - data.CurrentCount), (decimal) rd);
                val = decimal.Round(val, 2, MidpointRounding.AwayFromZero);
            }

            var isover = data.TotalCount - data.CurrentCount == 1;
            var createdResult = await DalBargain.AddBargainAction(userId, apId, data.PKID, val, isover,
                data.CurrentCount + 1, data.CurrentRedece + val);
            if (createdResult)
            {
                result.Code = 1;
                result.Reduce = val;
                result.Rate = data.Average > 0 ? (double) (val / data.Average) : 1;
                result.Info = data.SuccessfulHint.Replace(@"{{ReducePrice}}", val.ToString("#0.00"));
                result.RemnantMoney = data.OriginalPrice - data.FinalPrice - data.CurrentRedece - val;
                result.SimpleDisplayName = data.SimpleDisplayName;
                await RefreshShareBargainCache(new List<BargainProductItem>
                {
                    new BargainProductItem {ActivityProductId = apId, PID = data.PID}
                });
                await SetBargainOwnerCache(data.OwnerId);
                await SetBargainOwnerCache(idKey);
                await PushMessageByUserId(data, isover, apId,userId,idKey);
                if (data.CurrentStockCount == 1)
                {
                    await SetBargainProductCache();
                }

                if (isover)
                {
                    await AddSliceShowInfo(new SliceShowInfoModel
                    {
                        UserId = userId,
                        ProductName = data.ProductName,
                        SimpleDisplayName = data.SimpleDisplayName,
                        FinishTime = DateTime.Now
                    });
                }
            }
            else
            {
                result.Code = 0;
            }

            return result;
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

            return result;

        }

        #endregion
    }
}
