using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.WeChat;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALWechatHome
    {
        public bool AddWechatHomeList(SqlConnection conn, WechatHomeList model)
        {
            string sql = @"INSERT INTO Configuration.dbo.WechatHomeList
                        ( Title ,
                          IsEnabled ,
                          CDateTime ,
                          UDateTime,OrderBy,IsNewUser,TypeName,IsShownButtom,HomePageConfigID,Headings,Subtitle,ImageUrl,Uri
                        )
                VALUES  ( @Title , -- Title - varchar(30)
                          @IsEnabled , -- IsEnabled - int
                          GETDATE() , -- CDateTime - datetime
                          GETDATE(),  -- UDateTime - datetime
                          @OrderBy,@IsNewUser,@TypeName,@IsShownButtom,@HomePageConfigID,@Headings,@Subtitle,@ImageUrl,@Uri
                        )";

            return conn.Execute(sql, model) > 0;
        }

        public bool AddHomePageConfig(SqlConnection conn, HomePageConfiguation model)
        {
            string sql = @"INSERT INTO Configuration.dbo.LightAppHomePageConfig
                        ( Name ,
                          KeyValue,
                          CreateDateTime ,
                          LastUpdateDateTime
                        )
               VALUES  ( @Name , -- Name - nvarchar(64)
                         @KeyValue,
                          GETDATE() , -- CreateDateTime - datetime
                          GETDATE()  -- LastUpdateDateTime - datetime
                        )";

            return conn.Execute(sql, model) > 0;
        }

        public int AddWechatHomeListToInt(SqlConnection conn, WechatHomeList model)
        {
            string sql = @"INSERT INTO Configuration.dbo.WechatHomeList
                        ( Title ,
                          IsEnabled ,
                          CDateTime ,
                          UDateTime,OrderBy,IsNewUser,TypeName,IsShownButtom,HomePageConfigID,Headings,Subtitle,ImageUrl,Uri
                        )
                VALUES  ( @Title , -- Title - varchar(30)
                          @IsEnabled , -- IsEnabled - int
                          GETDATE() , -- CDateTime - datetime
                          GETDATE(),  -- UDateTime - datetime
                          @OrderBy,@IsNewUser,@TypeName,@IsShownButtom,@HomePageConfigID,@Headings,@Subtitle,@ImageUrl,@Uri
                        );  SELECT @@IDENTITY AS ID";

            return Convert.ToInt32(conn.ExecuteScalar(sql, model));
        }

        public bool UpdateWechatHomeList(SqlConnection conn, WechatHomeList model)
        {
            string sql = @"     UPDATE  Configuration.dbo.WechatHomeList
                                SET     Title = @Title ,
                                        IsEnabled = @IsEnabled ,
                                        UDateTime =  GETDATE(),
                                        OrderBy=@OrderBy,IsNewUser=@IsNewUser,TypeName=@TypeName,IsShownButtom=@IsShownButtom,HomePageConfigID=@HomePageConfigID,Headings=@Headings,Subtitle=@Subtitle,ImageUrl=@ImageUrl,Uri=@Uri
                                WHERE   ID = @ID;";

            return conn.Execute(sql, model) > 0;
        }

        public bool UpdateHomePageConfig(SqlConnection conn, HomePageConfiguation model)
        {
            string sql = @"     UPDATE  Configuration.dbo.LightAppHomePageConfig
                                SET     Name = @Name ,
                                        LastUpdateDateTime =  GETDATE()
                                WHERE   ID = @ID;";

            return conn.Execute(sql, model) > 0;
        }

        public bool DeleteWechatHomeList(SqlConnection conn, int id)
        {
            string sql = "DELETE FROM Configuration.dbo.WechatHomeList WHERE ID=@ID";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ID", id);
            return conn.Execute(sql, parameters) > 0;
        }

        public bool DeleteHomePageConfig(SqlConnection conn, int id)
        {
            string sql = "DELETE FROM Configuration.dbo.LightAppHomePageConfig WHERE ID=@ID";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ID", id);
            return conn.Execute(sql, parameters) > 0;
        }

        public IEnumerable<WechatHomeList> GetWechatHomeList(SqlConnection conn, int homepageconfigId)
        {
            /*
            string sql = "SELECT * FROM Configuration.dbo.WechatHomeList WITH (NOLOCK) where HomePageConfigID=@homepageconfigId";

                DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@homepageconfigId", homepageconfigId,DbType.Int32);
            return conn.Query<WechatHomeList>(sql);
            */
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Configuration_AlwaysOnRead")))
            {
                //  pager.TotalItem = GetListCount(dbHelper, model, pager);
                return dbHelper.ExecuteDataTable(@"SELECT * FROM Configuration.dbo.WechatHomeList WITH (NOLOCK) where HomePageConfigID=@homepageconfigId
                                                       	;", CommandType.Text,
                                                                                           new SqlParameter[] {
                                                                                               new SqlParameter("@homepageconfigId",homepageconfigId)
                                                                                           }).ConvertTo<WechatHomeList>();
            }
        }

        public IEnumerable<HomePageModuleType> GetHomePageModuleTypeList(SqlConnection conn)
        {
            string sql = "SELECT * FROM Configuration.dbo.HomePageModuleType WITH (NOLOCK) ";
            return conn.Query<HomePageModuleType>(sql);
        }

        public IEnumerable<HomePageConfiguation> GetHomePageConfigList(SqlConnection conn)
        {
            string sql = "SELECT * FROM Configuration.dbo.LightAppHomePageConfig WITH (NOLOCK) ";
            return conn.Query<HomePageConfiguation>(sql);
        }

        public WechatHomeList GetWechatHomeEntity(SqlConnection conn, int id)
        {
            string sql = "SELECT * FROM Configuration.dbo.WechatHomeList WITH (NOLOCK) where ID=@ID ";
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@ID", id);
            return conn.Query<WechatHomeList>(sql, parameter).FirstOrDefault();
        }

        public HomePageConfiguation GetHomePageConfigEntity(SqlConnection conn, int id)
        {
            string sql = "SELECT * FROM Configuration.dbo.LightAppHomePageConfig WITH (NOLOCK) where ID=@ID ";
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@ID", id);
            return conn.Query<HomePageConfiguation>(sql, parameter).FirstOrDefault();
        }

        public bool AddContent(SqlConnection conn, WechatHomeContent model)
        {
            string sql = @"INSERT INTO Configuration.dbo.WechatHomeContent
                            ( FKID, Title ,
                              ImageUrl ,
                              Uri ,
                              IsEnabled ,
                              CDateTime ,
                              UDateTime,
                               OrderBy,Headings,Subtitle,UriType,BuriedPointParam,AppID,UriTypeText
                            )
                    VALUES  ( @FKID, @Title , -- Title - varchar(50)
                              @ImageUrl , -- ImageUrl - varchar(500)
                              @Uri , -- Uri - varchar(200)
                              @IsEnabled , -- IsEnabled - int
                              GETDATE() , -- CDateTime - datetime
                              GETDATE(),  -- UDateTime - datetime
                              @OrderBy,@Headings,@Subtitle,@UriType,@BuriedPointParam,@AppID,@UriTypeText
                            )";

            return conn.Execute(sql, model) > 0;
        }

        public bool AddAreaContent(SqlConnection conn, WechatHomeAreaContent model)
        {
            string sql = @"INSERT INTO Configuration.dbo.WechatHomeAreaContent
                            ( FKID,
                              ImageUrl ,
                              Uri ,
                              IsEnabled ,
                              CityIDs,
                              Headings,Subtitle,AppID,UriTypeText
                            )
                    VALUES  ( @FKID,
                              @ImageUrl , -- ImageUrl - varchar(500)
                              @Uri , -- Uri - varchar(200)
                              @IsEnabled , -- IsEnabled - int
                              @CityIDs,
                              @Headings,@Subtitle,@AppID,@UriTypeText
                            )";

            return conn.Execute(sql, model) > 0;
        }

        public bool AddProductContent(SqlConnection conn, WechatHomeProductContent model)
        {
            string sql = @"INSERT INTO Configuration.dbo.WechatHomeProductContent
                            ( FKID,
                              PID ,
                              GroupId,
                              ProductName,
                              ImageUrl ,
                              OrderBy,
                              IsEnabled,
                              BuriedPointParam
                            )
                    VALUES  ( @FKID,
                              @PID,
                              @GroupId,
                              @ProductName,
                              @ImageUrl , -- ImageUrl - varchar(500)
                              @OrderBy,
                              @IsEnabled,  -- IsEnabled - int
                              @BuriedPointParam
                            )";

            return conn.Execute(sql, model) > 0;
        }

        public bool UpdateContent(SqlConnection conn, WechatHomeContent model)
        {
            string sql = @"    UPDATE  Configuration.dbo.WechatHomeContent
                                SET     Title = @Title ,
                                        ImageUrl = @ImageUrl ,
                                        Uri = @Uri ,
                                        IsEnabled = @IsEnabled ,
                                        UDateTime = GETDATE(),OrderBy=@OrderBy,Headings=@Headings,Subtitle=@Subtitle,UriType=@UriType,BuriedPointParam=@BuriedPointParam,AppID=@AppID,UriTypeText=@UriTypeText
                                WHERE   ID = @ID;";
            return conn.Execute(sql, model) > 0;
        }

        public bool UpdateAreaContent(SqlConnection conn, WechatHomeAreaContent model)
        {
            string sql = @"    UPDATE  Configuration.dbo.WechatHomeAreaContent
                                SET
                                        ImageUrl = @ImageUrl ,
                                        Uri = @Uri ,
                                        CityIDs=@CityIDs,
                                        IsEnabled = @IsEnabled ,
                                        UDateTime = GETDATE(),Headings=@Headings,Subtitle=@Subtitle,AppID=@AppID,UriTypeText=@UriTypeText
                                WHERE   ID = @ID;";
            return conn.Execute(sql, model) > 0;
        }

        public bool UpdateProductContent(SqlConnection conn, WechatHomeProductContent model)
        {
            string sql = @"    UPDATE  Configuration.dbo.WechatHomeProductContent
                                SET     PID=@PID,
                                        ImageUrl = @ImageUrl ,
                                        GroupId=@GroupId,
                                        ProductName=@ProductName,
                                        IsEnabled = @IsEnabled ,
                                        UDateTime = GETDATE(),OrderBy=@OrderBy,BuriedPointParam=@BuriedPointParam
                                WHERE   ID = @ID;";
            return conn.Execute(sql, model) > 0;
        }

        public bool DeleteContent(SqlConnection conn, int id)
        {
            string sql = "DELETE FROM Configuration.dbo.WechatHomeContent WHERE ID=@ID ";
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("ID", id);
            return conn.Execute(sql, parameter) > 0;
        }

        public bool DeleteAreaContent(SqlConnection conn, int id)
        {
            string sql = "DELETE FROM Configuration.dbo.WechatHomeAreaContent WHERE ID=@ID ";
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("ID", id);
            return conn.Execute(sql, parameter) > 0;
        }

        public bool DeleteProductContent(SqlConnection conn, int id)
        {
            string sql = "DELETE FROM Configuration.dbo.WechatHomeProductContent WHERE ID=@ID ";
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("ID", id);
            return conn.Execute(sql, parameter) > 0;
        }

        public IEnumerable<WechatHomeContent> GetContentList(SqlConnection conn, int fkid)
        {
            string sql = "SELECT * FROM Configuration.dbo.WechatHomeContent WHERE FKID=@FKID";
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@FKID", fkid);
            return conn.Query<WechatHomeContent>(sql, parameter);
        }

        public IEnumerable<WechatHomeAreaContent> GetAreaContentList(SqlConnection conn, int fkid)
        {
            string sql = "SELECT * FROM Configuration.dbo.WechatHomeAreaContent WHERE FKID=@FKID";
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@FKID", fkid);
            return conn.Query<WechatHomeAreaContent>(sql, parameter);
        }

        public IEnumerable<WechatHomeProductContent> GetProductContentList(SqlConnection conn, int fkid)
        {
            string sql = "SELECT * FROM Configuration.dbo.WechatHomeProductContent WHERE FKID=@FKID";
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@FKID", fkid);
            return conn.Query<WechatHomeProductContent>(sql, parameter);
        }

        public WechatHomeContent GetContentEntity(SqlConnection conn, int id)
        {
            string sql = "SELECT * FROM Configuration.dbo.WechatHomeContent WHERE ID=@ID";
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@ID", id);
            return conn.Query<WechatHomeContent>(sql, parameter).FirstOrDefault();
        }

        public WechatHomeAreaContent GetAreaContentEntity(SqlConnection conn, int id)
        {
            string sql = "SELECT * FROM Configuration.dbo.WechatHomeAreaContent WHERE ID=@ID";
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@ID", id);
            return conn.Query<WechatHomeAreaContent>(sql, parameter).FirstOrDefault();
        }

        public WechatHomeProductContent GetProductContentEntity(SqlConnection conn, int id)
        {
            string sql = "SELECT * FROM Configuration.dbo.WechatHomeProductContent WHERE ID=@ID";
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@ID", id);
            return conn.Query<WechatHomeProductContent>(sql, parameter).FirstOrDefault();
        }

        #region 微信小程序配置

        /// <summary>
        /// 微信小程序用户事件配置
        /// </summary>
        /// <returns></returns>
        public int SaveWxAppUserEventConfig(SqlConnection conn, WxAppUserEventConfigModel model)
        {
            return model.PKID > 0
                ? conn.Execute(@"UPDATE [Configuration].[dbo].[WxApp_UserEventConfig] WITH (ROWLOCK)
SET [EventType] = @EventType,
    [MsgType] = @MsgType,
    [ResponseJson] = @ResponseJson,
        [IsActive] = @IsActive,
        [UserData] = @UserData,
		[UpdateBy] = @UpdateBy,
		[LastUpdateDateTime] = GETDATE()
WHERE [IsDeleted] = 0
      AND [PKID] = @pkid;", model)
                : conn.Execute(@"INSERT INTO [Configuration].[dbo].[WxApp_UserEventConfig]
(
    [OriginId],
    [EventType],
    [MsgType],
    [ResponseJson],
    [UserData],
    [IsActive],
    [CreateBy],
    [UpdateBy],
    [CreateDateTime],
    [LastUpdateDateTime]
)
VALUES
(   @OriginId,        -- OriginId - varchar(20)
    @EventType,       -- EventType - nvarchar(50)
    @MsgType,        -- MsgType -nvarchar
    @ResponseJson,         -- ResponseJson - int
    @UserData,       -- UserData - nvarchar(500)
    @IsActive,      -- IsActive - bit
    @CreateBy,       -- CreateBy - nvarchar(50)
    @CreateBy,       -- UpdateBy - nvarchar(50)
    GETDATE(), -- CreateDateTime - datetime
    GETDATE()  -- LastUpdateDateTime - datetime
    )", model);
        }

        /// <summary>
        /// 查询用户事件配置
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="originId">微信原始id</param>
        /// <param name="eventType">事件类型</param>
        /// <param name="userData">用户输入</param>
        /// <returns></returns>
        public WxAppUserEventConfigModel FetchWxAppUserEventConfig(SqlConnection conn, string originId, string eventType, string userData)
        {
            if (string.IsNullOrEmpty(originId))
            {
                return new WxAppUserEventConfigModel();
            }

            if (string.IsNullOrEmpty(eventType))
            {
                eventType = "user_enter_tempsession";
            }

            return conn.QueryFirstOrDefault<WxAppUserEventConfigModel>($@"
SELECT [PKID],
       [OriginId],
       [EventType],
       [MsgType],
       [ResponseJson],
       [UserData],
       [IsActive],
       [CreateBy],
       [UpdateBy],
       [CreateDateTime],
       [LastUpdateDateTime]
FROM [Configuration].[dbo].[WxApp_UserEventConfig] WITH(NOLOCK)
WHERE [IsDeleted] = 0
AND [OriginId] = @originId
AND [EventType] = @eventType
AND {(string.IsNullOrWhiteSpace(userData) ? "[UserData] IS NULL" : "[UserData] = @userData")}", new
            {
                originId,
                eventType,
                userData = userData?.Trim().Replace(" ", "")
            }) ?? new WxAppUserEventConfigModel();
        }

        /// <summary>
        /// 查询用户事件配置
        /// </summary>
        /// <param name="conn">SqlConnection</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        public WxAppUserEventConfigModel FetchWxAppUserEventConfig(SqlConnection conn, int id)
        {
            return conn.QueryFirstOrDefault<WxAppUserEventConfigModel>(@"
SELECT [PKID],
       [OriginId],
       [EventType],
       [MsgType],
       [ResponseJson],
       [UserData],
       [IsActive],
       [CreateBy],
       [UpdateBy],
       [CreateDateTime],
       [LastUpdateDateTime]
FROM [Configuration].[dbo].[WxApp_UserEventConfig] WITH(NOLOCK)
WHERE [IsDeleted] = 0
AND [PKID] = @id", new
            {
                id
            }) ?? new WxAppUserEventConfigModel();
        }

        /// <summary>
        /// 查询用户事件配置
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="originId">微信原始id</param>
        /// <returns></returns>
        public IEnumerable<WxAppUserEventConfigModel> SelectWxAppUserEventConfig(SqlConnection conn, string originId)
        {
            if (string.IsNullOrEmpty(originId))
            {
                return Enumerable.Empty<WxAppUserEventConfigModel>();
            }

            return conn.Query<WxAppUserEventConfigModel>(@"
SELECT [PKID],
       [OriginId],
       [EventType],
       [MsgType],
       [ResponseId],
       [KeyWord],
       [IsActive],
       [CreateBy],
       [UpdateBy],
       [CreateDateTime],
       [LastUpdateDateTime]
FROM [Configuration].[dbo].[WxApp_UserEventConfig] WITH(NOLOCK)
WHERE [IsDeleted] = 0
AND [OriginId] = @originId", new
            {
                originId
            }) ?? Enumerable.Empty<WxAppUserEventConfigModel>();
        }

        /// <summary>
        /// 查询所有小程序配置
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns></returns>
        public IEnumerable<WxAppUserEventConfigModel> SelectWxAppUserEventConfig(SqlConnection conn, int pageIndex, int pageSize)
        {
            return conn.Query<WxAppUserEventConfigModel>(@"
SELECT [PKID],
       [OriginId],
       [EventType],
       [MsgType],
       [UserData],
       [IsActive],
       [CreateBy],
       [UpdateBy],
       [CreateDateTime],
       [LastUpdateDateTime]
FROM [Configuration].[dbo].[WxApp_UserEventConfig] WITH(NOLOCK)
WHERE [IsDeleted] = 0
ORDER BY [PKID] DESC
OFFSET @offset ROWS
FETCH NEXT @size ROWS ONLY", new
            {
                offset = (pageIndex - 1) * pageSize,
                size = pageSize
            });
        }

        /// <summary>
        /// 删除配置（逻辑删除
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="model">model</param>
        /// <returns></returns>
        public int DeleteWxAppConfig(SqlConnection conn, WxAppUserEventConfigModel model)
        {
            return conn.Execute(@"UPDATE [Configuration].[dbo].[WxApp_UserEventConfig] WITH (ROWLOCK)
SET [IsDeleted] = 1,
    [LastUpdateDateTime] = GETDATE(),
    [UpdateBy] = @UpdateBy
WHERE [PKID]=@PKID;", model);
        }

        #endregion 微信小程序配置

        #region 微信社交立减金配置

        /// <summary>
        /// 添加微信社交立减金代金券配置
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="model">model</param>
        /// <returns></returns>
        public int AddWechatSocialCardConfig(SqlConnection conn, WechatSocialCardConfigModel model) => conn.Execute(@"INSERT INTO [dbo].[WechatSocialCardConfig]
(
    [CardId],
    [MerchantName],
    [MerchantNo],
    [CardName],
    [CardDescription],
    [LogoUrl],
    [OtherMerchantNo],
    [CardColor],
    [CutomerServiceTelphone],
    [CardDateInfoType],
    [BeginTime],
    [EndTime],
    [FixedTerm],
    [FixedBeginTerm],
    [EndDateTime],
    [CardAmount],
    [CardCondition],
    [IsCanUseWithOtherDiscount],
    [CardButtonText],
    [CardButtonToWxApp],
    [CardButtonToPath],
    [IsCanShare],
    [IsCanGiveToFriend],
    [CardStockQuantity],
    [CardGetLimit],
    [CreateDateTime],
    [CreatedBy],
    [LastUpdateDateTime],
    [UpdatedBy]
)
VALUES
(   @CardId,       -- CardId - nvarchar(50)
    @MerchantName,       -- MerchantName - nvarchar(20)
    @MerchantNo,       -- MerchantNo - nvarchar(50)
    @CardName,       -- CardName - nvarchar(20)
    @CardDescription,       -- CardDescription - nvarchar(500)
    @LogoUrl,       -- LogoUrl - nvarchar(500)
    @OtherMerchantNo,       -- OtherMerchantNo - nvarchar(500)
    @CardColor,       -- CardColor - nvarchar(50)
    @CutomerServiceTelphone,       -- CutomerServiceTelphone - nvarchar(50)
    @CardDateInfoType,       -- CardDateInfoType - nvarchar(50)
    @BeginTime, -- BeginTime - datetime
    @EndTime, -- EndTime - datetime
    @FixedTerm,         -- FixedTerm - int
    @FixedBeginTerm,         -- FixedBeginTerm - int
    @EndDateTime, -- EndDateTime - datetime
    @CardAmount,       -- CardAmount - nvarchar(50)
    @CardCondition,         -- CardCondition - int
    @IsCanUseWithOtherDiscount,      -- IsCanUseWithOtherDiscount - bit
    @CardButtonText,       -- CardButtonText - nvarchar(50)
    @CardButtonToWxApp,       -- CardButtonToWxApp - nvarchar(50)
    @CardButtonToPath,       -- CardButtonToPath - nvarchar(200)
    @IsCanShare,      -- IsCanShare - bit
    @IsCanGiveToFriend,      -- IsCanGiveToFriend - bit
    @CardStockQuantity,         -- CardStockQuantity - bigint
    @CardGetLimit,         -- CardGetLimit - int
    GETDATE(), -- CreateDateTime - datetime
    @CreatedBy,       -- CreatedBy - nvarchar(50)
    GETDATE(), -- LastUpdateDateTime - datetime
    @CreatedBy        -- UpdatedBy - nvarchar(50)
    )", model);

        /// <summary>
        /// 查询微信社交立减金代金券配置
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns></returns>
        public IEnumerable<WechatSocialCardConfigModel> SelectWechatSocialCardConfig(SqlConnection conn, int pageIndex, int pageSize) => conn.Query<WechatSocialCardConfigModel>(@"SELECT [PKID],
       [CardId],
       [MerchantName],
       [MerchantNo],
       [CardName],
       [CardDescription],
       [LogoUrl],
       [OtherMerchantNo],
       [CardColor],
       [CutomerServiceTelphone],
       [CardDateInfoType],
       [BeginTime],
       [EndTime],
       [FixedTerm],
       [FixedBeginTerm],
       [EndDateTime],
       [CardAmount],
       [CardCondition],
       [IsCanUseWithOtherDiscount],
       [CardButtonText],
       [CardButtonToWxApp],
       [CardButtonToPath],
       [IsCanShare],
       [IsCanGiveToFriend],
       [CardStockQuantity],
       [CardGetLimit],
       [CreateDateTime],
       [CreatedBy]
FROM [dbo].[WechatSocialCardConfig] WITH (NOLOCK)
ORDER BY [PKID] DESC OFFSET @offset ROWS FETCH NEXT @size ROWS ONLY;",
            new { offset = (pageIndex - 1) * pageSize, size = pageSize });

        public WechatSocialCardConfigModel FetchWxAppByCardId(SqlConnection conn, string cardId) => conn.QueryFirstOrDefault<WechatSocialCardConfigModel>(@"SELECT TOP 1
             [CardId],
             [MerchantName],
             [MerchantNo],
             [CardName],
             [CardColor],
             [CutomerServiceTelphone],
             [CardDateInfoType],
             [BeginTime],
             [EndTime],
             [FixedTerm],
             [FixedBeginTerm],
             [EndDateTime],
             [CardAmount],
             [CardCondition],
             [IsCanUseWithOtherDiscount],
             [CardButtonText],
             [CardButtonToWxApp],
             [CardButtonToPath],
             [IsCanShare],
             [IsCanGiveToFriend],
             [CardStockQuantity],
             [CardGetLimit],
             [CreateDateTime],
             [CreatedBy]
FROM [dbo].[WechatSocialCardConfig] WITH (NOLOCK)
WHERE [CardId] = @cardId;", new { cardId });

        /// <summary>
        /// 添加微信社交立减金活动配置
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="model">model</param>
        /// <returns></returns>
        public int AddWechatSocialActivityConfig(SqlConnection conn, WechatSocialActivityConfigModel model) => conn.Execute(@"INSERT INTO [dbo].[WechatSocialActivityConfig]
(
    [CardId],
    [ActivityId],
    [ActivityBgColor],
    [ActivityWxAppId],
    [BeginTime],
    [EndTime],
    [GiftNum],
    [MerchantNo],
    [MaxParticTimesAct],
    [MaxParticTimesOneDay],
    [MinAmount],
    [UserScope],
    [CreateDateTime],
    [CreatedBy],
    [LastUpdateDateTime],
    [UpdatedBy]
)
VALUES
(   @CardId,       -- CardId - nvarchar(50)
    @ActivityId,       -- ActivityId - nvarchar(50)
    @ActivityBgColor,       -- ActivityBgColor - nvarchar(50)
    @ActivityWxAppId,       -- ActivityWxAppId - nvarchar(50)
    @BeginTime, -- BeginTime - datetime
    @EndTime, -- EndTime - datetime
    @GiftNum,         -- GiftNum - int
    @MerchantNo,       -- MerchantNo - nvarchar(50)
    @MaxParticTimesAct,         -- MaxParticTimesAct - int
    @MaxParticTimesOneDay,         -- MaxParticTimesOneDay - int
    @MinAmount,         -- MinAmount - int
    @UserScope,         -- UserScope - int
    GETDATE(), -- CreateDateTime - datetime
    @CreatedBy,       -- CreatedBy - nvarchar(50)
    GETDATE(), -- LastUpdateDateTime - datetime
    @CreatedBy        -- UpdatedBy - nvarchar(50)
    )", model);

        /// <summary>
        /// 查询微信社交立减金活动配置
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns></returns>
        public IEnumerable<WechatSocialActivityConfigModel> SelectWechatSocialActivityConfig(SqlConnection conn,
            int pageIndex, int pageSize) => conn.Query<WechatSocialActivityConfigModel>(@"SELECT [PKID],
       [CardId],
       [ActivityId],
       [ActivityBgColor],
       [ActivityWxAppId],
       [BeginTime],
       [EndTime],
       [GiftNum],
       [MerchantNo],
       [MaxParticTimesAct],
       [MaxParticTimesOneDay],
       [MinAmount],
       [UserScope],
       [CreateDateTime],
       [CreatedBy],
       [LastUpdateDateTime],
       [UpdatedBy]
FROM [dbo].[WechatSocialActivityConfig] WITH (NOLOCK)
ORDER BY [PKID] DESC OFFSET @offset ROWS FETCH NEXT @size ROWS ONLY;",
            new { offset = (pageIndex - 1) * pageSize, size = pageSize });

        #endregion 微信社交立减金配置
    }
}