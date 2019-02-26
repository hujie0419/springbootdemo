using System;
using System.Collections.Generic;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;


namespace Tuhu.Provisioning.Business.ThirdPartyExchangeCode
{
    public class ThirdPartyExchangeCodeManage

    {
        private static ILog Logger = LoggerFactory.GetLogger("ThirdPartyExchangeCode");

        /// <summary>
        /// 搜索兑换码批次信息
        /// </summary>
        /// <returns></returns>
        public static List<ThirdPartyCodeBatch> SelectBatches(SerchElement serchElement)
        {
            try
            {
                return DalThirdPartyExchangeCode.SelectBatches(serchElement);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyExchangerCodeException(1, "SelectBatches", ex);
                Logger.Log(Level.Error, exception, "SelectBatches");
                throw ex;
            }
        }

        /// <summary>
        /// 增加兑换码批次
        /// </summary>
        /// <param name="codeBatch"></param>
        public static int InserBatches(ThirdPartyCodeBatch codeBatch)
        {
            try
            {
                return DalThirdPartyExchangeCode.InserBatches(codeBatch);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyExchangerCodeException(1, "InserBatches", ex);
                Logger.Log(Level.Error, exception, "InserBatches");
                throw ex;
            }
        }

        /// <summary>
        /// 编辑兑换码批次记录
        /// </summary>
        /// <param name="codeBatch"></param>
        /// <returns></returns>
        public static int UpdateBatches(ThirdPartyCodeBatch codeBatch)
        {
            try
            {
                return DalThirdPartyExchangeCode.UpdateBatches(codeBatch);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyExchangerCodeException(1, "UpdateBatches", ex);
                Logger.Log(Level.Error, exception, "UpdateBatches");
                throw ex;
            }
        }

        public static ThirdPartyCodeBatch SelectBatch(Guid batchId)
        {
            try
            {
                return DalThirdPartyExchangeCode.SelectBatch(batchId);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyExchangerCodeException(1, "SelectBatch", ex);
                Logger.Log(Level.Error, exception, "SelectBatch");
                throw ex;
            }
        }

        /// <summary>
        /// 批次导入兑换码
        /// </summary>
        /// <param name="exchangeCode"></param>
        /// <returns></returns>
        public static int InsertExchangeCode(DataAccess.Entity.ThirdPartyExchangeCode exchangeCode)
        {
            try
            {
                return DalThirdPartyExchangeCode.InsertExchangeCode(exchangeCode);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyExchangerCodeException(1, "InsertExchangeCode", ex);
                Logger.Log(Level.Error, exception, "InsertExchangeCode");
                throw ex;
            }
        }

        /// <summary>
        /// 搜索兑换码批次信息
        /// </summary>
        /// <returns></returns>
        public static List<Tuhu.Provisioning.DataAccess.Entity.ThirdPartyExchangeCode> SelectExchangCode(SerchCodeElement serchElement)
        {
            try
            {
                return DalThirdPartyExchangeCode.SelectExchangCode(serchElement);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyExchangerCodeException(1, "SelectExchangCode", ex);
                Logger.Log(Level.Error, exception, "SelectExchangCode");
                throw ex;
            }
        }
        /// <summary>
        ///更新库存
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="batchQty"></param>
        /// <param name="stockQty"></param>
        /// <returns></returns>
        public static int UpdateQty(int pkid, int batchQty, int stockQty)
        {
            try
            {
                return DalThirdPartyExchangeCode.UpdateQty(pkid, batchQty, stockQty);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyExchangerCodeException(1, "UpdateQty", ex);
                Logger.Log(Level.Error, exception, "UpdateQty");
                throw ex;
            }
        }
        /// <summary>
        /// 查询总行数
        /// </summary>
        /// <returns></returns>
        public static int SelectCount(SerchElement serchElement)
        {
            try
            {
                return DalThirdPartyExchangeCode.SelectCount(serchElement);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyExchangerCodeException(1, "SelectCount", ex);
                Logger.Log(Level.Error, exception, "SelectCount");
                throw ex;
            }

        }

    
    }
}
