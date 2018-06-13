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
    }
}

