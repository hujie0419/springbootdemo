using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Server.Manager;
using Tuhu.Service.Product;
using Tuhu.Service.Activity.Models.Requests;
using CreateOrderRequest = Tuhu.Service.Order.Request.CreateOrderRequest;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.MessageQueue;

namespace Tuhu.Service.Activity.Server
{
    public class FlashSaleService : IFlashSaleService
    {

        public static readonly RabbitMQProducer produceer;

        static FlashSaleService()
        {
            try
            {
                produceer = RabbitMQClient.DefaultClient.CreateProducer("topic.notificationExchage");
            }
            catch (Exception e)
            {

                produceer = null;
            }

        }

        public async Task<OperationResult<FlashSaleModel>> SelectFlashSaleDataByActivityIDAsync(Guid activityID)
                   => OperationResult.FromResult(await FlashSaleManager.SelectFlashSaleDataByActivityIDAsync(activityID));

        public async Task<OperationResult<bool>> UpdateFlashSaleDataToCouchBaseByActivityIDAsync(Guid activityID)
          => OperationResult.FromResult(await FlashSaleManager.UpdateFlashSaleCacheAsync(activityID));


        public Task<OperationResult<List<FlashSaleModel>>> GetFlashSaleListAsync(Guid[] activityIDs)
            => OperationResult.FromResultAsync(FlashSaleManager.GetFlashSaleListAsync(activityIDs));

        public async Task<OperationResult<IEnumerable<FlashSaleModel>>> SelectSecondKillTodayDataAsync(int activityType, DateTime? scheduleDate = null, bool needProducts = true, bool excludeProductTags = false)
        => OperationResult.FromResult(await FlashSaleManager.SelectSecondKillTodayDataAsync(activityType, scheduleDate, needProducts, excludeProductTags));
        public Task<OperationResult<int>> DeleteFlashSaleRecordsAsync(int orderId)
     => OperationResult.FromResultAsync<int>(DalFlashSale.DeleteFlashSaleRecordsAsync(orderId));

        public Task<OperationResult<IEnumerable<FlashSaleProductBuyLimitModel>>> CheckFlashSaleProductBuyLimitAsync(CheckFlashSaleProductRequest request)
       => OperationResult.FromResultAsync(FlashSaleManager.CheckFlashSaleProductBuyLimitAsync(request));

        public async Task<OperationResult<FlashSaleProductCanBuyCountModel>> GetUserCanBuyFlashSaleItemCountAsync(Guid userId, Guid activityId, string pid)
        {
            if (userId == Guid.Empty || activityId == Guid.Empty || string.IsNullOrWhiteSpace(pid))
                return OperationResult.FromError<FlashSaleProductCanBuyCountModel>("-1", "参数错误");
            else
                return OperationResult.FromResult(await FlashSaleManager.GetUserCanBuyFlashSaleItemCountAsync(userId, activityId, pid));
        }

        public async Task<OperationResult<FlashSaleOrderResponse>> CheckCanBuyFlashSaleOrderAsync(FlashSaleOrderRequest request)
        {
            if (request == null ||
                request.UserId == Guid.Empty ||
                string.IsNullOrWhiteSpace(request.UseTel) ||
                request.Products == null ||
                !request.Products.Any())
                return OperationResult.FromError<FlashSaleOrderResponse>("-1", "参数错误");
            else
                return OperationResult.FromResult(await FlashSaleManager.CheckCanBuyFlashSaleOrderAsync(request));
        }

        /// <summary>
        /// 查询限时抢购详情
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="variantId"></param>
        /// <param name="activityId"></param>
        /// <param name="channel"></param>
        /// <param name="userId"></param>
        /// <param name="productGroupId"></param>
        ///  <param name="btyQty"></param>
        /// <returns></returns>
        public async Task<OperationResult<FlashSaleProductDetailModel>> FetchProductDetailForFlashSaleAsync(string productId, string variantId, string activityId, string channel, string userId, string productGroupId = null, int btyQty = 1)
        {
            Guid result;
            if (!Guid.TryParse(activityId, out result))
            {
                return OperationResult.FromError<FlashSaleProductDetailModel>(ErrorCode.ParameterError, "参数无效");
            }
            return OperationResult.FromResult(await FlashSaleManager.FetchProductDetailForFlashSaleAsync(productId, variantId, activityId, channel, productGroupId, userId));
        }


        public async Task<OperationResult<CreateOrderResult>> FlashSaleCreateOrderAsync(CreateOrderRequest request, int type, FlashSaleOrderRequest flashrequest)
        {
            return OperationResult.FromResult(await FlashSaleCreateOrderManager.FlashSaleCreateOrder(request));
        }

        /// <summary>
        /// 减少计数器
        /// </summary>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        public Task<OperationResult<bool>> DecrementCounterAsync(int orderId)
            => OperationResult.FromResultAsync(FlashSaleManager.DecrementCounter(orderId));

        public Task<OperationResult<bool>> OrderCancerMaintenanceFlashSaleDataAsync(int orderId)

           => OperationResult.FromResultAsync<bool>(FlashSaleManager.OrderCancerMaintenanceFlashSaleData(orderId));

        public async Task<OperationResult<bool>> RefreshFlashSaleHashCountAsync(List<string> activtyids, bool isAllRefresh)
        {
            return OperationResult.FromResult<bool>(await FlashSaleManager.RefreshFlashSaleHashCountAsync(activtyids, isAllRefresh));
        }

        public async Task<OperationResult<List<FlashSaleModel>>> GetFlashSaleWithoutProductsListAsync(List<Guid> activityids)
        {
            return OperationResult.FromResult(await FlashSaleManager.GetFlashSaleWithoutProductsListAsync(activityids));
        }

        public async Task<OperationResult<FlashSaleProductDetailModel>> FetchProductDetailDisountInfoAsync(DiscountActivtyProductRequest request)
        {
            return OperationResult.FromResult(await Task.Run(() => new FlashSaleProductDetailModel()));
        }

        public async Task<OperationResult<OrderCountResponse>> GetUserCreateFlashOrderCountCacheAsync(OrderCountCacheRequest request)
        {
            return OperationResult.FromResult(await FlashSaleManager.GetUserCreateFlashOrderCountCacheAsync(request));
        }

        public async Task<OperationResult<OrderCountResponse>> SetUserCreateFlashOrderCountCacheAsync(OrderCountCacheRequest request)
        {
            return OperationResult.FromResult(await FlashSaleManager.SetUserCreateFlashOrderCountCacheAsync(request));
        }

        public async Task<OperationResult<List<FlashSaleWrongCacheResponse>>> SelectFlashSaleWrongCacheAsync()
        {
            return OperationResult.FromResult(await FlashSaleManager.SelectFlashSaleWrongCacheAsync());
        }

        public async Task<OperationResult<bool>> OrderCreateMaintenanceFlashSaleDbDataAsync(FlashSaleOrderRequest flashSale)
        {
            return OperationResult.FromResult(await FlashSaleManager.OrderCreateMaintenanceFlashSaleDbDataAsync(flashSale));
        }

        public async Task<OperationResult<bool>> UpdateConfigSaleoutQuantityFromLogAsync(UpdateConfigSaleoutQuantityRequest request)
        {
            return OperationResult.FromResult(await FlashSaleManager.UpdateConfigSaleoutQuantityFromLogAsync(request));
        }

        public async Task<OperationResult<UserReminderInfo>> GetUserReminderInfoAsync(EveryDaySeckillUserInfo model)
        {

            return await OperationResult.FromResultAsync(FlashSaleManager.GetUserReminderInfoAsync(model));
        }

        public async Task<OperationResult<InsertEveryDaySeckillUserInfoResponse>> InsertEveryDaySeckillUserInfoAsync(EveryDaySeckillUserInfo model)
        {

            return await OperationResult.FromResultAsync(FlashSaleManager.InsertEveryDaySeckillUserInfo(model));
        }

        public async Task<OperationResult<bool>> RefreshSeckillDefaultDataByScheduleAsync(string schedule)
        {
            return await OperationResult.FromResultAsync(FlashSaleManager.RefreshSeckillDefaultDataByScheduleAsync(schedule));
        }

        public async Task<OperationResult<Dictionary<string, List<SeckilScheduleInfoRespnose>>>> GetSeckillScheduleInfoAsync(List<string> pids, DateTime sSchedule, DateTime eSchedule)
        {
            return await OperationResult.FromResultAsync(FlashSaleManager.GetSeckillScheduleInfoAsync(pids, sSchedule, eSchedule));
        }

        public async Task<OperationResult<List<SeckillAvailableStockInfoResponse>>> GetSeckillAvailableStockResponseAsync(List<SeckillAvailableStockInfoRequest> request)
        {
            return await OperationResult.FromResultAsync(FlashSaleManager.GetSeckillAvailableStockResponseAsync(request));
        }

        /// <inheritdoc />
        /// <summary>
        /// 查询首页天天秒杀数据
        /// </summary>
        /// <param name="request">查询个数</param>
        /// <returns></returns>
        public async Task<OperationResult<List<FlashSaleModel>>> SelectHomeSeckillDataAsync(SelectHomeSecKillRequest request)
        {
            return await OperationResult.FromResultAsync(FlashSaleManager.SelectHomeSeckillDataWithMemoryAsync(request));
        }

        /// <inheritdoc />
        /// <summary>
        /// 根据场次Id获取天天秒杀产品数据
        /// </summary>
        /// <param name="request">场次活动Id</param>
        /// <returns></returns>
        public async Task<OperationResult<List<FlashSaleProductModel>>> SelectSeckillDataByActivityIdAsync(SelectSecKillByIdRequest request)
        {
            return await OperationResult.FromResultAsync(FlashSaleManager.SelectSeckillDataByActivityIdAsync(request));
        }

        /// <summary>
        /// 根据时间范围查询秒杀场次信息
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<FlashSaleModel>>> GetSecondsBoysAsync(int activityType, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (startDate == null || endDate == null)
                return OperationResult.FromError<IEnumerable<FlashSaleModel>>("-1", "参数错误");
            else
                return OperationResult.FromResult(await FlashSaleManager.GetSecondsBoysAsync(activityType, startDate, endDate));
        }

        /// <summary>
        /// 根据活动ID,与PID集合查询对应的活动信息与产品信息
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<FlashSaleModel>>> SelectFlashSaleDataByActivityIdsAsync(List<FlashSaleDataByActivityRequest> flashSaleDataByActivityRequests)
        {
            if (flashSaleDataByActivityRequests.Count > 20)
            {
                return OperationResult.FromError<List<FlashSaleModel>>(ErrorCode.ParameterError, "最多支持20个活动ID集合查询");
            }
            //return OperationResult.FromResult(await FlashSaleManager.SelectFlashSaleDataByActivityIdsAsync(flashSaleDataByActivityRequests));
            return OperationResult.FromResult(await FlashSaleManager.GetFlashSaleActivityAndProductInfoAsync(flashSaleDataByActivityRequests));
        }

        /// <summary>
        /// 天天秒杀数据列表缓存刷新
        /// </summary>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> SpikeListRefreshAsync(Guid activityId)
        {
            if (activityId == null)
                return OperationResult.FromError<bool>("-1", "活动ID不可为空");
            else
                return OperationResult.FromResult(await FlashSaleManager.SpikeListRefreshAsync(activityId));
        }

        /// <summary>
        /// 活动页秒杀查询最新有效场次
        /// </summary>
        /// <param name="topNumber">场次返回条数</param>
        /// <param name="isProducts">是否查询活动下的产品信息，默认为true</param>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<FlashSaleModel>>> GetActivePageSecondKillAsync(int topNumber, bool isProducts = true)
        {
            if (topNumber <= 0)
                return OperationResult.FromError<IEnumerable<FlashSaleModel>>("-1", "topNumber必须大于0");
            else
                return OperationResult.FromResult(await FlashSaleManager.GetActivePageSecondKillManager(topNumber, isProducts));
        }


        /// <summary>
        /// 新活动页查询活动信息接口
        /// </summary>
        /// <param name="flashSaleActivityPageRequest"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<FlashSaleActivityPageModel>>> GetFlashSaleDataActivityPageByIdsAsync(List<FlashSaleActivityPageRequest> flashSaleActivityPageRequest)
        {
            if (flashSaleActivityPageRequest.Count > 20)
            {
                return OperationResult.FromError<List<FlashSaleActivityPageModel>>(ErrorCode.ParameterError, "最多支持20个活动ID集合查询");
            }

            return OperationResult.FromResult(await FlashSaleManager.GetFlashSaleDataActivityPageByIdsAsync(flashSaleActivityPageRequest));
        }
    }
}
