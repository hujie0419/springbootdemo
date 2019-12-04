using System;
using System.Collections.Generic;
using System.Text;

namespace Tuhu.Service.Promotion.DataAccess.Entity
{
    /// <summary>
    /// 优惠券任务已发记录表
    /// </summary>
    public class PromotionSingleTaskUsersHistoryEntity
    {
        /// <summary>
		/// 优惠券2.0已发列表主键
        /// </summary>		
        public int PromotionSingleTaskUsersHistoryId { get; set; }
        /// <summary>
        /// 优惠券2.0 已发表外键对应 tbl_PromotionTask 主键
        /// </summary>		
        public int PromotionTaskId { get; set; }
        /// <summary>
        /// 发券的手机号
        /// </summary>		
        public string UserCellPhone { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>		
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 【没用】是否已推送
        /// </summary>		
        public int SendState { get; set; }
        /// <summary>
        /// 来源订单号
        /// </summary>		
        public string OrderNo { get; set; }
        /// <summary>
        /// 待发pkid
        /// </summary>		
        public int PromotionSingleTaskUsersPKID { get; set; }
        /// <summary>
        /// 是否成功【默认1发送成功，2发送失败】
        /// </summary>		
        public int IsSuccess { get; set; }
        /// <summary>
        /// 备注【记录错误信息或者不可发送原因】
        /// </summary>		
        public string Message { get; set; }
        /// <summary>
        /// 发券成功的优惠券id【可能存在多个】
        /// </summary>		
        public string PromotionCodeIDs { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserID { get; set; }
    }
}
