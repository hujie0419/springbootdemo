using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.UserAuthJob.Model;

namespace Tuhu.C.Job.UserAuthJob.DAL
{
    public static partial class UserProfileInitializationDal
    {
        /// <summary>
        /// 查询未初始化的用户属性的用户数
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public static async Task<int> SelectUnInitUserCount(string profileName)
        {
            using (var cmd = new SqlCommand(Select_AllRefreshUserCNT))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@profileName", profileName);
                cmd.CommandTimeout = 60 * 5; // 设置超时时间为5分钟
                var result =await DbHelper.ExecuteScalarAsync(true, cmd);
                int.TryParse(result?.ToString(), out int cnt);
                return cnt;
            }
        }

        public static async Task<IEnumerable<UserProfileWithValue>> SelectUserProfileWithPageOrderQTY(int pageSize,int offset)
        {
            using (var cmd = new SqlCommand(Select_UserProfileWithPage_OrderQTY))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.Parameters.AddWithValue("@Offset", offset);
                cmd.CommandTimeout = 60 * 5; // 设置超时时间为5分钟
                return await DbHelper.ExecuteSelectAsync<UserProfileWithValue>(cmd);
            }
        }

        /// <summary>
        /// 查询未初始化的用户属性的用户数
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public static async Task<int> SelectUnInitUserWithDateCount(DateTime beginDate, DateTime endDate)
        {
            using (var cmd = new SqlCommand(Select_AllRefreshUserCNTWithDate))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@BeginDate", beginDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);
                cmd.CommandTimeout = 60 * 5; // 设置超时时间为5分钟
                var result = await DbHelper.ExecuteScalarAsync(true, cmd);
                int.TryParse(result?.ToString(), out int cnt);
                return cnt;
            }
        }

        public static async Task<IEnumerable<UserProfileWithValue>> SelectUserProfileWithDateQTY(int pageSize, int offset, DateTime beginDate, DateTime endDate)
        {
            using (var cmd = new SqlCommand(Select_UserProfileWithPage_OrderQTY_Date))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@BeginDate", beginDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.Parameters.AddWithValue("@Offset", offset);
                cmd.CommandTimeout = 60 * 5; // 设置超时时间为5分钟
                return await DbHelper.ExecuteSelectAsync<UserProfileWithValue>(cmd);
            }
        }
        
    }
}
