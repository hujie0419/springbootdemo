using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Tuhu.C.Job.UserShareJob.Model;

namespace Tuhu.C.Job.UserShareJob.Dal
{
    public class DalExpiringIntegral
    {
        #region [过期积分统计]

        public static bool CheckIntegralData()
        {
            string sqlStr =
                "SELECT COUNT(1) FROM Tuhu_profiles..tbl_UserIntegralStatistics WITH(NOLOCK) WHERE Year=YEAR(GETDATE())-1;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd)) > 0;
            }
        }

        public static IEnumerable<ExpiringIntegralModel> GetIntegralID(int index, int step)
        {
            string sql = @"SELECT  IntegralID
FROM    Tuhu_profiles..tbl_UserIntegral WITH ( NOLOCK )
WHERE   Status = 1
ORDER BY IntegralID
OFFSET @begin ROWS FETCH NEXT @step ROWS ONLY";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@begin", index * step);
                cmd.Parameters.AddWithValue("@step", step);
                return DbHelper.ExecuteSelect<ExpiringIntegralModel>(true, cmd);
            }
        }

        public static int InsertIntegralStatistics(DataTable dt)
        {
            string sql = @"
insert into Tuhu_profiles..tbl_UserIntegralStatistics
(
    IntegralID,
    Year,
    IntegralType,
    Integral
)
select TA.Id,
       YEAR(TB.CreateDateTime) as Year,
       TC.IntegralType,
       case
           when TC.IntegralType = 0 then
               SUM(ABS(TB.TransactionIntegral))
           else
               SUM(0 - ABS(TB.TransactionIntegral))
       end as Integral
from @UserIds as TA
    join Tuhu_profiles..tbl_UserIntegralDetail as TB with (nolock)
        on TA.Id = TB.IntegralID
    join Tuhu_profiles..tbl_UserIntegralRule as TC with (nolock)
        on TB.IntegralRuleID = TC.IntegralRuleID
where YEAR(TB.CreateDateTime) = YEAR(GETDATE()) - 1
      and TB.IsActive = 1
group by TA.Id,
         YEAR(TB.CreateDateTime),
         TC.IntegralType
order by TA.Id;";
            using (var cmd = new SqlCommand(sql))
            {
                var dtPara = cmd.Parameters.AddWithValue("@UserIds", dt);
                dtPara.SqlDbType = SqlDbType.Structured;
                dtPara.TypeName = "[GuidTypeList]";
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static DataTable ConvertToDataTable(List<Guid> data)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Id", typeof(Guid))
            });
            foreach (var item in data)
            {
                DataRow r = dt.NewRow();
                r[0] = item;
                dt.Rows.Add(r);
            }

            return dt;
        }

        #endregion

        #region [积分过期]

        public static int GetExpiringIntegralUserCount()
        {
            const string sqlStr = @"
SELECT  COUNT(1)
FROM    ( SELECT    SUM(TB.Integral) AS Integral ,
                    UserId
          FROM      Tuhu_profiles..tbl_UserIntegral AS TA WITH ( NOLOCK )
                    JOIN Tuhu_profiles..tbl_UserIntegralStatistics AS TB WITH ( NOLOCK ) ON TA.IntegralID = TB.IntegralID
          WHERE     Year < YEAR(GETDATE()) - 1 
          GROUP BY  UserId
        ) AS T;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                var data = DbHelper.ExecuteScalar(cmd);
                int.TryParse(data?.ToString(), out var value);
                return value;
            }
        }

        public static List<UserExpiringPointModel> GetUserExpiringPointInfo(int start, int step)
        {
            const string sqlStr = @"
SELECT  SUM(TB.Integral) AS Integral ,
        UserId ,
        MIN(TB.IntegralID) AS IntegralId
FROM    Tuhu_profiles..tbl_UserIntegral AS TA WITH ( NOLOCK )
        JOIN Tuhu_profiles..tbl_UserIntegralStatistics AS TB WITH ( NOLOCK ) ON TA.IntegralID = TB.IntegralID
WHERE   Year < YEAR(GETDATE()) - 1
GROUP BY UserId
ORDER BY UserId
        OFFSET @start ROWS FETCH NEXT @step ROWS ONLY;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@step", step);
                return DbHelper.ExecuteSelect<UserExpiringPointModel>(true, cmd)?.ToList() ??
                       new List<UserExpiringPointModel>();
            }
        }

        public static List<UserExpiringPointModel> GetUserConsumeIntegral(List<Guid> userList)
        {
            const string sqlStr = @"
SELECT  SUM(0 - ABS(TB.TransactionIntegral)) AS Integral ,
        TA.UserId
FROM    @TBV AS T
        LEFT JOIN Tuhu_profiles..tbl_UserIntegral AS TA WITH ( NOLOCK ) ON T.Id = TA.UserId
        JOIN Tuhu_profiles..tbl_UserIntegralDetail AS TB WITH ( NOLOCK ) ON TA.IntegralID = TB.IntegralID
        JOIN Tuhu_profiles..tbl_UserIntegralRule AS TC WITH ( NOLOCK ) ON TB.IntegralRuleID = TC.IntegralRuleID
WHERE   YEAR(TB.CreateDateTime) >= YEAR(GETDATE())-1
        AND TC.IntegralType = 1
        AND TB.IsActive = 1
GROUP BY TA.UserId;";
            var dt = new DataTable();
            dt.Columns.AddRange(new[]
            {
                new DataColumn("Id", typeof(Guid))
            });
            foreach (var item in userList)
            {
                var r = dt.NewRow();
                r[0] = item;
                dt.Rows.Add(r);
            }

            using (var cmd = new SqlCommand(sqlStr))
            {
                var dtPara = cmd.Parameters.AddWithValue("@TBV", dt);
                dtPara.SqlDbType = SqlDbType.Structured;
                dtPara.TypeName = "[GuidTypeList]";
                return DbHelper.ExecuteSelect<UserExpiringPointModel>(true, cmd)?.ToList() ??
                       new List<UserExpiringPointModel>();
            }
        }

        public static void ExpireUserPoint(List<UserExpiringPointModel> data, Guid integralRule)
        {
            #region [构建表数据]

            var dt = new DataTable();
            dt.Columns.AddRange(new[]
            {
                new DataColumn("IntegralDetailID", typeof(Guid)),
                new DataColumn("IntegralID", typeof(Guid)),
                new DataColumn("IntegralRuleID", typeof(Guid)),
                new DataColumn("TransactionIntegral", typeof(int)),
                new DataColumn("TransactionRemark", typeof(string)),
                new DataColumn("TransactionDescribe", typeof(string)),
                new DataColumn("IsActive", typeof(bool)),
                new DataColumn("TransactionChannel", typeof(string)),
                new DataColumn("Versions", typeof(string)),
                new DataColumn("CreateDateTime", typeof(DateTime)),
                new DataColumn("LasetUpdateTime", typeof(DateTime))
            });

            foreach (var item in data)
            {
                var r = dt.NewRow();
                r["IntegralDetailID"] = Guid.NewGuid();
                r["IntegralID"] = item.IntegralId;
                r["IntegralRuleID"] = integralRule;
                r["TransactionIntegral"] = item.Integral;
                r["TransactionRemark"] = "积分过期";
                r["TransactionDescribe"] = $"{DateTime.Now.Year - 2}年积分过期";
                r["TransactionChannel"] = "CJob";
                r["Versions"] = "0.0.1";
                r["CreateDateTime"] = $"{DateTime.Now.Year - 1}-12-31 23:59";
                r["LasetUpdateTime"] = DateTime.Now;
                dt.Rows.Add(r);
            }

            #endregion

            using (var db = DbHelper.CreateDbHelper())
            {
                db.BeginTransaction();
                dt.TableName = "Tuhu_profiles.dbo.tbl_UserIntegralDetail";
                db.BulkCopy(dt);
                db.Commit();
            }
        }

        #endregion

        #region [积分数据统计]

        public static List<Guid> GetTerribleIntegralIds()
        {
            const string sqlStr = @"
select distinct
       IntegralID
from Tuhu_profiles..tbl_UserIntegralDetail with (nolock)
where YEAR(CreateDateTime) = 2016
      and IsActive = 0;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteQuery(true, cmd, dt =>
                {
                    var result = new List<Guid>();
                    if (dt == null || dt.Rows.Count < 1) return result;
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var value = dt.Rows[i].GetValue<Guid>("IntegralID");
                        if (value != Guid.Empty) result.Add(value);
                    }

                    return result;
                });
            }
        }

        public static bool DeleteTerribleItem(DataTable dt)
        {
            const string sqlStr = @"
delete Tuhu_profiles..tbl_UserIntegralStatistics
where IntegralID in (
                        select id from @dt
                    );";
            using (var cmd = new SqlCommand(sqlStr))
            {
                var dtPara = cmd.Parameters.AddWithValue("@dt", dt);
                dtPara.SqlDbType = SqlDbType.Structured;
                dtPara.TypeName = "[GuidTypeList]";
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }
        #endregion

    }
}
