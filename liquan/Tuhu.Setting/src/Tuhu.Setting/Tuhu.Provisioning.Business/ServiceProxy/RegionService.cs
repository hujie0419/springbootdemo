using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models.Enum;
using Tuhu.Service.Shop.Models.Region;

namespace Tuhu.Provisioning.Business.ServiceProxy
{
    public class RegionService
    {
        private readonly ILog _logger;
        private const string CacheClientName = "setting";

        public RegionService()
        {
            _logger = LogManager.GetLogger<RegionService>();
        }

        /// <summary>
        /// 从缓存中获取所有三级区域
        /// </summary>
        /// <returns></returns>
        public async Task<List<SimpleRegion>> GetAllDistrictsFromCache()
        {
            List<SimpleRegion> result;
            var key = $"GetAllDistricts";
            using (var cacheHelper = CacheHelper.CreateCacheClient(CacheClientName))
            {
                var cacheResult = await cacheHelper.GetOrSetAsync(key, () =>
                     GetAllDistrictsAsync(), TimeSpan.FromMinutes(10));
                if (cacheResult.Success)
                {
                    result = cacheResult.Value;
                }
                else
                {
                    result = await GetAllDistrictsAsync();
                }
            }
            return result;
        }

        /// <summary>
        /// 获取所有三级区域
        /// </summary>
        /// <returns></returns>
        public async Task<List<SimpleRegion>> GetAllDistrictsAsync()
        {
            var result = new List<SimpleRegion>();
            var regions = await GetAllMiniRegionAsync();
            foreach (var province in regions)
            {
                var provinceId = province.RegionId;
                var provinceName = province.RegionName;
                if (province.ChildRegions != null && province.ChildRegions.Any())
                {
                    foreach (var city in province.ChildRegions)
                    {
                        if (city.ChildRegions == null || !city.ChildRegions.Any())
                        {
                            result.Add(new SimpleRegion()
                            {
                                ProvinceId = provinceId,
                                ProvinceName = provinceName,
                                CityId = provinceId,
                                CityName = provinceName,
                                DistrictId = city.RegionId,
                                DistrictName = city.RegionName
                            });
                        }
                        else
                        {
                            var cityId = city.RegionId;
                            var cityName = city.RegionName;
                            foreach (var district in city.ChildRegions)
                            {
                                result.Add(new SimpleRegion()
                                {
                                    ProvinceId = provinceId,
                                    ProvinceName = provinceName,
                                    CityId = cityId,
                                    CityName = cityName,
                                    DistrictId = district.RegionId,
                                    DistrictName = district.RegionName
                                });
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 根据地区Id获取下属三级地区
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public async Task<List<int>> GetDistrictIdsByRegionId(int regionId)
        {
            var result = new List<int>();
            var region = await GetRegionByRegionIdAsync(regionId);
            if (region != null)
            {
                switch (region.RegionType)
                {
                    case RegionType.Province:
                        {
                            if (region.IsMunicipality)
                            {
                                result.AddRange(region.ChildRegions.Select(s => s.DistrictId));
                            }
                            else
                            {
                                var districts = await GetAllDistrictsFromCache();
                                result.AddRange(districts.Where(s => s.ProvinceId == regionId).Select(s => s.DistrictId));
                            }
                            break;
                        }
                    case RegionType.City:
                        result.AddRange(region.ChildRegions.Select(s => s.DistrictId)); break;
                    case RegionType.District: result.Add(region.DistrictId); break;
                    default: break;
                }
            }
            return result.Where(s => s > 0).ToList();
        }

        /// <summary>
        /// 根据地区Id获取下属二级地区
        /// </summary>
        /// <param name="regionId">地区Id</param>
        /// <param name="municipalityIncludeDistrict">直辖市是否包含下属区</param>
        /// <returns></returns>
        public async Task<List<int>> GetCityIdsByRegionId(int regionId, bool municipalityIncludeDistrict)
        {
            var result = new List<int>();
            var region = await GetRegionByRegionIdAsync(regionId);
            if (region != null)
            {
                switch (region.RegionType)
                {
                    case RegionType.Province:
                        {
                            if (region.IsMunicipality)//直辖市的返回类型为Province
                            {
                                if (municipalityIncludeDistrict)
                                {
                                    result.AddRange(region.ChildRegions.Select(s => s.DistrictId));
                                }
                                else
                                {
                                    result.Add(region.CityId);
                                }
                            }
                            else
                            {
                                result.AddRange(region.ChildRegions.Select(d => d.CityId));
                            }
                            break;
                        }
                    case RegionType.City:
                        result.Add(region.CityId); break;
                    default: break;
                }
            }
            return result;
        }

        /// <summary>
        /// 根据地区Id获取地区信息
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public async Task<Region> GetRegionByRegionIdAsync(int regionId)
        {
            using (var client = new RegionClient())
            {
                var clientResult = await client.GetRegionByRegionIdAsync(regionId);
                clientResult.ThrowIfException(true);
                return clientResult.Result;
            }
        }

        /// <summary>
        /// 根据所有地区信息
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public async Task<List<MiniRegion>> GetAllMiniRegionAsync()
        {
            using (var client = new RegionClient())
            {
                var clientResult = await client.GetAllMiniRegionAsync();
                clientResult.ThrowIfException(true);
                return clientResult.Result?.ToList();
            }
        }

        /// <summary>
        /// 获取所有省份信息
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SimpleRegion>> GetAllProvince()
        {
            using (var client = new RegionClient())
            {
                var clientResult = await client.GetAllProvinceAsync();
                clientResult.ThrowIfException(true);
                return clientResult.Result;
            }
        }
    }
}
