using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Activity.DataAccess.Models;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalBargain
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalBargain));

        public static async Task<IEnumerable<BargainProductModel>> GetAllBargainProduct()
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4GetAllBargainProduct))
            {
                return await DbHelper.ExecuteSelectAsync<BargainProductModel>(true, cmd);
            }
        }

        public static async Task<IEnumerable<BargainProductModel>> GetBargainProductByUser(Guid userId)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4GetBargainProductByUser))
            {
                cmd.Parameters.AddWithValue("@userid", userId);
                return await DbHelper.ExecuteSelectAsync<BargainProductModel>(true, cmd);
            }
        }

        public static async Task<List<BargainHistory>> FetchBargainProductHistory(Guid userId, int apId, string pid,
            bool readOnly)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4FetchBargainHistory))
            {
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@apId", apId);
                cmd.Parameters.AddWithValue("@pid", pid);
                return (await DbHelper.ExecuteSelectAsync<BargainHistory>(readOnly, cmd)).ToList();
            }
        }

        public static async Task<CurrentBargainData> FetchCurrentBargainData(Guid idKey, bool readOnly)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4FetchCurrentBargainData))
            {
                cmd.Parameters.AddWithValue("@idkey", idKey);
                return await DbHelper.ExecuteFetchAsync<CurrentBargainData>(readOnly, cmd);
            }
        }

        public static async Task<List<Tuple<Guid, decimal>>> CheckBargainShare(Guid userId, Guid idkey, bool readOnly)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4checkBargainShare))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@idkey", idkey);
                return await DbHelper.ExecuteQueryAsync(readOnly, cmd, dt =>
                {
                    var result = new List<Tuple<Guid, decimal>>();
                    if (dt == null || dt.Rows.Count < 1) return result;
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var idKey = dt.Rows[i].GetValue<Guid>("idKey");
                        var reduce = dt.Rows[i].GetValue<decimal>("Reduce");
                        result.Add(Tuple.Create(idKey, reduce));
                    }

                    return result;
                });
            }
        }

        /// <summary>
        /// 获取针对一个活动产品，用户的所有砍价数据（砍价数据=帮砍数据+用户发起砍价时砍价数据）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static async Task<List<BargainShareTimes>> CheckBargainAllShare(Guid userId, string pid, DateTime beginDateTime, DateTime endDateTime)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4checkBargainAllShare))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@beginDateTime", beginDateTime);
                cmd.Parameters.AddWithValue("@endDateTime", endDateTime);
                return (await DbHelper.ExecuteSelectAsync<BargainShareTimes>(true, cmd)).ToList();
            }
        }

        public static async Task<int> CheckUserBargainCount(Guid userId, string todayDate, bool readOnly)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4CheckUserBargainedCount))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@todayDate", todayDate);
                var dat = await DbHelper.ExecuteScalarAsync(readOnly, cmd);
                int result = 0;
                if (Int32.TryParse(dat?.ToString(), out result))
                {
                    return result;
                }
                else
                {
                    Logger.Warn($"{dat}转换失败");
                    return 0;
                }
            }
        }

        public static async Task<bool> AddBargainAction(Guid userid, int apId, int parentId, decimal reduce,
            bool IsOver, int step, decimal totalreduce)
        {
            using (var db = DbHelper.CreateDbHelper())
            {
                var val = 0;
                using (var cmd = new SqlCommand(DalBargainSqlText.sql4insertBargainShareIdAction))
                {
                    cmd.Parameters.AddWithValue("@apId", apId);
                    cmd.Parameters.AddWithValue("@parentId", parentId);
                    cmd.Parameters.AddWithValue("@userId", userid);
                    cmd.Parameters.AddWithValue("@step", step);
                    cmd.Parameters.AddWithValue("@reduce", reduce);
                    val = await db.ExecuteNonQueryAsync(cmd);
                }

                if (val > 0)
                {
                    using (var cmd = new SqlCommand(DalBargainSqlText.sql4updateBargainOwnerData))
                    {
                        cmd.Parameters.AddWithValue("@isover", IsOver);
                        //cmd.Parameters.AddWithValue("@currentcount", step);
                        cmd.Parameters.AddWithValue("@totalreduce", totalreduce);
                        cmd.Parameters.AddWithValue("@pkid", parentId);
                        var tmp = (await db.ExecuteNonQueryAsync(cmd)) > 0;
                        if (!IsOver)
                        {
                            return tmp;
                        }
                    }

                    if (IsOver)
                    {
                        using (var cmd = new SqlCommand(DalBargainSqlText.sql4updateBargainConfigure))
                        {
                            cmd.Parameters.AddWithValue("@apId", apId);
                            return (await db.ExecuteNonQueryAsync(cmd)) > 0;
                        }
                    }
                }

                return false;
            }
        }

        public static async Task<BargainShareResult> CheckBargainProductStatus(Guid ownerId, int apId, string pid)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4checkBargainProductStatus))
            {
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@apId", apId);
                cmd.Parameters.AddWithValue("@ownerId", ownerId);
                return await DbHelper.ExecuteFetchAsync<BargainShareResult>(false, cmd);
            }
        }

        public static async Task<bool> CheckBargainProductStatusByPID(Guid ownerId, string pid)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4checkBargainProductStatusByPid))
            {
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@ownerId", ownerId);
                var dat = await DbHelper.ExecuteScalarAsync(false, cmd);
                int result = 0;
                if (Int32.TryParse(dat?.ToString(), out result))
                {
                    return result > 0;
                }
                else
                {
                    Logger.Warn($"{dat}转换失败");
                    return false;
                }
            }
        }

        public static async Task<int> CheckBargainProduct(int apId, string pid, bool readOnly)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4checekBargainShareStatus))
            {
                cmd.Parameters.AddWithValue("@apId", apId);
                cmd.Parameters.AddWithValue("@pid", pid);
                var dat = await DbHelper.ExecuteScalarAsync(readOnly, cmd);
                int result = 0;
                if (Int32.TryParse(dat?.ToString(), out result))
                {
                    return result;
                }
                else
                {
                    Logger.Warn($"{dat}转换失败");
                    return 0;
                }

            }
        }

        public static async Task<BargainShareResult> CheckUserShareBargain(Guid ownerId, int apId, bool readOnly)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4CheckUserShareBargain))
            {
                cmd.Parameters.AddWithValue("@apid", apId);
                cmd.Parameters.AddWithValue("@ownerid", ownerId);
                return await DbHelper.ExecuteFetchAsync<BargainShareResult>(readOnly, cmd);
            }
        }

        public static async Task<int> CheckUserBargainOwnerActionCount(Guid ownerId, DateTime beginTime,
            DateTime endTime, bool readOnly)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4CheckUserBargainOwnerActionCount))
            {
                cmd.Parameters.AddWithValue("@ownerid", ownerId);
                cmd.Parameters.AddWithValue("@beginTime", beginTime);
                cmd.Parameters.AddWithValue("@endTime", endTime);
                var dat = await DbHelper.ExecuteScalarAsync(readOnly, cmd);
                int result = 0;
                if (Int32.TryParse(dat?.ToString(), out result))
                {
                    return result;
                }
                else
                {
                    Logger.Warn($"{dat}转换失败");
                    return 0;
                }
            }
        }
        /// <summary>
        ///     增加一条用户砍价数据
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="apId"></param>
        /// <param name="pid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static async Task<BargainShareResult> AddShareBargain(Guid ownerId, int apId, string pid,
            bool status = false)
        {
            var idKey = Guid.NewGuid();
            var result = new BargainShareResult();
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4insertShareBargain))
            {
                cmd.Parameters.AddWithValue("@apId", apId);
                cmd.Parameters.AddWithValue("@ownerId", ownerId);
                cmd.Parameters.AddWithValue("@idkey", idKey);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@status", status);
                result.Code = await DbHelper.ExecuteNonQueryAsync(cmd);
                if (result.Code == 1)
                {
                    result.IdKey = idKey;
                    result.Info = "分享成功~";
                }
            }

            return result;
        }

        public static async Task<BargainShareProductModel> FetchShareBargainInfo(Guid idKey)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4fetchShareBargainInfo))
            {
                cmd.Parameters.AddWithValue("@idkey", idKey);
                return await DbHelper.ExecuteFetchAsync<BargainShareProductModel>(true, cmd);
            }
        }

        public static async Task<int> BuyBargainProductAsync(Guid ownerId, string pid, int OrderId)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4updateBargainOwnerAction))
            {
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@owner", ownerId);
                cmd.Parameters.AddWithValue("@OrderId", OrderId);
                var value = await DbHelper.ExecuteScalarAsync(false, cmd);
                int.TryParse(value?.ToString(), out var result);
                return result;
            }
        }

        public static async Task<BargainrulesModel> GetBackgroundStyle()
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4fetchbackgroundstyle))
            {
                return await DbHelper.ExecuteFetchAsync<BargainrulesModel>(true, cmd);
            }
        }

        public static async Task<ActivityPriceModel> FetchBargainPrice(string pid, Guid userId)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4fetchBargainPrice))
            {
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@ownerid", userId);
                var item = await DbHelper.ExecuteScalarAsync(true, cmd);
                if (item == null || item is DBNull)
                {
                    return new ActivityPriceModel()
                    {
                        Code = 0,
                    };
                }
                else
                {
                    return new ActivityPriceModel()
                    {
                        Code = 1,
                        PID = pid,
                        ActivityPrice = Convert.ToDecimal(item),
                        ApplyCoupon = false
                    };
                }
            }
        }

        public static async Task<IEnumerable<ActivityPriceModel>> SelectProductActivityPrice(Guid ActivityId,
            List<string> pids)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4selectProductActivityPrice))
            {
                cmd.Parameters.AddWithValue("@pids", string.Join(",", pids));
                cmd.Parameters.AddWithValue("@activityId", ActivityId);
                return await DbHelper.ExecuteSelectAsync<ActivityPriceModel>(true, cmd);
            }
        }

        public static async Task<BargainProductInfo> FetchBargainProductItemByIdKey(Guid idKey, bool readOnly = true)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4FetchBargainProductItemByIdKey))
            {
                cmd.Parameters.AddWithValue("@idkey", idKey);
                return await DbHelper.ExecuteFetchAsync<BargainProductInfo>(readOnly, cmd);
            }
        }

        public static async Task<IEnumerable<BargainProductModel>> SelectBargainProductItems()
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4SelectBargainProductItems))
            {
                return await DbHelper.ExecuteSelectAsync<BargainProductModel>(false, cmd);
            }
        }

        public static async Task<IEnumerable<BargainProductModel>> SelectBargainProductItemsByUserId(Guid ownerId,
            bool readOnly)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4SelectBargainProductItemsByUserId))
            {
                cmd.Parameters.AddWithValue("@ownerid", ownerId);
                return await DbHelper.ExecuteSelectAsync<BargainProductModel>(readOnly, cmd);
            }
        }

        public static async Task<List<BargainProductModel>> SelectBargainProductById(Guid? userId,
            List<BargainProductItem> ProductItems, bool readOnly)
        {
            string sql = @"select distinct
       BP.PKID as ActivityProductId,
       BP.PID,
       BP.productName as ProductName,
       BP.PageName,
       BP.SuccessfulHint,
       BP.OriginalPrice,
       BP.FinalPrice,
       BP.Image1,
       BP.Times,
       BP.WXShareTitle,
       BP.APPShareId,
       BP.Sequence,
       BP.BeginDateTime,
       BP.EndDateTime,
       ISNULL(BP.ShowBeginTime,BP.BeginDateTime) ShowBeginTime,
       BP.ProductType, 
       case
           when BP.CurrentStockCount < 1 then
               0
           else
               1
       end as HasStock,
       ISNULL(BOA.IsOver, 0) as IsOver,
       ISNULL(BOA.IsPurchased, 0) as IsPurchased,
       case
           when BOA.PKID is null then
               0
           else
               1
       end as IsShare,
       case
           when ISNULL(BOA.CurrentCount, 0) > 0 then
               1
           else
               0
       end as HasBargainHistory,
       case
           when @ownerid is null then
               0
           else
               1
       end as IsOwner,
       BOA.PKID,
       BP.ProductType,
       BP.CurrentStockCount,
       ISNULL(BP.ShowBeginTime, BP.BeginDateTime) as ShowBeginTime
from Configuration..BargainProduct as BP with (nolock)
    left join Activity..BargainOwnerAction as BOA with (nolock)
        on BOA.ActivityProductId = BP.PKID
           and (   (   @ownerid is null
                       and BOA.PKID = -1)
                   or @ownerid = BOA.OwnerId)
WHERE BP.IsDelete=0
AND  BP.PKID IN ( " + string.Join(",", ProductItems.Select(g => g.ActivityProductId).ToList()) + @" )";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ownerid", userId);
                return (await DbHelper.ExecuteSelectAsync<BargainProductModel>(readOnly, cmd)).ToList();
            }
        }

        public static async Task<bool> CheckUserBargained(Guid ownerid, Guid userId, BargainProductItem ProductItem)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4CheckUserBargained))
            {
                cmd.Parameters.AddWithValue("@apid", ProductItem.ActivityProductId);
                cmd.Parameters.AddWithValue("@ownerid", ownerid);
                cmd.Parameters.AddWithValue("@userid", userId);
                var dat = await DbHelper.ExecuteScalarAsync(true, cmd);
                int result = 0;
                if (Int32.TryParse(dat?.ToString(), out result))
                {
                    return result > 0;
                }
                else
                {
                    Logger.Warn($"{dat}转换失败");
                    return false;
                }
            }
        }

        public static async Task<bool> SetShareBargainStatus(Guid IdKey)
        {
            string sqlStr = @"UPDATE Activity..BargainOwnerAction WITH(ROWLOCK) SET Status=1 WHERE idKey=@idkey;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@idkey", IdKey);
                return (await DbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        public static async Task<bool> MarkUserReceiveCoupon(Guid ownerId, int apId)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4MarkUserReceiveCoupon))
            {
                cmd.Parameters.AddWithValue("@ownerId", ownerId);
                cmd.Parameters.AddWithValue("@ActivityProductId", apId);
                var res = await DbHelper.ExecuteNonQueryAsync(cmd);
                return res > 0;
            }
        }

        public static async Task<BargainProductModel> SelectProductInfo(int PKID)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4SelectBargainProductInfo))
            {
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return await DbHelper.ExecuteFetchAsync<BargainProductModel>(true, cmd);
            }
        }

        //获取用户一段时间内砍价的次数
        public static async Task<int> GetUserBargainCountAtTimerange(Guid ownerId, DateTime beginTime, DateTime endTime,
            bool readOnly)
        {
            using (var cmd = new SqlCommand(DalBargainSqlText.sql4SelectUserBargainCountAtTimeRange))
            {
                cmd.Parameters.AddWithValue("@UserId", ownerId);
                cmd.Parameters.AddWithValue("@beginTime", beginTime);
                cmd.Parameters.AddWithValue("@endTime", endTime);
                var dat = await DbHelper.ExecuteScalarAsync(readOnly, cmd);
                int result = 0;
                if (Int32.TryParse(dat?.ToString(), out result))
                {
                    return result;
                }
                else
                {
                    Logger.Warn($"{dat}转换失败");
                    return 0;
                }
            }
        }

        public static async Task<List<SimpleBargainProduct>> GetBargainProductForIndex()
        {
            const string sqlStr = @"
select top 100
       PKID as ActivityProductId,
       OriginalPrice as OriginPrice,
       FinalPrice as ActivityPrice,
       PID as Pid
from Configuration..BargainProduct with (nolock)
where IsDelete=0
      AND BeginDateTime < GETDATE()
      and EndDateTime > DATEADD(hour, 1, GETDATE())
      and CurrentStockCount > 0
      and ProductType = 1
order by Sequence desc ,pkid desc;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return (await DbHelper.ExecuteSelectAsync<SimpleBargainProduct>(true, cmd))?.ToList() ??
                       new List<SimpleBargainProduct>();
            }
        }

        public static async Task<List<Tuple<int, int>>> GetCurrentBargainCount(List<int> apIds)
        {
            var sqlStr = @"select bsa.ActivityProductId,
                   COUNT(distinct bsa.UserId) as count
            from Activity..BargainShareAction as bsa with (nolock)
            inner join  ( SELECT    Item
                      FROM      Activity..SplitString(@apIds, ',', 1)
                    ) AS T on T.Item = bsa.ActivityProductId
            group by bsa.ActivityProductId;";
            //var sqlStr = $@"select ActivityProductId,
            //               COUNT(distinct T.UserId) as count
            //        from Activity..BargainShareAction as T with (nolock)
            //        where T.ActivityProductId in ({string.Join(",", apIds)} )
            //        group by T.ActivityProductId;";

            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.Add(new SqlParameter("@apIds", string.Join(",", apIds)));
                //cmd.Parameters.AddWithValue("@apIds", apIds);
                return await DbHelper.ExecuteQueryAsync(false, cmd, dt =>
                {
                    var result = new List<Tuple<int, int>>();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            var apid = dt.Rows[i].GetValue<int>("ActivityProductId");
                            var count = dt.Rows[i].GetValue<int>("count");
                            if (apid > 0)
                            {
                                result.Add(Tuple.Create(apid, count));
                            }
                        }
                    }

                    return result;
                });
            }
        }

        public static async Task<PagedModel<BargainProductItem>> SelectBargainProductList(int pageIndex, int pageSize)
        {
            const string sqlStr = @"
select COUNT(*)
from Configuration..BargainProduct with (nolock)
where IsDelete=0
      AND ISNULL(ShowBeginTime, BeginDateTime) < GETDATE()
      and CurrentStockCount > 0
      and EndDateTime > GETDATE();";
            const string sqlStr2 = @"
select PKID as ActivityProductId,
       PID
from Configuration..BargainProduct with (nolock)
where IsDelete=0
      AND ISNULL(ShowBeginTime, BeginDateTime) < GETDATE()
      and CurrentStockCount > 0
      and EndDateTime > GETDATE()
order by Sequence desc ,pkid desc offset @start rows fetch next @step rows only;";
            var result = new PagedModel<BargainProductItem>()
            {
                Pager = new PagerModel()
                {
                    PageSize = pageSize,
                    CurrentPage = pageIndex
                }
            };
            using (var cmd = new SqlCommand(sqlStr))
            {
                var value = await DbHelper.ExecuteScalarAsync(true, cmd);
                int.TryParse(value?.ToString(), out var count);
                if (count > 0)
                {
                    result.Pager.Total = count;
                    cmd.CommandText = sqlStr2;
                    cmd.Parameters.AddWithValue("@start", (pageIndex - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@step", pageSize);
                    var data = (await DbHelper.ExecuteSelectAsync<BargainProductItem>(true, cmd))?.ToList() ??
                               new List<BargainProductItem>();
                    result.Source = data;
                }

                return result;
            }
        }

        public static async Task<PagedModel<BargainHistoryModel>> SelectBargainHistory(int pageIndex, int pageSize,
            Guid userId)
        {
            const string sqlStr = @"
select COUNT(*)
from Activity..BargainOwnerAction as T with (nolock)
    inner join Configuration..BargainProduct as S with (nolock)
        on T.ActivityProductId = S.PKID AND S.IsDelete=0
where T.OwnerId = @userId
      and T.CreateDateTime > '2018-05-25';";
            const string sqlStr2 = @"
select T.ActivityProductId,
       IIF(T.IsOver = 1, 1, 0) as BargainStatus,
       T.CreateDateTime as StartTime,
       T.TotalReduce as Reduce,
       T.IsPurchased as IsFinish,
       T.PID as Pid,
       T.idKey,
       T.CurrentCount,
       S.HelpCutPriceTimes,
       S.CutPricePersonLimit
from Activity..BargainOwnerAction as T with (nolock)
    inner join Configuration..BargainProduct as S with (nolock)
        on T.ActivityProductId = S.PKID AND S.IsDelete=0
where T.OwnerId = @userId
      and T.Status = 1
      and T.CreateDateTime > '2018-05-25'
order by T.CreateDateTime desc offset @start rows fetch next @step rows only;";
            var result = new PagedModel<BargainHistoryModel>
            {
                Pager = new PagerModel
                {
                    PageSize = pageSize,
                    CurrentPage = pageIndex
                },
                Source = new List<BargainHistoryModel>()
            };
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                var value = await DbHelper.ExecuteScalarAsync(true, cmd);
                int.TryParse(value?.ToString(), out var count);
                if (count > 0)
                {
                    cmd.CommandText = sqlStr2;
                    result.Pager.Total = count;
                    cmd.Parameters.AddWithValue("@start", (pageIndex - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@step", pageSize);
                    var data = (await DbHelper.ExecuteSelectAsync<BargainHistoryModel>(true, cmd))?.ToList() ??
                               new List<BargainHistoryModel>();
                    result.Source = data;
                }
            }

            return result;
        }

        public static async Task<List<SliceShowInfoModel>> GetSliceShowInfo(int count)
        {
            const string sqlStr = @"
select top (@count)
       T.OwnerId as UserId,
       T.LastUpdateDateTime as FinishTime,
       S.productName,
       S.SimpleDisplayName
from Activity..BargainOwnerAction as T with (nolock)
    left join Configuration..BargainProduct as S with (nolock)
        on T.ActivityProductId = S.PKID AND S.IsDelete=0
where T.IsOver = 1
      and T.Status = 1
      and S.CreateDateTime > GETDATE() - 7
order by T.LastUpdateDateTime desc;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@count", count);
                return (await DbHelper.ExecuteSelectAsync<SliceShowInfoModel>(true, cmd))?.ToList() ??
                       new List<SliceShowInfoModel>();
            }
        }

        public static async Task<List<BargainProductNewModel>> SelectBargainProductInfo(List<string> ids)
        {
            var sqlStr = $@"
select T.PKID as ActivityProductId,
       PID as Pid,
       T.OriginalPrice as OriginPrice,
       T.FinalPrice as ActivityPrice,
       T.Image1 as ImageUrl,
       T.productName as ProductName,
       T.BeginDateTime as BeginTime,
       T.EndDateTime as EndTime,
       T.ProductType,
       T.Times as RequiredTimes,
       T.CurrentStockCount,
       DATEADD(hour, 24, T.EndDateTime) as FinalTime,
       T.SuccessfulHint,
       T.WXShareTitle,
       T.AppShareID,
       T.SimpleDisplayName,
       T.HelpCutPriceTimes,
       T.CutPricePersonLimit
from Configuration..BargainProduct as T with (nolock)
where  T.IsDelete=0
       AND T.PKID in ( {string.Join(",", ids)} );";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return (await DbHelper.ExecuteSelectAsync<BargainProductNewModel>(false, cmd))?.ToList() ??
                       new List<BargainProductNewModel>();
            }
        }

        public static async Task<List<Tuple<Guid, string, decimal, decimal>>> CheckUserBargainRecord(Guid userId,
            string pid, int apId)
        {
            const string sqlStr = @"
select T.idKey,
       S.SimpleDisplayName,
       S.OriginalPrice - S.FinalPrice - T.TotalReduce as remnantMoney,
       P.Reduce as reduce
from Configuration..BargainProduct as S with (nolock)
    inner join Activity..BargainOwnerAction as T with (nolock)
        on T.ActivityProductId = S.PKID 
    left join Activity..BargainShareAction as P with (nolock)
        on P.ParentId = T.PKID
where S.PKID = @apId
      AND S.IsDelete=0
      and T.OwnerId = @userId
      and T.PID = @pid
      and T.Status = 1
      and P.UserId = @userId";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@apId", apId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@pid", pid);
                return await DbHelper.ExecuteQueryAsync(false, cmd, dt =>
                {
                    var result = new List<Tuple<Guid, string, decimal, decimal>>();
                    if (dt == null || dt.Rows.Count < 1) return result;
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var idkey = dt.Rows[i].GetValue<Guid>("idKey");
                        var simple = dt.Rows[i].GetValue<string>("SimpleDisplayName");
                        var remnantMoney = dt.Rows[i].GetValue<decimal>("remnantMoney");
                        var reduce = dt.Rows[i].GetValue<decimal>("reduce");
                        result.Add(Tuple.Create(idkey, simple, remnantMoney, reduce));
                    }

                    return result;
                });
            }
        }

        public static async Task<List<BargainProductNewModel>> GetBargainUserInfo(Guid userId, List<int> apIds,
            bool readOnly = true)
        {
            string sqlStr = $@"
select T.ActivityProductId,
       IIF(T.IsOver = 1, 1, 0) as BargainStatus,
       T.CreateDateTime as StartTime,
       T.TotalReduce as Reduce,
       T.IsPurchased as IsFinish,
       T.PID as Pid,
       T.idKey
from Activity..BargainOwnerAction as T with (nolock)
where T.OwnerId = @userId and T.Status=1 and T.ActivityProductId in ({string.Join(",", apIds)})
      and Status = 1";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                return (await DbHelper.ExecuteSelectAsync<BargainProductNewModel>(readOnly, cmd))?.ToList() ??
                       new List<BargainProductNewModel>();
            }
        }

        public static async Task<List<BargainProductNewModel>> GetUserBargainCount(Guid userId, List<int> apIds,
            bool readOnly = true)
        {
            string sqlStr = $@"select S.ActivityProductId,
       case
           when COUNT(*) > 1 then
               1
           else
               0
       end as IsShared
from Activity..BargainShareAction as T with (nolock)
    inner join Activity..BargainOwnerAction as S with (nolock)
        on T.ParentId = S.PKID
where S.OwnerId = @userId and S.OwnerId=T.UserId
      and S.Status = 1
      and S.ActivityProductId in ( {string.Join(",", apIds)} )
group by S.ActivityProductId";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                return (await DbHelper.ExecuteSelectAsync<BargainProductNewModel>(readOnly, cmd))?.ToList() ??
                       new List<BargainProductNewModel>();
            }
        }

        public static async Task<bool> CheckInviteeBargainResult(Guid idKey, Guid userId, bool readOnly = true)
        {
            const string sqlStr = @"
select COUNT(*)
from Activity..BargainShareAction as T with (nolock)
    inner join Activity..BargainOwnerAction as S with (nolock)
        on T.ParentId = S.PKID
where UserId = @userId
      and S.idKey = @idkey;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@idKey", idKey);
                var value = await DbHelper.ExecuteScalarAsync(readOnly, cmd);
                int.TryParse(value?.ToString(), out var result);
                return result > 0;
            }
        }

        public static async Task<Guid> GetInviteeIdKey(int apId, Guid userId, bool readOnly = true)
        {
            const string sqlStr = @"
select T.idKey
from Activity..BargainOwnerAction as T with (nolock)
where T.OwnerId = @userId
      and T.ActivityProductId = @apId";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@apId", apId);
                var value = await DbHelper.ExecuteScalarAsync(readOnly, cmd);
                Guid.TryParse(value?.ToString(), out var result);
                return result;
            }
        }

        public static async Task<List<BargainProductItem>> GetAllBargainProductItem()
        {
            const string sqlStr = @"
select PKID as ActivityProductId,
       PID
from Configuration..BargainProduct with (nolock)
where IsDelete=0;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return (await DbHelper.ExecuteSelectAsync<BargainProductItem>(true, cmd))?.ToList() ??
                       new List<BargainProductItem>();
            }
        }


        /// <summary>
        /// 获取未完成的 发起砍价记录
        /// </summary>
        /// <param name="apIds"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public static async Task<List<CurrentBargainData>> GetValidBargainOwnerActionsByApidsAsync(int apId, DateTime startDate, DateTime endDate, int status, int IsOver, bool readOnly = true)
        {
            string sqlStr = @"
                            select
                                   T.PKID,
                                   T.idKey,
                                   T.ActivityProductId,
                                   T.OwnerId,
                                   T.IsOver,
                                   T.PID
                            from Activity..BargainOwnerAction as T with (nolock)
                            where T.ActivityProductId = @apId and  T.CreateDateTime	>=@startDate and  T.CreateDateTime	<=@endDate and  T.status=@status and IsOver = @IsOver
                            ";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@apId", apId);
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@IsOver", IsOver);
                return (await DbHelper.ExecuteSelectAsync<CurrentBargainData>(readOnly, cmd))?.ToList() ??
                       new List<CurrentBargainData>();
            }
        }


        /// <summary>
        /// 获取砍价的配置  【时间】
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static async Task<List<BargainProductNewModel>> SelectBargainProductsByDateAsync(DateTime startDate, DateTime endDate)
        {
            var sql = $@"   select T.PKID as ActivityProductId,
                                   PID as Pid,
                                   T.OriginalPrice as OriginPrice,
                                   T.FinalPrice as ActivityPrice,
                                   T.Image1 as ImageUrl,
                                   T.productName as ProductName,
                                   T.BeginDateTime as BeginTime,
                                   T.EndDateTime as EndTime,
                                   T.ProductType,
                                   T.Times as RequiredTimes,
                                   T.CurrentStockCount,
                                   DATEADD(hour, 24, T.EndDateTime) as FinalTime,
                                   T.SuccessfulHint,
                                   T.WXShareTitle,
                                   T.AppShareID,
                                   T.SimpleDisplayName,
                                   T.HelpCutPriceTimes,
                                   T.CutPricePersonLimit
                            from Configuration..BargainProduct as T with (nolock)
                            WHERE  T.IsDelete=0
                                AND T.EndDateTime>=@startDate
                                AND T.EndDateTime<=@endDate ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);
                return (await DbHelper.ExecuteSelectAsync<BargainProductNewModel>(true, cmd))?.ToList() ??
                         new List<BargainProductNewModel>();
            }
        }

        #region 新方法

        /// <summary>
        /// 获取砍价商品配置
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static async Task<BargainProductSettingModel> GetBargainProductInfoAsync(int PKID, bool isReadOnly = true)
        {
            string sqlStr = @"  SELECT  [PKID],
                                        [PID],
                                        [productName],
                                        [OriginalPrice],
                                        [FinalPrice],
                                        [Sequence],
                                        [Image1],
                                        [WXShareTitle],
                                        [APPShareId],
                                        [Times],
                                        [BeginDateTime],
                                        [EndDateTime],
                                        [TotalStockCount],
                                        [CurrentStockCount],
                                        [Operator],
                                        [CreateDateTime],
                                        [LastUpdateDateTime],
                                        [PageName],
                                        [SuccessfulHint],
                                        [ShowBeginTime],
                                        [ProductType],
                                        [SimpleDisplayName],
                                        [HelpCutPriceTimes],
                                        [CutPricePersonLimit],
                                        [IsDelete],
                                        [BigCutBeforeCount],
                                        [BigCutPriceRate],
                                        [ProductDetailImg1],
                                        [ProductDetailImg2],
                                        [ProductDetailImg3],
                                        [ProductDetailImg4],
                                        [ProductDetailImg5]
                                    FROM [Configuration].[dbo].[BargainProduct] WITH (NOLOCK)
                                    WHERE IsDelete = 0
                                          AND PKID = @PKID;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return await DbHelper.ExecuteFetchAsync<BargainProductSettingModel>(isReadOnly, cmd);
            }
        }

        /// <summary>
        /// 获取砍价商品参与人数:帮砍用户包含发起人 去重
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static async Task<int> GetBargainProductParticipantCountAsync(int PKID, bool isReadOnly = true)
        {
            string sqlStr = @"  SELECT COUNT(DISTINCT UserId)
                                FROM Activity..BargainShareAction WITH (NOLOCK)
                                WHERE ActivityProductId = @PKID";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@PKID", PKID);
                int.TryParse((await DbHelper.ExecuteScalarAsync(isReadOnly, cmd)).ToString(), out int result);
                return result;
            }
        }

        #endregion

    }
}

