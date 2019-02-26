using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.CarsAffairs;

namespace Tuhu.Provisioning.Controllers
{
    public class CarsAffairsController : Controller
    {
        public ActionResult CarsAffairsLog()
        {
            ViewBag.StartTime = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd");
            ViewBag.EndTime = DateTime.Now.ToString("yyyy/MM/dd");
            return View();
        }

        /// <summary>
        /// 获取车务日志
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="orderType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult SelectCarsAffairsLog(DateTime startTime, DateTime endTime, string orderType, int pageIndex = 1, int pageSize = 10)
        {
            CarsAffairsManager manager = new CarsAffairsManager();
            var result = manager.SelectCarsAffairs(startTime, endTime, orderType, pageIndex, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}