using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class PointsRulesConfigController : Controller
    {
        private static readonly Lazy<PointsRulesConfigManager> lazy = new Lazy<PointsRulesConfigManager>();

        private PointsRulesConfigManager PointsRulesConfigManager
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
            var lists = PointsRulesConfigManager.GetPointsRulesConfigList(strSql, pageSize, pageIndex, out count);

            var list = new OutData<List<PointsRulesConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<PointsRulesConfig>(list.ReturnValue, pager));
        }


        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return View(new PointsRulesConfig());
            }
            else
            {
                return View(PointsRulesConfigManager.GetPointsRulesConfig(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(PointsRulesConfig model)
        {
            string js = "<script>alert(\"保存失败 \");location='/PointsRulesConfig/Index';</script>";
            if (model.Id != 0)
            {
                if (PointsRulesConfigManager.UpdatePointsRulesConfig(model))
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
                if (PointsRulesConfigManager.InsertPointsRulesConfig(model))
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
        public JsonResult Delete(int id)
        {
            return Json(PointsRulesConfigManager.DeletePointsRulesConfig(id));
        }

    }
}
