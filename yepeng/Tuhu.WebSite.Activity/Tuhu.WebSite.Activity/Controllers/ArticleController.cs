using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Tuhu.MessageQueue;
using Tuhu.Service.Utility.Request;
using Tuhu.Nosql;
using Tuhu.WebSite.Component.Activity.Business;
using Tuhu.WebSite.Component.Discovery.Business;
using Tuhu.WebSite.Component.Discovery.BusinessData;
using Tuhu.WebSite.Component.SystemFramework.Log;
using Tuhu.WebSite.Component.SystemFramework.Models;
using Tuhu.WebSite.Web.Activity.Common;
using Tuhu.WebSite.Web.Activity.Filters;
using Tuhu.WebSite.Web.Activity.Helpers;
using Tuhu.WebSite.Web.Activity.Models;
using System.Web.Caching;
using System.Xml;
using Tuhu.WebSite.Component.Activity.BusinessData;
using Tuhu.WebSite.Web.Activity.DataAccess;

namespace Tuhu.WebSite.Web.Activity.Controllers
{
    [ExceptionLog]
    public class ArticleController : Controller
    {
        static readonly string defaultExchageName = "direct.defaultExchage";
        //发现文章更新充值消息队列
        static readonly string DiscoveryArticleNotificationQueueName = "notification.DiscoveryArticleModify";
        static readonly RabbitMQProducer DiscoveryArticleRechargeProduceer;

        private static readonly Dictionary<string, string> Appkeys;
        static ArticleController()
        {
            try
            {
                DiscoveryArticleRechargeProduceer = RabbitMQClient.DefaultClient.CreateProducer(defaultExchageName);
                DiscoveryArticleRechargeProduceer.ExchangeDeclare(defaultExchageName);
                DiscoveryArticleRechargeProduceer.QueueBind(DiscoveryArticleNotificationQueueName, defaultExchageName);
            }
            catch
            {

            }
            Appkeys = new Dictionary<string, string> {{"baiduopenapi2016", "eb2b3e2389d84db0abe2d57e4e61c77e"}};
        }
        // GET: Article
        public ActionResult Index()
        {
            return View();
        }
        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, dateTime.Kind);
            return Convert.ToInt64((dateTime - start).TotalSeconds);
        }

        #region 优选文章
        /// <summary>
        /// 获取我喜欢的优选文章列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SelectMyFavoriteYouXuanListByUserId(Guid? userId,int pageIndex=1,int pageSize=10)
        {
            var dic = new Dictionary<string, object> { ["Code"] = "0" };
            if(!userId.HasValue||userId.Value==Guid.Empty)
            {
                dic["Message"] = "用户异常";
                return Json(dic);
            }
            
            var data = await ArticleSystem.SelectMyFavoriteYouXuanListByUserId(userId.Value, pageIndex, pageSize);

            dic["Code"] = "1";
            if (data != null && data.Any())
            {
                await data.ForEachAsync(async x =>
                {
                    //喜欢数
                    x.PraiseCount = await CacheManager.SelectYouXuanFavoriteCountByArticleId(x.Pkid);
                    //商品数量
                    x.ProductCount = GetProductCount(x.Content);
                });
                dic["ArticleList"] = data.Select(x => new
                {
                    x.Pkid,
                    x.SmallTitle,
                    x.BigTitle,
                    PublishDateTime = x.PublishDateTime?.ToString("MM-dd HH:mm"),
                    x.ClickCount,
                    x.VoteCount,
                    x.PraiseCount,
                    x.ProductCount,
                    x.CoverType,
                    x.CoverImg,
                    x.CoverVideo
                }).ToList();
            }
            else
            {
                dic["ArticleList"] = new object[0];
            }
            
            return Json(dic);
        }
        /// <summary>
        /// 通过分类ID获取优选列表
        /// </summary>
        /// <param name="categoryTagId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectYouXuanArticleByTagId(string categoryTagId, int pageIndex = 1, int pageSize = 20)
        {
            var dic = new Dictionary<string, object> { ["Code"] = "1" };
            List<Models.YouXuantModel> data = new List<Models.YouXuantModel>();
            using (var reclient = CacheHelper.CreateCacheClient("YouXuan"))
            {
                var result = await reclient.GetOrSetAsync($"YouXuanList/{categoryTagId}/{pageSize}", () => ArticleSystem.SelectYouXuanArticleByTagId(pageIndex, pageSize, categoryTagId), TimeSpan.FromMinutes(5));
                if (result.Value != null)
                    data = result.Value;
            }
            await data.ForEachAsync(async x =>
            {
                //喜欢数
                x.PraiseCount = await CacheManager.SelectYouXuanFavoriteCountByArticleId(x.Pkid);
                //商品数量
                x.ProductCount = GetProductCount(x.Content);
            });
            //var result = await DiscoverBLL.SelectYouXuanArticleByTagId(pageIndex, pageSize, categoryTagId);
            dic["Data"] = data.Select(x => new
            {
                x.Pkid,
                x.SmallTitle,
                x.BigTitle,
                PublishDateTime=x.PublishDateTime?.ToString("MM-dd HH:mm"),
                x.ClickCount,
                x.VoteCount,
                x.PraiseCount,
                x.ProductCount,
                x.CoverType,
                x.CoverImg,
                x.CoverVideo,
                x.IsTopMost
            });
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        private static int GetProductCount(string Content)
        {
            Regex reg = new Regex("(data-pid=['\"])(.+?)(['\"])");
            var mat = reg.Matches(Content);
            return mat.Count;
        } 

        /// <summary>
        /// 获取优选详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CollectUserAction(UserOperationEnum.Read)]
        public async Task<ActionResult> FirstYouXuanDetailById(int? id)
        {
            var dic = new Dictionary<string, object> { ["Code"] = "0" };
            if (!id.HasValue)
            {
                dic["Message"] = "参数错误";
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            Models.YouXuantModel data = null;

            using (var reclient = CacheHelper.CreateCacheClient("YouXuan"))
            {
                var result = await reclient.GetOrSetAsync($"YouXuanDetailById/{id.Value}", () => ArticleSystem.FirstYouXuanDetailById(id.Value), TimeSpan.FromMinutes(5));
                if (result.Value != null)
                    data = result.Value;
            }

            //var data = await DiscoverBLL.FirstYouXuanDetailById(id.Value);
            if (data == null)
            {
                dic["Message"] = "文章不存在！";
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            if (data.PublishDateTime.HasValue && data.PublishDateTime.Value.Date <= DateTime.Now.Date)
            {
                //喜欢数
                data.PraiseCount = await CacheManager.SelectYouXuanFavoriteCountByArticleId(id.Value);

                dic["Code"] = "1";
                dic["Detail"] = new
                {
                    data.Pkid,
                    data.SmallTitle,
                    data.BigTitle,
                    data.Brief,
                    data.Content,
                    PublishDateTime = data.PublishDateTime?.ToString("MM-dd HH:mm"),
                    data.ClickCount,
                    data.VoteCount,
                    data.PraiseCount,
                    data.ProductCount,
                    data.CoverType,
                    data.CoverImg,
                    data.CoverVideo,
                    data.OtherImgs,
                    data.IsTopMost
                };
                #region 评论数据
                var comments = await CacheManager.SelectCommentsTop10(id.Value);
                if (comments != null)
                {
                    //评论列表
                    dic["CommentList"] = comments.Item1.Select(x => new
                    {
                        x.Id,
                        x.CommentContent,
                        CommentTime = x.CommentTime.ToString(),
                        x.Praise,
                        x.UserId,
                        x.HeadImage,
                        x.NickName
                    });
                    //评论数量
                    dic["CommentCount"] = comments.Item2;
                }
                else
                {
                    //评论列表
                    dic["CommentList"] = new List<Comment>();
                    //评论数量
                    dic["CommentCount"] = 0;
                }
                #endregion
                #region 优选文章产品
                var pids = new List<string>();
                Regex reg = new Regex("(data-pid=['\"])(.+?)(['\"])");
                var mat = reg.Matches(data.Content);
                foreach (Match item in mat)
                {
                    if (item.Groups.Count == 4)
                        pids.Add(item.Groups[2].Value);
                }
                if (pids.Count > 0)
                {
                    var productList = await ProductService.SelectSkuProductListByPidsAsync(pids);
                    if (productList != null && productList.Any())
                    {
                        dic["ProductList"] = productList.Select(x => new
                        {
                            x.Pid,
                            x.DisplayName,
                            x.Price,
                            Image = x.Image.GetMobileUrl(),
                            x.ShuXing5,
                            x.OrderQuantity,
                            x.SalesQuantity
                        }).ToList();
                    }
                }
                else
                {
                    dic["ProductList"] = new object[0];
                }
                #endregion

            }
            else
            {
                dic["Message"] = "文章尚未发布";
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 优选点赞或收藏
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> YouXuanVoteOrFavorite(YouXuanRelatedOperactionModel info)
        {
            var dic = new Dictionary<string, object> { ["Code"] = "0" };
            if (info?.UserId == Guid.Empty)
            {
                dic["Message"] = "用户错误";
                return Json(dic);
            }
            if (info?.ArticleId <= 0)
            {
                dic["Message"] = "文章ID错误";
            }
            var result =await ArticleSystem.YouXuanVoteOrFavorite(info);
            if (result)
            {
                dic["Code"] = "1";
                await CacheManager.RemoveFromCacheAsync($"MyFavoriteYouXuanList/{info.UserId.ToString()}");
                await CacheManager.RemoveFromCacheAsync($"YouXuanFavoriteCount/{info.ArticleId}");
            }
                
            return Json(dic);
        }
        /// <summary>
        /// 查询优选文章收藏或点赞状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SelectYouXuanVoteAndFavorite(Guid? userId,int articleId=0)
        {
            var dic = new Dictionary<string, object> { ["Code"] = "0" };
            if (userId == null || userId == Guid.Empty||articleId<=0)
            {
                dic["Message"] = "参数错误";
                return Json(dic);
            }
            dic["Code"] = "1";

            var ids = await CacheManager.SelectMyFavoriteYouXuanIdsByUserId(userId.Value);
            if (ids != null && ids.Any() && ids.Contains(articleId))
                dic["Favorite"] = true;
            else
                dic["Favorite"] = false;

            dic["Vote"] = false;
            return Json(dic);
        }
        #endregion

        #region 新文章列表（包含优选视频文章）
        //[OutputCache(VaryByParam = "*", Duration = 10)]
        public async Task<ActionResult> GetArticleAndYouXuanList(int PageIndex = 1, int PageSize = 10)
        {
            var dic = new Dictionary<string, object> { ["Code"] = "1" };
            var articleList = await CacheManager.SelectArticleListAndYouXuanList(PageIndex, PageSize);
            if (articleList?.Count > 0)
            {
                articleList.ForEach(x =>
                {
                    x.ShowImage = x.Type == 9 ? x.ShowImage + "?watermark/1/image/aHR0cDovL2ltYWdlLnR1aHUuY24vdmlkZW9zL3BsYXkucG5nP19f/gravity/Center/ws/0.35|imageView2/2/w/500/h/500/q/100" : x.ShowImage;
                });
                dic["ArticleList"] = articleList;
            }
            else
            {
                dic["ArticleList"] = new object[0];
            }
            
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 外部合作

        public async Task<ActionResult> SelectArticleByTouTiao()
        {
            var result = await DiscoverBLL.SelectArticleByTouTiao();
            return Json(result.Select(x =>
            {
                var image = new List<ArticleImage>();
                var tages = new List<Category>();
                if (!string.IsNullOrWhiteSpace(x.ShowImages))
                {
                    image = JsonConvert.DeserializeObject<List<ArticleImage>>(x.ShowImages);
                }
                if (!string.IsNullOrWhiteSpace(x.CategoryTags))
                {
                    tages =
                        JsonConvert.DeserializeObject<List<JObject>>(x.CategoryTags)
                            .Where(y => y.Value<string>("isShow") == "1")
                            .Select(y => new Category() {Id = y.Value<int>("key"), Name = y.Value<string>("value")})
                            .ToList();
                }
                return new
                {
                    id = x.PKID,
                    type = 1,
                    style = (image.Count == 0 ? 5 : (image.Count == 1 ? 1 : 3)),
                    tags = tages.Select(y => y.Name).ToList(),
                    timestamp = DateTimeToUnixTimestamp(x.PublishDateTime),
                    title = x.BigTitle,
                    src = string.IsNullOrWhiteSpace(x.Source) ? "途虎养车" : x.Source,
                    content = x.Brief + x.ContentHTML,
                    small_image_url = image.Count>0? image.FirstOrDefault()?.BImage + "@320w@64-8-192-144a":"",
                    album_image_url = image.Select(y => y.BImage + "@320w@64-8-192-144a")
                };
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> SelectArticleList(string appkey, string sign, int pageIndex = 0)
        {
            var dic = new Dictionary<string, object>();
            if (string.IsNullOrWhiteSpace(appkey))
            {
                dic.Add("code", "-10");
                dic.Add("message", "appId不能为空");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrWhiteSpace(sign))
            {
                dic.Add("code", "-11");
                dic.Add("message", "sign不能为空");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }

            if (pageIndex == 0)
            {
                dic.Add("code", "-12");
                dic.Add("message", "页码不能为空");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            var sotrObjects = new SortedDictionary<string, object>();
            if (Appkeys.Any(x => x.Key.Equals(appkey, StringComparison.OrdinalIgnoreCase)))
            {
                sotrObjects.Add("appkey", appkey);
                sotrObjects.Add("pageIndex", pageIndex);
                var signstring = string.Join("&", sotrObjects.Select(x => x.Key + "=" + x.Value));
                //var verificationstr = SecurityHelper.Hash(signstring + "eb2b3e2389d84db0abe2d57e4e61c77e", HashType.Md5, false);
                var verificationstr = SecurityHelper.GetHashAlgorithm(HashType.Md5).ComputeHash(signstring + "eb2b3e2389d84db0abe2d57e4e61c77e", false);

                if (verificationstr == sign)
                {
                    PagerModel page = new PagerModel(pageIndex, 200);
                    var article = await ArticleBll.SearchArticle(page);

                    dic.Add("code", "0");
                    dic.Add("articleList", article.Select(x => new
                    {
                        articleId = x.PKID,
                        title = x.SmallTitle,
                        articleImage = !string.IsNullOrWhiteSpace(x.Image) ? x.Image + "@80-0-480-320a" : "",
                        linkUrl = $"{DomainConfig.FaXian}/Article/ArticleView?id={x.PKID}",
                        PublishDateTime = x.PublishDateTime?.ToString("yyyy-MM-dd hh:mm:ss") ?? x.CreateDateTime.ToString("yyyy-MM-dd hh:mm:ss")
                    }));
                    dic.Add("totalPage", page.TotalPage);
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                dic.Add("code", "-1");
                dic.Add("message", "签名错误");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            dic.Add("code", "-21");
            dic.Add("message", "appKey错误");
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> SelectLastArticleList(string appkey, string sign)
        {
            var dic = new Dictionary<string, object>();
            if (string.IsNullOrWhiteSpace(appkey))
            {
                dic.Add("code", "-10");
                dic.Add("message", "appId不能为空");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrWhiteSpace(sign))
            {
                dic.Add("code", "-11");
                dic.Add("message", "sign不能为空");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }


            var sotrObjects = new SortedDictionary<string, object>();
            if (Appkeys.Any(x => x.Key.Equals(appkey, StringComparison.OrdinalIgnoreCase)))
            {
                sotrObjects.Add("appkey", appkey);
                var signstring = string.Join("&", sotrObjects.Select(x => x.Key + "=" + x.Value));
                var verificationstr = SecurityHelper.Hash(signstring + "eb2b3e2389d84db0abe2d57e4e61c77e", false);
                if (verificationstr == sign)
                {
                    PagerModel page = new PagerModel(1, 50);
                    var article = await ArticleBll.SearchArticle(page);
                    article = article.Where(x => x.PublishDateTime?.Day == DateTime.Now.Day).ToList();
                    dic.Add("code", "0");
                    dic.Add("articleList", article.Select(x => new
                    {
                        articleId = x.PKID,
                        title = x.SmallTitle,
                        articleImage = !string.IsNullOrWhiteSpace(x.Image) ? x.Image + "@80-0-480-320a" : "",
                        linkUrl = $"{DomainConfig.FaXian}/Article/ArticleView?id={x.PKID}",
                        PublishDateTime = x.PublishDateTime?.ToString("yyyy-MM-dd hh:mm:ss") ?? x.CreateDateTime.ToString("yyyy-MM-dd hh:mm:ss")
                    }));
                    //dic.Add("totalPage", page.TotalPage);
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                dic.Add("code", "-1");
                dic.Add("message", "签名错误");
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            dic.Add("code", "-21");
            dic.Add("message", "appKey错误");
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 文章详情
        /// <summary>
        /// 文章详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [CollectUserAction(UserOperationEnum.Read)]
        //[OutputCache( Duration =10,VaryByParam ="*")]
        public async Task<ActionResult> Detail(string id, string userId = null)
        {
            int articleId;//文章Id
            if (!int.TryParse(id, out articleId))
                return View("DetailNotFound");
            Article articleModel = null;
            using (var reclient = CacheHelper.CreateCacheClient("ArticleDetail"))
            {
                var result = await reclient.GetOrSetAsync("DetailById/" + id, () => ArticleBll.GetArticleDetailById(articleId), TimeSpan.FromHours(1));
                if (result.Value != null)
                    articleModel = result.Value;
            }
            #region 判断校验
            if (articleModel == null)
                return View("DetailNotFound");

            //如果用户不为空则去取收藏状态
            if (!string.IsNullOrWhiteSpace(userId))
                articleModel.IsFavorite = await ArticleBll.IsFavoriteArticle(articleId, userId);

            if (articleModel.Type == 5)
            {
                //不是发布状态
                if (articleModel.Status != ArticleStatus.Published.ToString())
                    return View("DetailNotFound");
                //发表时间没有到
                else if (!articleModel.PublishDateTime.HasValue && articleModel.PublishDateTime.Value > DateTime.Now)
                    return View("DetailNotFound");
            }
            else if (articleModel.IsShow != 1)
                return View("DetailNotFound");

            #endregion

            #region 标签
            //标签
            if (!string.IsNullOrEmpty(articleModel.CategoryTags))
            {
                try
                {
                    //标签实体转换
                    var Tags = JsonConvert.DeserializeObject<List<JObject>>(articleModel.CategoryTags)
                        .Select(x => new Category()
                        {
                            Id = x.Value<int>("key"),
                            Name = x.Value<string>("value"),
                            Disable = x.Value<string>("isShow") == "1" ? true : false
                        }).ToList();
                    ViewData["TagList"] = Tags;
                    //相关阅读
                    using (var client = CacheHelper.CreateCacheClient("ArticleDetail"))
                    {
                        var firstTag = Tags.FirstOrDefault(s => s.Disable);
                        int cateId = firstTag != null ? firstTag.Id : 1;
                        var result = await client.GetOrSetAsync("DetailRelatedArticlesById/" + articleId + "/" + cateId, () => ArticleBll.GetRelateArticleByArticleId(articleId, cateId, new PagerModel() { CurrentPage = 1, PageSize = 5 }), TimeSpan.FromHours(1));
                        if (result.Value != null)
                            ViewData["RelatedArticles"] = result.Value;
                        else
                            ViewData["RelatedArticles"] = new IEnumerable<RelatedArticle>[0];
                    }

                }
                catch (Exception ex)
                {
                    WebLog.LogException(ex);
                    ViewData["TagList"] = null;
                    ViewData["RelatedArticles"] = null;
                }
            }
            else
                ViewData["RelatedArticles"] = null;
            #endregion
            

            using (var reclient = CacheHelper.CreateCacheClient("ArticleDetail"))
            {
                var result = await reclient.GetOrSetAsync("DetailCommentsTopNumById/" + id, () => CommentBll.SelectCommentsTopNum(articleId, 10), TimeSpan.FromHours(1));
                if (result.Value != null)
                {
                    var getResult = result.Value;
                    //评论列表
                    ViewData["CommentList"] = getResult.Item1;
                    //评论数量
                    ViewData["ArticleCommentCount"] = getResult.Item2;

                }
                else
                {
                    //评论列表
                    ViewData["CommentList"] = new List<Comment>();
                    //评论数量
                    ViewData["ArticleCommentCount"] = 0;
                }

            }
            ViewData["IsAppRequest"] = IsAppRequest();


            return PartialView(articleModel);
        }
        [CrossHost]
        public async Task<ActionResult> GetArticleDetail(int? id)
        {
            var dic = new Dictionary<string, object> { ["Code"] = "1" };
            if (!id.HasValue)
            {
                dic["Code"] = "1";
                dic["Message"] = "缺少参数";
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            Article articleModel = null;
            using (var reclient = CacheHelper.CreateCacheClient("ArticleDetail"))
            {
                var result = await reclient.GetOrSetAsync("DetailById/" + id, () => ArticleBll.GetArticleDetailById(id.Value), TimeSpan.FromHours(1));
                if (result.Value != null)
                    articleModel = result.Value;
            }
            if (articleModel == null)
            {
                dic["Code"] = "1";
                dic["Message"] = "文章不存在";
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            if (articleModel.IsShow != 1)
            {
                dic["Code"] = "1";
                dic["Message"] = "文章未发布";
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            if (articleModel.Type != 99)
            {
                dic["Code"] = "1";
                dic["Message"] = "文章类型错误";
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            dic["ArticleModel"] = new
            {
                articleModel.SmallImage,
                articleModel.SmallTitle,
                articleModel.Brief,
                articleModel.Content
            };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> CommentList(string Aid)
        {
            PagerModel pager = new PagerModel()
            {
                CurrentPage = 1,
                PageSize = 10
            };
            var model = await GetComment(int.Parse(Aid), pager, CommentOrderBy.none);
            return PartialView(model);
        }

        /// <summary>
        /// 判断是否被收藏
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="userId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> IsFavoriate(string Id, string userId = null, string deviceId = null)
        {
            if (!int.TryParse(Id, out int aid))
            {
                return Json(new { success = false, IsFavoriate = false }, JsonRequestBehavior.AllowGet);
            }
            if (!Guid.TryParse(userId, out Guid uid))
            {
                return Json(new { success = false, IsFavoriate = false }, JsonRequestBehavior.AllowGet);
            }
            var IsFavorite = await ArticleSystem.isLikeByArticleIdAndUserId(aid, uid);// await ArticleBll.IsFavoriteArticle(aid, userId);
            return Json(new { success = true, IsFavoriate = IsFavorite }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 收集分享次数
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="userId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpPost]
        [CrossHost]
        [CollectUserAction(UserOperationEnum.Share)]
        public JsonResult CollectShareTimes(int Id, string userId = null, string deviceId = null)
        {
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        private static readonly  List<string> TipWrodList = new List<string>()
            {
                "有想法就说，看对眼就上",
                "走过路过，评论不要错过",
                "少一些套路，多一些留言",
                "据说评论是小编创作的最大动力",
                "这儿还是块处女地，不要让别人抢先评论奥",
                "觉得这篇文章有用么？给句话儿吧",
                "沙发已经摆好，欢迎大神入座",
                "老司机，当滔滔不绝，不要停",
                "可以赞，可以喷，沉默不是金",
                "掌声已经准备好，请开始发言",
                "简单点，说话的方式简单",
                "看到美女了么？不得说点啥么？",
                "辛辛苦苦写文章，你却只看不说话",
                "产品妹妹想知道，此刻你在想啥"
            };
        [CrossHost]
        [CollectUserAction(UserOperationEnum.Read)]
        //[OutputCache(VaryByParam = "*", Duration = 30)]
        public async Task<ActionResult> SelectArticleDetail(string id,Guid? userid=null)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                //int articleId;//文章Id
                if (!int.TryParse(id, out int articleId))
                {
                    dic.Add("Code", "0");
                    dic.Add("Msg", "文章Id不合法");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                Article articleModel = await CacheManager.GetArticleDetailById(articleId);
                #region 检验文章数据
                if (articleModel == null)
                {
                    dic.Add("Code", "0");
                    dic.Add("Msg", "文章不存在");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                bool IsValid = true;
                if (articleModel.Type == 5)
                {
                    //不是发布状态
                    if (articleModel.Status != ArticleStatus.Published.ToString())
                        IsValid = false;
                    //发表时间没有到
                    else if (!articleModel.PublishDateTime.HasValue && articleModel.PublishDateTime.Value > DateTime.Now)
                        IsValid = false;
                }
                else if (articleModel.IsShow != 1)
                    IsValid = false;
                if (!IsValid)
                {
                    dic.Add("Code", "0");
                    dic.Add("Msg", "文章状态不正确");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                #endregion
                dic.Add("Code", "1");
                dic.Add("Msg", "数据获取成功");
                var authorModel = await ArticleSystem.SelectCoverAuthorByName(articleModel.CoverTag);
                dic.Add("ArticleDetail", new
                {
                    articleModel.PKID,
                    Title = articleModel.SmallTitle,
                    articleModel.ContentHtml,
                    articleModel.Image,
                    articleModel.CoverImage,
                    articleModel.ShowImages,
                    articleModel.Brief,
                    CreateDateTime = Convert.ToInt64((articleModel.CreateDateTime - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalSeconds).ToString(),
                    PublishDateTime = Convert.ToInt64((articleModel.PublishDateTime.Value - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalSeconds).ToString(),
                    articleModel.ClickCount,
                    articleModel.Vote,
                    articleModel.CoverTag,
                    AuthorInfo = new
                    {
                        authorModel.AuthorName,
                        authorModel.AuthorHead,
                        authorModel.Description
                    },
                    articleModel.CategoryTags,
                    articleModel.Type,
                    articleModel.IsShow,
                    articleModel.QRCodeImg
                });
                #region 标签&相关阅读
                if (!string.IsNullOrEmpty(articleModel.CategoryTags))
                {
                    List<RelatedArticle> relatedArticles = null;
                    List<CategoryTagsModel> Tags = null;
                    try
                    {
                        //标签实体转换
                        Tags = JsonConvert.DeserializeObject<List<CategoryTagsModel>>(articleModel.CategoryTags);
                    }
                    catch (Exception ex)
                    {
                        WebLog.LogException(ex);
                    }
                    var cateId = Tags?.FirstOrDefault(s => "1".Equals(s.isShow))?.key ?? 1;
                    //相关阅读
                    var result=await CacheManager.SelectRelateArticleByCategoryTagId(cateId);
                    relatedArticles = result?.Where(x => x.PKID != articleId)?.Select(x => new RelatedArticle
                    {
                        PKID = x.PKID,
                        Title = x.SmallTitle,
                        CoverTag = x.CoverTag,
                        CoverImage = x.CoverImage,
                        ReadCount = x.ClickCount,
                        PublishDateTime = x.PublishDateTime,
                        Image = x.Image,
                        Type = x.Type
                    })?.Take(5)?.ToList();


                    dic.Add("TagList", Tags?.Select(x => new Category
                    {
                        Id = x.key,
                        Name = x.value,
                        Disable = x.isShow == "1"
                    }));
                    dic.Add("RelatedArticles", relatedArticles);
                }
                else
                {
                    dic.Add("TagList", null);
                    dic.Add("RelatedArticles", null);
                }
                #endregion
                var comment = await ArticleSystem.SelectCommentsTop10(articleId);

                //评论列表
                dic["CommentList"] = comment?.Item1 ?? new List<Comment>();
                //评论数量
                dic["ArticleCommentCount"] = comment?.Item2 ?? 0;

                
                dic.Add("TipWrod", TipWrodList[new Random().Next(0, TipWrodList.Count)]);
            }
            catch (Exception ex)
            {
                dic.Clear();
                dic.Add("Code", "0");
                dic.Add("Msg", ex.Message);
                WebLog.LogException("查询文章详情页的接口", ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }


        #endregion
        #region 评论(已废)
        public async Task<ActionResult> CommentList1(string Aid, Guid? UserId = null)
        {
            PagerModel pager = new PagerModel()
            {
                CurrentPage = 1,
                PageSize = 10
            };
            ViewData["UserID"] = UserId;
            ViewData["RelatedArticleId"] = Request.QueryString["RelatedArticleId"];
            var model = await GetComment(int.Parse(Aid), pager, CommentOrderBy.none, UserId);
            ViewData["PagerModel"] = pager;
            return PartialView(model);
        }

        public async Task<ActionResult> Comment(string Aid, string UserId = "", string ReplyId = "", string NickName = "")
        {
            ViewData["ArticleId"] = Aid;
            ViewData["UserID"] = UserId;
            ViewData["ReplyID"] = ReplyId;
            ViewData["NickName"] = NickName;
            return PartialView();
        }
        #endregion

        #region 评论API
        public async Task<ActionResult> SelectArticleCommentList(string PKID, Guid? UserID = null, int PageIndex = 1)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                PagerModel pager = new PagerModel()
                {
                    CurrentPage = PageIndex,
                    PageSize = 10
                };
                var model = await GetComment(int.Parse(PKID), pager, CommentOrderBy.none, UserID);
                //文章Model
                var articleModel = model.article;
                //最新评论
                var newComments = model.news.Select(s => new
                {
                    UserID = s.UserId,
                    ID = s.CommentId,
                    AuditStatus = 2,
                    Category = string.Empty,
                    CommentContent = s.CommentContent,
                    CommentTime = (DateTime.Now.Day - s.CommentDatetime.Day) == 0 ? "今天 " + s.CommentDatetime.ToString("HH:mm") : "" + s.CommentDatetime.ToString("yyyy-MM-dd HH:mm"),
                    CommentNum = s.CommentCount,
                    PraiseNum = s.AgreeCount,
                    PhoneNum = s.CellPhone,
                    Title = articleModel.Title,
                    UserHead = s.HeadImage,
                    UserName = s.NickName,
                    UserGrade = s.UserGrade,
                    floor = 1,
                    ParentID = s.ReplyComment != null ? s.ReplyComment.CommentId.ToString() : "",
                    ParentName = s.ReplyComment != null ? s.ReplyComment.NickName.ToString() : "",
                    IsPraise = s.IsLike ? 1 : 0
                });
                var commentsPage = pager.TotalPage;
                if (commentsPage > 1 && PageIndex == 1)
                {
                    //热门评论
                    var hotComments = model.hots.Select(s => new
                    {
                        UserID = s.UserId,
                        ID = s.CommentId,
                        AuditStatus = 2,
                        Category = string.Empty,
                        CommentContent = s.CommentContent,
                        CommentTime = (DateTime.Now.Day - s.CommentDatetime.Day) == 0 ? "今天 " + s.CommentDatetime.ToString("HH:mm") : "" + s.CommentDatetime.ToString("yyyy-MM-dd HH:mm"),
                        CommentNum = s.CommentCount,
                        PraiseNum = s.AgreeCount,
                        PhoneNum = s.CellPhone,
                        Title = articleModel.Title,
                        UserHead = s.HeadImage,
                        UserName = s.NickName,
                        UserGrade = s.UserGrade,
                        floor = 1,
                        ParentID = s.ReplyComment != null ? s.ReplyComment.CommentId.ToString() : "",
                        ParentName = s.ReplyComment != null ? s.ReplyComment.NickName.ToString() : "",
                        IsPraise = s.IsLike ? 1 : 0
                    });
                    dic["HotComment"] = hotComments;
                }
                else
                {
                    dic["HotComment"] = new ArticleCommentModel[0];
                }
                dic["Code"] = "1";
                dic["CommentsPage"] = commentsPage;
                dic["CommentNew"] = newComments;
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
        /// 点赞&取消点赞
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CommentVoteOrNot(CommentPraise model)
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
                var result = await CommentBll.UpdateAgree(model.CommentId, Guid.Parse(model.UserId), model.VoteState == 1 ? LikeOrNot.Like : LikeOrNot.NotLike);
                if (result)
                {
                    dic["Code"] = "1";
                    dic["VoteState"] = model.VoteState == 1 ? true : false;
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

        /// <summary>
        /// 新增评论
        /// </summary>
        /// <param name="acm"></param>
        /// <returns></returns>
        [HttpPost]
        [CollectUserAction(UserOperationEnum.Comment)]
        public async Task<ActionResult> AddNewComment(ArticleCommentModel acm)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                #region 验证数据
                if (acm.PKID == 0)
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "文章关联ID有误");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrWhiteSpace(acm.UserID))
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "用户ID不能为空");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                Guid users;
                if (Guid.TryParse(acm.UserID, out users) == false)
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "用户ID异常");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                acm.UserID = new Guid(acm.UserID).ToString("B");
                //过滤非法关键字
                if (HttpClientHelper.AcccessApi(HttpContext.Request, "/Advertise/ValidateCommentContent",
                    new Dictionary<string, string> { { "userId", acm.UserID }, { "content", acm.CommentContent } }) == "false")
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "存在非法词汇");
                    return Json(dic);
                }
                #endregion

                Comment cm = new Comment()
                {
                    CommentContent = acm.CommentContent,
                    ParentID = !string.IsNullOrEmpty(acm.ParentID) ? acm.ParentID : null,
                    UserId = users.ToString("B"),
                    PKID = acm.PKID
                };
                var result = await CommentBll.AddComment(cm);
                if (result != null)
                {
                    dic.Add("Code", "1");
                    dic.Add("Message", "评论成功");
                }
                else
                {
                    dic.Add("Code", "0");
                    dic.Add("Message", "评论失败");
                }

            }
            catch (Exception ex)
            {
                dic.Add("Code", "0");
                dic.Add("Message", "服务器忙，请重试！");
                dic.Add("message", ex.Message);
                WebLog.LogException("新发现新增评论", ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region 列表页面
        public ActionResult Firstline()
        {
            Server.Transfer("/static/index.html");
            return Content("");
        }
        public ActionResult FindNew()
        {
            Server.Transfer("/react/find_new/index.html");
            return Content("");
        }
        
        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="ArticleTitle">文章标题</param>
        /// <param name="ArticleTagId">文章标签</param>
        /// <param name="PageIndex">文章页数</param>
        /// <param name="PageSize">文章每页大小</param>
        /// <returns></returns>
        [HttpGet]
        [CrossHost]
        public async Task<JsonResult> GetArticleList2(string ArticleTitle = "", string ArticleTagId = "", string ArticleAuthor = "", int? PageIndex = 1, int? PageSize = 10,string isHot= "false")
        {
            if (isHot=="true")
            {
                return await GetHotArticleList(PageIndex, PageSize);
            }
            using (var client = CacheHelper.CreateCacheClient("TouTiao_List_Cache"))
            {
                ArticleViewModel viewModel = new ArticleViewModel();
                PagerModel pager = new PagerModel();
                bool isSuccess = false;
                //头条列表页面首先从缓存中读取
                if (string.IsNullOrEmpty(ArticleTitle) && string.IsNullOrEmpty(ArticleAuthor))
                {
                    string key = string.Format("ArticleList/{0}/{1}/{2}", PageIndex, PageSize, ArticleTagId);

                    isSuccess = client.Exists(key).Success;

                    IResult<ArticleViewModel> cacheResult = await client.GetOrSetAsync(key, () =>
                               {
                                   ArticleViewModel vm = new ArticleViewModel();
                                   return GetArticleListByConditions(pager, ArticleTitle, ArticleTagId, ArticleAuthor, PageIndex.Value, PageSize.Value);
                               });
                    if (cacheResult.Success && cacheResult.Exception == null)
                    {
                        viewModel = cacheResult.Value;
                    }
                    else
                    {
                        viewModel = await GetArticleListByConditions(pager, ArticleTitle, ArticleTagId, ArticleAuthor, PageIndex.Value, PageSize.Value);
                    }
                }
                else
                {
                    //搜索结果从数据库中读取
                    viewModel = await GetArticleListByConditions(pager, ArticleTitle, ArticleTagId, ArticleAuthor, PageIndex.Value, PageSize.Value);
                }
                if (!string.IsNullOrEmpty(ArticleTitle))
                {
                    viewModel.hasArticleTitle = true;
                }
                else
                {
                    viewModel.hasArticleTitle = false;
                }
                if (!string.IsNullOrEmpty(ArticleAuthor))
                {
                    var authorModel = await ArticleSystem.SelectCoverAuthorByName(ArticleAuthor);
                    viewModel.author = new AuthorInfo
                    {
                        AuthorName=authorModel.AuthorName,
                        AuthorHead= authorModel.AuthorHead,
                        Description=authorModel.Description
                    };
                }
                viewModel.msg = viewModel.data.Any()? "调用成功" : "数据为空！";
                viewModel.success = viewModel.data.Any() ? true : false;

                //viewModel.data = isSuccess ? viewModel.body.Select(s => s.ToString()) : viewModel.body.Select(s => s.ToJson());
                //viewModel.body = null;

                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        [CrossHost]
        [OutputCache(VaryByParam = "*", Duration = 10)]
        public async Task<JsonResult> GetArticleList(string ArticleTitle = "", string ArticleTagId = "", string ArticleAuthor = "", int? PageIndex = 1, int? PageSize = 10, string isHot = "false")
        {
            ArticleViewModel viewModel = new ArticleViewModel() { success = true, msg = "调用成功" };
            List<SpecialColumn> scList = new List<SpecialColumn>();
            if (PageIndex > 20)
            {
                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }

            //using(var client = CacheHelper.CreateCounterClient("ArticleListCount",TimeSpan.FromSeconds(1)))
            //{
            //    var count=await client.IncrementAsync(Request.UserIp());
            //    if(count.Value > 3)
            //    {
            //        return Json(viewModel, JsonRequestBehavior.AllowGet);
            //    }
            //}

            if (isHot == "true")
            {
                return await GetHotArticleList(PageIndex, PageSize);
            }
            using (var client = CacheHelper.CreateCacheClient("TouTiao_List_Cache"))
            {
                

                PagerModel pager = new PagerModel();
                int totalPage = 0;
                Tuple<List<ArticleItem>, int> resultData = null;
                bool isToutiao = string.IsNullOrWhiteSpace(ArticleTitle) && string.IsNullOrWhiteSpace(ArticleAuthor) && string.IsNullOrWhiteSpace(ArticleTagId);
                //头条列表页面首先从缓存中读取
                if (string.IsNullOrWhiteSpace(ArticleTitle) && string.IsNullOrWhiteSpace(ArticleAuthor))
                {
                    #region 取文章的数据--一页
                    string key = $"ArticleListNew/{PageIndex}/{PageSize}/{ArticleTagId}";
                    var cacheResult = await client.GetOrSetAsync(key, () => SelectArticleListByConditions(pager, isToutiao, ArticleTitle, ArticleTagId, ArticleAuthor, PageIndex.Value, PageSize.Value), TimeSpan.FromMinutes(5));

                    if (cacheResult.Success && cacheResult.Exception == null)
                    {
                        resultData = cacheResult.Value;
                    }
                    else
                    {
                        resultData = await SelectArticleListByConditions(pager, isToutiao, ArticleTitle, ArticleTagId, ArticleAuthor, PageIndex.Value, PageSize.Value);
                    }
                    #endregion
                }
                else
                {
                    //搜索结果从数据库中读取
                    resultData = await SelectArticleListByConditions(pager, isToutiao, ArticleTitle, ArticleTagId, ArticleAuthor, PageIndex.Value, PageSize.Value);
                }
                totalPage = resultData.Item2;

                viewModel.hasArticleTitle = !string.IsNullOrEmpty(ArticleTitle) ? true : false;

                if (!string.IsNullOrEmpty(ArticleAuthor))
                {
                    var authorModel = await ArticleSystem.SelectCoverAuthorByName(ArticleAuthor);
                    viewModel.author = new AuthorInfo
                    {
                        AuthorName = authorModel.AuthorName,
                        AuthorHead = authorModel.AuthorHead,
                        Description = authorModel.Description
                    };
                }
                //if (isToutiao)
                //{
                //    string key = string.Format("SpecialColumnList/{0}/{1}", PageIndex, PageSize);
                //    var cacheResult = await client.GetOrSetAsync(key, () => { return SelectSpecialColumnByConditions(PageIndex.Value, PageSize.Value); });
                //    scList = (cacheResult.Success && cacheResult.Exception == null) ? cacheResult.Value : await SelectSpecialColumnByConditions(PageIndex.Value, PageSize.Value);
                //}
                await resultData.Item1?.ForEachAsync(async x => 
                {
                    x.ContentUrl = $"{DomainConfig.FaXian}/react/find_new/#/detail?bgColor=%252523ffffff&id={x.Id}&textColor=%252523333333&userid=&_k=xpbooq";
                    x.AuthorInfo = await ArticleSystem.SelectCoverAuthorByName(x.CoverTag);
                });

                viewModel.data = resultData.Item1.ToList<object>(); //SelectCombineArticleAndSpecialColumn(resultData.Item1, scList);
                viewModel.TotalPage = totalPage;
                viewModel.CurrentPage = PageIndex.Value;
                viewModel.msg = viewModel.data.Any() ? "调用成功" : "数据为空！";
                viewModel.success = viewModel.data.Any() ? true : false;
                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 合并文章和专题
        /// </summary>
        /// <param name="articleList">文章列表</param>
        /// <param name="scList">专题列表</param>
        /// <returns></returns>
        private List<object> SelectCombineArticleAndSpecialColumn(List<ArticleItem> articleList, List<SpecialColumn> scList)
        {
            if (scList.Count > 0)
            {
                List<object> combineResult = new List<object>();
                for (int i = 0; i < articleList.Count; i++)
                {
                    #region 文章和专题组合
                    combineResult.Add(articleList[i]);
                    if (i % 2 == 1)
                    {
                        SpecialColumn scModel = scList.FirstOrDefault();
                        if (scModel != null)
                        {
                            combineResult.Add(scModel);
                            scList.RemoveAt(0);
                        }
                    }
                    #endregion
                }
                return combineResult;
            }
            else
                return articleList.ToList<object>();
        }

        [NonAction]
        private async Task<Tuple<List<ArticleItem>, int>> SelectArticleListByConditions(PagerModel pager, bool IsToutiao, string ArticleTitle = "", string ArticleTag = "", string ArticleAuthor = "", int PageIndex = 1, int PageSize = 10)
        {
            List<Article> list;
            pager.CurrentPage = PageIndex;
            pager.PageSize = PageSize > 50 ? 50 : PageSize;
            ArticleTitle = ConvertEmoji2UnicodeHex(ArticleTitle);
            
            if ((!string.IsNullOrEmpty(ArticleTitle) || !string.IsNullOrEmpty(ArticleAuthor)) && string.IsNullOrEmpty(ArticleTag))
            {
                list = await ArticleBll.SearchArticle(pager, keyword: ArticleTitle, author: ArticleAuthor);
            }
            else if (!string.IsNullOrEmpty(ArticleTag))
            {
                list = await ArticleBll.GetArticlesByCategoryId(Int32.Parse(ArticleTag), pager, ArticleTitle);
            }
            else if (!string.IsNullOrEmpty(ArticleAuthor))
            {
                list = await ArticleBll.SearchArticle(pager, author: ArticleAuthor);
            }
            else
            {
                list = await ArticleBll.SearchArticle(pager, null, null, IsToutiao);
            }
            if (list != null && list.Count > 0 && string.IsNullOrWhiteSpace(ArticleAuthor) && string.IsNullOrWhiteSpace(ArticleTitle) && string.IsNullOrWhiteSpace(ArticleTag))
            {
                await list.ForEachAsync(async u =>
                {
                    u.CommentCountNum = await ArticleBll.GetCommentCount(u.PKID);
                });
            }
           
                var items = ArticleItem.ConvertToArtcleVM(list);
            return new Tuple<List<ArticleItem>, int>(items, pager.TotalPage);

        }

        private async Task<List<SpecialColumn>> SelectSpecialColumnByConditions(int pageIndex,int pageSize)
        {
            PagerModel newPager = new PagerModel
            {
                CurrentPage = pageIndex,
                PageSize = pageSize / 2
            };
            //获取专题
            return await SpecialColumnBll.SelectSpecialCloumns(newPager);
        }



        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <returns></returns>
        [CrossHost]
        public JsonResult RefreshCache(string name, string key)
        {
            using (var client = CacheHelper.CreateCacheClient(name))
            {
                var result = client.Remove(key);
                return Json(new { success = result.Success, msg = result.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取阅读数大于2W的文章
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [CrossHost]
        public async Task<JsonResult> GetHotArticleList(int? PageIndex = 1, int? PageSize = 10)
        {
            using (var client = CacheHelper.CreateCacheClient("HotArticle_List_Cache"))
            {
                ArticleViewModel viewModel = new ArticleViewModel();
                PagerModel pager = new PagerModel();
                Tuple<List<ArticleItem>, int> resultData = null;
                //头条列表页面首先从缓存中读取
                string key = string.Format("HotArticleList/{0}/{1}/{2}", PageIndex, PageSize, "");
                bool isSuccess = client.Exists(key).Success;
                var cacheResult = await client.GetOrSetAsync(key, () => { return GetArticleListByConditions2(pager, PageIndex.Value, PageSize.Value); });
                if (cacheResult.Success && cacheResult.Exception == null)
                {
                    resultData = cacheResult.Value;
                }
                else
                {
                    resultData = await GetArticleListByConditions2(pager, PageIndex.Value, PageSize.Value);
                }
                viewModel.data = resultData.Item1.ToList<object>();
                viewModel.CurrentPage = PageIndex.Value;
                viewModel.TotalPage = resultData.Item2;
                viewModel.hasArticleTitle = false;
                viewModel.msg = viewModel.data.Any()? "调用成功" : "数据为空！";
                viewModel.success = viewModel.data.Any()? true : false;
                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }
        }
        [NonAction]
        private async Task<Tuple<List<ArticleItem>, int>> GetArticleListByConditions2(PagerModel pager, int PageIndex = 1, int PageSize = 10)
        {
            pager.CurrentPage = PageIndex;
            pager.PageSize = PageSize > 50 ? 50 : PageSize;
            List<Article> list = await ArticleBll.SearchArticleMoreThan(pager, 20000);
            if (list.Count > 0)
            {
                await list.ForEachAsync(async u =>
                {
                    u.CommentCountNum = await ArticleBll.GetCommentCount(u.PKID);
                });
            }
            var items = ArticleItem.ConvertToArtcleVM(list);
            return new Tuple<List<ArticleItem>, int>(items, pager.TotalPage);
        }

        /// <summary>
        /// 晒图列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [CrossHost]
        public async Task<ActionResult> SelectShareImagesListAsync(string userId,int pageIndex=1,int pageSize=20)
        {
            var iosVersion = Request.Headers.Get("version");
            Version appversion = null;
            if (iosVersion != null)
            {
                var arr = iosVersion.Split(' ');
                if (arr[0] == "iOS")
                {
                    if (new Version(arr[1])==new Version(5,0,3))
                    {
                        pageSize = 50;
                    }
                }
            }
            var dic = new Dictionary<string, object> {["Code"] = "1"};
            var shareImages=await DiscoverBLL.SelectShareImagesListAsync(new PagerModel(pageIndex, pageSize),null);

            dic["shareImages"] =
                shareImages.Select(
                    x =>
                    {
                        var user =HttpClientHelper.SelectUserInfoByUserId(x.UserId);
                        if (user != null)
                        {
                            x.UserName = user.Nickname;
                            x.UserHead = "http://image.tuhu.cn" + user.HeadImage;
                        }
                        else
                        {
                            x.UserName = "途虎用户";
                            x.UserHead = "http://resource.tuhu.cn/Image/Product/zhilaohu.png";
                        }
                        return new
                        {

                            PKID = x.Pkid,
                            SmallImage = x.ImagesUrl[0],
                            UserName=GetUserName(x.UserName),
                            x.UserHead,
                            x.Content,
                            UserId=Guid.Parse(x.UserId).ToString("B"),
                            x.LikesCount,
                            CommentTimes = x.CommentCount,
                            x.ShareCount,
                            ImagesCount = x.ImagesUrl.Count()
                        }
                            ;
                    }).ToList();

            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        public static string GetUserName(string name)
        {
            if (name.StartsWith("1") && name.Length == 11)
            {
                name = name.Substring(0, 3) + "****" + name.Substring(7, 4);
            }
            return name;
        }
        /// <summary>
        /// 晒图详情
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        ///
        [CrossHost]
        public async Task<ActionResult> SelectShareImagesDetailByIdAsync(string userId,int id=0)
        {
            var dic = new Dictionary<string, object> { ["Code"] = "1" };
            if (id==0)
            {
                dic["Code"] = "0";
                dic["Message"] = "参数错误";
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(userId))
                userId = null;
            var detail = await DiscoverBLL.FirstShareImagesByInfoIdAsync(id, userId);
            if (detail == null)
            {
                dic["Code"] = "0";
                dic["Message"] = "该晒图不存在或已被删除";
                return Json(dic, JsonRequestBehavior.AllowGet);
            }

            var isFavorite = false;
            if (!string.IsNullOrWhiteSpace(userId))
                isFavorite = await ArticleBll.IsFavoriteShareImages(id, userId);

            var user=HttpClientHelper.SelectUserInfoByUserId(detail.UserId);
            dic["Details"] = new
            {
                PKID = detail.Pkid,
                ImagesUrl = detail.ImagesUrl.Select(x => x).ToList(),
                UserName= GetUserName(user?.Nickname??"途虎用户"),
                UserHead = "http://image.tuhu.cn" + (user?.HeadImage??detail.UserHead),
                detail.Content,
                UserId = Guid.Parse(detail.UserId).ToString("B"),
                detail.LikesCount,
                CommentTimes = detail.CommentCount,
                detail.ShareCount,
                ImagesCount = detail.ImagesUrl.Count(),
                PublishTime = DateTimeHelper.GetShowTime(detail.CreateTime),
                IsFavorite = isFavorite,
                UserGrade = user?.UserGrade ?? "V0"
            };
            return JavaScript(JsonConvert.SerializeObject(dic,Newtonsoft.Json.Formatting.Indented,new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat="yyyy/MM/dd"}));
        }
        /// <summary>
        /// 晒图评论
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [CrossHost]
        public async Task<ActionResult> SelectShareImagesCommentListAsync(string userId,int id,int pageIndex=1)
        {
            var dic = new Dictionary<string, object> { ["Code"] = "1" };

            PagerModel page=new PagerModel(pageIndex,20);
            if (string.IsNullOrWhiteSpace(userId))
                userId = null;
            var comment = await DiscoverBLL.SelectShareImagesComment(id, userId, page);
            var result= comment.Select(x =>
            {

                var user = HttpClientHelper.SelectUserInfoByUserId(x.UserID);
                string userName = "";
                string parentName = "";
                string parentuserHead = "";
                string userHead = "";

                if (user != null)
                {
                    userName = user.Nickname;
                    userHead = "http://image.tuhu.cn" + user.HeadImage;
                }
                else
                {
                    userName = "途虎用户";
                    userHead = "http://resource.tuhu.cn/Image/Product/zhilaohu.png";
                }
                if (!string.IsNullOrWhiteSpace(x.ParentUserID))
                {
                    var parentUser = HttpClientHelper.SelectUserInfoByUserId(x.ParentUserID);
                    if (parentUser != null)
                    {
                        parentName = parentUser.Nickname;
                        parentuserHead = "http://image.tuhu.cn" + parentUser.HeadImage;
                    }
                    else
                    {
                        parentName = "途虎用户";
                        parentuserHead = "http://resource.tuhu.cn/Image/Product/zhilaohu.png";
                    }
                }
                return new
                {
                    x.ID,
                    x.PKID,
                    x.CommentContent,
                    x.CommentTime,
                    UserId = Guid.Parse(x.UserID).ToString("B"),
                    UserHead = userHead,
                    UserName = GetUserName(userName),
                    Praise = 0,
                    IsLike = false,
                    x.VehicleId,
                    x.Vehicle,
                    ReplyComment = !string.IsNullOrWhiteSpace(x.ParentUserID)
                        ? new
                        {
                            CommentContent = x.ParentCommentContent,
                            UserId = x.ParentUserID,
                            UserName = parentName,
                            UserHead = parentuserHead
                        }
                        : null,
                    UserGrade = user?.UserGrade ?? "VO"
                };

            }).ToList();
            dic["Comment"] = result;
            return JavaScript(JsonConvert.SerializeObject(dic, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy/MM/dd" }));
        }
        /// <summary>
        /// 喜欢晒图
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="favorite"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> FavoriteShareImagesByUserId(string userId, bool favorite, int id=0)
        {
            var dic=new Dictionary<string,object> { ["Code"] = "1" };
            if (string.IsNullOrWhiteSpace(userId)||id==0)
            {
                dic["Code"] = "0";
                dic["Message"] = "参数错误";
                return Json(dic);
            }
            if (favorite)
            {
                var result = await DiscoverBLL.InsertFavoriteShareImages(id, Guid.Parse(userId).ToString("B"));
                if (result > 0)
                {
                    await DiscoverBLL.UpdateShareImagesLikesStatisticsById(id, 1);
                    var user = HttpClientHelper.SelectUserInfoByUserId(userId);
                    var detail = await DiscoverBLL.FirstShareImagesByInfoIdAsync(id, userId);
                    var detailuser = HttpClientHelper.SelectUserInfoByUserId(detail.UserId);
                    //插入通知消息
                    string androidActivity;
                    string androidValue;
                    string iosActivity;
                    string iosValue;

                    var news = "赞了你晒的图片！";
                    androidActivity = "cn.TuHu.Activity.NewFound.Found.ImagesDetailActivity";
                    androidValue = "[{'PKID':" + id + "}]";
                    iosActivity = "Tuhu.DiscoverShowPicturesDetailVC";
                    iosValue = "{\"PKID\":\"" + id + "\"}";



                    HttpClientHelper.AcccessApi(HttpContext.Request, "/User/InsertPraiseNotice",
                        new Dictionary<string, string>
                        {
                            {"userHead", "http://image.tuhu.cn/" + user.HeadImage},
                            {"userName", GetUserName(user.Nickname)},
                            {"phoneNumber", detailuser.Cellphone},
                            {"userId", detail.UserId},
                            {"id", id.ToString()},
                            {"news", news},
                            {"androidKey", androidActivity},
                            {"iosKey", iosActivity},
                            {"androidValue", androidValue},
                            {"iosValue", iosValue}
                        });
                    //插入通知消息
                    //using (var pushClient = new PushClient())
                    //{
                    //    var pushMessage = new PushMessageModel
                    //    {
                    //        Type = MessageType.AppNotification,
                    //        CenterMsgType = "3私信",
                    //        PhoneNumbers = new List<string> { detailuser.Cellphone },
                    //        Content = "赞了你晒的图片！",
                    //        HeadImageUrl = "http://image.tuhu.cn/"+user.HeadImage,
                    //        InsertAppCenterMsg = true,
                    //        Title = GetUserName(user.Nickname),
                    //        SourceName = "faxian.tuhu.cn",
                    //        AndriodModel = new AndriodModel
                    //        {
                    //            AfterOpen = AfterOpenEnum.GoActivity,
                    //            AppActivity = "cn.TuHu.Activity.NewFound.Found.ImagesDetailActivity",
                    //            ExKey1 = "PKID",
                    //            ExValue1 = id.ToString()
                    //        },
                    //        IOSModel = new IOSModel
                    //        {
                    //            ExKey1 = "appoperateval",
                    //            ExValue1 = "Tuhu.DiscoverShowPicturesDetailVC",
                    //            ExKey2 = "keyvaluelenth",
                    //            ExValue2 = "{\"PKID\":\"" + id.ToString() + "\"}"
                    //        }
                    //    };
                    //var a = await pushClient.PushMessagesAsync(pushMessage);
                    //}
                }




            }
            else
            {
                var result = await DiscoverBLL.DeleteFavoriteShareImages(id, Guid.Parse(userId).ToString("B"));
                if (result > 0)
                    await DiscoverBLL.UpdateShareImagesLikesStatisticsById(id, -1);
            }

            return Json(dic);
        }

        [HttpPost]
        public async Task<ActionResult> SubmitShareImages()
        {
            var dic = new Dictionary<string, object> {["Code"] = "1"};
            Stream postData = Request.InputStream;
            StreamReader sRead = new StreamReader(postData);
            string postContent = sRead.ReadToEnd();
            sRead.Close();
            if (string.IsNullOrWhiteSpace(postContent))
            {
                dic["Code"] = "0";
                dic["Message"] = "参数错误";
                return Json(dic);
            }
            JObject jsonstr = null;
            try
            {
                jsonstr = JsonConvert.DeserializeObject<JObject>(postContent);
            }
            catch
            {
                dic["Code"] = "0";
                dic["Message"] = "错误的数据";
                return Json(dic);
            }

            string userId=jsonstr.Value<string>("userId");
            string content = jsonstr.Value<string>("content");
            dynamic imagesList = jsonstr.Value<dynamic>("imagesList");
            if (string.IsNullOrWhiteSpace(userId))
            {
                dic["Code"] = "0";
                dic["Message"] = "用户缺失";
                return Json(dic);
            }
            if (imagesList == null)
            {
                dic["Code"] = "0";
                dic["Message"] = "图片缺失";
                return Json(dic);
            }

            if (imagesList.Count == 0 || imagesList.Count > 9)
            {
                dic["Code"] = "0";
                dic["Message"] = "图片过少或过多";
                return Json(dic);
            }
            using (var dbHelper = Component.SystemFramework.DbHelper.CreateDbHelper())
            {
                dbHelper.BeginTransaction();
                var infoId = await DiscoverBLL.InsertShareImagesInfo(userId, content, dbHelper);
                if (infoId > 0)
                    if (await DiscoverBLL.InsertShareImagesStatistics(infoId, dbHelper) > 0)
                    {
                        foreach (var i in imagesList)
                        {
                            var result=await DiscoverBLL.InsertShareImagesDetail(infoId, i.ToString(), dbHelper);
                            if (result <= 0)
                            {
                                dbHelper.Rollback();
                                dic["Code"] = "0";
                                dic["Message"] = "发布失败";
                                return Json(dic);
                            }

                        }
                        dic["infoId"] = infoId;
                        dbHelper.Commit();
                        DiscoveryArticleRechargeProduceer.Send(DiscoveryArticleNotificationQueueName, new
                        {
                            Type = 11,
                            ActileId = infoId,
                            Vote = 1
                        });
                    }
            }
            return Json(dic);
        }

        [HttpPost]
        public async Task<ActionResult> UploadShareImages(int? index)
        {
            var dic = new Dictionary<string, object>();
            var file = Request.Files;
            if (file == null || file.Count == 0)
            {
                dic["Code"] = "0";
                dic["Message"] = "请选择要上传的图片";
                return Json(dic);
            }
            var path = await UtilityService.UploadImageAsync(file[0], "comment/shareimages/", 800, 800);
            if (!string.IsNullOrEmpty(path))
            {
                dic["Code"] = "1";
                dic["Index"] = index;
                dic["filename"] = path.GetImageUrl();
                return Json(dic);
            }
            //var postedFileBase = file[0];
            //string fileName = string.Concat("/comment/shareimages/", Guid.NewGuid().ToString(),
            //    (!string.IsNullOrWhiteSpace(postedFileBase?.FileName)
            //        ? Path.GetExtension(postedFileBase.FileName)
            //        : ".jpg"));
            //;
            //if (await postedFileBase.SaveAsRemoteFileAsync(fileName) > 0)
            //{
            //    dic["Code"] = "1";
            //    dic["Index"] = index;
            //    dic["filename"] = DomainConfig.ImageSite + fileName;
            //    return Json(dic);
            //}
            dic["Code"] = "0";
            dic["Message"] = "上传失败";
            return Json(dic);
        }

        /// <summary>
        /// 提交晒图评论
        /// </summary>
        /// <param name="id"></param>
        /// <param name="content"></param>
        /// <param name="parentId"></param>
        /// <param name="userId"></param>
        /// <param name="phone"></param>
        /// <param name="userHead"></param>
        /// <param name="userName"></param>
        /// <param name="userGrade"></param>
        /// <param name="realName"></param>
        /// <param name="sex"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SubmitShareImagesCommentById(int? id,string content,int? parentId,string userId,string phone,string userHead,string userName,string userGrade,string realName,string sex,string vehicle,string vehicleId)
        {
            var dic = new Dictionary<string, object> { ["Code"] = "1" };
            //关键字过滤 GOTO
            //过滤非法关键字
            if (string.IsNullOrWhiteSpace(userId) || !id.HasValue||string.IsNullOrWhiteSpace(content))
            {
                dic["Code"] = "0";
                dic["Message"] = "缺少参数";
                return Json(dic);
            }
            var isActive = await DiscoverBLL.SelectShareImagesStatusById(id.Value);
            if (!isActive.HasValue || !isActive.Value)
            {
                dic["Code"] = "0";
                dic["Message"] = "晒图尚未审核，请稍后评论！";
                return Json(dic);
            }
            int auditStatus = 2;
            try
            {
                //格式：22:00-08:00
                string timeLine = await DistributedCacheHelper.SelectDictionaryValueByKey("tbl_Question_Audit_Duration");
                if (!string.IsNullOrEmpty(timeLine) && DiscoveryController.IsLegalTime(DateTime.Now, timeLine))
                {
                    auditStatus = 0;
                }
                if (HttpClientHelper.AcccessApi(HttpContext.Request, "/Advertise/ValidateCommentContent",
                    new Dictionary<string, string> {{"userId", userId}, {"content", content}}) ==
                    "false")
                {
                    dic["Code"] = "0";
                    dic["Message"] = "存在非法词汇";
                    return Json(dic);
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException("验证非法字符出错", ex);
            }
            DiscoveryCommentModel c = new DiscoveryCommentModel
            {
                PKID = id.Value,
                ParentID = parentId,
                UserID = userId,
                PhoneNum = phone,
                UserHead = userHead,
                CommentContent = content,
                AuditStatus = auditStatus,
                UserName = userName,
                UserGrade = userGrade,
                RealName = realName,
                Sex = sex,
                Vehicle = vehicle,
                VehicleId = vehicleId
            };
            var result=await DiscoverBLL.InsertShareImagesComment(c);
            if (result <= 0)
            {
                dic["Code"] = "0";
                dic["Message"] = "评论失败";
                return Json(dic);
            }
            if(result>0&& auditStatus==2)
            {
                await DiscoverBLL.UpdateShareImagesCommentStatisticsById(id.Value, 1);
                var user = HttpClientHelper.SelectUserInfoByUserId(userId);
                //var detail = await DiscoverBLL.FirstShareImagesByInfoIdAsync(id.Value, null);
                var detailuserid = "";
                if (parentId.HasValue)
                {
                    var shc = await DiscoverBLL.SelectShareCommentByCommentId(parentId.Value);
                    if (shc == null)
                    {
                        return Json(dic);
                    }
                    detailuserid = shc.UserID;
                }
                else
                {
                    var detail = await DiscoverBLL.FirstShareImagesByInfoIdAsync(id.Value, null);
                    detailuserid = detail.UserId;
                }
                var detailuser = HttpClientHelper.SelectUserInfoByUserId(detailuserid);

                //插入通知消息
                string androidActivity;
                string androidValue;
                string iosActivity;
                string iosValue;

                var news = parentId.HasValue ? "回复了你的发言！" : "评论了你晒的图片！";
                androidActivity = "cn.TuHu.Activity.NewFound.Found.ImagesDetailActivity";
                androidValue = "[{'PKID':" + id + "}]";
                iosActivity = "Tuhu.DiscoverShowPicturesDetailVC";
                iosValue = "{\"PKID\":\"" + id + "\"}";



                HttpClientHelper.AcccessApi(HttpContext.Request, "/User/InsertPraiseNotice",
                    new Dictionary<string, string>
                    {
                            {"userHead", "http://image.tuhu.cn/" + user.HeadImage},
                            {"userName", GetUserName(user.Nickname)},
                            {"phoneNumber", detailuser.Cellphone},
                            {"userId", detailuserid},
                            {"id", id.ToString()},
                            {"news", news},
                            {"androidKey", androidActivity},
                            {"iosKey", iosActivity},
                            {"androidValue", androidValue},
                            {"iosValue", iosValue}
                    });
                ////插入通知消息
                //using (var pushClient = new PushClient())
                //{
                //    var pushMessage = new PushMessageModel
                //    {
                //        Type = MessageType.AppNotification,
                //        CenterMsgType = "3私信",
                //        PhoneNumbers = new List<string> {detailuser.Cellphone },
                //        Content = parentId.HasValue ? "回复了你的发言！" : "评论了你晒的图片！",
                //        HeadImageUrl = "http://image.tuhu.cn/" + user.HeadImage,
                //        InsertAppCenterMsg = true,
                //        Title = GetUserName(user.Nickname),
                //        SourceName = "faxian.tuhu.cn",
                //        AndriodModel = new AndriodModel
                //        {
                //            AfterOpen = AfterOpenEnum.GoActivity,
                //            AppActivity = "cn.TuHu.Activity.NewFound.Found.ImagesDetailActivity",
                //            ExKey1 = "PKID",
                //            ExValue1 = id.ToString()
                //        },
                //        IOSModel = new IOSModel
                //        {
                //            ExKey1 = "appoperateval",
                //            ExValue1 = "Tuhu.DiscoverShowPicturesDetailVC",
                //            ExKey2 = "keyvaluelenth",
                //            ExValue2 = "{\"PKID\":\"" + id.ToString() + "\"}"
                //        }
                //    };
                //    var a = await pushClient.PushMessagesAsync(pushMessage);
                //}
            }
            return Json(dic);
        }

        public async Task<ActionResult> SelectMyShareImagesListAsync(string userId, string targetUserId,int pageIndex=1)
        {
            var dic=new Dictionary<string, object>();
            if (string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(targetUserId))
            {
                dic["Code"] = "0";
                dic["Message"] = "参数错误";
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            dic["Code"] = "1";
            var mySubmitShareImages = await DiscoverBLL.SelectMyShareImages(string.IsNullOrEmpty(targetUserId), string.IsNullOrEmpty(targetUserId) ? userId : targetUserId, new PagerModel(pageIndex, 20));
            dic["mySubmitShareImages"] = mySubmitShareImages;
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> InsertShareImagesShareNumberStatisticsById(int? id)
        {
            var dic = new Dictionary<string, object>();
            if (!id.HasValue ||id==0)
            {
                dic["Code"] = "0";
                dic["Message"] = "参数错误";
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            var result=await DiscoverBLL.UpdateShareImagesShareNumberStatisticsById(id.Value, 1);
            dic["Code"] = result.ToString();
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 从App内分享出去的接口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CrossHost]
        public async Task<ActionResult> SelectShaiTuDetailForShareAsync(int id = 0)
        {
            var dic = new Dictionary<string, object> { ["Code"] = "1" };
            if (id == 0)
            {
                dic["Code"] = "0";
                dic["Message"] = "参数错误";
                return Json(dic, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var detail = await DiscoverBLL.SelectShaiTuShareByInfoIdAsync(id);
                if (detail == null)
                {
                    dic["Code"] = "0";
                    dic["Message"] = "该晒图不存在或审核不通过";
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                var userObject = HttpClientHelper.SelectUserInfoByUserId(detail.UserId);
                dic["Details"] = new
                {
                    #region 获取详细信息
                    PKID = detail.Pkid,
                    ImagesUrl = detail.ImagesUrl.Select(x => x).ToList(),
                    UserName = GetUserName(userObject?.Nickname ?? "途虎用户"),
                    UserHead = "http://image.tuhu.cn" + (userObject?.HeadImage ?? detail.UserHead),
                    detail.Content,
                    UserId = Guid.Parse(detail.UserId).ToString("B"),
                    detail.LikesCount,
                    CommentTimes = detail.CommentCount,
                    detail.ShareCount,
                    ImagesCount = detail.ImagesUrl.Count(),
                    PublishTime = DateTimeHelper.GetShowTime(detail.CreateTime),
                    IsFavorite = false,
                    UserGrade = userObject?.UserGrade ?? "V0"
                    #endregion
                };
                PagerModel page = new PagerModel(1, 20);
                var comment = await DiscoverBLL.SelectShareImagesComment(id, string.IsNullOrEmpty(detail.UserId) ? null : detail.UserId, page);
                dic["Comment"] = comment.Select(x =>
                {
                #region 获取评论列表
                var user = HttpClientHelper.SelectUserInfoByUserId(x.UserID);
                    string userName = "";
                    string parentName = "";
                    string parentuserHead = "";
                    string userHead = "";

                    if (user != null)
                    {
                        userName = user.Nickname;
                        userHead = "http://image.tuhu.cn" + user.HeadImage;
                    }
                    else
                    {
                        userName = "途虎用户";
                        userHead = "http://resource.tuhu.cn/Image/Product/zhilaohu.png";
                    }
                    if (!string.IsNullOrWhiteSpace(x.ParentUserID))
                    {
                        var parentUser = HttpClientHelper.SelectUserInfoByUserId(x.ParentUserID);
                        if (parentUser != null)
                        {
                            parentName = parentUser.Nickname;
                            parentuserHead = "http://image.tuhu.cn" + parentUser.HeadImage;
                        }
                        else
                        {
                            parentName = "途虎用户";
                            parentuserHead = "http://resource.tuhu.cn/Image/Product/zhilaohu.png";
                        }
                    }
                    return new
                    {
                        x.ID,
                        x.PKID,
                        x.CommentContent,
                        x.CommentTime,
                        UserId = Guid.Parse(x.UserID).ToString("B"),
                        UserHead = userHead,
                        UserName = GetUserName(userName),
                        Praise = 0,
                        IsLike = false,
                        x.VehicleId,
                        x.Vehicle,
                        ReplyComment = !string.IsNullOrWhiteSpace(x.ParentUserID)
                            ? new
                            {
                                CommentContent = x.ParentCommentContent,
                                UserId = x.ParentUserID,
                                UserName = parentName,
                                UserHead = parentuserHead
                            }
                            : null,
                        UserGrade = user?.UserGrade ?? "VO"
                    };
                #endregion
            }).ToList();
            }
            catch (Exception ex)
            {
                dic["Code"] = "0";
                dic["Message"] = ex.Message;
                WebLog.LogException("晒图分享接口异常", ex);
            }
            return JavaScript(JsonConvert.SerializeObject(dic, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy/MM/dd" }));
        }
        #endregion
        #region 收藏文章
        /// <summary>
        /// 收藏文章
        /// </summary>
        /// <param name="Id">当前文章ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AddFavoriate(string Id, string userId, string DeviceId = "")
        {
            if (!int.TryParse(Id, out int articleId) || !Guid.TryParse(userId, out Guid uid))
            {
                return Json(new FavoriteViewModel() { success = false, msg = "参数错误！" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                int result = await ArticleFavoriteBll.SaveArticleFavorite(new CommentLike
                {
                    PKID = articleId,
                    UserId = uid.ToString("B"),
                    Type = 0,
                    IsRead = 1,
                    OperateTime = DateTime.Now
                });
                //关注和取消关注需要通知MQ去更新ES数据
                if (result > 0)
                {
                    await CacheManager.RemoveFromCacheAsync($"VoteUserList/{articleId}");
                    //var Vote = await ArticleBll.IsFavoriteArticle(int.Parse(Id), Guid.Parse(userId).ToString("B"));
                    //DiscoveryArticleRechargeProduceer.Send(DiscoveryArticleNotificationQueueName, new
                    //{
                    //    Type = 5,
                    //    ActileId = int.Parse(Id),
                    //    UserId = Guid.Parse(userId).ToString("B"),
                    //    Vote = Vote
                    //});
                }
                return Json(new FavoriteViewModel() { success = result > 0, msg = result > 0 ? "操作成功！" : "操作失败！" }, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException e)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in e.EntityValidationErrors)
                {
                    foreach (var n in item.ValidationErrors)
                    {
                        sb.AppendLine(n.ErrorMessage + "||");
                    }
                }
                return Json(new FavoriteViewModel() { success = false, msg = e.Message + "-----" + e.StackTrace + "----" + sb.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new FavoriteViewModel() { success = false, msg = ex.Message + "-----" + ex.StackTrace }, JsonRequestBehavior.AllowGet);
            }




        }
        #endregion
        #region 根据条件获取文章列表
        /// <summary>
        /// 根据条件获取文章列表
        /// </summary>
        /// <param name="ArticleTitle"></param>
        /// <param name="ArticleTag"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [NonAction]
        private async Task<ArticleViewModel> GetArticleListByConditions(PagerModel pager, string ArticleTitle = "", string ArticleTag = "", string ArticleAuthor = "", int PageIndex = 1, int PageSize = 10)
        {
            ArticleViewModel vm = new ArticleViewModel();
            List<Article> list = new List<Article>();
            pager.CurrentPage = PageIndex;
            pager.PageSize = PageSize > 50 ? 50 : PageSize;
            ArticleTitle = ConvertEmoji2UnicodeHex(ArticleTitle);
            //头条的列表
            bool isToutiao = string.IsNullOrEmpty(ArticleTitle) && string.IsNullOrEmpty(ArticleAuthor) && string.IsNullOrEmpty(ArticleTag);
            
            if ((!string.IsNullOrEmpty(ArticleTitle) || !string.IsNullOrEmpty(ArticleAuthor)) && string.IsNullOrEmpty(ArticleTag))
            {
                list = await ArticleBll.SearchArticle(pager, keyword: ArticleTitle, author: ArticleAuthor);
            }
            else if (!string.IsNullOrEmpty(ArticleTag))
            {
                list = await ArticleBll.GetArticlesByCategoryId(Int32.Parse(ArticleTag), pager, ArticleTitle);
            }
            else if (!string.IsNullOrEmpty(ArticleAuthor))
            {
                list = await ArticleBll.SearchArticle(pager, author: ArticleAuthor);
            }
            else
            {
                list = await ArticleBll.SearchArticle(pager);
            }

            if (list.Count > 0)
            {
                await list.ForEachAsync(async u =>
                {
                    u.CommentCountNum = await ArticleBll.GetCommentCount(u.PKID);
                });
            }
            List<ArticleItem> toutiaoList = ArticleItem.ConvertToArtcleVM(list);
            List<object> combineData = await GetCombineArticleAndSpecialColumn(pager, toutiaoList);
            vm.data = isToutiao ? combineData : toutiaoList.ToList<object>();
            vm.TotalPage = pager.TotalPage;
            vm.CurrentPage = pager.CurrentPage;
            return vm;
        }
        /// <summary>
        /// 把专题插入到文章中
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="articleList"></param>
        /// <returns></returns>
        private async Task<List<object>> GetCombineArticleAndSpecialColumn(PagerModel pager,List<ArticleItem> articleList)
        {
            List<object> combineResult = new List<object>();
            PagerModel newPager = new PagerModel
            {
                CurrentPage = pager.CurrentPage,
                PageSize = pager.PageSize / 2
            };
            //获取专题
            var specialColumns = await SpecialColumnBll.SelectSpecialCloumns(newPager);
            if (specialColumns.Count > 0)
            {
                for (int i = 0; i < articleList.Count; i++)
                {
                    #region 文章和专题组合
                    combineResult.Add(articleList[i]);
                    if (i % 2 == 1)
                    {
                        SpecialColumn scModel = specialColumns.FirstOrDefault();
                        if (scModel != null)
                        {
                            combineResult.Add(scModel);
                            specialColumns.RemoveAt(0);
                        }
                    }
                    #endregion
                }
            }
            else
            {
                 combineResult.AddRange(articleList);
            }
            return combineResult;
        }

        /// <summary>
        /// emoji 表情转换
        /// </summary>
        /// <param name="emoji"></param>
        /// <returns></returns>
        [NonAction]
        private static string ConvertEmoji2UnicodeHex(string emoji)
        {
            if (string.IsNullOrWhiteSpace(emoji))
                return emoji;
            var reg = new Regex(@"\p{Cs}");
            foreach (Match mt in reg.Matches(emoji))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(mt.Value);
                string firstItem = Convert.ToString(bytes[0], 2); //获取首字节二进制

                int iv;
                if (bytes.Length == 1)
                {
                    //单字节字符
                    iv = Convert.ToInt32(firstItem, 2);
                }
                else
                {
                    //多字节字符
                    StringBuilder sbBinary = new StringBuilder();
                    sbBinary.Append(firstItem.Substring(bytes.Length + 1).TrimStart('0'));
                    for (int i = 1; i < bytes.Length; i++)
                    {
                        string item = Convert.ToString(bytes[i], 2);
                        item = item.Substring(2);
                        sbBinary.Append(item);
                    }

                    iv = Convert.ToInt32(sbBinary.ToString(), 2);
                }
                emoji = emoji.Replace(mt.Value, Convert.ToString(iv, 16).PadLeft(4, '0'));
            }
            return emoji;
        }
        #endregion

        #region 专题

        /// <summary>
        /// 专题详情页接口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CrossHost]
        public async Task<ActionResult> SelectSpecialColumnDetail(string id)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                int scId;//文章Id
                if (!int.TryParse(id, out scId))
                {
                    dic.Add("Code", "0");
                    dic.Add("Msg", "专题id不合法");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                SpecialColumn scModel = null;
                using (var reclient = CacheHelper.CreateCacheClient("SpecialColumnDetail"))
                {
                    var result = await reclient.GetOrSetAsync("SCDetailById/" + id, () => SpecialColumnBll.SelectSpecialColumnDetail(scId), TimeSpan.FromHours(1));
                    if (result.Value != null)
                        scModel = result.Value;
                    else
                        scModel =await SpecialColumnBll.SelectSpecialColumnDetail(scId);
                }
                if (scModel == null)
                {
                    dic.Add("Code", "0");
                    dic.Add("Msg", "专题不存在");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                dic.Add("Code", "1");
                dic.Add("Msg", "数据获取成功");
                dic.Add("Detail", scModel);
            }
            catch (Exception ex)
            {
                dic.Clear();
                dic.Add("Code", "0");
                dic.Add("Msg", ex.Message);
                WebLog.LogException("查询文章专题详情页的接口", ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        [CrossHost]
        public async Task<ActionResult> SelectAllSpecialColumn(int? PageIndex = 1, int? PageSize = 20)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                using (var client = CacheHelper.CreateCacheClient("ZhuanTi_List_Cache"))
                {
                    PagerModel pager = new PagerModel()
                    {
                        CurrentPage=PageIndex.Value,
                        PageSize=PageSize.Value
                    };
                    List<SpecialColumn> scList = null;
                    string key= string.Format("SpecialColumnList/{0}/{1}", PageIndex, PageSize);
                    var result = await client.GetOrSetAsync(key, () => SpecialColumnBll.SelectAllSpecialColumn(pager), TimeSpan.FromHours(1));
                    if (result.Success && result.Exception == null)
                    {
                        scList = result.Value;
                    }
                    else
                    {
                        scList = await SpecialColumnBll.SelectAllSpecialColumn(pager);
                    }
                    if (scList.Count > 0)
                    {
                        dic.Add("Code", "1");
                        dic.Add("Msg", "数据获取成功");
                        dic.Add("TotalPage", pager.TotalPage);
                        dic.Add("Detail", scList.Select(u => new
                        {
                            ID = u.ID,
                            ColumnName = u.ColumnName,
                            ColumnDesc = u.ColumnDesc,
                            ColumnImage = u.ColumnImage,
                            ShowTime = Models.Tools.ShowTime(u.CreateTime)
                        }));
                    }
                    else
                    {
                        dic.Add("Code", "0");
                        dic.Add("Msg", "未获取到数据");
                    }
                }
            }
            catch (Exception ex)
            {
                dic.Clear();
                dic.Add("Code", "0");
                dic.Add("Msg", ex.Message);
                WebLog.LogException("查询所有专题的接口", ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 评论列表
        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetCommentList(int ArticleId, int? PageIndex = 1, int? PageSize = 10, Guid? userId = null, CommentOrderBy orderby = CommentOrderBy.none)
        {
            PagerModel pager = new PagerModel();
            pager.CurrentPage = PageIndex.Value;
            pager.PageSize = PageSize.Value;
            var list = await GetComment(ArticleId, pager, orderby, userId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取评论
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        [NonAction]
        private async Task<CommentViewModel> GetComment(int ArticleId, PagerModel pager, CommentOrderBy orderby, Guid? userId = null)
        {
            CommentViewModel vm = new CommentViewModel();
            if (orderby == CommentOrderBy.none)
            {
                vm.hots = await CommentItem.ConvertToCommentVM(await CommentBll.GetCommentList(ArticleId, pager, CommentOrderBy.Like, userId));
                vm.news = await CommentItem.ConvertToCommentVM(await CommentBll.GetCommentList(ArticleId, pager, CommentOrderBy.CreateDateTime, userId));
                vm.article = (ArticleItem)await ArticleBll.GetArticleDetailById(ArticleId);
                vm.article.ContentHtml = null;
                vm.success = true;
            }
            else
            {
                vm.news = await CommentItem.ConvertToCommentVM(await CommentBll.GetCommentList(ArticleId, pager, CommentOrderBy.CreateDateTime, userId));
                vm.success = true;
            }
            return vm;

        }
        #endregion
        #region 评论
        /// <summary>
        /// 评论/回复
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="userId"></param>
        /// <param name="commentId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AddComment(int Id, Guid userId, string commentId, string content, string DeviceId = "")
        {
            CommentViewModel vm = new CommentViewModel();
            Comment cm = new Comment()
            {
                CommentContent = content,
                ParentID = commentId,
                UserId = userId.ToString("B"),
                PKID = Id
            };
            var result = await CommentBll.AddComment(cm);
            vm.success = result != null ? true : false;
            return Json(vm);
        }
        #endregion
        #region 删除评论
        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [CollectUserAction(UserOperationEnum.DeleteComment)]
        public async Task<JsonResult> RemoveComment(int commentId, Guid userId)
        {
            CommentViewModel vm = new CommentViewModel();
            var result = await CommentBll.RemoveComment(commentId, userId);
            vm.success = result != null ? true : false;
            return Json(vm);
        }
        #endregion
        #region 点赞
        /// <summary>
        /// 点赞/取消点赞
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="isLike"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> LikeOrNot1(int commentId, Guid userId, LikeOrNot isLike)
        {
            CommentViewModel vm = new CommentViewModel();
            vm.success = await CommentBll.UpdateAgree(commentId, userId, isLike);
            return Json(vm);
        }
        #endregion

        #region Utils
        /// <summary>
        /// 发送Bi请求
        /// </summary>
        /// <param name="Api">BI统计API接口</param>
        /// <param name="Data">数据</param>
        /// <param name="OriginUrl">http://faxian.tuhu.cn</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SendBiRequest(string Api, string Data, string OriginUrl, string callback)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(Data);
            WebClient client = new WebClient();
            client.Headers.Add("Accept", "application/json");
            client.Headers.Add("Content-Type", "application/json");
            client.Headers.Add("Request_Origin", OriginUrl);
            byte[] responseData = client.UploadData(Api, "POST", byteData);
            //返回值
            string responseStr = Encoding.UTF8.GetString(responseData);
            string[] cookies = client.ResponseHeaders.GetValues("Set-Cookie");
            string uid = cookies[0].Split(';').First();
            string sid = cookies[1].Split(';').First();
            if (String.IsNullOrWhiteSpace(callback))
            {
                return Json(new
                {
                    Index = responseStr,
                    UUID = uid.Split('=').Last(),
                    SID = sid.Split('=').Last()
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return JavaScript(callback + "(" + JsonConvert.SerializeObject(new
                {
                    Index = responseStr,
                    UUID = uid.Split('=').Last(),
                    SID = sid.Split('=').Last()
                }) + ")");
            }


        }

        public string GetRandomWord()
        {
            List<string> list = new List<string>()
            {
                "有想法就说，看对眼就上",
                "走过路过，评论不要错过",
                "少一些套路，多一些留言",
                "据说评论是小编创作的最大动力",
                "这儿还是块处女地，不要让别人抢先评论奥",
                "觉得这篇文章有用么？给句话儿吧",
                "沙发已经摆好，欢迎大神入座",
                "老司机，当滔滔不绝，不要停",
                "可以赞，可以喷，沉默不是金",
                "掌声已经准备好，请开始发言",
                "简单点，说话的方式简单",
                "看到美女了么？不得说点啥么？",
                "辛辛苦苦写文章，你却只看不说话",
                "产品妹妹想知道，此刻你在想啥"
            };
            Random rand = new Random();
            int n = rand.Next(0, list.Count);
            return list[n];
        }

        public List<string> GetByTags(string[] ids)
        {
            List<string> tagList = new List<string>();
            foreach (string item in ids)
            {
                switch (item)
                {
                    case "FU-TU-BBP|": tagList.Add("刹车片"); break;
                    case "FU-TU-BBD|": tagList.Add("刹车盘"); break;
                    case "FU-TU-ABP|": tagList.Add("刹车片"); break;
                    case "FU-TU-ABD|": tagList.Add("刹车盘"); break;
                    case "FU-BY-XBY|": tagList.Add("小保养服务"); break;
                    case "FU-BY-LM|": tagList.Add("空调制冷剂"); break;
                    case "FU-BY-RANYOU|": tagList.Add("燃油滤清器"); break;
                    case "FU-BY-KONGQI|": tagList.Add("空气滤清器"); break;
                    case "FU-BY-KONGTIAO|": tagList.Add("空调滤清器"); break;
                    case "FU-BY-YUSHUA|": tagList.Add("雨刷"); break;
                    case "FU-BY-DBY|": tagList.Add("大保养服务"); break;
                    case "FU-PM-ANZHUANGFEI|": tagList.Add("PM2.5滤芯"); break;
                    case "FU-TU-Shacheyou|": tagList.Add("刹车油"); break;
                    case "FU-TU-Huohuasai|": tagList.Add("火花塞"); break;
                    case "FU-TU-Jieqimen|": tagList.Add("气节门清洗"); break;
                    case "FU-TU-Fadongji|": tagList.Add("发动机清洗"); break;
                    case "FU-TU-Kongtiaoguanlu|": tagList.Add("空调管路清洗"); break;
                    case "ap-cleaner-Total": tagList.Add("防冻液"); break;
                    default: tagList.Add("其他"); break;
                }
            }
            return tagList;

        }

        /// <summary>
        /// 判断是否是App内请求
        /// </summary>
        /// <returns></returns>
        public bool IsAppRequest()
        {
            string userAgent = Request.ServerVariables["HTTP_USER_AGENT"];
            return (userAgent.Contains("tuhuAndroid") || userAgent.Contains("tuhuIOS")) ? true : false;
        }

        //public CoverAuthor GetCoverAuthor(string name)
        //{
        //    CoverAuthor defaultModel = new CoverAuthor();
        //    defaultModel.AuthorName = "虎小编";
        //    defaultModel.AuthorHead = "https://res.tuhu.org/Image/Product/zhilaohu.png";
        //    defaultModel.Description = "";
        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        try
        //        {
        //            CoverAuthor author = ArticleBll.SelectCoverAuthorByName(name);
        //            if (author != null)
        //                return author;
        //            else
        //            {
        //                defaultModel.AuthorName = name;
        //                return defaultModel;
        //            }
        //        }
        //        catch
        //        {
        //            return defaultModel;
        //        }
        //    }
        //    else
        //        return defaultModel;
        //}

        #endregion

        #region 晒单
        /// <summary>
        /// 晒单页面
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public ActionResult ShaiDanView(string id)
        {
            string api = string.Format("https://www.tuhu.cn/Community/SelectCommentDetail.aspx?id={0}", id);
            try
            {
                string jsonData = "";
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    jsonData = wc.DownloadString(api);
                }
                var dataModel = JsonConvert.DeserializeObject<ActionDataModel>(jsonData);
                if (dataModel.Code == "1")
                {
                    return View(dataModel.Detail);
                }
                else
                    return Json(new { success = false, msg = dataModel.Msg }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [CrossHost]
        public ActionResult SelectShaiDanDetail(string id)
        {
            var dic = new Dictionary<string, object>() { ["Code"] = "0", ["Msg"] = "请升级到最新版本" };
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 论坛帖子H5详情页

        public static string forumHost= "https://hushuoapi.tuhu.cn/v1";
        //public static string forumHost = "http://api.lietome.tuhu.test/v1";
        public static string SALT= "lietome@2017!";
        public ActionResult ForumShare(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new { success = false, msg = "参数错误"},JsonRequestBehavior.AllowGet);
            }
            //时间戳
            string timestamp = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString();
            string random = new Random().Next().ToString();
            string paramsStr = string.Format("user,category{0}{1}{2}", SALT, timestamp, random);
            string sign = Helper.HelpMethod.GetMD5(paramsStr);
            string api = string.Format("{0}/topics/{1}?include=user,category&nonce={2}&sign={3}&timestamp={4}", forumHost, id, random, sign, timestamp);
            try
            {
                string jsonData = HttpGet(api,"");
                var dataModel = JsonConvert.DeserializeObject<ForumDataModel>(jsonData);

                if (dataModel.code == "1")
                {
                    //解析论坛详情页的数据
                    var detailModel = JsonConvert.DeserializeObject<ForumDetailModel>(dataModel.data.ToString());
                    return View(detailModel);
                }
                else
                    return View("DetailNotFound");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ForumReply(string id)
        {
            //时间戳
            string timestamp = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString();
            string random = new Random().Next().ToString();
            string paramsStr = string.Format("user,replyto{0}{1}{2}", SALT, timestamp, random);
            string sign = Helper.HelpMethod.GetMD5(paramsStr);
            string api = string.Format("{0}/topics/{1}/replies?include=user,replyto&nonce={2}&sign={3}&timestamp={4}", forumHost, id, random, sign, timestamp);
            try
            {
                string jsonData = HttpGet(api, "");
                //using (WebClient wc = new WebClient())
                //{
                //    wc.Encoding = Encoding.UTF8;
                //    jsonData = wc.DownloadString(api);
                //}
                var dataModel = JsonConvert.DeserializeObject<ForumDataModel>(jsonData);
                if (dataModel.code == "1")
                {
                    //回复的数据
                    var replyList = JsonConvert.DeserializeObject<List<ForumReplyModel>>(dataModel.data.ToString());
                    return PartialView(replyList);
                }
                else
                    return Content("");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        public ActionResult PageNotFound() { return View("DetailNotFound"); }

        /// <summary>
        /// 发送Http请求--get方式
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        #endregion

        public async Task<ActionResult> ShaiTuDetail(string id)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                int pkid;
                if (!int.TryParse(id, out pkid))
                {
                    dic.Add("Code", "0");
                    dic.Add("Msg", "晒图id不合法");
                    return Json(dic, JsonRequestBehavior.AllowGet);
                }
                //查询晒图信息

            }
            catch (Exception ex)
            {
                dic.Clear();
                dic.Add("Code", "0");
                dic.Add("Msg", ex.Message);
                WebLog.LogException("查询晒图详情页的接口", ex);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ArticleView(string id)
        {
            int articleId;//文章Id
            if (!int.TryParse(id, out articleId))
                return Content("文章Id不合法");
            Article articleModel = null;
            using (var reclient = CacheHelper.CreateCacheClient("ArticleDetail"))
            {
                var result = await reclient.GetOrSetAsync("BaiduDetailById/" + id, () => ArticleBll.GetArticleDetailById(articleId), TimeSpan.FromHours(1));
                if (result.Value != null)
                    articleModel = result.Value;
            }
            if (articleModel == null)
                return Content("文章不存在");

            if (articleModel.Type == 5)
            {
                //不是发布状态
                if (articleModel.Status != ArticleStatus.Published.ToString())
                    return Content("文章不存在");
                //发表时间没有到
                else if (!articleModel.PublishDateTime.HasValue && articleModel.PublishDateTime.Value > DateTime.Now)
                    return Content("文章不存在");
            }
            else if (articleModel.IsShow != 1)
                return Content("文章不存在");

            return PartialView(articleModel);

        }

        public async Task<ActionResult> baijia()
        {
            XmlDocument mMainXmlDocument = new XmlDocument();
            XmlDocument imageXmllDocument = new XmlDocument();
            //编码
            XmlDeclaration nodeDeclar = mMainXmlDocument.CreateXmlDeclaration("1.0", System.Text.Encoding.UTF8.BodyName, "");
            mMainXmlDocument.AppendChild(nodeDeclar);
            XmlElement mRootXmlElement = mMainXmlDocument.CreateElement("rss");
            //版本信息
            mRootXmlElement.SetAttribute("version", "2.0");
            mMainXmlDocument.AppendChild(mRootXmlElement);

            //Channel标签
            XmlElement channelXmlElement = mMainXmlDocument.CreateElement("channel");
            mRootXmlElement.AppendChild(channelXmlElement);

            XmlElement rssXmlElement = mMainXmlDocument.CreateElement("title");

            XmlNode txtXmlNode = mMainXmlDocument.CreateTextNode("途虎养车");
            rssXmlElement.AppendChild(txtXmlNode);
            channelXmlElement.AppendChild(rssXmlElement);
            //Link标签
            rssXmlElement = mMainXmlDocument.CreateElement("link");
            txtXmlNode = mMainXmlDocument.CreateTextNode("https://www.tuhu.cn");
            rssXmlElement.AppendChild(txtXmlNode);
            channelXmlElement.AppendChild(rssXmlElement);
            //描述标签
            rssXmlElement = mMainXmlDocument.CreateElement("description");
            XmlNode cDataNode = mMainXmlDocument.CreateTextNode("专业的汽车养护电商平台！换轮胎、做保养，就选途虎养车！正品自营，全国1万家安装网点。");
            rssXmlElement.AppendChild(cDataNode);
            channelXmlElement.AppendChild(rssXmlElement);
            //图标
            rssXmlElement = mMainXmlDocument.CreateElement("image");

            var imageUrlXmlElement = mMainXmlDocument.CreateElement("url");
            imageUrlXmlElement.AppendChild(mMainXmlDocument.CreateTextNode("http://image.tuhu.cn/images/favicon.ico"));

            var imageTitleXmlElement = mMainXmlDocument.CreateElement("title");
            imageTitleXmlElement.AppendChild(mMainXmlDocument.CreateTextNode("途虎养车"));

            var imagelinkXmlElement = mMainXmlDocument.CreateElement("link");
            imagelinkXmlElement.AppendChild(mMainXmlDocument.CreateTextNode("https://www.tuhu.cn"));

            rssXmlElement.AppendChild(imageUrlXmlElement);
            rssXmlElement.AppendChild(imageTitleXmlElement);
            rssXmlElement.AppendChild(imagelinkXmlElement);

            channelXmlElement.AppendChild(rssXmlElement);

            //创建ITEM子节点，里面的数据可以根据自己的需求添加或者减少。title和link标签是必须的，不能少。
            //获取评论数据
            var comment = await ArticleComment.SelectOrderCommentByBaiJia();
            foreach (var i in comment)
            {

                var title = "";
                if (i.CommentProductId.IndexOf("TR-", StringComparison.Ordinal) == 0)
                {
                    //车型+连接符+车辆品牌+空格+轮胎的中文品牌+轮胎（固定的两个字）+空格+轮胎花纹简称+空格+轮胎规格+空格+作业（固定的两个字）
                    i.DisplayName = $"{i.CP_Brand} 轮胎 {i.CP_Tire_Pattern} {i.TireSize} {i.SpeedRating}";
                }
                if (i.CommentExtenstionAttribute?.CarID != null)
                {
                    var carObject = await ArticleComment.SelectCommentCarInfoByCarID(
                        i.CommentExtenstionAttribute.CarID.ToString());

                    title = (!string.IsNullOrWhiteSpace(carObject) ? carObject : "途虎车主：") + " " +
                            (!string.IsNullOrWhiteSpace(i.SingleTitle) ? i.SingleTitle : i.DisplayName) + " 作业";

                }
                else
                {
                    title = "途虎车主： " + (!string.IsNullOrWhiteSpace(i.SingleTitle) ? i.SingleTitle : i.DisplayName) +
                            " 作业";
                }
                var imageHtml = i.CommentImagesList.Aggregate("", (current, j) => current + $"<img src = \"{j}\" style=\"width:100%;\"/>");
                var descriptionHtml = $"<div style=\"margin-bottom:30px;\"><div style=\"font-size:16px;color:#454545;text-align:left;line-height:25px;margin-bottom:8px;\">{(i.CommentContent.Length > 200 ? i.CommentContent.Substring(0, 200) + "..." : i.CommentContent)}</div><div style=\"margin:12px 0;\">{imageHtml}</div></div>";
                XmlElement itemNode = mMainXmlDocument.CreateElement("item");
                rssXmlElement = mMainXmlDocument.CreateElement("title");
                txtXmlNode = mMainXmlDocument.CreateCDataSection(title);
                rssXmlElement.AppendChild(txtXmlNode);
                itemNode.AppendChild(rssXmlElement);
                //链接
                rssXmlElement = mMainXmlDocument.CreateElement("link");
                txtXmlNode = mMainXmlDocument.CreateTextNode($"http://faxian.tuhu.cn/Article/ShaiDanView?id={i.CommentId}");
                rssXmlElement.AppendChild(txtXmlNode);
                itemNode.AppendChild(rssXmlElement);
                //描述
                rssXmlElement = mMainXmlDocument.CreateElement("description");
                txtXmlNode = mMainXmlDocument.CreateCDataSection(descriptionHtml);
                rssXmlElement.AppendChild(txtXmlNode);
                itemNode.AppendChild(rssXmlElement);
                //发布日期
                rssXmlElement = mMainXmlDocument.CreateElement("pubDate");
                txtXmlNode = mMainXmlDocument.CreateTextNode(DateTime.Now.ToString(CultureInfo.InvariantCulture));
                rssXmlElement.AppendChild(txtXmlNode);
                itemNode.AppendChild(rssXmlElement);
                //作者
                rssXmlElement = mMainXmlDocument.CreateElement("source");
                txtXmlNode = mMainXmlDocument.CreateTextNode("途虎养车");
                rssXmlElement.AppendChild(txtXmlNode);
                itemNode.AppendChild(rssXmlElement);
                channelXmlElement.AppendChild(itemNode);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = new UTF8Encoding(false),
                    NewLineChars = Environment.NewLine
                };
                //要求缩进
                //注意如果不设置encoding默认将输出utf-16
                //注意这儿不能直接用Encoding.UTF8如果用Encoding.UTF8将在输出文本的最前面添加4个字节的非xml内容

                //设置换行符

                using (XmlWriter xmlWriter = XmlWriter.Create(ms, settings))
                {
                    mMainXmlDocument.WriteTo(xmlWriter);

                }
                Response.ContentType = "text/xml";

                //将xml内容输出到控制台中
                string xml = Encoding.UTF8.GetString(ms.ToArray());
                Response.Write(xml);
                return Content("");
            }
        }
        [CrossHost]
        public async Task<ActionResult> SelectZhongCeArticle(int pageIndex=1,int pageSize=20)
        {
            var dic = new Dictionary<string, object> {["Code"] = "1"};
            List<ZhongCeArticleModel> article = null;
            using (var reclient = CacheHelper.CreateCacheClient("ArticleDetail"))
            {
                var orSetAsync = await reclient.GetOrSetAsync($"Article_ZhongCeArticle_{pageIndex}_{pageSize}",
                    () => SelectZhongCeArticle(new PagerModel(pageIndex, pageSize)), TimeSpan.FromMinutes(10));

                article = orSetAsync?.Value?.ToList() ?? (await SelectZhongCeArticle(new PagerModel(pageIndex, pageSize)))?.ToList();
            }
            
            if (article == null)
            {
                dic["Code"] = "0";
                dic["Message"] = "无数据";
            }
            else
            {
                await article.ForEachAsync(async x =>
                {
                    using (var reclient = CacheHelper.CreateCacheClient("ArticleDetail"))
                    {
                        var orSetAsync = reclient?.GetOrSetAsync($"Article_DetailCommentsTopNumById_{x.PKID}",
                            () => CommentBll.SelectCommentsTopNum(x.PKID, 10), TimeSpan.FromHours(1));
                        if (orSetAsync != null)
                            x.CommentCountNum =(await orSetAsync)?.Value?.Item2 ??0;
                        x.ContentUrl = $"{DomainConfig.FaXian}/react/findDetail/?bgColor=%23ffffff&textColor=%23333333&id={x.PKID}";
                    }
                });
                dic["article"] = article;
            }
            return Json(dic,JsonRequestBehavior.AllowGet);
        }

        public static async Task<IEnumerable<ZhongCeArticleModel>> SelectZhongCeArticle(PagerModel pager)
        {
            var source = HttpRuntime.Cache["Article_ZhongCe"] as IEnumerable<ZhongCeArticleModel>;
            if (source == null)
            {
                source =await DiscoverBLL.SelectZhongCeArticle();
                if (source != null && source.Any())
                {
                    source = source.Where(x =>
                    {
                        if (string.IsNullOrWhiteSpace(x.CategoryTags))
                            return false;
                        try
                        {
                            var obj = JsonConvert.DeserializeObject<List<JObject>>(x.CategoryTags)
                                .Select(y => new Category() {Id = y.Value<int>("key"), Name = y.Value<string>("value")})
                                .ToList();
                            if (obj.Any(y => y.Id == 11867))
                            {
                                return true;
                            }
                        }
                        catch (Exception)
                        {
                            if (x.CategoryTags.Contains("11867"))
                                return true;
                        }
                        return false;
                    }).ToArray();
                    HttpRuntime.Cache.Insert("Article_ZhongCe", source, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration);
                }
            }
            if (source==null)
                return new ZhongCeArticleModel[0];
            return source.ToArray().OrderByDescending(x=>x.PKID).Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();

        }
        #region 论坛页面转换
        ///// <summary>
        ///// 同步文章
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult GetRenderDetail(string url)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(url) && !url.StartsWith("http://www.tuhu.org"))
        //        {
        //            return Json(new { obj = false }, JsonRequestBehavior.AllowGet);
        //        }
        //        string username = "小马过河";
        //        string passwd = GetMD5("cherry106");
        //        string loginUrl = "http://www.tuhu.org/member.php?mod=logging&action=login&loginsubmit=yes&infloat=yes&lssubmit=yes&inajax=1";
        //        string postData = "fastloginfield=" + username + "&username=" + username + "&password=" + passwd + "&quickforward=yes&handlekey=ls";
        //        CookieContainer cookie = new CookieContainer();
        //        PostLogin(postData, loginUrl, ref cookie);
        //        string html = GetHTML(url, cookie);
        //        var article = ConvertToArticle(html);
        //        article.ContentUrl = url;
        //        article.Brief = article.SmallTitle;
        //        var list = new List<Article>();
        //        list.Add(article);
        //        list = ReplaceImg(list);
        //        list = ArticleBll.SyncArticle(list);
        //        return Json(new { list = list }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.LogException(ex);
        //        return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
        //    }

        //}
        ///// <summary>
        ///// 同步数据
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult GetRenderData(int index)
        //{
        //    try
        //    {
        //        if (index > 13 || index < 1)
        //        {
        //            return Json(new { already = true }, JsonRequestBehavior.AllowGet);
        //        }
        //        var hasSync = ArticleBll.HasSyncArticle(index);
        //        if (hasSync)
        //        {
        //            return Json(new { already = true }, JsonRequestBehavior.AllowGet);
        //        }
        //        System.Diagnostics.Stopwatch sww = new System.Diagnostics.Stopwatch();
        //        sww.Start();
        //        var allArticles = GetAllArticles(index);
        //        sww.Stop();
        //        var AllArticles = sww.Elapsed;
        //        sww.Reset();
        //        sww.Start();
        //        allArticles = ReplaceImg(allArticles);
        //        sww.Stop();
        //        var Img = sww.Elapsed;
        //        sww.Reset();
        //        sww.Start();
        //        var list = ArticleBll.SyncArticle(allArticles);
        //        sww.Stop();
        //        var arts = sww.Elapsed;
        //        return Json(new { AllArticles = AllArticles, Img = Img, arts = arts, list = list }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.LogException(ex);
        //        return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="allArticles"></param>
        /// <returns></returns>
        private List<Article> ReplaceImg(List<Article> allArticles)
        {
            var reg = new Regex(@"<[img]+\s*(.*?)file=['""](?<file>.*?)['""](.*?)>",
                                    RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            var regImg = new Regex(@"<[img]+\s*(.*?)src=['""](?<src>.*?)['""](.*?)>",
                                    RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            string imgDir = WebConfigurationManager.AppSettings["UploadDoMain_image"];
            string domain = WebConfigurationManager.AppSettings["DoMain_image"];
            using (var client = new Tuhu.Service.Utility.FileUploadClient())
            {
                foreach (var item in allArticles)
                {
                    if (reg.IsMatch(item.ContentHtml) && reg.Matches(item.ContentHtml).Count > 0)
                    {
                        foreach (Match m in reg.Matches(item.ContentHtml))
                        {
                            var imgSrc = m.Groups["file"] != null ? m.Groups["file"].Value : string.Empty;
                            if (!string.IsNullOrEmpty(imgSrc))
                            {
                                var imgBytes = ImgRequest("http://www.tuhu.org/" + imgSrc);
                                imgSrc = ImgUpload(imgBytes, imgDir, client);
                                var img = @" <img src=""" + domain + imgSrc + @""" />";
                                item.ContentHtml = item.ContentHtml.Replace(m.Groups["0"].Value, img);
                            }
                        }
                    }
                    else if (regImg.IsMatch(item.ContentHtml) && regImg.Matches(item.ContentHtml).Count > 0)
                    {
                        foreach (Match m in regImg.Matches(item.ContentHtml))
                        {
                            var imgSrc = m.Groups["src"] != null ? m.Groups["src"].Value : string.Empty;
                            if (!string.IsNullOrEmpty(imgSrc))
                            {
                                var imgBytes = ImgRequest(imgSrc);
                                imgSrc = ImgUpload(imgBytes, imgDir, client);
                                var img = @" <img src=""" + domain + imgSrc + @""" />";
                                item.ContentHtml = item.ContentHtml.Replace(m.Groups["0"].Value, img);
                            }
                        }
                    }
                }
            }
            return allArticles;
        }

        /// <summary>
        /// 获取所有花纹测评文章
        /// </summary>
        /// <returns></returns>
        private List<Article> GetAllArticles(int index)
        {
            string tuhuorg = $"http://www.tuhu.org/forum-48-{index}.html";
            List<string> articleUrls = new List<string>();
            List<Article> articles = new List<Article>();
            Regex regList = new Regex(@"<(?<HtmlTag>[\w]+)[^>]*\s[cC][lL][aA][sS][sS]=(?<Quote>[""""']?)s xst(?(Quote)\k<Quote>)[""""']?[^>]*>((?<Nested><\k<HtmlTag>[^>]*>)|</\k<HtmlTag>>(?<-Nested>)|.*?)*</\k<HtmlTag>>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture);
            Regex regDetail = new Regex(@"<[a]+\s*(.*?)href=['""](?<href>.*?)['""](.*?)>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            //列表页所有文章链接
            string list = PostRequest(tuhuorg, null);
            if (regList.IsMatch(list))
            {
                if (regList.Matches(list).Count > 0)
                {
                    foreach (Match m in regList.Matches(list))
                    {
                        var url = regDetail.Match(m.Groups["0"].Value).Groups["href"].Value;
                        articleUrls.Add("http://www.tuhu.org/" + url);
                    }
                }
            }
            string username = "小马过河";
            string passwd = GetMD5("cherry106");
            string loginUrl = "http://www.tuhu.org/member.php?mod=logging&action=login&loginsubmit=yes&infloat=yes&lssubmit=yes&inajax=1";
            string postData = "fastloginfield=" + username + "&username=" + username + "&password=" + passwd + "&quickforward=yes&handlekey=ls";
            CookieContainer cookie = new CookieContainer();
            PostLogin(postData, loginUrl, ref cookie);
            foreach (var item in articleUrls)
            {
                var detail = GetHTML(item, cookie);
                var article = ConvertToArticle(detail);
                article.ContentUrl = item;
                article.Brief = "tuhuorg_page" + index;
                articles.Add(article);
            }
            return articles;
        }
        /// <summary>
        /// 抓取页面数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<ActionResult> Render(string url = "")
        {
            if (string.IsNullOrEmpty(url))
            {
                return HttpNotFound();
            }
            var article = await ArticleBll.GetArticleByContentUrl(url);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View("DetailForRender", article);
        }
        /// <summary>
        /// 获取html
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [NonAction]
        private string GetHTML(string url, CookieContainer cookie)
        {

            try
            {
                var html = PostRequest(url, cookie);
                if (string.IsNullOrEmpty(html))
                {
                    throw new Exception("抓取页面失败");
                }
                return html;
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                return "";
            }

        }
        /// <summary>
        /// 获取MD5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [NonAction]
        private string GetMD5(string str)
        {
            byte[] encodedPassword = new UTF8Encoding().GetBytes(str);
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
            string encoded = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            return encoded;
        }
        /// <summary>
        /// 提取信息
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        [NonAction]
        private Article ConvertToArticle(string HTML)
        {
            Article art = new Article();
            //匹配标题
            Regex reg = new Regex("(?<=<span id=\"thread_subject\">).*(?=</span>)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (reg.IsMatch(HTML))
            {
                art.SmallTitle = reg.Match(HTML).Value;
                art.BigTitle = reg.Match(HTML).Value;
            }
            reg = new Regex("<img id=[\"|']aimg_.*[\"|']/>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            //匹配文章内容
            reg = new Regex(@"<(?<HtmlTag>[\w]+)[^>]*\s[cC][lL][aA][sS][sS]=(?<Quote>[""']?)pcb(?(Quote)\k<Quote>)[""']?[^>]*>((?<Nested><\k<HtmlTag>[^>]*>)|</\k<HtmlTag>>(?<-Nested>)|.*?)*</\k<HtmlTag>>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture);
            if (reg.IsMatch(HTML))
            {
                var content = reg.Match(HTML).Groups["0"].Value;
                art.ContentHtml = content;
                art.Content = content;
                art.CoverMode = CoverMode.NoPicMode.ToString();
                art.CreateDateTime = new DateTime(2014, 1, 1);
                art.PublishDateTime = new DateTime(2014, 1, 1);
                art.CoverTag = "途虎养车";
                art.Type = 100;
                art.LastUpdateDateTime = new DateTime(2014, 1, 1);
                art.SmallImage = "";
                art.Image = "";
            }
            return art;
        }
        /// <summary>
        /// 请求页面
        /// </summary>
        /// <param name="postData"></param>
        /// <param name="requestUrlString"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        private string PostRequest(string requestUrlString, CookieContainer cookie)
        {
            try
            {
                string html = string.Empty;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrlString);
                request.Accept = "*/*"; //接受任意文件
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 Safari/537.36";
                request.AllowAutoRedirect = true;//是否允许302
                request.CookieContainer = cookie;
                request.Referer = requestUrlString; //当前页面的引用
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    html = reader.ReadToEnd();
                    stream.Close();
                    response.Close();
                    request.Abort();
                    return html;
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                return null;
            }
        }
        /// <summary>
        /// 读取图片
        /// </summary>
        /// <param name="requestUrlString"></param>
        /// <returns></returns>
        private byte[] ImgRequest(string requestUrlString)
        {
            return null;
            //try
            //{
            //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrlString);
            //    request.Accept = "*/*"; //接受任意文件
            //    request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 Safari/537.36";
            //    request.AllowAutoRedirect = true;//是否允许302
            //    request.Referer = requestUrlString; //当前页面的引用
            //    request.KeepAlive = false;
            //    request.AllowReadStreamBuffering = false;
            //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            //    {
            //        Stream stream = response.GetResponseStream();
            //        System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
            //        var img=image.SaveAsByteArray();
            //        stream.Close();
            //        response.Close();
            //        request.Abort();
            //        return img;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    WebLog.LogException(ex);
            //    return null;
            //}

        }
        /// <summary>
        /// 读取图片
        /// </summary>
        /// <param name="requestUrlString"></param>
        /// <returns></returns>
        private string ImgUpload(byte[] img, string dir, Tuhu.Service.Utility.FileUploadClient client)
        {
            try
            {
                var result = client.UploadImage(new ImageUploadRequest(dir, img));
                if (result.Success)
                {
                    return result.Result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                return null;
            }

        }
        /// <summary>
        /// 模拟登陆
        /// </summary>
        /// <param name="postData"></param>
        /// <param name="requestUrlString"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public bool PostLogin(string postData, string requestUrlString, ref CookieContainer cookie)
        {
            try
            {
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] data = encoding.GetBytes(postData);
                //向服务端请求
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(requestUrlString);
                myRequest.Method = "POST";
                myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.ContentLength = data.Length;
                myRequest.CookieContainer = new CookieContainer();
                Stream newStream = myRequest.GetRequestStream();
                newStream.Write(data, 0, data.Length);
                newStream.Close();
                //将请求的结果发送给客户端(界面、应用)
                using (HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse())
                {
                    cookie.Add(myResponse.Cookies);
                    StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                    return reader.ReadToEnd() != string.Empty;
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
                return false;
            }

        }
        #endregion


        public async Task<ActionResult> BaiduArticle(int? id)
        {
            if (id.HasValue)
            {
                Article article = null;
                using (var reclient = CacheHelper.CreateCacheClient("ArticleDetail"))
                {
                    var result = await reclient.GetOrSetAsync("BaiduArticle/" + id, () => ArticleBll.GetArticleDetailById(id.Value), TimeSpan.FromHours(1));
                    if (result.Value != null)
                        article = result.Value;
                }
                if (article == null)
                    return HttpNotFound();
                var authorModel = await ArticleSystem.SelectCoverAuthorByName(article.CoverTag);
                ViewBag.authorModel = authorModel;
                return View(article);
            }
            else
                return HttpNotFound();
        }
    }
}


