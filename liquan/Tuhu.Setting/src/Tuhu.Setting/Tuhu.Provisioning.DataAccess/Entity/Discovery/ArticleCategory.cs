using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{
    /// <summary>
    /// 文章、类型关系
    /// </summary>
    [Table("HD_ArticleCategoryTag")]
    public  class ArticleCategory
    {
        public ArticleCategory()
        {
            this.CreateDateTime = DateTime.Now;
        }
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ArticleId { get; set; }

     
        [Required]
        public int CategoryTagId { get; set; }


        [NotMapped]
        //[Required]
        public DateTime CreateDateTime { get; set; }

        [NotMapped]
        public DateTime UpdateDateTime { get; set; }

    }
}
