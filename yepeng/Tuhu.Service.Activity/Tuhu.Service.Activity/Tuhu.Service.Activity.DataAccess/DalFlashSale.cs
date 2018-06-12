using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Service.Activity.DataAccess.Models;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalFlashSale
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalFlashSale));
        public static async Task<FlashSaleModel> SelectFlashSaleFromDBAsync(Guid activityID, bool isreadonly = true)
        {
            using (var cmd = new SqlCommand(@"SELECT  FS.ActivityID ,
                                                      FS.ActivityName ,
                                                      FS.StartDateTime ,
                                                      FS.EndDateTime ,
                                                      FS.CreateDateTime ,
                                                      FS.UpdateDateTime ,
                                                      FS.ActiveType ,
                                                      FS.PlaceQuantity ,
                                                      FS.IsNewUserFirstOrder ,
                                                      FSP.PKID ,
                                                      FSP.PID ,
                                                      FSP.Position ,
                                                      FSP.Price ,
                                                      FSP.TotalQuantity ,
                                                      FSP.MaxQuantity ,
                                                      FSP.SaleOutQuantity ,
                                                      ISNULL(FSP.ProductName, VP.DisplayName) AS ProductName ,
                                                      ISNULL(VP.Image_filename,
                                                             ISNULL(VP.Image_filename_2,
                                                                    ISNULL(VP.Image_filename_3,
                                                                           ISNULL(Image_filename_4, Image_filename_5)))) AS ProductImg ,
                                                      VP.TireSize ,
                                                      VP.CP_ShuXing5 AS AdvertiseTitle ,
                                                      FSP.InstallAndPay ,
                                                      FSP.Level ,
                                                      FSP.ImgUrl ,
                                                      FSP.IsUsePCode ,
                                                      FSP.Channel ,
                                                      FSP.FalseOriginalPrice ,
                                                      FSP.IsJoinPlace ,
                                                      FSP.IsShow,
                                                      FSP.InstallService,
                                                      VP.CP_Brand
                                              FROM    Activity..tbl_FlashSale AS FS WITH ( NOLOCK )
                                                      JOIN Activity..tbl_FlashSaleProducts AS FSP WITH ( NOLOCK ) ON FS.ActivityID = FSP.ActivityID
                                                                                                            AND FS.ActivityID = @ActivityID
                                                      JOIN Tuhu_productcatalog..vw_Products AS VP ON FSP.PID = VP.PID;")
            )
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityID", activityID);
                FlashSaleModel model = new FlashSaleModel();

                var FlashSale = await DbHelper.ExecuteQueryAsync(isreadonly, cmd, dt =>
                {
                    if (dt == null || dt.Rows.Count == 0)
                        return null;
                    model = dt.ConvertTo<FlashSaleModel>()?.FirstOrDefault();
                    model.Products = dt.ConvertTo<FlashSaleProductModel>();
                    return model;
                });
                return FlashSale;
            }
        }

        public static async Task<IEnumerable<FlashSaleProductModel>> SelectFlashSaleSaleOutQuantityAsync(Guid activityId)
            =>
                await DbHelper.ExecuteSelectAsync<FlashSaleProductModel>(true, @"SELECT   FSP.PID ,
                                                                                      FSP.SaleOutQuantity
                                                                             FROM     Activity.dbo.tbl_FlashSaleProducts AS FSP WITH ( NOLOCK )
                                                                             WHERE    FSP.ActivityID = @ActivityID;",
                    CommandType.Text, new SqlParameter("@ActivityID", activityId));

        public static async Task<IEnumerable<Guid>> SelectSecondKillTodayDataAsync(int activityType)
        {
            var model = await DbHelper.ExecuteSelectAsync<FlashSaleModel>(true,
                @"Activity..ActivityService_FlashSale_SelectSecondKillTodayActivityIDs", CommandType.StoredProcedure,
                new SqlParameter("@ActiveType", activityType));
            var query = from d in model
                        select d.ActivityID;
            return query;
        }

        public static Task<int> DeleteFlashSaleRecordsAsync(int orderId)
            => DbHelper.ExecuteNonQueryAsync(
                "DELETE	SystemLog..tbl_FlashSaleRecords WHERE	OrderId = @OrderId",
                CommandType.Text,
                new SqlParameter("@OrderId", orderId));

        public async static Task<IEnumerable<FlashSaleRecordsModel>> SelectFlashSaleRecordsAsync(Guid userId,
            string deviceId, IEnumerable<Guid> ActivityIDs)
            => await DbHelper.ExecuteSelectAsync<FlashSaleRecordsModel>(true, @"SELECT   FSP.ActivityID ,
                                                                                         FSP.PID ,
                                                                                         FSP.Quantity
                                                                               FROM      SystemLog.dbo.tbl_FlashSaleRecords AS FSP WITH ( NOLOCK )
                                                                               WHERE     FSP.ActivityID IN ( SELECT  *
                                                                                                             FROM    Gungnir.dbo.Split(@ActivityIDs, ',') )
                                                                                         AND ( FSP.UserID = @UserID
                                                                                               OR FSP.DeviceID = @DeviceID
                                                                                             );", CommandType.Text,
                new SqlParameter[]
                {
                    new SqlParameter("@ActivityIDs", string.Join(",", ActivityIDs)),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@DeviceID", deviceId)
                });

        public async static Task<IEnumerable<FlashSaleRecordsModel>> SelectActivityProductOrderRecordsAsync(Guid userId,
            string deviceId, Guid ActivityID)
        {
            using (var dbhelper = DbHelper.CreateLogDbHelper(true))
            {
                return await dbhelper.ExecuteSelectAsync<FlashSaleRecordsModel>(@"SELECT   FSP.ActivityID ,
                                                                                         FSP.PID ,
                                                                                         FSP.Quantity
                                                                               FROM      Tuhu_log..ActivityProductOrderRecords AS FSP WITH ( NOLOCK )
                                                                               WHERE     FSP.ActivityID =@ActivityID
                                                                                         AND (OrderStatus = '0New'or OrderStatus is null)
                                                                                         AND ( FSP.UserID = @UserID
                                                                                               OR FSP.DeviceID = @DeviceID
                                                                                             );"
                    , CommandType.Text,
                    new SqlParameter("@ActivityID", ActivityID),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@DeviceID", deviceId));
            }
        }
        public async static Task<IEnumerable<FlashSaleRecordsModel>> SelectAllActivityProductOrderRecordsAsync(Guid userId,
    string deviceId, Guid ActivityID)
        {
            using (var dbhelper = DbHelper.CreateLogDbHelper(true))
            {
                return await dbhelper.ExecuteSelectAsync<FlashSaleRecordsModel>(@"SELECT   FSP.ActivityID ,
                                                                                         FSP.PID ,
                                                                                         FSP.Quantity
                                                                               FROM      Tuhu_log..ActivityProductOrderRecords AS FSP WITH ( NOLOCK )
                                                                               WHERE     FSP.AllPlaceLimitId =@ActivityID
                                                                                         AND (OrderStatus = '0New'or OrderStatus is null)
                                                                                         AND ( FSP.UserID = @UserID
                                                                                               OR FSP.DeviceID = @DeviceID
                                                                                             );"
                    , CommandType.Text,
                    new SqlParameter("@ActivityID", ActivityID),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@DeviceID", deviceId));
            }
        }
        //public static async Task<int> InsertFlashSaleRecordsAsync(OrderItems item, Guid userId, string deviceId, string userTel, int orderId, int orderListId)
        //{
        //    using (var cmd = new SqlCommand(@" INSERT  INTO SystemLog..tbl_FlashSaleRecords
        //                                        ( ActivityID ,
        //                                          PID ,
        //                                          UserID ,
        //                                          Phone ,
        //                                          DeviceID ,
        //                                          OrderId ,
        //                                          OrderListId ,
        //                                          Quantity
        //                                        )
        //                                   values(@ActivityID,
        //                                        @PID,
        //                                        @UserID,
        //                                        @Phone,
        //                                        @DeviceID,
        //                                        @OrderId,
        //                                        @OrderListId,
        //                                        @Quantity)"))
        //    {
        //        cmd.CommandType = CommandType.Text;
        //        cmd.Parameters.AddWithValue("@ActivityID", item.ActivityId);
        //        cmd.Parameters.AddWithValue("@PID", item.PID);
        //        cmd.Parameters.AddWithValue("@UserID", userId);
        //        cmd.Parameters.AddWithValue("@Phone", userTel);
        //        cmd.Parameters.AddWithValue("@DeviceID", deviceId);
        //        cmd.Parameters.AddWithValue("@OrderId", orderId);
        //        cmd.Parameters.AddWithValue("@orderListId", orderListId);
        //        cmd.Parameters.AddWithValue("@Quantity", item.Num);
        //        return await DbHelper.ExecuteNonQueryAsync(cmd);
        //    }

        //}

        public static async Task<int> UpdateFlashSaleProducts(OrderItems item,string allPlaceLimitId=null)
        {
            using (var cmd = new SqlCommand(@" UPDATE  Activity..tbl_FlashSaleProducts WITH ( ROWLOCK )
                                                SET     SaleOutQuantity = SaleOutQuantity - @Num
                                                WHERE   ActivityID = @ActivityID
                                                        AND PID = @PID;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Num", item.Num);
                cmd.Parameters.AddWithValue("@ActivityID", string.IsNullOrEmpty(allPlaceLimitId)?item.ActivityId: new Guid(allPlaceLimitId));
                cmd.Parameters.AddWithValue("@PID", item.PID);
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        public static async Task<int> DeleteActivityProductOrderRecords(int orderId)
        {
            using (var dbhelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(@"UPDATE Tuhu_log..ActivityProductOrderRecords SET OrderStatus = '7Canceled' WHERE OrderId = @OrderId"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    return await dbhelper.ExecuteNonQueryAsync(cmd);
                }
            }
        }

        public static async Task<FlashSaleProductModel> FetchFlashSaleProductModel(OrderItems item)
        {
            using (var cmd = new SqlCommand(@" SELECT FS.ActivityName ,
                                                FS.ActiveType,
                                                FSP.Price ,
                                                FSP.MaxQuantity ,
                                                FS.PlaceQuantity ,
                                                FSP.IsJoinPlace ,
                                                FSP.TotalQuantity ,
                                                FSP.SaleOutQuantity
                                         FROM   Activity..tbl_FlashSale AS FS
                                                WITH ( NOLOCK )
                                                JOIN Activity..tbl_FlashSaleProducts
                                                AS FSP WITH ( NOLOCK ) ON FS.ActivityID = FSP.ActivityID
                                        WHERE   FSP.ActivityID = @ActivityID
                                                AND FSP.PID = @PID;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityID", item.ActivityId);
                cmd.Parameters.AddWithValue("@PID", item.PID);
                return await DbHelper.ExecuteFetchAsync<FlashSaleProductModel>(true, cmd);
            }
        }

        public static async Task<IEnumerable<FlashSaleRecordsModel>> SelectFlashSaleRecordsAsync(Guid userId,
            string deviceId, Guid activityId, string userTel)
            => await DbHelper.ExecuteSelectAsync<FlashSaleRecordsModel>(true, @"SELECT   FSR.ActivityID ,
                                                                                         FSR.PID,
                                                                                         FSR.Quantity,
                                                                                         FSR.OrderId
                                                                               FROM      SystemLog.dbo.tbl_FlashSaleRecords AS FSR WITH ( NOLOCK )
                                                                               WHERE     FSR.ActivityID =@ActivityID
                                                                                         AND ( FSR.UserID = @UserID
                                                                                               OR FSR.DeviceID = @DeviceID
                                                                                               OR FSR.Phone=@UserTel);"
                , CommandType.Text,
                new SqlParameter("@ActivityID", activityId),
                new SqlParameter("@UserID", userId),
                new SqlParameter("@DeviceID", deviceId),
                new SqlParameter("@UserTel", userTel));

        public static async Task<FlashSaleRecordsModel> SelectFlashSaleRecordsByOrderIdAsync(int orderId)
        {
            using (var dbhelper = DbHelper.CreateLogDbHelper(true))
            {
                return await dbhelper.ExecuteFetchAsync<FlashSaleRecordsModel>(@"SELECT  ActivityId
                                                                                FROM    Tuhu_log..ActivityProductOrderRecords
                                                                                WHERE   OrderId = @OrderId
                                                                                        AND (OrderStatus = '0New'or OrderStatus is null);"
                    , CommandType.Text,
                    new SqlParameter("@OrderId", orderId));
            }

        }

        public static async Task<IEnumerable<FlashSaleRecordsModel>> SelectActivityProductOrderRecordsByOrderIdAsync(
            int orderId)
        {
            using (var dbhelper = DbHelper.CreateLogDbHelper(true))
            {
                return await dbhelper.ExecuteSelectAsync<FlashSaleRecordsModel>(@"SELECT  * 
                                                                                FROM    Tuhu_log..ActivityProductOrderRecords WITH (NOLOCK)
                                                                                WHERE   OrderId = @OrderId
                                                                                AND (OrderStatus = '0New'or OrderStatus is null);"
                    , CommandType.Text,
                    new SqlParameter("@OrderId", orderId));
            }

        }

        public static async Task<IEnumerable<int>> SelectFlashSaleOrderIdsByUserAsync(Guid activityId, Guid userId,
            string deviceId, string userTel)
        {
            using (var cmd = new SqlCommand(@"SELECT  FSR.OrderId
                                            FROM    SystemLog.dbo.tbl_FlashSaleRecords AS FSR WITH ( NOLOCK )
                                            WHERE   FSR.ActivityID = @ActivityID
                                                    AND ( FSR.UserID = @UserID
                                                          OR FSR.DeviceID = @DeviceID
                                                          OR FSR.Phone = @UserTel
                                                        );"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityID", activityId);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@DeviceID", deviceId);
                cmd.Parameters.AddWithValue("@UserTel", userTel);

                return await DbHelper.ExecuteQueryAsync(true, cmd, ConvertDt2List<int>);
            }
        }

        public static async Task<IEnumerable<int>> SelectFlashSaleOrderIdsByActivityAsync(Guid activityId,
            DateTime startTime, DateTime endTime)
        {
            using (var cmd = new SqlCommand(@"SELECT  FSR.OrderId
                                            FROM    SystemLog.dbo.tbl_FlashSaleRecords AS FSR WITH ( NOLOCK )
                                            WHERE   FSR.ActivityID = @ActivityID AND FSR.CreateDate >= @StartTime AND FSR.CreateDate < @EndTime;")
            )
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityID", activityId);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);

                return await DbHelper.ExecuteQueryAsync(true, cmd, ConvertDt2List<int>);
            }
        }

        public static async Task<Dictionary<int, string>> SelectOrderStatusByOrderIdsAsync(IEnumerable<int> orderIds)
        {
            using (var cmd = new SqlCommand(@"WITH    OrderIds
          AS ( SELECT   *
               FROM     Gungnir..SplitString(@OrderIds, ',', 1)
             )
    SELECT  o.PKID ,
            o.Status
    FROM    Gungnir..[tbl_Order] AS o WITH ( NOLOCK )
            JOIN OrderIds AS ids ON o.PKID = ids.Item"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@OrderIds", string.Join(",", orderIds));

                return await DbHelper.ExecuteQueryAsync(true, cmd, ConvertDt2Dic<int, string>);
            }
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

        public static Dictionary<TKey, TValue> ConvertDt2Dic<TKey, TValue>(DataTable dt)
        {
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    TKey key = row.IsNull(0) ? default(TKey) : (TKey)row[0];
                    TValue value = row.IsNull(1) ? default(TValue) : (TValue)row[1];
                    result[key] = value;
                }
            }

            return result;
        }


        public static async Task<IEnumerable<string>> SelectFlashSaleActivtyIds()
        {
            using (
                var cmd =
                    new SqlCommand(
                        @"  SELECT  VFS.ActivityId FROM    Activity..tbl_FlashSale VFS WITH ( NOLOCK ) WHERE VFS.Status=1 AND VFS.StartDateTime<=GETDATE() AND VFS.EndDateTime>=GETDATE() ")
            )
            {
                cmd.CommandType = CommandType.Text;
                return await DbHelper.ExecuteQueryAsync(cmd, dt => dt.ToList<string>());
            }
        }


        public static async Task<ActivityPriceModel> FetchGroupBuyingPrice(string Pid, Guid UserId, Guid GroupId,
            ILog Logger)
        {
            var sqlStr = @"
select IIF(@userid = S.OwnerId, P.SpecialPrice, P.FinalPrice) as ActivityPrice,
       P.UseCoupon
from Activity..tbl_GroupBuyingInfo as S with (nolock)
    left join Configuration..GroupBuyingProductConfig as P with (nolock)
        on P.ProductGroupId = S.ProductGroupId
where S.GroupId = @groupid
      and P.PID = @pid;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@userid", UserId);
                cmd.Parameters.AddWithValue("@groupid", GroupId);
                cmd.Parameters.AddWithValue("@pid", Pid);

                return await DbHelper.ExecuteQueryAsync(false, cmd, dt =>
                {
                    var result = new ActivityPriceModel
                    {
                        ApplyCoupon = false,
                        Code = 0,
                        ActivityPrice = 0M,
                        ErrorMsg = "未找到对应拼团价"
                    };
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var ActivityPrice = dt.Rows[0].GetValue<decimal>("ActivityPrice");
                        var ApplyCoupon = dt.Rows[0].GetValue<bool>("UseCoupon");
                        result.Code = 1;
                        result.ActivityPrice = ActivityPrice;
                        result.ApplyCoupon = ApplyCoupon;
                        result.PID = Pid;
                        result.ErrorMsg = "";
                    }
                    else
                    {
                        Logger.Warn($"未找到拼团产品的活动价->{Pid}/{GroupId}");
                    }
                    return result;
                });

                //var val = await DbHelper.ExecuteScalarAsync(cmd);
                //var value = 0M;
                //var result = new ActivityPriceModel
                //{
                //    ApplyCoupon = false
                //};
                //if (decimal.TryParse(val?.ToString(), out value))
                //{
                //    result.Code = 1;
                //    result.ActivityPrice = value;
                //    result.PID = Pid;
                //}
                //else
                //{
                //    result.Code = 0;
                //    result.ActivityPrice = -1M;
                //    result.ErrorMsg = "未找到对应拼团价";
                //    Logger.Warn($"该拼团商品的活动价转换失败，{UserId}/{GroupId:D}/{Pid}");
                //}
                //return result;
            }
        }

        public static async Task<ActivityPriceModel> FetchGroupBuyingPrtoductPrice(Guid activityId, string pid,
            ILog logger)
        {
            var SqlStr = @"
select S.SpecialPrice,
       S.UseCoupon
from Configuration..GroupBuyingProductGroupConfig as T
    left join Configuration..GroupBuyingProductConfig as S with (nolock)
        on T.ProductGroupId = S.ProductGroupId
where T.ActivityId = @activityId
      and S.PID = @Pid
      and T.IsDelete = 0
      and T.BeginTime < GETDATE()
      and T.EndTime > GETDATE();";
            using (var cmd = new SqlCommand(SqlStr))
            {
                cmd.Parameters.AddWithValue("@activityId", activityId);
                cmd.Parameters.AddWithValue("@pid", pid);

                return await DbHelper.ExecuteQueryAsync(false, cmd, dt =>
                {
                    var result = new ActivityPriceModel
                    {
                        ApplyCoupon = false,
                        Code = 0,
                        ActivityPrice = 0M,
                        ErrorMsg = "未找到对应拼团价"
                    };
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var ActivityPrice = dt.Rows[0].GetValue<decimal>("SpecialPrice");
                        var ApplyCoupon = dt.Rows[0].GetValue<bool>("UseCoupon");
                        result.Code = 1;
                        result.ActivityPrice = ActivityPrice;
                        result.ApplyCoupon = ApplyCoupon;
                        result.PID = pid;
                        result.ErrorMsg = "";
                    }
                    else
                    {
                        logger.Warn($"未找到拼团产品的活动价->{pid}/{activityId}");
                    }
                    return result;
                });

                //var result = new ActivityPriceModel
                //{
                //    ApplyCoupon = false
                //};
                //var val = await DbHelper.ExecuteScalarAsync(false, cmd);
                //if (decimal.TryParse(val?.ToString(), out decimal value))
                //{
                //    result.Code = 1;
                //    result.ActivityPrice = value;
                //    result.PID = pid;
                //}
                //else
                //{
                //    result.Code = 0;
                //    result.ActivityPrice = 0M;
                //    result.ErrorMsg = "未找到对应拼团价";
                //    logger.Warn($"该拼团商品的活动价转换失败，{pid}/{activityId}");
                //}
                //return result;
            }
        }


        public static async Task<bool> CheckFreeCouponInfo(Guid activityId, Guid userId)
        {
            const string sqlStr = @"select COUNT(1)
from Activity..tbl_GroupBuyingFreeCoupons with (nolock)
where UserId = @userid
      and OrderId = 0
      and EndDatetime > GETDATE()
      and (   select COUNT(1)
              from Configuration..GroupBuyingProductGroupConfig with (nolock)
              where ActivityId = @activityId
                    and GroupType = 3) > 0;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@activityId", activityId);
                var value = await DbHelper.ExecuteScalarAsync(true, cmd);
                int.TryParse(value?.ToString(), out var result);
                return result > 0;
            }
        }

        public static async Task<FlashSaleModel> SelectFlashSaleModelFromdbAsync(Guid activityId, bool isreadonly = true)
        {
            using (var cmd = new SqlCommand(@"SELECT  FS.ActivityID ,
                                                      FS.ActivityName ,
                                                      FS.StartDateTime ,
                                                      FS.EndDateTime ,
                                                      FS.CreateDateTime ,
                                                      FS.UpdateDateTime ,
                                                      FS.ActiveType ,
                                                      FS.PlaceQuantity ,
                                                      FS.IsNewUserFirstOrder 
                                              FROM    Activity..tbl_FlashSale AS FS WITH ( NOLOCK )
Where FS.ActivityId=@ActivityID"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityID", activityId);
                return await DbHelper.ExecuteFetchAsync<FlashSaleModel>(isreadonly, cmd);
            }


        }
        public async static Task<IEnumerable<FlashSaleRecordsModel>> SelectOrderActivityProductOrderRecordsAsync(Guid userId,
    string deviceId, Guid ActivityID, string userTel)
        {
            using (var dbhelper = DbHelper.CreateLogDbHelper(true))
            {
                return await dbhelper.ExecuteSelectAsync<FlashSaleRecordsModel>(@"SELECT   FSP.ActivityID ,
                                                                                         FSP.PID ,
                                                                                         FSP.Quantity
                                                                               FROM      Tuhu_log..ActivityProductOrderRecords AS FSP WITH ( NOLOCK )
                                                                               WHERE     FSP.ActivityID =@ActivityID
                                                                                         AND (OrderStatus = '0New'or OrderStatus is null)
                                                                                         AND ( FSP.UserID = @UserID
                                                                                               OR FSP.DeviceID = @DeviceID
                                                                                               OR FSP.Phone=@UserTel
                                                                                             );"
                    , CommandType.Text,
                    new SqlParameter("@ActivityID", ActivityID),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@DeviceID", deviceId),
                    new SqlParameter("@UserTel", userTel)
                );

            }
        }

        public static async Task<Tuple<int, int, int>> SelectOrderActivityProductRecordNumAsync(Guid userId,
            string deviceId, Guid activityId, string userTel, List<string> pids)
        {
            using (var dbhelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(@"
                                  WITH    pids
                                  AS ( SELECT   Item COLLATE Chinese_PRC_CI_AS AS PID
                                       FROM     Tuhu_log..SplitString(@Pids,
                                                              ',', 1)
                                     ),
                                records
                                  AS ( SELECT   FSP.Quantity ,
                                                DeviceId ,
                                                FSP.UserId ,
                                                Phone
                                       FROM     Tuhu_log..ActivityProductOrderRecords
                                                AS FSP WITH ( NOLOCK )
                                                JOIN pids ON pids.PID = FSP.Pid
                                       WHERE    FSP.ActivityId = @ActivityID
                                                AND ( OrderStatus = '0New'
                                                      OR OrderStatus IS NULL
                                                    )
                                     )
                            SELECT  SUM(FSP.Quantity) AS q1 ,
                                    'UserId' flag
                            FROM    records AS FSP WITH ( NOLOCK )
                            WHERE   FSP.UserId = @UserID
                            UNION ALL
                            SELECT  SUM(FSP.Quantity) AS q1 ,
                                    'DeviceId' flag
                            FROM    records AS FSP WITH ( NOLOCK )
                            WHERE   FSP.DeviceId = @DeviceID
                            UNION ALL
                            SELECT  SUM(FSP.Quantity) AS q1 ,
                                    'Phone' flag
                            FROM    records AS FSP WITH ( NOLOCK )
                            WHERE   FSP.Phone = @UserTel;"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@ActivityID", activityId);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@DeviceID", deviceId);
                    cmd.Parameters.AddWithValue("@UserTel", userTel);
                    cmd.Parameters.AddWithValue("@Pids", string.Join(",", pids));
                    return await dbhelper.ExecuteQueryAsync(cmd, FuncTabelToTuple);
                }
            }

        }
        public static async Task<Tuple<int, int, int>> SelectAllPlaceOrderActivityProductRecordNumAsync(Guid userId,
    string deviceId, Guid activityId, string userTel, List<string> pids)
        {
            using (var dbhelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(@"
                                  WITH    pids
                                  AS ( SELECT   Item COLLATE Chinese_PRC_CI_AS AS PID
                                       FROM     Tuhu_log..SplitString(@Pids,
                                                              ',', 1)
                                     ),
                                records
                                  AS ( SELECT   FSP.Quantity ,
                                                DeviceId ,
                                                FSP.UserId ,
                                                Phone
                                       FROM     Tuhu_log..ActivityProductOrderRecords
                                                AS FSP WITH ( NOLOCK )
                                                JOIN pids ON pids.PID = FSP.Pid
                                       WHERE    FSP.AllPlaceLimitId = @ActivityID
                                                AND ( OrderStatus = '0New'
                                                      OR OrderStatus IS NULL
                                                    )
                                     )
                            SELECT  SUM(FSP.Quantity) AS q1 ,
                                    'UserId' flag
                            FROM    records AS FSP WITH ( NOLOCK )
                            WHERE   FSP.UserId = @UserID
                            UNION ALL
                            SELECT  SUM(FSP.Quantity) AS q1 ,
                                    'DeviceId' flag
                            FROM    records AS FSP WITH ( NOLOCK )
                            WHERE   FSP.DeviceId = @DeviceID
                            UNION ALL
                            SELECT  SUM(FSP.Quantity) AS q1 ,
                                    'Phone' flag
                            FROM    records AS FSP WITH ( NOLOCK )
                            WHERE   FSP.Phone = @UserTel;"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@ActivityID", activityId);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@DeviceID", deviceId);
                    cmd.Parameters.AddWithValue("@UserTel", userTel);
                    cmd.Parameters.AddWithValue("@Pids", string.Join(",", pids));
                    return await dbhelper.ExecuteQueryAsync(cmd, FuncTabelToTuple);
                }
            }

        }
        private static Tuple<int, int, int> FuncTabelToTuple(DataTable dt)
        {
            var tuple = Tuple.Create(0, 0, 0);
            try
            {

                if (dt == null || dt.Rows.Count == 0)
                    return tuple;
                else
                {
                    var dic = new Dictionary<string, int>();
                    foreach (DataRow row in dt.Rows)
                    {
                        dic.Add(row.GetValue<string>("flag"), row.GetValue<int?>("q1") ?? 0);
                    }
                    //var userCount = dt.Rows[0].GetValue<int?>("q1") ?? 0;
                    //var deviceCount = string.IsNullOrEmpty(deviceId)
                    //    ? userCount
                    //    : dt.Rows[1].GetValue<int?>("q1") ?? 0;
                    //var telCount = dt.Rows[2].GetValue<int?>("q1") ?? 0;

                    var userCount = dic["UserId"];
                    var deviceCount = dic["DeviceId"];
                    var telCount = dic["Phone"];
                    tuple = Tuple.Create(
                        userCount,
                        deviceCount,
                        telCount);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"限时抢购根据设备号，userid，手机号获取用户下单记录，避免未知错误", e);
            }
            return tuple;
        }

        public static async Task<IEnumerable<FlashSaleProductModel>> SelectFlashSaleProductsAsync(Guid activityId)
        {
            using (var cmd = new SqlCommand(@"
        SELECT  *
        FROM    Activity..tbl_FlashSaleProducts AS FSP WITH ( NOLOCK )
        WHERE   FSP.ActivityID = @ActivityID")
            )
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityID", activityId);
                return await DbHelper.ExecuteSelectAsync<FlashSaleProductModel>(true, cmd);
            }
        }
        public static async Task<int> InsertFlashSaleRecordsAsync(OrderItems item, Guid userId, string deviceId, string userTel, int orderId, int orderListId)
        {
            using (var dbhelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(@" INSERT  INTO Tuhu_log.dbo.ActivityProductOrderRecords
                                                ( ActivityID ,
                                                  PID ,
                                                  UserID ,
                                                  Phone ,
                                                  DeviceID ,
                                                  OrderId ,
                                                  Quantity,
                                                  Type,
                                                  OrderStatus
                                                )
                                           values(@ActivityID,
                                                @PID,
                                                @UserID,
                                                @Phone,
                                                @DeviceID,
                                                @OrderId,
                                                @Quantity,
                                                1,
                                                '0New')"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@ActivityID", item.ActivityId);
                    cmd.Parameters.AddWithValue("@PID", item.PID);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Phone", userTel);
                    cmd.Parameters.AddWithValue("@DeviceID", deviceId);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@Quantity", item.Num);
                    return await dbhelper.ExecuteNonQueryAsync(cmd);
                }
            }

        }

        public static async Task<int> UpdateFlashSaleProductsForPlusAsync(OrderItems item)
        {
            using (var cmd = new SqlCommand(@" UPDATE  Activity..tbl_FlashSaleProducts WITH ( ROWLOCK )
                                                SET     SaleOutQuantity = SaleOutQuantity + @Num
                                                WHERE   ActivityID = @ActivityID
                                                        AND PID = @PID;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Num", item.Num);
                cmd.Parameters.AddWithValue("@ActivityID", item.ActivityId);
                cmd.Parameters.AddWithValue("@PID", item.PID);
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        public static async Task<int?> SelectSumSaleoutQuantityfromLogRecordAsync(string activityId, string pid)
        {
            using (var dbhelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(@"SELECT
                                                     SUM(Quantity)
                                                     FROM
                                                     Tuhu_log..ActivityProductOrderRecords
                                                     AS APS
                                                     WHERE
                                                     APS.ActivityId = @ActivityID
                                                     AND APS.Pid = @PID
                                                     AND OrderStatus = '0New'"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@ActivityID", activityId);
                    cmd.Parameters.AddWithValue("@PID", pid);
                    return await dbhelper.ExecuteScalarAsync(cmd) as int?;
                }
            }
        }
        #region 更新配置里的销售数量
        //        public static async Task<int> RefreshAllConfigSaleoutQuantityAsync()
        //        {
        //            using (var dbhelper = DbHelper.CreateLogDbHelper(true))
        //            {
        //                using (var cmd = new SqlCommand(@"WITH    SOURCE
        //          AS ( SELECT   ActivityId ,
        //                        Pid ,
        //                        SUM(Quantity) sq
        //               FROM     Tuhu_log..ActivityProductOrderRecords WITH ( NOLOCK ) WHERE OrderStatus='0New'
        //               GROUP BY ActivityId ,
        //                        Pid
        //             )
        //    SELECT  ActivityId ,
        //            Pid ,
        //            sq
        //    INTO    #temp
        //    FROM    SOURCE 

        //UPDATE  Activity..tbl_FlashSaleProducts 
        //SET     SaleOutQuantity =ISNULL(( SELECT   sq
        //                            FROM    #temp
        //                            WHERE   Activity..tbl_FlashSaleProducts.ActivityID = #temp.ActivityId
        //                                    AND Activity..tbl_FlashSaleProducts.PID = #temp.Pid
        //                          ),0)
        //DROP TABLE #temp;"))
        //                {
        //                    cmd.CommandType = CommandType.Text;
        //                    cmd.Parameters.AddWithValue("@ActivityID", activityId);
        //                    cmd.Parameters.AddWithValue("@PID", pid);
        //                    return Convert.ToInt32(await dbhelper.ExecuteScalarAsync(cmd));
        //                }
        //            }
        //        }

        public static async Task<int> UpdateFlashSaleProductsByLogRecordAsync(string activityId, string pid, int quantity)
        {
            using (var cmd = new SqlCommand(@" UPDATE  Activity..tbl_FlashSaleProducts WITH ( ROWLOCK )
                                                SET     SaleOutQuantity = @Num
                                                WHERE   ActivityID = @ActivityID
                                                        AND PID = @PID;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Num", quantity);
                cmd.Parameters.AddWithValue("@ActivityID", activityId);
                cmd.Parameters.AddWithValue("@PID", pid);
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        public static async Task<List<FlashSaleProductModel>> GetAllValidActvivitysByActivityIdsAsync(List<string> activityids)
        {
            using (var cmd = new SqlCommand(@"
	            SELECT  *  FROM    Activity.dbo.vw_ValidFlashSale vvfs WITH ( NOLOCK ) where ActivityId in (@ActivityIds)"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityIds", string.Join(",", activityids));
                var aa = $"{string.Join(",", activityids)}";
                return (await DbHelper.ExecuteSelectAsync<FlashSaleProductModel>(cmd)).ToList();
            }
        }
        public static async Task<FlashSaleProductModel> GetAllValidActvivitysByActivityIdAndPidAsync(string activityId, string pid)
        {
            using (var cmd = new SqlCommand(@"
	            SELECT  *  FROM    Activity.dbo.vw_ValidFlashSale vvfs WITH ( NOLOCK ) where ActivityId =@ActivityId and Pid=@Pid"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                cmd.Parameters.AddWithValue("@Pid", pid);
                return (await DbHelper.ExecuteFetchAsync<FlashSaleProductModel>(cmd));
            }
        }

        #endregion

        public static DateTime GetActivityStartDateTime(Guid activityId)
        {
            DateTime startTime = DateTime.MinValue;
            using (var db = DbHelper.CreateDbHelper(true))
            {

                string datetimeSql = @"SELECT  top 1 StartDateTime FROM Activity..tbl_FlashSale  WITH ( NOLOCK)
                    where ActivityID = @falshactivityGuid order by StartDateTime desc";
                using (SqlCommand cmd = new SqlCommand(datetimeSql))
                {
                    cmd.Parameters.AddWithValue("@falshactivityGuid", activityId);
                    Object obj = DbHelper.ExecuteScalar(true, cmd);
                    if (obj != null && DateTime.TryParse(obj.ToString(), out startTime))
                    {

                    }
                }
            }
            return startTime;
        }



        public static int InsertEveryDaySeckillUserInfo(EveryDaySeckillUserInfo model,out string msg)
        {

            msg = "";
           
            DateTime startTime = GetActivityStartDateTime(model.FlashActivityGuid);
            model.FlashActivityStartTime = startTime;
            if (startTime == DateTime.MinValue)
            {
                msg = "FlashActivityGuid不存在";
                return -2;
            }
            string sql = @"
        IF NOT EXISTS ( SELECT *
                        FROM   [Configuration].[dbo].[SE_EveryDaySeckillUserInfo] SKUI
                        WHERE   SKUI.UserId = @UserId
                                AND SKUI.FlashActivityGuid  = @FlashActivityGuid                    
                                AND SKUI.Pid = @Pid    
                                AND SKUI.FlashActivityStartTime = @FlashActivityStartTime
                                AND SKUI.Type=@Type)
            BEGIN
                INSERT  INTO [Configuration].[dbo].[SE_EveryDaySeckillUserInfo]
                        ( [UserId],[MobilePhone],[FlashActivityGuid],[Pid],[FlashActivityStartTime],[Type]   
                        )
                VALUES  ( @UserId ,
                          @MobilePhone,
                          @FlashActivityGuid ,
                          @Pid ,
                          @FlashActivityStartTime,
                          @Type
                        );
            END;";


            try
            {



                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@MobilePhone", model.MobilePhone);
                    cmd.Parameters.AddWithValue("@FlashActivityGuid", model.FlashActivityGuid);
                    cmd.Parameters.AddWithValue("@Pid", model.Pid);
                    cmd.Parameters.AddWithValue("@FlashActivityStartTime", startTime);
                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    int i= DbHelper.ExecuteNonQuery(cmd);
                    return i;
                }


            }
            catch (Exception ex)
            {
                 msg = ex.Message;

                return -2;
            }
        }


        public static IEnumerable<EveryDaySeckillUserInfo> GetEveryDaySeckillUserInfoByUserId(EveryDaySeckillUserInfo model, DateTime datetime)
        {

            const string sql = @" 
                                      SELECT    [UserId],[FlashActivityGuid],[Pid],[FlashActivityStartTime]     
                                             FROM [Configuration].[dbo].[SE_EveryDaySeckillUserInfo] WITH (NOLOCK) WHERE UserId=@UserId AND FlashActivityGuid=@FlashActivityGuid AND Pid=@Pid AND Type=@Type AND FlashActivityStartTime>=@datetime";




            var sqlParams = new SqlParameter[]
               {

                        new SqlParameter("@UserId",model.UserId),
                        new SqlParameter("@FlashActivityGuid",model.FlashActivityGuid),
                        new SqlParameter("@Pid",model.Pid),
                        new SqlParameter("@Type",model.Type),
                        new SqlParameter("@datetime",datetime)
               };
            return DbHelper.ExecuteSelect<EveryDaySeckillUserInfo>(true, sql, CommandType.Text, sqlParams);
        }

        public static async Task<IEnumerable<Guid>> SelectSecondKillTodayDataSqlAsync(int activityType,DateTime dt)
        {
            var model = await DbHelper.ExecuteSelectAsync<FlashSaleModel>(true,
                @"        SELECT  FS.ActivityID
        FROM    Activity..tbl_FlashSale AS FS WITH ( NOLOCK )
        WHERE   FS.ActiveType = @ActiveType
                AND FS.StartDateTime >= CONVERT(VARCHAR(100), CONVERT(DATE, @Dt), 23)
                AND FS.StartDateTime <= CONVERT(VARCHAR(100), DATEADD(DAY, 1,
                                                              CONVERT(DATE, @Dt)), 23)
                AND FS.EndDateTime >= CONVERT(VARCHAR(100), CONVERT(DATE, @Dt), 23)
                AND FS.EndDateTime <= CONVERT(VARCHAR(100), DATEADD(DAY, 1,
                                                              CONVERT(DATE, @Dt)), 23);", 
                CommandType.Text,
                new SqlParameter[]
                {
                    new SqlParameter("@ActiveType", activityType),
                    new SqlParameter("@Dt", dt)
                });
            var query = from d in model
                        select d.ActivityID;
            return query;
        }
        public static async Task<string> SelectDefultActivityIdBySchedule(string schedule,bool isreadonly)
        {

                var datetime = Convert.ToDateTime("2018-04-26");
                var strDate = "";
                var endDate = "";
                switch (schedule)
                {
                    case "10点场":
                        strDate = datetime.ToString("yyyy-MM-dd 10:00:0");
                        endDate = datetime.ToString("yyyy-MM-dd 13:00:0");
                        break;
                    case "13点场":
                        strDate = datetime.ToString("yyyy-MM-dd 13:00:00");
                        endDate = datetime.ToString("yyyy-MM-dd 16:00:0");
                        break;
                    case "16点场":
                        strDate = datetime.ToString("yyyy-MM-dd 16:00:00");
                        endDate = datetime.ToString("yyyy-MM-dd 20:00:0");
                        break;
                    case "20点场":
                        strDate = datetime.ToString("yyyy-MM-dd 20:00:00");
                        endDate = datetime.ToString("yyyy-MM-dd 23:59:59");
                        break;
                    case "0点场":
                        strDate = datetime.ToString("yyyy-MM-dd 00:00:00");
                        endDate = datetime.ToString("yyyy-MM-dd 10:00:0");
                        break;
                }
                var whereCondition = $"where EndDateTime<='{endDate}' AND StartDatetime>='{strDate}'";
                var sql = $@"SELECT A.ActivityId FROM Activity..tbl_FlashSale AS A WITH ( NOLOCK) 
		JOIN Activity..tbl_FlashSaleProducts AS B WITH(NOLOCK) ON B.ActivityID = A.ActivityID {whereCondition}AND IsDefault=1";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType=CommandType.Text;
                return (await DbHelper.ExecuteScalarAsync(isreadonly,cmd)).ToString();
            } ;
        }

        public static async Task<string> SelectActivityProductOrderRecordAsync(int orderId,string pid)
        {
            using (var dbhelper = DbHelper.CreateLogDbHelper(true))
            {
                return (await dbhelper.ExecuteScalarAsync(@"SELECT  AllPlaceLimitId
                                                                                FROM    Tuhu_log..ActivityProductOrderRecords WITH (NOLOCK)
                                                                                WHERE   OrderId = @OrderId AND Pid=@Pid
                                                                                AND (OrderStatus = '0New'or OrderStatus is null);"
                    , CommandType.Text,
                    new SqlParameter("@OrderId", orderId),
                    new SqlParameter("@Pid", pid))).ToString();
            }
        }

        public static async Task<IEnumerable<DalSeckillScheudleInfoModel>> GetDalSeckillScheudleInfoModels(List<string> pids,
            DateTime sdateTime, DateTime eDateTime)
        {
            using (var cmd=new SqlCommand(@"   
              WITH    pids
              AS ( SELECT   *
                   FROM     Activity..SplitString(@Pids, ',', 1)
                 )
        SELECT  PID ,
                tfsp.ActivityID ,
                tfs.StartDateTime ,
                tfs.EndDateTime ,
                tfsp.Price AS SeckillPrice
        FROM    Activity..tbl_FlashSaleProducts AS tfsp WITH ( NOLOCK )
                JOIN Activity..tbl_FlashSale AS tfs ON tfs.ActivityID = tfsp.ActivityID
                JOIN pids ON tfsp.PID = pids.Item
        WHERE   tfs.StartDateTime >= @sSchedule
                AND tfs.EndDateTime <= @eSchedule
                AND tfs.ActiveType = 1 And IsDefault=0"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@sSchedule", sdateTime);
                cmd.Parameters.AddWithValue("@eSchedule", eDateTime);
                cmd.Parameters.AddWithValue("Pids", string.Join(",", pids));
                return await DbHelper.ExecuteSelectAsync<DalSeckillScheudleInfoModel>(true, cmd);
            }
        }
    }
}
