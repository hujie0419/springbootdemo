using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class TireCouponModel
    {
        public int PKID { get; set; }
        public string ShopName { get; set; }
        /// <summary>
        /// 1-A类优惠券;2-B类优惠券;3-C类优惠券
        /// </summary>
        public int CouponType { get; set; }
        /// <summary>
        /// 0-满A减B;1-每满A减B
        /// </summary>
        public int CouponUseRule { get; set; }
        public string Description
        {
            get
            {
                return (CouponUseRule == 0 ? "满" : "每满") + QualifiedPrice.ToString("0") + "减" + Reduce.ToString("0");
            }
        }
        public decimal QualifiedPrice { get; set; }
        public decimal Reduce { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class TireCouponResultModel
    {
        public string ShopName { get; set; }
        public List<TireCouponModel> CouponA { get; set; } = new List<TireCouponModel>();
        public List<TireCouponModel> CouponB { get; set; } = new List<TireCouponModel>();
        public List<TireCouponModel> CouponC { get; set; } = new List<TireCouponModel>();
    }

    public class TireCouponLogModel
    {
        public string Operator { get; set; }
        public string OperateType { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string CouponName { get; set; }
        public DateTime CouponEndTime { get; set; }
        public string ShopName { get; set; }
    }

    public class LowestLimitLogModel
    {
        public string PID { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public string Operator { get; set; }
        public DateTime CreateDateTime { set; get; }
    }
    public class ProductListRequest
    {
        public string ProductName { get; set; }
        public string PID { get; set; }
        public string Brand { get; set; }
        public int OnSale { get; set; }
        public int Type { get; set; }
        /// <summary>
        /// 其他比较字段，使用"|"分割
        /// </summary>
        public string ManyCustomPrice { get; set; }
        /// <summary>
        /// -1：小于等于；1：大于等于
        /// </summary>
        public int Contrast { get; set; }
        /// <summary>
        /// 相对字段
        /// </summary>
        public string SingleCustomPrice { get; set; }

        public int Min_maoliStatus { get; set; }
        public string Min_maoliValue { get; set; }

        public int Website_maoliStatus { get; set; }
        public string Website_maoliValue { get; set; }
        /// <summary>
        /// 其他比较比率
        /// </summary>
        public double Proportion { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int MaxTireCount { get; set; } = 5;
    }

    public class ProductPriceModel
    {
        public string Brand { get; set; }
        public string ProductName { get; set; }
        public string PID { get; set; }
        public decimal? Cost { get; set; }
        public decimal? LowestLimit { get; set; }
        public decimal? Min_maoliValue { get; set; }
        public decimal? LowestPrice { get; set; }
        public decimal? Price { get; set; }
        public decimal? TBPrice { get; set; }
        public decimal? TBLowestPrice { get; set; }
        public string TBPID { get; set; }
        public decimal? TB2LowestPrice { get; set; }
        public decimal? TB2Price { get; set; }
        public string TB2PID { get; set; }
        public decimal? TM1Price { get; set; }
        public decimal? TM1LowestPrice { get; set; }
        public string TM1PID { get; set; }
        public decimal? TM2Price { get; set; }
        public decimal? TM2LowestPrice { get; set; }
        public string TM2PID { get; set; }
        public decimal? TM3Price { get; set; }
        public decimal? TM3LowestPrice { get; set; }
        public string TM3PID { get; set; }
        public decimal? TM4Price { get; set; }
        public decimal? TM4LowestPrice { get; set; }
        public string TM4PID { get; set; }
        public decimal? JDFlagShipPrice { get; set; }
        public decimal? JDFlagShipLowestPrice { get; set; }
        public string JDFlagShipPID { get; set; }
        public decimal? JDPrice { get; set; }
        public decimal? JDLowestPrice { get; set; }
        public bool? CanUseCoupon { get; set; }
        public decimal? WebSiteCouponPrice
        {
            get
            {
                if (LowestPrice.HasValue && Cost.HasValue)
                {
                    return LowestPrice.Value - Cost.Value;
                }
                return null;
            }
        }
        public string JDPID { get; set; }
    }

}