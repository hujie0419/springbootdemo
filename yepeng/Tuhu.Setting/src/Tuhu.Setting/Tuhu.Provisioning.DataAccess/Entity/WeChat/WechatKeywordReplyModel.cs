using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.WeChat
{
    public class WechatKeywordReplyModel: WechartReplyBase
    {
        public string RuleName { get; set; }
        public List<KeywordReplyListModel> KeywordReplyList { get; set; }
        public string OriginalID { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEnabled { get; set; }
        public Guid RuleGroup { get; set; }
    }

    public class KeywordReplyListModel
    {
        public long PKID { get; set; }
        public int MatchedPattern { get; set; }
        public string Keyword { get; set; }
    }
}
