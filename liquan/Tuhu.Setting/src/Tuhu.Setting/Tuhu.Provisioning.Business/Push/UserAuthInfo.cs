using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.Business.Push
{
    public class UserAuthInfo
    {
        public string UserId { get; set; }
        public string Channel { get; set; }
        public string OpenId { get; set; }
        public string UnionId { get; set; }
    }
}
