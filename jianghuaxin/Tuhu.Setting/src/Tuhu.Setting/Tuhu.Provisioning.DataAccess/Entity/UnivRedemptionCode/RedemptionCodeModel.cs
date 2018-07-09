using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.UnivRedemptionCode
{
    public class RedemptionConfig
    {
        public int PKID { get; set; }
        /// <summary>
        /// 配置GUID
        /// </summary>
        public Guid ConfigId { get; set; }

        /// <summary>
        /// 公司名
        /// </summary>
        public string CooperateName { get; set; }

        public int CooperateId { get; set; }

        /// <summary>
        /// 兑换码名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 兑换码有效期,从兑换码生成开始算起
        /// </summary>
        public int EffectiveDay { get; set; }

        /// <summary>
        /// 至少勾选几个
        /// </summary>
        public int AtLeastNum { get; set; }
        /// <summary>
        /// 最多选择几项
        /// </summary>
        public int AtMostNum { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int SumQuantity { get; set; }

        /// <summary>
        /// 结算方式
        /// </summary>
        public string SettlementMethod { get; set; }

        /// <summary>
        /// 生成方式Interface或Batch
        /// </summary>
        public string GenerateType { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 审核状态Pending,Accepted,Rejected 
        /// </summary>
        public string AuditStatus { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string Auditor { get; set; }

        /// <summary>
        /// 大客户的ID
        /// </summary>
        public Guid VipUserId { get; set; }
        /// <summary>
        /// 所属分组
        /// </summary>
        public Guid? GroupId { get; set; }

    }

    public class RedeemMrCodeConfig
    {
        public int PKID { get; set; }

        /// <summary>
        /// 兑换码配置
        /// </summary>
        public Guid RedemptionConfigId { get; set; }

        /// <summary>
        /// 服务码名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 结算方式,BatchPreSettled,SinglePreSettled,ByPeriod
        /// </summary>
        public string SettlementMethod { get; set; }

        /// <summary>
        /// 大客户结算价
        /// </summary>
        public decimal SettlementPrice { get; set; }

        /// <summary>
        /// 门店佣金比
        /// </summary>
        public decimal ShopCommission { get; set; }

        /// <summary>
        /// 服务数量
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 有效期(天)
        /// </summary>
        public int? EffectiveDay { get; set; }

        /// <summary>
        /// 对应BeautyServiceCodeTypeConfig的PKID
        /// </summary>
        public int CodeTypeConfigId { get; set; }

        /// <summary>
        /// 是否必选
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 是否生效
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        public string ServiceName { get; set; }

        public RedeemMrCodeConfig()
        {
            CreateTime = DateTime.Now;
        }

    }

    public class RedeemMrCodeLimitConfig
    {
        public int PKID { get; set; }

        public int MrCodeConfigId { get; set; }

        public string CycleType { get; set; }

        public int CycleLimit { get; set; }

        public string ProvinceIds { get; set; }

        public string CityIds { get; set; }

        public string CreateUser { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public RedeemMrCodeLimitConfig()
        {
            this.CreateTime = DateTime.Now;
        }
    }

    public class RedeemPromotionConfig
    {
        /// <summary>
        /// 主键自增ID
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 配置ID
        /// </summary>
        public Guid RedemptionConfigId { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 优惠券领取规则
        /// </summary>
        public Guid GetRuleGuid { get; set; }

        /// <summary>
        /// 优惠券描述
        /// </summary>
        public string CouponDescription { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BusinessType { get; set; }

        /// <summary>
        /// 套餐ID(普通流程为0)
        /// </summary>
        public int PackageId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 结算方式
        /// </summary>
        public string SettlementMethod { get; set; }

        /// <summary>
        /// 结算价
        /// </summary>
        public decimal SettlementPrice { get; set; }

        /// <summary>
        /// 必选
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

    }

    public class RedemptionCodeRecordResult
    {
        /// <summary>
        /// 兑换码配置Id
        /// </summary>
        public Guid RedemptionConfigId { get; set; }

        /// <summary>
        /// 兑换码配置的PKID
        /// </summary>
        public int ConfigId { get; set; }

        /// <summary>
        /// BatchCode
        /// </summary>
        public string BatchCode { get; set; }

        public string CreateUser { get; set; }

        public DateTime CreateTime { get; set; }

        public string Name { get; set; }

        public string CooperateName { get; set; }

        public int EffectiveDay { get; set; }

        public int Num { get; set; }
        /// <summary>
        /// 批次状态
        /// </summary>
        public string Status { get; set; }
    }

    public class RedemptionCodeRecord
    {
        /// <summary>
        /// 兑换码
        /// </summary>
        public string RedemptionCode { get; set; }

        /// <summary>
        /// 兑换码配置Id
        /// </summary>
        public Guid RedemptionConfigId { get; set; }

        /// <summary>
        /// BatchCode
        /// </summary>
        public string BatchCode { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// MobileNum
        /// </summary>
        public string MobileNum { get; set; }

        /// <summary>
        /// 大客户买断订单号
        /// </summary>
        public int? OrderId { get; set; }

        /// <summary>
        /// 兑换码创建人，手动生成的是相应的人，接口生成是相应的大客户
        /// </summary>
        public string CreateUser { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public int EffectiveDay { get; set; }
    }

    public class PromotionBusinessTypeConfig
    {
        /// <summary>
        /// PKID
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// --VipBaoYang,NormalBaoYang ,Paint,MeiRong
        /// </summary>
        public string BusinessType { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 优惠券领取规则
        /// </summary>
        public Guid GetRuleGuid { get; set; }

        public int RuleId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

    }

    public class RedeemGroupSetting
    {
        public int PKID { get; set; }
        public Guid? GroupId { get; set; }
        public string GroupName { get; set; }
        public DateTime CreateTime { get; set; }
        public string AppId { get; set; }
        public int BusinessType { get; set; }
        public int SendCodeType { get; set; }

        public string CreateTimeString
        {
            get
            {
                return CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }

    public class OpenAppModel
    {
        public string AppId { get; set; }

        public string AppName { get; set; }
        public int Id { get; set; }
        public string OrderChannel { get; set; }
        public DateTime CreateTime { get; set; }

        public string CreateTimeString
        {
            get
            {
                return CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public string InputCharset { get; set; }
        public string AppSecret { get; set; }
        public string SignType { get; set; }
        public string EncryptType { get; set; }
        public string PrivateKey { get; set; }
        public string BigCustomerUrl { get; set; }
        private BigCustomerUrlModel _BigCustomerUrlInstanse;
        public BigCustomerUrlModel BigCustomerUrlInstanse
        {
            get
            {
                if (!string.IsNullOrEmpty(BigCustomerUrl))
                {
                    _BigCustomerUrlInstanse = JsonConvert.DeserializeObject<BigCustomerUrlModel>(BigCustomerUrl);
                }
                return _BigCustomerUrlInstanse ?? new BigCustomerUrlModel() { GetUserUrl = "", VerifyUrl = "" };
            }
            set
            {
                _BigCustomerUrlInstanse = value;
            }
        }
        public string BigCustomerStatus { get; set; }
    }
    public class BigCustomerUrlModel
    {
        public string GetUserUrl { get; set; }
        public string VerifyUrl { get; set; }
    }
    /// <summary>
    /// 通用兑换码和美容兑换码Result
    /// </summary>
    public class UnivBeautyRedemptionCodeResult
    {
        public int PKID { get; set; }
        /// <summary>
        /// 配置Id
        /// </summary>
        public int ConfigId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 用户手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 合作用户Id
        /// </summary>
        public int CooperateId { get; set; }
        /// <summary>
        /// 合作者名称
        /// </summary>
        public string CooperateName { get; set; }
        /// <summary>
        /// 兑换码
        /// </summary>
        public string RedemptionCode { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 兑换时间
        /// </summary>
        public DateTime ExchangeTime { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUser { get; set; }
        /// <summary>
        /// 兑换码类型：BeautyServicePackageCode、RedemptionCodeRecord
        /// </summary>
        public string RedemptionCodeType { get; set; }
    }
}
