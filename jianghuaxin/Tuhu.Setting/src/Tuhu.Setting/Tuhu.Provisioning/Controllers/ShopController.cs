using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Xml;
using Tuhu.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Component.Common.Models;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models.Region;
using System.Threading.Tasks;
using Tuhu.Service.Shop.Models.Request;
using Tuhu.Service.Shop.Models;
using System;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class ShopController : Controller
    {
        [PowerManage]
        public ActionResult Index()
        {
            return View();
        }
       
        /// <summary>
        /// 获取所有省份
        /// </summary>
        /// <returns>Json格式</returns>
        public async Task<ActionResult> GetAllProvince()
        {
            IEnumerable<SimpleRegion> result = null;
            using (var client = new RegionClient())
            {
                var serviceResult = await client.GetAllProvinceAsync();
                serviceResult.ThrowIfException(true);
                result = serviceResult?.Result;
            }
            return Json(new { isSuccess = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过regionId获取市/区
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns>Json格式</returns>
        public async Task<ActionResult> GetRegionByRegionId(int regionId)
        {
            Tuhu.Service.Shop.Models.Region.Region result = null;
            using (var client = new RegionClient())
            {
                var serviceResult = await client.GetRegionByRegionIdAsync(regionId);
                serviceResult.ThrowIfException(true);
                result = serviceResult?.Result;
            }
            return Json(new { isSuccess = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

      
        public async Task<ActionResult> SearchShopModel(double? latBegin, double? lngBegin,
            string city, string district, string province, string sort, string serviceType, string serviceId,
            int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object> {["Code"] = "0"};
            Tuhu.Models.PagerModel page = null;

            #region 获取所有相关的ShopId所需的Request

            var filters = new List<ShopQueryFilterModel>();
            if (!string.IsNullOrWhiteSpace(serviceType))
            {
                if (string.Equals(serviceType, "MR"))
                {
                    filters.Add(new ShopQueryFilterModel()
                    {
                        FilterValueType = ShopQueryFilterValueType.BeautyCategoryId.ToString(),
                        Values = null,
                        JoinType = JoinType.And.ToString()
                    });
                }
                else
                {
                    filters.Add(new ShopQueryFilterModel()
                    {
                        FilterValueType = ShopQueryFilterValueType.ServiceType.ToString(),
                        Values = new List<string>() {serviceType},
                        JoinType = JoinType.And.ToString()
                    });
                }
            }
            if (!string.IsNullOrWhiteSpace(serviceId))
            {
                filters.Add(new ShopQueryFilterModel()
                {
                    FilterValueType = ShopQueryFilterValueType.ServiceId.ToString(),
                    Values = new List<string>() {serviceId},
                    JoinType = JoinType.And.ToString()
                });
            }
            var request = new ShopSearchRequest()
            {

                ProvinceName = province,
                CityName = city,
                DistrictName = district,
                Lat = latBegin,
                Lon = lngBegin,
                Filters = filters,
                SortTypes = new List<ShopQuerySortModel>()
                {
                    new ShopQuerySortModel()
                    {
                        SortValueType = ShopQuerySortValueType.Default.ToString(),
                        ServiceType = serviceType,
                        SortOrder = sort
                    },
                },

                PageSize = pageSize,
                PageIndex = pageIndex

            };

            #endregion

            IEnumerable<ShopSimpleDetailModel> shopSimpleDetails = null;
            Dictionary<int, decimal> biResultDic = null;
            using (var client = new ShopClient())
            {
                var searchResult = (await client.SearchShopIdsAsync(request))?.Result;
                var shopIds = searchResult?.Source;

                page = searchResult?.Pager;
                if (shopIds != null && shopIds.Any())
                {
                    #region 获取BI综合值

                    ShopSearchRequest bIrequest = new ShopSearchRequest()
                    {
                        Lat = latBegin,
                        Lon = lngBegin,
                        Filters = new List<ShopQueryFilterModel>()
                        {
                            new ShopQueryFilterModel()
                            {
                                FilterValueType = ShopQueryFilterValueType.ShopId.ToString(),
                                Values = shopIds.Select(x => x.ToString()).ToList(),
                                JoinType = JoinType.Or.ToString()
                            }
                        },
                        SortTypes = new List<ShopQuerySortModel>()
                        {
                            new ShopQuerySortModel()
                            {
                                SortValueType = ShopQuerySortValueType.Default.ToString(),
                                ServiceType = serviceType,
                            },
                        },
                        PageSize = pageSize,
                        PageIndex = pageIndex
                    };
                    biResultDic = (await client.GetBiWeightByShopIdsAsync(bIrequest))?.Result;

                    #endregion

                    shopSimpleDetails = (await client.SelectShopSimpleDetailsAsync(shopIds))?.Result;
                }
            }
            IEnumerable<ShopSortModel> shops = null;
            if (shopSimpleDetails != null)
            {
                shops = shopSimpleDetails
                    .Select(t => Controls.ModelConvertHelper.ConvertToShopSortModel(t, serviceType))
                    .ToList();

                #region ShopSimpleDetailModel中不存在的字段进行单独处理

                shops.ForEach(x =>
                {
                    x.Distance = (latBegin != null && lngBegin != null && x.ShopLat != null && x.ShopLng != null)
                        ? Controls.MathHelper.GetDistance(latBegin.Value,
                            lngBegin.Value, x.ShopLat.Value, x.ShopLng.Value)
                        : 0;
                    x.ComprehensiveScore = biResultDic != null && biResultDic.ContainsKey(x.ShopId)
                        ? biResultDic[x.ShopId]
                        : 0;

                });

                #endregion
            }
            if (shops != null)
            {
                dic.Add("TotalPage", page?.TotalPage);
                dic["Code"] = "1";
                dic.Add("Shops", shops);
                dic.Add("Count", page?.Total);
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
    }
}