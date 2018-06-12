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
    }
}