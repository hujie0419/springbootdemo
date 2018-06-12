using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 优惠券任务类型
    /// </summary>
    public enum PromotionTaskType
    {
        [Display(Name = "单次任务")]
        /// <summary>
        /// 单次任务
        /// </summary>
        Once = 1,
        [Display(Name = "重复任务")]
        /// <summary>
        /// 重复任务
        /// </summary>
        Repeat = 2
    }
}
