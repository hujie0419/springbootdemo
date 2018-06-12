using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tuhu.WebSite.Component.Discovery.Business;
using Tuhu.WebSite.Component.Discovery.BusinessData;
using Tuhu.WebSite.Component.SystemFramework;
using Tuhu.WebSite.Component.SystemFramework.Models;
using Tuhu.WebSite.Web.Activity.BusinessFacade;
using Tuhu.WebSite.Web.Activity.Models;

namespace Tuhu.WebSite.Web.Activity.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public async Task<ActionResult> Index()
        {
            if (Request.Url.Host.Contains("tuhu.cn"))
            {
                return RedirectPermanent(DomainConfig.WwwSite);
            }
            else
            {
                var pager = new PagerModel
                {
                    PageSize = 20
                };
                var list = await ArticleBll.SearchArticle(pager);
                return PartialView("/Views/Article/List.cshtml", list);
            }

        }

        [OutputCache(CacheProfile = "DefaultCacheProfile", VaryByParam = "*")]
        [HttpPost]
        public ActionResult ArticleComment(int PKID, string UserID, int PageIndex = 1)
        {
            var PageSize = 10;
            var comments = ArticleCommentSystem.SelectArticleCommentByPKID(PKID, UserID, PageIndex, PageSize)
                .Select(a => new
                {
                    a.AuditStatus,
                    a.Category,
                    a.CommentContent,
                    CommentTime = formatCommentTime(a.CommentTime),
                    a.PhoneNum,
                    a.Title,
                    a.UserHead,
                    a.UserName,
                    a.UserGrade,
                    a.floor
                });
            return Json(comments);
        }

        /// <summary>
        /// 最新评论
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="UserID"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ArticleCommentV1(int PKID, string UserID, int PageIndex = 1)
        {
            var PageSize = 10;
            int TotalCount = 0;
            var comments = ArticleCommentSystem.GetArticleCommentByPKID(PKID, UserID,out TotalCount, PageIndex, PageSize)
                .Select(a => new
                {
                    a.ID,
                    a.AuditStatus,
                    a.Category,
                    a.CommentContent,
                    CommentTime = formatCommentTime(a.CommentTime),
                    CommentNum = CountComment(a.ID),
                    PraiseNum = CountPraise(a.ID),
                    a.PhoneNum,
                    a.Title,
                    UserHead= a.UserHead.Replace("img", "image"),
                    UserName = GetUserName(a.UserName, a.RealName, a.PhoneNum, a.Sex),
                    a.UserGrade,
                    a.floor,
                    ParentName = GetUserNameByID(a.ParentID),
                    IsPraise = GetIsPraise(a.ID, UserID)
                });

            return Json(comments);
        }


        /// <summary>
        /// 热门评论
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ArticleCommentTop3(int PKID, string UserID)
        {
            var modeltTop3 = ArticleCommentSystem.GetArticleCommentTop3(PKID, UserID)
                    .Select(a => new
                    {
                        a.ID,
                        a.AuditStatus,
                        a.Category, 
                        a.CommentContent,
                        CommentTime = formatCommentTime(a.CommentTime),
                        CommentNum = CountComment(a.ID),
                        PraiseNum = CountPraise(a.ID),
                        a.PhoneNum,
                        a.Title,
                        UserHead= a.UserHead.Replace("img", "image"),
                        UserName = GetUserName(a.UserName, a.RealName, a.PhoneNum, a.Sex),
                        a.UserGrade,
                        a.floor,
                        a.ParentID,
                        ParentName = GetUserNameByID(a.ParentID),
                        IsPraise = GetIsPraise(a.ID, UserID)
                    });

            return Json(modeltTop3);
        }

        [HttpPost]
        public ActionResult GetCountCommentBYId(int PKID, string UserID)
        {

            return Json(ArticleCommentSystem.GetCountCommentBYId(PKID, UserID));
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
                return "游客";
            }
        }

        /// <summary>
        /// 评论总数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int CountComment(int id)
        {
            return ArticleCommentSystem.CountComment(id);
        }
        /// <summary>
        /// 点赞总数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int CountPraise(int id)
        {
            return ArticleCommentSystem.CountPraise(id);
        }

        [HttpPost]
        public ActionResult InsertCommentPraise(CommentPraise model)
        {
            if (string.IsNullOrWhiteSpace(model.UserId))
            {
                return Json(200);
            }
            //if (!string.IsNullOrWhiteSpace(model.RealName) && !string.IsNullOrWhiteSpace(model.Sex))
            //{
            //    if (model.Sex.Trim().Equals("男"))
            //    {
            //        model.RealName = model.RealName.Substring(0, 1) + "先生";
            //    }
            //    else if (model.Sex.Trim().Equals("女"))
            //    {
            //        model.RealName = model.RealName.Substring(0, 1) + "小姐";
            //    }
            //}

            if (!string.IsNullOrWhiteSpace(model.PhoneNum))
            {
                model.PhoneNum = model.PhoneNum.Substring(0, 3) + "****" + model.PhoneNum.Substring(7, 4);
            }

            return Json(ArticleCommentSystem.InsertCommentPraise(model));
        }

        public int GetIsPraise(int commentId, string userId)
        {
            int i = ArticleCommentSystem.GetIsPraise(commentId, userId);
            return i;

        }
        public string GetUserNameByID(string id)
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
                return "<span style='color:blue;'>回复</span> " + name + "：";
            }

        }
        public string formatCommentTime(DateTime d)
        {
            var now = DateTime.Now;
            var yesterday = now.AddDays(-1);
            if (now.Year == d.Year && now.Month == d.Month && now.Day == d.Day)//今天
            {
                return "今天 " + d.ToString("HH:mm");
            }
            else if (yesterday.Year == d.Year && yesterday.Month == d.Month && yesterday.Day == d.Day)//昨天
            {
                return "昨天 " + d.ToString("HH:mm");
            }
            else
            {
                return d.ToString("yyyy-MM-dd HH:mm");
            }
        }
        [OutputCache(CacheProfile = "DefaultCacheProfile", Duration = 7200)]
        public ActionResult ArticleComment()
        {

            return View();
        }
    }
}