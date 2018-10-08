using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class BaoYangRecommendServiceConfigController : Controller
    {
        private static readonly Lazy<BaoYangRecommendServiceConfigManager> lazy = new Lazy<BaoYangRecommendServiceConfigManager>();

        private BaoYangRecommendServiceConfigManager BaoYangRecommendServiceConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        public ActionResult Index()
        {
            List<BaoYangPackage> packages = GetBaoYangPackage();
            packages.Add(new BaoYangPackage { Type = "tire", Name = "保养轮胎" });
            ViewBag.BaoYangPack = packages;
            return View();
        }

        public ActionResult List(string serviceType, int pageIndex = 1, int pageSize = 20)
        {
            var configs = BaoYangRecommendServiceConfigManager.GetBaoYangRecommendServiceConfigList(serviceType, pageSize, pageIndex);
            var lists = configs.Item1;
            var count = configs.Item2;
            var list = new OutData<List<BaoYangRecommendServiceConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<BaoYangRecommendServiceConfig>(list.ReturnValue, pager));
        }

        public ActionResult Edit(int id = 0)
        {
            List<BaoYangPackage> packages = GetBaoYangPackage();
            packages.Add(new BaoYangPackage { Type = "tire", Name = "保养轮胎" });
            ViewBag.BaoYangPack = packages;
            if (id == 0)
            {
                BaoYangRecommendServiceConfig model = new BaoYangRecommendServiceConfig();
                return View(model);
            }
            else
            {
                return View(BaoYangRecommendServiceConfigManager.GetBaoYangRecommendServiceConfig(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(BaoYangRecommendServiceConfig model)
        {
            if (model.Id != 0)
            {
                //if (BaoYangRecommendServiceConfigManager.CheckService(model))
                //{
                //    return Json(-100);
                //}

                return Json(BaoYangRecommendServiceConfigManager.UpdateBaoYangRecommendServiceConfig(model));
            }
            else
            {
                if (BaoYangRecommendServiceConfigManager.CheckService(model))
                {
                    return Json(-100);
                }

                return Json(BaoYangRecommendServiceConfigManager.InsertBaoYangRecommendServiceConfig(model));
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            return Json(BaoYangRecommendServiceConfigManager.DeleteBaoYangRecommendServiceConfig(id));
        }

        public List<BaoYangPackage> GetBaoYangPackage()
        {
            var manager = new BaoYangActivitySettingManager();
            var list = manager.GetBaoYangPackageDescription();
            return list ?? new List<BaoYangPackage>();
        }
    }
}
