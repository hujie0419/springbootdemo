using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalHomePagePopup
    {
        public static int InsertHomePagePopup(SqlConnection conn, HomePagePopup model)
        {
            const string sql1 =
                @"SELECT  COUNT(0) FROM Configuration.dbo.HomePagePopupConfig WITH ( NOLOCK ) WHERE Sort = @Sort ";

            var sqlParam1 = new[]
            {
                new SqlParameter("@Sort", model.Sort)
            };

            var count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql1, sqlParam1);
            if (count > 0)
            {
                return -1;
            }

            const string sql = @"INSERT INTO Configuration.dbo.HomePagePopupConfig
                                        (Description, ImageUrl, LinkUrl, Channel, Position, TargetGroups, Sort, Period, StartVersion, EndVersion,
                                         StartDateTime, EndDateTime, IsEnabled, CreateTime, Creator, NoticeChannel, NewNoticeChannel)
                                    VALUES(
                                             @Description,
                                             @ImageURL,
                                             @LinkUrl,
                                             @Channel,
                                             @Position,
                                             @TargetGroups,
                                             @Sort, 
                                             @Period,
                                             @StartVersion,
                                             @EndVersion,
                                             @StartDateTime, 
                                             @EndDateTime, 
                                             @IsEnabled, 
                                             GETDATE(),
                                             @Creator, 
                                             @NoticeChannel,
                                             @NewNoticeChannel
                                          ); SELECT @@IDENTITY;";

            var sqlParam = new[]
            {
                new SqlParameter("@Description", model.Description),
                new SqlParameter("@ImageUrl", model.ImageUrl),
                new SqlParameter("@LinkUrl", model.LinkUrl),
                new SqlParameter("@Channel", model.Channel),
                new SqlParameter("@Position", model.Position),
                new SqlParameter("@TargetGroups", model.TargetGroups),
                new SqlParameter("@Sort", model.Sort),
                new SqlParameter("@Period", model.Period),
                new SqlParameter("@StartVersion", model.StartVersion),
                new SqlParameter("@EndVersion", model.EndVersion),
                new SqlParameter("@StartDateTime", model.StartDateTime),
                new SqlParameter("@EndDateTime", model.EndDateTime),
                new SqlParameter("@IsEnabled", model.IsEnabled),
                new SqlParameter("@Creator", model.Creator),
                new SqlParameter("@NoticeChannel", model.NoticeChannel),
                new SqlParameter("@NewNoticeChannel", model.NewNoticeChannel)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParam));
        }
        public static int UpdateHomePagePopup(SqlConnection conn, HomePagePopup model)
        {
            const string sql1 =
                @"SELECT PKID FROM Configuration.dbo.HomePagePopupConfig WITH ( NOLOCK ) WHERE Sort = @Sort ";

            var sqlParam1 = new[]
            {
                new SqlParameter("@Sort", model.Sort)
            };

            var pkid = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql1, sqlParam1);
            if (pkid != null && (int)pkid != model.PKID)
            {
                return -1;
            }
            const string sql = @"UPDATE Configuration.dbo.HomePagePopupConfig 
                                    SET Description=@Description, 
                                        ImageUrl=@ImageUrl,
                                        LinkUrl=@LinkUrl, 
                                        Channel=@Channel, 
                                        Position=@Position, 
                                        TargetGroups=@TargetGroups, 
                                        Sort=@Sort, 
                                        Period=@Period, 
                                        StartVersion=@StartVersion, 
                                        EndVersion=@EndVersion,
                                        StartDateTime=@StartDateTime, 
                                        EndDateTime=@EndDateTime, 
                                        IsEnabled=@IsEnabled,
                                        NoticeChannel=@NoticeChannel,
                                        NewNoticeChannel=@NewNoticeChannel
                                        WHERE PKID=@PKID";
            var sqlParam = new[]
                       {
                new SqlParameter("@PKID", model.PKID),
                new SqlParameter("@Description", model.Description),
                new SqlParameter("@ImageUrl", model.ImageUrl),
                new SqlParameter("@LinkUrl", model.LinkUrl),
                new SqlParameter("@Channel", model.Channel),
                new SqlParameter("@Position", model.Position),
                new SqlParameter("@TargetGroups", model.TargetGroups),
                new SqlParameter("@Sort", model.Sort),
                new SqlParameter("@Period", model.Period),
                new SqlParameter("@StartVersion", model.StartVersion),
                new SqlParameter("@EndVersion", model.EndVersion),
                new SqlParameter("@StartDateTime", model.StartDateTime),
                new SqlParameter("@EndDateTime", model.EndDateTime),
                new SqlParameter("@IsEnabled", model.IsEnabled),
                new SqlParameter("@NoticeChannel", model.NoticeChannel),
                new SqlParameter("@NewNoticeChannel", model.NewNoticeChannel)
                       };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam);
        }

        public static List<NoticeChannel> QueryNoticeChannel(SqlConnection conn)
        {
            string sql =
                @"SELECT * FROM Configuration.dbo.NoticeChannelConfig WITH ( NOLOCK )";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<NoticeChannel>().ToList();
        }

        public static List<HomePagePopup> QueryHomePagePopup(SqlConnection conn, HomePagePopupQuery queryModel, int page)
        {
            var flag = false;
            string sql =
                @"SELECT * FROM Configuration.dbo.HomePagePopupConfig WITH ( NOLOCK )";
            if (!string.IsNullOrWhiteSpace(queryModel.DescriptionCriterion))
            {
                sql += @" WHERE Description LIKE N'%" + queryModel.DescriptionCriterion + @"%'";
                flag = true;
            }
            if (queryModel.ChannelCriterion != 0)
            {
                sql += (flag ? " AND " : " WHERE ") + @"Channel=" + queryModel.ChannelCriterion;
                flag = true;
            }
            if (!string.IsNullOrWhiteSpace(queryModel.TargetGroupsCriterion))
            {
                sql += (flag ? " AND " : " WHERE ") + @"N'"+ queryModel.TargetGroupsCriterion + @"' IN ( SELECT  * FROM  Configuration.dbo.SplitString(TargetGroups, ',', 1))";
                flag = true;
            }
            if (!string.IsNullOrWhiteSpace(queryModel.StartVersionCriterion))
            {
                sql += (flag ? " AND " : " WHERE ") + @"StartVersion='" + queryModel.StartVersionCriterion + @"'";
                flag = true;
            }
            if (!string.IsNullOrWhiteSpace(queryModel.EndVersionCriterion))
            {
                sql += (flag ? " AND " : " WHERE ") + @"EndVersion='" + queryModel.EndVersionCriterion + @"'";
                flag = true;
            }
            if (queryModel.VisibleCriterion != 0)
            {
                if (queryModel.VisibleCriterion == 1)
                    sql += (flag ? " AND " : " WHERE ") + @"StartDateTime<=GETDATE() AND EndDateTime>GETDATE() AND IsEnabled=1";
                else
                    sql += (flag ? " AND " : " WHERE ") + @"(IsEnabled=0 OR StartDateTime>GETDATE() OR EndDateTime<=GETDATE())";
                flag = true;
            }
            if (!string.IsNullOrWhiteSpace(queryModel.CreatorCriterion))
            {
                sql += (flag ? " AND " : " WHERE ") + @"Creator LIKE N'%" + queryModel.CreatorCriterion + @"%'";
            }

            switch (queryModel.OrderCriterion)
            {
                case 1:
                    sql += @" ORDER BY PKID";
                    break;
                case 2:
                    sql += @" ORDER BY Sort DESC";
                    break;
                case 3:
                    sql += @" ORDER BY Sort";
                    break;
                case 4:
                    sql += @" ORDER BY Period DESC";
                    break;
                case 5:
                    sql += @" ORDER BY Period";
                    break;
                case 6:
                    sql += @" ORDER BY CreateTime DESC";
                    break;
                case 7:
                    sql += @" ORDER BY CreateTime";
                    break;
                default:
                    sql += @" ORDER BY PKID DESC";
                    break;
            }
            sql += @" OFFSET " + (page - 1) * 50 + " ROWS FETCH NEXT 50 ROWS ONLY";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<HomePagePopup>().ToList();
        }

        public static int CountHomePagePopup(SqlConnection conn, HomePagePopupQuery queryModel)
        {
            var flag = false;
            string sql =
                @"SELECT Count(0) FROM Configuration.dbo.HomePagePopupConfig WITH ( NOLOCK )";
            if (!string.IsNullOrWhiteSpace(queryModel.DescriptionCriterion))
            {
                sql += @" WHERE Description LIKE '%" + queryModel.DescriptionCriterion + @"%'";
                flag = true;
            }
            if (queryModel.ChannelCriterion != 0)
            {
                sql += (flag ? " AND " : " WHERE ") + @"Channel=" + queryModel.ChannelCriterion;
                flag = true;
            }
            if (!string.IsNullOrWhiteSpace(queryModel.TargetGroupsCriterion))
            {
                sql += (flag ? " AND " : " WHERE ") + @"N'" + queryModel.TargetGroupsCriterion + @"' IN ( SELECT  * FROM  Configuration.dbo.SplitString(TargetGroups, ',', 1))";
                flag = true;
            }
            if (!string.IsNullOrWhiteSpace(queryModel.StartVersionCriterion))
            {
                sql += (flag ? " AND " : " WHERE ") + @"StartVersion LIKE '%" + queryModel.StartVersionCriterion + @"%'";
                flag = true;
            }
            if (!string.IsNullOrWhiteSpace(queryModel.EndVersionCriterion))
            {
                sql += (flag ? " AND " : " WHERE ") + @"EndVersion LIKE '%" + queryModel.EndVersionCriterion + @"%'";
                flag = true;
            }
            if (queryModel.VisibleCriterion != 0)
            {
                if (queryModel.VisibleCriterion == 1)
                    sql += (flag ? " AND " : " WHERE ") + @"StartDateTime<=GETDATE() AND EndDateTime>GETDATE() AND IsEnabled=1";
                else
                    sql += (flag ? " AND " : " WHERE ") + @"(IsEnabled=0 OR StartDateTime>GETDATE() OR EndDateTime<=GETDATE())";
                flag = true;
            }
            if (!string.IsNullOrWhiteSpace(queryModel.CreatorCriterion))
            {
                sql += (flag ? " AND " : " WHERE ") + @"Creator LIKE '%" + queryModel.CreatorCriterion + @"%'";
            }

            return (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
        }

        public static List<HomePagePopupAnimation> GetAnimation(SqlConnection conn,int pkid)
        {
            const string sql = @"SELECT * FROM [Configuration].[dbo].[HomePagePopupAnimationConfig] WITH ( NOLOCK ) WHERE PopupConfigId=@pkid";
            //@"SELECT PKId,PopupConfigId,ImageUrl,ImageWidth,ImageHeight,LeftMargin,TopMargin,ZIndex,LinkUrl,MovementType=case MovementType when @num0 then '无动画效果' when @num1 then '下落加旋转'when @num2 then '斜向下落'when @num3 then '左右摇摆'when @num4 then '变大变小'when @num5 then '延迟变大变小'when @num6 then '上下移动' end FROM [Configuration].[dbo].[HomePagePopupAnimationConfig] WITH ( NOLOCK ) WHERE PopupConfigId=@pkid";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, new SqlParameter[]{
                //new SqlParameter("@num0",0),
                //new SqlParameter("@num1",1),
                //new SqlParameter("@num2",2),
                //new SqlParameter("@num3",3),
                //new SqlParameter("@num4",4),
                //new SqlParameter("@num5",5),
                //new SqlParameter("@num6",6),
                new SqlParameter("@pkid",pkid)
            }).ConvertTo<HomePagePopupAnimation>().ToList();
        }
        //GetTargetGroup
        public static List<HomePagePopupTargetGroup> GetTargetGroup(SqlConnection conn)
        {
            const string sql = @"SELECT * FROM [Configuration].[dbo].[HomePopUserGroup] WITH ( NOLOCK )";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<HomePagePopupTargetGroup>().ToList();
        }
    }
}
