using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.GeneralBeautyServerCode
{
    public class GeneralBeautyServerCodes
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 包Id
        /// </summary>
        public Guid PackageId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 服务码
        /// </summary>
        public string ServiceCode { get; set; }
        /// <summary>
        /// 兑换码
        /// </summary>
        public string RedemptionCode { get; set; }
        /// <summary>
        /// 结算价
        /// </summary>
        public decimal SettlementPrice { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        public string SettlementMethod { get; set; }
        /// <summary>
        /// 佣金比
        /// </summary>
        public decimal ShopCommission { get; set; }
        /// <summary>
        /// PID
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 适配车型
        /// </summary>
        public int AdapterVehicle { get; set; }
        /// <summary>
        /// 合作用户
        /// </summary>
        public string CooperateName { get; set; }
        /// <summary>
        /// 核销时间
        /// </summary>
        public DateTime? VerifyTime { get; set; }
        /// <summary>
        /// 订单Id
        /// </summary>
        public int TuhuOrderId { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}
