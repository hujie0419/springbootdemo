using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Models
{
    /// <summary>
    /// 商品信息模型
    /// <remarks>
    /// 创建：2015.05.21
    /// 修改：2015.05.21
    /// </remarks>
    /// </summary>
    public class ProductInfoModel
    {
        public String UID { get; set; }
        public String ProductNo { get; set; }
        public String ProductName { get; set; }
        public String Weight { get; set; }
        public String ChanDi { get; set; }
        public String BrandName { get; set; }
        public String CatalogName { get; set; }
        public String Images { get; set; }
        public String Standard { get; set; }
        public String Recommend { get; set; }
        public String Barcode { get; set; }
        public double MarketPrice { get; set; }
        public double TaoQiPrice { get; set; }
        public double TaoBaoPrice { get; set; }
        public double JDPrice { get; set; }
        public double ExPrice1 { get; set; }
        public double ExPrice2 { get; set; }
        public double ExPrice3 { get; set; }
        public double ExPrice4 { get; set; }
        public String Extension { get; set; }
        public int Availability { get; set; }
        public int IsRemove { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public String Createduser { get; set; }
        public String Updateduser { get; set; }

		//public List<ProductAttributesModel> productAttributes = null;

		//public PriceInventoryModel priceInventory = null;

		//public ProductAreaInfoModel productAreaInfo = null;
    }
}
