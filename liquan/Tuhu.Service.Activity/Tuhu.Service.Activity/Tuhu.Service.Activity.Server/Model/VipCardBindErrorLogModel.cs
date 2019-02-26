using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.Server.Model
{
    public class VipCardBindErrorLogModel: VipCardRecordRequest
    {
        public string BindCardMsg { get; set; }
    }
}
