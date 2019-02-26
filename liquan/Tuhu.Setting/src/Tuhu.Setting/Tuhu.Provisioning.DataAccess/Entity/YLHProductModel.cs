using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class YLHProductModel
    {
        /// <summary>
        /// 商品编号（外键）
        /// </summary>
        public int? oid { get; set; }
        /// <summary>
        /// 门店名
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 大类
        /// </summary>
        [Column("[1stProductType]")]
        public string ProductType1St { get; set; }
        /// <summary>
        /// 中类
        /// </summary>
        [Column("[2ndProductType]")]
        public string ProductType2Nd { get; set; }
        /// <summary>
        /// 小类
        /// </summary>
        [Column("[3rdProductType]")]
        public string ProductType3Rd { get; set; }
        /// <summary>
        /// 细类
        /// </summary>
        [Column("[4thProdictType]")]
        public string ProductType4Th { get; set; }
        /// <summary>
        /// 细细类
        /// </summary>
        [Column("[5thProductType]")]
        public string ProductType5Th { get; set; }
        /// <summary>
        /// counter_id
        /// </summary>
        public int? counter_id { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductNunber { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 系统数量
        /// </summary>
        public float? SystemQuantity { get; set; }
       /// <summary>
       /// 系统金额
       /// </summary>
        public decimal SyetemSettlement { get; set; }
        /// <summary>
        /// 实盘数量
        /// </summary>
        public float? RealQuantity { get; set; }
        /// <summary>
        /// 实盘金额
        /// </summary>
        public decimal? RealSettlement { get; set; }
        /// <summary>
        /// 实盘差异
        /// </summary>
        public int QuantityDiff { get; set; }
        /// <summary>
        /// 差异金额
        /// </summary>
        public decimal? SettlementDiff { get; set; }
        /// <summary>
        /// 原因
        /// </summary>
        public string DiffReason { get; set; }
        /// <summary>
        /// 最后进货日
        /// </summary>
        public DateTime? LastPurchaseDate { get; set; }
        /// <summary>
        /// 库龄年
        /// </summary>
        public int? YearInWareHouse { get; set; }
        /// <summary>
        /// 库龄天
        /// </summary>
        public int? DayInWareHouse { get; set; }
        /// <summary>
        /// 铺货金额
        /// </summary>
        public decimal? DistributionAmount { get; set; }
        /// <summary>
        /// 买断金额
        /// </summary>
        public decimal? BuyoutAmount { get; set; }
        /// <summary>
        /// 全仓月均销量
        /// </summary>
        public float? MonthlySales { get; set; }
        /// <summary>
        /// 商品分类
        /// </summary>
        public string QualityClassification { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public decimal? cy_list_price { get; set; }

        public int? MonthInWareHouse { get; set; }

        public string PID { get; set; }

    }
}
