using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class FlashSaleProductsExtendManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("FlashSaleProductsExtend");

        public List<FlashSaleProductsExtend> GetFlashSaleProductsExtendList(string sqlStr, int pageSize, int pageIndex, string orderBy, out int recordCount)
        {
            try
            {
                return DALFlashSaleProductsExtend.GetFlashSaleProductsExtendList(sqlStr, pageSize, pageIndex, orderBy, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new FlashSaleProductsExtendException(1, "GetFlashSaleProductsExtendList", ex);
                Logger.Log(Level.Error, exception, "GetFlashSaleProductsExtendList");
                throw ex;
            }
        }

        public bool UpdateFlashSaleProductsIsUsePCode(FlashSaleProductsExtend model)
        {
            try
            {
                return DALFlashSaleProductsExtend.UpdateFlashSaleProductsIsUsePCode(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new FlashSaleProductsExtendException(1, "UpdateFlashSaleProductsIsUsePCode", ex);
                Logger.Log(Level.Error, exception, "UpdateFlashSaleProductsIsUsePCode");
                throw ex;
            }
        }
    }
}
