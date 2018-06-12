using PddSdk.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace PddSdk.Request
{
    public class PddLogisticsSendRequest : IPddRequest<PddLogisticsSendResponse>
    {
        public PddLogisticsSendRequest(string mallId, string secret)
        {
            Type = "pdd.logistics.online.send";
            MallId = mallId;
            Secret = secret;
        }
        /// <summary>
        ///三方订单号
        /// </summary>
        public string OrderSn { get; set; }
        /// <summary>
        /// 快递公司编号
        /// </summary>
        public string LogisticsId { get; set; }
        /// <summary>
        /// 快递单号
        /// </summary>
        public string TrackingNumber { get; set; }
        /// <summary>
        /// 接口名称
        /// </summary>
        public string Type { get; set; }
        public HttpMethod Method => HttpMethod.Post;

        public string Uri => string.Empty;

        /// <summary>
        /// 拼多多后台，服务市场->公共接口 页面的接入码
        /// </summary>
        public string MallId { get; set; }
        private Dictionary<string, object> RequestParam
        {
            get
            {
                var param = new Dictionary<string, object>
                {
                    ["type"] = Type,
                    ["data_type"] = "JSON",
                    ["timestamp"] = CommonHelper.ToTimestamp(DateTime.Now.AddHours(-8)),
                    ["mall_id"] = MallId,
                    ["order_sn"] = OrderSn,
                    ["logistics_id"] = LogisticsId,
                    ["tracking_number"] = TrackingNumber
                };
                return param;
            }
        }
        public object GetParam()
        {
            var param = RequestParam;
            param.Add("sign", Sign);
            return param;
        }

        private string Sign
        {
            get
            {
                var result = RequestParam.OrderBy(_ => _.Key).Aggregate(string.Empty, (current, p) => current + (p.Key + p.Value));
                return CommonHelper.GetMd5(Secret + result + Secret, Encoding.UTF8);
            }
        }
        /// <summary>
        /// 接口密码
        /// </summary>
        public string Secret { get; set; }
        public void Validate() { }
    }
}
