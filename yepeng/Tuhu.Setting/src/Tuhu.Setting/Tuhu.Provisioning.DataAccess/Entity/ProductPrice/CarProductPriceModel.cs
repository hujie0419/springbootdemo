using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.ProductPrice
{
   public class CarProductPriceModel
    {
        public string ProductName { get; set; }
        public string PID { get; set; }


        public int Oid { get; set; }

        /// <summary>
        /// 天天秒杀价格
        /// </summary>
        public decimal DaydaySeckillPrice { get; set; }

        /// <summary>
        /// 拼团价格
        /// </summary>
        public decimal PintuanPrice { get; set; }

        /// <summary>
        /// 限时抢购价
        /// </summary>
        public decimal FlashSalePrice { get; set; }

        /// <summary>
        /// 官网原价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 采购价
        /// </summary>
        public decimal CaigouPrice { get; set; }

        /// <summary>
        /// 是否代发
        /// </summary>
        public bool IsDaifa { get; set; }

        /// <summary>
        /// 上海保养仓在途库存
        /// </summary>
        public int SH_ZaituStockQuantity { get; set; }

        /// <summary>
        /// 上海保养仓可用库存
        /// </summary>
        public int SH_AvailableStockQuantity { get; set; }

        /// <summary>
        /// 武汉保养仓在途库存
        /// </summary>
        public int WH_ZaituStockQuantity { get; set; }

        /// <summary>
        /// 武汉保养仓可用库存
        /// </summary>
        public int WH_AvailableStockQuantity { get; set; }

        /// <summary>
        /// 义乌保养仓在途库存
        /// </summary>
        public int YW_ZaituStockQuantity { get; set; }

        /// <summary>
        /// 义乌保养仓可用库存
        /// </summary>
        public int YW_AvailableStockQuantity { get; set; }

        /// <summary>
        /// 广州保养仓在途库存
        /// </summary>
        public int GZ_ZaituStockQuantity { get; set; }

        /// <summary>
        /// 广州保养仓可用库存
        /// </summary>
        public int GZ_AvailableStockQuantity { get; set; }

        /// <summary>
        /// 北京保养仓在途库存
        /// </summary>
        public int BJ_ZaituStockQuantity { get; set; }

        /// <summary>
        /// 北京保养仓可用库存
        /// </summary>
        public int BJ_AvailableStockQuantity { get; set; }

        /// <summary>
        /// 所有在途库存
        /// </summary>
        public int TotalZaituStockQuantity { get; set; }

        /// <summary>
        /// 所有可用库存
        /// </summary>
        public int TotalAvailableStockQuantity { get; set; }

        /// <summary>
        /// 是否上架
        /// </summary>
        public bool OnSale { get; set; }

        /// <summary>
        /// 是否缺货
        /// </summary>
        public bool StockOut { get; set; }

        /// <summary>
        /// 历史天天秒杀价格
        /// </summary>
        public List<ActivityPriceModel> DaydaySeckillPriceList { get; set; }

        /// <summary>
        /// 历史拼团价格
        /// </summary>
        public List<ActivityPriceModel> PintuanPriceList { get; set; }

        /// <summary>
        /// 历史限时抢购价
        /// </summary>
        public List<ActivityPriceModel> FlashSalePriceList { get; set; }

        /// <summary>
        /// 采购价格
        /// </summary>
        public decimal PurchasePrice { get; set; }

        /// <summary>
        /// 代发价格
        /// </summary>
        public decimal ContractPrice { get; set; }

        /// <summary>
        /// 优惠采购价格
        /// </summary>
        public decimal OfferPurchasePrice { get; set; }

        /// <summary>
        /// 优惠代发价格
        /// </summary>
        public decimal OfferContractPrice { get; set; }


        #region 优惠券相关参数

        /// <summary>
        /// 券后价 [多张 优惠券 只显示最低的]
        /// </summary>
        public decimal UsedCouponPrice { get; set; }

        /// <summary>
        /// 券后毛利 【以券后价为准】
        /// </summary>
        public decimal UsedCouponProfit { get; set; }


        public List<GetCouponRules> Coupons { get; set; }

        #endregion

    }

    public class ActivityPriceModel
    {
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 活动价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 活动id
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// 活动类型 1.天天秒杀 4.限时抢购
        /// </summary>
        public int ActiveType { get; set; }

        /// <summary>
        /// 商品pid
        /// </summary>
        public string PID { get; set; }
    }
}
