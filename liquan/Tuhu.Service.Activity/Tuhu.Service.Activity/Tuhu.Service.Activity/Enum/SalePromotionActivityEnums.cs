using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Enum
{
    /// <summary>
    /// 促销活动类型
    /// </summary>
    public enum SalePromotionActivityType
    {
        [Description("满折活动")]
        FullDiscount=1
    }

    /// <summary>
    /// 活动分类显示状态
    /// </summary>
    public enum SalePromotionActivityStatus
    {
        [Description("所有")]
        All = 0,
        [Description("进行中")]
        Online = 1,
        [Description("待审核")]
        WaitAudit = 2,
        [Description("待上线")]
        StayOnline = 3,
        [Description("已拒绝")]
        Rejected = 4,
        [Description("已结束")]
        End = 5
    }

    /// <summary>
    /// 满折活动类型
    /// </summary>
    public enum SalePromotionActivityDiscountMethod
    {
        [Description("满额折扣")]
        FullAmount = 1,
        [Description("满件折扣")]
        FullCount = 2
    }

    /// <summary>
    /// 促销活动审核状态
    /// </summary>
    public enum SalePromotionActivityAuditStatus 
    {
        [Description("初始")]
        Initial = 0,
        [Description("待审核")]
        WaitAudit = 1,
        [Description("通过审核")]
        PassAudit = 2,
        [Description("拒绝")]
        RejectAudit = 3
    }

    /// <summary>
    /// 活动下架状态 
    /// </summary>
    public enum SalePromotionActivityShelveStatus 
    {
        [Description("未下架")]
        OnShelve = 0,
        [Description("已下架")]
        UnShelve = 1
    }
   
}
