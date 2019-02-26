using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tuhu.Provisioning.DataAccess.Entity
{
    //SE_BrandFlagshipDetail
    public class SE_BrandFlagshipDetail
    {

        /// <summary>
        /// ID
        /// </summary>		
        public int ID { get; set; }
        /// <summary>
        /// FK_BrandFlagship
        /// </summary>		
        public int FK_BrandFlagship { get; set; }
        /// <summary>
        /// ArticleID
        /// </summary>		
        public int ArticleID { get; set; }
        /// <summary>
        /// ArticleTitle
        /// </summary>		
        public string ArticleTitle { get; set; }
        /// <summary>
        /// 0资讯 1评测
        /// </summary>		
        public int ArticleType { get; set; }

        public string Description { get; set; }

        public int OrderBy { get; set; }

    }
}

