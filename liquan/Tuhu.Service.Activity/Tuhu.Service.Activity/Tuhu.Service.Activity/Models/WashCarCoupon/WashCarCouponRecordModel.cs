using System;

namespace Tuhu.Service.Activity.Models
{
    /// <summary>
    /// 一分钱洗车优惠券  [业务层 对象]
    /// </summary>
    public class WashCarCouponRecordModel
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// 车id
        /// </summary>
        public Guid CarID { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车型id
        /// </summary>
        public string VehicleID { get; set; }

        /// <summary>
        /// 车型
        /// </summary>
        public string Vehicle { get; set; }

        /// <summary>
        /// 排量
        /// </summary>
        public string PaiLiang { get; set; }
        /// <summary>
        /// 年
        /// </summary>
        public string Nian { get; set; }
        /// <summary>
        /// Tid
        /// </summary>
        public string Tid { get; set; }

        /// <summary>
        /// 优惠券id
        /// </summary>
        public int PromotionCodeID { get; set; }


        //public DateTime CreateDateTime { get; set; }
        //public DateTime LastUpdateDateTime { get; set; }
    }
}

