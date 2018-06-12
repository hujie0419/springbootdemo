using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models;
using Tuhu.Service.TuhuShopWork;
using Tuhu.Service.TuhuShopWork.Models.ShopInfo;
namespace Tuhu.CarArchiveService.Utils
{
    public class ServicesManager
    {
        public static RoadOperationPermit[] FetchRoadOperationPermit(int shopId)
        {
            using (var client = new ShopBasicInfoClient())
            {
                var result = client.FetchRoadOperationPermit(shopId);
                result.ThrowIfException(true);
                return result.Result?.ToArray();
            }
        }

        public static ShopDetailModel FetchShopDetail(int shopId)
        {
            using (var client = new ShopClient())
            {
                var result = client.FetchShopDetail(shopId);
                result.ThrowIfException(true);
                return result.Result;
            }
        }
    }
}
