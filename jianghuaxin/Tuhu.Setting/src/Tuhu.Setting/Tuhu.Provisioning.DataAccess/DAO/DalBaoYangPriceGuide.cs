using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalBaoYangPriceGuide
    {
        public static List<BaoYangPriceGuideList> SelectBaoYangProductInfo(BaoYangPriceSelectModel model)
        {
            var result = new List<BaoYangPriceGuideList>();
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                #region sql
                var sql = @"SELECT  VP.CP_Brand AS Brand ,
                                VP.PID ,
                                VP.DisplayName AS ProductName ,
                                VP.stockout AS StockStatus,
								VP.OnSale AS OnSaleStatus,
                                VP.Category,
                                DPSD.totalstock ,
                                DPSD.SelfStock ,
                                DPSD.num_week ,
                                DPSD.num_month ,
                                DPSD.num_threemonth ,
                                DPSD.cost ,
                                DPSD.PurchasePrice ,
                                VP.cy_list_price AS Price ,
                                DPSD.taobao_tuhuid AS TBPID ,
                                DPSD.taobao_tuhuprice AS TBPrice ,
                                DPSD.taobao2_tuhuid AS TB2PID ,
                                DPSD.taobao2_tuhuprice AS TB2Price ,
                                DPSD.tianmao1_tuhuprice AS TM1Price ,
                                DPSD.tianmao1_tuhuid AS TM1PID ,
                                DPSD.tianmao2_tuhuprice AS TM2Price ,
                                DPSD.tianmao2_tuhuid AS TM2PID ,
                                DPSD.tianmao3_tuhuprice AS TM3Price ,
                                DPSD.tianmao3_tuhuid AS TM3PID ,
                                DPSD.tianmao4_tuhuid AS TM4PID ,
                                DPSD.tianmao4_tuhuprice AS TM4Price ,
                                DPSD.jingdong_tuhuprice AS JDPrice ,
                                DPSD.jingdong_tuhuid AS JDPID ,
                                DPSD.jingdongflagship_tuhuprice AS JDFlagShipPrice ,
                                DPSD.jingdongflagship_tuhuid AS JDFlagShipPID ,
                                DPSD.jingdongself_price AS JDSelfPrice ,
                                DPSD.jingdongself_id AS JDSelfPID ,
                                DPSD.teweilun_tianmaoprice AS TWLTMPrice ,
                                DPSD.teweilun_tianmaoid AS TWLTMPID,
                                DPSD.qccr_retailprice AS QccrlPrice ,
                                DPSD.qccr_retailid AS QccrlId ,
                                DPSD.qccr_wholesaleprice AS QccrpPrice ,
                                DPSD.qccr_wholesaleid AS QccrpId,
                                DPSD.yangche51_id AS YcwyId,
                                DPSD.yangche51_price AS YcwyPrice,
                                DPSD.carzone_id AS KzId,
                                DPSD.carzone_price AS KzPrice
                      FROM      Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
                                LEFT JOIN Tuhu_bi.dbo.dm_Product_SalespredictData
                                AS DPSD WITH ( NOLOCK ) ON VP.PID = DPSD.pid
                      WHERE     ( ( @PID IS NULL
                                       OR @PID = ''
                                )
                                   OR VP.PID = @PID
                                )
								AND VP.Category IN (
                                                SELECT  CategoryName                                                
                                                FROM
                                                Tuhu_productcatalog..vw_ProductCategories (NOLOCK)
                                                WHERE NodeNo LIKE N'%' + @FirstType + N'%'
                                                AND ( ( @SecondType IS NULL
                                                        OR @SecondType = ''
                                                       )
                                                       OR NodeNo LIKE N'%' + @SecondType + N'%'
                                                     )
                                                AND ( ( @ThirdType IS NULL
                                                        OR @ThirdType = ''
                                                       )
                                                       OR NodeNo LIKE N'%' + @ThirdType + N'%'
                                                     ))
                                AND ( VP.DisplayName LIKE '%' + @ProductName
                                      + '%'
                                      OR @ProductName IS NULL
                                    )
                                AND ( VP.CP_Brand = @Brand
                                      OR @Brand IS NULL
                                    )
                               AND ( @StockStatus = 0
                                      OR @StockStatus = 1
                                      AND VP.stockout = 0
                                      OR @StockStatus = 2
                                      AND VP.stockout = 1
                                    )
                               AND ( @OnSaleStatus = 0
                                      OR @OnSaleStatus = 1
                                      AND VP.OnSale = 1
                                      OR @OnSaleStatus = 2
                                      AND VP.OnSale = 0
                                    )";
                #endregion
               SqlParameter[] parameters =
               {
                      new SqlParameter("@PID",string.IsNullOrWhiteSpace(model.PID)?null:model.PID),
                      new SqlParameter("@ProductName",string.IsNullOrWhiteSpace(model.ProductName)?null:model.ProductName),
                      new SqlParameter("@Brand",string.IsNullOrWhiteSpace(model.Brand)?null:model.Brand),
                      new SqlParameter("@StockStatus",model.StockStatus),
                      new SqlParameter("@OnSaleStatus",model.OnSaleStatus),
                      new SqlParameter("@FirstType", model.FirstType),
                      new SqlParameter("@SecondType", model.SecondType),
                      new SqlParameter("@ThirdType", model.ThirdType)
               };
                var dt = dbHelper.ExecuteDataTable(sql, CommandType.Text, parameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    result = dt.ConvertTo<BaoYangPriceGuideList>().ToList();
                }
                return result;
            }
        }

        public static IEnumerable<BaoYangPriceWeight> SelectAllWeight()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sql = @"SELECT * FROM Tuhu_productcatalog.dbo.tbl_BaoYangPriceWeight (NOLOCK)";
                return dbHelper.ExecuteDataTable(sql, CommandType.Text).ConvertTo<BaoYangPriceWeight>();
            }
        }     

        public static IEnumerable<BaoYangShopStock> SelectBaoYangShopStock(string pids)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("WmsReadOnly")))
            {
                var sql = @"WITH pids AS (SELECT * FROM WMS..SplitString(@Pids,',',1))
        SELECT  sl.PID ,
        ISNULL(SUM(Num),0) AS StockQuantity ,
        sl.LocationId AS ShopId,
        sl.Location AS ShopName
        FROM WMS.dbo.StockLocation AS SL WITH ( NOLOCK )
        JOIN pids ON SL.PID = pids.item
		GROUP BY SL.PID, SL.LocationId, SL.Location";
                var param = new SqlParameter("@Pids", pids);
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, param).ConvertTo<BaoYangShopStock>();
            }
        }

        public static Dictionary<string, int> SelectBaoYangShopStockSum(string pids, string shopIds)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("WmsReadOnly")))
            {
                var sql = @"WITH pids AS (SELECT * FROM WMS..SplitString(@Pids,',',1)),
	     shopids AS (SELECT * FROM WMS..SplitString(@ShipIds,',',1))
        SELECT  sl.PID ,
        ISNULL(SUM(Num),0) AS StockQuantity
        FROM WMS.dbo.StockLocation AS SL WITH ( NOLOCK )
        JOIN pids ON SL.PID = pids.item
		JOIN shopids ON sl.LocationId = shopIds.item
		GROUP BY SL.PID";
                DbParameter[] param = {
                    new SqlParameter("@Pids", pids),
                    new SqlParameter("@ShipIds", shopIds)
                };
                var dt = dbHelper.ExecuteDataTable(sql, CommandType.Text, param);
                foreach (DataRow dr in dt.Rows)
                {
                    var pid = dr["PID"].ToString();
                    result[pid] = Convert.ToInt32(dr["StockQuantity"]);
                }
            }
            return result;
        }

        public static IEnumerable<BaoYangShopStock> SelectShopSaleNum(string pid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_BI")))
            {
                var sql = @"SELECT  ShopID ,
                            PID ,
                            ISNULL(SalesNum,0) as SaleNum
                            FROM   Tuhu_bi.[dbo].[dm_TuhuShopMonthSales] (NOLOCK)
                            WHERE   PID = @PID;";
                var param = new SqlParameter("@PID", pid);
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, param).ConvertTo<BaoYangShopStock>();
            }
        }

        /// <summary>
        /// 查询所有保养品品牌
        /// </summary>
        /// <returns></returns>
        public static List<string> SelectBaoYangBrands(string firstType, string secondType, string thirdType)
        {
            List<string> result = new List<string>();
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sql = @"SELECT DISTINCT CP_Brand
            FROM      Tuhu_productcatalog.dbo.vw_Products (NOLOCK)
            WHERE     Category IN (
                      SELECT  CategoryName
                      FROM    Tuhu_productcatalog..vw_ProductCategories (NOLOCK)
                      WHERE   NodeNo LIKE N'%' + @FirstType + N'%'
                            AND ( ( @SecondType IS NULL
                                    OR @SecondType = ''
                                   )
                                   OR NodeNo LIKE N'%' + @SecondType + N'%'
                                 )
                            AND ( ( @ThirdType IS NULL
                                    OR @ThirdType = ''
                                   )
                                   OR NodeNo LIKE N'%' + @ThirdType + N'%'
                                 )
		                    AND CP_Brand IS NOT NULL)
                    ORDER BY CP_Brand ASC";
                DbParameter[] param =
                {
                    new SqlParameter("@FirstType", firstType),
                    new SqlParameter("@SecondType", secondType),
                    new SqlParameter("@ThirdType", thirdType),
                };
                var dt = dbHelper.ExecuteDataTable(sql, CommandType.Text, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        result.Add(dr["CP_Brand"].ToString());
                    }
                }
                return result;
            }        
        }

        /// <summary>
        /// 查询保养品分仓信息
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static List<Product_SalespredictData> SelectProductSalespredictData(string pid)
        {
            List<Product_SalespredictData> result = new List<Product_SalespredictData>();
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_BI")))
            {
                var sql = @"SELECT  pid ,
        ISNULL(totalstock, 0) AS totalstock ,
        ISNULL(num_threemonth, 0) AS num_threemonth ,
        ISNULL(num_month, 0) AS num_month ,
        ISNULL(num_week, 0) AS num_week
FROM    Tuhu_bi..dm_Product_SalespredictData (NOLOCK)
WHERE   pid = @pid;

SELECT  pid ,
        warehouseid ,
        warehousename ,
        ISNULL(stocknum, 0) AS totalstock ,
        ISNULL(num_threemonth, 0) AS num_threemonth ,
        ISNULL(num_month, 0) AS num_month ,
        ISNULL(num_week, 0) AS num_week
FROM    Tuhu_bi..dm_Product_Warehouse_SalespredictData (NOLOCK)
WHERE   pid = @pid
        AND warehouseid IS NOT NULL;";
                var param = new SqlParameter("@Pid", pid);
                var ds = dbHelper.ExecuteDataSet(sql, CommandType.Text, param);
                var totalData = ds.Tables[0];
                if (totalData != null && totalData.Rows.Count > 0)
                {
                    var item = new Product_SalespredictData
                    {
                        WareHouseName = "总库存",
                        TotalStock = Convert.ToInt32(totalData.Rows[0]["totalstock"]),
                        Num_ThreeMonth = Convert.ToInt32(totalData.Rows[0]["num_threemonth"]),
                        Num_Month = Convert.ToInt32(totalData.Rows[0]["num_month"]),
                        Num_Week = Convert.ToInt32(totalData.Rows[0]["num_week"]),
                    };
                    result.Add(item);
                }
                var regionData = ds.Tables[1];
                if (regionData != null && regionData.Rows.Count > 0)
                {
                    foreach (DataRow row in regionData.Rows)
                    {
                        var item = new Product_SalespredictData
                        {
                            WareHouseName = row["warehousename"].ToString(),
                            TotalStock = Convert.ToInt32(row["totalstock"]),
                            Num_ThreeMonth = Convert.ToInt32(row["num_threemonth"]),
                            Num_Month = Convert.ToInt32(row["num_month"]),
                            Num_Week = Convert.ToInt32(row["num_week"]),
                        };
                        result.Add(item);
                    }
                }
                return result;
            }
        }

        static readonly Dictionary<string, string> dicGuidePara = new Dictionary<string, string>() {
            { "Brand", "CP_Brand" },
            { "Category","Category" },
            { "Base", "" },
        };

        /// <summary>
        /// 查询权重信息
        /// </summary>
        /// <param name="firstType"></param>
        /// <param name="secondType"></param>
        /// <param name="thirdType"></param>
        /// <returns></returns>
        public static DataTable SelectGuideParaByType(string firstType, string secondType, string thirdType)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {

                var sql = @"SELECT  DISTINCT
        vc.DisplayName AS Item ,
        vc.CategoryName AS Link ,
        VP.CP_Brand ,
        vc.oid ,
        vc.ParentOid ,
        vc.NodeNo
FROM    Tuhu_productcatalog..vw_ProductCategories AS vc WITH ( NOLOCK )
        LEFT JOIN Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK ) ON VP.Category = vc.CategoryName
WHERE   vc.NodeNo LIKE N'%' + @FirstType + N'%'
        AND ( ( @SecondType IS NULL
                OR @SecondType = ''
              )
              OR vc.NodeNo LIKE N'%' + @SecondType + N'%'
            )
        AND ( ( @ThirdType IS NULL
                OR @ThirdType = ''
              )
              OR vc.NodeNo LIKE N'%' + @ThirdType + N'%'
            )
        AND vc.ParentOid != -1
        AND ( vc.ParentOid != @FirstType
              OR @FirstType = N'193649'
            );";
                DbParameter[] param = 
                {
                    new SqlParameter("@FirstType", firstType),
                    new SqlParameter("@SecondType", secondType),
                    new SqlParameter("@ThirdType", thirdType),
                };
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, param);
            }

        }

        public static Dictionary<string, string> SelectProductCategoryByParentOid(int oid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                var sql = @"SELECT oid,DisplayName FROM  Tuhu_productcatalog..vw_ProductCategories (NOLOCK)
                            WHERE ParentOid = @oid";
                SqlParameter param = new SqlParameter("@oid", oid);
                var dt = dbHelper.ExecuteDataTable(sql, CommandType.Text, param);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        result[dr["oid"].ToString()] = dr["DisplayName"].ToString();
                    }
                }
                return result;
            }
        }

        public static Tuple<string, string> SelectProductCategoryByOid(int oid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                Tuple<string, string> result = null;
                var sql = @"SELECT oid,DisplayName FROM  Tuhu_productcatalog..vw_ProductCategories (NOLOCK)
                            WHERE Oid = @oid";
                SqlParameter param = new SqlParameter("@oid", oid);
                var ds = dbHelper.ExecuteDataSet(sql, CommandType.Text, param);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                    result =  Tuple.Create(ds.Tables[0].Rows[0]["oid"].ToString(),
                        ds.Tables[0].Rows[0]["DisplayName"].ToString());
                return result;
            }

        }

        /// <summary>
        /// 查询预警线信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<BaoYangWarningLine> SelectWarningLine()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  *
                                                   FROM    Configuration..tbl_BaoYang_WarningLine WITH ( NOLOCK ) ORDER BY MinGuidePrice;").ConvertTo<BaoYangWarningLine>();
            }
        }

        /// <summary>
        /// 更新预警线信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int UpdateWarningLine(BaoYangWarningLine model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"UPDATE  Configuration..tbl_BaoYang_WarningLine  SET LowerLimit=@LowerLimit,UpperLimit=@UpperLimit WHERE PKID=@PKID", CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@LowerLimit",model.LowerLimit),
                    new SqlParameter("@UpperLimit",model.UpperLimit),
                    new SqlParameter("@PKID",model.PKID)
                });
            }
        }

        /// <summary>
        /// 删除权重信息
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static int DeleteGuidePara(int PKID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"DELETE Tuhu_productcatalog.dbo.tbl_BaoYangPriceWeight WHERE PKID=@PKID", CommandType.Text, new SqlParameter("@PKID", PKID));
            }
        }

        /// <summary>
        /// 更新权重值
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int UpdateGuidePara(BaoYangPriceWeight model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"UPDATE Tuhu_productcatalog.dbo.tbl_BaoYangPriceWeight SET WeightValue=@Value,UpdateTime=GETDATE() WHERE PKID=@PKID", CommandType.Text, new SqlParameter[] { new SqlParameter("@Value", model.WeightValue), new SqlParameter("@PKID", model.PKID) });
            }
        }

        /// <summary>
        /// 插入权重配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertGuidePara(BaoYangPriceWeight model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"INSERT INTO Tuhu_productcatalog.dbo.tbl_BaoYangPriceWeight
                                                  		( WeightType, WeightName, WeightValue, CategoryName )
                                                  VALUES	( @Type, @Item, @Value, @CategoryName )", CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@Value", model.WeightValue),
                    new SqlParameter("@Type", model.WeightType),
                    new SqlParameter("@Item", model.WeightName),
                    new SqlParameter("@CategoryName", model.CategoryName) });
            }
        }

        /// <summary>
        /// 查询加权值修改日志
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static IEnumerable<ConfigHistory> SelectWeightOprLog(string objectType, string operation)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  *
                                                   FROM    [Tuhu_Log].[dbo].[BaoYangPriceGuideLog] WITH ( NOLOCK )
                                                   WHERE   ObjectType = @ObjectType
                                                           AND Operation = @Operation 
                                                   ORDER BY ChangeDatetime DESC;", CommandType.Text, new SqlParameter[] { new SqlParameter("@ObjectType", objectType), new SqlParameter("@Operation", operation) }).ConvertTo<ConfigHistory>();
            }
        }

        public static IEnumerable<ConfigHistory> SelectWarnOprLog(string objectType, string aftervalue)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  *
                                                   FROM    [Tuhu_Log].[dbo].[BaoYangPriceGuideLog] WITH ( NOLOCK )
                                                   WHERE   ObjectType = @ObjectType
                                                           AND AfterValue = @AfterValue 
                                                   ORDER BY ChangeDatetime DESC;", CommandType.Text, new SqlParameter[] { new SqlParameter("@ObjectType", objectType), new SqlParameter("@AfterValue", aftervalue) }).ConvertTo<ConfigHistory>();
            }
        }

        /// <summary>
        /// 查询保养待审核记录
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PriceUpdateAuditModel> SelectNeedAuditBaoYang()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  EUTP.PKID ,
        EUTP.PID ,
        EUTP.ApplyPrice ,
        EUTP.ApplyReason ,
        EUTP.ApplyDateTime ,
        EUTP.ApplyPerson ,
        EUTP.AuditDateTime ,
        EUTP.AuditPerson ,
        EUTP.AuditStatus ,
        VP.CP_Brand AS Brand ,
        VP.DisplayName AS ProductName ,
        DPSD.cost AS Cost,
        DPSD.PurchasePrice ,
        DPSD.totalstock AS TotalStock,
        DPSD.num_week AS Num_Week,
        DPSD.num_month AS Num_Month,
        DPSD.jingdongself_price AS JDSelfPrice ,
        ( ISNULL(( SELECT TOP 1
                            TPGP.WeightValue
                   FROM     Tuhu_productcatalog.dbo.tbl_BaoYangPriceWeight AS TPGP
                            WITH ( NOLOCK )
                   WHERE    TPGP.WeightType = 'Base'
                 ), 0)
          + ISNULL(( SELECT TOP 1
                            TPGP.WeightValue
                     FROM   Tuhu_productcatalog.dbo.tbl_BaoYangPriceWeight AS TPGP
                            WITH ( NOLOCK )
                     WHERE  TPGP.WeightType = 'Brand'
                            AND TPGP.WeightName = VP.CP_Brand
                   ), 0)
          + +ISNULL(( SELECT TOP 1
                                TPGP.WeightValue
                      FROM      Tuhu_productcatalog.dbo.tbl_BaoYangPriceWeight
                                AS TPGP WITH ( NOLOCK )
                      WHERE     TPGP.WeightType = 'Category'
                                AND TPGP.WeightName = VP.Category
                    ), 0) ) AS JiaQuan ,
        VP.cy_list_price AS ListPrice
FROM    Configuration.dbo.tbl_BaoYangPriceUpdateAudit AS EUTP WITH ( NOLOCK )
        INNER JOIN Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK ) ON EUTP.PID = VP.PID
        LEFT  JOIN Tuhu_bi.dbo.dm_Product_SalespredictData AS DPSD WITH ( NOLOCK ) ON EUTP.PID = DPSD.pid
WHERE   EUTP.AuditStatus = 0; ").ConvertTo<PriceUpdateAuditModel>();
            }
        }

        /// <summary>
        /// 新增保养价格审核记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int ApplyUpdatePrice(PriceUpdateAuditModel model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                ApplyUpdatePrice(dbHelper, model.PID);
                return dbHelper.ExecuteNonQuery(@" INSERT	INTO Configuration.dbo.tbl_BaoYangPriceUpdateAudit
                                                   		(
                                                   		  PID,
                                                   		  ApplyPrice,
                                                   		  ApplyReason,
                                                   		  ApplyPerson,
                                                   		  ApplyDateTime,
                                                   		  AuditStatus )
                                                   VALUES	(
                                                   		  @PID,
                                                   		  @Price,
                                                   		  @Reason,
                                                   		  @ApplyPerson,
                                                   		  GETDATE(),
                                                   		  0 )", CommandType.Text,
                                                          new SqlParameter[] {
                                                              new SqlParameter("@PID", model.PID),
                                                              new SqlParameter("@Price", model.ApplyPrice),
                                                               new SqlParameter("@Reason", model.ApplyReason),
                                                                new SqlParameter("@ApplyPerson", model.ApplyPerson)
                                                          });
            }
        }

        /// <summary>
        /// 同一个保养品多次提交审核，如果之前的有未审核，则之前的提交申请被最新的一条申请覆盖，
        /// 并保留申请记录
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static int ApplyUpdatePrice(SqlDbHelper dbHelper, string pid)
        {
            return dbHelper.ExecuteNonQuery(@" UPDATE Configuration.dbo.tbl_BaoYangPriceUpdateAudit
                                                   SET AuditStatus = 3
                                                   WHERE AuditStatus = 0
                                                   		AND PID = @PID", CommandType.Text,
                                                      new SqlParameter[] {
                                                              new SqlParameter("@PID", pid)
                                                      });
        }

        /// <summary>
        /// 查询保养价格审核记录
        /// </summary>
        /// <param name="pID"></param>
        /// <param name="pager"></param>
        /// <returns></returns>
        public static IEnumerable<PriceUpdateAuditModel> SelectAuditLogByPID(string pID, PagerModel pager)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var dt = dbHelper.ExecuteDataTable(@"SELECT  EUTP.* ,
                                                             VP.DisplayName AS ProductName ,
                                                             VP.CP_Brand AS Brand,
                                                             Count(1) OVER() AS TotalCount
                                                    FROM     Configuration.dbo.tbl_BaoYangPriceUpdateAudit AS EUTP WITH ( NOLOCK )
                                                             JOIN Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK ) ON EUTP.PID = VP.PID
                                                    WHERE    ( EUTP.PID = @PID
                                                               OR @PID IS NULL
                                                             )
                                                             AND AuditStatus IN ( 1, 2 )
                                                    ORDER BY PKID DESC
                                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
                                                    FETCH NEXT @PageSize ROWS ONLY;", CommandType.Text,
                                                   new SqlParameter[] {
                                                       new SqlParameter("@PageIndex", pager.CurrentPage),
                                                       new SqlParameter("@PageSize", pager.PageSize),
                                                       new SqlParameter("@PID", pID)
                                                   });
                if (dt != null && dt.Rows.Count > 0)
                {
                    pager.TotalItem = Convert.ToInt32(dt.Rows[0]["TotalCount"] ?? 0);
                }
                return dt.ConvertTo<PriceUpdateAuditModel>();
            }
        }

        /// <summary>
        /// 审核保养品价格修改申请
        /// </summary>
        /// <param name="isAccess"></param>
        /// <param name="auther"></param>
        /// <param name="pid"></param>
        /// <param name="cost"></param>
        /// <param name="PurchasePrice"></param>
        /// <param name="totalstock"></param>
        /// <param name="num_week"></param>
        /// <param name="num_month"></param>
        /// <param name="guidePrice"></param>
        /// <param name="nowPrice"></param>
        /// <param name="maoliLv"></param>
        /// <param name="chaochu"></param>
        /// <param name="jdself"></param>
        /// <param name="maolie"></param>
        /// <returns></returns>
        public static int GotoAudit(bool isAccess, string auther, string pid, decimal? cost, decimal? PurchasePrice, int? totalstock, int? num_week, int? num_month, decimal? guidePrice, decimal nowPrice, string maoliLv, string chaochu, decimal? jdself, decimal? maolie)
        {
            double? maoliLv_1;
            if (maoliLv == null || !maoliLv.Contains("%"))
                maoliLv_1 = null;
            else
                maoliLv_1 = Convert.ToDouble(maoliLv.Replace("%", string.Empty)) / 100;
            double? chaochu_1;
            if (chaochu == null || !chaochu.Contains("%"))
                chaochu_1 = null;
            else
                chaochu_1 = Convert.ToDouble(chaochu.Replace("%", string.Empty)) / 100;

            int temp = isAccess ? 1 : 2;
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"UPDATE  Configuration.dbo.tbl_BaoYangPriceUpdateAudit
                                                  SET     AuditPerson = @AuditPerson ,
                                                  AuditDateTime = GETDATE() ,
                                                  Cost = @cost ,
                                                  PurchasePrice = @PurchasePrice ,
                                                  TotalStock = @totalstock ,
                                                  Num_Week = @num_week ,
                                                  Num_Month = @num_month ,
                                                  GuidePrice = @guidePrice ,
                                                  NowPrice = @nowPrice ,
                                                  MaoLiLv = @maoliLv ,
                                                  OverFlow = @overflow ,
                                                  JdSelf = @jdself ,
                                                  MaoLiE = @maolie ,
                                                  AuditStatus = @AuditStatus
                                                  WHERE   AuditStatus = 0
                                                  AND PID = @PID;", CommandType.Text,
                                                            new SqlParameter[] {
                                                                new SqlParameter("@AuditPerson", auther),
                                                                new SqlParameter("@AuditStatus", temp),
                                                                new SqlParameter("@PID", pid),
                                                                new SqlParameter("@cost", cost),
                                                                new SqlParameter("@PurchasePrice", PurchasePrice),
                                                                new SqlParameter("@totalstock", totalstock),
                                                                new SqlParameter("@num_week", num_week),
                                                                new SqlParameter("@num_month", num_month),
                                                                new SqlParameter("@guidePrice", guidePrice),
                                                                new SqlParameter("@nowPrice", nowPrice),
                                                                new SqlParameter("@maoliLv", maoliLv_1),
                                                                new SqlParameter("@overflow", chaochu_1),
                                                                new SqlParameter("@jdself", jdself),
                                                                new SqlParameter("@maolie", maolie),
                                                            });
            }
        }

        public static PriceUpdateAuditModel FetchPriceAudit(int pkid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  * FROM  Configuration.dbo.tbl_BaoYangPriceUpdateAudit WITH(NOLOCK) WHERE PKID=@PKID", CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@PKID",pkid)
                }).ConvertTo<PriceUpdateAuditModel>().FirstOrDefault();
            }
        }

        public static List<int> WorkShopIds()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var result = new List<int>();
                var sql = @"SELECT PKID FROM Gungnir..Shops (NOLOCK) WHERE ShopType&512=512";
                var data =  dbHelper.ExecuteDataTable(sql, CommandType.Text);
                if (data != null && data.Rows.Count > 0)
                {
                    foreach (DataRow dr in data.Rows)
                    {
                        result.Add(Convert.ToInt32(dr["PKID"]));
                    }
                }
                return result;
            }
        }

        public static IEnumerable<QiangGouProductModel> GetFlashSalePriceByPids(string pids)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"WITH    pids
          AS ( SELECT   *
               FROM   Gungnir..SplitString(@Pids, ',', 1)
             )
    SELECT  FS.ActivityID ,
            FSP.PID ,
            FSP.Price ,
            FS.ActivityName AS DisplayName
    FROM    Activity..tbl_FlashSale AS FS WITH ( NOLOCK )
            JOIN Activity..tbl_FlashSaleProducts AS FSP WITH ( NOLOCK ) ON FSP.ActivityID = FS.ActivityID
            JOIN pids ON pids.Item = FSP.PID COLLATE Chinese_PRC_CI_AS
    WHERE   FS.EndDateTime > GETDATE()
            AND ActiveType <> 2;", CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@PIDS",pids)
                }).ConvertTo<QiangGouProductModel>();
            }
        }
    }
}
