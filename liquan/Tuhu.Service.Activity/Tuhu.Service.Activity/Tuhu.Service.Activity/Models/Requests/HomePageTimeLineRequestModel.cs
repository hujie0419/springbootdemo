using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class HomePageTimeLineRequestModel
    {
        public int PKID { get; set; }

        public string ArticleShowMode { get; set; }
        public int RelatedArticleId { get; set; }
        //[ScriptIgnore]
        public int DistinctId { get; set; }
        public string Content { get; set; }
        public string SmallTitle { get; set; }
        public string BigTitle { get; set; }

        public string SmallImage { get; set; }
        public string ShowImages { get; set; }
        public int ShowType { get; set; }
        public string AnswerContent { get; set; }
        public int AnswerId { get; set; }
        public int Praise { get; set; }
        public int CommentTimes { get; set; }

        public string CommentImage { get; set; }
        public string PublishNewDateTime { get { return GetShowTime(OperateTime); } }

        //[ScriptIgnore]
        public DateTime OperateTime { get; set; }
        public int AttentionCount { get; set; }
        public string FirstAttentionUserId { get; set; }
        public string AttentionUserIds { get; set; }
        public string AttentionUserNames { get; set; }
        public string AttentionUserHead { get; set; }

        public string UserHead { get; set; }

        public string CreatorInfo { get; set; }

        public string CategoryTags { get; set; }
        public int ClickCount { get; set; }

        public string ContentUrl { get; set; }
        public int VoteNum { get; set; }

        public int Vote { get { return VoteNum; } }
        public string Brief { get; set; }
        public string CategoryImage { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int Type { get; set; }

        public int ImagesCount { set; get; }
        public HomePageTimeLineRequestModel() { }

        public static string GetShowTime(DateTime orginalDateTime)
        {
            //1分钟之内
            if ((DateTime.Now - orginalDateTime).TotalSeconds <= 59)
            {
                return Math.Floor((DateTime.Now - orginalDateTime).TotalSeconds).ToString() + "秒前";
            }
            //1小时之内
            else if ((DateTime.Now - orginalDateTime).TotalHours < 1)
            {
                return Math.Floor((DateTime.Now - orginalDateTime).TotalMinutes).ToString() + "分钟前";
            }
            //1天之内
            else if ((DateTime.Now - orginalDateTime).TotalDays < 1)
            {
                return Math.Floor((DateTime.Now - orginalDateTime).TotalHours).ToString() + "小时前";
            }
            //1天之外
            else if ((DateTime.Now - orginalDateTime).TotalDays > 1)
            {
                return orginalDateTime.ToString("yyyy/MM/dd");
            }
            return orginalDateTime.ToString("yyyy/MM/dd");
        }
    }
}
