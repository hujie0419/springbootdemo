using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Const
{
   public class SalePromotionActivityConst
    {
        #region 是否只读库标识缓存key

        ///促销活动表和促销活动商品表公共标识key
        public const string SalePromotionActivityAndProduct = "SalePromotionActivityAndProduct";

        //促销活动折扣内容表公共标识key
        public const string SalePromotionActivityDiscount = "SalePromotionActivityDiscount";

        //打折活动订单打折记录表公共标识key
        public const string SalePromotionDiscountOrder = "SalePromotionDiscountOrder";

        #endregion
    }
}
