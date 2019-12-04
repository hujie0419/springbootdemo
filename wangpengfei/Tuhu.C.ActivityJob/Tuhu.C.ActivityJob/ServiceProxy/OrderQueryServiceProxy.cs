using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;

namespace Tuhu.C.ActivityJob.ServiceProxy
{
    public class OrderQueryServiceProxy
    {
        private static readonly ILog _logger = LogManager.GetLogger<OrderQueryServiceProxy>();

        /// <summary>
        /// 获取订单的拆单订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static List<int> GetRelatedSplitOrderIds(int orderId)
        {
            try
            {
                using (var queryClient = new OrderQueryClient())
                {
                    var queryResult = queryClient.GetRelatedSplitOrderIDs(orderId, SplitQueryType.Full);
                    queryResult.ThrowIfException(true);
                    return queryResult.Result?.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"获取订单的拆单订单失败：{orderId}", ex);
                return null;
            }
        }
    }
}
