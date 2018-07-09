using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.C.Job.UserAuthJob.DAL
{
    public static partial class WxUserAuthRefreshDal
    {
        /// <summary>
        /// 获取所有需要刷新token的Auth信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<UserAuth> GetAllRefreshUserAuth(int expainDay)
        {
            using (var cmd = new SqlCommand(Sql_Select_AllRefalshUser))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ExpanDay", expainDay);
                cmd.CommandTimeout = 60 * 5; // 设置超时时间为5分钟
                return DbHelper.ExecuteSelect<UserAuth>(true, cmd);
            }
        }

        public static async Task<bool> UpdateRefreshToken(RefreshOAuthTokenKey refreshOAuth)
        {
            using (var helper = DbHelper.CreateDbHelper())
            using (var cmd = new SqlCommand(Sql_Update_UserAuthToken))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 60 * 5; // 设置超时时间为5分钟

                cmd.Parameters.AddWithValue("@AccessToken", refreshOAuth?.AccessTokey);
                cmd.Parameters.AddWithValue("@AccessTokenExpire", ChangeExpireTime(refreshOAuth?.ExpriesTime, DateTime.Now));
                cmd.Parameters.AddWithValue("@NewRefreshToken", refreshOAuth?.RefreshTokey);
                cmd.Parameters.AddWithValue("@RefreshTokenExpire", ChangeExpireTime(refreshOAuth?.RefreshExpriesTime, DateTime.Now));
                cmd.Parameters.AddWithValue("@MetaData", refreshOAuth.MetaData);
                cmd.Parameters.AddWithValue("@RefreshStatus", refreshOAuth.RefreshStatus);
                cmd.Parameters.AddWithValue("@OldRefreshToken", refreshOAuth.oldRefreshTokey);

                cmd.Parameters.AddWithValue("@AuthorizationStatus", refreshOAuth?.AuthorizationStatus?.ToString());

                return await helper.ExecuteNonQueryAsync(cmd) > 0;
            }
        }
        public static DateTime? ChangeExpireTime(string expireTime, DateTime? startTime = null)
        {
            var miliS = 0d;
            if (double.TryParse(expireTime, out miliS))
            {
                return (startTime ?? DateTime.Now).AddMilliseconds(miliS);
            }
            return null;
        }
    }
}
