using Common.Logging;
using Qpl.Api;
using Qpl.Api.Models;
using Qpl.Api.Request;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.Models;

namespace Tuhu.C.SyncProductPriceJob.PriceManages
{
    public class QipeilongPriceManage : IPriceManage
    {
        private static readonly OpenApiConfigurationElement Config;
        private static readonly ILog Logger;

        static QipeilongPriceManage()
        {
            Config = ShopManageConfiguration.ShopApiCollection.Get("Qipeilong");

            Logger = LogManager.GetLogger<QipeilongPriceManage>();
        }

        private readonly string _shopCode;
        private readonly IQplClient _client;

        public QipeilongPriceManage(string shopCode)
        {
            _shopCode = shopCode;

            _client = new QplClient(Config.ApiUrl, Config.AppKey, Config.AppSecret);
        }

        public Task SyncProductPrice() => Task.CompletedTask;

        public Task SyncProductsPrice(IReadOnlyCollection<ProductPriceMappingModel> products)
            => Task.CompletedTask;

        /// <summary>获得汽配龙商品的价格</summary>
        /// <param name="pid">商品编号</param>
        /// <returns>价格和名称</returns>
        public async Task<QplProductInfoModel> GetQplPrice(string pid)
        {
            var request = new SyncProductInfoRequest { Pid = pid };

            var response = await _client.ExecuteAsync(request);
            if (response.IsError)
            {
                Logger.Error(new LogModel { ShopCode = _shopCode, Message = "下载商品信息失败", RefNo = pid, Api = request.ApiName, ResponseContent = response.Body });
                return null;
            }
            return response.Product;
        }

        public Task SyncProductMapping() => Task.CompletedTask;
    }
}
