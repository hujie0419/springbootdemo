using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{

    /// <summary>
    /// GetBargainProductAndUserInfo   获取砍价商品信息和用户发起砍价信息   (砍价详情页)
    /// </summary>
    public class GetBargainProductAndUserInfoResponse
    {
        /// <summary>
        /// 砍价商品信息
        /// </summary>
        public ShareBargainProductInfo ProductInfo { get; set; }

        /// <summary>
        /// 用户发起砍价信息
        /// </summary>
        public UserOwnerBargainInfo OwnerBargainInfo { get; set; }

    }

    /// <summary>
    /// 砍价商品信息
    /// </summary>
    public class ShareBargainProductInfo 
    {
        public int ActivityProductID { get; set; } 

        /// <summary>
        /// 砍价活动ID - 固定值
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// 产品开始显示时间
        /// </summary>
        public DateTime? ShowBeginTime { get; set; }

        /// <summary>
        /// 砍价活动上架时间
        /// </summary>
        public DateTime BeginDateTime { get; set; }

        /// <summary>
        /// 砍价活动下架时间
        /// </summary>
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// 产品类型 1.商品 2.优惠券 
        /// </summary>
        public int ProductType { get; set; }

        /// <summary>
        /// 商品pid/券id
        /// </summary>
        public string Pid { get; set; } 

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品简称
        /// </summary>
        public string SimpleDisplayName { get; set; }

        /// <summary>
        /// 商品售价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 商品活动价
        /// </summary>
        public decimal FinalPrice { get; set; }

        /// <summary>
        /// 商品库存上限
        /// </summary>
        public int TotalStockCount { get; set; }

        /// <summary>
        /// 商品剩余库存
        /// </summary>
        public int CurrentStockCount { get; set; }

        /// <summary>
        /// 砍价人群限制:1-老带新商品 ,0-普通砍价商品
        /// </summary>
        public int CutPricePersonLimit { get; set; }

        /// <summary>
        /// 砍价所需次数
        /// </summary>
        public int Times { get; set; }

        /// <summary>
        /// 帮砍次数
        /// </summary>
        public int HelpCutPriceTimes { get; set; }

        /// <summary>
        /// 显示序号,从小到大
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 商品宣传图片
        /// </summary>
        public string Image1 { get; set; }

        /// <summary>
        /// 小程序分享标题
        /// </summary>
        public string WXShareTitle { get; set; }

        /// <summary>
        /// APP分享ID
        /// </summary>
        public string APPShareId { get; set; }

        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// 砍价成功提示
        /// </summary>
        public string SuccessfulHint { get; set; }

        /// <summary>
        /// 参与人数
        /// </summary>
        public int ParticipantCount { get; set; }

        /// <summary>
        /// 产品库上下架状态
        /// </summary>
        public bool OnSale { get; set; }

    }

    /// <summary>
    /// 用户发起砍价信息
    /// </summary>
    public class UserOwnerBargainInfo
    {
        /// <summary>
        /// 发起砍价时间
        /// </summary>
        public DateTime CreateBargainTime { get; set; }

        /// <summary>
        /// 发起砍价结束时间
        /// </summary>
        public DateTime CreateBargainEndTime { get; set; }

        /// <summary>
        /// 砍价商品可购买最后时间
        /// </summary>
        public DateTime CanBuyEndTIme { get; set; }
        
        /// <summary>
        /// 发起砍价已砍金额
        /// </summary>
        public decimal TotalReduce { get; set; }

        /// <summary>
        /// 砍价是否完成:已购买
        /// </summary>
        public bool IsFinish { get; set; }

        /// <summary>
        /// 是否发起过砍价
        /// </summary>
        public bool IsCreatedBargain { get; set; }

        /// <summary>
        /// 是否已分享(自己砍第二刀)
        /// </summary>
        public bool IsShared { get; set; }

        /// <summary>
        /// 发起砍价唯一标识
        /// </summary>
        public Guid IdKey { get; set; }

        /// <summary>
        /// 砍价状态。0-正在砍价；1-砍价成功；2-砍价失败（商品下架）；3-砍价失败(商品库存不足)
        ///     4-砍价失败(发起时间超过48小时)
        /// </summary>
        public int BargainStatus { get; set; }
        
    }

}
