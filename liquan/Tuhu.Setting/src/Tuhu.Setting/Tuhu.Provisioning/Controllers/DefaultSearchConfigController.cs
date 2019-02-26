using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Config;

namespace Tuhu.Provisioning.Controllers
{
    public class DefaultSearchConfigController : Controller
    {
        private static readonly Lazy<DefaultSearchConfigManager> lazy = new Lazy<DefaultSearchConfigManager>();

        private DefaultSearchConfigManager DefaultSearchConfigManager
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

        public ActionResult List(string type, int pageIndex = 1, int pageSize = 20)
        {
            int count = 0;
         
            List<DefaultSearchConfig> lists = DefaultSearchConfigManager.GetDefaultSearchConfigList(type, pageSize, pageIndex, out count);

            var list = new OutData<List<DefaultSearchConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<DefaultSearchConfig>(list.ReturnValue, pager));
        }


        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                DefaultSearchConfig model = new DefaultSearchConfig();
                model.Type = 2;
                return View(model);
            }
            else
            {
                return View(DefaultSearchConfigManager.GetDefaultSearchConfig(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(DefaultSearchConfig model)
        {
            string js = "<script>alert(\"保存失败 \");location='/DefaultSearchConfig/Index';</script>";
            if (model.Id != 0)
            {
                if (DefaultSearchConfigManager.UpdateDefaultSearchConfig(model))
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
                if (DefaultSearchConfigManager.InsertDefaultSearchConfig(model))
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
            return Json(DefaultSearchConfigManager.DeleteDefaultSearchConfig(id));
        }

        [PowerManage]
        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Refresh()
        {
            using (var product = new CacheClient())
            {
                var refresh = product.RemoveRedisCacheKey("HomePage", "DefaultSearch4/");
                return Json(refresh.Result);
            }
        }
    }
}
