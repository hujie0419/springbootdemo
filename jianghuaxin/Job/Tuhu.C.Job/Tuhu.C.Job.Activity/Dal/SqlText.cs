namespace Tuhu.C.Job.Activity.Dal
{
    public class SqlText
    {
        internal const string SqlStr4GetExpiringGroupInfo = @"select GroupId,
       EndTime,
       CurrentMemberCount
from Activity..tbl_GroupBuyingInfo with (nolock)
where GroupStatus = 1
      and StartTime < DATEADD(hour, -23, GETDATE());";

        internal const string SqlStr4GetExpiringUserByGroupId = @"SELECT	T.UserId ,
		T.PID ,
		T.OrderId ,
		P.ProductName ,
		S.CurrentMemberCount ,
		S.RequiredMemberCount
FROM	Activity..tbl_GroupBuyingInfo AS S WITH ( NOLOCK )
		LEFT JOIN Activity..tbl_GroupBuyingUserInfo AS T WITH ( NOLOCK ) ON S.GroupId = T.GroupId
		LEFT JOIN Configuration..GroupBuyingProductConfig AS P WITH ( NOLOCK ) ON P.ProductGroupId = S.ProductGroupId
																					AND P.PID = T.PID
WHERE	S.GroupId = @groupid
		AND T.UserStatus = 1;";

        internal const string SqlStr4GetExpiredUserList = @"SELECT	UserId ,
		OrderId
FROM	Activity..tbl_GroupBuyingUserInfo WITH ( NOLOCK )
WHERE	CreateDateTime < DATEADD(DAY, -1, GETDATE())
		AND USERStatus = 0;";

        internal const string SqlStr4ChangeUserOrderStatus = @"UPDATE	Activity..tbl_GroupBuyingUserInfo WITH ( ROWLOCK )
SET		UserStatus = 4
WHERE	OrderId = @orderid
		AND UserStatus = 0;";

        /// <summary>
        /// 过期拼团信息
        /// </summary>
        internal const string SqlStr4GetExpiredGroupBuyingInfo = @"SELECT [P].[PID],
       [P].[ProductGroupId],
       [G].[BeginTime],
       [G].[EndTime],
       [P].[ProductName],
       [P].[OriginalPrice],
       [P].[FinalPrice],
       [P].[SpecialPrice],
       [G].[Label],
       [P].[Creator],
       N'活动到期' AS TriggerType
FROM [Configuration].[dbo].[GroupBuyingProductConfig] AS P WITH (NOLOCK)
    JOIN [Configuration].[dbo].[GroupBuyingProductGroupConfig] AS G WITH (NOLOCK)
        ON [P].[ProductGroupId] = [G].[ProductGroupId]
           AND [EndTime] < GETDATE()
           AND [EndTime] >= DATEADD(DAY, -1, GETDATE())
ORDER BY [G].[EndTime];";

        /// <summary>
        /// 未过期拼团信息
        /// </summary>
        internal const string SqlStr4GetActiveGroupBuyingInfo = @"SELECT [P].[PID],
       [P].[ProductGroupId],
       [G].[BeginTime],
       [G].[EndTime],
       [P].[ProductName],
       [P].[OriginalPrice],
       [P].[FinalPrice],
       [P].[SpecialPrice],
       [G].[Label],
       [P].[Creator],
       1 AS IsActive,
       [P].[TotalStockCount],
       [P].[CurrentSoldCount]
FROM [Configuration].[dbo].[GroupBuyingProductConfig] AS P WITH (NOLOCK)
    JOIN [Configuration].[dbo].[GroupBuyingProductGroupConfig] AS G WITH (NOLOCK)
        ON [P].[ProductGroupId] = [G].[ProductGroupId]
           AND [G].[EndTime] > GETDATE()
           AND [G].[BeginTime] <= GETDATE()
ORDER BY [G].[EndTime];";

        /// <summary>
        /// 获取义乌仓库存不足的团
        /// </summary>
        internal const string SqlStr4GetYiwuStockOutProductGroup = @"SELECT [IdentityID]
FROM [Tuhu_log].[dbo].[GroupBuyingConfigLog] WITH (NOLOCK)
WHERE [UpdateTime] > DATEADD(DAY, -1, GETDATE())
      AND [OperateUser] = 'YiWuPinTuanStockMonitorConsumer'
ORDER BY PKID DESC;";

        /// <summary>
        /// 根据ProductGroupId获取团以及团配置的商品信息
        /// </summary>
        internal const string SqlStr4GetGroupBuyingInfoByGroupIdFormat = @"SELECT [P].[PID],
       [P].[ProductGroupId],
       [G].[BeginTime],
       [G].[EndTime],
       [P].[ProductName],
       [P].[OriginalPrice],
       [P].[FinalPrice],
       [P].[SpecialPrice],
       [G].[Label],
       [P].[Creator],
       N'义乌仓无货' AS TriggerType
FROM [Configuration].[dbo].[GroupBuyingProductConfig] AS P WITH (NOLOCK)
    JOIN [Configuration].[dbo].[GroupBuyingProductGroupConfig] AS G WITH (NOLOCK)
        ON [P].[ProductGroupId] = [G].[ProductGroupId]
	WHERE [P].[ProductGroupId] IN ({0})
ORDER BY [G].[EndTime] DESC";


    }
}
