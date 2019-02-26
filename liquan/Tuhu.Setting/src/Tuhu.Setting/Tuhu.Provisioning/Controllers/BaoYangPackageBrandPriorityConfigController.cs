using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class BaoYangPackageBrandPriorityConfigController : Controller
    {
        private static readonly Lazy<BaoYangPackageBrandPriorityConfigManager> lazy = new Lazy<BaoYangPackageBrandPriorityConfigManager>();

        private BaoYangPackageBrandPriorityConfigManager BaoYangPackageBrandPriorityConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(string brand, string vehicle, string data, int pageSize = 1000, int pageIndex = 1, int startPrice = 0, int endPrice = 0)
        {
            int count = 0;
            var lists = BaoYangPackageBrandPriorityConfigManager.GetBaoYangPackageBrandPriorityConfigList(brand, vehicle, data, pageSize, pageIndex, out count, startPrice, endPrice);

            var list = new OutData<List<BaoYangPackageBrandPriorityConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<BaoYangPackageBrandPriorityConfig>(list.ReturnValue, pager));
        }

        public ActionResult Edit(int id = 0, string edit = "", string VehicleID = "")
        {
            ViewBag.Edit = edit;
            ViewBag.VehicleID = VehicleID;
            if (id == 0)
            {
                BaoYangPackageBrandPriorityConfig model = new BaoYangPackageBrandPriorityConfig();
                return View(model);
            }
            else
            {
                return View(BaoYangPackageBrandPriorityConfigManager.GetBaoYangPackageBrandPriorityConfig(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(string dataList)
        {
            List<BaoYangPackageBrandPriorityConfig> list = JsonConvert.DeserializeObject<List<BaoYangPackageBrandPriorityConfig>>(dataList);

            return Json(BaoYangPackageBrandPriorityConfigManager.InsertOrUpdate(list));
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            return Json(BaoYangPackageBrandPriorityConfigManager.DeleteBaoYangPackageBrandPriorityConfig(id));
        }

        /// <summary>
        /// 获取所有车型的品牌
        /// </summary>
        /// <returns></returns>
        public JsonResult GetVehicleBrands()
        {

            var data = BaoYangPackageBrandPriorityConfigManager.GetAllVehicleBrands();

            if (data != null && data.Count() > 0)
            {
                return Json(new { status = "success", data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据选择的品牌获取该品牌的车型系列
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public JsonResult GetVehicleSeries(string brand)
        {

            IDictionary<string, string> data = null;
            if (!string.IsNullOrEmpty(brand))
            {
                data = BaoYangPackageBrandPriorityConfigManager.GetVehicleSeries(Server.UrlDecode(brand));
            }
            if (data != null && data.Any())
            {
                return Json(new { status = "success", data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail", data = data }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
