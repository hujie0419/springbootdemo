using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.VipBaoYangPackage
{
    public class VipBaoYangPackageModel
    {
        public int PKID { get; set; }

        public string PID { get; set; }

        public string PackageName { get; set; }

        public Guid VipUserId { get; set; }

        public string VipUserName { get; set; }

        public string Brands { get; set; }

        public decimal Price { get; set; }

        public double? Volume { get; set; }

        public string CreateUser { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        public int Total { get; set; }

        public string SettlementMethod { get; set; }

        public List<VipBaoYangPackageOilConfig> OilConfigs { get; set; }

        public string Source { get; set; }
    }

    /// <summary>
    /// 单次保养套餐DB Model
    /// </summary>
    public class VipBaoYangPackageDbModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// PID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 套餐名称
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        /// 大客户UserId
        /// </summary>
        public Guid VipUserId { get; set; }

        /// <summary>
        /// 品牌(JSON字符串,或者逗号分隔)
        /// </summary>
        [JsonIgnore]
        public string Brands { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 机油升数
        /// </summary>
        public double? Volume { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }

        /// <summary>
        /// 结算方式
        /// </summary>
        public string SettlementMethod { get; set; }

        /// <summary>
        /// 结算用户
        /// </summary>
        public Guid SettlementVipUserId { get; set; }

        /// <summary>
        /// 规则GUID
        /// </summary>
        public Guid GetRuleGUID { get; set; }

        /// <summary>
        /// 渠道 运营（YunYing）接口（Interface）
        /// </summary>
        public string Source { get; set; }

        public VipBaoYangPackageDbModel()
        {
            this.CreateDateTime = DateTime.Now;
        }

        public VipBaoYangPackageDbModel(VipBaoYangPackageViewModel package) : this()
        {
            if (package != null)
            {
                this.PKID = package.PKID;
                this.PID = package.PID;
                this.PackageName = package.PackageName;
                this.CreateUser = package.CreateUser;
                this.Price = package.Price;
                this.SettlementMethod = package.SettlementMethod;
                this.SettlementVipUserId = package.SettlementVipUserId;
                this.GetRuleGUID = package.GetRuleGUID;
                this.Source = package.Source;
                this.VipUserId = package.VipUserId;
                this.Volume = package.Volume;
                this.OilConfigs = package.OilConfigs?.Select((config, index) => new VipBaoYangPackageOilConfig
                {
                    Brand = config.Brand,
                    Grade = config.Grade ?? string.Empty,
                    Series = config.Series ?? string.Empty,
                    PackageId = this.PKID,
                }).ToList() ?? new List<VipBaoYangPackageOilConfig>();
            }
        }

        public List<VipBaoYangPackageOilConfig> OilConfigs { get; set; }
    }

    public class BaoYangPackagePromotionRecord
    {
        public int PKID { get; set; }

        public int PackageId { get; set; }

        public string BatchCode { get; set; }

        public Guid RulesGUID { get; set; }

        public string CreateUser { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
    }

    public class BaoYangPackagePromotionDetail
    {
        public long PKID { get; set; }

        public string BatchCode { get; set; }

        public string MobileNumber { get; set; }

        public string Carno { get; set; }

        public Status Status { get; set; }

        public string PromotionId { get; set; }

        public string Remarks { get; set; }

        public int Quantity { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public int Total { get; set; }

    }

    [Serializable]
    public class ErrorUploadInfo
    {
        public string MobileNumber { get; set; }

        public string Carno { get; set; }

        public string ErrorMsg { get; set; }
    }

    public class VipSimpleUser
    {
        public string VipUserId { get; set; }

        public string VipUserName { get; set; }
    }

    public class PromotionOperationRecord
    {
        public int PackageId { get; set; }
        public string BatchCode { get; set; }

        public string VipUserName { get; set; }

        public string Volume { get; set; }

        public string PID { get; set; }

        public string PackageName { get; set; }

        public string CreateUser { get; set; }

        public DateTime CreateDateTime { get; set; }

        public int WaitCount { get; set; }

        public int SuccessCount { get; set; }

        public bool IsSendSms { get; set; }

        public int Total { get; set; }
    }

    public class VipBaoYangPackageSmsConfig
    {
        public int PKID { get; set; }

        public string VipUserId { get; set; }

        public string UserName { get; set; }

        public bool IsSendSms { get; set; }

        public string CreateUser { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public int Total { get; set; }
    }
    public enum Status
    {
        NONE,
        WAIT,
        Running,
        SUCCESS,
        FAIL,
        /// <summary>
        /// 已作废
        /// </summary>
        Invalid,
        /// <summary>
        /// 已使用
        /// </summary>
        Used
    }

    public enum SettlementMethod
    {
        ByPeriod, //据实
        PreSettled //买断
    }

    public class BaoYangPromotionSimpleInfo
    {
        public int? Quantity { get; set; }

        public int GetQuantity { get; set; }

        public int SingleQuantity { get; set; }
    }

    public class BaoYangPackageConfigSimpleInfo
    {
        public int PackageId { get; set; }

        public string BatchCode { get; set; }

        public string VipUserName { get; set; }

        public string PID { get; set; }

        public string PackageName { get; set; }

        public float? Volume { get; set; }

        public string CreateUser { get; set; }

        public string SettlementMethod { get; set; }

        public decimal Price { get; set; }

        public bool IsSendSms { get; set; }

        public string Brands { get; set; }

        public Guid? RulesGUID { get; set; }
    }

    public class BaoYangPackagePromotionDetailSimpleModel
    {
        public long PKID { get; set; }

        public string BatchCode { get; set; }

        public string MobileNumber { get; set; }

        public string Carno { get; set; }

        public string Status { get; set; }

        public int PromotionStatus { get; set; }

        public DateTime? PromotionUsedTime { get; set; }

        public long? PromotionId { get; set; }

        public string Remarks { get; set; }

        public int Total { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class VipBaoYangGiftPackConfig
    {
        public long PKID { get; set; }

        public string PackName { get; set; }

        /// <summary>
        /// 套餐ID
        /// </summary>
        public int PackageId { get; set; }

        /// <summary>
        /// 套餐名称
        /// </summary>
        public string PackageName { get; set; }

        public bool IsValid { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        public long PackId
        {
            get
            {
                return PKID;
            }
            set
            {
                PKID = value;
            }
        }
    }

    public class GiftPackCouponConfig
    {
        public long PKID { get; set; }

        public long PackId { get; set; }

        public int GetRuleID { get; set; }

        public int Quantity { get; set; }

        public string PromtionName { get; set; }

        public string Description { get; set; }

        public int RuleID { get; set; }

        public Guid GetRuleGUID { get; set; }

        public string Name { get; set; }

        public DateTime? ValiStartDate { get; set; }

        public DateTime? ValiEndDate { get; set; }

        public int? Term { get; set; }
    }

    public class VipBaoYangPagerModel<T>
    {
        public int Total { get; set; }

        public List<T> Data { get; set; }
    }

    public class VipBaoYangRedemptionCode
    {
        public long PKID { get; set; }

        public string BatchCode { get; set; }

        public string RedemptionCode { get; set; }

        public string CreateUser { get; set; }

        public string Status { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public long PackId { get; set; }
    }

    public class VipBaoYangRedemptionCodeSimpleModel
    {
        public string BatchCode { get; set; }

        public string CreateUser { get; set; }

        public DateTime CreateDateTime { get; set; }

        public List<BaoYangOprLog> Records { get; set; }
    }

}
