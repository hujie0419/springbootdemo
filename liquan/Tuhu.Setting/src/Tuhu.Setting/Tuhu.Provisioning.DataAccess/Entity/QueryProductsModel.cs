using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 产品列表查询对象
    /// </summary>
    public class SeachProducts
    {
        public SeachProducts()
        {
            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            if (PageSize <= 0)
            {
                PageSize = 100;
            }
        }
        /// <summary>
        /// 品类 默认为1
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tab { get; set; }

        /// <summary>
        /// 尺寸
        /// </summary>
        public string Rim { get; set; }

        /// <summary>
        /// 优惠券Id集合
        /// </summary>
        public string CouponIds { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// 起始价格
        /// </summary>
        public int? BeginPrice { get; set; }

        /// <summary>
        /// 结束价格
        /// </summary>
        public int? EndPrice { get; set; }

        /// <summary>
        /// 产品Id
        /// </summary>
        public string PId { get; set; }

        /// <summary>
        /// 产品Id 英文逗号分割
        /// </summary>
        public List<string> PidList { get; set; }

        /// <summary>
        /// 花纹
        /// </summary>
        public string Pattern { get; set; }
        
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 商品状态 1 上架 0 下架 
        /// </summary>
        public int? OnSale { get; set; }

        /// <summary>
        /// 毛利
        /// </summary>
        public string Maoli { get; set; }

        /// <summary>
        /// 毛利排序
        /// </summary>
        public string MaoliSort { get; set; }

        /// <summary>
        /// 商品展示 -1:全部 1:显示 0:隐藏
        /// </summary>
        public int? IsShow { get; set; }
            
        /// <summary>
        /// 花费价格
        /// </summary>
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

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string CP_Brand { get; set; }
        public string CP_Tab { get; set; }

        /// <summary>
        /// 广告语
        /// </summary>
        public string CP_ShuXing5 { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool OnSale { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 花纹
        /// </summary>
        public string CP_Tire_Pattern { get; set; }

        /// <summary>
        /// 尺寸
        /// </summary>
        public string CP_Tire_Rim { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal cy_list_price { get; set; }

        /// <summary>
        /// 销售价
        /// </summary>
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