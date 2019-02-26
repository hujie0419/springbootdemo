using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Manager;

namespace Tuhu.Service.Activity.Server
{
    public class SalePromotionActivityService : ISalePromotionActivityService
    {
        #region 操作

        /// <summary>
        /// 新增促销活动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> InsertActivityAsync(SalePromotionActivityModel model)
        {
            return await SalePromotionActivityManager.InsertActivityAsync(model);
        }

        /// <summary>
        /// 修改促销活动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> UpdateActivityAsync(SalePromotionActivityModel model)
        {
            return await SalePromotionActivityManager.UpdateActivityAsync(model);
        }

        /// <summary>
        /// 审核后修改促销活动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> UpdateActivityAfterAuditAsync(SalePromotionActivityModel model)
        {
            return await SalePromotionActivityManager.UpdateActivityAfterAuditAsync(model);
        }

        /// <summary>
        /// 手动下架促销活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> UnShelveActivityAsync(string activityId, string userName)
        {
            return await SalePromotionActivityManager.UnShelveActivityAsync(activityId, userName);
        }

        /// <summary>
        /// 设置促销活动审核状态
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="auditUserName"></param>
        /// <param name="auditStatus"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> SetActivityAuditStatusAsync(string activityId, string auditUserName, int auditStatus, string remark)
        {
            return await SalePromotionActivityManager.SetActivityAuditStatusAsync(activityId, auditUserName, auditStatus, remark);
        }

        /// <summary>
        /// 获取促销活动审核状态
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public async Task<OperationResult<int>> GetActivityAuditStatusAsync(string activityId)
        {
            return await SalePromotionActivityManager.GetActivityAuditStatusAsync(activityId);
        }

        /// <summary>
        /// 促销活动批量添加商品
        /// </summary>
        /// <param name="productList"></param>
        /// <param name="activityId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> InsertActivityProductListAsync(List<SalePromotionActivityProduct> productList, string activityId, string userName)
        {
            return await SalePromotionActivityManager.InsertActivityProductListAsync(productList, activityId, userName);
        }

        /// <summary>
        /// 批量设置促销活动商品限购库存
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <param name="stock"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<OperationResult<int>> SetProductLimitStockAsync(string activityId, List<string> pidList, int stock, string userName)
        {
            return await SalePromotionActivityManager.SetProductLimitStockAsync(activityId, pidList, stock, userName);
        }

        /// <summary>
        /// 从活动中移除商品
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="activityId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<OperationResult<int>> DeleteProductFromActivityAsync(string pid, string activityId, string userName)
        {
            return await SalePromotionActivityManager.DeleteProductFromActivityAsync(pid, activityId, userName);
        }

        #endregion

        #region  查询

        /// <summary>
        /// 分页查询活动列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<OperationResult<SelectActivityListModel>> SelectActivityListAsync(SalePromotionActivityModel model, int pageIndex, int pageSize)
        {
            return await SalePromotionActivityManager.SelectActivityListAsync(model, pageIndex, pageSize);
        }

        /// <summary>
        /// 获取活动的打折规则
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<SalePromotionActivityDiscount>>> GetActivityContentAsync(string activityId)
        {
            return await SalePromotionActivityManager.GetActivityContentAsync(activityId);
        }

        /// <summary>
        /// 获取活动信息、打折规则
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public async Task<OperationResult<SalePromotionActivityModel>> GetActivityInfoAsync(string activityId)
        {
            return await SalePromotionActivityManager.GetActivityInfoAsync(activityId);
        }

        /// <summary>
        /// 根据活动id和pids获取活动商品列表
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<SalePromotionActivityProduct>>> GetProductInfoListAsync(string activityId, List<string> pidList)
        {
            return await SalePromotionActivityManager.GetProductInfoListAsync(activityId, pidList);
        }

        /// <summary>
        /// 按条件分页查询活动商品
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<SalePromotionActivityProduct>>> SelectProductListAsync(SelectActivityProduct condition, int pageIndex, int pageSize)
        {
            return await SalePromotionActivityManager.SelectProductListAsync(condition, pageIndex, pageSize);
        }

        /// <summary>
        /// 查询在当前活动时间内，与其他活动商品重复的商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList">检查的商品pid</param>
        /// <returns></returns>
        public async Task<OperationResult<IList<SalePromotionActivityProduct>>> GetRepeatProductListAsync(string activityId, List<string> pidList)
        {
            return await SalePromotionActivityManager.GetRepeatProductListAsync(activityId, pidList);
        }

        /// <summary>
        /// 查询在当前活动时间内 与其他活动商品重复的所有商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public async Task<OperationResult<IList<SalePromotionActivityProduct>>> GetActivityRepeatProductListAsync(string activityId, string startTime, string endTime)
        {
            return await SalePromotionActivityManager.GetActivityRepeatProductListAsync(activityId, startTime, endTime);
        }

        #endregion

        /// <summary>
        /// 批量新增、删除活动的商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="stock"></param>
        /// <param name="addList"></param>
        /// <param name="delList"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> AddAndDelActivityProductAsync(string activityId, int stock, List<SalePromotionActivityProduct> addList, List<string> delList, string userName)
        {
            return await SalePromotionActivityManager.AddAndDelActivityProductAsync(activityId, stock, addList, delList, userName);
        }

        /// <summary>
        /// 同步商品信息到打折商品表
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="productList"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> RefreshProductAsync(string activityId, List<SalePromotionActivityProduct> productList)
        {
            return await SalePromotionActivityManager.RefreshProductAsync(activityId, productList);
        }

        /// <summary>
        /// 批量设置活动商品列表牛皮癣
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <param name="imgUrl"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<OperationResult<int>> SetProductImageAsync(string activityId, List<string> pidList, string imgUrl, string userName)
        {
            return await SalePromotionActivityManager.SetProductImageAsync(activityId, pidList, imgUrl, userName);
        }

        /// <summary>
        /// 设置打折商品详情页牛皮癣
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<SetDiscountProductDetailImgResponse>> SetDiscountProductDetailImgAsync(SetDiscountProductDetailImgRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ActivityId))
            {
                return OperationResult.FromError<SetDiscountProductDetailImgResponse>("-1", "参数错误");
            }

            var updateResult = await SalePromotionActivityManager.SetDiscountProductDetailImgAsync(request);

            return updateResult;
        }
         

        /// <summary>
        /// 获取活动信息，包括打折规则和商品信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pidList"></param>
        /// <returns></returns>
        public async Task<OperationResult<SalePromotionActivityModel>> GetActivityAndProductsAsync(string activityId, List<string> pidList)
        {
            return await SalePromotionActivityManager.GetActivityAndProductsAsync(activityId, pidList);
        }


        #region 审核权限

        /// <summary>
        /// 新增促销活动审核权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<OperationResult<int>> InsertAuditAuthAsync(SalePromotionAuditAuth model)
        {
            return await SalePromotionActivityManager.InsertAuditAuthAsync(model);
        }

        /// <summary>
        /// 删除促销活动审核权限
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public async Task<OperationResult<int>> DeleteAuditAuthAsync(int PKID)
        {
            return await SalePromotionActivityManager.DeleteAuditAuthAsync(PKID);
        }

        /// <summary>
        /// 分页查询用户审核权限
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<SalePromotionAuditAuth>>> SelectAuditAuthListAsync(SalePromotionAuditAuth searchModel, int pageIndex, int pageSize)
        {
            return await SalePromotionActivityManager.SelectAuditAuthListAsync(searchModel, pageIndex, pageSize);
        }

        /// <summary>
        /// 根据类型和username获取用户审核权限信息
        /// </summary>
        /// <param name="promotionType"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<OperationResult<SalePromotionAuditAuth>> GetUserAuditAuthAsync(int promotionType, string userName)
        {
            return await SalePromotionActivityManager.GetUserAuditAuthAsync(promotionType, userName);
        }

        #endregion

    }
}
