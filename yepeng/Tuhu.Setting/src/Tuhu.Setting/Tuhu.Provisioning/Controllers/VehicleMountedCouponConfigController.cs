using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class VehicleMountedCouponConfigController : Controller
    {
        private static readonly Lazy<VehicleMountedCouponConfigManager> lazy = new Lazy<VehicleMountedCouponConfigManager>();

        private VehicleMountedCouponConfigManager VehicleMountedCouponConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        private static readonly Lazy<MeiRongAcitivityConfigManager> lazy1 = new Lazy<MeiRongAcitivityConfigManager>();

        private MeiRongAcitivityConfigManager MeiRongAcitivityConfigManager
        {
            get
            {
                return lazy1.Value;
            }
        }
        public ActionResult Index()
        {
            ViewBag.ProvinceList = MeiRongAcitivityConfigManager.GetRegion(0);
            return View();
        }

        public ActionResult List(VehicleMountedCouponConfig model, int pageIndex = 1, int pageSize = 15)
        {
            int count = 0;
            string strSql = string.Empty;

            var lists = VehicleMountedCouponConfigManager.GetVehicleMountedCouponConfigList(model, pageSize, pageIndex, out count);

            var list = new OutData<List<VehicleMountedCouponConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<VehicleMountedCouponConfig>(list.ReturnValue, pager));
        }



        public ActionResult Edit(int id = 0)
        {
            ViewBag.ProvinceList = MeiRongAcitivityConfigManager.GetRegion(0);

            if (id == 0)
            {
                VehicleMountedCouponConfig model = new VehicleMountedCouponConfig();
                model.CreateTime = DateTime.Now;
                model.UpdateTime = DateTime.Now;
                model.Region = "[{'ProvinceName':'','CityName':''}]";
                model.Status = true;
                return View(model);
            }
            else
            {
                VehicleMountedCouponConfig model = VehicleMountedCouponConfigManager.GetVehicleMountedCouponConfigById(id);
                model.RegionList = JsonConvert.DeserializeObject<List<RegionModel>>(model.Region);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(VehicleMountedCouponConfig model)
        {
            if (model.Id != 0)
            {
                model.UpdateName = User.Identity.Name;
                return Json(VehicleMountedCouponConfigManager.UpdateVehicleMountedCouponConfig(model));
            }
            else
            {
                model.CreateName = User.Identity.Name;
                model.UpdateName = User.Identity.Name;
                int outId = 0;
                return Json(VehicleMountedCouponConfigManager.InsertVehicleMountedCouponConfig(model, ref outId));
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            return Json(VehicleMountedCouponConfigManager.DeleteVehicleMountedCouponConfig(id));
        }

        public ActionResult JiaYouCard(string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 25)
        {
            ViewBag.StartTime = string.IsNullOrWhiteSpace(startTime) ? DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") : Convert.ToDateTime(startTime).ToString("yyyy-MM-dd");
            ViewBag.EndTime = string.IsNullOrWhiteSpace(endTime) ? DateTime.Now.ToString("yyyy-MM-dd") : Convert.ToDateTime(endTime).ToString("yyyy-MM-dd"); ;

            int count = 0;
            string strSql = string.Empty;

            var lists = VehicleMountedCouponConfigManager.JiayouCard(ViewBag.StartTime, ViewBag.EndTime, pageSize, pageIndex, out count);

            var list = new OutData<List<tbl_OrderModel>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<tbl_OrderModel>(list.ReturnValue, pager));
        }
    }
}
