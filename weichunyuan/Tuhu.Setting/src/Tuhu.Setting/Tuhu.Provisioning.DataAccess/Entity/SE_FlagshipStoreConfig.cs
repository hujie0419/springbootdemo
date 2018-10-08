using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
  public  class SE_FlagshipStoreConfig
    {
        /// <summary>
        /// PKID
        /// </summary>		
        public int PKID { get; set; }
        /// <summary>
        /// Brand
        /// </summary>		
        public string Brand { get; set; }
        /// <summary>
        /// Name
        /// </summary>		
        public string Name { get; set; }
        /// <summary>
        /// Describe
        /// </summary>		
        public string Describe { get; set; }
        /// <summary>
        /// Uri
        /// </summary>		
        public string Uri { get; set; }
        /// <summary>
        /// ImageUrl
        /// </summary>		
        public string ImageUrl { get; set; }
        /// <summary>
        /// Remark
        /// </summary>		
        public string Remark { get; set; }
        /// <summary>
        /// CreateDT
        /// </summary>		
        public DateTime CreateDT { get; set; }
        /// <summary>
        /// UpdateDT
        /// </summary>		
        public DateTime UpdateDT { get; set; }


    }
}
