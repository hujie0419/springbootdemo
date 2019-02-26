using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalProductInfomation
    {

        public static List<ProductInformation> GetAllProductInfomation(SqlConnection connection,
            Dictionary<string, string>.KeyCollection pricenNameList,
            Dictionary<string, string>.KeyCollection stockNameList)
        {
            string _PriceList = string.Empty;
            string _InPriceList = string.Empty;
            string _StockList = string.Empty;
            string _InStockList = string.Empty;
            foreach (string str in pricenNameList)
            {
                _PriceList += string.IsNullOrEmpty(_PriceList)
                    ? "CONVERT(NVARCHAR(12), ISNULL(TB_PRICE.[" + str + "], 0))"
                    : "+',' + CONVERT(NVARCHAR(12), ISNULL(TB_PRICE.[" + str + "], 0))";
                _InPriceList += string.IsNullOrEmpty(_InPriceList) ? "[" + str + "]" : ",[" + str + "]";
            }
            foreach (string str in stockNameList)
            {
                _StockList += string.IsNullOrEmpty(_StockList)
                    ? "CONVERT(NVARCHAR(12), ISNULL(TB_STOCK.[" + str + "], 0))"
                    : "+',' + CONVERT(NVARCHAR(12), ISNULL(TB_STOCK.[" + str + "], 0))";
                _InStockList += string.IsNullOrEmpty(_InStockList) ? "[" + str + "]" : ",[" + str + "]";
            }
            if (!string.IsNullOrEmpty(_PriceList))
            {
                _PriceList = "(" + _PriceList + ") AS PriceList,";
            }
            if (!string.IsNullOrEmpty(_StockList))
            {
                _StockList = "(" + _StockList + ") AS StockList";
            }
            var sql = @"SELECT
ISNULL(TB_Commission.Commission, 1) AS Commission,
(TB_PRODUCT.ProductID+'|'+TB_PRODUCT.VariantID) AS PID,
TB_PRODUCT.DisplayName,

Brand,CP_Tire_Width,CP_Tire_AspectRatio,CP_Tire_Rim,CP_Tire_SpeedRating,CP_Tire_Type,Name,
CP_Tire_LoadIndex,ROF,CP_Place,CP_Tire_Pattern,DefinitionName,

ISNULL(TB_PRODUCT.cy_list_price, 0) AS SitePrice,
ISNULL(TB_PRODUCT.cy_bj_price, 0) AS SiteBJPrice,TB_PRODUCT.OnSale,
TB_PRODUCT.stockout,
		" + _PriceList + _StockList + @"
FROM	(SELECT	CP.ProductID,
				CP.VariantID,
				C.DisplayName,

C.CP_Brand Brand, C.CP_Tire_Width, C.CP_Tire_AspectRatio,  C.CP_Tire_Rim,
ISNULL( C.CP_Tire_SpeedRating, 0) CP_Tire_SpeedRating, 
ISNULL( CP.CP_Tire_Type, 0) CP_Tire_Type,c.Name,
ISNULL(C.CP_Tire_LoadIndex,0) CP_Tire_LoadIndex, 
(CASE   WHEN  CP_Tire_ROF='防爆' THEN '缺气防爆轮胎' ELSE '' end) ROF,
CP.CP_Place,ISNULL(C.CP_Tire_Pattern,0) CP_Tire_Pattern,
                DefinitionName,
				CP.OnSale,
                CP.cy_list_price,
                CP.cy_bj_price,
				isnull(CP.stockout,0) as stockout
		 FROM	Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH (NOLOCK)
		 JOIN	Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] AS C WITH (NOLOCK)
				ON C.#Catalog_Lang_Oid = CP.oid
		 WHERE	CP.i_ClassType IN (2, 4) AND C.DisplayName IS NOT NULL
		) AS TB_PRODUCT
LEFT	JOIN (SELECT	ShopCode,
						Pid,
						Price
			  FROM		Tuhu_productcatalog..ProductPriceMapping WITH (NOLOCK)
			 ) AS T PIVOT ( MIN(Price) FOR ShopCode IN (" + _InPriceList + @") ) AS TB_PRICE
		ON TB_PRODUCT.ProductID + '|' + TB_PRODUCT.VariantID COLLATE Chinese_PRC_CI_AS = TB_PRICE.Pid
LEFT JOIN (SELECT	PID,
					Commission
		   FROM		ProductCommission WITH (NOLOCK)
		  ) AS TB_Commission
		ON TB_PRODUCT.ProductID + '|' + TB_PRODUCT.VariantID COLLATE Chinese_PRC_CI_AS = TB_Commission.PID
LEFT JOIN (SELECT	WareHouseId,
					ProductId,
					AvailableStockQuantity
		   FROM		OrderProductInformation WITH (NOLOCK)
		  ) AS T2 PIVOT ( SUM(AvailableStockQuantity) FOR WareHouseId IN (" + _InStockList + @") ) AS TB_STOCK
		ON TB_PRODUCT.ProductID + '|' + TB_PRODUCT.VariantID COLLATE Chinese_PRC_CI_AS = TB_STOCK.ProductId
   ";
            return
                SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<ProductInformation>().ToList();
        }

        public static List<ProductInfo_Order> GetProductInfo_Order(SqlConnection connection, string PIDS,
            string OrderChannel, string fromRegion)
        {
            List<ProductInfo_Order> _ProductInfo_OrderList = new List<ProductInfo_Order>();
            try
            {
                string _PIDList = string.Empty;
                string[] _PIDArray = PIDS.Split(',');
                foreach (string str in _PIDArray)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        string[] _PIDItem = str.Split('^');
                        _PIDList += string.IsNullOrEmpty(_PIDList) ? "'" + _PIDItem[0] + "'" : ",'" + _PIDItem[0] + "'";
                    }
                }
                if (!string.IsNullOrEmpty(_PIDList))
                {
                    var sql = @"SELECT  1 AS Num ,
                                    ( CP.ProductID + '|' + CP.VariantID ) AS PID ,
                                    CP.DefinitionName ,
                                    C.DisplayName ,
                                    ISNULL(CP.cy_list_price, 0) AS Price ,
                                    ISNULL(CP.cy_bj_price, 0) AS BJ_Price ,
                                    ISNULL(CP.cy_marketing_price, 0) AS MarkedPrice ,
                                    ISNULL(( CP_Tire_Width + '/' + CP_Tire_AspectRatio + 'R' + CP_Tire_Rim
                                             + ' ' + CP_Tire_LoadIndex + CP_Tire_SpeedRating ), '') AS Size ,
                                    CP.invoice ,
                                    CP.OnSale ,
                                    CP.stockout ,
                                    CP.cy_cost ,
                                    CP_Tire_Rim AS Tire_Rim ,
                                    CP.CP_Tire_ROF AS Tire_ROF ,
                                    CP.PrimaryParentCategory AS Catalog
                            FROM    Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH ( NOLOCK )
                                    JOIN Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] AS C WITH ( NOLOCK ) ON CP.oid = C.#Catalog_Lang_Oid
                            WHERE   i_ClassType IN ( 2, 4 )
                                    AND CP.ProductID + '|' + CP.VariantID COLLATE Chinese_PRC_CI_AS IN (
                                    " + _PIDList + " )";
                    _ProductInfo_OrderList =
                        SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql)
                            .ConvertTo<ProductInfo_Order>()
                            .Select(p => new ProductInfo_Order
                            {
                                PID = p.PID,
                                DefinitionName = p.DefinitionName,
                                DisplayName = p.DisplayName,
                                Price =
                                    fromRegion == "bj" && p.BJ_Price > 0
                                        ? p.BJ_Price
                                        : GetPriceByOrderChannel(connection, p.PID, OrderChannel,
                                            decimal.Parse(string.Format("{0:N2}", p.Price))),
                                MarkedPrice = decimal.Parse(string.Format("{0:N2}", p.MarkedPrice)),
                                Size = p.Size,
                                invoice = p.invoice,
                                OnSale = p.OnSale,
                                stockout = p.stockout,
                                Num = GetCartNum(PIDS, p.PID),
                                Tire_Rim = p.Tire_Rim,
                                Tire_ROF = p.Tire_ROF,
                                Catalog = p.Catalog
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
            }
            return _ProductInfo_OrderList;
        }

        public static List<ProductInfo_Order> GetProductInfo_Order(SqlConnection connection, string PIDS,
            string OrderChannel)
        {
            List<ProductInfo_Order> _ProductInfo_OrderList = new List<ProductInfo_Order>();
            try
            {
                string _PIDList = string.Empty;
                string[] _PIDArray = PIDS.Split(',');
                foreach (string str in _PIDArray)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        string[] _PIDItem = str.Split('^');
                        _PIDList += string.IsNullOrEmpty(_PIDList) ? "'" + _PIDItem[0] + "'" : ",'" + _PIDItem[0] + "'";
                    }
                }
                if (!string.IsNullOrEmpty(_PIDList))
                {
                    var sql = @"SELECT	1 AS Num,
		                                (CP.ProductID + '|' + CP.VariantID) AS PID,
		                                CP.DefinitionName,
		                                C.DisplayName,
		                                ISNULL(CP.cy_list_price, 0) AS Price,
		                                ISNULL(CP.cy_marketing_price, 0) AS MarkedPrice,
		                                ISNULL(( ISNULL(CP_Tire_Width, '') + '/' + ISNULL(CP_Tire_AspectRatio,
                                                          '') + 'R'
                                         + ISNULL(CP_Tire_Rim, '') + ' ' + ISNULL(CP_Tire_LoadIndex,
                                                                                  '')
                                         + ISNULL(CP_Tire_SpeedRating, '') ), '') AS Size ,
		                                CP.invoice,
		                                CP.OnSale,
		                                CP.stockout,
		                                CP.cy_cost,
		                                CP_Tire_Rim AS Tire_Rim,
		                                CP.CP_Tire_ROF AS Tire_ROF,
		                                CP.CatalogName,
                                        CP.PrimaryParentCategory as Catalog,
                                        C.CP_Brake_Position,
                                        C.CP_Brake_Type ,
                                        CP.Weight ,
                                        CP_ShuXing1
                                FROM	Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH (NOLOCK)
                                JOIN	Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] AS C WITH (NOLOCK)
		                                ON CP.oid = C.#Catalog_Lang_Oid
                                WHERE	i_ClassType IN (2, 4)
                                AND CP.ProductID + '|' + CP.VariantID COLLATE Chinese_PRC_CI_AS
                                IN (" + _PIDList + ")";
                    _ProductInfo_OrderList =
                        SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql)
                            .ConvertTo<ProductInfo_Order>()
                            .Select(p => new ProductInfo_Order
                            {
                                PID = p.PID,
                                DefinitionName = p.DefinitionName,
                                DisplayName = p.DisplayName,
                                Price =
                                    GetPriceByOrderChannel(connection, p.PID, OrderChannel,
                                        decimal.Parse(string.Format("{0:N2}", p.Price))),
                                MarkedPrice = decimal.Parse(string.Format("{0:N2}", p.MarkedPrice)),
                                Size = p.Size,
                                invoice = p.invoice,
                                OnSale = p.OnSale,
                                stockout = p.stockout,
                                Num = GetCartNum(PIDS, p.PID),
                                Tire_Rim = string.IsNullOrEmpty(p.Tire_Rim) ? "" : p.Tire_Rim.Substring(0, 2),
                                Tire_ROF = p.Tire_ROF,
                                CatalogName = p.CatalogName,
                                Catalog = p.Catalog,
                                CP_Brake_Position = p.CP_Brake_Position,
                                CP_Brake_Type = p.CP_Brake_Type,
                                Weight = p.Weight,
                                CP_ShuXing1 = p.CP_ShuXing1
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
            }
            return _ProductInfo_OrderList;
        }

        public static decimal GetPriceByOrderChannel(SqlConnection connection, string PID, string OrderChannel,
            decimal SitePrice)
        {
            if (string.IsNullOrEmpty(OrderChannel) || string.IsNullOrEmpty(PID))
            {
                return SitePrice;
            }
            else
            {
                var sqlParamters = new[]
                {
                    new SqlParameter("@Pid", PID),
                    new SqlParameter("@ShopCode", OrderChannel)
                };
                var _price = SqlHelper.ExecuteScalar(connection, CommandType.Text,
                    @"SELECT top 1 Price FROM Tuhu_productcatalog..ProductPriceMapping WITH (NOLOCK) where ShopCode=@ShopCode and Pid=@Pid",
                    sqlParamters);
                if (_price == null || string.IsNullOrEmpty(_price.ToString()))
                {
                    return SitePrice;
                }
                else
                {
                    return decimal.Parse(string.Format("{0:N2}", _price));
                }
            }
        }

        public static int GetCartNum(string PIDS, string PID)
        {
            int _NUM = 1;
            try
            {
                string[] _PIDArray = PIDS.Split(',');
                foreach (string str in _PIDArray)
                {
                    string[] _PIDItem = str.Split('^');
                    if (_PIDItem[0] == PID)
                    {
                        _NUM = int.Parse(_PIDItem[1]);
                        break;
                    }
                }
            }
            catch
            {
            }
            return _NUM;
        }

        public static long PlatFormItemID(SqlConnection connection, string PID, string OrderChannel)
        {
            long _ItemID = 0;
            var sqlParamters = new[]
            {
                new SqlParameter("@Pid", string.IsNullOrEmpty(PID) ? "" : PID),
                new SqlParameter("@ShopCode", string.IsNullOrEmpty(OrderChannel) ? "" : OrderChannel)
            };
            string _SqlStr = "SELECT " + (OrderChannel.Contains("京东") ? "SkuID" : "ItemID") +
                             " FROM Tuhu_productcatalog..ProductPriceMapping where Pid=@Pid and ShopCode=@ShopCode";
            var _Com = SqlHelper.ExecuteScalar(connection, CommandType.Text, _SqlStr, sqlParamters);
            long.TryParse(_Com == null ? "1" : _Com.ToString(), out _ItemID);
            return _ItemID;
        }

        public static void UpdateCommission(SqlConnection connection, string PID, float Commission)
        {
            var sqlParamters1 = new[]
            {
                new SqlParameter("@PID", PID)
            };
            var sqlParamters2 = new[]
            {
                new SqlParameter("@PID", PID),
                new SqlParameter("@Commission", Commission)
            };
            if (
                SqlHelper.ExecuteScalar(connection, CommandType.Text,
                    "select top 1 PID from ProductCommission where PID=@PID", sqlParamters1) == null)
            {
                SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                    @"insert into ProductCommission(PID,Commission) VALUES(@PID,@Commission)", sqlParamters2);
            }
            else
            {
                SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                    @"UPDATE ProductCommission SET Commission=@Commission WHERE PID=@PID", sqlParamters2);
            }
        }

        public static void UpdatePrice(SqlConnection connection, string PID, float Price, string ShopCode)
        {
            var sqlParamters1 = new[]
            {
                new SqlParameter("@PID", PID),
                new SqlParameter("@ShopCode", ShopCode)
            };
            var sqlParamters2 = new[]
            {
                new SqlParameter("@PID", PID),
                new SqlParameter("@Price", Price),
                new SqlParameter("@ShopCode", ShopCode)
            };
            if (
                SqlHelper.ExecuteScalar(connection, CommandType.Text,
                    "select top 1 Pid from Tuhu_productcatalog..ProductPriceMapping where Pid=@PID AND ShopCode=@ShopCode",
                    sqlParamters1) == null)
            {
                SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                    @"INSERT INTO Tuhu_productcatalog..ProductPriceMapping
(ShopCode,Pid,ItemID,SkuID,Properties,Price,Promotion,Title,CreateDateTime,LastUpdateDateTime)
VALUES(@ShopCode,@PID," + double.Parse(DateTime.Now.ToString("yyMMddHHmmssfff")) +
                    ",0,NULL,@Price,0,'Title',getdate(),getdate())", sqlParamters2);
            }
            else
            {
                SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                    @"UPDATE Tuhu_productcatalog..ProductPriceMapping SET Price=@Price WHERE Pid=@PID AND ShopCode=@ShopCode",
                    sqlParamters2);
            }
        }

        public static List<ProductUser> GetProductUser(SqlConnection connection, string UserPhone, string UserTel,
            string TaoBaoID, string CarNO)
        {
            List<ProductUser> _ProductUsers = new List<ProductUser>();
            try
            {
                string _SqlStr = @"SELECT a.UserID,
ISNULL(a.last_name,'') AS last_name,
ISNULL(a.email,'') AS email,
ISNULL(a.mobile,'') AS mobile,
ISNULL(a.telephone,'') AS telephone,
ISNULL(a.TaobaoId,'') AS TaobaoId,
ISNULL(b.CarID,'') AS CarID,
ISNULL(b.carno,'') AS carno
  FROM vw_Remote_UserObject a WITH (NOLOCK)
  LEFT JOIN
  vw_Remote_CarObject b WITH (NOLOCK)
  ON '{'+a.UserID+'}'=b.UserID";

                var sqlParamters = new[]
                {
                    new SqlParameter("@UserPhone", string.IsNullOrEmpty(UserPhone) ? "" : UserPhone),
                    new SqlParameter("@UserTel", string.IsNullOrEmpty(UserTel) ? "" : UserTel),
                    new SqlParameter("@TaoBaoID", string.IsNullOrEmpty(TaoBaoID) ? "" : TaoBaoID),
                    new SqlParameter("@CarNO", string.IsNullOrEmpty(CarNO) ? "" : CarNO),
                };
                string _AddSql = string.Empty;
                _AddSql = string.IsNullOrEmpty(UserPhone) ? "" : " where a.telephone =@telephone";
                _AddSql += string.IsNullOrEmpty(UserTel)
                    ? ""
                    : (string.IsNullOrEmpty(_AddSql) ? " where" : " and") + " a.mobile =@UserTel";
                _AddSql += string.IsNullOrEmpty(TaoBaoID)
                    ? ""
                    : (string.IsNullOrEmpty(_AddSql) ? " where" : " and") + " a.TaobaoId=@TaobaoId";
                _AddSql += string.IsNullOrEmpty(CarNO)
                    ? ""
                    : (string.IsNullOrEmpty(_AddSql) ? " where" : " and") + " b.carno like N'%" + CarNO + "%'";
                _SqlStr = _SqlStr + _AddSql;
                _ProductUsers =
                    SqlHelper.ExecuteDataTable(connection, CommandType.Text, _SqlStr, sqlParamters)
                        .ConvertTo<ProductUser>()
                        .ToList();

                //_ProductUsers = SqlHelper.ExecuteDataTable(connection, CommandType.Text, _SqlStr).ConvertTo<ProductUser>().ToList().Where(p => string.IsNullOrEmpty(UserName) ? true : p.last_name.Contains(UserName)
                //	&& string.IsNullOrEmpty(UserTel) ? true : p.mobile.Contains(UserTel)
                //	&& string.IsNullOrEmpty(TaoBaoID) ? true : p.TaobaoId.Contains(TaoBaoID)
                //	&& string.IsNullOrEmpty(CarNO) ? true : p.carno.Contains(CarNO)).ToList();
            }
            catch (Exception ex)
            {
            }
            return _ProductUsers;
        }

        public static List<BizShopSimple> GetShopSimple(SqlConnection connection)
        {
            List<BizShopSimple> _ShopSimple = new List<BizShopSimple>();
            try
            {
                string _SqlStr =
                    @"SELECT PKID,Category,Province,City,District,RegionID,SimpleName,Service,ShopType,ShopStatus,WorkTime,Position,FirstPriority,AddressBrief,Address,Pos,CarparName
FROM dbo.Shops WITH (NOLOCK)
where IsActive=1 AND (ShopType&1)=1 and isnull(Service,'')<>'' AND ShopStatus IS NULL";
                _ShopSimple =
                    SqlHelper.ExecuteDataTable(connection, CommandType.Text, _SqlStr, null)
                        .ConvertTo<BizShopSimple>()
                        .ToList();
            }
            catch (Exception ex)
            {
            }
            return _ShopSimple;
        }

        public static int SaveOrder(SqlConnection connection, BizOrder bizOrder)
        {

            var sqlParamters = new[]
            {
                new SqlParameter("@UserId", bizOrder.UserId),
                new SqlParameter("@UserName", bizOrder.UserName ?? string.Empty),
                new SqlParameter("@UserTel", bizOrder.UserTel ?? string.Empty),
                new SqlParameter("@Status", bizOrder.Status ?? string.Empty),
                new SqlParameter("@OrderDatetime", bizOrder.OrderDatetime),
                new SqlParameter("@Owner", bizOrder.Owner ?? string.Empty),
                new SqlParameter("@Submitor", bizOrder.Submitor ?? string.Empty),
                new SqlParameter("@PurchaseStatus", bizOrder.PurchaseStatus),
                new SqlParameter("@PayStatus", bizOrder.PayStatus ?? string.Empty),
                new SqlParameter("@DeliveryStatus", bizOrder.DeliveryStatus ?? string.Empty),
                new SqlParameter("@OrderType", bizOrder.OrderType ?? string.Empty),
                new SqlParameter("@OrderChannel", bizOrder.OrderChannel ?? string.Empty),
                new SqlParameter("@CarID", Guid.Parse(bizOrder.CarID ?? Guid.NewGuid().ToString())),
                new SqlParameter("@CarPlate", bizOrder.CarPlate ?? string.Empty),
                new SqlParameter("@Refno", bizOrder.Refno ?? string.Empty),
                new SqlParameter("@DeliveryType", bizOrder.DeliveryType ?? string.Empty),
                new SqlParameter("@PayMothed", bizOrder.PayMothed ?? string.Empty),
                new SqlParameter("@InvoiceType", bizOrder.InvoiceType ?? string.Empty),
                new SqlParameter("@InvoiceTitle", bizOrder.InvoiceTitle ?? string.Empty),
                new SqlParameter("@InvTaxNum", bizOrder.InvTaxNum),
                new SqlParameter("@InvBank", bizOrder.InvBank ?? string.Empty),
                new SqlParameter("@InvBankAccount", bizOrder.InvBankAccount ?? string.Empty),
                new SqlParameter("@InvAmont", bizOrder.InvAmont),
                new SqlParameter("@InvAddress", bizOrder.InvAddress ?? string.Empty),
                new SqlParameter("@ShippingMoney", bizOrder.ShippingMoney),
                new SqlParameter("@InstallMoney", bizOrder.InstallMoney),
                new SqlParameter("@InvoiceAddTax", bizOrder.InvoiceAddTax),
                new SqlParameter("@InstallType", bizOrder.InstallType ?? string.Empty),
                new SqlParameter("@InstallShopName", bizOrder.InstallShopName ?? string.Empty),
                new SqlParameter("@InstallShopID", bizOrder.InstallShopID),
                new SqlParameter("@BookDatetime", bizOrder.BookDatetime),
                new SqlParameter("@BookPeriod", bizOrder.BookPeriod ?? string.Empty),
                new SqlParameter("@RegionId", bizOrder.RegionId ?? 0),
                new SqlParameter("@WareHouseId", bizOrder.WareHouseId),
                new SqlParameter("@WareHouseName", bizOrder.WareHouseName ?? string.Empty),
                new SqlParameter("@DeliveryCompany", bizOrder.DeliveryCompany ?? string.Empty),
                new SqlParameter("@DeliveryAddressID",
                    Guid.Parse(bizOrder.DeliveryAddressID ?? Guid.NewGuid().ToString())),
                new SqlParameter("@Remark", bizOrder.Remark == null ? "" : bizOrder.Remark)
            };
            return int.Parse(SqlHelper.ExecuteScalar(connection, CommandType.Text, @"insert into 
tbl_Order(UserID,UserName,UserTel,Status,OrderDatetime,Owner,Submitor,PurchaseStatus,PayStatus,DeliveryStatus,OrderType,OrderChannel,
CarID,CarPlate,Refno,DeliveryType,PayMothed,InvoiceType,InvoiceTitle,InvTaxNum,InvBank,InvBankAccount
,InvAmont,InvAddress,ShippingMoney,InstallMoney,InvoiceAddTax,InstallType,InstallShop,InstallShopID,BookDatetime,BookPeriod

,RegionId,WareHouseId,WareHouseName,DeliveryCompany," + (bizOrder.DeliveryAddressID == null ? "" : "DeliveryAddressID,") +
                                                                                   @"Remark) 
VALUES(@UserID,@UserName,@UserTel,@Status,@OrderDatetime,@Owner,@Submitor,@PurchaseStatus,@PayStatus,@DeliveryStatus,@OrderType,@OrderChannel
,@CarID,@CarPlate,@Refno,@DeliveryType,@PayMothed,@InvoiceType,@InvoiceTitle,@InvTaxNum,@InvBank,@InvBankAccount
,@InvAmont,@InvAddress,@ShippingMoney,@InstallMoney,@InvoiceAddTax,@InstallType,@InstallShopName,@InstallShopID,@BookDatetime,@BookPeriod

,@RegionId,@WareHouseId,@WareHouseName,@DeliveryCompany," +
                                                                                   (bizOrder.DeliveryAddressID == null
                                                                                       ? ""
                                                                                       : "@DeliveryAddressID,") +
                                                                                   @"@Remark)
SELECT @@IDENTITY", sqlParamters).ToString());
        }

        public static float GetCommission(SqlConnection connection, string pID)
        {
            float _Commission = 1;
            var sqlParamters = new[]
            {
                new SqlParameter("@PID", pID),
            };
            string sql = "SELECT Commission  FROM ProductCommission where PID=@PID";
            var _Com = SqlHelper.ExecuteScalar(connection, CommandType.Text, sql, sqlParamters);
            float.TryParse(_Com == null ? "1" : _Com.ToString(), out _Commission);
            return _Commission;
        }

        public static void SaveOrderNo(SqlConnection connection, int PKID, string OrderNo)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID", PKID),
                new SqlParameter("@OrderNo", OrderNo)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"UPDATE tbl_Order SET OrderNo=@OrderNo WHERE PKID=@PKID", sqlParamters);

        }

        public static void SaveOrderItem(SqlConnection connection, OrderListProduct orderListProduct)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@OrderID", orderListProduct.OrderID),
                new SqlParameter("@OrderNo", orderListProduct.OrderNo),
                new SqlParameter("@PID", orderListProduct.PID),
                new SqlParameter("@Category",
                    string.IsNullOrEmpty(orderListProduct.Category) ? "" : orderListProduct.Category),
                new SqlParameter("@Name", orderListProduct.Name),
                new SqlParameter("@Size", string.IsNullOrEmpty(orderListProduct.Size) ? "" : orderListProduct.Size),
                new SqlParameter("@InstallFee", orderListProduct.InstallFee),
                new SqlParameter("@Num", orderListProduct.Num),
                new SqlParameter("@MarkedPrice", orderListProduct.MarkedPrice),
                new SqlParameter("@Price", orderListProduct.Price),
                new SqlParameter("@Discount", orderListProduct.Discount),
                new SqlParameter("@TotalDiscount", orderListProduct.TotalDiscount),
                new SqlParameter("@TotalPrice", orderListProduct.TotalPrice),
                new SqlParameter("@CreateDate", orderListProduct.CreateDate),
                new SqlParameter("@LastUpdateTime", orderListProduct.LastUpdateTime),
                new SqlParameter("@Remark", string.IsNullOrEmpty(orderListProduct.Remark) ? "" : orderListProduct.Remark),
                new SqlParameter("@Commission", GetCommission(connection, orderListProduct.PID))
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, @"insert into 
tbl_OrderList(OrderID,OrderNo,PID,Category,Name,Size,InstallFee,Num,MarkedPrice,Price,Discount,TotalDiscount,TotalPrice,CreateDate,LastUpdateTime,Remark,Commission) 
VALUES(@OrderID,@OrderNo,@PID,@Category,@Name,@Size,@InstallFee,@Num,@MarkedPrice,@Price,@Discount,@TotalDiscount,@TotalPrice,@CreateDate,@LastUpdateTime,@Remark,@Commission)",
                sqlParamters);
        }


        public static void SaveOrderSumPrice(SqlConnection connection, int PKID, decimal SumMarkedMoney, int SumNum,
            decimal SumDisMoney, decimal SumMoney)
        {
            SqlParameter[] sqlParamters = new SqlParameter[]
            {
                new SqlParameter("@PKID", SqlDbType.Int),
                new SqlParameter("@SumMarkedMoney", SqlDbType.Money),
                new SqlParameter("@SumNum", SqlDbType.Int),
                new SqlParameter("@SumDisMoney", SqlDbType.Money),
                new SqlParameter("@SumMoney", SqlDbType.Money)
            };
            sqlParamters[0].Value = PKID;
            sqlParamters[1].Value = SumMarkedMoney;
            sqlParamters[2].Value = SumNum;
            sqlParamters[3].Value = SumDisMoney;
            sqlParamters[4].Value = SumMoney;

            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                "UPDATE tbl_Order SET SumMarkedMoney=@SumMarkedMoney,SumNum=@SumNum,SumDisMoney=@SumDisMoney,SumMoney=@SumMoney WHERE PKID=@PKID",
                sqlParamters);
        }

        public static List<string> SelectBaoyangPIDs(SqlConnection connection)
        {
            var baoyangPIDs = new List<string>();

            using (
                var reader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure,
                    "Product_SelectBaoyangPIDs"))
            {
                while (reader.Read())
                {
                    var pid = reader.GetTuhuString(0);
                    if (!string.IsNullOrEmpty(pid) && !baoyangPIDs.Contains(pid))
                    {
                        baoyangPIDs.Add(pid);
                    }
                }
            }

            return baoyangPIDs;
        }

        public static List<ProductInformation> SelectBYPID(SqlConnection connection)
        {
            var baoyangPIDs = new List<ProductInformation>();

            using (
                var reader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure,
                    "Product_SelectBaoyangPIDs"))
            {
                while (reader.Read())
                {
                    var prodcutInfo = new ProductInformation();
                    prodcutInfo.PID = reader.GetTuhuString(0);
                    if (!string.IsNullOrEmpty(prodcutInfo.PID))
                    {
                        baoyangPIDs.Add(prodcutInfo);
                    }
                }
            }

            return baoyangPIDs;
        }

        public static List<ProductSalesPrice> SelectProductSalesPrice(SqlConnection cn)
        {
            var productSalesPrices = new List<ProductSalesPrice>();

            using (
                SqlDataReader reader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure,
                    "Product_SelectProductWithSalesPrice"))
            {
                while (reader.Read())
                {
                    ProductSalesPrice psp = new ProductSalesPrice();
                    psp.ProductId = reader.GetTuhuString(0);
                    psp.ProductName = reader.GetTuhuString(1);
                    psp.TuhuPrice = reader.GetTuhuValue<decimal>(2);
                    psp.OnSale = reader.GetTuhuValue<bool>(3);
                    psp.TianMaoOnePrice = reader.GetTuhuValue<decimal>(4);
                    psp.TianMaoTwoPrice = reader.GetTuhuValue<decimal>(5);
                    psp.TianMaoThreePrice = reader.GetTuhuValue<decimal>(6);
                    psp.TaoBaoPrice = reader.GetTuhuValue<decimal>(7);
                    psp.JingDongPrice = reader.GetTuhuValue<decimal>(8);
                    psp.Brand = reader.GetTuhuString(9);
                    psp.CreateTime = reader.GetTuhuValue<DateTime>(10);
                    psp.StockOut = reader.GetTuhuValue<bool>(11);
                    productSalesPrices.Add(psp);
                }
            }

            return productSalesPrices;
        }

        public static void UpdateOrderProductInfoCache(SqlConnection cn, DataTable cacheOrderProductInfo)
        {
            if (cacheOrderProductInfo != null)
            {
                var sqlParams = new[]
                {
                    new SqlParameter("@CacheOrderProductInfo", cacheOrderProductInfo)
                };

                SqlHelper.ExecuteNonQueryV2(cn, CommandType.StoredProcedure, "Purchase_UpdateOrderProductInfoCache",
                    sqlParams);
            }
        }

        public static string SelectBrandByPID(SqlConnection cn, string PID)
        {
            string brand = string.Empty;

            var sqlParams = new[]
            {
                new SqlParameter("@PID", PID)
            };

            using (
                SqlDataReader reader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure,
                    "Product_SelectBrandByPID",
                    sqlParams))
            {
                if (reader.Read())
                {
                    brand = reader.GetTuhuString(0);
                }
            }

            return brand;
        }

        public static List<TireProductModel> GetTireProductByID(string ProductID, string VariantID)
        {
            string sSql = @"SELECT	CP.ProductID,
				CP.VariantID,(ProductID+'|'+VariantID) AS PID,
				C.DisplayName,
                C.CP_Brand Brand, C.CP_Tire_Width, C.CP_Tire_AspectRatio,  C.CP_Tire_Rim,
                ISNULL( C.CP_Tire_SpeedRating, 0) CP_Tire_SpeedRating, 
                ISNULL( CP.CP_Tire_Type, 0) CP_Tire_Type,c.Name,
                ISNULL(C.CP_Tire_LoadIndex,0) CP_Tire_LoadIndex, 
                (CASE   WHEN  CP_Tire_ROF='防爆' THEN '缺气防爆轮胎' ELSE '' end) ROF,
                CP.CP_Place,ISNULL(C.CP_Tire_Pattern,0) CP_Tire_Pattern,
                DefinitionName,
				CP.OnSale,
                CP.cy_list_price
                ,CP_OriginalSKU
                ,CP_Place
                ,CP_RateMile
                ,CP_RateNumber
                ,CP_RateSource
                ,CP_SKU
                ,cy_marketing_price
                ,OnSale
                ,Brand
                ,CP_SI_BuyAgain 
                ,CP_Wiper_OriginalNo
                ,CP_Wiper_SI_Joint
                ,CP_Wiper_SI_Silent
                ,CP_Wiper_SI_Wearable
                ,CP_Wiper_Size
                ,CP_Wiper_Stand,
				CP_Wiper_Series, 
				CP_Wiper_Baffler 
				,CP_Filter_Type,C.特别说明
                ,CP_Rank
                ,CP_Tire_SI_CorneringStability
                ,CP_Tire_SI_DeepSnowTraction
                ,CP_Tire_SI_DryTraction
                ,CP_Tire_SI_HydroplaningResistance
                ,CP_Tire_SI_IceTraction
                ,CP_Tire_SI_LightSnowTraction
                ,CP_Tire_SI_NoiseComfort
                ,CP_Tire_SI_RideComfort
                ,CP_Tire_SI_SteeringResponse
                ,CP_Tire_SI_Treadwear
                ,CP_Tire_SI_WetTraction
                ,CP_Tire_Type
                ,Variant_Image_filename_1
                ,Variant_Image_filename_2
                ,IntroductionDate
                ,IsScoreItem
                ,CP_Battery_Info
                ,CP_Battery_SI_Capacity
                ,CP_Battery_SI_ColdBoot
                ,CP_Battery_SI_Convenience
                ,CP_Battery_SI_Current
                ,CP_Battery_SI_Life
                ,CP_Battery_SI_Safety
                ,CP_Battery_Size
                ,CP_Brief_Auto
                ,CP_Brake_SI_BrakeForce
                ,CP_Brake_SI_Comfortable
                ,CP_Brake_SI_Life
                ,CP_Brake_SI_Noise
                ,CP_Brake_SI_Pollute,
                CP_Brake_Position,
                C.CP_Brake_Type,CP_Unit
                ,CP_Filter_RefNo
                ,Color
                ,Place
                ,Weight
                ,cy_cost
                ,gift
                ,invoice
                ,CP_Tire_ROF
                ,ProductRefer
                ,cy_tuan
                ,MonitorLevel
                ,cy_tb_price
                ,cy_hd_price
                ,isOE
                ,CP_Hub_CB
                ,CP_Hub_ET
                ,CP_Hub_H
                ,CP_Hub_PCD
                ,CP_Hub_Stand
                ,CP_Hub_Width
                ,CP_Vehicle
                ,VehicleMatchLevel2
                ,Variant_Image_filename_3
                ,Variant_Image_filename_4
                ,PartNo
                ,CP_ShuXing1
                ,CP_ShuXing2
                ,CP_ShuXing3
                ,CP_ShuXing4
                ,CP_ShuXing5
				,isnull(CP.stockout,0) as stockout
		 FROM	Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH (NOLOCK)
		 JOIN	Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] AS C WITH (NOLOCK)
				ON C.#Catalog_Lang_Oid = CP.oid
		 WHERE	CP.i_ClassType IN (2, 4) AND CP.OnSale=1 AND C.DisplayName IS NOT NULL  ";

            return DbHelper.ExecuteDataTable(sSql, CommandType.Text).ConvertTo<TireProductModel>().ToList();
        }

        public static List<Tuple<string, string, int>> SelectSkuProductsAndStockQuantity(SqlConnection conn,
            string productPID, string brand, string category)
        {
            List<Tuple<string, string, int>> result = new List<Tuple<string, string, int>>();
            SqlParameter[] parameters =
            {
                new SqlParameter("@PID", productPID),
                new SqlParameter("@Brand", brand),
                new SqlParameter("@Category", category)
            };

            using (
                var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure,
                    "Product_SelectSkuProductsAndStockQuantity", parameters))
            {
                while (reader.Read())
                {
                    string pid = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                    string displayName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    int stock = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                    Tuple<string, string, int> data = new Tuple<string, string, int>(pid, displayName, stock);
                    result.Add(data);
                }
            }

            return result;
        }

        public static String  GetNameById(SqlConnection connection,String id)
        {
            var dp = new DynamicParameters();
            dp.Add("@PID", id);
            return connection
                .Query<String>(
                    "SELECT DisplayName FROM Tuhu_productcatalog.dbo.vw_Products where PID=@PID",
                    dp)
                .FirstOrDefault();

            
            
        }
    }
}
