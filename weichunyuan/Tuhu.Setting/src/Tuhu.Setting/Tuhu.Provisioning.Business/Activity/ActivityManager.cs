using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft.Json.Linq;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;

namespace Tuhu.Provisioning.Business.Activity
{
    public class ActivityManager
    {
        #region Private Fields  
        private static readonly IConnectionManager ConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(ConnectionManager);

        private readonly IDBScopeManager DbGungnirManager = null;
        private readonly IDBScopeManager DbGungnirReadOnlyManager = null;

        private readonly ActivityHandler handler;
        private readonly ActivityHandler handlerReadonly;
        private static readonly Common.Logging.ILog Logger = Common.Logging.LogManager.GetLogger(typeof(ActivityManager));

        #endregion

        public ActivityManager()
        {
            string gungnirConnStr = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string gungnirReadOnlyConnStr = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(gungnirConnStr))
            {
                gungnirConnStr = SecurityHelp.DecryptAES(gungnirConnStr);
            }
            if (SecurityHelp.IsBase64Formatted(gungnirReadOnlyConnStr))
            {
                gungnirReadOnlyConnStr = SecurityHelp.DecryptAES(gungnirReadOnlyConnStr);
            }

            DbGungnirManager = new DBScopeManager(new ConnectionManager(gungnirConnStr));
            DbGungnirReadOnlyManager = new DBScopeManager(new ConnectionManager(gungnirReadOnlyConnStr));
            handler = new ActivityHandler(DbScopeManager, ConnectionManager);
            handlerReadonly = new ActivityHandler(DbScopeManager,new ConnectionManager(gungnirReadOnlyConnStr));
        }
        public bool InsertActivityBuild(ActivityBuild model)
        {
            try
            {
                return handler.InsertActivityBuild(model);
            }
            catch (Exception ex)
            {
                var exception = new ActivityException(1, "InsertActivityBuild", ex);
                Logger.Error("InsertActivityBuild", exception);
                throw ex;
            }

        }

        public bool DeleteActivityBuild(int id)
        {
            try
            {
                return handler.DeleteActivityBuild(id);
            }
            catch (Exception ex)
            {
                var exception = new ActivityException(1, "DeleteActivityBuild", ex);
                Logger.Error("DeleteActivityBuild", exception);
                throw ex;
            }
        }

        public int CreateActivity(string title)
        {
            return handler.CreateActivity(title);
        }


        public bool UpdateActivityBuild(ActivityBuild model, int id)
        {
            try
            {
                return handler.UpdateActivityBuild(model, id);
            }
            catch (Exception ex)
            {
                var exception = new ActivityException(1, "UpdateActivityBuild", ex);
                Logger.Error("UpdateActivityBuild", exception);
                throw ex;
            }
        }

        public int GetMaxID()
        {
            return handler.GetMaxID();
        }


        public ActivityBuild GetActivityBuildById(int id)
        {
            try
            {
                return handlerReadonly.GetActivityBuildById(id);
            }
            catch (Exception ex)
            {
                var exception = new ActivityException(1, "GetActivityBuildById", ex);
                Logger.Error("GetActivityBuildById", exception);
                throw ex;
            }

        }

        public List<ActivityBuild> GetActivityBuild(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return handlerReadonly.GetActivityBuild(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (Exception ex)
            {
                var exception = new ActivityException(1, "GetActivityBuild", ex);
                Logger.Error("GetActivityBuild", exception);
                throw ex;
            }

        }

        public bool SaveCouponActivityConfig(CouponActivity configObject, string userName)
        {
            bool result = false;
            try
            {
                result = DbGungnirManager.Execute(conn => handler.SaveCouponActivityConfig(conn, configObject, userName));
            }
            catch (Exception ex)
            {
                var exception = new ActivityException(1, "SaveCouponActivityConfig", ex);
                Logger.Error("SaveCouponActivityConfig", exception);
                throw ex;
            }
            return result;
        }

        public bool SaveCouponActivityWebConfig(WebCouponActivity configObject, string userName)
        {
            bool result = false;
            try
            {
                if (configObject != null)
                {
                    configObject.PromotionDescription = string.Empty;
                    configObject.PromotionCodeChannel = string.Empty;
                    result = DbGungnirManager.Execute(conn => handler.SaveCouponActivityWebConfig(conn, configObject, userName));
                    if (result)
                    {
                        var key = configObject.ActivityId == -1 ? configObject.ActivityKey?.ToString() : configObject.ActivityId.ToString();
                        using (var client = new Service.Config.CacheClient())
                        {
                            var serviceResult = client.RemoveRedisCacheKey("Config1", $"WebCouponActivity/{key}");
                            serviceResult.ThrowIfException(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var exception = new ActivityException(1, "SaveCouponActivityWebConfig", ex);
                Logger.Error("SaveCouponActivityWebConfig", exception);
                throw ex;
            }
            return result;
        }

        public CouponActivity GetCurrentCouponActivity(string id)
        {
            CouponActivity result = null;
            try
            {
                result = DbGungnirReadOnlyManager.Execute(conn => handler.GetCurrentCouponActivity(conn, id)) ?? new CouponActivity()
                {
                    ActivityId = -1,
                    ActivityKey = new Guid(Guid.NewGuid().ToString("N")),
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today
                };
            }
            catch (Exception ex)
            {
                var exception = new ActivityException(1, "GetCurrentCouponActivity", ex);
                Logger.Error("GetCurrentCouponActivity", exception);
                throw ex;
            }

            return result;
        }

        public WebCouponActivity GetCurrentWebCouponActivity(string id)
        {
            WebCouponActivity result = null;
            try
            {
                result = DbGungnirReadOnlyManager.Execute(conn => handler.GetCurrentWebCouponActivity(conn, id)) ?? new WebCouponActivity()
                {
                    ActivityId = -1,
                    ActivityKey = new Guid(Guid.NewGuid().ToString("N")),
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today
                };
            }
            catch (Exception ex)
            {
                var exception = new ActivityException(1, "GetCurrentWebCouponActivity", ex);
                Logger.Error("GetCurrentWebCouponActivity", exception);
            }

            return result;
        }

        public Tuple<int, List<CouponActivity>> GetActivityList(string type, int pageSize, int pageIndex)
        {
            Tuple<int, List<CouponActivity>> result = null;

            try
            {
                result = DbGungnirReadOnlyManager.Execute(conn => handler.GetActivityList(conn, type, pageSize, pageIndex));
            }
            catch (Exception ex)
            {
                var exception = new ActivityException(1, "GetActivityList", ex);
                Logger.Error("GetActivityList", exception);
                throw ex;
            }
            return result;
        }

        public bool DeleteActivityConfig(string type, string id, string userName)
        {
            bool result = false;
            try
            {
                result = DbGungnirManager.Execute(conn => handler.DeleteActivityConfig(conn, type, id, userName));
            }
            catch (Exception ex)
            {
                var exception = new ActivityException(1, "DeleteActivityConfig", ex);
                Logger.Error("DeleteActivityConfig", exception);
                throw ex;
            }
            return result;
        }


        public Dictionary<string, string> GetProductImageUrl(string pid)
        {
            Dictionary<string, string> dic = DataAccess.DAO.DALActivity.GetProductImageUrl(pid);
            if (dic.Count > 0)
            {
                return dic;
            }
            else
            {
                return null;
            }


        }


        public Dictionary<int, string> GetActivity_Coupon(string activityID)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            Guid ActivityID = new Guid();
            if (!Guid.TryParse(activityID, out ActivityID))
            {
                return dictionary;
            }
            System.Data.DataTable dt = DataAccess.DAO.DALActivity.GetActivity_Coupon(activityID);
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                dictionary.Add(int.Parse(dr["PKID"].ToString()), dr["Instructions"].ToString());
            }
            return dictionary;
        }

        /// <summary>
        /// 校验优惠券
        /// </summary>
        /// <param name="couponRulePKID"></param>
        /// <returns></returns>
        public string CouponVlidate(string couponRulePKID)
        {
            System.Data.DataTable dt = DataAccess.DAO.DALActivity.CouponVlidate(couponRulePKID);

            JObject json = new JObject();
            if (dt != null && dt.Rows.Count > 0)
            {
                json.Add("RuleID", dt.Rows[0]["RuleID"].ToString());
                json.Add("Name", dt.Rows[0]["PromtionName"].ToString());
                json.Add("Description", dt.Rows[0]["Description"].ToString());
                json.Add("Minmoney", dt.Rows[0]["Minmoney"].ToString());
                json.Add("Discount", dt.Rows[0]["Discount"].ToString());
                json.Add("CouponStartTime", dt.Rows[0]["ValiStartDate"].ToString());
                json.Add("CouponEndTime", dt.Rows[0]["ValiEndDate"].ToString());
                json.Add("CouponDuration", dt.Rows[0]["Term"].ToString());
                json.Add("Quantity", dt.Rows[0]["Quantity"].ToString());
                json.Add("PKID", dt.Rows[0]["PKID"].ToString());
                json.Add("GetRuleGUID", dt.Rows[0]["GetRuleGUID"].ToString());
            }
            else
            {
                json.Add("RuleID", "");
                json.Add("Name", "");
                json.Add("Description", "");
                json.Add("Minmoney", "");
                json.Add("Discount", "");
                json.Add("CouponStartTime", "");
                json.Add("CouponEndTime", "");
                json.Add("CouponDuration", "");
                json.Add("Quantity", "");
                json.Add("PKID", "");
                json.Add("GetRuleGUID", "");
            }

            return json.ToString();
        }

        public string CouponVlidateForPKID(int pkid)
        {
            System.Data.DataTable dt = DataAccess.DAO.DALActivity.CouponVlidateForPKID(pkid);

            JObject json = new JObject();
            if (dt != null && dt.Rows.Count > 0)
            {
                json.Add("RuleID", dt.Rows[0]["RuleID"].ToString());
                json.Add("Name", dt.Rows[0]["PromtionName"].ToString());
                json.Add("Description", dt.Rows[0]["Description"].ToString());
                json.Add("Minmoney", dt.Rows[0]["Minmoney"].ToString());
                json.Add("Discount", dt.Rows[0]["Discount"].ToString());
                json.Add("CouponStartTime", dt.Rows[0]["ValiStartDate"].ToString());
                json.Add("CouponEndTime", dt.Rows[0]["ValiEndDate"].ToString());
                json.Add("CouponDuration", dt.Rows[0]["Term"].ToString());
                json.Add("Quantity", dt.Rows[0]["Quantity"].ToString());
                json.Add("SupportUserRange", dt.Rows[0]["SupportUserRange"].ToString());
            }
            else
            {
                json.Add("RuleID", "");
                json.Add("Name", "");
                json.Add("Description", "");
                json.Add("Minmoney", "");
                json.Add("Discount", "");
                json.Add("CouponStartTime", "");
                json.Add("CouponEndTime", "");
                json.Add("CouponDuration", "");
                json.Add("Quantity", "");
                json.Add("SupportUserRange","");
            }

            return json.ToString();
        }

        public static GetPCodeModel GetPCodeModelByRulePKID(int pkid)
        {
            var result = new GetPCodeModel();

            System.Data.DataTable dt = DataAccess.DAO.DALActivity.CouponVlidateForPKID(pkid);
            if (dt != null && dt.Rows.Count > 0)
            {
                var firstRow = dt.Rows[0];
                result.GETPKID = firstRow.IsNull("PKID") ? -1 : Convert.ToInt32(firstRow["PKID"]);
                result.RuleID = firstRow.IsNull("RuleID") ? -1 : Convert.ToInt32(firstRow["RuleID"]);
                result.PromtionName = firstRow.IsNull("PromtionName") ? string.Empty : firstRow["PromtionName"].ToString();
                result.Description = firstRow.IsNull("Description") ? string.Empty : firstRow["Description"].ToString();
                result.Minmoney = firstRow.IsNull("Minmoney") ? 0 : Convert.ToDecimal(firstRow["Minmoney"]);
                result.Discount = firstRow.IsNull("Discount") ? 0 : Convert.ToDecimal(firstRow["Discount"]);
                result.ValiStartDate = firstRow.IsNull("ValiStartDate") ? (DateTime?)null : 
                    Convert.ToDateTime(firstRow["ValiStartDate"]);
                result.ValiEndDate = firstRow.IsNull("ValiEndDate") ? (DateTime?)null : 
                    Convert.ToDateTime(firstRow["ValiEndDate"]);
                result.Term = firstRow.IsNull("Term") ? 0 : Convert.ToInt32(firstRow["Term"]);
                result.Quantity = firstRow.IsNull("Quantity") ? 0 : Convert.ToInt32(firstRow["Quantity"]);
                result.SupportUserRange = firstRow.IsNull("SupportUserRange") ? 0 : Convert.ToInt32(firstRow["SupportUserRange"]);
            }

            return result;
        }

        public static Dictionary<int,GetPCodeModel> GetPCodeModelByRulePKIDS(IEnumerable<int> pkids)
        {
            

            System.Data.DataTable dt = DataAccess.DAO.DALActivity.CouponVlidateForPKIDS(pkids);
            if (dt != null && dt.Rows.Count > 0)
            {
                var dic = new Dictionary<int, GetPCodeModel>(dt.Rows.Count);
                foreach (DataRow dr in dt.Rows)
                {
                    var result = new GetPCodeModel();
                    var firstRow = dr;
                    result.GETPKID = firstRow.IsNull("PKID") ? -1 : Convert.ToInt32(firstRow["PKID"]);
                    result.RuleID = firstRow.IsNull("RuleID") ? -1 : Convert.ToInt32(firstRow["RuleID"]);
                    result.PromtionName = firstRow.IsNull("PromtionName") ? string.Empty : firstRow["PromtionName"].ToString();
                    result.Description = firstRow.IsNull("Description") ? string.Empty : firstRow["Description"].ToString();
                    result.Minmoney = firstRow.IsNull("Minmoney") ? 0 : Convert.ToDecimal(firstRow["Minmoney"]);
                    result.Discount = firstRow.IsNull("Discount") ? 0 : Convert.ToDecimal(firstRow["Discount"]);
                    result.ValiStartDate = firstRow.IsNull("ValiStartDate") ? (DateTime?)null :
                        Convert.ToDateTime(firstRow["ValiStartDate"]);
                    result.ValiEndDate = firstRow.IsNull("ValiEndDate") ? (DateTime?)null :
                        Convert.ToDateTime(firstRow["ValiEndDate"]);
                    result.Term = firstRow.IsNull("Term") ? 0 : Convert.ToInt32(firstRow["Term"]);
                    result.Quantity = firstRow.IsNull("Quantity") ? 0 : Convert.ToInt32(firstRow["Quantity"]);
                    result.SupportUserRange = firstRow.IsNull("SupportUserRange") ? 0 : Convert.ToInt32(firstRow["SupportUserRange"]);
                    if (!dic.ContainsKey(result.GETPKID))
                    {
                        dic.Add(result.GETPKID, result);
                    }
                    return dic;
                }
            }

            return new Dictionary<int, GetPCodeModel>();
        }


        public string CouponVlidate1(string couponRulePKID)
        {
            System.Data.DataTable dt = DataAccess.DAO.DALActivity.CouponVlidate1(couponRulePKID);

            JObject json = new JObject();
            if (dt != null && dt.Rows.Count > 0)
            {
                json.Add("RuleID", dt.Rows[0]["RuleID"].ToString());
                json.Add("Name", dt.Rows[0]["PromtionName"].ToString());
                json.Add("Description", dt.Rows[0]["Description"].ToString());
                json.Add("Minmoney", dt.Rows[0]["Minmoney"].ToString());
                json.Add("Discount", dt.Rows[0]["Discount"].ToString());
                json.Add("CouponStartTime", dt.Rows[0]["ValiStartDate"].ToString());
                json.Add("CouponEndTime", dt.Rows[0]["ValiEndDate"].ToString());
                json.Add("CouponDuration", dt.Rows[0]["Term"].ToString());
                json.Add("Quantity", dt.Rows[0]["Quantity"].ToString());
                //json += dt.Rows[0]["PromtionName"].ToString() + "\",\"Description\":\"" + dt.Rows[0]["Description"].ToString() + "\"}";
            }
            else
            {
                json.Add("RuleID", "");
                json.Add("Name", "");
                json.Add("Description", "");
                json.Add("Minmoney", "");
                json.Add("Discount", "");
                json.Add("CouponStartTime", "");
                json.Add("CouponEndTime", "");
                json.Add("CouponDuration", "");
                json.Add("Quantity", "");
            }

            return json.ToString();
        }

        public List<string> GetMiaoSha()
        {
            System.Data.DataTable dt = DataAccess.DAO.DALActivity.GetMiaoSha();
            if (dt != null && dt.Rows.Count > 0)
            {
                List<string> list = new List<string>();
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    list.Add(dr[0].ToString());
                }
                return list;
            }
            else
            {
                return null;
            }
        }

        #region 活动看板
        public List<ActivityBuild> GetActivityDetailsForActivityBoard(DateTime start, DateTime end, string title, string createdUser, string owner)
        {
            List<ActivityBuild> result = new List<ActivityBuild>();

            try
            {
                if (start != null && end != null)
                {
                    result = DbGungnirReadOnlyManager.Execute(conn => handler.SelectActivityDetailsForActivityBoard(conn, start, end, title, createdUser, owner));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetActivityBuildDetailsForActivityBoard", ex);
            }

            return result;
        }
        #endregion

        public void ReloadActivity(string host, int id)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
               var result =  client.GetAsync("https://"+host+"/Activity/ReloadEntity?id=" + id);
                var r = result?.Result;
            }
        }


        public WebCouponActivityRuleModel GetCouponRule(Guid ruleGUID)
        {
            WebCouponActivityRuleModel result = null;
            if (ruleGUID != Guid.Empty)
            {
                try
                {
                    result = DbGungnirReadOnlyManager.Execute(conn => handler.SelectCouponRule(conn, ruleGUID));
                }
                catch (Exception ex)
                {
                    Logger.Error("GetCouponRule", ex);
                }
            }
            return result;
        }


        /// <summary>
        /// 根据 大优惠券的 pkid 获取 所有的 可用的小优惠券
        /// </summary>
        /// <param name="pkids"></param>
        /// <returns></returns>
        public List<WebCouponActivityRuleModel> SelectCouponRuleByCouponRulesPKID(List<int> pkids)
        {
            List<WebCouponActivityRuleModel> models = new List<WebCouponActivityRuleModel>();
            try
            {
                models = DbGungnirReadOnlyManager.Execute(conn => handler.SelectCouponRuleByCouponRulesPKID(conn, pkids));
            }
            catch (Exception ex)
            {
                models = null;
                Logger.Error("GetCouponRule", ex);
            }
            return models;
        }
    }
}
