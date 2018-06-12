using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.Service.Order.Models;
using Tuhu.Service.Order.Request;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BLL
{
    class OrderBusiness
    {
        private static readonly string orderChannel = "f大客户美容团购";
        private static readonly string orderType = "40大客户美容2B批";
        /// <summary>
        /// 创建导入用户买断订单
        /// </summary>
        /// <param name="vipUserId">大客户ID</param>
        /// <param name="vipSettleMentPrice">大客户结算价</param>
        /// <param name="pid">服务id</param>
        /// <param name="name">名字</param>
        /// <param name="num">服务数量</param>
        /// <returns></returns>
        public static  CreateOrderResult CreateServiceCodeOrderForVip(Guid vipUserId, decimal vipSettleMentPrice, string pid, string name, int num)
        {
            var vipuserInfo = UserAccountBusiness.GetCompanyUserInfo(vipUserId);
            var sumMoney = vipSettleMentPrice * num;
            var createOrderRequest = new CreateOrderRequest
            {
                OrderChannel = orderChannel,
                OrderType = orderType,
                Status = OrderEnum.OrderStatus.New,
                Customer = new OrderCustomer()
                {
                    UserId = vipUserId,
                    UserName = vipuserInfo.UserName,// wesureUser.Profile?.UserName,
                    UserTel = vipuserInfo.UserMobile
                },
                Delivery = new OrderDelivery
                {
                    DeliveryStatus = OrderEnum.DeliveryStatus.NotStarted,
                    DeliveryType = OrderEnum.DeliveryType.NoDelivery,
                    InstallType = OrderEnum.InstallType.NoInstall,
                },
                Payment = new OrderPayment
                {
                    PayStatus = OrderEnum.PayStatus.Waiting,
                    PayMothed = OrderEnum.PayMethod.MonthPay,
                    PaymentType = "5Special",
                },
                Money = new OrderMoney()
                {
                    SumMoney = sumMoney,
                    SumMarkedMoney = sumMoney
                },
                Items = new List<OrderItem>()
                {
                    new OrderItem()
                    {
                      Price = vipSettleMentPrice,
                      Pid = pid,
                      Num =  num,
                      Name = name,
                      Category = (ProductBusiness.GetProduct(pid))?.Category
                    }
                },
                SumNum = num
            };

            var result = CreateOrder(createOrderRequest);
            if (result.OrderId > 0)
            {
                var executeResult = InvokeServiceManager.ExecuteOrderProcess(new ExecuteOrderProcessRequest()
                {
                    CreateBy = vipuserInfo.UserName,
                    OrderId = result.OrderId,
                    OrderProcessEnum = OrderProcessEnum.GeneralCompleteToHome,
                    PayMethod = OrderEnum.PayMethod.MonthPay
                });
            }
			
            return result;
        }
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="createOrderRequest"></param>
        /// <returns></returns>
        public static  CreateOrderResult CreateOrder(CreateOrderRequest createOrderRequest)
        {
            CreateOrderResult result = null;
            using (var orderClient = new CreateOrderClient())
            {
                var createOrderResult =  orderClient.CreateOrder(createOrderRequest);
                createOrderResult.ThrowIfException(true);
                result = createOrderResult.Result;
            }
            return result;
        }
    }
}
