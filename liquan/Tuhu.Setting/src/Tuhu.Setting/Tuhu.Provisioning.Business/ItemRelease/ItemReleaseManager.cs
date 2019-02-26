using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Nosql;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ItemRelease
{
    public class ItemReleaseManager
    {
        public static IEnumerable<ReleaseItemModel> SelectList(ReleaseItemModel model, PagerModel pager) => DalItemRelease.SelectList(model, pager);

        public static IEnumerable<TuhuReleaseModel> SelectList() => DalItemRelease.SelectList();


        public static IEnumerable<TuhuReleaseModel> SelectTuhuReleaseModelByCache()
        {
            using (var cache = CacheHelper.CreateCacheClient("SelectTuhuReleaseModel_ByCache"))
            {
                var cacheResult =
            cache.GetOrSet("SelectTuhuReleaseModel_ByCache:", () => SelectList(), TimeSpan.FromDays(1));
                return cacheResult?.Value;
            }

        }



        public static bool AddReleaseItem(ReleaseItemModel model)
        {
            return DalItemRelease.AddReleaseItem(model);
        }


        public static int DeleteReleaseItem(int PKID) => DalItemRelease.DeleteReleaseItem(PKID);


        public static bool UpdateReleaseItem(ReleaseItemModel model) => DalItemRelease.UpdateReleaseItemModel(model);

        public static ReleaseItemModel FetchReleaseItem(long pkid) => DalItemRelease.FetchReleaseItemModel(pkid);
    }
}
