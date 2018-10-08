using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class BeautyServicePackageDetail
    {
        public int PKID { get; set; }

        public int PackageId { get; set; }

        public string PID { get; set; }

        public int ServiceCodeTypeId { get; set; }

        public string Name { get; set; }
        /// <summary>
        /// 和大客户的结算价
        /// </summary>
        public decimal VipSettlementPrice { get; set; }
        /// <summary>
        /// 和门店的佣金比例
        /// </summary>
        public decimal ShopCommission { get; set; }
        /// <summary>
        /// PreSettled or ByPeriod
        /// </summary>
        public string SettlementMethod { get; set; }

        public int Num { get; set; }
        /// <summary>
        /// 生成服务码的数量
        /// </summary>
        public int ServiceCodeNum { get; set; }

        public DateTime? ServiceCodeStartTime { get; set; }

        public DateTime? ServiceCodeEndTime { get; set; }

        public int EffectiveDayAfterExchange { get; set; }

        public bool IsActive { get; set; }

        public bool IsServiceCodeGenerated { get; set; }

        public int BuyoutOrderId { get; set; }

        public bool IsImportUser { get; set; }

        public string CreateUser { get; set; }

        public string UpdateUser { get; set; }
        /// <summary>
        /// 合作用户ID
        /// </summary>
        public int CooperateId { get; set; }
        /// <summary>
        /// 合作用户名
        /// </summary>
        public string CooperateName { get; set; }
    }
}
