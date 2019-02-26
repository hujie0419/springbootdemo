using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Tire;

namespace Tuhu.Provisioning.DataAccess.DAO.Tire
{
    public class DalTireSpecParamsConfig
    {
        public static List<TireSpecParamsEditLog> SelectTireSpecParamsEditLog(SqlConnection conn, string pid)
        {
            string sql = @"select * from [Configuration].dbo.tbl_TireSpecParamsEditLog WITH ( NOLOCK)
                            where PId=@pid";
            var sqlParam = new[]
                {
                    new SqlParameter("@pid",pid),
                };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam)
                .ConvertTo<TireSpecParamsEditLog>()
                .ToList();
        }

        public static List<TireSpecParamsModel> QueryTireSpecParamsModel(SqlConnection conn, TireSpecParamsConfigQuery query)
        {
            string countSql = @"SELECT Count(1) 
                            FROM Tuhu_productcatalog..vw_Products as vwP WITH ( NOLOCK) 
                            left join [Configuration].dbo.tbl_TireSpecParamsConfig as tsPC with (nolock)
                            on vwP.PID=tsPC.PId
                           WHERE ";
            string sql = @"SELECT vwP.PID as ProductID,DisplayName,TireSize,tsPC.* 
                            FROM Tuhu_productcatalog..vw_Products as vwP WITH ( NOLOCK) 
                            left join [Configuration].dbo.tbl_TireSpecParamsConfig as tsPC with (nolock)
                            on vwP.PID=tsPC.PId
                           WHERE ";
            string addsql;
            if (string.IsNullOrWhiteSpace(query.PIDCriterion))
            {
                if (string.IsNullOrWhiteSpace(query.DisplayNameCriterion))
                    addsql = @"vwP.PID like 'TR%'";
                else
                    addsql = @"vwP.PID like 'TR%' and vwP.DisplayName like N'%" + query.DisplayNameCriterion + "%'";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(query.DisplayNameCriterion))
                    addsql = @"vwP.PID like 'TR%' and vwP.PID like N'%" + query.PIDCriterion + "%'";
                else
                    addsql = @"vwP.PID like 'TR%' and vwP.PID like N'%" + query.PIDCriterion + "%' and vwP.DisplayName like N'%" + query.DisplayNameCriterion + "%'";
            }

            sql = sql + addsql +
                @" ORDER BY vwP.PID ASC OFFSET @pagesdata ROWS FETCH NEXT @pagedataquantity ROWS ONLY";
            countSql = countSql + addsql;

            var sqlParam = new[]
                {
                    new SqlParameter("@pagesdata",(query.PageIndex-1)*query.PageDataQuantity),
                    new SqlParameter("@pagedataquantity",query.PageDataQuantity),
                };

            query.TotalCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, countSql, sqlParam);
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<TireSpecParamsModel>().ToList();
        }

        public static TireSpecParamsModel SelectTireSpecParamsModelByPid(SqlConnection conn, string pid)
        {
            string sql = @"SELECT vwP.PID as ProductID,DisplayName,TireSize,tsPC.* 
                            FROM Tuhu_productcatalog..vw_Products as vwP WITH ( NOLOCK) 
                            left join [Configuration].dbo.tbl_TireSpecParamsConfig as tsPC with (nolock)
                            on vwP.PID=tsPC.PId
                            WHERE vwP.PID=@pid";
            var sqlParam = new[]
            {
                new SqlParameter("pid", pid),
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<TireSpecParamsModel>().ToList().FirstOrDefault();
        }

        public static bool InsertTireSpecParamsConfig(SqlConnection conn, TireSpecParamsConfig config)
        {
            string sql = @"INSERT INTO [Configuration].[dbo].[tbl_TireSpecParamsConfig]
           ([PId]
           ,[QualityInspectionName]
           ,[OriginPlace]
           ,[RimProtection]
           ,[TireLoad]
           ,[MuddyAndSnow]
           ,[ThreeT_Treadwear]
           ,[ThreeT_Traction]
           ,[ThreeT_Temperature]
           ,[TireCrown_Polyester]
           ,[TireCrown_Steel]
           ,[TireCrown_Nylon]
           ,[TireSideWall_Polyester]
           ,[TireLable_RollResistance]
           ,[TireLable_WetGrip]
           ,[TireLable_Noise]
           ,[PatternSymmetry]
           ,[TireGuideRotation]
           ,[FactoryCode] 
           ,[GrooveNum]
           ,[Remark]
           ,[CreateTime]
           ,[LastUpdateDataTime])
     VALUES
           (@PId
           ,@QualityInspectionName
           ,@OriginPlace
           ,@RimProtection
           ,@TireLoad
           ,@MuddyAndSnow
           ,@ThreeT_Treadwear
           ,@ThreeT_Traction
           ,@ThreeT_Temperature
           ,@TireCrown_Polyester
           ,@TireCrown_Steel
           ,@TireCrown_Nylon
           ,@TireSideWall_Polyester
           ,@TireLable_RollResistance
           ,@TireLable_WetGrip
           ,@TireLable_Noise
           ,@PatternSymmetry
           ,@TireGuideRotation
           ,@FactoryCode
           ,@GrooveNum
           ,@Remark
           ,GETDATE()
           ,GETDATE())";
            int? RimProtection = null;
            if (config.RimProtection.HasValue)
            {
                if (config.RimProtection == true) RimProtection = 1;
                else RimProtection = 0;
            }

            int? MuddyAndSnow = null;
            if (config.MuddyAndSnow.HasValue)
            {
                if (config.MuddyAndSnow == true) MuddyAndSnow = 1;
                else MuddyAndSnow = 0;
            }

            var sqlParam = new[]
            {
                new SqlParameter("@PId", config.PId),
                new SqlParameter("@QualityInspectionName", config.QualityInspectionName),
                new SqlParameter("@OriginPlace", config.OriginPlace),
                new SqlParameter("@RimProtection", RimProtection),
                new SqlParameter("@TireLoad", config.TireLoad),
                new SqlParameter("@MuddyAndSnow", MuddyAndSnow),
                new SqlParameter("@ThreeT_Treadwear", config.ThreeT_Treadwear),
                new SqlParameter("@ThreeT_Traction", config.ThreeT_Traction),
                new SqlParameter("@ThreeT_Temperature", config.ThreeT_Temperature),
                new SqlParameter("@TireCrown_Polyester", config.TireCrown_Polyester),
                new SqlParameter("@TireCrown_Steel", config.TireCrown_Steel),
                new SqlParameter("@TireCrown_Nylon", config.TireCrown_Nylon),
                new SqlParameter("@TireSideWall_Polyester", config.TireSideWall_Polyester),
                new SqlParameter("@TireLable_RollResistance", config.TireLable_RollResistance),
                new SqlParameter("@TireLable_WetGrip", config.TireLable_WetGrip),
                new SqlParameter("@TireLable_Noise", config.TireLable_Noise),
                new SqlParameter("@PatternSymmetry", config.PatternSymmetry),
                new SqlParameter("@TireGuideRotation", config.TireGuideRotation),
                new SqlParameter("@FactoryCode", config.FactoryCode),
                new SqlParameter("@GrooveNum", config.GrooveNum),
                new SqlParameter("@Remark", config.Remark),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0 ? true : false;
        }

        public static bool UpdateTireSpecParamsConfig(SqlConnection conn, TireSpecParamsConfig config)
        {
            string sql = @"UPDATE [Configuration].[dbo].[tbl_TireSpecParamsConfig]
   SET [QualityInspectionName] = @QualityInspectionName
      ,[OriginPlace] = @OriginPlace
      ,[RimProtection] = @RimProtection
      ,[TireLoad] = @TireLoad
      ,[MuddyAndSnow] = @MuddyAndSnow
      ,[ThreeT_Treadwear] = @ThreeT_Treadwear
      ,[ThreeT_Traction] = @ThreeT_Traction
      ,[ThreeT_Temperature] = @ThreeT_Temperature
      ,[TireCrown_Polyester] = @TireCrown_Polyester
      ,[TireCrown_Steel] = @TireCrown_Steel
      ,[TireCrown_Nylon] = @TireCrown_Nylon
      ,[TireSideWall_Polyester] = @TireSideWall_Polyester
      ,[TireLable_RollResistance] = @TireLable_RollResistance
      ,[TireLable_WetGrip] = @TireLable_WetGrip
      ,[TireLable_Noise] = @TireLable_Noise
      ,[PatternSymmetry] = @PatternSymmetry
      ,[TireGuideRotation] = @TireGuideRotation
      ,[FactoryCode] = @FactoryCode
      ,[GrooveNum] = @GrooveNum
      ,[Remark] = @Remark
      ,[LastUpdateDataTime] = getdate()
 WHERE [PId] = @PId";
            int? RimProtection = null;
            if (config.RimProtection.HasValue)
            {
                if (config.RimProtection == true) RimProtection = 1;
                else RimProtection = 0;
            }

            int? MuddyAndSnow = null;
            if (config.MuddyAndSnow.HasValue)
            {
                if (config.MuddyAndSnow == true) MuddyAndSnow = 1;
                else MuddyAndSnow = 0;
            }

            var sqlParam = new[]
            {
                new SqlParameter("@PId", config.PId),
                new SqlParameter("@QualityInspectionName", config.QualityInspectionName),
                new SqlParameter("@OriginPlace", config.OriginPlace),
                new SqlParameter("@RimProtection", RimProtection),
                new SqlParameter("@TireLoad", config.TireLoad),
                new SqlParameter("@MuddyAndSnow", MuddyAndSnow),
                new SqlParameter("@ThreeT_Treadwear", config.ThreeT_Treadwear),
                new SqlParameter("@ThreeT_Traction", config.ThreeT_Traction),
                new SqlParameter("@ThreeT_Temperature", config.ThreeT_Temperature),
                new SqlParameter("@TireCrown_Polyester", config.TireCrown_Polyester),
                new SqlParameter("@TireCrown_Steel", config.TireCrown_Steel),
                new SqlParameter("@TireCrown_Nylon", config.TireCrown_Nylon),
                new SqlParameter("@TireSideWall_Polyester", config.TireSideWall_Polyester),
                new SqlParameter("@TireLable_RollResistance", config.TireLable_RollResistance),
                new SqlParameter("@TireLable_WetGrip", config.TireLable_WetGrip),
                new SqlParameter("@TireLable_Noise", config.TireLable_Noise),
                new SqlParameter("@PatternSymmetry", config.PatternSymmetry),
                new SqlParameter("@TireGuideRotation", config.TireGuideRotation),
                new SqlParameter("@FactoryCode", config.FactoryCode),
                new SqlParameter("@GrooveNum", config.GrooveNum),
                new SqlParameter("@Remark", config.Remark),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0 ? true : false;
        }

        public static bool InsertTireSpecParamsEditLog(SqlConnection conn, TireSpecParamsEditLog log)
        {
            string sql = @"INSERT INTO [Configuration].[dbo].[tbl_TireSpecParamsEditLog]
           ([PId]
           ,[ChangeBefore]
           ,[ChangeAfter]
           ,[Operator]
           ,[CreateTime]
           ,[LastUpdateDataTime])
     VALUES
           (@PId
           ,@ChangeBefore
           ,@ChangeAfter
           ,@Operator
           ,GETDATE()
           ,GETDATE())";
            var sqlParam = new[]
            {
                new SqlParameter("@PId",log.PId),
                new SqlParameter("@ChangeBefore",log.ChangeBefore),
                new SqlParameter("@ChangeAfter",log.ChangeAfter),
                new SqlParameter("@Operator",log.Operator),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0 ? true : false;
        }

        public static bool CheckPidExist(SqlConnection conn, string pid)
        {
            string sql = @"select * from [Configuration].[dbo].[tbl_TireSpecParamsConfig] with (nolock)
                        where pid=@PID";
            var sqlParam = new[]
                {
                    new SqlParameter("@PID",pid),
                };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam)
                .ConvertTo<TireSpecParamsConfig>().ToList().Count > 0 ? true : false;
        }
    }
}
