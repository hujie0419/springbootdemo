using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{
    public class CpsProductListModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int PkId { get; set; }

        /// <summary>
        /// 业务ID唯一值商品下单时记录,获取佣金比例使用
        /// </summary>
        public Guid CpsId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 佣金比例
        /// </summary>
        public decimal CommissionRatio { get; set; }

        /// <summary>
        /// 是否启用; true:是; false:否;
        /// </summary>
        public bool IsEnable { get; set; }
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
