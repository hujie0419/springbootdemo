using System;

namespace Tuhu.Service.Activity.DataAccess.Models.DragonBall
{
    /// <summary>
    ///     七龙珠 - 用户领取的奖励   
    /// </summary>
    public class DragonBallUserLootModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     领取的奖励名称
        /// </summary>
        public string LootName { get; set; }

        /// <summary>
        ///     领取奖励 使用 开始时间
        /// </summary>
        public DateTime? LootStartTime { get; set; }

        /// <summary>
        ///     领取奖励 使用 结束时间
        /// </summary>
        public DateTime? LootEndTime { get; set; }

        /// <summary>
        ///     领取备注
        /// </summary>
        public string LootDesc { get; set; }

        /// <summary>
        ///     领取奖励的图片URL
        /// </summary>
        public string LootPicUrl { get; set; }

        /// <summary>
        ///     领取类型 - 1 是任务奖励 2 是召唤神龙奖励
        /// </summary>
        public int LootType { get; set; }

        /// <summary>
        ///     XX品牌额外为你奉上
        /// </summary>
        public string LootTitile { get; set; }

        /// <summary>
        ///     获取物品的其他属性
        /// </summary>
        public string LootMemo { get; set; }
    }
}
