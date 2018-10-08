using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.CouponManagement
{
    public class CouponHandler
    {
        #region Private Fields
        private readonly IDBScopeManager dbManager;
        #endregion

        #region Ctor
        public CouponHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }
        #endregion

        #region CouponCategory
        public List<CouponCategory> GetAllCouponCategory()
        {
            Func<SqlConnection, List<CouponCategory>> action = (connection) => DalCoupon.GetAllCouponCategory(connection);
            return dbManager.Execute(action);
        }

        public void DeleteCouponCategory(int id)
        {
            Action<SqlConnection> action = (connection) => DalCoupon.DeleteCouponCategory(connection, id);
            dbManager.Execute(action);
        }

        public void AddCouponCategory(CouponCategory couponCategory)
        {
            Action<SqlConnection> action = (connection) => DalCoupon.AddCouponCategory(connection, couponCategory);
            dbManager.Execute(action);
        }

        public void UpdateCouponCategory(CouponCategory couponCategory)
        {
            Action<SqlConnection> action = (connection) => DalCoupon.UpdateCouponCategory(connection, couponCategory);
            dbManager.Execute(action);
        }

        public void UpdateCouponCategoryPercentage(int id, int perc)
        {
            Action<SqlConnection> action = (connection) => DalCoupon.UpdateCouponCategoryPercentage(connection, id, perc);
            dbManager.Execute(action);
        }

        public CouponCategory GetCouponCategoryByID(int id)
        {
            Func<SqlConnection, CouponCategory> action = (connection) => DalCoupon.GetCouponCategoryByID(connection, id);
            return dbManager.Execute(action);
        }

        public int GetPKIDByEnID(string EnID)
        {
            Func<SqlConnection, int> action = (connection) => DalCoupon.GetPKIDByEnID(connection, EnID);
            return dbManager.Execute(action);
        }
        #endregion

        #region Coupon
        public List<Coupon> GetCouponByCategoryID(int CategoryID)
        {
            Func<SqlConnection, List<Coupon>> action = (connection) => DalCoupon.GetCouponByCategoryID(connection, CategoryID);
            return dbManager.Execute(action);
        }
        public string GetCountByCategoryID(int CategoryID)
        {
            Func<SqlConnection, string> action = (connection) => DalCoupon.GetCountByCategoryID(connection, CategoryID);
            return dbManager.Execute(action);
        }
        public void DeleteCoupon(int PKID)
        {
            Action<SqlConnection> action = (connection) => DalCoupon.DeleteCoupon(connection, PKID);
            dbManager.Execute(action);
        }
        public void UpdateCoupon(Coupon coupon)
        {
            Action<SqlConnection> action = (connection) => DalCoupon.UpdateCoupon(connection, coupon);
            dbManager.Execute(action);
        }
        public Coupon GetCouponByID(int id)
        {
            Func<SqlConnection, Coupon> action = (connection) => DalCoupon.GetCouponByID(connection, id);
            return dbManager.Execute(action);
        }
        public void AddCoupon(Coupon coupon)
        {
            Action<SqlConnection> action = (connection) => DalCoupon.AddCoupon(connection, coupon);
            dbManager.Execute(action);
        }
        public DataTable SelectDropDownList()
        {
            Func<SqlConnection, DataTable> action = (connection) => DalCoupon.SelectDropDownList(connection);
            return dbManager.Execute(action);
        }
        #endregion
    }
}
