using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
   public class TirePatternModel
    {
        public int PKID { get; set; }
        public string Brand { get; set; }
        public string Pattern { get; set; }

        public string Image { get; set; }

        public string Title { get; set; }

        public string Describe { get; set; }
        public string Author { get; set; }

        public DateTime? Date { get; set; }

        public string ArticleLink { get; set; }

        public int IsActive { get; set; }
        public bool? IsShow { get; set; }
        /// <summary>
        /// 0所有  1 》0  2  =0
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 0所有  1 》0  2  =0
        /// </summary>
        public int ShowCount { get; set; }

        public IEnumerable<BrandToPatterns> Patterns { get; set; }

        public string Group { get; set; }
    }

    public class BrandToPatterns
    {
        public string Brand { get; set; }
        public string Patterns { get; set; }
    }

    public class PatternArticleModel
    {
        public int PKID { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Describe { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public string ArticleLink { get; set; }
        public bool IsShow { get; set; }
        public string Brand { get; set; }
        public string Pattern { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
