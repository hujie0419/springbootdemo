using System;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models
{
    public class FlashSaleProductDetailModel

    {
        /// <summary> 产品id </summary>
        public string PID { get; set; }

        /// <summary> 位置 </summary>
        public int? Position { get; set; }

        /// <summary> 活动Id </summary>
        public Guid ActivityID { get; set; }

        /// <summary> 活动价格||拼团团长价 </summary>
        public decimal Price { get; set; }

        /// <summary>伪原价||市场价||促销价*1.3||拼团原价 </summary>
        public decimal MarketingPrice { get; set; }
        /// <summary>限时抢购~~||拼团活动价</summary>
        public decimal FalseOriginalPrice { get; set; }
        public decimal ListPrice { get; set; }

        /// <summary> 产品标签 </summary>
        public string Label { get; set; }

        /// <summary> 库存数 </summary>
        /// <value> The total number of quantity. </value>
        public int TotalQuantity { get; set; }
        /// <summary> 单人限购数量 </summary>
        /// <value> The maximum quantity. </value>
        public int MaxQuantity { get; set; }
        /// <summary> 已售出数量 </summary>
        /// <value> The sale out quantity. </value>
        public int SaleOutQuantity { get; set; }
        public bool IsUsePcode { get; set; }

        /// <summary> 安装/付款方式 </summary>
        public string InstallAndPay { get; set; }

        /// <summary> 展示楼层 </summary>
		public int Level { get; set; }

        /// <summary> 楼层图片 </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 使用优惠券? true可以使用 false不可以使用
        /// </summary>
        public bool IsUsePCode { get; set; }

        /// <summary> 渠道 </summary>
        public string Channel { get; set; }

        /// <summary> 产品图片 </summary>
        public string Image { get; set; }

        /// <summary> 产品名称 </summary>
        public string DisplayName { get; set; }

        public int BYCount { get; set; }

        /// <summary> 全场限制购买数量 </summary>
        public int PlaceQuantity { get; set; }

        /// <summary> 是否参加全场购买限制 </summary>
        public int IsJoinPlace { get; set; }

        /// <summary> 1闪购 </summary>
        public int? ActiveType { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        /// <summary>个人购买数量 </summary>
        public int PersonalSaleQuantity { get; set; }

        /// <summary> 个人购买参加全场限制活动的总数量 </summary>
        public int PersonalSalePlaceQuantity { get; set; }

        /// <summary>库存剩余数量 </summary>
        public int StockQuantity { get { var quantity = TotalQuantity - SaleOutQuantity; return quantity > 0 ? quantity : 0; } }
        public string CP_Remark { get; set; }
        /// <summary>
        /// 限时抢购状态，0：成功，1：渠道不符，2：已售完，3：非活动时间，4：已限购,5:仅限新用户购买
        /// </summary>
        public string FlashSaleCode { get; set; }
        /// <summary>
        /// 指定用户还可以购买指定限时抢购中某产品的数量
        /// </summary>
        public int UserCanBuyCount { get; set; }
        /// <summary>
        /// 打折描述
        /// </summary>
        public List<string> DiscountRules { get; set; }

        public bool OnSale { get; set; }

        public string AdvertiseTitle { get; set; }
        public bool IsShow { get; set; }
    }
}
