using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Models
{
    [Serializable]
    public class QplProductInfoModel
    {
        public string ID { get; set; }
        /// <summary>
        /// 途虎编号
        /// </summary>
        public string ThPid { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 商品价格
        /// </summary>
        public decimal ListPrice { get; set; }

        /// <summary>
        ///上架状态 1上架 0下架
        /// </summary>
        public int UAvailability { get; set; }
        /// <summary>
        /// 是否移除  0未移除 1 已移除
        /// </summary>
        public int UPIsReMove { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedDate { get; set; }
        /// <summary>
        /// 查查记录编号
        /// </summary>
        public int RN { get; set; }

    }
}
