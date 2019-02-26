using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.Product;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BaoYangProductPriority;
using Tuhu.Provisioning.DataAccess.Request;

namespace Tuhu.Provisioning.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        #region 保养产品推荐 
        /// <summary>
        /// 根据类别获取产品品牌
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetProductBrand(string category)
        {
            if (string.IsNullOrEmpty(category))
                return Json(new { status = false, msg = "参数验证失败" }, JsonRequestBehavior.AllowGet);
            var manager = new ProductManager();
            var array = manager.GetProductPrioritySettingsBrand(category);
            return Json(new { status = true, data = array ?? new string[] { } }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据类别和品牌获取系列
        /// </summary>
        /// <param name="category"></param>
        /// <param name="brand"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetProductSeries(string category, string brand)
        {

            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(brand))
                return Json(new { status = false, msg = "参数验证失败" }, JsonRequestBehavior.AllowGet);
            var manager = new ProductManager();
            var array = manager.GetProductPrioritySettingsSeries(category, brand);
            return Json(new { status = true, data = array ?? new string[] { } }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据产品ID 获取产品名称
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetProductNameByPid(string pid)
        {

            BaoYangResultEntity<string> result = new BaoYangResultEntity<string>() { Status = false };
            if (string.IsNullOrEmpty(pid))
            {
                result.Msg = "参数验证失败";
            }
            else
            {
                var pids = new[] { pid.Trim() };
                var manager = new ProductManager(); 
                var productDetailDic = manager.SelectProductDetail(new List<string> { pid.Trim() });
                if (productDetailDic != null && productDetailDic.Any() && productDetailDic.ContainsKey(pid.Trim()))
                {
                    result.Status = true;
                    result.Data = productDetailDic[pid.Trim()].Name;
                } 
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取产品推荐排序
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public JsonResult GetBaoYangProductPriority(string partName, string category)
        {
            if (string.IsNullOrWhiteSpace(partName) || string.IsNullOrWhiteSpace(category))
            {
                return Json(new { status = false, Msg = "参数验证失败" }, JsonRequestBehavior.AllowGet);
            }

            var manager = new BaoYangRecommendManager();
            if (string.Equals(partName, "机油", StringComparison.OrdinalIgnoreCase) && string.Equals(category, "Oil", StringComparison.OrdinalIgnoreCase))
            {
                var data = manager.GetOilBaoYangProductPriority();
                return Json(new { status = true, Msg = "成功", Data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = manager.GetBaoYangProductPriority(partName, category);
                return Json(new { status = true, Msg = "成功", Data = data }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetAllBrandAndSeries(string partName, string category)
        {
            if (string.IsNullOrWhiteSpace(partName) || string.IsNullOrWhiteSpace(category))
            {
                return Json(new { status = false, Msg = "参数验证失败" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangRecommendManager();
            if (string.Equals(partName, "机油", StringComparison.OrdinalIgnoreCase) && string.Equals(category, "Oil", StringComparison.OrdinalIgnoreCase))
            {
                var baoYangmanager = new BaoYangManager();
                var data = baoYangmanager.GetOilBrandAndSeries();
                return Json(new { status = true, Msg = "成功", Data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = manager.GetAllBrandAndSeries(partName, category);
                return Json(new { status = true, Msg = "成功", Data = data }, JsonRequestBehavior.AllowGet);
            }

        }


        /// <summary>
        /// 保存推荐排序
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public ActionResult SaveBaoYangProductPriority(List<BaoYangProductPriorityModel> settings, string partName, string grade, string viscosity)
        {
            if (settings == null || !settings.Any()
                || string.IsNullOrWhiteSpace(partName)
                || (string.Equals(partName, "机油", StringComparison.OrdinalIgnoreCase) && (string.IsNullOrWhiteSpace(grade) || string.IsNullOrWhiteSpace(viscosity))))
            {
                return Json(new { status = false, Msg = "参数验证失败" }, JsonRequestBehavior.AllowGet);
            }
            if (settings.Count() != settings.Select(x => x.Priority).Distinct().Count())
            {
                return Json(new { status = false, Msg = "排序存在重复" }, JsonRequestBehavior.AllowGet);
            }
            var success = false;
            var manager = new BaoYangRecommendManager();
            if (!string.IsNullOrWhiteSpace(viscosity) && !string.IsNullOrWhiteSpace(grade) && string.Equals(partName, "机油", StringComparison.OrdinalIgnoreCase))
            {
                success = manager.SaveOilBaoYangProductPriority(settings, grade, viscosity, User.Identity.Name);
            }
            else
            {
                success = manager.SaveBaoYangProductPriority(settings, partName, User.Identity.Name);

            }
            return Json(new { status = success }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}