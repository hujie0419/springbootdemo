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
    public class VendorProductPriorityConfigManager:VendorProductCommonManager
    {
        private static readonly IConnectionManager configConnRo =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private static readonly IConnectionManager configReadConnRo =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        private readonly IDBScopeManager _dbScopeManagerConfigRead;
        private readonly IDBScopeManager _dbScopeManagerConfig;
        private readonly DalVendorProductPriorityConfig _dal;
        private readonly BaoYangManager _baoYangManager;
        private readonly RegionService _regionService;
        private readonly Common.Logging.ILog _logger;
        private readonly VendorProductService _vendorProductService;


        public VendorProductPriorityConfigManager()
        {
            _dbScopeManagerConfig = new DBScopeManager(configConnRo);
            _dbScopeManagerConfigRead = new DBScopeManager(configReadConnRo);
            _dal = new DalVendorProductPriorityConfig();
            _baoYangManager = new BaoYangManager();
            _regionService = new RegionService();
            _logger = LogManager.GetLogger(typeof(VendorProductPriorityConfigManager));
            _vendorProductService = new VendorProductService();
        }

        /// <summary>
        /// 选择优先级
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<List<VendorProductPriorityConfigModel>, int>> SelectVendorProductPriorityConfigPriority
            (string productType, string configType, int provinceId, int cityId, PagerModel pager)
        {
            var result = new List<VendorProductPriorityConfigModel>();
            var totalCount = 0;
            try
            {
                var regionIds = cityId > 0 ? new List<int>(1) { cityId }
                    : provinceId > 0 ? await _regionService.GetCityIdsByRegionId(provinceId, true)
                        : new List<int>(1) { provinceId };
                var searchResult = _dbScopeManagerConfigRead.Execute(conn =>
                     _dal.SearchVendorProductPriorityConfig(conn, productType, configType, regionIds, pager));
                result = searchResult?.Item1;
                totalCount = searchResult?.Item2 ?? 0;
            }
            catch (Exception ex)
            {
                _logger.Error("SelectVendorProductPriorityConfigPriority", ex);
            }
            return Tuple.Create(result, totalCount);
        }

        /// <summary>
        /// 编辑优先级
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public bool UpSertVendorProductPriorityConfigPriority(VendorProductPriorityConfigModel model, string user)
        {
            var result = false;
            try
            {
                var oldValue = _dbScopeManagerConfigRead.Execute(conn =>
                            _dal.GetVendorProductPriorityConfigByPKID(conn, model.PKID));
                if (oldValue != null)
                {
                    model.CreateDateTime = oldValue.CreateDateTime;
                    model.LastUpdateDateTime = DateTime.Now;
                    result = _dbScopeManagerConfig.Execute(conn => _dal.EditVendorProductPriorityConfig(conn, model));
                    if (result)
                    {
                        _baoYangManager.InsertLog("OprVendorProduct", new OprVendorProductModel()
                        {
                            LogType = "VendorProductPriorityConfig",
                            IdentityId = $"{model.ProductType}_{model.ConfigType}_{model.VehicleId ?? model.RegionId.ToString()}",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = JsonConvert.SerializeObject(model),
                            OperateUser = user,
                            Remarks = "Update",
                            CreateDateTime = DateTime.Now,
                        });
                    }
                }
                else
                {
                    var pkid = _dbScopeManagerConfig.Execute(conn => _dal.AddVendorProductPriorityConfig(conn, model));
                    result = pkid > 0;
                    if (result)
                    {
                        model.PKID = pkid;
                        model.CreateDateTime = DateTime.Now;
                        model.LastUpdateDateTime = DateTime.Now;
                        _baoYangManager.InsertLog("OprVendorProduct", new OprVendorProductModel()
                        {
                            LogType = "VendorProductPriorityConfig",
                            IdentityId= $"{model.ProductType}_{model.ConfigType}_{model.VehicleId ?? model.RegionId.ToString()}",
                            OldValue = string.Empty,
                            NewValue = JsonConvert.SerializeObject(model),
                            OperateUser = user,
                            Remarks = "Add",
                            CreateDateTime = DateTime.Now,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("EditVendorProductPriorityConfigPriority", ex);
            }
            return result;
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public async Task<bool> RemoveCache(List<VendorProductPriorityConfigModel> models)
        {
            var result = false;
            try
            {
                if (models != null && models.Any())
                {
                    var request = new RemoveCacheByTypeRequest()
                    {
                        Type = "BrandPriority",
                        Data = models.Where(s => !string.IsNullOrEmpty(s.ProductType))
                            .Select(s => $"{s.ProductType}/{s.RegionId}").ToList()
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
    }
}
