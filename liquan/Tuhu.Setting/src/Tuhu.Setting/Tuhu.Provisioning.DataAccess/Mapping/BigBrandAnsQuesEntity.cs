using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
   public class BigBrandAnsQuesEntity: EntityTypeConfiguration<BigBrandAnsQuesEntity>
    {
        public BigBrandAnsQuesEntity()
        {
            this.ToTable("dbo.BigBrandAnsQues");
            this.HasKey(_ => _.PKID);
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? LastUpdateDateTime { get; set; }

        public string Tip { get; set; }

        public int TipCount { get; set; }

        public string DefTitle { get; set; }

        public string DefSmallTitle { get; set; }

        public string ShareTitle { get; set; }

        public string ShareSmallTitle { get; set; }

        public string NoTimeTitle { get; set; }

        public string NoTimeSmallTitle { get; set; }

        public string DefResTitle { get; set; }

        public string DefResSmallTitle { get; set; }

        public string ShareResTitle { get; set; }

        public string ShareResSmallTitle { get; set; }

        public string BgImgUri { get; set; }

        public string LastImgUri { get; set; }
        public string HomeBgImgUri { get; set; }

        public string ResultImgUri { get; set; }

        public int BigBrandPKID { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool Is_Deleted { get; set; }
    }
}
