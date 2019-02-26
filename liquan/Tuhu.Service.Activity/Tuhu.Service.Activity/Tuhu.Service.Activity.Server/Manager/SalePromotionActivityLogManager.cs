using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.Server.Manager
{
    public static class SalePromotionActivityLogManager
    {

        #region 查询

        public static async Task<OperationResult<PagedModel<SalePromotionActivityLogModel>>> GetOperationLogListAsync(string referId, int pageIndex, int pageSize)
        {
            var pager = await DalSalePromotionActivityLog.GetOperationLogListAsync(referId, pageIndex, pageSize);
            var source = pager?.Source?.ToList();
            if (source?.Count() > 0)
            {
                //详情数量
                var PKIDs = source.Select(a => a.PKID);
                var detailCounts = await DalSalePromotionActivityLog.GetDetailCountListAsync(PKIDs.ToList());
                if (detailCounts?.Count() > 0)
                {
                    for (int i = 0; i < source.Count; i++)
                    {
                        var model = detailCounts.FirstOrDefault(a => a.PKID == source[i].PKID);
                        if (model?.DetailCount > 0)
                        {
                            source[i].DetailCount = model.DetailCount;
                        }
                    }
                }
                pager.Source = source;
            }
            return OperationResult.FromResult(pager);
        }

        public static async Task<OperationResult<IEnumerable<SalePromotionActivityLogDetail>>> GetOperationLogDetailListAsync(string FPKID)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivityLog.GetOperationLogDetailListAsync(FPKID));
        }

        #endregion

        #region 操作

        /// <summary>
        /// 新增操作日志描述
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> InsertActivityLogDescriptionAsync(SalePromotionActivityLogDescription model)
        {
            OperationResult<bool> result;
            bool isRepeat = await DalSalePromotionActivityLog.CheckLogTypeIsNoRepeatAsync(model.OperationLogType);
            if (!isRepeat)
            {
                return OperationResult.FromError<bool>("1", $"操作日志类型【{model.OperationLogType}】已存在");
            }
            else
            {
                var insertResult = await DalSalePromotionActivityLog.InsertActivityLogDescriptionAsync(model);
                if (insertResult != 1)
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

        public static async Task<OperationResult<bool>> InsertAcitivityLogAndDetailAsync(SalePromotionActivityLogModel model)
        {
            return await OperationResult.FromResultAsync(DalSalePromotionActivityLog.InsertAcitivityLogAndDetailAsync(model));
        }

        #endregion
    }
}

