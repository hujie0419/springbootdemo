using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models;

namespace Tuhu.Provisioning.Business.ShopServiceProxy
{
    public class ShopServiceProxy
    {
        private ShopServiceProxy()
        {
        }
        private static ShopServiceProxy _Instanse;
        public static ShopServiceProxy Instanse
        {
            get
            {
                if (_Instanse == null)
                {
                    _Instanse = new ShopServiceProxy();
                }
                return _Instanse;
            }
        }
        public async Task<IEnumerable<ShopDetailModel>> SelectShopDetailsAsync(IEnumerable<int> shopIds)
        {
            using (var client = new ShopClient())
            {
                var result = await client.SelectShopDetailsAsync(shopIds);
                result.ThrowIfException(true);
                return result.Result;
            }
        }

    }
}
