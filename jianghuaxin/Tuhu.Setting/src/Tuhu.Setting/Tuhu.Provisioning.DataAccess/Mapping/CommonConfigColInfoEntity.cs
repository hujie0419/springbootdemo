using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
    public class CommonConfigColInfoEntity : EntityTypeConfiguration<CommonConfigColInfoEntity>
    {
        public CommonConfigColInfoEntity()
        {
            this.ToTable("dbo.CommonConfigColInfo");
            this.HasKey(_ => _.PKID);
        }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PKID
        {
            set;
            get;
        }
        public string ColName { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateTime { get; set; } = DateTime.Now;
        public DateTime LastUpdateDateTime { get; set; } = DateTime.Now;
    }
}
