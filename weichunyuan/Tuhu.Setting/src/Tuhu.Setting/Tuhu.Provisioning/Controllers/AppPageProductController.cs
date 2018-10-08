using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.AppHomeConfig;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    /// <summary>
    /// App 首页配置
    /// </summary>
    public class AppPageProductController : Controller
    {
        IProductServices services = new ProductServicesImpl();
        private static IAutoSuppliesManager manager = new AutoSuppliesManager();

        /// <summary>
        /// 添加一个产品信息
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(TblAdProductV2 product , int id)
        {

            var resultData = manager.GetProductInfoByPIdNewVersion(product.PId);
            if (resultData == null) 
            {
                ViewBag.Result = "您输入的产品Id不存在.";
                return View();

            }

            if (product != null)
            { 
                // 设置上一个请求传递过来的appsetDataid
                product.NewAppSetDataId = id;
                ViewBag.Result = services.AddProduct(product);
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
        public ActionResult Edit(TblAdProductV2 product)
        {

            //查询产品Id是否存在
            var resultData = manager.GetProductInfoByPIdNewVersion(product.PId);
            if (resultData == null)
            {
                ViewBag.Result = "您输入的产品Id不存在.";
                return View();

            }
            //1.先查询数据是否存在
            //2. 如果不存在，直接跳转到HomePageConfig/Index页面
            if (product != null) 
            {
                if(services.HasSingleProduct(product.Id))
                    ViewBag.Result = services.UpdateProduct(product);
                else
                    return RedirectToAction("Index", "HomePageConfig");
            }

            return RedirectToAction("SearchProductsByMoudleId", new { id = product.NewAppSetDataId });
        }

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            //id=0表示输入了非法参数
            if (id == 0)
                return RedirectToAction("Index", "HomePageConfig");
            //如果数据为空,表示准备编辑的数据不存在,那么直接跳转到 HomePageConfig/Index
            var model = services.SelectSingleProduct(id);
            if(model == null)
                return RedirectToAction("Index", "HomePageConfig");
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
                return RedirectToAction("Index", "HomePageConfig"); //如果数据为0 表示传递的参数是非int类型

            ViewBag.ModuleId = id;
            return View(services.GetProductByNewsAppSetDataId(id));
        }

        [HttpPost]
        public ActionResult GetProductById(string pid)
        {
             try
            {
                if (string.IsNullOrEmpty(pid))
                    return Content("-1");
                var resultData = manager.GetProductInfoByPIdNewVersion(pid);
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
