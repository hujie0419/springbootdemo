using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Nosql;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.Server.Manager
{
    public static class LuckyCharmManager
    {
        #region 锦鲤活动

        public static async Task<OperationResult<PageLuckyCharmActivityResponse>> PageActivityActivityAsync(PageLuckyCharmActivityRequest model)
        {
            if (model.PageIndex < 1)
            {
                model.PageIndex = 1;
            }
            if (model.PageSize <= 0)
            {
                model.PageSize = 10;
            }
            var act = await DalLuckyCharm.PageActivity(model);
            var total = await DalLuckyCharm.TotalActivity(model);
            var result = new PageLuckyCharmActivityResponse();
            result.Activitys = Mapper.Map<List<LuckyCharmActivityInfoResponse>>(act);
            result.Total = total;
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 活动详细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<LuckyCharmActivityInfoResponse>> GetActivityAsync(int pkid)
        {
            if (pkid <= 0)
            {
                return OperationResult.FromError<LuckyCharmActivityInfoResponse>("1", "参数错误");
            }
            using (var cacheClient = CacheHelper.CreateCacheClient())
            {
                var model = await cacheClient.GetOrSetAsync<LuckyCharmActivityModel>(string.Format(GlobalConstant.LuckyCharmActivityKeyString,pkid),
                     async () => await DalLuckyCharm.GetActivityInfo(pkid), TimeSpan.FromSeconds(30));

                //var model = await DalLuckyCharm.GetActivityInfo(pkid);
                if (model == null || model.Value == null || model.Value.PKID <= 0)
                {
                    return OperationResult.FromError<LuckyCharmActivityInfoResponse>("1", "活动不存在");
                }
                var info = Mapper.Map<LuckyCharmActivityInfoResponse>(model.Value);
                return OperationResult.FromResult(info);
            }
        }

        /// <summary>
        /// 新增活动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> AddActivityAsync(AddLuckyCharmActivityRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.ActivityTitle) || model.ActivityType < 0)// || model.ActivityTitle.Length > 50 || model.ActivitySlug.Length > 25 
            {
                return OperationResult.FromError<bool>("1", "参数错误");
            }
            if (model.StarTime == null || model.StarTime == new DateTime())
            {
                return OperationResult.FromError<bool>("1", "请传入活动开始时间");
            }
            if (model.EndTime == null || model.EndTime == new DateTime())
            {
                return OperationResult.FromError<bool>("1", "请传入活动结束时间");
            }

            return OperationResult.FromResult(await DalLuckyCharm.AddActivityInfo(model) > 0);
        }

        /// <summary>
        ///删除活动
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> DelActivityAsync(int pkid)
        {
            if (pkid <= 0)
            {
                return OperationResult.FromError<bool>("1", "参数错误");
            }
            var isExist = (await DalLuckyCharm.IsExistUserByPKID(pkid)) > 0;
            if (!isExist)
            {
                return OperationResult.FromError<bool>("3", "活动不存在");
            }
            return OperationResult.FromResult(await DalLuckyCharm.DeleteActivity(pkid) > 0);
        }
        #endregion

        #region 锦鲤活动报名用户

        public static async Task<OperationResult<PageLuckyCharmUserResponse>> PageActivityUserAsync(PageLuckyCharmUserRequest model)
        {
            if (model.PageIndex < 1)
            {
                model.PageIndex = 1;
            }
            if (model.PageSize <= 0)
            {
                model.PageSize = 10;
            }
            var user = await DalLuckyCharm.PageActivityUser(model);
            var total = await DalLuckyCharm.TotalActivityUser(model);
            var result = new PageLuckyCharmUserResponse();
            result.Users = Mapper.Map<List<LuckyCharmUserInfoRespone>>(user);
            result.Total = total;
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 新增报名用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> AddActivityUserAsync(AddLuckyCharmUserRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.AreaName) || string.IsNullOrWhiteSpace(model.UserName) || model.Phone.Length != 11)
            {
                return OperationResult.FromError<bool>("1", "参数错误");
            }
            var isUserExist = await DalLuckyCharm.IsExistUserByPhone(model.Phone);
            var isActivityExist = await DalLuckyCharm.IsExistActivityByPKID(model.ActivityId);
            
            if (isUserExist>0)
            {
                return OperationResult.FromError<bool>("2", "用户已报名");
            }
            if (isActivityExist <= 0)
            {
                return OperationResult.FromError<bool>("1", "活动不存在");
            }

            return OperationResult.FromResult(await DalLuckyCharm.AddActivityUser(model) > 0);
        }

        /// <summary>
        /// 修改报名用户资料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> UpdateActivityUserAsync(UpdateLuckyCharmUserRequest model)
        {
            if (model.PKID <= 0 || string.IsNullOrWhiteSpace(model.AreaName) || string.IsNullOrWhiteSpace(model.UserName) || model.Phone.Length != 11)
            {
                return OperationResult.FromError<bool>("1", "参数错误");
            }
            var isUserExist = await DalLuckyCharm.IsExistUserByPKID(model.PKID);
            var isActivityExist = await DalLuckyCharm.IsExistActivityByPKID(model.ActivityId);
            if (isUserExist > 0)
            {
                return OperationResult.FromError<bool>("2", "用户已报名");
            }
            if (isActivityExist <= 0)
            {
                return OperationResult.FromError<bool>("1", "活动不存在");
            }

            return OperationResult.FromResult(await DalLuckyCharm.UpdateActivityUser(model) > 0);
        }

        /// <summary>
        /// 审核用户报名状态
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="auditUserName"></param>
        /// <param name="auditStatus"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> AuditActivityStatusAsync(int pkid)
        {
            if (pkid <= 0)
            {
                return OperationResult.FromError<bool>("1", "参数错误");
            }
            var isExist = (await DalLuckyCharm.IsExistUserByPKID(pkid)) > 0;
            if (!isExist)
            {
                return OperationResult.FromError<bool>("3", "用户不存在");
            }
            return OperationResult.FromResult(await DalLuckyCharm.AuditActivityUser(pkid) > 0);
        }

        /// <summary>
        ///删除活动用户
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> DelUserAsync(int pkid)
        {
            if (pkid <= 0)
            {
                return OperationResult.FromError<bool>("1", "参数错误");
            }
            var isExist = (await DalLuckyCharm.IsExistUserByPKID(pkid)) > 0;
            if (!isExist)
            {
                return OperationResult.FromError<bool>("3", "活动不存在");
            }
            return OperationResult.FromResult(await DalLuckyCharm.DeleteUser(pkid) > 0);
        }

        #endregion

    }


    public static class LuckyCharRedis
    {

    }
}
