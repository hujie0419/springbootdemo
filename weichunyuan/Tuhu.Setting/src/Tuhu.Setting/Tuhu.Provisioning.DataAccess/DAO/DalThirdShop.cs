using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALThirdShop
    {
        #region 优惠券管理
        public static IEnumerable<TireCouponModel> TireCouponManage(string ShopName)
        {
            var SqlStr = @"SELECT	PKID ,
		ShopName ,
		QualifiedPrice ,
		Reduce ,
		StartTime ,
		Description ,
		CouponUseRule ,
		CouponType ,
		EndTime
FROM	Activity..ShopCouponManage WITH ( NOLOCK )
WHERE	IsDelete = 0
		AND EndTime > GETDATE()
		AND StartTime < GETDATE()
		AND ISNULL(@shopname, ShopName) = ShopName;";
            using (var helper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_ReadOnly")))
            {
                using (var cmd = new SqlCommand(SqlStr))
                {
                    cmd.Parameters.AddWithValue("@shopname", ShopName);
                    cmd.CommandType = CommandType.Text;
                    return helper.ExecuteDataTable(cmd).ConvertTo<TireCouponModel>();
                }
            }

        }


        public static int AddTireCoupon(TireCouponModel request)
        {
            var SqlStr = @"INSERT	INTO Activity..ShopCouponManage
		(	ShopName ,
			QualifiedPrice ,
			Reduce ,
			EndTime ,
			Description ,
			StartTime ,
			CouponUseRule ,
			CouponType
		)
OUTPUT	Inserted.PKID
VALUES	(	@shopname ,
			@qualifiefprice ,
			@reduce ,
			@endtime ,
			@description ,
			@starttime ,
			@couponuserule ,
			@coupontype
		);";
            using (var cmd = new SqlCommand(SqlStr))
            {
                cmd.Parameters.AddWithValue("@shopname", request.ShopName);
                cmd.Parameters.AddWithValue("@qualifiefprice", request.QualifiedPrice);
                cmd.Parameters.AddWithValue("@reduce", request.Reduce);
                cmd.Parameters.AddWithValue("@description", request.Description);
                cmd.Parameters.AddWithValue("@endtime", request.EndTime);
                cmd.Parameters.AddWithValue("@starttime", request.StartTime);
                cmd.Parameters.AddWithValue("@couponuserule", request.CouponUseRule);
                cmd.Parameters.AddWithValue("@coupontype", request.CouponType);
                cmd.CommandType = CommandType.Text;
                var dat = DbHelper.ExecuteScalar(cmd);
                if (Int32.TryParse(dat?.ToString(), out int value))
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
UPDATE	Activity..ShopCouponManage WITH ( ROWLOCK )
SET		IsDelete = 1
OUTPUT	Inserted.PKID ,
		Inserted.ShopName ,
		Inserted.QualifiedPrice ,
		Inserted.Reduce ,
		Inserted.EndTime
WHERE	PKID = @pkid;";
            using (var cmd = new SqlCommand(SqlStr))
            {
                cmd.Parameters.AddWithValue("@pkid", pkid);
                return DbHelper.ExecuteDataTable(cmd).ConvertTo<TireCouponModel>()?.FirstOrDefault() ?? new TireCouponModel();
            }
        }

        public static List<TireCouponLogModel> FetchCouponLogByShopName(string ShopName)
        {
            const string sqlStr = @"SELECT  ShopName ,
        CouponName ,
        CouponEndTime ,
        Operator ,
        OperateType ,
        CreateDateTime
FROM    Tuhu_log..TireCouponManageLog WITH ( NOLOCK )
WHERE   ShopName = @shopname
ORDER BY CreateDateTime DESC; ";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@shopname", ShopName);
                    return dbHelper.ExecuteDataTable(cmd).ConvertTo<TireCouponLogModel>()?.ToList() ?? new List<TireCouponLogModel>();
                }
            }
        }

        public static void AddTireCouponLog(string ShopName, int CouponType, string CouponName, DateTime CouponEndTime, string Operator, string OperatrType)
        {
            var SqlStr = @"INSERT INTO Tuhu_log..TireCouponManageLog (ShopName, CouponName, CouponEndTime, Operator, OperateType) VALUES (@shopName, @couponName, @CouponEndTime, @operator, @OperateType);";
            var TypeName = "A类优惠券";
            if (CouponType == 2)
            {
                TypeName = "B类优惠券";
            }
            if (CouponType == 3)
            {
                TypeName = "C类优惠券";
            }
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(SqlStr))
                {
                    cmd.Parameters.AddWithValue("@shopName", ShopName);
                    cmd.Parameters.AddWithValue("@couponName", $"{TypeName}/{CouponName}");
                    cmd.Parameters.AddWithValue("@CouponEndTime", CouponEndTime);
                    cmd.Parameters.AddWithValue("@operator", Operator);
                    cmd.Parameters.AddWithValue("@OperateType", OperatrType);
                    dbHelper.ExecuteNonQuery(cmd);
                }
            }
        }
        #endregion

        #region 第三方价格管理

        public static Dictionary<string, decimal> SelectProductCouponPrice(IEnumerable<string> pids)
        {
            if (pids != null && pids.Any())
            {
                string sql = @"
WITH PIDS AS (SELECT Item COLLATE Chinese_PRC_CI_AS AS PID FROM Tuhu_productcatalog..SplitString(@PIDS, ',', 1))
select * from  Tuhu_productcatalog..tbl_CouponPrice WITH ( NOLOCK) where pid in ( select PID from PIDS )";
                using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
                {
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@PIDS", string.Join(",", pids));
                        var result = dbHelper.ExecuteDataTable(cmd);
                        if (result != null && result.Rows.Count > 0)
                        {
                            var dict = new Dictionary<string, decimal>();
                            foreach (DataRow row in result.Rows)
                            {
                                string pid = row["PID"]?.ToString();
                                decimal temp;
                                if (decimal.TryParse(row["NewPrice"]?.ToString(), out temp))
                                {
                                    dict[pid] = temp;
                                }
                            }

                            return dict;
                        }

                    }
                }
            }

            return new Dictionary<string, decimal>();
        }


        public static List<ProductPriceModel> FetchProductList(ProductListRequest request, PagerModel pager, bool isExport)
        {
            #region SQL
            var SqlStr = @"SELECT	T.CP_Brand AS Brand ,
		T.PID AS PID ,
		T.DisplayName AS ProductName ,
		S.cost AS Cost ,
		P.LowestPrice AS LowestLimit ,
		T.cy_list_price AS Price ,
		S.taobao_tuhuid AS TBPID ,
		S.taobao_tuhuprice AS TBPrice ,
		S.taobao2_tuhuid AS TB2PID ,
		S.taobao2_tuhuprice AS TB2Price ,
		S.tianmao1_tuhuprice AS TM1Price ,
		S.tianmao1_tuhuid AS TM1PID ,
		S.tianmao2_tuhuprice AS TM2Price ,
		S.tianmao2_tuhuid AS TM2PID ,
		S.tianmao3_tuhuprice AS TM3Price ,
		S.tianmao3_tuhuid AS TM3PID ,
		S.tianmao4_tuhuid AS TM4PID ,
		S.tianmao4_tuhuprice AS TM4Price ,
		S.jingdongflagship_tuhuprice AS JDFlagShipPrice ,
		S.jingdongflagship_tuhuid AS JDFlagShipPID ,
		S.jingdong_tuhuprice AS JDPrice ,
		S.jingdong_tuhuid AS JDPID,
        e.CanUseCoupon as CanUseCoupon,
        s.num_threemonth,
        p.LowestPrice_Normal,
        p.LowestPrice_Promotion,
        p.CouponPrice_Normal,
        p.CouponPrice_Promotion
FROM	Tuhu_productcatalog..vw_Products AS T WITH ( NOLOCK )
		LEFT JOIN Tuhu_productcatalog..TireLowestPrice AS P WITH ( NOLOCK ) ON T.PID = P.PID
		LEFT JOIN Tuhu_bi..dm_Product_SalespredictData AS S WITH ( NOLOCK ) ON T.PID = S.PID
        left join Tuhu_productcatalog..tbl_ProductExtraProperties as e  WITH ( NOLOCK ) on e.pid=t.pid
WHERE	T.IsShow = 1
		AND ( ( T.PID LIKE '%' + @pid + '%'
				OR @pid IS NULL
				)
				AND T.PID LIKE 'TR-%'
			)
		AND ( T.DisplayName LIKE '%' + @productname + '%'
				OR @productname IS NULL
			)
		AND ( T.CP_Brand = @Brand
				OR @Brand IS NULL
			)
		AND ( @OnSaleStatus = 0
				OR @OnSaleStatus = 1
				AND T.OnSale = 1
				OR @OnSaleStatus = 2
				AND T.OnSale = 0
			)
		AND ( {0}
			)";
            SqlStr += isExport ? ";" : @"ORDER BY s.num_threemonth desc
		OFFSET @start ROWS FETCH NEXT @step ROWS ONLY;";
            #endregion+
            pager.TotalItem = FetchProductListCount(request);
            List<string> ManyCustomPrice = request.ManyCustomPrice?.Split('|').ToList();
            int Contrast = request.Contrast;
            string SingleCustomPrice = GetSqlParameter(request.SingleCustomPrice);
            double Proportion = request.Proportion;
            var sqlPara = "1 = 1";
            if (request.Type > 0 && ManyCustomPrice != null && !string.IsNullOrWhiteSpace(SingleCustomPrice) && Contrast != 0)
            {
                sqlPara = "1 <> 1";
                var ContrastSig = Contrast == -1 ? "<=" : ">=";
                foreach (var item in ManyCustomPrice)
                {
                    var realPara = GetSqlParameter(item);
                    if (!string.IsNullOrWhiteSpace(realPara))
                    {
                        sqlPara += $@" OR ( ISNULL({realPara}, 0) > 0
						AND {realPara} / IIF(ISNULL({SingleCustomPrice}, 0) = 0, 1, {SingleCustomPrice}) {ContrastSig} {Proportion})";
                    }
                }
            }
            //string.Format(SqlStr, sqlPara);
            SqlStr = string.Format(SqlStr, sqlPara);
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                using (var cmd = new SqlCommand(SqlStr))
                {
                    cmd.CommandTimeout = 3 * 60;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@pid", request.PID);
                    cmd.Parameters.AddWithValue("@productname", request.ProductName);
                    cmd.Parameters.AddWithValue("@OnSaleStatus", request.OnSale);
                    cmd.Parameters.AddWithValue("@Brand", request.Brand);
                    cmd.Parameters.AddWithValue("@start", (request.PageIndex - 1) * request.PageSize);
                    cmd.Parameters.AddWithValue("@step", request.PageSize);
                    var result = dbHelper.ExecuteDataTable(cmd);
                    return result.ConvertTo<ProductPriceModel>()?.ToList() ?? new List<ProductPriceModel>();
                }
            }
        }

        public static int FetchProductListCount(ProductListRequest request)
        {
            #region SQL
            var SqlStr = @"SELECT	COUNT(1)
FROM	Tuhu_productcatalog..vw_Products AS T WITH ( NOLOCK )
		LEFT JOIN Tuhu_productcatalog..TireLowestPrice AS P WITH ( NOLOCK ) ON T.PID = P.PID
		LEFT JOIN Tuhu_bi..dm_Product_SalespredictData AS S WITH ( NOLOCK ) ON T.PID = S.PID
WHERE	T.IsShow = 1
		AND ( ( T.PID LIKE '%' + @pid + '%'
				OR @pid IS NULL
				)
				AND T.PID LIKE 'TR-%'
			)
		AND ( T.DisplayName LIKE '%' + @productname + '%'
				OR @productname IS NULL
			)
		AND ( T.CP_Brand = @Brand
				OR @Brand IS NULL
			)
		AND ( @OnSaleStatus = 0
				OR @OnSaleStatus = 1
				AND T.OnSale = 1
				OR @OnSaleStatus = 2
				AND T.OnSale = 0
			)
		AND ( {0}
			);";
            #endregion

            List<string> ManyCustomPrice = request.ManyCustomPrice?.Split('|').ToList();
            int Contrast = request.Contrast;
            string SingleCustomPrice = GetSqlParameter(request.SingleCustomPrice);
            double Proportion = request.Proportion;
            var sqlPara = "1=1";
            if (request.Type > 0 && ManyCustomPrice != null && !string.IsNullOrWhiteSpace(SingleCustomPrice) && Contrast != 0)
            {
                sqlPara = "1 <> 1";
                var ContrastSig = Contrast == -1 ? "<=" : ">=";
                foreach (var item in ManyCustomPrice)
                {
                    var realPara = GetSqlParameter(item);
                    if (!string.IsNullOrWhiteSpace(realPara))
                    {
                        sqlPara += $@" OR ( ISNULL({realPara}, 0) > 0
						AND {realPara} / IIF(ISNULL({SingleCustomPrice}, 0) = 0, 1, {SingleCustomPrice}) {ContrastSig} {Proportion})";
                    }
                }
            }
            SqlStr = string.Format(SqlStr, sqlPara);
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_Gungnir_BI")))
            {
                using (var cmd = new SqlCommand(SqlStr))
                {
                    cmd.CommandTimeout = 3 * 60;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@pid", request.PID);
                    cmd.Parameters.AddWithValue("@productname", request.ProductName);
                    cmd.Parameters.AddWithValue("@OnSaleStatus", request.OnSale);
                    cmd.Parameters.AddWithValue("@Brand", request.Brand);
                    var result = dbHelper.ExecuteScalar(cmd);
                    if (Int32.TryParse(result?.ToString(), out int value))
                    {
                        return value;
                    }
                    return 0;
                }
            }
        }
        private static string GetSqlParameter(string OriginalPara)
        {
            var result = "";
            switch (OriginalPara)
            {
                case "list":
                    result = "T.cy_list_price";
                    break;
                case "cost":
                    result = "S.cost";
                    break;
                case "taobao1":
                    result = "S.taobao_tuhuprice";
                    break;
                case "taobao2":
                    result = "S.taobao2_tuhuprice";
                    break;
                case "tmall1":
                    result = "S.tianmao1_tuhuprice";
                    break;
                case "tmall2":
                    result = "S.tianmao2_tuhuprice";
                    break;
                case "tmall3":
                    result = "S.tianmao3_tuhuprice";
                    break;
                case "tmall4":
                    result = "S.tianmao4_tuhuprice";
                    break;
                case "tuhujd":
                    result = "S.jingdong_tuhuprice";
                    break;
                case "jingdongqj":
                    result = "S.jingdongflagship_tuhuprice";
                    break;
            }
            return result;
        }

        #endregion

        public static int SetLowestLimitPrice(string PID, decimal LowestLimitPrice, string type)
        {
            var SqlStr = $@"MERGE INTO Tuhu_productcatalog..TireLowestPrice AS T
USING
	( SELECT	@pid AS PID
	) AS S
ON T.PID = S.PID
WHEN MATCHED THEN
	UPDATE SET	T.{type} = @lowestprice
WHEN NOT MATCHED THEN
	INSERT	( PID, {type} )
	VALUES	( @pid, @lowestprice );";
            using (var cmd = new SqlCommand(SqlStr))
            {
                cmd.Parameters.AddWithValue("@pid", PID);
                cmd.Parameters.AddWithValue("@lowestprice", LowestLimitPrice);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
        public static List<LowestLimitLogModel> GetLowestLimitLog(string PID, string type)
        {
            var SqlStr = @"SELECT	PID ,
		OldPrice ,
		NewPrice ,
		Operator ,type,

		CreateDateTime
FROM	Tuhu_log..ProductLowestLimitLog WITH ( NOLOCK )
WHERE	PID = @pid and type=@type;";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(SqlStr))
                {
                    cmd.Parameters.AddWithValue("@pid", PID);
                    cmd.Parameters.AddWithValue("@type", type);
                    return dbHelper.ExecuteDataTable(cmd).ConvertTo<LowestLimitLogModel>()?.ToList() ?? new List<LowestLimitLogModel>();
                }
            }
        }
        public static int AddLowestLimitPriceLog(string PID, decimal? oldPrice, decimal newPrice, string Operator, string type)
        {
            var SqlStr = @"INSERT	INTO Tuhu_log..ProductLowestLimitLog
		(	PID ,
			OldPrice ,
			NewPrice ,
			Operator,
            Type
		)
VALUES	(	@pid ,
			@oldprice ,
			@newprice ,
			@operator,
            @Type
		);";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(SqlStr))
                {
                    cmd.Parameters.AddWithValue("@pid", PID);
                    cmd.Parameters.AddWithValue("@oldprice", oldPrice);
                    cmd.Parameters.AddWithValue("@newprice", newPrice);
                    cmd.Parameters.AddWithValue("@operator", Operator);
                    cmd.Parameters.AddWithValue("@type", type);
                    return dbHelper.ExecuteNonQuery(cmd);
                }
            }
        }
        public static int FetchPurchaseRestriction()
        {
            var SqlStr = @"SELECT	Value
FROM	[Configuration].[dbo].[ConfigApi] WITH ( NOLOCK ) WHERE [key]='PurchaseRestriction';";
            using (var cmd = new SqlCommand(SqlStr))
            {
                var data = DbHelper.ExecuteScalar(cmd);
                if (Int32.TryParse(data?.ToString(), out int value))
                {
                    return value;
                }
                return 0;
            }
        }
    }
}