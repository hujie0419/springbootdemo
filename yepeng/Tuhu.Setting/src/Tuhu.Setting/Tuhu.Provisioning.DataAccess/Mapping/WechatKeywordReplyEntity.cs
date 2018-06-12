using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
    public class WechatKeywordReplyEntity : EntityTypeConfiguration<WechatKeywordReplyEntity>
    {
        public WechatKeywordReplyEntity()
        {
            this.HasKey(_ => _.PKID);
            this.ToTable("dbo.WXKeywordReply");
        }

        public long PKID { get; set; }
        public Guid? RuleGroup { get; set; }
        public string Keyword { get; set; }
        public string OriginalID { get; set; }
        private DateTime _createDateTime = DateTime.Now;
        public DateTime CreateDateTime
        {
            get => this._createDateTime;
            set => this._createDateTime = value;
        }

        private DateTime _lastUpdateDateTime = DateTime.Now;
        public DateTime LastUpdateDateTime
        {
            get => this._lastUpdateDateTime;
            set => this._lastUpdateDateTime = value;
        }
        public bool IsDeleted { get; set; }
        public bool IsEnabled { get; set; }
        public int? MatchedPattern { get; set; }
        public string RuleName { get; set; }

    }
}
