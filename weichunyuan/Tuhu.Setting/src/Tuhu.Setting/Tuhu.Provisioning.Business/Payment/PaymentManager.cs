using System;
using System.Configuration;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Payment
{
    public class PaymentManager
    {
        #region Private Fields

        private static readonly IConnectionManager ConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(ConnectionManager);

        private readonly PaymentHandler handler;

        private static readonly ILog Logger = LoggerFactory.GetLogger("Payment");

        #endregion

        public PaymentManager()
        {
            handler = new PaymentHandler(DbScopeManager, ConnectionManager);
        }

        public dynamic GetPayment_way()
        {

            try
            {
                return handler.GetPayment_way();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetPayment_way");
                throw ex;
            }
        }
        public int UpdateState(int state)
        {
            try
            {
                return handler.UpdateState(state);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "UpdateState");
                throw ex;
            }
        }
    }
}
