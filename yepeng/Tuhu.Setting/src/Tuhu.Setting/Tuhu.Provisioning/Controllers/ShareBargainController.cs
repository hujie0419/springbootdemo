using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.Business.ShareBargain;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Framework.Identity;
using Tuhu.Service.Activity;

namespace Tuhu.Provisioning.Controllers
{
    public class ShareBargainController : Controller
    {
        // GET: ShareBargain
        public ActionResult ShareBargain()
        {
            return View(ShareBargainManager.GetBackgroundTheme());
        }
        [HttpPost]
        public PartialViewResult ShareBargainList(BargainProductRequest request)
        {
            var pager = new PagerModel(request.PageIndex, request.PageSize);
            ViewBag.request = request;
            var data = ShareBargainManager.SelectBargainProductList(request, pager);
            return PartialView(new ListModel<ShareBargainItemModel>() { Pager = pager, Source = data });
        }

        [HttpGet]
        public JsonResult FetchBargainProductById(int apId)
        {
            if (apId == 0)
            {
                return Json(new
                {
                    Code = 0,
                    Message = "参数不符合要求"
                }, JsonRequestBehavior.AllowGet);
            }

            var dat = ShareBargainManager.FetchBargainProductById(apId);
            if (dat == null)
            {
                return Json(new
                {
                    Code = 2,
                    Message = "未找到详细信息"
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                Code = 1,
                Result = dat
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckPid(string PID, DateTime BeginTime, DateTime EndTime)
        {
            if (string.IsNullOrWhiteSpace(PID))
            {
                return Json(new CheckPidResult()
                {
                    Code = 0,
                    Info = "PID不能为空"
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(ShareBargainManager.CheckPid(PID, BeginTime, EndTime), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 检查优惠券id是否可用
        /// </summary>
        /// <param name="PID">优惠券id</param>
        /// <param name="BeginTime">砍价产品开始时间</param>
        /// <param name="EndTime">砍价产品结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult CheckCouponPid(string PID, DateTime BeginTime, DateTime EndTime)
        {
            if (string.IsNullOrWhiteSpace(PID))
            {
                return Json(new CheckPidResult()
                {
                    Code = 0,
                    Info = "PID不能为空"
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(ShareBargainManager.CheckCouponPid(PID, BeginTime, EndTime), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddSharBargainProduct(ShareBargainProductModel request)
        {
            if (request.EndDateTime < request.BeginDateTime)
            {
                return Json(new { Code = 0, Info = "下架时间必须大于上架时间" });
            }
            if (request.ShowBeginTime >= request.BeginDateTime)
            {
                return Json(new { Code = 0, Info = "开始显示时间必须小于上架时间" });
            }
            var data = ShareBargainManager.CheckPid(request.PID, request.BeginDateTime, request.EndDateTime);
            if (data.Code == 2)
            {
                return Json(new { Code = 0, Info = "该时间段不能配置该商品" });
            }
            var dat = ShareBargainManager.AddSharBargainProduct(request, ThreadIdentity.Operator.Name);
            if (dat)
            {
                using (var client = new ShareBargainClient())
                {
                    client.RefreshShareBargainCache();
                }
                return Json(new { Code = 1, Info = "添加成功" });
            }
            return Json(new { Code = 0, Info = "添加失败，请稍后重试" });
        }

        /// <summary>
        /// 添加砍价优惠券信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddSharBargainCoupon(ShareBargainProductModel request)
        {
            if (request.EndDateTime < request.BeginDateTime)
            {
                return Json(new { Code = 0, Info = "下架时间必须大于上架时间" });
            }

            if (request.ShowBeginTime >= request.BeginDateTime)
            {
                return Json(new { Code = 0, Info = "开始显示时间必须小于上架时间" });
            }

            var data = ShareBargainManager.CheckCouponPid(request.PID, request.BeginDateTime, request.EndDateTime);
            if (data.Code == 2)
            {
                return Json(new { Code = 0, Info = "该时间段不能配置该商品" });
            }
            var dat = ShareBargainManager.AddSharBargainCoupon(request, ThreadIdentity.Operator.Name);
            if (dat)
            {
                using (var client = new ShareBargainClient())
                {
                    client.RefreshShareBargainCache();
                }
                return Json(new { Code = 1, Info = "添加成功" });
            }
            return Json(new { Code = 0, Info = "添加失败，请稍后重试" });
        }

        [HttpPost]
        public bool UpdateGlobalConfig(BargainGlobalConfigModel request)
        {
            request.QAData = JsonConvert.SerializeObject(request.BargainRule);
            return ShareBargainManager.UpdateGlobalConfig(request);
        }
        [HttpGet]
        public JsonResult FetchBargainProductGlobalConfig()
        {
            return Json(ShareBargainManager.FetchBargainProductGlobalConfig(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool UpdateBargainProductById(ShareBargainProductModel request)
        {
            if (request.PKID == null)
            {
                return false;
            }
            if (request.CurrentStockCount > request.TotalStockCount 
                || request.EndDateTime < request.BeginDateTime
                || request.ShowBeginTime >= request.BeginDateTime)
            {
                return false;
            }
            var dat = ShareBargainManager.UpdateBargainProductById(request);
            if (dat)
            {
                using (var client = new ShareBargainClient())
                {
                    client.RefreshShareBargainCache();
                }
            }
            return dat;
        }

        /// <summary>
        /// 修改砍价优惠券信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public bool UpdateBargainCouponById(ShareBargainProductModel request)
        {
            if (request.PKID == null)
            {
                return false;
            }
            if (request.CurrentStockCount > request.TotalStockCount 
                || request.EndDateTime < request.BeginDateTime
                || request.ShowBeginTime >= request.BeginDateTime)
            {
                return false;
            }
            var dat = ShareBargainManager.UpdateBargainCouponById(request);
            if (dat)
            {
                using (var client = new ShareBargainClient())
                {
                    client.RefreshShareBargainCache();
                }
            }
            return dat;
        }

        [HttpGet]
        public int DeleteBargainProductById(int PKID)
        {
            if (PKID < 1)
            {
                return 0;
            }
            var dat = ShareBargainManager.DeleteBargainProductById(PKID);
            if (dat)
            {
                using (var client = new ShareBargainClient())
                {
                    client.RefreshShareBargainCache();
                }
            }
            return dat ? 1 : 0;
        }

        [HttpGet]
        public bool RefreshShareBargain()
        {
            using (var client = new ShareBargainClient())
            {
                var dat = client.RefreshShareBargainCache();
                if (dat.Success && dat.Result)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}