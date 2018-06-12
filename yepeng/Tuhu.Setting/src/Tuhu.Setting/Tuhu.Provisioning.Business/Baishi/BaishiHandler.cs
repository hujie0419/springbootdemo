using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Baishi
{
    public class BaishiHandler
    {

        private readonly IDBScopeManager dbManager;

        private const string BaishiPNPrefix = "PN";

        private static readonly ILog logger = LoggerFactory.GetLogger("BaishiHandler");

        public BaishiHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }


        public List<ExternalShop> SelectExternalShops()
        {
            return dbManager.Execute(connection => DalExternalShop.SelectExternalShopsByShopType(connection, ExternalShopType.BaoYang));
        }

    }
}
