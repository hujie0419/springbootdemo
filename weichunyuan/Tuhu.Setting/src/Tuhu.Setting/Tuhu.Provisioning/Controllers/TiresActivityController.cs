using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.Business.ActivityBoard;
using Tuhu.Provisioning.Business.RegionMarketing;
using Tuhu.Provisioning.Business.TiresActivity;
using Tuhu.Provisioning.DataAccess.Entity.ActivityBoard;
using Tuhu.Provisioning.DataAccess.Entity.TiresActivity;

namespace Tuhu.Provisioning.Controllers
{
    public class TiresActivityController : Controller
    {
        private static readonly TiresActivityManager manager = new TiresActivityManager();
        private static readonly ActivityBoardPowerManager powerManager = new ActivityBoardPowerManager();
        private static readonly RegionMarketingManager tiresManager = new RegionMarketingManager();

        public ActionResult TiresActivity(string activityName, DateTime? startTime, DateTime? endTime, Guid? activityId, int pageIndex = 1)
        {
            PagerModel pager = new PagerModel()
            {
                CurrentPage = pageIndex,
                PageSize = 20
            };

            var result = manager.SelectTiresActivity(activityName, startTime, endTime, activityId, pager.CurrentPage, pager.PageSize);
            if (result != null && result.Any()) { pager.TotalItem = result.FirstOrDefault().Total; }

            ViewBag.CurrentPage = pager.CurrentPage;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.ActivityId = activityId;
            ViewBag.ActivityName = activityName;
            ViewBag.TotalPage = pager.TotalPage;
            ViewBag.MiniRegion = manager.GetAllMiniRegion();
            return View(result ?? new List<TiresActivityModel>());
        }

        public ActionResult TiresActivityInfo(Guid? activityId)
        {
            var result = new TiresActivityModel();
            ViewBag.ActivityId = activityId ?? Guid.Empty;
            if (activityId != null && activityId.Value != Guid.Empty)
            {
                result = manager.SelectTiresActivity("", null, null, activityId, 1, 20).FirstOrDefault();
            }
            return View(result);
        }

        public ActionResult TiresFloorInfo(Guid? floorId, Guid parentId)
        {
            TiresFloorActivityConfig result = new TiresFloorActivityConfig();
            ViewData["ImgDic"] = manager.GetAllImgMapping();
            ViewBag.ParentId = parentId;
            ViewBag.FloorActivityId = floorId ?? Guid.Empty;
            if (floorId != null && floorId.Value != Guid.Empty)
            {
                result = manager.SelectTiresActivityByFloorId(floorId.Value);
            }
            return View(result);
        }


        public JsonResult GetTiresFloorInfoByFloorActivity(Guid flashId, Guid parentId)
        {
            TiresFloorActivityConfig result = new TiresFloorActivityConfig();
            result = manager.SelectTiresActivityByFlashId(flashId, parentId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectTiresActivityByActivityId(string activityName, DateTime? startTime, DateTime? endTime, Guid? activityId, int pageIndex = 1)
        {
            var result = manager.SelectTiresActivity(activityName, startTime, endTime, activityId, pageIndex, 20);
            if (result != null)
            {
                return Json(new { status = "success", data = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpserTiresActivity(string model)
        {
            var result = false;
            if (!string.IsNullOrEmpty(model))
            {
                var data = JsonConvert.DeserializeObject<TiresActivityModel>(model);
                data.ActivityRules = Server.UrlDecode(data.ActivityRules);
                if (data.StartTime != null && data.EndTime != null && data.EndTime > data.StartTime)
                    result = manager.UpserTiresActivity(data, HttpContext.User.Identity.Name);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpsertTiresFloorActivity(string model)
        {
            var result = false;
            if (!string.IsNullOrEmpty(model))
            {
                var data = JsonConvert.DeserializeObject<TiresFloorActivityConfig>(model);
                result = manager.UpsertTiresFloorActivity(data, HttpContext.User.Identity.Name);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteTiresActivityConfig(Guid activityId)
        {
            var result = false;

            result = manager.DeleteTiresActivityConfig(activityId, HttpContext.User.Identity.Name);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteTiresFloorActivity(Guid activityId, Guid floorId)
        {
            var result = false;

            result = manager.DeleteTiresFloorActivity(activityId, floorId, HttpContext.User.Identity.Name);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectOperationLog(Guid activityId)
        {
            var result = tiresManager.SelectOperationLog(activityId, "TiresActivity");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefershAllRegionTiresActivityCache(Guid activityId)
        {
            var result = manager.RefershAllRegionTiresActivityCache(activityId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefershTiresActivityCacheByRegionId(Guid activityId, int regionId)
        {
            var result = manager.RefreshRegionTiresActivityCache(activityId, regionId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectProductRegionStock(int regionId, string pidStr)
        {
            var result = manager.GetProductRegionStock(regionId, pidStr);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllMiniRegion()
        {
            var result = manager.GetAllMiniRegion();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChildMiniRegion(int provinceId)
        {
            var result = manager.GetAllMiniRegion();
            var data = result.Where(x => x.RegionId == provinceId).Select(y => y.ChildRegions).FirstOrDefault();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}