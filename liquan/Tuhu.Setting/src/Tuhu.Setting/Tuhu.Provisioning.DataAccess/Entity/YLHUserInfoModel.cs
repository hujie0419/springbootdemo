using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public class YLHUserInfoModel
    {
        #region YLHUserInfo
        public string u_user_id { get; set; }

        public string MemberNumber { get; set; }
        public string MemberName { get; set; }
        public string MemberAddress { get; set; }

        public string MemberPhone { get; set; }

        public DateTime MemberBirthday { get; set; }

        public int Integration { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime UpdatedTime{ get; set; }

        public String Tag { get; set; }
        #endregion
    }

    public class PromotionItem
    {
        public string ProductName { get; set; }
        /// <summary>
		/// 优惠券所对应策略的编号
		/// </summary>
		public int RuleID { get; set; }
        /// <summary>
        /// 优惠券名称
        /// </summary>
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
		/// 优惠券来源渠道
		/// </summary>
		public string CodeChannel { get; set; }
    }
}
