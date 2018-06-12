using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class ShareActivityProductConfigController : Controller
    {
        private static readonly Lazy<ShareActivityProductConfigManager> lazy = new Lazy<ShareActivityProductConfigManager>();

        private ShareActivityProductConfigManager ShareActivityProductConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        public ActionResult Index()
        {
            ViewBag.products = ShareActivityProductConfigManager.GetShareActivityProduct();
            return View();
        }

        public ActionResult List(ShareActivityProductConfig model, int pageIndex = 1, int pageSize = 30)
        {
            int count = 0;

            var lists = ShareActivityProductConfigManager.GetShareActivityProductConfigList(model, pageSize, pageIndex, out count);

            var list = new OutData<List<ShareActivityProductConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<ShareActivityProductConfig>(list.ReturnValue, pager));
        }


        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return View(new ShareActivityProductConfig());
            }
            else
            {
                return View(ShareActivityProductConfigManager.GetShareActivityProductConfig(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ShareActivityProductConfig model)
        {
            string js = "<script>alert(\"保存失败 \");location='/ShareActivityProductConfig/Index';</script>";
            if (model.Id != 0)
            {
                if (ShareActivityProductConfigManager.UpdateShareActivityProductConfig(model))
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
                if (ShareActivityProductConfigManager.InsertShareActivityProductConfig(model))
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
            return Json(ShareActivityProductConfigManager.DeleteShareActivityProductConfig(id));
        }
    }
}
