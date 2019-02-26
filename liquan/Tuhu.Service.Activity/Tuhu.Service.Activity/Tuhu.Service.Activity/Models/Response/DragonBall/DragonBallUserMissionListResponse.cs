using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///    七龙珠 -   用户任务列表 返回类
    /// </summary>
    public class DragonBallUserMissionListResponse
    {
        /// <summary>
        ///     任务列表
        /// </summary>
        public List<DragonBallUserMissionListResponseItem> Items { get; set; }
    }

    public class DragonBallUserMissionListResponseItem
    {
        /// <summary>
        ///     任务ID
        /// </summary>
        public int MissionId { get; set; }

        /// <summary>
        ///     任务用户状态 0 去做任务  1 可领取  2 已领取 3 今日已分享  
        /// </summary>
        public int MissionUserStatus { get; set; }

        /// <summary>
        ///     龙珠数量
        /// </summary>
        public int DragonBallCount { get; set; }
    }
}
