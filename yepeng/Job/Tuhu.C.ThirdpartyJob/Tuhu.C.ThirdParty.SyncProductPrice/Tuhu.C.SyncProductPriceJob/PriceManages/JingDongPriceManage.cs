using Common.Logging;
using JdSdk;
using JdSdk.Domain;
using JdSdk.Request;
using JdSdk.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.BusinessFacade;
using Tuhu.C.SyncProductPriceJob.Helpers;
using Tuhu.C.SyncProductPriceJob.Models;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Models;

namespace Tuhu.C.SyncProductPriceJob.PriceManages
{
    public class JingDongPriceManage : BasePriceManage
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ILog _logger = LogManager.GetLogger<JingDongPriceManage>();

        private readonly string _sessionKey;
        private readonly DefaultJdClient _client;

        public JingDongPriceManage(string shopCode, string sessionKey) : base(shopCode, _logger)
        {
            _sessionKey = sessionKey;

            var config = ShopManageConfiguration.ShopApiCollection.Get(shopCode);

            _client = new DefaultJdClient(config.ApiUrl, config.AppKey, config.AppSecret, sessionKey);
        }

        /// <summary>获得京东的价格</summary>
        /// <param name="itemId"></param>
        /// <returns>Item1：价格；Item2：是否促销；Item3：商品名称</returns>
        public static async Task<ItemPriceModel> GetJingDongPrice(string itemId)
        {
            IHttpProxyClient client = null;
            try
            {
                client = new HttpProxyClient();

                var result = await client.GetJingdongPriceAsync(itemId);

                result.ThrowIfException(true);

                return result.Result;
            }
            catch (Exception ex)
            {
                _logger.Warn(new LogModel { Message = $"下载商品信息失败,{ex}", RefNo = itemId });

                return null;
            }
            finally
            {
                client?.Dispose();
            }
        }

        protected override IReadOnlyCollection<ProductPriceMappingModel> SyncProductMappingsInternal()
        {
            var request = new WareListingGetRequest
            {
                Fields = "ware_id,title",
                PageSize = 100,
                Page = 1
            };

            var items = new List<Ware>();
            while (true)
            {
                var response = RetryHelper.TryInvoke(() => _client.Execute(request, _sessionKey), _ => !_.IsError);

                if (response.IsError)
                {
                    Logger.Error(new LogModel { Api = request.ApiName, ShopCode = ShopCode, Message = "调用接口失败", ResponseContent = response.Body });
                    break;
                }
                if (response.Wares != null && response.Wares.Count > 0)
                {
                    items.AddRange(response.Wares);
                }
#if DEBUG
                break;
#endif
                if (response.Total <= items.Count)
                {
                    break;
                }

                request.Page++;
            }
            var skus = new List<Sku>();
            foreach (var group in items.Split(10))
            {
                var request1 = new WareSkusGetRequest();
                request1.Fields = "ware_id,sku_id,outer_id,status,attributes";
                request1.WareIds = string.Join(",", group.Select(item => item.WareId.Value)); //ware_ids个数不能超过10个

                var response1 = RetryHelper.TryInvoke(() =>
                {
                    try
                    {
                        return _client.Execute(request1, _sessionKey);
                    }
                    catch (WebException e)
                    {
                        Logger.Warn(new LogModel
                        {
                            Api = request.ApiName,
                            ShopCode = ShopCode,
                            RefNo = request1.WareIds,
                            Message = $"调用京东接口获取产品价格发生网络异常：异常信息{e}"
                        });
                        return new WareSkusGetResponse
                        {
                            ErrCode = "-99998",
                            ZhErrMsg = e.Message
                        };
                    }
                    catch (Exception e)
                    {
                        Logger.Warn(new LogModel
                        {
                            Api = request.ApiName,
                            ShopCode = ShopCode,
                            RefNo = request1.WareIds,
                            Message = $"调用京东接口获取产品价格发生异常：{e}"
                        });
                        return new WareSkusGetResponse
                        {
                            ErrCode = "-99999",
                            ZhErrMsg = e.Message
                        };
                    }
                }, _ => !_.IsError);

                if (response1.IsError)
                {
                    Logger.Error(new LogModel { Api = request.ApiName, Message = "调用接口失败", ResponseContent = response1.Body });
                }
                else
                {
                    skus.AddRange(response1.Skus);
                }
            }

            return skus.Select(_ =>
            {
                var item = items.FirstOrDefault(i => i.WareId == _.WareId);
                if (null == item || !"Valid".Equals(_.Status, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                return new ProductPriceMappingModel
                {
                    ItemId = item.WareId.GetValueOrDefault(),
                    SkuId = _.SkuId.GetValueOrDefault(),
                    Pid = _.OuterId,
                    ShopCode = ShopCode,
                    Title = item.Title,
                    Properties = _.Attributes
                };
            }).Where(_ => null != _).ToArray();
        }

        protected override async Task SyncPriceInternal(IReadOnlyCollection<ProductPriceMappingModel> items)
        {
            foreach (var mapping in items)
            {
                var jdPrice = await GetJingDongPrice(mapping.SkuId.ToString());
                if (jdPrice == null)
                {
                    Logger.Warn(new LogModel { Message = mapping.SkuId + "获取价格失败", ShopCode = ShopCode, RefNo = mapping.SkuId.ToString() });
                    Interlocked.Increment(ref ErrorCount);
                }
                else
                {
                    mapping.Price = jdPrice.Price;
                    mapping.Price2 = jdPrice.Price2;
                    if (await ProductSystem.SavePrice(mapping))
                    {
                        Interlocked.Increment(ref TotalCount);
                        Logger.Info(new LogModel
                        {
                            Message = $"sku商品'{mapping.SkuId}'价格{jdPrice.Price:0.00}保存成功",
                            ShopCode = ShopCode
                        });
                    }
                    else
                    {
                        Interlocked.Increment(ref ErrorCount);
                        Logger.Error(new LogModel
                        {
                            Message = $"sku商品'{mapping.SkuId}'价格保存失败",
                            ShopCode = ShopCode
                        });
                    }
                }
            }
        }
    }
}
