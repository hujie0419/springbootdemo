using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Data;
using System.Data.SqlClient;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALSE_HomePageConfig
    {



        public DALSE_HomePageConfig()
        {

        }


        #region 首页列表操作
        /// <summary>
        /// 返回主键的ID
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddHomePage(SqlConnection connection, SE_HomePageConfig model, SqlTransaction transaction)
        {
            string sqlString = @"
                    DECLARE @InseredTable TABLE (
                    ID INT 
                    ) ;


                    INSERT INTO Configuration.dbo.SE_HomePageConfig
                            ( HomePageName ,
                              IsEnabled ,
                              CreateDateTime ,
                              UpdateDateTime,
                              TypeConfig,
                              StartVersion,
                              EndVersion
                            )
		                    OUTPUT Inserted.ID INTO @InseredTable
                    VALUES  ( @HomePageName , -- HomePageName - nvarchar(50)
                              @IsEnabled , -- IsEnabled - bit
                              GETDATE() , -- CreateDateTime - datetime
                              GETDATE(),  -- UpdateDateTime - datetime
                                @TypeConfig,
                              @StartVersion,
                              @EndVersion
                            )
                    SELECT TOP 1 @PKID=ID FROM @InseredTable	
";


            var parameters = new DynamicParameters();
            parameters.Add("@PKID", null, DbType.Int32, ParameterDirection.Output, null);
            parameters.Add("@HomePageName", model.HomePageName);
            parameters.Add("@IsEnabled", model.IsEnabled);
            parameters.Add("@TypeConfig", model.TypeConfig);
            parameters.Add("@StartVersion", model.StartVersion);
            parameters.Add("@EndVersion", model.EndVersion);
            int n = connection.Execute(sqlString, parameters, transaction);
            if (n > 0)
            {
                model.ID = parameters.Get<int>("@PKID");
                return parameters.Get<int>("@PKID");
            }
            else
                return -1;

        }


        /// <summary>
        /// 获取首页实体
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public SE_HomePageConfig GetHomePageEntity(SqlConnection connection, int id)
        {
            string sqlString = @"SELECT * FROM Configuration.dbo.SE_HomePageConfig (NOLOCK) WHERE ID=@ID";
            return connection.Query<SE_HomePageConfig>(sqlString, new SE_HomePageConfig() { ID = id }).FirstOrDefault();
        }

        /// <summary>
        /// 判断首页是否存在
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="startVersion"></param>
        /// <param name="endVersion"></param>
        /// <returns></returns>
        public bool IsExitsVersion(SqlConnection connection, string startVersion, string endVersion, int? id = null)
        {
            string sqlString = @"SELECT COUNT(*) FROM Configuration.dbo.SE_HomePageConfig WITH (NOLOCK) WHERE  ((Configuration.dbo.fun_split_version(StartVersion) <= Configuration.dbo.fun_split_version(@StartVersion) 
                AND  Configuration.dbo.fun_split_version(EndVersion) >=Configuration.dbo.fun_split_version(@StartVersion) ) OR (Configuration.dbo.fun_split_version(StartVersion) <= Configuration.dbo.fun_split_version(@EndVersion) 
                AND  Configuration.dbo.fun_split_version(EndVersion) >=Configuration.dbo.fun_split_version(@EndVersion) ) )  AND TypeConfig=1 AND  IsEnabled=1 ";
            var parameters = new DynamicParameters();
            if (id.HasValue)
            {
                sqlString += "  AND ID != @ID  ";
                parameters.Add("@ID", id.Value);
            }
            parameters.Add("@StartVersion", startVersion);
            parameters.Add("@EndVersion", endVersion);
            int n = (int)connection.ExecuteScalar(sqlString, parameters);
            return n > 0 ? true : false;
        }


        /// <summary>
        /// 更新首页信息
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateHomePage(SqlConnection connection, SE_HomePageConfig model)
        {

            string sqlString = @"UPDATE Configuration.dbo.SE_HomePageConfig SET StartVersion=@StartVersion,EndVersion=@EndVersion,  HomePageName=@HomePageName, 
                                   IsEnabled=@IsEnabled,
                                UpdateDateTime=@UpdateDateTime WHERE ID=@ID";
            return connection.Execute(sqlString, model) > 0;
        }

        public bool UpdateHomePageAll(SqlConnection connection, SE_HomePageConfig model)
        {
            var trans = connection.BeginTransaction();
            try
            {
                connection.Execute("UPDATE Configuration.dbo.SE_HomePageConfig SET IsEnabled=0 where TypeConfig=@TypeConfig", model, trans);

                string sqlString = @"UPDATE Configuration.dbo.SE_HomePageConfig SET HomePageName=@HomePageName, 
                                   IsEnabled=@IsEnabled,
                                UpdateDateTime=@UpdateDateTime WHERE ID=@ID";
                connection.Execute(sqlString, model, trans);
                trans.Commit();
                return true;
            }
            catch (Exception em)
            {
                trans.Rollback();
                throw new Exception("更新失败" + em.Message);
            }
        }


        public bool IsExitsEnabled(SqlConnection connection, int outID, int TypeConfig)
        {
            string sql = "SELECT * FROM Configuration.dbo.SE_HomePageConfig WHERE IsEnabled=1 and TypeConfig=@TypeConfig AND ID != @ID";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ID", outID);
            dp.Add("@TypeConfig", TypeConfig);
            return connection.Query<SE_HomePageConfig>(sql, dp).Count() > 0;
        }


        #endregion


        #region 更新模块顺序
        /// <summary>
        /// 添加首页模块
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddHomePageModule(SqlConnection connection, SE_HomePageModuleConfig model, SqlTransaction transaction = null)
        {
            string sqlString = @"
                   INSERT INTO Configuration.dbo.SE_HomePageModuleConfig
                    ( FKHomePage ,
                      ModuleName ,
                      ModuleType ,
                      IsEnabled ,
                      PriorityLevel ,
                      CreateDateTime ,
                      UpdateDateTime ,
                      SpliteLine ,
                      BgImageUrl ,
                      FontColor ,
                      StartVersion ,
                      EndVersion ,
                      Title ,
                      TitleImageUrl ,
                      TitleColor ,
                      IsMore ,
                      MoreUri ,
                      IsTag ,
                      TagContent ,
                      IsChildModule ,
                      IsMoreChannel ,
                      IsMoreCity,
                      BgColor,
                      TitleBgColor,
                      Pattern,
                      Margin,UriCount,IsNewUser
                    )
                    VALUES  ( @FKHomePage , -- FKHomePage - int
                      @ModuleName , -- ModuleName - nvarchar(50)
                      @ModuleType , -- ModeulType - int
                      @IsEnabled , -- IsEnabled - bit
                      @PriorityLevel , -- PriorityLevel - int
                      GETDATE() , -- CreateDateTime - datetime
                      GETDATE() , -- UpdateDateTime - datetime
                      @SpliteLine , -- SpliteLine - nvarchar(50)
                      @BgImageUrl , -- BgImageUrl - nvarchar(1000)
                      @FontColor , -- FontColor - nvarchar(100)
                      @StartVersion , -- StartVersion - nvarchar(20)
                      @EndVersion , -- EndVersion - nvarchar(20)
                      @Title , -- Title - nvarchar(200)
                      @TitleImageUrl , -- TitleImageUrl - nvarchar(1000)
                      @TitleColor , -- TitleColor - nvarchar(100)
                      @IsMore , -- IsMore - bit
                      @MoreUri , -- MoreUri - nvarchar(500)
                      @IsTag , -- IsTag - bit
                     @TagContent , -- TagContent - nvarchar(500)
                      @IsChildModule , -- IsChildModule - bit
                      @IsMoreChannel , -- IsMoreChannel - bit
                      @IsMoreCity , -- IsMoreCity - bit
                      @BgColor,
                      @TitleBgColor,
                      @Pattern,
                      @Margin,@UriCount,@IsNewUser)	";
            var n = transaction == null ? connection.Execute(sqlString, model) : connection.Execute(sqlString, model, transaction);

            if (n > 0)
                // 更新排序后，主动更新配置表的时间
                connection.Execute(" UPDATE Configuration.dbo.SE_HomePageConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID", new SE_HomePageConfig { ID = (int)model.FKHomePage });

            return n > 0;
        }


        /// <summary>
        /// 添加首页模块
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddHomePageModule(SqlConnection connection, SqlTransaction transaction, SE_HomePageModuleConfig model)
        {
            string sqlString = @"
  DECLARE @InseredTable TABLE (
                    ID INT 
                    ) ;

                   INSERT INTO Configuration.dbo.SE_HomePageModuleConfig
        ( FKHomePage ,
          ModuleName ,
          ModuleType ,
          IsEnabled ,
          PriorityLevel ,
          CreateDateTime ,
          UpdateDateTime ,
          SpliteLine ,
          BgImageUrl ,
          FontColor ,
          StartVersion ,
          EndVersion ,
          Title ,
          TitleImageUrl ,
          TitleColor ,
          IsMore ,
          MoreUri ,
          IsTag ,
          TagContent ,
          IsChildModule ,
          IsMoreChannel ,
          IsMoreCity,
          BgColor,
          TitleBgColor,
          Pattern,
          Margin,UriCount,IsNewUser,FileUrl,ImageType
        )
  OUTPUT Inserted.ID INTO @InseredTable
 VALUES  ( @FKHomePage , -- FKHomePage - int
          @ModuleName , -- ModuleName - nvarchar(50)
          @ModuleType , -- ModeulType - int
          @IsEnabled , -- IsEnabled - bit
          @PriorityLevel , -- PriorityLevel - int
          GETDATE() , -- CreateDateTime - datetime
          GETDATE() , -- UpdateDateTime - datetime
          @SpliteLine , -- SpliteLine - nvarchar(50)
          @BgImageUrl , -- BgImageUrl - nvarchar(1000)
          @FontColor , -- FontColor - nvarchar(100)
          @StartVersion , -- StartVersion - nvarchar(20)
          @EndVersion , -- EndVersion - nvarchar(20)
          @Title , -- Title - nvarchar(200)
          @TitleImageUrl , -- TitleImageUrl - nvarchar(1000)
          @TitleColor , -- TitleColor - nvarchar(100)
          @IsMore , -- IsMore - bit
          @MoreUri , -- MoreUri - nvarchar(500)
          @IsTag , -- IsTag - bit
         @TagContent , -- TagContent - nvarchar(500)
          @IsChildModule , -- IsChildModule - bit
          @IsMoreChannel , -- IsMoreChannel - bit
          @IsMoreCity , -- IsMoreCity - bit
          @BgColor,
          @TitleBgColor,
          @Pattern,
          @Margin,@UriCount,@IsNewUser,@FileUrl,@ImageType
        )	 SELECT TOP 1 @PKID=ID FROM @InseredTable	";
            var parameters = new DynamicParameters();
            parameters.Add("@FKHomePage", model.FKHomePage);
            parameters.Add("@ModuleName", model.ModuleName);
            parameters.Add("@ModuleType", model.ModuleType);
            parameters.Add("@IsEnabled", model.IsEnabled);
            parameters.Add("@PriorityLevel", model.PriorityLevel);
            parameters.Add("@SpliteLine", model.SpliteLine);
            parameters.Add("@BgImageUrl", model.BgImageUrl);
            parameters.Add("@FontColor", model.FontColor);
            parameters.Add("@StartVersion", model.StartVersion);
            parameters.Add("@EndVersion", model.EndVersion);
            parameters.Add("@Title", model.Title);
            parameters.Add("@TitleImageUrl", model.TitleImageUrl);
            parameters.Add("@TitleColor", model.TitleColor);
            parameters.Add("@IsMore", model.IsMore);
            parameters.Add("@MoreUri", model.MoreUri);
            parameters.Add("@IsTag", model.IsTag);
            parameters.Add("@TagContent", model.TagContent);
            parameters.Add("@IsChildModule", model.IsChildModule);
            parameters.Add("@IsMoreChannel", model.IsMoreChannel);
            parameters.Add("@IsMoreCity", model.IsMoreCity);
            parameters.Add("@BgColor", model.BgColor);
            parameters.Add("@TitleBgColor", model.TitleBgColor);
            parameters.Add("@Pattern", model.Pattern);
            parameters.Add("@Margin", model.Margin);
            parameters.Add("@UriCount", model.UriCount);
            parameters.Add("@IsNewUser", model.IsNewUser);
            parameters.Add("@FileUrl", model.FileUrl);
            parameters.Add("@ImageType", model.ImageType);
            parameters.Add("@PKID", null, DbType.Int32, ParameterDirection.Output, null);
            int n = connection.Execute(sqlString, parameters, transaction);
            if (n > 0)
            {
                return parameters.Get<int>("@PKID");
            }
            else
                throw new Exception("添加模块失败");
        }

        /// <summary>
        /// 更新模块信息
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateHomePageModule(SqlConnection connection, SE_HomePageModuleConfig model)
        {
            StringBuilder sqlString = new StringBuilder("UPDATE Configuration.dbo.SE_HomePageModuleConfig set ");
            sqlString.Append(" Margin=@Margin, Pattern=@Pattern, ModuleName =@ModuleName, IsEnabled=@IsEnabled,PriorityLevel=@PriorityLevel, UpdateDateTime=GETDATE(), ");
            sqlString.Append(" SpliteLine=@SpliteLine, BgImageUrl=@BgImageUrl,   FontColor=@FontColor , StartVersion=@StartVersion, EndVersion=@EndVersion, ");
            sqlString.Append(" Title=@Title, TitleImageUrl=@TitleImageUrl, TitleColor=@TitleColor,TitleBgColor=@TitleBgColor,  IsMore=@IsMore ,UriCount=@UriCount,");
            sqlString.Append("  MoreUri=@MoreUri, IsTag=@IsTag, TagContent=@TagContent,IsChildModule=@IsChildModule  , IsMoreChannel=@IsMoreChannel,IsMoreCity=@IsMoreCity,BgColor=@BgColor,IsNewUser=@IsNewUser,FileUrl=@FileUrl,ImageType=@ImageType ");
            sqlString.Append(" where ID=@ID ");

            // 更新排序后，主动更新配置表的时间
            connection.Execute(" UPDATE Configuration.dbo.SE_HomePageConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID", new SE_HomePageConfig { ID = (int)model.FKHomePage });

            return connection.Execute(sqlString.ToString(), model) > 0;

        }

        /// <summary>
        /// 更新模块顺序
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="modules"></param>
        /// <returns></returns>
        public bool UpdateHomePageModulePriorityLevel(SqlConnection connection, IEnumerable<SE_HomePageModuleConfig> modules)
        {
            var trans = connection.BeginTransaction();
            try
            {
                var configs = modules as SE_HomePageModuleConfig[] ?? modules.ToArray();
                foreach (var item in configs)
                {
                    connection.Execute("UPDATE Configuration.dbo.SE_HomePageModuleConfig SET PriorityLevel=@PriorityLevel,UpdateDateTime = GETDATE()  WHERE ID=@ID ", item, trans);
                }

                if (configs.Any())
                {
                    // 更新排序后，主动更新配置表的时间
                    var updateSql = @"UPDATE Configuration.dbo.SE_HomePageConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID";
                    var p = new DynamicParameters();
                    p.Add("@ID", configs[0].FKHomePage);
                    connection.Execute(updateSql, p, trans);
                }

                trans.Commit();

                return true;
            }
            catch (Exception em)
            {
                trans.Rollback();
                return false;
            }
        }

        /// <summary>
        /// 添加附属模块
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        /// <param name="citys"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool AddHomePageModuleHelper(SqlConnection connection, SE_HomePageModuleHelperConfig model, IEnumerable<SE_ModuleHelperCityConfig> citys, int pkid)
        {
            var trans = connection.BeginTransaction();
            try
            {

                #region 附属模块SQL
                const string sql = @"
                 DECLARE @InseredTable TABLE (
                     ID INT 
                 ) ;

                INSERT INTO Configuration.dbo.SE_HomePageModuleHelperConfig
                ( FKHomePageModuleID ,
                  ModuleName ,
                  ModuleType ,
                  IsEnabled ,
                  PriorityLevel ,
                  SpliteLine ,
                  BgImageUrl ,
                  FontColor ,
                  StartVersion ,
                  EndVersion ,
                  Title ,
                  TitleImageUrl ,
                  TitleColor ,
                  IsMore ,
                  MoreUri ,
                  IsTag ,
                  TagContent ,
                  Channel ,
                  CreateDateTime ,
                  UpdateDateTime,
                  BgColor,
                  TitleBgColor,
                  Pattern,Margin,UriCount,IsNewUser,NoticeChannel,FileUrl,ImageType
                )
                OUTPUT Inserted.ID INTO @InseredTable
                VALUES  ( @FKHomePageModuleID , -- FKHomePageModuleID - int
                          @ModuleName , -- ModuleName - nvarchar(50)
                          @ModuleType , -- ModeulType - int
                          @IsEnabled , -- IsEnabled - bit
                          @PriorityLevel , -- PriorityLevel - int
                          @SpliteLine , -- SpliteLine - nvarchar(50)
                          @BgImageUrl , -- BgImageUrl - nvarchar(1000)
                          @FontColor , -- FontColor - nvarchar(100)
                          @StartVersion , -- StartVersion - nvarchar(20)
                          @EndVersion , -- EndVersion - nvarchar(20)
                          @Title , -- Title - nvarchar(200)
                          @TitleImageUrl , -- TitleImageUrl - nvarchar(1000)
                          @TitleColor , -- TitleColor - nvarchar(100)
                          @IsMore , -- IsMore - bit
                          @MoreUri , -- MoreUri - nvarchar(500)
                          @IsTag , -- IsTag - bit
                          @TagContent , -- TagContent - nvarchar(500)
                          @Channel , -- Channel - int
                          GETDATE() , -- CreateDateTime - datetime
                          GETDATE(),  -- UpdateDateTime - datetime
                          @BgColor,
                          @TitleBgColor,
                          @Pattern,@Margin,@UriCount,@IsNewUser,@NoticeChannel,@FileUrl,@ImageType
                        )  

               SELECT TOP 1 @PKID=ID FROM @InseredTable	";
                #endregion

                var parameters = new DynamicParameters();
                parameters.Add("@FKHomePageModuleID", model.FKHomePageModuleID);
                parameters.Add("@ModuleName", model.ModuleName);
                parameters.Add("@ModuleType", model.ModuleType);
                parameters.Add("@IsEnabled", model.IsEnabled);
                parameters.Add("@PriorityLevel", model.PriorityLevel);
                parameters.Add("@SpliteLine", model.SpliteLine);
                parameters.Add("@BgImageUrl", model.BgImageUrl);
                parameters.Add("@FontColor", model.FontColor);
                parameters.Add("@StartVersion", model.StartVersion);
                parameters.Add("@EndVersion", model.EndVersion);
                parameters.Add("@Title", model.Title);
                parameters.Add("@TitleImageUrl", model.TitleImageUrl);
                parameters.Add("@TitleColor", model.TitleColor);
                parameters.Add("@IsMore", model.IsMore);
                parameters.Add("@MoreUri", model.MoreUri);
                parameters.Add("@IsTag", model.IsTag);
                parameters.Add("@TagContent", model.TagContent);
                parameters.Add("@Channel", model.Channel);
                parameters.Add("@PKID", null, DbType.Int32, ParameterDirection.Output, null);
                parameters.Add("@BgColor", model.BgColor);
                parameters.Add("@TitleBgColor", model.TitleBgColor);
                parameters.Add("@Pattern", model.Pattern);
                parameters.Add("@Margin", model.Margin);
                parameters.Add("@UriCount", model.UriCount);
                parameters.Add("@IsNewUser", model.IsNewUser);
                parameters.Add("@NoticeChannel", model.NoticeChannel);
                parameters.Add("@FileUrl", model.FileUrl);
                parameters.Add("@ImageType", model.ImageType);
                connection.Execute(sql, parameters, trans);
                model.ID = parameters.Get<int>("@PKID");
                if (model.ID <= 0)
                {
                    throw new Exception("插入附属的模块失败");
                }

                //添加模块城市数据
                const string sqlCityString = @"INSERT INTO Configuration.dbo.SE_ModuleHelperCityConfig
                                    ( FKHomePageModuleHelperID ,
                                      FKRegionPKID ,
                                      CreateDateTime ,
                                      UpdateDateTime
                                    )
                                    VALUES  ( @FKHomePageModuleHelperID , -- FKHomePageModuleHelperID - int
                                      @FKRegionPKID , -- FKRegionPKID - int
                                      GETDATE() , -- CreateDateTime - datetime
                                      GETDATE()  -- UpdateDateTime - datetime
                                    )";
                if (citys != null)
                {
                    foreach (var item in citys)
                    {
                        var p = new DynamicParameters();
                        p.Add("@FKHomePageModuleHelperID", model.ID);
                        p.Add("@FKRegionPKID", item.FKRegionPKID);
                        connection.Execute(sqlCityString, p, trans);
                    }
                }

                // 更新排序后，主动更新配置表的时间
                connection.Execute(" UPDATE Configuration.dbo.SE_HomePageConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID", new SE_HomePageConfig { ID = pkid }, trans);
                connection.Execute(" UPDATE Configuration.dbo.SE_HomePageModuleConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID", new SE_HomePageModuleConfig { ID = model.FKHomePageModuleID.Value }, trans);

                trans.Commit();

                #region 添加对应的父模块内容

                //如果是拼图模块就不默认增加父模块内容了，单独的替换逻辑处理
                if (model.ModuleType == 30)
                    return true;

                var parentContent = GetHomePageContentList(connection, model.FKHomePageModuleID, null);

                if (parentContent == null)
                    return true;

                foreach (var content in parentContent)
                {
                    content.ID = 0;
                    content.FKHomePageModuleHelperID = model.ID;
                    content.FKHomePageModuleID = null;
                    AddHomePageContent(connection, content, pkid);
                }

                #endregion
                return true;
            }
            catch (Exception em)
            {
                trans.Rollback();
                throw new Exception(em.Message);
            }
        }

        /// <summary>
        /// 添加附属模块
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="trans"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddHomePageModuleHelper(SqlConnection connection, SqlTransaction trans, SE_HomePageModuleHelperConfig model)
        {
            #region 附属模块SQL
            string sql = @"
                 DECLARE @InseredTable TABLE (
                                    ID INT 
                                    ) ;

                INSERT INTO Configuration.dbo.SE_HomePageModuleHelperConfig
                        ( FKHomePageModuleID ,
                          ModuleName ,
                          ModuleType ,
                          IsEnabled ,
                          PriorityLevel ,
                          SpliteLine ,
                          BgImageUrl ,
                          FontColor ,
                          StartVersion ,
                          EndVersion ,
                          Title ,
                          TitleImageUrl ,
                          TitleColor ,
                          IsMore ,
                          MoreUri ,
                          IsTag ,
                          TagContent ,
                          Channel ,
                          CreateDateTime ,
                          UpdateDateTime,
                          BgColor,
                         TitleBgColor,
                        Pattern,Margin,UriCount,IsNewUser,NoticeChannel
                        )
                  OUTPUT Inserted.ID INTO @InseredTable
                VALUES  ( @FKHomePageModuleID , -- FKHomePageModuleID - int
                          @ModuleName , -- ModuleName - nvarchar(50)
                          @ModuleType , -- ModeulType - int
                          @IsEnabled , -- IsEnabled - bit
                          @PriorityLevel , -- PriorityLevel - int
                          @SpliteLine , -- SpliteLine - nvarchar(50)
                          @BgImageUrl , -- BgImageUrl - nvarchar(1000)
                          @FontColor , -- FontColor - nvarchar(100)
                          @StartVersion , -- StartVersion - nvarchar(20)
                          @EndVersion , -- EndVersion - nvarchar(20)
                          @Title , -- Title - nvarchar(200)
                          @TitleImageUrl , -- TitleImageUrl - nvarchar(1000)
                          @TitleColor , -- TitleColor - nvarchar(100)
                          @IsMore , -- IsMore - bit
                          @MoreUri , -- MoreUri - nvarchar(500)
                          @IsTag , -- IsTag - bit
                          @TagContent , -- TagContent - nvarchar(500)
                          @Channel , -- Channel - int
                          GETDATE() , -- CreateDateTime - datetime
                          GETDATE(),  -- UpdateDateTime - datetime
                          @BgColor,
                          @TitleBgColor,
                          @Pattern,@Margin,@UriCount,@IsNewUser,@NoticeChannel
                        )  

               SELECT TOP 1 @PKID=ID FROM @InseredTable	
";
            #endregion
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@FKHomePageModuleID", model.FKHomePageModuleID);
            parameters.Add("@ModuleName", model.ModuleName);
            parameters.Add("@ModuleType", model.ModuleType);
            parameters.Add("@IsEnabled", model.IsEnabled);
            parameters.Add("@PriorityLevel", model.PriorityLevel);
            parameters.Add("@SpliteLine", model.SpliteLine);
            parameters.Add("@BgImageUrl", model.BgImageUrl);
            parameters.Add("@FontColor", model.FontColor);
            parameters.Add("@StartVersion", model.StartVersion);
            parameters.Add("@EndVersion", model.EndVersion);
            parameters.Add("@Title", model.Title);
            parameters.Add("@TitleImageUrl", model.TitleImageUrl);
            parameters.Add("@TitleColor", model.TitleColor);
            parameters.Add("@IsMore", model.IsMore);
            parameters.Add("@MoreUri", model.MoreUri);
            parameters.Add("@IsTag", model.IsTag);
            parameters.Add("@TagContent", model.TagContent);
            parameters.Add("@Channel", model.Channel);
            parameters.Add("@PKID", null, DbType.Int32, ParameterDirection.Output, null);
            parameters.Add("@BgColor", model.BgColor);
            parameters.Add("@TitleBgColor", model.TitleBgColor);
            parameters.Add("@Pattern", model.Pattern);
            parameters.Add("@Margin", model.Margin);
            parameters.Add("@UriCount", model.UriCount);
            parameters.Add("@IsNewUser", model.IsNewUser);
            parameters.Add("@NoticeChannel", model.NoticeChannel);
            connection.Execute(sql, parameters, trans);
            int pkid = parameters.Get<int>("@PKID");
            if (pkid <= 0)
            {
                throw new Exception("插入附属的模块失败");
            }
            return pkid;
        }

        /// <summary>
        /// 添加城市
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="trans"></param>
        /// <param name="citys"></param>
        /// <param name="moduleHelperID"></param>
        /// <returns></returns>
        public bool AddHomePageModuleHelperByCity(SqlConnection connection, SqlTransaction trans, IEnumerable<SE_ModuleHelperCityConfig> citys, int moduleHelperID)
        {
            string sqlCityString = @"INSERT INTO Configuration.dbo.SE_ModuleHelperCityConfig
        ( FKHomePageModuleHelperID ,
          FKRegionPKID ,
          CreateDateTime ,
          UpdateDateTime
        )
VALUES  ( @FKHomePageModuleHelperID , -- FKHomePageModuleHelperID - int
          @FKRegionPKID , -- FKRegionPKID - int
          GETDATE() , -- CreateDateTime - datetime
          GETDATE()  -- UpdateDateTime - datetime
        )";
            if (citys != null)
            {
                foreach (var item in citys)
                {
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@FKHomePageModuleHelperID", moduleHelperID);
                    p.Add("@FKRegionPKID", item.FKRegionPKID);
                    connection.Execute(sqlCityString, p, trans);
                }
            }
            return true;
        }

        /// <summary>
        /// 更新附属模块
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        /// <param name="citys"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool UpdateHomePageModuleHelper(SqlConnection connection, SE_HomePageModuleHelperConfig model, IEnumerable<SE_ModuleHelperCityConfig> citys, int pkid)
        {
            var trans = connection.BeginTransaction();
            try
            {
                //更新附属模块信息
                var sqlString = new StringBuilder();
                sqlString.Append(" UPDATE Configuration.dbo.SE_HomePageModuleHelperConfig SET  ");
                sqlString.Append("NoticeChannel=@NoticeChannel, IsNewUser=@IsNewUser, Margin=@Margin,UriCount=@UriCount, Pattern=@Pattern, ModuleName=@ModuleName, IsEnabled=@IsEnabled, SpliteLine=@SpliteLine,BgImageUrl=@BgImageUrl,FontColor=@FontColor,StartVersion=@StartVersion,EndVersion=@EndVersion,Title=@Title,TitleImageUrl=@TitleImageUrl,TitleColor=@TitleColor,TitleBgColor=@TitleBgColor,IsMore=@IsMore,MoreUri=@MoreUri,IsTag=@IsTag,TagContent=@TagContent,Channel=@Channel,UpdateDateTime=GETDATE(),BgColor=@BgColor,FileUrl=@FileUrl,ImageType=@ImageType ");
                sqlString.Append(" where ID=@ID ");
                var i = connection.Execute(sqlString.ToString(), model, trans);
                if (i <= 0)
                {
                    throw new Exception("更新附属模块信息");
                }

                // 添加模块城市数据
                const string sqlCityString = @"INSERT INTO Configuration.dbo.SE_ModuleHelperCityConfig
                                        ( FKHomePageModuleHelperID ,
                                          FKRegionPKID ,
                                          CreateDateTime ,
                                          UpdateDateTime
                                        )
                                        VALUES  ( @FKHomePageModuleHelperID , -- FKHomePageModuleHelperID - int
                                          @FKRegionPKID , -- FKRegionPKID - int
                                          GETDATE() , -- CreateDateTime - datetime
                                          GETDATE()  -- UpdateDateTime - datetime
                                        )";
                if (citys != null)
                {
                    var p = new DynamicParameters();
                    p.Add("@FKHomePageModuleHelperID", model.ID);
                    connection.Execute("DELETE FROM Configuration.dbo.SE_ModuleHelperCityConfig WHERE FKHomePageModuleHelperID=@FKHomePageModuleHelperID", p, trans);
                    foreach (var item in citys)
                    {
                        connection.Execute(sqlCityString, item, trans);
                    }
                }
                else
                {
                    var p = new DynamicParameters();
                    p.Add("@FKHomePageModuleHelperID", model.ID);
                    connection.Execute("DELETE FROM Configuration.dbo.SE_ModuleHelperCityConfig WHERE FKHomePageModuleHelperID=@FKHomePageModuleHelperID", p, trans);
                }

                // 更新排序后，主动更新配置表的时间
                connection.Execute(" UPDATE Configuration.dbo.SE_HomePageConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID", new SE_HomePageConfig { ID = pkid }, trans);
                connection.Execute(" UPDATE Configuration.dbo.SE_HomePageModuleConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID", new SE_HomePageModuleConfig { ID = model.FKHomePageModuleID.Value }, trans);

                trans.Commit();
                return true;

            }
            catch (Exception em)
            {
                trans.Rollback();
                throw new Exception(em.Message);
            }
        }

        public bool UpdateHomePageModuleHelperPattern(SqlConnection connection, SE_HomePageModuleHelperConfig model)
        {
            try
            {
                StringBuilder sqlString = new StringBuilder();
                sqlString.Append(" UPDATE Configuration.dbo.SE_HomePageModuleHelperConfig SET  ");
                sqlString.Append("NoticeChannel=@NoticeChannel, IsNewUser=@IsNewUser, Margin=@Margin,UriCount=@UriCount, Pattern=@Pattern, ModuleName=@ModuleName, IsEnabled=@IsEnabled, SpliteLine=@SpliteLine,BgImageUrl=@BgImageUrl,FontColor=@FontColor,StartVersion=@StartVersion,EndVersion=@EndVersion,Title=@Title,TitleImageUrl=@TitleImageUrl,TitleColor=@TitleColor,TitleBgColor=@TitleBgColor,IsMore=@IsMore,MoreUri=@MoreUri,IsTag=@IsTag,TagContent=@TagContent,Channel=@Channel,UpdateDateTime=GETDATE(),BgColor=@BgColor ");
                sqlString.Append(" where ID=@ID ");
                int i = connection.Execute(sqlString.ToString(), model);
                if (i <= 0)
                {
                    throw new Exception("更新附属模块信息");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception em)
            {
                throw new Exception(em.Message);
                return false;
            }
        }

        /// <summary>
        /// 更新模块图片
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateHomePageModuleHelperBgImage(SqlConnection connection, SE_HomePageModuleHelperConfig model)
        {

            var result = connection.Execute("UPDATE Configuration.dbo.SE_HomePageModuleHelperConfig SET BgImageUrl=@BgImageUrl,UpdateDateTime = GETDATE()  where ID=@ID ", model);
            if (model.FKHomePageModuleID.HasValue)
            {
                var module = GetHomePageModuleEntity(connection, model.FKHomePageModuleID.Value);
                if (module.ID > 0)
                {
                    result = connection.Execute(" UPDATE Configuration.dbo.SE_HomePageConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID", new SE_HomePageConfig { ID = module.FKHomePage.Value });
                    result = connection.Execute(" UPDATE Configuration.dbo.SE_HomePageModuleConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID", new SE_HomePageModuleConfig { ID = module.ID });
                }
            }
            return result > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="moduleHelperID"></param>
        /// <returns></returns>
        public bool DeleteHomePageModuleHelper(SqlConnection connection, int moduleHelperID)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@ID", moduleHelperID);
            return connection.Execute("DELETE FROM Configuration.dbo.SE_HomePageModuleHelperConfig WHERE ID=@ID", p) > 0;
        }


        public IEnumerable<SE_HomePageModuleHelperConfig> GetHomeModuleHelperList(SqlConnection connection, int moduleID)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@FKHomePageModuleID", moduleID);
            return connection.Query<SE_HomePageModuleHelperConfig>("SELECT * FROM Configuration.dbo.SE_HomePageModuleHelperConfig (NOLOCK) WHERE FKHomePageModuleID=@FKHomePageModuleID", p);
        }

        /// <summary>
        /// 查询模块下的附属模块
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public SE_HomePageModuleHelperConfig GetHomeModuleHelperEntity(SqlConnection connection, int id)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@ID", id);
            return connection.Query<SE_HomePageModuleHelperConfig>("SELECT * FROM Configuration.dbo.SE_HomePageModuleHelperConfig (NOLOCK) where ID=@ID ", p).FirstOrDefault();
        }

        public IEnumerable<SE_ModuleHelperCityConfig> GetModuleHelperCityEntityList(SqlConnection connection, int moduleHelperID)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@FKHomePageModuleHelperID", moduleHelperID);
            return connection.Query<SE_ModuleHelperCityConfig>("SELECT MHC.*,R.ParentID FROM Configuration.dbo.SE_ModuleHelperCityConfig (NOLOCK) MHC LEFT JOIN Gungnir.dbo.tbl_region(NOLOCK) R ON MHC.FKRegionPKID = R.PKID WHERE MHC.FKHomePageModuleHelperID=@FKHomePageModuleHelperID", p);
        }



        #endregion



        #region 获取城市信息
        public IEnumerable<string> GetCityGroupProvince(SqlConnection connection, string pkids)
        {
            string sql = "SELECT DISTINCT ParentID FROM Gungnir.dbo.tbl_region (NOLOCK) WHERE PKID IN (@pkids) GROUP BY ParentID HAVING ParentID>0";
            DynamicParameters dp = new DynamicParameters();
            return connection.Query<string>(sql, dp);
        }
        #endregion






        /// <summary>
        /// 查询当前模块的总数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="fkHome"></param>
        /// <returns></returns>
        public int SelectHomePageModulePriorityLevel(SqlConnection connection, int fkHome)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@FKHomePage", fkHome);
            return Convert.ToInt32(connection.ExecuteScalar("SELECT COUNT(1) FROM Configuration.dbo.SE_HomePageModuleConfig WHERE FKHomePage=@FKHomePage", parameters));
        }



        /// <summary>
        /// 删除首页配置
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteHomePage(SqlConnection connection, int id)
        {
            return connection.Execute(" delete from Configuration.dbo.SE_HomePageConfig where ID=@ID ", new SE_HomePageConfig() { ID = id }) > 0;
        }

        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteHomePageModule(SqlConnection connection, int id, int pkid)
        {
            // 更新排序后，主动更新配置表的时间
            connection.Execute(" UPDATE Configuration.dbo.SE_HomePageConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID", new SE_HomePageConfig { ID = pkid });
            return connection.Execute(" delete from Configuration.dbo.SE_HomePageModuleConfig where ID=@ID ", new SE_HomePageModuleConfig { ID = id }) > 0;
        }



        /// <summary>
        /// 获取首页配置列表
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public IEnumerable<SE_HomePageConfig> GetPersonlCenterConfig(SqlConnection connection)
        {
            return connection.Query<SE_HomePageConfig>("SELECT * FROM Configuration.dbo.SE_HomePageConfig WITH(NOLOCK) where  TypeConfig=2");
        }

        /// <summary>
        /// 获取首页配置列表
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public IEnumerable<SE_HomePageConfig> GetHomePageConfig(SqlConnection connection)
        {
            return connection.Query<SE_HomePageConfig>("SELECT * FROM Configuration.dbo.SE_HomePageConfig WITH(NOLOCK) where  TypeConfig=1");
        }


        public IEnumerable<SE_HomePageConfig> GetPersonalCenterConfig(SqlConnection connection)
        {
            return connection.Query<SE_HomePageConfig>("SELECT * FROM Configuration.dbo.SE_HomePageConfig WITH(NOLOCK) where  TypeConfig=2");
        }

        /// <summary>
        /// 获取模块列表
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="id">首页ID</param>
        /// <returns></returns>
        public IEnumerable<SE_HomePageModuleConfig> GetHomePageModuleConfig(SqlConnection connection, int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@FKHomePage", id);
            return connection.Query<SE_HomePageModuleConfig>("SELECT * FROM Configuration.dbo.SE_HomePageModuleConfig WITH(NOLOCK) where FKHomePage=@FKHomePage ", parameters);
        }

        /// <summary>
        /// 获取模块信息
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="moduleID">模块ID</param>
        /// <returns></returns>
        public SE_HomePageModuleConfig GetHomePageModuleEntity(SqlConnection connection, int moduleID)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ID", moduleID);
            return connection.Query<SE_HomePageModuleConfig>("SELECT * FROM Configuration.dbo.SE_HomePageModuleConfig WITH(NOLOCK) where ID=@ID ", parameters).FirstOrDefault();
        }

        /// <summary>
        /// 获取模块下的子模块列表
        /// </summary>
        /// <param name="connectin"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public IEnumerable<SE_HomePageModuleConfig> GetHomePageModuleChildList(SqlConnection connectin, int parentID)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@PraentID", parentID);
            return connectin.Query<SE_HomePageModuleConfig>("SELECT *  FROM Configuration.dbo.SE_HomePageModuleConfig WHERE PraentID=@PraentID ", p);
        }


        public IEnumerable<Region> GetRegionList(SqlConnection connection, string pkids)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@PK", pkids);
            return connection.Query<Region>("SELECT * FROM Gungnir.dbo.tbl_region (NOLOCK) WHERE PKID IN (@PK)", p);
        }



        public IEnumerable<SE_WapParameterConfig> GetWapParameterList(SqlConnection connection)
        {
            return connection.Query<SE_WapParameterConfig>("SELECT * FROM Configuration.dbo.SE_WapParameterConfig WITH(NOLOCK) ORDER BY createdatetime DESC");
        }

        /// <summary>
        /// 添加参数对应关系
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddWapParameter(SqlConnection connection, SE_WapParameterConfig model)
        {
            string sql = @"INSERT INTO Configuration.dbo.SE_WapParameterConfig
        ( Name ,
          KeyName ,
          Android ,
          IOS ,
          CreateDateTime
        )
VALUES  ( @Name , -- Name - nvarchar(50)
          @KeyName , -- KeyName - nvarchar(50)
          @Android , -- Android - nvarchar(2000)
          @IOS , -- IOS - nvarchar(2000)
          GETDATE()  -- CreateDateTime - datetime
        )";
            return connection.Execute(sql, model) > 0;
        }

        public bool DeleteWapParameter(SqlConnection connection, int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ID", id);
            return connection.Execute("DELETE FROM Configuration.dbo.SE_WapParameterConfig WHERE ID=@ID", parameters) > 0;
        }

        public IEnumerable<Tuhu.Provisioning.DataAccess.Mapping.HomePageCarActivityRegionEntity> GetHomePageCarActivityCity(SqlConnection sqlconnection, int fkpkid)
        {
            string sql = @"SELECT  CAR.* ,
        R.ParentID
        FROM    Configuration.dbo.HomePageCarActivityRegion(NOLOCK) CAR
                LEFT JOIN Gungnir.dbo.tbl_region(NOLOCK) R ON CAR.RegionID = R.PKID
        WHERE   CAR.FKCarActviityPKID = @FKCarActviityPKID;";
            var p = new DynamicParameters();
            p.Add("@FKCarActviityPKID", fkpkid);
            return sqlconnection.Query<Tuhu.Provisioning.DataAccess.Mapping.HomePageCarActivityRegionEntity>(sql, p);
        }


        #region 闪屏配置
        public bool AddFlashScreenEntity(SqlConnection connection, SE_FlashScreenConfig model)
        {
            string sqlString = @"INSERT INTO Configuration.dbo.SE_FlashScreenConfig
                                    ( StartVersion ,
                                      EndVersion ,
                                      Name ,
                                      PriorityLevel ,
                                      Channel ,
                                      ButtonImage ,
                                      BannerImage ,
                                      LinkUrl ,
                                      Counts ,
                                      IsEnabled ,
                                      StartDateTime ,
                                      EndDateTime ,
                                      CreateDateTime ,
                                      UpdateDateTime,Zip,Md5,APPChannel,Type,NewNoticeChannel
                                    )
                            VALUES  ( @StartVersion , -- StartVersion - nvarchar(50)
                                      @EndVersion , -- EndVersion - nvarchar(50)
                                      @Name , -- Name - nvarchar(100)
                                      @PriorityLevel , -- PriorityLevel - int
                                      @Channel , -- Channel - nvarchar(50)
                                      @ButtonImage , -- ButtonImage - nvarchar(1000)
                                      @BannerImage , -- BannerImage - nvarchar(1000)
                                      @LinkUrl , -- AppUrl - nvarchar(500)
                                      @Counts , -- Counts - nvarchar(50)
                                      @IsEnabled , -- IsEnabled - bit
                                     @StartDateTime , -- StartDateTime - datetime
                                      @EndDateTime , -- EndDateTime - datetime
                                      GETDATE() , -- CreateDateTime - datetime
                                      GETDATE(),@Zip,@Md5,@APPChannel,@Type,@NewNoticeChannel  -- UpdateDateTime - datetime
                                    )";

            return connection.Execute(sqlString, model) > 0;
        }

        public IEnumerable<SE_FlashScreenConfig> GetFlashScreenList(SqlConnection connection, int type = 1)
        {
            var para = new DynamicParameters();
            para.Add("@Type", type);
            if (type == 2)
            {
                return connection.Query<SE_FlashScreenConfig>("SELECT * FROM Configuration.dbo.SE_FlashScreenConfig WITH(NOLOCK) where [Type]=@Type  ORDER BY CreateDateTime DESC", para);
            }
            else
                return connection.Query<SE_FlashScreenConfig>("SELECT * FROM Configuration.dbo.SE_FlashScreenConfig WITH(NOLOCK) where [Type]=1 or [Type] is null or [Type]=''  ORDER BY CreateDateTime DESC", para);

        }

        public bool UpdateFlashScreenEntity(SqlConnection connection, SE_FlashScreenConfig model)
        {
            string sqlString = @"UPDATE Configuration.dbo.SE_FlashScreenConfig SET [Type]=@Type, APPChannel=@APPChannel, NewNoticeChannel=@NewNoticeChannel, Zip=@Zip,Md5=@Md5, StartVersion=@StartVersion, EndVersion=@EndVersion, Name=@Name, PriorityLevel=@PriorityLevel, Channel=@Channel, ButtonImage=@ButtonImage,BannerImage=@BannerImage,LinkUrl=@LinkUrl,Counts=@Counts,IsEnabled=@IsEnabled,StartDateTime=@StartDateTime,EndDateTime=@EndDateTime,UpdateDateTime=@UpdateDateTime where ID=@ID";
            return connection.Execute(sqlString, model) > 0;
        }

        public bool DeleteFlashScreen(SqlConnection connection, int id)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@ID", id);
            return connection.Execute("DELETE FROM Configuration.dbo.SE_FlashScreenConfig WHERE ID=@ID", p) > 0;
        }



        public SE_FlashScreenConfig GetFlashScreenEntity(SqlConnection connection, int id)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@ID", id);
            return connection.Query<SE_FlashScreenConfig>("SELECT * FROM Configuration.dbo.SE_FlashScreenConfig WITH(NOLOCK) where ID=@ID ", p).FirstOrDefault();
        }

        #endregion


        #region 首页内容配置
        /// <summary>
        /// 获取模块列表内容
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="module"></param>
        /// <param name="moduleHelper"></param>
        /// <returns></returns>
        public IEnumerable<SE_HomePageModuleContentConfig> GetHomePageContentList(SqlConnection connection, int? module, int? moduleHelper)
        {
            if (module != null)
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@FKHomePageModuleID", module);
                return connection.Query<SE_HomePageModuleContentConfig>("SELECT * FROM  Configuration.dbo.SE_HomePageModuleContentConfig (NOLOCK) WHERE FKHomePageModuleID=@FKHomePageModuleID ORDER BY PriorityLevel ASC", dp);
            }
            else if (moduleHelper != null)
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@FKHomePageModuleHelperID", moduleHelper);
                return connection.Query<SE_HomePageModuleContentConfig>("SELECT * FROM  Configuration.dbo.SE_HomePageModuleContentConfig (NOLOCK) WHERE FKHomePageModuleHelperID=@FKHomePageModuleHelperID ORDER BY PriorityLevel ASC", dp);
            }
            else
                return null;
        }
        /// <summary>
        /// 获取模块列表内容
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="module"></param>
        /// <param name="moduleHelper"></param>
        /// <returns></returns>
        public IEnumerable<SE_HomePageModuleContentConfig> GetHomePageContentListV2(SqlConnection connection, int? module, bool isMoreCity, int? moduleHelper)
        {
            if (module != null)
            {
                var sql = @"SELECT * FROM  Configuration.dbo.SE_HomePageModuleContentConfig (NOLOCK) WHERE FKHomePageModuleID=@FKHomePageModuleID";
                if (isMoreCity)
                {
                    sql += " AND FKHomePageModuleHelperID IS NULL OR FKHomePageModuleHelperID = ''";
                }
                sql += " ORDER BY PriorityLevel ASC";
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@FKHomePageModuleID", module);
                return connection.Query<SE_HomePageModuleContentConfig>(sql, dp);
            }
            else if (moduleHelper != null)
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@FKHomePageModuleHelperID", moduleHelper);
                return connection.Query<SE_HomePageModuleContentConfig>("SELECT * FROM  Configuration.dbo.SE_HomePageModuleContentConfig (NOLOCK) WHERE FKHomePageModuleHelperID=@FKHomePageModuleHelperID ORDER BY PriorityLevel ASC", dp);
            }
            else
                return null;
        }

        /// <summary>
        /// 添加首页模块内容
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        /// <param name="pkid"></param>
        public int AddHomePageContent(SqlConnection connection, SE_HomePageModuleContentConfig model, int pkid, List<SE_HomePageModuleTagConfig> tags = null)
        {
            string sql = @"
                     DECLARE @InseredTable TABLE (
                    ID INT 
                    ) ;

                    INSERT INTO Configuration.dbo.SE_HomePageModuleContentConfig
                                ( FKHomePageModuleID ,
                                  FKHomePageModuleHelperID ,
                                  StartVersion ,
                                  EndVersion ,
                                  Title ,
                                  DeviceType ,
                                  PriorityLevel ,
                                  ButtonImageUrl ,
                                  BannerImageUrl ,
                                  LinkUrl ,
                                  UriCount ,
                                  IsEnabled ,
                                  StartDateTime ,
                                  EndDateTime ,
                                  CreateDateTime ,
                                  UpdateDateTime ,
                                  Width ,
                                  Height ,
                                  UpperLeftX ,
                                  UpperLeftY ,
                                  LowerRightX ,
                                  LowerRightY,
                                  BigTitle,BigTilteColor,SmallTitle,SmallTitleColor,PromoteTitle,PromoteTitleColor,PromoteTitleBgColor,AnimationStyle,UserRank,VIPRank,PeopleTip,NoticeChannel,NewNoticeChannel,
                                  DeviceBrand,
                                  DeviceTypes
                                )
                          OUTPUT Inserted.ID INTO @InseredTable
                        VALUES  ( @FKHomePageModuleID , -- FKHomePageModuleID - int
                                  @FKHomePageModuleHelperID , -- FKHomePageModuleHelperID - int
                                  @StartVersion , -- StartVersion - nvarchar(20)
                                  @EndVersion , -- EndVersion - nvarchar(20)
                                  @Title , -- Title - nvarchar(50)
                                  @DeviceType , -- DeviceType - int
                                  @PriorityLevel , -- PriorityLevel - int
                                  @ButtonImageUrl , -- ButtonImageUrl - nvarchar(500)
                                  @BannerImageUrl , -- BannerImageUrl - nvarchar(500)
                                  @LinkUrl , -- LinkUrl - nvarchar(1000)
                                  @UriCount , -- UriCount - nvarchar(500)
                                  @IsEnabled , -- IsEnabled - bit
                                  @StartDateTime , -- StartDateTime - datetime
                                  @EndDateTime , -- EndDateTime - datetime
                                  GETDATE() , -- CreateDateTime - datetime
                                  GETDATE() , -- UpdateDateTime - datetime
                                  @Width , -- Width - int
                                  @Height , -- Height - int
                                  @UpperLeftX , -- UpperLeftX - int
                                  @UpperLeftY , -- UpperLeftY - int
                                  @LowerRightX , -- LowerRightX - int
                                  @LowerRightY,  -- LowerRightY - int
                                   @BigTitle,@BigTilteColor,@SmallTitle,@SmallTitleColor,@PromoteTitle,@PromoteTitleColor,@PromoteTitleBgColor,@AnimationStyle,@UserRank,@VIPRank,@PeopleTip,@NoticeChannel,@NewNoticeChannel,@DeviceBrand,@DeviceTypes
                                ) 
                   SELECT TOP 1 @PKID=ID FROM @InseredTable	
                             ";

            DynamicParameters dp = new DynamicParameters();
            dp.Add("@FKHomePageModuleID", model.FKHomePageModuleID);
            dp.Add("@FKHomePageModuleHelperID", model.FKHomePageModuleHelperID);
            dp.Add("@StartVersion", model.StartVersion);
            dp.Add("@EndVersion", model.EndVersion);
            dp.Add("@Title", model.Title);
            dp.Add("@DeviceType", model.DeviceType);
            dp.Add("@PriorityLevel", model.PriorityLevel);
            dp.Add("@ButtonImageUrl", model.ButtonImageUrl);
            dp.Add("@BannerImageUrl", model.BannerImageUrl);
            dp.Add("@LinkUrl", model.LinkUrl);
            dp.Add("@UriCount", model.UriCount);
            dp.Add("@IsEnabled", model.IsEnabled);
            dp.Add("@StartDateTime", model.StartDateTime);
            dp.Add("@EndDateTime", model.EndDateTime);
            dp.Add("@Width", model.Width);
            dp.Add("@Height", model.Height);
            dp.Add("@UpperLeftX", model.UpperLeftX);
            dp.Add("@UpperLeftY", model.UpperLeftY);
            dp.Add("@LowerRightX", model.LowerRightX);
            dp.Add("@LowerRightY", model.LowerRightY);
            dp.Add("@BigTitle", model.BigTitle);
            dp.Add("@BigTilteColor", model.BigTilteColor);
            dp.Add("@SmallTitle", model.SmallTitle);
            dp.Add("@SmallTitleColor", model.SmallTitleColor);
            dp.Add("@PromoteTitle", model.PromoteTitle);
            dp.Add("@PromoteTitleColor", model.PromoteTitleColor);
            dp.Add("@PromoteTitleBgColor", model.PromoteTitleBgColor);
            dp.Add("@AnimationStyle", model.AnimationStyle);
            dp.Add("@UserRank", model.UserRank);
            dp.Add("@VIPRank", model.VIPRank);
            dp.Add("@PeopleTip", model.PeopleTip);
            dp.Add("@NoticeChannel", model.NoticeChannel);
            dp.Add("@NewNoticeChannel", model.NewNoticeChannel);
            dp.Add("@DeviceBrand", model.DeviceBrand);
            dp.Add("@DeviceTypes", model.DeviceTypes);
            dp.Add("@PKID", null, DbType.Int32, ParameterDirection.Output, null);

            var n = connection.Execute(sql, dp);

            model.ID = dp.Get<int>("@PKID");

            if (n <= 0)
                return -1;

            //添加模块标签数据
            const string sqlTagsString = @"INSERT INTO Configuration.dbo.SE_HomePageModuleTagConfig
                                    ( FKHomePageModuleContentID ,
                                      FKTagIDs
                                    )
                                    VALUES  ( @FKHomePageModuleContentID ,
                                      @FKTagIDs
                                    )";
            if (tags != null)
            {
                foreach (var item in tags)
                {
                    var p = new DynamicParameters();
                    p.Add("@FKHomePageModuleContentID", model.ID);
                    p.Add("@FKTagIDs", item.FKTagIDs);
                    connection.Execute(sqlTagsString, p);
                }
            }

            // 主动更新配置表的时间
            if (model.FKHomePageModuleID != null)
                connection.Execute(" UPDATE Configuration.dbo.SE_HomePageModuleConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID", new SE_HomePageModuleConfig { ID = model.FKHomePageModuleID.Value });
            if (model.FKHomePageModuleHelperID != null)
                connection.Execute(" UPDATE Configuration.dbo.SE_HomePageModuleHelperConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID", new SE_HomePageModuleHelperConfig { ID = model.FKHomePageModuleHelperID.Value });
            connection.Execute(" UPDATE Configuration.dbo.SE_HomePageConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID", new SE_HomePageConfig { ID = pkid });
            return dp.Get<int>("@PKID");

        }

        /// <summary>
        /// 添加模块内容数据
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="trans"></param>
        /// <param name="model"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public int AddHomePageContent(SqlConnection connection, SqlTransaction trans, SE_HomePageModuleContentConfig model, int pkid)
        {
            const string sql = @"
                     DECLARE @InseredTable TABLE (
                         ID INT 
                     ) ;

                    INSERT INTO Configuration.dbo.SE_HomePageModuleContentConfig
                                ( FKHomePageModuleID ,
                                  FKHomePageModuleHelperID ,
                                  StartVersion ,
                                  EndVersion ,
                                  Title ,
                                  DeviceType ,
                                  PriorityLevel ,
                                  ButtonImageUrl ,
                                  BannerImageUrl ,
                                  LinkUrl ,
                                  UriCount ,
                                  IsEnabled ,
                                  StartDateTime ,
                                  EndDateTime ,
                                  CreateDateTime ,
                                  UpdateDateTime ,
                                  Width ,
                                  Height ,
                                  UpperLeftX ,
                                  UpperLeftY ,
                                  LowerRightX ,
                                  LowerRightY,
                                  BigTitle,BigTilteColor,SmallTitle,SmallTitleColor,PromoteTitle,PromoteTitleColor,PromoteTitleBgColor,AnimationStyle,UserRank,VIPRank,PeopleTip,NoticeChannel,NewNoticeChannel
                                )
                          OUTPUT Inserted.ID INTO @InseredTable
                        VALUES  ( @FKHomePageModuleID , -- FKHomePageModuleID - int
                                  @FKHomePageModuleHelperID , -- FKHomePageModuleHelperID - int
                                  @StartVersion , -- StartVersion - nvarchar(20)
                                  @EndVersion , -- EndVersion - nvarchar(20)
                                  @Title , -- Title - nvarchar(50)
                                  @DeviceType , -- DeviceType - int
                                  @PriorityLevel , -- PriorityLevel - int
                                  @ButtonImageUrl , -- ButtonImageUrl - nvarchar(500)
                                  @BannerImageUrl , -- BannerImageUrl - nvarchar(500)
                                  @LinkUrl , -- LinkUrl - nvarchar(1000)
                                  @UriCount , -- UriCount - nvarchar(500)
                                  @IsEnabled , -- IsEnabled - bit
                                  @StartDateTime , -- StartDateTime - datetime
                                  @EndDateTime , -- EndDateTime - datetime
                                  GETDATE() , -- CreateDateTime - datetime
                                  GETDATE() , -- UpdateDateTime - datetime
                                  @Width , -- Width - int
                                  @Height , -- Height - int
                                  @UpperLeftX , -- UpperLeftX - int
                                  @UpperLeftY , -- UpperLeftY - int
                                  @LowerRightX , -- LowerRightX - int
                                  @LowerRightY,  -- LowerRightY - int
                                   @BigTitle,@BigTilteColor,@SmallTitle,@SmallTitleColor,@PromoteTitle,@PromoteTitleColor,@PromoteTitleBgColor,@AnimationStyle,@UserRank,@VIPRank,@PeopleTip,@NoticeChannel,@NewNoticeChannel
                                ) 
                   SELECT TOP 1 @PKID=ID FROM @InseredTable	
                             ";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@FKHomePageModuleID", model.FKHomePageModuleID);
            dp.Add("@FKHomePageModuleHelperID", model.FKHomePageModuleHelperID);
            dp.Add("@StartVersion", model.StartVersion);
            dp.Add("@EndVersion", model.EndVersion);
            dp.Add("@Title", model.Title);
            dp.Add("@DeviceType", model.DeviceType);
            dp.Add("@PriorityLevel", model.PriorityLevel);
            dp.Add("@ButtonImageUrl", model.ButtonImageUrl);
            dp.Add("@BannerImageUrl", model.BannerImageUrl);
            dp.Add("@LinkUrl", model.LinkUrl);
            dp.Add("@UriCount", model.UriCount);
            dp.Add("@IsEnabled", model.IsEnabled);
            dp.Add("@StartDateTime", model.StartDateTime);
            dp.Add("@EndDateTime", model.EndDateTime);
            dp.Add("@Width", model.Width);
            dp.Add("@Height", model.Height);
            dp.Add("@UpperLeftX", model.UpperLeftX);
            dp.Add("@UpperLeftY", model.UpperLeftY);
            dp.Add("@LowerRightX", model.LowerRightX);
            dp.Add("@LowerRightY", model.LowerRightY);
            dp.Add("@BigTitle", model.BigTitle);
            dp.Add("@BigTilteColor", model.BigTilteColor);
            dp.Add("@SmallTitle", model.SmallTitle);
            dp.Add("@SmallTitleColor", model.SmallTitleColor);
            dp.Add("@PromoteTitle", model.PromoteTitle);
            dp.Add("@PromoteTitleColor", model.PromoteTitleColor);
            dp.Add("@PromoteTitleBgColor", model.PromoteTitleBgColor);
            dp.Add("@AnimationStyle", model.AnimationStyle);
            dp.Add("@UserRank", model.UserRank);
            dp.Add("@VIPRank", model.VIPRank);
            dp.Add("@PeopleTip", model.PeopleTip);
            dp.Add("@NoticeChannel", model.NoticeChannel);
            dp.Add("@NewNoticeChannel", model.NewNoticeChannel);
            dp.Add("@PKID", null, DbType.Int32, ParameterDirection.Output, null);

            var n = connection.Execute(sql, dp, trans);
            if (n > 0)
            {
                // 更新排序后，主动更新配置表的时间
                connection.Execute(" UPDATE Configuration.dbo.SE_HomePageConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID", new SE_HomePageConfig { ID = pkid }, trans);

                return dp.Get<int>("@PKID");
            }

            return -1;
        }


        /// <summary>
        /// 更新模块内容
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="model"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool UpdateHomePageContent(SqlConnection connection, SE_HomePageModuleContentConfig model, int pkid, List<SE_HomePageModuleTagConfig> tags = null)
        {
            var result = connection.Execute("UPDATE Configuration.dbo.SE_HomePageModuleContentConfig SET NewNoticeChannel=@NewNoticeChannel,  PeopleTip=@PeopleTip,NoticeChannel=@NoticeChannel,  BigTitle=@BigTitle,BigTilteColor=@BigTilteColor,SmallTitle=@SmallTitle,SmallTitleColor=@SmallTitleColor,PromoteTitle=@PromoteTitle,PromoteTitleColor=@PromoteTitleColor,PromoteTitleBgColor=@PromoteTitleBgColor,AnimationStyle=@AnimationStyle,UserRank=@UserRank,VIPRank=@VIPRank,  StartVersion = @StartVersion, EndVersion=@EndVersion, Title=@Title,DeviceType=@DeviceType,PriorityLevel=@PriorityLevel,ButtonImageUrl=@ButtonImageUrl,BannerImageUrl=@BannerImageUrl,LinkUrl=@LinkUrl,UriCount=@UriCount,IsEnabled=@IsEnabled,StartDateTime=@StartDateTime,EndDateTime=@EndDateTime,UpdateDateTime =@UpdateDateTime,DeviceBrand=@DeviceBrand,DeviceTypes=@DeviceTypes where ID=@ID ", model);
            if (result > 0)
            {
                var p = new DynamicParameters();
                p.Add("@FKHomePageModuleContentID", model.ID);
                connection.Execute("DELETE FROM Configuration.dbo.SE_HomePageModuleTagConfig WHERE FKHomePageModuleContentID=@FKHomePageModuleContentID", p);

                if (tags != null)
                {
                    //添加模块标签数据
                    const string sqlTagsString = @"INSERT INTO Configuration.dbo.SE_HomePageModuleTagConfig
                                    ( FKHomePageModuleContentID ,
                                      FKTagIDs
                                    )
                                    VALUES  ( @FKHomePageModuleContentID ,
                                      @FKTagIDs
                                    )";
                    foreach (var item in tags)
                    {
                        p.Add("@FKTagIDs", item.FKTagIDs);
                        connection.Execute(sqlTagsString, p);
                    }
                }

                if (model.FKHomePageModuleID > 0)
                {
                    connection.Execute(
                        "UPDATE Configuration..SE_HomePageModuleConfig SET UpdateDateTime = GETDATE() WHERE ID = @ID",
                        new SE_HomePageModuleConfig { ID = model.FKHomePageModuleID.Value });
                }
                if (model.FKHomePageModuleHelperID > 0)
                {
                    connection.Execute(
                        "UPDATE Configuration..SE_HomePageModuleHelperConfig SET UpdateDateTime = GETDATE() WHERE ID = @ID",
                        new SE_HomePageModuleHelperConfig { ID = model.FKHomePageModuleHelperID.Value });
                }
                // 更新排序后，主动更新配置表的时间
                connection.Execute(" UPDATE Configuration.dbo.SE_HomePageConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID", new SE_HomePageConfig { ID = pkid });
            }
            return result > 0;
        }

        /// <summary>
        /// 删除模块内容
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteHomePageContent(SqlConnection connection, SE_HomePageModuleContentConfig model, SE_HomePageModuleConfig pageModel)
        {

            // 删除内容后，主动更新配置表的时间
            if (pageModel.FKHomePage.HasValue)
                connection.Execute(" UPDATE Configuration.dbo.SE_HomePageConfig SET UpdateDateTime = GETDATE()  WHERE ID = @ID", new SE_HomePageConfig { ID = pageModel.FKHomePage.Value });
            if (model.FKHomePageModuleID.HasValue)
                connection.Execute("UPDATE Configuration..SE_HomePageModuleConfig SET UpdateDateTime = GETDATE() WHERE ID = @ID", new SE_HomePageModuleConfig { ID = model.FKHomePageModuleID.Value });
            if (model.FKHomePageModuleHelperID.HasValue)
                connection.Execute("UPDATE Configuration..SE_HomePageModuleHelperConfig SET UpdateDateTime = GETDATE() WHERE ID = @ID", new SE_HomePageModuleHelperConfig { ID = model.FKHomePageModuleHelperID.Value });

            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ID", model.ID);
            return connection.Execute("DELETE FROM Configuration.dbo.SE_HomePageModuleContentConfig WHERE ID=@ID ", dp) > 0;

        }


        public SE_HomePageModuleContentConfig GetHomePageModuleContentEntity(SqlConnection connection, int id)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ID", id);
            return connection.Query<SE_HomePageModuleContentConfig>("SELECT * FROM Configuration.dbo.SE_HomePageModuleContentConfig (NOLOCK) WHERE ID=@ID ", dp).FirstOrDefault();
        }
        public List<SE_HomePageModuleHelperConfig> GetSE_HomePageModuleHelperConfigsByFkHomePageId(SqlConnection connection, int fkid)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@FkId", fkid);
            return connection.Query<SE_HomePageModuleHelperConfig>("SELECT * FROM Configuration..SE_HomePageModuleHelperConfig WITH ( NOLOCK)  WHERE FKHomePageModuleID=@FkId", dp).ToList();
        }

        public List<DeviceBrandModel> SelectDeviceBrands(SqlConnection connection)
        {
            return connection.Query<DeviceBrandModel>(" SELECT * FROM Configuration..tbl_DeviceBrand WITH(NOLOCK) ").ToList();
        }
        public List<DeviceTypeModel> SelectDeviceTypes(SqlConnection connection, int brandId)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@BrandID", brandId);
            return connection.Query<DeviceTypeModel>(" SELECT * FROM Configuration..tbl_DeviceType WITH(NOLOCK)  where BrandID=@BrandID", dp).ToList();
        }
        #endregion


        #region 瀑布流
        public bool AddFlow(SqlConnection connection, SE_HomePageFlowConfig model)
        {
            string sql = @"INSERT INTO Configuration.dbo.SE_HomePageFlowConfig
        ( Title ,
          BgImageUrl ,
          StartVersion ,
          EndVersion ,
          Channel ,
          LinkUrl ,
          MPLinkUrl ,
          WebLinkUrl ,
          UriCount ,
          IsEnabled ,
          IsCountDown ,
          StartDateTime ,
          EndDateTime ,
          CreateDateTime ,
          UpdateDateTime ,
          IsChildProduct,
          PriorityLevel,
          [Type],ParentPKID,BigTitle,BigTilteColor,SmallTitle,SmallTitleColor,APPChannel,SmallBgImage
        )
VALUES  ( @Title, -- Title - nvarchar(50)
          @BgImageUrl , -- BgImageUrl - nvarchar(1000)
          @StartVersion , -- StartVersion - nvarchar(50)
          @EndVersion , -- EndVersion - nvarchar(50)
          @Channel , -- Channel - int
          @LinkUrl , -- LinkUrl - nvarchar(1000)
          @MPLinkUrl ,
          @WebLinkUrl ,
          @UriCount , -- UriCount - nvarchar(500)
          @IsEnabled , -- IsEnabled - bit
          @IsCountDown , -- IsCountDown - bit
          @StartDateTime , -- StartDateTime - datetime
          @EndDateTime , -- EndDateTime - datetime
          GETDATE() , -- CreateDateTime - datetime
          GETDATE() , -- UpdateDateTime - datetime
          @IsChildProduct,  -- IsChildProduct - bit
          @PriorityLevel,
          @Type,@ParentPKID,@BigTitle,@BigTilteColor,@SmallTitle,@SmallTitleColor,@APPChannel,@SmallBgImage
        )
";
            return connection.Execute(sql, model) > 0;
        }

        public bool UpdateFlow(SqlConnection connection, SE_HomePageFlowConfig model)
        {
            string sql = @"UPDATE Configuration.dbo.SE_HomePageFlowConfig SET APPChannel=@APPChannel, BigTitle=@BigTitle,BigTilteColor=@BigTilteColor,SmallTitle=@SmallTitle,SmallTitleColor=@SmallTitleColor , PriorityLevel=@PriorityLevel, Title=@Title, BgImageUrl=@BgImageUrl,StartVersion=@StartVersion,EndVersion=@EndVersion,Channel=@Channel,LinkUrl=@LinkUrl,MPLinkUrl=@MPLinkUrl,WebLinkUrl=@WebLinkUrl,UriCount=@UriCount,IsEnabled=@IsEnabled,IsCountDown=@IsCountDown,StartDateTime=@StartDateTime,EndDateTime=@EndDateTime,UpdateDateTime=GetDate(),IsChildProduct=@IsChildProduct,SmallBgImage=@SmallBgImage WHERE ID=@ID";
            return connection.Execute(sql, model) > 0;
        }

        public SE_HomePageFlowConfig GetFlowEntity(SqlConnection connection, int id)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ID", id);
            return connection.Query<SE_HomePageFlowConfig>("SELECT * FROM Configuration.dbo.SE_HomePageFlowConfig  (NOLOCK) WHERE ID=@ID ", dp).FirstOrDefault();
        }

        /// <summary>
        /// 获取瀑布流列表
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public IEnumerable<SE_HomePageFlowConfig> GetFlowList(SqlConnection connection, string type)
        {
            string sql = "SELECT * FROM Configuration.dbo.SE_HomePageFlowConfig  (NOLOCK) where [Type]=@Type  ORDER BY PriorityLevel ASC ";
            DynamicParameters parame = new DynamicParameters();
            parame.Add("@Type", type);
            return connection.Query<SE_HomePageFlowConfig>(sql, parame);
        }

        public bool DeleteFlow(SqlConnection connection, int id)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ID", id);
            return connection.Execute("DELETE FROM Configuration.dbo.SE_HomePageFlowConfig WHERE ID=@ID", dp) > 0;
        }


        public bool AddFlowProduct(SqlConnection connection, SE_HomePageFlowProductConfig model)
        {
            string sql = @"INSERT INTO Configuration.dbo.SE_HomePageFlowProductConfig
        ( FKHomePageFlowConfig ,
          PID ,
          DisplayName ,
          Price ,
          PKFlashSale ,
          PriorityLevel ,
          IsEnabled ,
          CreateDateTime ,
          UpdateDateTime
        )
VALUES  ( @FKHomePageFlowConfig, -- FKHomePageFlowConfig - int
          @PID, -- PID - nvarchar(200)
          @DisplayName , -- DisplayName - nvarchar(300)
          @Price , -- Price - money
          @PKFlashSale , -- PKFlashSale - uniqueidentifier
          @PriorityLevel , -- PriorityLevel - int
          @IsEnabled , -- IsEnabled - bit
          GETDATE() , -- CreateDateTime - datetime
          GETDATE()  -- UpdateDateTime - datetime
        )";
            return connection.Execute(sql, model) > 0;
        }


        public bool DeleteFlowProduct(SqlConnection connection, int id)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ID", id);
            return connection.Execute("DELETE FROM Configuration.dbo.SE_HomePageFlowProductConfig WHERE ID=@ID", dp) > 0;
        }

        public IEnumerable<SE_HomePageFlowProductConfig> GetFlowProductList(SqlConnection connection, int id)
        {
            string sql = @"SELECT * FROM Configuration.dbo.SE_HomePageFlowProductConfig (NOLOCK) WHERE FKHomePageFlowConfig=@FKHomePageFlowConfig ORDER BY PriorityLevel ASC ";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@FKHomePageFlowConfig", id);
            return connection.Query<SE_HomePageFlowProductConfig>(sql, dp);
        }


        #endregion


        #region 预览首页
        /// <summary>
        /// 获取当前显示首页的模块列表
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public IEnumerable<SE_HomePageModuleConfig> GetPreViewModuleList(SqlConnection connection)
        {
            string sql = @"SELECT * FROM Configuration.dbo.SE_HomePageModuleConfig (NOLOCK)
                            WHERE FKHomePage  IN (
                             SELECT TOP 1 ID FROM Configuration.dbo.SE_HomePageConfig (NOLOCK) WHERE IsEnabled=1  )
                             ORDER BY PriorityLevel ASC";
            return connection.Query<SE_HomePageModuleConfig>(sql);
        }
        #endregion


        #region 底部菜单
        public bool AddHomePageMenu(SqlConnection connection, SE_HomePageMenuConfig model)
        {
            string sql = @"INSERT INTO Configuration.dbo.SE_HomePageMenuConfig
                                ( MenuName ,
                                  ShowImageUrl ,
                                  ClickImageUrl ,
                                  PriorityLevel ,
                                  CreateDateTime ,
                                  UpdateDateTime,
                                 FK_MenuListID,
                                 MenuType,ShowFontColor,ClickFontColor
                                )
                        VALUES  ( @MenuName , -- MenuName - nvarchar(100)
                                  @ShowImageUrl , -- ShowImageUrl - nvarchar(1000)
                                  @ClickImageUrl , -- ClickImageUrl - nvarchar(1000)
                                  @PriorityLevel , -- PriorityLevel - int
                                  GETDATE() , -- CreateDateTime - datetime
                                  GETDATE(),  -- UpdateDateTime - datetime
                                  @FK_MenuListID,
                                  @MenuType,
                                  @ShowFontColor,
                                  @ClickFontColor
                                )";

            return connection.Execute(sql, model) > 0;
        }


        public bool UpdateHomePageMenu(SqlConnection connection, SE_HomePageMenuConfig model)
        {
            string sql = "UPDATE Configuration.dbo.SE_HomePageMenuConfig SET ClickFontColor=@ClickFontColor,ShowFontColor=@ShowFontColor, MenuType=@MenuType,  FK_MenuListID=@FK_MenuListID, MenuName=@MenuName, ShowImageUrl=@ShowImageUrl, ClickImageUrl=@ClickImageUrl, PriorityLevel=@PriorityLevel,UpdateDateTime = GETDATE() WHERE ID=@ID";

            return connection.Execute(sql, model) > 0;

        }


        public bool DeleteHomePageMenu(SqlConnection connection, int id)
        {
            string sql = @"DELETE FROM Configuration.dbo.SE_HomePageMenuConfig WHERE ID=@ID";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ID", id);
            return connection.Execute(sql, dp) > 0;
        }


        public SE_HomePageMenuConfig GetHomePageMenuEntity(SqlConnection connection, int id)
        {
            string sql = @"SELECT * FROM Configuration.dbo.SE_HomePageMenuConfig (NOLOCK) WHERE ID=@ID ";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ID", id);
            return connection.Query<SE_HomePageMenuConfig>(sql, dp).FirstOrDefault();
        }

        public IEnumerable<SE_HomePageMenuConfig> GetHomePageMenuList(SqlConnection connection, int id)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@FK_MenuListID", id);
            return connection.Query<SE_HomePageMenuConfig>("SELECT * FROM Configuration.dbo.SE_HomePageMenuConfig (NOLOCK) where FK_MenuListID=@FK_MenuListID  ORDER BY PriorityLevel ASC", dp);
        }


        public bool AddHomePageMenuList(SqlConnection connection, SE_HomePageMenuListConfig model)
        {

            string sql = @"INSERT INTO Configuration.dbo.SE_HomePageMenuListConfig
                            (Name,
                              StartDateTime,
                              EndDateTime,
                              CreateDateTime,
                              UpdateDateTime,
                              StartVersion,
                              EndVersion
                            )
                    VALUES(@Name, --Name - nvarchar(50)
                              @StartDateTime, --StartDateTime - datetime
                             @EndDateTime, --EndDateTime - datetime
                              GETDATE(), --CreateDateTime - datetime
                              GETDATE(),-- UpdateDateTime - datetime
                              @StartVersion,@EndVersion
                            )";

            return connection.Execute(sql, model) > 0;
        }


        public bool ExistsHomePageMenuList(SqlConnection connection, DateTime startDate, DateTime endDate)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@StartDateTime", startDate);
            dp.Add("@EndDateTime", endDate);
            return (int)connection.ExecuteScalar(@"SELECT SUM(n) FROM(
                SELECT  COUNT(1) AS n  FROM Configuration.dbo.SE_HomePageMenuListConfig (NOLOCK) WHERE StartDateTime<=@StartDateTime AND  EndDateTime>= @StartDateTime
                UNION
                SELECT  COUNT(1) AS n  FROM Configuration.dbo.SE_HomePageMenuListConfig (NOLOCK) WHERE StartDateTime<=@EndDateTime AND  EndDateTime>= @EndDateTime
                ) AS MT", dp) > 0;
        }


        public bool UpdateHomePageMenuList(SqlConnection connection, SE_HomePageMenuListConfig model)
        {

            string sql = "		UPDATE Configuration.dbo.SE_HomePageMenuListConfig SET StartVersion=@StartVersion,EndVersion=@EndVersion,  Name=@Name, StartDateTime=@StartDateTime, EndDateTime=@EndDateTime,UpdateDateTime=@UpdateDateTime WHERE ID=@ID ";
            return connection.Execute(sql, model) > 0;
        }

        public IEnumerable<SE_HomePageMenuListConfig> GetHomePageMenuListList(SqlConnection connection)
        {
            string sql = "		SELECT *FROM Configuration.dbo.SE_HomePageMenuListConfig (NOLOCK) ";
            return connection.Query<SE_HomePageMenuListConfig>(sql);
        }

        public SE_HomePageMenuListConfig GetHomePageMenuListEntity(SqlConnection connection, int id)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ID", id);

            string sql = "		SELECT *  FROM Configuration.dbo.SE_HomePageMenuListConfig (NOLOCK) where ID=@ID  ";
            return connection.Query<SE_HomePageMenuListConfig>(sql, dp).FirstOrDefault();
        }


        public bool DeleteHomePageMenuList(SqlConnection connection, int id)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ID", id);

            string sql = "		DELETE   FROM Configuration.dbo.SE_HomePageMenuListConfig where ID=@ID  ";
            return connection.Execute(sql, dp) > 0;
        }


        #endregion


        #region 更新下次获取时间
        public string GetNextDateTime(SqlConnection connection)
        {
            string date = connection.Query<string>("SELECT  [Value]　FROM Configuration..ConfigApi (NOLOCK) WHERE [Key]='themeUpdateTime'").FirstOrDefault();
            return string.IsNullOrWhiteSpace(date) == true ? "" : Convert.ToDateTime(date).ToString("yyyy-MM-dd HH:mm:ss");
        }

        public bool UpdateNextDateTime(SqlConnection connection, DateTime date)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@Value", date.ToString("yyyy-MM-dd HH:mm:ss"));
            return connection.Execute("UPDATE Configuration.dbo.ConfigApi SET Value=@Value WHERE [Key]='themeUpdateTime'", dp) > 0;
        }

        #endregion



        #region 动画模块
        public IEnumerable<SE_HomePageAnimationContent> GetHomePageAnimationContentList(SqlConnection conn, int? moduleID, int? moduleHelperID)
        {
            if (moduleID != null)
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@FKModuleID", moduleID);
                return conn.Query<SE_HomePageAnimationContent>("SELECT * FROM Configuration.dbo.SE_HomePageAnimationContent WHERE FKModuleID=@FKModuleID", dp);
            }
            else if (moduleHelperID != null)
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@FKModuleHelper", moduleHelperID);
                return conn.Query<SE_HomePageAnimationContent>("SELECT * FROM Configuration.dbo.SE_HomePageAnimationContent WHERE FKModuleHelper=@FKModuleHelper", dp);

            }
            else
                return null;

        }


        public bool AddHomePageAnimationContent(SqlConnection conn, SE_HomePageAnimationContent model)
        {
            string sql = @"INSERT INTO Configuration.dbo.SE_HomePageAnimationContent
                            ( Name ,
                              StartVersion ,
                              EndVersion ,
                              BannerImageUrl ,
                              ButtonImageUrl ,
                              LinkUrl ,
                              TrackingId ,
                              BigTitle ,
                              BigTilteColor ,
                              SmallTitle ,
                              SmallTitleColor ,
                              PromoteTitle ,
                              PromoteTitleColor ,
                              PromoteTitleBgColor ,
                              AnimationStyle ,
                              PriorityLevel ,
                              IsEnabled ,
                              CreateDateTime ,
                              UpdateDateTime ,
                              FKModuleID ,
                              FKModuleHelper
                            )
                    VALUES  ( @Name , -- Name - varchar(30)
                              @StartVersion , -- StartVersion - varchar(20)
                              @EndVersion , -- EndVersion - varchar(20)
                              @BannerImageUrl , -- BannerImageUrl - varchar(1000)
                              @ButtonImageUrl , -- ButtonImageUrl - varchar(1000)
                             @LinkUrl , -- LinkUrl - varchar(1000)
                              @TrackingId , -- TrackingId - varchar(50)
                             @BigTitle , -- BigTitle - varchar(50)
                              @BigTilteColor , -- BigTilteColor - varchar(100)
                              @SmallTitle , -- SmallTitle - varchar(50)
                              @SmallTitleColor , -- SmallTitleColor - varchar(100)
                             @PromoteTitle , -- PromoteTitle - varchar(100)
                             @PromoteTitleColor , -- PromoteTitleColor - varchar(100)
                              @PromoteTitleBgColor , -- PromoteTitleBgColor - varchar(100)
                             @AnimationStyle , -- AnimationStyle - int
                             @PriorityLevel , -- PriorityLevel - int
                              @IsEnabled , -- IsEnabled - bit
                              GETDATE() , -- CreateDateTime - datetime
                              GETDATE() , -- UpdateDateTime - datetime
                              @FKModuleID , -- FKModuleID - int
                              @FKModuleHelper  -- FKModuleHelper - int
                            )";
            return conn.Execute(sql, model) > 0;
        }

        public bool UpdateHomePageAnimationContent(SqlConnection conn, SE_HomePageAnimationContent model)
        {
            string sql = @"UPDATE Configuration.dbo.SE_HomePageAnimationContent SET Name=@Name ,
          StartVersion =@StartVersion,
          EndVersion =@EndVersion,
          BannerImageUrl =@BannerImageUrl,
          ButtonImageUrl =@ButtonImageUrl,
          LinkUrl =@LinkUrl,
          TrackingId =@TrackingId,
          BigTitle =@BigTitle,
          BigTilteColor =@BigTilteColor,
          SmallTitle=@SmallTitle ,
          SmallTitleColor =@SmallTitleColor,
          PromoteTitle =@PromoteTitle,
          PromoteTitleColor=@PromoteTitleColor ,
          PromoteTitleBgColor=@PromoteTitleBgColor ,
          AnimationStyle =@AnimationStyle,
          PriorityLevel=@PriorityLevel ,
          IsEnabled =@IsEnabled,
          UpdateDateTime =@UpdateDateTime,
          FKModuleID =@FKModuleID,
          FKModuleHelper=@FKModuleHelper WHERE ID=@ID";
            return conn.Execute(sql, model) > 0;
        }

        public SE_HomePageAnimationContent GetHomePageAnimationContentEntity(SqlConnection conn, int id)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ID", id);
            return conn.Query<SE_HomePageAnimationContent>("SELECT * FROM Configuration.dbo.SE_HomePageAnimationContent WHERE ID=@ID", dp).FirstOrDefault();
        }

        public bool DeleteHomePageAnimationContent(SqlConnection conn, int id)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ID", id);
            return conn.Execute("DELETE FROM Configuration.dbo.SE_HomePageAnimationContent WHERE ID=@ID", dp) > 0;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public List<SE_HomePageConfigTags> SelectModuleTags(SqlConnection connection, int parentId = 0)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ParentID", parentId);
            return connection.Query<SE_HomePageConfigTags>(" SELECT * FROM Configuration..SE_HomePageConfigTags WITH(NOLOCK) WHERE ParentID = @ParentID", dp).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public List<SE_HomePageConfigTags> SelectModuleTagsById(SqlConnection connection, string ids)
        {
            return connection.Query<SE_HomePageConfigTags>($@"SELECT  T.Category PCategory,
		                                                            Ts.PKID ,
                                                                    Ts.Category ,
                                                                    Ts.ParentId 
                                                            FROM    Configuration..SE_HomePageConfigTags T WITH ( NOLOCK )
                                                                    JOIN Configuration..SE_HomePageConfigTags Ts WITH ( NOLOCK ) ON Ts.ParentId = T.PKID
                                                            WHERE   T.PKID IN ({ids});").ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public List<SE_HomePageModuleTagConfig> GetModuleTagsConfigList(SqlConnection connection, int moduleId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@FKHomePageModuleContentID", moduleId);
            return connection.Query<SE_HomePageModuleTagConfig>(@"SELECT  *
            FROM    Configuration..SE_HomePageModuleTagConfig (NOLOCK) HPMTC
            WHERE   HPMTC.FKHomePageModuleContentID = @FKHomePageModuleContentID;", p).ToList();
        }
    }
}

