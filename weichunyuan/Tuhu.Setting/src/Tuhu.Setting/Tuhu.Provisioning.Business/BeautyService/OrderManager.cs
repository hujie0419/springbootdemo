using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.Service.Order.Models;
using Tuhu.Service.Order.Request;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.Provisioning.Business.BeautyService
{
    public class OrderManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OrderManager));
        private static readonly string orderChannel = "f大客户美容团购";
        private static readonly string orderType = "40大客户美容2B批";
        public static async Task<CreateOrderResult> CreatePackageCodeOrderForVipUser(BeautyServicePackage package, IEnumerable<BeautyServicePackageDetail> packageDetails)
        {
            var vipuserInfo = await GetCompanyUserInfo(package.VipUserId);
            var totalMoney = packageDetails.Sum(t => t.VipSettlementPrice * t.Num) * package.PackageCodeNum;
            var createOrderRequest = new CreateOrderRequest
            {
                OrderChannel = orderChannel,
                OrderType = orderType,
                Status = OrderEnum.OrderStatus.New,
                Customer = new OrderCustomer()
                {
                    UserId = package.VipUserId,
                    UserName = package.VipUserName,// wesureUser.Profile?.UserName,
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
                    SumMoney = totalMoney,
                    SumMarkedMoney = totalMoney
                },
                Items = packageDetails.Select(p => new OrderItem()
                {
                    Price = p.VipSettlementPrice,
                    Pid = p.PID,
                    Num = package.PackageCodeNum * p.Num,
                    Name = p.Name,
                    Category = (GetProduct(p.PID))?.Category
                }),
                BigCustomerCompanyId = vipuserInfo.CompanyInfo?.Id,
                BigCustomerCompanyName = vipuserInfo.CompanyInfo?.Name,
                SumNum = packageDetails.Sum(t => t.Num) * package.PackageCodeNum
            };
            var result = await CreateOrder(createOrderRequest);
            if (result.OrderId > 0)
            {
                ExecuteOrderProcess(new ExecuteOrderProcessRequest()
                {
                    CreateBy = vipuserInfo.UserName,
                    OrderId = result.OrderId,
                    OrderProcessEnum = OrderProcessEnum.GeneralCompleteToHome,
                    PayMethod = OrderEnum.PayMethod.MonthPay
                });
            }
            return result;
        }

        public static async Task<CreateOrderResult> CreateServiceCodeOrderForVip(MrCooperateUserConfig cooperateUser, BeautyServicePackageDetail packageDetail)
        {
            var vipuserInfo = await GetCompanyUserInfo(cooperateUser.VipUserId);
            var sumMoney = packageDetail.VipSettlementPrice * packageDetail.ServiceCodeNum;
            var createOrderRequest = new CreateOrderRequest
            {
                OrderChannel = orderChannel,
                OrderType = orderType,
                Status = OrderEnum.OrderStatus.New,
                Customer = new OrderCustomer()
                {
                    UserId = cooperateUser.VipUserId,
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
                Items = new List<OrderItem>() {  new OrderItem()
                {
                    Price = packageDetail.VipSettlementPrice,
                    Pid = packageDetail.PID,
                    Num =  packageDetail.ServiceCodeNum,
                    Name = packageDetail.Name,
                    Category = (GetProduct(packageDetail.PID))?.Category
                }},
                BigCustomerCompanyId = vipuserInfo.CompanyInfo?.Id,
                BigCustomerCompanyName = vipuserInfo.CompanyInfo?.Name,
                SumNum = packageDetail.ServiceCodeNum
            };
            var result = await CreateOrder(createOrderRequest);
            if (result.OrderId > 0)
            {
                ExecuteOrderProcess(new ExecuteOrderProcessRequest()
                {
                    CreateBy = vipuserInfo.UserName,
                    OrderId = result.OrderId,
                    OrderProcessEnum = OrderProcessEnum.GeneralCompleteToHome,
                    PayMethod = OrderEnum.PayMethod.MonthPay
                });
            }

            return result;
        }

        public static async Task<CreateOrderResult> CreateOrder(CreateOrderRequest createOrderRequest)
        {
            CreateOrderResult result = null;
            using (var orderClient = new CreateOrderClient())
            {
                var createOrderResult = await orderClient.CreateOrderAsync(createOrderRequest);
                createOrderResult.ThrowIfException(true);
                result = createOrderResult.Result;
            }
            return result;
        }
        public static async Task<SYS_CompanyUser> GetCompanyUserInfo(Guid userId)
        {
            SYS_CompanyUser result = null;

            using (var client = new UserAccountClient())
            {
                var serviceResult = await client.SelectCompanyUserInfoAsync(userId);
                serviceResult.ThrowIfException(true);
                result = serviceResult.Result;
            }

            return result;
        }

        public static ProductModel GetProduct(string pid)
        {
            ProductModel product = null;

            using (var productClient = new ProductClient())
            {
                var productResult = productClient.FetchProduct(pid);
                productResult.ThrowIfException(true);
                product = productResult.Result;
            }

            return product;
        }
        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
		
        public static string ExecuteOrderProcess(ExecuteOrderProcessRequest request)
        {
            var result = string.Empty;

            try
            {
                using (var client = new OrderOperationClient())
                {
                    var clientResult = client.ExecuteOrderProcess(request);
                    clientResult.ThrowIfException(true);
                    result = clientResult.Result;
                    if (!string.IsNullOrEmpty(result))
                    {
                        Logger.Warn($"更新订单{request.OrderId}失败,result:{result}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;
        }
    }
}
