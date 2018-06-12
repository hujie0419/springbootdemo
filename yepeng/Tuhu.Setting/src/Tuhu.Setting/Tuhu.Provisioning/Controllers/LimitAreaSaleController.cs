using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.LimitAreaSaleManager;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;
using Tuhu.Service.Shop;

namespace Tuhu.Provisioning.Controllers
{
    public class LimitAreaSaleController : Controller
    {
        #region 查询参数
        private int _pageIndex;
        private int PageIndex
        {
            get
            {
                if (_pageIndex <= 0)
                {
                    if (!int.TryParse(Request["PageIndex"], out _pageIndex))
                        _pageIndex = 1;
                }
                return _pageIndex;
            }
        }
        private int _pageSize;
        private int PageSize
        {
            get
            {
                if (_pageSize <= 0)
                {
                    if (!int.TryParse(Request["PageSize"], out _pageSize) || _pageSize >= 200)
                        _pageSize = 20;
                }
                return _pageSize;
            }
        }

        private string _keyWord;
        private string KeyWord
        {
            get
            {
                if (string.IsNullOrEmpty(_keyWord))
                {
                    _keyWord = Request["KeyWord"];
                }
                return _keyWord;
            }
        }
        private int _isLImit;
        private int IsLimit
        {
            get
            {
                _isLImit = Convert.ToInt32(Request["isLImit"]);
                return _isLImit;
            }
        }
        private int _isAllowSale;
        private int IsAllowSale
        {
            get
            {
                _isAllowSale = Convert.ToInt32(Request["isallowSale"]);
                return _isAllowSale;
            }
        }
        private int _cityId;
        private int CityId
        {
            get
            {
                _cityId = Convert.ToInt32(Request["City"]);
                return _cityId;
            }
        }

        private int _provinceId;
        private int ProvinceId
        {
            get
            {
                _provinceId = Convert.ToInt32(Request["Province"]);
                return _provinceId;
            }
        }
        #endregion
        #region 限制地区销售的产品
        public ActionResult LimitAreaSalePid()
        {
            return View();
        }

        public JsonResult GetLimitAreaSalePid()
        {
            Tuple<int, List<LimitAreaSaleModel>> result;
            if (!string.IsNullOrEmpty(KeyWord))
            {
                result = new Tuple<int, List<LimitAreaSaleModel>>(1, LimitAreaSaleManager.GetProductByPid(KeyWord));
            }
            else
            {
                var db = LimitAreaSaleManager.GetLimitAreaSalePidList(PageSize, PageIndex, KeyWord, IsLimit);
                result = new Tuple<int, List<LimitAreaSaleModel>>(db.Select(r => r.TotalCount).FirstOrDefault(), db);
            }
            return Json(result);
        }

        public JsonResult SaveLimitAreaSalePid(string pid, int isLimit)
        {
            var result = LimitAreaSaleManager.SaveLimitAreaSalePid(pid, isLimit, User.Identity.Name);
            if (result)
            {
                return Json(new
                {
                    status = 1
                });
            }
            else
            {
                return Json(new
                {
                    status = 0
                });
            }
        }

        public JsonResult Refreshcache(string pid)
        {
            var result = LimitAreaSaleManager.Refreshcache(new List<string> { pid });
            if (result)
            {
                return Json(new
                {
                    status = 1
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    status = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region 限制地区销售受限地区配置
        public ActionResult LimitAreaSaleCity()
        {
            return View();
        }
        public JsonResult SelectAllProvince()
        {
            var result = new List<LimitSaleRegionModel>();
            var dbresult = LimitAreaSaleManager.SelectRegions();
            foreach (var province in dbresult)
            {
                var regionModel = new LimitSaleRegionModel()
                {
                    ProvinceId = province.PKID,
                    ProvinceName = province.RegionName,
                    CityId = province.PKID,
                    CityName = province.RegionName
                };
                result.Add(regionModel);

            }
            return Json(result);
        }
        public JsonResult GetAllCitys(int ProvinceId)
        {
            using (var client = new RegionClient())
            {
                var regions = client.GetRegionByRegionId(ProvinceId);
                regions.ThrowIfException(true);
                if (regions.Result.ChildRegions.FirstOrDefault().IsBelongMunicipality)
                    return Json(regions.Result.ChildRegions.Select(s => new { id = s.CityId, name = s.CityName }).Distinct().ToArray());
                else
                {
                    return Json(regions.Result.ChildRegions.Select(s => new { id = s.CityId, name = s.CityName }).ToArray());
                }
            }
        }
        public JsonResult GetLimitAreaSaleCity()
        {
            Tuple<int, List<LimitSaleRegionModel>> result2;
            var municipalit = new[] { "上海市", "北京市", "天津市", "重庆市" };
            var dbresult = LimitAreaSaleManager.SelectRegions();
            var result = new List<LimitSaleRegionModel>();
            foreach (var province in dbresult)
            {
                if (municipalit.Contains(province.RegionName))
                {
                    var regionModel = new LimitSaleRegionModel()
                    {
                        ProvinceId = province.PKID,
                        ProvinceName = province.RegionName,
                        CityId = province.PKID,
                        CityName = province.RegionName
                    };
                    result.Add(regionModel);
                }
                else
                {
                    foreach (var city in province.ChildrenRegion)
                    {
                        var regionModel = new LimitSaleRegionModel()
                        {
                            ProvinceId = province.PKID,
                            ProvinceName = province.RegionName,
                            CityId = city.PKID,
                            CityName = city.RegionName
                        };
                        result.Add(regionModel);
                    }
                }
            }
            var tableData = LimitAreaSaleManager.SelectLimitAreaSaleCityInfo();
            var tempdata = (from a in result
                            join b in tableData on a.CityId equals b.CityId into temp
                            from b in temp.DefaultIfEmpty()
                            select new LimitSaleRegionModel
                            {
                                Pkid = b.Pkid,
                                //ProductConfigId = limitPkid,
                                ProvinceId = a.ProvinceId,
                                ProvinceName = a.ProvinceName,
                                CityId = a.CityId,
                                CityName = a.CityName,
                                IsAllowSale = b?.IsAllowSale ?? 0,
                                WarehouseId = b?.WarehouseId,
                                WarehouseName = b?.WarehouseName,
                                SupplierId = b?.SupplierId,
                                SupplierName = b?.SupplierName,
                                //Pid= KeyWord,
                            }).Where(r =>(r.IsAllowSale == IsAllowSale|| IsAllowSale==2) &&(r.ProvinceId==ProvinceId|| ProvinceId==0)&&(r.CityId==CityId||CityId==0)).OrderBy(r => r.Pkid).ToList();
            result2 = new Tuple<int, List<LimitSaleRegionModel>>(tempdata.Count, tempdata.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToList());
            return Json(result2);
            // }
        }

        public ActionResult SelectSupplier()
        {
            var result = LimitAreaSaleManager.SelectSupplier();
            return Json(result);
        }
        public ActionResult SelectWarehouse()
        {
            var result = LimitAreaSaleManager.SelectWarehouse();
            return Json(result);
        }

        public JsonResult SaveLimitAreaSaleCity(int productConfigId, int cityId, string cityName, int isAllowSale, int warehouseId, string warehouseName, int supplierId, string supplierName,string muticityCheckedIds)
        {
            bool result=true ;
            var warehouses = LimitAreaSaleManager.SelectWarehouse();
            warehouseName = warehouses.Where(r => r.WarehouseId == warehouseId).Select(r => r.WarehouseName).FirstOrDefault();
            var suppliers = LimitAreaSaleManager.SelectSupplier();
            supplierName = suppliers.Where(r => r.SupplierId == supplierId).Select(r => r.SupplierName).FirstOrDefault();
            if (string.IsNullOrEmpty(muticityCheckedIds))
            {
                 result = LimitAreaSaleManager.SaveLimitAreaSaleCity(productConfigId, cityId, cityName, isAllowSale,
        warehouseId, warehouseName, supplierId, supplierName, User.Identity.Name);
            }

            else
            {
                //批量新增
                var list = muticityCheckedIds.Split(',').ToList();
                foreach (var item in list)
                {
                    cityId = Convert.ToInt32(item.Split(';')[0]);
                    cityName = item.Split(';')[1];
                    result = result&& LimitAreaSaleManager.SaveLimitAreaSaleCity(productConfigId, cityId, cityName, isAllowSale,
warehouseId, warehouseName, supplierId, supplierName, User.Identity.Name);
                }
            }
            if (result)
            {

                return Json(new
                {
                    status = 1
                });
            }
            else
            {
                return Json(new
                {
                    status = 0
                });
            }
        }
        #endregion

        public JsonResult GetLimitAreaSaleConfigLog(string logId)
        {
            if (string.IsNullOrEmpty(logId))
                return Json(null);
            var result = LoggerManager.SelectFlashSaleHistoryByLogId(logId, "LimitAreaPid");
            return Json(result);
        }
        public ActionResult LimitAreaSaleLog(string logId)
        {
            return View((object)logId);
        }
        public JsonResult GetLimitAreaSaleConfigCityLog(string logId)
        {
            if (string.IsNullOrEmpty(logId))
                return Json(null);
            var result = LoggerManager.SelectFlashSaleHistoryByLogId(logId, "LimitAreaCity");
            return Json(result);
        }
        public ActionResult LimitAreaSaleCityLog(string logId)
        {
            return View((object)logId);
        }
    }
}