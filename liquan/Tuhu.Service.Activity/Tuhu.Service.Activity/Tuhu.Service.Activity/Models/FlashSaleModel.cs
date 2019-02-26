using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class FlashSaleModel
    {
        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid ActivityID { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndDateTime { get; set; }
        /// <summary>
        /// 活动创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 活动修改时间
        /// </summary>
        public DateTime UpdateDateTime { get; set; }
        /// <summary>
        /// 区(A,B,C,D)对应的页面上的位置
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// 安卓的bannerUrl
        /// </summary>
        public string BannerUrlAndroid { get; set; }
        /// <summary>
        /// IOS的bannerUrl
        /// </summary>
        public string BannerUrlIOS { get; set; }
        /// <summary>
        /// android的app处理值
        /// </summary>
        public string AppVlueAndroid { get; set; }
        /// <summary>
        /// IOS的app处理值
        /// </summary>
        public string AppVlueIOS { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        public string BackgoundColor { get; set; }
        /// <summary>
        /// 下期预告
        /// </summary>
        public string TomorrowText { get; set; }
        /// <summary>
        /// IOSbanner是否显示
        /// </summary>
        public int IsBannerIOS { get; set; }
        /// <summary>
        /// android banner是否显示
        /// </summary>
        public int IsBannerAndroid { get; set; }
        /// <summary>
        /// 展示类型（1：列表，2：单品）
        /// </summary>
        public int ShowType { get; set; }
        /// <summary>
        /// 送货类型（1：到家，2：到店）
        /// </summary>
        public int ShippType { get; set; }
        /// <summary>
        /// 下期预告是否启用（0：停用，1：启用）
        /// </summary>
        public int IsTomorrowTextActive { get; set; }
        /// <summary>
        /// 点击数
        /// </summary>
        public int CountDown { get; set; }
        /// <summary>
        /// 模板状态（1：启用0：关闭）
        /// </summary>
        public int Status { get; set; }
        public string WebBanner { get; set; }
        public string WebCornerMark { get; set; }
        public string WebBackground { get; set; }
        /// <summary>
        /// app和h5是否显示时间（1：显示，0：不显示）
        /// </summary>
        public int IsNoActiveTime { get; set; }
        public string EndImage { get; set; }
        public bool IsEndImage { get; set; }
        public string WebOtherPart { get; set; }
        /// <summary>
        /// 0限时抢购 1闪购 2普通活动 3活动秒杀
        /// </summary>
        public int ActiveType { get; set; }
        public string PCodeIDS { get; set; }
        public bool ShoppingCart { get; set; }
        public string H5Url { get; set; }
        public int? PlaceQuantity { get; set; }
        public string TireSize { get; set; }
        public bool IsNewUserFirstOrder { get; set; }
        public IEnumerable<FlashSaleProductModel> Products { get; set; }
        public  Dictionary<string, FlashSaleProductModel> ProductsDic { get; set; }
    } 
    public class FlashSaleProductModel
    {
        /// <summary>
        /// 系统主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid ActivityID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid UserID { get; set; }
        public string DeviceID { get; set; }
        public string Phone { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string PID { get; set; }
        public int? Position { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
        public string Label { get; set; }
        public int? TotalQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public int SaleOutQuantity { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        public string InstallAndPay { get; set; }
        public int? Level { get; set; }
        /// <summary>
        /// 产品图片
        /// </summary>
        public string ImgUrl { get; set; }
        public bool IsUsePCode { get; set; }
        public string Channel { get; set; }
        public decimal FalseOriginalPrice { get; set; }
        public bool IsJoinPlace { get; set; }
        public bool IsShow { get; set; }

        public string InstallService { get; set; }
        public string ProductImg { get; set; }


        /// <summary>
        /// 是否上下架
        /// </summary>
        public bool OnSale { get; set; }

        /// <summary>
        /// 是否缺货
        /// </summary>
        public bool stockout { get; set; }
        /// <summary>
        /// 广告语
        /// </summary>
        public string AdvertiseTitle { get; set; }

        public int? PlaceQuantity { get; set; }
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 产品品牌
        /// </summary>
        [Column("CP_Brand")]
        public string Brand { get; set; }

        /// <summary>
        /// 活动类型
        /// </summary>
        public int ActiveType { get; set; }
    }
    public class CheckFlashSaleProductRequest
    {
        public Guid UserID { get; set; }

        public IEnumerable<FlashSaleProductBuyLimitModel> ActivityProducts { get; set; }

        public string DeviceID { get; set; }
    }
    public class FlashSaleProductBuyLimitModel
    {
        public Guid ActivityID { get; set; }
        public string PID { get; set; }
        public int Num { get; set; }

        /// <summary>
        /// 是否有商品数量限制
        /// </summary>
        public bool HasNumLimit { get; set; }

        public bool OnlyNewMemberCanBuy { get; set; }
        public FlashSaleProductLimitType Type { get; set; }
    }
    public class FlashSaleProductCanBuyCountModel
    {
        public int Num { get; set; }
        public bool OnlyNewMemberCanBuy { get; set; }
        public CanBuyCountStatus Status { get; set; }
    }
    public enum CanBuyCountStatus
    {
        /// <summary>
        /// 已结束
        /// </summary>
        End,
        /// <summary>
        /// 未开始
        /// </summary>
        NotStart,
        /// <summary>
        /// 限购
        /// </summary>
        Limit,
        /// <summary>
        /// 不限购
        /// </summary>
        NoLimit,
        /// <summary>
        /// 不存在
        /// </summary>
        NoExist,
    }
    public enum FlashSaleProductLimitType
    {
        /// <summary>
        /// 总限购
        /// </summary>
        TotalLimit,
        /// <summary>
        /// 个人限购
        /// </summary>
        SingleLimit,
        /// <summary>
        /// 会场限购
        /// </summary>
        PlaceLimit,
        /// <summary>
        /// 已结束
        /// </summary>
        End,
        /// <summary>
        /// 未开始
        /// </summary>
        NotStart,
        /// <summary>
        /// 正常
        /// </summary>
        Success,
    }
    public class FlashSaleRecordsModel : FlashSaleProductModel
    {
        public int Quantity { get; set; }
    }
    public class FlashSaleTempModel
    {
        public Guid ActivityID { get; set; }
        public string PID { get; set; }
        /// <summary>
        /// 已经购买数量
        /// </summary>
        public int RecordQuantity { get; set; }

        /// <summary>
        /// 要买的数量
        /// </summary>
        public int BuyQuantity { get; set; }
        /// <summary>
        /// 总库存剩余
        /// </summary>
        public int? OverplusQuantity { get; set; }
        /// <summary>
        /// 会场限购
        /// </summary>
        public int? PlaceQuantity { get; set; }
        /// <summary>
        /// 个人限购数量
        /// </summary>
        public int? SingleQuantity { get; set; }

        public bool IsJoinPlace { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsNewUserFirstOrder { get; set; }

        /// <summary>
        /// 活动是否有设置限购
        /// </summary>
        public bool HasNumLimit { get; set; }
        
    }
}
