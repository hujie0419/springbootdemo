using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class PromotionConsts
    {
        /// <summary>
        /// 优惠券任务状态
        /// </summary>
        public enum PromotionTaskStatusEnum
        {
            /// <summary>
            /// 待审核
            /// </summary>
            PendingAudit = 0,
            /// <summary>
            /// 已生效
            /// </summary>
            Executed = 1,
            /// <summary>
            /// 已关闭
            /// </summary>
            Closed = 2
        }

        public enum SelectUserTypeEnum
        {
            /// <summary>
            /// 上传文件
            /// </summary>
            UploadFile = 1,
            /// <summary>
            /// 条件选择
            /// </summary>
            ConditionSelect = 2,
            /// <summary>
            /// 从bi表里选择
            /// </summary>
            BiActivity=3
        }

        public enum TaskTypeEnum
        {
            /// <summary>
            /// 单词任务
            /// </summary>
            Single = 1,
            /// <summary>
            /// 重复任务
            /// </summary>
            Repeat = 2
        }

        //public enum TaskStatusEnum
        //{
        //    /// <summary>
        //    /// 待审核
        //    /// </summary>
        //    WaitAudit = 0,
        //    /// <summary>
        //    /// 已生效
        //    /// </summary>
        //    Efficient = 1,
        //    /// <summary>
        //    /// 已关闭
        //    /// </summary>
        //    Closed = 2
        //}

    }
}
