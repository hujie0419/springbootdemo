using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class MsgResultModel
    {
        public int State { get; set; }
        public string ResultData { get; set; }
        public string Message { get; set; }
    }
}