using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.WebSite.Web.Activity.Models.Enum
{
    /// <summary>
    /// 审核状态
    /// </summary>
    public enum AuditStatus
    {
        None = -1,
        /// <summary>
        /// 通过
        /// </summary>
        Passed = 1,
        /// <summary>
        /// 未通过
        /// </summary>
        NotPassed = 2,
        /// <summary>
        /// 待审核
        /// </summary>
        Pending = 3,
    }
}