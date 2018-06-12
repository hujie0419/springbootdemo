using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.UMApi.Core
{
    public interface IPush
    {
        ReturnJsonClass SendMessage(UMBaseMessage paramsJsonObj);

        TaskQueryResult QueryTaskStatus(TaskQueryModel paramsJsonObj);

        void AsynSendMessage(UMBaseMessage paramsJsonObj, Action<ReturnJsonClass> callback);
    }
}
