using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess.Models;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Activity.Server.Model;

namespace Tuhu.Service.Activity.Server.Manager
{
    /// <summary>
    ///     公众号领红包 - 红包缓存逻辑类
    /// </summary>
    internal class OARedEnvelopeCacheManager
    {
        private readonly Nosql.CacheClient _cacheClient;

        private readonly CounterClient _counterClient;

        // 每日红包Hash类
        private readonly HashClient _hashClient;
        private readonly DateTime _now;
        private readonly int _officialAccountType;
        private readonly string _today;

        public OARedEnvelopeCacheManager(int officialAccountType, DateTime now)
        {
            _officialAccountType = officialAccountType;
            _now = now;
            var cacheName = GlobalConstant.OARedEnvelopeCacheHeader;
            _cacheClient = CacheHelper.CreateCacheClient(cacheName);
            _counterClient = CacheHelper.CreateCounterClient(cacheName, TimeSpan.FromDays(2));
            _today = _now.ToString("yyyyMMdd");
            _hashClient =
                CacheHelper.CreateHashClient(cacheName + $":{officialAccountType}:{_today}", TimeSpan.FromDays(2));
        }

        /// <summary>
        ///     增加红包数量  返回当前数量 原子 - 缓存
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<long> IncrementNumAsync(int count)
        {
            var result = await _counterClient.IncrementAsync(GetCountCacheKey + $":{_today}", count);


            return result.Value;
        }

        /// <summary>
        ///     返回红包数量
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<long> IncrementCountAsync()
        {
            var result = await _counterClient.CountAsync(GetCountCacheKey + $":{_today}");
            return result.Value;
        }

        /// <summary>
        ///     获取一个有效的红包对象 - 缓存
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public async Task<OARedEnvelopeObjectModel> GetOARedEnvelopeObjectAsync(long position)
        {
            var result = await _hashClient.GetAsync<OARedEnvelopeObjectModel>(position.ToString());

            return result?.Value;
        }

        /// <summary>
        ///     获取一个生成的全部红包对象 - 缓存
        /// </summary>
        /// <returns></returns>
        public async Task<IDictionary<string, OARedEnvelopeObjectModel>> GetAllOARedEnvelopeObjectAsync()
        {
            var result = await _hashClient.GetAllAsync<OARedEnvelopeObjectModel>();

            return result?.Value;
        }

        /// <summary>
        ///     设置红包对象 - 缓存
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<bool> SaveOARedEnvelopeObjectListAsync(List<OARedEnvelopeObjectModel> list)
        {
            await Task.Yield();
            Parallel.ForEach(list, new ParallelOptions {MaxDegreeOfParallelism = 10},
                (p, loopState) => { _hashClient.Set(p.Position.ToString(), p); });

            return true;
        }

        /// <summary>
        ///     删除红包对象 - 缓存
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteOARedEnvelopeObjectListAsync()
        {
            await _hashClient.RemoveSelfAsync();
            return true;
        }


        /// <summary>
        ///     保存用户领取的红包对象  - 缓存
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveUserOARedEnvelopeObjectAsync(OARedEnvelopeBuilderModel oaRedEnvelopeBuilderModel)
        {
            var cacheKey = string.Format(GlobalConstant.OARedEnvelopeUserGetCache, _officialAccountType,
                oaRedEnvelopeBuilderModel.UserId);
            var setResult = await _cacheClient.SetAsync(
                cacheKey,
                oaRedEnvelopeBuilderModel,
                TimeSpan.FromDays(1));
            await _counterClient.IncrementAsync(cacheKey + ":Incr", 1);
            return setResult.Success;
        }

        /// <summary>
        ///     删除用户领取的红包对象 - 缓存
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveUserOARedEnvelopeObjectAsync(Guid userId)
        {
            var cacheKey = string.Format(GlobalConstant.OARedEnvelopeUserGetCache, _officialAccountType,
                userId);
            await Task.WhenAll(
                _cacheClient.RemoveAsync(cacheKey),
                _counterClient.IncrementAsync(cacheKey + ":Incr", -1)
            );
            return true;
        }

        /// <summary>
        ///     获取用户领取的红包对象 非原子操作 - 缓存
        /// </summary>
        /// <returns></returns>
        public async Task<OARedEnvelopeBuilderModel> GetUserOARedEnvelopeObjectAsync(Guid userId)
        {
            var setResult = await _cacheClient.GetAsync<OARedEnvelopeBuilderModel>(
                string.Format(GlobalConstant.OARedEnvelopeUserGetCache, _officialAccountType, userId));
            return setResult.Value;
        }

        /// <summary>
        ///     获取用户领取的红包对象 是否存在 原子操作 - 缓存
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> GetUserOARedEnvelopeObjectExistsAsync(Guid userId)
        {
            var cacheKey = string.Format(GlobalConstant.OARedEnvelopeUserGetCache, _officialAccountType,
                userId);

            var getResult = await _counterClient.CountAsync(cacheKey + ":Incr");
            return getResult.Value > 0;
        }


        /// <summary>
        ///     统计 - 缓存
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StatisticsDoingAsync(int count, decimal money)
        {
            // 总数
            var task1 = _counterClient.IncrementAsync(GetStatisticsCacheKey + ":" + _today + ":count", count);
            // 总金额
            var task2 = _counterClient.IncrementAsync(GetStatisticsCacheKey + ":" + _today + ":summoney",
                Convert.ToInt32(money * 100));
            await Task.WhenAll(task1, task2);
            return true;
        }

        /// <summary>
        ///     获取某一日的统计数据 - 缓存
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<Tuple<long, decimal>> GetStatisticsAsync()
        {
            // 总数
            var task1 = _counterClient.IncrementAsync(GetStatisticsCacheKey + ":" + _today + ":count", 0);
            // 总金额
            var task2 = _counterClient.IncrementAsync(GetStatisticsCacheKey + ":" + _today + ":summoney", 0);
            await Task.WhenAll(task1, task2);

            return Tuple.Create(task1.Result.Value, (decimal) task2.Result.Value / 100);
        }

        /// <summary>
        ///     保存每日设置 - 到缓存 - 缓存
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveDailySettingAsync(OARedEnvelopeSettingModel settingModel)
        {
            var cacheKey = GlobalConstant.OARedEnvelopeStatisticsCache + $":{_officialAccountType}:{_today}";
            // 保存到缓存中
            var setResult = await _cacheClient.SetAsync(
                cacheKey,
                settingModel,
                TimeSpan.FromDays(3));
            return true;
        }

        /// <summary>
        ///     获取缓存中的每日设置 - 缓存
        /// </summary>
        /// <returns></returns>
        public async Task<OARedEnvelopeSettingModel> GetDailySettingAsync()
        {
            var cacheKey = GlobalConstant.OARedEnvelopeStatisticsCache + $":{_officialAccountType}:{_today}";
            // 保存到缓存中
            var setResult = await _cacheClient.GetAsync<OARedEnvelopeSettingModel>(
                cacheKey);
            return setResult.Value;
        }

        /// <summary>
        ///     删除每日设置 - 到缓存 - 缓存
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteDailySettingAsync()
        {
            var cacheKey = GlobalConstant.OARedEnvelopeStatisticsCache + $":{_officialAccountType}:{_today}";
            // 删除
            await _cacheClient.RemoveAsync(
                cacheKey);
            return true;
        }

        #region CacheKey

        /// <summary>
        ///     总数的缓存KEY
        /// </summary>
        private string GetCountCacheKey => GlobalConstant.OARedEnvelopeCountCache + $":{_officialAccountType}";

        /// <summary>
        ///     每日统计的数量
        /// </summary>
        private string GetStatisticsCacheKey =>
            GlobalConstant.OARedEnvelopeStatisticsCache + $":{_officialAccountType}";

        #endregion
    }
}
