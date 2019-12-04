using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.Request;
using Tuhu.Service.Promotion.Response;
using Tuhu.Service.Promotion.Server.Config;
using Tuhu.Service.Promotion.Server.Manager.IManager;
using Tuhu.Service.Promotion.Server.ServiceProxy;
using Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using Tuhu.Service.Promotion.DataAccess.IRepository;

namespace Tuhu.Service.Promotion.Server
{
    /// <summary>
    /// 优惠券任务相关 接口
    /// </summary>
    public class PromotionTaskController : PromotionTaskService
    {
        public ICouponTaskManager couponTaskManager;
        public ICommonManager commonManager;
        private readonly ILogger logger;
        private readonly IProductService productQueryService;
        private readonly IOrderService orderService;
        private readonly IShopService shopService;
        private readonly IUserAccountService userAccountService;
        private readonly ICreatePromotionService createPromotionService;
        private readonly IPromotionSingleTaskUsersHistoryRepository promotionSingleTaskUsersHistoryRepository;
        
        AppSettingOptions appSettingOptions;

        public PromotionTaskController(
            ICouponTaskManager ICouponTaskManager,
            ICommonManager ICommonManager,
            ILogger<PromotionTaskController> Logger,
            IProductService IProductQueryService,
            IShopService IShopService,
            IUserAccountService IUserAccountService,
            IOrderService IOrderService,
            IOptionsSnapshot<AppSettingOptions> AppSettingOptions,
            IPromotionSingleTaskUsersHistoryRepository IPromotionSingleTaskUsersHistoryRepository,
            ICreatePromotionService ICreatePromotionService
        )
        {
            couponTaskManager = ICouponTaskManager;
            commonManager = ICommonManager;
            logger = Logger;
            productQueryService = IProductQueryService;
            shopService = IShopService;
            userAccountService = IUserAccountService;
            orderService = IOrderService;
            appSettingOptions = AppSettingOptions.Value;
            promotionSingleTaskUsersHistoryRepository = IPromotionSingleTaskUsersHistoryRepository;
            createPromotionService = ICreatePromotionService;
        }

        /// <summary>
        /// 获取优惠券 任务列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<List<PromotionTaskModel>>> GetPromotionTaskListAsync([FromBody] GetPromotionTaskListRequest request)
        {
            var result = await couponTaskManager.GetPromotionTaskListMemoryCacheAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 根据订单号匹配有效的优惠券任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<GetPromotionTaskListByOrderIdResponse>> GetPromotionTaskListByOrderIdAsync([FromBody] GetPromotionTaskListByOrderIdRequest request)
        {
            var promotionTaskList = request.PromotionTaskList;
            //1.获取订单详情
            var orderResult = await orderService.FetchOrderAndListByOrderIdAsync(request.OrderID, false, HttpContext.RequestAborted).ConfigureAwait(false);
            if (!orderResult.Success || orderResult.Result == null)
            {
                return OperationResult.FromError<GetPromotionTaskListByOrderIdResponse>("-1", "获取订单详情异常");
            }
            var order = orderResult.Result;
            //2.获取产品和门店信息
            var shopResult = shopService.FetchShopAsync(order.InstallShopId ?? 0, HttpContext.RequestAborted);
            var productResult = productQueryService.SelectProductBaseInfoAsync(order.OrderListModel.Select(p => p.Pid).ToList());
            //3.获取优惠券任务配置的订单渠道
            var orderChannelListResult =  couponTaskManager.GetAllOrderChannelMemoryCacheAsync(HttpContext.RequestAborted);
            await Task.WhenAll(shopResult, productResult, orderChannelListResult).ConfigureAwait(false);
            var shop = shopResult.Result.Result;
            var productList = productResult.Result.Result;
            var orderChannelList = orderChannelListResult.Result;

            //4.以订单维度进行筛选 [使用 Parallel 会存在异常]

            //定义task 数组，后面一起执行，提高执行效率
            List<Task<bool>> allTask = new List<Task<bool>>();
            foreach (var task in promotionTaskList)
            {
                allTask.Add(couponTaskManager.CheckPromotionTaskWithOrderAsync(request, task, order, productList, shop, orderChannelList, HttpContext.RequestAborted));
            }
            await Task.WhenAll(allTask).ConfigureAwait(false);

            //foreach (var task in promotionTaskList)
            //{
            //    await couponTaskManager.CheckPromotionTaskWithOrderAsync(request,task, order, productList, shop, orderChannelList, HttpContext.RequestAborted).ConfigureAwait(false);
            //}

            //Parallel.ForEach(promotionTaskList, new ParallelOptions() { MaxDegreeOfParallelism = appSettingOptions.CouponTaskMaxDegreeOfParallelism }, async task =>
            // {
            //    await couponTaskManager.CheckPromotionTaskWithOrderAsync(task, order, productList, shop, orderChannelList, HttpContext.RequestAborted).ConfigureAwait(false);
            // });

            var result = new GetPromotionTaskListByOrderIdResponse()
            {
                PromotionTaskList = promotionTaskList,
                OrderStatus = order.Status,
                UserID= order.UserId,
            };

            logger.Warn($"GetPromotionTaskListByOrderIdAsync  orderid= {request.OrderID} CouponTaskMaxDegreeOfParallelism ={ appSettingOptions.CouponTaskMaxDegreeOfParallelism}");
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 验证当前订单符合 条件的优惠券任务 --内部测试使用
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<GetPromotionTaskListByOrderIdResponse>> CheckSendCouponByOrderIdAsync([FromBody] CheckSendCouponByOrderIdRequest request)
        {
            GetPromotionTaskListByOrderIdRequest getPromotionTaskListByOrderIdRequest = new GetPromotionTaskListByOrderIdRequest()
            {
                OrderID = request.orderID,
                Budget = request.Budget,
                BusinessLineName =request.BusinessLineName,
                CouponRuleIDList= request.CouponRuleIDList,
            };
            GetPromotionTaskListRequest getPromotionTaskListRequest = new GetPromotionTaskListRequest()
            {
                TaskType = 2,
                TaskStatus = 1,
                TaskTime = DateTime.Now
            };
            getPromotionTaskListByOrderIdRequest.PromotionTaskList = (await GetPromotionTaskListAsync(getPromotionTaskListRequest).ConfigureAwait(false)).Result;

            var result = (await GetPromotionTaskListByOrderIdAsync(getPromotionTaskListByOrderIdRequest).ConfigureAwait(false)).Result;

            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 订单发券前优惠券任务验证  --内部测试使用 指定任务id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<GetPromotionTaskListByOrderIdResponse>> CheckSendCouponAsync([FromBody] CheckSendCouponRequest request)
        {
            GetPromotionTaskListRequest getPromotionTaskListRequest = new GetPromotionTaskListRequest()
            {
                TaskType = 2,
                TaskStatus = 1,
                TaskTime = DateTime.Now
            };                                                                                                                                                    
            var  allPromotionTaskList = (await GetPromotionTaskListAsync(getPromotionTaskListRequest).ConfigureAwait(false)).Result;
            GetPromotionTaskListByOrderIdRequest getPromotionTaskListByOrderIdRequest = new GetPromotionTaskListByOrderIdRequest()
            {
                OrderID = request.OrderID,
                //PromotionTaskList = allPromotionTaskList,
                PromotionTaskList = allPromotionTaskList.Where(p => request.PromotionTaskIdList.Contains(p.PromotionTaskId)).ToList(),
            };
            var result = await GetPromotionTaskListByOrderIdAsync(getPromotionTaskListByOrderIdRequest).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// 发券
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override  ValueTask<OperationResult<SendCouponResponse>> SendCouponAsync([FromBody] SendCouponRequest request)
        {
            SendCouponResponse result = new SendCouponResponse();
            #region 1.发送前验证 【根据订单号和任务号】
            var concurrency = await commonManager.PreventConcurrencyAsync($"{GlobalConstant.ConcurrencyPrefixForCouponTask}:{request.OrderID}/{request.Status}/{request.PromotionTaskId}", 1, TimeSpan.FromSeconds(20)).ConfigureAwait(false);
            if (!concurrency)
            {
                logger.Warn($@"SendCouponAsync failed OrderID={request.OrderID} 并发请求");
                return OperationResult.FromError<SendCouponResponse>("-1", "并发请求");
            }
            //获取用户信息
            var user = await userAccountService.GetUserByIdAsync(request.UserID, HttpContext.RequestAborted).ConfigureAwait(false);
            if (!user.Success || user.Result == null || String.IsNullOrWhiteSpace(user.Result.MobileNumber))
            {
                logger.Warn($@"SendCouponAsync failed OrderID={request.OrderID} GetUserByIdAsync error");
                return OperationResult.FromError<SendCouponResponse>("-2", "GetUserByIdAsync error");
            }

            //验证该订单是否发送过
            var sendHistory = await promotionSingleTaskUsersHistoryRepository.GetByPromotionTaskIdAndOrderIDAsync(request.PromotionTaskId, request.OrderID.ToString(), HttpContext.RequestAborted).ConfigureAwait(false);
            if (sendHistory != null && sendHistory.PromotionSingleTaskUsersHistoryId > 0 && !string.IsNullOrWhiteSpace(sendHistory.PromotionCodeIDs))
            {
                logger.Warn($@"SendCouponAsync failed OrderID={request.OrderID} 该订单已发送过优惠券");
                return OperationResult.FromError<SendCouponResponse>("-3", "该订单已发送过优惠券");
            }

            if (request.IsLimitOnce)//是否限制每人每个任务限制发送一次
            {
                //获取发送历史
                var historyID = await couponTaskManager.CheckSendHistoryAsync(request, user.Result, HttpContext.RequestAborted).ConfigureAwait(false);
                if (historyID > 0)
                {
                    logger.Warn($@"SendCouponAsync failed OrderID={request.OrderID} 每人每个任务限制发送一次");
                    return OperationResult.FromError<SendCouponResponse>("-3", "每人每个任务限制发送一次");
                }
            }
         
            #endregion

            #region 2.获取任务配置发券配置
            GetCouponRuleByTaskIDRequest getCouponRuleByTaskIDRequest = new GetCouponRuleByTaskIDRequest() { PromotionTaskId = request.PromotionTaskId };
            var couponRule = await couponTaskManager.GetCouponRuleByTaskIDAsync(getCouponRuleByTaskIDRequest, HttpContext.RequestAborted).ConfigureAwait(false);
            if (couponRule == null)
            {
                logger.Warn($@"SendCouponAsync failed OrderID={request.OrderID} 未获取任务配置发券配置");
                return OperationResult.FromError<SendCouponResponse>("-4", "未获取任务配置发券配置");
            }
            #endregion
            //发送优惠券
            result.PromotionCodeIDs = await couponTaskManager.SendCouponsLogicAsync(couponRule, user.Result, request,HttpContext.RequestAborted).ConfigureAwait(false);

            //发送成功推送【短信，模板消息】
            var pushResult = await couponTaskManager.SendNoyificationAsync(couponRule, user.Result, request, HttpContext.RequestAborted).ConfigureAwait(false);

            #region 数据埋点
            List<KeyValuePair<string, string>> sKeyValuePairs = new List<KeyValuePair<string, string>>();
            sKeyValuePairs.Add(new KeyValuePair<string, string>("PromotionTaskID", request.PromotionTaskId.ToString()));
            sKeyValuePairs.Add(new KeyValuePair<string, string>("Status", string.IsNullOrWhiteSpace(result.PromotionCodeIDs) ? "0" : "1"));
            await commonManager.MetricsCounterAsync("CouponTask", sKeyValuePairs).ConfigureAwait(false);
            #endregion

            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 获取发券规则
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<List<PromotionTaskPromotionListModel>>> GetCouponRuleListByTaskIDAsync([FromBody] GetCouponRuleByTaskIDRequest request)
        {
            var result = await couponTaskManager.GetCouponRuleByTaskIDAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        ///  关闭任务
        /// </summary>
        /// <param name="request"></param>.
        /// <returns></returns>
        public async override ValueTask<OperationResult<bool>> ClosePromotionTaskByPKIDAsync([FromBody] ClosePromotionTaskByPkidRequest request)
        {
            var result = await couponTaskManager.ClosePromotionTaskByPKIDAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
            return OperationResult.FromResult(result);
        }

      
    }
}
