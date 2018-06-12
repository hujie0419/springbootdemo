using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Nosql;
using Tuhu.Profiling.Util;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models;
using AjaxHelper = Tuhu.Provisioning.Models.WebResult.AjaxHelper;

namespace Tuhu.Provisioning.Controllers
{
    public class WashCarActivityController : Controller
    {
        private static readonly string WashCarActivityUrlPrefix =
            "http://tuhu.website.activity/Activity/ApplyActivityPage?activityId=";
        #region 随机免费洗车活动

        public ActionResult ActivityList()
        {
            return View();
        }

        /// <summary>
        /// 获取活动列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetActivityModels(int pageIndex = 1, int pageSize = 10)
        {
            using (var client = new ActivityClient())
            {
                var result = await client.GetActivityModelsPagedAsync(pageIndex, pageSize);
                if (result.Success)
                {
                    foreach (var activity in result.Result.Item1)
                    {
                        var applyUserCountResult =
                            await client.GetActivityApplyUserCountByActivityIdAsync(activity.ActivityId);
                        var auditPassUserCount =
                            await client.GetActivityApplyUserPassCountByActivityIdAsync(activity.ActivityId);
                        activity.ApplyUserCount = applyUserCountResult.Success ? applyUserCountResult.Result : 0;
                        activity.AuditPassUserCount = auditPassUserCount.Success ? auditPassUserCount.Result : 0;
                    }
                    return AjaxHelper.MvcJsonResult(HttpStatusCode.OK, "成功", result.Result);
                }
                else
                {
                    return AjaxHelper.MvcJsonResult(HttpStatusCode.BadGateway, "服务器内部错误");
                }
            }
        }
        /// <summary>
        /// 创建或修改活动
        /// </summary>
        /// <param name="activityModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CreateOrUpdateActivity(ActivityNewModel activityModel)
        {
            if (string.IsNullOrEmpty(activityModel.Name))
            {
                return AjaxHelper.MvcJsonResult(HttpStatusCode.BadRequest, "活动名称不能为空");
            }
            var dateNow = DateTime.Now.Date;
            if (activityModel.StartTime < dateNow || activityModel.EndTime <= dateNow)
            {
                return AjaxHelper.MvcJsonResult(HttpStatusCode.BadRequest, "活动开始时间或结束时间不能小于当前时间");
            }
            if (activityModel.EndTime <= activityModel.StartTime)
            {
                return AjaxHelper.MvcJsonResult(HttpStatusCode.BadRequest, "活动结束时间不能小于等于活动开始时间");
            }
            if (activityModel.Quota <= 0)
            {
                return AjaxHelper.MvcJsonResult(HttpStatusCode.BadRequest, "活动限购不能小于等于0");
            }

            activityModel.CreateUser = HttpContext.User.Identity.Name;
            using (var client = new ActivityClient())
            {
                if (activityModel.PKID > 0)//更新活动
                {
                    var result = await client.UpdateActivityModelAsync(activityModel);
                    //刷新活动缓存
                    var recacheResult = await client.RefreshActivityModelByActivityIdCacheAsync(activityModel.ActivityId);
                    if (!result.Success || !recacheResult.Success)
                    {
                        return AjaxHelper.MvcJsonResult(HttpStatusCode.BadGateway, "服务器内部错误");
                    }
                }
                else//创建活动
                {
                    activityModel.ActivityId = Guid.NewGuid();
                    activityModel.ActivityUrl = WashCarActivityUrlPrefix + activityModel.ActivityId;
                    var result = await client.InsertActivityModelAsync(activityModel);
                    if (!result.Success)
                    {
                        return AjaxHelper.MvcJsonResult(HttpStatusCode.BadGateway, "服务器内部错误");
                    }
                }
            }
            return AjaxHelper.MvcJsonResult(HttpStatusCode.OK, "成功");
        }
        /// <summary>
        /// 删除活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> DeleteActivityByActivityId(Guid activityId)
        {
            if (activityId == Guid.Empty)
            {
                return AjaxHelper.MvcJsonResult(HttpStatusCode.BadRequest, "缺少必要参数");
            }
            using (var activityClient = new ActivityClient())
            {
                var result = await activityClient.DeleteActivityModelByActivityIdAsync(activityId);
                if (result.Success && result.Result)
                {
                    //移除活动缓存
                    await activityClient.RemoveActivityModelByActivityIdCacheAsync(activityId);
                    return AjaxHelper.MvcJsonResult(HttpStatusCode.OK, "成功");
                }
                return AjaxHelper.MvcJsonResult(HttpStatusCode.BadGateway, "失败");
            }
        }

        private async Task<bool> CheckApplyUserCountAsync(Guid activityId, int quota)
        {
            //判断当前活动审核通过人数是否已达上限
            using (var client = new ActivityClient())
            {
                var auditPassUserCount =
                        await client.GetActivityApplyUserPassCountByActivityIdAsync(activityId);
                if (auditPassUserCount.Success)
                {
                    if (auditPassUserCount.Result >= quota)
                    {
                        return false;
                    }
                }
                else
                {
                    Logger.Error($"审核通过人数查询失败。Message:{auditPassUserCount.Exception}");
                    return false;
                }
            }
            return true;
        }

        [HttpGet]
        public ActionResult UserActivityList(Guid activityId)
        {
            ViewBag.ActivityId = activityId;
            return View();
        }
        /// <summary>
        /// 获取用户报名列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetUserActivityModels(Guid activityId, Service.Activity.Enum.AuditStatus auditStatus = Service.Activity.Enum.AuditStatus.None, int pageIndex = 1, int pageSize = 10)
        {
            using (var client = new ActivityClient())
            {
                var result = await client.GetUserApplyActivityModelsPagedAsync(activityId, auditStatus, pageIndex, pageSize);
                if (result.Success)
                {
                    return AjaxHelper.MvcJsonResult(HttpStatusCode.OK, "成功", result.Result);
                }
                return AjaxHelper.MvcJsonResult(HttpStatusCode.BadGateway, "服务器内部错误");
            }
        }
        /// <summary>
        /// 审核用户报名活动
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AuditUserActivityStatusByPKID(int pkid, Guid activityId, Service.Activity.Enum.AuditStatus status, string remark)
        {
            if (pkid <= 0 || activityId == Guid.Empty || status != Service.Activity.Enum.AuditStatus.NotPassed && status != Service.Activity.Enum.AuditStatus.Passed)
            {
                return AjaxHelper.MvcJsonResult(HttpStatusCode.BadRequest, "参数不完整");
            }
            string sendText;
            var userActivity = new UserApplyActivityModel
            {
                PKID = pkid,
                Remark = remark,
                Status = status
            };
            if (status == Service.Activity.Enum.AuditStatus.Passed)
            {
                using (var client = new ActivityClient())
                {
                    var activity = await client.GetActivityModelByActivityIdAsync(activityId);
                    if (activity.Success)
                    {
                        if (!await CheckApplyUserCountAsync(activityId, activity.Result.Quota))
                        {
                            return AjaxHelper.MvcJsonResult(HttpStatusCode.Accepted, "活动审核通过人数已满");
                        }
                    }
                    else
                    {
                        return AjaxHelper.MvcJsonResult(HttpStatusCode.BadGateway, "服务器内部错误");
                    }
                }
                var serviceCode = Guid.NewGuid();
                sendText = serviceCode.ToString();
                userActivity.ServiceCode = serviceCode;
            }
            else
            {
                sendText = $"抱歉，您报名途虎免费洗车活动审核未通过。备注:{remark}";
            }
            using (var activityClient = new ActivityClient())
            {
                var result = await activityClient.UpdateUserActivityStatusByPKIDAsync(userActivity);
                if (result.Success && result.Result)
                {
                    //短信发送服务码
                    var ua = await activityClient.GetUserApplyActivityByPKIDAsync(pkid);
                    if (ua.Success)
                    {
                        using (var client = new Service.Utility.SmsClient())
                        {
                            var sendResult = client.SendSms(ua.Result.Mobile, 138, sendText);
                            if (sendResult.Success)
                            {
                                return AjaxHelper.MvcJsonResult(HttpStatusCode.OK, "审核成功");
                            }
                        }
                    }
                    return AjaxHelper.MvcJsonResult(HttpStatusCode.OK, "审核成功,但短信发送失败。");
                }
            }
            return AjaxHelper.MvcJsonResult(HttpStatusCode.BadGateway, "审核失败");
        }

        /// <summary>
        /// 删除用户报名
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> DeleteUserActivityByPKID(int pkid)
        {
            if (pkid <= 0)
            {
                return AjaxHelper.MvcJsonResult(HttpStatusCode.BadRequest, "缺少必要参数");
            }
            using (var client = new ActivityClient())
            {
                var result = await client.DeleteUserApplyActivityModelByPKIDAsync(pkid);
                if (result.Success && result.Result)
                {
                    return AjaxHelper.MvcJsonResult(HttpStatusCode.OK, "成功");
                }
                return AjaxHelper.MvcJsonResult(HttpStatusCode.BadGateway, "失败");
            }
        }
        #endregion
    }
}