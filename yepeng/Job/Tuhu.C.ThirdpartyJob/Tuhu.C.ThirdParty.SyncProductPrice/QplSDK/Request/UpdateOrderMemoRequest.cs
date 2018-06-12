using Qpl.Api.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Qpl.Api.Request
{
    public class UpdateOrderMemoRequest : IQplRequest<UpdateMemoResponse>
    {
        public string OrderNo { get; set; }
        public string Remark { get; set; }

        /// <summary>提前验证参数。</summary>
        public bool EncryptParam => false;

        
        public string ApiName { get { return "Qcj.CustomerOrder.UpdateOrderMemo"; } }

        public HttpMethod Method { get { return HttpMethod.Post; } }

        public string Uri { get { return "CustomerOrder/UpdateOrderMemo/"; } }

        public object GetParam()
        {
            var param = new Dictionary<string, object>();

            param["OrderNo"] = OrderNo;
            param["Remark"] = Remark;
            return param;
        }

        public void Validate() { }
    }
}