using System;

namespace Tuhu.Provisioning.DataAccess.Entity.GroupBuyingV2
{
	/// <summary>
	/// 虚拟商品优惠券配置表
	/// </summary>
	public class VirtualProductCouponConfigModel
	{
		/// <summary>
		/// 主键
		/// </summary>
		// ReSharper disable once InconsistentNaming
		public int PKID { get; set; }

		/// <summary>
		/// 商品PID
		/// </summary>
		// ReSharper disable once InconsistentNaming
		public string PID { get; set; }		

		/// <summary>
		/// 优惠券id
		/// </summary>
		public Guid CouponId { get; set; }

		/// <summary>
		/// 优惠券数量
		/// </summary>
		public int CouponCount { get; set; }

		/// <summary>
		/// 虚拟成本比例
		/// </summary>
		public int CouponRate { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateDateTime { get; set; }

		/// <summary>
		/// 创建人
		/// </summary>
		public string CreatedBy { get; set; }

		/// <summary>
		/// 最后更新时间
		/// </summary>
		public DateTime LastUpdateDateTime { get; set; }

		/// <summary>
		/// 更新人
		/// </summary>
		public string UpdatedBy { get; set; }
        
        
        /// <summary>
		/// 优惠券 自身 的 价值
		/// </summary>
		public decimal CouponValue { get; set; }
    }
}