using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class QiangGouModel
    {
        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid? ActivityID { get; set; }
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
        /// 活动类型 0限时抢购 1天天秒杀
        /// </summary>
        public int ActiveType { get; set; }

        public string ShortDate { get; set; }
        public string StrStatus { get; set; }
        /// <summary>
        /// 会场限购数量
        /// </summary>
        public int? PlaceQuantity { get; set; }

        public bool NeedExam { get; set; }
        /// <summary>
        /// 活动下的商品对象
        /// </summary>
        public IEnumerable<QiangGouProductModel> Products { get; set; }

        public bool IsNewUserFirstOrder { get; set; }

        public string NeedExamPids { get; set; }
        public string HashKey { get; set; }

        public bool IsDefault { get; set; }
    }
    public class QiangGouDiffModel
    {        /// <summary>
             /// 活动ID
             /// </summary>
        public Guid? ActivityID { get; set; }

        public int ActiveType { get; set; }

        public string ActivityName { get; set; }

        public string PID { get; set; }
        /// <summary>
        /// 促销价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 安装方式和支付方式
        /// </summary>
        public string InstallAndPay { get; set; }
        /// <summary>
        /// 是否可以使用优惠券
        /// </summary>
        public bool IsUsePCode { get; set; }
        /// <summary>
        /// 伪原价
        /// </summary>
        public decimal? FalseOriginalPrice { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        public bool NeedExam { get; set; }

    }
    public class QiangGouProductModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid ActivityID { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public string ActivityName { get; set; }
        /// <summary>
        /// 产品ID
        /// </summary> 
        /// 
        public string PID { get; set; }

        public string HashKey { get; set; }
        /// <summary>
        /// 促销价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 总限购
        /// </summary>
        public int? TotalQuantity { get; set; }
        /// <summary>
        /// 个人限购
        /// </summary>
        public int? MaxQuantity { get; set; }
        /// <summary>
        /// 已售出数量
        /// </summary>
        public int SaleOutQuantity { get; set; }
        /// <summary>
        /// 安装方式和支付方式
        /// </summary>
        public string InstallAndPay { get; set; }
        /// <summary>
        /// 是否可以使用优惠券
        /// </summary>
        public bool IsUsePCode { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        public String Channel { get; set; }
        /// <summary>
        /// 是否加入会场限购
        /// </summary>
        public bool IsJoinPlace { get; set; }
        /// <summary>
        /// 伪原价
        /// </summary>
        public decimal? FalseOriginalPrice { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 配置的活动产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string Label { get; set; }

        public decimal? CostPrice { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Position { get; set; }
        public bool IsShow { get; set; }
        public string Image { get; set; }
        public string InstallService { get; set; }
        public string DecreaseDegree { get; set; }
    }

    public class QiangGouForCache
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

        public IEnumerable<QiangGouProductForCache> Products { get; set; }
    }
    public class QiangGouProductForCache
    {
        public int PKID { get; set; }
        public Guid ActivityID { get; set; }
        public string PID { get; set; }

        public string TireSize { get; set; }
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
        public string ProductImg { get; set; }
    }

    public class QianggouSelectModel
    {
        public int type { get; set; }
        public Guid? aid { get; set; }

        public string aname { get; set; }
        public DateTime? stime { get; set; }
        public DateTime? etime { get; set; }
    }

    public class ProductModel : QiangGouProductModel
    {
        //public string PID { get; set; }
        public bool OnSale { get; set; }
        public bool Stockout { get; set; }

        public decimal cy_list_price { get; set; }

        public bool CaseSensitive { get; set; }

        public string Image { get; set; }
        //public string DisplayName { get; set; }

        //public string InstallService { get; set; }
    }

    public class QiangGouImportModel
    {
        public string PID { get; set; }

        public string 排序 { get; set; }

        public string 名称 { get; set; }
        public string 促销价 { get; set; }

        public string 伪原价 { get; set; }
        public string 每人限购 { get; set; }
        public string 总限购 { get; set; }

        public string 安装和支付方式 { get; set; }

        public string 优惠券 { get; set; }

        public string 渠道 { get; set; }

        public string 是否加入会场限购 { get; set; }

        public string 标签 { get; set; }

        public string 显示 { get; set; }
    }

    public class QiangGouSeckillImportModel
    {
        public string PID { get; set; }

        public string 排序 { get; set; }

        public string 秒杀标题 { get; set; }
        public string 秒杀价 { get; set; }

        public string 伪原价 { get; set; }
        public string 每人限购 { get; set; }
        public string 秒杀库存 { get; set; }

        public string 优惠券 { get; set; }


        public string 显示 { get; set; }
    }
}
