using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Nosql;
using Tuhu.Service.Activity.Const;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.DataAccess.Models.SalePromotionActivity;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Product.Models;

namespace Tuhu.Service.Activity.Server.Manager
{
    public static class DiscountActivityInfoManager
    {
        private static ILog Logger = LogManager.GetLogger(typeof(DiscountActivityInfoManager));
        public static readonly string DefaultClientName = "Activity";
        private static readonly string ActivityUserBuyNumCachePrefix = "ActivityUserBuyNum";//用户活动已购买数
        private static readonly string ActivityProductSoldNumCachePrefix = "ActivityProductSoldNum";//活动商品已售数量

        #region 查询

        /// <summary>
        /// 获取时间段内商品的活动信息列表
        /// </summary>
        /// <param name="pidList"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static async Task<OperationResult<IEnumerable<ProductActivityInfoForTag>>> GetProductDiscountInfoForTagAsync(List<string> pidList, DateTime startTime, DateTime endTime)
        {
            return await OperationResult.FromResultAsync(DalDiscountActivityInfo.GetProductDiscountInfoForTagAsync(pidList, startTime, endTime));
        }

        /// <summary>
        /// 获取商品的打折活动信息和用户限购数
        /// </summary>
        /// <param name="pidList"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<IEnumerable<ProductDiscountActivityInfo>>> GetProductAndUserDiscountInfoAsync(List<string> pidList, string userId)
        {
            var listDiscountInfo = new List<ProductDiscountActivityInfo>();
            //获取商品的活动信息
            var listDiscountInfoResult = await GetProductDiscountInfoAsync(pidList);
            listDiscountInfo = listDiscountInfoResult.Result;
            if (!listDiscountInfoResult.Success)
            {
                return OperationResult.FromError<IEnumerable<ProductDiscountActivityInfo>>("2", listDiscountInfoResult.ErrorMessage);
            }
            //有传userid 查询用户限购数
            if (listDiscountInfo?.Count > 0 && !string.IsNullOrWhiteSpace(userId))
            {
                //批量获取用户已购数量,计算还可购买数量
                var userBuyNumList = await GetOrSetUserActivityBuyNumCache(userId, listDiscountInfo.Select(a => a.DiscountActivityInfo?.ActivityId).ToList());
                foreach (var item in listDiscountInfo)
                {
                    if (item.HasDiscountActivity && item.DiscountActivityInfo?.IsUserPurchaseLimit == true)
                    {
                        var userBuyNum = (userBuyNumList.FirstOrDefault(n => n.ActivityId == item.DiscountActivityInfo?.ActivityId) ?? new UserActivityBuyNumModel()).BuyNum;
                        var userVisibleNum = item.DiscountActivityInfo.UserLimitNum - userBuyNum;
                        item.DiscountActivityInfo.UserVisibleNum = userVisibleNum > 0 ? userVisibleNum : 0;
                    }
                }
            }
            //没有活动的商品 hasactivity返回false。
            var resultList = new List<ProductDiscountActivityInfo>();
            foreach (var item in pidList)
            {
                var model = listDiscountInfo?.FirstOrDefault(a => a.Pid == item) ?? new ProductDiscountActivityInfo()
                {
                    Pid = item,
                    HasDiscountActivity = false
                };
                if (!string.IsNullOrWhiteSpace(model?.DiscountActivityInfo?.ActivityId))
                {
                    model.HasDiscountActivity = true;
                }
                resultList.Add(model);
            }
            return OperationResult.FromResult<IEnumerable<ProductDiscountActivityInfo>>(resultList);
        }

        /// <summary>
        /// 批量获取商品的打折信息
        /// </summary>
        /// <param name="pidList"></param>
        /// <returns></returns>
        private static async Task<OperationResult<List<ProductDiscountActivityInfo>>> GetProductDiscountInfoAsync(List<string> pidList)
        {
            var resultList = new List<ProductDiscountActivityInfo>();
            var tagList = new List<ProductDiscountActivity>();
            var fromDbList = new List<ProductDiscountActivityInfo>();
            using (var tagClient = new ProductConfigClient())
            {
                //从标签服务获取商品当前参加的打折信息
                var pidsDiscountTagResult = await tagClient.SelectProductDiscountFromCacheAsync(pidList);
                if (!pidsDiscountTagResult.Success)
                {
                    //缓存获取失败 ,从数据库读取活动信息
                    resultList = await GetDiscountActivityInfoByPidsFromDB(pidList);
                    Logger.Warn("GetProductDiscountInfoAsync，从标签缓存获取商品打折信息失败");
                }
                else
                {
                    tagList = pidsDiscountTagResult?.Result;
                    var excepPid = pidList.Except(tagList.Select(p => p.Pid));
                    if (excepPid?.Count() > 0)
                    {
                        //没有从缓存取到的查数据库
                        fromDbList = await GetDiscountActivityInfoByPidsFromDB(excepPid.ToList());
                    }
                    if (tagList?.Count > 0)
                    {
                        foreach (var item in pidList)
                        {
                            var model = new ProductDiscountActivityInfo()
                            {
                                Pid = item,
                                HasDiscountActivity = false
                            };
                            var pidActivityModel = tagList.FirstOrDefault(a => a.Pid == item);
                            //有活动
                            if (pidActivityModel?.DiscountActivityList?.Count > 0)
                            {
                                var tagActivityModel = pidActivityModel.DiscountActivityList[0];
                                model.HasDiscountActivity = true;
                                //活动商品的还可购买数量：限购数-已售数(缓存)
                                // var ableNum = tagActivityModel.LimitQuantity - await GetOrSetActivityProductSoldNumCache(tagActivityModel.ActivityId, tagActivityModel.Pid);
                                model.DiscountStock = tagActivityModel.LimitQuantity;
                                model.ImageUrl = tagActivityModel.ImageUrl;
                                var ruleList = new List<DiscountActivityRule>();
                                if (tagActivityModel.RuleList?.Count > 0)
                                {
                                    foreach (var rule in tagActivityModel.RuleList)
                                    {
                                        var ruleModel = new DiscountActivityRule()
                                        {
                                            Condition = rule.Condition,
                                            DiscountRate = rule.DiscountRate
                                        };
                                        ruleList.Add(ruleModel);
                                    }
                                }
                                model.DiscountActivityInfo = new DiscountActivityAndUserLimitInfo()
                                {
                                    ActivityId = tagActivityModel.ActivityId,
                                    Description = tagActivityModel.Description,
                                    Banner = tagActivityModel.Banner,
                                    DiscountType = tagActivityModel.DiscountType,
                                    IsDefaultLabel = tagActivityModel.IsDefaultLabel,
                                    Label = tagActivityModel.IsDefaultLabel ? "折" : tagActivityModel.Label,
                                    RuleList = ruleList,
                                    IsUserPurchaseLimit = tagActivityModel.IsUserPurchaseLimit,
                                    UserLimitNum = tagActivityModel.UserLimitNum
                                };
                                resultList.Add(model);
                            }
                        }

                        //获取活动商品已售数量
                        var soldNumCacheRequest = new List<ActivityPidSoldNumModel>();
                        foreach (var item in resultList)
                        {
                            if (item.HasDiscountActivity)
                            {
                                var requestModel = new ActivityPidSoldNumModel()
                                {
                                    ActivityId = item.DiscountActivityInfo?.ActivityId,
                                    Pid = item.Pid
                                };
                                soldNumCacheRequest.Add(requestModel);
                            }
                        }
                        var soldNumCacheList = await GetOrSetActivitysProductsSoldNumCache(soldNumCacheRequest);
                        foreach (var item in resultList)
                        {
                            var ableNum = item.DiscountStock - (int)soldNumCacheList.FirstOrDefault(a => a.Pid == item.Pid)?.SoldNum;
                            item.DiscountStock = ableNum > 0 ? ableNum : 0;
                        }
                    }
                }
            }
            if (fromDbList?.Count > 0)
            {
                resultList?.AddRange(fromDbList);
            }
            return OperationResult.FromResult(resultList);
        }

        #endregion

        /// <summary>
        /// 创建订单时 记录订单的打折活动信息， 刷新缓存,验证商品下架
        /// </summary>
        /// <param name="orderInfoList"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> SaveCreateOrderDiscountInfoAndSetCache(List<DiscountCreateOrderRequest> orderInfoList)
        {
            //同一商品出现多次 进行合并
            bool result = true;
            var pidList = orderInfoList.Select(a => a.Pid).Distinct();
            var list = new List<SalePromotionActivityDiscountOrder>();
            foreach (var item in pidList)
            {
                var requestModel = orderInfoList.FirstOrDefault(o => o.Pid == item);
                if (requestModel != null)
                {
                    var model = new SalePromotionActivityDiscountOrder()
                    {
                        UserId = requestModel.UserId,
                        OrderId = requestModel.OrderId,
                        Pid = requestModel.Pid,
                        Num = orderInfoList.Where(o => o.Pid == item).Sum(o => o.Num),
                        ActivityId = requestModel.ActivityId,
                        OrderStatus = 1
                    };
                    list.Add(model);
                }
            }
            //有传orderid就保存记录
            if (list.Where(o => o.OrderId > 0)?.Count() > 0)
            {
                result = await DalDiscountActivityInfo.SaveCreateOrderDiscountInfo(list);
                if (!result)
                {
                    result = false;
                    return OperationResult.FromError<bool>("3", "保存订单打折信息失败");
                }
            }
            // 刷新缓存
            //1.活动商品已售数量缓存
            var productSoldList = orderInfoList.Select(a => new ActivityPidSoldNumModel
            {
                ActivityId = a.ActivityId,
                Pid = a.Pid
            }).ToList();
            await SetActivityProductSoldNumCache(productSoldList);
            //2.活动用户已购数量
            var userBuyList = orderInfoList.Select(a => new UserActivityBuyNumModel
            {
                ActivityId = a.ActivityId,
                UserId = a.UserId
            }).ToList();
            await SetActivityUserBuyNumCache(userBuyList);
            //3.验证下架
            await OrderUnShelveActivityAndProduct(list);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 取消订单时 更新订单打折活动信息 刷新缓存
        /// </summary>
        /// <param name="orderInfoList"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> UpdateCancelOrderDiscountInfoAndSetCache(int orderId)
        {
            var orderInfoList = await DalDiscountActivityInfo.GetDiscountOrderInfoByOid(orderId);
            if (orderInfoList?.Count() > 0)//订单存在打折记录
            {
                //改变记录状态
                var result = await DalDiscountActivityInfo.UpdateCancelOrderDiscountInfo(orderId);
                if (!result)
                {
                    Logger.Warn($"取消订单时更新订单打折信息记录失败，订单id:{orderId}");
                }
                //刷新缓存
                //1.活动商品已售数量缓存,更新活动商品表的已售数量
                var productSoldList = orderInfoList.Select(a => new ActivityPidSoldNumModel
                {
                    ActivityId = a.ActivityId,
                    Pid = a.Pid
                }).ToList();
                await SetActivityProductSoldNumCache(productSoldList);
                //2.活动用户已购数量
                var userBuyList = orderInfoList.Select(a => new UserActivityBuyNumModel
                {
                    ActivityId = a.ActivityId,
                    UserId = a.UserId
                }).ToList();
                await SetActivityUserBuyNumCache(userBuyList);
                //3.验证上架
                await OrderOnShelveActivityAndProduct(orderInfoList.ToList());
                return OperationResult.FromResult(result);
            }
            else
            {
                return OperationResult.FromResult(true);
            }
        }

        #region 缓存

        /// <summary>
        /// 批量获取用户活动已购数量缓存数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityIdList"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<UserActivityBuyNumModel>> GetOrSetUserActivityBuyNumCache(string userId, List<string> activityIdList)
        {
            if (string.IsNullOrEmpty(userId) || !(activityIdList?.Count > 0))
            {
                return null;
            }
            activityIdList = activityIdList.Distinct().ToList();
            var resultList = new List<UserActivityBuyNumModel>();
            var cacheKeyDic = activityIdList.ToDictionary(
                p => GetActivityUserBuyNumCacheKey(p, userId),
                p => p);
            using (var cacheClient = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                //批量读取缓存
                var cacheResultList = await cacheClient.GetAsync<int>(cacheKeyDic.Select(p => p.Key));

                //获取缓存成功的数据
                var successCache = cacheResultList.Where(ca => ca.Value.Success).Select(ca =>
                {
                    cacheKeyDic.TryGetValue(ca.Key, out string actId);
                    return new UserActivityBuyNumModel
                    {
                        ActivityId = actId,
                        UserId = userId,
                        BuyNum = ca.Value.Value
                    };
                });
                resultList.AddRange(successCache.Where(p => !string.IsNullOrEmpty(p.ActivityId)) ?? new List<UserActivityBuyNumModel>());

                //缓存读取失败的重新计算缓存
                var failActivityKey = cacheResultList.Where(ca => !ca.Value.Success).Select(p => p.Key);
                foreach (var p in failActivityKey)
                {
                    if (!cacheKeyDic.TryGetValue(p, out string value))
                    {
                        Logger.Warn("GetOrSetUserActivityBuyNumCache，从字典中获取活动id失败");
                    }
                    var buyNUm = await GetOrSetUserActivityBuyNumCache(userId, value);
                    resultList.Add(new UserActivityBuyNumModel()
                    {
                        ActivityId = value,
                        UserId = userId,
                        BuyNum = buyNUm
                    });
                }
                return activityIdList.Select(p =>
                    resultList.FirstOrDefault(pp => pp.ActivityId == p) ?? new UserActivityBuyNumModel
                    {
                        ActivityId = p,
                        BuyNum = 0,
                        UserId = userId
                    });
            }
        }

        /// <summary>
        /// 批量获取活动商品已售数量缓存数据
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ActivityPidSoldNumModel>> GetOrSetActivityProductSoldNumCache(string activityId, List<string> pidList)
        {
            if (string.IsNullOrEmpty(activityId) || !(pidList?.Count > 0))
            {
                return null;
            }
            pidList = pidList.Distinct().ToList();
            var resultList = new List<ActivityPidSoldNumModel>();
            var cacheKeyDic = pidList.ToDictionary(
                p => GetActivityProductSoldNumCacheKey(activityId, p),
                p => p);
            using (var cacheClient = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                //批量读取缓存
                var cacheResultList = await cacheClient.GetAsync<int>(cacheKeyDic.Select(p => p.Key));
                //获取缓存成功的数据
                var successCache = cacheResultList.Where(ca => ca.Value.Success).Select(ca =>
                {
                    if (!cacheKeyDic.TryGetValue(ca.Key, out string pid))
                    {
                        Logger.Warn("GetOrSetActivityProductSoldNumCache，从字典中获取pid失败");
                    }
                    return new ActivityPidSoldNumModel()
                    {
                        ActivityId = activityId,
                        Pid = pid,
                        SoldNum = ca.Value.Value
                    };
                });
                resultList.AddRange(successCache.Where(c => !string.IsNullOrWhiteSpace(c.Pid)) ?? new List<ActivityPidSoldNumModel>());

                //缓存读取失败的重新计算缓存
                var failPidKey = cacheResultList.Where(ca => !ca.Value.Success).Select(p => p.Key);
                foreach (var item in failPidKey)
                {
                    if (!cacheKeyDic.TryGetValue(item, out string pid))
                    {
                        Logger.Warn("GetOrSetActivityProductSoldNumCache，从字典中获取pid失败");
                    }
                    var soldNum = await GetOrSetActivityProductSoldNumCache(activityId, pid);
                    var model = new ActivityPidSoldNumModel()
                    {
                        ActivityId = activityId,
                        Pid = pid,
                        SoldNum = soldNum
                    };
                    resultList.Add(model);
                }
                return pidList.Select(p =>
                    resultList.FirstOrDefault(pp => pp.Pid == p) ?? new ActivityPidSoldNumModel
                    {
                        ActivityId = activityId,
                        Pid = p,
                        SoldNum = 0
                    });
            }
        }

        /// <summary>
        /// 临时增加活动商品已售数量缓存
        /// </summary>
        /// <param name="activityid"></param>
        /// <param name="pid"></param>
        /// <param name="increaseNum"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static async Task<bool> IncreaseActivityProductSoldNumCache(string activityid, string pid, int increaseNum, TimeSpan timeSpan)
        {
            if (string.IsNullOrWhiteSpace(activityid) || string.IsNullOrWhiteSpace(pid))
            {
                return false;
            }
            bool result = false;
            int oldNum = await GetOrSetActivityProductSoldNumCache(activityid, pid);
            using (var helper = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                string key = GetActivityProductSoldNumCacheKey(activityid, pid);
                var cacheResult = await helper.SetAsync(key, oldNum + increaseNum, timeSpan);
                if (!cacheResult.Success)
                {
                    result = false;
                    Logger.Warn($"IncreaseActivityProductSoldNumCache临时增加活动商品已售数量缓存失败：{cacheResult.Exception}");
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 临时增加用户活动已购数量缓存
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityId"></param>
        /// <param name="increaseNum"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static async Task<bool> IncreaseUserActivityBuyNumCache(string userId, string activityId, int increaseNum, TimeSpan timeSpan)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(activityId))
            {
                return false;
            }
            bool result = false;
            int oldNum = await GetOrSetUserActivityBuyNumCache(userId, activityId);
            using (var helper = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                string key = GetActivityUserBuyNumCacheKey(activityId, userId);
                var cacheResult = await helper.SetAsync(key, oldNum + increaseNum, timeSpan);
                if (!cacheResult.Success)
                {
                    result = false;
                    Logger.Warn($"IncreaseUserActivityBuyNumCache临时增加用户活动已购数量缓存失败：{cacheResult.Exception}");
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 设置并获取用户活动已购数量缓存计数器
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<int> GetOrSetUserActivityBuyNumCache(string userId, string activityId)
        {
            var userBuyNum = 0;
            using (var cacheClient = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                string cacheKey = GetActivityProductSoldNumCacheKey(activityId, userId);
                var BuyNumResult = await cacheClient.GetOrSetAsync(
                    cacheKey,
                    () => DalDiscountActivityInfo.SumActivityUserBuyNumAsync(userId, activityId),
                    TimeSpan.FromDays(7));
                if (!BuyNumResult.Success)
                {
                    Logger.Warn($"GetProductAndUserDiscountInfoAsync获取redis数据失败,缓存key:{cacheKey};Error:{BuyNumResult.Message}", BuyNumResult.Exception);
                    //缓存失败从数据库取
                    userBuyNum = await DalDiscountActivityInfo.SumActivityUserBuyNumAsync(userId, activityId);
                }
                else
                {
                    userBuyNum = BuyNumResult.Value;
                }
            }
            return userBuyNum;
        }

        /// <summary>
        /// 设置并获取活动商品已售数量缓存计数器
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static async Task<int> GetOrSetActivityProductSoldNumCache(string activityId, string pid)
        {
            var soldNum = 0;
            using (var cacheClient = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                string cacheKey = GetActivityProductSoldNumCacheKey(activityId, pid);
                var soldNumResult = await cacheClient.GetOrSetAsync(cacheKey,
                           () => DalDiscountActivityInfo.SumActivityProductSoldNumAsync(activityId, pid),
                           TimeSpan.FromDays(7));
                if (!soldNumResult.Success)
                {
                    Logger.Warn($"GetOrSetActivityProductSoldNumCache获取redis数据失败,缓存key:{cacheKey};Error:{soldNumResult.Message}", soldNumResult.Exception);
                    soldNum = await DalDiscountActivityInfo.SumActivityProductSoldNumAsync(activityId, pid);
                }
                else
                {
                    soldNum = soldNumResult.Value;
                }
            }
            return soldNum;
        }

        /// <summary>
        /// 批量活动活动商品的已售数量
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static async Task<List<ActivityPidSoldNumModel>> GetOrSetActivitysProductsSoldNumCache(List<ActivityPidSoldNumModel> list)
        {
            var resultList = new List<ActivityPidSoldNumModel>();
            //缓存key
            // var activityPidKeys = list.Select(a => GetActivityProductSoldNumCacheKey(a.ActivityId, a.Pid));
            var cacheKeyDic = list.ToDictionary(
               p => GetActivityProductSoldNumCacheKey(p.ActivityId, p.Pid),
               p => p.ActivityId + "/" + p.Pid);

            using (var cacheClient = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                //批量读取缓存
                var cacheResultList = await cacheClient.GetAsync<int>(cacheKeyDic.Select(c => c.Key));
                #region 缓存成功的数据
                var cacheSuccessList = cacheResultList?.Where(ca => ca.Value.Success);
                if (cacheSuccessList?.Count() > 0)
                {
                    foreach (var ca in cacheSuccessList)
                    {
                        if (!cacheKeyDic.TryGetValue(ca.Key, out string activityIdAndPid))
                        {
                            Logger.Warn("GetOrSetActivitysProductsSoldNumCache，从字典中获取活动id+pid失败");
                        }
                        var arr = activityIdAndPid.Split('/');
                        var activityId = "";
                        var pid = "";
                        if (arr.Length == 2)
                        {
                            activityId = arr[0];
                            pid = arr[1];
                        }
                        var model = new ActivityPidSoldNumModel()
                        {
                            ActivityId = activityId,
                            Pid = pid,
                            SoldNum = ca.Value.Value
                        };
                        resultList.Add(model);
                    }
                }
                #endregion

                #region 缓存读取失败的重新计算并设置缓存
                var cacheFailList = cacheResultList?.Where(ca => !ca.Value.Success);
                if (cacheFailList?.Count() > 0)
                {
                    //请求数据
                    var failDBRequest = new List<ActivityPidSoldNumModel>();
                    foreach (var ca in cacheFailList)
                    {
                        if (!cacheKeyDic.TryGetValue(ca.Key, out string activityIdAndPid))
                        {
                            Logger.Warn("GetOrSetActivitysProductsSoldNumCache，从字典中获取活动id+pid失败");
                        }
                        var arr = activityIdAndPid.Split('/');
                        var activityId = "";
                        var pid = "";
                        if (arr.Length == 2)
                        {
                            activityId = arr[0];
                            pid = arr[1];
                        }
                        failDBRequest.Add(new ActivityPidSoldNumModel()
                        {
                            ActivityId = activityId,
                            Pid = pid
                        });
                    }

                    //获取db查询数据
                    var dbResult = await DalDiscountActivityInfo.SumActivityProductSoldNumAsync(failDBRequest);
                    if (dbResult?.Count() > 0)
                    {
                        resultList.AddRange(dbResult);
                        //添加缓存
                        foreach (var item in dbResult)
                        {
                            string key = GetActivityProductSoldNumCacheKey(item.ActivityId, item.Pid);
                            var cacheResult = await cacheClient.SetAsync(key, item.SoldNum, TimeSpan.FromDays(7));
                            if (!cacheResult.Success)
                            {
                                Logger.Warn($"GetOrSetActivitysProductsSoldNumCache刷新活动商品已售数量缓存失败：{cacheResult.Exception}");
                            }
                        }
                    }
                }
                #endregion
            }
            return resultList;
        }

        /// <summary>
        /// 批量刷新活动商品已售数量缓存，更商品已售数量记录
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public static async Task<bool> SetActivityProductSoldNumCache(List<ActivityPidSoldNumModel> list)
        {
            //查询商品已售数量
            bool result = true;
            var soldNumList = await DalDiscountActivityInfo.SumActivityProductSoldNumAsync(list);
            if (soldNumList?.Count() > 0)
            {
                //更新活动商品表中已售数量
                await DalDiscountActivityInfo.UpdateProductSoldQuantity(soldNumList.ToList());
                //刷新缓存
                using (var helper = CacheHelper.CreateCacheClient(DefaultClientName))
                {
                    foreach (var item in soldNumList)
                    {
                        string key = GetActivityProductSoldNumCacheKey(item.ActivityId, item.Pid);
                        var cacheResult = await helper.SetAsync(key, item.SoldNum, TimeSpan.FromDays(7));
                        if (!cacheResult.Success)
                        {
                            result = false;
                            Logger.Warn($"SetActivityProductSoldNumCache刷新活动商品已售数量缓存失败：{cacheResult.Exception}");
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 批量刷新用户活动已购数量缓存
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static async Task<bool> SetActivityUserBuyNumCache(List<UserActivityBuyNumModel> list)
        {
            //批量查询用户活动已购数量 
            bool result = true;
            var buyNumList = await DalDiscountActivityInfo.SumActivityUserBuyNumAsync(list[0]?.UserId, list.Select(a => a.ActivityId)?.Distinct().ToList());
            using (var helper = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                foreach (var item in buyNumList)
                {
                    string key = GetActivityUserBuyNumCacheKey(item.ActivityId, item.UserId);
                    var cacheResult = await helper.SetAsync(key, item.BuyNum, TimeSpan.FromDays(7));
                    if (!cacheResult.Success)
                    {
                        result = false;
                        Logger.Warn($"SetActivityUserBuyNumCache刷新用户活动已购数量缓存失败：{cacheResult.Exception}");
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 获取活动商品已售数量缓存key
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        private static string GetActivityProductSoldNumCacheKey(string activityId, string pid)
        {
            return $"{ActivityProductSoldNumCachePrefix}/{activityId}/{pid}";
        }

        /// <summary>
        /// 获取活动用户已购商品数缓存key
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        private static string GetActivityUserBuyNumCacheKey(string activityId, string userid)
        {
            return $"{ActivityUserBuyNumCachePrefix}/{activityId}/{userid}";
        }

        #endregion

        /// <summary>
        ///从数据库获取商品活动信息
        /// </summary>
        /// <param name="pidList"></param>
        /// <returns></returns>
        private static async Task<List<ProductDiscountActivityInfo>> GetDiscountActivityInfoByPidsFromDB(List<string> pidList)
        {
            var resultList = new List<ProductDiscountActivityInfo>();
            //1.从数据库获取活动和商品信息
            var list = await DalDiscountActivityInfo.GetActivityInfoByPids(pidList);
            if (list?.Count > 0)
            {
                foreach (var item in pidList)
                {
                    var model = new ProductDiscountActivityInfo()
                    {
                        Pid = item,
                        HasDiscountActivity = false
                    };
                    var discountTag = list.FirstOrDefault(a => a.Pid == item);
                    if (discountTag?.DiscountActivityList.Count > 0 && !string.IsNullOrWhiteSpace(discountTag.DiscountActivityList[0].ActivityId))
                    {
                        var tagActivity = discountTag.DiscountActivityList[0];
                        model.HasDiscountActivity = true;
                        model.ImageUrl = tagActivity.ImageUrl;
                        model.DiscountStock = tagActivity.LimitQuantity;
                        model.DiscountActivityInfo = new DiscountActivityAndUserLimitInfo()
                        {
                            ActivityId = tagActivity.ActivityId,
                            Description = tagActivity.Description,
                            Banner = tagActivity.Banner,
                            DiscountType = tagActivity.DiscountType,
                            IsDefaultLabel = tagActivity.IsDefaultLabel,
                            Label = tagActivity.IsDefaultLabel ? "折" : tagActivity.Label,
                            RuleList = tagActivity.RuleList,
                            IsUserPurchaseLimit = tagActivity.IsUserPurchaseLimit,
                            UserLimitNum = tagActivity.UserLimitNum
                        };
                        resultList.Add(model);
                    }
                }
                //获取活动商品已售数量
                var soldNumCacheRequest = new List<ActivityPidSoldNumModel>();
                foreach (var item in resultList)
                {
                    if (item.HasDiscountActivity)
                    {
                        var requestModel = new ActivityPidSoldNumModel()
                        {
                            ActivityId = item.DiscountActivityInfo?.ActivityId,
                            Pid = item.Pid
                        };
                        soldNumCacheRequest.Add(requestModel);
                    }
                }
                var soldNumCacheList = await GetOrSetActivitysProductsSoldNumCache(soldNumCacheRequest);
                foreach (var item in resultList)
                {
                    var ableNum = item.DiscountStock - (int)soldNumCacheList.FirstOrDefault(a => a.Pid == item.Pid)?.SoldNum;
                    item.DiscountStock = ableNum > 0 ? ableNum : 0;
                }
            }
            return resultList;
        }

        #region 自动上下架

        /// <summary>
        /// 审核活动时自动上下架活动商品和上架活动(补充库存)
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<bool> AuditShelveActivityProduct(string activityId)
        {
            var productList = await DalDiscountActivityInfo.GetProductListByActivityId(activityId);
            if (productList?.Count > 0)
            {
                //从缓存获取商品已售数量
                var activityPidSoldNumList = await BindSoldNumList(activityId, productList.ToList());

                //1.已经下架的商品
                var unShelvedList = activityPidSoldNumList.Where(p => p.Is_UnShelve == 1);
                if (unShelvedList?.Count() > 0)
                {
                    //判断并上架活动商品
                    await OnShelveProduct(activityId, unShelvedList.ToList());
                }

                //2.已经上架的商品
                var onShelvedList = activityPidSoldNumList.Where(p => p.Is_UnShelve == 0);
                if (onShelvedList?.Count() > 0)
                {
                    //判断并下架活动商品
                    await UnShelveProduct(activityId, onShelvedList.ToList());
                }

                //3.上架活动
                await OnShelveActivity(activityId);
                //4.下架活动
                await UnShelveActivity(activityId,null);
            }
            return true;
        }

        #region 上架

        /// <summary>
        /// 取消订单上架活动和活动商品
        /// </summary>
        /// <param name="activitysAndPids"></param>
        /// <returns></returns>
        public static async Task<bool> OrderOnShelveActivityAndProduct(List<SalePromotionActivityDiscountOrder> activitysAndPids)
        {
            if (!(bool)(activitysAndPids?.Any(a => !string.IsNullOrWhiteSpace(a.ActivityId) && !string.IsNullOrWhiteSpace(a.Pid))))
            {
                return false;
            }
            //1.批量获取活动商品信息
            var productList = await DalDiscountActivityInfo.GetProductListByActivityIdAsync(activitysAndPids);
            var activityIdList = activitysAndPids?.Select(a => a.ActivityId)?.Distinct();
            if (productList?.Count > 0 && activityIdList?.Count() > 0)
            {
                //遍历参数的活动id
                foreach (var item in activityIdList)
                {
                    //获取当前活动的下架商品- 仅是订单传递过来的pid
                    var onProductLost = productList.Where(p => p.ActivityId == item && p.Is_UnShelve == 1);
                    if (onProductLost?.Count() > 0)
                    {
                        //从缓存获取活动商品已售数量,添加到集合
                        var productInfoList = await BindSoldNumList(item, onProductLost.ToList());
                        //1.验证并上架商品
                        int onShelveCount = await OnShelveProduct(item, productInfoList);
                    }
                    //2.验证并上架活动
                    await OnShelveActivity(item);
                }
            }
            return true;
        }

        /// <summary>
        /// 上架活动商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="productList"></param>
        /// <returns></returns>
        public static async Task<int> OnShelveProduct(string activityId, List<SalePromotionActivityProductDB> productList)
        {
            int result = 0;
            //1.获取活动已上架的商品
            var onShelvePidList = productList.Where(p => p.LimitQuantity > 0 && p.LimitQuantity > p.SoldQuantity)?.Select(p => p.Pid).ToList();
            if (onShelvePidList?.Count > 0)
            {
                //2.验证和执行商品上架
                result = await DalDiscountActivityInfo.OnShelveProduct(activityId, onShelvePidList);
                if (result <= 0)
                {
                    Logger.Warn($"OnShelveActivity,自动上架活动商品失败，活动id:{activityId},pid：{string.Join(",", onShelvePidList)}");
                }
                else
                {
                    //刷新标签缓存
                    NotifyRefreshProductCommonTag(onShelvePidList);
                    #region 记录操作日志
                    var operationLogModel = new SalePromotionActivityLogModel()
                    {
                        ReferId = activityId,
                        ReferType = "SalePromotionActivity",
                        OperationLogType = "SalePromotion_AutoOnShelveProduct",
                        CreateDateTime = DateTime.Now.ToString(),
                        CreateUserName = "系统自动操作",
                    };
                    StringBuilder newValueBuilder = new StringBuilder();
                    string newValue = string.Empty;
                    foreach (var item in onShelvePidList)
                    {
                        newValueBuilder.Append($"商品PID:{item};");
                    }
                    if (newValueBuilder.Length > 0)
                    {
                        newValue = newValueBuilder.ToString().Substring(0, newValueBuilder.Length - 1);
                        operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                        {
                            Property = "自动上架商品",
                            NewValue = newValue,
                        });
                    }
                    await SetOperationLog(operationLogModel, "OnShelveProduct");
                    #endregion
                }
            }
            return result;
        }

        /// <summary>
        /// 上架活动
        /// </summary>
        /// <param name="activityId"></param>
        private static async Task<bool> OnShelveActivity(string activityId)
        {
            //1.获取活动信息
            var activityModel = await DalSalePromotionActivity.GetActivityInfoAsync(activityId);//写库
            if (activityModel?.DiscountContentList?.Count > 0 && activityModel.Is_UnShelveAuto == 1)
            {
                //2.获取活动所有商品
                var actPList = await DalDiscountActivityInfo.GetProductListByActivityId(activityId);
                if (actPList?.Count > 0)
                {
                    var soldList = await BindSoldNumList(activityId, actPList?.ToList());
                    //3.验证是否需要上架活动
                    int discountMethod = activityModel.DiscountContentList[0].DiscountMethod;
                    decimal minCondition = activityModel.DiscountContentList.Min(d => d.Condition);
                    decimal activitySum = 0;//活动剩余有效的商品数量(金额)累加
                    if (discountMethod == 1)//满额
                    {
                        activitySum = (decimal)soldList.Where(p => p.LimitQuantity > 0 && p.LimitQuantity > p.SoldQuantity)?
                           .Sum(p => p.SalePrice * (p.LimitQuantity - p.SoldQuantity));
                    }
                    else//满件
                    {
                        activitySum = (decimal)soldList.Where(p => p.LimitQuantity > 0 && p.LimitQuantity > p.SoldQuantity)?
                           .Sum(p => p.LimitQuantity - p.SoldQuantity);
                    }
                    if (activitySum >= minCondition)
                    {
                        //4.满足打折最低条件,上架
                        int onShelveActivityResult = await DalDiscountActivityInfo.SetActivityShelveStatus(activityId, 0, actPList.Select(a => a.Pid).ToList());
                        if (onShelveActivityResult != 1)
                        {
                            Logger.Warn($"OnShelveActivity,自动上架活动失败，活动id:{activityId}");
                        }
                        else
                        {
                            //刷新标签缓存
                            //获取活动的pid
                            var pList = soldList?.Select(p => p.Pid);
                            NotifyRefreshProductCommonTag(pList.ToList());
                            //记录操作日志
                            var operationLogModel = new SalePromotionActivityLogModel()
                            {
                                ReferId = activityId,
                                ReferType = "SalePromotionActivity",
                                OperationLogType = "SalePromotion_AutoOnShelveActivity",
                                CreateDateTime = DateTime.Now.ToString(),
                                CreateUserName = "系统自动操作",
                            };
                            await SetOperationLog(operationLogModel, "OnShelveActivity");
                        }
                    }
                }
            }
            return true;
        }

        #endregion

        #region 下架

        /// <summary>
        /// 创建订单 下架活动和活动商品
        /// </summary>
        /// <param name="activitysAndPids"></param>
        /// <returns></returns>
        public static async Task<bool> OrderUnShelveActivityAndProduct(List<SalePromotionActivityDiscountOrder> activitysAndPids)
        {
            if (!(bool)(activitysAndPids?.Any(a => !string.IsNullOrWhiteSpace(a.ActivityId) && !string.IsNullOrWhiteSpace(a.Pid))))
            {
                return false;
            }
            //1.批量获取活动商品信息
            var productList = await DalDiscountActivityInfo.GetProductListByActivityIdAsync(activitysAndPids);
            var activityIdList = activitysAndPids?.Select(a => a.ActivityId)?.Distinct() ?? new List<string>();
            if (productList?.Count > 0 && activityIdList?.Count() > 0)
            {
                foreach (var item in activityIdList)
                {
                    //2.获取活动pid参数中已上架的商品
                    var onShelvePList = productList.Where(p => p.ActivityId == item && p.Is_UnShelve == 0);
                    if (onShelvePList?.Count() > 0)
                    {
                        //缓存绑定已售数量
                        var pSoldList = await BindSoldNumList(item, onShelvePList.ToList());
                        //3.下架商品
                        int unShelveCount = await UnShelveProduct(item, pSoldList);
                        //4.验证和下架活动
                        await UnShelveActivity(item, pSoldList);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 下架活动商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="productList"></param>
        /// <returns></returns>
        public static async Task<int> UnShelveProduct(string activityId, List<SalePromotionActivityProductDB> productList)
        {
            int result = 0;
            //1.获取需要下架的商品
            var unShelvePidList = productList.Where(p => p.LimitQuantity == 0 || p.LimitQuantity <= p.SoldQuantity)?.Select(p => p.Pid).ToList();
            if (unShelvePidList?.Count > 0)
            {
                //2.执行商品下架
                result = await DalDiscountActivityInfo.UnShelveProduct(activityId, unShelvePidList);
                if (result <= 0)
                {
                    Logger.Warn($"UnShelveProduct,自动下架活动商品失败，活动id:{activityId},活动pid:{string.Join(",", unShelvePidList)}");
                }
                else
                {
                    //刷新标签缓存
                    NotifyRefreshProductCommonTag(unShelvePidList);
                    #region 记录日志
                    var operationLogModel = new SalePromotionActivityLogModel()
                    {
                        ReferId = activityId,
                        ReferType = "SalePromotionActivity",
                        OperationLogType = "SalePromotion_AutoUnShelveProduct",
                        CreateDateTime = DateTime.Now.ToString(),
                        CreateUserName = "系统自动操作",
                    };
                    StringBuilder newValueBuilder = new StringBuilder();
                    string newValue = string.Empty;
                    foreach (var item in unShelvePidList)
                    {
                        newValueBuilder.Append($"商品PID:{item};");
                    }
                    if (newValueBuilder.Length > 0)
                    {
                        newValue = newValueBuilder.ToString().Substring(0, newValueBuilder.Length - 1);
                        operationLogModel.LogDetailList.Add(new SalePromotionActivityLogDetail()
                        {
                            Property = "自动下架商品",
                            NewValue = newValue,
                        });
                    }
                    await SetOperationLog(operationLogModel, "UnShelveProduct");
                    #endregion
                }
            }
            return result;
        }

        /// <summary>
        /// 下架活动
        /// </summary>
        /// <param name="activityId"></param>
        private static async Task<int> UnShelveActivity(string activityId, List<SalePromotionActivityProductDB> requestPList)
        {
            int result = 0;
            //获取活动信息
            var activityModel = await DalSalePromotionActivity.GetActivityInfoAsync(activityId);
            if (activityModel?.DiscountContentList?.Count > 0 && activityModel.Is_UnShelveAuto == 0)
            {
                //1.先验证本次订单商品是否低于最低打折要求
                int discountMethod = activityModel.DiscountContentList[0].DiscountMethod;
                decimal minCondition = activityModel.DiscountContentList.Min(d => d.Condition);
                decimal activitySum = 0;//活动剩余有效的商品数量(金额)累加
                if (requestPList?.Count > 0)
                {
                    if (discountMethod == 1)
                    {
                        activitySum = (decimal)requestPList.Where(p => p.LimitQuantity > 0 && p.LimitQuantity > p.SoldQuantity)?
                               .Sum(p => p.SalePrice * (p.LimitQuantity - p.SoldQuantity));
                    }
                    else
                    {
                        activitySum = (decimal)requestPList.Where(p => p.LimitQuantity > 0 && p.LimitQuantity > p.SoldQuantity)?
                              .Sum(p => p.LimitQuantity - p.SoldQuantity);
                    }
                    if (activitySum >= minCondition)
                    {
                        return 0;//当前购买商品的剩余数量满足上架条件
                    }
                }
                //获取活动商品信息
                var actPList = await DalDiscountActivityInfo.GetProductListByActivityId(activityId);
                if (actPList?.Count() > 0)
                {
                    var actPInfoList = await BindSoldNumList(activityId, actPList.ToList());
                    //2.验证是否需要下架
                    if (discountMethod == 1)//满额
                    {
                        activitySum = (decimal)actPInfoList.Where(p => p.LimitQuantity > 0 && p.LimitQuantity > p.SoldQuantity)?
                           .Sum(p => p.SalePrice * (p.LimitQuantity - p.SoldQuantity));
                    }
                    else//满件
                    {
                        activitySum = (decimal)actPInfoList.Where(p => p.LimitQuantity > 0 && p.LimitQuantity > p.SoldQuantity)?
                           .Sum(p => p.LimitQuantity - p.SoldQuantity);
                    }
                    if (activitySum < minCondition)
                    {
                        //不满足打折最低条件,下架
                        result = await DalDiscountActivityInfo.SetActivityShelveStatus(activityId, 1, actPList.Select(a => a.Pid).ToList());
                        if (result != 1)
                        {
                            Logger.Warn($"UnShelveActivity,自动下架活动失败，活动id:{activityId}");
                        }
                        else
                        {
                            //刷新标签缓存
                            //获取活动的pid
                            var pList = actPInfoList?.Select(p => p.Pid);
                            NotifyRefreshProductCommonTag(pList.ToList());
                            //记录操作日志
                            var operationLogModel = new SalePromotionActivityLogModel()
                            {
                                ReferId = activityId,
                                ReferType = "SalePromotionActivity",
                                OperationLogType = "SalePromotion_AutoUnShelveActivity",
                                CreateDateTime = DateTime.Now.ToString(),
                                CreateUserName = "系统自动操作",
                            };
                            await SetOperationLog(operationLogModel, "UnShelveActivity");
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 通知刷新标签缓存
        /// </summary>
        /// <param name="pidList"></param>
        private static void NotifyRefreshProductCommonTag(List<string> pidList)
        {
            try
            {
                if (pidList?.Count > 0)
                {
                    var data = new
                    {
                        type = "RebuildCache",
                        pids = pidList,
                        tag = ProductCommonTag.Discount
                    };
                    TuhuNotification.SendNotification("notification.productModify.ProductCommonTag", data);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"通知标签缓存失败ex{ex},pid:{string.Join(",", pidList)}"); ;
            }
        }

        /// <summary>
        /// 绑定商品已售数量 从缓存获取
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private static async Task<List<SalePromotionActivityProductDB>> BindSoldNumList(string activityId, List<SalePromotionActivityProductDB> list)
        {
            var resultList = new List<SalePromotionActivityProductDB>();
            if (!(list?.Count > 0))
            {
                return resultList;
            }
            var pidList = list.Select(p => p.Pid).Distinct().ToList();
            //从缓存获取该活动商品已售数量,添加到集合
            var pAndSoldNumList = (await GetOrSetActivityProductSoldNumCache(activityId, pidList));
            if (pAndSoldNumList?.Count() > 0)
            {
                foreach (var item in list)
                {
                    item.SoldQuantity = (int)pAndSoldNumList.FirstOrDefault(a => a.Pid == item.Pid)?.SoldNum;
                    resultList.Add(item);
                }
            }
            return resultList;
        }

        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="operationLogModel"></param>
        /// <param name="funNameString"></param>
        private static async Task<bool> SetOperationLog(SalePromotionActivityLogModel operationLogModel, string funNameString)
        {
            try
            {
                var logResult = await SalePromotionActivityLogManager.InsertAcitivityLogAndDetailAsync(operationLogModel);
                if (!(logResult.Success && logResult.Result))
                {
                    Logger.Warn($"{funNameString}操作日志记录失败ErrorMessage:{logResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{funNameString}操作日志记录异常:{ex}");
            }
            return true;
        }

        #endregion
    }
}

