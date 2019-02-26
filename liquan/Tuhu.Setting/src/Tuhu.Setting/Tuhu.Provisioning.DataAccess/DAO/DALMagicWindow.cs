using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Component.Common;
using System.Configuration;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALMagicWindow
    {

        public static int InsertMagicWindow(string url)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string sql = @"INSERT  INTO Configuration.dbo.tbl_MagicWindow( Url, IsEnable, CreateTime) VALUES  (@Url,1,GETDATE())";
            var sqlParams = new SqlParameter[]
                    {
                    new SqlParameter("@Url",url),
                    };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParams);
        }

        public static int UpdateMagicWindow(string url, int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string sql = @"UPDATE Configuration.dbo.tbl_MagicWindow SET Url=@url WHERE PKID=@pkid";

            var sqlParams = new SqlParameter[]
                   {
                    new SqlParameter("@url",url),
                    new SqlParameter("@pkid",pkid)
                   };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParams);
        }

        public static int DeleteMagicWindow(int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string sql = "DELETE Configuration.dbo.tbl_MagicWindow WHERE PKID=@pkid";
            var sqlParam = new SqlParameter("@pkid", pkid);
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam);
        }

        public static List<MagicWindowModel> fetchMagicWindow(int pageIndex, int pageSize)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            var sql = @"   SELECT  *,COUNT(1) OVER() AS Total
            FROM    Configuration.dbo.tbl_MagicWindow WITH ( NOLOCK )
            ORDER BY  PKID DESC
            OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize
            ROWS ONLY";
            var sqlParams = new SqlParameter[]
                   {
                    new SqlParameter("@PageIndex",pageIndex),
                    new SqlParameter("@PageSize",pageSize)
                   };
            return SqlHelper.ExecuteDataTable(new SqlConnection(conn), CommandType.Text, sql, sqlParams).ConvertTo<MagicWindowModel>().ToList();
        }
    }
}
