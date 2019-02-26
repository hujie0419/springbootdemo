using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class VipCardModel
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public Guid ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string VipCards { get; set; }
        public string Url { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public int TotalCount { get; set; }
    }

    public class VipCardDetailModel
    {
        public int Pkid { get; set; }
        public Guid ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string Url { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        /// <summary>
        /// 企业客户id
        /// </summary>
        public int ClientId { get; set; }
        /// <summary>
        /// 企业客户名称
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 批次id
        /// </summary>
        public string BatchId { get; set; }
        /// <summary>
        /// Vip卡名称
        /// </summary>
        public string CardName { get; set; }
        /// <summary>
        /// Vip卡面额
        /// </summary>
        public int CardValue { get; set; }
        /// <summary>
        /// 销售单价
        /// </summary>
        public int SalePrice { get; set; }
        /// <summary>
        /// 使用范围
        /// </summary>
        public string UseRange { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }
        public int TotalCount { get; set; }

        public int Stock { get; set; }
        public bool _checked { get; set; }
    }

    public class VipCardLogModel
    {
        public string ActivityName { get; set; }

        public string BatchIds { get; set; }
    }
}
