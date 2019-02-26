using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Tire
{
    public sealed class StockoutStatusRequest
    {
        public int CityId { get; set; }
        public string TireSize { get; set; }
        public string PID { get; set; }
        public int Status { get; set; }
    }

    public sealed class StockoutStatusModel
    {
        public string PID { get; set; }
        public string TireSize { get; set; }
        public string DisplayName { get; set; }
        public string Status { get; set; }
        public bool Stockout { get; set; }
    }
    public sealed class StockoutStatusWhiteModel
    {
        /// <summary>
        /// 白名单状态
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 上架状态
        /// </summary>
        public bool OnSale { get; set; }
        /// <summary>
        /// isShow状态
        /// </summary>
        public bool IsShow { get; set; }
        /// <summary>
        /// 系统标缺状态(全局)
        /// </summary>
        public string SystemStuckout { get; set; }
        /// <summary>
        /// 人工标缺状态
        /// </summary>
        public bool Stuckout { get; set; }
        public string PID { get; set; }
        public string TireSize { get; set; }
        public string DisplayName { get; set; }
        /// <summary>
        /// 局部缺货状态
        /// </summary>
        public string RegionalStockout { get; set; }
    }

    public sealed class WhiteRequest
    {
        public int CityId { get; set; }
        /// <summary>
        /// 白名单状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 上架状态
        /// </summary>
        public int OnSale { get; set; }
        /// <summary>
        /// 系统标缺状态(全局)
        /// </summary>
        public int SystemStuckout { get; set; }
        /// <summary>
        /// 人工标缺状态
        /// </summary>
        public int Stuckout { get; set; }
        public string PID { get; set; }
        public string TireSize { get; set; }
        public string DisplayName { get; set; }
        /// <summary>
        /// 缺货状态(区域)
        /// </summary>
        public int RegionalStockout { get; set; }
        /// <summary>
        /// 产品显示状态
        /// </summary>
        public int IsShow { get; set; }
    }
}