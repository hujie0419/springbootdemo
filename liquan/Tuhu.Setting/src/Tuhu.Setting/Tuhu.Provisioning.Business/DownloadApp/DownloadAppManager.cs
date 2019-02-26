using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Nosql;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.DownloadApp
{
    public class DownloadAppManager
    {
        #region Private Fields
        private static readonly IConnectionManager ConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(ConnectionManager);

        private readonly DownloadAppHandler handler;

        private static readonly ILog Logger = LoggerFactory.GetLogger("DownloadApp");

        #endregion


        public DownloadAppManager()
        {
            handler = new DownloadAppHandler(DbScopeManager, ConnectionManager);
        }

        public bool InsertDownloadApp(Tuhu.Provisioning.DataAccess.Entity.DownloadApp model)
        {
            try
            {
                return handler.InsertDownloadApp(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertDownloadApp", ex);
                Logger.Log(Level.Error, exception, "InsertDownloadApp");
                throw ex;
            }

        }

        public bool DeleteDownloadApp(int id)
        {
            try
            {
                return handler.DeleteDownloadApp(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "DeleteDownloadApp", ex);
                Logger.Log(Level.Error, exception, "DeleteDownloadApp");
                throw ex;
            }
        }

        public bool UpdateDownloadApp(Tuhu.Provisioning.DataAccess.Entity.DownloadApp model, int id)
        {
            try
            {
                return handler.UpdateDownloadApp(model, id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "UpdateDownloadApp", ex);
                Logger.Log(Level.Error, exception, "UpdateDownloadApp");
                throw ex;
            }
        }

        public Tuhu.Provisioning.DataAccess.Entity.DownloadApp GetDownloadAppById(int id)
        {
            try
            {
                return handler.GetDownloadAppById(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "GetDownloadAppById", ex);
                Logger.Log(Level.Error, exception, "GetDownloadAppById");
                throw ex;
            }

        }

        public List<Tuhu.Provisioning.DataAccess.Entity.DownloadApp> GetDownloadAppList(Tuhu.Provisioning.DataAccess.Entity.DownloadApp model, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return handler.GetDownloadAppList(model, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "GetDownloadAppList", ex);
                Logger.Log(Level.Error, exception, "GetDownloadAppList");
                throw ex;
            }

        }

        public bool InsertBatteryBanner(BatteryBanner model)
        {
            try
            {
                return handler.InsertBatteryBanner(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertBatteryBanner", ex);
                Logger.Log(Level.Error, exception, "InsertBatteryBanner");
                throw ex;
            }

        }

        public List<BatteryBanner> GetBatteryBanner(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return handler.GetBatteryBanner(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "GetBatteryBanner", ex);
                Logger.Log(Level.Error, exception, "GetBatteryBanner");
                throw ex;
            }

        }

        public List<Region> GetRegion(int id)
        {
            try
            {
                return handler.GetRegion(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "GetRegion", ex);
                Logger.Log(Level.Error, exception, "GetRegion");
                throw ex;
            }

        }
        public ConfigApi GetConfigApi(int id)
        {
            try
            {
                return DALConfigApi.GetConfigApi(id);
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "GetConfigApi", ex);
                Logger.Log(Level.Error, exception, "GetConfigApi");
                throw ex;
            }
        }

        public List<ConfigApi> GetConfigApi(ConfigApi model)
        {
            try
            {
                return DALConfigApi.GetConfigApi(model);
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "GetConfigApi", ex);
                Logger.Log(Level.Error, exception, "GetConfigApi");
                throw ex;
            }
        }

        public bool UpdateConfigApi(ConfigApi model)
        {
            try
            {
                return DALConfigApi.UpdateConfigApi(model);
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "UpdateConfigApi", ex);
                Logger.Log(Level.Error, exception, "UpdateConfigApi");
                throw ex;
            }
        }

        public bool DeleteConfigApi(int id)
        {
            try
            {
                return DALConfigApi.DeleteConfigApi(id);
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "DeleteConfigApi", ex);
                Logger.Log(Level.Error, exception, "DeleteConfigApi");
                throw ex;
            }
        }

        public int InsertConfigApi(ConfigApi model)
        {
            try
            {
                return DALConfigApi.InsertConfigApi(model);
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertConfigApi", ex);
                Logger.Log(Level.Error, exception, "InsertConfigApi");
                throw ex;
            }
        }

        public List<FriendLink> GetFriendLink()
        {
            try
            {
                return handler.GetFriendLink();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "GetFriendLink", ex);
                Logger.Log(Level.Error, exception, "GetFriendLink");
                throw ex;
            }
        }

        public bool DeleteFriendLink(int id)
        {
            try
            {
                return handler.DeleteFriendLink(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "DeleteFriendLink", ex);
                Logger.Log(Level.Error, exception, "DeleteFriendLink");
                throw ex;
            }
        }

        public bool UpdateFriendLink(FriendLink model)
        {
            try
            {
                return handler.UpdateFriendLink(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "UpdateFriendLink", ex);
                Logger.Log(Level.Error, exception, "UpdateFriendLink");
                throw ex;
            }
        }

        public int InsertFriendLink(FriendLink model)
        {
            try
            {
                return handler.InsertFriendLink(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertFriendLink", ex);
                Logger.Log(Level.Error, exception, "InsertFriendLink");
                throw ex;
            }
        }

        public int InsertHomePagePopup(HomePagePopup model)
        {
            try
            {
                return handler.InsertHomePagePopup(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertHomepagePopup", ex);
                Logger.Log(Level.Error, exception, "InsertHomepagePopup");
                throw ex;
            }
        }

        public int UpdateHomePagePopup(HomePagePopup model)
        {
            try
            {
                return handler.UpdateHomePagePopup(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "UpdateHomepagePopup", ex);
                Logger.Log(Level.Error, exception, "UpdateHomepagePopup");
                throw ex;
            }
        }

        public List<NoticeChannel> QueryNoticeChannel()
        {
            try
            {
                return  handler.QueryNoticeChannel();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "QueryNoticeChannel", ex);
                Logger.Log(Level.Error, exception, "QueryNoticeChannel");
                throw ex;
            }
        }

        public List<HomePagePopup> QueryHomePagePopup(HomePagePopupQuery model, int page)
        {
            try
            {
                return handler.QueryHomePagePopup(model, page);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "QueryHomePagePopup", ex);
                Logger.Log(Level.Error, exception, "QueryHomePagePopup");
                throw ex;
            }
        }

        public int CountHomePagePopup(HomePagePopupQuery model)
        {
            try
            {
                return handler.CountHomePagePopup(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "CountHomePagePopup", ex);
                Logger.Log(Level.Error, exception, "CountHomePagePopup");
                throw ex;
            }
        }

        public List<BannerConfig> QueryBannerConfig(BannerFilterQuery model,int page)
        {
            try
            {
                return handler.QueryBannerConfig(model, page);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "QueryBannerConfig", ex);
                Logger.Log(Level.Error, exception, "QueryBannerConfig");
                throw ex;
            }
        }

        public int CountBannerConfig(BannerFilterQuery queryModel)
        {
            try
            {
                return handler.CountBannerConfig(queryModel);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "CountBannerConfig", ex);
                Logger.Log(Level.Error, exception, "CountBannerConfig");
                throw ex;
            }
        }

        public bool DeleteBannerConfig(int Id)
        {
            try
            {
                return handler.DeleteBannerConfig(Id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "DeleteBannerConfig", ex);
                Logger.Log(Level.Error, exception, "DeleteBannerConfig");
                throw ex;
            }
        }

        public int InsertBannerConfig(BannerConfig model)
        {
            try
            {
                return handler.InsertBannerConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertBannerConfig", ex);
                Logger.Log(Level.Error, exception, "InsertBannerConfig");
                throw ex;
            }
        }

        public int UpdateBannerConfig(BannerConfig model)
        {
            try
            {
                return handler.UpdateBannerConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "UpdateBannerConfig", ex);
                Logger.Log(Level.Error, exception, "UpdateBannerConfig");
                throw ex;
            }
        }

        public int HasUnOrRecentlyCompletedOrder(string userId)
        {
            try
            {
                var orderCompCriteria = handler.SelectOrderCompJudgeCriteria(userId);
                if (orderCompCriteria != null)
                {
                    if (
                        orderCompCriteria.Any(
                            od =>
                                !string.Equals(od.Status, "7Canceled") && !string.Equals(od.Status, "6Complete") &&
                                ((od.InstallShopId.GetValueOrDefault() > 0 && !string.Equals(od.Status, "3Installed")) ||
                                 (od.InstallShopId.GetValueOrDefault() == 0 &&
                                  (!string.Equals(od.Status, "2Shipped") ||
                                   !string.Equals(od.DeliveryStatus, "3Received") && !string.Equals(od.DeliveryStatus, "3.5Signed"))))))
                        return -3;
                    if (
                        orderCompCriteria.Any(
                            od =>
                                od.InstallDatetime.GetValueOrDefault() > DateTime.Now.AddDays(-7) &&
                                od.InstallShopId.GetValueOrDefault() > 0 && string.Equals(od.Status, "3Installed")
                                 ))
                        return -4;
                    if (
                        orderCompCriteria.Any(
                            od =>
                                od.DeliveryDatetime.GetValueOrDefault() > DateTime.Now.AddDays(-7) &&
                                od.InstallShopId.GetValueOrDefault() == 0 && string.Equals(od.Status, "2Shipped") && string.Equals(od.DeliveryStatus, "3Received")
                                 ))
                        return -4;
                    if (orderCompCriteria.Any(
                        od =>
                            od.DeliveryDatetimeInLog.GetValueOrDefault() > DateTime.Now.AddDays(-7) &&
                            od.InstallShopId.GetValueOrDefault() == 0 && string.Equals(od.Status, "2Shipped") &&
                             string.Equals(od.DeliveryStatusInLog, "3.5Signed")
                                ))
                        return -4;
                }
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "HasUnOrRecentlyCompletedORder", ex);
                Logger.Log(Level.Error, exception, "HasUnOrRecentlyCompletedORder");
                throw ex;
            }
            return 2;
        }

        public ConfigCoupon GetConfigCoupon()
        {
            try
            {
                return handler.GetConfigCoupon();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "GetConfigCoupon", ex);
                Logger.Log(Level.Error, exception, "GetConfigCoupon");
                throw ex;
            }
        }

        public bool InsertConfigCoupon(ConfigCoupon model)
        {
            try
            {
                return handler.InsertConfigCoupon(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertConfigCoupon", ex);
                Logger.Log(Level.Error, exception, "InsertConfigCoupon");
                throw ex;
            }

        }

        public bool UpdateConfigCoupon(ConfigCoupon model)
        {
            try
            {
                return handler.UpdateConfigCoupon(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "UpdateConfigCoupon", ex);
                Logger.Log(Level.Error, exception, "UpdateConfigCoupon");
                throw ex;
            }

        }

        public List<HomePagePopupAnimation> GetAnimation(int pkid)
        {
            var data = handler.GetAnimation(pkid); 
            return data;
        }

        public List<HomePagePopupTargetGroup> GetTargetGroup()
        {
            return handler.GetTargetGroup();
        }


    }
}
