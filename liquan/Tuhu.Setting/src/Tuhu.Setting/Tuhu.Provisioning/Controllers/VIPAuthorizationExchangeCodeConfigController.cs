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
    public class VIPAuthorizationExchangeCodeConfigController : Controller
    {
        private static readonly Lazy<VIPAuthorizationExchangeCodeConfigManager> lazy = new Lazy<VIPAuthorizationExchangeCodeConfigManager>();

        private VIPAuthorizationExchangeCodeConfigManager VIPAuthorizationExchangeCodeConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        private static readonly Lazy<VIPAuthorizationRuleConfigManager> lazy1 = new Lazy<VIPAuthorizationRuleConfigManager>();

        private VIPAuthorizationRuleConfigManager VIPAuthorizationRuleConfigManager
        {
            get
            {
                return lazy1.Value;
            }
        }


        public ActionResult Index()
        {
            ViewBag.CodebatchList = VIPAuthorizationExchangeCodeConfigManager.GetCodeBatch();

            return View();
        }

        public ActionResult List(string CodeBatch, int pageIndex = 1, int pageSize = 20)
        {
            int count = 0;
         
            var lists = VIPAuthorizationExchangeCodeConfigManager.GetVIPAuthorizationExchangeCodeConfigList(CodeBatch, pageSize, pageIndex, out count);

            var list = new OutData<List<VIPAuthorizationExchangeCodeConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<VIPAuthorizationExchangeCodeConfig>(list.ReturnValue, pager));
        }

        public ActionResult Edit(int id = 0)
        {
            ViewBag.RuleList = VIPAuthorizationRuleConfigManager.GetVIPAuthorizationRuleAndId();


            if (id == 0)
            {

                VIPAuthorizationExchangeCodeConfig model = new VIPAuthorizationExchangeCodeConfig();
                model.SumNum = 1;
                model.EndTime= DateTime.Now;
                return View(model);
            }
            else
            {
                return View(VIPAuthorizationExchangeCodeConfigManager.GetVIPAuthorizationExchangeCodeConfig(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(VIPAuthorizationExchangeCodeConfig model)
        {
            string js = "<script>alert(\"保存失败 \");location='/VIPAuthorizationExchangeCodeConfig/Index';</script>";

            if (VIPAuthorizationExchangeCodeConfigManager.InsertVIPAuthorizationExchangeCodeConfig(model))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return Content(js);
            }

        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            return Json(VIPAuthorizationExchangeCodeConfigManager.DeleteVIPAuthorizationExchangeCodeConfig(id));
        }
    }
}
