using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class VIPAuthorizationRuleConfigController : Controller
    {
        private static readonly Lazy<VIPAuthorizationRuleConfigManager> lazy = new Lazy<VIPAuthorizationRuleConfigManager>();

        private VIPAuthorizationRuleConfigManager VIPAuthorizationRuleConfigManager
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

        public ActionResult List(int pageIndex = 1, int pageSize = 20)
        {
            int count = 0;
            string strSql = string.Empty;

            var lists = VIPAuthorizationRuleConfigManager.GetVIPAuthorizationRuleConfigList(strSql, pageSize, pageIndex, out count);

            var list = new OutData<List<VIPAuthorizationRuleConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<VIPAuthorizationRuleConfig>(list.ReturnValue, pager));
        }

        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                VIPAuthorizationRuleConfig model = new VIPAuthorizationRuleConfig();
                model.ValidityDay = 1;
                return View(model);
            }
            else
            {
                return View(VIPAuthorizationRuleConfigManager.GetVIPAuthorizationRuleConfig(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(VIPAuthorizationRuleConfig model)
        {
            if (model.Id != 0)
            {

                return Json(VIPAuthorizationRuleConfigManager.UpdateVIPAuthorizationRuleConfig(model));
            }
            else
            {
                return Json(VIPAuthorizationRuleConfigManager.InsertVIPAuthorizationRuleConfig(model));
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            return Json(VIPAuthorizationRuleConfigManager.DeleteVIPAuthorizationRuleConfig(id));
        }

    }
}
