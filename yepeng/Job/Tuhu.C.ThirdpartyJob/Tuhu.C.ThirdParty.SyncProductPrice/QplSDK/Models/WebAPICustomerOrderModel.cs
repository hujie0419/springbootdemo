using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Models
{
    [Serializable]
    public class WebAPICustomerOrderModel
    {
        /// <summary>
        /// 订单唯一id
        /// </summary>
        public string UID { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string PIID { get; set; }

        /// <summary>
        /// 订单编号,这是订单编号，在支付环节使用的
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 最终支付金额（销售修改后的最终原始金额）
        /// </summary>
        public decimal TotalFinalPrice { get; set; }

        /// <summary>
        /// 汽配龙： 状态固定为8
        /// </summary>
        public int? OrderStatus { get; set; }
        //public string OrderStatus { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 经销商ID(商家编号)
        /// </summary>
        public string SalesId { get; set; }
        /// <summary>
        /// 买家备注
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// 订单明细信息
        /// </summary>
        public List<WebAPICustomerOrderItemModel> OrderItems { get; set; }

        public WebAPICustomerOrderModel()
        {
            OrderItems = new List<WebAPICustomerOrderItemModel>();
        }

    }
}
