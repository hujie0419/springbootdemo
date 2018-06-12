using Qpl.Api.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Qpl.Api.Request
{
    public class SyncSkuPriceRequest : IQplRequest<SyncSkuPriceResponse>
    {

        public int PageSize { get; set; }
        /// <summary>默认1</summary>
        public int PageIndex { get; set; }

        public DateTime UpdateTime { get; set; }

        /// <summary>提前验证参数。</summary>
        public bool EncryptParam => false;

        public string ApiName
        {
            get { return "Qpl.ProductInfo.GetProductList"; }
        }

        public HttpMethod Method
        {
            get { return HttpMethod.Get; }
        }

        public string Uri
        {
            get { return "ProductInfo/GetProductList"; }
        }

        public SyncSkuPriceRequest()
        {
            PageSize = 20;
            PageIndex = 1;
            UpdateTime = DateTime.Now;
        }

        public object GetParam()
        {
            return $"pageSize={PageSize}&pageIndex={PageIndex}";
        }

        public void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
