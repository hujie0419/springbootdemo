using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.WebSite.Web.Activity.Models
{
    public class NewsInfoModel
    {
        public List<tbl_ArticleModel> ArticleItems { get; set; }
        public List<tbl_CommentModel> CommentItems { get; set; }
    }

    /// <summary>
    /// 文章评论
    /// </summary>
    public class tbl_CommentModel
    {
        public string UserHead { get; set; }
        public string CommentContent { get; set; }
        public DateTime CommentTime { get; set; }
        public string UserName { get; set; }
        public string UserGrade { get; set; }
        public int CommentCount { get; set; }
    }

    /// <summary>
    /// 文章
    /// </summary>
    public class tbl_ArticleModel
    {
        public int PKID { get; set; }
        public string Catalog { get; set; }
        public string Image { get; set; }
        public string SmallImage { get; set; }
        public string SmallTitle { get; set; }
        public string BigTitle { get; set; }
        public string TitleColor { get; set; }
        public string Brief { get; set; }

        //public string Content { get; set; }
        public string ContentUrl { get; set; }
        public string Source { get; set; }
        /// <summary>/// 发布时间/// </summary>
        public DateTime PublishDateTime { get; set; }
        /// <summary>/// 创建时间/// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>/// 最后修改时间/// </summary>
        public DateTime LastUpdateDateTime { get; set; }
        /// <summary>/// 点击数/// </summary>
        public string ClickCount { get; set; }
        /// <summary>/// 重定向地址/// </summary>
        public string RedirectUrl { get; set; }
        /// <summary>/// 文章类别ID/// </summary>
        public string Category { get; set; }
        /// <summary>/// 文章类别名字/// </summary>
        public string CategoryName { get; set; }
        /// <summary>/// 热度/// </summary>
        public int Heat { get; set; }
        /// <summary>/// 审核是否通过/// </summary>
        public bool CommentIsActive { get; set; }
        public int IsRead { get; set; }
        /// <summary>///  特推主题大图片/// </summary>
        public string ArticleBanner { get; set; }
        /// <summary>///  特推主题小图片/// </summary>
        public string SmallBanner { get; set; }
        /// <summary>/// 特推主题勾选状态/// </summary>
        public bool Bestla { get; set; }
        /// <summary>/// 热门文章/// </summary>
        public bool HotArticles { get; set; }
        public int Type { get; set; }
        /// <summary>
        /// 喜欢总数
        /// </summary>
        public int Vote { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public int IsShow { get; set; }
        public int CommentCount { get; set; }
    }


    public class ArticleInfosModel
    {
        public tbl_ArticleModelNew ArticleDetail { get; set; }//文章的详情
        public List<tbl_ArticleModelNew> RelatedArticles { get; set; }//相关文章4篇
    }

    /// <summary>
    /// 文章另一种显示（用于PC端的发现）
    /// </summary>
    public class tbl_ArticleModelNew
    {
        public int PKID { get; set; }
        public string Catalog { get; set; }
        public string Image { get; set; }
        public string SmallImage { get; set; }
        public string SmallTitle { get; set; }
        public string BigTitle { get; set; }
        public string TitleColor { get; set; }
        public string Brief { get; set; }
        public string Content { get; set; }
        public string ContentUrl { get; set; }

        /// <summary>/// 发布时间/// </summary>
        public DateTime PublishDateTime { get; set; }
        /// <summary>/// 创建时间/// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>/// 最后修改时间/// </summary>
        public DateTime LastUpdateDateTime { get; set; }
        /// <summary>/// 点击数/// </summary>
        public string ClickCount { get; set; }
        /// <summary>/// 文章类别ID/// </summary>
        public string Category { get; set; }
        /// <summary>/// 文章类别名字/// </summary>
        public string CategoryName { get; set; }
        /// <summary>/// 文章类别名字/// </summary>
        public string CategoryTags { get; set; }

        /// <summary>/// 热门文章/// </summary>
        public bool HotArticles { get; set; }
        public int Type { get; set; }
        /// <summary>
        /// 喜欢总数
        /// </summary>
        public int Vote { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public int IsShow { get; set; }
        public int CommentCount { get; set; }

        public string Status { get; set; }

        public string ContentHTML { get; set; }

    }


}