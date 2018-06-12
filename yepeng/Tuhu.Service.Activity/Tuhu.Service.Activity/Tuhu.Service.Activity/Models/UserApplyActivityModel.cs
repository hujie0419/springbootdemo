using System;
using Tuhu.Service.Activity.Enum;

namespace Tuhu.Service.Activity.Models
{
    public class UserApplyActivityModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 活动id
        /// </summary>
        public Guid ActivityId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNum { get; set; }
        /// <summary>
        /// 驾驶证号
        /// </summary>
        public string DriverNum { get; set; }
        /// <summary>
        /// ip地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public AuditStatus Status { get; set; } = AuditStatus.Pending;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 服务码
        /// </summary>
        public Guid ServiceCode { get; set; }
        /// <summary>
        /// 服务码过期时间
        /// </summary>
        public DateTime ExpirationTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedDateTime { get; set; }
    }
}
