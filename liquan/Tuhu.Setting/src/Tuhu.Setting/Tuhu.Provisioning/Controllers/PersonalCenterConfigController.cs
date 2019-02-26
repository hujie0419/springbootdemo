using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class PersonalCenterConfigController : Controller
    {
        private static readonly Lazy<PersonalCenterConfigManager> lazy = new Lazy<PersonalCenterConfigManager>();

        private PersonalCenterConfigManager PersonalCenterConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        private static readonly Lazy<VIPAuthorizationRuleConfigManager> lazyRule = new Lazy<VIPAuthorizationRuleConfigManager>();

        private VIPAuthorizationRuleConfigManager VIPAuthorizationRuleConfigManager
        {
            get
            {
                return lazyRule.Value;
            }
        }


        public ActionResult Index(int pageIndex = 1, int pageSize = 15)
        {
            int count = 0;
            string strSql = string.Empty;
            var lists = PersonalCenterConfigManager.GetPersonalCenterConfigList(strSql, pageSize, pageIndex, out count);

            var list = new OutData<List<PersonalCenterConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            var locationList = new BannerConfigController().LocationDropDownList();
            foreach (var model in list.ReturnValue)
            {
                var locationNameModel = locationList.Find(t => t.Id == model.Location.ToString());
                model.LocationName = locationNameModel != null ? locationNameModel.Name : "";
            }
            return View(new ListModel<PersonalCenterConfig>(list.ReturnValue, pager));
        }


        public ActionResult Edit(int id = 0)
        {
            ViewBag.RuleList = VIPAuthorizationRuleConfigManager.GetVIPAuthorizationRuleAndId();
            if (id == 0)
            {  
                return View(new PersonalCenterConfig());
            }
            else
            {
                return View(PersonalCenterConfigManager.GetPersonalCenterConfig(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(PersonalCenterConfig model)
        {
            string js = "<script>alert(\"保存失败 \");location='/PersonalCenterConfig/Index';</script>";
            if (model.Id != 0)
            {
                if (PersonalCenterConfigManager.UpdatePersonalCenterConfig(model))
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
                if (PersonalCenterConfigManager.InsertPersonalCenterConfig(model))
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
            return Json(PersonalCenterConfigManager.DeletePersonalCenterConfig(id));
        }

        public ActionResult PersonalCenter()
        {
            return View();
        }

    }
}
