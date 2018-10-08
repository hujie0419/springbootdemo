using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{
    /// <summary>
    ///     奖品/兑换物 客户兑换明细表
    /// </summary>
    public class ActivityPrizeOrderDetailModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     外键：关联表 tbl_Activity
        /// </summary>
        public long ActivityId { get; set; }

        /// <summary>
        ///     冗余-tbl_Activity 的 ActivityName
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        ///     外键：关联表Tuhu_profiles.UserObject  Userid
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后一次修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        ///     外键：关联表tbl_ActivityPrize
        /// </summary>
        public long ActivityPrizeId { get; set; }

        /// <summary>
        ///     冗余-兑换的商品名称
        /// </summary>
        public string ActivityPrizeName { get; set; }

        /// <summary>
        ///     冗余-兑换商品的照片url
        /// </summary>
        public string ActivityPrizePicUrl { get; set; }

        /// <summary>
        ///     实际支付的兑换券数量
        /// </summary>
        public int CouponCount { get; set; }
    }
}
