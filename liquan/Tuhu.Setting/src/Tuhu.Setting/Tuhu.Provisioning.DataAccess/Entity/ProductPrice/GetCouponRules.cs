using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.ProductPrice
{
    public class GetCouponRules
    {
        public string RuleID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Minmoney { get; set; }
        public string Discount { get; set; }
        public string CouponStartTime { get; set; }
        public string CouponEndTime { get; set; }
        public string CouponDuration { get; set; }
        public string Quantity { get; set; }
        public string PKID { get; set; }
        public string GetRuleGUID { get; set; }


        #region 车品相关参数

        public bool IsUseful { get; set; }

        private decimal _usedcouponprice;
        /// <summary>
        /// 券后价 [多张 优惠券 只显示最低的]
        /// </summary>
        public decimal UsedCouponPrice
        {
            get
            {
                return Convert.ToDecimal(_usedcouponprice.ToString("f2"));
            }
            set { _usedcouponprice = value; }
        }

        private decimal _usedcouponprofit;
        /// <summary>
        /// 券后毛利 【以券后价为准】
        /// </summary>
        public decimal UsedCouponProfit
        {
            get
            {
                return  Convert.ToDecimal(_usedcouponprofit.ToString("f2"));
            }
            set { _usedcouponprofit = value; }
        }


        public string UsedCouponPriceFormate
        {
            get {
                return this.UsedCouponPrice.ToString();
            }
        }

        public string UsedCouponProfitFormate
        {
            get
            {
                return this.UsedCouponProfit.ToString();
            }
        }

        #endregion

    }
}
