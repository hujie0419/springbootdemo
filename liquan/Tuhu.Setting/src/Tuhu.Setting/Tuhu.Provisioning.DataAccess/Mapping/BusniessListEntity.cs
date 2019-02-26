using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
    public class BusniessListEntity : EntityTypeConfiguration<BusniessListEntity>
    {

        public BusniessListEntity()
        {
            this.ToTable("dbo.WxApp_BusniessList");
            this.HasKey(o => o.PKID);
        }

        /// <summary>
        /// PKID
        /// </summary>		
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }

        /// <summary>
        /// Title
        /// </summary>		
        public string Title { get; set; }

        /// <summary>
        /// OrderBy
        /// </summary>		
        public int? OrderBy { get; set; }

        /// <summary>
        /// Status
        /// </summary>		
        public bool? Status { get; set; }

        /// <summary>
        /// CreateDateTime
        /// </summary>		
        public DateTime? CreateDateTime { get; set; }

        /// <summary>
        /// UpdateDateTime
        /// </summary>		
        public DateTime? UpdateDateTime { get; set; }

        /// <summary>
        /// CreateUserName
        /// </summary>		
        public string CreateUserName { get; set; }

        /// <summary>
        /// UpdateUserName
        /// </summary>		
        public string UpdateUserName { get; set; }
    }
}
