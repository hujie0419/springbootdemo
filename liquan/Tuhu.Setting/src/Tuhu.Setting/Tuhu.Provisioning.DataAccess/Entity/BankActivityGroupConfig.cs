using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 银行活动组配置
    /// </summary>
    public class BankActivityGroupConfig
    {
        public int PKID { get; set; }
        /// <summary>
        /// 组Id
        /// </summary>
        public Guid GroupId { get; set; } = Guid.NewGuid();
        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 地区Id
        /// </summary>
        public int RegionId { get; set; }
        /// <summary>
        /// 地区名称
        /// </summary>
        public string RegionName { get; set; }
        /// <summary>
        /// 活动Id
        /// </summary>
        public Guid ActivityId { get; set; }
        /// <summary>
        /// 软删除
        /// </summary>
        public bool IsDeleted { get; set; } = false;
        /// <summary>
        /// 创建者
        /// </summary>
        public string CreateUser { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }
}
