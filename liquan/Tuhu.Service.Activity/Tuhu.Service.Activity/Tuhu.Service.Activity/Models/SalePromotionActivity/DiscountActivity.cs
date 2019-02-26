using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{

    public class BaseActivityModel
    {
        public string ActivityId { get; set; }

        /// <summary>
        /// 活动描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 打折描述（文案）
        /// </summary>
        public string Banner { get; set; }

        /// <summary>
        /// 打折类型 FullAmount全场满额折 FullCount单件满件折
        /// </summary>
        public string DiscountType { get; set; }

        /// <summary>
        /// 是否默认标签
        /// </summary>
        public bool IsDefaultLabel { get; set; }

        /// <summary>
        /// 活动标签
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 活动规则
        /// </summary>
        public List<DiscountActivityRule> RuleList { get; set; }

        /// <summary>
        /// 活动是否用户限购
        /// </summary>
        public bool IsUserPurchaseLimit { get; set; }
        public int UserLimitNum { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    #region Tag接口使用

    public class DiscountActivityForTag : BaseActivityModel
    {
        /// <summary>
        /// 活动限购数
        /// </summary>
        public int LimitQuantity { get; set; }

        /// <summary>
        /// 列表打折牛皮癣
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 详情页打折牛皮癣
        /// </summary>
        public string DetailImageUrl { get; set; }

        public string Pid { get; set; } 
    }

    public class ProductActivityInfoForTag
    {
        public string Pid { get; set; }
        public List<DiscountActivityForTag> DiscountActivityList { get; set; }
    }

    #endregion

    /// <summary>
    /// 商品的打折信息
    /// </summary>
    public class ProductDiscountActivityInfo 
    {
        public string Pid { get; set; }

        /// <summary>
        /// 商品当前时间是否有打折活动
        /// </summary>
        public bool HasDiscountActivity { get; set; }

        /// <summary>
        /// 商品剩余打折库存
        /// </summary>

        public int DiscountStock { get; set; }

        /// <summary>
        /// 商品打折牛皮癣
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 商品的打折活动信息
        /// </summary>
        public DiscountActivityAndUserLimitInfo DiscountActivityInfo { get; set; }

    }

    public class DiscountActivityAndUserLimitInfo : BaseActivityModel
    {
        /// <summary>
        /// 用户还可购买数量
        /// </summary>
        public int UserVisibleNum { get; set; }

    }

    /// <summary>
    /// 打折活动规则
    /// </summary>
    public class DiscountActivityRule
    {
        /// <summary>
        /// 打折条件
        /// </summary>
        public decimal Condition { get; set; }

        /// <summary>
        /// 折扣率百分比
        /// </summary>
        public int DiscountRate { get; set; }

        public string ActivityId { get; set; }

        public int? DiscountMethod { get; set; }

    }

    /// <summary>
    /// 商品命中的打折信息
    /// </summary>
    public class ProductHitDiscountResponse
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 商品当前时间是否有打折活动
        /// </summary>
        public bool HasDiscountActivity { get; set; }

        /// <summary>
        /// 是否可以享受活动折扣
        /// </summary>
        public bool IsHit { get; set; }

        /// <summary>
        /// 折扣后的价格
        /// </summary>
        public decimal DiscountPrice { get; set; }

        /// <summary>
        /// 当前产品命中的打折规则，不命中为null
        /// </summary>
        public SalePromotionActivityDiscount DiscountRule { get;set;}


        public string FailCode { get; set; }
        /// <summary>
        /// 产品不命中打折的原因
        /// </summary>
        public string FailMessage { get; set; }

    }

    public class ValidOrderDiscountResponse
    {
        /// <summary>
        /// 是否验证通过
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 失败原因列表
        /// </summary>
        public List<FailModel> FailList { get; set; }

    }

    /// <summary>
    /// 享受打折失败的原因
    /// </summary>
    public class FailModel
    {
        public string ActivityId { get; set; }

        public string FailCode { get; set; }

        public string FailMessage { get; set; }

    }

    #region 请求参数

    public class DiscountActivityRequest
    {
        public string Pid { get; set; }

        public int Num { get; set; }

        public decimal Price { get; set; }
        /// <summary>
        /// 1 到店
        /// 2 在线
        /// </summary>
        public int PaymentMethod { get; set; }
        /// <summary>
        /// 1 到店安装
        /// 2 上门安装
        /// 3 无需安装
        /// </summary>
        public int InstallMethod { get; set; }
    }

    #endregion

}
