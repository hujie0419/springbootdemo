using System;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     活动排名返回对象
    /// </summary>
    public class ActivityCouponRankResponse
    {
        /// <summary>
        ///     冗余-排行计算时候的兑换券数量  （废弃）
        /// </summary>
        [Obsolete]
        public int CouponCount { get; set; }

        /// <summary>
        ///     冗余-用户历史总兑换券数量
        /// </summary>
        public int CouponSum { get; set; }

        /// <summary>
        ///     排行状态 
        ///     0 前端展示，距离第 3 名还有X兑换券，取To3rdCouponCount字段  
        ///     1 前端展示，当前未上榜
        ///     2 前端展示，您当前名次X名，请继续保持，取Rank字段
        /// </summary>
        public int RankStatus { get; set; }

        /// <summary>
        ///     途虎昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        ///     称号 例如：（足球小将 ）
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     排行日期
        /// </summary>
        public DateTime RankDate { get; set; }

        /// <summary>
        ///     用户的排名 
        /// </summary>
        public long Rank { get; set; }

        /// <summary>
        ///     途虎头像
        /// </summary>
        public string UserImgUrl { get; set; }

        /// <summary>
        ///     离第 3 名的差距
        /// </summary>
        public long To3rdCouponCount { get; set; }

        /// <summary>
        ///     外键：关联表Tuhu_profiles.UserObject  Userid
        /// </summary>
        public Guid UserId { get; set; }
    }
}
