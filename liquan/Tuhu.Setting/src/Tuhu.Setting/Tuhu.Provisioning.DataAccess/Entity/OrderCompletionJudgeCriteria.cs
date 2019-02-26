using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class OrderCompletionJudgeCriteria
    {
        /// <summary>
        /// 安装门店Id
        /// </summary>
        public int? InstallShopId { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 订单递送状态
        /// </summary>
        public string DeliveryStatus { get; set; }

        /// <summary>
        /// 订单安装时间
        /// </summary>
        public DateTime? InstallDatetime { get; set; }

        /// <summary>
        /// 订单递送时间
        /// </summary>
        public DateTime? DeliveryDatetime { get; set; }

        /// <summary>
        /// 递送日志表中的订单递送时间
        /// </summary>
        public DateTime? DeliveryDatetimeInLog { get; set; }

        /// <summary>
        /// 递送日志表中的订单递送状态
        /// </summary>
        public string DeliveryStatusInLog { get; set; }
    }
}
