using Common.Logging;
using Quartz;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.DataAccess;
using Tuhu.C.SyncProductPriceJob.PriceManages;

namespace Tuhu.C.SyncProductPriceJob.Job
{
    /// <summary>
    /// 同步天猫价格
    /// </summary>
    public class SyncTmallProductPriceJob : BaseQuartzJob
    {
        public SyncTmallProductPriceJob() : base(LogManager.GetLogger<SyncTmallProductPriceJob>())
        {
        }

        protected override async Task ExecuteAsync(IJobExecutionContext context)
        {
            foreach (var shop in ShopManageConfiguration.ShopSessionPools.Values.Where(s => s.ShopType == ShopTypes.Taobao))
            {
                await PriceManageHelper.GetPriceMangeInstance(shop).SyncProductsPrice((await Products.QueryProductMappings(shop.ShopCode)).ToArray());
            }
        }
    }
}
