using BaoYangRefreshCacheService.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu;
namespace Tuhu.C.Job.DAL
{
    public class BeautyGroupBuyingDal
    {
        private static BeautyGroupBuyingDal _Instanse;
        private BeautyGroupBuyingDal() { }
        public static BeautyGroupBuyingDal Instanse
        {
            get
            {
                if (_Instanse == null)
                {
                    _Instanse = new BeautyGroupBuyingDal();
                }
                return _Instanse;
            }
        }
        public async Task<IEnumerable<ShopModel>> GetAllShopIdsAsync()
        {
            const string sql = @"SELECT  PKID
  FROM [Gungnir].[dbo].[Shops] WITH(NOLOCK) 
  WHERE IsActive=1  
  ORDER BY PKID ";

            using (var cmd = new SqlCommand(sql))
            {
                var result = await DbHelper.ExecuteSelectAsync<ShopModel>(true,cmd);
                return result;
            }
        }




    }
}
