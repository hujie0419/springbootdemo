using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.Vehicle;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class VehicleArticleController : Controller
    {
        #region 选车攻略配置
        /// <summary>
        /// 添加或更新选车攻略配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpSertVehicleArticle(VehicleArticleModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.VehicleId))
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.ArticleUrl))
            {
                return Json(new { Status = false, Msg = "请填写文章链接" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new VehicleArticleManager(User.Identity.Name);
            var isExist = manager.IsExistVehicleArticle(model);
            if (isExist)
            {
                return Json(new { Status = false, Msg = "已存在重复的数据，不能重复添加" }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.UpSertVehicleArticle(model);
            return Json(new { Status = result, Msg = $"添加{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除选车攻略配置
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="surfaceCount"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteVehicleArticle(VehicleArticleModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.VehicleId))
            {
                return Json(new { Status = false, Msg = "未知的删除对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new VehicleArticleManager(User.Identity.Name);
            var result = manager.DeleteVehicleArticle(model);
            return Json(new { Status = result, Msg = $"删除{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        /// <summary>
        /// 批量添加或更新选车攻略配置
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public ActionResult MultUpsertVehicleArticle(List<VehicleArticleModel> models, string articleUrl)
        {
            if (models == null || !models.Any())
            {
                return Json(new { Status = false, Msg = "未知的编辑对象" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(articleUrl))
            {
                return Json(new { Status = false, Msg = "文章链接不能为空" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new VehicleArticleManager(User.Identity.Name);
            foreach (var model in models)
            {
                if (string.IsNullOrWhiteSpace(model.VehicleId))
                {
                    return Json(new { Status = false, Msg = "请选择二级车型" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.ArticleUrl = articleUrl;
                }
            }
            var result = manager.MultUpsertVehicleArticle(models);
            return Json(new { Status = result, Msg = $"批量添加或更新{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量删除选车攻略配置
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MultDeleteVehicleArticle(List<VehicleArticleModel> models)
        {
            if (models == null || !models.Any())
            {
                return Json(new { Status = false, Msg = "未知的删除对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new VehicleArticleManager(User.Identity.Name);
            var result = manager.MultDeleteVehicleArticle(models);
            return Json(new { Status = result, Msg = $"批量删除{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据等级查询配置信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="vehicleLevel"></param>
        /// <returns></returns>
        public ActionResult SelectVehicleArticleModel(VehicleSearchModel request, int vehicleLevel)
        {
            var manager = new VehicleArticleManager(User.Identity.Name);
            if (request == null)
            {
                return Json(new { Status = false, Msg = "未知的查询对象" }, JsonRequestBehavior.AllowGet);
            }
            request.PageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
            request.PageSize = request.PageSize < 1 ? 20 : request.PageSize;
            var result = manager.SelectVehicleArticleModelByLevel(request, vehicleLevel);
            return Json(new { Status = result.Item1 != null, Msg = "", Data = result.Item1, TotalCount = result.Item2 }
                , JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public ActionResult RemoveCache(List<VehicleArticleModel> models)
        {
            if (models == null || !models.Any())
            {
                return Json(new { Status = false, Msg = "未知的查询对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new VehicleArticleManager(User.Identity.Name);
            var result = manager.RemoveCache(models);
            return Json(new { Status = result, Msg = $"清除缓存{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}