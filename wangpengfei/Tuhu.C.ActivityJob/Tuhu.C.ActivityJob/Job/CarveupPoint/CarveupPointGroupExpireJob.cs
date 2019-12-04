using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.C.ActivityJob.Dal.CarveupPoint;
using Tuhu.C.ActivityJob.ServiceProxy;
using Tuhu.Service.CashBackActivity.Request;

namespace Tuhu.C.ActivityJob.Job.CarveupPoint
{
    public class CarveupPointGroupExpireJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CarveupPointGroupExpireJob));

        private int MaxExpireCount => 20;

        public void Execute(IJobExecutionContext context)
        {
            int totalCount = CarveupPointDal.GetExpiredGroupCount();

            int pageNum = (totalCount + MaxExpireCount - 1) / MaxExpireCount;
            Logger.Info($"过期瓜分积分活动开始.一共{totalCount}个团.一共{pageNum}批次.");

            int maxPkid = 0;
            if (totalCount > 0)
            {
                for (int index = 1; index <= pageNum; index++)
                {
                    var groups = CarveupPointDal.GetExpiredGroups(MaxExpireCount, ref maxPkid);

                    CarveupPointServiceProxy.ExpireCarveupPointGroup(new ExpireCarveupPointGroupRequest
                    {
                        ExpireGroups = groups.Select(x => new ExpireGroup
                        {
                            GroupId = x.GroupId,
                            ActivityId = x.ActivityId
                        }).ToList()
                    });

                    Logger.Info($"过期第{index}批次,一共{pageNum}批次.MaxPkid:{maxPkid}");
                }
            }

            Logger.Info($"过期瓜分积分活动结束.");
        }
    }
}
