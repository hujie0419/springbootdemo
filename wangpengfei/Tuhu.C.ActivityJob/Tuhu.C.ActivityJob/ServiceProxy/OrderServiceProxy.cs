using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.Service.Order.Models;

namespace Tuhu.C.ActivityJob.ServiceProxy
{
    public class OrderServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OrderServiceProxy));

        /// <summary>
        /// 根据订单号判断订单产品类型
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static string CheckOrderProductTypeByOrderId(int orderId)
        {
            var result = string.Empty;
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = orderClient.CheckOrderProductTypeByOrderId(orderId);
                    if (!fetchResult.Success)
                    {
                        Logger.Warn($"CheckOrderProductTypeByOrderId,根据订单号判断订单产品类型接口失败，message:{fetchResult.ErrorMessage}");
                    }
                    else
                    {
                        result = fetchResult.Result ?? string.Empty;
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Error($"CheckOrderProductTypeByOrderId接口异常，异常信息：{ex.Message}，堆栈信息：{ex.StackTrace}");
            }
            return result;
        }

        /// <summary>
        /// 获取订单信息(订单主信息和产品)
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static OrderModel FetchOrderInfoByID(int orderId)
        {
            OrderModel order = null;
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = orderClient.FetchOrderInfoByID(orderId);
                    if (!fetchResult.Success)
                    {
                        Logger.Warn($"FetchOrderInfoByID,获取订单信息接口失败，message:{fetchResult.ErrorMessage}");
                    }
                    else
                    {
                        order = fetchResult.Result;
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Error($"FetchOrderInfoByID接口异常，异常信息：{ex.Message}，堆栈信息：{ex.StackTrace}");
            }
            return order;
        }

        /// <summary>
        /// 根据订单ID获取拆单订单的相关订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<int> GetRelatedSplitOrderIDs(int orderId, SplitQueryType type)
        {
            List<int> order = new List<int>();
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = orderClient.GetRelatedSplitOrderIDs(orderId, type);
                    if (!fetchResult.Success)
                    {
                        Logger.Warn($"GetRelatedSplitOrderIDs,根据订单ID获取拆单订单的相关订单接口失败，message：{fetchResult.ErrorMessage}");
                    }
                    else
                    {
                        order = fetchResult.Result == null ? new List<int>() : fetchResult.Result.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetRelatedSplitOrderIDs 接口异常，异常信息：{ex.Message}，堆栈信息：{ex.StackTrace}");
            }
            return order;
        }
    }
}
