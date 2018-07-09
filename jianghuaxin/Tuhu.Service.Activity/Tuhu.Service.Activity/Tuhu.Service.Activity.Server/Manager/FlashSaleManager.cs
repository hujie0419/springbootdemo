using Common.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public static async Task<FlashSaleModel> SelectFlashSaleDataByActivityIDAsync(Guid activityID)
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
                    model.Products = (from p1 in model.Products
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
                                          InstallService = p1.InstallService,
                                          AdvertiseTitle = p1.AdvertiseTitle,
                                          Brand = p1.Brand
                                      }).OrderBy(O => O.Position).ThenByDescending(O => O.SaleOutQuantity / (O.TotalQuantity.GetValueOrDefault(999999) * 1.0));
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

        private static async Task<IEnumerable<FlashSaleProductModel>> SelectFlashSaleSaleOutQuantityAsync(Guid activityID)
            => await TuhuMemoryCache.Instance.GetOrSetAsync("saleOutCount/" + activityID,
                () => DalFlashSale.SelectFlashSaleSaleOutQuantityAsync(activityID),
                GlobalConstant.FlashSaleSaleQuantityCacheExpiration);

        public async static Task<IEnumerable<FlashSaleModel>> SelectSecondKillTodayDataAsync(int activityType,
            DateTime? scheduleDate = null, bool needProducts = true)
        {
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
            var prefix =await GlobalConstant.GetKeyPrefix("SecondKillPrefix", DefaultClientName);
            using (var cacheClient = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await cacheClient.GetOrSetAsync($"{prefix}{year}/{month}/{day}/{activityType}",
                    () => DalFlashSale.SelectSecondKillTodayDataSqlAsync(activityType, scheduleDate.Value),
                    GlobalConstant.SecondKillTodayCacheExpiration);
                if (result.Success)
                {
                    ids = result.Value;
                }
                else
                {
                    Logger.Warn(
                        $"获取当日秒杀产品redis数据失败SelectSecondKillTodayDataAsync:{$"{prefix}{year}/{month}/{day}/{activityType}"};Error:{result.Message}",
                        result.Exception);
                    ids = await DalFlashSale.SelectSecondKillTodayDataSqlAsync(activityType, scheduleDate.Value);
                }

            }
            if (ids.Any())
            {
                if (needProducts)
                {
                    var result = await GetFlashSaleListAsync(ids.ToArray());
                    flashsales = result.OrderBy(D => D.StartDateTime).ToList();
                }
                else
                {
                    var result = await GetFlashSaleWithoutProductsListAsync(ids.ToList());
                    flashsales = result.OrderBy(D => D.StartDateTime).ToList();
                }
            }
            if (activityType == 1 && ids.Count() < 5)
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
                            StartTime = fs.StartDateTime
                        };
            var notNewMember = !(await IsNewMember(request.UserID));
            var result = query.Select(_ =>
            {
                return new FlashSaleProductBuyLimitModel()
                {
                    ActivityID = _.ActivityID,
                    PID = _.PID,
                    OnlyNewMemberCanBuy = _.IsNewUserFirstOrder && notNewMember,
                    Type = _.StartTime > DateTime.Now ? FlashSaleProductLimitType.NotStart :
                                (_.EndTime <= DateTime.Now ? FlashSaleProductLimitType.End :
                                (_.OverplusQuantity != null && _.OverplusQuantity.Value - _.BuyQuantity < 0 ? FlashSaleProductLimitType.TotalLimit :
                                (_.SingleQuantity != null && _.SingleQuantity.Value - _.RecordQuantity - _.BuyQuantity < 0 ? FlashSaleProductLimitType.SingleLimit :
                                (_.PlaceQuantity != null && query.Where(q => q.ActivityID == _.ActivityID).Sum(q => q.BuyQuantity + q.RecordQuantity) > _.PlaceQuantity.Value ? FlashSaleProductLimitType.PlaceLimit : FlashSaleProductLimitType.Success))))

                };
            });

            return result;
        }

        public async static Task<List<FlashSaleModel>> GetFlashSaleListAsync(Guid[] activityIDs)
        {
            Stopwatch w = new Stopwatch();
            Logger.Info("开始执行接口");
            w.Start();
            List<FlashSaleModel> list = new List<FlashSaleModel>();
            foreach (var activityID in activityIDs)
            {
                var result = await SelectFlashSaleDataByActivityIDAsync(activityID);
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
            var a = CacheHelper.CreateHashClient(activityId.ToString(), TimeSpan.FromDays(30));
            var b = a.Get<int>(new List<string>() { "1" });
            FixedPriceActivityStatusResult result = new FixedPriceActivityStatusResult();
            if (activityId != Guid.Empty)
            {
                var activityConfig = await ActivityCache.GetBaoYangActivityConfig(activityId);

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
                            var activityRoundLimitCount = round.LimitedQuantity;
                            var userLimitCount = activityConfig.ItemQuantityPerUser;
                            var userPurchaseCount = await counter.GetCurrentUserPurchaseCount(userId.ToString(), "userId");
                            var activityRoundPurchaseCount = await counter.GetCurrentActivityPurchaseCount();

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
                            var userLimitCount = activityConfig.ItemQuantityPerUser;
                            var userPurchaseCount = await counter.GetCurrentUserPurchaseCount(userId.ToString(), "userId");
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
            return await SelectFlashSaleDataByActivityIDAsync(new Guid(activityId));
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
                        EndDateTime = p.EndDateTime
                    }).ToList());
        }
    }
}
