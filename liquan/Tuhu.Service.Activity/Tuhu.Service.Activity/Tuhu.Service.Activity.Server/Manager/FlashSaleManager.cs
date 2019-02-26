using Common.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Caching;
using AutoMapper;
using Newtonsoft.Json;
using RabbitMQ.Client.Apigen.Attributes;
using Tuhu.Models;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.UserAccount;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Utils;
using Tuhu.Service.Order;
using Tuhu.Service.Activity.Server.FlashSaleSystem;
using Tuhu.Service.Activity.Server.Manager.FlashSaleSystem;
using Tuhu.Service.Activity.Server.Model;
using Tuhu.Service.Config;
using Tuhu.Service.Config.Models.Response;
using Tuhu.Service.Activity.DataAccess.Models;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models.ProductConfig;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Product.Request;
using Tuhu.Service.Product.ModelDefinition.ProductModels;
using Tuhu.Service.ProductQuery;

namespace Tuhu.Service.Activity.Server.Manager
{
    public class FlashSaleManager
    {
        public static readonly string DefaultClientName = "FlashSale";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FlashSaleManager));

        public static bool CheckAllPlaceLimitStatrtDate()
        {
            var date = GlobalConstant.AllPlaceLimitDate;
            var startDate = Convert.ToDateTime(date.Split(',')[0]);
            var endDate = Convert.ToDateTime(date.Split(',')[1]);
            return DateTime.Now >= startDate && DateTime.Now <= endDate;
        }



        /// <summary>
        /// 查询 活动信息以及 实时活动销量
        /// </summary>
        /// <param name="activityIDs"></param>
        /// <returns></returns>
        public static async Task<List<FlashSaleModel>> GetFlashSaleDataAndQuantityByIdsAsync(Guid[] activityIDs)
        {
            var flashSaleModelResults = await GetFlashSaleModelWithReidsWithMemory(activityIDs.Distinct().ToArray());

            if (flashSaleModelResults.Any())
            {
                var salOutQResult = await Task.WhenAll(
                    flashSaleModelResults.Select(p => p.ActivityID)
                    .ParallelSelect(GetSaleOutQuantityWithHashReidsWithMemoryAsync, 10));
                var tempResult = salOutQResult.ToDictionary(p => p.Item1, p => p.Item2);
                flashSaleModelResults.ForEach(p =>
                {
                    tempResult.TryGetValue(p.ActivityID, out Dictionary<string, int> activityDic);
                    p.Products.ForEach(pp =>
                    {
                        pp.OnSale = true;
                        pp.stockout = false;
                        pp.Position = pp.Position.GetValueOrDefault(99999);

                        activityDic.TryGetValue(pp.PID, out int SaleOutQuantity);
                        pp.SaleOutQuantity = SaleOutQuantity;
                    });
                });
            }
            return flashSaleModelResults;
        }

        private static async Task<List<FlashSaleModel>> GetFlashSaleModelWithReidsWithMemory(Guid[] activityIDs)
        {
            return await TuhuMemoryCacheNoJson.Instance.GetOrSetAsync(
                "GetFlashSaleModelWithReidsWithMemory" + string.Join(";", activityIDs.OrderBy()),
                () => GetFlashSaleModelWithReids(activityIDs), TimeSpan.FromMinutes(2.5));
        }

        private static async Task<List<FlashSaleModel>> GetFlashSaleModelWithReids(Guid[] activityIDs)
        {
            var flashSaleModelResults = new List<FlashSaleModel>();
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var activityCahceKeys = activityIDs.Select(activityID => GlobalConstant.FlashSaleCacheKeyPrefix + activityID);
                var falshModelResults = await client.GetAsync<FlashSaleModel>(activityCahceKeys);
                flashSaleModelResults = falshModelResults
                   .Where(p => p.Value.Success && p.Value?.Value != null && p.Value.Value.ActivityID != Guid.Empty)
                   .Select(p => p.Value.Value).ToList();

            }
            return flashSaleModelResults;
        }

        private static async Task<Tuple<Guid, Dictionary<string, int>>> GetSaleOutQuantityWithHashReidsWithMemoryAsync(Guid activityID)
        {
            var result = await TuhuMemoryCacheNoJson.Instance.GetOrSetAsync("GetSaleOutQuantityWithHashReidsWithMemory" + activityID, () => GetSaleOutQuantityWithHashReidsAsync(activityID), TimeSpan.FromMinutes(1));
            return new Tuple<Guid, Dictionary<string, int>>(activityID, result);
        }

        private static async Task<Dictionary<string, int>> GetSaleOutQuantityWithHashReidsAsync(Guid activityID)
        {
            var resultDic = new Dictionary<string, int>();
            using (var hashClient = CacheHelper.CreateHashClient(activityID.ToString(), TimeSpan.FromDays(30)))
            {
                var hashResult = await hashClient.GetAllAsync();
                if (!hashResult.Success) return new Dictionary<string, int>();
                var result = hashResult.Value?.Select(r => new FlashSaleProductModel
                {
                    ActivityID = activityID,
                    PID = r.Key,
                    SaleOutQuantity = r.Value.To<int>()
                }).ToList() ?? new List<FlashSaleProductModel>();
                result.ForEach(p => resultDic[p.PID] = p.SaleOutQuantity);
                return resultDic;
            }
        }

        public static async Task<FlashSaleModel> SelectFlashSaleDataByActivityIDAsync(Guid activityID, bool excludeProductTags = false)
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            FlashSaleModel model = null;
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.GetOrSetAsync(GlobalConstant.FlashSaleCacheKeyPrefix + activityID, () => DalFlashSale.SelectFlashSaleFromDBAsync(activityID), GlobalConstant.FlashSaleCacheExpiration);
                if (result.Success)
                {
                    model = result.Value;
                    Logger.Info($"从缓存中读执行时间=》{w.ElapsedMilliseconds}参数activityID=》{activityID}");
                }

                else
                {
                    model = await DalFlashSale.SelectFlashSaleFromDBAsync(activityID);
                    if (w.ElapsedMilliseconds > 100)
                        Logger.Info($"SelectFlashSaleFromDBAsync执行时间=>{w.ElapsedMilliseconds}参数activityID=》{activityID}");
                    Logger.Warn($"获取限时抢购redis数据失败SelectFlashSaleDataByActivityIDAsync:{GlobalConstant.FlashSaleCacheKeyPrefix + activityID};Error:{result.Message}", result.Exception);
                }

                if (model == null)
                    return new FlashSaleModel();
                else
                {
                    //var runtimeSwitch = new RuntimeSwitchResponse();
                    List<FlashSaleProductModel> tempResult;
                    using (var hashClient = CacheHelper.CreateHashClient(activityID.ToString(), TimeSpan.FromDays(30)))
                    {
                        var recordCache = await hashClient.GetAllAsync();
                        if (!recordCache.Success)
                        {
                            tempResult = (await SelectFlashSaleSaleOutQuantityAsync(activityID)).ToList();
                            Logger.Info($"CreateHashClientError={w.ElapsedMilliseconds}参数activityID=》{activityID}=>{recordCache.Message}=>{recordCache?.Value?.Count()}");
                            if (w.ElapsedMilliseconds > 100)
                                Logger.Info($"CreateHashClientError=>DB={w.ElapsedMilliseconds}参数activityID=》{activityID}=>{recordCache.Message}");
                        }
                        else
                        {
                            if (recordCache.Value != null && recordCache.Value.Any())
                            {
                                tempResult = recordCache.Value.Select(r => new FlashSaleProductModel
                                {
                                    PID = r.Key,
                                    SaleOutQuantity = r.Value.To<int>()
                                }).ToList();
                                if (w.ElapsedMilliseconds > 100)
                                    Logger.Info($"CreateHashClientr={w.ElapsedMilliseconds}=>{recordCache.ElapsedMilliseconds}=>参数activityID=》{activityID}");
                            }
                            else
                            {
                                tempResult = (await SelectFlashSaleSaleOutQuantityAsync(activityID)).ToList();
                                if (w.ElapsedMilliseconds > 100)
                                    Logger.Info($"CreateHashClient=>DB={w.ElapsedMilliseconds}参数activityID=》{activityID}=>{recordCache.Message}");
                                var dic = new Dictionary<string, object>();
                                foreach (var pidItem in tempResult)
                                {
                                    dic.Add(pidItem.PID, pidItem.SaleOutQuantity);
                                }
                                IReadOnlyDictionary<string, object> roDic = new ReadOnlyDictionary<string, object>(dic);
                                var increment = await hashClient.SetAsync<IReadOnlyDictionary<string, object>>(roDic);
                                if (!increment.Success)
                                {
                                    Logger.Error($"限时抢购初始化redis失败=>SelectFlashSaleDataByActivityIDAsync：ActivityId=》{activityID}");
                                }
                            }
                        }
                    }

                    if (w.ElapsedMilliseconds > 100)
                        Logger.Info($"SelectFlashSaleSaleOutQuantityAsync执行时间=》{w.ElapsedMilliseconds}参数activityID=》{activityID}");
                    model.Products = from p1 in model.Products
                                     join p2 in tempResult on p1.PID equals p2.PID into temp
                                     from p2 in temp.DefaultIfEmpty()
                                     select new FlashSaleProductModel
                                     {
                                         ActivityID = p1.ActivityID,
                                         FalseOriginalPrice = p1.FalseOriginalPrice,
                                         Channel = p1.Channel,
                                         ImgUrl = p1.ImgUrl,
                                         InstallAndPay = p1.InstallAndPay,
                                         IsJoinPlace = p1.IsJoinPlace,
                                         IsUsePCode = p1.IsUsePCode,
                                         Label = p1.Label,
                                         Level = p1.Level,
                                         MaxQuantity = p1.MaxQuantity,
                                         PID = p1.PID,
                                         PKID = p1.PKID,
                                         Position = p1.Position.GetValueOrDefault(99999),
                                         Price = p1.Price,
                                         ProductImg = p1.ProductImg,
                                         ProductName = p1.ProductName,
                                         SaleOutQuantity = p2?.SaleOutQuantity ?? 0,
                                         IsShow = p1.IsShow,
                                         OnSale = true,
                                         stockout = false,
                                         TotalQuantity = p1.TotalQuantity,
                                         InstallService = "",
                                         AdvertiseTitle = p1.AdvertiseTitle,
                                         Brand = p1.Brand
                                     };
                    Logger.Info($"接口总执行时间=》{w.ElapsedMilliseconds}参数activityID=》{activityID}");
                    w.Stop();
                    return model;
                }
            }
        }

        /// <summary>
        /// 根据天天秒杀活动id查询活动信息和产品信息
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="excludeProductTags"></param>
        /// <returns></returns>
        public static async Task<FlashSaleModel> SelectSecKillFlashSaleDataByActivityIDAsync(Guid activityID, bool excludeProductTags = false)
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            FlashSaleModel model = null;
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.GetOrSetAsync(GlobalConstant.FlashSaleCacheKeyPrefix + activityID, () => DalFlashSale.SelectSecKillFlashSaleFromDBAsync(activityID), GlobalConstant.FlashSaleCacheExpiration);
                if (result.Success)
                {
                    model = result.Value;
                    Logger.Info($"从缓存中读执行时间=》{w.ElapsedMilliseconds}参数activityID=》{activityID}");
                }

                else
                {
                    model = await DalFlashSale.SelectSecKillFlashSaleFromDBAsync(activityID);
                    if (w.ElapsedMilliseconds > 100)
                        Logger.Info($"SelectFlashSaleFromDBAsync执行时间=>{w.ElapsedMilliseconds}参数activityID=》{activityID}");
                    Logger.Warn($"获取限时抢购redis数据失败SelectFlashSaleDataByActivityIDAsync:{GlobalConstant.FlashSaleCacheKeyPrefix + activityID};Error:{result.Message}", result.Exception);
                }

                if (model == null)
                    return new FlashSaleModel();
                else
                {
                    //var runtimeSwitch = new RuntimeSwitchResponse();
                    List<FlashSaleProductModel> tempResult;
                    using (var hashClient = CacheHelper.CreateHashClient(activityID.ToString(), TimeSpan.FromDays(30)))
                    {
                        var recordCache = await hashClient.GetAllAsync();
                        if (!recordCache.Success)
                        {
                            tempResult = (await SelectFlashSaleSaleOutQuantityAsync(activityID)).ToList();
                            Logger.Info($"CreateHashClientError={w.ElapsedMilliseconds}参数activityID=》{activityID}=>{recordCache.Message}=>{recordCache?.Value?.Count()}");
                            if (w.ElapsedMilliseconds > 100)
                                Logger.Info($"CreateHashClientError=>DB={w.ElapsedMilliseconds}参数activityID=》{activityID}=>{recordCache.Message}");
                        }
                        else
                        {
                            if (recordCache.Value != null && recordCache.Value.Any())
                            {
                                tempResult = recordCache.Value.Select(r => new FlashSaleProductModel
                                {
                                    PID = r.Key,
                                    SaleOutQuantity = r.Value.To<int>()
                                }).ToList();
                                if (w.ElapsedMilliseconds > 100)
                                    Logger.Info($"CreateHashClientr={w.ElapsedMilliseconds}=>{recordCache.ElapsedMilliseconds}=>参数activityID=》{activityID}");
                            }
                            else
                            {
                                tempResult = (await SelectFlashSaleSaleOutQuantityAsync(activityID)).ToList();
                                if (w.ElapsedMilliseconds > 100)
                                    Logger.Info($"CreateHashClient=>DB={w.ElapsedMilliseconds}参数activityID=》{activityID}=>{recordCache.Message}");
                                var dic = new Dictionary<string, object>();
                                foreach (var pidItem in tempResult)
                                {
                                    dic.Add(pidItem.PID, pidItem.SaleOutQuantity);
                                }
                                IReadOnlyDictionary<string, object> roDic = new ReadOnlyDictionary<string, object>(dic);
                                var increment = await hashClient.SetAsync<IReadOnlyDictionary<string, object>>(roDic);
                                if (!increment.Success)
                                {
                                    Logger.Error($"限时抢购初始化redis失败=>SelectFlashSaleDataByActivityIDAsync：ActivityId=》{activityID}");
                                }
                            }
                        }
                    }

                    //  包安装 标签的列表
                    List<string> installFreeTagPids = null;
                    if (!excludeProductTags)
                    {
                        try
                        {
                            using (var productSearchClient = new ProductSearchClient())
                            {
                                // 获取产品的标签
                                var productTagResult = await productSearchClient.SelectProductsTagAsync(new SeachTagRequest()
                                {
                                    PidList = model?.Products?.Select(p => p.PID).ToList() ?? new List<string>(),
                                    SeachTags = new List<ProductTagTypeIn>()
                                    {
                                        ProductTagTypeIn.InstallFree
                                    }
                                });
                                productTagResult.ThrowIfException();

                                // 找到`包安装`标签
                                installFreeTagPids = productTagResult?
                                    .Result?
                                    .FirstOrDefault(p => "InstallFree" == p.Key)
                                    .Value?
                                    .ToList();
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error(" FlashSaleManager -> SelectFlashSaleDataByActivityIDAsync -> error ", e.InnerException ?? e);
                        }
                    }

                    if (w.ElapsedMilliseconds > 100)
                        Logger.Info($"SelectFlashSaleSaleOutQuantityAsync执行时间=》{w.ElapsedMilliseconds}参数activityID=》{activityID}");
                    model.Products = from p1 in model.Products
                                     join p2 in tempResult on p1.PID equals p2.PID into temp
                                     from p2 in temp.DefaultIfEmpty()
                                     select new FlashSaleProductModel
                                     {
                                         ActivityID = p1.ActivityID,
                                         FalseOriginalPrice = p1.FalseOriginalPrice,
                                         Channel = p1.Channel,
                                         ImgUrl = p1.ImgUrl,
                                         InstallAndPay = p1.InstallAndPay,
                                         IsJoinPlace = p1.IsJoinPlace,
                                         IsUsePCode = p1.IsUsePCode,
                                         Label = p1.Label,
                                         Level = p1.Level,
                                         MaxQuantity = p1.MaxQuantity,
                                         PID = p1.PID,
                                         PKID = p1.PKID,
                                         Position = p1.Position.GetValueOrDefault(99999),
                                         Price = p1.Price,
                                         ProductImg = p1.ProductImg,
                                         ProductName = p1.ProductName,
                                         SaleOutQuantity = p2?.SaleOutQuantity ?? 0,
                                         IsShow = p1.IsShow,
                                         OnSale = true,
                                         stockout = false,
                                         TotalQuantity = p1.TotalQuantity,
                                         InstallService = (installFreeTagPids?.Contains(p1.PID) ?? false) ? "包安装" : "",
                                         AdvertiseTitle = p1.AdvertiseTitle,
                                         Brand = p1.Brand
                                     };
                    Logger.Info($"接口总执行时间=》{w.ElapsedMilliseconds}参数activityID=》{activityID}");
                    w.Stop();
                    return model;
                }
            }
        }

        public async static Task<FlashSaleOrderResponse> CheckCanBuyFlashSaleOrderAsync(FlashSaleOrderRequest request)
        {
            FlashSaleOrderResponse result = new FlashSaleOrderResponse();
            result.IsCanBuy = true;
            if (request.Products.Any(_ => _.ActivityId != null))
            {
                bool isNewMember = await IsNewMember(request.UserId);
                if (!isNewMember)
                {
                    var activitys = await GetFlashSaleListAsync(request.Products.Where(_ => _.ActivityId != null).Select(_ => _.ActivityId.GetValueOrDefault()).Distinct().ToArray());
                    if (activitys.Any(_ => _.IsNewUserFirstOrder))
                    {
                        result.ErrorCode = -7;
                        result.ErrorMessage = "仅限新用户购买";
                        result.IsCanBuy = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 根据活动ID获取限时抢购数量
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<FlashSaleProductModel>> SelectFlashSaleSaleOutQuantityAsync(Guid activityId)
        {
            return await TuhuMemoryCacheNoJson.Instance.GetOrSetAsync("saleOutCount/" + activityId,
                () => DalFlashSale.SelectFlashSaleSaleOutQuantityAsync(activityId),
                GlobalConstant.FlashSaleSaleQuantityCacheExpiration);
        }


        /// <summary>
        /// 新活动页根据活动ID获取限时抢购数量
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<FlashSaleProductModel>> GetFlashSaleSaleOutQuantityActivityPage(Guid activityId, List<string> pids)
        {
            return await TuhuMemoryCacheNoJson.Instance.GetOrSetAsync("saleOutCount/" + activityId,
                () => DalFlashSale.GetFlashSaleSaleOutQuantityActivityPage(activityId, pids),
                GlobalConstant.FlashSaleSaleQuantityCacheExpiration);
        }

        /// <summary>
        /// 获取秒杀数据
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="scheduleDate"></param>
        /// <param name="needProducts"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FlashSaleModel>> SelectSecondKillTodayDataAsync(int activityType, DateTime? scheduleDate = null, bool needProducts = true, bool excludeProductTags = false)
        {
            var sw = new Stopwatch();
            sw.Start();

            IEnumerable<Guid> ids;
            var schedules = new List<string>
            {
                "0点场",
                "13点场",
                "16点场",
                "20点场",
                "10点场",

            };
            var flashsales = new List<FlashSaleModel>();
            if (!scheduleDate.HasValue)
                scheduleDate = DateTime.Now;

            var year = scheduleDate.Value.Year;
            var month = scheduleDate.Value.Month;
            var day = scheduleDate.Value.Day;

            var prefix = await GlobalConstant.GetKeyPrefix("SecondKillPrefix", DefaultClientName);
            var prefixKey = $"{prefix}{year}/{month}/{day}/{activityType}";
            using (var cacheClient = CacheHelper.CreateCacheClient(DefaultClientName))
            {

                var result = await cacheClient.GetOrSetAsync(prefixKey, () => DalFlashSale.SelectSecondKillTodayDataSqlAsync(activityType, scheduleDate.Value), GlobalConstant.SecondKillTodayCacheExpiration);

                if (result.Success)
                {
                    ids = result.Value;

                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"{prefixKey}.GetOrSetAsync 执行时间=>{sw.ElapsedMilliseconds}");
                        sw.Restart();
                    }
                }
                else
                {
                    Logger.Warn($"获取当日秒杀产品redis数据失败SelectSecondKillTodayDataAsync:{prefixKey};Error:{result.Message}", result.Exception);

                    ids = await DalFlashSale.SelectSecondKillTodayDataSqlAsync(activityType, scheduleDate.Value);

                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"SelectSecondKillTodayDataSqlAsync 执行时间=>{sw.ElapsedMilliseconds}");
                        sw.Restart();
                    }
                }
            }

            var guids = ids as Guid[] ?? ids.ToArray();
            if (guids.Any())
            {
                if (needProducts)
                {
                    var result = await GetSecKillFlashSaleListAsync(guids.ToArray(), excludeProductTags);
                    flashsales = result.OrderBy(d => d.StartDateTime).ToList();
                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"GetFlashSaleDataAsync 执行时间=>{sw.ElapsedMilliseconds}");
                        sw.Restart();
                    }
                }
                else
                {
                    var result = await GetFlashSaleWithoutProductsListAsync(guids.ToList());
                    flashsales = result.OrderBy(d => d.StartDateTime).ToList();
                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"GetFlashSaleWithoutProductsListAsync 执行时间=>{sw.ElapsedMilliseconds}");
                        sw.Restart();
                    }
                }
            }
            if (activityType == 1 && guids.Length < 5)
            {
                flashsales.ForEach(item =>
                {
                    var s = ConvertToSchedule(item.StartDateTime.Hour);
                    schedules.Remove(s);
                });
                foreach (var item in schedules)
                {
                    var result = await GetSeckillDefaultDataByScheduleAsync(item);
                    var tupletime = CovertDefaultTimeToRealScheduleTime(item, scheduleDate.Value);

                    result.StartDateTime = Convert.ToDateTime(tupletime.Item1);
                    result.EndDateTime = Convert.ToDateTime(tupletime.Item2);

                    flashsales.Add(result);

                }
            }

            if (sw.ElapsedMilliseconds > 100)
                Logger.Info($"SelectSecondKillTodayDataAsyncEnd 执行时间=>{sw.ElapsedMilliseconds}");

            sw.Stop();
            return flashsales;
        }
        private static Tuple<string, string> CovertDefaultTimeToRealScheduleTime(string schedule, DateTime scheduleDate)
        {
            var datetime = scheduleDate;
            var strDate = "";
            var endDate = "";
            switch (schedule)
            {
                case "10点场":
                    strDate = datetime.ToString("yyyy-MM-dd 10:00:0");
                    endDate = datetime.ToString("yyyy-MM-dd 13:00:0");
                    break;
                case "13点场":
                    strDate = datetime.ToString("yyyy-MM-dd 13:00:00");
                    endDate = datetime.ToString("yyyy-MM-dd 16:00:0");
                    break;
                case "16点场":
                    strDate = datetime.ToString("yyyy-MM-dd 16:00:00");
                    endDate = datetime.ToString("yyyy-MM-dd 20:00:0");
                    break;
                case "20点场":
                    strDate = datetime.ToString("yyyy-MM-dd 20:00:00");
                    endDate = datetime.ToString("yyyy-MM-dd 23:59:59");
                    break;
                case "0点场":
                    strDate = datetime.ToString("yyyy-MM-dd 00:00:00");
                    endDate = datetime.ToString("yyyy-MM-dd 10:00:0");
                    break;
            }
            return new Tuple<string, string>(strDate, endDate);
        }
        private static string ConvertToSchedule(int hour)
        {
            var s = "";
            if (hour >= 20 && hour <= 24)
            {
                s = "20点场";
            }
            if (hour >= 0 && hour <= 9)
            {
                s = "0点场";
            }
            if (hour >= 10 && hour <= 12)
            {
                s = "10点场";
            }
            if (hour >= 13 && hour <= 15)
            {
                s = "13点场";
            }
            if (hour >= 16 && hour <= 19)
            {
                s = "16点场";
            }
            return s;
        }
        public static async Task<bool> UpdateFlashSaleCacheAsync(Guid activityID)
        {
            Logger.Info($"刷新限时抢购redis缓存接口调用:{GlobalConstant.FlashSaleCacheKeyPrefix + activityID}");
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var data = await DalFlashSale.SelectFlashSaleFromDBAsync(activityID, false);
                var data2 = await DalFlashSale.SelectFlashSaleModelFromdbAsync(activityID, false);
                var result = await client.SetAsync(GlobalConstant.FlashSaleCacheKeyPrefix + activityID, data, GlobalConstant.FlashSaleCacheExpiration);
                if (!result.Success)
                    Logger.Warn($"刷新限时抢购redis缓存失败UpdateFlashSaleCacheAsync:{GlobalConstant.FlashSaleCacheKeyPrefix + activityID};Error:{result.Message}", result.Exception);
                var result2 = await client.SetAsync(GlobalConstant.FlashSaleCacheKeyPrefix2 + activityID, data2, GlobalConstant.FlashSaleCacheExpiration);
                if (!result2.Success)
                    Logger.Warn($"刷新限时抢购redis缓存失败UpdateFlashSaleCacheAsync:{GlobalConstant.FlashSaleCacheKeyPrefix2 + activityID};Error:{result2.Message}", result2.Exception);
                return result.Success && result2.Success;
            }
        }

        public async static Task<IEnumerable<FlashSaleProductBuyLimitModel>> CheckFlashSaleProductBuyLimitAsync(CheckFlashSaleProductRequest request)
        {
            var dic = request.ActivityProducts.GroupBy(_ => _.ActivityID).ToDictionary(_ => _.Key, _ => _);

            var FlashSale = await GetFlashSaleListAsync(dic.Keys.ToArray());

            System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var runtimeSwitch = new RuntimeSwitchResponse();
            bool bSwitch = false;
            try
            {

                using (var config = new ConfigClient())
                {
                    var switchCache = await config.GetOrSetRuntimeSwitchAsync("NewFalshSale");
                    if (switchCache.Success)
                        runtimeSwitch = switchCache.Result;

                }

            }
            catch (Exception ex)
            {
                Logger.Error($"调用开关服务出错" + ex.InnerException);
            }
            if (request.ActivityProducts.Any(r => r.ActivityID.ToString() == runtimeSwitch.Description) || runtimeSwitch.Description == "New")
                bSwitch = true;
            List<FlashSaleRecordsModel> records = new List<FlashSaleRecordsModel>();
            if (bSwitch)
            {
                foreach (var item in dic.Keys.ToArray())
                {
                    var record = await DalFlashSale.SelectActivityProductOrderRecordsAsync(request.UserID, request.DeviceID, item);
                    records.AddRange(record);
                }
            }

            else
            {
                records = (await DalFlashSale.SelectFlashSaleRecordsAsync(request.UserID, request.DeviceID, dic.Keys.ToArray())).ToList();
            }

            stopwatch.Stop();
            Logger.Warn($"CheckFlashSaleProductBuyLimitAsync查询购买记录耗时：{stopwatch.Elapsed.TotalMilliseconds}毫秒");

            var sumRecords = records.GroupBy(_ => _.ActivityID + _.PID).ToDictionary(_ => _.Key, _ => _.Sum(c => c.Quantity));

            var query = from fs in FlashSale
                        from fsp in fs.Products
                        from d in request.ActivityProducts
                        where fsp.ActivityID == d.ActivityID && fsp.PID == d.PID
                        join r in sumRecords on fsp.ActivityID + fsp.PID equals r.Key into j
                        select new FlashSaleTempModel()
                        {
                            ActivityID = fsp.ActivityID,
                            BuyQuantity = d.Num,
                            OverplusQuantity = fsp.TotalQuantity == null ? 99999 : fsp.TotalQuantity.Value - fsp.SaleOutQuantity,
                            PID = fsp.PID,
                            PlaceQuantity = fs.PlaceQuantity,
                            RecordQuantity = j.FirstOrDefault().Value,
                            SingleQuantity = fsp.MaxQuantity,
                            IsJoinPlace = fsp.IsJoinPlace,
                            IsNewUserFirstOrder = fs.IsNewUserFirstOrder,
                            EndTime = fs.EndDateTime,
                            StartTime = fs.StartDateTime,
                            HasNumLimit = fsp.TotalQuantity > 0|| fsp.MaxQuantity > 0 || (fs.PlaceQuantity > 0 && fsp.IsJoinPlace)
                        };
            var notNewMember = !(await IsNewMember(request.UserID));
            var result = query.Select(_ =>
            {
                return new FlashSaleProductBuyLimitModel()
                {
                    ActivityID = _.ActivityID,
                    PID = _.PID,
                    OnlyNewMemberCanBuy = _.IsNewUserFirstOrder && notNewMember,
                    HasNumLimit = _.HasNumLimit,
                    Type = _.StartTime > DateTime.Now ? FlashSaleProductLimitType.NotStart :
                                (_.EndTime <= DateTime.Now ? FlashSaleProductLimitType.End :
                                (_.OverplusQuantity != null && _.OverplusQuantity.Value - _.BuyQuantity < 0 ? FlashSaleProductLimitType.TotalLimit :
                                (_.SingleQuantity != null && _.SingleQuantity.Value - _.RecordQuantity - _.BuyQuantity < 0 ? FlashSaleProductLimitType.SingleLimit :
                                (_.PlaceQuantity != null && query.Where(q => q.ActivityID == _.ActivityID).Sum(q => q.BuyQuantity + q.RecordQuantity) > _.PlaceQuantity.Value ? FlashSaleProductLimitType.PlaceLimit : FlashSaleProductLimitType.Success))))

                };
            });

            return result;
        }

        public async static Task<List<FlashSaleModel>> GetFlashSaleListAsync(Guid[] activityIDs, bool excludeProductTags = false)
        {
            Stopwatch w = new Stopwatch();
            Logger.Info("开始执行接口");
            w.Start();
            List<FlashSaleModel> list = new List<FlashSaleModel>();

            foreach (var activityID in activityIDs)
            {
                var result = await SelectFlashSaleDataByActivityIDAsync(activityID, excludeProductTags);
                if (result != null)
                {
                    list.Add(result);
                }
            }
            w.Stop();
            if (w.ElapsedMilliseconds > 500)
                Logger.Info($"活动页执行接口结束{w.ElapsedMilliseconds}活动Id=》{string.Join(",", activityIDs)}");
            return list;
        }

        /// <summary>
        /// 查询天天秒杀及产品数据
        /// </summary>
        /// <param name="activityIDs"></param>
        /// <param name="excludeProductTags"></param>
        /// <returns></returns>
        public static async Task<List<FlashSaleModel>> GetSecKillFlashSaleListAsync(Guid[] activityIDs, bool excludeProductTags = false)
        {
            Stopwatch w = new Stopwatch();
            Logger.Info("开始执行接口");
            w.Start();
            List<FlashSaleModel> list = new List<FlashSaleModel>();

            foreach (var activityID in activityIDs)
            {
                var result = await SelectSecKillFlashSaleDataByActivityIDAsync(activityID, excludeProductTags);
                if (result != null)
                {
                    list.Add(result);
                }
            }
            w.Stop();
            if (w.ElapsedMilliseconds > 500)
                Logger.Info($"活动页执行接口结束{w.ElapsedMilliseconds}活动Id=》{string.Join(",", activityIDs)}");
            return list;
        }

        public async static Task<List<FlashSaleModel>> GetFlashSaleListParallelSelectAsync(Guid[] activityIDs, bool excludeProductTags = false)
        {
            Stopwatch w = new Stopwatch();
            Logger.Info("开始执行接口");
            w.Start();
            List<FlashSaleModel> list = new List<FlashSaleModel>();
            var taskAsync = await Task.WhenAll(activityIDs.ParallelSelect(
                activityID => SelectFlashSaleDataByActivityIDAsync(activityID, true), 5).ToList());
            foreach (var result in taskAsync)
            {
                if (result != null)
                {
                    list.Add(result);
                }
            }
            w.Stop();
            Logger.Info($"GetFlashSaleListParallelSelectAsync-->{w.ElapsedMilliseconds}");
            if (w.ElapsedMilliseconds > 500)
                Logger.Info($"活动页执行接口结束{w.ElapsedMilliseconds}活动Id=》{string.Join(",", activityIDs)}");
            return list;
        }

        public static async Task<List<FlashSaleModel>> GetFlashSaleDataAsync(Guid[] activityIds)
        {
            // 1.循环activityIds读取缓存
            // 2.循环没有命中缓存的数据 set 缓存
            // 3.分批查询产品标签数据

            var flashSaleList = new List<FlashSaleModel>();
            var cacheList = new List<FlashSaleModel>();
            var needCacheList = new List<Guid>();
            var pids = new List<string>();
            var sw = new Stopwatch();
            sw.Start();

            // 获取活动缓存
            if (activityIds.Any())
            {
                using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
                {
                    var keyDicList = activityIds.ToDictionary(p => GlobalConstant.FlashSaleCacheKeyPrefix + p, p => p);

                    var resultList = await client.GetAsync<FlashSaleModel>(keyDicList.Select(p => p.Key).ToList());

                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"GetFlashSaleDataAsync 批量Get活动缓存数据执行时间=>{sw.ElapsedMilliseconds}");
                        sw.Restart();
                    }

                    resultList.ForEach(p =>
                    {
                        if (p.Value.Success)
                        {
                            cacheList.Add(p.Value.Value);
                            if (p.Value.Value.Products.Any())
                            {
                                pids.AddRange(p.Value.Value.Products.Select(product => product.PID));
                            }
                        }
                        else
                        {
                            needCacheList.Add(keyDicList[p.Key]);
                            Logger.Info($"从缓存中获取天天秒杀数据失败=》{sw.ElapsedMilliseconds}参数activityID=》{keyDicList[p.Key]};Error:{p.Value.Message}", p.Value.Exception);
                        }
                    });

                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"GetFlashSaleDataAsync ForEach活动及产品数据执行时间=>{sw.ElapsedMilliseconds}");
                        sw.Restart();
                    }
                }
            }

            // 缓存活动数据
            if (needCacheList.Any())
            {
                var newFlashSaleList = await DalFlashSale.SelectFlashSaleFromDbByActivityIdsAsync(needCacheList.ToArray());

                if (sw.ElapsedMilliseconds > 100)
                {
                    Logger.Info($"GetFlashSaleDataAsync 批量查询活动数据执行时间=>{sw.ElapsedMilliseconds}");
                    sw.Restart();
                }

                using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
                {
                    foreach (var flashSale in newFlashSaleList)
                    {

                        var cacheFlashSaleResult = await client.GetOrSetAsync(GlobalConstant.FlashSaleCacheKeyPrefix + flashSale.ActivityID, () => DalFlashSale.SelectFlashSaleFromDBAsync(flashSale.ActivityID), GlobalConstant.FlashSaleCacheExpiration);

                        if (cacheFlashSaleResult.Success)
                        {
                            cacheList.Add(cacheFlashSaleResult.Value);
                            if (cacheFlashSaleResult.Value.Products.Any())
                            {
                                pids.AddRange(cacheFlashSaleResult.Value.Products.Select(product => product.PID));
                            }
                        }
                        else
                        {
                            var singleFlashSale = await DalFlashSale.SelectFlashSaleFromDBAsync(flashSale.ActivityID);

                            if (singleFlashSale != null)
                            {
                                cacheList.Add(singleFlashSale);
                                if (singleFlashSale.Products.Any())
                                    pids.AddRange(singleFlashSale.Products.Select(product => product.PID));
                            }
                            Logger.Warn($"从缓存中获取天天秒杀数据失败=》{sw.ElapsedMilliseconds}参数activityID=》{flashSale.ActivityID};Error:{cacheFlashSaleResult.Message}", cacheFlashSaleResult.Exception);
                        }

                        if (sw.ElapsedMilliseconds > 100)
                        {
                            Logger.Info($"GetFlashSaleDataAsync 循环需要缓存活动数据执行时间=>{sw.ElapsedMilliseconds}");
                            sw.Restart();
                        }
                    }

                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"循环需要缓存活动数据的总执行时间=>{sw.ElapsedMilliseconds}");
                        sw.Restart();
                    }
                }
            }

            // 包安装 标签的列表
            var installFreeTagPids = new List<string>();
            try
            {
                using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
                {
                    var pidSort = pids?.OrderBy().Distinct()?.ToList() ?? new List<string>();
                    var paralleResult = await Task.WhenAll(pidSort.Split(100).ParallelSelect(async pid =>
                    {
                        var pidArr = pid as string[] ?? pid.ToArray();
                        var tagsResult = await client.GetOrSetAsync(GlobalConstant.FlashSaleProductInstallTagKeyPrefix + string.Join(",", pidArr).GetHashCode(), async () =>
                        {
                            using (var productSearchClient = new ProductSearchClient())
                            {
                                // 获取产品的标签
                                var productTagResult = await productSearchClient.SelectProductsTagAsync(
                                         new SeachTagRequest
                                         {
                                             PidList = pidArr.ToList(),
                                             SeachTags = new List<ProductTagTypeIn>
                                             {
                                                  ProductTagTypeIn.InstallFree
                                             }
                                         });
                                productTagResult.ThrowIfException();
                                if (!productTagResult.Success)
                                {
                                    Logger.Warn(
                                        $"errorCode:{productTagResult.ErrorCode}-->errorMessage:{productTagResult.ErrorMessage}",
                                        productTagResult.Exception);
                                }

                                // 找到`包安装`标签
                                return productTagResult.Result?
                                         .FirstOrDefault(p => "InstallFree" == p.Key)
                                         .Value?
                                         .ToList() ?? new List<string>();
                            }
                        }, GlobalConstant.productInfoExpiration); // 缓存10分钟
                        if (tagsResult.Success)
                        {
                            if (sw.ElapsedMilliseconds > 100)
                            {
                                Logger.Info($"GetFlashSaleDataAsync 批量查询产品标签数据执行时间=>{sw.ElapsedMilliseconds}");
                                sw.Restart();
                            }
                            return tagsResult.Value;
                        }

                        Logger.Warn($"从缓存中获取产品的标签失败=》{sw.ElapsedMilliseconds};Error:{tagsResult.Message}", tagsResult.Exception);
                        return new List<string>();

                    }, 5));

                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"GetFlashSaleDataAsync 批量查询产品标签数据总执行时间=>{sw.ElapsedMilliseconds}");
                    }
                    sw.Restart();

                    installFreeTagPids = paralleResult.SelectMany(p => p ?? new List<string>()).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error(" FlashSaleManager -> 包安装 标签 -> error ", e.InnerException ?? e);
            }

            // 获取活动和活动产品销量数据
            var casheListResult = await Task.WhenAll(cacheList.Select(async model =>
            {
                using (var hashClient = CacheHelper.CreateHashClient(model.ActivityID.ToString(), TimeSpan.FromDays(30)))
                {
                    var recordCache = await hashClient.GetAllAsync();
                    List<FlashSaleProductModel> cacheProduct;
                    if (!recordCache.Success)
                    {
                        cacheProduct = (await SelectFlashSaleSaleOutQuantityAsync(model.ActivityID)).ToList();
                        Logger.Info($"CreateHashClientError={sw.ElapsedMilliseconds}参数activityID=》{model.ActivityID}=>{recordCache.Message}=>{recordCache.Value?.Count}");
                        if (sw.ElapsedMilliseconds > 100)
                        {
                            Logger.Info($"CreateHashClientError=>DB={sw.ElapsedMilliseconds}参数activityID=》{model.ActivityID}=>{recordCache.Message}");
                            sw.Restart();
                        }
                    }
                    else
                    {
                        if (recordCache.Value != null && recordCache.Value.Any())
                        {
                            cacheProduct = recordCache.Value.Select(r => new FlashSaleProductModel
                            {
                                PID = r.Key,
                                SaleOutQuantity = r.Value.To<int>()
                            }).ToList();
                            if (sw.ElapsedMilliseconds > 100)
                            {
                                Logger.Info($"CreateHashClientr={sw.ElapsedMilliseconds}=>{recordCache.ElapsedMilliseconds}=>参数activityID=》{model.ActivityID}");
                                sw.Restart();
                            }
                        }
                        else
                        {
                            cacheProduct = (await SelectFlashSaleSaleOutQuantityAsync(model.ActivityID)).ToList();
                            if (sw.ElapsedMilliseconds > 100)
                            {
                                Logger.Info($"CreateHashClient=>DB={sw.ElapsedMilliseconds}参数activityID=》{model.ActivityID}=>{recordCache.Message}");
                                sw.Restart();
                            }

                            var dic = new Dictionary<string, object>();
                            foreach (var pidItem in cacheProduct)
                            {
                                dic.Add(pidItem.PID, pidItem.SaleOutQuantity);
                            }

                            IReadOnlyDictionary<string, object> roDic = new ReadOnlyDictionary<string, object>(dic);
                            var increment = await hashClient.SetAsync<IReadOnlyDictionary<string, object>>(roDic);
                            if (!increment.Success)
                            {
                                Logger.Error($"限时抢购初始化redis失败=>GetFlashSaleDataAsync：ActivityId=》{model.ActivityID}");
                            }
                            if (sw.ElapsedMilliseconds > 100)
                            {
                                Logger.Info($"CreateHashClient=>redis={sw.ElapsedMilliseconds}参数activityID=》{model.ActivityID}=>{recordCache.Message}");
                                sw.Restart();
                            }
                        }
                    }

                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"CreateHashClient执行时间=>{sw.ElapsedMilliseconds}参数activityID=》{model.ActivityID}=>{recordCache.Message}");
                        sw.Restart();
                    }

                    return new Tuple<Guid, List<FlashSaleProductModel>>(model.ActivityID, cacheProduct);
                }
            }));

            foreach (var model in cacheList)
            {
                var cacheProduct = casheListResult.FirstOrDefault(p => p.Item1 == model.ActivityID)?.Item2 ?? new List<FlashSaleProductModel>();

                model.Products = (from p1 in model.Products
                                  join p2 in cacheProduct on p1.PID equals p2.PID into temp
                                  from p2 in temp.DefaultIfEmpty()
                                  select new FlashSaleProductModel
                                  {
                                      ActivityID = p1.ActivityID,
                                      FalseOriginalPrice = p1.FalseOriginalPrice,
                                      Channel = p1.Channel,
                                      ImgUrl = p1.ImgUrl,
                                      InstallAndPay = p1.InstallAndPay,
                                      IsJoinPlace = p1.IsJoinPlace,
                                      IsUsePCode = p1.IsUsePCode,
                                      Label = p1.Label,
                                      Level = p1.Level,
                                      MaxQuantity = p1.MaxQuantity,
                                      PID = p1.PID,
                                      PKID = p1.PKID,
                                      Position = p1.Position.GetValueOrDefault(99999),
                                      Price = p1.Price,
                                      ProductImg = p1.ProductImg,
                                      ProductName = p1.ProductName,
                                      SaleOutQuantity = p2?.SaleOutQuantity ?? 0,
                                      IsShow = p1.IsShow,
                                      OnSale = true,
                                      stockout = false,
                                      TotalQuantity = p1.TotalQuantity,
                                      InstallService = (bool)installFreeTagPids?.Contains(p1.PID) ? "包安装" : "",
                                      AdvertiseTitle = p1.AdvertiseTitle,
                                      Brand = p1.Brand
                                  }).OrderBy(o => o.Position).ThenByDescending(O =>
                                  O.SaleOutQuantity / (O.TotalQuantity.GetValueOrDefault(999999) * 1.0));
                flashSaleList.Add(model);

                if (sw.ElapsedMilliseconds > 100)
                {
                    Logger.Info($"foreach组装活动产品数据执行时间=>{sw.ElapsedMilliseconds}，activity=>{model.ActivityID}");
                    sw.Restart();
                }
            }

            if (sw.ElapsedMilliseconds > 100)
            {
                Logger.Info($"foreach组装活动产品数据总执行时间=>{sw.ElapsedMilliseconds}");
            }

            sw.Stop();

            return flashSaleList;
        }

        public async static Task<FlashSaleProductCanBuyCountModel> GetUserCanBuyFlashSaleItemCountAsync(Guid userId, Guid activityId, string pid)
        {
            var result = new FlashSaleProductCanBuyCountModel();
            var allPlaceLimitId = GlobalConstant.AllPlaceLimitId;
            IEnumerable<Guid> forPara = new List<Guid>() { activityId };
            var flashSale = (await GetFlashSaleListAsync(forPara.ToArray())).FirstOrDefault();
            DateTime now = DateTime.Now;
            if (flashSale == null)
                result.Status = CanBuyCountStatus.NoExist;
            else
            {
                var product = flashSale.Products.FirstOrDefault(_ => _.PID == pid);
                if (product == null)
                    result.Status = CanBuyCountStatus.NoExist;
                else if (!(await IsNewMember(userId)) && flashSale.IsNewUserFirstOrder)
                    result.OnlyNewMemberCanBuy = true;
                else if (flashSale.StartDateTime > now)
                    result.Status = CanBuyCountStatus.NotStart;
                else if (flashSale.EndDateTime < now)
                    result.Status = CanBuyCountStatus.End;
                else if (product.MaxQuantity == null && product.TotalQuantity == null)
                    result.Status = CanBuyCountStatus.NoLimit;
                else
                {
                    var records = await DalFlashSale.SelectActivityProductOrderRecordsAsync(userId, null, activityId);
                    //var personalRecord = records.Where(r => r.PID == pid).Sum(r => r.Quantity);
                    Logger.Info($"1,用户{userId}购买活动{activityId}记录{records}");
                    Logger.Info($"2,用户{userId}购买活动{activityId}下产品{pid}记录{records}");
                    int sumRecord = 0;
                    if (flashSale.PlaceQuantity > 0 && product.IsJoinPlace)
                    {
                        var placePids = flashSale.Products.Where(p => p.IsJoinPlace).Select(_ => _.PID);
                        //参与会场限购
                        records = records.Where(r => placePids.Contains(r.PID));
                    }
                    else
                        records = records.Where(r => r.PID == pid);

                    if (records.Any())
                        sumRecord = records.Sum(r => r.Quantity);
                    result.Status = CanBuyCountStatus.Limit;
                    var totalQuantity = product.TotalQuantity ?? 99999;
                    var maxQuantity = product.MaxQuantity ?? 99999;
                    result.Num = Math.Max(0, Math.Min(totalQuantity - sumRecord, maxQuantity - sumRecord));
                    if (flashSale.PlaceQuantity > 0 && product.IsJoinPlace)
                        result.Num = Math.Min(result.Num, flashSale.PlaceQuantity.Value - sumRecord);
                    if (flashSale.ActiveType == 3 && CheckAllPlaceLimitStatrtDate())
                    {
                        var allrecords = await DalFlashSale.SelectAllActivityProductOrderRecordsAsync(userId, null, new Guid(allPlaceLimitId));
                        var allLimitflashSale = (await SelectFlashSaleDataByActivityIDAsync(new Guid(allPlaceLimitId)));
                        var allLimitProduct = allLimitflashSale.Products.FirstOrDefault(_ => _.PID == pid);
                        if (allLimitflashSale.PlaceQuantity > 0 && allLimitProduct != null && allLimitProduct.IsJoinPlace)
                        {
                            var placePids = allLimitflashSale.Products.Where(p => p.IsJoinPlace).Select(_ => _.PID);
                            //参与全局会场限购
                            records = allrecords.Where(r => placePids.Contains(r.PID));
                            var allsumRecord = records.Sum(r => r.Quantity);
                            result.Num = Math.Min(result.Num,
                                Math.Max(allLimitflashSale.PlaceQuantity.Value - allsumRecord, 0));
                        }
                    }
                    Logger.Info($"3,用户{userId}购买活动{activityId}下产品{pid}可购买数量{result.Num}TotalQuantity{product.TotalQuantity}MaxQuantity{product.MaxQuantity}PlaceQuantity{flashSale.PlaceQuantity}sumRecord{sumRecord}");
                }

            }
            return result;
        }


        public async static Task<bool> IsNewMember(Guid userId)
        {
            try
            {
                using (var client = new UserProfileClient())
                {
                    var ret = await client.GetAsync(userId, "CreatedOrderQTY");
                    ret.ThrowIfException(true);
                    int num = 0;
                    int.TryParse(ret.Result, out num);
                    return num <= 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }
        /// <summary>
        /// 查询产品详情页限时抢购详情
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <param name="variantId"></param>
        /// <param name="channel"></param>
        /// <param name="phone"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<FlashSaleProductDetailModel> FetchProductDetailForFlashSaleAsync(string productId, string variantId, string activityId, string channel, string productGroupId, string userId = null, string phone = null)
        {
            Guid aid;
            Guid.TryParse(activityId, out aid);
            if (aid == Guid.Empty)
                return null;
            #region 拼团活动价
            if (await GroupBuyingManager.CheckProductGroupId(aid))
            {
                return await FetchPinTuanProductDetial(productGroupId, productId + "|" + variantId);
            }
            #endregion
            var flashsaleList = await GetFlashSaleListAsync(new Guid[] { aid });
            if (flashsaleList != null)
            {
                var flashSale = flashsaleList.FirstOrDefault();
                if (flashSale?.Products != null && flashSale.Products.Any(_ => _.PID == productId + "|" + variantId))
                {
                    var product = flashSale.Products.FirstOrDefault(_ => _.PID == productId + "|" + variantId);
                    if (product != null)
                    {
                        var result = new FlashSaleProductDetailModel()
                        {
                            ActivityID = flashSale.ActivityID,
                            StartDateTime = flashSale.StartDateTime,
                            ActiveType = flashSale.ActiveType,
                            Channel = product.Channel,
                            TotalQuantity = product.TotalQuantity.GetValueOrDefault(0),
                            SaleOutQuantity = product.SaleOutQuantity,
                            DisplayName = product.ProductName,
                            FalseOriginalPrice = product.FalseOriginalPrice,
                            EndDateTime = flashSale.EndDateTime,
                            InstallAndPay = product.InstallAndPay,
                            Image = product.ProductImg,
                            ImgUrl = product.ImgUrl,
                            IsJoinPlace = product.IsJoinPlace ? 1 : 0,
                            IsUsePcode = product.IsUsePCode,
                            IsUsePCode = product.IsUsePCode,
                            Level = product.Level.GetValueOrDefault(0),
                            Label = product.Label,
                            MaxQuantity = product.MaxQuantity.GetValueOrDefault(0),
                            PID = product.PID,
                            PlaceQuantity = flashSale.PlaceQuantity.GetValueOrDefault(0),
                            Price = product.Price,
                            Position = product.Position,
                        };
                        if (product.Channel != channel && product.Channel != "all")
                        {
                            result.FlashSaleCode = "1"; //渠道不符
                            return result;
                        }
                        if (product.TotalQuantity != null && product.SaleOutQuantity >= product.TotalQuantity.Value)
                        {
                            result.FlashSaleCode = "2"; //已售完
                            return result;
                        }
                        if (flashSale.StartDateTime > DateTime.Now || flashSale.EndDateTime <= DateTime.Now)
                        {
                            result.FlashSaleCode = "3"; //非活动时间
                            return result;
                        }
                        Guid uid;
                        Guid.TryParse(userId, out uid);
                        var canBuyResult = await GetUserCanBuyFlashSaleItemCountAsync(uid, aid, productId + "|" + variantId);
                        // if (uid != Guid.Empty)
                        // {
                        if (canBuyResult != null)
                        {
                            var canBuy = canBuyResult;
                            if (canBuy.OnlyNewMemberCanBuy)
                            {
                                result.FlashSaleCode = "5"; //新人购买
                                return result;
                            }
                            if (canBuy.Status == CanBuyCountStatus.NoLimit)
                            {
                                result.UserCanBuyCount = 9999;
                            }
                            else
                            {
                                result.UserCanBuyCount = canBuy.Num;
                                if (canBuy.Status == CanBuyCountStatus.Limit)
                                {
                                    if (canBuy.Num <= 0)
                                    {
                                        result.FlashSaleCode = "4"; //已限购
                                        return result;
                                    }
                                    else
                                        result.PersonalSaleQuantity = Math.Max(result.MaxQuantity - canBuy.Num, 0);
                                }
                            }
                        }
                        // }
                        result.FlashSaleCode = "0"; //成功
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                    return null;
            }
            else
                return null;

        }


        /// <summary>
        /// 根据取消的订单号维护counter
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>成功或者 失败</returns>
        public static async Task<bool> DecrementCounter(int orderId)
        {
            bool result = true;
            var record = await DalFlashSale.SelectFlashSaleRecordsByOrderIdAsync(orderId);
            if (record != null)
            {
                var activityId = record.ActivityID;

                var activityTypes = ActivityManager.SelectActivityTypeByActivityIds(new List<Guid>() { activityId });

                if (activityTypes != null && activityTypes.Any())
                {
                    var activityType = activityTypes.First();
                    if (activityType.Type == 5)
                    {
                        var round = await ActivityValidator.GetRoundConfig(activityId, DateTime.Now);
                        if (round != null && round.EndTime > DateTime.Now)
                        {
                            var counter = new BaoYangCounter(activityId, 0, 0, round);
                            result = await counter.RemoveOrderRecord(orderId);
                        }
                    }
                }
            }

            return result;
        }

        public static async Task<FixedPriceActivityStatusResult> GetFixedPriceActivityStatus(Guid activityId, Guid userId, int regionId)
        {
            var sw = new Stopwatch();
            sw.Start();
            long createHashClientTime = sw.ElapsedMilliseconds;
            long getBaoYangActivityConfigTime = 0;
            long baoYangCounterTime = 0;
            long getCurrentUserPurchaseCountTime = 0;
            long getCurrentActivityPurchaseCount = 0;
            FixedPriceActivityStatusResult result = new FixedPriceActivityStatusResult();
            if (activityId != Guid.Empty)
            {
                var activityConfig = await ActivityCache.GetBaoYangActivityConfig(activityId);
                getBaoYangActivityConfigTime = sw.ElapsedMilliseconds - createHashClientTime;
                if (activityConfig != null)
                {
                    result.ActivityPrice = activityConfig.PromotionPrice;
                    result.BackgroundImg = activityConfig.BackgroundImg;
                    result.ButtonBackgroundColor = activityConfig.ButtonBackgroundColor;
                    result.ButtonTextColor = activityConfig.ButtonTextColor;
                    result.TipTextColor = activityConfig.TipTextColor;
                    result.OngoingButtonText = activityConfig.OngoingButtonText;

                    var now = DateTime.Now;
                    if (now < activityConfig.StartTime)
                    {
                        result.ActivityStatus = "NotStart";
                        result.ActivityTime = activityConfig.StartTime;
                    }
                    else if (now > activityConfig.EndTime)
                    {
                        result.ActivityStatus = "End";
                    }
                    else
                    {
                        var round = ActivityValidator.GetCurrentRoundConfig(activityConfig, DateTime.Now);
                        if (round != null)
                        {
                            var counter = new BaoYangCounter(activityId, round.LimitedQuantity, activityConfig.ItemQuantityPerUser, round);
                            baoYangCounterTime = sw.ElapsedMilliseconds - getBaoYangActivityConfigTime;
                            var activityRoundLimitCount = round.LimitedQuantity;
                            var userLimitCount = activityConfig.ItemQuantityPerUser;
                            var userPurchaseCount = await counter.GetCurrentUserPurchaseCount(userId.ToString(), "userId");
                            getCurrentUserPurchaseCountTime = sw.ElapsedMilliseconds - baoYangCounterTime;
                            var activityRoundPurchaseCount = await counter.GetCurrentActivityPurchaseCount();
                            getCurrentActivityPurchaseCount = sw.ElapsedMilliseconds - getCurrentUserPurchaseCountTime;
                            if (round != null && userPurchaseCount < userLimitCount && activityRoundPurchaseCount < activityRoundLimitCount)
                            {
                                result.ActivityStatus = "Ing";
                            }
                            else if (round != null && round.EndTime == activityConfig.RoundConfigs.Max(o => o.EndTime)
                                && activityRoundPurchaseCount >= activityRoundLimitCount)
                            {
                                // 是最后一场，切活动场次购买数量大于限购数量
                                result.ActivityStatus = "OutOfStock";
                            }
                            else if (userPurchaseCount >= userLimitCount)
                            {
                                result.ActivityStatus = "UserEnd";
                            }
                            else
                            {
                                // 可以买
                                result.ActivityStatus = "WaitNext";
                                result.ActivityTime = activityConfig.RoundConfigs.Where(o => o.StartTime >= DateTime.Now).Min(o => o.StartTime);
                            }
                        }
                        else
                        {
                            var counter = new BaoYangCounter(activityId, 0, activityConfig.ItemQuantityPerUser, round);
                            baoYangCounterTime = sw.ElapsedMilliseconds - getBaoYangActivityConfigTime;
                            var userLimitCount = activityConfig.ItemQuantityPerUser;
                            var userPurchaseCount = await counter.GetCurrentUserPurchaseCount(userId.ToString(), "userId");
                            getCurrentUserPurchaseCountTime = sw.ElapsedMilliseconds - baoYangCounterTime;
                            // 当前时间无可用场次时
                            if (userPurchaseCount >= userLimitCount)
                            {
                                result.ActivityStatus = "UserEnd";
                            }
                            else
                            {
                                // 可以买
                                result.ActivityStatus = "WaitNext";
                                result.ActivityTime = activityConfig.RoundConfigs.Where(o => o.StartTime >= DateTime.Now).Min(o => o.StartTime);
                            }
                        }
                    }
                }
            }
            Logger.Info($"GetFixedPriceActivityStatus:getBaoYangActivityConfigTime:{getBaoYangActivityConfigTime}," +
                $"baoYangCounterTime:{baoYangCounterTime},getCurrentUserPurchaseCountTime:{getCurrentUserPurchaseCountTime}," +
                $"getCurrentActivityPurchaseCount:{getCurrentActivityPurchaseCount},totalTime:{sw.ElapsedMilliseconds}");
            sw.Stop();
            return result;
        }


        public static async Task<bool> OrderCancerMaintenanceFlashSaleData(int orderId)
        {
            Logger.Info($"开始回滚限购数据OrderId{orderId}");
            bool result = true;
            var records = await DalFlashSale.SelectActivityProductOrderRecordsByOrderIdAsync(orderId);
            var flashSaleRecordsModels = records as FlashSaleRecordsModel[] ?? records.ToArray();
            if (flashSaleRecordsModels.Any())
            {
                var first = flashSaleRecordsModels.First();
                var activityId = first.ActivityID;
                var userId = first.UserID;
                var deviceId = first.DeviceID;
                var userTel = first.Phone;

                var activityTypes = ActivityManager.SelectActivityTypeByActivityIds(new List<Guid>() { activityId });

                if (activityTypes != null && activityTypes.Any())
                {
                    var activityType = activityTypes.First();
                    if (activityType.Type == 1 || activityType.Type == 3)
                    {
                        #region 1;回滚计数器，回撤销售数量
                        var flashRequest = new FlashSaleOrderRequest
                        {
                            UseTel = userTel,
                            DeviceId = deviceId,
                            UserId = userId
                        };
                        var listflashSale = new List<CheckFlashSaleResponseModel>();
                        foreach (var item in flashSaleRecordsModels)
                        {
                            var orderItem = new OrderItems
                            {
                                Num = item.Quantity,
                                PID = item.PID,
                                ActivityId = item.ActivityID
                            };
                            var flashSale = await DalFlashSale.FetchFlashSaleProductModel(orderItem);
                            if (flashSale == null)
                            {
                                Logger.Warn($"取消订单OrderId{orderId}时活动商品已经不存在");
                                return result;
                            }
                            if (flashSale.SaleOutQuantity < item.Quantity)
                            {
                                Logger.Error($"取消订单OrderId{orderId}时活动商品售出数量{flashSale.SaleOutQuantity}小于需要回滚的数量");
                                return result;
                            }
                            var allPlaceLimitId = await DalFlashSale.SelectActivityProductOrderRecordAsync(orderId, item.PID);
                            var checkFlashSaleResponse = new CheckFlashSaleResponseModel
                            {
                                HasPlaceLimit = flashSale.PlaceQuantity.HasValue && flashSale.PlaceQuantity.Value > 0,
                                HasQuantityLimit = flashSale.MaxQuantity.HasValue,
                                Num = item.Quantity,
                                PID = item.PID,
                                ActivityId = item.ActivityID,
                                AllPlaceLimitId = allPlaceLimitId

                            };
                            Logger.Info($"全局会场限购id是{checkFlashSaleResponse.AllPlaceLimitId}");
                            listflashSale.Add(checkFlashSaleResponse);
                            #region 回撤销售数量

                            var tempResult1 = await DalFlashSale.UpdateFlashSaleProducts(orderItem);
                            result = tempResult1 > 0;
                            if (!result)
                            {
                                Logger.Error($"回撤销售数量UpdateFlashSaleProducts失败OrderId{orderId}");
                            }

                            //回滚全场的销售数量，这个数量维护是为了刷新缓存的取值取得这个
                            if (!string.IsNullOrEmpty(allPlaceLimitId))
                            {
                                var tempResult2 = await DalFlashSale.UpdateFlashSaleProducts(orderItem, allPlaceLimitId);
                                result = tempResult2 > 0;
                                if (!result)
                                {
                                    Logger.Error($"回撤全场销售数量UpdateFlashSaleProducts失败OrderId{orderId}");
                                }
                            }
                            #endregion
                        }

                        await FlashSaleCounter.DecrementAllFlashCount(flashRequest, listflashSale);
                        #endregion
                        #region 2;删除日志记录后面修改为逻辑删除

                        var tempResult = await DalFlashSale.DeleteActivityProductOrderRecords(orderId);
                        result = tempResult > 0;
                        if (!result)
                        {
                            Logger.Error($"删除ActivityProductOrderRecords记录失败OrderId{orderId}");
                        }
                        #endregion
                    }
                }
            }
            Logger.Info($"成功回滚限购数据OrderId{orderId}");
            return result;
        }


        public static async Task<bool> RefreshFlashSaleHashCountAsync(List<string> activtyids, bool isAllRefresh)
        {
            var result = true;
            if (isAllRefresh)
            {
                activtyids = (await DalFlashSale.SelectFlashSaleActivtyIds()).ToList();
            }
            foreach (var activtyid in activtyids)
            {
                Guid aid;
                if (Guid.TryParse(activtyid, out aid))
                {
                    var model = await DalFlashSale.SelectFlashSaleFromDBAsync(aid);
                    if (model?.Products != null && model.Products.Any())
                    {
                        foreach (var product in model.Products)
                        {
                            var item = new OrderItems()
                            {
                                ActivityId = product.ActivityID,
                                Num = product.SaleOutQuantity,
                                PID = product.PID,
                                Type = 1
                            };
                            var cacheresult = await FlashSaleCounter.RefreshRedisHashRecord(item.PID, item);
                            if (!cacheresult)
                            {
                                Logger.Error($"刷新限时抢购计数器缓存失败key{item.PID}活动id{item.ActivityId}");
                            }
                            result = result && cacheresult;

                        }
                    }
                }
                else
                {
                    Logger.Error($"刷新限时抢购计数器缓存失败不合法的活动id{activtyid}");
                }

            }
            return result;
        }


        //private static async Task<bool> CheckPinTuanActivity(Guid ActivityId)
        //{
        //    var data = await GroupBuyingManager.CheckProductGroupId(ActivityId);
        //    if (data == null || data.Type != 7)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        private static async Task<FlashSaleProductDetailModel> FetchPinTuanProductDetial(string productGroupId, string pId)
        {
            var data = await GroupBuyingManager.SelectProductInfoByPid(productGroupId, pId);
            if (data != null)
            {
                return new FlashSaleProductDetailModel
                {
                    PID = data.PID,
                    Price = data.SpecialPrice,
                    MarketingPrice = data.OriginalPrice,
                    FalseOriginalPrice = data.FinalPrice,
                    DisplayName = data.ProductName,
                    TotalQuantity = 100,
                    MaxQuantity = GlobalConstant.GroupBuyingProductCount,
                    UserCanBuyCount = GlobalConstant.GroupBuyingProductCount
                };
            }
            return null;
        }
        public static async Task<List<FlashSaleModel>> GetFlashSaleWithoutProductsListAsync(List<Guid> activityIds)
        {
            var list = new List<FlashSaleModel>();
            foreach (var activityId in activityIds)
            {
                var result = await SelectFlashSaleModelByActivityIdAsync(activityId);
                if (result != null)
                {
                    list.Add(result);
                }
            }
            return list;
        }
        public static async Task<FlashSaleModel> SelectFlashSaleModelByActivityIdAsync(Guid activityId)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.GetOrSetAsync(GlobalConstant.FlashSaleCacheKeyPrefix2 + activityId,
                    () => DalFlashSale.SelectFlashSaleModelFromdbAsync(activityId), GlobalConstant.FlashSaleCacheExpiration);
                if (result.Success)
                {
                    return result.Value;
                }
                else
                {
                    return await DalFlashSale.SelectFlashSaleModelFromdbAsync(activityId);
                }
            }
        }

        public static async Task<OrderCountResponse> GetUserCreateFlashOrderCountCacheAsync(OrderCountCacheRequest request)
        {
            return await FlashSaleCounter.GetUserCreateFlashOrderCountCacheAsync(request);
        }
        public static async Task<OrderCountResponse> SetUserCreateFlashOrderCountCacheAsync(OrderCountCacheRequest request)
        {
            return await FlashSaleCounter.SetUserCreateFlashOrderCountCacheAsync(request);
        }
        /// <summary>
        /// 检查缓存中数据是否正确，计划早晨比如八点先跑一次job检查下
        /// </summary>
        /// <returns></returns>
        public static async Task<List<FlashSaleWrongCacheResponse>> SelectFlashSaleWrongCacheAsync()
        {
            var globallist = new List<FlashSaleWrongCacheResponse>();
            var dbAct = await DalActivity.GetAllValidActvivitysAsync();
            var groupdbAct = dbAct.GroupBy(r => r.ActivityID).Select(g => new FlashSaleModel()
            {
                ActivityID = g.Key,
                Products = g.Select(p => new FlashSaleProductModel()
                {
                    PID = p.PID,
                    SaleOutQuantity = p.SaleOutQuantity,
                    TotalQuantity = p.TotalQuantity
                })
            });
            foreach (var db in groupdbAct)
            {
                var innerList = new List<FlashSaleCacheProduct>();
                var model = new FlashSaleWrongCacheResponse();
                using (var hashClient = CacheHelper.CreateHashClient(db.ActivityID.ToString(), TimeSpan.FromDays(30)))
                {
                    var recordCache = await hashClient.GetAllAsync();
                    if (!recordCache.Success)
                    {
                        //todo log

                        Logger.Error("redis执行失败", recordCache.Exception);
                    }
                    else
                    {
                        if (recordCache.Value != null && recordCache.Value.Any())
                        {
                            var cacheResult = recordCache.Value.Select(r => new FlashSaleProductModel
                            {
                                PID = r.Key,
                                SaleOutQuantity = r.Value.To<int>()
                            }).ToList();
                            foreach (var dbP in db.Products)
                            {
                                var cachesaleQty =
                                    cacheResult.Where(r => r.PID == dbP.PID)
                                        .Select(r => r.SaleOutQuantity)
                                        .FirstOrDefault();
                                var logRecordQuantity =
                                                        await DalFlashSale.SelectSumSaleoutQuantityfromLogRecordAsync(
                                                        db.ActivityID.ToString(), dbP.PID) ?? 0;
                                if (cachesaleQty != dbP.SaleOutQuantity)
                                {
                                    innerList.Add(new FlashSaleCacheProduct()
                                    {
                                        CacheQuantity = cachesaleQty,
                                        Pid = dbP.PID,
                                        SaleOutQuantity = dbP.SaleOutQuantity,
                                        TotalQuantity = dbP.TotalQuantity ?? 0,
                                        LogRecordQuantity = logRecordQuantity,
                                        StrType = "CacheInconsistent"
                                    });
                                    // Logger.Warn("缓存跟数据库中数据不一致");
                                }

                                if (dbP.SaleOutQuantity > dbP.TotalQuantity)
                                {
                                    innerList.Add(new FlashSaleCacheProduct()
                                    {
                                        CacheQuantity = cachesaleQty,
                                        Pid = dbP.PID,
                                        SaleOutQuantity = dbP.SaleOutQuantity,
                                        TotalQuantity = dbP.TotalQuantity ?? 0,
                                        LogRecordQuantity = logRecordQuantity,
                                        StrType = "SaleOver"
                                    });
                                }
                                if (dbP.SaleOutQuantity != logRecordQuantity)
                                {
                                    innerList.Add(new FlashSaleCacheProduct()
                                    {
                                        CacheQuantity = cachesaleQty,
                                        Pid = dbP.PID,
                                        SaleOutQuantity = dbP.SaleOutQuantity,
                                        TotalQuantity = dbP.TotalQuantity ?? 0,
                                        LogRecordQuantity = logRecordQuantity,
                                        StrType = "ConfigInconsistent"
                                    });
                                    // Logger.Warn("缓存跟数据库中数据不一致");
                                }
                            }
                            if (innerList.Any())
                            {
                                model.ActiivtyId = db.ActivityID.ToString();
                                model.CacheProducts = innerList;
                                globallist.Add(model);
                            }

                        }
                        else
                        {
                            var dic = new Dictionary<string, object>();
                            foreach (var pidItem in db.Products)
                            {
                                dic.Add(pidItem.PID, pidItem.SaleOutQuantity);
                            }
                            IReadOnlyDictionary<string, object> roDic = new ReadOnlyDictionary<string, object>(dic);
                            var increment = await hashClient.SetAsync<IReadOnlyDictionary<string, object>>(roDic);
                            if (!increment.Success)
                            {
                                Logger.Error($"限时抢购初始化redis失败=>SelectFlashSaleDataByActivityIDAsync：ActivityId=》{db.ActivityID}");
                            }
                        }
                    }
                }

            }
            return globallist;
        }

        /// <summary>
        /// 用户创建订单时候维护销售数据，方便排查问题
        /// </summary>
        /// <param name="flashSale"></param>
        /// <returns></returns>
        public static async Task<bool> OrderCreateMaintenanceFlashSaleDbDataAsync(FlashSaleOrderRequest flashSale)
        {
            var flag = true;
            foreach (var item in flashSale.Products)
            {
                var result = await DalFlashSale.InsertFlashSaleRecordsAsync(item, flashSale.UserId,
                    flashSale.DeviceId, flashSale.UseTel, flashSale.OrderId, flashSale.OrderListId);

                if (result <= 0)
                {
                    Logger.Error($"记录用户购买记录失败{JsonConvert.SerializeObject(flashSale)}");
                    flag = false;
                }

                result = await DalFlashSale.UpdateFlashSaleProductsForPlusAsync(item);
                if (result <= 0)
                {
                    Logger.Error($"更新限购商品销售数量{JsonConvert.SerializeObject(flashSale)}");
                    flag = false;
                }

            }
            return flag;
        }

        public static async Task<bool> UpdateConfigSaleoutQuantityFromLogAsync(UpdateConfigSaleoutQuantityRequest request)
        {
            var result2 = true;
            var dbAct = new List<FlashSaleProductModel>();
            try
            {
                async Task<bool> UpdateDb(List<FlashSaleProductModel> models)
                {
                    var result = true;
                    foreach (var model in models)
                    {
                        var logRecordQuantity = await DalFlashSale.SelectSumSaleoutQuantityfromLogRecordAsync(
                                      model.ActivityID.ToString(), model.PID) ?? 0;
                        var dbResult =
                            await DalFlashSale.UpdateFlashSaleProductsByLogRecordAsync(model.ActivityID.ToString(),
                                model.PID, logRecordQuantity);
                        if (dbResult <= 0)
                        {
                            Logger.Error($"更新限时抢购配置销售数量报错，活动Id{model.ActivityID}产品id{model.PID}");
                        }
                        result = result && dbResult > 0;

                    }
                    return result;
                }

                switch (request.RefreshType)
                {
                    case RefreshType.RefreshAll:
                        dbAct = await DalActivity.GetAllValidActvivitysAsync();
                        result2 = await UpdateDb(dbAct);
                        break;
                    case RefreshType.RefreshByActivityId:
                        dbAct = await DalFlashSale.GetAllValidActvivitysByActivityIdsAsync
                            (request.ProductModels.Select(r => r.ActivityId.ToString()).ToList());
                        result2 = await UpdateDb(dbAct);
                        break;
                    case RefreshType.RefreshByPid:
                        foreach (var pm in request.ProductModels)
                        {
                            dbAct.Add(await DalFlashSale.GetAllValidActvivitysByActivityIdAndPidAsync(pm.ActivityId.ToString(), pm.Pid));
                        }
                        result2 = await UpdateDb(dbAct);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                result2 = false;
                Logger.Error($"更新限时抢购配置销售数量报错", e);
            }
            return result2;
        }


        public static async Task<UserReminderInfo> GetUserReminderInfoAsync(EveryDaySeckillUserInfo model)
        {
            UserReminderInfo userReminderInfo = new UserReminderInfo()
            {
                CanbeReminded = false,
                IsReminded = false
            };
            if (model?.UserId != null)
            {
                DateTime starttime = await Task.Run(() => DalFlashSale.GetActivityStartDateTime(model.FlashActivityGuid));
                var basedatetimeNow = DateTime.Now;
                var ts = (starttime - basedatetimeNow).TotalMinutes;
                if (ts > 10)
                {
                    userReminderInfo.CanbeReminded = true;
                    var EveryDaySeckillUserInfoList = DalFlashSale.GetEveryDaySeckillUserInfoByUserId(model, basedatetimeNow);
                    if (EveryDaySeckillUserInfoList != null && EveryDaySeckillUserInfoList.Any())
                    {
                        userReminderInfo.IsReminded = true;
                    }
                }

            }
            return userReminderInfo;
        }

        public static async Task<InsertEveryDaySeckillUserInfoResponse> InsertEveryDaySeckillUserInfo(EveryDaySeckillUserInfo model)
        {

            InsertEveryDaySeckillUserInfoResponse response = new InsertEveryDaySeckillUserInfoResponse()
            {
                IsInsertSuccess = true,
                Code = 1,
                Message = "记录插入成功"
            };
            string msg = string.Empty;
            if (model.FlashActivityGuid == Guid.Empty || string.IsNullOrWhiteSpace(model.Pid) || model.UserId == Guid.Empty || string.IsNullOrWhiteSpace(model.MobilePhone))
            {
                response.Message = "FlashActivityGuid,Pid,UserId,MobilePhone 这四个字段不允许为空";
                response.Code = -1;
                response.IsInsertSuccess = false;
                return await Task.Run(() => { return response; });
            }
            var result = DalFlashSale.InsertEveryDaySeckillUserInfo(model, out msg);
            if (result > 0)
            {
                Logger.Info($"UserId:{model.UserId},ActivityId:{model.FlashActivityGuid},Pid:{model.Pid},StartTime:{model.FlashActivityStartTime},此记录插入成功");
                FlashSaleService.produceer?.Send("notification.FlashActivityQueue", new EveryDaySeckillUserInfo()
                {
                    MobilePhone = model.MobilePhone,
                    FlashActivityStartTime = model.FlashActivityStartTime,
                    Type = model.Type
                });
            }
            else if (result == -1)
            {
                response.IsInsertSuccess = false;
                response.Code = 0;
                response.Message = "记录已存在,无需插入";
                Logger.Info($"UserId:{model.UserId},ActivityId:{model.FlashActivityGuid},Pid:{model.Pid},StartTime:{model.FlashActivityStartTime},此记录已存在");
            }
            else
            {
                response.IsInsertSuccess = false;
                response.Code = -1;
                response.Message = msg;
                Logger.Info($"UserId:{model.UserId},ActivityId:{model.FlashActivityGuid},Pid:{model.Pid},StartTime:{model.FlashActivityStartTime},此记录在插入时出错");
            }
            return await Task.Run(() => { return response; });
        }

        public static async Task<bool> RefreshSeckillDefaultDataByScheduleAsync(string schedule)
        {
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var data = await DalFlashSale.SelectDefultActivityIdBySchedule(schedule, false);
                var result = await client.SetAsync(GlobalConstant.FlashSaleCacheKeyPrefix + schedule, data, GlobalConstant.SeckillDefaultCacheExpiration);
                if (!result.Success)
                    Logger.Warn($"刷新秒杀default数据失败{GlobalConstant.FlashSaleCacheKeyPrefix + schedule};Error:{result.Message}", result.Exception);
                var dataUpdate = await UpdateFlashSaleCacheAsync(new Guid(data));
                return result.Success && dataUpdate;
            }
        }

        public static async Task<FlashSaleModel> GetSeckillDefaultDataByScheduleAsync(string schedule)
        {
            var activityId = "";
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.GetOrSetAsync(GlobalConstant.FlashSaleCacheKeyPrefix + schedule, () => DalFlashSale.SelectDefultActivityIdBySchedule(schedule, true), GlobalConstant.SeckillDefaultCacheExpiration);
                if (result.Success)
                {
                    activityId = result.Value;
                }
                else
                {
                    Logger.Warn($"获取秒杀default数据失败{GlobalConstant.FlashSaleCacheKeyPrefix + schedule};Error:{result.Message}", result.Exception);
                    activityId = await DalFlashSale.SelectDefultActivityIdBySchedule(schedule, true);
                }
            }
            if (Guid.TryParse(activityId, out Guid id))
            {
                return await SelectFlashSaleDataByActivityIDAsync(id);
            }
            else
            {
                Logger.Info($"秒杀根据场次{schedule}获取的活动id{activityId}不是规则guid");
                return new FlashSaleModel
                {
                    Products = new List<FlashSaleProductModel>()
                };
            }

        }

        public static async Task<Dictionary<string, List<SeckilScheduleInfoRespnose>>> GetSeckillScheduleInfoAsync(
            List<string> pids, DateTime sSchedule, DateTime eSchedule)
        {
            var dbResult = (await DalFlashSale.GetDalSeckillScheudleInfoModels(pids, sSchedule, eSchedule));
            if (dbResult == null || !dbResult.Any())
            {
                return null;
            }
            return dbResult.GroupBy(r => r.Pid)
                .ToDictionary(x => x.Key
                    , y => y.Select(p => new SeckilScheduleInfoRespnose()
                    {
                        ActivityId = p.ActivityId,
                        SeckillPrice = p.SeckillPrice,
                        StartDateTime = p.StartDateTime,
                        EndDateTime = p.EndDateTime,
                        IsUsePCode = p.IsUsePCode
                    }).ToList());
        }

        public static async Task<List<SeckillAvailableStockInfoResponse>> GetSeckillAvailableStockResponseAsync(
            List<SeckillAvailableStockInfoRequest> request)
        {
            var groupRequest = request.GroupBy(r => r.ActivityId).Select(p => new
            {
                ActivityId = p.Key,
                Pids = p.Select(r => r.Pid).ToList()
            });
            var list = new List<SeckillAvailableStockInfoResponse>();
            foreach (var item in groupRequest)
            {
                var flashsale = await SelectFlashSaleDataByActivityIDAsync(item.ActivityId);
                var products = flashsale.Products?.Where(r => item.Pids.Contains(r.PID)).ToList();
                if (products != null && products.Any())
                {
                    list.AddRange(products.Select(r => new SeckillAvailableStockInfoResponse
                    {
                        Pid = r.PID,
                        ActivityId = r.ActivityID,
                        SaleOutQuantity = r.SaleOutQuantity,
                        TotalQuantity = r.TotalQuantity ?? 9999
                    }));
                }
            }
            return list;
        }



        /// <summary>
        /// 根据时间范围查询秒杀场次信息
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FlashSaleModel>> GetSecondsBoysAsync(int activityType = 3, DateTime? startDate = null, DateTime? endDate = null)
        {
            IEnumerable<Guid> ids;

            var flashsales = new List<FlashSaleModel>();

            var prefix = await GlobalConstant.GetKeyPrefix("SecondKillPrefix", DefaultClientName);

            var startTime = Convert.ToDateTime(startDate).ToString("yyyyMMddHHmmss");

            using (var cacheClient = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await cacheClient.GetOrSetAsync($"{prefix}{startTime}",
                    () => DalFlashSale.GetSecondsBoysDataSqlAsync(activityType, startDate, endDate),
                    GlobalConstant.SecondKillTodayCacheExpiration);
                if (result.Success)
                {
                    ids = result.Value;
                }
                else
                {
                    Logger.Warn(
                        $"根据时间范围查询秒杀场次信息redis返回失败GetSecondsBoysAsync:{$"{prefix}{startTime}"};Error:{result.Message}",
                        result.Exception);
                    ids = await DalFlashSale.GetSecondsBoysDataSqlAsync(activityType, startDate, endDate);
                }

            }
            if (ids.Any())
            {
                var result = await GetFlashSaleListAsync(ids.ToArray());
                flashsales = result.OrderBy(D => D.StartDateTime).ToList();
            }

            return flashsales;
        }



        /// 根据活动ID,与PID集合查询对应的活动信息与产品信息
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        public static async Task<List<FlashSaleModel>> SelectFlashSaleDataByActivityIdsAsync(List<FlashSaleDataByActivityRequest> flashSaleDataByActivityRequests)
        {

            var listFlashSaleModel = new List<FlashSaleModel>();

            try
            {
                var taskAsync = await Task.WhenAll(flashSaleDataByActivityRequests.ParallelSelect(
                    p => SelectFlashSaleDataByActivityIDAsync(p.activityID, true), 5).ToList());

                foreach (var item in flashSaleDataByActivityRequests)
                {
                    var flashSaleModel = taskAsync.Where(a => a.ActivityID == item.activityID).FirstOrDefault();

                    if (flashSaleModel != null)
                    {
                        if (item.pids != null)
                        {
                            if (flashSaleModel != null)
                            {
                                flashSaleModel.Products = flashSaleModel.Products.Where(a => item.pids.Contains(a.PID)).ToList();
                            }
                        }

                        listFlashSaleModel.Add(flashSaleModel);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"批量查询活动产品接口异常;Error:{ex.Message}", ex);
            }

            return listFlashSaleModel;
        }

        /// <summary>
        /// 批量获取限时抢购活动、活动商品信息和活动商品销售数量
        /// </summary>
        /// <param name="activityIds"></param>
        /// <returns></returns>
        public static async Task<List<FlashSaleModel>> GetFlashSaleActivityAndProductInfoAsync(List<FlashSaleDataByActivityRequest> requests)
        {
            var activityIds = requests?.Select(x => x.activityID).ToList();

            var flashSaleList = new List<FlashSaleModel>();

            if (!(activityIds?.Count > 0))
            {
                return flashSaleList;
            }

            Stopwatch w = new Stopwatch();
            w.Start();

            try
            {

                flashSaleList = await GetFlashSaleActivityAndProductInfoWithMemCache(activityIds);

                if (w.ElapsedMilliseconds > 100)
                    Logger.Info($"GetFlashSaleActivityAndProductInfoAsync1=>{w.ElapsedMilliseconds}"); w.Stop();

                //2.获取限时抢购活动商品销售数量
                foreach (var item in flashSaleList)
                {
                    //根据请求参数pid,只返回请求的pid
                    var itemRequestPids = requests.FirstOrDefault(x => x.activityID == item.ActivityID)?.pids;

                    if (itemRequestPids?.Count > 0)
                    {
                        item.Products = item.Products?.Where(x => itemRequestPids.Contains(x.PID));
                    }

                    if (item.Products?.Count() > 0)
                    {
                        if (w.ElapsedMilliseconds > 100)
                            Logger.Info($"GetFlashSaleActivityAndProductInfoAsync2=>{w.ElapsedMilliseconds}"); w.Stop();
                        //获取活动商品的销售数量
                        var saleQuantitys = await SelectFlashSaleSaleQuantityAsync(item.ActivityID);
                        if (w.ElapsedMilliseconds > 100)
                            Logger.Info($"GetFlashSaleActivityAndProductInfoAsync3=>{w.ElapsedMilliseconds}"); w.Stop();

                        item.Products = item.Products.Join(saleQuantitys, p1 => p1.PID, p2 => p2.PID, (p1, p2) =>
                        {
                            return new FlashSaleProductModel()
                            {
                                ActivityID = p1.ActivityID,
                                FalseOriginalPrice = p1.FalseOriginalPrice,
                                Channel = p1.Channel,
                                ImgUrl = p1.ImgUrl,
                                InstallAndPay = p1.InstallAndPay,
                                IsJoinPlace = p1.IsJoinPlace,
                                IsUsePCode = p1.IsUsePCode,
                                Label = p1.Label,
                                Level = p1.Level,
                                MaxQuantity = p1.MaxQuantity,
                                PID = p1.PID,
                                PKID = p1.PKID,
                                Position = p1.Position.GetValueOrDefault(99999),
                                Price = p1.Price,
                                ProductImg = p1.ProductImg,
                                ProductName = p1.ProductName,
                                SaleOutQuantity = p2?.SaleOutQuantity ?? 0,
                                IsShow = p1.IsShow,
                                OnSale = true,
                                stockout = false,
                                TotalQuantity = p1.TotalQuantity,
                                InstallService = "",
                                AdvertiseTitle = p1.AdvertiseTitle,
                                Brand = p1.Brand
                            };
                        });

                        if (w.ElapsedMilliseconds > 100)
                            Logger.Info($"GetFlashSaleActivityAndProductInfoAsync4=>{w.ElapsedMilliseconds}"); w.Stop();
                        item.Products = item.Products.OrderBy(x => x.Position);
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetFlashSaleActivityAndProductInfoAsync异常,活动id：{string.Join(",", activityIds)}，ex:{ex}");
            }

            return flashSaleList;
        }

        public static async Task<List<FlashSaleModel>> GetFlashSaleActivityAndProductInfoWithMemCache(List<Guid> activityIds)
        {
            Stopwatch w = new Stopwatch();
            w.Start();

            var memKeys = activityIds.Select(x => $"GetFlashSaleActivityAndProductInfoWithMemCache/{x}");
            var memResults = TuhuMemoryCacheNoJson.Instance.Gets<FlashSaleModel>(memKeys?.ToArray());
            if (w.ElapsedMilliseconds > 100)
                Logger.Info($"GetFlashSaleActivityAndProductInfoWithMemCache1=>{w.ElapsedMilliseconds}"); w.Stop();

            var flashSaleList = new List<FlashSaleModel>();
            flashSaleList.AddRange(memResults?.Where(x => x.Value != null).Select(x => x.Value));

            if (w.ElapsedMilliseconds > 100)
                Logger.Info($"GetFlashSaleActivityAndProductInfoWithMemCache2=>{w.ElapsedMilliseconds}"); w.Stop();

            if (flashSaleList.Count != activityIds.Count)
            {
                var notGetIds = activityIds.Except(flashSaleList.Select(x => x.ActivityID));
                var redisResults = await GetFlashSaleActivityAndProductInfo(notGetIds.ToList());
                foreach (var item in redisResults)
                {
                    await TuhuMemoryCacheNoJson.Instance.GetOrSetAsync($"GetFlashSaleActivityAndProductInfoWithMemCache/{item.ActivityID}", () => Task.FromResult(item), TimeSpan.FromMinutes(5));
                    flashSaleList.Add(item);
                }

                if (w.ElapsedMilliseconds > 100)
                    Logger.Info($"GetFlashSaleActivityAndProductInfoWithMemCache3=>{w.ElapsedMilliseconds}"); w.Stop();
            }

            if (w.ElapsedMilliseconds > 100)
                Logger.Info($"GetFlashSaleActivityAndProductInfoWithMemCache4=>{w.ElapsedMilliseconds}"); w.Stop();

            return flashSaleList;
        }
        public static async Task<List<FlashSaleModel>> GetFlashSaleActivityAndProductInfo(List<Guid> activityIds)
        {
            var flashSaleList = new List<FlashSaleModel>();

            Stopwatch w = new Stopwatch();
            w.Start();

            //1.获取限时抢购活动信息和活动商品信息
            //准备缓存key[cacheKey-activityId]
            var prefix = await GlobalConstant.GetKeyPrefix("SecondKillPrefix", DefaultClientName);
            var cacheKeyDic = activityIds.ToDictionary(x => prefix + x, x => x);
            var cacheKeys = cacheKeyDic.Select(x => x.Key);

            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var allDataCacheResult = await client.GetAsync<FlashSaleModel>(cacheKeys);
                if (w.ElapsedMilliseconds > 200)
                {
                    Logger.Warn($"GetFlashSaleActivityAndProductInfoAsync,批量获取限时抢购活动信息缓存耗时{w.ElapsedMilliseconds},活动id：{string.Join(",", activityIds)}");
                    w.Stop();
                }
                //缓存成功的数据
                var successCacheData = allDataCacheResult.Where(cache => cache.Value.Success)?.Select(x => x.Value.Value);
                if (successCacheData?.Count() > 0)
                {
                    flashSaleList.AddRange(successCacheData);
                }

                Logger.Warn($"GetFlashSaleActivityAndProductInfoAsync2,批量获取限时抢购活动信息缓存耗时{w.ElapsedMilliseconds},活动id：{string.Join(",", activityIds)}");
                w.Stop();

                //缓存失败的重新取并设缓存
                var failCaches = allDataCacheResult.Where(cache => !cache.Value.Success);
                var failCacheKeys = failCaches?.Select(x => x.Key)?.ToList();
                if (failCacheKeys?.Count > 0)
                {
                    //失败的活动id，记录日志 重新获取数据
                    var failActivityIds = new List<Guid>();
                    foreach (var item in failCacheKeys)
                    {
                        cacheKeyDic.TryGetValue(item.ToString(), out Guid activityId);
                        failActivityIds.Add(activityId);
                    }

                    Logger.Warn($"GetFlashSaleActivityAndProductInfoAsync，获取限时抢购活动信息缓存失败，活动id：{string.Join(",", failActivityIds)}");

                    Logger.Warn($"GetFlashSaleActivityAndProductInfoAsync3,批量获取限时抢购活动信息缓存耗时{w.ElapsedMilliseconds},活动id：{string.Join(",", activityIds)}");
                    w.Stop();

                    //获取缓存失败的数据
                    var failActivityList = await DalFlashSale.SelectFlashSaleListFromDBAsync(failActivityIds);
                    var failFlashSaleList = failActivityList?.Item1;

                    Logger.Warn($"GetFlashSaleActivityAndProductInfoAsync4,批量获取限时抢购活动信息缓存耗时{w.ElapsedMilliseconds},活动id：{string.Join(",", activityIds)}");
                    w.Stop();

                    if (failFlashSaleList.Count() > 0 && failActivityList?.Item2?.Count() > 0)
                    {
                        foreach (var key in failCacheKeys)
                        {
                            cacheKeyDic.TryGetValue(key.ToString(), out Guid activityId);
                            var cacheValue = failFlashSaleList.FirstOrDefault(x => x.ActivityID == activityId);
                            if (cacheValue == null)
                            {
                                Logger.Warn($"GetFlashSaleActivityAndProductInfoAsync重新设置失败数据缓存时未获取到活动数据,活动id：{activityId}");
                                continue;
                            }
                            //重新设置活动商品信息缓存
                            cacheValue.Products = failActivityList.Item2.Where(x => x.ActivityID == activityId);
                            var failCacheSetResult = await client.SetAsync(key.ToString(), cacheValue, GlobalConstant.FlashSaleCacheExpiration);

                            flashSaleList.Add(cacheValue);
                        }
                        if (w.ElapsedMilliseconds > 200)
                        {
                            Logger.Warn($"GetFlashSaleActivityAndProductInfoAsync,重新设置失败数据缓存耗时{w.ElapsedMilliseconds},活动id：{string.Join(",", failActivityIds)}");
                        }
                    }
                }
            }

            return flashSaleList;
        }

        /// <summary>
        /// 新活动页活动信息查询内存方法
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        private static async Task<FlashSaleModel> GetFlashSaleDataByActivityIdActivityPageMemory(Guid activityID, List<string> pids)
        {
            var cacheResult = await TuhuMemoryCacheNoJson.Instance.GetOrSetAsync(
                "GetFlashSaleDataByActivityIdActivityPageMemory" + activityID,
                () => GetFlashSaleDataByActivityIdActivityPage(activityID, pids),
                TimeSpan.FromMinutes(1.2));

            return cacheResult;
        }


        /// <summary>
        /// 新活动页活动信息查询
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="excludeProductTags"></param>
        /// <returns></returns>
        public static async Task<FlashSaleModel> GetFlashSaleDataByActivityIdActivityPage(Guid activityID, List<string> pids)
        {
            Stopwatch w = new Stopwatch();
            w.Start();

            FlashSaleModel model = null;

            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var prefix = await GlobalConstant.GetKeyPrefix("SecondKillPrefix", DefaultClientName);

                var result = await client.GetOrSetAsync(prefix + activityID, () => DalFlashSale.GetFlashSaleFromDBActivityPage(activityID, pids), GlobalConstant.FlashSaleCacheExpiration);
                if (result.Success)
                {
                    model = result.Value;
                    if (w.ElapsedMilliseconds > 100)
                        Logger.Info($"GetFlashSaleDataByActivityIdActivityPage=>{w.ElapsedMilliseconds}参数activityID=》{activityID}");
                }

                else
                {
                    model = await DalFlashSale.SelectFlashSaleFromDBAsync(activityID);
                }

                return model;
            }
        }


        /// <summary>
        /// 新活动页活动产品内存查询方法
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        private static async Task<List<FlashSaleProductModel>> GetFlashSaleSaleQuantityActivityPageMemory(Guid activityID, List<string> pids)
        {
            var cacheResult = await TuhuMemoryCacheNoJson.Instance.GetOrSetAsync(
                "GetFlashSaleSaleQuantityActivityPageMemory" + activityID,
                () => GetFlashSaleSaleQuantityActivityPage(activityID, pids),
                TimeSpan.FromSeconds(20));

            return cacheResult;
        }


        /// <summary>
        /// 新活动页活动产品销售数量查询
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public static async Task<List<FlashSaleProductModel>> GetFlashSaleSaleQuantityActivityPage(Guid activityID, List<string> pids)
        {
            Stopwatch w = new Stopwatch();
            w.Start();

            List<FlashSaleProductModel> saleQuantitys;
            using (var hashClient = CacheHelper.CreateHashClient(activityID.ToString(), TimeSpan.FromDays(30)))
            {
                var recordCache = await hashClient.GetAllAsync();
                if (!recordCache.Success)
                {
                    saleQuantitys = (await GetFlashSaleSaleOutQuantityActivityPage(activityID, pids)).ToList();
                    if (w.ElapsedMilliseconds > 100)
                        Logger.Info($"GetFlashSaleSaleQuantityActivityPage1=>{w.ElapsedMilliseconds}参数activityID=》{activityID}");
                }
                else
                {
                    if (recordCache.Value != null && recordCache.Value.Any())
                    {
                        saleQuantitys = recordCache.Value.Select(r => new FlashSaleProductModel
                        {
                            PID = r.Key,
                            SaleOutQuantity = r.Value.To<int>()
                        }).ToList();

                        if (w.ElapsedMilliseconds > 100)
                            Logger.Info($"GetFlashSaleSaleQuantityActivityPage2=>{w.ElapsedMilliseconds}参数activityID=》{activityID}");
                    }
                    else
                    {
                        saleQuantitys = (await GetFlashSaleSaleOutQuantityActivityPage(activityID, pids)).ToList();

                        if (w.ElapsedMilliseconds > 100)
                            Logger.Info($"GetFlashSaleSaleQuantityActivityPage3=>{w.ElapsedMilliseconds}参数activityID=》{activityID}");

                        var dic = new Dictionary<string, object>();
                        foreach (var pidItem in saleQuantitys)
                        {
                            dic.Add(pidItem.PID, pidItem.SaleOutQuantity);
                        }
                        IReadOnlyDictionary<string, object> roDic = new ReadOnlyDictionary<string, object>(dic);
                        var increment = await hashClient.SetAsync<IReadOnlyDictionary<string, object>>(roDic);
                        if (!increment.Success)
                        {
                            Logger.Error($"限时抢购初始化redis失败=>SelectFlashSaleDataByActivityIDAsync：ActivityId=》{activityID}");
                        }
                    }
                }
            }
            return saleQuantitys;
        }



        /// <summary>
        /// 获取限时抢购活动的销售数量
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public static async Task<List<FlashSaleProductModel>> SelectFlashSaleSaleQuantityAsync(Guid activityID)
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            List<FlashSaleProductModel> saleQuantitys;
            using (var hashClient = CacheHelper.CreateHashClient(activityID.ToString(), TimeSpan.FromDays(30)))
            {
                var recordCache = await hashClient.GetAllAsync();
                if (!recordCache.Success)
                {
                    saleQuantitys = (await SelectFlashSaleSaleOutQuantityAsync(activityID)).ToList();
                    Logger.Info($"CreateHashClientError={w.ElapsedMilliseconds}参数activityID=》{activityID}=>{recordCache.Message}=>{recordCache?.Value?.Count()}");
                    if (w.ElapsedMilliseconds > 100)
                        Logger.Info($"CreateHashClientError=>DB={w.ElapsedMilliseconds}参数activityID=》{activityID}=>{recordCache.Message}");
                }
                else
                {
                    if (recordCache.Value != null && recordCache.Value.Any())
                    {
                        saleQuantitys = recordCache.Value.Select(r => new FlashSaleProductModel
                        {
                            PID = r.Key,
                            SaleOutQuantity = r.Value.To<int>()
                        }).ToList();
                        if (w.ElapsedMilliseconds > 100)
                            Logger.Info($"CreateHashClientr={w.ElapsedMilliseconds}=>{recordCache.ElapsedMilliseconds}=>参数activityID=》{activityID}");
                    }
                    else
                    {
                        saleQuantitys = (await SelectFlashSaleSaleOutQuantityAsync(activityID)).ToList();
                        if (w.ElapsedMilliseconds > 100)
                            Logger.Info($"CreateHashClient=>DB={w.ElapsedMilliseconds}参数activityID=》{activityID}=>{recordCache.Message}");
                        var dic = new Dictionary<string, object>();
                        foreach (var pidItem in saleQuantitys)
                        {
                            dic.Add(pidItem.PID, pidItem.SaleOutQuantity);
                        }
                        IReadOnlyDictionary<string, object> roDic = new ReadOnlyDictionary<string, object>(dic);
                        var increment = await hashClient.SetAsync<IReadOnlyDictionary<string, object>>(roDic);
                        if (!increment.Success)
                        {
                            Logger.Error($"限时抢购初始化redis失败=>SelectFlashSaleDataByActivityIDAsync：ActivityId=》{activityID}");
                        }
                    }
                }
            }
            return saleQuantitys;
        }


        /// <summary>
        /// 批量查询活动信息与产品信息使用
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public static async Task<FlashSaleModel> SelectFlashSaleDataByActivityId(Guid activityID)
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            FlashSaleModel model = null;
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.GetOrSetAsync(GlobalConstant.FlashSaleCacheKeyPrefix + activityID, () => DalFlashSale.SelectFlashSaleFromDBAsync(activityID), GlobalConstant.FlashSaleCacheExpiration);
                if (result.Success)
                {
                    model = result.Value;
                    Logger.Info($"从缓存中读执行时间=》{w.ElapsedMilliseconds}参数activityID=》{activityID}");
                }

                else
                {
                    model = await DalFlashSale.SelectFlashSaleFromDBAsync(activityID);
                    if (w.ElapsedMilliseconds > 100)
                        Logger.Info($"SelectFlashSaleFromDBAsync执行时间=>{w.ElapsedMilliseconds}参数activityID=》{activityID}");
                    Logger.Warn($"获取限时抢购redis数据失败SelectFlashSaleDataByActivityIDAsync:{GlobalConstant.FlashSaleCacheKeyPrefix + activityID};Error:{result.Message}", result.Exception);
                }
            }

            return model;
        }

        /// <summary>
        /// 根据场次id和pid集合获取产品销量
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        public static async Task<List<FlashSaleProductModel>> SelectSecKillProductOutQuantityAsync(Guid activityId, string[] pids)
        {
            var sw = new Stopwatch();
            sw.Start();

            List<FlashSaleProductModel> productList;

            using (var hashClient = CacheHelper.CreateHashClient(activityId.ToString(), TimeSpan.FromDays(30)))
            {
                var cache = await hashClient.GetAsync(pids);
                if (!cache.Success)
                {
                    productList = (await SelectFlashSaleSaleOutQuantityAsync(activityId)).ToList();
                    Logger.Info($"获取活动和活动产品销量数据.CreateHashClientError={sw.ElapsedMilliseconds}参数activityID=》{activityId}=>{cache.Message}=>{cache.Value?.Count}");
                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"获取活动和活动产品销量数据.CreateHashClientError=>DB={sw.ElapsedMilliseconds}参数activityID=》{activityId}=>{cache.Message}");
                        sw.Restart();
                    }
                }
                else
                {
                    if (cache.Value != null && cache.Value.Any())
                    {
                        productList = cache.Value.Select(r => new FlashSaleProductModel
                        {
                            PID = r.Key,
                            SaleOutQuantity = r.Value.To<int>()
                        }).ToList();
                        if (sw.ElapsedMilliseconds > 100)
                        {
                            Logger.Info($"获取活动和活动产品销量数据.CreateHashClientr={sw.ElapsedMilliseconds}=>{cache.ElapsedMilliseconds}=>参数activityID=》{activityId}");
                            sw.Restart();
                        }
                    }
                    else
                    {
                        productList = (await SelectFlashSaleSaleOutQuantityAsync(activityId)).ToList();
                        if (sw.ElapsedMilliseconds > 100)
                        {
                            Logger.Info($"获取活动和活动产品销量数据.CreateHashClient=>DB={sw.ElapsedMilliseconds}参数activityID=》{activityId}=>{cache.Message}");
                            sw.Restart();
                        }

                        var dic = productList.ToDictionary<FlashSaleProductModel, string, object>(p => p.PID, p => p.SaleOutQuantity);

                        IReadOnlyDictionary<string, object> roDic = new ReadOnlyDictionary<string, object>(dic);
                        var increment = await hashClient.SetAsync<IReadOnlyDictionary<string, object>>(roDic);
                        if (!increment.Success)
                        {
                            Logger.Error($"获取活动和活动产品销量数据.初始化redis失败=>ActivityId=》{activityId}");
                        }
                        if (sw.ElapsedMilliseconds > 100)
                        {
                            Logger.Info($"获取活动和活动产品销量数据.CreateHashClient=>redis={sw.ElapsedMilliseconds}参数activityID=》{activityId}=>{cache.Message}");
                            sw.Restart();
                        }
                    }
                }

                if (sw.ElapsedMilliseconds > 100)
                {
                    Logger.Info($"获取活动和活动产品销量数据.CreateHashClient执行时间=>{sw.ElapsedMilliseconds}参数activityID=》{activityId}=>{cache.Message}");
                    sw.Restart();
                }
            }
            sw.Stop();

            return productList;
        }

        /// <summary>
        /// 根据活动id分页取出产品数据
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="excludeProductTags"></param>
        /// <param name="isFullPageSize">返回商品数量是否补齐pageSize数</param>
        /// <returns></returns>
        private static async Task<List<FlashSaleProductModel>> SelectSeckillProductDataPagerByActivityId(Guid activityId, int pageIndex, int pageSize, bool excludeProductTags, bool isFullPageSize = false)
        {
            var sw = new Stopwatch();
            sw.Start();
            var pids = new List<string>();

            //1.分页读取上架的活动产品数据,根据 activityId/pageIndex/pageSize 缓存10分钟
            var productList = await GetOnSaleFlashSaleProductPagerWithMorePageAsync(activityId, pageIndex, pageSize, isFullPageSize ? 3 : 0);

            // 2.包安装 标签的列表
            if (productList?.Count > 0)
            {
                pids.AddRange(productList?.Select(p => p.PID) ?? new List<string>());
            }

            var installFreeTagPids = new List<string>();
            if (!excludeProductTags)
            {
                try
                {
                    using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
                    {
                        var pidSort = pids?.OrderBy().Distinct()?.ToList() ?? new List<string>();
                        var paralleResult = await Task.WhenAll(pidSort.Split(50).ParallelSelect(async pid =>
                        {
                            var pidArr = pid as string[] ?? pid.ToArray();
                            var tagsResult = await client.GetOrSetAsync(GlobalConstant.FlashSaleProductInstallTagKeyPrefix + string.Join(",", pidArr).GetHashCode(), async () =>
                            {
                                using (var productSearchClient = new ProductSearchClient())
                                {
                                    // 获取产品的标签
                                    var productTagResult = await productSearchClient.SelectProductsTagAsync(
                                        new SeachTagRequest
                                        {
                                            PidList = pidArr.ToList(),
                                            SeachTags = new List<ProductTagTypeIn>
                                            {
                                                ProductTagTypeIn.InstallFree
                                            }
                                        });
                                    productTagResult.ThrowIfException();
                                    if (!productTagResult.Success)
                                    {
                                        Logger.Warn($"errorCode:{productTagResult.ErrorCode}-->errorMessage:{productTagResult.ErrorMessage}", productTagResult.Exception);
                                    }

                                    // 找到`包安装`标签
                                    return productTagResult.Result?
                                               .FirstOrDefault(p => "InstallFree" == p.Key)
                                               .Value?
                                               .ToList() ?? new List<string>();
                                }
                            }, GlobalConstant.productInfoExpiration); // 缓存10分钟
                            if (tagsResult.Success)
                            {
                                if (sw.ElapsedMilliseconds > 100)
                                {
                                    Logger.Info($"GetFlashSaleDataAsync 批量查询产品标签数据执行时间=>{sw.ElapsedMilliseconds}");
                                    sw.Restart();
                                }
                                return tagsResult.Value;
                            }

                            Logger.Warn($"从缓存中获取产品的标签失败=》{sw.ElapsedMilliseconds};Error:{tagsResult.Message}", tagsResult.Exception);
                            return new List<string>();

                        }, 10));

                        if (sw.ElapsedMilliseconds > 100)
                        {
                            Logger.Info($"GetFlashSaleDataAsync 批量查询产品标签数据总执行时间=>{sw.ElapsedMilliseconds}");
                        }
                        sw.Restart();

                        installFreeTagPids = paralleResult.SelectMany(p => p ?? new List<string>()).ToList();
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(" FlashSaleManager -> 包安装 标签 -> error ", e.InnerException ?? e);
                }
            }

            // 3.获取活动和活动产品销量数据
            var cacheProduct = await SelectSecKillProductOutQuantityAsync(activityId, pids?.ToArray());

            var result = from p1 in productList
                         join p2 in cacheProduct on p1.PID equals p2.PID into temp
                         from p2 in temp.DefaultIfEmpty()
                         select new FlashSaleProductModel
                         {
                             ActivityID = p1.ActivityID,
                             FalseOriginalPrice = p1.FalseOriginalPrice,
                             Channel = p1.Channel,
                             ImgUrl = p1.ImgUrl,
                             InstallAndPay = p1.InstallAndPay,
                             IsJoinPlace = p1.IsJoinPlace,
                             IsUsePCode = p1.IsUsePCode,
                             Label = p1.Label,
                             Level = p1.Level,
                             MaxQuantity = p1.MaxQuantity,
                             PID = p1.PID,
                             PKID = p1.PKID,
                             Position = p1.Position.GetValueOrDefault(99999),
                             Price = p1.Price,
                             ProductImg = p1.ProductImg,
                             ProductName = p1.ProductName,
                             SaleOutQuantity = p2?.SaleOutQuantity ?? 0,
                             IsShow = p1.IsShow,
                             OnSale = p1.OnSale,
                             stockout = p1.stockout,
                             TotalQuantity = p1.TotalQuantity,
                             InstallService = (bool)installFreeTagPids?.Contains(p1.PID) ? "包安装" : "",
                             AdvertiseTitle = p1.AdvertiseTitle,
                             Brand = p1.Brand
                         };

            return result.ToList();
        }

        /// <summary>
        /// 分页获取活动的上架商品,不足pageSize数补齐
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="morePageCount">补齐时,额外递归页数</param>
        /// <returns></returns>
        private static async Task<List<FlashSaleProductModel>> GetOnSaleFlashSaleProductPagerWithMorePageAsync(Guid activityId, int pageIndex, int pageSize, int morePageCount = 0)
        {
            var productList = new List<FlashSaleProductModel>();
            try
            {
                for (int i = 0; i < morePageCount + 1; i++)
                {
                    //从缓存分页获取活动商品
                    var suppleProductList = await GetOrSetCacheOnSaleFlashSaleProductPagerAsync(activityId, pageIndex + i, pageSize);
                    if (suppleProductList.Count > 0)
                    {
                        productList.AddRange(suppleProductList);
                    }
                    if (productList.Count >= pageSize)
                        break;
                }

                //去掉多余
                if (productList.Count > pageSize)
                {
                    productList = productList.Take(pageSize)?.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetOnSaleFlashSaleProductPagerWithMorePageAsync,异常ex:{ex}");
            }
            return productList;
        }

        /// <summary>
        /// 根据活动id/页号/页码 获取/设置活动商品缓存
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param> 
        private static async Task<List<FlashSaleProductModel>> GetOrSetCacheOnSaleFlashSaleProductPagerAsync(Guid activityId, int pageIndex, int pageSize)
        {
            var sw = new Stopwatch();
            sw.Start();
            List<FlashSaleProductModel> productList;

            //根据活动id/页号/页码获取/设置活动商品缓存 10分钟
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var prefixKey = $"{GlobalConstant.FlashSaleProductPagerCacheKeyPrefix}{activityId}/{pageIndex}/{pageSize}";
                var flashCacheResult = await client.GetOrSetAsync(prefixKey, () => DalFlashSale.SelectFlashSaleProductPagerAsync(activityId, true, pageIndex, pageSize),
                    GlobalConstant.HomoSecKillTodayCacheExpiration);

                if (flashCacheResult.Success)
                {
                    productList = flashCacheResult.Value;
                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"分页读取活动产品数据执行时间=>{sw.ElapsedMilliseconds}参数activityID=》{activityId};pageIndex=>{pageIndex};ppageSize=>{pageSize}");
                    }
                }
                else
                {
                    sw.Restart();
                    productList = await DalFlashSale.SelectFlashSaleProductPagerAsync(activityId, true, pageIndex, pageSize);

                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"SelectFlashSaleProductPagerAsync执行时间=>{sw.ElapsedMilliseconds}参数activityID=》{activityId};pageIndex=>{pageIndex};ppageSize=>{pageSize}");
                        sw.Restart();
                    }
                    Logger.Warn($"获取限时抢购redis数据失败GetOrSetCacheOnSaleFlashSaleProductPagerAsync:{prefixKey};Error:{flashCacheResult.Message}", flashCacheResult.Exception);
                }
            }

            //实时筛选上架商品
            if (productList?.Count() > 0)
            {
                //获取商品基本信息
                var productInfos = SelectProductBaseInfo(productList.Select(p => p.PID)?.ToList(), activityId.ToString());
                //给活动商品的Onsale属性赋值
                productList.ForEach(x =>
                {
                    var productInfo = productInfos?.FirstOrDefault(y => y.Pid.ToUpper() == x.PID.ToUpper());
                    x.OnSale = (bool)productInfo?.Onsale;
                    x.stockout = (bool)productInfo?.Stockout;
                });
            }
            //筛选Onsale
            return productList?.Where(x => x.OnSale && x.stockout == false)?.ToList() ?? new List<FlashSaleProductModel>();
        }

        public static async Task<List<FlashSaleModel>> SelectHomeSeckillDataWithMemoryAsync(SelectHomeSecKillRequest request)
        {
            return await SelectHomeSeckillDataAsync(request);
            return await TuhuMemoryCacheNoJson.Instance.GetOrSetAsync(
                string.Join(request.excludeProductTags.ToString(), request.scheduleDate.ToString(), request.topNum),
                () => SelectHomeSeckillDataAsync(request),
                TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// 查询首页天天秒杀数据
        /// </summary>
        /// <param name="request">查询个数</param>
        /// <returns></returns>
        public static async Task<List<FlashSaleModel>> SelectHomeSeckillDataAsync(SelectHomeSecKillRequest request)
        {
            var sw = new Stopwatch();
            sw.Start();

            const int activityType = 1;
            List<FlashSaleModel> flashsales;
            var response = new List<FlashSaleModel>();

            // 首页至少4个产品
            if (request.topNum == 0)
                request.topNum = 4;

            if (request.scheduleDate.ToString() == DateTime.MinValue.ToString())
                request.scheduleDate = DateTime.Now;

            var year = request.scheduleDate.Year;
            var month = request.scheduleDate.Month;
            var day = request.scheduleDate.Day;

            // 取出当日所有活动场次和起止时间数据 缓存10分钟
            var prefix = await GlobalConstant.GetKeyPrefix("HomeSecondKillPrefix", DefaultClientName);
            var prefixKey = $"{prefix}/{year}/{month}/{day}/{activityType}";
            using (var cacheClient = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var cacheResult = await cacheClient.GetOrSetAsync(prefixKey,
                    () => DalFlashSale.SelectSecondKillTodayData(activityType, request.scheduleDate),
                    GlobalConstant.HomoSecKillTodayCacheExpiration);

                if (cacheResult.Success)
                {
                    flashsales = cacheResult.Value;
                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"SelectHomeSeckillDataAsync.{prefixKey}.GetOrSetAsync 执行时间=>{sw.ElapsedMilliseconds}");
                        sw.Restart();
                    }
                }
                else
                {
                    Logger.Warn($"获取当日秒杀场次数据失败.SelectSecondKillTodayData:{prefixKey};Error:{cacheResult.Message}", cacheResult.Exception);
                    flashsales = await DalFlashSale.SelectSecondKillTodayData(activityType, request.scheduleDate);
                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"SelectSecondKillTodayData 执行时间=>{sw.ElapsedMilliseconds}");
                        sw.Restart();
                    }
                }
            }

            // 根据当前时间取出所在场次活动Id
            var model = flashsales?.FirstOrDefault(t => t.StartDateTime <= DateTime.Now && t.EndDateTime >= DateTime.Now);

            if (model == null)
            {
                Logger.Warn($"SelectHomeSeckillDataAsync，未获取到当前活动场次信息");
                return new List<FlashSaleModel>();
            }

            // 根据活动Id和num取出产品数据
            model.Products = await SelectSeckillProductDataPagerByActivityId(model.ActivityID, 1, request.topNum, request.excludeProductTags, true);
            response.Add(model);

            if (sw.ElapsedMilliseconds > 100)
            {
                Logger.Info($"SelectHomeSeckillDataAsync.筛选top产品数据执行时间=>{sw.ElapsedMilliseconds}");
            }
            sw.Stop();

            return response;
        }

        /// <summary>
        /// 根据场次Id获取天天秒杀产品数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<List<FlashSaleProductModel>> SelectSeckillDataByActivityIdAsync(SelectSecKillByIdRequest request)
        {
            var sw = new Stopwatch();
            sw.Start();

            if (request.pageIndex == 0)
                request.pageIndex = 1;

            if (request.pageSize == 0)
                request.pageSize = 10;

            // 根据活动Id和num取出产品数据
            var response = await SelectSeckillProductDataPagerByActivityId(request.activityId, request.pageIndex, request.pageSize, request.excludeProductTags);
            if (sw.ElapsedMilliseconds > 100)
            {
                Logger.Info($"SelectSeckillDataByActivityIdAsync.根据活动Id和num取出产品数据执行时间=>{sw.ElapsedMilliseconds}");
            }
            sw.Stop();
            return response;
        }

        /// <summary>
        /// 天天秒杀数据列表缓存刷新
        /// </summary>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        public static async Task<bool> SpikeListRefreshAsync(Guid activityID)
        {
            Logger.Info($"天天秒杀redis缓存接口调用:{GlobalConstant.FlashSaleCacheKeyPrefix + activityID}");
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var data = await DalFlashSale.SelectSecKillFlashSaleFromDBAsync(activityID, false);
                var data2 = await DalFlashSale.SelectFlashSaleModelFromdbAsync(activityID, false);
                var result = await client.SetAsync(GlobalConstant.FlashSaleCacheKeyPrefix + activityID, data, GlobalConstant.FlashSaleCacheExpiration);
                if (!result.Success)
                    Logger.Warn($"天天秒杀redis缓存失败SpikeListRefreshAsync:{GlobalConstant.FlashSaleCacheKeyPrefix + activityID};Error:{result.Message}", result.Exception);
                var result2 = await client.SetAsync(GlobalConstant.FlashSaleCacheKeyPrefix2 + activityID, data2, GlobalConstant.FlashSaleCacheExpiration);
                if (!result2.Success)
                    Logger.Warn($"天天秒杀redis缓存失败SpikeListRefreshAsync:{GlobalConstant.FlashSaleCacheKeyPrefix2 + activityID};Error:{result2.Message}", result2.Exception);
                return result.Success && result2.Success;
            }
        }

        /// <summary>
        /// 从服务查询商品的基本信息
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        private static List<ProductBaseInfo> SelectProductBaseInfo(List<string> pids, string activityId)
        {
            var resultList = new List<ProductBaseInfo>();
            using (var client = new ProductInfoQueryClient())
            {
                var clientResult = client.SelectProductBaseInfo(pids);
                if (clientResult.Success)
                {
                    resultList = clientResult.Result;
                }
                else
                {
                    Logger.Warn($"SelectProductBaseInfo,查询商品基本信息失败，activityId:{activityId},pids:{string.Join(",", pids)},ErrorMessage：{clientResult.ErrorMessage}");
                }
            }
            return resultList;
        }



        /// <summary>
        /// 活动页秒杀查询最新有效场次
        /// </summary>
        /// <param name="topNumber">场次返回条数</param>
        /// <param name="isProducts">是否查询活动下的产品信息，默认为true</param>
        /// <returns></returns>
        public static async Task<IEnumerable<FlashSaleModel>> GetActivePageSecondKillManager(int topNumber, bool isProducts = true)
        {
            var sw = new Stopwatch();
            sw.Start();

            var flashsales = new List<FlashSaleModel>(); //返回对象
                                                         //通用内存缓存Key
            var prefix = await TuhuMemoryCacheNoJson.Instance.GetOrSetAsync("",
                () => GlobalConstant.GetKeyPrefix("ActivePageSecondKill", DefaultClientName), TimeSpan.FromMinutes(1));

            //场次信息整点时缓存失效
            string hourTime = DateTime.Now.ToString("yyyyMMddHH");
            var prefixKey = $"{prefix}/{topNumber}{hourTime}"; //业务缓存Key

            #region 活动页秒杀场次查询

            try
            {
                using (var cacheClient = CacheHelper.CreateCacheClient(DefaultClientName))
                {
                    var result = await cacheClient.GetOrSetAsync(prefixKey, () => DalFlashSale.GetActivePageSecondKillSession(topNumber), TimeSpan.FromMinutes(5));


                    if (result.Success)
                    {
                        flashsales = result.Value;

                        if (sw.ElapsedMilliseconds > 100)
                        {
                            Logger.Info($"活动页秒杀场次查询{prefixKey}.GetOrSetAsync 执行时间=>{sw.ElapsedMilliseconds}");
                            sw.Restart();
                        }
                    }
                    else
                    {
                        Logger.Warn($"活动页秒杀场次查询redis数据失败GetActivePageSecondKillManager:{prefixKey};Error:{result.Message};ElapsedMilliseconds:{result.ElapsedMilliseconds}", result.Exception);

                        flashsales = await DalFlashSale.GetActivePageSecondKillSession(topNumber);

                        if (sw.ElapsedMilliseconds > 100)
                        {
                            Logger.Info($"活动页秒杀场次查询DB 执行时间=>{sw.ElapsedMilliseconds}");
                            sw.Restart();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetActivePageSecondKillManager活动页秒杀场次查询异常ErreMessage:{ex.Message};堆栈:{ex.StackTrace}");
            }

            #endregion

            var guids = flashsales.Select(a => a.ActivityID);

            if (guids.Any())
            {
                if (isProducts)
                {
                    try
                    {
                        var result = await GetActivePageSecondKillFlashSaleDataAsync(guids.ToArray());
                        flashsales = result.OrderBy(d => d.StartDateTime).ToList();
                        if (sw.ElapsedMilliseconds > 100)
                        {
                            Logger.Info($"GetFlashSaleDataAsync 执行时间=>{sw.ElapsedMilliseconds}");
                            sw.Restart();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"GetActivePageSecondKillManager活动页秒杀产品查询异常ErreMessage:{ex.Message};堆栈:{ex.StackTrace}");
                    }
                }

            }

            if (sw.ElapsedMilliseconds > 100)
                Logger.Info($"GetActivePageSecondKillManager 执行时间=>{sw.ElapsedMilliseconds}");

            sw.Stop();
            return flashsales;
        }


        /// <summary>
        /// 活动页秒杀查询产品方法
        /// </summary>
        /// <param name="activityIds">活动ID集合</param>
        /// <returns></returns>

        private static async Task<List<FlashSaleModel>> GetActivePageSecondKillFlashSaleDataAsync(Guid[] activityIds)
        {
            // 1.循环activityIds读取缓存
            // 2.循环没有命中缓存的数据 set 缓存
            // 3.分批查询产品标签数据

            var flashSaleList = new List<FlashSaleModel>(); //返回对象
            var cacheList = new List<FlashSaleModel>();
            var needCacheList = new List<Guid>();
            var pids = new List<string>();

            var sw = new Stopwatch();
            sw.Start();


            #region 活动产品信息获取
            if (activityIds.Any())
            {
                using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
                {
                    var keyDicList = activityIds.ToDictionary(p => GlobalConstant.FlashSaleCacheKeyPrefix + p, p => p);

                    var resultList = await client.GetAsync<FlashSaleModel>(keyDicList.Select(p => p.Key).ToList());

                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"GetFlashSaleDataAsync 批量Get活动缓存数据执行时间=>{sw.ElapsedMilliseconds}");
                        sw.Restart();
                    }

                    resultList.ForEach(p =>
                    {
                        if (p.Value.Success)
                        {
                            cacheList.Add(p.Value.Value);
                            if (p.Value.Value.Products.Any())
                            {
                                pids.AddRange(p.Value.Value.Products.Select(product => product.PID));
                            }
                        }
                        else
                        {
                            needCacheList.Add(keyDicList[p.Key]);
                            Logger.Info($"从缓存中获取天天秒杀数据失败=》{sw.ElapsedMilliseconds}参数activityID=》{keyDicList[p.Key]};Error:{p.Value.Message}", p.Value.Exception);
                        }
                    });

                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"GetFlashSaleDataAsync ForEach活动及产品数据执行时间=>{sw.ElapsedMilliseconds}");
                        sw.Restart();
                    }
                }
            }


            #endregion
            // 缓存活动数据
            if (needCacheList.Any())
            {
                var newFlashSaleList = await DalFlashSale.SelectFlashSaleFromDbByActivityIdsAsync(needCacheList.ToArray());

                if (sw.ElapsedMilliseconds > 100)
                {
                    Logger.Info($"GetFlashSaleDataAsync 批量查询活动数据执行时间=>{sw.ElapsedMilliseconds}");
                    sw.Restart();
                }

                using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
                {
                    foreach (var flashSale in newFlashSaleList)
                    {

                        var cacheFlashSaleResult = await client.GetOrSetAsync(GlobalConstant.FlashSaleCacheKeyPrefix + flashSale.ActivityID, () => DalFlashSale.SelectFlashSaleFromDBAsync(flashSale.ActivityID), GlobalConstant.FlashSaleCacheExpiration);

                        if (cacheFlashSaleResult.Success)
                        {
                            cacheList.Add(cacheFlashSaleResult.Value);
                            if (cacheFlashSaleResult.Value.Products.Any())
                            {
                                pids.AddRange(cacheFlashSaleResult.Value.Products.Select(product => product.PID));
                            }
                        }
                        else
                        {
                            var singleFlashSale = await DalFlashSale.SelectFlashSaleFromDBAsync(flashSale.ActivityID);

                            if (singleFlashSale != null)
                            {
                                cacheList.Add(singleFlashSale);
                                if (singleFlashSale.Products.Any())
                                    pids.AddRange(singleFlashSale.Products.Select(product => product.PID));
                            }
                            Logger.Warn($"从缓存中获取天天秒杀数据失败=》{sw.ElapsedMilliseconds}参数activityID=》{flashSale.ActivityID};Error:{cacheFlashSaleResult.Message}", cacheFlashSaleResult.Exception);
                        }

                        if (sw.ElapsedMilliseconds > 100)
                        {
                            Logger.Info($"GetFlashSaleDataAsync 循环需要缓存活动数据执行时间=>{sw.ElapsedMilliseconds}");
                            sw.Restart();
                        }
                    }

                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"循环需要缓存活动数据的总执行时间=>{sw.ElapsedMilliseconds}");
                        sw.Restart();
                    }
                }
            }



            // 获取活动和活动产品销量数据
            var casheListResult = await Task.WhenAll(cacheList.Select(async model =>
            {
                using (var hashClient = CacheHelper.CreateHashClient(model.ActivityID.ToString(), TimeSpan.FromDays(30)))
                {
                    var recordCache = await hashClient.GetAllAsync();
                    List<FlashSaleProductModel> cacheProduct;
                    if (!recordCache.Success)
                    {
                        cacheProduct = (await SelectFlashSaleSaleOutQuantityAsync(model.ActivityID)).ToList();
                        Logger.Info($"CreateHashClientError={sw.ElapsedMilliseconds}参数activityID=》{model.ActivityID}=>{recordCache.Message}=>{recordCache.Value?.Count}");
                        if (sw.ElapsedMilliseconds > 100)
                        {
                            Logger.Info($"CreateHashClientError=>DB={sw.ElapsedMilliseconds}参数activityID=》{model.ActivityID}=>{recordCache.Message}");
                            sw.Restart();
                        }
                    }
                    else
                    {
                        if (recordCache.Value != null && recordCache.Value.Any())
                        {
                            cacheProduct = recordCache.Value.Select(r => new FlashSaleProductModel
                            {
                                PID = r.Key,
                                SaleOutQuantity = r.Value.To<int>()
                            }).ToList();
                            if (sw.ElapsedMilliseconds > 100)
                            {
                                Logger.Info($"CreateHashClientr={sw.ElapsedMilliseconds}=>{recordCache.ElapsedMilliseconds}=>参数activityID=》{model.ActivityID}");
                                sw.Restart();
                            }
                        }
                        else
                        {
                            cacheProduct = (await SelectFlashSaleSaleOutQuantityAsync(model.ActivityID)).ToList();
                            if (sw.ElapsedMilliseconds > 100)
                            {
                                Logger.Info($"CreateHashClient=>DB={sw.ElapsedMilliseconds}参数activityID=》{model.ActivityID}=>{recordCache.Message}");
                                sw.Restart();
                            }

                            var dic = new Dictionary<string, object>();
                            foreach (var pidItem in cacheProduct)
                            {
                                dic.Add(pidItem.PID, pidItem.SaleOutQuantity);
                            }

                            IReadOnlyDictionary<string, object> roDic = new ReadOnlyDictionary<string, object>(dic);
                            var increment = await hashClient.SetAsync<IReadOnlyDictionary<string, object>>(roDic);
                            if (!increment.Success)
                            {
                                Logger.Error($"限时抢购初始化redis失败=>GetFlashSaleDataAsync：ActivityId=》{model.ActivityID}");
                            }
                            if (sw.ElapsedMilliseconds > 100)
                            {
                                Logger.Info($"CreateHashClient=>redis={sw.ElapsedMilliseconds}参数activityID=》{model.ActivityID}=>{recordCache.Message}");
                                sw.Restart();
                            }
                        }
                    }

                    if (sw.ElapsedMilliseconds > 100)
                    {
                        Logger.Info($"CreateHashClient执行时间=>{sw.ElapsedMilliseconds}参数activityID=》{model.ActivityID}=>{recordCache.Message}");
                        sw.Restart();
                    }

                    return new Tuple<Guid, List<FlashSaleProductModel>>(model.ActivityID, cacheProduct);
                }
            }));

            foreach (var model in cacheList)
            {
                var cacheProduct = casheListResult.FirstOrDefault(p => p.Item1 == model.ActivityID)?.Item2 ?? new List<FlashSaleProductModel>();

                model.Products = (from p1 in model.Products
                                  join p2 in cacheProduct on p1.PID equals p2.PID into temp
                                  from p2 in temp.DefaultIfEmpty()
                                  select new FlashSaleProductModel
                                  {
                                      ActivityID = p1.ActivityID,
                                      FalseOriginalPrice = p1.FalseOriginalPrice,
                                      Channel = p1.Channel,
                                      ImgUrl = p1.ImgUrl,
                                      InstallAndPay = p1.InstallAndPay,
                                      IsJoinPlace = p1.IsJoinPlace,
                                      IsUsePCode = p1.IsUsePCode,
                                      Label = p1.Label,
                                      Level = p1.Level,
                                      MaxQuantity = p1.MaxQuantity,
                                      PID = p1.PID,
                                      PKID = p1.PKID,
                                      Position = p1.Position.GetValueOrDefault(99999),
                                      Price = p1.Price,
                                      ProductImg = p1.ProductImg,
                                      ProductName = p1.ProductName,
                                      SaleOutQuantity = p2?.SaleOutQuantity ?? 0,
                                      IsShow = p1.IsShow,
                                      OnSale = true,
                                      stockout = false,
                                      TotalQuantity = p1.TotalQuantity,
                                      InstallService = "",
                                      AdvertiseTitle = p1.AdvertiseTitle,
                                      Brand = p1.Brand
                                  }).OrderBy(o => o.Position).ThenByDescending(O =>
                                  O.SaleOutQuantity / (O.TotalQuantity.GetValueOrDefault(999999) * 1.0));
                flashSaleList.Add(model);

                if (sw.ElapsedMilliseconds > 100)
                {
                    Logger.Info($"foreach组装活动产品数据执行时间=>{sw.ElapsedMilliseconds}，activity=>{model.ActivityID}");
                    sw.Restart();
                }
            }

            if (sw.ElapsedMilliseconds > 100)
            {
                Logger.Info($"foreach组装活动产品数据总执行时间=>{sw.ElapsedMilliseconds}");
            }

            sw.Stop();

            return flashSaleList;
        }


        /// <summary>
        /// 新活动页查询活动信息接口
        /// </summary>
        /// <param name="activityIds"></param>
        /// <returns></returns>
        public static async Task<List<FlashSaleActivityPageModel>> GetFlashSaleDataActivityPageByIdsAsync(List<FlashSaleActivityPageRequest> requests)
        {
            var activityIds = requests?.Select(x => x.activityID)?.Distinct()?.ToList();

            var flashSaleList = new List<FlashSaleActivityPageModel>();

            if (!(activityIds?.Count > 0))
            {
                return flashSaleList;
            }

            Stopwatch w = new Stopwatch();
            w.Start();

            try
            {
                flashSaleList = await GetFlashSaleProductActivityPageWithMemCache(activityIds);

                if (w.ElapsedMilliseconds > 100)
                {
                    Logger.Info($"GetFlashSaleDataActivityPageByIdsAsync1=>{w.ElapsedMilliseconds}");
                    w.Restart();
                }

                //2.获取限时抢购活动商品销售数量
                foreach (var item in flashSaleList)
                {
                    //根据请求参数pid,只返回请求的pid
                    var itemRequestPids = requests.FirstOrDefault(x => x.activityID == item.ActivityID)?.pids;

                    if (itemRequestPids?.Count > 0)
                    {
                        item.Products = item.Products?.Where(x => itemRequestPids.Contains(x.PID));
                    }

                    if (item.Products?.Count() > 0)
                    {
                        //获取活动商品的销售数量
                        var saleQuantitys = await GetFlashSaleSaleQuantityActivityPageWithMemCache(item.ActivityID);
                        if (w.ElapsedMilliseconds > 100)
                        {
                            Logger.Info($"GetFlashSaleActivityAndProductInfoAsync2=>{w.ElapsedMilliseconds}");
                            w.Restart();
                        }

                        item.Products = item.Products.Join(saleQuantitys, p1 => p1.PID, p2 => p2.PID, (p1, p2) =>
                        {
                            return new FlashSaleActivityPageProductModel()
                            {
                                ActivityID = p1.ActivityID,
                                Channel = p1.Channel,
                                IsUsePCode = p1.IsUsePCode,
                                MaxQuantity = p1.MaxQuantity,
                                PID = p1.PID,
                                PKID = p1.PKID,
                                Position = p1.Position.GetValueOrDefault(99999),
                                Price = p1.Price,
                                ProductName = p1.ProductName,
                                SaleOutQuantity = p2?.SaleOutQuantity ?? 0,
                                TotalQuantity = p1.TotalQuantity,
                                IsHide = p1.IsHide
                            };
                        });

                        if (w.ElapsedMilliseconds > 100)
                        {
                            Logger.Info($"GetFlashSaleDataActivityPageByIdsAsync3=>{w.ElapsedMilliseconds}");
                        }
                        w.Stop();

                        item.Products = item.Products.OrderBy(x => x.Position);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetFlashSaleDataActivityPageByIdsAsync异常,活动id：{string.Join(",", activityIds)}，ex:{ex}");
            }

            return flashSaleList;
        }


        /// <summary>
        /// 从内存缓存获取活动、活动商品信息
        /// </summary>
        /// <param name="activityIds"></param>
        /// <returns></returns>
        public static async Task<List<FlashSaleActivityPageModel>> GetFlashSaleProductActivityPageWithMemCache(List<Guid> activityIds)
        {
            var flashSaleList = new List<FlashSaleActivityPageModel>();
            Stopwatch w = new Stopwatch();
            w.Start();

            //从内存缓存获取活动、活动商品信息
            var memKeys = activityIds.Select(x => $"GetFlashSaleProductActivityPageWithMemCache/{x}");
            var memResults = TuhuMemoryCacheNoJson.Instance.Gets<FlashSaleActivityPageModel>(memKeys?.ToArray());

            var memCacheData = memResults?.Where(x => x.Value != null)?.Select(x => x.Value)?.ToList();
            if (memCacheData?.Count > 0)
            {
                flashSaleList.AddRange(memCacheData);
            }

            if (flashSaleList.Count != activityIds.Count)
            {
                //未从内存缓存获取的活动数据 重新获取 设置到内存缓存
                var notGetIds = activityIds.Except(flashSaleList.Select(x => x.ActivityID));
                var redisResults = await GetFlashSaleProductActivityPageInfo(notGetIds.ToList());
                foreach (var item in redisResults)
                {
                    await TuhuMemoryCacheNoJson.Instance.GetOrSetAsync($"GetFlashSaleActivityAndProductInfoWithMemCache/{item.ActivityID}", () => Task.FromResult(item), TimeSpan.FromMinutes(5));
                    flashSaleList.Add(item);
                }
            }

            if (w.ElapsedMilliseconds > 100)
            {
                Logger.Info($"GetFlashSaleProductActivityPageWithMemCache4=>{w.ElapsedMilliseconds}");
            }
            w.Stop();

            return flashSaleList;
        }

        /// <summary>
        /// (活动页)批量查询活动、活动商品信息 - 缓存1天 key:SecondKillPrefix + activityId
        /// </summary>
        /// <param name="activityIds"></param>
        /// <returns></returns>
        public static async Task<List<FlashSaleActivityPageModel>> GetFlashSaleProductActivityPageInfo(List<Guid> activityIds)
        {
            var flashSaleList = new List<FlashSaleActivityPageModel>();

            Stopwatch w = new Stopwatch();
            w.Start();

            //1.获取限时抢购活动信息和活动商品信息
            //准备缓存key[cacheKey-activityId]
            var prefix = await GlobalConstant.GetKeyPrefix("SecondKillPrefix", DefaultClientName);
            var cacheKeyDic = activityIds.ToDictionary(x => prefix + x, x => x);
            var cacheKeys = cacheKeyDic.Select(x => x.Key);

            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var allDataCacheResult = await client.GetAsync<FlashSaleActivityPageModel>(cacheKeys);
                if (w.ElapsedMilliseconds > 200)
                {
                    Logger.Warn($"GetFlashSaleProductActivityPageInfo,批量获取限时抢购活动信息缓存耗时{w.ElapsedMilliseconds},活动id：{string.Join(",", activityIds)}");
                    w.Restart();
                }

                //缓存成功的数据
                var successCacheData = allDataCacheResult.Where(cache => cache.Value.Success)?.Select(x => x.Value.Value);
                if (successCacheData?.Count() > 0)
                {
                    flashSaleList.AddRange(successCacheData);
                }

                //缓存失败的重新取并设缓存
                var failCaches = allDataCacheResult.Where(cache => !cache.Value.Success);
                var failCacheKeys = failCaches?.Select(x => x.Key)?.ToList();
                if (failCacheKeys?.Count > 0)
                {
                    //失败的活动id，记录日志 重新获取数据
                    var failActivityIds = new List<Guid>();
                    foreach (var item in failCacheKeys)
                    {
                        cacheKeyDic.TryGetValue(item.ToString(), out Guid activityId);
                        failActivityIds.Add(activityId);
                    }

                    //获取缓存失败的数据
                    var failActivityList = await DalFlashSale.GetFlashSaleListActivitPageAsync(failActivityIds);
                    var failFlashSaleList = failActivityList?.Item1;

                    if (failFlashSaleList.Count() > 0 && failActivityList?.Item2?.Count() > 0)
                    {
                        foreach (var key in failCacheKeys)
                        {
                            cacheKeyDic.TryGetValue(key.ToString(), out Guid activityId);
                            var cacheValue = failFlashSaleList.FirstOrDefault(x => x.ActivityID == activityId);
                            if (cacheValue == null)
                            {
                                Logger.Warn($"GetFlashSaleProductActivityPageInfo重新设置失败数据缓存时未获取到活动数据,活动id：{activityId}");
                                continue;
                            }
                            //重新设置活动商品信息缓存
                            cacheValue.Products = failActivityList.Item2.Where(x => x.ActivityID == activityId);
                            var failCacheSetResult = await client.SetAsync(key.ToString(), cacheValue, GlobalConstant.FlashSaleCacheExpiration);

                            flashSaleList.Add(cacheValue);
                        }

                        if (w.ElapsedMilliseconds > 200)
                        {
                            Logger.Warn($"GetFlashSaleProductActivityPageInfo,重新设置失败数据缓存耗时{w.ElapsedMilliseconds},活动id：{string.Join(",", failActivityIds)}");
                        }
                        w.Stop();
                    }
                }
            }

            return flashSaleList;
        }

        /// <summary>
        /// 新活动页活动产品内存查询方法
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        private static async Task<List<FlashSaleActivityPageProductModel>> GetFlashSaleSaleQuantityActivityPageWithMemCache(Guid activityID)
        {
            var cacheResult = await TuhuMemoryCacheNoJson.Instance.GetOrSetAsync(
                "GetFlashSaleSaleQuantityActivityPageWithMemCache" + activityID,
                () => GetFlashSaleSaleQuantityActivityPage(activityID),
                TimeSpan.FromMinutes(3));

            return cacheResult;
        }



        /// <summary>
        /// 新活动页活动的销售数量
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public static async Task<List<FlashSaleActivityPageProductModel>> GetFlashSaleSaleQuantityActivityPage(Guid activityID)
        {
            Stopwatch w = new Stopwatch();
            w.Start();

            List<FlashSaleActivityPageProductModel> saleQuantitys;

            using (var hashClient = CacheHelper.CreateHashClient(activityID.ToString(), TimeSpan.FromDays(30)))
            {
                var recordCache = await hashClient.GetAllAsync();
                if (!recordCache.Success)
                {
                    saleQuantitys = (await GetFlashSaleSaleQuantityActivityPageMemory(activityID)).ToList();

                    if (w.ElapsedMilliseconds > 100)
                        Logger.Info($"GetFlashSaleSaleQuantityActivityPageCreateHashClientError=>DB={w.ElapsedMilliseconds}参数activityID=》{activityID}=>{recordCache.Message}");
                }
                else
                {
                    if (recordCache.Value != null && recordCache.Value.Any())
                    {
                        saleQuantitys = recordCache.Value.Select(r => new FlashSaleActivityPageProductModel
                        {
                            PID = r.Key,
                            SaleOutQuantity = r.Value.To<int>()
                        }).ToList();

                        if (w.ElapsedMilliseconds > 100)
                            Logger.Info($"GetFlashSaleSaleQuantityActivityPageCreateHashClientr={w.ElapsedMilliseconds}=>{recordCache.ElapsedMilliseconds}=>参数activityID=》{activityID}");
                    }
                    else
                    {
                        saleQuantitys = (await GetFlashSaleSaleQuantityActivityPageMemory(activityID)).ToList();

                        var dic = new Dictionary<string, object>();
                        foreach (var pidItem in saleQuantitys)
                        {
                            dic.Add(pidItem.PID, pidItem.SaleOutQuantity);
                        }
                        IReadOnlyDictionary<string, object> roDic = new ReadOnlyDictionary<string, object>(dic);
                        var increment = await hashClient.SetAsync<IReadOnlyDictionary<string, object>>(roDic);

                        if (w.ElapsedMilliseconds > 100)
                            Logger.Info($"GetFlashSaleSaleQuantityActivityPageCreateHashClient=>DB={w.ElapsedMilliseconds}参数activityID=》{activityID}=>{recordCache.Message}");
                    }
                }
            }
            return saleQuantitys;
        }



        /// <summary>
        /// 新活动页活动销量查询
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<FlashSaleActivityPageProductModel>> GetFlashSaleSaleQuantityActivityPageMemory(Guid activityId)
        {
            return await TuhuMemoryCacheNoJson.Instance.GetOrSetAsync("GetFlashSaleSaleQuantityActivityPageMemory/" + activityId,
                () => DalFlashSale.GetFlashSaleSaleQuantityActivityPage(activityId),
                GlobalConstant.FlashSaleSaleQuantityCacheExpiration);
        }
    }
}
