using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft.Json.Linq;
using Tuhu.Provisioning.Business;


namespace Tuhu.Provisioning.Controllers
{
    public class FlagshipStoreConfigController : Controller
    {
        //
        // GET: /FlagshipStoreConfig/

        public ActionResult Index()
        {
            FlagshipStoreConfigManager manager = new FlagshipStoreConfigManager();
            var list = manager.GetList();
            if (list== null)
                return View();
            return View(list);
        }

        [HttpPost]
        public ActionResult Edit(SE_FlagshipStoreConfig model=null)
        {
            FlagshipStoreConfigManager manager = new FlagshipStoreConfigManager();
            ViewBag.BrandList = manager.GetBrand();
            if (model.Name == null)
            {
                return View(new SE_FlagshipStoreConfig() {  ImageUrl=string.Empty});
            }
            else
            {
              
                return View(model);
            }
         
        }

        public ActionResult Save(SE_FlagshipStoreConfig model)
        {
            JObject json = new JObject();
            FlagshipStoreConfigManager manager = new FlagshipStoreConfigManager();
            if (model.PKID == 0)
            {
                if (manager.Add(model))
                    return Json(1);
                else
                    return Json(0);
            }
            else
            {
                if (manager.Update(model) == true)
                    return Json(1);
                else
                    return Json(0);
            }
           
           
        }

        [HttpPost]
        public ActionResult Delete(string PKID)
        {
            if (string.IsNullOrWhiteSpace(PKID))
                return Json(0);

            FlagshipStoreConfigManager manager = new FlagshipStoreConfigManager();

            if (manager.Delete(PKID))
                return Json(1);
            else
                return Json(0);
               
        }

    }
}
