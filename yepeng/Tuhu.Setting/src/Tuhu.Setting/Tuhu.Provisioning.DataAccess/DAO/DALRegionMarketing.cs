using Dapper;
using Microsoft.ApplicationBlocks.Data;
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
using Tuhu.Provisioning.DataAccess.Entity.RegionMarketing;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DALRegionMarketing
    {
        public static List<RegionMarketingModel> SelectRegionMarketingConfig(SqlConnection conn, Guid? activityId, string activityName, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            const string sql = @"SELECT  *
FROM    ( SELECT    rmc.PKID ,
                    rmc.ActivityId ,
                    tfs.ActivityName ,
                    rmc.WXUrl ,
                    rmc.AppUrl ,
                    rmc.ShareImg ,
                    rmc.ShareTitle ,
                    rmc.ShareDes ,
                    rmc.IsAdaptationVehicle ,
                    CASE WHEN rmc.StartTime IS NOT NULL THEN rmc.StartTime
                         ELSE tfs.StartDateTime
                    END AS StartTime ,
                    CASE WHEN rmc.EndTime IS NOT NULL THEN rmc.EndTime
                         ELSE tfs.EndDateTime
                    END AS EndTime ,
                    rmc.ActivityRules ,
                    rmc.ActivityRulesImg ,
                    rmc.CreatedTime ,
                    COUNT(1) OVER ( ) AS Total
          FROM      Configuration..RegionMarketingConfig AS rmc WITH ( NOLOCK )
                    JOIN Activity.dbo.tbl_FlashSale AS tfs WITH ( NOLOCK ) ON rmc.ActivityId = tfs.ActivityID
        ) AS s
WHERE   ( @ActivityId = '00000000-0000-0000-0000-000000000000'
          OR s.ActivityId = @ActivityId
        )
        AND ( @ActivityName = ''
              OR @ActivityName IS NULL
              OR s.ActivityName LIKE '%' + @ActivityName + '%'
            )
        AND ( @StartTime = ''
              OR @StartTime IS NULL
              OR s.StartTime >= @StartTime
            )
        AND ( @EndTime = ''
              OR @EndTime IS NULL
              OR s.EndTime <= @EndTime
            )
ORDER BY s.PKID DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;";
            return conn.Query<RegionMarketingModel>(sql, new { ActivityId = activityId ?? Guid.Empty, ActivityName = activityName, StartTime = startTime, EndTime = endTime, PageIndex = pageIndex, PageSize = pageSize }, commandType: CommandType.Text).ToList();
        }

        public static RegionMarketingModel SelectRegionConfigByActivityId(SqlConnection conn, Guid activityId)
        {
            const string sql = @"SELECT  rmc.PKID ,
        rmc.ActivityId ,
        rmc.WXUrl ,
        rmc.AppUrl ,
        rmc.ShareImg ,
        rmc.ShareTitle ,
        rmc.ShareDes ,
        rmc.IsAdaptationVehicle ,
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

            return conn.Query<RegionMarketingModel>(sql, new { ActivityId = activityId }, commandType: CommandType.Text).SingleOrDefault();
        }

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


        public static RegionMarketingProductConfig SelectRegionProductsByPID(SqlConnection conn, Guid activityId, string pid)
        {
            const string sql = @"SELECT  *
            FROM    Configuration..RegionMarketingProductConfig AS rpc WITH ( NOLOCK )
            WHERE  rpc.ActivityId = @ActivityId AND rpc.ProductId = @ProductId;";
            return conn.Query<RegionMarketingProductConfig>(sql, new { ActivityId = activityId, ProductId = pid }, commandType: CommandType.Text).SingleOrDefault();
        }
        public static List<RegionMarketingProductConfig> SelectRegionProductsByActivityId(SqlConnection conn, Guid activityId)
        {
            const string sql = @"SELECT  *
            FROM    Configuration..RegionMarketingProductConfig AS rpc WITH ( NOLOCK )
            WHERE  rpc.ActivityId = @ActivityId ;";
            return conn.Query<RegionMarketingProductConfig>(sql, new { ActivityId = activityId }, commandType: CommandType.Text).ToList();
        }

        public static int InsertRegionProductInfo(SqlConnection conn, Guid activityId, string productId, string slogan, int specialCondition)
        {
            const string sql = @"INSERT INTO Configuration..RegionMarketingProductConfig
                ( ActivityId ,
                  ProductId ,
                  AdvertiseTitle ,
                  SpecialCondition ,
                  CreatedTime 
                )
        VALUES  ( @ActivityId ,
                  @ProductId ,
                  @AdvertiseTitle ,
                  @SpecialCondition ,
                  GETDATE() 
                )";
            return conn.Execute(sql, new { ActivityId = activityId, ProductId = productId, AdvertiseTitle = slogan, SpecialCondition = specialCondition }, commandType: CommandType.Text);
        }

        public static int UpdateRegionProductInfo(SqlConnection conn, Guid activityId, string productId, string slogan, int specialCondition)
        {
            const string sql = @"UPDATE  Configuration..RegionMarketingProductConfig
            SET     AdvertiseTitle = @AdvertiseTitle ,
                    SpecialCondition = @SpecialCondition ,
                    UpdatedTime = GETDATE()
            WHERE   ActivityId = @ActivityId
                    AND ProductId = @ProductId;";
            return conn.Execute(sql, new { ActivityId = activityId, ProductId = productId, AdvertiseTitle = slogan, SpecialCondition = specialCondition }, commandType: CommandType.Text);
        }

        public static int UpdateRegionMarketingConfig(SqlConnection conn, RegionMarketingModel data)
        {
            const string sql = @"
            UPDATE  Configuration..RegionMarketingConfig
            SET     WXUrl = @WXUrl ,
                    AppUrl = @AppUrl ,
                    ShareImg = @ShareImg ,
                    ShareTitle = @ShareTitle ,
                    ShareDes = @ShareDes ,
                    IsAdaptationVehicle = @IsAdaptationVehicle ,
                    StartTime = @StartTime ,
                    EndTime = @EndTime ,
                    ActivityRules = @ActivityRules ,
                    ActivityRulesImg = @ActivityRulesImg ,
                    UpdatedTime = GETDATE()
            WHERE   ActivityId = @ActivityId;";
            return conn.Execute(sql, new
            {
                ActivityId = data.ActivityId,
                WXUrl = data.WXUrl,
                AppUrl = data.AppUrl,
                ShareImg = data.ShareImg,
                ShareTitle = data.ShareTitle,
                ShareDes = data.ShareDes,
                IsAdaptationVehicle = data.IsAdaptationVehicle,
                StartTime = data.StartTime,
                ActivityRules = data.ActivityRules,
                ActivityRulesImg = data.ActivityRulesImg,
                EndTime = data.EndTime
            }, commandType: CommandType.Text);
        }

        public static int InsertRegionMarketingConfig(SqlConnection conn, RegionMarketingModel data)
        {
            const string sql = @"INSERT INTO Configuration..RegionMarketingConfig
                    ( ActivityId ,
                      WXUrl ,
                      AppUrl ,
                      ShareImg ,
                      ShareTitle ,
                      ShareDes ,
                      IsAdaptationVehicle ,
                      StartTime ,
                      EndTime ,
                      ActivityRules ,
                      ActivityRulesImg ,
                      CreatedTime 
                    )
            OUTPUT  Inserted.PKID
            VALUES  ( @ActivityId ,
                      @WXUrl ,
                      @AppUrl ,
                      @ShareImg ,
                      @ShareTitle ,
                      @ShareDes ,
                      @IsAdaptationVehicle ,
                      @StartTime ,
                      @EndTime ,
                      @ActivityRules ,
                      @ActivityRulesImg ,
                      GETDATE() 
                    )";
            return conn.Execute(sql, new
            {
                ActivityId = data.ActivityId,
                WXUrl = data.WXUrl,
                AppUrl = data.AppUrl,
                ShareImg = data.ShareImg,
                ShareTitle = data.ShareTitle,
                ShareDes = data.ShareDes,
                IsAdaptationVehicle = data.IsAdaptationVehicle,
                StartTime = data.StartTime,
                ActivityRules = data.ActivityRules,
                ActivityRulesImg = data.ActivityRulesImg,
                EndTime = data.EndTime
            }, commandType: CommandType.Text);
        }

        public static int DeleteRegionMarketingConfig(SqlConnection conn, Guid activityId)
        {
            const string sql = @"DELETE FROM Configuration..RegionMarketingConfig WHERE ActivityId=@ActivityId
            DELETE FROM Configuration..ActivityImageConfig WHERE ActivityId=@ActivityId
            DELETE FROM Configuration..RegionMarketingProductConfig WHERE ActivityId=@ActivityId";

            return conn.Execute(sql, new { ActivityId = activityId }, commandType: CommandType.Text);
        }

        public static List<RegionMarketingLog> SelectOperationLog(Guid activityId, string type)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"SELECT * FROM Tuhu_log..RegionMarketingLog WITH (NOLOCK) WHERE ActivityId=@ActivityId AND Type=@Type ORDER BY CreatedTime DESC");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                cmd.Parameters.AddWithValue("@Type", type);
                return dbHelper.ExecuteDataTable(cmd).ConvertTo<RegionMarketingLog>().ToList();
            }
        }
        //public static void BatchInsertActivityImageConfig(SqlConnection conn, List<ActivityImageConfig> imgList)
        //{
        //    using (var sbc = new SqlBulkCopy(conn))
        //    {
        //        sbc.BatchSize = 50;
        //        sbc.BulkCopyTimeout = 0;
        //        sbc.DestinationTableName = "Configuration..ActivityImageConfig";
        //        DataTable table = new DataTable();
        //        table.Columns.Add("ActivityId");
        //        table.Columns.Add("Type");
        //        table.Columns.Add("ImgUrl");
        //        foreach (DataColumn col in table.Columns)
        //        {
        //            sbc.ColumnMappings.Add(col.ColumnName, col.ColumnName);
        //        }
        //        foreach (var item in imgList)
        //        {
        //            var row = table.NewRow();
        //            row["ActivityId"] = item.ActivityId;
        //            row["Type"] = item.Type.ToString();
        //            row["ImgUrl"] = item.ImgUrl;
        //            table.Rows.Add(row);
        //        }
        //        sbc.WriteToServer(table);
        //    }
        //}

        //public static void BatchInsertRegionMarketingProductConfig(SqlConnection conn, List<RegionMarketingProductConfig> productList)
        //{
        //    using (var sbc = new SqlBulkCopy(conn))
        //    {
        //        sbc.BatchSize = 50;
        //        sbc.BulkCopyTimeout = 0;
        //        sbc.DestinationTableName = "Configuration..RegionMarketingProductConfig";
        //        DataTable table = new DataTable();
        //        table.Columns.Add("ActivityId");
        //        table.Columns.Add("ProductId");
        //        table.Columns.Add("AdvertiseTitle");
        //        table.Columns.Add("SpecialCondition");
        //        foreach (DataColumn col in table.Columns)
        //        {
        //            sbc.ColumnMappings.Add(col.ColumnName, col.ColumnName);
        //        }
        //        foreach (var item in productList)
        //        {
        //            var row = table.NewRow();
        //            row["ActivityId"] = item.ActivityId.ToString();
        //            row["ProductId"] = item.ProductId;
        //            row["AdvertiseTitle"] = item.AdvertiseTitle;
        //            row["SpecialCondition"] = item.SpecialCondition;
        //            table.Rows.Add(row);
        //        }
        //        sbc.WriteToServer(table);
        //    }
        //}
    }
}
