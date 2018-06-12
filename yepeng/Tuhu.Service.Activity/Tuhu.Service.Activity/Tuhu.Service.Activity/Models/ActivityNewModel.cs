using System;
using Tuhu.Service.Activity.Enum;

namespace Tuhu.Service.Activity.Models
{
    public class ActivityNewModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 活动Id
        /// </summary>
        public Guid ActivityId { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 活动类型
        /// </summary>
        public ActivityType ActivityType { get; set; }
        /// <summary>
        /// 活动链接
        /// </summary>
        public string ActivityUrl { get; set; }
        /// <summary>
        /// 配额
        /// </summary>
        public int Quota { get; set; }
        /// <summary>
        /// 已报名用户数量
        /// </summary>
        public int ApplyUserCount { get; set; }
        /// <summary>
        /// 审核通过人数
        /// </summary>
        public int AuditPassUserCount { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string CreateUser { get; set; }
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
