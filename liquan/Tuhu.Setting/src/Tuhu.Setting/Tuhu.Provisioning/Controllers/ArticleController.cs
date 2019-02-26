using Newtonsoft.Json;
using Qiniu.IO;
using Qiniu.RS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.FileUpload;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.CategoryTag;
using Tuhu.Provisioning.Business.Crm;
using Tuhu.Provisioning.DataAccess.Entity;
using swc = System.Web.Configuration;
using Newtonsoft.Json.Linq;
using Tuhu.Provisioning.Business.Logger;
using System.Net.Http;
using Tuhu.Service.Utility.Request;
using Tuhu.MessageQueue;
using Tuhu.Provisioning.Common;

namespace Tuhu.Provisioning.Controllers
{
    public class ArticleController : Controller
    {
        private static readonly Random rd = new Random();
        private readonly IArticleManager manager = new ArticleManager();
        public static CategoryTagManager _CategoryTagManager = new CategoryTagManager();

        private Lazy<ArticleManager> lazyArticleManager = new Lazy<ArticleManager>();
        private ArticleManager ArticleManagerLazy => this.lazyArticleManager.Value;
        private readonly IArticleCommentManager _ArticleCommentManager = new ArticleCommentManager();

        private static readonly Business.BlackListConfig.BlackListConfigManage _BlackListConfigManage = new Business.BlackListConfig.BlackListConfigManage();

        private readonly string ObjectType = "ArticleTbl";
        //[PowerManage]
        public ActionResult ArticleManage()
        {
            return View();
        }
        [HttpPost]
        public JsonResult ArticleManage(string KeyStr, int? PageIndex, int? type = -1)
        {
            int _PageSize = 20;
            var _List = manager.SelectBy(KeyStr, _PageSize, PageIndex, type);
            if (_List != null && _List.Count > 0)
            {
                return Json(new
                {
                    ArticleList = _List.Select(p => new
                    {
                        PKID = p.PKID,
                        SmallImage = p.SmallImage,
                        SmallTitle = p.SmallTitle,
                        BigTitle = p.BigTitle,
                        TitleColor = p.TitleColor ?? string.Empty,
                        Source = p.Source ?? string.Empty,
                        PublishDateTime = p.PublishDateTime.ToString("yyyy-MM-dd HH:mm:00"),
                        YesterdayClickCount = p.YesterdayClickCount,
                        //SkipLocalUrlCount = p.SkipLocalUrlCount,
                        //SkipTaoBaoUrlCount = p.SkipTaoBaoUrlCount,
                        ClickCount = p.ClickCount,
                        ContentUrl = p.ContentUrl,
                        CategoryName = p.CategoryName,
                        Vote = p.Vote,
                        p.Category,
                        p.CommentIsActive,
                        p.CommentCount,
                        //p.ShareWX,
                        //p.SharePYQ,
                        p.Type,
                        p.Content,
                        p.IsShow,
                        p.CreatorID
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { ArticleList = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ArticleIndex()
        {
            ViewData["YesterdaySumCount"] = ArticleManagerLazy.YesterdaySumCount();
            return View();
        }
        [HttpPost]
        public JsonResult ArticleIndex(string KeyStr, string StartTime, string EndTime, string CategoryTag, int? PageIndex, int? type = -1)
        {
            int _PageSize = 20;
            var _List = manager.SelectArticle(KeyStr, StartTime, EndTime, CategoryTag, _PageSize, PageIndex, type);
            if (_List != null && _List.Count > 0)
            {
                return Json(new
                {
                    ArticleList = _List.Select(p => new
                    {
                        PKID = p.PKID,
                        SmallImage = p.SmallImage,
                        SmallTitle = p.SmallTitle,
                        Image = p.Image,
                        BigTitle = p.BigTitle,
                        PublishDateTime = p.PublishDateTime.ToString("yyyy-MM-dd HH:mm:00"),
                        ContentUrl = p.ContentUrl,
                        CategoryName = p.CategoryName,
                        p.Category,
                        p.CommentIsActive,
                        p.Type,
                        p.Content,
                        p.IsShow,
                        p.CreatorID,
                        p.CommentCount,
                        p.CreatorInfo
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { ArticleList = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteArticle(int PKID)
        {
            try
            {
                manager.Delete(PKID);
                CreateJson();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ReplaceContentImgUrl(int pkid, bool isAall = false, int pageSize = 10)
        {
            var contentUrl = "";
            IDictionary<int, string> replaceResult = new Dictionary<int, string>();
            var id = 0;
            var ArticleFilePath = swc.WebConfigurationManager.AppSettings["UploadDoMain_news"];
            var ArticleAddress = swc.WebConfigurationManager.AppSettings["DoMain_news"] + ArticleFilePath;
            List<Article> errorData = new List<Article>();
            if (pkid > 0)
            {
                var article = new ArticleManager().GetByPKID(pkid);
                if (article != null)
                {
                    article.PKID = Convert.ToInt32(article.PKID);
                    errorData.Add(article);
                }
            }
            else if (isAall)
            {
                errorData = new ArticleManager().GetErrorImgUrlData(pageSize);
            }
            if (errorData != null && errorData.Any())
            {
                foreach (var article in errorData)
                {
                    Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
                    List<Tuple<string, string>> imgUrlList = new List<Tuple<string, string>>();
                    if (article.Type == 1)
                    {
                        var content = JsonConvert.DeserializeObject<List<ArticleContentModel>>(article.Content);
                        foreach (var item in content)
                        {
                            if (item.describe.Contains("7sboir.com"))
                            {
                                MatchCollection matches = regImg.Matches(WebUtility.HtmlDecode(item.describe));
                                foreach (Match match in matches)
                                {
                                    var originUrl = match.Groups["imgUrl"].Value;
                                    if (originUrl.Contains("7sboir.com"))
                                    {
                                        var newImgUrl = ConvertNewImgUrl(originUrl);
                                        if (!string.IsNullOrEmpty(newImgUrl))
                                            imgUrlList.Add(Tuple.Create(originUrl, newImgUrl));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        MatchCollection matches = regImg.Matches(WebUtility.HtmlDecode(article.Content));
                        foreach (Match match in matches)
                        {
                            var originUrl = match.Groups["imgUrl"].Value;
                            if (originUrl.Contains("7sboir.com"))
                            {
                                var newImgUrl = ConvertNewImgUrl(originUrl);
                                if (!string.IsNullOrEmpty(newImgUrl))
                                    imgUrlList.Add(Tuple.Create(originUrl, newImgUrl));
                            }
                        }
                    }

                    if (imgUrlList != null && imgUrlList.Any())
                    {
                        imgUrlList.ForEach(x =>
                        {
                            article.Content = article.Content.Replace(x.Item1, x.Item2);
                        });
                        if (article.Type == 1)
                        {
                            article.ContentHtml = ConvertToContentHtml(article.Content);
                        }
                        else if (article.Type == 0 || article.Type == 99)
                        {
                            article.ContentHtml = article.Content;
                        }
                        imgUrlList.ForEach(x =>
                        {
                            article.ContentHtml = article.ContentHtml.Replace(x.Item1, x.Item2);
                        });
                        manager.Add(article, out contentUrl, out id, ArticleAddress);
                        if (article.Type != 2)
                        {
                            //SaveFileAddress(article, contentUrl, ArticleFilePath, id);
                            RefreshArticleCache(article.PKID);//刷新文章的缓存
                            LoggerManager.InsertOplog(User.Identity.Name, ObjectType, article.PKID, "替换7sboir.com的图片");
                        }
                        replaceResult.Add(article.PKID, "转换成功");
                    }
                    else
                    {
                        replaceResult.Add(article.PKID, "没有需要转换的图片");
                    }
                }
            }
            else
            {
                replaceResult.Add(0, "没有需要转换的数据");
            }

            return Json(JsonConvert.SerializeObject(replaceResult), JsonRequestBehavior.AllowGet);
        }

        private string ConvertNewImgUrl(string originUrl)
        {
            var imgUrl = "";
            try
            {
                WebClient webClient = new WebClient();
                webClient.Credentials = CredentialCache.DefaultCredentials;
                var buffer = webClient.DownloadData(originUrl);
                using (var client = new Tuhu.Service.Utility.FileUploadClient())
                {
                    string dirName = swc.WebConfigurationManager.AppSettings["UploadDoMain_image"];

                    var result = client.UploadImage(new ImageUploadRequest(dirName, buffer));
                    result.ThrowIfException(true);
                    if (result.Success)
                    {
                        imgUrl = result.Result;
                        //_SImage= ImageHelper.GetImageUrl(result.Result, 100);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return swc.WebConfigurationManager.AppSettings["DoMain_image"] + imgUrl;
        }

        public ActionResult DeleteQuestion(int PKID)
        {
            try
            {
                CreateJson();
                LoggerManager.InsertOplog(User.Identity.Name, ObjectType, PKID, "删除了该问题");
                MQMessageClient.DeleteMessageQueue(1, PKID.ToString());
                return Json(manager.DeleteQuestion(PKID) > 0, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddOperateLog(int PKID, string userId, string objectType = "ArticleTbl")
        {
            bool r = LoggerManager.InsertOplog(User.Identity.Name, objectType, PKID, "加入黑名单：" + userId);
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogList(string objectType, string objectID)
        {
            var list = LoggerManager.SelectOprLogByParams(objectType, objectID);
            return View(list);
        }
        public ActionResult AddArticle(int? PKID, int type = 0) //type =0 老文章,1 = 新文章,2=专题,3=问题
        {
            int _PKID = PKID.GetValueOrDefault(0);
            List<ZTreeModel> tagTreeItems = _CategoryTagManager.SelectByTree().ToList();

            if (_PKID != 0)
            {
                ViewBag.Title = "修改文章";
                //获取类目集合
                ViewBag.categoryModelList = ArticleManagerLazy.SelectCategory();
                Article articleModel = manager.GetByPKID(_PKID);
                if (articleModel.Type == 1) { ViewBag.contentList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ContentViewModel>>(articleModel.Content); }

                #region 标签树初始化
                if (articleModel != null && !string.IsNullOrEmpty(articleModel.CategoryTags))
                {
                    List<ZTreeTagModel> dicTreeItems = JsonConvert.DeserializeObject<List<ZTreeTagModel>>(articleModel.CategoryTags);
                    for (int i = 0; i < dicTreeItems.Count(); i++)
                    {
                        var _dicTreeItems = dicTreeItems[i];
                        for (int j = 0; j < tagTreeItems.Count(); j++)
                        {
                            var _tagTreeItems = tagTreeItems[j];
                            if (_dicTreeItems.key.Equals(_tagTreeItems.id))
                            {
                                _tagTreeItems.open = true;
                                _tagTreeItems.isChecked = true;
                            }
                        }
                    }
                    ViewBag.CategoryTagManager = JsonConvert.SerializeObject(tagTreeItems).Replace("isChecked", "checked");
                }
                else
                {
                    ViewBag.CategoryTagManager = JsonConvert.SerializeObject(tagTreeItems).Replace("isChecked", "checked");
                }
                #endregion
                return View(articleModel);
            }
            else
            {
                //标签树列表
                ViewBag.CategoryTagManager = JsonConvert.SerializeObject(tagTreeItems).Replace("isChecked", "checked");

                ViewBag.Title = "新增文章";
                //获取类目集合
                ViewBag.categoryModelList = ArticleManagerLazy.SelectCategory();
                Article _Model = new Article()
                {
                    PKID = 0,
                    Type = type,
                    PublishDateTime = DateTime.Now
                };
                return View(_Model);
            }
        }

        /// <summary>
        /// html 解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlEncode(string str)
        {
            if (!string.IsNullOrEmpty(str))
                return str.Replace(@"&amp;", "&").Replace(@"&lt;", "<").Replace(@"&gt;", ">");
            return "";
        }

        /// <summary>
        /// html 编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlDecode(string str)
        {
            if (!string.IsNullOrEmpty(str))
                return str.Replace(@"&", "&amp;").Replace(@"<", "&lt;").Replace(@">", "&gt;");
            return "";
        }

        [HttpGet]
        public ActionResult EditCategory()
        {
            List<ArticleCategory> categoryModelList = ArticleManagerLazy.SelectCategory();
            ViewBag.CategoryModelList = categoryModelList;
            return View();
        }

        //添加/修改类目
        [HttpPost]
        public string EditCategoryData(string categoryStr)
        {
            var categoryModelList = CategorySerialize(categoryStr);
            if (categoryModelList != null && categoryModelList.Count > 0)
            {
                if (ArticleManagerLazy.EditCategory(categoryModelList))
                    return "1";
                else
                    return "0";
            }
            return "0";
        }

        /// <summary>
        /// 数据序列化
        /// </summary>
        /// <param name="payMentWay"></param>
        /// <returns></returns>
        public List<ArticleCategory> CategorySerialize(string categoryStr)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ArticleCategory>>(categoryStr) ?? null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult ViewArticle(int id = 7)
        {
            // string sql = "select * from tbl_article where pkid=@id";

            Article article = manager.GetByPKID(id);
            return Content(GetArticleContent(article));
        }
        private string GetArticleContent(Article article)
        {
            string _PureHtml = GetPureHtml("/Content/CommonFile/ArticleTemplate.html");
            _PureHtml = _PureHtml.Replace("$headimg$", article.Image);
            _PureHtml = _PureHtml.Replace("$BigTitle$", article.BigTitle);
            _PureHtml = _PureHtml.Replace("$PKID$", article.PKID.ToString());
            _PureHtml = _PureHtml.Replace("$PublishDateTime$", article.PublishDateTime.ToString("yyyy-MM-dd"));
            _PureHtml = _PureHtml.Replace("$Content$", article.Content);
            return _PureHtml;
        }

        /// <summary>
        /// 新文章模板
        /// </summary>
        /// <param name="article"></param>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        private string GetNewArticleContent(Article article, int id)
        {
            var colorArr = new string[] { "blue", "green", "orange", "red", "sky-blue", "blue", "green", "orange", "red", "sky-blue" };
            int colorRandom = rd.Next(0, colorArr.Length);
            string _PureHtml = GetPureHtml("/Content/CommonFile/ArticleNewTemplate.html");
            if (!string.IsNullOrEmpty(_PureHtml))
            {
                List<ContentViewModel> modelList = null;
                if (!string.IsNullOrEmpty(article.Content)) { modelList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ContentViewModel>>(article.Content); }
                StringBuilder sbContentList = new StringBuilder();
                Regex Reg = new Regex(@"(?is)<forCode>(.*?)</forCode>", RegexOptions.IgnoreCase);
                string forCodeContent = Reg.Match(_PureHtml).Value;
                if (modelList != null && modelList.Count > 0 && !string.IsNullOrEmpty(forCodeContent))
                {
                    forCodeContent = forCodeContent.Replace("<forCode>", "").Replace("</forCode>", "");
                    for (int i = 0; i < modelList.Count; i++)
                    {
                        string _forCodeContent = forCodeContent;
                        _forCodeContent = _forCodeContent.Replace("$productCount$", (i + 1).ToString());
                        _forCodeContent = _forCodeContent.Replace("$productTitle$", modelList[i].title);
                        _forCodeContent = _forCodeContent.Replace("$productDescribe$", HtmlEncode(modelList[i].describe));
                        _forCodeContent = _forCodeContent.Replace("$productName$", modelList[i].productName);
                        _forCodeContent = _forCodeContent.Replace("$productPrice$", modelList[i].productPrice);
                        _forCodeContent = _forCodeContent.Replace("$productUrl$", modelList[i].productUrl);
                        _forCodeContent = _forCodeContent.Replace("$maxPic$", modelList[i].maxPic);

                        if (string.IsNullOrEmpty(modelList[i].productName) ||
                            string.IsNullOrEmpty(modelList[i].productPrice) ||
                            string.IsNullOrEmpty(modelList[i].productUrl))
                        {
                            _forCodeContent = _forCodeContent.Replace("$isshowcss$", "style='display:none;'");
                        }
                        else
                        {
                            _forCodeContent = _forCodeContent.Replace("$isshowcss$", "");
                        }
                        sbContentList.Append(_forCodeContent);
                    }
                }
                _PureHtml = Regex.Replace(_PureHtml, @"(?is)<forCode>(.*?)</forCode>", sbContentList.ToString(), RegexOptions.IgnoreCase);
                _PureHtml = _PureHtml.Replace("$Category$", article.Category);
                _PureHtml = _PureHtml.Replace("$headimg$", article.Image);
                _PureHtml = _PureHtml.Replace("$BigTitle$", article.BigTitle);
                _PureHtml = _PureHtml.Replace("$Brief$", article.Brief);
                _PureHtml = _PureHtml.Replace("$PKID$", article.PKID == 0 ? id.ToString() : article.PKID.ToString());
                //_PureHtml = _PureHtml.Replace("$Vote$", article.Vote.ToString());
                _PureHtml = _PureHtml.Replace("$Color$", colorArr[colorRandom]);
                _PureHtml = _PureHtml.Replace("$ContentUrl$", article.ContentUrl);
                _PureHtml = _PureHtml.Replace("$SmallTitle$", article.SmallTitle);
                var CategoryModel = ArticleManagerLazy.SelectCategory().Where(w => w.id == Convert.ToInt32(article.Category)).FirstOrDefault<ArticleCategory>();
                //_PureHtml = _PureHtml.Replace("$CategoryName$", CategoryModel.CategoryName);
                _PureHtml = _PureHtml.Replace("$PublishDateTime$", article.PublishDateTime.ToString("MM月dd日"));

                string CategoryTagsName = "";
                List<ZTreeTagModel> CategoryTagList = JsonConvert.DeserializeObject<List<ZTreeTagModel>>(article.CategoryTags ?? "");
                foreach (var items in CategoryTagList.Where(w => w.isShow == 1).ToList())
                {
                    CategoryTagsName += string.Format("<span onclick=\"toDiscoveryListApi.Invoke_v1({0},'{1}')\">{1}</span>&nbsp;", items.key, items.value);
                }
                _PureHtml = _PureHtml.Replace("$CategoryTags$", CategoryTagsName ?? "");
            }
            return _PureHtml;
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AddArticle(Article article)
        {
            //输出参数（文章URL）
            var contentUrl = "";
            //输出参数（ID）
            var id = 0;
            //路径
            var ArticleFilePath = swc.WebConfigurationManager.AppSettings["UploadDoMain_news"];
            //var ArticleFilePath = "/activity/ActivityHtml/";
            //地址
            var ArticleAddress = swc.WebConfigurationManager.AppSettings["DoMain_news"] + ArticleFilePath;
            if (article.PKID == 0)
            {
                if (article.Type == 1) article.ContentHtml = ConvertToContentHtml(article.Content);
                else if (article.Type == 0 || article.Type == 99)
                {
                    if (article.IsFaxianChannel == 1)
                    {
                        article.Type = 0;
                    }
                    else
                    {
                        article.Type = 99;
                    }
                    article.ContentHtml = article.Content;
                }
                //添加数据(表：[Marketing]..tbl_Article)
                manager.Add(article, out contentUrl, out id, ArticleAddress);
                if (article.Type != 2)
                {
                    SaveFileAddress(article, contentUrl, ArticleFilePath, id);
                    LoggerManager.InsertOplog(User.Identity.Name, ObjectType, id, "新增文章&问题");
                }
            }
            else
            {
                article.PKID = Convert.ToInt32(article.PKID);
                if (article.Type == 1) article.ContentHtml = ConvertToContentHtml(article.Content);
                else if (article.Type == 0 || article.Type == 99)
                {
                    if (article.IsFaxianChannel == 1)
                    {
                        article.Type = 0;
                    }
                    else
                    {
                        article.Type = 99;
                    }
                    article.ContentHtml = article.Content;
                }

                //修改数据(表：[Marketing]..tbl_Article)
                manager.Add(article, out contentUrl, out id, ArticleAddress);
                if (article.Type != 2)
                {
                    SaveFileAddress(article, contentUrl, ArticleFilePath, id);
                    RefreshArticleCache(article.PKID);//刷新文章的缓存
                    LoggerManager.InsertOplog(User.Identity.Name, ObjectType, article.PKID, "编辑文章&问题");
                    if (article.Type == 3)
                    {
                        MQMessageClient.UpdateMessageQueue(1, article.PKID.ToString(), article.Content, article.Image);
                    }
                }
            }
            UpdateArticleCategoryTag(id, article.CategoryTags);
            CreateJson();
            //return Redirect("ArticleManage");
            return Content("ok");
        }

        /// <summary>
        /// 更新标签关联表
        /// </summary> 
        /// <param name="id"></param>
        /// <param name="categoryTags"></param>
        private void UpdateArticleCategoryTag(int id, string categoryTags)
        {
            if (!string.IsNullOrEmpty(categoryTags))
            {
                List<ZTreeTagModel> dicTreeItems = JsonConvert.DeserializeObject<List<ZTreeTagModel>>(categoryTags);
                List<ArticleCategoryTagModel> articleTags = new List<ArticleCategoryTagModel>();
                foreach (var item in dicTreeItems)
                {
                    articleTags.Add(new ArticleCategoryTagModel()
                    {
                        ArticleId = id,
                        CategoryTagId = item.key
                    });
                }
                _CategoryTagManager.UpdateBatch(id, articleTags);
            }
        }

        /// <summary>
        /// 刷某一篇文章的缓存
        /// </summary>
        /// <param name="articleId">文章Id</param>
        private void RefreshArticleCache(int articleId)
        {
            try
            {
                string domain = Request.Url.Host.Contains("tuhu.cn") ? "tuhu.cn" : "tuhu.test";
                string api = string.Format("http://faxian.{0}/Article/RefreshCache?name=ArticleDetail&key=DetailById/{1}", domain, articleId);
                using (WebClient client = new WebClient())
                {
                    string result = client.DownloadString(api);
                    RefreshResult res = JsonConvert.DeserializeObject<RefreshResult>(result);
                }
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// 刷头条列表文章的缓存（只刷一页）
        /// </summary>
        /// <param name="tagId">标签的Id</param>
        private void RefreshTouTiaoListCache(string tagId = "")
        {
            try
            {
                string domain = Request.Url.Host.Contains("tuhu.cn") ? "tuhu.cn" : "tuhu.test";
                string api = string.Format("http://faxian.{0}/Article/RefreshCache?name=TouTiao_List_Cache&key=ArticleList/1/20/{1}", domain, tagId);
                using (WebClient client = new WebClient())
                {
                    string result = client.DownloadString(api);
                    RefreshResult res = JsonConvert.DeserializeObject<RefreshResult>(result);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public string ConvertToContentHtml(string Content)
        {
            StringBuilder sbHtml = new StringBuilder();
            try
            {
                var articleJsons = JsonConvert.DeserializeObject<List<ArticleContentModel>>(Content);
                sbHtml.Append("<div  id='content'>");
                for (int i = 0; i < articleJsons.Count; i++)
                {
                    var content = articleJsons[i];
                    string html = @"<forcode><div style='padding: 20px 0 0 0;position: relative;'><div style='color: #333;margin-bottom: 18px;font-size: 18px;line-height: 26px;padding-left: 26px;position: relative;'><span style='position:absolute;width:21px;height:21px;background-color:#e03548;border-radius:50%;color:#fff;font-size:16px;text-align:center;line-height:22px;overflow:hidden;top:0;left:0'>@Num</span>@Title</div><div style='color:#363636;font-size:16px;line-height:28px;-webkit-box-pack:justify;-moz-box-pack:justify;justify-content:space-between;-webkit-justify-content:space-between;text-align:justify;margin-bottom: 15px;'>@Content</div>@Product</div ></forcode>";
                    html = html.Replace("@Num", (i + 1).ToString()).Replace("@Title", content.title).Replace("@Content", WebUtility.HtmlDecode(content.describe.Replace(@"\", "")));
                    //存在商品信息
                    if (!string.IsNullOrEmpty(content.productName) && !string.IsNullOrEmpty(content.productPrice) && !string.IsNullOrEmpty(content.productUrl))
                    {
                        string productInfo = string.Format(@"<div style='position:relative;padding:15px;margin:20px 0;'><i style='position:absolute;width:15px;height:15px;border-style:solid;border-color:#d9d9d9;border-width:2px 0 0 2px;top:0;left:0;'></i><i style='position:absolute;width:15px;height:15px;border-style:solid;border-color:#d9d9d9;border-width:0 2px 2px 0;right:0;bottom:0;'></i><div style='border:1px solid #eee;'><img src='@ProductPic' style='display:block;width:100%;' class='cover'>
                        <div style='width:94.6%;clear:both;display:table;background-color:#fafafa;padding:10px 15px;'><p style='color:#666;font-size:14px;float:left;'>
                                @ProductName<br><span style='color:#df3448;font-size:16px;'>￥@Price</span>
                            </p><a href=""@ProductUrl"" style='float:right;width:75px;height:24px;border:1px solid #e74c3c;border-radius:5px;font-size:12px;color:#e74c3c;text-align:center;line-height:24px;margin-top:3px;text-decoration:none;'>查看详情</a></div></div></div>");
                        productInfo = productInfo.Replace("@ProductPic", content.maxPic).Replace("@ProductName", content.productName).Replace("@Price", content.productPrice).Replace("@ProductUrl", content.productUrl);
                        html = html.Replace("@Product", productInfo);
                    }
                    else
                    {
                        html = html.Replace("@Product", "");
                    }
                    sbHtml.Append(html);
                }
                sbHtml.Append("</div>");
            }
            catch
            {
                sbHtml.Append("");
            }
            return sbHtml.ToString().Replace("\r\n", "").Replace(@"\", "");
        }

        /// <summary>
        /// 保存地址文件到http://image.tuhu.test/activity/setting/activityhtml/news
        /// </summary>
        /// <param name="article">对象</param>
        /// <param name="strUrl">地址</param>
        public void SaveFileAddress(Article article, string strUrl, string filePath, int id)
        {
            if (article != null && !string.IsNullOrEmpty(strUrl))
            {
                //截取地址
                var url = strUrl.Substring((strUrl.LastIndexOf("/") + 1));
                url = url.Substring(0, url.LastIndexOf("."));
                if (!string.IsNullOrEmpty(article.Content))
                {
                    string ArticleFilePath = string.Empty;
                    Exception ex = null;
                    try
                    {
                        string _PureHtml = "";
                        if (article.Type == 1)
                            _PureHtml = GetNewArticleContent(article, id);
                        else
                            _PureHtml = GetArticleContent(article);

                        byte[] array = System.Text.Encoding.UTF8.GetBytes(_PureHtml);
                        ArticleFilePath = filePath + url.ToString() + ".html";
                        var client = new WcfClinet<IFileUpload>();
                        var result = client.InvokeWcfClinet(w => w.UploadFile(ArticleFilePath, array));
                    }
                    catch (Exception error)
                    {
                        ex = error;
                    }
                }
            }
        }

        //public ActionResult AddArticle(Article article)
        //{
        //    if (!string.IsNullOrEmpty(article.Content))
        //    {
        //        string _PureHtml = GetArticleContent(article);
        //        byte[] array = System.Text.Encoding.UTF8.GetBytes(_PureHtml);
        //        Stream stream = new MemoryStream(array);
        //        article.ContentUrl = UploadFile(stream, "article_" + DateTime.Now.ToString("yyMMddHHmmssfff") + ".html");
        //    }
        //    if (article.PKID == 0)
        //    {
        //        manager.Add(article);
        //    }
        //    else
        //    {
        //        manager.Update(article);
        //    }
        //    CreateJson();
        //    return Redirect("ArticleManage");
        //}

        public static string GetPureHtml(string strfile)
        {
            string strout = "";
            string fpath = System.Web.HttpContext.Current.Server.MapPath(strfile);
            if (System.IO.File.Exists(fpath))
            {
                StreamReader sr = new StreamReader(fpath, System.Text.Encoding.UTF8);
                string input = sr.ReadToEnd();
                sr.Close();
                if (isLuan(input))
                {
                    StreamReader sr2 = new StreamReader(fpath, System.Text.Encoding.Default);
                    input = sr2.ReadToEnd();
                    sr2.Close();
                }
                strout = input;
            }
            return strout;
        }
        public static bool isLuan(string txt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(txt);
            //239 191 189
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i < bytes.Length - 3)
                    if (bytes[i] == 239 && bytes[i + 1] == 191 && bytes[i + 2] == 189)
                    {
                        return true;
                    }
            }
            return false;
        }
        [HttpPost]
        public JsonResult AddArticleImg()
        {
            string _BImage = string.Empty;
            string _SImage = string.Empty;
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    try
                    {
                        string _FileExt = Path.GetExtension(file.FileName);
                        string _ImageName = "article_" + DateTime.Now.ToString("yyMMddHHmmssfff");

                        var _InputStream = file.InputStream;

                        Image bitmap;
                        ImageFormat rawFormt;
                        using (var rawImage = Bitmap.FromStream(_InputStream))
                        {
                            rawFormt = rawImage.RawFormat;
                            bitmap = CutForSquare(rawImage, 100, 100);
                        }

                        using (var stream = new MemoryStream())
                        {
                            using (bitmap)
                            {
                                bitmap.Save(stream, rawFormt);
                            }

                            _SImage = UploadFile(stream, _ImageName + "_s" + _FileExt);
                            _BImage = UploadFile(_InputStream, _ImageName + _FileExt);
                        }
                    }
                    catch { }
                }
            }
            return Json(new { BImage = _BImage, SImage = _SImage }, "text/html");
        }

        /// <summary>
        /// 此方法为上传文件到七牛方法(新首页资源老上传方法)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddArticleImg2ForQiLiu()
        {
            string _BImage = string.Empty;
            string _SImage = string.Empty;
            Exception ex = null;
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    try
                    {
                        string _FileExt = Path.GetExtension(file.FileName);
                        string _ImageName = "article_" + DateTime.Now.ToString("yyMMddHHmmssfff");

                        var _InputStream = file.InputStream;
                        _SImage = UploadFile(_InputStream, _ImageName + "_s" + _FileExt);
                        _BImage = UploadFile(_InputStream, _ImageName + _FileExt);
                    }
                    catch (Exception error)
                    {
                        ex = error;
                    }
                }
            }
            return Json(new { BImage = _BImage, SImage = _SImage, Msg = ex == null ? "上传成功" : ex.Message }, "text/html");
        }

        /// <summary>
        /// 图片上传至阿里云
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddArticleImg2()
        {
            string msg = string.Empty;
            string imageUrl = string.Empty;
            if (Request.Files.Count > 0)
            {
                try
                {
                    var imgfile = Request.Files[0];
                    var buffer = new byte[imgfile.ContentLength];
                    imgfile.InputStream.Read(buffer, 0, buffer.Length);
                    var uploadResult = ImageUploadHelper.UpdateLoadImage(buffer);
                    if (uploadResult.Item1)
                    {
                        imageUrl = ImageHelper.GetImageUrl(uploadResult.Item2);
                        msg = "上传成功";
                    }
                    else
                    {
                        msg = uploadResult.Item2;
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }
            else
            {
                msg = "请选择文件";
            }
            return Json(new
            {
                BImage = imageUrl,
                SImage = imageUrl,
                Msg = msg
            }, "text/html");
        }
        /// <summary>
        /// 图片上传至阿里云
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddArticleImgForBeautyBanner()
        {
            string imageUrl = string.Empty;
            string msg = string.Empty;
            if (Request.Files.Count > 0)
            {
                try
                {
                    var Imgfile = Request.Files[0];
                    if (Imgfile.ContentLength <= 0)
                        return Json(new
                        {
                            BImage = "",
                            Msg = "请选择文件"
                        }, "text/html");
                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);
                    using (var imageSteam = Image.FromStream(Imgfile.InputStream))
                    {
                        if (imageSteam.Width * 1.0 / imageSteam.Height != 5.0 / 2)
                            return Json(new
                            {
                                BImage = "",
                                Msg = "图片宽高尺寸比例应为(5:2)"
                            }, "text/html");
                    }
                    var uploadResult = ImageUploadHelper.UpdateLoadImage(buffer);
                    if (uploadResult.Item1)
                    {
                        imageUrl = ImageHelper.GetImageUrl(uploadResult.Item2);
                        msg = "上传成功";
                    }
                    else
                    {
                        msg = uploadResult.Item2;
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }
            else
            {
                msg = "请选择文件";
            }
            return Json(new
            {
                BImage = imageUrl,
                Msg = msg
            }, "text/html");
        }
        /// <summary>
        /// 图片上传地址
        /// </summary>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="identifying">标识</param>
        /// <param name="urlStr">地址</param>
        /// <returns></returns>
        public ActionResult ImageUploadToAli(string from)
        {
            string _BImage = string.Empty;
            string _SImage = string.Empty;
            string _ImgGuid = Guid.NewGuid().ToString();
            Exception ex = null;
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                try
                {
                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);
                    using (var client = new Tuhu.Service.Utility.FileUploadClient())
                    {
                        string dirName = swc.WebConfigurationManager.AppSettings["UploadDoMain_image"];

                        var result = client.UploadImage(new ImageUploadRequest(dirName, buffer));
                        result.ThrowIfException(true);
                        if (result.Success)
                        {
                            _BImage = result.Result;
                            //_SImage= ImageHelper.GetImageUrl(result.Result, 100);
                        }
                    }
                }
                catch (Exception error)
                {
                    ex = error;
                }
            }
            string imgUrl = swc.WebConfigurationManager.AppSettings["DoMain_image"] + _BImage;

            if (from == "content")
            {
                return Json(new { error = 0, url = imgUrl });
            }
            return Json(new
            {
                BImage = imgUrl,
                SImage = imgUrl,
                Msg = ex == null ? "上传成功" : ex.Message
            }, "text/html");
        }

        /// <summary>
        /// 字节流转换成图片
        /// </summary>
        /// <param name="byt">要转换的字节流</param>
        /// <returns>转换得到的Image对象</returns>
        public static Dictionary<string, object> BytToImg(byte[] byt)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(byt))
                {
                    using (Image img = Image.FromStream(ms))
                    {
                        Dictionary<string, object> dicToImg = new Dictionary<string, object>();
                        dicToImg.Add("Width", img.Width);
                        dicToImg.Add("Height", img.Height);
                        return dicToImg;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public ActionResult RefreshView()
        {
            return View();
        }
        //刷新点赞数的json文件
        public ActionResult Refurbish()
        {
            try
            {
                var p = CreateJson();
                return Content(p);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        protected string CreateJson()
        {
            try
            {
                List<Article> _LatestArticle = manager.SelectBy(null, 50, 1);
                List<Article> _LatestID = new List<Article>();
                _LatestID.Add(_LatestArticle.Where(p => p.PublishDateTime <= DateTime.Now).FirstOrDefault());
                _LatestID.AddRange(_LatestArticle.Where(p => p.PublishDateTime > DateTime.Now).OrderBy(p => p.PublishDateTime).ThenBy(p => p.PKID));

                string _ArticleJson = JsonConvert.SerializeObject(_LatestArticle.Select(p => new
                {
                    PKID = p.PKID,
                    Catalog = p.Catalog,
                    Image = p.Image,
                    SmallImage = p.SmallImage,
                    SmallTitle = p.SmallTitle,
                    BigTitle = p.BigTitle,
                    TitleColor = string.IsNullOrEmpty(p.TitleColor) ? string.Empty : p.TitleColor.Replace(" ", ""),
                    Brief = p.Brief,
                    ContentUrl = p.ContentUrl,
                    Source = p.Source,
                    PublishDateTime = p.PublishDateTime,
                    RedirectUrl = p.RedirectUrl,
                    ClickCount = p.ClickCount,
                    Vote = p.Vote
                }));
                string _IDJson = JsonConvert.SerializeObject(_LatestID.Select(p => new { PKID = p.PKID, PublishDateTime = p.PublishDateTime.ToString("yyyy-MM-dd HH:mm:00") })).ToString();
                string _Path = System.Configuration.ConfigurationManager.AppSettings["sem:appSavePath"] + "article/";
                CreateDirectory(_Path);
                if (Debugger.IsAttached)
                {
                    _Path = Server.MapPath("/Content/");
                    //_Path="C:/svn/TuhuWeb/Tuhu.WebSite/Tuhu.WebSite.Web/Tuhu.WebSite.Web.Resource/app/article/";
                }
                StreamWriter mysw = new StreamWriter(_Path + "latestid.json", false, System.Text.Encoding.UTF8);
                mysw.Write(_IDJson);
                mysw.Close();

                StreamWriter mysw2 = new StreamWriter(_Path + "latestarticle.json", false, System.Text.Encoding.UTF8);
                mysw2.Write(_ArticleJson);
                mysw2.Close();
                return _Path;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        #region 正方型裁剪并缩放

        /// <summary>
        /// 正方型裁剪
        /// 以图片中心为轴心，截取正方型，然后等比缩放
        /// 用于头像处理
        /// </summary>
        /// <remarks>吴剑 2012-08-08</remarks>
        /// <param name="fromFile">原图Stream对象</param>
        /// <param name="fileSaveUrl">缩略图存放地址</param>
        /// <param name="side">指定的边长（正方型）</param>
        /// <param name="quality">质量（范围0-100）</param>
        public static Image CutForSquare(Image initImage, int side, int quality)
        {
            using (var stream = new MemoryStream())
            {
                //原图宽高均小于模版，不作处理，直接保存
                if (initImage.Width <= side && initImage.Height <= side)
                {
                    initImage.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                else
                {
                    //原始图片的宽、高
                    int initWidth = initImage.Width;
                    int initHeight = initImage.Height;

                    //非正方型先裁剪为正方型
                    if (initWidth != initHeight)
                    {
                        //截图对象
                        System.Drawing.Image pickedImage = null;
                        System.Drawing.Graphics pickedG = null;

                        //宽大于高的横图
                        if (initWidth > initHeight)
                        {
                            //对象实例化
                            pickedImage = new System.Drawing.Bitmap(initHeight, initHeight);
                            pickedG = System.Drawing.Graphics.FromImage(pickedImage);
                            //设置质量
                            pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            //定位
                            Rectangle fromR = new Rectangle((initWidth - initHeight) / 2, 0, initHeight, initHeight);
                            Rectangle toR = new Rectangle(0, 0, initHeight, initHeight);
                            //画图
                            pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);
                            //重置宽
                            initWidth = initHeight;
                        }
                        //高大于宽的竖图
                        else
                        {
                            //对象实例化
                            pickedImage = new System.Drawing.Bitmap(initWidth, initWidth);
                            pickedG = System.Drawing.Graphics.FromImage(pickedImage);
                            //设置质量
                            pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            //定位
                            Rectangle fromR = new Rectangle(0, (initHeight - initWidth) / 2, initWidth, initWidth);
                            Rectangle toR = new Rectangle(0, 0, initWidth, initWidth);
                            //画图
                            pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);
                            //重置高
                            initHeight = initWidth;
                        }

                        //将截图对象赋给原图
                        initImage = (System.Drawing.Image)pickedImage.Clone();
                        //释放截图资源
                        pickedG.Dispose();
                        pickedImage.Dispose();
                    }

                    //缩略图对象
                    System.Drawing.Image resultImage = new System.Drawing.Bitmap(side, side);
                    System.Drawing.Graphics resultG = System.Drawing.Graphics.FromImage(resultImage);
                    //设置质量
                    resultG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    resultG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    //用指定背景色清空画布
                    resultG.Clear(Color.White);
                    //绘制缩略图
                    resultG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, side, side), new System.Drawing.Rectangle(0, 0, initWidth, initHeight), System.Drawing.GraphicsUnit.Pixel);

                    //关键质量控制
                    //获取系统编码类型数组,包含了jpeg,bmp,png,gif,tiff
                    ImageCodecInfo[] icis = ImageCodecInfo.GetImageEncoders();
                    ImageCodecInfo ici = null;
                    foreach (ImageCodecInfo i in icis)
                    {
                        if (i.MimeType == "image/jpeg" || i.MimeType == "image/bmp" || i.MimeType == "image/png" || i.MimeType == "image/gif")
                        {
                            ici = i;
                        }
                    }
                    EncoderParameters ep = new EncoderParameters(1);
                    ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);


                    //释放关键质量控制所用资源
                    ep.Dispose();

                    //释放缩略图资源
                    resultG.Dispose();
                    //resultImage.Dispose();

                    //释放原始图片资源
                    initImage.Dispose();
                    return resultImage;
                }
            }
            return initImage;
        }

        #endregion

        protected string UploadFile(Stream stream, string fileName)
        {
            string _ReturnStr = string.Empty;
            stream.Seek(0, SeekOrigin.Begin);
            IOClient _IOClient = new IOClient();
            var _PutPolicy = new PutPolicy(swc.WebConfigurationManager.AppSettings["Qiniu:comment_scope"], 3600).Token();
            var _Result = _IOClient.Put(_PutPolicy, fileName, stream, new PutExtra());
            if (_Result.OK)
                _ReturnStr = swc.WebConfigurationManager.AppSettings["Qiniu:comment_url"] + fileName;
            return _ReturnStr;
        }

        public ActionResult DeleteSeekKeyWord(string keyWord)
        {
            return Json(ArticleManagerLazy.DeleteSeekKeyWord(keyWord));
        }

        public ActionResult Theme()
        {

            return this.View();
        }

        public ActionResult Hot()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ArticleManageList(string KeyStr, int? PageIndex, int HotArticles = 0, int Category = 0)
        {
            string sqlStr = "  WHERE 1=1 ";

            if (HotArticles != 0)
            {
                if (HotArticles == 1)
                {
                    sqlStr += " AND HotArticles = 1";
                }
                else if (HotArticles == 2)
                {
                    sqlStr += " AND HotArticles = 0";
                }

            }

            if (Category != 0)
            {
                sqlStr += " AND Category  =" + Category;

            }
            if (!string.IsNullOrEmpty(KeyStr))
            {
                if (KeyStr == "ArticleBanner")
                {
                    sqlStr += " AND SmallBanner IS NOT NULL AND SmallBanner !='' ";
                }
                else
                {
                    sqlStr += " AND SmallTitle like N'%" + KeyStr + "%'";
                }
            }

            int _PageSize = 10;

            List<Article> _List = ArticleManagerLazy.GetArticle(sqlStr, _PageSize, PageIndex);

            List<ArticleView> list = new List<ArticleView>();
            foreach (var p in _List)
            {
                ArticleView item = new ArticleView();
                item.PKID = p.PKID;
                item.SmallImage = p.SmallImage;
                item.SmallTitle = p.SmallTitle;
                item.BigTitle = p.BigTitle;
                item.TitleColor = p.TitleColor ?? string.Empty;
                item.Source = p.Source ?? string.Empty;
                item.PublishDateTime = p.PublishDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                item.ClickCount = p.ClickCount;
                item.ContentUrl = p.ContentUrl;
                item.Vote = p.Vote;
                item.Category = p.Category;
                item.CommentIsActive = p.CommentIsActive;
                item.CommentCount = p.CommentCount;
                item.HotArticles = p.HotArticles;
                item.SmallBanner = p.SmallBanner;
                item.ArticleBanner = p.ArticleBanner;
                item.Bestla = p.Bestla;
                list.Add(item);
            }

            return Json(list, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UpdateHotArticles(string dataList)
        {
            var article = JsonConvert.DeserializeObject<List<Article>>(dataList);
            foreach (var item in article)
            {
                ArticleManagerLazy.UpdateHotArticles(item.PKID, item.HotArticles);
            }
            return Json(true);
        }

        public ActionResult UpdateTheme(int pkid, Article article)
        {
            if (ArticleManagerLazy.UpdateTheme(pkid, article))
            {
                return Json(true);
            }
            return Json(false);
        }

        public ActionResult DeleteTheme(int pkid)
        {
            Article article = new Article();
            article.SmallBanner = string.Empty;
            article.ArticleBanner = string.Empty;
            article.Bestla = false;
            if (ArticleManagerLazy.UpdateTheme(pkid, article))
            {
                return Json(true);
            }
            return Json(false);
        }

        public ActionResult AddTheme(int PKID = 0)
        {
            ViewBag.SmallTitle = ArticleManagerLazy.GetSmallTilte();
            if (PKID != 0)
            {
                return View(manager.GetByPKID(PKID));
            }
            else
            {
                Article _Model = new Article()
                {
                    PKID = 0,
                    PublishDateTime = DateTime.Now
                };
                return View(_Model);
            }

        }

        public ActionResult GetContentUrlById(int id)
        {
            return Json(ArticleManagerLazy.GetContentUrlById(id));
        }

        public ActionResult UserSeekKeyWord(int pageIndex = 1)
        {
            int count = 0;
            var model = ArticleManagerLazy.GetSeekKeyWord(15, pageIndex, out count);

            var list = new OutData<List<SeekKeyWord>, int>(model, count);
            var pager = new PagerModel(pageIndex, 15)
            {
                TotalItem = count
            };
            return this.View(new ListModel<SeekKeyWord>(list.ReturnValue, pager));
        }

        public ActionResult SeekKeyWord(int pageIndex = 1)
        {
            int count = 0;

            var model = ArticleManagerLazy.GetSeekKeyWord("", 15, pageIndex, out count);

            var list = new OutData<List<SeekHotWord>, int>(model, count);
            var pager = new PagerModel(pageIndex, 15)
            {
                TotalItem = count
            };
            return this.View(new ListModel<SeekHotWord>(list.ReturnValue, pager));
        }

        public ActionResult AddKeyWord(int id = 0)
        {
            if (id > 0)
            {
                SeekHotWord model = ArticleManagerLazy.GetSeekHotWordById(id);
                return this.View(model);
            }
            else
            {
                SeekHotWord model = new SeekHotWord();
                return this.View(model);
            }

        }

        public ActionResult InsertSeekKeyWord(SeekHotWord model)
        {
            if (!ArticleManagerLazy.IsRepeatSeekKeyWord(model.HotWord))
            {
                if (ArticleManagerLazy.InsertSeekKeyWord(model))
                {
                    return Json(2);

                }
                else
                {
                    return Json(3);
                }

            }
            return Json(1);
        }
        public ActionResult UpdateSeekKeyWord(SeekHotWord model)
        {
            if (!ArticleManagerLazy.IsRepeatSeekKeyWord(model))
            {
                if (ArticleManagerLazy.UpdateSeekKeyWord(model, model.id))
                {
                    return Json(2);

                }
                else
                {
                    return Json(3);
                }
            }
            return Json(1);
        }

        public ActionResult RemoveSeekKeyWord(int id)
        {
            return Json(ArticleManagerLazy.DeleleSeekKeyWordById(id));
        }

        /// <summary>
        /// 文章,问题,审核
        /// </summary>
        public ActionResult UpdateArticleToComment(int pkid, int type, int isShow, string UserID)
        {
            if (pkid > 0)
            {
                bool result = _ArticleCommentManager.UpdateArticleToComment(pkid, type, isShow);
                RefreshTouTiaoListCache();
                RefreshArticleCache(pkid);
                LoggerManager.InsertOplog(User.Identity.Name, ObjectType, pkid, isShow == 0 ? "设置为不显示" : "设置为显示");
                if (isShow == 1)
                {
                    MQMessageClient.InsertMessageQueue(1, pkid.ToString(), UserID);
                }
                else
                {
                    MQMessageClient.DeleteMessageQueue(1, pkid.ToString());
                }
                return Content(result ? "1" : "0");
            }
            return Content("0");
        }

        /// <summary>
        /// 保存回复
        /// </summary>
        public int SaveReply(string replyPhone, string replyContent, int replyPraise, int replyPKID, int replyID = 0, int replyType = 3)
        {
            bool isSuccess = false;
            CrmUserCaseManager crm = new CrmUserCaseManager();
            if (string.IsNullOrWhiteSpace(replyPhone))
                return -1;

            using (Tuhu.Service.UserAccount.UserAccountClient client = new Service.UserAccount.UserAccountClient())
            {

                using (var userClicent = new Tuhu.Service.Member.UserClient())
                {

                    var result = client.GetUserByMobile(replyPhone);
                    result.ThrowIfException(true);
                    if (result.Success && result.Result != null)
                    {
                        var user = userClicent.GetUserGradeByUserId(result.Result.UserId);
                        user.ThrowIfException(true);
                        ArticleComment articleComment = new ArticleComment()
                        {
                            PKID = replyPKID,
                            ParentID = replyID,
                            PhoneNum = replyPhone,
                            UserID = result.Result.UserId.ToString("B"),//dt.Rows[0]["u_Imagefile"].ToString()
                            UserHead = !string.IsNullOrEmpty(result.Result.Profile.HeadUrl) ? System.Configuration.ConfigurationManager.AppSettings["DoMain_image"] + result.Result.Profile.HeadUrl : "",
                            CommentContent = replyContent,
                            CommentTime = DateTime.Now,
                            AuditStatus = 0,
                            AuditTime = DateTime.Now,
                            UserName = GetUserName(result.Result.Profile.NickName),
                            UserGrade = user.Result != null ? user.Result.Grade : "",
                            Sort = 0,
                            RealName = result.Result.Profile.UserName,
                            Sex = result.Result.Profile.Gender == Service.UserAccount.Enums.GenderEnum.Female ? "女" : "男",
                            Type = replyType,
                            Praise = replyPraise,
                            TopID = replyType == 0 ? replyID : 0
                        };

                        isSuccess = ArticleManagerLazy.Insert(articleComment);
                        if (isSuccess)
                        {
                            MQMessageClient.InsertMessageQueue(2, replyPKID.ToString(), articleComment.UserID);
                            LoggerManager.InsertOplog(User.Identity.Name, ObjectType, replyPKID, "回答了该问题");
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                        return -1;
                }
            }
        }

        /// <summary>
        /// 用户名称转换
        /// </summary>
        public string GetUserName(string userName = "", string realName = "", string phone = "", string sex = "")
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                if (userName.StartsWith("1") && userName.Length == 11)
                {
                    userName = userName.Substring(0, 3) + "****" + userName.Substring(7, 4);
                }
                return userName;
            }
            else if (!string.IsNullOrWhiteSpace(realName))
            {
                if (realName.StartsWith("1") && realName.Length == 11)
                {
                    realName = realName.Substring(0, 3) + "****" + realName.Substring(7, 4);
                }
                else
                {
                    if (sex == "男")
                    {
                        realName = realName.Substring(0, 1) + "先生";
                    }
                    else if (sex == "女")
                    {
                        realName = realName.Substring(0, 1) + "小姐";
                    }
                    else
                    {
                        realName = realName.Substring(0, 1) + "先生";
                    }
                }
                return realName;
            }
            else if (!string.IsNullOrWhiteSpace(phone))
            {
                return phone.Substring(0, 3) + "****" + phone.Substring(7, 4);
            }
            else
            {
                return "游客";
            }
        }

        #region 文件操作
        /// <summary>
        /// 检测指定目录是否存在
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        /// <returns></returns>
        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        /// <summary>
        /// 创建一个目录
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        public static void CreateDirectory(string directoryPath)
        {
            try
            {
                //如果目录不存在则创建该目录
                if (!IsExistDirectory(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }
            catch { }
        }
        #endregion
    }

    public class WcfClinet<TService> where TService : class
    {
        public TReturn InvokeWcfClinet<TReturn>(Expression<Func<TService, TReturn>> operation)
        {
            var channelFactory = new ChannelFactory<TService>("*");
            TService channel = channelFactory.CreateChannel();
            var client = (IClientChannel)channel;
            client.Open();
            TReturn result = operation.Compile().Invoke(channel);
            try
            {
                if (client.State != CommunicationState.Faulted) { client.Close(); }
            }
            catch
            {
                client.Abort();
            }
            return result;
        }
    }
    public class ArticleView
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
        public string ContentUrl { get; set; }
        public string Source { get; set; }
        public string PublishDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public int ClickCount { get; set; }
        public string RedirectUrl { get; set; }

        public string Category { get; set; }

        public bool CommentIsActive { get; set; }

        public long CommentCount { get; set; }
        public int Vote { get; set; }

        public string ArticleBanner { get; set; }

        public string SmallBanner { get; set; }

        public bool Bestla { get; set; }

        public bool HotArticles { get; set; }

    }
    public class ContentViewModel
    {
        public string title { get; set; }
        public string describe { get; set; }
        public string maxPic { get; set; }
        public string minPic { get; set; }
        public string productName { get; set; }
        public string productPrice { get; set; }
        public string productUrl { get; set; }
    }
    public class FileOperate
    {
        public static string ReadFile(string Path)
        {
            string s = "";
            if (!System.IO.File.Exists(Path))
                s = null;
            else
            {
                StreamReader f2 = new StreamReader(Path, System.Text.Encoding.GetEncoding("gb2312"));
                s = f2.ReadToEnd();
                f2.Close();
                f2.Dispose();
            }

            return s;
        }
    }

    public class MQMessageClient
    {
        static readonly string defaultExchageName = "direct.defaultExchage";

        //发现文章更新消息队列
        static readonly string DiscoveryArticleNotificationQueueName = "notification.DiscoveryArticleModify";
        static readonly RabbitMQProducer DiscoveryArticleRechargeProduceer;

        static MQMessageClient()
        {
            DiscoveryArticleRechargeProduceer = RabbitMQClient.DefaultClient.CreateProducer(defaultExchageName);
            DiscoveryArticleRechargeProduceer.ExchangeDeclare(defaultExchageName);
            DiscoveryArticleRechargeProduceer.QueueBind(DiscoveryArticleNotificationQueueName, defaultExchageName);
        }
        //Type 1提问2回答
        public static void UpdateMessageQueue(int type, string articleId, string content, string image)
        {
            DiscoveryArticleRechargeProduceer.Send(DiscoveryArticleNotificationQueueName, new
            {
                UpdateType = type,
                ActileId = articleId,
                Content = content,
                ContentImage = image,
                Update = true
            });
        }
        public static void DeleteMessageQueue(int type, string articleId)
        {
            DiscoveryArticleRechargeProduceer.Send(DiscoveryArticleNotificationQueueName, new
            {
                DeleteType = type,
                ActileId = articleId,
                Delete = true
            });
        }

        public static void InsertMessageQueue(int type, string articleId, string userId)
        {
            DiscoveryArticleRechargeProduceer.Send(DiscoveryArticleNotificationQueueName, new
            {
                InsertType = type,
                ActileId = articleId,
                UserId = userId,
                Vote = true,
                Insert = true
            });
        }

        public static void UpdateSTMessageQueue(int type, int infoId, int vote)
        {
            DiscoveryArticleRechargeProduceer.Send(DiscoveryArticleNotificationQueueName, new
            {
                Type = type,
                ActileId = infoId,
                Vote = vote
            });
        }

    }

    /// <summary>
    /// 文章标签
    /// </summary>
    public class ZTreeTagModel
    {
        public int key { get; set; }
        public string value { get; set; }
        public int isShow { get; set; }
        public string userData { get; set; }
    }

    public class RefreshResult
    {
        public bool success { get; set; }
        public string msg { get; set; }
    }
}