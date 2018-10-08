using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.DownloadApp
{
    public class DownloadAppHandler
    {
        private readonly IConnectionManager connectionManager;
        private readonly IDBScopeManager dbManager;

        private static string strConn = SecurityHelp.IsBase64Formatted(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString) ? SecurityHelp.DecryptAES(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString) : ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;

        private SqlConnection conn = new SqlConnection(strConn);
        public DownloadAppHandler(IDBScopeManager dbManager, IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
            this.dbManager = dbManager;
        }

        public bool InsertDownloadApp(Tuhu.Provisioning.DataAccess.Entity.DownloadApp model)
        {
            return dbManager.Execute(conn => DALDownloadApp.InsertDownloadApp(conn, model));

        }

        public bool UpdateDownloadApp(Tuhu.Provisioning.DataAccess.Entity.DownloadApp model, int id)
        {
            return dbManager.Execute(conn => DALDownloadApp.UpdateDownloadApp(conn, model, id));
        }

        public bool DeleteDownloadApp(int id)
        {
            return dbManager.Execute(conn => DALDownloadApp.DeleteDownloadApp(conn, id));
        }

        public Tuhu.Provisioning.DataAccess.Entity.DownloadApp GetDownloadAppById(int id)
        {
            return dbManager.Execute(conn => DALDownloadApp.GetDownloadAppById(conn, id));
        }

        public List<Tuhu.Provisioning.DataAccess.Entity.DownloadApp> GetDownloadAppList(Tuhu.Provisioning.DataAccess.Entity.DownloadApp model, int pageSize, int pageIndex, out int recordCount)
        {
            string strConn = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
            strConn = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
            SqlConnection conn = new SqlConnection(strConn);
            return DALDownloadApp.GetDownloadAppList(conn, model, pageSize, pageIndex, out recordCount);
        }

        public bool InsertBatteryBanner(BatteryBanner model)
        {
            return dbManager.Execute(conn => DALDownloadApp.InsertBatteryBanner(conn, model));

        }

        public List<BatteryBanner> GetBatteryBanner(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string strConn = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
            strConn = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
            SqlConnection conn = new SqlConnection(strConn);
            return DALDownloadApp.GetBatteryBanner(conn, sqlStr, pageSize, pageIndex, out recordCount);
        }
        public List<Region> GetRegion(int id)
        {
            return DALDownloadApp.GetRegion(conn, id);
        }    
  
        public List<FriendLink> GetFriendLink()
        {
            return DALFriendLink.GetFrindLink(conn);
        }

        public bool DeleteFriendLink(int id)
        {
            return DALFriendLink.DeleteFriendLink(conn, id);
        }

        public bool UpdateFriendLink(FriendLink model)
        {
            return DALFriendLink.UpdateFriendLink(conn, model);
        }

        public int InsertFriendLink(FriendLink model)
        {
            return DALFriendLink.InsertFriendLink(conn, model);
        }

        public int InsertHomePagePopup(HomePagePopup model)
        {
            return DalHomePagePopup.InsertHomePagePopup(conn, model);
        }

        public int UpdateHomePagePopup(HomePagePopup model)
        {
            return DalHomePagePopup.UpdateHomePagePopup(conn, model);
        }

        public List<NoticeChannel> QueryNoticeChannel()
        {
            return DalHomePagePopup.QueryNoticeChannel(conn);
        }

        public List<HomePagePopup> QueryHomePagePopup(HomePagePopupQuery model, int page)
        {
            return DalHomePagePopup.QueryHomePagePopup(conn, model,page);
        }
        public List<HomePagePopupAnimation> GetAnimation(int pkid)
        {
            return DalHomePagePopup.GetAnimation(conn,pkid);
        }
        public List<HomePagePopupTargetGroup> GetTargetGroup()
        {
            return DalHomePagePopup.GetTargetGroup(conn);
        }

        public int CountHomePagePopup(HomePagePopupQuery model)
        {
            return DalHomePagePopup.CountHomePagePopup(conn, model);
        }

        public List<BannerConfig> QueryBannerConfig(BannerFilterQuery queryModel, int page)
        {
            return DalBannerConfig.QueryBannerConfig(conn, queryModel, page);
        }

        public int CountBannerConfig(BannerFilterQuery queryModel)
        {
            return DalBannerConfig.CountBannerConfig(conn, queryModel);
        }

        public bool DeleteBannerConfig(int Id)
        {
            return DalBannerConfig.DeleteBannerConfig(conn, Id);
        }

        public int InsertBannerConfig(BannerConfig model)
        {
            return DalBannerConfig.InsertBannerConfig(conn, model);
        }

        public int UpdateBannerConfig(BannerConfig model)
        {
            return DalBannerConfig.UpdateBannerConfig(conn, model);
        }

        public List<OrderCompletionJudgeCriteria> SelectOrderCompJudgeCriteria(string userId)
        {
            return DalMobileBindUnbind.SelectOrderCompJudgeCriteria(userId);
        }

        public ConfigCoupon GetConfigCoupon()
        {
            return DALConfigApi.GetConfigCoupon(conn);
        }

        public bool UpdateConfigCoupon(ConfigCoupon model)
        {
            return DALConfigApi.UpdateConfigCoupon(conn, model);
        }

        public bool InsertConfigCoupon(ConfigCoupon model)
        {
            return DALConfigApi.InsertConfigCoupon(conn, model);
        }

    }
}
