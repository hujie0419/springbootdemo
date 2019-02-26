using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Game
{
    /// <summary>
    ///     小游戏 - 用户积分变动明细
    /// </summary>
    public class GameUserPointDetailModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     活动id
        /// </summary>
        public long ActivityId { get; set; }

        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     变动的积分
        /// </summary>
        public int Point { get; set; }

        /// <summary>
        ///     创建日期
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后一次更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        ///     变动类型
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        ///     备注
        /// </summary>
        public string Memo { get; set;}

        /// <summary>
        /// 积分是否被消耗:1-已消耗 0未消耗
        /// </summary>
        public int IsUsed { get; set; }
        
    }
}
