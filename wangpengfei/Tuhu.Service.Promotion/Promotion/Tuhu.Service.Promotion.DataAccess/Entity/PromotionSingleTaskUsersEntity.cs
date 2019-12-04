using System;
using System.Collections.Generic;
using System.Text;

namespace Tuhu.Service.Promotion.DataAccess.Entity
{
    /// <summary>
    /// 优惠券2.0用户待发表
    /// </summary>
    public class PromotionSingleTaskUsersEntity
    {
        /// <summary>
        /// 优惠券2.0待发表主键
        /// </summary>		
        public int PromotionSingleTaskUsersId { get; set; }
        /// <summary>
        /// 优惠券2.0 待发表外键对应 tbl_PromotionTask 主键
        /// </summary>		
        public int PromotionTaskId { get; set; }
        /// <summary>
        /// 要发券的手机号
        /// </summary>		
        public string UserCellPhone { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>		
        public DateTime CeateTime { get; set; }
        /// <summary>
        /// 从哪个订单来的
        /// </summary>		
        public string OrderNo { get; set; }
    }
}
