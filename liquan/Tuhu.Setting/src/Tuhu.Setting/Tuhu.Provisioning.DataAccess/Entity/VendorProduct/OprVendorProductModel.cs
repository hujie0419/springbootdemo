using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// VendorProduct操作日志
    /// </summary>
    public class OprVendorProductModel
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        public string LogType { get; set; }
        /// <summary>
        /// 唯一键
        /// </summary>
        public string IdentityId { get; set; }

        /// <summary>
        /// 操作前数据
        /// </summary>
        public string OldValue { get; set; }

        /// <summary>
        /// 操作后数据
        /// </summary>
        public string NewValue { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperateUser { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }
    }
}
