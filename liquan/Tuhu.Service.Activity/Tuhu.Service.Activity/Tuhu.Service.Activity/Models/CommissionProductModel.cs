using System;

namespace Tuhu.Service.Activity.Models
{
    /// <summary>
    /// 佣金商品实体
    /// </summary>
    public class CommissionProductModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int PKID { get; set; }


        /// <summary>
        /// 业务ID
        /// </summary>
        public Guid CpsId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }


        /// <summary>
        /// 佣金比例
        /// </summary>
        public decimal CommissionRatio { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public int IsEnable { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
