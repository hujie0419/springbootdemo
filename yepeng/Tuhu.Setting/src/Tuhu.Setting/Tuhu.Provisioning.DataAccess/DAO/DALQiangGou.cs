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
    public class DALQiangGou
    {
        public static IEnumerable<QiangGouModel> SelectAllQiangGou()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  * FROM  Activity..tbl_FlashSale AS FS with(nolock) ORDER BY FS.CreateDateTime DESC").ConvertTo<QiangGouModel>();
            }
        }

        public static int DelFlashSale(Guid aid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                const string sql = @"Delete FROM  Activity..tbl_FlashSale WITH(ROWLOCK) where ActivityID='{0}'";
                var sql1 = string.Format(sql, aid);

                dbHelper.BeginTransaction();
                var result = dbHelper.ExecuteNonQuery(sql1);
                if (result > 0)
                {
                    const string sql2 = @"Delete FROM  Activity..tbl_FlashSaleProducts WITH(ROWLOCK) where ActivityID='{0}'";
                    var sql3 = string.Format(sql2, aid);
                    result = dbHelper.ExecuteNonQuery(sql3);
                    if (result > 0)
                    {
                        dbHelper.Commit();
                        return result;
                    }
                    else dbHelper.Rollback();
                }
                else dbHelper.Rollback();
                return result;
            }
        }

        public static IEnumerable<QiangGouProductModel> SelectQiangGouProductModels(Guid aid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                using (var cmd = new SqlCommand(@"select * from Activity..tbl_FlashSaleProducts WITH ( NOLOCK ) where ActivityID=@Aid"))
                {
                    cmd.Parameters.AddWithValue("@Aid", aid);
                    cmd.CommandType = CommandType.Text;
                    return dbHelper.ExecuteDataTable(cmd).ConvertTo<QiangGouProductModel>();
                }
            }
        }

        public static DataTable FetchQiangGouAndProducts(Guid aid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT	FS.ActiveType,
		FS.ActivityName,
		FS.StartDateTime,
		FS.EndDateTime,
		FS.PlaceQuantity,
        FS.IsNewUserFirstOrder,
        B.DisplayName,
        B.cy_list_price AS OriginalPrice,
		FSP.*
FROM	Activity..tbl_FlashSale AS FS WITH ( NOLOCK )
JOIN	Activity..tbl_FlashSaleProducts AS FSP WITH ( NOLOCK )
		ON FS.ActivityID = FSP.ActivityID
JOIN Tuhu_productcatalog..[CarPAR_zh-CN] B WITH ( NOLOCK ) ON FSP.PID = B.PID
WHERE	FS.ActivityID = @ActivityID  ORDER BY FSP.Position", CommandType.Text, new SqlParameter("@ActivityID", aid));
            }
        }
        public static DataTable FetchQiangGouAndProductsFromTemp(Guid aid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT	FS.ActiveType,
		FS.ActivityName,
		FS.StartDateTime,
		FS.EndDateTime,
		FS.PlaceQuantity,
        FS.IsNewUserFirstOrder,
        CPZCC.DisplayName,
        CPCP.cy_list_price AS OriginalPrice,
		FSP.*
FROM	Activity..tbl_FlashSale_Temp AS FS WITH ( NOLOCK )
JOIN	Activity..tbl_FlashSaleProducts_Temp AS FSP WITH ( NOLOCK )
		ON FS.ActivityID = FSP.ActivityID
JOIN    Tuhu_productcatalog..CarPAR_CatalogProducts AS CPCP ON FSP.PID = CPCP.ProductID
                                                              + '|'
                                                              + CPCP.VariantID
JOIN Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] AS CPZCC ON CPCP.oid = CPZCC.#Catalog_Lang_Oid
WHERE	FS.ActivityID = @ActivityID  ORDER BY FSP.Position", CommandType.Text, new SqlParameter("@ActivityID", aid));
            }
        }
        public static DataTable FetchNeedExamQiangGouAndProducts(Guid aid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT	FS.ActiveType,
		FS.ActivityName,
		FS.StartDateTime,
		FS.EndDateTime,
		FS.PlaceQuantity,
        FS.IsNewUserFirstOrder,
		VP.DisplayName,
		VP.cy_list_price AS OriginalPrice,
		FSP.*
FROM	Activity..tbl_FlashSale_Temp AS FS WITH ( NOLOCK )
JOIN	Activity..tbl_FlashSaleProducts_Temp AS FSP WITH ( NOLOCK )
		ON FS.ActivityID = FSP.ActivityID
JOIN	Tuhu_productcatalog..vw_Products AS VP
		ON FSP.PID  COLLATE Chinese_PRC_CI_AS = VP.PID 
WHERE	FS.ActivityID = @ActivityID ORDER BY FSP.PKID ;", CommandType.Text, new SqlParameter("@ActivityID", aid));
            }
        }

        public static List<ProductModel> SelectProductCostPriceByPids(List<string> pids)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_BI_ReadOnly")))
            {
                using (var cmd = new SqlCommand(@"	            
                        WITH    pids
                      AS ( SELECT   Item COLLATE Chinese_PRC_CI_AS AS PID
                           FROM     Tuhu_bi..SplitString(@Pids,
                                                              ',', 1)
                         )
                      SELECT  t.pid ,t.cost AS CostPrice
                        FROM    Tuhu_bi.dbo.dm_Product_SalespredictData AS t
                                WITH ( NOLOCK )
                        JOIN pids ON pids.PID = t.PID "))
                {
                    cmd.Parameters.AddWithValue("@Pids", string.Join(",", pids));
                    return dbHelper.ExecuteDataTable(cmd).ConvertTo<ProductModel>().ToList();
                }
            }
        }
        public static ProductModel FetchProductByPID(string pid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var dt = dbHelper.ExecuteDataTable("SELECT * FROM Tuhu_productcatalog.dbo.vw_Products AS VP WITH(NOLOCK) WHERE PID=@PID", CommandType.Text, new SqlParameter("@PID", pid));
                if (dt == null || dt.Rows.Count == 0)
                    return null;
                else
                    return dt.ConvertTo<ProductModel>().FirstOrDefault();
            }
        }
        public static int? SelectFlashSaleSaleOutQuantity(string activityid, string pid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            using (var cmd = new SqlCommand(@"SELECT SUM(Quantity) FROM tuhu_log..ActivityProductOrderRecords WITH ( NOLOCK) WHERE ActivityId=@ActivityId AND pid=@Pid AND orderStatus='0New' "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityId", activityid);
                cmd.Parameters.AddWithValue("@Pid", pid);
                return dbhelper.ExecuteScalar(cmd) as int?;
            }
        }

        public static IEnumerable<string> SelectPidsByParent(string productId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var dt = dbHelper.ExecuteDataTable(@"SELECT   PID
FROM    Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
WHERE   VP.ProductID = @ProductID AND VP.OnSale=1 AND VP.stockout=0
ORDER BY CASE WHEN ISNULL(VP.VariantID,'')='' then 0 ELSE  CONVERT(INT,VP.VariantID) END ", CommandType.Text, new SqlParameter("@ProductID", productId));
                List<string> list = new List<string>();
                if (dt != null && dt.Rows.Count > 0)
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(row[0].ToString());
                    }
                return list;
            }
        }



        public static DataTable SelectQiangGouToCache(Guid activityID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT FS.ActivityID,
        FS.ActivityName,
		FS.StartDateTime,
		FS.EndDateTime,
		FS.CreateDateTime,
		FS.UpdateDateTime,
		FS.Area,
		FS.BannerUrlAndroid,
		FS.BannerUrlIOS,
		FS.AppVlueAndroid,
		FS.AppVlueIOS,
		FS.BackgoundColor,
		FS.TomorrowText,
		FS.IsBannerIOS,
		FS.IsBannerAndroid,
		FS.ShowType,
		FS.ShippType,
		FS.IsTomorrowTextActive,
		FS.CountDown,
		FS.Status,
		FS.WebBanner,
		FS.WebCornerMark,
		FS.WebBackground,
		FS.IsNoActiveTime,
		FS.EndImage,
		FS.IsEndImage,
		FS.WebOtherPart,
		FS.ActiveType,
		FS.PCodeIDS,
		FS.ShoppingCart,
        FS.IsNewUserFirstOrder,
		FS.H5Url,
		FS.PlaceQuantity,
		FSP.PKID,
		FSP.PID,
		FSP.Position,
		FSP.Price,
		FSP.TotalQuantity,
		FSP.MaxQuantity,
		FSP.SaleOutQuantity,
		ISNULL(FSP.ProductName, VP.DisplayName) AS ProductName,
        ISNULL(VP.Image_filename, ISNULL(VP.Image_filename_2, ISNULL(VP.Image_filename_3, ISNULL(Image_filename_4, Image_filename_5)))) AS ProductImg,
        VP.TireSize,
		FSP.InstallAndPay,
		FSP.Level,
		FSP.ImgUrl,
		FSP.IsUsePCode,
		FSP.Channel,
		FSP.FalseOriginalPrice,
		FSP.IsJoinPlace,
        FSP.IsShow
FROM    Activity..tbl_FlashSale AS FS WITH(NOLOCK)
JOIN Activity..tbl_FlashSaleProducts AS FSP WITH (NOLOCK)
     ON FS.ActivityID = FSP.ActivityID AND FS.ActivityID = @ActivityID
JOIN  Tuhu_productcatalog..vw_Products AS VP WITH (NOLOCK) ON FSP.PID = VP.PID", CommandType.Text, new SqlParameter("@ActivityID", activityID));
            }
        }

        public static int ExamActivity(Guid actitvityID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var cmd = new SqlCommand("Activity..Activity_QiangGou_ExamActivity");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ActivityID", actitvityID);
                cmd.Parameters.Add(new SqlParameter()
                {
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output,
                    ParameterName = "@Result",
                });
                dbHelper.ExecuteNonQuery(cmd);
                return Convert.ToInt32(cmd.Parameters["@Result"].Value);

            }
        }

        public static int ExamUpdatePrice(QiangGouProductModel model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"UPDATE Activity..tbl_FlashSaleProducts_Temp WITH(ROWLOCK) SET Price=@Price WHERE ActivityID=@ActivityID AND PID=@PID", CommandType.Text,
                     new SqlParameter[] {
                        new SqlParameter("@Price",model.Price),
                        new SqlParameter("@PID",model.PID),
                        new SqlParameter("@ActivityID",model.ActivityID)
                     });
            }
        }

        public static IEnumerable<QiangGouModel> SelectAllNeedExamQiangGou()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  * FROM  Activity..tbl_FlashSale_Temp AS FS with(nolock) ORDER BY FS.CreateDateTime DESC").ConvertTo<QiangGouModel>();
            }
        }

        public static IEnumerable<QiangGouProductModel> CheckPIDSamePriceInOtherActivity(QiangGouProductModel product)
        {
            var dic = new Dictionary<string, string>();
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var cmd = new SqlCommand(@"SELECT	FS.ActivityID,FSP.Price,FSP.PID
FROM	Activity..tbl_FlashSale AS FS WITH ( NOLOCK )
JOIN	Activity..tbl_FlashSaleProducts AS FSP WITH ( NOLOCK )
		ON FSP.ActivityID = FS.ActivityID
WHERE	FS.EndDateTime > GETDATE()
		AND FSP.PID = @PID
		AND FSP.Price <> @Price
		AND FS.ActivityID<>@ActivityID
		AND ActiveType IN ( 0, 1 )");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PID", product.PID);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@ActivityID", product.ActivityID);
                var dt = dbHelper.ExecuteDataTable(cmd);
                if (dt == null || dt.Rows.Count == 0)
                    return null;


                return dt.ConvertTo<QiangGouProductModel>();
            }
        }

        /// <summary>
        /// 保存活动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //public static int Save(QiangGouModel model)
        //{
        //    using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
        //    {
        //        dbHelper.BeginTransaction();
        //        var syncResult = SynchroDiffActivity(model, dbHelper);
        //        if (syncResult < 0)
        //            return syncResult;
        //        var result = CreateOrUpdateQianggou(dbHelper, model);
        //        dbHelper.Commit();
        //        return result;
        //    }
        //}


        public static QiangGouModel SelectQiangGouForSynchro(Guid aid, SqlDbHelper dbHelper)
        {
            var dt = dbHelper.ExecuteDataTable(@" SELECT   *
               FROM     Activity.dbo.tbl_FlashSale AS FS WITH ( NOLOCK )
                        JOIN Activity.dbo.tbl_FlashSaleProducts AS FSP WITH ( NOLOCK ) ON FSP.ActivityID = FS.ActivityID AND FSP.ActivityID=@ActivityID;", CommandType.Text, new SqlParameter("@ActivityID", aid));
            var model = dt.ConvertTo<QiangGouModel>().FirstOrDefault();
            model.Products = dt.ConvertTo<QiangGouProductModel>();
            return model;
        }
        public static Dictionary<string, IEnumerable<QiangGouProductModel>> SelectDiffActivityProducts(Guid? aid, int atype, IEnumerable<QiangGouProductModel> products)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                return SelectDiffActivityProducts(aid, atype, products, dbHelper);
            }
        }
        public static Guid SelectLastActivityId()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                using (var cmd = new SqlCommand(@"Select Top 1 ActivityID from Activity.dbo.tbl_FlashSale WITH ( NOLOCK ) order by CreateDateTime desc "))
                {
                    cmd.CommandType = CommandType.Text;
                    return (Guid)dbHelper.ExecuteScalar(cmd);
                }
            }
        }
        public static Dictionary<string, IEnumerable<QiangGouProductModel>> SelectDiffActivityProducts(Guid? aid, int atype, IEnumerable<QiangGouProductModel> products, SqlDbHelper dbHelper)
        {
            Dictionary<string, IEnumerable<QiangGouProductModel>> dic = new Dictionary<string, IEnumerable<QiangGouProductModel>>();
            //return dic;
            var list = dbHelper.ExecuteDataTable(@"SELECT  FS.ActivityID ,FS.ActivityName,
                ISNULL(FSP.ProductName, VP.DisplayName) AS ProductName ,
                FSP.Price ,
                FSP.FalseOriginalPrice ,
                FSP.InstallAndPay ,
                FSP.IsUsePCode,
                FSP.PID
        FROM    Activity.dbo.tbl_FlashSale AS FS WITH ( NOLOCK )
                JOIN Activity.dbo.tbl_FlashSaleProducts AS FSP WITH ( NOLOCK ) ON FSP.ActivityID = FS.ActivityID
                JOIN Tuhu_productcatalog.dbo.vw_Products  AS VP WITH ( NOLOCK ) ON VP.PID = FSP.PID COLLATE Chinese_PRC_CI_AS
        WHERE   FS.EndDateTime > GETDATE()
                AND FS.ActiveType =@ActivityType and FS.ActiveType !=4
                AND FSP.PID COLLATE Chinese_PRC_CI_AS IN ( SELECT *
                             FROM   Gungnir..Split(@pids, ';') );", CommandType.Text, new SqlParameter[] {
                                                     new SqlParameter("@pids", string.Join(";", products.Select(_ => _.PID))),
                                                     new SqlParameter("@ActivityType",atype)
            }).ConvertTo<QiangGouProductModel>();

            if (aid != null)
                list = list.Where(_ => _.ActivityID != aid);
            if (list.Any())
            {
                foreach (var item in products)
                {
                    var diff = list.Where(_ => _.PID == item.PID &&
                    (
                    _.ProductName != item.ProductName ||
                    _.Price != item.Price ||
                    _.FalseOriginalPrice != item.FalseOriginalPrice ||

                    (_.InstallAndPay != item.InstallAndPay && !(string.IsNullOrWhiteSpace(_.InstallAndPay) && string.IsNullOrWhiteSpace(item.InstallAndPay))) ||
                    _.IsUsePCode != item.IsUsePCode));
                    if (diff.Any())
                        dic.Add(item.PID, diff);
                }
            }
            return dic;

        }
        public static IEnumerable<QiangGouProductModel> SelectSelectedDiffActivityProducts(Guid? aid, int atype, IEnumerable<QiangGouProductModel> products, SqlDbHelper dbHelper)
        {
            var list = dbHelper.ExecuteDataTable(@"SELECT  FS.ActivityID ,
                ISNULL(FSP.ProductName, VP.DisplayName) AS ProductName ,
                FSP.Price ,
                FSP.FalseOriginalPrice ,
                FSP.InstallAndPay ,
                FSP.IsUsePCode,
                FSP.PID
        FROM    Activity.dbo.tbl_FlashSale AS FS WITH ( NOLOCK )
                JOIN Activity.dbo.tbl_FlashSaleProducts AS FSP WITH ( NOLOCK ) ON FSP.ActivityID = FS.ActivityID
                JOIN Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK ) ON VP.PID = FSP.PID COLLATE Chinese_PRC_CI_AS
        WHERE   FS.EndDateTime > GETDATE()
                AND FS.ActiveType =@ActivityType
                AND FS.ActivityID =@ActivityID
                AND FSP.PID COLLATE Chinese_PRC_CI_AS IN ( SELECT *
                             FROM   Gungnir..Split(@pids, ';') );", CommandType.Text, new SqlParameter[] {
                                                     new SqlParameter("@pids", string.Join(";", products.Select(_ => _.PID))),
                                                     new SqlParameter("@ActivityType",atype),
                                                     new SqlParameter("@ActivityID",aid)
            }).ConvertTo<QiangGouProductModel>();

            return list;

        }
        public static Tuple<int, Guid> CreateOrUpdateQianggou(SqlDbHelper dbHelper, QiangGouModel model)
        {
            Tuple<int, Guid> resultWithAct;
            var result = -99;
            //创建活动
            if (model.ActivityID == null)
            {
                var cmd = new SqlCommand("Activity..Activity_QiangGou_CreateActivity");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ActivityName", model.ActivityName);
                cmd.Parameters.AddWithValue("@StartDateTime", model.StartDateTime);
                cmd.Parameters.AddWithValue("@EndDateTime", model.EndDateTime);
                cmd.Parameters.AddWithValue("@ActiveType", model.ActiveType);
                cmd.Parameters.AddWithValue("@PlaceQuantity", model.PlaceQuantity);
                cmd.Parameters.AddWithValue("@NeedExam", model.NeedExam);
                cmd.Parameters.AddWithValue("@IsDefault", model.IsDefault);
                cmd.Parameters.AddWithValue("@IsNewUserFirstOrder", model.IsNewUserFirstOrder);
                cmd.Parameters.Add(new SqlParameter()
                {
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output,
                    ParameterName = "@Result",
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    DbType = DbType.Guid,
                    Direction = ParameterDirection.Output,
                    ParameterName = "@ActivityID",
                });
                dbHelper.ExecuteNonQuery(cmd);
                result = Convert.ToInt32(cmd.Parameters["@Result"].Value);
                model.ActivityID = Guid.Parse(cmd.Parameters["@ActivityID"].Value.ToString());
                if (model.ActivityID != null && result >= 0)
                    result = CraateOrUpdateQiangGouProducts(dbHelper, model, false);
                resultWithAct = new Tuple<int, Guid>(result, model.ActivityID.Value);

            }
            else//修改活动
            {
                var cmd = new SqlCommand("Activity..Activity_QiangGou_UpdateActivity");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ActivityName", model.ActivityName);
                cmd.Parameters.AddWithValue("@ActivityID", model.ActivityID);
                cmd.Parameters.AddWithValue("@StartDateTime", model.StartDateTime);
                cmd.Parameters.AddWithValue("@EndDateTime", model.EndDateTime);
                cmd.Parameters.AddWithValue("@ActiveType", model.ActiveType);
                cmd.Parameters.AddWithValue("@PlaceQuantity", model.PlaceQuantity);
                cmd.Parameters.AddWithValue("@NeedExam", model.NeedExam);
                cmd.Parameters.AddWithValue("@IsDefault", model.IsDefault);
                cmd.Parameters.AddWithValue("@IsNewUserFirstOrder", model.IsNewUserFirstOrder);
                cmd.Parameters.AddWithValue("@PIDS", string.Join(",", model.Products.Select(p => p.PID)));
                cmd.Parameters.Add(new SqlParameter()
                {
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output,
                    ParameterName = "@Result",
                });

                dbHelper.ExecuteNonQuery(cmd);
                result = Convert.ToInt32(cmd.Parameters["@Result"].Value);
                if (result > 0)
                    result = CraateOrUpdateQiangGouProducts(dbHelper, model, true);
                resultWithAct = new Tuple<int, Guid>(result, model.ActivityID.Value);
            }
            return resultWithAct;
        }

        public static int CraateOrUpdateQiangGouProducts(SqlDbHelper dbHelper, QiangGouModel model, bool isUpdate)
        {
            var result = -99;
            string sql;
            if (model.NeedExam)
            {
                sql = @"	  INSERT	INTO Activity..tbl_FlashSaleProducts_Temp
                (
                  ActivityID,
                  PID,
                  Position,
                  Price,
                  Label,
                  TotalQuantity,
                  MaxQuantity,
                  SaleOutQuantity,
                  ProductName,
                  InstallAndPay,
                  IsUsePCode,
                  Channel,
                  FalseOriginalPrice,
                  IsJoinPlace,
                  IsShow,
                  InstallService)

      VALUES(
                  @ActivityID,
                  @PID,
                  @Position,
                  @Price,
                  @Label,
                  @TotalQuantity,
                  @MaxQuantity,
                  @SaleOutQuantity,
                  @ProductName,
                  @InstallAndPay,
                  @IsUsePCode,
                  @Channel,
                  @FalseOriginalPrice,
                  @IsJoinPlace,
                  @IsShow,
                  @InstallService); ";
            }
            else
            {
                sql = @" MERGE INTO Activity..tbl_FlashSaleProducts AS T
                       USING
                        ( SELECT    @Pid AS pid ,
                                    @ActivityID AS activityID
                        ) AS S
                       ON T.PID = S.pid
                        AND S.activityID = T.ActivityID
                       WHEN MATCHED THEN
                        UPDATE SET Price = @Price ,
                                   Position = @Position ,
                                   Label = @Label ,
                                   TotalQuantity = @TotalQuantity ,
                                   MaxQuantity = @MaxQuantity ,
                                   ProductName = @ProductName ,
                                   InstallAndPay = @InstallAndPay ,
                                   IsUsePCode = @IsUsePCode ,
                                   Channel = @Channel ,
                                   FalseOriginalPrice = @FalseOriginalPrice ,
                                   IsJoinPlace = @IsJoinPlace ,
                                   IsShow = @IsShow ,
                                   InstallService = @InstallService,
                                   SaleOutQuantity = @SaleOutQuantity,
                                   LastUpdateDateTime=GetDate()
                       WHEN NOT MATCHED THEN
                        INSERT ( ActivityID ,
                                 PID ,
                                 Position ,
                                 Price ,
                                 Label ,
                                 TotalQuantity ,
                                 MaxQuantity ,
                                 ProductName ,
                                 InstallAndPay ,
                                 IsUsePCode ,
                                 Channel ,
                                 FalseOriginalPrice ,
                                 IsJoinPlace ,
                                 IsShow ,
                                 InstallService,
                                 SaleOutQuantity
                               )
                        VALUES ( @ActivityID ,
                                 @PID ,
                                 @Position ,
                                 @Price ,
                                 @Label ,
                                 @TotalQuantity ,
                                 @MaxQuantity ,
                                 @ProductName ,
                                 @InstallAndPay ,
                                 @IsUsePCode ,
                                 @Channel ,
                                 @FalseOriginalPrice ,
                                 @IsJoinPlace ,
                                 @IsShow ,
                                 @InstallService,
                                 @SaleOutQuantity
                               );";
            }
            foreach (var item in model.Products)
            {

                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@ActivityID", model.ActivityID);
                    cmd.Parameters.AddWithValue("@PID", item.PID);
                    cmd.Parameters.AddWithValue("@Position", item.Position);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@Label", string.IsNullOrWhiteSpace(item.Label) ? null : item.Label);
                    cmd.Parameters.AddWithValue("@TotalQuantity", item.TotalQuantity);
                    cmd.Parameters.AddWithValue("@MaxQuantity", item.MaxQuantity);
                    cmd.Parameters.AddWithValue("@ProductName", item.ProductName);
                    cmd.Parameters.AddWithValue("@InstallAndPay", string.IsNullOrWhiteSpace(item.InstallAndPay) ? null : item.InstallAndPay);
                    cmd.Parameters.AddWithValue("@IsUsePCode", item.IsUsePCode);
                    cmd.Parameters.AddWithValue("@Channel", item.Channel);
                    cmd.Parameters.AddWithValue("@FalseOriginalPrice", item.FalseOriginalPrice);
                    cmd.Parameters.AddWithValue("@IsJoinPlace", item.IsJoinPlace);
                    cmd.Parameters.AddWithValue("@IsShow", item.IsShow);
                    cmd.Parameters.AddWithValue("@InstallService", item.InstallService);
                    cmd.Parameters.AddWithValue("@SaleOutQuantity", item.SaleOutQuantity);
                    result = dbHelper.ExecuteNonQuery(cmd);
                    if (result <= 0)
                        dbHelper.Rollback();
                }
            }
            return result;
        }
    }
}
