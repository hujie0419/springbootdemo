using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Models;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess.Models;
using Tuhu.Service.Activity.DataAccess.Models.Activity;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Models.Requests.Activity;
using Newtonsoft.Json;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalActivity
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalActivity));
        private static string ReadonlyFlagClientName = "ReadonlyFlagClientName";
        public static async Task<DownloadApp> GetActivityConfigForDownloadApp(int id)
        {
            string sql = @"SELECT TOP 1 *  FROM [Tuhu_huodong].[dbo].[DownloadApp] WITH(NOLOCK) WHERE Id = @id";
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", id);
                    return (await dbHelper.ExecuteSelectAsync<DownloadApp>(cmd)).FirstOrDefault();
                }
            }

        }
        public static async Task<IEnumerable<TireActivityModel>> SelectTireActivity(string vehicleId, string tireSize, bool flag = true)
        {
            return await DbHelper.ExecuteSelectAsync<TireActivityModel>(flag, @"SELECT	* FROM	Activity.dbo.tbl_TireListActivity AS TLA WITH (NOLOCK) WHERE	TLA.Status = 1 AND EndTime > GETDATE() AND VehicleID = @VehicleID AND TireSize = @TireSize",
                CommandType.Text,
                new SqlParameter("@VehicleID", vehicleId),
                new SqlParameter("@tireSize", tireSize));
        }
        public static async Task<TireActivityModel> SelectTireActivityByActivityId(Guid activityId)
        {
            return await DbHelper.ExecuteFetchAsync<TireActivityModel>(true, @"SELECT TOP 1 * FROM	Activity.dbo.tbl_TireListActivity AS TLA WITH (NOLOCK) WHERE	TLA.Status = 1 AND EndTime > GETDATE() AND ActivityId=@ActivityId",
                CommandType.Text,
                new SqlParameter("@ActivityId", activityId));
        }

        public static async Task<IEnumerable<string>> SelectTireActivityPids(Guid activityId)
        {
            using (var dbHelper = DbHelper.CreateDbHelper(true))
            using (var cmd = dbHelper.CreateCommand(@"SELECT PID FROM Activity.dbo.tbl_TireListActivityProducts AS TLAP WITH (NOLOCK) WHERE  ActivityID = @ActivityID  ORDER BY Postion"))
            {
                cmd.Parameters.Add(new SqlParameter("@ActivityID", activityId));
                return await dbHelper.ExecuteQueryAsync(cmd, dt => dt.ToList<string>());
            }
        }

        public static async Task<IEnumerable<VehicleAdaptTireModel>> SelectVehicleAdaptTireAsync(VehicleAdaptTireRequestModel request, string tireSize)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(@"Select PKID,TireSize,VehicleId,PID,SalesOrder,CreatedTime,UpdatedTime from Tuhu_bi..VehicleAdaptTire with(nolock)
                                             Where VehicleId=@VehicleId and TireSize=@TireSize
                                             Order By SalesOrder
                                             OFFSET (@PageNumber - 1) * @PageSize ROW
				                             FETCH NEXT @PageSize ROW ONLY;"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@VehicleId", request.VehicleId);
                    cmd.Parameters.AddWithValue("@PageNumber", request.CurrentPage);
                    cmd.Parameters.AddWithValue("@PageSize", request.PageSize);
                    cmd.Parameters.AddWithValue("@TireSize", tireSize);
                    return await dbHelper.ExecuteSelectAsync<VehicleAdaptTireModel>(cmd);
                }
            }

        }


        public static async Task<IEnumerable<VehicleAdaptBaoyangModel>> SelectVehicleAdaptBaoyangAsync(string vehicleId)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(@"Select PKID,VehicleId,MinPrice,SalesOrder,Image,BaoyangType,MobileLine,IProcessValue,AProcessValue,ICommunicationValue,ACommunicationValue from Tuhu_bi..VehicleAdaptBaoyang with(nolock)
                                             Where VehicleId=@VehicleId
                                             Order By SalesOrder ;"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@VehicleId", vehicleId);
                    return await dbHelper.ExecuteSelectAsync<VehicleAdaptBaoyangModel>(cmd);
                }
            }
        }

        public static async Task<IEnumerable<VehicleAdaptChepinModel>> SelectVehicleAdaptChepinAsync(string vehicleId)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(@"Select PKID,VehicleId,CategoryName,PID,SalesOrder from Tuhu_bi..VehicleAdaptChepin with(nolock)
                                                 Where VehicleId=@VehicleId
                                                 Order By SalesOrder ;"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@VehicleId", vehicleId);
                    return await dbHelper.ExecuteSelectAsync<VehicleAdaptChepinModel>(cmd);
                }
            }
        }
        public static async Task<IEnumerable<string>> SelectVehicleSortedCategoryNamesAsync()
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(@"Select CategoryName from Tuhu_bi..SortedChepinCategoryName with(nolock)
                                                 order by NameOrder ;"))
                {
                    cmd.CommandType = CommandType.Text;
                    var result = await dbHelper.ExecuteQueryAsync(cmd, dt => dt.ToList<string>());
                    return result;
                }
            }
        }


        public async static Task<IEnumerable<CarTagCouponConfigModel>> SelectCarTagCouponConfigsAsync()
        {
            using (var cmd = new SqlCommand(@"SELECT ID,CouponGuid,Discount,Description,MinMoney,Status,CreateDate,StartDateTime,EndDateTime,Name,ImageUrl FROM Configuration.dbo.SE_CarTagCouponConfig with(nolock)
                                             where Status=1"))
            {
                cmd.CommandType = CommandType.Text;
                return await DbHelper.ExecuteSelectAsync<CarTagCouponConfigModel>(cmd);

            }
        }

        public async static Task<IEnumerable<VehicleSortedTireSizeModel>> SelectVehicleSortedTireSizesAsync(string vehicleId)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(@"Select PKID,TireSize,VehicleId,HistorySaleOrder from Tuhu_bi..VehicleSortedTireSize with(nolock)
                                            where VehicleId=@VehicleId
                                            order by HistorySaleOrder "))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@VehicleId", vehicleId);
                    return await dbHelper.ExecuteSelectAsync<VehicleSortedTireSizeModel>(cmd);
                }
            }

        }

        public async static Task<int> InsertUserShareInfoAsyncAsync(Guid shareId, string pid, Guid batchGuid, Guid userId)
        {
            using (var cmd = new SqlCommand(@"insert into Activity..ActivityUserShareInfo (ShareId,UserId,Times,PID,CreatedTime,BatchId) select @ShareId,@UserId,Times,PID,GETDATE(),@BatchGuid
                                            from  Configuration..SE_ShareMakeImportProducts with(nolock) where PID=@PID and BatchGuid=@BatchGuid"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ShareId", shareId);
                cmd.Parameters.AddWithValue("@PID", pid);
                cmd.Parameters.AddWithValue("@BatchGuid", batchGuid);
                cmd.Parameters.AddWithValue("@UserId", userId);
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        public async static Task<ActivityUserShareInfoModel> GetActivityUserShareInfoAsync(Guid shareId)
        {
            using (
                var cmd = new SqlCommand(@"Select ShareId,UserId,PID,Times,CreatedTime,BatchId,ConfigGuid from Activity..ActivityUserShareInfo with(nolock)
                                            where ShareId=@ShareId"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ShareId", shareId);
                return await DbHelper.ExecuteFetchAsync<ActivityUserShareInfoModel>(cmd);

            }
        }

        public async static Task<IEnumerable<PromotionPacketHistoryModel>> SelectPromotionPacketHistoryAsync(Guid userId, Guid luckyWheel)
        {
            using (var cmd = new SqlCommand("SELECT PKID,UserID,EntityID,IsGet,Type,CreateDateTime FROM SystemLog.dbo.tbl_PromotionPacketHistory AS PH WITH(NOLOCK) WHERE PH.UserID = @UserID AND PH.CreateDateTime>=@Time AND LuckyWheel=@LuckyWheel "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@Time", DateTime.Now.Date);
                cmd.Parameters.AddWithValue("@LuckyWheel", luckyWheel);
                return await DbHelper.ExecuteSelectAsync<PromotionPacketHistoryModel>(cmd);
            }
        }

        public async static Task<int> GetGuidAndInsertUserForShareAsync(Guid shareId, Guid configGuid, Guid userId)
        {
            using (var cmd = new SqlCommand(@"insert into Activity..ActivityUserShareInfo (ShareId,UserId,CreatedTime,ConfigGuid) values(@ShareId,@UserId,GETDATE(),@ConfigGuid)"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ShareId", shareId);
                cmd.Parameters.AddWithValue("@ConfigGuid", configGuid);
                cmd.Parameters.AddWithValue("@UserId", userId);
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        public static async Task<RecommendGetGiftConfigModel> FetchRecommendGetGiftConfigAsync(Guid? number)
        {
            const string sql =
                @"Select top 1 Id,Number,Name,Banner,AwardLimit,AwardType,AwardValue,RegisteredText,AwardedText,GetRuleGUID,ShareButtonValue,ShareChannel,Rules,TimeLimitCollectRules,CreateName,CreateTime,UpdateName,UpdateTime,TabName  from Configuration..SE_RecommendGetGiftConfig with(nolock)
                                                where (@Number is null or Number=@Number) {0}
                                                order by CreateTime desc";
            string ExcuteSql = string.Format(sql, (number == null || number == Guid.Empty) ? " and ([IsSendCode] is NULL Or [IsSendCode]=0) " : " ");
            //string ExcuteSql = string.Format(sql," ");
            using (var cmd = new SqlCommand(ExcuteSql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Number", number);
                return await DbHelper.ExecuteFetchAsync<RecommendGetGiftConfigModel>(true, cmd);
            }
        }


        public async static Task<DataTable> SelectPacketByUsersAsync()
        {
            using (var cmd = new SqlCommand(@"SELECT DISTINCT PPN.*,'135****1255' AS u_last_name,PC.Discount,PC.PromtionName	FROM
	                       ( SELECT TOP 20 PKID,PacketSettingID,UserID,CreateTime,Channel FROM Gungnir..tbl_PromotionPacketNew WITH(NOLOCK)
	                         WHERE Channel IN ('H5','Wechat') ORDER BY  CreateTime DESC)  AS PPN JOIN Gungnir..tbl_PromotionCode PC WITH(NOLOCK)
	                         ON PPN.UserID=PC.UserId AND PPN.PacketSettingID=PC.GetRuleID  ORDER BY PPN.CreateTime DESC"))
            {
                cmd.CommandType = CommandType.Text;
                return (DataTable)await DbHelper.ExecuteSelectAsync<DataTable>(true, cmd);
            }
        }
        public async static Task<RegionActivityPageModel> GetActivityPageUrlAsync(string city, string activityId)
        {
            using (var cmd = new SqlCommand(@"
                    SELECT TOP 1
                            a.Id ,
                            Url ,
                            ( CASE WHEN GETDATE() < d.StartTime THEN 1
                                   WHEN GETDATE() > d.StartTime
                                        AND GETDATE() < d.EndTime THEN 2
                                   WHEN GETDATE() > d.EndTime THEN 3
                              END ) AS Code
                    FROM    Configuration..SE_ActivityPageUrlConfig AS a
                            INNER JOIN Configuration..SE_ActivityPageConfig AS d ON d.Id = a.ActivityPageId
                    WHERE   ActivityPageId = @ActivityId
                            AND a.Id IN (
                            SELECT DISTINCT
                                    c.UrlId
                            FROM    Configuration..SE_ActivityPageRegionConfig AS c
                            WHERE   c.City = @city )
                    ORDER BY a.Id; "))
            {
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@city", city);
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                return await DbHelper.ExecuteFetchAsync<RegionActivityPageModel>(true, cmd);
            }
        }

        public async static Task<RegionActivityPageModel> GetActivityPageDefaultUrlAsync(string city, string activityId)
        {
            using (var cmd = new SqlCommand(@"
                    SELECT TOP 1
                            a.Id ,
                            Url ,
                            d.ShareParameters,
							d.Name,
                            ( CASE WHEN GETDATE() < d.StartTime THEN 1
                                   WHEN GETDATE() > d.StartTime
                                        AND GETDATE() < d.EndTime THEN 2
                                   WHEN GETDATE() > d.EndTime THEN 3
                              END ) AS Code
                    FROM    Configuration..SE_ActivityPageUrlConfig AS a
                            INNER JOIN Configuration..SE_ActivityPageConfig AS d ON d.Id = a.ActivityPageId
                    WHERE    a.Id IN ( SELECT b.DefaultUrlId
                                         FROM   Configuration..SE_ActivityPageConfig AS b
                                         WHERE  Id = @ActivityId ) "))
            {
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@city", city);
                cmd.Parameters.AddWithValue("@ActivityId", int.TryParse(activityId, out int id) ? id : 0);
                return await DbHelper.ExecuteFetchAsync<RegionActivityPageModel>(true, cmd);
            }
        }

        #region 分车型分地区获取活动链接

        public async static Task<RegionVehicleIdActivityConfig> SelectActivityType(Guid activityId)
        {
            var sql = @" SELECT  config.ActivityId ,
                                config.ActivityType ,
                                config.StartTime ,
                                config.EndTime
                        FROM    Configuration..RegionVehicleIdActivityConfig AS config WITH ( NOLOCK )
                        WHERE   config.ActivityId = @ActivityId
                                AND config.IsEnabled = 1;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                var result = await DbHelper.ExecuteFetchAsync<RegionVehicleIdActivityConfig>(true, cmd);
                return result;
            }
        }

        public async static Task<List<RegionVehicleIdActivityUrlConfig>> SelectRegionVehicleIdActivityConfigsByactivityId(Guid activityId)
        {
            var sql = @"SELECT  config.TargetUrl ,
                                config.IsDefault ,
                                config.RegionId ,
                                config.VehicleId ,
                                config.WxappUrl
                        FROM    Configuration..RegionVehicleIdActivityUrlConfig AS config WITH ( NOLOCK )
                        WHERE   config.ActivityId = @ActivityId;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                var result = await DbHelper.ExecuteSelectAsync<RegionVehicleIdActivityUrlConfig>(true, cmd);
                return result.ToList();
            }
        }

        #endregion

        public async static Task<IEnumerable<SendCodeForUserGroupModel>> GetSendCodeForUserGroup(Guid userId)
        {
            const string sql = @"SELECT [ID]
      ,[GroupId]
      ,[UserId]
      ,[SendCode]
      ,[GetUserId]
      ,[GetUserName]
      ,[CreateDateTime]
      ,[UpdateDateTime]
      ,[UserPhone]
  FROM [Configuration].[dbo].[SE_SendCodeForUserGroup] with(nolock)
  WHERE UserId=@UserId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                return await DbHelper.ExecuteSelectAsync<SendCodeForUserGroupModel>(true, cmd);
            }
        }
        public async static Task<RecommendGetGiftConfigModel> FetchRecommendGetGiftConfigAsync(int groupId)
        {
            using (var cmd = new SqlCommand(@"Select top 1 StartTime,EndTime,IsSendCode,Id,Number,Name,Banner,AwardLimit,AwardType,AwardValue,RegisteredText,AwardedText,GetRuleGUID,ShareButtonValue,ShareChannel,Rules,TimeLimitCollectRules,CreateName,CreateTime,UpdateName,UpdateTime,TabName  from Configuration..SE_RecommendGetGiftConfig with(nolock)
                                                where userGroupId=@userGroupId and  ((StartTime<getdate() and EndTime>getdate()) or (StartTime is null  and EndTime is null))
                                                order by CreateTime desc"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@userGroupId", groupId);
                return await DbHelper.ExecuteFetchAsync<RecommendGetGiftConfigModel>(true, cmd);
            }
        }

        public static List<Tuple<string, DateTime, DateTime>> GetStartAndEndTimes(List<string> Codes)
        {
            List<Tuple<string, DateTime, DateTime>> result = null;
            const string sql = @" Select ExChangeStartTime,ExChangeEndTime,Code　from  Activity..tbl_ExchangeCode AS EC with(nolock) left join  Activity..tbl_ExchangeCodeDetail as ECD WITH ( NOLOCK )
 ON EC.DetailsID=ECD.PKID
 WHERE EC.Code IN('{0}') AND EC.Status=0";
            Func<DataTable, List<Tuple<string, DateTime, DateTime>>> action = delegate (DataTable dt)
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    result = new List<Tuple<string, DateTime, DateTime>>();
                    foreach (DataRow argRow in dt.Rows)
                    {
                        result.Add(Tuple.Create(argRow.GetValue<string>("Code"), argRow.GetValue<DateTime>("ExChangeStartTime"),
                            argRow.GetValue<DateTime>("ExChangeEndTime")));
                    }
                }
                return result;
            };
            string sqlResult = string.Format(sql, string.Join("','", Codes));
            using (var cmd = new SqlCommand(sqlResult))
            {
                return DbHelper.ExecuteQuery(true, cmd, action);
            }
        }

        public static async Task<ActivePageListModel> FetchActivePageListModelasync(int id, string hashKey)
        {
            using (var cmd = new SqlCommand(@"
                                            SELECT  Title ,
                                                    H5Uri ,
                                                    WWWUri ,
                                                    BgImageUrl ,
                                                    BgColor ,
                                                    TireBrand ,
                                                    ActivityType ,
                                                    DataParames ,
                                                    MenuType ,
                                                    IsShowDate ,
                                                    SelKeyImage ,
                                                    SelKeyName ,
                                                    IsTireSize ,
                                                    StartDate ,
                                                    EndDate ,
                                                    CustomerService ,
                                                    IsNeedLogIn,
		                                            FloatWindow,
		                                            FloatWindowImageUrl,
		                                            AlertTabImageUrl,
		                                            AlertJumpApp,
		                                            AlertJumpWxApp,
		                                            FloatWindowJump
                                            FROM    Configuration.dbo.ActivePageList WITH ( NOLOCK ) where PKID=@ID Or hashKey=@hashKey"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@hashKey", hashKey);
                return await DbHelper.ExecuteFetchAsync<ActivePageListModel>(true, cmd);
            }
        }
        public static async Task<int> FetchActivePageListModelIdasync(string hashKey)
        {
            using (var cmd = new SqlCommand(@"select PKid from Configuration.dbo.ActivePageList with(nolock) where  hashKey=@hashKey"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@hashKey", hashKey);
                return Convert.ToInt32(await DbHelper.ExecuteScalarAsync(true, cmd));
            }
        }

        public static async Task<IEnumerable<ActivePageHomeWithDetailModel>> SelectActivePageHomeWithDetailModels(int id)
        {
            using (var cmd = new SqlCommand(@"SELECT  A.PKID ,
        FKActiveID ,
        BigHomeName ,
        HidBigHomePic ,
        BigHomeUrl ,
        HidBigHomePicWww ,
        BigHomeUrlWww ,
        Sort ,
        IsHome ,
        AD.PKID AS DetailPkid ,
        FKActiveHome ,
        HomeName ,
        HidBigFHomePic ,
        BigFHomeMobileUrl ,
        BigFHomeWwwUrl ,
        BigFHomeOrder ,
        HidBigHomePicWxApp ,
        BigHomeUrlWxApp ,
        BigFHomeWxAppUrl,
        BackgroundColor
FROM    Configuration.dbo.ActivePageHome A WITH ( NOLOCK )
        LEFT JOIN Configuration.dbo.ActivePageHomeDeatil AD WITH ( NOLOCK ) ON A.PKID = AD.FKActiveHome
WHERE   FKActiveID = @ID
ORDER BY A.Sort ,
        AD.BigFHomeOrder;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                return await DbHelper.ExecuteSelectAsync<ActivePageHomeWithDetailModel>(true, cmd);
            }
        }

        public static async Task<ActivePageTireSizeConfigModel> FetchActivePageTireSizeConfigModel(int id)
        {
            using (var cmd = new SqlCommand(@"select * from Configuration.dbo.ActivePageTireSizeConfig with(nolock)
                                        where FKActiveID=@ID"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                return await DbHelper.ExecuteFetchAsync<ActivePageTireSizeConfigModel>(true, cmd);
            }
        }

        public static async Task<IEnumerable<ActivePageContentModel>> SelectActivePageContents(int id, string channel)
        {
            using (var cmd = new SqlCommand(@"select * from Configuration.DBO.ActivePageContent with(nolock) where FKActiveID=@ID AND (channel=@channel or channel='all') order by OrderBY"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@channel", channel);
                return await DbHelper.ExecuteSelectAsync<ActivePageContentModel>(true, cmd);
            }
        }

        public static async Task<IEnumerable<ActivePageMenuModel>> SelectActivePageMenus(int contentId)
        {
            using (var cmd = new SqlCommand(@"select FKActiveContentID,MenuName,MenuValue,MenuValueEnd,Sort,Color,Description from Configuration.dbo.ActivePageMenu with(nolock) where FKActiveContentID=@ContentId order by Sort"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ContentId", contentId);
                return await DbHelper.ExecuteSelectAsync<ActivePageMenuModel>(true, cmd);
            }
        }

        public static async Task<IEnumerable<ActivePageMenuModel>> SelectActivePageMenus(List<int> contentIds)
        {
            using (var cmd = new SqlCommand(@"select FKActiveContentID,MenuName,MenuValue,MenuValueEnd,Sort,Color,Description from Configuration.dbo.ActivePageMenu with(nolock) where FKActiveContentID in (SELECT  *
                                                                                                             FROM    Gungnir.dbo.Split(@ContentIds, ',')) order by Sort"))
            {
                cmd.CommandType = CommandType.Text;
                var aa = string.Join(",", contentIds);
                cmd.Parameters.AddWithValue("@ContentIds", string.Join(",", contentIds));
                return await DbHelper.ExecuteSelectAsync<ActivePageMenuModel>(true, cmd);
            }
        }

        public static async Task<LuckyWheelUserlotteryCountModel> GetLuckyWheelUserlotteryCountAsync(Guid userId, Guid userGroup, string hashKey)
        {
            string sql = "select top 1 [Count],Record from Tuhu_Log..LuckyWheelUserlotteryCount with(nolock) where userId=@UserId";
            if (userGroup != Guid.Empty)
            {
                sql += " AND UserGroup=@UserGroup ";
            }
            if (!string.IsNullOrEmpty(hashKey))
            {
                sql += " AND HashKey=@HashKey ";
            }
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@UserGroup", userGroup);
                    cmd.Parameters.AddWithValue("@HashKey", hashKey);
                    return await dbHelper.ExecuteFetchAsync<LuckyWheelUserlotteryCountModel>(cmd);
                }
            }
        }
        public static async Task<int> UpdateLuckyWheelUserlotteryCountAsync(Guid userId, Guid userGroup, string hashKey)
        {
            string sql = "update Tuhu_Log..LuckyWheelUserlotteryCount WITH(ROWLOCK) set Record=Record + 1 where UserId= @UserId";
            if (userGroup != Guid.Empty)
            {
                sql += " AND UserGroup=@UserGroup ";
            }
            if (!string.IsNullOrEmpty(hashKey))
            {
                sql += " AND HashKey=@HashKey ";
            }
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@UserGroup", userGroup);
                    cmd.Parameters.AddWithValue("@HashKey", hashKey);
                    return Convert.ToInt32(await dbHelper.ExecuteNonQueryAsync(cmd));
                }
            }
        }
        public static LuckyWheelModel GetLuckyWheelWithDetail(string id)
        {
            using (var cmd = new SqlCommand(@"SELECT * FROM Activity.dbo.LuckyWheel LW with(NOLOCK)
                                            join Activity.dbo.LuckyWheelDeatil LWD with(NOLOCK)  on LW.ID =LWD.FKLuckyWheelID
                                            where LW.ID= @ID "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                LuckyWheelModel model;
                var luckyWheel = DbHelper.ExecuteQuery(true, cmd, dt =>
                {
                    if (dt == null || dt.Rows.Count == 0)
                        return null;
                    model = dt.ConvertTo<LuckyWheelModel>()?.FirstOrDefault();
                    if (model != null)
                        model.Items = dt.ConvertTo<LuckyWheelDeatil>().ToList();
                    return model;
                });
                return luckyWheel;
            }
        }

        public static async Task<IEnumerable<LuckyWheelDeatil>> GetLuckyWheelDetail(string id)
        {
            using (var cmd = new SqlCommand(@"SELECT * FROM Activity.dbo.LuckyWheelDeatil (NOLOCK) WHERE FKLuckyWheelID=@ID "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                return await DbHelper.ExecuteSelectAsync<LuckyWheelDeatil>(true, cmd);
            }
        }

        public static async Task<TiresOrderRecordModel> SelectTiresOrderRecordByOrderId(int orderId)
        {
            using (var cmd = new SqlCommand("SELECT Pkid,OrderId, Number, AddressPhone, UserPhone, DeviceId, UserIp, IsRevoke, CreateDateTime AS CreateTime,LastUpdateDateTime AS LastUpdateTime FROM Activity..tbl_TiresOrderRecord AS TOR WITH(NOLOCK) WHERE TOR.OrderId = @OrderId AND TOR.IsRevoke = 0"))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                return await DbHelper.ExecuteFetchAsync<TiresOrderRecordModel>(true, cmd);
            }
        }
        //public static async Task<IEnumerable<TiresOrderRecordModel>> SelectTiresOrderRecordByUserPhone(string userPhone,DateTime? time=null)
        //{
        //    using (var cmd = new SqlCommand("SELECT  Pkid,OrderId, Number, AddressPhone, UserPhone, DeviceId, UserIp, IsRevoke, CreateDateTime AS CreateTime,LastUpdateDateTime AS LastUpdateTime FROM    Activity..tbl_TiresOrderRecord AS TOR WITH(NOLOCK) WHERE   TOR.UserPhone = @UserPhone  AND TOR.IsRevoke = 0  AND TOR.CreateDateTime >= @Time; "))
        //    {
        //        cmd.Parameters.AddWithValue("@UserPhone", userPhone);
        //        cmd.Parameters.AddWithValue("@Time", time??DateTime.Now.Date);
        //        return await DbHelper.ExecuteSelectAsync<TiresOrderRecordModel>(true, cmd);
        //    }
        //}
        public static async Task<IEnumerable<TiresOrderRecordModel>> SelectTiresOrderRecordByPhone(string Phone, DateTime? time = null)
        {
            using (var cmd = new SqlCommand("SELECT	Pkid,  OrderId, Number, AddressPhone, UserPhone, DeviceId, UserIp, IsRevoke, CreateDateTime AS CreateTime, LastUpdateDateTime AS LastUpdateTime FROM    Activity..tbl_TiresOrderRecord AS TOR WITH(NOLOCK) WHERE(TOR.AddressPhone = @Phone  OR TOR.UserPhone = @Phone)  AND TOR.IsRevoke = 0  AND TOR.CreateDateTime >= @Time; ; "))
            {
                cmd.Parameters.AddWithValue("@Phone", Phone);
                cmd.Parameters.AddWithValue("@Time", time ?? DateTime.Now.Date);
                return await DbHelper.ExecuteSelectAsync<TiresOrderRecordModel>(true, cmd);
            }
        }
        public static async Task<IEnumerable<TiresOrderRecordModel>> SelectTiresOrderRecordByDeviceId(string deviceId, DateTime? time = null)
        {
            using (var cmd = new SqlCommand("SELECT	Pkid,OrderId,  Number, AddressPhone, UserPhone, DeviceId, UserIp, IsRevoke,CreateDateTime AS CreateTime,LastUpdateDateTime AS LastUpdateTime FROM    Activity..tbl_TiresOrderRecord AS TOR WITH(NOLOCK) WHERE   TOR.DeviceId = @DeviceId  AND TOR.IsRevoke = 0  AND TOR.CreateDateTime >= @Time; "))
            {
                cmd.Parameters.AddWithValue("@DeviceId", deviceId);
                cmd.Parameters.AddWithValue("@Time", time ?? DateTime.Now.Date);
                return await DbHelper.ExecuteSelectAsync<TiresOrderRecordModel>(true, cmd);
            }
        }

        public static async Task<int> UpdateTiresOrderRecordByPkid(int pkid)
        {
            using (var cmd = new SqlCommand("UPDATE	Activity..tbl_TiresOrderRecord WITH(ROWLOCK) SET IsRevoke = 1,LastUpdateDateTime = GETDATE() WHERE Pkid = @Pkid; "))
            {
                cmd.Parameters.AddWithValue("@Pkid", pkid);
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        public static async Task<int> InserTiresOrderRecord(TiresOrderRecordRequestModel requestModel)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO Activity.dbo.tbl_TiresOrderRecord
		                                                    ( OrderId,
		                                                      Number,
		                                                      AddressPhone,
		                                                      UserPhone,
		                                                      DeviceId,
		                                                      UserIp,
		                                                      IsRevoke,
		                                                      CreateDateTime,
		                                                      LastUpdateDateTime )
                                                    VALUES	( @OrderId,
		                                                      @Number,
		                                                      @AddressPhone,
		                                                      @UserPhone,
		                                                      @DeviceId,
		                                                      @UserIp,
		                                                      0,
		                                                      GETDATE(),
		                                                      GETDATE());"))
            {
                cmd.Parameters.AddWithValue("@OrderId", requestModel.OrderId);
                cmd.Parameters.AddWithValue("@Number", requestModel.Number);
                cmd.Parameters.AddWithValue("@AddressPhone", requestModel.AddressPhone);
                cmd.Parameters.AddWithValue("@UserPhone", requestModel.UserPhone);
                cmd.Parameters.AddWithValue("@DeviceId", requestModel.DeviceId);
                cmd.Parameters.AddWithValue("@UserIp", requestModel.UserIp);
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        public static async Task<ShareProductModel> SelectShareActivityProductById(string ProductId, string BatchGuid)
        {
            const string sql = @"SELECT TOP 1
SSMIP.BatchGuid, SSMIP.Times, SSMMC.FirstShareNumber, SSMMC.RuleInfo, SSMIP.PID
FROM  Configuration..SE_ShareMakeImportProducts AS SSMIP (NOLOCK)
JOIN  Configuration..SE_ShareMakeMoneyConfig AS SSMMC (NOLOCK) ON SSMIP.FKID = SSMMC.ID
WHERE SSMIP.IsMakeMoney = 1 AND SSMIP.PID =@PID
AND (@BatchGuid IS NULL OR SSMIP.BatchGuid = @BatchGuid)
ORDER BY SSMIP.ID DESC;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PID", ProductId);
                cmd.Parameters.AddWithValue("@BatchGuid", BatchGuid);
                cmd.CommandType = CommandType.Text;
                return await DbHelper.ExecuteFetchAsync<ShareProductModel>(true, cmd);
            }
        }

        public static async Task<string> SelectShareActivityBatchId()
        {
            const string sql = @"SELECT TOP 1
SSMIP.BatchGuid
FROM  Configuration..SE_ShareMakeImportProducts AS SSMIP (NOLOCK)
JOIN  Configuration..SE_ShareMakeMoneyConfig AS SSMMC (NOLOCK) ON SSMIP.FKID = SSMMC.ID
WHERE SSMIP.IsMakeMoney = 1
ORDER BY SSMIP.ID DESC;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                return (await DbHelper.ExecuteScalarAsync(true, cmd)).ToString();
            }
        }
        public static async Task<BaoYangActivitySetting> SelectBaoYangActivitySetting(string activityId)
        {
            using (var cmd = new SqlCommand(@"SELECT TOP 1

        BYAS.ActivityNum,
        BYAS.LayerImage,
        BYAS.LayerImage2,
        BYAS.CouponId,
        BYAS.GetRuleGUID,
        BYAS.ButtonChar,
        BYAS.RelateServicesTypes,
        BYAS.ActivityImage
FROM    Gungnir..BaoYangActivitySetting AS BYAS WITH(NOLOCK)
WHERE   BYAS.ActivityStatus = 1

        AND((@ActivityId IS NULL

                AND BYAS.LayerStatus = 1)

              OR(@ActivityId IS NOT NULL

                   AND ActivityNum = @ActivityId))"))
            {
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                return await DbHelper.ExecuteFetchAsync<BaoYangActivitySetting>(true, cmd);
            }
        }

        #region 蓄电池/加油卡 活动配置

        /// <summary>
        /// 获取蓄电池活动
        /// </summary>
        /// <returns></returns>
        public static async Task<SE_CouponActivityConfigModel> GetCouponActivityConfig(string activityNum, int type)
        {
            #region Sql
            var sqlText = @"SELECT
                              c.Id,
                              c.ActivityNum,
                              c.ActivityName,
                              c.CheckStatus,
                              c.LayerImage,
                              c.ActivityImage,
                              c.Type,
                              c.Channel
                            FROM Configuration..SE_CouponActivityConfig
                              AS c WITH ( NOLOCK )
                            WHERE c.ActivityNum = @ActivityNum
                                  AND c.Type = @Type
                                  AND c.ActivityStatus = 1;";
            #endregion
            using (var cmd = new SqlCommand(sqlText))
            {
                cmd.Parameters.AddWithValue("@ActivityNum", activityNum);
                cmd.Parameters.AddWithValue("@Type", type);
                return await DbHelper.ExecuteFetchAsync<SE_CouponActivityConfigModel>(true, cmd);
            }
        }

        /// <summary>
        /// 获取蓄电池/加油卡活动配置的优惠券/跳转链接
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public static async Task<List<SE_CouponActivityChannelConfigModel>> GetCouponActivityChannelConfig(int configId)
        {
            #region Sql
            var sqlText = @"SELECT
                              s.ConfigId,
                              s.GetRuleGUID,
                              s.Type,
                              s.Url,
                              s.Channel
                            FROM Configuration..SE_CouponActivityChannelConfig
                              AS s WITH ( NOLOCK )
                            WHERE s.ConfigId = @ConfigId;";
            #endregion
            using (var cmd = new SqlCommand(sqlText))
            {
                cmd.Parameters.AddWithValue("ConfigId", configId);
                var configs = await DbHelper.ExecuteSelectAsync<SE_CouponActivityChannelConfigModel>(true, cmd);
                return configs?.ToList() ?? new List<SE_CouponActivityChannelConfigModel>();
            }
        }

        #endregion


        /// <summary>
        /// 获取所有的搜索活动
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<ActivityBuild>> SelectAllActivityBuildConfig()
        {
            var sqlText = @"
SELECT  0 AS [id] ,
        N'' [ActivityUrl] ,
        SelKeyName ,
        SelKeyImage ,
        UpdateDateTime AS UpdateTime ,
        HashKey
FROM    Configuration.dbo.ActivePageList WITH ( NOLOCK )
WHERE   SelKeyName IS NOT NULL
        AND HashKey IS NOT NULL
        AND ( StartDate IS NULL
              OR StartDate <= @Date
            )
        AND ( EndDate IS NULL
              OR EndDate >= @Date
            )
UNION ALL
SELECT  [id] ,
        [ActivityUrl] ,
        [SelKeyName] ,
        [SelKeyImage] ,
        UpdateTime ,
        N'' HashKey
FROM    [Activity].[dbo].[ActivityBuild] WITH ( NOLOCK )
WHERE   SelKeyName IS NOT NULL
        AND ( StartDT IS NULL
              OR StartDT <= @Date
            )
        AND ( EndDate IS NULL
              OR EndDate >= @Date
            );";
            using (var cmd = new SqlCommand(sqlText))
            {
                cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                return await DbHelper.ExecuteSelectAsync<ActivityBuild>(true, cmd);
            }
        }


        public static ActivityTypeModel GetActivityTypeModel(Guid activityId)
        {

            using (var cmd = new SqlCommand(@"select ActivityId,Type from SystemLog.dbo.ActiveTypeRecord with(nolock) where ActivityId=@ActivityId "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                return DbHelper.ExecuteFetch<ActivityTypeModel>(true, cmd);

            }
        }


        public static async Task<int> RecordActivityTypeLog(ActivityTypeRequest request)
        {
            using (var cmd = new SqlCommand(@"
MERGE INTO SystemLog.dbo.ActiveTypeRecord  WITH ( ROWLOCK ) AS atp
USING
    ( SELECT    @ActivityId AS a
    ) sb
ON atp.ActivityId = sb.a
     WHEN MATCHED THEN
         UPDATE
               SET ActivityId = @ActivityId,
                     Type  = @Type,
					 StartDateTime=@StartDateTime,
					 EndDateTime=@EndDateTime,
					 status=@status,
					 CreateDateTime=getdate(),
					 LastUpdateDateTime=getdate()
     WHEN NOT MATCHED THEN
               INSERT (ActivityId,Type,StartDateTime,EndDateTime,status,CreateDateTime,LastUpdateDateTime) VALUES (@ActivityId,@Type,@StartDateTime,@EndDateTime,@status,getdate(),getdate()); "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityId", request.ActivityId);
                cmd.Parameters.AddWithValue("@Type", request.Type);
                cmd.Parameters.AddWithValue("@StartDateTime", request.StartDateTime);
                cmd.Parameters.AddWithValue("@EndDateTime", request.EndDateTime);
                cmd.Parameters.AddWithValue("@status", request.Status);
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        #region 轮胎活动
        public static async Task<TiresActivityConfig> SelectRegionTiresActivity(Guid activityId)
        {
            const string sql = @"
             SELECT tac.ActivityId ,
                    tac.ActivityName ,
                    tac.WXUrl ,
                    tac.AppUrl ,
                    tac.ShareImg ,
                    tac.ShareTitle ,
                    tac.ShareDes ,
                    tac.IsAdaptationVehicle ,
                    tac.ActivityRules ,
                    tac.ActivityRulesImg ,
                    tac.HeadImg ,
                    tac.NoAdaptationImg ,
                    tac.BackgroundColor ,
                    tac.StartTime ,
                    tac.EndTime ,
                    tac.IsShowInstallmentPrice
             FROM   Configuration..TiresActivityConfig AS tac WITH ( NOLOCK )
             WHERE  tac.ActivityId = @ActivityId;";
            return await DbHelper.ExecuteFetchAsync<TiresActivityConfig>(true, sql, CommandType.Text, new SqlParameter("@ActivityId", activityId.ToString()));
        }

        public static async Task<IEnumerable<TiresFloorActivityInfo>> SelectFloorActivityByParentId(Guid tiresActivityId)
        {
            const string sql = @" SELECT tfa.FloorActivityId ,tfa.FlashSaleId ,tfa.TiresActivityId ,fs.ActivityName ,fs.StartDateTime AS StartTime , fs.EndDateTime AS EndTime
             FROM   Configuration..TiresFloorActivityConfig AS tfa WITH ( NOLOCK )
                    LEFT JOIN Activity..tbl_FlashSale AS fs WITH ( NOLOCK ) ON tfa.FlashSaleId = fs.ActivityID
             WHERE  tfa.TiresActivityId = @TiresActivityId;";
            return await DbHelper.ExecuteSelectAsync<TiresFloorActivityInfo>(true, sql, CommandType.Text, new SqlParameter("@TiresActivityId", tiresActivityId.ToString()));
        }

        public static async Task<IEnumerable<ActivityImageConfig>> FetchActivityImageConfig(Guid activityId)
        {
            const string sql = @"SELECT * FROM Configuration..ActivityImageConfig WITH (NOLOCK) WHERE ActivityId=@ActivityId";
            return await DbHelper.ExecuteSelectAsync<ActivityImageConfig>(true, sql, CommandType.Text, new SqlParameter("@ActivityId", activityId.ToString()));
        }

        public static async Task<IEnumerable<TiresActivityProductConfig>> FetchRegionMarketingProductConfig(Guid activityId)
        {
            const string sql = @"SELECT  ActivityId , ProductId , AdvertiseTitle , SpecialCondition , IsCancelProgressBar , Position , CreatedTime FROM Configuration..RegionMarketingProductConfig WITH (NOLOCK) WHERE ActivityId=@ActivityId";
            return await DbHelper.ExecuteSelectAsync<TiresActivityProductConfig>(true, sql, CommandType.Text, new SqlParameter("@ActivityId", activityId.ToString()));
        }

        public static async Task<IEnumerable<SimpleTireProductInfo>> FetchSimpleTireProductInfo(string pids)
        {
            const string sql = @"SELECT  PID ,CP_Tire_Rim ,CP_Tire_Width ,CP_Tire_AspectRatio FROM    Tuhu_productcatalog..[vw_Products] WITH ( NOLOCK )
            WHERE   EXISTS ( SELECT 1 FROM   Tuhu_productcatalog..SplitString(@PIDS, ',', 1) AS t WHERE  t.Item = PID );";
            return await DbHelper.ExecuteSelectAsync<SimpleTireProductInfo>(true, sql, CommandType.Text, new SqlParameter("@PIDS", pids));
        }

        public static async Task<string> SelectTireChangedActivity(string vehicleId, string tireSize)
        {
            const string sql = @"
SELECT TOP 1
        act.HashKey
FROM    Configuration.dbo.TireChangedActPage AS act WITH ( NOLOCK )
        JOIN Configuration..ActivePageList AS activity WITH ( NOLOCK ) ON activity.HashKey = act.HashKey
                                                              AND activity.StartDate <= GETDATE()
                                                              AND activity.EndDate >= GETDATE()
WHERE   act.State = 1
        AND act.VehicleId = @VehicleId
        AND act.TireSize = @TireSize
ORDER BY act.CreateDateTIme DESC";
            return (await DbHelper.ExecuteScalarAsync(true, sql, CommandType.Text,
                new SqlParameter("@VehicleId", vehicleId.ToString()), new SqlParameter("@TireSize", tireSize)))?.ToString() ?? "";
        }
        #endregion


        public static async Task<int> RecordActivityProductUserRemindLogAsync(ActivityProductUserRemindRequest request)
        {
            using (var db = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(@"
	            INSERT INTO Tuhu_Log..ActivityProductUserRemindLog
                 (UserId,ActivityId,ActivityName,ProductName,pid,Status,CreateDateTime,LastUpdateDateTime) Values(@UserId,@ActivityId,@ActivityName,@ProductName,@Pid,1,GETDATE(),GETDATE())")
                            )
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserId", request.UserId);
                    cmd.Parameters.AddWithValue("@ActivityId", request.ActivityId);
                    cmd.Parameters.AddWithValue("@ActivityName", request.ActivityName);
                    cmd.Parameters.AddWithValue("@ProductName", request.PorductName);
                    cmd.Parameters.AddWithValue("@pid", request.Pid);
                    return await db.ExecuteNonQueryAsync(cmd);
                }
            }
        }
        public static async Task<List<FlashSaleProductModel>> GetAllValidActvivitysAsync()
        {
            using (var cmd = new SqlCommand(@"
	            SELECT  *  FROM    Activity.dbo.vw_ValidFlashSale vvfs WITH ( NOLOCK )"))
            {
                cmd.CommandType = CommandType.Text;
                return (await DbHelper.ExecuteSelectAsync<FlashSaleProductModel>(cmd)).ToList();
            }
        }

        public static async Task<RebateApplyPageConfig> SelectRebateApplyPageConfig()
        {
            const string sql = @"SELECT TOP 1 rapc.PKID,rapc.BackgroundImg,rapc.ActivityRules,rapc.RebateSuccessMsg,rapc.RedBagRemark FROM Activity..RebateApplyPageConfig AS rapc WITH (NOLOCK) ORDER BY PKID DESC";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                return (await DbHelper.ExecuteSelectAsync<RebateApplyPageConfig>(cmd)).FirstOrDefault();
            }
        }

        public static async Task<List<RebateApplyImageConfig>> SelectRebateApplyImageConfig(int parentId)
        {
            const string sql = @"
            SELECT  raic.ImgUrl ,
                    raic.Source ,
                    raic.Remarks
             FROM   Activity..RebateApplyImageConfig AS raic WITH ( NOLOCK )
             WHERE  raic.ParentId = @ParentId AND raic.Source = N'PageImg'";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ParentId", parentId);
                return (await DbHelper.ExecuteSelectAsync<RebateApplyImageConfig>(cmd)).ToList();
            }
        }

        public static async Task<List<RebateApplyConfigModel>> SelectRebateApplyConfigByParamV2(BaseDbHelper helper, RebateApplyRequest request, string source)
        {
            const string sql = @"
                SELECT  rac.OrderId ,
                        rac.UserPhone ,
                        rac.Status ,
                        rac.WXId ,
                        rac.WXName ,
                        rac.OpenId ,
                        rac.BaiDuId ,
                        rac.BaiDuName
                FROM    Activity..RebateApplyConfig AS rac WITH ( NOLOCK )
                WHERE   rac.IsDelete = 0
                        AND rac.Source = @Source
                        AND ( rac.OrderId = @OrderId
                              OR rac.UserPhone = @UserPhone
                              OR rac.OpenId = @OpenId
                            );";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@OrderId", request.OrderId);
                cmd.Parameters.AddWithValue("@UserPhone", request.UserPhone);
                cmd.Parameters.AddWithValue("@OpenId", request.OpenId);
                cmd.Parameters.AddWithValue("@Source", source);
                return (await helper.ExecuteSelectAsync<RebateApplyConfigModel>(cmd)).ToList();
            }
        }

        public static async Task<int> InsertRebateApplyRecordV2(BaseDbHelper helper, RebateApplyRequest request, string source, int installShopId)
        {
            const string sql = @"
            INSERT  INTO Activity..RebateApplyConfig
                    ( OrderId ,
                      UserPhone ,
                      Status ,
                      WXId ,
                      WXName ,
                      BaiDuId ,
                      BaiDuName ,
                      PrincipalPerson ,
                      Remarks ,
                      QRCodeImg ,
                      RebateMoney ,
                      OpenId ,
                      UnionId ,
                      Source ,
                      InstallShopId ,
                      CreateTime ,
                      UpdateTime
                    )
            OUTPUT  Inserted.PKID
            VALUES  ( @OrderId ,
                      @UserPhone ,
                      @Status ,
                      @WXId ,
                      @WXName ,
                      @BaiDuId ,
                      @BaiDuName ,
                      @PrincipalPerson ,
                      @Remarks ,
                      @QRCodeImg ,
                      @RebateMoney ,
                      @OpenId ,
                      @UnionId ,
                      @Source ,
                      @InstallShopId ,
                      GETDATE() ,
                      GETDATE()
                    );";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@OrderId", request.OrderId);
                cmd.Parameters.AddWithValue("@UserPhone", request.UserPhone);
                cmd.Parameters.AddWithValue("@WXId", request.WXId);
                cmd.Parameters.AddWithValue("@WXName", request.WXName);
                cmd.Parameters.AddWithValue("@BaiDuId", request.BaiDuId);
                cmd.Parameters.AddWithValue("@BaiDuName", request.BaiDuName);
                cmd.Parameters.AddWithValue("@PrincipalPerson", request.PrincipalPerson);
                cmd.Parameters.AddWithValue("@Remarks", request.Remarks);
                cmd.Parameters.AddWithValue("@RebateMoney", request.RebateMoney);
                cmd.Parameters.AddWithValue("@OpenId", request.OpenId);
                cmd.Parameters.AddWithValue("@UnionId", request.UnionId);
                cmd.Parameters.AddWithValue("@Source", source);
                cmd.Parameters.AddWithValue("@InstallShopId", installShopId);
                cmd.Parameters.AddWithValue("@QRCodeImg", request.QRCodeImg);
                cmd.Parameters.AddWithValue("@Status", "Applying");
                return Convert.ToInt32(await helper.ExecuteScalarAsync(cmd));
            }
        }

        public static async Task<List<RebateApplyResponse>> GetRebateApplyByOpenId(string openId)
        {
            const string sql = @"
            SELECT  rac.PKID ,
                    rac.OrderId ,
                    rac.UserPhone ,
                    rac.Status ,
                    rac.WXId ,
                    rac.RefusalReason ,
                    rac.WXName ,
                    rac.BaiDuId ,
                    rac.BaiDuName ,
                    rac.CreateTime ,
                    rac.CheckTime ,
                    rac.RebateTime
            FROM    Activity..RebateApplyConfig AS rac WITH ( NOLOCK )
            WHERE   rac.OpenId = @OpenId
                    AND rac.IsDelete = 0
            ORDER BY rac.PKID DESC;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@OpenId", openId);
                return (await DbHelper.ExecuteSelectAsync<RebateApplyResponse>(cmd)).ToList();
            }
        }
        /// <summary>
        /// 通过OrderId获取最新的申请记录
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<RebateApplyResponse>> GetAllRebateApplyByOrderId(int orderId)
        {
            const string sql = @"
            SELECT 
            rac.PKID ,
            rac.OrderId ,
            rac.UserPhone ,
            rac.Status ,
            rac.WXId ,
            rac.RefusalReason ,
            rac.WXName ,
            rac.BaiDuId ,
            rac.BaiDuName ,
            rac.Source ,
            rac.CreateTime ,
            rac.CheckTime ,
            rac.RebateTime
    FROM    Activity..RebateApplyConfig AS rac WITH ( NOLOCK )
    WHERE   rac.OrderId = @OrderId
            AND rac.IsDelete = 0
    ORDER BY rac.PKID DESC;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                return (await DbHelper.ExecuteSelectAsync<RebateApplyResponse>(true, cmd)).ToList();
            }
        }

        public static async Task<IEnumerable<string>> SelectRebateApplyImages(int parentId)
        {
            const string sql = @"SELECT ImgUrl FROM Activity..RebateApplyImageConfig WITH (NOLOCK) WHERE ParentId=@ParentId AND Source<>N'PageImg'";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@ParentId", parentId));
                return await DbHelper.ExecuteQueryAsync(cmd, dt => dt.ToList<string>());
            }
        }

        public static async Task<int> InsertRebateApplyImage(BaseDbHelper helper, int parentId, string imgUrl)
        {
            const string sql = @"
            INSERT INTO Activity..RebateApplyImageConfig
	                ( ParentId ,
	                  ImgUrl ,
	                  Source ,
	                  CreateTime ,
	                  UpdateTime
	                )
	        VALUES  ( @ParentId ,
	                  @ImgUrl ,
	                  'UserImg' ,
	                  GETDATE() ,
	                  GETDATE()
	                )";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ParentId", parentId);
                cmd.Parameters.AddWithValue("@ImgUrl", imgUrl);
                return await helper.ExecuteNonQueryAsync(cmd);
            }
        }
        #region 活动页白名单
        public static async Task<int> MergeIntoActivityPageWhiteListRecord(ActivityPageWhiteListModel whitelist)
        {
            using (var cmd = new SqlCommand(@"
                                        MERGE INTO Configuration..ActivityPageWhiteList   AS T
										USING(SELECT @UserId AS UserId,@PhoneNum AS PhoneNum) AS S
										ON T.UserId=s.UserId AND T.PhoneNum=s.PhoneNum
										WHEN MATCHED
										THEN UPDATE SET UserId=@UserId,PhoneNum=@PhoneNum,Status=@Status
										WHEN NOT MATCHED
										THEN INSERT (UserId,PhoneNum,Status) VALUES(@UserId,@PhoneNum,@Status);"))
            {
                cmd.Parameters.AddWithValue("@UserId", whitelist.UserId);
                cmd.Parameters.AddWithValue("@PhoneNum", whitelist.PhoneNum);
                cmd.Parameters.AddWithValue("@Status", whitelist.Status);
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }
        public static async Task<int> GetActivityPageWhiteListByUserIdAsync(Guid userId)
        {
            using (var cmd = new SqlCommand(@"
                                       	SELECT TOP 1 pkid FROM Configuration..ActivityPageWhiteList WITH ( NOLOCK) WHERE Status=1 AND UserId=@UserId"))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                return Convert.ToInt32(await DbHelper.ExecuteScalarAsync(true, cmd));
            }
        }
        public static async Task<IEnumerable<string>> GetActivityAllPageWhiteListUserIdAsync()
        {
            const string sql = @"SELECT UserId FROM Configuration..ActivityPageWhiteList WITH ( NOLOCK) WHERE Status=1";
            using (var cmd = new SqlCommand(sql))
            {
                return await DbHelper.ExecuteQueryAsync(cmd, dt => dt.ToList<string>());
            }
        }
        #endregion

        public static async Task<int> InsertUserRewardApplication(UserRewardApplicationRequest request)
        {
            using (var cmd = new SqlCommand(@"
         INSERT INTO Configuration..UserRewardApplication
        ( ApplicationName ,
          Phone ,
          ImageUrl1 ,
		  ImageUrl2 ,
		  ImageUrl3 ,
          ApplicationState
        )
VALUES  ( @ApplicationName , -- ApplicationName - nvarchar(100)
          @Phone , -- Phone - nvarchar(20)
          @ImageUrl1 , -- ImageUrl - nvarchar(500)
		  @ImageUrl2 , -- ImageUrl - nvarchar(500)
		  @ImageUrl3 , -- ImageUrl - nvarchar(500)
          @ApplicationState  -- ApplicationState - int
        )"))
            {
                cmd.Parameters.AddWithValue("@ApplicationName", request.ApplicationName);
                cmd.Parameters.AddWithValue("@Phone", request.Phone);
                cmd.Parameters.AddWithValue("@ImageUrl1", request.ImageUrl1);
                cmd.Parameters.AddWithValue("@ImageUrl2", request.ImageUrl2);
                cmd.Parameters.AddWithValue("@ImageUrl3", request.ImageUrl3);
                cmd.Parameters.AddWithValue("@ApplicationState", 1);
                await SetReadOnlyFlag(request.Phone);
                return Convert.ToInt32(await DbHelper.ExecuteNonQueryAsync(cmd));
            }
        }

        public static async Task<int> SelectUserRewardApplicationByPhoneAsync(string phone)
        {
            var flag = await GetReadOnlyFlag(phone);
            using (var cmd = new SqlCommand(@"
               SELECT  TOP 1 ApplicationState FROM  Configuration..UserRewardApplication AS FS WITH(NOLOCK) WHERE FS.Phone=@Phone "))
            {
                cmd.Parameters.AddWithValue("@Phone", phone);
                return Convert.ToInt32(await DbHelper.ExecuteScalarAsync(flag, cmd));
            }
        }

        private static async Task<bool> SetReadOnlyFlag(string key, string prefix = "UserReward")
        {
            using (var client = CacheHelper.CreateCacheClient(ReadonlyFlagClientName))
            {
                var result = await client.SetAsync($"{prefix}{key}", false, TimeSpan.FromSeconds(10));
                return result.Success && result.Value;
            }
        }

        private static async Task<bool> GetReadOnlyFlag(string key, string prefix = "UserReward")
        {
            using (var client = CacheHelper.CreateCacheClient(ReadonlyFlagClientName))
            {
                var result = await client.GetAsync<bool>($"{prefix}{key}");
                return !result.Success || result.Value;
            }
        }

        public static async Task<bool> InsertApplyCompensateAsync(ApplyCompensateRequest model)
        {
            const string sql = @"INSERT INTO Configuration.dbo.SE_ApplyCompensateConfig
			                            ( UserName ,
			                              PhoneNumber ,
			                              OrderId ,
			                              ProductName ,
			                              Link ,
			                              DifferencePrice ,
			                              Images ,
			                              ApplyTime ,
			                              Status,
                                          OrderChannel
			                            )
			                    VALUES  ( @UserName , -- UserName - nvarchar(50)
			                              @PhoneNumber , -- PhoneNumber - varchar(50)
			                              @OrderId , -- OrderId - varchar(50)
			                              @ProductName , -- ProductName - nvarchar(50)
			                              @Link , -- Link - nvarchar(1000)
			                              @DifferencePrice , -- DifferencePrice - money
			                              @Images , -- Images - nvarchar(2000)
			                              GETDATE() , -- ApplyTime - datetime
			                              0,  -- Status - smallint
                                          @OrderChannel
			                            )";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber ?? string.Empty);
                cmd.Parameters.AddWithValue("@DifferencePrice", model.DifferencePrice);
                cmd.Parameters.AddWithValue("@Images", model.Images ?? string.Empty);
                cmd.Parameters.AddWithValue("@Link", model.Link ?? string.Empty);
                cmd.Parameters.AddWithValue("@OrderId", model.OrderId ?? string.Empty);
                cmd.Parameters.AddWithValue("@ProductName", model.ProductName ?? string.Empty);
                cmd.Parameters.AddWithValue("@UserName", model.UserName ?? string.Empty);
                cmd.Parameters.AddWithValue("@OrderChannel", model.OrderChannel ?? string.Empty);
                await SetReadOnlyFlag(model.OrderId);
                return Convert.ToInt32(await DbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }
        public static async Task<string> SelectApplyCompensateAsync(string orderId)
        {
            var flag = await GetReadOnlyFlag(orderId);
            using (var cmd = new SqlCommand(@"
               SELECT  TOP 1 OrderId FROM  Configuration.dbo.SE_ApplyCompensateConfig AS SACC WITH(NOLOCK) WHERE SACC.OrderId=@OrderId "))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                return (await DbHelper.ExecuteScalarAsync(flag, cmd))?.ToString();
            }
        }

        #region vipcard预付卡

        public static async Task<IEnumerable<VipCardSaleConfigDetailModel>> GetVipCardSaleConfigDetailsAsync(string activityId)
        {
            using (var cmd = new SqlCommand(@"
										SELECT  * FROM Configuration..VipCardSaleConfigDetail AS Detail WITH ( NOLOCK)
										JOIN Configuration..VipCardSaleConfig AS Card WITH(NOLOCK) ON Detail.VipCardId=Card.Pkid
										WHERE Card.ActivityId=@ActivityId and status=1"))
            {
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                return await DbHelper.ExecuteSelectAsync<VipCardSaleConfigDetailModel>(true, cmd);
            }
        }
        public static async Task<int> GetVipCardStockAsync(string batchid)
        {
            using (var cmd = new SqlCommand(@"
										SELECT Stock FROM Configuration..VipClientBatchConfig WITH ( NOLOCK) WHERE BatchId=@BatchId "))
            {
                cmd.Parameters.AddWithValue("@BatchId", batchid);
                return Convert.ToInt32(await DbHelper.ExecuteScalarAsync(true, cmd));
            }
        }
        public static async Task<int> GetVipCardDetailSumStockAsync(string batchid)
        {
            using (var cmd = new SqlCommand(@"
			SELECT SUM(SaleOutQuantity) FROM Configuration..VipCardSaleConfigDetail WITH ( NOLOCK) WHERE BatchId=@BatchId"))
            {
                cmd.Parameters.AddWithValue("@BatchId", batchid);
                var flag = await GetReadOnlyFlag(batchid, "vipcard");
                return Convert.ToInt32(await DbHelper.ExecuteScalarAsync(flag, cmd));
            }
        }

        public static async Task<bool> PutVipCardRecordAsync<T>(T request, int type = 1, string bindmsg = null) where T : VipCardRecordRequest
        {
            var flag = true;
            var sql = @"INSERT INTO Tuhu_log..VipCardBuyLog
			(
			ActivityId,
			OrderId,
			UserId,
			UserPhone,
			BatchId,
			BuyNum,
            LogType,
            BindCardMsg
			)
			VALUES(
			@ActivityId,
			@OrderId,
			@UserId,
			@UserPhone,
			@BatchId,
			@BuyNum,
            @LogType,
            @BindCardMsg
			)";
            using (var db = DbHelper.CreateLogDbHelper())
            {
                await SetReadOnlyFlag(request.OrderId.ToString(), "vipcard");
                foreach (var res in request.Batches)
                {
                    try
                    {
                        using (var cmd = new SqlCommand(sql))
                        {
                            cmd.Parameters.AddWithValue("@ActivityId", request.ActivityId);
                            cmd.Parameters.AddWithValue("@OrderId", request.OrderId);
                            cmd.Parameters.AddWithValue("@UserId", request.UserId);
                            cmd.Parameters.AddWithValue("@UserPhone", request.UserPhone);
                            cmd.Parameters.AddWithValue("@BatchId", res.BatchId);
                            cmd.Parameters.AddWithValue("@BuyNum", res.CardNum);
                            cmd.Parameters.AddWithValue("@LogType", type);
                            cmd.Parameters.AddWithValue("@BindCardMsg", bindmsg);
                            var result = Convert.ToInt32(await db.ExecuteNonQueryAsync(cmd));
                            flag = flag && result > 0;
                        }
                    }

                    catch (Exception e)
                    {
                        flag = false;
                        Logger.Error(
                            $"记录vipcard数据报错，OrderId==>{request.OrderId},batchId==>{res.BatchId},Num==>{res.CardNum}",
                            e.InnerException);
                    }
                }
            }
            return flag;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static async Task<VipCardRecordRequest> GetVipCardRecordByOrderIdAsync(int orderId)
        {
            try
            {

                var model = new VipCardRecordRequest();
                var flag = await GetReadOnlyFlag(orderId.ToString(), "vipcard");
                using (var db = DbHelper.CreateLogDbHelper(flag))
                {
                    using (var cmd = new SqlCommand(@"
			SELECT * FROM Tuhu_log..VipCardBuyLog WITH ( NOLOCK) WHERE OrderId=@OrderId And LogType=1 and Status=1"))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        return await db.ExecuteQueryAsync(cmd, dt =>
                        {
                            if (dt == null || dt.Rows.Count == 0)
                                return model;
                            else
                            {
                                model.ActivityId = dt.Rows[0].GetValue<string>("ActivityId");
                                model.OrderId = dt.Rows[0].GetValue<int>("OrderId");
                                model.UserId = dt.Rows[0].GetValue<Guid>("UserId");
                                model.UserPhone = dt.Rows[0].GetValue<string>("UserPhone");
                                var list = new List<VipCardBatchModel>();
                                foreach (DataRow row in dt.Rows)
                                {
                                    var batchModel = new VipCardBatchModel();
                                    batchModel.CardNum = row.GetValue<int>("BuyNum");
                                    batchModel.BatchId = row.GetValue<string>("BatchId");
                                    list.Add(batchModel);
                                }
                                model.Batches = list;
                                return model;
                            }
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task<bool> UpdateVipCardBuyLogByOrderId(int orderId)
        {
            await SetReadOnlyFlag(orderId.ToString(), "vipcard");
            using (var db = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(@"UPDATE Tuhu_log..VipCardBuyLog SET status=0  WHERE OrderId=@OrderId And LogType=1"))
                {
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    return await db.ExecuteNonQueryAsync(cmd) > 0;
                }
            }
        }

        public static async Task<bool> UpdateVipCardDetailQtyByBatchId(string batchId, int num, int vipCardId)
        {
            using (var cmd = new SqlCommand(@"UPDATE Configuration..VipCardSaleConfigDetail WITH(ROWLOCK) SET
										SaleOutQuantity=SaleOutQuantity+@Num WHERE BatchId=@BatchId And VipCardId=@VipCardId"))
            {
                cmd.Parameters.AddWithValue("@BatchId", batchId);
                cmd.Parameters.AddWithValue("@Num", num);
                cmd.Parameters.AddWithValue("@VipCardId", vipCardId);
                await SetReadOnlyFlag(batchId, "vipcard");
                return await DbHelper.ExecuteNonQueryAsync(cmd) > 0;
            }
        }

        public static async Task<int> SelectVipCardId(string activityId)
        {
            using (var cmd = new SqlCommand(@"	SELECT pkid FROM Configuration..VipCardSaleConfig WITH ( NOLOCK)  WHERE ActivityId=@ActivityId"))
            {
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                return Convert.ToInt32(await DbHelper.ExecuteScalarAsync(cmd));
            }
        }
        #endregion

        public static async Task<List<VehicleBanner>> SelectVehicleBannerAsync(string hashKey, string group, int col)
        {
            using (var cmd = new SqlCommand(@"
                 SELECT Value AS Brand ,
                        Value1 AS VehicleId ,
                        Value2 AS ImageUrl
                 FROM   Configuration..CommonConfigColValueInfo AS cccv WITH ( NOLOCK )
                        JOIN Configuration..CommonConfigColInfo AS ccvi WITH ( NOLOCK ) ON cccv.ConfigInfoPkId = ccvi.PKID
                 WHERE  ColName = 'VehicleImage'
                        AND ActiveHashKey = @hashKey
                        AND GroupId = @group
                        AND FormIndex = @col;"))
            {
                cmd.Parameters.AddWithValue("@hashKey", hashKey);
                cmd.Parameters.AddWithValue("@group", group);
                cmd.Parameters.AddWithValue("@col", col);
                return (await DbHelper.ExecuteSelectAsync<VehicleBanner>(true, cmd)).ToList();
            }
        }

        #region 创建活动
        public static async Task<int> CreateActivityAsync(FlashSaleModel model)
        {
            using (var cmd = new SqlCommand(@"
        	  INSERT	INTO Activity..tbl_FlashSale
				(
				  ActivityID,
				  ActivityName,
				  StartDateTime,
				  EndDateTime,
				  ActiveType,
				  PlaceQuantity,
				  IsNewUserFirstOrder,
				  IsDefault )
	  VALUES	(
				  @ActivityID,
				  @ActivityName,
				  getdate(),
				  getdate(),
				  @ActiveType,
				  0,
				  0,
				  0 )"))
            {
                cmd.Parameters.AddWithValue("@ActivityID", model.ActivityID);
                cmd.Parameters.AddWithValue("@ActivityName", model.ActivityName);
                cmd.Parameters.AddWithValue("@ActiveType", model.ActiveType);
                return (await DbHelper.ExecuteNonQueryAsync(cmd));
            }
        }
        public static async Task<bool> CreateActivityProductsAsync(List<FlashSaleProductModel> products, string activityId)
        {
            var result = true;
            const string sql = @"
		INSERT INTO activity..tbl_FlashSaleProducts
		        ( ActivityID ,
		          PID ,
		          Position ,
		          Price ,
		          Label ,
		          TotalQuantity ,
		          MaxQuantity ,
		          SaleOutQuantity ,
		          CreateDateTime ,
		          LastUpdateDateTime ,
		          ProductName ,
		          InstallAndPay ,
		          Level ,
		          ImgUrl ,
		          IsUsePCode ,
		          Channel ,
		          FalseOriginalPrice ,
		          IsJoinPlace ,
		          IsShow ,
		          InstallService
		        )
		VALUES  ( @ActivityID , -- ActivityID - uniqueidentifier
		          @PID , -- PID - varchar(200)
		          @Position, -- Position - int
		          0 , -- Price - money
		          N'' , -- Label - nvarchar(20)
		          0 , -- TotalQuantity - int
		          0 , -- MaxQuantity - int
		          0 , -- SaleOutQuantity - int
		          GETDATE() , -- CreateDateTime - datetime
		          GETDATE() , -- LastUpdateDateTime - datetime
		          N'' , -- ProductName - nvarchar(200)
		          N'' , -- InstallAndPay - nvarchar(50)
		          0 , -- Level - int
		          N'' , -- ImgUrl - nvarchar(200)
		          0 , -- IsUsePCode - bit
		          N'' , -- Channel - nvarchar(10)
		          0 , -- FalseOriginalPrice - money
		          0 , -- IsJoinPlace - bit
		          0 , -- IsShow - bit
		          N''  -- InstallService - nvarchar(100)
		        )
";

            foreach (var item in products)
            {

                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@ActivityID", activityId);
                    cmd.Parameters.AddWithValue("@PID", item.PID);
                    cmd.Parameters.AddWithValue("@Position", item.Position);
                    var result1 = await DbHelper.ExecuteNonQueryAsync(cmd);
                    if (result1 <= 0)
                    {
                        Logger.Error($"活动{activityId}插入产品{item.PID}失败");
                        result = result && result1 > 0;
                    }
                }
            }
            return result;
        }

        public static async Task<int> UpdateActivityPageContentAsync(int pkid, string activityId)
        {
            using (var cmd = new SqlCommand(@"
                UPDATE Configuration..ActivePageContent SET systemActivityId=@SystemActivityId WHERE Pkid=@Pkid"))
            {
                cmd.Parameters.AddWithValue("@SystemActivityId", activityId);
                cmd.Parameters.AddWithValue("@Pkid", pkid);
                return await DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }
        public static async Task<string> SelectActivityPageContentSystemActivityIdAsync(int pkid)
        {
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 apc.SystemActivityId FROM Configuration..ActivePageContent AS apc WITH ( NOLOCK) WHERE pkid=@Pkid"))
            {
                cmd.Parameters.AddWithValue("@Pkid", pkid);
                return (await DbHelper.ExecuteScalarAsync(cmd))?.ToString();
            }
        }

        public static async Task<int> DeleteActivityAsync(string activityId)
        {
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                const string sql = @"Delete FROM  Activity..tbl_FlashSale WITH(ROWLOCK) where ActivityID='{0}'";
                var sql1 = string.Format(sql, activityId);

                dbHelper.BeginTransaction();
                var result = await dbHelper.ExecuteNonQueryAsync(sql1);
                if (result > 0)
                {
                    const string sql2 = @"Delete FROM  Activity..tbl_FlashSaleProducts WITH(ROWLOCK) where ActivityID='{0}'";
                    var sql3 = string.Format(sql2, activityId);
                    result = await dbHelper.ExecuteNonQueryAsync(sql3);
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
        #endregion

        #region 活动


        /// <summary>
        ///     通过活动名称获取活动对象
        /// </summary>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public static async Task<ActivityModel> GetActivityById(long activityId)
        {
            var sql = @" select    [PKID]
                                  ,[ActivityName]
                                  ,[StartTime]
                                  ,[EndTime]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[QuestionnaireID]
                                  ,[ShareIntegral]
                                  ,[ActivityType]
                                  ,[SecEndTime]
                                  ,GameRuleText
                                  ,SupportRuleText
                        from [Activity].[dbo].tbl_Activity with (nolock) where PKID=@PKID";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@PKID", activityId);
                var result = await DbHelper.ExecuteSelectAsync<ActivityModel>(true, cmd);
                return result?.FirstOrDefault();
            }
        }

        /// <summary>
        ///     通过活动类型ID获取活动对象
        /// </summary>
        /// <param name="activityType">
        ///     0 2018世界杯 1 拼团车型认证 2 公众号领红包 3 七龙珠
        /// </param>
        /// <returns></returns>
        public static async Task<ActivityModel> GetActivityByTypeId(int activityType)
        {
            var sql = @" select    [PKID]
                                  ,[ActivityName]
                                  ,[StartTime]
                                  ,[EndTime]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[QuestionnaireID]
                                  ,[ShareIntegral]
                                  ,[ActivityType]
                                  ,[SecEndTime]
                                  ,GameRuleText
                                  ,SupportRuleText
                        from [Activity].[dbo].tbl_Activity with (nolock) where ActivityType=@ActivityType";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityType", activityType);
                var result = await DbHelper.ExecuteSelectAsync<ActivityModel>(true, cmd);
                return result?.FirstOrDefault();
            }
        }

        /// <summary>
        ///     更新活动对象
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> UpdateActivtyAsync(BaseDbHelper dbHelper, ActivityModel model)
        {
            var sql = @"

                        UPDATE Activity.[dbo].[tbl_Activity]
                           SET [ActivityName] = @ActivityName
                              ,[StartTime] = @StartTime
                              ,[EndTime] = @EndTime
                              ,[LastUpdateDateTime] =  getdate()
                              ,[QuestionnaireID] = @QuestionnaireID
                              ,[ShareIntegral] = @ShareIntegral
                              ,[SecEndTime] = @SecEndTime
                         WHERE ActivityType = @ActivityType
                ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityType", model.ActivityType);

                cmd.AddParameter("@ActivityName", model.ActivityName);
                cmd.AddParameter("@StartTime", model.StartTime);
                cmd.AddParameter("@EndTime", model.EndTime);
                cmd.AddParameter("@QuestionnaireID", model.QuestionnaireID);
                cmd.AddParameter("@ShareIntegral", model.ShareIntegral);
                cmd.AddParameter("@SecEndTime", model.SecEndTime);

                var result = await dbHelper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }


        /// <summary>
        ///     通过用户ID获取兑换券对象
        /// </summary>
        /// <returns></returns>
        public static async Task<ActivityCouponModel> GetActivityCouponByUserId(bool readOnly, Guid userId, long activityId)
        {
            var sql = @" SELECT [PKID]
                              ,[ActivityId]
                              ,[UserId]
                              ,[CouponCount]
                              ,[CreateDatetime]
                              ,[LastUpdateDateTime]
                              ,CONVERT(VARCHAR, (CONVERT(VARBINARY,[timestamp])),1) as timestamp
                              ,[CouponSum]
                          FROM [Activity].[dbo].[tbl_ActivityCoupon] with (nolock) where ActivityId = @ActivityId and userId=@userId ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@userId", userId);
                cmd.AddParameter("@ActivityId", activityId);
                var result = await DbHelper.ExecuteSelectAsync<ActivityCouponModel>(readOnly, cmd);
                return result?.FirstOrDefault();
            }
        }


        /// <summary>
        ///     兑换券排名
        /// </summary>
        /// <returns></returns>
        public static async Task<PagedModel<ActivityCouponModel>> SearchActivityCouponRank(long activityId, int pageIndex = 1, int pageSize = 20)
        {
            var sql = @"

                                SELECT
                               a.RowNum as Rank
                              ,[UserId]
                              ,[CouponSum]
                                    FROM (
                                                SELECT
                                                                  [UserId]
                                                                  ,[CouponSum]
                                                                  , ROW_NUMBER() OVER (ORDER BY CouponSum DESC,LastUpdateDateTime asc) AS RowNum
                                        FROM [Activity].[dbo].[tbl_ActivityCoupon] with (nolock)
                                        where ActivityId = @ActivityId and CouponSum > 0
                                    ) AS a
                                    WHERE a.RowNum >= @startRow AND a.RowNum <= @endRow
                          ";
            var sqlCount = @"SELECT
                                        Count(*)
                                        FROM [Activity].[dbo].[tbl_ActivityCoupon] with (nolock)
                                        where ActivityId = @ActivityId";

            var result = new PagedModel<ActivityCouponModel>();

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@startRow", (pageIndex - 1) * pageSize + 1);
                cmd.AddParameter("@endRow", pageSize * pageIndex);

                var list = await DbHelper.ExecuteSelectAsync<ActivityCouponModel>(true, cmd);
                result.Source = list;
            }

            using (var cmd = new SqlCommand(sqlCount))
            {
                cmd.AddParameter("@ActivityId", activityId);

                var count = Convert.ToInt32(await DbHelper.ExecuteScalarAsync(true, cmd));
                result.Pager = new PagerModel(pageIndex, pageSize)
                {
                    Total = count
                };
            }
            return result;
        }

        /// <summary>
        ///     获取用户的兑换券排名
        /// </summary>
        /// <returns></returns>
        public static async Task<int> GetUserCouponRank(Guid userId, long activityId, long coupon)
        {
            var sql = @"
                                        DECLARE @sumcount int
                                        DECLARE @userlastupdate datetime
                                        --比当前用户兑换券多的人 总数
                                        SELECT @sumcount = COUNT(*) FROM [Activity].[dbo].[tbl_ActivityCoupon] AS a with (nolock)
                                        WHERE a.ActivityId  = @ActivityId AND CouponSum > @CouponSum
                                        --当前用户最后修改时间
                                        select @userlastupdate = LastUpdateDateTime FROM [Activity].[dbo].[tbl_ActivityCoupon] AS a with (nolock)
                                        where a.ActivityId  = @ActivityId AND a.UserId = @UserId
                                        --当前和自己相同的人数，排序时间
                                        SELECT @sumcount= @sumcount +  COUNT(*) FROM [Activity].[dbo].[tbl_ActivityCoupon] AS a with (nolock)
                                        WHERE a.ActivityId  = @ActivityId AND CouponSum = @CouponSum AND a.UserId <> @UserId AND a.LastUpdateDateTime < @userlastupdate
                                        --输出总量
                                        SELECT isnull(@sumcount, 0) + 1
                          ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@UserId", userId);
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@CouponSum", coupon);

                var result = await DbHelper.ExecuteScalarAsync(true, cmd);
                return Convert.ToInt32(result);
            }
        }


        /// <summary>
        ///     获取第几名的兑换券
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="activityId"></param>
        /// <param name="coupon"></param>
        /// <returns></returns>
        public static async Task<ActivityCouponModel> GetCouponByRank(int rank, long activityId)
        {
            var sql = @"

                                SELECT
                               a.RowNum as Rank
							  ,[PKID]
                              ,[ActivityId]
                              ,[UserId]
                              ,[CouponCount]
                              ,[CreateDatetime]
                              ,[LastUpdateDateTime]
                              ,[CouponSum]
                                    FROM (
                                                SELECT

                                                                   [PKID]
                                                                  ,[ActivityId]
                                                                  ,[UserId]
                                                                  ,[CouponCount]
                                                                  ,[CreateDatetime]
                                                                  ,[LastUpdateDateTime]
                                                                  ,[CouponSum]
                                                                  ,CONVERT(VARCHAR, (CONVERT(VARBINARY,[timestamp])),1) as timestamp
                                                                  , ROW_NUMBER() OVER (ORDER BY CouponSum DESC,LastUpdateDateTime asc) AS RowNum
                                        FROM [Activity].[dbo].[tbl_ActivityCoupon] with (nolock)
                                        where ActivityId = @ActivityId
                                    ) AS a
                                    WHERE a.RowNum = @rank
                          ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@rank", rank);
                cmd.AddParameter("@ActivityId", activityId);

                var result = await DbHelper.ExecuteSelectAsync<ActivityCouponModel>(true, cmd);
                return result?.FirstOrDefault();
            }
        }


        /// <summary>
        ///     查询奖品列表
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userCouponCount"></param>
        /// <param name="userId"></param>
        /// <param name="sortByUserCouponCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<PagedModel<ActivityPrizeModel>> SearchPrizeList(long activityId, int? userCouponCount, Guid? userId, bool sortByUserCouponCount, int pageIndex = 1, int pageSize = 20)
        {


            var where = @" where 1=1 ";
            var orderby = "";
            if (activityId != 0)
            {
                where = where + " and ActivityId = @ActivityId ";
            }
            where = where + " and IsDeleted = 0 and OnSale = 1  ";

            //如果是    当前可兑换
            if (userCouponCount != null && sortByUserCouponCount)
            {
                //不可销售最上
                orderby = orderby + " a.IsDisableSale desc  ";
                //库存大于0的最上
                orderby = orderby + ", (CASE when a.Stock > 0 THEN 1 ELSE 0 END) desc  ";
                //没买过的最上
                orderby = orderby + ", ((select count(*) from  [Activity].[dbo].tbl_ActivityPrizeOrderDetail b where a.ActivityId = b.ActivityId and b.UserId = @userId and b.ActivityPrizeId = a.PKID  ) = 0 ) asc  ";
                //可以购买的最上
                orderby = orderby + " , (CASE when a.CouponCount <= @userCouponCount THEN 1 ELSE 0 END) desc  ";
                //按照价值排序
                orderby = orderby + " , a.CouponCount desc";
            }
            else if (userCouponCount != null)
            {
                //库存大于0的最上
                orderby = orderby + "(CASE when a.Stock > 0 THEN 1 ELSE 0 END) desc  ";
                //可以购买的最上
                orderby = orderby + " , (CASE when a.CouponCount <= @userCouponCount THEN 1 ELSE 0 END) desc  ";
                //不可销售最上
                orderby = orderby + " , a.IsDisableSale desc  ";
                //按照价值排序
                orderby = orderby + " , a.CouponCount desc";
            }
            if (!string.IsNullOrWhiteSpace(orderby))
            {
                orderby = " order by " + orderby;
            }


            var sql = @"
                         SELECT    a.[PKID]
                                  ,a.[ActivityId]
                                  ,a.[PID]
                                  ,a.[ActivityPrizeName]
                                  ,a.[PicUrl]
                                  ,a.[CouponCount]
                                  ,a.[Stock]
                                  ,a.[SumStock]
                                  ,a.[OnSale]
                                  ,a.[GetRuleId]
                                  ,a.[CreateDatetime]
                                  ,a.[LastUpdateDateTime]
                                  ,a.[IsDeleted]
                                  ,CONVERT(VARCHAR, (CONVERT(VARBINARY,a.[timestamp])),1) as timestamp
                                  ,a.[IsDisableSale]
                              FROM [Activity].[dbo].[tbl_ActivityPrize] as a with (nolock)
                              " + where + @"
                              " + orderby + @"
                              OFFSET (@pageIndex -1) * @pageSize ROWS
                              FETCH NEXT @pageSize ROWS ONLY
                          ";
            var sqlCount = @"SELECT count(*) from [Activity].[dbo].[tbl_ActivityPrize] with (nolock)  " + where;


            var result = new PagedModel<ActivityPrizeModel>();

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@userCouponCount", userCouponCount);
                cmd.AddParameter("@pageIndex", (pageIndex));
                cmd.AddParameter("@pageSize", pageSize);

                var list = await DbHelper.ExecuteSelectAsync<ActivityPrizeModel>(true, cmd);
                result.Source = list;
            }

            using (var cmd = new SqlCommand(sqlCount))
            {
                cmd.AddParameter("@ActivityId", activityId);

                var count = Convert.ToInt32(await DbHelper.ExecuteScalarAsync(true, cmd));
                result.Pager = new PagerModel(pageIndex, pageSize)
                {
                    Total = count
                };
            }
            return result;
        }

        /// <summary>
        ///     查询奖品列表
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userCouponCount"></param>
        /// <param name="userId"></param>
        /// <param name="sortByUserCouponCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ActivityPrizeModel>> SearchPrizeList(long activityId)
        {


            var where = @" where 1=1 ";
            var orderby = " order by pkid asc ";

            if (activityId != 0)
            {
                where = where + " and ActivityId = @ActivityId ";
            }
            where = where + " and IsDeleted = 0 and OnSale = 1  ";


            var sql = @"
                         SELECT    a.[PKID]
                                  ,a.[ActivityId]
                                  ,a.[PID]
                                  ,a.[ActivityPrizeName]
                                  ,a.[PicUrl]
                                  ,a.[CouponCount]
                                  ,a.[Stock]
                                  ,a.[SumStock]
                                  ,a.[OnSale]
                                  ,a.[GetRuleId]
                                  ,a.[CreateDatetime]
                                  ,a.[LastUpdateDateTime]
                                  ,a.[IsDeleted]
                                  ,CONVERT(VARCHAR, (CONVERT(VARBINARY,a.[timestamp])),1) as timestamp
                                  ,a.[IsDisableSale]
                              FROM [Activity].[dbo].[tbl_ActivityPrize] as a with (nolock)
                              " + where + @"
                              " + orderby + @"
                          ";

            var result = new PagedModel<ActivityPrizeModel>();

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);

                var list = await DbHelper.ExecuteSelectAsync<ActivityPrizeModel>(true, cmd);
                result.Source = list;
            }

            return result;
        }

        /// <summary>
        ///     判断兑换品是否已经被兑换过
        /// </summary>
        /// <param name="isReadOnly">是否读取读库</param>
        /// <returns></returns>
        public static async Task<bool> GetExistsActivityPrizeOrderDetail(bool isReadOnly, Guid userId, long activityPrizeId, long activityId)
        {
            var sql = @" SELECT count(*)
                          FROM [Activity].[dbo].[tbl_ActivityPrizeOrderDetail] with (nolock) where ActivityId = @ActivityId and userId=@userId and  ActivityPrizeId = @ActivityPrizeId";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@userId", userId);
                cmd.AddParameter("@ActivityPrizeId", activityPrizeId);

                var result = Convert.ToInt32(await DbHelper.ExecuteScalarAsync(isReadOnly, cmd));

                return result > 0;
            }
        }


        /// <summary>
        ///     获取兑换品
        /// </summary>
        /// <returns></returns>
        public static async Task<ActivityPrizeModel> GetActivityPrizeById(bool readOnly, long activityPrizeId)
        {
            var sql = @" SELECT top 1 [PKID]
                                  ,[ActivityId]
                                  ,[PID]
                                  ,[ActivityPrizeName]
                                  ,[PicUrl]
                                  ,[CouponCount]
                                  ,[Stock]
                                  ,[SumStock]
                                  ,[OnSale]
                                  ,[GetRuleId]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                                  ,CONVERT(VARCHAR, (CONVERT(VARBINARY,[timestamp])),1) as timestamp
                                  ,[IsDisableSale]
                          FROM [Activity].[dbo].[tbl_ActivityPrize] with (nolock) where PKID = @PKID ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@PKID", activityPrizeId);

                var result = await DbHelper.ExecuteFetchAsync<ActivityPrizeModel>(readOnly, cmd);

                return result;
            }
        }

        /// <summary>
        ///     减少兑换奖品库存
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="activityPrizeId"></param>
        /// <param name="stockCount">要减少的库存</param>
        /// <param name="timestamp">乐观锁</param>
        /// <returns></returns>
        public static async Task<bool> ActivityPrizeConsumptionStock(BaseDbHelper helper, long activityPrizeId, int stockCount, string timestamp)
        {
            const string sql = @" update [Activity].[dbo].[tbl_ActivityPrize]
                            set [Stock] = [Stock] - @stockCount,LastUpdateDateTime = getdate()
                            where pkid = @pkid and Stock>=@stockCount and CONVERT(VARCHAR, (CONVERT(VARBINARY,[timestamp])),1) = @timestamp";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@PKID", activityPrizeId);
                cmd.AddParameter("@stockCount", stockCount);
                cmd.AddParameter("@timestamp", timestamp);

                var result = await helper.ExecuteNonQueryAsync(cmd);

                return result > 0;
            }
        }

        /// <summary>
        ///     减少用户兑换券
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="pkid"></param>
        /// <param name="coupon"></param>
        /// <param name="timestamp">乐观锁</param>
        /// <returns></returns>
        public static async Task<bool> ActivityCouponConsumptionCoupon(BaseDbHelper helper, long pkid, int coupon, string timestamp)
        {
            const string sql = @" update [Activity].[dbo].[tbl_ActivityCoupon]
                            set [CouponCount] = [CouponCount] - @coupon, LastUpdateDateTime = getdate()
                            where pkid = @pkid and CouponCount>=@coupon and CONVERT(VARCHAR, (CONVERT(VARBINARY,[timestamp])),1) = @timestamp";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@PKID", pkid);
                cmd.AddParameter("@coupon", coupon);
                cmd.AddParameter("@timestamp", timestamp);

                var result = await helper.ExecuteNonQueryAsync(cmd);

                return result > 0;
            }
        }


        /// <summary>
        ///     添加换品订单明细
        /// </summary>
        /// <returns></returns>
        public static async Task<long> InsertActivityPrizeOrderDetail(BaseDbHelper helper, ActivityPrizeOrderDetailModel activityPrizeOrderDetailModel)
        {
            var sql = @" INSERT INTO [Activity].[dbo].[tbl_ActivityPrizeOrderDetail]
                                                           ([ActivityId]
                                                           ,[ActivityName]
                                                           ,[UserId]
                                                           ,[ActivityPrizeId]
                                                           ,[ActivityPrizePicUrl]
                                                           ,[ActivityPrizeName]
                                                           ,[CouponCount]
                                                           ,[CreateDatetime]
                                                           ,[LastUpdateDateTime])
                                                     VALUES
                                                           (@ActivityId
                                                           ,@ActivityName
                                                           ,@UserId
                                                           ,@ActivityPrizeId
                                                           ,@ActivityPrizePicUrl
                                                           ,@ActivityPrizeName
                                                           ,@CouponCount
                                                           ,getdate()
                                                           ,getdate());
                                                SELECT SCOPE_IDENTITY();
                                        ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityPrizeOrderDetailModel.ActivityId);
                cmd.AddParameter("@ActivityName", activityPrizeOrderDetailModel.ActivityName ?? "");
                cmd.AddParameter("@UserId", activityPrizeOrderDetailModel.UserId);
                cmd.AddParameter("@ActivityPrizeId", activityPrizeOrderDetailModel.ActivityPrizeId);
                cmd.AddParameter("@ActivityPrizePicUrl", activityPrizeOrderDetailModel.ActivityPrizePicUrl ?? "");
                cmd.AddParameter("@ActivityPrizeName", activityPrizeOrderDetailModel.ActivityPrizeName ?? "");
                cmd.AddParameter("@CouponCount", activityPrizeOrderDetailModel.CouponCount);

                var result = await helper.ExecuteScalarAsync(cmd);

                return Convert.ToInt64(result);
            }
        }

        /// <summary>
        ///     添加兑换券明细
        /// </summary>
        /// <returns></returns>
        public static async Task<long> InsertActivityCouponDetail(BaseDbHelper helper, ActivityCouponDetailModel activityCouponDetailModel)
        {
            var sql = @" INSERT INTO [Activity].[dbo].[tbl_ActivityCouponDetail]
                                                         ([ActivityId]
                                                           ,[ActivityName]
                                                           ,[UserId]
                                                           ,[CouponCount]
                                                           ,[CouponName]
                                                           ,[CreateDateTime]
                                                           ,[LastUpdateDateTime]
                                                            )
                                                     VALUES
                                                           (@ActivityId
                                                           ,@ActivityName
                                                           ,@UserId
                                                           ,@CouponCount
                                                           ,@CouponName
                                                           ,getdate()
                                                           ,getdate()

                                                            );
                                                SELECT SCOPE_IDENTITY();
                                        ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityCouponDetailModel.ActivityId);
                cmd.AddParameter("@ActivityName", activityCouponDetailModel.ActivityName ?? "");
                cmd.AddParameter("@UserId", activityCouponDetailModel.UserId);
                cmd.AddParameter("@CouponCount", activityCouponDetailModel.CouponCount);
                cmd.AddParameter("@CouponName", activityCouponDetailModel.CouponName ?? "");

                var result = await helper.ExecuteScalarAsync(cmd);

                return Convert.ToInt64(result);
            }
        }

        /// <summary>
        ///     获取活动兑换物记录
        /// </summary>
        /// <returns></returns>
        public static async Task<PagedModel<ActivityPrizeOrderDetailModel>> SearchPrizeOrderDetail(Guid userId, long activityId, int pageIndex = 1, int pageSize = 20)
        {
            var sql = @"
                            SELECT [PKID]
                                  ,[ActivityId]
                                  ,[ActivityName]
                                  ,[UserId]
                                  ,[ActivityPrizeId]
                                  ,[ActivityPrizePicUrl]
                                  ,[ActivityPrizeName]
                                  ,[CouponCount]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                              FROM [Activity].[dbo].[tbl_ActivityPrizeOrderDetail] with (nolock)
                              WHERE ActivityId = @ActivityId and UserId = @UserId
                              order by CreateDatetime desc
                              OFFSET (@pageIndex -1) * @pageSize ROWS
                              FETCH NEXT @pageSize ROWS ONLY
                          ";
            var sqlCount = @"SELECT
                                        Count(*)
                                        FROM [Activity].[dbo].[tbl_ActivityPrizeOrderDetail] with (nolock)
                                        where ActivityId = @ActivityId  and UserId = @UserId ";

            var result = new PagedModel<ActivityPrizeOrderDetailModel>();

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@UserId", userId);
                cmd.AddParameter("@pageIndex", pageIndex);
                cmd.AddParameter("@pageSize", pageSize);

                var list = await DbHelper.ExecuteSelectAsync<ActivityPrizeOrderDetailModel>(true, cmd);
                result.Source = list;
            }

            using (var cmd = new SqlCommand(sqlCount))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@UserId", userId);


                var count = Convert.ToInt32(await DbHelper.ExecuteScalarAsync(cmd));
                result.Pager = new PagerModel(pageIndex, pageSize)
                {
                    Total = count
                };
            }
            return result;
        }

        /// <summary>
        ///     获取活动兑换物记录 只取ID
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<ActivityPrizeOrderDetailModel>> SearchPrizeOrderDetailID(bool readOnly, Guid userId, long activityId)
        {
            var sql = @"
                            SELECT [PKID]
                                  ,[ActivityPrizeId]
                              FROM [Activity].[dbo].[tbl_ActivityPrizeOrderDetail] with (nolock)
                              WHERE ActivityId = @ActivityId and UserId = @UserId
                          ";

            var result = new PagedModel<ActivityPrizeOrderDetailModel>();

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@UserId", userId);

                var list = await DbHelper.ExecuteSelectAsync<ActivityPrizeOrderDetailModel>(readOnly, cmd);
                result.Source = list;
            }

            return result;
        }

        /// <summary>
        ///     通过活动ID返回活动等级
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<ActivityLevelModel>> GetActivityLevelByActivityId(long activityId)
        {
            var sql = @" SELECT [PKID]
                              ,[ActivityId]
                              ,[LevelName]
                              ,[StartCount]
                              ,[EndCount]
                              ,[CreateDatetime]
                              ,[LastUpdateDateTime]
                          FROM [Activity].[dbo].[tbl_ActivityLevel] with (nolock) where ActivityId = @ActivityId order by StartCount asc  ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                var result = await DbHelper.ExecuteSelectAsync<ActivityLevelModel>(true, cmd);
                return result;
            }
        }

        /// <summary>
        ///     查询用户的活动分享数据
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<ActivityShareDetailModel>> SearchActivityShareDetail(long activityId, Guid userId, DateTime? shareDate)
        {
            var where = " where 1=1 and  ActivityId = @ActivityId and UserId = @UserId ";

            if (shareDate != null)
            {
                where = where + " and  CONVERT(varchar(100), ShareTime, 23) = @shareDate  ";
            }

            var sql = @" SELECT [PKID]
                                  ,[ActivityId]
                                  ,[UserId]
                                  ,[ShareTime]
                                  ,[ShareName]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                          FROM [Activity].[dbo].[tbl_ActivityShareDetail] with (nolock)    " + where
                          + " order by ShareTime desc ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@UserId", userId);
                cmd.AddParameter("@shareDate", shareDate?.ToString("yyyy-MM-dd"));
                var result = await DbHelper.ExecuteSelectAsync<ActivityShareDetailModel>(cmd);
                return result;
            }
        }

        /// <summary>
        ///     添加用户的活动分享数据
        /// </summary>
        /// <returns></returns>
        public static async Task<long> InsertActivityShareDetail(BaseDbHelper helper, ActivityShareDetailModel activityShareDetailModel)
        {

            var sql = @"
                            INSERT INTO [Activity].[dbo].[tbl_ActivityShareDetail]
                                                   ([ActivityId]
                                                   ,[UserId]
                                                   ,[ShareTime]
                                                   ,[ShareName]
                                                   ,[CreateDatetime]
                                                   ,[LastUpdateDateTime])
                                             VALUES
                                                   (@ActivityId
                                                   ,@UserId
                                                   ,@ShareTime
                                                   ,@ShareName
                                                   ,getdate()
                                                   ,getdate());
                                             SELECT SCOPE_IDENTITY();
                        ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityShareDetailModel.ActivityId);
                cmd.AddParameter("@UserId", activityShareDetailModel.UserId);
                cmd.AddParameter("@ShareTime", activityShareDetailModel.ShareTime);
                cmd.AddParameter("@ShareName", activityShareDetailModel.ShareName ?? "");

                var result = await helper.ExecuteScalarAsync(cmd);

                return Convert.ToInt64(result);
            }
        }


        /// <summary>
        ///     获取 兑换券排名设置
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<ActivityCouponRankSettingModel> GetActivityCouponRankSettingByActivityId(long activityId)
        {
            var sql = @" SELECT [PKID]
                              ,[ActivityId]
                              ,[RankHead]
                              ,[RankMiddle]
                              ,[CreateDatetime]
                              ,[LastUpdateDateTime]
                          FROM [Activity].[dbo].[tbl_ActivityCouponRankSetting] with (nolock) where ActivityId = @ActivityId  ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                var result = await DbHelper.ExecuteFetchAsync<ActivityCouponRankSettingModel>(true, cmd);
                return result;
            }
        }


        /// <summary>
        ///     添加活动用户兑换券
        /// </summary>
        /// <returns></returns>
        public static async Task<long> InsertActivityCoupon(BaseDbHelper helper, ActivityCouponModel activityCouponModel)
        {
            var sql = @" INSERT INTO [Activity].[dbo].[tbl_ActivityCoupon]
                                                           ([ActivityId]
                                                           ,[UserId]
                                                           ,[CouponCount]
                                                           ,[CouponSum])
                                                     VALUES
                                                           (@ActivityId
                                                           ,@UserId
                                                           ,@CouponCount
                                                           ,@CouponSum);
                                                SELECT SCOPE_IDENTITY();
                                        ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityCouponModel.ActivityId);
                cmd.AddParameter("@UserId", activityCouponModel.UserId);
                cmd.AddParameter("@CouponCount", activityCouponModel.CouponCount);
                cmd.AddParameter("@CouponSum", activityCouponModel.CouponSum);

                var result = await helper.ExecuteScalarAsync(cmd);

                return Convert.ToInt64(result);
            }
        }


        /// <summary>
        ///     更新或者添加活动用户兑换券  返回主键
        /// </summary>
        /// <returns></returns>
        public static async Task<long> ModifyActivityCoupon(BaseDbHelper helper, Guid userId, long activityId, int couponCount, DateTime? modifyDateTime = null)
        {
            var sql = @"
                        DECLARE @Id bigint;
                        MERGE INTO [Activity].[dbo].[tbl_ActivityCoupon]  as X
                        USING  (
                            VALUES(@ActivityId, @UserId)
                        ) as S(ActivityId,UserId)
                        ON X.ActivityId = S.ActivityId AND x.UserId = S.UserId
                        WHEN NOT MATCHED THEN
	                         INSERT (ActivityId,UserId,CouponCount,CouponSum)
	                         VALUES (@ActivityId,@UserId,0,0)
                        /*
                        WHEN MATCHED THEN
	                         UPDATE SET X.CouponCount = X.CouponCount + @CouponCount,X.CouponSum = X.CouponSum + @CouponSum,X.LastUpdateDateTime = getdate(),@Id = x.PKID
                        */
                        ;
                        --查找主键
                        set @Id = SCOPE_IDENTITY();
                        IF @Id IS NULL
                            begin
                                select @Id = PKID from [Activity].[dbo].[tbl_ActivityCoupon] where ActivityId=@ActivityId and UserId = @UserId
	                        end
                        SELECT @Id;


                                        ";

            //判断是否更新最后更新日期
            var lastUpdateSQL = "";
            //兑换券正的时候 更新
            if (modifyDateTime != null)
            {
                lastUpdateSQL = ",LastUpdateDateTime = @modifyDateTime ";
            }
            else if (couponCount > 0)
            {
                lastUpdateSQL = ",LastUpdateDateTime = getdate()";
            }

            var sqlUpdate = @"

                        --获取乐观锁
                        DECLARE @versionId timestamp;

                        select @versionId = timestamp from [Activity].[dbo].[tbl_ActivityCoupon] where  pkid = @id

                        update [Activity].[dbo].[tbl_ActivityCoupon]
                        set CouponCount = CouponCount + @CouponCount , CouponSum  = CouponSum +  @CouponSum " + lastUpdateSQL + @"
                        where   pkid  = @Id and timestamp = @versionId and (CouponCount + @CouponCount) >= 0

                    ";
            var pkid = 0L;
            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@UserId", userId);

                var result = await helper.ExecuteScalarAsync(cmd);
                pkid = Convert.ToInt64(result);
            }

            using (var cmd = new SqlCommand(sqlUpdate))
            {
                cmd.AddParameter("@CouponCount", couponCount);
                cmd.AddParameter("@CouponSum", (couponCount < 0 ? 0 : couponCount));
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@id", pkid);
                if (modifyDateTime != null)
                {
                    cmd.AddParameter("@modifyDateTime", modifyDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                var result = await helper.ExecuteNonQueryAsync(cmd);
                if (result == 0)
                {
                    pkid = -1;
                }
            }


            return Convert.ToInt64(pkid);
        }
        #endregion


        #region 大客户活动专享配置

        /// <summary>
        ///根据活动专享ID查询活动配置信息
        /// </summary>
        /// <param name="activityExclusiveId">活动专享ID</param>
        /// <param name="readOnly">是否只读库</param>
        /// <returns></returns>
        public static async Task<ActiveCustomerSettingResponse> SelectCustomerExclusiveSettingInfo(string activityExclusiveId, bool readOnly)
        {
            ActiveCustomerSettingResponse activeCustomerSettingResponse = new ActiveCustomerSettingResponse();
            try
            {
                const string sql = @"SELECT
     	               [PKID]
                      ,[ActivityExclusiveId]
                      ,[OrderChannel]
                      ,[LargeCustomersID]
                      ,[LargeCustomersName]
                      ,[EventLink]
                      ,[ActivityId]
                      ,[ImageUrl]
                      ,[BusinessHotline]
                      ,[IsEnable]
                      ,[IsDelete]
                      ,[CreateTime]
                      ,[CreateBy]
                      ,[UpdateDatetime]
                      ,[UpdateBy]
	                  FROM  Activity.[dbo].[CustomerExclusiveSetting] WITH(NOLOCK)
	                WHERE [IsDelete] = 0  AND ActivityExclusiveId=@activityExclusiveId;";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@activityExclusiveId", activityExclusiveId);
                    cmd.CommandType = CommandType.Text;
                    activeCustomerSettingResponse = await DbHelper.ExecuteFetchAsync<ActiveCustomerSettingResponse>(readOnly, cmd);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return activeCustomerSettingResponse;
        }

        /// <summary>
        /// 查询用户绑定的券码
        /// </summary>
        /// <param name="activityExclusiveId">活动专享Id</param>
        /// <param name="userid">用户Id</param>
        /// <returns></returns>
        public static async Task<string> SelectUserCouponCode(string activityExclusiveId, string userid, bool readOnly)
        {
            string result = string.Empty;

            try
            {
                const string sql = @"SELECT CouponCode FROM  Activity.[dbo].[CustomerExclusiveCoupon] WITH(NOLOCK)
                                    WHERE IsDelete = 0 AND STATUS=0 AND ActivityExclusiveId = @ActivityExclusiveId AND UserId = @UserId";

                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@ActivityExclusiveId", activityExclusiveId);
                    cmd.Parameters.AddWithValue("@UserId", userid);
                    cmd.CommandType = CommandType.Text;
                    result = (await DbHelper.ExecuteScalarAsync(readOnly, cmd) + "");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;
        }

        /// <summary>
        /// 用户券码绑定
        /// </summary>
        /// <param name="activityCustomerCouponRequests"></param>
        /// <returns></returns>
        public static async Task<int> CouponCodeBound(ActivityCustomerCouponRequests activityCustomerCouponRequests)
        {
            int result = 0;

            try
            {
                const string sql = @"UPDATE Activity.[dbo].[CustomerExclusiveCoupon] WITH(ROWLOCK)
                                   SET [UserName] = @UserName
                                      ,[UserId] = @UserId
                                      ,[Phone] = @Phone
                                      ,[UpdateDateTime] = GETDATE()
                                      ,[UpdateBy] = @UpdateBy
                                 WHERE  ActivityExclusiveId = @ActivityExclusiveId
                                  AND	CouponCode = @CouponCode";

                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@UserName", activityCustomerCouponRequests.UserName);
                    cmd.Parameters.AddWithValue("@UserId", activityCustomerCouponRequests.UserId);
                    cmd.Parameters.AddWithValue("@Phone", activityCustomerCouponRequests.Phone);
                    cmd.Parameters.AddWithValue("@UpdateBy", activityCustomerCouponRequests.UserName);
                    cmd.Parameters.AddWithValue("@ActivityExclusiveId", activityCustomerCouponRequests.ActivityExclusiveId);
                    cmd.Parameters.AddWithValue("@CouponCode", activityCustomerCouponRequests.CouponCode);
                    cmd.CommandType = CommandType.Text;

                    result = await DbHelper.ExecuteNonQueryAsync(false, cmd);

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;
        }

        /// <summary>
        /// 查询券码是否存在
        /// </summary>
        /// <param name="activityExclusiveId">活动专享ID</param>
        /// <param name="CouponCode">券码</param>
        /// <returns></returns>
        public static async Task<int> SelectCouponCode(string activityExclusiveId, string CouponCode)
        {
            int reslut = 0;

            try
            {
                const string sql = @"SELECT COUNT(1) FROM  Activity.[dbo].[CustomerExclusiveCoupon] WITH(NOLOCK)
                                    WHERE IsDelete = 0 AND STATUS=0 AND ActivityExclusiveId = @ActivityExclusiveId AND CouponCode = @CouponCode AND UserId IS  NULL";

                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@ActivityExclusiveId", activityExclusiveId);
                    cmd.Parameters.AddWithValue("@CouponCode", CouponCode);
                    cmd.CommandType = CommandType.Text;
                    reslut = Convert.ToInt32(await DbHelper.ExecuteScalarAsync(true, cmd));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return reslut;
        }

        /// <summary>
        /// 根据限时抢购ID查询活动配置信息
        /// </summary>
        /// <param name="ActivityId"></param>
        /// <returns></returns>
        public static async Task<ActiveCustomerSettingResponse> SelectCustomerSettingActivityId(string ActivityId)
        {
            ActiveCustomerSettingResponse activeCustomerSettingResponse = new ActiveCustomerSettingResponse();

            try
            {
                const string sql = @"SELECT
     	               [PKID]
                      ,[ActivityExclusiveId]
                      ,[OrderChannel]
                      ,[LargeCustomersID]
                      ,[LargeCustomersName]
                      ,[EventLink]
                      ,[ActivityId]
                      ,[ImageUrl]
                      ,[BusinessHotline]
                      ,[IsEnable]
                      ,[IsDelete]
                      ,[CreateTime]
                      ,[CreateBy]
                      ,[UpdateDatetime]
                      ,[UpdateBy]
	                  FROM  Activity.[dbo].[CustomerExclusiveSetting] WITH(NOLOCK)
	                WHERE [IsDelete] = 0  AND IsEnable = 1 AND ActivityId=@ActivityId;";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@ActivityId", ActivityId);
                    cmd.CommandType = CommandType.Text;
                    activeCustomerSettingResponse = await DbHelper.ExecuteFetchAsync<ActiveCustomerSettingResponse>(true, cmd);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return activeCustomerSettingResponse;
        }

        /// <summary>
        /// 获取有效的大客户配置信息
        /// </summary>
        /// <returns></returns>
        public static async Task<List<ActiveCustomerSettingResponse>> SelectValidCustomerSetting()
        {
            var resultList = new List<ActiveCustomerSettingResponse>();

            try
            {
                string sql = @"SELECT ActivityId,ActivityExclusiveId,CreateTime
                                FROM  Activity.[dbo].[CustomerExclusiveSetting] WITH (NOLOCK)
                                WHERE IsDelete = 0
                                      AND IsEnable = 1;";
                using (var cmd = new SqlCommand(sql))
                {
                    resultList = (await DbHelper.ExecuteSelectAsync<ActiveCustomerSettingResponse>(cmd))?.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"SelectValidCustomerSetting异常,ex:{ex}");
            }

            return resultList ?? new List<ActiveCustomerSettingResponse>();
        }

        /// <summary>
        /// 获取活动最早绑定券码的时间
        /// </summary>
        /// <returns></returns>
        public static async Task<List<ActiveCustomerSettingResponse>> GetCouponFirstCreateTimes()
        {
            var resultList = new List<ActiveCustomerSettingResponse>();

            try
            {
                string sql = @"SELECT ActivityExclusiveId,
                                      MIN(CreateTime) AS CreateTime
                                FROM Activity.[dbo].[CustomerExclusiveCoupon] WITH (NOLOCK)
                                WHERE ActivityExclusiveId IS NOT NULL 
                                GROUP BY ActivityExclusiveId;";
                using (var cmd = new SqlCommand(sql))
                {
                    resultList = (await DbHelper.ExecuteSelectAsync<ActiveCustomerSettingResponse>(cmd))?.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetCouponFirstCreateTimes异常,ex:{ex}");
            }

            return resultList ?? new List<ActiveCustomerSettingResponse>();
        }


        /// <summary>
        /// 根据活动id 获取活动订单记录信息
        /// </summary>
        /// <param name="activityIDs"></param>
        /// <returns></returns>
        public static async Task<List<ActivityProductOrderRecordsModel>> SelectActivityProductOrderRecords(List<Guid> activityIDs)
        {
            var resultList = new List<ActivityProductOrderRecordsModel>();

            try
            {
                string sql = @"SELECT  [Pkid]
                                      ,[ActivityId]
                                      ,[Type]
                                      ,[Pid]
                                      ,[UserId]
                                      ,[Phone]
                                      ,[OrderId]
                                      ,[Quantity] 
                                      ,[OrderStatus]
                                      ,[AllPlaceLimitId]
                                      ,[CreateDateTime]
                                  FROM [Tuhu_log].[dbo].[ActivityProductOrderRecords] r  WITH(NOLOCK)
                                  JOIN [Tuhu_log].[dbo].SplitString(@activityIDs,',',1) AS B ON r.ActivityId=B.Item
                                  WHERE r.OrderStatus='0New'
                                        AND DATEDIFF(dd, CreateDateTime, GETDATE()) < 731;";
                using (var dbhelper = DbHelper.CreateLogDbHelper(true))
                {
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.Parameters.AddWithValue("@activityIDs", string.Join(",", activityIDs));
                        resultList = (await dbhelper.ExecuteSelectAsync<ActivityProductOrderRecordsModel>(cmd))?.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"SelectValidCustomerSetting异常,ex:{ex}");
            }

            return resultList;
        }

        /// <summary>
        /// 根据userids获取大客户券信息
        /// </summary>
        /// <param name="userIDs"></param>
        /// <returns></returns>
        public static async Task<List<ActiveCustomerCouponModel>> SelectCustomerSettingByUserIds(List<Guid> userIDs)
        {
            if (!(bool)(userIDs?.Any()))
            {
                return new List<ActiveCustomerCouponModel>();
            }

            var resultList = new List<ActiveCustomerCouponModel>();

            try
            {
                string sql = @"SELECT c.CouponCode,
                                      c.UserId,
                                      c.ActivityExclusiveId,
                                      s.ActivityId,
                                      c.Status,
                                      c.CreateTime,
                                      c.UpdateDateTime
                                FROM Activity.[dbo].[CustomerExclusiveCoupon] c WITH (NOLOCK)
                                     JOIN Activity.[dbo].[CustomerExclusiveSetting] s WITH (NOLOCK)
                                        ON c.ActivityExclusiveId = s.ActivityExclusiveId
                                           AND s.IsDelete = 0
                                           AND s.IsEnable = 1
                                    JOIN Activity.[dbo].SplitString(@userIDs, ',', 1) B
                                        ON c.UserId = B.Item
                                WHERE c.UserId IS NOT NULL
                                      AND c.IsDelete=0;";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userIDs", string.Join(",", userIDs));
                    resultList = (await DbHelper.ExecuteSelectAsync<ActiveCustomerCouponModel>(cmd))?.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"SelectValidCustomerSetting异常,ex:{ex}");
            }

            return resultList ?? new List<ActiveCustomerCouponModel>();
        }

        #endregion

        /// <summary>
        /// 添加途虎星级认证门店
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<int> AddStarRatingStoreAsync(StarRatingStoreModel model)
        {
            var sql = @"INSERT INTO [Activity].[dbo].[StarRatingStore] WITH(ROWLOCK)
           ([UserName]
           ,[Phone]
           ,[StoreName]
           ,[Duty]
           ,[ProvinceID]
           ,[CityID]
           ,[DistrictID]
           ,[ProvinceName]
           ,[CityName]
           ,[DistrictName]
           ,[StoreAddress]
           ,[Area]
           ,[StoreArea]
           ,[StoreNum]
           ,[WorkPositionNum]
           ,[MaintainQualification]
           ,[Storefront]
           ,[StorefrontDesc]
           ,[StoreLocation]
           ,[IsAgree]
           ,[CreateDateTime]
           ,[LastUpdateDateTime]
			)
			VALUES(
			@UserName
           ,@Phone
           ,@StoreName
           ,@Duty
           ,@ProvinceID
           ,@CityID
           ,@DistrictID
           ,@ProvinceName
           ,@CityName
           ,@DistrictName
           ,@StoreAddress
           ,@Area
           ,@StoreArea
           ,@StoreNum
           ,@WorkPositionNum
           ,@MaintainQualification
           ,@Storefront
           ,@StorefrontDesc
           ,@StoreLocation
           ,@IsAgree
           ,GETDATE()
           ,GETDATE()
			)";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@UserName", model.UserName);
                cmd.Parameters.AddWithValue("@Phone", model.Phone);
                cmd.Parameters.AddWithValue("@StoreName", model.StoreName);
                cmd.Parameters.AddWithValue("@Duty", model.Duty);
                cmd.Parameters.AddWithValue("@ProvinceID", model.ProvinceID);
                cmd.Parameters.AddWithValue("@CityID", model.CityID);
                cmd.Parameters.AddWithValue("@DistrictID", model.DistrictID);
                cmd.Parameters.AddWithValue("@ProvinceName", model.ProvinceName);
                cmd.Parameters.AddWithValue("@CityName", model.CityName);
                cmd.Parameters.AddWithValue("@DistrictName", model.DistrictName);
                cmd.Parameters.AddWithValue("@StoreAddress", model.StoreAddress);
                cmd.Parameters.AddWithValue("@Area", model.ProvinceName + model.CityName + model.DistrictName + model.StoreAddress);
                cmd.Parameters.AddWithValue("@StoreArea", model.StoreArea);
                cmd.Parameters.AddWithValue("@StoreNum", model.StoreNum);
                cmd.Parameters.AddWithValue("@WorkPositionNum", model.WorkPositionNum);
                cmd.Parameters.AddWithValue("@MaintainQualification", model.MaintainQualification);
                cmd.Parameters.AddWithValue("@Storefront", model.Storefront);
                cmd.Parameters.AddWithValue("@StorefrontDesc", model.StorefrontDesc);
                cmd.Parameters.AddWithValue("@StoreLocation", model.StoreLocation);
                cmd.Parameters.AddWithValue("@IsAgree", model.IsAgree);
                var result = Convert.ToInt32(await DbHelper.ExecuteNonQueryAsync(cmd));
                return result;
            }
        }

        /// <summary>
        /// 根据手机号获取途虎星级认证门店
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static async Task<int> GetStarRatingStoreByPhone(string phone)
        {
            var sql = @"SELECT  COUNT(*) FROM  [Activity].[dbo].[StarRatingStore] with(nolock) WHERE Phone=@Phone";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@Phone", phone);
                var count = Convert.ToInt32(await DbHelper.ExecuteScalarAsync(true, cmd));
                return count;
            }
        }

        /// <summary>
        /// 根据限时抢购ID查询活动配置信息 (锦湖员工轮胎)
        /// </summary>
        /// <param name="activityId">限时抢购ID</param>
        /// <returns></returns>
        public static async Task<ActiveCustomerSettingResponse> SelectCustomerSettingInfoByActivityId(string activityId)
        {
            ActiveCustomerSettingResponse activeCustomerSettingResponse = new ActiveCustomerSettingResponse();
            try
            {
                const string sql = @"SELECT
     	               [PKID]
                      ,[ActivityExclusiveId]
                      ,[OrderChannel]
                      ,[LargeCustomersID]
                      ,[LargeCustomersName]
                      ,[EventLink]
                      ,[ActivityId]
                      ,[ImageUrl]
                      ,[BusinessHotline]
                      ,[IsEnable]
                      ,[IsDelete]
                      ,[CreateTime]
                      ,[CreateBy]
                      ,[UpdateDatetime]
                      ,[UpdateBy]
	                  FROM  Activity.[dbo].[CustomerExclusiveSetting] WITH(NOLOCK)
	                WHERE [IsDelete] = 0  AND ActivityId=@activityId;";
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@activityId", activityId);
                    cmd.CommandType = CommandType.Text;
                    activeCustomerSettingResponse = await DbHelper.ExecuteFetchAsync<ActiveCustomerSettingResponse>(true, cmd);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return activeCustomerSettingResponse;
        }

        #region 活动报名页

        /// <summary>
        /// 添加活动报名页数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<bool> AddRegistrationOfActivitiesDataAsync(RegistrationOfActivitiesRequest request)
        {
            string sql = @"
                        INSERT INTO [Activity].[dbo].[RegistrationOfActivities]
                        (
                            [UserId],
                            [UserName],
                            [ClientIp],
                            [Phone],
                            [ApplicationReasons],
                            [Pictures],
                            [Is_Deleted],
                            [CreateDateTime],
                            [LastUpdateDateTime]
                        )
                        VALUES
                        (   @UserId,
                            @UserName,
                            @ClientIp,
                            @Phone,
                            @ApplicationReasons,
                            @Pictures,
                            0,
                            GETDATE(),
                            GETDATE()
                        );SELECT SCOPE_IDENTITY();";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@UserId", request.UserId);
                cmd.Parameters.AddWithValue("@UserName", request.UserName ?? "");
                cmd.Parameters.AddWithValue("@ClientIp", request.ClientIp ?? "");
                cmd.Parameters.AddWithValue("@Phone", request.Phone ?? "");
                cmd.Parameters.AddWithValue("@ApplicationReasons", request.ApplicationReasons ?? "");
                cmd.Parameters.AddWithValue("@Pictures", JsonConvert.SerializeObject(request.Pictures));
                return (await DbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }
        #endregion
    }
}

