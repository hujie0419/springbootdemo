using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;

namespace Tuhu.Service.Activity.Server.Manager
{
    public static class SalePromotionActivityManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SalePromotionActivityManager));

        #region 操作

        /// <summary>
        /// 新增活动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> InsertActivityAsync(SalePromotionActivityModel model)
        {
            OperationResult<bool> result;
            //检查名称重复
            var is_repeat = await DalSalePromotionActivity.CheckNameRepeatAsync(model.Name, model.ActivityId);
            if (!is_repeat)
            {
                return OperationResult.FromError<bool>("1", "活动名称已存在");
            }
            else
            {
                var insertResult = await DalSalePromotionActivity.InsertActivityAsync(model);
                if (!insertResult)
                {
                    result = OperationResult.FromResult(false);
                }
                else
                {
                    result = OperationResult.FromResult(true);
                }
            }
            return result;
        }

        /// <summary>
        /// 修改活动信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> UpdateActivityAsync(SalePromotionActivityModel model)
        {
            OperationResult<bool> result;
            //检查名称重复
            var is_repeat = await DalSalePromotionActivity.CheckNameRepeatAsync(model.Name, model.ActivityId);
            if (!is_repeat)
            {
                return OperationResult.FromError<bool>("1", "活动名称已存在");
            }
            else
            {
                var updateResult = await DalSalePromotionActivity.UpdateActivityAsync(model);
                if (!updateResult)
                {
                    result = OperationResult.FromResult(false);
                }
                else
                {
                    result = OperationResult.FromResult(true);
                }
            }
            return result;
        }

        /// <summary>
        /// 审核后修改活动信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> UpdateActivityAfterAuditAsync(SalePromotionActivityModel model)
        {
            OperationResult<bool> result;
            //检查名称重复
            var is_repeat = await DalSalePromotionActivity.CheckNameRepeatAsync(model.Name, model.ActivityId);
            if (!is_repeat)
            {
                return OperationResult.FromError<bool>("1", "活动名称已存在");
            }
            else
            {
                var updateResult = await DalSalePromotionActivity.UpdateActivityAfterAuditAsync(model);
                if (!updateResult)
                {
                    result = OperationResult.FromResult(false);
                }
                else
                {
                    result = OperationResult.FromResult(true);
                }
            }
            return result;
        }

        public static async Task<OperationResult<bool>> UnShelveActivityAsync(string activityId, string userName)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.UnShelveActivityAsync(activityId, userName));
        }

        /// <summary>
        /// 修改活动的审核状态
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="auditUserName"></param>
        /// <param name="auditStatus"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> SetActivityAuditStatusAsync(string activityId, string auditUserName, int auditStatus, string remark)
        {
            bool result;
            result = await DalSalePromotionActivity.SetActivityAuditStatusAsync(activityId, auditUserName, auditStatus, remark);
            if (result && auditStatus == 2)
            {
                //审核活动， 自动上下架
                await DiscountActivityInfoManager.AuditShelveActivityProduct(activityId);
            }
            return OperationResult.FromResult(result); ;
        }

        #endregion

        #region 查询

        public static async Task<OperationResult<int>> GetActivityAuditStatusAsync(string activityId)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.GetActivityAuditStatusAsync(activityId));
        }

        public static async Task<OperationResult<SelectActivityListModel>> SelectActivityListAsync(SalePromotionActivityModel model, int pageIndex, int pageSize)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.SelectActivityListAsync(model, pageIndex, pageSize));
        }

        public static async Task<OperationResult<List<SalePromotionActivityDiscount>>> GetActivityContentAsync(string activityId)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.GetActivityContentAsync(activityId));
        }

        public static async Task<OperationResult<SalePromotionActivityModel>> GetActivityInfoAsync(string activityId)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.GetActivityInfoAsync(activityId));
        }

        #endregion

        #region 活动商品

        public static async Task<OperationResult<bool>> InsertActivityProductListAsync(List<SalePromotionActivityProduct> productList, string activityId, string userName)
        {
            if (string.IsNullOrWhiteSpace(activityId))
            {
                return OperationResult.FromError<bool>("1", "活动id为空");
            }
            if (!(productList?.Count > 0))
            {
                return OperationResult.FromError<bool>("1", "新增商品为空");
            }
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.InsertActivityProductListAsync(productList, activityId, userName));
        }

        public static async Task<OperationResult<int>> SetProductLimitStockAsync(string activityId, List<string> pidList, int stock, string userName)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.SetProductLimitStockAsync(activityId, pidList, stock, userName));
        }

        public static async Task<OperationResult<int>> SetProductImageAsync(string activityId, List<string> pidList, string imgUrl, string userName)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.SetProductImageAsync(activityId, pidList, imgUrl, userName));
        }

        /// <summary>
        /// 批量设置打折详情页牛皮癣
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<SetDiscountProductDetailImgResponse>> SetDiscountProductDetailImgAsync(SetDiscountProductDetailImgRequest request)
        {
            var response = new SetDiscountProductDetailImgResponse()
            {
                ResponseCode = "00001",
                ResponseMessage = "操作失败"
            };
            int result = 0;
            int newAuditStatus = 0;//审核状态
            bool isAllPids = false;

            try
            {
                //获取活动原先审核状态: 初始状态的改为初始状态,其他状态则改为已提交状态
                int oldAuditStatus = await DalSalePromotionActivity.GetActivityAuditStatus(request.ActivityId);
                if (oldAuditStatus > 0)
                {
                    newAuditStatus = 1;
                }

                //没有指定pid就更新活动下所有商品的图片
                if (!(request.Pid?.Count > 0))
                {
                    isAllPids = true;
                }

                using (var dbHelper = DbHelper.CreateDbHelper())
                {
                    bool transResult = false;
                    try
                    {
                        dbHelper.BeginTransaction();

                        //1.设置图片
                        var updateImgResult = await DalSalePromotionActivity.BatchUpdateProductDetailImageAsync(dbHelper, request, isAllPids);

                        if (updateImgResult > 0)
                        {
                            result = updateImgResult;

                            //2.修改活动审核状态、下架状态字段
                            var updateStatusResult = await DalSalePromotionActivity.UpdateActivityAuditAndUnShelveStatusAsync(dbHelper,
                                                                request.ActivityId, newAuditStatus, request.Operator);

                            if (updateStatusResult > 0)
                            {
                                transResult = true;
                            }
                        }

                        if (transResult)
                        {
                            dbHelper.Commit();
                            response.ResponseCode = "0000";
                            response.ResponseMessage = "操作成功";

                            //设置读写库标识
                            await DalSalePromotionActivity.SetDBFlagCache(request.ActivityId);
                        }
                        else
                        {
                            dbHelper.Rollback();
                            Logger.Warn($"SetDiscountProductDetailImgAsync=>Rollback,{JsonConvert.SerializeObject(request)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        dbHelper.Rollback();
                        Logger.Error($"SetDiscountProductDetailImgAsync=>Rollback 异常,{JsonConvert.SerializeObject(request)},ex:{ex.InnerException}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"SetDiscountProductDetailImgAsync,{JsonConvert.SerializeObject(request)}", ex);
            }

            response.ResponseRow = result;
            return OperationResult.FromResult(response);
        }


        public static async Task<OperationResult<SalePromotionActivityModel>> GetActivityAndProductsAsync(string activityId, List<string> pidList)
        {
            var model = await DalSalePromotionActivity.GetActivityInfoAsync(activityId);
            if (model != null)
            {
                model.Products = (await DalSalePromotionActivity.GetProductInfoListAsync(activityId, pidList))?.ToList();
            }
            return OperationResult.FromResult(model);
        }

        public static async Task<OperationResult<int>> DeleteProductFromActivityAsync(string pid, string activityId, string userName)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.DeleteProductFromActivity(pid, activityId, userName));
        }

        public static async Task<OperationResult<int>> GetActivityProductCountAsync(string activityId)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.GetActivityProductCountAsync(activityId));
        }

        public static async Task<OperationResult<IEnumerable<SalePromotionActivityProduct>>> GetProductInfoListAsync(string activityId, List<string> pidList)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.GetProductInfoListAsync(activityId, pidList));
        }

        public static async Task<OperationResult<PagedModel<SalePromotionActivityProduct>>> SelectProductListAsync(SelectActivityProduct condition, int pageIndex, int pageSize)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.SelectProductListAsync(condition, pageIndex, pageSize));
        }

        public static async Task<OperationResult<IList<SalePromotionActivityProduct>>> GetRepeatProductListAsync(string activityId, List<string> pidList)
        {
            if (pidList == null || pidList.Count == 0)
            {
                return OperationResult.FromError<IList<SalePromotionActivityProduct>>("2", "pid集合为空");
            }
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.GetRepeatProductListAsync(activityId, pidList));
        }

        public static async Task<OperationResult<IList<SalePromotionActivityProduct>>> GetActivityRepeatProductListAsync(string activityId, string startTime, string endTime)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.GetActivityRepeatProductListAsync(activityId, startTime, endTime));
        }

        public static async Task<OperationResult<bool>> AddAndDelActivityProductAsync(string activityId, int stock, List<SalePromotionActivityProduct> addList, List<string> delList, string userName)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.AddAndDelActivityProductAsync(activityId, stock, addList, delList, userName));
        }

        public static async Task<OperationResult<bool>> RefreshProductAsync(string activityId, List<SalePromotionActivityProduct> productList)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivity.RefreshProductAsync(activityId, productList));
        }

        #endregion

        #region 审核权限

        /// <summary>
        /// 新增促销活动审核权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<int>> InsertAuditAuthAsync(SalePromotionAuditAuth model)
        {
            int result = await DalSalePromotionActivity.InsertAuditAuthAsync(model);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 删除促销活动审核权限
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public static async Task<OperationResult<int>> DeleteAuditAuthAsync(int PKID)
        {
            int result = await DalSalePromotionActivity.DeleteAuditAuthAsync(PKID);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 分页查询用户审核权限
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<OperationResult<PagedModel<SalePromotionAuditAuth>>> SelectAuditAuthListAsync(SalePromotionAuditAuth searchModel, int pageIndex, int pageSize)
        {
            var result = await DalSalePromotionActivity.SelectAuditAuthListAsync(searchModel, pageIndex, pageSize);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 根据类型和username获取用户审核权限信息
        /// </summary>
        /// <param name="promotionType"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static async Task<OperationResult<SalePromotionAuditAuth>> GetUserAuditAuthAsync(int promotionType, string userName)
        {
            var result = await DalSalePromotionActivity.GetUserAuditAuthAsync(promotionType, userName);
            return OperationResult.FromResult(result);
        }

        #endregion

    }
}

