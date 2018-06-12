using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Tuhu.C.SyncProductPriceJob
{
    public static class ShopManageConfiguration
    {
        public static OpenApiConfigurationElementCollection ShopApiCollection { get; }

        public static IDictionary<string, Shop> ShopSessionPools { get; } = new Dictionary<string, Shop>();

        static ShopManageConfiguration()
        {
            var logger = LogManager.GetLogger(typeof(ShopManageConfiguration));

            logger.Info("开始加载配置");

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            ShopApiCollection = (config.GetSection("shopApiConfig") as OpenApiConfigurationSection).OpenApiConfig;

            var eticketSession = config.GetSection("shopSessionSettings") as AppSettingsSection;
            if (eticketSession != null)
            {
                foreach (var shop in eticketSession.Settings
                    .Cast<KeyValueConfigurationElement>()
                    .ToDictionary(s => s.Key, s =>
                    {
                        var arr = s.Value.Split('|');
                        return new Shop
                        {
                            ShopCode = arr[0],
                            ShopType = (ShopTypes)Convert.ToByte(arr[1]),
                            SessionKey = arr[2],
                            NickName = arr.Length > 3 ? arr[3] : null
                        };
                    }))
                {
                    logger.Info("加载授权码，商铺代码：" + shop.Key);

                    ShopSessionPools[shop.Key] = shop.Value;
                }
            }

            logger.Info("结束加载配置");
        }
    }
}
