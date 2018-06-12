using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Request;
using Tuhu.Service.Order.Response;

namespace Tuhu.Provisioning.Business.OrderServiceProxy
{
    public class OrderServiceProxy
    {
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OrderCancelResp>  CancelOrder(CancelOrderRequest request)
        {
            using (var client = new OrderOperationClient())
            {
                var clientResult = client.CancelOrderForApp(request);
                clientResult.ThrowIfException(true);
                return clientResult.Result;
            }
        }

    }
}
