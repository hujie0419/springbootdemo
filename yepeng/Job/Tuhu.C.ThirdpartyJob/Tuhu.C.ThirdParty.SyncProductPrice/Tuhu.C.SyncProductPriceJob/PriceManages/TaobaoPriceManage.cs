using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Top.Api;
using Top.Api.Domain;
using Top.Api.Request;
using Tuhu.C.SyncProductPriceJob.BusinessFacade;
using Tuhu.C.SyncProductPriceJob.Helpers;
using Tuhu.C.SyncProductPriceJob.Models;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Models;
using Task = System.Threading.Tasks.Task;

namespace Tuhu.C.SyncProductPriceJob.PriceManages
{
    public class TaobaoPriceManage : BasePriceManage
    {
        private static readonly OpenApiConfigurationElement Config;

        // ReSharper disable once InconsistentNaming
        private static readonly ILog _logger;

        static TaobaoPriceManage()
        {
            Config = ShopManageConfiguration.ShopApiCollection.Get("Taobao");

            _logger = LogManager.GetLogger<TaobaoPriceManage>();
        }

        private readonly string _sessionKey;
        private readonly ITopClient _client;

        public TaobaoPriceManage(string shopCode, string sessionKey) : base(shopCode, _logger)
        {
            _sessionKey = sessionKey;

            _client = new AutoRetryTopClient(Config.ApiUrl, Config.AppKey, Config.AppSecret);
        }

        /// <summary>获得淘宝商品的价格</summary>
        /// <param name="itemId">商品编号</param>
        /// <returns>价格和名称</returns>
        public static async Task<ItemPriceModel> GetTabaoPrice(string itemId)
        {
            IHttpProxyClient client = null;
            try
            {
                client = new HttpProxyClient();

                var result = await client.GetTaobaoPriceAsync(itemId);
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

        private static readonly IDictionary<string, object> _lock = new Dictionary<string, object>();

        public void SendWarn(TopResponse response)
        {
            if (response.ErrCode == "27" && response.SubErrCode != null && response.SubErrCode.IndexOf("session", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                lock (_lock)
                {
                    if (!_lock.ContainsKey(ShopCode))
                    {
                        TuhuMessage.SendEmail(ShopCode + "的session过期，不能导订单了！", "电话中心管理组<ccguanli@tuhu.cn>", "屈作霖<quzuolin@tuhu.cn>;刘超<liuchao1@tuhu.cn>", $"<h1 style=\"font-size: 36px;\"><span style=\"color: red\">{ShopCode}</span>的session过期，不能导订单了！</h1><p>请使用{ShopCode}的主账号登录<a href=\"http://container.api.taobao.com/container?appkey=21729190\">http://container.api.taobao.com/container?appkey=21729190</a>，将登录成功后的结果发给<a href=\"mailto:屈作霖&lt;quzuolin@tuhu.cn&gt;\"><b>quzuolin@tuhu.cn</b></a></p>");

                        Logger.Info("已发送邮件提醒：" + ShopCode);
                        _lock[ShopCode] = new object();
                    }
                }
            }
        }

        protected override IReadOnlyCollection<ProductPriceMappingModel> SyncProductMappingsInternal()
        {
            var request = new ItemsOnsaleGetRequest
            {
                Fields = "num_iid,outer_id",
                PageSize = 100,
                PageNo = 1
            };

            var items = new List<Item>();
            while (true)
            {
                var response = RetryHelper.TryInvoke(() => _client.Execute(request, _sessionKey), _ => !_.IsError);

                if (response.IsError)
                {
                    Logger.Error(new LogModel { Api = request.GetApiName(), ShopCode = ShopCode, Message = "调用接口失败", ResponseContent = response.Body });

                    SendWarn(response);
                    break;
                }
                if (response.Items != null && response.Items.Count > 0)
                    items.AddRange(response.Items);

                if (response.TotalResults <= items.Count)
                    break;

                request.PageNo++;
            }
            //
            foreach (var array in items.Split(40))
            {
                var request1 = new ItemSkusGetRequest
                {
                    Fields = "num_iid,sku_id,outer_id",
                    NumIids = string.Join(",", array.Select(item => item.NumIid)) // sku所属商品数字id，必选。num_iid个数不能超过40个
                };

                var response1 = RetryHelper.TryInvoke(() => _client.Execute(request1, _sessionKey), _ => !_.IsError);

                if (response1.IsError)
                {
                    Logger.Error(new LogModel { Api = request.GetApiName(), ShopCode = ShopCode, Message = "调用接口失败", ResponseContent = response1.Body });

                    SendWarn(response1);
                }
                else
                {
                    Logger.Info(new LogModel { Message = "接口调用成功", ShopCode = ShopCode, Api = request.GetApiName(), ResponseContent = response1.Body });
                }

                foreach (var group in response1.Skus.GroupBy(sku => sku.NumIid))
                {
                    var item = items.FirstOrDefault(_ => _.NumIid == group.Key);
                    if (item == null)
                    {
                        continue;
                    }

                    item.Skus = new List<Sku>();

                    item.Skus.AddRange(group);
                }
            }

            var list = items.Where(_ => _.Skus == null || _.Skus.Count == 0).Select(item =>
                new ProductPriceMappingModel
                {
                    ItemId = item.NumIid,
                    ShopCode = ShopCode,
                    Pid = item.OuterId,
                    Title = item.Title,
                    Properties = item.Props
                }).ToList();

            list.AddRange(items.Where(_ => _.Skus != null && _.Skus.Count > 0).Select(item => item.Skus.Select(sku => new ProductPriceMappingModel
            {
                ItemId = item.NumIid,
                SkuId = sku.SkuId,
                Pid = sku.OuterId,
                Title = item.Title,
                Properties = sku.Properties,
                ShopCode = ShopCode
            })).SelectMany(_ => _));

            return list;
        }

        protected override async Task SyncPriceInternal(IReadOnlyCollection<ProductPriceMappingModel> items)
        {
            foreach (var group in items.GroupBy(_ => _.ItemId))
            {
                var item = group.First();
                var itemPrice = await GetTabaoPrice(item.ItemId.ToString());
                if (itemPrice != null)
                {
                    if (itemPrice.Skus == null || group.Count() == 1)
                    {
                        item.Price = itemPrice.Price;
                        item.Title = itemPrice.Title;

                        if (await ProductSystem.SavePrice(item))
                        {
                            Interlocked.Increment(ref TotalCount);
                            Logger.Info(new LogModel
                            {
                                Message = $"商品'{item.ItemId}'价格{item.Price:0.00}保存成功",
                                ShopCode = ShopCode
                            });
                        }
                        else
                        {
                            Interlocked.Increment(ref ErrorCount);
                            Logger.Error(new LogModel
                            {
                                Message = "商品" + item.ItemId + "价格保存失败，原因：商家外部编码没有对应的产品",
                                ShopCode = ShopCode
                            });
                        }
                    }
                    else
                    {
                        await SaveProductPrice(itemPrice.Skus.Select(skuPrice => new ProductPriceMappingModel
                        {
                            ShopCode = ShopCode,
                            Pid = item.Pid,
                            Title = itemPrice.Title,
                            ItemId = item.ItemId,
                            Price = skuPrice.Value.Price,
                            SkuId = Convert.ToInt64(skuPrice.Key),
                            Properties = skuPrice.Value.Properties
                        }).ToArray());
                    }
                }
                else
                {
                    Interlocked.Increment(ref ErrorCount);
                }
            }
        }
    }
}
