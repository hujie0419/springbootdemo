using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{
    public class YouXuanArticle
    {
        /// <summary>
        /// 文章主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        public string SmallTitle { get; set; }

        public string BigTitle { get; set; }
        /// <summary>
        /// 文章内容
        /// </summary>
        public string Content { get; set; }
        public string ContentHtml { get; set; }
        public string SmallImage { get; set; }
        public string Image { get; set; }
        public string ContentUrl { get; set; }

        public string Brief { get; set; }
        /// <summary>
        /// 文章列表(封面)图片
        /// </summary>
        public string ShowImages { get; set; }

        /// <summary>
        /// 二维码图片
        /// </summary>
        public string QRCodeImg { get; set; }

        public int? ShowType { get; set; }
        /// <summary>
        /// 文章创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        public int IsShow { get; set; }

        public bool? IsDescribe { get; set; }

        /// <summary>
        /// 文章更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
        /// <summary>
        /// 文章发布时间
        /// </summary>
        public Nullable<System.DateTime> PublishDateTime { get; set; }

        /// <summary>
        /// 文章状态，对应ArticleStatus枚举
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 文章创建者Id
        /// </summary>
        public string CreatorId { get; set; }

        public int? ClickCount { get; set; }

        /// <summary>
        /// 文章收藏数
        /// </summary>
        public int? Vote { get; set; }

        /// <summary>
        /// 文章阅读数
        /// </summary>
        public int? ReadCountNum { get; set; }

        /// <summary>
        /// 文章分享数
        /// </summary>
        public int? ShareCountNum { get; set; }

        /// <summary>
        /// 文章收藏数
        /// </summary>
        public int? LikeCountNum { get; set; }

        /// <summary>
        /// 文章评论数
        /// </summary>
        public int? CommentCountNum { get; set; }

        /// <summary>
        /// 排序Ranking
        /// </summary>
        public int? Ranking { get; set; }


        /// <summary>
        /// 封面标签(或者发表者名称)
        /// </summary>
        public string CoverTag { get; set; }

        public string Category { get; set; }

        /// <summary>
        /// 文章关联的标签Json格式数据
        /// </summary>
        public string CategoryTags { get; set; }

        /// <summary>
        /// 当前文章是否置顶
        /// </summary>
        public bool? IsTopMost { get; set; }

        /// <summary>
        /// 文章是否被用户收藏(已登录用户)
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// 文章类型，新后台的文章类型为5
        /// </summary>
        public int Type { get; set; }

        public string CoverMode { get; set; }

        public string CoverImage { get; set; }

        public ArticleCoverConfig CoverConfig { get; set; }
    }

    public class ArticleCoverConfig
    {
        public int PKID { get; set; }

        public int ArticleId { get; set; }

        public string CoverType { get; set; }

        public string CoverImg { get; set; }

        public string CoverVideo { get; set; }

        public string OtherImg { get; set; }

        public string Source { get; set; }
    }
}
