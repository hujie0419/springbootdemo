using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Setting
{
    public class SettingManager : ISettingManager
    {
        #region Private Fields

        private static readonly IConnectionManager connectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);

        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("Setting");

        private SettingHandler handler = null;

        public SettingManager()
        {
            handler = new SettingHandler(DbScopeManager);
        }
        #endregion

        public List<CouponActivityModel> SelectAllCouponActivity()
        {
            try
            {
                return handler.SelectAllCouponActivity();
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetCouponSetting");
                throw;
            }
        }

        public CouponActivityModel FetchCouponActivity(Guid activityID)
        {
            try
            {
                return handler.FetchCouponActivity(activityID);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetAllSetting");
                throw;
            }
        }


        public List<CouponActivityModel> SelectAllCouponActivityV1()
        {
            try
            {
                return handler.SelectAllCouponActivityV1();
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "SelectAllCouponActivityV1");
                throw;
            }
        }

        public bool EditSetting(Guid? activityID, CouponActivityModel CASModel)
        {
            try
            {
                if (CASModel != null)
                {
                    return handler.EditSetting(activityID, CASModel);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "EditSetting");
                throw;
            }
        }

        public bool EditSettingV1(Guid? activityID, CouponActivityModel CASModel)
        {
            try
            {
                if (CASModel != null)
                {
                    return handler.EditSettingV1(activityID, CASModel);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "EditSetting");
                throw;
            }
        }

        public bool Delete(Guid activityID)
        {
            try
            {
                return handler.Delete(activityID);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "Delete");
                throw;
            }
        }

        public IEnumerable<Tuple<int, int?, string>> SelectAllPromotionRules()
        {
            return handler.SelectAllPromotionRules();
        }
    }
}
