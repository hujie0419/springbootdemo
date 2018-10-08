using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class EnterpriseUserBeautyConfig
    {
        public int PKID { get; set; }

        public Guid UserId { get; set; }

        public int PackageDetailsId { get; set; }

        public string Remark { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime UpdateDateTime { get; set; }
    }

    public class CooperateUserService
    {
        /// <summary>
        /// 服务配置Id
        /// </summary>
        public int PackageDetailId { get; set; }
        /// <summary>
        /// 合作商Id
        /// </summary>
        public int CooperateId { get; set; }
        /// <summary>
        /// 合作商名称
        /// </summary>
        public string CooperateName { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        public string SettlementMethod { get; set; }
        /// <summary>
        /// 服务Id
        /// </summary>
        public string ServiceId { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// userId
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 客户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 所属公司名
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 所属公司Id
        /// </summary>
        public string CompanyId { get; set; }

    }

    public class EnterpriseUserModuleConifg
    {
        public int PKID { get; set; }

        public Guid UserId { get; set; }

        public string ModuleType { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime UpdateDateTime { get; set; }
    }
}
