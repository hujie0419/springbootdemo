using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Product;
using Tuhu.C.Job.ProductCache.DAL;
using Tuhu.Service.Product.Enum;

namespace Tuhu.C.Job.ProductCache.Job
{
    [DisallowConcurrentExecution]
    public class PerMinuteJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(PerMinuteJob));

        public void Execute(IJobExecutionContext context)
        {
            var curDateTime = DateTime.Now;

            Logger.Info($"发送产品标签更新通知:{curDateTime}");
            TuhuNotification.SendNotification("notification.productModify.ProductCommonTag", new
            {
                type = "RefreshInterval",
                timespan = curDateTime
            });
           

            if (curDateTime.Hour == 8 && curDateTime.Minute == 20) //8点20分
            {
                Logger.Info($"notification.productmatch.modify.RebuildActivity=>{curDateTime}");
                TuhuNotification.SendNotification("notification.productmatch.modify", new { type = "RebuildActivity" });
            }
            if (curDateTime.Hour == 16 && curDateTime.Minute == 30) //16点30分
            {
                Logger.Info($"notification.productmatch.modify.RebuildActivity=>{curDateTime}");
                TuhuNotification.SendNotification("notification.productmatch.modify", new { type = "RebuildActivity" });
            }
            if (curDateTime.Hour >= 4 && curDateTime.Hour <= 23 && curDateTime.Minute == 13) //4点到23点 每隔1小时触发一次
            {
                Logger.Info($"notification.productmatch.modify.RebuildPerHourEsCache=>{curDateTime}");
                TuhuNotification.SendNotification("notification.productmatch.modify", new { type = "RebuildPerHourEsCache" });
            }
            if (curDateTime.Hour == 2 && curDateTime.Minute == 16) //2点16分
            {
                Logger.Info($"ProductNodeCntRefreshJob刷新类目下的产品数量=>{curDateTime}");
                new ProductNodeCntRefreshJob().Execute(null);
            }
            if (curDateTime.Hour == 7 && curDateTime.Minute == 16) //7点16分
            {
                Logger.Info($"notification.ProductModify.ProductCacheModify.ProductLimit=>{curDateTime}");
                TuhuNotification.SendNotification("notification.ProductModify.ProductCacheModify", new { type = "ProductLimit" });
            }
        }
    }

}
