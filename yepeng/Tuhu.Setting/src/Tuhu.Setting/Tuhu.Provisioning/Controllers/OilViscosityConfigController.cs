using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.BaoYangOilViscosityPriorityConfig;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class OilViscosityConfigController : Controller
    {
        #region 机油粘度优先级配置
        // GET: BaoYangOilViscosityPriorityConfig
        public ActionResult OilViscosityPriorityConfig()
        {
            return View();
        }

        /// <summary>
        /// 从配置表中获取所有机油粘度
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllOilViscosity()
        {
            var manager = new BaoYangConfigManager();
            var result = manager.ViscosityList();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加或编辑机油粘度优先级配置表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage]
        public ActionResult AddOrEditOilViscosityPriorityConfig(BaoYangOilViscosityPriorityConfigModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.OriginViscosity))
            {
                return Json(new { Status = false, Msg = "未知的编辑对象" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.ConfigType) || !Enum.IsDefined(typeof(BaoYangOilViscosityPriorityConfigType), model.ConfigType))
            {
                return Json(new { Status = false, Msg = "未知的类型" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.ViscosityPriority) || !model.ViscosityPriorities.Any())
            {
                return Json(new { Status = false, Msg = "优先级不能为空" }, JsonRequestBehavior.AllowGet);
            }
            var distinctCount = model.ViscosityPriority.Split(',').Count();
            if (model.ViscosityPriorities.Count != distinctCount)
            {
                return Json(new { Status = false, Msg = $"粘度重复,请重新选择" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangOilViscosityPriorityConfigManager();
            var result = manager.AddOrEditBaoYangOilViscosityPriorityConfig(model, User.Identity.Name);
            return Json(new { Status = result, Msg = $"编辑{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除机油粘度优先级配置表
        /// </summary>
        /// <param name="originViscosity"></param>
        /// <param name="configType"></param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage]
        public ActionResult DeleteOilViscosityPriorityConfig(string originViscosity, string configType)
        {
            if (string.IsNullOrWhiteSpace(originViscosity))
            {
                return Json(new { Status = false, Msg = "未知的删除对象" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(configType) || !Enum.IsDefined(typeof(BaoYangOilViscosityPriorityConfigType), configType))
            {
                return Json(new { Status = false, Msg = "未知的类型" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangOilViscosityPriorityConfigManager();
            var result = manager.DeleteBaoYangOilViscosityPriorityConfig(originViscosity, configType, User.Identity.Name);
            return Json(new { Status = result, Msg = $"删除{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询机油粘度优先级配置表
        /// </summary>
        /// <param name="originViscosity"></param>
        /// <param name="configType"></param>
        /// <returns></returns>
        public ActionResult SelectOilViscosityPriorityConfig(string originViscosity, string configType)
        {
            if (string.IsNullOrWhiteSpace(configType) || !Enum.IsDefined(typeof(BaoYangOilViscosityPriorityConfigType), configType))
            {
                return Json(new { Status = false, Msg = "未知的类型" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangOilViscosityPriorityConfigManager();
            var result = manager.SelectBaoYangOilViscosityPriorityConfig(originViscosity, configType);
            var lenths = result?.Select(s => s.ViscosityPriorities.Count).ToList() ?? new List<int>();
            var maxPriorityLength = lenths.Any() ? lenths.Max() : 0;
            return Json(new { Status = result != null, Data = result, MaxPriorityLengrh = maxPriorityLength }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 移除机油粘度优先级配置服务缓存
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="originViscosity"></param>
        /// <returns></returns>
        public ActionResult RemoveOilViscosityPriorityConfigCache(string configType, string originViscosity)
        {
            if (string.IsNullOrWhiteSpace(configType) || string.IsNullOrWhiteSpace(originViscosity))
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangOilViscosityPriorityConfigManager();
            var result = manager.RemoveOilViscosityPriorityConfigCache(configType, originViscosity);
            return Json(new { Status = result, Msg = $"清除缓存{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 机油等级推荐规则 --服务逻辑写死 仅展示
        public ActionResult OilLevelPriority()
        {
            return View();
        }
        #endregion

        #region 配置的地区里只显示0W或5W机油
        public ActionResult OilViscosityRegionConfig()
        {
            return View();
        }

        /// <summary>
        /// 获取所有城市数据(到市一级)
        /// </summary>
        /// <returns>Json格式</returns>
        public ActionResult GetAllRegion()
        {
            var manager = new BaoYangActivitySettingManager();
            var regions = manager.GetAllRegion();
            return Json(new { Status = regions.Any(), Data = regions }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 从配置表中获取所有机油粘度并分组
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllGroupedOilViscosity()
        {
            var manager = new BaoYangOilViscosityPriorityConfigManager();
            var result = manager.GetAllGroupedOilViscosity();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取机油粘度特殊地区配置
        /// </summary>
        /// <param name="regionIds"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectOilViscosityRegionConfig(List<int> regionIds, int pageIndex = 1, int pageSize = 20)
        {
            if (regionIds == null || !regionIds.Any())
            {
                return Json(new { Status = false, Msg = "请选择城市" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangOilViscosityPriorityConfigManager();
            var result = manager.SelectOilViscosityRegionConfig(regionIds, pageIndex, pageSize);
            var totalCount = regionIds.Count;
            var totalPage = (totalCount % pageSize == 0) ? ((int)totalCount / pageSize) : ((int)totalCount / pageSize + 1);
            return Json(new { Status = result != null, Data = result, TotalCount = totalCount, TotalPage = totalPage },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量添加或编辑机油粘度特殊地区配置
        /// </summary>
        /// <param name="regionIds"></param>
        /// <param name="oilViscosity"></param>
        /// <returns></returns>
        public ActionResult MultAddOrEditOilViscosityRegionConfig(List<int> regionIds, string oilViscosity)
        {
            if (regionIds == null || !regionIds.Any())
            {
                return Json(new { Status = false, Msg = "请选择要编辑的对象" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(oilViscosity))
            {
                return Json(new { Status = false, Msg = "粘度不能为空" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangOilViscosityPriorityConfigManager();
            var user = User.Identity.Name;
            var result = manager.MultAddOrEditOilViscosityRegionConfig(regionIds, oilViscosity, user);
            return Json(new { Status = result, Msg = $"批量编辑{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量删除机油粘度特殊地区配置
        /// </summary>
        /// <param name="regionIds"></param>
        /// <returns></returns>
        public ActionResult MultDeleteOilViscosityRegionConfig(List<int> regionIds)
        {
            if (regionIds == null || !regionIds.Any())
            {
                return Json(new { Status = false, Msg = "未知的删除对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangOilViscosityPriorityConfigManager();
            var user = User.Identity.Name;
            var result = manager.MultDeleteOilViscosityRegionConfig(regionIds, user);
            return Json(new { Status = result, Msg = $"批量删除{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 移除机油粘度特殊地区配置服务缓存
        /// </summary>
        /// <param name="regionIds"></param>
        /// <returns></returns>
        public ActionResult RemoveOilViscosityRegionConfigCache(List<int> regionIds)
        {
            if (regionIds == null || !regionIds.Any())
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangOilViscosityPriorityConfigManager();
            var result = manager.RemoveOilViscosityRegionConfigCache(regionIds);
            return Json(new { Status = result, Msg = $"清除缓存{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        #region 查询日志
        /// <summary>
        /// 查询机油粘度特殊地区操作日志
        /// </summary>
        /// <param name="identityId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult GetOilViscosityRegionOprLog(string identityId, string type)
        {
            if (string.IsNullOrWhiteSpace(identityId) || string.IsNullOrWhiteSpace(type))
            {
                return Json(new { Status = false, Msg = "未知的查询对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangActivitySettingManager();
            var result = manager.GetBaoYangOprLogByIdentityIdAndType(identityId, type);
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion
    }
}