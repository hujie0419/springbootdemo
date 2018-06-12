using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models.Result;

namespace Tuhu.C.Job.CheckAlipayShopJob.BLL
{
    public class ShopBusiness
    {

        public static List<string> tuangouxicheTypes = new List<string>() { "FU-MD-QCJX-F|1", "FU-MD-QCJX-F|2", "FU-MD-BZXC-F|1", "FU-MD-BZXC-F|2" };

        public static List<ShopBeautyProductResultModel> GetTuanGouXiCheModel(int shopid)
        {
            List<ShopBeautyProductResultModel> TGXiCheList = new List<ShopBeautyProductResultModel>();
            using (var client = new ShopClient())
            {
                var result = client.GetShopBeautyDetailResultModel(shopid, null);
                if (result != null && result.Result != null)
                {
                    var xicheitem = result.Result.Where(q => q.CategoryId == 1).FirstOrDefault();
                    if (xicheitem != null && xicheitem.Products != null)
                    {
                        TGXiCheList = xicheitem.Products.Where(q => tuangouxicheTypes.Contains(q.PID)).ToList();
                    }
                }
            }
            return TGXiCheList;
        }
    }
}
