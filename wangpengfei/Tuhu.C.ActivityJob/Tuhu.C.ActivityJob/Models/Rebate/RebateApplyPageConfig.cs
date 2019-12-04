using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Models.Rebate
{
    public class RebateApplyPageConfig
    {
        /// <summary>
        /// 活动id
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// 活动类型
        /// </summary>
        public string ActivityType { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
    }
}
