using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Models.Rebate
{
    /// <summary>
    /// 门店订单派工技师信息
    /// </summary>
    public class ShopOrderTechResult
    {
        /// <summary>
        /// 返回码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 返回码的描述信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 返回状态
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 订单派工技师信息
        /// </summary>
        public List<DataInfo> Data { get; set; }
    }

    public class DataInfo
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// 技师Id
        /// </summary>
        public int TechId { get; set; }
    }
}
