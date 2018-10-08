using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.Discovery;

namespace Tuhu.Provisioning.DataAccess.DAO.Discovery
{
    public class SpecialColumnDAL
    {
        public static bool AddSpecialColumn(SqlConnection conn, SpecialColumn model)
        {
            SqlTransaction tran = conn.BeginTransaction();
            List<bool> res = new List<bool>();
            bool result = false;
            string insertSql1 = "INSERT INTO Marketing..tbl_SpecialColumn(ColumnName,ColumnDesc,ColumnImage,IsTop,IsShow,CreateTime,Creator,PublishTime) OUTPUT Inserted.ID VALUES(@ColumnName, @ColumnDesc, @ColumnImage, 0, @IsShow, @CreateTime, @Creator,@PublishTime)";
            string insertSql2 = "INSERT INTO Marketing..tbl_ColumnArticle(PKID,SCID,CreateTime)VALUES(@PKID,@SCID,@CreateTime)";
            try
            {
                var sqlParamsInfo = new SqlParameter[]
                {
                    new SqlParameter("@ColumnName",model.ColumnName),
                    new SqlParameter("@ColumnDesc",model.ColumnDesc),
                    new SqlParameter("@ColumnImage",model.ColumnImage),
                    new SqlParameter("@IsShow",model.IsShow),
                    new SqlParameter("@CreateTime",model.CreateTime),
                    new SqlParameter("@Creator",model.Creator),
                    new SqlParameter("@PublishTime",model.PublishTime)
                };
                object obj = SqlHelper.ExecuteScalar(tran, CommandType.Text, insertSql1, sqlParamsInfo);
                int columnId = Convert.ToInt32(obj);
                foreach (var article in model.Articles)
                {
                    var param = new SqlParameter[] {
                        new SqlParameter("@PKID",article.PKID),
                        new SqlParameter("@SCID",columnId),
                        new SqlParameter("@CreateTime",DateTime.Now)
                    };
                    res.Add(SqlHelper.ExecuteNonQuery(tran, CommandType.Text, insertSql2, param) > 0);
                }
                if (res.Contains(false))
                {
                    tran.Rollback();
                }
                else
                {
                    result = true;
                    tran.Commit();
                }
                    
            }
            catch(Exception ex)
            {
                tran.Rollback();
            }
            conn.Close();
            return result;
        }


        public static bool UpdateSpecialColumn(SqlConnection conn, SpecialColumn model)
        {
            SqlTransaction tran = conn.BeginTransaction();
            List<bool> res = new List<bool>();
            bool result = false;
            string updateSql = "UPDATE Marketing..tbl_SpecialColumn SET ColumnName=@ColumnName,ColumnDesc=@ColumnDesc,ColumnImage=@ColumnImage,IsShow=@IsShow,PublishTime=@PublishTime WHERE ID=@ID";
            string deleteSql = "DELETE FROM Marketing..tbl_ColumnArticle WHERE SCID=" + model.ID;
            string insertSql = "INSERT INTO Marketing..tbl_ColumnArticle(PKID,SCID,CreateTime)VALUES(@PKID,@SCID,@CreateTime)";
            try
            {
                var sqlParamsInfo = new SqlParameter[]
                {
                    new SqlParameter("@ColumnName",model.ColumnName),
                    new SqlParameter("@ColumnDesc",model.ColumnDesc),
                    new SqlParameter("@ColumnImage",model.ColumnImage),
                    new SqlParameter("@IsShow",model.IsShow),
                    new SqlParameter("@PublishTime",model.PublishTime),
                    new SqlParameter("@ID",model.ID)
                };
                res.Add(SqlHelper.ExecuteNonQuery(tran, CommandType.Text, updateSql, sqlParamsInfo) > 0);
                res.Add(SqlHelper.ExecuteNonQuery(tran, CommandType.Text, deleteSql) > 0);
                foreach (var article in model.Articles)
                {
                    var param = new SqlParameter[] {
                        new SqlParameter("@PKID",article.PKID),
                        new SqlParameter("@SCID",model.ID),
                        new SqlParameter("@CreateTime",DateTime.Now)
                    };
                    res.Add(SqlHelper.ExecuteNonQuery(tran, CommandType.Text, insertSql, param) > 0);
                }
                if (res.Contains(false))
                {
                    tran.Rollback();
                }
                else
                {
                    result = true;
                    tran.Commit();
                }

            }
            catch
            {
                tran.Rollback();
            }
            return result;
        }

        public static bool UpdateIsShow(SqlConnection conn, int id,bool isShow)
        {
            using (conn)
            {
                string updateSql = "UPDATE Marketing..tbl_SpecialColumn SET IsShow=@IsShow WHERE ID=@ID";
                var sqlParamsInfo = new SqlParameter[]
                  {
                        new SqlParameter("@IsShow",isShow),
                        new SqlParameter("@ID",id)
                  };

                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, updateSql, sqlParamsInfo) > 0;
            }
        }

        public static DataTable SelectColumnList(SqlConnection connection, PagerModel pager, string strWhere)
        {
            using (connection)
            {
                object totalCount = SqlHelper.ExecuteScalar(connection, CommandType.Text, "SELECT COUNT(1) FROM Marketing..tbl_SpecialColumn(NOLOCK) A WHERE 1=1 " + strWhere);
                pager.TotalItem = Convert.ToInt32(totalCount);
                string sql = @"
                                    SELECT * FROM
                                    (
                                        SELECT A.*,ROW_NUMBER() OVER(ORDER BY A.ID DESC) AS RowNum
                                        FROM Marketing..tbl_SpecialColumn(NOLOCK) A
                                        WHERE 1=1  {0}
                                    ) AS T
                                    WHERE T.RowNum BETWEEN (@PageIndex-1)*@PageSize+1 AND @PageIndex*@PageSize 
                                    ORDER BY PublishTime DESC ";
                var sqlParams = new[] {
                    new SqlParameter("@PageIndex",pager.CurrentPage),
                    new SqlParameter("@PageSize",pager.PageSize)
                 };
                return SqlHelper.ExecuteDataTable(connection, CommandType.Text, string.Format(sql, strWhere), sqlParams);
            }
        }

        public static DataRow SelectSpecialColumnByID(SqlConnection connection, int ID)
        {
            using (connection)
            {
                return SqlHelper.ExecuteDataRow(connection, CommandType.Text, @"SELECT TOP 1 * FROM Marketing..tbl_SpecialColumn(NOLOCK) WHERE ID=@ID", new SqlParameter("@ID", ID));
            }
        }

        public static DataTable SelectColumnArticleBySql(SqlConnection connection, string strWhere)
        {
            using (connection)
            {
                string script = "SELECT ID, PKID, SCID, CreateTime FROM Marketing..tbl_ColumnArticle(NOLOCK) WHERE 1=1 " + strWhere;
                return SqlHelper.ExecuteDataTable(connection, CommandType.Text, script);
            }
        }

    }
}
