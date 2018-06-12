using Qpl.Api.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Request
{
    public class SyncProductInfoRequest : IQplRequest<SyncProductInfoResponse>
    {
        public string Pid { get; set; }

        /// <summary>提前验证参数。</summary>
        public bool EncryptParam => false;

        public string ApiName
        {
            get { return "Qpl.ProductInfo.GetUserProductInfoByPID"; }
        }

        public HttpMethod Method
        {
            get { return HttpMethod.Get; }
        }

        public string Uri
        {
            get { return "ProductInfo/GetUserProductInfoByPID"; }
        }

        public object GetParam()
        {
            return $"?pid={Pid}";
        }

        public void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
