using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.BaoYangPackageTypeRelation;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class PackageTypeRelationController : Controller
    {
        // GET: PackageTypeRelation
        public ActionResult PackageTypeRelation()
        {
            return View();
        }

        /// <summary>
        /// 获取所有保养项目
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllBaoYangPackageTypes()
        {
            var manager = new BaoYangPackageTypeRelationManager();
            var result = manager.GetAllBaoYangPackageTypes();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加保养关联项目配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddConfig(BaoYangPackageTypeRelationViewModel model)
        {
            if (model == null)
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.MainPackageType))
            {
                return Json(new { Status = false, Msg = "请选择主项目" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.RelatedPackageTypes))
            {
                return Json(new { Status = false, Msg = "请选择辅项目" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangPackageTypeRelationManager();
            var user = User.Identity.Name;
            var isExist = manager.IsExistBaoYangPackageTypeRelation(model);
            if (isExist)
            {
                return Json(new { Status = false, Msg = "该主项目已存在，不可重复添加" }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.AddBaoYangPackageTypeRelation(model, user);
            return Json(new { Status = result, Msg = $"添加{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除保养关联项目配置
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="telNum"></param>
        /// <param name="carNoPrefix"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteConfig(string mainPackageType)
        {
            if (string.IsNullOrWhiteSpace(mainPackageType))
            {
                return Json(new { Status = false, Msg = "未知的删除对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangPackageTypeRelationManager();
            var user = User.Identity.Name;
            var result = manager.DeleteBaoYangPackageTypeRelation(mainPackageType, user);
            return Json(new { Status = result, Msg = $"删除{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新保养关联项目配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateConfig(BaoYangPackageTypeRelationViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.MainPackageType))
            {
                return Json(new { Status = false, Msg = "未知的编辑对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangPackageTypeRelationManager();
            var isExist = manager.IsExistBaoYangPackageTypeRelation(model);
            if (isExist)
            {
                return Json(new { Status = false, Msg = "该主项目已存在，不可重复添加" }, JsonRequestBehavior.AllowGet);
            }
            var user = User.Identity.Name;
            var result = manager.UpdateBaoYangPackageTypeRelation(model, user);
            return Json(new { Status = result, Msg = $"编辑{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询保养关联项目配置
        /// </summary>
        /// <param name="mainPackageType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectConfig(string mainPackageType, int pageIndex = 1, int pageSize = 20)
        {
            var manager = new BaoYangPackageTypeRelationManager();
            var result = manager.SelectBaoYangPackageTypeRelation(mainPackageType, pageIndex, pageSize);
            return Json(new { Status = result != null && result.Item1 != null, Data = result.Item1, TotalCount = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查看操作日志
        /// </summary>
        /// <param name="mainPackageType"></param>
        /// <returns></returns>
        public ActionResult GetOprLog(string mainPackageType)
        {
            if (string.IsNullOrWhiteSpace(mainPackageType))
            {
                return Json(new { Status = false, Msg = "未知的查询对象" }, JsonRequestBehavior.AllowGet);
            }
            var totalCount = 0;
            var logType = "BaoYangPackageTypeRelation";
            var result = new BaoYangManager().SelectBaoYangOprLog(logType, mainPackageType, string.Empty, (DateTime?)null, (DateTime?)null, 1, 20, out totalCount);
            return Json(new { Status = result.Item1, Data = result.Item2, TotalCount = totalCount }, behavior: JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 刷新服务缓存
        /// </summary>
        /// <returns></returns>
        public ActionResult RefreshCache()
        {
            var manager = new BaoYangPackageTypeRelationManager();
            var result = manager.RefreshCache();
            return Json(new { Status = result, Msg = $"刷新服务缓存{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }
    }
}