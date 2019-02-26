using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.ThirdParty;
using Tuhu.Provisioning.DataAccess.Entity.ThirdParty;

namespace Tuhu.Provisioning.Controllers
{
    public class ThirdPartyCodeController : Controller
    {
        /// <summary>
        /// 获取三方码配置信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        public JsonResult GetCodeSourceConfig(int pageIndex = 1, int pageSize = 10)
        {
            ThirdPartyManager manager = new ThirdPartyManager();
            var result = manager.GetServiceCodeSourceConfig(pageIndex, pageSize);
            return Json(new { data = result, total = result.FirstOrDefault().Total }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 编辑配置信息
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        public JsonResult UpsertCodeSourceConfig(ServiceCodeSourceConfig config)
        {
            ThirdPartyManager manager = new ThirdPartyManager();
            var result = manager.UpsertCodeSourceConfig(config);
            return Json(new { status = result.Item1, msg = result.Item2 }, JsonRequestBehavior.AllowGet);
        }
    }
}