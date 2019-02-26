using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Server.FlashSaleSystem;
using Tuhu.Service.Activity.Server.Model;
using Tuhu.Service.BaoYang;
using Tuhu.Service.BaoYang.Models.Activity;
using Tuhu.Service.Order;
using Tuhu.ZooKeeper;

namespace Tuhu.Service.Activity.Server.Manager.FlashSaleSystem
{
    public class BaoYangCounter : ICounter
    {
        private const string _keyPrefix = "PurchaseCounter";
        private Guid _activityId;
        private HashClient _counter;
        private SortedSetClient<string> orderHistory;
        private ILog _logger;
        public int _activityLimitCount;
        public int _userLimitCount;
        public FixedPriceActivityRoundConfig _round;

        public BaoYangCounter(Guid activityId, int activityLimitCount, int userLimitCount, FixedPriceActivityRoundConfig round)
        {
            DateTime now = DateTime.Now;
            this._activityId = activityId;
            this._counter = CacheHelper.CreateHashClient(activityId.ToString(), TimeSpan.FromDays(30));
            this.orderHistory = CacheHelper.CreateSortedSetClient<string>(activityId.ToString(), TimeSpan.FromDays(30));
            this._activityLimitCount = activityLimitCount;
            this._userLimitCount = userLimitCount;
            this._round = round;
            _logger = LogManager.GetLogger("保养定价活动计数器");
        }

        private string GetUserKey(string id, string type)
        {
            return $"{_keyPrefix}/{this._activityId.ToString()}/{type}/{id}";
        }

        private string GetActivityKey()
        {
            return $"{_keyPrefix}/{this._activityId.ToString()}/{_round.StartTime.Ticks}/{_round.EndTime.Ticks}";
        }

        public async Task<List<int?>> GetCurrentPurchaseCount(Guid userId, string deviceId, string userTel)
        {
            var count1 = await GetCurrentUserPurchaseCount(userId.ToString(), "userId");
            var count2 = await GetCurrentUserPurchaseCount(deviceId, "deviceId");
            var count3 = await GetCurrentUserPurchaseCount(userTel, "userTel");

            return new List<int?>() { count1, count2, count3 };
        }

        /// <summary>
        /// 获取当前用户的购买数量
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public async Task<int?> GetCurrentUserPurchaseCount(string id, string type)
        {
            int? result = null;
            long getcountCahceTime = 0;
            long selectFlashSaleOrderIdsByUserTime = 0;
            var sw = new Stopwatch();
            sw.Start();
            if (string.IsNullOrEmpty(id))
            {
                return 0;
            }

            string key = GetUserKey(id, type);
            var dataInCache = await _counter.GetAsync<int>(new List<string>() { key });
            getcountCahceTime = sw.ElapsedMilliseconds;
            if (dataInCache.Success)
            {
                if (dataInCache.Value != null && dataInCache.Value.Any())
                {
                    result = dataInCache.Value.First().Value;
                }
                else
                {
                    IEnumerable<int> orderIds = null;
                    switch (type)
                    {
                        case "userId":
                            orderIds = await DalFlashSale.SelectFlashSaleOrderIdsByUserAsync(_activityId, Guid.Parse(id), Guid.NewGuid().ToString(), string.Empty);
                            break;
                        case "deviceId":
                            orderIds = await DalFlashSale.SelectFlashSaleOrderIdsByUserAsync(_activityId, Guid.Empty, id, string.Empty);
                            break;
                        case "userTel":
                            orderIds = await DalFlashSale.SelectFlashSaleOrderIdsByUserAsync(_activityId, Guid.Empty, Guid.NewGuid().ToString(), id);
                            break;
                    }
                    selectFlashSaleOrderIdsByUserTime = sw.ElapsedMilliseconds - getcountCahceTime;
                    var count = 0;

                    if (orderIds != null && orderIds.Any())
                    {
                        count = orderIds.Distinct().Count();
                    }

                    var increResult = await _counter.IncrementAsync(key, count);
                    if (increResult.Success)
                    {
                        if (increResult.Value > count)
                        {
                            bool decResult = await RetryDecrement(key, count);
                        }
                        else
                        {
                            result = count;
                        }
                    }
                    else
                    {
                        _logger.Error($"计数器设置失败, id: {id}, type: {type}, {dataInCache.Message}", dataInCache.Exception);
                    }
                }
            }
            else
            {
                _logger.Error($"计数器获取失败, {dataInCache.Message}", dataInCache.Exception);
            }
            _logger.Info($"GetCurrentUserPurchaseCount=>getcountCahceTime:{getcountCahceTime}," +
                $"selectFlashSaleOrderIdsByUserTime:{selectFlashSaleOrderIdsByUserTime},total:{sw.ElapsedMilliseconds}");
            sw.Stop();
            return result;
        }

        private async Task<bool> RetryDecrement(string key, int count)
        {
            bool result = true;
            var decResult = await _counter.DecrementAsync(key, count);
            if (!decResult.Success)
            {
                decResult = await _counter.DecrementAsync(key, count);

                if (!decResult.Success)
                {
                    result = false;
                    _logger.Error($"RetryDecrement error, {key}, {count}", decResult.Exception);
                }
            }

            return result;
        }

        public async Task<int?> GetCurrentActivityPurchaseCount()
        {
            int? result = null;
            var sw = new Stopwatch();
            sw.Start();
            long getCountCache = 0;
            long beforeLockTime = 0;
            long inLockTime = 0;
            long selectFlashSaleOrderIdsByActivityTime = 0;
            long selectOrderStatusByOrderIdsTime = 0;
            string key = GetActivityKey();
            var dataInCache = await _counter.GetAsync<int>(new List<string>() { key });
            getCountCache = sw.ElapsedMilliseconds;
            if (dataInCache.Success)
            {
                if (dataInCache.Value != null && dataInCache.Value.Any())
                {
                    result = dataInCache.Value.First().Value;
                }
                else
                {
                    beforeLockTime = sw.ElapsedMilliseconds - getCountCache;
                    using (var zlock = new ZooKeeperLock(key))
                    {
                        if (await zlock.WaitAsync(1500))
                        {
                            inLockTime = sw.ElapsedMilliseconds - beforeLockTime;
                            var retryGet = await _counter.GetAsync<int>(new List<string>() { key });
                            if (retryGet.Success && retryGet.Value != null && retryGet.Value.Any())
                            {
                                result = retryGet.Value.First().Value;
                            }
                            else
                            {
                                var orderIds = await DalFlashSale.SelectFlashSaleOrderIdsByActivityAsync(_activityId, _round.StartTime, _round.EndTime);
                                selectFlashSaleOrderIdsByActivityTime = sw.ElapsedMilliseconds - inLockTime;
                                var count = 0;
                                if (orderIds != null && orderIds.Any())
                                {
                                    var orderStatus = await DalFlashSale.SelectOrderStatusByOrderIdsAsync(orderIds);
                                    selectOrderStatusByOrderIdsTime = sw.ElapsedMilliseconds - selectFlashSaleOrderIdsByActivityTime;
                                    count = orderStatus.Where(o => !string.Equals(o.Value, "7Canceled")).Count();
                                }

                                var increResult = await _counter.IncrementAsync(key, count);
                                if (increResult.Success)
                                {
                                    if (increResult.Value > count)
                                    {
                                        bool decResult = await RetryDecrement(key, count);
                                    }
                                    else
                                    {
                                        result = count;
                                    }
                                }
                                else
                                {
                                    _logger.Error($"计数器设置失败, activity: {_activityId}, {dataInCache.Message}", dataInCache.Exception);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                _logger.Error($"计数器获取失败, {dataInCache.Message}", dataInCache.Exception);
            }
            _logger.Info($"GetCurrentActivityPurchaseCount=>getCountCache:{getCountCache}," +
                $"beforeLockTime:{beforeLockTime},inLockTime:{inLockTime}," +
                $"selectFlashSaleOrderIdsByActivityTime:{selectFlashSaleOrderIdsByActivityTime}," +
                $"selectOrderStatusByOrderIdsTime:{selectOrderStatusByOrderIdsTime}," +
                $"total:{sw.ElapsedMilliseconds}");
            sw.Stop();
            return result;
        }

        public async Task<bool> DecreaseActivityPurchaseCount()
        {
            bool result = true;
            var key = GetActivityKey();
            var dataInCache = await _counter.GetAsync<int>(key);
            if (dataInCache.Success)
            {
                result = await RetryDecrement(key, 1);
            }

            return result;
        }

        public async Task<bool> DecreasePurchaseCount(Guid userId, string deviceId, string userTel)
        {
            bool result = true;
            var keys = new List<string>() { GetActivityKey(), GetUserKey(userId.ToString(), "userId"), GetUserKey(userTel, "userTel") };
            if (!string.IsNullOrEmpty(deviceId))
            {
                keys.Add(GetUserKey(deviceId, "deviceId"));
            }

            var dataInCache = await _counter.GetAsync<int>(keys);
            if (dataInCache.Success)
            {
                foreach (var data in dataInCache.Value)
                {
                    result = result && await RetryDecrement(data.Key, 1);
                }
            }

            return result;
        }

        public async Task<bool> DecreaseUserPurchaseCount(Guid userId, string deviceId, string userTel)
        {
            bool result = true;
            var keys = new List<string>() { GetUserKey(userId.ToString(), "userId"), GetUserKey(userTel, "userTel") };
            if (!string.IsNullOrEmpty(deviceId))
            {
                keys.Add(GetUserKey(deviceId, "deviceId"));
            }

            var dataInCache = await _counter.GetAsync<int>(keys);
            if (dataInCache.Success)
            {
                foreach (var data in dataInCache.Value)
                {
                    result = result && await RetryDecrement(data.Key, 1);
                }
            }

            return result;
        }

        private async Task<ResultModel<string>> IncreasePurchaseCount(Guid userId, string deviceId, string userTel)
        {
            ResultModel<string> result = new ResultModel<string>();
            var activityCount = await _counter.IncrementAsync(GetActivityKey(), 1);
            IResult<double> userIdCount, deviceIdCount = null, userTelCount;
            userIdCount = await _counter.IncrementAsync(GetUserKey(userId.ToString(), "userId"), 1);
            if (!string.IsNullOrEmpty(deviceId))
            {
                deviceIdCount = await _counter.IncrementAsync(GetUserKey(deviceId, "deviceId"), 1);
            }
            userTelCount = await _counter.IncrementAsync(GetUserKey(userTel, "userTel"), 1);

            if (!activityCount.Success || !userIdCount.Success || !userTelCount.Success || (deviceIdCount!= null && !deviceIdCount.Success))
            {
                result.Code = CreateOrderErrorCode.ServerBusy;
            }
            else if (activityCount.Value > _activityLimitCount)
            {
                result.Code = CreateOrderErrorCode.ActivityLimit;
            }
            else if (userIdCount.Value > _userLimitCount || userTelCount.Value > _userLimitCount || (deviceIdCount!= null && deviceIdCount.Value > _userLimitCount))
            {
                result.Code = CreateOrderErrorCode.UserLimit;
            }
            else
            {
                result.Code = CreateOrderErrorCode.ActivitySatisfied;
            }

            // 若判断不符合条件，Decrement
            if (result.Code != CreateOrderErrorCode.ActivitySatisfied)
            {
                if (activityCount.Success)
                {
                    await RetryDecrement(GetActivityKey(), 1);
                }

                if (userIdCount.Success)
                {
                    await RetryDecrement(GetUserKey(userId.ToString(), "userId"), 1);
                }

                if (deviceIdCount != null && deviceIdCount.Success)
                {
                    await RetryDecrement(GetUserKey(deviceId, "deviceId"), 1);
                }

                if (userTelCount.Success)
                {
                    await RetryDecrement(GetUserKey(userTel, "userTel"), 1);
                }
            }

            return result;
        }

        public async Task<ResultModel<string>> CanPurchaseAndIncreaseCount(Guid userId, string deviceId, string userTel)
        {
            ResultModel<string> result = new ResultModel<string>();
            string message = string.Empty;
            
            var currentUserCount = await GetCurrentPurchaseCount(userId, deviceId, userTel);
            var currentActivityCount = await GetCurrentActivityPurchaseCount();

            // 先初步验证
            if (currentActivityCount == null)
            {
                // 未获取到当前活动已购买数量
                result.Code = CreateOrderErrorCode.ServerBusy;
            }
            if (currentActivityCount.Value >= _activityLimitCount)
            {
                // 已达到活动最高限购数量
                result.Code = CreateOrderErrorCode.ActivityLimit;
            }
            else if (currentUserCount == null || currentUserCount.Any(o => o == null))
            {
                // 未获取到用户已购买数量
                result.Code = CreateOrderErrorCode.ServerBusy;
            }
            else if (currentUserCount.Any(o => o.Value >= _userLimitCount))
            {
                // 已达到最高用户限购数量
                result.Code = CreateOrderErrorCode.UserLimit;
            }
            else
            {
                // 二次验证
                result = await IncreasePurchaseCount(userId, deviceId, userTel);
            }

            return result;
        }
        
        /// <summary>
        /// 添加订单记录
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <param name="deviceId"></param>
        /// <param name="userTel"></param>
        /// <returns></returns>
        public async Task<bool> AddOrderRecord(int orderId, Guid userId, string deviceId, string userTel)
        {
            var addResult = await orderHistory.AddAsync($"{orderId}/{userId}/{deviceId}/{userTel}/{DateTime.Now.Ticks}", orderId);

            if(!addResult.Success || !addResult.Value)
            {
                Thread.Sleep(50);
                addResult = await orderHistory.AddAsync($"{orderId}/{userId}/{deviceId}/{userTel}/{DateTime.Now.Ticks}", orderId);
            }

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["type"] = "create";
            data["activityId"] = _activityId.ToString();
            data["userId"] = userId.ToString();
            data["orderId"] = orderId.ToString();
            data["deviceId"] = deviceId;
            data["userTel"] = userTel;
            TuhuNotification.SendNotification("notification.baoyangfixedprice.createorder", data);

            return addResult.Success && addResult.Value;
        }

        /// <summary>
        /// 删除订单记录
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveOrderRecord(int orderId)
        {
            bool result = true;

            var range = new SortedSetScoreRange();
            range.Rang((decimal)orderId, orderId);
            var history = await orderHistory.GetRangeByScoreAsync(range);

            if (history.Success && history.Value != null && history.Value.Any())
            {
                var dataList = history.Value.First().Split('/');
                if (dataList.Length == 5)
                {
                    var userId = Guid.Parse(dataList[1]);
                    var deviceId = dataList[2];
                    var userTel = dataList[3];
                    var dateTime = new DateTime(long.Parse(dataList[4]));

                    result = await DecreaseUserPurchaseCount(userId, deviceId, userTel);

                    if(dateTime >= _round.StartTime && dateTime < _round.EndTime)
                    {
                        result = result && await DecreaseActivityPurchaseCount();
                    }

                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data["type"] = "cancel";
                    data["orderId"] = orderId.ToString();
                    TuhuNotification.SendNotification("notification.baoyangfixedprice.cancelorder", data);
                }
            }

            return result;
        }

        public async Task<bool> ResetPurchaseCount()
        {
            var key = GetActivityKey();
            var removeResult = await _counter.RemoveAsync(key);
            if (!removeResult.Success)
            {
                _logger.Error($"计数器重置失败, {removeResult.Message}", removeResult.Exception);
            }
            return removeResult.Success;
        }


        public async Task<List<string>> GetOrderHistory()
        {
            List<string> result = null;
            var cacheResult = await orderHistory.GetAllAsync();
            if (cacheResult.Success)
            {
                var records = cacheResult.Value;
                result = records?.Where(x =>
                {
                    var ticksStr = x.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)?.LastOrDefault();
                    long ticks;
                    return long.TryParse(ticksStr, out ticks) &&
                        _round.StartTime.Ticks <= ticks &&
                        _round.EndTime.Ticks >= ticks;
                })?.ToList() ?? new List<string>();
            }
            else
            {
                _logger.Error($"缓存获取失败, {cacheResult.Message}", cacheResult.Exception);
            }
            return result;
        }

    }

    public class ResultModel<T>
    {
        public CreateOrderErrorCode Code { get; set; }
    }
}
