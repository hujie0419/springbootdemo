using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Tuhu.WebSite.Component.Discovery.BusinessData;

namespace Tuhu.WebSite.Web.Activity.Models
{
    /// <summary>
    /// 页面模型
    /// </summary>
    public class ArticleViewModel
    {

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 调用信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage { get; set; }
        /// <summary>
        /// 参数是否包含title
        /// </summary>
        public bool hasArticleTitle { get; set; }
        /// <summary>
        /// 作者信息
        /// </summary>
        public AuthorInfo author { get; set; }
        /// <summary>
        /// 文章和专题列表
        /// </summary>
        public List<object> data { get; set; }
    }

    /// <summary>
    /// 标签
    /// </summary>
    public class TagItem
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        public string TagName { get; set; }
        /// <summary>
        /// 标签Id
        /// </summary>
        public string TagId { get; set; }
    }
    /// <summary>
    /// 作者信息
    /// </summary>
    public class AuthorInfo
    {
        public string AuthorName { get; set; }

        public string AuthorHead { get; set; }

        public string Description { get; set; }
    }
    /// <summary>
    /// 文章
    /// </summary>
    public class ArticleItem
    {

        private bool IsDetail = true;
        /// <summary>
        /// 顺序
        /// </summary>
        public long Index { get; set; }
        /// <summary>
        /// 文章ID
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 文章描述
        /// </summary>
        public string ContentDes { get; set; }
        /// <summary>
        /// 老文章链接
        /// </summary>
        public string ContentUrl { get; set; }
        /// <summary>
        /// 文章类型
        /// </summary>
        public int Type { get; set; }
        private string _contentHtml;
        /// <summary>
        /// 文章内容
        /// </summary>
        public string ContentHtml
        {
            get
            {
                if (this.IsDetail)
                {
                    return _contentHtml;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this._contentHtml = value;
            }
        }
        /// <summary>
        /// 文章封面模式{0：无图模式}{1：单图小图模式}{2：单图大图模式} {3：三图模式}
        /// </summary>
        public int CoverMode { get; set; }
        /// <summary>
        /// 文章封面模式描述  {0：无图模式}{1：单图小图模式}{2：单图大图模式} {3：三图模式}
        /// </summary>
        public string CoverModeDes { get; set; }
        /// <summary>
        /// 文章封面标签
        /// </summary>
        public string CoverTag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 文章发表时间
        /// </summary>
        public DateTime PublishDateTime { get; set; }
        public string PublishDateTimeDes { get; set; }
        /// <summary>
        /// 文章封面图片
        /// </summary>
        public string CoverImage { get; set; }

        public string[] CoverImage_Array {
            get {
                return this.CoverImage.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        /// <summary>
        /// 所属标签
        /// </summary>
        public string FromTag { get; set; }
        /// <summary>
        /// 所属标签
        /// </summary>
        public int? FromTagId { get; set; }
        /// <summary>
        /// 阅读数
        /// </summary>
        public int ReadCount { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 喜欢数
        /// </summary>
        public int StarCount { get; set; }
        /// <summary>
        /// 自定义标签
        /// </summary>
        public int CustomTag { get; set; }
        /// <summary>
        /// 自定义标签名称
        /// </summary>
        public string CustomTagDes { get; set; }
        /// <summary>
        /// 标签列表
        /// </summary>
        private List<TagItem> _tags { get; set; }
        public List<TagItem> Tags
        {
            get
            {
                if (this.IsDetail)
                {
                    return _tags;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this._tags = value;
            }
        }
        public CoverAuthorModel AuthorInfo { get; set; }
        /// <summary>
        /// 相关文章
        /// </summary>
        private List<RelatedArticle> _relatedArticles { get; set; }
        public List<RelatedArticle> RelatedArticles
        {
            get
            {
                if (this.IsDetail)
                {
                    return _relatedArticles;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this._relatedArticles = value;
            }
        }
        public static string GetImage(Article item)
        {
            string image = string.Empty;
            if (item.Type != 5)
            {
                if (!string.IsNullOrEmpty(item.ShowImages))
                {
                    var imgs = JsonConvert.DeserializeObject<List<JObject>>(item.ShowImages).Select(x => x.Value<string>("BImage")).ToList();
                    if (imgs.Count > 0)
                    {
                        item.CoverMode = Tuhu.WebSite.Component.Discovery.BusinessData.CoverMode.TopBigPicMode.ToString();
                        image = ";" + imgs[0];
                    }
                    else
                    {
                        item.CoverMode = Tuhu.WebSite.Component.Discovery.BusinessData.CoverMode.NoPicMode.ToString();
                    }
                }
                else
                {
                    item.CoverMode = Tuhu.WebSite.Component.Discovery.BusinessData.CoverMode.NoPicMode.ToString();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(item.CoverImage))
                {
                    string[] imgs = item.CoverImage.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (imgs.Length > 0)
                    {
                        if ((item.CoverMode == Tuhu.WebSite.Component.Discovery.BusinessData.CoverMode.ThreePicMode.ToString()
                        || item.CoverMode == Tuhu.WebSite.Component.Discovery.BusinessData.CoverMode.BigPicLeftMode.ToString())
                        && imgs.Length != 3)
                        {
                            item.CoverMode = Tuhu.WebSite.Component.Discovery.BusinessData.CoverMode.TopBigPicMode.ToString();
                            image = ";" + imgs[0];
                        }
                        else
                        {
                            image = item.CoverImage.StartsWith(";") ? item.CoverImage : (";" + item.CoverImage);
                        }
                    }
                    else
                    {
                        item.CoverMode = Tuhu.WebSite.Component.Discovery.BusinessData.CoverMode.NoPicMode.ToString();
                    }
                }
            }
            if (string.IsNullOrEmpty(image))
            {
                item.CoverMode = Tuhu.WebSite.Component.Discovery.BusinessData.CoverMode.NoPicMode.ToString();
            }
            return image;
        }
        #region 页面模型和数据模型转换
        public static explicit operator ArticleItem(Article item)
        {
            if (item == null)
            {
                return null;
            }
            ArticleItem art = new ArticleItem();
            art.ContentHtml = item.Content;
            art.CoverImage = GetImage(item);
            art.CoverMode = (int)System.Enum.Parse(typeof(CoverMode), item.CoverMode);
            art.CoverTag = item.CoverTag;
            art.Type = item.Type;
            art.ContentUrl = item.ContentUrl;
            art.Id = item.PKID;
            art.ContentDes = item.Type == 5 ? item.Description : item.Brief;
            art.StarCount = item.Vote.HasValue ? item.Vote.Value : 0;
            art.CommentCount = item.CommentCountNum.HasValue ? item.CommentCountNum.Value : 0;
            art.ReadCount = item.ClickCount.HasValue ? item.ClickCount.Value : 0;
            if (!string.IsNullOrEmpty(item.CategoryTags))
            {
                var tags = JsonConvert.DeserializeObject<List<JObject>>(item.CategoryTags).Where(x => x.Value<string>("isShow") == "1").
                    Select(x => new Category() { Id = x.Value<int>("key"), Name = x.Value<string>("value") }).ToList();
                art.FromTag = tags.Count > 0 ? tags.First().Name : null;
                art.FromTagId = tags.Count > 0 ? (int?)tags.First().Id : null;
                if (art.FromTagId == null && art.CoverMode == 4)
                {
                    if (!string.IsNullOrEmpty(art.CoverImage))
                    {
                        art.CoverMode = 2;
                    }
                    else
                    {
                        art.CoverMode = 0;
                    }
                }
            }
            art.CoverModeDes = item.CoverMode;
            art.CustomTag = (int)item.CustomeTag;
            art.CustomTagDes = item.CustomeTag.ToDesString();
            art.PublishDateTime = item.PublishDateTime.HasValue ? item.PublishDateTime.Value : DateTime.Now;
            art.Description = GetDescription(art);
            art.Title = item.SmallTitle;
            art.PublishDateTimeDes = item.PublishDateTime.HasValue ? Tools.ShowTime(item.PublishDateTime.Value) : "";
            return art;
        }
        public static explicit operator ArticleItem(RelatedArticle item)
        {
            if (item == null)
            {
                return null;
            }
            ArticleItem art = new ArticleItem();
            art.CoverImage = item.CoverImage;
            art.CoverTag = item.CoverTag;
            art.Id = item.PKID;
            art.Title = item.Title;
            return art;
        }
        public static string  GetDescription(ArticleItem item)
        {
            StringBuilder sbd = new StringBuilder();
            //if (!string.IsNullOrEmpty(item.CoverTag))
            //{
            //    sbd.Append(item.CoverTag).Append(" · ");
            //}
            if (item.ReadCount > 0)
            {
                sbd.Append(item.ReadCount).Append("阅读").Append(" · ");
            }
            if (item.StarCount > 0)
            {
                sbd.Append(item.StarCount).Append("喜欢").Append(" · ");
            }
            if (item.CommentCount > 0)
            {
                sbd.Append(item.CommentCount).Append("评论").Append(" · ");
            }
            if (sbd.Length > 0 && !sbd.ToString().EndsWith(" · "))
            {
                sbd.Append(" · ");
            }
            sbd.Append(Tools.ShowTime(item.PublishDateTime));
            return sbd.ToString();
        }
        public static List<ArticleItem> ConvertToArtcleVM(List<Article> item)
        {
            if (item == null)
            {
                return new List<ArticleItem>();
            }
            List<ArticleItem> list = new List<ArticleItem>();
            long index = 1;
            item.ForEach(m =>
            {
                var art = (ArticleItem)m;
                art.IsDetail = false;
                art.Index = index;
                list.Add(art);
                index++;
            });
            return list;
        }
        public static List<ArticleItem> ConvertToRelatedVM(List<RelatedArticle> item)
        {
            if (item == null)
            {
                return null;
            }
            List<ArticleItem> list = new List<ArticleItem>();
            item.ForEach(m =>
            {
                list.Add((ArticleItem)m);
            });
            return list;
        }
        #endregion
    }
    /// <summary>
    /// 文章详细页面
    /// </summary>
    public class ArticleDetailViewModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 调用信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 文章
        /// </summary>
        public ArticleItem data { get; set; }
    }
    /// <summary>
    /// 相关阅读
    /// </summary>
    public class RelatedArticleViewModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 调用信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage { get; set; }
        /// <summary>
        /// 文章列表
        /// </summary>
        public List<ArticleItem> data { get; set; }
    }
    /// <summary>
    /// 收藏
    /// </summary>
    public class FavoriteViewModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 调用信息
        /// </summary>
        public string msg { get; set; }
    }

    /// <summary>
    /// 评论
    /// </summary>
    public class CommentViewModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 调用信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 最新评论
        /// </summary>
        public List<CommentItem> news { get; set; }
        /// <summary>
        /// 最热评论
        /// </summary>
        public List<CommentItem> hots { get; set; }
        /// <summary>
        /// 对应文章信息
        /// </summary>
        public ArticleItem article { get; set; }


    }
    /// <summary>
    /// 公用方法
    /// </summary>
    public class Tools
    {
        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static long ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }

        public static string ConvertPhoneSecret(string phone)
        {
            var reg = new Regex("^((13[0-9])|(15[^4,\\D])|(18[0,5-9]))\\d{8}$");
            if (reg.IsMatch(phone))
            {
                return reg.Replace(phone, "****", 4, 3);
            }
            return phone;
        }
        public static string ShowTime(DateTime publish)
        {
            DateTime now = DateTime.Now;
            TimeSpan ts = now.Subtract(publish);
            if (ts.Days > 0)
            {
                return ts.Days + "天前";
            }
            if (ts.Hours> 0)
            {
                return ts.Hours + "小时前";
            }
            if (ts.Minutes > 0)
            {
                return ts.Minutes + "分钟前";
            }
            if (ts.Seconds > 0)
            {
                return ts.Seconds + "秒前";
            }
            return publish.ToString("yyyy/M/d HH:mm");
        }
    }
    public class ImageMode
    {
        public string BImage { get; set; }
        public string Simage { get; set; }
    }
    /// <summary>
    /// 评论信息
    /// </summary>
    public class CommentItem
    {
        /// <summary>
        /// 评论ID
        /// </summary>
        public int CommentId { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        public string CommentContent { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int AgreeCount { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string CellPhone { get; set; }
        /// <summary>
        /// 用户等级
        /// </summary>
        public string UserGrade { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeadImage { get; set; }
        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime CommentDatetime { get; set; }
        /// <summary>
        /// 回复评论
        /// </summary>
        public CommentItem ReplyComment { get; set; }

        /// <summary>
        /// 当前用户是否点赞
        /// </summary>
        public bool IsLike { get; set; }

        /// <summary>
        /// 对象转换
        /// </summary>
        /// <param name="item"></param>
        public static explicit operator CommentItem(Comment item)
        {
            if (item == null)
            {
                return null;
            }
            CommentItem com = new CommentItem();
            com.CommentId = item.Id;
            com.AgreeCount = item.Praise.HasValue ? item.Praise.Value : 0;
            com.CommentContent = item.CommentContent;
            com.CommentDatetime = item.CommentTime;
            com.UserId = new Guid(item.UserId);
            com.IsLike = item.IsLike;
            //com.CommentCount = item.count;
            if (item.ReplyComment != null)
            {
                CommentItem reply = new CommentItem();
                reply.CommentId = item.ReplyComment.Id;
                reply.UserId =new Guid( item.UserId);
                reply.AgreeCount = item.ReplyComment.Praise.Value;
                reply.CommentContent = item.ReplyComment.CommentContent;
                reply.CommentDatetime = item.ReplyComment.CommentTime;
                reply.CommentCount =0;
                reply.IsLike = item.ReplyComment.IsLike;
                com.ReplyComment = reply;
            }
            return com;
        }
        /// <summary>
        /// 模型转换
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<List<CommentItem>> ConvertToCommentVM(List<Comment> item)
        {
            if (item == null)
            {
                return new List<CommentItem>();
            }
            using (var userClient = new Tuhu.Service.UserAccount.UserAccountClient())
            {
                List<CommentItem> list = new List<CommentItem>();
                string defaultImg = "http://resource.tuhu.cn/Image/Product/zhilaohu.png";
                foreach (var m in item)
                {
                    var cm = (CommentItem)m;
                    var u = await userClient.GetUserByIdAsync(Guid.Parse(m.UserId));
                    if (u.Result != null)
                    {
                        cm.UserId = u.Result.UserId;
                        cm.NickName = Tools.ConvertPhoneSecret(u.Result.Profile.NickName) == u.Result.MobileNumber ?
                                            u.Result.MobileNumber.Replace(u.Result.MobileNumber.Substring(3, 4), "****")
                                            : Tools.ConvertPhoneSecret(u.Result.Profile.NickName);
                        cm.UserGrade = "";
                        cm.CellPhone = Tools.ConvertPhoneSecret(u.Result.MobileNumber);
                        cm.HeadImage = !string.IsNullOrEmpty(u.Result.Profile.HeadUrl) ? u.Result.Profile.HeadUrl : defaultImg;
                    }
                    else
                    {
                        cm.UserId = Guid.Parse(m.UserId);
                        cm.NickName = "游客";
                        cm.HeadImage = defaultImg;
                        //continue;
                    }
                    if (cm.ReplyComment != null)
                    {
                        var us = await userClient.GetUserByIdAsync(Guid.Parse(m.ReplyComment.UserId));
                        if (us.Result != null)
                        {
                            cm.ReplyComment.UserId = us.Result.UserId;
                            cm.ReplyComment.NickName = Tools.ConvertPhoneSecret(us.Result.Profile.NickName) == us.Result.MobileNumber ?
                                                                            us.Result.MobileNumber.Replace(us.Result.MobileNumber.Substring(3, 4), "****")
                                                                            : Tools.ConvertPhoneSecret(us.Result.Profile.NickName);
                            cm.ReplyComment.UserGrade = "";
                            cm.ReplyComment.CellPhone = Tools.ConvertPhoneSecret(us.Result.MobileNumber);
                            cm.ReplyComment.HeadImage = !string.IsNullOrEmpty(us.Result.Profile.HeadUrl) ? us.Result.Profile.HeadUrl : defaultImg;
                        }
                    }
                    list.Add(cm);
                }
                return list;
            }
        }

    }

}