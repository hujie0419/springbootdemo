using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.ThirdPartyReplaceOrder;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class ThirdSubmitOrderController : Controller
    {
        public ActionResult ThirdSubmitOrder()
        {
            ViewBag.StartTime = DateTime.Now.AddDays(-1).ToShortDateString();
            ViewBag.EndTime = DateTime.Now.ToShortDateString();
            ViewBag.OrderType = "12加油卡";
            return View();
        }

        public JsonResult SelectNeedReplaceOrder(DateTime startTime, DateTime endTime, string orderType, int pageIndex, int pageSize)
        {
            ThirdPartyReplaceOrderManager manager = new ThirdPartyReplaceOrderManager();
            var totalCount = 0;
            IEnumerable<OrderLists> data = new List<OrderLists>();
            data = manager.SelectNeedSendOrder(startTime, endTime, orderType);
            totalCount = data.Count();
            data = data.Skip(pageSize * (pageIndex - 1)).Take(pageSize).Select(i => i).ToList();
            return Json(new { totalCount = totalCount, data = data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SubmitThirdReplaceOrder(int tuhuOrderId, string orderType)
        {
            ThirdPartyReplaceOrderManager manager = new ThirdPartyReplaceOrderManager();
            var result = manager.SubmitThirdReplaceOrder(tuhuOrderId, orderType, HttpContext.User.Identity.Name);
            return Json(new { status = result.Item1, msg = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SubmitThirdReplaceOrderByOrderId(int tuhuOrderId)
        {
            ThirdPartyReplaceOrderManager manager = new ThirdPartyReplaceOrderManager();
            var result = manager.SubmitThirdReplaceOrderByOrderId(tuhuOrderId, HttpContext.User.Identity.Name);
            return Json(new { status = result.Item1, msg = result.Item2 }, JsonRequestBehavior.AllowGet);
        }
    }
}