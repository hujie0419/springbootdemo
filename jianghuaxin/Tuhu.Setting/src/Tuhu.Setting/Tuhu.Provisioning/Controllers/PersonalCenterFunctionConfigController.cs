using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class PersonalCenterFunctionConfigController : Controller
    {
        private static readonly Lazy<PersonalCenterFunctionConfigManager> lazy = new Lazy<PersonalCenterFunctionConfigManager>();

        private PersonalCenterFunctionConfigManager PersonalCenterFunctionConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        public ActionResult Index(PersonalCenterFunctionConfig model, int pageIndex = 1, int pageSize = 10)
        {
            int count = 0;
          
            var lists = PersonalCenterFunctionConfigManager.GetPersonalCenterFunctionConfigList(model, pageSize, pageIndex, out count);

            var list = new OutData<List<PersonalCenterFunctionConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<PersonalCenterFunctionConfig>(list.ReturnValue, pager));
        }


        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                PersonalCenterFunctionConfig model = new PersonalCenterFunctionConfig();             
                return View(model);
            }
            else
            {
                return View(PersonalCenterFunctionConfigManager.GetPersonalCenterFunctionConfig(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(PersonalCenterFunctionConfig model)
        {
            string js = "<script>alert(\"保存失败 \");location='/PersonalCenterFunctionConfig/Index';</script>";
            if (model.Id != 0)
            {
                if (PersonalCenterFunctionConfigManager.UpdatePersonalCenterFunctionConfig(model))
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
                if (PersonalCenterFunctionConfigManager.InsertPersonalCenterFunctionConfig(model))
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
            return Json(PersonalCenterFunctionConfigManager.DeletePersonalCenterFunctionConfig(id));
        }
    }
}
