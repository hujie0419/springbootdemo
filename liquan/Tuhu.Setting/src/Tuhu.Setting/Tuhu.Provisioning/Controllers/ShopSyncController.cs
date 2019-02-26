using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.Business.GeneralBeautyServerCode;
using Tuhu.Provisioning.Business.RegionPackageMap;
using Tuhu.Provisioning.Business.ShopSync;
using Tuhu.Provisioning.Business.VipBaoYangPackage;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;
using Tuhu.Service.ThirdParty;
using Tuhu.Service.ThirdParty.Models.ShopSync;

namespace Tuhu.Provisioning.Controllers
{
    public class ShopSyncController:Controller
    {
        /// <summary>
        /// 分页查询门店同步记录
        /// </summary>
        /// <param name="company"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="syncStatus"></param>
        /// <param name="shopId"></param>
        /// <param name="simpleName"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        [HttpGet]
        public ActionResult GetShopSyncRecord(string company, int pageIndex, int pageSize,
            string syncStatus, int shopId, string simpleName, string fullName)
        {
            var result = new ShopSyncManager().SelectThirdPartyShopSyncRecord(company.Trim(), pageIndex, pageSize, syncStatus, shopId, simpleName.Trim(), fullName.Trim());
            return Json(new
            {
                data = result.Item1,
                totalCount = result.Item2,
                pageIndex = pageIndex
            },
            JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 查询所有的第三方名
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetShopSyncThirdParties()
        {
            var companies = new ShopSyncManager().SelectShopSyncThirdPartyies();
            return Json(companies, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 同步门店
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public ActionResult SyncShop(int shopId, string thirdParty)
        {
            var result = new ShopSyncManager().SyncShop(shopId, thirdParty);
            return Json(new { success = result, msg = result ? "同步成功" : "同步失败" },
                JsonRequestBehavior.AllowGet);
        }


        #region

        public ActionResult PingAnRegionPackageMap()
        {
            return View();
        }
        /// <summary>
        /// 获取地区和包的对应关系
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="byPackagePID"></param>
        /// <param name="businessId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetPingAnRegionPackageMapList(Guid? packageId, string byPackagePID, string businessId, int pageIndex = 1, int pageSize = 10)
        {
            RegionPackageMapManager mananger = new RegionPackageMapManager();
            var result = mananger.GetPingAnRegionPackageMapList(packageId, byPackagePID, businessId, pageIndex, pageSize);
            return Json(new { data = result, total = result?.FirstOrDefault()?.Total }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 新增或更新对应关系
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonResult UpsertPingAnRegionPackageMap(PingAnRegionPackageMap data)
        {
            RegionPackageMapManager mananger = new RegionPackageMapManager();
            var result = mananger.UpsertPingAnRegionPackageMap(data, User.Identity.Name);
            return Json(new { status = result.Item1, msg = result.Item2 ?? "操作失败" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除对应关系
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public JsonResult DeletePingAnRegionPackageMap(int pkid)
        {
            RegionPackageMapManager mananger = new RegionPackageMapManager();
            var result = mananger.DeletePingAnRegionPackageMap(pkid, User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取所有的美容包配置
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllThirdPartyBeautyPackageConfig()
        {
            var result = GeneralBeautyServerCodeManager.Instanse.GetAllThirdPartyBeautyPackageConfig();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取所有的保养套餐
        /// </summary>
        /// <returns></returns>
        public JsonResult SelectAllVipBaoYangPackage()
        {
            var result = new VipBaoYangPackageManager().SelectAllVipBaoYangPackage();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取所有省
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllProvince()
        {
            var result = ShopService.GetAllProvince();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据regionId获取region
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public JsonResult GetRegionByRegionId(int regionId)
        {
            var result = ShopService.GetRegionByRegionId(regionId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}