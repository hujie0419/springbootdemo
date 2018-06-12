using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.MessageQueue;
using Tuhu.Nosql;
using Tuhu.WebSite.Component.Activity.Business;
using Tuhu.WebSite.Component.Activity.BusinessData;
using Tuhu.WebSite.Component.SystemFramework;
using Tuhu.WebSite.Component.SystemFramework.Log;
using Tuhu.WebSite.Component.SystemFramework.Models;
using Tuhu.WebSite.Component.SystemFramework.Security;
using Tuhu.WebSite.Web.Activity.Common;

namespace Tuhu.WebSite.Web.Activity.Controllers
{
    public partial class DiscoveryController : Controller
    {

        /// <summary>
        /// 根据车型Id查询车型标签和品牌标签
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SelectVehicleAndBrandCategoryTag(string vehicleId, string vehicleName)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据关键字查询标签列表
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Obsolete]
        public ActionResult SearchCategoryTagByKeyWord(string keyWord, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新增包含多个标签的问题
        /// </summary>
        /// <param name="question"></param>
        /// <param name="createInfo"></param>
        /// <returns></returns>
        public ActionResult AddQuestionWithMultipleCategory(string userId, string questionContent, string categoryTags, string commentImage,
                                                                                                    string userHead, string userGrade, string userName, string realName, string sex, string categoryIds)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// App首页瀑布流文章
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectArticleForAppHomePage(int pageIndex = 1, int pageSize = 30)
        {
            var dic = new Dictionary<string, object>();
            try
            {

                var versionNumber = Request.Headers.Get("VersionCode");
                var version = Request.Headers.Get("version");

                var pager = new PagerModel { CurrentPage = pageIndex, PageSize = pageSize };
                List<DiscoveryModel> articles = null;
                using (var reclient = CacheHelper.CreateCacheClient("SelectArticleForAppHomePage"))
                {
                    var result = await reclient.GetOrSetAsync("AppHomePage/" + pageIndex+"/"+pageSize,()=>DiscoverBLL.SelectArticleForAppHomePage(pager),TimeSpan.FromHours(2));
                    if (result.Value != null)
                        articles = result.Value;
                    else
                        articles = await DiscoverBLL.SelectArticleForAppHomePage(pager);
                }
                pager.TotalItem = 100;
                dic.Add("Code", "1");
                dic.Add("TotalPage", pager.TotalPage);
                dic.Add("TotalCount", pager.TotalItem);
                var articleShowMode = await DistributedCacheHelper.SelectArticleShowMode();
                dic.Add("Articles", articles.Select(article => new
                {
                    ArticleId = article.PKID,
                    Image = JsonConvert.DeserializeObject<List<ShowImageModel>>(article.ShowImages).FirstOrDefault().BImage,
                    Title = article.SmallTitle,
                    ArticleShowMode = articleShowMode,
                    //ContentUrl = ((string.IsNullOrEmpty(versionNumber) == false && (string.Compare(versionNumber, "50") < 0) ||
                    //    (string.IsNullOrEmpty(version) == false && string.Compare(version, "iOS 3.4.5") < 0))) ? article.ContentUrl : DiscoverySite + "/Article/Detail?Id=" + article.RelatedArticleId,
                    //URL = ((string.IsNullOrEmpty(versionNumber) == false && (string.Compare(versionNumber, "50") < 0) ||
                    //    (string.IsNullOrEmpty(version) == false && string.Compare(version, "iOS 3.4.5") < 0))) ? article.ContentUrl : DiscoverySite + "/Article/Detail?Id=" + article.RelatedArticleId
                    ContentUrl = (articleShowMode == "New" || article.Type == 5) ? DomainConfig.FaXian + " /react/findDetail/?bgColor=%23ffffff&textColor=%23333333&id=" + article.PKID : article.ContentUrl,
                    URL = (articleShowMode == "New" || article.Type == 5) ? DomainConfig.FaXian + "/react/findDetail/?bgColor=%23ffffff&textColor=%23333333&id=" + article.PKID : article.ContentUrl
                }));

            }
            catch (Exception ex)
            {
                WebLog.LogException("App首页瀑布流文章", ex);
                dic.Clear();
                dic.Add("Code", "0");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询文章的相关信息(点赞数、评论数)
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="selectType">查询类型，默认为查询单条信息</param>
        /// <returns></returns>
        [CrossHost]
        public async Task<ActionResult> SelectArticleInfo(string articleId, string userId, int? selectType)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var articles = await DiscoverBLL.SelectArticleInfo(articleId, userId);
                var versionNumber = Request.Headers.Get("VersionCode");
                var version = Request.Headers.Get("version");

                var articleShowMode = await DistributedCacheHelper.SelectArticleShowMode();
                //默认返回单条信息
                if (selectType.HasValue == false)
                {
                    var articleInfo = articles.FirstOrDefault();
                    dic.Add("Article", new
                    {
                        PKID = articleInfo.PKID,
                        ArticleShowMode = articleShowMode,
                        //URL =
                        //((string.IsNullOrEmpty(versionNumber) == false && (string.Compare(versionNumber, "50") < 0) ||
                        //(string.IsNullOrEmpty(version) == false && string.Compare(version, "iOS 3.4.5") < 0))) ? articleInfo.ContentUrl : DiscoverySite + "/Article/Detail?Id=" + articleInfo.RelatedArticleId,
                        URL = (articleShowMode == "New" || articleInfo.Type == 5) ? DomainConfig.FaXian + "/react/findDetail/?bgColor=%23ffffff&textColor=%23333333&id=" + articleId : articleInfo.ContentUrl,
                        Title = articleInfo.BigTitle,
                        CommentCount = articleInfo.CommentCount,
                        VoteCount = articleInfo.VoteCount,
                        Brief = articleInfo.Brief,
                        SmallTitle = articleInfo.SmallTitle,
                        SmallImage = articleInfo.SmallImage,
                        VoteState = articleInfo.VoteState,
                        PublishDateTime = articleInfo.PublishDateTime.ToShortDateString(),
                        ClickCount = articleInfo.ClickCount,
                        Source = articleInfo.Source
                    });
                }
                else
                {
                    dic.Add("Article", articles.Select(articleInfo => new
                    {
                        PKID = articleInfo.PKID,
                        URL = articleInfo.ContentUrl,
                        Title = articleInfo.BigTitle,
                        CommentCount = articleInfo.CommentCount,
                        VoteCount = articleInfo.VoteCount,
                        Brief = articleInfo.Brief,
                        SmallTitle = articleInfo.SmallTitle,
                        SmallImage = articleInfo.SmallImage,
                        VoteState = articleInfo.VoteState,
                        PublishDateTime = articleInfo.PublishDateTime.ToShortDateString(),
                        ClickCount = articleInfo.ClickCount,
                        Source = articleInfo.Source
                    }));
                }
                dic.Add("Code", "1");


            }
            catch (Exception ex)
            {
                WebLog.LogException("查询文章的相关信息(点赞数、评论数)", ex);
                dic.Clear();
                dic.Add("Code", "0");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据用户Id和文章Id(问题Id)获得关注人列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public ActionResult SelectAttentionUsersByArticleId(string userId, int articleId)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据用户Id和回答Id获得关注人列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="answerId"></param>
        /// <returns></returns>
        public ActionResult SelectAttentionUsersByAnswerId(string userId, int answerId)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 新版发现菜单栏
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetMenuBar(string userId)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "1" };
            #region 推荐菜单栏
            var recommend = new List<DiscoveryMenuModel>() {
                new DiscoveryMenuModel()
                {
                    MenuName = "热帖",
                    DirectUrl = "",
                    AppKey = "Recommend"
                },
                new DiscoveryMenuModel()
                {
                    MenuName = "头条",
                    DirectUrl = DomainConfig.FaXian + "/react/find_new/#/?_k=tycmkl",
                    AppKey = "Headline"
                },
                new DiscoveryMenuModel()
                {
                    MenuName = "众测",
                    DirectUrl = DomainConfig.FaXian + "/firstline?ArticleTagId=11867",
                    AppKey = "ArticleCategory"
                },
                new DiscoveryMenuModel()
                {
                    MenuName = "赛车",
                    DirectUrl = "https://m.hupu.com/bbs/118",
                    AppKey = "HuPu"
                }
            };

            if (!string.IsNullOrWhiteSpace(userId) && Guid.TryParse(userId, out Guid uid))
            {
                var myAttentionCategory = await DiscoverBLL.SelectAttentionCategorysForToolBar(uid.ToString("B"));

                if (myAttentionCategory != null && myAttentionCategory.Any())
                    recommend.AddRange(myAttentionCategory.Select(category => new DiscoveryMenuModel
                    {
                        MenuName = category.SmallTitle,
                        DirectUrl = $"{DomainConfig.FaXian}/firstline?ArticleTagId={category.PKID}",
                        AppKey = "ArticleCategory",
                        CategoryId = category.PKID
                    }));
            }
            dic["Recommend"] = recommend;
            #endregion

            #region 优选菜单栏
            var preferred = new List<DiscoveryMenuModel>() {
                new DiscoveryMenuModel()
                {
                    MenuName = "我的喜欢",
                    DirectUrl = "https://wx.tuhu.cn/vue/preference",
                    AppKey = ""
                },new DiscoveryMenuModel()
                {
                    MenuName = "推荐",
                    DirectUrl = "https://wx.tuhu.cn/vue/preference?categoryTagId=100001",
                    AppKey = ""
                },new DiscoveryMenuModel()
                {
                    MenuName = "轮胎",
                    DirectUrl = "https://wx.tuhu.cn/vue/preference?categoryTagId=100002",
                    AppKey = ""
                },new DiscoveryMenuModel()
                {
                    MenuName = "保养",
                    DirectUrl = "https://wx.tuhu.cn/vue/preference?categoryTagId=100003",
                    AppKey = ""
                },new DiscoveryMenuModel()
                {
                    MenuName = "美容",
                    DirectUrl = "https://wx.tuhu.cn/vue/preference?categoryTagId=100004",
                    AppKey = ""
                },new DiscoveryMenuModel()
                {
                    MenuName = "车品",
                    DirectUrl = "https://wx.tuhu.cn/vue/preference?categoryTagId=100005",
                    AppKey = ""
                },new DiscoveryMenuModel()
                {
                    MenuName = "改装",
                    DirectUrl = "https://wx.tuhu.cn/vue/preference?categoryTagId=100006",
                    AppKey = ""
                }
            };
            dic["Preferred"] = preferred;
            #endregion

            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 查询发现菜单
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> SelectDiscoveryMenuBar(string userId = null, int articleTagId = 6)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                Version versionNumber = null;
                int version = 1;
                var v = Request.Headers.Get("VersionNumber");
                if (!string.IsNullOrWhiteSpace(v))
                {
                    versionNumber = new Version(v);
                }

                var v1 = Request.Headers.Get("version");
                if (!string.IsNullOrWhiteSpace(v1))
                {
                    versionNumber = new Version(v1.Replace("iOS ", ""));
                }
                if (versionNumber != null && versionNumber >= new Version("5.0.3")&& versionNumber < new Version("5.0.21"))
                {
                    version = 2;
                }else if(versionNumber != null && versionNumber >= new Version("5.0.21"))
                {
                    version = 3;
                }
                

                var host = HttpContext.Request.Url.Host;
                var menuList = new List<DiscoveryMenuModel>();
                //menuList.Add(new DiscoveryMenuModel() { MenuName = "头条", DirectUrl = "", AppKey = "Headline" });
                if (version == 3)
                    menuList.Add(new DiscoveryMenuModel() { MenuName = "热帖", DirectUrl = "", AppKey = "Recommend" });

                menuList.Add(new DiscoveryMenuModel() { MenuName = "头条", DirectUrl = DomainConfig.FaXian + "/react/find_new/#/?_k=tycmkl", AppKey = "Headline" });
                
                //menuList.Add(new DiscoveryMenuModel() { MenuName = "关注", DirectUrl = "", AppKey = "Follow" });

                //if (version > 1)
                //menuList.Add(new DiscoveryMenuModel() {MenuName = "晒图", DirectUrl = "", AppKey = "ShareImages"});
                //menuList.Add(new DiscoveryMenuModel() { MenuName = "问答", DirectUrl = "", AppKey = "QAndA" });
                //menuList.Add(new DiscoveryMenuModel() { MenuName = "版块", DirectUrl = "", AppKey = "Board" });

                //menuList.Add(new DiscoveryMenuModel() { MenuName = "头条", DirectUrl = "http://www.baidu.com", AppKey = "Headline" });SelectDiscoveryMenuBar
                //menuList.Add(new DiscoveryMenuModel() { MenuName = "关注", DirectUrl = "http://www.163.com", AppKey = "Follow" });
                //menuList.Add(new DiscoveryMenuModel() { MenuName = "问答", DirectUrl = "http://www.sina.com", AppKey = "QAndA" });

                //menuList.Add(new DiscoveryMenuModel() { MenuName = "版块", DirectUrl = string.Format("http://{0}/firstline?ArticleTagId={1}", host, articleTagId), AppKey = "Board" });


                menuList.Add(new DiscoveryMenuModel() { MenuName = "众测", DirectUrl = DomainConfig.FaXian + "/firstline?ArticleTagId=11867", AppKey = "ArticleCategory" });

                menuList.Add(new DiscoveryMenuModel() { MenuName = "赛车", DirectUrl = "https://m.hupu.com/bbs/118", AppKey = "HuPu" });

                if (string.IsNullOrEmpty(userId) == false)
                {
                    var myAttentionCategory = await DiscoverBLL.SelectAttentionCategorysForToolBar(new Guid(userId).ToString("B"));
                    menuList.AddRange(myAttentionCategory.Select(category => new DiscoveryMenuModel
                    {

                        MenuName = category.SmallTitle,
                        //DirectUrl = DiscoverySite+ string.Format("/article/GetArticlelist?ArticleTagId={0}",category.RelatedCategoryId),
                        //DirectUrl = string.Format("http://172.16.22.53:12316/firstline?ArticleTagId={0}", category.RelatedCategoryId),
                        DirectUrl = string.Format(DomainConfig.FaXian+"/firstline?ArticleTagId={1}", host, category.PKID),
                        AppKey = "ArticleCategory",
                        CategoryId = category.PKID
                    }));
                }

                dic.Add("Code", "1");
                dic.Add("Menus", menuList);
            }
            catch (Exception ex)
            {
                WebLog.LogException("查询发现菜单", ex);
                dic.Add("Code", "0");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        #region MyRegion
        /// <summary>
        /// 同步旧文章到新库
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public async Task<ActionResult> SyncArticleData(int pageIndex = 1, int pageSize = 10)
        {

            try
            {
                var articles = await DiscoverBLL.SyncArticleData();
                articles.ForEach(article =>
                {
                    if (string.IsNullOrEmpty(article.ShowImages) == false)
                    {
                        if (article.ShowType == 1 || article.ShowType == 2)
                        {
                            article.ShowImages = ";" + JsonConvert.DeserializeObject<List<ArticleImage>>(article.ShowImages).FirstOrDefault().BImage;
                        }
                        else if (article.ShowType == 3)
                        {
                            var showImages = JsonConvert.DeserializeObject<List<ArticleImage>>(article.ShowImages);
                            article.ShowType = showImages.Count == 3 ? 3 : 1;
                            article.ShowImages = showImages.Count == 3 ? ";" + string.Join(";", showImages.Select(a => a.BImage)) : ";" + showImages.FirstOrDefault().BImage;
                        }
                    }
                    else
                        article.ShowType = 0;

                    //旧版本发现的新文章
                    if (article.Type == 1 && ((article.Content.StartsWith("{") && article.Content.EndsWith("}")) || (article.Content.StartsWith("[") && article.Content.EndsWith("]"))))
                    {
                        var articleSourceContents = JsonConvert.DeserializeObject<List<ArticleContentModel>>(article.Content);
                        for (int i = 0; i < articleSourceContents.Count; i++)
                        {
                            var content = articleSourceContents[i];
                            content.Content = @"<forcode>
                                                                                    <div style='padding: 20px 0 0 0;position: relative;'>
                                                                                        <div style='color: #333;margin-bottom: 18px;font-size: 18px;line-height: 26px;padding-left: 26px;position: relative;'>
                                                                                            <span style='position:absolute;width:21px;height:21px;background-color:#e03548;border-radius:50%;color:#fff;
                                                                                                                                    font-size:16px;text-align:center;line-height:22px;overflow:hidden;top:0;left:0'>@Num</span>@Title
                                                                                        </div>
                                                                                        <div style='color:#363636;font-size:16px;line-height:28px;-webkit-box-pack:justify;-moz-box-pack:justify;
                                                                                                                              justify-content:space-between;-webkit-justify-content:space-between;text-align:justify;margin-bottom: 15px;'>
                                                                                        @Content         
                                                                                        </div>
                                                                                          @Product
                                                                                    </div >
                                                                                </forcode>";
                            content.Content = content.Content.Replace("@Num", (i + 1).ToString()).Replace("@Title", content.Title).Replace("@Content", WebUtility.HtmlDecode(content.Describe.Replace(@"\", "")));
                            //存在商品信息
                            if (string.IsNullOrEmpty(content.ProductName) == false && string.IsNullOrEmpty(content.ProductPrice) == false && string.IsNullOrEmpty(content.ProductUrl) == false)
                            {
                                var productInfo = string.Format(@"<div style='position:relative;padding:15px;margin:20px 0;'>
                                                                                                    <i style='position:absolute;width:15px;height:15px;border-style:solid;border-color:#d9d9d9;border-width:2px 0 0 2px;top:0;left:0;'></i><i style='position:absolute;width:15px;height:15px;border-style:solid;border-color:#d9d9d9;border-width:0 2px 2px 0;right:0;bottom:0;'></i>
                                                                                                    <div style='border:1px solid #eee;'>
                                                                                                        <img src='@ProductPic' style='display:block;width:100%;' class='cover'>
                                                                                                        <div style='width:94.6%;clear:both;display:table;background-color:#fafafa;padding:10px 15px;'>
                                                                                                            <p style='color:#666;font-size:14px;float:left;'>
                                                                                                               @ProductName<br>
                                                                                                                <span style='color:#df3448;font-size:16px;'>￥@Price</span>
                                                                                                            </p>
                                                                                                            <a href=""@ProductUrl"" style='float:right;width:75px;height:24px;border:1px solid #e74c3c;border-radius:5px;font-size:12px;color:#e74c3c;text-align:center;line-height:24px;margin-top:3px;text-decoration:none;'>查看详情</a>
                                                                                                        </div>
                                                                                                    </div>
                                                                                                </div>");
                                productInfo = productInfo.Replace("@ProductPic", content.MaxPic).Replace("@ProductName", content.ProductName)
                                                                          .Replace("@Price", content.ProductPrice).Replace("@ProductUrl", content.ProductUrl);
                                content.Content = content.Content.Replace("@Product", productInfo);
                            }
                            else
                            {
                                content.Content = content.Content.Replace("@Product", "");
                            }
                        }

                        //拼接新文章的内容为字符串
                        article.Content = "<div  id='content'>" + string.Join("", articleSourceContents.Select(articleSource => articleSource.Content).ToArray()).Replace("\r\n", "").Replace(@"\", "") + "</div>";
                        DiscoverBLL.SyncArticleContentToHtml(article.PKID, article.Content);
                    }
                });

                //return Json(articles.Select(article => new
                //{
                //    RelatedArticleId = article.PKID,
                //    Title = article.SmallTitle,
                //    Status = article.PublishDateTime <= DateTime.Now ? "Published" : "PrePublish",
                //    CoverMode = (article.ShowType == 1 || article.ShowType == 2) ? "OnePicBigMode" : (article.ShowType == 3 ? "ThreePicMode" : "NoPicMode"),
                //    CoverImage = article.ShowType < 1 ? string.Empty : article.ShowImages,
                //    CreateDateTime = article.CreateDateTime,
                //    PublishDateTime = article.PublishDateTime,
                //    ContentHtml = article.Content,
                //    ReadCount = article.ReadCountNum,
                //    StarCount = article.LikeCountNum,
                //    CommentCount = article.AnswerNumber

                //}), JsonRequestBehavior.AllowGet);
                return Content("同步Content成功");
            }
            catch (Exception ex)
            {
                WebLog.LogException("同步旧文章到新库", ex);
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        /// <summary>
        /// 取消收藏文章
        /// </summary>
        /// <param name="articleId">文章Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [CrossHost]
        public ActionResult RemoveFavoriteArticle(int articleId, string userId)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 新增收藏文章记录
        /// </summary>
        /// <param name="articleId">文章Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [CrossHost]
        public async Task<ActionResult> InsertFavoriteArticle(int articleId, string userId)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 查询问答板块中我关注的标签和问题列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetAttentionQuestionForQAndAHomePage(string userId, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                //查询热门问答
                if (string.IsNullOrWhiteSpace(userId))
                {
                    await GetPopularQAndAForQuestionBrand(pageIndex, pageSize, dic);
                }
                else
                {
                    var dataList = await DiscoverBLL.GetAttentionQuestionForQAndAHomePage(userId, pageIndex, pageSize);
                    if (dataList.Any() == false)
                    {
                        await GetPopularQAndAForQuestionBrand(pageIndex, pageSize, dic);
                    }
                    else
                    {
                        dic.Add("Code", "1");

                        var timeLineList = new List<object>();
                        foreach (var timeLine in dataList)
                        {
                            //if (timeLine.UserIdentity > 0)
                            //    continue;
                            Tuhu.Service.UserAccount.Models.User userInfo = null;
                            if (!string.IsNullOrWhiteSpace(timeLine.UserId))
                            {
                                Guid UserID;
                                if (Guid.TryParse(timeLine.UserId, out UserID))
                                {
                                    using (var userClient = new Tuhu.Service.UserAccount.UserAccountClient())
                                    {
                                        var userInfoResult = await userClient.GetUserByIdAsync(new Guid(timeLine.UserId));
                                        if (userInfoResult.Result != null)
                                        {
                                            userInfo = userInfoResult.Result;
                                        }
                                    }
                                }
                                else
                                {
                                    //WebLog.LogException(new Exception(JsonConvert.SerializeObject(timeLine)));
                                }
                            }

                            if (userInfo != null && userInfo.Profile != null)
                                userInfo.Profile.HeadUrl = userInfo.Profile.HeadUrl ?? string.Empty;

                            timeLineList.Add(new
                            {
                                PKID = timeLine.PKID,
                                Content = timeLine.Question,
                                FirstAttentionUserId = timeLine.UserId,
                                UserId = timeLine.UserId,
                                AnswerId = timeLine.AnswerId,
                                AnswerContent = timeLine.AnswerContent,
                                CommentImage = timeLine.CommentImage,

                                UserHead = userInfo == null ? (timeLine.UserIdentity > 0 ? timeLine.UserHead : GetDefaultUserHeadByUserGrade(string.Empty)) : (userInfo.Profile.HeadUrl.Contains("http") ? userInfo.Profile.HeadUrl : DomainConfig.ImageSite + userInfo.Profile.HeadUrl),
                                UserName = userInfo == null ? (timeLine.UserIdentity > 0 ? timeLine.UserName : "途虎用户") : (GetUserName(userInfo.Profile.NickName, userInfo.Profile.NickName, userInfo.MobileNumber)),
                                Type = timeLine.Type,
                                UserIdentity = timeLine.UserIdentity,
                                Praise = timeLine.Praise
                            });
                        }
                        dic.Add("Data", new { TimeLineList = timeLineList });
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException("GetAttentionQuestionForQAndAHomePage", ex);
                dic.Clear();
                dic.Add("Messages", ex.Message);
                dic.Add("Code", "0");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAttentionQuestionForQAndAHomePageNew(string userId, int pageIndex = 1,
            int pageSize = 10)
        {

            var dic = new Dictionary<string, object>();
            if (string.IsNullOrWhiteSpace(userId))
            {
                dic["Code"] = "0";
                dic["Message"] = "参数错误";
                return Json(dic, JsonRequestBehavior.AllowGet);
            }

            var dataList = await DiscoverBLL.GetAttentionQuestionForQAndAHomePage(userId, pageIndex, pageSize);

            
            if (!dataList.Any())
            {
                dic["Code"] = "1";
                dic["AttentionQuestion"] = new object[0];
                return Json(dic, JsonRequestBehavior.AllowGet);
            }

            var timeLineList = new List<object>();
            foreach (var timeLine in dataList)
            {
                //if (timeLine.UserIdentity > 0)
                //    continue;
                Tuhu.Service.Member.Models.UserObjectModel userInfo = null;
                if (!string.IsNullOrWhiteSpace(timeLine.UserId))
                {
                    Guid UserID;
                    if (Guid.TryParse(timeLine.UserId, out UserID))
                    {
                        userInfo = HttpClientHelper.SelectUserInfoByUserId(timeLine.UserId);
                    }
                }

                timeLineList.Add(new
                {
                    PKID = timeLine.PKID,
                    Content = timeLine.Question,
                    FirstAttentionUserId = timeLine.UserId,
                    UserId = timeLine.UserId,
                    AnswerId = timeLine.AnswerId,
                    AnswerContent = timeLine.AnswerContent,
                    CommentImage = timeLine.CommentImage,
                    UserHead =
                        userInfo?.HeadImage == null
                            ? (timeLine.UserIdentity > 0
                                ? timeLine.UserHead
                                : GetDefaultUserHeadByUserGrade(string.Empty))
                            : (userInfo.HeadImage.Contains("http")
                                ? userInfo.HeadImage
                                : DomainConfig.ImageSite + userInfo.HeadImage),
                    UserName =
                        userInfo == null
                            ? (timeLine.UserIdentity > 0 ? timeLine.UserName : "途虎用户")
                            : ArticleController.GetUserName(userInfo.Nickname),
                    Type = timeLine.Type,
                    UserIdentity = timeLine.UserIdentity,
                    Praise = timeLine.Praise,
                    CommentTimes = 0,
                    VoteNum = 0,
                    timeLine.QuestionStatus,
                    timeLine.Vehicle,
                    timeLine.VehicleTagId,
                    timeLine.PublishNewDateTime,
                    Image =
                        string.IsNullOrWhiteSpace(timeLine.Image)
                            ? new string[0]
                            : timeLine.Image.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries)?.Take(3)

                });
            }
            dic["Code"] = "1";
            dic.Add("AttentionQuestion",  timeLineList);

            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询问答板块中的热门问答
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        private async Task GetPopularQAndAForQuestionBrand(int pageIndex, int pageSize, Dictionary<string, object> dic)
        {
            var popularQuestion = await DistributedCacheHelper.SelectPopularAnswers(new PagerModel { CurrentPage = pageIndex, PageSize = pageSize });
            dic.Add("Code", "1");
           
            var timeLineList = new List<object>();
            if (popularQuestion?.Item2 != null)
                foreach (var question in popularQuestion.Item2)
                {
                    //if (question.UserIdentity > 0)
                    //    continue;

                    Tuhu.Service.Member.Models.UserObjectModel userInfo = null;
                    if (!(question.UserIdentity > 0))
                    {
                        userInfo=HttpClientHelper.SelectUserInfoByUserId(question.BestAnswererUserId);
                   
                    }
                    timeLineList.Add(new
                    {
                        PKID = question.PKID,
                        FirstAttentionUserId = question.BestAnswererUserId,
                        UserId = question.BestAnswererUserId,
                        Content = question.Content,
                        AnswerId = question.BestAnswerId,
                        AnswerContent = question.BestAnswerContent,
                        CommentImage = question.CommentImage,
                        UserHead = userInfo?.HeadImage == null ? (question.UserIdentity > 0 ? question.BestAnswererHead : GetDefaultUserHeadByUserGrade(string.Empty)) : (userInfo.HeadImage.Contains("http") ? userInfo.HeadImage : DomainConfig.ImageSite + userInfo.HeadImage),
                        UserName = userInfo == null ? (question.UserIdentity > 0 ? question.BestAnswerer : "途虎用户") : (ArticleController.GetUserName(userInfo.Nickname)),
                        Type = question.BestAnswerId > 0 ? 3 : 4,
                        UserIdentity = question.UserIdentity,
                        Praise = question.Praise

                    });
                }
            dic.Add("Data", new { TimeLineList = timeLineList });
        }

        /// <summary>
        /// 查询车型相关的问答
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectQuestionAboutCar(string vehicleId, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var dataList = await DiscoverBLL.SelectQuestionAboutCar(vehicleId, pageIndex, pageSize);
                dic.Add("Code", "1");

                var timeLineList = new List<object>();
                foreach (var question in dataList)
                {
                    //if (question.UserIdentity > 0)
                    //    continue;

                    Tuhu.Service.Member.Models.UserObjectModel userInfo = null;
                    if (!(question.UserIdentity > 0))
                    {
                        if (!string.IsNullOrWhiteSpace(question.UserId))
                        {
                            userInfo = HttpClientHelper.SelectUserInfoByUserId(question.UserId);
                        }
                    }


                    timeLineList.Add(new
                    {
                        question.PKID,
                        FirstAttentionUserId = question.UserId,
                        question.UserId,
                        Content = question.Question,
                        question.AnswerId,
                        question.AnswerContent,
                        question.CommentImage,
                        UserHead =
                            userInfo?.HeadImage == null
                                ? (question.UserIdentity > 0
                                    ? question.UserHead
                                    : GetDefaultUserHeadByUserGrade(string.Empty))
                                : (userInfo.HeadImage.Contains("http")
                                    ? userInfo.HeadImage
                                    : DomainConfig.ImageSite + userInfo.HeadImage),
                        UserName =
                            userInfo?.Nickname == null
                                ? (question.UserIdentity > 0 ? question.UserName : "途虎用户")
                                : (ArticleController.GetUserName(userInfo.Nickname)),
                        question.Type,
                        question.Praise,
                        question.UserIdentity,
                        question.QuestionStatus,
                        question.PublishNewDateTime,
                        Image =
                        string.IsNullOrWhiteSpace(question.Image)
                            ? new string[0]
                            : question.Image.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)?.Take(3)

                    });
                }
                dic.Add("Data", new { TimeLineList = timeLineList });
            }
            catch (Exception ex)
            {
                WebLog.LogException("GetMyQAndA", ex);
                dic.Clear();
                var innerException = ex.InnerException == null ? "无" : ex.InnerException.Message;
                dic.Add("Messages", ex.Message + innerException);
                dic.Add("Code", "0");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询问答板块中我的问答
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetMyQAndA(string userId, int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var dataList = await DiscoverBLL.GetMyQAndA(userId, pageIndex, pageSize);
                dic.Add("Code", "1");

                var timeLineList = new List<object>();
                foreach (var question in dataList)
                {

                    if (question.UserIdentity > 0)
                        continue;

                    Tuhu.Service.Member.Models.UserObjectModel userInfo = null;

                    if (!(question.UserIdentity > 0))
                    {
                        userInfo = HttpClientHelper.SelectUserInfoByUserId(userId);
                    }

                    timeLineList.Add(new
                    {
                        question.PKID,
                        FirstAttentionUserId = question.UserId,
                        question.UserId,
                        Content = question.Question,
                        question.AnswerId,
                        question.AnswerContent,
                        question.CommentImage,
                        UserHead =
                            userInfo?.HeadImage == null
                                ? (question.UserIdentity > 0
                                    ? question.UserHead
                                    : GetDefaultUserHeadByUserGrade(string.Empty))
                                : (userInfo.HeadImage.Contains("http")
                                    ? userInfo.HeadImage
                                    : DomainConfig.ImageSite + userInfo.HeadImage),
                        UserName =
                            userInfo?.Nickname == null
                                ? (question.UserIdentity > 0 ? question.UserName : "途虎用户")
                                : (ArticleController.GetUserName(userInfo.Nickname)),
                        question.Type,
                        Praise = question.PraiseCount,
                        question.UserIdentity,
                        question.QuestionStatus

                    });
                }
                dic.Add("Data", new { TimeLineList = timeLineList });
            }
            catch (Exception ex)
            {
                WebLog.LogException("GetMyQAndA", ex);
                dic.Clear();
                dic.Add("Messages", ex.Message);
                dic.Add("Code", "0");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询文章详情显示模式
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectArticleShowMode()
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Message"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

    }

    class ArticleContentModel
    {
        public string Title { get; set; }

        public string Content { get; set; }
        public string Describe { get; set; }
        public string MaxPic { get; set; }
        public string MinPic { get; set; }
        public string ProductName { get; set; }
        public string ProductPrice { get; set; }
        public string ProductUrl { get; set; }
    }

    class ArticleImage
    {
        public string BImage { get; set; }
        public string SImage { get; set; }
    }



}
