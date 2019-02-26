using System;
using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class TaskModel
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 任务链接
        /// </summary>
        public string TaskLink { get; set; }
        /// <summary>
        /// 任务图标
        /// </summary>
        public string TaskIcon { get; set; }
        /// <summary>
        /// 任务类型 0-每日任务；1-新手任务；2-成长任务
        /// </summary>
        public int TaskType { get; set; }
        /// <summary>
        /// 任务状态 0-不显示；1-显示
        /// </summary>
        public int DisplayStatus { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 任务Id
        /// </summary>
        public Guid TaskId { get; set; }
        /// <summary>
        /// 有效期（天）；若小于0，则为永久
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// 任务显示顺序
        /// </summary>
        public int Sequence { get; set; }
    }
    public class TaskConfigModel : TaskModel
    {
        /// <summary>
        /// 任务按钮文案
        /// </summary>
        public string TaskText { get; set; }
        /// <summary>
        /// 任务开始时间
        /// </summary>
        public DateTime? TriggerTime { get; set; }
        /// <summary>
        /// 进度触发任务列表
        /// </summary>
        public List<TaskSimpleModel> TriggerList { get; set; } = new List<TaskSimpleModel>();
        /// <summary>
        /// 显示触发任务列表
        /// </summary>
        public List<TaskSimpleModel> DisplayTriggerList { get; set; } = new List<TaskSimpleModel>();
        /// <summary>
        /// 显示触发任务（任务过期后触发）
        /// </summary>
        public List<TaskSimpleModel> TimeoutTriggerList { get; set; } = new List<TaskSimpleModel>();
        /// <summary>
        /// 起始版本
        /// </summary>
        public string StartVersion { get; set; }
        /// <summary>
        /// 终止版本
        /// </summary>
        public string EndVersion { get; set; }
        /// <summary>
        /// 任务达成条件
        /// </summary>
        public List<TaskConditionModel> ConditionList { get; set; } = new List<TaskConditionModel>();
        /// <summary>
        /// 奖励文案
        /// </summary>
        public string AwardText { get; set; }
        /// <summary>
        /// 奖励链接
        /// </summary>
        public string AwardLink { get; set; }
        /// <summary>
        /// 奖励图片
        /// </summary>
        public string AwardImg { get; set; }
        /// <summary>
        /// 奖励积分
        /// </summary>
        public int Integral { get; set; }
        /// <summary>
        /// 奖励优惠券
        /// </summary>
        public List<CouponInfoModel> CouponList { get; set; } = new List<CouponInfoModel>();
        /// <summary>
        /// 奖励轮胎险
        /// </summary>
        public bool TireInsurance { get; set; }
        /// <summary>
        /// 随机优惠券
        /// </summary>
        public bool RandomCoupon { get; set; }
        /// <summary>
        /// 任务规则
        /// </summary>
        public string TaskRule { get; set; }
        /// <summary>
        /// 所需达成条件数量
        /// </summary>
        public int RequireActionCount { get; set; } = 1;
        /// <summary>
        /// 任务推荐优先级
        /// </summary>
        public int RecommendLevel { get; set; }
        /// <summary>
        /// 推荐任务图片
        /// </summary>
        public string RecommendImg { get; set; }
        /// <summary>
        /// 特定人群编号
        /// </summary>
        public int PopulationsTag { get; set; }
        /// <summary>
        /// 任务渠道
        /// </summary>
        public string TaskChannel { get; set; }
        public string WXAwardAppId { get; set; }
        public string WXAwardLink { get; set; }
        public string WXTaskAppId { get; set; }
        public string WXTaskLink { get; set; }
    }
    public class TaskConditionModel
    {
        /// <summary>
        /// Action名称
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// 所需数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 特殊参数
        /// </summary>
        public string SpecialPara { get; set; }
        public string Remark { get; set; }
        public string ChildName { get; set; }
    }

    public class CouponInfoModel
    {
        public Guid GetRuleId { get; set; }
        public int Count { get; set; }
    }

    public class TaskActionModel
    {
        public string ActionName { get; set; }
        public string Remark { get; set; }
    }

    public class TaskOprLogModel
    {
        public string Operator { get; set; }
        public string Operate { get; set; }
        public DateTime CreateDateTime { get; set; }
    }

    public class OrdertaskRuleModel
    {
        public int RuleNo { get; set; }
        /// <summary>
        /// Rule类型,默认为`商品购买类型`
        /// </summary>
        public string RuleType { get; set; }
        public string RuleName { get; set; }
        public bool RuleStatus { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public string Creator { get; set; }
    }
    public class OrderRuleDetailModel: OrdertaskRuleModel
    {
        public string Brand { get; set; }
        public string CategoryList { get; set; }
        public string PIDS { get; set; }
        /// <summary>
        /// 任务达成类型,0-根据品类;1-根据PID
        /// </summary>
        public int ConditionType { get; set; }
        /// <summary>
        /// 任务计时及结算状态,0-付款完成;1-订单完成
        /// </summary>
        public int MatchType { get; set; }
    }

    public class OrderRuleProductModel
    {
        public string PID { get; set;}
        public String DisplayName { get; set; }
    }

    public class TaskSimpleModel
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; }
    }


    public class TriggerTaskModel
    {
        public Guid TriggerTaskId { get; set; }
        public string TriggerTaskName { get; set; }
        public int TriggerType { get; set; }
        public bool IsTimeout { get; set; }
    }
}
