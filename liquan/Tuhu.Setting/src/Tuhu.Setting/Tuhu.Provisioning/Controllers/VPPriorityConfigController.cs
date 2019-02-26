using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class VPPriorityConfigController:Controller
    {
        /// <summary>
        /// 选择优先级
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> SelectPriority
            (string productType, string configType, int provinceId, int cityId, int pageIndex, int pageSize = 20)
        {
            if (string.IsNullOrEmpty(productType) || string.IsNullOrEmpty(configType))
            {
                return Json(new { Satus = false, Msg = "未知的查询对象" }, JsonRequestBehavior.AllowGet);
            }
            var pager = new PagerModel(pageIndex, pageSize);
            var manager = new VendorProductPriorityConfigManager();
            var searchResult = await manager.SelectVendorProductPriorityConfigPriority
                (productType, configType, provinceId, cityId, pager);
            return Json(new { Status = searchResult.Item1 != null, Data = searchResult.Item1, TotalCount = searchResult.Item2 }
                , JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑优先级
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public ActionResult EditPriority(VendorProductPriorityConfigModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.ProductType) || string.IsNullOrEmpty(model.ConfigType))
            {
                return Json(new { Satus = false, Msg = "未知的编辑对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new VendorProductPriorityConfigManager();
            var result = manager.UpSertVendorProductPriorityConfigPriority(model, User.Identity.Name);
            return Json(new { Status = result, Msg = $"更新优先级{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> RemoveCache(VendorProductPriorityConfigModel model)
        {
            if (string.IsNullOrEmpty(model?.ProductType))
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new VendorProductPriorityConfigManager();
            var result = await manager.RemoveCache(new List<VendorProductPriorityConfigModel>(1) { model });
            return Json(new { Status = result, Msg = $" 清除缓存{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }
    }
}