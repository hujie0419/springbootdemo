using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using static Tuhu.Provisioning.DataAccess.Entity.PeccancyQueryModel;

namespace Tuhu.Provisioning.Business.PeccancyQueryConfig
{
    public class PeccancyQueryConfigManager
    {
        private static readonly Common.Logging.ILog Logger;
        private static readonly string grconnectionString;
        static PeccancyQueryConfigManager()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            grconnectionString = SecurityHelp.IsBase64Formatted(conn) ? SecurityHelp.DecryptAES(conn) : conn;
            Logger = Common.Logging.LogManager.GetLogger(typeof(PeccancyQueryConfigManager));
        }

        #region 违章查询省份配置
        /// <summary>
        /// 添加违章查询省份配置
        /// </summary>
        /// <param name="province"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddPeccancyProvinceConfig(PeccancyQueryProvinceModel province, string user)
        {
            var result = false;
            try
            {
                using (var dbhelper = new Tuhu.Component.Common.SqlDbHelper(grconnectionString))
                {
                    result = DalPeccancyQueryConfig.AddPeccancyProvinceConfig(dbhelper, province);
                }
                if (result)
                {
                    var log = new PeccancyConfigOprLogModel
                    {
                        LogType = "PeccancyProvinceConfig",
                        IdentityId = $"{province.ProvinceId}",
                        OperationType = "Add",
                        OldValue = null,
                        NewValue = JsonConvert.SerializeObject(province),
                        Remarks = $"添加 违章查询省份:{province.ProvinceName} 的配置",
                        Operator = user,
                    };
                    LoggerManager.InsertLog("PeccancyConfigOprLog", log);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("AddPeccancyProvinceConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 更新违章查询省份配置
        /// </summary>
        /// <param name="province"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdatePeccancyProvinceConfig(PeccancyQueryProvinceModel province, string user)
        {
            var result = false;
            try
            {
                var oldValue = DalPeccancyQueryConfig.GetPeccancyProvinceConfigByProvinceId(province.ProvinceId);
                if (oldValue != null)
                {
                    using (var dbhelper = new Tuhu.Component.Common.SqlDbHelper(grconnectionString))
                    {
                        result = DalPeccancyQueryConfig.UpdatePeccancyProvinceConfig(dbhelper, province);
                    }
                    if (result)
                    {
                        var log = new PeccancyConfigOprLogModel
                        {
                            LogType = "PeccancyProvinceConfig",
                            IdentityId = $"{province.ProvinceId}",
                            OperationType = "Update",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = JsonConvert.SerializeObject(province),
                            Remarks = $"更新 违章查询省份:{province.ProvinceName} 的配置",
                            Operator = user,
                        };
                        LoggerManager.InsertLog("PeccancyConfigOprLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UpdatePeccancyProvinceConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 删除违章查询省份配置
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public bool DeletePeccancyProvinceConfig(int provinceId, string user)
        {
            var result = false;
            try
            {
                using (var dbhelper = new Tuhu.Component.Common.SqlDbHelper(grconnectionString))
                {
                    var oldValue = DalPeccancyQueryConfig.GetPeccancyProvinceConfigByProvinceId(provinceId);
                    if (oldValue != null)
                    {
                        var cityConfigs = DalPeccancyQueryConfig.GetPeccancyCitiesByProvinceId(provinceId);
                        if (cityConfigs != null && cityConfigs.Any())
                        {
                            dbhelper.BeginTransaction();
                            var deleteProvince = DalPeccancyQueryConfig.DeletePeccancyCityConfigByProvinceId(dbhelper, provinceId);
                            var deleteCity = DalPeccancyQueryConfig.DeletePeccancyProvinceConfig(dbhelper, provinceId);//同时删除该省份下的所有城市配置
                            if (deleteProvince && deleteCity)
                            {
                                dbhelper.Commit();
                                result = true;
                                #region 删除的省份和城市记录日志
                                var logs = new List<PeccancyConfigOprLogModel>();
                                var provinceLog = new PeccancyConfigOprLogModel
                                {
                                    LogType = "PeccancyProvinceConfig",
                                    IdentityId = $"{oldValue.ProvinceId}",
                                    OperationType = "Delete",
                                    OldValue = JsonConvert.SerializeObject(oldValue),
                                    NewValue = null,
                                    Remarks = $"删除 违章查询省份:{oldValue.ProvinceName} 的配置",
                                    Operator = user,
                                };
                                logs.Add(provinceLog);
                                foreach (var city in cityConfigs)
                                {
                                    var cityLog = new PeccancyConfigOprLogModel
                                    {
                                        LogType = "PeccancyCityConfig",
                                        IdentityId = $"{city.CityId}",
                                        OperationType = "Delete",
                                        OldValue = JsonConvert.SerializeObject(city),
                                        NewValue = null,
                                        Remarks = $"删除 违章查询城市:{city.CityName} 的配置",
                                        Operator = user,
                                    };
                                    logs.Add(cityLog);
                                }
                                if (logs.Any())
                                {
                                    logs.ForEach(s => LoggerManager.InsertLog("PeccancyConfigOprLog", s));
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            result = DalPeccancyQueryConfig.DeletePeccancyProvinceConfig(dbhelper, provinceId);
                            if (result)
                            {
                                var log = new PeccancyConfigOprLogModel
                                {
                                    LogType = "PeccancyProvinceConfig",
                                    IdentityId = $"{oldValue.ProvinceId}",
                                    OperationType = "Delete",
                                    OldValue = JsonConvert.SerializeObject(oldValue),
                                    NewValue = null,
                                    Remarks = $"删除 违章查询省份:{oldValue.ProvinceName} 的配置",
                                    Operator = user,
                                };
                                LoggerManager.InsertLog("PeccancyConfigOprLog", log);
                            }
                        }
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("DeletePeccancyProvinceConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 查询违章查询省份配置
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<List<PeccancyQueryProvinceModel>, int> SelectPeccancyProvinceConfig(int provinceId, int pageIndex, int pageSize)
        {
            var result = new Tuple<List<PeccancyQueryProvinceModel>, int>(null, 0);
            try
            {
                result = DalPeccancyQueryConfig.SelectPeccancyProvinceConfig(provinceId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                Logger.Error("SelectPeccancyProvinceConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 省份配置是否重复
        /// </summary>
        /// <param name="peccancyProvinceModel"></param>
        /// <returns></returns>
        public PeccancyQueryProvinceModel GetRepeatPeccancyProvinceConfig(PeccancyQueryProvinceModel peccancyProvinceModel)
        {
            PeccancyQueryProvinceModel result = null;
            try
            {
                result = DalPeccancyQueryConfig.GetRepeatPeccancyProvinceConfig(peccancyProvinceModel);
            }
            catch (Exception ex)
            {
                Logger.Error("GetRepeatPeccancyProvinceConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取该省份下的城市配置数量
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public int GetPeccancyCityConfigCountByPrvinceId(int provinceId)
        {
            var cityCount = -1;
            try
            {
                cityCount = DalPeccancyQueryConfig.GetPeccancyCityConfigCountByPrvinceId(provinceId);
            }
            catch (Exception ex)
            {
                Logger.Error("GetPeccancyCityConfigCountByPrvinceId", ex);
            }
            return cityCount;
        }
        #endregion

        /// <summary>
        /// 获取所有配置的省份
        /// </summary>
        /// <returns></returns>
        public List<PeccancyRegionMiniModel> GetAllPeccancyProvinces()
        {
            List<PeccancyRegionMiniModel> result = null;
            try
            {
                result = DalPeccancyQueryConfig.GetAllPeccancyProvinces();
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllPeccancyProvinces", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取该省份下的城市--下拉框选项
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public List<PeccancyRegionMiniModel> GetPeccancyCitiesByProvinceId(int provinceId)
        {
            List<PeccancyRegionMiniModel> result = null;
            try
            {
                var configs = DalPeccancyQueryConfig.GetPeccancyCitiesByProvinceId(provinceId);
                result = configs?.Select(s => new PeccancyRegionMiniModel { RegionId = s.CityId, RegionName = s.CityName }).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error("GetPeccancyCitiesByProvinceId", ex);
            }
            return result;
        }


        #region 违章查询城市配置

        /// <summary>
        /// 添加违章查询城市配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddPeccancyCityConfig(PeccancyQueryCityModel model, string user)
        {
            var result = false;
            try
            {
                using (var dbhelper = new Tuhu.Component.Common.SqlDbHelper(grconnectionString))
                {
                    result = DalPeccancyQueryConfig.AddPeccancyCityConfig(dbhelper, model);
                    if (result)
                    {
                        var log = new PeccancyConfigOprLogModel
                        {
                            LogType = "PeccancyCityConfig",
                            IdentityId = $"{model.CityId}",
                            OperationType = "Add",
                            OldValue = null,
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"添加 违章查询城市:{model.CityName} 的配置",
                            Operator = user,
                        };
                        LoggerManager.InsertLog("PeccancyConfigOprLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("AddPeccancyCityConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 更新违章查询城市配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdatePeccancyCityConfig(PeccancyQueryCityModel model, string user)
        {
            var result = false;
            try
            {
                var oldValue = DalPeccancyQueryConfig.GetPeccancyCityConfigByCityId(model.CityId);
                if (oldValue != null)
                {
                    using (var dbhelper = new Tuhu.Component.Common.SqlDbHelper(grconnectionString))
                    {
                        result = DalPeccancyQueryConfig.UpdatePeccancyCityConfig(dbhelper, model);
                    }
                    if (result)
                    {
                        var log = new PeccancyConfigOprLogModel
                        {
                            LogType = "PeccancyCityConfig",
                            IdentityId = $"{model.CityId}",
                            OperationType = "Update",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"更新 违章查询城市:{model.CityName} 的配置",
                            Operator = user,
                        };
                        LoggerManager.InsertLog("PeccancyConfigOprLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UpdatePeccancyCityConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 删除违章查询城市配置
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public bool DeletePeccancyCityConfig(int cityId, string user)
        {
            var result = false;
            try
            {
                var oldValue = DalPeccancyQueryConfig.GetPeccancyCityConfigByCityId(cityId);
                if (oldValue != null)
                {
                    using (var dbhelper = new Tuhu.Component.Common.SqlDbHelper(grconnectionString))
                    {
                        result = DalPeccancyQueryConfig.DeletePeccancyCityConfigByCityId(dbhelper, cityId);
                    }
                    if (result)
                    {
                        var log = new PeccancyConfigOprLogModel
                        {
                            LogType = "PeccancyCityConfig",
                            IdentityId = $"{oldValue.CityId}",
                            OperationType = "Delete",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = null,
                            Remarks = $"删除 违章查询城市:{oldValue.CityName} 的配置",
                            Operator = user,
                        };
                        LoggerManager.InsertLog("PeccancyConfigOprLog", log);
                    }
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("DeletePeccancyCityConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 查询违章查询城市配置
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<List<PeccancyQueryCityModel>, int> SelectPeccancyCityConfig(int provinceId, int cityId, int pageIndex, int pageSize)
        {
            var result = new Tuple<List<PeccancyQueryCityModel>, int>(null, 0);
            try
            {
                result = DalPeccancyQueryConfig.SelectPeccancyCityConfig(provinceId, cityId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                Logger.Error("SelectPeccancyCityConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 根据省份Id获取违章查询省份配置
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public PeccancyQueryProvinceModel GetPeccancyProvinceConfigByProvinceId(int provinceId)
        {
            PeccancyQueryProvinceModel model = null;
            try
            {
                model = DalPeccancyQueryConfig.GetPeccancyProvinceConfigByProvinceId(provinceId);
            }
            catch (Exception ex)
            {
                Logger.Error("GetPeccancyProvinceConfigByProvinceId", ex);
            }
            return model;
        }

        /// <summary>
        /// 根据城市Id获取违章查询城市配置
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public PeccancyQueryCityModel GetPeccancyCityConfigByCityId(int cityId)
        {
            PeccancyQueryCityModel peccancyCityModel = null;
            try
            {
                peccancyCityModel = DalPeccancyQueryConfig.GetPeccancyCityConfigByCityId(cityId);
            }
            catch (Exception ex)
            {
                Logger.Error("GetPeccancyCityConfigByCityId", ex);
            }
            return peccancyCityModel;
        }
        #endregion

        #region 清除服务缓存
        /// <summary>
        /// 刷新违章查询城市服务缓存
        /// </summary>
        /// <returns></returns>
        public bool CleanPeccancyCitysCache()
        {
            var result = false;
            try
            {
                using (var client = new Tuhu.Service.Config.CacheClient())
                {
                    var cacheResult = client.UpdatePeccancyCitysCache();
                    cacheResult.ThrowIfException(true);
                    result = cacheResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CleanPeccancyCitysCache", ex);
            }
            return result;
        }
        #endregion

        #region 查看操作日志
        /// <summary>
        /// 查看违章配置操作日志
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="identityId"></param>
        /// <returns></returns>
        public List<PeccancyConfigOprLogModel> SelectPeccancyConfigOprLog(string logType, string identityId)
        {
            var result = null as List<PeccancyConfigOprLogModel>;
            try
            {
                result = DalPeccancyQueryConfig.SelectPeccancyConfigOprLog(logType, identityId);
            }
            catch (Exception ex)
            {
                Logger.Error("SelectPeccancyConfigOprLog", ex);
            }
            return result;
        }

        /// <summary>
        /// 查看违章配置操作记录详情
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public PeccancyConfigOprLogModel GetPeccancyConfigOprLog(int pkid)
        {
            var result = null as PeccancyConfigOprLogModel;
            try
            {
                result = DalPeccancyQueryConfig.GetPeccancyConfigOprLog(pkid);
            }
            catch (Exception ex)
            {
                Logger.Error("GetPeccancyConfigOprLog", ex);
            }
            return result;
        }
        #endregion

    }
}
