using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalBaoYangActivitySetting
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(strConn);

        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(connectionStringOnRead);
        public static List<IGrouping<int, BaoYangActivitySetting>> GetUpkeepActivitySetting(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT A.[Id] ,
                                    [ActivityNum] ,
                                    [ActivityName] ,
                                    [ActivityStatus] ,
                                    [CheckStatus] ,
                                    [LayerStatus] ,
                                    [LayerImage] ,
                                    [LayerImage2] ,
                                    [CouponId] ,
                                    [ButtonChar] ,
                                    [ShopAuthentication] AS StoreAuthentication ,
                                    [ShopAuthenticationName] AS StoreAuthenticationName ,
                                    A.[CreateTime] ,
                                    A.[UpdateTime] ,
                                    B.Brands AS RelevanceBrands ,
                                    B.Series AS RelevanceSeries ,
                                    B.Products AS RelevanceProducts,
                                    ActivityImage,
                                    A.GetRuleGUID
                            FROM    [BaoYang].[dbo].[BaoYangActivitySetting] AS A WITH (NOLOCK)
                                    LEFT JOIN [BaoYang].[dbo].BaoYangActivitySettingItem AS B WITH (NOLOCK) ON B.BaoYangActivityId = A.Id   
                                    ORDER BY  A.[UpdateTime] DESC 
                                " + sqlStr;

            recordCount = 0;

            List<BaoYangActivitySetting> list = SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql).ConvertTo<BaoYangActivitySetting>().ToList();
            return list.GroupBy(x => x.Id).ToList();
        }

        public static List<ServiceTypeSetting> GetServiceTypeSetting()
        {
            const string sql = @"SELECT [PKID] AS Id
                                      ,[Type] AS ServiceType
                                      ,[CatalogName]
                                      ,[TypeName] AS ServiceTypeName                                
                                  FROM [BaoYang].[dbo].[BaoYangTypeConfig] WITH (NOLOCK)";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql).ConvertTo<ServiceTypeSetting>().ToList();
        }

        public static BaoYangActivitySetting GetUpkeepActivitySettingById(int id)
        {
            const string sql = @"SELECT [Id]
                                      ,[ActivityNum]
                                      ,[ActivityName]
                                      ,[ActivityStatus]
                                      ,[CheckStatus]
                                      ,[LayerStatus]
                                      ,[LayerImage]
                                      ,[LayerImage2]
                                      ,[CouponId]
                                      ,[ButtonChar]
                                      ,[ShopAuthentication] AS StoreAuthentication
                                      ,[ShopAuthenticationName] AS StoreAuthenticationName                                    
                                      ,[CreateTime] 
                                      ,[UpdateTime]
                                      ,ActivityImage 
                                      ,GetRuleGUID    
                              FROM [BaoYang].[dbo].[BaoYangActivitySetting] WITH (NOLOCK)  WHERE Id = @Id ";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@Id", id)).ConvertTo<BaoYangActivitySetting>().ToList().FirstOrDefault();
        }

        public static ServiceTypeSetting GetServiceTypeSettingById(int id)
        {
            const string sql = @"SELECT [Id]
                                      ,[ServiceType]
                                      ,[ServiceTypeName]
                                      ,[CreateTime]
                                      ,[UpdateTime]
                                  FROM [Gungnir].[dbo].[ServiceTypeSetting] WITH (NOLOCK) WHERE Id = @Id ";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@Id", id)).ConvertTo<ServiceTypeSetting>().ToList().FirstOrDefault();
        }

        public static bool InsertUpkeepActivitySetting(BaoYangActivitySetting model, out int id, out string activityNum)
        {

            const string sql = @"INSERT INTO BaoYang..BaoYangActivitySetting
                                        ( 
                                           [ActivityNum]
                                          ,[ActivityName]
                                          ,[ActivityStatus]
                                          ,[CheckStatus]
                                          ,[LayerStatus]
                                          ,[LayerImage]
                                          ,[LayerImage2]
                                          ,[CouponId]
                                          ,[ButtonChar]
                                          ,[ShopAuthentication]
                                          ,[ShopAuthenticationName]
                                          ,[CreateTime]
                                          ,[UpdateTime]
                                          ,ActivityImage
                                          ,GetRuleGUID
                                        )
                                VALUES  ( @ActivityNum
                                          ,@ActivityName
                                          ,@ActivityStatus
                                          ,@CheckStatus
                                          ,@LayerStatus
                                          ,@LayerImage
                                          ,@LayerImage2
                                          ,@CouponId
                                          ,@ButtonChar
                                          ,@ShopAuthentication
                                          ,@ShopAuthenticationName 
                                          ,GETDATE()
                                          ,GETDATE()
                                          ,@ActivityImage
                                          ,@GetRuleGUID
                                        )SELECT @@IDENTITY
                                ";
            activityNum = Guid.NewGuid().ToString();
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityName",model.ActivityName),
                 new SqlParameter("@ActivityNum",activityNum),
                  new SqlParameter("@ActivityStatus",model.ActivityStatus),
                   new SqlParameter("@ButtonChar",model.ButtonChar),
                    new SqlParameter("@CheckStatus",model.CheckStatus),
                     new SqlParameter("@CouponId",model.CouponId),
                      new SqlParameter("@LayerImage",model.LayerImage),
                       new SqlParameter("@LayerImage2",model.LayerImage2),
                        new SqlParameter("@LayerStatus",model.LayerStatus),
                         new SqlParameter("@ShopAuthentication",model.StoreAuthentication),
                           new SqlParameter("@ShopAuthenticationName",model.StoreAuthenticationName),
                             new SqlParameter("@ActivityImage",model.ActivityImage),
                                new SqlParameter("@GetRuleGUID",model.GetRuleGUID)

            };
            id = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParameters));

            if (id > 0)
            {
                if (model.LayerStatus)
                {
                    UpdateLayerStatus(activityNum);
                    return true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 有且只有一个活动的浮层是可用状态
        /// </summary>
        /// <param name="activityNum">activityNum</param>
        /// <returns></returns>
        public static bool UpdateLayerStatus(string activityNum)
        {
            string sql = @"UPDATE [BaoYang].[dbo].[BaoYangActivitySetting] SET LayerStatus=0 WHERE ActivityNum <> @ActivityNum";
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@ActivityNum", activityNum)) > 0;
        }

        public static bool InsertServiceTypeSetting(ServiceTypeSetting model)
        {
            const string sql = @" INSERT INTO Gungnir..ServiceTypeSetting
                                            ( ServiceType ,
                                              ServiceTypeName ,
                                              CreateTime ,
                                              UpdateTime
                                            )
                                    VALUES  ( @ServiceType , -- ServiceType - varchar(100)
                                              @ServiceTypeName , -- ServiceTypeName - nvarchar(200)
                                              GETDATE() , -- CreateTime - datetime
                                              GETDATE()  -- UpdateTime - datetime
                                            )
		                                    ";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ServiceType",model.ServiceType),
                 new SqlParameter("@ServiceTypeName",model.ServiceTypeName),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }

        public static bool UpdateUpkeepActivitySetting(BaoYangActivitySetting model)
        {
            const string sql = @"UPDATE  BaoYang..BaoYangActivitySetting
                                SET     --ActivityNum = @ActivityNum ,
                                        ActivityName = @ActivityName ,
                                        ActivityStatus = @ActivityStatus ,
                                        CheckStatus = @CheckStatus ,
                                        LayerStatus = @LayerStatus ,
                                        LayerImage = @LayerImage ,
                                        LayerImage2 = @LayerImage2 ,
                                        CouponId = @CouponId ,
                                        ButtonChar = @ButtonChar ,                                     
                                        ShopAuthentication = @StoreAuthentication,
                                        ShopAuthenticationName = @StoreAuthenticationName ,
                                        ActivityImage = @ActivityImage,
                                        UpdateTime = GETDATE(),
                                        GetRuleGUID=@GetRuleGUID
                                WHERE   Id = @Id";

            var sqlParameters = new SqlParameter[]
           {
                new SqlParameter("@ActivityName",model.ActivityName),
                 new SqlParameter("@ActivityNum",model.ActivityNum),
                  new SqlParameter("@ActivityStatus",model.ActivityStatus),
                   new SqlParameter("@ButtonChar",model.ButtonChar),
                    new SqlParameter("@CheckStatus",model.CheckStatus),
                     new SqlParameter("@CouponId",model.CouponId),
                      new SqlParameter("@LayerImage",model.LayerImage),
                       new SqlParameter("@LayerImage2",model.LayerImage2),
                        new SqlParameter("@LayerStatus",model.LayerStatus),
                         new SqlParameter("@Id",model.Id),
                           new SqlParameter("@StoreAuthentication",model.StoreAuthentication),
                             new SqlParameter("@StoreAuthenticationName",model.StoreAuthenticationName),
                               new SqlParameter("@ActivityImage",model.ActivityImage),
                                 new SqlParameter("@GetRuleGUID",model.GetRuleGUID??string.Empty)
           };
            if (SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0)
            {
                if (model.LayerStatus)
                {
                    UpdateLayerStatus(model.ActivityNum);
                    return true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool UpdateServiceTypeSetting(ServiceTypeSetting model)
        {
            const string sql = @"UPDATE  Gungnir..ServiceTypeSetting
                                SET     ServiceType = @ServiceType ,
                                        ServiceTypeName = @ServiceTypeName ,
                                        UpdateTime = GETDATE()
                                WHERE   Id = @Id";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ServiceType",model.ServiceType),
                 new SqlParameter("@ServiceTypeName",model.ServiceTypeName),
                  new SqlParameter("@Id",model.Id),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }

        public static bool DeleteUpkeepActivitySetting(int id)
        {
            const string sql = @" DELETE FROM BaoYang..[BaoYangActivitySettingItem] WHERE BaoYangActivityId=@Id  DELETE FROM BaoYang..[BaoYangActivitySetting] WHERE Id=@Id";

            var sqlParameters = new SqlParameter[]
            {
                  new SqlParameter("@Id",id),
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }

        public static bool DeleteServiceTypeSetting(int id)
        {
            const string sql = @"DELETE FROM Gungnir..ServiceTypeSetting WHERE Id=@Id";

            var sqlParameters = new SqlParameter[]
            {
                  new SqlParameter("@Id",id),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }

        public static List<BaoYangActivityRelevance> GetRelevanceBrand(string catalogNames)
        {
            const string sql = @"SELECT  DISTINCT CP_Brand AS Brand
                                FROM    Tuhu_productcatalog..[CarPAR_zh-CN] WITH (NOLOCK)
                                WHERE   PrimaryParentCategory COLLATE Chinese_PRC_CI_AS IN (SELECT * FROM Gungnir..SplitString(@catalogNames,';',1))";//CatalogName: OilFilter
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@catalogNames", catalogNames)).ConvertTo<BaoYangActivityRelevance>().Where(x => !string.IsNullOrWhiteSpace(x.Brand)).ToList();

        }
        public static List<BaoYangActivityRelevance> GetRelevanceSeries(string catalogName)
        {
            const string sql = @"SELECT DISTINCT
		                                A.CP_ShuXing6 AS Series		
	                                FROM      Tuhu_productcatalog..[CarPAR_zh-CN] AS A WITH (NOLOCK)
	                                WHERE   A.PrimaryParentCategory=@catalogName ";
            //-- 'DCL', 'HEYNER', 'YooCar', '阿波罗', '博世/BOSCH','法雷奥/VALEO', '辉门 安扣/FM ANCO', '山多力/SANDOLLY'        
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@catalogName", catalogName)).ConvertTo<BaoYangActivityRelevance>().Where(x => !string.IsNullOrWhiteSpace(x.Series)).ToList();
        }

        public static List<BaoYangActivityRelevance> GetStoreAuthentication()
        {
            const string sql = @"SELECT ShopCertification  FROM   Gungnir.dbo.ShopConfig WHERE ShopCertification &1=1 ";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<BaoYangActivityRelevance>().ToList();
        }

        /// <summary>
        /// 根据保养活动ActivityId获取关联配置
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static List<BaoYangActivitySettingItem> GetBaoYangActivitySettingItemByBaoYangActivityId(int activityId)
        {
            const string sql = @"SELECT [Id]
                                  ,[BaoYangActivityId]
                                  ,[ServicePackagesType]
                                  ,[ServicePackagesItems]
                                  ,[ServicePackagesName]
                                  ,[ServiceType]
                                  ,[ServiceCatalog]
                                  ,[ServiceCatalogName]
                                  ,[Brands]
                                  ,[Series]
                                  ,[Products]
                                  ,[CreateTime]
                                  ,[UpdateTime]
                                  ,[InAdapteTipPrefix]
                                  ,[Grades]
                              FROM [BaoYang].[dbo].[BaoYangActivitySettingItem] WITH (NOLOCK) WHERE BaoYangActivityId=@Id ORDER BY UpdateTime DESC";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@Id", activityId)).ConvertTo<BaoYangActivitySettingItem>().ToList();
        }

        /// <summary>
        /// 插入保养活动关联配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool InsertBaoYangActivitySettingItem(BaoYangActivitySettingItem model)
        {
            const string sql = @"INSERT INTO BaoYang..[BaoYangActivitySettingItem]
                                         (     [BaoYangActivityId]
                                              ,[ServicePackagesType]
                                              ,[ServicePackagesItems]
                                              ,[ServicePackagesName]
                                              ,[ServiceType]
                                              ,[ServiceCatalog]
                                              ,[ServiceCatalogName]
                                              ,[Brands]
                                              ,[Series]
                                              ,[Products]
                                              ,[InAdapteTipPrefix]
                                              ,[CreateTime]
                                              ,[UpdateTime]
                                              ,[Grades]
                                         )VALUES
                                         (
                                               @BaoYangActivityId
                                              ,@ServicePackagesType
                                              ,@ServicePackagesItems
                                              ,@ServicePackagesName
                                              ,@ServiceType
                                              ,@ServiceCatalog
                                              ,@ServiceCatalogName
                                              ,@Brands
                                              ,@Series
                                              ,@Products
                                              ,@InAdapteTipPrefix
                                              ,GETDATE()
                                              ,GETDATE()
                                              ,@Grades
                                         )";

            var sqlParameters = new SqlParameter[] {
                new SqlParameter("@BaoYangActivityId",model.BaoYangActivityId),
                new SqlParameter("@Brands",model.Brands??string.Empty),
                new SqlParameter("@Products",model.Products??string.Empty),
                new SqlParameter("@Series",model.Series??string.Empty),
                new SqlParameter("@ServiceCatalog",model.ServiceCatalog??string.Empty),
                new SqlParameter("@ServiceCatalogName",model.ServiceCatalogName??string.Empty),
                new SqlParameter("@ServicePackagesItems",model.ServicePackagesItems??string.Empty),
                new SqlParameter("@ServicePackagesName",model.ServicePackagesName??string.Empty),
                new SqlParameter("@ServicePackagesType",model.ServicePackagesType??string.Empty),
                new SqlParameter("@ServiceType",model.ServiceType??string.Empty),
                new SqlParameter("@InAdapteTipPrefix",model.InAdapteTipPrefix??string.Empty),
                new SqlParameter("@Grades",model.Grades??string.Empty)
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }

        /// <summary>
        /// 更新保养活动关联配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateBaoYangActivitySettingItem(BaoYangActivitySettingItem model)
        {
            const string sql = @" UPDATE  [BaoYang].[dbo].[BaoYangActivitySettingItem]
                                SET   
                                        [Brands] = @Brands ,
                                        [Series] = @Series ,
                                        [Products] = @Products ,
                                        [Grades] = @Grades,
                                        [InAdapteTipPrefix] = @InAdapteTipPrefix ,
                                        [UpdateTime] = GETDATE()
                                WHERE   Id = @Id";
            var sqlParameters = new SqlParameter[] {

                new SqlParameter("@Brands",model.Brands??string.Empty),
                new SqlParameter("@Id",model.Id),
                new SqlParameter("@Products",model.Products??string.Empty),
                 new SqlParameter("@Grades",model.Grades??string.Empty),
                new SqlParameter("@Series",model.Series??string.Empty),
                new SqlParameter("@InAdapteTipPrefix",model.InAdapteTipPrefix??string.Empty)
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }

        /// <summary>
        /// 获取该保养活动的关联配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static BaoYangActivitySettingItem GetBaoYangActivitySettingItemById(int id)
        {
            const string sql = @"SELECT [Id]
                                  ,[BaoYangActivityId]
                                  ,[ServicePackagesType]
                                  ,[ServicePackagesItems]
                                  ,[ServicePackagesName]
                                  ,[ServiceType]
                                  ,[ServiceCatalog]
                                  ,[ServiceCatalogName]
                                  ,[Brands]
                                  ,[Series]
                                  ,[Products]
                                  ,[InAdapteTipPrefix]
                                  ,[CreateTime]
                                  ,[UpdateTime]
                                  ,[Grades]
                              FROM [BaoYang].[dbo].[BaoYangActivitySettingItem] WITH (NOLOCK) WHERE Id=@Id";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@Id", id)).ConvertTo<BaoYangActivitySettingItem>().ToList().FirstOrDefault();
        }

        /// <summary>
        /// 删除保养活动关联配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteBaoYangActivitySettingItem(int id)
        {
            const string sql = @"DELETE FROM [BaoYang].[dbo].[BaoYangActivitySettingItem] WHERE Id =@Id";
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

        #region 保养活动分车型配置

        /// <summary>
        /// 获取所有车型品牌
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<string> GetAllVehicleBrand(SqlConnection conn)
        {
            var sql = @"SELECT DISTINCT
                                vw.Brand
                        FROM    Gungnir..vw_Vehicle_Type AS vw WITH ( NOLOCK )
                        WHERE   vw.Brand IS NOT NULL
                        ORDER BY vw.Brand;";
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            return (from DataRow row in dt.Rows select row[0].ToString()).ToList();
        }

        /// <summary>
        /// 获取机油等级
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<OilLevelModel> GetAllOilLevel(SqlConnection conn)
        {
            var sql = @"SELECT DISTINCT
                                CP_ShuXing6 AS OilLevel ,
                                CP_ShuXing1 AS OilType
                        FROM    Tuhu_productcatalog..[CarPAR_zh-CN] WITH ( NOLOCK )
                        WHERE   PrimaryParentCategory = 'Oil'
                                AND CP_ShuXing1 IS NOT NULL
                                AND CP_ShuXing6 IS NOT NULL;";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<OilLevelModel>().ToList();
        }

        /// <summary>
        /// 获取机油粘度
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<string> GetAllOilViscosity(SqlConnection conn)
        {
            var sql = @"SELECT  DISTINCT Viscosity
                        FROM    BaoYang.[dbo].[tbl_PartAccessory] TT WITH ( NOLOCK )
                        WHERE TT.Viscosity IS NOT NULL AND TT.Viscosity <> N''
                        ORDER BY TT.Viscosity";
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            return (from DataRow row in dt.Rows select row[0].ToString()).ToList();
        }

        /// <summary>
        /// 查询保养活动车型配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Tuple<List<BaoYangActivityVehicleViewModel>, int> SelectBaoYangActivityVehicle(SqlConnection conn,
            BaoYangActivityVehicleSearchModel model, int pageIndex, int pageSize)
        {
            var result = null as List<BaoYangActivityVehicleViewModel>;
            int totalCount = 0;
            #region sql
            var sql = @"SELECT @Total = COUNT(DISTINCT gt.VehicleID)
                        FROM    Gungnir..tbl_Vehicle_Type_Timing AS gt WITH ( NOLOCK )
                                INNER JOIN BaoYang..tbl_PartAccessory AS bt ON gt.TID = bt.TID
                        WHERE   gt.TID IN (
                                SELECT  pa.TID
                                FROM    BaoYang..tbl_PartAccessory AS pa WITH ( NOLOCK )
                                WHERE   pa.TID IN (
                                        SELECT  tt.TID
                                        FROM    Gungnir..tbl_Vehicle_Type_Timing AS tt WITH ( NOLOCK )
                                        WHERE   tt.VehicleID IN (
                                                SELECT  vt.ProductID
                                                FROM    Gungnir..tbl_Vehicle_Type AS vt WITH ( NOLOCK )
                                                WHERE   ( @BrandCategory = N'All'
                                                          OR vt.BrandCategory IN (
                                                          SELECT    *
                                                          FROM      Gungnir..SplitString(@BrandCategory,
                                                                                      ',', 1) )
                                                        )
                                                        AND ( vt.AvgPrice BETWEEN @MinPrice AND @MaxPrice )
                                                        AND ( @Brands = N'All'
                                                              OR vt.Brand IN (
                                                              SELECT    *
                                                              FROM      Gungnir..SplitString(@Brands,
                                                                                      ',', 1) )
                                                            ) ) )
                                        AND AccessoryName = N'发动机油'
                                        AND ( @OilLevel = N''
                                              OR pa.Level COLLATE Chinese_PRC_CI_AS IN (
                                              SELECT    *
                                              FROM      Gungnir..SplitString(@OilLevel, ',', 1) )
                                            )
                                        AND ( @OilViscosity = N'All'
                                              OR pa.Viscosity COLLATE Chinese_PRC_CI_AS = @OilViscosity
                                            ) );

                        SELECT DISTINCT
                                gt.VehicleID AS VehicleId ,
                                ( SELECT TOP ( 1 )
                                            Brand
                                  FROM      Gungnir..tbl_Vehicle_Type WITH ( NOLOCK )
                                  WHERE     ProductID = gt.VehicleID
                                ) AS Brand ,
                                ( SELECT TOP ( 1 )
                                            Vehicle
                                  FROM      Gungnir..tbl_Vehicle_Type WITH ( NOLOCK )
                                  WHERE     ProductID = gt.VehicleID
                                ) AS VehicleSeries ,
                                ( SELECT    gp.AvgPrice
                                  FROM      Gungnir..tbl_Vehicle_Type AS gp ( NOLOCK )
                                  WHERE     gp.ProductID = gt.VehicleID
                                ) AS AvgPrice ,
                                STUFF(( SELECT DISTINCT
                                                ',' + Viscosity
                                        FROM    BaoYang..tbl_PartAccessory WITH ( NOLOCK )
                                        WHERE   TID IN (
                                                SELECT DISTINCT
                                                        TID
                                                FROM    Gungnir..tbl_Vehicle_Type_Timing WITH ( NOLOCK )
                                                WHERE   VehicleID = gt.VehicleID )
                                                AND Viscosity IS NOT NULL
                                                AND Viscosity <> N''
                                      FOR
                                        XML PATH('')
                                      ), 1, 1, '') AS OilViscosity ,
                                STUFF(( SELECT  DISTINCT
                                                ',' + Level
                                        FROM    BaoYang..tbl_PartAccessory WITH ( NOLOCK )
                                        WHERE   TID IN (
                                                SELECT DISTINCT
                                                        TID
                                                FROM    Gungnir..tbl_Vehicle_Type_Timing WITH ( NOLOCK )
                                                WHERE   VehicleID = gt.VehicleID )
                                                AND Level IS NOT NULL
                                                AND Level <> N''
                                                AND AccessoryName = N'发动机油'
                                      FOR
                                        XML PATH('')
                                      ), 1, 1, '') AS OilLevel ,
                                t3.ActivityId ,
                                t3.ActivityName
                        FROM    Gungnir..tbl_Vehicle_Type_Timing AS gt WITH ( NOLOCK )
                                INNER JOIN BaoYang..tbl_PartAccessory AS bt ON gt.TID = bt.TID
                                LEFT JOIN ( SELECT  t1.VehicleId ,
                                                    t1.ActivityId ,
                                                    t2.ActivityName
                                            FROM    BaoYang..BaoYangActivityVehicleAndRegion AS t1
                                                    WITH ( NOLOCK )
                                                    INNER JOIN BaoYang..BaoYangActivitySetting AS t2 ON t1.ActivityId = t2.ActivityNum
                                                    WHERE   t1.Type = N'Vehicle'
                                          ) AS t3 ON gt.VehicleID = t3.VehicleID COLLATE Chinese_PRC_CI_AS
                        WHERE   gt.TID IN (
                                SELECT  pa.TID
                                FROM    BaoYang..tbl_PartAccessory AS pa WITH ( NOLOCK )
                                WHERE   pa.TID IN (
                                        SELECT  tt.TID
                                        FROM    Gungnir..tbl_Vehicle_Type_Timing AS tt WITH ( NOLOCK )
                                        WHERE   tt.VehicleID IN (
                                                SELECT  vt.ProductID
                                                FROM    Gungnir..tbl_Vehicle_Type AS vt WITH ( NOLOCK )
                                                WHERE   ( @BrandCategory = N'All'
                                                          OR vt.BrandCategory IN (
                                                          SELECT    *
                                                          FROM      Gungnir..SplitString(@BrandCategory,
                                                                                      ',', 1) )
                                                        )
                                                        AND ( vt.AvgPrice BETWEEN @MinPrice AND @MaxPrice )
                                                        AND ( @Brands = N'All'
                                                              OR vt.Brand IN (
                                                              SELECT    *
                                                              FROM      Gungnir..SplitString(@Brands,
                                                                                      ',', 1) )
                                                            ) ) )
                                        AND AccessoryName = N'发动机油'
                                        AND ( @OilLevel = N''
                                              OR pa.Level COLLATE Chinese_PRC_CI_AS IN (
                                              SELECT    *
                                              FROM      Gungnir..SplitString(@OilLevel, ',', 1) )
                                            )
                                        AND ( @OilViscosity = N'All'
                                              OR pa.Viscosity COLLATE Chinese_PRC_CI_AS = @OilViscosity
                                            ) )
                                ORDER BY Brand ,
                                        VehicleSeries
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                ONLY;";
            #endregion
            var parameters = new SqlParameter[] {
                    new SqlParameter("@PageIndex", pageIndex),
                    new SqlParameter("@PageSize", pageSize),
                    new SqlParameter("@BrandCategory", string.Join(",",model.BrandCategories)),
                    new SqlParameter("@Brands", string.Join(",",model.Brands)),
                    new SqlParameter("@OilLevel", string.Join(",",(model.OilLevels??new List<string>()))),
                    new SqlParameter("@OilViscosity", model.OilViscosity),
                    new SqlParameter("@MinPrice", model.MinPrice),
                    new SqlParameter("@MaxPrice", model.MaxPrice),
                    new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            result = dt.ConvertTo<BaoYangActivityVehicleViewModel>().ToList();
            totalCount = Convert.ToInt32(parameters.Last().Value);
            return Tuple.Create(result, totalCount);
        }

        /// <summary>
        /// 添加保养活动车型配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddBaoYangActivityVehicle(SqlConnection conn, BaoYangActivityVehicleAndRegionModel model)
        {
            var sql = @"INSERT  INTO BaoYang..BaoYangActivityVehicleAndRegion
                                ( VehicleId ,
                                  RegionId ,
                                  ActivityId ,
                                  Type ,
                                  CreateDateTime ,
                                  LastUpdateDateTime
                                )
                        VALUES  ( @VehicleId ,
                                  NULL ,
                                  @ActivityId ,
                                  N'Vehicle' ,
                                  GETDATE() ,
                                  GETDATE()
                                )
                                SELECT  SCOPE_IDENTITY();";
            var parameters = new SqlParameter[] {
                new SqlParameter("@VehicleId", model.VehicleId),
                new SqlParameter("@ActivityId", model.ActivityId)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 更新保养活动车型配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateBaoYangActivityVehicle(SqlConnection conn, BaoYangActivityVehicleAndRegionModel model)
        {
            var sql = @"UPDATE  BaoYang..BaoYangActivityVehicleAndRegion
                        SET     ActivityId = @ActivityId ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   VehicleId = @VehicleId
                                AND Type = N'Vehicle';";
            var parameters = new SqlParameter[] {
                new SqlParameter("@VehicleId", model.VehicleId),
                new SqlParameter("@ActivityId", model.ActivityId)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 删除保养活动车型配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public static bool DeleteBaoYangActivityVehicle(SqlConnection conn, string vehicleId)
        {
            var sql = @"DELETE   FROM BaoYang..BaoYangActivityVehicleAndRegion
                        WHERE    VehicleId = @VehicleId
                                 AND Type = N'Vehicle';";
            var parameters = new SqlParameter[] {
                new SqlParameter("@VehicleId", vehicleId)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 根据VehicleId查询保养活动车型配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public static BaoYangActivityVehicleAndRegionModel GetBaoYangActivityVehicleByVehicleId(SqlConnection conn, string vehicleId)
        {
            var sql = @"SELECT  s.PKID ,
                                s.VehicleId ,
                                s.ActivityId ,
		                        s.Type,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime
                        FROM    BaoYang..BaoYangActivityVehicleAndRegion AS s WITH ( NOLOCK )
                        WHERE   s.VehicleId = @VehicleId
                                AND s.Type = N'Vehicle';";
            var parameters = new SqlParameter[] {
                new SqlParameter("@VehicleId", vehicleId)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<BaoYangActivityVehicleAndRegionModel>().FirstOrDefault();
        }
        #endregion

        #region 保养活动特殊地区配置

        /// <summary>
        /// 查询保养活动地区配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="regionIds"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<BaoYangActivityRegionViewModel> SelectBaoYangActivityRegion(SqlConnection conn,
            List<int> regionIds, int pageIndex, int pageSize)
        {
            var result = null as List<BaoYangActivityRegionViewModel>;
            #region SQL
            var sql = @"SELECT  CONVERT(INT ,r.Item) AS RegionId ,
                                t.ActivityId ,
                                t.ActivityName ,
                                t.CreateDateTime ,
                                t.LastUpdateDateTime
                        FROM    Gungnir..SplitString(@RegionId, N',', 1) AS r
                                LEFT JOIN ( SELECT  t2.PKID ,
                                                    t2.RegionId ,
                                                    t2.ActivityId ,
                                                    t1.ActivityName ,
                                                    t2.CreateDateTime ,
                                                    t2.LastUpdateDateTime
                                            FROM    BaoYang..BaoYangActivitySetting AS t1 WITH ( NOLOCK )
                                                    INNER JOIN BaoYang..BaoYangActivityVehicleAndRegion
                                                    AS t2 ON t2.ActivityId = t1.ActivityNum
                                            WHERE   t2.Type = N'Region'
                                          ) AS t ON r.Item = t.RegionId
                        ORDER BY RegionId 
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                ONLY;";
            #endregion
            var parameters = new SqlParameter[] {
                    new SqlParameter("@PageIndex", pageIndex),
                    new SqlParameter("@PageSize", pageSize),
                    new SqlParameter("@RegionId", string.Join(",",regionIds))
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<BaoYangActivityRegionViewModel>().ToList();
        }

        /// <summary>
        /// 添加保养活动地区配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddBaoYangActivityRegion(SqlConnection conn, BaoYangActivityVehicleAndRegionModel model)
        {
            var sql = @"INSERT  INTO BaoYang..BaoYangActivityVehicleAndRegion
                                ( VehicleId ,
                                  RegionId ,
                                  ActivityId ,
                                  Type ,
                                  CreateDateTime ,
                                  LastUpdateDateTime
                                )
                        VALUES  ( NULL ,
                                  @RegionId ,
                                  @ActivityId ,
                                  N'Region' ,
                                  GETDATE() ,
                                  GETDATE()
                                )
                                SELECT  SCOPE_IDENTITY();";
            var parameters = new SqlParameter[] {
                new SqlParameter("@RegionId", model.RegionId),
                new SqlParameter("@ActivityId", model.ActivityId)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 更新保养活动地区配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateBaoYangActivityRegion(SqlConnection conn, BaoYangActivityVehicleAndRegionModel model)
        {
            var sql = @"UPDATE  BaoYang..BaoYangActivityVehicleAndRegion
                        SET     ActivityId = @ActivityId ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   RegionId = @RegionId
                                AND Type = N'Region';";
            var parameters = new SqlParameter[] {
                new SqlParameter("@RegionId", model.RegionId),
                new SqlParameter("@ActivityId", model.ActivityId)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 删除保养活动地区配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public static bool DeleteBaoYangActivityRegion(SqlConnection conn, int regionId)
        {
            var sql = @"DELETE   FROM BaoYang..BaoYangActivityVehicleAndRegion
                        WHERE    RegionId = @RegionId
                                 AND Type = N'Region';";
            var parameters = new SqlParameter[] {
                new SqlParameter("@RegionId", regionId)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 根据地区Id查询保养活动地区配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public static BaoYangActivityVehicleAndRegionModel GetBaoYangActivityRegionByRegionId(SqlConnection conn, int regionId)
        {
            var sql = @"SELECT  s.PKID ,
                                s.RegionId ,
                                s.ActivityId ,
		                        s.Type,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime
                        FROM    BaoYang..BaoYangActivityVehicleAndRegion AS s WITH ( NOLOCK )
                        WHERE   s.RegionId = @RegionId
                                AND s.Type = N'Region';";
            var parameters = new SqlParameter[] {
                new SqlParameter("@RegionId", regionId)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<BaoYangActivityVehicleAndRegionModel>().FirstOrDefault();
        }

        #endregion

        /// <summary>
        /// 根据保养活动Id获取活动名称
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static string GetBaoYangActivityNameByActivityId(SqlConnection conn, string activityId)
        {
            var sql = @"SELECT    bas.ActivityName
                        FROM      BaoYang..BaoYangActivitySetting AS bas WITH ( NOLOCK )
                        WHERE     bas.ActivityNum = @ActivityId;";
            var parameters = new SqlParameter[] {
                    new SqlParameter("@ActivityId", activityId),
            };
            return SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters)?.ToString();
        }

        /// <summary>
        /// 查看保养活动分车型分地区配置日志
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="identityId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<BaoYangOprLog> GetBaoYangOprLogByIdentityIdAndType(SqlConnection conn, string identityId, string type)
        {
            var sql = @"SELECT  log.PKID ,
                                log.LogType ,
                                log.OldValue ,
                                log.NewValue ,
                                log.OperateUser ,
                                log.Remarks ,
                                log.IdentityID ,
                                log.CreateTime
                        FROM    Tuhu_log..BaoYangOprLog AS log WITH ( NOLOCK )
                        WHERE   log.LogType = @LogType
                                AND log.IdentityID = @IdentityID
                        ORDER BY log.PKID DESC;";
            var parameters = new SqlParameter[] {
                new SqlParameter("@LogType", type),
                new SqlParameter("@IdentityID", identityId)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<BaoYangOprLog>().ToList();
        }
    }
}
