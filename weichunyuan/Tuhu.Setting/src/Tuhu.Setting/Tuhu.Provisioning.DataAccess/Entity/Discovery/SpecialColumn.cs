using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{
    public class SpecialColumn
    {
        public SpecialColumn()
        {
            this.IsTop = 0;
        }
        public int ID { get; set; }

        public string ColumnName { get; set; }

        public string ColumnDesc { get; set; }

        public string ColumnImage { get; set; }

        public int IsTop { get; set; }

        public bool IsShow { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? PublishTime { get; set; }

        public string Creator { get; set; }

        public string ArticleIds { get; set; }

        public List<ColumnArticle> Articles { get; set; }
    }


    public class ColumnArticle
    {
        public int ID { get; set; }
        public int PKID { get; set; }
        public int SCID { get; set; }
        public DateTime CreateTime { get; set; }
    }

}
