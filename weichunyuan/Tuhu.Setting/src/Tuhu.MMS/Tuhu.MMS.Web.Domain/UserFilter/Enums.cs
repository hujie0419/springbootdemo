using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.MMS.Web.Domain.UserFilter
{
    public enum JobStatus
    {
        Created,
        Submit,
        WaittingForRun,
        Running,
        Success,
        Failed
    }

    public enum PreviewStatus
    {
        Created,
        Submit,
        WaittingForRun,
        Running,
        Success,
        Failed
    }

    public enum JoinType
    {
        /// <summary>
        /// 且
        /// </summary>
        And,
        /// <summary>
        /// 或
        /// </summary>
        Or,
        /// <summary>
        /// 排除
        /// </summary>
        Except
    }

    public enum CompareType
    {
        /// <summary>
        /// 大于等于
        /// </summary>
        GreaterOrEqual,
        /// <summary>
        /// 小于等于
        /// </summary>
        LessOrEqual,
        /// <summary>
        /// 大于
        /// </summary>
        Greater,
        /// <summary>
        /// 小于
        /// </summary>
        Less,
        /// <summary>
        ///   距今天天数(不含今天)
        /// </summary>
        DateFromToday,
        /// <summary>
        /// 等于
        /// </summary>
        Equal

    }
}

