using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalBaoYangYearCard
    {
        public static DataTable SelectAllBaoYangYearCard(SqlConnection conn, int pageIndex, int pageSize)
        {
            var sql = @"SELECT      
                 yearcard.PKID, 
                 yearcard.DisplayName,
                 yearcard.PID,
                 yearcard.Category,
                 yearcard.UpdateTime,
				 c.DisplayName as ProductName,
                 ISNULL(bd.PromotionIndex,0) AS PromotionCount 
                 FROM (SELECT * FROM BaoYang..BaoYangYearCard AS bc (NOLOCK)
				 ORDER BY bc.UpdateTime DESC
				 OFFSET (@PageIndex -1)*@PageSize ROWS FETCH NEXT @PageSize
                 ROWS ONLY) AS yearcard 
                 LEFT JOIN BaoYang.dbo.BaoYangYearCardDetail AS bd (NOLOCK)
                 ON yearcard.PKID = bd.YearCardId
                 LEFT JOIN Tuhu_productcatalog.dbo.[CarPAR_zh-CN] AS c (NOLOCK)
                 ON bd.PID = c.PID AND c.PrimaryParentCategory = N'oil'";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            return result;
        }

        public static DataTable SelectBaoYangYearCardWithCondition(SqlConnection conn, int pageIndex, int pageSize, 
            string pid, string category, string fuelBrand, string productId)
        {
            var sql = @"SELECT           
                 DISTINCT
                 yearcard.PKID, 
                 yearcard.DisplayName,
                 yearcard.PID,
                 yearcard.Category,
                 yearcard.UpdateTime
                 INTO #temp
                 FROM BaoYang..BaoYangYearCard AS yearcard (NOLOCK)
                 LEFT JOIN BaoYang.dbo.BaoYangYearCardDetail AS bd (NOLOCK)
                 ON yearcard.PKID = bd.YearCardId
                 LEFT JOIN Tuhu_productcatalog.dbo.[CarPAR_zh-CN] AS c (NOLOCK)
                 ON bd.PID = c.PID AND c.PrimaryParentCategory = N'oil'
                 WHERE ((@Pid IS NULL OR @Pid = '') 
                 OR yearcard.PID = @Pid )
                 AND((@Category IS NULL OR @Category = '') 
                 OR yearcard.Category = @Category)
                 AND ((@fuelBrand IS NULL OR @fuelBrand = '')
                 OR c.CP_Brand = @fuelBrand)
                 AND ((@productId IS NULL OR @productId = '')
                 OR bd.PID = @productId)
				 ORDER BY yearcard.pkid DESC
                 OFFSET (@PageIndex -1)*@PageSize ROWS FETCH NEXT @PageSize
                 ROWS ONLY


                 SELECT a.PKID, 
                 a.DisplayName,
                 a.PID,
                 a.Category,
                 c.DisplayName as ProductName,
                 ISNULL(bd.PromotionIndex,0) AS PromotionCount 
				 FROM #temp AS a
                 LEFT JOIN BaoYang.dbo.BaoYangYearCardDetail AS bd (NOLOCK)
                 ON a.PKID = bd.YearCardId
                 LEFT JOIN Tuhu_productcatalog.dbo.[CarPAR_zh-CN] AS c (NOLOCK)
                 ON bd.PID = c.PID AND c.PrimaryParentCategory = N'oil'
                 ORDER BY a.UpdateTime DESC
                 DROP TABLE #temp";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@Pid", pid),
                new SqlParameter("@Category", category),
                new SqlParameter("@fuelBrand", fuelBrand),
                new SqlParameter("@productId", productId)    
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            return result;
        }

        public static int SelectBaoYangYearCardCount(SqlConnection conn, string pid, string category, string fuelBrand, string productId)
        {
            var sql = @"SELECT ISNULL(COUNT(DISTINCT bc.PKID),0) AS TotalCount
                 FROM BaoYang..BaoYangYearCard AS bc (NOLOCK)
                 LEFT JOIN BaoYang.dbo.BaoYangYearCardDetail AS bd (NOLOCK)
                 ON bc.PKID = bd.YearCardId
                 LEFT JOIN Tuhu_productcatalog.dbo.[CarPAR_zh-CN] AS c (NOLOCK)
                 ON bd.PID = c.PID AND c.PrimaryParentCategory = N'oil'
                 WHERE ((@Pid IS NULL OR @Pid = '') 
                 OR bc.PID = @Pid )
                 AND((@Category IS NULL OR @Category = '') 
                 OR bc.Category = @Category)
                 AND ((@fuelBrand IS NULL OR @fuelBrand = '')
                 OR c.CP_Brand = @fuelBrand)
                 AND ((@productId IS NULL OR @productId = '')
                 OR bd.PID = @productId)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Pid", pid),
                new SqlParameter("@Category", category),
                new SqlParameter("@fuelBrand", fuelBrand),
                new SqlParameter("@productId", productId)
            };
            var dt = SqlHelper.ExecuteDataRow(conn, CommandType.Text, sql, parameters);
            var result = Convert.ToInt32(dt["TotalCount"]);
            return result;
        }

        public static List<string> SelectAllFuelBrand(SqlConnection conn)
        {
            var sql = @"SELECT DISTINCT CP_Brand  
                        FROM Tuhu_productcatalog.dbo.[CarPAR_zh-CN] (NOLOCK)
                        WHERE PrimaryParentCategory = N'oil' AND IsUsedInAdaptation = 1 
                        AND stockout = 0 AND OnSale = 1";
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            List<string> result = (from DataRow dr in dt.Rows select dr["CP_Brand"].ToString()).ToList();
            return result;
        }

        public static List<string> SelectAllOilLevel(SqlConnection conn)
        {
            var sql = @"SELECT DISTINCT CP_ShuXing1
                        FROM Tuhu_productcatalog.dbo.[CarPAR_zh-CN] (NOLOCK)
                        WHERE PrimaryParentCategory = N'oil' AND IsUsedInAdaptation = 1 
                        AND stockout = 0 AND OnSale = 1";
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            List<string> result = (from DataRow dr in dt.Rows select dr["CP_ShuXing1"].ToString()).ToList();
            return result;
        }

        public static List<string> SelectOilviscosity(SqlConnection conn)
        {
            var sql = @"SELECT DISTINCT CP_ShuXing2
                        FROM Tuhu_productcatalog.dbo.[CarPAR_zh-CN] (NOLOCK)
                        WHERE PrimaryParentCategory = N'oil' AND IsUsedInAdaptation = 1 
                        AND stockout = 0 AND OnSale = 1";
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            List<string> result = (from DataRow dr in dt.Rows select dr["CP_ShuXing2"].ToString()).ToList();
            return result;
        }

        public static List<string> SelectOilBundle(SqlConnection conn, string brand, string level, string viscosity)
        {
            var sql = @"SELECT DISTINCT 
                        DisplayName,PID
                        FROM Tuhu_productcatalog.dbo.[CarPAR_zh-CN] (NOLOCK)
                        WHERE PrimaryParentCategory = N'oil' AND IsUsedInAdaptation = 1 
                        AND stockout = 0 AND OnSale = 1
                        AND CP_Brand = @Brand AND CP_ShuXing1 = @Level AND CP_ShuXing2 = @Viscosity";
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@Brand", brand),
               new SqlParameter("@Level", level),
               new SqlParameter("@Viscosity", viscosity)  
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            return (from DataRow dr in dt.Rows select dr["DisplayName"] + "," + dr["PID"]).ToList();
        }

        public static Tuple<string,string> SelectOilDisplayNameByProperty(SqlConnection conn, string brand, string level, string viscosity, string unit)
        {
            Tuple<string, string> result = null;
            var sql = @"SELECT DISTINCT 
                        DisplayName, PID
                        FROM Tuhu_productcatalog.dbo.[CarPAR_zh-CN] (NOLOCK)
                        WHERE PrimaryParentCategory = N'oil' AND IsUsedInAdaptation = 1 
                        AND stockout = 0 AND OnSale = 1
                        AND CP_Brand = @Brand AND CP_ShuXing1 = @Level 
                        AND CP_ShuXing2 = @Viscosity AND CP_Unit=@Unit";
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@Brand", brand),
               new SqlParameter("@Level", level),
               new SqlParameter("@Viscosity", viscosity),
               new SqlParameter("@Unit", unit)
            };
            var dr = SqlHelper.ExecuteDataRow(conn, CommandType.Text, sql, parameters);
            return dr!=null ? new Tuple<string, string>(dr["PID"].ToString(), dr["DisplayName"].ToString()) : null ;
        }

        public static bool DeleteBaoYangYearCard(SqlConnection conn, int pkid)
        {
            var sql = @"DELETE FROM BaoYang.dbo.BaoYangYearCard WHERE PKID=@Pkid;
                        DELETE FROM BaoYang.dbo.BaoYangYearCardDetail WHERE YearCardId=@Pkid;
                        DELETE FROM BaoYang.dbo.BaoYangYearCardShop WHERE YearCardId=@Pkid;";
            SqlParameter parameter = new SqlParameter("@Pkid",pkid);
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        public static List<BaoYangYearCardDetail> SelectBaoYangYearCardDetails(SqlConnection conn, int pkid)
        {
            List<BaoYangYearCardDetail> result = new List<BaoYangYearCardDetail>();
            var sql = @"SELECT 
            bc.PKID,
            bc.YearCardId,
            bc.PackageType ,
            bc.BaoYangType ,
            bc.PID,
            c.DisplayName,
            bc.PromotionIndex,
            bc.ProductCount,
			ISNULL(bp.PromotionPercentage,0) AS PromotionPercentage
            FROM baoyang..BaoYangYearCardDetail (NOLOCK) AS bc
            LEFT JOIN Tuhu_productcatalog..[CarPAR_zh-CN] (NOLOCK) AS c
            ON c.PID = bc.PID
			LEFT JOIN BaoYang..BaoYangYearCardPromotionPercentage (NOLOCK) AS bp 
			ON bp.YearCardId = bc.YearCardId AND bp.PromotionIndex = bc.PromotionIndex
            WHERE bc.YearCardId = @Pkid
            ORDER BY bc.PKID ASC";
            SqlParameter parameter = new SqlParameter("@Pkid",pkid);
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter);
            foreach (DataRow dr in dt.Rows)
            {
                BaoYangYearCardDetail item  = new BaoYangYearCardDetail
                {
                    Pkid = Convert.ToInt32(dr["PKID"]),
                    YearCardId = Convert.ToInt32(dr["YearCardId"]),
                    PackageType = dr["PackageType"].ToString(),
                    BaoYangType = dr["BaoYangType"].ToString(),
                    Pid = dr["PID"].ToString(),
                    DisplayName = dr["DisplayName"].ToString(),
                    PromotionIndex = Convert.ToInt32(dr["PromotionIndex"]),
                    ProductCount = Convert.ToInt32(dr["ProductCount"]),
                    PromotionPercentage = Convert.ToDecimal(dr["PromotionPercentage"] ?? 0)
                };
                result.Add(item);
            }
            return result;
        }

        public static List<BaoYangYearCard> SelectYearCardInfoByPkid(SqlConnection conn, int pkid)
        {
            List<BaoYangYearCard> result = new List<BaoYangYearCard>();
            var sql = @"SELECT DISTINCT
            bc.PKID,
            bc.PID,
            bc.DisplayName,
            bc.Category,
            bc.ImageUrl,
            ISNULL(bs.ShopType, 0) AS ShopType,
            ISNULL(bs.ShopID,0) AS ShopID,
            ISNULL(bd.PromotionIndex,0) AS PromotionIndex,
            vs.CarparName,
            ISNULL(bp.PromotionPercentage,0) AS PromotionPercentage
            FROM 
            BaoYang..BaoYangYearCard (NOLOCK) AS bc
            LEFT JOIN BaoYang.dbo.BaoYangYearCardShop (NOLOCK) AS bs
            ON bc.PKID = bs.YearCardId
            LEFT JOIN BaoYang.dbo.BaoYangYearCardDetail (NOLOCK) AS bd
            ON bd.YearCardId = bc.PKID
            LEFT JOIN BaoYang..BaoYangYearCardPromotionPercentage (NOLOCK) AS bp 
			ON bp.YearCardId = bc.PKID AND bp.PromotionIndex = bd.PromotionIndex
            LEFT JOIN Gungnir.dbo.vw_Shop (NOLOCK) AS vs
            ON bs.ShopID = vs.PKID
            WHERE bc.PKID = @Pkid";
            SqlParameter parameter = new SqlParameter("@Pkid", pkid);
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter);
            foreach (DataRow dr in dt.Rows)
            {
                BaoYangYearCard item = new BaoYangYearCard
                {
                    Pkid = Convert.ToInt32(dr["PKID"]),
                    Pid = dr["PID"].ToString(),
                    DisplayName = dr["DisplayName"].ToString(),
                    CategoryName = dr["Category"].ToString(),
                    ImageUrl = dr["ImageUrl"].ToString(),
                    ShopType = Convert.ToInt32(dr["ShopType"] ?? 0),
                    ShopID = Convert.ToInt32(dr["ShopID"] ?? 0),
                    ShopName = dr["CarparName"].ToString(),
                    PromotionIndex = Convert.ToInt32(dr["PromotionIndex"] ?? 0),
                    PromotionPercentage = Convert.ToDouble(dr["PromotionPercentage"] ?? 0)
                };
                result.Add(item);
            }
            return result;
        }

        public static bool IsExistBaoYangYearCardPid(SqlConnection conn, string pid)
        {
            bool result = false;
            var sql = @"SELECT PID FROM BaoYang..BaoYangYearCard (NOLOCK) WHERE PID = @PID";
            SqlParameter parameter = new SqlParameter("@PID", pid);
            var dr = SqlHelper.ExecuteDataRow(conn, CommandType.Text, sql, parameter);
            if (!string.IsNullOrEmpty(dr?["PID"].ToString()))
            {
                result = true;
            }
            return result;
        }

        public static bool IsInputPidBelongsToYearCardPid(SqlConnection conn, string pid)
        {
            bool result = false;
            var sql = @"SELECT PID FROM Tuhu_productcatalog..[CarPAR_zh-CN](NOLOCK) 
                        WHERE PID = @PID AND PrimaryParentCategory = 'Card'";
            SqlParameter parameter = new SqlParameter("@PID", pid);
            var dr = SqlHelper.ExecuteDataRow(conn, CommandType.Text, sql, parameter);
            if (!string.IsNullOrEmpty(dr?["PID"].ToString()))
            {
                result = true;
            }
            return result;
        }

        public static string SelectProductNameByPid(SqlConnection conn, string pid)
        {
            var sql = @"SELECT DISTINCT DisplayName  
                        FROM Tuhu_productcatalog.dbo.[CarPAR_zh-CN] (NOLOCK)
                        WHERE IsUsedInAdaptation = 1 AND stockout = 0 AND OnSale = 1
                        AND PID = @PID";
            SqlParameter parameter = new SqlParameter("@PID", pid);
            var dr = SqlHelper.ExecuteDataRow(conn, CommandType.Text, sql, parameter);
            var result = dr?["DisplayName"].ToString() ?? string.Empty;
            return result;
        }

        public static List<BaoYangYearCardShop> SelectBaoYangYearCardShops(SqlConnection conn, int yearCardId)
        {
            List<BaoYangYearCardShop> result = new List<BaoYangYearCardShop>();
            var sql = @"SELECT 
                        bs.PKID,
                        bs.YearCardId,
                        ISNULL(bs.ShopType,0) AS ShopType,
                        ISNULL(bs.ShopID,0) AS ShopID
                        FROM baoyang..BaoYangYearCardShop (NOLOCK) AS bs
                        WHERE bs.YearCardId = @YearCardId";
            SqlParameter parameter = new SqlParameter("@YearCardId", yearCardId);
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    BaoYangYearCardShop item = new BaoYangYearCardShop
                    {
                        Pkid = Convert.ToInt32(dr["PKID"]),
                        YearCardId = Convert.ToInt32(dr["YearCardId"]),
                        ShopType = Convert.ToInt32(dr["ShopType"]),
                        ShopID = Convert.ToInt32(dr["ShopID"])
                    };
                    result.Add(item);
                }
            }
            return result;
        }

        public static bool DeleteBaoYangYearCardShop(SqlConnection conn, int yearCardId)
        {
            var sql = @"DELETE FROM BaoYang..BaoYangYearCardShop WHERE YearCardId = @YearCardId";
            SqlParameter parameter = new SqlParameter("@YearCardId", yearCardId);
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        public static bool AddBaoYangYearCardShop(SqlConnection conn, int yearCardId, int shopType, int shopId)
        {
            var sql = @"INSERT INTO baoyang..BaoYangYearCardShop
            ( YearCardId ,
              ShopType ,
              ShopID ,
              CreateTime
            )
            VALUES  ( @YearCardId , 
                      @ShopType , 
                      @ShopID , 
                      GETDATE()  
                     )";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@YearCardId", yearCardId),
                new SqlParameter("@ShopType", shopType == 0 ? (int?) null : shopType),
                new SqlParameter("@ShopID", shopId == 0 ? (int?) null : shopId),   
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }


        public static bool DeleteYearCardProjectItem(SqlConnection conn, int yearCardId)
        {
            var sql = @"DELETE FROM BaoYang.dbo.BaoYangYearCardDetail
                        WHERE YearCardId = @YearCardId";
            SqlParameter parameter = new SqlParameter("@YearCardId", yearCardId);
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        public static bool AddBaoYangYearCardDetail(SqlConnection conn, BaoYangYearCardDetail details)
        {
            var sql = @"INSERT INTO BaoYang..BaoYangYearCardDetail
           ( YearCardId ,
             PromotionIndex ,
             PackageType ,
             BaoYangType ,
             PID ,
             ProductCount ,
             CreateTime
          )
          VALUES  ( @YearCardId , 
          @PromotionIndex , 
          @PackageType , 
          @BaoYangType , 
          @PID , 
          @ProductCount ,
          GETDATE()  
          )";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@YearCardId", details.YearCardId),
                new SqlParameter("@PromotionIndex", details.PromotionIndex),
                new SqlParameter("@PackageType", details.PackageType),
                new SqlParameter("@BaoYangType", details.BaoYangType),
                new SqlParameter("@PID", details.Pid),
                new SqlParameter("@ProductCount", details.ProductCount)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        public static int AddBaoYangYearCard(SqlConnection conn, YearCardParameter yearCard)
        {
            var sql = @"INSERT INTO BaoYang..BaoYangYearCard
        ( PID ,
          DisplayName ,
          Category ,
          ImageUrl ,
          CreateTime ,
          UpdateTime
        )
        VALUES  ( @PID , -- PID - nvarchar(513)
          @DisplayName , -- DisplayName - nvarchar(100)
          @Category , -- Category - nvarchar(20)
          @ImageUrl , -- ImageUrl - nvarchar(200)
          GETDATE() , -- CreateTime - datetime
          GETDATE()  -- UpdateTime - datetime
        )
        SELECT SCOPE_IDENTITY()";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PID", yearCard.Pid),
                new SqlParameter("@DisplayName", yearCard.DisplayName),
                new SqlParameter("@Category", yearCard.CategoryName),
                new SqlParameter("@ImageUrl", yearCard.ImageUrl)
            };
            var data = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
            return data!=null ? Convert.ToInt32(data) : 0;
        }

        public static bool UpdateBaoYangYearCardInfo(SqlConnection conn, YearCardParameter yearCard)
        {
            var sql = @"UPDATE BaoYang..BaoYangYearCard
                        SET PID = @PID,
                        DisplayName = @DisplayName,
	                    Category = @Category,
	                    ImageUrl = @ImageUrl,
	                    UpdateTime = GETDATE()
                        WHERE PKID = @YearCardId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@YearCardId", yearCard.Pkid), 
                new SqlParameter("@PID", yearCard.Pid),
                new SqlParameter("@DisplayName", yearCard.DisplayName),
                new SqlParameter("@Category", yearCard.CategoryName),
                new SqlParameter("@ImageUrl", yearCard.ImageUrl)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        public static bool AddLogToBaoYangOprLog(SqlConnection conn, string logType, string identityId, string oldValue, string newValue, string operateUser, string remark)
        {
            var sql = @"INSERT INTO Tuhu_log.dbo.BaoYangOprLog
        ( LogType ,
          IdentityID ,
          OldValue ,
          NewValue ,
          OperateUser ,
          Remarks ,
          CreateTime
        )
          VALUES  ( @LogType , -- LogType - nvarchar(100)
          @IdentityID , -- IdentityID - nvarchar(513)
          @OldValue , -- OldValue - nvarchar(max)
          @NewValue , -- NewValue - nvarchar(max)
          @OperateUser , -- OperateUser - nvarchar(100)
          @Remark , -- Remarks - nvarchar(200)
          GETDATE()  -- CreateTime - datetime
        )";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@LogType", logType),
                new SqlParameter("@IdentityID", identityId),
                new SqlParameter("@NewValue", newValue),
                new SqlParameter("@OldValue", oldValue),
                new SqlParameter("@OperateUser", operateUser),
                new SqlParameter("@Remark", remark)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        public static List<YearCardConfig> SelectYearCardConfig(SqlConnection conn)
        {
            List<YearCardConfig> result = new List<YearCardConfig>();
            var sql = @"SELECT ISNULL(yc.YearCardType, yr.YearCardType) AS YearCardType ,
            yc.Icon ,
            yc.PanelImage ,
            yr.Brands
            FROM  BaoYang..BaoYangYearCardTypeConfig AS yc ( NOLOCK )
            FULL OUTER JOIN BaoYang..BaoYangYearCardRecommendConfig AS yr ( NOLOCK ) ON yr.YearCardType = yc.YearCardType;";
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            foreach (DataRow dr in dt.Rows)
            {
                YearCardConfig item = new YearCardConfig
                {
                    YearCardType = dr["YearCardType"].ToString(),
                    Icon = dr["Icon"].ToString(),
                    PanelImage = dr["PanelImage"].ToString(),
                    Brands = dr["Brands"].ToString()
                };
                result.Add(item);
            }
            return result;
        }

        public static DataTable SelectYearCardConfigByYearType(SqlConnection conn, string category)
        {
            var sql = @"SELECT 
            PKID  ,
            YearCardType , 
            Icon ,
            PanelImage ,
            UpdateTime ,
            CreateTime 
            FROM BaoYang..BaoYangYearCardTypeConfig (NOLOCK)
            WHERE YearCardType = @Category";
            SqlParameter parameter = new SqlParameter("@Category", category);
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter);
        }

        public static DataTable SelectYearCardRecommendConfig(SqlConnection conn, string category)
        {
            var sql = @"SELECT 
            PKID ,
            YearCardType ,
            Brands ,
            BaoYangType ,
            UpdateTime ,
            CreateTime 
            FROM BaoYang..BaoYangYearCardRecommendConfig (NOLOCK)
            WHERE YearCardType = @Category";
            SqlParameter parameter = new SqlParameter("@Category", category);
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter);
        }

        public static bool DeleteYearCardConfig(SqlConnection conn, string category)
        {
            var sql = @"DELETE FROM BaoYang..BaoYangYearCardTypeConfig WHERE YearCardType = @Category";
            SqlParameter parameter = new SqlParameter("@Category", category);
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        public static bool DeleteYearCardRecommendConfig(SqlConnection conn, string category)
        {
            var sql = @"DELETE FROM BaoYang..BaoYangYearCardRecommendConfig WHERE YearCardType = @Category";
            SqlParameter parameter = new SqlParameter("@Category", category);
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        public static bool AddYearCardConfig(SqlConnection conn, string category, string icon, string panelImage)
        {
            var sql = @"INSERT INTO BaoYang..BaoYangYearCardTypeConfig
                        ( YearCardType ,
                          Icon ,
                          PanelImage ,
                          UpdateTime ,
                          CreateTime
                        )
                        VALUES  ( @Category , 
                                  @Icon , 
                                  @PanelImage , 
                                  GETDATE() , 
                                  GETDATE()  
                                 )";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Category", category),
                new SqlParameter("@Icon", icon),
                new SqlParameter("@PanelImage", panelImage)   
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        public static bool AddYearCardRecommendConfig(SqlConnection conn, string category, string brands, string baoyangType)
        {
            var sql = @"INSERT INTO BaoYang..BaoYangYearCardRecommendConfig
                        ( YearCardType ,
                          Brands ,
                          BaoYangType,
                          UpdateTime ,
                          CreateTime
                         )
                         VALUES  ( @Category , 
                         @Brands ,
                         @BaoYangType,
                         GETDATE() , 
                         GETDATE()  
                         )";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Category", category),
                new SqlParameter("@Brands", brands),
                new SqlParameter("@BaoYangType", baoyangType)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        public static bool UpdateYearCardConfig(SqlConnection conn, string category, string icon, string panelImage)
        {
            var sql = @"UPDATE BaoYang..BaoYangYearCardTypeConfig
                       SET Icon = @Icon,
                       PanelImage = @PanelImage,
	                   UpdateTime = GETDATE()
                       WHERE YearCardType = @Category";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Category", category),
                new SqlParameter("@Icon", icon),
                new SqlParameter("@PanelImage", panelImage)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        public static bool UpdateYearCardRecommendConfig(SqlConnection conn, string category, string brands, string baoyangType)
        {
            var sql = @"UPDATE BaoYang..BaoYangYearCardRecommendConfig
                        SET Brands = @Brands,
                        BaoYangType = @BaoYangType,
                        UpdateTime = GETDATE()
                        WHERE YearCardType = @Category";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Category", category),
                new SqlParameter("@Brands", brands),
                new SqlParameter("@BaoYangType", baoyangType)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        public static List<BaoYangYearCardPromotion> SelectBaoYangYearCardPromotion(SqlConnection conn, int yearCardId)
        {
            List<BaoYangYearCardPromotion> result = new List<BaoYangYearCardPromotion>();
            var sql = @"SELECT 
                        PKID, 
                        YearCardId,
                        PromotionIndex,
                        PromotionPercentage,
                        CreateTime,
                        UpdateTime
                        FROM BaoYang..BaoYangYearCardPromotionPercentage (NOLOCK) 
                        WHERE YearCardId = @YearCardId";
            SqlParameter parameter = new SqlParameter("@YearCardId", yearCardId);
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter);
            foreach (DataRow dr in dt.Rows)
            {
                BaoYangYearCardPromotion item = new BaoYangYearCardPromotion
                {
                    Pkid = Convert.ToInt32(dr["PKID"]),
                    YearCardId = Convert.ToInt32(dr["YearCardId"]),
                    PromotionIndex = Convert.ToInt32(dr["PromotionIndex"]),
                    PromotionPercentage = Convert.ToDecimal(dr["PromotionPercentage"]),
                    CreateTime = dr["CreateTime"].ToString(),
                    UpdateTime = dr["UpdateTime"].ToString()
                };
                result.Add(item);
            }
            return result;
        }

        public static bool DeleteYearCardPromotionPercentage(SqlConnection conn, int yearCardId)
        {
            var sql = @"DELETE FROM BaoYang.dbo.BaoYangYearCardPromotionPercentage
                        WHERE YearCardId = @YearCardId";
            SqlParameter parameter = new SqlParameter("@YearCardId", yearCardId);
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        public static bool AddYearCardPromotionPercentage(SqlConnection conn, BaoYangYearCardPromotion cardInfo)
        {
            var sql = @"INSERT INTO BaoYang..BaoYangYearCardPromotionPercentage
                       ( YearCardId ,
                         PromotionIndex ,
                         PromotionPercentage ,
                         CreateTime ,
                         UpdateTime
                        )
                        VALUES  ( @YearCardId , 
                                  @PromotionIndex , 
                                  @PromotionPercentage , 
                                  GETDATE() , 
                                  GETDATE()  
                                 )";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@YearCardId", cardInfo.YearCardId),
                new SqlParameter("@PromotionIndex", cardInfo.PromotionIndex),
                new SqlParameter("@PromotionPercentage", cardInfo.PromotionPercentage/100)   
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }
    }
}
