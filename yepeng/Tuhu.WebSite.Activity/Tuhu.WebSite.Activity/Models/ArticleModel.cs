using System;
using System.Collections.Generic;
using System.Data;
using Tuhu.WebSite.Component.SystemFramework.Models;

namespace Tuhu.WebSite.Web.Activity.Models
{
    [Serializable]
    public class ArticleModel : BaseModel
    {
        public int PKID { get; set; }
        public int RelatedArticleId { get; set; }
        public string Catalog { get; set; }
        public int ShowType { get; set; }
        public string ShowImages { get; set; }
        public string Image { get; set; }
        public string SmallImage { get; set; }
        public string SmallTitle { get; set; }
        public string BigTitle { get; set; }
        public string TitleColor { get; set; }
        /// <summary>/// 文章小描述/// </summary>
        public string Brief { get; set; }
        public string Content { get; set; }
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
        /// <summary> 评论总数/// </summary>
        public string CommentNum { get; set; }
        public string CommentTimes { get; set; }

        /// <summary>赞/// </summary>
        public int Vote { get; set; }
        /// <summary>显示（今天-昨天-前天）/// </summary>
        public DateTime AnnotationTime { get; set; }
        /// <summary>点赞状态/// </summary>
        public bool VoteState { get; set; }
        public string CategoryTags { get; set; }
        public int ShareWx { get; set; }
        public int Sharepyq { get; set; }
        public double Hot { get; set; }

        public int Type { get; set; }
        public ArticleModel() : base() { }
        public ArticleModel(DataRow row) : base(row)
        {
            SmallImage = string.IsNullOrWhiteSpace(SmallImage) ? "http://image.tuhu.cn/activity/image/ShareImage.jpg" : SmallImage;
        }
    }

    public class ActionDataModel
    {
        public string Code { get; set; }
        public string Msg { get; set; }
        public ActionDataDetail Detail { get; set; }
    }

    public class ActionDataDetail
    {
        public int PKID { get; set; }
        public string SingleTitle { get; set; }
        public string Content { get; set; }
        public string Images { get; set; }
        public string UserExtAttr { get; set; }
        public string UserVehicle { get; set; }
        public string UserName { get; set; }

        public DateTime CreateTime { get; set; }

        public int Type { get; set; }
        public List<ActionProductModel> ProductList { get; set; }
    }

    public class ActionProductModel
    {
        public string PID { get; set; }
        public string ProductPic { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductNum { get; set; }
        public string ProductUrl { get; set; }
    }


    public class ForumDataModel
    {
        public object data { get; set; }

        public string code { get; set; }
    }

    public class ForumDetailModel
    {
        public int id { get; set; }

        public string title { get; set; }

        public string body { get; set; }
        public List<ForumBodyModel> body_original { get; set; }

        public int reply_count { get; set; }
        public int vote_count { get; set; }
        public int view_count { get; set; }

        public string cover_image_url { get; set; }

        public string created_at { get; set; }

        public ForumDataModel user { get; set; }

        public ForumDataModel category { get; set; }
    }
    public class ForumReplyModel
    {
        public int id { get; set; }
        public int topic_id { get; set; }

        public string user_name { get; set; }
        public string body { get; set; }       
        public string body_original { get; set; }
        public string[] image_urls { get; set; }
        public string created_at { get; set; }
        public int source_id { get; set; }
        public ForumDataModel user { get; set; }
        public ForumDataModel replyto { get; set; }
    }

    public class ForumUserModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
    }
    public class ForumCategoryModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
    public class ForumBodyModel
    {
        public string content { get; set; }
        public string description { get; set; }
        public string type { get; set; }
    }
    /// <summary>
    /// 作者信息
    /// </summary>
    public class CoverAuthorModel
    {
        public CoverAuthorModel() { }
        /// <summary>
        /// 默认作者信息
        /// </summary>
        /// <param name="name"></param>
        public CoverAuthorModel(string name)
        {
            this.AuthorName = !string.IsNullOrWhiteSpace(name) ? name : "虎小编";
            this.AuthorHead = "https://res.tuhu.org/Image/Product/zhilaohu.png";
            this.Description = "";
        }
        public string AuthorName { get; set; }
        public string AuthorHead { get; set; }
        public string Description { get; set; }
    }
    /// <summary>
    /// 标签信息
    /// </summary>
    public class CategoryTagsModel
    {
        public int key { get; set; }
        public string value { get; set; }
        public string isShow { get; set; }
    }
    public class RelatedArticleModel
    {
         public int PKID { get; set; }
         public string SmallTitle { get; set; }
         public string CoverTag { get; set; }
         public string CoverImage { get; set; }
         public DateTime PublishDateTime { get; set; }
         public int Type { get; set; }
         public int ClickCount { get; set; }
         public string Image { get; set; }
    }
    public class ArticleVoteModel
    {
        public string UserId { get; set; }
    }
    public class ArticleListModel
    {
        public int Pkid { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public string PublishDateTime { get; set; }
        public int ClickCount { get; set; }
        public string ShowImage { get; set; }
        public string CoverTag { get; set; }
        public string ContentUrl
        {
            get
            {
                return Type == 9 ? $"https://wx.tuhu.cn/vue/preference/pages/detail/index?articleId={Pkid}&navHidden=1" : $"{DomainConfig.FaXian}/react/find_new/#/detail?bgColor=%252523ffffff&id={Pkid}&textColor=%252523333333&userid=&_k=xpbooq";
            }
        }
    }
}
