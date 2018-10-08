using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Member;

namespace Tuhu.Provisioning.Business
{
    public class TaskConfigManager
    {
        public static List<TaskModel> GetTaskListList() => DalTask.GetTaskListList();
        public static List<TaskActionModel> GetActionList() => DalTask.GetActionList();
        public static TaskConfigModel FetchTaskDetail(Guid taskId)
        {
            var result = DalTask.GetTaskInfo(taskId);
            var triggerData = DalTask.GetTriggerTaskList(taskId);
            if (result == null) return null;
            result.ConditionList = DalTask.GetConditionList(taskId);
            foreach (var item in result.ConditionList)
            {
                if (item.ActionName.Equals("11FinishTask", StringComparison.CurrentCultureIgnoreCase) && Guid.TryParse(item.SpecialPara, out var value))
                {
                    item.ChildName = DalTask.GetTaskInfo(value)?.TaskName;
                }
            }
            result.CouponList = DalTask.GetCouponList(taskId);
            result.TriggerList = triggerData.Where(g => g.TriggerType == 1 && !g.IsTimeout).Select(g => new TaskSimpleModel
            {
                TaskId = g.TriggerTaskId,
                TaskName = g.TriggerTaskName
            }).ToList();
            result.DisplayTriggerList = triggerData.Where(g => g.TriggerType == 0 && !g.IsTimeout).Select(g => new TaskSimpleModel
            {
                TaskId = g.TriggerTaskId,
                TaskName = g.TriggerTaskName
            }).ToList();
            result.TimeoutTriggerList = triggerData.Where(g => g.TriggerType == 0 && g.IsTimeout).Select(g => new TaskSimpleModel
            {
                TaskId = g.TriggerTaskId,
                TaskName = g.TriggerTaskName
            }).ToList();
            return result;
        }
        public static bool RefreshCache()
        {
            using (var client = new TaskClient())
            {
                var result = client.RefreshTaskCache(Guid.Empty);
                return result.Success && result.Result;
            }
        }
        public static int EditTaskConfig(TaskConfigModel request, string Operator)
        {
            var taskId = Guid.Empty;
            var result = true;
            var operate = default(string);
            var triggerData = request.TriggerList.Select(
                g => new TriggerTaskModel
                {
                    TriggerTaskId = g.TaskId,
                    TriggerTaskName = g.TaskName,
                    TriggerType = 1,
                    IsTimeout = false
                })
            .Union(request.DisplayTriggerList.Select(
                g => new TriggerTaskModel
                {
                    TriggerTaskId = g.TaskId,
                    TriggerTaskName = g.TaskName,
                    TriggerType = 0,
                    IsTimeout = false
                }))
            .Union(request.TimeoutTriggerList.Select(
                g => new TriggerTaskModel
                {
                    TriggerTaskId = g.TaskId,
                    TriggerTaskName = g.TaskName,
                    TriggerType = 0,
                    IsTimeout = true
                }))
             .ToList();
            foreach (var item in request.ConditionList)
            {
                if (item.ActionName.Equals("10BuyProduct"))
                {
                    var specialpara = new List<string>();
                    var data = item.SpecialPara.Split(';').Where(g => !string.IsNullOrEmpty(g)).ToList();
                    foreach (var paraItem in data)
                    {
                        if (int.TryParse(paraItem, out var value))
                        {
                            specialpara.Add($"OR{value.ToString("D8")}");
                        }
                        else if (paraItem.StartsWith("OR"))
                        {
                            specialpara.Add(paraItem);
                        }
                    }
                    if (!specialpara.Any()) return 0;
                    item.SpecialPara = string.Join(";", specialpara);
                }
            }
            using (var dbHelper = Component.Common.DbHelper.CreateDefaultDbHelper())
            {
                dbHelper.BeginTransaction();
                if (request.TaskId == Guid.Empty)
                {
                    request.TaskId = Guid.NewGuid();
                    taskId = request.TaskId;
                    operate = "Create";
                    result = DalTask.AddTaskConfig(request, Operator, dbHelper);
                    if (result)
                        result = DalTask.AddCondition(request.TaskId, request.ConditionList, dbHelper);
                    if (result && triggerData.Any())
                        result = DalTask.AddTriggerTask(request.TaskId, triggerData, dbHelper);
                    if (result && request.CouponList.Any())
                        result = DalTask.AddCoupon(request.TaskId, request.CouponList, dbHelper);
                }
                else
                {
                    taskId = request.TaskId;
                    operate = "Update";
                    result = DalTask.UpdateTaskConfig(request, dbHelper);
                    if (result)
                        result = DalTask.EditCoupon(request.TaskId, request.CouponList, dbHelper);
                    if (result)
                        result = DalTask.UpdateTriggerTask(request.TaskId, triggerData, dbHelper);
                    if (result)
                    {
                        result = DalTask.DeleteCondition(request.TaskId, dbHelper);
                        if (result) result = DalTask.AddCondition(request.TaskId, request.ConditionList, dbHelper);
                    }
                }
                if (result)
                {
                    result = RefreshCache();
                    dbHelper.Commit();
                }
                else
                {
                    dbHelper.Rollback();
                    return 2;
                }
                if (result) TaskOprLog(request.TaskId, Operator, operate, JsonConvert.SerializeObject(request));
                return result ? 1 : 3;
            }
        }

        public static List<TaskSimpleModel> GetTaskList(Guid? TaskId) => DalTask.GetTaskList(TaskId);

        public static int DeleteTaskConfig(Guid taskId) => DalTask.DeleteTaskConfig(taskId);


        public static bool TaskOprLog(Guid taskId, string Operator, string operate, string remark)
            => DalTask.TaskOprLog(taskId, Operator, operate, remark);

        public static List<TaskOprLogModel> GetTaskOprLog(Guid taskId)
            => DalTask.GetTaskOprLog(taskId);
        public static IEnumerable<DataAccess.Entity.Category> SelectProductCategory()
            => DataAccess.Entity.Category.Parse(DalTask.SelectProductCategory());

        public static List<OrdertaskRuleModel> GetRuleInfo() => DalTask.GetRuleInfo();
        public static void childCategory(ICollection<DataAccess.Entity.Category> Children, DataAccess.Entity.Category Category)
        {
            if (Category.ChildrenCategory == null || !Category.ChildrenCategory.Any())
                Children.Add(Category);
            else
            {
                foreach (var item in Category.ChildrenCategory)
                {
                    childCategory(Children, item);
                }
            }
        }

        public static bool EditOrderRule(OrderRuleDetailModel request, string Operator)
        {
            if (request.RuleNo == 0)
            {
                return DalTask.AddOrderRule(request, Operator);
            }
            else
            {
                return DalTask.UpdateOrderRuleInfo(request);
            }
        }

        public static OrderRuleDetailModel FetchOrderRuleInfo(int ruleId) => DalTask.FetchOrderRuleInfo(ruleId);
        public static OrderRuleProductModel FetchProductNameByPID(string PID) => DalTask.FetchProductNameByPID(PID);
    }
}