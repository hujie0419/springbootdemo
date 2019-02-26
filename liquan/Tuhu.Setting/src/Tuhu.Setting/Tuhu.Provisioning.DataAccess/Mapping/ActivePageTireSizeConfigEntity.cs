using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
    public class ActivePageTireSizeConfigEntity : EntityTypeConfiguration<ActivePageTireSizeConfigEntity>
    {

        public ActivePageTireSizeConfigEntity()
        {
            this.ToTable("dbo.ActivePageTireSizeConfig");
            this.HasKey(o => o.PKID);
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }

        public int FKActiveID { get; set; }

        public bool? IsChangeTire { get; set; }

        public bool? IsChangeTireSize { get; set; }

        public bool? IsShowTag { get; set; }
        public bool? IsShowVehicleBar { get; set; }

        public bool? IsMargin { get; set; }

        public string MarginColor { get; set; }

        public string FillColor { get; set; }

        public string PromptColor { get; set; }

        public string PromptFontSize { get; set; }

        public string CarInfoColor { get; set; }

        public string CarInfoFontSize { get; set; }

        public string NoCarTypePrompt { get; set; }

        public string NoFormatPrompt { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? UpdateDateTime { get; set; }

    }
}
