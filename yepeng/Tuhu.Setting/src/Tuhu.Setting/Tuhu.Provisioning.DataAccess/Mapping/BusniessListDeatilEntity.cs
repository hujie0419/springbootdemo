using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
  public  class BusniessListDeatilEntity:EntityTypeConfiguration<BusniessListDeatilEntity>
    {

        public BusniessListDeatilEntity()
        {
            this.ToTable("dbo.WxApp_BusniessListDeatil");
            this.HasKey(o => o.PKID);

        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }

        /// <summary>
        /// FKID
        /// </summary>		
        public int? FKID { get; set; }

        /// <summary>
        /// Name
        /// </summary>		
        public string Name { get; set; }

        /// <summary>
        /// IconUri
        /// </summary>		
        public string IconUri { get; set; }

        /// <summary>
        /// Uri
        /// </summary>		
        public string Uri { get; set; }

        /// <summary>
        /// 跳转类型
        /// </summary>
        public bool? UriType { get; set; }

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
