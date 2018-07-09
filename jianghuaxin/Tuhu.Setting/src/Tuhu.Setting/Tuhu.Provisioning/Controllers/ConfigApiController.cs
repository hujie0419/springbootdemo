using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.DownloadApp;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class ConfigApiController : Controller
    {
        private readonly Lazy<DownloadAppManager> lazy = new Lazy<DownloadAppManager>();

        private DownloadAppManager DownloadAppManager
        {
            get { return lazy.Value; }
        }

        public ActionResult Index(ConfigApi model)
        {
            List<ConfigApi> list = DownloadAppManager.GetConfigApi(model);
            return View(list);
        }

        public ActionResult Add(ConfigApi model)
        {
            if (model.Id == 0)
            {
                return View(new ConfigApi());
            }
            else
            {
                return View(DownloadAppManager.GetConfigApi(model.Id));
            }
           
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Update(ConfigApi model)
        {
            return Json(DownloadAppManager.UpdateConfigApi(model));
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Insert(ConfigApi model)
        {
            return Json(DownloadAppManager.InsertConfigApi(model));
        }

        public JsonResult Delete(int id)
        {
            return Json(DownloadAppManager.DeleteConfigApi(id));
        }
    }
}