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
        public Guid ActivityID { get; set; }
        public string ActivityName { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string Area { get; set; }
        public string BannerUrlAndroid { get; set; }
        public string BannerUrlIOS { get; set; }
        public string AppVlueAndroid { get; set; }
        public string AppVlueIOS { get; set; }
        public string BackgoundColor { get; set; }
        public string TomorrowText { get; set; }
        public int IsBannerIOS { get; set; }
        public int IsBannerAndroid { get; set; }
        public int ShowType { get; set; }
        public int ShippType { get; set; }
        public int IsTomorrowTextActive { get; set; }
        public int CountDown { get; set; }
        public int Status { get; set; }
        public string WebBanner { get; set; }
        public string WebCornerMark { get; set; }
        public string WebBackground { get; set; }
        public int IsNoActiveTime { get; set; }
        public string EndImage { get; set; }
        public bool IsEndImage { get; set; }
        public string WebOtherPart { get; set; }
        public int ActiveType { get; set; }
        public string PCodeIDS { get; set; }
        public bool ShoppingCart { get; set; }
        public string H5Url { get; set; }
        public int? PlaceQuantity { get; set; }
        public string TireSize { get; set; }
        public bool IsNewUserFirstOrder { get; set; }
        public IEnumerable<FlashSaleProductModel> Products { get; set; }
    }
    public class FlashSaleProductModel
    {
        public int PKID { get; set; }
        public Guid ActivityID { get; set; }
        public Guid UserID { get; set; }
        public string DeviceID { get; set; }
        public string Phone { get; set; }
        public string PID { get; set; }
        public int? Position { get; set; }
        public decimal Price { get; set; }
        public string Label { get; set; }
        public int? TotalQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public int SaleOutQuantity { get; set; }
        public string ProductName { get; set; }
        public string InstallAndPay { get; set; }
        public int? Level { get; set; }
        public string ImgUrl { get; set; }
        public bool IsUsePCode { get; set; }
        public string Channel { get; set; }
        public decimal FalseOriginalPrice { get; set; }
        public bool IsJoinPlace { get; set; }
        public bool IsShow { get; set; }

        public string InstallService { get; set; }
        public string ProductImg { get; set; }

        public bool OnSale { get; set; }
        public bool stockout { get; set; }
        /// <summary>
        /// 广告语
        /// </summary>
        public string AdvertiseTitle { get; set; }

        public int? PlaceQuantity { get; set; }
        public DateTime CreateDate { get; set; }

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
    }
}
