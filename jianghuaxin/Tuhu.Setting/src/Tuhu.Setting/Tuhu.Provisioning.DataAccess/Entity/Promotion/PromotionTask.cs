using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 优惠券任务模型
    /// </summary>
    public class PromotionTask
    {
        public PromotionTask()
        {
            //默认状态为待审核
            TaskStatus = (int)PromotionConsts.PromotionTaskStatusEnum.PendingAudit;
            TaskType = (int)PromotionConsts.TaskTypeEnum.Single;
            SelectUserType = (int)PromotionConsts.SelectUserTypeEnum.UploadFile;
        }

        public int? PromotionTaskId { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [DisplayName("任务名称:")]
        public string TaskName { get; set; }

        /// <summary>
        /// 任务类型(单次任务、重复任务)
        /// </summary>
        [DisplayName("任务类型:")]
        public int? TaskType { get; set; }

        /// <summary>
        /// 任务开始时间
        /// </summary>
        [DisplayName("任务开始时间:")]
        public DateTime? TaskStartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [DisplayName("任务结束时间:")]
        public DateTime? TaskEndTime { get; set; }


        /// <summary>
        /// 执行周期(天)
        /// </summary>
        [DisplayName("执行周期:")]
        public int? ExecPeriod { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        [DisplayName("状态:")]
        public int? TaskStatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间:")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 选择用户的方式 上传文件 条件选择
        /// </summary>
        public int? SelectUserType { get; set; }

        /// <summary>
        /// 过滤条件开始时间
        /// </summary>
        [DisplayName("过滤起始时间:")]
        public DateTime? FilterStartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [DisplayName("过滤终止时间:")]
        public DateTime? FilterEndTime { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Pid
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 购买金额
        /// </summary>
        public double? SpendMoney { get; set; }

        /// <summary>
        /// 购买件数
        /// </summary>
        public int? PurchaseNum { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        ///订单类型
        /// </summary>
        public string OrderType { get; set; }
        /// <summary>
        /// 安装类型
        /// </summary>
        public int? InstallType { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public string OrderStatus { get; set; }

        /// <summary>
        /// 车型品牌
        /// </summary>
        public string Seable { get; set; }

        /// <summary>
        /// 车型型号
        /// </summary>
        public string Vehicle { get; set; }

        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 任务对应的优惠券规则编号  ,117,118,119,
        /// </summary>
        public string CouponRulesIds { get; set; }
        /// <summary>
        /// 任务对应的优惠券规则逐渐编号
        /// </summary>
        public string PromotionListIds { get; set; }
        /// <summary>
        /// 是否立刻执行
        /// </summary>
        public int? IsImmediately { get; set; }
        /// <summary>
        /// 是否每人限发一次
        /// </summary>
        public bool IsLimitOnce { get; set; }
        /// <summary>
        /// 短信模板id
        /// </summary>
        public int? SmsId { get; set; }
        /// <summary>
        /// 短信参数 ["200", "轮胎499减100，保养199减100"] JSON 数组
        /// </summary>
        public string SmsParam { get; set; }
        /// <summary>
        /// 从bi活动表里获取数据的时候，传活动表的pkid
        /// </summary>
        public int? PromotionTaskActivityId { get; set; }

    }
}
