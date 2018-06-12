using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class EnterpriseUserViewModel
    {
        public int PKID { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public Guid UserId { get; set; }

        public string UserMobile { get; set; }

        public List<int> PackageDetailIds { get; set; }

        //public IEnumerable<CooperateUserConfig> CooperateUserConfigDetails { get; set; }

        public IEnumerable<CooperateUserService> CooperateUserServiceDeails { get; set; }

        public string Remark { get; set; }

        public bool SearchVerifyServiceCode { get; set; }

        public bool SearchServiceCode { get; set; }

        public bool GenerateCode { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime UpdateDateTime { get; set; }

        public IEnumerable<EnterpriseUserModuleConifg> UserModuleDetails { get; set; }
    }

    public class CooperateUserConfig
    {
        /// <summary>
        /// 合作商户Id
        /// </summary>
        public int CooperateId { get; set; }
        /// <summary>
        /// 合作名
        /// </summary>
        public string CooperateName { get; set; }

        public List<CooperateUserService> CooperateUserServiceDetails { get; set; }
    }
}
