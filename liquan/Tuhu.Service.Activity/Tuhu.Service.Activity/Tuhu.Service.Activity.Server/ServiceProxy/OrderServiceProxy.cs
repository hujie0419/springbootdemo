using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.Service.Order.Models;

namespace Tuhu.Service.Activity.Server.ServiceProxy
{
    public class OrderServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OrderServiceProxy));

        /// <summary>
        /// 根据号判断订单产品类型
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static async Task<string> CheckOrderProductTypeByOrderIdAsync(int orderId)
        {
            var result = string.Empty;
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = await orderClient.CheckOrderProductTypeByOrderIdAsync(orderId);
                    fetchResult.ThrowIfException(true);
                    result = fetchResult.Success ? fetchResult.Result : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CheckOrderProductTypeByOrderIdAsync 接口异常", ex);
            }
            return result ?? string.Empty;
        }
        /// <summary>
        /// 获取所有第三方订单渠道
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<string>> Get3rdOrderChannel()
        {
            IEnumerable<string> result = new List<string>();
            try
            {
                using (var client = new OrderQueryClient())
                {
                    var getResult = await client.Get3rdOrderChannelAsync();//获取三方渠道集合
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CheckOrderProductTypeByOrderIdAsync 接口异常", ex);
            }
            return result;
        }
        /// <summary>
        /// 根据订单PKID获取订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static async Task<OrderModel> FetchOrderByOrderId(int orderId)
        {
            OrderModel order = null;
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = await orderClient.FetchOrderByOrderIdAsync(orderId);
                    fetchResult.ThrowIfException(true);
                    order = fetchResult.Success ? fetchResult.Result : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FetchOrderByOrderId 接口异常", ex);
            }
            return order;
        }
        /// <summary>
        /// 获取订单信息(订单主信息和产品)
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static async Task<OrderModel> FetchOrderInfoByID(int orderId)
        {
            OrderModel order = null;
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = await orderClient.FetchOrderInfoByIDAsync(orderId);
                    fetchResult.ThrowIfException(true);
                    order = fetchResult.Success ? fetchResult.Result : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("FetchOrderInfoByID 接口异常", ex);
            }
            return order;
        }
        /// <summary>
        /// 根据订单ID获取拆单订单的相关订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<int>> GetRelatedSplitOrderIDsAsync(int orderId, SplitQueryType type)
        {
            IEnumerable<int> order = null;
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = await orderClient.GetRelatedSplitOrderIDsAsync(orderId, type);
                    fetchResult.ThrowIfException(true);
                    order = fetchResult.Success ? fetchResult.Result : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetRelatedSplitOrderIDsAsync 接口异常", ex);
            }
            return order;
        }

    }
}
