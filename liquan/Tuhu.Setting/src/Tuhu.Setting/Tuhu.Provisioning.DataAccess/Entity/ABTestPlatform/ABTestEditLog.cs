using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.ABTestPlatform
{
    public class ABTestEditLog
    {
        public Guid TestGuid { get; set; }
        public string TestName { get; set; }
        /// <summary>
        /// 修改变更
        /// </summary>
        public string Change { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateDataTime { get; set; }
    }
}
