using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class VW_StockProduct
    {

        public string DisplayName { get; set; }

        public string CP_Tire_ROF { get; set; }

        public string ProductID { get; set; }

        public string VariantID { get; set; }

        public System.Nullable<int> AvailableNum { get; set; }

        public string Location { get; set; }

        public int LocationId { get; set; }

        public string CP_Tire_Width { get; set; }

        public string CP_Tire_AspectRatio { get; set; }

        public string CP_Tire_Rim { get; set; }

        public string PID { get; set; }

        public System.Nullable<decimal> cy_list_price { get; set; }

        public int SoStockNum { get; set; }

        public int TransferStockNum { get; set; }

        public int SoStockBatchId { get; set; }
    }
    public class Article
    {
        public int PKID { get; set; }
        public int Catalog { get; set; }
        public string Image { get; set; }
        public string SmallImage { get; set; }
        public string SmallTitle { get; set; }
        public string BigTitle { get; set; }
        public string TitleColor { get; set; }
        public string Brief { get; set; }
        public string Content { get; set; }
        public string ContentHtml { get; set; }
        public string ContentUrl { get; set; }
        public string Source { get; set; }
        public DateTime PublishDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public int ClickCount { get; set; }
        public string RedirectUrl { get; set; }
        /// <summary>
        /// 文章类目  
        ///			1：汽车百科
        ///			2：驾驶技巧
        ///			3：保养秘诀
        ///			4：必备车品
        /// </summary>
        public string Category { get; set; }
        /// <summary>文字评论开关<summary>
        public bool CommentIsActive { get; set; }
        /// <summary>评论总数</summary>
        public long CommentCount { get; set; }
        public int Vote { get; set; }
        ////点赞数
        //public int? _Vote;
        //public int ? Vote {
        //    get { return _Vote ?? 0; }
        //    set { _Vote = value; }
        //}

        public string ArticleBanner { get; set; }

        public string SmallBanner { get; set; }

        public bool Bestla { get; set; }

        public bool HotArticles { get; set; }

        /// <summary>
        /// 文章类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public int IsShow { get; set; }

        /// <summary>
        /// 是否展示在头条
        /// </summary>
        public int IsShowTouTiao { get; set; }

        //是否用于发现频道
        public int IsFaxianChannel { get; set; }

        //类目名称
        public string CategoryName { get; set; }

        //当前文章浏览时间
        public DateTime CurrentOperateTime { get; set; }
        //当前文章浏览次数
        public int CurrentClickCount { get; set; }
        //当前日期前一天浏览次数
        public int YesterdayClickCount { get; set; }
        //当前日期前一天浏览总次数
        public int YesterdaySumCount { get; set; }

        //跳转本地地址的前一天浏览次数
        public int SkipLocalUrlCount { get; set; }

        //跳转淘宝地址的前一天浏览次数
        public int SkipTaoBaoUrlCount { get; set; }

        public int ShareWX { get; set; }
        public int SharePYQ { get; set; }
        /// <summary>
        /// 文章标签
        /// </summary>
        public string CategoryTags { get; set; }

        /// <summary>
        /// 多图片资源
        /// </summary>
        public string ShowImages { get; set; }

        /// <summary>
        /// 多图片资源显示类型
        /// </summary>
        public int ShowType { get; set; }

        /// <summary>
        /// 是否显示描述
        /// </summary>
        public bool IsDescribe { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string CreatorID { get; set; }


        public string CreatorInfo { get; set; }
    }

    public class SeekHotWord
    {
        public int id { get; set; }

        public string HotWord { get; set; }

        public DateTime? CreateTime { get; set; }

        public bool OnOff { get; set; }
    }
    public class SeekKeyWord
    {
        public int id { get; set; }

        public string KeyWord { get; set; }

        public string Versions { get; set; }

        public string Channel { get; set; }

        public DateTime? CreateTime { get; set; }

        public int Num { get; set; }
    }

    public class ArticleCategory
    {
        public int id { get; set; }
        public string CategoryName { get; set; }
        public int Sort { get; set; }
        public string Color { get; set; }
    }

    public class ArticleNewList
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public string ArticleUrl { get; set; }
        public string ProductId { get; set; }
        public int Type { get; set; }
        public DateTime CreateTime { get; set; }
        public string Field_1 { get; set; }
        public string Field_2 { get; set; }
        public string Field_3 { get; set; }
        public string Field_4 { get; set; }
    }

    public class ArticleContentModel
    {
        public string title { get; set; }
        public string describe { get; set; }
        public string maxPic { get; set; }
        public string minPic { get; set; }
        public string productName { get; set; }
        public string productPrice { get; set; }
        public string productUrl { get; set; }
    }

}
