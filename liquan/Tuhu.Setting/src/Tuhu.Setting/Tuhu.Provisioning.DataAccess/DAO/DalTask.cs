using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalTask
    {
        public static List<TaskModel> GetTaskListList()
        {
            const string sqlStr = @"SELECT  TaskName ,
        TaskId ,
        TaskRule ,
        TaskLink ,
        TaskType ,
        TaskIcon ,
        DisplayStatus ,
        CreateDateTime ,
        Duration ,
        Operator ,
        Sequence
FROM    Configuration..TaskConfigInfo WITH ( NOLOCK )
WHERE   IsDelete=0
ORDER BY DisplayStatus DESC ,
        TaskType ASC ,
        Sequence ASC,
        CreateDateTime DESC;";
            return DbHelper.ExecuteDataTable(sqlStr, CommandType.Text)?.ConvertTo<TaskModel>().ToList() ?? new List<TaskModel>();
        }


        public static List<TaskActionModel> GetActionList()
        {
            const string sqlStr = @"
SELECT DISTINCT
        ActionName,Remark
FROM    Configuration..TaskActionInfo WITH ( NOLOCK )
WHERE   isdelete = 0;";
            return DbHelper.ExecuteDataTable(sqlStr, CommandType.Text)?.ConvertTo<TaskActionModel>().ToList() ?? new List<TaskActionModel>();

        }

        public static TaskConfigModel GetTaskInfo(Guid taskId)
        {
            const string sqlStr = @"
select T.TaskName,
       T.TaskId,
       T.TaskRule,
       T.TaskLink,
       T.TaskType,
       T.TaskIcon,
       T.DisplayStatus,
       T.CreateDateTime,
       T.Duration,
       T.Operator,
       T.TaskText,
       ISNULL(T.TriggerTime, T.CreateDateTime) as TriggerTime,
       T.AwardText,
       T.AwardLink,
       T.AwardImg,
       T.Integral,
       T.TireInsurance,
       T.RandomCoupon,
       T.ButtonText,
       T.Sequence,
       T.StartVersion,
       T.EndVersion,
       T.RequireActionCount,
       T.RecommendImg,
       T.RecommendLevel,
       T.PopulationsTag,
       T.TaskChannel,
       T.WXAwardAppId,
       T.WXAwardLink,
       T.WXTaskAppId,
       T.WXTaskLink
from Configuration..TaskConfigInfo as T with (nolock)
where T.TaskId = @taskId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@taskId", taskId);
                return DbHelper.ExecuteDataTable(cmd)?.ConvertTo<TaskConfigModel>()?.FirstOrDefault();
            }
        }

        public static List<TaskConditionModel> GetConditionList(Guid taskId)
        {
            const string sqlStr = @"SELECT  T.ActionName, T.Count, T.SpecialPara, S.Remark
FROM    Configuration..TaskConditionInfo AS T WITH ( NOLOCK )
        LEFT JOIN Configuration..TaskActionInfo AS S WITH ( NOLOCK ) ON T.ActionName = S.ActionName
WHERE   TaskId = @taskId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@taskId", taskId);
                return DbHelper.ExecuteDataTable(cmd)?.ConvertTo<TaskConditionModel>()?.ToList() ?? new List<TaskConditionModel>();
            }
        }
        public static List<CouponInfoModel> GetCouponList(Guid taskId)
        {
            const string sqlStr = @"SELECT  CouponId AS GetRuleId ,
        Count
FROM    Configuration..TaskCouponInfo WITH ( NOLOCK )
WHERE   TaskId = @taskId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@taskId", taskId);
                return DbHelper.ExecuteDataTable(cmd)?.ConvertTo<CouponInfoModel>()?.ToList() ?? new List<CouponInfoModel>();
            }
        }

        public static List<TriggerTaskModel> GetTriggerTaskList(Guid TaskId)
        {
            const string sqlStr = @"SELECT  TriggerTaskId, TriggerTaskName, TriggerType, IsTimeout
FROM    Configuration..TaskTriggerInfo WITH ( NOLOCK )
WHERE   IsDelete = 0
        AND TaskId = @taskId;";
            using(var cmd=new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@taskId", TaskId);
                return DbHelper.ExecuteDataTable(cmd).ConvertTo<TriggerTaskModel>()?.ToList() ?? new List<TriggerTaskModel>();
            }
        }

        public static bool AddTaskConfig(TaskConfigModel request, string Operator, BaseDbHelper dbHelper)
        {
            const string sqlStr = @"
insert into Configuration..TaskConfigInfo
(
    TaskId,
    TaskName,
    TaskType,
    TaskIcon,
    DisplayStatus,
    TaskLink,
    TaskText,
    TriggerTime,
    Duration,
    AwardText,
    AwardLink,
    ButtonText,
    AwardImg,
    Integral,
    TaskRule,
    TireInsurance,
    RandomCoupon,
    Operator,
    IsDelete,
    Sequence,
    PopulationsTag,
    StartVersion,
    EndVersion,
    RequireActionCount,
    RecommendImg,
    RecommendLevel,
    TaskChannel,
    WXAwardAppId,
    WXAwardLink,
    WXTaskAppId,
    WXTaskLink
)
values
(@taskid, @taskname, @tasktype, @taskicon, @displaystatus, @tasklink, @tasktext, @triggertime, @duration, @awardtext,
 @awardlink, '', @awardimg, @integral, @taskrule, @tireinsurance, @randomcoupon, @operator, 0, @sequence,
 @populationsTag, @startVersion, @endVersion, @requireActionCount, @recommendImg, @recommendLevel, @taskChannel,
 @wxAwardAppId, @wxAwardLink, @wxTaskAppId, @wxTaskLink);";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@taskid", request.TaskId);
                cmd.Parameters.AddWithValue("@taskname", request.TaskName);
                cmd.Parameters.AddWithValue("@tasktype", request.TaskType);
                cmd.Parameters.AddWithValue("@taskicon", request.TaskIcon);
                cmd.Parameters.AddWithValue("@displaystatus", request.DisplayStatus);
                cmd.Parameters.AddWithValue("@tasklink", request.TaskLink);
                cmd.Parameters.AddWithValue("@tasktext", request.TaskText);
                cmd.Parameters.AddWithValue("@triggertime", request.TriggerTime);
                cmd.Parameters.AddWithValue("@duration", request.Duration);
                cmd.Parameters.AddWithValue("@awardtext", request.AwardText);
                cmd.Parameters.AddWithValue("@awardlink", request.AwardLink);
                cmd.Parameters.AddWithValue("@awardimg", request.AwardImg);
                cmd.Parameters.AddWithValue("@integral", request.Integral);
                cmd.Parameters.AddWithValue("@taskrule", request.TaskRule);
                cmd.Parameters.AddWithValue("@tireinsurance", request.TireInsurance);
                cmd.Parameters.AddWithValue("@randomcoupon", request.RandomCoupon);
                cmd.Parameters.AddWithValue("@operator", Operator);
                cmd.Parameters.AddWithValue("@sequence", request.Sequence);
                cmd.Parameters.AddWithValue("@populationsTag", request.PopulationsTag);
                cmd.Parameters.AddWithValue("@startVersion", request.StartVersion);
                cmd.Parameters.AddWithValue("@endVersion", request.EndVersion);
                cmd.Parameters.AddWithValue("@requireActionCount", request.RequireActionCount);
                cmd.Parameters.AddWithValue("@recommendImg", request.RecommendImg);
                cmd.Parameters.AddWithValue("@recommendLevel", request.RecommendLevel);
                cmd.Parameters.AddWithValue("@taskChannel", request.TaskChannel);
                cmd.Parameters.AddWithValue("@wxAwardAppId", request.WXAwardAppId);
                cmd.Parameters.AddWithValue("@wxAwardLink", request.WXAwardLink);
                cmd.Parameters.AddWithValue("@wxTaskAppId", request.WXTaskAppId);
                cmd.Parameters.AddWithValue("@wxTaskLink", request.WXTaskLink);
                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool AddCondition(Guid taskId, List<TaskConditionModel> conditionList, BaseDbHelper dbHelper)
        {
            const string sqlStr = @"INSERT  INTO Configuration..TaskConditionInfo
        ( TaskId ,
          ActionName ,
          Count ,
          SpecialPara ,
          Remark
        )
VALUES  ( @taskId ,
          @actionName ,
          @count ,
          @specialPara ,
          @remark
        );";
            foreach (var item in conditionList)
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@taskId", taskId);
                    cmd.Parameters.AddWithValue("@actionName", item.ActionName);
                    cmd.Parameters.AddWithValue("@specialPara", item.SpecialPara);
                    cmd.Parameters.AddWithValue("@count", item.Count);
                    cmd.Parameters.AddWithValue("@remark", default(string));
                    var result = dbHelper.ExecuteNonQuery(cmd);
                    if (result < 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool AddCoupon(Guid taskId, List<CouponInfoModel> couponList, BaseDbHelper dbHelper)
        {
            const string sqlStr = @"INSERT  INTO Configuration..TaskCouponInfo
        ( TaskId, CouponId, Count )
VALUES  ( @taskId, @couponId, @count );";
            foreach (var item in couponList)
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@taskId", taskId);
                    cmd.Parameters.AddWithValue("@couponId", item.GetRuleId);
                    cmd.Parameters.AddWithValue("@count", item.Count);
                    var result = dbHelper.ExecuteNonQuery(cmd);
                    if (result < 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool AddTriggerTask(Guid taskId, List<TriggerTaskModel> triggerList, BaseDbHelper dbHelper)
        {
            const string sqlStr = @"
INSERT  INTO Configuration..TaskTriggerInfo
        ( TaskId ,
          TriggerTaskId ,
          TriggerTaskName ,
          TriggerType ,
          IsTimeout
        )
VALUES  ( @taskId ,
          @triggerTaskId ,
          @triggerTaskName ,
          @triggerType ,
          @isTimeout
        );
";
            foreach (var item in triggerList)
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@taskId", taskId);
                    cmd.Parameters.AddWithValue("@triggerTaskId", item.TriggerTaskId);
                    cmd.Parameters.AddWithValue("@triggerTaskName", item.TriggerTaskName);
                    cmd.Parameters.AddWithValue("@triggerType", item.TriggerType);
                    cmd.Parameters.AddWithValue("@isTimeout", item.IsTimeout);
                    var result = dbHelper.ExecuteNonQuery(cmd);
                    if (result < 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static bool UpdateTriggerTask(Guid taskId, List<TriggerTaskModel> triggerList, BaseDbHelper dbHelper)
        {
            const string sqlStr = @"DELETE Configuration..TaskTriggerInfo WHERE TaskId = @taskid;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@taskid", taskId);
                var result = dbHelper.ExecuteNonQuery(cmd);
                return AddTriggerTask(taskId, triggerList, dbHelper);
            }
        }


        public static bool UpdateTaskConfig(TaskConfigModel request, BaseDbHelper dbHelper)
        {
            const string sqlStr = @"
update Configuration..TaskConfigInfo with (rowlock)
set TaskName = @taskName,
    TaskIcon = @taskicon,
    DisplayStatus = @displayStatus,
    TaskLink = @taskLink,
    TaskText = @taskText,
    Duration = @duration,
    AwardText = @awardText,
    AwardLink = @awardLink,
    AwardImg = @awardImg,
    Integral = @integral,
    TireInsurance = @tireInsurance,
    RandomCoupon = @randomCoupon,
    TaskRule = @taskrule,
    TaskType = @taskType,
    LastUpdateDateTime = GETDATE(),
    Sequence = @sequence,
    PopulationsTag = @populationsTag,
    StartVersion = @startVersion,
    EndVersion = @endVersion,
    RequireActionCount = @requireActionCount,
    RecommendImg = @recommendImg,
    RecommendLevel = @recommendLevel,
    TaskChannel = @taskChannel,
    WXAwardAppId = @wxAwardAppId,
    WXAwardLink = @wxAwardLink,
    WXTaskAppId = @wxTaskAppId,
    WXTaskLink = @wxTaskLink
where TaskId = @taskid;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@taskName", request.TaskName);
                cmd.Parameters.AddWithValue("@taskIcon", request.TaskIcon);
                cmd.Parameters.AddWithValue("@displayStatus", request.DisplayStatus);
                cmd.Parameters.AddWithValue("@taskLink", request.TaskLink);
                cmd.Parameters.AddWithValue("@taskText", request.TaskText);
                cmd.Parameters.AddWithValue("@duration", request.Duration);
                cmd.Parameters.AddWithValue("@awardText", request.AwardText);
                cmd.Parameters.AddWithValue("@awardLink", request.AwardLink);
                cmd.Parameters.AddWithValue("@awardImg", request.AwardImg);
                cmd.Parameters.AddWithValue("@integral", request.Integral);
                cmd.Parameters.AddWithValue("@tireInsurance", request.TireInsurance);
                cmd.Parameters.AddWithValue("@randomCoupon", request.RandomCoupon);
                cmd.Parameters.AddWithValue("@taskrule", request.TaskRule);
                cmd.Parameters.AddWithValue("@taskid", request.TaskId);
                cmd.Parameters.AddWithValue("@taskType", request.TaskType);
                cmd.Parameters.AddWithValue("@sequence", request.Sequence);
                cmd.Parameters.AddWithValue("@populationsTag", request.PopulationsTag);
                cmd.Parameters.AddWithValue("@startVersion", request.StartVersion);
                cmd.Parameters.AddWithValue("@endVersion", request.EndVersion);
                cmd.Parameters.AddWithValue("@requireActionCount", request.RequireActionCount);
                cmd.Parameters.AddWithValue("@recommendImg", request.RecommendImg);
                cmd.Parameters.AddWithValue("@recommendLevel", request.RecommendLevel);
                cmd.Parameters.AddWithValue("@taskChannel", request.TaskChannel);
                cmd.Parameters.AddWithValue("@wxAwardAppId", request.WXAwardAppId);
                cmd.Parameters.AddWithValue("@wxAwardLink", request.WXAwardLink);
                cmd.Parameters.AddWithValue("@wxTaskAppId", request.WXTaskAppId);
                cmd.Parameters.AddWithValue("@wxTaskLink", request.WXTaskLink);
                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }
        public static bool EditCoupon(Guid taskId, List<CouponInfoModel> couponList, BaseDbHelper dbHelper)
        {
            const string sqlStr = @"DELETE  Configuration..TaskCouponInfo WHERE   TaskId = @taskid;";
            const string sqlStr2 = @"INSERT  INTO Configuration..TaskCouponInfo(TaskId, CouponId, Count) VALUES (@taskId, @couponId, @count);";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@taskid", taskId);
                dbHelper.ExecuteNonQuery(cmd);
            }
            var coupons = couponList.Where(g => g.GetRuleId != Guid.Empty && g.Count > 0).ToList();
            foreach (var item in coupons)
            {
                using (var cmd = new SqlCommand(sqlStr2))
                {
                    cmd.Parameters.AddWithValue("@taskId", taskId);
                    cmd.Parameters.AddWithValue("@couponId", item.GetRuleId);
                    cmd.Parameters.AddWithValue("@count", item.Count);
                    var result = dbHelper.ExecuteNonQuery(cmd);
                    if (result < 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool DeleteCondition(Guid taskId, BaseDbHelper dbhelper)
        {
            const string sqlStr = @"DELETE Configuration..TaskConditionInfo WHERE TaskId=@taskId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@taskId", taskId);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static List<TaskSimpleModel> GetTaskList(Guid? TaskId)
        {
            const string sqlStr = @"SELECT  TaskId, TaskName
FROM    Configuration..TaskConfigInfo
WHERE   isdelete = 0
        AND ( @taskId IS NULL
              OR @taskId <> TaskId
            );";
            using(var cmd=new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@taskId", TaskId);
                return DbHelper.ExecuteDataTable(cmd).ConvertTo<TaskSimpleModel>()?.ToList() ?? new List<TaskSimpleModel>();
            }
        }
        /// <summary>
        /// 校验任务是否作为其他任务的子任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public static bool CheckTaskConfigHasCondition(Guid taskId)
        {
            const string sqlStr = @"SELECT TOP 1 1
FROM   Configuration..TaskConditionInfo WITH ( NOLOCK )
WHERE  SpecialPara = @taskId OR TaskId=@taskId";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@taskId", taskId.ToString());
                return DbHelper.ExecuteScalar(cmd) != null;
            }
        }

        /// <summary>
        /// 删除任务配置
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public static int DeleteTaskConfig(Guid taskId)
        {
            const string sqlStr = @"
UPDATE  Configuration..TaskConfigInfo WITH ( ROWLOCK )
SET     IsDelete = 1
WHERE   TaskId = @taskid;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@taskid", taskId);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
        public static bool TaskOprLog(Guid taskId, string Operator, string operate, string remark)
        {
            const string sqlStr = @"INSERT INTO tuhu_log..TaskOprLog(TaskId, Operator,Operate,Remark) VALUES(@taskId,@operator,@operate,@remark);";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@taskId", taskId);
                    cmd.Parameters.AddWithValue("@operator", Operator);
                    cmd.Parameters.AddWithValue("@operate", operate);
                    cmd.Parameters.AddWithValue("@remark", remark);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }
        }

        public static List<TaskOprLogModel> GetTaskOprLog(Guid taskId)
        {
            const string sqlStr = @"SELECT TaskId,Operator,Operate,CreateDateTime FROM tuhu_log..TaskOprLog WITH(NOLOCK) WHERE TaskId=@taskId ORDER BY CreateDateTime DESC;";
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@taskId", taskId);
                    return dbHelper.ExecuteDataTable(cmd).ConvertTo<TaskOprLogModel>()?.ToList() ?? new List<TaskOprLogModel>();
                }
            }
        }
        public static DataTable SelectProductCategory()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"SELECT  oid ,
        ParaentOid,
        ParentOid,
        CategoryName,
        DisplayName,
        Description,
        NodeNo
FROM    Tuhu_productcatalog..vw_ProductCategories WITH(NOLOCK);");
                cmd.CommandType = CommandType.Text;
                return dbhelper.ExecuteDataTable(cmd);
            }

        }
        public static List<OrdertaskRuleModel> GetRuleInfo()
        {
            const string sqlStr = @"SELECT  PKID AS RuleNo, RuleName, RuleType, RuleStatus, Creator, CreatedateTime,
        LastUpdateTime
FROM    Configuration..TaskOrderRuleId WITH ( NOLOCK )
WHERE   IsDelete = 0;";
            return DbHelper.ExecuteDataTable(sqlStr).ConvertTo<OrdertaskRuleModel>()?.ToList() ?? new List<OrdertaskRuleModel>();
        }
        public static bool AddOrderRule(OrderRuleDetailModel request,string creator)
        {
            const string sqlStr = @"
INSERT  INTO Configuration..TaskOrderRuleId ( RuleName, RuleStatus, RuleType,
                                              MatchType, ConditionType,
                                              CategoryList, Brand, PIDS,
                                              Creator )
VALUES  ( @ruleName, @ruleStatus, @ruleType, @matchType, @conditionType,
          @categoryList, @brand, @pids, @creator );";
            using(var cmd=new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@ruleName", request.RuleName);
                cmd.Parameters.AddWithValue("@ruleStatus", request.RuleStatus);
                cmd.Parameters.AddWithValue("@ruleType", request.RuleType);
                cmd.Parameters.AddWithValue("@matchType", request.MatchType);
                cmd.Parameters.AddWithValue("@conditionType", request.ConditionType);
                cmd.Parameters.AddWithValue("@categoryList", request.CategoryList);
                cmd.Parameters.AddWithValue("@brand", request.Brand);
                cmd.Parameters.AddWithValue("@pids", request.PIDS);
                cmd.Parameters.AddWithValue("@creator", creator);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool UpdateOrderRuleInfo(OrderRuleDetailModel request)
        {
            const string sqlStr = @"UPDATE  Configuration..TaskOrderRuleId WITH ( ROWLOCK )
SET     RuleName = @ruleName, RuleStatus = @ruleStatus, RuleType = @ruleType,
        MatchType = @matchType, ConditionType = @conditionType,
        CategoryList = @categoryList, Brand = @brand, PIDS = @pids
WHERE   PKID = @ruleno;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@ruleName", request.RuleName);
                cmd.Parameters.AddWithValue("@ruleStatus", request.RuleStatus);
                cmd.Parameters.AddWithValue("@ruleType", request.RuleType);
                cmd.Parameters.AddWithValue("@matchType", request.MatchType);
                cmd.Parameters.AddWithValue("@conditionType", request.ConditionType);
                cmd.Parameters.AddWithValue("@categoryList", request.CategoryList);
                cmd.Parameters.AddWithValue("@brand", request.Brand);
                cmd.Parameters.AddWithValue("@pids", request.PIDS);
                cmd.Parameters.AddWithValue("@ruleno", request.RuleNo);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static OrderRuleDetailModel FetchOrderRuleInfo(int ruleId)
        {
            const string sqlStr = @"SELECT  PKID AS RuleNo, RuleName, RuleStatus, MatchType, ConditionType,
        CategoryList, Brand, PIDS
FROM    Configuration..TaskOrderRuleId WITH ( NOLOCK )
WHERE   IsDelete = 0
        AND PKID = @ruleId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@ruleId", ruleId);
                return DbHelper.ExecuteDataTable(cmd).ConvertTo<OrderRuleDetailModel>()?.FirstOrDefault() ?? new OrderRuleDetailModel();
            }
        }
        public static OrderRuleProductModel FetchProductNameByPID(string PID)
        {
            const string sqlStr = @"SELECT  [PID], [DisplayName]
FROM    [Tuhu_productcatalog].[dbo].[vw_Products] WITH ( NOLOCK )
WHERE   PID = @PID
        AND OnSale = 1
        AND Stockout = 0;";
            using(var cmd=new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@PID", PID);
                return DbHelper.ExecuteDataTable(cmd).ConvertTo<OrderRuleProductModel>()?.FirstOrDefault();
            }
        }
    }
}