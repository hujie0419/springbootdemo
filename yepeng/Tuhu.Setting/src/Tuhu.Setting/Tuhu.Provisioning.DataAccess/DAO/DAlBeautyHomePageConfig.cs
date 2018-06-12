using Microsoft.ApplicationBlocks.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess
{
    public class DALBeautyHomePageConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);

        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Tuhu_Groupon_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);

        public static List<BeautyHomePageConfig> GetBeautyHomePageConfigList(BeautyHomePageConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @" 
                        SELECT  *
                        FROM    ( 
                                 SELECT ROW_NUMBER() OVER ( ORDER BY A.Id DESC ) AS ROWNUMBER ,
                                        A.[Id] ,
                                        [Type] ,
                                        [StartVersion] ,
                                        [EndVersion] ,
                                        A.[Name] ,
                                        [Sort] ,
                                        [Title] ,
                                        [SmallTitle] ,
                                        [Channel] ,
                                        A.[CategoryID] ,
                                        [CategoryName] ,
                                        [Link] ,
                                        [Icon] ,
                                        A.[Status] ,
                                        [IsRegion] ,
                                        [CarLevel] ,
                                        A.[ActivityId] ,
                                        [StartTime] ,
                                        [EndTime] ,
                                        A.[CreateTime] ,
                                        A.[UpdateTime] ,
                                        b.Name AS ActivityName,
                                        ActivityPKID,
                                        Banner
                                 FROM   [Tuhu_Groupon].[dbo].SE_BeautyHomePageConfig AS A WITH ( NOLOCK )
                                        LEFT JOIN Tuhu_Groupon.dbo.ShopBeautyAcitivity AS b WITH ( NOLOCK ) ON A.ActivityPKID = b.Id
                                  WHERE     1 = 1  AND Type=@Type 
                                ) AS PG
                        WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                             AND     STR(@PageIndex * @PageSize)
                          ";
            string sqlCount = @"SELECT COUNT(1) FROM [Tuhu_Groupon].[dbo].[SE_BeautyHomePageConfig] WITH (NOLOCK)  WHERE 1=1 AND Type=@Type  ";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@Type",model.Type)
            };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount, sqlParameters);

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<BeautyHomePageConfig>().ToList();

        }


        public static BeautyHomePageConfig GetBeautyHomePageConfig(int id)
        {
            const string sql = @"SELECT
                                           [Id]
                                          ,[Type]
                                          ,[StartVersion]
                                          ,[EndVersion]
                                          ,[Name]
                                          ,[Sort]
                                          ,[Title]
                                          ,[SmallTitle]
                                          ,[Channel]
                                          ,[CategoryID]
                                          ,[CategoryName]
                                          ,[Link]
                                          ,[Icon]
                                          ,[Status]                                 
                                          ,[IsRegion]
                                          ,[CarLevel]
                                          ,[ActivityId]
                                          ,[StartTime]
                                          ,[EndTime]
                                          ,[CreateTime]
                                          ,[UpdateTime]
                                          ,[ActivityPKID]
                                          ,[Banner]
                                          ,[IsNotShow] 
                                          ,[BannerConfigs] 
                                  FROM [Tuhu_Groupon].[dbo].[SE_BeautyHomePageConfig] WITH (NOLOCK) WHERE Id=@id";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<BeautyHomePageConfig>().ToList().FirstOrDefault();

        }

        public static bool InsertBeautyHomePageConfig(BeautyHomePageConfig model, ref int id)
        {
            const string sql = @"INSERT INTO Tuhu_Groupon..SE_BeautyHomePageConfig
                                          (    
                                           [Type]
                                          ,[StartVersion]
                                          ,[EndVersion]
                                          ,[Name]
                                          ,[Sort]
                                          ,[Title]
                                          ,[SmallTitle]
                                          ,[Channel]
                                          ,[CategoryID]
                                          ,[CategoryName]
                                          ,[Link]
                                          ,[Icon]
                                          ,[Status]                                        
                                          ,[IsRegion]
                                          ,[CarLevel]
                                          ,[ActivityId]
                                          ,[StartTime]
                                          ,[EndTime]
                                          ,[CreateTime]
                                          ,[UpdateTime]
                                          ,[ActivityPKID]
                                          ,[Banner]
                                          ,[IsNotShow]
                                          ,[BannerConfigs] 
                                          )
                                  VALUES(  
                                           @Type
                                          ,@StartVersion
                                          ,@EndVersion
                                          ,@Name
                                          ,@Sort
                                          ,@Title
                                          ,@SmallTitle
                                          ,@Channel
                                          ,@CategoryID
                                          ,@CategoryName
                                          ,@Link
                                          ,@Icon
                                          ,@Status                                        
                                          ,@IsRegion
                                          ,@CarLevel
                                          ,@ActivityId
                                          ,@StartTime
                                          ,@EndTime
                                          ,GETDATE()
                                          ,GETDATE()  
                                          ,@ActivityPKID 
                                          ,@Banner   
                                          ,@IsNotShow
                                          ,@BannerConfigs 
                                        )SELECT @@IDENTITY";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@ActivityPKID",model.ActivityPKID),
                    new SqlParameter("@ActivityId",model.ActivityId??string.Empty),
                    new SqlParameter("@Banner",model.Banner??string.Empty),
                    new SqlParameter("@CarLevel",model.CarLevel),
                    new SqlParameter("@CategoryID",model.CategoryID),
                    new SqlParameter("@CategoryName",model.CategoryName??string.Empty),
                    new SqlParameter("@Channel",model.Channel??string.Empty),
                    new SqlParameter("@EndTime",model.EndTime),
                    new SqlParameter("@EndVersion",model.EndVersion??string.Empty),
                    new SqlParameter("@Icon",model.Icon??string.Empty),
                    new SqlParameter("@IsRegion",model.IsRegion),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@Link",model.Link??string.Empty),
                    new SqlParameter("@Name",model.Name??string.Empty),
                    new SqlParameter("@SmallTitle",model.SmallTitle??string.Empty),
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@StartTime",model.StartTime),
                    new SqlParameter("@StartVersion",model.StartVersion??string.Empty),
                    new SqlParameter("@Title",model.Title??string.Empty),
                    new SqlParameter("@Type",model.Type),
                    new SqlParameter("@IsNotShow",model.IsNotShow),
                    new SqlParameter("@BannerConfigs",model.BannerConfigs),
                };


            List<RegionRelation> region = JsonConvert.DeserializeObject<List<RegionRelation>>(model.Region ?? "") ?? new List<RegionRelation>();

            string strConn = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
            string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;

            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();
            try
            {
                id = Convert.ToInt32(SqlHelper.ExecuteScalar(tran, CommandType.Text, sql, sqlParameter));

                DALMeiRongAcitivityConfig.DeleteRegionRelation(model.Id, tran);

                foreach (var item in region)
                {
                    item.Type = 2;
                    item.ActivityId = id;
                    DALMeiRongAcitivityConfig.InsertRegionRelation(item, tran);
                }

                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
                throw ex;
            }
            finally
            {
                tran.Dispose();
            }
        }

        public static bool UpdateBeautyHomePageConfig(BeautyHomePageConfig model)
        {
            const string sql = @"UPDATE Tuhu_Groupon.dbo.SE_BeautyHomePageConfig SET                                      
                                           Type=@Type
                                          ,StartVersion=@StartVersion
                                          ,EndVersion=@EndVersion
                                          ,Name=@Name
                                          ,Sort=@Sort
                                          ,Title=@Title
                                          ,SmallTitle=@SmallTitle
                                          ,Channel=@Channel
                                          ,CategoryID=@CategoryID
                                          ,CategoryName=@CategoryName
                                          ,Link=@Link
                                          ,Icon=@Icon  
                                          ,Banner=@Banner 
                                          ,Status=@Status                                       
                                          ,IsRegion=@IsRegion
                                          ,CarLevel=@CarLevel
                                          ,ActivityId=@ActivityId  
                                          ,ActivityPKID=@ActivityPKID
                                          ,StartTime=@StartTime
                                          ,EndTime=@EndTime
                                          ,UpdateTime=GETDATE()  
                                          ,IsNotShow=@IsNotShow
                                          ,BannerConfigs=@BannerConfigs 
                                WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@ActivityPKID",model.ActivityPKID),
                    new SqlParameter("@ActivityId",model.ActivityId??string.Empty),
                    new SqlParameter("@Banner",model.Banner??string.Empty),
                    new SqlParameter("@CarLevel",model.CarLevel),
                    new SqlParameter("@CategoryID",model.CategoryID),
                    new SqlParameter("@CategoryName",model.CategoryName??string.Empty),
                    new SqlParameter("@Channel",model.Channel??string.Empty),
                    new SqlParameter("@EndTime",model.EndTime),
                    new SqlParameter("@EndVersion",model.EndVersion??string.Empty),
                    new SqlParameter("@Icon",model.Icon??string.Empty),
                    new SqlParameter("@IsRegion",model.IsRegion),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@Link",model.Link??string.Empty),
                    new SqlParameter("@Name",model.Name??string.Empty),
                    new SqlParameter("@SmallTitle",model.SmallTitle??string.Empty),
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@StartTime",model.StartTime),
                    new SqlParameter("@StartVersion",model.StartVersion??string.Empty),
                    new SqlParameter("@Title",model.Title??string.Empty),
                    new SqlParameter("@Type",model.Type),
                    new SqlParameter("@Id",model.Id),
                    new SqlParameter("@IsNotShow",model.IsNotShow),
                    new SqlParameter("@BannerConfigs",model.BannerConfigs),
               };

            List<RegionRelation> region = JsonConvert.DeserializeObject<List<RegionRelation>>(model.Region ?? string.Empty) ?? new List<RegionRelation>();

            string strConn = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
            string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;

            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();
            try
            {
                SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql, sqlParameter);

                DALMeiRongAcitivityConfig.DeleteRegionRelation(model.Id, tran);

                if (model.IsRegion)
                {
                    foreach (var item in region)
                    {
                        item.Type = 2;
                        item.ActivityId = model.Id;
                        DALMeiRongAcitivityConfig.InsertRegionRelation(item, tran);
                    }
                }


                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
                throw ex;
            }
            finally
            {
                tran.Dispose();
            }
        }
        public static bool InsertBeautyHomePageBannerConfig(BeautyHomePageConfig model, ref int id)
        {
            const string sql = @"INSERT INTO Tuhu_Groupon..SE_BeautyHomePageConfig
                                          (    
                                           [Type]
                                          ,[StartVersion]
                                          ,[EndVersion]
                                          ,[Name]
                                          ,[Sort]
                                          ,[Title]
                                          ,[SmallTitle]
                                          ,[Channel]
                                          ,[CategoryID]
                                          ,[CategoryName]
                                          ,[Link]
                                          ,[Icon]
                                          ,[Status]                                        
                                          ,[IsRegion]
                                          ,[CarLevel]
                                          ,[ActivityId]
                                          ,[StartTime]
                                          ,[EndTime]
                                          ,[CreateTime]
                                          ,[UpdateTime]
                                          ,[ActivityPKID]
                                          ,[Banner]
                                          )
                                  VALUES(  
                                           @Type
                                          ,@StartVersion
                                          ,@EndVersion
                                          ,@Name
                                          ,@Sort
                                          ,@Title
                                          ,@SmallTitle
                                          ,@Channel
                                          ,@CategoryID
                                          ,@CategoryName
                                          ,@Link
                                          ,@Icon
                                          ,@Status                                        
                                          ,@IsRegion
                                          ,@CarLevel
                                          ,@ActivityId
                                          ,@StartTime
                                          ,@EndTime
                                          ,GETDATE()
                                          ,GETDATE()  
                                          ,@ActivityPKID 
                                          ,@Banner                                
                                        )SELECT @@IDENTITY";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@ActivityPKID",model.ActivityPKID),
                    new SqlParameter("@ActivityId",model.ActivityId??string.Empty),
                    new SqlParameter("@Banner",model.Banner??string.Empty),
                    new SqlParameter("@CarLevel",model.CarLevel),
                    new SqlParameter("@CategoryID",model.CategoryID),
                    new SqlParameter("@CategoryName",model.CategoryName??string.Empty),
                    new SqlParameter("@Channel",model.Channel??string.Empty),
                    new SqlParameter("@EndTime",model.EndTime),
                    new SqlParameter("@EndVersion",model.EndVersion??string.Empty),
                    new SqlParameter("@Icon",model.Icon??string.Empty),
                    new SqlParameter("@IsRegion",model.IsRegion),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@Link",model.Link??string.Empty),
                    new SqlParameter("@Name",model.Name??string.Empty),
                    new SqlParameter("@SmallTitle",model.SmallTitle??string.Empty),
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@StartTime",model.StartTime),
                    new SqlParameter("@StartVersion",model.StartVersion??string.Empty),
                    new SqlParameter("@Title",model.Title??string.Empty),
                    new SqlParameter("@Type",model.Type),

                };


            List<RegionRelation> region = JsonConvert.DeserializeObject<List<RegionRelation>>(model.Region ?? "") ?? new List<RegionRelation>();

            string strConn = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
            string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;

            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();
            try
            {
                id = Convert.ToInt32(SqlHelper.ExecuteScalar(tran, CommandType.Text, sql, sqlParameter));

                DALMeiRongAcitivityConfig.DeleteRegionRelation(model.Id, tran);

                foreach (var item in region)
                {
                    item.Type = 3;
                    item.ActivityId = id;
                    DALMeiRongAcitivityConfig.InsertRegionRelation(item, tran);
                }

                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
                throw ex;
            }
            finally
            {
                tran.Dispose();
            }
        }
        public static bool UpdateBeautyHomePageBannerConfig(BeautyHomePageConfig model)
        {
            const string sql = @"UPDATE Tuhu_Groupon.dbo.SE_BeautyHomePageConfig SET                                      
                                           Type=@Type
                                          ,StartVersion=@StartVersion
                                          ,EndVersion=@EndVersion
                                          ,Name=@Name
                                          ,Sort=@Sort
                                          ,Title=@Title
                                          ,SmallTitle=@SmallTitle
                                          ,Channel=@Channel
                                          ,CategoryID=@CategoryID
                                          ,CategoryName=@CategoryName
                                          ,Link=@Link
                                          ,Icon=@Icon  
                                          ,Banner=@Banner 
                                          ,Status=@Status                                       
                                          ,IsRegion=@IsRegion
                                          ,CarLevel=@CarLevel
                                          ,ActivityId=@ActivityId  
                                          ,ActivityPKID=@ActivityPKID
                                          ,StartTime=@StartTime
                                          ,EndTime=@EndTime
                                          ,UpdateTime=GETDATE()                                        
                                WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@ActivityPKID",model.ActivityPKID),
                    new SqlParameter("@ActivityId",model.ActivityId??string.Empty),
                    new SqlParameter("@Banner",model.Banner??string.Empty),
                    new SqlParameter("@CarLevel",model.CarLevel),
                    new SqlParameter("@CategoryID",model.CategoryID),
                    new SqlParameter("@CategoryName",model.CategoryName??string.Empty),
                    new SqlParameter("@Channel",model.Channel??string.Empty),
                    new SqlParameter("@EndTime",model.EndTime),
                    new SqlParameter("@EndVersion",model.EndVersion??string.Empty),
                    new SqlParameter("@Icon",model.Icon??string.Empty),
                    new SqlParameter("@IsRegion",model.IsRegion),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@Link",model.Link??string.Empty),
                    new SqlParameter("@Name",model.Name??string.Empty),
                    new SqlParameter("@SmallTitle",model.SmallTitle??string.Empty),
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@StartTime",model.StartTime),
                    new SqlParameter("@StartVersion",model.StartVersion??string.Empty),
                    new SqlParameter("@Title",model.Title??string.Empty),
                    new SqlParameter("@Type",model.Type),
                    new SqlParameter("@Id",model.Id)

               };

            List<RegionRelation> region = JsonConvert.DeserializeObject<List<RegionRelation>>(model.Region ?? string.Empty) ?? new List<RegionRelation>();

            string strConn = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
            string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;

            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();
            try
            {
                SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql, sqlParameter);

                DALMeiRongAcitivityConfig.DeleteRegionRelation(model.Id, tran);

                if (model.IsRegion)
                {
                    foreach (var item in region)
                    {
                        item.Type = 3;
                        item.ActivityId = model.Id;
                        DALMeiRongAcitivityConfig.InsertRegionRelation(item, tran);
                    }
                }


                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
                throw ex;
            }
            finally
            {
                tran.Dispose();
            }
        }
        public static bool DeleteBeautyHomePageConfig(int id)
        {
            const string sql = @"DELETE FROM Tuhu_Groupon.dbo.SE_BeautyHomePageConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }


        public static IEnumerable<string> GetBeautyChannel()
        {
            const string sql = @"SELECT  DISTINCT
        c.Channel
FROM    Tuhu_groupon.dbo.SE_BeautyHomePageConfig AS c WITH ( NOLOCK );";

            var dt = SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql);

            var result = dt.Rows.Cast<DataRow>().Select(x => x["Channel"].ToString()).Where(x => !string.IsNullOrWhiteSpace(x));

            return result;
        }

        public static IEnumerable<BeautyHomePageConfig> SelectShopMapConfigs(SqlConnection conn)
        {
            var sql = @"SELECT  c.Id ,
        c.Name ,
        CAST(c.Type - 1000 AS SMALLINT) AS Type ,
        c.Sort ,
        c.Icon ,
        c.Channel ,
        c.Status
FROM    Tuhu_groupon..SE_BeautyHomePageConfig AS c WITH ( NOLOCK )
WHERE   c.Type >= 1000
        AND c.Type < 2000
ORDER BY c.Sort ,
        c.Channel ,
        c.Type;
";
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            var result = dt.Rows.Cast<DataRow>().Select(row => new BeautyHomePageConfig
            {
                Id = (int)row["Id"],
                Name = row["Name"].ToString(),
                Sort = row["Sort"] != DBNull.Value ? (int)row["Sort"] : 0,
                Icon = row["Icon"].ToString(),
                Channel = row["Channel"].ToString(),
                Type = (short)row["Type"],
                Status = (bool)row["Status"]
            });
            return result;
        }

        public static List<BeautyHomePageConfig> SelectBeautyBannerConfig(int status, string keyWords, int pageSize, int pageIndex, out int count)
        {
            #region sql
            string sql_count = @"SELECT COUNT(1) 
FROM   [Tuhu_Groupon].[dbo].SE_BeautyHomePageConfig AS A WITH ( NOLOCK )
LEFT JOIN Tuhu_Groupon.dbo.ShopBeautyAcitivity AS b WITH ( NOLOCK ) ON A.ActivityPKID = b.Id
WHERE   [Type]=3  {0} {1}";
            string sql = @"SELECT 
       A.[Id] ,
       [Type] ,
       [StartVersion] ,
       [EndVersion] ,
       A.[Name] ,
       [Sort] ,
       [Title] ,
       [SmallTitle] ,
       [Channel] ,
       A.[CategoryID] ,
       [CategoryName] ,
       [Link] ,
       [Icon] ,
       A.[Status] ,
       [IsRegion] ,
       A.[ActivityId] ,
       [StartTime] ,
       [EndTime] ,
       A.[CreateTime] ,
       A.[UpdateTime] ,
       b.Name AS ActivityName,
       ActivityPKID,
       Banner
FROM   [Tuhu_Groupon].[dbo].SE_BeautyHomePageConfig AS A WITH ( NOLOCK )
LEFT JOIN Tuhu_Groupon.dbo.ShopBeautyAcitivity AS b WITH ( NOLOCK ) ON A.ActivityPKID = b.Id
WHERE   [Type]=3  {0} {1}
Order by  A.[UpdateTime] DESC 
Offset 0 Rows Fetch NEXT 10 Rows Only";
            List<SqlParameter> paraList = new List<SqlParameter>();
            string sql_status = "";
            if (status == 0 || status == 1)
            {
                sql_status = $" AND A.[Status]=@status";
                paraList.Add(new SqlParameter("@status", status));
            }
            string sql_keyword = "";
            if (!string.IsNullOrEmpty(keyWords))
            {
                sql_keyword = $" AND A.[Name] LIKE  @keyWords ";
                paraList.Add(new SqlParameter("@keyWords", $"{keyWords}%"));
            }
            sql_count = string.Format(sql_count, sql_status, sql_keyword);
            sql = string.Format(sql, sql_status, sql_keyword);
            #endregion
            List<BeautyHomePageConfig> result = new List<BeautyHomePageConfig>();
            count = Convert.ToInt32(SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sql_count, paraList.ToArray()));
            if (count > 0)
            {
                result = SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, paraList.ToArray()).AsEnumerable().ToArray().Select(s => new BeautyHomePageConfig
                {
                    ActivityId = s.Field<string>("ActivityId"),
                    ActivityName = s.Field<string>("ActivityName"),
                    ActivityPKID = s.Field<int>("ActivityPKID"),
                    Banner = s.Field<string>("Banner"),
                    CategoryID = s.Field<int>("CategoryID"),
                    CategoryName = s.Field<string>("CategoryName"),
                    Channel = s.Field<string>("Channel"),
                    CreateTime = s.Field<DateTime>("CreateTime"),
                    EndTime = s.Field<DateTime>("EndTime"),
                    EndVersion = s.Field<string>("EndVersion"),
                    Icon = s.Field<string>("Icon"),
                    Id = s.Field<int>("Id"),
                    IsRegion = s.Field<bool>("IsRegion"),
                    Link = s.Field<string>("Link"),
                    Name = s.Field<string>("Name"),
                    Sort = s.Field<int>("Sort"),
                    StartVersion = s.Field<string>("StartVersion"),
                    StartTime = s.Field<DateTime>("StartTime"),
                    Status = s.Field<bool>("Status"),
                    Title = s.Field<string>("Title"),
                    UpdateTime = s.Field<DateTime>("UpdateTime"),

                }).ToList();

            }
            return result;

        }

        public static List<BeautyPopUpWindowsConfig> SelectBeautyPopUpConfigs(int status, string keyWords, int pageSize, int pageIndex, out int count)
        {
            #region sql
            string sql_count = @"SELECT COUNT(1) 
FROM   [Tuhu_Groupon].[dbo].SE_BeautyPopUpWindowsConfig  WITH ( NOLOCK ) 
WHERE   IsDeleted=0  {0} {1}";
            string sql = @"SELECT PKID, 
PlaceType, 
Name, 
Channel, 
CategoryId, 
StartVersion, 
EndVersion, 
StartTime, 
EndTime, 
BackGroundImage, 
BackGroundLink, 
PromotionInfo, 
Status, 
CreateTime, 
UpdateTime 
FROM   [Tuhu_Groupon].[dbo].SE_BeautyPopUpWindowsConfig  WITH ( NOLOCK ) 
WHERE   IsDeleted=0  {0} {1} 
Order by  [UpdateTime] DESC  
Offset 0 Rows Fetch NEXT 10 Rows Only";
            List<SqlParameter> paraList = new List<SqlParameter>();
            string sql_status = "";
            if (status == 0 || status == 1)
            {
                sql_status = $" AND [Status]=@status";
                paraList.Add(new SqlParameter("@status", status));
            }
            string sql_keyword = "";
            if (!string.IsNullOrEmpty(keyWords))
            {
                sql_keyword = $" AND [Name] LIKE  @keyWords ";
                paraList.Add(new SqlParameter("@keyWords", $"{keyWords}%"));
            }
            sql_count = string.Format(sql_count, sql_status, sql_keyword);
            sql = string.Format(sql, sql_status, sql_keyword);
            #endregion
            List<BeautyPopUpWindowsConfig> result = new List<BeautyPopUpWindowsConfig>();
            count = Convert.ToInt32(SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sql_count, paraList.ToArray()));
            if (count > 0)
            {
                result = SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, paraList.ToArray()).AsEnumerable().ToArray().Select(s => new BeautyPopUpWindowsConfig
                {
                    BackGroundImage = s.Field<string>("BackGroundImage"),
                    BackGroundLink = s.Field<string>("BackGroundLink"),
                    CategoryId = s.Field<int>("CategoryId"),
                    Channel = s.Field<string>("Channel"),
                    PKID = s.Field<int>("PKID"),
                    PlaceType = s.Field<int>("PlaceType"),
                    PromotionInfo = s.Field<string>("PromotionInfo"),
                    CreateTime = s.Field<DateTime>("CreateTime"),
                    EndTime = s.Field<DateTime>("EndTime"),
                    EndVersion = s.Field<string>("EndVersion"),
                    Name = s.Field<string>("Name"),
                    StartVersion = s.Field<string>("StartVersion"),
                    StartTime = s.Field<DateTime>("StartTime"),
                    Status = s.Field<int>("Status"),
                    UpdateTime = s.Field<DateTime>("UpdateTime")
                }).ToList();

            }
            return result;

        }


        public static BeautyPopUpWindowsConfig GetBeautyPopUpWindowsConfig(int pkid)
        {
            const string sql = @"SELECT  [PKID]
      ,[PlaceType]
      ,[Name]
      ,[Channel]
      ,[CategoryId]
      ,[StartVersion]
      ,[EndVersion]
      ,[StartTime]
      ,[EndTime]
      ,[BackGroundImage]
      ,[BackGroundLink]
      ,[PromotionInfo]
      ,[Status]
      ,[CreateTime]
      ,[UpdateTime]
      ,[IsDeleted]
      ,[IsRegion]
      ,[PopUpLogo] 
  FROM [Tuhu_groupon].[dbo].[SE_BeautyPopUpWindowsConfig] WITH(NOLOCK) 
  WHERE PKID=@PKID AND isDeleted=0";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PKID",pkid)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<BeautyPopUpWindowsConfig>().ToList().FirstOrDefault();

        }

        public static bool InsertBeautyPopUpConfig(BeautyPopUpWindowsConfig model, out int pkid)
        {
            #region sql
            const string sql = @"  Insert Into [Tuhu_groupon].[dbo].[SE_BeautyPopUpWindowsConfig](
  [PlaceType]
 ,[Name]
 ,[Channel]
 ,[CategoryId]
 ,[StartVersion]
 ,[EndVersion]
 ,[StartTime]
 ,[EndTime]
 ,[BackGroundImage]
 ,[BackGroundLink]
 ,[PromotionInfo]
 ,[Status] 
 ,[CreateTime]
 ,[UpdateTime]
 ,[IsDeleted]
 ,[IsRegion]
 ,[PopUpLogo])
 Values(
  @PlaceType
 ,@Name
 ,@Channel
 ,@CategoryId
 ,@StartVersion
 ,@EndVersion
 ,@StartTime
 ,@EndTime
 ,@BackGroundImage
 ,@BackGroundLink
 ,@PromotionInfo
 ,@Status
 ,GetDate()
 ,GetDate()
 ,0
 ,@IsRegion
 ,@PopUpLogo
);SELECT @@Identity;";
            #endregion
            var sqlparameters = new SqlParameter[] {
                new SqlParameter("PlaceType",model.PlaceType),
                new SqlParameter("Name",model.Name),
                new SqlParameter("PromotionInfo",model.PromotionInfo),
                new SqlParameter("StartTime",model.StartTime),
                new SqlParameter("EndTime",model.EndTime),
                new SqlParameter("StartVersion",model.StartVersion),
                new SqlParameter("EndVersion",model.EndVersion),
                new SqlParameter("CategoryId",model.CategoryId),
                new SqlParameter("Channel",model.Channel),
                new SqlParameter("BackGroundImage",model.BackGroundImage),
                new SqlParameter("BackGroundLink",model.BackGroundLink),
                new SqlParameter("Status",model.Status),
                new SqlParameter("IsRegion",model.IsRegion),
                new SqlParameter("PopUpLogo",model.PopUpLogo)
            };
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                var tran = con.BeginTransaction();

                var result = Convert.ToInt32(SqlHelper.ExecuteScalar(tran, CommandType.Text, sql, sqlparameters));
                pkid = result;
                if (result > 0)
                {
                    if (!model.IsRegion)
                    {
                        tran.Commit();
                        return true;
                    }
                    model.RegionList.ForEach(f => f.BeautyPopUpId = result);
                    var temp = InsertBeautyPopUpWindowsRegionConfigs(tran, model.RegionList, true);
                    if (temp)
                    {
                        tran.Commit();
                        return true;
                    }
                    else
                    {
                        tran.Rollback();
                        return false;
                    }
                }
                else
                {
                    tran.Rollback();
                    return false;
                }
            }
        }

        public static bool UpdateBeautyPopUpConfig(BeautyPopUpWindowsConfig model)
        {
            #region sql
            const string sql = @"Update [Tuhu_groupon].[dbo].[SE_BeautyPopUpWindowsConfig] WITH(ROWLOCK) 
 SET [PlaceType]=ISNULL(@PlaceType,PlaceType)
 ,[Name]=ISNULL(@Name,Name)
 ,[Channel]=ISNULL(@Channel,Channel)
 ,[CategoryId]=ISNULL(@CategoryId,CategoryId)
 ,[StartVersion]=ISNULL(@StartVersion,StartVersion)
 ,[EndVersion]=ISNULL(@EndVersion,EndVersion)
 ,[StartTime]=ISNULL(@StartTime,StartTime)
 ,[EndTime]=ISNULL(@EndTime,EndTime)
 ,[BackGroundImage]=ISNULL(@BackGroundImage,BackGroundImage)
 ,[BackGroundLink]=ISNULL(@BackGroundLink,BackGroundLink)
 ,[PromotionInfo]=ISNULL(@PromotionInfo,PromotionInfo)
 ,[Status] =ISNULL(@Status,Status)
 ,[UpdateTime]=GetDate() 
 ,[IsRegion]=ISNULL(@IsRegion,IsRegion)
 ,[PopUpLogo]=ISNULL(@PopUpLogo,PopUpLogo) 
 WHERE PKID=@PKID AND ISDELETED=0 
";
            #endregion
            var sqlparameters = new SqlParameter[] {
                new SqlParameter("PlaceType",model.PlaceType),
                new SqlParameter("Name",model.Name),
                new SqlParameter("PromotionInfo",model.PromotionInfo),
                new SqlParameter("StartTime",model.StartTime),
                new SqlParameter("EndTime",model.EndTime),
                new SqlParameter("StartVersion",model.StartVersion),
                new SqlParameter("EndVersion",model.EndVersion),
                new SqlParameter("CategoryId",model.CategoryId),
                new SqlParameter("Channel",model.Channel),
                new SqlParameter("BackGroundImage",model.BackGroundImage),
                new SqlParameter("BackGroundLink",model.BackGroundLink),
                new SqlParameter("Status",model.Status),
                new SqlParameter("IsRegion",model.IsRegion),
                new SqlParameter("PopUpLogo",model.PopUpLogo),
                new SqlParameter("PKID",model.PKID),
            };
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                var tran = con.BeginTransaction();

                var result = Convert.ToInt32(SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql, sqlparameters));
                if (result > 0)
                {
                    if (!model.IsRegion)
                    {
                        tran.Commit();
                        return true;
                    }
                    model.RegionList.ForEach(f => f.BeautyPopUpId = model.PKID);
                    var temp = InsertBeautyPopUpWindowsRegionConfigs(tran, model.RegionList);
                    if (temp)
                    {
                        tran.Commit();
                        return true;
                    }
                    else
                    {
                        tran.Rollback();
                        return false;
                    }
                }
                else
                {
                    tran.Rollback();
                    return false;
                }
            }
        }

        public static bool DeleteBeautyPopUpConfig(int pkid)
        {
            #region sql
            const string sql = @"Update    [Tuhu_groupon].[dbo].[SE_BeautyPopUpWindowsConfig] WITH(ROWLOCK) 
 SET ISDELETED=1 
 WHERE PKID=@PKID 
";
            #endregion
            var sqlparameters = new SqlParameter[] {
                new SqlParameter("PKID",pkid)
            };
            return SqlHelper.ExecuteNonQuery(connOnRead, CommandType.Text, sql, sqlparameters) > 0;
        }

        public static IEnumerable<BeautyCategorySimple> GetBeautyCategorysByChannel(string channel)
        {
            const string sql = @"SELECT 
[Id]
,[Name] 
  FROM [Tuhu_groupon].[dbo].[SE_BeautyHomePageConfig] WITH(NOLOCK)
  WHERE [Type]=1 AND [Status]=1  AND Channel=@channel";
            var sqlParameters = new SqlParameter[]
         {
                new SqlParameter("@channel",channel)
         };
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<BeautyCategorySimple>().ToList();
        }

        public static IEnumerable<BeautyPopUpWindowsRegionModel> GetBeautyPopUpWindowsRegionConfigs(int BeautyPopUpId)
        {
            const string sql = @"SELECT [BeautyPopUpId]
      ,[ProvinceId]
      ,[CityId]
      ,[IsAllCity]
  FROM [Tuhu_groupon].[dbo].[SE_BeautyPopUpWindowsRegionConfig] With(Nolock)
  Where BeautyPopUpId=@BeautyPopUpId";
            using (var conn = new SqlConnection(strConn))
            {
                var dt = SqlHelper.ExecuteDataTable(conn,
                   CommandType.Text,
                   sql,
                   new SqlParameter[] { new SqlParameter("@BeautyPopUpId", BeautyPopUpId) });
                var result = dt.Rows.Cast<DataRow>().Select(row => new BeautyPopUpWindowsRegionModel
                {
                    BeautyPopUpId = Convert.ToInt32(row["BeautyPopUpId"]),
                    ProvinceId = Convert.ToInt32(row["ProvinceId"]),
                    CityId = row["CityId"]?.ToString(),
                    IsAllCity = Convert.ToBoolean(row["IsAllCity"]),
                });
                return result;
            }


        }
        public static bool InsertBeautyPopUpWindowsRegionConfigs(SqlTransaction conn, IEnumerable<BeautyPopUpWindowsRegionModel> models, bool isInsert = false)
        {
            #region sql
            const string sql = @"Insert into [Tuhu_groupon].[dbo].[SE_BeautyPopUpWindowsRegionConfig](
[BeautyPopUpId]
,[ProvinceId]
,[CityId]
,[IsAllCity]
,[CreateTime]
,[UpdateTime]
)Values(
@BeautyPopUpId,
@ProvinceId,
@CityId,
@IsAllCity,
GetDate(),
GetDate()
)";
            const string sql_del = @"Delete from  [Tuhu_groupon].[dbo].[SE_BeautyPopUpWindowsRegionConfig] 
 WHERE BeautyPopUpId=@BeautyPopUpId";
            #endregion
            var result = true;
            var del = true;
            if (!isInsert)
                del = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql_del, new SqlParameter[] { new SqlParameter("@BeautyPopUpId", models.FirstOrDefault().BeautyPopUpId) }) > 0;
            foreach (var item in models)
            {
                var temp = SqlHelper.ExecuteNonQuery(conn,
CommandType.Text,
sql,
new SqlParameter[] { new SqlParameter("@BeautyPopUpId", item.BeautyPopUpId),
   new SqlParameter("@ProvinceId", item.ProvinceId) ,
   new SqlParameter("@IsAllCity", item.IsAllCity) ,
   new SqlParameter("@CityId",item.IsAllCity? null: item.CityId) });
                if (temp <= 0)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
    }
}
