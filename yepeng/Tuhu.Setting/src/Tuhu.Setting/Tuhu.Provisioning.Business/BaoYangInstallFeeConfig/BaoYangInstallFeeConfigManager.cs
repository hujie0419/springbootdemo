using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.BaoYangInstallFeeConfig
{
    public class BaoYangInstallFeeConfigManager
    {
        private static readonly Common.Logging.ILog Logger;
        private static readonly IConnectionManager gungnirConnRo =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager configConnRo =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private static readonly IConnectionManager configReadConnRo =
     new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager gungnirReadConnRo =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
        private static readonly IDBScopeManager dbScopeManagerGungnirRead;
        private static readonly IDBScopeManager dbScopeManagerGungnir;
        private static readonly IDBScopeManager dbScopeManagerConfigRead;
        private static readonly IDBScopeManager dbScopeManagerConfig;

        static BaoYangInstallFeeConfigManager()
        {
            Logger = Common.Logging.LogManager.GetLogger(typeof(BaoYangInstallFeeConfigManager));
            dbScopeManagerGungnir = new DBScopeManager(gungnirConnRo);
            dbScopeManagerGungnirRead = new DBScopeManager(gungnirReadConnRo);
            dbScopeManagerConfig = new DBScopeManager(configConnRo);
            dbScopeManagerConfigRead = new DBScopeManager(configReadConnRo);
        }

        /// <summary>
        /// 获取所有保养项目
        /// </summary>
        /// <returns></returns>
        public List<BaoYangService> GetAllBaoYangServices()
        {
            List<BaoYangService> result = null;
            try
            {
                result = dbScopeManagerGungnirRead.Execute(conn => DalBaoYangInstallFeeConfig.GetAllBaoYangServices(conn));
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllBaoYangServices", ex);
            }
            return result ?? new List<BaoYangService>();
        }

        /// <summary>
        /// 添加或编辑保养项目加价配置
        /// </summary>
        /// <param name="models"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddOrEditBaoYangInstallFeeConfig(List<BaoYangInstallFeeConfigModel> models, string user)
        {
            var result = false;
            try
            {
                var delList = new List<BaoYangInstallFeeConfigModel>();
                var logs = new List<BaoYangOprLog>();
                var serviceId = models.Select(s => s.ServiceId).FirstOrDefault();
                var configs = dbScopeManagerGungnirRead.Execute(conn => DalBaoYangInstallFeeConfig.SelectBaoYangInstallFeeConfig(conn, serviceId));
                var existConfig = configs.Where(s => s.PKID > 0).ToList();
                foreach (var oldValue in existConfig)
                {
                    var sameValue = models.Where(model =>
                          (string.Equals(model.ServiceId, oldValue.ServiceId) && model.CarMinPrice.Equals(oldValue.CarMinPrice)
                              && model.CarMaxPrice.Equals(oldValue.CarMaxPrice) && model.AddInstallFee.Equals(oldValue.AddInstallFee))).FirstOrDefault();
                    if (sameValue == null)
                    {
                        delList.Add(oldValue);//删除旧数据
                    }
                    else
                    {
                        models.Remove(sameValue);//数据没变的不做任何操作
                    }
                }
                dbScopeManagerGungnir.CreateTransaction(conn =>
                {
                    #region 删除旧数据
                    foreach (var oldValue in delList)
                    {
                        var log = new BaoYangOprLog
                        {
                            LogType = "BaoYangInstallFeeConfig",
                            IdentityID = oldValue.PKID.ToString(),
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = null,
                            Remarks = $"删除保养项目:{oldValue.ServiceName}的加价配置",
                            OperateUser = user,
                        };
                        logs.Add(log);
                        DalBaoYangInstallFeeConfig.DeleteBaoYangInstallFeeConfigByPKID(conn, oldValue.PKID);
                    }
                    #endregion

                    #region 插入新数据
                    foreach (var model in models)
                    {
                        var deletedModel = dbScopeManagerGungnirRead.Execute(co => DalBaoYangInstallFeeConfig.GetDeletedBaoYangInstallFeeConfig(co, model));
                        if (deletedModel != null)
                        {
                            var backUpResult = DalBaoYangInstallFeeConfig.BackUpBaoYangInstallFeeConfigByPKID(conn, deletedModel.PKID);
                            if (!backUpResult)
                            {
                                throw new Exception($"BackUpBaoYangInstallFeeConfigByPKID失败,待插入数据{JsonConvert.SerializeObject(deletedModel)}");
                            }
                            var log = new BaoYangOprLog
                            {
                                LogType = "BaoYangInstallFeeConfig",
                                IdentityID = deletedModel.PKID.ToString(),
                                OldValue = null,
                                NewValue = JsonConvert.SerializeObject(deletedModel),
                                Remarks = $"添加保养项目:{model.ServiceName}的加价配置",
                                OperateUser = user,
                            };
                            logs.Add(log);
                        }
                        else
                        {
                            var pkid = DalBaoYangInstallFeeConfig.AddBaoYangInstallFeeConfig(conn, model);
                            if (pkid < 1)
                            {
                                throw new Exception($"AddBaoYangInstallFeeConfig失败,待插入数据{JsonConvert.SerializeObject(model)}");
                            }
                            model.PKID = pkid;
                            model.CreateDateTime = DateTime.Now;
                            model.LastUpdateDateTime = DateTime.Now;
                            var log = new BaoYangOprLog
                            {
                                LogType = "BaoYangInstallFeeConfig",
                                IdentityID = model.PKID.ToString(),
                                OldValue = null,
                                NewValue = JsonConvert.SerializeObject(model),
                                Remarks = $"添加保养项目:{model.ServiceName}的加价配置",
                                OperateUser = user,
                            };
                            logs.Add(log);
                        }
                    }
                    #endregion
                    result = true;
                });
                foreach (var log in logs)
                {
                    LoggerManager.InsertLog("BYOprLog", log);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("AddOrEditBaoYangInstallFeeConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 判断保养项目车型价格区间是否连续
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsPriceLegalBaoYangInstallFeeConfig(List<BaoYangInstallFeeConfigModel> models)
        {
            var isLegal = false;
            try
            {
                decimal lowPrice = 0;
                var sortedModel = models.OrderBy(s => s.CarMinPrice).ToList();
                for (var i = 0; i < sortedModel.Count; i++)
                {
                    if (sortedModel[i].CarMinPrice.Equals(lowPrice) && sortedModel[i].CarMinPrice < sortedModel[i].CarMaxPrice)
                    {
                        lowPrice = sortedModel[i].CarMaxPrice;
                    }
                    else
                    {
                        return isLegal;
                    }
                }
                isLegal = true;
            }
            catch (Exception ex)
            {
                Logger.Error("IsPriceLegalBaoYangInstallFeeConfig", ex);
            }
            return isLegal;
        }

        /// <summary>
        /// 删除保养项目加价配置 --只做逻辑删除
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeleteBaoYangInstallFeeConfig(string serviceId, string user)
        {
            var result = true;
            try
            {
                var logs = new List<BaoYangOprLog>();
                var configs = dbScopeManagerGungnirRead.Execute(conn => DalBaoYangInstallFeeConfig.SelectBaoYangInstallFeeConfig(conn, serviceId));
                var oldValues = configs.Where(s => s.PKID > 0).ToList();
                if (oldValues != null && oldValues.Any())
                {

                    dbScopeManagerGungnir.CreateTransaction(conn =>
                    {
                        foreach (var oldValue in oldValues)
                        {
                            var isDel = DalBaoYangInstallFeeConfig.DeleteBaoYangInstallFeeConfigByPKID(conn, oldValue.PKID);
                            if (!isDel)
                            {
                                throw new Exception($"DeleteBaoYangInstallFeeConfigByPKID失败,待删除数据{JsonConvert.SerializeObject(oldValue)}");
                            }
                            var log = new BaoYangOprLog
                            {
                                LogType = "BaoYangInstallFeeConfig",
                                IdentityID = oldValue.PKID.ToString(),
                                OldValue = JsonConvert.SerializeObject(oldValues),
                                NewValue = null,
                                Remarks = $"删除保养项目:{oldValue.ServiceName}的加价配置",
                                OperateUser = user,
                            };
                            logs.Add(log);
                        }
                    });
                    foreach (var log in logs)
                    {
                        LoggerManager.InsertLog("BYOprLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("DeleteBaoYangInstallFeeConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 查询保养项目加价配置
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public Dictionary<string, List<BaoYangInstallFeeConfigModel>> SelectBaoYangInstallFeeConfig(string serviceId)
        {
            var result = null as Dictionary<string, List<BaoYangInstallFeeConfigModel>>;
            try
            {
                var configs = dbScopeManagerGungnirRead.Execute(conn =>
                      DalBaoYangInstallFeeConfig.SelectBaoYangInstallFeeConfig(conn, serviceId));
                if (configs != null && configs.Any())
                {
                    var configGroup = configs.GroupBy(g => g.ServiceName).ToDictionary(s => s.Key, s => s.ToList());
                    if (configGroup != null)
                    {
                        result = configGroup;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SelectBaoYangInstallFeeConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 刷新保养项目加价服务缓存
        /// </summary>
        /// <returns></returns>
        public bool RefreshBaoYangInstallFeeConfigCache()
        {
            var result = true;
            try
            {
                var serviceIds = GetAllBaoYangServices();
                if (serviceIds != null && serviceIds.Any())
                {
                    var keys = serviceIds.Select(s => $"ServicePrice/BaoYangInstallFeeConfig/{s.ServiceId}");
                    using (var client = new Tuhu.Service.Shop.CacheClient())
                    {
                        var cacheResult = client.RemoveCaches(keys);
                        cacheResult.ThrowIfException(true);
                        result = cacheResult.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("RefreshBaoYangInstallFeeConfigCache", ex);
            }
            return result;
        }

        #region 特殊车型附加安装费开关

        /// <summary>
        /// 获取特殊车型附加安装费开关状态
        /// </summary>
        /// <returns></returns>
        public bool GetVehicleAdditionalPriceSwitchStatus()
        {
            var status = true;
            var switchXml = dbScopeManagerConfigRead.Execute(conn => DalBaoYangInstallFeeConfig.IsExistVehicleAdditionalPriceSwitch(conn));
            if (!string.IsNullOrWhiteSpace(switchXml))
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(switchXml);
                var node = xmlDoc.SelectSingleNode("Switch");
                Boolean.TryParse(node.InnerText, out status);
            }
            return status;
        }

        /// <summary>
        /// 特殊车型附加安装费开关控制
        /// </summary>
        /// <param name="isOpen"></param>
        /// <returns></returns>
        public bool UpdateVehicleAdditionalPriceSwitch(bool isOpen)
        {
            var result = false;
            try
            {
                var switchStatus = dbScopeManagerConfigRead.Execute(conn => DalBaoYangInstallFeeConfig.IsExistVehicleAdditionalPriceSwitch(conn));
                if (string.IsNullOrWhiteSpace(switchStatus))
                {
                    result = dbScopeManagerConfig.Execute(conn => DalBaoYangInstallFeeConfig.AddVehicleAdditionalPriceSwitch(conn, isOpen));
                }
                else
                {
                    result = dbScopeManagerConfig.Execute(conn => DalBaoYangInstallFeeConfig.UpdateVehicleAdditionalPriceSwitch(conn, isOpen));
                }
                if (result)
                {
                    RefreshVehicleAdditionalPriceCache();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateVehicleAdditionalPriceSwitch", ex);
            }
            return result;
        }

        /// <summary>
        /// 清除特殊车型附加安装费服务缓存
        /// </summary>
        /// <returns></returns>
        public bool RefreshVehicleAdditionalPriceCache()
        {
            var key = "VehicleAdditionalPriceSwitch";
            var keys = new List<string>() { key };
            using (var client = new Tuhu.Service.Shop.CacheClient())
            {
                var cacheResult = client.RemoveCaches(keys);
                cacheResult.ThrowIfException(true);
                return cacheResult.Result;
            }
        }
        #endregion
    }
}
