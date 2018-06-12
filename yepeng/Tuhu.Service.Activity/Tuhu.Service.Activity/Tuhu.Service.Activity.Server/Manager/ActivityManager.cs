using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Tuhu.Models;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.DataAccess.Models;
using Tuhu.Service.Activity.DataAccess.Models.Activity;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Activity.Server.Model;
using Tuhu.Service.Activity.Server.Utils;
using Tuhu.Service.BaoYang;
using Tuhu.Service.Config.Models;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Models;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Product.Models.New;
using Tuhu.Service.Product.Models.ProductConfig;
using Tuhu.Service.Product.Request;
using Tuhu.Service.UserAccount;
using Tuhu.ZooKeeper;
using FlashSaleProductModel = Tuhu.Service.Activity.Models.FlashSaleProductModel;
using static Tuhu.Service.Order.Enum.OrderEnum;
using Tuhu.Service.Order.Enum;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models.Push;
using Tuhu.Service.Activity.DataAccess.Questionnaire;
using Tuhu.Service.Activity.Models.Questionnaire;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
<<<<<<< HEAD
using Tuhu.MessageQueue;
=======
using Tuhu.Service.UserAccount.Models;
using Tuhu.Service.UserAccount.Enums;
>>>>>>> cef10ecfe95a62dbe27086896d29d0abab5da3d4

namespace Tuhu.Service.Activity.Server.Manager
{
    public static class ActivityManager
    {
        public static readonly string DefaultClientName = "Activity";
        public static readonly string WashCarActivityClientName = "UserApplyActivitySortedSetCache";
        public static readonly TimeSpan ActivityCacheExpiration = TimeSpan.FromHours(1);
        public static readonly ILog Logger = LogManager.GetLogger(typeof(ActivityManager));


        public async static Task<DownloadApp> GetActivityConfigForDownloadApp(int id)
        {
            var client = CacheHelper.CreateCacheClient(DefaultClientName);
            var result = await client.GetOrSetAsync(string.Concat(GlobalConstant.ActivityConfigForDownloadApp, id), () => DalActivity.GetActivityConfigForDownloadApp(id), ActivityCacheExpiration);
            DownloadApp model = null;
            if (result.Success)
            {
                model = result.Value;
            }
            else
            {
                Logger.Warn($"获取redis数据失败GetActivityConfigForDownloadApp:{string.Concat(GlobalConstant.ActivityConfigForDownloadApp, id)};Error:{result.Message}", result.Exception);
            }
            return model;

        }

        public async static Task<bool> CleanActivityConfigForDownloadAppCache(int id)
        {
            var status = false;
            var client = CacheHelper.CreateCacheClient(DefaultClientName);
            var result = await client.RemoveAsync(string.Concat(GlobalConstant.ActivityConfigForDownloadApp, id));
            status = result.Success;
            return status;
        }

        public static async Task<TireActivityModel> SelectTireActivityAsync(string vehicleId, string tireSize)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.GetOrSetAsync(string.Concat("TireActivity/", vehicleId, tireSize), () => DalActivity.SelectTireActivity(vehicleId, tireSize), ActivityCacheExpiration);
                IEnumerable<TireActivityModel> tireActivity;
                if (result.Success)
                {
                    tireActivity = result.Value;
                }
                else
                {
                    Logger.Warn($"获取redis数据失败SelectTireActivityAsync:{string.Concat("TireActivity/", vehicleId, tireSize)};Error:{result.Message}", result.Exception);
                    // redis查询失败查数据库
                    tireActivity = await DalActivity.SelectTireActivity(vehicleId, tireSize);
                }
                if (tireActivity != null && tireActivity.Any())
                {
                    return tireActivity.FirstOrDefault(_ => _.StartTime <= DateTime.Now && _.EndTime > DateTime.Now);
                }
                return null;
            }
        }
        public static async Task<List<TireActivityModel>> SelectTireActivityListAsync(string vehicleId, string tireSize)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.GetOrSetAsync(string.Concat("TireActivity/", vehicleId, tireSize), () => DalActivity.SelectTireActivity(vehicleId, tireSize), ActivityCacheExpiration);
                IEnumerable<TireActivityModel> tireActivity;
                if (result.Success)
                {
                    tireActivity = result.Value;
                }
                else
                {
                    Logger.Warn($"获取redis数据失败SelectTireActivityAsync:{string.Concat("TireActivity/", vehicleId, tireSize)};Error:{result.Message}", result.Exception);
                    // redis查询失败查数据库
                    tireActivity = await DalActivity.SelectTireActivity(vehicleId, tireSize);
                }
                if (tireActivity != null && tireActivity.Any())
                {
                    return tireActivity.Where(_ => _.StartTime <= DateTime.Now && _.EndTime > DateTime.Now)?.ToList() ?? new List<TireActivityModel>();
                }
                return null;
            }
        }

        public async static Task<TireActivityModel> SelectTireActivityByActivityIdAsync(Guid activityId)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.GetOrSetAsync(string.Concat("TireActivityContent/", activityId), () => DalActivity.SelectTireActivityByActivityId(activityId), ActivityCacheExpiration);
                TireActivityModel tireActivity = null;
                if (result.Success)
                {
                    tireActivity = result.Value;
                }
                else
                {
                    Logger.Warn($"获取redis数据失败SelectTireActivityByActivityIdAsync:{string.Concat("TireActivityContent/", activityId)};Error:{result.Message}", result.Exception);
                    // redis查询失败查数据库
                    tireActivity = await DalActivity.SelectTireActivityByActivityId(activityId);
                }
                return tireActivity != null && tireActivity.StartTime <= DateTime.Now &&
                       tireActivity.EndTime > DateTime.Now
                    ? tireActivity
                    : null;

            }
        }

        public static async Task<IEnumerable<string>> SelectTireActivityPidsAsync(Guid activityId)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.GetOrSetAsync(string.Concat("TireActivity/", activityId), () => DalActivity.SelectTireActivityPids(activityId), ActivityCacheExpiration);
                if (result.Success)
                {
                    return result.Value;
                }
                else
                {
                    Logger.Warn($"获取redis数据失败SelectTireActivityPidsAsync:{string.Concat("TireActivity/", activityId)};Error:{result.Message}", result.Exception);
                    // redis查询失败查数据库
                    return await DalActivity.SelectTireActivityPids(activityId);
                }

            }
        }

        public static async Task<bool> UpadeTireActivityAsync(string vehicleId, string tireSize)
        {
            var data = await DalActivity.SelectTireActivity(vehicleId, tireSize, false);
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.SetAsync(string.Concat("TireActivity/", vehicleId, tireSize), data, ActivityCacheExpiration);
                return result.Success;
            }
        }
        public static async Task<bool> UpadeTireActivityPidsAsync(Guid activityId)
        {
            var data = await DalActivity.SelectTireActivityPids(activityId);
            var data2 = await DalActivity.SelectTireActivityByActivityId(activityId);
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.SetAsync(string.Concat("TireActivity/", activityId), data, ActivityCacheExpiration);
                var result2 = await client.SetAsync(string.Concat("TireActivityContent/", activityId), data2, ActivityCacheExpiration);
                return result.Success && result2.Success;
            }
        }

        /// <summary>
        /// 获取车型适配轮胎信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        public static async Task<List<VehicleAdaptTireTireSizeDetailModel>> SelectVehicleAaptTiresAsync(VehicleAdaptTireRequestModel request)
        {
            using (var cache = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var tireSizeDetails = new List<VehicleAdaptTireTireSizeDetailModel>();
                foreach (var tireSize in request.TireSizes)
                {
                    var cacheResult = await cache.GetOrSetAsync(request.VehicleId + tireSize + request.CurrentPage + request.PageSize, () => DalActivity.SelectVehicleAdaptTireAsync(request, tireSize), GlobalConstant.VehicleAdapterCacheExpiration);
                    IEnumerable<VehicleAdaptTireModel> vehicleAdaptTires;
                    if (cacheResult.Success)
                        vehicleAdaptTires = cacheResult.Value.ToArray();
                    else
                    {
                        Logger.Warn($"获取车型适配轮胎信息redis数据失败SelectVehicleAaptTiresAsync:{request.VehicleId + tireSize + request.CurrentPage + request.PageSize};Error:{cacheResult.Message}", cacheResult.Exception);
                        var dbResult = await DalActivity.SelectVehicleAdaptTireAsync(request, tireSize);
                        vehicleAdaptTires = dbResult.ToArray();
                    }
                    var pids = vehicleAdaptTires.Select(r => r.PID).ToList();
                    if (pids.Count == 0)
                    {
                        tireSizeDetails.Add(new VehicleAdaptTireTireSizeDetailModel()
                        {
                            TireSize = tireSize,
                            Products = null
                        });
                    }
                    else
                    {
                        using (var client = new ProductClient())
                        {
                            var models = await client.SelectSkuProductListByPidsAsync(pids);

                            if (models?.Result != null && models.Result.Any())
                            {
                                var skuDetailModels = models.Result.ToArray();
                                var detailModels = (from v in vehicleAdaptTires
                                                    join s in skuDetailModels on v.PID equals s.Pid
                                                    select new VehicleAdaptTireDetailModel
                                                    {
                                                        PKID = v.PKID,
                                                        PID = v.PID,
                                                        TireSize = v.TireSize,
                                                        VehicleId = v.VehicleId,
                                                        SalesOrder = v.SalesOrder,
                                                        Image = s.Image.GetImageUrl(),
                                                        SalePrice = s.Price,
                                                        DisplayName = s.DisplayName
                                                    }).ToArray();

                                tireSizeDetails.Add(new VehicleAdaptTireTireSizeDetailModel()
                                {
                                    TireSize = tireSize,
                                    Products = !detailModels.Any() ? null : detailModels
                                });
                            }
                            else
                            {
                                tireSizeDetails.Add(new VehicleAdaptTireTireSizeDetailModel()
                                {
                                    TireSize = tireSize,
                                    Products = null
                                });
                            }
                        }
                    }
                }
                return tireSizeDetails;
            }
        }

        public async static Task<TireActivityModel> SelectTireActivityNewAsync(TireActivityRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.VehicleId) || string.IsNullOrWhiteSpace(request.TireSize))
            {
                Logger.Info($"SelectTireActivityNewAsync参数为空无活动返回，参数：{JsonConvert.SerializeObject(request)}");
                return null;
            }
            else
            {
                var activitys = await SelectTireActivityListAsync(request.VehicleId, request.TireSize);
                if (activitys == null || !activitys.Any())
                    return null;
                else
                {
                    var pids = new List<string>();
                    foreach (var activity in activitys)
                    {
                        var result = await SelectTireActivityPidsAsync(activity.ActivityId);
                        activity.Pids = result?.ToList() ?? new List<string>();
                    };

                    if (request.CityID > 0)
                    {
                        using (var client = new ProductClient())
                        using (var clientSearch = new ProductSearchClient())
                        {
                            var pidList = activitys.SelectMany(p => p.Pids)?.Distinct().ToList() ?? new List<string>();
                            if (pidList.Any())
                            {
                                var limitResultAsync = clientSearch.SelectLimitRegionProductAsync(pidList, request.CityID);
                                var stockResultAsync = client.SelectProductsRegionStockAsync(request.CityID, pidList);
                                await Task.WhenAll(limitResultAsync, stockResultAsync);
                                var limitResult = limitResultAsync.Result?.Result ?? new List<ProductLimitCitys>();
                                var stockResult = stockResultAsync.Result?.Result ?? new List<ProductCityStock>();
                                if (limitResultAsync.Result == null || !limitResultAsync.Result.Success)
                                {
                                    Logger.Warn($"SelectLimitRegionProductAsync-->ErrorCode:{limitResultAsync.Result?.ErrorCode}-->ErrorMessage:{limitResultAsync.Result?.ErrorMessage}"
                                        , limitResultAsync.Result?.Exception);
                                }
                                if (stockResultAsync.Result == null || !stockResultAsync.Result.Success)
                                {
                                    Logger.Warn($"SelectProductsRegionStockAsync-->ErrorCode:{stockResultAsync.Result?.ErrorCode}-->ErrorMessage:{stockResultAsync.Result?.ErrorMessage}"
                                        , stockResultAsync.Result?.Exception);
                                }

                                var outPids = stockResult.Where(p => p.RegionStockNum != null && p.RegionStockNum < 8).Select(p => p.Pid)
                                    .Concat(limitResult.Where(p => p.IsLimit && p.LimitCitys != null && p.LimitCitys.Contains(request.CityID)).Select(p => p.Pid));
                                activitys.ForEach(p => p.Pids = p.Pids.Except(outPids).ToList());
                            }

                        }
                    }
                    return activitys.Where(p => p.Pids.Count() > 0).OrderBy(p => p.SortLevel).FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// 获取车型适配保养信息
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>

        public async static Task<IEnumerable<VehicleAdaptBaoyangModel>> SelectVehicleAaptBaoyangsAsync(string vehicleId)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                IEnumerable<VehicleAdaptBaoyangModel> vehicleAdaptBaoyangs;
                var cacheResult = await client.GetOrSetAsync(string.Concat("BaoyangActivity/", vehicleId), () => DalActivity.SelectVehicleAdaptBaoyangAsync(vehicleId), GlobalConstant.VehicleAdapterCacheExpiration);
                if (cacheResult.Success)
                {
                    vehicleAdaptBaoyangs = cacheResult.Value;
                }
                else
                {
                    Logger.Warn($"获取车型适配保养redis数据失败SelectVehicleAaptBaoyangsAsync:{string.Concat("BaoyangActivity/", vehicleId)};Error:{cacheResult.Message}", cacheResult.Exception);
                    var dbResult = await DalActivity.SelectVehicleAdaptBaoyangAsync(vehicleId);
                    vehicleAdaptBaoyangs = dbResult;
                }
                return vehicleAdaptBaoyangs;

            }
        }


        public async static Task<IEnumerable<string>> SelectVehicleSortedCategoryNamesAsync()
        {
            using (var cache = CacheHelper.CreateCacheClient())
            {
                IEnumerable<string> categoryNames;
                var cacheResult = await cache.GetOrSetAsync("categoryName", DalActivity.SelectVehicleSortedCategoryNamesAsync, GlobalConstant.VehicleAdapterCacheExpiration);
                if (cacheResult.Success)
                {
                    categoryNames = cacheResult.Value;
                }
                else
                {
                    Logger.Warn($"获取排序后的品类名称redis数据失败SelectVehicleSortedCategoryNamesAsync:{"categoryName"};Error:{cacheResult.Message}", cacheResult.Exception);
                    var dbResult = await DalActivity.SelectVehicleSortedCategoryNamesAsync();
                    categoryNames = dbResult;
                }
                return categoryNames;
            }
        }
        /// <summary>
        /// 获取车型适配车品信息
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public async static Task<IDictionary<string, IEnumerable<VehicleAdaptChepinDetailModel>>> SelectVehicleAdaptChepinsAsync(string vehicleId)
        {
            using (var cache = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                IEnumerable<VehicleAdaptChepinModel> vehicleAdaptChepins;
                var cacheResult = await cache.GetOrSetAsync(string.Concat("ChepinActivity/", vehicleId), () => DalActivity.SelectVehicleAdaptChepinAsync(vehicleId), GlobalConstant.VehicleAdapterCacheExpiration);
                {
                    if (cacheResult.Success)
                    {
                        vehicleAdaptChepins = cacheResult.Value.ToArray();
                    }
                    else
                    {
                        Logger.Warn($"获取车型适配车品redis数据失败SelectVehicleAdaptChepinsAsync:{string.Concat("ChepinActivity/", vehicleId)};Error:{cacheResult.Message}", cacheResult.Exception);
                        var dbResult = await DalActivity.SelectVehicleAdaptChepinAsync(vehicleId);
                        vehicleAdaptChepins = dbResult.ToArray();
                    }
                }
                var pids = vehicleAdaptChepins.Select(r => r.PID).ToList();
                var sortedCategoryNames = await SelectVehicleSortedCategoryNamesAsync();
                if (!pids.Any() || sortedCategoryNames == null || !sortedCategoryNames.Any())
                {
                    return new Dictionary<string, IEnumerable<VehicleAdaptChepinDetailModel>>();
                }
                using (var client = new ProductClient())
                {
                    var models = await client.SelectSkuProductListByPidsAsync(pids);
                    if (models?.Result != null && models.Result.Any())
                    {
                        var skuDetailModels = models.Result.ToArray();
                        var detailModels = (from sc in sortedCategoryNames
                                            join v in vehicleAdaptChepins on sc equals v.CategoryName
                                            join p in skuDetailModels on v.PID equals p.Pid
                                            select new VehicleAdaptChepinDetailModel
                                            {
                                                PKID = v.PKID,
                                                PID = v.PID,
                                                CategoryName = sc,
                                                VehicleId = v.VehicleId,
                                                SalesOrder = v.SalesOrder,
                                                Image = p.Image.GetImageUrl(),
                                                SalePrice = p.Price,
                                                DisplayName = p.DisplayName
                                            }).GroupBy(r => r.CategoryName).ToDictionary(x => x.Key, y => y.AsEnumerable());
                        return detailModels;
                    }
                    else
                    {
                        return new Dictionary<string, IEnumerable<VehicleAdaptChepinDetailModel>>();
                    }
                }
            }
        }

        /// <summary>
        /// 获取优惠券信息
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<CarTagCouponConfigModel>> SelectCarTagCouponConfigsAsync()
        {
            using (var client = CacheHelper.CreateCacheClient())
            {
                IEnumerable<CarTagCouponConfigModel> result;
                var cacheResult = await client.GetOrSetAsync("CouponGuid", DalActivity.SelectCarTagCouponConfigsAsync, GlobalConstant.VehicleAdapterCacheExpiration);
                if (cacheResult.Success)
                {
                    result = cacheResult.Value;
                }
                else
                {
                    Logger.Warn($"获取优惠券redis数据失败SelectCarTagCouponConfigsAsync:{"CouponGuid"};Error:{cacheResult.Message}", cacheResult.Exception);
                    var dbResult = await DalActivity.SelectCarTagCouponConfigsAsync();
                    result = dbResult;
                }
                return result;
            }
        }


        /// <summary>
        /// 获取排序后的轮胎规格
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<VehicleSortedTireSizeModel>> SelectVehicleSortedTireSizesAsync(string vehicleId)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                IEnumerable<VehicleSortedTireSizeModel> result;
                var cacheResult = await client.GetOrSetAsync(string.Concat("SortedTire/", vehicleId), () => DalActivity.SelectVehicleSortedTireSizesAsync(vehicleId), GlobalConstant.VehicleAdapterCacheExpiration);
                if (cacheResult.Success)
                {
                    result = cacheResult.Value;
                }
                else
                {
                    Logger.Warn($"获取排序后的轮胎redis数据失败SelectVehicleSortedTireSizesAsync:{string.Concat("SortedTire/", vehicleId)};Error:{cacheResult.Message}", cacheResult.Exception);
                    var dbResult = await DalActivity.SelectVehicleSortedTireSizesAsync(vehicleId);
                    result = dbResult;
                }
                return result;
            }
        }


        /// <summary>
        /// 插入用户分享信息并返回guid
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="batchGuid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>

        public async static Task<int> GetGuidAndInsertUserShareInfoAsync(Guid shreId, string pid, Guid batchGuid, Guid userId)
        {
            return await DalActivity.InsertUserShareInfoAsyncAsync(shreId, pid, batchGuid, userId);
        }


        /// <summary>
        /// 根据Guid取出写入表中的数据
        /// </summary>
        /// <param name="shareId"></param>
        /// <returns></returns>
        public async static Task<ActivityUserShareInfoModel> GetActivityUserShareInfoAsync(Guid shareId)
        {
            return await DalActivity.GetActivityUserShareInfoAsync(shareId);
        }

        /// <summary>
        /// 根据配置表Guid跟用户id取出生成的新id，推荐有礼
        /// </summary>
        /// <param name="configGuid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async static Task<int> GetGuidAndInsertUserForShareAsync(Guid shareId, Guid configGuid, Guid userId)
        {
            return await DalActivity.GetGuidAndInsertUserForShareAsync(shareId, configGuid, userId);
        }

        /// <summary>
        /// 获取配置表的一条数据，分享赚钱功能
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async static Task<RecommendGetGiftConfigModel> FetchRecommendGetGiftConfigAsync(Guid? number, Guid? userId = null)
        {
            if (userId != null && userId != Guid.Empty)
            {
                var SendCodeForUserGroup = await DalActivity.GetSendCodeForUserGroup(userId ?? Guid.Empty);
                var forUserGroup = SendCodeForUserGroup as SendCodeForUserGroupModel[] ?? SendCodeForUserGroup.ToArray();
                GetStartAndEndTime(forUserGroup);//获取兑换码的起止时间
                foreach (var forUserGroupModels in forUserGroup.GroupBy(g => g.GroupId).OrderBy(g => g.Key))
                {
                    var tempArray = forUserGroupModels.ToArray();
                    if (tempArray.Any(w => (w.CodeEndTime > DateTime.Now && w.CodeStartTime < DateTime.Now) || (w.IsGet)))
                    {
                        var temp = await DalActivity.FetchRecommendGetGiftConfigAsync(tempArray.FirstOrDefault().GroupId);
                        if (temp != null)
                        {
                            temp.SendCodes = tempArray;
                            return temp;
                        }
                    }
                }
            }
            return await DalActivity.FetchRecommendGetGiftConfigAsync(number);
        }

        private static void GetStartAndEndTime(SendCodeForUserGroupModel[] forUserGroup)
        {
            var temp = DalActivity.GetStartAndEndTimes(forUserGroup.Select(s => s.SendCode).ToList());
            foreach (SendCodeForUserGroupModel item in forUserGroup)
            {
                var tempModel = temp?.FindLast(x => x.Item1 == item.SendCode);
                if (tempModel != null)
                {
                    item.CodeStartTime = tempModel.Item2;
                    item.CodeEndTime = tempModel.Item3;
                }
            }
        }

        #region wx迁移过来的服务

        /// <summary>
        /// 根据活动ID查询用户领取次数
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="luckyWheel"></param>
        /// <returns></returns>
        public async static Task<IEnumerable<PromotionPacketHistoryModel>> SelectPromotionPacketHistoryAsync(Guid userId, Guid luckyWheel)
        {
            return await DalActivity.SelectPromotionPacketHistoryAsync(userId, luckyWheel);
        }

        /// <summary>
        /// 查询礼包领取
        /// </summary>
        /// <returns></returns>
        public async static Task<DataTable> SelectPacketByUsersAsync()
        {
            return await DalActivity.SelectPacketByUsersAsync();
        }
        #endregion

        public static async Task<RegionActivityPageModel> GetRegionActivityPageUrlAsync(string city, string activityId)
        {
            //拿到默认的URL
            RegionActivityPageModel activityUrl = await DalActivity.GetActivityPageDefaultUrlAsync(city, activityId);
            if (activityUrl != null) //存在该活动
            {
                var result = await DalActivity.GetActivityPageUrlAsync(city, activityId);
                if (result != null)
                {
                    if (result.Code == 2) //活动正在举行
                    {
                        activityUrl.Url = result.Url;
                        activityUrl.Id = result.Id;
                    }
                    else //活动没有开始或者过期
                    {
                        activityUrl.Id = 0;
                        activityUrl.Url = string.Empty;
                    }
                }
            }
            else//不存在该活动
            {
                activityUrl = new RegionActivityPageModel();
            }
            return activityUrl;
        }

        #region 活动页
        #region 数据储备
        /// <summary>
        /// 活动页配置数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="channel"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        public static async Task<ActivePageListModel> ActivePageListModelAsCache(int id, string channel, string hashKey)
        {
            if (channel != "wap" && channel != "all" && !string.IsNullOrEmpty(channel))
                channel = "www";
            var sw = new Stopwatch();
            sw.Start();
            if (!string.IsNullOrEmpty(hashKey))
                id = await DalActivity.FetchActivePageListModelIdasync(hashKey);
            var activePageContents = DalActivity.SelectActivePageContents(id, channel);
            var tireSizeConfig = DalActivity.FetchActivePageTireSizeConfigModel(id);
            var activePageList = DalActivity.FetchActivePageListModelasync(id, hashKey);
            var homewithDetails = DalActivity.SelectActivePageHomeWithDetailModels(id);
            await Task.WhenAll(activePageContents, tireSizeConfig, activePageList, homewithDetails);
            var contentIds = activePageContents.Result.OrderBy(r => r.Group).ThenBy(r => r.OrderBy).Where(r => r.Type == -2).Select(r => r.Pkid).ToList();
            var menuLists = new List<ActivePageMenuModel>();
            if (contentIds.Any())
                menuLists = (await DalActivity.SelectActivePageMenus(contentIds)).ToList();
            var activityHomes = homewithDetails.Result.GroupBy(
                r =>
                     new
                     {
                         r.FkActiveId,
                         r.BigHomeName,
                         r.HidBigHomePic,
                         r.BigHomeUrl,
                         r.HidBigHomePicWww,
                         r.BigHomeUrlWww,
                         r.HidBigHomePicWxApp,
                         r.BigHomeUrlWxApp,
                         r.IsHome,
                         r.Sort
                     }).
                            OrderBy(g => g.Key.Sort).
                            Select(a => new ActivePageHomeModel
                            {
                                FkActiveId = a.Key.FkActiveId,
                                BigHomeName = a.Key.BigHomeName,
                                HidBigHomePic = a.Key.HidBigHomePic,
                                BigHomeUrl = a.Key.BigHomeUrl,
                                HidBigHomePicWww = a.Key.HidBigHomePicWww,
                                BigHomeUrlWww = a.Key.BigHomeUrlWww,
                                HidBigHomePicWxApp = a.Key.HidBigHomePicWxApp,
                                BigHomeUrlWxApp = a.Key.BigHomeUrlWxApp,
                                IsHome = a.Key.IsHome,
                                ActivePageHomeDeatilModels = a.All(r => r.DetailPkid == 0) ? null : a.Select(detail => new ActivePageHomeDeatilModel
                                {
                                    HomeName = detail.HomeName,
                                    HidBigFHomePic = detail.HidBigFHomePic,
                                    BigFHomeMobileUrl = detail.BigFHomeMobileUrl,
                                    BigFHomeWwwUrl = detail.BigFHomeWwwUrl,
                                    BigFHomeOrder = detail.BigFHomeOrder,
                                    BigFHomeWxAppUrl = detail.BigFHomeWxAppUrl,
                                    Pkid = detail.DetailPkid
                                })
                            });
            var result = activePageList.Result ?? new ActivePageListModel();
            result.ActivePageHomeModels = activityHomes;
            result.ActivePageContents = activePageContents.Result.OrderBy(r => r.Group).ThenBy(r => r.OrderBy).ToList();
            result.ActivePageTireSizeConfigModel = tireSizeConfig.Result ?? new ActivePageTireSizeConfigModel().TireSizeConfigTrim();
            result.AllMenuList = menuLists;
            sw.Stop();
            if (sw.ElapsedMilliseconds > 50)
                Logger.Info($"活动页获取配置数据耗时==>{sw.ElapsedMilliseconds}");
            return result;

        }

        private static ActivePageTireSizeConfigModel TireSizeConfigTrim(this ActivePageTireSizeConfigModel trimtireSizeConfig)
        {
            string RemoveSpace(string configProp) => configProp?.Trim().Replace("&nbsp;", "");
            trimtireSizeConfig.CarInfoColor = RemoveSpace(trimtireSizeConfig.CarInfoColor);
            trimtireSizeConfig.PromptFontSize = RemoveSpace(trimtireSizeConfig.PromptFontSize);
            trimtireSizeConfig.MarginColor = RemoveSpace(trimtireSizeConfig.MarginColor);
            trimtireSizeConfig.FillColor = RemoveSpace(trimtireSizeConfig.FillColor);
            trimtireSizeConfig.PromptColor = RemoveSpace(trimtireSizeConfig.PromptColor);
            trimtireSizeConfig.CarInfoFontSize = RemoveSpace(trimtireSizeConfig.CarInfoFontSize);
            trimtireSizeConfig.NoCarTypePrompt = RemoveSpace(trimtireSizeConfig.NoCarTypePrompt);
            trimtireSizeConfig.NoFormatPrompt = RemoveSpace(trimtireSizeConfig.NoFormatPrompt);
            trimtireSizeConfig.CarInfoColor = RemoveSpace(trimtireSizeConfig.CarInfoColor);
            return trimtireSizeConfig;
        }
        /// <summary>
        /// 获取大翻盘对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static LuckyWheelModel GetLuckWheel(string id)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityPageClientName))
            {
                var sw = new Stopwatch();
                sw.Start();
                var result = client.GetOrSet(GlobalConstant.LuckWheelPrefix + id, () => DalActivity.GetLuckyWheelWithDetail(id), TimeSpan.FromDays(30));
                if (result.Success)
                {
                    if (result.Value == null)
                    {
                        return new LuckyWheelModel { isStatus = 0 };
                    }
                    GetLucyItems(result.Value);
                    return result.Value;
                }
                var model = DalActivity.GetLuckyWheelWithDetail(id);
                if (model == null)
                {
                    return new LuckyWheelModel { isStatus = 0 };
                }
                model = GetLucyItems(model);
                sw.Stop();
                if (sw.ElapsedMilliseconds > 50)
                    Logger.Info($"活动页获取大翻盘数据耗时==>{sw.ElapsedMilliseconds}");
                return model;
            }

        }
        public static LuckyWheelModel GetLucyItems(LuckyWheelModel model)
        {
            model.LuckyItems = new List<LuckyItem>();
            foreach (var item in model.Items)
            {
                var coupons = item.CouponRuleID.Split(',');
                if (!string.IsNullOrEmpty(item.MaxCoupon))
                {
                    var counts = item.MaxCoupon.Split(',');
                    if (counts.Length != coupons.Length)
                    {
                        Logger.Error(item.FKLuckyWheelID + "大翻盘配置错误" + item.CouponRuleID);
                    }
                    for (var i = 0; i < counts.Length && i < coupons.Length; i++)
                    {
                        var lucky = new LuckyItem
                        {
                            UserRank = item.UserRank,
                            Coupon = Convert.ToInt32(coupons[i])
                        };
                        if (!string.IsNullOrEmpty(counts[i]))
                        {
                            lucky.Count = Convert.ToInt32(counts[i]);
                        }
                        model.LuckyItems.Add(lucky);
                    }
                }
                else
                {
                    for (var i = 0; i < coupons.Length; i++)
                    {
                        var luck = new LuckyItem { UserRank = item.UserRank };
                        if (!string.IsNullOrWhiteSpace(coupons[i]))
                        {
                            luck.Coupon = Convert.ToInt32(coupons[i]);
                            model.LuckyItems.Add(luck);
                        }
                    }
                }
            }
            return model;
        }

        private static async Task<BigBrandRewardListModel> GetBigBrandAsync(string hashKey)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var result = await TuhuMemoryCache.Instance.GetOrSetAsync(hashKey + "BigBrand",
                    () => BigBrandManager.GetBigBrand(hashKey), TimeSpan.FromSeconds(15));
                sw.Stop();
                if (sw.ElapsedMilliseconds > 50)
                    Logger.Info($"活动页调用新大翻盘接口数据耗时,hashkey{hashKey}==>{sw.ElapsedMilliseconds}");
                return result;

            }
            catch (Exception ex)
            {
                Logger.Error($"调用新大翻盘接口失败,异常信息,hashkey{hashKey}", ex);
                return new BigBrandRewardListModel();
            }
        }

        private static async Task<FlashSaleModel> GetFlashSaleModelByFloor(FlashSaleModel productPool, string rim, Guid activityId, int cityId)
        {
            try
            {
                var swf = new Stopwatch();
                swf.Start();
                var result = await FetchRegionTiresActivity(new FlashSaleTiresActivityRequest()
                {
                    ActivityId = activityId,
                    RegionId = cityId,
                    TireSize = rim
                });
                swf.Stop();
                Logger.Info($"调用FetchRegionTiresActivity接口总耗时=》{swf.ElapsedMilliseconds}");
                productPool.StartDateTime = result.StartTime;
                productPool.EndDateTime = result.EndTime;
                productPool.Products = new List<FlashSaleProductModel>();
                var listproducts = new List<FlashSaleProductModel>();
                foreach (var floorActivity in result.TiresFloorActivity)
                {
                    var single = floorActivity.OtherImgAndProductMap
                                     .FirstOrDefault(r => r.IsAdaptaionTireSize) ??
                                 new Models.ImgAndProductMap();
                    var listP = single.ProductDetails?.NoSloganProductInfo.Union(
                                    single.ProductDetails?.SloganProductInfo) ?? new List<Models.TireProductInfo>();
                    listproducts.AddRange(listP.Select(item => new FlashSaleProductModel
                    {
                        ActivityID = floorActivity.TiresActivityId,
                        PID = item.PID,
                        ProductName = item.ProductName,
                        ProductImg = item.ProductImg.GetImageUrl(),
                        Price = item.Price,
                        AdvertiseTitle = item.AdvertiseTitle,
                        TotalQuantity = item.TotalQuantity,
                        SaleOutQuantity = item.SaleOutQuantity,
                    }));
                    productPool.Products = listproducts;
                }
                return productPool;
            }
            catch (Exception e)
            {
                Logger.Error($"调用FetchRegionTiresActivity接口报错", e.InnerException);
                return productPool;
            }
        }
        private static FixedPriceActivityConfig GetFixedPriceActivityConfigByActivityIdAsync(Guid activityId)
        {
            try
            {
                using (var client = new BaoYangClient())
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    var result = client.GetFixedPriceActivityConfigByActivityId(activityId);
                    result.ThrowIfException();
                    if (result.Success)
                    {
                        sw.Stop();
                        if (sw.ElapsedMilliseconds > 50)
                            Logger.Info($"活动页调用保养接口数据耗时==>{sw.ElapsedMilliseconds},接口耗时=》{result.ElapsedMilliseconds}");
                        if (result.Result != null)
                        {
                            var productConfigs = new List<FixedPriceActivityProductConfig>();
                            var roundConfigs = new List<FixedPriceActivityRoundConfig>();
                            var shopConfigs = new List<FixedPriceActivityShopConfig>();
                            if (result.Result.ProductConfigs != null)
                                productConfigs.AddRange(
                                    result.Result.ProductConfigs.Select(r => new FixedPriceActivityProductConfig
                                    {
                                        CategoryName = r.CategoryName,
                                        PID = r.PID,
                                        Brand = r.Brand,
                                        IsIngore = r.IsIngore

                                    }));
                            if (result.Result.RoundConfigs != null)
                                roundConfigs.AddRange(
                                    result.Result.RoundConfigs.Select(r => new FixedPriceActivityRoundConfig
                                    {
                                        StartTime = r.StartTime,
                                        EndTime = r.EndTime,
                                        LimitedQuantity = r.LimitedQuantity

                                    }));
                            if (result.Result.ShopConfigs != null)
                                shopConfigs.AddRange(result.Result.ShopConfigs.Select(r => new FixedPriceActivityShopConfig
                                {
                                    ShopType = r.ShopType,
                                    ShopId = r.ShopId

                                }));

                            return new FixedPriceActivityConfig()
                            {
                                PKID = result.Result.PKID,
                                ActivityId = result.Result.ActivityId,
                                ActivityName = result.Result.ActivityName,
                                PackageTypes = result.Result.PackageTypes,
                                MaxSaleQuantity = result.Result.MaxSaleQuantity,
                                IsChargeInstallFee = result.Result.IsChargeInstallFee,
                                IsUsePromotion = result.Result.IsUsePromotion,
                                InstallOrPayType = result.Result.InstallOrPayType,
                                ItemQuantityPerUser = result.Result.ItemQuantityPerUser,
                                PromotionPrice = result.Result.PromotionPrice,
                                TipTextColor = result.Result.TipTextColor,
                                ButtonBackgroundColor = result.Result.ButtonBackgroundColor,
                                ButtonTextColor = result.Result.ButtonTextColor,
                                BackgroundImg = result.Result.BackgroundImg,
                                ProductConfigs = productConfigs,
                                RoundConfigs = roundConfigs,
                                ShopConfigs = shopConfigs

                            };
                        }
                        else
                        {
                            sw.Stop();
                            if (sw.ElapsedMilliseconds > 50)
                                Logger.Info($"活动页调用保养接口数据耗时==>{sw.ElapsedMilliseconds},接口耗时=》{result.ElapsedMilliseconds}");
                            return new FixedPriceActivityConfig();
                        }
                    }
                    else
                    {


                        Logger.Error($"调用保养定价接口失败{result.ErrorCode + result.ErrorMessage}");
                        return new FixedPriceActivityConfig();
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Error($"调用保养定价接口失败{ex}");
                return new FixedPriceActivityConfig();
            }
        }

        private static async Task<IEnumerable<SkuProductDetailModel>> SelectSkuProductDetailModels(List<string> pids)
        {
            var sw = new Stopwatch();
            sw.Start();
            try
            {
                using (var client = new ProductClient())
                {
                    var skuList = await client.SelectSkuProductListByPidsAsync(pids);
                    skuList.ThrowIfException();
                    sw.Stop();
                    if (sw.ElapsedMilliseconds > 50)
                        Logger.Info($"活动页调用产品接口数据耗时==>{sw.ElapsedMilliseconds},接口耗时=》{skuList.ElapsedMilliseconds}");
                    return skuList.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"活动页调用产品接口失败Exception{ex.Message}{ex.InnerException}");
                return new List<SkuProductDetailModel>();
            }


        }

        private static async Task<IEnumerable<Product.Models.New.FlashSaleProductDetailModel>> GetProductSaleActivityByPidsAsync(List<string> pids)
        {
            var sw = new Stopwatch();
            sw.Start();
            try
            {
                using (var client = new ProductClient())
                {
                    var skuList = await client.GetProductSaleActivityByPidsAsync(pids);
                    skuList.ThrowIfException();
                    sw.Stop();
                    if (sw.ElapsedMilliseconds > 50)
                        Logger.Info($"活动页调用获取全网活动价接口数据耗时==>{sw.ElapsedMilliseconds},接口耗时=》{skuList.ElapsedMilliseconds}");
                    return skuList.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"活动页调用获取全网活动价接口失败Exception{ex.Message}{ex.InnerException}");
                return new List<Product.Models.New.FlashSaleProductDetailModel>();
            }


        }
        ///// <summary>
        ///// 获取产品库信息
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="pids"></param>
        ///// <returns></returns>
        //private static async Task<IEnumerable<SkuProductDetailModel>> SelectSkuProductDetailModels(int id, List<string> pids)
        //{
        //    using (var client = CacheHelper.CreateCacheClient("ActivityProducts"))
        //    {
        //        var result = await client.GetOrSetAsync(id.ToString(), () => SelectSkuProductDetailModels(pids), TimeSpan.FromMinutes(3));
        //        if (result.Success)
        //            return result.Value;
        //        else
        //        {
        //            Logger.Warn($"查询产品接口redis缓存失败SelectSkuProductDetailModels:{id};Error:{result.Message}", result.Exception);
        //            return await SelectSkuProductDetailModels(pids);
        //        }
        //    }
        //}

        private static async Task<IEnumerable<CheckGiftModel>> SelectCheckGiftModels(List<string> pids, string orderChannel)
        {
            var sw = new Stopwatch();
            sw.Start();
            var matchGiftsProduct = from p in pids
                                    select new MatchGiftsProduct
                                    {
                                        Pid = p
                                    };
            var result = new List<CheckGiftModel>();
            try
            {
                using (var client = new GiftsClient())
                {
                    var gifts = await client.SelectProductDetailGiftResponseAsync(new MatchGiftsRequest
                    {
                        OrderChannel = orderChannel,
                        Products = matchGiftsProduct
                    });
                    if (gifts.Result != null)
                    {
                        result.AddRange(gifts.Result.Select(gift => new CheckGiftModel
                        {
                            Pid = gift.Key,
                            HasGift = !gift.Value.Any() ? 0 : 1
                        }));
                    }
                    sw.Stop();
                    if (sw.ElapsedMilliseconds > 50)
                        Logger.Info($"活动页调用赠品接口数据耗时==>{sw.ElapsedMilliseconds},接口耗时=》{gifts.ElapsedMilliseconds}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"活动页调用赠品接口失败Exception{ex.Message}{ex.InnerException}");
                return result;
            }
        }

        private static async Task<IEnumerable<ProductInstallServicesModel>> SelectProductInstallServicesModel(List<string> pids)
        {
            var sw = new Stopwatch();
            sw.Start();
            try
            {
                using (var client = new ProductClient())
                {
                    var installservice = await client.SelectProductInstallServicesAsync(pids);
                    installservice.ThrowIfException(true);
                    sw.Stop();
                    if (sw.ElapsedMilliseconds > 50)
                        Logger.Info($"活动页调用安装服务接口数据总耗时==>{sw.ElapsedMilliseconds},接口耗时=》{installservice.ElapsedMilliseconds}");
                    return installservice.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"活动页调用安装服务接口失败Exception{ex.Message}{ex.InnerException}");
                return new List<ProductInstallServicesModel>();
            }
        }

        private static async Task<List<VehicleBanner>> GetVechicleBannerAsync(string hashKey, string group, int col)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.GetOrSetAsync($"vehcleBanner2{hashKey}/{group}/{col}",
                    () => DalActivity.SelectVehicleBannerAsync(hashKey, group, col), TimeSpan.FromHours(1));
                if (result.Success)
                {
                    return result.Value;
                }
                else
                {
                    Logger.Error($"redis缓存失败GetVechicleBannerAsync");
                    return null;
                }
            }
        }

        private static async Task<List<string>> SearchProductByPidsAsync(SearchProductRequest request)
        {
            var result = new List<string>();
            try
            {
                var sw = new Stopwatch();
                sw.Start();

                using (var client = new ProductSearchClient())
                {
                    var pageModelResult = await client.SearchProductWithPidsAsync(request);
                    var data = pageModelResult.Result.Source.ToList();
                    result.AddRange(data);
                    if (pageModelResult.Result.Pager != null && pageModelResult.Result.Pager.TotalPage > 1)
                    {
                        for (int i = 2; i <= pageModelResult.Result.Pager.TotalPage; i++)
                        {
                            request.CurrentPage += 1;
                            var splitresult = await client.SearchProductWithPidsAsync(request);
                            if (splitresult.Result.Source != null && splitresult.Result.Source.Any())
                            {
                                result.AddRange(splitresult.Result.Source.ToList());
                            }
                            else
                            {
                                break;
                            }

                        }
                    }
                }

                sw.Stop();
                if (sw.ElapsedMilliseconds > 30)
                {
                    Logger.Info($"调用接口SearchProductByPidsAsync耗时{sw.ElapsedMilliseconds}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"调用接口SearchProductByPidsAsync报错", ex);
            }
            return result;
        }

        private static async Task<List<SkuProductDetailModel>> SearchProductWithBIDataAsync(
            SearchProductRequest referRequest)
        {
            var result = new List<SkuProductDetailModel>();
            try
            {
                var sw = new Stopwatch();
                sw.Start();

                using (var client = new ProductSearchClient())
                {
                    var pageModelResult = await client.SearchProductWithBIDataAsync(referRequest);
                    result = pageModelResult.Result.Source.ToList();
                }
                sw.Stop();
                if (sw.ElapsedMilliseconds > 30)
                {
                    Logger.Info($"调用接口SearchProductWithPidsAsync耗时{sw.ElapsedMilliseconds}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"调用接口SearchProductWithPidsAsync报错", ex);
            }
            return result;
        }

        #endregion
        #region 组装数据

        private static async Task<bool> CheckActivityTime(ActivePageListModel activePageList, ActivtyPageRequest request)
        {
            var flag = true;
            if (request.UserId != Guid.Empty)
            {
                var isExist = await GetActivityPageWhiteListByUserIdAsync(request.UserId);
                if (isExist)
                {
                    if (activePageList.EndDate < DateTime.Now)
                        flag = false;
                }
                else
                {
                    if (activePageList.StartDate > DateTime.Now || activePageList.EndDate < DateTime.Now)
                        flag = false;
                }
            }
            else
            {
                if (activePageList.StartDate > DateTime.Now || activePageList.EndDate < DateTime.Now)
                    flag = false;
            }
            return flag;
        }
        private static async Task<ActivePageListModel> ActivePageListModelAssembly(ActivePageListModel activePageList, ActivtyPageRequest request)
        {
            if (!await CheckActivityTime(activePageList, request))
            {
                return null;
            }
            var pageContents = activePageList.ActivePageContents;
            var allPids = pageContents.Select(r => r.Pid).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            if (allPids.Any())
            {
                var products = (await SelectSkuProductDetailModels(allPids)).ToList();
                //比较手动配置的产品跟推荐产品有重复的时候去重

                Guid[] activityGuid = pageContents.Where(o => o.ActivityId.HasValue && (o.Type != 19 || o.Type != 30)).Select(r => r.ActivityId.Value).Distinct().ToArray();
                var flashSaleDatas = await FlashSaleManager.GetFlashSaleListAsync(activityGuid);
                foreach (var pageContent in pageContents)
                {
                    FlashSaleModel sale = null;
                    if (pageContent.ActivityId.HasValue && flashSaleDatas != null && flashSaleDatas.Count > 0)
                        sale = flashSaleDatas.FirstOrDefault(o => o.ActivityID == pageContent.ActivityId);
                    if (!string.IsNullOrWhiteSpace(pageContent.Pid))
                        pageContent.ActivityPrice = Convert.ToDecimal(products?.FirstOrDefault(o => o.Pid == pageContent.Pid)?.Price);
                    var item = products?.FirstOrDefault(o => o.Pid == pageContent.Pid);
                    if (item != null)
                    {
                        pageContent.Onsale = item.Onsale;
                        pageContent.Brand = item.Brand;
                        pageContent.Pattern = item.Pattern;
                        pageContent.SpeedRating = item.SpeedRating;
                    }
                    FullEntity(pageContent, sale);
                }
<<<<<<< HEAD
=======

                var pids = allPids.Where(pid => !string.IsNullOrEmpty(pid) && !pid.StartsWith("TR-")).ToList();
                if (pids.Any())
                {

                    giftts = (await SelectCheckGiftModels(pids, request.OrderChannel)).ToList();
                    installs = (await SelectProductInstallServicesModel(pids)).GroupBy(r => r.ProductPID).Select(d => new ProductInstallServicesModel
                    {
                        ProductPID = d.Key,
                        ServicePrice = d.Select(r => r.ServicePrice).OrderByDescending(r => r ?? 0).FirstOrDefault()
                    }).ToList();
                }
                pageContents = (from a in pageContents
                                join b in installs on a.Pid equals b.ProductPID into t
                                from b in t.DefaultIfEmpty()
                                join c in giftts on a.Pid equals c.Pid into g
                                from c in g.DefaultIfEmpty()
                                select new ActivePageContentModel
                                {
                                    Pkid = a.Pkid,
                                    FkActiveId = a.FkActiveId,
                                    Group = a.Group,
                                    Type = a.Type,
                                    Channel = a.Channel,
                                    IsUploading = a.IsUploading,
                                    DisplayWay = a.DisplayWay,
                                    Pid = a.Pid,
                                    ProductName = a.ProductName,
                                    ActivityId = a.ActivityId,
                                    TireSize = a.TireSize,
                                    Cid = a.Cid,
                                    Image = a.Image,
                                    RowType = a.RowType,
                                    LinkUrl = a.LinkUrl,
                                    AppUrl = a.AppUrl,
                                    PcUrl = a.PcUrl,
                                    Description = a.Description,
                                    OrderBy = a.OrderBy,
                                    SaleOutQuantity = a.SaleOutQuantity,
                                    TotalQuantity = a.TotalQuantity,
                                    StartDateTime = a.StartDateTime,
                                    EndDateTime = a.EndDateTime,
                                    HasGift = c?.HasGift ?? 0,
                                    InstallService = b?.ServicePrice == null ? 0 : b?.ServicePrice == 0 ? 1 : 2,
                                    Price = a.Price,
                                    ActivityPrice = a.ActivityPrice,
                                    Onsale = a.Onsale,
                                    IsShow = a.IsShow,
                                    IsNewUserFirstOrder = a.IsNewUserFirstOrder,
                                    AdvertiseTitle = a.AdvertiseTitle,
                                    IsRecommended = a.IsRecommended,
                                    IsLogin = a.IsLogin,
                                    IsTireStandard = a.IsTireStandard,
                                    IsTireSize = a.IsTireSize,
                                    IsHiddenTtile = a.IsHiddenTtile,
                                    ByService = a.ByService,
                                    ByActivityId = a.ByActivityId,
                                    Vehicle = a.Vehicle,
                                    OthersType = a.OthersType,
                                    WXAPPUrl = a.WXAPPUrl,
                                    //AppletsDetailUrl = a.AppletsDetailUrl,
                                    //AppletsListUrl = a.AppletsListUrl,
                                    Brand = a.Brand,
                                    HashKey = a.HashKey,
                                    ColumnNumber = a.ColumnNumber,
                                    ProductType = a.ProductType,
                                    IsAdapter = a.IsAdapter,
                                    Tips = a.Tips,
                                    WxAppId = a.WxAppId,
                                    ProductGroupId = a.ProductGroupId,
                                    IsReplace = a.IsReplace,
                                    ActivityType = a.ActivityType,
                                    FileUrl = a.FileUrl,
                                    ActiveText = a.ActiveText,
                                    JsonContent = a.JsonContent,
                                    CountDownStyle = a.CountDownStyle,
                                    Pattern = a.Pattern,
                                    SpeedRating = a.SpeedRating,
                                    SystemActivityId = a.SystemActivityId,
                                    IsVehicle = a.IsVehicle,
                                    VehicleLevel = a.VehicleLevel,
                                    RowLimit = a.RowLimit,
                                }).ToList();
>>>>>>> cef10ecfe95a62dbe27086896d29d0abab5da3d4
            }
            var pidsAndpool = new List<string>();
            var result = await GetActivePageListWithContentsGroup(pageContents, activePageList, request);
            var configPids = result.ActivePageContents.Select(r => r.Pid).Where(p => !string.IsNullOrEmpty(p)).ToList();
            pidsAndpool.AddRange(configPids);
            foreach (var content in result.ActivePageGroupContents)
            {
                var first = content.Contents.First();//这里主要是获取商品池的pid
                if (first.Contents != null && first.Contents.Any())
                {
                    pidsAndpool.AddRange(first.Contents.Select(r => r.Pid).Where(p => !string.IsNullOrEmpty(p)).ToList());
                }
            }
            //var poolPids = result.ActivePageGroupContents.SelectMany(g => g.Contents?.Select(r=>r.Pid) ?? new List<string>()).Where(p => !string.IsNullOrEmpty(p)).ToList();
            //pidsAndpool.AddRange(poolPids);
            var contents = await SetRecommendProducts(result.ActivePageContents, request, pidsAndpool);
            // var listGroup = new List<ActivePageGroupContentModel>();
            var dic = new Dictionary<string, int>();
            foreach (var groupContent in result.ActivePageGroupContents)
            {
                if (groupContent.Contents.Select(r => r.IsReplace).First() == 1)
                {
                    if (groupContent.RowType == 4)
                    {
                        if (dic.ContainsKey(groupContent.Order))
                        {
                            groupContent.Contents = contents.Where(r => r.Group == groupContent.Order).Skip(2).Take(2);
                        }
                        else
                        {
                            groupContent.Contents = contents.Where(r => r.Group == groupContent.Order).Take(2);
                            dic.Add(groupContent.Order, 0);

                        }

                    }
                    else
                    {
                        groupContent.Contents = contents.Where(r => r.Group == groupContent.Order);
                    }

                }

            }
            #region 设置菜单显示的产品（即每个不同类型的菜单下面显示的产品）
            if (result.MenuType == 1)
            {
                var totalRowsRemove = GetMenuActivityRow(result);
                //移除List中在菜单区域里显示的行
                result.ActivePageGroupContents.RemoveAll(row => totalRowsRemove.Contains(row));
                // model.Lists = dictionary;
            }
            #endregion
            return result;
        }
        private static int GetActivityRowsType(ActivePageContentModel firstModel, string channel, int menuType = 0)
            => channel == "wap" ? GetActivityRowsTypeApp(firstModel, menuType) : GetActivityRowsTypeWebSite(firstModel, menuType);

        /// <returns>
        ///1：一行一列模板
        ///2：一行一列图片
        ///3：一行一列抢购模板
        ///4：一行两列或者三列模板
        ///6：一行两列或者三列抢购模板
        ///5：一行两列或者三列图片
        ///7：滚动菜单
        ///8：左右滑动
        ///9：秒杀
        ///10：大翻盘大翻盘
        /// </returns>
        private static int GetActivityRowsTypeApp(ActivePageContentModel firstModel, int menuType)
        {
            if (firstModel.RowType == 0)
            {
                if ((firstModel.Type == 0 || firstModel.Type == 1 || firstModel.Type == 10 || firstModel.Type == 23) && firstModel.IsUploading == 0)
                    return (int)AppRowType.Template_1R1C;

                if ((firstModel.Type == 6 || firstModel.Type == 7 || firstModel.Type == 11) && firstModel.IsUploading == 0)
                    return (int)AppRowType.FlashTemplate_1R1C;

                if (firstModel.Type == 9)
                    return (int)AppRowType.SecondKill;

                if (firstModel.Type == 13)
                    return (int)AppRowType.LuckyWheel;
                if (firstModel.Type == 16)
                    return (int)AppRowType.BaoyangPrice;
                if (firstModel.Type == 20)
                    return (int)AppRowType.NewLuckyWheel;
                if (firstModel.Type == 18)
                    return (int)AppRowType.Coupon;
                if (firstModel.Type == 17)
                    return (int)AppRowType.Video;
                if (firstModel.Type == 19)
                    return (int)AppRowType.ProductPool;
                if (firstModel.Type == 21)
                    return (int)AppRowType.QAlottery;
                if (firstModel.Type == 22)
                    return (int)AppRowType.Luckylottery;
                if (firstModel.Type == 24)
                    return (int)AppRowType.ActiveText;
                if (firstModel.Type == 25)
                    return (int)AppRowType.ScrollTextChain;
                if (firstModel.Type == 26)
                    return (int)AppRowType.SlipCoupon;
                if (firstModel.Type == 27)
                    return (int)AppRowType.CountDown;
                if (firstModel.Type == 28)
                    return (int)AppRowType.LotteryMachine;
                if (firstModel.Type == 29)
                    return (int)AppRowType.VehicleBanner;
                if (firstModel.Type == 30)
                    return (int)AppRowType.NewProductPool;
                if (firstModel.IsUploading == 1)
                    return (int)AppRowType.Image_1R1C;
            }

            if (firstModel.RowType == 1 || firstModel.RowType == 2)
            {
                if ((firstModel.Type == 0 || firstModel.Type == 1 || firstModel.Type == 10 || firstModel.Type == 23) && firstModel.IsUploading == 0)
                    return (int)AppRowType.Template_1R2C_1R3C;

                if ((firstModel.Type == 6 || firstModel.Type == 7 || firstModel.Type == 11) && firstModel.IsUploading == 0)
                    return (int)AppRowType.FlashTemplate_1R2C_1R3C;
                if (firstModel.Type == 9)
                    return (int)AppRowType.SecondKill;
                if (firstModel.IsUploading == 1)
                    return (int)AppRowType.Image_1R2C_1R3C;
            }

            if (firstModel.RowType == 4)
            {
                if ((firstModel.Type == 0 || firstModel.Type == 1 || firstModel.Type == 10 || firstModel.Type == 23) && firstModel.IsUploading == 0)
                    return (int)AppRowType.Template_1R2C_1R3C;

                if ((firstModel.Type == 6 || firstModel.Type == 7 || firstModel.Type == 11) && firstModel.IsUploading == 0)
                    return (int)AppRowType.FlashTemplate_1R2C_1R3C;
                if (firstModel.Type == 9)
                    return (int)AppRowType.SecondKill;
                if (firstModel.IsUploading == 1)
                    return (int)AppRowType.Image_1R2C_1R3C;
            }
            //app一图三产品
            if (firstModel.RowType == 8)
            {
                return (int)AppRowType.Image_1R2C_1R3C;
            }
            if (firstModel.Type == -2)
                return menuType == 0 ? (int)AppRowType.Menu : (int)AppRowType.SlideMenu;
            return 0;
        }
        /// 网站：Type:14 banner 1一行一列 2一行两列 3 一行三列 4一行四列 5一行5列 7导航菜单滚动 8左右切换菜单 9秒杀 13 一图三产品
        private static int GetActivityRowsTypeWebSite(ActivePageContentModel firstModel, int menuType)
        {
            if (firstModel.RowType == 0 && firstModel.Type == 9)
                return (int)WebSiteRowType.SecondKill;
            switch (firstModel.RowType)
            {
                case 0:
                    return (int)WebSiteRowType.OneRowOneColumn;
                case 1:
                    return (int)WebSiteRowType.OneRowTwoColumn;
                case 2:
                    return (int)WebSiteRowType.OneRowThreeColumn;
                case 3:
                    return (int)WebSiteRowType.Banner;
                case 4:
                    return (int)WebSiteRowType.OneRowFourColumn;
                case 6:
                    return (int)WebSiteRowType.OneRowFiveColumn;
                case 7:
                    return (int)WebSiteRowType.OneImageThreeProduct;
            }
            if (firstModel.Type == -2 && menuType != 1)
                return (int)WebSiteRowType.Menu;
            if (firstModel.Type == -2 && menuType == 1)
                return (int)WebSiteRowType.SildeMenu;
            return 0;
        }

        public static async Task<ActivePageListModel> GetActivePageListWithContentsGroup(IEnumerable<ActivePageContentModel> list, ActivePageListModel model, ActivtyPageRequest request)
        {
            var sw = new Stopwatch();
            sw.Start();
            var productTypelist = new List<int> { 0, 1, 6, 7, 10, 11 };
            if (list == null)
                return model;
            if (model != null)
            {
                model.ActivePageGroupContents = new List<ActivePageGroupContentModel>();
                model.SlideMenuList = new List<SlideMenuList>();
                model.MenuList = new List<ActivePageMenuModel>();

                var groupContents = list.GroupBy(o => new { o.Group, o.RowType })
                    .OrderBy(g => g.Key.Group)
                    .Select(items =>
                        {
                            return new ActivePageGroupContentModel
                            {
                                Contents = items.OrderBy(o => o.OrderBy),
                                Order = items.Key.Group,
                                RowType = items.Key.RowType,
                                OrigionType = items.Select(r => r.Type).FirstOrDefault(),
                                Type = GetActivityRowsType(items.First(), request.Channel, model.MenuType)
                            };
                        }
                    );
                foreach (var groupContent in groupContents)
                {
                    if (groupContent.RowType == 4 &&
                        (productTypelist.Contains(groupContent.OrigionType) &&
                         groupContent.Contents.Select(r => r.IsUploading).FirstOrDefault() == 0) &&
                        request.Channel == "wap")
                    //轮胎轮毂车品且为模板的时候一行四列 拆分成两个一行两列显示
                    {
                        model.ActivePageGroupContents.Add(new ActivePageGroupContentModel
                        {
                            Contents = groupContent.Contents.Take(2).OrderBy(o => o.OrderBy),
                            Order = groupContent.Order,
                            RowType = groupContent.RowType,
                            OrigionType = groupContent.OrigionType,
                            Type = groupContent.Type
                        });

                        model.ActivePageGroupContents.Add(new ActivePageGroupContentModel
                        {
                            Contents = groupContent.Contents.Skip(2).OrderBy(o => o.OrderBy),
                            Order = groupContent.Order,
                            RowType = groupContent.RowType,
                            OrigionType = groupContent.OrigionType,
                            Type = groupContent.Type
                        });
                    }
                    else
                    {
                        model.ActivePageGroupContents.Add(groupContent);
                    }

                    if (groupContent.OrigionType == (int)ActivityPageContentType.Navigation) //导航菜单
                    {

                        switch (model.MenuType)
                        {
                            case 0:
                                model.MenuList.AddRange(model.AllMenuList.Where(r =>
                                    r.FkActiveContentId ==
                                    groupContent.Contents?.Select(p => p.Pkid).FirstOrDefault()).OrderBy(s => s.Sort));
                                break;
                            case 1:
                                if (model.SlideMenuList == null)
                                    model.SlideMenuList = new List<SlideMenuList>();
                                model.SlideMenuList.Add(new SlideMenuList
                                {
                                    Group = groupContent.Order,
                                    SlideMenuContents = new List<ActivityMenuCell>(),
                                    MenuList =
                                        model.AllMenuList.Where(
                                                r =>
                                                    r.FkActiveContentId ==
                                                    (groupContent.Contents?.Select(p => p.Pkid).FirstOrDefault()))
                                            .OrderBy(s => s.Sort)
                                            .ToList()
                                });
                                break;
                        }
                    }

                    if (groupContent.OrigionType == (int)ActivityPageContentType.Luckwheel)
                    {
                        var activityId = groupContent.Contents.Select(r => r.ActivityId).FirstOrDefault().ToString();
                        if (string.IsNullOrEmpty(activityId))
                            model.LuckyWheelModel = null;
                        else
                        {
                            model.LuckyWheelModel = GetLuckWheel(activityId);
                            if (model.LuckyWheelModel.isStatus == 0)
                                model.LuckyWheelModel = null;
                        }
                    }
                    if (groupContent.OrigionType == (int)ActivityPageContentType.NewLuckyWheel)
                    {
                        var hashkey = groupContent.Contents.Select(r => r.HashKey).FirstOrDefault();
                        var bigBrand = await GetBigBrandAsync(hashkey);
                        model.BigBrandPageStyleModels = bigBrand?.ItemStyles;
                    }
                    if (groupContent.OrigionType == (int)ActivityPageContentType.BaoyangPrice)
                    {
                        var activityId = groupContent.Contents.Select(r => r.ActivityId).FirstOrDefault().ToString();
                        Guid aid;
                        model.FixedPriceActivityConfig = Guid.TryParse(activityId, out aid)
                            ? GetFixedPriceActivityConfigByActivityIdAsync(aid)
                            : null;
                    }
                    if (groupContent.OrigionType == (int)ActivityPageContentType.ProductPool)
                    {
                        //商品池功能限制，1，配置的商品类型都是一样的，2，选择一行一列进行配置，里面选择一行*列配置
                        var listContents = new List<ActivePageContentModel>();
                        var first = groupContent.Contents.FirstOrDefault();
                        var activityId = first?.ActivityId;
                        var activityIdType = first?.ActivityType;
                        var productPool = new FlashSaleModel();
                        if (activityId != null)
                        {
                            switch (activityIdType)
                            {
                                case (int)ActivityIdType.FlashSaleActivity:
                                    productPool =
                                        await FlashSaleManager.SelectFlashSaleDataByActivityIDAsync(activityId.Value);
                                    break;
                                case (int)ActivityIdType.FloorActivity when (!string.IsNullOrEmpty(request.CityId)):
                                    productPool = await GetFlashSaleModelByFloor(productPool, request.Rim,
                                        activityId.Value,
                                        Convert.ToInt32(request.CityId));
                                    break;
                            }
                            if (productPool.Products != null)
                            {
                                var tips = first.Tips?.Split(',');

                                var tags = new Dictionary<string, IEnumerable<string>>();
                                var installment = new List<TireInstallmentModel>();
                                var pidList = productPool.Products.Select(r => r.PID).ToList();
                                var productDetails = (await SelectSkuProductDetailModels(pidList)).ToList();
                                if (tips != null)
                                {
                                    try
                                    {
                                        var sw1 = new Stopwatch();
                                        sw1.Start();
                                        using (var client = new ProductClient())
                                        {

                                            var searchTags = new List<ProductTagTypeIn>();

                                            //if (tips.Contains("1"))
                                            //    searchTags.Add(ProductTagTypeIn.Adapter);
                                            if (tips.Contains("3"))
                                                searchTags.Add(ProductTagTypeIn.GiftProduct);
                                            if (tips.Contains("4"))
                                            {
                                                searchTags.Add(ProductTagTypeIn.InstallNoSupport);
                                                searchTags.Add(ProductTagTypeIn.InstallSupport);
                                                searchTags.Add(ProductTagTypeIn.InstallFree);
                                            }

                                            var productTags = await client.SelectProductsTagAsync(new SeachTagRequest()
                                            {
                                                PidList = pidList,
                                                SeachTags = searchTags,
                                                VehicleId = request.VehiclId,
                                                Tid = request.Tid != null ? Convert.ToInt32(request.Tid) : 0
                                            });
                                            productTags.ThrowIfException(true);
                                            tags = productTags.Result;
                                            sw1.Stop();
                                            if (sw1.ElapsedMilliseconds > 50)
                                                Logger.Info(
                                                    $"调用标签接口总耗时=》{sw1.ElapsedMilliseconds},接口耗时=》{productTags.ElapsedMilliseconds}");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error("调用标签接口报错，错误信息：", ex);

                                    }

                                    if (tips.Contains("2"))
                                    {
                                        try
                                        {
                                            using (var client2 = new ProductConfigClient())
                                            {
                                                var installments = client2.SelectTireInstallmentByPIDs(pidList);
                                                installments.ThrowIfException();
                                                installment = installments.Result.ToList();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Error("调用分期购接口报错，错误信息：", ex);

                                        }
                                    }
                                }
                                foreach (var item in productPool.Products)
                                {
                                    var product = productDetails.FirstOrDefault(r => r.Pid == item.PID);
                                    if (product != null)
                                    {
                                        var content = new ActivePageContentModel
                                        {
                                            DisplayWay = first.DisplayWay,
                                            ActivityId = item.ActivityID,
                                            Pid = item.PID,
                                            ProductName = item.ProductName,
                                            Type = ConvertType(first.ProductType),
                                            ProductType = first.ProductType,
                                            IsAdapter = first.IsAdapter,
                                            Image = item.ProductImg.GetImageUrl(),
                                            ActivityPrice = item.Price,
                                            Price = item.FalseOriginalPrice,
                                            AdvertiseTitle = item.AdvertiseTitle,
                                            TotalQuantity = item.TotalQuantity,
                                            SaleOutQuantity = item.SaleOutQuantity,
                                            StartDateTime = productPool.StartDateTime,
                                            EndDateTime = productPool.EndDateTime,
                                            TireSize = product.Size.ToString(),
                                            Onsale = product.Onsale,
                                            Pattern = product.Pattern,
                                            SpeedRating = product.SpeedRating,
                                            IsUsePcode = item?.IsUsePCode ?? false,
                                            Brand = product.Brand
                                        };

                                        #region 活动商品

                                        content.ConcatUrl(first.ProductType);

                                        #endregion

                                        #region 标签

                                        content.SetTag("GiftProduct", tags);
                                        content.SetTag("Adapter", tags);
                                        content.SetTag("InstallNoSupport", tags);
                                        content.SetTag("InstallSupport", tags);
                                        content.SetTag("InstallFree", tags);

                                        #endregion

                                        #region 分期

                                        if (installment != null && installment.Any(r => r.PID == content.Pid))
                                        {
                                            content.InstalIlmentTag = 1;
                                        }

                                        #endregion

                                        listContents.Add(content);
                                    }
                                    //去掉适配逻辑，改为前端自己处理，避免更换车型，重复请求活动页整个接口
                                    //if (first.IsAdapter == 1)
                                    //{
                                    //    listContents = listContents.Where(r => r.AdapterTag == 1).ToList();
                                    //}
                                    //只会在一行一列里配置，瞎配不支持
                                    groupContent.Contents.First().Contents = new List<ActivePageContentModel>();
                                    groupContent.Contents.First().Contents.AddRange(listContents);
                                    //groupContent.Contents.First().Contents = listContents;
                                }
                            }
                        }
                    }
                    if (groupContent.OrigionType == (int)ActivityPageContentType.QAlottery)
                    {
                        var hashkey = groupContent.Contents.Select(r => r.HashKey).FirstOrDefault();
                        var bigBrand = await GetBigBrandAsync(hashkey);
                        model.QAlotteryPageStyleModel = bigBrand.AnsQuesConfig;
                    }
                    if (groupContent.OrigionType == (int)ActivityPageContentType.ScrollTextChain)
                    {
                        try
                        {
                            var jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                            var jsonContent = groupContent.Contents.Select(r => r.JsonContent).FirstOrDefault();
                            groupContent.Contents.First().ScrollTextChains =
                                (JsonConvert.DeserializeObject<List<ScrollTextChainModel>>(jsonContent, jsonSetting))
                                .Where(r => !string.IsNullOrEmpty(r.H5Url)
                                || !string.IsNullOrEmpty(r.Icon)
                                || !string.IsNullOrEmpty(r.Test)
                                || !string.IsNullOrEmpty(r.WxUrl)).ToList();

                        }
                        catch (Exception e)
                        {
                            Logger.Info($"反序列化滚动文字链结构失败，请检查配置数据是否准确", e.InnerException);
                        }
                    }
                    if (groupContent.OrigionType == (int)ActivityPageContentType.SlipCoupon)
                    {
                        try
                        {
                            var jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                            var jsonContent = groupContent.Contents.Select(r => r.JsonContent).FirstOrDefault();
                            groupContent.Contents.First().SlipCoupons =
                                JsonConvert.DeserializeObject<List<SlipCouponModel>>(jsonContent, jsonSetting)
                                .Where(r => !string.IsNullOrEmpty(r.CouponId)
                                || !string.IsNullOrEmpty(r.CouponImage)).ToList();

                        }
                        catch (Exception e)
                        {
                            Logger.Info($"反序列化滑动优惠券结构失败，请检查配置数据是否准确", e.InnerException);
                        }
                    }
                    if (groupContent.OrigionType == (int)ActivityPageContentType.VehicleBanner)
                    {
                        try
                        {
                            var first = groupContent.Contents.FirstOrDefault();
                            if (first.IsVehicle)
                            {
                                var bannerlist = await GetVechicleBannerAsync(request.HashKey, first.Group, 1);
                                var banner =
                                    bannerlist.FirstOrDefault(r => r.VehicleId.ToLower().Equals(request.VehiclId.ToLower()));
                                if (banner != null)
                                    groupContent.Contents.First().Image = banner.ImageUrl;
                            }

                        }
                        catch (Exception e)
                        {
                            Logger.Error($"获取分车型头图失败", e.InnerException);
                        }
                    }
                    if (groupContent.OrigionType == (int)ActivityPageContentType.NewProductPool)
                    {
                        var first = groupContent.Contents.FirstOrDefault();
                        var categorys = first.ProductType == (int)ProductType.Default
                            ? new[] { nameof(ProductType.Tires), nameof(ProductType.hub), nameof(ProductType.AutoProduct) }
                            : first.ProductType == (int)ProductType.Tires
                                ? new[] { nameof(ProductType.Tires) }
                                : first.ProductType == (int)ProductType.hub ? new[] { nameof(ProductType.hub) } : new[] { nameof(ProductType.AutoProduct) };
                        var searchRequest = new SearchProductRequest()
                        {
                            VehicleId = first.VehicleLevel == 2 ? request.VehiclId : null,
                            Tid = first.VehicleLevel == 5 ? request.Tid : null,
                            CurrentPage = 1,
                            OrderType = 1,
                            PageSize = 1000,
                            JustAdpter = true,
                            Parameters = new Dictionary<string, IEnumerable<string>>()
                            {
                                ["CP_Brand"] = new[] { first.Brand },
                                ["CP_Tire_Rim"] = new[] { request.Rim },
                                ["CP_Tire_Width"] = new[] { request.Width },
                                ["CP_Tire_AspectRatio"] = new[] { request.AspectRatio },
                                ["OnSale"] = new[] { "1" },
                                ["stockout"] = new[] { "0" },
                                ["Category"] = categorys,
                                ["StockOutExceptCategory"] = new[] { "Tires" }
                            }
                        };
                        var adaptAllPidList = await SearchProductByPidsAsync(searchRequest);
                        var sortedPidsRequest = new SortedPidsRequest
                        {
                            DicActivityId = new KeyValuePair<string, ActivityIdType>(first.SystemActivityId,
                                ActivityIdType.AutoActivity),
                            AdaptPids = adaptAllPidList,
                            ProductType = ProductType.Default
                        };
                        var sortedList = new List<string>();
                        if (first.VehicleLevel != 0)
                        {
                            try
                            {
                                sortedList = (await GetOrSetActivityPageSortedPidsAsync(sortedPidsRequest)).Item2;
                            }
                            catch (Exception e)
                            {
                                Logger.Error("调用排序好的活动页适配产品失败", e.InnerException);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(first.SystemActivityId))
                                sortedList = (await FlashSaleManager.SelectFlashSaleDataByActivityIDAsync(new Guid(first.SystemActivityId))).Products?.Select(p => p.PID)?.ToList();
                        }
                        if (sortedList == null || !sortedList.Any())
                        {
                            Logger.Error($"新商品池获取排序好的商品列表为空活动{request.HashKey};组号{first.Group}");
                        }
                        else
                        {
                            var needReturnPids = first.RowLimit == 0
                                ? sortedList
                                : sortedList.Take(first.RowLimit * first.ColumnNumber).ToList();
                            var products = (await SelectSkuProductDetailModels(needReturnPids)).ToList();
                            var flashSale = new FlashSaleModel()
                            {
                                Products = new List<FlashSaleProductModel>()
                            };
                            if (first.ActivityType == (int)ActivityIdType.FlashSaleActivity)
                            {
                                flashSale =
                                    await FlashSaleManager.SelectFlashSaleDataByActivityIDAsync(first.ActivityId.Value);
                            }
                            var query = (from product in products
                                         join actvity in flashSale.Products on product.Pid equals actvity.PID into temp
                                         from actvity in temp.DefaultIfEmpty()
                                         select new
                                         {
                                             product,
                                             actvity
                                         });
                            var innerContents = new List<ActivePageContentModel>();
                            foreach (var item in query)
                            {
                                innerContents.Add(new ActivePageContentModel
                                {
                                    DisplayWay = first.DisplayWay,
                                    ActivityId = item.actvity?.ActivityID,
                                    Pid = item.product.Pid,
                                    ProductName = item.actvity == null ? item.product.DisplayName : item.actvity.ProductName,
                                    Type = ConvertType(first.ProductType, item.product.RootCategoryName),
                                    ProductType = first.ProductType,
                                    IsAdapter = first.IsAdapter,
                                    Image = item.product.Image.GetImageUrl(),
                                    Price = item.actvity?.FalseOriginalPrice ?? item.product.Price,
                                    ActivityPrice = item.actvity?.Price ?? item.product.Price,
                                    AdvertiseTitle = item.product.ShuXing5,
                                    TotalQuantity = item.actvity?.TotalQuantity,
                                    SaleOutQuantity = item.actvity?.SaleOutQuantity ?? 0,
                                    StartDateTime = flashSale?.StartDateTime ?? DateTime.MinValue,
                                    EndDateTime = flashSale?.EndDateTime ?? DateTime.MinValue,
                                    TireSize = item.product.Size.ToString(),
                                    Onsale = item.product.Onsale,
                                    Pattern = item.product.Pattern,
                                    SpeedRating = item.product.SpeedRating,
                                    IsUsePcode = item.actvity?.IsUsePCode ?? false,
                                    Brand = item.product.Brand,
                                });
                            }

                            var sortedContents = needReturnPids.Select(sortedPid => innerContents.FirstOrDefault(r => r.Pid == sortedPid)).ToList();
                            groupContent.Contents.First().Contents = new List<ActivePageContentModel>();
                            groupContent.Contents.First().Contents.AddRange(sortedContents);
                        }
                    }
                }
            }
            sw.Stop();
            if (sw.ElapsedMilliseconds > 50)
                Logger.Info($"活动页数据分组总共耗时==>{sw.ElapsedMilliseconds}");
            return model;
        }
        /// <summary>
        /// 设置标签
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="value"></param>
        /// <param name="tags"></param>
        private static void SetTag(this ActivePageContentModel detail, string value, Dictionary<string, IEnumerable<string>> tags)
        {
            //var type = detail.GetType();
            //var propertyInfo = type.GetProperty(value);
            //if(propertyInfo!=null)
            //propertyInfo.SetValue(detail, 1, null); 
            if (tags.ContainsKey(value) && tags[value] != null)
            {
                if (tags[value].Any(t => t == detail.Pid))
                {
                    switch (value)
                    {
                        case "GiftProduct":
                            detail.HasGift = 1;
                            break;
                        case "Adapter":
                            detail.AdapterTag = 1;
                            break;
                        case "InstallNoSupport":
                            detail.InstallService = 0;
                            break;
                        case "InstallSupport":
                            detail.InstallService = 2;
                            break;
                        case "InstallFree":
                            detail.InstallService = 1;
                            break;
                        default:
                            break;
                    }

                }
            }
        }
        private static void ConcatUrl(this ActivePageContentModel detail, int productType)
        {
            if (string.IsNullOrWhiteSpace(detail.Pid))
            {
                return;
            }

            if (detail.Pid.Split('|').Length == 2)
            {

                var productId = detail.Pid.Split('|')[0];
                var vid = detail.Pid.Split('|')[1];
                switch (productType)
                {
                    case 0:
                        detail.AppUrl = $"/tire/item?pid={productId}&vid={vid}&aid={detail.ActivityId}";
                        detail.LinkUrl = $"https://wx.tuhu.cn/Products/Tires?pid={productId}&vid={vid}&actid={detail.ActivityId}";
                        detail.PcUrl = $"https://item.tuhu.cn/Products/{productId}/{vid}.html?a={detail.ActivityId}";
                        break;
                    case 1:
                        detail.AppUrl = $"/accessory/item?pid={productId}&vid={vid}&aid={detail.ActivityId}";
                        detail.LinkUrl = $"https://wx.tuhu.cn/Products/Details?pid={productId}&vid={vid}&actid={detail.ActivityId}";
                        detail.PcUrl = $"https://item.tuhu.cn/Products/{productId}/{vid}.html?a={detail.ActivityId}";
                        break;
                    case 2:
                        detail.AppUrl = $"/wheelRim/item?pid={productId}&vid={vid}&aid={detail.ActivityId}";
                        detail.LinkUrl = $"https://wx.tuhu.cn/Products/Details?pid={productId}&vid={vid}&actid={detail.ActivityId}";
                        detail.PcUrl = $"https://item.tuhu.cn/Products/{productId}/{vid}.html?a={detail.ActivityId}";
                        break;
                }
            }
        }

        private static int ConvertType(int productType, string categoryName = null)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                switch (productType)
                {
                    case 0:
                        return (int)ActivityPageContentType.ATire;
                    case 1:
                        return (int)ActivityPageContentType.ACarProduct;
                    case 2:
                        return (int)ActivityPageContentType.ALunGu;
                    default:
                        return 0;
                }
            }
            else
            {
                switch (categoryName.ToLower())
                {
                    case "tires":
                        return (int)ActivityPageContentType.ATire;
                    case "autoproduct":
                        return (int)ActivityPageContentType.ACarProduct;
                    case "hub":
                        return (int)ActivityPageContentType.ALunGu;
                    default:
                        return 0;
                }
            }
        }
        /// <summary>s
        /// 获取菜单显示的行列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<ActivePageGroupContentModel> GetMenuActivityRow(ActivePageListModel model)
        {
            var totalRowsRemove = new List<ActivePageGroupContentModel>();
            if (model.SlideMenuList != null && model.SlideMenuList.Any())
            {
                foreach (var collection in model.SlideMenuList)
                {
                    foreach (var menu in collection.MenuList.OrderBy(o => o.Sort))
                    {
                        var cell = new ActivityMenuCell();
                        cell.ActivePageGroupContentModels = new List<ActivePageGroupContentModel>();

                        foreach (var row in model.ActivePageGroupContents)
                        {
                            if (string.Compare(row.Order, menu.MenuValue, StringComparison.Ordinal) >= 0 && string.Compare(row.Order, menu.MenuValueEnd, StringComparison.Ordinal) <= 0)
                            {
                                cell.ActivePageGroupContentModels.Add(row);
                                totalRowsRemove.Add(row);
                            }
                        }
                        cell.Group = menu.MenuValue;
                        collection.SlideMenuContents.Add(cell);
                    }
                }

            }
            return totalRowsRemove;
        }
        private static void FullEntity(ActivePageContentModel detail, FlashSaleModel saleModel)
        {
            var productTypelist = new List<int> { 0, 1, 6, 7, 10, 11 };
            #region 活动页内容
            detail.Image = string.IsNullOrEmpty(detail.Image) ? detail.Image : detail.Image.ToLower().EndsWith("gif") ? detail.Image : detail.Image.GetImageUrl();
            if (productTypelist.Contains(detail.Type) && !string.IsNullOrEmpty(detail.Pid))
            {
                #region 产品有活动GUID时，获取活动价格
                if (detail.Type == 0 || detail.Type == 6)
                {
                    detail.ConcatUrl(0);
                }
                if (detail.Type == 1 || detail.Type == 7)
                {
                    detail.ConcatUrl(1);
                }
                if (detail.Type == 10 || detail.Type == 11)
                {
                    detail.ConcatUrl(2);
                }
                if (detail.IsUploading == 0)
                {
                    if (detail.ActivityId.HasValue)
                    {
                        var pIdSaleModel = saleModel?.Products?.FirstOrDefault(o => o.PID == detail.Pid);

                        if (pIdSaleModel != null)
                        {
                            detail.Price = pIdSaleModel.FalseOriginalPrice;
                            detail.ActivityPrice = pIdSaleModel.Price;
                            detail.IsShow = pIdSaleModel.IsShow;
                            detail.IsNewUserFirstOrder = saleModel.IsNewUserFirstOrder;
                            detail.AdvertiseTitle = pIdSaleModel.AdvertiseTitle;
                            detail.IsUsePcode = pIdSaleModel.IsUsePCode;

                            #region 传给前段打标签用

                            detail.TotalQuantity = pIdSaleModel.TotalQuantity;
                            detail.SaleOutQuantity = pIdSaleModel.SaleOutQuantity;
                            detail.StartDateTime = saleModel.StartDateTime;
                            detail.EndDateTime = saleModel.EndDateTime;
                            #endregion
                        }
                        else
                        {
                            detail.ActivityId = null;
                        }
                        #endregion
                    }
                }
                #endregion
            }
        }

        private static async Task<List<string>> GetNoRepeatRecommendPids(List<string> pids, int currentPage, int pageSize, Guid userId)
        {
            var listPids = new List<string>();
            try
            {
                using (var client = new ProductSearchClient())
                {
                    var esResult = await client.GetRecommendEsCacheAsync(new RecommendEsSearchRequest()
                    {
                        UserId = userId,
                        PageSize = pageSize,
                        CurrentPage = currentPage

                    });
                    var esPids = esResult.Result.Source?.ToList() ?? new List<string>();
                    listPids = esPids.Except(pids).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error("调用获取bi推荐产品接口失败GetRecommendEsCacheAsync", e);
            }
            return listPids;
        }

        private static async Task<List<ActivePageContentModel>> SetRecommendProducts(
            List<ActivePageContentModel> pageContents, ActivtyPageRequest request, List<string> allPids)
        {

            var recommendContents = new List<ActivePageContentModel>();
            var recommendContentCps = pageContents.Where(r => r.IsReplace == 1 && (r.Type == (int)ActivityPageContentType.CarProduct || r.Type == (int)ActivityPageContentType.ACarProduct))?.ToList();
            if (recommendContentCps.Any())
            {
                recommendContents.AddRange(await SetCpRecommendProducts(recommendContentCps, request.UserId, allPids));
            }
            var recommendContentTires = pageContents.Where(r => r.IsReplace == 1 && (r.Type == (int)ActivityPageContentType.Tire || r.Type == (int)ActivityPageContentType.ATire))?.ToList();
            if (recommendContentTires.Any())
            {
                recommendContents.AddRange(await SetTireRecommendProducts(recommendContentTires, request));
            }
            return recommendContents.ToList();

        }

        private static async Task<List<ActivePageContentModel>> SetCpRecommendProducts(
            List<ActivePageContentModel> recommendContentCps, Guid userId, List<string> allPids)
        {
            var listNoRepeatePids = new List<string>();
            if (recommendContentCps.Any())
            {
                //var configContents = pageContents.Except(recommendContents).ToList();
                var count = recommendContentCps.Count();
                var counter = 0;
                while (listNoRepeatePids.Count < count && counter <= 10)
                {
                    counter++;
                    var noRepeatePids = await GetNoRepeatRecommendPids(allPids, counter, 50, userId);
                    listNoRepeatePids.AddRange(noRepeatePids);
                }
                var replacePids = listNoRepeatePids.Take(count).ToList();
                var replaceFlashSales = new List<Models.FlashSaleProductDetailModel>();
                //var flashsaleProducts = (await GetProductSaleActivityByPidsAsync(replacePids)).ToList();
                //replaceFlashSales.AddRange(flashsaleProducts);
                //var replaceProductPids = replacePids.Except(flashsaleProducts.Select(r => r.PID)).ToList();
                var detailProducts = (await SelectSkuProductDetailModels(replacePids)).ToList();
                replaceFlashSales.AddRange(detailProducts.Select(r => new Models.FlashSaleProductDetailModel
                {
                    PID = r.Pid,
                    DisplayName = r.DisplayName,
                    ImgUrl = r.Image,
                    Price = r.Price,
                    FalseOriginalPrice = r.Price,
                    OnSale = r.Onsale,
                    IsShow = r.IsShow,
                    AdvertiseTitle = r.ShuXing5

                }));
                if (count > replaceFlashSales.Count)
                {
                    var exceptPids = replacePids.Except(replaceFlashSales.Select(r => r.PID));
                    Logger.Error($"BI推荐的商品存在无效Pid{string.Join(",", exceptPids)}");
                    count = replaceFlashSales.Count;
                }
                for (var i = 0; i < count; i++)
                {
                    recommendContentCps[i].Pid = replaceFlashSales[i].PID;
                    recommendContentCps[i].ProductName = replaceFlashSales[i].DisplayName;
                    recommendContentCps[i].Price = replaceFlashSales[i].FalseOriginalPrice;
                    recommendContentCps[i].Onsale = replaceFlashSales[i].OnSale;
                    if (replaceFlashSales[i].ActivityID == Guid.Empty)
                        recommendContentCps[i].ActivityId = null;
                    else
                        recommendContentCps[i].ActivityId = replaceFlashSales[i].ActivityID;
                    recommendContentCps[i].Image = replaceFlashSales[i].ImgUrl.GetImageUrl();
                    recommendContentCps[i].TotalQuantity = replaceFlashSales[i].TotalQuantity;
                    recommendContentCps[i].SaleOutQuantity = replaceFlashSales[i].SaleOutQuantity;
                    recommendContentCps[i].StartDateTime = replaceFlashSales[i].StartDateTime;
                    recommendContentCps[i].EndDateTime = replaceFlashSales[i].EndDateTime;
                    recommendContentCps[i].ActivityPrice = replaceFlashSales[i].Price;
                    recommendContentCps[i].IsShow = replaceFlashSales[i].IsShow;
                    recommendContentCps[i].AdvertiseTitle = replaceFlashSales[i].AdvertiseTitle;
                    recommendContentCps[i].ConcatUrl(1);
                }
            }
            Logger.Info($"用户userid{userId}BI推荐产品是{string.Join(";", recommendContentCps.Select(r => r.Pid))}");
            return recommendContentCps.ToList();
        }

        private static async Task<List<ActivePageContentModel>> SetTireRecommendProducts(
            List<ActivePageContentModel> recommendContentTires, ActivtyPageRequest quest)
        {
            int? cityid = null;
            if (string.IsNullOrEmpty(quest.CityId))
            {
                cityid = null;
            }
            else
            {
                if (int.TryParse(quest.CityId, out int cid))
                {
                    cityid = cid;
                }
                else
                {
                    Logger.Error($"活动页传参城市id不合法{quest.CityId}");
                }
            }
            var count = recommendContentTires.Count;
            var result = new List<SkuProductDetailModel>();
            var referRequest = new SearchProductRequest()
            {
                CityId = cityid,
                VehicleId = quest.VehiclId,
                CurrentPage = 1,
                OrderType = 0,
                PageSize = count,
                Parameters = new Dictionary<string, IEnumerable<string>>()
                {
                    ["CP_Tire_Width"] = new string[] { quest.Width },
                    ["CP_Tire_AspectRatio"] = new string[] { quest.AspectRatio },
                    ["CP_Tire_Rim"] = new string[] { quest.Rim },
                    ["OnSale"] = new string[] { "1" },
                    ["stockout"] = new string[] { "0" },
                    ["Category"] = new string[] { "Tires" },
                    ["StockOutExceptCategory"] = new string[] { "Tires" }
                }
            };
            result = await SearchProductWithBIDataAsync(referRequest);
            for (var i = 0; i < result.Count; i++)
            {
                recommendContentTires[i].Pid = result[i].Pid;
                recommendContentTires[i].ProductName = result[i].DisplayName;
                recommendContentTires[i].Price = result[i].Price;
                recommendContentTires[i].ActivityPrice = result[i].Price;
                recommendContentTires[i].Onsale = result[i].Onsale;
                recommendContentTires[i].Image = result[i].Image.GetImageUrl();
                recommendContentTires[i].IsShow = result[i].IsShow;
                recommendContentTires[i].AdvertiseTitle = result[i].ShuXing5;
                recommendContentTires[i].Pattern = result[i].Pattern;
                recommendContentTires[i].SpeedRating = result[i].SpeedRating;
                recommendContentTires[i].ConcatUrl(0);
            }
            return recommendContentTires;
        }
        #endregion
        #region 返回结果
        public static async Task<ActivePageListModel> GetActivePageListModel(int id, ActivtyPageRequest request, string hashKey, bool isAssembly = true)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityPageClientName))
            {
                var cacheResult = await client.GetOrSetAsync(GlobalConstant.ActivityPagePrefix + id + request.Channel + hashKey,
                    () => ActivePageListModelAsCache(id, request.Channel, hashKey), GlobalConstant.ActivityPageCache);
                ActivePageListModel result;
                if (cacheResult.Success)
                {

                    if (!isAssembly)
                        result = cacheResult.Value;
                    else
                    {
                        result = await ActivePageListModelAssembly(cacheResult.Value, request);
                    }
                    return result;
                }
                else
                {
                    Logger.Warn($"查询活动页接口redis缓存失败GetActivePageListModel:{GlobalConstant.ActivityPagePrefix + id + request.Channel};Error:{cacheResult.Message}", cacheResult.Exception);
                    var retryCache = await RefreshActivePageListModelCache(id, request.Channel, hashKey);
                    var dbReslut = new ActivePageListModel();// await ActivePageListModelAsCache(id, channel, activityId);
                    if (retryCache)
                    {
                        var retryReslut =
                            (await client.GetAsync<ActivePageListModel>(GlobalConstant.ActivityPagePrefix + id + request.Channel + hashKey));
                        if (retryReslut.Success)
                            dbReslut = retryReslut.Value;
                    }
                    else
                    {
                        dbReslut = await TuhuMemoryCache.Instance.GetOrSetAsync("LocalCacheForactivityError", () => ActivePageListModelAsCache(id, request.Channel, hashKey), TimeSpan.FromSeconds(30));
                    }
                    if (!isAssembly)
                        result = dbReslut;
                    else
                    {
                        result = await ActivePageListModelAssembly(dbReslut, request);
                    }
                    return result;
                }
            }
        }
        #endregion
        #region 刷新缓存

        public static async Task<bool> RefreshActivePageListModelCache(int id, string channel, string hashKey)
        {
            try
            {
                using (var zklock = new ZooKeeperLock(id + channel + hashKey))
                {
                    if (await zklock.WaitAsync(3000))
                    {
                        var data = await ActivePageListModelAsCache(id, channel, hashKey);
                        using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityPageClientName))
                        {
                            var count = 0;
                            var result = await client.SetAsync(GlobalConstant.ActivityPagePrefix + id + channel + hashKey, data, GlobalConstant.ActivityPageCache);
                            while (!result.Success && count < 3)
                            {
                                Thread.Sleep(100);
                                count++;
                                result = await client.SetAsync(GlobalConstant.ActivityPagePrefix + id + channel + hashKey, data, GlobalConstant.ActivityPageCache);
                            }
                            if (!result.Success)
                                Logger.Warn($"刷新活动页redis缓存失败RefreshActivePageListModelCache:{GlobalConstant.ActivityPagePrefix + id + channel + hashKey};Error:{result.Message}", result.Exception);
                            return result.Success;
                        }
                    }
                    else
                    {
                        Logger.Error("刷新活动页redis缓存RefreshActivePageListModelCache等待链接超时");
                        return false;
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Error($"使用zookeeper异常{ex.InnerException + ex.Message}");
                return false;
            }
        }

        public static bool RefreshLuckWheelCache(string id)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityPageClientName))
            {
                var result = client.Set(GlobalConstant.LuckWheelPrefix + id, DalActivity.GetLuckyWheelWithDetail(id), GlobalConstant.ActivityPageCache);
                if (result.Success)
                    return result.Success;
                else
                {
                    Logger.Warn($"刷新活动页redis缓存失败RefreshLuckWheelCache:{GlobalConstant.LuckWheelPrefix + id };Error:{result.Message}", result.Exception);
                    return result.Success;
                }
            }

        }
        #endregion
        #endregion

        #region 分车型分地区活动页配置
        /// <summary>
        /// 根据活动Id，活动渠道,（活动地区或者活动车型）获取活动地址
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="regionId"></param>
        /// <param name="vehilceId"></param>
        /// <param name="activityChannel"></param>
        /// <returns></returns>
        public static async Task<Models.ResultModel<string>> GetRegionVehicleIdTargetUrl(Guid activityId, int regionId, string vehilceId, string activityChannel)
        {
            //根据活动Id获取活动信息
            Func<Task<RegionVehicleIdActivityConfig>> dbFunc = async () =>
            {
                var config = await DalActivity.SelectActivityType(activityId);
                if (config != null)
                {
                    config.UrlConfigs = await DalActivity.SelectRegionVehicleIdActivityConfigsByactivityId(activityId);
                }
                return config;
            };

            //从缓存中读取活动信息
            Func<Task<RegionVehicleIdActivityConfig>> cacheFunc = async () =>
            {
                using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityPageClientName))
                {
                    var cacheResult = await client.GetOrSetAsync(activityId.ToString(), dbFunc, TimeSpan.FromHours(4));
                    if (!cacheResult.Success)
                    {
                        Logger.Warn(cacheResult.Message);
                        return await dbFunc();
                    }
                    var value = cacheResult.Value;
                    return value;
                }
            };

            var funcResult = await cacheFunc();

            var result = new Models.ResultModel<string> { Code = 0, IsSuccess = true };

            if (string.IsNullOrWhiteSpace(funcResult?.ActivityType))
            {
                result.Code = 41;
                result.Msg = "活动不存在或者活动无效";
            }
            else if (funcResult.ActivityType != "Vehicle" && funcResult.ActivityType != "Region")
            {
                result.Code = 21;
                result.Msg = "未知活动类型";
            }
            else if (funcResult.StartTime > DateTime.Now)
            {
                result.Code = 11;
                result.Msg = "活动未开始";
            }
            else if (funcResult.EndTime < DateTime.Now)
            {
                result.Code = 12;
                result.Msg = "活动已结束";
            }
            else
            {
                var resultUrlConfig = null as RegionVehicleIdActivityUrlConfig;
                //取配置了该车型或该地区的活动页,未配置则取该渠道的默认页
                if (funcResult.ActivityType == "Vehicle")
                {
                    resultUrlConfig = funcResult.UrlConfigs?.FirstOrDefault(x => string.Equals(x.VehicleId, vehilceId) && !x.IsDefault)
                                      ?? funcResult.UrlConfigs?.FirstOrDefault(x => x.IsDefault);
                }
                else if (funcResult.ActivityType == "Region")
                {
                    resultUrlConfig = funcResult.UrlConfigs?.FirstOrDefault(x => x.RegionId == regionId && !x.IsDefault)
                                      ?? funcResult.UrlConfigs?.FirstOrDefault(x => x.IsDefault);
                }
                if (string.Equals(activityChannel, "kH5", StringComparison.OrdinalIgnoreCase))
                {
                    result.Result = resultUrlConfig?.TargetUrl;
                }
                else if (string.Equals(activityChannel, "WXAPP", StringComparison.OrdinalIgnoreCase))
                {
                    result.Result = resultUrlConfig?.WxappUrl;
                }
                if (string.IsNullOrWhiteSpace(result.Result))
                {
                    result.Code = 42;
                    result.Msg = "未配置活动Url";
                }
                else
                {
                    result.Code = 1;
                }
            }
            return result;
        }

        /// <summary>
        /// 刷新分车型分地区活动页缓存
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<bool> RefreshRegionVehicleIdTargetUrlCache(Guid activityId)
        {
            Func<Task<RegionVehicleIdActivityConfig>> dbFunc = async () =>
            {
                var config = await DalActivity.SelectActivityType(activityId);
                if (config != null)
                {
                    config.UrlConfigs = await DalActivity.SelectRegionVehicleIdActivityConfigsByactivityId(activityId);
                }
                return config;
            };

            Func<Task<bool>> cacheFunc = async () =>
            {
                var value = await dbFunc();
                using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityPageClientName))
                {
                    var cacheResult = await client.SetAsync(activityId.ToString(), value, TimeSpan.FromHours(4));
                    if (!cacheResult.Success)
                    {
                        Logger.Warn(cacheResult.Message);
                        return false;
                    }
                    return true;
                }
            };
            return await cacheFunc();
        }

        #endregion

        public static async Task<int> GetLuckyWheelUserlotteryCountAsync(Guid userid, Guid userGroup, string hashkey)
        {
            var result = await DalActivity.GetLuckyWheelUserlotteryCountAsync(userid, userGroup, hashkey);
            if (result == null)
                return -1;
            else
            {
                var count = result.Count;
                var record = result.Record;
                return Math.Max((count - record), 0);
            }
        }
        public static async Task<int> UpdateLuckyWheelUserlotteryCountAsync(Guid userid, Guid userGroup, string hashkey)
        {
            return await DalActivity.UpdateLuckyWheelUserlotteryCountAsync(userid, userGroup, hashkey);
        }

        #region 轮胎限购
        //TODO 改配置
        //private static readonly List<string> Blacklist = new List<string> { "13760797868", "13413656586", "13763380088", "18898318029", "13428126788", "15915906184 ", "13697419000", "13413656586", "13763054994", "18566377718", "13760797868", "15692410676", "13827158355", "18825148862", "13500006066", "13428126788", "13535232600", "13809731297", "13922249757", "13697419000", "18312101379", "13432861974", "13533440140", "15902051628", "15014422847", "17665443661", "13763380088", "18027111337" };
        /// <summary>
        /// 验证轮胎订单是否可以购买
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<VerificationTiresResponseModel>> VerificationByTiresAsync(VerificationTiresRequestModel requestModel)
        {

            if (string.IsNullOrWhiteSpace(requestModel.AddressPhone) && string.IsNullOrWhiteSpace(requestModel.UserPhone))
            {
                return OperationResult.FromError<VerificationTiresResponseModel>(ErrorCode.ParameterError, "手机号不能都为空");
            }

            var fraudApiInvokerResponse = new VerificationTiresResponseModel
            {
                Result = true,
                HitRules = new List<HitRulesModel>()
            };
            //验证限购黑名单
            BlackListItemRequest request = new BlackListItemRequest();
            List<BlackListItemModel> blackList = new List<BlackListItemModel>();

            if (!string.IsNullOrWhiteSpace(requestModel.AddressPhone))
                blackList.Add(new BlackListItemModel { BlackNumber = requestModel.AddressPhone, Type = 1 });
            if (!string.IsNullOrWhiteSpace(requestModel.UserPhone))
                blackList.Add(new BlackListItemModel { BlackNumber = requestModel.UserPhone, Type = 1 });
            if (!string.IsNullOrWhiteSpace(requestModel.DeviceId))
                blackList.Add(new BlackListItemModel { BlackNumber = requestModel.DeviceId, Type = 2 });

            request.Items = blackList;

            if (request.Items != null && request.Items.Any())
            {
                var result = await CheckBlackListItemAsync(request);
                if (result)
                {
                    fraudApiInvokerResponse.Result = false;
                    fraudApiInvokerResponse.ErrorMessage = "不允许下单";
                    Logger.Info($"VerificationByTiresAsync addressPhone{requestModel.AddressPhone} userPhone {requestModel.UserPhone} deviceid{requestModel.DeviceId} 返回结果：{JsonConvert.SerializeObject(fraudApiInvokerResponse)}");
                    return OperationResult.FromResult(fraudApiInvokerResponse);
                }
            }

            //if (Blacklist.Contains(requestModel.AddressPhone) || (!string.IsNullOrWhiteSpace(requestModel.UserPhone) && Blacklist.Contains(requestModel.UserPhone)))
            //{
            //    fraudApiInvokerResponse.Result = false;
            //    fraudApiInvokerResponse.ErrorMessage = "不允许下单";
            //    return OperationResult.FromResult(fraudApiInvokerResponse);
            //}
            if (requestModel.Number > 8)
            {
                fraudApiInvokerResponse.Result = false;
                fraudApiInvokerResponse.ErrorMessage = "购买数量超限制";
                Logger.Info($"VerificationByTiresAsync返回结果：{JsonConvert.SerializeObject(fraudApiInvokerResponse)}");
                return OperationResult.FromResult(fraudApiInvokerResponse);
            }
            //设备ID统一使用大写
            if (!string.IsNullOrWhiteSpace(requestModel.DeviceId))
                requestModel.DeviceId = requestModel.DeviceId.ToUpper();

            fraudApiInvokerResponse.HitRules.AddRange(await CheckOneDayTiresCount(requestModel));
            fraudApiInvokerResponse.HitRules.AddRange(await CheckOneMonthTiresCount(requestModel));

            //如果命中的规则大于0则不能下单
            if (fraudApiInvokerResponse.HitRules.Count > 0)
            {
                fraudApiInvokerResponse.Result = false;
            }
            Logger.Info($"VerificationByTiresAsync addressPhone{requestModel.AddressPhone} userPhone {requestModel.UserPhone} deviceid{requestModel.DeviceId} 返回结果：{JsonConvert.SerializeObject(fraudApiInvokerResponse)}");
            return OperationResult.FromResult(fraudApiInvokerResponse);
        }

        public static async Task<OperationResult<bool>> InsertTiresOrderRecordAsync(TiresOrderRecordRequestModel requestModel)
        {
            if (requestModel.OrderId <= 0)
                return OperationResult.FromError<bool>(ErrorCode.ParameterError, "订单号错误");

            if (string.IsNullOrWhiteSpace(requestModel.AddressPhone))
                return OperationResult.FromError<bool>(ErrorCode.ParameterError, "收货地址手机号不能为空");

            if (requestModel.Number <= 0)
                return OperationResult.FromError<bool>(ErrorCode.ParameterError, "数量必须大于0");

            //设备ID统一使用大写
            if (!string.IsNullOrWhiteSpace(requestModel.DeviceId))
                requestModel.DeviceId = requestModel.DeviceId.ToUpper();
            else
                requestModel.DeviceId = null;

            //插入数据库轮胎下单记录
            var result = await DalActivity.InserTiresOrderRecord(requestModel);
            if (result <= 0)
                Logger.Error("轮胎下单限制记录插入数据库失败");

            //插入Redis下单记录-----天记录
            await InsertOneDayTiresOrderRecor(requestModel);
            //插入Redis下单记录-----月记录
            await InsertOneMonthTiresOrderRecor(requestModel);
            return OperationResult.FromResult(true);
        }

        public static async Task<OperationResult<bool>> RevokeTiresOrderRecordAsync(int orderId)
        {
            //通过订单获得下单时的信息
            var recordModel = await DalActivity.SelectTiresOrderRecordByOrderId(orderId);
            if (recordModel == null)
                return OperationResult.FromResult(true);

            //取消的订单非本月订单无需处理
            if (recordModel.CreateTime.Date < DateTime.Now.AddDays(-30).Date)
                return OperationResult.FromResult(true);

            //如果不是当天订单无需撤销当天限购数量
            if (recordModel.CreateTime.Date == DateTime.Now.Date)
            {
                //撤销当天的限购数量
                await RevokeOneDayTiresNumber(recordModel);
            }
            await RevokeOneMonthTiresNumber(recordModel);
            try
            {
                var result = await DalActivity.UpdateTiresOrderRecordByPkid(recordModel.Pkid);
                if (result <= 0)
                    Logger.Error("轮胎下单限制撤销数据完成修改数据库失败");

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return OperationResult.FromResult(true);
        }



        public static async Task<OperationResult<bool>> RedisTiresOrderRecordReconStructionAsync(TiresOrderRecordRequestModel requestModel)
        {
            using (
                var client =
                    CacheHelper.CreateCounterClient(
                        $"TiresPurchaseQuantityStatistics_{DateTime.Now.Date:yyyy_MM_dd}", TimeSpan.FromDays(1)))
            {
                //设备ID统一使用大写
                if (!string.IsNullOrWhiteSpace(requestModel.DeviceId))
                    requestModel.DeviceId = requestModel.DeviceId.ToUpper();

                if (!string.IsNullOrWhiteSpace(requestModel.AddressPhone))
                {
                    if (requestModel.Number > 0)
                        await client.IncrementAsync(SecurityHelper.Hash(requestModel.AddressPhone), requestModel.Number);
                    else if (requestModel.Number < 0)
                        await client.DecrementAsync(SecurityHelper.Hash(requestModel.AddressPhone), -requestModel.Number);
                }

                if (!string.IsNullOrWhiteSpace(requestModel.UserPhone) && (!string.IsNullOrWhiteSpace(requestModel.AddressPhone) && !requestModel.AddressPhone.Equals(requestModel.UserPhone)))
                {
                    if (requestModel.Number > 0)
                        await client.IncrementAsync(SecurityHelper.Hash(requestModel.UserPhone), requestModel.Number);
                    else if (requestModel.Number < 0)
                        await client.DecrementAsync(SecurityHelper.Hash(requestModel.UserPhone), -requestModel.Number);
                }
                if (!string.IsNullOrWhiteSpace(requestModel.DeviceId))
                {
                    if (requestModel.Number > 0)
                        await client.IncrementAsync(SecurityHelper.Hash(requestModel.DeviceId), requestModel.Number);
                    else if (requestModel.Number < 0)
                        await client.DecrementAsync(SecurityHelper.Hash(requestModel.DeviceId), -requestModel.Number);
                }
            }
            return OperationResult.FromResult(true);
        }

        public static async Task<OperationResult<Dictionary<string, object>>> SelectTiresOrderRecordAsync(TiresOrderRecordRequestModel requestModel)
        {
            var dic = new Dictionary<string, object>();
            using (
                var client =
                    CacheHelper.CreateCounterClient(
                        $"TiresPurchaseQuantityStatistics_{DateTime.Now.Date:yyyy_MM_dd}", TimeSpan.FromDays(1)))
            {


                if (!string.IsNullOrWhiteSpace(requestModel.AddressPhone))
                {
                    long sqlCount = 0;
                    long redisCount = 0;
                    var sqlRecord = (await DalActivity.SelectTiresOrderRecordByPhone(requestModel.AddressPhone))?.ToList();
                    if (sqlRecord != null && sqlRecord.Any())
                    {
                        sqlCount = sqlRecord.Select(x => x.Number).Sum();
                    }
                    var redisRecord = await client.CountAsync(SecurityHelper.Hash(requestModel.AddressPhone));
                    if (redisRecord.Success)
                        redisCount = redisRecord.Value;
                    else
                        redisCount = -1;
                    dic["AddressPhonesqlCount"] = sqlCount;
                    dic["AddressPhoneredisCount"] = redisCount;
                }

                if (!string.IsNullOrWhiteSpace(requestModel.UserPhone))
                {
                    long sqlCount = 0;
                    long redisCount = 0;
                    var sqlRecord = (await DalActivity.SelectTiresOrderRecordByPhone(requestModel.UserPhone))?.ToList();
                    if (sqlRecord != null && sqlRecord.Any())
                    {
                        sqlCount = sqlRecord.Select(x => x.Number).Sum();
                    }
                    var redisRecord = await client.CountAsync(SecurityHelper.Hash(requestModel.UserPhone));
                    if (redisRecord.Success)
                        redisCount = redisRecord.Value;
                    else
                        redisCount = -1;
                    dic["UserPhonesqlCount"] = sqlCount;
                    dic["UserPhoneredisCount"] = redisCount;
                }
                if (!string.IsNullOrWhiteSpace(requestModel.DeviceId))
                {
                    requestModel.DeviceId = requestModel.DeviceId.ToUpper();
                    long sqlCount = 0;
                    long redisCount = 0;
                    var sqlRecord = (await DalActivity.SelectTiresOrderRecordByDeviceId(requestModel.DeviceId))?.ToList();
                    if (sqlRecord != null && sqlRecord.Any())
                    {
                        sqlCount = sqlRecord.Select(x => x.Number).Sum();
                    }
                    var redisRecord = await client.CountAsync(SecurityHelper.Hash(requestModel.DeviceId));
                    if (redisRecord.Success)
                        redisCount = redisRecord.Value;
                    else
                        redisCount = -1;
                    dic["DeviceIdsqlCount"] = sqlCount;
                    dic["DeviceIdredisCount"] = redisCount;
                }
            }
            return OperationResult.FromResult(dic);
        }
        #endregion

        public static async Task<ShareProductModel> SelectShareActivityProductById(string ProductId, string BatchGuid)
        {

            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ShareActivityClientName))
            {
                if (string.IsNullOrEmpty(BatchGuid))
                {
                    var result = await client.GetOrSetAsync(GlobalConstant.ShareActivityKey + "/" + ProductId,
                        async () =>
                        {
                            var batchguid = await DalActivity.SelectShareActivityBatchId();
                            return await DalActivity.SelectShareActivityProductById(ProductId, batchguid);
                        }, TimeSpan.FromHours(1));
                    if (result.Success)
                        return result.Value;
                    else
                    {
                        Logger.Warn($"获取redis缓存失败SelectShareActivityProductById:{GlobalConstant.ShareActivityKey + "/" + ProductId };Error:{result.Message}", result.Exception);
                        return await DalActivity.SelectShareActivityProductById(ProductId, null);
                    }
                }
                else
                {
                    var result = await client.GetOrSetAsync(GlobalConstant.ShareActivityKey + "/" + ProductId + "/" + BatchGuid, () => DalActivity.SelectShareActivityProductById(ProductId, BatchGuid), TimeSpan.FromHours(1));
                    if (result.Success)
                        return result.Value;
                    else
                    {
                        Logger.Warn($"获取redis缓存失败SelectShareActivityProductById:{GlobalConstant.ShareActivityKey + "/" + ProductId + "/" + BatchGuid };Error:{result.Message}", result.Exception);
                        return await DalActivity.SelectShareActivityProductById(ProductId, BatchGuid);
                    }
                }

            }

        }

        public static async Task<BaoYangActivitySetting> SelectBaoYangActivitySetting(string activityId)
        {
            return await DalActivity.SelectBaoYangActivitySetting(activityId);
        }

        public static async Task<CouponActivityConfigModel> SelectCouponActivityConfig(string activityNum, int type)
        {
            return await DalActivity.SelectCouponActivityConfig(activityNum, type);
        }

        public static async Task<ActivityBuild> GetActivityBuildWithSelKeyAsync(string keyword)
        {
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityBuildKey))
            {
                var allActivity = await client.GetOrSetAsync(
                    GlobalConstant.ActivityBuildKey,
                    () => DalActivity.SelectAllActivityBuildConfig(),
                    GlobalConstant.ActivityBuildSpan);
                if (!allActivity.Success)
                {
                    Logger.Warn($"获取redis缓存失败,GetActivityBuildWithSelKeyAsync");
                    return null;
                }
                if (allActivity.Value?.ToList() == null)
                {
                    Logger.Warn($"获取redis缓存为null,GetActivityBuildWithSelKeyAsync");
                    return null;
                }
                var activity = allActivity.Value.Where(p => p.SelKeyName.Trim() == keyword.Trim());
                if (!activity.Any())
                {
                    activity = allActivity.Value.Where(p => p.SelKeyNameList.Contains(keyword.Trim()));
                }
                return activity.OrderByDescending(p => p.UpdateTime).FirstOrDefault();
            }
        }

        public static IEnumerable<ActivityTypeModel> SelectActivityTypeByActivityIds(List<Guid> activityIds)
        {
            var activeTypes = new List<ActivityTypeModel>();
            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
            {
                foreach (var item in activityIds)
                {
                    ActivityTypeModel activeType;

                    var cacheResult = client.GetOrSet(item.ToString(), () => DalActivity.GetActivityTypeModel(item), TimeSpan.FromDays(20));

                    if (cacheResult.Success)
                    {
                        activeType = cacheResult.Value;
                    }
                    else
                    {
                        activeType = DalActivity.GetActivityTypeModel(item);
                    }
                    activeTypes.Add(activeType);
                }
            }
            return activeTypes;
        }


        public static async Task<bool> RecordActivityTypeLogAsync(ActivityTypeRequest request)
        {
            var result = await DalActivity.RecordActivityTypeLog(request);
            return result > 0;
        }

        #region 验证轮胎限购
        /// <summary>
        /// 验证轮胎周限购
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private static async Task<List<HitRulesModel>> CheckOneDayTiresCount(VerificationTiresRequestModel requestModel)
        {
            List<HitRulesModel> hits = new List<HitRulesModel>();
            using (var client = Nosql.CacheHelper.CreateCounterClient($"TiresPurchaseQuantityStatistics_{DateTime.Now.Date:yyyy_MM_dd}", TimeSpan.FromDays(1)))
            {
                long redisRecordCount = 0;
                //判断收货地址手机号码是否超限
                if (!string.IsNullOrWhiteSpace(requestModel.AddressPhone))
                {
                    var result = await client.CountAsync(SecurityHelper.Hash(requestModel.AddressPhone));
                    if (result.Success)
                    {
                        redisRecordCount = result.Value + requestModel.Number;
                    }
                    else
                    {
                        var sqlRecord = (await DalActivity.SelectTiresOrderRecordByPhone(requestModel.AddressPhone))?.ToList();
                        if (sqlRecord != null && sqlRecord.Any())
                        {
                            redisRecordCount = sqlRecord.Select(x => x.Number).Sum() + requestModel.Number;
                        }
                        else
                        {
                            redisRecordCount = 0;
                        }
                    }

                    if (redisRecordCount > 8)
                    {
                        hits.Add(new HitRulesModel()
                        {
                            Count = redisRecordCount,
                            HitName = nameof(requestModel.AddressPhone),
                            Description = "收货地址手机号码购买超天限制"
                        });
                    }
                }
                //判断用户登录手机手机号码是否超限
                if (!string.IsNullOrWhiteSpace(requestModel.UserPhone))
                {
                    var result = await client.CountAsync(SecurityHelper.Hash(requestModel.UserPhone));

                    if (result.Success)
                    {
                        redisRecordCount = result.Value + requestModel.Number;
                    }
                    else
                    {
                        var sqlRecord =
                            (await DalActivity.SelectTiresOrderRecordByPhone(requestModel.UserPhone))?.ToList();
                        if (sqlRecord != null && sqlRecord.Any())
                        {
                            redisRecordCount = sqlRecord.Select(x => x.Number).Sum() + requestModel.Number;
                        }
                        else
                        {
                            redisRecordCount = 0;
                        }
                    }
                    if (redisRecordCount > 8)
                    {
                        hits.Add(new HitRulesModel()
                        {
                            Count = redisRecordCount,
                            HitName = nameof(requestModel.UserPhone),
                            Description = "用户账户手机号码购买超天限制"
                        });
                    }
                }
                //判断设备ID是否超限
                if (!string.IsNullOrWhiteSpace(requestModel.DeviceId))
                {
                    var result = await client.CountAsync(SecurityHelper.Hash(requestModel.DeviceId));
                    if (result.Success)
                    {
                        redisRecordCount = result.Value + requestModel.Number;
                    }
                    else
                    {
                        var sqlRecord = (await DalActivity.SelectTiresOrderRecordByDeviceId(requestModel.DeviceId))?.ToList();
                        if (sqlRecord != null && sqlRecord.Any())
                        {
                            redisRecordCount = sqlRecord.Select(x => x.Number).Sum() + requestModel.Number;
                        }
                        else
                        {
                            redisRecordCount = 0;
                        }
                    }
                    if (redisRecordCount > 8)
                    {
                        hits.Add(new HitRulesModel()
                        {
                            Count = redisRecordCount,
                            HitName = nameof(requestModel.DeviceId),
                            Description = "设备ID购买超天限制"
                        });
                    }
                }
            }
            return hits;
        }
        /// <summary>
        /// 验证轮胎月限购
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private static async Task<List<HitRulesModel>> CheckOneMonthTiresCount(VerificationTiresRequestModel requestModel)
        {
            List<HitRulesModel> hits = new List<HitRulesModel>();
            using (var client = CacheHelper.CreateCounterClient($"TiresMonthPurchaseQuantityStatistics_{DateTime.Now.Date:yyyy_MM_dd}", TimeSpan.FromDays(1)))
            {
                long redisRecordCount = 0;
                //判断收货地址手机号码是否超限
                if (!string.IsNullOrWhiteSpace(requestModel.AddressPhone))
                {
                    var result = await client.CountAsync(SecurityHelper.Hash(requestModel.AddressPhone));
                    if (result.Success)
                    {
                        redisRecordCount = result.Value + requestModel.Number;
                    }
                    else
                    {
                        var sqlRecord = (await DalActivity.SelectTiresOrderRecordByPhone(requestModel.AddressPhone, DateTime.Parse(DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd"))))?.ToList();
                        if (sqlRecord != null && sqlRecord.Any())
                        {
                            redisRecordCount = sqlRecord.Select(x => x.Number).Sum() + requestModel.Number;
                        }
                        else
                        {
                            redisRecordCount = 0;
                        }
                    }
                    if (redisRecordCount > 12)
                    {
                        hits.Add(new HitRulesModel()
                        {
                            Count = redisRecordCount,
                            HitName = nameof(requestModel.AddressPhone),
                            Description = "收货地址手机号码购买超月限制"
                        });
                    }
                }
                //判断用户登录手机手机号码是否超限
                if (!string.IsNullOrWhiteSpace(requestModel.UserPhone))
                {
                    var result = await client.CountAsync(SecurityHelper.Hash(requestModel.UserPhone));

                    if (result.Success)
                    {
                        redisRecordCount = result.Value + requestModel.Number;
                    }
                    else
                    {
                        var sqlRecord =
                            (await DalActivity.SelectTiresOrderRecordByPhone(requestModel.UserPhone, DateTime.Parse(DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd"))))?.ToList();
                        if (sqlRecord != null && sqlRecord.Any())
                        {
                            redisRecordCount = sqlRecord.Select(x => x.Number).Sum() + requestModel.Number;
                        }
                        else
                        {
                            redisRecordCount = 0;
                        }
                    }
                    if (redisRecordCount > 12)
                    {
                        hits.Add(new HitRulesModel()
                        {
                            Count = redisRecordCount,
                            HitName = nameof(requestModel.UserPhone),
                            Description = "用户账户手机号码购买超月限制"
                        });
                    }
                }
                //判断设备ID是否超限
                if (!string.IsNullOrWhiteSpace(requestModel.DeviceId))
                {
                    var result = await client.CountAsync(SecurityHelper.Hash(requestModel.DeviceId));
                    if (result.Success)
                    {
                        redisRecordCount = result.Value + requestModel.Number;
                    }
                    else
                    {
                        var sqlRecord = (await DalActivity.SelectTiresOrderRecordByDeviceId(requestModel.DeviceId, DateTime.Parse(DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd"))))?.ToList();
                        if (sqlRecord != null && sqlRecord.Any())
                        {
                            redisRecordCount = sqlRecord.Select(x => x.Number).Sum() + requestModel.Number;
                        }
                        else
                        {
                            redisRecordCount = 0;
                        }
                    }
                    if (redisRecordCount > 12)
                    {
                        hits.Add(new HitRulesModel()
                        {
                            Count = redisRecordCount,
                            HitName = nameof(requestModel.DeviceId),
                            Description = "设备ID购买超月限制"
                        });
                    }
                }
            }
            return hits;
        }
        #endregion

        #region 增加轮胎购买记录
        /// <summary>
        /// 增加轮胎月限购数量
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private static async Task InsertOneMonthTiresOrderRecor(TiresOrderRecordRequestModel requestModel)
        {
            using (
                var client =
                    CacheHelper.CreateCounterClient(
                        $"TiresMonthPurchaseQuantityStatistics_{DateTime.Now.Date:yyyy_MM_dd}", TimeSpan.FromDays(1)))
            {
                int sqlCount = 0;
                if (!string.IsNullOrWhiteSpace(requestModel.AddressPhone))
                {
                    var addressPhoneCount = client.Count(SecurityHelper.Hash(requestModel.AddressPhone));
                    //如果获取成功代表key 已经存在直接增加数量
                    if (!addressPhoneCount.Success)
                    {
                        //如果获取失败 有可能存在数据丢失 需要去获取一下数据库数量 进行Redis数量重建
                        var result = await DalActivity.SelectTiresOrderRecordByPhone(requestModel.AddressPhone, DateTime.Parse(DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd")));
                        if (result != null && result.Any())
                        {
                            sqlCount = result.Sum(x => x.Number);
                        }
                    }
                    var increresult = await client.IncrementAsync(SecurityHelper.Hash(requestModel.AddressPhone),
                        requestModel.Number + sqlCount);
                    if (!increresult.Success)
                        Logger.Error(
                            $"Redis 轮胎下单收货地址手机{requestModel.AddressPhone} 月限购数量增加失败订单号：{requestModel.OrderId} 数量：{requestModel.Number} 错误信息{increresult.Message}");
                    else if (increresult.Success && increresult.Value > 12)
                        Logger.Warn(
                            $"Redis 轮胎下单月限制出现已超 超出数量的 收货地址手机{requestModel.AddressPhone}下单成功 订单号：{requestModel.OrderId} 数量：{requestModel.Number}");
                }

                if (requestModel.UserPhone != null && !requestModel.AddressPhone.Equals(requestModel.UserPhone))
                {
                    var userPhoneCount = client.Count(SecurityHelper.Hash(requestModel.UserPhone));
                    //如果获取成功代表key 已经存在直接增加数量
                    if (!userPhoneCount.Success)
                    {
                        //如果获取失败 有可能存在数据丢失 需要去获取一下数据库数量 进行Redis数量重建
                        var result = await DalActivity.SelectTiresOrderRecordByPhone(requestModel.UserPhone, DateTime.Parse(DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd")));
                        if (result != null && result.Any())
                        {
                            sqlCount = result.Sum(x => x.Number);
                        }
                    }
                    var increresult = await client.IncrementAsync(SecurityHelper.Hash(requestModel.UserPhone),
                        requestModel.Number + sqlCount);
                    if (!increresult.Success)
                        Logger.Error(
                            $"Redis 轮胎下单用户手机{requestModel.UserPhone} 月限购数量增加失败订单号：{requestModel.OrderId} 数量：{requestModel.Number} 错误信息{increresult.Message}");
                    else if (increresult.Success && increresult.Value > 12)
                        Logger.Warn(
                            $"Redis 轮胎下单月限制出现已超 超出数量的 用户手机{requestModel.UserPhone}下单成功 订单号：{requestModel.OrderId} 数量：{requestModel.Number}");
                }
                if (!string.IsNullOrWhiteSpace(requestModel.DeviceId))
                {
                    var deviceIdCount = client.Count(SecurityHelper.Hash(requestModel.UserPhone));
                    //如果获取成功代表key 已经存在直接增加数量
                    if (!deviceIdCount.Success)
                    {
                        //如果获取失败 有可能存在数据丢失 需要去获取一下数据库数量 进行Redis数量重建
                        var result = await DalActivity.SelectTiresOrderRecordByDeviceId(requestModel.UserPhone, DateTime.Parse(DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd")));
                        if (result != null && result.Any())
                        {
                            sqlCount = result.Sum(x => x.Number);
                        }
                    }
                    var increresult = await client.IncrementAsync(SecurityHelper.Hash(requestModel.DeviceId),
                        requestModel.Number + sqlCount);
                    if (!increresult.Success)
                        Logger.Error(
                            $"Redis 轮胎下单设备ID{requestModel.DeviceId} 月限购数量增加失败订单号：{requestModel.OrderId} 数量：{requestModel.Number} 错误信息{increresult.Message}");
                    else if (increresult.Success && increresult.Value > 12)
                        Logger.Warn(
                            $"Redis 轮胎下单月限制出现已超 超出数量的 设备ID{requestModel.DeviceId} 下单成功 订单号：{requestModel.OrderId} 数量：{requestModel.Number}");
                }
            }
        }
        /// <summary>
        /// 增加轮胎周限购数量
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private static async Task InsertOneDayTiresOrderRecor(TiresOrderRecordRequestModel requestModel)
        {
            using (
                var client =
                    CacheHelper.CreateCounterClient(
                        $"TiresPurchaseQuantityStatistics_{DateTime.Now.Date:yyyy_MM_dd}", TimeSpan.FromDays(1)))
            {
                var increresult = await client.IncrementAsync(SecurityHelper.Hash(requestModel.AddressPhone),
                    requestModel.Number);
                if (!increresult.Success)
                    Logger.Error(
                        $"Redis 轮胎下单收货地址手机{requestModel.AddressPhone} 限购数量增加失败订单号：{requestModel.OrderId} 数量：{requestModel.Number} 错误信息{increresult.Message}");
                else if (increresult.Success && increresult.Value > 8)
                    Logger.Warn(
                        $"Redis 轮胎下单限制出现已超 超出数量的 收货地址手机{requestModel.AddressPhone}下单成功 订单号：{requestModel.OrderId} 数量：{requestModel.Number}");

                if (requestModel.UserPhone != null && !requestModel.AddressPhone.Equals(requestModel.UserPhone))
                {
                    increresult = await client.IncrementAsync(SecurityHelper.Hash(requestModel.UserPhone),
                        requestModel.Number);
                    if (!increresult.Success)
                        Logger.Error(
                            $"Redis 轮胎下单用户手机{requestModel.UserPhone} 限购数量增加失败订单号：{requestModel.OrderId} 数量：{requestModel.Number} 错误信息{increresult.Message}");
                    else if (increresult.Success && increresult.Value > 8)
                        Logger.Warn(
                            $"Redis 轮胎下单限制出现已超 超出数量的 用户手机{requestModel.UserPhone}下单成功 订单号：{requestModel.OrderId} 数量：{requestModel.Number}");
                }
                if (!string.IsNullOrWhiteSpace(requestModel.DeviceId))
                {
                    increresult = await client.IncrementAsync(SecurityHelper.Hash(requestModel.DeviceId),
                        requestModel.Number);
                    if (!increresult.Success)
                        Logger.Error(
                            $"Redis 轮胎下单设备ID{requestModel.DeviceId} 限购数量增加失败订单号：{requestModel.OrderId} 数量：{requestModel.Number} 错误信息{increresult.Message}");
                    else if (increresult.Success && increresult.Value > 8)
                        Logger.Warn(
                            $"Redis 轮胎下单限制出现已超 超出数量的 设备ID{requestModel.DeviceId} 下单成功 订单号：{requestModel.OrderId} 数量：{requestModel.Number}");
                }
            }
        }
        #endregion

        #region 撤销轮胎购买记录
        private static async Task RevokeOneDayTiresNumber(TiresOrderRecordModel recordModel)
        {
            using (
                var client =
                    CacheHelper.CreateCounterClient(
                        $"TiresPurchaseQuantityStatistics_{DateTime.Now.Date:yyyy_MM_dd}", TimeSpan.FromDays(1)))
            {
                //如果收货人手机号码不为空则撤销相应的数量
                if (!string.IsNullOrWhiteSpace(recordModel.AddressPhone))
                {
                    var decrementResult = await client.DecrementAsync(SecurityHelper.Hash(recordModel.AddressPhone), recordModel.Number);
                    if (!decrementResult.Success)
                        Logger.Error(
                            $"Redis 轮胎下单收货人手机号码{recordModel.AddressPhone} 限购数量撤销失败订单号：{recordModel.OrderId} 数量：{recordModel.Number} 错误信息{decrementResult.Message}");
                }
                //如果用户手机号码不为空并且与收货人手机不一致则撤销相应的数量
                if (!string.IsNullOrWhiteSpace(recordModel.UserPhone) &&
                    (!string.IsNullOrWhiteSpace(recordModel.AddressPhone) &&
                     !recordModel.AddressPhone.Equals(recordModel.UserPhone)))
                {
                    var decrementResult = await client.DecrementAsync(SecurityHelper.Hash(recordModel.UserPhone),
                        recordModel.Number);
                    if (!decrementResult.Success)
                        Logger.Error(
                            $"Redis 轮胎下单用户手机号码{recordModel.UserPhone} 限购数量撤销失败订单号：{recordModel.OrderId} 数量：{recordModel.Number} 错误信息{decrementResult.Message}");
                }
                //如果收货人手机号码不为空则撤销相应的数量
                if (!string.IsNullOrWhiteSpace(recordModel.DeviceId))
                {
                    recordModel.DeviceId = recordModel.DeviceId.ToUpper();
                    var decrementResult = await client.DecrementAsync(SecurityHelper.Hash(recordModel.DeviceId), recordModel.Number);
                    if (!decrementResult.Success)
                        Logger.Error(
                            $"Redis 轮胎下单设备ID{recordModel.DeviceId} 限购数量撤销失败订单号：{recordModel.OrderId} 数量：{recordModel.Number} 错误信息{decrementResult.Message}");
                }
            }
        }
        private static async Task RevokeOneMonthTiresNumber(TiresOrderRecordModel recordModel)
        {
            using (
                var client =
                    CacheHelper.CreateCounterClient(
                        $"TiresMonthPurchaseQuantityStatistics_{DateTime.Now.Date:yyyy_MM_dd}", TimeSpan.FromDays(1)))
            {
                int? sqlCount = 0;
                //如果收货人手机号码不为空则撤销相应的数量
                if (!string.IsNullOrWhiteSpace(recordModel.AddressPhone))
                {
                    var addressPhoneCount = await client.CountAsync(SecurityHelper.Hash(recordModel.AddressPhone));
                    if (!addressPhoneCount.Success)
                    {
                        // 如果获取失败 有可能存在数据丢失 需要去获取一下数据库数量 进行Redis数量重建
                        var result = await DalActivity.SelectTiresOrderRecordByPhone(recordModel.AddressPhone, DateTime.Parse(DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd")));
                        if (result != null && result.Any())
                        {
                            var oldSqlCount = result?.Sum(x => x.Number);
                            sqlCount = result.Where(x => x.Pkid != recordModel.Pkid)?.Sum(x => x.Number);
                            if (sqlCount.HasValue && sqlCount >= 0)
                            {
                                var IncrementResult = await client.IncrementAsync(SecurityHelper.Hash(recordModel.AddressPhone), sqlCount.Value);
                                if (!IncrementResult.Success)
                                    Logger.Error(
                                        $"Redis 轮胎下单收货人手机号码{recordModel.AddressPhone} 限购数量撤销失败订单号：{recordModel.OrderId} 数量：{recordModel.Number} 错误信息{IncrementResult.Message}");
                            }
                            else if (oldSqlCount != recordModel.Number)
                                Logger.Info($"撤销轮胎购买数量出现sql记录小于需要撤销记录  轮胎下单收货人手机号码{recordModel.AddressPhone} 限购数量撤销失败订单号：{recordModel.OrderId} 数量：{recordModel.Number} sql数量:{sqlCount}");
                        }
                    }
                    else
                    {
                        var decrementResult = await client.DecrementAsync(SecurityHelper.Hash(recordModel.AddressPhone), recordModel.Number);
                        if (!decrementResult.Success)
                            Logger.Error(
                                $"Redis 轮胎下单收货人手机号码{recordModel.AddressPhone} 限购数量撤销失败订单号：{recordModel.OrderId} 数量：{recordModel.Number} 错误信息{decrementResult.Message}");
                    }
                }
                //如果用户手机号码不为空并且与收货人手机不一致则撤销相应的数量
                if (!string.IsNullOrWhiteSpace(recordModel.UserPhone) &&
                    (!string.IsNullOrWhiteSpace(recordModel.AddressPhone) &&
                     !recordModel.AddressPhone.Equals(recordModel.UserPhone)))
                {
                    var userPhoneCount = await client.CountAsync(SecurityHelper.Hash(recordModel.UserPhone));
                    if (!userPhoneCount.Success)
                    {
                        // 如果获取失败 有可能存在数据丢失 需要去获取一下数据库数量 进行Redis数量重建
                        var result = await DalActivity.SelectTiresOrderRecordByPhone(recordModel.UserPhone, DateTime.Parse(DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd")));
                        if (result != null && result.Any())
                        {
                            var oldSqlCount = result?.Sum(x => x.Number);
                            sqlCount = result.Where(x => x.Pkid != recordModel.Pkid)?.Sum(x => x.Number);
                            if (sqlCount.HasValue && sqlCount >= 0)
                            {
                                var IncrementResult = await client.IncrementAsync(SecurityHelper.Hash(recordModel.UserPhone), sqlCount.Value);
                                if (!IncrementResult.Success)
                                    Logger.Error(
                                        $"Redis 轮胎下单用户手机号码{recordModel.UserPhone} 限购数量撤销失败订单号：{recordModel.OrderId} 数量：{recordModel.Number} 错误信息{IncrementResult.Message}");
                            }
                            else if (oldSqlCount != recordModel.Number)
                                Logger.Info($"撤销轮胎购买数量出现sql记录小于需要撤销记录  轮胎下单单用户手机号码{recordModel.UserPhone} 限购数量撤销失败订单号：{recordModel.OrderId} 数量：{recordModel.Number} sql数量:{sqlCount}");
                        }
                    }
                    else
                    {
                        var decrementResult = await client.DecrementAsync(SecurityHelper.Hash(recordModel.UserPhone), recordModel.Number);
                        if (!decrementResult.Success)
                            Logger.Error(
                                $"Redis 轮胎下单用户手机号码{recordModel.UserPhone} 限购数量撤销失败订单号：{recordModel.OrderId} 数量：{recordModel.Number} 错误信息{decrementResult.Message}");
                    }
                }
                //如果设备ID不为空则撤销相应的数量
                if (!string.IsNullOrWhiteSpace(recordModel.DeviceId))
                {
                    recordModel.DeviceId = recordModel.DeviceId.ToUpper();
                    var deviceIdCount = await client.CountAsync(SecurityHelper.Hash(recordModel.DeviceId));
                    if (!deviceIdCount.Success)
                    {
                        // 如果获取失败 有可能存在数据丢失 需要去获取一下数据库数量 进行Redis数量重建
                        var result = await DalActivity.SelectTiresOrderRecordByDeviceId(recordModel.DeviceId, DateTime.Parse(DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd")));
                        if (result != null && result.Any())
                        {
                            var oldSqlCount = result?.Sum(x => x.Number);
                            sqlCount = result.Where(x => x.Pkid != recordModel.Pkid)?.Sum(x => x.Number);
                            if (sqlCount.HasValue && sqlCount >= 0)
                            {
                                var IncrementResult = await client.IncrementAsync(SecurityHelper.Hash(recordModel.DeviceId), sqlCount.Value);
                                if (!IncrementResult.Success)
                                    Logger.Error(
                                        $"Redis 轮胎下单设备ID{recordModel.DeviceId} 限购数量撤销失败订单号：{recordModel.OrderId} 数量：{recordModel.Number} 错误信息{IncrementResult.Message}");
                            }
                            else if (oldSqlCount != recordModel.Number)
                                Logger.Info($"撤销轮胎购买数量出现sql记录小于需要撤销记录  轮胎下单单用户手机号码{recordModel.DeviceId} 限购数量撤销失败订单号：{recordModel.OrderId} 数量：{recordModel.Number} sql数量:{sqlCount}");
                        }
                    }
                    else
                    {
                        var decrementResult = await client.DecrementAsync(SecurityHelper.Hash(recordModel.DeviceId), recordModel.Number);
                        if (!decrementResult.Success)
                            Logger.Error(
                                $"Redis 轮胎下单设备ID{recordModel.DeviceId} 限购数量撤销失败订单号：{recordModel.OrderId} 数量：{recordModel.Number} 错误信息{decrementResult.Message}");
                    }
                }
            }
        }
        #endregion

        #region 验证是否为限购黑名单
        /// <summary>
        /// 验证是否存在黑名单
        /// </summary>
        /// <param name="request"></param>
        /// <returns>true 存在 false 不存在</returns>
        public static async Task<bool> CheckBlackListItemAsync(BlackListItemRequest request)
        {
            try
            {
                using (var client = new Service.Config.ConfigClient())
                {
                    var result = await client.CheckBlackListItemAsync(request);
                    result.ThrowIfException(true);
                    return result.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }
        #endregion

        #region  轮胎活动
        const string RegionTiresAcvitityClientName = "RegionTiresAcvitity";
        const string FlashSaleTiresAcvitityPrefix = "FlashSaleTiresAcvitity/";
        const string TiresFloorActivityPrefix = "TiresFloorActivity/";

        public static async Task<TiresActivityResponse> FetchRegionTiresActivity(FlashSaleTiresActivityRequest request)
        {
            var result = new TiresActivityResponse();
            var data = await GetTiresActivityConfigCache(request.ActivityId);
            if (data != null)
            {
                result = await ConvertTiresActivityModel(data, request);
            }
            return result;
        }

        #region 缓存

        /// <summary>
        /// 刷新区域活动缓存
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public static async Task<bool> RefreshRegionTiresActivityCache(Guid activityId, int regionId)
        {
            var flag = false;
            await SetTiresActivityConfigCache(activityId);//刷新活动配置缓存
            var result = await GetTiresActivityConfigCache(activityId);//缓存中读取配置
            if (result != null && result.TiresFloorActivity != null && result.TiresFloorActivity.Any())
            {
                flag = await SetTiresProductCache(activityId, regionId, result);//刷新产品缓存
            }
            return flag;
        }

        /// <summary>
        /// 获取轮胎活动配置信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<TiresActivityConfig> GetTiresActivityConfigCache(Guid activityId)
        {
            using (var client = CacheHelper.CreateCacheClient(RegionTiresAcvitityClientName))
            {
                var cacheResult = await client.GetOrSetAsync($"{FlashSaleTiresAcvitityPrefix + activityId }", async () =>
                    await SelectTiresActivityConfigByActivityId(activityId), TimeSpan.FromHours(1));
                return cacheResult.Success ? cacheResult.Value : null;
            }
        }

        /// <summary>
        /// 获取抢购的轮胎产品
        /// </summary>
        /// <param name="data"></param>
        /// <param name="flashSaleInfos"></param>
        /// <param name="request"></param>
        /// <param name="isShowInstallmentPrice"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Models.TiresFloorActivityInfo>> GetFiltrationProductInfoForCache(IEnumerable<TiresFloorActivityInfo> data,
            IEnumerable<FlashSaleModel> flashSaleInfos, FlashSaleTiresActivityRequest request, bool isShowInstallmentPrice = false)
        {
            using (var client = CacheHelper.CreateCacheClient(RegionTiresAcvitityClientName))
            {
                var cacheResult = await client.GetOrSetAsync($"{TiresFloorActivityPrefix + request.ActivityId }/{request.RegionId}", async () =>
                {
                    var result = await FiltrationProductInfo(data, flashSaleInfos, request, isShowInstallmentPrice);
                    return result.ToList();
                }, TimeSpan.FromHours(1));
                return cacheResult.Success ? cacheResult.Value : null;
            }
        }

        /// <summary>
        /// 刷新轮胎活动配置信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<bool> SetTiresActivityConfigCache(Guid activityId)
        {
            using (var client = CacheHelper.CreateCacheClient(RegionTiresAcvitityClientName))
            {
                var cacheResult = await client.SetAsync($"{FlashSaleTiresAcvitityPrefix + activityId }",
                    await SelectTiresActivityConfigByActivityId(activityId), TimeSpan.FromHours(1));
                if (!cacheResult.Success)
                    Logger.Warn("刷新轮胎活动配置信息失败", cacheResult.Exception);
                return cacheResult.Success;
            }
        }

        /// <summary>
        /// 刷新过滤的轮胎产品信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="regionId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<bool> SetTiresProductCache(Guid activityId, int regionId, TiresActivityConfig data)
        {
            var result = false;

            var flashSaleInfos = await FlashSaleManager.GetFlashSaleListAsync(data.TiresFloorActivity.Select(x => x.FlashSaleId).ToArray());//获取限购信息

            using (var client = CacheHelper.CreateCacheClient(RegionTiresAcvitityClientName))
            {
                var cacheResult = await client.SetAsync($"{TiresFloorActivityPrefix + activityId }/{regionId}",
                    await FiltrationProductInfo(data.TiresFloorActivity, flashSaleInfos,
                        new FlashSaleTiresActivityRequest() { ActivityId = activityId, RegionId = regionId },
                        data.IsShowInstallmentPrice), TimeSpan.FromHours(1));
                if (!cacheResult.Success)
                    Logger.Warn("刷新过滤轮胎产品信息失败", cacheResult.Exception);
                result = cacheResult.Success;
            }

            return result;
        }

        #endregion
        /// <summary>
        /// 获取轮胎活动配置信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<TiresActivityConfig> SelectTiresActivityConfigByActivityId(Guid activityId)
        {
            var result = await DalActivity.SelectRegionTiresActivity(activityId);
            if (result != null)
            {
                result.TiresFloorActivity = (await DalActivity.SelectFloorActivityByParentId(activityId)).ToList();
                if (result.TiresFloorActivity != null && result.TiresFloorActivity.Any())
                {
                    foreach (var item in result.TiresFloorActivity)
                    {
                        item.ActivityImageConfig = (await DalActivity.FetchActivityImageConfig(item.FloorActivityId)).ToList();
                        item.TiresActivityProductConfig = (await DalActivity.FetchRegionMarketingProductConfig(item.FloorActivityId)).ToList();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 转换model
        /// </summary>
        /// <param name="data"></param>
        /// <param name="request"></param>
        /// <returns></returns>

        public static async Task<TiresActivityResponse> ConvertTiresActivityModel(TiresActivityConfig data, FlashSaleTiresActivityRequest request)
        {
            var result = new TiresActivityResponse()
            {
                ActivityId = data.ActivityId,
                ActivityName = data.ActivityName,
                WXUrl = data.WXUrl,
                AppUrl = data.AppUrl,
                ShareImg = data.ShareImg,
                ShareDes = data.ShareDes,
                ShareTitle = data.ShareTitle,
                IsAdaptationVehicle = data.IsAdaptationVehicle,
                ActivityRules = data.ActivityRules,
                ActivityRulesImg = data.ActivityRulesImg,
                HeadImg = data.HeadImg,
                NoAdaptationImg = data.NoAdaptationImg,
                BackgroundColor = data.BackgroundColor,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                IsShowInstallmentPrice = data.IsShowInstallmentPrice,
                TiresFloorActivity = new List<Models.Response.TiresFloorActivityConfig>()
            };

            if (data.TiresFloorActivity != null && data.TiresFloorActivity.Any())
            {
                var flashSaleInfos = await FlashSaleManager.GetFlashSaleListAsync(data.TiresFloorActivity.Select(x => x.FlashSaleId).ToArray());//获取限购信息

                data.TiresFloorActivity = await GetFiltrationProductInfoForCache(data.TiresFloorActivity, flashSaleInfos, request, result.IsShowInstallmentPrice);//过滤产品

                foreach (var item in data.TiresFloorActivity)
                {
                    var floorItem = new Models.Response.TiresFloorActivityConfig()
                    {
                        TiresActivityId = item.TiresActivityId,
                        FloorActivityId = item.FloorActivityId,
                        FlashSaleId = item.FlashSaleId,
                        ActivityName = item.ActivityName,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        Status = item.Status,
                        ImgType = item.ImgType,
                        ImgUrl = item.ImgUrl,
                        OtherImgAndProductMap = new List<Models.ImgAndProductMap>()
                    };

                    if (item.TireProductFiltrationInfo != null && item.TireProductFiltrationInfo.Any())
                    {
                        item.TireProductFiltrationInfo.ForEach(x =>
                        {
                            var productInfo = flashSaleInfos.Where(y => y.ActivityID == item.FlashSaleId).FirstOrDefault();
                            if (productInfo != null && productInfo.Products != null && productInfo.Products.Any())
                            {
                                var temp = productInfo.Products.Where(_ => _.PID == x.PID).FirstOrDefault();
                                x.MaxQuantity = temp != null ? temp.MaxQuantity : 0;
                                x.TotalQuantity = temp != null ? temp.TotalQuantity : 0;
                                x.SaleOutQuantity = temp != null ? temp.SaleOutQuantity : 0;
                            }
                        });
                    }

                    floorItem.OtherImgAndProductMap = item.ActivityImageConfig.Select(x => new Models.ImgAndProductMap
                    {
                        ImgType = x.ImgType,
                        ImgUrl = x.ImgUrl,
                        Position = x.Position,
                        IsAdaptaionTireSize = !string.IsNullOrEmpty(request.TireSize) ? x.ImgType == request.TireSize : false,
                        ProductDetails = AdapationTireSizeProduct(item.TireProductFiltrationInfo, x.ImgType)
                    }).Where(_ => _.ProductDetails != null && (_.ProductDetails.SloganProductInfo.Count > 0 || _.ProductDetails.NoSloganProductInfo.Count > 0)).ToList();

                    result.TiresFloorActivity.Add(floorItem);
                }
            }

            return result;
        }

        /// <summary>
        /// 过滤轮胎产品
        /// </summary>
        /// <param name="data"></param>
        /// <param name="flashSaleInfos"></param>
        /// <param name="request"></param>
        /// <param name="isShowInstallmentPrice"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Models.TiresFloorActivityInfo>> FiltrationProductInfo(IEnumerable<TiresFloorActivityInfo> data, IEnumerable<FlashSaleModel> flashSaleInfos, FlashSaleTiresActivityRequest request, bool isShowInstallmentPrice = false)
        {
            if (data != null && data.Any() && flashSaleInfos != null && flashSaleInfos.Any())
            {
                foreach (var item in data)
                {
                    item.Status = ConvertActivityStatus(item);
                    if (item.ActivityImageConfig != null && item.ActivityImageConfig.Any())//没有尺寸图片不推荐产品
                    {
                        item.ImgType = ActivityImageType.HeadImg.ToString();
                        item.ImgUrl = item.ActivityImageConfig.Where(y => y.Type == ActivityImageType.HeadImg).Select(x => x.ImgUrl).FirstOrDefault() ?? string.Empty;
                        var productInfo = flashSaleInfos.Where(x => x.ActivityID == item.FlashSaleId).FirstOrDefault()?.Products;
                        if (productInfo != null && productInfo.Any())//是否存在限购产品
                            item.TireProductFiltrationInfo = await FiltrationTireProductInfoRorRegionStock(item, productInfo, request, isShowInstallmentPrice);
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// 根据库存过滤产品
        /// </summary>
        /// <param name="floorInfo"></param>
        /// <param name="flashSaleInfo"></param>
        /// <param name="request"></param>
        /// <param name="isShowInstallmentPrice"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Models.TireProductInfo>> FiltrationTireProductInfoRorRegionStock(TiresFloorActivityInfo floorInfo,
            IEnumerable<Models.FlashSaleProductModel> flashSaleInfo, FlashSaleTiresActivityRequest request, bool isShowInstallmentPrice = false)
        {
            IEnumerable<TireInstallmentModel> installmentInfo = new List<TireInstallmentModel>();
            var result = new List<Models.TireProductInfo>();

            var productStockInfo = await GetProductRegionStock(request.RegionId, flashSaleInfo.Select(x => x.PID).ToList());//获取该地区产品的库存
            if (productStockInfo != null && productStockInfo.Any())
            {
                var productInfo = (from p in floorInfo.TiresActivityProductConfig
                                   join ps in productStockInfo on p.ProductId equals ps.Pid
                                   where (p.SpecialCondition == 0 && (ps.StockNum > ps.SafeStockNum || ps.CentralStockNum > ps.CentralSafeStockNum))
                                         || (p.SpecialCondition == 1 && (ps.StockNum > 1 || ps.CentralStockNum > 1))
                                         || (p.SpecialCondition == 2 && (ps.StockNum > 4 || ps.CentralStockNum > 4))
                                   select p).ToList();//根据库存过滤产品

                if (productInfo != null && productInfo.Any())
                {
                    if (isShowInstallmentPrice)
                        installmentInfo = await GetTiresInstallmentInfo(productInfo.Select(x => x.ProductId));
                    var tireInfo = await DalActivity.FetchSimpleTireProductInfo(string.Join(",", productInfo.Select(x => x.ProductId)));
                    result = ConvertTiresProductInfo(flashSaleInfo, productInfo, tireInfo, installmentInfo).ToList();
                }
            }

            //if (productStockInfo == null || !productStockInfo.Any() || result == null || !result.Any())
            //    EmailHelper.SendEmailNotification("zhanglei1@tuhu.cn", string.Empty, $"轮胎区域活动无产品通知", $"ActivityId:{request.ActivityId.ToString()},RegionId:{request.RegionId.ToString()}");

            return result;
        }

        /// <summary>
        /// 转换信息
        /// </summary>
        /// <param name="flashSaleInfo"></param>
        /// <param name="productInfo"></param>
        /// <param name="tireInfo"></param>
        /// <param name="installmentInfo"></param>
        /// <returns></returns>
        private static IEnumerable<Models.TireProductInfo> ConvertTiresProductInfo(IEnumerable<Models.FlashSaleProductModel> flashSaleInfo,
            IEnumerable<TiresActivityProductConfig> productInfo, IEnumerable<Models.SimpleTireProductInfo> tireInfo, IEnumerable<TireInstallmentModel> installmentInfo = null)
        {
            var result = (from fs in flashSaleInfo
                          join pl in productInfo
                          on fs.PID equals pl.ProductId
                          join tp in tireInfo on fs.PID equals tp.PID into infoData
                          from o in infoData.DefaultIfEmpty()
                          select new Models.TireProductInfo()
                          {
                              PID = fs.PID,
                              ProductName = fs.ProductName,
                              Price = fs.Price,
                              TireSize = o != null ? o.CP_Tire_Rim : string.Empty,
                              Specification = o != null ? ((!string.IsNullOrEmpty(o.CP_Tire_Width) ? o.CP_Tire_Width : string.Empty) + "/"
                                                           + (!string.IsNullOrEmpty(o.CP_Tire_AspectRatio) ? o.CP_Tire_AspectRatio : string.Empty) + "R"
                                                           + (!string.IsNullOrEmpty(o.CP_Tire_Rim) ? o.CP_Tire_Rim : string.Empty)) : string.Empty,
                              //MaxQuantity = fs.MaxQuantity,
                              //TotalQuantity = fs.TotalQuantity,
                              //SaleOutQuantity = fs.SaleOutQuantity,
                              AdvertiseTitle = !string.IsNullOrEmpty(pl.AdvertiseTitle) ? pl.AdvertiseTitle : string.Empty,
                              ProductImg = fs.ProductImg,
                              CP_Brand = fs.Brand,
                              IsCancelProgressBar = pl.IsCancelProgressBar
                          }).ToList();

            if (installmentInfo != null && installmentInfo.Any())
            {
                result.ForEach(x =>
                {
                    var installmentPrice = string.Empty;
                    var isInstallment = false;
                    var info = installmentInfo.Where(y => y.IsInstallmentOpen && String.Equals(x.PID, y.PID, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (info != null)
                    {
                        if (info.ThreePeriods != null && info.ThreePeriods == InstallmentType.Tuhu)
                        {
                            installmentPrice = $"{(Math.Ceiling(x.Price / 3 * 100) / 100).ToString("f2")}元 x 3期";
                            isInstallment = true;
                        }
                        else if (info.SixPeriods != null && info.SixPeriods == InstallmentType.Tuhu)
                        {
                            installmentPrice = $"{(Math.Ceiling(x.Price / 6 * 100) / 100).ToString("f2") }元 x 6期";
                            isInstallment = true;
                        }
                        else if (info.TwelvePeriods != null && info.TwelvePeriods == InstallmentType.Tuhu)
                        {
                            installmentPrice = $"{(Math.Ceiling(x.Price / 12 * 100) / 100).ToString("f2")}元 x 12期";
                            isInstallment = true;
                        }
                    }
                    x.InstallmentPrice = installmentPrice;
                    x.IsInstallment = isInstallment;
                });
            }
            return result;
        }

        /// <summary>
        /// 适配产品
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tireSize"></param>
        /// <returns></returns>
        private static Models.ProductDetails AdapationTireSizeProduct(IEnumerable<Models.TireProductInfo> data, string tireSize)
        {
            var result = new Models.ProductDetails()
            {
                SloganProductInfo = new List<Models.TireProductInfo>(),
                NoSloganProductInfo = new List<Models.TireProductInfo>()
            };
            if (data != null && data.Any())
            {
                result = new Models.ProductDetails()
                {
                    SloganProductInfo = data.Where(x => x.TireSize == tireSize && !string.IsNullOrEmpty(x.AdvertiseTitle)).ToList() ?? new List<Models.TireProductInfo>(),
                    NoSloganProductInfo = data.Where(x => x.TireSize == tireSize && string.IsNullOrEmpty(x.AdvertiseTitle)).ToList() ?? new List<Models.TireProductInfo>()
                };
            }
            return result;
        }

        /// <summary>
        /// 获取库存信息
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        public static async Task<List<ProductCityStock>> GetProductRegionStock(int regionId, List<string> pids)
        {
            List<ProductCityStock> result = new List<ProductCityStock>();
            try
            {
                using (var client = new ProductClient())
                {
                    var data = await client.SelectProductsRegionStockAsync(regionId, pids);
                    if (!data.Success && data.Exception != null)
                        throw data.Exception;
                    result = data.Result.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Info($"根据地区获取库存:{regionId} Error:{ex.Message}", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取分期信息
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        public static async Task<List<TireInstallmentModel>> GetTiresInstallmentInfo(IEnumerable<string> pids)
        {
            List<TireInstallmentModel> result = new List<TireInstallmentModel>();
            try
            {
                using (var client = new ProductConfigClient())
                {
                    var getRestult = await client.SelectTireInstallmentByPIDsAsync(pids);
                    if (!getRestult.Success && getRestult.Exception != null)
                        throw getRestult.Exception;
                    result = getRestult.Result.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string ConvertActivityStatus(TiresFloorActivityInfo data)
        {
            var status = "OnGoing";
            if (data != null && data.StartTime != null && data.EndTime != null)
            {
                if (data.StartTime > DateTime.Now)
                {
                    status = "NoStart";
                }
                else if (data.StartTime <= DateTime.Now && DateTime.Now <= data.EndTime)
                {
                    status = "OnGoing";
                }
                else if (data.EndTime < DateTime.Now)
                {
                    status = "Overdue";
                }
            }
            return status;
        }

        public static async Task<string> SelectTireChangedActivityAsync(TireActivityRequest request)
        {
            var cachKey = GlobalConstant.GetCacheKeyPrefixWithCache(GlobalConstant.TireChangedActivityKey)
                .Replace("{VehicleId}", request.VehicleId)
                .Replace("{TireSize}", request.TireSize);

            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.ActivityDeafultKey))
            {
                var result = await client.GetOrSetAsync(
                    cachKey,
                    () => DalActivity.SelectTireChangedActivity(request.VehicleId, request.TireSize), GlobalConstant.TireChangedActivitySpan);

                if (result.Success)
                    return result.Value;
                else
                {
                    Logger.Warn("redis查询失败 休眠100ms后重试一次");
                    Thread.Sleep(100);
                    using (var client2 = CacheHelper.CreateCacheClient(GlobalConstant.ActivityDeafultKey))
                    {
                        result = await client2.GetOrSetAsync<string>(
                            cachKey,
                            () => DalActivity.SelectTireChangedActivity(request.VehicleId, request.TireSize), GlobalConstant.TireChangedActivitySpan);
                        if (result.Success)
                            return result.Value;

                        Logger.Warn("redis查询失败2");
                        return await DalActivity.SelectTireChangedActivity(request.VehicleId, request.TireSize);
                    }
                }
            }
        }
        #endregion



        public static async Task<bool> RecordActivityProductUserRemindLogAsync(ActivityProductUserRemindRequest request)
        {
            var result = await DalActivity.RecordActivityProductUserRemindLogAsync(request);
            return result > 0;
        }

        #region 返现申请记录
        public static async Task<Models.ResultModel<bool>> InsertRebateApplyRecordNewAsync(RebateApplyRequest request)
        {
            Models.ResultModel<bool> result = new Models.ResultModel<bool> { Code = 0, IsSuccess = true };

            #region 条件判断
            var order = await FetchOrderInfoByID(request.OrderId);
            var thirdPartyChannels = await Get3rdOrderChannel();
            var orderType = await CheckOrderProductTypeByOrderIdAsync(request.OrderId);
            if (order == null || order.PKID <= 0)
            {
                result.Code = 1001;
                result.Msg = "您的订单不存在，请确认订单";
                return result;
            }
            if (order.OrderDatetime < Convert.ToDateTime("2018/01/01 00:00:00"))
            {
                result.Code = 1002;
                result.Msg = "返现活动仅针对2018年1月1日之后的订单";
                return result;
            }
            if (thirdPartyChannels.Contains(order.OrderChannel))
            {
                result.Code = 1003;
                result.Msg = "此活动仅限途虎自有平台参与";
                return result;
            }
            if (!orderType.Contains("轮胎") && !orderType.Contains("保养"))
            {
                result.Code = 1004;
                result.Msg = "本活动仅限轮胎订单或保养订单";
                return result;
            }
            if (IsBatteryOrder(order.Items))
            {
                if (!String.Equals(order.DeliveryStatus, DeliveryStatus.Signed.ToString(),
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    result.Code = 1009;
                    result.Msg = "已签收订单才可参加返现，请登录途虎养车APP确认收货。";
                    return result;
                }
            }
            else
            {
                if ((order.InstallShopId ?? 0) <= 0)
                {
                    result.Code = 1005;
                    result.Msg = "本活动仅限到途虎门店安装的订单。";
                    return result;
                }
                if (!string.Equals(order.Status, OrderStatus.Installed.ToString()))
                {
                    result.Code = 1010;
                    result.Msg = "已安装订单才可参加返现，联系客服更改状态。";
                    return result;
                }
            }
            if (!String.Equals(order.UserTel, request.UserPhone))
            {
                result.Code = 1006;
                result.Msg = "请填写正确的手机号";
                return result;
            }
            #endregion

            #region 插入申请
            using (var helper = DbHelper.CreateDbHelper())
            {
                try
                {
                    helper.BeginTransaction(IsolationLevel.ReadCommitted);
                    var existedData = await DalActivity.SelectRebateApplyConfigByParamV2(helper, request);
                    existedData =
                        existedData.Where(x => String.Equals(x.Status, "Applying")
                                               || String.Equals(x.Status, "Approved")
                                               || String.Equals(x.Status, "Complete")
                        ).ToList();
                    if (existedData?.Where(x => String.Equals(x.OrderId, request.OrderId)).Count() > 0
                        || existedData?.Where(x => String.Equals(x.UserPhone, request.UserPhone)).Count() > 0
                        || existedData?.Where(x => !string.IsNullOrEmpty(x.OpenId) && String.Equals(x.OpenId, request.OpenId)).Count() > 0)
                    {
                        result.Code = 1007;
                        result.Msg = "每个客户只能参与一次（包含手机号、订单号、微信号）均视为同一客户";
                    }
                    else
                    {
                        request.RebateMoney = 25;
                        request.Remarks = orderType;
                        var pkid = await DalActivity.InsertRebateApplyRecordV2(helper, request, "Rebate25");
                        if (pkid > 0 && request.ImgList != null && request.ImgList.Any())
                        {
                            foreach (var imgUrl in request.ImgList)
                            {
                                await DalActivity.InsertRebateApplyImage(helper, pkid, imgUrl);
                            }
                        }
                        result.Code = 1000;
                        result.Msg = "申请成功";
                        result.Result = true;
                        await PushWechatInfoByBatchId(3381, new PushTemplateLog() { Target = request.OpenId });
                    }
                    helper.Commit();
                }
                catch (Exception ex)
                {
                    result.Code = 1008;
                    result.Msg = "网络异常，请刷新重试";
                    helper.Rollback();
                    Logger.Error("返现申请记录异常", ex);
                }
            }
            #endregion

            return result;
        }

        public static async Task<bool> PushWechatInfoByBatchId(int batchId, PushTemplateLog template)
        {
            var result = false;
            try
            {
                var templateResult = await SelectTemplateByBatchID(batchId);
                if (templateResult != null && templateResult.Any())
                {
                    var wxTemplate = templateResult.FirstOrDefault(x => x.DeviceType == DeviceType.WeChat);
                    if (wxTemplate != null)
                    {
                        result = await PushTemplate(wxTemplate, template);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return result;
        }

        public static async Task<List<PushTemplate>> SelectTemplateByBatchID(int batchId)
        {
            List<PushTemplate> result = new List<PushTemplate>();
            try
            {
                using (var client = new TemplatePushClient())
                {
                    var getResult = await client.SelectTemplateByBatchIDAsync(batchId);
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return result;
        }

        public static async Task<bool> PushTemplate(PushTemplate data, PushTemplateLog template)
        {
            var result = false;
            try
            {
                using (var client = new WeiXinPushClient())
                {
                    var getResult = await client.PushByTemplateAsync(data, template);
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return result;
        }

        private static bool IsBatteryOrder(IEnumerable<OrderItemModel> orderItems)
        {
            var result = false;
            if (orderItems != null &&
                orderItems.Where(x => String.Equals(x.Category, "battery", StringComparison.InvariantCultureIgnoreCase) ||
                String.Equals(x.Category, "XDCFW", StringComparison.InvariantCultureIgnoreCase)).Count() == orderItems.Count())
            {
                result = true;
            }
            return result;
        }

        public static async Task<List<RebateApplyResponse>> SelectRebateApplyByOpenIdAsync(string openId)
        {
            var result = new List<RebateApplyResponse>();
            if (!string.IsNullOrEmpty(openId))
            {
                result = await DalActivity.GetRebateApplyByOpenId(openId);
                if (result != null && result.Any())
                {
                    foreach (var item in result)
                    {
                        item.ImgList = await DalActivity.SelectRebateApplyImages(item.PKID);
                    }
                }
            }
            return result;
        }

        public static async Task<string> CheckOrderProductTypeByOrderIdAsync(int orderId)
        {
            var result = string.Empty;
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = await orderClient.CheckOrderProductTypeByOrderIdAsync(orderId);
                    fetchResult.ThrowIfException(true);
                    result = fetchResult.Success ? fetchResult.Result : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CheckOrderProductTypeByOrderIdAsync 接口异常", ex);
            }
            return result ?? string.Empty;
        }

        public static async Task<IEnumerable<string>> Get3rdOrderChannel()
        {
            IEnumerable<string> result = new List<string>();
            try
            {
                using (var client = new OrderQueryClient())
                {
                    var getResult = await client.Get3rdOrderChannelAsync();//获取三方渠道集合
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CheckOrderProductTypeByOrderIdAsync 接口异常", ex);
            }
            return result;
        }

        public static async Task<OrderModel> FetchOrderByOrderId(int orderId)
        {
            OrderModel order = null;
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = await orderClient.FetchOrderByOrderIdAsync(orderId);
                    fetchResult.ThrowIfException(true);
                    order = fetchResult.Success ? fetchResult.Result : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FetchOrderByOrderId 接口异常", ex);
            }
            return order;
        }

        public static async Task<OrderModel> FetchOrderInfoByID(int orderId)
        {
            OrderModel order = null;
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = await orderClient.FetchOrderInfoByIDAsync(orderId);
                    fetchResult.ThrowIfException(true);
                    order = fetchResult.Success ? fetchResult.Result : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FetchOrderInfoByID 接口异常", ex);
            }
            return order;
        }

        public static async Task<RebateApplyPageConfig> SelectRebateApplyPageConfigAsync()
        {
            RebateApplyPageConfig result = null;
            try
            {
                result = await DalActivity.SelectRebateApplyPageConfig();
                if (result != null && result.PKID > 0)
                {
                    result.ImgList = await DalActivity.SelectRebateApplyImageConfig(result.PKID);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SelectRebateApplyPageConfig 接口异常", ex);
            }
            return result;
        }
        #endregion

        #region 活动页白名单
        /// <summary>
        /// 初始化白名单数据或者后面个别用户白名单状态调整使用
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        public static async Task<bool> InsertOrUpdateActivityPageWhiteListRecordsAsync(
            List<ActivityPageWhiteListRequest> requests)
        {
            var result = true;
            foreach (var request in requests)
            {
                using (var client = new UserAccountClient())
                {
                    try
                    {
                        var userResult = await client.GetUserByMobileAsync(request.PhoneNum);
                        userResult.ThrowIfException();
                        if (userResult.Result == null)
                        {
                            result = false;
                            Logger.Error($"接口==》InsertOrUpdateActivityPageWhiteListRecordsAsync，该手机号{request.PhoneNum}并未注册");
                        }
                        else
                        {
                            var whiteListModel = new ActivityPageWhiteListModel()
                            {
                                UserId = userResult.Result.UserId,
                                PhoneNum = request.PhoneNum,
                                Status = request.Status
                            };
                            var dbResult = await DalActivity.MergeIntoActivityPageWhiteListRecord(whiteListModel);
                            result = dbResult > 0 && result;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"调用用户账号接口GetUserByMobileAsync报错", e.InnerException);
                        result = false;
                    }
                }
            }
            //换key
            var cacheResult = await CacheManager.CommonRefreshKeyPrefixAsync(DefaultClientName,
                GlobalConstant.WhiteListKeyPrefix, TimeSpan.FromDays(1));
            return result && cacheResult;
        }

        public static async Task<bool> GetActivityPageWhiteListByUserIdAsync(Guid userId)
        {
            var keyprefix = await CacheManager.CommonGetKeyPrefixAsync(DefaultClientName,
                GlobalConstant.WhiteListKeyPrefix);
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var cacheResult = await client.GetOrSetAsync($"{keyprefix}/{userId}",
                    () => DalActivity.GetActivityPageWhiteListByUserIdAsync(userId), TimeSpan.FromDays(1));
                if (cacheResult.Success)
                {
                    return cacheResult.Value > 0;
                }
                else
                {
                    Logger.Error($"从redis中获取白名单信息报错接口GetActivityPageWhiteListByUserIdAsync");
                    var dbResult = await DalActivity.GetActivityPageWhiteListByUserIdAsync(userId);
                    return dbResult > 0;
                }
            }
        }
        #endregion

        #region 3-16活动


        public static async Task<OperationResult<UserRewardApplicationResponse>> PutUserRewardApplicationAsync(UserRewardApplicationRequest request)
        {
            var enumStatus = await CheckParameter(request);
            if (enumStatus.Status < 0)
            {
                return OperationResult.FromResult(new UserRewardApplicationResponse
                {
                    Code = (int)enumStatus.Status,
                    Message = enumStatus.Status.GetRemark()
                });
            }
            var result = await DalActivity.InsertUserRewardApplication(request);
            if (result > 0)
            {
                return OperationResult.FromResult(new UserRewardApplicationResponse()
                {
                    Code = 1,
                    Message = "成功"
                });
            }
            return OperationResult.FromError<UserRewardApplicationResponse>("-12", "插入异常");
        }

        private static async Task<RewardApplicationCheckModel> CheckParameter(UserRewardApplicationRequest request)
        {
            #region 活动有效期
            var dt = DateTime.Now;
            if (dt > new DateTime(2018, 3, 27))
            {
                return new RewardApplicationCheckModel(RewardApplicationCheckStatus.Expired);
            }

            #endregion
            #region Name
            if (string.IsNullOrWhiteSpace(request.ApplicationName))
            {
                return new RewardApplicationCheckModel(RewardApplicationCheckStatus.NameEmpty);
            }
            if (request.ApplicationName.Length > 20)//中英文长度问题待测试
            {
                return new RewardApplicationCheckModel(RewardApplicationCheckStatus.NameTooLong);
            }
            #endregion
            #region Phone
            if (string.IsNullOrWhiteSpace(request.Phone))
            {
                return new RewardApplicationCheckModel(RewardApplicationCheckStatus.PhoneEmpty);
            }
            if (!Regex.IsMatch(request.Phone, @"^[1][3|4|5|6|7|8]{1}[0-9]{9}$"))
            {
                return new RewardApplicationCheckModel(RewardApplicationCheckStatus.PhoneWrong);
            }
            try
            {
                using (var client = new UserAccountClient())
                {
                    var userResult = await client.GetUserByMobileAsync(request.Phone);
                    userResult.ThrowIfException();
                    if (userResult.Result == null)
                    {
                        return new RewardApplicationCheckModel(RewardApplicationCheckStatus.AccountWrong);

                    }

                }
            }
            catch (Exception e)
            {
                Logger.Error($"调用用户账号接口GetUserByMobileAsync报错", e.InnerException);
            }
            #endregion
            #region Image

            if (string.IsNullOrWhiteSpace(request.ImageUrl1) &&
                (string.IsNullOrWhiteSpace(request.ImageUrl2) && (string.IsNullOrWhiteSpace(request.ImageUrl3))))
            {
                return new RewardApplicationCheckModel(RewardApplicationCheckStatus.ImageEmpty);

            }

            #endregion
            #region Other
            var status = await DalActivity.SelectUserRewardApplicationByPhoneAsync(request.Phone);
            switch (status)
            {
                case 2:
                    return new RewardApplicationCheckModel(RewardApplicationCheckStatus.Passed);
                case 1:
                    return new RewardApplicationCheckModel(RewardApplicationCheckStatus.UnderReview);
            }
            #endregion

            return new RewardApplicationCheckModel(RewardApplicationCheckStatus.Succeed);
        }
        #endregion

        #region 对途虎商品贵就赔的活动,从website迁移过来代码

        public static async Task<OperationResult<bool>> PutApplyCompensateRecordAsync(ApplyCompensateRequest request)
        {
            var orderId = await DalActivity.SelectApplyCompensateAsync(request.OrderId);
            if (!string.IsNullOrEmpty(orderId))
            {
                return OperationResult.FromError<bool>("-1", $"您已提交过{orderId}这个订单，请勿重复提交");
            }
            return OperationResult.FromResult(await DalActivity.InsertApplyCompensateAsync(request));
        }

        #endregion

        #region 批量活动有效性验证接口

        private static bool CheckActivtyValidity(DateTime dtS, DateTime dtE)
        {
            return dtS < DateTime.Now && dtE > DateTime.Now;
        }

        public static async Task<List<ActivtyValidityResponse>> GetActivtyValidityResponsesAsync(ActivtyValidityRequest request)
        {
            var listResult = new List<ActivtyValidityResponse>();
            var actRequest = new ActivtyPageRequest
            {
                Channel = request.Channel
            };
            foreach (var hashKey in request.HashKeys)
            {
                var cacheResult = await GetActivePageListModel(0, actRequest, hashKey, false);
                listResult.Add(new ActivtyValidityResponse
                {
                    HashKey = hashKey,
                    IsValidity = CheckActivtyValidity(cacheResult.StartDate, cacheResult.EndDate)
                });
            }
            return listResult;
        }
        #endregion

        #region vipcard 预付卡
        /// <summary>
        /// 获取预付卡场次信息
        /// </summary>
        public static async Task<List<VipCardSaleConfigDetailModel>> GetVipCardSaleConfigDetailsAsync(string activityId)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.GetOrSetAsync(GlobalConstant.VipCardPrefix + activityId,
                    () => DalActivity.GetVipCardSaleConfigDetailsAsync(activityId), TimeSpan.FromDays(10));

                List<VipCardSaleConfigDetailModel> detailModels;
                if (result.Success)
                {
                    detailModels = result.Value.Where(r => r.EndDate > DateTime.Now).ToList();
                }
                else
                {
                    Logger.Warn($"查询redis缓存失败，接口==》GetVipCardSaleConfigDetailsAsync");
                    detailModels = (await DalActivity.GetVipCardSaleConfigDetailsAsync(activityId)).Where(r => r.EndDate > DateTime.Now).ToList();
                }
                //todo 1,对于缓存问题直接读取数据库操作需要优化下;
                using (var cacheClient = CacheHelper.CreateCounterClient(DefaultClientName))
                {
                    foreach (var item in detailModels)
                    {
                        var counter = await cacheClient.CountAsync(GlobalConstant.VipCardCounterPrefix + item.BatchId);
                        var saleQty = 0;
                        if (!counter.Success)
                        {
                            if (counter.Message == "Key不存在")
                            {
                                saleQty = await DalActivity.GetVipCardDetailSumStockAsync(item.BatchId);

                            }
                            else
                            {
                                saleQty = await DalActivity.GetVipCardDetailSumStockAsync(item.BatchId);
                                Logger.Error($"Vipcard获取库存缓存失败，redis报错" +
                                             $"批次Id==》{item.BatchId}");
                            }
                        }
                        else
                        {
                            saleQty = Convert.ToInt32(counter.Value);
                        }
                        item.RemainingStock = item.Stock - saleQty;
                    }
                }
                return detailModels;
            }
        }
        /// <summary>
        /// check购买的批次是否还有剩余库存
        /// </summary>
        /// <param name="batchIds"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, bool>> VipCardCheckStockAsync(List<string> batchIds)
        {
            var dic = new Dictionary<string, bool>();
            foreach (var batchId in batchIds)
            {
                var stock = 0;
                using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
                {
                    var result = await client.GetOrSetAsync(GlobalConstant.VipCardPrefix + batchId, () => DalActivity.GetVipCardStockAsync(batchId),
                        TimeSpan.FromDays(10));
                    if (result.Success)
                    {
                        stock = result.Value;
                    }
                    else
                    {
                        stock = await DalActivity.GetVipCardStockAsync(batchId);
                    }
                }
                var saleQty = await DalActivity.GetVipCardDetailSumStockAsync(batchId);
                dic.Add(batchId, stock >= saleQty);
            }
            return dic;
        }

        /// <summary>
        /// 创建订单时记录卡信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<bool> PutVipCardRecordAsync(VipCardRecordRequest request)
        {
            var result = await DalActivity.PutVipCardRecordAsync(request);
            var flag = true;
            var vipcardId = await DalActivity.SelectVipCardId(request.ActivityId);
            foreach (var batch in request.Batches)
            {
                var saleQty = 0;
                using (var client = CacheHelper.CreateCounterClient(DefaultClientName))
                {
                    var counter = await client.CountAsync(GlobalConstant.VipCardCounterPrefix + batch.BatchId);
                    if (!counter.Success)
                    {
                        if (counter.Message == "Key不存在")
                        {
                            saleQty = await DalActivity.GetVipCardDetailSumStockAsync(batch.BatchId);

                        }
                        else
                        {
                            Logger.Error($"Vipcard记录到缓存失败，redis报错" +
                                         $"订单号===》{request.OrderId}批次Id==》{batch.BatchId}数量==》{batch.CardNum}");
                        }
                    }

                    var cacheResult = await client.IncrementAsync(GlobalConstant.VipCardCounterPrefix + batch.BatchId, saleQty + batch.CardNum);
                    if (!cacheResult.Success)
                    {
                        Logger.Error($"Vipcard记录到缓存失败，redis报错" +
                                     $"订单号===》{request.OrderId}批次Id==》{batch.BatchId}数量==》{batch.CardNum}");
                    }
                    flag = flag && cacheResult.Success;

                }
                var dbReuslt = await DalActivity.UpdateVipCardDetailQtyByBatchId(batch.BatchId, batch.CardNum, vipcardId);
                if (!dbReuslt)
                {
                    Logger.Error($"Vipcard更新销量报错" +
                                 $"订单号===》{request.OrderId}批次Id==》{batch.BatchId}数量==》{batch.CardNum}活动==》{request.ActivityId}");
                }
                flag = flag && dbReuslt;

            }
            return result && flag;
        }

        /// <summary>
        /// 支付成功时调用绑卡
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static async Task<bool> BindVipCardAsync(int orderId)
        {
            var url = GlobalConstant.VipCardBindUrl;
            var dalresult = await DalActivity.GetVipCardRecordByOrderIdAsync(orderId);

            var model = new
            {
                ownerPhone = dalresult.UserPhone,
                ownerUserId = dalresult.UserId,
                platformId = "C",
                list = from a in dalresult.Batches
                       select new
                       {
                           batchNo = a.BatchId,
                           cardNums = a.CardNum
                       }
            };
            var body = JsonConvert.SerializeObject(model);
            var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip };
            using (var client = new HttpClient(handler))
            {
                var contenbody = new StringContent(body, Encoding.UTF8, "application/json");
                try
                {
                    var response = await client.PostAsync(url, contenbody);
                    //确保HTTP成功状态值
                    response.EnsureSuccessStatusCode();
                    var responseMes = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<VipCardBindInfoModel>(responseMes);
                    if (res.ReturnCode == 1)
                    {
                        return true;
                    }
                    else
                    {
                        Logger.Info($"vipCard绑卡失败orderId==>{orderId}, returnMsg{res.ReturnMsg}");
                        await DalActivity.PutVipCardRecordAsync(dalresult, 2, res.ReturnMsg + url);
                        //await ModifyVipCardRecordByOrderIdAsync(orderId);
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"vipCard绑卡失败orderId==>{orderId}", e.InnerException);
                    await DalActivity.PutVipCardRecordAsync(dalresult, 2, e.Message + url);
                    //await ModifyVipCardRecordByOrderIdAsync(orderId);
                    return false;
                }

            }
        }

        public static async Task<bool> ModifyVipCardRecordByOrderIdAsync(int orderId)
        {
            var dalResult = await DalActivity.GetVipCardRecordByOrderIdAsync(orderId);
            var flag = true;
            if (dalResult != null && dalResult.OrderId != 0)
            {
                var dalLog = await DalActivity.UpdateVipCardBuyLogByOrderId(orderId);
                if (!dalLog)
                {
                    Logger.Error($"Vipcard取消订单更细表失败订单号===》{orderId}");
                }
                else
                {
                    var vipcardId = await DalActivity.SelectVipCardId(dalResult.ActivityId);
                    foreach (var batch in dalResult.Batches)
                    {

                        var dbReuslt = await DalActivity.UpdateVipCardDetailQtyByBatchId(batch.BatchId, -batch.CardNum, vipcardId);
                        if (!dbReuslt)
                        {
                            Logger.Error($"Vipcard更新销量报错" +
                                         $"订单号===》{orderId}批次Id==》{batch.BatchId}数量==》{batch.CardNum}活动==》{dalResult.ActivityId}");
                        }
                        flag = flag && dbReuslt;
                        var saleQty = 0;
                        using (var client = CacheHelper.CreateCounterClient(DefaultClientName))
                        {
                            var counter = await client.CountAsync(GlobalConstant.VipCardCounterPrefix + batch.BatchId);
                            if (!counter.Success)
                            {
                                if (counter.Message != "Key不存在")
                                {
                                    Logger.Error($"Vipcard取消订单更新缓存失败，redis报错" +
                                                 $"订单号===》{dalResult.OrderId}批次Id==》{batch.BatchId}数量==》{batch.CardNum}");

                                }

                            }
                            else
                            {
                                var cacheResult = await client.DecrementAsync(GlobalConstant.VipCardCounterPrefix + batch.BatchId, saleQty + batch.CardNum);
                                if (!cacheResult.Success)
                                {
                                    Logger.Error($"Vipcard取消订单更新缓存失败，redis报错" +
                                                 $"订单号===》{dalResult.OrderId}批次Id==》{batch.BatchId}数量==》{batch.CardNum}");
                                }
                                flag = flag && cacheResult.Success;

                            }
                        }
                    }
                }
            }
            return flag;
        }
        #endregion
        #region 设置或者获取排序好的商品


        private static async Task<bool> SetHashClientCache(string clientName, FlashSaleModel flashSale)
        {
            using (var hashClient = CacheHelper.CreateHashClient(clientName, TimeSpan.FromDays(30)))
            {
                var dic = new Dictionary<string, object>();
                foreach (var p in flashSale.Products)
                {
                    var key = p.PID;
                    dic.Add(key, p.Position);
                }
                IReadOnlyDictionary<string, object> roDic = new ReadOnlyDictionary<string, object>(dic);
                var setResult = await hashClient.SetAsync<IReadOnlyDictionary<string, object>>(roDic);
                if (!setResult.Success)
                {
                    Logger.Error($"redis 分车型创建的活动商品刷新到缓存失败", setResult.Exception);
                    return false;
                }
                return true;
            }
        }

        public static async
            Task<Tuple<KeyValuePair<int, string>, List<string>>> GetOrSetActivityPageSortedPidsAsync(SortedPidsRequest request)
        {
            List<string> product = null;
            FlashSaleModel flashSale = null;
            IResult<IDictionary<string, int>> cacheresult = null;
            var result = new Tuple<KeyValuePair<int, string>, List<string>>(new KeyValuePair<int, string>
(1, "成功"), null);
            #region 【1】如果勾选了品牌,获取默认排序的商品
            if (!string.IsNullOrEmpty(request.Brand) || request.ProductType != ProductType.Default)
            {
                var Parameters = new Dictionary<string, IEnumerable<string>>()
                {
                    ["OnSale"] = new string[] { "1" },
                    ["stockout"] = new string[] { "0" },
                };
                if (request.ProductType != ProductType.Default)
                {
                    Parameters["Category"] = new[] { request.ProductType.ToString() };
                }
                if (!string.IsNullOrEmpty(request.Brand))
                {
                    Parameters["CP_Brand"] = new[] { request.Brand };
                }
                var referRequest = new SearchProductRequest()
                {
                    Parameters = Parameters,
                    PageSize = 1000,
                };
                product = await SearchProductByPidsAsync(referRequest);
            }
            #endregion
            #region【2】如果填写了活动id，要获取该活动id下的所有有顺序商品
            if (!string.IsNullOrEmpty(request.DicActivityId.Key) && request.DicActivityId.Value == ActivityIdType.FlashSaleActivity)
            {
                flashSale = await FlashSaleManager.SelectFlashSaleDataByActivityIDAsync(new Guid(request.DicActivityId.Key));
            }
            #endregion
            #region 【3】后台创建活动，sethash缓存
            if (product != null || flashSale != null)
            {
                var newflashSale = new FlashSaleModel()
                {
                    ActivityName = "后台创建的活动页排序用活动,勿动",
                    ActiveType = -1,
                    ActivityID = Guid.NewGuid()
                };
                var flashProducts = new List<FlashSaleProductModel>();
                if (product != null && flashSale != null)
                {
                    flashProducts = (from flash in flashSale.Products ?? new List<FlashSaleProductModel>()
                                     join p in product on flash.PID equals p
                                     select new FlashSaleProductModel
                                     {
                                         PID = flash.PID,
                                         Position = flash.Position
                                     }).ToList();
                }
                if (product != null && flashSale == null)
                {
                    flashProducts = product.Select((i, index) => new FlashSaleProductModel
                    {
                        PID = i,
                        Position = index
                    }).ToList();
                }
                if (flashSale != null && product == null)
                {
                    if (flashSale.Products == null || !flashSale.Products.Any())
                    {
                        result = new Tuple<KeyValuePair<int, string>, List<string>>(new KeyValuePair<int, string>
                            (6, $"传入的活动id{request.DicActivityId.Key}无效"), null);
                    }
                    else
                    {
                        flashProducts = flashSale.Products.Select(i => new FlashSaleProductModel
                        {
                            PID = i.PID,
                            Position = i.Position
                        }).ToList();
                    }
                }
                if (flashProducts.Any())
                {
                    //后台创建一个活动
                    var intactivityId = await DalActivity.CreateActivityAsync(newflashSale);
                    if (intactivityId <= 0)
                    {
                        Logger.Error($"后台创建活动失败，活动页pkid:{request.NeedUpdatePkid}");
                    }
                    else
                    {
                        var actProducts = await DalActivity.CreateActivityProductsAsync(flashProducts,
                            newflashSale.ActivityID.ToString());
                        newflashSale.Products = flashProducts;
                        //创建hash结构缓存
                        var hash = await SetHashClientCache(newflashSale.ActivityID.ToString(), newflashSale);
                        if (!hash)
                            result = new Tuple<KeyValuePair<int, string>, List<string>>(new KeyValuePair<int, string>(2, "创建hash结构缓存失败")
                            , null);
                        var exist = await DalActivity.SelectActivityPageContentSystemActivityIdAsync(request.NeedUpdatePkid);
                        if (!string.IsNullOrEmpty(exist))
                        {
                            var delete = await DalActivity.DeleteActivityAsync(exist);
                        }
                        var update = await DalActivity.UpdateActivityPageContentAsync(request.NeedUpdatePkid,
                            newflashSale.ActivityID.ToString());
                    }
                }
                else
                {
                    result = new Tuple<KeyValuePair<int, string>, List<string>>(new KeyValuePair<int, string>
                            (5, "所选条件筛选出的产品为空，请排查所选类目跟活动id"), null);
                }

            }
            #endregion
            #region 【4】如果是要刷新缓存走这里
            if (request.IsRefresh && request.DicActivityId.Key != null &&
                request.DicActivityId.Value == ActivityIdType.AutoActivity)
            {
                flashSale = await FlashSaleManager.SelectFlashSaleModelByActivityIdAsync(new Guid(request.DicActivityId.Key));
                var hash = await SetHashClientCache(flashSale.ActivityID.ToString(), flashSale);
                result = new Tuple<KeyValuePair<int, string>, List<string>>(new KeyValuePair<int, string>
(3, "刷新缓存失败"), null);
            }
            #endregion
            #region 【5】Get缓存
            if (!request.IsRefresh && request.DicActivityId.Key != null &&
    request.DicActivityId.Value == ActivityIdType.AutoActivity)
            {
                using (var hashClient = CacheHelper.CreateHashClient(request.DicActivityId.Key))
                {
                    cacheresult = await hashClient.GetAsync<int>(request.AdaptPids);
                    if (!cacheresult.Success)
                    {
                        Logger.Error($"redis 分车型创建的活动商品获取缓存失败", cacheresult.Exception);
                        result = new Tuple<KeyValuePair<int, string>, List<string>>(new KeyValuePair<int, string>
(4, "获取缓存数据失败"), null);
                    }
                    else
                        result = new Tuple<KeyValuePair<int, string>, List<string>>(new KeyValuePair<int, string>
(1, "成功"), cacheresult.Value.OrderBy(v => v.Value).Select(key => key.Key).ToList());
                }
            }
            #endregion
            return result;
        }
        #endregion
        #region 活动

        /// <summary>
        ///     获取2018世界杯的活动对象和积分规则信息
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<ActivityResponse>> GetWorldCup2018ActivityAsync()
        {
            try
            {
                //定义函数
                var func = new Func<Task<ActivityResponse>>(async () =>
                {
                    var activityModel = await DalActivity.GetActivityByTypeId(0);
                    //AutoMapper初始化配置文件
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<ActivityModel, ActivityResponse>();
                    });
                    var mapper = config.CreateMapper();
                    var result = mapper.Map<ActivityResponse>(activityModel);
                    if (result != null)
                    {
                        using (var userIntegralClient = new UserIntegralClient())
                        {
                            //获取积分规则
                            var ruleShare = await userIntegralClient.GetIntegralRuleByRemarkAsync("2018世界杯分享送积分");
                            var ruleAnswer = await userIntegralClient.GetIntegralRuleByRemarkAsync("2018世界杯答题消耗积分");
                            try
                            {
                                ruleShare.ThrowIfException(true);
                                ruleAnswer.ThrowIfException(true);
                            }
                            catch (Exception e)
                            {
                                Logger.Error($"获取2018世界杯活动 2018世界杯分享送积分  失败 ", e);
                            }
                            if (!ruleShare.Success || ruleShare?.Result?.IntegralRuleID == null)
                            {
                                Logger.Warn($"获取2018世界杯活动 2018世界杯分享送积分  失败  {ruleShare.ErrorMessage}");
                            }
                            if (!ruleAnswer.Success || ruleAnswer?.Result?.IntegralRuleID == null)
                            {
                                Logger.Warn($"获取2018世界杯活动 2018世界杯答题消耗积分  失败  {ruleAnswer.ErrorMessage}");
                            }
                            result.ShareIntegralRuleID = ruleShare?.Result?.IntegralRuleID ?? Guid.Empty;
                            result.UserSelectionIntegralRuleID = ruleAnswer?.Result?.IntegralRuleID ?? Guid.Empty;
                        }
                    }
                    return result;
                });
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
                {
                    var cacheResult =
                        await cacheClient.GetOrSetAsync("WorldCup2018Activity", func, TimeSpan.FromMinutes(5));
                    if (cacheResult.Success)
                    {
                        return OperationResult.FromResult(cacheResult.Value);
                    }
                    else
                    {
                        Logger.Warn($"获取redis缓存失败 {nameof(GetWorldCup2018ActivityAsync)}");
                        return OperationResult.FromResult(await func());
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"获取2018世界杯活动对象失败", e.InnerException ?? e);
                return OperationResult.FromError<ActivityResponse>("-1", Resource.Activity_Error);
            }
        }

        /// <summary>
        ///     获取活动对象
        /// </summary>
        /// <returns></returns>
        private static async Task<ActivityModel> GetActivityById(long activityId)
        {
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
            {
                var cacheResult = await cacheClient.GetOrSetAsync($"GetActivityById/{activityId}", async () =>
                 {
                     //活动对象
                     var activityModel = await DalActivity.GetActivityById(activityId);
                     return activityModel;
                 }, TimeSpan.FromMinutes(5));
                if (cacheResult.Success)
                {
                    return cacheResult.Value;
                }
                return null;
            }
        }

        /// <summary>
        ///     获取活动等级
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        private static async Task<IList<ActivityLevelModel>> SearchActivityLevel(long activityId)
        {
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
            {
                var cacheResult = await cacheClient.GetOrSetAsync($"SearchActivityLevel/{activityId}", async () =>
                {
                    //活动对象
                    var activityModel = (await DalActivity.GetActivityLevelByActivityId(activityId)).ToList();
                    activityModel.ForEach(p =>
                    {
                        if (p.StartCount == null)
                        {
                            p.StartCount = int.MinValue;
                        }
                        if (p.EndCount == null)
                        {
                            p.EndCount = int.MaxValue;
                        }
                    });
                    return activityModel;
                }, TimeSpan.FromMinutes(5));
                if (cacheResult.Success)
                {
                    return cacheResult.Value;
                }
                return null;
            }
        }


        /// <summary>
        ///     获取活动排名设置
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        private static async Task<ActivityCouponRankSettingModel> SearchActivityRankSetting(long activityId)
        {
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
            {
                var cacheResult = await cacheClient.GetOrSetAsync($"SearchActivityRankSetting/{activityId}", async () =>
                {
                    //活动对象
                    var activityModel = (await DalActivity.GetActivityCouponRankSettingByActivityId(activityId));
                    return activityModel;
                }, TimeSpan.FromMinutes(5));
                if (cacheResult.Success)
                {
                    return cacheResult.Value;
                }
                return null;
            }
        }


        /// <summary>
        ///     通过用户ID获取兑换券数量接口
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<int>> GetCouponCountByUserIdAsync(Guid userId, long activityId)
        {
            try
            {
                var coupon = await GetUserActivityCoupon(userId, activityId);
                return OperationResult.FromResult(coupon?.CouponCount ?? 0);
            }
            catch (Exception e)
            {
                Logger.Error($"通过用户ID获取兑换券数量接口失败==》{userId:D} {activityId} ", e.InnerException ?? e);
                return OperationResult.FromError<int>("-1", Resource.Invalid_SystemError);
            }
        }

        /// <summary>
        ///     今日竞猜题目
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<IEnumerable<Models.Response.Question>>> SearchQuestionAsync(long activityId)
        {
            try
            {
                //活动对象
                var activityModel = await GetActivityById(activityId);
                if (activityModel == null)
                {
                    //找不到活动
                    throw new Exception(Resource.Invalid_Activity_NotFound);
                }

                //问题
                var questions = await SearchQuestion(activityModel.QuestionnaireID);

                return OperationResult.FromResult(questions);
            }
            catch (Exception e)
            {
                Logger.Error($"获取今日竞猜失败==》 {activityId} ", e.InnerException ?? e);
                return OperationResult.FromError<IEnumerable<Models.Response.Question>>("-1", Resource.Invalid_SystemError);
            }
        }

        /// <summary>
        ///     获取用户的回答
        /// </summary>
        /// <returns></returns>
        private static async Task<IEnumerable<QuestionnaireAnswerRecordModel>> SearchQuestionnaireAnswerRecordInfo(bool readOnly
            , Guid userId
            , long questionnaireID
            , long questionId)
        {
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
            {
                var cacheResult = await cacheClient.GetOrSetAsync($"/SearchQuestionnaireAnswerRecordInfo/{userId:N}/{questionnaireID}/{questionId}", async () =>
                {
                    //用户是否已经答题
                    var answerRecordInfo = await DalQuestionnaireAnswerRecord.SearchQuestionnaireAnswerRecordInfo(true, userId, questionnaireID,
                        questionId);
                    return answerRecordInfo;

                }, TimeSpan.FromMinutes(30));
                return cacheResult?.Value;
            }
        }


        /// <summary>
        ///     今日竞猜题目
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<IEnumerable<Models.Response.Question>>> SearchQuestionAsync(Guid userId, long activityId)
        {
            try
            {
                //活动对象
                var activityModel = await GetActivityById(activityId);
                if (activityModel == null)
                {
                    //找不到活动
                    throw new Exception(Resource.Invalid_Activity_NotFound);
                }

                //获取问题和选项
                var questions = (await SearchQuestion(activityModel.QuestionnaireID)).ToList();
                //判断是否没有传用户参数
                if (userId != Guid.Empty)
                {
                    using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
                    {
                        //循环、判断用户的选择并且进行赋值
                        await questions.ForEachAsync(async question =>
                        {

                            /*
                                通过MQ保存用户答题数据的，会先保存到Redis中，所以先取一下Redis数据
                            */
                            var cacheKey = $"SaveQuestionAnswer/{activityId}/{userId:N}/{question.QuestionID}";
                            var cacheResult = cacheClient.GetAsync<int?>(cacheKey);
                            if (cacheResult.Result != null && (cacheResult.Result?.Value ?? 0) > 0)
                            {
                                SetQuestionOptionUserAnswer(question, new List<QuestionnaireAnswerRecordModel>()
                                {
                                    new QuestionnaireAnswerRecordModel()
                                    {
                                        AnswerOptionID = cacheResult.Result.Value ?? 0
                                    }
                                });
                                //已经参与
                                question.QuestionStatus = 2;
                            }
                            else
                            {
                                //用户是否已经答题
                                var answerRecordInfo = await SearchQuestionnaireAnswerRecordInfo(true, userId, activityModel.QuestionnaireID,
                                    question.QuestionID);
                                //获取用户的答案
                                if (answerRecordInfo != null && question.QuestionOptionList != null)
                                {
                                    SetQuestionOptionUserAnswer(question, answerRecordInfo);
                                }
                                //判断选项状态
                                if (answerRecordInfo?.Count() > 0)
                                {
                                    //已经参与
                                    question.QuestionStatus = 2;
                                }
                            }
                        });
                    }
                }
                return OperationResult.FromResult<IEnumerable<Models.Response.Question>>(questions);
            }
            catch (Exception e)
            {
                Logger.Error($"获取今日竞猜题目失败==》{userId:D} {activityId} ", e.InnerException ?? e);
                return OperationResult.FromError<IEnumerable<Models.Response.Question>>("-1", Resource.Invalid_SystemError);
            }
        }

        /// <summary>
        ///     设置此项是否被用户选择了
        /// </summary>
        /// <param name="question"></param>
        /// <param name="userChooseId"></param>
        private static void SetQuestionOptionUserAnswer(Models.Response.Question question, IEnumerable<QuestionnaireAnswerRecordModel> recordModels)
        {
            //选择的选项ID
            var recordIds = recordModels.Select(p => p.AnswerOptionID).ToList();

            Action<IList<QuestionOption>> func = null;
            func = options =>
            {
                options.ForEach(option =>
                {
                    if (option.SubOptions?.Count > 0)
                    {
                        func(option.SubOptions);

                        if (option.SubOptions.Any(x => x.IsUserAnswer == true))
                        {
                            option.IsUserAnswer = true;
                        }

                    }
                    if (option.IsUserAnswer != true)
                    {
                        option.IsUserAnswer = recordIds.Any(x => x == option.OptionID);
                    }
                });
            };

            func(question.QuestionOptionList);


        }



        /// <summary>
        ///     根据问卷ID 找对应的问题
        /// </summary>
        /// <param name="questionnaireID"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<Models.Response.Question>> SearchQuestion(long questionnaireID)
        {
            var func = new Func<Task<IEnumerable<Models.Response.Question>>>(async () =>
            {
                //AutoMapper初始化配置文件
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<QuestionOptionModel, QuestionOption>();
                    cfg.CreateMap<QuestionModel, Models.Response.Question>();
                });
                var mapper = config.CreateMapper();
                // 通过对应的问卷ID问题对象
                var questionArray = (await DalQuestion.GetQuestionList(questionnaireID, DateTime.Now))
                //结束时间倒叙
                .OrderByDescending(p => p.EndTime)
                //然后正序排序ID
                .ThenBy(p => p.QuestionID)
                .Select(mapper.Map<Models.Response.Question>)
                .Take(3)
                .ToList();

                var now = DateTime.Now;
                var questionnaireOptions = (await DalQuestionOption.GetQuestionOptionListByQuestionnaireId(questionnaireID))
                    .ToList();
                //找问题选项对象 和 子项
                questionArray.ForEach(p =>
                {
                    //问题是否过期
                    p.QuestionStatus = p.EndTime < now || (p.DeadLineTime != null && p.DeadLineTime < now) ? 1 : 0;

                    //当前问题的选项
                    var questionOptions = questionnaireOptions.Where(x => x.QuestionID == p.QuestionID)
                        .Select(mapper.Map<QuestionOption>)
                        .OrderBy(x => x.OptionID)
                        .ToList();

                    questionOptions = RecursiveQuestionOption(questionOptions);
                    p.QuestionOptionList = questionOptions;
                });
                return questionArray;

            });

            //读取缓存
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
            {
                var cacheResult =
                    await cacheClient.GetOrSetAsync($"ActivitySearchQuestion/{questionnaireID}", func, TimeSpan.FromMinutes(5));
                if (cacheResult.Success)
                {
                    return cacheResult.Value;
                }
                else
                {
                    Logger.Warn($" 获取redis缓存失败 {nameof(SearchQuestion)}");
                    return await func();
                }
            }

        }

        /// <summary>
        ///     选项重构
        /// </summary>
        private static List<QuestionOption> RecursiveQuestionOption(List<QuestionOption> questionOptionModels)
        {
            //获取没有父ID的option
            var topOptions = questionOptionModels
                .Where(p => p.QuestionParentID == 0)
                .OrderBy(p => p.OptionID)
                .ToList();
            foreach (var questionOptionModel in topOptions)
            {
                //选项子项 排列
                ChildQuestionOption(questionOptionModel, questionOptionModels);
            }
            return topOptions;
        }

        /// <summary>
        ///     拣选选项的子项(递归
        /// </summary>
        private static void ChildQuestionOption(QuestionOption questionOptionModel
            , List<QuestionOption> questionOptionModels)
        {
            questionOptionModel.SubOptions =
                questionOptionModels
                .Where(p => p.QuestionParentID == questionOptionModel.OptionID)
                .ToList();
            foreach (var questionOption in questionOptionModel?.SubOptions)
            {
                ChildQuestionOption(questionOption, questionOptionModels);
            }
        }


        /// <summary>
        ///     验证提交用户竞猜参数
        /// </summary>
        /// <param name="submitQuestionAnswerRequest"></param>
        private static string SubmitQuestionAnswerParameterValidate(SubmitQuestionAnswerRequest submitQuestionAnswerRequest)
        {
            //验证参数
            if (submitQuestionAnswerRequest.ActivityId == 0)
            {
                return (Resource.Invalid_Activity_NotFound);
            }
            if (submitQuestionAnswerRequest.UserId == Guid.Empty)
            {
                return (Resource.Invalid_Activity_Empty_UserId);
            }
            if (submitQuestionAnswerRequest.IntegralRuleID == Guid.Empty)
            {
                return (Resource.Invalid_Activity_Empty_IntegralRuleID);
            }
            if (submitQuestionAnswerRequest.OptionId == 0)
            {
                return (Resource.Invalid_Activity_Empty_OptionID);
            }
            return "";
        }

        /// <summary>
        ///     保存用户答题数据到数据库
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<SubmitActivityQuestionUserAnswerResponse>> SubmitQuestionUserAnswerAsync(SubmitActivityQuestionUserAnswerRequest request)
        {
            var result = new SubmitActivityQuestionUserAnswerResponse() { };
            try
            {
                using (var db = DbHelper.CreateDbHelper())
                {
                    //添加答题
                    var idRecord = await DalQuestionnaireAnswerRecord.SubmitQuestionnaire(db,
                        new QuestionnaireAnswerRecordModel()
                        {
                            UserID = request.UserId,
                            QuestionnaireID = request.QuestionnaireID,
                            QuestionnaireName = request?.QuestionnaireName,
                            QuestionID = request.QuestionID,
                            QuestionName = request.QuestionName,
                            QuestionType = request.QuestionType,
                            AnswerText = request?.AnswerText,
                            AnswerOptionID = request.AnswerOptionID,
                            AnswerOptionContent = request.AnswerOptionContent,
                            AnswerDate = DateTime.Now,
                            QuestionScore = 0,
                            ObjectID = 0,

                        });
                    if (idRecord <= 0)
                    {
                        Logger.Error($"{nameof(SubmitQuestionUserAnswerAsync)} {nameof(DalQuestionnaireAnswerRecord.SubmitQuestionnaire)}  保存失败 {request.UserId} {request.AnswerOptionID}");
                        return OperationResult.FromResult(result);
                    }
                    result.RecordId = idRecord;
                    //添加答题记录
                    var resultId = await DalQuestionnaireAnswerResult.SubmitQuestionnaireAnswerResult(db,
                        new QuestionnaireAnswerResultModel()
                        {
                            QuestionnaireAnswerID = idRecord,
                            AnswerResultStatus = 0,
                            UseIntegral = request.UseIntegral,
                            WinCouponCount = request.WinCouponCount,

                        });
                    if (resultId <= 0)
                    {
                        Logger.Error($"{nameof(SubmitQuestionUserAnswerAsync)}  {nameof(DalQuestionnaireAnswerResult.SubmitQuestionnaireAnswerResult)} 保存失败 {request.UserId} {request.AnswerOptionID}");
                        return OperationResult.FromResult(result);
                    }
                    result.ResultId = resultId;
                    result.IsOk = true;
                    return OperationResult.FromResult(result);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{nameof(SubmitQuestionUserAnswerAsync)} 失败", e);
                return OperationResult.FromError<SubmitActivityQuestionUserAnswerResponse>("-1", e.Message);
            }
        }

        /// <summary>
        ///     私有：保存用户数据到数据库中
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> SaveQuestionAnswerToDB(
                                                            SubmitQuestionAnswerRequest submitQuestionAnswerRequest
                                                               , ActivityModel activityModel
                                                               , QuestionModel questionModel
                                                               , QuestionnaireModel questionaireModel
                                                               , QuestionOptionModel questionOptionModel
                                                               , QuestionOptionModel questionOptionRootModel)
        {
            try
            {

                var result = await SubmitQuestionUserAnswerAsync(new SubmitActivityQuestionUserAnswerRequest()
                {
                    UserId = submitQuestionAnswerRequest.UserId,
                    QuestionnaireID = activityModel.QuestionnaireID,
                    QuestionnaireName = questionaireModel?.QuestionnaireName,
                    QuestionID = questionModel.QuestionID,
                    QuestionName = questionModel.QuestionTitle,
                    QuestionType = questionModel.QuestionType,
                    AnswerText = questionOptionRootModel?.OptionContent,
                    AnswerOptionID = questionOptionModel.OptionID,
                    AnswerOptionContent = questionOptionModel.OptionContent,
                    AnswerDate = DateTime.Now,
                    QuestionScore = 0,
                    ObjectID = 0,
                    UseIntegral = questionOptionModel.UseIntegral ?? 0,
                    WinCouponCount = questionOptionModel.WinCouponCount ?? 0
                });

                if (result.Result?.IsOk == true)
                {
                    var cacheKey = $"SaveQuestionAnswer/{submitQuestionAnswerRequest.ActivityId}/{submitQuestionAnswerRequest.UserId:N}/{questionModel.QuestionID}";
                    using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
                    {
                        await cacheClient.SetAsync(cacheKey, submitQuestionAnswerRequest.OptionId,
                            TimeSpan.FromHours(12));
                    }
                }

                return result.Result?.IsOk ?? false;
            }
            catch (Exception e)
            {
                Logger.Error($"{nameof(SaveQuestionAnswerToDB)} 失败", e);
                return false;
            }
        }

        /// <summary>
        ///     私有：保存用户数据到MQ中 异步执行
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> SaveQuestionAnswerToMQ(
                                                                SubmitQuestionAnswerRequest submitQuestionAnswerRequest
                                                                , ActivityModel activityModel
                                                                , QuestionModel questionModel
                                                                , QuestionnaireModel questionaireModel
                                                                , QuestionOptionModel questionOptionModel
                                                                , QuestionOptionModel questionOptionRootModel
                                                                )
        {

            try
            {
                Logger.Info($"{nameof(SaveQuestionAnswerToMQ)} {submitQuestionAnswerRequest.UserId} {submitQuestionAnswerRequest.OptionId} MQ消息 ");
                //先把数据放到redis中。
                //MQ是异步的，所以先放到redis中做判断
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
                {
                    var cacheKey = $"SaveQuestionAnswer/{submitQuestionAnswerRequest.ActivityId}/{submitQuestionAnswerRequest.UserId:N}/{questionModel.QuestionID}";
                    var cacheResult = await cacheClient
                        .ExistsAsync(cacheKey);
                    if (cacheResult != null && cacheResult.Success)
                    {
                        Logger.Info(
                            $"{nameof(SaveQuestionAnswerToMQ)} {submitQuestionAnswerRequest.UserId} {submitQuestionAnswerRequest.OptionId} MQ消息已经存在了 ");
                        return false;
                    }
                    else
                    {
                        RabbitMQProducer producer = null;
                        try
                        {
                            producer = RabbitMQClient.DefaultClient.CreateProducer("direct.defaultExchage");
                            producer?.Send("ActivityUserQuestionAnswer", new
                            {
                                submitQuestionAnswerRequest.UserId,
                                questionaireModel.QuestionnaireID,
                                questionaireModel.QuestionnaireName,
                                questionModel.QuestionID,
                                QuestionName = questionModel.QuestionTitle,
                                questionModel.QuestionType,
                                AnswerText = questionOptionRootModel?.OptionContent,
                                AnswerOptionID = questionOptionModel.OptionID,
                                AnswerOptionContent = questionOptionModel.OptionContent,
                                UseIntegral = questionOptionModel.UseIntegral ?? 0,
                                WinCouponCount = questionOptionModel.WinCouponCount ?? 0
                            });
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        finally
                        {
                            producer?.Dispose();
                        }

                        await cacheClient.SetAsync(cacheKey, submitQuestionAnswerRequest.OptionId, TimeSpan.FromHours(12));
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{nameof(SaveQuestionAnswerToDB)} 失败", e);
                return await Task.FromResult(false);
            }
        }

        /// <summary>
        ///     私有：提交用户回答
        /// </summary>
        /// <param name="submitQuestionAnswerRequest"></param>
        /// <param name="activityModel"></param>
        /// <param name="questionModel"></param>
        /// <param name="questionaireModel"></param>
        /// <param name="questionOptionModel"></param>
        /// <param name="questionOptionRootModel"></param>
        /// <returns></returns>
        private static async Task<OperationResult<bool>> SubmitQuestionAnswerToDB(
                                                             SubmitQuestionAnswerRequest submitQuestionAnswerRequest
                                                            , ActivityModel activityModel
                                                            , QuestionModel questionModel
                                                            , QuestionnaireModel questionaireModel
                                                            , QuestionOptionModel questionOptionModel
                                                            , QuestionOptionModel questionOptionRootModel)
        {
            //启动分布式锁
            using (var zklock = new ZooKeeperLock(SecurityHelper.Hash("/SubmitQuestionAnswer/" + submitQuestionAnswerRequest.UserId)))
            using (var configClient = new Service.Config.ConfigClient())
            {
                if (await zklock.WaitAsync(3000)) //如果锁释放 会立即执行，正常逻辑不会等待5秒钟
                {
                    //判断是否答题过
                    var answerRecordInfos = await DalQuestionnaireAnswerRecord.SearchQuestionnaireAnswerRecordInfo(false,
                        submitQuestionAnswerRequest.UserId, activityModel.QuestionnaireID, questionModel.QuestionID);
                    if (answerRecordInfos?.Count() > 0)
                    {
                        //重复答题
                        return OperationResult.FromError<bool>("-5", Resource.Invalid_Activity_RepeatAnswer);
                    }
                    {
                        try
                        {
                            //判断执行直接写库  还是 放到MQ中
                            if (configClient.GetOrSetRuntimeSwitch("ActivitySubmitQuestionAnswerToMQ")?.Result?.Value != true)
                            {
                                //DB
                                if (!await SaveQuestionAnswerToDB(submitQuestionAnswerRequest, activityModel,
                                    questionModel,
                                    questionaireModel, questionOptionModel, questionOptionRootModel))
                                {
                                    return OperationResult.FromError<bool>("-1", Resource.Invalid_SystemError);
                                }
                            }
                            else
                            {
                                //MQ
                                if (!await SaveQuestionAnswerToMQ(submitQuestionAnswerRequest, activityModel,
                                    questionModel,
                                    questionaireModel, questionOptionModel, questionOptionRootModel))
                                {
                                    return OperationResult.FromError<bool>("-1", Resource.Invalid_SystemError);
                                }
                            }
                            //扣积分
                            using (var client = new Tuhu.Service.Member.UserIntegralClient())
                            {
                                var userIntegralResult = client.UserIntegralChangeByUserID(
                                    submitQuestionAnswerRequest.UserId,
                                    new UserIntegralDetailModel()
                                    {
                                        TransactionIntegral = questionOptionModel.UseIntegral ?? 0,
                                        TransactionChannel = "H5",
                                        Versions = "1.0.0",
                                        TransactionRemark = activityModel.ActivityName,
                                        IntegralRuleID = submitQuestionAnswerRequest.IntegralRuleID
                                    }, null, 0);
                                //判断扣积分是否成功
                                if (!userIntegralResult.Success)
                                {
                                    Logger.Error($"答题活动扣积分失败！ {submitQuestionAnswerRequest.UserId} {submitQuestionAnswerRequest.OptionId}  {userIntegralResult.ErrorMessage} ");

                                    return OperationResult.FromError<bool>("-1", userIntegralResult.ErrorMessage);
                                }
                            }
                            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
                            {
                                await cacheClient.RemoveAsync($"/SearchQuestionnaireAnswerRecordInfo/{submitQuestionAnswerRequest.UserId:N}/{questionModel.QuestionnaireID}/{questionModel.QuestionID}");
                            }
                            return OperationResult.FromResult(true);
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
                else
                {
                    //获取锁失败
                    throw new Exception(Resource.Invalid_Activity_Locker);
                }
            }
        }


        /// <summary>
        ///     提交用户竞猜
        ///     -1 请重试  -2 参数异常   -3 时间已经截止    -4 用户积分不足  -5 已参与
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> SubmitQuestionAnswerAsync(SubmitQuestionAnswerRequest submitQuestionAnswerRequest)
        {
            try
            {
                //参数验证
                var validateMessage = SubmitQuestionAnswerParameterValidate(submitQuestionAnswerRequest);
                if (!string.IsNullOrWhiteSpace(validateMessage))
                {
                    return OperationResult.FromError<bool>("-2", validateMessage);
                }

                var activityId = submitQuestionAnswerRequest.ActivityId;

                //活动对象
                var activityModel = await GetActivityById(activityId);
                if (activityModel == null)
                {
                    //找不到活动
                    throw new Exception(Resource.Invalid_Activity_NotFound);
                }

                //找选项 =》 找问题  =》 找题目 =》 找问卷 =》 添加记录（用户答题，用户答题记录） =》 扣除积分
                var questionOptionModel =
                    await DalQuestionOption.GetQuestionOptionById(submitQuestionAnswerRequest.OptionId);

                if (questionOptionModel == null)
                {
                    //找不到选项
                    return OperationResult.FromError<bool>("-1", Resource.Invalid_Activity_NotFound_QuestionOption);
                }

                //父级选项
                var questionOptionRootModel =
                    (await DalQuestionOption.GetQuestionOptionListByQuestionId(questionOptionModel.QuestionID))?
                    .FirstOrDefault(p => p.QuestionParentID == 0 && p.OptionID == questionOptionModel.QuestionParentID);

                //获取问题
                var questionModel = await DalQuestion.GetQuestion(questionOptionModel.QuestionID);



                //判断是否为空 
                if (questionModel == null)
                {
                    //找不到问题
                    return OperationResult.FromError<bool>("-1", Resource.Invalid_Activity_NotFound_Question);
                }
                var now = DateTime.Now;

                //判断问题是否截止
                if (questionModel.DeadLineTime != null && questionModel.DeadLineTime < now)
                {
                    //答题时间不对 直接返回
                    return OperationResult.FromError<bool>("-3", Resource.Invalid_Activity_TimeEnd);
                }

                //判断问题的时间范围
                if (questionModel.EndTime < now || questionModel.StartTime > now)
                {
                    //答题时间不对 直接返回
                    return OperationResult.FromError<bool>("-3", Resource.Invalid_Activity_TimeEnd);
                }

                //判断问题是否下架
                //if (questionModel.QuestionConfirm != 2)
                //{
                //    //找不到问题
                //    return OperationResult.FromError<bool>("-1" ,Resource.Invalid_Activity_NotFound_Question);
                //}

                //问卷
                var questionaireModel =
                    await DalQuestionnaire.GetQuestionnaireInfoByPKID(activityModel.QuestionnaireID);

                //验证问卷id一致性
                if (questionaireModel.QuestionnaireID != questionModel.QuestionnaireID)
                {
                    return OperationResult.FromError<bool>("-1", Resource.Invalid_SystemError);
                }
                //获取用户积分
                using (var client = new Tuhu.Service.Member.UserIntegralClient())
                {
                    var userIntegral = await client.SelectUserIntegralByUserIDAsync(userId: submitQuestionAnswerRequest.UserId);
                    //用户积分小于选择问题的积分
                    if ((userIntegral?.Result?.Integral ?? 0) < questionOptionModel.UseIntegral)
                    {
                        return OperationResult.FromError<bool>("-4", Resource.Invalid_Activity_TimeEnd);
                    }
                }
                //提交
                return await SubmitQuestionAnswerToDB(submitQuestionAnswerRequest, activityModel, questionModel,
                     questionaireModel, questionOptionModel, questionOptionRootModel);
            }
            catch (Exception e)
            {
                Logger.Error($"提交用户竞猜失败 ==》{submitQuestionAnswerRequest.UserId:D} {submitQuestionAnswerRequest.OptionId} {submitQuestionAnswerRequest.IntegralRuleID} ", e.InnerException ?? e);
                return OperationResult.FromError<bool>("-1", Resource.Invalid_SystemError);
            }
        }


        /// <summary>
        ///     获取用户最后一次答题对应的选项
        /// </summary>
        /// <returns></returns>
        private static async Task<QuestionModel> GetUserLastAnswerOption(Guid userId, long questionnaireID)
        {
            //获取用户最后一次答题记录 并且 清算过的数据
            var userLastAnswer =
                await DalQuestionnaireAnswerRecord.GetLastQuestionnaireAnswerRecordInfo(true, userId,
                    questionnaireID, 1);
            if (userLastAnswer != null)
            {
                //获取对应选项
                var answerOption = await DalQuestionOption.GetQuestionOptionById(userLastAnswer.AnswerOptionID);
                if (answerOption != null)
                {
                    //获取对应问题
                    var question = await DalQuestion.GetQuestion(answerOption.QuestionID);
                    return question;
                }
            }
            return null;
        }

        /// <summary>
        ///     获得问题的历史
        /// </summary>
        /// <returns></returns>
        private static async Task<PagedModel<QuestionUserAnswerHistoryResponse>> SearchQuestionAnswerHistoryByQuestion(long questionaireId, Guid userId, int pageIndex, int pageSize)
        {

            //获取比当前截止日期小和已发布的题目
            var questionList = await DalQuestion.GetQuestionListPage(questionaireId, DateTime.Now, pageIndex, pageSize);

            var questionResult = await QuestionUserAnswerConstruction(userId, questionaireId, questionList);

            //转换回答历史
            var result = new PagedModel<QuestionUserAnswerHistoryResponse>(questionResult, questionList.Pager);
            return result;
        }

        /// <summary>
        ///     获得用户答题的历史
        /// </summary>
        /// <returns></returns>
        private static async Task<PagedModel<QuestionUserAnswerHistoryResponse>> SearchQuestionAnswerHistoryByUser(long questionaireId
            , Guid userId
            , int showFlag
            , int pageIndex
            , int pageSize)
        {
<<<<<<< HEAD


            //对应的问题的列表
            var questionList = await DalQuestion.GetQuestionListPage(questionaireId, userId, showFlag, pageIndex,
                pageSize);

            var questionResult = await QuestionUserAnswerConstruction(userId, questionaireId, questionList);

            //转换回答历史
            var result = new PagedModel<QuestionUserAnswerHistoryResponse>(questionResult, questionList.Pager);
            return result;

=======


            //获取用户答题数据 最后一次已经清算的答题
            var userLastAnswer =
                await DalQuestionnaireAnswerRecord.GetLastQuestionnaireAnswerRecordInfo(true, userId, questionaireId,
                    1);
            if (userLastAnswer != null)
            {
                //对应的选项
                var option = await DalQuestionOption.GetQuestionOptionById(userLastAnswer.AnswerOptionID);
                if (option != null)
                {
                    //对应的问题
                    var question = await DalQuestion.GetQuestion(option.QuestionID);
                    if (question != null)
                    {
                        //对应的问题的列表
                        var questionList = await DalQuestion.GetQuestionListPage(questionaireId, question.StartTime, question.EndTime, pageIndex,
                             pageSize);

                        var questionResult = await QuestionUserAnswerConstruction(userId, questionaireId, questionList);

                        //转换回答历史
                        var result = new PagedModel<QuestionUserAnswerHistoryResponse>(questionResult, questionList.Pager);
                        return result;
                    }
                }
            }

            return new PagedModel<QuestionUserAnswerHistoryResponse>(new List<QuestionUserAnswerHistoryResponse>());
>>>>>>> cef10ecfe95a62dbe27086896d29d0abab5da3d4
        }

        /// <summary>
        ///     问题对象拼装
        /// </summary>
        /// <returns></returns>
        private static async Task<IList<QuestionUserAnswerHistoryResponse>> QuestionUserAnswerConstruction(Guid userId, long questionaireId, IEnumerable<QuestionModel> questionModels)
        {
            //AutoMapper初始化配置文件
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<QuestionModel, QuestionUserAnswerHistoryResponse>()
                    .ForMember(dest => dest.QuestionDateTime, m => m.MapFrom(x => x.StartTime))
                    .ForMember(dest => dest.QuestionId, m => m.MapFrom(x => x.QuestionID));
            });
            var mapper = config.CreateMapper();
            //转换题目历史
            var userAnswerHistoryResponses = mapper.Map<IList<QuestionUserAnswerHistoryResponse>>(questionModels);

            //循环题目
            await userAnswerHistoryResponses.ForEachAsync(async p =>
            {
                //获取该问题用户的回答
                var answerRecordModel =
                (await DalQuestionnaireAnswerRecord.SearchQuestionnaireAnswerRecordInfo(true, userId,
                    questionaireId, p.QuestionId)).FirstOrDefault();
                if (answerRecordModel != null)
                {
                    p.RecordId = answerRecordModel.PKID;
                    //获取该问题用户回答结果
                    var questionnaireAnswerResult =
                        await DalQuestionnaireAnswerResult.GetQuestionnaireAnswerResult(true,
                            answerRecordModel.PKID);
                    if (questionnaireAnswerResult != null)
                    {
                        p.UseIntegral = questionnaireAnswerResult.UseIntegral;
                        p.AnswerResultStatus = questionnaireAnswerResult.AnswerResultStatus;
                        //赢得的时候
                        if (p.AnswerResultStatus == 1)
                        {
                            p.WinCouponCount = questionnaireAnswerResult.WinCouponCount;
                        }
                    }
                }
                else
                {
                    p.AnswerResultStatus = 3;
                }
            });
            return userAnswerHistoryResponses;
        }


        /// <summary>
        ///     返回用户答题历史
        /// </summary>
        /// <param name="searchQuestionAnswerHistoryRequest"></param>
        /// <returns></returns> 
        public static async Task<OperationResult<PagedModel<QuestionUserAnswerHistoryResponse>>> SearchQuestionAnswerHistoryByUserIdAsync(SearchQuestionAnswerHistoryRequest searchQuestionAnswerHistoryRequest)
        {
            var activityId = searchQuestionAnswerHistoryRequest.ActivityId;
            try
            {
                //参数验证
                if (searchQuestionAnswerHistoryRequest.UserId == Guid.Empty)
                {
                    return OperationResult.FromError<PagedModel<QuestionUserAnswerHistoryResponse>>("-1", Resource.Invalid_SystemError);
                }

                
                //通过UserId 和 问卷ID获得用户答题记录和答题结果
                var userId = searchQuestionAnswerHistoryRequest.UserId;
                var pageIndex = searchQuestionAnswerHistoryRequest.PageIndex;
                var pageSize = searchQuestionAnswerHistoryRequest.PageSize;
                var showFlag = searchQuestionAnswerHistoryRequest.ShowFlag;


                //活动对象
                var activityModel = await GetActivityById(activityId);
                if (activityModel == null)
                {
                    //找不到活动
                    throw new Exception(Resource.Invalid_Activity_NotFound);
                }
                //查找活动对应的 问卷ID
                var questionaireId = activityModel.QuestionnaireID;

                //缓存
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
                {
                   var cacheResult = await cacheClient.GetOrSetAsync(
                        $"/SearchQuestionAnswerHistoryByUserIdAsync/{activityId}/{userId:N}/{questionaireId}/{showFlag}/{pageIndex}/{pageSize}",
                        async () =>
                        {
                            //用户回答历史
                            var result = await SearchQuestionAnswerHistoryByUser(questionaireId, userId, showFlag, pageIndex, pageSize);
                            return result;
                        },TimeSpan.FromSeconds(30));
                    return OperationResult.FromResult(cacheResult?.Value);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"返回用户答题历史失败==》 {activityId} ", e.InnerException ?? e);
                return OperationResult.FromError<PagedModel<QuestionUserAnswerHistoryResponse>>("-1", Resource.Invalid_SystemError);
            }
        }


        /// <summary>
        ///     返回用户胜利次数和胜利称号
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<ActivityVictoryInfoResponse>> GetVictoryInfoAsync(Guid userId, long activityId)
        {
            try
            {
                //缓存
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
                {
                    var cacheResult = await cacheClient.GetOrSetAsync($"/GetVictoryInfoAsync/{activityId}/{userId:N}", async () =>
                    {
                        var result = new ActivityVictoryInfoResponse();
                        //活动对象
                        var activityModel = await GetActivityById(activityId);
                        if (activityModel == null)
                        {
                            //找不到活动
                            throw new Exception(Resource.Invalid_Activity_NotFound);
                        }
                        var levels = await SearchActivityLevel(activityId);

                        //获取胜利次数
                        var count = await DalQuestionnaireAnswerResult.GetQuestionnaireAnswerUserWinCount(userId,
                            activityModel.QuestionnaireID);

                        //获取用户兑换券
                        var userCoupon = await DalActivity.GetActivityCouponByUserId(userId, activityId);
                        var couponSum = userCoupon?.CouponSum ?? 0;
                        result.VictoryNumber = count;
                        result.VictoryTitle = levels.FirstOrDefault(p => p.StartCount <= couponSum && p.EndCount >= couponSum)?.LevelName;
                        return result;
                    }, TimeSpan.FromMinutes(1));
                    return OperationResult.FromResult(cacheResult?.Value);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"返回用户胜利次数和胜利称号==》{userId:D} {activityId} ", e.InnerException ?? e);
                return OperationResult.FromError<ActivityVictoryInfoResponse>("-1", Resource.Invalid_SystemError);
            }
        }

        /// <summary>
        ///     读取用户数据 有缓存优先缓存
        /// </summary>
        /// <returns></returns>
        private static async Task<OperationResult<UserObjectModel>> FetchUserByUserIdByCacheAsync(Guid userId)
        {
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
            {
                var cacheKey = $"/Activity/FetchUserByUserIdByCacheAsync/{userId:N}";
                var cacheResult = await cacheClient.GetOrSetAsync(cacheKey, async () =>
                {
                    using (var userClient = new UserClient())
                    {
                        //获取用户数据
                        var userInfo = await userClient.FetchUserByUserIdAsync(userId.ToString());
                        return userInfo;
                    }
                }, TimeSpan.FromDays(1));
                return cacheResult?.Value;
            }
        }


        /// <summary>
        ///     返回活动兑换券排行排名
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<OperationResult<PagedModel<ActivityCouponRankResponse>>> SearchCouponRankAsync(
            long activityId, int pageIndex = 1, int pageSize = 20)
        {
            try
            {
                var func = new Func<Task<OperationResult<PagedModel<ActivityCouponRankResponse>>>>(async () =>
                {
                    var result = new PagedModel<ActivityCouponRankResponse>();

                    //AutoMapper初始化配置文件
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<ActivityCouponModel, ActivityCouponRankResponse>();
                    });
                    var mapper = config.CreateMapper();


                    //获取排名
                    var rankList = (await DalActivity.SearchActivityCouponRank(activityId, pageIndex, pageSize));

                    var rankResponse = mapper.Map<IList<ActivityCouponRankResponse>>(rankList.Source);

                    var levels = await SearchActivityLevel(activityId);

                    using (var userClient = new UserClient())
                    {
                        //循环 赋值
                        await rankResponse.ForEachAsync(async p =>
                        {
                            p.RankDate = DateTime.Now;
                            p.Title = levels?.FirstOrDefault(x => x.StartCount <= p.CouponSum && x.EndCount >= p.CouponSum)?.LevelName;
                            //获取用户数据
                            var userInfo = await FetchUserByUserIdByCacheAsync(p.UserId);
                            if (userInfo != null && userInfo.Success && userInfo.Result != null)
                            {
                                p.UserImgUrl = ImageHelper.GetImageUrl(userInfo.Result.HeadImage);

                                p.NickName = userInfo.Result.Nickname?.Trim() ?? "";
                                //加密 如果11位 那么隐藏中间 四位
                                if (p.NickName.Length == 11)
                                {
                                    p.NickName = p.NickName.Substring(0, 3) + "****" + p.NickName.Substring(7, 4);
                                }
                            }

                        });
                    }
                    result.Pager = rankList.Pager;
                    result.Source = rankResponse;

                    return OperationResult.FromResult(result);
                });

                //读取缓存
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
                {
                    var cacheResult =
                        await cacheClient.GetOrSetAsync($"SearchCouponRank/{activityId}/{pageIndex}/{pageSize}", func, TimeSpan.FromMinutes(30));
                    if (cacheResult.Success)
                    {
                        return cacheResult.Value;
                    }
                    else
                    {
                        Logger.Warn($" 获取redis缓存失败 {nameof(SearchQuestion)}");
                        return await func();
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Error($"返回活动兑换券排行排名==》{activityId} {pageIndex} {pageSize} ", e.InnerException ?? e);
                return OperationResult.FromError<PagedModel<ActivityCouponRankResponse>>("-1", Resource.Invalid_SystemError);
            }
        }

        /// <summary>
        ///     返回用户的兑换券排名情况
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        public static async Task<OperationResult<ActivityCouponRankResponse>> GetUserCouponRankAsync(Guid userId, long activityId)
        {
            try
            {
                //读取缓存
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
                {
                   var cacheResult = await cacheClient.GetOrSetAsync($"/GetUserCouponRankAsync/{activityId}/{userId:N}",async () =>
                    {
                        //AutoMapper初始化配置文件
                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<ActivityCouponModel, ActivityCouponRankResponse>();
                        });
                        var mapper = config.CreateMapper();

                        //var levels = await SearchActivityLevel(activityId);
                        var rankSetting = await SearchActivityRankSetting(activityId);

                        //获取用户兑换券的数据
                        var userCoupon = await DalActivity.GetActivityCouponByUserId(userId, activityId);
                        //如果没有此用户兑换券的数据
                        if (userCoupon == null || userCoupon?.CouponSum == 0)
                        {
                            return OperationResult.FromResult(new ActivityCouponRankResponse()
                            {
                                CouponSum = 0,
                                RankStatus = 1,
                                //Title = levels?.FirstOrDefault(p => p.StartCount <= 0 && p.EndCount >= 0)?.LevelName
                            });
                        }

                        //返回对象
                        var result = mapper.Map<ActivityCouponRankResponse>(userCoupon);
                        //获取当前用户的排名
                        var userRank = await DalActivity.GetUserCouponRank(userId, activityId, userCoupon?.CouponSum ?? 0);

                        //计算
                        result.RankDate = DateTime.Now;
                        result.Rank = userRank;
                        if (rankSetting != null)
                        {
                            if (userRank <= rankSetting.RankHead)
                            {
                                result.RankStatus = 2;
                            }
                            else if (userRank <= rankSetting.RankMiddle)
                            {
                                result.RankStatus = 0;
                                //获取第三名数据
                                var st3UserRank = await DalActivity.GetCouponByRank(3, activityId);
                                if (st3UserRank?.CouponSum > userCoupon?.CouponSum)
                                {
                                    result.To3rdCouponCount = (st3UserRank?.CouponSum - userCoupon?.CouponSum) ?? 0;
                                }
                            }
                            else
                            {
                                result.RankStatus = 1;
                            }
                        }
                        else
                        {
                            result.RankStatus = 1;
                        }

                        return OperationResult.FromResult(result);
                    }, TimeSpan.FromMinutes(30));

                    return cacheResult?.Value;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"返回用户的兑换券排名情况==》{userId:D} {activityId} ", e.InnerException ?? e);
                return OperationResult.FromError<ActivityCouponRankResponse>("-1", Resource.Invalid_SystemError);
            }
        }

        /// <summary>
        ///     获取活动兑换物记录 只取ID
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<ActivityPrizeOrderDetailModel>> SearchPrizeOrderDetailID(Guid userId, long activityId)
        {
            //读取缓存
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
            {
               var result = await cacheClient.GetOrSetAsync($"/SearchPrizeOrderDetailID/{activityId}/{userId:N}", async () =>
                {
                    var userPrizeListIds = await DalActivity.SearchPrizeOrderDetailID(userId, activityId);
                    return userPrizeListIds;
                }, TimeSpan.FromMinutes(30));
                return result?.Value;
            }
        }

        /// <summary>
        ///     删除活动兑换物记录缓存
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        private static async Task RemovePrizeOrderDetailIDCache(Guid userId, long activityId)
        {
            //读取缓存
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
            {
                await cacheClient.RemoveAsync($"/SearchPrizeOrderDetailID/{activityId}/{userId:N}");
            }
        }

        /// <summary>
        ///     排序和分页
        /// </summary>
        /// <param name="userCouponCount"></param>
        /// <param name="userId"></param>
        /// <param name="sortByUserCouponCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private static async Task<PagedModel<ActivityPrizeResponse>> SearchPrizeListSortPaged(IList<ActivityPrizeResponse> prizeList, long activityId, int? userCouponCount, Guid? userId, bool sortByUserCouponCount, int pageIndex = 1, int pageSize = 20)
        {
            var result = new PagedModel<ActivityPrizeResponse>();
            IEnumerable<ActivityPrizeResponse> prizeStream;
            //获取用户已兑换的奖品id
            var userPrizeListIds = await SearchPrizeOrderDetailID(userId ?? Guid.Empty, activityId);
            #region 排序
            if (userCouponCount != null && sortByUserCouponCount)
            {
                prizeStream = prizeList
                    //可兑换
                    .OrderByDescending(p => !p.IsDisableSale && p.CouponCount <= userCouponCount && p.Stock > 0 && !userPrizeListIds.Any(x => x.ActivityPrizeId == p.PKID))
                    //不可销售最上
                    .ThenByDescending(p => p.IsDisableSale)
                    //不足以兑换
                    .ThenByDescending(p => p.CouponCount > userCouponCount && p.Stock > 0 && !userPrizeListIds.Any(x => x.ActivityPrizeId == p.PKID))
                    //买过的最上
                    .ThenByDescending(p => userPrizeListIds.Any(x => x.ActivityPrizeId == p.PKID))
                    //库存大于0的最上
                    .ThenByDescending(p => p.Stock > 0)
                    //按照价值排序
                    .ThenByDescending(p => p.CouponCount);

            }
            else if (userCouponCount != null)
            {
                prizeStream = prizeList
                    //不可销售最上
                    .OrderByDescending(p => p.IsDisableSale)
                    //可兑换
                    .ThenByDescending(p => !p.IsDisableSale && p.CouponCount <= userCouponCount && p.Stock > 0 && !userPrizeListIds.Any(x => x.ActivityPrizeId == p.PKID))
                    //不足以兑换
                    .ThenByDescending(p => p.CouponCount > userCouponCount && p.Stock > 0 && !userPrizeListIds.Any(x => x.ActivityPrizeId == p.PKID))
                    //买过的最上
                    .ThenByDescending(p => userPrizeListIds.Any(x => x.ActivityPrizeId == p.PKID))
                    //库存大于0的最上
                    .ThenByDescending(p => p.Stock > 0)
                    //按照价值排序
                    .ThenByDescending(p => p.CouponCount);

            }
            else
            {
                prizeStream = prizeList
                    //不可销售最上
                    .OrderByDescending(p => p.IsDisableSale)
                    //库存大于0的最上
                    .ThenByDescending(p => p.Stock > 0)
                    //按照价值排序
                    .ThenByDescending(p => p.CouponCount);
            }
            #endregion
            var count = prizeList.Count;
            #region 分页 
            prizeStream = prizeStream
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
            #endregion
            result.Source = prizeStream.ToList();
            result.Source.ForEach(p =>
            {
                if (p.IsDisableSale)
                {
                    //判断是否可以售卖
                    p.ActivityPrizeStatus = 3;
                }
                else if (p.Stock <= 0)
                {
                    //判断库存
                    p.ActivityPrizeStatus = 2;
                }
                //判断用户是否已经兑换
                if (userPrizeListIds.Any(x => x.ActivityPrizeId == p.PKID))
                {
                    {
                        p.ActivityPrizeStatus = 1;
                    }
                }
            });


            result.Pager = new PagerModel(pageIndex, pageSize)
            {
                Total = count
            };
            return result;
        }


        /// <summary>
        ///     同步兑换物库存
        /// </summary>
        /// <param name="prizeList"></param>
        private static async Task SyncPrizeListStock(IList<ActivityPrizeResponse> prizeList, int ignoreCache = 0)
        {
            var cacheKey = $"SearchPrizeListStock/";
            var cacheTime = TimeSpan.FromMinutes(5);


            //读取缓存
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
            {
                await prizeList?.ForEachAsync(async prize =>
                {
                    //默认
                    if (ignoreCache == 0)
                    {
                        var cacheResult = await cacheClient.GetOrSetAsync($"{cacheKey}{prize.PKID}", async () =>
                        {
                            //获取库存
                            var prizeDto = await DalActivity.GetActivityPrizeById(false, prize.PKID);

                            return prizeDto?.Stock ?? 0;
                        }, cacheTime);
                        prize.Stock = cacheResult?.Value ?? 0;
                    }
                    //读取数据库
                    else if (ignoreCache == 1)
                    {
                        //获取库存
                        var prizeDto = await DalActivity.GetActivityPrizeById(false, prize.PKID);
                        prize.Stock = prizeDto?.Stock ?? 0;
                    }
                    //读取数据库 并 刷新缓存
                    else if (ignoreCache == 2)
                    {
                        //获取库存
                        var prizeDto = await DalActivity.GetActivityPrizeById(false, prize.PKID);
                        prize.Stock = prizeDto?.Stock ?? 0;
                        await cacheClient.SetAsync($"{cacheKey}{prize.PKID}", prize.Stock, cacheTime);
                    }
                });
            }
        }

        /// <summary>
        ///     删除兑换物缓存
        /// </summary>
        /// <returns></returns>
        private static async Task RemovePrizeStockCache(long prizeId)
        {
            //读取缓存
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
            {
                await cacheClient.RemoveAsync($"SearchPrizeListStock/{prizeId}");
            }
        }

        /// <summary>
        ///     通过数据库读取奖品列表
        /// </summary>
        /// <param name="prizeId"></param>
        /// <returns></returns>
        private static async Task<IList<ActivityPrizeResponse>> SearchPrizeListFromDB(long activityId)
        {

            //AutoMapper初始化配置文件
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ActivityPrizeModel, ActivityPrizeResponse>();
            });
            var mapper = config.CreateMapper();

            //兑换物列表
            var prizeListDto = await DalActivity.SearchPrizeList(activityId);

            //转换对象
            var prizeResult = mapper.Map<IList<ActivityPrizeResponse>>(prizeListDto);


            return prizeResult;
        }

        /// <summary>
        ///     查询奖品列表
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userCouponCount"></param>
        /// <param name="userId"></param>
        /// <param name="sortByUserCouponCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private static async Task<PagedModel<ActivityPrizeResponse>> SearchPrizeList(long activityId, int? userCouponCount, Guid? userId, bool sortByUserCouponCount, int pageIndex = 1, int pageSize = 20, int ignoreCache = 0)
        {
            //获取兑换物列表
            var funcsearchprize = new Func<Task<IList<ActivityPrizeResponse>>>(async delegate
            {
                var cacheKey = $"SearchPrizeList/{activityId}";
                var cacheTime = TimeSpan.FromMinutes(5);
                //默认
                if (ignoreCache == 0)
                {
                    //读取缓存
                    using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
                    {
                        var cacheResult = await cacheClient.GetOrSetAsync(cacheKey,
                            async () =>
                            {
                                return await SearchPrizeListFromDB(activityId);
                            }, cacheTime);
                        return cacheResult?.Value;
                    }
                }
                //读取数据库
                else if (ignoreCache == 1)
                {
                    return await SearchPrizeListFromDB(activityId);
                }
                //读取数据库 并 刷新缓存
                else if (ignoreCache == 2)
                {
                    var result = await SearchPrizeListFromDB(activityId);
                    using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
                    {
                        await cacheClient.SetAsync(cacheKey, result, cacheTime);
                    }
                    return result;
                }
                return null;
            });

            //获取兑换物列表
            var prizeList = await funcsearchprize();

            //兑换物库存
            await SyncPrizeListStock(prizeList, ignoreCache);

            //分页和排序
            var prizePaged = await SearchPrizeListSortPaged(prizeList, activityId, userCouponCount, userId,
                sortByUserCouponCount, pageIndex, pageSize);

            return prizePaged;
        }

        /// <summary>
        ///     获取用户兑换券
        /// </summary>
        /// <returns></returns>
        private static async Task<ActivityCouponModel> GetUserActivityCoupon(Guid userId,long activityId)
        {
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
            {
                var cacheResult = await cacheClient.GetOrSetAsync($"/GetUserActivityCoupon/{activityId}/{userId:N}", async () =>
                {
                    var userCoupon = await DalActivity.GetActivityCouponByUserId(userId, activityId);
                    return userCoupon;
                }, TimeSpan.FromMinutes(2));
                return cacheResult?.Value;
            }
        }


        /// <summary>
        ///     返回兑换物列表
        /// </summary>
        /// <param name="searchPrizeListRequest">请求对象</param>
        /// <returns></returns>
        public static async Task<OperationResult<PagedModel<ActivityPrizeResponse>>> SearchPrizeListAsync(SearchPrizeListRequest searchPrizeListRequest)
        {
            var result = new PagedModel<ActivityPrizeResponse>();
            var activityId = searchPrizeListRequest.ActivityId;
            var userId = searchPrizeListRequest.UserId;
            var pageIndex = searchPrizeListRequest.PageIndex;
            var pageSize = searchPrizeListRequest.PageSize;
            var showCanPay = searchPrizeListRequest.ShowCanPay;
            var ignoreCache = searchPrizeListRequest.IgnoreCache;
            try
            {
                //用户当前的兑换券
                var userCoupon = await GetUserActivityCoupon(userId, activityId);

                //兑换物列表
                var prizeList = await SearchPrizeList(activityId, userCoupon?.CouponCount, userId,
                    showCanPay ?? false, pageIndex, pageSize, ignoreCache);

                result = prizeList;

                return OperationResult.FromResult(result);
            }
            catch (Exception e)
            {
                Logger.Error($"返回兑换物列表==》{userId:D} {activityId} {pageIndex} {pageSize}  ", e.InnerException ?? e);
                return OperationResult.FromError<PagedModel<ActivityPrizeResponse>>("-1", Resource.Invalid_SystemError);
            }
        }

        /// <summary>
        ///     用户兑换奖品参数验证
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="prizeId"></param>
        /// <param name="activityId"></param>
        /// <param name="userCoupon"></param>
        /// <param name="prize"></param>
        /// <returns></returns>
        private static async Task<Tuple<string, string>> UserRedeemPrizeValidate(Guid userId, long prizeId, long activityId, ActivityCouponModel userCoupon, ActivityPrizeModel prize)
        {

            if (prize == null)
            {
                return new Tuple<string, string>("-1", Resource.Invalid_Activity_PrizeError);
            }
            //判断是否是一个活动
            if (prize.ActivityId != activityId)
            {
                return new Tuple<string, string>("-1", Resource.Activity_Error);
            }
            //判断兑换券是否足够
            if ((userCoupon?.CouponCount ?? 0) < prize.CouponCount)
            {
                return new Tuple<string, string>("-2", Resource.Invalid_Acitivty_CouponInsufficient);
            }
            //库存
            if (prize.Stock <= 0)
            {
                return new Tuple<string, string>("-3", Resource.Invalid_Activity_OutOfStock);

            }
            //判断是否已经下架
            if (prize.OnSale == 0 || prize.IsDeleted)
            {
                //下架了
                return new Tuple<string, string>("-4", Resource.Invalid_Activity_NoOnSale);
            }
            //判断是否只是看的
            if (prize.IsDisableSale)
            {
                return new Tuple<string, string>("-1", Resource.Invalid_Activity_PrizeReadOnly);
            }
            //判断兑换品是否有优惠券ID
            if (prize.GetRuleId == Guid.Empty)
            {
                return new Tuple<string, string>("-1", Resource.Invalid_Activity_PrizeError);
            }
            //判断当前用户是否已经兑换过了
            var isUserPrizeExists =
                await DalActivity.GetExistsActivityPrizeOrderDetail(false, userId, prizeId, activityId);
            if (isUserPrizeExists)
            {
                return new Tuple<string, string>("-5", Resource.Invalid_Activity_UserPrizeExists);
            }
            return null;
        }

        /// <summary>
        ///     用户兑换奖品 添加到数据库
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private static async Task<bool> UserRedeemPrizesToDB(Guid userId, long prizeId, long activityId, ActivityModel activityModel, ActivityCouponModel userCoupon, ActivityPrizeModel prize)
        {
            using (var promotionClient = new PromotionClient())
            using (var db = DbHelper.CreateDbHelper())
            {
                //开启事务
                await db.BeginTransactionAsync(IsolationLevel.RepeatableRead);
                var coupon = prize.CouponCount;

                //添加用户兑换记录
                var orderDetailResult =
                    await DalActivity.InsertActivityPrizeOrderDetail(db,
                        new ActivityPrizeOrderDetailModel()
                        {
                            ActivityId = activityId,
                            ActivityName = activityModel?.ActivityName,
                            ActivityPrizeId = prizeId,
                            ActivityPrizeName = prize.ActivityPrizeName,
                            ActivityPrizePicUrl = prize.PicUrl,
                            CouponCount = coupon,
                            UserId = userId
                        });
                //消耗库存
                var stockResult = await DalActivity.ActivityPrizeConsumptionStock(db, prizeId, 1, prize.Timestamp);


                //操作用户兑换券
                var couponResult = await ModifyActivityCoupon(db, userId, activityId, -coupon, "兑换：" + prize.ActivityPrizeName);

                //判断关键任务是否执行成功
                if (!stockResult || couponResult < 0)
                {
                    db.Rollback();
                    Logger.Info($"{userId} 用户兑换活动商品失败 {stockResult} {couponResult} ");
                    return false;
                }

                Logger.Info($"用户兑换活动商品 {userId} {prize.ActivityPrizeName}  {prize.PKID} ");
                //赠送给用户抵用券
                var promotionResult = await promotionClient.CreatePromotionNewAsync(new CreatePromotionModel()
                {
                    UserID = userId,
                    GetRuleGUID = prize.GetRuleId,
                    Author = userId.ToString(),
                    Channel = activityModel?.ActivityName,
                });
                Logger.Info($"用户兑换活动商品 抵用券结果 {userId} {promotionResult.ErrorCode} {promotionResult.ErrorMessage} {promotionResult?.Result?.ErrorMessage} ");

                if (promotionResult?.Success == true && promotionResult?.Result?.IsSuccess == true)
                {
                    try
                    {
                        //提交事务
                        db.Commit();
                        await RemovePrizeStockCache(prizeId);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Logger.Error("提交用户兑换奖品异常！db.Commit失败！", e);
                        throw e;
                    }
                }
                else
                {
                    //记录下消息
                    Logger.Info($" {userId} 用户兑换活动商品失败 {promotionResult?.Result?.ErrorMessage}  ");
                }
                db.Rollback();
                return false;
            }
        }

        /// <summary>
        ///     用户兑换奖品
        ///     异常代码：-1 系统异常（请重试） , -2 兑换卡不足  -3 库存不足  -4 已经下架  -5 已经兑换  -6 兑换时间已经截止不能兑换
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="prizeId"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> UserRedeemPrizesAsync(Guid userId, long prizeId, long activityId)
        {
            try
            {
                var activityModel = await GetActivityById(activityId);
                if (activityModel == null)
                {
                    //找不到活动
                    throw new Exception(Resource.Invalid_Activity_NotFound);
                }
                var now = DateTime.Now;
                if (activityModel.SecEndTime < now)
                {
                    return OperationResult.FromError<bool>("-6", Resource.Invalid_Activity_PrizeEndTime);
                }


                //启动分布式锁
                using (var zklock =
                    new ZooKeeperLock(SecurityHelper.Hash("/UserRedeemPrizes/" + prizeId)))
                {
                    if (await zklock.WaitAsync(5000)) //如果锁释放 会立即执行，正常逻辑不会等待5秒钟
                    {
                        //获取用户兑换券
                        var userCoupon = await DalActivity.GetActivityCouponByUserId(userId, activityId);

                        //获取兑换品
                        var prize = await DalActivity.GetActivityPrizeById(false, prizeId);

                        //验证逻辑
                        var validateResult =
                            await UserRedeemPrizeValidate(userId, prizeId, activityId, userCoupon, prize);
                        if (validateResult != null)
                        {
                            //库存不足的时候 尝试删除库存缓存
                            if (validateResult.Item1 == "-3")
                            {
                                await RemovePrizeStockCache(prizeId);
                            }

                            return OperationResult.FromError<bool>(validateResult.Item1, validateResult.Item2);
                        }

                        //提交
                        var isOk = await UserRedeemPrizesToDB(userId, prizeId, activityId, activityModel, userCoupon,
                            prize);

                        if (isOk)
                        {
                            //成功 删除用户缓存
                            await RemovePrizeOrderDetailIDCache(userId, activityId);

                            return OperationResult.FromResult(true);
                        }
                    }
                    else
                    {
                        Logger.Info($"用户兑换奖品获取不了锁 {userId} {prizeId} {activityId}  ");
                    }
                }
                return OperationResult.FromResult(false);
            }
            catch (Exception e)
            {
                Logger.Error($"用户兑换奖品失败 ==》{userId} {prizeId} {activityId} ", e.InnerException ?? e);
                return OperationResult.FromError<bool>("-1", Resource.Invalid_SystemError);
            }
        }

        /// <summary>
        ///     用户已兑换商品列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="activityId">活动ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        public static async Task<OperationResult<PagedModel<ActivityPrizeOrderDetailResponse>>>
            SearchPrizeOrderDetailListByUserIdAsync(Guid userId, long activityId, int pageIndex = 1, int pageSize = 20)
        {
            try
            {
                //AutoMapper初始化配置文件
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ActivityPrizeOrderDetailModel, ActivityPrizeOrderDetailResponse>();
                });
                var mapper = config.CreateMapper();

                var result = new PagedModel<ActivityPrizeOrderDetailResponse>();

                //查询  分页
                var prizeOrderDetailPaged = await DalActivity.SearchPrizeOrderDetail(userId, activityId, pageIndex, pageSize);

                //转换对象
                var prizeOrderDetailResponseArray =
                    mapper.Map<IList<ActivityPrizeOrderDetailResponse>>(prizeOrderDetailPaged.Source);

                result.Source = prizeOrderDetailResponseArray;
                result.Pager = prizeOrderDetailPaged.Pager;

                return OperationResult.FromResult(result);
            }
            catch (Exception e)
            {
                Logger.Error($"返回用户已兑换商品列表失败 ==》{userId:D} {activityId} {pageIndex} {pageSize} ", e.InnerException ?? e);
                return OperationResult.FromError<PagedModel<ActivityPrizeOrderDetailResponse>>("-1", Resource.Invalid_SystemError);
            }
        }


        /// <summary>
        ///     活动分享赠送积分 
        /// </summary>
        /// <param name="shareDetailRequest">分享的对象</param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> ActivityShareAsync(ActivityShareDetailRequest shareDetailRequest)
        {
            var activityId = shareDetailRequest.ActivityId;
            var userId = shareDetailRequest.UserId;
            var shareName = shareDetailRequest.ShareName;
            var integralRuleID = shareDetailRequest.IntegralRuleID;
            ZooKeeperLock zklock = null;
            BaseDbHelper db = null;
            try
            {
                var activityModel = await GetActivityById(activityId);
                if (activityModel == null)
                {
                    //找不到活动
                    throw new Exception(Resource.Invalid_Activity_NotFound);
                }

                var now = DateTime.Now;
                //分布式锁
                zklock = new ZooKeeperLock(SecurityHelper.Hash($"/ActivityShare/{activityId}/{userId:N}"));
                db = DbHelper.CreateDbHelper();
                if (await zklock.WaitAsync(5000)) //如果锁释放 会立即执行，正常逻辑不会等待5秒钟
                {
                    //判断用户今天是否分享过了
                    var userShareDetail = await DalActivity.SearchActivityShareDetail(activityId, userId, now);
                    if (!userShareDetail.Any())
                    {
                        //判断活动是否到期
                        if (activityModel.StartTime >= now || activityModel.EndTime <= now)
                        {
                            //活动尚未开始
                            return OperationResult.FromError<bool>("-77", Resource.Invalid_Activity_DateError);
                        }

                        await db.BeginTransactionAsync();
                        //没有分享就增加积分
                        var insertResult = await DalActivity.InsertActivityShareDetail(db,
                            new ActivityShareDetailModel()
                            {
                                ActivityId = activityId,
                                UserId = userId,
                                ShareName = shareName,
                                ShareTime = now
                            });
                        //赠送积分
                        using (var client = new Tuhu.Service.Member.UserIntegralClient())
                        {
                            var userIntegralResult = client.UserIntegralChangeByUserID(
                                userId,
                                new UserIntegralDetailModel()
                                {
                                    TransactionIntegral = activityModel.ShareIntegral,
                                    TransactionChannel = "H5",
                                    Versions = "1.0.0",
                                    TransactionRemark = activityModel.ActivityName,
                                    IntegralRuleID = shareDetailRequest.IntegralRuleID
                                }, null, 0);
                            //判断扣积分是否成功
                            if (!userIntegralResult.Success)
                            {
                                db.Rollback();
                                return OperationResult.FromError<bool>("-1", userIntegralResult.ErrorMessage);
                            }
                            else
                            {
                                try
                                {
                                    //提交事务
                                    db.Commit();
                                    return OperationResult.FromResult(true);
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("提交活动分享赠送积分异常！db.Commit失败！", e);
                                    throw e;
                                }
                            }
                        }
                    }
                    else
                    {
                        //返回已经分享提示
                        return OperationResult.FromError<bool>("-2", Resource.Invalid_Activity_AlreadyShare);
                    }
                }

                return OperationResult.FromResult(false);

            }
            catch (Exception e)
            {
                Logger.Error($"活动分享赠送积分失败 ==》{userId:D} {activityId} {shareName} {integralRuleID:N} ", e.InnerException ?? e);
                return OperationResult.FromError<bool>("-1", Resource.Invalid_SystemError);
            }
            finally
            {
                zklock?.Dispose();
                db?.Dispose();
            }

        }


        /// <summary>
        ///     今日是否已经分享了
        ///     true = 今日已经分享
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> ActivityTodayAlreadyShareAsync(Guid userId, long activityId)
        {
            var now = DateTime.Now;
            //判断用户今天是否分享过了
            var userShareDetail = await DalActivity.SearchActivityShareDetail(activityId, userId, now);
            if (userShareDetail.Any())
            {
                return OperationResult.FromResult(true);
            }
            return OperationResult.FromResult(false);
        }





        /// <summary>
        ///     修改或者增加用户兑换券  返回主键
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityId"></param>
        /// <param name="couponCount"></param>
        /// <param name="couponName"></param>
        /// <returns></returns>
        public static async Task<OperationResult<long>> ModifyActivityCouponAsync(Guid userId, long activityId, int couponCount, string couponName, DateTime? modifyDateTime = null)
        {
            try
            {
                using (var db = DbHelper.CreateDbHelper())
                {
                    await db.BeginTransactionAsync(IsolationLevel.RepeatableRead);
                    var result = await ModifyActivityCoupon(db, userId, activityId, couponCount, couponName, modifyDateTime);
                    if (result > 0)
                    {
                        db.Commit();

                    }
                    else
                    {
                        db.Rollback();
                    }
                    //删除用户缓存
                    using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
                    {
                        await cacheClient.RemoveAsync($"/GetUserActivityCoupon/{activityId}/{userId:N}");
                    }

                    return OperationResult.FromResult(result);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"修改或者增加用户兑换券失败 ==》{userId:D} {activityId} {couponCount} {couponName} ", e.InnerException ?? e);
                return OperationResult.FromError<long>("-1", Resource.Invalid_SystemError);
            }
        }

        /// <summary>
        ///     私有：修改或者增加用户兑换券  返回主键  -1 失败
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityId"></param>
        /// <param name="couponCount"></param>
        /// <param name="couponName"></param>
        /// <returns></returns>
        private static async Task<long> ModifyActivityCoupon(BaseDbHelper db, Guid userId, long activityId,
            int couponCount, string couponName, DateTime? modifyDateTime = null)
        {
            var activityModel = await GetActivityById(activityId);
            if (activityModel == null)
            {
                //找不到活动
                throw new Exception(Resource.Invalid_Activity_NotFound);
            }

            //更新主表
            var result = await DalActivity.ModifyActivityCoupon(db, userId, activityId, couponCount, modifyDateTime);

            //增加日志表
            var couponDetailResult = await DalActivity.InsertActivityCouponDetail(db, new ActivityCouponDetailModel()
            {
                ActivityId = activityId,
                ActivityName = activityModel.ActivityName,
                CouponCount = couponCount,
                CouponName = couponName,
                UserId = userId
            });
            if (result > 0 && couponDetailResult > 0)
            {
                return result;

            }

            return -1;
        }


        /// <summary>
        ///      刷新活动题目  缓存
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> RefreshActivityQuestionCacheAsync(long activityId)
        {
            try
            {
                var activityModel = await GetActivityById(activityId);
                if (activityModel == null)
                {
                    //找不到活动
                    throw new Exception(Resource.Invalid_Activity_NotFound);
                }

                //删除缓存
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
                {
                    await cacheClient.RemoveAsync($"ActivitySearchQuestion/{activityModel.QuestionnaireID}");
                    return OperationResult.FromResult(true);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"刷新活动题目  缓存失败 ==》{activityId} ", e.InnerException ?? e);
                return OperationResult.FromResult((false));

            }

        }

        /// <summary>
        ///      刷新活动兑换物  缓存
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> RefreshActivityPrizeCacheAsync(long activityId)
        {

            try
            {
                //删除缓存
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.ActivityType))
                {
                    await cacheClient.RemoveAsync($"SearchPrizeList/{activityId}");
                    //获取当前所有的商品，清空缓存
                    await SearchPrizeList(activityId, 0, null, false, 1, 1, 2);
                    return OperationResult.FromResult(true);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"刷新活动兑换物  缓存失败 ==》{activityId} ", e.InnerException ?? e);
                return OperationResult.FromResult((false));
            }

        }


        /// <summary>
        ///     更新用户答题结果状态
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<ModifyQuestionUserAnswerResultResponse>> ModifyQuestionUserAnswerResultAsync(ModifyQuestionUserAnswerResultRequest request)
        {
            var result = new ModifyQuestionUserAnswerResultResponse();
            try
            {
                using (var db = DbHelper.CreateDbHelper())
                {
                    var updateResult = await DalQuestionnaireAnswerResult.UpdateUserAnswerResult(db, request.ResultId,
                        request.WinCouponCount, request.AnswerResultStatus);

                    result.IsOk = updateResult;
                    return OperationResult.FromResult(result);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"更新用户答题结果状态 失败 ==》{request.ResultId} {request.AnswerResultStatus} {request.WinCouponCount} ", e.InnerException ?? e);
                return OperationResult.FromResult((result));
            }
        }
        #endregion

        #region 活动_yepeng

        /// <summary>
        /// 获取活动列表
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<Tuple<IEnumerable<ActivityNewModel>, int>>> GetActivityModelsPagedAsync(int pageIndex,
            int pageSize)
        {
            var result = await DalActivity.GetActivityModelsPagedAsync(pageIndex, pageSize);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 根据活动Id获取活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<ActivityNewModel>> GetActivityModelByActivityIdAsync(Guid activityId)
        {
            using (var client = CacheHelper.CreateCacheClient())
            {
                var result = await client.GetOrSetAsync($"ActivityModelCache/{activityId}",
                    async () => await DalActivity.GetActivityModelByActivityIdAsync(activityId), TimeSpan.FromHours(2));
                if (result != null && !result.Success && result.Exception != null)
                {
                    Logger.Error($"缓存设置失败。Message:{result.Exception}");
                    return OperationResult.FromError<ActivityNewModel>("-1", "GetActivityModelByActivityIdAsync方法中GetOrSet缓存方法错误");
                }
                else
                {
                    return OperationResult.FromResult(result?.Value);
                }
            }
        }
        /// <summary>
        /// 设置活动缓存
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> SetActivityModelByActivityIdCacheAsync(Guid activityId)
        {
            using (var client = CacheHelper.CreateCacheClient())
            {
                var result = await client.SetAsync($"ActivityModelCache/{activityId}",
                    await DalActivity.GetActivityModelByActivityIdAsync(activityId), TimeSpan.FromHours(2));
                if (result.Success)
                {
                    return OperationResult.FromResult(true);
                }
                else
                {
                    Logger.Error("ActivityModelByActivityId缓存设置失败");
                    return OperationResult.FromResult(false);
                }
            }
        }
        /// <summary>
        /// 移除活动缓存
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> RemoveActivityModelByActivityIdCacheAsync(Guid activityId)
        {
            using (var client = CacheHelper.CreateCacheClient())
            {
                var result = await client.RemoveAsync($"ActivityModelCache/{activityId}");
                if (result.Success)
                {
                    return OperationResult.FromResult(true);
                }
                else
                {
                    Logger.Error("RemoveActivityModelByActivityId缓存移除失败");
                    return OperationResult.FromResult(false);
                }
            }
        }
        /// <summary>
        /// 创建活动
        /// </summary>
        /// <param name="activityModel"></param>
        /// <returns></returns>
        public static async Task<OperationResult<int>> InsertActivityModelAsync(ActivityNewModel activityModel)
        {
            var result = await DalActivity.InsertActivityModelAsync(activityModel);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 更新活动
        /// </summary>
        /// <param name="activityModel"></param>
        /// <returns></returns>
        public static async Task<OperationResult<int>> UpdateActivityModelAsync(ActivityNewModel activityModel)
        {
            var result = await DalActivity.UpdateActivityModelAsync(activityModel);
            Logger.Info("修改免费洗车活动，Method:UpdateActivityModelAsync");
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 删除活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> DeleteActivityModelByActivityIdAsync(Guid activityId)
        {
            var result = await DalActivity.DeleteActivityModelByActivityIdAsync(activityId);
            Logger.Info("删除免费洗车活动，Method:DeleteActivityModelByPKIDAsync");
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 用户报名
        /// </summary>
        /// <param name="userActivityModel"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> InsertUserActivityModelAsync(UserApplyActivityModel userActivityModel)
        {
            var result = await DalActivity.InsertUserActivityModelAsync(userActivityModel);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 审核用户报名活动
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> UpdateUserActivityStatusByPKIDAsync(UserApplyActivityModel userActivityModel)
        {
            var result = await DalActivity.UpdateUserActivityStatusByPKIDAsync(userActivityModel);
            using (var client = CacheHelper.CreateCacheClient())
            {
                await client.RemoveAsync(new List<string>() { $"UserApplyActivityInfoIsExistCache/{userActivityModel.ActivityId}", $"ActivityModelApplyUserPassCountCache/{userActivityModel.ActivityId}" });
            }
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 分页获取用户报名列表
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<Tuple<IEnumerable<UserApplyActivityModel>, int>>> GetUserApplyActivityModelsPagedAsync(Guid activityId, AuditStatus auditStatus, int pageIndex, int pageSize)
        {
            var result = await DalActivity.GetUserApplyActivityModelsPagedAsync(activityId, auditStatus, pageIndex, pageSize);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 根据活动id获取报名人员数量
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<int>> GetActivityApplyUserCountByActivityIdAsync(Guid activityId)
        {
            using (var client = CacheHelper.CreateCacheClient())
            {
                var result = await client.GetOrSetAsync($"ActivityModelApplyUserCountCache/{activityId}",
                    async () => await DalActivity.GetActivityApplyUserCountByActivityIdAsync(activityId), TimeSpan.FromHours(2));
                if (result != null && !result.Success && result.Exception != null)
                {
                    Logger.Error($"缓存设置失败。Message:{result.Exception}");
                    return OperationResult.FromError<int>("-1", "GetActivityApplyUserCountByActivityIdAsync方法中GetOrSet缓存方法错误");
                }
                else
                {
                    return OperationResult.FromResult<int>(result != null ? result.Value : 0);
                }
            }
        }

        /// <summary>
        /// 根据活动id获取报名人员审核通过数量
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<int>> GetActivityApplyUserPassCountByActivityIdAsync(Guid activityId)
        {
            using (var client = CacheHelper.CreateCacheClient())
            {
                var result = await client.GetOrSetAsync($"ActivityModelApplyUserPassCountCache/{activityId}",
                        async () => await DalActivity.GetActivityApplyUserPassCountByActivityIdAsync(activityId), TimeSpan.FromHours(2));
                if (result != null && !result.Success && result.Exception != null)
                {
                    Logger.Error($"缓存设置失败。Message:{result.Exception}");
                    return OperationResult.FromError<int>("-1", "GetActivityApplyUserPassCountByActivityIdAsync方法中GetOrSet缓存方法错误");
                }
                else
                {
                    return OperationResult.FromResult<int>(result != null ? result.Value : 0);
                }
            }
        }

        /// <summary>
        /// 根据pkid获取报名人员
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<OperationResult<UserApplyActivityModel>> GetUserApplyActivityByPKIDAsync(int pkid)
        {
            var result = await DalActivity.GetUserApplyActivityByPKIDAsync(pkid);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 删除用户报名
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> DeleteUserApplyActivityModelByPKIDAsync(int pkid)
        {
            var result = await DalActivity.DeleteUserApplyActivityModelByPKIDAsync(pkid);
            Logger.Info("删除免费洗车活动用户报名信息，Method:DeleteUserApplyActivityModelByPKIDAsync");
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 检查用户报名活动手机号、车牌号、驾驶证号是否重复
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="carNum"></param>
        /// <param name="driverNum"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> CheckUserApplyActivityInfoIsExistAsync(Guid activityId, string mobile, string carNum, string driverNum)
        {
            using (var client = CacheHelper.CreateCacheClient())
            {
                var result = await client.GetOrSetAsync($"UserApplyActivityInfoIsExistCache/{activityId}",
                    async () => await DalActivity.CheckUserApplyActivityInfoIsExistAsync(activityId, mobile, carNum, driverNum), TimeSpan.FromHours(2));
                if (result != null && !result.Success && result.Exception != null)
                {
                    Logger.Error($"缓存设置失败。Message:{result.Exception}");
                    return OperationResult.FromError<bool>("-1", "CheckUserApplyActivityInfoIsExistAsync方法中GetOrSet缓存方法错误");
                }
                else
                {
                    return OperationResult.FromResult<bool>(result.Value);
                }
            }
        }
        /// <summary>
        /// 添加用户报名活动SortedSetCache
        /// </summary>
        /// <param name="userApplyActivityModel"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> AddUserApplyActivitySortedSetCacheAsync(UserApplyActivityModel userApplyActivityModel)
        {
            using (var client = CacheHelper.CreateSortedSetClient<UserApplyActivityModel>(WashCarActivityClientName))
            {
                var result = await client.AddAsync(userApplyActivityModel,
                    (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds);
                return OperationResult.FromResult(result.Success);
            }
        }
        /// <summary>
        /// 删除一个用户报名活动SortedSetCache
        /// </summary>
        /// <param name="userApplyActivityModel"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> RemoveOneUserApplyActivitySortedSetCacheAsync(
            UserApplyActivityModel userApplyActivityModel)
        {
            using (var client = CacheHelper.CreateSortedSetClient<UserApplyActivityModel>(WashCarActivityClientName))
            {
                var result = await client.RemoveAsync(userApplyActivityModel);
                return OperationResult.FromResult(result.Success);
            }
        }
        /// <summary>
        /// 获取用户报名活动集合
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<IEnumerable<UserApplyActivityModel>>> GetUserApplyActivityRangeByScoreAsync()
        {
            using (var client = CacheHelper.CreateSortedSetClient<UserApplyActivityModel>(WashCarActivityClientName))
            {
                var result = await client.GetRangeByScoreAsync(new SortedSetScoreRange());
                return OperationResult.FromResult(result.Success ? result.Value as IEnumerable<UserApplyActivityModel> : null);
            }
        }
        /// <summary>
        /// 获取用户报名活动添加用户报名活动SortedSetLength
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<long>> GetUserApplyActivitySortedSetLengthAsync()
        {
            using (var client = CacheHelper.CreateSortedSetClient<UserApplyActivityModel>(WashCarActivityClientName))
            {
                var length = await client.LengthAsync(new SortedSetScoreRange());
                return OperationResult.FromResult(length.Success ? length.Value : 0);
            }
        }
        #endregion
    }
}

