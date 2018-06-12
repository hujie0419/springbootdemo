using System;
using System.Collections.Generic;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;


namespace Tuhu.Provisioning.Business.ThirdPartyMallConfig
{
    public class ThirdPartyMallConfigManage

    {
        private static ILog Logger = LoggerFactory.GetLogger("ThirdPartyExchangeCode");
        /// <summary>
        /// 搜索三方商城记录
        /// </summary>
        /// <returns></returns>
        public static List<ThirdPartyMallModel> SelectThirdMall(SerchThirdPartyMallModel serchMall)
        {
            try
            {
                return DalThirdPartyMall.SelectThirdMall(serchMall);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyMallConfigException(1, "SelectThirdMall", ex);
                Logger.Log(Level.Error, exception, "SelectThirdMall");
                throw ex;
            }
        }

        /// <summary>
        /// 增加三方商城记录
        /// </summary>
        /// <param name="codeBatch"></param>
        public static int InserThirdMall(ThirdPartyMallModel thirdMall)
        {
            try
            {
                return DalThirdPartyMall.InserThirdMall(thirdMall);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyMallConfigException(1, "InserThirdMall", ex);
                Logger.Log(Level.Error, exception, "InserThirdMall");
                throw ex;
            }
        }

        /// <summary>
        /// 编辑三方商城记录
        /// </summary>
        /// <param name="codeBatch"></param>
        /// <returns></returns>
        public static int EditThirdMall(ThirdPartyMallModel thirdMall)
        {
            try
            {
                return DalThirdPartyMall.EditThirdMall(thirdMall);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyMallConfigException(1, "EditThirdMall", ex);
                Logger.Log(Level.Error, exception, "EditThirdMall");
                throw ex;
            }
        }
        /// <summary>
        /// 根据PKID查询具体的信息
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public static ThirdPartyMallModel SelectThirdMall(int pkid)
        {
            try
            {
                return DalThirdPartyMall.SelectThirdMall(pkid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyMallConfigException(1, "SelectThirdMall", ex);
                Logger.Log(Level.Error, exception, "SelectThirdMall");
                throw ex;
            }
        }
        /// <summary>
        /// 根据总行数
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public static int SelectCout(SerchThirdPartyMallModel serchMall)
        {
            try
            {
                return DalThirdPartyMall.SelectCout(serchMall);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyMallConfigException(1, "SelectCout", ex);
                Logger.Log(Level.Error, exception, "SelectCout");
                throw ex;
            }
        }

        /// <summary>
        /// 根据batchId查询批次信息
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
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
                var exception = new ThirdPartyMallConfigException(1, "SelectBatch", ex);
                Logger.Log(Level.Error, exception, "SelectBatch");
                throw ex;
            }
        }
        /// <summary>
        /// 判断是否有重复顺序
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public static int SortChange(int sort)
        {
            try
            {
                return DalThirdPartyExchangeCode.SortChange(sort);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyMallConfigException(1, "SortChange", ex);
                Logger.Log(Level.Error, exception, "SortChange");
                throw ex;
            }
        }

    }
}
