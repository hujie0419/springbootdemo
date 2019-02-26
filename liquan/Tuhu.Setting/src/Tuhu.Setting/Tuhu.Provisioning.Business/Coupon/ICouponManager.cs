using System.Collections.Generic;
using System.Data;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public interface ICouponManager
    {
        #region CouponCategory
        List<CouponCategory> GetAllCouponCategory();
        void DeleteCouponCategory(int id);
        void AddCouponCategory(CouponCategory couponCategory);
        void UpdateCouponCategory(CouponCategory couponCategory);
        void UpdateCouponCategoryPercentage(int id, int perc);
        CouponCategory GetCouponCategoryByID(int id);
        int GetPKIDByEnID(string EnID);
        #endregion

        #region Coupon
        Coupon GetCouponByID(int id);
        List<Coupon> GetCouponByCategoryID(int CategoryID);
        string GetCountByCategoryID(int CategoryID);
        void DeleteCoupon(int PKID);
        void UpdateCoupon(Coupon coupon);
        void AddCoupon(Coupon coupon);
        DataTable SelectDropDownList();
        #endregion
    }
}
