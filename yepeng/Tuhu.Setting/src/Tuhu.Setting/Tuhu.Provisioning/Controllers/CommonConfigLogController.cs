using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.ConfigLog;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class CommonConfigLogController : Controller
    {
        /// <summary>
        /// 日志列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult ListLoger()
        {
            return View();
        }
        /// <summary>
        /// 日志详情页
        /// </summary>
        /// <returns></returns>
        public ActionResult Logger()
        {
            return View();
        }

        /// <summary>
        /// 获取通用日志列表
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult GetCommonConfigLogList(Pagination pagination, string objectType, string objectId = "", string startTime = "", string endTime = "")
        {
            var manager = new CommonConfigLogManager();
            DateTime dtStartTime;
            DateTime dtEndTime;

            if (!string.IsNullOrEmpty(startTime))
                dtStartTime = Convert.ToDateTime(startTime);
            else
                dtStartTime = DateTime.Now.Date.AddDays(-30);
            if (!string.IsNullOrEmpty(endTime))
                dtEndTime = Convert.ToDateTime(endTime);
            else
                dtEndTime = DateTime.Now.Date.AddDays(1);

            var commonConfigLogList = manager.GetCommonConfigLogList(pagination, objectId, objectType, dtStartTime, dtEndTime);

            return Content(JsonConvert.SerializeObject(new
            {
                rows = commonConfigLogList,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            }));
        }
        /// <summary>
        /// 获取通用日志列表
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public JsonResult GetCommonConfigLogs(string objectType, string objectId = "")
        {
            var manager = new CommonConfigLogManager();
            Pagination pagination = new Pagination();
            pagination.page = 1;
            pagination.rows = 10000;
            var commonConfigLogList = manager.GetCommonConfigLogList(pagination, objectId, objectType);
            return Json(commonConfigLogList,JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取通用日志详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetCommonConfigLogInfo(int id)
        {
            var manager = new CommonConfigLogManager();
            var commonConfigLogInfo = manager.GetCommonConfigLogInfo(id);
            if (commonConfigLogInfo != null)
                return Content(JsonConvert.SerializeObject(commonConfigLogInfo));
            else
                return HttpNotFound();
        }
    }
}