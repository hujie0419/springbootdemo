using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{
    public enum ArticleStatus
    {
        /// <summary>
        /// 所有状态
        /// </summary>
        All,
        /// <summary>
        /// 已保存
        /// </summary>
        Saved,

        /// <summary>
        /// 预发表
        /// </summary>
        PrePublish,

        /// <summary>
        /// 已发布
        /// </summary>
        Published,

        /// <summary>
        /// 已撤回
        /// </summary>
        Withdrew
    }


    public enum ShareImgStatus
    {

        /// <summary>
        /// 所有状态
        /// </summary>
        All,
        /// <summary>
        /// 待审核
        /// </summary>
        WaitAudit,

        /// <summary>
        /// 审核通过
        /// </summary>
        AuditPass,

        /// <summary>
        /// 审核不通过
        /// </summary>
        AuditNotPass,
    }
}
