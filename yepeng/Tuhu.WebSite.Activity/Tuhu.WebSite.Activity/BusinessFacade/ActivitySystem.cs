using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Tuhu.Nosql;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Enums;
using Tuhu.Service.UserAccount.Models;
using Tuhu.WebSite.Web.Activity.Models;
using Tuhu.WebSite.Web.Activity.Models.Activity;
using Tuhu.WebSite.Web.Activity.Models.Enum;

namespace Tuhu.WebSite.Web.Activity.BusinessFacade
{
    public class ActivitySystem
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ActivitySystem));
        public static IEnumerable<BatteryBanner> GetBatteryBanner()
        {
            var dt = DataAccess.Activity.GetBatteryBanner();
            if (dt == null || dt.Rows.Count == 0)
            {
                return new BatteryBanner[0];
            }
            return dt.Rows.Cast<DataRow>().Select(x => new BatteryBanner(x)).ToList();
        }
        public static IEnumerable<ActivityBuild> GetActivityBuildById(int id)
        {
            var dt = Tuhu.WebSite.Web.Activity.DataAccess.Activity.GetActivityBuildById(id);
            if (dt == null || dt.Rows.Count == 0)
            {
                return new ActivityBuild[0];
            }
            return dt.Rows.Cast<DataRow>().Select(x => new ActivityBuild(x)).ToList();
        }

        public static IEnumerable<DownloadApp> GeDownloadAppById(int id)
        {
            var dt = Tuhu.WebSite.Web.Activity.DataAccess.Activity.GeDownloadAppById(id);
            if (dt == null || dt.Rows.Count == 0)
            {
                return new DownloadApp[0];
            }
            return dt.Rows.Cast<DataRow>().Select(x => new DownloadApp(x)).ToList();
        }
        public static CouponActivity GetCurrentCouponActivity()
        {
            return Tuhu.WebSite.Web.Activity.DataAccess.Activity.GetCurrentCouponActivity();
        }

        public static bool InsertPromotionFromActivity(CouponActivity couponDetail, string user)
        {
            int rows = Tuhu.WebSite.Web.Activity.DataAccess.Activity.InsertPromotionFromActivity(couponDetail, user);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool UserIsClaimedCoupon(CouponActivity couponDetail, string userId)
        {
            var result = Tuhu.WebSite.Web.Activity.DataAccess.Activity.UserIsClaimedCoupon(couponDetail, userId);
            if (result == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取活动列表
        /// </summary>
        /// <returns></returns>
        public static async Task<Tuple<IEnumerable<ActivityModel>, int>> GetActivityModelsAsync(int pageIndex, int pageSize)
        {
            var dt = await DataAccess.Activity.GetActivityModelsAsync(pageIndex, pageSize);
            if (dt == null || dt.Item1.Rows.Count == 0)
            {
                return new Tuple<IEnumerable<ActivityModel>, int>(new List<ActivityModel>(), 0);
            }
            //return dt.Rows.Cast<DataRow>().ParallelSelect(async i => await Task.FromResult(new ActivityModel(i)));
            var result = dt.Item1.Rows.Cast<DataRow>().Select(i => new ActivityModel(i)).ToList();
            return new Tuple<IEnumerable<ActivityModel>, int>(result, dt.Item2);
        }

        /// <summary>
        /// 根据activityId获取活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<ActivityModel> GetActivityModelByActivityIdAsync(Guid activityId)
        {
            DataTable dt = null;
            using (var client = CacheHelper.CreateCacheClient())
            {
                var result = await client.GetOrSetAsync($"ActivityModelCache/{activityId}",
                    async () => await DataAccess.Activity.GetActivityModelByActivityIdAsync(activityId), TimeSpan.FromHours(2));
                if (result != null && !result.Success && result.Exception != null)
                {
                    Logger.Error($"缓存设置失败。Message:{result.Exception}");
                }
                else
                {
                    dt = result?.Value;
                }
            }
            if (dt == null || dt.Rows.Count == 0)
            {
                return new ActivityModel();
            }
            return dt.Rows.Cast<DataRow>().Select(i => new ActivityModel(i)).FirstOrDefault();
        }
        /// <summary>
        /// 创建活动
        /// </summary>
        /// <param name="activityModel"></param>
        /// <returns></returns>
        public static async Task<int> CreateActivityAsync(ActivityModel activityModel)
        {
            return await DataAccess.Activity.InsertActivityModelAsync(activityModel);
        }

        /// <summary>
        /// 更新活动
        /// </summary>
        /// <param name="activityModel"></param>
        /// <returns></returns>
        public static async Task<int> UpdateActivityModelAsync(ActivityModel activityModel)
        {
            return await DataAccess.Activity.UpdateActivityModelAsync(activityModel);
        }

        /// <summary>
        /// 删除活动
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteActivityModelByPKIDAsync(int pkid)
        {
            return await DataAccess.Activity.DeleteActivityModelByPKIDAsync(pkid);
        }
        /// <summary>
        /// 用户报名
        /// </summary>
        /// <param name="userActivityModel"></param>
        /// <returns></returns>
        public static async Task<bool> CreateUserActivityModelAsync(UserActivityModel userActivityModel)
        {
            return await DataAccess.Activity.InsertUserActivityModelAsync(userActivityModel);
        }

        /// <summary>
        /// 审核用户报名活动
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> AuditUserActivityStatusByPKIDAsync(UserActivityModel userActivityModel)
        {
            return await DataAccess.Activity.UpdateUserActivityStatusByPKIDAsync(userActivityModel);
        }
        /// <summary>
        /// 查询或者创建用户
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="userName"></param>
        /// <param name="nickName"></param>
        /// <param name="headUrl"></param>
        /// <returns></returns>
        public static User GetOrCreateUser(string mobile, string userName = null, string nickName = null, string headUrl = null)
        {
            User user = null;
            if (!string.IsNullOrEmpty(mobile))
            {
                using (var client = new UserAccountClient())
                {
                    var result = client.GetUserByMobile(mobile);
                    result.ThrowIfException(true);
                    user = result.Result;
                    if (user == null)
                    {
                        var createResult = client.CreateUserRequest(
                            new CreateUserRequest
                            {
                                MobileNumber = mobile,
                                Profile = new UserProfile
                                {
                                    UserName = userName,
                                    NickName = nickName,
                                    HeadUrl = headUrl
                                },
                                ChannelIn = nameof(ChannelIn.None),
                                UserCategoryIn = nameof(UserCategory.Tuhu),
                            });
                        createResult.ThrowIfException(true);
                        user = createResult.Result;
                    }
                }
            }

            return user;
        }

        /// <summary>
        /// 分页获取用户报名列表
        /// </summary>
        /// <returns></returns>
        public static async Task<Tuple<IEnumerable<UserActivityModel>, int>> GetUserActivityModelsAsync(Guid activityId, AuditStatus auditStatus,int pageIndex, int pageSize)
        {
            var dt = await DataAccess.Activity.GetUserActivityModelsAsync(activityId, auditStatus,pageIndex, pageSize);
            if (dt == null || dt.Item1.Rows.Count == 0)
            {
                return new Tuple<IEnumerable<UserActivityModel>, int>(new List<UserActivityModel>(), 0);
            }
            var result = dt.Item1.Rows.Cast<DataRow>().Select(i => new UserActivityModel(i)).ToList();
            return new Tuple<IEnumerable<UserActivityModel>, int>(result, dt.Item2);
        }

        /// <summary>
        /// 根据活动id获取报名人员数量
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<int> GetUserActivityByActivityIdAsync(Guid activityId)
        {
            return await DataAccess.Activity.GetActivityApplyUserCountByActivityIdAsync(activityId);
        }

        /// <summary>
        /// 根据活动id获取报名人员审核通过数量
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<int> GetActivityApplyUserPassCountByActivityIdAsync(Guid activityId)
        {
            return await DataAccess.Activity.GetActivityApplyUserPassCountByActivityIdAsync(activityId);
        }
        /// <summary>
        /// 根据pkid获取报名人员
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<UserActivityModel> GetUserActivityByPKIDAsync(int pkid)
        {
            var dt = await DataAccess.Activity.GetUserActivityByPKIDAsync(pkid);
            if (dt == null || dt.Rows.Count == 0)
            {
                return new UserActivityModel();
            }
            return dt.Rows.Cast<DataRow>().Select(i => new UserActivityModel(i)).FirstOrDefault();
        }
        /// <summary>
        /// 删除用户报名
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteUserActivityModelByPKIDAsync(int pkid)
        {
            return await DataAccess.Activity.DeleteUserActivityModelByPKIDAsync(pkid);
        }

        /// <summary>
        /// 检查用户报名活动手机号、车牌号、驾驶证号是否重复
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="carNum"></param>
        /// <param name="driverNum"></param>
        /// <returns></returns>
        public static async Task<bool> CheckUserActivityDupAsync(string mobile, string carNum, string driverNum)
        {
            return await DataAccess.Activity.CheckUserActivityDupAsync(mobile, carNum, driverNum);
        }
    }
}