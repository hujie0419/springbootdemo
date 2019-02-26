using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{
    public class CpsSplitOrderItemRecordModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int PkId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 拆单父订单的PKID
        /// </summary>
        public int CpsOrderItemRecordID { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
