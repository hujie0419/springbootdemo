using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BeautyServicePackageSimpleModel
    {
        public int PKID { get; set; }

        public int PackageId => PKID;

        public string PackageName { get; set; }
    }


    public class BeautyServicePackageDetailSimpleModel
    {
        public int PKID { get; set; }

        public string Name { get; set; }

        public int PackageDetailId => PKID;
    }

    public class CreateBeautyCodeTaskModel
    {
        public int PKID { get; set; }

        public string MobileNumber { get; set; }

        public int? Quantity { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string BatchCode { get; set; }

        public int MappingId { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public string CreateUser { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime? LastUpdateDateTime { get; set; }

    }

    public class BeautyCodeStatistics
    {
        public string BatchCode { get; set; }

        public int MappingId { get; set; }

        /// <summary>
        /// 合作名
        /// </summary>
        public string CooperateName { get; set; }
        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 大客户结算价
        /// </summary>
        public decimal VipSettlementPrice { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        public string SettlementMethod { get; set; }

        public string CreateUser { get; set; }

        public string Type { get; set; }

        public int TotalCount { get; set; }

        public int Count { get; set; }
        /// <summary>
        /// 买断订单号
        /// </summary>
        public int BuyoutOrderId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
    }

}
