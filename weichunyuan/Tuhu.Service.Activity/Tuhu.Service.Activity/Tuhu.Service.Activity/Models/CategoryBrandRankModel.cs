using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models
{
    /// <summary>
    /// CBR20171111
    /// </summary>
    public class CategoryBrandRankModel:BaseModel
    {
        public long PKID { get; set; }
        public long ParentPkid { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string NameIndex { get; set; }
        public string PageTitle { get; set; }
        public string PageShareTitle { get; set; }
        public string PageShareDescription { get; set; }
        public string PageShareContent { get; set; }
        public DateTime CreateTime { get; set; }

        public List<CategoryBrandModel> Branks { get; set; }
    }

    public class CategoryBrandModel
    {
        public long PKID { get; set; }
        public long ParentPkid { get; set; }
        public string Name { get; set; }
        public string NameIndex { get; set; }
    }
}
