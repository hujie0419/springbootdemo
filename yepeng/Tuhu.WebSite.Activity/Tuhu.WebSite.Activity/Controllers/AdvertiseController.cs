using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Tuhu.WebSite.Component.SystemFramework;
using Tuhu.WebSite.Component.SystemFramework.Log;
using Tuhu.WebSite.Component.SystemFramework.Models;
using Tuhu.WebSite.Web.Activity.BusinessFacade;
using Tuhu.WebSite.Web.Activity.Models;
using System.Collections;
using Tuhu.WebSite.Web.Activity.DataAccess;
using HuoDong.Common;
using System.Diagnostics;
using System.Data;
using Tuhu.WebSite.Component.SystemFramework.Extensions;
using System.Text.RegularExpressions;
using Tuhu.WebSite.Component.Activity.Common.Cache;
using Tuhu.WebSite.Component.Activity.Common;
using System.Threading.Tasks;
using Tuhu.WebSite.Web.Activity.Common;
using System.Net.Http;
using Tuhu.WebSite.Component.Activity.Business;
using Tuhu.WebSite.Component.Discovery.BusinessData;
using Tuhu.Nosql;
using Tuhu.WebSite.Component.Discovery.Business;

namespace Tuhu.WebSite.Web.Activity.Controllers
{
    public class AdvertiseController : Controller
    {
        private static string DiscoverySite = System.Configuration.ConfigurationManager.AppSettings["DiscoverySite"];
        // GET: Advertise
        public ActionResult Index()
        {
            return View();
        }

        #region 限时抢购（新）
        /// <summary>
        /// 新版本获取限时抢购列表
        /// </summary>
        /// <param name="advertiseID"></param>
        /// <param name="ShowType"></param>
        /// <returns></returns>
        public ActionResult SelectFlashSalesImg(string type)
        {
            Dictionary<string, object> dic = new Dictionary<string, object> { ["Code"] = "0", ["Message"] = "请升级到最新版本！" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 新版本获取限时抢购列表
        /// </summary>
        /// <param name="advertiseID"></param>
        /// <param name="ShowType"></param>
        /// <returns></returns>
        public ActionResult SelectFlashSalesList(string type, string userID, int ShowType = 2)
        {
            Dictionary<string, object> dic = new Dictionary<string, object> { ["Code"] = "0", ["Message"] = "请升级到最新版本！" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 限时抢购明细
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        /// 
        ///[OutputCache(CacheProfile = "ArticleCacheProfile", VaryByParam = "ProductID;VariantID;advertiseID;")]
        public ActionResult SelectFlashSalesDetail(string PID, string userID, string type)
        {
            Dictionary<string, object> dic = new Dictionary<string, object> { ["Code"] = "0", ["Message"] = "请升级到最新版本！" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }



        public ActionResult test()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("key", "1");
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        #endregion 限时抢购（新）


        /// <summary>
        /// 查询所有的类别
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectAllCategory()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var dt = ArticleSystem.SelectAllCategory();
                if (dt != null)
                {
                    dic.Add("Code", "1");
                    dic.Add("Result", dt);
                }
            }
            catch (Exception ex)
            {
                dic.Add("Code", "0");
                dic.Add("Message", "服务器异常");
                //WebLog.LogException(ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询文章用户评论总数
        /// </summary>
        /// <param name="pkId"></param>
        /// <returns></returns>
        public ActionResult SelectCommentCount(string pkId)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                if (string.IsNullOrWhiteSpace(pkId))
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "文章关联ID有误");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                var result = ArticleSystem.SelectCommentCount(pkId);
                dic.Add("Code", "1");
                dic.Add("Count", result);
            }
            catch (Exception ex)
            {
                dic.Add("Code", "0");
                dic.Add("Message", "服务器异常");
                //WebLog.LogException(ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// 添加热度
        ///// </summary>
        ///// <param name="pkId"></param>
        ///// <returns></returns>
        //[CrossSite(Domain = "*")]
        //public ActionResult AddHeat(string pkId)
        //{
        //    var dic = new Dictionary<string, object>();
        //    try
        //    {
        //        Random rd = new Random();
        //        var Num = rd.Next(1, 9);
        //        var result = ArticleSystem.AddHeat(int.Parse(pkId), Num);
        //        if (result > 0)
        //        {
        //            dic.Add("Code", "1");
        //            dic.Add("Hot", Num);
        //        }
        //        else
        //        {
        //            dic.Add("Code", "0");
        //            dic.Add("Message", "失败");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        dic.Add("Code", "0");
        //        dic.Add("Message", "服务器忙，请重试！");
        //        //WebLog.LogException(ex);
        //    }
        //    return Json(dic, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// 图文推送=>评论
        /// </summary>
        /// <param name="ac"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddComment(ArticleCommentModel ac)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                if (ac.PKID == 0)
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "文章关联ID有误");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrWhiteSpace(ac.UserID))
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "用户ID不能为空");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                Guid users;
                if (Guid.TryParse(ac.UserID, out users) == false)
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "用户ID异常");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                var head = "tuhu";
                bool exists = !string.IsNullOrWhiteSpace(ac.UserHead) ? ac.UserHead.Contains(head) : true;
                if (exists)
                {
                    ac.UserHead = ac.UserHead;
                }
                else
                {
                    ac.UserHead = "http://resource.tuhu.cn/" + ac.UserHead;
                }
                if (ac.UserHead.Length < 20)
                {
                    ac.UserHead = "http://resource.tuhu.cn/Image/Product/zhilaohu.png";
                }
                //过滤img 开头
                //ac.UserHead = ac.UserHead.Replace("img", "image");

                ac.UserID = new Guid(ac.UserID).ToString("B");
                ac.UserName = string.IsNullOrWhiteSpace(ac.UserName) ? !string.IsNullOrWhiteSpace(ac.Sex) && !string.IsNullOrWhiteSpace(ac.RealName) ? ac.Sex == "女" ? ac.RealName.Substring(0, 1) + "小姐" : ac.RealName.Substring(0, 1) + "先生" : !string.IsNullOrWhiteSpace(ac.PhoneNum) ? ac.PhoneNum = ac.PhoneNum.Replace(ac.PhoneNum.Substring(3, 4), "****") : "游客" : ac.UserName;

                //过滤非法关键字
                if (HttpClientHelper.AcccessApi(HttpContext.Request, "/Advertise/ValidateCommentContent",
                    new Dictionary<string, string> { { "userId", ac.UserID }, { "content", ac.CommentContent } }) == "false")
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "存在非法词汇");
                    return Json(dic);
                }

                var result = ArticleSystem.AddComment(ac);
                if (result > 0)
                {
                
                    bool Status = false;
                    //添加文章ID和对应UserID
                    ArticleSystem.AddUserReviewOfArticles(ac.UserID, ac.PKID, 2, "0", out Status);
                    Random rd = new Random();
                    var Num = rd.Next(1, 9);
                    ArticleSystem.AddHeat(ac.PKID, Num);

                    var isShowSettingDialog = await IsShowSettingHeadImageDialog(ac.UserID);

                    dic.Add("IsShowSettingDialog", isShowSettingDialog);
                    dic.Add("Code", "1");
                    dic.Add("Message", "评论成功");
                }
            }
            catch (Exception ex)
            {
                dic.Add("Code", "0");
                dic.Add("Message", "服务器忙，请重试！");
                dic.Add("message", ex.Message);
                WebLog.LogException("图文推送=>评论", ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        private async Task<string> IsShowSettingHeadImageDialog(string userId)
        {
            try
            {
                using (var userClient = new Tuhu.Service.UserAccount.UserAccountClient())
                {
                    var userInfo = await userClient.GetUserByIdAsync(new Guid(userId));
                    if (userInfo.Result != null)
                    {
                        var userHeadImage = userInfo.Result.Profile.HeadUrl;
                        if (string.IsNullOrEmpty(userHeadImage))
                        {
                            return "1";
                        }
                        else
                            return "0";
                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException("查询用户信息出错", ex);
                return "0";
            }
        }

        public const string KeyVote = "vote";
        #region 旧图文推送
        /// <summary>
        /// 获取图文推送。前n页取json文件中的数据，后面才访问数据库取数据。
        /// Category  --类别（为空-->查询全部  １、汽车百科　２、驾驶技巧　３保养秘诀　４、必备车品）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Category"></param>
        /// <param name="pIndex"></param>
        /// <param name="pSize"></param>
        /// <returns></returns>
        public ActionResult SelectArticle(string userId, string Category, int pIndex = 1, int pSize = 10)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                List<ArticleModel> artList;
                int totalCount = 0;
                #region 注释
                //if (pIndex * pSize <= 40)//只在Json文件里取前40条。留下10条位置给还未到发布时间的条目<=
                //{
                //    string JsonListUrl = Debugger.IsAttached ? "http://resource.tuhu.cn/app/article/latestarticle.json" : ConfigurationManager.AppSettings["TuWenListJsonURL"].ToString();
                //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(JsonListUrl); //获取前50条最新推送的json文件地址
                //    req.Method = "GET";//提交方式

                //    HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                //    Stream inStream = res.GetResponseStream();
                //    StreamReader sr = new StreamReader(inStream, Encoding.UTF8);
                //    string htmlResult = sr.ReadToEnd();

                //    artList = JsonConvert.DeserializeObject<List<ArticleModel>>(htmlResult);
                //    totalCount = artList.Count;

                //    DateTime dtNow = DateTime.Now;
                //    artList = artList.Where(x => x.PublishDateTime < dtNow).ToList();
                //    List<ArticleModel> artList_Result = new List<ArticleModel>();
                //    for (int i = ((pIndex - 1) * pSize); i < ((pIndex - 1) * pSize) + pSize; i++)
                //    {
                //        if (artList.Count > i)
                //        {

                //            artList_Result.Add(artList[i]);
                //        }
                //        else
                //        {
                //            break;
                //        }
                //    }
                //    artList = artList_Result;
                //    //if (totalCount < 50 || artList_Result.Count < pSize)
                //    //{
                //    //    dic.Add("NoMore", true);
                //    //}
                //    if ((totalCount < 50 && pIndex * pSize >= totalCount) || artList.Count < pSize)//总数量少于50条说明
                //    {
                //        dic.Add("NoMore", true);//说明已到最大页数
                //    }
                //}
                //else
                //{
                #endregion
                if (string.IsNullOrWhiteSpace(Category))
                {
                    Category = null;
                }
                PagerModel pager = new PagerModel(pIndex, pSize);
                artList = ArticleSystem.SelectArticlesForApi(pager, Category).ToList();
                totalCount = pager.TotalItem;
                if (artList.Count < pSize || pIndex * pSize >= totalCount)//如果已经是最后一页
                {
                    dic.Add("NoMore", true);//说明已到最大页数
                }
                //  }
                if (artList != null && artList.Count > 0)
                {
                    using (var ra = RedisAdapter.Create(this.GetType().Name))
                    {
                        // ra.Get(userId + artList[i].PKID, "vote");
                        dic.Add("Code", "1");
                        var list = artList.Select(x => new
                        {
                            BigTitle = string.IsNullOrWhiteSpace(x.BigTitle) ? "" : x.BigTitle,
                            Brief = string.IsNullOrWhiteSpace(x.Brief) ? "" : x.Brief,
                            Catalog = string.IsNullOrWhiteSpace(x.Catalog) ? "" : x.Catalog,
                            ClickCount = string.IsNullOrWhiteSpace(x.ClickCount) ? "0" : x.ClickCount,
                            //x.Content,//内容显示使用url访问静态页面
                            ContentUrl = string.IsNullOrWhiteSpace(x.ContentUrl) ? "" : x.ContentUrl,
                            //CreateDateTime = x.CreateDateTime.ToString("yyyy-MM-dd"),
                            Image = string.IsNullOrWhiteSpace(x.Image) ? "" : x.Image,
                            //LastUpdateDateTime = x.LastUpdateDateTime.ToString("yyyy-MM-dd"),
                            x.PKID,
                            Vote = x.Vote,
                            Voted = userId == null ? 0 : string.IsNullOrEmpty(ra.Get(userId + x.PKID, KeyVote)) ? 0 : 1,//1:已经赞过，0没有赞过
                            PublishDateTime = x.PublishDateTime.ToString("yyyy-MM-dd"),
                            SmallImage = string.IsNullOrWhiteSpace(x.SmallImage) ? "" : x.SmallImage,
                            SmallTitle = string.IsNullOrWhiteSpace(x.SmallTitle) ? "" : x.SmallTitle,
                            Source = string.IsNullOrWhiteSpace(x.Source) ? "" : x.Source,
                            TitleColor = string.IsNullOrWhiteSpace(x.TitleColor) ? "" : x.TitleColor,
                            RedirectUrl = string.IsNullOrWhiteSpace(x.RedirectUrl) ? "" : x.RedirectUrl,
                            CategoryName = string.IsNullOrWhiteSpace(x.CategoryName) ? "" : x.CategoryName,
                            Heat = x.Heat,
                        });
                        List<object> articles = new List<object>();
                        foreach (var i in list)
                        {
                            articles.Add(i);
                        }
                        dic.Add("Article", articles);
                    }
                }
                else
                {
                    dic.Add("Code", "0");
                    //dic.Add("Message", "已查出全部信息");
                }
            }
            catch (Exception ex)
            {
                dic = new Dictionary<string, object>();
                dic.Add("Code", "0");
                //WebLog.LogException(ex);
                //dic.Add("Message", ex.Message);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        //根据关键字获取图文推送
        [OutputCache(CacheProfile = "DefaultCacheProfile", VaryByParam = "keyWord;")]
        public async Task<ActionResult> SelectArticleByWord(string keyWord, string userId)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                List<ArticleModel> artList = ArticleSystem.SelectArticlesByWord(keyWord).ToList();
                if (artList != null && artList.Count > 0)
                {
                    var articleShowMode = await DistributedCacheHelper.SelectArticleShowMode();
                    using (var ra = RedisAdapter.Create(this.GetType().Name))
                    {
                        dic.Add("Code", "1");
                        var list = artList.Select(x => new
                        {
                            ArticleShowMode = articleShowMode,
                            BigTitle = string.IsNullOrWhiteSpace(x.BigTitle) ? "" : x.BigTitle,
                            Brief = string.IsNullOrWhiteSpace(x.Brief) ? "" : x.Brief,
                            Catalog = string.IsNullOrWhiteSpace(x.Catalog) ? "" : x.Catalog,
                            ClickCount = string.IsNullOrWhiteSpace(x.ClickCount) ? "0" : x.ClickCount,
                            //x.Content,//内容显示使用url访问静态页面
                            ContentUrl = string.IsNullOrWhiteSpace(x.ContentUrl) ? "" : x.ContentUrl,
                            //CreateDateTime = x.CreateDateTime.ToString("yyyy-MM-dd"),
                            Image = string.IsNullOrWhiteSpace(x.Image) ? "" : x.Image,
                            //LastUpdateDateTime = x.LastUpdateDateTime.ToString("yyyy-MM-dd"),
                            x.PKID,
                            Vote = x.Vote,
                            Voted = userId == null ? 0 : string.IsNullOrEmpty(ra.Get(userId + x.PKID, KeyVote)) ? 0 : 1,//1:已经赞过，0没有赞过
                            PublishDateTime = x.PublishDateTime.ToString("yyyy-MM-dd"),
                            SmallImage = string.IsNullOrWhiteSpace(x.SmallImage) ? "" : x.SmallImage,
                            SmallTitle = string.IsNullOrWhiteSpace(x.SmallTitle) ? "" : x.SmallTitle,
                            Source = string.IsNullOrWhiteSpace(x.Source) ? "" : x.Source,
                            TitleColor = string.IsNullOrWhiteSpace(x.TitleColor) ? "" : x.TitleColor,
                            RedirectUrl = string.IsNullOrWhiteSpace(x.RedirectUrl) ? "" : x.RedirectUrl,
                            CategoryName = string.IsNullOrWhiteSpace(x.CategoryName) ? "" : x.CategoryName,
                            Heat = x.Heat,
                        });
                        List<object> articles = new List<object>();
                        foreach (var i in list)
                        {
                            articles.Add(i);
                        }
                        dic.Add("Article", articles);
                    }
                }
                else
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "没有找到您要的信息");
                }
            }
            catch (Exception ex)
            {
                dic = new Dictionary<string, object>();
                dic.Add("Code", "0");
                //WebLog.LogException(ex);
                dic.Add("Message", "未查到相关数据");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectNewArticleDate()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                string TuWenNewJsonURL = ConfigurationManager.AppSettings["TuWenNewJsonURL"].ToString();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(TuWenNewJsonURL); //获取最新更新日期（已生效）的json文件地址
                req.Method = "GET";//提交方式

                HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                Stream inStream = res.GetResponseStream();
                StreamReader sr = new StreamReader(inStream, Encoding.UTF8);
                string htmlResult = sr.ReadToEnd();

                List<LatestArticleID> artList = JsonConvert.DeserializeObject<List<LatestArticleID>>(htmlResult);
                if (artList.Count > 0)
                {
                    dic.Add("Code", "1");
                    dic.Add("Article", artList[0].PublishDateTime.ToString("yyyy-MM-dd"));
                }
                else
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "无更新内容");
                }
            }
            catch (Exception ex)
            {
                dic = new Dictionary<string, object>();
                dic.Add("Code", "0");
                //WebLog.LogException(ex);
                dic.Add("message", ex.Message);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        //增加文章点击数
        public ActionResult UpdateArticleClick(int PKID)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                //    int resultValue = ArticleSystem.UpdateArticleClick(PKID);
                //    if (resultValue > 0)
                //    {
                //        dic.Add("Code", "1");
                //    }
                //    else
                //    {
                //        dic.Add("Code", "0");
                //    }
                dic.Add("Code", "1");
            }
            catch (Exception ex)
            {
                dic = new Dictionary<string, object>();
                dic.Add("Code", "0");
                //WebLog.LogException(ex);
                //dic.Add("message", ex.Message);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        #endregion 图文推送

        #region 点赞评论通知
        /// <summary>
        /// 点赞评论通知
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type">1、评论通知  2、点赞通知</param>
        /// <returns></returns>
        public ActionResult SelectVoteAndCommentNotice(string userId, int type, int pIndex = 1, int pSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    dic["Code"] = "0";
                    dic["Erro"] = "请先登录！";
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                userId = new Guid(userId).ToString("B");
                if (type == 1)
                {
                    //评论通知内容
                    var dt = ArticleSystem.SelectCommentNotice(userId).ToList();
                    dic = ArticleHelper.GetCommentNoticeByPage(dt, pIndex, pSize);
                    #region 注释
                    //var CommentNotice = ArticleSystem.SelectCommentNotice(userId).Select(x => new { MyCommentContent = x.MyCommentContent, UserHead = x.UserHead, CommentContent = x.CommentContent, UserName = string.IsNullOrWhiteSpace(x.UserName) ? !string.IsNullOrWhiteSpace(x.Sex) && !string.IsNullOrWhiteSpace(x.RealName) ? x.Sex == "女" ? x.RealName.Substring(0, 1) + "小姐" : x.RealName.Substring(0, 1) + "先生" : !string.IsNullOrWhiteSpace(x.PhoneNum) ? x.PhoneNum = x.PhoneNum.Replace(x.PhoneNum.Substring(3, 4), "****") : "游客" : x.UserName, UserGrade = x.UserGrade, CommentTime = (DateTime.Now.Day - x.CommentTime.Day) == 0 ? "今天 " + x.CommentTime.ToString("HH:mm") : "" + x.CommentTime.ToString("yyyy-MM-dd HH:mm") }).ToList();
                    ////评论通知个数
                    //var CommentNoticeCount = CommentNotice.Count();

                    //dic["CommentNotice"] = CommentNotice;
                    #endregion
                }
                else if (type == 2)
                {
                    //点赞通知内容
                    var dt = ArticleSystem.SelectVoteNotice(userId);
                    dic = ArticleHelper.GetVoteNoticeByPage(dt, pIndex, pSize);
                }
                else
                {
                    dic["Code"] = "0";
                    dic["Message"] = "系统异常！";
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                //dic["Code"] = "1";
            }
            catch (Exception ex)
            {
                WebLog.LogException("点赞评论通知", ex);
                dic["Code"] = "0";

                dic["message"] = ex;
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询点赞(对评论、回答、说说)和回复(对评论、回答、说说)的列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <param name="pIndex"></param>
        /// <param name="pSize"></param>
        /// <returns></returns>
        public ActionResult SelectVotesAndCommentNotices(string userId, int type, int pIndex = 1, int pSize = 10)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    dic["Code"] = "0";
                    dic["Erro"] = "请先登录！";
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                userId = new Guid(userId).ToString("B");
                if (type == 1)
                {
                    PagerModel page = new PagerModel { CurrentPage = pIndex, PageSize = pSize };
                    //评论通知内容
                    var articles = ArticleSystem.SelectCommentNotice(userId, true, page).ToList();
                    HomeController hc = new HomeController();
                    dic = new Dictionary<string, object>();
                    var CommentNotice = new List<ArticleCommentModel>();
                    foreach (var at in articles)
                    {
                        var item = new ArticleCommentModel()
                        {
                            PKID = at.PKID,
                            ID = at.ID,
                            MyCommentContent = at.MyCommentContent,
                            UserGrade = at.UserGrade,
                            UserHead = string.IsNullOrWhiteSpace(at.UserHead) ? GetDefaultUserHeadByUserGrade(at.UserGrade) : at.UserHead,
                            CommentContent = at.CommentContent,
                            Title = at.Title,
                            Type = at.Type,
                            ArticleType = at.ArticleType,
                            IsRead = at.IsRead,
                            ParentID = at.ParentID,
                            TopId = at.TopId,
                            CommentTimeForOne = (DateTime.Now.Day - at.CommentTime.Day) == 0 ? "今天 " + at.CommentTime.ToString("HH:mm") : "" + at.CommentTime.ToString("yyyy-MM-dd HH:mm"),
                            UserName = hc.GetUserName(at.UserName, at.RealName, at.PhoneNum, at.Sex)
                            //UserName = string.IsNullOrWhiteSpace(at.UserName) ? !string.IsNullOrWhiteSpace(at.Sex) && !string.IsNullOrWhiteSpace(at.RealName) ? at.Sex == "女" ? at.RealName.Substring(0, 1) + "小姐" : at.RealName.Substring(0, 1) + "先生" : !string.IsNullOrWhiteSpace(at.PhoneNum) ? at.PhoneNum = at.PhoneNum.Replace(at.PhoneNum.Substring(3, 4), "****") : "游客" : at.UserName
                        };
                        CommentNotice.Add(item);
                    }
                    dic["Code"] = "1";
                    dic["CommentNoticeCount"] = page.TotalItem;
                    dic["CommentPage"] = page.TotalPage;
                    dic["CommentNotice"] = CommentNotice.Select(x => new { PKID = x.PKID, Id = x.ID, IsRead = x.IsRead, ParentId = x.ParentID, TopId = x.TopId, Title = x.Title, MyCommentContent = x.MyCommentContent, UserHead = x.UserHead.Replace("img", "image"), CommentContent = x.CommentContent, UserName = x.UserName, UserGrade = x.UserGrade, CommentTime = x.CommentTimeForOne, Type = x.Type, ArticleType = x.ArticleType }).ToList();
                }
                else if (type == 2)
                {
                    var page = new PagerModel { CurrentPage = pIndex, PageSize = pSize };
                    //点赞通知内容
                    var articles = ArticleSystem.SelectVoteNotices(userId, page);

                    HomeController hc = new HomeController();
                    var CommentNotice = new List<ArticleCommentModel>();
                    foreach (var at in articles)
                    {
                        var item = new ArticleCommentModel()
                        {
                            UserHead = string.IsNullOrWhiteSpace(at.UserHead) ? GetDefaultUserHeadByUserGrade(at.UserGrade) : at.UserHead,
                            CommentContent = at.CommentContent,
                            Item = at.Item,
                            UserName = hc.GetUserName(at.UserName, at.RealName, at.PhoneNum, at.Sex),
                            Type = at.Type,
                            PKID = at.PKID,
                            ArticleType = at.ArticleType,
                            ParentID = at.ParentID,
                            ID = at.ID,
                            Title = at.Title,
                            IsRead = at.IsRead,
                            OperateTime = at.OperateTime
                            //UserName = string.IsNullOrWhiteSpace(at.UserName) ? !string.IsNullOrWhiteSpace(at.Sex) && !string.IsNullOrWhiteSpace(at.RealName) ? at.Sex == "女" ? at.RealName.Substring(0, 1) + "小姐" : at.RealName.Substring(0, 1) + "先生" : !string.IsNullOrWhiteSpace(at.PhoneNum) ? at.PhoneNum = at.PhoneNum.Replace(at.PhoneNum.Substring(3, 4), "****") : "游客" : at.UserName
                        };
                        CommentNotice.Add(item);
                    }
                    dic["Code"] = "1";
                    dic["VotePage"] = page.TotalPage;
                    dic["CommentNoticeCount"] = page.TotalItem;
                    dic["MyComment"] = CommentNotice.Select(x => new { UserName = x.UserName, Title = x.Title, PKID = x.PKID, Id = x.ID, ParentID = x.ParentID, IsRead = x.IsRead, CommentContent = x.CommentContent, VoteCount = x.Item.Count(), Type = x.Type, ArticleType = x.ArticleType, item = x.Item.Select(i => i.UserHead).Take(7) }).ToList();
                }
                else
                {
                    dic["Code"] = "0";
                    dic["Message"] = "系统异常！";
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                //dic["Code"] = "1";
            }
            catch (Exception ex)
            {
                WebLog.LogException("查询点赞(对评论、回答、说说)和回复(对评论、回答、说说)的列表", ex);
                dic["Code"] = "0";
                dic["message"] = ex;
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        private static string GetDefaultUserHeadByUserGrade(string userGrade)
        {
            string userHead;
            switch (userGrade)
            {
                case "银卡会员":
                case "V1":
                    userHead = "http://resource.tuhu.cn/Image/Product/bulaohu.png";
                    break;
                case "金卡会员":
                case "V2":
                    userHead = "http://resource.tuhu.cn/Image/Product/mulaohu.png";
                    break;
                case "白金卡会员":
                case "V3":
                    userHead = "http://resource.tuhu.cn/Image/Product/tielaohu.png";
                    break;
                case "黑金卡会员":
                case "V4":
                    userHead = "http://resource.tuhu.cn/Image/Product/tonglaohu.png";
                    break;
                default:
                    userHead = "http://resource.tuhu.cn/Image/Product/zhilaohu.png";
                    break;
            }
            return userHead;
        }
        /// <summary>
        /// 发送已读消息(评论、回答)通知
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<ActionResult> ReadCommentNotic(int commentId)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var result = await ArticleSystem.ReadCommentNotic(commentId);
                dic["Code"] = result.ToString();
            }
            catch (Exception ex)
            {
                WebLog.LogException("根据用户Id和文章Id(问题Id)获得关注人列表", ex);
                dic["Code"] = "0";
                dic["message"] = ex;
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发送已读点赞消息通知
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> ReadPraiseNotice(int id, int praiseType)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var result = await ArticleSystem.ReadPraiseNotice(id, praiseType);
                dic["Code"] = result.ToString();
            }
            catch (Exception ex)
            {
                WebLog.LogException("发送已读点赞消息通知", ex);
                dic["Code"] = "0";
                dic["message"] = ex;
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 我的收藏夹For喜欢的文章
        /// <summary>
        /// 我的个人主页收藏夹--喜欢的文章
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Category">5、我喜欢的</param>
        /// <param name="pIndex"></param>
        /// <param name="pSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectNewArticleForApiMyFavorite(string userId, string version, string Category = "5", int pIndex = 1, int pSize = 10)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                List<ArticleModel> artList;
                int totalCount = 0;
                PagerModel pager = new PagerModel(pIndex, pSize);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    dic.Add("Code", "1");
                    dic.Add("Erro", "用户ID为空！");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                userId = new Guid(userId).ToString("d");
                //列表页
                artList = ArticleSystem.SelectNewArticlesForApi(pIndex, pSize, Category, userId, version, out totalCount).ToList();
                var articleShowMode = await DistributedCacheHelper.SelectArticleShowMode();
                artList.ForEach(article =>
                {
                    article.ContentUrl = (articleShowMode == "New" || article.Type == 5) ? DomainConfig.FaXian+ "/react/findDetail/?bgColor=%23ffffff&textColor=%23333333&id=" + article.PKID : article.ContentUrl;
                });

                var result = new ArticleHelper().GetArticle(artList, userId);
                dic.Add("ArticleCount", totalCount);//已到最后一页
                dic.Add("Article", new { Result = result });
                dic["Code"] = "1";
            }
            catch (Exception ex)
            {
                WebLog.LogException("我的个人主页收藏夹--喜欢的文章", ex);
                dic["Code"] = "0";
                dic.Add("Message", ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 新版图文推送
        /// <summary>
        /// 查询所有文章
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="Category"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OutputCache(Duration = 15)]
        public ActionResult SelectNewArticle(string userId, string version, string Category = "1", int pIndex = 1, int pSize = 10)
        {
            Response.Cache.SetOmitVaryStar(true);
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                List<ArticleModel> artList;
                List<ArticleModel> ToptList;
                int totalCount = 0; //总个数
                if (Category == "8" && pIndex == 1)
                {
                    //特推主题
                    #region 特推主题
                    var API_Cache_ToptList = EnyimMemcachedContext.MemcachedContext.Get<string>("API_Cache_ToptList");
                    if (API_Cache_ToptList != null)
                    {
                        ToptList = JsonConvert.DeserializeObject<List<ArticleModel>>(API_Cache_ToptList);
                    }
                    else
                    {
                        ToptList = ArticleSystem.SelectTopics().ToList();
                        EnyimMemcachedContext.MemcachedContext.Set("API_Cache_ToptList", JsonConvert.SerializeObject(ToptList), DateTime.Now.AddMinutes(5));
                    }
                    if (ToptList != null && ToptList.Count > 0)
                    {
                        dic.Add("Topics", ArticleHelper.GetTopArticle(ToptList, userId));
                    }
                    #endregion
                }
                var dics = new Dictionary<string, object>();
                //列表页
                if (Category == "5" || Category == "6")
                {
                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        dic.Add("Code", "0");
                        return Json(dic, JsonRequestBehavior.AllowGet);
                    }
                    userId = new Guid(userId).ToString("d");
                    artList = ArticleSystem.SelectNewArticlesForApi(pIndex, pSize, Category, userId, version, out totalCount).ToList();
                }
                else
                {
                    var API_Cache_Article = EnyimMemcachedContext.MemcachedContext.Get<string>("API_Cache_Article" + pIndex + "_pIndex" + pSize + "_pSize" + Category + "_Category" + version + "_version");
                    if (API_Cache_Article != null)
                    {
                        var list = JsonConvert.DeserializeObject<Dictionary<string, object>>(API_Cache_Article);
                        artList = JsonConvert.DeserializeObject<List<ArticleModel>>(list["artList"].ToString());
                        totalCount = Convert.ToInt32(list["totalCount"].ToString());
                    }
                    else
                    {
                        //缓存中没有则取数据并写入数据
                        artList = ArticleSystem.SelectNewArticlesForApi(pIndex, pSize, Category, userId, version, out totalCount).ToList();
                        dics.Add("artList", artList);
                        dics.Add("totalCount", totalCount);
                        EnyimMemcachedContext.MemcachedContext.Set("API_Cache_Article" + pIndex + "_pIndex" + pSize + "_pSize" + Category + "_Category" + version + "_version", JsonConvert.SerializeObject(dics), DateTime.Now.AddMinutes(1));
                    }
                    #region 暂时注释
                    //if (HttpRuntime.Cache["API_Cache_Article" + pIndex + "_pIndex" + pSize + "_pSize" + Category + "_Category" + version + "_version"] != null)
                    //{
                    //    var artLists = HttpRuntime.Cache["API_Cache_Article" + pIndex + "_pIndex" + pSize + "_pSize" + Category + "_Category" + version + "_version"] as Dictionary<string, object>;
                    //    artList = artLists["artList"] as List<ArticleModel>;
                    //    totalCount = Convert.ToInt32(artLists["totalCount"]);
                    //}
                    //else
                    //{
                    //    //缓存中没有则取数据并写入数据
                    //    artList = ArticleSystem.SelectNewArticlesForApi(pIndex, pSize, Category, userId, version, out totalCount).ToList();
                    //    dics.Add("artList", artList);
                    //    dics.Add("totalCount", totalCount);

                    //    HttpRuntime.Cache.Insert("API_Cache_Article" + pIndex + "_pIndex" + pSize + "_pSize" + Category + "_Category" + version + "_version", dics, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
                    //}
                    #endregion
                }
                dic.Add("totalCount", totalCount);
                if (artList.Count < pSize || pIndex * pSize >= totalCount)//如果已经是最后一页
                {
                    dic.Add("NoMore", true);//已到最后一页
                }
                if (artList != null && artList.Count > 0)
                {
                    dic.Add("Article", new ArticleHelper().GetArticle(artList, userId));
                    dic.Add("Code", "1");
                }
                else
                {
                    dic.Add("Code", "0");
                }
            }
            catch (Exception ex)
            {
                dic.Add("Code", "0");
                //WebLog.LogException(ex);
                dic.Add("Message", "服务器异常");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据关键字获取图文推送
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="keyWord">搜索关键词</param>
        /// <param name="Versions">版本号</param>
        /// <param name="Channel">渠道</param>
        /// <param name="version">判断新老版本  过度词  new 代表新版本</param>
        /// <param name="pIndex"></param>
        /// <param name="pSize"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SelectNewArticleByWord(string userId, string keyWord, string Versions, string Channel, string version, int pIndex = 1, int pSize = 10)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                List<ArticleModel> artList;
                int totalCount = 0;
                PagerModel pager = new PagerModel(pIndex, pSize);
                //keyWord = keyWord.Replace("[^\\u0000-\\uFFFF]", "");
                keyWord = Regex.Replace(keyWord, @"\p{Cs}", "");
                if (string.IsNullOrWhiteSpace(keyWord))
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "未找到相关数据");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                artList = ArticleSystem.SelectNewArticlesByWord(pager, keyWord, version).ToList();
                ArticleSystem.AddSeekKeyWord(keyWord, Versions, Channel);
                totalCount = pager.TotalItem;

                if (artList.Count < pSize || pIndex * pSize >= totalCount)//如果已经是最后一页
                {
                    dic.Add("NoMore", true);//已到最后一页
                }
                //计算总页数 
                int count = totalCount % pSize == 0 ? totalCount / pSize : totalCount / pSize + 1;
                dic.Add("PageIndexCount", count);
                if (artList != null && artList.Count > 0)
                {
                    dic.Add("Code", "1");
                    dic.Add("Article", new ArticleHelper().GetArticle(artList, userId));

                }
                else
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "未找到相关数据");
                }
            }
            catch (Exception ex)
            {
                dic.Add("Code", "0");
                //WebLog.LogException(ex);
                //dic.Add("Message", "服务器异常");
                dic.Add("Message", ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 首页汽车头条
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> SelectCarMadeHeadlines()
        {
            var dic = new Dictionary<string, object>();
            try
            {
                List<ArticleModel> artList;
                using (var client = CacheHelper.CreateCacheClient("CarMadeHeadlines"))
                {
                    artList=(await client.GetOrSetAsync("API_Cache_ArticleCarMadeHeadlines",()=> ArticleSystem.SelectCarMadeHeadlines())).Value; 
                }
               
                //var list = EnyimMemcachedContext.MemcachedContext.Get("API_Cache_ArticleCarMadeHeadlines");
                //if (list != null)
                //{
                //    artList = JsonConvert.DeserializeObject<List<ArticleModel>>(list.ToString());
                //}
                //else
                //{
                //    //var versionNumber = Request.Headers.Get("VersionCode");
                //    //var version = Request.Headers.Get("version");

                //    //if (((string.IsNullOrEmpty(versionNumber) == false && (string.Compare(versionNumber, "50") < 0) ||
                //    //   (string.IsNullOrEmpty(version) == false && string.Compare(version, "iOS 3.4.5") < 0))))
                //    //{
                //    //    artList = ArticleSystem.SelectCarMadeHeadlines().ToList();
                //    //}
                //    //else
                //    //{
                //    //    artList = ArticleSystem.SelectCarMadeHeadlinesVersion1().ToList();
                //    //    artList.ForEach(article =>
                //    //    {
                //    //        article.ContentUrl = DiscoverySite + "/Article/Detail?Id=" + article.RelatedArticleId;
                //    //    });
                //    //}

                //    artList = ArticleSystem.SelectCarMadeHeadlines().ToList();
                //    var articleShowMode = await DistributedCacheHelper.SelectArticleShowMode();
                //    artList.ForEach(a=>
                //    {
                //        //
                //        a.ContentUrl =  (articleShowMode == "New" || a.Type == 5) ? DomainConfig.FaXian+ "/react/findDetail/?bgColor=%23ffffff&textColor=%23333333&id=" + a.PKID : a.ContentUrl;
                //    });

                //    EnyimMemcachedContext.MemcachedContext.Set("API_Cache_ArticleCarMadeHeadlines2", JsonConvert.SerializeObject(artList), DateTime.Now.AddMinutes(10));
                //}
                if (artList != null && artList.Count > 0)
                {
                    dic.Add("Article", new ArticleHelper().GetArticle(artList, null));
                    dic.Add("Code", "1");
                }
                else
                {
                    dic.Add("Code", "0");
                }
            }
            catch (Exception ex)
            {
                dic.Add("Code", "0");
                dic.Add("message", ex.Message);
                WebLog.LogException("首页汽车头条", ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发现文章小红点
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectArticleIsNew()
        {
            var dic = new Dictionary<string, object>();
            try
            {
                string ArticlePKID = null;
                var list = EnyimMemcachedContext.MemcachedContext.Get("API_Cache_ArticleIsNew");
                if (list != null)
                {
                    ArticlePKID = list.ToString();
                }
                else
                {
                    ArticlePKID = ArticleSystem.SelectArticleIsNew();
                    EnyimMemcachedContext.MemcachedContext.Set("API_Cache_ArticleIsNew", ArticlePKID, DateTime.Now.AddMinutes(10));
                }
                if (!string.IsNullOrWhiteSpace(ArticlePKID))
                {
                    dic["Code"] = "1";
                    dic["ArticlePKID"] = Convert.ToInt32(ArticlePKID);
                }
                else
                {
                    dic["Code"] = "0";
                }
            }
            catch (Exception ex)
            {
                dic.Add("Code", "0");
                dic.Add("message", ex.Message);
                WebLog.LogException("发现文章小红点", ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询出搜索热门的前12个关键词
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectHotKeyWords()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var dt = ArticleSystem.SelectHotKeyWord().ToList();
                if (dt != null)
                {
                    dic.Add("Code", "1");
                    dic.Add("Result", dt);
                }
                else
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "未查到相关数据");
                }
            }
            catch (Exception ex)
            {
                dic.Add("Code", "0");
                //WebLog.LogException(ex);
                dic.Add("Message", "服务器异常");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新版查询所有的类别
        /// </summary>
        /// <returns></returns>
        //[OutputCache(Duration = 180)]
        public ActionResult SelectAllNewCategory(string userId)
        {
            Response.Cache.SetOmitVaryStar(true);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                //IOS版本号
                var version = Request.Headers.Get("version");
                var dics = new Dictionary<string, object>();
                List<ArticleModel> ToptList; //特推主题
                List<CategoryListModel> Category;  //文章类别
                var IOS = !string.IsNullOrWhiteSpace(version) ? string.Compare(version, "iOS 3.0.0") : 1; //IOS版本号
                if (EnyimMemcachedContext.MemcachedContext.Get("API_Cache_ArticleCategory") != null)
                {
                    var list = JsonConvert.DeserializeObject<Dictionary<string, object>>(EnyimMemcachedContext.MemcachedContext.Get<string>("API_Cache_ArticleCategory"));
                    Category = JsonConvert.DeserializeObject<List<CategoryListModel>>(list["Category"].ToString());
                    ToptList = JsonConvert.DeserializeObject<List<ArticleModel>>(list["Topics"].ToString());
                }
                else
                {
                    Category = ArticleSystem.SelectAllNewCategory().ToList();
                    ToptList = ArticleSystem.SelectTopics().ToList();
                    dics["Category"] = Category;
                    dics["Topics"] = ToptList;
                    EnyimMemcachedContext.MemcachedContext.Set("API_Cache_ArticleCategory", JsonConvert.SerializeObject(dics), DateTime.Now.AddMinutes(10));
                }
                if (IOS < 0)
                {
                    Category = Category.Where(y => y.CategoryName != "虎友口碑").OrderByDescending(x => x.Sort).ToList();
                }
                else
                {
                    Category = Category.OrderByDescending(x => x.Sort).ToList();
                }

                if (Category != null)
                {
                    dic.Add("Code", "1");
                    dic.Add("Result", Category);
                }
                if (IOS < 0)
                {
                    if (ToptList != null && ToptList.Count > 0)
                    {
                        dic.Add("Topics", ArticleHelper.GetTopArticle(ToptList, userId));
                    }
                }
            }
            catch (Exception ex)
            {
                dic.Clear();
                dic.Add("Code", "0");
                dic.Add("Message", "服务器异常");
                WebLog.LogException("新版查询所有的类别", ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 增加文章点击数
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public  ActionResult AddArticleClickNew(int PKID, string callback)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>() { ["Code"] = "1" };
            //try
            //{
            //    //ArticleSystem.AddReadingRecord(null, 3, "0", PKID);
            //    //int Result = ArticleSystem.UpdateArticleClick(PKID);
            //    //if (Result > 0)
            //    //{
            //    //    dic["Code"] = "1";
            //    //}
            //    dic["Code"] = "1";

            //    //收藏文章时候，同步到新的发现系统中

            //    //var relatedArticleId = await DiscoverBLL.SelectAssociationArticeId(Convert.ToInt16(PKID));
            //    //if (relatedArticleId > 0)
            //    //{
            //    //    //同步收藏状态到新发现中
            //    //    var httpClient = new HttpClient();

            //    //    var deviceId = Request.Headers.Get("DeviceID");

            //    //    Dictionary<string, string> parameters = new Dictionary<string, string>();


            //    //    parameters.Add("Id", relatedArticleId.ToString());
            //    //    //parameters.Add("UserId", UserId);
            //    //    parameters.Add("DeviceId", deviceId);

            //    //    System.Net.Http.HttpContent content = new System.Net.Http.FormUrlEncodedContent(parameters);
            //    //    System.Net.Http.HttpResponseMessage response = await httpClient.PostAsync("http://faxian.tuhu.test/Article/AddFavoriate", content);
            //    //    var responseContent = await response.Content.ReadAsStringAsync();
            //    //}

            //}
            //catch (Exception ex)
            //{
            //    dic.Clear();
            //    dic["Code"] = "0";
            //    dic["Message"] = "服务器异常";
            //    WebLog.LogException("增加文章点击数", ex);
            //}
            return this.Jsonp(callback, dic);
        }

        /// <summary>
        /// 查询点赞状态
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public ActionResult SelectVoteState(string PKID, string UserId)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                if (string.IsNullOrWhiteSpace(UserId))
                {
                    //阅读文章后插入记录
                    ArticleSystem.AddReadingRecord(UserId, 3, "0", int.Parse(PKID));
                    dic.Add("Status", false);
                    dic.Add("Code", "1");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                UserId = new Guid(UserId).ToString("d");
                //ArticleSystem.UpdateArticleClick(int.Parse(PKID));
                //阅读文章后插入记录
                //ArticleSystem.AddReadingRecord(UserId, 3, "0", int.Parse(PKID));
                //查询点赞状态
                var Status = ArticleSystem.SelectVoteState(UserId, int.Parse(PKID));
                dic.Add("Status", Status);
                dic.Add("Code", "1");
            }
            catch (Exception ex)
            {
                dic.Add("Status", false);
                dic.Add("Code", "0");
                dic.Add("Message", "服务器异常");
                //WebLog.LogException(ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询相关文章，以及置顶评论
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="Category"></param>
        /// <returns></returns>
        public ActionResult SelectNewsInfo(int PKID, int Category, string callback)
        {
            try
            {
                var resultData = ArticleSystem.SelectNewsInfo(PKID, Category);
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(resultData);
                return this.Jsonp(callback, jsonData);
            }
            catch (Exception ex)
            {
                return this.Jsonp(callback, "-1");
            }
        }

        public ActionResult SelectNewsUrl(int pkid, string callback)
        {
            try
            {
                if (pkid > 0)
                {
                    string key = "SelectNewsUrl" + pkid, resultData = "";
                    if (EnyimMemcachedContext.MemcachedContext.Get(key) != null)
                    {
                        resultData = EnyimMemcachedContext.MemcachedContext.Get<string>(key);
                    }
                    else
                    {
                        resultData = ArticleSystem.SelectNewsUrl(pkid);
                        EnyimMemcachedContext.MemcachedContext.Set<string>(key, resultData, DateTime.Now.AddMinutes(1));
                    }
                    return this.Jsonp(callback, resultData);
                }
                return this.Jsonp(callback, "-1");
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex.ToString(), ex);
                return this.Jsonp(callback, "-1");
            }
        }

        /// <summary>
        /// 查询文章详情以及相关文章
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public ActionResult SelectArticlesAndRelates(int PKID)
        {
            try
            {
                var resultData = ArticleSystem.SelectArticlesAndRelates(PKID);
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(resultData);
                return Content(jsonData);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        /// <summary>
        /// 查询评论页面
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="UserID"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectAiticleCommentNew(int PKID, string UserID, int PageIndex = 1)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var PageSize = 10;
                int TotalCount = 0;
                UserID = !string.IsNullOrWhiteSpace(UserID) ? new Guid(UserID).ToString("B") : null;
                HomeController hc = new HomeController();

                //最新评论
                var comments = ArticleCommentSystem.GetArticleCommentByPKID(PKID, UserID, out TotalCount, PageIndex, PageSize)
                    .Select(a => new
                    {
                        a.UserID,
                        a.ID,
                        a.AuditStatus,
                        a.Category,
                        a.CommentContent,
                        CommentTime = (DateTime.Now.Day - a.CommentTime.Day) == 0 ? "今天 " + a.CommentTime.ToString("HH:mm") : "" + a.CommentTime.ToString("yyyy-MM-dd HH:mm"),
                        CommentNum = ArticleCommentSystem.CountComment(a.ID),
                        PraiseNum = ArticleCommentSystem.CountPraise(a.ID),
                        a.PhoneNum,
                        a.Title,
                        UserHead = GetUserHeadHandle(a.UserHead),
                        //UserName = string.IsNullOrWhiteSpace(a.UserName) ? !string.IsNullOrWhiteSpace(a.Sex) && !string.IsNullOrWhiteSpace(a.RealName) ? a.Sex == "女" ? a.RealName.Substring(0, 1) + "小姐" : a.RealName.Substring(0, 1) + "先生" : !string.IsNullOrWhiteSpace(a.PhoneNum) ? a.PhoneNum = a.PhoneNum.Replace(a.PhoneNum.Substring(3, 4), "****") : "游客" : a.UserName,
                        UserName = hc.GetUserName(a.UserName, a.RealName, a.PhoneNum, a.Sex),
                        a.UserGrade,
                        a.floor,
                        ParentName = ArticleHelper.GetUserNameByID(a.ParentID),
                        IsPraise = !string.IsNullOrWhiteSpace(UserID) ? hc.GetIsPraise(a.ID, UserID) : 0
                    });
                var CommentsPage = (TotalCount + PageSize - 1) / PageSize;
                if (TotalCount >= 10)
                {
                    if (PageIndex == 1)
                    {
                        //热门评论
                        var HotComment = ArticleCommentSystem.GetArticleCommentTop3(PKID, UserID)
                        .Select(a => new
                        {
                            a.UserID,
                            a.ID,
                            a.AuditStatus,
                            a.Category,
                            a.CommentContent,
                            CommentTime = hc.formatCommentTime(a.CommentTime),
                            CommentNum = hc.CountComment(a.ID),
                            PraiseNum = hc.CountPraise(a.ID),
                            a.PhoneNum,
                            a.Title,
                            UserHead = GetUserHeadHandle(a.UserHead),
                            UserName = hc.GetUserName(a.UserName, a.RealName, a.PhoneNum, a.Sex),
                            a.UserGrade,
                            a.floor,
                            a.ParentID,
                            ParentName = ArticleHelper.GetUserNameByID(a.ParentID),
                            IsPraise = !string.IsNullOrWhiteSpace(UserID) ? hc.GetIsPraise(a.ID, UserID) : 0
                        });
                        dic["HotComment"] = HotComment;
                    }
                }
                else
                {
                    dic["HotComment"] = new ArticleCommentModel[0];
                }
                Article articleModel = null;
                using (var reclient = CacheHelper.CreateCacheClient("ArticleDetail"))
                {
                    var result = await reclient.GetOrSetAsync("DetailById/" + PKID, () => ArticleBll.GetArticleDetailById(PKID), TimeSpan.FromHours(1));
                    if (result.Value != null)
                        articleModel = result.Value;
                }
                dic["ArticleTitle"] = articleModel?.SmallTitle;
                dic["ArticleDetailUrl"] = $"{DomainConfig.FaXian}/react/findDetail/?bgColor=%23ffffff&textColor=%23333333&id={PKID}";
                dic["Code"] = "1";
                dic["CommentsPage"] = CommentsPage;
                dic["CommentNew"] = comments;
            }
            catch (Exception ex)
            {
                dic["Code"] = "0";
                dic["Message"] = "服务器异常";
                WebLog.LogException("查询评论页面", ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>文章评论点赞
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> InsertCommentVote(CommentPraise model)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                if (string.IsNullOrWhiteSpace(model.UserId))
                {
                    dic["Code"] = "0";
                    dic["Message"] = "请先登录！";
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                if (!string.IsNullOrWhiteSpace(model.RealName) && !string.IsNullOrWhiteSpace(model.Sex))
                {
                    if (model.Sex.Trim().Equals("女"))
                    {
                        model.RealName = model.RealName.Substring(0, 1) + "小姐";
                    }
                    else
                    {
                        model.RealName = model.RealName.Substring(0, 1) + "先生";
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.PhoneNum))
                {
                    model.PhoneNum = model.PhoneNum.Substring(0, 3) + "****" + model.PhoneNum.Substring(7, 4);
                }
                model.UserId = new Guid(model.UserId).ToString("B");

                int articleId;
                int status;
                int Result = ArticleCommentSystem.InsertCommentPraiseNew(model, out status, out articleId);
                if (Result > 0)
                {
                    dic["Code"] = "1";
                    dic["VoteState"] = status == 0 ? false : true;

                    //安卓请求
                    var androidType = Request.Headers.Get("VersionCode");
                    //IOS请求
                    var iosType = Request.Headers.Get("version");

                    if (model.PKID <= 0 && articleId > 0)
                        model.PKID = articleId;
                    //点赞通知
                    if (model.VoteState == 1 && string.IsNullOrEmpty(model.TargetUserId) == false && model.PKID > 0)
                    {
                        //查询当前用户的个人信息
                        var userResultJson = HttpClientHelper.AcccessApi(HttpContext.Request, "/User/GetUsersInfoForAttention",
                                                         new Dictionary<string, string> { { "userIds", string.Join(",", model.UserId) } });
                        var userInfo = JsonConvert.DeserializeObject<List<MyAttentionUserList>>(userResultJson).FirstOrDefault();


                        if (userInfo != null)
                        {
                            model.UserName = userInfo.UserName;
                            model.UserHead = userInfo.UserHead;
                            model.UserGrade = userInfo.UserGrade;
                            model.PhoneNum = userInfo.PhoneNumber;
                        }
                        await UtilityService.PushArticleMessage(model.TargetUserId, 673, model.PKID, model.UserName);

                        //var userName = string.IsNullOrEmpty(model.UserName) ? GetUserName(model.UserName, model.RealName, "") : model.UserName;
                        //var androidRedictKey = "cn.TuHu.Activity.Found.DiscoveryH5Activity";
                        //var androidRedictValue = "[{'PKID':" + model.PKID + ",'Category':'','Title':'','keyboard':1,'AddClick':false}]";
                        //var iosRedictKey = "THDiscoverCommentVC";
                        //var iosRedictValue = "{\"pkid\":" + model.PKID + "}";
                        //HttpClientHelper.AcccessApi(HttpContext.Request, "/User/InsertPraiseNotice",
                        //new Dictionary<string, string> {
                        //     { "userHead",string.IsNullOrEmpty(model.UserHead) ? GetDefaultUserHeadByUserGrade(model.UserGrade) : DomainConfig.ImageSite+model.UserHead},
                        //            { "userName",userName},
                        //    { "userId", string.Join(",", model.TargetUserId) },
                        //     { "phoneNumber",model.PhoneNum},
                        //    { "id", model.CommentId.ToString() },
                        //    { "news",  "赞了你的评论" },
                        //    { "androidKey",androidRedictKey },
                        //        { "iosKey",iosRedictKey },
                        //         { "androidValue",androidRedictValue },
                        //         { "iosValue",iosRedictValue }

                        //});
                    }
                }
                else
                {
                    dic["Code"] = "0";
                    dic["Message"] = "服务器异常";
                }
            }
            catch (Exception ex)
            {
                dic["Code"] = "0";
                dic["Message"] = "服务器异常";
                WebLog.LogException("文章评论点赞", ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        public string GetUserHeadHandle(string userHead)
        {
            string headImg = "";
            if (userHead.Contains("zhilaohu.png"))
            {
                headImg = userHead;
            }
            else
            {
                headImg = userHead.Contains("http") ? userHead : userHead.GetImageUrl();

                if (headImg.Contains("image") && !headImg.Contains("image.tuhu.cn"))
                    headImg = headImg.Replace("image", "img");
                
                //int index = userHead.IndexOf('/', 10);
                //int len = userHead.Length - index;
                //string childStr = userHead.Substring(index, len);
                //headImg = "http://image.tuhu.cn" + childStr;
            }
            return headImg;
        }

        /// <summary>
        /// 转译显示用户名
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="realName"></param>
        /// <param name="phone"></param>
        /// <param name="sex"></param>
        /// <returns></returns>
        public string GetUserName(string userName, string realName, string phone, string sex = "")
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
                return "途虎用户";
            }
        }

        /// <summary>
        /// 查询当前用户是否收藏过指定文章
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ActionResult IsNewsForLikeNews(int PKID, string UserId, string callback)
        {
            try
            {
                var resultData = ArticleSystem.IsNewsForLikeNews(PKID, UserId);
                return this.Jsonp(callback, resultData == true ? 1 : 0);
            }
            catch (Exception ex)
            {
                return this.Jsonp(callback, "-1");
            }
        }

        /// <summary>
        /// 文章点赞，与取消点赞
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="PKID"></param>
        /// <param name="Vote"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public ActionResult AddVoteNewsJsonp(string UserId, string PKID, string Vote, string callback)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                if (string.IsNullOrWhiteSpace(UserId))
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "请登录后重试！");
                    return this.Jsonp(callback, Newtonsoft.Json.JsonConvert.SerializeObject(dic));
                }
                if (string.IsNullOrWhiteSpace(PKID))
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "文章编号不能为空！");
                    return this.Jsonp(callback, Newtonsoft.Json.JsonConvert.SerializeObject(dic));
                }
                Guid users;
                if (Guid.TryParse(UserId, out users) == false)
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "用户ID异常");
                    return this.Jsonp(callback, Newtonsoft.Json.JsonConvert.SerializeObject(dic));
                }
                bool Status = false;
                //if (!UserId.Contains("{") || !UserId.Contains("}"))
                //{
                //    UserId = "{" + UserId + "}";
                //}
                UserId = new Guid(UserId).ToString("B");
                int Result = ArticleSystem.AddUserReviewOfArticles(UserId, int.Parse(PKID), 1, Vote, out Status);
                if (Result > 0)
                {
                    dic.Add("Code", "1");
                    dic.Add("Status", Status);
                }
                else
                {
                    dic.Add("Message", "失败");
                }
            }
            catch (Exception ex)
            {
                dic.Add("Code", "0");
                //WebLog.LogException(ex);
                dic.Add("Message", "服务器异常");
            }
            return this.Jsonp(callback, Newtonsoft.Json.JsonConvert.SerializeObject(dic));
        }

        /// <summary>
        /// 点赞和取消点赞
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="PKID"></param>
        /// <param name="Vote"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddVote(string UserId, string PKID, string Vote = "1")
        {
            var dic = new Dictionary<string, object>();
            try
            {
                if (string.IsNullOrWhiteSpace(UserId))
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "请登录后重试！");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrWhiteSpace(PKID))
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "文章编号不能为空！");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                Guid users;
                if (Guid.TryParse(UserId, out users) == false)
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "用户ID异常");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                bool Status = false;
                //if (!UserId.Contains("{") || !UserId.Contains("}"))
                //{
                //    UserId = "{" + UserId + "}";
                //}
                UserId = new Guid(UserId).ToString("B");
                int Result = ArticleSystem.AddUserReviewOfArticles(UserId, int.Parse(PKID), 1, Vote, out Status);
                if (Result > 0)
                {
                    dic.Add("Code", "1");
                    dic.Add("Status", Status);

                    //收藏文章时候，同步到新的发现系统中

                    //var relatedArticleId = await DiscoverBLL.SelectAssociationArticeId(Convert.ToInt16(PKID));
                    //if (relatedArticleId > 0)
                    //{
                    //    //同步收藏状态到新发现中
                    //    var httpClient = new HttpClient();

                    //    var deviceId = Request.Headers.Get("DeviceID");

                    //    Dictionary<string, string> parameters = new Dictionary<string, string>();


                    //    parameters.Add("Id", relatedArticleId.ToString());
                    //    parameters.Add("UserId", UserId);
                    //    parameters.Add("DeviceId", deviceId);

                    //    System.Net.Http.HttpContent content = new System.Net.Http.FormUrlEncodedContent(parameters);
                    //    System.Net.Http.HttpResponseMessage response = await httpClient.PostAsync("http://faxian.tuhu.test/Article/AddFavoriate", content);
                    //    var responseContent = await response.Content.ReadAsStringAsync();
                    //}



                }
                else
                {
                    dic.Add("Message", "失败");
                }
            }
            catch (Exception ex)
            {
                dic.Add("Code", "0");
                //WebLog.LogException(ex);
                dic.Add("Message", "服务器异常");
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(CacheProfile = "VehicleCacheProfile", Duration = 3600)]
        public ActionResult SelectHotArticle()
        {
            var dic = new Dictionary<string, object>();
            var articles = ArticleSystem.SelectAllArticle().ToList();
            for (var i = 0; i < articles.Count(); i++)
            {
                articles[i].Hot = GetArticleHot(articles[i].CreateDateTime, articles[i].ShareWx + articles[i].Sharepyq, articles[i].Vote);
            }
            dic.Add("Code", "1");
            dic.Add("Articles", articles.OrderByDescending(article => article.Hot).Take(20));
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        private static double GetArticleHot(DateTime articleCreateTime, int shareQuantity, int likeQuantity)
        {
            var hours = (DateTime.Now - articleCreateTime).TotalHours;
            var item = shareQuantity * 3 + likeQuantity * 0.8;
            var rank = Math.Round(Math.Max(item, 1) / Math.Pow(hours + 2, 1.8), 5);
            return rank;
        }
        #endregion

        #region 旧点赞and取消点赞
        ////[CrossSite(Domain = "http://wxbanner.qiniudn.com")]
        //[CrossSite(Domain = "*")]
        //public ActionResult UnVote(int? pkid, string userid)
        //{
        //    var pid = pkid.HasValue ? pkid.Value : 0;
        //    if (string.IsNullOrWhiteSpace(userid) || pkid < 1)
        //    {
        //        return Content("{error:'Invalid vote parameters'}");
        //    }
        //    using (var ra = RedisAdapter.Create(this.GetType().Name))
        //    {
        //        var vkey = GenKeyUid(userid, pid);

        //        var vstatus = ra.GetAs<int>(vkey, KeyVote);
        //        if (vstatus > 0)
        //        {
        //            ra.Del(vkey, KeyVote);
        //        }
        //        var rlt = UnVoteDb(userid, pid, ra, vstatus);
        //        "UnVote".Logi(Session.SessionID);
        //        return Content(rlt.ToJson());
        //    }
        //}
        ////点赞，并返回目前本片文章总的点赞数
        ////[CrossSite(Domain = "http://wxbanner.qiniudn.com")]
        //[CrossSite(Domain = "*")]
        //public ActionResult Vote(int? pkid, string userid, int? vc = null)
        //{
        //    var pid = pkid.HasValue ? pkid.Value : 0;
        //    if (string.IsNullOrWhiteSpace(userid) || pkid < 1)
        //    {
        //        return Content("{error:'Invalid vote parameters'}");
        //    }
        //    AddHeat(pkid.Value.ToString());
        //    using (var ra = RedisAdapter.Create(this.GetType().Name))
        //    {
        //        if (vc.HasValue)
        //        {
        //            ra.Set(GenKeyVc(pid), vc.Value, TimeSpan.FromDays(3));
        //            return Content(vc.Value.ToString());
        //        }
        //        var votestatus = ra.GetAs<int>(GenKeyUid(userid, pid), KeyVote);
        //        var result = VoteDb(userid, pid, ra, votestatus);
        //        return Content(result.ToJson());
        //    }
        //}


        private static Hashtable UnVoteDb(string userid, int pid, RedisAdapter ra, int vstatus)
        {
            int VoteCount = 0;
            var result = new Hashtable();
            if (vstatus > 0)
            {
                result = Articles.UpdateVoteAndReturnCount(pid, true);
                ra.Set(GenKeyUid(userid, pid), "0", KeyVote, TimeSpan.FromDays(3));
                ra.Set(GenKeyVc(pid), result["AllVoteCount"], KeyVote, TimeSpan.FromDays(3));
                VoteCount = ra.GetAs<int>(GenKeyVc(pid), KeyVote); //Convert.ToInt32(ra.Get(GenKeyVc(pid), KeyVote));
                if (VoteCount < 1)
                {
                    VoteCount = Articles.AllVoteCount(pid);
                }
                result["Code"] = 1; //已经赞过了
            }
            else
            {
                VoteCount = Articles.AllVoteCount(pid);
                result["Code"] = 0; //已经取消了
            }
            result["AllVoteCount"] = VoteCount;//文章所对应的点赞的总数
            return result;
        }
        private static Hashtable VoteDb(string userid, int pid, RedisAdapter ra, int votestatus)
        {
            int VoteCount = 0;
            var result = new Hashtable();
            if (votestatus < 1)
            {
                result = Articles.UpdateVoteAndReturnCount(pid);
                ra.Set(GenKeyUid(userid, pid), "1", KeyVote, TimeSpan.FromDays(3));
                ra.Set(GenKeyVc(pid), result["AllVoteCount"], KeyVote, TimeSpan.FromDays(3));
                result["Code"] = 1;//已经赞过了
            }
            else//已经赞过，需要查点赞总数
            {
                VoteCount = Convert.ToInt32(ra.Get(GenKeyVc(pid), KeyVote));
                if (VoteCount == null)
                {
                    VoteCount = Articles.AllVoteCount(pid);
                }
                result["Code"] = 0;//已经赞过了
                result.Add("AllVoteCount", VoteCount);//文章所对应的点赞的总数
            }
            return result;
        }

        private static string GenKeyUid(string userid, int pid)
        {
            return userid + pid;
        }

        private static string GenKeyVc(int pid)
        {
            return "votecount" + pid;
        }

        #endregion

        #region Model

        /// <summary>
        /// 图文推送用来获取最新更新时间的Json文件内数据Model
        /// </summary>
        public class LatestArticleID
        {
            public int PKID { get; set; }
            public DateTime PublishDateTime { get; set; }
        }

        #endregion Model

        public  class ArticleHelper
        {
            /// <summary>
            /// 文章列表赋值
            /// </summary>
            /// <param name="artList"></param>
            /// <param name="userId"></param>
            /// <returns></returns>
            public object GetArticle(List<ArticleModel> artList, string userId)
            {
                //var articleShowMode = await DistributedCacheHelper.SelectArticleShowMode();
                var list = artList.Select(x => new
                {
                    ArticleShowMode = "New",
                    BigTitle = string.IsNullOrWhiteSpace(x.BigTitle) ? "" : x.BigTitle,
                    Brief = string.IsNullOrWhiteSpace(x.Brief) ? "" : x.Brief,
                    Catalog = string.IsNullOrWhiteSpace(x.Catalog) ? "" : x.Catalog,
                    //ClickCount = ArticleSystem.SelectClickCount(x.PKID),
                    ClickCount = x.ClickCount,
                    ContentUrl =  DomainConfig.FaXian+ "/react/findDetail/?bgColor=%23ffffff&textColor=%23333333&id=" + x.PKID,
                      Image = string.IsNullOrWhiteSpace(x.Image) ? "" : x.Image,
                    x.PKID,
                    ShowType = x.ShowType,
                    ShowImages = x.ShowImages,
                    //Vote = ArticleSystem.SelectVoteCount(x.PKID), //点赞数
                    Vote = x.Vote, //点赞数
                    PublishNewDateTime = x.PublishDateTime.ToString("yyyy/MM/dd"),
                    PublishDateTime = x.PublishDateTime.ToString("yyyy-MM-dd"),
                    //AnnotationTime = (DateTime.Now.Day - x.PublishDateTime.Day) == 0 ? "今天" : (DateTime.Now.Day - x.PublishDateTime.Day) == 1 ? "昨天" : (DateTime.Now.Day - x.PublishDateTime.Day) == 2 ? "前天" : "",
                    AnnotationTime = (DateTime.Now.Date == x.PublishDateTime.Date) ? "今天" : System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(x.PublishDateTime.DayOfWeek),
                    SmallImage = string.IsNullOrWhiteSpace(x.SmallImage) ? "" : x.SmallImage,
                    SmallTitle = string.IsNullOrWhiteSpace(x.SmallTitle) ? "" : x.SmallTitle,
                    Source = string.IsNullOrWhiteSpace(x.Source) ? "" : x.Source,
                    TitleColor = string.IsNullOrWhiteSpace(x.TitleColor) ? "" : x.TitleColor,
                    RedirectUrl = string.IsNullOrWhiteSpace(x.RedirectUrl) ? "" : x.RedirectUrl,
                    CategoryName = string.IsNullOrWhiteSpace(x.CategoryName) ? "" : x.CategoryName,//文章分类
                    Heat = x.Heat, //热度
                    VoteState = !string.IsNullOrWhiteSpace(userId) ? ArticleSystem.SelectVoteState(userId, x.PKID) : false,//点赞状态
                    CommentNum = x.CommentNum,
                    CommentTimes = x.CommentNum
                    //CommentNum = ArticleSystem.SelectCommentCount(x.PKID.ToString())
                }).ToList();
                return list;
            }

            /// <summary>
            /// 特推主题赋值
            /// </summary>
            /// <param name="artList"></param>
            /// <param name="userId"></param>
            /// <returns></returns>
            public static object GetTopArticle(List<ArticleModel> artList, string userId)
            {
                var list = artList.Select(x => new
                {
                    BigTitle = string.IsNullOrWhiteSpace(x.BigTitle) ? "" : x.BigTitle,
                    Brief = string.IsNullOrWhiteSpace(x.Brief) ? "" : x.Brief,
                    Catalog = string.IsNullOrWhiteSpace(x.Catalog) ? "" : x.Catalog,
                    //ClickCount = ArticleSystem.SelectClickCount(x.PKID),
                    ClickCount = x.ClickCount,
                    ContentUrl = string.IsNullOrWhiteSpace(x.ContentUrl) ? "" : x.ContentUrl,
                    Image = string.IsNullOrWhiteSpace(x.Image) ? "" : x.Image,
                    x.PKID,
                    // Vote = ArticleSystem.SelectVoteCount(x.PKID), //点赞数
                    Vote = x.Vote, //点赞数
                    PublishNewDateTime = x.PublishDateTime.ToString("MM月dd日"),
                    PublishDateTime = x.PublishDateTime.ToString("yyyy-MM-dd"),
                    AnnotationTime = (DateTime.Now.Date == x.PublishDateTime.Date) ? "今天" : System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(x.PublishDateTime.DayOfWeek),
                    SmallImage = string.IsNullOrWhiteSpace(x.SmallImage) ? "" : x.SmallImage,
                    SmallTitle = string.IsNullOrWhiteSpace(x.SmallTitle) ? "" : x.SmallTitle,
                    Source = string.IsNullOrWhiteSpace(x.Source) ? "" : x.Source,
                    TitleColor = string.IsNullOrWhiteSpace(x.TitleColor) ? "" : x.TitleColor,
                    RedirectUrl = string.IsNullOrWhiteSpace(x.RedirectUrl) ? "" : x.RedirectUrl,
                    CategoryName = string.IsNullOrWhiteSpace(x.CategoryName) ? "" : x.CategoryName,
                    Heat = x.Heat,
                    //CommentNum = ArticleSystem.SelectCommentCount(x.PKID.ToString()),//评论总数
                    CommentNum = x.CommentNum,//评论总数
                    ArticleBanner = string.IsNullOrWhiteSpace(x.ArticleBanner) ? "0" : x.ArticleBanner,
                    SmallBanner = string.IsNullOrWhiteSpace(x.SmallBanner) ? "0" : x.SmallBanner,
                    VoteState = !string.IsNullOrWhiteSpace(userId) ? ArticleSystem.SelectVoteState(userId, x.PKID) : false//点赞状态
                }).ToList();
                return list;
            }

            /// <summary>
            /// 评论回复消息分页
            /// </summary>
            /// <param name="list"></param>
            /// <param name="pIndex"></param>
            /// <param name="pSize"></param>
            /// <returns></returns>
            public static Dictionary<string, object> GetCommentNoticeByPage(IEnumerable<ArticleCommentModel> list, int pIndex, int pSize)
            {
                HomeController hc = new HomeController();
                var dic = new Dictionary<string, object>();
                int CommentNoticeCount = list.Count();
                int CommentPage = (list.Count() + pSize - 1) / pSize;
                var Articles = list.Skip((pIndex - 1) * pSize).Take(pSize);
                var CommentNotice = new List<ArticleCommentModel>();
                foreach (var at in Articles)
                {
                    var item = new ArticleCommentModel()
                    {
                        PKID = at.PKID,
                        ID = at.ID,
                        MyCommentContent = at.MyCommentContent,
                        UserGrade = at.UserGrade,
                        UserHead = at.UserHead,
                        CommentContent = at.CommentContent,
                        Title = at.Title,
                        Type = at.Type,
                        ArticleType = at.ArticleType,
                        IsRead = at.IsRead,
                        CommentTimeForOne = (DateTime.Now.Day - at.CommentTime.Day) == 0 ? "今天 " + at.CommentTime.ToString("HH:mm") : "" + at.CommentTime.ToString("yyyy-MM-dd HH:mm"),
                        UserName = hc.GetUserName(at.UserName, at.RealName, at.PhoneNum, at.Sex)
                        //UserName = string.IsNullOrWhiteSpace(at.UserName) ? !string.IsNullOrWhiteSpace(at.Sex) && !string.IsNullOrWhiteSpace(at.RealName) ? at.Sex == "女" ? at.RealName.Substring(0, 1) + "小姐" : at.RealName.Substring(0, 1) + "先生" : !string.IsNullOrWhiteSpace(at.PhoneNum) ? at.PhoneNum = at.PhoneNum.Replace(at.PhoneNum.Substring(3, 4), "****") : "游客" : at.UserName
                    };
                    CommentNotice.Add(item);
                }
                dic["Code"] = "1";
                dic["CommentNoticeCount"] = CommentNoticeCount;
                dic["CommentPage"] = CommentPage;
                dic["CommentNotice"] = CommentNotice.Select(x => new { PKID = x.PKID, Id = x.ID, IsRead = x.IsRead, Title = x.Title, MyCommentContent = x.MyCommentContent, UserHead = x.UserHead.Replace("img", "image"), CommentContent = x.CommentContent, UserName = x.UserName, UserGrade = x.UserGrade, CommentTime = x.CommentTimeForOne, Type = x.Type, ArticleType = x.ArticleType }).ToList();
                return dic;
            }

            /// <summary>
            /// 点赞回复消息分页
            /// </summary>
            /// <param name="list"></param>
            /// <param name="pIndex"></param>
            /// <param name="pSize"></param>
            /// <returns></returns>
            public static Dictionary<string, object> GetVoteNoticeByPage(IEnumerable<ArticleCommentModel> list, int pIndex, int pSize)
            {
                HomeController hc = new HomeController();
                var dic = new Dictionary<string, object>();
                int VoteNoticeCount = list.Count();
                int VotePage = (list.Count() + pSize - 1) / pSize;
                var Articles = list.Skip((pIndex - 1) * pSize).Take(pSize);
                var CommentNotice = new List<ArticleCommentModel>();
                foreach (var at in Articles)
                {
                    var item = new ArticleCommentModel()
                    {
                        UserHead = at.UserHead,
                        CommentContent = at.CommentContent,
                        Item = at.Item,
                        UserName = hc.GetUserName(at.UserName, at.RealName, at.PhoneNum, at.Sex),
                        Type = at.Type,
                        PKID = at.PKID,
                        ArticleType = at.ArticleType,
                        ParentID = at.ParentID,
                        ID = at.ID,
                        Title = at.Title,
                        IsRead = at.IsRead
                        //UserName = string.IsNullOrWhiteSpace(at.UserName) ? !string.IsNullOrWhiteSpace(at.Sex) && !string.IsNullOrWhiteSpace(at.RealName) ? at.Sex == "女" ? at.RealName.Substring(0, 1) + "小姐" : at.RealName.Substring(0, 1) + "先生" : !string.IsNullOrWhiteSpace(at.PhoneNum) ? at.PhoneNum = at.PhoneNum.Replace(at.PhoneNum.Substring(3, 4), "****") : "游客" : at.UserName
                    };
                    CommentNotice.Add(item);
                }
                dic["Code"] = "1";
                dic["VotePage"] = VotePage;
                dic["CommentNoticeCount"] = VoteNoticeCount;
                dic["MyComment"] = CommentNotice.Select(x => new { UserName = x.UserName, Title = x.Title, PKID = x.PKID, Id = x.ID, ParentID = x.ParentID, IsRead = x.IsRead, CommentContent = x.CommentContent, VoteCount = x.Item.Count(), Type = x.Type, ArticleType = x.ArticleType, item = x.Item.Where(y => !string.IsNullOrWhiteSpace(y.UserHead)).Select(y => y.UserHead).Take(7) }).ToList();
                return dic;
            }

            /// <summary>
            /// 查询回复XX的昵称
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static string GetUserNameByID(string id)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return "";
                }
                else
                {
                    string name = ArticleCommentSystem.GetUserNameByID(Convert.ToInt32(id));
                    if (name.StartsWith("1") && name.Length == 11)
                    {
                        name = name.Substring(0, 3) + "****" + name.Substring(7, 4);
                    }
                    return name;
                }
            }

        }
    }
}
