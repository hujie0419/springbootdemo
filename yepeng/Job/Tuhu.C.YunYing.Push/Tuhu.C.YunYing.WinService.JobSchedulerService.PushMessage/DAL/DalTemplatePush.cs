using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using Tuhu.Service.Push.Models.Push;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Model;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.DAL
{
    internal class DalTemplatePush
    {
        public static IEnumerable<PushTemplate> SelectPushTemplates()
        {
            string sql = $"SELECT * FROM Tuhu_notification..tbl_PushTemplate where  pushstatus={(int)PushStatus.Intend} and sendtime <= getdate()";
            using (var helper = DbHelper.CreateLogDbHelper())
            {
                var result = helper.ExecuteSelect<PushTemplate>(sql);
                return result;
            }
        }
        public static PushTemplate SelectPushTemplateByPkid(int pkid)
        {
            string sql = $"SELECT * FROM Tuhu_notification..tbl_PushTemplate where  pkid={pkid}";
            using (var helper = DbHelper.CreateLogDbHelper())
            {
                var result = helper.ExecuteFetch<PushTemplate>(sql);
                return result;
            }
        }

        public static IEnumerable<PushTemplate> SelectPushTemplatesByBatchID(int batchid)
        {
            string sql = $"SELECT * FROM Tuhu_notification..tbl_PushTemplate  with(NOLOCK)  where  BatchID={batchid}";
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                var result = helper.ExecuteSelect<PushTemplate>(sql);
                return result;
            }
        }
        public static int UpdateTemplateStatus(int templateid, PushStatus status)
        {
            string sql = $"UPDATE Tuhu_notification..tbl_PushTemplate SET PushStatus={(int)status} WHERE PKID={templateid}";
            using (var helper = DbHelper.CreateLogDbHelper(false))
            {
                var result = helper.ExecuteNonQuery(sql);
                return result;
            }
        }

        public static IEnumerable<PushTemplate> SelectPushedTemplates()
        {
            string sql = $@"SELECT  *
            FROM Tuhu_notification..tbl_PushTemplate WITH(NOLOCK )
           WHERE PushStatus = 2
            AND CreateTime>= '{DateTime.Now.AddDays(-14).ToString("yyyy-MM-dd 00:00:00")}'
              AND DeviceType in (1,2)
            AND batchid <> 14
           AND SendType = 1; ";
            using (var helper = DbHelper.CreateLogDbHelper())
            {
                var result = helper.ExecuteSelect<PushTemplate>(sql);
                return result;
            }
        }

        public static IEnumerable<BiTemplatePushLog> SelectBiTemplatePushLogs(int maxcount)
        {
            string sql = $@"
SELECT TOP {maxcount} * INTO #bitemplate FROM Tuhu_bi..tbl_BiTemplatePushLog WITH ( NOLOCK) WHERE IsSync=0 AND (SendTime is null or SendTime <=GetDate()) ORDER BY PKID ;

UPDATE Tuhu_bi..tbl_BiTemplatePushLog  WITH(ROWLOCK) SET IsSync=1 WHERE PKID IN (SELECT pkid FROM #bitemplate);
SELECT * FROM #bitemplate;
DROP TABLE #bitemplate;
";
            using (var helper = DbHelper.CreateDbHelper("Tuhu_BI", false))
            {
                var result = helper.ExecuteSelect<BiTemplatePushLog>(sql);
                return result;
            }
        }
        /// <summary>
        /// 计划送达数
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CountSinglePushModel> CountSinglePushResolved()
        {
            string sql = $@"SELECT  COUNT(1) AS SendCount ,
        CONVERT(VARCHAR(12), CreateTime, 111) AS PushTime ,
        TemplateInfo ,
        DeviceType
FROM    Tuhu_notification..tbl_PushTemplateLog WITH ( NOLOCK )
WHERE   CreateTime>='{DateTime.Now.AddDays(-14)}'
        AND DeviceType IN ( 1, 2, 5,4,6 )
       
        AND TemplateInfo IN (
        SELECT  PKID
        FROM    Tuhu_notification..tbl_PushTemplate WITH ( NOLOCK )
        WHERE   SendType = 3
                AND DeviceType IN ( 1, 2,4 ,6) )
GROUP BY CONVERT(VARCHAR(12), CreateTime, 111) ,
        TemplateInfo ,
        DeviceType;";

            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    var result = helper.ExecuteSelect<CountSinglePushModel>(cmd);
                    if (result != null && result.Any())
                    {
                        return result;
                    }
                    else
                    {
                        return new List<CountSinglePushModel>();
                    }
                }
            }
        }
        /// <summary>
        /// 送达数
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CountSinglePushModel> CountSinglePushDelivered()
        {
            string sql = $@"SELECT  COUNT(1) AS SendCount ,
        CONVERT(VARCHAR(12), CreateTime, 111) AS PushTime ,
        TemplateInfo ,
        DeviceType
FROM    Tuhu_notification..tbl_PushTemplateLog WITH ( NOLOCK )
WHERE    CreateTime>='{DateTime.Now.AddDays(-14)}'
        AND DeviceType IN ( 1, 2, 5,4,6 )
        AND IsSend = 1
        AND TemplateInfo IN (
        SELECT  PKID
        FROM    Tuhu_notification..tbl_PushTemplate WITH ( NOLOCK )
        WHERE   SendType = 3
                AND DeviceType IN ( 1, 2,4,6 ) )
GROUP BY CONVERT(VARCHAR(12), CreateTime, 111) ,
        TemplateInfo ,
        DeviceType;";

            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    var result = helper.ExecuteSelect<CountSinglePushModel>(cmd);
                    if (result != null && result.Any())
                    {
                        return result;
                    }
                    else
                    {
                        return new List<CountSinglePushModel>();
                    }
                }
            }

        }

        /// <summary>
        /// 按照设备类型计算送达数
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CountSinglePushModel> CountSinglePushDeliveredByDeviceType(int devicetype)
        {
            string sql = $@"SELECT  COUNT(1) AS SendCount ,
        CONVERT(VARCHAR(12), CreateTime, 111) AS PushTime ,
        TemplateInfo ,
        DeviceType
FROM    Tuhu_notification..tbl_PushTemplateLog WITH ( NOLOCK )
WHERE    CreateTime>='{DateTime.Now.AddDays(-14)}'
        AND DeviceType ={devicetype}
        AND IsSend = 1
        AND TemplateInfo IN (
        SELECT  PKID
        FROM    Tuhu_notification..tbl_PushTemplate WITH ( NOLOCK )
        WHERE   SendType = 3
                AND DeviceType ={(devicetype == 5 ? 2 : devicetype)} )
GROUP BY CONVERT(VARCHAR(12), CreateTime, 111) ,
        TemplateInfo ,
        DeviceType;";

            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandTimeout = 50;
                    var result = helper.ExecuteSelect<CountSinglePushModel>(cmd);
                    if (result != null && result.Any())
                    {
                        return result;
                    }
                    else
                    {
                        return new List<CountSinglePushModel>();
                    }
                }
            }

        }
        /// <summary>
        /// 计划送达数
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CountSinglePushModel> CountSinglePushResolvedByDeviceType(int devicetype)
        {
            string sql = $@"SELECT  COUNT(1) AS SendCount ,
        CONVERT(VARCHAR(12), CreateTime, 111) AS PushTime ,
        TemplateInfo ,
        DeviceType
FROM    Tuhu_notification..tbl_PushTemplateLog WITH ( NOLOCK )
WHERE   CreateTime>='{DateTime.Now.AddDays(-14)}'
        AND DeviceType ={devicetype}
       
        AND TemplateInfo IN (
        SELECT  PKID
        FROM    Tuhu_notification..tbl_PushTemplate WITH ( NOLOCK )
        WHERE   SendType = 3
               AND DeviceType ={(devicetype == 5 ? 2 : devicetype)} )
GROUP BY CONVERT(VARCHAR(12), CreateTime, 111) ,
        TemplateInfo ,
        DeviceType;";

            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandTimeout = 50;
                    var result = helper.ExecuteSelect<CountSinglePushModel>(cmd);
                    if (result != null && result.Any())
                    {
                        return result;
                    }
                    else
                    {
                        return new List<CountSinglePushModel>();
                    }
                }
            }
        }

        public static bool CheckIsOpenByName(string name)
        {
#if DEBUG
            return true;
#endif
            string sql = $"SELECT Value FROM Gungnir..RuntimeSwitch WITH ( NOLOCK) WHERE SwitchName = N'{name}'";
            using (var helper = DbHelper.CreateDbHelper(false))
            {
                var result = helper.ExecuteScalar(sql);
                return result != null && string.Equals(result?.ToString(), "true", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static Tuple<bool, int> CheckIsOpen()
        {
            string sql = $"SELECT Value,Description FROM Gungnir..RuntimeSwitch WITH ( NOLOCK) WHERE SwitchName = N'bipush'";
            using (var helper = DbHelper.CreateDbHelper(false))
            using (var cmd = new SqlCommand(sql))
            {
                {
                    var result = helper.ExecuteQuery(cmd, dt =>
                    {
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            var row = dt.Rows[0];
                            var isopen = string.Equals("true", row[0]?.ToString(), StringComparison.OrdinalIgnoreCase);
                            int maxcount;
                            if (int.TryParse(row[1]?.ToString(), out maxcount))
                            {
                                return Tuple.Create(isopen, maxcount);
                            }
                            else
                            {
                                return Tuple.Create(false, 0);
                            }
                        }
                        return Tuple.Create(false, 0);
                    });
                    return result;
                }
            }
        }

        public static bool UpdateRunTimeSwitchDescription(string name, string description)
        {
            string sql = $" update Gungnir..RuntimeSwitch with(rowlock) set description=N'{description}' where SwitchName=N'{name}' ";
            using (var helper = DbHelper.CreateDbHelper(false))
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
                    var result = helper.ExecuteQuery(cmd, dt =>
                    {
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            var row = dt.Rows[0];
                            var isopen = string.Equals("true", row[0]?.ToString(), StringComparison.OrdinalIgnoreCase);
                            return Tuple.Create(isopen, row[1]?.ToString());
                        }
                        return Tuple.Create(false, "");
                    });
                    return result;
                }
            }
        }
        public static int CreateOrUpdateMessageStatistics(CalculateMessageInfo info, string CreateTime)
        {
            string sql = @"
MERGE INTO Tuhu_notification..tbl_MessageStatistics WITH ( ROWLOCK ) AS s
USING
    ( SELECT    @Templateid AS Templateid ,
                @CreateTime as CreateDateTime,
                @PushServiceType AS PushServiceType ,
                @Devicetype AS Devicetype
    ) AS c
ON c.Templateid = s.Templateid
   AND c.CreateDateTime= CONVERT(VARCHAR(12),   s.CreateDateTime, 111)
    AND c.PushServiceType = s.PushServiceType
    AND c.Devicetype = s.Devicetype
WHEN MATCHED THEN
    UPDATE SET Delivered = @Delivered ,
               DeliveryRate = @DeliveryRate ,
               Click = @Click ,
               Resolved = @Resolved ,
               ClickRate = @ClickRate,
               LastUpdateDateTime=GETDATE()
WHEN NOT MATCHED THEN
    INSERT ( Templateid ,
             MessageID ,
             Devicetype ,
             PushServiceType ,
             Delivered ,
             DeliveryRate ,
             Click ,
             Resolved ,
             ClickRate ,
             CreateDateTime ,
             LastUpdateDateTime
           )
    VALUES ( @Templateid , -- Templateid - int
             @MessageID , -- MessageID - nvarchar(100)
             @Devicetype , -- Devicetype - nvarchar(100)
             @PushServiceType , -- PushServiceType - nvarchar(100)
             @Delivered , -- Delivered - nvarchar(100)
             @DeliveryRate , -- DeliveryRate - nvarchar(100)
             @Click , -- Click - nvarchar(100)
             @Resolved , -- Resolved - nvarchar(100)
             @ClickRate , -- ClickRate - nvarchar(100)
             @CreateTime , -- CreateDateTime - datetime
             GETDATE()  -- LastUpdateDateTime - datetime
           );";
            using (var dbhelper = DbHelper.CreateLogDbHelper(false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@Templateid", info.Templateid));
                    cmd.Parameters.Add(new SqlParameter("@MessageID", info.MessageID));
                    cmd.Parameters.Add(new SqlParameter("@Devicetype", info.Devicetype.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@PushServiceType", info.PushServiceType.ToString()));
                    cmd.Parameters.Add(new SqlParameter("@Delivered", info.Delivered));
                    cmd.Parameters.Add(new SqlParameter("@Click", info.Click));
                    cmd.Parameters.Add(new SqlParameter("@Resolved", info.Resolved));
                    cmd.Parameters.Add(new SqlParameter("@ClickRate", info.ClickRate));
                    cmd.Parameters.Add(new SqlParameter("@DeliveryRate", info.DeliveryRate));
                    cmd.Parameters.Add(new SqlParameter("@CreateTime", CreateTime));
                    var result = dbhelper.ExecuteNonQuery(cmd);
                    return result;
                }
            }
        }

        public static IEnumerable<ProductJobPush> SelectProductSendSmsModels()
        {
            string sql = @"SELECT   *
   FROM     Tuhu_notification..tbl_ProductJobPush WITH ( NOLOCK )
   WHERE    IsSend = 1
           AND IsSendSms = 0
            AND SendSmsTime IS NULL
            AND ClickTime <= DATEADD(HOUR, -24, GETDATE())
            AND IsCreateOrder = 0;";
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                var result = helper.ExecuteSelect<ProductJobPush>(sql);
                return result;
            }
        }
        public static IEnumerable<ProductJobPush> SelectProductPushModels()
        {
            string sql = @"     SELECT   *
   FROM     Tuhu_notification..tbl_ProductJobPush WITH ( NOLOCK )
   WHERE    DeviceID NOT IN (
            SELECT  DeviceID
            FROM    Tuhu_notification..tbl_ProductJobPush WITH ( NOLOCK )
            WHERE   IsSend = 1
                    -- AND LastSendTime >= DATEADD(HOUR, -24, GETDATE())
            UNION ALL
            SELECT  DeviceID
            FROM    Tuhu_notification..tbl_ProductJobPush WITH ( NOLOCK )
            WHERE   IsCreateOrder = 1
                    AND CreateOrderTime IS NOT NULL
                    AND ClickTime >= DATEADD(MINUTE, -10, CreateDateTime) )
            AND ClickTime <= DATEADD(MINUTE, -10, GETDATE());  ";

            using (var helper = DbHelper.CreateLogDbHelper())
            {
                var result = helper.ExecuteSelect<ProductJobPush>(sql);
                return result;
            }
        }


        public static int UpdateProductPushPushResult(string deviceid)
        {
            string sql =
                $"UPDATE Tuhu_notification..tbl_ProductJobPush WITH(ROWLOCK) SET IsSend=1,LastSendTime=GETDATE() WHERE DeviceID='{deviceid}'";
            using (var helper = DbHelper.CreateLogDbHelper(false))
            {
                var result = helper.ExecuteNonQuery(sql);
                return result;
            }
        }

        public static int UpdateProductPushPushSmsResult(string userid)
        {
            string sql =
                $"UPDATE Tuhu_notification..tbl_ProductJobPush WITH(ROWLOCK) SET IsSendSms=1,SendSmsTime=GETDATE() WHERE UserID='{userid}' ";
            using (var helper = DbHelper.CreateLogDbHelper(false))
            {
                var result = helper.ExecuteNonQuery(sql);
                return result;
            }
        }
        public static Dictionary<string, string> SelectMobilesByUserIDAsync(IEnumerable<string> userids)
        {
            if (userids != null && userids.Any())
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                string sql = @"SELECT  
        UserID,u_mobile_number
FROM    Tuhu_profiles..UserObject (NOLOCK) uo
        JOIN @TVP p ON uo.UserID = p.TargetID;";
                var records = new List<SqlDataRecord>(userids.Count());
                foreach (var target in userids)
                {
                    var record = new SqlDataRecord(new SqlMetaData("TargetID", SqlDbType.Char, 100));
                    var chars = new SqlChars(target);
                    record.SetSqlChars(0, chars);
                    records.Add(record);
                }
                using (var helper = DbHelper.CreateDbHelper())
                {
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlParameter p = new SqlParameter("@TVP", SqlDbType.Structured);
                        p.TypeName = "dbo.Targets";
                        p.Value = records;
                        cmd.Parameters.Add(p);
                        var result = helper.ExecuteQuery(cmd, dt => dt);
                        if (result != null && result.Rows.Count > 0)
                        {
                            foreach (DataRow row in result.Rows)
                            {
                                var userid = row[0]?.ToString()?.Trim();
                                var mobile = row[1]?.ToString()?.Trim();
                                if (!string.IsNullOrEmpty(userid) && !string.IsNullOrEmpty(mobile))
                                {
                                    dict[userid] = mobile;
                                }
                            }
                        }
                        return dict;
                    }
                }
            }
            else
            {
                return new Dictionary<string, string>();
            }
        }

        public static IEnumerable<DelayPushModel> SelectDelayPushLogs(int maxcount)
        {
            string sql = $@"
SELECT TOP {maxcount} * INTO #delaypush FROM Tuhu_notification..DelayPushLogs WITH ( NOLOCK) WHERE IsSync=0  ORDER BY PKID ;

UPDATE Tuhu_notification..DelayPushLogs  WITH(ROWLOCK) SET IsSync=1 WHERE PKID IN (SELECT pkid FROM #delaypush);
SELECT * FROM #delaypush;
DROP TABLE #delaypush;
";
            using (var helper = DbHelper.CreateLogDbHelper(false))
            {
                var result = helper.ExecuteSelect<DelayPushModel>(sql);
                return result;
            }
        }
    }
}
