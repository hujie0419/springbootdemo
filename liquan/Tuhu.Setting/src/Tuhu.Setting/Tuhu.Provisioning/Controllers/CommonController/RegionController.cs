using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.ServiceProxy;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models.Region;

namespace Tuhu.Provisioning.Controllers
{
    /// <summary>
    /// 获取地区信息公用模块
    /// </summary>
    public class RegionController:Controller
    {
        private readonly ILog _logger= LogManager.GetLogger(typeof(RegionController));

        /// <summary>
        /// 获取所有省份信息
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetAllProvince()
        {
            IEnumerable<SimpleRegion> result = null;
            try
            {
                var regionProxy = new RegionService();
                result = await regionProxy.GetAllProvince();
            }
            catch (Exception ex)
            {
                result = null;
                _logger.Error("GetAllProvince", ex);
            }
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取二级城市
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetCitiesByProvinceId(int provinceId)
        {
            List<Region> result = null;
            try
            {
                if (provinceId > 0)
                {
                    var regionProxy = new RegionService();
                    var province = await regionProxy.GetRegionByRegionIdAsync(provinceId);
                    result = province.ChildRegions.Distinct(s => s.CityId).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("GetCitiesByProvinceId", ex);
            }
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取三级地区
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetDistrictsByCityId(int cityId)
        {
            List<Region> result = null;
            try
            {
                if (cityId > 0)
                {
                    var regionProxy = new RegionService();
                    var province = await regionProxy.GetRegionByRegionIdAsync(cityId);
                    result = province.ChildRegions.Distinct(s => s.DistrictId).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("GetDistrictsByCityId", ex);
            }
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }
    }
}