using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class BeautyHomePageConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("BeautyHomePageConfig");

        private static readonly Common.Logging.ILog _logger = Common.Logging.LogManager.GetLogger(typeof(BeautyHomePageConfigManager));

        #region Connection

        private static string strConn = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;

        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Tuhu_Groupon_AlwaysOnRead"].ConnectionString;

        private static readonly IDBScopeManager Tuhu_GrouponDBScopeManager = new DBScopeManager(new ConnectionManager(strConn));

        private static readonly IDBScopeManager Tuhu_GrouponReadOnlyDBScopeManager = new DBScopeManager(new ConnectionManager(strConnOnRead));

        #endregion


        public List<BeautyHomePageConfig> GetBeautyHomePageConfigList(BeautyHomePageConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALBeautyHomePageConfig.GetBeautyHomePageConfigList(model, pageSize, pageIndex, out recordCount);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetBeautyHomePageConfigList");
                throw ex;
            }
        }

        public BeautyHomePageConfig GetBeautyHomePageConfigById(int id)
        {
            try
            {
                return DALBeautyHomePageConfig.GetBeautyHomePageConfig(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetBeautyHomePageConfig");
                throw ex;
            }
        }

        public bool UpdateBeautyHomePageConfig(BeautyHomePageConfig model)
        {
            try
            {
                return DALBeautyHomePageConfig.UpdateBeautyHomePageConfig(model);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "UpdateBeautyHomePageConfig");
                throw ex;
            }

        }

        public bool InsertBeautyHomePageConfig(BeautyHomePageConfig model, ref int id)
        {
            try
            {
                return DALBeautyHomePageConfig.InsertBeautyHomePageConfig(model, ref id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "InsertBeautyHomePageConfig");
                throw ex;
            }
        }
        public bool DeleteBeautyHomePageConfig(int id)
        {
            try
            {
                return DALBeautyHomePageConfig.DeleteBeautyHomePageConfig(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "DeleteBeautyHomePageConfig");
                throw ex;
            }
        }

        public IEnumerable<string> GetBeautyChannel()
        {
            IEnumerable<string> result = null;
            try
            {
                result = DALBeautyHomePageConfig.GetBeautyChannel();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result ?? new List<string>();
        }

        public IEnumerable<BeautyHomePageConfig> GetShopMapConfigs()
        {
            var result = null as List<BeautyHomePageConfig>;
            try
            {
                result = Tuhu_GrouponReadOnlyDBScopeManager.Execute(conn => DALBeautyHomePageConfig.SelectShopMapConfigs(conn)).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }

        public IEnumerable<BeautyHomePageConfig> SelectBeautyBannerConfig(int status, string keyword, int pageIndex, int pageSize, out int count)
        {
            IEnumerable<BeautyHomePageConfig> result = new List<BeautyHomePageConfig>();
            int counts = 0;
            try
            {
                result = DALBeautyHomePageConfig.SelectBeautyBannerConfig(status, keyword, pageIndex, pageSize, out counts);
            }
            catch (Exception ex)
            {
                _logger.Error($"SelectBeautyBannerConfig:{ex.Message}", ex);
            }
            count = counts;
            return result;
        }
        public bool UpdateBeautyHomePageBannerConfig(BeautyHomePageConfig model)
        {
            try
            {
                return DALBeautyHomePageConfig.UpdateBeautyHomePageBannerConfig(model);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "UpdateBeautyHomePageBannerConfig");
                throw ex;
            }

        }

        public bool InsertBeautyHomePageBannerConfig(BeautyHomePageConfig model, ref int id)
        {
            try
            {
                return DALBeautyHomePageConfig.InsertBeautyHomePageBannerConfig(model, ref id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "InsertBeautyHomePageBannerConfig");
                throw ex;
            }
        }

        public IEnumerable<BeautyPopUpWindowsConfig> SelectBeautyPopUpConfig(int status, string keyword, int pageIndex, int pageSize, out int count)
        {
            IEnumerable<BeautyPopUpWindowsConfig> result = new List<BeautyPopUpWindowsConfig>();
            int counts = 0;
            try
            {
                result = DALBeautyHomePageConfig.SelectBeautyPopUpConfigs(status, keyword, pageIndex, pageSize, out counts);
            }
            catch (Exception ex)
            {
                _logger.Error($"SelectBeautyPopUpConfig:{ex.Message}", ex);
            }
            count = counts;
            return result;
        }

        public BeautyPopUpWindowsConfig GetBeautyPopUpWindowsConfigById(int pkid)
        {
            try
            {
                return DALBeautyHomePageConfig.GetBeautyPopUpWindowsConfig(pkid);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetBeautyPopUpWindowsConfigById");
                throw ex;
            }
        }
        public IEnumerable<BeautyPopUpWindowsRegionModel> GetBeautyPopUpWindowsRegionConfigs(int BeautyPopUpId)
        {
            try
            {
                return DALBeautyHomePageConfig.GetBeautyPopUpWindowsRegionConfigs(BeautyPopUpId);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetBeautyPopUpWindowsRegionConfigs");
                throw ex;
            }
        }
        public bool SaveBeautyPopUpWindowsConfig(BeautyPopUpWindowsConfig model, string userName)
        {
            var result = false;
            try
            {
                var log = new BeautyOprLog
                {
                    LogType = "SaveBeautyPopUpWindowsConfig",
                    IdentityID = $"{model.PKID}",
                    OldValue = null,
                    NewValue = Newtonsoft.Json.JsonConvert.SerializeObject(model),
                    OperateUser = userName,
                    Remarks = $"新增美容弹窗配置",
                };
                if (model.PKID <= 0)
                {
                    int pkid = 0;
                    result = DALBeautyHomePageConfig.InsertBeautyPopUpConfig(model, out pkid);

                    log.IdentityID = pkid.ToString();
                }
                else
                {
                    var oldModel = GetBeautyPopUpWindowsConfigById(model.PKID);
                    if (oldModel.IsRegion)
                        oldModel.RegionList = GetBeautyPopUpWindowsRegionConfigs(oldModel.PKID)?.ToList();
                    log.OldValue = Newtonsoft.Json.JsonConvert.SerializeObject(oldModel);
                    log.Remarks = $"更新美容弹窗配置";

                    result = DALBeautyHomePageConfig.UpdateBeautyPopUpConfig(model);
                }
                if (result)
                {
                    LoggerManager.InsertLog("BeautyOprLog", log);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "SaveBeautyPopUpWindowsConfig");
                throw ex;
            }
            return result;
        }

        public bool DeleteBeautyPopUpWindowsConfig(int pkid)
        {
            return DALBeautyHomePageConfig.DeleteBeautyPopUpConfig(pkid);
        }

        public IEnumerable<BeautyCategorySimple> GetBeautyCategoryByChannel(string channel)
        {
            return DALBeautyHomePageConfig.GetBeautyCategorysByChannel(channel);
        }
    }
}
