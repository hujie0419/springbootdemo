using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.Shipping;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service;
using Tuhu.Service.OprLog;
using Tuhu.Service.OprLog.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class ShippingController : Controller
    {
        //
        // GET: /Shipping/
        [PowerManage]
        public ActionResult Index()
        {
            var model = ShippingManager.SelectShippingRule();
            return View(model);
        }
        /// <summary>轮胎配置页面</summary>
        public ActionResult Tire()
        {
            var model = ShippingManager.SelectShippingRule().Where(m => (m.Types & 2) == 2);//轮胎
            if (Request.QueryString.ToString().Contains("home"))
            {
                ViewBag.Where = "home";
                model = model.Where(m => (m.Types & 4) == 0);//到家
            }
            else
                model = model.Where(m => (m.Types & 4) == 4);//到店
            return View(model);
        }

        /// <summary>保养配置页面</summary>
        public ActionResult BaoYang(GradeDeliveryFeeRule rule)
        {
            rule.ProductType = 1;
            ViewBag.GradeDeliveryFeeRule = ShippingManager.GradeDeliveryFeeRule(rule);

            var model = ShippingManager.SelectShippingRule().Where(m => (m.Types & 2) == 0 && (m.Types & 64) == 0);
            if (Request.QueryString.ToString().Contains("home"))
            {
                ViewBag.Where = "home";
                model = model.Where(m => (m.Types & 4) == 0);
            }
            else
                model = model.Where(m => (m.Types & 4) == 4);
            return View(model);
        }

        public ActionResult Hub(GradeDeliveryFeeRule rule)
        {
            rule.ProductType = 1;
            ViewBag.GradeDeliveryFeeRule = ShippingManager.GradeDeliveryFeeRule(rule);

            var model = ShippingManager.SelectShippingRule().Where(m => (m.Types & 64) == 64);
            if (Request.QueryString.ToString().Contains("home"))
            {
                ViewBag.Where = "home";
                model = model.Where(m => (m.Types & 4) == 0);
            }
            else
                model = model.Where(m => (m.Types & 4) == 4);
            return View(model);
        }

        /// <summary>地区弹框</summary>
        /// <param name="id">点击的规则的id触发弹框</param>
        public ActionResult Regions(string id)
        {
            ViewBag.ID = id;
            return View();
        }
        [HttpPost]
        public ActionResult InsertAndUpdateaRule(string data, string rank, bool status, bool IsShopInstall, int ProductType)
        {
            IEnumerable<ShippingModel> list = JsonConvert.DeserializeObject<List<ShippingModel>>(data);
            ShippingManager.InsertAndUpdateaRule(list);

            try
            {
                if (!string.IsNullOrWhiteSpace(rank))
                {
                    List<GradeDeliveryFeeRule> rankList = JsonConvert.DeserializeObject<List<GradeDeliveryFeeRule>>(rank);

                    foreach (var item in rankList)
                    {
                        if (item.PKID != 0)
                        {
                            ShippingManager.UpdateGradeDeliveryFeeRule(item);
                        }
                        else
                        {
                            ShippingManager.InsertGradeDeliveryFeeRule(item);
                        }

                    }
                }
                if (!status)
                {
                    GradeDeliveryFeeRule model = new GradeDeliveryFeeRule();
                    model.IsShopInstall = IsShopInstall;
                    model.ProductType = ProductType;
                    ShippingManager.DeleteGradeDeliveryFeeRule(model);
                }
                return Json(true);
            }
            catch (System.Exception ex)
            {
                return Json(false);
                throw ex;
            }

        }
        [HttpPost]
        public ActionResult DelRule(int pkid)
        {
            return Json(ShippingManager.DelRule(pkid));
        }
        public ActionResult InsertOprLog(string ObjectType, int ObjectID, string Operation)//轮胎到店 1  轮胎非到店2  保养到店3  保养非到店4  轮毂到店 5  轮毂非到店6
        {
            return Json(ShippingManager.InsertOprLog(ObjectType, ObjectID, Operation, User == null ? "" : User.Identity.Name, Request.UserHostName, ""), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectOprLog(string type)//轮胎到店 1  轮胎非到店2  保养到店3  保养非到店4  轮毂到店 5  轮毂非到店6
        {
            var content = "";
            OperationResult<IEnumerable<OprLogModel>> oprlist = null;
            using (var client = new OprLogClient())
            {
                oprlist = client.SelectOrderOprLog(int.Parse(type), "SetYunFei");

            }
            if (oprlist.Result == null || oprlist.Result.Count() == 0)
            {
                content = "暂无记录";
            }
            else
            {
                foreach (OprLogModel model in oprlist.Result)
                {
                    content += "<span style='display:block;margin-bottom:5px;'>操作人：" + model.Author + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;操作：" + model.Operation + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;操作时间：" + model.ChangeDatetime + "</span>";
                }
            }
            return Content(content);
        }

    }
}
