using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class RewardApplicationController : Controller
    {
#if !DEBUG
        [PowerManage]
#endif
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetDate(int pageIndex, int pageSize, int applicationState, string applicationName, string phone, DateTime? createDateTime)
        {
            TempData["applicationName"] = applicationName;
            TempData["createDateTime"] = createDateTime;
            var dbResult = RewardApplicationManager.SelectRewardApplicationModels(pageIndex, pageSize, applicationState, applicationName, phone, createDateTime).ToList();
            return Json(new Tuple<int, List<RewardApplicationModel>>(dbResult.Select(r => r.TotalCount).FirstOrDefault(), dbResult.ToList()), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save(string phone, int state)
        {
            var dbResult = RewardApplicationManager.SaveRewardApplicationModels(phone, state, User.Identity.Name);

            return Json(new
            {
                status = dbResult ? 1 : 0
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FindNextRewardApplication(string phone,string[] phones)
        {
            var applicationName = TempData["applicationName"]?.ToString();
            var createDateTime = TempData["createDateTime"]as DateTime?;
            var dbResult = RewardApplicationManager.FetchRewardApplicationModel(phone, phones, applicationName, createDateTime);
            if (dbResult != null)
                return Json(new
                {
                    status = 1,
                    dbResult,

                }, JsonRequestBehavior.AllowGet);
            else
            {
                return Json(new
                {
                    status = -1,
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult FetchNextOrPre(string phone)
        {
            var dbResult = RewardApplicationManager.FetchNextOrPreRewardApplicationModel(phone);
            if (dbResult != null)
                return Json(new
                {
                    status = 1,
                    dbResult,

                }, JsonRequestBehavior.AllowGet);
            else
            {
                return Json(new
                {
                    status = -1,
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}