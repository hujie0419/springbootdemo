using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 数据访问-SE_MDBeautyPartConfigDAL   
    /// </summary>
    public partial class SE_MDBeautyPartConfigDAL
    {
        public static IEnumerable<SE_MDBeautyPartConfigModel> SelectPages(SqlConnection connection, int pageIndex = 1, int pageSize = 20, string strWhere = "")
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT * FROM (
                               SELECT ROW_NUMBER() OVER(ORDER BY Soft desc) AS 'RowNumber'
                               ,'TotalCount' = (SELECT COUNT(1) FROM SE_MDBeautyPartConfig WITH(NOLOCK) WHERE 1=1 {0})
                               ,* FROM SE_MDBeautyPartConfig WITH(NOLOCK) {0}
                               ) AS tab1 
                               WHERE tab1.RowNumber between ((@pageIndex - 1)* @pageSize) + 1 AND @pageIndex * @pageSize ";

                if (!string.IsNullOrWhiteSpace(strWhere))
                    sql = string.Format(sql, strWhere);
                else
                    sql = string.Format(sql, "", "");

                return conn.Query<SE_MDBeautyPartConfigModel>(sql, new { pageIndex = pageIndex, pageSize = pageSize });
            }
        }

        public static SE_MDBeautyPartConfigModel Select(SqlConnection connection, int Id)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT TOP 1 * FROM SE_MDBeautyPartConfig WITH(NOLOCK) WHERE Id = @Id ";
                return conn.Query<SE_MDBeautyPartConfigModel>(sql, new { Id = Id })?.FirstOrDefault();
            }
        }

        public static bool Insert(SqlConnection connection, SE_MDBeautyPartConfigModel model)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @" 
                                INSERT INTO SE_MDBeautyPartConfig
								(
									Name,
									InteriorCategorys,
									ExternalCategorys,
                                    H5URL,
									Soft,
									IsShow,
                                    ExcludePids
								)
                                VALUES
                                (
									@Name,
									@InteriorCategorys,
									@ExternalCategorys,
                                    @H5URL,
									@Soft,
									@IsShow,
                                    @ExcludePids
								)";
                return conn.Execute(sql, model) > 0;
            }
        }

        public static bool Update(SqlConnection connection, SE_MDBeautyPartConfigModel model)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @" UPDATE  SE_MDBeautyPartConfig
                                SET	Name = @Name,
									InteriorCategorys = @InteriorCategorys,
									ExternalCategorys = @ExternalCategorys,
                                    H5URL = @H5URL,
									Soft = @Soft,
									IsShow = @IsShow,
                                    ExcludePids=@ExcludePids 
								WHERE Id = @Id ";
                return conn.Execute(sql, model) > 0;
            }
        }

        public static bool Delete(SqlConnection connection, int Id)
        {
            using (IDbConnection conn = connection)
            {
                string sql = " DELETE SE_MDBeautyPartConfig WHERE Id = @Id ";
                return conn.Execute(sql, new { Id = Id }) > 0;
            }
        }
    }
}