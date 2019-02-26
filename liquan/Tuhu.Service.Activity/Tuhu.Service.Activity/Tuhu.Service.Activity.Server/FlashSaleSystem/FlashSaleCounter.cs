using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models.Requests;
using Common.Logging;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Activity.Server.Model;
using CheckFlashSaleStatus = Tuhu.Service.Activity.Server.Model.CheckFlashSaleStatus;

namespace Tuhu.Service.Activity.Server.FlashSaleSystem
{
    public class FlashSaleCounter
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FlashSaleCounter));
        public static readonly string ClientName = "ActivityCounter";

        /// <summary>
        /// 重试
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisFunc"></param>
        /// <param name="times"></param>
        /// <param name="spanMiliSecond"></param>
        /// <returns></returns>
        public static async Task<double> RedisCountRetry<T>(Func<Task<IResult<T>>> redisFunc, int times = 3,
            int spanMiliSecond = 100) where T : struct
        {
            times = times > 0 ? times : 3;
            spanMiliSecond = spanMiliSecond > 0 ? spanMiliSecond : 100;
            for (var i = 0; i < times; i++)
            {
                var setResult = await redisFunc();
                if (setResult.Success)
                    return Convert.ToDouble(setResult.Value);
                Thread.Sleep(spanMiliSecond);
            }
            return -1;

        }

        /// <summary>
        /// 记录购买记录进计数器
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>

        /// <returns></returns>
        public static async Task<SimpleResponseModel> RedisHashRecord(string key, OrderItems item)
        {
            using (var client = CacheHelper.CreateHashClient(item.ActivityId.ToString(), TimeSpan.FromDays(30)))
            {
                var recordCache = await client.GetAsync<int>(key);
                double retryresult = 0;
                IResult<double> increment;
                var incrementNum = 0;
                if (!recordCache.Success)
                {
                    if (recordCache.Message == "Key不存在") //暂时写死错误信息，后面如果底层针对这种情况有固定errorcode返回修改下
                    {
                        if (item.ActivityId.ToString() == "c6cc9628-21aa-4f85-a19c-fd12529159e8") //买三送一活动打log
                            Logger.Info($"hash缓存加之前=》Key{key}value{recordCache.Value}活动id：{item.ActivityId.ToString()}");

                        var result = await DalFlashSale.FetchFlashSaleProductModel(item);
                        var quantity = result.SaleOutQuantity;
                        increment = await client.IncrementAsync(key, quantity + item.Num);
                        incrementNum = quantity + item.Num;
                    }
                    else
                    {
                        Logger.Error($"redis计数记录失败=>CheckFlashSaleAsync{key}");
                        return new SimpleResponseModel
                        {
                            Code = CheckFlashSaleStatus.CreateOrderFailed,
                            Mesage = "计数器执行失败"
                        };
                    }
                }
                else
                {
                    increment = await client.IncrementAsync(key, item.Num);
                    incrementNum = item.Num;
                }

                if (!increment.Success) //失败情况后续考虑
                {
                    retryresult = await RedisCountRetry(() => client.IncrementAsync(key, incrementNum));
                    if (retryresult < 0)
                    {
                        Logger.Error($"redis计数记录失败=>CheckFlashSaleAsync{key}");
                        return new SimpleResponseModel
                        {
                            Code = CheckFlashSaleStatus.CreateOrderFailed,
                            Mesage = "计数器执行失败"
                        };
                    }
                }
                if (item.ActivityId.ToString() == "c6cc9628-21aa-4f85-a19c-fd12529159e8") //买三送一活动打log
                    Logger.Info($"hash缓存加之后=》Key：{key}value：{increment.Value}活动id：{item.ActivityId.ToString()}");
                return new SimpleResponseModel
                {
                    Code = CheckFlashSaleStatus.Succeed,
                    Record = (int)(increment.Success ? increment.Value : retryresult),
                    Mesage = "成功"
                };
            }
        }

        public static async Task<SimpleResponseModel> RedisCounterRecord(string key, LimitType type, CounterKeyType keyType, OrderItems item,
            Guid userId = new Guid(), string deviceId = null, string userTel = null, List<string> pids = null,
            int dbQuantity = 0)
        {
            Logger.Info($"计数器Key=>{key}增加{item.Num}");
            using (var client = CacheHelper.CreateCounterClient(ClientName, TimeSpan.FromDays(30)))
            {
                var recordCache = await client.CountAsync(key); //这里跟hashclient一点咯要区分开，这里key不存在会报错
                double retryresult = 0;
                IResult<long> increment;
                var tupleReult = new Tuple<int, int, int>(0, -1, -1);
                var incrementNum = 0;
                var quantity = dbQuantity;
                if (!recordCache.Success)
                {
                    if (recordCache.Message == "Key不存在") //暂时写死错误信息，后面如果底层针对这种情况有固定errorcode返回修改下
                    {
                        if (dbQuantity == -1)
                        {
                            switch (type)
                            {

                                case LimitType.PlaceLimit:
                                    if (item.ActivityId != null)
                                    {
                                        tupleReult = await DalFlashSale.SelectOrderActivityProductRecordNumAsync(
                                                userId, deviceId,
                                               string.IsNullOrEmpty(item.AllPlaceLimitId)? item.ActivityId.Value: new Guid(item.AllPlaceLimitId), userTel, pids);
                                        quantity = GetDbQtyBykeyType(tupleReult, keyType, deviceId);
                                    }
                                    break;
                                case LimitType.PersonalLimit:
                                    if (item.ActivityId != null)
                                    {
                                        tupleReult = await DalFlashSale.SelectOrderActivityProductRecordNumAsync(userId, deviceId,
                                              item.ActivityId.Value, userTel, new List<string>
                                        {
                                            item.PID
                                        });
                                        quantity = GetDbQtyBykeyType(tupleReult, keyType, deviceId);
                                    }
                                    break;
                                case LimitType.AllPlaceLimit:
                                    if (item.ActivityId != null)
                                    {
                                        tupleReult = await DalFlashSale.SelectAllPlaceOrderActivityProductRecordNumAsync(
                                                userId, deviceId, new Guid(item.AllPlaceLimitId), userTel, pids);
                                        quantity = GetDbQtyBykeyType(tupleReult, keyType, deviceId);
                                    }
                                    break;
                            }
                        }
                        increment = await client.IncrementAsync(key, quantity + item.Num);
                        incrementNum = quantity + item.Num;
                    }
                    else
                    {
                        Logger.Error($"redis计数记录失败=>CheckFlashSaleAsync{key}");
                        return new SimpleResponseModel
                        {
                            Code = CheckFlashSaleStatus.CreateOrderFailed,
                            Mesage = "计数器执行失败"
                        };
                    }
                }
                else
                {
                    increment = await client.IncrementAsync(key, item.Num);
                    incrementNum = item.Num;
                }
                if (!increment.Success) //失败情况后续考虑
                {
                    retryresult = await RedisCountRetry(() => client.IncrementAsync(key, incrementNum));
                    if (retryresult < 0)
                    {
                        Logger.Error($"redis计数记录失败=>CheckFlashSaleAsync{key}");
                        return new SimpleResponseModel
                        {
                            Code = CheckFlashSaleStatus.CreateOrderFailed,
                            Mesage = "计数器执行失败"
                        };
                    }
                }
                return new SimpleResponseModel
                {
                    Code = CheckFlashSaleStatus.Succeed,
                    Record = (int)(increment.Success ? increment.Value : retryresult),
                    Mesage = "成功",
                    //DbQuantity = quantity,
                    DeviceCount = tupleReult.Item2,
                    TelCount = tupleReult.Item3
                };
            }

        }

        #region 失败情况下记录从计数器里减去

        public static async Task DecrementAllFlashCount(FlashSaleOrderRequest requset,
            List<CheckFlashSaleResponseModel> flashSale)
        {

            foreach (var orderItem in flashSale)
            {
                await DecrementHashCount(orderItem, orderItem.PID);

                var request = new GenerateKeyRequest
                {
                    DeviceId = requset.DeviceId,
                    UserId = requset.UserId.ToString(),
                    UserTel = requset.UseTel,
                    ActivityId = orderItem.ActivityId.ToString(),
                    Pid = orderItem.PID
                };
                var countKey = new GenerateFlashSaleKey(request);
                if (orderItem.HasQuantityLimit)
                {
                    await DecrementCountCount(orderItem, countKey.PersonalkeyUserId);
                    await DecrementCountCount(orderItem, countKey.PersonalkeyDeviceId);
                    await DecrementCountCount(orderItem, countKey.PersonalkeyUseTel);
                }
                if (orderItem.HasPlaceLimit)
                {
                    await DecrementCountCount(orderItem, countKey.PlacekeyUserId);
                    await DecrementCountCount(orderItem, countKey.PlacekeyDeviceId);
                    await DecrementCountCount(orderItem, countKey.PlacekeyUseTel);
                }
                Logger.Info($"全局会场限购2id是{orderItem.AllPlaceLimitId}");
                if (!string.IsNullOrEmpty(orderItem.AllPlaceLimitId))
                {
                    var request1 = new GenerateKeyRequest
                    {
                        DeviceId = requset.DeviceId,
                        UserId = requset.UserId.ToString(),
                        UserTel = requset.UseTel,
                        ActivityId = orderItem.AllPlaceLimitId,
                        Pid = orderItem.PID,
                        IsAllPlaceLimit = true
                    };
                    var countKeyAllLimit = new GenerateFlashSaleKey(request1);
                    await DecrementCountCount(orderItem, countKeyAllLimit.PlacekeyUserId);
                    await DecrementCountCount(orderItem, countKeyAllLimit.PlacekeyDeviceId);
                    await DecrementCountCount(orderItem, countKeyAllLimit.PlacekeyUseTel);
                    Logger.Info($"全局会场限购3id是{orderItem.AllPlaceLimitId}");
                }


            }
        }

        private static async Task<double> DecrementHashCount(CheckFlashSaleResponseModel item, string key,string clientName=null)
        {
            using (var client = CacheHelper.CreateHashClient(string.IsNullOrEmpty(clientName)?item.ActivityId.ToString():clientName, TimeSpan.FromDays(30)))
            {
                var decrement = await client.DecrementAsync(key, item.Num);

                if (!decrement.Success)
                {
                    var retryresult = await RedisCountRetry(() => client.DecrementAsync(key, item.Num));
                    if (retryresult < 0)
                    {
                        Logger.Error($"回滚redis计数器失败=>CheckFlashSaleAsync{key}");
                        return (int)CheckFlashSaleStatus.CreateOrderFailed;
                    }
                }
                return decrement.Value;
            }

        }
        public static async Task<double> DecrementCountCount(CheckFlashSaleResponseModel item, string key)
        {
            Logger.Info($"计数器Key=>{key}减少{item.Num}");
            using (var client = CacheHelper.CreateCounterClient(ClientName, TimeSpan.FromDays(30)))
            {
                var counter = await client.CountAsync(key);
                if (counter.Success && counter.Value == 0)
                    return counter.Value;
                var decrement = await client.DecrementAsync(key, item.Num);

                if (!decrement.Success)
                {
                    var retryresult = await RedisCountRetry(() => client.DecrementAsync(key, item.Num));
                    if (retryresult < 0)
                    {
                        Logger.Error($"回滚redis计数器失败=>CheckFlashSaleAsync{key}");
                        return (int)CheckFlashSaleStatus.CreateOrderFailed;
                    }
                }
                return decrement.Value;
            }

        }

        #endregion

        private static int GetDbQtyBykeyType(Tuple<int, int, int> tupleSource, CounterKeyType keyType, string deviceId)
        {
            switch (keyType)
            {
                case CounterKeyType.UserIdKey:
                    return tupleSource.Item1;
                case CounterKeyType.DeviceIdKey://设备号不存在情况下特殊处理
                    if (string.IsNullOrEmpty(deviceId))
                        return tupleSource.Item1;
                    else
                    {
                        return tupleSource.Item2;
                    }

                case CounterKeyType.UserTelKey:
                    return tupleSource.Item3;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 刷新用进计数器
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>

        /// <returns></returns>
        public static async Task<bool> RefreshRedisHashRecord(string key, OrderItems item)
        {
            using (var client = CacheHelper.CreateHashClient(item.ActivityId.ToString(), TimeSpan.FromDays(30)))
            {
                var recordCache = await client.GetAsync<int>(key);
                IResult<double> increment;
                if (!recordCache.Success || recordCache.Value == 0)
                {
                    increment = await client.IncrementAsync(key, item.Num);
                }
                else
                {
                    await client.DecrementAsync(key, recordCache.Value);
                    // await client.RemoveAsync(key);
                    increment = await client.IncrementAsync(key, item.Num);
                }
                return increment.Success;
            }

        }

        public static async Task<OrderCountResponse> GetUserCreateFlashOrderCountCacheAsync(
            OrderCountCacheRequest request)
        {
            var responseModel = new OrderCountResponse();
            var keyModels = SetKeys(request);
            using (var client = CacheHelper.CreateCounterClient(ClientName))
            {
                foreach (var key in keyModels.OrderBy(r => r.Sort))
                {
                    await GetCountValue(responseModel, key.Type, key.Key, client);
                    //if(result.RedisField)
                    //    break;
                }

            }
            var flashsale = (await DalFlashSale.SelectFlashSaleFromDBAsync(new Guid(request.ActivityId)));
            if (flashsale == null)
                return responseModel;
            var saleProducts = flashsale.Products.ToList();
            var salePids = saleProducts.Where(r => r.IsJoinPlace).Select(r => r.PID).ToList();
            var saleproduct = saleProducts.FirstOrDefault(r => r.PID == request.Pid);
            var records = (await DalFlashSale.SelectOrderActivityProductOrderRecordsAsync(
                                new Guid(request.UserId), request.DeviceId,
                                new Guid(request.ActivityId), request.UserTel)).ToList();
            var personQty = records?.Where(r => r.PID == request.Pid).Sum(r => r.Quantity) ?? 0;
            var placeQty = records.Where(r => salePids.Contains(r.PID)).Sum(r => r.Quantity);
            responseModel.UserDbPersonLimitQty = personQty;
            responseModel.UserDbPlaceLimitQty = placeQty;
            responseModel.PersonConfigLimitQty = saleproduct?.MaxQuantity;
            responseModel.PlaceConfigLimitQty = saleproduct?.PlaceQuantity ;
            responseModel.TotalConfigQty = saleproduct?.TotalQuantity;
            return responseModel;
        }

        public static async Task<OrderCountResponse> SetUserCreateFlashOrderCountCacheAsync(
            OrderCountCacheRequest request)
        {
            var keyModels = SetKeys(request);
            var flashSale = await DalFlashSale.FetchFlashSaleProductModel(new OrderItems()
            {
                PID = request.Pid,
                ActivityId = new Guid(request.ActivityId)
            });
            var response = new OrderCountResponse();
            using (var client = CacheHelper.CreateCounterClient(ClientName))
            {
                //Queryable<int> personSorts = new Queryable[4, 5, 6];
                //个人限购
                if (flashSale.MaxQuantity.HasValue)
                {
                    foreach (var key in keyModels.OrderBy(r => r.Sort).Where(r => r.Sort == 4 || r.Sort == 5 || r.Sort == 6))
                    {
                        response = await SetCountValue(response,key.Key, request.PerSonalNum, client);
                        if (response.RedisField)
                            break;
                    }
                }
                //会场限购
                if (flashSale.PlaceQuantity.HasValue && flashSale.PlaceQuantity.Value > 0)
                {
                    var saleProducts = (await DalFlashSale.SelectFlashSaleFromDBAsync(new Guid(request.ActivityId))).Products.ToList();
                    var salePids = saleProducts.Where(r => r.IsJoinPlace).Select(r => r.PID).ToList();
                    if (salePids.Contains(request.Pid))
                    {
                        foreach (var key in keyModels.OrderBy(r => r.Sort).Where(r => r.Sort == 1 || r.Sort == 2 || r.Sort == 3))
                        {
                            response = await SetCountValue(response,key.Key, request.PlaceNum, client);
                            if (response.RedisField)
                                break;
                        }
                    }
                }
            }
            if (response.RedisField)
            {
                return response;
            }
            return await GetUserCreateFlashOrderCountCacheAsync(request);
        }
        private static async Task<OrderCountResponse> GetCountValue(OrderCountResponse model, KeyType type, string key, CounterClient client)
        {
            switch (type)
            {
                case KeyType.PlaceUserIdKey:
                    var counterpu = await client.CountAsync(key);
                    if (counterpu.Success)
                        model.PlaceUserIdCountCacheNum = (int)counterpu.Value;
                    else
                    {
                        model.RedisField = true;
                        model.RedisMsg = $"{counterpu.Message}key==>{key},";
                        //  return model;
                    }
                    break;
                case KeyType.PlaceDeviceIdKey:
                    var counterpd = await client.CountAsync(key);
                    if (counterpd.Success)
                        model.PlaceDeviceCountCacheNum = (int)counterpd.Value;
                    else
                    {
                        model.RedisField = true;
                        model.RedisMsg += $"{counterpd.Message}key==>{key},";
                        // return model;
                    }
                    break;
                case KeyType.PlaceUserTelKey:
                    var counterpt = await client.CountAsync(key);
                    if (counterpt.Success)
                        model.PlaceUserTelCountCacheNum = (int)counterpt.Value;
                    else
                    {
                        model.RedisField = true;
                        model.RedisMsg += $"{counterpt.Message}key==>{key},";
                        // return model;
                    }
                    break;
                case KeyType.PersonUserIdKey:
                    var counterpu2 = await client.CountAsync(key);
                    if (counterpu2.Success)
                        model.PersonUserIdCountCacheNum = (int)counterpu2.Value;
                    else
                    {
                        model.RedisField = true;
                        model.RedisMsg += $"{counterpu2.Message}key==>{key},";
                        return model;
                    }
                    break;
                case KeyType.PersonDeviceIdKey:
                    var counterpd2 = await client.CountAsync(key);
                    if (counterpd2.Success)
                        model.PersonDeviceCountCacheNum = (int)counterpd2.Value;
                    else
                    {
                        model.RedisField = true;
                        model.RedisMsg += $"{counterpd2.Message}key==>{key},";
                        //  return model;
                    }
                    break;
                case KeyType.PersonUserTelKey:
                    var counterpt2 = await client.CountAsync(key);
                    if (counterpt2.Success)
                        model.PersonUserTelCountCacheNum = (int)counterpt2.Value;
                    else
                    {
                        model.RedisField = true;
                        model.RedisMsg += $"{counterpt2.Message}key==>{key}";
                        // return model;
                    }
                    break;
                default:
                    return model;
            }
            return model;
        }

        private static List<CountKeyModel> SetKeys(OrderCountCacheRequest request)
        {
            #region key
            var keyModels = new List<CountKeyModel>();
            var keyrequest = new GenerateKeyRequest
            {
                DeviceId = request.DeviceId,
                UserId = request.UserId.ToString(),
                UserTel = request.UserTel,
                ActivityId = request.ActivityId.ToString(),
                Pid = request.Pid
            };
            var countKey = new GenerateFlashSaleKey(keyrequest);
            keyModels.Add(new CountKeyModel
            {
                Key = countKey.PlacekeyUserId,
                Sort = 1,
                Type = KeyType.PlaceUserIdKey
            });
            keyModels.Add(new CountKeyModel
            {
                Key = countKey.PlacekeyDeviceId,
                Sort = 2,
                Type = KeyType.PlaceDeviceIdKey
            });

            keyModels.Add(new CountKeyModel
            {
                Key = countKey.PlacekeyUseTel,
                Sort = 3,
                Type = KeyType.PlaceUserTelKey
            });

            keyModels.Add(new CountKeyModel
            {
                Key = countKey.PersonalkeyUserId,
                Sort = 4,
                Type = KeyType.PersonUserIdKey
            });

            keyModels.Add(new CountKeyModel
            {
                Key = countKey.PersonalkeyDeviceId,
                Sort = 5,
                Type = KeyType.PersonDeviceIdKey
            });

            keyModels.Add(new CountKeyModel
            {
                Key = countKey.PersonalkeyUseTel,
                Sort = 6,
                Type = KeyType.PersonUserTelKey
            });
            return keyModels;

            #endregion
        }

        private static async Task<OrderCountResponse> SetCountValue(OrderCountResponse model, string key, int num, CounterClient client)
        {
            var getcount = await client.CountAsync(key);
            if (!getcount.Success && !getcount.Message.Equals("Key不存在"))
            {
                model.RedisField = true;
                model.RedisMsg = getcount.Message;
                return model;
            }
            else
            {
                IResult<long> setcount;
                if (getcount.Message != null && getcount.Message.Equals("Key不存在"))
                {
                    setcount = await client.IncrementAsync(key, num);
                    if (!setcount.Success)
                    {
                        model.RedisField = true;
                        model.RedisMsg = getcount.Message;
                        return model;
                    }
                }
                else
                {
                    var discountcounter = await client.DecrementAsync(key, getcount.Value);
                    if (!discountcounter.Success)
                    {
                        model.RedisField = true;
                        model.RedisMsg = getcount.Message;
                        return model;
                    }
                    else
                    {
                        setcount = await client.IncrementAsync(key, num);
                        if (!setcount.Success)
                        {
                            model.RedisField = true;
                            model.RedisMsg = getcount.Message;
                            return model;
                        }
                    }

                }
            }

            return model;
        }

        public static async Task<SimpleResponseModel> RedisAllPlaceLimitHashRecord(string key, OrderItems item)
        {
            using (var client = CacheHelper.CreateHashClient(GlobalConstant.AllPlaceLimitId, TimeSpan.FromDays(30)))
            {
                var recordCache = await client.GetAsync<int>(key);
                double retryresult = 0;
                IResult<double> increment;
                var incrementNum = 0;
                if (!recordCache.Success)
                {
                    if (recordCache.Message == "Key不存在") //暂时写死错误信息，后面如果底层针对这种情况有固定errorcode返回修改下
                    {
                        return null;
                    }
                    else
                    {
                        Logger.Error($"redis计数记录失败=>CheckFlashSaleAsync{key}");
                        return new SimpleResponseModel
                        {
                            Code = CheckFlashSaleStatus.CreateOrderFailed,
                            Mesage = "计数器执行失败"
                        };
                    }
                }
                else
                {
                    increment = await client.IncrementAsync(key, item.Num);
                    incrementNum = item.Num;
                }

                if (!increment.Success) //失败情况后续考虑
                {
                    retryresult = await RedisCountRetry(() => client.IncrementAsync(key, incrementNum));
                    if (retryresult < 0)
                    {
                        Logger.Error($"redis计数记录失败=>CheckFlashSaleAsync{key}");
                        return new SimpleResponseModel
                        {
                            Code = CheckFlashSaleStatus.CreateOrderFailed,
                            Mesage = "计数器执行失败"
                        };
                    }
                }
                return new SimpleResponseModel
                {
                    Code = CheckFlashSaleStatus.Succeed,
                    Record = (int)(increment.Success ? increment.Value : retryresult),
                    Mesage = "成功"
                };
            }
        }
    }
}
