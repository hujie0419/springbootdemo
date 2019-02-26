using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class APPDAL
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(strConn);

        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);

        /// <summary>
        /// 获取app发布版本信息 分页
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static List<tbl_app_Versions> GetAppVersions(PagerModel pagerModel)
        {          
            List<tbl_app_Versions> lversion = new List<tbl_app_Versions>();
            string sql = @"SELECT   [PKID] ,
                                    [Version_Number],
                                    [CreateTime],
                                    [LastUpdateTime],
                                    [Download],
                                    [Download_Number],
                                    [UpdateConnent],
                                    [Size],
                                    [VersionCode],
                                    [MustUpdate]
                            FROM[Configuration].[dbo].[tbl_app_Versions] WITH(NOLOCK)
                            WHERE 1=1 
                            ORDER BY PKID DESC
                            OFFSET ( @pageIndex - 1 ) * @pageSize ROWS  FETCH NEXT @PageSize ROWS  ONLY 
                            ";

            SqlParameter[] pars = {  
                new SqlParameter("@pageIndex",pagerModel.CurrentPage),
                new SqlParameter("@pageSize",pagerModel.PageSize),                               
                                  };
         
            
            lversion = SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, pars).ConvertTo<tbl_app_Versions>().ToList();
            pagerModel.TotalItem = Convert.ToInt32(DbHelper.ExecuteScalar("select COUNT(1) from Configuration.dbo.tbl_app_Versions"));

            return lversion;
        }

        public static tbl_app_Versions load2(int PKID)
        {
            List<tbl_app_Versions> vers = new List<tbl_app_Versions>();
            string sql = @" SELECT  *
                                FROM Configuration.dbo.tbl_app_Versions WITH (NOLOCK)
                             WHERE   PKID = @PKID
                        ";

         List <SqlParameter> list = new List<SqlParameter>() { new SqlParameter("@PKID", PKID) };
            tbl_app_Versions tb = new tbl_app_Versions();
            DataTable dt = SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, list.ToArray());
            if (dt.Rows.Count > 0)
            {

                tb.Version_Number = dt.Rows[0]["Version_Number"].ToString();
                tb.VersionCode = dt.Rows[0]["VersionCode"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["VersionCode"]) : 0;
                tb.MustUpdate = dt.Rows[0]["MustUpdate"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["MustUpdate"]) : 0;
                tb.CreateTime = Convert.ToDateTime(dt.Rows[0]["CreateTime"]);
                tb.LastUpdateTime = Convert.ToDateTime(dt.Rows[0]["LastUpdateTime"]);
                tb.Download_Number = Convert.ToInt32(dt.Rows[0]["Download_Number"]);
                tb.Download = dt.Rows[0]["Download"].ToString();
                tb.UpdateConnent = dt.Rows[0]["UpdateConnent"].ToString();
                tb.PKID = Convert.ToInt32(dt.Rows[0]["PKID"]);
                vers.Add(tb);
            }
            return tb;
        }
        public static int upload(string Version_Number, string Download, string UpdateConnent, string Size, string versionCode, string mustUpdate)
        {
            string sql = @"
                        INSERT  INTO Configuration.dbo.tbl_app_Versions
                        VALUES  ( @Version_Number, GETDATE(), GETDATE(), @Download, 0,
                                  @UpdateConnent, @Size, @VersionCode, @MustUpdate )";
            var sqlparams = new SqlParameter[] {
                new SqlParameter("@Version_Number",Version_Number),
                new SqlParameter("@Download", Download),
                 new SqlParameter("@UpdateConnent", UpdateConnent),
                  new SqlParameter("@Size", Size),
                   new SqlParameter("@VersionCode", Convert.ToInt32(versionCode)),
                    new SqlParameter("@MustUpdate", Convert.ToInt32(mustUpdate)),
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlparams);


        }
        public static int upDate(tbl_app_Versions tb)
        {
            string sql = @"UPDATE  Configuration.dbo.tbl_app_Versions
                            SET     UpdateConnent = @UpdateConnent ,
                                    Version_Number = @Version_Number ,
                                    VersionCode = @VersionCode ,
                                    MustUpdate = @MustUpdate ,
                                    LastUpdateTime = GETDATE()
                            WHERE   PKID = @PKID";

            var sqlparams = new SqlParameter[] {
                new SqlParameter("@UpdateConnent", tb.UpdateConnent),
                new SqlParameter("@Version_Number", tb.Version_Number),
                 new SqlParameter("@MustUpdate", tb.MustUpdate),
                  new SqlParameter("@VersionCode", tb.VersionCode),
                    new SqlParameter("@PKID", tb.PKID),
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlparams);
        }

        public static int InsertTuhuDiTuiLog(string TuhuUserPhone, string UserPhone, string DeviceID, string Channal, string City, string UserType, string Versions)
        {
            string sql = @"INSERT INTO SystemLog..TuhuDiTuiInfo VALUES(@TuhuUserPhone,@UserPhone,@DeviceID,@Versions,@Channal,@City,@UserType,GETDATE())";

            var sqlparams = new SqlParameter[] {
                new SqlParameter("@TuhuUserPhone", TuhuUserPhone),
                new SqlParameter("@UserPhone", UserPhone),
                 new SqlParameter("@DeviceID", DeviceID),
                  new SqlParameter("@Versions", Versions),
                    new SqlParameter("@Channal", Channal),
                     new SqlParameter("@City", City),
                      new SqlParameter("@UserType", UserType),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlparams);
        }

        public static DataTable SelectInsuranceCompany()
        {
            const string sql = @"SELECT * FROM Configuration..InsuranceCompanyConfig (NOLOCK)";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql);
        }
    }
}