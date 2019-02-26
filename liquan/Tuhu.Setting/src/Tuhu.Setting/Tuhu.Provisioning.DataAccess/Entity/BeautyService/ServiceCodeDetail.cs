using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class ServiceCodeDetail
    {
        /// <summary>
        /// BeautyServicePackageDetailCode的PKID
        /// </summary>
        public int PackageDetailCodeId { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchCode { get; set; }
        /// <summary>
        /// BeautyServicePackageDetail的PKID
        /// </summary>
        public int PackageDetailId { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 服务码
        /// </summary>
        public string ServiceCode { get; set; }
        /// <summary>
        /// 兑换码
        /// </summary>
        public string PackageCode { get; set; }
        /// <summary>
        /// 服务码状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 大客户公司名
        /// </summary>
        public string VipCompanyName { get; set; }
        /// <summary>
        /// 核销时间
        /// </summary>
        public DateTime? VerifyTime { get; set; }
        /// <summary>
        /// 服务码开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 服务码结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 和大客户的结算价
        /// </summary>
        public decimal VipSettleMentPrice { get; set; }
        /// <summary>
        /// 门店佣金比
        /// </summary>
        public decimal ShopCommission { get; set; }
        /// <summary>
        /// 服务ID
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 限制车型
        /// </summary>
        public string RestrictVehicle { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 订单号跳转链接
        /// </summary>
        public string OrderNoLink { get; set; }
        /// <summary>
        /// 核销门店
        /// </summary>
        public string VerifyShop { get; set; }
        /// <summary>
        /// ServiceCode、ImportUser、Bank
        /// </summary>
        public string Type { get; set; }

    }
}
