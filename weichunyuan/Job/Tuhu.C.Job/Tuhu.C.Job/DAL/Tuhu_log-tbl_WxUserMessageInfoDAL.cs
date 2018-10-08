using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.DAL
{
    public class Tuhu_log_tbl_WxUserMessageInfoDAL
    {
        private ILog logger;

        public Tuhu_log_tbl_WxUserMessageInfoDAL(ILog logger)
        {
            this.logger = logger;
        }

        public List<string> GetMessageOpenids(string OriginalID,DateTime date)
        {
            string sql = @"SELECT  DISTINCT FromUserName from [Tuhu_log].[dbo].[tbl_WxUserMessageInfo]  WITH ( NOLOCK ) where ToUserName = '{0}' and MsgType = 'text'
and(Content like '%轮胎%' or Content like '%机油%'  or Content like '%店%') and CreateDateTime> '{1}' and  CreateDateTime <='{2}' ;";

            string sqlExe = string.Format(sql, OriginalID, date.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss"), date.ToString("yyyy-MM-dd HH:mm:ss"));
            List<string> openids = new List<string>();
            using (var db = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd =db.CreateCommand(sqlExe))
                {
                    try
                    {
                       
                        var dt = db.ExecuteQuery(cmd, _ => _);
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow item in dt.Rows)
                            {
                                openids.Add(item["FromUserName"].ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("获取微信 text 记录异常  ex:" + ex.Message);
                    }
                }
            }
            

            return openids;
        }

    }
}
