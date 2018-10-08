using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
   public  class CBrandModel
    {
        public string CategoryName { get; set; }
        public string Category { get; set; }
        private string categoryNodeNO;

        public string CategoryNodeNO
        {
            get
            {
                if (string.IsNullOrEmpty(categoryNodeNO))
                     categoryNodeNO=string.Join(".", CategoryName.Split(':')[1]) +(CategoryName.Length == 1 ? "." : "");
                    return categoryNodeNO;
                
            }
            //set { categoryNodeNO = value; }
        }
        public string Brands { get; set; }
    }
}
