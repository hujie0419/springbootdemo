using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Qpl.Api.Response;

namespace Qpl.Api.Request
{
    public class OrderSearchRequest : IQplRequest<OrderSearchResponse>
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

        private string ShopName = string.Empty;

        public OrderSearchRequest(string shopname= "q汽配龙")
        {
            UpdateDateTime = DateTime.Now.AddDays(-1);
            OrderStatus = 2;
            PageSize = 20;
            Page = 1;
            ShopName = shopname;
            if (shopname == "q汽配龙湖北马牌代理")
            {
#if DEBUG
                TraderName = "13999999999";//13999999999
#else
                TraderName = "13666666666";
#endif
            }
        }

        #region IQplRequest<OrderSearchResponse> 成员

        public string ApiName { get { return "Qpl.CustomerOrder.Search"; } }

        public HttpMethod Method { get { return HttpMethod.Post; } }

        public string Uri
        {
            get
            {
                if (ShopName == "q汽配龙湖北马牌代理")
                {
                    return "CustomerOrder/SearchOrderByCondition";
                }
                else if(ShopName == "q汽配龙清仓")
                {
                    return "CustomerOrder/SearchOrderQplTireStock";
                }
                else
                {
                    return "CustomerOrder/Search";
                }

            }
        }

        public object GetParam()
        {
            var param = new Dictionary<string, object>();

            param["updateDate"] = UpdateDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            param["orderStatus"] = OrderStatus;
            param["pageSize"] = PageSize;
            param["pageNo"] = Page;
            if (ShopName == "q汽配龙湖北马牌代理")
            {
                param["TraderName"] = TraderName;
            }
            return param;
        }

        public void Validate()
        {
        }

#endregion
    }
}
