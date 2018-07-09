using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Server.Manager;
using Tuhu.Service.Order.Request;
using CreateOrderResult = Tuhu.Service.Activity.Models.CreateOrderResult;
using CreateOrderRequest= Tuhu.Service.Order.Request.CreateOrderRequest;
using Tuhu.Service.Product;

namespace Tuhu.Service.Activity.Server
{
    public class FlashSaleCreateOrderService : IFlashSaleCreateOrderService
    {

        public async Task<OperationResult<CreateOrderResult>> FlashSaleCreateOrderAsync(CreateOrderRequest request)
        {
            return OperationResult.FromResult(await FlashSaleCreateOrderManager.FlashSaleCreateOrder(request));
        }
        public async Task<OperationResult<IEnumerable<ActivityPriceModel>>> FetchActivityProductPriceAsync(ActivityPriceRequest request)
        {
            if (request.Items.Count < 1)
            {
                return OperationResult.FromError<IEnumerable<ActivityPriceModel>>(ErrorCode.ParameterError, "参数不正确");
            }
            var result = await FlashSaleCreateOrderManager.FetchActivityProductPrice(request);
            if (result == null)
            {
                return OperationResult.FromError<IEnumerable<ActivityPriceModel>>(ErrorCode.ParameterError, "该商品未找到活动价");
            }
            return OperationResult.FromResult(result);
        }
    }
}
