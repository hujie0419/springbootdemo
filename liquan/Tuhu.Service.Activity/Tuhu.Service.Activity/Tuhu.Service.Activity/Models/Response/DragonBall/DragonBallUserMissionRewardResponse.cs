using System;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///    七龙珠 -   用户任务领取奖励 返回类
    /// </summary>
    public class DragonBallUserMissionRewardResponse
    {
        /// <summary>
        ///     获得的龙珠数量
        /// </summary>
        public int DragonBallCount { get; set; }

        /// <summary>
        ///     奖励名称
        /// </summary>
        public string LootName { get; set; }

        /// <summary>
        ///     使用开始时间
        /// </summary>
        public DateTime? LootStartTime { get; set; }

        /// <summary>
        ///     使用结束时间
        /// </summary>
        public DateTime? LootEndTime { get; set; }

        /// <summary>
        ///     获取备注
        /// </summary>
        public string LootMemo { get; set; }

        /// <summary>
        ///     使用说明
        /// </summary>
        public string LootDesc { get; set; }

        /// <summary>
        ///     XX品牌额外为你奉上
        /// </summary>
        public string LootTitile { get; set; }

        /// <summary>
        ///     图片URL
        /// </summary>
        public string LootPicUrl { get; set; }
    }
}
