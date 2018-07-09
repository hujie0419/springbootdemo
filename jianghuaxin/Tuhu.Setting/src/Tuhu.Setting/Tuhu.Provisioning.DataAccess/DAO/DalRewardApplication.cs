using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalRewardApplication
    {
        public static IEnumerable<RewardApplicationModel> SelectRewardApplicationModels(int pageIndex, int pageSize, int applicationState, string applicationName, string phone, DateTime? createDateTime)
        {
            var sql = @"SELECT  *,COUNT(1) OVER ( ) AS TotalCount FROM  Configuration..UserRewardApplication AS FS Where 1=1";
            if (applicationState != 4)
            {
                sql += $@"AND ApplicationState={applicationState}";
            }
            if (!string.IsNullOrEmpty(applicationName))
            {
                sql += $@"AND ApplicationName like N'%{applicationName}%'";
            }
            if (!string.IsNullOrEmpty(phone))
            {
                sql += $@"AND Phone ='{phone}'";
            }
            if (createDateTime.HasValue)
            {
                sql += $@"AND CreateDateTime >= CONVERT(VARCHAR(100), CONVERT(DATE, '{createDateTime.Value}'), 23) AND 
                        CreateDateTime <= CONVERT(VARCHAR(100), CONVERT(DATE, '{createDateTime.Value.AddDays(1)}'), 23)";
            }
            sql += $@"ORDER BY Pkid
                        OFFSET({pageIndex} - 1) * {pageSize} ROWS FETCH NEXT {pageSize}
                        ROWS ONLY";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(sql).ConvertTo<RewardApplicationModel>();
            }
        }
        public static bool SaveRewardApplicationModels(string phone, int state, string user)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sql = @"
                       UPDATE  Configuration..UserRewardApplication WITH(ROWLOCK) SET ApplicationState=@ApplicationState,Auditor=@Auditor,LastUpdateDateTime=getdate() WHERE Phone=@Phone";
                return Convert.ToInt32(dbHelper.ExecuteNonQuery(sql, CommandType.Text, new SqlParameter[]
                    {
                        new SqlParameter("@ApplicationState", state),
                        new SqlParameter("@Phone",phone),
                        new SqlParameter("@Auditor",user)
                    })) > 0;

            }
        }

        /// <summary>
        /// todo  按照晒许查询下一个
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="applicationName"></param>
        /// <param name="createDateTime"></param>
        /// <returns></returns>
        public static RewardApplicationModel FetchRewardApplicationModel(string phone, string[] phones, string applicationName, DateTime? createDateTime)
        {
            var sql = $@"
                       	SELECT TOP 1 * FROM  Configuration..UserRewardApplication WITH(NOLOCK)  WHERE Phone <> @Phone and Phone in (SELECT  *
                                                                                                             FROM    Configuration.dbo.Split(@Phones, ',') ) AND ApplicationState=1 order by pkid";
            //if (!string.IsNullOrEmpty(applicationName))
            //{
            //    sql += $@"AND ApplicationName like N'%{applicationName}%'";
            //}
            //if (createDateTime.HasValue)
            //{
            //    sql += $@"AND CreateDateTime >= CONVERT(VARCHAR(100), CONVERT(DATE, '{createDateTime.Value}'), 23) AND 
            //            CreateDateTime <= CONVERT(VARCHAR(100), CONVERT(DATE, '{createDateTime.Value.AddDays(1)}'), 23)";
            //}
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {

                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new[]
                {
                        new SqlParameter("@Phones", string.Join(",",phones)),
                        new SqlParameter("@Phone",phone)
                }).ConvertTo<RewardApplicationModel>().FirstOrDefault();
            }
        }

        public static RewardApplicationModel FetchNextOrPreRewardApplicationModel(string phone)
        {
            var sql = $@"
                       	SELECT TOP 1 * FROM  Configuration..UserRewardApplication WITH(NOLOCK)  WHERE Phone = {phone}";

            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {

                return dbHelper.ExecuteDataTable(sql, CommandType.Text).ConvertTo<RewardApplicationModel>().FirstOrDefault();
            }
        }

    }
}
