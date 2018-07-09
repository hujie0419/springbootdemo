using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class OrderAreaManager
    {
        #region Private Fields
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog Logger = LoggerFactory.GetLogger("OrderAreaManager");
        private OrderAreaHandler handler = null;
        #endregion 

        public OrderAreaManager()
        {
            handler = new OrderAreaHandler(DbScopeManager);
        }

        /// <summary>
        /// 根据ID获取配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<OrderArea> GetOrderAreaById(int id)
        {
            try
            {
                return handler.GetOrderAreaById(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new OrderAreaException(1, "GetOrderAreaById", ex);
                Logger.Log(Level.Error, exception, "GetOrderAreaById");
                throw ex;
            }

        }

        /// <summary>
        /// 根据parentid获取配置信息
        /// </summary>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public List<OrderArea> GetOrderAreaByParentId(int parentid)
        {
            try
            {
                return handler.GetOrderAreaByParentId(parentid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new OrderAreaException(1, "GetOrderAreaByParentId", ex);
                Logger.Log(Level.Error, exception, "GetOrderAreaByParentId");
                throw ex;
            }

        }

        /// <summary>
        /// 获取全部配置信息
        /// </summary>
        /// <returns></returns>
        public List<OrderArea> GetALLOrderArea()
        {
            try
            {
                return handler.GetALLOrderArea();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new OrderAreaException(1, "GetALLOrderArea", ex);
                Logger.Log(Level.Error, exception, "GetALLOrderArea");
                throw ex;
            }

        }

        /// <summary>
        /// 修改大渠道
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateOrderArea(OrderArea model)
        {
            try
            {
                return handler.UpdateOrderArea(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new OrderAreaException(1, "UpdateOrderArea", ex);
                Logger.Log(Level.Error, exception, "UpdateOrderArea");
                throw ex;
            }

        }

        /// <summary>
        /// 添加大渠道
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddOrderArea(OrderArea model)
        {
            try
            {
                return handler.AddOrderArea(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new OrderAreaException(1, "AddOrderArea", ex);
                Logger.Log(Level.Error, exception, "AddOrderArea");
                throw ex;
            }

        }

        /// <summary>
        /// 添加区域
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddChidAreaData(OrderArea model)
        {
            try
            {
                return handler.AddChidAreaData(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new OrderAreaException(1, "AddChidAreaData", ex);
                Logger.Log(Level.Error, exception, "AddChidAreaData");
                throw ex;
            }

        }

        /// <summary>
        /// 修改区域
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateChidAreaData(OrderArea model)
        {
            try
            {
                return handler.UpdateChidAreaData(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new OrderAreaException(1, "UpdateChidAreaData", ex);
                Logger.Log(Level.Error, exception, "UpdateChidAreaData");
                throw ex;
            }

        }
    }
}