using System;
using System.Collections.Generic;
//using System.Data;
using Tuhu.WebSite.Component.SystemFramework.Models;
using Tuhu.WebSite.Component.SystemFramework.Extensions;
using System.Linq;

namespace Tuhu.WebSite.Web.Activity.Models
{
    public class ProductModel : BaseModel
    {
        public ProductModel() { }
        internal ProductModel(System.Data.DataRow row) : base(row) { }

        /// <summary>主键</summary>
        public int Oid { get; set; }
        /// <summary>产品系列编号</summary>
        public string ProductID { get; set; }
        /// <summary>产品类别</summary>
        public string Category { get; set; }
        /// <summary>Definition Name</summary>
        public string DefinitionName { get; set; }
        /// <summary>显示名称</summary>
        public string DisplayName { get; set; }
        /// <summary>产品文字介绍</summary>
        public string Description { get; set; }
        /// <summary>配套(isOE)</summary>
        public bool IsOE { get; set; }
        /// <summary>推荐度(ProductRefer)</summary>
        public int ProductRefer { get; set; }
        /// <summary>提供发票(invoice)</summary>
        public bool Invoice { get; set; }
        /// <summary>价格(cy_list_price)</summary>
        public decimal? Price { get; set; }
        /// <summary>市场价(cy_marketing_price)</summary>
        public decimal? MarketingPrice { get; set; }
        /// <summary>品牌(CP_Brand)</summary>
        public string Brand { get; set; }
        /// <summary>包装尺寸(CP_Unit)</summary>
        public string Unit { get; set; }
        /// <summary>产品产地(Place CP_Place)</summary>
        public string Place { get; set; }

        /// <summary>上架中(Onsale)</summary>
        public bool Onsale { get; set; }

        /// <summary>缺货(Onsale)</summary>
        public bool Stockout { get; set; }

        #region 产品分组使用
        /// <summary>产品代码(ProductCode)</summary>
        public string ProductCode { get; set; }
        /// <summary>产品颜色(ProductColor)</summary>
        public string ProductColor { get; set; }
        /// <summary>产品尺寸(ProductSize)</summary>
        public string ProductSize { get; set; }
        #endregion

        /// <summary>产品图片</summary>
        public ProductImageModel Image { get; set; }

        /// <summary>产品评论明细</summary>
        public IEnumerable<ProductCommentModel> Comments { get; set; }

        protected override void Parse(System.Data.DataRow row)
        {
            if (row == null)
                return;

            if (row.HasValue("oid"))
                Oid = Convert.ToInt32(row["oid"]);

            ProductID = row.GetValue("ProductID");
            Category = row.GetValue("Category");
            DefinitionName = row.GetValue("DefinitionName");
            DisplayName = row.GetValue("DisplayName");
            Description = row.GetValue("Description");

            int result;
            if (int.TryParse(row.GetValue("isOE"), out result) && result > 0)
                IsOE = true;

            if (int.TryParse(row.GetValue("ProductRefer"), out result))
                ProductRefer = result;

            if (row.HasValue("invoice") && string.Compare(row["invoice"].ToString(), "Yes", true) == 0)
                Invoice = true;

            if (row.HasValue("cy_list_price"))
                Price = Convert.ToDecimal(row["cy_list_price"]);
            if (row.HasValue("cy_marketing_price"))
                MarketingPrice = Convert.ToDecimal(row["cy_marketing_price"]);

            Brand = row.GetValue("CP_Brand");
            Unit = row.GetValue("CP_Unit");
            Place = row.GetValue("CP_Place");

            ProductCode = row.GetValue("ProductCode");
            ProductColor = row.GetValue("ProductColor");
            ProductSize = row.GetValue("ProductSize");

            if (row.HasValue("Onsale"))
                Onsale = Convert.ToBoolean(row.GetValue("Onsale"));
            else
                Onsale = true;
            Onsale = Onsale && (Price.GetValueOrDefault() > 0);

            if (row.HasValue("stockout"))
                Stockout = Convert.ToBoolean(row.GetValue("stockout"));

            Image = new ProductImageModel(row);
        }
    }

    public class ProductImageModel : BaseModel
    {
        public ProductImageModel() { }
        public ProductImageModel(System.Data.DataRow row) : base(row) { }

        /// <summary>Image_width</summary>
        public int Width { get; set; }
        /// <summary>Image_height</summary>
        public int Height { get; set; }
        public IEnumerable<string> ImageUrls { get; set; }

        protected override void Parse(System.Data.DataRow row)
        {
            var value = 0;
            if (row.HasValue("Image_height") && int.TryParse(row["Image_height"].ToString(), out value))
                this.Height = value;
            if (row.HasValue("Image_width") && int.TryParse(row["Image_width"].ToString(), out value))
                this.Width = value;

            var list = new List<string>();
            list.Add(row.GetValue("Image_filename_Big"));
            list.Add(row.GetValue("Image_filename"));
            list.Add(row.GetValue("Image_filename_2"));
            list.Add(row.GetValue("Image_filename_3"));
            list.Add(row.GetValue("Image_filename_4"));
            list.Add(row.GetValue("Image_filename_5"));
            ImageUrls = list.Where(img => !string.IsNullOrWhiteSpace(img)).Distinct().ToArray();
        }
    }


}