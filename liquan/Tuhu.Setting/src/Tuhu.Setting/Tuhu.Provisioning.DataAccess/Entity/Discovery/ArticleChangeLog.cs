using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{
    [Table("tbl_Article_Change_Log")]
    public  class ArticleChangeLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public System.Guid OperatorId { get; set; }

        [MaxLength(50)]
        public string Operation { get; set; }
        public string OperationValue { get; set; }

        [Required]
        public DateTime CreateDateTime { get; set; }
    }
}
