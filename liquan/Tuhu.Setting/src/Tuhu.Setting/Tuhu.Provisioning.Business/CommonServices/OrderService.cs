using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Models;

namespace Tuhu.Provisioning.Business.CommonServices
{
   public static  class OrderService
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(CommentService));

        public static OrderModel FetchOrderByOrderId(int orderId)
        {
            OrderModel order = null;
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = orderClient.FetchOrderByOrderId(orderId);
                    fetchResult.ThrowIfException(true);
                    order = fetchResult.Success ? fetchResult.Result : null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return order;
        }

        public static OrderModel FetchOrderAndListByOrderId(int orderId)
        {
            OrderModel order = null;
            try
            {
                using (var orderClient = new OrderApiForCClient())
                {
                    var fetchResult = orderClient.FetchOrderAndListByOrderId(orderId);
                    fetchResult.ThrowIfException(true);
                    order = fetchResult.Success ? fetchResult.Result : null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return order;
        }
    }
}
