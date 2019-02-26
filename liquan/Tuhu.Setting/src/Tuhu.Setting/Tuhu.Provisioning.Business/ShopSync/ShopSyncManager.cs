using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Log;
using Tuhu.Provisioning.DataAccess.DAO.ShopSync;
using Tuhu.Provisioning.DataAccess.Entity.ShopSync;
using Tuhu.Service.ThirdParty;
using Tuhu.Service.ThirdParty.Models.ShopSync;

namespace Tuhu.Provisioning.Business.ShopSync
{
    public class ShopSyncManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("ShopSyncManager");
        /// <summary>
        /// 分页查询门店同步记录
        /// </summary>
        /// <param name="thirdParty"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="syncStatus"></param>
        /// <param name="shopId"></param>
        /// <param name="simpleName"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public Tuple<List<ThirdPartyShopSyncRecord>, int> SelectThirdPartyShopSyncRecord(string thirdParty, int pageIndex, int pageSize, 
            string syncStatus, int shopId, string simpleName, string fullName)
        {
            Tuple<List<ThirdPartyShopSyncRecord>, int> result =null;
            try
            {
                result = ShopSyncDAL.SelectThirdPartyShopSyncRecord(thirdParty, pageIndex, pageSize, syncStatus, shopId, simpleName, fullName);
            }
            catch (Exception ex)
            {
                Logger.Log(Tuhu.Component.Log.Level.Error, ex, nameof(SelectThirdPartyShopSyncRecord), null);
            }
            return result;
        }
        /// <summary>
        /// 查询所有的第三方名
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> SelectShopSyncThirdPartyies()
        {
            var result = new List<string>();
            try
            {
                result = ShopSyncDAL.SelectShopSyncThirdParties();
            }
            catch (Exception ex)
            {
                Logger.Log(Tuhu.Component.Log.Level.Error, ex, nameof(SelectThirdPartyShopSyncRecord), null);
            }
            return result;
        }
        /// <summary>
        /// 门店同步
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="thirdParty"></param>
        /// <returns></returns>
        public bool SyncShop(int shopId, string thirdParty)
        {
            var result = false;
            try
            {
                using (var client = new ShopSyncClient())
                {
                    var serviceResult = client.SyncShop(new ShopSyncRequest()
                    {
                        ShopId = shopId,
                        ThirdParty = thirdParty
                    });
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result?.IsSuccess ?? false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Tuhu.Component.Log.Level.Error, ex, nameof(SyncShop), null);
            }

            return result;
        }
    }
}
