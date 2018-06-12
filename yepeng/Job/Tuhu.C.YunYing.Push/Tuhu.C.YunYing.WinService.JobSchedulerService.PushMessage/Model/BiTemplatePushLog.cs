using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Model
{
    public class BiTemplatePushLog
    {
        public int PKID { get; set; }
        public int BatchID { get; set; }
        public bool IsSync { get; set; }
        public string UserID { get; set; }
        public string Replacement { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public string DeviceID { get; set; }
        public string OpenID { get; set; }
        public string WxOpenID { get; set; }
        public string PayName { get; set; }
        public Dictionary<string, string> ReplacementDict
        {
            get
            {
                if (!string.IsNullOrEmpty(Replacement))
                {
                    try
                    {
                        var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(Replacement);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        BiTemplatePushJob.Logger.Warn(ex);
                    }
                }
                return new Dictionary<string, string>();
            }
        }

    }
}
