using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class BusinessKeywordsConfigController : Controller
    {
        private static readonly Lazy<BusinessKeywordsConfigManager> lazy = new Lazy<BusinessKeywordsConfigManager>();

        private BusinessKeywordsConfigManager BusinessKeywordsConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        public ActionResult Index(int pageIndex = 1, int pageSize = 25)
        {
            int count = 0;
            string strSql = string.Empty;
            var lists = BusinessKeywordsConfigManager.GetBusinessKeywordsConfigList(strSql, pageSize, pageIndex, out count);

            var list = new OutData<List<BusinessKeywordsConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<BusinessKeywordsConfig>(list.ReturnValue, pager));
        }


        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return View(new BusinessKeywordsConfig());
            }
            else
            {
                return View(BusinessKeywordsConfigManager.GetBusinessKeywordsConfig(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(BusinessKeywordsConfig model)
        {
            string js = "<script>alert(\"保存失败 \");location='/BusinessKeywordsConfig/Index';</script>";
            if (model.Id != 0)
            {
                if (BusinessKeywordsConfigManager.UpdateBusinessKeywordsConfig(model))
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
                if (BusinessKeywordsConfigManager.InsertBusinessKeywordsConfig(model))
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
            return Json(BusinessKeywordsConfigManager.DeleteBusinessKeywordsConfig(id));
        }

    }
}
