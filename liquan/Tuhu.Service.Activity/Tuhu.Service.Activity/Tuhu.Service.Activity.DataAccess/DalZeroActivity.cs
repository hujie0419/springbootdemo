using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tuhu.Service.Activity.DataAccess.Models;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.DataAccess
{
    public partial class DalZeroActivity
    {
        public static async Task<int> FetchNumOfApplicationsAsync(int period)
        {
            using (var cmd = new SqlCommand(SelectNumOfApplications))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Period", period);
                return Convert.ToInt32(await DbHelper.ExecuteScalarAsync(!(await RedisHelper.GetZeroActivityApplyCacheOnPeriod(period)), cmd));
            }
        }

        /// <summary>
        /// 获取 所有 未过期的 的 众测配置【包括 未开始 和 正在进行 】  顺序
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<ZeroActivityModel>> SelectUnfinishedZeroActivitiesForHomepageAsync()
        {
            using (var cmd = new SqlCommand(SelectUnfinishedZeroActivitiesForHomepage))
            {
                cmd.CommandType = CommandType.Text;
                return await DbHelper.ExecuteSelectAsync<ZeroActivityModel>(true, cmd);
            }
        }

        public static async Task<IEnumerable<ZeroActivityModel>> SelectFinishedZeroActivitiesForHomepageAsync(int pageNumber)
        {
            using (var cmd = new SqlCommand(SelectFinishedZeroActivitiesForHomepage))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                return await DbHelper.ExecuteSelectAsync<ZeroActivityModel>(true, cmd);
            }
        }

        public static async Task<ZeroActivityDetailModel> FetchZeroActivityDetailAsync(int period)
        {
            using (var cmd = new SqlCommand(FetchZeroActivityDetail))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Period", period);
                return await DbHelper.ExecuteFetchAsync<ZeroActivityDetailModel>(true, cmd);
            }
        }

        public static async Task<bool> HasZeroActivityApplicationSubmittedAsync(Guid userId, int period)
        {
            using (var cmd = new SqlCommand(NumOfZeroActivityApplications))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@Period", period);
                return Convert.ToInt32(await DbHelper.ExecuteScalarAsync(!(await RedisHelper.GetZeroActivityApplyCacheOnPeriod(period)) || !(await RedisHelper.GetZeroActivityApplyCacheOnUserId(userId)), cmd)) > 0;
            }
        }

        public static async Task<bool> HasZeroActivityReminderSubmittedAsync(Guid userId, int period)
        {
            using (var dbhelper = DbHelper.CreateLogDbHelper(!(await RedisHelper.GetZeroActivityReminderCache(userId, period))))
            {
                using (var cmd = new SqlCommand(NumOfZeroActivityReminders))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Period", period);
                    return Convert.ToInt32(await dbhelper.ExecuteScalarAsync(cmd)) > 0;
                }
            }
        }

        public static async Task<IEnumerable<SelectedTestReport>> SelectChosenUserReportsAsync(int period)
        {
            using (var cmd = new SqlCommand(SelectChosenUserReports))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Period", period);
                return await DbHelper.ExecuteQueryAsync(!(await RedisHelper.GetZeroActivityApplyCacheOnPeriod(period)), cmd, (dt) =>
                {
                    var result = new List<SelectedTestReport>();
                    if (dt?.Rows?.OfType<DataRow>() == null || !dt.Rows.OfType<DataRow>().Any())
                    {
                        return result;
                    }
                    foreach (var dr in dt.Rows.OfType<DataRow>())
                    {
                        if (dr != null)
                        {
                            var testReport = new SelectedTestReport();
                            if (!(dr["UserID"] is DBNull))
                                testReport.UserId = new Guid(dr["UserID"].ToString());

                            if (!(dr["CommentStatus"] is DBNull) && Convert.ToInt32(dr["CommentStatus"]) == 2 && !(dr["CommentType"] is DBNull) && Convert.ToInt32(dr["CommentType"]) == 3)
                            {
                                if (!(dr["CommentId"] is DBNull))
                                    testReport.CommentId = Convert.ToInt32(dr["CommentId"]);
                                testReport.ReportTitle = dr["SingleTitle"]?.ToString();
                                testReport.ReportAbstract = (dr["CommentContent"]?.ToString() != null && dr["CommentContent"].ToString().Length > 100)
                                                               ? dr["CommentContent"].ToString().Substring(0, 100) : dr["CommentContent"]?.ToString();
                                if (!(dr["CreateTime"] is DBNull))
                                    testReport.ReportCreateTime = Convert.ToDateTime(dr["CreateTime"]);
                                if (!(dr["CommentImages"] is DBNull) && !string.IsNullOrWhiteSpace(dr["CommentImages"].ToString()))
                                    testReport.ReportImages = dr["CommentImages"].ToString().Split(';');
                            }
                            result.Add(testReport);
                        }
                    }
                    return result.GroupBy(_ => _.UserId).Select(_ => _.OrderByDescending(ur => ur.CommentId == null ? 0 : 1).FirstOrDefault()).Where(_ => _ != null).ToList();
                });
            }
        }

        public static async Task<SelectedTestReportDetail> FetchTestReportDetailAsync(int commentId)
        {
            using (var cmd = new SqlCommand(FetchTestReportDetail))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@CommentId", commentId);
                return await DbHelper.ExecuteQueryAsync(!(await RedisHelper.GetZeroActivityApplyCache()), cmd, (dt) =>
                {
                    if (dt?.Rows?.OfType<DataRow>() == null || !dt.Rows.OfType<DataRow>().Any())
                    {
                        return null;
                    }
                    var dr = dt.Rows.OfType<DataRow>().First();
                    var result = new SelectedTestReportDetail();

                    result.CommentId = Convert.ToInt32(dr["CommentId"]);
                    if (!(dr["UserID"] is DBNull))
                        result.UserId = Guid.Parse(dr["UserID"].ToString());
                    result.ReportTitle = dr["SingleTitle"]?.ToString();
                    result.ReportContent = dr["CommentContent"]?.ToString();
                    result.ReportAbstract = (result.ReportContent != null && result.ReportContent.Length > 100)
                                            ? result.ReportContent.Substring(0, 100) : result.ReportContent;
                    if (!(dr["CreateTime"] is DBNull))
                        result.ReportCreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    if (!(dr["Period"] is DBNull))
                        result.Period = Convert.ToInt32(dr["Period"]);
                    result.ProvinceID = Convert.ToInt32(dr["ProvinceID"]);
                    result.CityID = Convert.ToInt32(dr["CityID"]);
                    if (!(dr["UpdateTime"] is DBNull))
                        result.ReportUpdateTime = Convert.ToDateTime(dr["UpdateTime"]);
                    result.ProductId = dr["CommentProductId"]?.ToString();
                    result.ProductFamilyId = dr["CommentProductFamilyId"]?.ToString();
                    if (!(dr["CommentOrderId"] is DBNull))
                        result.OrderId = Convert.ToInt32(dr["CommentOrderId"]);
                    if (!(dr["CommentOrderListId"] is DBNull))
                        result.OrderListId = Convert.ToInt32(dr["CommentOrderListId"]);
                    if (!(dr["CommentStatus"] is DBNull))
                        result.CommentStatus = Convert.ToInt32(dr["CommentStatus"]);
                    if (!(dr["ReportStatus"] is DBNull))
                        result.ReportStatus = Convert.ToInt32(dr["ReportStatus"]);
                    result.OfficialReply = dr["OfficialReply"]?.ToString();
                    if (!(dr["CommentR1"] is DBNull))
                        result.Comfortability = Convert.ToInt32(dr["CommentR1"]);
                    if (!(dr["CommentR2"] is DBNull))
                        result.MutePerformance = Convert.ToInt32(dr["CommentR2"]);
                    if (!(dr["CommentR3"] is DBNull))
                        result.Controllability = Convert.ToInt32(dr["CommentR3"]);
                    if (!(dr["CommentR4"] is DBNull))
                        result.AbrasionPerformance = Convert.ToInt32(dr["CommentR4"]);
                    if (!(dr["CommentR5"] is DBNull))
                        result.OilSaving = Convert.ToInt32(dr["CommentR5"]);
                    if (!(dr["CommentR6"] is DBNull))
                        result.CustomServiceSatisfaction = Convert.ToInt32(dr["CommentR6"]);
                    if (!(dr["CommentR7"] is DBNull))
                        result.ShopSatisfaction = Convert.ToInt32(dr["CommentR7"]);
                    if (!(dr["CommentImages"] is DBNull) && !string.IsNullOrWhiteSpace(dr["CommentImages"].ToString()))
                        result.ReportImages = dr["CommentImages"].ToString().Split(';');
                    if (!(dr["CommentExtAttr"] is DBNull) && !string.IsNullOrWhiteSpace(dr["CommentExtAttr"].ToString()))
                        result.TestReportExtenstionAttribute = JsonConvert.DeserializeObject<CommentExtenstionAttribute>(dr["CommentExtAttr"].ToString());
                    return result;
                });
            }
        }

        public static async Task<IEnumerable<MyZeroActivityApplications>> SelectMyApplicationsAsync(Guid userId, int applicationStatus)
        {
            using (var cmd = new SqlCommand(SelectMyApplications))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@ApplicationStatus", applicationStatus);

                return await DbHelper.ExecuteSelectAsync<MyZeroActivityApplications>(!(await RedisHelper.GetZeroActivityApplyCacheOnUserId(userId)), cmd);
            }
        }

        public static async Task<int> SubmitZeroActivityApplicationAsync(ZeroActivityRequest requestModel, ZeroActivityDetailModel activityDetail, string userMobile)
        {
            using (var cmd = new SqlCommand(InsertZeroActivityApplication))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Period", requestModel.Period);
                cmd.Parameters.AddWithValue("@UserID", requestModel.UserId);
                cmd.Parameters.AddWithValue("@UserName", requestModel.UserName);
                cmd.Parameters.AddWithValue("@PID", activityDetail.PID);
                cmd.Parameters.AddWithValue("@ProductName", activityDetail.ProductName);
                cmd.Parameters.AddWithValue("@Quantity", (activityDetail.NumOfWinners == 0 ? 0 : activityDetail.Quantity / activityDetail.NumOfWinners));
                cmd.Parameters.AddWithValue("@ProvinceID", requestModel.ProvinceID);
                cmd.Parameters.AddWithValue("@CityID", requestModel.CityID);
                cmd.Parameters.AddWithValue("@ApplyReason", requestModel.ApplicationReason);
                cmd.Parameters.AddWithValue("@CarID", requestModel.CarID);
                cmd.Parameters.AddWithValue("@Mobile", userMobile);
                var count = Convert.ToInt32(await DbHelper.ExecuteScalarAsync(cmd));
                if (count > 0)
                {
                    await RedisHelper.CreateZeroActivityApplyCache();
                    await RedisHelper.CreateZeroActivityApplyCacheOnPeriod(requestModel.Period);
                    await RedisHelper.CreateZeroActivityApplyCacheOnUserId(requestModel.UserId);
                }
                return count;
            }
        }

        public static async Task<string> FetchStartTimeOfZeroActivityAsync(int period)
        {
            using (var cmd = new SqlCommand(FetchStartTimeOfZeroActivity))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Period", period);
                return (await DbHelper.ExecuteScalarAsync(true, cmd))?.ToString();
            }
        }

        public static async Task<int> SubmitZeroActivityReminderAsync(Guid userId, int period)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(InsertZeroActivityReminder))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Period", period);
                    var count = Convert.ToInt32(await dbHelper.ExecuteScalarAsync(cmd));
                    if (count > 0)
                        await RedisHelper.CreateZeroActivityReminderCache(userId, period);
                    return count;
                }
            }
        }

        public static async Task<IEnumerable<DalZeroActivityModel>> SelectZeroActivitySimpleModelAsync()
        {
            string sql = @"SELECT top 300
        pid,tza.Period,tza.StartDateTime,tza.EndDateTime,tza.Quantity,SucceedQuota
        FROM Activity..tbl_ZeroActivity AS tza WITH(NOLOCK)
        order by Period desc, EndDateTime desc; ";
            
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                return await DbHelper.ExecuteSelectAsync<DalZeroActivityModel>(true, cmd);
            }

        }
    }
}
