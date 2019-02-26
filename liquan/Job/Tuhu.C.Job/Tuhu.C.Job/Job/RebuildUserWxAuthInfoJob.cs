using Common.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Push.Models.WeiXinPush;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class RebuildUserWxAuthInfoJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<RebuildUserWxAuthInfoJob>();
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始重建");
            var runtimeResponse = CheckIsOpenWithDescription("RebuildUserWxAuthInfo");
            int maxpkid = 0;
            int minpkid = 0;
            if (runtimeResponse != null && runtimeResponse.Item1 && !string.IsNullOrEmpty(runtimeResponse.Item2) && runtimeResponse.Item2.Split(',').Count() >= 2)
            {
                var splitResult = runtimeResponse.Item2.Split(',');
                if (int.TryParse(splitResult[0], out minpkid) && int.TryParse(splitResult[1], out maxpkid))
                {
                    int pagesize = 100;
                    var totalCounts = SelectOpenIdCountsByMaxPkid(minpkid, maxpkid);
                    Logger.Info($"开始刷新.共{totalCounts}个.");
                    if (totalCounts != 0)
                    {
                        int pageCounts = (totalCounts / pagesize) + 1;
                        for (int i = 0; i < pageCounts; i++)
                        {
                            Logger.Info($"开始刷新{i}批次.共{totalCounts}个.{pageCounts}批次.");
                            var openids = SelectOpenIdByMaxPkid(ref minpkid, maxpkid, pagesize);
                            if (openids != null && openids.Any())
                            {
                                foreach (var kvp in openids)
                                {
                                    using (var client = new Tuhu.Service.Push.WeiXinPushClient())
                                    {
                                        var result = client.LogWxUserOpenIDWithChannel(kvp.Key, true, kvp.Value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Logger.Info("结束重建");
        }

        private static int SelectOpenIdCountsByMaxPkid(int minpkid, int maxpkid)
        {
            string sql = $@"SELECT count(1) as c 
FROM    Tuhu_notification..WXUserAuth WITH ( NOLOCK )
WHERE   PKID >= {minpkid} and pkid <= {maxpkid}
AND BindingStatus='Bound'
AND AuthorizationStatus='Authorized'
;";
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                var result = helper.ExecuteScalar(sql);
                if (int.TryParse(result?.ToString(), out int count))
                {
                    return count;
                }
                return 0;
            }
        }

        private static Dictionary<string, string> SelectOpenIdByMaxPkid(ref int minpkid, int maxpkid, int maxcount)
        {
            string sql = $@"SELECT TOP {maxcount}
        PKID ,
        OpenId ,
        Channel
FROM    Tuhu_notification..WXUserAuth WITH ( NOLOCK )
WHERE   PKID >= {minpkid} and pkid <= {maxpkid}
AND BindingStatus='Bound'
AND AuthorizationStatus='Authorized'
order by pkid asc
;";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    var datatable = helper.ExecuteQuery(cmd, _ => _);
                    if (datatable != null && datatable.Rows.Count > 0)
                    {
                        var result = datatable.Select().Select(r => new
                        {
                            openid = r["OpenId"]?.ToString(),
                            channel = r["Channel"]?.ToString(),
                            pkid = Convert.ToInt32(r["PKID"]?.ToString())
                        });
                        minpkid = result.Max(x => x.pkid);
                        foreach (var item in result)
                        {
                            if (!string.IsNullOrEmpty(item.openid) && !string.IsNullOrEmpty(item.channel))
                            {
                                dict[item.openid] = item.channel;
                            }
                        }
                    }
                }
            }
            return dict;
        }

        public static Tuple<bool, string> CheckIsOpenWithDescription(string name)
        {
            string sql = $"SELECT Value,Description FROM Gungnir..RuntimeSwitch WITH ( NOLOCK) WHERE SwitchName = N'{name}'";
            using (var helper = DbHelper.CreateDbHelper(true))
            using (var cmd = new SqlCommand(sql))
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
}
