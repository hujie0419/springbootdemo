using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Models
{
    /// <summary>
    /// 购物车模型
    /// <remarks>
    /// 创建：2015.05.21
    /// 修改：2015.05.21
    /// </remarks>
    /// </summary>
    public class UserProductInfoModel
    {
        public String UID { get; set; }
        public String ProductId { get; set; }
        public String UserId { get; set; }
        public String CurrentPrice { get; set; }
        public String LastPrice { get; set; }
        public String ProductNo { get; set; }
        public String ProductName { get; set; }
        public int Reserved { get; set; }
        public String Contact { get; set; }
        public double MarketPrice { get; set; }
        public double ListPrice { get; set; }
        public int Inventory { get; set; }

        public String UserSKU { get; set; }
        /// <summary>
        /// 用户条码
        /// </summary>
        public String UserBarcode { get; set; }
        public String Gift { get; set; }
        public String Barcode { get; set; }
        public String Extension { get; set; }
        /// <summary>
        /// 0：上架；1：下架；2：禁售
        /// </summary>
        public int Availability { get; set; }
        public int IsRemove { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public String Createduser { get; set; }
        public String Updateduser { get; set; }
        public PriceInventoryModel priceInventory { get; set; }

        public ProductInfoModel ProductInfo { get; set; }
    }
}
