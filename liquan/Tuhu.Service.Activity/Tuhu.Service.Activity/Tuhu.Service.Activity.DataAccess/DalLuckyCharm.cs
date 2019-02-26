using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalLuckyCharm
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalLuckyCharm));

        /// <summary>
        /// 根据条件分页获取锦鲤活动的列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<List<LuckyCharmActivityModel>> PageActivity(PageLuckyCharmActivityRequest model)
        {
            //string sql = @"select top @pageSize * from (select row_number() over(order by PKID asc) as rownumber,*
            //from Activity.dbo.T_LuckyCharmUser WITH (NOLOCK) {0})temp_row where rownumber>10";

            string sql = @"select  * from  Activity.dbo.T_LuckyCharmActivity WITH (NOLOCK) {0} ORDER BY PKID DESC
                              OFFSET (@PageIndex - 1) * @PageSize ROW
				              FETCH NEXT @PageSize ROW ONLY";

            var parameters = new Dictionary<string, object>();
            if (model.PKID > 0)
            {
                parameters.Add("PKID", model.PKID);
            }
            StringBuilder condition = new StringBuilder("where IsDelete=0 ");
            foreach (var key in parameters.Keys)
            {
                condition.AppendFormat("and {0}=@{1} ", key, key);
            }
            using (var cmd = new SqlCommand(string.Format(sql, condition.ToString())))
            {
                foreach (var key in parameters.Keys)
                {
                    cmd.AddParameter(string.Format("@{0}", key), parameters[key]);
                }
                cmd.AddParameter("@PageSize", model.PageSize);
                cmd.AddParameter("@PageIndex", model.PageIndex);
                var result = await DbHelper.ExecuteSelectAsync<LuckyCharmActivityModel>(true, cmd);
                return result?.ToList();
            }
        }

        /// <summary>
        /// 根据条件获取活动总数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<int> TotalActivity(PageLuckyCharmActivityRequest model)
        {
            string sql = @"select count(1) from Activity.dbo.T_LuckyCharmActivity WITH (NOLOCK) {0}";

            var parameters = new Dictionary<string, object>();
            if (model.PKID > 0)
            {
                parameters.Add("PKID", model.PKID);
            }
            StringBuilder condition = new StringBuilder("where IsDelete=0 ");
            foreach (var key in parameters.Keys)
            {
                condition.AppendFormat("and {0}=@{1} ", key, key);
            }
            using (var cmd = new SqlCommand(string.Format(sql, condition.ToString())))
            {
                foreach (var key in parameters.Keys)
                {
                    cmd.AddParameter(string.Format("@{0}", key), parameters[key]);
                }
                int.TryParse((await DbHelper.ExecuteScalarAsync(true, cmd)).ToString(), out int result);
                return result;
            }
        }

        public static async Task<LuckyCharmActivityModel> GetActivityInfo(int pkid)
        {
            string sql = @"SELECT TOP 1 * FROM Activity.dbo.T_LuckyCharmActivity WITH (NOLOCK) WHERE PKID = @PKID and IsDelete=0";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return await DbHelper.ExecuteFetchAsync<LuckyCharmActivityModel>(true, cmd);
            }
        }

        public static async Task<int> AddActivityInfo(AddLuckyCharmActivityRequest model)
        {
            string sql = @"insert Activity.dbo.T_LuckyCharmActivity(ActivityType,StarTime,EndTime,ActivityTitle,ActivitySlug,ActivityDes)
VALUES(@ActivityType,@StarTime,@EndTime,@ActivityTitle,@ActivitySlug,@ActivityDes)";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ActivityType", model.ActivityType);
                cmd.Parameters.AddWithValue("@StarTime", model.StarTime);
                cmd.Parameters.AddWithValue("@EndTime", model.EndTime);
                cmd.Parameters.AddWithValue("@ActivityTitle", model.ActivityTitle);
                cmd.Parameters.AddWithValue("@ActivitySlug", model.ActivitySlug);
                cmd.Parameters.AddWithValue("@ActivityDes", model.ActivityDes);
                return await DbHelper.ExecuteNonQueryAsync(false, cmd);
            }
        }

        public static async Task<int> DeleteActivity(int PKID)
        {
            string sql = @"Update Activity.dbo.T_LuckyCharmActivity set IsDelete=1 WHERE PKID = @PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return await DbHelper.ExecuteNonQueryAsync(false, cmd);
            }
        }

        public static async Task<int> IsExistActivityByPKID(int PKID)
        {
            string sql = @"SELECT count(1) FROM Activity.dbo.T_LuckyCharmActivity WITH (NOLOCK) WHERE PKID = @PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKID", PKID);
                int.TryParse((await DbHelper.ExecuteScalarAsync(true, cmd)).ToString(), out int result);
                return result;
            }
        }

        public static async Task<int> AddActivityUser(AddLuckyCharmUserRequest model)
        {
            string sql = @"insert Activity.dbo.T_LuckyCharmUser (ActivityId,UserId,UserName,Phone,AreaId,AreaName)
VALUES(@ActivityId,@UserId,@UserName,@Phone,@AreaId,@AreaName)";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ActivityId", model.ActivityId);
                cmd.Parameters.AddWithValue("@UserId", model.UserId);
                cmd.Parameters.AddWithValue("@UserName", model.UserName);
                cmd.Parameters.AddWithValue("@Phone", model.Phone);
                cmd.Parameters.AddWithValue("@AreaId", model.AreaId);
                cmd.Parameters.AddWithValue("@AreaName", model.AreaName);
                return await DbHelper.ExecuteNonQueryAsync(false, cmd);
            }
        }

        public static async Task<int> UpdateActivityUser(UpdateLuckyCharmUserRequest model)
        {
            string sql = @"Update Activity.dbo.T_LuckyCharmUser set UserName=@UserName,Phone=@Phone,AreaId=@AreaId,AreaName=@AreaName,ActivityId=@ActivityId where PKID=@PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@UserName", model.UserName);
                cmd.Parameters.AddWithValue("@Phone", model.Phone);
                cmd.Parameters.AddWithValue("@AreaId", model.AreaId);
                cmd.Parameters.AddWithValue("@AreaName", model.AreaName);
                cmd.Parameters.AddWithValue("@PKID", model.PKID);
                cmd.Parameters.AddWithValue("@ActivityId", model.ActivityId);
                return await DbHelper.ExecuteNonQueryAsync(false, cmd);
            }
        }

        /// <summary>
        /// 审批用户-PKID
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<int> AuditActivityUser(int pkid)
        {
            string sql = @"Update Activity.dbo.T_LuckyCharmUser set CheckState=1  where PKID=@PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return await DbHelper.ExecuteNonQueryAsync(false, cmd);
            }
        }

        /// <summary>
        /// 审批用户--区域名称or区域ID
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<int> AuditActivityUser(string areaName)
        {
            string sql = @"Update Activity.dbo.T_LuckyCharmUser set CheckState=1  where AreaName=@AreaName";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@AreaName", areaName);
                return await DbHelper.ExecuteNonQueryAsync(false, cmd);
            }
        }

        /// <summary>
        /// 根据条件分页获取报名参加用户的列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<List<LuckyCharmUserModel>> PageActivityUser(PageLuckyCharmUserRequest model)
        {
            //string sql = @"select top @pageSize * from (select row_number() over(order by PKID asc) as rownumber,*
            //from Activity.dbo.T_LuckyCharmUser WITH (NOLOCK) {0})temp_row where rownumber>10";

            string sql = @"select  * from  Activity.dbo.T_LuckyCharmUser WITH (NOLOCK) {0} ORDER BY PKID DESC
                              OFFSET (@PageIndex - 1) * @PageSize ROW
				              FETCH NEXT @PageSize ROW ONLY";

            var parameters = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(model.AreaName))
            {
                parameters.Add("AreaName", model.AreaName);
            }
            if (model.PKID > 0)
            {
                parameters.Add("PKID", model.PKID);
            }
            if (!string.IsNullOrWhiteSpace(model.Phone))
            {
                parameters.Add("Phone", model.Phone);
            }
            if (model.CheckState >= 0)
            {
                parameters.Add("CheckState", model.CheckState);
            }
            StringBuilder condition = new StringBuilder("where IsDelete=0 ");
            foreach (var key in parameters.Keys)
            {
                condition.AppendFormat("and {0}=@{1} ", key, key);
            }
            using (var cmd = new SqlCommand(string.Format(sql, condition.ToString())))
            {
                foreach (var key in parameters.Keys)
                {
                    cmd.AddParameter(string.Format("@{0}", key), parameters[key]);
                }
                cmd.AddParameter("@PageSize", model.PageSize);
                cmd.AddParameter("@PageIndex", model.PageIndex);
                var result = await DbHelper.ExecuteSelectAsync<LuckyCharmUserModel>(true, cmd);
                return result?.ToList();
            }
        }

        /// <summary>
        /// 根据条件获取用户总数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<int> TotalActivityUser(PageLuckyCharmUserRequest model)
        {
            string sql = @"select count(1) from Activity.dbo.T_LuckyCharmUser WITH (NOLOCK) {0}";

            var parameters = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(model.AreaName))
            {
                parameters.Add("AreaName", model.AreaName);
            }
            if (model.PKID > 0)
            {
                parameters.Add("PKID", model.PKID);
            }
            if (!string.IsNullOrWhiteSpace(model.Phone))
            {
                parameters.Add("Phone", model.Phone);
            }
            if (model.CheckState >= 0)
            {
                parameters.Add("CheckState", model.CheckState);
            }
            StringBuilder condition = new StringBuilder("where IsDelete=0 ");
            foreach (var key in parameters.Keys)
            {
                condition.AppendFormat("and {0}=@{1} ", key, key);
            }
            using (var cmd = new SqlCommand(string.Format(sql, condition.ToString())))
            {
                foreach (var key in parameters.Keys)
                {
                    cmd.AddParameter(string.Format("@{0}", key), parameters[key]);
                }
                int.TryParse((await DbHelper.ExecuteScalarAsync(true, cmd)).ToString(), out int result);
                return result;
            }
        }

        public static async Task<int> IsExistUserByPhone(string phone)
        {
            string sql = @"SELECT count(1) FROM Activity.dbo.T_LuckyCharmUser WITH (NOLOCK) WHERE Phone = @Phone";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@Phone", phone);
                int.TryParse((await DbHelper.ExecuteScalarAsync(true, cmd)).ToString(), out int result);
                return result;
            }
        }

        public static async Task<int> IsExistUserByPKID(int PKID)
        {
            string sql = @"SELECT count(1) FROM Activity.dbo.T_LuckyCharmUser WITH (NOLOCK) WHERE PKID = @PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKID", PKID);
                int.TryParse((await DbHelper.ExecuteScalarAsync(true, cmd)).ToString(), out int result);
                return result;
            }
        }

        public static async Task<int> DeleteUser(int PKID)
        {
            string sql = @"Update Activity.dbo.T_LuckyCharmUser set IsDelete=1 WHERE PKID = @PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return await DbHelper.ExecuteNonQueryAsync(false, cmd);
            }
        }

    }
}
