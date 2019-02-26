using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Service.Activity.Const;
using Tuhu.Service.Activity.DataAccess.Models;
using Tuhu.Service.Activity.DataAccess.Models.SalePromotionActivity;
using Tuhu.Service.Activity.DataAccess.Tools;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalDiscountActivityInfo
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalDiscountActivityInfo));

        #region 查询 

        /// <summary>
        /// 获取商品在时间段内的活动列表
        /// </summary>
        /// <param name="pidList"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ProductActivityInfoForTag>> GetProductDiscountInfoForTagAsync(List<string> pidList, DateTime startTime, DateTime endTime)
        {

            #region sql
            //获取商品信息sql
            string sqlProduct = @" select  p.Pid,p.ImageUrl,p.DetailImageUrl,p.LimitQuantity,
                                           a.ActivityId,a.Description,a.Banner,
                                           a.Is_DefaultLabel as IsDefaultLabel,a.Label,
                                           a.Is_PurchaseLimit as IsUserPurchaseLimit,
                                           a.LimitQuantity as UserLimitNum,a.StartTime,a.EndTime
                                    from   [Activity].[dbo].[SalePromotionActivityProduct] p with (nolock)
                                           Join Activity..SplitString(@pidList, ',', 1) b on p.Pid=b.Item
                                           join [Activity].[dbo].[SalePromotionActivity] a with (nolock) on p.ActivityId=a.ActivityId
                                   where   p.Is_Deleted=0 and p.Is_UnShelve=0
                                            and ( a.StartTime >= @StartTime
                                                          AND a.EndTime <= @EndTime
                                                          OR a.StartTime < @StartTime
                                                          AND a.EndTime > @StartTime
                                                          OR a.StartTime < @EndTime
                                                          AND a.EndTime > @EndTime
                                                        )
                                            and a.Is_UnShelveAuto=0
                                            and a.Is_Deleted=0 and a.AuditStatus=2 and a.Is_UnShelve=0";

            //获取打折规则
            string sqlDiscount = @" select  d.ActivityId,d.Condition,d.DiscountRate,d.DiscountMethod
                                     from   [Activity].[dbo].[SalePromotionActivityDiscount] d with (nolock)
                                     Join   Activity..SplitString(@activityIdList, ',', 1) b
                                            on d.ActivityId=b.Item
                                    where   d.Is_Deleted=0";
            #endregion

            var activityList = new List<DiscountActivityForTag>();
            var ruleList = new List<DiscountActivityRule>();
            var pidActivityList = new List<ProductActivityInfoForTag>();

            try
            {
                using (var cmd = new SqlCommand(sqlProduct))
                {
                    //1.获取活动信息和商品信息
                    cmd.Parameters.Add(new SqlParameter("@pidList", string.Join(",", pidList)));
                    cmd.Parameters.Add(new SqlParameter("@StartTime", startTime));
                    cmd.Parameters.Add(new SqlParameter("@EndTime", endTime));

                    var productReadOnlyFlag = await DBReadOnlyFlagHelper.GetDBReadOnlyFlagFromCache(SalePromotionActivityConst.SalePromotionActivityAndProduct, pidList);
                    Logger.Info($"GetProductDiscountInfoForTagAsync只读库标识：{productReadOnlyFlag}");

                    activityList = (await DbHelper.ExecuteSelectAsync<DiscountActivityForTag>(productReadOnlyFlag, cmd))?.ToList();
                    if (activityList?.Count > 0)
                    {
                        //取出活动id
                        var activityIdList = activityList.Where(a => !string.IsNullOrWhiteSpace(a.ActivityId))
                                                         .Select(a => a.ActivityId).Distinct();
                        //2.根据活动id 获取活动打折规则
                        cmd.CommandText = sqlDiscount;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@activityIdList", string.Join(",", activityIdList)));

                        var discountReadOnlyFlag = await DBReadOnlyFlagHelper.GetDBReadOnlyFlagFromCache(SalePromotionActivityConst.SalePromotionActivityDiscount, activityIdList?.ToList());
                        ruleList = (await DbHelper.ExecuteSelectAsync<DiscountActivityRule>(discountReadOnlyFlag, cmd))?.Distinct().ToList();
                    }
                }

                foreach (var item in activityList)
                {
                    //绑定打折规则到活动
                    item.RuleList = ruleList.Where(r => r.ActivityId == item.ActivityId)?.ToList();
                    if (item.RuleList?[0]?.DiscountMethod == 1)
                    {
                        item.DiscountType = "FullAmount";
                    }
                    else
                    {
                        item.DiscountType = "FullCount";
                    }
                }
                foreach (var item in pidList)
                {
                    //绑定pid的活动列表
                    var model = new ProductActivityInfoForTag()
                    {
                        Pid = item,
                        DiscountActivityList = activityList.Where(a => a.Pid == item)?.ToList() ?? new List<DiscountActivityForTag>()
                    };
                    pidActivityList.Add(model);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetProductDiscountInfoForTagAsync异常：{ex}");
            }
            pidActivityList = pidActivityList.Where(a => a?.DiscountActivityList?.Count > 0)?.ToList();

            return pidActivityList;
        }

        /// <summary>
        /// 根据pids 活动商品当前有效的活动信息
        /// </summary>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public static async Task<List<ProductActivityInfoForTag>> GetActivityInfoByPids(List<string> pidList)
        {
            #region sql
            //获取商品信息sql
            string sqlProduct = @" select  p.Pid,p.ImageUrl,p.LimitQuantity,
                                           a.ActivityId,a.Description,a.Banner,
                                           a.Is_DefaultLabel as IsDefaultLabel,a.Label,
                                           a.Is_PurchaseLimit as IsUserPurchaseLimit,
                                           a.LimitQuantity as UserLimitNum,a.StartTime,a.EndTime
                                    from   [Activity].[dbo].[SalePromotionActivityProduct] p with (nolock)
                                           Join Activity..SplitString(@pidList, ',', 1) b on p.Pid=b.Item
                                           join [Activity].[dbo].[SalePromotionActivity] a  with (nolock) on p.ActivityId=a.ActivityId
                                   where   p.Is_Deleted=0 and p.Is_UnShelve=0
                                           and (a.StartTime<getdate() and a.EndTime>getdate())
                                           and a.Is_UnShelveAuto=0
                                           and a.Is_Deleted=0 and a.AuditStatus=2 and a.Is_UnShelve=0";
            string sqlDiscount = @" select  d.ActivityId,d.Condition,d.DiscountRate,d.DiscountMethod
                                     from   [Activity].[dbo].[SalePromotionActivityDiscount] d with (nolock)
                                     Join   Activity..SplitString(@activityIdList, ',', 1) b
                                            on d.ActivityId=b.Item
                                    where   d.Is_Deleted=0";
            #endregion
            var activityList = new List<DiscountActivityForTag>();
            var ruleList = new List<DiscountActivityRule>();
            var pidActivityList = new List<ProductActivityInfoForTag>();
            try
            {
                using (var cmd = new SqlCommand(sqlProduct))
                {
                    //1.获取活动信息和商品信息
                    cmd.Parameters.Add(new SqlParameter("@pidList", string.Join(",", pidList)));
                    var productReadOnlyFlag = await DBReadOnlyFlagHelper.GetDBReadOnlyFlagFromCache(SalePromotionActivityConst.SalePromotionActivityAndProduct, pidList);
                    Logger.Info($"GetActivityInfoByPids只读库标识：{productReadOnlyFlag}");
                    activityList = (await DbHelper.ExecuteSelectAsync<DiscountActivityForTag>(productReadOnlyFlag, cmd))?.ToList();
                    if (activityList?.Count > 0)
                    {
                        //取出活动id
                        var activityIdList = activityList.Where(a => !string.IsNullOrWhiteSpace(a.ActivityId))
                                                         .Select(a => a.ActivityId).Distinct();
                        //2.根据活动id 获取活动打折规则
                        cmd.CommandText = sqlDiscount;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@activityIdList", string.Join(",", activityIdList)));
                        var discountReadOnlyFlag = await DBReadOnlyFlagHelper.GetDBReadOnlyFlagFromCache(SalePromotionActivityConst.SalePromotionActivityDiscount, activityIdList.ToList());
                        Logger.Info($"活动打折规则只读库标识：{discountReadOnlyFlag}");
                        ruleList = (await DbHelper.ExecuteSelectAsync<DiscountActivityRule>(discountReadOnlyFlag, cmd))?.Distinct().ToList();

                        foreach (var item in activityList)
                        {
                            //绑定打折规则到活动
                            item.RuleList = ruleList.Where(r => r.ActivityId == item.ActivityId)?.ToList();
                            if (item.RuleList[0]?.DiscountMethod == 1)
                            {
                                item.DiscountType = "FullAmount";
                            }
                            else
                            {
                                item.DiscountType = "FullCount";
                            }
                        }
                        foreach (var item in pidList)
                        {
                            //绑定pid的活动列表
                            var model = new ProductActivityInfoForTag()
                            {
                                Pid = item
                            };
                            model.DiscountActivityList = activityList.Where(a => a.Pid == model.Pid)?.ToList();
                            if (model.DiscountActivityList == null)
                            {
                                model.DiscountActivityList = new List<DiscountActivityForTag>();
                            }
                            pidActivityList.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetActivityInfoByPids：{ex}");
            }
            return pidActivityList;
        }

        /// <summary>
        /// 获取用户的活动商品购买数
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<int> SumActivityUserBuyNumAsync(string userId, string activityId)
        {
            string sql = @" select  isnull(sum(o.num),0)
                             from   [Activity].[dbo].[SalePromotionDiscountOrder] o with(nolock)
                            where   o.UserId=@userId and ActivityId=@activityId and o.OrderStatus=1
                          group by  o.ActivityId ";
            int sum = 0;
            using (var cmd = new SqlCommand(sql))
            {
                try
                {
                    cmd.Parameters.Add(new SqlParameter("@activityId", activityId));
                    cmd.Parameters.Add(new SqlParameter("@userId", userId));
                    var keys = new List<string>() { activityId + "/" + userId };
                    var readOnlyFlag = await DBReadOnlyFlagHelper.GetDBReadOnlyFlagFromCache(SalePromotionActivityConst.SalePromotionDiscountOrder, keys);
                    Logger.Info($"SumActivityUserBuyNumAsync只读库标识：{readOnlyFlag}");
                    int.TryParse((await DbHelper.ExecuteScalarAsync(readOnlyFlag, cmd))?.ToString(), out sum);
                }
                catch (Exception ex)
                {
                    Logger.Error($"SumActivityUserBuyNumAsync获取用户的活动商品购买数异常,用户id：{userId}，活动id{activityId},ex:{ex}");
                }
            }
            return sum;
        }

        /// <summary>
        /// 计算某个活动下某个商品的已售打折数量
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static async Task<int> SumActivityProductSoldNumAsync(string activityId, string pid)
        {
            string sql = @" select  isnull(sum(Num),0) 
                             from   [Activity].[dbo].[SalePromotionDiscountOrder] with(nolock)
                             where  ActivityId=@ActivityId and Pid=@Pid and OrderStatus=1 ";
            int sum = 0;
            using (var cmd = new SqlCommand(sql))
            {
                try
                {
                    cmd.Parameters.Add(new SqlParameter("@ActivityId", activityId));
                    cmd.Parameters.Add(new SqlParameter("@Pid", pid));
                    var keys = new List<string>() { activityId + "/" + pid };
                    var readOnlyFlag = await DBReadOnlyFlagHelper.GetDBReadOnlyFlagFromCache(SalePromotionActivityConst.SalePromotionDiscountOrder, keys);
                    Logger.Info($"SumActivityProductSoldNumAsync单个计算，只读库标识：{readOnlyFlag}");

                    int.TryParse((await DbHelper.ExecuteScalarAsync(readOnlyFlag, cmd))?.ToString(), out sum);
                }
                catch (Exception ex)
                {
                    Logger.Error($"SumActivityProductSoldNumAsync计算某个活动下某个商品的已售打折数量异常,pid：{pid}，活动id{activityId},ex:{ex}");
                }
            }
            return sum;
        }

        /// <summary>
        /// 批量获取活动下用户的已购数量
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<UserActivityBuyNumModel>> SumActivityUserBuyNumAsync(string userId, List<string> activityIdList)
        {
            var dbList = new List<UserActivityBuyNumModel>();
            var resultList = new List<UserActivityBuyNumModel>();
            activityIdList = activityIdList.Distinct()?.ToList();
            string sql = @"select  o.ActivityId,UserId=@userId,sum(isnull(o.Num,0)) as BuyNum
                            from   [Activity].[dbo].[SalePromotionDiscountOrder] o with(nolock) 
                             Join   Activity..SplitString(@activityIdList, ',', 1) b
                                                    on o.ActivityId=b.Item
                            where  UserId=@userId and OrderStatus=1
                         group by  o.ActivityId";
            using (var cmd = new SqlCommand(sql))
            {
                try
                {
                    cmd.Parameters.Add(new SqlParameter("@activityIdList", string.Join(",", activityIdList)));
                    cmd.Parameters.Add(new SqlParameter("@userId", userId));
                    var keys = new List<string>();
                    foreach (var item in activityIdList)
                    {
                        keys.Add(item + "/" + userId);
                    }
                    var readOnlyFlag = await DBReadOnlyFlagHelper.GetDBReadOnlyFlagFromCache(SalePromotionActivityConst.SalePromotionDiscountOrder, keys);
                    Logger.Info($"SumActivityUserBuyNumAsync批量获取只读库标识：{readOnlyFlag}");

                    dbList = (await DbHelper.ExecuteSelectAsync<UserActivityBuyNumModel>(readOnlyFlag, cmd))?.ToList();
                }
                catch (Exception ex)
                {
                    Logger.Error($"SumActivityUserBuyNumAsync批量获取活动下用户的已购数量异常,用户id：{userId},ex:{ex}");
                }
            }
            resultList = activityIdList.Select(
                a =>
            dbList.Where(d => d.ActivityId == a).FirstOrDefault() ?? new UserActivityBuyNumModel() { ActivityId = a, UserId = userId }
            )?.ToList();
            return resultList;
        }

        /// <summary>
        /// 批量获取活动商品的已售数量
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ActivityPidSoldNumModel>> SumActivityProductSoldNumAsync(List<ActivityPidSoldNumModel> list)
        {
            var newList = new List<ActivityPidSoldNumModel>();
            var resultList = new List<ActivityPidSoldNumModel>();
            string sql = @" select  o.ActivityId,o.Pid ,sum(isnull(o.Num,0)) as SoldNum  
                             from   [Activity].[dbo].[SalePromotionDiscountOrder] o with(nolock)
                               Join  Activity..SplitString(@activityIdList, ',', 1) b
                                    on o.ActivityId=b.Item
                               Join  Activity..SplitString(@pidList, ',', 1) c
                                    on o.Pid=c.Item
                             where  o.OrderStatus=1 
                            group by o.ActivityId,o.Pid";
            using (var cmd = new SqlCommand(sql))
            {
                try
                {
                    var activityIdList = list.Select(a => a.ActivityId).Distinct();
                    var pidList = list.Select(a => a.Pid).Distinct();
                    cmd.Parameters.Add(new SqlParameter("@activityIdList", string.Join(",", activityIdList)));
                    cmd.Parameters.Add(new SqlParameter("@pidList", string.Join(",", pidList)));

                    var keys = new List<string>();
                    foreach (var item in list)
                    {
                        keys.Add(item.ActivityId + "/" + item.Pid);
                    }
                    var readOnlyFlag = await DBReadOnlyFlagHelper.GetDBReadOnlyFlagFromCache(SalePromotionActivityConst.SalePromotionDiscountOrder, keys);
                    Logger.Info($"SumActivityProductSoldNumAsync批量获取活动商品的已售数量只读库标识：{readOnlyFlag}");

                    newList = (await DbHelper.ExecuteSelectAsync<ActivityPidSoldNumModel>(readOnlyFlag, cmd)).ToList();
                }
                catch (Exception ex)
                {
                    Logger.Error($"SumActivityProductSoldNumAsync批量获取活动商品的已售数量异常,ex:{ex}");
                }
            }
            resultList = list.Select(a => newList.Where(d => d.ActivityId == a.ActivityId && d.Pid == a.Pid).FirstOrDefault() ?? a)?.ToList();
            return resultList;
        }

        #endregion

        #region 订单打折信息记录

        /// <summary>
        /// 创建订单时记录打折活动信息
        /// </summary>
        /// <param name="requestList"></param>
        /// <returns></returns>
        public static async Task<bool> SaveCreateOrderDiscountInfo(List<SalePromotionActivityDiscountOrder> orderInfoList)
        {
            bool result = true;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = new SqlCommand("", conn, tran))
                    {
                        try
                        {
                            var orderInfoTmp = orderInfoList.Select(item => new
                            {
                                UserId = item.UserId,
                                OrderId = item.OrderId,
                                Pid = item.Pid,
                                Num = item.Num,
                                ActivityId = item.ActivityId
                            });
                            DataTable orderInfoDT = ConvertDataTable.ToDataTable(orderInfoTmp);
                            cmd.CommandText = @"CREATE TABLE #orderInfoTmp([UserId] [nvarchar](100) NOT NULL,
	                                                                        [OrderId] [int] NULL,
	                                                                        [Pid] [nvarchar](100) NOT NULL,
	                                                                        [Num] [int] NOT NULL,
	                                                                        [ActivityId] [nvarchar](100) NOT NULL);";
                            await cmd.ExecuteNonQueryAsync();
                            using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, tran))
                            {
                                bulkcopy.BulkCopyTimeout = 660;
                                bulkcopy.DestinationTableName = "#orderInfoTmp";
                                bulkcopy.WriteToServer(orderInfoDT);
                                bulkcopy.Close();
                            }
                            //若记录已存在,覆盖；不存在则插入
                            string sql = @"MERGE INTO [Activity].[dbo].[SalePromotionDiscountOrder] WITH ( ROWLOCK ) AS do
                                            USING #orderInfoTmp AS temp
                                            ON do.OrderId = temp.OrderId and do.UserId=temp.UserId
                                               and do.Pid=temp.Pid and do.OrderStatus=1
                                            WHEN MATCHED THEN
                                                UPDATE SET do.Num=temp.Num,
                                                           do.ActivityId=temp.ActivityId,
                                                           do.LastUpdateDateTime=getdate()
                                            WHEN NOT MATCHED THEN
                                                INSERT ([UserId],[OrderId],[Pid],[Num],
                                                        [ActivityId],[OrderStatus],[CreateDateTime])
                                                VALUES (temp.UserId,temp.OrderId,temp.Pid,temp.Num,
                                                        temp.ActivityId,1,getdate());";
                            cmd.CommandText = sql;
                            if ((await cmd.ExecuteNonQueryAsync()) != orderInfoList.Count)
                            {
                                result = false;
                            }
                            if (result)
                            {
                                tran.Commit();
                                var keys = new List<string>();
                                foreach (var item in orderInfoList)
                                {
                                    keys.Add(item.ActivityId + "/" + item.Pid);
                                    keys.Add(item.ActivityId + "/" + item.UserId);
                                    keys.Add(item.OrderId.ToString());
                                }
                                DBReadOnlyFlagHelper.SetDBReadOnlyFlagCache(SalePromotionActivityConst.SalePromotionDiscountOrder, keys.ToList());
                            }
                            else
                            {
                                tran.Rollback();
                                Logger.Error($"SaveCreateOrderDiscountInfo插入订单折扣信息记录失败，订单id{orderInfoList[0].OrderId}");
                            }
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            tran.Rollback();
                            Logger.Error($"SaveCreateOrderDiscountInfo插入订单折扣信息记录异常，订单id{orderInfoList[0].OrderId}，异常：{ex}");
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 改变订单打折记录状态
        /// </summary>
        /// <param name="orderInfoList"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateCancelOrderDiscountInfo(int orderId)
        {
            bool result = false;
            string sqlSelect = @"select [UserId]
                                       ,[OrderId]
                                       ,[Pid]
                                       ,[Num]
                                       ,[ActivityId]
                                 FROM  [Activity].[dbo].[SalePromotionDiscountOrder] with(nolock)
                                 where   OrderId=@orderId and OrderStatus=1";
            string sqlOrder = @"update [Activity].[dbo].[SalePromotionDiscountOrder] with(rowlock)
                                                 set    OrderStatus=0,
                                                        LastUpdateDateTime=getdate()
                                                where   OrderId=@orderId and OrderStatus=1";
            using (var cmd = new SqlCommand(sqlSelect))
            {
                //1.修改订单打折记录状态
                try
                {
                    cmd.Parameters.Add(new SqlParameter("@orderId", orderId));
                    var selectList = await DbHelper.ExecuteSelectAsync<SalePromotionActivityDiscountOrder>(false, cmd);
                    cmd.CommandText = sqlOrder;
                    result = (await DbHelper.ExecuteNonQueryAsync(cmd) > 0);
                    if (result)
                    {
                        var keys = new List<string>();
                        foreach (var item in selectList)
                        {
                            keys.Add(item.ActivityId + "/" + item.Pid);
                            keys.Add(item.ActivityId + "/" + item.UserId);
                            keys.Add(item.OrderId.ToString());
                        }
                        DBReadOnlyFlagHelper.SetDBReadOnlyFlagCache(SalePromotionActivityConst.SalePromotionDiscountOrder, keys.ToList());
                    }
                    else
                    {
                        Logger.Error($"UpdateCancelOrderDiscountInfo取消订单更新打折记录失败，订单id{orderId}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"UpdateCancelOrderDiscountInfo取消订单更新打折记录异常，订单id{orderId}，异常{ex}");
                }
            }
            return result;
        }

        /// <summary>
        /// 修改活动商品的已售数量
        /// </summary>
        /// <param name="orderInfoList"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateProductSoldQuantity(List<ActivityPidSoldNumModel> orderInfoList)
        {
            bool result = true;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = new SqlCommand("", conn, tran))
                    {
                        try
                        {
                            var orderInfoTmp = orderInfoList.Select(item => new
                            {
                                Pid = item.Pid,
                                SoldQuantity = item.SoldNum >= 0 ? item.SoldNum : 0,
                                ActivityId = item.ActivityId
                            });
                            DataTable orderInfoDT = ConvertDataTable.ToDataTable(orderInfoTmp);
                            cmd.CommandText = @"CREATE TABLE #orderInfoTmp([Pid] [nvarchar](100) NOT NULL,
                                                                           [SoldQuantity] [int] NOT NULL,
                                                                           [ActivityId] [nvarchar](100) NOT NULL);";
                            await cmd.ExecuteNonQueryAsync();
                            using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, tran))
                            {
                                bulkcopy.BulkCopyTimeout = 660;
                                bulkcopy.DestinationTableName = "#orderInfoTmp";
                                bulkcopy.WriteToServer(orderInfoDT);
                                bulkcopy.Close();
                            }
                            string sql = @"MERGE INTO [Activity].[dbo].[SalePromotionActivityProduct] WITH ( ROWLOCK ) AS p
                                            USING #orderInfoTmp AS temp
                                            ON p.ActivityId=temp.ActivityId and p.Pid=temp.Pid and p.Is_Deleted=0
                                            WHEN MATCHED THEN
                                                UPDATE SET  p.SoldQuantity=temp.SoldQuantity;";
                            cmd.CommandText = sql;
                            await cmd.ExecuteNonQueryAsync();
                            if (result)
                            {
                                tran.Commit();
                            }
                            else
                            {
                                tran.Rollback();
                                Logger.Error($"UpdateProductSoldQuantity更新活动打折商品已售数量异常，" +
                                    $"活动id{string.Join(",", orderInfoList.Select(o => o.ActivityId).Distinct())}");
                            }
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            tran.Rollback();
                            Logger.Error($"UpdateProductSoldQuantity,更新活动打折商品已售数量异常," +
                                $"活动id{string.Join(",", orderInfoList.Select(o => o.ActivityId).Distinct())},ex：{ex}");
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 根据订单id获取订单的打折信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SalePromotionActivityDiscountOrder>> GetDiscountOrderInfoByOid(int orderId)
        {
            string sql = @"   select [UserId],
                                     [OrderId],
                                     [Pid],
                                     [Num],
                                     [ActivityId],
                                     [OrderStatus]
                              from  [Activity].[dbo].[SalePromotionDiscountOrder] with(nolock)
                              where  OrderId=@OrderId and OrderStatus=1";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@OrderId", orderId));
                var keys = new List<string>() { orderId.ToString() };
                var readOnlyFlag = await DBReadOnlyFlagHelper.GetDBReadOnlyFlagFromCache(SalePromotionActivityConst.SalePromotionDiscountOrder, keys);
                Logger.Info($"GetDiscountOrderInfoByOid只读库标识：{readOnlyFlag}");

                return await DbHelper.ExecuteSelectAsync<SalePromotionActivityDiscountOrder>(readOnlyFlag, cmd);
            }
        }

        #endregion

        #region 自动上下架

        /// <summary>
        /// 批量获取活动的商品信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<IList<SalePromotionActivityProductDB>> GetProductListByActivityIdAsync(List<SalePromotionActivityDiscountOrder> activitysAndPids)
        {
            var list = new List<SalePromotionActivityProductDB>();
            string sql = @" select distinct p.ActivityId,p.Pid,
                                            p.LimitQuantity,p.SalePrice,
                                            p.Is_UnShelve
                             from  [Activity].[dbo].[SalePromotionActivityProduct] p with(nolock)
                                    Join   Activity..SplitString(@pidList, ',', 1) b1
                                       on p.Pid=b1.Item
                                    Join   Activity..SplitString(@activityIdList, ',', 1) b2
                                       on p.ActivityId=b2.Item
                            where  p.Is_Deleted=0 ";
            var pidList = activitysAndPids.Select(p => p.Pid).Distinct();
            var activityIdList = activitysAndPids.Select(p => p.ActivityId).Distinct();
            if (!(pidList?.Count() > 0) || !(activityIdList?.Count() > 0))
            {
                return list;
            }
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@pidList", string.Join(",", pidList));
                cmd.Parameters.AddWithValue("@activityIdList", string.Join(",", activityIdList));
                var keys = new List<string>();
                keys.AddRange(pidList);
                keys.AddRange(activityIdList);
                var readOnlyFlag = await DBReadOnlyFlagHelper.GetDBReadOnlyFlagFromCache(SalePromotionActivityConst.SalePromotionActivityAndProduct, keys);
                Logger.Info($"GetProductListByActivityIdAsync只读库标识：{readOnlyFlag}");

                list = (await DbHelper.ExecuteSelectAsync<SalePromotionActivityProductDB>(readOnlyFlag, cmd)).ToList();
                return list;
            }
        }

        /// <summary>
        /// 获取活动商品信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<IList<SalePromotionActivityProductDB>> GetProductListByActivityId(string activityId)
        {
            var list = new List<SalePromotionActivityProductDB>();
            string sql = @" select ActivityId,Pid,LimitQuantity,SalePrice,Is_UnShelve
                             from  [Activity].[dbo].[SalePromotionActivityProduct]  with(nolock)
                            where  Is_Deleted=0 and ActivityId=@activityId ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@activityId", activityId);
                list = (await DbHelper.ExecuteSelectAsync<SalePromotionActivityProductDB>(false, cmd)).ToList();
                return list;
            }
        }

        /// <summary>
        /// 上架活动的商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public static async Task<int> OnShelveProduct(string activityId, List<string> pidList)
        {
            int result = -1;
            if (!(pidList?.Count > 0))
            {
                return result;
            }
            string sql = @"  update p
                              set   p.Is_UnShelve=0
                             from   [Activity].[dbo].[SalePromotionActivityProduct] p with(rowlock)
                                    Join   Activity..SplitString(@pidList, ',', 1) b
                                       on p.Pid=b.Item
                            where  p.Is_Deleted=0 and ActivityId=@activityId
                                    and p.LimitQuantity>0 and p.LimitQuantity>p.SoldQuantity";
            using (var cmd = new SqlCommand(sql))
            {
                try
                {
                    cmd.Parameters.Add(new SqlParameter("@pidList", string.Join(",", pidList)));
                    cmd.Parameters.AddWithValue("@activityId", activityId);
                    int.TryParse((await DbHelper.ExecuteNonQueryAsync(cmd)).ToString(), out result);
                    if (result > 0)
                    {
                        var keys = new List<string>();
                        keys.AddRange(pidList);
                        keys.Add(activityId);
                        DBReadOnlyFlagHelper.SetDBReadOnlyFlagCache(SalePromotionActivityConst.SalePromotionActivityAndProduct, keys.ToList());
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"OnShelveProduct上架活动的商品异常，活动id{activityId}，异常：{ex}");
                }
                return result;
            }
        }

        /// <summary>
        /// 下架活动的商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public static async Task<int> UnShelveProduct(string activityId, List<string> pidList)
        {
            int result = -1;
            if (!(pidList?.Count > 0))
            {
                return result;
            }
            string sql = @"  update p
                              set   p.Is_UnShelve=1
                             from   [Activity].[dbo].[SalePromotionActivityProduct] p with(rowlock)
                                    Join   Activity..SplitString(@pidList, ',', 1) b
                                       on p.Pid=b.Item
                            where  p.Is_Deleted=0 and ActivityId=@activityId
                                    and (p.LimitQuantity=0 or p.LimitQuantity<=p.SoldQuantity)";
            using (var cmd = new SqlCommand(sql))
            {
                try
                {
                    cmd.Parameters.Add(new SqlParameter("@pidList", string.Join(",", pidList)));
                    cmd.Parameters.AddWithValue("@activityId", activityId);
                    int.TryParse((await DbHelper.ExecuteNonQueryAsync(cmd)).ToString(), out result);
                    if (result > 0)
                    {
                        var keys = new List<string>();
                        keys.AddRange(pidList);
                        DBReadOnlyFlagHelper.SetDBReadOnlyFlagCache(SalePromotionActivityConst.SalePromotionActivityAndProduct, keys.ToList());
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"UnShelveProduct下架活动的商品异常，活动id{activityId}，异常：{ex}");
                }
                return result;
            }
        }

        /// <summary>
        /// 修改活动的上下架状态
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="shelveStatus"></param>
        /// <returns></returns>
        public static async Task<int> SetActivityShelveStatus(string activityId, int shelveStatus, List<string> pidList)
        {
            int result = -1;
            string sql = @"  update [Activity].[dbo].[SalePromotionActivity] with(rowlock)
                              set   Is_UnShelveAuto=@shelveStatus
                             where  Is_Deleted=0 and ActivityId=@activityId ";
            using (var cmd = new SqlCommand(sql))
            {
                try
                {
                    cmd.Parameters.AddWithValue("@shelveStatus", shelveStatus);
                    cmd.Parameters.AddWithValue("@activityId", activityId);
                    int.TryParse((await DbHelper.ExecuteNonQueryAsync(cmd)).ToString(), out result);
                    if (result > 0)
                    {
                        var keys = new List<string>() { activityId };
                        if (pidList?.Count > 0)
                        {
                            keys.AddRange(pidList);
                        }
                        DBReadOnlyFlagHelper.SetDBReadOnlyFlagCache(SalePromotionActivityConst.SalePromotionActivityAndProduct, keys.ToList());
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"SetActivityShelveStatus修改活动的上下架状态，活动id{activityId},下架状态{shelveStatus}，异常：{ex}");
                }
                return result;
            }
        }

        #endregion


    }
}
