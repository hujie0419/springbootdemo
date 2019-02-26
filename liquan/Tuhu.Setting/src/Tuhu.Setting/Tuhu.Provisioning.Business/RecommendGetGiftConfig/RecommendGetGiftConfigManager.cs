using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class RecommendGetGiftConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("RecommendGetGiftConfig");

        public List<RecommendGetGiftConfig> GetRecommendGetGiftConfigList(RecommendGetGiftConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALRecommendGetGiftConfig.GetRecommendGetGiftConfig(model, pageSize, pageIndex, out recordCount);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetRecommendGetGiftConfigList");
                throw ex;
            }
        }

        public RecommendGetGiftConfig GetRecommendGetGiftConfigById(int id)
        {
            try
            {
                return DALRecommendGetGiftConfig.GetRecommendGetGiftConfigById(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetRecommendGetGiftConfigById");
                throw ex;
            }
        }

        public bool UpdateRecommendGetGiftConfig(RecommendGetGiftConfig model)
        {
            try
            {
                var temp = GetRecommendGetGiftConfigById(model.Id);
                if (temp != null)
                {
                    if (temp.StartTime < DateTime.Now||(temp.StartTime==null&&temp.EndTime==null))//已经开始了
                    {
                        model.IsSendCode = null;
                        model.UserGroupId = null;
                        model.StartTime = null;
                        model.EndTime = temp.EndTime == null ? null : model.EndTime;
                    }
                }
                return DALRecommendGetGiftConfig.UpdateRecommendGetGiftConfig(model);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "UpdateRecommendGetGiftConfig");
                throw ex;
            }

        }

        public bool InsertRecommendGetGiftConfig(RecommendGetGiftConfig model, ref int id)
        {
            try
            {
                return DALRecommendGetGiftConfig.InsertRecommendGetGiftConfig(model, ref id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "InsertRecommendGetGiftConfig");
                throw ex;
            }
        }
        public bool DeleteRecommendGetGiftConfig(int id)
        {
            try
            {
                return DALRecommendGetGiftConfig.DeleteRecommendGetGiftConfig(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "DeleteRecommendGetGiftConfig");
                throw ex;
            }
        }

        public string GetCoupon(string guidOrId)
        {
            System.Data.DataTable dt = DALRecommendGetGiftConfig.GetCoupon(guidOrId);

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
    }
}
