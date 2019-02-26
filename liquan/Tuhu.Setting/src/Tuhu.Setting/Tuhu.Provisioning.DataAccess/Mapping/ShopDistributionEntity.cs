using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
    public class ShopDistributionEntity : EntityTypeConfiguration<ShopDistributionEntity>
    {
        public ShopDistributionEntity()
        {
            this.ToTable("dbo.ShopDistribution");
            this.HasKey(_ => _.PKID);
        }
        public int PKID { get; set; }
        public string FKPID { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateTime { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public int IsDelete { get; set; }
    }
}
