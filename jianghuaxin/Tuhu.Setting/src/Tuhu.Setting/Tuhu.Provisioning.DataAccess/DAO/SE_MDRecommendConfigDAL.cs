using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 数据访问-SE_MDRecommendConfigDAL   
    /// </summary>
    public partial class SE_MDRecommendConfigDAL
    {
        public static IEnumerable<SE_MDRecommendConfigModel> SelectPages(SqlConnection connection, int pageIndex = 1, int pageSize = 20, string strWhere = "")
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT * FROM (
                               SELECT ROW_NUMBER() OVER(ORDER BY Id) AS 'RowNumber'
                               ,'TotalCount' = (SELECT COUNT(1) FROM Tuhu_Groupon.dbo.SE_MDRecommendConfig WITH(NOLOCK) WHERE 1=1 {0})
                               ,* FROM Tuhu_Groupon.dbo.SE_MDRecommendConfig WITH(NOLOCK) WHERE 1=1 {0}
                               ) AS tab1 
                               WHERE tab1.RowNumber between ((@pageIndex - 1)* @pageSize) + 1 AND @pageIndex * @pageSize ";

                if (!string.IsNullOrWhiteSpace(strWhere))
                    sql = string.Format(sql, strWhere);
                else
                    sql = string.Format(sql, "", "");

                return conn.Query<SE_MDRecommendConfigModel>(sql, new { pageIndex = pageIndex, pageSize = pageSize });
            }
        }


        public static SE_MDRecommendConfigModel GetProducts(SqlConnection connection, int part, int type, int vtype)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT TOP 1 * FROM Tuhu_Groupon.dbo.SE_MDRecommendConfig WITH(NOLOCK) 
                            WHERE Type=@Type AND VehicleType=@VehicleType AND PartId=@PartId ";

                return conn.Query<SE_MDRecommendConfigModel>(sql, new { PartId = part, VehicleType = vtype, Type = type }).FirstOrDefault();
            }
        }

        public static SE_MDRecommendConfigModel GetEntity(SqlConnection connection, int Id)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @"SELECT TOP 1 * FROM Tuhu_Groupon.dbo.SE_MDRecommendConfig WITH(NOLOCK) WHERE Id = @Id ";
                return conn.Query<SE_MDRecommendConfigModel>(sql, new { Id = Id })?.FirstOrDefault();
            }
        }

        public static bool Insert(SqlConnection connection, SE_MDRecommendConfigModel model)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @" 
                                INSERT INTO Tuhu_Groupon.dbo.SE_MDRecommendConfig
								(
									PartId,
									Type,
									VehicleType,
									IsDisable,
                                    Products,
									CreateTime
								)
                                VALUES
                                (
									@PartId,
									@Type,
									@VehicleType,
									@IsDisable,
                                    @Products,
									@CreateTime
								)";
                return conn.Execute(sql, model) > 0;
            }
        }

        public static bool Update(SqlConnection connection, SE_MDRecommendConfigModel model)
        {
            using (IDbConnection conn = connection)
            {
                string sql = @" UPDATE  Tuhu_Groupon.dbo.SE_MDRecommendConfig
                                SET	PartId = @PartId,
									Type = @Type,
									VehicleType = @VehicleType,
									IsDisable = @IsDisable,
                                    Products = @Products,
									CreateTime = @CreateTime
								WHERE Id = @Id ";
                return conn.Execute(sql, model) > 0;
            }
        }

        public static bool Delete(SqlConnection connection, int Id)
        {
            using (IDbConnection conn = connection)
            {
                string sql = " DELETE Tuhu_Groupon.dbo.SE_MDRecommendConfig WHERE Id = @Id ";
                return conn.Execute(sql, new { Id = Id }) > 0;
            }
        }

        public static string IsExistsPId(SqlConnection connection, string pid)
        {
            using (IDbConnection conn = connection)
            {
                string sql = " SELECT top 1 DisplayName FROM Tuhu_productcatalog.dbo.vw_Products WITH(NOLOCK) WHERE PId = @PId ";
                return conn.Query<string>(sql, new { PId = pid })?.FirstOrDefault() ?? "";
            }
        }


    }
}
