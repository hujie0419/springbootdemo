using Qpl.Api.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Qpl.Api.Request
{
    /// <summary>
    /// 全车件订单
    /// </summary>
    public class QcjOrderSearchRequest : IQplRequest<QcjOrderSearchResponse>
    {
        /// <summary>DateTime.Now.AddDays(-1)</summary>
        public DateTime UpdateDateTime { get; set; }
        /// <summary>订单状态=1：待付款；2：待发货；3：待收货；4：待评价；5：退款申请；6：退款审核；7：退款成功；8：关闭；</summary>
        public int OrderStatus { get; set; }
        /// <summary>默认20</summary>
        public int PageSize { get; set; }
        /// <summary>默认1</summary>
        public int Page { get; set; }

        public QcjOrderSearchRequest()
        {
            UpdateDateTime = DateTime.Now.AddDays(-1);
            OrderStatus = 2;
            PageSize = 20;
            Page = 1;
        }

        public string ApiName
        {
            get
            {
                return "Qcj.CustomerOrder.SearchQcjOrder";
            }
        }

        public bool EncryptParam
        {
            get
            {
                return false;
            }
        }

        public HttpMethod Method
        {
            get
            {
                return HttpMethod.Post;
            }
        }

        public string Uri
        {
            get
            {
                return "CustomerOrder/SearchOrder";
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
            //Do nothing.
        }
    }
}
