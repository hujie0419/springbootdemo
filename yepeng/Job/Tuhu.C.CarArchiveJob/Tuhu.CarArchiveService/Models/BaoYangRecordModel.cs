using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.CarArchiveService.Models
{
    public class BaoYangRecordModel
    {
        public int PKID { get; set; }

        public int OrderId { get; set; }

        public DateTime InstallDatetime { get; set; }

        public int InstallShopId { get; set; }

        public string InstallShopName { get; set; }

        public string ShopCode { get; set; }

        public string ShopRegionCode { get; set; }

        public string VinCode { get; set; }

        public string PlateNumber { get; set; }

        public int Distance { get; set; }

        public List<PartItem> PartList { get; set; }

        public List<ProjectItem> ProjectList { get; set; }
    }

    public class PartItem
    {
        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public string PartCode { get; set; }

        public int Num { get; set; }
    }

    public class ProjectItem
    {
        public string ServiceId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }
    }

    public class ShopIdentityModel
    {
        public string ShopAccount { get; set; }
        public string PassWord { get; set; }
    }

    public enum PushToEnum
    {
        ChengDu

    }

    public class ChengDuCarArchiveRegisterModel
    {
        /// <summary>
        /// 维修企业名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 登陆密码
        /// </summary>
        public string CompanyPassword { get; set; }
        /// <summary>
        /// 道路运输经营许可证号
        /// </summary>
        public string CompanyRoadTransportationLicense { get; set; }
        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string CompanyUnifiedSocialCreditidentifier { get; set; }
        /// <summary>
        /// 维修企业地址
        /// </summary>
        public string CompanyAddress { get; set; }
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string CompanyPostalcode { get; set; }
        /// <summary>
        /// 经济类型
        /// </summary>
        public string CompanyEconomicCategory { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        public string CompanyCategory { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string CompanyLinkmanname { get; set; }
        /// <summary>
        /// 联系人方式
        /// </summary>
        public string CompanyLinkmantel { get; set; }
        /// <summary>
        /// 负责人姓名
        /// </summary>
        public string CompanySuperintendentname { get; set; }
        /// <summary>
        /// 负责人联系电话
        /// </summary>
        public string CompanySuperintendenttel { get; set; }
        /// <summary>
        /// 经营范围
        /// </summary>
        public string CompanyBusinessscope { get; set; }
        /// <summary>
        /// 道路运输经营许证号开始日期
        /// </summary>
        public string RoadTransportationLicenseStartdate { get; set; }
        /// <summary>
        /// 道路运输经营许证号结束日期
        /// </summary>
        public string RoadTransportationLicenseEnddate { get; set; }
        /// <summary>
        /// 经营状态
        /// </summary>
        public string CompanyOperationState { get; set; }
        /// <summary>
        /// 区域编码
        /// </summary>
        public string CompanyAdministrativedivisioncode { get; set; }
        /// <summary>
        /// 注册邮箱
        /// </summary>
        public string CompanyEmail { get; set; }

    }
}
