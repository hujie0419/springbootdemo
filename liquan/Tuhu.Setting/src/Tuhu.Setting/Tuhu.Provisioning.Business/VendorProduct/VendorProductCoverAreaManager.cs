using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.ServiceProxy;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.VendorProduct.Request;

namespace Tuhu.Provisioning.Business
{
    public class VendorProductCoverAreaManager : VendorProductCommonManager
    {
        private static readonly IConnectionManager configConnRo =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private static readonly IConnectionManager configReadConnRo =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        private readonly IDBScopeManager _dbScopeManagerConfigRead;
        private readonly IDBScopeManager _dbScopeManagerConfig;
        private readonly DalVendorProductCoverArea _dal;
        private readonly BaoYangManager _baoYangManager;
        private readonly RegionService _regionService;
        private readonly Common.Logging.ILog _logger;
        private readonly VendorProductService _vendorProductService;


        public VendorProductCoverAreaManager()
        {
            _dbScopeManagerConfig = new DBScopeManager(configConnRo);
            _dbScopeManagerConfigRead = new DBScopeManager(configReadConnRo);
            _dal = new DalVendorProductCoverArea();
            _baoYangManager = new BaoYangManager();
            _regionService = new RegionService();
            _logger = LogManager.GetLogger(typeof(VendorProductCoverAreaManager));
            _vendorProductService = new VendorProductService();
        }

        #region 覆盖区域

        /// <summary>
        /// 查询覆盖区域
        /// </summary>
        /// <param name="brand"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="district"></param>
        /// <param name="pager"></param>
        /// <returns></returns>
        public async Task<Tuple<List<VendorProductCoverAreaViewModel>, int>> SelectVendorProductCoverArea
            (string productType, string coverType, string brand, string pid,
            int provinceId, int cityId, int districtId, PagerModel pager)
        {
            var result = new List<VendorProductCoverAreaViewModel>();
            var totalCount = 0;
            try
            {
                var regionIds = await GetDistrictIdsByRegionAsync(provinceId, cityId, districtId);
                var searchResult = null as Tuple<List<VendorProductCoverAreaModel>, int>;
                switch (coverType)
                {
                    case "Brand":
                        searchResult = _dbScopeManagerConfigRead.Execute(conn =>
                            _dal.SearchVendorProductCoverArea(conn, productType, coverType, brand, null, regionIds, pager)); break;
                    case "Pid":
                        searchResult = _dbScopeManagerConfigRead.Execute(conn =>
                            _dal.SearchVendorProductCoverArea(conn, productType, coverType, null, pid, regionIds, pager)); break;
                    default: break;
                }
                var configs = searchResult?.Item1;
                totalCount = searchResult?.Item2 ?? 0;
                var regions = await _regionService.GetAllDistrictsFromCache();
                configs?.ForEach(s =>
                  {
                      var district = regions.FirstOrDefault(r => r.DistrictId == s.CoverRegionId);
                      if (district != null)
                      {
                          result.Add(new VendorProductCoverAreaViewModel()
                          {
                              Brand = s.Brand,
                              CityId = district.CityId,
                              CityName = district.CityName,
                              CoverRegionId = s.CoverRegionId,
                              CoverType = s.CoverType,
                              CreateDateTime = s.CreateDateTime,
                              DistrictId = district.DistrictId,
                              DistrictName = district.DistrictName,
                              IsDeleted = s.IsDeleted,
                              IsEnabled = s.IsEnabled,
                              LastUpdateDateTime = s.LastUpdateDateTime,
                              Pid = s.Pid,
                              PKID = s.PKID,
                              ProductType = s.ProductType,
                              ProvinceId = district.ProvinceId,
                              ProvinceName = district.ProvinceName,
                              Remark = s.Remark
                          });
                      }
                  });
            }
            catch (Exception ex)
            {
                _logger.Error("SearchCoverArea", ex);
                result = null;
            }
            return Tuple.Create(result, totalCount);
        }

        /// <summary>
        /// 编辑覆盖区域
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool EditCoverArea(VendorProductCoverAreaModel model, string user)
        {
            bool result = false;
            try
            {
                var oldValue = _dbScopeManagerConfigRead.Execute(conn =>
                    _dal.GetVendorProductConverAreaByPKID(conn, model.PKID));
                if (oldValue != null && !oldValue.IsDeleted)
                {
                    model.CreateDateTime = oldValue.CreateDateTime;
                    model.LastUpdateDateTime = DateTime.Now;
                    result = _dbScopeManagerConfig.Execute(conn => _dal.EditVendorProductCoverArea(conn, model));
                    if (result)
                    {
                        _baoYangManager.InsertLog("OprVendorProduct", new OprVendorProductModel()
                        {
                            LogType = "VendorProductCoverArea",
                            IdentityId = $"{model.ProductType}_{model.CoverType}_{model.CoverRegionId}_{model.Brand ?? model.Pid}",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = JsonConvert.SerializeObject(model),
                            OperateUser = user,
                            Remarks = "Update",
                            CreateDateTime = DateTime.Now
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("EditCoverArea", ex);
            }
            return result;
        }

        /// <summary>
        /// 删除覆盖区域
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeleteCoverArea(VendorProductCoverAreaModel model, string user)
        {
            bool result = false;
            try
            {
                var oldValue = _dbScopeManagerConfigRead.Execute(conn =>
                    _dal.GetVendorProductConverAreaByPKID(conn, model.PKID));
                if (oldValue == null)
                {
                    result = true;
                }
                else
                {
                    result = _dbScopeManagerConfig.Execute(conn => _dal.DeleteVendorProductCoverArea(conn, model.PKID));
                    if (result)
                    {
                        _baoYangManager.InsertLog("OprVendorProduct", new OprVendorProductModel()
                        {
                            LogType = "VendorProductCoverArea",
                            IdentityId = $"{model.ProductType}_{model.CoverType}_{model.CoverRegionId}_{model.Brand ?? model.Pid}",
                            OldValue = JsonConvert.SerializeObject(model),
                            NewValue = (string)null,
                            OperateUser = user,
                            Remarks = "Delete",
                            CreateDateTime = DateTime.Now,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("DeleteCoverArea", ex);
            }
            return result;
        }

        /// <summary>
        /// 添加蓄覆盖区域
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddCoverArea(VendorProductCoverAreaModel model, string user)
        {
            bool result = false;
            try
            {
                VendorProductCoverAreaModel oldValue = _dbScopeManagerConfigRead.Execute(conn =>
                   _dal.GetVendorProductConverAreaByBrand(conn, model));
                if (oldValue == null || oldValue.IsDeleted)
                {
                    var pkid = _dbScopeManagerConfig.Execute(conn => _dal.AddVendorProductCoverArea(conn, model));
                    result = pkid > 0;
                    if (result)
                    {
                        model.PKID = pkid;
                        _baoYangManager.InsertLog("OprVendorProduct", new OprVendorProductModel()
                        {
                            LogType = "VendorProductCoverArea",
                            IdentityId = $"{model.ProductType}_{model.CoverType}_{model.CoverRegionId}_{model.Brand ?? model.Pid}",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = JsonConvert.SerializeObject(model),
                            OperateUser = user,
                            Remarks = "Add",
                            CreateDateTime = DateTime.Now
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("AddCoverArea", ex);
            }
            return result;
        }

        /// <summary>
        /// 上传的文件填充覆盖区域信息
        /// </summary>
        /// <param name="models"></param>
        /// <param name="productType"></param>
        /// <param name="coverType"></param>
        /// <returns></returns>
        public async Task<Tuple<List<VendorProductCoverAreaModel>, string>> FillConvertCoverArea
            (List<VendorProductCoverAreaViewModel> models, string productType, string coverType)
        {
            var result = new List<VendorProductCoverAreaModel>();
            var message = string.Empty;
            try
            {
                var districts = await _regionService.GetAllDistrictsFromCache();
                foreach (var model in models)
                {
                    var district = districts.FirstOrDefault(d => string.Equals(model.ProvinceName, d.ProvinceName) &&
                      string.Equals(model.CityName, d.CityName) && string.Equals(model.DistrictName, d.DistrictName));
                    if (district != null)
                    {
                        result.Add(new VendorProductCoverAreaModel()
                        {
                            Brand = model.Brand,
                            CoverType = coverType,
                            CoverRegionId = district.DistrictId,
                            IsEnabled = model.IsEnabled,
                            ProductType = productType,
                            Remark = model.Remark
                        });
                    }
                    else
                    {
                        message = $"{model.ProvinceName}-{model.CityName}-{model.DistrictName}不存在，请检查";
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _logger.Error("FillConvertCoverArea", ex);
            }
            return Tuple.Create(result, message);
        }

        /// <summary>
        /// 验证覆盖区域配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Tuple<bool, string> ValidateVendorProductConverArea(VendorProductCoverAreaModel model)
        {
            var result = false;
            var message = string.Empty;
            try
            {
                switch (model.CoverType)
                {
                    case "Brand":
                        if (string.IsNullOrEmpty(model.Brand))
                        {
                            message = "品牌不能为空";
                        }
                        else
                        {
                            var isExist = _dbScopeManagerConfigRead.Execute(conn =>
                                                         _dal.IsExistVendorProductConverAreaByBrand(conn, model));
                            message = isExist ? "配置已存在,无法重复添加" : string.Empty;
                            result = !isExist;
                        }
                        break;
                    case "Pid":
                        if (string.IsNullOrEmpty(model.Pid))
                        {
                            message = "Pid不能为空";
                        }
                        else
                        {
                            var isExist = _dbScopeManagerConfigRead.Execute(conn =>
                             _dal.IsExistVendorProductConverAreaByPid(conn, model));
                            message = isExist ? "配置已存在,无法重复添加" : string.Empty;
                            result = !isExist;
                        }
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("IsExistVendorProductConverArea", ex);
            }
            return Tuple.Create(result, message);
        }

        /// <summary>
        /// 上传覆盖区域
        /// </summary>
        /// <param name="models"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> UploadBatterCoverArea(List<VendorProductCoverAreaModel> models, string user)
        {
            var result = false;
            try
            {
                var insertList = new List<VendorProductCoverAreaModel>();
                var updateList = new List<VendorProductCoverAreaModel>();
                var logs = new List<OprVendorProductModel>();
                foreach (var model in models)
                {
                    var oldValue = _dbScopeManagerConfigRead.Execute(conn =>
                   _dal.GetVendorProductConverAreaByBrand(conn, model));
                    if (oldValue != null)
                    {
                        updateList.Add(model);
                        model.PKID = oldValue.PKID;
                        logs.Add(new OprVendorProductModel()
                        {
                            LogType = "VendorProductCoverArea",
                            IdentityId = $"{model.ProductType}_{model.CoverType}_{model.CoverRegionId}_{model.Brand ?? model.Pid}",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = JsonConvert.SerializeObject(model),
                            CreateDateTime = DateTime.Now,
                            Remarks = "Update",
                            OperateUser = user
                        });
                    }
                    else
                    {
                        insertList.Add(model);
                    }
                }
                _dbScopeManagerConfig.CreateTransaction(conn =>
                {
                    if (insertList.Any() || updateList.Any())
                    {
                        result = UplnsertBatterCoverArea(insertList, updateList, logs, user);
                    }
                });
                if (result)
                {
                    await RemoveCache(models);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("UploadBatterCoverArea", ex);
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public async Task<bool> RemoveCache(List<VendorProductCoverAreaModel> models)
        {
            var result = false;
            try
            {
                if (models != null && models.Any())
                {
                    var request = new RemoveCacheByTypeRequest()
                    {
                        Type = "CoverArea",
                        Data = models.Where(s => !string.IsNullOrEmpty(s.ProductType) && s.CoverRegionId > 0)
                            .Select(s => $"{s.ProductType}/{s.CoverRegionId}").ToList()
                    };
                    result = await _vendorProductService.RemoveCacheByType(request);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("RemoveCache", ex);
                result = false;
            }
            return result;
        }

        #region privateMethods

        /// <summary>
        /// 根据省市区获取三级区域信息
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="districtId"></param>
        /// <returns></returns>
        private async Task<List<int>> GetDistrictIdsByRegionAsync(int provinceId, int cityId, int districtId)
        {
            var regionId = cityId > 0 ? cityId : (provinceId > 0 ? provinceId : 0);
            var regionIds = districtId > 0 ? new List<int>(1) { districtId } :
                 regionId > 0 ? await _regionService.GetDistrictIdsByRegionId(regionId)
                 : (await _regionService.GetAllDistrictsFromCache())?.Select(s => s.DistrictId)?.ToList();
            return regionIds;
        }

        private bool UplnsertBatterCoverArea(List<VendorProductCoverAreaModel> insertList,
            List<VendorProductCoverAreaModel> updateList
            , List<OprVendorProductModel> logs, string user)
        {
            var result = false;
            _dbScopeManagerConfig.CreateTransaction(conn =>
            {
                foreach (var addModel in insertList)
                {
                    var pkid = _dal.AddVendorProductCoverArea(conn, addModel);
                    if (pkid > 0)
                    {
                        addModel.PKID = pkid;
                        logs.Add(new OprVendorProductModel()
                        {
                            LogType = "VendorProductCoverArea",
                            IdentityId = $"{addModel.ProductType}_{addModel.CoverType}_{addModel.CoverRegionId}_{addModel.Brand ?? addModel.Pid}",
                            OldValue = null,
                            NewValue = JsonConvert.SerializeObject(addModel),
                            CreateDateTime = DateTime.Now,
                            Remarks = "Add",
                            OperateUser = user
                        });
                    }
                    else
                    {
                        throw new Exception($"AddCoverArea,数据{JsonConvert.SerializeObject(addModel)}");
                    }
                }
                foreach (var updateModel in updateList)
                {
                    var isUpdated = _dal.EditVendorProductCoverArea(conn, updateModel);
                    if (!isUpdated)
                    {
                        throw new Exception($"EditCoverArea失败,数据{JsonConvert.SerializeObject(updateModel)}");
                    }
                }
                result = true;
            });
            if (result)
            {
                foreach (var log in logs)
                {
                    _baoYangManager.InsertLog("OprVendorProduct", log);
                }
            }
            return result;
        }

        #endregion
    }
}
