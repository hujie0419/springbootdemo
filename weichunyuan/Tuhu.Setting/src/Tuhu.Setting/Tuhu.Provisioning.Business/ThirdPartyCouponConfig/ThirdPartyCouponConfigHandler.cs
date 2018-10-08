using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ThirdPartyCouponConfig
{
    internal class ThirdPartyCouponConfigHandler
    {
        #region Private Fields

        private readonly IDBScopeManager dbManager;
        internal ThirdPartyCouponConfigHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        #endregion

        public int UpdateThirdPartyCouponConfig(ThirdPartyCouponConfigModel model)
        {
            return dbManager.Execute(connection => DalThirdPartyCouponConfig.UpdateThirdPartyCouponConfig(connection, model));
        }

        public int InsertThirdPartyCouponConfig(ThirdPartyCouponConfigModel model)
        {
            return dbManager.Execute(connection => DalThirdPartyCouponConfig.InsertThirdPartyCouponConfig(connection, model));
        }
    }
}
