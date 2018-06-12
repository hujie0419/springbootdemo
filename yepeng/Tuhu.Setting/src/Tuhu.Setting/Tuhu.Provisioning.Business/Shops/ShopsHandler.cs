using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ShopsManagement
{
    internal class ShopsHandler
    {
        #region Private Fields

        private readonly IDBScopeManager dbManager;
        internal ShopsHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        #endregion

        public string GetSimpleNameByShopID(int ShopID)
        {           
            return dbManager.Execute(connection => DalShop.GetSimpleNameByShopID(connection, ShopID));
        }
        public List<ShopAppBanner> GetShopAppBannerList()
        {
            return dbManager.Execute(connection => DalShop.GetShopAppBannerList(connection));
        }

        public ShopAppBanner GetAppShopBannerByPKID(int _PKID)
        {
            return dbManager.Execute(connection => DalShop.GetAppShopBannerByPKID(connection, _PKID));
        }

        public int DeleteShopBannerByPKID(int PKID)
        {
            return dbManager.Execute(connection => DalShop.DeleteShopBannerByPKID(connection, PKID));
        }

        public int UpdateShopBanner(ShopAppBanner banner)
        {
            return dbManager.Execute(connection => DalShop.UpdateShopBanner(connection, banner));
        }

        public int InsertShopBanner(ShopAppBanner banner)
        {
            return dbManager.Execute(connection => DalShop.InsertShopBanner(connection, banner));
        }

    }
}