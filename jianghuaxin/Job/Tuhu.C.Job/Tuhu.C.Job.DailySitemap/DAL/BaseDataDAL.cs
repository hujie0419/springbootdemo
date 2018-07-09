using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.DailySitemap.DAL
{
    class BaseDataDAL
    {
        public static IEnumerable<int> GetArticleID(bool isFull = false)
        {
            string sqltext = string.Empty;
            List<int> articleIDList = new List<int>();
            if (isFull)
            {
                sqltext = BaseDataSqlText.SqlTextAllArticleIDs;
            }
            else
            {
                sqltext = BaseDataSqlText.sqlTextPartialArticleIDs;
            }

            using (
               var dbhelper =
                   DbHelper.CreateDbHelper(
                       ConfigurationManager.ConnectionStrings["Marketing"].ConnectionString))
            {
                using (var cmd = new SqlCommand(sqltext) { CommandTimeout = 10 * 60 })
                {
                    return dbhelper.ExecuteQuery(cmd, dt => dt.ToList<int>("PKID"));
                }
            }
        }

        public static void InsertURL(string url, string type)
        {
            if (!string.IsNullOrWhiteSpace(url) && !string.IsNullOrWhiteSpace(type))
            {
                using (var dbhelper = DbHelper.CreateDbHelper())
                {
                    using (
                         var cmd =
                             new SqlCommand(BaseDataSqlText.sqlTextInsertUrl))
                    {
                        DateTime time = DateTime.Now;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@URL", url);
                        cmd.Parameters.AddWithValue("@Type", type);
                        cmd.Parameters.AddWithValue("@DataCreate_Time", time);
                        cmd.Parameters.AddWithValue("@DataUpdate_Time", time);
                    }
                }
            }
        }
    }
}
