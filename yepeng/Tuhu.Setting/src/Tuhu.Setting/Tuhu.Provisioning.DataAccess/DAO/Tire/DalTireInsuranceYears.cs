using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.Entity.Tire;

namespace Tuhu.Provisioning.DataAccess.DAO.Tire
{
    public class DalTireInsuranceYears
    {
        public static async Task<IEnumerable<TireInsuranceYearModel>> SelectInstallFeeListAsync(InstallFeeConditionModel condition, PagerModel pager)
        {
            string sql = @"SELECT  VP.PID ,
        AIF.TireInsuranceYears ,
        VP.DisplayName ,
        VP.TireSize
FROM    Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
        LEFT JOIN Tuhu_productcatalog..tbl_ProductExtraProperties AS AIF WITH ( NOLOCK ) ON AIF.PID = VP.PID
WHERE   VP.PID LIKE 'TR-%'
        AND ( @Brands IS NULL
              OR VP.CP_Brand COLLATE Chinese_PRC_CI_AS IN (
              SELECT    *
              FROM      Gungnir.dbo.Split(@Brands, ';') )
            )
        AND ( @Patterns IS NULL
              OR VP.CP_Tire_Pattern COLLATE Chinese_PRC_CI_AS IN (
              SELECT    *
              FROM      Gungnir.dbo.Split(@Patterns, ';') )
            )
        AND ( @TireSizes IS NULL
              OR VP.TireSize COLLATE Chinese_PRC_CI_AS IN (
              SELECT    *
              FROM      Gungnir.dbo.Split(@TireSizes, ';') )
            )
        AND ( @Rims IS NULL
              OR VP.CP_Tire_Rim COLLATE Chinese_PRC_CI_AS IN (
              SELECT    *
              FROM      Gungnir.dbo.Split(@Rims, ';') )
            )
        AND ( @Rof IS NULL
              OR VP.CP_Tire_ROF = @Rof
            )
        AND ( @Winter IS NULL
              OR VP.CP_Tire_Snow = @Winter
            )
        AND ( @PID IS NULL
              OR VP.PID LIKE '%' + @PID + '%'
            )
        AND ( @OnSale IS NULL
              OR VP.OnSale = @OnSale
            )
        AND ( @IsConfig IS NULL
              OR @IsConfig = 1
              AND AIF.TireInsuranceYears = 2
              OR @IsConfig = 0
              AND (AIF.TireInsuranceYears IS NULL or AIF.TireInsuranceYears=1 )
            )
ORDER BY AIF.PKID DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;";
            string countsql = @"SELECT COUNT(1) FROM    Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
        LEFT JOIN Tuhu_productcatalog..tbl_ProductExtraProperties AS AIF WITH ( NOLOCK ) ON AIF.PID = VP.PID
WHERE   VP.PID LIKE 'TR-%'
        AND ( @Brands IS NULL
              OR VP.CP_Brand COLLATE Chinese_PRC_CI_AS IN (
              SELECT    *
              FROM      Gungnir.dbo.Split(@Brands, ';') )
            )
        AND ( @Patterns IS NULL
              OR VP.CP_Tire_Pattern COLLATE Chinese_PRC_CI_AS IN (
              SELECT    *
              FROM      Gungnir.dbo.Split(@Patterns, ';') )
            )
        AND ( @TireSizes IS NULL
              OR VP.TireSize COLLATE Chinese_PRC_CI_AS IN (
              SELECT    *
              FROM      Gungnir.dbo.Split(@TireSizes, ';') )
            )
        AND ( @Rims IS NULL
              OR VP.CP_Tire_Rim COLLATE Chinese_PRC_CI_AS IN (
              SELECT    *
              FROM      Gungnir.dbo.Split(@Rims, ';') )
            )
        AND ( @Rof IS NULL
              OR VP.CP_Tire_ROF = @Rof
            )
        AND ( @Winter IS NULL
              OR VP.CP_Tire_Snow = @Winter
            )
        AND ( @PID IS NULL
              OR VP.PID LIKE '%' + @PID + '%'
            )
        AND ( @OnSale IS NULL
              OR VP.OnSale = @OnSale
            )
        AND ( @IsConfig IS NULL
              OR @IsConfig = 1
              AND AIF.TireInsuranceYears = 2
              OR @IsConfig = 0
              AND (AIF.TireInsuranceYears IS NULL or AIF.TireInsuranceYears=1 )
            )";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {

                var para = new SqlParameter[] {
                    new SqlParameter("@Brands",condition.Brands),
                    new SqlParameter("@IsConfig",condition.IsConfig),
                    new SqlParameter("@OnSale",condition.OnSale),
                    new SqlParameter("@Patterns",condition.Patterns),
                    new SqlParameter("@PID",condition.PID),
                    new SqlParameter("@Rims",condition.Rims),
                    new SqlParameter("@Rof",condition.Rof),
                    new SqlParameter("@TireSizes",condition.TireSizes),
                    new SqlParameter("@Winter",condition.Winter),
                    new SqlParameter("@PageIndex",pager.CurrentPage),
                    new SqlParameter("@PageSize",pager.PageSize),
                };
                var para_temp = new SqlParameter[] {
                    new SqlParameter("@Brands",condition.Brands),
                    new SqlParameter("@IsConfig",condition.IsConfig),
                    new SqlParameter("@OnSale",condition.OnSale),
                    new SqlParameter("@Patterns",condition.Patterns),
                    new SqlParameter("@PID",condition.PID),
                    new SqlParameter("@Rims",condition.Rims),
                    new SqlParameter("@Rof",condition.Rof),
                    new SqlParameter("@TireSizes",condition.TireSizes),
                    new SqlParameter("@Winter",condition.Winter)
                };
                var count = await dbHelper.ExecuteScalarAsync(countsql, CommandType.Text, para_temp);
                int totalcount = 0;
                int.TryParse(count?.ToString(), out totalcount);
                pager.TotalItem = totalcount;
                return dbHelper.ExecuteDataTable(sql, CommandType.Text, para).ConvertTo<TireInsuranceYearModel>();
            }
        }

        public static async Task<IEnumerable<TireModifyConfigLog>> SelectLogsAsync(string pid,string type)
        {
            string sql = $"SELECT * FROM  Tuhu_log..TireModifyConfigLog WITH ( NOLOCK) WHERE pid=N'{pid}' and type=N'{type}' ";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                var result = (await dbHelper.ExecuteDataTableAsync(sql)).ConvertTo<TireModifyConfigLog>();
                return result;
            }
        }
        public static async Task<bool> WriteLogsAsync(IEnumerable<TireModifyConfigLog> logs)
        {
            await Task.Yield();
            bool result = true;
            if (logs != null && logs.Any())
            {
                foreach (var temps in ParseTargets(logs, 10))
                {
                    var sqls = temps.Select(x => $@" INSERT INTO Tuhu_log..TireModifyConfigLog
        ( UserId ,
          Before ,
          After ,
          Pid,
          Type,
          CreateDateTime ,
          LastUpdateDateTime
        )
VALUES  ( N'{x.UserId}' , -- UserId - nvarchar(100)
          N'{x.Before}' , -- Before - nvarchar(1000)
          N'{x.After}' , -- After - nvarchar(1000)
          N'{x.Pid}',
          N'{x.Type}',
          GETDATE() , -- CreateDateTime - datetime
          GETDATE()  -- LastUpdateDateTime - datetime
        ) ");
                    string sql = string.Join(" ; ", sqls);
                    if (string.IsNullOrEmpty(sql))
                    {
                        continue;
                    }
                    using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
                    {
                        using (var cmd = new SqlCommand(sql))
                        {
                            cmd.CommandType = CommandType.Text;
                            var insertresult = dbHelper.ExecuteNonQuery(cmd);
                            result &= insertresult > 0;
                        }

                    }
                }

            }
            return result;
        }

        public static IEnumerable<IEnumerable<T>> ParseTargets<T>(IEnumerable<T> targets, int maxcount)
        {
            if (targets != null && targets.Any())
            {
                int TotalCount = targets.Count();
                int pages = TotalCount / maxcount;
                for (int i = 0; i <= pages; i++)
                {
                    var temps = targets.Skip(i * maxcount).Take(maxcount);
                    yield return temps;
                }
            }
            else
            {
                yield break;
            }
        }
    }
}
