using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.DAO.Tire
{
    internal class TireSql
    {
        internal class InstallFee
        {
            private static string select_part = @" FROM    Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
        LEFT JOIN Configuration.dbo.tbl_TireAddInstallFee AS AIF WITH ( NOLOCK ) ON AIF.PID = VP.PID
WHERE   VP.PID LIKE 'TR-%'
        AND ( @Brands IS NULL
              OR VP.CP_Brand COLLATE Chinese_PRC_CI_AS IN ( SELECT    *
                                  FROM      Gungnir.dbo.Split(@Brands, ';') )
            )
        AND ( @Patterns IS NULL
              OR VP.CP_Tire_Pattern COLLATE Chinese_PRC_CI_AS IN (
              SELECT    *
              FROM      Gungnir.dbo.Split(@Patterns, ';') )
            )
        AND ( @TireSizes IS NULL
              OR VP.TireSize COLLATE Chinese_PRC_CI_AS IN ( SELECT    *
                                  FROM      Gungnir.dbo.Split(@TireSizes, ';') )
            )
        AND ( @Rims IS NULL
              OR VP.CP_Tire_Rim COLLATE Chinese_PRC_CI_AS IN ( SELECT *
                                     FROM   Gungnir.dbo.Split(@Rims, ';') )
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
              AND AIF.PKID > 0
              OR @IsConfig = 0
              AND AIF.PKID IS NULL
            ) ";
            internal static string select = @"SELECT  VP.PID ,
        AIF.AddPrice ,
        VP.DisplayName ,
        VP.TireSize" + select_part + @"ORDER BY AIF.PKID DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;";
            internal static string select_count = @"SELECT COUNT(1)"+select_part;
            internal static string delete = "DELETE Configuration.dbo.tbl_TireAddInstallFee WHERE PID=@PID";
            internal static string update = "UPDATE Configuration.dbo.tbl_TireAddInstallFee SET AddPrice=@AddPrice,LastUpdateDateTime=GETDATE() WHERE PID=@PID";
            internal static string insert = "INSERT INTO Configuration.dbo.tbl_TireAddInstallFee ( PID , AddPrice) VALUES  ( @PID,@AddPrice)";
        }

        internal class StockoutWgite
        {
            private static string select_part = @" FROM    Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
            LEFT JOIN Tuhu_bi..tbl_TireStock AS TS WITH ( NOLOCK ) ON VP.PID = TS.Pid AND TS.CityId = @CityId
    WHERE   VP.PID LIKE 'TR-%'
            AND ( VP.TireSize = @TireSize
                  OR @TireSize IS NULL
                )
            AND ( VP.Pid LIKE '%' + @PID + '%'
                  OR @PID IS NULL
                )
            AND ( @Status = 0
                  OR @Status = -1
                  AND ( TS.Stock = 0
                        OR TS.Stock IS NULL
                      )
                  AND TS.Pid IS NOT NULL--缺货
                  OR @Status = 1
                  AND TS.Stock > 0--有货
                  OR @Status = -2
                  AND TS.Pid IS NULL --N/A
                )  ";
            internal static string select = @"SELECT  VP.PID ,
            VP.DisplayName ,
            VP.TireSize ,
            VP.Stockout,
            (CASE WHEN TS.Stock>0 THEN N'有货' 
			WHEN  TS.Pid IS NULL THEN N'N/A'
			ELSE N'无货' END) AS Status" + select_part + @"ORDER BY TS.Stock DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;";
            internal static string select_count = @"SELECT COUNT(1)" + select_part;
        }

        internal class StockoutStatusWhite
        {
            private static string select_part = @" FROM    Tuhu_productcatalog.dbo.vw_Products AS VP WITH ( NOLOCK )
        LEFT JOIN Configuration..TireStockWhiteList AS TSW WITH ( NOLOCK ) ON VP.PID = TSW.PID
WHERE   VP.PID LIKE 'TR-%'
        AND ( VP.PID LIKE '%' + @PID + '%'
              OR @PID IS NULL
            )
        AND ( @Status = 0
           OR @Status = 1
           AND TSW.PKID > 0
           OR @Status = -1
           AND TSW.PKID IS NULL
         )  ";
            internal static string select = @"SELECT  VP.PID ,
        VP.DisplayName ,
         VP.TireSize ,
        ( CASE WHEN TSW.PKID IS NULL THEN 0
               ELSE 1
          END ) AS Status" + select_part + @"ORDER BY TSW.PKID DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;";
            internal static string select_count = @"SELECT COUNT(1)" + select_part;

            internal static string fetch_pidStatus = @"SELECT  1
FROM    Configuration.dbo.TireStockWhiteList AS TSW WITH ( NOLOCK )
WHERE   TSW.PID = @PID";

            internal static string insert = @"INSERT  INTO Configuration.dbo.TireStockWhiteList  ( PID ) VALUES  ( @PID );";
            internal static string update = @"UPDATE  Configuration..TireStockWhiteList
                                              SET     LastUpdateDateTime = GETDATE(),
                                                      Type=0
                                              WHERE   PID = @PID";
            internal static string delete = @"DELETE  Configuration.dbo.TireStockWhiteList WHERE  PID = @PID ;";
        }

        internal class StockoutWhite
        {
            private static string select_part = @"
from Tuhu_productcatalog..vw_Products as VP with (nolock)
    left join Configuration..TireStockWhiteList as TSW with (nolock)
        on VP.PID = TSW.PID
    left join Tuhu_bi..tbl_TireStock as TS with (nolock)
        on VP.PID = TS.Pid
           and @CityId = TS.CityId
where VP.PID like 'TR-%'
      and (   VP.PID like '%' + @PID + '%'
              or @PID is null)
      and (   VP.TireSize = @TireSize
              or @TireSize is null)
      and (   @ProductName is null
              or VP.DisplayName like '%' + @ProductName + '%')
      and (   @OnSale = 0
              or @OnSale = -1
                 and VP.OnSale = 0
              or @OnSale = 1
                 and VP.OnSale = 1)
      and (   @stuckout = 0
              or @stuckout = -1
                 and VP.stockout = 0
              or @stuckout = 1
                 and VP.stockout = 1)
      and (   @SystemStuckout = 0
              or @SystemStuckout = -1
                 and (   TS.Stock = 0
                         or TS.Stock is null)
                 and TS.Pid is not null
              or @SystemStuckout = 1
                 and TS.Stock > 0
              or @SystemStuckout = -2
                 and TS.Pid is null)
      and (   @Status = 0
              or @Status = 1
                 and TSW.PKID > 0
              or @Status = -1
                 and TSW.PKID is null)
      and (   @RegionalStockout = 0
              or @RegionalStockout = -1
                 and (   TS.CentralDefaultWarehouseStock = 0
                         or TS.CentralDefaultWarehouseStock is null)
                 and TS.Pid is not null
              or @RegionalStockout = 1
                 and TS.CentralDefaultWarehouseStock > 0)
      and (   @isShow = 0
              or @isShow = -1
                 and VP.IsShow = 0
              or @isShow = 1
                 and VP.IsShow = 1)";

            internal static string select = @"
select VP.PID,
       VP.DisplayName,
       VP.TireSize,
       VP.OnSale,
       VP.IsShow,
       (case
            when TS.Stock > 0 then
                N'有货'
            when TS.Pid is null then
                N'有货'
            else
                N'缺货'
        end) as SystemStuckout,
       VP.stockout as Stuckout,
       (case
            when TSW.PKID is null then
                0
            else
                1
        end) as Status,
       case
           when TS.CentralDefaultWarehouseStock > 0
                or TS.Pid is null then
               N'有货'
           else
               N'缺货'
       end as RegionalStockout " + select_part + @" ORDER BY TSW.PKID DESC
    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;";
            internal static string select_count = @"SELECT COUNT(1) " + select_part;

            internal static string select_showStatus = @"WITH PIDS AS (SELECT Item COLLATE Chinese_PRC_CI_AS AS PID FROM Tuhu_productcatalog..SplitString(@pids, ',', 1)) "
+ @"SELECT VP.PID,VP.DisplayName,VP.TireSize,VP.OnSale,VP.IsShow, 
(CASE WHEN TS.Stock>0 THEN N'有货' 
			WHEN  TS.Pid IS NULL THEN N'N/A'
			ELSE N'无货' END) AS SystemStuckout,
VP.stockout AS Stuckout,( CASE WHEN TSW.PKID IS NULL THEN 0
               ELSE 1
          END ) AS Status " + @"FROM PIDS WITH (NOLOCK) LEFT JOIN Tuhu_productcatalog..vw_Products AS VP WITH(NOLOCK) ON PIDS.PID=VP.PID LEFT JOIN Configuration..TireStockWhiteList AS TSW WITH(NOLOCK) ON VP.PID=TSW.PID 
LEFT JOIN Tuhu_bi..tbl_TireStock AS TS WITH ( NOLOCK ) ON VP.PID = TS.Pid AND TS.CityId=1; ";
        }
    }
}
