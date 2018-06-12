using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models;
using Tuhu.Service.Shop.Models.Result;

namespace Tuhu.C.Job.ShopSyncJob.BLL
{
    public class ShopBusiness
    {
        public static List<string> tuangouxicheTypes = new List<string>() { "FU-MD-QCJX-F|1", "FU-MD-QCJX-F|2", "FU-MD-BZXC-F|1", "FU-MD-BZXC-F|2" };


        public static List<ShopDetailModel> GetShopsForBaoyangAndXiChe()
        {
            List<ShopDetailModel> shopDetailList = new List<ShopDetailModel>();
            List<int> specificregionIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ShopRegionIdList"]))
            {
                specificregionIdList = ConfigurationManager.AppSettings["ShopRegionIdList"].Split(new char[] { ',' }).Select(q => Convert.ToInt32(q)).ToList();
            }

            try
            {
                if (specificregionIdList.Any())
                {
                    using (var client = new ShopClient())
                    {

                        foreach (var regionId in specificregionIdList)
                        {
                            //refresh shops in regions
                             client.UpdateShopDetailsByRegionId(regionId);
                            var result = client.SelectShopDetailsByRegionId(regionId);
                            result.ThrowIfException();
                            if (result.Success && result.Result != null && result.Result.Any())
                            {
                                shopDetailList.AddRange(result.Result);
                            }
                        }
                        
                    }

                   
                    shopDetailList = shopDetailList.Where(q => (q.ServiceType & 4) == 4 || (q.ServiceType & 2) == 2).ToList();
                    shopDetailList = shopDetailList.Where(q => q.SuspendStartDateTime == null || q.SuspendEndDateTime == null || (DateTime.Now < q.SuspendStartDateTime) || DateTime.Now > q.SuspendEndDateTime.Value.AddDays(1)).ToList();
                }
            }
            catch (Exception ex)
            {
                TuhuShopSyncJob.Logger.Error(ex);
            }
            return shopDetailList;
        }

        public static List<ShopDetailModel> GetShopsByShopIds()
        {
            List<ShopDetailModel> shopDetailList = new List<ShopDetailModel>();
            List<int> specificshopIdList = new List<int>();
            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ShopIdList"]))
            {
                specificshopIdList = ConfigurationManager.AppSettings["ShopIdList"].Split(new char[] { ',' }).Select(q => Convert.ToInt32(q)).ToList();
            }

            try
            {
                if (specificshopIdList.Any())
                {
                    using (var client = new ShopClient())
                    {
                       
                        var result = client.SelectShopDetails(specificshopIdList);
                        result.ThrowIfException();
                        if (result.Success && result.Result != null && result.Result.Any())
                        {
                            shopDetailList.AddRange(result.Result);
                        }

                    }
                    shopDetailList = shopDetailList.Where(q => (q.ServiceType & 4) == 4 || (q.ServiceType & 2) == 2).ToList();
                    shopDetailList = shopDetailList.Where(q => q.SuspendStartDateTime == null || q.SuspendEndDateTime == null || (DateTime.Now < q.SuspendStartDateTime) || DateTime.Now > q.SuspendEndDateTime.Value.AddDays(1)).ToList();
                }
            }
            catch (Exception ex)
            {
                TuhuShopSyncJob.Logger.Error(ex);
            }
            return shopDetailList;
        }


        public static List<ShopServiceModel> GetShopServiceModelListByShopId(int shopId)
        {
            List<ShopServiceModel> shopservicemodelList = new List<ShopServiceModel>();
            try
            {
                using (var client = new ShopClient())
                {
                    client.UpdateShop(shopId);
                    var shopdetail = client.FetchShopDetail(shopId);
                    if (shopdetail != null && shopdetail.Result != null && shopdetail.Result.ShopServices != null && shopdetail.Result.ShopServices.Any())
                    {
                        shopservicemodelList = shopdetail.Result.ShopServices.Where(
                       q => q.ServersType.Equals("CarWashing", StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                }
            }
            catch (Exception ex)
            {

                TuhuShopSyncJob.Logger.Error(ex); ;
            }
            return shopservicemodelList;
        }

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

        public static int GetTuanGouXiCheType(string typeName)
        {
            int type = 4;
            if (!string.IsNullOrWhiteSpace(typeName))
            {
                if (typeName.Equals("FU-MD-BZXC-F|2", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = 5;
                }
                else if (typeName.Equals("FU-MD-QCJX-F|1", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = 6;
                }
                else if (typeName.Equals("FU-MD-QCJX-F|2", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = 7;
                }
            }

            return type;
        }

        public static List<ShopServiceModel> GetXBYShopServiceModelListByShopId(int shopId)
        {
            List<ShopServiceModel> shopservicemodelList = new List<ShopServiceModel>();
            try
            {
                using (var client = new ShopClient())
                {
                    var shopdetail = client.FetchShopDetail(shopId);
                    if (shopdetail != null && shopdetail.Result != null && shopdetail.Result.ShopServices != null && shopdetail.Result.ShopServices.Any())
                    {
                        shopservicemodelList = shopdetail.Result.ShopServices.Where(
                       q => q.ServiceId.Contains("FU-BY-XBY|")).ToList();
                    }

                }
            }
            catch (Exception ex)
            {

                TuhuShopSyncJob.Logger.Error(ex); ;
            }
            return shopservicemodelList;
        }
    }
}
