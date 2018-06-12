using Qpl.Api.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Request
{
    public class OrderDeliveryRequest : IQplRequest<OrderDeliveryResponse>
    {
        public string OrderNo { get; set; }

        /// <summary>提前验证参数。</summary>
        public bool EncryptParam => false;

        /// <summary>物流类型 1：商品；2：发票</summary>
        public int DeliveryType { get; set; }

        /// <summary>配送方式 0：快递配送；1：商家配送</summary>
        public int DeliveryWay { get; set; }

        public string DeliveryCompany { get; set; }
        public string DeliveryCode { get; set; }


        public string ApiName { get { return "Qcj.CustomerOrder.OrderDelivery"; } }

        public HttpMethod Method { get { return HttpMethod.Post; } }

        public string Uri { get { return "CustomerOrder/OrderDelivery/"; } }

        public object GetParam()
        {
            var dic = new Dictionary<string, object>();
            dic["OrderNo"] = OrderNo;
            dic["DeliveryType"] = DeliveryType;
            dic["DeliveryWay"] = DeliveryWay;
            dic["DeliveryComp"] = DeliveryCompany;
            dic["DeliveryCode"] = DeliveryCode;
            return dic;
        }

        public void Validate() { }

    }
}
