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
    public class SalePromotionActivityLogService : ISalePromotionActivityLogService
    {

        #region 查询
        public async Task<OperationResult<PagedModel<SalePromotionActivityLogModel>>> GetOperationLogListAsync(string referId, int pageIndex, int pageSize)
        {
            return await SalePromotionActivityLogManager.GetOperationLogListAsync(referId, pageIndex, pageSize);
        }
        public async Task<OperationResult<IEnumerable<SalePromotionActivityLogDetail>>> GetOperationLogDetailListAsync(string FPKID)
        {
            return await SalePromotionActivityLogManager.GetOperationLogDetailListAsync(FPKID);
        }
        #endregion

        #region 操作

        public async Task<OperationResult<bool>> InsertActivityLogDescriptionAsync(SalePromotionActivityLogDescription model)
        {
            return await SalePromotionActivityLogManager.InsertActivityLogDescriptionAsync(model);
        }

        public async Task<OperationResult<bool>> InsertAcitivityLogAndDetailAsync(SalePromotionActivityLogModel model)
        {
            return await SalePromotionActivityLogManager.InsertAcitivityLogAndDetailAsync(model);
        }

        #endregion


    }
}
