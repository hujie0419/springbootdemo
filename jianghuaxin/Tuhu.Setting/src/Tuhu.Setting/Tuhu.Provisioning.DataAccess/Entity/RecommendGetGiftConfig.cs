using System;
using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class RecommendGetGiftConfig
    {
        public int Id { get; set; }

        public Guid Number { get; set; }

        public string Name { get; set; }

        public string Banner { get; set; }

        public float AwardLimit { get; set; }

        public string AwardType { get; set; } = "Coupon";

        public float AwardValue { get; set; }

        public string GetRuleGUID { get; set; }

        public string RegisteredText { get; set; }

        public string AwardedText { get; set; }

        public string ShareButtonValue { get; set; }

        public string ShareChannel { get; set; }

        public string Rules { get; set; }

        public string TimeLimitCollectRules { get; set; }

        public DateTime? CreateTime { get; set; }

        public string CreateName { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string UpdateName { get; set; }

        public List<ShareChannel> Channel { get; set; }

        public string TabName { get; set; }
        public bool? IsSendCode { get; set; }
        public int? UserGroupId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class shareData
    {
        public string shareUrl { get; set; }

        public string shareImage { get; set; }
        public string shareBgImage { get; set; }

        public string shareTitle { get; set; }

        public string shareDescrip { get; set; }

        public int shareSort { get; set; }

        public string shareType { get; set; }

    }

    public class ShareChannel
    {
        public bool status { get; set; }
        public string key { get; set; }
        public shareData data { get; set; }
    }
}
