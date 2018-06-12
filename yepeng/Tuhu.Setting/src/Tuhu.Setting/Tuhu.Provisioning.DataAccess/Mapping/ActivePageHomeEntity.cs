using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
  public  class ActivePageHomeEntity:EntityTypeConfiguration<ActivePageHomeEntity>
    {

        public ActivePageHomeEntity()
        {
            this.ToTable("dbo.ActivePageHome");
            this.HasKey(o => o.PKID);
        }


       [DatabaseGenerated( DatabaseGeneratedOption.Identity)]
      public int PKID { get; set; }

        public int FKActiveID { get; set; }

        public int Sort { get; set; }

        public string BigHomeName { get; set; }

        public string HidBigHomePic { get; set; }

        public string BigHomeUrl { get; set; }

        public string HidBigHomePicWww { get; set; }

        public string BigHomeUrlWww { get; set; }
        /// <summary>
        /// 微信小程序图标
        /// </summary>
        public string HidBigHomePicWxApp { get; set; }
        /// <summary>
        /// 微信小程序跳转
        /// </summary>
        public string BigHomeUrlWxApp { get; set; }

        public bool? IsHome { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? UpdateDateTime { get; set; }

    }
}
