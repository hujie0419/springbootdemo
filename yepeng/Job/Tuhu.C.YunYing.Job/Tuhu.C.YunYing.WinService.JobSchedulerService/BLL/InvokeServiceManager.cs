using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Member;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.Service.Order.Models;
using Tuhu.Service.Order.Request;
using Tuhu.Service.Order.Response;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BLL
{
    public class InvokeServiceManager
    {
        /// <summary>
        /// 获取拆分订单
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        public static IEnumerable<CSplitOrderResponse> GetSplitOrders(IEnumerable<int> orderIds)
        {
            if (orderIds != null && orderIds.Any())
            {
                using (var client = new OrderApiForCClient())
                {
                    var serviceResult = client.GetSplitOrders(orderIds);
                    serviceResult.ThrowIfException(true);
                    return serviceResult.Result ?? new List<CSplitOrderResponse>();
                }
            }
            return new List<CSplitOrderResponse>();
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static CreateOrderResult CreateOrder(CreateOrderRequest request)
        {
            if (request != null)
            {
                using (var client = new CreateOrderClient())
                {
                    var serviceResult = client.CreateOrder(request);
                    serviceResult.ThrowIfException(true);
                    return serviceResult.Result;
                }
            }
            return null;
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static OrderCancelResp CancelOrder(CancelOrderRequest request)
        {
            using (var client = new OrderOperationClient())
            {
                var clientResult = client.CancelOrderForApp(request);
                clientResult.ThrowIfException(true);
                return clientResult.Result;
            }
        }


        /// <summary>
        /// 插入关联关系
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int InsertOrderRelationship(OrderRelationshipRequest request)
        {
            if (request != null)
            {
                using (var client = new OrderOperationClient())
                {
                    var serviceResult = client.InsertOrderRelationship(request);
                    serviceResult.ThrowIfException(true);
                    return serviceResult.Result;
                }
            }
            return 0;
        }

        /// <summary>
        /// 根据大客户Id获取大客户信息
        /// </summary>
        /// <param name="vipUserId"></param>
        /// <returns></returns>
        public static SYS_CompanyUser GetCompanyUserInfo(Guid vipUserId)
        {
            using (var client = new Service.UserAccount.UserAccountClient())
            {
                var serviceResult = client.SelectCompanyUserInfo(vipUserId);
                serviceResult.ThrowIfException(true);
                return serviceResult.Result;
            }
        }

        /// <summary>
        /// 根据OrderId获取优惠券Id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static long GetPromotionIdByOrderId(int orderId)
        {
            if (orderId > 0)
            {
                using (var client = new PromotionClient())
                {
                    var serviceResult = client.FetchPromotionCodeByOrderId(orderId);
                    serviceResult.ThrowIfException(true);
                    return serviceResult.Result?.Pkid ?? 0;
                }
            }
            return 0;
        }

        /// <summary>
        /// 查找关联订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static List<int> GetOrderRelationshipIds(OrderRelationshipTypeEnum orderRelationshipType, int orderId)
        {
            List<int> result = null;
            if (orderId > 0)
            {
                using (var client = new OrderQueryClient())
                {
                    var serviceResult = client.SelectOrderRelationshipByRelationshipTypeAndOrderId(orderRelationshipType, orderId);
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result;
                }
            }
            return result ?? new List<int>();
        }

        /// <summary>
        ///根据订单Id获取订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static OrderModel GetOrderById(int orderId)
        {
            using (var client = new OrderQueryClient())
            {
                var serviceResult = client.FetchOrderByOrderId(orderId);
                serviceResult.ThrowIfException(true);
                return serviceResult.Result;
            }
        }

        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ExecuteOrderProcess(ExecuteOrderProcessRequest request)
        {
            using (var client = new OrderOperationClient())
            {
                var clientResult = client.ExecuteOrderProcess(request);
                clientResult.ThrowIfException(true);
                return clientResult.Result;
            }
        }

    }
}
