using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
   public class SEGetPromotionActivityConfig:EntityTypeConfiguration<SEGetPromotionActivityConfig>
    {

        public SEGetPromotionActivityConfig()
        {
            this.ToTable("dbo.SE_GetPromotionActivityConfig");
            this.HasKey(_ => _.ID);
        }

        /// <summary>
        /// ID
        /// </summary>	
        public Guid? ID { get; set; }
        /// <summary>
        /// ActivityName
        /// </summary>		
        public string ActivityName { get; set; }
        /// <summary>
        /// StartDateTime
        /// </summary>		
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// EndDateTime
        /// </summary>		
        public DateTime EndDateTime { get; set; }
        /// <summary>
        /// Channel
        /// </summary>		
        public string Channel { get; set; }
        /// <summary>
        /// Status
        /// </summary>		
        public bool Status { get; set; }

        [NotMapped]
        public string StatusText { get; set; }

        /// <summary>
        /// true 新用户
        /// </summary>
        public bool IsNewUser { get; set; }

        [NotMapped]
        public string NewUserText { get; set; }

        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 领取数量
        /// </summary>
        [NotMapped]
        public int? GetCouponNumbers { get; set; }

        /// <summary>
        /// 剩余数量
        /// </summary>
        [NotMapped]
        public int? GetCouponTotal { get; set; }

        [NotMapped]
        public string Uri { get; set; }


    }
}
