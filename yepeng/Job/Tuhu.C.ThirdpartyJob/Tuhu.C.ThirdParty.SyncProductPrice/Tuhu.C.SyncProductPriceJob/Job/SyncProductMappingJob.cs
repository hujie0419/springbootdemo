using Common.Logging;
using Quartz;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.PriceManages;

namespace Tuhu.C.SyncProductPriceJob.Job
{
    public class SyncProductMappingJob : BaseQuartzJob
    {
        public SyncProductMappingJob() : base(LogManager.GetLogger<SyncProductMappingJob>())
        {
        }

        protected override Task ExecuteAsync(IJobExecutionContext context) => Task.WhenAll(ShopManageConfiguration.ShopSessionPools.Values.Select(PriceManageHelper.GetPriceMangeInstance)
            .Select(_ => _?.SyncProductMapping()));
    }
}
