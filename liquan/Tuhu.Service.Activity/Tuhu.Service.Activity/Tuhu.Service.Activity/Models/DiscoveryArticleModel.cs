using System;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models
{
    public class DiscoveryArticleModel
    {
        public int Number { set; get; }
        public int AttentionCount { set; get; }

        public int PKID { set; get; }

        public int DISTINCTID { set; get; }

        #region Order By

        public DateTime OperateTime { set; get; }

        #endregion Order By

        #region Where AnswerUser

        public List<string> AnswerUser { set; get; }

        public int AuditStatus { set; get; }

        #endregion Where AnswerUser

        public string Content { set; get; }

        public string SmallTitle { set; get; }

        public string BigTitle { set; get; }

        public string ShowImages { set; get; }

        public int ShowType { set; get; }

        public string AnswerContent { set; get; }

        public int AnswerId { set; get; }

        public int Praise { set; get; }

        public int CommentTimes { set; get; }

        public string AttentionUserIds { set; get; }

        public string CategoryTags { set; get; }

        public int ClickCount { set; get; }

        public string Title { set; get; }

        public string ContentUrl { set; get; }

        public int VoteNUm { set; get; }

        public string Brief { set; get; }

        public string CategoryImage { set; get; }

        public string CategoryName { set; get; }

        public int CategoryId { set; get; }

        public int TYPE { set; get; }

        public string UserHead { set; get; }

        public string CreatorInfo { set; get; }

        public string SmallImage { set; get; }

        public string CommentImage { set; get; }

        public int RelatedArticleId { set; get; }

        public int ImagesCount { set; get; }
    }



    public class DiscoveryArticleAnswerUser
    {
        public string UserID { set; get; }
        public int CategoryId { set; get; }
        public int PKID { set; get; }
        public string AttentionUserId { set; get; }
        public int? UserIdentity { set; get; }
    }
}