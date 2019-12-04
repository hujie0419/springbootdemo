using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.C.ActivityJob.Dal.Monitor;
using Tuhu.C.ActivityJob.Models.Monitor;
using Tuhu.C.ActivityJob.ServiceProxy;

namespace Tuhu.C.ActivityJob.Job.Monitor
{
    public class UserLocationMonitorSyncJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserLocationMonitorSyncJob));

        private int MaxSyncCount => 50;

        public void Execute(IJobExecutionContext context)
        {
            var startTime = DateTime.Now.AddHours(-1);
            int totalCount = UserLocationMonitorDal.GetLocationMonitorCount(startTime);

            int pageNum = (totalCount + MaxSyncCount - 1) / MaxSyncCount;
            Logger.Info($"同步用户区域信息开始.一共{totalCount}条记录.一共{pageNum}批次.");

            var appKeyDict = GetUserLocationMonitorAPPKey();
            if (appKeyDict != null && appKeyDict.ContainsKey("AMapAPPKey") && appKeyDict.ContainsKey("TuHuAPPKey"))
            {
                var aMapAPPKey = appKeyDict["AMapAPPKey"];
                var tuHuAPPKey = appKeyDict["TuHuAPPKey"];

                int maxPkid = 0;
                for (int index = 0; index <= pageNum; index++)
                {
                    var monitors = UserLocationMonitorDal.GetLocationMonitors(MaxSyncCount, startTime, ref maxPkid);
                    if (monitors != null && monitors.Any())
                    {
                        foreach (var item in monitors)
                        {
                            if (!string.IsNullOrWhiteSpace(item.Gps) && string.IsNullOrEmpty(item.GpsLocation))
                            {
                                item.GpsLocation = GetGpsLocationByGps(item.Gps, aMapAPPKey);
                            }

                            if (!string.IsNullOrWhiteSpace(item.Ip) && string.IsNullOrEmpty(item.IpLocation))
                            {
                                item.IpLocation = GetIpLocationByIp(item.Ip, aMapAPPKey);
                            }

                            if (!string.IsNullOrWhiteSpace(item.Mobile) && string.IsNullOrEmpty(item.MobileLocation))
                            {
                                // 聚合数据API调用限制：1000次/日，避免到达上限
                                //item.MobileLocation = UserLocationMonitorDal.GetMobileLocation(item.Mobile);
                                //if (string.IsNullOrEmpty(item.MobileLocation))
                                //{
                                //    item.MobileLocation = GetLocationByMobile_JuHe(item.Mobile, juHeAPPKey);
                                //}

                                item.MobileLocation = GetLocationByMobile_TuHu(item.Mobile, tuHuAPPKey);
                            }

                            UserLocationMonitorDal.UpdateLocationMonitor(item);
                        }
                    }

                    Logger.Info($"结束同步第{index}批次,一共{pageNum}批次.MaxPkid:{maxPkid}");
                }
            }
        }

        private Dictionary<string, string> GetUserLocationMonitorAPPKey()
        {
            var appKeyDict = new Dictionary<string, string>();
            AsyncHelper.RunSync(async () => appKeyDict = await ConfigBaseServiceProxy.GetBaseConfigList("UserLocationMonitor"));
            return appKeyDict;
        }

        private string GetIpLocationByIp(string ip, string appKey)
        {
            var aMapIp = new AMapIpResult();
            AsyncHelper.RunSync(async () => aMapIp = await AMapServiceProxy.GetIpLocationByIp(ip, appKey));
            if (aMapIp != null)
            {
                if (aMapIp.Status == 1)
                {
                    return $"{aMapIp.Province}{aMapIp.City}";
                }
                else
                {
                    Logger.Warn($"GetIpLocationByIp Error：{aMapIp.Info}");
                }
            }
            return null;
        }

        private string GetGpsLocationByGps(string gps, string appKey)
        {
            var aMapGps = new AMapGpsResult();
            AsyncHelper.RunSync(async () => aMapGps = await AMapServiceProxy.GetGpsLocationByGps(gps, appKey));
            if (aMapGps != null)
            {
                if (aMapGps.Status == 1)
                {
                    return aMapGps.RegeoCode?.Formatted_Address;
                }
                else
                {
                    Logger.Warn($"GetGpsLocationByGps Error：{aMapGps.Info}");
                }
            }
            return null;
        }

        private string GetLocationByMobile_TuHu(string mobile, string appKey)
        {
            var mobile2AreaMobile = new Mobile2AreaResult();
            AsyncHelper.RunSync(async () => mobile2AreaMobile = await ToolKitServiceProxy.GetLocationByMobile(mobile, appKey));
            if (mobile2AreaMobile != null)
            {
                if (mobile2AreaMobile.Code == 200)
                {
                    if (!string.IsNullOrEmpty(mobile2AreaMobile.Data?.Operator))
                    {
                        mobile2AreaMobile.Data.Operator = mobile2AreaMobile.Data.Operator.Replace("中国", string.Empty).Replace("网络", string.Empty);
                    }
                    return $"{mobile2AreaMobile.Data?.Province}{mobile2AreaMobile.Data?.City}{mobile2AreaMobile.Data?.Operator}";
                }
                else
                {
                    Logger.Warn($"GetLocationByMobile Error：{mobile2AreaMobile.Msg}");
                }
            }
            return null;
        }

        private string GetLocationByMobile_JuHe(string mobile, string appKey)
        {
            var juHeMobile = new JuHeMobileResult();
            AsyncHelper.RunSync(async () => juHeMobile = await JuHeServiceProxy.GetLocationByMobile(mobile, appKey));
            if (juHeMobile != null)
            {
                if (juHeMobile.ResultCode == 200)
                {
                    return $"{juHeMobile.Result?.Province}{juHeMobile.Result?.City}{juHeMobile.Result?.Company}";
                }
                else
                {
                    Logger.Warn($"GetLocationByMobile Error：{juHeMobile.Reason}");
                }
            }
            return null;
        }
    }
}
