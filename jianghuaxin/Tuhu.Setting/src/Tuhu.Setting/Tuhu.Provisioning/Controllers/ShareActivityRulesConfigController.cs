using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class ShareActivityRulesConfigController : Controller
    {
        private static readonly Lazy<ShareActivityRulesConfigManager> lazy = new Lazy<ShareActivityRulesConfigManager>();

        private ShareActivityRulesConfigManager ShareActivityRulesConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        public ActionResult Index(int pageIndex = 1, int pageSize = 15)
        {
            int count = 0;
            string strSql = string.Empty;
            var lists = ShareActivityRulesConfigManager.GetShareActivityRulesList(strSql, pageSize, pageIndex, out count);

            var list = new OutData<List<ShareActivityRulesConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<ShareActivityRulesConfig>(list.ReturnValue, pager));
        }


        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return View(new ShareActivityRulesConfig());
            }
            else
            {
                return View(ShareActivityRulesConfigManager.GetShareActivityRules(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ShareActivityRulesConfig model)
        {
            if (model.Id != 0)
            {
                return Json(ShareActivityRulesConfigManager.UpdateShareActivityRules(model));
            }
            else
            {
                return Json(ShareActivityRulesConfigManager.InsertShareActivityRules(model));
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            return Json(ShareActivityRulesConfigManager.DeleteShareActivityRules(id));
        }

    }
}
