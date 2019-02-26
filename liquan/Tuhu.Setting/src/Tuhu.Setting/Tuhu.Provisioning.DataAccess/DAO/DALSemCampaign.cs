using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 新APP下载记录统计
    /// </summary>
    public class DALSemCampaign
    {
        /// <summary>
        /// 新增下载统计记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="adc"></param>
        /// <returns></returns>
        public static int CreateApp(SqlConnection conn, AppDownloadCount adc)
        {
            string insertSql = "INSERT Tuhu_sem.dbo.tbl_AppCampaign WITH(ROWLOCK) VALUES  ( @ArticleTitle ,@Channel ,@OtherChannel ,@AppPath, 0 ,@Creator ,GETDATE() , '' , NULL);SET @pkid=SCOPE_IDENTITY();";
            var para = new[] {
                new SqlParameter("@ArticleTitle", adc.ArticleTitle),
                new SqlParameter("@Channel", adc.Channel),
                new SqlParameter("@OtherChannel", adc.OtherChannel),
                new SqlParameter("@AppPath", adc.AppUrl),
                new SqlParameter("@Creator", adc.Creator),
                new SqlParameter("@pkid",DbType.Int32){Direction=ParameterDirection.Output}
            };
            SqlHelper.ExecuteNonQueryV2(conn, CommandType.Text, insertSql, para);
            return int.Parse(para.Last().Value.ToString());
        }
        /// <summary>
        /// 查询下载统计记录
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<AppDownloadCount> AppCampaignList(SqlConnection conn)
        {
            var list = new List<AppDownloadCount>();
            using (var reader = SqlHelper.ExecuteReaderV2(conn, CommandType.Text, @"SELECT a.*,ISNULL(b.DownloadCount,0) DownloadCount FROM Tuhu_sem.dbo.tbl_AppCampaign a WITH(NOLOCK) LEFT JOIN (SELECT CampaignID,COUNT(*) DownloadCount FROM Tuhu_sem.dbo.tbl_AppDownloadAction WITH(NOLOCK) GROUP BY CampaignID) b ON a.PKID=b.CampaignID where IsDeleted!=1"))
            {
                while (reader.Read())
                {
                    var app = new AppDownloadCount()
                    {
                        PKID = reader.GetTuhuValue<int>(0),
                        ArticleTitle = reader.GetTuhuString(1),
                        Channel = reader.GetTuhuString(2),
                        OtherChannel = reader.GetTuhuString(3),
                        AppUrl = reader.GetTuhuString(4),
                        IsDeleted = reader.GetTuhuValue<bool>(5).ToString(),
                        Creator = reader.GetTuhuString(6),
                        CreateDateTime = reader.GetTuhuValue<DateTime>(7),
                        Deletor = reader.GetTuhuString(8),
                        DeleteDateTime = reader.GetTuhuValue<DateTime>(9),
                        DownloadCount = reader.GetTuhuValue<int>(10).ToString()
                    };

                    list.Add(app);

                }
            }
            return list;

            //  return dt == null || dt.Rows.Count == 0 ? new List<AppDownloadCount>() : dt.c<AppDownloadCount>().ToList();
        }
        /// <summary>
        /// 根据ID查询具体的下载(IP,设备等)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<AppDownloadHistory> AppDownloadActionList(SqlConnection conn, int id)
        {
            List<AppDownloadHistory> ss = new List<AppDownloadHistory>();

            using (var reader = SqlHelper.ExecuteReaderV2(conn, CommandType.Text, @"SELECT UserAgent,IP,CreateDateTime FROM Tuhu_sem.dbo.tbl_AppDownloadAction WITH(NOLOCK) WHERE CampaignID=" + id))
            {
                while (reader.Read())
                {
                    var history = new AppDownloadHistory()
                    {
                        Machine = reader.GetTuhuString(0),
                        IP = reader.GetTuhuString(1),
                        DownloadTime = reader.GetTuhuValue<DateTime>(2).ToString()
                    };

                    ss.Add(history);
                }
            }

            return ss;
        }
        /// <summary>
        /// 删除某条下载渠道记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="deletor"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static int DeleteApp(SqlConnection conn, string deletor, int pkid)
        {
            string updateSql = "UPDATE Tuhu_sem.dbo.tbl_AppCampaign WITH(ROWLOCK) SET IsDeleted=1,DeleteDateTime=GETDATE(),Deletor= @Deletor where PKID=@PkId";
            var para = new[] {
                new SqlParameter("@Deletor", deletor),
                new SqlParameter("@PkId", pkid)
            };
            return SqlHelper.ExecuteNonQueryV2(conn, CommandType.Text, updateSql, para);
        }
        /// <summary>
        /// 新增下载渠道记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="url"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static int CreateOrDeleteApp(SqlConnection conn, string url, int pkid)
        {
            string sql;
            SqlParameter[] param;
            if (!string.IsNullOrEmpty(url.Trim()))
            {
                sql = "UPDATE Tuhu_sem.dbo.tbl_AppCampaign WITH(ROWLOCK) SET AppUrl = @Url where PKID=@PkId";
                param = new[]{
                    new SqlParameter("@Url",url),
                    new SqlParameter("@PkId",pkid)
                };
            }
            else   //如果没有URL，说明文件没有上传成功，直接删除记录，防止重复
            {
                sql = "Delete Tuhu_sem.dbo.tbl_AppCampaign WITH(ROWLOCK) where PKID=@PkId";
                param = new[]{
                    new SqlParameter("@PkId",pkid)
                };
            }
            return SqlHelper.ExecuteNonQueryV2(conn, CommandType.Text, sql, param);
        }
    }
}
