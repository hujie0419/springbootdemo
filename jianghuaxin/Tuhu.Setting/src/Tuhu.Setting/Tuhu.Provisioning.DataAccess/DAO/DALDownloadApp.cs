using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALDownloadApp
    {
        public static bool InsertDownloadApp(SqlConnection conn, DownloadApp model)
        {

            string sql = @"   INSERT INTO Tuhu_huodong..DownloadApp
                                      ( Title ,
                                        BottomContent ,
                                        ImageContent ,
                                        TimerContent ,
                                        BottomStatus ,
                                        TimerStatus ,
                                        CreateTime  ,
                                        UpdateTime  ,Type                                 
                                      )
                              VALUES  ( @Title ,
                                        @BottomContent ,
                                        @ImageContent ,
                                        @TimerContent ,
                                        @BottomStatus ,
                                        @TimerStatus ,
                                        GETDATE() , -- CreateTime - datetime
                                        GETDATE() ,@Type
                                      )";

            var sqlPrams = new SqlParameter[]
            {
               new SqlParameter("@Title",model.Title??string.Empty),
               new SqlParameter("@BottomContent",model.BottomContent??string.Empty),
               new SqlParameter("@ImageContent",model.ImageContent??string.Empty),
               new SqlParameter("@BottomStatus",model.BottomStatus),
               new SqlParameter("@TimerStatus",model.TimerStatus),
               new SqlParameter("@TimerContent",model.TimerContent??string.Empty),
               new SqlParameter("@Type",model.Type)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrams) > 0;
        }

        public static bool DeleteDownloadApp(SqlConnection conn, int id)
        {
            string sql = @"DELETE FROM [Tuhu_huodong].[dbo].[DownloadApp] WHERE Id=@id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@id", id)) > 0;
        }

        public static bool UpdateDownloadApp(SqlConnection conn, DownloadApp model, int id)
        {
            string sql = @"UPDATE    Tuhu_huodong..DownloadApp
                          SET       Title = @Title ,
                                    BottomContent = @BottomContent ,
                                    ImageContent = @ImageContent ,
                                    TimerContent = @TimerContent ,
                                    BottomStatus = @BottomStatus ,
                                    TimerStatus = @TimerStatus ,
                                    Type = @Type,
                                    UpdateTime = GETDATE()
                          WHERE     Id = @id";
            var sqlPrams = new SqlParameter[]
            {
               new SqlParameter("@Title",model.Title??string.Empty),
               new SqlParameter("@BottomContent",model.BottomContent??string.Empty),
               new SqlParameter("@ImageContent",model.ImageContent??string.Empty),
               new SqlParameter("@BottomStatus",model.BottomStatus),
               new SqlParameter("@TimerStatus",model.TimerStatus),
               new SqlParameter("@TimerContent",model.TimerContent??string.Empty),
               new SqlParameter("@id",id),
               new SqlParameter("@Type",model.Type)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrams) > 0;
        }

        public static DownloadApp GetDownloadAppById(SqlConnection conn, int id)
        {
            string sql = @"SELECT [Id]
                                  ,[Title]
                                  ,[BottomContent]
                                  ,[ImageContent]
                                  ,[TimerContent]
                                  ,BottomStatus
                                  ,TimerStatus
                                  ,[CreateTime]
                                  ,[UpdateTime],Type
                              FROM [Tuhu_huodong].[dbo].[DownloadApp] WITH(NOLOCK) WHERE id=@id";

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, new SqlParameter("@id", id)).ConvertTo<DownloadApp>().FirstOrDefault();
        }

        public static List<DownloadApp> GetDownloadAppList(SqlConnection conn, DownloadApp model, int pageSize, int pageIndex, out int recordCount)
        {
            string sqlStr = "";
            if (model.Id > 0)
            {
                sqlStr += " And Id = @Id";
            }
            if (!string.IsNullOrWhiteSpace(model.Title))
            {
                sqlStr += " And Title =@Title";
            }
            string sql = @" 
                            SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY UpdateTime DESC ) AS ROWNUMBER ,
                                                [Id] ,
                                                [Title] ,
                                                [BottomContent] ,
                                                [ImageContent] ,
                                                [TimerContent] ,
                                                BottomStatus ,
                                                TimerStatus ,
                                                [CreateTime] ,
                                                [UpdateTime],Type
                                      FROM      [Tuhu_huodong].[dbo].[DownloadApp] WITH ( NOLOCK )
                                      WHERE     1 = 1  " + sqlStr + @"
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)
                            ";
            string sqlCount = @"SELECT COUNT(1) FROM [Tuhu_huodong].[dbo].[DownloadApp]  WITH(NOLOCK)  WHERE 1=1    " + sqlStr;

            var sqlPrams = new SqlParameter[]
             {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@Id",model.Id),
                new SqlParameter("@Title",model.Title)
             };
            recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount, sqlPrams);                   

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlPrams).ConvertTo<DownloadApp>().ToList();
        }

        public static bool InsertBatteryBanner(SqlConnection conn, BatteryBanner model)
        {
            const string sql = @" 	INSERT INTO Tuhu_huodong..BatteryBanner
			                                ( SmallImage ,
			                                  Image ,
			                                  ShowTime ,
			                                  Province ,
			                                  City ,
                                              Display ,
                                              Available
			                                )
	                                VALUES  (@SmallImage , -- SmallImage - nvarchar(1000)
			                                 @Image , -- Image - nvarchar(1000)
			                                 @ShowTime , -- ShowTime - datetime
			                                 @Province , -- Province - nvarchar(50)
			                                 @City ,  -- City - nvarchar(50) 
			                                 @Display ,
			                                 @Available 
			                                )";

            var sqlPrams = new SqlParameter[]
            {
                new SqlParameter("@Image",model.Image??string .Empty),
                new SqlParameter("@SmallImage",model.SmallImage??string.Empty),
                new SqlParameter("@ShowTime",model.ShowTime),
                new SqlParameter("@Province",model.Province??string.Empty),
                new SqlParameter("@City",model.City??string.Empty),
                new SqlParameter("@Display",model.Display),
                new SqlParameter("@Available",model.Available),
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrams) > 0;

        }

        public static List<BatteryBanner> GetBatteryBanner(SqlConnection conn, string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @" SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY Id DESC ) AS ROWNUMBER ,
                                                *
                                      FROM      [Tuhu_huodong].[dbo].[BatteryBanner] WITH ( NOLOCK )
                                      WHERE     1 = 1   " + sqlStr + @"
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)

                             ";

            string sqlCount = @"SELECT COUNT(1) FROM [Tuhu_huodong].[dbo].[BatteryBanner] WITH(NOLOCK)  WHERE 1=1  " + sqlStr;
            recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount);

            var sqlPrams = new SqlParameter[]
                  {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
                 };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlPrams).ConvertTo<BatteryBanner>().ToList();
        }

        public static List<Region> GetRegion(SqlConnection conn, int id)
        {
            const string sql = @"SELECT [PKID]
                                      ,[RegionName]	    
	                                 , ParentID
                                  FROM [Gungnir].[dbo].[tbl_region] WITH(NOLOCK) WHERE ParentID = @id ";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, new SqlParameter("@id", id)).ConvertTo<Region>().ToList();
        }
    }
}
