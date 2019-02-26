using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
   public class BaseQueryModel
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }


        public string OrderBy { get; set; }

        /// <summary>
        /// true:降序 false:升序
        /// </summary>
        public bool ToSort { get; set;}
    }
    
}
