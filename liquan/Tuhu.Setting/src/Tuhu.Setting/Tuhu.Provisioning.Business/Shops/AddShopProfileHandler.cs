using ThBiz.Business.ShopsManagement.Actions;
using ThBiz.DataAccess.Entity;

using Tuhu.Component.Framework;

namespace ThBiz.Business.ShopsManagement
{
    internal class AddShopProfileHandler
    {
        #region Private Fields

        private readonly IAddShopProfileAction addShopProfileAction;

        #endregion

        #region Ctor

        internal AddShopProfileHandler(IAddShopProfileAction addShopProfileAction)
        {
            this.addShopProfileAction = addShopProfileAction;
        }

        internal AddShopProfileHandler(IConnectionManager connectionManager)
        {
            this.addShopProfileAction = new AddShopProfileAction(connectionManager);
        }

        #endregion

        #region Public Methods

        public void AddShopProfile(UserObjects shopProfile)
        {
            this.addShopProfileAction.AddShopProfile(shopProfile);
        }

        #endregion
    }
}
