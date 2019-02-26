using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Response
{
    /// <summary>
    /// 拼团轮胎商品配置
    /// </summary>
    public class GroupBuyingTireProductConfigResponse
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 产品配置Id
        /// </summary>
        public int ProductConfigID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 轮胎品牌
        /// </summary>
        public string TireBrand { get; set; }

        /// <summary>
        /// 轮胎花纹
        /// </summary>
        public string TirePattern { get; set; }

        /// <summary>
        /// 轮胎胎面宽
        /// </summary>
        public string TireWidth { get; set; }

        /// <summary>
        /// 轮胎扁平比
        /// </summary>
        public string TireAspectRatio { get; set; }

        /// <summary>
        /// 轮胎直径
        /// </summary>
        public string TireRim { get; set; }

        /// <summary>
        /// 途虎价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 成本价
        /// </summary>
        public decimal CostPrice { get; set; }

        /// <summary>
        /// 拼团价
        /// </summary>
        public decimal FinalPrice { get; set; }

        /// <summary>
        /// 团长价
        /// </summary>
        public decimal SpecialPrice { get; set; }

        /// <summary>
        /// 每人限购单数
        /// </summary>
        public int BuyLimitCount { get; set; }

        /// <summary>
        /// 每单限购数量
        /// </summary>
        public int UpperLimitPerOrder { get; set; }

        /// <summary>
        /// 总库存
        /// </summary>
        public int TotalStockCount { get; set; }

        /// <summary>
        /// 消耗库存
        /// </summary>
        public int CurrentSoldCount { get; set; }

        /// <summary>
        /// 剩余限购
        /// </summary>
        public int UsableStockCount { get; set; }

        /// <summary>
        /// 是否可用券
        /// </summary>
        public bool UseCoupon { get; set; }

        /// <summary>
        /// 是否默认商品
        /// </summary>
        public bool DisPlay { get; set; }

        /// <summary>
        /// 上下架
        /// </summary>
        public bool OnSale { get; set; }
    }
}
