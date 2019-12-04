using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.C.ActivityJob.Common;
using Tuhu.C.ActivityJob.Models.Monitor;

namespace Tuhu.C.ActivityJob.ServiceProxy
{
    /// <summary>
    /// 高德地图API代理
    /// </summary>
    public class AMapServiceProxy
    {
        private static readonly HttpClient HttpClient = new HttpClient() { BaseAddress = new Uri("https://restapi.amap.com") };
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AMapServiceProxy));

        /// <summary>
        /// 根据Ip获取定位
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public static async Task<AMapIpResult> GetIpLocationByIp(string ip, string appKey)
        {
            var path = $"/v3/ip?ip={ip}&key={appKey}";

            try
            {
                var response = await HttpClient.GetAsync(path);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var responseError = await response.Content.ReadAsStringAsync();
                    Logger.Error($"根据Ip获取定位失败：{responseError}");
                }
                return await response.Content.ReadAsAMapAsync<AMapIpResult>();
            }
            catch (Exception ex)
            {
                Logger.Error($"根据Ip获取定位失败", ex);
                return null;
            }
        }

        /// <summary>
        /// 根据Gps获取定位
        /// </summary>
        /// <param name="gps"></param>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public static async Task<AMapGpsResult> GetGpsLocationByGps(string gps, string appKey)
        {
            var path = $"/v3/geocode/regeo?location={gps}&key={appKey}";

            try
            {
                var response = await HttpClient.GetAsync(path);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var responseError = await response.Content.ReadAsStringAsync();
                    Logger.Error($"根据Gps获取定位失败：{responseError}");
                }
                return await response.Content.ReadAsAMapAsync<AMapGpsResult>();
            }
            catch (Exception ex)
            {
                Logger.Error($"根据Gps获取定位失败", ex);
                return null;
            }
        }
    }
}
