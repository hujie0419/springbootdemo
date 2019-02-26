using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class BargainProductModel
    {
        /// <summary>
        /// 标识该次活动中该件商品
        /// </summary>
        public int ActivityProductId { get; set; }
        /// <summary>
        /// 产品PID
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName { get; set; }
        /// <summary>
        /// 砍价成功提示
        /// </summary>
        public string SuccessfulHint { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; set; }
        /// <summary>
        /// 砍价成功后最终价
        /// </summary>
        public decimal FinalPrice { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        public string Image1 { get; set; }
        /// <summary>
        /// 需要砍价次数
        /// </summary>
        public int Times { get; set; }
        public int Sequence { get; set; }
        /// <summary>
        /// 上架时间
        /// </summary>
        public DateTime BeginDateTime { get; set; }
        /// <summary>
        /// 下架时间
        /// </summary>
        public DateTime EndDateTime { get; set; }
        /// <summary>
        /// 小程序分享标题
        /// </summary>
        public string WXShareTitle { get; set; }
        /// <summary>
        /// APP分享ID
        /// </summary>
        public string AppShareID { get; set; }
        /// <summary>
        /// 是否有库存
        /// </summary>
        public bool HasStock { get; set; }
        /// <summary>
        /// 砍价是否已完成
        /// </summary>
        public bool IsOver { get; set; }
        /// <summary>
        /// 是否已购买
        /// </summary>
        public bool IsPurchased { get; set; }
        /// <summary>
        /// 是否分享过
        /// </summary>
        public bool IsShare { get; set; }
        /// <summary>
        /// 是否有砍价记录
        /// </summary>
        public bool HasBargainHistory { get; set; }
        /// <summary>
        /// 是否是页面分享人，用于判断分享界面
        /// </summary>
        public bool IsOwner { get; set; }
        /// <summary>
        /// 是否已参与过砍价
        /// </summary>
        public bool Bargained { get; set; }
        /// <summary>
        /// 砍价活动默认活动ID
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// 产品类型：1.商品 2.优惠券
        /// </summary>
        public int ProductType { get; set; }

        /// <summary>
        /// 剩余库存
        /// </summary>
        public int CurrentStockCount { get; set; }

        /// <summary>
        /// 砍价产品开始显示时间 
        /// </summary>
        public DateTime? ShowBeginTime { get; set; }

    }

    public class BargainProductItem
    {
        public int ActivityProductId { get; set; }
        public string PID { get; set; }

        /// <summary>
        /// 发起砍价唯一标识
        /// </summary>
        public Guid IdKey { get; set; } = Guid.Empty;
    }

    public class BargainProductInfo : BargainProductItem
    {
        public Guid OwnerId { get; set; }
    }

    public class BargainProductHistory
    {
        public Guid UserId { get; set; }
        /// <summary>
        /// 该次活动该件商品
        /// </summary>
        public int ActivityProductId { get; set; }
        /// <summary>
        /// 当前已砍去价格
        /// </summary>
        public decimal TotalReduce { get; set; }
        /// <summary>
        /// 当前已分享砍价次数
        /// </summary>
        public int CurrentlyTimes { get; set; }
        /// <summary>
        /// 受邀人砍价记录
        /// </summary>
        public List<BargainHistory> BargainHistoryList { get; set; }
    }

    public class BargainHistory
    {
        /// <summary>
        /// 受邀人UserId
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// “砍去”的价格
        /// </summary>
        public decimal Reduce { get; set; }
        /// <summary>
        /// 0.5-0.8 摸了一下，0.8-1.2 华丽一刀，1.2-1.5 暴击了
        /// </summary>
        public double Rate { get; set; }
        /// <summary>
        /// 提示文案
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// 参与砍价的时间
        /// </summary>
        public DateTime BargainTime { get; set; }

        /// <summary>
        /// 当前商品总共被“砍去”的价格
        /// </summary>
        public decimal TotalReduce { get; set; }
    }

    public class BargainResult
    {
        public int Code { get; set; }
        /// <summary>
        /// 碎金砍去比例
        /// </summary>
        public double Rate { get; set; }
        /// <summary>
        /// 本次砍去金额
        /// </summary>
        public decimal Reduce { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// 剩余需要砍去金额
        /// </summary>
        public decimal RemnantMoney { get; set; }
        /// <summary>
        /// 商品简称
        /// </summary>
        public string SimpleDisplayName { get; set; }
        /// <summary>
        /// 帮砍次数
        /// </summary>
        public int HelpCutPriceTimes { get; set; }

        /// <summary>
        /// 当前帮砍成功是第N刀 - 帮砍成功才有效
        /// </summary>
        public int Setp { get; set; }
    }

    /// <summary>
    /// 砍价配置实体
    /// </summary>
    public class CurrentBargainData
    {
        public int PKID { get; set; }
        /// <summary>
        /// 标识该次产品分享的唯一键值
        /// </summary>
        public Guid IdKey { get; set; }
        /// <summary>
        /// 活动产品Id（砍价产品配置表主键）
        /// </summary>
        public int ActivityProductId { get; set; }
        public string PID { get; set; }
        public string ProductName { get; set; }
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// 砍价配置库存
        /// </summary>
        public int TotalStockCount { get; set; }

        /// <summary>
        /// 剩余砍价库存
        /// </summary>
        public int CurrentStockCount { get; set; }

        /// <summary>
        /// 是否完成砍价
        /// </summary>
        public bool IsOver { get; set; }

        /// <summary>
        /// 砍价发起人id
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// 砍价基准金额
        /// </summary>
        public decimal Average { get; set; }
        public int TotalCount { get; set; }

        /// <summary>
        /// 当前已砍次数
        /// </summary>
        public int CurrentCount { get; set; }

        /// <summary>
        /// 当前已砍金额
        /// </summary>
        public decimal CurrentRedece { get; set; }
        public decimal OriginalPrice { get; set; }
        public string SuccessfulHint { get; set; }
        public decimal FinalPrice { get; set; }

        /// <summary>
        /// 砍价产品类型：1.商品，2.优惠券 
        /// </summary>
        public int ProductType { get; set; }

        public string SimpleDisplayName { get; set; }

        /// <summary>
        /// 帮砍次数
        /// </summary>
        public int HelpCutPriceTimes { get; set; }

        /// <summary>
        /// 砍价人群限制, 0=普通砍价商品，1=老带新商品
        /// </summary>
        public int CutPricePersonLimit { get; set; }

        /// <summary>
        /// 上架时间
        /// </summary>
        public DateTime BeginDateTime { get; set; }

        /// <summary>
        /// 前几个砍价力度大的人数
        /// </summary>
        public int BigCutBeforeCount { get; set; }

        /// <summary>
        /// 砍价力度大的金额区间占目标价百分比
        /// </summary>
        public int BigCutPriceRate { get; set; }

    }

    public class ShareBargainBaseResult
    {
        public int Code { get; set; }
        public string Info { get; set; }
    }
    public class BargainShareResult : ShareBargainBaseResult
    {
        public Guid? IdKey { get; set; }
    }

    public class BargainShareProductModel
    {
        /// <summary>
        /// 1-数据正常，2-商品售罄，3-砍价已结束，4-已下架，5-用户已参与过此次砍价
        /// </summary>
        public int Code { get; set; }
        public int ActivityProductId { get; set; }
        public string PageName { get; set; }
        public bool IsPurchased { get; set; }
        public string Info { get; set; }
        public bool IsOwner { get; set; }
        public Guid OwnerId { get; set; }
        public Guid Idkey { get; set; }
        public string PID { get; set; }
        public string ProductName { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public string Image1 { get; set; }
        public int Times { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }
    public class ActivityPriceModel
    {
        public string PID { get; set; }
        public int Code { get; set; }
        public decimal ActivityPrice { get; set; }
        public bool ApplyCoupon { get; set; }
        public string ErrorMsg { get; set; }
    }
    public class ActivityPriceRequest
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public List<ActivityPriceItem> Items { get; set; }
    }
    public class ActivityPriceItem
    {
        public Guid ActicityId { get; set; }
        public string PID { get; set; }
    }
    public class ProductActivityType
    {
        public Guid ActivityId { get; set; }
        public List<string> PIDs { get; set; }
    }

    public class BargainrulesModel
    {
        public string Style { get; set; }
        public string ImgUrl { get; set; }
        public string Title { get; set; }
        public int RulesCount { get; set; }
        public string QAdATA { get; set; }
        /// <summary>
        /// 小程序列表分享文案
        /// </summary>
        public string WXAPPListShareText { get; set; }
        /// <summary>
        /// 小程序列表分享图片
        /// </summary>
        public string WXAPPListShareImg { get; set; }
        /// <summary>
        /// 小程序详情页分享文案
        /// </summary>
        public string WXAPPDetailShareText { get; set; }
        /// <summary>
        /// APP列表分享标签
        /// </summary>
        public string APPListShareTag { get; set; }
        /// <summary>
        /// APP详情分享标签
        /// </summary>
        public string AppDetailShareTag { get; set; }
        /// <summary>
        /// 轮播信息文案
        /// </summary>
        public string SliceShowText { get; set; }
    }
    public class BargainGlobalConfigModel
    {
        /// <summary>
        /// 风格主题名称
        /// </summary>
        public string Style { get; set; }
        /// <summary>
        /// 背景图片
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 介绍页名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 砍价规则数量
        /// </summary>
        public int RulesCount { get; set; }
        /// <summary>
        /// 砍价规则
        /// </summary>
        public List<BargainRules> BargainRule { get; set; }
        /// <summary>
        /// 小程序列表分享文案
        /// </summary>
        public string WXAPPListShareText { get; set; }
        /// <summary>
        /// 小程序列表分享图片
        /// </summary>
        public string WXAPPListShareImg { get; set; }
        /// <summary>
        /// 小程序详情页分享文案
        /// </summary>
        public string WXAPPDetailShareText { get; set; }
        /// <summary>
        /// APP列表分享标签
        /// </summary>
        public string APPListShareTag { get; set; }
        /// <summary>
        /// APP详情分享标签
        /// </summary>
        public string AppDetailShareTag { get; set; }
        /// <summary>
        /// 轮播信息文案
        /// </summary>
        public string SliceShowText { get; set; }
    }
    public class BargainRules
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    #region 砍价重构
    /// <summary>
    /// APP 首页模块显示砍价商品
    /// </summary>
    public class SimpleBargainProduct
    {
        /// <summary>
        /// 标识该次活动中该件商品
        /// </summary>
        public int ActivityProductId { get; set; }
        /// <summary>
        /// 商品PID
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 商品原价
        /// </summary>
        public decimal OriginPrice { get; set; }
        /// <summary>
        /// 商品活动价
        /// </summary>
        public decimal ActivityPrice { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 当前砍价人数
        /// </summary>
        public int CurrentCount { get; set; }
        /// <summary>
        /// 产品库上下架状态
        /// </summary>
        public bool OnSale { get; set; }
    }
    /// <summary>
    /// 砍价商品信息Model
    /// </summary>
    public class BargainProductNewModel : SimpleBargainProduct
    {
        /// <summary>
        ///  商品显示名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 砍价商品简称~
        /// </summary>
        public string SimpleDisplayName { get; set; }
        /// <summary>
        /// 商品上架时间
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 商品下架时间
        /// </summary>~
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 商品类型 1-商品；2-优惠券
        /// </summary>
        public int ProductType { get; set; }
        /// <summary>
        /// 所需砍价次数
        /// </summary>
        public int RequiredTimes { get; set; }
        /// <summary>
        /// 剩余库存
        /// </summary>
        public int CurrentStockCount { get; set; }
        /// <summary>
        /// 用户可购买时间限制（若砍价完成，此时间点之前可购买）
        /// </summary>
        public DateTime FinalTime { get; set; }
        /// <summary>
        /// 砍价成功提示
        /// </summary>
        public string SuccessfulHint { get; set; }
        /// <summary>
        /// 砍价活动默认活动ID
        /// </summary>
        public Guid ActivityId { get; set; }
        /// <summary>
        /// 小程序分享标题
        /// </summary>
        public string WXShareTitle { get; set; }
        /// <summary>
        /// APP分享ID
        /// </summary>
        public string AppShareID { get; set; }
        /// <summary>
        /// 帮砍次数
        /// </summary>
        public int HelpCutPriceTimes { get; set; }

        /// <summary>
        /// 砍价人群限制, 0=普通砍价商品，1=老带新商品
        /// </summary>
        public int CutPricePersonLimit { get; set; }

        #region 用户状态相关
        /// <summary>
        /// 0-正在砍价；1-砍价成功；2-砍价失败（商品下架）；3-砍价失败(商品库存不足)
        /// </summary>
        public int BargainStatus { get; set; }
        /// <summary>
        /// 发起砍价时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// “砍去”的价格
        /// </summary>
        public decimal Reduce { get; set; }
        /// <summary>
        /// 是否已购买/已领券
        /// </summary>
        public bool IsFinish { get; set; }
        /// <summary>
        /// 是否已经发起过砍价（主人砍第一刀）
        /// </summary>
        public bool IsOwner { get; set; }
        /// <summary>
        /// 用户是否已分享（主人砍第二刀）
        /// </summary>
        public bool IsShared { get; set; }
        /// <summary>
        /// 用户已分享，该次砍价唯一标识
        /// </summary>
        public Guid IdKey { get; set; }
        #endregion
    }

    public class BargainHistoryModel : BargainProductNewModel
    {
    }
    public class SliceShowInfoModel
    {
        public Guid UserId { get; set; }
        public string ProductName { get; set; }
        public string SliceShowText { get; set; }
        public DateTime FinishTime { get; set; }
        public string SimpleDisplayName { get; set; }
    }

    public class CreateBargainResult : BargainResult
    {
        public Guid IdKey { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsShared { get; set; }
    }
    public class InviteeBarginInfo
    {
        /// <summary>
        /// 是否页面主人
        /// </summary>
        public bool IsOwner { get; set; }
        /// <summary>
        /// 受邀人当天砍价次数
        /// </summary>
        public int IntradayBargainCount { get; set; }
        /// <summary>
        /// 受邀人是否砍价成功
        /// </summary>
        public bool BargainSucceed { get; set; }
        /// <summary>
        /// 受邀人发起砍价IdKey(若未发起成功，则为Guid.Empty)
        /// </summary>
        public Guid InviteeIdkey { get; set; }
        /// <summary>
        /// 产品PID
        /// </summary>
        public string PId { get; set; }
        public int ActivityProductId { get; set; }
    }

    /// <summary>
    /// 同一用户对同一活动商品的所有砍价数据
    /// </summary>
    public class BargainShareTimes
    {

        /// <summary>
        /// 标识该次产品分享的唯一键值
        /// </summary>
        public Guid Idkey { get; set; }

        /// <summary>
        /// 砍价活动发起人的UserId
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// 砍价用户UserId
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// “砍去”的价格
        /// </summary>
        public decimal Reduce { get; set; }
    }
    #endregion
}
