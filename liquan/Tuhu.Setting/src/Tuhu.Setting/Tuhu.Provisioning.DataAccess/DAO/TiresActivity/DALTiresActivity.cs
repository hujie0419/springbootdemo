using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.TiresActivity;

namespace Tuhu.Provisioning.DataAccess.DAO.TiresActivity
{
    public static class DALTiresActivity
    {

        #region  活动配置
        public static List<TiresActivityModel> SelectTiresActivity(SqlConnection conn, string activityName, DateTime? startTime, DateTime? endTime, Guid activityId, int pageIndex, int pageSize)
        {
            const string sql = @"SELECT rmc.PKID ,
            rmc.ActivityId ,
            rmc.ActivityName ,
            rmc.HeadImg ,
            rmc.NoAdaptationImg ,
            rmc.WXUrl ,
            rmc.AppUrl ,
            rmc.ShareImg ,
            rmc.ShareTitle ,
            rmc.ShareDes ,
            rmc.IsAdaptationVehicle ,
            rmc.IsShowInstallmentPrice ,
            rmc.StartTime ,
            rmc.EndTime ,
            rmc.ActivityRules ,
            rmc.ActivityRulesImg ,
            rmc.BackgroundColor ,
            rmc.Owner ,
            rmc.CreateDateTime ,
            rmc.LastUpdateDateTime ,
            COUNT(1) OVER ( ) AS Total
     FROM   Configuration..TiresActivityConfig AS rmc WITH ( NOLOCK )
     WHERE  ( @ActivityId = '00000000-0000-0000-0000-000000000000'
              OR rmc.ActivityId = @ActivityId
            )
            AND ( @ActivityName = ''
                  OR @ActivityName IS NULL
                  OR rmc.ActivityName LIKE '%' + @ActivityName + '%'
                )
            AND ( @StartTime = ''
                  OR @StartTime IS NULL
                  OR rmc.StartTime >= @StartTime
                )
            AND ( @EndTime = ''
                  OR @EndTime IS NULL
                  OR rmc.EndTime <= @EndTime
                )
     ORDER BY rmc.PKID DESC
            OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
            ONLY;";

            return conn.Query<TiresActivityModel>(sql, new
            {
                ActivityId = activityId,
                ActivityName = activityName,
                StartTime = startTime,
                EndTime = endTime,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }

        public static TiresActivityModel SelectRegionConfigByActivityId(SqlConnection conn, Guid activityId)
        {
            const string sql = @"SELECT  rmc.PKID ,
            rmc.ActivityId ,
            rmc.WXUrl ,
            rmc.AppUrl ,
            rmc.ShareImg ,
            rmc.ShareTitle ,
            rmc.ShareDes ,
            rmc.IsAdaptationVehicle ,
            rmc.IsShowInstallmentPrice ,
            CASE WHEN rmc.StartTime IS NOT NULL THEN rmc.StartTime
                 ELSE tfs.StartDateTime
            END AS StartTime ,
            CASE WHEN rmc.EndTime IS NOT NULL THEN rmc.EndTime
                 ELSE tfs.EndDateTime
            END AS EndTime ,
            rmc.ActivityRules ,
            rmc.ActivityRulesImg ,
            rmc.CreatedTime ,
            tfs.ActivityName
    FROM    Configuration..RegionMarketingConfig AS rmc WITH ( NOLOCK )
            RIGHT JOIN Activity.dbo.tbl_FlashSale AS tfs WITH ( NOLOCK ) ON rmc.ActivityId = tfs.ActivityID
    WHERE   tfs.ActivityId = @ActivityId;";

            return conn.Query<TiresActivityModel>(sql, new { ActivityId = activityId }, commandType: CommandType.Text).SingleOrDefault();
        }

        public static int UpdateTiresActivityConfig(SqlConnection conn, TiresActivityModel data, string user)
        {
            const string sql = @"
            UPDATE  Configuration..TiresActivityConfig
            SET     WXUrl = @WXUrl ,
                    AppUrl = @AppUrl ,
                    ShareImg = @ShareImg ,
                    ShareTitle = @ShareTitle ,
                    ShareDes = @ShareDes ,
                    ActivityName = @ActivityName ,
                    IsAdaptationVehicle = @IsAdaptationVehicle ,
                    IsShowInstallmentPrice = @IsShowInstallmentPrice ,
                    StartTime = @StartTime ,
                    EndTime = @EndTime ,
                    ActivityRules = @ActivityRules ,
                    ActivityRulesImg = @ActivityRulesImg ,
                    HeadImg = @HeadImg ,
                    NoAdaptationImg = @NoAdaptationImg ,
                    BackgroundColor = @BackgroundColor ,
                    Owner = @Owner ,
                    LastUpdateDateTime = GETDATE(),
                    UpdateUser = @UpdateUser
            WHERE   ActivityId = @ActivityId;";
            return conn.Execute(sql, new
            {
                ActivityId = data.ActivityId,
                ActivityName = data.ActivityName,
                WXUrl = data.WXUrl,
                AppUrl = data.AppUrl,
                ShareImg = data.ShareImg,
                ShareTitle = data.ShareTitle,
                ShareDes = data.ShareDes,
                IsAdaptationVehicle = data.IsAdaptationVehicle,
                IsShowInstallmentPrice = data.IsShowInstallmentPrice,
                ActivityRules = data.ActivityRules,
                ActivityRulesImg = data.ActivityRulesImg,
                HeadImg = data.HeadImg,
                NoAdaptationImg = data.NoAdaptationImg,
                BackgroundColor = data.BackgroundColor,
                Owner = data.Owner,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                UpdateUser = user
            }, commandType: CommandType.Text);
        }

        public static int InsertTiresActivityConfig(SqlConnection conn, TiresActivityModel data, string user)
        {
            const string sql = @"INSERT INTO Configuration..TiresActivityConfig
                    ( ActivityId ,
                      ActivityName ,
                      WXUrl ,
                      AppUrl ,
                      ShareImg ,
                      ShareTitle ,
                      ShareDes ,
                      IsAdaptationVehicle ,
                      IsShowInstallmentPrice ,
                      StartTime ,
                      EndTime ,
                      ActivityRules ,
                      ActivityRulesImg ,
                      HeadImg ,
                      NoAdaptationImg ,
                      BackgroundColor ,
                      Owner ,
                      CreateDateTime ,
                      CreateUser
                    )
            VALUES  ( @ActivityId ,
                      @ActivityName ,
                      @WXUrl ,
                      @AppUrl ,
                      @ShareImg ,
                      @ShareTitle ,
                      @ShareDes ,
                      @IsAdaptationVehicle ,
                      @IsShowInstallmentPrice ,
                      @StartTime ,
                      @EndTime ,
                      @ActivityRules ,
                      @ActivityRulesImg ,
                      @HeadImg ,
                      @NoAdaptationImg ,
                      @BackgroundColor ,
                      @Owner ,
                      GETDATE() ,
                      @CreateUser
                    )";
            return conn.Execute(sql, new
            {
                ActivityId = data.ActivityId,
                ActivityName = data.ActivityName,
                WXUrl = data.WXUrl,
                AppUrl = data.AppUrl,
                ShareImg = data.ShareImg,
                ShareTitle = data.ShareTitle,
                ShareDes = data.ShareDes,
                IsAdaptationVehicle = data.IsAdaptationVehicle,
                IsShowInstallmentPrice = data.IsShowInstallmentPrice,
                StartTime = data.StartTime,
                ActivityRules = data.ActivityRules,
                ActivityRulesImg = data.ActivityRulesImg,
                HeadImg = data.HeadImg,
                NoAdaptationImg = data.NoAdaptationImg,
                BackgroundColor = data.BackgroundColor,
                Owner = data.Owner,
                EndTime = data.EndTime,
                CreateUser = user
            }, commandType: CommandType.Text);
        }

        public static int DeleteTiresActivityConfig(SqlConnection conn, Guid activityId)
        {
            const string sql = @"DELETE FROM Configuration..TiresActivityConfig WHERE ActivityId=@ActivityId";
            return conn.Execute(sql, new { ActivityId = activityId }, commandType: CommandType.Text);
        }
        #endregion

        #region 楼层配置

        public static int InsertTiresFloorActivity(SqlConnection conn, TiresFloorActivityConfig data)
        {
            const string sql = @"INSERT INTO Configuration..TiresFloorActivityConfig
                (TiresActivityId,
                  FloorActivityId,
                  FlashSaleId,
                  CreateDateTime,
                  LastUpdateDateTime
                )
        VALUES(@TiresActivityId,
                  @FloorActivityId,
                  @FlashSaleId,
                  GETDATE(),
                  GETDATE()
                )";
            return conn.Execute(sql, new { TiresActivityId = data.TiresActivityId, FloorActivityId = data.FloorActivityId, FlashSaleId = data.FlashSaleId }, commandType: CommandType.Text);
        }

        public static int UpdateTiresFloorActivity(SqlConnection conn, TiresFloorActivityConfig data)
        {
            const string sql = "UPDATE Configuration..TiresFloorActivityConfig SET LastUpdateDateTime=GETDATE() WHERE FlashSaleId=@FlashSaleId";
            return conn.Execute(sql, new { FlashSaleId = data.FlashSaleId }, commandType: CommandType.Text);
        }

        public static List<TiresFloorActivityConfig> SelectTiresFloorInfoByParentId(SqlConnection conn, Guid actitivityId)
        {
            const string sql = @"
            SELECT  tfa.* ,
                    fs.ActivityName , fs.StartDateTime AS StartTime  ,fs.EndDateTime AS EndTime 
            FROM    Configuration..TiresFloorActivityConfig AS tfa WITH ( NOLOCK )
                    LEFT JOIN Activity..tbl_FlashSale AS fs WITH ( NOLOCK ) ON tfa.FlashSaleId = fs.ActivityID
            WHERE   tfa.TiresActivityId = @TiresActivityId;";
            return conn.Query<TiresFloorActivityConfig>(sql, new { TiresActivityId = actitivityId }, commandType: CommandType.Text).ToList();
        }

        public static TiresFloorActivityConfig SelectTiresFloorInfoByFloorId(SqlConnection conn, Guid floorId)
        {
            const string sql = @"SELECT  tfa.TiresActivityId ,
                    tfa.FloorActivityId ,
                    tfs.StartDateTime AS StartTime ,
                    tfs.EndDateTime AS EndTime ,
                    tfs.ActivityName ,
                    tfa.FlashSaleId
            FROM    Configuration..TiresFloorActivityConfig AS tfa WITH ( NOLOCK )
                    LEFT  JOIN Activity.dbo.tbl_FlashSale AS tfs WITH ( NOLOCK ) ON tfs.ActivityID = tfa.FlashSaleId
            WHERE   tfa.FloorActivityId = @FloorActivityId;";
            return conn.Query<TiresFloorActivityConfig>(sql, new { FloorActivityId = floorId }, commandType: CommandType.Text).SingleOrDefault();
        }

        public static TiresFloorActivityConfig SelectTiresFloorInfoByFlashId(SqlConnection conn, Guid flashId, Guid parentId)
        {
            const string sql = @" SELECT tfa.TiresActivityId ,
                    tfa.FloorActivityId ,
                    tfs.StartDateTime AS StartTime ,
                    tfs.EndDateTime AS EndTime ,
                    tfs.ActivityName ,
                    tfs.ActivityID
             FROM   Configuration..TiresFloorActivityConfig AS tfa WITH ( NOLOCK )
                    RIGHT  JOIN Activity.dbo.tbl_FlashSale AS tfs WITH ( NOLOCK ) ON tfs.ActivityID = tfa.FlashSaleId
                                                                          AND tfa.TiresActivityId = @TiresActivityId
             WHERE  tfs.ActivityID = @FlashId;";
            return conn.Query<TiresFloorActivityConfig>(sql, new { FlashId = flashId, TiresActivityId = parentId }, commandType: CommandType.Text).SingleOrDefault();
        }

        public static int DeleteTiresFloorActivity(SqlConnection conn, Guid floorId)
        {
            const string sql = @"DELETE FROM Configuration..TiresFloorActivityConfig WHERE FloorActivityId=@ActivityId
            DELETE FROM Configuration..ActivityImageConfig WHERE ActivityId=@ActivityId
            DELETE FROM Configuration..RegionMarketingProductConfig WHERE ActivityId=@ActivityId";
            return conn.Execute(sql, new { ActivityId = floorId }, commandType: CommandType.Text);
        }

        #endregion

        #region  图片池

        public static ActivityImageConfig SelectActivityImgByIdAndType(SqlConnection conn, Guid activityId, ActivityImageType type)
        {
            const string sql = @"SELECT  *
            FROM    Configuration..ActivityImageConfig AS aic WITH ( NOLOCK )
            WHERE   aic.ActivityId = @ActivityId
                    AND aic.Type = @Type;";
            return conn.Query<ActivityImageConfig>(sql, new { ActivityId = activityId, Type = type.ToString() }, commandType: CommandType.Text).SingleOrDefault();
        }
        public static List<ActivityImageConfig> SelectActivityImgById(SqlConnection conn, Guid activityId)
        {
            const string sql = @"SELECT  *
            FROM    Configuration..ActivityImageConfig AS aic WITH ( NOLOCK )
            WHERE   aic.ActivityId = @ActivityId";
            return conn.Query<ActivityImageConfig>(sql, new { ActivityId = activityId }, commandType: CommandType.Text).ToList();
        }

        public static int InsertActivityImg(SqlConnection conn, Guid activityId, string imgUrl, ActivityImageType type, int position)
        {
            const string sql = @"INSERT  INTO Configuration..ActivityImageConfig
                ( ActivityId ,
                  Type ,
                  ImgUrl ,
                  Position ,
                  CreatedTime 
                )
        VALUES  ( @ActivityId ,
                  @Type ,
                  @ImgUrl ,
                  @Position ,
                  GETDATE()
                );";
            return conn.Execute(sql, new { ActivityId = activityId, ImgUrl = imgUrl, Type = type.ToString(), Position = position }, commandType: CommandType.Text);
        }

        public static int UpdateActivityImgByIdAnType(SqlConnection conn, Guid activityId, string imgUr, ActivityImageType type, int position)
        {
            const string sql = @"UPDATE  Configuration..ActivityImageConfig
            SET     ImgUrl = @ImgUrl ,
                    Position = @Position ,
                    UpdatedTime = GETDATE()
            WHERE   ActivityId = @ActivityId
                    AND Type = @Type;";
            return conn.Execute(sql, new { ActivityId = activityId, ImgUrl = imgUr, Type = type.ToString(), Position = position }, commandType: CommandType.Text);
        }
        #endregion

        #region  产品池
        public static List<SimpleTireProductInfo> SelectProductInfoByPID(SqlConnection conn, string pidStr)
        {
            const string sql = @"SELECT  PID ,
        CP_Tire_Rim ,
        CP_Tire_Width ,
        CP_Tire_AspectRatio
        FROM    Tuhu_productcatalog..[vw_Products] WITH ( NOLOCK )
        WHERE   EXISTS ( SELECT 1
                         FROM   Tuhu_productcatalog..SplitString(@PIDStr, ',', 1) AS t
                         WHERE  t.Item = PID );";
            return conn.Query<SimpleTireProductInfo>(sql, new { PIDStr = pidStr }, commandType: CommandType.Text).ToList();
        }

        public static TiresActivityProductConfig SelectRegionProductsByPID(SqlConnection conn, Guid activityId, string pid)
        {
            const string sql = @"SELECT  *
            FROM    Configuration..RegionMarketingProductConfig AS rpc WITH ( NOLOCK )
            WHERE  rpc.ActivityId = @ActivityId AND rpc.ProductId = @ProductId;";
            return conn.Query<TiresActivityProductConfig>(sql, new { ActivityId = activityId, ProductId = pid }, commandType: CommandType.Text).SingleOrDefault();
        }
        public static List<TiresActivityProductConfig> SelectRegionProductsByActivityId(SqlConnection conn, Guid activityId)
        {
            const string sql = @"SELECT  *
            FROM    Configuration..RegionMarketingProductConfig AS rpc WITH ( NOLOCK )
            WHERE  rpc.ActivityId = @ActivityId ;";
            return conn.Query<TiresActivityProductConfig>(sql, new { ActivityId = activityId }, commandType: CommandType.Text).ToList();
        }

        public static int InsertTiresProductInfo(SqlConnection conn, Guid activityId, string productId, string slogan, int specialCondition, bool isCancelProgressBar, int? position)
        {
            const string sql = @"INSERT INTO Configuration..RegionMarketingProductConfig
                ( ActivityId ,
                  ProductId ,
                  AdvertiseTitle ,
                  SpecialCondition ,
                  IsCancelProgressBar ,
                  Position ,
                  CreatedTime 
                )
        VALUES  ( @ActivityId ,
                  @ProductId ,
                  @AdvertiseTitle ,
                  @SpecialCondition ,
                  @IsCancelProgressBar ,
                  @Position ,
                  GETDATE() 
                )";
            return conn.Execute(sql, new
            {
                ActivityId = activityId,
                ProductId = productId,
                AdvertiseTitle = slogan,
                SpecialCondition = specialCondition,
                IsCancelProgressBar = isCancelProgressBar,
                Position = position
            }, commandType: CommandType.Text);
        }

        public static int UpdateTiresProductInfo(SqlConnection conn, Guid activityId, string productId, string slogan, int specialCondition, bool isCancelProgressBar, int? position)
        {
            const string sql = @"UPDATE  Configuration..RegionMarketingProductConfig
            SET     AdvertiseTitle = @AdvertiseTitle ,
                    SpecialCondition = @SpecialCondition ,
                    IsCancelProgressBar = @IsCancelProgressBar ,
                    Position = @Position ,
                    UpdatedTime = GETDATE()
            WHERE   ActivityId = @ActivityId
                    AND ProductId = @ProductId;";
            return conn.Execute(sql, new
            {
                ActivityId = activityId,
                ProductId = productId,
                AdvertiseTitle = slogan,
                SpecialCondition = specialCondition,
                IsCancelProgressBar = isCancelProgressBar,
                Position = position
            }, commandType: CommandType.Text);
        }
        #endregion
    }
}
