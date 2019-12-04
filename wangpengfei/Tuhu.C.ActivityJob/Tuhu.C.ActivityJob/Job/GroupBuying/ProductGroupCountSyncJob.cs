using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.ActivityJob.Dal.GroupBuying;
using Tuhu.C.ActivityJob.ServiceProxy;
using Common.Logging;
using Quartz;

namespace Tuhu.C.ActivityJob.Job.GroupBuying
{
    public class ProductGroupCountSyncJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(ProductGroupCountSyncJob));

        private int MaxSyncCount => 200;

        public void Execute(IJobExecutionContext context)
        {
            int totalCount = GroupBuyingDal.SelectProductCount();

            int pageNum = (totalCount + MaxSyncCount - 1) / MaxSyncCount;
            Logger.Info($"同步拼团已开团数开始.一共{totalCount}个产品.一共{pageNum}批次.");

            int maxPkid = 0;
            if (totalCount > 0)
            {
                var sbMsg = new StringBuilder();
                for (int index = 0; index <= pageNum; index++)
                {
                    var products = GroupBuyingDal.SelectProducts(MaxSyncCount, ref maxPkid);
                    if (products != null && products.Any())
                    {
                        foreach (var item in products)
                        {
                            item.CurrentPGroupCount = GroupBuyingDal
                                .GetCurrentGroupCount(item.ProductGroupId, item.Pid);

                            if (GroupBuyingDal.UpdateGroupCount(item))
                            {
                                if (item.CurrentPGroupCount >= item.TotalPGroupCount && item.TotalPGroupCount != 0)
                                {
                                    sbMsg.AppendLine($"{item.ProductGroupId} {item.Pid} 到达开团上限，" +
                                        $"已开团数：{item.CurrentPGroupCount}，限开团数：{item.TotalPGroupCount}");
                                }
                            }
                        }
                    }

                    Logger.Info($"结束刷新第{index}批次,一共{pageNum}批次.MaxPkid:{maxPkid}");
                }

                if (sbMsg.Length > 0)
                {
                    Logger.Info(sbMsg.ToString());
                    PinTuanServiceProxy.NotifyEmployee(
                        new Service.PinTuan.Models.NotifyEmployeeRequest
                        {
                            ErrorMessage = sbMsg.ToString()
                        });
                }
            }

            Logger.Info($"同步拼团已开团数结束.");
        }
    }
}
