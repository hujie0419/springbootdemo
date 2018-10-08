using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{

    [Table("tbl_Article")]
    public class Article
    {

        public Article()
        {
            this.CreateDateTime = DateTime.Now;
        }

        /// <summary>
        /// 文章主键
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string SmallTitle { get; set; }

        public string BigTitle { get; set; }
        /// <summary>
        /// 文章内容
        /// </summary>
        [Required]
        public string Content { get; set; }
        public string ContentHtml { get; set; }
        public string SmallImage { get; set; }
        public string Image { get; set; }
        public string ContentUrl { get; set; }

        /// <summary>
        /// 文章列表(封面)显示模式
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string CoverMode { get; set; }

        public string CoverImage { get; set; }

        public string Brief { get; set; }
        /// <summary>
        /// 文章列表(封面)图片
        /// </summary>

        [MaxLength(2000)]
        public string ShowImages { get; set; }

        /// <summary>
        /// 二维码图片
        /// </summary>
        public string QRCodeImg { get; set; }

        public int? ShowType { get; set; }
        /// <summary>
        /// 文章创建时间
        /// </summary>
        [Required]
        public DateTime CreateDateTime { get; set; }

        public int IsShow { get; set;}

        /// <summary>
        /// 是否展示在头条
        /// </summary>
        public bool? IsShowTouTiao { get; set; }
        /// <summary>
        /// 是否用于发现频道
        /// </summary>
        [NotMapped]
        public bool? IsShowFaxian { get; set; }

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
        [MaxLength(30)]
        [Required]
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
        [MaxLength(100)]
        public string CoverTag { get; set; }

        public string Category { get; set; }

        /// <summary>
        /// 文章关联的标签Json格式数据
        /// </summary>
        [MaxLength(500)]
        public string CategoryTags { get; set; }

        /// <summary>
        /// 当前文章是否置顶
        /// </summary>
        public bool? IsTopMost { get; set; }

        /// <summary>
        /// 文章是否被用户收藏(已登录用户)
        /// </summary>
        [NotMapped]
        public bool IsFavorite { get; set; }

        /// <summary>
        /// 文章类型，新后台的文章类型为5
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 文章和标签Many To Many集合
        /// </summary>
        [NotMapped]
        public virtual ICollection<ArticleCategory> ArticleCategories { get; set; }

        /// <summary>
        /// 文章的相关文章
        /// </summary>
        [NotMapped]
        public ICollection<RelatedArticle> RelatedArticles { get; set; }

        /// <summary>
        /// 自定义标签，当前仅显示置顶状态的文本
        /// </summary>
        [NotMapped]
        public CustomCategoryMode CustomeTag { get { return (IsTopMost.HasValue && IsTopMost.Value == true) ? CustomCategoryMode.TopMost : CustomCategoryMode.NoCustom; } }

        /// <summary>
        /// 文章描述(废弃)
        /// </summary>
        [NotMapped]
        public string Description{ get; set; }
    }


    /// <summary>
    /// 相关文章Model
    /// </summary>
    public class RelatedArticle
    {
        /// <summary>
        /// 文章Id
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 文章封面标签(发表者)
        /// </summary>
        public string CoverTag { get; set; }

        /// <summary>
        /// 文章列表(封面)图片
        /// </summary>
        public string CoverImage { get; set; }

        public DateTime PublishDateTime { get; set; }
    }

    public class ArticleTemp
    {
        public int PKID { get; set; }
        public string SmallTitle { get; set; }
        public string BigTitle { get; set; }
    }

    [ElasticsearchType(IdProperty = "id", Name = "article")]
    public class ArticleES
    {
        [String(Index = FieldIndexOption.NotAnalyzed)]
        public int id { get; set; }
        // <summary>Ik_smart分词的Keyword</summary>
        [String(Analyzer = "ik_smart", SearchAnalyzer = "ik_smart")]
        public string title { get; set; }

        public string body { get; set; }

        public string created_at { get; set; }

        public string updated_at { get; set; }
        public string reply_count { get; set; }
        public string view_count { get; set; }
        public string vote_count { get; set; }
    }
}
