using Common.Logging;
using Nest;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Push.Models.MessageBox;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class SyncMessageBoxToEsJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<SyncMessageBoxToEsJob>();
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

        static bool CreatIndex(IElasticClient client)
        {
            return client.CreateIndexIfNotExists(UserMessageBoxRelationIndex,
                c => c.Settings(_ => _.NumberOfShards(20).NumberOfReplicas(1))
                    .Mappings(cm => cm
                        .Map<UserMessageBoxInfoRelationC>(m => m
                            .RoutingField(mr => mr.Required())
                            .SourceField(ms => ms.Enabled(false))
                            .AutoMap())));
        }
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info($"刷新开始执行");
            int count = 0;
            int maxpkid = 0;
            string runtimename = "SyncMessageBoxToEs";
            var result = CheckIsOpenWithDescription(runtimename);
            if (!result.Item1)
            {
                Logger.Info("开关已关,return");
                return;
            }
            int.TryParse(result.Item2, out maxpkid);
#if DEBUG
            maxpkid = 0;
#endif
            Logger.Info($"maxpkid:{maxpkid}");
            if (!CreatIndex(ElasticSearchClient))
            {
                Logger.Info($"index创建失败");
                return;
            }
            while (true)
            {
                try
                {
                    result = CheckIsOpenWithDescription(runtimename);
                    if (!result.Item1)
                    {
                        Logger.Info("开关已关,return");
                        return;
                    }
                    count++;
                    Logger.Info($"第{count}批次循环刷新开始执行,maxpkid:{maxpkid}");
                    var relations = SelectMessageRelation(maxpkid);
                    if (relations != null && relations.Any())
                    {
                        maxpkid = relations.Max(x => x.PKID);
                        var temps = ParseTargets(relations, 1000);
                        Parallel.ForEach(temps, new ParallelOptions()
                        {
                            MaxDegreeOfParallelism = 3,
                            TaskScheduler = TaskScheduler.Default
                        },
                            temp =>
                         {
                             var operations = new List<IBulkOperation>();
                             foreach (var relation in ConvertMessageLogToEsLog(temp))
                             {
                                 operations.Add(new BulkIndexOperation<UserMessageBoxInfoRelationC>(relation)
                                 {
                                     Routing = relation.UserId
                                 });
                             }
                             var bulkRequest = new BulkRequest(UserMessageBoxRelationIndex)
                             {
                                 Operations = operations.ToArray()
                             };
                             var responseOne = ElasticSearchClient.Bulk(bulkRequest);
                             var errorcount = responseOne.ItemsWithErrors.Count();
                             Logger.Info($"第{count}批次结束刷新.errorcount:{errorcount}");
                         });
                        UpdateRunTimeSwitchDescription(runtimename, maxpkid.ToString());
                    }
                    else
                    {
                        break;
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Warn(ex);
                    break;
                }
            }
            Logger.Info($"结束刷新");
        }

        public IEnumerable<IEnumerable<T>> ParseTargets<T>(IEnumerable<T> targets, int maxcount)
        {
            if (targets != null && targets.Any())
            {
                int TotalCount = targets.Count();
                int pages = TotalCount / maxcount;
                for (int i = 0; i <= pages; i++)
                {
                    var temps = targets.Skip(i * maxcount).Take(maxcount);
                    yield return temps;
                }
            }
            else
            {
                yield break;
            }
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
                UserId = Convert.ToBase64String(x.UserID.ToByteArray()),
                MessageType = x.MessageType
            });
            return result;
        }
        private IEnumerable<UserMessageBoxInfoRelation> SelectMessageRelation(int maxpkid)
        {
            string sql = $@"SELECT TOP 10000
        *
FROM    Tuhu_notification..UserMessageBoxInfoRelation WITH ( NOLOCK )
WHERE   PKID > {maxpkid};";
            using (var helper = DbHelper.CreateLogDbHelper())
            using (var cmd = helper.CreateCommand(sql))
            {
                var table = helper.ExecuteQuery(cmd, _ => _);
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
                        MessageType = Convert.IsDBNull(row["MessageType"]) ? MessageType.Activity : (MessageType)Enum.Parse(typeof(MessageType), row["MessageType"]?.ToString() ?? "1"),
                        MessageNavigationTypeId = Convert.IsDBNull(row["MessageNavigationTypeId"]) ? 1 : Convert.ToInt32(row["MessageNavigationTypeId"]),
                        SendType = (SendType)Enum.Parse(typeof(SendType), row["SendType"]?.ToString()),
                    });
                }
                else
                {
                    return new List<UserMessageBoxInfoRelation>();
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
                    var dt = helper.ExecuteQuery(cmd, _ => _);
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

    [ElasticsearchType(Name = "umbir", IdProperty = "Id")]
    public class UserMessageBoxInfoRelationC
    {
        /// <summary>
        /// messageId
        /// </summary>
        [String(Ignore = true)]
        public string Id => $"{PKID}|{MessageId}";

        [String(Index = FieldIndexOption.NotAnalyzed)]
        public string UserId { get; set; }

        [Number(Ignore = true)]
        public int PKID { get; set; }

        public int MessageId { get; set; }

        public bool IsDelete { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreateDateTime { get; set; } = DateTime.Now;
        public int MessageNavigationTypeId { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType MessageType { get; set; }
        /// <summary>
        /// 推送类型
        /// </summary>
        public SendType SendType { get; set; }
    }
}
