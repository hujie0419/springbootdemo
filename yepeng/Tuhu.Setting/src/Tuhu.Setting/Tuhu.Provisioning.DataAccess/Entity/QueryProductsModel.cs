using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class SeachProducts
    {
        public string category { get; set; }
        public string brand { get; set; }
        public string tab { get; set; }
        public string rim { get; set; }
        public string couponIds { get; set; }
        public string price { get; set; }
        public string pid { get; set; }
        public string pattern { get; set; }
        public string soft { get; set; }
        public int pageIndex { get; set; } = 1;
        public int pageSize { get; set; } = 100;
        public string onSale { get; set; }
        public string maoli { get; set; }
        public string maoliSort { get; set; }
        public string isShow { get; set; }

        public string CostPrice { get; set; }

        public string MaoliAfter { get; set; }

        public string SalePriceAfter { get; set; }

        public string FiltrateType { get; set; }

    }
    public class QueryProductsModel
    {
        /// <summary>
        /// 产品OID
        /// </summary>
        public int Oid { get; set; }
        /// <summary>
        /// 优惠券ID
        /// </summary>
        public string CouponIds { get; set; }
        public string DisplayName { get; set; }
        public string CP_Brand { get; set; }
        public string CP_Tab { get; set; }
        public string CP_ShuXing5 { get; set; }
        public string OnSale { get; set; }
        public string PID { get; set; }
        public string CP_Tire_Pattern { get; set; }
        public string CP_Tire_Rim { get; set; }
        public decimal cy_list_price { get; set; }
        public decimal cy_marketing_price { get; set; }
        public decimal cy_cost { get; set; }
        public string Image { get; set; }
        public int PageCount { get; set; }
        public decimal Maoli { get; set; }
        public int IsShow { get; set; }
        /// <summary>
        /// 券后最低售价
        /// </summary>
        public decimal? PriceAfterCoupon { get; set; }
        /// <summary>
        /// 券后最低毛利
        /// </summary>
        public decimal? GrossProfit { get; set; }

        public IEnumerable<UseCouponEffect> UseCouponEffects { get; set; }
    }

    public class UseCouponEffect
    {
        public int ProductCount { get; set; }
        public int CouponPkId { get; set; }

        public string CouponDescription { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal? Discount { get; set; }
        /// <summary>
        /// 满多少元减
        /// </summary>
        public decimal? Minmoney { get; set; }
        /// <summary>
        /// 券后销售价
        /// </summary>
        public decimal? PriceAfterCoupon { get; set; }
        /// <summary>
        /// 券后毛利率
        /// </summary>
        public decimal? GrossProfit { get; set; }

        public string Status { get; set; }
        /// <summary>
        /// 自多少天
        /// </summary>
        public int? CouponDuration { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

    }
}