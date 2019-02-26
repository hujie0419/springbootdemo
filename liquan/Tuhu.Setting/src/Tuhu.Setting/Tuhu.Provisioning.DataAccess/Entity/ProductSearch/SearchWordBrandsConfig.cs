using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.ProductSearch
{
    public class SearchWordBrandsConfig
    {
        public int Pkid { get; set; }

        public string Keywords { get; set; }

        public string Brands { get; set; }

        public int IsShow { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public bool IsDelete { get; set; }

        public List<string> KeyWordList { get
            {
                if (string.IsNullOrEmpty(Keywords))
                    return null;
                return Keywords.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }
        public List<string> BrandsList
        {
            get
            {
                if (string.IsNullOrEmpty(Brands))
                    return null;
                return Brands.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }
    }
}
