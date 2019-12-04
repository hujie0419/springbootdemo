using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Response
{
    /// <summary>
    /// 优惠券任务
    /// </summary>
    public class PromotionTaskModel
    {
        /// <summary>
		/// 主键自增列
        /// </summary>		
        public int PromotionTaskId { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>		
        public string TaskName { get; set; }
        /// <summary>
        /// 任务类型，单次任务或者重复任务
        /// </summary>		
        public int TaskType { get; set; }
        /// <summary>
        /// 执行任务的时间
        /// </summary>		
        public DateTime TaskStartTime { get; set; }
        /// <summary>
        /// 当为触发任务的时候 有任务执行的结束时间
        /// </summary>		
        public DateTime TaskEndTime { get; set; }
        /// <summary>
        /// 执行周期，按天算
        /// </summary>		
        public int ExecPeriod { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>		
        public int TaskStatus { get; set; }
        /// <summary>
        /// 任务创建时间
        /// </summary>		
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 选择用户的类型，1上传文件，2用户筛选，3运营活动
        /// </summary>		
        public int SelectUserType { get; set; }
        /// <summary>
        /// 过滤条件的开始时间  [单个]
        /// </summary>		
        public DateTime? FilterStartTime { get; set; }
        /// <summary>
        /// 过滤条件的结束日期  [单个]
        /// </summary>		
        public DateTime? FilterEndTime { get; set; }
        /// <summary>
        /// 过滤条件产品品牌
        /// </summary>		
        public string Brand { get; set; }
        /// <summary>
        /// 过滤条件类别
        /// </summary>		
        public string Category { get; set; }
        /// <summary>
        /// 过滤条件Pid
        /// </summary>		
        public string Pid { get; set; }
        /// <summary>
        /// 花费金额
        /// </summary>		
        public decimal SpendMoney { get; set; }
        /// <summary>
        /// 购买件数
        /// </summary>		
        public int PurchaseNum { get; set; }
        /// <summary>
        /// 过滤条件区域
        /// </summary>		
        public string Area { get; set; }
        /// <summary>
        /// 过滤条件渠道  【支持多选 逗号分割】
        /// </summary>		
        public string Channel { get; set; }
        /// <summary>
        /// 过滤条件订单类型
        /// </summary>		
        public string OrderType { get; set; }
        /// <summary>
        /// 安装类型，1到店安装 2非到店安装
        /// </summary>		
        public int InstallType { get; set; }
        /// <summary>
        /// 过滤条件订单状态
        /// </summary>		
        public string OrderStatus { get; set; }
        /// <summary>
        /// 过滤条件车品牌
        /// </summary>		
        public string Seable { get; set; }
        /// <summary>
        /// 过滤条件车型号
        /// </summary>		
        public string Vehicle { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>		
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 优惠券Ids 逗号隔开
        /// </summary>		
        public string CouponRulesIds { get; set; }
        /// <summary>
        /// 创建者email
        /// </summary>		
        public string Creater { get; set; }
        /// <summary>
        /// 修改者email
        /// </summary>		
        public string Auditor { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>		
        public DateTime AuditTime { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>		
        public DateTime ExecuteTime { get; set; }
        /// <summary>
        /// ShipStatus
        /// </summary>		
        //public string ShipStatus { get; set; }
        /// <summary>
        /// 是否立即执行
        /// </summary>		
        public int IsImmediately { get; set; }
        /// <summary>
        /// 关闭时间
        /// </summary>		
        public DateTime CloseTime { get; set; }
        /// <summary>
        /// 每人限塞几次
        /// </summary>		
        public bool IsLimitOnce { get; set; }
        /// <summary>
        /// 如果要发短信 ，需要短信模板id
        /// </summary>		
        public int SmsId { get; set; }
        /// <summary>
        /// 如果选择的是从bi表里发，需要保存Tuhu_bi..tbl_PromotionTaskActivity的主键
        /// </summary>		
        public int PromotionTaskActivityId { get; set; }
        /// <summary>
        /// 短信模板对应的参数格式为json数组
        /// </summary>		
        public string SmsParam { get; set; }
        /// <summary>
        /// 订单的产品类型,1=全部；2=套装产品；3=非套装产品
        /// </summary>		
        public int ProductType { get; set; }
        /// <summary>
        /// 发送成功数目
        /// </summary>		
        public int GetQuantity { get; set; }
        /// <summary>
        /// 发送失败数目
        /// </summary>		
        public int SendFailedQuantity { get; set; }

        /// <summary>
        /// 是否匹配
        /// </summary>		
        public bool IsMatch { get; set; }
        /// 不匹配原因
        /// </summary>		
        public string NotMatchMessage { get; set; }
    }
}
