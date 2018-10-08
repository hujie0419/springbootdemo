using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.SqlServer.Server;
using Nest;
using Quartz;
using Tuhu.Service.Push.Models.MessageBox;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class DeleteExpiredMessageBoxJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<DeleteExpiredMessageBoxJob>();
        public const string UserMessageBoxRelationIndex = "umbrindex";
        private static IElasticClient ElasticSearchClient
        {
            get
            {
                var client = ElasticsearchHelper.CreateClient();
#if DEBUG
                ElasticsearchHelper.EnableDebug();
#endif
                return client;
            }
        }
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info($"刷新开始执行");
            int count = 0;
            int maxpkid = 0;
            int exceptioncount = 0;
            string runtimename = "DeleteExpiredMessageBox";
            var result = CheckIsOpenWithDescription(runtimename);
            if (!result.Item1)
            {
                Logger.Info("开关已关,return");
                return;
            }

            int.TryParse(result.Item2, out maxpkid);
            Logger.Info($"maxpkid:{maxpkid}");
            while (true)
            {
                count++;
                try
                {
                    result = CheckIsOpenWithDescription(runtimename);
                    if (!result.Item1)
                    {
                        Logger.Info("开关已关,return");
                        return;
                    }
                    Logger.Info($"第{count}批次循环刷新开始执行,maxpkid:{maxpkid}");
                    var relations = SelectDeleteMessageRelation(maxpkid);
                    if (relations != null && relations.Any())
                    {
                        maxpkid = relations.Max(x => x.PKID);
                        var operations = new List<IBulkOperation>();
                        var exlogs = ConvertMessageLogToEsLog(relations);
                        foreach (var log in exlogs)
                        {
                            operations.Add(new BulkDeleteOperation<UserMessageBoxInfoRelationC>(log.Id)
                            {
                                Routing = log.UserId
                            });
                        }
                        var bulkRequest = new BulkRequest(UserMessageBoxRelationIndex)
                        {
                            Operations = operations.ToArray()
                        };
                        var responseOne = ElasticSearchClient.Bulk(bulkRequest);
                        var errorcount = responseOne.ItemsWithErrors.Count();
                        WriteSyncLogs(relations);
                        UpdateRunTimeSwitchDescription(runtimename, maxpkid.ToString());
                        Logger.Info($"第{count}批次结束刷新.errorcount:{errorcount}");
                    }
                    else
                    {
                        break;
                    }
                }
                catch (System.Exception ex)
                {
                    maxpkid -= 1000;
                    Logger.Warn(ex);
                    exceptioncount++;
                    if (exceptioncount >= 10)
                    {
                        break;
                    }
                }
            }
            Logger.Info($"刷新结束");
        }
        IEnumerable<UserMessageBoxInfoRelationC> ConvertMessageLogToEsLog(
            IEnumerable<UserMessageBoxInfoRelation> logs)
        {
            var result = logs.Select(x => new UserMessageBoxInfoRelationC()
            {
                SendType = x.SendType,
                CreateDateTime = x.CreateDateTime,
                IsDelete = x.IsDelete,
                IsRead = x.IsRead,
                MessageId = x.MessageID,
                MessageNavigationTypeId = x.MessageNavigationTypeId,
                PKID = x.PKID,
                MessageType=x.MessageType,
                UserId = Convert.ToBase64String(x.UserID.ToByteArray())
            });
            return result;
        }

        private void WriteSyncLogs(IEnumerable<UserMessageBoxInfoRelation> logs)
        {
            // type=1的时候id是Tuhu_notification..MessageBoxMessageInfo的pkid type=2时id是Tuhu_notification..UserMessageBoxInfoRelation的pkid
            if (logs != null && logs.Any())
            {
                Func<IEnumerable<int>, int, int> WriteLogs = (ids, type) =>
                {
                    if (ids != null && ids.Any())
                    {
                        string sql = $@"INSERT  INTO Tuhu_notification..ExpiredMessageBoxInfo
                                        ( ID ,
                                          Type ,
                                          CreateDateTime ,
                                          LastUpdateDateTime
		                                 )
                                        SELECT  ss.TargetID ,
                                                {type} ,
                                                GETDATE() ,
                                                GETDATE()
                                        FROM    @TVP AS ss;";
                        using (var helper = DbHelper.CreateLogDbHelper())
                        {
                            using (var cmd = new SqlCommand(sql))
                            {
                                cmd.CommandType = CommandType.Text;
                                var records = new List<SqlDataRecord>(ids.Count());
                                foreach (var target in ids)
                                {
                                    var record = new SqlDataRecord(new SqlMetaData("TargetID", SqlDbType.Char, 40));
                                    var chars = new SqlChars(target.ToString());
                                    record.SetSqlChars(0, chars);
                                    records.Add(record);
                                }
                                SqlParameter p = new SqlParameter("@TVP", SqlDbType.Structured);
                                p.TypeName = "dbo.Target";
                                p.Value = records;
                                cmd.Parameters.Add(p);
                                return helper.ExecuteNonQuery(cmd);
                            }
                        }
                    }
                    return 0;
                };
                var messageids = logs.Select(x => x.MessageID).Distinct();
                var relationids = logs.Select(x => x.PKID);
                var messageresult = WriteLogs(messageids, 1);
                Logger.Info($"写过期消息{messageresult}个");
                var relationresult = WriteLogs(relationids, 2);
                Logger.Info($"写过期消息关系{relationresult}个");
            }
        }


        private IEnumerable<UserMessageBoxInfoRelation> SelectDeleteMessageRelation(int maxpkid)
        {
            string sql = $@"
         SELECT  top 1000  *
          FROM      Tuhu_notification..UserMessageBoxInfoRelation AS r WITH ( NOLOCK )
          WHERE     r.PKID > {maxpkid}
                    AND ( r.MessageID = 0
                          OR r.MessageNavigationTypeId IS NULL
                        )
          ORDER BY  r.PKID;  ";
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandTimeout = 100;
                    cmd.CommandType = CommandType.Text;
                    var table = helper.ExecuteQuery(cmd, dt => dt);
                    if (table != null && table.Rows.Count > 0)
                    {
                        return table.AsEnumerable().Select(row => new UserMessageBoxInfoRelation()
                        {
                            PKID = Convert.ToInt32(row["PKID"]),
                            CreateDateTime = Convert.ToDateTime(row["CreateDateTime"]),
                            IsDelete = Convert.ToBoolean(row["IsDelete"]),
                            LastUpdateDateTime = Convert.ToDateTime(row["LastUpdateDateTime"]),
                            MessageID = Convert.ToInt32(row["MessageID"]),
                            UserID = Guid.Parse(row["UserID"].ToString()),
                            IsRead = Convert.ToBoolean(row["IsRead"]),
                            MessageType = string.IsNullOrEmpty(row["MessageType"]?.ToString()) ? MessageType.Activity : (MessageType)Enum.Parse(typeof(MessageType), row["MessageType"]?.ToString()),
                            SendType = (SendType)Enum.Parse(typeof(SendType), row["SendType"]?.ToString()),
                            MessageNavigationTypeId = string.IsNullOrEmpty(row["MessageNavigationTypeId"]?.ToString()) ? 0 : Convert.ToInt32(row["MessageID"])
                        });
                    }
                    else
                    {
                        return new List<UserMessageBoxInfoRelation>();
                    }
                }

            }

        }
        public static bool UpdateRunTimeSwitchDescription(string name, string description)
        {
            string sql = $" update Gungnir..RuntimeSwitch with(rowlock) set description=N'{description}' where SwitchName=N'{name}' ";
            using (var helper = DbHelper.CreateDbHelper())
            {
                var result = helper.ExecuteNonQuery(sql);
                return result > 0;
            }
        }
        public static Tuple<bool, string> CheckIsOpenWithDescription(string name)
        {
            string sql = $"SELECT Value,Description FROM Gungnir..RuntimeSwitch WITH ( NOLOCK) WHERE SwitchName = N'{name}'";
            using (var helper = DbHelper.CreateDbHelper())
            using (var cmd = new SqlCommand(sql))
            {
                {
                    var dt = helper.ExecuteQuery(cmd, t => t);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        var isopen = string.Equals("true", row[0]?.ToString(), StringComparison.OrdinalIgnoreCase);
                        return Tuple.Create(isopen, row[1]?.ToString());
                    }
                    return Tuple.Create(false, "");
                }
            }
        }
    }

}
