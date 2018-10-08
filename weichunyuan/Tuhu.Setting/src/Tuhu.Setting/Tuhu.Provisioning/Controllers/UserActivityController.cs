using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Provisioning.Controllers
{
    public class UserActivityController : Controller
    {
        /// <summary>
        /// 获取报名参加活动的用户列表
        /// </summary>
        [PowerManage(IwSystem = "OperateSys")]
        public async Task<JsonResult> GetPagedUserRegistration(string area, int pageSize = 20, int pageIndex = 1)
        {
            using (var client = new ActivityClient())
            {
                var result = (await client.GetUserRegistrationsByAreaAsync(area, pageSize, pageIndex)).Result;
                return Json(new { data = result.Source, totalCount = result.Pager.Total, pageSize, pageIndex }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取用户活动列表
        /// </summary>
        public async Task<JsonResult> GetUserActivities()
        {
            using (var client = new ActivityClient())
            {
                var result = (await client.GetUserActivitiesAsync()).Result;
                return Json(result.Select(d => new { value = d.PKID, label = d.Content }), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取用户报名信息
        /// </summary>
        public async Task<JsonResult> GetUserRegistration(long pkid)
        {
            using (var client = new ActivityClient())
            {
                var result = (await client.GetUserRegistrationByIdAsync(pkid)).Result;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 新增用户报名信息
        /// </summary>
        public async Task<JsonResult> AddUserRegistration(UserRegistrationRequest request)
        {
            using (var client = new ActivityClient())
            {
                var result = (await client.AddUserRegistrationAsync(request)).Result;
                return Json(result ? "" : "新增报名信息失败");
            }
        }

        /// <summary>
        /// 修改用户报名信息
        /// </summary>
        public async Task<JsonResult> ModifyUserRegistration(UserRegistrationRequest request)
        {
            using (var client = new ActivityClient())
            {
                var result = (await client.ModifyUserRegistrationAsync(request)).Result;
                return Json(result ? "" : "修改用户信息失败");
            }
        }
    }
}
