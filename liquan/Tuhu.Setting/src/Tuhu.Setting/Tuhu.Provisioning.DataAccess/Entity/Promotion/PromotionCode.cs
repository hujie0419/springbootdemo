using System;

namespace Tuhu.Provisioning.DataAccess
{
    public class PromotionCode
	{
		/// <summary>
		/// 优惠券唯一标识符
		/// </summary>
		public int PKID { get; set; }
		/// <summary>
		/// 优惠券编号
		/// </summary>
		public string Code { get; set; }
        public string PromotionName { get; set; }
		/// <summary>
		/// 优惠券描述
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// 折扣
		/// </summary>
		public int Discount { get; set; }
		/// <summary>
		/// 最低消费
		/// </summary>
		public int MinMoney { get; set; }
		/// <summary>
		/// 优惠券状态
		/// </summary>
		public int Status { get; set; }
		/// <summary>
		/// 优惠券类型
		/// </summary>
		public int Type { get; set; }
		/// <summary>
		/// 用户ID
		/// </summary>
		public Guid? UserID { get; set;}
		/// <summary>
		/// 优惠券所对应策略的编号
		/// </summary>
		public int RuleID { get; set; }
		/// <summary>
		/// 订单号
		/// </summary>
		public int OrderID { get; set; }
		/// <summary>
		/// 优惠券开始使用时间
		/// </summary>
		public DateTime StartTime { get; set; }
		/// <summary>
		/// 优惠券使用的截止时间
		/// </summary>
		public DateTime EndTime { get; set; }
		/// <summary>
		/// 优惠券使用的创建时间
		/// </summary>
		public DateTime CreateTime { get; set; }

		/// <summary>
		/// 优惠券来源渠道
		/// </summary>
		public string CodeChannel { get; set; }
        public int BatchId { get; set; }
        public string Issuer { get; set; }
        public string Creater { get; set; }
        public string IssueChannle { get; set; }
        public string IssueChannleId { get; set; }
        public string DepartmentName { get; set; }
        public string IntentionName { get; set; }


    }
}