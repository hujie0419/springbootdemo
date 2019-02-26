using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.PaymentWay;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.Payment;

namespace Tuhu.Provisioning.Controllers
{
    public class AirlinesController : Controller
    {
        private readonly IAirlinesManager manager = new AirlinesManager();
        private readonly IPaymentWayManager payMentWaymanager = new PaymentWayManager();

		[PowerManage]
        [HttpGet]
        public ActionResult AirlinesIndex()
        {
            List<Airlines> _AllCouponCategory = manager.GetAllAirlines();
            ViewBag.CouponCategory = _AllCouponCategory;
            return View();
        }

        //添加客服信息视图
        [HttpGet]
        public ActionResult AddAirlinesView()
        {
            return View();
        }

        //添加客服信息
        [HttpPost]
        public ActionResult AddAirlines(Airlines airlines)
        {
            string kstr = "";
            bool IsSuccess = true;
            if (string.IsNullOrEmpty(airlines.AirlinesName))
            {
                kstr = "名称不能为空";
                IsSuccess = false;
            }
            if (airlines.State != 0 && airlines.State != 1)
            {
                kstr = "状态不能为空";
                IsSuccess = false;
            }
            airlines.ID = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            airlines.CreateDateTime = System.DateTime.Now;
            if (IsSuccess)
            {
                manager.AddAirlines(airlines);
                return RedirectToAction("AirlinesIndex");
            }
            else
            {
                string js = "<script>alert(\"" + kstr + "\");location='AirlinesIndex';</script>";
                return Content(js);
            }
        }

        //删除客服信息
        [HttpPost]
        public ActionResult DeleteAirlines(string id)
        {
            try
            {
                manager.DeleteAirlines(id);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        //修改客服信息视图
        [HttpGet]
        public ActionResult EditAirlinesView(string Id)
        {
            var Airlines = manager.GetCouponCategoryByID(Id);
            return View(Airlines);
        }

        //修改客服信息
        [HttpPost]
        public ActionResult EditAirlines(Airlines airlines)
        {
            string kstr = "";
            bool IsSuccess = true;
            if (string.IsNullOrEmpty(airlines.AirlinesName))
            {
                kstr = "名称不能为空";
                IsSuccess = false;
            }
            if (airlines.State != 0 && airlines.State != 1)
            {
                kstr = "状态不能为空";
                IsSuccess = false;
            }
            airlines.CreateDateTime = System.DateTime.Now;
            if (IsSuccess)
            {
                manager.UpdateAirlines(airlines);
                return RedirectToAction("AirlinesIndex");
            }
            else
            {
                string js = "<script>alert(\"" + kstr + "\");location='AirlinesIndex';</script>";
                return Content(js);
            }
        }

        public ActionResult WebLog()
        {
            var s = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
            return Content(s);
        }

        //-----------------------------------------------支付配置
		[PowerManage]
        public ActionResult PaySeting()
        {
            dynamic payModel = new PaymentManager().GetPayment_way();
            var text = "";
            if (payModel.State == 1)
            {
                text = "只显示微信和更多";
            }
            if (payModel.State == 2)
            {
                text = "只显示支付宝和更多";
            }
            if (payModel.State == 3)
            {
                text = "微信在上";
            }
            if (payModel.State == 4)
            {
                text = "支付宝在上";
            }
            ViewBag.text = text;
            ViewBag.id = payModel.id;
            return View();
        }

        public ActionResult SavePay(int? svalue)
        {
            if (svalue.HasValue)
            {
                var n = new PaymentManager().UpdateState(svalue.Value);

                return Content(n.ToString());
            }
            return Content("0");
        }

        [HttpGet]
        public ActionResult AddPaySeting()
        {
            List<PaymentWayModel> _AllPaymentWayModelList = payMentWaymanager.GetAllPaymentWay();
            ViewBag.payModelList = _AllPaymentWayModelList;
            return View();
        }

        [HttpPost]
        public string UpdatePaymentWay(string payMentWayStr)
        {
            var paymentWayModelList = PaymentWaySerialize(payMentWayStr);
            if (paymentWayModelList != null && paymentWayModelList.Count > 0)
            {
                if (payMentWaymanager.UpdatePaymentWay(paymentWayModelList))
                    return "1";
                else
                    return "0";
            }
            return "0";
        }

        /// <summary>
        /// 支付方式数据序列化
        /// </summary>
        /// <param name="payMentWay"></param>
        /// <returns></returns>
        public List<PaymentWayModel> PaymentWaySerialize(string payMentWay)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<PaymentWayModel>>(payMentWay) ?? null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}