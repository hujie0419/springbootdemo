using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ShopsManagement
{
    public class ShopsManager
    {
        #region Private Fields
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);

        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);

        private static readonly ILog logger = LoggerFactory.GetLogger("Shops");

        private ShopsHandler handler = null;

        public ShopsManager()
        {
            handler = new ShopsHandler(DbScopeManager);
        }
        #endregion

        //根据门店ID得到门店名称
        public string GetSimpleNameByShopID(int ShopID)
        {
            return handler.GetSimpleNameByShopID(ShopID);
        }

        public List<ShopAppBanner> GetShopAppBannerList()
        {
            try
            {
                return handler.GetShopAppBannerList();
            }
            catch (Exception ex)
            {
                var exception = new ShopsException(0, "查看App首页图片配置失败！", ex);
                logger.Log(Level.Error, exception, "Error in GetShopAppBannerList.");
                throw exception;
            }
        }

        public ShopAppBanner GetAppShopBannerByPKID(int _PKID)
        {
            try
            {
                return handler.GetAppShopBannerByPKID(_PKID);
            }
            catch (Exception ex)
            {
                var exception = new ShopsException(0, "更加ID查看App首页图片配置失败！", ex);
                logger.Log(Level.Error, exception, "Error in GetAppShopBannerByPKID.");
                throw exception;
            }
        }

        public int DeleteShopBannerByPKID(int PKID)
        {
            try
            {
                return handler.DeleteShopBannerByPKID(PKID);
            }
            catch (Exception ex)
            {
                var exception = new ShopsException(0, "删除App首页图片配置失败！", ex);
                logger.Log(Level.Error, exception, "Error in DeleteShopBannerByPKID.");
                throw exception;
            }
        }

        public int InsertShopBanner(ShopAppBanner banner)
        {
            try
            {
                return handler.InsertShopBanner(banner);
            }
            catch (Exception ex)
            {
                var exception = new ShopsException(0, "新增App首页图片配置失败！", ex);
                logger.Log(Level.Error, exception, "Error in InsertShopBanner.");
                throw exception;
            }
        }


        public int UpdateShopBanner(ShopAppBanner banner)
        {
            try
            {
                return handler.UpdateShopBanner(banner);
            }
            catch (Exception ex)
            {
                var exception = new ShopsException(0, "修改App首页图片配置失败！", ex);
                logger.Log(Level.Error, exception, "Error in UpdateShopBanner.");
                throw exception;
            }
        }

    }
}