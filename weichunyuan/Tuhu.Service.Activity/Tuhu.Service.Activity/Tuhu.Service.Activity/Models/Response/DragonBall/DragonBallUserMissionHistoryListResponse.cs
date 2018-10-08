using System;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///    七龙珠 -   用户任务历史 返回类
    /// </summary>
    public class DragonBallUserMissionHistoryListResponse
    {
        public List<DragonBallUserMissionHistoryListResponseItem> Items { get; set; } =
            new List<DragonBallUserMissionHistoryListResponseItem>();
    }

    public class DragonBallUserMissionHistoryListResponseItem
    {
        /// <summary>
        ///     任务ID
        /// </summary>
        public int MissionId { get; set; }

        /// <summary>
        ///     备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        ///     获得的龙珠数量
        /// </summary>
        public int DragonBallCount { get; set; }

        /// <summary>
        ///     时间
        /// </summary>
        public DateTime Date { get; set; }
    }
}
