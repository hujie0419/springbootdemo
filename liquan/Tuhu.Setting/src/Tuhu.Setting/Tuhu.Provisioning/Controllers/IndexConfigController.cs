using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ThBiz.DataAccess.Entity;
using Tuhu.Provisioning.Models;
using Tuhu.Provisioning.Business.HomePageConfig;
using BusPowerManage = ThBiz.Business.Power.PowerManage;
using EnOrDe = Tuhu.Component.Framework.EnOrDeHelper;
using ThBiz.Common.Configurations;
using ThBiz.Business.Power;
using System;
using Tuhu.Provisioning.Business.Setting;
using IndexModuleConfig = Tuhu.Provisioning.DataAccess.Entity.IndexModuleConfig;
using IndexModuleItem = Tuhu.Provisioning.DataAccess.Entity.IndexModuleItem;

namespace Tuhu.Provisioning.Controllers
{
    public class IndexConfigController : Controller
    {
        public ActionResult Index()
        {       
            return View();
        }

        public ActionResult Module(int moduleId)
        {
            ViewBag.moduleId = moduleId;
            return View();
        }

        [HttpPost]
        public ActionResult SetIndexModulesCache()
        {
            var status = -1;
            if (IndexConfigManager.SetIndexModulesCache())
            {
                status = 1;
            }
            return Json(status);
        }

        [HttpPost]
        public ActionResult UpdateModuleIndex(string moduleIds)
        {
            var status = -1;
            if (!string.IsNullOrEmpty(moduleIds))
            {
                List<string> miList = moduleIds.Split(',').ToList();
                miList.RemoveAt(miList.Count - 1);
                if(IndexConfigManager.UpdateIndexModuleIndex(miList))
                    status = 1;
            }
            return Json(status);
        }

        [HttpPost]
        public ActionResult UpdateModuleName(int moduleId, string moduleName)
        {
            var status = -1;
            if(IndexConfigManager.UpdateIndexModuleName(moduleId, moduleName))
            {
                status = 1;
            }
            return Json(status, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult DeleteModule(int moduleId)
        {
            var status = -1;
            if (IndexConfigManager.DeleteIndexModule(moduleId))
            {
                status = 1;
            }
            return Json(status);
        }

        [HttpPost]
        public ActionResult CreateModule(string moduleName)
        {
            var status = -1;
            if (IndexConfigManager.CreateIndexModule(moduleName))
            {
                status = 1;
            }
            return Json(status, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult UpdateModuleItemIndex(string itemIds)
        {
            var status = -1;
            if (!string.IsNullOrEmpty(itemIds))
            {
                List<string> iiList = itemIds.Split(',').ToList();
                iiList.RemoveAt(iiList.Count - 1);
                if(IndexConfigManager.UpdateIndexModuleItemIndex(iiList))
                {
                    status = 1;
                }
            }
            return Json(status);
        }

        [HttpPost]
        public ActionResult UpdateModuleItemEntry(int itemId, string entryName, string controller, string action)
        {
            var status = -1;
            if(IndexConfigManager.UpdateIndexModuleItem(itemId, entryName, controller, action))
            {
                status = 1;
            }
            return Json(status);
        }

        [HttpPost]
        public ActionResult DeleteModuleItem(int itemId)
        {
            var status = -1;
            if (IndexConfigManager.DeleteIndexModuleItem(itemId))
            {
                status = 1;
            }
            return Json(status);
        }

        [HttpPost]
        public ActionResult CreateModuleItem(int moduleId, string entryName, string controller, string action)
        {
            var status = -1;
            if(IndexConfigManager.CreateIndexModuleItem(moduleId, entryName, controller, action))
            {
                status = 1;
            }
            return Json(status);
        }
    }
}
