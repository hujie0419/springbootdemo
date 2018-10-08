using Common.Logging;
using Microsoft.SqlServer.Server;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
using Tuhu.Service.Push.Models.WeiXinPush;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class UpdateWxUserAuthJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<UpdateWxUserAuthJob>();
        public void Execute(IJobExecutionContext context)
        {
            string jobname = "UpdateWxUserAuthJob";
            var result = SyncMessageBoxToEsJob.CheckIsOpenWithDescription(jobname);
            if (!result.Item1)
            {
                Logger.Info("开关已关,return");
                return;
            }
            int maxpkid = 0;
            int count = 0;
            int.TryParse(result.Item2, out maxpkid);
            Logger.Info($"maxpkid:{maxpkid}");
#if DEBUG
            maxpkid = 0;
#endif
            while (true)
            {
                result = SyncMessageBoxToEsJob.CheckIsOpenWithDescription(jobname);
                if (!result.Item1)
                {
                    Logger.Info("开关已关,return");
                    return;
                }
                int.TryParse(result.Item2, out maxpkid);
#if DEBUG
                maxpkid = 0;
#endif
                count++;

                var datas = SelectWxUserAuth(maxpkid);
                if (datas != null && datas.Any())
                {
                    try
                    {
                        Logger.Info($"开始更新.maxpkid:{maxpkid} count:{datas.Count()}");
                        maxpkid = datas.Max(x => x.PKID);
                        SyncMessageBoxToEsJob.UpdateRunTimeSwitchDescription(jobname, maxpkid.ToString());
                        var unionids = datas.Select(x => x.UnionId).Distinct().Where(x => !string.IsNullOrEmpty(x));
                        var useridinfos = SelectUserIdsByUnionID(unionids);
                        if (useridinfos != null && useridinfos.Rows.Count > 0)
                        {
                            UpdateWxUserAuthInfo(useridinfos);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Logger.Warn(ex);
                    }
                }
                else
                {
                    break;
                }
            }
            Logger.Info("同步结束");
        }

        public static int UpdateWxUserAuthInfo(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                string sql = @"UPDATE  Tuhu_notification..WXUserAuth WITH(ROWLOCK)
SET     UserId = t.UserId,UpdatedTime=GETDATE()
FROM    Tuhu_notification..WXUserAuth AS w
        INNER  JOIN @TVP AS t ON t.UnionId = w.UnionId;";
                using (var helper = DbHelper.CreateLogDbHelper(false))
                {
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlParameter p = new SqlParameter("@TVP", SqlDbType.Structured);
                        p.TypeName = "dbo.WxUserAuthInfo";
                        p.Value = dt;
                        cmd.Parameters.Add(p);
                        var result = helper.ExecuteNonQuery(cmd);
                        return result;
                    }
                }
            }
            return 0;
        }

        public static DataTable SelectUserIdsByUnionID(IEnumerable<string> unionids)
        {
            if (unionids != null && unionids.Any())
            {
                string sql = @"SELECT  u.UserId ,u.UnionId   COLLATE Chinese_PRC_CI_AS 
FROM    Tuhu_profiles..UserAuth AS u WITH ( NOLOCK )
        JOIN @TVP AS t ON u.UnionId COLLATE Chinese_PRC_CI_AS   = t.TargetID  
WHERE   u.AuthSource = N'Weixin'
        AND u.BindingStatus = N'Bound' and u.UserId is not null and u.UnionId is not null ;";
                var records = new List<SqlDataRecord>(unionids.Count());
                foreach (var target in unionids)
                {
                    var record = new SqlDataRecord(new SqlMetaData("TargetID", SqlDbType.Char, 100));
                    var chars = new SqlChars(target);
                    record.SetSqlChars(0, chars);
                    records.Add(record);
                }
                using (var helper = DbHelper.CreateDbHelper(true))
                {
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlParameter p = new SqlParameter("@TVP", SqlDbType.Structured);
                        p.TypeName = "dbo.Targets";
                        p.Value = records;
                        cmd.Parameters.Add(p);
                        DataTable result = helper.ExecuteQuery(cmd, dt => dt);
                        return result;
                    }
                }
            }
            return null;
        }


        public static IEnumerable<WXUserAuthModel> SelectWxUserAuth(int maxpkid)
        {
            string sql = $@"SELECT TOP 1000
                *
        FROM    Tuhu_notification..WXUserAuth WITH ( NOLOCK )
        WHERE   BindingStatus = N'Bound'
                AND AuthorizationStatus = N'Authorized'
		        AND PKID > {maxpkid}
		        ;";
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                var result = helper.ExecuteSelect<WXUserAuthModel>(sql);
                return result;
            }
        }
    }
}
