using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class DefaultSearchConfig
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public string Keywords { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        public string IOSProcessValue { get; set; }

        public string AndroidProcessValue { get; set; }

        public string IOSCommunicationValue { get; set; }

        public string AndroidCommunicationValue { get; set; }

        /// <summary>
        /// 1 默认搜索你 2 业务搜索 3 品牌旗舰店 
        /// </summary>
        public int Type { get; set; }

        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 小程序参数值
        /// </summary>
        public string WXAPPValue { get; set; }

        /// <summary>
        /// 移动站参数值
        /// </summary>
        public string H5Value { get; set; }

        public DateTime? ActivityStartTime { get; set; }
        public DateTime? ActivityEndTime { get; set; }
        public ActivityTypeEnum ActivityType { get; set; }
        public string ActivityPromotionId { get; set; }
    }

    public enum ActivityTypeEnum
    {
        NONE,
        领优惠券,
        活动专场
    }

    public class AnnouncementConfig
    {
        public int PKID { get; set; }

        public string Content { get; set; }

        public string ImageUrl { get; set; }

        public int NoticeType { get; set; }

        public bool IsDeleted { get; set; }

        public string H5Url { get; set; }

        public string IOSProcessValue { get; set; }

        public string AndroidProcessValue { get; set; }

        public string IOSCommunicationValue { get; set; }

        public string AndroidCommunicationValue { get; set; }

        public DateTime? CreateTime { get; set; }

        public string CreatedUser { get; set; }

        public bool Status { get; set; }
    }
}
