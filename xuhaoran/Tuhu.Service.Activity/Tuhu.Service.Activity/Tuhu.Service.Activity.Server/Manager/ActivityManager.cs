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
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.DataAccess;

namespace Tuhu.Service.Activity.Server.Manager
{
    public static class ActivityManager
    {
        public static readonly string DefaultClientName = "Activity";
        public static readonly string WashCarActivityClientName = "UserApplyActivitySortedSetCache";
        public static readonly TimeSpan ActivityCacheExpiration = TimeSpan.FromHours(1);
        public static readonly ILog Logger = LogManager.GetLogger(typeof(ActivityManager));

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
        /// <summary>
        /// 获取所有可用活动
        /// </summary>
        /// <returns></returns>
        public static async Task<List<T_Activity_xhrModel>> GetAllActivityAsync(int pageIndex, int pageSize)
        {
            using (var hashClient = CacheHelper.CreateHashClient("GetAllActivity", ActivityCacheExpiration))
            {
                IEnumerable<T_Activity_xhrModel> xhrActivity;
                var getresult =hashClient.GetAsync<IEnumerable<T_Activity_xhrModel>>("GetAllActivityAsync/" + pageIndex.ToString());
                if (!getresult.Result.Success)
                {
                    Logger.Warn($"查询redis数据失败GetActivity:{"GetAllActivityAsync/" + pageIndex.ToString()};Error:{getresult.Result.Message}", getresult.Exception);
                    return null;
                }
                else
                {
                    if (getresult.Result.Value.Any() && getresult.Result.Value != null)
                    {
                        xhrActivity = getresult.Result.Value;
                    }
                    else
                    {
                        var result = DalActivity.GetAllActivityAsync(pageIndex, pageSize);

                        var dic = new Dictionary<string, object>();
                        dic.Add("GetAllActivityAsync" + pageIndex.ToString(), result.Result);
                        IReadOnlyDictionary<string, object> roDic = new ReadOnlyDictionary<string, object>(dic);
                        var setResult = await hashClient.SetAsync<IEnumerable<T_Activity_xhrModel>>(roDic);
                        if (setResult.Success)
                        {
                            xhrActivity = result.Result;
                        }
                        else
                        {
                            Logger.Warn($"存储redis数据失败SetActivity:{"GetAllActivityAsync/" + pageIndex.ToString()};Error:{setResult.Message}", setResult.Exception);
                            return null;
                        }
                    }
                }

                if (xhrActivity != null && xhrActivity.Any())
                {
                    return xhrActivity.Where(_ => _.StartTime <= DateTime.Now && _.EndTime > DateTime.Now)?.ToList() ?? new List<T_Activity_xhrModel>();
                }
                return null;

                
            }
        }

        /// <summary>
        /// 获取所有活动
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<T_Activity_xhrModel>> GetAllActivityManagerAsync(int pageIndex, int pageSize)
        {
            try
            {

                return  await DalActivity.GetAllActivityManagerAsync(pageIndex, pageSize);

                    
            }
            catch (Exception ex)
            {
                Logger.Error($"GetAllActivityManagerAsync", ex);
                throw ex;
            }
            
        }

        /// <summary>
        /// 新增活动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<bool> AddActivityAsync(T_Activity_xhrModel request)
        {
            using (var hashClient = CacheHelper.CreateHashClient("GetAllActivity"))
            {
                try
                {

                    var result= await DalActivity.AddActivityAsync(request);
                    hashClient.RemoveSelf();
                    return result;
                
                }
                catch (Exception ex)
                {
                    Logger.Error($"AddActivityAsync -> {JsonConvert.SerializeObject(request)}", ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 修改活动信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateActivityAsync(T_Activity_xhrModel request)
        {
            using (var hashClient = CacheHelper.CreateHashClient("GetAllActivity"))
            {
                try
                {
                    var result = await DalActivity.UpdateActivityAsync(request);
                    hashClient.RemoveSelf();
                    return result;
                }
                catch (Exception ex)
                {
                    Logger.Error($"UpdateActivityAsync -> {JsonConvert.SerializeObject(request)}", ex);
                    throw ex;
                }
            }
        }

            /// <summary>
            /// 活动报名
            /// </summary>
            /// <param name="request"></param>
            /// <returns></returns>
            public static async Task<bool> AddActivitiesUserAsync(ActivityUserInfo_xhrRequest request)
        {
            try
            {
                return await DalActivity.AddActivitiesUserAsync(request);
            }
            catch (Exception ex)
            {
                Logger.Error($"AddActivitiesUserAsync -> {JsonConvert.SerializeObject(request)}", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 修改报名信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateActivitiesUserAsync(ActivityUserInfo_xhrRequest request)
        {
            try
            {
                return await DalActivity.UpdateActivitiesUserAsync(request);
            }
            catch (Exception ex)
            {
                Logger.Error($"UpdateActivitiesUserAsync -> {JsonConvert.SerializeObject(request)}", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取所有活动地区
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<T_ArearModel>> GetAllAreaAsync()
        {
            var key = $"GetAllAreaAsync";
            using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
            {
                var result = await client.GetOrSetAsync(key, () => DalActivity.GetAllAreaAsync(), ActivityCacheExpiration);
                IEnumerable<T_ArearModel> xhrActivity;
                if (result.Success)
                {
                    xhrActivity = result.Value;
                }
                else
                {
                    Logger.Warn($"获取redis数据失败SelectAllArea:{key};Error:{result.Message}", result.Exception);
                    // redis查询失败查数据库
                    xhrActivity = await DalActivity.GetAllAreaAsync();
                }

                if (xhrActivity != null && xhrActivity.Any())
                {
                    return xhrActivity;
                }
                return null;
            }
        }

        /// <summary>
        /// 根据地区查询用户信息
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<T_ActivityUserInfo_xhrModels>> GetActivityUserInfoByAreaAsync(int areaId, int pageIndex, int pageSize)
        {
            IEnumerable<T_ActivityUserInfo_xhrModels> xhrActivity;
            xhrActivity = await DalActivity.GetActivityUserInfoByAreaAsync(areaId, pageIndex, pageSize);

            if (xhrActivity != null && xhrActivity.Any())
            {
                return xhrActivity;
            }
            return null;
        }

        /// <summary>
        /// 审核活动
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> ReviewActivityTaskAsync()
        {
            try
            {
                return await DalActivity.ReviewActivityTaskAsync();
            }
            catch (Exception ex)
            {
                Logger.Error($"ReviewActivityTask", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<string> ManagerLoginAsync(T_ActivityManagerUserInfo_xhrModel request)
        {
            try
            {
                return await DalActivity.ManagerLoginAsync(request);
            }
            catch (Exception ex)
            {
                Logger.Error($"ManagerLogin", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 管理员注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<bool> ManagerRegisterAsync(T_ActivityManagerUserInfo_xhrModel request)
        {
            try
            {
                return await DalActivity.ManagerRegisterAsync(request);
            }
            catch (Exception ex)
            {
                Logger.Error($"ManagerRegister", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取密码盐
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetPassWordsSalt(T_ActivityManagerUserInfo_xhrModel request)
        {
            try
            {
                return DalActivity.GetPassWordsSalt(request);
            }
            catch (Exception ex)
            {
                Logger.Error($"GetPassWordsSalt", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 验证登录状态
        /// </summary>
        /// <param name="managerId"></param>
        /// <returns></returns>
        public static async Task<bool> CheckLoginAsync(int managerId)
        {
            try
            {
                return await DalActivity.CheckLoginAsync(managerId);
            }
            catch (Exception ex)
            {
                Logger.Error($"ManagerRegister", ex);
                throw ex;
            }
        }

    }
}

