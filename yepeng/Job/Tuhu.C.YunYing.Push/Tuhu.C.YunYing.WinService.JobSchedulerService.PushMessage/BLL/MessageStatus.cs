using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.YunYing.WinService.JobSchedulerService.PushMessage.BLL
{
    public static class MessageStatus
    {
        public static readonly string New = "New"; //新建

        public static readonly string Sending = "Sending"; //处理中

        public static readonly string Close = "Close";//用户关闭推送开关,写入消息盒子成功

        public static readonly string Success = "Success";//成功 推送成功-消息盒子成功

        public static readonly string Fail = "Fail";//失败  推送失败-消息盒子失败
        public static readonly string FailClose = "FailClose";//用户关闭推送开关,消息盒子失败
        public static readonly string FailCenter = "FailCenter";//推送成功，消息盒子失败 
        public static readonly string FailPush = "FailPush";//推送失败，消息盒子成功 
    }
}
