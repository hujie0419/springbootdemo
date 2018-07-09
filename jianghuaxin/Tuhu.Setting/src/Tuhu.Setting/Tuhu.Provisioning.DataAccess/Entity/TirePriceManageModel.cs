using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class GuidePara
    {
        /// <summary>
        /// 类别
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 类别对应的值
        /// </summary>
        public string Item { get; set; }

        /// <summary>
        /// 加权值
        /// </summary>
        public double? Value { get; set; }

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

    public class WarningLineModel
    {
        public int PKID { get; set; }
        public decimal MinGuidePrice { get; set; }
        public decimal MaxGuidePrice { get; set; }
        public decimal LowerLimit { get; set; }
        public decimal UpperLimit { get; set; }
    }
    public class GuideViewModel
    {
       public IEnumerable<WarningLineModel> Warn { get; set; }
        public IEnumerable<GuidePara> Para { get; set; }
    }
    public class TireListModel
    {
        public string Brand { get; set; }
        public string PID { get; set; }
        public string ProductName { get; set; }
        public int JiaQuan { get; set; }
        public int? totalstock { get; set; }

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

        public decimal? JDFlagShipPrice { get; set; }
        public string JDFlagShipPID { get; set; }

        public decimal? JDSelfPrice { get; set; }
        public string JDSelfPID { get; set; }
        public decimal? JDPlus { get; set; }
        public decimal? TWLTMPrice { get; set; }
        public string TWLTMPID { get; set; }

        public decimal? MLTTMPrice { get; set; }
        public string MLTTMPID { get; set; }

        public decimal? MLTPrice { get; set; }
        public string MLTPID { get; set; }
        public decimal UpperLimit { get; set; }
        public decimal LowerLimit { get; set; }

        public decimal SingleCustomPrice { get; set; }
        /// <summary>
        /// 券后价
        /// </summary>
        public decimal? CouponPrice { get; set; }
        /// <summary>
        /// 最低活动价
        /// </summary>
        public decimal? ActivePrice { get; set; }

        public int ShowStatus { get; set; }

        /// <summary>
        /// 采购在途
        /// </summary>
        public int? CaigouZaitu { get; set; }
        /// <summary>
        /// 全网最低价
        /// </summary>
        public decimal MinPrice { get; set; }
    }
    public class PriceSelectModel
    {
        public string ProductName { get; set; }

        public string PID { get; set; }

        public string Brand { get; set; }
        /// <summary>
        /// 花纹
        /// </summary>
        public string Patterns { get; set; }
        public string TireSize { get; set; }
        public string Rims { get; set; }

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

        public decimal? PCPricePer { get; set; }
        public double MatchPCPricePer { set; get; }

        public int MatchWarnLine { get; set; }
        public int MatchStock { get; set; }
        public int MatchStockValue { get; set; }
        public int MatchZZTS { get; set; }
        public double MatchZZTSValue { get; set; }

        public string SingleCustomPrice { get; set; }
        public string ManyCustomPrice { get; set; }

        public int MonthCount { get; set; }
        public int MonthCountValue { get; set; }
        public int WeekCount { get; set; }
        public int WeekCountValue { get; set; }
        public int ShowStatus { get; set; }
        public int VehicleCount { get; set; }
        public int VehicleCountValue { get; set; }
    }

    public class Product_Warehouse
    {
        public string pid { get; set; }
        public string warehousename { get; set; }

        public int? stocknum { get; set; }

        public int? num_week { get; set; }

        public int? num_month { get; set; }
        public int? num_threemonth { get; set; }
        public int? CaigouZaitu { get; set; }
    }

    public class ExamUpdatePriceModel
    {
        public int PKID { get; set; }
        public string PID { get; set; }
        public decimal Price { get; set; }
        public string Reason { get; set; }
        public DateTime ApplyDateTime { get; set; }
        public string ApplyPerson { get; set; }
        public DateTime ExamDateTime { get; set; }
        public string ExamPerson { get; set; }
        /// <summary>
        /// 0 待审批 1审批通过 2审批失败 3被覆盖
        /// </summary>
        public int ExamStatus { get; set; }

        public string Brand { get; set; }

        public string ProductName { get; set; }

        public decimal? cost { get; set; }

        public int JiaQuan { get; set; }

        public decimal ListPrice { get; set; }

        public decimal? JDSelfPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public int? totalstock { get; set; }
        public int? num_week { get; set; }
        public int? num_month { get; set; }
        public decimal? guidePrice { get; set; }
        public decimal? nowPrice { get; set; }
        public double? maoliLv { get; set; }
        public double? chaochu { get; set; }
        public decimal? jdself { get; set; }
        public decimal? maolie { get; set; }

        public string Remark { get; set; }
    }

    public class UpdatePriceBitchReasultModel
    {
        public string Message { get; set; }
        /// <summary>
        /// -99提示错误 直接return  -1无指导价浮动10% 确认修改？  1 保存成功,申请改价 2 保存成功 -2保存失败
        /// </summary>
        public int Status { get; set; }
    }

    public class PriceChangeLog
    {
        public string PID { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime ChangeDateTime { get; set; }
        public string ChangeUser { get; set; }
        public string ChangeReason { get; set; }

        public string ChangeDateStr { get; set; }

    }

    public class ZXTCost
    {
        public string PID { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public string WareHouse { get; set; }
        public decimal CostPrice { get; set; }
        public int Num { get; set; }
    }

    public class TireSizeSalesQuantity
    {
        public string TireSize { get; set; }
        public string SalesQuantity { get; set; }
    }


    public class ActivePriceModel
    {
        public string PID { get; set; }
        public decimal? ActivePrice { get; set; }
        public int? CaigouZaitu { get; set; }
    }

    public class ShowStatusModel
    {
        public string PID { get; set; }
        public int ShowStatus { get; set; }
    }
    public class CaigouZaituModel
    {
        public string PID { get; set; }
        public int? CaigouZaitu { get; set; }
    }
}
