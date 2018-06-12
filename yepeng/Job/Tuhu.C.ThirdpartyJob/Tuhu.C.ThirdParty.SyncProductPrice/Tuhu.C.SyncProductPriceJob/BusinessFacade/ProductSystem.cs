using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.DataAccess;
using Tuhu.C.SyncProductPriceJob.Models;

namespace Tuhu.C.SyncProductPriceJob.BusinessFacade
{
    public static class ProductSystem
    {
        public static Task<bool> SavePrice(ProductPriceMappingModel model) => Products.SavePrice(model);
    }
}
