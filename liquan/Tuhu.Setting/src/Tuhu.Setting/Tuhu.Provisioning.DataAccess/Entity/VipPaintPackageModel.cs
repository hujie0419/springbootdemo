using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class VipPaintPackageModel
    {

        /// <summary>
        /// 喷漆大客户套餐配置
        /// </summary>
        public class VipPaintPackageConfigModel
        {
            /// <summary>
            /// 自增主键
            /// </summary>
            public int PKID { get; set; }
            /// <summary>
            /// 套餐Pid
            /// </summary>
            public string PackagePid { get; set; }
            /// <summary>
            /// 套餐名称
            /// </summary>
            public string PackageName { get; set; }
            /// <summary>
            /// 套餐价格
            /// </summary>
            public decimal PackagePrice { get; set; }
            /// <summary>
            /// 套餐所属大客户UserId
            /// </summary>
            public Guid VipUserId { get; set; }
            /// <summary>
            /// 套餐所属大客户名称
            /// </summary>
            [JsonIgnore]
            public string VipUserName { get; set; }
            /// <summary>
            /// 适用Pid
            /// </summary>
            public string ServicePids { get; set; }
            /// <summary>
            /// 结算方式
            /// </summary>
            public string SettlementMethod { get; set; }
            /// <summary>
            /// 操作人
            /// </summary>
            public string Operator { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime? CreateDateTime { get; set; }
            /// <summary>
            /// 最后更新时间
            /// </summary>
            public DateTime? LastUpdateDateTime { get; set; }
        }

        /// <summary>
        /// 塞券记录Db 
        /// </summary>
        public class VipPaintPackagePromotionRecordModel
        {
            /// <summary>
            /// 自增主键
            /// </summary>
            public int PKID { get; set; }
            /// <summary>
            /// 套餐Id
            /// </summary>
            public int PackageId { get; set; }
            /// <summary>
            /// 批次号
            /// </summary>
            public string BatchCode { get; set; }
            /// <summary>
            /// 优惠券规则
            /// </summary>
            public Guid RuleGUID { get; set; }
            /// <summary>
            /// ToB订单
            /// </summary>
            public string ToBOrder { get; set; }
            /// <summary>
            /// 是否发送短信
            /// </summary>
            public bool IsSendSms { get; set; }
            /// <summary>
            /// 创建人
            /// </summary>
            public string CreateUser { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime? CreateDateTime { get; set; }
            /// <summary>
            /// 最后更新时间
            /// </summary>
            public DateTime? LastUpdateDateTime { get; set; }
        }

        /// <summary>
        /// 塞券记录展示
        /// </summary>
        public class VipPaintPromotionRecordViewModel : VipPaintPackagePromotionRecordModel
        {
            /// <summary>
            /// 所属大客户UserId
            /// </summary>
            public Guid VipUserId { get; set; }
            /// <summary>
            /// 所属大客户名称
            /// </summary>
            public string VipUserName { get; set; }
            /// <summary>
            /// 套餐名称
            /// </summary>
            public string PackageName { get; set; }
            /// <summary>
            /// 套餐Pid
            /// </summary>
            public string PackagePid { get; set; }
            /// <summary>
            /// 上传数量
            /// </summary>
            public int UploadCount { get; set; }

            /// <summary>
            /// 塞券成功数量
            /// </summary>
            public int SuccessCount { get; set; }
        }

        /// <summary>
        /// 大客户套餐Simple
        /// </summary>
        public class VipPaintPackageSimpleModel
        {
            /// <summary>
            /// 套餐Id
            /// </summary>
            public int PackageId { get; set; }
            /// <summary>
            /// 套餐Pid
            /// </summary>
            public string PackagePid { get; set; }
            /// <summary>
            /// 套餐名称
            /// </summary>
            public string PackageName { get; set; }
            /// <summary>
            /// 套餐价格
            /// </summary>
            public decimal PackagePrice { get; set; }
            /// <summary>
            /// 结算方式
            /// </summary>
            public string SettlementMethod { get; set; }
        }

        /// <summary>
        /// 上传的塞券模版
        /// </summary>
        public class VipPaintPromotionTemplateModel
        {
            /// <summary>
            /// 手机号
            /// </summary>
            public string MobileNumber { get; set; }
            /// <summary>
            /// 车牌号
            /// </summary>
            public string CarNo { get; set; }
            /// <summary>
            /// 塞券数量
            /// </summary>
            public int PromotionCount { get; set; }
            /// <summary>
            /// 大客户名称
            /// </summary>
            public string VipUserName { get; set; }
            /// <summary>
            /// 套餐Pid
            /// </summary>
            public string PackagePid { get; set; }
            /// <summary>
            /// 套餐名称
            /// </summary>
            public string PackageName { get; set; }
            /// <summary>
            /// 优惠券Id
            /// </summary>
            public int PromotionId { get; set; }
            /// <summary>
            /// 优惠券名称
            /// </summary>
            public string PromotionName { get; set; }
            /// <summary>
            /// 优惠券描述
            /// </summary>
            public string PromotionDescription { get; set; }
            /// <summary>
            /// 券开始时间
            /// </summary>
            public DateTime? StartDateTime { get; set; }
            /// <summary>
            /// 券结束时间
            /// </summary>
            public DateTime? EndDateTime { get; set; }
        }

        /// <summary>
        /// 塞券详情Db Model
        /// </summary>
        public class VipPaintPackagePromotionDetail
        {
            /// <summary>
            /// 自增主键
            /// </summary>
            public long PKID { get; set; }
            /// <summary>
            /// 批次号
            /// </summary>
            public string BatchCode { get; set; }
            /// <summary>
            /// 手机号
            /// </summary>
            public string MobileNumber { get; set; }
            /// <summary>
            /// 车牌号
            /// </summary>
            public string CarNo { get; set; }
            /// <summary>
            /// 优惠券Id
            /// </summary>
            public int PromotionId { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            public string Status { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            public string Remarks { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime? CreateDateTime { get; set; }
            /// <summary>
            /// 最后更新时间
            /// </summary>
            public DateTime? LastUpdateDateTime { get; set; }

            /// <summary>
            /// 券开始时间
            /// </summary>
            public DateTime? StartDateTime { get; set; }
            /// <summary>
            /// 券结束时间
            /// </summary>
            public DateTime? EndDateTime { get; set; }
        }

        /// <summary>
        /// 塞券详情展示套餐相关信息
        /// </summary>
        public class VipPaintPackageConfigForDetail : VipPaintPackageConfigModel
        {
            /// <summary>
            /// 批次号
            /// </summary>
            public string BatchCode { get; set; }
            /// <summary>
            /// 优惠券规则Guid
            /// </summary>
            public Guid RuleGUID { get; set; }
            /// <summary>
            /// 创建人
            /// </summary>
            public string CreateUser { get; set; }
            /// <summary>
            /// 该批次是否发送短信
            /// </summary>
            public bool IsSendSms { get; set; }
            /// <summary>
            /// 优惠券规则Id
            /// </summary>
            public int RuleId { get; set; }
            /// <summary>
            /// 优惠券规则名称
            /// </summary>
            public string PromotionName { get; set; }
            /// <summary>
            /// 优惠券规则描述
            /// </summary>
            public string Description { get; set; }
        }

        /// <summary>
        /// 塞券详情展示
        /// </summary>
        public class VipPaintPromotionDetailViewModel : VipPaintPackagePromotionDetail
        {
            /// <summary>
            /// 2B订单
            /// </summary>
            public string ToBOrder { get; set; }
            /// <summary>
            /// 2C订单
            /// </summary>
            public string ToCOrder { get; set; }
        }

        /// <summary>
        /// 喷漆大客户优惠券配置
        /// </summary>
        public class VipPaintPackageSimpleConfig
        {
            /// <summary>
            /// 优惠券规则Id
            /// </summary>
            public int ParentCouponId { get; set; }
        }

        /// <summary>
        /// 短信配置
        /// </summary>
        public class VipPaintPackageSmsConfig
        {
            /// <summary>
            /// 自增主键
            /// </summary>
            public int PKID { get; set; }
            /// <summary>
            /// 套餐Id
            /// </summary>
            public int PackageId { get; set; }
            /// <summary>
            /// 套餐Pid
            /// </summary>
            [JsonIgnore]
            public string PackagePid { get; set; }
            /// <summary>
            /// 套餐名称
            /// </summary>
            [JsonIgnore]
            public string PackageName { get; set; }
            /// <summary>
            /// 所属大客户UserId
            /// </summary>
            [JsonIgnore]
            public Guid VipUserId { get; set; }
            /// <summary>
            /// 所属大客户名称
            /// </summary>
            [JsonIgnore]
            public string VipUserName { get; set; }
            /// <summary>
            /// 是否发送短信
            /// </summary>
            public bool IsSendSms { get; set; }
            /// <summary>
            /// 操作人
            /// </summary>
            public string Operator { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime? CreateDateTime { get; set; }
            /// <summary>
            /// 最近更新时间
            /// </summary>
            public DateTime? LastUpdateDateTime { get; set; }
        }
    }
}
