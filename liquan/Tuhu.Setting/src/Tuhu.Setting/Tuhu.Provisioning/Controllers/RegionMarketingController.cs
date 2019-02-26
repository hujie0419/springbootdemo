using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.RegionMarketing;
using Tuhu.Provisioning.DataAccess.Entity.RegionMarketing;

namespace Tuhu.Provisioning.Controllers
{
    public class RegionMarketingController : Controller
    {
        public ActionResult RegionMarketing()
        {
            return View();
        }

        public JsonResult GetRegionMarketingConfig(Guid? activityId, string activityName, DateTime? startTime, DateTime? endTime, int pageIndex = 1, int PageSize = 15)
        {
            RegionMarketingManager manager = new RegionMarketingManager();

            var result = manager.GetRegionMarketingConfig(activityId, activityName, startTime, endTime, pageIndex, PageSize);

            if (result != null && result.Any())
            {
                return Json(new { status = "success", data = result, count = result.FirstOrDefault().Total }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveRegionarketingConfig(string data)
        {
            RegionMarketingManager manager = new RegionMarketingManager();
            var result = false;

            if (!string.IsNullOrEmpty(data))
            {
                var info = JsonConvert.DeserializeObject<RegionMarketingModel>(data);
                info.ActivityRules = Server.UrlDecode(info.ActivityRules);
                result = manager.SaveRegionarketingConfig(info, HttpContext.User.Identity.Name);
            }

            return Json(new { status = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RegionMarketingConfig(string activityId, string optionType = "Add")
        {
            ViewBag.OptionType = optionType;
            var result = new RegionMarketingModel()
            {
                ImgList = new List<ActivityImageConfig>(),
                ProductList = new List<RegionMarketingProductConfig>()
            };

            if (optionType.Equals("Edit")&&!string.IsNullOrEmpty(activityId))
            {
                RegionMarketingManager manager = new RegionMarketingManager();
                result = manager.GetRegionActivityConfigByActivityId(Guid.Parse(activityId));
            }

            return View(result);
        }

        public JsonResult GetRegionActivityConfigByActivityId(Guid activityId)
        {
            RegionMarketingManager manager = new RegionMarketingManager();
            var result = manager.GetRegionActivityConfigByActivityId(activityId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteRegionMarketingConfig(Guid activityId)
        {
            RegionMarketingManager manager = new RegionMarketingManager();
            var result = false;
            result = manager.DeleteRegionMarketingConfig(activityId, HttpContext.User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectOperationLog(Guid activityId)
        {
            RegionMarketingManager manager = new RegionMarketingManager();
            var result = manager.SelectOperationLog(activityId, "RegionMarketing");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}