using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{
    public class UserBlackList
    {
        //PKID, UserId, RelatedContent, BanType, Operator, CreateTime, State
        public int PKID { get; set; }

        public string UserId { get; set; }

        public string RelatedContent { get; set; }

        public string BanType { get; set; }

        public string Operator { get; set; }

        public DateTime CreateTime { get; set; }

        public bool State { get; set; }

    }
}
