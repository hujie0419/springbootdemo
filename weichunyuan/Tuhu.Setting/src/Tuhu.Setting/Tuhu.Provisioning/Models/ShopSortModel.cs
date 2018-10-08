using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Service.Shop.Models;

namespace Tuhu.Provisioning.Models
{
    public class ShopSortModel
    {
        public int ShopId { get; set; }
        public string Brand { get; set; }
        /// <summary>
        /// 店类型
        /// <results>门店类型划分( 维修厂、4S店、快修店)</results>
        /// </summary>
        public string ShopClassification { get; set; }
        /// <summary>
        /// 服务类型
        /// <results>BY,TR,MR,PQ,GZ,FW</results>
        /// </summary>
        public string ServiceType { get; set; }
        public int ShopType { get; set; }
        /// <summary>
        /// 对外店名
        /// </summary>
        public string CarparName { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }
        /// <summary>
        /// 置顶距离
        /// </summary>
        public int ShopRang { get; set; }
        /// <summary>
        /// 推荐距离
        /// </summary>
        public int ShopRecommendRange { get; set; }
        public string[] Images { get; set; }
        /// <summary>
        /// 轮保等级
        /// </summary>
        public int? ShopLevel { get; set; }
        public double? ShopLat { get; set; }
        public double? ShopLng { get; set; }
        public double  Distance { get; set; }
        /// <summary>
        /// BI权重
        /// </summary>
        public decimal DistanceWeight { get; set; }
        /// <summary>
        /// BI分值
        /// </summary>
        public decimal Grade { get; set; }
        /// <summary>
        /// BI综合分值
        /// </summary>
        public decimal? ComprehensiveScore { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public ShopCoverRegionModel CoverRegion { get; set; }
        /// <summary>
        /// 综合评价
        /// </summary>
        public decimal CommentRate { get; set; }
        /// <summary>
        /// 评价数量
        /// </summary>
        public decimal  CommentTimes { get; set; }
        /// <summary>
        /// 订单数量
        /// </summary>
        public int InstallQuantity { get; set; }
        public bool IsSuspend { get; set; }
    }
}