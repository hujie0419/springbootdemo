using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.MemberPage;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.DataAccess.Entity.MemberPage;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class NewActivityController : Controller
    {
        // GET: NewActivity
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取报名用户信息分页数据
        /// </summary>
        /// <param name="area"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        public JsonResult GetActivityPageList(string area = "", int pageIndex = 1, int pageSize = 20)
        {
            using (var activityClient = new ActivityClient())
            {
                var result = activityClient.SelectEnrollInfoByArea(area, pageIndex, pageSize);
                //return Json(new { msg = result.ErrorMessage }, JsonRequestBehavior.AllowGet);
                return Json(new { data = result.Result.Source, totalCount = result.Result.Pager.Total, pageSize = result.Result.Pager.PageSize, currentPage = result.Result.Pager.CurrentPage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据手机号获取model
        /// </summary>
        /// <param name="tel"></param>
        /// <returns></returns>
        public JsonResult GetEnrollModel(string tel = "")
        {
            using (var activityClient = new ActivityClient())
            {
                var result = activityClient.GetEnrollInfoModelByTel(tel);
                result.ThrowIfException(true);
                return Json(new { data = result.Result }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 修改报名用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        [HttpPost]
        public JsonResult UpdateEnrollInfo(TEnrollInfoModel model)
        {
            using (var activityClient = new ActivityClient())
            {
                var result = activityClient.UpdateEnrollInfo(model);
                if (!result.Success)
                {
                    return Json(new { msg = "编辑报名用户信息失败！" });
                }
                else
                {
                    return Json(new { data = result.Result });
                }
            }

        }

        /// <summary>
        /// 添加报名用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        [HttpPost]
        public JsonResult AddEnrollInfo(TEnrollInfoModel model)
        {
            using (var activityClient = new ActivityClient())
            {
                var result = activityClient.AddEnrollInfo(model);
                if (!result.Success)
                {
                    return Json(new { msg = "编辑报名用户信息失败！" });
                }
                else
                {
                    return Json(new { data = result.Result });
                }
            }
        }

        /// <summary>
        /// 上海市闵行区用户的报名状态为已通过
        /// </summary>
        [PowerManage(IwSystem = "OperateSys")]
        [HttpPost]
        public JsonResult UpdateUserEnrollStatus(string area)
        {
            using (var activityClient = new ActivityClient())
            {
                var result = activityClient.UpdateUserEnrollStatus(area);
                return Json(new { data = result.Result });
            }

        }
    }
}