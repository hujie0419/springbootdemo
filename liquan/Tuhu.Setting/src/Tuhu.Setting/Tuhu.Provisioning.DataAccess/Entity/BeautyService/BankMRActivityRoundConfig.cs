using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class BankMRActivityRoundConfig
    {
        /// <summary>
        /// 活动场次ID
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid ActivityId { get; set; }
        /// <summary>
        /// 活动场次总限量
        /// </summary>
        public int LimitCount { get; set; }
        /// <summary>
        /// 用户活动场次总限量
        /// </summary>
        public int UserLimitCount { get; set; }
        /// <summary>
        /// 活动场次开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 活动场次结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsActive { get; set; }
    }
}
