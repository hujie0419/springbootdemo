using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.YunYing.WinService.JobSchedulerService.DAL;
using Tuhu.Service.Activity;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.Job
{
    [DisallowConcurrentExecution]
    public class Dealgrouporder : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Dealgrouporder));
        public void Execute(IJobExecutionContext context)
        {
            var data = dealgroup.GetOrderInfo();
            foreach(var item in data)
            {
                var activityId = item.Remark.Replace("限时抢购价：活动ID为", "");
                var productGroupId = dealgroup.GetProductGroupIdByActivityId(new Guid(activityId));
                using(var client=new GroupBuyingClient())
                {
                    var result = client.CreateGroupBuying(item.UserId, productGroupId, item.PID, item.OrderId);
                    if (result.Success&& item.PayStatus=="2Paid")
                    {
                        var groupId = dealgroup.GetGroupBuyingId(item.OrderId);
                        TuhuNotification.SendNotification("notification.GroupBuyingOrderStatusQueue", new
                        {
                            OrderId = item.OrderId,
                            Operate = "Paid"
                        },3000);
                    }
                }
            }
        }
    }
}
