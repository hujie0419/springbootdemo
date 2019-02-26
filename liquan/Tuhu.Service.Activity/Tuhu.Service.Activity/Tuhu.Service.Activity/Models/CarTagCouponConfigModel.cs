using System;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class CarTagCouponConfigModel : BaseModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 车型优惠名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 优惠券的GUID
        /// </summary>
        public string CouponGuid { get; set; }

        /// <summary>
        /// 面额
        /// </summary>
        public decimal? Discount { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 使用条件
        /// </summary>
        public decimal? MinMoney { get; set; }

        /// <summary>
        /// 可用不可用，默认可用
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public String ImageUrl { get; set; }
    }
}
