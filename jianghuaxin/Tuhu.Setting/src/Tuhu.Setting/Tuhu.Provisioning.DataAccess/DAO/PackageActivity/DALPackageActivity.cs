using Dapper;
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
using Tuhu.Provisioning.DataAccess.Entity.PackageActivity;

namespace Tuhu.Provisioning.DataAccess.DAO.PackageActivity
{
    public static class DALPackageActivity
    {
        #region 保养定价活动配置
        public static List<PackageActivityConfig> SelectPackageActivity(SqlConnection conn, Guid? activityId, string activityName, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            const string sql = @"SELECT  * ,
        COUNT(1) OVER ( ) AS Total
        FROM    BaoYang..PackageActivityConfig WITH ( NOLOCK )
        WHERE   ( @ActivityId = '00000000-0000-0000-0000-000000000000'
                  OR @Activityid IS NULL
                  OR ActivityId = @ActivityId
                )
                AND ( @ActivityName = ''
                      OR @ActivityName IS NULL
                      OR ActivityName LIKE '%' + @ActivityName + '%'
                    )
                AND ( @StartTime = ''
                      OR @StartTime IS NULL
                      OR StartTime >= @StartTime
                    )
                AND ( @EndTime = ''
                      OR @EndTime IS NULL
                      OR EndTime <= @EndTime
                    )
        ORDER BY PKID DESC
                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                ONLY;";
            return conn.Query<PackageActivityConfig>(sql, new { ActivityId = activityId ?? Guid.Empty, ActivityName = activityName, StartTime = startTime, EndTime = endTime, PageIndex = pageIndex, PageSize = pageSize }, commandType: CommandType.Text).ToList();
        }

        public static bool InsertPackageActivityConfig(SqlConnection conn, PackageActivityConfig data, string user)
        {
            const string sql = @"INSERT  INTO BaoYang..PackageActivityConfig
                    ( ActivityId ,
                      ActivityName ,
                      StartTime ,
                      EndTime ,
                      MaxSaleQuantity ,
                      PackageTypes ,
                      IsChargeInstallFee ,
                      IsUsePromotion ,
                      InstallOrPayType ,
                      ItemQuantityPerUser ,
                      PromotionPrice ,
                      TipTextColor ,
                      ButtonBackgroundColor ,
                      ButtonTextColor ,
                      BackgroundImg ,
                      OngoingButtonText ,
                      CreateDateTime ,
                      CreateUser ,
                      LastUpdateDateTime 
                    )
            VALUES  ( @ActivityId ,
                      @ActivityName ,
                      @StartTime ,
                      @EndTime ,
                      @MaxSaleQuantity ,
                      @PackageTypes ,
                      @IsChargeInstallFee ,
                      @IsUsePromotion ,
                      @InstallOrPayType ,
                      @ItemQuantityPerUser ,
                      @PromotionPrice ,
                      @TipTextColor ,
                      @ButtonBackgroundColor ,
                      @ButtonTextColor ,
                      @BackgroundImg ,
                      @OngoingButtonText ,
                      GETDATE() ,
                      @CreateUser ,
                      GETDATE()
                    );";
            return conn.Execute(sql, new
            {
                ActivityId = data.ActivityId,
                ActivityName = data.ActivityName,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                MaxSaleQuantity = data.MaxSaleQuantity,
                PackageTypes = data.PackageTypes,
                IsChargeInstallFee = data.IsChargeInstallFee,
                IsUsePromotion = data.IsUsePromotion,
                InstallOrPayType = data.InstallOrPayType,
                ItemQuantityPerUser = data.ItemQuantityPerUser,
                PromotionPrice = data.PromotionPrice.ToString("f2"),
                TipTextColor = data.TipTextColor,
                ButtonBackgroundColor = data.ButtonBackgroundColor,
                ButtonTextColor = data.ButtonTextColor,
                BackgroundImg = data.BackgroundImg,
                OngoingButtonText = data.OngoingButtonText,
                CreateUser = user,
            }, commandType: CommandType.Text) > 0;
        }

        public static bool UpdatePackageActivityConfig(SqlConnection conn, PackageActivityConfig data, string user)
        {
            const string sql = @"UPDATE  BaoYang..PackageActivityConfig
            SET     ActivityName = @ActivityName ,
                    StartTime = @StartTime ,
                    EndTime = @EndTime ,
                    MaxSaleQuantity = @MaxSaleQuantity ,
                    PackageTypes = @PackageTypes ,
                    IsChargeInstallFee = @IsChargeInstallFee ,
                    IsUsePromotion = @IsUsePromotion ,
                    InstallOrPayType = @InstallOrPayType ,
                    ItemQuantityPerUser = @ItemQuantityPerUser ,
                    PromotionPrice = @PromotionPrice ,
                    TipTextColor = @TipTextColor ,
                    ButtonBackgroundColor = @ButtonBackgroundColor ,
                    ButtonTextColor = @ButtonTextColor ,
                    BackgroundImg = @BackgroundImg ,
                    OngoingButtonText = @OngoingButtonText ,
                    LastUpdateDateTime = GETDATE() ,
                    UpdateUser = @UpdateUser
            WHERE   ActivityId = @ActivityId; ";
            return conn.Execute(sql, new
            {
                ActivityId = data.ActivityId,
                ActivityName = data.ActivityName,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                MaxSaleQuantity = data.MaxSaleQuantity,
                PackageTypes = data.PackageTypes,
                IsChargeInstallFee = data.IsChargeInstallFee,
                IsUsePromotion = data.IsUsePromotion,
                InstallOrPayType = data.InstallOrPayType,
                ItemQuantityPerUser = data.ItemQuantityPerUser,
                PromotionPrice = data.PromotionPrice.ToString("f2"),
                TipTextColor = data.TipTextColor,
                ButtonBackgroundColor = data.ButtonBackgroundColor,
                ButtonTextColor = data.ButtonTextColor,
                BackgroundImg = data.BackgroundImg,
                OngoingButtonText = data.OngoingButtonText,
                UpdateUser = user,
            }, commandType: CommandType.Text) > 0;
        }

        public static bool DeleteBaoYangPricingConfig(SqlConnection conn, Guid activityId)
        {
            const string sql = @"DELETE FROM BaoYang..PackageActivityConfig WHERE   ActivityId = @ActivityId; ";
            return conn.Execute(sql, new { ActivityId = activityId }, commandType: CommandType.Text) > 0;
        }

        #endregion

        #region 保养定价门店配置

        public static List<PackageActivityShopConfig> SelectPackageActivityShopConfig(SqlConnection conn, Guid activityId)
        {
            const string sql = @"SELECT * FROM BaoYang..PackageActivityShopConfig WITH ( NOLOCK ) WHERE   ActivityId = @ActivityId; ";
            return conn.Query<PackageActivityShopConfig>(sql, new { ActivityId = activityId }, commandType: CommandType.Text).ToList();
        }

        public static bool InsertPackageActivityShopConfig(SqlConnection conn, PackageActivityShopConfig data)
        {
            const string sql = @"INSERT INTO BaoYang..PackageActivityShopConfig
                        ( ActivityId ,
                          ShopType ,
                          ShopId ,
                          CreateDateTime ,
                          LastUpdateDateTime
                        )
                VALUES  ( @ActivityId ,
                          @ShopType ,
                          @ShopId ,
                          GETDATE() ,
                          GETDATE()
                        )";
            return conn.Execute(sql, new
            {
                ActivityId = data.ActivityId,
                ShopType = data.ShopType,
                ShopId = data.ShopId
            }, commandType: CommandType.Text) > 0;
        }

        public static bool DeletePackageActivityShopConfig(SqlConnection conn, Guid activityId)
        {
            const string sql = @"DELETE FROM BaoYang..PackageActivityShopConfig WHERE   ActivityId = @ActivityId; ";
            return conn.Execute(sql, new { ActivityId = activityId }, commandType: CommandType.Text) > 0;
        }

        #endregion

        #region 保养定价场次配置
        public static List<PackageActivityRoundConfig> SelectPackageActivitySceneConfig(SqlConnection conn, Guid activityId)
        {
            const string sql = @"SELECT * FROM BaoYang..PackageActivityRoundConfig WITH ( NOLOCK ) WHERE   ActivityId = @ActivityId; ";
            return conn.Query<PackageActivityRoundConfig>(sql, new { ActivityId = activityId }, commandType: CommandType.Text).ToList();
        }

        public static bool InsertPackageActivitySceneConfig(SqlConnection conn, PackageActivityRoundConfig data)
        {
            const string sql = @"INSERT INTO BaoYang..PackageActivityRoundConfig
                    ( ActivityId ,
                      StartTime ,
                      EndTime ,
                      LimitedQuantity ,
                      CreateDateTime ,
                      LastUpdateDateTime
                    )
            VALUES  ( @ActivityId ,
                      @StartTime ,
                      @EndTime ,
                      @LimitedQuantity ,
                      GETDATE() ,
                      GETDATE()  
                    )";
            return conn.Execute(sql, new
            {
                ActivityId = data.ActivityId,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                LimitedQuantity = data.LimitedQuantity
            }, commandType: CommandType.Text) > 0;
        }

        public static bool DeletePackageActivitySceneConfig(SqlConnection conn, Guid activityId)
        {
            const string sql = @"DELETE FROM BaoYang..PackageActivityRoundConfig WHERE   ActivityId = @ActivityId; ";
            return conn.Execute(sql, new { ActivityId = activityId }, commandType: CommandType.Text) > 0;
        }

        #endregion

        #region  保养定价产品配置

        public static List<PackageActivityProductConfig> SelectPackageActivityProductConfig(SqlConnection conn, Guid activityId)
        {
            const string sql = @"SELECT  * FROM BaoYang..PackageActivityProductConfig WITH ( NOLOCK ) WHERE   ActivityId = @ActivityId; ";
            return conn.Query<PackageActivityProductConfig>(sql, new { ActivityId = activityId }, commandType: CommandType.Text).ToList();
        }

        public static bool InsertPackageActivityProductConfig(SqlConnection conn, PackageActivityProductConfig data)
        {
            const string sql = @"INSERT  INTO BaoYang..PackageActivityProductConfig
                    ( ActivityId ,
                      CategoryName ,
                      PID ,
                      Brand ,
                      IsIngore ,
                      CreateDateTime ,
                      LastUpdateDateTime
                    )
            VALUES  ( @ActivityId ,
                      @CategoryName ,
                      @PID ,
                      @Brand ,
                      @IsIngore ,
                      GETDATE() , -- CreateDateTime - datetime
                      GETDATE()  -- LastUpdateDateTime - datetime
                    );";
            return conn.Execute(sql, new
            {
                ActivityId = data.ActivityId,
                CategoryName = data.CategoryName,
                PID = data.PID,
                Brand = data.Brand,
                IsIngore = data.IsIngore
            }, commandType: CommandType.Text) > 0;
        }

        public static bool DeletePackageActivityProductConfig(SqlConnection conn, Guid activityId)
        {
            const string sql = @"DELETE FROM BaoYang..PackageActivityProductConfig WHERE   ActivityId = @ActivityId; ";
            return conn.Execute(sql, new { ActivityId = activityId }, commandType: CommandType.Text) > 0;
        }

        #endregion

        #region  查询订单状态
        public static List<OrderLists> SelectOrderInfo(SqlConnection conn, List<int> orerIds)
        {
            const string sql = @" WITH    PKIDs
                  AS ( SELECT   *
                       FROM     Gungnir..Split(@OrderIdStr, ',')
                     )
            SELECT  ps.PKID AS OrderId,*
            FROM    Gungnir..tbl_Order AS ps WITH ( NOLOCK )
                    JOIN PKIDs ON ps.PKID = PKIDs.col;";
            return conn.Query<OrderLists>(sql, new { OrderIdStr = string.Join(",", orerIds) }, commandType: CommandType.Text).ToList();
        }
        #endregion

        #region  日志
        public static List<PackageActivityLog> SelectOperationLog(string objectId, string type)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"SELECT * FROM Tuhu_log..PackageActivityLog WITH (NOLOCK) WHERE ObjectId=@ObjectId AND Type=@Type ORDER BY CreatedTime DESC");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ObjectId", objectId);
                cmd.Parameters.AddWithValue("@Type", type.ToString());
                return dbHelper.ExecuteDataTable(cmd).ConvertTo<PackageActivityLog>().ToList();
            }
        }

        public static List<FlashSaleRecordsLog> SelectFlashSalRecordsLog(Guid activityId)
        {
            var conn = ConfigurationManager.ConnectionStrings["SystemLog_AlwaysOnRead"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"SELECT * FROM SystemLog..tbl_FlashSaleRecords WITH (NOLOCK) WHERE ActivityID=@ActivityID ");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityID", activityId);
                return dbHelper.ExecuteDataTable(cmd).ConvertTo<FlashSaleRecordsLog>().ToList();
            }
        }
        #endregion

        #region Vehicle

        public static IEnumerable<string> SelectAllVehicleBrandCategory(SqlConnection conn)
        {
            const string sql = @"SELECT DISTINCT
        t.BrandCategory
FROM    Gungnir..tbl_Vehicle_Type AS t WITH ( NOLOCK )
WHERE   t.BrandCategory IS NOT NULL;";
            return conn.Query<string>(sql, commandType: CommandType.Text);
        }

        public static IEnumerable<string> SelectAllVehicleBrand(SqlConnection conn)
        {
            const string sql = @"SELECT DISTINCT
        t.Brand
FROM    Gungnir..tbl_Vehicle_Type AS t WITH ( NOLOCK )
WHERE   t.ProductID IS NOT NULL;";
            return conn.Query<string>(sql, commandType: CommandType.Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="series"></param>
        /// <param name="brands"></param>
        /// <param name="excludeVehicleIds"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <returns></returns>
        public static Tuple<int, IEnumerable<VehicleSimpleInfo>> SelectVehicles(SqlConnection conn,
            IEnumerable<string> series, IEnumerable<string> brands, IEnumerable<string> excludeVehicleIds,
            double minPrice, double maxPrice, int index, int size)
        {
            var parameters = new DynamicParameters();
            string sql = string.Empty;

            #region Generate SQL

            string brandsCondition = string.Empty, seriesCondition = string.Empty,
                excludeVidCondition = string.Empty, priceCondition = string.Empty;
            if (brands != null && brands.Any())
            {
                brandsCondition = $@"AND EXISTS ( SELECT 1 FROM Gungnir..SplitString(@Brands, N',', 1) WHERE  t.Brand = Item )";
                parameters.Add("@Brands", string.Join(",", brands));
            }
            if (series != null && series.Any())
            {
                seriesCondition = $@"AND EXISTS ( SELECT 1 FROM Gungnir..SplitString(@Series, N',', 1) WHERE t.BrandCategory = Item )";
                parameters.Add("@Series", string.Join(",", series));
            }
            if (excludeVehicleIds != null && excludeVehicleIds.Any())
            {
                excludeVidCondition = $@"AND NOT EXISTS ( SELECT 1 FROM   Gungnir..SplitString(@ExcludeVIDs, N',', 1) WHERE  t.ProductID = Item )";
                parameters.Add("@ExcludeVIDs", string.Join(",", excludeVehicleIds));
            }
            if (minPrice >= 0 && maxPrice > minPrice)
            {
                priceCondition = $@"AND t.AvgPrice >= @MinPrice AND t.AvgPrice <= @MaxPrice";
                parameters.Add("@MinPrice", minPrice);
                parameters.Add("@MaxPrice", maxPrice);
            }
            sql = $@"SELECT  @Total = COUNT(1)
FROM    Gungnir..tbl_Vehicle_Type AS t WITH ( NOLOCK )
WHERE   1 = 1
{brandsCondition}
{seriesCondition}
{excludeVidCondition}
{priceCondition};
SELECT  t.ProductID AS VehicleID ,
        t.Brand ,
        t.Vehicle ,
        t.AvgPrice
FROM    Gungnir..tbl_Vehicle_Type AS t WITH ( NOLOCK )
WHERE   1 = 1
{brandsCondition}
{seriesCondition}
{excludeVidCondition}
{priceCondition}
ORDER BY t.Brand
        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";
            parameters.Add("@Skip", (index - 1) * size);
            parameters.Add("@Take", size);
            parameters.Add("@Total", size, DbType.Int32, ParameterDirection.Output);

            #endregion

            var result = conn.Query<VehicleSimpleInfo>(sql, parameters, commandType: CommandType.Text);
            var total = parameters.Get<int>("@Total");
            return Tuple.Create(total, result);
        }

        public static List<PackageActivityVehicleConfig> SelectPackageActivityVehicleConfig(SqlConnection conn, Guid activityId)
        {
            const string sql = @"SELECT  c.VehicleID ,
        t.Brand ,
        t.Vehicle ,
        t.AvgPrice
FROM    Gungnir..tbl_Vehicle_Type AS t WITH ( NOLOCK )
        INNER JOIN BaoYang..PackageActivityVehicleConfig AS c WITH ( NOLOCK ) ON t.ProductID = c.VehicleID COLLATE Chinese_PRC_CI_AS
WHERE   c.ActivityId = @ActivityId;";
            var result = conn.Query<PackageActivityVehicleConfig>(sql, new { ActivityId = activityId }, commandType: CommandType.Text);
            return result?.ToList() ?? new List<PackageActivityVehicleConfig>();
        }

        public static List<PackageActivityPriceConfig> SelectPackageActivityPriceConfig(SqlConnection conn, Guid activityId)
        {
            const string sql = @"SELECT  t.PKID ,
        t.ActivityId ,
        t.TierType ,
        t.Price ,
        t.CreateDateTime ,
        t.LastUpdateDateTime
FROM    BaoYang..PackageActivityPriceConfig AS t WITH ( NOLOCK )
WHERE   t.ActivityId = @ActivityId;";
            var result = conn.Query<PackageActivityPriceConfig>(sql, new { ActivityId = activityId }, commandType: CommandType.Text);
            return result?.ToList() ?? new List<PackageActivityPriceConfig>();
        }

        public static bool DeletePackageActivityVehicleConfig(SqlConnection conn, Guid activityId)
        {
            const string sql = @"DELETE  FROM BaoYang..PackageActivityVehicleConfig WHERE   ActivityId = @ActivityId;";
            var result = conn.Execute(sql, new { ActivityId = activityId }, commandType: CommandType.Text) > 0;
            return result;
        }

        public static bool DeletePackageActivityPriceConfig(SqlConnection conn, Guid activityId)
        {
            const string sql = @"DELETE  FROM BaoYang..PackageActivityPriceConfig WHERE   ActivityId = @ActivityId";
            var result = conn.Execute(sql, new { ActivityId = activityId }, commandType: CommandType.Text) > 0;
            return result;
        }

        public static bool InsertPackageActivityVehicleConfig(SqlConnection conn, Guid activityId, string vehicleId)
        {
            const string sql = @"INSERT  INTO BaoYang..PackageActivityVehicleConfig
        ( ActivityId ,
          VehicleID ,
          CreateDateTime ,
          LastUpdateDateTime
        )
VALUES  ( @ActivityId ,
          @VehicleID ,
          GETDATE() ,
          GETDATE()
        );";
            var result = conn.Execute(sql, new
            {
                ActivityId = activityId,
                VehicleID = vehicleId
            }, commandType: CommandType.Text) > 0;
            return result;
        }

        public static bool InsertPackageActivityPriceConfig(SqlConnection conn, Guid activityId, PackageActivityPriceConfig config)
        {
            const string sql = @"INSERT  INTO BaoYang..PackageActivityPriceConfig
        ( ActivityId ,
          TierType ,
          Price ,
          CreateDateTime ,
          LastUpdateDateTime
        )
VALUES  ( @ActivityId ,
          @TierType ,
          @Price ,
          GETDATE() ,
          GETDATE()
        )";
            var result = conn.Execute(sql, new
            {
                ActivityId = activityId,
                TierType = config.TierType,
                Price = config.Price,
            }, commandType: CommandType.Text) > 0;
            return result;
        }

        #endregion
    }
}
