using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class CouponActivityConfigController : Controller
    {

        private static readonly Lazy<CouponActivityConfigManager> lazyCouponActivityConfigManager = new Lazy<CouponActivityConfigManager>();

        private CouponActivityConfigManager CouponActivityConfigManager
        {
            get { return lazyCouponActivityConfigManager.Value; }
        }

        public ActionResult Index(int type, int pageIndex = 1, int pageSize = 15)
        {
            int count = 0;
            string strSql = string.Empty;
            var lists = CouponActivityConfigManager.GetCouponActivityConfigList(type, strSql, pageSize, pageIndex, out count);
            var list = new OutData<List<CouponActivityConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = list.OutValue
            };
            return View(new ListModel<CouponActivityConfig>(list.ReturnValue, pager));
        }

        public ActionResult Edit(int type, int id = 0)
        {
            if (id == 0)
            {
                CouponActivityConfig model = new CouponActivityConfig();
                model.Type = type;
                return View(model);
            }
            else
            {
                return View(CouponActivityConfigManager.GetCouponActivityConfig(id));
            }

        }

        [HttpPost]
        public ActionResult Edit(CouponActivityConfig model)
        {
            string js = "<script>alert(\"保存失败 \");location='/CouponActivityConfig/Index';</script>";
            if (model.Id != 0)
            {
                if (CouponActivityConfigManager.UpdateCouponActivityConfig(model))
                {
                    return RedirectToAction("Index", "CouponActivityConfig", new { type = model.Type });
                }
                else
                {
                    return Content(js);
                }
            }
            else
            {
                if (CouponActivityConfigManager.InsertCouponActivityConfig(model))
                {
                    return RedirectToAction("Index", "CouponActivityConfig", new { type = model.Type });
                }
                else
                {
                    return Content(js);
                }
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            return Json(CouponActivityConfigManager.DeleteCouponActivityConfig(id));
        }

        public ActionResult Product()
        {
            return View();
        }
    }
}
