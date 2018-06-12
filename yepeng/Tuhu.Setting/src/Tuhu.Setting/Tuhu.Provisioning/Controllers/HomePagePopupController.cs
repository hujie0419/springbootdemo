using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using Tuhu.Component.Framework.Extension;
using Tuhu.Provisioning.Business.DownloadApp;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Config;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;
using Tuhu.Provisioning.Business.HomePageConfig;
using Tuhu.Provisioning.Business.ShareConfig;
using Newtonsoft.Json;

namespace Tuhu.Provisioning.Controllers
{
    public class HomePagePopupController : Controller
    {
        private readonly Lazy<DownloadAppManager> lazy = new Lazy<DownloadAppManager>();

        private DownloadAppManager DownloadAppManager
        {
            get { return lazy.Value; }
        }

        private Dictionary<string, string> GetTargetGroupDic()
        {
            var targetGroups = DownloadAppManager.GetTargetGroup();
            var result = new Dictionary<string, string>();
            if (targetGroups != null && targetGroups.Any())
            {
                Dictionary<string, string> targetGroupDic = new Dictionary<string, string>();
                foreach (var tg in targetGroups)
                {
                    if (!targetGroupDic.ContainsKey(tg.TargetKey))
                        targetGroupDic.Add(tg.TargetKey, tg.TargetGroups);
                }
                result = targetGroupDic;// JsonConvert.SerializeObject(targetGroupDic);
            }
            return result;
        }

        [PowerManage]
        public async Task<ViewResult> Index()
        {
            ViewBag.NewNoticeChannel = ViewBag.NoticeChannel = DownloadAppManager.QueryNoticeChannel() ?? new List<NoticeChannel>();
            var result = DownloadAppManager.QueryHomePagePopup(new HomePagePopupQuery { PositionCriterion = 1 }, 1);
           
            ViewBag.NumOfPopups = DownloadAppManager.CountHomePagePopup(new HomePagePopupQuery { PositionCriterion = 1 });
            ViewBag.Page = 1;
            ViewBag.OrderCriterion = 0;
            ViewBag.AllTargetGroups = GetTargetGroupDic();
            var scManager = new ShareConfigManager();
            ViewBag.WxConfigs = JsonConvert.SerializeObject((await scManager.SelectWxConfigsAsync()).Select(_=>new {appId=_.appId,name=_.name}));
            return View(result ?? new List<HomePagePopup>());
        }
        
        [HttpPost]
        public async Task<ViewResult> Index(HomePagePopupQuery popupQuery, int orderCriterion, int page = 1)
        {
            ViewBag.NewNoticeChannel = ViewBag.NoticeChannel = DownloadAppManager.QueryNoticeChannel() ?? new List<NoticeChannel>();
            popupQuery.OrderCriterion = orderCriterion;
            if (!string.IsNullOrWhiteSpace(popupQuery.StartVersionCriterion))
            {
                popupQuery.StartVersionCriterion = popupQuery.StartVersionCriterion.Trim();
            }
            if (!string.IsNullOrWhiteSpace(popupQuery.EndVersionCriterion))
            {
                popupQuery.EndVersionCriterion = popupQuery.EndVersionCriterion.Trim();
            }
            var result = DownloadAppManager.QueryHomePagePopup(popupQuery, page);

            ViewBag.NumOfPopups = DownloadAppManager.CountHomePagePopup(popupQuery);
            ViewBag.Page = page;
            ViewBag.OrderCriterion = orderCriterion;
            ViewBag.AllTargetGroups = GetTargetGroupDic();
            var scManager = new ShareConfigManager();
            ViewBag.WxConfigs = JsonConvert.SerializeObject((await scManager.SelectWxConfigsAsync()).Select(_ => new {appId=_.appId,name=_.name}));
            return View(result ?? new List<HomePagePopup>());
        }
        
        [HttpPost]
        public JsonResult AddOrUpdate(HomePagePopup popup)
        {
            if(!string.IsNullOrWhiteSpace(popup.StartVersion))
            {
                popup.StartVersion = popup.StartVersion.Trim();
            }
            if (!string.IsNullOrWhiteSpace(popup.EndVersion))
            {
                popup.EndVersion = popup.EndVersion.Trim();
            }

            if (popup.PKID < 0)
            {
                var result = DownloadAppManager.InsertHomePagePopup(popup);
                if (result > 0)
                {
                    var oprLog = new OprLog
                    {
                        ObjectID = result,
                        ObjectType = "HPPopup",
                        AfterValue =
                            "优先级： " + popup.Sort + "， 周期：" + popup.Period + "， 目标群体：" +
                            (string.IsNullOrWhiteSpace(popup.TargetGroups) ? "" : popup.TargetGroups),
                        Author = HttpContext.User.Identity.Name,
                        Operation = "新建弹窗"
                    };
                    new OprLogManager().AddOprLog(oprLog);
                }
                if (popup.IsEnabled && popup.StartDateTime<= DateTime.Now &&
                    popup.EndDateTime >= DateTime.Now)
                    return Json(result + ",1");
                return Json(result + ",0");
            }
            else
            {
                var result = DownloadAppManager.UpdateHomePagePopup(popup);
                if (result > 0)
                {
                    if (popup.IsEnabled && popup.StartDateTime <= DateTime.Now &&
                        popup.EndDateTime >= DateTime.Now)
                        result = 2;
                    else
                        result = 1;
                    var oprLog = new OprLog
                    {
                        ObjectID = popup.PKID,
                        ObjectType = "HPPopup",
                        AfterValue =
                            "优先级： " + popup.Sort + "， 周期：" + popup.Period + "， 目标群体：" +
                            (string.IsNullOrWhiteSpace(popup.TargetGroups) ? "" : popup.TargetGroups),
                        Author = HttpContext.User.Identity.Name,
                        Operation = "更新弹窗"
                    };
                    new OprLogManager().AddOprLog(oprLog);
                }
                return Json(result);
            }
        }

        public ActionResult SelectOprLogByPKID(int PKID)
        {
            var result = LoggerManager.SelectOprLogByParams("HPPopup", PKID.ToString());
            return result != null && result.Any()
                ? Json(new { status = "success", data = result }, JsonRequestBehavior.AllowGet)
                : Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UploadImage()
        {
            var result = "";
            if (Request.Files.Count > 0)
            {
                var Imgfile = Request.Files[0];
                try
                {
                    var buffer = new byte[Imgfile.ContentLength];
                    Imgfile.InputStream.Read(buffer, 0, buffer.Length);
                    using (var client = new FileUploadClient())
                    {
                        var res = client.UploadImage(new ImageUploadRequest()
                        {
                            Contents = buffer,
                            DirectoryName = "HomePagePopUpImg",
                            MaxHeight = 1920,
                            MaxWidth = 1920
                        });
                        if (res.Success && res.Result != null)
                        {
                           result= Tuhu.ImageHelper.GetImageUrl(res.Result);
                        }
                    }
                }
                catch (Exception exp)
                {
                    WebLog.LogException(exp);
                }
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult CacheUpdate()
        {
            try
            {
                using (var client = new HomePageClient())
                {
                    var res = client.RefreshHomePagePopCache();
                    if (res.Success && res.Result)
                    {
                        return Json(1);
                    }
                    else
                    {
                        return Json(-1);
                    }
                }
            }
            catch (Exception exp)
            {
                WebLog.LogException(exp);
                return Json(-1);
            }
        }

        public JsonResult GetAnimation(int Pkid)
        {
            if (Pkid <= 0)
            {
                return Json(new List<HomePagePopupAnimation>(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(DownloadAppManager.GetAnimation(Pkid), JsonRequestBehavior.AllowGet);
            }
            //return View(result ?? new List<HomePagePopupAnimation>());
        }

        [HttpPost]
        public int AnimationSave(HomePagePopupAnimation model)
        {
            if (model.MovementType == -1)
            {
                return 0;
            }        
            else {
                HomePageConfigManager config = new HomePageConfigManager();
                if (string.IsNullOrWhiteSpace(model.Creator))
                    model.Creator = HttpContext.User.Identity.Name;
                var addedId = config.AnimationSave(model);
                if (addedId > 0)
                {
                    var oprLog = new OprLog
                    {
                        ObjectID = model.PopupConfigId,
                        ObjectType = "HPPopup",
                        AfterValue =
                           "新建的弹窗动画配置的pkid：" + addedId + "， 图片链接：" + model.ImageUrl + "， 动作类型：" +
                           model.MovementType +"， 跳转链接：" + model.LinkUrl,
                        Author = HttpContext.User.Identity.Name,
                        Operation = "新建弹窗动画"
                    };
                    new OprLogManager().AddOprLog(oprLog);
                }
                return addedId;
            }
        }

        [HttpPost]
        public int AnimationDelete(int PKID, int configId)
        {
            HomePageConfigManager obj = new HomePageConfigManager();
            var affectedRows = obj.AnimationDelete(PKID);
            if (affectedRows > 0)
            {
                var oprLog = new OprLog
                {
                    ObjectID = configId,
                    ObjectType = "HPPopup",
                    Author = HttpContext.User.Identity.Name,
                    BeforeValue = "删除的弹窗动画配置的pkid：" + PKID,
                    Operation = "删除弹窗动画"
                };
                new OprLogManager().AddOprLog(oprLog);
            }
            return affectedRows;
        }

        [HttpPost]
        public bool AnimationUpdate(HomePagePopupAnimation model)
        {

            HomePageConfigManager config = new HomePageConfigManager();
            var hasUpdateSucceeded = config.AnimationUpdate(model);
            if (hasUpdateSucceeded)
            {
                var oprLog = new OprLog
                {
                    ObjectID = model.PopupConfigId,
                    ObjectType = "HPPopup",
                    AfterValue =
                           "更新的弹窗动画配置的pkid：" + model.PKId + "， 图片链接：" + model.ImageUrl + "， 动作类型：" +
                           model.MovementType + "， 跳转链接：" + model.LinkUrl,
                    Author = HttpContext.User.Identity.Name,
                    Operation = "更新弹窗动画"
                };
                new OprLogManager().AddOprLog(oprLog);
            }
            return hasUpdateSucceeded;
        }

        public JsonResult SelectCouponsInPopup(int animaId)
        {
            if (animaId <= 0)
            {
                return Json(new List<CouponsInPopup>(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                HomePageConfigManager config = new HomePageConfigManager();
                return Json(config.SelectCouponsOnAnimaId(animaId), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddOrUpdateCoupon(CouponsInPopup model)
        {
            HomePageConfigManager config = new HomePageConfigManager();
            if (model.PKId < 0)
            {
                var result = config.InsertCouponInPopup(model);
                if (result > 0)
                {
                    var oprLog = new OprLog
                    {
                        ObjectID = result,
                        ObjectType = "CouponsInPopup",
                        AfterValue =
                            "对应的弹窗动画Id： " + model.PopupAnimationId + "， 优惠券ID或GUID：" + model.CouponId,
                        Author = HttpContext.User.Identity.Name,
                        Operation = "新建弹窗中可领优惠券"
                    };
                    new OprLogManager().AddOprLog(oprLog);
                }
                return Json(result);
            }
            else
            {
                var result = config.UpdateCouponInPopup(model);
                if (result > 0)
                {
                    var oprLog = new OprLog
                    {
                        ObjectID = model.PKId,
                        ObjectType = "CouponsInPopup",
                        AfterValue =
                            "对应的弹窗动画Id： " + model.PopupAnimationId + "， 优惠券ID或GUID：" + model.CouponId,
                        Author = HttpContext.User.Identity.Name,
                        Operation = "更新弹窗中可领优惠券"
                    };
                    new OprLogManager().AddOprLog(oprLog);
                }
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult DeleteCouponInPopup(int PKId, int animationId)
        {
            HomePageConfigManager config = new HomePageConfigManager();
            var affectedRows = config.DeleteCouponInPopup(PKId);
            if (affectedRows > 0)
            {
                var oprLog = new OprLog
                {
                    ObjectID = PKId,
                    ObjectType = "CouponsInPopup",
                    Author = HttpContext.User.Identity.Name,
                    BeforeValue = "删除的优惠券对应的弹窗动画Id：" + animationId,
                    Operation = "更新弹窗中可领优惠券"
                };
                new OprLogManager().AddOprLog(oprLog);
            }
            return Json(affectedRows);
        }

        public ActionResult SelectOprLogsOfCouponsInPopup(int PKId)
        {
            var result = LoggerManager.SelectOprLogByParams("CouponsInPopup", PKId.ToString());
            return (result != null && result.Any())
                ? Json(new { status = "success", data = result }, JsonRequestBehavior.AllowGet)
                : Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
        }
    }
}