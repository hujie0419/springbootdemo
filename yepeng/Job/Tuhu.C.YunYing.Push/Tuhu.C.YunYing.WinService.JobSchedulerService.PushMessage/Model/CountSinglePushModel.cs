using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Model
{
    public class CountSinglePushModel
    {
        public int SendCount { get; set; }
        public string PushTime { get; set; }
        public int TemplateInfo { get; set; }
        public DeviceType DeviceType { get; set; }
    }
}
