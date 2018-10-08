using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace Tuhu.Provisioning.Controllers
{
    /// <summary>
    /// 产品品类公共选择模块
    /// </summary>
    public class ProductCategoryController : Controller
    {
        private static readonly Business.OperationCategory.OperationCategoryManager _OperationCategoryManager = new Business.OperationCategory.OperationCategoryManager();

        /// <summary>
        /// 产品品类选择页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取产品类型数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPrductCategoryData()
        {
            var bllCategory = new Business.OperationCategory.OperationCategoryManager();
            var categories = bllCategory.SelectProductCategories().ToList() ?? null;
            return Json(categories, JsonRequestBehavior.AllowGet);
        }
    }
}