using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{
    public class CoverAuthor
    {
        //PKID, AuthorName, AuthorPhone, AuthorHead, Description, CreateTime, IsDelete
        public int PKID { get; set; }

        public string AuthorName { get; set; }

        public string AuthorPhone { get; set; }

        public string AuthorHead { get; set; }

        public string Description { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsDelete { get; set; }

    }
}
