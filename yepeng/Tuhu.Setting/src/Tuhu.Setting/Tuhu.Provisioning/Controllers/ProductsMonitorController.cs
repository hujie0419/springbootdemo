using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.Business.Tire;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.CompetingProductsMonitorManager;
using Tuhu.Provisioning.DataAccess.Entity.CompetingProductsMonitor;

namespace Tuhu.Provisioning.Areas.CompetingProductsMonitor.Controllers
{
    public class ProductsMonitorController : Controller
    {
       /// <summary>
       /// 首次加载
       /// </summary>
       /// <returns></returns>
       [PowerManage]
        public ActionResult Price()
        {
            var brands = RecommendManager.GetBrands();
            var tireSizes = RecommendManager.SelectALLTireSize();
            return View(Tuple.Create(brands, tireSizes));
        }
        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="selectModel"></param>
        /// <param name="pageType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PartialViewResult PriceProductList(PriceSelectModel selectModel, string pageType, int pageIndex = 1, int pageSize = 10)
        {
            PagerModel pager = new PagerModel(pageIndex, pageSize);
            
            var monitorManager = new CompetingProductsMonitorManager();
            var monitorList = monitorManager.GetProductList(selectModel, pager);
            
            ViewBag.SelectModel = selectModel;
            ViewBag.PageType = pageType;
            return PartialView(new ListModel<CompetingProductsMonitorModel>() { Pager = pager, Source = monitorList });
        }
        /// <summary>
        /// 根据品牌获取规格
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public ActionResult GetPatternsByBrand(string brand)
        {
            var patterns = PatternManager.SelectPatternsByBrand(brand);
            return Json(patterns, JsonRequestBehavior.AllowGet);
        }
    }
}
