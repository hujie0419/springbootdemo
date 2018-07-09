using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Activity.Server.Manager;
using Tuhu.Service.Activity.Server.Model;
using CheckFlashSaleStatus = Tuhu.Service.Activity.Server.Model.CheckFlashSaleStatus;
using FixedPriceActivityConfig = Tuhu.Service.BaoYang.Models.Activity.FixedPriceActivityConfig;
using FixedPriceActivityRoundConfig = Tuhu.Service.BaoYang.Models.Activity.FixedPriceActivityRoundConfig;

namespace Tuhu.Service.Activity.Server.FlashSaleSystem
{
    public class ActivityValidator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ActivityValidator));
        public static async Task<Tuple<CreateOrderErrorCode, int, int, FixedPriceActivityRoundConfig>> ValidateBaoyang(Guid activityId)
        {
            CreateOrderErrorCode result = CreateOrderErrorCode.ActivitySatisfied;
            int userLimitCount = 0;
            int activityLimitCount = 0;
            FixedPriceActivityRoundConfig round = null;

            try
            {
                var activity = await ActivityCache.GetBaoYangActivityConfig(activityId);

                if (activity != null)
                {
                    var now = DateTime.Now;
                    round = GetCurrentRoundConfig(activity, now);

                    if (now >= activity.EndTime)
                    {
                        result = CreateOrderErrorCode.ActivityExpired;
                    }
                    else if (now < activity.StartTime)
                    {
                        result = CreateOrderErrorCode.ActivityNotStart;
                    }
                    else if (round == null)
                    {
                        result = CreateOrderErrorCode.WaitingNext;
                    }
                    else
                    {
                        userLimitCount = activity.ItemQuantityPerUser;
                        activityLimitCount = round.LimitedQuantity;
                    }
                }
                else
                {
                    result = CreateOrderErrorCode.ActivityExpired;
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("ActivityValidator").Error(ex);
                result = CreateOrderErrorCode.ServerBusy;
            }

            return Tuple.Create(result, activityLimitCount, userLimitCount, round);
        }

        public static FixedPriceActivityRoundConfig GetCurrentRoundConfig(FixedPriceActivityConfig activityConfig, DateTime now)
        {
            FixedPriceActivityRoundConfig result = null;
            if (activityConfig != null && activityConfig.RoundConfigs != null && activityConfig.RoundConfigs.Any())
            {
                result = activityConfig.RoundConfigs.FirstOrDefault(o => now >= o.StartTime && now < o.EndTime);
            }

            return result;
        }

        public static async Task<FixedPriceActivityRoundConfig> GetRoundConfig(Guid activityId, DateTime time)
        {
            FixedPriceActivityRoundConfig result = null;
            var activityConfig = await ActivityCache.GetBaoYangActivityConfig(activityId);
            if (activityConfig != null && activityConfig.RoundConfigs != null && activityConfig.RoundConfigs.Any())
            {
                result = activityConfig.RoundConfigs.FirstOrDefault(o => time >= o.StartTime && time < o.EndTime);
            }

            return result;
        }

        #region 普通限购相关的验证

        public static async Task<List<CheckFlashSaleResponseModel>> CheckFlashSaleAsync(FlashSaleOrderRequest request)
        {
            var result = new CheckFlashSaleResponseModel()
            {
                Code = CheckFlashSaleStatus.CreateOrderFailed
            };
            var orderItems = new List<CheckFlashSaleResponseModel>();
            foreach (var item in request.Products)
            {
                result = await CheckFlashSaleByPidAsync(item, request.UserId, request.DeviceId, request.UseTel);
                if (item.ActivityId != null)
                    orderItems.Add(new CheckFlashSaleResponseModel()
                    {
                        HasPlaceLimit = result.HasPlaceLimit,
                        HasQuantityLimit = result.HasQuantityLimit,
                        AllPlaceLimitId = result.AllPlaceLimitId,
                        Code = result.Code,
                        Num = item.Num,
                        PID = item.PID,
                        ActivityId = item.ActivityId.Value
                    });
                if (result.Code < 0)
                {
                    break;
                }
            }
            var items = orderItems.Where(r =>
               r.Code != CheckFlashSaleStatus.CreateOrderFailed &&
               r.Code != CheckFlashSaleStatus.NoExist).ToList();
            if (!orderItems.Any() || result.Code > 0) return items;

            await FlashSaleCounter.DecrementAllFlashCount(request, items);
            return orderItems;
        }


        private static async Task<CheckFlashSaleResponseModel> CheckFlashSaleByPidAsync(OrderItems item, Guid userId, string deviceId, string userTel)
        {
            var hasMaxLimit = false;
            var hasPlaceLimit = false;
            string allPlaceLimitId = null;
            try
            {
                if (!item.ActivityId.HasValue)
                    return new CheckFlashSaleResponseModel()
                    {
                        Code = CheckFlashSaleStatus.NoExist,
                    };

                var flashSale = await DalFlashSale.FetchFlashSaleProductModel(item);

                if (flashSale == null)
                {
                    return new CheckFlashSaleResponseModel()
                    {
                        Code = CheckFlashSaleStatus.NoExist,
                    };
                }

                var model = await FlashSaleCounter.RedisHashRecord(item.PID, item);

                if (model.Code == CheckFlashSaleStatus.CreateOrderFailed)
                    return new CheckFlashSaleResponseModel()
                    {
                        Code = CheckFlashSaleStatus.CreateOrderFailed,
                    };

                var record = model.Record;

                var remainQuantity = flashSale.TotalQuantity - record;
                if (remainQuantity < 0)
                    return new CheckFlashSaleResponseModel()
                    {
                        Code = CheckFlashSaleStatus.NoEnough,
                    };
                var request = new GenerateKeyRequest
                {
                    DeviceId = deviceId,
                    UserId = userId.ToString(),
                    UserTel = userTel,
                    ActivityId = item.ActivityId.ToString(),
                    Pid = item.PID
                };
                var countKey = new GenerateFlashSaleKey(request);
                if (flashSale.MaxQuantity.HasValue)
                {
                    var personLimit1 = await FlashSaleCounter.RedisCounterRecord(countKey.PersonalkeyUserId,
                        LimitType.PersonalLimit, CounterKeyType.UserIdKey, item, userId, deviceId, userTel, null, -1);
                    if (personLimit1.Code < 0)
                        return new CheckFlashSaleResponseModel()
                        {
                            Code = CheckFlashSaleStatus.CreateOrderFailed,
                        };
                    var personLimit2 = await FlashSaleCounter.RedisCounterRecord(countKey.PersonalkeyDeviceId,
                        LimitType.PersonalLimit, CounterKeyType.DeviceIdKey, item, userId, deviceId, userTel, null,
                        personLimit1.DeviceCount);
                    if (personLimit2.Code < 0)
                    {
                        var orderItem = new CheckFlashSaleResponseModel
                        {
                            Num = item.Num
                        };
                        await FlashSaleCounter.DecrementCountCount(orderItem, countKey.PersonalkeyUserId);
                        return new CheckFlashSaleResponseModel()
                        {
                            Code = CheckFlashSaleStatus.CreateOrderFailed,
                        };
                    }
                    var personLimit3 = await FlashSaleCounter.RedisCounterRecord(countKey.PersonalkeyUseTel,
                        LimitType.PersonalLimit, CounterKeyType.UserTelKey, item, userId, deviceId, userTel, null,
                        personLimit1.TelCount);
                    if (personLimit3.Code < 0)
                    {
                        var orderItem = new CheckFlashSaleResponseModel
                        {
                            Num = item.Num
                        };
                        await FlashSaleCounter.DecrementCountCount(orderItem, countKey.PersonalkeyUserId);
                        await FlashSaleCounter.DecrementCountCount(orderItem, countKey.PersonalkeyDeviceId);
                        return new CheckFlashSaleResponseModel()
                        {
                            Code = CheckFlashSaleStatus.CreateOrderFailed,
                        };
                    }
                    if (personLimit1.Code < 0 || personLimit2.Code < 0 || personLimit3.Code < 0)
                    {
                        return new CheckFlashSaleResponseModel()
                        {
                            Code = CheckFlashSaleStatus.CreateOrderFailed,
                        };
                    }
                    var maxLimit = Math.Max(Math.Max(personLimit1.Record, personLimit2.Record), personLimit3.Record);
                    hasMaxLimit = true;
                    if (maxLimit > flashSale.MaxQuantity)
                    {
                        return new CheckFlashSaleResponseModel()
                        {
                            Code = CheckFlashSaleStatus.MaxQuantityLimit,
                            HasQuantityLimit = true
                        };
                    }
                }
                if (flashSale.PlaceQuantity.HasValue && flashSale.PlaceQuantity.Value > 0)
                {
                    var saleProducts = (await DalFlashSale.SelectFlashSaleProductsAsync(item.ActivityId.Value)).ToList();
                    var salePids = saleProducts.Where(r => r.IsJoinPlace).Select(r => r.PID).ToList();
                    if (salePids.Contains(item.PID))
                    {
                        var placeLimit1 = await FlashSaleCounter.RedisCounterRecord(countKey.PlacekeyUserId,
                            LimitType.PlaceLimit, CounterKeyType.UserIdKey, item, userId, deviceId, userTel, salePids,
                            -1);
                        if (placeLimit1.Code < 0)
                            return new CheckFlashSaleResponseModel()
                            {
                                Code = CheckFlashSaleStatus.CreateOrderFailed,
                            };
                        var placeLimit2 = await FlashSaleCounter.RedisCounterRecord(countKey.PlacekeyDeviceId,
                            LimitType.PlaceLimit, CounterKeyType.DeviceIdKey, item, userId, deviceId, userTel, salePids,
                            placeLimit1.DeviceCount);
                        if (placeLimit2.Code < 0)
                        {
                            var orderItem = new CheckFlashSaleResponseModel
                            {
                                Num = item.Num
                            };
                            await FlashSaleCounter.DecrementCountCount(orderItem, countKey.PlacekeyUserId);
                            return new CheckFlashSaleResponseModel()
                            {
                                Code = CheckFlashSaleStatus.CreateOrderFailed,
                            };
                        }
                        var placeLimit3 = await FlashSaleCounter.RedisCounterRecord(countKey.PlacekeyUseTel,
                            LimitType.PlaceLimit, CounterKeyType.UserTelKey, item, userId, deviceId, userTel, salePids,
                            placeLimit1.TelCount);
                        if (placeLimit3.Code < 0)
                        {
                            var orderItem = new CheckFlashSaleResponseModel
                            {
                                Num = item.Num
                            };
                            await FlashSaleCounter.DecrementCountCount(orderItem, countKey.PlacekeyUserId);
                            await FlashSaleCounter.DecrementCountCount(orderItem, countKey.PlacekeyDeviceId);
                            return new CheckFlashSaleResponseModel()
                            {
                                Code = CheckFlashSaleStatus.CreateOrderFailed,
                            };
                        }
                        if (placeLimit1.Code < 0 || placeLimit2.Code < 0 || placeLimit3.Code < 0)
                        {
                            return new CheckFlashSaleResponseModel()
                            {
                                Code = CheckFlashSaleStatus.CreateOrderFailed,
                            };
                        }
                        hasPlaceLimit = true;
                        var placeLimit = Math.Max(Math.Max(placeLimit1.Record, placeLimit2.Record), placeLimit3.Record);
                        if (placeLimit > flashSale.PlaceQuantity.Value)
                        {
                            return new CheckFlashSaleResponseModel()
                            {
                                Code = CheckFlashSaleStatus.PlaceQuantityLimit,
                                HasQuantityLimit = hasMaxLimit,
                                HasPlaceLimit = true
                            };
                        }
                    }

                }

                #region 针对618增加一个全局会场限购

                if (flashSale.ActiveType == 3 && FlashSaleManager.CheckAllPlaceLimitStatrtDate())
                {

                    var allPLimitId = GlobalConstant.AllPlaceLimitId;
                    item.AllPlaceLimitId = allPLimitId;
                    var request1 = new GenerateKeyRequest
                    {
                        DeviceId = deviceId,
                        UserId = userId.ToString(),
                        UserTel = userTel,
                        ActivityId = item.AllPlaceLimitId,
                        Pid = item.PID,
                        IsAllPlaceLimit=true
                    };
                    var countKeyAllLimit = new GenerateFlashSaleKey(request1);
                    var config = await FlashSaleManager.SelectFlashSaleDataByActivityIDAsync(new Guid(allPLimitId));
                    if (config.PlaceQuantity.HasValue)
                    {
                        var saleProducts = config.Products.ToList();
                        var salePids = saleProducts.Where(r => r.IsJoinPlace).Select(r => r.PID).ToList();
                        if (salePids.Contains(item.PID))
                        {
                            var placeLimit1 = await FlashSaleCounter.RedisCounterRecord(
                                countKeyAllLimit.PlacekeyUserId, LimitType.AllPlaceLimit, CounterKeyType.UserIdKey, item,
                                userId, deviceId, userTel, salePids, -1);
                            if (placeLimit1.Code < 0)
                                return new CheckFlashSaleResponseModel()
                                {
                                    Code = CheckFlashSaleStatus.CreateOrderFailed,
                                };
                            var placeLimit2 = await FlashSaleCounter.RedisCounterRecord(countKeyAllLimit.PlacekeyDeviceId,
                                LimitType.AllPlaceLimit, CounterKeyType.DeviceIdKey, item, userId, deviceId, userTel,
                                salePids, placeLimit1.DeviceCount);
                            if (placeLimit2.Code < 0)
                            {
                                var orderItem = new CheckFlashSaleResponseModel
                                {
                                    Num = item.Num
                                };
                                await FlashSaleCounter.DecrementCountCount(orderItem, countKeyAllLimit.PlacekeyUserId);
                                return new CheckFlashSaleResponseModel()
                                {
                                    Code = CheckFlashSaleStatus.CreateOrderFailed,
                                };
                            }
                            var placeLimit3 = await FlashSaleCounter.RedisCounterRecord(countKeyAllLimit.PlacekeyUseTel,
                                LimitType.AllPlaceLimit, CounterKeyType.UserTelKey, item, userId, deviceId, userTel,
                                salePids, placeLimit1.TelCount);
                            if (placeLimit3.Code < 0)
                            {
                                var orderItem = new CheckFlashSaleResponseModel
                                {
                                    Num = item.Num
                                };
                                await FlashSaleCounter.DecrementCountCount(orderItem, countKeyAllLimit.PlacekeyUserId);
                                await FlashSaleCounter.DecrementCountCount(orderItem, countKeyAllLimit.PlacekeyDeviceId);
                                return new CheckFlashSaleResponseModel()
                                {
                                    Code = CheckFlashSaleStatus.CreateOrderFailed,
                                };
                            }
                            if (placeLimit1.Code < 0 || placeLimit2.Code < 0 || placeLimit3.Code < 0)
                            {
                                return new CheckFlashSaleResponseModel()
                                {
                                    Code = CheckFlashSaleStatus.CreateOrderFailed,
                                };
                            }
                            var allplaceLimit = Math.Max(Math.Max(placeLimit1.Record, placeLimit2.Record),
                                placeLimit3.Record);
                            allPlaceLimitId = allPLimitId;
                            if (allplaceLimit > config.PlaceQuantity.Value)
                            {
                                return new CheckFlashSaleResponseModel()
                                {
                                    Code = CheckFlashSaleStatus.PlaceQuantityLimit,
                                    AllPlaceLimitId = allPlaceLimitId
                                };
                            }
                        }

                        #region 错误代码

                        //var hashRecord618 = await FlashSaleCounter.RedisAllPlaceLimitHashRecord(item.PID, item);
                        //if (hashRecord618 != null)
                        //{
                        //    if (hashRecord618.Code == CheckFlashSaleStatus.CreateOrderFailed)
                        //        return new CheckFlashSaleResponseModel()
                        //        {
                        //            Code = CheckFlashSaleStatus.CreateOrderFailed,
                        //        };

                        //    var saledAll = model.Record;
                        //    var config =
                        //        await FlashSaleManager.SelectFlashSaleDataByActivityIDAsync(
                        //            new Guid(allPLimitId));
                        //    var configQty = config.Products.Where(r => r.PID == item.PID).Select(r => r.TotalQuantity).FirstOrDefault();
                        //    allPlaceLimitId = allPLimitId;
                        //    if (configQty - saledAll < 0)
                        //        return new CheckFlashSaleResponseModel()
                        //        {
                        //            Code = CheckFlashSaleStatus.NoEnough,
                        //            AllPlaceLimitId = allPlaceLimitId
                        //        };
                        //}

                        #endregion
                    }
                }

                #endregion

                    return new CheckFlashSaleResponseModel()
                    {
                        Code = CheckFlashSaleStatus.Succeed,
                        HasPlaceLimit = hasPlaceLimit,
                        HasQuantityLimit = hasMaxLimit,
                        AllPlaceLimitId = allPlaceLimitId
                    };               
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Info(ex);
                return new CheckFlashSaleResponseModel()
                {
                    Code = CheckFlashSaleStatus.CreateOrderFailed,
                    HasPlaceLimit = hasPlaceLimit,
                    HasQuantityLimit = hasMaxLimit
                };
            }

        }
    }

}
