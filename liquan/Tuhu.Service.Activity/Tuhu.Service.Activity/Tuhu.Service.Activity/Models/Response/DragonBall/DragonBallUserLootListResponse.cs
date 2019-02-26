using System;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     七龙珠 -  用户历史奖励 返回类
    /// </summary>
    public class DragonBallUserLootListResponse
    {

        /// <summary>
        ///     免单数据
        /// </summary>
        public List<DragonBallUserLootListResponseItem> FreeItems { get; set; } = new List<DragonBallUserLootListResponseItem>();

        /// <summary>
        ///     优惠券数据
        /// </summary>
        public List<DragonBallUserLootListResponseItem> CouponItems { get; set; } = new List<DragonBallUserLootListResponseItem>();
    }

    public class DragonBallUserLootListResponseItem
    {
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
        ///     使用说明
        /// </summary>
        public string LootDesc { get; set; }

        /// <summary>
        ///     图片URL
        /// </summary>
        public string LootPicUrl { get; set; }

       
    }
}
