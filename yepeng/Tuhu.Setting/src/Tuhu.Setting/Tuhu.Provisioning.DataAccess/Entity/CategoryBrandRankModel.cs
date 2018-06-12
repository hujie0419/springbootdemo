using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CategoryBrandRankModel
    {
        public long PKID { get; set; }
        public long ParentPkid { get; set; }
        public DateTime? Date { get; set; }
        public string Name { get; set; }
        public string NameIndex { get; set; }
        public string PageTitle { get; set; }
        public string PageShareTitle { get; set; }
        public string PageShareDescription { get; set; }
        public string PageShareContent { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
