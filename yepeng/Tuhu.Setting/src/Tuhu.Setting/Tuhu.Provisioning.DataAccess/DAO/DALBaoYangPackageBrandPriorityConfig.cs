using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALBaoYangPackageBrandPriorityConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);

        public static List<BaoYangPackageBrandPriorityConfig> GetBaoYangPackageBrandPriorityConfigList(string brand, string vehicle, string data, int pageSize, int pageIndex, out int recordCount, int startPrice = 0, int endPrice = 0)
        {
            string sqlStr = "";
            if (!string.IsNullOrWhiteSpace(brand))
            {
                sqlStr += " AND R.Brand=@Brand";
            }
            if (!string.IsNullOrWhiteSpace(vehicle))
            {
                sqlStr += " AND R.[VehicleID]=@Vehicle";
            }
            if (!string.IsNullOrWhiteSpace(data))
            {
                sqlStr += " AND R.[BVehicleID] IS NOT  NULL ";
            }

            if (startPrice > 0)
            {
                sqlStr += "   AND R.MinPrice >= @StartPrice ";

            }
            if (endPrice > 0)
            {
                sqlStr += " AND R.MinPrice <= @EndPrice ";

            }
            string sql = @"SELECT * FROM( SELECT B.[VehicleID] AS BVehicleID , 
                                                V.[VehicleID] ,
                                                V.[Brand] ,
                                                V.[Vehicle] ,
                                                B.[Id] ,
                                                B.[PackageBrands] ,
                                                B.[JiYouGrade],
                                                B.[CreateTime] ,
                                                B.[UpdateTime],
	                                            ( SELECT TOP 1
                                                            MinPrice
                                                  FROM      Gungnir..tbl_Vehicle_Type_Timing
                                                            WITH ( NOLOCK )
                                                  WHERE     V.VehicleID = VehicleID
                                                            AND MinPrice IS NOT NULL
                                                  ORDER BY  MinPrice
                                                ) AS MinPrice
                                        FROM    Gungnir..vw_Vehicle_Type AS V WITH ( NOLOCK )
                                                LEFT JOIN [Configuration].[dbo].[SE_BaoYangPackageBrandPriorityConfig]
                                                AS B WITH ( NOLOCK ) ON V.VehicleID = B.VehicleID  COLLATE Chinese_PRC_CI_AS
                                        ) AS R
                            WHERE 1=1   " + sqlStr + @"
                            ORDER BY R.CreateTime DESC
                                    OFFSET ( @PageIndex - 1 ) * @PageSize  ROWS  FETCH NEXT  @PageSize ROWS ONLY";
            string sqlCount = @"SELECT Count(*) FROM( SELECT 
                                                B.[VehicleID] AS BVehicleID ,
                                                V.[VehicleID] ,
                                                V.[Brand] ,
                                                V.[Vehicle] ,
                                                B.[Id] ,
                                                B.[PackageBrands] ,
                                                B.[JiYouGrade],
                                                B.[CreateTime] ,
                                                B.[UpdateTime],
	                                            ( SELECT TOP 1
                                                            MinPrice
                                                  FROM      Gungnir..tbl_Vehicle_Type_Timing
                                                            WITH ( NOLOCK )
                                                  WHERE     V.VehicleID = VehicleID
                                                            AND MinPrice IS NOT NULL
                                                  ORDER BY  MinPrice
                                                ) AS MinPrice
                                        FROM    Gungnir..vw_Vehicle_Type AS V WITH ( NOLOCK )
                                                LEFT JOIN [Configuration].[dbo].[SE_BaoYangPackageBrandPriorityConfig]
                                                AS B WITH ( NOLOCK ) ON V.VehicleID = B.VehicleID  COLLATE Chinese_PRC_CI_AS
                                        ) AS R
                            WHERE 1=1  " + sqlStr;
            var sqlParameters = new SqlParameter[]
              {
                    new SqlParameter("@Brand",brand),
                    new SqlParameter("@Vehicle",vehicle),
                    new SqlParameter("@PageSize",pageSize),
                    new SqlParameter("@PageIndex",pageIndex),
                    new SqlParameter("@StartPrice",startPrice),
                    new SqlParameter("@EndPrice",endPrice)
              };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount, sqlParameters);

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<BaoYangPackageBrandPriorityConfig>().ToList();

        }


        public static BaoYangPackageBrandPriorityConfig GetBaoYangPackageBrandPriorityConfig(int id)
        {
            const string sql = @"SELECT [Id]
                                      ,[VehicleID]                                    
                                      ,[PackageBrands]
                                      ,[JiYouGrade]
                                      ,[CreateTime]
                                      ,[UpdateTime]
                              FROM [Configuration].[dbo].[SE_BaoYangPackageBrandPriorityConfig] WITH (NOLOCK) WHERE Id=@id";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<BaoYangPackageBrandPriorityConfig>().ToList().FirstOrDefault();

        }

        public static bool InsertBaoYangPackageBrandPriorityConfig(BaoYangPackageBrandPriorityConfig model)
        {
            const string sql = @"INSERT INTO Configuration..SE_BaoYangPackageBrandPriorityConfig
                                          ( 
                                           [VehicleID]                                       
                                          ,[PackageBrands]
                                          ,[JiYouGrade]
                                          ,[CreateTime]
                                          ,[UpdateTime]
                                          )
                                  VALUES( 
                                           @VehicleID                                         
                                          ,@PackageBrands
                                          ,@JiYouGrade
                                          ,GETDATE()
                                          ,GETDATE()                                       
                                          )";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@VehicleID",model.VehicleID??string.Empty),                 
                    new SqlParameter("@PackageBrands",model.PackageBrands??string.Empty),
                    new SqlParameter("@JiYouGrade",model.JiYouGrade??string.Empty)

                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }


        public static bool UpdateBaoYangPackageBrandPriorityConfig(BaoYangPackageBrandPriorityConfig model)
        {
            const string sql = @"UPDATE Configuration..SE_BaoYangPackageBrandPriorityConfig SET                                      
                                           VehicleID=@VehicleID                                       
                                          ,PackageBrands=@PackageBrands      
                                          ,JiYouGrade=@JiYouGrade
                                          ,[UpdateTime]=GETDATE()                                        
                                WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@VehicleID",model.VehicleID??string.Empty),
                    new SqlParameter("@Brand",model.Brand??string.Empty),
                    new SqlParameter("@Vehicle",model.Vehicle??string.Empty),
                    new SqlParameter("@PackageBrands",model.PackageBrands??string.Empty),
                    new SqlParameter("@JiYouGrade",model.JiYouGrade??string.Empty),
                    new SqlParameter("@Id",model.Id)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool InsertOrUpdate(List<BaoYangPackageBrandPriorityConfig> list)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();
            try
            {
                foreach (var item in list)
                {
                    if (item.Id != 0)
                    {
                        UpdateBaoYangPackageBrandPriorityConfig(item);
                    }
                    else
                    {
                        InsertBaoYangPackageBrandPriorityConfig(item);
                    }

                }
                tran.Commit();
                return true;
            }
            catch (Exception)
            {
                tran.Rollback();
                return false;
                throw;
            }
            finally
            {
                conn.Dispose();
                tran.Dispose();
            }
        }

        public static bool DeleteBaoYangPackageBrandPriorityConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.SE_BaoYangPackageBrandPriorityConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

        /// <summary>
        /// 获取所有车型的品牌
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<string> GetAllVehicleBrands()
        {
            var result = new List<string>();

            const string sql = @"SELECT DISTINCT
                                        vw.Brand
                                FROM    vw_Vehicle_Type AS vw WITH(NOLOCK)
                                WHERE   vw.Brand IS NOT NULL
                                ORDER BY vw.Brand";
            using (var reader = SqlHelper.ExecuteReader(connOnRead, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    result.Add(reader.IsDBNull(0) ? string.Empty : reader.GetString(0));
                }
            }

            return result;
        }

        /// <summary>
        /// 根据选择的品牌该品牌的系列
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="brand"></param>
        /// <returns></returns>
        public static IDictionary<string, string> GetVehicleSeries(string brand)
        {
            var result = new Dictionary<string, string>();

            const string sql = @"   SELECT  ProductID ,
                                            vehicle.Vehicle
                                    FROM    Gungnir.dbo.tbl_Vehicle_Type AS vehicle WITH ( NOLOCK )
                                    WHERE   vehicle.Brand = @Brand
                                    ORDER BY Vehicle;";
            var parameters = new[]
            {
                new SqlParameter("@Brand", brand)
            };

            using (var reader = SqlHelper.ExecuteReader(conn, CommandType.Text, sql, parameters))
            {
                while (reader.Read())
                {

                    var key = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                    var value = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                    if (!result.ContainsKey(key))
                    {
                        result.Add(key, value);
                    }
                }
            }
            return result;
        }
    }
}
