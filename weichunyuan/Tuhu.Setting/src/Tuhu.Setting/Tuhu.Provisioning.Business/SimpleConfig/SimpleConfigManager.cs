using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models.New;

namespace Tuhu.Provisioning.Business.SimpleConfig
{
    public class SimpleConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger(nameof(SimpleConfigManager));

        public FuelCardConfig SelectFuelCardConfig()
        {
            FuelCardConfig result = new FuelCardConfig();
            try
            {
                var config = DalSimpleConfig.SelectConfig("FuelCardConfig");
                if (!string.IsNullOrWhiteSpace(config))
                {
                    result = XmlHelper.Deserialize<FuelCardConfig>(config);
                    if (result.CardTypes == null || !result.CardTypes.Any())
                    {
                        return result;
                    }
                    foreach (var item in result.CardTypes)
                    {
                        if (item.FuelCards == null || !item.FuelCards.Any())
                        {
                            continue;
                        }
                        var pids = item.FuelCards.Select(p => p.Pid).ToList();
                        var fuelCards = new List<SkuProductDetailModel>();
                        using (var client = new ProductClient())
                        {
                            var products = client.SelectSkuProductListByPids(pids);
                            if (!products.Success || products.Result == null)
                            {
                                products.ThrowIfException(true);
                            }
                            fuelCards = products.Result;
                        }
                        foreach (var card in item.FuelCards)
                        {
                            if (fuelCards.Any(t => t.Pid.Equals(card.Pid)))
                            {
                                var fuelCard = fuelCards.FirstOrDefault(p => p.Pid.Equals(card.Pid));
                                if (!fuelCard.Onsale)
                                {
                                    Logger.Log(Level.Warning, $"加油卡:{JsonConvert.SerializeObject(card)} ,该产品已下架，详细数据：{JsonConvert.SerializeObject(fuelCard)}");
                                    item.FuelCards.Remove(card);
                                    continue;
                                }
                                if (fuelCard.Stockout)
                                {
                                    Logger.Log(Level.Warning, $"加油卡:{JsonConvert.SerializeObject(card)} ,该产品无货，详细数据：{JsonConvert.SerializeObject(fuelCard)}");
                                    item.FuelCards.Remove(card);
                                    continue;
                                }
                                if (fuelCard.MarketingPrice != null && fuelCard.MarketingPrice > 0 &&
                                    fuelCard.Price >= 0)
                                {
                                    card.Value = (decimal)fuelCard.MarketingPrice;
                                    card.Price = fuelCard.Price;
                                }
                                else
                                {
                                    Logger.Log(Level.Warning, $"加油卡:{JsonConvert.SerializeObject(card)} ,面值或价格数据不合法，详细数据：{JsonConvert.SerializeObject(fuelCard)}");
                                    item.FuelCards.Remove(card);
                                }
                            }
                            else
                            {
                                Logger.Log(Level.Warning, $"加油卡:{JsonConvert.SerializeObject(card)} ,查不到产品id");
                                item.FuelCards.Remove(card);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "SelectFuelCardConfig");
            }
            return result;
        }

        public bool UpdateConfig(string configName, object config)
        {
            var result = false;            
            try
            {
                using (StringWriter xml = new StringWriter())
                {
                    XmlSerializer xz = new XmlSerializer(typeof(FuelCardConfig));
                    xz.Serialize(xml, config);
                    result = DalSimpleConfig.UpdateConfig(configName, xml.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "UpdateConfig");
            }
            return result;
        }
    }
}
