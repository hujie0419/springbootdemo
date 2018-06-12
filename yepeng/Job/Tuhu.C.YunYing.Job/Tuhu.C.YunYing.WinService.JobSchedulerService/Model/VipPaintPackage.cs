using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.Model
{
    public class VipPaintPackagePromotionRecord
    {
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchCode { get; set; }

        /// <summary>
        /// 优惠券Guid
        /// </summary>
        public Guid RuleGUID { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 是否发送短信
        /// </summary>
        public bool IsSendSms { get; set; }

        /// <summary>
        /// 结算方式
        /// </summary>
        public string SettlementMethod { get; set; }
    }

    /// <summary>
    /// 大客户喷漆塞券详情
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
        public long PromotionId { get; set; }
    }

    /// <summary>
    /// 大客户喷漆套餐配置
    /// </summary>
    public class VipPaintPackageConfig
    {
        /// <summary>
        /// 大客户UserId
        /// </summary>
        public Guid VipUserId { get; set; }

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

        /// <summary>
        /// 套餐Id
        /// </summary>
        public int PKID { get; set; }
    }
}
