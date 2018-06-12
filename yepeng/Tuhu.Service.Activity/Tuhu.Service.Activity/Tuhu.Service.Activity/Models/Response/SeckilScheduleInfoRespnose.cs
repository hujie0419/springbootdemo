using System;

namespace Tuhu.Service.Activity.Models.Response
{
    public class SeckilScheduleInfoRespnose
    {
        /// <summary>
        /// 活动id
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// 秒杀价
        /// </summary>
        public decimal SeckillPrice { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDateTime { get; set; }
    }
}
