using AutoMapper;
using Microsoft.ApplicationBlocks.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service;
using Tuhu.Service.OprLog;
using Tuhu.Service.OprLog.Models;

namespace Tuhu.Provisioning.Business.RegionActivityPageConfig
{
    public class CityActivityPageConfig
    {
        #region private
        private static readonly Common.Logging.ILog Logger = Common.Logging.LogManager.GetLogger(typeof(CityActivityPageConfig));

        private static readonly IConnectionManager GungnirConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IDBScopeManager dbScopeManager = new DBScopeManager(GungnirConnectionManager);
        private static readonly IConnectionManager GungnirReadConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
        private static readonly IDBScopeManager GungnirReadDb = new DBScopeManager(GungnirReadConnectionManager);

        private static readonly IConnectionManager ConfigurationConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private static readonly IDBScopeManager ConfigurationDb = new DBScopeManager(ConfigurationConnectionManager);
        private static readonly IConnectionManager ConfigurationReadConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        private static readonly IDBScopeManager ConfigurationReadDb = new DBScopeManager(ConfigurationReadConnectionManager);

        private static readonly IConnectionManager LogDbManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString);
        private static readonly IDBScopeManager LogDb = new DBScopeManager(LogDbManager);

        #endregion

        #region activity

        public List<RegionVehicleIdActivityConfig> GetAllActivity(RegionVehicleIdActivityConfig filter, int pageIndex, int pageSize, out int totalItem)
        {
            List<RegionVehicleIdActivityConfig> activityList = null;
            totalItem = 0;
            try
            {
                activityList = ConfigurationReadDb.Execute(conn => DalCityActivityPageConfig.SelectAllActivity(conn, filter, pageIndex, pageSize));
                totalItem = ConfigurationReadDb.Execute(conn => DalCityActivityPageConfig.SelectActivityCount(conn, filter));
            }
            catch (Exception ex)
            {
                Logger.Error("GetActivity", ex);
            }
            return activityList;
        }

        /// <summary>
        /// 创建活动
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public bool CreateActivity(RegionVehicleIdActivityConfig activity)
        {

            bool result;
            try
            {
                result = ConfigurationDb.Execute(conn => DalCityActivityPageConfig.CreateActivity(conn, activity));
            }
            catch (Exception ex)
            {
                Logger.Error("CreateActivity", ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 删除活动其对应的活动页
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public bool DeleteActivity(Guid activityId)
        {
            var result = false;
            try
            {
                ConfigurationDb.CreateTransaction(conn =>
                {
                    if (!DalCityActivityPageConfig.DeleteActivityByActivityId(conn, activityId))
                        result = false;
                    DalCityActivityPageConfig.DeleteActivityUrlByActivityId(conn, activityId);
                    result = true;

                });
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("deleteActivity", ex);
            }
            return result;
        }

        /// <summary>
        /// 更新活动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateActivity(RegionVehicleIdActivityConfig model)
        {
            bool result = false;
            try
            {
                result = ConfigurationDb.Execute(conn => DalCityActivityPageConfig.UpdateActivity(conn, model));
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateActivity", ex);
            }
            return result;
        }
        #endregion


        #region vehicle
        public List<VehicleTypeModel> GetAllBrand()
        {
            List<VehicleTypeModel> brandList = null;
            try
            {
                brandList = GungnirReadDb.Execute(conn => DalCityActivityPageConfig.SelectAllBrand(conn));
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllBrand", ex);
            }
            return brandList ?? new List<VehicleTypeModel>();
        }

        public List<VehicleTypeModel> GetVehicleByBrand(string brand)
        {
            List<VehicleTypeModel> vehicleList = null;
            try
            {
                vehicleList = GungnirReadDb.Execute(conn => DalCityActivityPageConfig.SelectVehicleByBrand(conn, brand));
            }
            catch (Exception ex)
            {
                Logger.Error("GetVehicleByBrand", ex);
            }
            return vehicleList ?? new List<VehicleTypeModel>();
        }
        #endregion


        #region activityUrl
        /// <summary>
        /// 根据活动页和渠道获取配置的地区
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="targetUrl"></param>
        /// <param name="wxappUrl"></param>
        /// <returns></returns>
        public List<ProvinceCity> GetRegionIdByActivityIdUrl(Guid activityId, string targetUrl, string wxappUrl)
        {
            List<RegionVehicleIdActivityUrlConfig> idList = null;
            List<ProvinceCity> pcList = new List<ProvinceCity>();
            try
            {
                var client = new Service.Shop.RegionClient();
                idList = ConfigurationReadDb.Execute(conn => DalCityActivityPageConfig.SelectRegionIdVehicleIdByActivityIdUrl(conn, activityId, targetUrl, wxappUrl));
                var ids = idList?.Where(s => s.RegionId > 0)?.Distinct()?.ToList() ?? new List<RegionVehicleIdActivityUrlConfig>();
                foreach (var id in ids)
                {
                    var result = client.GetRegionByRegionId(id.RegionId);
                    if (!result.Success)
                        result.ThrowIfException(true);
                    if (result.Result != null)
                    {
                        ProvinceCity pc = new ProvinceCity()
                        {
                            ProvinceId = result.Result.ProvinceId,
                            ProvinceName = result.Result.ProvinceName,
                            CityId = result.Result.CityId,
                            CityName = result.Result.CityName
                        };
                        pcList.Add(pc);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SelectRegionIdVehicleIdByActivityIdUrl", ex);
            }
            return pcList;
        }

        /// <summary>
        /// 根据活动页和渠道获取配置的车型
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="targetUrl"></param>
        /// <returns></returns>
        public List<VehicleTypeModel> GetVehicleIdByActivityIdUrl(Guid activityId, string targetUrl, string wxappUrl)
        {
            List<RegionVehicleIdActivityUrlConfig> idList = null;
            List<VehicleTypeModel> vtList = new List<VehicleTypeModel>();
            try
            {
                idList = ConfigurationReadDb.Execute(conn => DalCityActivityPageConfig.SelectRegionIdVehicleIdByActivityIdUrl(conn, activityId, targetUrl, wxappUrl));
                var ids = idList?.Select(x => x.VehicleId)?.Distinct()?.ToList() ?? new List<string>();

                vtList = GungnirReadDb.Execute(conn => DalCityActivityPageConfig.SelectVehicleByProductID(conn, string.Join(",", ids)));

            }
            catch (Exception ex)
            {
                Logger.Error("SelectVehicleIdByActivityIdUrl", ex);
            }
            return vtList;
        }

        /// <summary>
        /// 创建活动页
        /// </summary>
        /// <param name="model"></param>
        /// <param name="vehicleIds"></param>
        /// <param name="regionIds"></param>
        /// <returns></returns>
        public bool CreateActivityUrl(RegionVehicleIdActivityUrlConfig model, string vehicleIds, string regionIds)
        {
            var result = false;
            try
            {
                result = ConfigurationDb.Execute(conn => CreateActivityUrlConfig(conn, model, vehicleIds, regionIds));
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("CreateActivityUrl", ex);
            }
            return result;
        }

        /// <summary>
        /// 创建活动页
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="targetUrl"></param>
        /// <param name="isDefault"></param>
        /// <param name="vehicleIds"></param>
        /// <param name="regionIds"></param>
        /// <returns></returns>
        private bool CreateActivityUrlConfig(SqlConnection conn, RegionVehicleIdActivityUrlConfig model, string vehicleIds, string regionIds)
        {
            var result = false;
            var vehicleIdArr = vehicleIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            var regionIdArr = regionIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            //默认页面
            if (model.IsDefault == 1 || !(vehicleIdArr.Any() || regionIdArr.Any()))
            {
                result = DalCityActivityPageConfig.CreateActivityUrl(conn, model);
                if (!result)
                {
                    throw new Exception($"CreateActivityUrl失败,插入失败数据{JsonConvert.SerializeObject(model)}");
                }
            }
            else
            {
                var models = new List<RegionVehicleIdActivityUrlConfig>();
                foreach (var vehicleId in vehicleIdArr)
                {
                    var config = new RegionVehicleIdActivityUrlConfig()
                    {
                        ActivityId = model.ActivityId,
                        TargetUrl = model.TargetUrl,
                        WxappUrl = model.WxappUrl,
                        IsDefault = model.IsDefault,
                        VehicleId = vehicleId,
                        RegionId = 0,
                    };
                    models.Add(config);
                }
                foreach (var regionIdStr in regionIdArr)
                {
                    var regionId = 0;
                    if (int.TryParse(regionIdStr, out regionId))
                    {
                        var config = new RegionVehicleIdActivityUrlConfig()
                        {
                            ActivityId = model.ActivityId,
                            TargetUrl = model.TargetUrl,
                            WxappUrl = model.WxappUrl,
                            IsDefault = model.IsDefault,
                            VehicleId = string.Empty,
                            RegionId = regionId
                        };
                        models.Add(config);
                    }
                }
                if (models.Any())
                {
                    foreach (var urlConfig in models)
                    {
                        var isCreated = DalCityActivityPageConfig.CreateActivityUrl(conn, urlConfig);
                        if (!isCreated)
                        {
                            throw new Exception($"CreateActivityUrlConfig失败,插入失败数据{JsonConvert.SerializeObject(urlConfig)}");
                        }
                    }
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 更新活动页配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="oldTargetUrl"></param>
        /// <param name="oldWxappUrl"></param>
        /// <param name="vehicleIds"></param>
        /// <param name="regionIds"></param>
        /// <returns></returns>
        public bool UpdateActivityUrl(RegionVehicleIdActivityUrlConfig model, string oldTargetUrl, string oldWxappUrl, string vehicleIds, string regionIds)
        {
            bool result = false;
            try
            {
                ConfigurationDb.CreateTransaction(conn =>
                {
                    if (!DalCityActivityPageConfig.DeleteActivityUrlByActivityIdUrl(conn, model.ActivityId, oldTargetUrl, oldWxappUrl))
                        result = false;
                    else
                    {
                        result = CreateActivityUrlConfig(conn, model, vehicleIds, regionIds);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateActivityUrl", ex);
            }
            return result;
        }

        /// <summary>
        /// 删除活动页配置
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="targetUrl"></param>
        /// <param name="wxappUrl"></param>
        /// <returns></returns>
        public bool DeleteActivityUrl(Guid activityId, string targetUrl, string wxappUrl)
        {
            var result = false;
            try
            {
                result = ConfigurationDb.Execute(conn => DalCityActivityPageConfig.DeleteActivityUrlByActivityIdUrl(conn, activityId, targetUrl, wxappUrl));
            }
            catch (Exception ex)
            {
                Logger.Error("DeleteActivityUrl", ex);
            }
            return result;
        }

        /// <summary>
        /// 该渠道该活动是否存在默认页
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public bool IsExistDefaultUrl(Guid activityId)
        {
            var result = false;
            try
            {
                result = ConfigurationReadDb.Execute(conn => DalCityActivityPageConfig.IsExistDefaultUrl(conn, activityId));
            }
            catch (Exception ex)
            {
                Logger.Error("IsExistDefaultUrl", ex);
            }
            return result;
        }

        /// <summary>
        /// 根据活动Id获取活动页
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public List<RegionVehicleIdActivityUrlConfig> GetActivityUrlByActivityId(Guid activityId)
        {
            List<RegionVehicleIdActivityUrlConfig> urlList = null;
            try
            {
                urlList = ConfigurationReadDb.Execute(conn => DalCityActivityPageConfig.SelectActivityUrlByActivityId(conn, activityId));
                foreach (var url in urlList)
                {
                    if (!string.IsNullOrEmpty(url.TargetUrl))
                    {
                        url.UrlTitle = GetActivityTitleByUrl(url.TargetUrl);
                    }
                    if (!string.IsNullOrEmpty(url.WxappUrl))
                    {
                        url.WxappUrlTitle = GetActivityTitleByUrl(url.WxappUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetActivityUrlByActivityId", ex);
            }
            return urlList;
        }
        #endregion

        /// <summary>
        /// 获取活动Id对应的PKID
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public int GetActivityPKIDByActivityId(Guid activityId)
        {
            var pkid = 0;
            try
            {
                pkid = GungnirReadDb.Execute(conn => DalCityActivityPageConfig.GetActivityPKIDByActivityId(conn, activityId));
            }
            catch (Exception ex)
            {
                Logger.Error("GetActivityPKIDByActivityId", ex);
            }
            return pkid;
        }

        public List<ConfigHistory> GetOprLog(int pkid, DateTime startTime, DateTime endTime)
        {
            OperationResult<IEnumerable<OprLogModel>> oprlist = null;
            using (var client = new OprLogClient())
            {
                oprlist = client.SelectOrderOprLog(pkid, "CVPage");
            }
            var tempList = oprlist.Result.Where(t => t.ChangeDatetime >= startTime && t.ChangeDatetime <= endTime).ToList();
            //AutoMapper初始化配置文件
            var config = new MapperConfiguration(cfg => cfg.CreateMap<OprLogModel, ConfigHistory>());
            var mapper = config.CreateMapper();
            //集合类型转换
            return mapper.Map<IEnumerable<ConfigHistory>>(tempList).ToList();
        }

        /// <summary>
        /// 获取活动页标题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetActivityTitleByUrl(string url)
        {
            var result = string.Empty;
            try
            {
                Regex regex = new Regex("id=[a-zA-Z0-9]+");
                var idPar = regex.Match(url ?? string.Empty)?.Value?.Replace("id=", "")?.Trim();
                if (!string.IsNullOrWhiteSpace(idPar))
                {
                    int id = -1;
                    if (int.TryParse(idPar, out id))
                    {
                        result = GungnirReadDb.Execute(conn => DalCityActivityPageConfig.GetActivityBuildTitle(conn, id));
                    }
                    else
                    {
                        result = GungnirReadDb.Execute(conn => DalCityActivityPageConfig.GetActivePageListTitle(conn, idPar));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetActivityTitleByUrl", ex);
            }
            return result;
        }

        /// <summary>
        /// 一个活动下H5活动页不能重复
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="targetUrl"></param>
        /// <returns></returns>
        public bool IsExistActivityTargetUrl(Guid activityId, string targetUrl)
        {
            var result = true;
            try
            {
                result = ConfigurationReadDb.Execute(conn => DalCityActivityPageConfig.IsExistH5Url(conn, activityId, targetUrl));
            }
            catch (Exception ex)
            {
                Logger.Error("IsExistActivityTargetUrl", ex);
            }
            return result;
        }

        /// <summary>
        /// 一个活动下相同小程序活动页不能重复
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="wxappUrl"></param>
        /// <returns></returns>
        public bool IsExistActivityWxappUrl(Guid activityId, string wxappUrl)
        {
            var result = true;
            try
            {
                result = ConfigurationReadDb.Execute(conn => DalCityActivityPageConfig.IsExistWxUrl(conn, activityId, wxappUrl));
            }
            catch (Exception ex)
            {
                Logger.Error("IsExistActivityWxappUrl", ex);
            }
            return result;
        }
    }
}