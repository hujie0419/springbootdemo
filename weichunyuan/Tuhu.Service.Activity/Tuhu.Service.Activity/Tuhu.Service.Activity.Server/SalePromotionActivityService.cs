using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Server.Manager;

namespace Tuhu.Service.Activity.Server
{
    public class SalePromotionActivityService : ISalePromotionActivityService
    {
        #region 操作
        public async Task<OperationResult<bool>> InsertActivityAsync(SalePromotionActivityModel model)
        {
            return await SalePromotionActivityManager.InsertActivityAsync(model);
        }
        public async Task<OperationResult<bool>> UpdateActivityAsync(SalePromotionActivityModel model)
        {
            return await SalePromotionActivityManager.UpdateActivityAsync(model);
        }

        public async Task<OperationResult<bool>> UpdateActivityAfterAuditAsync(SalePromotionActivityModel model)
        {
            return await SalePromotionActivityManager.UpdateActivityAfterAuditAsync(model);
        }

        public async Task<OperationResult<bool>> UnShelveActivityAsync(string activityId, string userName)
        {
            return await SalePromotionActivityManager.UnShelveActivityAsync(activityId, userName);
        }

        public async Task<OperationResult<bool>> SetActivityAuditStatusAsync(string activityId, string auditUserName, int auditStatus, string remark)
        {
            return await SalePromotionActivityManager.SetActivityAuditStatusAsync(activityId, auditUserName, auditStatus, remark);
        }
        public async Task<OperationResult<int>> GetActivityAuditStatusAsync(string activityId)
        {
            return await SalePromotionActivityManager.GetActivityAuditStatusAsync(activityId);
        }
        public async Task<OperationResult<bool>> InsertActivityProductListAsync(List<SalePromotionActivityProduct> productList, string activityId, string userName)
        {
            return await SalePromotionActivityManager.InsertActivityProductListAsync(productList, activityId, userName);
        }

        public async Task<OperationResult<int>> SetProductLimitStockAsync(string activityId, List<string> pidList, int stock, string userName)
        {
            return await SalePromotionActivityManager.SetProductLimitStockAsync(activityId, pidList, stock, userName);
        }

        public async Task<OperationResult<int>> DeleteProductFromActivityAsync(string pid, string activityId, string userName)
        {
            return await SalePromotionActivityManager.DeleteProductFromActivityAsync(pid, activityId, userName);
        }

        #endregion

        #region  查询

        public async Task<OperationResult<SelectActivityListModel>> SelectActivityListAsync(SalePromotionActivityModel model, int pageIndex, int pageSize)
        {
            return await SalePromotionActivityManager.SelectActivityListAsync(model, pageIndex, pageSize);
        }
        public async Task<OperationResult<List<SalePromotionActivityDiscount>>> GetActivityContentAsync(string activityId)
        {
            return await SalePromotionActivityManager.GetActivityContentAsync(activityId);
        }

        public async Task<OperationResult<SalePromotionActivityModel>> GetActivityInfoAsync(string activityId)
        {
            return await SalePromotionActivityManager.GetActivityInfoAsync(activityId);
        }
        public async Task<OperationResult<IEnumerable<SalePromotionActivityProduct>>> GetProductInfoListAsync(string activityId, List<string> pidList)
        {
            return await SalePromotionActivityManager.GetProductInfoListAsync(activityId, pidList);
        }
        public async Task<OperationResult<PagedModel<SalePromotionActivityProduct>>> SelectProductListAsync(SelectActivityProduct condition, int pageIndex, int pageSize)
        {
            return await SalePromotionActivityManager.SelectProductListAsync(condition, pageIndex, pageSize);
        }

        public async Task<OperationResult<IList<SalePromotionActivityProduct>>> GetRepeatProductListAsync(string activityId, List<string> pidList)
        {
            return await SalePromotionActivityManager.GetRepeatProductListAsync(activityId, pidList);
        }
        public async Task<OperationResult<IList<SalePromotionActivityProduct>>> GetActivityRepeatProductListAsync(string activityId, string startTime, string endTime)
        {
            return await SalePromotionActivityManager.GetActivityRepeatProductListAsync(activityId, startTime, endTime);
        }
        public async Task<OperationResult<bool>> AddAndDelActivityProductAsync(string activityId, int stock, List<SalePromotionActivityProduct> addList, List<string> delList, string userName)
        {
            return await SalePromotionActivityManager.AddAndDelActivityProductAsync(activityId, stock, addList, delList, userName);
        }
        public async Task<OperationResult<bool>> RefreshProductAsync(string activityId, List<SalePromotionActivityProduct> productList)
        {
            return await SalePromotionActivityManager.RefreshProductAsync(activityId, productList);
        }
        public async Task<OperationResult<int>> SetProductImageAsync(string activityId, List<string> pidList, string imgUrl, string userName)
        {
            return await SalePromotionActivityManager.SetProductImageAsync(activityId, pidList, imgUrl, userName);
        }

        public async Task<OperationResult<SalePromotionActivityModel>> GetActivityAndProductsAsync(string activityId, List<string> pidList)
        {
            return await SalePromotionActivityManager.GetActivityAndProductsAsync(activityId, pidList);
        }

        #endregion

        #region 审核权限

        /// <summary>
        /// 新增促销活动审核权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<OperationResult<int>> InsertAuditAuthAsync(SalePromotionAuditAuth model)
        {
            //await Task.Yield();
            // return null;
            return await SalePromotionActivityManager.InsertAuditAuthAsync(model);
        }

        /// <summary>
        /// 删除促销活动审核权限
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public async Task<OperationResult<int>> DeleteAuditAuthAsync(int PKID)
        {
            //await Task.Yield();
            // return null;
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
            //await Task.Yield();
            // return null;
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
            //await Task.Yield();
            // return null;
            return await SalePromotionActivityManager.GetUserAuditAuthAsync(promotionType, userName);
        }

        #endregion

    }
}
