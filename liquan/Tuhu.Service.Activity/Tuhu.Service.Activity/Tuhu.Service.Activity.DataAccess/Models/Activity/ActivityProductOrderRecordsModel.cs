using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{

    /// <summary>
    /// 表[Tuhu_log].[dbo].[ActivityProductOrderRecords] 的实体类
    /// DESC:活动的下单关键信息记录 
    /// </summary>
    public class ActivityProductOrderRecordsModel
    {

        public int Pkid { get; set; }

        /// <summary>
        /// 活动id
        /// </summary>
        public Guid ActivityId { get; set; }

        public string Pid { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 订单id
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        public string Phone { get; set; }

    }
}
