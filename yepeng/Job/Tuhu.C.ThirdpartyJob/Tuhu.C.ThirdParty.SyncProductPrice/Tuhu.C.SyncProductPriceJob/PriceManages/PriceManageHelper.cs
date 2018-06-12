using Common.Logging;
using System.Collections.Concurrent;

namespace Tuhu.C.SyncProductPriceJob.PriceManages
{
    public static class PriceManageHelper
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PriceManageHelper));

        private static readonly ConcurrentDictionary<string, IPriceManage> Factory = new ConcurrentDictionary<string, IPriceManage>();

        public static IPriceManage GetPriceMangeInstance(Shop shop)
        {
            if (shop == null)
                return null;

            return Factory.GetOrAdd(shop.ShopCode, _ =>
            {
                switch (shop.ShopType)
                {
                    case ShopTypes.Jingdong:
                        return new JingDongPriceManage(shop.ShopCode, shop.SessionKey);

                    case ShopTypes.Suning:
                        return new SuningPriceManage(shop.ShopCode);

                    case ShopTypes.Qipeilong:
                    case ShopTypes.QplQingCang:
                    case ShopTypes.HubeiMaPai:
                        return new QipeilongPriceManage(shop.ShopCode);

                    default:
                        return new TaobaoPriceManage(shop.ShopCode, shop.SessionKey);
                }
            });
        }
    }
}
