using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.C.Job.Activity.Models;

namespace Tuhu.C.Job.Activity.Dal
{
    public class DalGroupBuying
    {
        public static List<ExpiringGroupInfo> GetExpiringGroupInfo()
        {
            using (var cmd = new SqlCommand(SqlText.SqlStr4GetExpiringGroupInfo))
            {
                return DbHelper.ExecuteSelect<ExpiringGroupInfo>(cmd)?.ToList() ?? new List<ExpiringGroupInfo>();
            }
        }

        public static List<ExpiringUserInfo> GetExpiringUserByGroupId(Guid GroupId)
        {
            using (var cmd = new SqlCommand(SqlText.SqlStr4GetExpiringUserByGroupId))
            {
                cmd.Parameters.AddWithValue("@groupid", GroupId);
                return DbHelper.ExecuteSelect<ExpiringUserInfo>(cmd)?.ToList() ?? new List<ExpiringUserInfo>();
            }
        }

        public static List<ExpiringUserInfo> GetExpiredUserList()
        {
            using (var cmd = new SqlCommand(SqlText.SqlStr4GetExpiredUserList))
            {
                return DbHelper.ExecuteSelect<ExpiringUserInfo>(cmd)?.ToList() ?? new List<ExpiringUserInfo>();
            }
        }

        public static int ChangeUserOrderStatus(int OrderId)
        {
            using (var cmd = new SqlCommand(SqlText.SqlStr4ChangeUserOrderStatus))
            {
                cmd.Parameters.AddWithValue("@orderid", OrderId);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static List<UserOrderJobModel> GetTerribleOrderList()
        {
            const string sqlStr = @"
SELECT  T.UserId , T.OrderId
FROM    ( SELECT    S.UserId ,
                    T.StartTime ,
                    T.OwnerId ,
                    S.OrderId ,
                    P.Status ,
                    P.DeliveryStatus ,
                    ROW_NUMBER() OVER ( PARTITION BY S.UserId ORDER BY S.PKID ASC ) AS OrderNo
          FROM      Activity..tbl_GroupBuyingUserInfo AS S WITH ( NOLOCK )
                    LEFT JOIN Activity..tbl_GroupBuyingInfo AS T WITH ( NOLOCK ) ON S.GroupId = T.GroupId
                    LEFT JOIN Gungnir..vw_tbl_order AS P WITH ( NOLOCK ) ON S.OrderId = P.PKID
          WHERE     S.UserId <> T.OwnerId
                    AND T.GroupType = 1
                    AND T.StartTime > '2017-12-26'
                    AND P.DeliveryDatetime IS NULL
                    AND P.Status <> '7Canceled'
                    AND P.DeliveryStatus in (N'1NotStarted',N'1.5Prepared')
        ) AS T
WHERE   T.OrderNo > 1;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteSelect<UserOrderJobModel>(cmd)?.ToList() ?? new List<UserOrderJobModel>();
            }
        }

        public static int GetExpiringFreeCouponCount(DateTime startTime, DateTime endTime)
        {
            const string sqlStr = @"
select COUNT(distinct UserId)
from Activity..tbl_GroupBuyingFreeCoupons with (nolock)
where OrderId = 0
      and IsDeleted = 0
      and EndDatetime > @startTime
      and EndDatetime < @endTime;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@startTime", startTime);
                cmd.Parameters.AddWithValue("@endTime", endTime);
                var value = DbHelper.ExecuteScalar(true, cmd);
                int.TryParse(value?.ToString(), out var result);
                return result;
            }
        }

        public static List<Guid> GetExpiringFreeCouponList(DateTime startTime, DateTime endTime, int start, int step)
        {
            const string sqlStr = @"
select UserId
from Activity..tbl_GroupBuyingFreeCoupons with (nolock)
where OrderId = 0
      and IsDeleted = 0
      and EndDatetime > @startTime
      and EndDatetime < @endTime
group by UserId
order by UserId desc offset @start rows fetch next @step rows only;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@startTime", startTime);
                cmd.Parameters.AddWithValue("@endTime", endTime);
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@step", step);
                return DbHelper.ExecuteQuery(true, cmd, dt =>
                {
                    var result = new List<Guid>();
                    if (dt == null || dt.Rows.Count < 1) return result;
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var userId = dt.Rows[i].GetValue<Guid>("UserId");
                        if (userId != Guid.Empty) result.Add(userId);
                    }

                    return result;
                });
            }
        }

        public static int GetGroupBuyingCount()
        {
            const string sqlStr = @"select COUNT(*)
from Configuration..GroupBuyingProductGroupConfig with (nolock)
where IsDelete = 0;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                var value = DbHelper.ExecuteScalar(true, cmd);
                int.TryParse(value?.ToString(), out var result);
                return result;
            }
        }

        public static List<string> GetProductGroupList(int start, int step)
        {
            const string sqlStr = @"
select ProductGroupId
from Configuration..GroupBuyingProductGroupConfig with (nolock)
where IsDelete = 0
order by PKID desc offset @start rows fetch next @step rows only;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@step", step);
                return DbHelper.ExecuteQuery(true, cmd, dt =>
                {
                    var result = new List<string>();
                    if (dt == null || dt.Rows.Count < 1) return result;
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var value = dt.Rows[i].GetValue<string>("ProductGroupId");
                        if (!string.IsNullOrWhiteSpace(value)) result.Add(value);
                    }

                    return result;
                });
            }
        }

        public static List<Tuple<Guid, string>> GetGroupInfo()
        {
            const string sqlStr = @"
select distinct
       S.GroupId,
       T.ProductGroupId
from Activity..tbl_GroupBuyingInfo as T with (nolock)
    left join Activity..tbl_GroupBuyingUserInfo as S with (nolock)
        on T.GroupId = S.GroupId
    left join Configuration..GroupBuyingProductGroupConfig as P with (nolock)
        on T.ProductGroupId = P.ProductGroupId
 left join Gungnir..vw_tbl_Order as Q with(nolock) on Q.PKID=S.OrderId
where P.GroupCategory = 2
      and T.CreateDateTime > '2018-03-28'
      and T.GroupStatus = 2
      and S.IsFinish = 1
      and S.CreateCouponResult = 0 and Q.Status<>'7Canceled';";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteQuery(true, cmd, dt =>
                {
                    var result = new List<Tuple<Guid, string>>();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            var groupId = dt.Rows[i].GetValue<Guid>("GroupId");
                            var productGroupId = dt.Rows[i].GetValue<string>("ProductGroupId");
                            if (groupId != Guid.Empty && !string.IsNullOrWhiteSpace(productGroupId))
                            {
                                result.Add(Tuple.Create(groupId, productGroupId));
                            }
                        }
                    }

                    return result;
                });
            }
        }

        /// <summary>
        /// 获取已过期活动拼团信息
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<ExpiredGroupBuyingInfo> GetExpiredGroupBuyingInfo()
        {
            using (var dbHelper = DbHelper.CreateDbHelper(true))
            {
                return dbHelper.ExecuteSelect<ExpiredGroupBuyingInfo>(SqlText.SqlStr4GetExpiredGroupBuyingInfo);
            }
        }

        /// <summary>
        /// 获取未过期拼团信息
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<ExpiredGroupBuyingInfo> GetActiveGroupBuyingInfo()
        {
            using (var dbHelper = DbHelper.CreateDbHelper(true))
            {
                return dbHelper.ExecuteSelect<ExpiredGroupBuyingInfo>(SqlText.SqlStr4GetActiveGroupBuyingInfo);
            }
        }

        public static List<Tuple<int, Guid>> GetOrderList()
        {
            const string sqlStr = @"
select T.OrderId,
       T.UserId
from Activity..tbl_GroupBuyingUserInfo as T with (nolock)
    left join Gungnir..vw_tbl_Order as S with (nolock)
        on T.OrderId = S.PKID
where T.CreateDateTime > '2018-04-25 17:30:00'
      and T.UserStatus = 2
      and S.Status = '0NewPingTuan'";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteQuery(true, cmd, dt =>
                {
                    var result = new List<Tuple<int, Guid>>();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            var orderId = dt.Rows[i].GetValue<int>("OrderId");
                            var userId = dt.Rows[i].GetValue<Guid>("UserId");
                            if (orderId > 0)
                            {
                                result.Add(Tuple.Create(orderId, userId));
                            }
                        }
                    }

                    return result;
                });
            }
        }

        public static List<Tuple<Guid, string>> GetCouponList()
        {
            const string sqlStr = @"select distinct
       P.GroupId,
       P.ProductGroupId
from Activity..tbl_GroupBuyingUserInfo as T with (nolock)
    left join Activity..tbl_GroupBuyingInfo as P with (nolock)
        on T.GroupId = P.GroupId
    left join Configuration..GroupBuyingProductGroupConfig as Q with (nolock)
        on P.ProductGroupId = Q.ProductGroupId
where T.CreateDateTime > '2018-04-26 17:30:00'
      and T.UserStatus = 2
      and Q.GroupCategory = 2
      and T.CreateCouponResult = 0";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteQuery(true, cmd, dt =>
                {
                    var result = new List<Tuple<Guid, string>>();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            var groupId = dt.Rows[i].GetValue<Guid>("GroupId");
                            var pgId = dt.Rows[i].GetValue<string>("ProductGroupId");
                            if (!string.IsNullOrWhiteSpace(pgId))
                            {
                                result.Add(Tuple.Create(groupId, pgId));
                            }
                        }
                    }

                    return result;
                });
            }
        }

        /// <summary>
        /// 获取义乌仓库存不足的团信息
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<ExpiredGroupBuyingInfo> GetYiwuStockOutGroupBuyingInfo()
        {
            IReadOnlyList<string> groups;
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var command = new SqlCommand(SqlText.SqlStr4GetYiwuStockOutProductGroup))
                {
                    groups = dbHelper.ExecuteQuery(command, dt => dt.ToList<string>());
                }
            }
            if (groups.Count <= 0)
            {
                return Enumerable.Empty<ExpiredGroupBuyingInfo>();
            }
            using (var dbHelper = DbHelper.CreateDbHelper(true))
            {
                return dbHelper.ExecuteSelect<ExpiredGroupBuyingInfo>(
                    string.Format(SqlText.SqlStr4GetGroupBuyingInfoByGroupIdFormat, string.Join(", ", groups.Select(_ => $"'{_}'"))));
            }
        }

        public static List<string> GetAllPinTuanProduct()
        {
            const string sqlStr = @"
select ProductGroupId
from Configuration..GroupBuyingProductGroupConfig with (nolock)
where IsDelete = 0";
            using(var cmd=new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteQuery(cmd, dt => dt?.ToList<string>())?.ToList() ?? new List<string>();
            }
        }
        public static List<int> GetPinTuanOrderList()
        {
            const string sqlStr = @"
select T.OrderId
from Activity..tbl_GroupBuyingUserInfo as T with (nolock)
    inner join Activity..tbl_GroupBuyingInfo as S with (nolock)
        on T.GroupId = S.GroupId
    inner join Configuration..GroupBuyingProductGroupConfig as P with (nolock)
        on S.ProductGroupId = P.ProductGroupId
    left join Activity..tbl_GroupBuyingLotteryInfo as Q with (nolock)
        on T.OrderId = Q.OrderId
where T.IsFinish = 1
      and (   P.GroupCategory = 2
              and T.CreateCouponResult = 0
              or P.GroupCategory = 1
                 and Q.PKID is null)
      and T.CreateDateTime > DATEADD(day, -2, GETDATE())
      and T.LastUpdateDateTime < DATEADD(minute, -10, GETDATE())
union all
(select T.OrderId
 from Activity..tbl_GroupBuyingUserInfo as T with (nolock)
     inner join Gungnir..tbl_Order as S with (nolock)
         on T.OrderId = S.PKID
 where T.IsFinish = 1
       and T.CreateDateTime > DATEADD(day, -2, GETDATE())
       and T.LastUpdateDateTime < DATEADD(minute, -10, GETDATE())
       and S.Status = '0NewPingTuan'
       and S.PayStatus = '2Paid');";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteQuery(true, cmd, dt => dt?.ToList<int>())?.ToList() ?? new List<int>();
            }
        }
    }
}
