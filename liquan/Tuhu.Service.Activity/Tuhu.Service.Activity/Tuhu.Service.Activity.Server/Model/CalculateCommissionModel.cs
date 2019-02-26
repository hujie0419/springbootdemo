using System;
using Tuhu.Service.Order.Models;

namespace Tuhu.Service.Activity.Server.Model
{
    public class CalculateCommissionModel
    {
        /// <summary>
        /// 订单商品实体
        /// </summary>
        public OrderListModel OrderItem { get; set; }

        /// <summary>
        /// 佣金商品业务ID
        /// </summary>
        public Guid CpsId { get; set; }

        /// <summary>
        /// 达人ID
        /// </summary>
        public Guid DarenId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int Number { get; set; }

        #region 退款时使用

        /// <summary>
        /// 佣金比例
        /// </summary>
        public decimal CommissionRatio { get; set; }

        /// <summary>
        /// 原订单号
        /// </summary>
        public string OriOrderId { get; set; }

        /// <summary>
        /// 原支付系统订单号，支付完成金融返回
        /// </summary>
        public string OriPayInstructionId { get; set; }
        #endregion
    }
}
