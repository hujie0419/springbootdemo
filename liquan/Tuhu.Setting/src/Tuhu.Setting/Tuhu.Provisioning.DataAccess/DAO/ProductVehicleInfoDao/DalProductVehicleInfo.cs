using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common;
using System.Transactions;
using System.Collections;
using System.Data.Common;
using System.Diagnostics;
using Tuhu.Component.Framework;
using System.Configuration;

namespace Tuhu.Provisioning.DataAccess.DAO.ProductVehicleInfoDao
{
    public static class DalProductVehicleInfo
    {
        public static List<ProductInfo> SearchProductInfoByParam(SqlConnection conn, string pidOrDisplayName, int pageIndex, int pageSize, int type)
        {
            const string sql = @"DECLARE @Total INT= 0;
            SELECT  @Total = COUNT(1)
            FROM    Tuhu_productcatalog..[CarPAR_zh-CN] (NOLOCK) AS cpzc
            WHERE   ( @Type = -1
                      OR ( @Type = 3
                           AND ( cpzc.CP_Remark IS NULL
                                 OR cpzc.CP_Remark = N'无需车型'
                               )
                         )
                    )
                    AND ( cpzc.PID = @Pid
                          OR cpzc.DisplayName LIKE @DisplayName
                        )
                    AND ( cpzc.i_ClassType = 2
                          OR cpzc.i_ClassType = 4
                        );
            SELECT  cpzc.DisplayName ,
                    cpzc.PID ,
                    cpzc.Brand ,
                    cpzc.CP_Remark ,
                    @Total AS Total
            FROM    Tuhu_productcatalog..[CarPAR_zh-CN] (NOLOCK) AS cpzc
            WHERE   ( @Type = -1
                      OR ( @Type = 3
                           AND ( cpzc.CP_Remark IS NULL
                                 OR cpzc.CP_Remark = N'无需车型'
                               )
                         )
                    )
                    AND ( cpzc.PID = @Pid
                          OR cpzc.DisplayName LIKE @DisplayName
                        )
                    AND ( cpzc.i_ClassType = 2
                          OR cpzc.i_ClassType = 4
                        )
            ORDER BY cpzc.PID
                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                    ONLY;";

            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@Pid", pidOrDisplayName),
                new SqlParameter("@DisplayName",$"%{pidOrDisplayName}%"),
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@Type",type)
            };

            var resultList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParams).ConvertTo<ProductInfo>().ToList();
            return resultList;
        }

        public static List<ProductInfo> SelectProductConfigInfoByPIDs(SqlConnection conn, List<string> pidList)
        {
            var sql = @"SELECT  pvtc.PID ,
                ( SELECT TOP 1
                            t.UpdateTime
                  FROM      Tuhu_productcatalog..ProductVehicleTypeConfig (NOLOCK) AS t
                  WHERE     pvtc.PID = t.PID
                ) AS UpdateTime
        FROM    Tuhu_productcatalog..ProductVehicleTypeConfig (NOLOCK) AS pvtc
                JOIN Tuhu_productcatalog..SplitString(@PIDStr, ',', 1) AS s ON s.Item = pvtc.PID
        GROUP BY pvtc.PID ,
                pvtc.UpdateTime;";
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@PIDStr", String.Join(",",pidList))
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParams).ConvertTo<ProductInfo>().ToList();
        }

        public static List<ProductInfo> SelectProductInfo(SqlConnection conn, string pidOrDisplayName, int pageIndex, int pageSize, int type)
        {
            var sql = @" SELECT  * ,
                COUNT(1) OVER ( ) AS total
        FROM    ( SELECT DISTINCT
                            cpzc.DisplayName ,
                            cpzc.PID ,
                            cpzc.Brand ,
                            cpzc.CP_Remark
                  FROM      Tuhu_productcatalog..[CarPAR_zh-CN] (NOLOCK) AS cpzc
                            LEFT JOIN Tuhu_productcatalog..ProductVehicleTypeConfig (NOLOCK)
                            AS pvtc ON cpzc.PID = pvtc.PID
                  WHERE     ( cpzc.PID = @Pid
                              OR cpzc.DisplayName LIKE @DisplayName
                            )
                            AND ( cpzc.i_ClassType = 2
                                  OR cpzc.i_ClassType = 4
                                )
                            AND cpzc.CP_Remark <> ''
                            AND cpzc.CP_Remark IS NOT NULL
                            AND cpzc.CP_Remark <> N'无需车型'
                            AND ( ( @Type = 1
                                    AND pvtc.UpdateTime IS NOT NULL
                                  )
                                  OR ( @Type = 2
                                       AND pvtc.UpdateTime IS NULL
                                     )
                                )
                ) AS t
        ORDER BY t.PID
                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                ONLY;";
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@Pid", pidOrDisplayName),
                new SqlParameter("@DisplayName",$"%{pidOrDisplayName}%"),
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@Type",type)
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParams).ConvertTo<ProductInfo>().ToList();
        }

        public static ProductInfo GetProductInfoByPid(SqlConnection conn, string pid)
        {
            var sql = @"SELECT  cpzc.PID ,
        cpzc.DisplayName ,
        cpzc.ProductCode ,
        cpzc.Brand ,
        cpzc.cy_list_price AS ListPrice ,
        cpzc.cy_marketing_price AS MarketingPrice ,
        cpzc.CP_ShuXing5 ,
        cpzc.CP_Remark ,
		ISNULL(pc.IsAutoAssociate, 0) AS IsAutoAssociate,
         VehicleLevel
        FROM  [CarPAR_zh-CN] (NOLOCK) AS cpzc
        LEFT JOIN tbl_ProductConfig (NOLOCK) AS pc
		ON cpzc.PID = pc.ProductPID
        WHERE cpzc.PID=@Pid";
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@Pid", pid)
            };

            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParams).ConvertTo<ProductInfo>().FirstOrDefault();
            return result;
        }

        public static List<ProductVehicleTypeFileInfoDb> GetExcelInfoByPid(SqlConnection conn, string pid)
        {
            var sql = @"SELECT pvtfi.PID,pvtfi.Operator,pvtfi.FilePath,pvtfi.CreatedTime FROM ProductVehicleTypeFileInfo(NOLOCK) AS pvtfi WHERE pvtfi.PID=@Pid";

            var sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@Pid",pid)
                };

            var resultList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParams).ConvertTo<ProductVehicleTypeFileInfoDb>().ToList();
            return resultList;
        }

        public static List<ProductInfo> GetProductInfoByPids(SqlConnection conn, string pids)
        {
            var pidList = pids.TrimEnd(',').Split(',').ToList();
            var paramList = new List<SqlParameter>();
            var tt = NormalizeInParam(pidList, paramList);
            var sql = string.Format("SELECT cpzc.DisplayName,cpzc.PID,cpzc.Brand FROM [CarPAR_zh-CN](NOLOCK) AS cpzc WHERE cpzc.PID IN ({0})", tt);

            var resultList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, paramList.ToArray()).ConvertTo<ProductInfo>().ToList();
            return resultList;
        }

        //        public static DataTable GetDtForProductInfoByPids(SqlConnection conn, string[] pids)
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            foreach (var item in pids)
        //            {
        //                sb.AppendFormat("'{0}',", item);
        //            }
        //            var paramPids = sb.ToString().TrimEnd(',');

        //            var sql = @"SELECT pvtc.PID,pvtc.TID,pvtc.VehicleID FROM ProductVehicleTypeConfig(nolock) AS pvtc
        //WHERE pvtc.PID IN (" + paramPids + ")";

        //            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
        //            return dt;
        //        }

        public static DataTable GetDtForProductInfoByPid(SqlConnection conn, string[] pids)
        {
            var pidList = pids.ToList();
            var paramList = new List<SqlParameter>();
            var tt = NormalizeInParam(pidList, paramList);
            var sql = string.Format("SELECT cpzcc.#Catalog_Lang_Oid, cpzcc.CP_Remark FROM[CarPAR_zh-CN_Catalog] AS cpzcc WHERE cpzcc.#Catalog_Lang_Oid IN(SELECT cpzc.oid FROM [CarPAR_zh-CN](NOLOCK) AS cpzc WHERE cpzc.PID IN({0}))", tt);

            //            StringBuilder sb = new StringBuilder();
            //            foreach (var item in pids)
            //            {
            //                sb.AppendFormat("'{0}',", item);
            //            }
            //            var paramPids = sb.ToString().TrimEnd(',');

            //            var sql = @"SELECT cpzcc.#Catalog_Lang_Oid, cpzcc.CP_Remark
            //  FROM [CarPAR_zh-CN_Catalog] AS cpzcc
            //WHERE cpzcc.#Catalog_Lang_Oid IN(SELECT cpzc.oid FROM [CarPAR_zh-CN](NOLOCK) AS cpzc
            //WHERE cpzc.PID IN (" + paramPids + "))";

            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, paramList.ToArray());
            return dt;
        }


        public static bool SaveProductInfoByPid(SqlConnection conn, string pid, string cpremark, int isAutometic, string vehicleLevel)
        {
            string connectString = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;

            SqlConnection sqlCnt = new SqlConnection(connectString);
            sqlCnt.Open();

            using (TransactionScope scope = new TransactionScope())
            {
                var sql = @"Update Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] SET CP_Remark = @CPRemark WHERE #Catalog_Lang_Oid IN ( SELECT cpzc.oid FROM Tuhu_productcatalog..[CarPAR_zh-CN] AS cpzc WHERE cpzc.PID=@Pid)";
                var sqlupdate = @"IF EXISTS (SELECT 1 FROM Tuhu_productcatalog..tbl_ProductConfig (NOLOCK)
                                   WHERE ProductPID = @Pid)
                                   BEGIN
                                        UPDATE Tuhu_productcatalog..tbl_ProductConfig
                                        SET IsAutoAssociate = @IsAutometic,VehicleLevel=@VehicleLevel
                                        WHERE ProductPID = @Pid
                                   END
                                   ELSE
                                   BEGIN
                                         INSERT INTO Tuhu_productcatalog..tbl_ProductConfig
                                         ( ProductPID, IsAutoAssociate,VehicleLevel, CreateTime, UpdateTime )
                                         VALUES  ( @Pid, @IsAutometic,@VehicleLevel, GETDATE(), GETDATE() ) 
                                   END";
                var sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@Pid",pid),
                    new SqlParameter("@CPRemark",cpremark)
                };
                var sqlParamForUdp = new SqlParameter[]
                {
                    new SqlParameter("@Pid",pid),
                    new SqlParameter("@IsAutometic",isAutometic),
                    new SqlParameter("@VehicleLevel",vehicleLevel)
                };

                int cpremarkUpdateCount = Convert.ToInt32(SqlHelper.ExecuteNonQuery(sqlCnt, CommandType.Text, sql, sqlParams));
                int isAutUpdateCount = Convert.ToInt32(SqlHelper.ExecuteNonQuery(sqlCnt, CommandType.Text, sqlupdate, sqlParamForUdp));

                sqlCnt.Close();

                if (cpremarkUpdateCount > 0 && isAutUpdateCount > 0)
                {
                    scope.Complete();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool SaveProductVehicleExcelInfo(SqlConnection conn, ProductVehicleTypeFileInfoDb entity)
        {
            var sql = @"INSERT INTO ProductVehicleTypeFileInfo(PID,Operator,FilePath,CreatedTime) VALUES(@PID,@Operator,@FilePath,@CreatedTime)";
            var sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@PID",entity.PID),
                    new SqlParameter("@Operator",entity.Operator),
                    new SqlParameter("@FilePath",entity.FilePath),
                    new SqlParameter("@CreatedTime",entity.CreatedTime)
                };

            SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParams);
            return true;
        }


        public static bool SaveVehicleInfoByPid(SqlConnection conn, string pid, ProductVehicleTypeConfigDb entity)
        {
            var sql = @"INSERT INTO ProductVehicleTypeConfig(PID,TID,VehicleID,Nian,PaiLiang,SalesName,CreatedTime,UpdateTime) VALUES(@PID,@TID,@VehicleID,@Nian,@PaiLiang,@SalesName,@CreatedTime,@UpdateTime)";
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@PID",pid),
                new SqlParameter("@TID",entity.TID),
                new SqlParameter("@VehicleID",entity.VehicleID),
                new SqlParameter("@Nian",entity.Nian),
                new SqlParameter("@PaiLiang",entity.PaiLiang),
                new SqlParameter("@SalesName",entity.SalesName),
                new SqlParameter("@CreatedTime",entity.CreatedTime),
                new SqlParameter("@UpdateTime",entity.UpdateTime)
            };

            SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParams);
            return true;
        }

        public static bool BulkSaveVehicleInfoByPid(SqlConnection conn, DataTable tb, string fileName, string vehicleLevel)
        {
            using (SqlBulkCopy bulk = new SqlBulkCopy(conn))
            {

                bulk.BatchSize = tb.Rows.Count;
                bulk.DestinationTableName = "ProductVehicleTypeConfig";
                if (fileName.Contains("二级") && vehicleLevel.Contains("二级"))//二级车型数据
                {
                    bulk.ColumnMappings.Add("TID", "TID");
                    bulk.ColumnMappings.Add("VehicleID", "VehicleID");
                    bulk.ColumnMappings.Add("CreatedTime", "CreatedTime");
                    bulk.ColumnMappings.Add("UpdateTime", "UpdateTime");
                    bulk.ColumnMappings.Add("PID", "PID");
                    bulk.ColumnMappings.Add("ConfigLevel", "ConfigLevel");
                }
                else if (fileName.Contains("四级") && vehicleLevel.Contains("四级"))
                {
                    bulk.ColumnMappings.Add("TID", "TID");
                    bulk.ColumnMappings.Add("VehicleID", "VehicleID");
                    bulk.ColumnMappings.Add("PaiLiang", "PaiLiang");
                    bulk.ColumnMappings.Add("Nian", "Nian");
                    bulk.ColumnMappings.Add("CreatedTime", "CreatedTime");
                    bulk.ColumnMappings.Add("UpdateTime", "UpdateTime");
                    bulk.ColumnMappings.Add("PID", "PID");
                    bulk.ColumnMappings.Add("ConfigLevel", "ConfigLevel");
                }
                else if (fileName.Contains("五级") && vehicleLevel.Contains("五级"))
                {
                    bulk.ColumnMappings.Add("TID", "TID");
                    bulk.ColumnMappings.Add("VehicleID", "VehicleID");
                    bulk.ColumnMappings.Add("PaiLiang", "PaiLiang");
                    bulk.ColumnMappings.Add("Nian", "Nian");
                    bulk.ColumnMappings.Add("SalesName", "SalesName");
                    bulk.ColumnMappings.Add("CreatedTime", "CreatedTime");
                    bulk.ColumnMappings.Add("UpdateTime", "UpdateTime");
                    bulk.ColumnMappings.Add("PID", "PID");
                    bulk.ColumnMappings.Add("ConfigLevel", "ConfigLevel");
                }
                bulk.WriteToServer(tb);

            }
            return true;
        }

        public static List<ProductVehicleTypeConfigDb> GetAllProductVehicleTypeConfigInfoByPid(SqlConnection conn, string pid, string fileName, string vehicleLevel)
        {
            var sql = "";

            if (fileName.Contains("二级") && vehicleLevel.Contains("二级"))//二级车型数据
            {
                sql = @"SELECT * FROM ProductVehicleTypeConfig(NOLOCK) AS pvtc 
WHERE pvtc.PID=@Pid AND pvtc.ConfigLevel=2 ";
            }
            else if (fileName.Contains("四级") && vehicleLevel.Contains("四级"))
            {
                sql = @"SELECT * FROM ProductVehicleTypeConfig(NOLOCK) AS pvtc
WHERE pvtc.PID=@Pid AND pvtc.ConfigLevel =4 ";
            }
            else if (fileName.Contains("五级") && vehicleLevel.Contains("五级"))
            {
                sql = @"SELECT * FROM ProductVehicleTypeConfig(NOLOCK) AS pvtc
WHERE pvtc.PID=@Pid And pvtc.ConfigLevel=5";
            }
            else
            {
                sql = @"select top 100 * from ProductVehicleTypeConfig(Nolock) as pvtc where pvtc.PID=@Pid";
            }

            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@Pid",pid)
            };


            var resultList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParams).ConvertTo<ProductVehicleTypeConfigDb>().ToList();

            return resultList;
        }

        public static List<VehicleTypeInfoDb> GetVehicleTypeInfoByCharacter(SqlConnection conn, string alpha)
        {
            //+'"+alpha+"%'
            var sql = @"SELECT vtt.TID,vtt.VehicleID,vt.VehicleBodyType as VehicleType,vt.Brand,vt.VehicleSeries,vt.Vehicle,vtt.PaiLiang,vtt.ListedYear,vtt.StopProductionYear,vtt.Nian,vtt.SalesName,vtt.AvgPrice,vtt.JointVenture,vt.BrandCategory
  FROM tbl_Vehicle_Type_Timing(NOLOCK) AS vtt INNER JOIN tbl_Vehicle_Type(NOLOCK) AS vt ON vtt.VehicleID=vt.ProductID
WHERE vt.Brand LIKE @Alpha";

            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@Alpha", alpha+"%")
            };


            var resultList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParams).ConvertTo<VehicleTypeInfoDb>().ToList();

            return resultList;

        }

        public static List<string> GetVehicleBrandCategory(SqlConnection conn)
        {
            var brandCategoryList = new List<string>();
            const string sqlBrandCategory = @"SELECT DISTINCT vt.BrandCategory FROM tbl_Vehicle_Type AS vt WITH (NOLOCK)";
            var dtBrandCategory = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sqlBrandCategory);
            for (var i = 0; i < dtBrandCategory.Rows.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(dtBrandCategory.Rows[i][0].ToString()))
                {
                    brandCategoryList.Add(dtBrandCategory.Rows[i][0].ToString());
                }
            }
            return brandCategoryList;
        }

        public static List<string> GetVehicleBodyType(SqlConnection conn)
        {
            var vehicleBodyTypeList = new List<string>();
            const string sqlVehicleType = @"SELECT DISTINCT vt.VehicleBodyType FROM tbl_Vehicle_Type  AS vt WITH (NOLOCK)";
            var dtVehicleType = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sqlVehicleType);
            for (var i = 0; i < dtVehicleType.Rows.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(dtVehicleType.Rows[i][0].ToString()))
                {
                    vehicleBodyTypeList.Add(dtVehicleType.Rows[i][0].ToString());
                }
            }
            return vehicleBodyTypeList;
        }



        public static VehicleInfoExDb GetVehicleInfoExByTid(SqlConnection conn, string tid)
        {
            var sql = @"SELECT vt.Brand,vt.Vehicle,vtt.ListedYear,vtt.StopProductionYear
  FROM tbl_Vehicle_Type_Timing(nolock) AS vtt LEFT JOIN tbl_Vehicle_Type(nolock) AS vt ON vtt.VehicleID=vt.ProductID
WHERE vtt.TID=@Tid";

            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@Tid",tid)
            };


            var resultList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParams).ConvertTo<VehicleInfoExDb>().ToList().FirstOrDefault();

            return resultList;
        }

        public static List<VehicleInfoExDb> GetVehicleInfoExByVehicleId(SqlConnection conn, string vehicleId)
        {
            var sql = @"SELECT vt.Brand,vt.Vehicle,vtt.ListedYear,vtt.StopProductionYear
  FROM tbl_Vehicle_Type_Timing(nolock) AS vtt LEFT JOIN tbl_Vehicle_Type(nolock) AS vt ON vtt.VehicleID=vt.ProductID
WHERE vtt.VehicleID=@VehicleID";

            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@VehicleID",vehicleId)
            };


            var resultList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParams).ConvertTo<VehicleInfoExDb>().ToList();

            return resultList;
        }

        public static List<VehicleInfoExDb> GetVehicleInfoExByPid(SqlConnection conn, string pid, string level)
        {
            var sql = "";
            if (level == "二级车型")
            {
                sql = @"SELECT tt.PID,tt.VehicleID,tt.TID,tt.Nian,tt.PaiLiang,tt.SalesName,tt.ConfigLevel,vt.Brand,vt.Vehicle,vtt.ListedYear,vtt.StopProductionYear
FROM Tuhu_productcatalog..ProductVehicleTypeConfig(NOLOCK) tt
LEFT JOIN tbl_Vehicle_Type_Timing(NOLOCK) AS vtt ON tt.VehicleID COLLATE Chinese_PRC_CI_AS=vtt.VehicleID
LEFT JOIN tbl_Vehicle_Type(NOLOCK) AS vt ON vtt.VehicleID=vt.ProductID
WHERE tt.PID=@Pid";
            }
            if (level == "四级车型")
            {
                sql = @"SELECT tt.PID,tt.VehicleID,tt.TID,tt.Nian,tt.PaiLiang,tt.SalesName,tt.ConfigLevel,vt.Brand,vt.Vehicle,vtt.ListedYear,vtt.StopProductionYear
FROM Tuhu_productcatalog..ProductVehicleTypeConfig(NOLOCK) tt
LEFT JOIN tbl_Vehicle_Type_Timing(NOLOCK) AS vtt ON tt.VehicleID COLLATE Chinese_PRC_CI_AS=vtt.VehicleID AND tt.Nian COLLATE Chinese_PRC_CI_AS=vtt.Nian AND tt.PaiLiang COLLATE Chinese_PRC_CI_AS=vtt.PaiLiang
LEFT JOIN tbl_Vehicle_Type(NOLOCK) AS vt ON vtt.VehicleID=vt.ProductID
WHERE tt.PID=@Pid";
            }
            if (level == "五级车型")
            {
                sql = @"SELECT tt.PID,tt.VehicleID,tt.TID,tt.Nian,tt.PaiLiang,tt.SalesName,tt.ConfigLevel,vt.Brand,vt.Vehicle,vtt.ListedYear,vtt.StopProductionYear
FROM Tuhu_productcatalog..ProductVehicleTypeConfig(NOLOCK) tt
LEFT JOIN tbl_Vehicle_Type_Timing(NOLOCK) AS vtt ON tt.VehicleID COLLATE Chinese_PRC_CI_AS = vtt.VehicleID AND tt.PaiLiang COLLATE Chinese_PRC_CI_AS=vtt.PaiLiang AND tt.Nian COLLATE Chinese_PRC_CI_AS=vtt.Nian AND tt.SalesName COLLATE Chinese_PRC_CI_AS=vtt.SalesName
LEFT JOIN tbl_Vehicle_Type(NOLOCK) AS vt ON vtt.VehicleID=vt.ProductID
WHERE tt.PID=@Pid";
            }

            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@Pid",pid)
            };


            var resultList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParams).ConvertTo<VehicleInfoExDb>().ToList();

            return resultList;
        }

        public static List<VehicleInfoExDb> GetVehicleInfoEx(SqlConnection conn)
        {
            const string sql = @"
       SELECT vtt.VehicleID,
       vtt.TID,
       vtt.Nian,
       vtt.PaiLiang,
       vtt.SalesName,
       vt.Brand,
       vt.Vehicle,
       vtt.ListedYear,
       vtt.StopProductionYear
FROM   Gungnir..tbl_Vehicle_Type_Timing(NOLOCK) AS vtt
       LEFT JOIN Gungnir..tbl_Vehicle_Type(NOLOCK) AS vt
            ON  vtt.VehicleID = vt.ProductID
";

            var resultList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<VehicleInfoExDb>().ToList();

            return resultList;
        }

        public static List<VehicleTypeInfoDb> GetVehicleTypeInfoByBrandName(SqlConnection conn, string brandNames)
        {
            var brandNameList = brandNames.TrimEnd(',').Split(',').ToList();
            var paramList = new List<SqlParameter>();
            var tt = NormalizeInParam(brandNameList, paramList);
            var sql = string.Format("SELECT vtt.TID,vtt.VehicleID,vt.VehicleBodyType as VehicleType,vt.Brand,vt.VehicleSeries,vt.Vehicle,vtt.PaiLiang,vtt.ListedYear,vtt.StopProductionYear,vtt.Nian,vtt.SalesName,vtt.AvgPrice,vtt.JointVenture,vt.BrandCategory FROM tbl_Vehicle_Type_Timing(NOLOCK) AS vtt LEFT JOIN tbl_Vehicle_Type(NOLOCK) AS vt ON vtt.VehicleID=vt.ProductID WHERE vt.Brand in ({0})", tt);


            var resultList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, paramList.ToArray()).ConvertTo<VehicleTypeInfoDb>().ToList();

            return resultList;

        }

        public static bool InsertProductVehicleTypeConfigInfoByPid(SqlConnection conn, string pid, string destPids, string remark, string vehicleLevel)
        {
            var destPidArray = destPids.TrimEnd(',').Split(',');
            var sql = @"SELECT pvtc.PID,pvtc.TID,pvtc.VehicleID,pvtc.Nian,pvtc.PaiLiang,pvtc.SalesName,pvtc.ConfigLevel
  FROM ProductVehicleTypeConfig(NOLOCK) AS pvtc WHERE pvtc.PID=@Pid";
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@Pid",pid)
            };

            var dt = new DataTable();
            dt.Columns.Add("PID", typeof(string));
            dt.Columns.Add("TID", typeof(string));
            dt.Columns.Add("VehicleID", typeof(string));
            dt.Columns.Add("Nian", typeof(string));
            dt.Columns.Add("PaiLiang", typeof(string));
            dt.Columns.Add("SalesName", typeof(string));
            dt.Columns.Add("ConfigLevel", typeof(int));
            dt.Columns.Add("CreatedTime", typeof(DateTime));
            dt.Columns.Add("UpdateTime", typeof(DateTime));

            var productVehicleTypeConfig = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParams).ConvertTo<ProductVehicleTypeConfigDb>().ToList();
            for (var i = 0; i < destPidArray.Length; i++)
            {
                foreach (var item in productVehicleTypeConfig)
                {
                    var dr = dt.NewRow();
                    dr["PID"] = destPidArray[i];
                    dr["TID"] = item.TID;
                    dr["VehicleID"] = item.VehicleID;
                    dr["Nian"] = item.Nian;
                    dr["PaiLiang"] = item.PaiLiang;
                    dr["SalesName"] = item.SalesName;
                    dr["ConfigLevel"] = item.ConfigLevel;
                    dr["CreatedTime"] = DateTime.Now;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }


            var dtSource = GetDtForProductInfoByPid(conn, destPidArray);//获取待更新的产品信息
            for (var i = 0; i < dtSource.Rows.Count; i++)
            {
                dtSource.Rows[i][1] = remark;
            }

            //var dtSourceDel = GetDtForProductInfoByPids(conn, destPidArray);//获取待删除的产品车型配置信息


            using (TransactionScope transaction = new TransactionScope())//使用事务
            {

                string connectString = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;

                SqlConnection sqlCnt = new SqlConnection(connectString);
                sqlCnt.Open();

                //首先要修改产品目标产品的CP_Remark字段车型级别
                using (SqlDataAdapter sda = new SqlDataAdapter("", sqlCnt))
                {
                    string sqlUpdate = @"UPDATE Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] SET CP_Remark = @cpremark WHERE #Catalog_Lang_Oid=@oid";
                    sda.UpdateCommand = new SqlCommand(sqlUpdate, conn);
                    sda.UpdateCommand.Parameters.Add("@cpremark", SqlDbType.NVarChar, 300, "CP_Remark");
                    sda.UpdateCommand.Parameters.Add("@oid", SqlDbType.Int, 1, "#Catalog_Lang_Oid");

                    sda.UpdateCommand.UpdatedRowSource = UpdateRowSource.None;//启用批量更新则需要这么配置
                    sda.UpdateBatchSize = 0;//表示以服务器能接受的最大数量批量提交，1则表示禁止批量更新，具体数字这表示以相应数量批量更新
                    sda.Update(dtSource);
                    sqlCnt.Close();
                }

                StringBuilder sb = new StringBuilder();
                foreach (var item in destPidArray)
                {
                    sb.AppendFormat("'{0}',", item);
                }
                var paramPids = sb.ToString().TrimEnd(',');
                //其次要删掉目标产品已经配置过的车型数据
                //var sqlDel = @"Delete from ProductVehicleTypeConfig WHERE PID in (" + paramPids + ")";//@"Delete from ProductVehicleTypeConfig WHERE PID in (@Pids)";
                //SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sqlDel);


                var sqlDelOne = @"Delete from ProductVehicleTypeConfig WHERE PID in (";
                var sbSql = new StringBuilder();
                sbSql.AppendFormat("{0}", sqlDelOne);

                var paramList = new List<SqlParameter>();
                //var ids = destPidArray.ToList();
                //var tt = NormalizeInParam(ids, paramList);
                //var strSql = string.Format(" DELETE FROM ProductVehicleTypeConfig Where PID IN ({0}) ", tt);

                for (var i = 0; i < destPidArray.Length; i++)
                {
                    sbSql.Append(" @P" + i + ",");
                    var p = new SqlParameter("@P" + i, destPidArray[i]);
                    paramList.Add(p);
                }

                sbSql.Append(" @P" + destPidArray.Length);
                var p1 = new SqlParameter("@P" + destPidArray.Length, "''");
                paramList.Add(p1);

                sbSql.Append(")");
                var temps = sbSql.ToString();
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, temps, paramList.ToArray());


                //最后将当前产品的车型配置复制给目标产品
                using (SqlBulkCopy bulk = new SqlBulkCopy(conn))
                {
                    bulk.BatchSize = dt.Rows.Count;
                    bulk.DestinationTableName = "ProductVehicleTypeConfig";

                    bulk.ColumnMappings.Add("PID", "PID");
                    bulk.ColumnMappings.Add("TID", "TID");
                    bulk.ColumnMappings.Add("VehicleID", "VehicleID");
                    bulk.ColumnMappings.Add("Nian", "Nian");
                    bulk.ColumnMappings.Add("PaiLiang", "PaiLiang");
                    bulk.ColumnMappings.Add("SalesName", "SalesName");
                    bulk.ColumnMappings.Add("ConfigLevel", "ConfigLevel");
                    bulk.ColumnMappings.Add("CreatedTime", "CreatedTime");
                    bulk.ColumnMappings.Add("UpdateTime", "UpdateTime");

                    bulk.WriteToServer(dt);
                }
                bool isAutoAssociate;
                var sql1 = @"SELECT  IsAutoAssociate
                        FROM    Tuhu_productcatalog..tbl_ProductConfig (NOLOCK)
                        WHERE   ProductPID = @ProductPID;";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@ProductPID", pid)
                };
                var res = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql1, parameters);
                isAutoAssociate = res == null ? false : (bool)res;
                var updatesql = @"UPDATE  Tuhu_productcatalog..tbl_ProductConfig
                         SET     IsAutoAssociate = @IsAutoAssociate,vehiclelevel=@Vehiclelevel
                         WHERE   ProductPID = @ProductPID;";
                var insertsql = @"INSERT  INTO Tuhu_productcatalog..tbl_ProductConfig
                                ( ProductPID ,
                                  IsNoNeedInstall ,
                                  IsAutoAssociate ,
                                  vehiclelevel,
                                  CreateTime
                                )
                        VALUES  ( @ProductPID ,
                                  0 ,
                                  @IsAutoAssociate ,
                                  @Vehiclelevel,
                                  GETDATE()
                                );";
                foreach (var item in destPidArray)
                {
                    var parameters1 = new SqlParameter[]
                    {
                    new SqlParameter("@IsAutoAssociate", isAutoAssociate),
                    new SqlParameter("@ProductPID", item),
                    new SqlParameter("@Vehiclelevel", vehicleLevel)
                    };
                    if (SqlHelper.ExecuteNonQuery(conn, CommandType.Text, updatesql, parameters1) == 0)
                    {
                        SqlHelper.ExecuteNonQuery(conn, CommandType.Text, insertsql, parameters1);
                    }
                }
                transaction.Complete();
            }


            return true;
        }

        public static string NormalizeInParam(IEnumerable ie, List<SqlParameter> ps, string prex = "p",
            bool ansiString = true)
        {
            if (ie == null)
                throw new Exception("The list is null!");
            var enumerator = ie.GetEnumerator();
            var stringBuilder = new StringBuilder();
            int num = 0;
            while (enumerator.MoveNext())
            {
                string name = string.Format("@{0}{1}", (object)prex, (object)num++);
                stringBuilder.AppendFormat(name + ",", new object[0]);
                ps.Add(new SqlParameter(name, enumerator.Current));
            }
            if (stringBuilder.Length < 1)
                throw new Exception(
                    "The length of the list is zero!");
            return ((object)stringBuilder).ToString().TrimEnd(new char[1]
            {
                ','
            });
        }

        public static List<ProductVehicleTypeConfigDb> GetProductVehicleTypeConfigInfoListByPid(SqlConnection conn, string pid)
        {
            var sql = @"SELECT pvtc.PKID,pvtc.PID,pvtc.TID,pvtc.VehicleID,pvtc.Nian,pvtc.PaiLiang,pvtc.SalesName,pvtc.ConfigLevel
  FROM ProductVehicleTypeConfig(NOLOCK) AS pvtc WHERE pvtc.PID=@Pid";
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@Pid",pid)
            };

            var resultList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParams).ConvertTo<ProductVehicleTypeConfigDb>().ToList();
            return resultList;
        }

        public static bool DeleteProductVehicleTypeConfigByParams(SqlConnection conn, List<ProductVehicleTypeConfigDb> deleteList)
        {
            //二级车型删除
            var level2List = deleteList.Where(x => x.ConfigLevel == 2).ToList();

            if (level2List.Any())
            {
                var level2DeleteList = SpiltListInfos(level2List, 100);//将待删除的数据按每组100条分批删除，提升删除效率

                foreach (var itemList in level2DeleteList)
                {
                    var sqlParas2 = new List<SqlParameter>();
                    var strSql2 = new StringBuilder();
                    for (var i = 0; i < itemList.Count; i++)
                    {
                        sqlParas2.Add(new SqlParameter($"@PKID{i}", itemList[i].PKID));
                        strSql2.AppendFormat(" DELETE FROM ProductVehicleTypeConfig  Where PKID = @PKID{0} ; ", i);
                    }
                    if (strSql2.Length > 0)
                    {
                        SqlHelper.ExecuteNonQuery(conn, CommandType.Text, strSql2.ToString(), sqlParas2.ToArray());
                    }
                }
            }


            //四级车型删除
            var level4List = deleteList.Where(x => x.ConfigLevel == 4).ToList();
            if (level4List.Any())
            {
                var level4DeleteList = SpiltListInfos(level4List, 100);
                foreach (var itemlist in level4DeleteList)
                {
                    var sqlParas4 = new List<SqlParameter>();
                    var strSql4 = new StringBuilder();
                    for (var i = 0; i < itemlist.Count; i++)
                    {
                        sqlParas4.Add(new SqlParameter($"@PKID{i}", itemlist[i].PKID));
                        strSql4.AppendFormat(" DELETE FROM ProductVehicleTypeConfig WHERE PKID=@PKID{0} ; ", i);
                    }

                    if (strSql4.Length > 0)
                    {
                        SqlHelper.ExecuteNonQuery(conn, CommandType.Text, strSql4.ToString(), sqlParas4.ToArray());
                    }
                }
            }

            //五级车型删除
            var level5List = deleteList.Where(x => x.ConfigLevel == 5).ToList();
            if (!level5List.Any()) return true;

            var level5DeleteList = SpiltListInfos(level5List, 100);
            foreach (var itemlist in level5DeleteList)
            {
                var sqlParas5 = new List<SqlParameter>();
                var strSql5 = new StringBuilder();
                for (var i = 0; i < itemlist.Count; i++)
                {
                    sqlParas5.Add(new SqlParameter($"@PKID{i}", itemlist[i].PKID));
                    strSql5.AppendFormat(" DELETE FROM ProductVehicleTypeConfig WHERE PKID=@PKID{0} ; ", i);
                }

                if (strSql5.Length > 0)
                {
                    SqlHelper.ExecuteNonQuery(conn, CommandType.Text, strSql5.ToString(), sqlParas5.ToArray());
                }
            }
            return true;
        }

        private static List<List<ProductVehicleTypeConfigDb>> SpiltListInfos(List<ProductVehicleTypeConfigDb> sourceList, int size)
        {
            var listGroup = new List<List<ProductVehicleTypeConfigDb>>();
            if (sourceList == null || sourceList.Count <= 0) return listGroup;
            for (var i = 0; i < sourceList.Count; i += size)
            {
                var clist = sourceList.Skip(i).Take(size).ToList();
                listGroup.Add(clist);
            }
            return listGroup;
        }

        public static List<ProductInfo> SelectAllNoImportProduct(int pageIndex, int pageSize)
        {
            const string sql = @"SELECT p.PID ,
                            p.DisplayName ,
							p.OnSale,
							p.Stockout
                    FROM    Tuhu_productcatalog..[CarPAR_zh-CN] AS p WITH ( NOLOCK )
                            LEFT JOIN Tuhu_productcatalog..ProductVehicleTypeConfig
                            AS pv WITH ( NOLOCK ) ON pv.PID = p.PID
                    WHERE   ( CP_Remark = N'二级车型'
                              OR CP_Remark = N'四级车型'
                              OR CP_Remark = N'五级车型'
                            )
                            AND pv.PKID IS NULL
                            AND p.i_ClassType IN ( 2, 4 )
                    ORDER BY p.PID
                            OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH
                            NEXT @PageSize ROWS ONLY; ";

            var conn = ConfigurationManager.ConnectionStrings["Tuhu_productcatalog_AlwaysOnRead"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.CommandTimeout = 60;
                    return dbhelper.ExecuteDataTable(cmd).ConvertTo<ProductInfo>().ToList();
                }
            }
        }

        public static bool DeleteOldProductVehicleTypeConfig(SqlConnection conn, string pid)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var sqlDelete = @"DELETE FROM ProductVehicleTypeConfig WHERE PID=@Pid";
                var sqlParamForDel = new SqlParameter[]
                {
                    new SqlParameter("@Pid",pid),
                };
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sqlDelete, sqlParamForDel);
                scope.Complete();
            }
            return true;
        }

        public static bool IsExistProductVehicleTypeConfig(string pid, string vehicleLevel)
        {
            var sql = @"SELECT  COUNT(1)
                              FROM    Tuhu_productcatalog..tbl_ProductConfig AS s WITH ( NOLOCK )
                              WHERE   ProductPID=@PID
                                      AND s.VehicleLevel = @VehicleLevel";
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_productcatalog_AlwaysOnRead"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PID", pid);
                    cmd.Parameters.AddWithValue("@VehicleLevel", vehicleLevel);
                    cmd.CommandTimeout = 60;
                    int count = Convert.ToInt32(dbhelper.ExecuteScalar(cmd));
                    return count > 0;
                }
            }
        }

        #region  导入配置
        /// <summary>
        /// 批量添加二级车型信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vehicleIdList"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static int BatchInsertSecondVehicleTypeConfig(SqlConnection conn, List<string> vehicleIdList, string pid)
        {
            var sql = @"WITH  VehicleIdList
                              AS ( SELECT   *
                                   FROM     Tuhu_productcatalog..SplitString(@VehicleIdStr,
                                                              ',', 1)
                                 )
                        INSERT  INTO Tuhu_productcatalog..ProductVehicleTypeConfig
                                ( PID ,
                                  VehicleID ,
                                  CreatedTime ,
                                  UpdateTime ,
                                  ConfigLevel
						        )
                                SELECT  @PID ,
                                        t.Item ,
                                        GETDATE() ,
                                        GETDATE() ,
                                        2
                                FROM    VehicleIdList AS t; ";
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@PID",pid),
                new SqlParameter("@VehicleIdStr",String.Join(",",vehicleIdList))
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParams);
        }
        /// <summary>
        /// 批量添加五级车型信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tidList"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static int BatchInsertFiveVehicleTypeConfig(SqlConnection conn, List<string> tidList, string pid)
        {
            var sql = @" WITH  TidList
                              AS ( SELECT   *
                                   FROM     Tuhu_productcatalog..SplitString(@TidStr,
                                                              ',', 1)
                                 )
                        INSERT  INTO Tuhu_productcatalog..ProductVehicleTypeConfig
                                ( PID ,
                                  TID ,
                                  VehicleID ,
                                  Nian ,
                                  PaiLiang ,
                                  SalesName ,
                                  CreatedTime ,
                                  UpdateTime ,
                                  ConfigLevel
						        )
                                SELECT  @PID ,
                                        vtt.TID ,
                                        vtt.VehicleID ,
                                        vtt.Nian ,
                                        vtt.PaiLiang ,
                                        vtt.SalesName ,
                                        GETDATE() ,
                                        GETDATE() ,
                                        5
                                FROM    Gungnir..tbl_Vehicle_Type_Timing (NOLOCK)
                                        AS vtt
                                        JOIN TidList AS t ON t.Item = vtt.TID COLLATE Chinese_PRC_CI_AS;";
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@PID",pid),
                new SqlParameter("@TidStr",String.Join(",",tidList))
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParams);
        }
        /// <summary>
        /// 批量更新五级车型信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tidList"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static int BatchUpdateFiveVehicleTypeConfig(SqlConnection conn, List<string> tidList, string pid)
        {
            var sql = @"WITH  TidList
                              AS ( SELECT   *
                                   FROM     Tuhu_productcatalog..SplitString(@TidStr,
                                                              ',', 1)
                                 )
                        UPDATE  Tuhu_productcatalog.dbo.ProductVehicleTypeConfig
                        SET     VehicleID = vvt.VehicleID ,
                                Nian = vvt.Nian ,
                                PaiLiang = vvt.PaiLiang ,
                                SalesName = vvt.SalesName ,
                                ConfigLevel = 5 ,
                                UpdateTime = GETDATE()
                        FROM    Tuhu_productcatalog..ProductVehicleTypeConfig (NOLOCK)
                                AS pvtc
                                JOIN TidList AS t ON pvtc.TID = t.Item
                                                     AND PID = @PID
                                JOIN Gungnir..tbl_Vehicle_Type_Timing (NOLOCK) AS vvt ON vvt.TID COLLATE Chinese_PRC_CI_AS = t.Item;";
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@PID",pid),
                new SqlParameter("@TidStr",String.Join(",",tidList))
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParams);
        }
        /// <summary>
        /// 批量删除二级车型配置信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vehicleIdList"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static int BatchDeleteSecondVehicleTypeConfig(SqlConnection conn, List<string> vehicleIdList, string pid)
        {
            var sql = @"WITH    VehicleIdList
                  AS ( SELECT   *
                       FROM     Tuhu_productcatalog..SplitString(@VehicleIdStr, ',', 1)
                     )
            DELETE  FROM Tuhu_productcatalog..ProductVehicleTypeConfig
            WHERE   PID = @PID
                    AND EXISTS ( SELECT 1
                                 FROM   VehicleIdList AS t
                                 WHERE  VehicleID = t.Item );";
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@PID",pid),
                new SqlParameter("@VehicleIdStr",String.Join(",",vehicleIdList))
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParams);
        }
        /// <summary>
        /// 批量删除五级车型信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tidList"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static int BatchDeleteFiveVehicleTypeConfig(SqlConnection conn, List<string> tidList, string pid)
        {
            var sql = @"WITH    TidList
                  AS ( SELECT   *
                       FROM     Tuhu_productcatalog..SplitString(@TidStr, ',', 1)
                     )
            DELETE  FROM Tuhu_productcatalog..ProductVehicleTypeConfig
            WHERE   PID = @PID
                    AND EXISTS ( SELECT 1
                                 FROM   TidList AS t
                                 WHERE  TID = t.Item );";
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@PID",pid),
                new SqlParameter("@TidStr",String.Join(",",tidList))
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParams);
        }
        #endregion
    }
}
