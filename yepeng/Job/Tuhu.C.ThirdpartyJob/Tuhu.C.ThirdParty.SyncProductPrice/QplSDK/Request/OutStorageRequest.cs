using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Qpl.Api.Response;
using Qpl.Api.Models;

namespace Qpl.Api.Request
{
    public class OutStorageRequest : IQplRequest<OutStorageResponse>
    {
        public string OrderID { get; set; }

        /// <summary>提前验证参数。</summary>
        public bool EncryptParam => false;

        public IEnumerable<OutStorageModel> Deliveries { get; set; }

        #region IQplRequest<UpdateSalesNotesResponse> 成员
        public string ApiName { get { return "Qpl.CustomerOrder.OutStorageById"; } }

        public HttpMethod Method { get { return HttpMethod.Put; } }

        public string Uri { get { return "CustomerOrder/OutStorageById/" + OrderID; } }

        public object GetParam()
        {
            var param = new List<IDictionary<string, object>>();

            if (Deliveries != null)
                foreach (var item in Deliveries)
                {
                    var dic = new Dictionary<string, object>();

                    dic["DeliveryType"] = item.DeliveryType;
                    dic["DeliveryWay"] = item.DeliveryWay;
                    dic["DeliveryComp"] = item.DeliveryCompany;
                    dic["DeliveryCode"] = item.DeliveryCode;

                    param.Add(dic);
                }

            return param;
        }

        public void Validate() { }

        #endregion
    }
}
