using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{


    //SE_BrandFlagship
    public class SE_BrandFlagship
    {

        /// <summary>
        /// ID
        /// </summary>		
        public int ID { get; set; }
        /// <summary>
        /// Name
        /// </summary>		
        public string Name { get; set; }
        /// <summary>
        /// ImageUrl
        /// </summary>		
        public string ImageUrl { get; set; }
        /// <summary>
        /// ActivityHome
        /// </summary>		
        public string ActivityHome { get; set; }
        /// <summary>
        /// Description
        /// </summary>		
        public string Description { get; set; }
        /// <summary>
        /// ArticleID
        /// </summary>		
        public int ArticleID { get; set; }
        /// <summary>
        /// AticleTitle
        /// </summary>		
        public string ArticleTitle { get; set; }
        /// <summary>
        /// 花纹ID
        /// </summary>		
        public string DecorativePattern { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string LogoUrl { get; set; }


        /// <summary>
        /// 资讯
        /// </summary>
        public IEnumerable<SE_BrandFlagshipDetail> Information { get; set; }


        /// <summary>
        /// 评测
        /// </summary>
        public IEnumerable<SE_BrandFlagshipDetail> Testing { get; set; }

        public string Brand { get; set; }


        public string ShareParameter { get; set; }


    }



}
