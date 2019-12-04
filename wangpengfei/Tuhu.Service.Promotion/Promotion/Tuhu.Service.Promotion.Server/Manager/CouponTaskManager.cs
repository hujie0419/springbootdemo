using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.C.Utility.Components;
using Tuhu.Nosql;
using Tuhu.Service.CreatePromotion.Models;
using Tuhu.Service.Order.Models;
using Tuhu.Service.Product.ModelDefinition.ProductModels;
using Tuhu.Service.Promotion.DataAccess.Entity;
using Tuhu.Service.Promotion.DataAccess.IRepository;
using Tuhu.Service.Promotion.Request;
using Tuhu.Service.Promotion.Response;
using Tuhu.Service.Promotion.Server.Config;
using Tuhu.Service.Promotion.Server.Manager.IManager;
using Tuhu.Service.Promotion.Server.Model;
using Tuhu.Service.Promotion.Server.ServiceProxy;
using Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy;
using Tuhu.Service.Promotion.Server.Utility;
using Tuhu.Service.Push.Models.Push;
using Tuhu.Service.Shop.Models;
using Tuhu.Service.UserAccount.Models;
using Tuhu.Service.Utility.Request;


namespace Tuhu.Service.Promotion.Server.Manager
{
    /// <summary>
    /// 优惠券任务
    /// </summary>
    public class CouponTaskManager : ICouponTaskManager
    {
        private readonly ILogger logger;
        private readonly IPromotionTaskRepository promotionTaskRepository;
        private readonly IPromotionTaskPromotionListRepository promotionTaskPromotionListRepository;
        private readonly IPromotionSingleTaskUsersHistoryRepository promotionSingleTaskUsersHistoryRepository;
        private readonly IChannelDictionariesRepository channelDictionariesRepository;
        private readonly ICacheHelper cacheHelper;
        private readonly ICreatePromotionService createPromotionService;
        private readonly IPushService pushService;
        private readonly ISmsService smsService;
        private readonly IPromotionTaskProductCategoryRepository promotionTaskProductCategoryRepository;
        private readonly IPromotionTaskBudgetRepository promotionTaskBudgetRepository;
        private readonly TuhuMemoryCacheNoJson tuhuMemoryCache;




        public CouponTaskManager(
               ILogger<CouponTaskManager> Logger,
               ICacheHelper ICacheHelper,
               ICreatePromotionService ICreatePromotionService,
               IPromotionTaskRepository IPromotionTaskRepository,
               IPromotionSingleTaskUsersHistoryRepository IPromotionSingleTaskUsersHistoryRepository,
               IChannelDictionariesRepository IChannelDictionariesRepository,
               ISmsService ISmsService,
               IPushService IPushService,
               IPromotionTaskPromotionListRepository IPromotionTaskPromotionListRepository,
               TuhuMemoryCacheNoJson TuhuMemoryCacheNoJson,
               IPromotionTaskBudgetRepository IPromotionTaskBudgetRepository,
               IPromotionTaskProductCategoryRepository IPromotionTaskProductCategoryRepository
            )
        {
            logger = Logger;
            cacheHelper = ICacheHelper;
            promotionTaskRepository = IPromotionTaskRepository;
            promotionSingleTaskUsersHistoryRepository = IPromotionSingleTaskUsersHistoryRepository;
            promotionTaskPromotionListRepository = IPromotionTaskPromotionListRepository;
            channelDictionariesRepository = IChannelDictionariesRepository;
            pushService = IPushService;
            smsService = ISmsService;
            createPromotionService = ICreatePromotionService;
            tuhuMemoryCache = TuhuMemoryCacheNoJson;
            promotionTaskBudgetRepository = IPromotionTaskBudgetRepository;
            promotionTaskProductCategoryRepository = IPromotionTaskProductCategoryRepository;

        }

        #region 任务列表
        /// <summary>
        /// 获取优惠券 任务列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<PromotionTaskModel>> GetPromotionTaskListAsync(GetPromotionTaskListRequest request, CancellationToken cancellationToken)
        {
            var entities = await promotionTaskRepository.GetPromotionTaskListAsync(request, cancellationToken).ConfigureAwait(false);
            var result = ObjectMapper.ConvertTo<PromotionTaskEntity, PromotionTaskModel>(entities).ToList();
            return result;
        }

        public async ValueTask<List<PromotionTaskModel>> GetPromotionTaskListCacheAsync(GetPromotionTaskListRequest request, CancellationToken cancellationToken)
        {
            List<PromotionTaskModel> data = new List<PromotionTaskModel>();
            using (var client = cacheHelper.CreateCacheClient(GlobalConstant.RedisClient))
            {
                var result = await client.GetOrSetAsync(
                    GlobalConstant.RedisKeyForPromotionTaskList,
                    async () => (await GetPromotionTaskListAsync(request, cancellationToken).ConfigureAwait(false)),
                    GlobalConstant.RedisTTLForPromotionTaskList).ConfigureAwait(false);
                if (result.Success)
                {
                    data = result.Value;
                }
                else
                {
                    data = await GetPromotionTaskListAsync(request, cancellationToken).ConfigureAwait(false);
                }
            }
            return data;
        }


        /// <summary>
        /// 获取优惠券 任务列表 - 缓存
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<PromotionTaskModel>> GetPromotionTaskListMemoryCacheAsync(GetPromotionTaskListRequest request, CancellationToken cancellationToken)
        {
            var result = await tuhuMemoryCache.GetOrSetWithLockAsync(GlobalConstant.RedisKeyForPromotionTaskList,
                async () => await GetPromotionTaskListCacheAsync(request, cancellationToken).ConfigureAwait(false), TimeSpan.FromMinutes(5)).ConfigureAwait(false);
            return result;
        }



        #endregion

        /// <summary>
        ///  发券
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public ValueTask<SendCouponResponse> SendCouponAsync(SendCouponRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据任务id获取 发券规则
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<PromotionTaskPromotionListModel>> GetCouponRuleByTaskIDAsync(GetCouponRuleByTaskIDRequest request, CancellationToken cancellationToken)
        {
            var entity = await promotionTaskPromotionListRepository.GetPromotionTaskPromotionListByPromotionTaskIdAsync(request.PromotionTaskId, cancellationToken).ConfigureAwait(false);
            var result = ObjectMapper.ConvertTo<PromotionTaskPromotionListEntity, PromotionTaskPromotionListModel>(entity);
            return result.ToList();
        }

        /// <summary>
        /// 关闭任务
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> ClosePromotionTaskByPKIDAsync(ClosePromotionTaskByPkidRequest request, CancellationToken cancellationToken)
        {
            var result = await promotionTaskRepository.ClosePromotionTaskByPKIDAsync(request, cancellationToken).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        ///  验证 发送历史
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<int> CheckSendHistoryAsync(SendCouponRequest request, User user, CancellationToken cancellationToken)
        {
            var entity = await promotionSingleTaskUsersHistoryRepository.GetByPromotionTaskIdAndPhoneAsync(request.PromotionTaskId, user.MobileNumber, cancellationToken).ConfigureAwait(false);
            if (entity != null && entity.PromotionSingleTaskUsersHistoryId > 0 && !string.IsNullOrWhiteSpace(entity.PromotionCodeIDs))
            {
                return entity.PromotionSingleTaskUsersHistoryId;
            }
            else
            {
                return 0;
            }
           
        }

        /// 验证单个订单和优惠券任务是否匹配，并更新不匹配原因
        /// </summary>
        /// <param name="couponTask"></param>
        /// <param name="order"></param>
        /// <param name="productList"></param>
        /// <param name="shop"></param>
        /// <returns></returns>
        public async Task<bool> CheckPromotionTaskWithOrderAsync(GetPromotionTaskListByOrderIdRequest request, PromotionTaskModel couponTask, OrderModel order, List<ProductBaseInfo> productList, ShopModel shop, List<ChannelDictionariesModel> orderChannelList, CancellationToken cancellationToken)
        {
            logger.Info($"CheckPromotionTaskWithOrderAsync order.PKID={order.PKID} PromotionTaskId={couponTask.PromotionTaskId} productList.Count={productList.Count} OrderListModel.Count={order.OrderListModel.Count()}");
            var products = new List<ProductBaseInfo>(productList);
            #region 订单 order
            //筛选订单时间
            if (couponTask.FilterEndTime.HasValue && couponTask.FilterStartTime.HasValue && (couponTask.FilterStartTime > order.OrderDatetime || couponTask.FilterEndTime < order.OrderDatetime))
            {
                couponTask.IsMatch = false;
                couponTask.NotMatchMessage = $"订单时间不匹配 OrderDatetime ={order.OrderDatetime}";
                return false;
            }

            //订单状态 
            if (!request.Budget && !string.IsNullOrWhiteSpace(couponTask.OrderStatus))
            {
                if (couponTask.OrderStatus == "Paid" && order.PayStatus != "2Paid")
                {
                    couponTask.IsMatch = false;
                    couponTask.NotMatchMessage = $"订单状态不符合 PayStatus ={order.PayStatus}";
                    return false;
                }
                if (couponTask.OrderStatus == "Installed" && order.InstallStatus != "2Installed")
                {
                    couponTask.IsMatch = false;
                    couponTask.NotMatchMessage = $"订单状态不符合 InstallStatus ={order.InstallStatus}";
                    return false;
                }
                if (couponTask.OrderStatus == "OrderCompleted")
                {
                    var match = (order.InstallShopId > 0 && order.InstallStatus == "2Installed")
                                || (
                                    ((order.InstallShopId ?? 0) == 0)
                                    && order.Status == "2Shipped"
                                    && (order.DeliveryStatus == "3Received" || order.DeliveryStatus == "3.5Signed")
                                  );
                    if (!match)
                    {
                        couponTask.IsMatch = false;
                        couponTask.NotMatchMessage = $"订单状态不符合 OrderStatus ={order.InstallStatus}，{order.Status}，{order.DeliveryStatus}";
                        return false;
                    }
                }
            }

            //筛选渠道-OrderChannel
            var taskChannelList = couponTask.Channel?.Split(new char[] { ',', '，' })?.ToList();
            if (taskChannelList != null && taskChannelList.Any())
            {
                var taskOrderChannelList = orderChannelList.Where(p => taskChannelList.Contains(p.ChannelType) || taskChannelList.Contains(p.ChannelKey)).ToList();

                if (taskOrderChannelList.Where(p => p.ChannelKey == order.OrderChannel)?.Count() == 0)
                {
                    couponTask.IsMatch = false;
                    couponTask.NotMatchMessage = $"订单渠道不匹配 OrderChannel ={order.OrderChannel}";
                    return false;
                }
            }

            //安装状态-InstallType
            if (!request.Budget && couponTask.InstallType > 0)
            {
                var installTypeList = couponTask.InstallType == 1 ? new List<string>() { "1ShopInstall" } : new List<string>() { "3NoInstall", "4SentInstall" };
                if (!installTypeList.Contains(order.InstallType))
                {
                    couponTask.IsMatch = false;
                    couponTask.NotMatchMessage = $"订单安装状态不匹配 InstallType ={order.InstallType}";
                    return false;
                }
            }

            //地域
            if (!string.IsNullOrWhiteSpace(couponTask.Area))
            {
                var areaList = couponTask.Area?.Split(new char[] { ',', '，' })?.ToList();
                if (shop == null || !areaList.Contains(shop.CityId.ToString()))
                {
                    couponTask.IsMatch = false;
                    couponTask.NotMatchMessage = $"门店地域不符合 CityId = {shop?.CityId}";
                    return false;
                }
            }

            #endregion

            #region 子订单 orderList 产品相关=> 最后计算总的件数和金额
            //PID
            if (!string.IsNullOrWhiteSpace(couponTask.Pid))
            {
                var pidList = couponTask.Pid?.Split(new char[] { ',', '，' })?.ToList();
                products = products.Where(p => pidList.Contains(p.Pid)).ToList();
                if (!products.Any())
                {
                    couponTask.IsMatch = false;
                    couponTask.NotMatchMessage = $"Pid不符合 ={JsonConvert.SerializeObject(productList.Select(p => p.Pid))} ";
                    return false;
                }
            }

            //品牌 【brand】
            if (!string.IsNullOrWhiteSpace(couponTask.Brand))
            {
                var brandList = couponTask.Brand?.Split(new char[] { ',', '，' })?.ToList();
                products = products.Where(p => brandList.Contains(p.Brand)).ToList();
                if (!products.Any())
                {
                    couponTask.IsMatch = false;
                    couponTask.NotMatchMessage = $"产品品牌不符合 Brand= {JsonConvert.SerializeObject(productList.Select(p => p.Brand))}";
                    return false;
                }
            }

            //产品类别  【category】
            var categoryList = await GetPromotionTaskCategoryMemoryCacheAsync(couponTask.PromotionTaskId, cancellationToken).ConfigureAwait(false);
            if (categoryList != null && categoryList.Any())
            {
                couponTask.Category = string.Join(",", categoryList);
                //couponTask.Category += (couponTask.Category.IndexOf('.') > 0 ? "" : ".");
                products = products.Where(p => !string.IsNullOrWhiteSpace(p.NodeNo)).Where(p => p.NodeNo.Split('.').ToList().Intersect(categoryList).Count() > 0).ToList();
                if (!products.Any())
                {
                    couponTask.IsMatch = false;
                    couponTask.NotMatchMessage = $"产品分类不符合 Category = {JsonConvert.SerializeObject(productList.Select(p => p.NodeNo))} ";
                    return false;
                }
            }

            //订单类型 【套装】
            if (couponTask.ProductType == 2)  //套装
            {
                products = products.Where(p => p.IsPackageProduct == true).ToList();
                if (!products.Any())
                {
                    couponTask.IsMatch = false;
                    couponTask.NotMatchMessage = $"产品套装不符合 IsPackageProduct =  {JsonConvert.SerializeObject(productList.Select(p => p.IsPackageProduct))}";
                    return false;
                }
            }
            if (couponTask.ProductType == 3)  //非套装
            {
                products = products.Where(p => p.IsPackageProduct == null || p.IsPackageProduct == false).ToList();
                if (!products.Any())
                {
                    couponTask.IsMatch = false;
                    couponTask.NotMatchMessage = $"产品套装不符合 IsPackageProduct =  {JsonConvert.SerializeObject(productList.Select(p => p.IsPackageProduct))}";
                    return false;
                }
            }

            List<OrderListModel> OrderListModel = order.OrderListModel.Where(o => products.Select(p => p.Pid).Contains(o.Pid)).ToList();
            //最后判断 购买件数-PurchaseNum
            if (couponTask.PurchaseNum > 0)
            {
                int totalNum = OrderListModel.Sum(p => p.Num);
                var flag = (couponTask.PurchaseNum < 1000 && totalNum == couponTask.PurchaseNum)//==指定数量
                    || (couponTask.PurchaseNum >= 1000 && couponTask.PurchaseNum < 2000 && totalNum <= couponTask.PurchaseNum - 1000) //<=0 指定数量
                    || (couponTask.PurchaseNum >= 2000 && couponTask.PurchaseNum < 3000 && totalNum >= couponTask.PurchaseNum - 2000) //>=0 指定数量
                    ;
                if (!flag)
                {
                    couponTask.IsMatch = false;
                    couponTask.NotMatchMessage = $"购买件数不匹配 PurchaseNum ={ totalNum }";
                    return false;
                }
            }
            decimal totalMoney = OrderListModel.Sum(p => p.TotalPrice);
            //最后判断 购买金额-SumMoney
            if (couponTask.SpendMoney > 0 && totalMoney < couponTask.SpendMoney)
            {
                couponTask.IsMatch = false;
                couponTask.NotMatchMessage = $"订单金额不匹配 SumMoney ={totalMoney}";
                return false;
            }
            #endregion

            couponTask.IsMatch = true;
            couponTask.NotMatchMessage = "";
            //成功，插入预算
            if (request.Budget)
            {
                await InsertPromotionTaskBudgetAsync(request, couponTask, order, products, cancellationToken).ConfigureAwait(false);
            }


            return true;
        }

        /// <summary>
        /// 插入一条预算
        /// </summary>
        /// <param name="couponTask"></param>
        /// <param name="order"></param>
        /// <param name="productList"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask InsertPromotionTaskBudgetAsync(GetPromotionTaskListByOrderIdRequest request, PromotionTaskModel couponTask, OrderModel order, List<ProductBaseInfo> productList, CancellationToken cancellationToken)
        {
            try
            {
                //根据任务 和 使用规则 查询 发券规则
                var promotionTaskGetRuleList = await promotionTaskPromotionListRepository.GetPromotionTaskPromotionListByPromotionTaskIdAsync(couponTask.PromotionTaskId, cancellationToken).ConfigureAwait(false);
                if (request.CouponRuleIDList != null && request.CouponRuleIDList.Any())
                {
                    promotionTaskGetRuleList = promotionTaskGetRuleList.Where(p => request.CouponRuleIDList.Contains(p.CouponRulesId)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(request.BusinessLineName))
                {
                    promotionTaskGetRuleList = promotionTaskGetRuleList.Where(p => p.BusinessLineName == request.BusinessLineName.Trim()).ToList();
                }

                logger.Info($@"InsertPromotionTaskBudgetAsync check request.OrderID={request.OrderID}, couponTask.PromotionTaskId ={couponTask.PromotionTaskId},promotionTaskGetRuleList.Count {promotionTaskGetRuleList.Count} ");
                foreach (var promotionTaskGetRule in promotionTaskGetRuleList)
                {
                    PromotionTaskBudgetEntity entity = new PromotionTaskBudgetEntity()
                    {
                        PromotionTaskID = couponTask.PromotionTaskId,
                        CouponRulesId = promotionTaskGetRule.CouponRulesId,
                        BusinessLineName = promotionTaskGetRule.BusinessLineName,
                        OrderID = order.OrderId,
                        PIDs = string.Join(",", productList.Select(p => p.Pid)),

                        DiscountMoney = promotionTaskGetRule.DiscountMoney,
                        CouponNum = promotionTaskGetRule.Number,
                    };
                    await promotionTaskBudgetRepository.CreateAsync(entity, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($@"InsertPromotionTaskBudgetAsync  exception request.OrderID={request.OrderID}, couponTask.PromotionTaskId ={couponTask.PromotionTaskId} Message = {ex.Message}", ex);
            }
        }


        /// <summary>
        ///  批量发送优惠券并更新统计次数  ,记录已发日志
        /// </summary>
        /// <param name="couponRuleList"></param>
        /// <param name="user"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<string> SendCouponsLogicAsync(List<PromotionTaskPromotionListModel> couponRuleList, User user, SendCouponRequest request, CancellationToken cancellationToken)
        {
            var result = new CreatePromotionCodeResult();
            PromotionSingleTaskUsersHistoryEntity entity = new PromotionSingleTaskUsersHistoryEntity()
            {
                PromotionTaskId = request.PromotionTaskId,
                UserCellPhone = user.MobileNumber,
                SendState = 0,
                OrderNo = request.OrderID.ToString(),
                UserID = user.UserId
            };
            var createPromotionList = new List<CreatePromotionModel>();
            try
            {
                #region 3.创建优惠券 [批量]
                foreach (var couponRule in couponRuleList)
                {
                    for (int i = 0; i < couponRule.Number; i++)
                    {
                        CreatePromotionModel createPromotionModel = new CreatePromotionModel()
                        {
                            Author = couponRule.Creater,
                            MinMoney = couponRule.MinMoney,
                            Discount = couponRule.DiscountMoney,
                            DepartmentName = couponRule.DepartmentName,
                            IntentionName = couponRule.IntentionName,
                            BusinessLineName = couponRule.BusinessLineName,
                            Description = couponRule.PromotionDescription,
                            UserID = user.UserId,
                            Channel = "PromotionTask",
                            StartTime = couponRule.StartTime,
                            EndTime = couponRule.EndTime,
                            RuleId = couponRule.CouponRulesId,
                            GetRuleID = couponRule.GetCouponRuleID,
                            Creater = couponRule.Creater,
                            IssueChannleId = request.PromotionTaskId.ToString(),
                            IssueChannle = "优惠券任务塞券",
                            Issuer = couponRule.Issuer,
                            //Type = couponRule.,//创建时会自动添加
                            TaskPromotionListId = couponRule.TaskPromotionListId,
                            BathID = request.OrderID,
                        };
                        createPromotionList.Add(createPromotionModel);
                    }
                }
                logger.Info($@"CouponTaskManager CreatePromotionsForYeWuAsync createPromotionList={JsonConvert.SerializeObject(createPromotionList)}");
                var createPromotionResult = await createPromotionService.CreatePromotionsForYeWuAsync(createPromotionList).ConfigureAwait(false);
                result = createPromotionResult?.Result;
                entity.IsSuccess = result.IsSuccess ? 1 : 2;
                entity.Message = result.ErrorMessage;
                entity.PromotionCodeIDs = result.promotionIds == null ? "" : string.Join(",", result.promotionIds);
                #endregion
            }
            catch (Exception ex)
            {
                logger.LogWarning("CouponTaskManager SendCouponsLogicAsync Exception", ex);
                entity.IsSuccess = 2;
                entity.Message = result.ErrorMessage;
                entity.PromotionCodeIDs = result.promotionIds == null ? "" : string.Join(",", result.promotionIds);
                return entity.PromotionCodeIDs;
            }
            finally
            {
                await promotionSingleTaskUsersHistoryRepository.CreateAsync(entity, cancellationToken).ConfigureAwait(false);
                int successCount = result?.promotionIds?.Count() ?? 0;
                //记录到已发 日志表
                //增加塞券次数
                await promotionTaskRepository.UpdatePromotionTaskCountAsync(request.PromotionTaskId, successCount > 0 ? 1 : 0, (createPromotionList.Count() - successCount) > 0 ? 1 : 0, cancellationToken).ConfigureAwait(false);
            }
            return entity.PromotionCodeIDs;
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="couponRuleList"></param>
        /// <param name="user"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> SendNoyificationAsync(List<PromotionTaskPromotionListModel> couponRuleList, User user, SendCouponRequest request, CancellationToken cancellationToken)
        {
            try
            {
                List<string> smsParam = JsonConvert.DeserializeObject<List<string>>(request.SmsParam ?? "");
                string msgUrl = string.Empty;       //追踪短信链接
                string msgBatchs = string.Empty;    //追踪短信批次
                string msgSubject = string.Empty;   //追踪短信主题

                if (request.SmsId > 0 && smsParam.Any())//判断是否发送短信
                {
                    if (smsParam.FirstOrDefault().ToLower().Contains("tuhu.cn"))//发送追踪短信
                    {
                        msgUrl = smsParam[0];
                        smsParam.RemoveAt(0);
                        msgBatchs = $"CouponTask_{request.PromotionTaskId}_{DateTime.Now.ToString("yyyyMMdd")}";
                        msgSubject = $"优惠券任务：{request.PromotionTaskId}";

                        var biSmsRequest = new BiSmsRequest()
                        {
                            PhoneNumber = user.MobileNumber,
                            MsgBatchs = msgBatchs,
                            MsgSubject = msgSubject,
                            MsgBody = smsParam.ToArray(),
                            MsgUrl = msgUrl,
                            RelatedUser = request.Creater,
                            TemplateId = request.SmsId
                        };
                        await smsService.SendBiSmsAsync(biSmsRequest, cancellationToken).ConfigureAwait(false);
                    }
                    else //发送普通短信
                    {
                        var smsRequest = new SendTemplateSmsRequest()
                        {
                            Cellphone = user.MobileNumber,
                            TemplateId = request.SmsId,
                            TemplateArguments = smsParam.ToArray()
                        };
                        await smsService.SendSmsAsync(smsRequest).ConfigureAwait(false);
                    }
                }

                foreach (var couponRule in couponRuleList.Where(p => p.IsRemind == "1"))
                {
                    PushTemplateLog pushTemplateLog = new PushTemplateLog()
                    {
                        Replacement = JsonConvert.SerializeObject(
                                            new Dictionary<string, string>
                                            {
                                                ["{{currenttime}}"] = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm"),
                                                ["{{couponamount}}"] = couponRule.DiscountMoney.ToString(CultureInfo.InvariantCulture),
                                                ["{{couponname}}"] = couponRule.PromotionDescription
                                            })
                    };
                    await pushService.PushByUserIDAndBatchIDAsync(new List<string> { user.MobileNumber }, 1509, pushTemplateLog).ConfigureAwait(false);
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogWarning("SendNoyificationAsync Exception", ex);
            }
            return false;
        }

        #region 订单渠道

        /// <summary>
        /// 获取所有订单渠道 - 缓存
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<List<ChannelDictionariesModel>> GetAllOrderChannelAsync(CancellationToken cancellationToken)
        {
            List<ChannelDictionariesEntity> entities = await channelDictionariesRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
            var models = ConvertToChannelDictionariesModel(entities);
            return models;
        }

        /// <summary>
        /// 转换成 model
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        private List<ChannelDictionariesModel> ConvertToChannelDictionariesModel(List<ChannelDictionariesEntity> entities)
        {
            List<ChannelDictionariesModel> models = new List<ChannelDictionariesModel>(entities.Count);
            foreach (var item in entities)
            {
                ChannelDictionariesModel model = new ChannelDictionariesModel()
                {
                    PKID = item.PKID,
                    ChannelType = item.ChannelType,
                    ChannelKey = item.ChannelKey,
                    ChannelValue = item.ChannelValue
                };
                models.Add(model);
            }
            return models;
        }

        public async Task<List<ChannelDictionariesModel>> GetAllOrderChannelMemoryCacheAsync(CancellationToken cancellationToken)
        {
            var result = await tuhuMemoryCache.GetOrSetWithLockAsync(GlobalConstant.RedisKeyForOrderChannel,
                async () => await GetAllOrderChannelCacheAsync(cancellationToken).ConfigureAwait(false), TimeSpan.FromMinutes(30)).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// 获取所有订单渠道 - 缓存
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<ChannelDictionariesModel>> GetAllOrderChannelCacheAsync(CancellationToken cancellationToken)
        {
            List<ChannelDictionariesModel> result = new List<ChannelDictionariesModel>();

            using (var client = cacheHelper.CreateCacheClient(GlobalConstant.RedisClient))
            {
                var cacheResult = await client.GetOrSetAsync(GlobalConstant.RedisKeyForOrderChannel,
                                                           async () => await GetAllOrderChannelAsync(cancellationToken).ConfigureAwait(false),
                                                           GlobalConstant.RedisTTLForOrderChannel).ConfigureAwait(false);
                if (cacheResult.Success)
                {
                    result = cacheResult.Value;
                }
                else
                {
                    result = await GetAllOrderChannelAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            return result;
        }

        #endregion

        #region 优惠券任务配置的类目

        public async ValueTask<List<string>> GetPromotionTaskCategoryMemoryCacheAsync(int promotionTaskId, CancellationToken cancellationToken)
        {
            var result = await tuhuMemoryCache.GetOrSetWithLockAsync($"{ GlobalConstant.RedisKeyForPromotionTaskCategory }:{promotionTaskId}",
                async () => await GetPromotionTaskCategoryCacheAsync(promotionTaskId, cancellationToken).ConfigureAwait(false), TimeSpan.FromMinutes(10)).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// 获取优惠券任务对应的 类目
        /// </summary>
        /// <param name="promotionTaskId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<string>> GetPromotionTaskCategoryCacheAsync(int promotionTaskId, CancellationToken cancellationToken)
        {
            List<PromotionTaskProductCategoryEntity> entities = new List<PromotionTaskProductCategoryEntity>();
            try
            {
                using (var client = cacheHelper.CreateCacheClient(GlobalConstant.RedisClient))
                {
                    var result = await client.GetOrSetAsync($"{ GlobalConstant.RedisKeyForPromotionTaskCategory }:{promotionTaskId}",
                                                               async () => await promotionTaskProductCategoryRepository.GetByPromotionTaskIdsync(promotionTaskId, cancellationToken).ConfigureAwait(false),
                                                               GlobalConstant.RedisTTLForPromotionTaskCategory).ConfigureAwait(false);
                    if (result.Success)
                    {
                        entities = result.Value;
                    }
                    else
                    {
                        entities = await promotionTaskProductCategoryRepository.GetByPromotionTaskIdsync(promotionTaskId, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("GetPromotionTaskCategoryCacheAsync Exception", ex);
            }
            return entities.Select(p => p.ProductCategoryId.ToString()).ToList();
        }

        #endregion 

    }
}
