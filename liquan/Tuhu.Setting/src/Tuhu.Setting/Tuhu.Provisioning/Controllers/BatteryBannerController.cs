using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.DownloadApp;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class BatteryBannerController : Controller
    {
        private readonly Lazy<DownloadAppManager> lazy = new Lazy<DownloadAppManager>();

        private DownloadAppManager DownloadAppManager
        {
            get { return this.lazy.Value; }
        }

        public ActionResult Index(int pageIndex = 1, int pageSize = 10)
        {
            string sqlStr = "";
            int recordCount = 0;
            List<BatteryBanner> listModel = DownloadAppManager.GetBatteryBanner(sqlStr, pageSize, pageIndex, out recordCount);
            var list = new OutData<List<BatteryBanner>, int>(listModel, recordCount);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = recordCount
            };

            return View(new ListModel<BatteryBanner>(list.ReturnValue, pager));
        }

        public ActionResult Add()
        {
            ViewBag.ProvinceList = ViewBag.CityList = DownloadAppManager.GetRegion(0);
            return View(new BatteryBanner());
        }

        public JsonResult AddBatteryBanner(BatteryBanner model)
        {
            return Json(DownloadAppManager.InsertBatteryBanner(model));
        }

        [HttpGet]
        public ActionResult DisplayCity(int id = 0)
        {
            return Json(DownloadAppManager.GetRegion(id), JsonRequestBehavior.AllowGet);
        }
    }
}
