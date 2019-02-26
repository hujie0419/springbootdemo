using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class BlackListConfigDAL
    {
        public static IEnumerable<BlackListConfigModel> GetList(SqlConnection sqlconn, string userId)
        {
            string strSql = " SELECT * FROM Configuration..[SE_BlackListConfig] WITH(NOLOCK) ";

            List<SqlParameter> sqlparams = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(userId))
            {
                strSql += "  WHERE UserId = @UserId ";
                sqlparams.Add(new SqlParameter("@UserId", userId));
            }

            return SqlHelper.ExecuteDataTable(sqlconn, CommandType.Text, strSql.ToString(), (sqlparams.Count > 0 ? sqlparams.ToArray() : null)).ConvertTo<BlackListConfigModel>();
        }

        public static bool Add(SqlConnection sqlconn, string userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                string strSql = " INSERT INTO Configuration..[SE_BlackListConfig] ([UserId],[State],[CreateTime]) VALUES(@UserId, @STATE, @CreateTime) ";
                List<SqlParameter> sqlparams = new List<SqlParameter>();
                sqlparams.Add(new SqlParameter("@UserId",userId));
                sqlparams.Add(new SqlParameter("@STATE",1));
                sqlparams.Add(new SqlParameter("@CreateTime", DateTime.Now));
                return SqlHelper.ExecuteNonQuery(sqlconn, CommandType.Text, strSql.ToString(), sqlparams.ToArray()) > 0;
            }
            return false;
        }

        public static bool AddOnlyUserID(SqlConnection sqlconn, string userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                string strSql = @" 
                 IF((SELECT COUNT(UserId) FROM Configuration..SE_BlackListConfig WITH(NOLOCK) WHERE  UserId = @UserId) <= 0)
	                 BEGIN
                         INSERT INTO Configuration..[SE_BlackListConfig] ([UserId],[State],[CreateTime]) VALUES(@UserId, @STATE, @CreateTime)
                     END";
                List<SqlParameter> sqlparams = new List<SqlParameter>();
                sqlparams.Add(new SqlParameter("@UserId", userId));
                sqlparams.Add(new SqlParameter("@STATE", 1));
                sqlparams.Add(new SqlParameter("@CreateTime", DateTime.Now));
                return SqlHelper.ExecuteNonQuery(sqlconn, CommandType.Text, strSql.ToString(), sqlparams.ToArray()) > 0;
            }
            return false;
        }

        public static bool Delete(SqlConnection sqlconn, int id)
        {
            if (id > 0)
            {
                string strSql = " DELETE Configuration..[SE_BlackListConfig] WHERE Id =  @id ";
                return SqlHelper.ExecuteNonQuery(sqlconn, CommandType.Text, strSql.ToString(), new SqlParameter("@id", id)) > 0;
            }
            return false;
        }
    }
}
