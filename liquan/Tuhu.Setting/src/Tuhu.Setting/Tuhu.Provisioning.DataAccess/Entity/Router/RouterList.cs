using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Router
{
    public  class RouterList
    {

        public IEnumerable<RouterParameter> RouterParameterList { get; set; }

        public IEnumerable<RouterMainLink> RouterMainLinkList { get; set; }

    }
}
