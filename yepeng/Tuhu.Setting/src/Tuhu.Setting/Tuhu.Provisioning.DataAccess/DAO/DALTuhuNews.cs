using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALTuhuNews
    {

        public static List<tbl_TuhuNews> GetTuhuNews(SqlConnection conn,int pageNumber,int pageSize)
        {
            List<tbl_TuhuNews> list = new List<tbl_TuhuNews>();
            string sql = @"SELECT  Id ,
        Title ,
        NewsFrom ,
		IssueTime,
        NewsType,
        NewsGuid
FROM    Activity.[dbo].[tbl_TuhuNews] WITH ( NOLOCK )
ORDER BY IssueTime DESC ;";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<tbl_TuhuNews>().ToList(); ;
        }

        public static object InsertTuhuNews(SqlConnection conn,tbl_TuhuNews tn)
        {
            string sql = @"
                INSERT  INTO Activity..tbl_TuhuNews
                        ( Title ,
                          NewsFrom ,
                          IssueTime ,
                          NewsType ,
                          Content,
                          NewsGuid
                        )
                VALUES  ( @N_Title ,
                          @N_From ,
                          @N_IssueTime ,
                          @N_Type ,
                          @N_Content,
                          @N_Guid
                        );
            SELECT TOP 1 Id FROM  Activity.[dbo].[tbl_TuhuNews] WITH(NOLOCK)ORDER BY Id DESC
                    ";
            var sqlParams = new SqlParameter[]
                    {
                    new SqlParameter("@N_Title",tn.Title),
                    new SqlParameter("@N_From",tn.NewsFrom),
                    new SqlParameter("@N_IssueTime",tn.IssueTime),
                    new SqlParameter("@N_Type",tn.NewsType),
                    new SqlParameter("@N_Content",tn.Content),
                    new SqlParameter("@N_Guid",tn.NewsGuid)
                    };
            return SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParams);
        }

        public static int DeleteTuhuNews(SqlConnection conn,int N_Id)
        {
            string sql = "DELETE Activity.[dbo].[tbl_TuhuNews] WHERE Id=@N_Id";
            var sqlParam = new SqlParameter("@N_Id", N_Id);
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam);
        }

        public static int UpdateTuhuNews(SqlConnection conn, tbl_TuhuNews tn)
        {
            string sql = @"UPDATE  Activity.[dbo].[tbl_TuhuNews]
            SET     [Title] = @N_Title ,
                    [NewsFrom] = @N_From ,
                    [IssueTime] = @N_IssueTime ,
                    [NewsType] = @N_Type ,
                    [Content] = @N_Content
            WHERE   [Id] = @N_Id;
            ";

            var sqlParams = new SqlParameter[]
                    {
                    new SqlParameter("@N_Title",tn.Title),
                    new SqlParameter("@N_From",tn.NewsFrom),
                    new SqlParameter("@N_IssueTime",tn.IssueTime),
                    new SqlParameter("@N_Type",tn.NewsType),
                    new SqlParameter("@N_Content",tn.Content),
                    new SqlParameter("@N_Id",tn.Id)
                    };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParams);
        }


        public static tbl_TuhuNews SelectNewsbyID(SqlConnection conn,int N_Id)
        {
            string sql = "SELECT TOP 1 * FROM  Activity.[dbo].[tbl_TuhuNews] WHERE Id=@N_Id";
            var sqlParam=new SqlParameter("@N_Id", N_Id);
            DataTable dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam);
            if (dt != null && dt.Rows.Count > 0)
                return new tbl_TuhuNews(dt.Rows[0]);
            else
                return new tbl_TuhuNews();
            
        }

        public static tbl_TuhuNews SelectNewsbyID(SqlConnection conn)
        {
            string sql = "SELECT TOP 1 * FROM  Activity.[dbo].[tbl_TuhuNews] WITH(NOLOCK)ORDER BY Id DESC";
            
            DataTable dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, null);
            if (dt != null && dt.Rows.Count > 0)
                return new tbl_TuhuNews(dt.Rows[0]);
            else
                return new tbl_TuhuNews();

        }
    }
}
