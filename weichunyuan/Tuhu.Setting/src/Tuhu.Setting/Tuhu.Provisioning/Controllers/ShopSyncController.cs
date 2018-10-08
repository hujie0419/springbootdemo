using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.ShopSync;
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
    }
}