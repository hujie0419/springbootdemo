using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     公众号领红包 - 更新每日统计  请求类
    /// </summary>
    public class OARedEnvelopeStatisticsUpdateRequest
    {
        /// <summary>
        ///     更新日期
        /// </summary>
        public DateTime StatisticsDate { get; set; }

        /// <summary>
        ///     公众号类型 1主号
        /// </summary>
        public int OfficialAccountType { get; set; }
    }
}
