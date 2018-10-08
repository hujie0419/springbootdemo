using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.GroupBuying;
namespace Tuhu.C.Job.BLL
{
    public class BeautyGroupBuyingBLL
    {
        private BeautyGroupBuyingBLL() { }
        private static BeautyGroupBuyingBLL _Instanse;
        public static BeautyGroupBuyingBLL Instanse
        {
            get
            {
                if (_Instanse == null)
                {
                    _Instanse = new BeautyGroupBuyingBLL();
                }
                return _Instanse;
            }
        }
        public async Task<bool> UpdateSoldInfoStatictisAsync(IEnumerable<int> shopIds)
        {
            using (var client = new GroupBuyingClient())
            {
                var result = await client.UpdateSoldInfoStatictisAsync(shopIds);
                result.ThrowIfException(true);
                return result.Result;
            }
        }






    }
}
