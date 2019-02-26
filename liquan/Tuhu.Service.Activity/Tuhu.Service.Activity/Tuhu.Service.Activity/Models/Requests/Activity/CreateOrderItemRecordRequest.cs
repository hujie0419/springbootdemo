using System;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// 佣金订单商品记录接口请求实体
    /// </summary>
    public class CreateOrderItemRecordRequest
    {
        /// <summary>
        /// 父级订单号 拆单时使用
        /// </summary>
        public string FOrderId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 订单商品集合
        /// </summary>
        public List<OrderItemRecord> OrderItem { get; set; }
    }

    /// <summary>
    /// 商品订单记录
    /// </summary>
    public class OrderItemRecord
    {
        /// <summary>
        /// 达人ID
        /// </summary>
        public Guid DarenID { get; set; }

        /// <summary>
        /// 佣金商品业务ID
        /// </summary>
        public Guid CpsId { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int Number { get; set; }

    }
}
