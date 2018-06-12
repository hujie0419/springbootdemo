using System;
using System.Collections.Generic;

namespace Tuhu.Provisioning.Models.GroupBuyingV2
{
    public class VirtualProductViewModel
    {
        /// <summary>
        /// PID
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string PID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        
        /// <summary>
        /// 商品价格
        /// </summary>
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// 是否上架
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 优惠券数量
        /// </summary>
        public int CouponCount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }


        public IReadOnlyList<VirtualProductCouponViewModel> Coupons { get; set; }

        public string CreateTimeStr {
            get { return CreateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }
        public string UpdateTimeStr
        {
            get { return UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }
        public string IsActiveStr
        {
            get { return IsActive?"上架":"下架"; }
        }
    }

    public class VirtualProductCouponViewModel
    {
        /// <summary>
        /// 优惠券id
        /// </summary>
        public Guid CouponId { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string CouponName { get; set; }

        /// <summary>
        /// 优惠券描述
        /// </summary>
        public string CouponDescription { get; set; }

        /// <summary>
        /// 优惠券面额
        /// </summary>
        public decimal CouponPrice { get; set; }

        /// <summary>
        /// 优惠券使用条件
        /// </summary>
        public decimal CouponLeastPrice { get; set; }

        /// <summary>
        /// 优惠券有效期
        /// </summary>
        public string AvailablePeriod { get; set; }

        /// <summary>
        /// 虚拟成本比例百分比 %
        /// </summary>
        public int CouponRate { get; set; }

        /// <summary>
        /// PID
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string PID { get; set; }

        /// <summary>
        /// pkid
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int PKID { get; set; }
    }
}