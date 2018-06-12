using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.DataAccess
{
    public static class DalgroupBuyingText
    {
        internal const string SqlStr4GetGroupBuyingProductList = @"
select T.ProductGroupId,
       T.Image,
       T.ShareId,
       T.GroupType,
       T.MemberCount,
       T.Sequence,
       T.CurrentGroupCount,
       T.BeginTime,
       T.Label,
       T.EndTime,
       S.PID,
       S.ProductName,
       S.OriginalPrice,
       S.FinalPrice,
       S.SpecialPrice,
       T.ActivityId,
       T.IsShow,
       T.TotalGroupCount,
       T.GroupCategory,
       T.GroupDescription
from Configuration..GroupBuyingProductGroupConfig as T with (nolock)
    left join Configuration..GroupBuyingProductConfig as S with (nolock)
        on T.ProductGroupId = S.ProductGroupId
where S.DisPlay = 1
      and T.IsDelete = 0;";

        internal const string SqlStr4FetchGroupInfoByProductGroupId = @"
select T.ProductGroupId,
       T.Image,
       T.ShareId,
       T.GroupType,
       T.MemberCount,
       T.Sequence,
       T.CurrentGroupCount,
       T.BeginTime,
       T.Label,
       T.EndTime,
       S.PID,
       S.ProductName,
       S.OriginalPrice,
       S.FinalPrice,
       S.SpecialPrice,
       T.ActivityId,
       T.IsShow,
       T.TotalGroupCount,
       T.GroupCategory,
       T.GroupDescription
from Configuration..GroupBuyingProductGroupConfig as T with (nolock)
    left join Configuration..GroupBuyingProductConfig as S with (nolock)
        on T.ProductGroupId = S.ProductGroupId
where T.ProductGroupId = @productgroupid
      and S.PID = @Pid;";

        internal const string SqlStr4SelectGroupBuyingProductsById = @"select PID
from Configuration..GroupBuyingProductConfig with (nolock)
where ProductGroupId = @productgroupid
      and IsDelete = 0
      and IsShow = 1";

        internal const string SqlStr4SelectProductInfoByPid = @"select PID,
       ProductName,
       OriginalPrice,
       FinalPrice,
       SpecialPrice,
       UseCoupon,
       UpperLimitPerOrder
from Configuration..GroupBuyingProductConfig with (nolock)
where ProductGroupId = @productgroupid
      and IsShow = 1;";

        internal const string SqlStr4SelectGroupInfoByProductGroupId = @"select top 100
       T.GroupId,
       T.OwnerId,
       T.ProductGroupId,
       T.GroupType,
       T.GroupStatus,
       T.RequiredMemberCount,
       T.CurrentMemberCount,
       T.StartTime,
       T.EndTime
from Activity..tbl_GroupBuyingInfo as T with (nolock)
where T.GroupStatus = 1
      and T.ProductGroupId = @productgroupid
      and T.EndTime > GETDATE()
      and (   (   @flag = 1
                  and T.CurrentMemberCount > 0)
              or (   @flag = 0
                     and T.GroupType in ( 0, 2 )
                     and T.GroupId not in (   select GroupId
                                              from Activity..tbl_GroupBuyingUserInfo with (nolock)
                                              where UserId = @userid
                                                    and UserStatus < 2 )));";

        internal const string SqlStr4GetGroupInfoCountByProductGroupId = @"SELECT  COUNT(1)
FROM    Activity..tbl_GroupBuyingInfo AS T WITH ( NOLOCK )
WHERE   T.GroupStatus = 1
        AND T.ProductGroupId = @productgroupid
        AND T.EndTime > GETDATE()
        AND ( @flag = 1
              OR T.GroupType IN ( 0, 2 )
              AND T.GroupId NOT IN (
              SELECT    GroupId
              FROM      Activity..tbl_GroupBuyingUserInfo WITH ( NOLOCK )
              WHERE     UserId = @userid
                        AND UserStatus < 2 )
            );";


        internal const string SqlStr4FetchGroupInfoByGroupId = @"
select T.GroupId,
       T.OwnerId,
       T.ProductGroupId,
       T.GroupType,
       T.GroupStatus,
       T.RequiredMemberCount,
       T.CurrentMemberCount,
       T.StartTime,
       T.EndTime,
       S.GroupCategory
from
(   select GroupId,
           OwnerId,
           ProductGroupId,
           GroupType,
           GroupStatus,
           RequiredMemberCount,
           CurrentMemberCount,
           StartTime,
           EndTime
    from Activity..tbl_GroupBuyingInfo with (nolock)
    where GroupId = @groupid) as T
    left join Configuration..GroupBuyingProductGroupConfig as S with (nolock)
        on T.ProductGroupId = S.ProductGroupId";

        internal const string Sql4FetchGroupInfoByOrderId = @"SELECT	T.GroupId ,
		T.OwnerId ,
		T.ProductGroupId ,
		T.GroupType ,
		T.GroupStatus ,
		T.RequiredMemberCount ,
		T.CurrentMemberCount ,
		T.StartTime ,
		T.EndTime
FROM	Activity..tbl_GroupBuyingUserInfo AS S WITH ( NOLOCK )
		LEFT JOIN Activity..tbl_GroupBuyingInfo AS T WITH ( NOLOCK ) ON T.GroupId = S.GroupId
WHERE	S.OrderId = @orderid;";


        internal const string SqlStr4SelectGroupMemberByGroupId = @"
select T.UserId,
       S.OwnerId,
       T.OrderId,
       T.CreateDateTime
from Activity..tbl_GroupBuyingUserInfo as T with (nolock)
    left join Activity..tbl_GroupBuyingInfo as S with (nolock)
        on T.GroupId = S.GroupId
where T.GroupId = @groupid
      and (   S.GroupStatus <> 2
              and (   T.UserStatus = 1
                      or T.UserStatus = 3)
              or T.IsFinish = 1)
order by T.CreateDateTime;";

        internal const string SqlStr4CheckUserGroupInfo = @"SELECT	OrderId
FROM	Activity..tbl_GroupBuyingUserInfo WITH ( NOLOCK )
WHERE	GroupId = @groupid
		AND UserId = @userid
		AND UserStatus <> 4;";

        internal const string SqlStr4CheckProductGroupInfo =
            @"SELECT  CASE WHEN CurrentGroupCount >= TotalGroupCount THEN 2
             WHEN GETDATE() < BeginTime
                  OR GETDATE() > EndTime THEN 3
             WHEN SpecialUser <> 0 THEN 10
             ELSE 1
        END AS Code
FROM    Configuration..GroupBuyingProductGroupConfig WITH ( NOLOCK )
WHERE   ProductGroupId = @productgroupid
        AND IsDelete = 0;";

        internal const string SqlStr4CreateGroupBuyingGroupInfo = @"INSERT	INTO Activity..tbl_GroupBuyingInfo
		(	GroupId ,
			OwnerId ,
			ProductGroupId ,
			GroupType ,
			GroupStatus ,
			RequiredMemberCount ,
			CurrentMemberCount ,
			StartTime ,
			EndTime 
		)
		SELECT	@groupid ,
				@userid ,
				ProductGroupId ,
				GroupType ,
				0 ,
				MemberCount ,
				0 ,
				GETDATE() ,
				DATEADD(DAY, 1, GETDATE())
		FROM	Configuration..GroupBuyingProductGroupConfig
		WHERE	ProductGroupId = @productgroupid;";

        internal const string SqlStr4CreateGroupBuyingUserInfo = @"INSERT	INTO Activity..tbl_GroupBuyingUserInfo
		(	GroupId ,
			UserId ,
			PID ,
			UserStatus ,
			OrderId ,
			IsFinish 
		)
VALUES	(	@groupid ,
			@userid ,
			@pid ,
			0 ,
			@orderid ,
			0 
		);";

        internal const string SqlStr4GetUserGroupInfoCountByUserId = @"SELECT	COUNT(1)
FROM	Activity..tbl_GroupBuyingInfo AS T WITH ( NOLOCK )
		LEFT JOIN Activity..tbl_GroupBuyingUserInfo AS S WITH ( NOLOCK ) ON T.GroupId = S.GroupId
WHERE	S.UserId = @userid
		AND ( @type = 0
				OR T.GroupStatus = @type
			)";

        internal const string SqlStr4GetUserGroupInfoByUserId = @"select T.GroupId,
       S.PID,
       S.OrderId,
       T.CurrentMemberCount,
       T.StartTime,
       T.EndTime,
       T.OwnerId,
       T.GroupStatus,
       S.UserStatus,
       T.ProductGroupId,
       ISNULL(P.LotteryResult, 0) as LotteryResult
from Activity..tbl_GroupBuyingInfo as T with (nolock)
    left join Activity..tbl_GroupBuyingUserInfo as S with (nolock)
        on T.GroupId = S.GroupId
    left join Activity..tbl_GroupBuyingLotteryInfo as P with (nolock)
        on P.OrderId = S.OrderId
where S.UserId = @userid
      and (   @type = 0
              or @type = 5
                 and S.UserStatus = 0
              or @type = 3
                 and S.UserStatus > 2
              or (   @type = 1
                     or @type = 2)
                 and S.UserStatus = @type)
order by S.OrderId desc offset @begin rows fetch next @step rows only;";


        internal const string Sql4GetUserGroupCountByUserId = @"
select S.UserStatus,
       COUNT(1) as Count
from Activity..tbl_GroupBuyingInfo as T with (nolock)
    left join Activity..tbl_GroupBuyingUserInfo as S with (nolock)
        on T.GroupId = S.GroupId
where S.UserId = @userid
group by S.UserStatus";

        internal const string SqlStr4GetGroupCountByUserId = @"
select COUNT(1)
from Activity..tbl_GroupBuyingUserInfo as T with (nolock)
where S.UserId = @userid
      and (   @type = 0
              or @type = 5
                 and S.UserStatus = 0
              or @type = 3
                 and S.UserStatus > 2
              or (   @type = 1
                     or @type = 2)
                 and S.UserStatus = @type);";

        internal const string SqlSter4SelectGroupBuyingUserByGroupId = @"SELECT	OrderId ,
		UserId
FROM	Activity..tbl_GroupBuyingUserInfo WITH ( NOLOCK )
WHERE	GroupId = @groupid
		AND UserStatus < 2;";

        internal const string SqlStr4SetGroupStatus = @"
UPDATE	Activity..tbl_GroupBuyingInfo WITH ( ROWLOCK )
SET		GroupStatus = 3 ,
		LastUpdateDateTime = GETDATE()
WHERE	GroupId = @groupid;";

        internal const string SqlStr4SetProductGroupStatus = @"
update T
set T.CurrentGroupCount = T.CurrentGroupCount - 1,
    T.LastUpdateDateTime = GETDATE()
from Activity..tbl_GroupBuyingInfo as S with (nolock)
    left join Configuration..GroupBuyingProductGroupConfig as T with (rowlock)
        on T.ProductGroupId = S.ProductGroupId
where S.GroupId = @groupid
      and T.CurrentGroupCount > 0;";


        internal const string SqlStr4SetUserStatus = @"
UPDATE	T
SET		T.UserStatus = 3 ,
		T.LastUpdateDateTime = GETDATE()
FROM	Activity..tbl_GroupBuyingUserInfo AS T WITH ( ROWLOCK )
WHERE	T.GroupId = @groupid
		AND T.UserStatus < 2;";


        internal const string SqlStr4SetUserStatus1 = @"
UPDATE	Activity..tbl_GroupBuyingUserInfo WITH ( ROWLOCK )
SET		UserStatus = 4 ,
		LastUpdateDateTime = GETDATE()
OUTPUT	Deleted.UserStatus
WHERE	OrderId = @orderid
		AND UserStatus < 2;";

        internal const string SqlStr4SetGroupStatus1 = @"
UPDATE	T
SET		T.CurrentMemberCount = T.CurrentMemberCount - 1 ,
		T.LastUpdateDateTime = GETDATE()
FROM	Activity..tbl_GroupBuyingInfo AS T WITH ( ROWLOCK )
WHERE	T.GroupId = @groupid;";

        internal const string SqlStr4SetGroupStatus2 = @"
UPDATE	S
SET		S.GroupStatus = 1 ,
		S.StartTime = GETDATE() ,
		S.EndTime = DATEADD(DAY, 1, GETDATE()) ,
		S.LastUpdateDateTime = GETDATE()
FROM	Activity..tbl_GroupBuyingInfo AS S WITH ( ROWLOCK )
WHERE	S.GroupId = @groupid
		AND S.GroupStatus = 0;";

        internal const string SqlStr4SetProductGroupStatus2 = @"UPDATE	P
SET		P.CurrentGroupCount = P.CurrentGroupCount + 1 ,
		P.LastUpdateDateTime = GETDATE()
FROM	Activity..tbl_GroupBuyingInfo AS S WITH ( ROWLOCK )
		LEFT JOIN Configuration..GroupBuyingProductGroupConfig AS P WITH ( ROWLOCK ) ON S.ProductGroupId = P.ProductGroupId
WHERE	P.TotalGroupCount > P.CurrentGroupCount
		AND S.GroupStatus = 0
		AND S.GroupId = @groupid;";

        internal const string SqlStr4SetUserStatus2 = @"UPDATE	T
SET		T.UserStatus = 1 ,
		T.LastUpdateDateTime = GETDATE()
FROM	Activity..tbl_GroupBuyingUserInfo AS T WITH ( ROWLOCK )
WHERE	T.OrderId = @orderid
		AND T.UserStatus = 0;";

        internal const string SqlStr4SetGroupBuyingFinish = @"UPDATE	T
SET		T.GroupStatus = 2 ,
		T.LastUpdateDateTime = GETDATE()
FROM	Activity..tbl_GroupBuyingInfo AS T WITH ( ROWLOCK )
WHERE	T.GroupId = @groupid
		AND T.RequiredMemberCount <= T.CurrentMemberCount;";

        internal const string SqlStr4SetGroupBuyingUserCount = @"UPDATE	Activity..tbl_GroupBuyingInfo WITH ( ROWLOCK )
SET		CurrentMemberCount = CurrentMemberCount + 1 ,
		LastUpdateDateTime = GETDATE()
WHERE	GroupId = @groupid;";


        internal const string SqlStr4ChangeGroupOrderStatus = @"
UPDATE	Activity..tbl_GroupBuyingUserInfo WITH ( ROWLOCK )
SET		IsFinish = 1 ,
		UserStatus = 2 ,
		LastUpdateDateTime = GETDATE()
OUTPUT	Inserted.OrderId ,
		Inserted.UserId
WHERE	GroupId = @groupid
		AND UserStatus = 1;";


        internal const string SqlStr4ChangeUserGroupId = @"UPDATE	Activity..tbl_GroupBuyingUserInfo WITH ( ROWLOCK )
SET		OldGroupId = GroupId ,
		GroupId = @groupid ,
		LastUpdateDateTime = GETDATE()
WHERE	OrderId = @orderid;";

        internal const string SqlStr4FetchUserOrderInfo = @"
select top 1
       T.PID,
       S.OwnerId,
       S.StartTime,
       P.ProductName,
       T.OrderId,
       T.UserStatus,
       P.OriginalPrice,
       P.FinalPrice,
       P.SpecialPrice,
       Q.ShareImage
from Activity..tbl_GroupBuyingUserInfo as T with (nolock)
    left join Activity..tbl_GroupBuyingInfo as S with (nolock)
        on T.GroupId = S.GroupId
    left join Configuration..GroupBuyingProductConfig as P with (nolock)
        on P.ProductGroupId = S.ProductGroupId
           and T.PID = P.PID
    left join Configuration..GroupBuyingProductGroupConfig as Q with (nolock)
        on Q.ProductGroupId = S.ProductGroupId
where T.GroupId = @groupid
      and T.UserId = @userid
order by T.CreateDateTime desc;";


        internal const string SqlStr4FetchProductGroupInfoById = @"
select T.ProductGroupId,
       T.Image,
       T.ShareId,
       T.GroupType,
       T.MemberCount,
       T.Sequence,
       T.CurrentGroupCount,
       T.TotalGroupCount,
       T.BeginTime,
       T.Label,
       T.EndTime,
       S.PID,
       S.ProductName,
       S.OriginalPrice,
       S.FinalPrice,
       S.SpecialPrice,
       T.ActivityId,
       T.GroupCategory,
       T.GroupDescription
from Configuration..GroupBuyingProductGroupConfig as T with (nolock)
    left join Configuration..GroupBuyingProductConfig as S with (nolock)
        on T.ProductGroupId = S.ProductGroupId
where S.DisPlay = 1
      and T.IsDelete = 0
      and T.ProductGroupId = @productgroupid;";
    }
}
