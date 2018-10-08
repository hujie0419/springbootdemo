using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{
    [Table("tbl_User_Operation")]
    public  class UserOperation
    {

        public UserOperation()
        {
            this.CreateDateTime = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

       
        /// <summary>
        /// 用户ID
        /// </summary>
        public System.Guid UserId { get; set; }

        /// <summary>
        /// 设备号ID
        /// </summary>
        public System.Guid DeviceId { get; set; }

        /// <summary>
        /// 分享目的地
        /// </summary>
        [MaxLength(50)]
        public string Source { get; set; }

        [MaxLength(50)]
        [Required]
        public string Operation { get; set; }

        [Required]
        public int ArticleId { get; set; }

        [Required]
        public DateTime CreateDateTime { get; set; }

        public Nullable<System.DateTime> UpdateDateTime { get; set; }
    }
}
