using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class BaoYangPackageActivityConfigController : Controller
    {

        private static readonly Lazy<BaoYangPackageActivityConfigManager> lazyBaoYangPackageActivityConfigManager = new Lazy<BaoYangPackageActivityConfigManager>();

        private BaoYangPackageActivityConfigManager BaoYangPackageActivityConfigManager
        {
            get { return lazyBaoYangPackageActivityConfigManager.Value; }
        }

        public ActionResult Index(int pageIndex = 1, int pageSize = 15)
        {
            int count = 0;
            string strSql = string.Empty;
            var lists = BaoYangPackageActivityConfigManager.GetBaoYangPackageActivityConfigList(strSql, pageSize, pageIndex, out count);
            var list = new OutData<List<BaoYangPackageActivityConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = list.OutValue
            };
            return View(new ListModel<BaoYangPackageActivityConfig>(list.ReturnValue, pager));
        }

        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return View(new BaoYangPackageActivityConfig());
            }
            else
            {
                return View(BaoYangPackageActivityConfigManager.GetBaoYangPackageActivityConfig(id));
            }

        }

        [HttpPost]
        public ActionResult Edit(BaoYangPackageActivityConfig model)
        {
            string js = "<script>alert(\"保存失败 \");location='/BaoYangPackageActivityConfig/Index';</script>";
            if (model.Id != 0)
            {
                if (BaoYangPackageActivityConfigManager.UpdateBaoYangPackageActivityConfig(model))
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
                if (BaoYangPackageActivityConfigManager.InsertBaoYangPackageActivityConfig(model))
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
        public ActionResult Delete(int id)
        {
            return Json(BaoYangPackageActivityConfigManager.DeleteBaoYangPackageActivityConfig(id));
        }

        public ActionResult Product()
        {
            return View();
        }
    }
}
