using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.CustomersActivity
{
    /// <summary>
    /// 大客户活动专享券码表
    /// </summary>
    public class CustomerExclusiveCouponModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int PKID { get; set; }

        /// <summary>
        /// 外键关联CustomerExclusiveSetting表PKID
        /// </summary>
        ///
        public string CustomerExclusiveSettingPkId { get; set; }
        /// <summary>
        /// 活动专享ID,系统自动生成
        /// </summary>
        ///
        public string ActivityExclusiveId { get; set; }

        /// <summary>
        /// 券码
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }


        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 状态 0:正常; -1:删除;
        /// </summary>
        public string Status { get; set; }


        /// <summary>
        /// 是否删除;  0否; 1是;
        /// </summary>
        public string IsDelete { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateDatetime { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateBy { get; set; }
    }
}
