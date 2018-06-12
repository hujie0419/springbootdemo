using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft.Json.Linq;
using Tuhu.Provisioning.Business.CarProductFlagshipStoreConfig;
using Tuhu.Service.Product.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class CarProductFlagshipStoreConfigController : Controller
    {
        // GET: CarProductFlagshipStoreConfig
        public ActionResult Index()
        {
            CarProductFlagshipStoreConfigManager manager = new CarProductFlagshipStoreConfigManager();
            var list = manager.GetList();
            if (list == null)
                return View();
            return View(list);
        }

        public ActionResult Edit(string brand)
        {

            CarProductFlagshipStoreConfigManager manager = new CarProductFlagshipStoreConfigManager();
            ViewBag.BrandList = manager.GetBrand();

            Tuhu.Service.Product.Models.ProductConfig.CarProductFlagshipStoreConfig model = manager.GetConfigByBrand(brand);
            if (string.IsNullOrWhiteSpace(brand) || model == null || string.IsNullOrWhiteSpace(model.Name))
            {
                return View(new Tuhu.Service.Product.Models.ProductConfig.CarProductFlagshipStoreConfig()
                {
                    LogoUrl = string.Empty,
                    BackGroundUrl = string.Empty
                });
            }
            else
            {
                return View(model);
            }
        }

        [ValidateInput(false)]
        public JsonResult Save(Tuhu.Service.Product.Models.ProductConfig.CarProductFlagshipStoreConfig modelConfig, string actionString)
        {
            bool flag = false;
            CarProductFlagshipStoreConfigManager manager = new CarProductFlagshipStoreConfigManager();

            if (string.IsNullOrWhiteSpace(actionString)) return Json(flag);
            if (actionString == "add")
            {
                modelConfig.CreateTime = DateTime.Now;
                modelConfig.LastUpdateDataTime = DateTime.Now;
                flag = manager.InsertConfig(modelConfig);
            }
            else if (actionString == "edit")
            {
                modelConfig.CreateTime = DateTime.Now;
                modelConfig.LastUpdateDataTime = DateTime.Now;
                flag = manager.UpdateConfig(modelConfig);
            }

            return Json(flag);
        }

        public ActionResult Delete(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand))
                return Json(0);

            CarProductFlagshipStoreConfigManager manager = new CarProductFlagshipStoreConfigManager();
            if (manager.DeleteConfig(brand))
                return Json(1);
            else
                return Json(0);
        }
    }
}