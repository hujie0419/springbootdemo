using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.LuckyCharm;
using Tuhu.Provisioning.DataAccess.Entity.LuckyCharm;


namespace Tuhu.Provisioning.Controllers
{
    public class LuckyCharmController : Controller
    {
        private static readonly Lazy<LuckyCharmManager> lazyLuckyCharmManager = new Lazy<LuckyCharmManager>();

        private LuckyCharmManager LuckyCharmManager
        {
            get { return lazyLuckyCharmManager.Value; }
        }
        [HttpGet]
        public JsonResult PageActivity(int pkid = 0, int pageIndex = 1, int pageSize = 10)
        {
            var result = LuckyCharmManager.PageActivtiy(pageIndex, pageSize, pkid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetActivity(int pkid)
        {
            var result = LuckyCharmManager.GetActivtiy(pkid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddActivity(LuckyCharmActivityModel entity)
        {
            var result = LuckyCharmManager.AddActivtiy(entity);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult DelActivity(int pkid)
        {
            var result = LuckyCharmManager.DelActivtiy(pkid);
            return Json(result);
        }


        [HttpGet]
        public JsonResult PageUser(string phone, string areaName, int pkid = 0, int pageIndex = 1, int pageSize = 10, int checkState = -1)
        {
            var result = LuckyCharmManager.PageActivtiyUser(pageIndex, pageSize, phone, pkid, areaName, checkState);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult AddUser(LuckyCharmUserModel entity)
        {
            var result = LuckyCharmManager.AddActivtiyUser(entity);
            return Json(result);

        }

        [HttpPost]
        public JsonResult EditUser(LuckyCharmUserModel entity)
        {
            var result = LuckyCharmManager.UpdateActivtiyUser(entity);
            return Json(result);

        }

        [HttpPost]
        public JsonResult DelUser(int pkid)
        {
            var result = LuckyCharmManager.DelActivtiyUser(pkid);
            return Json(result);
        }


       
    }
}