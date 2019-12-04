using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.C.ActivityJob.Dal.Rebate;
using Tuhu.C.ActivityJob.Models.Rebate;
using Tuhu.C.ActivityJob.ServiceProxy;

namespace Tuhu.C.ActivityJob.Job.Rebate
{
    public class RebateApplyTechIdSyncJob : IJob
    {
        protected virtual ILog Logger => LogManager.GetLogger(typeof(RebateApplyTechIdSyncJob));

        protected virtual int MaxSyncCount => 40;

        protected virtual bool SyncAll => false;

        public void Execute(IJobExecutionContext context)
        {
            int totalCount = RebateDal.GetRebateApplyCount(SyncAll);

            int pageNum = (totalCount + MaxSyncCount - 1) / MaxSyncCount;
            Logger.Info($"同步返现申请技师ID开始.一共{totalCount}条记录.一共{pageNum}批次.");

            int maxPkid = 0;
            for (int index = 0; index <= pageNum; index++)
            {
                var applys = RebateDal.GetRebateApplys(MaxSyncCount, ref maxPkid, SyncAll);
                if (applys != null && applys.Any())
                {
                    var orderIds = applys.Select(x => (long)x.OrderId).ToArray();
                    var orderTechs = DispatchTechByOrder(orderIds);

                    applys.ForEach(x =>
                    {
                        var orderTech = orderTechs?.FirstOrDefault(o => o.OrderId == x.OrderId);
                        if (orderTech == null)
                        {
                            // 取拆单订单来查派工技师
                            var splitOrderIds = OrderQueryServiceProxy.GetRelatedSplitOrderIds(x.OrderId);
                            orderTech = GetSplitOrderTech(splitOrderIds);
                        }
                        x.TechId = orderTech?.TechId;
                    });

                    var models = applys.Where(x => x.TechId.HasValue)
                        .Select(x => new PkidWithTechId
                        {
                            PKID = x.PKID,
                            TechId = (int)x.TechId
                        }).ToList();

                    if (models.Any())
                        RebateDal.BatchUpdateTechIds(models);
                }

                Logger.Info($"结束同步第{index}批次,一共{pageNum}批次.MaxPkid:{maxPkid}");
            }

            Logger.Info($"同步返现申请技师ID结束.");
        }

        private List<DataInfo> DispatchTechByOrder(long[] orderIds)
        {
            var orderTech = new ShopOrderTechResult();
            AsyncHelper.RunSync(async () => orderTech = await ShopReceiveServiceProxy.DispatchTechByOrder(orderIds));
            if (orderTech != null)
            {
                if (orderTech.Code == 10000 && orderTech.Success == true)
                {
                    return orderTech.Data;
                }
                else
                {
                    Logger.Warn($"DispatchTechByOrder Error：{orderTech.Message}");
                }
            }
            return null;
        }

        private DataInfo GetSplitOrderTech(List<int> splitOrderIds)
        {
            if (splitOrderIds != null && splitOrderIds.Any())
            {
                var orderTechs = DispatchTechByOrder(splitOrderIds.Select(id => (long)id).ToArray());
                return orderTechs?.FirstOrDefault();
            }
            return null;
        }
    }

    public class RebateApplyTechIdSyncAllJob : RebateApplyTechIdSyncJob
    {
        protected override ILog Logger => LogManager.GetLogger(typeof(RebateApplyTechIdSyncAllJob));

        protected override bool SyncAll => true;
    }
}
