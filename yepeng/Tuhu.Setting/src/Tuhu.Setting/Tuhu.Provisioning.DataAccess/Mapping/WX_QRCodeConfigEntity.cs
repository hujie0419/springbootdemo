using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
    /// <summary>
    /// 微信二维码 实体
    /// </summary>
    public class WX_QRCodeConfigEntity : EntityTypeConfiguration<WX_QRCodeConfigEntity>
    {
        public WX_QRCodeConfigEntity()
        {
            this.HasKey(_ => _.PKID);
            this.ToTable("dbo.WX_QRCodeConfig");
        }
        public int PKID { get; set; }

        public string Title { get; set; }
        public string Type { get; set; }
        public string Scene { get; set; }
        public string action_name { get; set; }

        /// <summary>
        /// 临时二维码的 有效期 max = 2592000 [30 day]
        /// </summary>
        public long expire_seconds { get; set; }
        public string URL { get; set; }
        public string Userid { get; set; }

        public string OriginalID { get; set; }
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
    }
}
