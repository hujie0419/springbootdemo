using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.ThirdPart.Model.Enum
{
    public enum GFTaskStatus
    {
        /// <summary>
        /// 任务创建
        /// </summary>
        Created = 0,
        /// <summary>
        /// 发券或发码成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 发券或发码失败
        /// </summary>
        Failed = 2,
        /// <summary>
        /// 任务完成
        /// </summary>
        Complated = 3,
        /// <summary>
        /// 发短信失败（第一次）
        /// </summary>
        SmsFailed = 4,
        /// <summary>
        /// 重试发短信失败
        /// </summary>
        RetrySmsFailed = 5
    }
}
