using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class OrderAreaProductController : Controller
    {
        private Lazy<AutoSuppliesManager> LazyAutoSuppliesManager = new Lazy<AutoSuppliesManager>();

        private Lazy<OrderAreaProductManager> LazyOrderAreaProductManager = new Lazy<OrderAreaProductManager>();
        private AutoSuppliesManager AutoSuppliesManager
        {
            get
            {
                return LazyAutoSuppliesManager.Value;
            }
        }

        private OrderAreaProductManager OrderAreaProductManager
        {
            get
            {
                return LazyOrderAreaProductManager.Value;
            }
        }

        /// <summary>
        /// 添加一个产品信息
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(OrderAreaProduct product, int id)
        {

            var resultData = AutoSuppliesManager.GetProductInfoByPIdNewVersion(product.PId);
            if (resultData == null)
            {
                ViewBag.Result = "您输入的产品Id不存在.";
                return View();

            }

            if (product != null)
            {
                // 设置上一个请求传递过来的appsetDataid
                product.OrderAreaId = id;
                ViewBag.Result = OrderAreaProductManager.AddOrderAreaProduct(product);
            }

            return RedirectToAction("SearchProductsByMoudleId", new { id = id });
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// 编辑产品信息
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(OrderAreaProduct product)
        {

            //查询产品Id是否存在
            var resultData = AutoSuppliesManager.GetProductInfoByPIdNewVersion(product.PId);
            if (resultData == null)
            {
                ViewBag.Result = "您输入的产品Id不存在.";
                return View();

            }
            //1.先查询数据是否存在
            //2. 如果不存在，直接跳转到HomePageConfig/Index页面
            if (product != null)
            {
                if (OrderAreaProductManager.HasSingleProduct(product.Id))
                    ViewBag.Result = OrderAreaProductManager.UpdateOrderAreaProduct(product);
                else
                    return RedirectToAction("Index", "OrderArea");
            }

            return RedirectToAction("SearchProductsByMoudleId", new { id = product.OrderAreaId });
        }

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            //id=0表示输入了非法参数
            if (id == 0)
                return RedirectToAction("Index", "OrderArea");
            //如果数据为空,表示准备编辑的数据不存在,那么直接跳转到 HomePageConfig/Index
            var model = OrderAreaProductManager.SelectSingleOrderAreaProduct(id);
            if (model == null)
                return RedirectToAction("Index", "OrderArea");
            return View(model);
        }

        /// <summary>
        /// 通过父级模块Id获取所有产品信息
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public ActionResult SearchProductsByMoudleId(int id = 0)
        {

            #region todo
            //1. 先查询有没有这个模块

            //2. 如果有模块，数据为空，允许添加

            //3. 如果没有模块，那么数据也为空，不允许添加

            #endregion
            if (id == 0)
                return RedirectToAction("Index", "OrderArea"); //如果数据为0 表示传递的参数是非int类型

            ViewBag.ModuleId = id;
            return View(OrderAreaProductManager.GetProductByOrderAreaId(id));
        }

        [HttpPost]
        public ActionResult GetProductById(string pid)
        {
            try
            {
                if (string.IsNullOrEmpty(pid))
                    return Content("-1");
                var resultData = AutoSuppliesManager.GetProductInfoByPIdNewVersion(pid);
                if (resultData != null)
                    return Content(JsonConvert.SerializeObject(resultData, new DataTableConverter()));
                else
                    return Content("-1");
            }
            catch (Exception)
            {
                return Content("-1");
            }
        }
    }
}
