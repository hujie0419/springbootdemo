using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;

namespace Tuhu.Provisioning.Controllers
{
    public class VehicleController : Controller
    {
        private readonly VehicleTypeManager _vehicleTypeManager;

        public VehicleController()
        {
            this._vehicleTypeManager = new VehicleTypeManager();
        }

        public JsonResult GetAllVehicleBrands()
        {
            var result = _vehicleTypeManager.GetAllVehicleBrands();
            return Json(new { Success = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllBrandCategories()
        {
            var result = _vehicleTypeManager.GetAllBrandCategories();
            return Json(new { Success = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVehicleSeriesByBrand(string brand)
        {
            var result = _vehicleTypeManager.GetVehicleSeries(brand);
            return Json(new { Success = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取排量
        /// </summary>
        /// <param name="vid"></param>
        /// <returns></returns>
        public JsonResult GetVehiclePaiLiang(string vid)
        {

            var data = _vehicleTypeManager.GetVehiclePaiLiang(vid);
            return Json(new { Success = data != null, Data = data }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取生产年份
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="paiLiang"></param>
        /// <returns></returns>
        public JsonResult GetVehicleNian(string vid, string paiLiang)
        {

            var data = _vehicleTypeManager.GetVehicleNian(vid, paiLiang);

            return Json(new { Success = data != null, Data = data }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取年款信息
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="paiLiang"></param>
        /// <param name="nian"></param>
        /// <returns></returns>
        public JsonResult GetVehicleSalesName(string vid, string paiLiang, string nian)
        {
            int year = 0;
            if (!Int32.TryParse(nian, out year))
            {
                return Json(new { Success = false, Msg = "未知的年产" }, JsonRequestBehavior.AllowGet);
            }
            var data = _vehicleTypeManager.GetVehicleSalesName(vid, paiLiang, year);
            return Json(new { Success = data != null, Data = data }, JsonRequestBehavior.AllowGet);
        }
    }
}