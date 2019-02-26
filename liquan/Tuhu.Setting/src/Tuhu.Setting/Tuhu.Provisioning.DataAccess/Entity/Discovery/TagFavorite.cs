using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{
    [Table("tbl_CategoryFavorite")]
    public class CategoryFavorite
    {

        public CategoryFavorite()
        {
            this.CreateDateTime = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string UserId { get; set; }

        public string Type { get { return "标签"; } }

        [Required]
        public DateTime CreateDateTime { get; set; }

    }
}
