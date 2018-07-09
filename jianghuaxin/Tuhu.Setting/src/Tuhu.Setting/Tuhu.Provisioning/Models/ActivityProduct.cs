using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.Models
{
    public class ActivityProduct : BaseModel
    {
        public ActivityProduct() : base() { }
        public ActivityProduct(DataRow row) : base(row) { }
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 产品编号
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; set; }
        /// <summary>
        /// 促销价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 伪原价
        /// </summary>
        public decimal FalseOriginalPrice { get; set; }
        /// <summary>
        /// 总限购数量
        /// </summary>
        public int TotalQuantity { get; set; }
        /// <summary>
        /// 每人限购数量
        /// </summary>
        public int MaxQuantity { get; set; }
        /// <summary>
        /// 剩余数量
        /// </summary>
        public int SaleOutQuantity { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public int? Position { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid ActivityID { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndDateTime { get; set; }

        public string PCodeIDS { get; set; }

        public int Level { get; set; }
        public string InstallAndPay { get; set; }
        public string ImgUrl { get; set; }

        public int IsUsePCode { get; set; }
        public string Channel { get; set; }
        public int ActiveType { get; set; }

        /// <summary>
		/// 网站限时抢购的Banner
		/// </summary>
		public string WebBanner { get; set; }
        /// <summary>
        /// 网站角标
        /// </summary>
        public string WebCornerMark { get; set; }
        /// <summary>
        /// 网站背景色
        /// </summary>
        public string WebBackground { get; set; }
        /// <summary>
        /// 网站底部图片
        /// </summary>
        public string WebOtherPart { get; set; }
        /// <summary>
        /// 会场限购数量
        /// </summary>
        public int? PlaceQuantity { get; set; }
        /// <summary>
        /// 该产品是否参与会场限购
        /// </summary>
        public bool IsJoinPlace { get; set; }
    }
}