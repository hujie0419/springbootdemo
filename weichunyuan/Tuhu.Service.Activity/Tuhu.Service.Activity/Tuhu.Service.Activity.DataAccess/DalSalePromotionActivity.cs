using Common.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Activity.Const;
using Tuhu.Service.Activity.DataAccess.Models.SalePromotionActivity;
using Tuhu.Service.Activity.DataAccess.Tools;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalSalePromotionActivity
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalSalePromotionActivity));

        #region 查询活动

        /// <summary>
        /// 检查活动名称是否重复
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ActivityId"></param>
        /// <returns>true为不重复</returns>
        public static async Task<bool> CheckNameRepeatAsync(string Name, string ActivityId)
        {
            bool result = false;
            int count;
            string Sql = @"select count(*)
                            from  Activity.dbo.SalePromotionActivity WITH (NOLOCK) 
                            where Name=@Name and ActivityId<>@ActivityId";
            using (var cmd = new SqlCommand(Sql))
            {
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.Parameters.AddWithValue("@ActivityId", ActivityId);
                int.TryParse((await DbHelper.ExecuteScalarAsync(false, cmd)).ToString(), out count);
                if (count == 0)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 分页查询活动列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<SelectActivityListModel> SelectActivityListAsync(SalePromotionActivityModel model, int pageIndex, int pageSize)
        {
            #region sql
            string sqlSelect = @"select distinct  act.[ActivityId],
                                                  act.[Name],
                                                  act.[Description],
                                                  act.[Banner],
                                                  act.[PromotionType],
                                                  act.[Is_DefaultLabel],
                                                  act.[Label],
                                                  act.[Is_PurchaseLimit],
                                                  act.[LimitQuantity],
                                                  act.[PaymentMethod],
                                                  act.[InstallMethod],
                                                  act.[StartTime],
                                                  act.[EndTime],
                                                  act.[AuditStatus],
                                                  act.[AuditUserName],
                                                  act.[Is_UnShelve],
                                                  act.[CreateDateTime],
                                                  act.[CreateUserName],
                                                  act.[LastUpdateDateTime],
                                                  act.[LastUpdateUserName],
                                                  dis.DiscountMethod ";
            string sqlFrom = @"  from   [Activity].[dbo].[SalePromotionActivity] act  WITH (NOLOCK)
                                  join  [Activity].[dbo].[SalePromotionActivityDiscount] dis  WITH (NOLOCK)
                                        on act.ActivityId=dis.ActivityId and dis.Is_Deleted=0 ";
            StringBuilder sqlWhere = new StringBuilder(@" where act.Is_Deleted=0 ");
            string sqlOrderBy = @" order by act.CreateDateTime desc ";
            string sqlPager = @" OFFSET (@pageIndex -1) * @pageSize ROWS
                              FETCH NEXT @pageSize ROWS ONLY";
            string countFrom = @" from   Activity.[dbo].[SalePromotionActivity]  act  WITH (NOLOCK)";
            #region 查询条件拼接

            string condition = string.Empty;
            if (!string.IsNullOrWhiteSpace(model.Pid))
            {
                sqlFrom += @" join [Activity].[dbo].[SalePromotionActivityProduct] pro  WITH (NOLOCK)
                                   on act.ActivityId = pro.ActivityId
                                      and pro.Is_Deleted = 0";
                countFrom += @" join [Activity].[dbo].[SalePromotionActivityProduct] pro  WITH (NOLOCK)
                                   on act.ActivityId = pro.ActivityId
                                      and pro.Is_Deleted = 0";
                condition += " and pro.Pid=@Pid";
            }
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                condition += " and act.Name like @Name";
            }
            //时间区间交叉
            if (!string.IsNullOrWhiteSpace(model.StartTime) && !string.IsNullOrWhiteSpace(model.EndTime))
            {
                condition += " and not(act.StartTime>@EndTime or act.EndTime<@StartTime) ";
            }
            else if (!string.IsNullOrWhiteSpace(model.StartTime) && string.IsNullOrWhiteSpace(model.EndTime))
            {
                condition += " and not(act.EndTime<@StartTime) ";
            }
            else if (string.IsNullOrWhiteSpace(model.StartTime) && !string.IsNullOrWhiteSpace(model.EndTime))
            {
                condition += " and not(act.StartTime>@EndTime) ";
            }
            switch (model.Status)
            {
                case (int)SalePromotionActivityStatus.All://所有
                    break;
                case (int)SalePromotionActivityStatus.Online://进行中
                    condition += @"";
                    sqlWhere.Append(@" and act.AuditStatus = 2
                                       and act.Is_UnShelve = 0
                                       and (getdate() between act.StartTime and act.EndTime)");
                    break;
                case (int)SalePromotionActivityStatus.WaitAudit://待审核
                    sqlWhere.Append(@" and act.AuditStatus=1
                                       and act.Is_UnShelve =0 
                                       and (getdate() between act.StartTime and act.EndTime)");
                    break;
                case (int)SalePromotionActivityStatus.StayOnline://待上线
                    sqlWhere.Append(@" and act.AuditStatus = 2
                                       and act.Is_UnShelve = 0 
                                       and act.StartTime>getdate()");
                    break;
                case (int)SalePromotionActivityStatus.Rejected://已拒绝
                    sqlWhere.Append(@" and act.AuditStatus=3 
                                       and act.Is_UnShelve = 0 
                                       and getdate()<act.EndTime");
                    break;
                case (int)SalePromotionActivityStatus.End://已结束
                    sqlWhere.Append(@" and (act.EndTime<getdate() or act.Is_UnShelve = 1) ");
                    break;
                default:
                    break;
            }

            #endregion 
            string SqlCountList = $@" 
       select      (select  count(distinct act.ActivityId)   
	            	 {countFrom}
	            	 where  act.Is_Deleted=0 {condition}
                            and act.AuditStatus = 2
                            and act.Is_UnShelve = 0
                            and (getdate() between act.StartTime and act.EndTime)) 
           as OnlineCount,
                   (select  count(distinct act.ActivityId)   
	            	 {countFrom}
	            	 where  act.Is_Deleted=0 {condition} ) 
           as AllCount,
                   (select  count(distinct act.ActivityId)  
	            	 {countFrom}
	                 where  act.Is_Deleted=0   {condition}
                            and act.AuditStatus=1
                            and act.Is_UnShelve =0 
                            and (getdate() between act.StartTime and act.EndTime)) 
           as WaitAuditCount,
                   (select count(distinct act.ActivityId)  
	            	 {countFrom}
	            	 where act.Is_Deleted=0  {condition}
                           and act.AuditStatus = 2
                           and act.Is_UnShelve = 0 
                           and act.StartTime>getdate())
           as StayOnlineCount,
	               (select count(distinct act.ActivityId)  
	            	 {countFrom}
	                 where act.Is_Deleted=0  {condition}
                           and act.AuditStatus=3 
                           and act.Is_UnShelve = 0 
                           and getdate()<act.EndTime) 
           as RejectedCount,
	               (select count(distinct act.ActivityId)
	            	 {countFrom}
	            	where  act.Is_Deleted=0  {condition}
                          and (act.EndTime<getdate() or act.Is_UnShelve = 1) ) 
           as EndCount ";
            string sqlSource = sqlSelect + sqlFrom + sqlWhere + condition + sqlOrderBy + sqlPager;
            string sqlCount = "select count(distinct act.ActivityId) " + sqlFrom + sqlWhere + condition;
            var selectResult = new SelectActivityListModel()
            {
                Counts = new ActicityListCountModel()
            };
            #endregion
            try
            {
                using (var cmd = new SqlCommand(sqlCount))
                {
                    //1.查询数据列表的行数
                    cmd.Parameters.AddWithValue("@Pid", model.Pid);
                    cmd.Parameters.AddWithValue("@Name", $"%{model.Name}%");
                    cmd.Parameters.AddWithValue("@StartTime", model.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", model.EndTime);
                    cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    int count;
                    //查询行数
                    int.TryParse((await DbHelper.ExecuteScalarAsync(false, cmd)).ToString(), out count);
                    selectResult.CurrentStatusCount = count;
                    if (count > 0)
                    {
                        //2.获取活动信息列表
                        cmd.CommandText = sqlSource;
                        selectResult.ActivityList = (await DbHelper.ExecuteSelectAsync<SalePromotionActivityModel>(false, cmd)).ToList();
                    }
                    //3.获取各个状态活动的行数
                    cmd.CommandText = SqlCountList;
                    selectResult.Counts = await DbHelper.ExecuteFetchAsync<ActicityListCountModel>(false, cmd);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"SelectActivityListAsync异常，{ex}");
            }
            return selectResult;
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="activityId">活动id</param>
        /// <returns></returns>
        public static async Task<SalePromotionActivityModel> GetActivityInfoAsync(string activityId)
        {
            SalePromotionActivityModel model;
            //获取活动信息sql
            string sqlSelectActivity = @"select ActivityId,Name,Description,Banner,
                                                 PromotionType,Is_DefaultLabel,Label,
                                                 [Is_PurchaseLimit],
                                                 [LimitQuantity],
                                                 [PaymentMethod],
                                                 [InstallMethod],Is_UnShelve,Is_UnShelveAuto,
                                                 [StartTime],[EndTime],
                                                 AuditStatus,AuditDateTime,AuditRemark,AuditUserName,
                                                 CreateUserName,CreateDateTime
                                          from  [Activity].[dbo].[SalePromotionActivity]  WITH (NOLOCK)
                                         where  Is_Deleted=0 and ActivityId=@ActivityId ";
            //获取活动内容sql
            string sqlSelectContent = @"select DiscountMethod,Condition,DiscountRate
                                         from  [Activity].[dbo].[SalePromotionActivityDiscount]  WITH (NOLOCK)
                                        where  Is_Deleted=0 and ActivityId=@ActivityId ";
            using (var cmd = new SqlCommand(sqlSelectActivity))
            {
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                //获取活动
                model = await DbHelper.ExecuteFetchAsync<SalePromotionActivityModel>(false, cmd);
                if (model != null)
                {
                    cmd.CommandText = sqlSelectContent;
                    //获取活动内容
                    model.DiscountContentList = (await DbHelper.ExecuteSelectAsync<SalePromotionActivityDiscount>(false, cmd)).ToList();
                }
                return model;
            }
        }

        /// <summary>
        /// 获取活动的打折内容
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<List<SalePromotionActivityDiscount>> GetActivityContentAsync(string activityId)
        {
            string sqlSelectContent = @"select DiscountMethod,Condition,DiscountRate
                                         from  [Activity].[dbo].[SalePromotionActivityDiscount]  WITH (NOLOCK)
                                        where  Is_Deleted=0 and ActivityId=@ActivityId ";
            using (var cmd = new SqlCommand(sqlSelectContent))
            {
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                return (await DbHelper.ExecuteSelectAsync<SalePromotionActivityDiscount>(false, cmd)).ToList();
            }
        }

        /// <summary>
        /// 获取活动的审核状态
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static async Task<int> GetActivityAuditStatusAsync(string activityId)
        {
            int auditStatus = -1;
            string Sql = @"select AuditStatus
                            from  [Activity].[dbo].[SalePromotionActivity] with(nolock)
                           where  ActivityId=@ActivityId and Is_Deleted=0";
            using (var cmd = new SqlCommand(Sql))
            {
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                var s = (await DbHelper.ExecuteScalarAsync(false, cmd));
                int.TryParse(s.ToString(), out auditStatus);
            }
            return auditStatus;
        }

        #endregion

        #region 操作活动

        /// <summary>
        /// 新增活动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<bool> InsertActivityAsync(SalePromotionActivityModel model)
        {
            bool result = true;
            //新增活动sql
            string SqlInsertActivity = @"insert into
     Activity.dbo.SalePromotionActivity(ActivityId,Name,Description,Banner,
                                        PromotionType,Is_DefaultLabel,Label,Is_PurchaseLimit,LimitQuantity,
                                        PaymentMethod,InstallMethod,StartTime,EndTime,AuditStatus,Is_UnShelve,
                                        CreateDateTime,CreateUserName,Is_Deleted)
                                values(@ActivityId,@Name,@Description,@Banner,
                                       @PromotionType,@Is_DefaultLabel,@Label,@Is_PurchaseLimit,@LimitQuantity,
                                       @PaymentMethod,@InstallMethod,@StartTime,@EndTime,0,@Is_UnShelve,
                                       getdate(),@CreateUserName,0)";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    using (var cmd = new SqlCommand("", conn, tran))
                    {
                        try
                        {
                            #region 1.新增活动
                            cmd.CommandText = SqlInsertActivity;
                            cmd.Parameters.Add(new SqlParameter("@ActivityId", model.ActivityId ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@Name", model.Name ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@Description", model.Description ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@Banner", model.Banner ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@PromotionType", model.PromotionType));
                            cmd.Parameters.Add(new SqlParameter("@Is_DefaultLabel", model.Is_DefaultLabel));
                            cmd.Parameters.Add(new SqlParameter("@Label", string.IsNullOrWhiteSpace(model.Label) ? "" : model.Label));
                            cmd.Parameters.Add(new SqlParameter("@Is_PurchaseLimit", model.Is_PurchaseLimit));
                            cmd.Parameters.Add(new SqlParameter("@LimitQuantity", model.LimitQuantity));
                            cmd.Parameters.Add(new SqlParameter("@PaymentMethod", model.PaymentMethod));
                            cmd.Parameters.Add(new SqlParameter("@InstallMethod", model.InstallMethod));
                            cmd.Parameters.Add(new SqlParameter("@Is_UnShelve", SalePromotionActivityShelveStatus.OnShelve));
                            cmd.Parameters.Add(new SqlParameter("@StartTime", model.StartTime ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@EndTime", model.EndTime ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@CreateUserName", model.CreateUserName ?? ""));
                            if ((await cmd.ExecuteNonQueryAsync()) != 1)
                            {
                                result = false;
                            }
                            cmd.Parameters.Clear();
                            #endregion

                            #region 2.批量新增活动内容
                            switch (model.PromotionType)
                            {
                                case (int)SalePromotionActivityType.FullDiscount:
                                    //新增满折活动
                                    if (result && model.DiscountContentList != null && model.DiscountContentList.Count > 0)
                                    {
                                        var contentTmp = model.DiscountContentList.Select(item => new
                                        {
                                            ActivityId = model.ActivityId,
                                            DiscountMethod = item.DiscountMethod,
                                            Condition = item.Condition,
                                            DiscountRate = item.DiscountRate,
                                            CreateDateTime = DateTime.Now,
                                            Is_Deleted = 0
                                        });
                                        DataTable contentDT = ToDataTable(contentTmp);
                                        cmd.CommandText = @"CREATE TABLE #contentTemp([ActivityId] [nvarchar](36) NOT NULL,
	                                                                                  [DiscountMethod] [int] NOT NULL,
	                                                                                  [Condition] [decimal](18,2) NOT NULL,
	                                                                                  [DiscountRate] [decimal](18,2) NOT NULL,
	                                                                                  [CreateDateTime] [datetime] NOT NULL,
	                                                                                  [Is_Deleted] [bit] NOT NULL);";
                                        await cmd.ExecuteNonQueryAsync();
                                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, tran))
                                        {
                                            bulkcopy.BulkCopyTimeout = 660;
                                            bulkcopy.DestinationTableName = "#contentTemp";
                                            bulkcopy.WriteToServer(contentDT);
                                            bulkcopy.Close();
                                        }
                                        string sqlInsertContent = @"insert into
                                        Activity.dbo.SalePromotionActivityDiscount(ActivityId,DiscountMethod,Condition,
                                                                             DiscountRate,CreateDateTime,Is_Deleted)
                                                              select  ActivityId,DiscountMethod,
                                                                      Condition,DiscountRate,CreateDateTime,
                                                                       Is_Deleted
                                                              from    #contentTemp";
                                        cmd.CommandText = sqlInsertContent;
                                        if ((await cmd.ExecuteNonQueryAsync()) != model.DiscountContentList.Count)
                                        {
                                            result = false;
                                        }
                                    }
                                    break;
                            }
                            #endregion 
                            if (result)
                            {
                                tran.Commit();
                            }
                            else
                            {
                                tran.Rollback();
                            }
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            tran.Rollback();
                            Logger.Error($"InsertActivityAsync异常，{ex}");
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 修改活动信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateActivityAsync(SalePromotionActivityModel model)
        {
            bool result = true;
            #region SQL语句
            //更新活动sql 
            string SqlUpdateActivity = @"update  [Activity].[dbo].[SalePromotionActivity] with(rowlock)
                                           set   [Name]=@Name,
                                                 [Description]=@Description,
                                                 [Banner]=@Banner,
                                                 [PromotionType]=@PromotionType,
                                                 [Is_DefaultLabel]=@Is_DefaultLabel,
                                                 [Label]=@Label,
                                                 [Is_PurchaseLimit]=@Is_PurchaseLimit,
                                                 [LimitQuantity]=@LimitQuantity,
                                                 [PaymentMethod]=@PaymentMethod,
                                                 [InstallMethod]=@InstallMethod,
                                                 [StartTime]=@StartTime,
                                                 [EndTime]=@EndTime,
                                                 [Is_UnShelve]=0,
                                                 LastUpdateDateTime=getdate(),
                                                 LastUpdateUserName=@LastUpdateUserName
                                          where  ActivityId=@ActivityId and Is_Deleted=0";
            //删除活动内容
            string SqlDeleteRules = @"delete from Activity.dbo.SalePromotionActivityDiscount
                                        where ActivityId=@ActivityId";
            #endregion
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = new SqlCommand("", conn, tran))
                    {
                        try
                        {
                            #region 更新活动
                            cmd.CommandText = SqlUpdateActivity;
                            cmd.Parameters.Add(new SqlParameter("@Name", model.Name ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@ActivityId", model.ActivityId ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@Description", model.Description ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@Banner", model.Banner ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@PromotionType", model.PromotionType));
                            cmd.Parameters.Add(new SqlParameter("@Is_DefaultLabel", model.Is_DefaultLabel));
                            cmd.Parameters.Add(new SqlParameter("@Label", model.Label ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@Is_PurchaseLimit", model.Is_PurchaseLimit));
                            cmd.Parameters.Add(new SqlParameter("@LimitQuantity", model.LimitQuantity));
                            cmd.Parameters.Add(new SqlParameter("@PaymentMethod", model.PaymentMethod));
                            cmd.Parameters.Add(new SqlParameter("@InstallMethod", model.InstallMethod));
                            cmd.Parameters.Add(new SqlParameter("@StartTime", model.StartTime ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@EndTime", model.EndTime ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@LastUpdateUserName", model.LastUpdateUserName ?? ""));
                            result = await cmd.ExecuteNonQueryAsync() == 1;
                            #endregion

                            #region 更新活动规则
                            if (result && model.ChannelKeyList != null)
                            {
                                //1.删除原有活动规则
                                cmd.Parameters.Clear();
                                cmd.CommandText = SqlDeleteRules;
                                cmd.Parameters.Add(new SqlParameter("@ActivityId", model.ActivityId));
                                await cmd.ExecuteNonQueryAsync();
                                cmd.Parameters.Clear();
                                //2.批量插入修改后的活动规则
                                var contentTmp = model.DiscountContentList.Select(item => new
                                {
                                    ActivityId = model.ActivityId,
                                    DiscountMethod = item.DiscountMethod,
                                    Condition = item.Condition,
                                    DiscountRate = item.DiscountRate,
                                    CreateDateTime = DateTime.Now,
                                    Is_Deleted = 0
                                });
                                DataTable contentDT = ToDataTable(contentTmp);
                                cmd.CommandText = @"CREATE TABLE #contentTemp([ActivityId] [nvarchar](36) NOT NULL,
	                                                                          [DiscountMethod] [int] NOT NULL,
	                                                                          [Condition] [decimal](18,2) NOT NULL,
	                                                                          [DiscountRate] [decimal](18,2) NOT NULL,
	                                                                          [CreateDateTime] [datetime] NOT NULL,
	                                                                          [Is_Deleted] [bit] NOT NULL);";
                                await cmd.ExecuteNonQueryAsync();
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, tran))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "#contentTemp";
                                    bulkcopy.WriteToServer(contentDT);
                                    bulkcopy.Close();
                                }
                                string sqlInsertContent = @"insert into
                                        Activity.dbo.SalePromotionActivityDiscount(ActivityId,DiscountMethod,Condition,
                                                                             DiscountRate,CreateDateTime,Is_Deleted)
                                                              select  ActivityId,DiscountMethod,
                                                                      Condition,DiscountRate,CreateDateTime,
                                                                       Is_Deleted
                                                              from    #contentTemp";
                                cmd.CommandText = sqlInsertContent;
                                if ((await cmd.ExecuteNonQueryAsync()) != model.DiscountContentList.Count)
                                {
                                    result = false;
                                }
                            }
                            #endregion
                            if (result)
                            {
                                tran.Commit();
                            }
                            else
                            {
                                tran.Rollback();
                            }
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            tran.Rollback();
                            Logger.Error($"UpdateActivityAsync异常，{ex}");
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 审核后修改活动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateActivityAfterAuditAsync(SalePromotionActivityModel model)
        {
            bool result = false;
            string SqlUpdateActivity = @"update [Activity].[dbo].[SalePromotionActivity] with(rowlock)
                                           set   StartTime=@StartTime,
                                                 EndTime=@EndTime,
                                                 AuditStatus=@AuditStatus,
                                                 Is_UnShelve=0,
                                                 LastUpdateDateTime=getdate(),
                                                 LastUpdateUserName=@LastUpdateUserName
                                          where  ActivityId=@ActivityId and Is_Deleted=0";
            using (var cmd = new SqlCommand(SqlUpdateActivity))
            {
                try
                {
                    cmd.CommandText = SqlUpdateActivity;
                    cmd.Parameters.Add(new SqlParameter("@ActivityId", model.ActivityId));
                    cmd.Parameters.Add(new SqlParameter("@StartTime", model.StartTime));
                    cmd.Parameters.Add(new SqlParameter("@EndTime", model.EndTime));
                    cmd.Parameters.Add(new SqlParameter("@AuditStatus", model.AuditStatus));
                    cmd.Parameters.Add(new SqlParameter("@LastUpdateUserName", model.LastUpdateUserName));
                    result = await DbHelper.ExecuteNonQueryAsync(cmd) == 1;
                    if (result)
                    {
                        await SetDBFlagCache(model.ActivityId);
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    Logger.Error($"UpdateActivityAfterAuditAsync异常，{ex}");
                }
            }
            return result;
        }

        /// <summary>
        /// 下架活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<bool> UnShelveActivityAsync(string activityId, string userName)
        {
            bool result = false;
            string Sql = @"update [Activity].[dbo].[SalePromotionActivity] with(rowlock)
                            set   Is_UnShelve=1,
                                  LastUpdateDateTime=getdate(),
                                  LastUpdateUserName=@LastUpdateUserName
                           where  ActivityId=@ActivityId and Is_Deleted=0";
            using (var cmd = new SqlCommand(Sql))
            {
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                cmd.Parameters.AddWithValue("@LastUpdateUserName", userName);
                if ((await DbHelper.ExecuteNonQueryAsync(cmd)) == 1)
                {
                    result = true;
                    await SetDBFlagCache(activityId);
                }
            }
            return result;
        }

        /// <summary>
        /// 修改活动审核状态
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userName"></param>
        /// <param name="auditStatus"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static async Task<bool> SetActivityAuditStatusAsync(string activityId, string userName, int auditStatus, string remark)
        {
            bool result = false;
            string auditTime = string.Empty;
            if (auditStatus == 2 || auditStatus == 3)
            {
                auditTime = @"AuditDateTime=getdate(),";
            }
            string Sql = string.Format(@"update [Activity].[dbo].[SalePromotionActivity] with(rowlock)
                                          set   AuditStatus=@auditStatus,
                                                AuditUserName=@userName,
                                                {0}
                                                AuditRemark=@remark
                                         where  ActivityId=@ActivityId and Is_Deleted=0", auditTime);
            using (var cmd = new SqlCommand(Sql))
            {
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                cmd.Parameters.AddWithValue("@auditStatus", auditStatus);
                cmd.Parameters.AddWithValue("@userName", userName);
                cmd.Parameters.AddWithValue("@remark", remark);
                if ((await DbHelper.ExecuteNonQueryAsync(cmd)) == 1)
                {
                    result = true;
                    await SetDBFlagCache(activityId);
                }
            }
            return result;
        }

        #endregion

        #region 商品操作

        /// <summary>
        /// 新增活动商品
        /// </summary>
        /// <param name="productList"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<bool> InsertActivityProductListAsync(List<SalePromotionActivityProduct> productList, string activityId, string userName)
        {
            bool result = true;
            int newAuditStatus = 0;
            int oldAuditStatus = await GetActivityAuditStatus(activityId);
            if (oldAuditStatus > 0)
            {
                newAuditStatus = 1;
            }
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = new SqlCommand("", conn, tran))
                    {
                        try
                        {
                            #region 新增商品
                            if (productList?.Count > 0)
                            {
                                var productTmp = productList.Select(item => new
                                {
                                    ActivityId = activityId,
                                    Pid = item.Pid,
                                    ProductName = item.ProductName,
                                    TotalStock = item.TotalStock,
                                    CostPrice = item.CostPrice,
                                    SalePrice = item.SalePrice,
                                    LimitQuantity = item.LimitQuantity,
                                    SoldQuantity = item.SoldQuantity,
                                    CreateDateTime = DateTime.Now,
                                    Is_UnShelve = 0,
                                    Is_Deleted = 0
                                });
                                DataTable productDT = ToDataTable(productTmp);
                                cmd.CommandText = @"CREATE TABLE #productTemp([ActivityId] [nvarchar](50) NOT NULL,
	                                                                          [Pid] [nvarchar](100) NOT NULL,
	                                                                          [ProductName] [nvarchar](100) NULL,
	                                                                          [TotalStock] [int] NULL,
	                                                                          [CostPrice] [decimal](18, 2) NULL,
	                                                                          [SalePrice] [decimal](18, 2) NULL,
	                                                                          [LimitQuantity] [int] NOT NULL,
	                                                                          [SoldQuantity] [int] NOT NULL,
	                                                                          [CreateDateTime] [datetime] NOT NULL,
	                                                                          [Is_UnShelve] [bit] NOT NULL,
	                                                                          [Is_Deleted] [bit] NOT NULL);";
                                await cmd.ExecuteNonQueryAsync();
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, tran))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "#productTemp";
                                    bulkcopy.WriteToServer(productDT);
                                    bulkcopy.Close();
                                }
                                string sqlInsertProduct = @"MERGE INTO  Activity.dbo.SalePromotionActivityProduct WITH ( ROWLOCK ) AS ap
                                            USING #productTemp AS temp
                                            ON ap.Pid = temp.Pid and ap.ActivityId=@activityId and ap.Is_Deleted=1
                                            WHEN MATCHED THEN
                                                UPDATE SET ap.Is_Deleted=0,
                                                           ap.Is_UnShelve=0,
                                                           ap.ProductName=temp.ProductName,
                                                           ap.CostPrice=temp.CostPrice,
                                                           ap.TotalStock=temp.TotalStock,
                                                           ap.SalePrice=temp.SalePrice,
                                                           ap.LimitQuantity=temp.LimitQuantity,
                                                           ap.ImageUrl='',
                                                           ap.LastUpdateDateTime = GETDATE()
                                            WHEN NOT MATCHED THEN
                                                INSERT (ActivityId,Pid,ProductName,CostPrice,TotalStock,
                                                        SalePrice,LimitQuantity,SoldQuantity,CreateDateTime,Is_UnShelve,Is_Deleted)
                                                VALUES ( temp.ActivityId,temp.Pid,temp.ProductName,
                                                         temp.CostPrice,temp.TotalStock,temp.SalePrice,
                                                         temp.LimitQuantity,0,GETDATE(),0,0);";
                                cmd.Parameters.Add(new SqlParameter("@activityId", activityId));
                                cmd.CommandText = sqlInsertProduct;
                                if ((await cmd.ExecuteNonQueryAsync()) != productList.Count)
                                {
                                    result = false;
                                }
                            }
                            #endregion
                            if (result)
                            {
                                #region 更新活动 重置状态
                                string sqlUpdate = @"update  [Activity].[dbo].[SalePromotionActivity] with(rowlock)
                                                       set   AuditStatus=@newAuditStatus,
                                                             Is_UnShelve=0,
                                                             LastUpdateDateTime=getdate(),
                                                             LastUpdateUserName=@LastUpdateUserName
                                                      where  ActivityId=@activityId and Is_Deleted=0";
                                cmd.CommandText = sqlUpdate;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(new SqlParameter("@newAuditStatus", newAuditStatus));
                                cmd.Parameters.Add(new SqlParameter("@activityId", activityId));
                                cmd.Parameters.Add(new SqlParameter("@LastUpdateUserName", userName ?? ""));
                                if ((await cmd.ExecuteNonQueryAsync()) != 1)
                                {
                                    result = false;
                                }
                                #endregion
                            }
                            if (result)
                            {
                                tran.Commit();
                                await SetDBFlagCache(activityId);
                            }
                            else
                            {
                                tran.Rollback();
                            }
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            tran.Rollback();
                            Logger.Error($"InsertActivityProductListAsync异常，{ex}");
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 修改活动商品库存
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="stock"></param>
        /// <returns></returns>
        public static async Task<int> SetProductLimitStockAsync(string activityId, List<string> pidList, int stock, string userName)
        {
            int result = 0;
            bool transResult = false;
            int newAuditStatus = 0;
            int oldAuditStatus = await GetActivityAuditStatus(activityId);
            if (oldAuditStatus > 0)
            {
                newAuditStatus = 1;
            }
            string SqlProduct = @"update p
                                   set   p.LimitQuantity=@stock,
                                         p.LastUpdateDateTime=getdate()
                                   from  [Activity].[dbo].[SalePromotionActivityProduct] p with(rowlock)
                                         JOIN Activity..SplitString(@pidList, ',', 1) AS B ON p.pid = B.Item
                                  where  p.ActivityId=@activityId and p.Is_Deleted=0
                                         and @stock>=p.SoldQuantity;";
            string sqlUpdateActivity = @"update [Activity].[dbo].[SalePromotionActivity] with(rowlock)
                                           set   AuditStatus=@newAuditStatus,
                                                 Is_UnShelve=0,
                                                 LastUpdateDateTime=getdate(),
                                                 LastUpdateUserName=@userName
                                          where  ActivityId=@activityId and Is_Deleted=0;";
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                dbHelper.BeginTransaction();
                try
                {
                    using (var cmd = dbHelper.CreateCommand())
                    {
                        //1.修改商品库存
                        cmd.CommandText = SqlProduct;
                        cmd.Parameters.Add(new SqlParameter("@pidList", string.Join(",", pidList)));
                        cmd.Parameters.Add(new SqlParameter("@stock", stock));
                        cmd.Parameters.Add(new SqlParameter("@activityId", activityId));
                        var dbresult = await DbHelper.ExecuteNonQueryAsync(cmd);
                        int.TryParse(dbresult.ToString(), out result);
                        if (result > 0)
                        {
                            //2.修改活动的状态
                            cmd.CommandText = sqlUpdateActivity;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter("@activityId", activityId));
                            cmd.Parameters.Add(new SqlParameter("@newAuditStatus", newAuditStatus));
                            cmd.Parameters.Add(new SqlParameter("@userName", userName));
                            if (await DbHelper.ExecuteNonQueryAsync(cmd) == 1)
                            {
                                transResult = true;
                            }
                        }
                    }
                    if (transResult)
                    {
                        dbHelper.Commit();
                        await SetDBFlagCache(activityId);
                    }
                    else
                    {
                        dbHelper.Rollback();
                        Logger.Warn($"SetProductLimitStockAsync修改活动商品库存失败,活动id{activityId},pid:{string.Join(",", pidList)}");
                    }
                }
                catch (Exception ex)
                {
                    dbHelper.Rollback();
                    Logger.Error($"SetProductLimitStockAsync异常，{ex}");
                }
            }
            return result;
        }

        /// <summary>
        /// 修改活动商品url
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <param name="imgUrl"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static async Task<int> SetProductImageAsync(string activityId, List<string> pidList, string imgUrl, string userName)
        {
            int result = 0;
            bool transResult = false;
            string SqlProduct = string.Empty;
            int newAuditStatus = 0;
            int oldAuditStatus = await GetActivityAuditStatus(activityId);
            if (oldAuditStatus > 0)
            {
                newAuditStatus = 1;
            }
            string sqlUpdateActivity = @"update [Activity].[dbo].[SalePromotionActivity] with(rowlock)
                                           set   AuditStatus=@newAuditStatus,
                                                 Is_UnShelve=0,
                                                 LastUpdateDateTime=getdate(),
                                                 LastUpdateUserName=@userName
                                          where  ActivityId=@activityId and Is_Deleted=0;";
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                dbHelper.BeginTransaction();
                try
                {
                    using (var cmd = dbHelper.CreateCommand())
                    {
                        if (pidList?.Count > 0)//设置部分商品图片
                        {
                            SqlProduct = @"update p
                                            set   p.ImageUrl=@imgUrl,
                                                  p.LastUpdateDateTime=getdate()
                                            from  [Activity].[dbo].[SalePromotionActivityProduct] p with(rowlock)
                                                  JOIN Activity..SplitString(@pidList, ',', 1) AS B ON p.pid = B.Item
                                           where  p.ActivityId=@activityId and p.Is_Deleted=0;";
                            cmd.Parameters.Add(new SqlParameter("@pidList", string.Join(",", pidList)));
                        }
                        else//设置活动所有商品图片
                        {
                            SqlProduct = @"update  [Activity].[dbo].[SalePromotionActivityProduct] with(rowlock)
                                             set   ImageUrl=@imgUrl
                                            where  ActivityId=@activityId and Is_Deleted=0;";
                        }
                        //1.修改商品图片url
                        cmd.CommandText = SqlProduct;
                        cmd.Parameters.Add(new SqlParameter("@imgUrl", imgUrl));
                        cmd.Parameters.Add(new SqlParameter("@activityId", activityId));
                        var dbresult = await DbHelper.ExecuteNonQueryAsync(cmd);
                        int.TryParse(dbresult.ToString(), out result);
                        if (result > 0)
                        {
                            //2.修改活动的审核状态
                            cmd.CommandText = sqlUpdateActivity;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter("@activityId", activityId));
                            cmd.Parameters.Add(new SqlParameter("@newAuditStatus", newAuditStatus));
                            cmd.Parameters.Add(new SqlParameter("@userName", userName));
                            if (await DbHelper.ExecuteNonQueryAsync(cmd) == 1)
                            {
                                transResult = true;
                            }
                        }
                    }
                    if (transResult)
                    {
                        dbHelper.Commit();
                        await SetDBFlagCache(activityId);
                    }
                    else
                    {
                        dbHelper.Rollback();
                        Logger.Warn($"SetProductImageAsync修改活动牛皮癣失败,活动id{activityId},pid:{string.Join(",", pidList)}");
                    }
                }
                catch (Exception ex)
                {
                    dbHelper.Rollback();
                    Logger.Error($"SetProductImageAsync异常，{ex}");
                }
            }
            return result;
        }

        /// <summary>
        /// 从活动中移除商品
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<int> DeleteProductFromActivity(string pid, string activityId, string userName)
        {
            int result = 0;
            bool transResult = false;
            int newAuditStatus = 0;
            int oldAuditStatus = await GetActivityAuditStatus(activityId);
            if (oldAuditStatus > 0)
            {
                newAuditStatus = 1;
            }
            string sqlDelProduct = @"update [Activity].[dbo].[SalePromotionActivityProduct] with(rowlock)
                                      set   Is_Deleted=1,
                                            LastUpdateDateTime=getdate()
                                     where  Is_Deleted=0 and  ActivityId=@activityId and Pid=@pid ";
            string sqlUpdateActivity = @"update [Activity].[dbo].[SalePromotionActivity] with(rowlock)
                                           set   AuditStatus=@newAuditStatus,
                                                 Is_UnShelve=0,
                                                 LastUpdateDateTime=getdate(),
                                                 LastUpdateUserName=@userName
                                          where  ActivityId=@activityId and Is_Deleted=0";
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                dbHelper.BeginTransaction();
                try
                {
                    using (var cmd = dbHelper.CreateCommand())
                    {
                        //1.移除商品
                        cmd.CommandText = sqlDelProduct;
                        cmd.Parameters.Add(new SqlParameter("@pid", pid));
                        cmd.Parameters.Add(new SqlParameter("@activityId", activityId));
                        int.TryParse((await DbHelper.ExecuteNonQueryAsync(cmd)).ToString(), out result);
                        if (result > 0)
                        {
                            //2.修改活动的审核状态
                            cmd.CommandText = sqlUpdateActivity;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter("@activityId", activityId));
                            cmd.Parameters.Add(new SqlParameter("@newAuditStatus", newAuditStatus));
                            cmd.Parameters.Add(new SqlParameter("@userName", userName));
                            if (await DbHelper.ExecuteNonQueryAsync(cmd) == 1)
                            {
                                transResult = true;
                            }
                        }
                    }
                    if (transResult)
                    {
                        dbHelper.Commit();
                        await SetDBFlagCache(activityId);
                    }
                    else
                    {
                        dbHelper.Rollback();
                        Logger.Warn($"DeleteProductFromActivity从活动中移除商品失败,活动id{activityId},pid:{pid}");
                    }
                }
                catch (Exception ex)
                {
                    dbHelper.Rollback();
                    Logger.Error($"DeleteProductFromActivity异常，{ex}");
                }
            }
            return result;
        }

        /// <summary>
        /// 同步商品信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="productList"></param>
        /// <returns></returns>
        public static async Task<bool> RefreshProductAsync(string activityId, List<SalePromotionActivityProduct> productList)
        {
            if (!(productList?.Count > 0))
            {
                return true;
            }
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
                            if (productList?.Count > 0)
                            {
                                var productTmp = productList.Select(item => new
                                {
                                    Pid = item.Pid,
                                    ProductName = item.ProductName,
                                    TotalStock = item.TotalStock,
                                    CostPrice = item.CostPrice,
                                    SalePrice = item.SalePrice,
                                });
                                DataTable productDT = ToDataTable(productTmp);
                                cmd.CommandText = @"CREATE TABLE #productTemp([Pid] [nvarchar](100) NOT NULL,
	                                                                          [ProductName] [nvarchar](100) NULL,
	                                                                          [TotalStock] [int] NULL,
	                                                                          [CostPrice] [decimal](18, 2) NULL,
	                                                                          [SalePrice] [decimal](18, 2) NULL);";
                                await cmd.ExecuteNonQueryAsync();
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, tran))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "#productTemp";
                                    bulkcopy.WriteToServer(productDT);
                                    bulkcopy.Close();
                                }
                                string sqlUpdateProduct = @"MERGE INTO
                                                [Activity].[dbo].[SalePromotionActivityProduct] WITH ( ROWLOCK ) AS p
                                            USING #productTemp AS tempP
                                            ON p.Pid = tempP.Pid
                                            WHEN MATCHED THEN
                                                UPDATE SET p.ProductName=tempP.ProductName,
                                                           p.TotalStock=tempP.TotalStock,
                                                           p.CostPrice=tempP.CostPrice,
                                                           p.SalePrice=tempP.SalePrice;";
                                cmd.CommandText = sqlUpdateProduct;
                                await cmd.ExecuteNonQueryAsync();
                            }
                            if (result)
                            {
                                tran.Commit();
                                var keys = productList.Select(p => p.Pid)?.ToList();
                                keys.Add(activityId);
                                DBReadOnlyFlagHelper.SetDBReadOnlyFlagCache(SalePromotionActivityConst.SalePromotionActivityAndProduct, keys.ToList());
                            }
                            else
                            {
                                tran.Rollback();
                                Logger.Warn($"同步活动商品信息失败,活动id{activityId}");
                            }
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            tran.Rollback();
                            Logger.Error($"RefreshProductAsync异常，{ex}");
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 新增和删除活动商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="stock"></param>
        /// <param name="addList"></param>
        /// <param name="delList"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static async Task<bool> AddAndDelActivityProductAsync(string activityId, int stock, List<SalePromotionActivityProduct> addList, List<string> delList, string userName)
        {
            bool tansResult = true;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
            {
                conn.Open();
                int newAuditStatus = 0;
                int oldAuditStatus = await GetActivityAuditStatus(activityId);
                if (oldAuditStatus > 0)
                {
                    newAuditStatus = 1;
                }
                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = new SqlCommand("", conn, tran))
                    {
                        try
                        {
                            #region 新增商品
                            if (addList?.Count > 0)
                            {
                                var productTmp = addList.Select(item => new
                                {
                                    ActivityId = activityId,
                                    Pid = item.Pid,
                                    ProductName = item.ProductName,
                                    TotalStock = item.TotalStock,
                                    CostPrice = item.CostPrice,
                                    SalePrice = item.SalePrice,
                                    LimitQuantity = item.LimitQuantity,
                                    SoldQuantity = item.SoldQuantity,
                                    CreateDateTime = DateTime.Now,
                                    Is_UnShelve = 0,
                                    Is_Deleted = 0
                                });
                                DataTable productDT = ToDataTable(productTmp);
                                cmd.CommandText = @"CREATE TABLE #productTemp([ActivityId] [nvarchar](100) NOT NULL,
	                                                                          [Pid] [nvarchar](100) NOT NULL,
	                                                                          [ProductName] [nvarchar](100) NULL,
	                                                                          [TotalStock] [int] NULL,
	                                                                          [CostPrice] [decimal](18, 2) NULL,
	                                                                          [SalePrice] [decimal](18, 2) NULL,
	                                                                          [LimitQuantity] [int] NOT NULL,
	                                                                          [SoldQuantity] [int] NOT NULL,
	                                                                          [CreateDateTime] [datetime] NOT NULL,
	                                                                          [Is_UnShelve] [bit] NOT NULL,
	                                                                          [Is_Deleted] [bit] NOT NULL);";
                                await cmd.ExecuteNonQueryAsync();
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, tran))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "#productTemp";
                                    bulkcopy.WriteToServer(productDT);
                                    bulkcopy.Close();
                                }
                                //存在就找回原库存
                                string sqlAdd = @"MERGE INTO  Activity.dbo.SalePromotionActivityProduct WITH ( ROWLOCK ) AS ap
                                            USING #productTemp AS temp
                                            ON ap.Pid = temp.Pid and ap.ActivityId=@activityId
                                            WHEN MATCHED THEN
                                                UPDATE SET ap.Is_Deleted=0,
                                                           ap.ProductName=temp.ProductName,
                                                           ap.CostPrice=temp.CostPrice,
                                                           ap.TotalStock=temp.TotalStock,
                                                           ap.SalePrice=temp.SalePrice,
                                                           ap.ImageUrl='',
                                                           ap.LimitQuantity=@stock,
                                                           ap.LastUpdateDateTime = GETDATE()
                                            WHEN NOT MATCHED THEN
                                                INSERT (ActivityId,Pid,ProductName,CostPrice,TotalStock,SalePrice,
                                                        LimitQuantity,SoldQuantity,CreateDateTime,Is_UnShelve,Is_Deleted)
                                                VALUES ( temp.ActivityId,temp.Pid,temp.ProductName,
                                                         temp.CostPrice,temp.TotalStock,temp.SalePrice,
                                                         temp.LimitQuantity,0,GETDATE(),0,0);";
                                cmd.CommandText = sqlAdd;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(new SqlParameter("@activityId", activityId));
                                cmd.Parameters.Add(new SqlParameter("@stock", stock));
                                if ((await cmd.ExecuteNonQueryAsync()) != addList.Count)
                                {
                                    tansResult = false;
                                }
                            }
                            #endregion

                            #region 删除商品
                            if (delList?.Count > 0 && tansResult)
                            {
                                string sqlDel = @"update  ap
                                                    set   ap.Is_Deleted=1,
                                                          ap.LastUpdateDateTime=getdate()
                                                    from  Activity.dbo.SalePromotionActivityProduct ap WITH (ROWLOCK)
                                                    join  Activity..SplitString(@delList, ',', 1) AS B
                                                          ON ap.pid = B.Item
                                                   where  ap.ActivityId=@activityId";
                                cmd.CommandText = sqlDel;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(new SqlParameter("@delList", string.Join(",", delList)));
                                cmd.Parameters.Add(new SqlParameter("@activityId", activityId));
                                await cmd.ExecuteNonQueryAsync();
                            }
                            #endregion

                            #region 更新活动 重置审核状态
                            if (tansResult)
                            {
                                string sqlUpdate = @"update [Activity].[dbo].[SalePromotionActivity] with(rowlock)
                                                     set    AuditStatus=@newAuditStatus,
                                                            Is_UnShelve=0,
                                                            LastUpdateDateTime=getdate(),
                                                            LastUpdateUserName=@LastUpdateUserName
                                                     where  ActivityId=@activityId and Is_Deleted=0";
                                cmd.CommandText = sqlUpdate;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add(new SqlParameter("@activityId", activityId));
                                cmd.Parameters.Add(new SqlParameter("@newAuditStatus", newAuditStatus));
                                cmd.Parameters.Add(new SqlParameter("@LastUpdateUserName", userName));
                                if ((await cmd.ExecuteNonQueryAsync()) != 1)
                                {
                                    tansResult = false;
                                }
                            }
                            #endregion
                            if (tansResult)
                            {
                                tran.Commit();
                                await SetDBFlagCache(activityId);
                            }
                            else
                            {
                                tran.Rollback();
                                Logger.Warn($"新增和删除活动商品失败,活动id{activityId}，" +
                                    $"新增的pid：{string.Join(",", addList.Select(p => p.Pid))}" +
                                    $"删除的pid:{string.Join(",", delList)}");
                            }
                        }
                        catch (Exception ex)
                        {
                            tansResult = false;
                            tran.Rollback();
                            Logger.Error($"AddAndDelActivityProductAsync异常，{ex}");
                        }
                    }
                }
            }
            return tansResult;
        }

        #endregion

        #region 商品查询

        /// <summary>
        /// 获取活动下商品数量
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<int> GetActivityProductCountAsync(string activityId)
        {
            int count;
            string sql = @"select count(*)
                            from  [Activity].[dbo].[SalePromotionActivityProduct] with(nolock)
                           where  ActivityId=@activityId and Is_Deleted=0";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@activityId", activityId);
                int.TryParse((await DbHelper.ExecuteScalarAsync(false, cmd)).ToString(), out count);
                return count;
            }
        }

        /// <summary>
        /// 获取活动商品的信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SalePromotionActivityProduct>> GetProductInfoListAsync(string activityId, List<string> pidList)
        {
            string sql = string.Empty;
            using (var cmd = new SqlCommand())
            {
                if (pidList?.Count > 0)
                {
                    sql = @" select p.Pid,p.ProductName,p.LimitQuantity,p.TotalStock
                              from  [Activity].[dbo].[SalePromotionActivityProduct] p with(nolock)
                              join  Activity..SplitString(@pidList, ',', 1) AS B
                                    ON p.pid = B.Item
                             where  p.Is_Deleted=0 and p.ActivityId=@activityId ;";
                    cmd.Parameters.AddWithValue("@pidList", string.Join(",", pidList));
                }
                else
                {
                    sql = @"select Pid,ProductName,LimitQuantity,TotalStock
                             from  [Activity].[dbo].[SalePromotionActivityProduct] with(nolock)
                             where ActivityId=@activityId and Is_Deleted=0";
                }
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@activityId", activityId);
                return await DbHelper.ExecuteSelectAsync<SalePromotionActivityProduct>(false, cmd);
            }
        }

        /// <summary>
        /// 分页查询活动商品信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<PagedModel<SalePromotionActivityProduct>> SelectProductListAsync(SelectActivityProduct condition, int pageIndex = 1, int pageSize = 20)
        {
            if (condition == null)
            {
                return new PagedModel<SalePromotionActivityProduct> { Pager = new PagerModel() { Total = 0 } };
            }
            var pageResult = new PagedModel<SalePromotionActivityProduct>();
            string sqlSelect = @"select distinct p.Pid,p.TotalStock,p.ProductName,p.CostPrice,p.ImageUrl,
                                        p.SalePrice,p.LimitQuantity,p.SoldQuantity,p.CreateDateTime,p.LastUpdateDateTime ";
            string sqlSelectCount = @" select  count(distinct p.Pid) ";
            string sqlFrom = @" from  [Activity].[dbo].[SalePromotionActivityProduct] p with(nolock)";
            string sqlWhere = @" where p.ActivityId=@activityId";
            string sqlOrder = @" order by p.CreateDateTime desc ";
            string sqlPager = "";
            using (var cmd = new SqlCommand())
            {
                try
                {
                    #region 查询条件
                    if (!string.IsNullOrWhiteSpace(condition.Pid))
                    {
                        sqlWhere += " and p.Pid=@Pid";
                        cmd.Parameters.AddWithValue("@Pid", condition.Pid);
                    }
                    if (!string.IsNullOrWhiteSpace(condition.ProductName))
                    {
                        sqlWhere += " and p.ProductName like @ProductName";
                        cmd.Parameters.AddWithValue("@ProductName", $"%{condition.ProductName}%");
                    }
                    if (condition.LowProfile != null || condition.HighProfile != null)
                    {
                        sqlFrom += @" join [Activity].[dbo].[SalePromotionActivity] a with(nolock)
                                        on p.ActivityId=a.ActivityId and a.Is_Deleted=0
                                     join [Activity].[dbo].[SalePromotionActivityDiscount] d with(nolock)
                                         on a.ActivityId=d.ActivityId and d.Is_Deleted=0 ";
                        if (condition.LowProfile != null)
                        {
                            sqlWhere += " and @LowProfile<=(p.SalePrice*d.DiscountRate/100-p.CostPrice)";
                            cmd.Parameters.AddWithValue("@LowProfile", condition.LowProfile);
                        }
                        if (condition.HighProfile != null)
                        {
                            sqlWhere += " and @HighProfile>=(p.SalePrice*d.DiscountRate/100-p.CostPrice)";
                            cmd.Parameters.AddWithValue("@HighProfile", condition.HighProfile);
                        }
                    }
                    if (condition.IsDeleted)
                    {
                        sqlWhere += " and p.Is_Deleted = 1 ";
                    }
                    else
                    {
                        sqlWhere += " and p.Is_Deleted = 0 ";
                    }
                    if (!string.IsNullOrWhiteSpace(condition.AfterCreateDateTime))
                    {
                        sqlWhere += " and p.CreateDateTime>@AfterCreateDateTime";
                        cmd.Parameters.AddWithValue("@AfterCreateDateTime", condition.AfterCreateDateTime);
                    }
                    if (!string.IsNullOrWhiteSpace(condition.AfterLastUpdateDateTime))
                    {
                        sqlWhere += " and p.LastUpdateDateTime>@AfterLastUpdateDateTime";
                        cmd.Parameters.AddWithValue("@AfterLastUpdateDateTime", condition.AfterLastUpdateDateTime);
                    }
                    if (pageIndex > 0 && pageSize > 0)
                    {
                        sqlPager = @" OFFSET (@pageIndex -1) * @pageSize ROWS
                              FETCH NEXT @pageSize ROWS ONLY";
                        cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    }
                    #endregion
                    cmd.Parameters.AddWithValue("@activityId", condition.ActivityId);
                    string sqlCount = sqlSelectCount + sqlFrom + sqlWhere;
                    string sqlSource = sqlSelect + sqlFrom + sqlWhere + sqlOrder + sqlPager;
                    cmd.CommandText = sqlCount;
                    int count;
                    int.TryParse((await DbHelper.ExecuteScalarAsync(false, cmd)).ToString(), out count);
                    pageResult.Pager = new PagerModel() { Total = count };
                    if (count > 0)
                    {
                        cmd.CommandText = sqlSource;
                        pageResult.Source = await DbHelper.ExecuteSelectAsync<SalePromotionActivityProduct>(false, cmd);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"SelectProductListAsync异常，{ex}");
                }
                return pageResult;
            }
        }

        /// <summary>
        /// 获取当前时间已经存在其他活动中的商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public static async Task<IList<SalePromotionActivityProduct>> GetRepeatProductListAsync(string activityId, List<string> pidList)
        {
            var list = new List<SalePromotionActivityProduct>();
            string sqlGetTime = @"select StartTime,EndTime
                                   from  [Activity].[dbo].[SalePromotionActivity] with(nolock)
                                  where  ActivityId=@activityId";
            string sql = @" select p.Pid,a.ActivityId,a.Name as ActivityName
                             from  [Activity].[dbo].[SalePromotionActivityProduct] p with(nolock)
                             join  [Activity].[dbo].[SalePromotionActivity] a with(nolock)
                                   on p.ActivityId=a.ActivityId and a.Is_UnShelve=0 and a.Is_Deleted=0
                             join  Activity..SplitString(@pidList, ',', 1) AS B
                                   ON p.pid = B.Item
                            where  p.Is_Deleted=0 and p.ActivityId<>@activityId
                                   and not(@startTime>a.EndTime or @endTime<a.StartTime);";
            using (var cmd = new SqlCommand(sqlGetTime))
            {
                cmd.Parameters.AddWithValue("@activityId", activityId);
                //获取当前活动时间
                var activityModel = await DbHelper.ExecuteFetchAsync<SalePromotionActivityModel>(false, cmd);
                try
                {
                    if (activityModel != null)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@activityId", activityId);
                        cmd.Parameters.AddWithValue("@pidList", string.Join(",", pidList));
                        cmd.Parameters.AddWithValue("@startTime", activityModel.StartTime);
                        cmd.Parameters.AddWithValue("@endTime", activityModel.EndTime);
                        list = (await DbHelper.ExecuteSelectAsync<SalePromotionActivityProduct>(false, cmd)).ToList();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"GetRepeatProductListAsync异常，{ex}");
                }
            }
            return list;
        }

        /// <summary>
        /// 获取特定时间内当前活动和其他活动重复的商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<IList<SalePromotionActivityProduct>> GetActivityRepeatProductListAsync(string activityId, string startTime, string endTime)
        {
            var list = new List<SalePromotionActivityProduct>();
            string sql = @" select p.Pid,a.ActivityId,p.ProductName,a.Name as ActivityName
                             from  [Activity].[dbo].[SalePromotionActivityProduct] p with(nolock)
                             join  [Activity].[dbo].[SalePromotionActivity] a with(nolock)
                                   on p.ActivityId=a.ActivityId and a.Is_UnShelve=0 and a.Is_Deleted=0
                             join  [Activity].[dbo].[SalePromotionActivityProduct] p1 with(nolock)
                                   ON p1.Pid = p.Pid and p1.ActivityId=@activityId and p1.Is_Deleted=0
                            where  p.Is_Deleted=0 and p.ActivityId<>@activityId
                                   and not(@startTime>a.EndTime or @endTime<a.StartTime);";
            using (var cmd = new SqlCommand(sql))
            {
                try
                {
                    cmd.Parameters.AddWithValue("@activityId", activityId);
                    cmd.Parameters.AddWithValue("@startTime", startTime);
                    cmd.Parameters.AddWithValue("@endTime", endTime);
                    list = (await DbHelper.ExecuteSelectAsync<SalePromotionActivityProduct>(false, cmd)).ToList();
                }
                catch (Exception ex)
                {
                    Logger.Error($"GetActivityRepeatProductListAsync异常，{ex}");
                }
            }
            return list;
        }

        /// <summary>
        ///获取商品信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList">pid集合</param>
        /// <returns></returns>
        public static async Task<List<SalePromotionActivityProductDB>> GetProductListByActivityIdAsync(string activityId)
        {
            var list = new List<SalePromotionActivityProductDB>();
            string sql = @" select ActivityId,Pid,LimitQuantity,SalePrice
                             from  [Activity].[dbo].[SalePromotionActivityProduct] with(nolock)
                            where  Is_Deleted=0 and ActivityId=@activityId ;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@activityId", activityId);
                list = (await DbHelper.ExecuteSelectAsync<SalePromotionActivityProductDB>(false, cmd)).ToList();
                return list;
            }
        }

        #endregion

        /// <summary>
        /// 获取活动的审核状态
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        private static async Task<int> GetActivityAuditStatus(string activityId)
        {
            int auditStatus = 0;
            string sql = @"select AuditStatus
                            from  [Activity].[dbo].[SalePromotionActivity] with(nolock)
                           where  ActivityId=@activityId and Is_Deleted=0";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@activityId", activityId);
                int.TryParse((await DbHelper.ExecuteScalarAsync(false, cmd)).ToString(), out auditStatus);
                return auditStatus;
            }
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }

        /// <summary>
        /// 设置读写库标识
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        private static async Task<bool> SetDBFlagCache(string activityId)
        {
            var keys = new List<string>() { activityId };
            var pidList = await GetProductInfoListAsync(activityId, null);
            foreach (var item in pidList)
            {
                keys.Add(item.Pid);
            }
            DBReadOnlyFlagHelper.SetDBReadOnlyFlagCache(SalePromotionActivityConst.SalePromotionActivityAndProduct, keys.ToList());
            return true;
        }


        #region 审核权限

        /// <summary>
        /// 新增审核权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<int> InsertAuditAuthAsync(SalePromotionAuditAuth model)
        {
            int result = 0;
            string sql = @"INSERT INTO [Activity].[dbo].[SalePromotionAuditAuth]
                                       ([AuthId]
                                       ,[UserName]
                                       ,[RoleType]
                                       ,[PromotionType]
                                       ,[CreateDateTime]
                                       ,[CreateUserName]
                                       ,[Is_Deleted])
                                 VALUES
                                       (@AuthId
                                       ,@UserName
                                       ,@RoleType
                                       ,@PromotionType
                                       ,getdate()
                                       ,@CreateUserName
                                       ,0)";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@AuthId", model.AuthId);
                cmd.Parameters.AddWithValue("@UserName", model.UserName);
                cmd.Parameters.AddWithValue("@RoleType", model.RoleType);
                cmd.Parameters.AddWithValue("@PromotionType", model.PromotionType);
                cmd.Parameters.AddWithValue("@CreateUserName", model.CreateUserName);
                result = await DbHelper.ExecuteNonQueryAsync(cmd);
            }
            return result;
        }

        /// <summary>
        /// 删除审核权限
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static async Task<int> DeleteAuditAuthAsync(int PKID)
        {
            int result = 0;
            string sql = @"UPDATE [Activity].[dbo].[SalePromotionAuditAuth] WITH(ROWLOCK)
                            SET   Is_Deleted=1
                            WHERE PKID=@PKID AND Is_Deleted=0";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKID", PKID);
                result = await DbHelper.ExecuteNonQueryAsync(cmd);
            }
            return result;
        }

        /// <summary>
        /// 根据类型和username获取用户审核权限信息
        /// </summary>
        /// <param name="promotionType"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static async Task<SalePromotionAuditAuth> GetUserAuditAuthAsync(int promotionType, string userName)
        {
            var result = new SalePromotionAuditAuth();
            string sql = @"SELECT  [PKID]
                                  ,[AuthId]
                                  ,[UserName]
                                  ,[RoleType]
                                  ,[PromotionType]
                                  ,[CreateDateTime]
                                  ,[CreateUserName]
                                  ,[LastUpdateDateTime]
                                  ,[LastUpdateUserName]
                                  ,[Is_Deleted]
                            FROM   [Activity].[dbo].[SalePromotionAuditAuth] WITH(NOLOCK)
                            WHERE  PromotionType=@PromotionType
                                    AND UserName=@UserName
                                    AND Is_Deleted=0";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PromotionType", promotionType);
                cmd.Parameters.AddWithValue("@UserName", userName);
                result = await DbHelper.ExecuteFetchAsync<SalePromotionAuditAuth>(cmd);
            }
            return result;
        }

        /// <summary>
        /// 分页查询用户审核权限
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<PagedModel<SalePromotionAuditAuth>> SelectAuditAuthListAsync(SalePromotionAuditAuth searchModel, int pageIndex, int pageSize)
        {
            var pagedModel = new PagedModel<SalePromotionAuditAuth>()
            {
                Pager = new PagerModel()
            };
            string sqlSelectSource = @"SELECT  [PKID],[AuthId]
                                  ,[UserName]
                                  ,[RoleType]
                                  ,[PromotionType]
                                  ,[CreateDateTime]
                                  ,[CreateUserName]
                                  ,[LastUpdateDateTime]
                                  ,[LastUpdateUserName]
                                  ,[Is_Deleted]
                            FROM   [Activity].[dbo].[SalePromotionAuditAuth] WITH(NOLOCK)";
            string sqlSelectCount = @"SELECT  count(*)
                            FROM   [Activity].[dbo].[SalePromotionAuditAuth] WITH(NOLOCK)";
            string sqlWhere = @" WHERE Is_Deleted=0";
            string sqlOffset = @"   ORDER BY CreateDateTime DESC 
                              OFFSET (@pageIndex -1) * @pageSize ROWS
                              FETCH NEXT @pageSize ROWS ONLY ";
            using (var cmd = new SqlCommand())
            {
                if (searchModel.PromotionType > 0)
                {
                    sqlWhere += " AND PromotionType=@PromotionType ";
                    cmd.Parameters.AddWithValue("@PromotionType", searchModel.PromotionType);
                }
                if (!string.IsNullOrWhiteSpace(searchModel.UserName))
                {
                    sqlWhere += " AND UserName=@UserName ";
                    cmd.Parameters.AddWithValue("@UserName", searchModel.UserName);
                }
                cmd.CommandText = sqlSelectCount + sqlWhere;
                cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                int count = 0;
                int.TryParse((await DbHelper.ExecuteScalarAsync(cmd)).ToString(), out count);
                pagedModel.Pager.Total = count;
                if (count > 0)
                {
                    cmd.CommandText = sqlSelectSource + sqlWhere + sqlOffset;
                    pagedModel.Source = await DbHelper.ExecuteSelectAsync<SalePromotionAuditAuth>(cmd);
                }
                if (pagedModel.Source == null)
                {
                    pagedModel.Source = new List<SalePromotionAuditAuth>();
                }
            }
            return pagedModel;
        }

        #endregion

    }
}
