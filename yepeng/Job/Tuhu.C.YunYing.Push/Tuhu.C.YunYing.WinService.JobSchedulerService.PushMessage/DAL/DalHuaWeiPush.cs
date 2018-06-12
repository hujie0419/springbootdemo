using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Push.Models;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.DAL
{
    public class DalHuaWeiPush
    {

        public static int SelectHuaWeiDeviceTokenCounts(string vendor)
        {
            //WHERE   ( Vendor = N'huawei' or Vendor = N'huaweipro' )  and IsUserLogInDevice=1";
            int totalcount = 0;
            string sql =
                @" SELECT  COUNT(1)
FROM    [Tuhu_profiles].dbo.[Push_XiaoMiDeviceInfo] WITH ( NOLOCK )
WHERE   Vendor=@Vendor  and IsUserLogInDevice=1";
            using (var dbhelper = DbHelper.CreateDbHelper(true))
            {
                using (var cmd=new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Vendor", vendor);
                    var countresult = dbhelper.ExecuteScalar(cmd);
                    if (countresult != null && !Convert.IsDBNull(countresult))
                    {
                        int.TryParse(countresult.ToString(), out totalcount);
                    }
                }
                
            }
            return totalcount;
        }
        public static IEnumerable<PushDeviceInfo> SelectHuaWeiDeviceToken(int pageindex, int maxsize)
        {
            System.Threading.Thread.Sleep(1000);
            string sql = @"  SELECT *
FROM    [Tuhu_profiles].dbo.[Push_XiaoMiDeviceInfo] WITH ( NOLOCK )
WHERE  ( Vendor = N'huawei' or Vendor = N'huaweipro' )  and IsUserLogInDevice=1
ORDER BY pkid
            OFFSET ( @pagesize * ( @pageindex - 1 ) ) ROWS
    FETCH NEXT @pagesize ROWS ONLY;";
            //LogManager.GetLogger(typeof(DalHuaWeiPush)).Info($"华为推送mq.pageindex:{pageindex}.pagecount:{pagecount}");

            using (var dbhelper = DbHelper.CreateDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@pagesize", maxsize));
                    cmd.Parameters.Add(new SqlParameter("@pageindex", pageindex));
                    var result = dbhelper.ExecuteSelect<PushDeviceInfo>(cmd);
                    return result;
                }
            }

        }

        public static async Task<PushTemplate> SelectTemplateByPKIDAsync(int pkid)
        {
            string sql = $"select * from Tuhu_notification..tbl_PushTemplate  WITH(NOLOCK)  where  pkid = {pkid}";
            using (var dbhelper = DbHelper.CreateLogDbHelper())
            {
                var result = await dbhelper.ExecuteFetchAsync<PushTemplate>(sql);
                return result;
            }
        }

        public static IEnumerable<string> SelectMessageTypeClosedUserIDs(IEnumerable<string> userids, string topicname)
        {
            if (userids != null && userids.Any())
            {
                string sql = $@"SELECT  *
FROM    Tuhu_profiles..UserMessageSetting WITH ( NOLOCK )
WHERE   {topicname} = 0
        AND UserID IN ( SELECT  SS.TargetID COLLATE Chinese_PRC_CI_AS
                        FROM    @TVP AS SS )";
                var records = new List<SqlDataRecord>(userids.Count());
                foreach (var target in userids)
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
                        var result = helper.ExecuteSelect<UserMessageSetting>(cmd);
                        return result?.Select(x => x.UserID.ToString());
                    }
                }
            }
            return new List<string>();

        }
    }
}
