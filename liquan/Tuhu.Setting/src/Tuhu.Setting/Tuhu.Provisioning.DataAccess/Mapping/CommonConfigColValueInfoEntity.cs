using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
    public class CommonConfigColValueInfoEntity : EntityTypeConfiguration<CommonConfigColValueInfoEntity>
    {
        public CommonConfigColValueInfoEntity()
        {
            this.ToTable("dbo.CommonConfigColValueInfo");
            this.HasKey(_ => _.PKID);
        }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PKID
        {
            set;
            get;
        }
        public string ActiveHashKey { get; set; }
        public string GroupId { get; set; }
        public int FormIndex { get; set; }
        public int ConfigInfoPkId { get; set; }
        public string Value { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public DateTime CreateDateTime { get; set; } = DateTime.Now;
        public DateTime LastUpdateDateTime { get; set; } = DateTime.Now;
    }
}

