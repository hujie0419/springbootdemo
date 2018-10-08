using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.ShopSyncJob.DAL
{
    public class ShopSqlText
    {

        public static string SqlTextAllThirdPartyShopIds = "SELECT	PKID FROM ThirdPartySyncShops WITH(NOLOCK)";
        public static string SqlTextValidThirdPartyShopIds = "SELECT	PKID FROM ThirdPartySyncShops WITH(NOLOCK) WHERE IsDeleted=0";

        public static string SqlTextDeleteThirdPartyShops = "Delete FROM ThirdPartySyncShops(NOLOCK)";
    }
}
