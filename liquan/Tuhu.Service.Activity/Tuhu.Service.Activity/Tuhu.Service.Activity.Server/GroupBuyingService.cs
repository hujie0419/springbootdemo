using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Server.Manager;
using Tuhu.Service.Product;

namespace Tuhu.Service.Activity.Server
{
    public class GroupBuyingService : IGroupBuyingService
    {
        #region [产品信息]
        /// <summary>
        /// 分页获取首页ProductGroupId
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="flag">首页-false；热门产品拼团-true</param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<string>>> GetGroupBuyingProductListAsync(int PageIndex = 1,
            int PageSize = 10, bool flag = false, string channel = default(string), bool isOldUser = false)
        {
            if (PageSize < 0 || PageIndex < 0)
                return OperationResult.FromError<PagedModel<string>>(ErrorCode.ParameterError, "参数不正确");

            var result = await GroupBuyingManager.GetGroupBuyingProductList(PageIndex, PageSize, flag, isOldUser, -99, channel);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 根据ProductGroupId获取对应产品的PID
        /// </summary>
        /// <param name="ProductGroupId"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<string>>> SelectGroupBuyingProductsByIdAsync(string ProductGroupId)
        {
            if (string.IsNullOrWhiteSpace(ProductGroupId))
            {
                return OperationResult.FromError<List<string>>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await GroupBuyingManager.SelectGroupBuyingProductsById(ProductGroupId);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 根据ProductGroupId获取对应ProductGroup信息
        /// </summary>
        /// <param name="ProductGroupIds"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<ProductGroupModel>>> SelectProductGroupInfoAsync(
            List<string> ProductGroupIds)
        {
            if (ProductGroupIds == null || !ProductGroupIds.Any())
            {
                return OperationResult.FromError<List<ProductGroupModel>>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await GroupBuyingManager.SelectProductGroupInfo(ProductGroupIds);
            return OperationResult.FromResult(result);
        }
        /// <summary>
        /// 获取产品组中产品详细信息
        /// </summary>
        /// <param name="ProductGroupId"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<ProductGroupModel>>> SelectProductGroupDetailAsync(string ProductGroupId)
        {
            if (string.IsNullOrWhiteSpace(ProductGroupId))
            {
                return OperationResult.FromError<List<ProductGroupModel>>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await GroupBuyingManager.SelectProductGroupDetail(ProductGroupId);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 根据PID获取对应产品的信息
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<GroupBuyingProductModel>> SelectProductInfoByPidAsync(string ProductGroupId,
            string Pid)
        {
            if (string.IsNullOrWhiteSpace(ProductGroupId) || string.IsNullOrWhiteSpace(Pid))
            {
                return OperationResult.FromError<GroupBuyingProductModel>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await GroupBuyingManager.SelectProductInfoByPid(ProductGroupId, Pid);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 查询拼团拼团类目信息
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<List<GroupBuyingCategoryModel>>> GetGroupBuyingCategoryInfoAsync()
        {
            var result = await GroupBuyingManager.GetGroupBuyingCategoryInfo();
            return OperationResult.FromResult(result);
        }
        /// <summary>
        /// 查询拼团产品信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<SimplegroupBuyingModel>>> GetGroupBuyingProductListNewAsync(
            GroupBuyingQueryRequest request)
        {
            if (request.PageIndex < 1 || request.PageSize < 1 || request.NewCategoryCode < 0)
            {
                return OperationResult.FromError<PagedModel<SimplegroupBuyingModel>>(ErrorCode.ParameterError, "参数不正确");
            }
            var result = await GroupBuyingManager.GetGroupBuyingProductListNew(request);
            return OperationResult.FromResult(result);
        }
        /// <summary>
        /// 刷新ES数据
        /// </summary>
        /// <param name="productGroupIds"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> UpdateGroupBuyingInfoAsync(List<string> productGroupIds)
        {
            if (!productGroupIds.Any() || productGroupIds.Count > 1000)
            {
                return OperationResult.FromError<bool>(ErrorCode.ParameterError, "参数不正确");
            }
            var result = await GroupBuyingManager.UpdateGroupBuyingInfo(productGroupIds);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 根据PID获取对应产品的信息(批量)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<ProductGroupModel>>> SelectProductListByPidsAsync(
            List<GroupBuyingProductRequest> request)
        {
            if (request == null || request.Count == 0 || request.Count > 50 || request.Exists(g =>
                    string.IsNullOrWhiteSpace(g.ProductGroupId) || string.IsNullOrWhiteSpace(g.PId)))
            {
                return OperationResult.FromError<List<ProductGroupModel>>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await GroupBuyingManager.SelectProductListByPids(request);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 根据PID获取所属ProductGroupId以及价格
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        public async Task<OperationResult<GroupBuyingProductInfo>> GetProductGroupInfoByPIdAsync(string pId)
        {
            if (string.IsNullOrWhiteSpace(pId))
                return OperationResult.FromError<GroupBuyingProductInfo>(ErrorCode.ParameterError, "参数不正确");
            var result = await GroupBuyingManager.GetProductGroupInfoByPId(pId);
            return OperationResult.FromResult(result);
        }
        /// <summary>
        /// 根据关键词从ES搜索拼团信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Obsolete("已废弃，请使用SelectGroupBuyingListNewAsync", true)]
        public async Task<OperationResult<PagedModel<SimplegroupBuyingModel>>> SearchGroupBuyingByKeywordAsync(GroupBuyingQueryRequest request)
        {
            var result = await Task.Run(() => new PagedModel<SimplegroupBuyingModel>());
            return OperationResult.FromResult(result);
        }
        /// <summary>
        /// 查询拼团产品列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<SimplegroupBuyingModel>>> SelectGroupBuyingListNewAsync(GroupBuyingQueryRequest request)
        {
            if (request.PageIndex < 1 || request.PageSize < 1 || request.NewCategoryCode < 0)
            {
                return OperationResult.FromError<PagedModel<SimplegroupBuyingModel>>(ErrorCode.ParameterError, "参数不正确");
            }
            var result = await GroupBuyingManager.SelectGroupBuyingListCache(request);
            return OperationResult.FromResult(result);
        }
        #endregion

        #region [校验/缓存]

        /// <summary>
        /// 校验用户的参团资格
        /// </summary>
        /// <param name="GroupId"></param>
        /// <param name="UserId"></param>
        /// <param name="ProductGroupId"></param>
        /// <returns></returns>
        public async Task<OperationResult<CheckResultModel>> CheckGroupInfoByUserIdAsync(Guid GroupId, Guid UserId,
            string ProductGroupId, string pid = default(string))
        {
            if ((GroupId == Guid.Empty || UserId == Guid.Empty) && string.IsNullOrWhiteSpace(ProductGroupId))
                return OperationResult.FromError<CheckResultModel>(ErrorCode.ParameterError, "参数不正确");

            var result = new CheckResultModel();
            if (!string.IsNullOrWhiteSpace(pid) && !(await GroupBuyingManager.CheckProductOnSale(pid)))
            {
                result.Code = 12;
                result.Info = "产品库已下架";
                return OperationResult.FromResult(result);
            }
            if (GroupId == Guid.Empty && !string.IsNullOrWhiteSpace(ProductGroupId))
            {
                result = await GroupBuyingManager.CheckProductGroupInfoById(ProductGroupId, UserId, pid);
            }
            else
            {
                result = await GroupBuyingManager.CheckGroupInfoByUserId(GroupId, UserId, ProductGroupId, pid);
            }

            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 刷新拼团产品缓存
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<VerificationResultModel>> RefreshCacheAsync(string ProductGroupId = null)
        {
            var result = await GroupBuyingManager.RefreshCacheAsync(ProductGroupId);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 刷新团信息缓存(未使用)
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public async Task<OperationResult<VerificationResultModel>> RefreshGroupCacheAsync(Guid GroupId)
        {
            var result = await Task.Run(() => new VerificationResultModel());
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 拼团校验新人
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<OperationResult<NewUserCheckResultModel>> CheckNewUserAsync(Guid userId,
            string openId = default(string))
        {
            if (userId == Guid.Empty)
            {
                return OperationResult.FromError<NewUserCheckResultModel>(ErrorCode.ParameterError, "参数不正确");
            }

            var data = await GroupBuyingManager.CheckNewUser(userId);
            var result = new NewUserCheckResultModel
            {
                CheckResult = data.Item1 == Guid.Empty
            };
            return OperationResult.FromResult(result);
        }


        /// <summary>
        /// 查询用户限购信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<GroupBuyingBuyLimitModel>> GetUserBuyLimitInfoAsync(GroupBuyingBuyLimitRequest request)
        {
            if (request.UserId == Guid.Empty || string.IsNullOrWhiteSpace(request.ProductGroupId) || string.IsNullOrWhiteSpace(request.PID))
            {
                return OperationResult.FromError<GroupBuyingBuyLimitModel>(ErrorCode.ParameterError, "参数不正确");
            }
            var result = await GroupBuyingManager.GetUserBuyLimitInfo(request.PID, request.ProductGroupId, request.UserId);
            return OperationResult.FromResult(result);
        }
        #endregion

        #region [获取团信息]

        /// <summary>
        /// 分页获取该用户拼团记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<UserGroupBuyingInfoModel>>> GetUserGroupInfoByUserIdAsync(
            GroupInfoRequest request)
        {
            if (request == null || request.Type > 5 || request.PageIndex < 1 || request.PageSize < 0)
            {
                return OperationResult.FromError<PagedModel<UserGroupBuyingInfoModel>>(ErrorCode.ParameterError,
                    "参数不正确");
            }

            var result = await GroupBuyingManager.GetUserGroupInfoByUserId(request);
            return OperationResult.FromResult(result);
        }

        public async Task<OperationResult<GroupBuyingHistoryCount>> GetUserGroupCountByUserIdAsync(Guid userId)
        {
            var result = await GroupBuyingManager.GetUserGroupCountByUserId(userId);
            return OperationResult.FromResult(result);
        }
        /// <summary>
        /// 根据团号获取当前团成员
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public async Task<OperationResult<GroupMemberModel>> SelectGroupMemberByGroupIdAsync(Guid GroupId)
        {
            if (GroupId == Guid.Empty)
            {
                return OperationResult.FromError<GroupMemberModel>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await GroupBuyingManager.SelectGroupMemberByGroupId(GroupId);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 获取该产品组下，该用户可以参加的若干个团(废弃)
        /// </summary>
        /// <param name="ProductGroupId"></param>
        /// <param name="UserId"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<GroupInfoModel>>> SelectGroupInfoByProductGroupIdAsync(
            string ProductGroupId, Guid UserId, int Count = 100)
        {
            if (string.IsNullOrWhiteSpace(ProductGroupId) || Count < 1)
            {
                return OperationResult.FromError<List<GroupInfoModel>>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await GroupBuyingManager.SelectGroupInfoByProductGroupId(ProductGroupId, UserId, Count);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 获取该产品组下，该用户可以参加的若干个团(包含团总数)
        /// </summary>
        /// <param name="productGroupId"></param>
        /// <param name="userId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<OperationResult<GroupInfoResponse>> SelectGroupInfoWithTotalCountAsync(
            string productGroupId, Guid userId, int count = 100)
        {
            if (string.IsNullOrWhiteSpace(productGroupId) || count < 1)
            {
                return OperationResult.FromError<GroupInfoResponse>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await GroupBuyingManager.SelectGroupInfoWithTotalCount(productGroupId, userId, count);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 根据团号获取拼团信息
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public async Task<OperationResult<GroupInfoModel>> FetchGroupInfoByGroupIdAsync(Guid GroupId)
        {
            if (GroupId == Guid.Empty)
            {
                return OperationResult.FromError<GroupInfoModel>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await GroupBuyingManager.FetchGroupInfoByGroupId(GroupId);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 根据OrderId查询团信息
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public async Task<OperationResult<GroupInfoModel>> FetchGroupInfoByOrderIdAsync(int OrderId)
        {
            if (OrderId < 1)
            {
                return OperationResult.FromError<GroupInfoModel>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await GroupBuyingManager.FetchGroupInfoByOrderId(OrderId);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 根据ProductGroupId查询产品组详情
        /// </summary>
        /// <param name="ProductGroupId"></param>
        /// <returns></returns>
        public async Task<OperationResult<ProductGroupModel>> FetchProductGroupInfoByIdAsync(string ProductGroupId)
        {
            if (string.IsNullOrWhiteSpace(ProductGroupId))
            {
                return OperationResult.FromError<ProductGroupModel>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await GroupBuyingManager.FetchProductGroupInfoById(ProductGroupId);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 获取最终成团的用户信息
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<GroupFinalUserModel>>> GetGroupFinalUserListAsync(Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                return OperationResult.FromError<List<GroupFinalUserModel>>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await GroupBuyingManager.GetGroupFinalUserList(groupId);
            return OperationResult.FromResult(result);
        }


        /// <summary>
        /// 根据团号，UserId获取用户订单信息
        /// </summary>
        /// <param name="GroupId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<OperationResult<UserOrderInfoModel>> FetchUserOrderInfoAsync(Guid GroupId, Guid UserId)
        {
            if (GroupId == Guid.Empty || UserId == Guid.Empty)
            {
                return OperationResult.FromError<UserOrderInfoModel>(ErrorCode.ParameterError, "参数不正确");
            }

            var result = await GroupBuyingManager.FetchUserOrderInfo(GroupId, UserId);
            return OperationResult.FromResult(result);
        }
        #endregion

        #region [用户参拼团]

        /// <summary>
        /// 用户创建新团
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ProductGroupId"></param>
        /// <param name="Pid"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public async Task<OperationResult<VerificationResultModel>> CreateGroupBuyingAsync(Guid UserId,
            string ProductGroupId, string Pid, int OrderId)
        {
            if (UserId == Guid.Empty || string.IsNullOrWhiteSpace(ProductGroupId) || string.IsNullOrWhiteSpace(Pid) ||
                OrderId < 1)
                return OperationResult.FromError<VerificationResultModel>(ErrorCode.ParameterError, "参数不正确");

            var result =
                await GroupBuyingManager.CreateGroupBuyingInfo(UserId, ProductGroupId, Pid, OrderId, Guid.Empty);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 用户参与拼团
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="GroupId"></param>
        /// <param name="Pid"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public async Task<OperationResult<VerificationResultModel>> TakePartInGroupBuyingAsync(Guid UserId,
            Guid GroupId, string Pid, int OrderId)
        {
            if (UserId == Guid.Empty || GroupId == Guid.Empty || string.IsNullOrWhiteSpace(Pid) || OrderId < 0)
                return OperationResult.FromError<VerificationResultModel>(ErrorCode.ParameterError, "参数不正确");

            var result = await GroupBuyingManager.TakePartInGroupBuying(UserId, GroupId, Pid, OrderId);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 用户取消订单
        /// </summary>
        /// <param name="GroupId"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public async Task<OperationResult<VerificationResultModel>> CancelGroupBuyingOrderAsync(Guid GroupId,
            int OrderId)
        {
            if (GroupId == Guid.Empty || OrderId < 1)
                return OperationResult.FromError<VerificationResultModel>(ErrorCode.ParameterError, "参数不正确");

            var result = await GroupBuyingManager.CancelGroupBuyingOrder(GroupId, OrderId, Guid.Empty);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 团长付款，该团可见
        /// </summary>
        /// <param name="GroupId"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> ChangeGroupBuyingStatusAsync(Guid GroupId, int OrderId)
        {
            if (GroupId == Guid.Empty || OrderId < 0)
                return OperationResult.FromError<bool>(ErrorCode.ParameterError, "参数不正确");

            var result = await GroupBuyingManager.ChangeGroupBuyingStatus(GroupId, OrderId);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 团员付款加入
        /// </summary>
        /// <param name="GroupId"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> ChangeUserStatusAsync(Guid GroupId, Guid UserId, int OrderId)
        {
            if (GroupId == Guid.Empty || OrderId < 0)
                return OperationResult.FromError<bool>(ErrorCode.ParameterError, "参数不正确");

            var result = await GroupBuyingManager.ChangeUserStatus(GroupId, UserId, OrderId);
            return OperationResult.FromResult(result);
        }

        #endregion

        #region [拼团过期/拼团推送]
        /// <summary>
        /// 拼团过期
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public async Task<OperationResult<VerificationResultModel>> ExpireGroupBuyingAsync(Guid GroupId)
        {
            if (GroupId == Guid.Empty)
                return OperationResult.FromError<VerificationResultModel>(ErrorCode.ParameterError, "参数不正确");

            var result = await GroupBuyingManager.ExpireGroupBuying(GroupId);
            return OperationResult.FromResult(result);
        }
        /// <summary>
        /// 拼团推送
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> PushPinTuanMessageAsync(Guid groupId, int batchId)
        {
            if (groupId == Guid.Empty || batchId < 1649)
                return OperationResult.FromError<bool>(ErrorCode.ParameterError, "参数不正确");
            var result = await GroupBuyingManager.PushPinTuanMessage(groupId, batchId);
            return OperationResult.FromResult(result);
        }
        #endregion

        #region [拼团抽奖]

        /// <summary>
        /// 获取抽奖规则
        /// </summary>
        /// <param name="productGroupId"></param>
        /// <returns></returns>
        public async Task<OperationResult<GroupLotteryRuleModel>> GetLotteryRuleAsync(string productGroupId)
        {
            if (string.IsNullOrWhiteSpace(productGroupId))
                return OperationResult.FromError<GroupLotteryRuleModel>(ErrorCode.ParameterError, "参数不正确");
            var result = await GroupBuyingManager.GetLotteryRule(productGroupId);
            if (result == null) return OperationResult.FromError<GroupLotteryRuleModel>(ErrorCode.DataNotExisted, "规则不存在");
            return OperationResult.FromResult(result);
        }
        /// <summary>
        /// 获取中奖名单
        /// </summary>
        /// <param name="productGroupId"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<GroupBuyingLotteryInfo>>> GetWinnerListAsync(string productGroupId, int level = 0, int pageIndex = 1, int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(productGroupId) || level < 0 || pageIndex < 1 || pageSize < 0)
            {
                return OperationResult.FromError<PagedModel<GroupBuyingLotteryInfo>>(ErrorCode.ParameterError, "参数不正确");
            }
            if (pageSize == 0)
            {
                pageIndex = 1;
                pageSize = 100;
            }
            var result = await GroupBuyingManager.GetWinnerList(productGroupId, level, pageIndex, pageSize);
            return OperationResult.FromResult(result);
        }
        /// <summary>
        /// 查询用户中奖结果(单个订单)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productGroupId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<OperationResult<GroupBuyingLotteryInfo>> CheckUserLotteryResultAsync(Guid userId, string productGroupId, int orderId)
        {
            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(productGroupId) || orderId < 1)
                return OperationResult.FromError<GroupBuyingLotteryInfo>(ErrorCode.ParameterError, "参数不正确");
            var result = await GroupBuyingManager.CheckUserLotteryResult(userId, productGroupId, orderId);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 查询用户的中奖纪录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<GroupBuyingLotteryInfo>>> GetUserLotteryHistoryAsync(Guid userId, List<int> orderIds)
        {
            if (userId == Guid.Empty)
                return OperationResult.FromError<List<GroupBuyingLotteryInfo>>(ErrorCode.ParameterError, "参数不正确");
            var result = await GroupBuyingManager.GetUserLotteryHistory(userId, orderIds);
            return OperationResult.FromResult(result);
        }
        #endregion

        #region [团长免单]
        /// <summary>
        /// 按照拼团类型获取产品列表
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<string>>> GetActivityProductGroupAsync(ActivityGroupRequest request)
        {
            if (request.PageIndex < 1 || request.PageIndex < 1 || request.Type > 3)
            {
                return OperationResult.FromError<PagedModel<string>>(ErrorCode.ParameterError, "参数不正确");
            }
            var result = await GroupBuyingManager.GetGroupBuyingProductList(request.PageIndex, request.PageSize, false, false, request.Type);
            return OperationResult.FromResult(result);
        }
        /// <summary>
        /// 查询用户免单券
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<FreeCouponModel>>> GetUserFreeCouponAsync(Guid userId)
        {
            var result = await GroupBuyingManager.GetUserFreeCoupon(userId);
            return OperationResult.FromResult(result);
        }
        #endregion
    }
}
