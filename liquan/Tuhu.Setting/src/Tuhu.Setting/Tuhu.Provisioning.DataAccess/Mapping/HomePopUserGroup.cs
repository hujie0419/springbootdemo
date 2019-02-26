using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
   public class HomePopUserGroupEntity:EntityTypeConfiguration<HomePopUserGroupEntity>
    {

        public HomePopUserGroupEntity()
        {
            this.ToTable("dbo.HomePopUserGroup");
            this.HasKey(_ => _.pkid);
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int pkid { get; set; }

        public string TargetGroups { get; set; }

        public string TargetKey { get; set; }


    }
}
