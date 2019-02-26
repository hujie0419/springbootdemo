using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class MrCooperateUserConfig
    {
        public int PKID { get; set; }
        /// <summary>
        /// 合作名
        /// </summary>
        public string CooperateName { get; set; }
        /// <summary>
        /// 关联大客户ID
        /// </summary>
        public Guid VipUserId { get; set; }
        /// <summary>
        /// 大客户名
        /// </summary>
        public string VipUserName { get; set; }
        /// <summary>
        /// 大客户公司ID
        /// </summary>
        public int VipCompanyId { get; set; }
        /// <summary>
        /// 大客户所属公司名
        /// </summary>
        public string VipCompanyName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        public string CreateUser { get; set;  }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
    }
}
