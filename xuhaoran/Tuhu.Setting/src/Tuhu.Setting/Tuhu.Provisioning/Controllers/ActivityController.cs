using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.Activity;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class ActivityController : Controller
    {
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        private static readonly Lazy<ActivityManager> activityManager = new Lazy<ActivityManager>();

        private ActivityManager ActivityManager
        {
            get { return activityManager.Value; }
        }
        [HttpGet]
        public JsonResult GetActivityUserInfoByAreaAsync(int AreaId, int pageIndex, int pageSize)
        {
            var result = ActivityManager.GetActivityUserInfoByAreaAsync(AreaId,pageIndex,pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllActivityAsync(int pageIndex, int pageSize)
        {
            var result = ActivityManager.GetAllActivityAsync(pageIndex,pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllActivityManagerAsync(int pageIndex, int pageSize)
        {
            var result = ActivityManager.GetAllActivityManagerAsync(pageIndex, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllAreaAsync()
        {
            var result = ActivityManager.GetAllAreaAsync();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddActivitiesUserAsync(ActivityUserInfo_xhrRequestModel model)
        {
            var result = ActivityManager.AddActivitiesUserAsync(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateActivitiesUserAsync(ActivityUserInfo_xhrRequestModel model)
        {
            string managerId = this.Request.Headers["Authorization"];//Header中的token
            var result = ActivityManager.UpdateActivitiesUserAsync(model, managerId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddActivity(T_Activity_xhrModel model)
        {
            string managerId = this.Request.Headers["Authorization"];//Header中的token
            var result = ActivityManager.AddActivity(model, managerId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateActivity(T_Activity_xhrModel model)
        {
            string managerId = this.Request.Headers["Authorization"];//Header中的token
            var result = ActivityManager.UpdateActivity(model, managerId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ManagerLogin(T_ActivityManagerUserInfo_xhrRequest model)
        {
            var result = ActivityManager.ManagerLogin(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ManagerRegister(T_ActivityManagerUserInfo_xhrRequest model)
        {
            var result = ActivityManager.ManagerRegister(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}