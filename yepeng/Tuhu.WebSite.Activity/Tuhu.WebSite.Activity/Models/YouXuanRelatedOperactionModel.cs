using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.WebSite.Web.Activity.Models
{
    public class YouXuanRelatedOperactionModel
    {
        public int PKID { get; set; }
        public Guid UserId { get; set; }
        public int ArticleId { get; set; }
        public bool Vote { get; set; }
        public bool Favorite { get; set; }
    }
    public class YouXuantListModel 
    {
        public int Pkid { get; set; }
        public string SmallTitle { get; set; }
        public string BigTitle { get; set; }
        public DateTime? PublishDateTime { get; set; }
        public int ClickCount { get; set; }
        public int VoteCount { get; set; }
        public int PraiseCount { get; set; }
        public int ProductCount { get; set; }
        public string CoverType { get; set; }
        public string CoverImg { get; set; }
        public string CoverVideo { get; set; }
        public string Content { get; set; }
    }
    public class YouXuantModel
    {
        public int Pkid { get; set; }
        public string SmallTitle { get; set; }
        public string BigTitle { get; set; }
        public string Brief { get; set; }
        public string Content { get; set; }
        public DateTime? PublishDateTime { get; set; }
        public int ClickCount { get; set; }
        public int VoteCount { get; set; }
        public int PraiseCount { get; set; }
        public int ProductCount { get; set; }
        public string CoverType { get; set; }
        public string CoverImg { get; set; }
        public string CoverVideo { get; set; }
        public string OtherImg { get; set; }
        public string[] OtherImgs
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(OtherImg))
                    return OtherImg.ToString().Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                else
                    return null;
            }
        }
        public bool IsTopMost { get; set; }
    }
}
