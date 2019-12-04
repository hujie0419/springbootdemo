using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json;
using Tuhu.C.ActivityJob.Common;
using Tuhu.C.ActivityJob.Models.Rebate;

namespace Tuhu.C.ActivityJob.ServiceProxy
{
    /// <summary>
    /// 门店API代理
    /// </summary>
    public class ShopReceiveServiceProxy
    {
        private static readonly HttpClient _httpClient = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["ShopGateway"]) };
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ShopReceiveServiceProxy));

        /// <summary>
        /// 根据订单查询派工技师
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        public static async Task<ShopOrderTechResult> DispatchTechByOrder(long[] orderIds)
        {
            var path = $"/shop-receive/cl-receive/dispatch-tech-by-order";

            try
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(new
                {
                    orderIds = orderIds
                }));

                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await _httpClient.PostAsync(path, httpContent);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var responseError = await response.Content.ReadAsStringAsync();
                    _logger.Error($"根据订单查询派工技师失败：{responseError}");
                }
                return await response.Content.ReadAsAsync<ShopOrderTechResult>();
            }
            catch (Exception ex)
            {
                _logger.Error($"根据订单查询派工技师失败", ex);
                return null;
            }
        }
    }
}
