using System;
using System.Collections.Generic;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    public class RecommendGetGiftConfigModel : BaseModel
    {
        public int Id { get; set; }

        public Guid Number { get; set; }

        public string Name { get; set; }

        public string Banner { get; set; }

        public double? AwardLimit { get; set; }

        public string AwardType { get; set; }

        public double? AwardValue { get; set; }

        public string RegisteredText { get; set; }

        public string AwardedText { get; set; }

        public string ShareButtonValue { get; set; }

        public string ShareChannel { get; set; }

        public string Rules { get; set; }

        public string TimeLimitCollectRules { get; set; }

        public string CreateName { get; set; }

        public DateTime? CreateTime { get; set; }

        public string UpdateName { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string GetRuleGUID { get; set; }

        public string TabName { get; set; }
        public bool IsSendCode { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public IEnumerable<SendCodeForUserGroupModel> SendCodes { get; set; }
    }

    public class SendCodeForUserGroupModel : BaseModel
    {
        public int ID { get; set; }
        public int GroupId { get; set; }
        public string SendCode { get; set; }
        public Guid UserId { get; set; }
        public Guid? GetUserId { get; set; }
        public string GetUserName { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public DateTime CodeStartTime { get; set; }
        public DateTime CodeEndTime { get; set; }
        public string UserPhone { get; set; }
        public bool IsGet => GetUserId != null;

        public bool IsExpire => CodeEndTime <= DateTime.Now;
    }
}
