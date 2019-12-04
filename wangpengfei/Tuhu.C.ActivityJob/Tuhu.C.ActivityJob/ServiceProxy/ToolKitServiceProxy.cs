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
    public class ToolKitServiceProxy
    {
        private static readonly HttpClient HttpClient = new HttpClient() { BaseAddress = new Uri("http://toolkit.ad.tuhu.cn:9010") };
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ToolKitServiceProxy));

        /// <summary>
        /// 根据手机号获取归属地
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public static async Task<Mobile2AreaResult> GetLocationByMobile(string mobile, string appKey)
        {
            var path = $"/api/v1/addressPhone?phone={mobile}&token={appKey}";

            try
            {
                var response = await HttpClient.GetAsync(path);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var responseError = await response.Content.ReadAsStringAsync();
                    Logger.Error($"根据手机号获取归属地失败：{responseError}");
                }
                return await response.Content.ReadAsTuHuAsync<Mobile2AreaResult>();
            }
            catch (Exception ex)
            {
                Logger.Error($"根据手机号获取归属地失败", ex);
                return null;
            }
        }
    }
}
