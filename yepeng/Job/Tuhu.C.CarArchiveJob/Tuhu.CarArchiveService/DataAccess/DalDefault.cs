using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.CarArchiveService.Models;

namespace Tuhu.CarArchiveService.DataAccess
{
    public static class DalDefault
    {
        private static string tuhuShop_connString = ConfigurationManager.ConnectionStrings["Tuhu_shop_ReadOnly"].ConnectionString;
        /// <summary>
        /// 更新推送状态
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="status"></param>
        /// <param name="errorResult"></param>
        public static void UpdatePushStatus(int pkid, int status, string errorResult)
        {
            const string sql = @"UPDATE Tuhu_log..CarArchivePushHistory SET PushStatus = @Status, ErrorResult = @Error, UpdateTime = GETDATE() WHERE PKID = @PKID";

            using (var dbhelper = DbHelper.CreateLogDbHelper())
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@PKID", pkid);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Error", errorResult);
                dbhelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 更新推送状态shqx
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="status"></param>
        /// <param name="errorResult"></param>
        public static void UpdatePushStatus_SHQX(int pkid, int status, string errorResult)
        {
            const string sql = @"UPDATE Tuhu_log..CarArchivePushHistory_SHQX SET PushStatus = @Status, ErrorResult = @Error, UpdateTime = GETDATE() WHERE PKID = @PKID";

            using (var dbhelper = DbHelper.CreateLogDbHelper())
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@PKID", pkid);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Error", errorResult);
                dbhelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 更新推送状态shqx
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="status"></param>
        /// <param name="errorResult"></param>
        public static void UpdatePushStatus_ChengDu(int pkid, int status, string errorResult)
        {
            const string sql = @"UPDATE Tuhu_log..CarArchivePushHistory_ChengDu SET PushStatus = @Status, ErrorResult = @Error, UpdateTime = GETDATE() WHERE PKID = @PKID";

            using (var dbhelper = DbHelper.CreateLogDbHelper())
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@PKID", pkid);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Error", errorResult);
                dbhelper.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 根据ID获取已安装订单信息
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public static BaoYangRecordModel SelectOrderDetail(int orderId, string orderNo)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = @"SELECT  o.PKID AS OrderId ,
                                            o.InstallDatetime ,
                                            o.InstallShopID ,
                                            s.CompanyName AS InstallShopName ,
                                            car.VinCode ,
                                            car.PlateNumber ,
                                            CASE WHEN car.Distance IS NULL THEN 0
                                                 ELSE car.Distance
                                            END AS Distance ,
                                            ol.PID ,
                                            ol.Name ,
                                            ol.Num ,
                                            ol.Price
                                    FROM    Gungnir..vw_tbl_Order AS o WITH ( NOLOCK )
                                            JOIN Gungnir..vw_tbl_OrderList AS ol WITH ( NOLOCK ) ON ol.OrderID = o.PKID
                                            JOIN Gungnir..Shops AS s WITH ( NOLOCK ) ON s.PKID = o.InstallShopID
                                            JOIN ( SELECT    TOP 1 * 
                                                              FROM      Tuhu_order..tbl_OrderCar AS oc WITH ( NOLOCK )
						                                      WHERE oc.OrderNo = @OrderNo
						                                      ORDER BY UserCreated
                                                 ) AS car ON car.OrderNo = o.OrderNo COLLATE Chinese_PRC_CI_AS
                                    WHERE   o.PKID = @OrderId AND ol.Deleted = 0;";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@OrderNo", orderNo);

                return DbHelper.ExecuteQuery(true, cmd, ConvertDt2BaoYangRecordModel);
            }
        }

        /// <summary>
        /// 获取当前时间段的已安装订单
        /// </summary>
        /// <returns></returns>
        public static List<Tuple<int, string>> SelectOrderIds(DateTime begin)
        {
            using (var cmd = new SqlCommand(@"SELECT DISTINCT o.PKID, o.OrderNo
                                                FROM    Gungnir..tbl_Order AS o WITH ( NOLOCK )
                                                        JOIN Tuhu_order..tbl_OrderCar AS car WITH ( NOLOCK ) ON car.OrderNo = o.OrderNo COLLATE Chinese_PRC_CI_AS
                                                WHERE   InstallStatus = '2Installed'
                                                        AND InstallType = '1ShopInstall'
                                                        AND InstallDatetime > @Begin
                                                        AND car.VinCode IS NOT NULL
                                                        AND car.VinCode != ''
                                                        AND Len(car.VinCode)=17 
                                                        AND car.PlateNumber IS NOT NULL
                                                        AND car.PlateNumber != '';"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Begin", begin);
                return DbHelper.ExecuteQuery(true, cmd, (dt) => ConvertDtToTupleList(dt)).Distinct().ToList();
            }
        }

        /// <summary>
        /// 获取历史已加进记录表的最大安装时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetMaxInstallTimeInHistory()
        {
            DateTime result = DateTime.Now.AddHours(-12);
            using (var dbhelper = DbHelper.CreateLogDbHelper())
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT MAX(InstallDateTime) FROM Tuhu_log..CarArchivePushHistory WITH ( NOLOCK );";
                var time = dbhelper.ExecuteScalar(cmd);
                if (time != null && !(time is DBNull))
                {
                    result = Convert.ToDateTime(time) < result ? result : Convert.ToDateTime(time);
                }
            }

            return result;
        }
        /// <summary>
        /// 获取历史已加进记录表的最大安装时间(SHQX)
        /// </summary>
        /// <returns></returns>
        public static DateTime GetMaxInstallTimeInHistory_SHQX()
        {
            DateTime result = DateTime.Now.AddHours(-12);
            using (var dbhelper = DbHelper.CreateLogDbHelper())
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT MAX(InstallDateTime) FROM Tuhu_log..CarArchivePushHistory_SHQX WITH ( NOLOCK );";
                var time = dbhelper.ExecuteScalar(cmd);
                if (time != null && !(time is DBNull))
                {
                    result = Convert.ToDateTime(time);
                }
            }

            return result;
        }
        /// <summary>
        /// 获取历史已加进记录表的最大安装时间(ChengDu)
        /// </summary>
        /// <returns></returns>
        public static DateTime GetMaxInstallTimeInHistory_ChengDu()
        {
            DateTime result = DateTime.Now.AddHours(-12);
            using (var dbhelper = DbHelper.CreateLogDbHelper())
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT MAX(InstallDateTime) FROM Tuhu_log..CarArchivePushHistory_ChengDu WITH ( NOLOCK );";
                var time = dbhelper.ExecuteScalar(cmd);
                if (time != null && !(time is DBNull))
                {
                    result = Convert.ToDateTime(time);
                }
            }

            return result;
        }
        public static string SelectShopCode(int shopId)
        {
            using (var cmd = new SqlCommand(@"SELECT CarArchiveCode FROM Gungnir..ShopConfig With(NOLOCK) WHERE ShopID = @ShopId"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ShopId", shopId);
                return Convert.ToString(DbHelper.ExecuteScalar(true, cmd));
            }
        }
        public static Dictionary<int, string> SelectShopCodeDic(IEnumerable<int> shopIds)
        {
            using (var cmd = new SqlCommand(@"SELECT ShopID,CarArchiveCode FROM Gungnir..ShopConfig With(NOLOCK) WHERE ShopID IN(SELECT * FROM Gungnir..SplitString(@ShopIds,',',1))"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ShopIds", string.Join(",", shopIds));
                return DbHelper.ExecuteQuery(true, cmd, (DataTable dt) =>
               {
                   Dictionary<int, string> result = new Dictionary<int, string>();
                   if (dt != null && dt.Rows.Count > 0)
                   {
                       foreach (DataRow item in dt.Rows)
                       {
                           int shopid = item.GetValue<int>("ShopID");
                           string shopCode = item.GetValue<string>("CarArchiveCode");
                           if (!result.ContainsKey(shopid) && !string.IsNullOrEmpty(shopCode))
                           {
                               result.Add(shopid, shopCode);
                           }
                       }
                   }
                   return result;
               });
            }
        }

        public static Dictionary<int, Tuple<String, String>> SelectShopCodeDic_SHQX(IEnumerable<int> shopIds)
        {
            using (var cmd = new SqlCommand(@"SELECT ShopID,SHQX_ShopAccount,SHQX_PassWord FROM Gungnir..ShopConfig With(NOLOCK) WHERE ShopID IN(SELECT * FROM Gungnir..SplitString(@ShopIds,',',1))"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ShopIds", string.Join(",", shopIds));
                return DbHelper.ExecuteQuery(true, cmd, (DataTable dt) =>
                {
                    Dictionary<int, Tuple<String, String>> result = new Dictionary<int, Tuple<String, String>>();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow item in dt.Rows)
                        {
                            int shopid = item.GetValue<int>("ShopID");
                            string shopAccount = item.GetValue<string>("SHQX_ShopAccount");
                            string passWord = item.GetValue<string>("SHQX_PassWord");
                            if (!result.ContainsKey(shopid) && !string.IsNullOrEmpty(shopAccount) && !string.IsNullOrEmpty(passWord))
                            {
                                result.Add(shopid, Tuple.Create(shopAccount, passWord));
                            }
                        }
                    }
                    return result;
                });
            }
        }
        /// <summary>
        /// 获取注册过的门店
        /// </summary>
        /// <param name="shopIds"></param>
        /// <param name="pushTo"></param>
        /// <returns></returns>
        public static Dictionary<int, Tuple<String, String>> SelectShopCodeDic_All(IEnumerable<int> shopIds, PushToEnum pushTo)
        {
            using (var dbHelper = DbHelper.CreateDbHelper("ThirdParty_ReadOnly"))
            using (var cmd = new SqlCommand(@"SELECT  [PKID]
      ,[ShopId]
      ,[ShopAccount]
      ,[ShopPassword]
      ,[PushTo]
      ,[CreateTime]
      ,[UpdateTime]
  FROM [Tuhu_thirdparty].[dbo].[CarArchivePush_ShopConfig] With(NOLOCK) 
WHERE ShopID IN(SELECT * FROM [Tuhu_thirdparty].[dbo].SplitString(@ShopIds,',',1)) 
AND PushTo=@PushTo"))
            {
                try
                {
                    cmd.Parameters.AddWithValue("@ShopIds", string.Join(",", shopIds));
                    cmd.Parameters.AddWithValue("@PushTo", pushTo.ToString());
                    return dbHelper.ExecuteQuery(cmd, (DataTable dt) =>
                    {
                        Dictionary<int, Tuple<String, String>> result = new Dictionary<int, Tuple<String, String>>();
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow item in dt.Rows)
                            {
                                int shopid = item.GetValue<int>("ShopId");
                                string shopAccount = item.GetValue<string>("ShopAccount");
                                string passWord = item.GetValue<string>("ShopPassword");
                                if (!result.ContainsKey(shopid) && !string.IsNullOrEmpty(shopAccount) && !string.IsNullOrEmpty(passWord))
                                {
                                    result.Add(shopid, Tuple.Create(shopAccount, passWord));
                                }
                            }
                        }
                        return result;
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public static bool InsertShopCodeDic_All(int shopId, string account, string password, PushToEnum pushTo)
        {
            using (var dbHelper=DbHelper.CreateDbHelper("ThirdParty"))
            using (var cmd = new SqlCommand(@"  Insert into [Tuhu_thirdparty].[dbo].[CarArchivePush_ShopConfig](
  [ShopId]
 ,[ShopAccount]
 ,[ShopPassword]
 ,[PushTo]
 ,[CreateTime]
 ,[UpdateTime]
  )values(
   @ShopId
 , @ShopAccount
 , @ShopPassword
 , @PushTo
 , GetDate()
 , GetDate()
  )"))
            {
                try
                {
                    cmd.Parameters.AddWithValue("@ShopId", shopId);
                    cmd.Parameters.AddWithValue("@ShopAccount", account);
                    cmd.Parameters.AddWithValue("@ShopPassword", password);
                    cmd.Parameters.AddWithValue("@PushTo", pushTo.ToString());
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public static Tuple<string, string, string> SelectShopLicenceAndCode(int shopId)
        {
            using (var helper = DbHelper.CreateDbHelper("Tuhu_shop_ReadOnly"))
            {
                using (var cmd = new SqlCommand(@"DECLARE @RoadLicense NVARCHAR(100)
                                            DECLARE @ShopName NVARCHAR(50)
                                            DECLARE @RegionCode NVARCHAR(50)

                                            SELECT  @RoadLicense = de.Value
                                            FROM    Tuhu_shop.dbo.ShopBaseInfoDetails AS de WITH ( NOLOCK )
                                            WHERE   de.ShopID = @ShopId
                                                    AND de.ShopBaseInfoConfigID = 77
                                            SELECT  @ShopName = de.Value
                                            FROM    Tuhu_shop.dbo.ShopBaseInfoDetails AS de WITH ( NOLOCK )
                                            WHERE   de.ShopID = @ShopId
                                                    AND de.ShopBaseInfoConfigID = 83
                                            SELECT  @RegionCode = de.Value
                                            FROM    Tuhu_shop.dbo.ShopBaseInfoDetails AS de WITH ( NOLOCK )
                                            WHERE   de.ShopID = @ShopId
                                                    AND de.ShopBaseInfoConfigID = 119
                                            SELECT  @RoadLicense ,
                                                    @ShopName ,
                                                    @RegionCode;"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@ShopId", shopId);
                    return helper.ExecuteQuery(cmd, ConvertDtToTuple);
                }
            }
        }

        public static void UpdateCarArchiveCode(int shopId, string carArchiveCode)
        {
            using (var cmd = new SqlCommand(@"UPDATE Gungnir..ShopConfig SET CarArchiveCode = @CarArchiveCode WHERE ShopID = @ShopId;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ShopId", shopId);
                cmd.Parameters.AddWithValue("@CarArchiveCode", carArchiveCode);
                DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static List<string> SelectAllServiceIds()
        {
            using (var cmd = new SqlCommand(@"SELECT DISTINCT ServiceId FROM BaoYang..BaoYangTypeConfig (NOLOCK);"))
            {
                cmd.CommandType = CommandType.Text;
                return DbHelper.ExecuteQuery(true, cmd, (dt) => ConvertDt2List<string>(dt)).ToList();
            }
        }

        public static Dictionary<string, string> SelectPartCodes(List<string> pids)
        {
            const string sql = @"WITH pids AS (SELECT * FROM Gungnir..SplitString(@Pids, ',', 1))
                            SELECT TuhuProductID, LiYangProductID FROM Gungnir..tbl_BaoYang_NewProductRef WITH(NOLOCK) JOIN pids ON TuhuProductID = pids.Item";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Pids", string.Join(",", pids));
                return DbHelper.ExecuteQuery(true, cmd, ConvertDt2StringDic);
            }
        }

        /// <summary>
        /// 增加安装订单记录
        /// </summary>
        /// <param name="records"></param>
        public static void InsertRecord(BaoYangRecordModel record)
        {
            const string sql = @"INSERT  Tuhu_log..CarArchivePushHistory
                                        ( OrderId ,
                                          InstallShopId ,
                                          InstallShopName ,
                                          InstallDateTime ,
                                          ShopCode ,
                                          ShopRegionCode ,
                                          VinCode ,
                                          PlateNumber ,
                                          Distance ,
                                          VehiclePartList ,
                                          RepairProjectList ,
                                          PushStatus ,
                                          CreateTime 
                                        )
                                VALUES  ( @OrderId ,
                                          @InstallShopId ,
                                          @InstallShopName ,
                                          @InstallDateTime ,
                                          @ShopCode ,
                                          @ShopRegionCode ,
                                          @VinCode ,
                                          @PlateNumber ,
                                          @Distance ,
                                          @VehiclePartList ,
                                          @RepairProjectList ,
                                          0 ,
                                          GETDATE()
                                        );";

            using (var helper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@OrderId", record.OrderId);
                    cmd.Parameters.AddWithValue("@InstallShopId", record.InstallShopId);
                    cmd.Parameters.AddWithValue("@InstallShopName", record.InstallShopName);
                    cmd.Parameters.AddWithValue("@InstallDateTime", record.InstallDatetime);
                    cmd.Parameters.AddWithValue("@ShopCode", record.ShopCode);
                    cmd.Parameters.AddWithValue("@ShopRegionCode", record.ShopRegionCode);
                    cmd.Parameters.AddWithValue("@VinCode", record.VinCode);
                    cmd.Parameters.AddWithValue("@PlateNumber", record.PlateNumber);
                    cmd.Parameters.AddWithValue("@Distance", record.Distance);
                    cmd.Parameters.AddWithValue("@VehiclePartList", JsonConvert.SerializeObject(record.PartList));
                    cmd.Parameters.AddWithValue("@RepairProjectList", JsonConvert.SerializeObject(record.ProjectList));

                    helper.ExecuteNonQuery(cmd);
                }
            }
        }
        /// <summary>
        /// 增加安装订单记录SHQC
        /// </summary>
        /// <param name="records"></param>
        public static void InsertRecord_SHQX(BaoYangRecordModel record)
        {
            const string sql = @"INSERT  Tuhu_log..CarArchivePushHistory_SHQX
                                        ( OrderId ,
                                          InstallShopId ,
                                          InstallShopName ,
                                          InstallDateTime ,
                                          ShopCode ,
                                          ShopRegionCode ,
                                          VinCode ,
                                          PlateNumber ,
                                          Distance ,
                                          VehiclePartList ,
                                          RepairProjectList ,
                                          PushStatus ,
                                          CreateTime 
                                        )
                                VALUES  ( @OrderId ,
                                          @InstallShopId ,
                                          @InstallShopName ,
                                          @InstallDateTime ,
                                          @ShopCode ,
                                          @ShopRegionCode ,
                                          @VinCode ,
                                          @PlateNumber ,
                                          @Distance ,
                                          @VehiclePartList ,
                                          @RepairProjectList ,
                                          0 ,
                                          GETDATE()
                                        );";

            using (var helper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@OrderId", record.OrderId);
                    cmd.Parameters.AddWithValue("@InstallShopId", record.InstallShopId);
                    cmd.Parameters.AddWithValue("@InstallShopName", record.InstallShopName);
                    cmd.Parameters.AddWithValue("@InstallDateTime", record.InstallDatetime);
                    cmd.Parameters.AddWithValue("@ShopCode", record.ShopCode);
                    cmd.Parameters.AddWithValue("@ShopRegionCode", record.ShopRegionCode);
                    cmd.Parameters.AddWithValue("@VinCode", record.VinCode);
                    cmd.Parameters.AddWithValue("@PlateNumber", record.PlateNumber);
                    cmd.Parameters.AddWithValue("@Distance", record.Distance);
                    cmd.Parameters.AddWithValue("@VehiclePartList", JsonConvert.SerializeObject(record.PartList));
                    cmd.Parameters.AddWithValue("@RepairProjectList", JsonConvert.SerializeObject(record.ProjectList));

                    helper.ExecuteNonQuery(cmd);
                }
            }
        }
        /// <summary>
        /// 增加安装订单记录ChengDu
        /// </summary>
        /// <param name="records"></param>
        public static void InsertRecord_ChengDu(BaoYangRecordModel record)
        {
            const string sql = @"INSERT  Tuhu_log..CarArchivePushHistory_ChengDu
                                        ( OrderId ,
                                          InstallShopId ,
                                          InstallShopName ,
                                          InstallDateTime ,
                                          ShopCode ,
                                          ShopRegionCode ,
                                          VinCode ,
                                          PlateNumber ,
                                          Distance ,
                                          VehiclePartList ,
                                          RepairProjectList ,
                                          PushStatus ,
                                          CreateTime 
                                        )
                                VALUES  ( @OrderId ,
                                          @InstallShopId ,
                                          @InstallShopName ,
                                          @InstallDateTime ,
                                          @ShopCode ,
                                          @ShopRegionCode ,
                                          @VinCode ,
                                          @PlateNumber ,
                                          @Distance ,
                                          @VehiclePartList ,
                                          @RepairProjectList ,
                                          0 ,
                                          GETDATE()
                                        );";

            using (var helper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@OrderId", record.OrderId);
                    cmd.Parameters.AddWithValue("@InstallShopId", record.InstallShopId);
                    cmd.Parameters.AddWithValue("@InstallShopName", record.InstallShopName);
                    cmd.Parameters.AddWithValue("@InstallDateTime", record.InstallDatetime);
                    cmd.Parameters.AddWithValue("@ShopCode", record.ShopCode);
                    cmd.Parameters.AddWithValue("@ShopRegionCode", record.ShopRegionCode);
                    cmd.Parameters.AddWithValue("@VinCode", record.VinCode);
                    cmd.Parameters.AddWithValue("@PlateNumber", record.PlateNumber);
                    cmd.Parameters.AddWithValue("@Distance", record.Distance);
                    cmd.Parameters.AddWithValue("@VehiclePartList", JsonConvert.SerializeObject(record.PartList));
                    cmd.Parameters.AddWithValue("@RepairProjectList", JsonConvert.SerializeObject(record.ProjectList));

                    helper.ExecuteNonQuery(cmd);
                }
            }
        }
        public static List<BaoYangRecordModel> SelectTop500UnPushedRecords()
        {
            const string sql = @"SELECT TOP 500
                                        *
                                FROM    Tuhu_log..CarArchivePushHistory WITH ( NOLOCK )
                                WHERE   PushStatus = 0
                                        AND ShopCode IS NOT NULL
                                        AND ShopCode != '';";
            using (var dbhelper = DbHelper.CreateLogDbHelper())
            {
                var cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;

                return dbhelper.ExecuteQuery(cmd, ConvertDt2BaoYangRecordModelList);
            }
        }
        public static List<BaoYangRecordModel> SelectTop500UnPushedRecordsSHQX()
        {
            const string sql = @"SELECT TOP 500
                                        *
                                FROM    Tuhu_log..CarArchivePushHistory_SHQX WITH ( NOLOCK )
                                WHERE   PushStatus = 0;";
            using (var dbhelper = DbHelper.CreateLogDbHelper())
            {
                var cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;

                return dbhelper.ExecuteQuery(cmd, ConvertDt2BaoYangRecordModelList);
            }
        }
        public static List<BaoYangRecordModel> SelectTop500UnPushedRecordsChengDu()
        {
            const string sql = @"SELECT TOP 500
                                        *
                                FROM    Tuhu_log..CarArchivePushHistory_ChengDu WITH ( NOLOCK )
                                WHERE   PushStatus = 0;";
            using (var dbhelper = DbHelper.CreateLogDbHelper())
            {
                var cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;

                return dbhelper.ExecuteQuery(cmd, ConvertDt2BaoYangRecordModelList);
            }
        }
        private static List<BaoYangRecordModel> ConvertDt2BaoYangRecordModelList(DataTable dt)
        {
            List<BaoYangRecordModel> result = new List<BaoYangRecordModel>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var item = new BaoYangRecordModel();
                    item.PKID = row.GetValue<int>("PKID");
                    item.OrderId = row.GetValue<int>("OrderId");
                    item.InstallDatetime = row.GetValue<DateTime>("InstallDatetime");
                    item.InstallShopId = row.GetValue<int>("InstallShopID");
                    item.InstallShopName = row.GetValue<string>("InstallShopName");
                    item.ShopCode = row.GetValue<string>("ShopCode");
                    item.ShopRegionCode = row.GetValue<string>("ShopRegionCode");
                    item.VinCode = row.GetValue<string>("VinCode");
                    item.PlateNumber = row.GetValue<string>("PlateNumber");
                    item.Distance = row.GetValue<int>("Distance");
                    item.PartList = JsonConvert.DeserializeObject<List<PartItem>>(row.GetValue<string>("VehiclePartList"));
                    item.ProjectList = JsonConvert.DeserializeObject<List<ProjectItem>>(row.GetValue<string>("RepairProjectList"));

                    result.Add(item);
                }
            }

            return result;
        }

        public static Dictionary<string, string> ConvertDt2StringDic(DataTable dt)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string key = row.IsNull(0) ? string.Empty : row[0].ToString();
                    string value = row.IsNull(1) ? string.Empty : row[1].ToString();
                    result[key] = value;
                }
            }

            return result;
        }

        public static Tuple<string, string, string> ConvertDtToTuple(DataTable dt)
        {
            Tuple<string, string, string> result = Tuple.Create(string.Empty, string.Empty, string.Empty);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string item1 = row.IsNull(0) ? string.Empty : row[0].ToString();
                    string item2 = row.IsNull(1) ? string.Empty : row[1].ToString();
                    string item3 = row.IsNull(2) ? string.Empty : row[2].ToString();

                    result = Tuple.Create(item1, item2, item3);
                }
            }

            return result;
        }

        public static List<Tuple<int, string>> ConvertDtToTupleList(DataTable dt)
        {
            List<Tuple<int, string>> result = new List<Tuple<int, string>>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    int item1 = row.IsNull(0) ? 0 : Convert.ToInt32(row[0].ToString());
                    string item2 = row.IsNull(1) ? string.Empty : row[1].ToString();

                    result.Add(Tuple.Create(item1, item2));
                }
            }

            return result;
        }

        private static IEnumerable<T> ConvertDt2List<T>(DataTable dt)
        {
            List<T> result = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    T item = row.IsNull(0) ? default(T) : (T)row[0];
                    result.Add(item);
                }
            }

            return result;
        }

        private static BaoYangRecordModel ConvertDt2BaoYangRecordModel(DataTable dt)
        {
            BaoYangRecordModel result = null;

            if (dt != null && dt.Rows.Count > 0)
            {
                result = new BaoYangRecordModel();
                result.PartList = new List<PartItem>();
                result.ProjectList = new List<ProjectItem>();
                foreach (DataRow row in dt.Rows)
                {
                    result.OrderId = row.GetValue<int>("OrderId");
                    result.InstallDatetime = row.GetValue<DateTime>("InstallDatetime");
                    result.InstallShopId = row.GetValue<int>("InstallShopID");
                    result.InstallShopName = row.GetValue<string>("InstallShopName");
                    result.VinCode = row.GetValue<string>("VinCode");
                    result.PlateNumber = row.GetValue<string>("PlateNumber");
                    result.Distance = row.GetValue<int>("Distance");
                    string pid = row.GetValue<string>("PID");
                    string name = row.GetValue<string>("Name");
                    int num = row.GetValue<int>("Num");
                    decimal price = row.GetValue<decimal>("Price");

                    if (pid.StartsWith("FU"))
                    {
                        result.ProjectList.Add(new ProjectItem()
                        {
                            ServiceId = pid,
                            Name = name,
                            Price = price * num
                        });
                    }
                    else
                    {
                        result.PartList.Add(new PartItem()
                        {
                            ProductName = name,
                            ProductId = pid,
                            Num = num
                        });
                    }
                }
            }

            return result;
        }

        public static bool UpdateShopConfigs(int shopId, string shopAccount, string passWord)
        {
            const string sql = @"UPDATE [Gungnir].[dbo].[ShopConfig] WITH(RowLock) 
 SET SHQX_ShopAccount=@ShopAccount,
SHQX_PassWord=@PassWord 
 WHERE ShopId=@ShopId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ShopId", shopId);
                cmd.Parameters.AddWithValue("@ShopAccount", shopAccount);
                cmd.Parameters.AddWithValue("@PassWord", passWord);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }


        }
        public static string GetCreditidentifierByShopId(int shopId)
        {
            const string sql = @"SELECT TOP 1 TaxpayerIdentify FROM Tuhu_shop.dbo.ShopInvoiceInfomation WITH(NOLOCK) WHERE ShopId=@shopId";
            using (var helper = DbHelper.CreateDbHelper("Tuhu_shop_ReadOnly"))
            using (var cmd = new SqlCommand(sql))
            {
                try
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@shopId", shopId);
                    return helper.ExecuteScalar(cmd)?.ToString();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
