using System;
using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Setting
{
    public class SettingHandler
    {
        #region Private Fields
        private readonly IDBScopeManager dbManager;
        #endregion

        #region Ctor
        public SettingHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }
        #endregion

        public List<CouponActivityModel> SelectAllCouponActivity()
        {
            return dbManager.Execute(connection => DalSetting_CouponSetting.SelectAllCouponActivity(connection));
        }

        public CouponActivityModel FetchCouponActivity(Guid activityID)
        {
            return dbManager.Execute(connection => DalSetting_CouponSetting.FetchCouponActivity(connection, activityID));
        }


        public List<CouponActivityModel> SelectAllCouponActivityV1()
        {
            return dbManager.Execute(connection => DalSetting_CouponSetting.SelectAllCouponActivityV1(connection));
        }

        public bool EditSetting(Guid? activityID, CouponActivityModel CASModel)
        {
            if (CASModel != null)
                return dbManager.Execute(connection => DalSetting_CouponSetting.EditSetting(connection, activityID, CASModel));
            else
                return false;
        }

        public bool EditSettingV1(Guid? activityID, CouponActivityModel CASModel)
        {
            if (CASModel != null)
                return dbManager.Execute(connection => DalSetting_CouponSetting.EditSettingV1(connection, activityID, CASModel));
            else
                return false;
        }

        public bool Delete(Guid activityID)
        {
            return dbManager.Execute(connection => DalSetting_CouponSetting.Delete(connection, activityID));
        }

        public IEnumerable<Tuple<int, int?, string>> SelectAllPromotionRules()
        {
            return dbManager.Execute(connection => DalSetting_CouponSetting.SelectAllPromotionRules(connection));
        }
    }
}
