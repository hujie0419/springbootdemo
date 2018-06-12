using System;
using System.Collections.Generic;

namespace Tuhu.C.Job.Activity.Models
{
    internal class ExpiredGroupBuyingInfo
    {
        /// <summary>
        /// 拼团id
        /// </summary>
        public string ProductGroupId { get; set; }

        /// <summary>
        /// 商品PID
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string PID { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 商品售价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 商品活动价
        /// </summary>
        public decimal FinalPrice { get; set; }

        /// <summary>
        /// 团长特价
        /// </summary>
        public decimal SpecialPrice { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 触发条件类型
        /// </summary>
        public string TriggerType { get; set; }

        /// <summary>
        /// 是否上架
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 商品总库存
        /// </summary>
        public int TotalStockCount { get; set; }

        /// <summary>
        /// 商品已售出库存
        /// </summary>
        public int CurrentSoldCount { get; set; }
    }

    internal class ExpiredGroupBuyingInfoEqualityComparer : IEqualityComparer<ExpiredGroupBuyingInfo>
    {
        public bool Equals(ExpiredGroupBuyingInfo x, ExpiredGroupBuyingInfo y)
        {
            if (x == null || y == null)
            {
                return x == y;
            }

            return x.ProductGroupId.Equals(y.ProductGroupId) && x.PID.Equals(y.PID);
        }

        public int GetHashCode(ExpiredGroupBuyingInfo obj) => $"{obj.ProductGroupId}/{obj.PID}".GetHashCode();
    }
}
