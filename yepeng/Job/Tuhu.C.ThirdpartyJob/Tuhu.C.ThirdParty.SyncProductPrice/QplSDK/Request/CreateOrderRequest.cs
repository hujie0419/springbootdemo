using Qpl.Api.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Qpl.Api.Models;

namespace Qpl.Api.Request
{
    public class CreateOrderRequest : IQplRequest<CreateOrderResponse>
    {
       // public WebAPICustomerOrderModel Order { get; set; }
       public string coustomOrder { get; set; }
        public string ApiName
        {
            get { return "Qpl.CustomerOrder.CreateCoustomerOrder"; }
        }

        public HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }

        public string Uri
        {
            get { return "CustomerOrder/CreateCoustomerOrder"; }
        }

        /// <summary>提前验证参数。</summary>
        public bool EncryptParam => false;

       // public object GetParam() => Order;

        public object GetParam() => coustomOrder;

        public void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
