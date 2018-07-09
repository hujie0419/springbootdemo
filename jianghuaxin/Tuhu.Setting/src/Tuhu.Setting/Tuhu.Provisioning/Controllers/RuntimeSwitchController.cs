using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.RunSwitch;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Config;
using Tuhu.Service.Product;

namespace Tuhu.Provisioning.Controllers
{
    public class RuntimeSwitchController : Controller
    {
        private readonly Lazy<RuntimeSwitchManager> lazy = new Lazy<RuntimeSwitchManager>();

        private RuntimeSwitchManager RuntimeSwitchManager
        {
            get { return lazy.Value; }

        }
        // GET: RuntimeSwitch
        public ActionResult Index()
        {
            Thread.Sleep(1000);
            List<RuntimeSwitch> list = RuntimeSwitchManager.GetRuntimeSwitch();
            return View(list);
        }

        public ActionResult Add(RuntimeSwitch model)
        {
            if (model.PKID == 0)
            {
                return View(new RuntimeSwitch());
            }
            else
            {
                return View(RuntimeSwitchManager.GetRuntimeSwitch(model.PKID));
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Update(RuntimeSwitch model)
        {
            return Json(RuntimeSwitchManager.UpdateRuntimeSwitch(model));
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Insert(RuntimeSwitch model)
        {
            return Json(RuntimeSwitchManager.InsertRuntimeSwitch(model));
        }

        public JsonResult Delete(int id)
        {
            return Json(RuntimeSwitchManager.DeleteRuntimeSwitch(id));
        }


        [HttpPost]
        public JsonResult Refresh()
        {
            using (var product = new Tuhu.Service.Config.CacheClient())
            {
                var refresh = product.RemoveRedisCacheKeyAsync("Config1", "RuntimeSwitchCache");
                return Json(refresh.Result);
            }
        }

    }
}