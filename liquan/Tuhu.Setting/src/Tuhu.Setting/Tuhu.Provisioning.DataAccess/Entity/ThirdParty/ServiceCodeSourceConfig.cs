using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.ThirdParty
{
    public class ServiceCodeSourceConfig
    {
        /// <summary>
        /// PKID
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 来源名称
        /// </summary>
        public string SourceName { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 来源正则
        /// </summary>
        public string SourceRegex { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedTime { get; set; }
        /// <summary>
        /// total
        /// </summary>
        public int Total { get; set; }
    }
}
