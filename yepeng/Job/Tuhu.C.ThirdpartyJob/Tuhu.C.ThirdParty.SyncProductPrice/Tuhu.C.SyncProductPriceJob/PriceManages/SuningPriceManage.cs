extern alias Suning;

using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.BusinessFacade;
using Tuhu.C.SyncProductPriceJob.Helpers;
using Tuhu.C.SyncProductPriceJob.Models;
using DefaultSuningClient = Suning::suning_api_sdk.DefaultSuningClient;
using ISuningClient = Suning::suning_api_sdk.ISuningClient;
using ItemQuery = Suning::suning_api_sdk.Models.CustomItemModel.ItemQuery;
using ItemQueryRequest = Suning::suning_api_sdk.BizRequest.CustomItemRequest.ItemQueryRequest;
using PriceGetRequest = Suning::suning_api_sdk.BizRequest.CustomPriceRequest.PriceGetRequest;

namespace Tuhu.C.SyncProductPriceJob.PriceManages
{
    public class SuningPriceManage : BasePriceManage
    {
        private static readonly OpenApiConfigurationElement Config;

        static SuningPriceManage()
        {
            Config = ShopManageConfiguration.ShopApiCollection.Get("Suning");
        }

        private readonly ISuningClient _client;

        public SuningPriceManage(string shopCode) : base(shopCode, LogManager.GetLogger<SuningPriceManage>())
        {
            _client = new DefaultSuningClient(Config.ApiUrl, Config.AppKey, Config.AppSecret);
        }

        protected override IReadOnlyCollection<ProductPriceMappingModel> SyncProductMappingsInternal()
        {
            var request = new ItemQueryRequest
            {
                status = "2",
                pageSize = 50,
                pageNo = 1
            };

            var items = new List<ItemQuery>();
            while (true)
            {
                var response = RetryHelper.TryInvoke(() => _client.Execute(request), _ => _.respError == null);

                if (response.respError != null)
                {
                    Logger.Error(new LogModel { Api = request.GetAppMethod(), ShopCode = ShopCode, Message = "调用接口失败", ResponseContent = response.ToJson() });
                    break;
                }
                if (response.item != null && response.item.Count > 0)
                {
                    items.AddRange(response.item);
                }

                if (response.respHead.totalSize <= items.Count)
                {
                    break;
                }

                request.pageNo++;
            }

            var list = items.Where(_ => _.childItem == null || _.childItem.Count == 0).Select(item =>
                new ProductPriceMappingModel
                {
                    ItemId = Convert.ToInt64(item.productCode),
                    Pid = string.IsNullOrWhiteSpace(item.itemCode) ? "" : item.itemCode,
                    ShopCode = ShopCode,
                    Title = item.productName
                }).ToList();
            list.AddRange(items.Where(_ => _.childItem != null && _.childItem.Count > 0).Select(_ => _.childItem.Select(item => new ProductPriceMappingModel
            {
                ItemId = Convert.ToInt64(_.productCode),
                SkuId = Convert.ToInt64(item.productCode),
                Pid = string.IsNullOrWhiteSpace(item.itemCode) ? "" : item.itemCode,
                ShopCode = ShopCode,
                Title = item.productName
            })).SelectMany(_ => _));
            return list;
        }

        protected override async Task SyncPriceInternal(IReadOnlyCollection<ProductPriceMappingModel> items)
        {
            foreach (var group in items.GroupBy(_ => _.ItemId))
            {
                var array = group.ToArray();
                var item = array[0];
                var price = GetProductPrice(item.ItemId.ToString());
                if (price.HasValue && price > 0)
                {
                    item.Price = price.Value;
                    if (array.Length > 1)
                    {
                        await SaveProductPrice(array, price.Value);
                    }
                    else
                    {
                        if (await ProductSystem.SavePrice(item))
                        {
                            Interlocked.Increment(ref TotalCount);
                            Logger.Info(new LogModel
                            {
                                Message = $"商品'{item.ItemId}'价格{price}保存成功",
                                ShopCode = ShopCode
                            });
                        }
                        else
                        {
                            Interlocked.Increment(ref ErrorCount);
                            Logger.Error(new LogModel
                            {
                                Message = $"商品'{item.ItemId}'价格保存失败，原因：商家外部编码没有对应的产品",
                                ShopCode = ShopCode
                            });
                        }
                    }
                }
                else
                {
                    Interlocked.Increment(ref ErrorCount);
                }
            }
        }

        private decimal? GetProductPrice(string productCode)
        {
            var request = new PriceGetRequest
            {
                productCode = productCode
            };
            try
            {
                var response = RetryHelper.TryInvoke(() => _client.Execute(request), _ => _ != null && _.respError == null);

                if (response.respError != null)
                {
                    Logger.Error(new LogModel { Api = request.GetAppMethod(), ShopCode = ShopCode, Message = "调用接口失败", ResponseContent = response?.ToJson() });
                    return null;
                }
                return Convert.ToDecimal(response.price);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }
    }
}
