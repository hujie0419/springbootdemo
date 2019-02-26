using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.Models
{
    public class ActivityModel:BaseModel
    {
        public ActivityModel() : base() { }
        public ActivityModel(DataRow row) : base(row) { }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid ActivityID { get; set; }
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
        /// 活动开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndDateTime { get; set; }
		/// <summary>
		/// 活动类型  null或0表示限时抢购 1表示闪购
		/// </summary>
		public int ActiveType { get; set; }
        /// <summary>
        /// 活动产品
        /// </summary>
        public IEnumerable<ActivityProduct> Products { get; set; }

        public string PCodeIDS { get; set; }
        /// <summary>
        /// 会场限购数量
        /// </summary>
        public int? PlaceQuantity { get; set; }

    }
}