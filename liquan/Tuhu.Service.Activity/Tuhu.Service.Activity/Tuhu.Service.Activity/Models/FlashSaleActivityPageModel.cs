using System;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models
{
    /// <summary>
    /// 新活动页,活动信息查询实体
    /// </summary>
    public class FlashSaleActivityPageModel
    {
        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid ActivityID { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// 0限时抢购 1闪购 2普通活动 3活动秒杀
        /// </summary>
        public int ActiveType { get; set; }

        public IEnumerable<FlashSaleActivityPageProductModel> Products { get; set; }
    }


    public class FlashSaleActivityPageProductModel
    {
        /// <summary>
        /// 系统主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid ActivityID { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Position { get; set; }

        /// <summary>
        /// 促销价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 总限购
        /// </summary>
        public int? TotalQuantity { get; set; }

        /// <summary>
        /// 个人限购
        /// </summary>
        public int? MaxQuantity { get; set; }

        /// <summary>
        /// 销售数量
        /// </summary>

        public int SaleOutQuantity { get; set; }

        /// <summary>
        /// 活动产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 支付方式/安装方式
        /// </summary>
        public bool InstallAndPay { get; set; }

        /// <summary>
        /// 产品是否使用优惠券
        /// </summary>
        public bool IsUsePCode { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 是否显示 -废弃
        /// </summary>
        [Obsolete("废弃,请使用IsHide")]
        public string IsShow { get; set; }

        /// <summary>
        /// 是否显示 -  
        /// </summary>
        [Obsolete("废弃,请使用IsHide")]
        public bool ProductIsShow { get; set; }

        /// <summary>
        /// 是否隐藏- true:隐藏
        /// </summary>
        public bool IsHide { get; set; }

    }
}
