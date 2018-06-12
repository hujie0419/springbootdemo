using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
    public class ActivePageMenuEntity:EntityTypeConfiguration<ActivePageMenuEntity>
    {
        public ActivePageMenuEntity()
        {
            this.ToTable("dbo.ActivePageMenu");
            this.HasKey(o=>o.PKID);
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }

        public int FKActiveContentID { get; set; }

        public string MenuName { get; set; }

        public string MenuValue { get; set; }

        public string MenuValueEnd { get; set; }

        public int Sort { get; set; }

        public string Color { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? UpdateDateTime { get; set; }

    }
}
