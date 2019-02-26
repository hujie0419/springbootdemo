using System;

namespace Tuhu.Service.Activity.DataAccess.Models
{
    /// <summary>
    ///     公众号领红包每日统计表
    /// </summary>
    public class OARedEnvelopeStatisticsModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     统计日期
        /// </summary>
        public DateTime StatisticsDate { get; set; }

        /// <summary>
        ///     当天的红包上限
        /// </summary>
        public decimal DayMaxMoney { get; set; }

        /// <summary>
        ///     用户数量 参与人数
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        ///     已发放红包数
        /// </summary>
        public int RedEnvelopeCount { get; set; }

        /// <summary>
        ///     已发放红包金额（元）
        /// </summary>
        public decimal RedEnvelopeSumMoney { get; set; }

        /// <summary>
        ///     红包平均金额
        /// </summary>
        public decimal RedEnvelopeAvg { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后一次更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        ///     公众号类型 1主号
        /// </summary>
        public int OfficialAccountType { get; set; }

    }
}
