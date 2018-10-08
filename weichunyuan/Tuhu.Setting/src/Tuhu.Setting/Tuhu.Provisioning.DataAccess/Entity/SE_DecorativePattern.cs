using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class SE_DecorativePattern
    {

        /// <summary>
        /// ID
        /// </summary>		
        public Guid ID { get; set; }
        /// <summary>
        /// Name
        /// </summary>		
        public string Name { get; set; }
        /// <summary>
        /// Brand
        /// </summary>		
        public string Brand { get; set; }
        /// <summary>
        /// Flower
        /// </summary>		
        public string Flower { get; set; }
        /// <summary>
        /// ImageUrl1
        /// </summary>		
        public string ImageUrl1 { get; set; }
        /// <summary>
        /// ImageUrl2
        /// </summary>		
        public string ImageUrl2 { get; set; }
        /// <summary>
        /// ImageUrl3
        /// </summary>		
        public string ImageUrl3 { get; set; }

        public string Description { get; set; }

        public int ArticleID { get; set; }

        public string ArticleTitle { get; set; }

        public string ShareParameter { get; set; }

        public IEnumerable<SE_DecorativePatternDetail> Items { get; set; }

    }

    public class SE_DecorativePatternDetail
    {
        /// <summary>
        /// ID
        /// </summary>		
        public int ID { get; set; }
        /// <summary>
        /// FK_DecorativePattern
        /// </summary>		
        public Guid FK_DecorativePattern { get; set; }
        /// <summary>
        /// ArticleID
        /// </summary>		
        public int ArticleID { get; set; }
        /// <summary>
        /// ArticleTitle
        /// </summary>		
        public string ArticleTitle { get; set; }


        public string Image { get; set; }

        public string Description { get; set; }

        public int OrderBy { get; set; }
    }


}
