using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
  public  class ActivePageHomeDeatilEntity:EntityTypeConfiguration<ActivePageHomeDeatilEntity>
    {

        public ActivePageHomeDeatilEntity()
        {
            this.ToTable("dbo.ActivePageHomeDeatil");
            this.HasKey(o => o.PKID);
        }

        [DatabaseGenerated( DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }


        public int FKActiveHome { get; set; }

        public string HomeName { get; set; }



        public string HidBigFHomePic { get; set; }

        public string BigFHomeMobileUrl { get; set; }

        public string BigFHomeWwwUrl { get; set; }
        public string BigFHomeWxAppUrl { get; set; }

        public int BigFHomeOrder { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? UpdateDateTime { get; set; }



    }
}
