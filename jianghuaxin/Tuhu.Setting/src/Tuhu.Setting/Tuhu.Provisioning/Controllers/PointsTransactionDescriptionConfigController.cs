using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class PointsTransactionDescriptionConfigController : Controller
    {
        private static readonly Lazy<PointsTransactionDescriptionConfigManager> lazy = new Lazy<PointsTransactionDescriptionConfigManager>();

        private PointsTransactionDescriptionConfigManager PointsTransactionDescriptionConfigManager
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
            var lists = PointsTransactionDescriptionConfigManager.GetPointsTransactionDescriptionConfigList(strSql, pageSize, pageIndex, out count);

            var list = new OutData<List<PointsTransactionDescriptionConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<PointsTransactionDescriptionConfig>(list.ReturnValue, pager));
        }


        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                PointsTransactionDescriptionConfig model = new PointsTransactionDescriptionConfig();
                model.IntegralRuleID = string.Empty;
                return View(model);
            }
            else
            {
                return View(PointsTransactionDescriptionConfigManager.GetPointsTransactionDescriptionConfig(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(PointsTransactionDescriptionConfig model)
        {
            string js = "<script>alert(\"保存失败 \");location='/PointsTransactionDescriptionConfig/Index';</script>";
            if (!string.IsNullOrWhiteSpace(model.IntegralRuleID))
            {
                if (PointsTransactionDescriptionConfigManager.UpdatePointsTransactionDescriptionConfig(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content(js);
                }
            }
            else
            {
                if (PointsTransactionDescriptionConfigManager.InsertPointsTransactionDescriptionConfig(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content(js);
                }
            }
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            return Json(PointsTransactionDescriptionConfigManager.DeletePointsTransactionDescriptionConfig(id));
        }
    }
}
