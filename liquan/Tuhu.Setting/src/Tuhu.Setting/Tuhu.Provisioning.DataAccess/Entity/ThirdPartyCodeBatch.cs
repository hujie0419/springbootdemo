using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ThirdPartyCodeBatch
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 批次ID
        /// </summary>
        public Guid BatchGuid { get; set; }
        /// <summary>
        /// 批次名称
        /// </summary>
        public string BatchName { get; set; }
        /// <summary>
        /// 批次数量
        /// </summary>
        public int BatchQty { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public int StockQty { get; set; }
        /// <summary>
        /// 限领数量
        /// </summary>
        public int LimitQty { get; set; }
        /// <summary>
        /// 使用说明
        /// </summary>
        public string Instructions { get; set; }
        /// <summary>
        /// 兑换开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 兑换截止时间
        /// </summary>
        public DateTime EndDateTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDateTime { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        public string Modifier { get; set; }
    }

    public class SerchElement
    {
        /// <summary>
        /// 当前所在页
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// 每页数据条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 批次id
        /// </summary>
        public Guid? BatchGuid { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Instructions { get; set; }
        /// <summary>
        /// 批次名称
        /// </summary>
        public string BatchName { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 最后操作人
        /// </summary>
        public string Modifier { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public int Sort { get; set; }

        public bool? Status { get; set; }
    }

}
