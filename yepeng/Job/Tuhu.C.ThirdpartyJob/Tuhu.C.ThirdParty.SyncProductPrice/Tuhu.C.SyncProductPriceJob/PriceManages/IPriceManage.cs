using System.Collections.Generic;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.Models;

namespace Tuhu.C.SyncProductPriceJob.PriceManages
{
    public interface IPriceManage : ISyncProductMaping
    {
        Task SyncProductPrice();

        Task SyncProductsPrice(IReadOnlyCollection<ProductPriceMappingModel> products);
    }
}
