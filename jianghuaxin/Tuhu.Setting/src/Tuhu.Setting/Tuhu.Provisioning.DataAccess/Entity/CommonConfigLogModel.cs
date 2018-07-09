using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CommonConfigLogModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long PKID { get; set; }
        /// <summary>
        /// 业务表主键ID
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        public string ObjectType { get; set; }
        /// <summary>
        /// 之前值
        /// </summary>
        public string BeforeValue { get; set; }
        /// <summary>
        /// 之后值
        /// </summary>
        public string AfterValue { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDateTime { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>

        public string LastUpdateDateTime { get; set; }
    }
}
