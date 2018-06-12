using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 产品券后价Model
    /// </summary>
    public class CouponPrice
    {
        public string PID { get; set; }
        /// <summary>
        /// 券后价
        /// </summary>
        public decimal NewPrice { get; set; }
    }

    /// <summary>
    /// 券后价修改日志Model
    /// </summary>
    public class CouponPriceHistory
    {
        public string PID { get; set; }
        /// <summary>
        /// 原有券后价
        /// </summary>
        public decimal? OldPrice { get; set; }
        /// <summary>
        /// 修改后的券后价
        /// </summary>
        public decimal NewPrice { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ChangeDateTime { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string ChangeUser { get; set; }
        /// <summary>
        /// 修改理由
        /// </summary>
        public string ChangeReason { get; set; }
    }

    /// <summary>
    /// 卷后价申请Model
    /// </summary>
    public class PriceApply
    {
        public int? PKID { get; set; }
        public string PID { get; set; }
        /// <summary>
        /// 产品品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 进货价
        /// </summary>
        public decimal CostPrice { get; set; }
        /// <summary>
        /// 最后一次采购价
        /// </summary>
        public decimal? LastCostPrice { get; set; }
        /// <summary>
        /// 当前库存量
        /// </summary>
        public int StockCount { get; set; }
        /// <summary>
        /// 最近七天销量
        /// </summary>
        public int WeekSaleCount { get; set; }
        /// <summary>
        /// 最近三十天销量
        /// </summary>
        public int MonthSaleCount { get; set; }
        /// <summary>
        /// 指导价
        /// </summary>
        public decimal GuidePrice { get; set; }
        /// <summary>
        /// 京东自营价
        /// </summary>
        public decimal? JDPrice { get; set; }
        /// <summary>
        /// 汽车超人零售价
        /// </summary>
        public decimal? CarPrice { get; set; }
        /// <summary>
        /// 官网价格
        /// </summary>
        public decimal? TuhuPrice { get; set; }
        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal GrossProfit { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string PoductName { get; set; }
        /// <summary>
        /// 当前券后价
        /// </summary>
        public decimal? NowPrice { get; set; }
        /// <summary>
        /// 申请券后价
        /// </summary>
        public decimal NewPrice { get; set; }
        /// <summary>
        /// 申请理由
        /// </summary>
        public string ApplyReason { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime ApplyDateTime { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplyPerson { get; set; }
    }

    /// <summary>
    /// 券后价审核记录
    /// </summary>
    public class PriceApproval : PriceApply
    {
        /// <summary>
        /// 审核人
        /// </summary>
        public string ApprovalPerson { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime ApprovalTime { get; set; }
        /// <summary>
        /// 审核结果
        /// </summary>
        public bool ApprovalStatus { get; set; }
    }

    /// <summary>
    /// 卷后价申请Model
    /// </summary>
    public class PriceApplyRequest
    {
        public string PID { get; set; }
        /// <summary>
        /// 产品品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 进货价
        /// </summary>
        public decimal CostPrice { get; set; }
        /// <summary>
        /// 最后一次采购价
        /// </summary>
        public decimal? LastCostPrice { get; set; }
        /// <summary>
        /// 当前库存量
        /// </summary>
        public int StockCount { get; set; }
        /// <summary>
        /// 最近七天销量
        /// </summary>
        public int WeekSaleCount { get; set; }
        /// <summary>
        /// 最近三十天销量
        /// </summary>
        public int MonthSaleCount { get; set; }
        /// <summary>
        /// 指导价
        /// </summary>
        public decimal GuidePrice { get; set; }
        /// <summary>
        /// 京东自营价
        /// </summary>
        public decimal? JDPrice { get; set; }
        /// <summary>
        /// 汽车超人零售价
        /// </summary>
        public decimal? CarPrice { get; set; }
        /// <summary>
        /// 官网价格
        /// </summary>
        public decimal? TuhuPrice { get; set;}
        /// <summary>
        /// 毛利额
        /// </summary>
        public decimal GrossProfit { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string PoductName { get; set; }
        /// <summary>
        /// 当前券后价
        /// </summary>
        public decimal? NowPrice { get; set; }
        /// <summary>
        /// 申请券后价
        /// </summary>
        public decimal NewPrice { get; set; }
        /// <summary>
        /// 申请理由
        /// </summary>
        public string ApplyReason { get; set; }
    }

    public class CouponBlackItem
    {
        public string PhoneNum { get; set; }
        public DateTime CreateDateTime { get; set; }
    }


    public class TireBlackListItem
    {
        public string BlackNumber { get; set; }
        public int Type { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class TireBlackListLog: TireBlackListItem
    {
        public string Operator { get; set; }
    }
}