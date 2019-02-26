using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.NewCoupon;
using Tuhu.Provisioning.DataAccess.Entity.NewCoupon;

namespace Tuhu.Provisioning.Controllers
{
    public class NewCouponController : Controller
    {

        [PowerManage]
        public ActionResult NewCouponConfig()
        {
            return View();
        }

        public ActionResult CouponConfigInfo(Guid? activityId)
        {
            NewCouponManager manager = new NewCouponManager();
            NewCouponActivity result = new NewCouponActivity();
            ViewBag.ActivityId = activityId ?? Guid.Empty;
            if (activityId != null && activityId != Guid.Empty)
            {
                result = manager.GetNewCouponConfigByActivityId(activityId.Value);
            }

            return View(result);
        }

        public JsonResult GetCouponConfigInfo(string activityName,string activityId, int pageIndex, int pageSize = 20)
        {
            NewCouponManager manager = new NewCouponManager();
            var activityID = Guid.Empty;
            if (!string.IsNullOrEmpty(activityId))
            {
                Guid.TryParse(activityId, out activityID);
            }
            var result = manager.GetNewCouponConfig(activityName, activityID, pageIndex, pageSize);

            if (result != null && result.Any())
            {
                return Json(new { status = "success", data = result, count = result.FirstOrDefault().Total }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail", count = 0 }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult UpsertNewCouponConfig(string data)
        {
            NewCouponManager manager = new NewCouponManager();
            var result = false;
            var msg = "操作失败";

            if (!string.IsNullOrEmpty(data))
            {
                var model = JsonConvert.DeserializeObject<NewCouponActivity>(data);

                if (model != null)
                {
                    model.Description = Server.UrlDecode(model.Description);
                    result = manager.UpsertNewConponConfig(model, HttpContext.User.Identity.Name);
                }
            }

            return Json(new { data = result, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteNewCouponConfig(Guid activityId)
        {
            NewCouponManager manager = new NewCouponManager();
            var result = false;

            if (activityId != Guid.Empty)
            {
                result = manager.DeleteNewCouponConfigByActivityId(activityId, HttpContext.User.Identity.Name);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCouponRulesInfo(Guid? rulesId)
        {
            NewCouponManager manager = new NewCouponManager();
            CouponRulesConfig result = new CouponRulesConfig();
            if (rulesId != null && rulesId.Value != Guid.Empty)
            {
                result = manager.GetCouponRulesInfo(rulesId.Value);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshRandomCouponCache(Guid activityId)
        {
            NewCouponManager manager = new NewCouponManager();
            var result = manager.RefreshRandomCouponCache(activityId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectOperationLog(string objectId)
        {
            NewCouponManager manager = new NewCouponManager();
            var result = manager.SelectOperationLog(objectId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsExistRandomId(string randomGroupId)
        {
            NewCouponManager manager = new NewCouponManager();
            var result = manager.IsExistRandomId(randomGroupId.Trim());
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}