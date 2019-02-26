using Common.Logging;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.Job;
using Tuhu.Service.Push.Models;
using Tuhu.Service.Push.Models.MessageBox;
using Tuhu.Service.Push.Models.Push;
using MessageType = Tuhu.Service.Push.Models.Push.MessageType;

namespace Tuhu.C.Job.DAL
{
    public class DalMessageBox
    {
        private static readonly ILog Logger = LogManager.GetLogger<DeleteExpiredMessageBoxJob>();
        public static IEnumerable<int> SelectExpireBroadcastMarketingMessageIds()
        {
            string sql = @"SELECT  PKID
        FROM    Tuhu_notification..MessageBoxMessageInfo WITH ( NOLOCK )
        WHERE   SendType = 'Broadcast'
                AND CreateDateTime <= GETDATE() - 14;";
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandTimeout = 1200;
                    cmd.CommandType = CommandType.Text;
                    var result = helper.ExecuteQuery(cmd, dt => dt.AsEnumerable().Select(r => Convert.ToInt32(r[0]?.ToString())));
                    return result;
                }
            }
        }

        public static int SelectExpireSingleMarketingMessageCount()
        {
            string sql = @"
SELECT  COUNT(distinct m.pkid) AS c
FROM    Tuhu_notification..tbl_PushTemplate AS t WITH ( NOLOCK )
        JOIN Tuhu_notification..MessageBoxMessageInfo AS m WITH ( NOLOCK ) ON t.BatchID = m.BatchID
WHERE   t.SendType = 3
        AND t.TemplateType IN ( 2, 3 )
        AND m.CreateDateTime <= GETDATE() - 14;";
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandTimeout = 1200;
                    cmd.CommandType = CommandType.Text;
                    var result = helper.ExecuteScalar(cmd);
                    int.TryParse(result?.ToString(), out int totalcount);
                    return totalcount;
                }
            }

        }
        public static List<int> SelectExpireSingleMarketingMessageIds(int maxpkid, int pagesize)
        {
            string sql = $@" 
        SELECT DISTINCT TOP {pagesize}
                m.PKID
        FROM    Tuhu_notification..tbl_PushTemplate AS t WITH ( NOLOCK )
                JOIN Tuhu_notification..MessageBoxMessageInfo AS m WITH ( NOLOCK ) ON t.BatchID = m.BatchID
        WHERE   t.SendType = 3
                AND t.TemplateType IN ( 2, 3 )
                AND m.CreateDateTime <= GETDATE() - 14
                AND m.PKID > {maxpkid}
        ORDER BY m.PKID;
";
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandTimeout = 22000;
                    cmd.CommandType = CommandType.Text;
                    var result = helper.ExecuteQuery(cmd, dt => dt.AsEnumerable().Select(r => Convert.ToInt32(r[0]?.ToString())))?.ToList();
                    return result ?? new List<int>();
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

        public static void WriteSyncLogs(IEnumerable<UserMessageBoxInfoRelation> logs)
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
                var messageids = logs?.Where(x => x.SendType != SendType.Broadcast).Select(x => x.MessageID).Distinct();
                var relationids = logs.Select(x => x.PKID);
                var messageresult = WriteLogs(messageids, 1);
                Logger.Info($"写过期消息{messageresult}个");
                var relationresult = WriteLogs(relationids, 2);
                Logger.Info($"写过期消息关系{relationresult}个");
            }
        }


        public static int SelectMessageRelationCountByMessageId(IEnumerable<int> messageIds)
        {
            string sql = $@"  SELECT count(1) FROM Tuhu_notification..UserMessageBoxInfoRelation WITH ( NOLOCK) WHERE MessageID IN ({string.Join(",", messageIds)}) ;";
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandTimeout = 1200;
                    cmd.CommandType = CommandType.Text;
                    var result = helper.ExecuteScalar(cmd);
                    int.TryParse(result?.ToString(), out int totalcount);
                    return totalcount;
                }
            }
        }
        public static IEnumerable<UserMessageBoxInfoRelation> SelectMessageRelationByMessageId(IEnumerable<int> messageIds, int maxpkid, int pagesize)
        {
            string sql = $@"  SELECT TOP {pagesize} * FROM Tuhu_notification..UserMessageBoxInfoRelation WITH ( NOLOCK) WHERE MessageID IN ({string.Join(",", messageIds)}) AND PKID>{maxpkid};";
            using (var helper = DbHelper.CreateLogDbHelper(true))
            using (var cmd = helper.CreateCommand(sql))
            {
                cmd.CommandTimeout = 500;
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
                        MessageType = (MessageType)Enum.Parse(typeof(MessageType), row["MessageType"]?.ToString()),
                        SendType = (SendType)Enum.Parse(typeof(SendType), row["SendType"]?.ToString()),
                    });
                }
                else
                {
                    return new List<UserMessageBoxInfoRelation>();
                }
            }

        }

        public static int DeleteBroadcastMessageInfo(IEnumerable<int> messageIds)
        {
            //todo: 备份
            if (messageIds != null && messageIds.Any())
            {
                string sql = string.Join(";", messageIds.Select(mid => $"update Tuhu_notification..MessageBoxMessageInfo set isdelete=1	WHERE PKID ={mid}"));
                using (var helper = DbHelper.CreateLogDbHelper(false))
                {
                    var result = helper.ExecuteNonQuery(sql);
                    return result;
                }
            }
            else
            {
                return -1;
            }
        }
    }
}
