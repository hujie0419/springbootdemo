using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Tuhu.Provisioning.Business.BizUserIntergal;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.Service.Member.Request;

namespace Tuhu.Provisioning.Controllers
{
    public class UserIntergalController : Controller
    {
        //
        // GET: /UserIntergal/
		[PowerManage]
        public ActionResult Index()
        {
            
            return View();
        }
       
        public ActionResult UserDetail(string phone)
        {

            var list = new BizUserIntergal().PhoneSearch(phone);

            return Json(list);
        }

        [PowerManage]
        public JsonResult Compensate(string userId, int intergal, string cause, bool isActive,string id,int transactionIntegral)
        {
            if (ControllerContext.HttpContext.User == null)
            {
                return Json(new { result = "false" });
            }
            InsertIntergalModal insertIntergal = new InsertIntergalModal()
            {
                UserId = new Guid(userId),
                IntegralDetailID = Guid.NewGuid(),
                Integral = intergal,
                IntegralID = new Guid(id),
                IntegralRuleID = new Guid("6fa89adc-b26a-44db-9f59-2a033bd17e0b"),
                TransactionIntegral = transactionIntegral,
                TransactionRemark = "活动积分补偿", //规则的描述
                TransactionDescribe = cause,
                IsActive = isActive,
                TransactionChannel = "pc",
                Versions = "1.0.0.0",
                Author = ControllerContext.HttpContext.User.Identity.Name
            };
            int returnCount = new BizUserIntergal().InsertIntergal(insertIntergal);
            string result = returnCount == 0 ? "false" : "true";
            return Json(new {msg = result});
        }
        
        public JsonResult Details(string userId)
        {
            var list = new BizUserIntergal().GetUserIntegralDetail(userId);
            return Json(list);
        }
        [PowerManage]
        public JsonResult UpdateStatus(string id, string status, string cause,string ruleId)
        {
            var result = "";
            if (ControllerContext.HttpContext.User == null)
            {
                return Json(new {result = "请重新登录！"});
            }
            using (var client = new UserIntegralClient())
            {
                    UpdateUserIntegralStatusRequest integralStatus = new UpdateUserIntegralStatusRequest
                {
                    IntegralDetailID = new Guid(id),
                    IntegralRuleID = new Guid(ruleId),
                    IsActivity = bool.Parse(status),
                    OperationAuthor = ControllerContext.HttpContext.User.Identity.Name??"zhansan",
                    Reason = cause
                };
                var tempresult = client.UpdateUserIntegralGradeStatus(integralStatus);
                result = string.IsNullOrWhiteSpace(tempresult.ErrorMessage) ? "状态修改成功！" : tempresult.ErrorMessage;
            }
            return Json(new {result = result});
        }
        
        public JsonResult SelectUpdateLog(string id)
        {
            var detailId = new Guid(id);
            var list = new BizUserIntergal().SelectUpdateLog(detailId);
            return Json(list);
        }
		
        public ActionResult CheckUserPhone(string phone)
        {
            var user = new BizUserIntergal().CheckUser(phone);
            return Json(new {msg= user});
        }
    }
}
