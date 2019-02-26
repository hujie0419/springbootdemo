using Dapper;
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
    /// 数据访问-SE_DictionaryConfigDAL   
    /// </summary>
    public class SE_DictionaryConfigDAL
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("SE_DictionaryConfigDAL");
        public static IEnumerable<SE_DictionaryConfigModel> SelectPages(SqlConnection connection, int pageIndex = 1, int pageSize = 20, string strWhere = "")
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT * FROM (
                               SELECT ROW_NUMBER() OVER(ORDER BY Id desc) AS 'RowNumber'
                               ,'TotalCount' = (SELECT COUNT(1) FROM Configuration.dbo.SE_DictionaryConfig WITH(NOLOCK) WHERE 1=1 {0})
                               ,* FROM Configuration.dbo.SE_DictionaryConfig WITH(NOLOCK) WHERE 1=1 {0}
                               ) AS tab1 
                               WHERE tab1.RowNumber between ((@pageIndex - 1)* @pageSize) + 1 AND @pageIndex * @pageSize ";

                if (!string.IsNullOrWhiteSpace(strWhere))
                    sql = string.Format(sql, strWhere);
                else
                    sql = string.Format(sql, "", "");

                return conn.Query<SE_DictionaryConfigModel>(sql, new { pageIndex = pageIndex, pageSize = pageSize });
            }
        }

        public static SE_DictionaryConfigModel GetEntity(SqlConnection connection, int ParentId)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT TOP 1 * FROM Configuration.dbo.SE_DictionaryConfig WITH(NOLOCK) WHERE ParentId = @ParentId ";
                return conn.Query<SE_DictionaryConfigModel>(sql, new { ParentId = ParentId })?.FirstOrDefault();
            }
        }

        public static bool Insert(SqlConnection connection, SE_DictionaryConfigModel model)
        {
            try
            {
                using (IDbConnection conn = connection)
                {
                    string sql = @" 
                                INSERT INTO Configuration.dbo.SE_DictionaryConfig
								(
									ParentId,
									[Key],
									[Value],
									Describe,
									Sort,
									State,
									Url,
									Images,
									CreateTime,
									UpdateTime,
									Extend1,
									Extend2,
									Extend3,
									Extend4,
									Extend5
								)
                                VALUES
                                (
									@ParentId,
									@Key,
									@Value,
									@Describe,
									@Sort,
									@State,
									@Url,
									@Images,
									@CreateTime,
									@UpdateTime,
									@Extend1,
									@Extend2,
									@Extend3,
									@Extend4,
									@Extend5
								)";
                    return conn.Execute(sql, model) > 0;
                }

            }
            catch (Exception ex)
            {

                Logger.Log(Level.Info, $"赠品sql执行错误{ex}-{ex.InnerException}-{ex.StackTrace}");
                Logger.Log(Level.Error, $"赠品sql执行错误{ex}-{ex.InnerException}-{ex.StackTrace}");
                throw ex;


            }

        }

        public static bool Update(SqlConnection connection, SE_DictionaryConfigModel model)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @" UPDATE  Configuration.dbo.SE_DictionaryConfig
                                SET	ParentId = @ParentId,
									[Key] = @Key,
									[Value] = @Value,
									Describe = @Describe,
									Sort = @Sort,
									State = @State,
									Url = @Url,
									Images = @Images,
									CreateTime = @CreateTime,
									UpdateTime = @UpdateTime,
									Extend1 = @Extend1,
									Extend2 = @Extend2,
									Extend3 = @Extend3,
									Extend4 = @Extend4,
									Extend5 = @Extend5
								WHERE Id = @Id ";
                return conn.Execute(sql, model) > 0;
            }
        }

        public static bool Delete(SqlConnection connection, int Id)
        {
            using (IDbConnection conn = connection)
            {
                string sql = " DELETE Configuration.dbo.SE_DictionaryConfig WHERE Id = @Id ";
                return conn.Execute(sql, new { Id = Id }) > 0;
            }
        }
    }
}
