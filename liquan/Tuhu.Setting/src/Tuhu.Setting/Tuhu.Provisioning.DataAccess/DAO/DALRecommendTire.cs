using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALRecommendTire
    {
        #region 根据车型推荐轮胎
        public static DataTable GetVehicleDepartment()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT DISTINCT VT.BrandCategory  FROM  Gungnir..tbl_Vehicle_Type AS VT  WITH(NOLOCK) WHERE ISNULL(BrandCategory,'')<>''");
            }
        }
        public static DataTable GetBrands()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT DISTINCT VP.CP_Brand FROM Tuhu_productcatalog.dbo.vw_Products AS VP WITH(NOLOCK) WHERE VP.PID LIKE 'TR-%' ");
            }
        }
        public static DataTable GetBrandsByCategoryName(string categoryName)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(
                    $@"SELECT DISTINCT CP_Brand FROM Tuhu_productcatalog..vw_Products AS P WITH (NOLOCK) INNER JOIN Tuhu_productcatalog..CarPAR_CatalogHierarchy AS CH ON P.oid=CH.child_oid WHERE CH.NodeNo LIKE CONCAT((SELECT TOP 1 NodeNo FROM Tuhu_productcatalog..CarPAR_CatalogHierarchy WHERE CategoryName='{categoryName}'),'.%') AND P.OnSale=1 AND P.stockout=0 AND P.CP_Brand IS NOT NULL");
            }
        }
        public static DataTable GetSpecificationsByVehicle(string vehicleIDS)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT VT.Tires  FROM  Gungnir..tbl_Vehicle_Type AS VT WITH(NOLOCK) WHERE ISNULL(Tires,'')<>'' AND VT.ProductID IN (SELECT * FROM Gungnir..Split(@VehicleIDS,','))", CommandType.Text, new SqlParameter("@VehicleIDS", vehicleIDS));
            }
        }

        public static DataTable GetVehicleBodys()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@" SELECT DISTINCT VT.VehicleBodyType  FROM  Gungnir..tbl_Vehicle_Type AS VT  WITH(NOLOCK) WHERE ISNULL(VehicleBodyType,'')<>''");
            }
        }

        public static IEnumerable<VehicleModel> GetVehicleOneTwoLevel()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT DISTINCT VT.ProductID,VT.Brand,VT.Vehicle FROM  Gungnir..tbl_Vehicle_Type AS VT WITH(NOLOCK)").ConvertTo<VehicleModel>();
            }
        }

        public static IEnumerable<VehicleTireModel> SelectVehicleList(string Departments, string VehicleIDS, string PriceRanges, string VehicleBodyTypes)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var cmd = new SqlCommand("Tuhu_productcatalog..Setting_SelectVehicleForRecommendTire");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VehicleIDS", VehicleIDS.Contains("全部") || string.IsNullOrWhiteSpace(VehicleIDS) ? "全部" : VehicleIDS);
                cmd.Parameters.AddWithValue("@Departments", Departments.Contains("全部") || string.IsNullOrWhiteSpace(Departments) ? "全部" : Departments);
                cmd.Parameters.AddWithValue("@PriceRanges", PriceRanges.Contains("全部") || string.IsNullOrWhiteSpace(PriceRanges) ? "全部" : PriceRanges);
                cmd.Parameters.AddWithValue("@VehicleBodyTypes", VehicleBodyTypes.Contains("全部") || string.IsNullOrWhiteSpace(VehicleBodyTypes) ? "全部" : VehicleBodyTypes);
                return dbHelper.ExecuteDataTable(cmd).ConvertTo<VehicleTireModel>();
            }
        }


        public static IEnumerable<SelectListModel> SelectListNew(string Departments, string VehicleIDS,
            string PriceRanges, string VehicleBodyTypes, string Specifications, string Brands, int IsRof, string PID,
            string Province, string City, decimal? StartPrice, decimal? EndPrice)
        {
            string rofCondition = "";
            if (IsRof == 3)
            {
                rofCondition = " AND CHARINDEX(SS.Item,T.RofTireSize)>0";
            }
            else if (IsRof == 2)
            {
                rofCondition = " AND CHARINDEX(SS.Item,T.RofTireSize)<=0";
            }
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {

                #region SQL
                var sql = string.Format(@"SELECT  	TT.BrandCategory,
		        TT.Brand,
		        TT.Vehicle,
		        TT.Tires,
		        TT.MinPrice,
		        TT.TiresMatch,
		        TT.VehicleId,
		        TT.TireSize,
		        VTR.PID,
		        VTR.Postion,
		        BI.ProductId,
		        VTR.PKID,
                VTR.Reason,
                BI.RowNumber,
                VTR.RecommendedPicture,VTR.StartTime,VTR.EndTime
FROM	( SELECT	T.BrandCategory,
					T.Brand,
					T.Vehicle,
					T.Tires,
					T.MinPrice,
					T.TiresMatch,
                    T.RofTireSize,
					T.VehicleId,
					SS.Item AS TireSize
		  FROM		( SELECT  	VT.BrandCategory,
								VT.Brand,
								VT.Vehicle,
								VT.Tires,
								VT.MinPrice,
								VT.TiresMatch,
                                VT.RofTireSize,
								VT.ProductID AS VehicleId
					  FROM		Gungnir.dbo.tbl_Vehicle_Type AS VT WITH ( NOLOCK )
					  WHERE		(
								  VT.ProductID IN ( SELECT	*
													FROM	Gungnir..Split(@VehicleIDS, ',') AS S )
								  OR @VehicleIDS = N'全部' )
								AND (
									  VT.BrandCategory IN ( SELECT	*
															FROM	Gungnir..Split(@Departments, ',') AS S )
									  OR @Departments = N'全部'
									  OR (
										   CHARINDEX(N'其它', @Departments) > 0
										   AND ISNULL(VT.BrandCategory, '') = '' ) )
								AND (
									  @PriceRanges = N'全部'
									  OR (
										   CHARINDEX(N'高', @PriceRanges) > 0
										   AND VT.MinPrice >= 16 )
									  OR (
										   CHARINDEX(N'中', @PriceRanges) > 0
										   AND VT.MinPrice < 16
										   AND VT.MinPrice >= 8 )
                                      OR (
										   CHARINDEX(N'Between',@PriceRanges) > 0
										   AND VT.MinPrice >= @StartPrice
                                           AND VT.MinPrice <= @EndPrice
                                        )
									  OR (
										   CHARINDEX(N'低', @PriceRanges) > 0
										   AND VT.MinPrice < 8
										   AND ISNULL(VT.MinPrice, 0) > 0 )
                                     )
                                    
								AND (
									  @VehicleBodyTypes = N'全部'
									  OR VT.VehicleBodyType IN ( SELECT	*
																 FROM	Gungnir..Split(@VehicleBodyTypes, ',') AS S2 ) )
								AND ISNULL(VT.Tires, '') <> ''
					) AS T
		  CROSS APPLY Gungnir..SplitString(T.Tires, ';', 1) AS SS
		  WHERE  (@Specifications =N'不限' OR SS.Item IN (SELECT * FROM Gungnir.dbo.Split(@Specifications,',') AS S3))
{0}
		) AS TT
LEFT JOIN Tuhu_productcatalog.dbo.tbl_VehicleTireRecommend AS VTR WITH ( NOLOCK )
		ON TT.VehicleId = VTR.VehicleId COLLATE Chinese_PRC_CI_AS
		   AND TT.TireSize = VTR.TireSize COLLATE Chinese_PRC_CI_AS", rofCondition);
                if (string.IsNullOrWhiteSpace(Province) || string.IsNullOrWhiteSpace(City))
                {

                    sql += @" LEFT JOIN ( SELECT	T.VehicleId,
                    T.TireSize,
					T.ProductId,
                    T.RowNumber 
            FROM    (SELECT    CTR.VehicleId,
                                CTR.TireSize,
                                CTR.ProductId,
                                ROW_NUMBER() OVER(PARTITION BY CTR.VehicleId, CTR.TireSize ORDER BY CTR.Grade DESC) AS RowNumber

                      FROM      Tuhu_bi.dbo.tbl_CarTireRecommendation AS CTR WITH(NOLOCK)
					) AS T
            WHERE T.RowNumber <= 4
		  ) AS BI
        ON BI.VehicleId = TT.VehicleId COLLATE Chinese_PRC_CI_AS AND BI.TireSize COLLATE Chinese_PRC_CI_AS = TT.TireSize";
                }
                else
                {
                    sql += @"   LEFT JOIN (  SELECT	T.VehicleId,
                    T.TireSize,
					T.ProductId,
                    T.RowNumber 
            FROM    (SELECT    CTR.VehicleId,
                                CTR.TireSize,
                                CTR.ProductId,
                                ROW_NUMBER() OVER(PARTITION BY CTR.VehicleId, CTR.TireSize ORDER BY CTR.Grade DESC) AS RowNumber

                      FROM      Tuhu_bi.dbo.tbl_CarTireRecommendation_Region  AS CTR WITH(NOLOCK)
					  JOIN Gungnir.dbo.tbl_region AS R WITH(NOLOCK) ON CTR.CityID=R.PKID
					  WHERE R.RegionName=@City
					) AS T
            WHERE T.RowNumber <= 4
		  ) AS BI
        ON BI.VehicleId = TT.VehicleId COLLATE Chinese_PRC_CI_AS  AND BI.TireSize COLLATE Chinese_PRC_CI_AS = TT.TireSize";
                }
                if ((string.IsNullOrWhiteSpace(Brands) || Brands == "不限") && string.IsNullOrWhiteSpace(PID))
                {
                    //品牌不限PID不限
                }
                else
                {
                    sql += @" INNER  JOIN Tuhu_productcatalog.dbo.vw_Products AS VP
		ON VP.PID = VTR.PID
WHERE  (@Brands=N'不限' OR  VP.CP_Brand COLLATE Chinese_PRC_CI_AS IN (SELECT * FROM  Gungnir.dbo.Split(@Brands,',') AS S4))
       AND (@PID IS NULL OR VTR.PID =@PID)";
                }
                sql += @"  ORDER BY TT.VehicleId";
                #endregion

                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[] {
               new SqlParameter("@VehicleIDS",VehicleIDS.Contains("全部") || string.IsNullOrWhiteSpace(VehicleIDS) ? "全部" :VehicleIDS),
              new SqlParameter("@Departments",Departments.Contains("全部") || string.IsNullOrWhiteSpace(Departments) ? "全部" :Departments),
              new SqlParameter("@PriceRanges",PriceRanges.Contains("全部") || string.IsNullOrWhiteSpace(PriceRanges) ? "全部" :PriceRanges),
              new SqlParameter("@VehicleBodyTypes",VehicleBodyTypes.Contains("全部") || string.IsNullOrWhiteSpace(VehicleBodyTypes) ? "全部" :VehicleBodyTypes),
              new SqlParameter("@Specifications",Specifications.Contains("不限") || string.IsNullOrWhiteSpace(Specifications) ? "不限" :Specifications),
              new SqlParameter("@Brands",Brands.Contains("不限") || string.IsNullOrWhiteSpace(Brands) ? "不限" :Brands),
              //new SqlParameter("@Province",string.IsNullOrWhiteSpace(Province) ? null:Province),
               new SqlParameter("@City",string.IsNullOrWhiteSpace(City) ? null:City),
              new SqlParameter("@PID", string.IsNullOrWhiteSpace(PID) ? null:PID),
                    new SqlParameter("@StartPrice",StartPrice),
                    new SqlParameter("@EndPrice",EndPrice)
                }).ConvertTo<SelectListModel>();


            }
        }


        public static IEnumerable<VehicleTireRecommend> SelectVehicleRecommendTire()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable("SELECT VTR.PKID, VTR.VehicleId,VTR.TireSize,VTR.PID,VTR.Postion,VP.CP_Brand FROM Tuhu_productcatalog..tbl_VehicleTireRecommend AS VTR WITH ( NOLOCK )  JOIN Tuhu_productcatalog..vw_Products AS VP WITH(NOLOCK) ON VTR.PID=VP.PID COLLATE Chinese_PRC_CI_AS").ConvertTo<VehicleTireRecommend>();
            }
        }
        public static IEnumerable<VehicleTireRecommend> SelectVehicleRecommendTireBI()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                return dbHelper.ExecuteDataTable("SELECT CTR.PKID,CTR.VehicleId,CTR.TireSize,CTR.ProductId AS PID,CTR.Grade,VP.CP_Brand FROM  Tuhu_bi..tbl_CarTireRecommendation AS CTR WITH(NOLOCK) JOIN Tuhu_productcatalog..vw_Products AS VP WITH(NOLOCK) ON CTR.ProductId=VP.PID COLLATE Chinese_PRC_CI_AS").ConvertTo<VehicleTireRecommend>();
            }
        }

        public static DataRow CheckPID(string PID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataRow(@"SELECT TOP 1
        ISNULL(T.SpecialSize, VP.TireSize) AS TireSize,
        VP.DisplayName
FROM    Tuhu_productcatalog..vw_Products AS VP WITH(NOLOCK)

        LEFT JOIN Tuhu_productcatalog..CarPAR_CatalogProducts AS T WITH(NOLOCK) ON VP.PID = T.pid2
WHERE   PID = @PID; ", CommandType.Text, new SqlParameter("@PID", PID));
            }
        }
        public static bool CheckPIDNew(string PID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var obj = dbHelper.ExecuteScalar("	SELECT COUNT(1) FROM Tuhu_productcatalog..tbl_VehicleTireRecommend AS VTR WITH(NOLOCK) WHERE VTR.PID=@PID", CommandType.Text, new SqlParameter("@PID", PID));
                return Convert.ToInt32(obj) > 0;
            }
        }

        public static int ReplacePID(string OldPID, string NewPID, string Reason)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var checkResult = CheckTireSizeSame(dbHelper, OldPID, NewPID);
                if (checkResult == 1)
                {
                    return dbHelper.ExecuteNonQuery(@"UPDATE	Tuhu_productcatalog..tbl_VehicleTireRecommend
                                               SET		PID = @NewPID,
                                               		Reason = @Reason
                                               WHERE	PID = @OldPID;", CommandType.Text,
                                                new SqlParameter[] {
                                                   new SqlParameter("@NewPID",NewPID),
                                                    new SqlParameter("@OldPID",OldPID),
                                                     new SqlParameter("@Reason",string.IsNullOrWhiteSpace(Reason)?null:Reason)
                                                });
                }
                else
                    return -1;
                var cmd = new SqlCommand("Tuhu_productcatalog..Setting_ReplaceRecommendPIDForTire");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OldPID", OldPID);
                cmd.Parameters.AddWithValue("@NewPID", NewPID);
                cmd.Parameters.AddWithValue("@Result", SqlDbType.Int).Direction = ParameterDirection.Output;
                dbHelper.ExecuteNonQuery(cmd);
                return Convert.ToInt32(cmd.Parameters["@Result"].Value);
            }
        }

        public static int CheckTireSizeSame(SqlDbHelper dbHelper, string OldPID, string NewPID)
        {

            var cmd = new SqlCommand(@"SELECT	COUNT(1)
                                           FROM    Tuhu_productcatalog..vw_Products AS VP WITH(NOLOCK)
                                           WHERE   VP.PID = @NewPID
                                                   AND VP.TireSize IN(SELECT TireSize
                                                                        FROM   Tuhu_productcatalog..vw_Products AS VP2 WITH(NOLOCK)
                                                                        WHERE  PID = @OldPID); ");
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@OldPID", OldPID);
            cmd.Parameters.AddWithValue("@NewPID", NewPID);

            var result = Convert.ToInt32(dbHelper.ExecuteScalar(cmd));
            return result;
        }
        public static int SaveSingle(IEnumerable<VehicleTireRecommend> list)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var result = -99;
                dbHelper.BeginTransaction();
                //删
                DeleteDatasByVehicleAndTireSize(dbHelper, list.FirstOrDefault());
                foreach (var item in list)
                {
                    //插入
                    result = InsertDataToRecommend(dbHelper, item);
                    if (result <= 0)
                        dbHelper.Rollback();
                }
                dbHelper.Commit();
                return result;
            }
        }

        public static int SaveMany(IEnumerable<VehicleTireRecommend> list)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var result = -99;
                dbHelper.BeginTransaction();
                var Postions = list.Where(c => !string.IsNullOrWhiteSpace(c.PID)).Select(C => C.Postion).ToList(); //置顶图片
                if (!Postions.Any())
                {
                    Postions = new List<int>() { 0 };
                }
                //删
                DeleteDatasByPostion(dbHelper, list.FirstOrDefault(), string.Join(",", Postions));

                foreach (var item in list)
                {
                    foreach (var vehicle in item.VehicleIDS)
                    {
                        result = InsertDataToRecommend(dbHelper, item, vehicle);
                    }

                }
                dbHelper.Commit();
                return result;

            }

        }
        public static int Delete(string TireSize, string VehicleID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"DELETE Tuhu_productcatalog..tbl_VehicleTireRecommend WHERE VehicleId=@VehicleId AND TireSize=@TireSize",
               CommandType.Text,
               new SqlParameter[] {
                    new SqlParameter("@VehicleId",VehicleID),
                    new SqlParameter("@TireSize",TireSize)
               });
            }
        }

        public static IEnumerable<VehicleTireRecommend> SelectTireRecommendByVehicleSAndSize(string tireSize, string vehicleIDS)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT * FROM Tuhu_productcatalog..tbl_VehicleTireRecommend AS VTR WITH(NOLOCK) WHERE VTR.TireSize=@TireSize AND VTR.VehicleId COLLATE Chinese_PRC_CI_AS IN (SELECT * FROM Gungnir..Split(@VehicleIDS,',') AS S)",
               CommandType.Text,
               new SqlParameter[] {
                    new SqlParameter("@VehicleIDS",vehicleIDS),
                    new SqlParameter("@TireSize",tireSize)
               }).ConvertTo<VehicleTireRecommend>();
            }
        }

        public static IEnumerable<VehicleTireRecommend> SelectTireRecommendByPID(string PID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT * FROM  Tuhu_productcatalog..tbl_VehicleTireRecommend WITH(NOLOCK) WHERE PID=@PID",
                 CommandType.Text,
                    new SqlParameter("@PID", PID)
             ).ConvertTo<VehicleTireRecommend>();
            }
        }

        public static IEnumerable<VehicleTireRecommend> SelectTireRecommendByVehicleAndSize(string tireSize, string vehicleID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT * FROM  Tuhu_productcatalog..tbl_VehicleTireRecommend WITH(NOLOCK) WHERE VehicleId=@VehicleId AND TireSize=@TireSize",
                 CommandType.Text,
                 new SqlParameter[] {
                    new SqlParameter("@VehicleId",vehicleID),
                    new SqlParameter("@TireSize",tireSize)
             }).ConvertTo<VehicleTireRecommend>();
            }
        }

        public static VehicleModel FetchVehicleByID(string vehicleId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  TOP 1 VT.Brand,VT.Vehicle,VT.ProductID FROM  Gungnir.dbo.tbl_Vehicle_Type AS VT WITH(NOLOCK) WHERE VT.ProductID=@VehicleID",
                 CommandType.Text,
                   new SqlParameter("@VehicleID", vehicleId)).ConvertTo<VehicleModel>()?.FirstOrDefault();
            }
        }

        public static int DeleteDatasByVehicleAndTireSize(SqlDbHelper dbHelper, VehicleTireRecommend model)
        {
            return dbHelper.ExecuteNonQuery(@"DELETE Tuhu_productcatalog..tbl_VehicleTireRecommend WHERE VehicleId=@VehicleId AND TireSize=@TireSize",
                CommandType.Text,
                new SqlParameter[] {
                    new SqlParameter("@VehicleId",model.Vehicleid),
                    new SqlParameter("@TireSize",model.TireSize)
            });
        }
        public static int InsertDataToRecommend(SqlDbHelper dbHelper, VehicleTireRecommend model)
        {
            return dbHelper.ExecuteNonQuery(@"INSERT INTO Tuhu_productcatalog..tbl_VehicleTireRecommend(VehicleId,TireSize,PID,Postion,Reason,RecommendedPicture,StartTime,EndTime)VALUES(@VehicleId, @TireSize, @PID,@Postion,@Reason,@RecommendedPicture,@StartTime,@EndTime)",
                 CommandType.Text,
                 new SqlParameter[] {
                    new SqlParameter("@VehicleId",model.Vehicleid),
                    new SqlParameter("@TireSize",model.TireSize),
                    new SqlParameter("@PID",model.PID),
                    new SqlParameter("@Postion",model.Postion),
                    new SqlParameter("@RecommendedPicture",model.RecommendedPicture ?? ""),
                    new SqlParameter("@StartTime",model.StartTime),
                    new SqlParameter("@EndTime",model.EndTime),
                    new SqlParameter("@Reason",string.IsNullOrWhiteSpace(model.Reason)?null:model.Reason)
             });
        }
        public static int InsertDataToRecommend(SqlDbHelper dbHelper, VehicleTireRecommend model, string vehicleid)
        {
            return dbHelper.ExecuteNonQuery(@"INSERT INTO Tuhu_productcatalog..tbl_VehicleTireRecommend(VehicleId,TireSize,PID,Postion,Reason,RecommendedPicture,StartTime,EndTime)VALUES(@VehicleId, @TireSize, @PID,@Postion,@Reason,@RecommendedPicture,@StartTime,@EndTime)",
                 CommandType.Text,
                 new SqlParameter[] {
                    new SqlParameter("@VehicleId",vehicleid),
                    new SqlParameter("@TireSize",model.TireSize),
                    new SqlParameter("@PID",model.PID),
                    new SqlParameter("@Postion",model.Postion),
                    new SqlParameter("@RecommendedPicture",model.RecommendedPicture ?? ""),
                    new SqlParameter("@StartTime",model.StartTime),
                    new SqlParameter("@EndTime",model.EndTime),
                    new SqlParameter("@Reason",string.IsNullOrWhiteSpace(model.Reason)?null:model.Reason)
             });
        }
        public static int DeleteDatasByVehiclesAndTireSize(SqlDbHelper dbHelper, VehicleTireRecommend model, string PIDS)
        {
            return dbHelper.ExecuteNonQuery(@"DELETE  Tuhu_productcatalog..tbl_VehicleTireRecommend  WHERE TireSize=@TireSize AND VehicleId COLLATE Chinese_PRC_CI_AS IN (SELECT * FROM Gungnir..Split(@VehicleIDS,',') AS S) AND PID COLLATE Chinese_PRC_CI_AS IN (SELECT * FROM Gungnir..Split(@PIDS,',') AS SS)",
                CommandType.Text,
                new SqlParameter[] {
                    new SqlParameter("@TireSize",model.TireSize),
                    new SqlParameter("@VehicleIDS",string.Join(",",model.VehicleIDS)),
                    new SqlParameter("@PIDS",PIDS)
                }
                );
        }
        public static int DeleteDatasByPostion(SqlDbHelper dbHelper, VehicleTireRecommend model, string Postions)
        {
            return dbHelper.ExecuteNonQuery(@"DELETE  Tuhu_productcatalog..tbl_VehicleTireRecommend
WHERE   TireSize = @TireSize
        AND VehicleId COLLATE Chinese_PRC_CI_AS IN (
        SELECT  *
        FROM    Gungnir..Split(@VehicleIDS, ',') AS S )
        AND Postion  IN (
        SELECT  *
        FROM    Gungnir..Split(@Postions, ',') AS SS )",
                CommandType.Text,
                new SqlParameter[] {
                    new SqlParameter("@TireSize",model.TireSize),
                    new SqlParameter("@VehicleIDS",string.Join(",",model.VehicleIDS)),
                    new SqlParameter("@Postions",Postions)
                }
                );
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Postions"></param>
        /// <returns></returns>
        public static int DeleteDatasByPostion(VehicleTireRecommend model, List<int> Postions)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"DELETE  Tuhu_productcatalog..tbl_VehicleTireRecommend
            WHERE   TireSize = @TireSize
            AND VehicleId COLLATE Chinese_PRC_CI_AS IN (
            SELECT  *
            FROM    Gungnir..Split(@VehicleIDS, ',') AS S )
            AND Postion  IN (
            SELECT  *
            FROM    Gungnir..Split(@Postions, ',') AS SS )",
                    CommandType.Text,
                    new SqlParameter[]
                    {
                        new SqlParameter("@TireSize", model.TireSize),
                        new SqlParameter("@VehicleIDS", string.Join(",", model.VehicleIDS)),
                        new SqlParameter("@Postions", string.Join(",", Postions))
                    }
                );
            }
        }

        public static IEnumerable<TireSizeModel> SelectALLTireSize()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT	VP.CP_Tire_Width AS Width,
		                                                    VP.CP_Tire_AspectRatio AS AspectRatio,
	                                               	        VP.CP_Tire_Rim AS Rim
                                                   FROM	Tuhu_productcatalog..vw_Products AS VP WITH ( NOLOCK )
                                                   WHERE	ISNULL(VP.CP_Tire_Width, '') <> ''
                                                   		AND ISNULL(VP.CP_Tire_AspectRatio, '') <> ''
                                                   		AND ISNULL(VP.CP_Tire_Rim, '') <> ''
                                                   		AND VP.ProductID LIKE 'TR-%';").ConvertTo<TireSizeModel>();
            }
        }
        public static IEnumerable<TireSizeModel> SelectALLHubSize()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT DISTINCT VP.CP_Tire_Rim AS Rim
                                                   FROM	Tuhu_productcatalog..vw_Products AS VP WITH ( NOLOCK )
                                                   WHERE	VP.CP_Tire_Rim IS NOT NULL
                                                   		AND VP.ProductID LIKE 'LG-%';").ConvertTo<TireSizeModel>();
            }
        }
        public static DataTable GetMatchBySize(string matchTires, string tireSize)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT PID FROM Tuhu_productcatalog..vw_Products AS VP WHERE VP.PID COLLATE Chinese_PRC_CI_AS IN(SELECT * FROM Gungnir..Split(@PIDS, ';') AS S) AND VP.TireSize = @TireSize", CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@PIDS",matchTires),
                    new SqlParameter("@TireSize",tireSize)

                });
            }
        }
        #endregion

        #region 轮胎强制推荐
        public static DataTable SelectQZTJTires(QZTJSelectModel model, PagerModel pager)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                pager.TotalItem = FetchQZTJCount(model);
                return dbHelper.ExecuteDataTable(@"SELECT	T.PID,
		T.DisplayName,
        T.TireSize
FROM	( SELECT	VP.PID,
					VP.DisplayName,
                    VP.TireSize
		  FROM		Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
		  WHERE		VP.PID LIKE 'TR-%'
					AND (
						  VP.TireSize = @TireSize
						  OR @TireSize IS NULL )
					AND (
						  VP.PID = @PID
						  OR @PID IS NULL )
					AND (
						  VP.PID IN ( SELECT DISTINCT
												TRR2.PID
									  FROM		Tuhu_productcatalog.dbo.tbl_TireEnforceRecommend AS TRR2 WITH ( NOLOCK )
									  WHERE		TRR2.RecommendPID = @RecommendPID )
						  OR @RecommendPID IS NULL )
					AND (
						  @Brands IS NULL
						  OR VP.CP_Brand COLLATE Chinese_PRC_CI_AS IN ( SELECT	*
																		FROM	Gungnir.dbo.Split(@Brands, ',') AS S ) )
					AND (
						  @ShowType = 1
						  OR @ShowType = 2
						  AND VP.PID IN ( SELECT DISTINCT
													TRR2.PID
										  FROM		Tuhu_productcatalog.dbo.tbl_TireEnforceRecommend AS TRR2 WITH ( NOLOCK ) )
						  OR @ShowType = 3
						  AND VP.PID NOT  IN ( SELECT DISTINCT
														TRR2.PID
											   FROM		Tuhu_productcatalog.dbo.tbl_TireEnforceRecommend AS TRR2 WITH ( NOLOCK ) )
						  OR @ShowType = 4
						  AND VP.PID IN ( SELECT	TT.PID
										  FROM		( SELECT	ROW_NUMBER() OVER ( PARTITION BY TRR.PID, TRR.RecommendPID ORDER BY TRR.PKID ) AS RowNumber,
																*
													  FROM		Tuhu_productcatalog..tbl_TireEnforceRecommend AS TRR
													) AS TT
										  WHERE		TT.RowNumber > 1 ) )
		  ORDER BY	VP.oid
					OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
                                                   									FETCH NEXT @PageSize ROWS ONLY
		) AS T", CommandType.Text, new SqlParameter[] {
                                                     new SqlParameter("@PageIndex",pager.CurrentPage),
                                                     new SqlParameter("@PageSize",pager.PageSize),
                                                     new SqlParameter("@TireSize",string.IsNullOrWhiteSpace(model.TireSize)?null:model.TireSize),
                                                     new SqlParameter("@PID",string.IsNullOrWhiteSpace(model.PID)?null:model.PID),
                                                     new SqlParameter("@RecommendPID",string.IsNullOrWhiteSpace(model.RecommendPID)?null:model.RecommendPID),
                                                     new SqlParameter("@Brands",string.IsNullOrWhiteSpace(model.Brands)||model.Brands.Contains("不限")?null:model.Brands),
                                                     new SqlParameter("@ShowType",model.ShowType)


                });
            }
        }


        public static int FetchQZTJCount(QZTJSelectModel model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var obj = dbHelper.ExecuteScalar(@"SELECT	COUNT(1)
FROM	( SELECT	VP.PID
		  FROM		Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
		  WHERE		VP.PID LIKE 'TR-%'
					AND (
						  VP.TireSize = @TireSize
						  OR @TireSize IS NULL )
					AND (
						  VP.PID = @PID
						  OR @PID IS NULL )
					AND (
						  VP.PID IN ( SELECT DISTINCT
												TRR2.PID
									  FROM		Tuhu_productcatalog.dbo.tbl_TireEnforceRecommend AS TRR2 WITH ( NOLOCK )
									  WHERE		TRR2.RecommendPID = @RecommendPID )
						  OR @RecommendPID IS NULL )
					AND (
						  @Brands IS NULL
						  OR VP.CP_Brand COLLATE Chinese_PRC_CI_AS IN ( SELECT	*
											  FROM		Gungnir.dbo.Split(@Brands, ',') AS S ) )
					AND (
						  @ShowType = 1
						  OR @ShowType = 2
						  AND VP.PID IN ( SELECT DISTINCT
													TRR2.PID
										  FROM		Tuhu_productcatalog.dbo.tbl_TireEnforceRecommend AS TRR2 WITH ( NOLOCK ) )
						  OR @ShowType = 3
						  AND VP.PID NOT  IN ( SELECT DISTINCT
														TRR2.PID
											   FROM		Tuhu_productcatalog.dbo.tbl_TireEnforceRecommend AS TRR2 WITH ( NOLOCK ) )
						  OR @ShowType = 4
						  AND VP.PID IN ( SELECT	TT.PID
										  FROM		( SELECT	ROW_NUMBER() OVER ( PARTITION BY TRR.PID, TRR.RecommendPID ORDER BY TRR.PKID ) AS RowNumber,
																*
													  FROM		Tuhu_productcatalog..tbl_TireEnforceRecommend AS TRR
													) AS TT
										  WHERE		TT.RowNumber > 1 ) )
		) AS T", CommandType.Text, new SqlParameter[] {
                                                     new SqlParameter("@TireSize",string.IsNullOrWhiteSpace(model.TireSize)?null:model.TireSize),
                                                     new SqlParameter("@PID",string.IsNullOrWhiteSpace(model.PID)?null:model.PID),
                                                     new SqlParameter("@RecommendPID",string.IsNullOrWhiteSpace(model.RecommendPID)?null:model.RecommendPID),
                                                     new SqlParameter("@Brands",string.IsNullOrWhiteSpace(model.Brands)||model.Brands.Contains("不限")?null:model.Brands),
                                                     new SqlParameter("@ShowType",model.ShowType)


                });
                return Convert.ToInt32(obj);
            }
        }

        public static IEnumerable<RecommendProductModel> SelectQZTJByPID(string PID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT	TRR.RecommendPID AS PID,TRR.Reason,TRR.Image,TRR.Position AS Postion
	                                               FROM	Tuhu_productcatalog.dbo.tbl_TireEnforceRecommend AS TRR WITH ( NOLOCK )
	                                               WHERE	TRR.PID = @PID", CommandType.Text, new SqlParameter("@PID", PID)).ConvertTo<RecommendProductModel>();
            }
        }

        public static IEnumerable<RecommendProductModel> SelectQZTJByPIDs(string PIDs)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT TRR.PID,TRR.RecommendPID,TRR.Reason,TRR.Image,
		TRR.Position AS Postion
FROM	Tuhu_productcatalog.dbo.tbl_TireEnforceRecommend AS TRR WITH ( NOLOCK )
WHERE	TRR.PID COLLATE Chinese_PRC_CI_AS IN ( SELECT	*
											   FROM		Gungnir.dbo.Split(@PIDS, ';') AS S )", CommandType.Text, new SqlParameter("@PIDS", PIDs)).ConvertTo<RecommendProductModel>();
            }
        }
        public static int DeleteQZTJByPID(string PID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"DELETE	Tuhu_productcatalog.dbo.tbl_TireEnforceRecommend
	                                              WHERE	PID = @PID", CommandType.Text, new SqlParameter("@PID", PID));
            }
        }
        public static int DeleteQZTJByPIDAndPostion(SqlDbHelper dbHelper, string PID, int Postion)
        {

            return dbHelper.ExecuteNonQuery(@"DELETE	Tuhu_productcatalog.dbo.tbl_TireEnforceRecommend
	                                              WHERE	PID = @PID AND Position=@Postion", CommandType.Text, new SqlParameter[] { new SqlParameter("@PID", PID), new SqlParameter("@Postion", Postion) });
        }
        public static string CheckPIDQZTJ(string TireSize, string PID)
        {

            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var obj = dbHelper.ExecuteScalar(@"SELECT  VP.DisplayName FROM Tuhu_productcatalog.dbo.vw_Products AS VP WITH(NOLOCK) WHERE VP.PID=@PID AND VP.TireSize=@TireSize", CommandType.Text,
                    new SqlParameter[] { new SqlParameter("@PID", PID), new SqlParameter("@TireSize", TireSize) });
                if (obj == DBNull.Value || obj == null)
                    return null;
                return obj.ToString();
            }
        }

        public static IEnumerable<ProductNameInfo> GetDisplayName(List<string> pids)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT PID ,
                                                          DisplayName
                                                   FROM   Tuhu_productcatalog.dbo.vw_Products WITH ( NOLOCK )
                                                   WHERE  PID COLLATE Chinese_PRC_CI_AS IN ( SELECT	* FROM	Gungnir.dbo.Split(@PIDS, ';') AS S )",
                                                   CommandType.Text,
                                                   new SqlParameter("@PIDS", string.Join(";", pids)))
                                                   .ConvertTo<ProductNameInfo>();
            }
        }

        public static int SaveQZTJSingle(QZTJModel model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var result = -99;
                dbHelper.BeginTransaction();
                //删
                DeleteQZTJByPID(model.PID);
                foreach (var item in model.Products)
                {
                    //插入
                    result = InsertQZTJ(dbHelper, item, model.PID);
                    if (result <= 0)
                        dbHelper.Rollback();
                }
                dbHelper.Commit();
                return result;
            }
        }
        public static int InsertQZTJ(SqlDbHelper dbHelper, RecommendProductModel model, string PID)
        {
            return dbHelper.ExecuteNonQuery(@"INSERT	INTO Tuhu_productcatalog.dbo.tbl_TireEnforceRecommend
                                             		(
                                             		  PID,
                                             		  RecommendPID,
                                             		  Position,
                                             		  Reason,
                                             		  Image )
                                             VALUES	(
                                             		  @PID,
                                             		  @RecommendPID,
                                             		  @Postion,
                                             		  @Reason,
                                             		  @Image )",
                 CommandType.Text,
                 new SqlParameter[] {
                    new SqlParameter("@PID",PID),
                    new SqlParameter("@RecommendPID",model.PID),
                    new SqlParameter("@Postion",model.Postion),
                    new SqlParameter("@Reason",model.Reason),
                      new SqlParameter("@Image",model.Image)
             });
        }


        public static int SaveQZTJMany(List<QZTJModel> list)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var result = -99;
                dbHelper.BeginTransaction();
                //var pids = list.Select(C => C.PID);
                //List<int> Postion = new List<int>();
                //if (list.FirstOrDefault().Products.Any(C => C.Postion == 1))
                //    Postion.Add(1);
                //else if (list.FirstOrDefault().Products.Any(C => C.Postion == 2))
                //    Postion.Add(2);
                //else if (list.FirstOrDefault().Products.Any(C => C.Postion == 3))
                //    Postion.Add(3);
                //DeleteQZTJMany(string.Join(";",pids), string.Join(";", Postion));
                foreach (var item in list)
                {
                    foreach (var model in item.Products)
                    {
                        DeleteQZTJByPIDAndPostion(dbHelper, item.PID, model.Postion);
                        result = InsertQZTJ(dbHelper, model, item.PID);
                        if (result <= 0)
                            dbHelper.Rollback();
                    }
                }
                dbHelper.Commit();
                return result;
            }

        }

        private static void DeleteQZTJMany(string PIDS, string PostionS)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<FORCache> SelectQZTJForCache()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT	TRR.PID,
                                                           TRR.RecommendPID
                                                   FROM    Tuhu_productcatalog.dbo.tbl_TireEnforceRecommend AS TRR WITH(NOLOCK)
                                                   ORDER BY TRR.Position").ConvertTo<FORCache>();
            }
        }
        #endregion

        #region 券后价相关

        #endregion
    }
}