using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class RulesDictionaryConfigController : Controller
    {
        private static readonly Lazy<RulesDictionaryConfigManager> lazy = new Lazy<RulesDictionaryConfigManager>();

        private RulesDictionaryConfigManager RulesDictionaryConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        public ActionResult Index(int type)
        {
            return View(RulesDictionaryConfigManager.GetRulesDictionaryConfig(type));
        }

        public ActionResult Edit(int id = 0, int type = 0)
        {
            if (id == 0)
            {
                RulesDictionaryConfig model = new RulesDictionaryConfig();
                model.Type = type;
                return View(model);
            }
            else
            {
                return View(RulesDictionaryConfigManager.GetRulesDictionaryConfigById(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(RulesDictionaryConfig model)
        {
            if (model.Id != 0)
            {
                return Json(RulesDictionaryConfigManager.UpdateRulesDictionaryConfig(model));
            }
            else
            {
                return Json(RulesDictionaryConfigManager.InsertRulesDictionaryConfig(model));
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            return Json(RulesDictionaryConfigManager.DeleteRulesDictionaryConfig(id));
        }

    }
}
