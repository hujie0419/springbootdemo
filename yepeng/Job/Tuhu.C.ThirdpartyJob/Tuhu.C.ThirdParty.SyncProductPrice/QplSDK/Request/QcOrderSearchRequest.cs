using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Qpl.Api.Response;

namespace Qpl.Api.Request
{
    public class QcOrderSearchRequest : IQplRequest<QcOrderSearchResponse>
    {
        /// <summary>DateTime.Now.AddDays(-1)</summary>
        public DateTime UpdateDateTime { get; set; }
        /// <summary>订单状态=1：待付款；2：待发货；3：待收货；4：待评价；5：退款申请；6：退款审核；7：退款成功；8：关闭；</summary>
        public int OrderStatus { get; set; }
        /// <summary>默认20</summary>
        public int PageSize { get; set; }
        /// <summary>默认1</summary>
        public int Page { get; set; }

        public string TraderName { get; set; }

        /// <summary>提前验证参数。</summary>
        public bool EncryptParam => false;


        public QcOrderSearchRequest()
        {
            UpdateDateTime = DateTime.Now.AddDays(-1);
            OrderStatus = 2;
            PageSize = 20;
            Page = 1;
        }

        #region IQplRequest<QcOrderSearchResponse> 成员

        public string ApiName { get { return "Qpl.CustomerOrder.Search"; } }

        public HttpMethod Method { get { return HttpMethod.Post; } }

        public string Uri
        {
            get
            {
                return "CustomerOrder/SearchOrderQplTireStock";
            }
        }

        public object GetParam()
        {
            var param = new Dictionary<string, object>();
            param["updateDate"] = UpdateDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            param["orderStatus"] = OrderStatus;
            param["pageSize"] = PageSize;
            param["pageNo"] = Page;
            return param;
        }

        public void Validate()
        {
        }

        #endregion
    }
}
