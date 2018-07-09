using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.UserAuthJob.DAL
{
    public static partial class UserProfileInitializationDal
    {
        static string Select_AllRefreshUserCNT => @"
SELECT  COUNT(DISTINCT o.UserID)
FROM    Gungnir..tbl_Order o WITH ( NOLOCK )
WHERE   NOT EXISTS ( SELECT UP.UserId
                     FROM   Tuhu_profiles..tbl_UserProfile AS UP WITH ( NOLOCK )
                            JOIN Tuhu_profiles..tbl_UserProfileDescription AS UPD
                            WITH ( NOLOCK ) ON UP.ProfileId = UPD.PKID
                     WHERE  UPD.ProfileName = @ProfileName
                            AND o.UserID = UP.UserId )
        AND o.OrderChannel IN (
                SELECT  ChannelKey
                FROM    Gungnir..tbl_ChannelDictionaries WITH ( NOLOCK )
                WHERE   ChannelType = N'自有渠道' )
";

        static string Select_UserProfileWithPage_OrderQTY => @"
SELECT  t.UserID ,
        t.Value
FROM    ( SELECT    o.UserID ,
                    SUM(CASE o.Status
                          WHEN N'7Canceled' THEN 0
                          ELSE 1
                        END) Value ,
                    MIN(PKID) pkid
          FROM      Gungnir..tbl_Order o WITH ( NOLOCK )
          WHERE     NOT EXISTS ( SELECT UP.UserId
                     FROM   Tuhu_profiles..tbl_UserProfile AS UP WITH ( NOLOCK )
                            JOIN Tuhu_profiles..tbl_UserProfileDescription AS UPD
                            WITH ( NOLOCK ) ON UP.ProfileId = UPD.PKID
                     WHERE  UPD.ProfileName = N'CreatedOrderQTY'
                            AND o.UserID = UP.UserId )
                    AND o.OrderChannel IN (
                            SELECT  ChannelKey
                            FROM    Gungnir..tbl_ChannelDictionaries WITH ( NOLOCK )
                            WHERE   ChannelType = N'自有渠道' )
          GROUP BY  o.UserID
        ) t
ORDER BY t.pkid
        OFFSET @Offset ROW
				FETCH NEXT @PageSize ROW ONLY;
";

        static string Select_AllRefreshUserCNTWithDate => @"
SELECT  COUNT(DISTINCT UserID)
FROM    Gungnir..tbl_Order WITH ( NOLOCK )
WHERE   OrderDatetime >= @BeginDate
        AND OrderDatetime <= @EndDate
        AND OrderChannel IN (
        SELECT  ChannelKey
        FROM    Gungnir..tbl_ChannelDictionaries WITH ( NOLOCK )
        WHERE   ChannelType = N'自有渠道' )
";


        static string Select_UserProfileWithPage_OrderQTY_Date => @"
SELECT  t.UserID ,
        t.Value
FROM    ( SELECT    o.UserID ,
                    SUM(CASE o.Status
                          WHEN N'7Canceled' THEN 0
                          ELSE 1
                        END) Value ,
                    MIN(PKID) pkid
          FROM      Gungnir..tbl_Order o WITH ( NOLOCK )
          WHERE     OrderDatetime >= @BeginDate 
                    AND OrderDatetime<=@EndDate
                    AND o.OrderChannel IN (
                            SELECT  ChannelKey
                            FROM    Gungnir..tbl_ChannelDictionaries WITH ( NOLOCK )
                            WHERE   ChannelType = N'自有渠道' )
          GROUP BY  o.UserID
        ) t
ORDER BY t.pkid
        OFFSET @Offset ROW
				FETCH NEXT @PageSize ROW ONLY;
";

    }
}
