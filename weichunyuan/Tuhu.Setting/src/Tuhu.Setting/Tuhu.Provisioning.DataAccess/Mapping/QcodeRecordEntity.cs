using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
   public class QcodeRecordEntity:EntityTypeConfiguration<QcodeRecordEntity>
    {

        public QcodeRecordEntity()
        {
            this.ToTable("WxApp_QcodeRecord");
            this.HasKey(_ => _.PKID);
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }


        public string CreateUser { get; set; }

        public string Uri { get; set; }

        public DateTime? CreateDatetime { get; set; }

        public string Path { get; set; }

        public int? Width { get; set; }

        public bool? IsColor { get; set; }

        public int? R { get; set; }

        public int? G { get; set; }

        public int? B { get; set; }

        public string Scene { get; set; }

        public int? QcodeType { get; set; }



    }
}
