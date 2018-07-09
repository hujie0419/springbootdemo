using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.MMS.Web.Domain.UserFilter
{
    public class UserFilterResultConfig
    {
        public UserFilterResultConfig()
        {
            IsEffective = true;
        }
        public int PKID { get; set; }
        public int JobId { get; set; }
        public string TableName { get; set; }
        public string BasicAttribute { get; set; }
        public string ColName { get; set; }
        public bool IsEffective { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
    }
}
