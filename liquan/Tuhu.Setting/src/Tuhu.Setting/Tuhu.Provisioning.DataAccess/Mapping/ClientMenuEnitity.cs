using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
  public  class ClientMenuEnitity
    {
        public string F_ParentId { get; set; }

        public string F_Id { get; set; }

        public string F_Icon { get; set; }

        public string F_FullName { get; set; }

        public string F_UrlAddress { get; set; }

        public int F_SortCode { get; set; }

        public string Target { get; set; }

        public List<ClientMenuEnitity> ChildNodes { get; set; }
    }
}
