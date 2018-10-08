using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Nosql;

namespace Tuhu.Provisioning.Business.ThirdParty
{
   public class AliPayBaoYangManage
    {

        public static List<string> GetFailedkeyByRemoveCacheKey(List<string> cacheKeys)
        {
            List<string> failedkeys = new List<string>();
            using (var client = CacheHelper.CreateCacheClient())
            {
                foreach (string cacheKey in cacheKeys.Where(p => !string.IsNullOrWhiteSpace(p)))
                {

                    var result = client.Remove(cacheKey)?.Success ?? false;

                }

            }
            return failedkeys.Distinct().ToList();
        }
        public static int DeleteAliPayBaoYangItem(int pkid)
        {
            return DALAliPayBaoYang.DeleteAliPayBaoYangItem(pkid);
        }

        public static int SaveAliPayBaoYangItem(AliPayBaoYangItem model)
        {
            return DALAliPayBaoYang.SaveAliPayBaoYangItem(model);
        }

        public static int UpdateAliPayBaoYangItem(AliPayBaoYangItem model)
        {
            return DALAliPayBaoYang.UpdateAliPayBaoYangItem(model);
        }

        public static List<AliPayBaoYangItem> GetAliPayBaoYangItem(int pkid)
        {
            return DALAliPayBaoYang.GetAliPayBaoYangItem(pkid);
        }

        public static AliPayBaoYangActivity GetAliPayBaoYangActivity()
        {
            return DALAliPayBaoYang.GetAliPayBaoYangActivity();
        }

        public static int UpdateAliPayBaoYangActivity(AliPayBaoYangActivity model)
        {
            return DALAliPayBaoYang.UpdateAliPayBaoYangActivity(model);
        }
        public static List<string> GetModelIdList()
        {
            return DALAliPayBaoYang.GetModelIdList();
        }
    }
}
