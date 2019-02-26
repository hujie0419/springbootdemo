using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{
    public class CpsOrderItemRecordModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int PkId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 达人ID
        /// </summary>
        public Guid DarenID { get; set; }

        /// <summary>
        /// 商品配置表业务ID
        /// </summary>
        public Guid CpsId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
