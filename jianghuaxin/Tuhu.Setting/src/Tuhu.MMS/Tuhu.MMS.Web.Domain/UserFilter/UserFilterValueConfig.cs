using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.MMS.Web.Domain.UserFilter
{
    public class UserFilterValueConfig
    {
        public int PKID { get; set; }
        public string TableName { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string ParentValue { get; set; }
        public string ColName { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
    }
}
