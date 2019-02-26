using System;
using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.Setting
{
    public interface ISettingManager
    {
        List<CouponActivityModel> SelectAllCouponActivity();

        CouponActivityModel FetchCouponActivity(Guid activityID);

        List<CouponActivityModel> SelectAllCouponActivityV1();

        bool EditSetting(Guid? activityID, CouponActivityModel CASModel);

        bool EditSettingV1(Guid? activityID, CouponActivityModel CASModel);

        bool Delete(Guid activityID);

        IEnumerable<Tuple<int, int?, string>> SelectAllPromotionRules();
    }
}
