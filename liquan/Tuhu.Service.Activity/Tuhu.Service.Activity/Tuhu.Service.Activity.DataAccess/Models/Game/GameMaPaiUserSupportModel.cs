using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Game
{
    /// <summary>
    ///     小游戏 - 马牌用户助力
    /// </summary>
    public class GameMaPaiUserSupportModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     外键：关联 - Activity.tbl_Activity
        /// </summary>
        public int ActivityId { get; set; }

        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     助力人的OPENID - 途虎主公众号的
        /// </summary>
        public string SupportOpenId { get; set; }

        /// <summary>
        ///     助力人的昵称
        /// </summary>
        public string SupportNickName { get; set; }

        /// <summary>
        ///     助力人的头像
        /// </summary>
        public string SupportHeadImgURL { get; set; }

        /// <summary>
        ///     助力的距离
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        ///     创建日期
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后一次修改日期
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
    }
}
