using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.ActivityCalendar;
using Tuhu.Provisioning.Business.CouponManagement;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class CouponManager : ICouponManager
    {
        #region Private Fields
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);

        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("Coupon");

        private CouponHandler handler = null;

        #endregion

        public CouponManager()
        {
            handler = new CouponHandler(DbScopeManager);
        }

        #region CouponCategory
        public List<CouponCategory> GetAllCouponCategory()
        {
            return handler.GetAllCouponCategory();
        }

        public void DeleteCouponCategory(int id)
        {
            handler.DeleteCouponCategory(id);
        }

        public void AddCouponCategory(CouponCategory couponCategory)
        {
            handler.AddCouponCategory(couponCategory);
        }

        public void UpdateCouponCategory(CouponCategory couponCategory)
        {
            handler.UpdateCouponCategory(couponCategory);
        }
        public void UpdateCouponCategoryPercentage(int id, int perc)
        {
            handler.UpdateCouponCategoryPercentage(id, perc);
        }

        public CouponCategory GetCouponCategoryByID(int id)
        {
            return handler.GetCouponCategoryByID(id);
        }
        public int GetPKIDByEnID(string EnID)
        {
            return handler.GetPKIDByEnID(EnID);
        }
        #endregion

        #region Coupon
        public Coupon GetCouponByID(int id)
        {
            return handler.GetCouponByID(id);
        }

        public DataTable SelectDropDownList()
        {
            return handler.SelectDropDownList();
        }

        public List<Coupon> GetCouponByCategoryID(int CategoryID)
        {
            return handler.GetCouponByCategoryID(CategoryID);
        }
        public string GetCountByCategoryID(int CategoryID)
        {
            return handler.GetCountByCategoryID(CategoryID);
        }
        public void DeleteCoupon(int PKID)
        {
            handler.DeleteCoupon(PKID);
        }
        public void UpdateCoupon(Coupon coupon)
        {
            handler.UpdateCoupon(coupon);
        }

        public void AddCoupon(Coupon coupon)
        {
            handler.AddCoupon(coupon);
        }
        #endregion

        public void SelectDataForActivityFromCouponCatagory()
        {
            string pkidStr = string.Empty;
            //获取活动表中已有的套餐信息
            List<DataAccess.Entity.ActivityCalendar> listAc = new ActivityCalendarManager().SelectActivityByCondition(string.Empty).Where(_ => _.DataFrom.EndsWith(ActivityObject.CouponCategory.ToString())).ToList();
            //拼接已录入的活动信息,将来在套餐信息表中排除
            if (listAc.Any())
            {
                pkidStr = listAc.Where(_ => _.DataFromId != null).Aggregate(pkidStr, (current, item) => current + (item.DataFromId.ToString() + ','));
                pkidStr = pkidStr.Substring(0, pkidStr.Length - 1);
            }

            #region 没有的数据要添加

            //获取套餐信息表中新增的套餐信息
            //获取套餐信息表中新增的套餐信息
            List<CouponCategory> listZaConfigue;
            if (string.IsNullOrEmpty(pkidStr))
            {
                listZaConfigue = GetAllCouponCategory();
            }
            else
            {
                listZaConfigue = GetAllCouponCategory().FindAll(delegate (CouponCategory info)
                {
                    if (!pkidStr.Split(',').Contains(info.PKID.ToString(CultureInfo.InvariantCulture)))
                    {
                        return true;
                    }
                    return false;
                });
            }



            //向活动日历信息表添加数据
            foreach (var item in listZaConfigue)
            {
                var modelAc = new DataAccess.Entity.ActivityCalendar
                {
                    BeginDate = Convert.ToDateTime("1901-01-01"),
                    ActivityTitle = item.Name,
                    ActivityContent = item.Remark,
                    CreateDate = DateTime.Now,
                    CreateBy = "SYSTEM",
                    DataFrom = ActivityObject.CouponCategory.ToString(),
                    DataFromId = item.PKID,
                    IsActive = true
                };
                new ActivityCalendarManager().AddActivityCalendar(modelAc);
            }
            #endregion

            #region
            //获取套餐信息表中新增的套餐信息
            var updateList = GetAllCouponCategory().FindAll(delegate (CouponCategory info)
            {
                if (pkidStr.Split(',').Contains(info.PKID.ToString(CultureInfo.InvariantCulture)))
                {
                    return true;
                }
                return false;
            });
            foreach (var item in updateList)
            {
                new ActivityCalendarManager().UpdateIsActivity(item.PKID, ActivityObject.CouponCategory.ToString(), item.IsActive == 1 ? true : false);
            }
            #endregion
        }
    }
}
