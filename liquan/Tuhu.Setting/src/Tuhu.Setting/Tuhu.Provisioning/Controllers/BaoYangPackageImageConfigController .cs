using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class BaoYangPackageImageConfigController : Controller
    {

        private static readonly Lazy<BaoYangPackageImageConfigManager> lazyBaoYangPackageImageConfigManager = new Lazy<BaoYangPackageImageConfigManager>();

        private BaoYangPackageImageConfigManager BaoYangPackageImageConfigManager
        {
            get { return lazyBaoYangPackageImageConfigManager.Value; }
        }

        public ActionResult Index()
        {
            ViewBag.Prudcts = BaoYangPackageImageConfigManager.GetBaoYangPackagePruduct();
            return View();
        }

        public ActionResult List(string pid,int pageIndex = 1, int pageSize = 10)
        {
            int count = 0;        
            var lists = BaoYangPackageImageConfigManager.GetBaoYangPackageImageConfigList(pid, pageSize, pageIndex, out count);
            var list = new OutData<List<BaoYangPackageImageConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = list.OutValue
            };
            return View(new ListModel<BaoYangPackageImageConfig>(list.ReturnValue, pager));
        }

        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return View(new BaoYangPackageImageConfig());
            }
            else
            {
                return View(BaoYangPackageImageConfigManager.GetBaoYangPackageImageConfig(id));
            }

        }

        [HttpPost]
        public ActionResult Edit(BaoYangPackageImageConfig model)
        {
            string js = "<script>alert(\"保存失败 \");location='/BaoYangPackageImageConfig/Index';</script>";
            if (BaoYangPackageImageConfigManager.UpdateBaoYangPackageImageConfigNew(model))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return Content(js);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            return Json(BaoYangPackageImageConfigManager.DeleteBaoYangPackageImageConfig(id));
        }

       
    }
}
