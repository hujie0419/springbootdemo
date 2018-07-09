using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalTirePrice
    {
        #region 指导参数
        static readonly Dictionary<string, string> dicGuidePara = new Dictionary<string, string>() {
            { "Pattern", "CP_Tire_Pattern" },
            { "Rim", "CP_Tire_Rim" },
            { "Rof", "CP_Tire_ROF" },
            { "Brand", "CP_Brand" },
            { "SalesQuantity", "" },
            { "Base", "" },
            {"TireSize",""}

        };

        public static IEnumerable<GuidePara> SelectGuideParaByType(string type)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {

                var sql = string.Empty;
                if (dicGuidePara.ContainsKey(type))
                {
                    if (!string.IsNullOrWhiteSpace(dicGuidePara[type]))
                        sql = @"SELECT	@Type AS Type,
	                        		T.Item,
	                        		TPGP.Value,
                                    TPGP.PKID
	                        FROM	( SELECT  DISTINCT
	                        					" + dicGuidePara[type] + @" AS Item
	                        		  FROM		Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
	                        		  WHERE		VP.PID LIKE 'TR-%'
	                        					AND ISNULL(" + dicGuidePara[type] + @", '') <> ''
	                        		) T
	                        LEFT JOIN Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara AS TPGP WITH ( NOLOCK )
	                        		ON T.Item = TPGP.Item
	                        		   AND TPGP.Type =@Type;";
                    else if (type == "Base")
                        sql = @"SELECT TPGP.Type,TPGP.Item,TPGP.Value,TPGP.PKID FROM Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara AS TPGP WITH(NOLOCK) WHERE TPGP.Type=@Type";
                    else if (type == "SalesQuantity")
                        sql = @"SELECT	'SalesQuantity' AS Type,
                                         		T.Item,
                                         		TPGP.Value,
                                         		TPGP.PKID
                                         FROM	( SELECT DISTINCT
                                         					ISNULL(DPSD.level, 1) AS Item
                                         		  FROM		Tuhu_bi.dbo.dm_Product_SalespredictData AS DPSD
                                         		  WHERE		pid LIKE N'TR-%'
                                         		) AS T
                                         LEFT JOIN Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara AS TPGP WITH ( NOLOCK )
                                         		ON T.Item = TPGP.Item
                                         		   AND TPGP.Type = 'SalesQuantity'
                                         ORDER BY T.Item ";
                    else if (type == "TireSize")
                    {
                        sql = @"SELECT  'TireSize' AS Type ,
                                        T.TireSize AS Item ,
                                        TPGP.Value ,
                                        TPGP.PKID
                                FROM    ( SELECT  DISTINCT
                                                    V.TireSize
                                          FROM      Tuhu_productcatalog.dbo.vw_Products AS V WITH ( NOLOCK )
                                          WHERE     PID LIKE 'TR-%'
                                                    AND ISNULL(V.TireSize, '') <> ''
                                        ) AS T
                                        LEFT JOIN Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara AS TPGP WITH ( NOLOCK ) ON T.TireSize = TPGP.Item
                                                                                              AND TPGP.Type = 'TireSize'
                                        LEFT JOIN Tuhu_bi.dbo.tbl_TireSizeSalesQuantity AS SQ WITH ( NOLOCK ) ON SQ.TireSize = T.TireSize
                                ORDER BY SQ.SalesQuantity DESC ;";
                    }
                }
                else
                    throw new Exception($"不存在{type}的加权项");

                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@Type",type)
                }).ConvertTo<GuidePara>();
            }

        }


        public static DataTable GetTireSizesByBrand(string brand)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT DISTINCT
			                                        		VP.TireSize
			                                        FROM	Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
                                                   WHERE	VP.PID LIKE 'TR-%'
                                                   		AND ISNULL(VP.TireSize, '') <> ''
                                                   		AND VP.CP_Brand=@Brand", CommandType.Text, new SqlParameter("@Brand", brand));
            }
        }

        public static int GotoExam(bool isAccess, string auther, string pid, decimal? cost, decimal? PurchasePrice, int? totalstock, int? num_week, int? num_month, decimal? guidePrice, decimal nowPrice, string maoliLv, string chaochu, decimal? jdself, decimal? maolie)
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
                return dbHelper.ExecuteNonQuery(@"UPDATE	Tuhu_productcatalog.dbo.tbl_ExamUpdateTirePrice
												   SET		ExamPerson = @ExamPerson,
															ExamDateTime = GETDATE(),
                                                            cost=@cost,
                                                            PurchasePrice=@PurchasePrice,
                                                            totalstock=@totalstock,
                                                            num_week=@num_week,
                                                            num_month=@num_month,
                                                            guidePrice=@guidePrice,
                                                            nowPrice=@nowPrice,
                                                            maoliLv=@maoliLv,
                                                            chaochu=@chaochu,
                                                            jdself=@jdself,
                                                            maolie=@maolie,
															ExamStatus = @ExamStatus WHERE ExamStatus=0 AND PID=@PID", CommandType.Text,
                                                            new SqlParameter[] {
                                                                new SqlParameter("@ExamPerson", auther),
                                                                new SqlParameter("@ExamStatus", temp),
                                                                new SqlParameter("@PID", pid),
                                                                new SqlParameter("@cost", cost),
                                                                new SqlParameter("@PurchasePrice", PurchasePrice),
                                                                new SqlParameter("@totalstock", totalstock),
                                                                new SqlParameter("@num_week", num_week),
                                                                new SqlParameter("@num_month", num_month),
                                                                new SqlParameter("@guidePrice", guidePrice),
                                                                new SqlParameter("@nowPrice", nowPrice),
                                                                new SqlParameter("@maoliLv", maoliLv_1),
                                                                new SqlParameter("@chaochu", chaochu_1),
                                                                new SqlParameter("@jdself", jdself),
                                                                new SqlParameter("@maolie", maolie),
                                                            });
            }
        }

        public static IEnumerable<QiangGouProductModel> GetFlashSalePriceByPID(string pid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  FS.ActivityID ,
                                                           FSP.PID,
                                                   		   FSP.Price,
                                                           FS.ActivityName AS DisplayName
                                                   FROM    Activity..tbl_FlashSale AS FS WITH ( NOLOCK )
                                                           JOIN Activity..tbl_FlashSaleProducts AS FSP WITH ( NOLOCK ) ON FSP.ActivityID = FS.ActivityID
                                                   WHERE   FS.EndDateTime > GETDATE()
                                                           AND FSP.PID = @PID
                                                           AND ActiveType <> 2;", CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@PID",pid)
                }).ConvertTo<QiangGouProductModel>();
            }
        }

        public static ExamUpdatePriceModel FetchPriceExam(int pkid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  * FROM  Tuhu_productcatalog.dbo.tbl_ExamUpdateTirePrice WITH(NOLOCK) WHERE PKID=@PKID", CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@PKID",pkid)
                }).ConvertTo<ExamUpdatePriceModel>().FirstOrDefault();
            }
        }

        public static int UpdateWarningLine(WarningLineModel model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"UPDATE    Configuration..tbl_Tire_WarningLine  SET LowerLimit=@LowerLimit,UpperLimit=@UpperLimit WHERE PKID=@PKID", CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@LowerLimit",model.LowerLimit),
                    new SqlParameter("@UpperLimit",model.UpperLimit),
                    new SqlParameter("@PKID",model.PKID)
                });
            }
        }

        public static IEnumerable<WarningLineModel> SelectWarningLine()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  *
                                                   FROM    Configuration..tbl_Tire_WarningLine WITH ( NOLOCK ) ORDER BY MinGuidePrice;").ConvertTo<WarningLineModel>();
            }
        }

        public static IEnumerable<VehicleModel> LookYuanPeiByPID(string pid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"   SELECT DISTINCT
                                                          V.ProductID ,
                                                          V.Brand ,
                                                          V.Vehicle
                                                 FROM     Gungnir.dbo.tbl_Vehicle_Type AS V WITH ( NOLOCK )
                                                          CROSS APPLY Gungnir..SplitString(V.TiresMatch, ';', 1) AS SS
                                                 WHERE    SS.Item = @pid;", CommandType.Text, new SqlParameter[] { new SqlParameter("@pid", pid) }).ConvertTo<VehicleModel>();
            }
        }

        public static IEnumerable<PriceChangeLog> PriceChangeLog(string pid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT	*
                                                  FROM	Tuhu_productcatalog.dbo.tbl_PriceHistory AS PH WITH ( NOLOCK )
                                                  WHERE	PID = @PID
                                                  ORDER BY PKID DESC", CommandType.Text, new SqlParameter[] { new SqlParameter("@PID", pid) }).ConvertTo<PriceChangeLog>();
            }
        }

        public static IEnumerable<ZXTCost> GetZXTPurchaseByPID(string pid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  PID ,
                                                           POI.InstockDate AS CreatedDatetime ,
                                                           WareHouse ,
                                                           CostPrice ,
                                                           InstockNum AS Num
                                                   FROM    [Gungnir].dbo.PurchaseOrderItem AS POI WITH ( NOLOCK )
                                                   WHERE   Status IN ( N'已收货' )
                                                           AND POI.InstockDate > DATEADD(YEAR, -1, GETDATE())
                                                           AND PurchaseMode NOT IN ( 5, 6, 11, 12 )
                                                           AND POI.InstockNum > 0
                                                           AND PID = @pid;", CommandType.Text, new SqlParameter("@PID", pid)).ConvertTo<ZXTCost>();
            }
        }

        public static IEnumerable<ExamUpdatePriceModel> SelectExamLogByPID(string pID, PagerModel pager)
        {
            pager.TotalItem = SelectExamLogByPIDCount(pID);
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT   EUTP.* ,
                                                             VP.DisplayName AS ProductName ,
                                                             VP.CP_Brand AS Brand
                                                    FROM     Tuhu_productcatalog.dbo.tbl_ExamUpdateTirePrice AS EUTP WITH ( NOLOCK )
                                                             JOIN Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK ) ON EUTP.PID = VP.PID
                                                    WHERE    ( EUTP.PID = @PID
                                                               OR @PID IS NULL
                                                             )
                                                             AND ExamStatus IN ( 1, 2 )
                                                    ORDER BY PKID DESC
                                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
                                                    FETCH NEXT @PageSize ROWS ONLY;", CommandType.Text,
                                                   new SqlParameter[] {
                                                       new SqlParameter("@PageIndex", pager.CurrentPage),
                                                       new SqlParameter("@PageSize", pager.PageSize),
                                                       new SqlParameter("@PID", pID)
                                                   }).ConvertTo<ExamUpdatePriceModel>();
            }
        }
        public static int SelectExamLogByPIDCount(string pID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var OBJ = dbHelper.ExecuteScalar(@"SELECT    COUNT(1)
                                                  FROM      Tuhu_productcatalog.dbo.tbl_ExamUpdateTirePrice AS EUTP WITH ( NOLOCK )
                                                  WHERE     ( EUTP.PID = @PID
                                                              OR @PID IS NULL
                                                            )
                                                            AND ExamStatus IN ( 1, 2 );", CommandType.Text, new SqlParameter("@PID", pID));
                return Convert.ToInt32(OBJ);
            }
        }

        public static IEnumerable<ExamUpdatePriceModel> SelectNeedExamTire()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  EUTP.PKID,EUTP.PID,EUTP.Price,EUTP.Reason,EUTP.Remark,EUTP.ApplyDateTime,EUTP.ApplyPerson,EUTP.ExamDateTime,EUTP.ExamPerson,EUTP.ExamStatus,
        VP.CP_Brand AS Brand ,
        VP.DisplayName AS ProductName ,
        DPSD.cost,
        DPSD.jingdongself_price AS JDSelfPrice ,
        DPSD.PurchasePrice ,
        DPSD.totalstock  ,
        DPSD.num_week ,
        DPSD.num_month,
        ( ISNULL(( SELECT TOP 1
                            TPGP.Value
                   FROM     Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara AS TPGP
                            WITH ( NOLOCK )
                   WHERE    TPGP.Type = 'Base'
                 ), 0)
          + ISNULL(( SELECT TOP 1
                            TPGP.Value
                     FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara AS TPGP
                            WITH ( NOLOCK )
                     WHERE  TPGP.Type = 'Rim'
                            AND TPGP.Item = VP.CP_Tire_Rim
                   ), 0)
          + ISNULL(( SELECT TOP 1
                            TPGP.Value
                     FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara AS TPGP
                            WITH ( NOLOCK )
                     WHERE  TPGP.Type = 'Rof'
                            AND TPGP.Item = VP.CP_Tire_ROF
                   ), 0)
          + ISNULL(( SELECT TOP 1
                            TPGP.Value
                     FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara AS TPGP
                            WITH ( NOLOCK )
                     WHERE  TPGP.Type = 'SalesQuantity'
                            AND TPGP.Item = DPSD.level
                   ), 0)
          + ISNULL(( SELECT TOP 1
                            TPGP.Value
                     FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara AS TPGP
                            WITH ( NOLOCK )
                     WHERE  TPGP.Type = 'Pattern'
                            AND TPGP.Item = VP.CP_Tire_Pattern
                   ), 0)
          + ISNULL(( SELECT TOP 1
                            TPGP.Value
                     FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara AS TPGP
                            WITH ( NOLOCK )
                     WHERE  TPGP.Type = 'Brand'
                            AND TPGP.Item = VP.CP_Brand
                   ), 0)
          + ISNULL(( SELECT TOP 1
                            TPGP.Value
                     FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara AS TPGP
                            WITH ( NOLOCK )
                     WHERE  TPGP.Type = 'TireSize'
                            AND TPGP.Item = VP.TireSize
                   ), 0) ) AS JiaQuan ,
        VP.cy_list_price AS ListPrice
FROM    Tuhu_productcatalog.dbo.tbl_ExamUpdateTirePrice AS EUTP WITH ( NOLOCK )
        INNER JOIN Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK ) ON EUTP.PID = VP.PID
        LEFT   JOIN Tuhu_bi.dbo.dm_Product_SalespredictData AS DPSD WITH ( NOLOCK ) ON EUTP.PID = DPSD.pid
WHERE   EUTP.ExamStatus = 0; ").ConvertTo<ExamUpdatePriceModel>();
            }
        }

        public static int ApplyUpdatePrice(ExamUpdatePriceModel model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                ApplyUpdatePrice(dbHelper, model.PID);
                return dbHelper.ExecuteNonQuery(@" INSERT	INTO Tuhu_productcatalog.dbo.tbl_ExamUpdateTirePrice
                                                   		(
                                                   		  PID,
                                                   		  Price,
                                                   		  Reason,
                                                          Remark,
                                                   		  ApplyPerson,
                                                   		  ApplyDateTime,
                                                   		  ExamStatus )
                                                   VALUES	(
                                                   		  @PID,
                                                   		  @Price,
                                                   		  @Reason,
                                                          @Remark,
                                                   		  @ApplyPerson,
                                                   		  GETDATE(),
                                                   		  0 )", CommandType.Text,
                                                          new SqlParameter[] {
                                                              new SqlParameter("@PID", model.PID),
                                                              new SqlParameter("@Price", model.Price),
                                                               new SqlParameter("@Reason", model.Reason),
                                                                new SqlParameter("@ApplyPerson", model.ApplyPerson),
                                                                new SqlParameter("@Remark",model.Remark)
                                                          });
            }
        }

        public static int ApplyUpdatePrice(SqlDbHelper dbHelper, string pid)
        {
            return dbHelper.ExecuteNonQuery(@" UPDATE	Tuhu_productcatalog.dbo.tbl_ExamUpdateTirePrice
                                                   SET		ExamStatus = 3
                                                   WHERE	ExamStatus = 0
                                                   		AND PID = @PID", CommandType.Text,
                                                      new SqlParameter[] {
                                                              new SqlParameter("@PID", pid)
                                                      });
        }



        public static IEnumerable<Product_Warehouse> GetStock(string pid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                var data = dbHelper.ExecuteDataTable(@" select T.*
from Tuhu_bi.dbo.dm_Product_Warehouse_SalespredictData as T with (nolock)
where pid = @pid
      and warehousename is not null
order by case
             when T.warehousename like N'途虎养车工场店%' then
                 2
             else
                 1
         end,
         T.num_threemonth desc;", CommandType.Text, new SqlParameter("@pid", pid)).ConvertTo<Product_Warehouse>();
                return data;
            }
        }

        public static int DeleteGuidePara(int PKID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"DELETE Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara WHERE PKID=@PKID", CommandType.Text, new SqlParameter("@PKID", PKID));
            }
        }

        public static int UpdateGuidePara(GuidePara model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"UPDATE Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara SET Value=@Value,UpdateTime=GETDATE() WHERE PKID=@PKID", CommandType.Text, new SqlParameter[] { new SqlParameter("@Value", model.Value), new SqlParameter("@PKID", model.PKID) });
            }
        }

        public static int InsertGuidePara(GuidePara model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"INSERT	INTO Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara
                                                  		( Type, Item, Value )
                                                  VALUES	( @Type, @Item, @Value )", CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@Value", model.Value),
                    new SqlParameter("@Type", model.Type),
                    new SqlParameter("@Item", model.Item) });
            }
        }
        #endregion

        #region 轮胎价格查询
        public static IEnumerable<TireListModel> SelectPriceProductList2(PriceSelectModel model, PagerModel pager, bool isExport = false)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                pager.TotalItem = isExport ? 0 : GetTireListCount2(dbHelper, model);
                #region SQL
                var sql = @"SELECT  *
FROM    ( SELECT    T.*, ( CASE WHEN T.cost IS NULL THEN NULL
                                ELSE T.cost + ( T.cost * T.JiaQuan ) / 100
                           END ) AS TheoryGuidePrice,
                    ( CASE WHEN T.cost IS NULL
                                AND T.JDSelfPrice IS NULL THEN NULL
                           WHEN T.cost IS NULL
                                AND T.JDSelfPrice > 0 THEN T.JDSelfPrice
                           WHEN T.cost > 0
                                AND T.JDSelfPrice IS NULL
                           THEN T.cost + ( T.cost * T.JiaQuan ) / 100
                           WHEN T.cost + ( T.cost * T.JiaQuan ) / 100 >= T.JDSelfPrice
                           THEN T.JDSelfPrice
                           ELSE T.cost + ( T.cost * T.JiaQuan ) / 100
                      END ) AS ActualGuidePrice,
                    ( CASE WHEN @SingleCustomPrice = 'list' THEN T.Price
                           WHEN @SingleCustomPrice = 'taobao1' THEN T.TBPrice
                           WHEN @SingleCustomPrice = 'taobao2' THEN T.TB2Price
                           WHEN @SingleCustomPrice = 'tmall1' THEN T.TM1Price
                           WHEN @SingleCustomPrice = 'tmall2' THEN T.TM2Price
                           WHEN @SingleCustomPrice = 'tmall3' THEN T.TM3Price
                           WHEN @SingleCustomPrice = 'tmall4' THEN T.TM4Price
                           WHEN @SingleCustomPrice = 'tuhujd' THEN T.JDPrice
                           WHEN @SingleCustomPrice = 'jingdongqj'
                           THEN T.JDFlagShipPrice
                           WHEN @SingleCustomPrice = 'qipeilong'
                           THEN T.QPLPrice
                           WHEN @SingleCustomPrice = 'tmalltwl'
                           THEN T.TWLTMPrice
                           WHEN @SingleCustomPrice = 'jingdongself'
                           THEN T.JDSelfPrice
                           WHEN @SingleCustomPrice = 'qccrl' THEN T.MLTTMPrice
                           WHEN @SingleCustomPrice = 'qccrp' THEN T.MLTPrice
                           WHEN @SingleCustomPrice = 'cost' THEN T.cost
                           WHEN @SingleCustomPrice = 'coupon'
                           THEN T.CouponPrice
                           ELSE T.Price
                      END ) AS SingleCustomPrice
          FROM      ( SELECT    VP.CP_Brand AS Brand, VP.PID,
                                VP.DisplayName AS ProductName,
                                ( ISNULL(( SELECT TOP 1
                                                    TPGP.Value
                                           FROM     Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara
                                                    AS TPGP WITH ( NOLOCK )
                                           WHERE    TPGP.Type = 'Base'
                                         ), 0)
                                  + ISNULL(( SELECT TOP 1
                                                    TPGP.Value
                                             FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara
                                                    AS TPGP WITH ( NOLOCK )
                                             WHERE  TPGP.Type = 'Rim'
                                                    AND TPGP.Item = VP.CP_Tire_Rim
                                           ), 0)
                                  + ISNULL(( SELECT TOP 1
                                                    TPGP.Value
                                             FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara
                                                    AS TPGP WITH ( NOLOCK )
                                             WHERE  TPGP.Type = 'Rof'
                                                    AND TPGP.Item = VP.CP_Tire_ROF
                                           ), 0)
                                  + ISNULL(( SELECT TOP 1
                                                    TPGP.Value
                                             FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara
                                                    AS TPGP WITH ( NOLOCK )
                                             WHERE  TPGP.Type = 'SalesQuantity'
                                                    AND TPGP.Item = DPSD.level
                                           ), 0)
                                  + ISNULL(( SELECT TOP 1
                                                    TPGP.Value
                                             FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara
                                                    AS TPGP WITH ( NOLOCK )
                                             WHERE  TPGP.Type = 'Pattern'
                                                    AND TPGP.Item = VP.CP_Tire_Pattern
                                           ), 0)
                                  + ISNULL(( SELECT TOP 1
                                                    TPGP.Value
                                             FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara
                                                    AS TPGP WITH ( NOLOCK )
                                             WHERE  TPGP.Type = 'Brand'
                                                    AND TPGP.Item = VP.CP_Brand
                                           ), 0)
                                  + +ISNULL(( SELECT TOP 1
                                                        TPGP.Value
                                              FROM      Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara
                                                        AS TPGP WITH ( NOLOCK )
                                              WHERE     TPGP.Type = 'TireSize'
                                                        AND TPGP.Item = VP.TireSize
                                            ), 0) ) AS JiaQuan,
                                DPSD.totalstock, TMVC.VehicleCount,
                                DPSD.SelfStock, DPSD.num_week, DPSD.num_month,
                                DPSD.num_threemonth, DPSD.cost,
                                DPSD.PurchasePrice, VP.cy_list_price AS Price,
                                DPSD.taobao_tuhuid AS TBPID,
                                DPSD.taobao_tuhuprice AS TBPrice,
                                DPSD.taobao2_tuhuid AS TB2PID,
                                DPSD.taobao2_tuhuprice AS TB2Price,
                                DPSD.tianmao1_tuhuprice AS TM1Price,
                                DPSD.tianmao1_tuhuid AS TM1PID,
                                DPSD.tianmao2_tuhuprice AS TM2Price,
                                DPSD.tianmao2_tuhuid AS TM2PID,
                                DPSD.tianmao3_tuhuprice AS TM3Price,
                                DPSD.tianmao3_tuhuid AS TM3PID,
                                DPSD.tianmao4_tuhuid AS TM4PID,
                                DPSD.tianmao4_tuhuprice AS TM4Price,
                                DPSD.jingdong_tuhuprice AS JDPrice,
                                DPSD.jingdong_tuhuid AS JDPID,
                                DPSD.jingdongflagship_tuhuprice AS JDFlagShipPrice,
                                DPSD.jingdongflagship_tuhuid AS JDFlagShipPID,
                                DPSD.jingdongself_price AS JDSelfPrice,
                                DPSD.jingdongself_id AS JDSelfPID,
                                DPSD.teweilun_tianmaoprice AS TWLTMPrice,
                                DPSD.teweilun_tianmaoid AS TWLTMPID,
                                DPSD.qccr_retailprice AS MLTTMPrice,
                                DPSD.qccr_retailid AS MLTTMPID,
                                DPSD.qccr_wholesaleprice AS MLTPrice,
                                DPSD.qccr_wholesaleid AS MLTPID,
                                QPL.Price AS QPLPrice, JDP.Price2 AS JDPlus,
                                CT.NewPrice AS CouponPrice, DPSD.CaigouZaitu
                      FROM      Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
                                LEFT JOIN Tuhu_bi.dbo.dm_Product_SalespredictData
                                AS DPSD WITH ( NOLOCK ) ON VP.PID = DPSD.PID
                                LEFT JOIN Tuhu_productcatalog..ProductPriceMapping
                                AS QPL WITH ( NOLOCK ) ON ShopCode = N'汽配龙'
                                                          AND VP.PID = QPL.Pid
                                LEFT JOIN Tuhu_productcatalog..ProductPriceMapping
                                AS JDP WITH ( NOLOCK ) ON JDP.ShopCode = N'京东自营'
                                                          AND VP.PID = JDP.Pid
                                LEFT JOIN [Configuration].dbo.tbl_TireMatchVehicleCount
                                AS TMVC WITH ( NOLOCK ) ON VP.PID = TMVC.PID
                                LEFT JOIN Tuhu_productcatalog..tbl_CouponPrice
                                AS CT WITH ( NOLOCK ) ON CT.PID = VP.PID
                      WHERE     ( ( VP.PID LIKE '%' + @PID + '%'
                                    OR @PID IS NULL
                                  )
                                  AND VP.PID LIKE 'TR-%'
                                )
                                AND ( VP.DisplayName LIKE '%' + @ProductName
                                      + '%'
                                      OR @ProductName IS NULL
                                    )
                                AND ( @Brands IS NULL
                                      OR VP.CP_Brand COLLATE Chinese_PRC_CI_AS IN (
                                      SELECT    *
                                      FROM      Gungnir.dbo.Split(@Brands, ';') )
                                    )
                                AND ( @Patterns IS NULL
                                      OR VP.CP_Tire_Pattern COLLATE Chinese_PRC_CI_AS IN (
                                      SELECT    *
                                      FROM      Gungnir.dbo.Split(@Patterns,
                                                              ';') )
                                    )
                                AND ( @TireSizes IS NULL
                                      OR VP.TireSize COLLATE Chinese_PRC_CI_AS = @TireSizes
                                    )
                                AND ( @Rims IS NULL
                                      OR VP.CP_Tire_Rim COLLATE Chinese_PRC_CI_AS IN (
                                      SELECT    *
                                      FROM      Gungnir.dbo.Split(@Rims, ';') )
                                    )
                                AND ( @StockStatus = 0
                                      OR @StockStatus = 1
                                      AND VP.stockout = 1
                                      OR @StockStatus = 2
                                      AND DPSD.totalstock > DPSD.SelfStock
                                    )
                                AND ( @OnSaleStatus = 0
                                      OR @OnSaleStatus = 1
                                      AND VP.OnSale = 1
                                      OR @OnSaleStatus = 2
                                      AND VP.OnSale = 0
                                    )
                                AND ( @MaoLiE IS NULL
                                      OR ( @MaoLiE IS NOT NULL
                                           AND VP.cy_list_price > 0
                                           AND DPSD.cost > 0
                                           AND ( @MatchMaoLiE = 1
                                                 AND ISNULL(VP.cy_list_price,
                                                            0)
                                                 - ISNULL(DPSD.cost, 0) >= @MaoLiE
                                                 OR @MatchMaoLiE = -1
                                                 AND ISNULL(VP.cy_list_price,
                                                            0)
                                                 - ISNULL(DPSD.cost, 0) <= @MaoLiE
                                               )
                                         )
                                    )
                                AND ( @MatchStock = 0
                                      OR ( @MatchStock = -1
                                           AND DPSD.totalstock <= @MatchStockValue
                                           OR @MatchStock = 1
                                           AND DPSD.totalstock >= @MatchStockValue
                                         )
                                    )
                                AND ( @MatchZZTS = 0
                                      OR ( DPSD.num_month > 0
                                           AND ( @MatchZZTS = -1
                                                 AND DPSD.totalstock
                                                 / ( ( IIF(ISNULL(DPSD.num_month,
                                                              0) = 0, 1, DPSD.num_month))
                                                     / 30.0 ) <= @MatchZZTSValue
                                                 OR @MatchZZTS = 1
                                                 AND DPSD.totalstock
                                                 / ( ( IIF(ISNULL(DPSD.num_month,
                                                              0) = 0, 1, DPSD.num_month))
                                                     / 30.0 ) >= @MatchZZTSValue
                                               )
                                         )
                                    )
                    ) AS T
        ) AS U
        LEFT  JOIN Configuration.dbo.tbl_Tire_WarningLine AS TWL WITH ( NOLOCK ) ON ISNULL(U.ActualGuidePrice,
                                                              0) >= TWL.MinGuidePrice
                                                              AND ISNULL(U.ActualGuidePrice,
                                                              0) < TWL.MaxGuidePrice
WHERE   ( @PCPricePer IS NULL
          OR ( @PCPricePer IS NOT NULL
               AND U.Price > 0
               AND ( @MatchPCPricePer = 1
                     AND ( U.Price - U.ActualGuidePrice )
                     / IIF(U.ActualGuidePrice <= 0, 1, U.ActualGuidePrice) >= @PCPricePer
                     * 1.0 / 100
                     OR @MatchPCPricePer = -1
                     AND ( U.Price - U.ActualGuidePrice )
                     / IIF(U.ActualGuidePrice <= 0, 1, U.ActualGuidePrice) <= @PCPricePer
                     * 1.0 / 100
                   )
             )
        )
        AND ( @MonthCount = 0
              OR ( @MonthCount = 1
                   AND U.num_month >= @MonthCountValue
                   OR @MonthCount = -1
                   AND U.num_month <= @MonthCountValue
                 )
            )
        AND ( @WeekCount = 0
              OR ( @WeekCount = 1
                   AND U.num_week >= @WeekCountValue
                   OR @WeekCount = -1
                   AND U.num_week <= @WeekCountValue
                 )
            )
        AND ( @VehicleCount = 0
              OR ( @VehicleCount = 1
                   AND ( @VehicleCountValue > 0
                         AND U.VehicleCount >= @VehicleCountValue
                         OR @VehicleCountValue <= 0
                       )
                   OR @VehicleCount = -1
                   AND ( U.VehicleCount <= @VehicleCountValue
                         OR U.VehicleCount IS NULL
                       )
                   OR @VehicleCount = 2
                   AND @VehicleCountValue > 0
                   AND U.VehicleCount = @VehicleCountValue
                   OR @VehicleCount = 2
                   AND @VehicleCountValue = 0
                   AND ( U.VehicleCount = 0
                         OR U.VehicleCount IS NULL
                       )
                 )
            )
        AND ( @MatchWarnLine = -99
              OR ( @MatchWarnLine > -99
                   AND U.Price > 0
                   AND ( @MatchWarnLine = 1
                         AND U.ActualGuidePrice + TWL.UpperLimit < U.Price
                         OR @MatchWarnLine = -1
                         AND U.ActualGuidePrice - TWL.LowerLimit > U.Price
                       )
                 )
            )
        AND ( @Type = 0
              OR ( @Contrast = -1
                   AND U.Price > 0
                   AND ( @Type = 1
                         AND U.cost > 0
                         AND ( U.Price / IIF(ISNULL(U.cost, 0) = 0, 1, U.cost) <= @Proportion
                               OR U.QPLPrice
                               / IIF(ISNULL(U.cost, 0) = 0, 1, U.cost) <= @Proportion
                               AND U.QPLPrice > 0
                             )
                         OR @Type = 2
                         AND ( U.TBPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.TBPrice > 0
                               OR U.TB2Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.TB2Price > 0
                               OR U.TM1Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.TM1Price > 0
                               OR U.TM2Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.TM2Price > 0
                               OR U.TM3Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.TM3Price > 0
                               OR U.TM4Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.TM4Price > 0
                               OR U.JDPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.JDPrice > 0
                               OR U.JDFlagShipPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.JDFlagShipPrice > 0
                             )
                         OR @Type = 3
                         AND ( U.JDSelfPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.JDSelfPrice > 0
                               OR U.TWLTMPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.TWLTMPrice > 0
                               OR U.MLTTMPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.MLTTMPrice > 0
                             )
                         OR @Type = 4
                         AND ( ( U.Price
                                 / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                 AND U.Price > 0
                                 AND CHARINDEX('list', @ManyCustomPrice) > 0
                               )
                               OR ( U.TBPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.TBPrice > 0
                                    AND CHARINDEX('taobao1', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TB2Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.TB2Price > 0
                                    AND CHARINDEX('taobao2', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM1Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.TM1Price > 0
                                    AND CHARINDEX('tmall1', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM2Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.TM2Price > 0
                                    AND CHARINDEX('tmall2', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM3Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.TM3Price > 0
                                    AND CHARINDEX('tmall3', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM4Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.TM4Price > 0
                                    AND CHARINDEX('tmall4', @ManyCustomPrice) > 0
                                  )
                               OR ( U.JDPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.JDPrice > 0
                                    AND CHARINDEX('tuhujd', @ManyCustomPrice) > 0
                                  )
                               OR ( U.JDFlagShipPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.JDFlagShipPrice > 0
                                    AND CHARINDEX('jingdongqj',
                                                  @ManyCustomPrice) > 0
                                  )
                               OR ( U.QPLPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.QPLPrice > 0
                                    AND CHARINDEX('qipeilong',
                                                  @ManyCustomPrice) > 0
                                  )
                               OR ( U.TWLTMPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.TWLTMPrice > 0
                                    AND CHARINDEX('tmalltwl', @ManyCustomPrice) > 0
                                  )
                               OR ( U.JDSelfPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.JDSelfPrice > 0
                                    AND CHARINDEX('jingdongself',
                                                  @ManyCustomPrice) > 0
                                  )
                               OR ( U.MLTTMPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.MLTTMPrice > 0
                                    AND CHARINDEX('qccrl', @ManyCustomPrice) > 0
                                  )
                               OR ( U.MLTPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.MLTPrice > 0
                                    AND CHARINDEX('qccrp', @ManyCustomPrice) > 0
                                  )
                               OR ( U.cost
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.cost > 0
                                    AND CHARINDEX('cost', @ManyCustomPrice) > 0
                                  )
                               OR ( U.CouponPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.CouponPrice > 0
                                    AND CHARINDEX('coupon', @ManyCustomPrice) > 0
                                  )
                             )
                       )
                   OR @Contrast = 1
                   AND U.Price > 0
                   AND ( @Type = 1
                         AND U.cost > 0
                         AND ( U.Price / IIF(ISNULL(U.cost, 0) = 0, 1, U.cost) >= @Proportion
                               OR U.QPLPrice
                               / IIF(ISNULL(U.cost, 0) = 0, 1, U.cost) >= @Proportion
                               AND U.QPLPrice > 0
                             )
                         OR @Type = 2
                         AND ( U.TBPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.TBPrice > 0
                               OR U.TB2Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.TB2Price > 0
                               OR U.TM1Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.TM1Price > 0
                               OR U.TM2Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.TM2Price > 0
                               OR U.TM3Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.TM3Price > 0
                               OR U.TM4Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.TM4Price > 0
                               OR U.JDPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.JDPrice > 0
                               OR U.JDFlagShipPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.JDFlagShipPrice > 0
                             )
                         OR @Type = 3
                         AND ( U.JDSelfPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.JDSelfPrice > 0
                               OR U.TWLTMPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.TWLTMPrice > 0
                               OR U.MLTTMPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.MLTTMPrice > 0
                             )
                         OR @Type = 4
                         AND ( ( U.Price
                                 / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                 AND U.Price > 0
                                 AND CHARINDEX('list', @ManyCustomPrice) > 0
                               )
                               OR ( U.TBPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.TBPrice > 0
                                    AND CHARINDEX('taobao1', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TB2Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.TB2Price > 0
                                    AND CHARINDEX('taobao2', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM1Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.TM1Price > 0
                                    AND CHARINDEX('tmall1', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM2Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.TM2Price > 0
                                    AND CHARINDEX('tmall2', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM3Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.TM3Price > 0
                                    AND CHARINDEX('tmall3', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM4Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.TM4Price > 0
                                    AND CHARINDEX('tmall4', @ManyCustomPrice) > 0
                                  )
                               OR ( U.JDPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.JDPrice > 0
                                    AND CHARINDEX('tuhujd', @ManyCustomPrice) > 0
                                  )
                               OR ( U.JDFlagShipPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.JDFlagShipPrice > 0
                                    AND CHARINDEX('jingdongqj',
                                                  @ManyCustomPrice) > 0
                                  )
                               OR ( U.QPLPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.QPLPrice > 0
                                    AND CHARINDEX('qipeilong',
                                                  @ManyCustomPrice) > 0
                                  )
                               OR ( U.TWLTMPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.TWLTMPrice > 0
                                    AND CHARINDEX('tmalltwl', @ManyCustomPrice) > 0
                                  )
                               OR ( U.JDSelfPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.JDSelfPrice > 0
                                    AND CHARINDEX('jingdongself',
                                                  @ManyCustomPrice) > 0
                                  )
                               OR ( U.MLTTMPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.MLTTMPrice > 0
                                    AND CHARINDEX('qccrl', @ManyCustomPrice) > 0
                                  )
                               OR ( U.MLTPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.MLTPrice > 0
                                    AND CHARINDEX('qccrp', @ManyCustomPrice) > 0
                                  )
                               OR ( U.cost
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.cost > 0
                                    AND CHARINDEX('cost', @ManyCustomPrice) > 0
                                  )
                               OR ( U.CouponPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.CouponPrice > 0
                                    AND CHARINDEX('coupon', @ManyCustomPrice) > 0
                                  )
                             )
                       )
                 )
            )
ORDER BY U.num_threemonth DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
FETCH NEXT @PageSize ROWS ONLY;";
                //var brandList = model.Brand?.Split(';').Where(g => !string.IsNullOrWhiteSpace(g))?.ToList() ?? new List<string>();
                //var brandSelect = brandList.Any() ? @"AND VP.CP_Brand IN (N'" + string.Join("',N'", brandList) + @"')" : "";
                //sql = string.Format(sql, brandSelect);
                #endregion
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandTimeout = 3 * 60;
                    cmd.Parameters.AddRange(new SqlParameter[] {
                        new SqlParameter("@PID",string.IsNullOrWhiteSpace(model.PID)?null:model.PID),
                        new SqlParameter("@ProductName",string.IsNullOrWhiteSpace(model.ProductName)?null:model.ProductName),
                        new SqlParameter("@Brands",string.IsNullOrWhiteSpace(model.Brand)?null:model.Brand),
                        new SqlParameter("@StockStatus",model.StockStatus),
                        new SqlParameter("@Type",model.Type),
                        new SqlParameter("@Contrast",model.Contrast),
                        new SqlParameter("@Proportion",model.Proportion),
                        new SqlParameter("@PageIndex",pager.CurrentPage),
                        new SqlParameter("@PageSize",pager.PageSize),
                        new SqlParameter("@MaoLiE",model.MaoLiE),
                        new SqlParameter("@MatchMaoLiE",model.MatchMaoLiE),
                        new SqlParameter("@MatchStock",model.MatchStock),
                        new SqlParameter("@MatchStockValue",model.MatchStockValue),
                        new SqlParameter("@MatchZZTS",model.MatchZZTS),
                        new SqlParameter("@MatchZZTSValue",model.MatchZZTSValue),
                        new SqlParameter("@PCPricePer",model.PCPricePer),
                        new SqlParameter("@MatchPCPricePer",model.MatchPCPricePer),
                        new SqlParameter("@OnSaleStatus",model.OnSaleStatus),
                        new SqlParameter("@ManyCustomPrice",model.ManyCustomPrice),
                        new SqlParameter("@SingleCustomPrice",model.SingleCustomPrice),
                        new SqlParameter("@MatchWarnLine",model.MatchWarnLine),
                        new SqlParameter("@MonthCount",model.MonthCount),
                        new SqlParameter("@MonthCountValue",model.MonthCountValue),
                        new SqlParameter("@WeekCount",model.WeekCount),
                        new SqlParameter("@WeekCountValue",model.WeekCountValue),
                        new SqlParameter("@VehicleCount",model.VehicleCount),
                        new SqlParameter("@VehicleCountvalue",model.VehicleCountValue),
                        new SqlParameter("@Patterns",model.Patterns),
                        new SqlParameter("@Rims",model.Rims),
                        new SqlParameter("@TireSizes",model.TireSize)
                    });
                    return dbHelper.ExecuteDataTable(cmd).ConvertTo<TireListModel>();
                }

            }
        }

        public static int GetTireListCount2(SqlDbHelper dbHelper, PriceSelectModel model)
        {
            #region sql
            var sql = @"SELECT  COUNT(1)
FROM    ( SELECT    T.* ,
                    ( CASE WHEN T.cost IS NULL THEN NULL
                           ELSE T.cost + ( T.cost * T.JiaQuan ) / 100
                      END ) AS TheoryGuidePrice ,
                    ( CASE WHEN T.cost IS NULL
                                AND T.JDSelfPrice IS NULL THEN NULL
                           WHEN T.cost IS NULL
                                AND T.JDSelfPrice > 0 THEN T.JDSelfPrice
                           WHEN T.cost > 0
                                AND T.JDSelfPrice IS NULL
                           THEN T.cost + ( T.cost * T.JiaQuan ) / 100
                           WHEN T.cost + ( T.cost * T.JiaQuan ) / 100 >= T.JDSelfPrice
                           THEN T.JDSelfPrice
                           ELSE T.cost + ( T.cost * T.JiaQuan ) / 100
                      END ) AS ActualGuidePrice ,
                    ( CASE WHEN @SingleCustomPrice = 'list' THEN T.Price
                           WHEN @SingleCustomPrice = 'taobao1' THEN T.TBPrice
                           WHEN @SingleCustomPrice = 'taobao2' THEN T.TB2Price
                           WHEN @SingleCustomPrice = 'tmall1' THEN T.TM1Price
                           WHEN @SingleCustomPrice = 'tmall2' THEN T.TM2Price
                           WHEN @SingleCustomPrice = 'tmall3' THEN T.TM3Price
                           WHEN @SingleCustomPrice = 'tmall4' THEN T.TM4Price
                           WHEN @SingleCustomPrice = 'tuhujd' THEN T.JDPrice
                           WHEN @SingleCustomPrice = 'jingdongqj'
                           THEN T.JDFlagShipPrice
                           WHEN @SingleCustomPrice = 'qipeilong'
                           THEN T.QPLPrice
                           WHEN @SingleCustomPrice = 'tmalltwl'
                           THEN T.TWLTMPrice
                           WHEN @SingleCustomPrice = 'jingdongself'
                           THEN T.JDSelfPrice
                           WHEN @SingleCustomPrice = 'qccrl' THEN T.MLTTMPrice
                           WHEN @SingleCustomPrice = 'qccrp' THEN T.MLTPrice
                           WHEN @SingleCustomPrice = 'cost' THEN T.cost
                           WHEN @SingleCustomPrice = 'coupon'
                           THEN T.CouponPrice
                           ELSE T.Price
                      END ) AS SingleCustomPrice
          FROM      ( SELECT    VP.CP_Brand AS Brand ,
                                VP.PID ,
                                VP.DisplayName AS ProductName ,
                                ( ISNULL(( SELECT TOP 1
                                                    TPGP.Value
                                           FROM     Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara
                                                    AS TPGP WITH ( NOLOCK )
                                           WHERE    TPGP.Type = 'Base'
                                         ), 0)
                                  + ISNULL(( SELECT TOP 1
                                                    TPGP.Value
                                             FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara
                                                    AS TPGP WITH ( NOLOCK )
                                             WHERE  TPGP.Type = 'Rim'
                                                    AND TPGP.Item = VP.CP_Tire_Rim
                                           ), 0)
                                  + ISNULL(( SELECT TOP 1
                                                    TPGP.Value
                                             FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara
                                                    AS TPGP WITH ( NOLOCK )
                                             WHERE  TPGP.Type = 'Rof'
                                                    AND TPGP.Item = VP.CP_Tire_ROF
                                           ), 0)
                                  + ISNULL(( SELECT TOP 1
                                                    TPGP.Value
                                             FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara
                                                    AS TPGP WITH ( NOLOCK )
                                             WHERE  TPGP.Type = 'SalesQuantity'
                                                    AND TPGP.Item = DPSD.level
                                           ), 0)
                                  + ISNULL(( SELECT TOP 1
                                                    TPGP.Value
                                             FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara
                                                    AS TPGP WITH ( NOLOCK )
                                             WHERE  TPGP.Type = 'Pattern'
                                                    AND TPGP.Item = VP.CP_Tire_Pattern
                                           ), 0)
                                  + ISNULL(( SELECT TOP 1
                                                    TPGP.Value
                                             FROM   Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara
                                                    AS TPGP WITH ( NOLOCK )
                                             WHERE  TPGP.Type = 'Brand'
                                                    AND TPGP.Item = VP.CP_Brand
                                           ), 0)
                                  + +ISNULL(( SELECT TOP 1
                                                        TPGP.Value
                                              FROM      Tuhu_productcatalog.dbo.tbl_TirePriceGuidePara
                                                        AS TPGP WITH ( NOLOCK )
                                              WHERE     TPGP.Type = 'TireSize'
                                                        AND TPGP.Item = VP.TireSize
                                            ), 0) ) AS JiaQuan ,
                                DPSD.totalstock ,
                                TMVC.VehicleCount ,
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
                                DPSD.teweilun_tianmaoid AS TWLTMPID ,
                                DPSD.qccr_retailprice AS MLTTMPrice ,
                                DPSD.qccr_retailid AS MLTTMPID ,
                                DPSD.qccr_wholesaleprice AS MLTPrice ,
                                DPSD.qccr_wholesaleid AS MLTPID ,
                                QPL.Price AS QPLPrice ,
                                CT.NewPrice AS CouponPrice ,
                                DPSD.CaigouZaitu
                      FROM      Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
                                LEFT JOIN Tuhu_bi.dbo.dm_Product_SalespredictData
                                AS DPSD WITH ( NOLOCK ) ON VP.PID = DPSD.PID
                                LEFT JOIN Tuhu_productcatalog..ProductPriceMapping
                                AS QPL WITH ( NOLOCK ) ON ShopCode = N'汽配龙'
                                                          AND VP.PID = QPL.Pid
                                LEFT JOIN [Configuration].dbo.tbl_TireMatchVehicleCount
                                AS TMVC WITH ( NOLOCK ) ON VP.PID = TMVC.PID
                                LEFT JOIN Tuhu_productcatalog..tbl_CouponPrice
                                AS CT WITH ( NOLOCK ) ON CT.PID = VP.PID
                      WHERE     ( ( VP.PID LIKE '%' + @PID + '%'
                                    OR @PID IS NULL
                                  )
                                  AND VP.PID LIKE 'TR-%'
                                )
                                AND ( VP.DisplayName LIKE '%' + @ProductName
                                      + '%'
                                      OR @ProductName IS NULL
                                    )
                                AND ( @Brands IS NULL
                                      OR VP.CP_Brand COLLATE Chinese_PRC_CI_AS IN (
                                      SELECT    *
                                      FROM      Gungnir.dbo.Split(@Brands, ';') )
                                    )
                                AND ( @Patterns IS NULL
                                      OR VP.CP_Tire_Pattern COLLATE Chinese_PRC_CI_AS IN (
                                      SELECT    *
                                      FROM      Gungnir.dbo.Split(@Patterns,
                                                              ';') )
                                    )
                                AND ( @TireSizes IS NULL
                                      OR VP.TireSize COLLATE Chinese_PRC_CI_AS = @TireSizes
                                    )
                                AND ( @Rims IS NULL
                                      OR VP.CP_Tire_Rim COLLATE Chinese_PRC_CI_AS IN (
                                      SELECT    *
                                      FROM      Gungnir.dbo.Split(@Rims, ';') )
                                    )
                                AND ( @StockStatus = 0
                                      OR @StockStatus = 1
                                      AND VP.stockout = 1
                                      OR @StockStatus = 2
                                      AND DPSD.totalstock > DPSD.SelfStock
                                    )
                                AND ( @OnSaleStatus = 0
                                      OR @OnSaleStatus = 1
                                      AND VP.OnSale = 1
                                      OR @OnSaleStatus = 2
                                      AND VP.OnSale = 0
                                    )
                                AND ( @MaoLiE IS NULL
                                      OR ( @MaoLiE IS NOT NULL
                                           AND VP.cy_list_price > 0
                                           AND DPSD.cost > 0
                                           AND ( @MatchMaoLiE = 1
                                                 AND ISNULL(VP.cy_list_price,
                                                            0)
                                                 - ISNULL(DPSD.cost, 0) >= @MaoLiE
                                                 OR @MatchMaoLiE = -1
                                                 AND ISNULL(VP.cy_list_price,
                                                            0)
                                                 - ISNULL(DPSD.cost, 0) <= @MaoLiE
                                               )
                                         )
                                    )
                                AND ( @MatchStock = 0
                                      OR ( @MatchStock = -1
                                           AND DPSD.totalstock <= @MatchStockValue
                                           OR @MatchStock = 1
                                           AND DPSD.totalstock >= @MatchStockValue
                                         )
                                    )
                                AND ( @MatchZZTS = 0
                                      OR ( DPSD.num_month > 0
                                           AND ( @MatchZZTS = -1
                                                 AND DPSD.totalstock
                                                 / ( ( IIF(ISNULL(DPSD.num_month,
                                                              0) = 0, 1, DPSD.num_month))
                                                     / 30.0 ) <= @MatchZZTSValue
                                                 OR @MatchZZTS = 1
                                                 AND DPSD.totalstock
                                                 / ( ( IIF(ISNULL(DPSD.num_month,
                                                              0) = 0, 1, DPSD.num_month))
                                                     / 30.0 ) >= @MatchZZTSValue
                                               )
                                         )
                                    )
                    ) AS T
        ) AS U
        LEFT  JOIN Configuration.dbo.tbl_Tire_WarningLine AS TWL WITH ( NOLOCK ) ON ISNULL(U.ActualGuidePrice,
                                                              0) >= TWL.MinGuidePrice
                                                              AND ISNULL(U.ActualGuidePrice,
                                                              0) < TWL.MaxGuidePrice
WHERE   ( @PCPricePer IS NULL
          OR ( @PCPricePer IS NOT NULL
               AND U.Price > 0
               AND ( @MatchPCPricePer = 1
                     AND ( U.Price - U.ActualGuidePrice )
                     / IIF(U.ActualGuidePrice <= 0, 1, U.ActualGuidePrice) >= @PCPricePer
                     * 1.0 / 100
                     OR @MatchPCPricePer = -1
                     AND ( U.Price - U.ActualGuidePrice )
                     / IIF(U.ActualGuidePrice <= 0, 1, U.ActualGuidePrice) <= @PCPricePer
                     * 1.0 / 100
                   )
             )
        )
        AND ( @MonthCount = 0
              OR ( @MonthCount = 1
                   AND U.num_month >= @MonthCountValue
                   OR @MonthCount = -1
                   AND U.num_month <= @MonthCountValue
                 )
            )
        AND ( @WeekCount = 0
              OR ( @WeekCount = 1
                   AND U.num_week >= @WeekCountValue
                   OR @WeekCount = -1
                   AND U.num_week <= @WeekCountValue
                 )
            )
        AND ( @VehicleCount = 0
              OR ( @VehicleCount = 1
                   AND ( @VehicleCountValue > 0
                         AND U.VehicleCount >= @VehicleCountValue
                         OR @VehicleCountValue <= 0
                       )
                   OR @VehicleCount = -1
                   AND ( U.VehicleCount <= @VehicleCountValue
                         OR U.VehicleCount IS NULL
                       )
                   OR @VehicleCount = 2
                   AND @VehicleCountValue > 0
                   AND U.VehicleCount = @VehicleCountValue
                   OR @VehicleCount = 2
                   AND @VehicleCountValue = 0
                   AND ( U.VehicleCount = 0
                         OR U.VehicleCount IS NULL
                       )
                 )
            )
        AND ( @MatchWarnLine = -99
              OR ( @MatchWarnLine > -99
                   AND U.Price > 0
                   AND ( @MatchWarnLine = 1
                         AND U.ActualGuidePrice + TWL.UpperLimit < U.Price
                         OR @MatchWarnLine = -1
                         AND U.ActualGuidePrice - TWL.LowerLimit > U.Price
                       )
                 )
            )
        AND ( @Type = 0
              OR ( @Contrast = -1
                   AND U.Price > 0
                   AND ( @Type = 1
                         AND U.cost > 0
                         AND ( U.Price / IIF(ISNULL(U.cost, 0) = 0, 1, U.cost) <= @Proportion
                               OR U.QPLPrice
                               / IIF(ISNULL(U.cost, 0) = 0, 1, U.cost) <= @Proportion
                               AND U.QPLPrice > 0
                             )
                         OR @Type = 2
                         AND ( U.TBPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.TBPrice > 0
                               OR U.TB2Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.TB2Price > 0
                               OR U.TM1Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.TM1Price > 0
                               OR U.TM2Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.TM2Price > 0
                               OR U.TM3Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.TM3Price > 0
                               OR U.TM4Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.TM4Price > 0
                               OR U.JDPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.JDPrice > 0
                               OR U.JDFlagShipPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.JDFlagShipPrice > 0
                             )
                         OR @Type = 3
                         AND ( U.JDSelfPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.JDSelfPrice > 0
                               OR U.TWLTMPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.TWLTMPrice > 0
                               OR U.MLTTMPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) <= @Proportion
                               AND U.MLTTMPrice > 0
                             )
                         OR @Type = 4
                         AND ( ( U.Price
                                 / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                 AND U.Price > 0
                                 AND CHARINDEX('list', @ManyCustomPrice) > 0
                               )
                               OR ( U.TBPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.TBPrice > 0
                                    AND CHARINDEX('taobao1', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TB2Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.TB2Price > 0
                                    AND CHARINDEX('taobao2', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM1Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.TM1Price > 0
                                    AND CHARINDEX('tmall1', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM2Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.TM2Price > 0
                                    AND CHARINDEX('tmall2', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM3Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.TM3Price > 0
                                    AND CHARINDEX('tmall3', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM4Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.TM4Price > 0
                                    AND CHARINDEX('tmall4', @ManyCustomPrice) > 0
                                  )
                               OR ( U.JDPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.JDPrice > 0
                                    AND CHARINDEX('tuhujd', @ManyCustomPrice) > 0
                                  )
                               OR ( U.JDFlagShipPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.JDFlagShipPrice > 0
                                    AND CHARINDEX('jingdongqj',
                                                  @ManyCustomPrice) > 0
                                  )
                               OR ( U.QPLPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.QPLPrice > 0
                                    AND CHARINDEX('qipeilong',
                                                  @ManyCustomPrice) > 0
                                  )
                               OR ( U.TWLTMPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.TWLTMPrice > 0
                                    AND CHARINDEX('tmalltwl', @ManyCustomPrice) > 0
                                  )
                               OR ( U.JDSelfPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.JDSelfPrice > 0
                                    AND CHARINDEX('jingdongself',
                                                  @ManyCustomPrice) > 0
                                  )
                               OR ( U.MLTTMPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.MLTTMPrice > 0
                                    AND CHARINDEX('qccrl', @ManyCustomPrice) > 0
                                  )
                               OR ( U.MLTPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.MLTPrice > 0
                                    AND CHARINDEX('qccrp', @ManyCustomPrice) > 0
                                  )
                               OR ( U.cost
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.cost > 0
                                    AND CHARINDEX('cost', @ManyCustomPrice) > 0
                                  )
                               OR ( U.CouponPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) <= @Proportion
                                    AND U.CouponPrice > 0
                                    AND CHARINDEX('coupon', @ManyCustomPrice) > 0
                                  )
                             )
                       )
                   OR @Contrast = 1
                   AND U.Price > 0
                   AND ( @Type = 1
                         AND U.cost > 0
                         AND ( U.Price / IIF(ISNULL(U.cost, 0) = 0, 1, U.cost) >= @Proportion
                               OR U.QPLPrice
                               / IIF(ISNULL(U.cost, 0) = 0, 1, U.cost) >= @Proportion
                               AND U.QPLPrice > 0
                             )
                         OR @Type = 2
                         AND ( U.TBPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.TBPrice > 0
                               OR U.TB2Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.TB2Price > 0
                               OR U.TM1Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.TM1Price > 0
                               OR U.TM2Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.TM2Price > 0
                               OR U.TM3Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.TM3Price > 0
                               OR U.TM4Price
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.TM4Price > 0
                               OR U.JDPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.JDPrice > 0
                               OR U.JDFlagShipPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.JDFlagShipPrice > 0
                             )
                         OR @Type = 3
                         AND ( U.JDSelfPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.JDSelfPrice > 0
                               OR U.TWLTMPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.TWLTMPrice > 0
                               OR U.MLTTMPrice
                               / IIF(ISNULL(U.Price, 0) = 0, 1, U.Price) >= @Proportion
                               AND U.MLTTMPrice > 0
                             )
                         OR @Type = 4
                         AND ( ( U.Price
                                 / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                 AND U.Price > 0
                                 AND CHARINDEX('list', @ManyCustomPrice) > 0
                               )
                               OR ( U.TBPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.TBPrice > 0
                                    AND CHARINDEX('taobao1', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TB2Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.TB2Price > 0
                                    AND CHARINDEX('taobao2', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM1Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.TM1Price > 0
                                    AND CHARINDEX('tmall1', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM2Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.TM2Price > 0
                                    AND CHARINDEX('tmall2', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM3Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.TM3Price > 0
                                    AND CHARINDEX('tmall3', @ManyCustomPrice) > 0
                                  )
                               OR ( U.TM4Price
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.TM4Price > 0
                                    AND CHARINDEX('tmall4', @ManyCustomPrice) > 0
                                  )
                               OR ( U.JDPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.JDPrice > 0
                                    AND CHARINDEX('tuhujd', @ManyCustomPrice) > 0
                                  )
                               OR ( U.JDFlagShipPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.JDFlagShipPrice > 0
                                    AND CHARINDEX('jingdongqj',
                                                  @ManyCustomPrice) > 0
                                  )
                               OR ( U.QPLPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.QPLPrice > 0
                                    AND CHARINDEX('qipeilong',
                                                  @ManyCustomPrice) > 0
                                  )
                               OR ( U.TWLTMPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.TWLTMPrice > 0
                                    AND CHARINDEX('tmalltwl', @ManyCustomPrice) > 0
                                  )
                               OR ( U.JDSelfPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.JDSelfPrice > 0
                                    AND CHARINDEX('jingdongself',
                                                  @ManyCustomPrice) > 0
                                  )
                               OR ( U.MLTTMPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.MLTTMPrice > 0
                                    AND CHARINDEX('qccrl', @ManyCustomPrice) > 0
                                  )
                               OR ( U.MLTPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.MLTPrice > 0
                                    AND CHARINDEX('qccrp', @ManyCustomPrice) > 0
                                  )
                               OR ( U.cost
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.cost > 0
                                    AND CHARINDEX('cost', @ManyCustomPrice) > 0
                                  )
                               OR ( U.CouponPrice
                                    / IIF(ISNULL(U.SingleCustomPrice, 0) = 0, 1, U.SingleCustomPrice) >= @Proportion
                                    AND U.CouponPrice > 0
                                    AND CHARINDEX('coupon', @ManyCustomPrice) > 0
                                  )
                             )
                       )
                 )
            );";
            //var brandList = model.Brand?.Split(';').Where(g => !string.IsNullOrWhiteSpace(g))?.ToList() ?? new List<string>();
            //var brandSelect = brandList.Any() ? @"AND VP.CP_Brand IN (N'" + string.Join("',N'", brandList) + @"')" : "";
            //sql = string.Format(sql, brandSelect);
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 3 * 60;
                cmd.Parameters.AddRange(new SqlParameter[] {
                    new SqlParameter("@PID",string.IsNullOrWhiteSpace(model.PID)?null:model.PID),
                    new SqlParameter("@ProductName",string.IsNullOrWhiteSpace(model.ProductName)?null:model.ProductName),
                    new SqlParameter("@Brands",string.IsNullOrWhiteSpace(model.Brand)?null:model.Brand),
                    new SqlParameter("@StockStatus",model.StockStatus),
                    new SqlParameter("@Type",model.Type),
                    new SqlParameter("@Contrast",model.Contrast),
                    new SqlParameter("@Proportion",model.Proportion),
                    new SqlParameter("@MaoLiE",model.MaoLiE),
                    new SqlParameter("@MatchMaoLiE",model.MatchMaoLiE),
                    new SqlParameter("@MatchStock",model.MatchStock),
                    new SqlParameter("@MatchStockValue",model.MatchStockValue),
                    new SqlParameter("@MatchZZTS",model.MatchZZTS),
                    new SqlParameter("@MatchZZTSValue",model.MatchZZTSValue),
                    new SqlParameter("@PCPricePer",model.PCPricePer),
                    new SqlParameter("@MatchPCPricePer",model.MatchPCPricePer),
                    new SqlParameter("@OnSaleStatus",model.OnSaleStatus),
                    new SqlParameter("@ManyCustomPrice",model.ManyCustomPrice),
                    new SqlParameter("@SingleCustomPrice",model.SingleCustomPrice),
                    new SqlParameter("@MatchWarnLine",model.MatchWarnLine),
                    new SqlParameter("@MonthCount",model.MonthCount),
                    new SqlParameter("@MonthCountValue",model.MonthCountValue),
                    new SqlParameter("@WeekCount",model.WeekCount),
                    new SqlParameter("@WeekCountValue",model.WeekCountValue),
                    new SqlParameter("@VehicleCount",model.VehicleCount),
                    new SqlParameter("@VehicleCountvalue",model.VehicleCountValue),
                    new SqlParameter("@Patterns",model.Patterns),
                    new SqlParameter("@Rims",model.Rims),
                    new SqlParameter("@TireSizes",model.TireSize)
                });
                var OBJ = dbHelper.ExecuteScalar(cmd);
                return Convert.ToInt32(OBJ);
            }
        }
        #endregion

        public static IEnumerable<TireListModel> SelectUpdateBitch(string brand, string pattern, string tiresize)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT	DPSD.cost,
					VP.cy_list_price AS Price,
					VP.PID
			FROM	Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
			LEFT JOIN Tuhu_bi.dbo.dm_Product_SalespredictData AS DPSD WITH ( NOLOCK )
					ON VP.PID = DPSD.pid
			WHERE	VP.CP_Brand = @Brand
					AND VP.PID LIKE 'TR-%'
					AND (
						  VP.CP_Tire_Pattern = @Pattern
						  OR @Pattern IS NULL )
					AND (
						  VP.TireSize = @TireSize
						  OR @TireSize IS NULL ); ", CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@Brand", brand),
                    new SqlParameter("@Pattern", pattern),
                    new SqlParameter("@TireSize", tiresize) }).ConvertTo<TireListModel>();
            }
        }

        #region 券后价相关
        public static IEnumerable<CouponPriceHistory> CouponPriceChangeLog(string pid)
        {
            const string sql = @"SELECT PID,OldPrice,NewPrice,ChangeUser,ChangeReason,ChangeDateTime FROM Tuhu_log..tbl_CouponPriceHistory WITH(NOLOCK) WHERE PID=@PID ORDER BY ChangeDateTime DESC";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                return dbHelper.ExecuteDataTable(sql, CommandType.Text,
                    new SqlParameter[] { new SqlParameter("@PID", pid) }).ConvertTo<CouponPriceHistory>();
            }
        }

        public static int UpdateCouponPrice(CouponPriceHistory model)
        {
            const string sql1 = @"SELECT COUNT(1) FROM Tuhu_productcatalog..tbl_CouponPrice WITH(NOLOCK) WHERE PID=@PID";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                var isExit = DbHelper.ExecuteScalar(sql1, CommandType.Text, new SqlParameter[] { new SqlParameter("@PID", model.PID) });
                if (isExit == null)
                {
                    return 0;
                }
                string sql2 = "";
                if ((int)isExit > 0)
                {
                    sql2 = @"UPDATE Tuhu_productcatalog..tbl_CouponPrice SET NewPrice=@newPrice,UpdateDateTime=GETDATE(),LastUpdateDateTime=GETDATE() WHERE PID=@PID";

                }
                else
                {
                    sql2 = @"INSERT Tuhu_productcatalog..tbl_CouponPrice(PID,NewPrice) VALUES(@PID,@newPrice)";
                }
                var result = DbHelper.ExecuteNonQuery(sql2, CommandType.Text, new SqlParameter[] {
                        new SqlParameter("@newPrice",model.NewPrice),
                        new SqlParameter("@PID", model.PID)
                    });
                if (result > 0)
                {
                    const string sql3 = @"INSERT Tuhu_log..tbl_CouponPriceHistory (PID,OldPrice,NewPrice,ChangeUser,ChangeReason) VALUES(@PID,@OldPrice,@NewPrice,@ChangeUser,@ChangeReason)";
                    using (var dbHelper1 = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
                    {
                        return dbHelper1.ExecuteNonQuery(sql3, CommandType.Text, new SqlParameter[] {
                            new SqlParameter("@PID",model.PID),
                            new SqlParameter("@OldPrice",model.OldPrice),
                            new SqlParameter("NewPrice",model.NewPrice),
                            new SqlParameter("ChangeUser",model.ChangeUser),
                            new SqlParameter("ChangeReason",model.ChangeReason)
                        });
                    }
                }
                else
                {
                    return 0;
                }
            }
        }
        public static int ApplyUpdateCouponPrice(PriceApplyRequest Model, string applyPerson)
        {
            const string sql = @"INSERT Configuration..PriceApply(PID,Brand,CostPrice,LastCostPrice,StockCount,
WeekSaleCount,MonthSaleCount,GuidePrice,JDPrice,CarPrice,TuhuPrice,GrossProfit,PoductName,NowPrice,NewPrice,ApplyPerson,ApplyReason)
VALUES(@PID,@Brand,@CostPrice,@LastCostPrice,@StockCount,
@WeekSaleCount,@MonthSaleCount,@GuidePrice,@JDPrice,@CarPrice,@TuhuPrice,@GrossProfit,@PoductName,@NowPrice,@NewPrice,@ApplyPerson,@ApplyReason)";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(sql, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@PID",Model.PID),
                    new SqlParameter("@Brand",Model.Brand),
                    new SqlParameter("@CostPrice",Model.CostPrice),
                    new SqlParameter("@LastCostPrice",Model.LastCostPrice),
                    new SqlParameter("@StockCount",Model.StockCount),
                    new SqlParameter("@WeekSaleCount",Model.WeekSaleCount),
                    new SqlParameter("@MonthSaleCount",Model.MonthSaleCount),
                    new SqlParameter("@GuidePrice",Model.GuidePrice),
                    new SqlParameter("@JDPrice",Model.JDPrice),
                    new SqlParameter("@CarPrice",Model.CarPrice),
                    new SqlParameter("@TuhuPrice",Model.TuhuPrice),
                    new SqlParameter("@GrossProfit",Model.GrossProfit),
                    new SqlParameter("@PoductName",Model.PoductName),
                    new SqlParameter("@NowPrice",Model.NowPrice),
                    new SqlParameter("@NewPrice",Model.NewPrice),
                    new SqlParameter("@ApplyPerson",applyPerson),
                    new SqlParameter("@ApplyReason",Model.ApplyReason)
                });
            }
        }
        public static IEnumerable<PriceApply> GetCouponPriceApply()
        {
            const string sql = @"SELECT * from Configuration..PriceApply WITH(NOLOCK) WHERE ApplyStatus=0 ORDER BY ApplyDateTime DESC";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                return dbHelper.ExecuteDataTable(sql, CommandType.Text).ConvertTo<PriceApply>();
            }
        }
        public static PriceApply GetOneCouponPriceApply(int PKID)
        {
            const string sql = @"SELECT * from Configuration..PriceApply WITH(NOLOCK) WHERE ApplyStatus=0 AND PKID=@PKID";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@PKID",PKID)
                }).ConvertTo<PriceApply>().FirstOrDefault();
            }
        }
        public static string ApprovalCouponPrice(int PKID, bool pass, string ApprovalUser)
        {
            var apply = GetOneCouponPriceApply(PKID);
            if (pass)
            {
                var updateCouponPrice = UpdateCouponPrice(new CouponPriceHistory()
                {
                    PID = apply.PID,
                    OldPrice = apply.NowPrice,
                    NewPrice = apply.NewPrice,
                    ChangeUser = apply.ApplyPerson,
                    ChangeReason = @"审核通过(" + apply.ApplyReason + @")"
                });
                if (updateCouponPrice == 0)
                {
                    return null;
                }
            }
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                const string sql = @"UPDATE Configuration..PriceApply SET ApplyStatus=1, LastUpdateDateTime=GETDATE() WHERE PKID=@PKID";
                var result = dbHelper.ExecuteNonQuery(sql, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@PKID",PKID)
                });
                if (result <= 0)
                {
                    return null;
                }
            }
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                const string sql = @"INSERT Tuhu_log..PriceApproval(PID,Brand,CostPrice,LastCostPrice,StockCount,
WeekSaleCount,MonthSaleCount,GuidePrice,JDPrice,CarPrice,TuhuPrice,GrossProfit,PoductName,NowPrice,NewPrice,ApplyPerson,ApplyDateTime,ApplyReason,ApprovalPerson,ApprovalStatus)
VALUES(@PID,@Brand,@CostPrice,@LastCostPrice,@StockCount,
@WeekSaleCount,@MonthSaleCount,@GuidePrice,@JDPrice,@CarPrice,@TuhuPrice,@GrossProfit,
@PoductName,@NowPrice,@NewPrice,@ApplyPerson,@APplyDateTime,@ApplyReason,@ApprovalPerson,@ApprovalStatus)";
                var data = dbHelper.ExecuteNonQuery(sql, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@PID",apply.PID),
                    new SqlParameter("@Brand",apply.Brand),
                    new SqlParameter("@CostPrice",apply.CostPrice),
                    new SqlParameter("@LastCostPrice",apply.LastCostPrice),
                    new SqlParameter("@StockCount",apply.StockCount),
                    new SqlParameter("@WeekSaleCount",apply.WeekSaleCount),
                    new SqlParameter("@MonthSaleCount",apply.MonthSaleCount),
                    new SqlParameter("@GuidePrice",apply.GuidePrice),
                    new SqlParameter("@JDPrice",apply.JDPrice),
                    new SqlParameter("@CarPrice",apply.CarPrice),
                    new SqlParameter("@TuhuPrice",apply.TuhuPrice),
                    new SqlParameter("@GrossProfit",apply.GrossProfit),
                    new SqlParameter("@PoductName",apply.PoductName),
                    new SqlParameter("@NowPrice",apply.NowPrice),
                    new SqlParameter("@NewPrice",apply.NewPrice),
                    new SqlParameter("@ApplyPerson",apply.ApplyPerson),
                    new SqlParameter("@ApplyDateTime",apply.ApplyDateTime),
                    new SqlParameter("@ApplyReason",apply.ApplyReason),
                    new SqlParameter("@ApprovalPerson",ApprovalUser),
                    new SqlParameter("@ApprovalStatus",pass)
                });

                return apply.PID;

            }
        }
        public static IEnumerable<PriceApproval> CouponPriceApprovalLog(string pid, PagerModel pager)
        {
            const string sql = @"SELECT * FROM Tuhu_log..PriceApproval WITH(NOLOCK) WHERE PID=@PID OR @PID IS NULL ORDER BY ApprovalTime DESC OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter[] {
                    new SqlParameter("@PID",pid),
                    new SqlParameter("@Offset",pager.PageSize*(pager.CurrentPage-1)),
                    new SqlParameter("@Fetch",pager.PageSize)
                }).ConvertTo<PriceApproval>();
            }
        }

        public static int DeleteCouponPrice(string pid, string ChangeUser, decimal? price)
        {
            const string sql = @"DELETE FROM Tuhu_productcatalog..tbl_CouponPrice WHERE pid=@pid";
            using (var dbHelper = new SqlDbHelper())
            {
                var data = dbHelper.ExecuteNonQuery(sql, CommandType.Text, new SqlParameter[]
                {
                    new SqlParameter("@pid",pid),
                });
                if (data > 0)
                {
                    const string sql3 = @"INSERT Tuhu_log..tbl_CouponPriceHistory (PID,OldPrice,NewPrice,ChangeUser,ChangeReason) VALUES(@PID,@OldPrice,@NewPrice,@ChangeUser,@ChangeReason)";
                    using (var dbHelper1 = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
                    {
                        return dbHelper1.ExecuteNonQuery(sql3, CommandType.Text, new SqlParameter[] {
                            new SqlParameter("@PID",pid),
                            new SqlParameter("@OldPrice",price),
                            new SqlParameter("NewPrice",null),
                            new SqlParameter("ChangeUser",ChangeUser),
                            new SqlParameter("ChangeReason","手动删除券后价")
                        });
                    }
                }
                return data;
            }
        }
        #endregion

        #region 新增2017-06-16
        public static IEnumerable<ActivePriceModel> SelectActivePriceByPids(List<string> pids)
        {
            string sql =
                @"SELECT FSP.PID,MIN(Price) AS ActivePrice FROM Activity..tbl_FlashSaleProducts AS FSP WITH(NOLOCK) 
                        join @TVP p on FSP.PID COLLATE Chinese_PRC_CI_AS= p.TargetID
                    LEFT JOIN Activity..tbl_FlashSale AS FS WITH(NOLOCK) ON FS.ActivityID=FSP.ActivityID
WHERE FS.EndDateTime > GETDATE() AND ActiveType <> 2
GROUP BY FSP.PID";
            var records = new List<SqlDataRecord>(pids.Count);
            foreach (var target in pids)
            {
                var record = new SqlDataRecord(new SqlMetaData("TargetID", SqlDbType.Char, 100));
                var chars = new SqlChars(target);
                record.SetSqlChars(0, chars);
                records.Add(record);
            }
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                SqlParameter p = new SqlParameter("@TVP", SqlDbType.Structured);
                p.TypeName = "dbo.Targets";
                p.Value = records;
                cmd.Parameters.Add(p);
                return DbHelper.ExecuteDataTable(cmd).ConvertTo<ActivePriceModel>();
            }


        }

        public static IEnumerable<CaigouZaituModel> SelectCaigouZaituByPids(List<string> pids)
        {
            //const string sql = @"WITH PIDS AS (SELECT Item COLLATE Chinese_PRC_CI_AS AS PID FROM Tuhu_productcatalog..SplitString(@PIDS, ',', 1))
            //        SELECT PIDS.PID,PSD.CaigouZaitu
            //        FROM PIDS LEFT JOIN Tuhu_bi..dm_Product_SalespredictData AS PSD WITH(NOLOCK) ON PIDS.PID=PSD.pid";

            string sql = @"SELECT pid AS PID,CaigouZaitu FROM Tuhu_bi..dm_Product_SalespredictData WITH(NOLOCK) WHERE pid IN (N'" + string.Join("',N'", pids) + @"')";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_BI")))
            {
                return dbHelper.ExecuteDataTable(sql, CommandType.Text).ConvertTo<CaigouZaituModel>();
            }
        }
        #endregion


        #region 优惠券管理
        public static List<TireCouponModel> TireCouponManage( string ShopName)
        {
            var SqlStr = @"SELECT	PKID ,
		ShopName ,
		QualifiedPrice ,
		Reduce ,
		EndTime
FROM	Activity..TireCouponManage WITH ( NOLOCK )
WHERE	IsDelete = 0
		AND EndTime > GETDATE()
		AND ISNULL(@shopname, ShopName) = ShopName;";
            using (var cmd = new SqlCommand(SqlStr))
            {
                cmd.Parameters.AddWithValue("@shopname", ShopName);
                cmd.CommandType = CommandType.Text;
                return DbHelper.ExecuteDataTable(cmd).ConvertTo<TireCouponModel>()?.ToList() ?? new List<TireCouponModel>();
            }
        }


        public static int AddTireCoupon(string ShopName, decimal QualifiedPrice, decimal Reduce, DateTime EndTime)
        {
            var SqlStr = @"INSERT	INTO Activity..TireCouponManage
		(	ShopName ,
			QualifiedPrice ,
			Reduce ,
			EndTime
		)
OUTPUT	Inserted.PKID
VALUES	(	@shopname ,
			@Qp ,
			@Rd ,
			@Ed 
		);";
            using(var cmd=new SqlCommand(SqlStr))
            {
                cmd.Parameters.AddWithValue("@shopname", ShopName);
                cmd.Parameters.AddWithValue("@Qp", QualifiedPrice);
                cmd.Parameters.AddWithValue("@Rd", Reduce);
                cmd.Parameters.AddWithValue("@Ed", EndTime);
                cmd.CommandType = CommandType.Text;
                var dat = DbHelper.ExecuteScalar(cmd);
                if(Int32.TryParse(dat?.ToString(),out int value))
                {
                    return value;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static TireCouponModel DeleteTireCoupon(int pkid)
        {
            var SqlStr = @"
UPDATE	Activity..TireCouponManage WITH ( ROWLOCK )
SET		IsDelete = 1
OUTPUT	Inserted.PKID ,
		Inserted.ShopName ,
		Inserted.QualifiedPrice ,
		Inserted.Reduce ,
		Inserted.EndTime
WHERE	PKID = @pkid;";
            using(var cmd = new SqlCommand(SqlStr))
            {
                cmd.Parameters.AddWithValue("@pkid", pkid);
                return DbHelper.ExecuteDataTable(cmd).ConvertTo<TireCouponModel>()?.FirstOrDefault() ?? new TireCouponModel();
            }
        }

        public static List<TireCouponLogModel> FetchCouponLogByShopName(string ShopName)
        {
            var SqlStr = @"SELECT ShopName,CouponName,CouponEndTime,Operator,OperateType,CreateDateTime FROM Tuhu_log..TireCouponManageLog WITH(nolock) WHERE ShopName=@shopname";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using(var cmd=new SqlCommand(SqlStr))
                {
                    cmd.Parameters.AddWithValue("@shopname", ShopName);
                    return dbHelper.ExecuteDataTable(cmd).ConvertTo<TireCouponLogModel>()?.ToList() ?? new List<TireCouponLogModel>();
                }
            }
        }

        public static void AddTireCouponLog(string ShopName, string CouponName, DateTime CouponEndTime, string Operator, string OperatrType)
        {
            var SqlStr = @"INSERT INTO Tuhu_log..TireCouponManageLog (ShopName, CouponName, CouponEndTime, Operator, OperateType) VALUES (@shopName, @couponName, @CouponEndTime, @operator, @OperateType);";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(SqlStr))
                {
                    cmd.Parameters.AddWithValue("@shopName", ShopName);
                    cmd.Parameters.AddWithValue("@couponName", CouponName);
                    cmd.Parameters.AddWithValue("@CouponEndTime", CouponEndTime);
                    cmd.Parameters.AddWithValue("@operator", Operator);
                    cmd.Parameters.AddWithValue("@OperateType", OperatrType);
                    dbHelper.ExecuteNonQuery(cmd);
                }
            }
        }
        #endregion

    }
}


