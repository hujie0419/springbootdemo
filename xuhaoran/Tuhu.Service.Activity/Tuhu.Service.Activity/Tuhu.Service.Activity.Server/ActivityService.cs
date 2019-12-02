using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Server.Manager;
using Tuhu.Service.Activity.Server.Utils;

namespace Tuhu.Service.Activity.Server
{
    public class ActivityService : IActivityService
    {
        public Task<OperationResult<TireActivityModel>> SelectTireActivityAsync(string vehicleId, string tireSize) =>
            OperationResult.FromResultAsync(ActivityManager.SelectTireActivityAsync(vehicleId, tireSize));


        public async Task<OperationResult<List<T_Activity_xhrModel>>> GetAllActivityAsync(int pageIndex, int pageSize) =>
            OperationResult.FromResult(await ActivityManager.GetAllActivityAsync(pageIndex, pageSize));


        /// <summary>
        /// 新增活动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> AddActivityAsync(T_Activity_xhrModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_IsRequired, "标题"));
            if (string.IsNullOrWhiteSpace(request.ActivityContent))
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_IsRequired, "内容"));
            if(request.StartTime >= request.EndTime || request.StartTime == DateTime.MinValue || request.EndTime == DateTime.MinValue)
                return OperationResult.FromError<bool>("-31", "时间参数错误");
            if (string.IsNullOrWhiteSpace(request.Picture))
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_IsRequired, "图片地址"));
            return OperationResult.FromResult<bool>(await ActivityManager.AddActivityAsync(request));
        }

        /// <summary>
        /// 修改活动信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> UpdateActivityAsync(T_Activity_xhrModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_IsRequired, "标题"));
            if (string.IsNullOrWhiteSpace(request.ActivityContent))
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_IsRequired, "内容"));
            if (request.StartTime >= request.EndTime || request.StartTime == DateTime.MinValue || request.EndTime == DateTime.MinValue)
                return OperationResult.FromError<bool>("-31", "时间参数错误");
            if (string.IsNullOrWhiteSpace(request.Picture))
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_IsRequired, "图片地址"));
            return OperationResult.FromResult<bool>(await ActivityManager.UpdateActivityAsync(request));
        }

        /// <summary>
        /// 活动报名
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> AddActivitiesUserAsync(ActivityUserInfo_xhrRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_IsRequired, "UserName"));
            if (request.AreaID == 0)
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_IsRequired, "AreaID"));
            var regex = new Regex("^1[0-9]{10}$");
            if (string.IsNullOrWhiteSpace(request.UserTell) || !regex.IsMatch(request.UserTell))
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_NotConformityRule, "UserTell"));
            return OperationResult.FromResult<bool>(await ActivityManager.AddActivitiesUserAsync(request));
        }

        /// <summary>
        /// 修改报名信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> UpdateActivitiesUserAsync(ActivityUserInfo_xhrRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_IsRequired, "UserName"));
            if (request.AreaID == 0)
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_IsRequired, "AreaID"));
            var regex = new Regex("^1[0-9]{10}$");
            if (string.IsNullOrWhiteSpace(request.UserTell) || !regex.IsMatch(request.UserTell))
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_NotConformityRule, "UserTell"));
            return OperationResult.FromResult<bool>(await ActivityManager.UpdateActivitiesUserAsync(request));
        }


        /// <summary>
        /// 查询所有地区
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<T_ArearModel>>> GetAllAreaAsync() =>
            OperationResult.FromResult(await ActivityManager.GetAllAreaAsync());

        /// <summary>
        /// 根据地区查询用户信息
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<T_ActivityUserInfo_xhrModels>>> GetActivityUserInfoByAreaAsync(int areaId, int pageIndex, int pageSize) =>
            OperationResult.FromResult(await ActivityManager.GetActivityUserInfoByAreaAsync(areaId, pageIndex, pageSize));

        /// <summary>
        /// 审核活动
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<bool>> ReviewActivityTaskAsync() =>
            OperationResult.FromResult(await ActivityManager.ReviewActivityTaskAsync());


        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<string>> ManagerLoginAsync(T_ActivityManagerUserInfo_xhrModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return OperationResult.FromError<string>("-31", string.Format(Resource.ParameterError_IsRequired, "账号"));
            if (string.IsNullOrWhiteSpace(request.PassWords))
                return OperationResult.FromError<string>("-31", string.Format(Resource.ParameterError_IsRequired, "密码"));
            string passWordsSalt = ActivityManager.GetPassWordsSalt(request);
            if (string.IsNullOrWhiteSpace(passWordsSalt))
                return OperationResult.FromError<string>("-31", "用户名或密码不正确");
            request.PassWordsSalt = passWordsSalt;
            request.PassWords = HashSecurityHelper.Sha1Encrypt(request.PassWords + request.PassWordsSalt);
            return OperationResult.FromResult<string>(await ActivityManager.ManagerLoginAsync(request));
        }

        /// <summary>
        /// 管理员注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> ManagerRegisterAsync(T_ActivityManagerUserInfo_xhrModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_IsRequired, "账号"));
            if (string.IsNullOrWhiteSpace(request.PassWords))
                return OperationResult.FromError<bool>("-31", string.Format(Resource.ParameterError_IsRequired, "密码"));
            string passWordsSalt = ActivityManager.GetPassWordsSalt(request);
            if (!string.IsNullOrWhiteSpace(passWordsSalt))
                return OperationResult.FromError<bool>("-31", "用户名已存在");
            request.PassWordsSalt = Guid.NewGuid().ToString();
            request.PassWords = HashSecurityHelper.Sha1Encrypt(request.PassWords + request.PassWordsSalt);
            //if(request.PassWords.Length>0)
            //return OperationResult.FromError<bool>("-31", "密码"+ request.PassWords+"密码盐"+ request.PassWordsSalt);
            return OperationResult.FromResult<bool>(await ActivityManager.ManagerRegisterAsync(request));
        }

        /// <summary>
        /// 验证登录状态
        /// </summary>
        /// <param name="managerId"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> CheckLoginAsync(int managerId) =>
            OperationResult.FromResult<bool>(await ActivityManager.CheckLoginAsync(managerId));


        /// <summary>
        /// 获取所有活动
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<T_Activity_xhrModel>>> GetAllActivityManagerAsync(int pageIndex, int pageSize) =>
            OperationResult.FromResult(await ActivityManager.GetAllActivityManagerAsync(pageIndex, pageSize));
    }
}
