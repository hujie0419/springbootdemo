using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.UserAuthJob.DAL
{
    public static partial class WxUserAuthRefreshDal
    {
        /// <summary>
        /// 获取所有需要刷新token的授权信息
        /// @ExpanDay 间隔天数
        /// </summary>
        public static string Sql_Select_AllRefalshUser => @"
SELECT DISTINCT
        A.RefreshToken
FROM    ( SELECT    * ,
                    DATEDIFF(DAY, ISNULL(RefreshAuthTime,CreatedTime), GETDATE()) cnt
          FROM      Tuhu_profiles..UserAuth WITH ( NOLOCK )
          WHERE     RefreshToken IS NOT NULL AND AuthSource=N'Weixin' AND AuthorizationStatus=N'Authorized'
        ) A
WHERE   A.cnt  between @ExpanDay and 30;
";
        /// <summary>
        /// 更新token
        /// @AccessToken
        /// @AccessTokenExpire
        /// @NewRefreshToken
        /// @RefreshTokenExpire
        /// @OldRefreshToken
        /// </summary>
        public static string Sql_Update_UserAuthToken => @"
UPDATE  Tuhu_profiles..UserAuth WITH ( ROWLOCK )
SET     AccessToken = ISNULL(@AccessToken, AccessToken) ,
        AccessTokenExpire = ISNULL(@AccessTokenExpire, AccessTokenExpire) ,
        RefreshToken = ISNULL(@NewRefreshToken, RefreshToken) ,
        RefreshTokenExpire = ISNULL(@RefreshTokenExpire, RefreshTokenExpire) ,
        MetaData = ISNULL(@MetaData, MetaData) ,
        RefreshStatus = ISNULL(@RefreshStatus, RefreshStatus) ,
        AuthorizationStatus = ISNULL(@AuthorizationStatus, AuthorizationStatus) ,
        RefreshAuthTime = GETDATE()
WHERE   PKID IN (  SELECT PKID
                   FROM   Tuhu_profiles..UserAuth WITH ( NOLOCK )
                   WHERE  RefreshToken = @OldRefreshToken
                          AND AuthSource = N'Weixin'
                );";
    }
}
