using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Models
{
    /// <summary>
    /// **模型
    /// <remarks>
    /// 创建：2015.05.21
    /// 修改：2015.05.21
    /// </remarks>
    /// </summary>
    public class PriceInventoryModel
    {
        public String UID { get; set; }
        public String UserId { get; set; }
        public String UserSKU { get; set; }
        public String ProductId { get; set; }
        public String UserProductId { get; set; }
        public double ListPrice { get; set; }
        public double LimitPrice { get; set; }
        public int Inventory { get; set; }
        public int Reserved { get; set; }
        /// <summary>
        /// 上下架
        /// </summary>
        public int Availability { get; set; }
        /// <summary>
        /// 是否可以销售
        /// </summary>
        public int IsRemove { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public ProductInfoModel productInfo = null;
    }
}
