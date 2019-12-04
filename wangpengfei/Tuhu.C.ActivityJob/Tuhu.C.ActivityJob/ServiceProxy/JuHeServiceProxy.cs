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
    /// 聚合数据API代理
    /// </summary>
    public class JuHeServiceProxy
    {
        private static readonly HttpClient HttpClient = new HttpClient() { BaseAddress = new Uri("http://apis.juhe.cn") };
        private static readonly ILog Logger = LogManager.GetLogger(typeof(JuHeServiceProxy));

        /// <summary>
        /// 根据手机号获取归属地
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public static async Task<JuHeMobileResult> GetLocationByMobile(string mobile, string appKey)
        {
            var path = $"/mobile/get?phone={mobile}&key={appKey}";

            try
            {
                var response = await HttpClient.GetAsync(path);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var responseError = await response.Content.ReadAsStringAsync();
                    Logger.Error($"根据手机号获取归属地失败：{responseError}");
                }
                return await response.Content.ReadAsAsync<JuHeMobileResult>();
            }
            catch (Exception ex)
            {
                Logger.Error($"根据手机号获取归属地失败", ex);
                return null;
            }
        }
    }
}
