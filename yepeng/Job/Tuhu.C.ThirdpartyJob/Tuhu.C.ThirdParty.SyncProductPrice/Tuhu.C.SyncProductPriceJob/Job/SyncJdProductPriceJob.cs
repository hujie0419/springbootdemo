using Common.Logging;
using Quartz;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.DataAccess;
using Tuhu.C.SyncProductPriceJob.PriceManages;

namespace Tuhu.C.SyncProductPriceJob.Job
{
    /// <summary>
    /// 同步京东产品价格
    /// </summary>
    public class SyncJdProductPriceJob : BaseQuartzJob
    {
        public SyncJdProductPriceJob() : base(LogManager.GetLogger<SyncJdProductPriceJob>())
        {
        }

        protected override async Task ExecuteAsync(IJobExecutionContext context)
        {
            foreach (var shop in ShopManageConfiguration.ShopSessionPools.Values.Where(s => s.ShopType == ShopTypes.Jingdong))
            {
                await PriceManageHelper.GetPriceMangeInstance(shop).SyncProductsPrice((await Products.QueryProductMappings(shop.ShopCode)).ToArray());
            }
        }
    }
}
