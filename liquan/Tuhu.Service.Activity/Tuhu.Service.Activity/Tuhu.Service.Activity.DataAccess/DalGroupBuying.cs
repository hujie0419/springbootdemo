using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Service.Activity.Models;
using Tuhu.Models;
using System.Threading;
using Tuhu.Service.Activity.Server.Model;

namespace Tuhu.Service.Activity.DataAccess
{
    public static class DalGroupBuying
    {
        public static async Task<List<string>> SelectGroupBuyingProductsById(string productGroupId)
        {
            List<string> Action(DataTable dt)
            {
                var result = new List<string>();

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var value = dt.Rows[i].GetValue<string>("PID");
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            result.Add(value);
                        }
                    }
                }

                return result;
            }

            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4SelectGroupBuyingProductsById))
            {
                cmd.Parameters.AddWithValue("@productgroupid", productGroupId);
                cmd.CommandType = CommandType.Text;
                return await DbHelper.ExecuteQueryAsync(cmd, Action);

            }
        }

        public static async Task<List<GroupBuyingProductModel>> SelectProductInfoByPid(string productGroupId)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4SelectProductInfoByPid))
            {
                cmd.Parameters.AddWithValue("@productgroupid", productGroupId);
                return (await DbHelper.ExecuteSelectAsync<GroupBuyingProductModel>(true, cmd))?.ToList() ??
                       new List<GroupBuyingProductModel>();
            }
        }


        public static async Task<List<GroupBuyingCategoryModel>> GetGroupBuyingCategoryInfo()
        {
            const string sqlStr = @"
select top 1000
       Id as NewCategoryCode,
       DisplayName as NewCategoryName,
       Sort as SortBy
from Configuration..tbl_OperationCategory with (nolock)
where ParentId = 0
      and IsShow = 1
order by Sort;";
            using(var cmd=new SqlCommand(sqlStr))
            {
                return (await DbHelper.ExecuteSelectAsync<GroupBuyingCategoryModel>(true, cmd))?.ToList() ?? new List<GroupBuyingCategoryModel>();
            }
        }

        public static async Task<List<GroupBuyingCategoryModel>> GetGroupBuyingChildCategory(int oid)
        {
            const string sqlStr = @"select Id as NewCategoryCode,
       DisplayName as NewCategoryName
from Configuration..tbl_OperationCategory with (nolock)
where ParentId = @parentId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@parentId", oid);
                return (await DbHelper.ExecuteSelectAsync<GroupBuyingCategoryModel>(true, cmd))?.ToList() ?? new List<GroupBuyingCategoryModel>();
            }
        }

        public static async Task<List<GroupInfoModel>> SelectGroupInfoByProductGroupId(string productGroupId,
            Guid userId, bool isNewUser, bool flag, bool readOnly = false)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4SelectGroupInfoByProductGroupId))
            {
                cmd.Parameters.AddWithValue("@productgroupid", productGroupId);
                cmd.Parameters.AddWithValue("@isNewUser", isNewUser);
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@flag", flag);
                return (await DbHelper.ExecuteSelectAsync<GroupInfoModel>(readOnly, cmd))?.ToList() ??
                       new List<GroupInfoModel>();
            }
        }

        public static async Task<int> GetGroupInfoCountByProductGroupId(string productGroupId,
            Guid userId, bool isNewUser, bool flag, bool readOnly = false)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4GetGroupInfoCountByProductGroupId))
            {
                cmd.Parameters.AddWithValue("@productgroupid", productGroupId);
                cmd.Parameters.AddWithValue("@isNewUser", isNewUser);
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@flag", flag);
                var result = await DbHelper.ExecuteScalarAsync(readOnly, cmd);
                int.TryParse(result?.ToString(), out var value);
                return value;
            }
        }

        public static async Task<GroupInfoModel> FetchGroupInfoByGroupId(Guid groupId, bool readOnly = false)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4FetchGroupInfoByGroupId))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                return (await DbHelper.ExecuteFetchAsync<GroupInfoModel>(readOnly, cmd)) ?? new GroupInfoModel();
            }
        }

        public static async Task<GroupMemberModel> SelectGroupMemberByGroupId(Guid groupId, bool readOnly = false)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4SelectGroupMemberByGroupId))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);

                GroupMemberModel Action(DataTable dt)
                {
                    var result = new GroupMemberModel();

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            var userId = dt.Rows[i].GetValue<Guid>("UserId");
                            var orderId = dt.Rows[i].GetValue<int>("OrderId");
                            var createdTime = dt.Rows[i].GetValue<DateTime>("CreateDateTime");
                            var ownerId = dt.Rows[i].GetValue<Guid>("OwnerId");
                            if (userId != Guid.Empty && orderId > 0)
                            {
                                result.UserItems.Add(new UserOrderModel
                                {
                                    UserId = userId,
                                    CreatedTime = createdTime,
                                    OrderId = orderId,
                                    IsCaptain = userId == ownerId
                                });
                                result.Items.Add(userId);
                            }
                        }

                        result.OwnerId = dt.Rows[0].GetValue<Guid>("OwnerId");
                    }

                    return result;
                }

                return (await DbHelper.ExecuteQueryAsync(readOnly, cmd, Action)) ?? new GroupMemberModel();
            }
        }

        public static async Task<int> CheckUserGroupInfo(Guid groupId, Guid userId, ILog logger, bool readOnly = false)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4CheckUserGroupInfo))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                cmd.Parameters.AddWithValue("@userid", userId);
                var dat = await DbHelper.ExecuteScalarAsync(readOnly, cmd);
                if (int.TryParse(dat?.ToString(), out var value))
                {
                    return value;
                }

                logger.Warn($"查询用户参团状态，对象转换失败，CheckUserGroupInfo/{groupId}/{userId}");
                return 0;
            }
        }

        public static async Task<VerificationResultModel> CheckProductGroupInfo(string productGroupId)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4CheckProductGroupInfo))
            {
                cmd.Parameters.AddWithValue("@productgroupid", productGroupId);
                return (await DbHelper.ExecuteFetchAsync<VerificationResultModel>(false, cmd)) ??
                       new VerificationResultModel
                       {
                           Code = 4,
                           Info = "未查询到该产品"
                       };
            }
        }

        public static async Task<int> CreateGroupBuyingGroupInfo(Guid userId, string productGroupId,
            Guid groupId, BaseDbHelper dbHelper)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4CreateGroupBuyingGroupInfo))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@productgroupid", productGroupId);
                return await dbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        public static async Task<int> CreateGroupBuyingUserInfo(Guid userId, Guid groupId,
            String pid, int orderId, BaseDbHelper dbHelper)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4CreateGroupBuyingUserInfo))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@orderid", orderId);
                return await dbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        public static async Task<PagedModel<UserGroupBuyingInfoModel>>
            GetUserGroupInfoByUserId(GroupInfoRequest request, ILog logger)
        {
            var result = new PagedModel<UserGroupBuyingInfoModel>()
            {
                Pager = new PagerModel()
                {
                    PageSize = request.PageSize,
                    CurrentPage = request.PageIndex
                }
            };
            result.Pager.Total = await GetUserGroupInfoCountByUserId(request.UserId, request.Type, logger);
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4GetUserGroupInfoByUserId))
            {
                cmd.Parameters.AddWithValue("@userid", request.UserId);
                cmd.Parameters.AddWithValue("@type", request.Type);
                cmd.Parameters.AddWithValue("@begin", (request.PageIndex - 1) * request.PageSize);
                cmd.Parameters.AddWithValue("@step", request.PageSize);
                result.Source = (await DbHelper.ExecuteSelectAsync<UserGroupBuyingInfoModel>(true, cmd))?.ToList() ??
                                new List<UserGroupBuyingInfoModel>();
            }

            return result;
        }
        public static async Task<GroupBuyingHistoryCount> GetUserGroupCountByUserId(Guid userId)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.Sql4GetUserGroupCountByUserId))
            {
                cmd.Parameters.AddWithValue("@userid", userId);
                var data = (await DbHelper.ExecuteSelectAsync<GroupBuyingUserStatusCount>(true, cmd))?.ToList() ?? new List<GroupBuyingUserStatusCount>();
                return new GroupBuyingHistoryCount
                {
                    TotalItemCount = data.Sum(g => g.Count),
                    UnpaidItemCount = data.Where(g => g.UserStatus == 0).Sum(g => g.Count),
                    UnderwayItemCount = data.Where(g => g.UserStatus == 1).Sum(g => g.Count),
                    SuccessfulItemCount = data.Where(g => g.UserStatus == 2).Sum(g => g.Count),
                    UnsuccessfulItemCount = data.Where(g => g.UserStatus == 3 || g.UserStatus == 4).Sum(g => g.Count)
                };

            }
        }

        private static async Task<int> GetUserGroupInfoCountByUserId(Guid userId, int type, ILog logger)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4GetUserGroupInfoCountByUserId))
            {
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@type", type);
                var dat = await DbHelper.ExecuteScalarAsync(true, cmd);
                var value = 0;
                if (Int32.TryParse(dat?.ToString(), out value))
                {
                    return value;
                }

                logger.Warn($"查询用户拼团记录数据转换失败，GetUserGroupInfoCountByUserId/{userId}/{type}");
                return 0;
            }
        }

        public static async Task<List<UserOrderModel>> SelectGroupBuyingUserByGroupId(Guid groupId)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlSter4SelectGroupBuyingUserByGroupId))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                return (await DbHelper.ExecuteSelectAsync<UserOrderModel>(true, cmd))?.ToList() ??
                       new List<UserOrderModel>();
            }
        }

        public static async Task<bool> SetGroupStatus(BaseDbHelper dbhelper, Guid groupId)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4SetGroupStatus))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                return (await dbhelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        public static async Task<bool> SetProductGroupStatus(BaseDbHelper dbHelper, Guid groupId)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4SetProductGroupStatus))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                return (await dbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        public static async Task<bool> SetUserStatus(BaseDbHelper dbHelper, Guid groupId)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4SetUserStatus))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                return (await dbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        public static async Task<int> SetUserOrderCancel(int orderId, BaseDbHelper dbHelper)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4SetUserStatus1))
            {
                cmd.Parameters.AddWithValue("@orderid", orderId);
                var data = await dbHelper.ExecuteScalarAsync(cmd);
                int.TryParse(data?.ToString(), out var value);
                return value;
            }
        }

        public static async Task<int> SetGroupStatus(Guid groupId, bool isCancel, BaseDbHelper dbHelper)
        {
            var sqlStr = isCancel
                ? DalgroupBuyingText.SqlStr4SetGroupStatus1
                : DalgroupBuyingText.SqlStr4SetGroupStatus2;
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                return await dbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        public static async Task<bool> SetProductGroupStatus2(Guid groupId, BaseDbHelper dbHelper)
        {
            var sqlStr = DalgroupBuyingText.SqlStr4SetProductGroupStatus2;
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                return (await dbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        public static async Task<bool> SetGroupStatus2(Guid groupId, BaseDbHelper dbHelper)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4SetGroupStatus2))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                return (await dbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }



        public static async Task<bool> SetUserStatus2(int orderId, BaseDbHelper dbHelper)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4SetUserStatus2))
            {
                cmd.Parameters.AddWithValue("@orderid", orderId);
                return (await dbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }


        public static async Task<bool> SetGroupBuyingUserCount(Guid groupId, BaseDbHelper dbHelper)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4SetGroupBuyingUserCount))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                return (await dbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        public static bool SetGroupBuyingFinish(Guid groupId, BaseDbHelper dbHelper)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4SetGroupBuyingFinish))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static int SetGroupBuyingStatus(Guid groupId, BaseDbHelper dbHelper)
        {
            const string sqlStr = @"
update Activity..tbl_GroupBuyingInfo with (rowlock)
set CurrentMemberCount = CurrentMemberCount + 1,
    GroupStatus = IIF(RequiredMemberCount - 1 <= CurrentMemberCount, 2, GroupStatus),
    LastUpdateDateTime = GETDATE()
output Inserted.GroupStatus
where GroupId = @groupid;";
            using (var cmd=new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                var value = dbHelper.ExecuteScalar(cmd);
                int.TryParse(value?.ToString(), out var result);
                return result;
            }
        }


        public static List<UserOrderModel> ChangeGroupOrderStatus(Guid groupId)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4ChangeGroupOrderStatus))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                return DbHelper.ExecuteSelect<UserOrderModel>(cmd)?.ToList() ?? new List<UserOrderModel>();
            }
        }

        public static async Task<bool> ChangeUserGroupId(Guid groupId, int orderId)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4ChangeUserGroupId))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                cmd.Parameters.AddWithValue("@orderid", orderId);
                return (await DbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        public static async Task<UserOrderInfoModel> FetchUserOrderInfo(Guid groupId, Guid userId, bool readOnly)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4FetchUserOrderInfo))
            {
                cmd.Parameters.AddWithValue("@groupid", groupId);
                cmd.Parameters.AddWithValue("@userid", userId);
                return await DbHelper.ExecuteFetchAsync<UserOrderInfoModel>(readOnly, cmd);
            }
        }

        public static async Task<GroupInfoModel> FetchGroupInfoByOrderId(int orderId)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.Sql4FetchGroupInfoByOrderId))
            {
                cmd.Parameters.AddWithValue("@orderid", orderId);
                return await DbHelper.ExecuteFetchAsync<GroupInfoModel>(false, cmd);
            }
        }

        public static async Task<ProductGroupModel> FetchProductGroupInfoById(string productGroupId)
        {
            using (var cmd = new SqlCommand(DalgroupBuyingText.SqlStr4FetchProductGroupInfoById))
            {
                cmd.Parameters.AddWithValue("@productgroupid", productGroupId);
                return await DbHelper.ExecuteFetchAsync<ProductGroupModel>(cmd);
            }
        }

        public static async Task<List<GroupMemberInfo>> GetGroupMemberInfoBy(Guid groupId, bool readOnly = false)
        {
            const string sqlStr = @"
select T.OrderId,
       T.UserId,
       S.PID,
       S.ProductName
from Activity..tbl_GroupBuyingUserInfo as T with (nolock)
    left join Activity..tbl_GroupBuyingInfo as Q with (nolock)
        on T.GroupId = Q.GroupId
    left join Configuration..GroupBuyingProductConfig as S with (nolock)
        on T.PID = S.PID
           and Q.ProductGroupId = S.ProductGroupId
where T.GroupId = @groupId
      and (   Q.GroupStatus <> 2
              and (   T.UserStatus = 1
                      or T.UserStatus = 3)
              or T.IsFinish = 1)
order by T.CreateDateTime;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@groupId", groupId);
                return (await DbHelper.ExecuteSelectAsync<GroupMemberInfo>(readOnly, cmd))?.ToList() ??
                       new List<GroupMemberInfo>();
            }

        }




        public static async Task<List<Guid>> GetRelaedUserId(Guid userid)
        {
            List<Guid> Action(DataTable dt)
            {
                var result = new List<Guid>();

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var value = dt.Rows[i].GetValue<Guid>("UserId");
                        if (value != Guid.Empty)
                        {
                            result.Add(value);
                        }
                    }
                }

                return result;
            }

            const string sqlStr = @"SELECT DISTINCT
        T.UserId
FROM    Tuhu_profiles..UserAuth AS T WITH ( NOLOCK )
        LEFT JOIN Tuhu_profiles..UserAuth AS S WITH ( NOLOCK ) ON S.OpenId = T.OpenId
WHERE   T.UserId = @userid;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@userid", userid);
                return await DbHelper.ExecuteQueryAsync(true, cmd, Action);
            }
        }

        public static async Task<int> FetchTagNoById(string productGroupId)
        {
            var sqlStr = @"SELECT  T.SpecialUser
FROM    Configuration..GroupBuyingProductGroupConfig AS T WITH ( NOLOCK )
WHERE   ProductGroupId = @productgroupid;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@productgroupid", productGroupId);
                var result = await DbHelper.ExecuteScalarAsync(true, cmd);
                int.TryParse(result?.ToString(), out var value);
                return value;
            }
        }


        public static async Task<bool> CheckSpecialUserTag(int tagNo, string phoneNumber)
        {
            const string sqlStr = @"SELECT  COUNT(1)
FROM    Tuhu_log..tbl_GroupSpecialUser AS S WITH ( NOLOCK )
WHERE   S.TagNo = @tagno
        AND S.PhoneNumber = @phonenumber
        AND S.EndTime > GETDATE();";
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@tagno", tagNo);
                cmd.Parameters.AddWithValue("@phonenumber", phoneNumber);
                var result = await dbHelper.ExecuteScalarAsync(cmd);
                int.TryParse(result?.ToString(), out int value);
                return value > 0;
            }
        }

        public static async Task<GroupBuyingProductInfo> GetProductGroupInfoByPId(string pId)
        {
            const string sqlStr = @"
select T.PID,
       T.ProductGroupId,
       T.FinalPrice as Price,
       S.BeginTime as StartTime
from Configuration..GroupBuyingProductConfig as T with (nolock)
    inner join Configuration..GroupBuyingProductGroupConfig as S with (nolock)
        on T.ProductGroupId = S.ProductGroupId
where T.PID = @Pid
      and T.IsDelete = 0
      and S.IsDelete = 0;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@Pid", pId);
                var result = (await DbHelper.ExecuteSelectAsync<GroupBuyingProductItem>(true, cmd))?.ToList() ??
                             new List<GroupBuyingProductItem>();
                return new GroupBuyingProductInfo
                {
                    PId = pId,
                    ProductGroupList = result
                };
            }
        }


        public static async Task<bool> CheckProductGroupId(Guid activityId)
        {
            const string sqlStr = @"
select COUNT(1)
from Configuration..GroupBuyingProductGroupConfig with (nolock)
where IsDelete = 0
      and ActivityId = @activityId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@activityId", activityId);
                var result = await DbHelper.ExecuteScalarAsync(true, cmd);
                int.TryParse(result?.ToString(), out var value);
                return value > 0;
            }
        }


        public static int SetGroupBuyingLottery(Guid groupId)
        {
            const string sqlStr = @"
insert into Activity..tbl_GroupBuyingLotteryInfo
(
    GroupId,
    ProductGroupId,
    UserId,
    OrderId
)
select T.GroupId,
       S.ProductGroupId,
       T.UserId,
       T.OrderId
from Activity..tbl_GroupBuyingUserInfo as T with (nolock)
    left join Activity..tbl_GroupBuyingInfo as S with (nolock)
        on S.GroupId = T.GroupId
where S.GroupId = @groupId
      and T.IsFinish = 1
      and T.UserStatus = 2";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@groupId", groupId);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static async Task<GroupLotteryRuleModel> GetLotteryRule(string productGroupId)
        {
            const string sqlStr = @"
select top 1
       GroupDescription as RuleDescription,
       BeginTime,
       EndTime
from Configuration..GroupBuyingProductGroupConfig with (nolock)
where ProductGroupId = @productGroupId
      and IsDelete = 0;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                return await DbHelper.ExecuteFetchAsync<GroupLotteryRuleModel>(true, cmd);
            }
        }

        public static async Task<int> GetWinnerCount(string productGroupId, int level)
        {
            const string sqlStr = @"
select COUNT(1)
from Activity..tbl_GroupBuyingLotteryInfo with (nolock)
where ProductGroupId = @productGroupId
      and LotteryResult <> 0
      and (   @level = 0
              or @level <> 0
                 and LotteryResult = @level)
      and IsDelete = 0";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                cmd.Parameters.AddWithValue("@level", level);
                var result = await DbHelper.ExecuteScalarAsync(true, cmd);
                int.TryParse(result?.ToString(), out var value);
                return value;
            }

        }

        public static async Task<List<GroupBuyingLotteryInfo>> GetWinnerList(string productGroupId, int level,
            int pageIndex, int pageSize)
        {
            const string sqlStr = @"
select UserId,
       LotteryResult as Level,
       OrderId
from Activity..tbl_GroupBuyingLotteryInfo with (nolock)
where ProductGroupId = @productGroupId
      and LotteryResult <> 0
      and (   @level = 0
              or @level <> 0
                 and LotteryResult = @level)
      and IsDelete = 0
order by LotteryResult offset @start rows fetch next @step rows only";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                cmd.Parameters.AddWithValue("@level", level);
                cmd.Parameters.AddWithValue("@start", (pageIndex - 1) * pageSize);
                cmd.Parameters.AddWithValue("@step", pageSize);
                return (await DbHelper.ExecuteSelectAsync<GroupBuyingLotteryInfo>(true, cmd))?.ToList() ??
                       new List<GroupBuyingLotteryInfo>();
            }

        }

        public static async Task<GroupBuyingLotteryInfo> CheckUserLotteryResult(Guid userId, string productGroupId,
            int orderId)
        {
            const string sqlStr = @"
select top 1
       UserId,
       OrderId,
       LotteryResult as Level
from Activity..tbl_GroupBuyingLotteryInfo with (nolock)
where IsDelete = 0
      and OrderId = @orderId
      and ProductGroupId = @productGroupId
      and UserId = @userId
      and LotteryResult <> 0;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                cmd.Parameters.AddWithValue("@userId", userId);
                return await DbHelper.ExecuteFetchAsync<GroupBuyingLotteryInfo>(true, cmd);
            }
        }

        public static async Task<List<GroupBuyingLotteryInfo>> GetUserLotteryHistory(Guid userId)
        {
            const string sqlStr = @"select UserId,
       OrderId,
       LotteryResult as Level
from Activity..tbl_GroupBuyingLotteryInfo with (nolock)
where IsDelete = 0
      and UserId = @userId
      and LotteryResult <> 0;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                return (await DbHelper.ExecuteSelectAsync<GroupBuyingLotteryInfo>(true, cmd))?.ToList() ??
                       new List<GroupBuyingLotteryInfo>();
            }
        }

        public static async Task<int> GetAvailableGroupBuyingCount(int groupType, string channel)
        {
            #region sqlStr

            const string sqlStr = @"
select COUNT(1)
from Configuration..GroupBuyingProductGroupConfig as T with (nolock)
where T.IsDelete = 0
      and T.EndTime > GETDATE()
      and T.BeginTime < GETDATE()
      and T.CurrentGroupCount < T.TotalGroupCount
      and T.IsShow = 1
      and (   @groupType = -99
              and T.GroupType in ( 0, 1, 2 )
              or @groupType = T.GroupType)
      and (   @channel is null
              or (   @channel is not null
                     and CHARINDEX(@channel, Channel) > 0))";

            #endregion

            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@groupType", groupType);
                cmd.Parameters.AddWithValue("@channel", string.IsNullOrWhiteSpace(channel) ? null : channel);
                var result = await DbHelper.ExecuteScalarAsync(true, cmd);
                int.TryParse(result?.ToString(), out var value);
                return value;
            }
        }

        public static async Task<List<string>> GetAvailableGroupBuyingList(int pageIndex, int pageSize, bool flag, bool isOldUser,
            int groupType, string channel)
        {
            #region sqlStr
            string sortRuleSql = string.Empty;
            string caseWhenSql = string.Empty;
            string caseWhenStr = string.Empty;

            #region 排序规则
            if (flag)
            {
                //抽奖团>1分团>低价团 , 优惠券团>低价团 > 精品团
                var newUserSort = new List<string> { "低价团", "精品团", "优惠券团" };
                var oldUserSort = new List<string> { "优惠券团", "低价团", "精品团" };
                if (isOldUser)
                {
                    caseWhenStr = string.Join(" ", oldUserSort.Select((x, ind) => $" when label=N'{x}' then {ind} "));
                    caseWhenSql = $" case {caseWhenStr} ELSE {oldUserSort.Count} END AS labelInt ";
                }
                else
                {
                    caseWhenStr = string.Join("", newUserSort.Select((x, ind) => $" when label=N'{x}' then {ind} "));
                    caseWhenSql = $" case {caseWhenStr} ELSE {newUserSort.Count} END AS labelInt ";
                }
                caseWhenSql = $",{caseWhenSql}";
                sortRuleSql = "order by labelInt asc,CurrentGroupCount desc";
            }
            else
                sortRuleSql = "order by Sequence desc,CurrentGroupCount desc"; 
            #endregion

            string sqlStr = $@"
select T.ProductGroupId{caseWhenSql} 
from Configuration..GroupBuyingProductGroupConfig as T with (nolock)
where T.IsDelete = 0
      and T.EndTime > GETDATE()
      and T.BeginTime < GETDATE()
      and T.CurrentGroupCount < T.TotalGroupCount
      and T.IsShow = 1
      and (   @groupType = -99
              and T.GroupType in ( 0, 1, 2 )
              or @groupType = T.GroupType)
      and (   @channel is null
              or (   @channel is not null
                     and CHARINDEX(@channel, Channel) > 0))
 {sortRuleSql} offset @start rows fetch next @step rows only;";

            #endregion

            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@flag", flag);
                cmd.Parameters.AddWithValue("@groupType", groupType);
                cmd.Parameters.AddWithValue("@start", pageSize * (pageIndex - 1));
                cmd.Parameters.AddWithValue("@channel", string.IsNullOrWhiteSpace(channel) ? null : channel);
                cmd.Parameters.AddWithValue("@step", pageSize);
                return await DbHelper.ExecuteQueryAsync(true, cmd, dt =>
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

        public static async Task<Tuple<int, Guid, string>> GetOwnerAndGPIdByGroupId(Guid groupId)
        {
            const string sqlStr = @"
select top 1
       GroupType,
       OwnerId,
       ProductGroupId
from Activity..tbl_GroupBuyingInfo with (nolock)
where GroupId = @groupId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@groupId", groupId);
                return await DbHelper.ExecuteQueryAsync(true, cmd, dt =>
                {
                    if (dt == null || dt.Rows.Count < 1) return null;
                    var item1 = dt.Rows[0].GetValue<int>("GroupType");
                    var item2 = dt.Rows[0].GetValue<Guid>("OwnerId");
                    var item3 = dt.Rows[0].GetValue<string>("ProductGroupId");
                    return Tuple.Create(item1, item2, item3);
                });
            }
        }

        public static async Task<List<GroupFinalUserModel>> GetGroupFinalUserList(Guid groupId)
        {
            const string sqlStr = @"
select T.UserId,
       T.OrderId,
       T.PID,
       T.CreateCouponResult,
       case
           when T.UserId = S.OwnerId then
               1
           else
               0
       end as IsCaptain
from Activity..tbl_GroupBuyingUserInfo as T with (nolock)
    left join Activity..tbl_GroupBuyingInfo as S with (nolock)
        on T.GroupId = S.GroupId
where T.GroupId = @groupId
      and IsFinish = 1;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@groupId", groupId);
                return (await DbHelper.ExecuteSelectAsync<GroupFinalUserModel>(true, cmd))?.ToList() ??
                       new List<GroupFinalUserModel>();
            }
        }

        public static async Task<int> GetUserOrderCountInProductGrouop(Guid userId, string productGroupId, string pid, bool readOnly)
        {
            const string sqlStr = @"select COUNT(distinct T.OrderId)
from Activity..tbl_GroupBuyingUserInfo as T with (nolock)
    left join Activity..tbl_GroupBuyingInfo as S with (nolock)
        on T.GroupId = S.GroupId
where T.UserId = @userId
      and S.ProductGroupId = @productGroupId
      and T.PID = @pid
      and (   T.UserStatus = 0
              or T.UserStatus = 1
              or T.IsFinish = 1)";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                var value = await DbHelper.ExecuteScalarAsync(readOnly, cmd);
                int.TryParse(value?.ToString(), out var result);
                return result;
            }
        }


        #region 团长免单

        public static async Task<bool> RelateFreeCouponAndOrderId(int orderId, Guid userId)
        {
            #region sqlStr

            const string sqlStr = @"
update Activity..tbl_GroupBuyingFreeCoupons with (rowlock)
set OrderId = @orderId,
    Remark += @remark,
    LastUpdateDateTime = GETDATE()
where PKID = (   select top 1
                        PKID
                 from Activity..tbl_GroupBuyingFreeCoupons with (nolock)
                 where UserId = @userId
                       and OrderId = 0
                       and EndDatetime > GETDATE()
                 order by EndDatetime asc);";

            #endregion

            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("remark", $"/团长免单==>{orderId}");
                return (await DbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        public static async Task<bool> CreateFreeCoupons(Guid groupId, TimeSpan freeCouponSpan)
        {
            #region sqlStr

            const string sqlStr = @"
insert into Activity..tbl_GroupBuyingFreeCoupons
(
    UserId,
    StartDatetime,
    EndDatetime,
    PreOrderId,
    Remark
)
select U.UserId,
       @startTime,
       @endTime,
       U.OrderId,
       @remark
from Activity..tbl_GroupBuyingUserInfo as U with (nolock)
where U.IsFinish = 1
      and U.GroupId = @groupId;";

            #endregion

            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@startTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@endTime", DateTime.Now + freeCouponSpan);
                cmd.Parameters.AddWithValue("@groupId", groupId);
                cmd.Parameters.AddWithValue("@remark", $"{groupId:D}");
                return (await DbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        public static async Task<bool> ReleaseFreeCoupon(int orderId)
        {
            #region sqlStr

            const string sqlStr = @"
update Activity..tbl_GroupBuyingFreeCoupons with (rowlock)
set OrderId = 0,
    Remark += @remak,
    LastUpdateDateTime = GETDATE()
where OrderId = @orderId;";

            #endregion

            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);
                cmd.Parameters.AddWithValue("@remak", "/订单取消");
                return (await DbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        public static async Task<List<FreeCouponModel>> GetUserFreeCoupon(Guid userId, bool readOnly = true)
        {
            #region sqlStr

            const string sqlStr = @"
select StartDatetime as StartTime,
       EndDatetime as EndTime,
       PreOrderId
from Activity..tbl_GroupBuyingFreeCoupons with (nolock)
where UserId = @userId
      and IsDeleted = 0
      and OrderId = 0
      and EndDatetime > GETDATE();";

            #endregion

            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                return (await DbHelper.ExecuteSelectAsync<FreeCouponModel>(readOnly, cmd))?.ToList() ??
                       new List<FreeCouponModel>();
            }
        }

        #endregion



        public static async Task<BuyLimitAndOrderLimitModel> GetBuyLimitStaticInfo(Guid activityId, string pid)
        {
            const string sqlStr = @"select T.ProductGroupId,
       T.PID,
       T.BuyLimitCount,
       T.UpperLimitPerOrder
from Configuration..GroupBuyingProductConfig as T with (nolock)
    left join Configuration..GroupBuyingProductGroupConfig as S with (nolock)
        on T.ProductGroupId = S.ProductGroupId
where S.ActivityId = @activityId
      and T.PID = @pid;";
            using(var cmd=new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@activityId", activityId);
                cmd.Parameters.AddWithValue("@pid", pid);
                return await DbHelper.ExecuteFetchAsync<BuyLimitAndOrderLimitModel>(true, cmd);
            }
        }

        public static async Task<BuyLimitAndOrderLimitModel> GetBuyLimitInfo(Guid activityId, string pid, Guid userId,
            bool readOnly)
        {
            const string sqlStr = @"
select T.ProductGroupId,
       T.PID,
       T.BuyLimitCount,
       COUNT(distinct P.OrderId) as CurrentOrderCount,
       @userId as UserId,
       T.UpperLimitPerOrder
from Configuration..GroupBuyingProductGroupConfig as G with (nolock)
    left join Configuration..GroupBuyingProductConfig as T with (nolock)
        on G.ProductGroupId = T.ProductGroupId
    left join Activity..tbl_GroupBuyingInfo as S with (nolock)
        on T.ProductGroupId = S.ProductGroupId
           and S.GroupStatus < 3
    left join Activity..tbl_GroupBuyingUserInfo as P with (nolock)
        on S.GroupId = P.GroupId
           and T.PID = P.PID
           and (   P.UserStatus < 2
                   or P.IsFinish = 1)
           and P.UserId = @userId
where G.ActivityId = @activityId
      and T.PID = @pid
group by T.ProductGroupId,
         T.PID,
         T.BuyLimitCount,
         T.UpperLimitPerOrder;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@activityId", activityId);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@userId", userId);
                return await DbHelper.ExecuteFetchAsync<BuyLimitAndOrderLimitModel>(readOnly, cmd);
            }
        }


        public static async Task<GroupBuyingBuyLimitModel> GetUserBuyInfo(string pid, string productGroupId)
        {
            const string sqlStr = @"
select T.ProductGroupId,
       T.PID,
       T.BuyLimitCount
from Configuration..GroupBuyingProductConfig as T with (nolock)
where T.ProductGroupId = @productGroupid
      and T.PID = @pid;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                return await DbHelper.ExecuteFetchAsync<GroupBuyingBuyLimitModel>(true, cmd);
            }
        }

        public static async Task<GroupBuyingBuyLimitModel> GetUserBuyLimitInfo(string pid, string productGroupId, Guid userId, bool readOnly)
        {
            const string sqlStr = @"
select COUNT(distinct S.OrderId) as CurrentOrderCount
from Activity..tbl_GroupBuyingInfo as T with (nolock)
    left join Activity..tbl_GroupBuyingUserInfo as S with (nolock)
        on T.GroupId = S.GroupId
where T.GroupStatus < 3
      and (   S.UserStatus < 2
              or S.IsFinish = 1)
      and S.UserId = @userId
      and T.ProductGroupId = @productGroupId
      and S.PID = @pid;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@productGroupId", productGroupId);
                cmd.Parameters.AddWithValue("@userId", userId);
                return await DbHelper.ExecuteFetchAsync<GroupBuyingBuyLimitModel>(readOnly, cmd);
            }
        }


        #region tmp

        public static async Task<List<ProductGroupModel>> GetGroupBuyingProductList(List<string> productGroupIds)
        {
            #region sqlStr

            const string sqlStr = @"
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
       T.GroupDescription,
       T.ShareImage,
       S.UseCoupon,
       S.UpperLimitPerOrder
from Configuration..SplitString(@ids, ',', 1) as P
    left join Configuration..GroupBuyingProductGroupConfig as T with (nolock)
        on T.ProductGroupId = P.Item
    left join Configuration..GroupBuyingProductConfig as S with (nolock)
        on T.ProductGroupId = S.ProductGroupId
where S.DisPlay = 1
      and T.IsDelete = 0;";

            #endregion

            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@ids", string.Join(",", productGroupIds));
                return (await DbHelper.ExecuteSelectAsync<ProductGroupModel>(false, cmd))?.ToList() ??
                       new List<ProductGroupModel>();
            }
        }

        public static async Task<List<GroupBuyingUserModel>> GetGroupBuyingUserList(List<string> productGroupIds)
        {
            #region sqlStr

            const string sqlStr = @"
select distinct
       P.ProductGroupId,
       P.UserId
from
(   select T.ProductGroupId,
           ROW_NUMBER() over (partition by T.ProductGroupId
                              order by S.CreateDateTime desc,
                                       T.EndTime desc) as UserNo,
           S.UserId
    from Activity..SplitString(@ids, ',', 1) as Q
        left join Activity..tbl_GroupBuyingInfo as T with (nolock)
            on T.ProductGroupId = Q.Item
        left join Activity..tbl_GroupBuyingUserInfo as S with (nolock)
            on T.GroupId = S.GroupId
    where GETDATE() > T.StartTime
          and T.GroupStatus > 0) as P
where P.UserNo < 5;";

            #endregion

            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@ids", string.Join(",", productGroupIds));
                return (await DbHelper.ExecuteSelectAsync<GroupBuyingUserModel>(true, cmd))?.ToList() ??
                       new List<GroupBuyingUserModel>();
            }
        }

        public static async Task<List<GroupBuyingUserCount>> GetGroupBuyingUserCountList(List<string> productGroupIds)
        {
            #region sqlStr

            const string sqlStrForMany = @"
select S.ProductGroupId,
       COUNT(T.OrderId) as Count
from Configuration..GroupBuyingProductConfig as S with (nolock)
    left join Activity..tbl_GroupBuyingUserInfo as T with (nolock)
        on T.PID = S.PID
where T.UserStatus > 0
      and S.ProductGroupId in ( select Item from Configuration..SplitString(@ids, ',', 1) as Q )
group by S.ProductGroupId;";


            const string sqlStrForOne = @"
select S.ProductGroupId,
       COUNT(T.OrderId) as Count
from Configuration..GroupBuyingProductConfig as S with (nolock)
    left join Activity..tbl_GroupBuyingUserInfo as T with (nolock)
        on T.PID = S.PID
where T.UserStatus > 0
      and S.ProductGroupId = @ids
group by S.ProductGroupId;";


            var sqlStr = productGroupIds.Count == 1 ? sqlStrForOne : sqlStrForMany;

            #endregion


            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@ids", string.Join(",", productGroupIds));
                return (await DbHelper.ExecuteSelectAsync<GroupBuyingUserCount>(true, cmd))?.ToList() ??
                       new List<GroupBuyingUserCount>();
            }
        }

        #endregion


        #region ES查询

        public static async Task<List<ESGroupBuyingGroupModel>> GetGroupBuyingGroupInfo(List<string> ids)
        {
            const string sqlStr = @"
select T.ShareImage,
       GroupDescription,
       GroupCategory,
       T.IsShow,
       ActivityId,
       Label,
       EndTime,
       BeginTime,
       TotalGroupCount,
       CurrentGroupCount,
       Sequence,
       MemberCount,
       GroupType,
       ShareId,
       Image,
       T.ProductGroupId,
       Channel,
       SpecialUser,
       S.PID as DefaultProduct
from Configuration..GroupBuyingProductGroupConfig as T with (nolock)
    left join Configuration..GroupBuyingProductConfig as S with (nolock)
        on S.ProductGroupId = T.ProductGroupId
           and S.DisPlay = 1
where T.IsDelete = 0
      and T.ProductGroupId in ( select Item from Configuration..SplitString(@ids, ',', 1) );";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@ids", string.Join(",", ids));
                return (await DbHelper.ExecuteSelectAsync<ESGroupBuyingGroupModel>(true, cmd))?.ToList() ?? new List<ESGroupBuyingGroupModel>();
            }
        }

        public static async Task<List<ESGroupBuyingProductModel>> GetGroupBuyingProductInfo(List<string> ids)
        {
            const string sqlStr = @"select S.PID,
       S.ProductName,
       S.OriginalPrice,
       S.FinalPrice as ActivityPrice,
       S.SpecialPrice as CaptainPrice,
       S.UseCoupon,
       S.UpperLimitPerOrder,
       S.BuyLimitCount,
       S.DisPlay as IsDefault,
       S.IsShow,
       T.ProductGroupId,
       T.ActivityId,
       T.IsShow as GroupIsShow,
       T.GroupType,
       T.Sequence,
       T.Channel,
       T.SpecialUser,
       T.BeginTime,
       T.EndTime,
       T.Label,
       T.GroupCategory
from Configuration..GroupBuyingProductGroupConfig as T with (nolock)
    inner join Configuration..GroupBuyingProductConfig as S with (nolock)
        on T.ProductGroupId = S.ProductGroupId
where S.IsDelete = 0
      and T.IsDelete = 0
      and T.ProductGroupId in ( select Item from Configuration..SplitString(@ids, ',', 1) );";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@ids", string.Join(",", ids));
                return (await DbHelper.ExecuteSelectAsync<ESGroupBuyingProductModel>(true, cmd))?.ToList() ?? new List<ESGroupBuyingProductModel>();
            }
        }

        public static async Task<List<GetReleaseCategoryInfo>> GetReleaseCategory(List<int> oids)
        {
            var sqlStr = $@"select S.OId,
       T.CategoryCode,
       T.DisplayName
from Configuration..tbl_OperationCategory as T with (nolock)
    left join Configuration..tbl_OperationCategory_Products as S with (nolock)
        on S.OId = T.Id
where S.CorrelId in ( {string.Join(",", oids)} );";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return (await DbHelper.ExecuteSelectAsync<GetReleaseCategoryInfo>(true, cmd))?.ToList() ?? new List<GetReleaseCategoryInfo>();
            }
        }
        #endregion

    }
}
