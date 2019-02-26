using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Models
{
    public class PromotionTaskParamsModel
    {
        /// <summary>
        /// 任务编号
        /// </summary>
        [DisplayName("任务编号:")]
        public int? PromotionTaskId { get; set; }

        /// <summary>
        /// 优惠券Ids
        /// </summary>
        [DisplayName("优惠券Ids:")]
        public string TaskPromotionListIds { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [DisplayName("任务名称:")]
        public string TaskName { get; set; }


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

        //[DisplayName("使用金额:")]
        ///// <summary>
        ///// 使用金额
        ///// </summary>
        //public float? UseMoney { get; set; }

        //[DisplayName("优惠金额:")]
        ///// <summary>
        ///// 优惠金额
        ///// </summary>
        //public float? DiscountMoney { get; set; }

        /// <summary>
        /// 用户电话号码
        /// </summary>
        public List<string> UserPhone { get; set; }


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

        /// <summary>
        /// 是否上传文件
        /// </summary>
        public DataAccess.Entity.PromotionConsts.SelectUserTypeEnum SelectUserType { get; set; }

        /// <summary>
        /// 任务类型(单次任务、重复任务)
        /// </summary>
        [DisplayName("优惠券类型:")]
        public PromotionTaskType TaskType { get; set; }

        /// <summary>
        /// 任务对应的优惠券规则逐渐编号
        /// </summary>
        public string PromotionListIds { get; set; }

        public int? IsImmediately { get; set; }
        public int IsLimitOnce { get; set; }
        /// <summary>
        /// 短信模板id ,如果选择的是发短信，则 需要填写短信模板id
        /// </summary>
        public int? SmsId { get; set; }
        /// <summary>
        /// 短信模板参数 JSON 数组 ["200", "轮胎499减100，保养199减100"]
        /// </summary>
        public string SmsParam { get; set; }
        /// <summary>
        /// 如果选择的是从bi活动表拿数据，这里是bi活动表的id
        /// </summary>
        public int? PromotionTaskActivityId { get; set; }
        /// <summary>
        /// 订单的产品类型,0=全部；1=套装产品；2=非套装产品
        /// </summary>
        public int ProductType { get; set; }

        //======2018年9月17日 新增内容=====

        #region 产品分类

        /// <summary>
        /// 产品类别Id集合(多个英文逗号分割)
        /// </summary>
        public string ProductCategoryIds { get; set; }

        /// <summary>
        /// 产品父类类别Id集合(多个英文逗号分割)
        /// </summary>
        public string ParentCategoryIds { get; set; }

        /// <summary>
        /// 节点规则(多个英文逗号分割)
        /// </summary>
        public string NodeNos { get; set; }

        /// <summary>
        /// 产品类别名称(多个英文逗号分割)
        /// </summary>
        public string CategoryNames { get; set; }
        #endregion

    }
}