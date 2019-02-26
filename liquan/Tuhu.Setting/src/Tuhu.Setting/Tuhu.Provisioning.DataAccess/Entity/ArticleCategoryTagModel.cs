using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ArticleCategoryTagModel
    {
        public int Id { get; set; }
        /// <summary>
        /// (文章/专题)ID
        /// </summary>
        public int ArticleId { get; set; }
        /// <summary>
        /// 标签ID
        /// </summary>
        public int CategoryTagId { get; set; }
    }
}
