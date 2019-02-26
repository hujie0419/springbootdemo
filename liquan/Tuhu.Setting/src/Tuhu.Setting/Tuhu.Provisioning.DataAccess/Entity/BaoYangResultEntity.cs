using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
   public  class BaoYangResultEntity<T>
    {
        public bool Status { get; set; }
        public string Msg { get; set; }
        public T Data { get; set; }
    }
}
