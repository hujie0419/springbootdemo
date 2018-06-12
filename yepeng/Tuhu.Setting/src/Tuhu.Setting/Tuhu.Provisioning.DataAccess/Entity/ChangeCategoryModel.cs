using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.Mime;
using Tuhu.Component.Framework.Extension;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class GaiZhuangCategoryModel : BaseModel
    {

        public GaiZhuangCategoryModel() { }

        public GaiZhuangCategoryModel(DataRow row) : base(row)
        {
        }

        protected override void Parse(DataRow row)
        {
            base.Parse(row);
            //this.IsShow = Convert.ToInt32(row.GetValue("IsShow")) > 0 ? true : false;
            //this.IsActive = Convert.ToInt32(row.GetValue("IsActive")) > 0 ? true : false;
        }


        public int? PKID { get; set; }


        [Display(Name = "改装类目")]
        public string CategoryName { get; set; }

        [Display(Name = "父类目ID")]
        public int? ParentCategoryID { get; set; }

        [Display(Name = "类目级别")]
        public int? CategoryLevel { get; set; }

        [Display(Name = "类目类别")]
        public int? CategoryType { get; set; }
        [Display(Name = "副标题")]
        public string SubTitle { get; set; }
        [Display(Name = "排序")]
        public int? Sort { get; set; }
        [Display(Name = "是否显示")]
        public bool IsShow { get; set; }

        [Display(Name = "显示商品个数")]
        public int? ShowNum { get; set; }

        [Display(Name = "是否有效")]
        public bool IsActive { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "更新时间")]
        public DateTime? UpdatedTime { get; set; }

    }

    public class GaiZhuangRelateProductModel : BaseModel
    {
        public GaiZhuangRelateProductModel() { }

        public GaiZhuangRelateProductModel(DataRow row) : base(row)
        {
            base.Parse(row);
        }

        public int? PKID { get; set; }
        [Display(Name = "类目ID")]
        public int CategoryId { get; set; }


        [Display(Name = "关联商品PID")]
        public string PID { get; set; }

        [Display(Name = "是否有效")]
        public bool IsActive { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "更新时间")]
        public DateTime? UpdatedTime { get; set; }

    }
    public class GaiZhuangRelateArticleModel : BaseModel
    {
        public GaiZhuangRelateArticleModel() { }

        public GaiZhuangRelateArticleModel(DataRow row) : base(row)
        {
            base.Parse(row);
        }

        public int? PKID { get; set; }
        [Display(Name = "类目ID")]
        public int CategoryId { get; set; }

        [Display(Name = "文章名称")]
        public string ArticleName { get; set; }
        [Display(Name = "文章链接")]
        public string ArticleLink { get; set; }
        [Display(Name = "文章匹配车型")]
        public string VehicleID { get; set; }

        [Display(Name = "是否有效")]
        public bool IsActive { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "更新时间")]
        public DateTime? UpdatedTime { get; set; }

    }
    public class AdvertCategoryModel : BaseModel
    {
        public AdvertCategoryModel() { }

        public AdvertCategoryModel(DataRow row) : base(row)
        {
            base.Parse(row);
        }

        public int? PKID { get; set; }
        [Display(Name = "类目ID")]
        public int? CategoryId { get; set; }

        [Display(Name = "广告类别")]
        public int AdvertType { get; set; }

        [Display(Name = "广告类别名称")]
        public string AdvertTypeName { get; set; }
        [Display(Name = "广告名")]
        public string AdvertName { get; set; }
        [Display(Name = "图片路径")]
        public string ImageURL { get; set; }
        [Display(Name = "排序")]
        public int? Sort { get; set; }

        [Display(Name = "APP链接")]
        public string AppLink { get; set; }

        [Display(Name = "移动链接")]
        public string H5Link { get; set; }

        [Display(Name = "优惠券ID")]
        public string PromotionRuleID { get; set; }

        [Display(Name = "优惠券名字")]
        public string PromotionName { get; set; }
        [Display(Name = "优惠券说明")]
        public string PromotionDescription { get; set; }
        [Display(Name = "是否显示")]
        public bool IsShow { get; set; }
        [Display(Name = "是否有效")]
        public bool IsActive { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "更新时间")]
        public DateTime? UpdatedTime { get; set; }
    }

    public class RelateProductModel : BaseModel
    {
        public RelateProductModel() { }

        public RelateProductModel(DataRow row) : base(row)
        {
            base.Parse(row);
        }

        [Display(Name = "商品PID")]
        public string PID { get; set; }

        [Display(Name = "商品名称")]
        public string ProductName { get; set; }

        [Display(Name = "商品类别")]
        public string ProductType { get; set; }
        [Display(Name = "上下架")]
        public string OnSale { get; set; }


    }

    public class SelectedRelateProductModel : BaseModel
    {
        public SelectedRelateProductModel()
        {
        }

        public SelectedRelateProductModel(DataRow row) : base(row)
        {
            base.Parse(row);
        }

        public int PKID { get; set; }

        [Display(Name = "关联类目ID")]
        public string CategoryId { get; set; }

        [Display(Name = "商品PID")]
        public string PID { get; set; }

        [Display(Name = "商品名称")]
        public string ProductName { get; set; }

        [Display(Name = "商品类别")]
        public string ProductType { get; set; }

        [Display(Name = "上下架")]
        public string OnSale { get; set; }

        public int? RegionId { get; set; }

        public List<int> RegionIds { get; set; }

        public string StrCityNames { get; set; }
        public string StrCityIDs { get; set; }

        public int CityType { get; set; }=1;
        public string Brand { get; set; }


    }

    public class ArticleAdaptVehicleModel : BaseModel
    {
        public ArticleAdaptVehicleModel() { }

        public ArticleAdaptVehicleModel(DataRow row) : base(row)
        {
            base.Parse(row);
        }

        public int? PKID { get; set; }
        [Display(Name = "类目ID")]
        public int ArticleId { get; set; }

        [Display(Name = "文章匹配车品牌")]
        public string Brand { get; set; }
        [Display(Name = "文章匹配车型")]
        public string VehicleID { get; set; }
        [Display(Name = "文章匹配车年份")]
        public string Nian { get; set; }
        [Display(Name = "文章匹配车排量")]
        public string PaiLiang { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "更新时间")]
        public DateTime? UpdatedTime { get; set; }

    }


    public class CategoryModel : BaseModel
    {
        public CategoryModel() : base() { }
        public CategoryModel(DataRow row) : base(row) { }

        public int Oid { get; set; }
        public int ParentOid { get; set; }
        public string CategoryName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string NodeNo { get; set; }

        public int FirstParentOid { get; set; }

    }
}
