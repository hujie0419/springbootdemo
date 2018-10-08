using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.TagConfig
{
    public class ArticleTabConfig
    {
        public int PKID { get; set; }

        public string NormalImg { get; set; }

        public string SelectedImg { get; set; }

        public TagSource Source { get; set; }

        public string CreateUser { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public int Total { get; set; }
    }

    public enum TagSource
    {
        TuhuWangPai
    }
}
