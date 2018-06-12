using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using Tuhu.WebSite.Component.SystemFramework;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.SendEmail.DAL
{
    public static class SendActivityEmailJobDal
    {
        private static readonly string ConnectionStr =
            ConfigurationManager.ConnectionStrings["Insurance_db"]?.ConnectionString;

        public static DataTable GetSendUserPhone()
        {
            var sqlStr = @"
SELECT TOP 50000
        id,
        phone ,
        CreateDateTime
FROM    [Tuhu_huodong]..tbl_Insurance(NOLOCK)
WHERE   isSendEmail = 0
ORDER BY id;
";
            using(var dbHelper=DbHelper.CreateDbHelper(ConnectionStr))
            using (var cmd = new SqlCommand(sqlStr))
            {
                var dt = dbHelper.ExecuteQuery(cmd, one => one); // .ExecuteQuery(cmd)  .ExecuteDataTable(cmd);
                dt.TableName = "UserPhones";
                return dt;
            }
        }

        public static int UpdateSendStatus(List<int> idList)
        {
            var sqlStr = @"
UPDATE  [Tuhu_huodong]..tbl_Insurance
SET     isSendEmail = 1
FROM    [Tuhu_huodong]..tbl_Insurance ins
        INNER JOIN @ids ids ON ins.id = ids.IntId;
";
            List<SqlDataRecord> records = new List<SqlDataRecord>(idList.Count);
            foreach (var id in idList)
            {
                var record = new SqlDataRecord(new SqlMetaData("IntId", SqlDbType.Int));
                record.SetValue(0, id);
                records.Add(record);
            }
            try
            {
                var tvpParameter = new SqlParameter("@ids", records)
                {
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.[IntIdsType]"
                };

                using (var dbHelper = DbHelper.CreateDbHelper(ConnectionStr))
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.Add(tvpParameter);
                    var result = dbHelper.ExecuteNonQuery(cmd);
                    return result != idList.Count ? 0 : 1;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
