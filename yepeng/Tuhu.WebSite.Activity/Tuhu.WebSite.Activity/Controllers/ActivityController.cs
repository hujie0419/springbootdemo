using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Tuhu.Nosql;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Utility.Models;
using Tuhu.Service.Utility.Request;
using Tuhu.WebSite.Component.SystemFramework.Extensions;
using Tuhu.WebSite.Web.Activity.BusinessFacade;
using Tuhu.WebSite.Web.Activity.Models;
using Tuhu.WebSite.Web.Activity.Models.Activity;
using Tuhu.WebSite.Web.Activity.Models.Enum;
using AjaxHelper = Tuhu.WebSite.Web.Activity.Models.WebResult.AjaxHelper;
using log4net;
using log4net.Core;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models;
using ActivityBuild = Tuhu.WebSite.Web.Activity.Models.ActivityBuild;
using DownloadApp = Tuhu.WebSite.Web.Activity.Models.DownloadApp;

namespace Tuhu.WebSite.Web.Activity.Controllers
{
    public class ActivityController : Controller
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ActivityController));
        private static string htmlActivity = string.Empty;

        private readonly Lazy<ActivitySystem> lazyActivitySystem = new Lazy<ActivitySystem>();

        private ActivitySystem ActivitySystem
        {
            get { return this.lazyActivitySystem.Value; }
        }

        public ActionResult RemoveCache(string id)
        {
            //HuodongCacheHelper.RemoveAllCache(id);
            HttpRuntime.Cache.Remove(id.ToString());
            return this.Content("key = " + id + "   缓存清除成功!");
        }

        [OutputCache(Duration = 300)]
        public ActionResult Index(int id)
        {
            Response.Cache.SetOmitVaryStar(true);
            ActivityBuild activityBuild = ActivitySystem.GetActivityBuildById(id).ToList().FirstOrDefault();
            //object obj = HuodongCacheHelper.GetCache(id.ToString());
            //object obj = HttpRuntime.Cache.Get(id.ToString());
            //if (obj != null)
            //{
            //    activityBuild = obj as ActivityBuild;
            //}
            //else
            //{
            //    activityBuild = ActivitySystem.GetActivityBuildById(id).ToList().FirstOrDefault();

            //    //System.TimeSpan ts = new TimeSpan(0, 1, 0, 0);          
            //    //HuodongCacheHelper.SetCache(id.ToString(), activityBuild, ts);
            //    HttpRuntime.Cache.Add(id.ToString(), activityBuild, null, DateTime.Now.AddHours(12), Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
            //}

            htmlActivity = GetActivityContent(activityBuild);

            return this.Content(htmlActivity);
        }

        [HttpGet]
        public ActionResult GeDownloadAppById(string id)
        {
            int newId = 0;
            try
            {
                int.TryParse(id, out newId);
                return Json(ActivitySystem.GeDownloadAppById(newId).ToList().FirstOrDefault(), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new List<DownloadApp>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetBatteryBanner()
        {
            var jsonModel = ActivitySystem.GetBatteryBanner().Select(x => new
            {
                Id = x.Id,
                Image = x.Image,
                ShowTime = x.ShowTime.ToString("yyyy-MM-dd hh:mm;ss"),
                Province = x.Province,
                City = x.City,
                Display = x.Display,
                Available = x.Available
            }).FirstOrDefault();

            return Json(jsonModel, JsonRequestBehavior.AllowGet);
        }

        private static string GetActivityContent(ActivityBuild model)
        {
            if (string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Content))
            {
                return "";
            }

            string pureHtml = GetPureHtml("/ActivityHtml/ActivityTemplate.html");

            pureHtml = pureHtml.Replace("$title$", model.Title);
            var activityContent = JsonConvert.DeserializeObject<List<ActivityContent>>(model.Content);

            string content = string.Empty;
            foreach (var item in activityContent.OrderBy(x => x.OrderBy).ToList())
            {
                if (!string.IsNullOrWhiteSpace(item.PID))
                {
                    if (item.Type == 0 && item.BigImg == 1) //轮胎,1图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"img\" iostype=\"#testapp#customSegue#TNTireInfoVC#\" androidtype=\"cn.TuHu.Activity.TireInfoUI\" pid=\"" + item.PID + "\"   vid=\"" + item.VID + "\"  activityid=\"" + item.ActivityId + "\" ><img src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 1 && item.BigImg == 1) //车品,1图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"img\" iostype=\"#testapp#customSegue#TNGoodsListDetailViewController#\" androidtype=\"cn.TuHu.Activity.AutomotiveProducts.AutomotiveProductsDetialUI\" pid=\"" + item.PID + "\"   vid=\"" + item.VID + "\" activityid=\"" + item.ActivityId + "\"><img src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 0 && item.BigImg == 0) //轮胎,2图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"content\" iostype=\"#testapp#customSegue#TNTireInfoVC#\" androidtype=\"cn.TuHu.Activity.TireInfoUI\" pid=\"" + item.PID + "\"   vid=\"" + item.VID + "\" activityid=\"" + item.ActivityId + "\"><img src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 1 && item.BigImg == 0)//车品,2图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"content\" iostype=\"#testapp#customSegue#TNGoodsListDetailViewController#\" androidtype=\"cn.TuHu.Activity.AutomotiveProducts.AutomotiveProductsDetialUI\" pid=\"" + item.PID + "\"   vid=\"" + item.VID + "\"  activityid=\"" + item.ActivityId + "\"><img src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 0 && item.BigImg == 2)//轮胎,3图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"content\" iostype=\"#testapp#customSegue#TNTireInfoVC#\" androidtype=\"cn.TuHu.Activity.TireInfoUI\" pid=\"" + item.PID + "\"   vid=\"" + item.VID + "\" activityid=\"" + item.ActivityId + "\"><img class=\"img3\" src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 1 && item.BigImg == 2)//轮胎,3图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"content\" iostype=\"#testapp#customSegue#TNGoodsListDetailViewController#\" androidtype=\"cn.TuHu.Activity.AutomotiveProducts.AutomotiveProductsDetialUI\" pid=\"" + item.PID + "\"   vid=\"" + item.VID + "\" activityid=\"" + item.ActivityId + "\"><img class=\"img3\" src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                }
                else
                {
                    if (item.Type == 2 && item.BigImg == 2)//链接，3图
                    {
                        content += " <a href=\"" + item.LinkUrl + "\" class=\"content\"><img class=\"img3\" src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 2 && item.BigImg == 0)//链接，2图
                    {
                        content += " <a href=\"" + item.LinkUrl + "\" class=\"content\"><img  src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 2 && item.BigImg == 1)//链接，1图
                    {
                        content += " <a href=\"" + item.LinkUrl + "\" class=\"img\"><img src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 3 && item.BigImg == 1)//无，1图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"img\"><img src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 3 && item.BigImg == 0)//无，2图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"content\"><img  src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 3 && item.BigImg == 2)//无，3图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"content\"><img class=\"img3\" src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                }

            }

            pureHtml = pureHtml.Replace("$content$", content);
            return pureHtml;
        }

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

        public ActionResult GetCurrentCouponActivity(string callBack)
        {
            CouponActivity result = ActivitySystem.GetCurrentCouponActivity();
            return this.Jsonp(callBack, result);
        }

        public ActionResult InsertPromotionFromActivity(string userId, string callBack)
        {
            CouponActivity couponDetail = ActivitySystem.GetCurrentCouponActivity();
            bool result = ActivitySystem.InsertPromotionFromActivity(couponDetail, userId);
            return this.Jsonp(callBack, result);

        }

        public ActionResult UserIsClaimedCoupon(string userId, string callBack)
        {
            CouponActivity couponDetail = ActivitySystem.GetCurrentCouponActivity();
            bool result = ActivitySystem.UserIsClaimedCoupon(couponDetail, userId);
            return this.Jsonp(callBack, result);
        }

        #region 随机免费洗车活动

        /// <summary>
        /// 报名活动页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ApplyActivityPage(Guid activityId)
        {
            if (activityId == Guid.Empty)
            {
                return Content("活动不存在");
            }
            //判断当前活动有没有过期
            using (var client = new ActivityClient())
            {
                var result = await client.GetActivityModelByActivityIdAsync(activityId);
                if (result != null)
                {
                    if (result.Success && result.Result.EndTime < DateTime.Now.Date)
                    {
                        return Content("该活动已过期");
                    }
                    //判断当前活动审核通过人数是否已达上限
                    if (!await CheckApplyUserCountAsync(activityId, result.Result.Quota))
                    {
                        return Content("活动报名人数已满");
                    }
                }
                else
                {
                    return Content("活动不存在");
                }
            }
            ViewBag.ActivityId = activityId;
            return View();
        }
        /// <summary>
        /// 用户活动报名
        /// </summary>
        /// <param name="userActivityModel"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> UserApplyActivity(UserApplyActivityModel userActivityModel, string userName)
        {
            if (userActivityModel.ActivityId == Guid.Empty || string.IsNullOrWhiteSpace(userActivityModel.Mobile) || string.IsNullOrWhiteSpace(userActivityModel.CarNum) || string.IsNullOrWhiteSpace(userActivityModel.DriverNum))
            {
                return AjaxHelper.MvcJsonResult(HttpStatusCode.BadRequest, "缺少必要参数");
            }
            userActivityModel.UserName = userName;
            using (var activityClient = new ActivityClient())
            {
                //检查活动是否开始
                var activityModel = await activityClient.GetActivityModelByActivityIdAsync(userActivityModel.ActivityId);
                if (activityModel.Success)
                {
                    if (activityModel.Result.StartTime > DateTime.Now.Date)
                    {
                        return AjaxHelper.MvcJsonResult(HttpStatusCode.Accepted, "活动暂未开始");
                    }
                }
                else
                {
                    return AjaxHelper.MvcJsonResult(HttpStatusCode.BadGateway, "服务器内部错误");
                }
                //检查用户手机号、车牌号、驾驶证号是否已经使用
                var isExistResult = await activityClient.CheckUserApplyActivityInfoIsExistAsync(userActivityModel.ActivityId, userActivityModel.Mobile, userActivityModel.CarNum, userActivityModel.DriverNum);
                if (isExistResult.Success)
                {
                    if (!isExistResult.Result)
                    {
                        var activity = await activityClient.GetActivityModelByActivityIdAsync(userActivityModel.ActivityId);
                        //获取报名用户审核通过数
                        var auditPassCount =
                            await activityClient.GetActivityApplyUserPassCountByActivityIdAsync(userActivityModel
                                .ActivityId);
                        if (activity.Success && auditPassCount.Success && auditPassCount.Result < activity.Result.Quota)
                        {
                            var cacheResult =
                                await activityClient.AddUserApplyActivitySortedSetCacheAsync(userActivityModel);
                            if (cacheResult.Success)
                            {
                                return AjaxHelper.MvcJsonResult(HttpStatusCode.OK,
                                    "报名成功，审核通过后服务码将会以短信形式发送到您的手机，请注意查收");
                            }
                        }
                        else
                        {
                            return AjaxHelper.MvcJsonResult(HttpStatusCode.Accepted, "报名人数已满！");
                        }
                    }
                    else
                    {
                        return AjaxHelper.MvcJsonResult(HttpStatusCode.Accepted, "手机号、车牌号、驾驶证号已经被使用");
                    }
                }

            }
            return AjaxHelper.MvcJsonResult(HttpStatusCode.BadGateway, "服务器内部错误");
        }

        public ActionResult ApplySuccessPage()
        {
            return View();
        }
        private async Task<bool> CheckApplyUserCountAsync(Guid activityId, int quota)
        {
            //判断当前活动审核通过人数是否已达上限
            using (var client = new ActivityClient())
            {
                var auditPassUserCount =
                    await client.GetActivityApplyUserPassCountByActivityIdAsync(activityId);
                if (auditPassUserCount.Success)
                {
                    if (auditPassUserCount.Result >= quota)
                    {
                        return false;
                    }
                }
                else
                {
                    Logger.Error($"审核通过人数查询失败。Message:{auditPassUserCount.Exception}");
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}