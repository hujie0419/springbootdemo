using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Newtonsoft.Json;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.Models;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Product;
using Tuhu.Service.Vender;
using Tuhu.Service.Vender.Models;
using Tuhu.Service.WMS.Warehouse;
using Tuhu.Service.WMS.Warehouse.Request;

namespace Tuhu.Provisioning.Business.LimitAreaSaleManager
{
    public class LimitAreaSaleManager
    {
        private static readonly ILog logger = LogManager.GetLogger<LimitAreaSaleManager>();

        public static List<LimitAreaSaleModel> GetProductByPid(string keyWord)
        {
            List<LimitAreaSaleModel> result = null;
            try
            {
                result= DalLimitAreaSale.GetProductByPid(keyWord).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return result;
        }
        public static List<LimitAreaSaleModel> GetLimitAreaSalePidList(int pageIndex,int pageSize,string keyWord, int isLimit)
        {
            List<LimitAreaSaleModel> result = null;
            try
            {
                result = DalLimitAreaSale.GetLimitAreaSalePidList(pageIndex, pageSize, keyWord, isLimit).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return result;
        }

        public static bool SaveLimitAreaSalePid(string pid,int isLimit,string user)
        {
            var bResult = true;
            try
            {
                var beforvalue = DalLimitAreaSale.GetLimitAreaSaleProductConfigLimit(pid);
                var result1 = DalLimitAreaSale.SaveLimitAreaSalePid(pid,isLimit);
                if (result1 <= 0)
                    return false;
                var oprLog = new FlashSaleProductOprLog
                {
                    OperateUser = ThreadIdentity.Operator.Name,
                    CreateDateTime = DateTime.Now,
                    BeforeValue = beforvalue==null?JsonConvert.SerializeObject(null):
                    JsonConvert.SerializeObject(new SimpleLimitAreaSaleModel()
                    {
                        Pid = pid,
                        IsLimit = beforvalue.Value,
                    }),
                    AfterValue = JsonConvert.SerializeObject(new SimpleLimitAreaSaleModel()
                    {
                    Pid = pid,
                    IsLimit = isLimit,
                    }),
                    LogType = "LimitAreaPid",
                    LogId = pid,
                    Operation = beforvalue==null? "新增产品限售": "修改产品限售"
                };
                LoggerManager.InsertLog("LimitAreaOprLog", oprLog);

                var tableData = SelectLimitAreaSaleCityInfo();
                if (!tableData.Any())
                {
                    var municipalit = new[] { "上海市", "北京市", "天津市", "重庆市" };
                    var dbresult = LimitAreaSaleManager.SelectRegions();
                    var result = new List<LimitAreaSaleCityModel>();
                    foreach (var province in dbresult)
                    {
                        if (municipalit.Contains(province.RegionName))
                        {
                            var regionModel = new LimitAreaSaleCityModel()
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
                                var regionModel = new LimitAreaSaleCityModel()
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
                    var tempdata = (from a in result
                        join b in tableData on a.CityId equals b.CityId into temp
                        from b in temp.DefaultIfEmpty()
                        select new LimitAreaSaleCityModel
                        {
                            ProductConfigId = 0,
                            ProvinceId = a.ProvinceId,
                            ProvinceName = a.ProvinceName,
                            CityId = a.CityId,
                            CityName = a.CityName,
                            IsAllowSale = b?.IsAllowSale ?? 0,
                            WarehouseId = b?.WarehouseId,
                            WarehouseName = b?.WarehouseName,
                            SupplierId = b?.SupplierId,
                            SupplierName = b?.SupplierName,
                            Pid = pid,
                        }).ToList();
                    tempdata.ForEach(r =>
                    {
                        bResult = bResult &&
                                  SaveLimitAreaSaleCity(r.ProductConfigId, r.CityId, r.CityName, r.IsAllowSale,
                                      r.WarehouseId, r.WarehouseName, r.SupplierId, r.SupplierName);
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return bResult;
        }

        public static bool SaveLimitAreaSaleCity(int productConfigId, int cityId, string cityName, int isAllowSale, int? warehouseId, string warehouseName, int? supplierId, string supplierName,string user=null)
        {
            bool result = false;
            try
            {
                var beforvalue = DalLimitAreaSale.GetLimitAreaSaleCityConfigLimit(productConfigId, cityId);
                var dbresult = DalLimitAreaSale.SaveLimitAreaSaleCity(productConfigId, cityId, cityName, isAllowSale,
                    warehouseId, warehouseName, supplierId, supplierName);
                result = dbresult > 0;

                var oprLog = new FlashSaleProductOprLog
                {
                    OperateUser = ThreadIdentity.Operator.Name,
                    CreateDateTime = DateTime.Now,
                    BeforeValue = JsonConvert.SerializeObject(beforvalue),
                    AfterValue = JsonConvert.SerializeObject(new SimpleLimitAreaSaleCityModel()
                    {
                        CityId = cityId,
                        CityName = cityName,
                        IsAllowSale= isAllowSale,
                        WarehouseId= warehouseId,
                        WarehouseName= warehouseName,
                        SupplierId= supplierId,
                        SupplierName= supplierName
                    }),
                    LogType = "LimitAreaCity",
                    LogId = productConfigId+"|"+cityId,
                    Operation =  "修改地区限售"
                };
                LoggerManager.InsertLog("LimitAreaOprLog", oprLog);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return result;
        }
        public static IEnumerable<SimpleRegionModel> SelectRegions()
        {
            return SimpleRegionModel.Parse(DalLimitAreaSale.SelectRegions());
        }

        public static int SelectLimitAreaSaleByPid(string pid)
        {
            return DalLimitAreaSale.SelectLimitAreaSaleByPid(pid);
        }

        public static List<LimitAreaSaleCityModel> SelectLimitAreaSaleCityInfoByPid(string pid)
        {
            return DalLimitAreaSale.SelectLimitAreaSaleCityInfoByPid(pid);
        }

        public static List<LimitAreaSaleCityModel> SelectLimitAreaSaleCityInfo()
        {
            return DalLimitAreaSale.SelectLimitAreaSaleCityInfo();
        }
        public static List<SupplierAndWarehoseModel> SelectSupplier()
        {
            var list = new List<SupplierAndWarehoseModel>();
            using (var clint = new DeliveryClient())
            {
                var model = new VenderEntity();
                var reuslt = clint.GetVenderList(model);//可传空对象,查询所有订单详情
                var venderList = reuslt.Result;
                venderList.ForEach(r =>
                {
                    list.Add(new SupplierAndWarehoseModel()
                    {
                        SupplierId =r.PKID,
                        SupplierName = r.VenderShortName
                    });
                });
            }
            return list;
        }
        public static List<SupplierAndWarehoseModel> SelectWarehouse()
        {
            try
            {

            var list = new List<SupplierAndWarehoseModel>();
            using (var clint = new WarehouseClient())
            {
                var reuslt = clint.QueryAllWarehouses(new QueryAllWarehousesRequest());
                var warehouse = reuslt.Result;
                warehouse.Warehouses.ForEach(r =>
                {
                    list.Add(new SupplierAndWarehoseModel()
                    {
                        WarehouseId = r.LocationId,
                        WarehouseName = r.Location
                    });
                });
            }
                return list;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static bool Refreshcache(List<string> pids)
        {
            try
            {
                using (var clint = new CacheClient())
                {
                    var reuslt = clint.RefreshLimitRegionProduct(pids);
                    return reuslt.Result;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
