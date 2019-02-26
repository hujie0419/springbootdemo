using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models;
using Tuhu.Service.Shop.Models.Region;

namespace Tuhu.Provisioning.Business.CommonServices
{
    public class ShopService
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(ShopService));
        /// <summary>
        /// 根据门店ShopId列表获取简略门店详细信息
        /// </summary>
        /// <param name="shopIds"></param>
        /// <returns></returns>
        public static IEnumerable<ShopSimpleDetailModel> SelectShopSimpleDetails(List<int> shopIds)
        {
            IEnumerable<ShopSimpleDetailModel> result = null;
            try
            {
                using (var client = new Tuhu.Service.Shop.ShopClient())
                {
                    var getResult = client.SelectShopSimpleDetails(shopIds);
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 获取所有省
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public static IEnumerable<SimpleRegion> GetAllProvince()
        {
            IEnumerable<SimpleRegion> result = null;
            try
            {
                using (var client = new RegionClient())
                {
                    var getResult = client.GetAllProvince();
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 根据regionId获取region
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public static Region GetRegionByRegionId(int regionId)
        {
            Region result = null;
            try
            {
                using (var client = new RegionClient())
                {
                    var getResult = client.GetRegionByRegionId(regionId);
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
    }
}
