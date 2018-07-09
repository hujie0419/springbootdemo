using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.Activity
{
    public class ActivityHandler
    {
        private readonly IConnectionManager connectionManager;
        private readonly IDBScopeManager dbManager;

        public ActivityHandler(IDBScopeManager dbManager, IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
            this.dbManager = dbManager;
        }

        public bool InsertActivityBuild(ActivityBuild model)
        {
            return dbManager.Execute(conn => DALActivity.InsertActivityBuild(conn, model));

        }

        public bool UpdateActivityBuild(ActivityBuild model, int id)
        {
            return dbManager.Execute(conn => DALActivity.UpdateActivityBuild(conn, model, id));
        }

        public int GetMaxID()
        {
            return dbManager.Execute(conn =>DALActivity.GetMaxID(conn));
        }

        public bool DeleteActivityBuild(int id)
        {
            return dbManager.Execute(conn => DALActivity.DeleteActivityBuild(conn, id));
        }

        public  int CreateActivity(string title)
        {
            return dbManager.Execute(conn => DALActivity.CreateActivity(conn, title));
        }

        public ActivityBuild GetActivityBuildById(int id)
        {
            return dbManager.Execute(conn => DALActivity.GetActivityBuildById(conn, id));
        }     

        public List<ActivityBuild> GetActivityBuild(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            strConn = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
            SqlConnection conn = new SqlConnection(strConn);
            return DALActivity.GetActivityBuild(conn, sqlStr, pageSize, pageIndex, out recordCount);
        }

        public bool SaveCouponActivityConfig(SqlConnection conn,CouponActivity configObject, string userName)
        {
            var result = false;
            int rows=DALActivity.SaveCouponActivityConfig(conn,configObject, userName);
            if (rows > 0)
            {
                result = true;
            }
            return result;
        }

        public bool SaveCouponActivityWebConfig(SqlConnection conn,WebCouponActivity configObject, string userName)
        {
            var result = false;
            int rows = DALActivity.SaveCouponActivityWebConfig(conn,configObject,userName);
            if (rows > 0)
            {
                result = true;
            }
            return result;

        }

        public CouponActivity GetCurrentCouponActivity(SqlConnection conn,string id)
        {
       
               return   DALActivity.GetCurrentCouponActivity(conn, id);

        }

        public WebCouponActivity GetCurrentWebCouponActivity(SqlConnection conn, string id)
        {
            int activityId;
            WebCouponActivity activity = null;
            if (int.TryParse(id, out activityId))
            {
                activity = DALActivity.GetCurrentWebCouponActivityByActivityId(conn, activityId);
            }
            else
            {
                var activityKey = id;
                activity = DALActivity.GetCurrentWebCouponActivityByActivityKey(conn, activityKey);
            }
            return activity;
        }

        public Tuple<int, List<CouponActivity>> GetActivityList(SqlConnection conn, string type, int pageSize, int pageIndex)
        {
            type = type?.ToLower();
            switch (type)
            {
                case "app":
                    return DALActivity.GetActivityListForApp(conn, pageSize, pageIndex);
                case "web":
                    return DALActivity.GetActivityListForWeb(conn, pageSize, pageIndex);
                default:
                    return Tuple.Create(0, new List<CouponActivity>());
            }
        }

        public bool DeleteActivityConfig(SqlConnection conn,string type,string id, string userName)
        {
            return  DALActivity.DeleteActivityConfig(conn, type, id, userName);
        }

        public List<ActivityBuild> SelectActivityDetailsForActivityBoard(SqlConnection conn, DateTime start, DateTime end, string title, string createdUser, string owner)
        {
            return DALActivity.SelectActivityDetailsForActivityBoard(conn, start, end, title, createdUser, owner);
        }


        public WebCouponActivityRuleModel SelectCouponRule(SqlConnection conn, Guid GetRuleGUID)
        {
            return DALActivity.SelectCouponRule(conn, GetRuleGUID);
        }

        /// <summary>
        /// 批量 获取 优惠券
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkids"></param>
        /// <returns></returns>
        public List<WebCouponActivityRuleModel> SelectCouponRuleByCouponRulesPKID(SqlConnection conn, List<int> pkids)
        {
            return DALActivity.SelectCouponRuleByCouponRulesPKID(conn, pkids);
        }
    }
}
