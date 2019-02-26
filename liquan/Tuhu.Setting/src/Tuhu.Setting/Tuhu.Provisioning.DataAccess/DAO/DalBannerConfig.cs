using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalBannerConfig
    {
        public static int InsertBannerConfig(SqlConnection conn, BannerConfig model)
        {
            const string sql = @"
INSERT  INTO Configuration.dbo.PersonalCenterConfig ( DESCRIPTION, Route,
                                                      ShareType, Image,
                                                      Channel, Location,
                                                      NoticeChannels,
                                                      StartDateTime,
                                                      EndDateTime, Status,
                                                      TargetGroups, Grade,
                                                      VIPAuthorizationRuleId,
                                                      StartVersion, EndVersion,
                                                      Sort, CreateTime,
                                                      Creator, Link,
                                                      IOSProcessValue,
                                                      AndroidProcessValue,
                                                      IOSCommunicationValue,
                                                      AndroidCommunicationValue,
                                                      IsNew, TargetSmallAppId,
                                                      TargetSmallAppUrl )
VALUES  ( @Description, @Route, @ShareType, @Image, @Channel, @Location,
          @NoticeChannels, @StartDateTime, @EndDateTime, @Status,
          @TargetGroups, @Grade, @VIPAuthorizationRuleId, @StartVersion,
          @EndVersion, @Sort, GETDATE(), @Creator, @Link, @IOSProcessValue,
          @AndroidProcessValue, @IOSCommunicationValue,
          @AndroidCommunicationValue, 1, @TargetSmallAppId, @TargetSmallAppUrl );
SELECT  @@IDENTITY;";

            var sqlParam = new[]
            {
                new SqlParameter("@Description", model.Description),
                new SqlParameter("@Route", model.Route),
                new SqlParameter("@ShareType", model.ShareType),
                new SqlParameter("@Image", model.Image),
                new SqlParameter("@Channel", model.Channel),
                new SqlParameter("@Location", model.Location),
                new SqlParameter("@NoticeChannels", model.NoticeChannels),
                new SqlParameter("@StartDateTime", model.StartDateTime),
                new SqlParameter("@EndDateTime", model.EndDateTime),
                new SqlParameter("@Status", model.Status),
                new SqlParameter("@TargetGroups", model.TargetGroups),
                new SqlParameter("@Grade", model.Grade),
                new SqlParameter("@VIPAuthorizationRuleId", model.VIPAuthorizationRuleId),
                new SqlParameter("@StartVersion", model.StartVersion),
                new SqlParameter("@EndVersion", model.EndVersion),
                new SqlParameter("@Sort", model.Sort),
                new SqlParameter("@Creator", model.Creator),
                new SqlParameter("@Link", model.Link),
                new SqlParameter("@IOSProcessValue", model.IOSProcessValue),
                new SqlParameter("@AndroidProcessValue", model.AndroidProcessValue),
                new SqlParameter("@IOSCommunicationValue", model.IOSCommunicationValue),
                new SqlParameter("@AndroidCommunicationValue", model.AndroidCommunicationValue),
                new SqlParameter("@TargetSmallAppId", model.TargetSmallAppId),
                new SqlParameter("@TargetSmallAppUrl", model.TargetSmallAppUrl),
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParam));
        }

        public static int UpdateBannerConfig(SqlConnection conn, BannerConfig model)
        {
            const string sql = @"UPDATE Configuration.dbo.PersonalCenterConfig 
                                    SET Description=@Description, 
                                        Route=@Route,
                                        ShareType=@ShareType,
                                        Image=@Image,
                                        Channel=@Channel,
                                        Location=@Location, 
                                        NoticeChannels=@NoticeChannels,
                                        StartDateTime=@StartDateTime, 
                                        EndDateTime=@EndDateTime,
                                        Status=@Status,
                                        TargetGroups=@TargetGroups, 
                                        Grade=@Grade, 
                                        VIPAuthorizationRuleId = @VIPAuthorizationRuleId,
                                        StartVersion=@StartVersion, 
                                        EndVersion=@EndVersion,
                                        Sort=@Sort,
                                        Link=@Link,
                                        IOSProcessValue=@IOSProcessValue,
                                        AndroidProcessValue=@AndroidProcessValue,
                                        IOSCommunicationValue=@IOSCommunicationValue,
                                        AndroidCommunicationValue=@AndroidCommunicationValue,
                                        TargetSmallAppId=@TargetSmallAppId,
                                        TargetSmallAppUrl=@TargetSmallAppUrl
                                        WHERE Id=@Id";
            var sqlParam = new[]
                       {
                new SqlParameter("@Description", model.Description),
                new SqlParameter("@Route", model.Route),
                new SqlParameter("@ShareType", model.ShareType),
                new SqlParameter("@Image", model.Image),
                new SqlParameter("@Channel", model.Channel),
                new SqlParameter("@Location", model.Location),
                new SqlParameter("@NoticeChannels", model.NoticeChannels),
                new SqlParameter("@StartDateTime", model.StartDateTime),
                new SqlParameter("@EndDateTime", model.EndDateTime),
                new SqlParameter("@Status", model.Status),
                new SqlParameter("@TargetGroups", model.TargetGroups),
                new SqlParameter("@Grade", model.Grade),
                new SqlParameter("@VIPAuthorizationRuleId", model.VIPAuthorizationRuleId),
                new SqlParameter("@StartVersion", model.StartVersion),
                new SqlParameter("@EndVersion", model.EndVersion),
                new SqlParameter("@Sort", model.Sort),
                new SqlParameter("@Link", model.Link),
                new SqlParameter("@IOSProcessValue", model.IOSProcessValue),
                new SqlParameter("@AndroidProcessValue", model.AndroidProcessValue),
                new SqlParameter("@IOSCommunicationValue", model.IOSCommunicationValue),
                new SqlParameter("@AndroidCommunicationValue", model.AndroidCommunicationValue),
                new SqlParameter("@TargetSmallAppId", model.TargetSmallAppId),
                new SqlParameter("@TargetSmallAppUrl", model.TargetSmallAppUrl),
                new SqlParameter("@Id", model.Id)
                       };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam);
        }

        public static List<BannerConfig> QueryBannerConfig(SqlConnection conn, BannerFilterQuery queryModel, int page)
        {
            string sql = @"SELECT * FROM Configuration.dbo.PersonalCenterConfig WITH ( NOLOCK ) WHERE IsNew=1";
            if (queryModel.IDCriterion != null)
            {
                sql += @" AND Id=" + queryModel.IDCriterion.Value;
            }
            if (!string.IsNullOrWhiteSpace(queryModel.DescriptionCriterion))
            {
                sql += @" AND Description LIKE N'%" + queryModel.DescriptionCriterion + @"%'";
            }
            if (queryModel.ChannelCriterion != 0)
            {
                sql += @" AND Channel=" + queryModel.ChannelCriterion;
            }
            if (queryModel.LocationCriterion != 0)
            {
                sql += @" AND Location=" + queryModel.LocationCriterion;
            }
            if (queryModel.StatusCriterion != null)
            {
                sql += (queryModel.StatusCriterion.Value ? @" AND Status=1" : @" AND Status=0");
            }
            if (!string.IsNullOrWhiteSpace(queryModel.StartVersionCriterion))
            {
                sql += @" AND StartVersion='" + queryModel.StartVersionCriterion + @"'";
            }
            if (!string.IsNullOrWhiteSpace(queryModel.EndVersionCriterion))
            {
                sql += @" AND EndVersion='" + queryModel.EndVersionCriterion + @"'";
            }
            if (!string.IsNullOrWhiteSpace(queryModel.TargetGroupsCriterion))
            {
                sql += @" AND N'" + queryModel.TargetGroupsCriterion + @"' IN ( SELECT  * FROM  Configuration.dbo.SplitString(TargetGroups, ',', 1))";
            }
            if (queryModel.SortCriterion != null)
            {
                sql += @" AND Sort=" + queryModel.SortCriterion.Value;
            }
            if (queryModel.StartDateCriterion != null)
            {
                sql += @" AND CreateTime>='" + queryModel.StartDateCriterion.Value + @"'";
            }
            if (queryModel.EndDateCriterion != null)
            {
                sql += @" AND CreateTime<='" + queryModel.EndDateCriterion.Value + @"'";
            }
            if (!string.IsNullOrWhiteSpace(queryModel.CreatorCriterion))
            {
                sql += @" AND Creator LIKE N'%" + queryModel.CreatorCriterion + @"%'";
            }

            if (!string.IsNullOrWhiteSpace(queryModel.TargetSmallAppIdCriterion))
            {
                sql += $" AND TargetSmallAppId = N'{queryModel.TargetSmallAppIdCriterion}'";
            }

            sql += @"ORDER BY Id desc OFFSET (@page - 1) * 50 ROWS FETCH NEXT 50 ROWS ONLY";
            var sqlParam = new[] {new SqlParameter("@page", page)};
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<BannerConfig>().ToList();
        }

        public static int CountBannerConfig(SqlConnection conn, BannerFilterQuery queryModel)
        {
            
            string sql = @"SELECT Count(0) FROM Configuration.dbo.PersonalCenterConfig WITH ( NOLOCK ) WHERE IsNew=1";
            if (queryModel.IDCriterion != null)
            {
                sql += @" AND Id=" + queryModel.IDCriterion.Value;
            }
            if (!string.IsNullOrWhiteSpace(queryModel.DescriptionCriterion))
            {
                sql += @" AND Description LIKE N'%" + queryModel.DescriptionCriterion + @"%'";
            }
            if (queryModel.ChannelCriterion != 0)
            {
                sql += @" AND Channel=" + queryModel.ChannelCriterion;
            }
            if (queryModel.LocationCriterion != 0)
            {
                sql += @" AND Location=" + queryModel.LocationCriterion;
            }
            if (queryModel.StatusCriterion != null)
            {
                sql += (queryModel.StatusCriterion.Value ? @" AND Status=1" : @" AND Status=0");
            }
            if (!string.IsNullOrWhiteSpace(queryModel.StartVersionCriterion))
            {
                sql += @" AND StartVersion='" + queryModel.StartVersionCriterion + @"'";
            }
            if (!string.IsNullOrWhiteSpace(queryModel.EndVersionCriterion))
            {
                sql += @" AND EndVersion='" + queryModel.EndVersionCriterion + @"'";
            }
            if (!string.IsNullOrWhiteSpace(queryModel.TargetGroupsCriterion))
            {
                sql += @" AND N'" + queryModel.TargetGroupsCriterion + @"' IN ( SELECT  * FROM  Configuration.dbo.SplitString(TargetGroups, ',', 1))";
            }
            if (queryModel.SortCriterion != null)
            {
                sql += @" AND Sort=" + queryModel.SortCriterion.Value;
            }
            if (queryModel.StartDateCriterion != null)
            {
                sql += @" AND CreateTime>='" + queryModel.StartDateCriterion.Value + @"'";
            }
            if (queryModel.EndDateCriterion != null)
            {
                sql += @" AND CreateTime<='" + queryModel.EndDateCriterion.Value + @"'";
            }
            if (!string.IsNullOrWhiteSpace(queryModel.CreatorCriterion))
            {
                sql += @" AND Creator LIKE N'%" + queryModel.CreatorCriterion + @"%'";
            }
            var obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            if (obj != null)
                return Convert.ToInt32(obj);
            return 0;
        }

        public static bool DeleteBannerConfig(SqlConnection conn, int Id)
        {
            var sql = @"DELETE FROM Configuration.dbo.PersonalCenterConfig WHERE Id = @Id";
            var sqlParam = new[] { new SqlParameter("@Id", Id) };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0;
        }
    }
}
