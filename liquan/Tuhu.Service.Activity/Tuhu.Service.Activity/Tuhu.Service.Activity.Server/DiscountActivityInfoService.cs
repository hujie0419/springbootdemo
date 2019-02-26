using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Server.Manager;
using Tuhu.Service.Activity.Server.Manager.SalePromotion;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Product.Models.ProductConfig;

namespace Tuhu.Service.Activity.Server
{
    public class DiscountActivityInfoService : IDiscountActivityInfoService
    {
        /// <summary>
        /// 获取时间段内商品的活动信息列表
        /// </summary>
        /// <param name="pidList"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<ProductActivityInfoForTag>>> GetProductDiscountInfoForTagAsync(List<string> pidList, DateTime startTime, DateTime endTime)
        {
            if (!(pidList?.Count > 0))
            {
                return OperationResult.FromError<IEnumerable<ProductActivityInfoForTag>>("1", "请传入商品pid");
            }
            if (startTime == null || startTime == new DateTime())
            {
                return OperationResult.FromError<IEnumerable<ProductActivityInfoForTag>>("2", "请传入活动开始时间");
            }
            if (endTime == null || endTime == new DateTime())
            {
                return OperationResult.FromError<IEnumerable<ProductActivityInfoForTag>>("3", "请传入活动结束时间");
            }
            if (startTime >= endTime)
            {
                return OperationResult.FromError<IEnumerable<ProductActivityInfoForTag>>("4", "开始时间应小于结束时间");
            }
            pidList = pidList?.Distinct().ToList();
            return await DiscountActivityInfoManager.GetProductDiscountInfoForTagAsync(pidList, startTime, endTime);
        }

        /// <summary>
        /// 获取商品的打折活动信息和用户限购数
        /// </summary>
        /// <param name="pidList"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<ProductDiscountActivityInfo>>> GetProductAndUserDiscountInfoAsync(List<string> pidList, string userId)
        {
            if (!(pidList?.Count > 0))
            {
                return OperationResult.FromError<IEnumerable<ProductDiscountActivityInfo>>("1", "未传入参数商品Pid");
            }
            pidList = pidList.Distinct().ToList();
            return await DiscountActivityInfoManager.GetProductAndUserDiscountInfoAsync(pidList, userId);
        }
        /// <summary>
        /// 根据pid和数量返回打折活动命中信息
        /// </summary>
        /// <param name="requestList"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<ProductHitDiscountResponse>>> GetProductAndUserHitDiscountInfoAsync(List<DiscountActivityRequest> requestList, string userId)
        {
            if (requestList != null && requestList.Any())
            {
                var productHitDiscountManager = new ProductHitDiscountManager(requestList, userId);
                var result = await productHitDiscountManager.GetProductHitDiscountInfo();
                return OperationResult.FromResult(result);
            }
            else
            {
                return OperationResult.FromError<IEnumerable<ProductHitDiscountResponse>>(nameof(Resource.ParameterError),
                    $"requestList 为空");
            }
        }

        /// <summary>
        /// 创建订单时记录订单打折信息
        /// </summary>
        /// <param name="orderInfoList"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> SaveCreateOrderDiscountInfoAndSetCacheAsync(List<DiscountCreateOrderRequest> orderInfoList)
        {
            // 验证参数
            orderInfoList = orderInfoList?.Where(a =>
            !string.IsNullOrWhiteSpace(a.ActivityId) && !string.IsNullOrWhiteSpace(a.Pid)
            && !string.IsNullOrWhiteSpace(a.UserId) && a.Num > 0)?.ToList();
            if (!(orderInfoList?.Count > 0))
            {
                return OperationResult.FromError<bool>("1", "参数不正确");
            }
            return await DiscountActivityInfoManager.SaveCreateOrderDiscountInfoAndSetCache(orderInfoList);
        }

        /// <summary>
        /// 取消订单时 修改订单折扣记录  
        /// </summary>
        /// <param name="orderInfoList"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> UpdateCancelOrderDiscountInfoAndSetCacheAsync(int orderId)
        {
            if (!(orderId > 0))
            {
                return OperationResult.FromError<bool>("1", "未传入参数");
            }
            return await DiscountActivityInfoManager.UpdateCancelOrderDiscountInfoAndSetCache(orderId);
        }

        /// <summary>
        /// 批量获取用户活动已购数量缓存数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityIdList"></param>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<UserActivityBuyNumModel>>> GetOrSetUserActivityBuyNumCacheAsync(string userId, List<string> activityIdList)
        {
            if (string.IsNullOrEmpty(userId) || !(activityIdList?.Count > 0))
            {
                return OperationResult.FromError<IEnumerable<UserActivityBuyNumModel>>("1", "传入参数错误");
            }
            return OperationResult.FromResult(await DiscountActivityInfoManager.GetOrSetUserActivityBuyNumCache(userId, activityIdList));
        }

        /// <summary>
        /// 批量获取活动商品已售数量缓存数据
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<ActivityPidSoldNumModel>>> GetOrSetActivityProductSoldNumCacheAsync(string activityId, List<string> pidList)
        {
            if (string.IsNullOrEmpty(activityId) || !(pidList?.Count > 0))
            {
                return OperationResult.FromError<IEnumerable<ActivityPidSoldNumModel>>("1", "传入参数错误");
            }
            return OperationResult.FromResult(await DiscountActivityInfoManager.GetOrSetActivityProductSoldNumCache(activityId, pidList));
        }

    }
}
