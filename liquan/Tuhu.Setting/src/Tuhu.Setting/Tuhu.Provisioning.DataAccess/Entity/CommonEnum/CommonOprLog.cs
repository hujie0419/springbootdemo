using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.CommonEnum
{
    /// <summary>
    /// 通用日志查询
    /// </summary>
    public class CommonOprLog
    {
        public string PKid { get; set; }
        public string LogType { get; set; }
        public string LogId { get; set; }
        /// <summary>
        /// 修改前
        /// </summary>
        public string BeforeValue { get; set; }
        /// <summary>
        /// 修改后
        /// </summary>
        public string AfterValue { get; set; }
        public string OperateUser { get; set; }
        public string Operation { get; set; }
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
