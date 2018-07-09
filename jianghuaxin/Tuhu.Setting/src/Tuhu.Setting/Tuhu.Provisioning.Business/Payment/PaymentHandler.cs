using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business.Payment
{
    public class PaymentHandler
    {
        #region Private Fields

        private readonly IConnectionManager connectionManager;
        private readonly IDBScopeManager dbManager;
        #endregion

        #region Ctor
        public PaymentHandler(IDBScopeManager dbManager, IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
            this.dbManager = dbManager;
        }
        #endregion


        public  dynamic GetPayment_way()
        {
            return DALPayment.GetPayment_way();
        }

        public int UpdateState(int state)
        {
            return DALPayment.UpdateState(state);
        }
    }
}
