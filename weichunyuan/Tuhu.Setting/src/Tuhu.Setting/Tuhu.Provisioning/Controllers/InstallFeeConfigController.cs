using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.BaoYangInstallFeeConfig;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Config;

namespace Tuhu.Provisioning.Controllers
{
    public class InstallFeeConfigController : Controller
    {
        // GET: BaoYangInstallFeeConfig
        public ActionResult InstallFeeConfig()
        {
            return View();
        }

        /// <summary>
        /// 添加或编辑保养项目加价配置
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage]
        public ActionResult AddOrEditBaoYangInstallFeeConfig(List<BaoYangInstallFeeConfigModel> models)
        {
            if (models == null && models.Count < 1)
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            BaoYangInstallFeeConfigManager manager = new BaoYangInstallFeeConfigManager();
            var isLegal = manager.IsPriceLegalBaoYangInstallFeeConfig(models);
            if (!isLegal)
            {
                return Json(new { Status = false, Msg = "价格区间需连续" }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.AddOrEditBaoYangInstallFeeConfig(models, User.Identity.Name);
            if (result)
            {
                return Json(new { Status = true, Msg = "操作成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "操作失败" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 删除保养项目加价配置
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage]
        public ActionResult DeleteBaoYangInstallFeeConfig(string serviceId)
        {
            if (string.IsNullOrWhiteSpace(serviceId))
            {
                return Json(new { Status = false, Msg = "未知的删除对象" }, JsonRequestBehavior.AllowGet);
            }
            BaoYangInstallFeeConfigManager manager = new BaoYangInstallFeeConfigManager();
            var result = manager.DeleteBaoYangInstallFeeConfig(serviceId, User.Identity.Name);
            if (result)
            {
                return Json(new { Status = true, Msg = "删除成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Msg = "删除失败" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 查询保养项目加价配置
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectBaoYangInstallFeeConfig(string serviceId = "")
        {
            var manager = new BaoYangInstallFeeConfigManager();
            var result = manager.SelectBaoYangInstallFeeConfig(serviceId);
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取所有保养项目
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllBaoYangServices()
        {
            BaoYangInstallFeeConfigManager manager = new BaoYangInstallFeeConfigManager();
            var result = manager.GetAllBaoYangServices();
            if (result.Any())
            {
                return Json(new { Status = true, Data = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = false, Data = result , Msg = "无保养项目" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 清除保养项目加价服务缓存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RefreshBaoYangInstallFeeConfigCache()
        {
            BaoYangInstallFeeConfigManager manager = new BaoYangInstallFeeConfigManager();
            var result = manager.RefreshBaoYangInstallFeeConfigCache();
            return Json(new { Status = result, Msg = "刷新缓存" + (result ? "成功" : "失败") }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVehicleAdditionalPriceSwitchStatus()
        {
            BaoYangInstallFeeConfigManager manager = new BaoYangInstallFeeConfigManager();
            var result = manager.GetVehicleAdditionalPriceSwitchStatus();
            return Json(new {Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 改变特殊车型附加安装费开关
        /// </summary>
        /// <param name="isOpen"></param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage]
        public ActionResult UpdateVehicleAdditionalPriceSwitch(bool isOpen)
        {
            var manager = new BaoYangInstallFeeConfigManager();
            var result = manager.UpdateVehicleAdditionalPriceSwitch(isOpen);
            return Json(new { Status = result, Msg = $"{(isOpen ? "开启" : "关闭")}开关{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }
    }
}