using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class PointsCenterConfigController : Controller
    {
        private static readonly Lazy<PointsCenterConfigManager> lazy = new Lazy<PointsCenterConfigManager>();

        private PointsCenterConfigManager PointsCenterConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        public ActionResult Index(int pageIndex = 1, int pageSize = 10)
        {
            int count = 0;
            string strSql = string.Empty;
            var lists = PointsCenterConfigManager.GetPointsCenterConfigList(strSql, pageSize, pageIndex, out count);

            var list = new OutData<List<PointsCenterConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<PointsCenterConfig>(list.ReturnValue, pager));
        }


        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return View(new PointsCenterConfig());
            }
            else
            {
                return View(PointsCenterConfigManager.GetPointsCenterConfig(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(PointsCenterConfig model)
        {
            string js = "<script>alert(\"保存失败 \");location='/PointsCenterConfig/Index';</script>";
            if (model.Id != 0)
            {
                if (PointsCenterConfigManager.UpdatePointsCenterConfig(model))
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
                if (PointsCenterConfigManager.InsertPointsCenterConfig(model))
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
            return Json(PointsCenterConfigManager.DeletePointsCenterConfig(id));
        }

    }
}
