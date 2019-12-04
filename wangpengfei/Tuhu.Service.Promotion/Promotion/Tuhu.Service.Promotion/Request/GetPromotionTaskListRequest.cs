using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
    /// <summary>
    /// 获取优惠券 任务列表
    /// </summary>
    public class GetPromotionTaskListRequest
    {
        /// <summary>
        /// 任务类型 ：0 = 所有，1=单次任务，2=触发任务
        /// </summary>
        public int TaskType { get; set; }

        /// <summary>
        /// 任务状态：0-未审核，1=已审核，2=已关闭,
        /// </summary>
        public int TaskStatus { get; set; }

        /// <summary>
        /// 任务执行时间
        /// </summary>
        public DateTime? TaskTime { get; set; }




    }
}
