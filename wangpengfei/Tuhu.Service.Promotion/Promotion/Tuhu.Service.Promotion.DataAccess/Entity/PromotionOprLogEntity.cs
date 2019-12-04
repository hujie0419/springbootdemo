using System;
using System.Collections.Generic;
using System.Text;

namespace Tuhu.Service.Promotion.DataAccess.Entity
{
    /// <summary>
    /// 优惠券日志
    /// </summary>
    public class PromotionOprLogEntity
    {
        public int PKID { get; set; }
        /// <summary>
        /// 优惠券id
        /// </summary>
        public int PromotionPKID { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 操作类型 
        /// </summary>
        public string Operation { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Referer { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 领取规则id
        /// </summary>
        public int GetRuleID { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        ///设备id
        /// </summary>
        public string DeviceID { get; set; }
        /// <summary>
        /// 操作详情
        /// </summary>
        public string OperationDetail { get; set; }
        /// <summary>
        /// 优惠券类型
        /// </summary>
        public int CouponType { get; set; }

    }
}
