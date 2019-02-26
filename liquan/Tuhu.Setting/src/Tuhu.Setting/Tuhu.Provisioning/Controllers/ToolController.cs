using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tuhu.Provisioning.Controllers
{
    public class ToolController : Controller
    {
        // GET: Tool
        public readonly string env = ConfigurationManager.AppSettings["env"];
        // 拼团订单完成
        public JsonResult FinishPTOrder(int orderId, int type = 0)
        {
            if (env == "dev" && (type == 1 || type == 0))
            {
                TuhuNotification.SendNotification("notification.GroupBuyingOrderStatusQueue", new Dictionary<string, object>
                {
                    ["OrderId"] = orderId,
                    ["Operate"] = type == 0 ? "Paid" : "Canceled"
                });
                return Json("消息已推送", JsonRequestBehavior.AllowGet);
            }
            return Json("该工具只能在线下使用", JsonRequestBehavior.AllowGet);
        }
    }
}