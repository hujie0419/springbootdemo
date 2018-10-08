using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class BeautyServicePackage
    {
        public int PKID { get; set; }

        public int VipCompanyId { get; set; }

        public string VipCompanyName { get; set; }

        public Guid VipUserId { get; set; }

        public string VipUserName { get; set; }

        public string PackageName { get; set; }

        public string Department { get; set; }

        public string Purpose { get; set; }

        public int PackageCodeNum { get; set; }
        /// <summary>
        /// PreSettled or ByPeriod
        /// </summary>
        public string SettlementMethod { get; set; }

        public string Description { get; set; }
        /// <summary>
        ///  发礼包码(packageCode)或者直接发服务码（serviceCode）
        /// </summary>
        public string PackageType { get; set; }

        public DateTime? PackageCodeStartTime { get; set; }

        public DateTime? PackageCodeEndTime { get; set; }

        public string CreateUser { get; set; }

        public string UpdateUser { get; set; }

        public bool IsPackageCodeGenerated { get; set; }

        public bool IsActive { get; set; }

        public int BuyoutOrderId { get; set; }
        /// <summary>
        /// 合作公司ID
        /// </summary>
        public int CooperateId { get; set; }
        /// <summary>
        /// 合作公司名字
        /// </summary>
        public string CooperateName { get; set; }
    }
}
