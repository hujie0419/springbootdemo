using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
   public class WechatSubNumberEntity: EntityTypeConfiguration<WechatSubNumberEntity>
    {
        public WechatSubNumberEntity()
        {
            this.HasKey(_ => _.PKID);
            this.ToTable("dbo.Wechat_SubNumber");
        }

        public int PKID { get; set; }

        [NotMapped]
        public string Group { get; set; }
        public int? ParentPKID { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }

        public string Appid { get; set; }

        public string Pagepath { get; set; }

        public bool IsEnabled { get; set; }

        private DateTime _createDateTime = DateTime.Now;
        public DateTime CreateDateTime
        {
            get { return this._createDateTime; }
            set { this._createDateTime = value; }
        }

        private DateTime _lastUpdateDateTime = DateTime.Now;
        public DateTime LastUpdateDateTime
        {
            get { return this._lastUpdateDateTime; }
            set { this._lastUpdateDateTime = value; }
        }

        /// <summary>
        /// 排序
        /// </summary>
        public int? OrderBy { get; set; }
        /// <summary>
        /// 公众号原始ID
        /// </summary>
        public string OriginalID { get; set; }
        /// <summary>
        /// 微信按钮key
        /// </summary>
        public string ButtonKey { get; set; }
    }
}
