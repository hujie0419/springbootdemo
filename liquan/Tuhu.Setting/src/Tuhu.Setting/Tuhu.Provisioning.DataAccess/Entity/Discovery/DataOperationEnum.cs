using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{
    /// <summary>
    /// 数据操作枚举
    /// </summary>
    public enum DataOperationEnum
    {
        /// <summary>
        /// 修改
        /// </summary>
        Update,
        /// <summary>
        /// 置顶
        /// </summary>
        TopBest,
        /// <summary>
        /// 取消置顶
        /// </summary>
        CancelTopBest,
        /// <summary>
        /// 撤回
        /// </summary>
        Withdrew
    }
}
