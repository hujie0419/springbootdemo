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
    public class ExchangeCenterConfigController : Controller
    {
        private static readonly Lazy<ExchangeCenterConfigManager> lazy = new Lazy<ExchangeCenterConfigManager>();

        private ExchangeCenterConfigManager ExchangeCenterConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        public ActionResult Index()
        {
            ViewBag.exchanges = ExchangeCenterConfigManager.GetExchangeCenter();
            return View();
        }

        public ActionResult List(string SearchWord, int pageIndex = 1, int pageSize = 20)
        {
            int count = 0;
            string strSql = string.Empty;
            if (SearchWord == "兑换") strSql = "and (ExchangeCenterType=0)";
            else if (SearchWord == "抽奖") strSql = "and (ExchangeCenterType=1)";
            else if (!string.IsNullOrEmpty(SearchWord))
                strSql = "and (CouponName like N'%" + SearchWord + "%'or UserRank like N'%" + SearchWord + "%')";
            var lists = ExchangeCenterConfigManager.GetExchangeCenterConfigList(strSql, pageSize, pageIndex, out count);

            var list = new OutData<List<ExchangeCenterConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<ExchangeCenterConfig>(list.ReturnValue, pager));
        }

        public JsonResult RefreshCache()
        {
            using (var client = new MemberMallClient())
            {
               var response =  client.RefreshMemberMallConfigs();
                response.ThrowIfException(true);
            }
            return Json(new {Code = 1});
        }

        public ActionResult Edit(int id = 0)
        {
            ExchangeCenterConfig model = new ExchangeCenterConfig();
            if (id == 0)
            {
                model.EndTime = DateTime.Now.AddDays(7);
                model.Edit = "add";
                model.Postion = "";
                model.UserRank = "";
                return View(model);
            }
            else
            {
                model = ExchangeCenterConfigManager.GetExchangeCenterConfig(id);
                return View(model);
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ExchangeCenterConfig model)
        {
            Version sVersion = new Version();
            if (Version.TryParse(model.StartVersion, out sVersion))
            {
                model.StartVersion = sVersion.ToString();
            }
            else
            {
                return Json(false);
            }
            Version eVersion = new Version();
            if (Version.TryParse(model.EndVersion, out eVersion))
            {
                model.EndVersion = eVersion.ToString();
            }
            else
            {
                return Json(false);
            }
            if (model.Id != 0 && model.Edit.ToLower() == "update")
            {
                return Json(ExchangeCenterConfigManager.UpdateExchangeCenterConfig(model));
            }
            else
            {
                return Json(ExchangeCenterConfigManager.InsertExchangeCenterConfig(model));
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            return Json(ExchangeCenterConfigManager.DeleteExchangeCenterConfig(id));
        }

    }
}
