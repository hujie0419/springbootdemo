using System;
using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BaoYangPriceGuideList
    {
        public string Brand { get; set; }
        public string PID { get; set; }
        public string ProductName { get; set; }

        public string Category { get; set; }
        public int JiaQuan { get; set; }
        public int? totalstock { get; set; }

        public int QPLStock { get; set; }

        public int? SelfStock { get; set; }
        public int? num_week { get; set; }
        public int? num_month { get; set; }
        public int? num_threemonth { get; set; }
        public decimal? cost { get; set; }
        /// <summary>
        /// 最近一次采购价
        /// </summary>
        public decimal? PurchasePrice { get; set; }
        public int VehicleCount { get; set; }
        public decimal Price { get; set; }
        public decimal? TBPrice { get; set; }
        public string TBPID { get; set; }
        public decimal? TB2Price { get; set; }
        public string TB2PID { get; set; }
        public decimal? TM1Price { get; set; }
        public decimal? QPLPrice { get; set; }
        public string TM1PID { get; set; }
        public decimal? TM2Price { get; set; }
        public string TM2PID { get; set; }
        public decimal? TM3Price { get; set; }
        public string TM3PID { get; set; }
        public decimal? TM4Price { get; set; }
        public string TM4PID { get; set; }
        public decimal? JDPrice { get; set; }
        public string JDPID { get; set; }
        /// <summary>
        /// 市场价格
        /// </summary>
        public decimal? MarketingPrice { get; set; }
        public decimal? JDFlagShipPrice { get; set; }
        public string JDFlagShipPID { get; set; }

        public decimal? JDSelfPrice { get; set; }
        public string JDSelfPID { get; set; }

        public decimal? TWLTMPrice { get; set; }
        public string TWLTMPID { get; set; }

        public decimal UpperLimit { get; set; }
        public decimal LowerLimit { get; set; }

        public int StockStatus { get; set; }

        public int OnSaleStatus { get; set; }

        public decimal? TheoryGuidePrice { get; set; }

        public decimal? ActualGuidePrice { get; set; }

        public string YcwyId { get; set; }

        public decimal? YcwyPrice { get; set; }

        public string KzId { get; set; }

        public decimal? KzPrice { get; set; }

        public string QccrlId { get; set; }

        public decimal? QccrlPrice { get; set; }

        public string QccrpId { get; set; }

        public decimal? QccrpPrice { get; set; }

        public int ShopStock { get; set; }

        public string SecondType { get; set; }

        public string ThirdType { get; set; }

        public decimal? FlashSalePrice { get; set; }

        /// <summary>
        /// 是否展示
        /// </summary>
        public bool IsShow { get; set; }
    }

    public class BaoYangPriceSelectModel
    {
        public string ProductName { get; set; }

        public string FirstType { get; set; }
        public string SecondType { get; set; }
        public string ThirdType { get; set; }

        public string PID { get; set; }

        public string Brand { get; set; }

        public int StockStatus { get; set; }

        /// <summary>
        /// 类型 0无 1自营平台/进货价 2第三方品台/官网价格  3竞争对手价格/官网价格
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 大于1 小于-1
        /// </summary>
        public int Contrast { get; set; }

        /// <summary>
        /// 比例
        /// </summary>
        public double Proportion { get; set; }
        //1上架  2下架 0 所有
        public int OnSaleStatus { get; set; }
        /// <summary>
        /// 用于对比毛利额
        /// </summary>
        public decimal? MaoLiE { get; set; }

        /// <summary>
        /// 毛利额对比符号 -1 小于 1大于
        /// </summary>
        public int MatchMaoLiE { get; set; }

        public decimal? MaoLiLv { get; set; }

        public int MatchMaoLiLv { get; set; }

        public decimal? PCPricePer { get; set; }

        public double MatchPCPricePer { set; get; }

        public int MatchWarnLine { get; set; }

        public int? TurnoverDays { get; set; }

        public int MatchTurnoverDays { get; set; }

        public decimal? QplMaoLiE { get; set; }

        public int MatchQplMaoLiE { get; set; }

        public decimal? QplMaoLiLv { get; set; }

        public int MatchQplMaoLiLv { get; set; }

        public decimal? ShopMaoLiE { get; set; }

        public int MatchShopMaoLiE { get; set; }

        public decimal? ShopMaoLiLv { get; set; }

        public int MatchShopMaoLiLv { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int? TotalStock { get; set; }

        public int MatchTotalStock { get; set; }

        public string SitePrices { get; set; }

        public string SitePrice { get; set; }

        public bool? IsShow { get; set; }
    }

    public class Product_SalespredictData
    {
        public string Pid { get; set; }

        public string WareHouseName { get; set; }

        public int TotalStock { get; set; }

        public int Num_ThreeMonth { get; set; }

        public int Num_Month { get; set; }

        public int Num_Week { get; set; }

        /// <summary>
        /// 仓库类型
        /// </summary>
        public string WarehouseType { get; set; }
    }

    public class BaoYangPriceWeight
    {
        /// <summary>
        /// 类别
        /// </summary>
        public string WeightType { get; set; }

        /// <summary>
        /// 类别对应的值
        /// </summary>
        public string WeightName { get; set; }


        /// <summary>
        /// 加权值
        /// </summary>
        public double? WeightValue { get; set; }

        public string DisplayName { get; set; }

        public string CategoryName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///主键
        /// </summary>
        public int? PKID { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdateTime { get; set; }

    }

    public class ProductPriceWeight
    {
        /// <summary>
        /// key->二级类目名
        /// value-> 三级类目数据
        /// </summary>
        public Dictionary<string, List<CategoryWeight>> CategoryWeights { get; set; }

        /// <summary>
        /// 基础加权值
        /// </summary>
        public Tuple<int?,int> BaseWeight { get; set; }
    }

    /// <summary>
    /// 三级类目
    /// </summary>
    public class CategoryWeight
    {
        public int? Pkid { get; set; }
        //三级类目名
        public string WeightName { get; set; }

        //三级类目加权值
        public int WeightValue { get; set; }

        // 类目中文名
        public string DisplayName { get; set; }

        /// <summary>
        /// key->品牌名
        /// value->品牌加权值
        /// </summary>
        public List<BrandWeight> Brands { get; set; }
    }

    public class BrandWeight
    {
        public int? Pkid { get; set; }

        public string WeightName { get; set; }

        public int WeightValue { get; set; }
    }


    public class BaoYangWarningLine
    {
        public int PKID { get; set; }
        public decimal MinGuidePrice { get; set; }
        public decimal MaxGuidePrice { get; set; }
        public decimal LowerLimit { get; set; }
        public decimal UpperLimit { get; set; }
    }

    public class BaoYangGuideViewModel
    {
        public IEnumerable<BaoYangWarningLine> Warn { get; set; }
        public IEnumerable<BaoYangPriceWeight> Para { get; set; }
    }

    public class PriceUpdateAuditModel
    {
        public int PKID { get; set; }
        public string PID { get; set; }
        public decimal ApplyPrice { get; set; }
        public string ApplyReason { get; set; }
        public DateTime ApplyDateTime { get; set; }
        public string ApplyPerson { get; set; }
        public DateTime AuditDateTime { get; set; }
        public string AuditPerson { get; set; }
        /// <summary>
        /// 0 待审批 1审批通过 2审批失败 3被覆盖
        /// </summary>
        public int AuditStatus { get; set; }

        public string Brand { get; set; }

        public string ProductName { get; set; }

        public decimal? Cost { get; set; }

        public int JiaQuan { get; set; }

        public decimal ListPrice { get; set; }

        public decimal? JDSelfPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public int? TotalStock { get; set; }
        public int? Num_Week { get; set; }
        public int? Num_Month { get; set; }
        public decimal? GuidePrice { get; set; }
        public decimal? NowPrice { get; set; }
        public double? MaoLiLv { get; set; }
        public double? OverFlow { get; set; }
        public decimal? JdSelf { get; set; }
        public decimal? MaoLiE { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
    }

    public class BaoYangProductMapping
    {
        public string Pid { get; set; }
        public string ShopCode { get; set; }

        public decimal? Price { get; set; }

        public long? ItemID { get; set; }

        public string ItemCode { get; set; }
    }

    public class BaoYangShopStock
    {
        public string Pid { get; set; }

        public int ShopId { get; set; }

        public string ShopName { get; set; }

        public int StockQuantity { get; set; }

        public int SaleNum { get; set; }
    }
}
