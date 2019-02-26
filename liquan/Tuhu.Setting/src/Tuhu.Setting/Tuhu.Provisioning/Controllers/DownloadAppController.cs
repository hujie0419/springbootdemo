using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.DownloadApp;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Activity;

namespace Tuhu.Provisioning.Controllers
{
    public class DownloadAppController : Controller
    {
        private readonly Lazy<DownloadAppManager> lazy = new Lazy<DownloadAppManager>();

        private DownloadAppManager DownloadAppManager
        {
            get { return this.lazy.Value; }
        }

        public ActionResult Index(DownloadApp model, int pageIndex = 1, int pageSize = 25)
        {
            int count = 0;

            List<DownloadApp> listModel = DownloadAppManager.GetDownloadAppList(model, pageSize, pageIndex, out count);

            var list = new OutData<List<DownloadApp>, int>(listModel, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };

            return this.View(new ListModel<DownloadApp>(list.ReturnValue, pager));

        }

        public ActionResult AddDownloadApp(DownloadApp model)
        {
            return Json(DownloadAppManager.InsertDownloadApp(model));
        }

        public ActionResult UpdateDownloadApp(DownloadApp model, int id)
        {
            var status = false;

            try
            {
                if (DownloadAppManager.UpdateDownloadApp(model, id))
                {                    
                    using (var client = new ActivityClient())
                    {
                        var result = client.CleanActivityConfigForDownloadAppCache(id);
                        status = result.Success;
                    }
                }
                return Json(status);
            }
            catch (Exception ex)
            {
                return Json(false);
                throw ex;
            }
           
        }

        public ActionResult DeleteDownloadApp(int id)
        {
            return Json(DownloadAppManager.DeleteDownloadApp(id));
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            DownloadApp model = DownloadAppManager.GetDownloadAppById(id);
            ViewBag.DownloadApp = model;
            return View();
        }

        public ActionResult AddNew()
        {
            return View();
        }

        public ActionResult EditNew(int id)
        {
            DownloadApp model = DownloadAppManager.GetDownloadAppById(id);
            ViewBag.DownloadApp = model;
            return View();
        }

    }
}
