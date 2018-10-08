
using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Server.Manager;
using Tuhu.Service.Product;
//using static Tuhu.Service.Activity.Models.GroupBuyingModel;

namespace Tuhu.Service.Activity.Server
{
    public class ShareBargainService : IShareBargainService
    {
        public static readonly ILog Logger = LogManager.GetLogger(typeof(BargainManager));

        /// <summary>
        /// 获取砍价商品列表（已废弃）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<BargainProductModel>>> GetBargainPaoductListAsync(
            GetBargainproductListRequest request)
        {
            if (request.PageIndex < 1 || request.PageSize < 1)
            {
                return OperationResult.FromError<PagedModel<BargainProductModel>>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await BargainManager.GetBargainProductList(request);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 获取用户该活动商品下的砍价记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityProductId"></param>
        /// <returns></returns>
        public async Task<OperationResult<BargainProductHistory>> FetchBargainProductHistoryAsync(Guid userId,
            int activityProductId, string pid)
        {
            if (userId == Guid.Empty || activityProductId < 0)
            {
                return OperationResult.FromError<BargainProductHistory>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await BargainManager.FetchBargainProductHistory(userId, activityProductId, pid);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 受邀人进行一次砍价
        /// </summary>
        /// <param name="idKey"></param>
        /// <param name="userId"></param>
        /// <param name="activityproductId"></param>
        /// <returns></returns>
        public async Task<OperationResult<BargainResult>> AddBargainActionAsync(Guid idKey, Guid userId,
            int activityProductId)
        {
            if (idKey == Guid.Empty || userId == Guid.Empty || activityProductId < 0)
            {
                return OperationResult.FromError<BargainResult>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await BargainManager.AddBargainAction(idKey, userId, activityProductId);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 检查此商品是否可购买
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="apId"></param>
        /// <returns></returns>
        public async Task<OperationResult<ShareBargainBaseResult>> CheckBargainProductStatusAsync(Guid ownerId,
            int apId, string pid, string deviceId = default(string))
        {
            if (ownerId == Guid.Empty || apId < 0)
            {
                return OperationResult.FromError<ShareBargainBaseResult>(ErrorCode.ParameterError, "参数不正确");
            }
            //检查商品是否可购买
            var result = await BargainManager.CheckBargainProductStatus(ownerId, apId, pid, deviceId);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 用户发起砍价分享活动（已废弃）
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="apId"></param>
        /// <returns>1-分享成功，2-该用户已经分享过该商品，3-该商品下架或被禁用，4-该商品已抢光，0-数据库插入失败</returns>
        public async Task<OperationResult<BargainShareResult>> AddShareBargainAsync(Guid ownerId, int apId, string pid)
        {
            string inputStr = $"入参：ownerId={ownerId},apId={apId},pid={pid}";
            Logger.Info($"分享砍价活动服务 {nameof(AddShareBargainAsync)} {inputStr}");
            if (ownerId == Guid.Empty || apId < 0)
            {
                return OperationResult.FromError<BargainShareResult>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await BargainManager.AddShareBargain(ownerId, apId, pid);

            Logger.Info(
                $"分享砍价活动服务 {nameof(AddShareBargainAsync)} 出参：result={JsonConvert.SerializeObject(result)} {inputStr}");
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 受邀人获取分享产品信息
        /// </summary>
        /// <param name="idKey"></param>
        /// <returns></returns>
        public async Task<OperationResult<BargainShareProductModel>> FetchShareBargainInfoAsync(Guid idKey, Guid UserId)
        {
            if (idKey == Guid.Empty)
            {
                return OperationResult.FromError<BargainShareProductModel>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await BargainManager.FetchShareBargainInfo(idKey, UserId);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<bool>> RefreshShareBargainCacheAsync()
        {
            var result = await BargainManager.RefreshShareBargainCache();
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 获得分享砍价全局配置
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<BargainGlobalConfigModel>> GetShareBargainConfigAsync()
        {
            var result = await BargainManager.GetBargainShareConfig();
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 批量获取产品详情页
        /// </summary>
        /// <param name="OwnerId"></param>
        /// <param name="UserId"></param>
        /// <param name="ProductItems"></param>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<BargainProductModel>>> SelectBargainProductByIdAsync(Guid OwnerId,
            Guid UserId, List<BargainProductItem> ProductItems)
        {
            string inputStr =
                $"入参：OwnerId={OwnerId}，UserId={UserId}，ProductItems={JsonConvert.SerializeObject(ProductItems)}";
            Logger.Info($"分享砍价活动 {nameof(SelectBargainProductByIdAsync)} {inputStr}");

            if (ProductItems == null || ProductItems.Count == 0)
            {
                return OperationResult.FromError<IEnumerable<BargainProductModel>>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await BargainManager.SelectBargainProductById(OwnerId, UserId, ProductItems);

            Logger.Info(
                $"分享砍价活动 {nameof(SelectBargainProductByIdAsync)} 出参：result={JsonConvert.SerializeObject(result)} {inputStr}");
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 分页获取产品PID和apid
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<BargainProductItem>>> SelectBargainProductItemsAsync(Guid UserId,
            int PageIndex, int PageSize)
        {
            string inputStr = $"入参：UserId={UserId}，PageIndex={PageIndex}，PageSize={PageSize}";
            Logger.Info($"分享砍价活动 {nameof(SelectBargainProductItemsAsync)} {inputStr}");

            if (PageIndex < 1 || PageSize < 1)
            {
                return OperationResult.FromError<PagedModel<BargainProductItem>>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await BargainManager.SelectBargainProductItems(UserId, PageIndex, PageSize);

            Logger.Info(
                $"分享砍价活动 {nameof(SelectBargainProductItemsAsync)} 出参：result={JsonConvert.SerializeObject(result)} {inputStr}");
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 根据IdKey获取产品PID和apid
        /// </summary>
        /// <param name="IdKey"></param>
        /// <returns></returns>
        public async Task<OperationResult<BargainProductInfo>> FetchBargainProductItemByShareIdAsync(Guid IdKey)
        {
            if (IdKey == Guid.Empty)
            {
                return OperationResult.FromError<BargainProductInfo>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await BargainManager.FetchBargainProductItemByIdKey(IdKey);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 设置分享idkey的状态
        /// </summary>
        /// <param name="IdKey">分享Id</param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> SetShareBargainStatusAsync(Guid IdKey)
        {
            if (IdKey == Guid.Empty)
            {
                return OperationResult.FromError<bool>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await BargainManager.SetShareBargainStatus(IdKey);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 砍价流程完成，领取优惠券
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="IdKey"></param>
        /// <param name="apId"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public async Task<OperationResult<ShareBargainBaseResult>> MarkUserReceiveCouponAsync(Guid ownerId, int apId,
            string pid, string deviceId = default(string))
        {
            string inputStr = $"入参：ownerId={ownerId},apId={apId},pid={pid}";
            Logger.Info($"分享砍价活动服务 {nameof(MarkUserReceiveCouponAsync)} {inputStr}");
            if (ownerId == Guid.Empty
                || apId < 1
                || string.IsNullOrWhiteSpace(pid))
            {
                return OperationResult.FromError<ShareBargainBaseResult>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await BargainManager.MarkUserReceiveCoupon(ownerId, apId, pid, deviceId);

            Logger.Info(
                $"分享砍价活动服务 {nameof(MarkUserReceiveCouponAsync)} 出参：result={JsonConvert.SerializeObject(result)} {inputStr}");

            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 获取指定时间段中用户的砍价次数
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public async Task<OperationResult<int>> GetUserBargainCountAtTimerangeAsync(Guid ownerId, DateTime beginTime,
            DateTime endTime)
        {
            if (ownerId == Guid.Empty)
            {
                return OperationResult.FromError<int>(ErrorCode.ParameterError, "参数不正确");
            }

            var res = await BargainManager.GetUserBargainCountAtTimerange(ownerId, beginTime, endTime);
            return OperationResult.FromResult(res);
        }

        #region 砍价重构

        /// <summary>
        /// 首页获取默认的两个砍价商品
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<List<SimpleBargainProduct>>> GetBargainProductForIndexAsync()
        {
            var result = await BargainManager.GetBargainProductForIndex();
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 首页获取砍价产品列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<BargainProductItem>>> SelectBargainProductListAsync(int pageIndex,
            int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1)
                return OperationResult.FromError<PagedModel<BargainProductItem>>(ErrorCode.ParameterError, "参数不正确");
            var result = await BargainManager.SelectBargainProductList(pageIndex, pageSize);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 分页获取用户的砍价记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<BargainHistoryModel>>> SelectBargainHistoryAsync(int pageIndex,
            int pageSize, Guid userId)
        {
            if (pageIndex < 1 || pageSize < 1 || userId == Guid.Empty)
                return OperationResult.FromError<PagedModel<BargainHistoryModel>>(ErrorCode.ParameterError, "参数不正确");
            var result = await BargainManager.SelectBargainHistory(pageIndex, pageSize, userId);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 获取砍价商品详情
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productItems"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<BargainProductNewModel>>> GetBargsinProductDetailAsync(Guid userId,
            List<BargainProductItem> productItems)
        {
            if (!productItems.Any())
                return OperationResult.FromError<List<BargainProductNewModel>>(ErrorCode.ParameterError, "参数不正确");
            var result = await BargainManager.GetBargsinProductDetail(userId, productItems);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 获取轮播信息
        /// </summary>
        /// <param name="count">每次获取最新的N条</param>
        /// <returns></returns>
        public async Task<OperationResult<List<SliceShowInfoModel>>> GetSliceShowInfoAsync(int count = 10)
        {
            if (count < 1 || count > 100)
                return OperationResult.FromError<List<SliceShowInfoModel>>(ErrorCode.ParameterError, "参数不正确");
            var result = await BargainManager.GetSliceShowInfo(count);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 用户创建并砍价(CreateUserBargainAsync)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="apId"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public async Task<OperationResult<CreateBargainResult>> CreateserBargainAsync(Guid userId, int apId, string pid)
        {
            if (userId == Guid.Empty || apId < 1 || string.IsNullOrWhiteSpace(pid))
            {
                return OperationResult.FromError<CreateBargainResult>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await BargainManager.CreateUserBargain(userId, apId, pid);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 受邀人获取砍价结果
        /// </summary>
        /// <param name="idKey"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<OperationResult<InviteeBarginInfo>> GetInviteeBargainInfoAsync(Guid idKey, Guid userId)
        {
            if (idKey == Guid.Empty || userId == Guid.Empty)
            {
                return OperationResult.FromError<InviteeBarginInfo>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await BargainManager.GetInviteeBargainInfo(idKey, userId);
            return OperationResult.FromResult(result);
        }

        #endregion
    }
}
