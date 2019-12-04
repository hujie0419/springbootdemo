using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Models.OrderCashBack
{
    /// <summary>
    /// 订单返现助力发起记录表实体 [Activity].[dbo].[OrderCashBackOwner]
    /// </summary>
    public class OrderCashBackOwnerModel
    {
        public int PKID { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public int OrderId { get; set; }

        public Guid UserId { get; set; }

        /// <summary>
        /// 分享id
        /// </summary>
        public Guid ShareId { get; set; }
    }
}
