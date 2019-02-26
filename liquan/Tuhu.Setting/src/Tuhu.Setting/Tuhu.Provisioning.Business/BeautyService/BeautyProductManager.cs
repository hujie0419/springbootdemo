using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Provisioning.DataAccess.DAO.BeautyServicePackageDal;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;
using Tuhu.Provisioning.DataAccess.Entity.CommonEnum;

namespace Tuhu.Provisioning.Business.BeautyService
{
    public class BeautyProductManager
    {
        private static readonly string CacheClientName = "settting";

        private static readonly ILog Logger = LogManager.GetLogger(typeof(BeautyProductManager));
        /// <summary>
        /// 根据pid获取美容产品信息
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static BeautyProductModel GetBeautyProductByPid(string pid)
        {
            using (var client = CacheHelper.CreateCacheClient(CacheClientName))
            {
                string key = $"GetBeautyProductByPid/{pid}";
                var result = client.GetOrSet(key, () => BeautyServicePackageDal.SelectBeautyProductByPid(pid), TimeSpan.FromMinutes(20));
                if (!result.Success)
                {
                    return BeautyServicePackageDal.SelectBeautyProductByPid(pid);
                }
                else
                {
                    return result.Value;
                }
            }
        }
        /// <summary>
        /// 获取当前分类的子分类（包含自身）
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetBeautyChildAndSelfCategoryIdsByCategoryId(int categoryId)
        {
            using (var client = CacheHelper.CreateCacheClient(CacheClientName))
            {
                string key = $"GetBeautyChildCategoryIdsByCategoryId/{categoryId}";
                var result = client.GetOrSet(key, () => BeautyServicePackageDal.SelectBeautyChildAndSelfCategoryIdsByCategoryId(categoryId), TimeSpan.FromMinutes(20));
                if (!result.Success)
                {
                    return BeautyServicePackageDal.SelectBeautyChildAndSelfCategoryIdsByCategoryId(categoryId);
                }
                else
                {
                    return result.Value;
                }
            }
        }

        /// <summary>
        /// 根据车型类型获取车型描述
        /// </summary>
        /// <param name="vehicleType"></param>
        /// <returns></returns>
        public static string GetVehicleTypeDescription(MrVehicleType vehicleType)
        {
            string result = string.Empty;
            switch (vehicleType)
            {
                case MrVehicleType.FiveSeat:
                    result = "五座轿车";
                    break;
                case MrVehicleType.Suv:
                    result = "SUV";
                    break;
                case MrVehicleType.Mpv:
                    result = "MPV";
                    break;
                case MrVehicleType.SuvOrMpv:
                    result = "SUV/MPV";
                    break;
                case MrVehicleType.None:
                    result = "五座轿车 SUV MPV";
                    break;

            }
            return result;
        }
    }
}
