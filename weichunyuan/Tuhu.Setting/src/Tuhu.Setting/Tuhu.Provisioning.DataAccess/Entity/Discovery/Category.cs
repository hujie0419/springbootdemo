using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{
    [Table("HD_CategoryTag")]
    public class Category
    {
        public Category()
        {
            ChildrenCategory = new List<Category>();
        }

        /// <summary>
        /// 标签Id
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 标签名称
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 标签所属的父标签Id
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 标签描述
        /// </summary>
        [MaxLength(200)]
        public string Describe { get; set; }

        /// <summary>
        /// 标签图片
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 标签被关注数
        /// </summary>
        //public Nullable<int> AttentionCount { get; set; }

        public bool Disable { get; set; }

        //[DefaultValue("Saved")]
        //public string Status { get; set; }

        //[Required]
        //public DateTime CreateTime { get; set; }

        //public Nullable<System.DateTime> UpdateTime { get; set; }

        /// <summary>
        /// 标签的子标签
        /// </summary>
        [NotMapped]
        public  List<Category> ChildrenCategory { get; set; }

        /// <summary>
        /// 标签和文章的Many To Many映射集合
        /// </summary>
        //public virtual ICollection<ArticleCategory> ArticleCategorys { get; set; }
    }
}
