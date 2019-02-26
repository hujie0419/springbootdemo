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
using Tuhu.Provisioning.DataAccess.Tools;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALQiangGou
    {
        public static IEnumerable<QiangGouModel> SelectAllQiangGou()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  * FROM  Activity..tbl_FlashSale AS FS with(nolock) ORDER BY FS.EndDateTime DESC").ConvertTo<QiangGouModel>();
            }
        }

        /// <summary>
        /// 查询商品pid被配置的活动
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static IEnumerable<QiangGouProductModel> SelectQiangGouProductModelsByPid(string pid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                string sql = @"
                                SELECT  [PKID] ,
                                        [ActivityID] ,
                                        [PID] ,
                                        [Position] ,
                                        [Price] ,
                                        [Label] ,
                                        [TotalQuantity] ,
                                        [MaxQuantity] ,
                                        [SaleOutQuantity] ,
                                        [CreateDateTime] ,
                                        [LastUpdateDateTime] ,
                                        [ProductName] ,
                                        [InstallAndPay] ,
                                        [Level] ,
                                        [ImgUrl] ,
                                        [IsUsePCode] ,
                                        [Channel] ,
                                        [FalseOriginalPrice] ,
                                        [IsJoinPlace] ,
                                        [IsShow] ,
                                        [InstallService]
                                FROM    Activity.[dbo].[tbl_FlashSaleProducts]
                                WHERE   PID = @PID";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PID", pid);
                    cmd.CommandType = CommandType.Text;
                    return dbHelper.ExecuteDataTable(cmd).ConvertTo<QiangGouProductModel>();
                }
            }
        }

        public static int DelFlashSale(Guid aid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                //删除活动
                const string sql = @"Delete FROM  Activity..tbl_FlashSale WITH(ROWLOCK) where ActivityID='{0}'";
                var sql1 = string.Format(sql, aid);

                dbHelper.BeginTransaction();
                var result = dbHelper.ExecuteNonQuery(sql1);
                if (result > 0)
                {
                    //删除商品
                    const string sql2 = @"Delete FROM  Activity..tbl_FlashSaleProducts WITH(ROWLOCK) where ActivityID='{0}'";
                    var sql3 = string.Format(sql2, aid);
                    dbHelper.ExecuteNonQuery(sql3);
                    dbHelper.Commit();
                    return result;
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
JOIN    Tuhu_productcatalog..CarPAR_CatalogProducts AS CPCP ON FSP.PID = CPCP.Pid2
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
        /// <summary>
        /// 查询orderStatus='0New'的数据总数
        /// </summary>
        /// <param name="activityid"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
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

        //// <summary>
        //// 保存活动
        //// </summary>
        //// <param name="model"></param>
        //// <returns></returns>
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

        /// <summary>
        /// 根据ID查询商品信息
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        public static QiangGouModel SelectQiangGouForSynchro(Guid aid, SqlDbHelper dbHelper)
        {
            var dt = dbHelper.ExecuteDataTable(@" SELECT   *
               FROM     Activity.dbo.tbl_FlashSale AS FS WITH ( NOLOCK )
                        JOIN Activity.dbo.tbl_FlashSaleProducts AS FSP WITH ( NOLOCK ) ON FSP.ActivityID = FS.ActivityID AND FSP.ActivityID=@ActivityID;", CommandType.Text, new SqlParameter("@ActivityID", aid));
            var model = dt.ConvertTo<QiangGouModel>().FirstOrDefault();
            model.Products = dt.ConvertTo<QiangGouProductModel>();
            return model;
        }
        public static Dictionary<string, IEnumerable<QiangGouProductModel>> SelectDiffActivityProducts(Guid? aid, int atype, IEnumerable<QiangGouProductModel> products, DateTime StartDateTime, DateTime EndDateTime)
        {
            Dictionary<string, IEnumerable<QiangGouProductModel>> keyValuePairs;

            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                if (atype == 3)
                {
                    keyValuePairs = SelectDiffActivitySpikeProducts(aid, atype, products, dbHelper, StartDateTime, EndDateTime);
                }
                else
                {
                    keyValuePairs = SelectDiffActivityProducts(aid, atype, products, dbHelper);
                }
            }

            return keyValuePairs;
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

        /// <summary>
        /// 根据当前保存的PID集合，查询当前活动类型是否已经配置过对应产品 (已有逻辑活动页秒杀时不调用走新逻辑)
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="atype"></param>
        /// <param name="products"></param>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
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
                list = list.Where(_ => _.ActivityID != aid); //过滤当前活动ID的，修改时使用


            if (list.Any())
            {
                //循环遍历传入的产品集合，与已经配置的商品校验
                foreach (var item in products)
                {
                    //校验条件、PID、产品名称、促销价格、伪原价、或 支付方式 是否使用 优惠券 配置相同情况时，给出提示不允许配置
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

        /// <summary>
        /// 活动页秒杀配置验证,重叠时间的PID不可重复配置
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="atype"></param>
        /// <param name="products"></param>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        public static Dictionary<string, IEnumerable<QiangGouProductModel>> SelectDiffActivitySpikeProducts(Guid? aid, int atype, IEnumerable<QiangGouProductModel> products, SqlDbHelper dbHelper, DateTime StartDateTime, DateTime EndDateTime)
        {
            Dictionary<string, IEnumerable<QiangGouProductModel>> dic = new Dictionary<string, IEnumerable<QiangGouProductModel>>();
            //return dic;
            var list = dbHelper.ExecuteDataTable(@"SELECT  FS.ActivityID ,FS.ActivityName,FS.StartDateTime,FS.EndDateTime,
                ISNULL(FSP.ProductName, VP.DisplayName) AS ProductName ,
                FSP.Price ,
                FSP.FalseOriginalPrice ,
                FSP.InstallAndPay ,
                FSP.IsUsePCode,
                FSP.PID
        FROM    Activity.dbo.tbl_FlashSale AS FS WITH ( NOLOCK )
                JOIN Activity.dbo.tbl_FlashSaleProducts AS FSP WITH ( NOLOCK ) 
				ON FSP.ActivityID = FS.ActivityID
                JOIN Tuhu_productcatalog.dbo.vw_Products  AS VP WITH ( NOLOCK ) 
				ON VP.PID = FSP.PID COLLATE Chinese_PRC_CI_AS
        WHERE  FS.EndDateTime > GETDATE() AND  (( @StartDateTime >= FS.StartDateTime
								AND @StartDateTime < FS.EndDateTime )
							  OR (
								   @EndDateTime > FS.StartDateTime
								   AND @EndDateTime <= FS.EndDateTime )
							  OR (
								   @StartDateTime <= FS.StartDateTime   
								   AND @EndDateTime >= FS.EndDateTime ) )
                AND FS.ActiveType = @ActivityType
                AND FSP.PID COLLATE Chinese_PRC_CI_AS IN ( SELECT *
                             FROM   Gungnir..Split(@pids, ';') );", CommandType.Text, new SqlParameter[] {
                                                     new SqlParameter("@pids", string.Join(";", products.Select(_ => _.PID))),
                                                     new SqlParameter("@ActivityType",atype),
                                                      new SqlParameter("@StartDateTime",StartDateTime),
                                                       new SqlParameter("@EndDateTime",EndDateTime)
            }).ConvertTo<QiangGouProductModel>();

            if (aid != null)
                list = list.Where(_ => _.ActivityID != aid); //过滤当前活动ID的，修改时使用


            if (list.Any())
            {
                //循环遍历传入的产品集合，与已经配置的商品校验
                foreach (var item in products)
                {
                    var diff = list.Where(_ => _.PID == item.PID);
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
                    result = CreateOrUpdateQiangGouProducts(dbHelper, model, false);
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
                    result = CreateOrUpdateQiangGouProducts(dbHelper, model, true);
                resultWithAct = new Tuple<int, Guid>(result, model.ActivityID.Value);
            }
            return resultWithAct;
        }

        public static int CreateOrUpdateQiangGouProducts(SqlDbHelper dbHelper, QiangGouModel model, bool isUpdate)
        {
            var result = -99;
            string sql;
            if (model.NeedExam)
            {
                #region sql
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
                #endregion
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

        public static IEnumerable<ActivityPageContentModel> FetchActivityPageContentByActivityId(string activityId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                using (var cmd = new SqlCommand(@"  SELECT
                                                              apc.PKID ,
                                                              apc.Brand ,
                                                              apc.ProductType ,
                                                              apl.HashKey
                                                             FROM
                                                              Configuration..ActivePageContent
                                                              AS apc WITH ( NOLOCK )
                                                              JOIN Configuration..ActivePageList
                                                              AS apl WITH ( NOLOCK ) ON apc.FKActiveID = apl.PKID  where apc.ActivityID=@Aid AND apc.Type=30"))
                {
                    cmd.Parameters.AddWithValue("@Aid", activityId);
                    cmd.CommandType = CommandType.Text;
                    return dbHelper.ExecuteDataTable(cmd).ConvertTo<ActivityPageContentModel>();
                }
            }
        }

        /// <summary>
        /// 创建或修改抢购活动
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Tuple<int, Guid> CreateOrUpdateQiangGou(SqlDbHelper dbHelper, QiangGouModel model)
        {
            Tuple<int, Guid> resultWithAct;
            var result = -99;

            var procName = model.ActivityID == null ? "Activity..Activity_QiangGou_CreateActivity" : "Activity..Activity_QiangGou_UpdateActivity";
            var cmd = new SqlCommand(procName)
            {
                CommandType = CommandType.StoredProcedure
            };
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
            //创建活动
            if (model.ActivityID == null)
            {
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
                    result = CreateORUpdateQiangGouProducts(dbHelper, model, false);
                resultWithAct = new Tuple<int, Guid>(result, model.ActivityID.Value);

            }
            else//修改活动
            {
                cmd.Parameters.AddWithValue("@ActivityID", model.ActivityID);
                cmd.Parameters.AddWithValue("@PIDS", string.Join(",", model.Products.Select(p => p.PID)));

                dbHelper.ExecuteNonQuery(cmd);
                result = Convert.ToInt32(cmd.Parameters["@Result"].Value);
                if (result > 0)
                    result = CreateORUpdateQiangGouProducts(dbHelper, model, true);
                resultWithAct = new Tuple<int, Guid>(result, model.ActivityID.Value);
            }

            return resultWithAct;
        }

        /// <summary>
        /// 创建或更新抢购活动的产品数据
        /// </summary>
        /// <param name="db"></param>
        /// <param name="model"></param>
        /// <param name="isUpdate"></param>
        /// <returns></returns>
        public static int CreateORUpdateQiangGouProducts(SqlDbHelper db, QiangGouModel model, bool isUpdate)
        {
            var result = -99;
            string sql;
            if (model.NeedExam)
            {
                /***
                 *  之前的SQL语句,逻辑有问题，重新写语句
                 * INSERT  INTO Activity..tbl_FlashSaleProducts_Temp
(ActivityID ,PID ,Position ,Price ,Label ,TotalQuantity ,MaxQuantity ,SaleOutQuantity ,ProductName ,InstallAndPay ,
IsUsePCode ,Channel ,FalseOriginalPrice ,IsJoinPlace ,IsShow ,InstallService)
                        SELECT  ActivityID ,
PID ,Position ,Price ,Label ,TotalQuantity ,MaxQuantity ,SaleOutQuantity ,ProductName ,InstallAndPay ,IsUsePCode ,Channel ,FalseOriginalPrice ,IsJoinPlace ,IsShow ,InstallService
                        FROM    Activity..tbl_FlashSaleProducts
                        WHERE   ActivityID = @ActivityID;
                 **/
                sql = @"INSERT  INTO Activity..tbl_FlashSaleProducts_Temp
                                (ActivityID ,PID ,Position ,Price ,Label ,TotalQuantity ,MaxQuantity ,SaleOutQuantity ,ProductName ,InstallAndPay ,
                                IsUsePCode ,Channel ,FalseOriginalPrice ,IsJoinPlace ,IsShow ,InstallService)
                                VALUES(
                                @ActivityID,
                                @PID,
                                @Position ,
                                @Price,
                                @Label ,
                                @TotalQuantity ,
                                @MaxQuantity ,
                                @SaleOutQuantity ,
                                @ProductName ,
                                @InstallAndPay,
                                @IsUsePCode ,
                                @Channel ,
                                @FalseOriginalPrice,
                                @IsJoinPlace ,
                                @IsShow ,
                                @InstallService
                                );";
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
                //foreach (var item in model.Products)
                //{

                //    using (var cmd = new SqlCommand(sql))
                //    {
                //        cmd.Parameters.AddWithValue("@ActivityID", model.ActivityID);
                //        cmd.Parameters.AddWithValue("@PID", item.PID);
                //        cmd.Parameters.AddWithValue("@Position", item.Position);
                //        cmd.Parameters.AddWithValue("@Price", item.Price);
                //        cmd.Parameters.AddWithValue("@Label", string.IsNullOrWhiteSpace(item.Label) ? null : item.Label);
                //        cmd.Parameters.AddWithValue("@TotalQuantity", item.TotalQuantity);
                //        cmd.Parameters.AddWithValue("@MaxQuantity", item.MaxQuantity);
                //        cmd.Parameters.AddWithValue("@ProductName", item.ProductName);
                //        cmd.Parameters.AddWithValue("@InstallAndPay", string.IsNullOrWhiteSpace(item.InstallAndPay) ? null : item.InstallAndPay);
                //        cmd.Parameters.AddWithValue("@IsUsePCode", item.IsUsePCode);
                //        cmd.Parameters.AddWithValue("@Channel", item.Channel);
                //        cmd.Parameters.AddWithValue("@FalseOriginalPrice", item.FalseOriginalPrice);
                //        cmd.Parameters.AddWithValue("@IsJoinPlace", item.IsJoinPlace);
                //        cmd.Parameters.AddWithValue("@IsShow", item.IsShow);
                //        cmd.Parameters.AddWithValue("@InstallService", item.InstallService);
                //        cmd.Parameters.AddWithValue("@SaleOutQuantity", item.SaleOutQuantity);
                //        result = db.ExecuteNonQuery(cmd);
                //        if (result <= 0)
                //            db.Rollback();
                //    }
                //}
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
                    result = db.ExecuteNonQuery(cmd);
                    if (result <= 0)
                        db.Rollback();
                }
            }
            return result;
        }

        /// <summary>
        /// 活动配置刷新缓存按钮时使用
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        public static QiangGouModel SelectFlashSaleInfo(Guid aid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var dt = dbHelper.ExecuteDataTable(@"SELECT  ActivityID ,
                                ActivityName ,
                                StartDateTime ,
                                EndDateTime ,
                                ActiveType ,
                                PlaceQuantity
                        FROM    Activity.dbo.tbl_FlashSale
						 WITH ( NOLOCK )  WHERE
						  ActivityID= @ActivityID;", CommandType.Text, new SqlParameter("@ActivityID", aid));
                var model = dt.ConvertTo<QiangGouModel>().FirstOrDefault();
                return model;
            }
        }

        #region 活动删除数据备份

        /// <summary>
        /// 把删除的活动信息插入到活动删除备份表
        /// </summary>
        /// <param name="deletedModel"></param>
        /// <returns></returns>
        public static int InsertDeletedActivityInfo(QiangGouModel deletedModel)
        {
            string sqlInsert = @" INSERT INTO [Activity].[dbo].[tbl_FlashSaleLog]
                                           ([ActivityID]
                                           ,[ActivityName]
                                           ,[StartDateTime]
                                           ,[EndDateTime]
                                           ,[CreateDateTime]
                                           ,[UpdateDateTime]
                                           ,[ActiveType]
                                           ,[PlaceQuantity]
                                           ,[IsNewUserFirstOrder]
                                           ,[IsDefault]
                                           ,[DeletedTime])
                                     VALUES
                                           (@ActivityID
                                           ,@ActivityName
                                           ,@StartDateTime
                                           ,@EndDateTime
                                           ,@CreateDateTime
                                           ,@UpdateDateTime
                                           ,@ActiveType
                                           ,@PlaceQuantity
                                           ,@IsNewUserFirstOrder
                                           ,@IsDefault
                                           ,GETDATE()) ";

            using (var cmd = new SqlCommand(sqlInsert))
            {
                cmd.Parameters.AddWithValue("@ActivityID", deletedModel.ActivityID);
                cmd.Parameters.AddWithValue("@ActivityName", deletedModel.ActivityName);
                cmd.Parameters.AddWithValue("@StartDateTime", deletedModel.StartDateTime);
                cmd.Parameters.AddWithValue("@EndDateTime", deletedModel.EndDateTime);
                cmd.Parameters.AddWithValue("@CreateDateTime", deletedModel.CreateDateTime ?? DateTime.Now);
                cmd.Parameters.AddWithValue("@UpdateDateTime", deletedModel.UpdateDateTime ?? DateTime.Now);
                cmd.Parameters.AddWithValue("@ActiveType", deletedModel.ActiveType);
                cmd.Parameters.AddWithValue("@PlaceQuantity", deletedModel.PlaceQuantity);
                cmd.Parameters.AddWithValue("@IsNewUserFirstOrder", deletedModel.IsNewUserFirstOrder);
                cmd.Parameters.AddWithValue("@IsDefault", deletedModel.IsDefault);
                var result = DbHelper.ExecuteNonQuery(cmd);
                return result;
            }
        }

        /// <summary>
        /// 把删除的活动信商品信息插入到活动商品删除备份表
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public static int InsertDeletedActivityProducts(List<QiangGouProductModel> products)
        {
            var result = 0;
            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        using (var cmd = new SqlCommand("", conn, tran))
                        {
                            try
                            {
                                var productTmp = products.Select(x => new
                                {
                                    PKID = x.PKID,
                                    ActivityID = x.ActivityID,
                                    PID = x.PID,
                                    Position = x.Position,
                                    Price = x.Price,
                                    Label = x.Label,
                                    TotalQuantity = x.TotalQuantity,
                                    MaxQuantity = x.MaxQuantity,
                                    SaleOutQuantity = x.SaleOutQuantity,
                                    CreateDateTime = x.CreateDateTime ?? DateTime.Now,
                                    LastUpdateDateTime = x.LastUpdateDateTime ?? DateTime.Now,
                                    ProductName = x.ProductName,
                                    InstallAndPay = x.InstallAndPay,
                                    ImgUrl = x.Image,
                                    IsUsePCode = x.IsUsePCode == true ? 1 : 0,
                                    Channel = x.Channel,
                                    FalseOriginalPrice = x.FalseOriginalPrice,
                                    IsJoinPlace = x.IsJoinPlace == true ? 1 : 0,
                                    IsShow = x.IsShow == true ? 1 : 0,
                                    InstallService = x.InstallService
                                });
                                DataTable productDT = ConvertDataTable.ToDataTable(productTmp);
                                cmd.CommandText = @"CREATE TABLE #productTmp([PKID] [INT] NULL,
	                                                            [ActivityID] [UNIQUEIDENTIFIER] NULL,
	                                                            [PID] [VARCHAR](200) NOT NULL,
	                                                            [Position] [INT] NULL,
	                                                            [Price] [MONEY] NOT NULL,
	                                                            [Label] [NVARCHAR](20) NULL,
	                                                            [TotalQuantity] [INT] NULL,
	                                                            [MaxQuantity] [INT] NULL,
	                                                            [SaleOutQuantity] [INT] NULL,
	                                                            [CreateDateTime] [DATETIME] NULL,
	                                                            [LastUpdateDateTime] [DATETIME] NULL,
	                                                            [ProductName] [NVARCHAR](200) NULL,
	                                                            [InstallAndPay] [NVARCHAR](50) NULL,
	                                                            [ImgUrl] [NVARCHAR](200) NULL,
	                                                            [IsUsePCode] bit NULL,
	                                                            [Channel] [NVARCHAR](10) NULL,
	                                                            [FalseOriginalPrice] [MONEY] NULL,
	                                                            [IsJoinPlace] bit NULL,
	                                                            [IsShow] bit NULL,
	                                                            [InstallService] [NVARCHAR](100) NULL);";
                                cmd.ExecuteNonQuery();
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, tran))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "#productTmp";
                                    bulkcopy.WriteToServer(productDT);
                                    bulkcopy.Close();
                                }
                                string sqlInsert = @"INSERT INTO [Activity].[dbo].[tbl_FlashSaleProductsLog]
                                                                           ([FlashPKID]
                                                                           ,[ActivityID]
                                                                           ,[PID]
                                                                           ,[Position]
                                                                           ,[Price]
                                                                           ,[Label]
                                                                           ,[TotalQuantity]
                                                                           ,[MaxQuantity]
                                                                           ,[SaleOutQuantity]
                                                                           ,[CreateDateTime]
                                                                           ,[LastUpdateDateTime]
                                                                           ,[ProductName]
                                                                           ,[InstallAndPay]
                                                                           ,[ImgUrl]
                                                                           ,[IsUsePCode]
                                                                           ,[Channel]
                                                                           ,[FalseOriginalPrice]
                                                                           ,[IsJoinPlace]
                                                                           ,[IsShow]
                                                                           ,[InstallService]
                                                                           ,[DeletedTime])
                                                                    SELECT [PKID]
                                                                           ,[ActivityID]
                                                                           ,[PID]
                                                                           ,[Position]
                                                                           ,[Price]
                                                                           ,[Label]
                                                                           ,[TotalQuantity]
                                                                           ,[MaxQuantity]
                                                                           ,[SaleOutQuantity]
                                                                           ,[CreateDateTime]
                                                                           ,[LastUpdateDateTime]
                                                                           ,[ProductName]
                                                                           ,[InstallAndPay]
                                                                           ,[ImgUrl]
                                                                           ,[IsUsePCode]
                                                                           ,[Channel]
                                                                           ,[FalseOriginalPrice]
                                                                           ,[IsJoinPlace]
                                                                           ,[IsShow]
                                                                           ,[InstallService]
                                                                           ,getdate()
                                                                    FROM #productTmp;";
                                cmd.CommandText = sqlInsert;
                                result = cmd.ExecuteNonQuery();
                                if (result > 0)
                                {
                                    tran.Commit();
                                }
                                else
                                {
                                    tran.Rollback();
                                }
                            }
                            catch
                            {
                                tran.Rollback();
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 把删除的活动信息插入到活动删除备份表 - Temp表
        /// </summary>
        /// <param name="deletedModel"></param>
        /// <returns></returns>
        public static int InsertDeletedActivityInfo_Temp(QiangGouModel deletedModel)
        {
            string sqlInsert = @" INSERT INTO [Activity].[dbo].[tbl_FlashSaleExamLog]
                                           ([ActivityID]
                                           ,[ActivityName]
                                           ,[StartDateTime]
                                           ,[EndDateTime]
                                           ,[CreateDateTime]
                                           ,[UpdateDateTime]
                                           ,[ActiveType]
                                           ,[PlaceQuantity]
                                           ,[IsNewUserFirstOrder]
                                           ,[IsDefault]
                                           ,[DeletedTime])
                                     VALUES
                                           (@ActivityID
                                           ,@ActivityName
                                           ,@StartDateTime
                                           ,@EndDateTime
                                           ,@CreateDateTime
                                           ,@UpdateDateTime
                                           ,@ActiveType
                                           ,@PlaceQuantity
                                           ,@IsNewUserFirstOrder
                                           ,@IsDefault
                                           ,GETDATE()) ";

            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                using (var cmd = new SqlCommand(sqlInsert))
                {
                    cmd.Parameters.AddWithValue("@ActivityID", deletedModel.ActivityID);
                    cmd.Parameters.AddWithValue("@ActivityName", deletedModel.ActivityName);
                    cmd.Parameters.AddWithValue("@StartDateTime", deletedModel.StartDateTime);
                    cmd.Parameters.AddWithValue("@EndDateTime", deletedModel.EndDateTime);
                    cmd.Parameters.AddWithValue("@CreateDateTime", deletedModel.CreateDateTime ?? DateTime.Now);
                    cmd.Parameters.AddWithValue("@UpdateDateTime", deletedModel.UpdateDateTime ?? DateTime.Now);
                    cmd.Parameters.AddWithValue("@ActiveType", deletedModel.ActiveType);
                    cmd.Parameters.AddWithValue("@PlaceQuantity", deletedModel.PlaceQuantity);
                    cmd.Parameters.AddWithValue("@IsNewUserFirstOrder", deletedModel.IsNewUserFirstOrder);
                    cmd.Parameters.AddWithValue("@IsDefault", deletedModel.IsDefault);
                    var result = dbHelper.ExecuteNonQuery(cmd);
                    return result;
                }
            }
        }

        /// <summary>
        /// 把删除的活动信商品信息插入到活动商品删除备份表 - Temp表
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public static int InsertDeletedActivityProducts_Temp(List<QiangGouProductModel> products)
        {
            var result = 0;
            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        using (var cmd = new SqlCommand("", conn, tran))
                        {
                            try
                            {
                                var productTmp = products.Select(x => new
                                {
                                    PKID = x.PKID,
                                    ActivityID = x.ActivityID,
                                    PID = x.PID,
                                    Position = x.Position,
                                    Price = x.Price,
                                    Label = x.Label,
                                    TotalQuantity = x.TotalQuantity,
                                    MaxQuantity = x.MaxQuantity,
                                    SaleOutQuantity = x.SaleOutQuantity,
                                    CreateDateTime = x.CreateDateTime ?? DateTime.Now,
                                    LastUpdateDateTime = x.LastUpdateDateTime ?? DateTime.Now,
                                    ProductName = x.ProductName,
                                    InstallAndPay = x.InstallAndPay,
                                    ImgUrl = x.Image,
                                    IsUsePCode = x.IsUsePCode == true ? 1 : 0,
                                    Channel = x.Channel,
                                    FalseOriginalPrice = x.FalseOriginalPrice,
                                    IsJoinPlace = x.IsJoinPlace == true ? 1 : 0,
                                    IsShow = x.IsShow == true ? 1 : 0,
                                    InstallService = x.InstallService
                                });
                                DataTable productDT = ConvertDataTable.ToDataTable(productTmp);
                                cmd.CommandText = @"CREATE TABLE #productTmp([PKID] [INT] NULL,
	                                                            [ActivityID] [UNIQUEIDENTIFIER] NULL,
	                                                            [PID] [VARCHAR](200) NOT NULL,
	                                                            [Position] [INT] NULL,
	                                                            [Price] [MONEY] NOT NULL,
	                                                            [Label] [NVARCHAR](20) NULL,
	                                                            [TotalQuantity] [INT] NULL,
	                                                            [MaxQuantity] [INT] NULL,
	                                                            [SaleOutQuantity] [INT] NULL,
	                                                            [CreateDateTime] [DATETIME] NULL,
	                                                            [LastUpdateDateTime] [DATETIME] NULL,
	                                                            [ProductName] [NVARCHAR](200) NULL,
	                                                            [InstallAndPay] [NVARCHAR](50) NULL,
	                                                            [ImgUrl] [NVARCHAR](200) NULL,
	                                                            [IsUsePCode] bit NULL,
	                                                            [Channel] [NVARCHAR](10) NULL,
	                                                            [FalseOriginalPrice] [MONEY] NULL,
	                                                            [IsJoinPlace] bit NULL,
	                                                            [IsShow] bit NULL,
	                                                            [InstallService] [NVARCHAR](100) NULL);";
                                cmd.ExecuteNonQuery();
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, tran))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "#productTmp";
                                    bulkcopy.WriteToServer(productDT);
                                    bulkcopy.Close();
                                }
                                string sqlInsert = @"INSERT INTO [Activity].[dbo].[tbl_FlashSaleProductsExamLog]
                                                                           ([FlashPKID]
                                                                           ,[ActivityID]
                                                                           ,[PID]
                                                                           ,[Position]
                                                                           ,[Price]
                                                                           ,[Label]
                                                                           ,[TotalQuantity]
                                                                           ,[MaxQuantity]
                                                                           ,[SaleOutQuantity]
                                                                           ,[CreateDateTime]
                                                                           ,[LastUpdateDateTime]
                                                                           ,[ProductName]
                                                                           ,[InstallAndPay]
                                                                           ,[ImgUrl]
                                                                           ,[IsUsePCode]
                                                                           ,[Channel]
                                                                           ,[FalseOriginalPrice]
                                                                           ,[IsJoinPlace]
                                                                           ,[IsShow]
                                                                           ,[InstallService]
                                                                           ,[DeletedTime])
                                                                    SELECT [PKID]
                                                                           ,[ActivityID]
                                                                           ,[PID]
                                                                           ,[Position]
                                                                           ,[Price]
                                                                           ,[Label]
                                                                           ,[TotalQuantity]
                                                                           ,[MaxQuantity]
                                                                           ,[SaleOutQuantity]
                                                                           ,[CreateDateTime]
                                                                           ,[LastUpdateDateTime]
                                                                           ,[ProductName]
                                                                           ,[InstallAndPay]
                                                                           ,[ImgUrl]
                                                                           ,[IsUsePCode]
                                                                           ,[Channel]
                                                                           ,[FalseOriginalPrice]
                                                                           ,[IsJoinPlace]
                                                                           ,[IsShow]
                                                                           ,[InstallService]
                                                                           ,getdate()
                                                                    FROM #productTmp;";
                                cmd.CommandText = sqlInsert;
                                result = cmd.ExecuteNonQuery();
                                if (result > 0)
                                {
                                    tran.Commit();
                                }
                                else
                                {
                                    tran.Rollback();
                                }
                            }
                            catch
                            {
                                tran.Rollback();
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        #endregion
    }
}
