using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Cache;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO.NewCoupon;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.NewCoupon;
using Tuhu.Service.Member;

namespace Tuhu.Provisioning.Business.NewCoupon
{
    public class NewCouponManager
    {
        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;

        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(NewCouponManager));

        private static string LogType = "NewCouponActivity";

        public NewCouponManager()
        {
            this.dbScopeManager = new DBScopeManager(ConnectionManager);
            this.dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
        }

        public List<NewCouponActivity> GetNewCouponConfig(string activityName,Guid activityId, int pageIndex, int pageSize)
        {
            List<NewCouponActivity> result = new List<NewCouponActivity>();

            try
            {
                result = dbScopeReadManager.Execute(conn => DALNewCoupon.SelectNewCouponConfig(conn, activityId, activityName, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }


        public List<SE_GetPromotionActivityCouponInfoConfig> SelectActivityCouponInfo(string rulesIdStr)
        {
            var result = new List<SE_GetPromotionActivityCouponInfoConfig>();

            try
            {
                result = dbScopeReadManager.Execute(conn => DALNewCoupon.SelectActivityCouponInfo(conn, rulesIdStr));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public NewCouponActivity GetNewCouponConfigByActivityId(Guid activityId)
        {
            NewCouponActivity result = new NewCouponActivity();
            List<RecommendActivityConfig> recommendActivity = new List<RecommendActivityConfig>();
            List<CouponRulesConfig> couponRules = new List<CouponRulesConfig>();
            List<SE_GetPromotionActivityCouponInfoConfig> activityCouponInfo = new List<SE_GetPromotionActivityCouponInfoConfig>();

            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    result = DALNewCoupon.SelectNewCouponConfig(conn, activityId, "", 1, 99).FirstOrDefault();
                    recommendActivity = DALNewCoupon.SelectRecommendActivityConfig(conn, activityId);
                    couponRules = DALNewCoupon.SelectCouponRulesConfig(conn, activityId);
                    if (couponRules != null && couponRules.Any())
                    {
                        activityCouponInfo = DALNewCoupon.SelectActivityCouponInfo(conn, string.Join(",", couponRules.Select(x => x.RulesGUID)));
                    }
                });
                if (result != null)
                {
                    result.RecommendActivityForInit = recommendActivity.Where(x => x.ActivityType.Equals(RecommendActivityType.InitActivity)).ToList();
                    result.RecommendActivityForSuccess = recommendActivity.Where(x => x.ActivityType.Equals(RecommendActivityType.SuccessActivity)).ToList();
                    if (activityCouponInfo != null && activityCouponInfo.Any())
                    {
                        result.CouponRulesConfig = (from c in couponRules
                                                    join s in activityCouponInfo
                                                    on c.RulesGUID equals s.GetRuleGUID into temp
                                                    from t in temp.DefaultIfEmpty()
                                                    select new CouponRulesConfig()
                                                    {
                                                        RulesGUID = c.RulesGUID,
                                                        ValidDays = t.ValidDays,
                                                        ValidStartDateTime = t.ValidStartDateTime,
                                                        ValidEndDateTime = t.ValidEndDateTime,
                                                        MinMoney = t.MinMoney,
                                                        Description = t.Description,
                                                        Quantity = t.Quantity,
                                                        SingleQuantity = t.SingleQuantity,
                                                        UserType = t.UserType
                                                    }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public bool UpsertNewConponConfig(NewCouponActivity model, string user)
        {
            var result = false;
            var msg = string.Empty;
            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    var flag = false;
                    if (model != null)
                    {
                        if (model.ActivityId != Guid.Empty)
                        {
                            msg = "更新配置";
                            flag = DALNewCoupon.UpdateNewCouponConfig(conn, model, user) > 0;
                        }
                        else
                        {
                            msg = "添加配置";
                            model.ActivityId = Guid.NewGuid();
                            flag = DALNewCoupon.InsertNewCouponConfig(conn, model, user) > 0;
                        }

                        if (model.RecommendActivityForInit != null && model.RecommendActivityForInit.Any()) { model.AllRecommendActivity.AddRange(model.RecommendActivityForInit); }
                        if (model.RecommendActivityForSuccess != null && model.RecommendActivityForSuccess.Any()) { model.AllRecommendActivity.AddRange(model.RecommendActivityForSuccess); }

                        if (flag)
                        {
                            DALNewCoupon.DeleteRecommendActivityByActivityId(conn, model.ActivityId);
                            DALNewCoupon.DeleteCouponRulesConfig(conn, model.ActivityId);
                            if (model.AllRecommendActivity!=null&& model.AllRecommendActivity.Any())
                            {
                                foreach (var item in model.AllRecommendActivity)
                                {
                                    item.ActivityId = model.ActivityId;
                                    DALNewCoupon.InsertRecommendActivityConfig(conn, item);
                                }
                            }
                            if (model.CouponRulesConfig != null && model.CouponRulesConfig.Any())
                            {
                                foreach (var item in model.CouponRulesConfig)
                                {
                                    item.ActivityId = model.ActivityId;
                                    DALNewCoupon.InsertCouponRulesConfig(conn, item);
                                }
                            }
                        }
                        result = true;
                    }
                });
                if (result)
                {
                    model.AllRecommendActivity = new List<RecommendActivityConfig>();
                    InsertLog("UpsertNewConponConfig", model.ActivityId.ToString(), JsonConvert.SerializeObject(model), msg, user, LogType);
                    Thread.Sleep(2000);
                    RefreshRandomCouponCache(model.ActivityId);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public bool DeleteNewCouponConfigByActivityId(Guid activityId,string user)
        {
            var result = false;

            try
            {
                if (activityId != Guid.Empty)
                {
                    dbScopeManager.CreateTransaction(conn =>
                    {
                        DALNewCoupon.DeleteNewCouponConfigByActivityId(conn, activityId);
                        DALNewCoupon.DeleteRecommendActivityByActivityId(conn, activityId);
                        DALNewCoupon.DeleteCouponRulesConfig(conn, activityId);
                        result = true;
                    });
                    if (result)
                    {
                        InsertLog("DeleteNewCouponConfigByActivityId", activityId.ToString(), "删除配置", "删除配置", user, LogType);
                        Thread.Sleep(2000);
                        RefreshRandomCouponCache(activityId);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public CouponRulesConfig GetCouponRulesInfo(Guid rulesId)
        {
            CouponRulesConfig result = null;

            try
            {
                PromotionActivityManager manager = new PromotionActivityManager();
                var model = manager.GetCouponValidate(rulesId);
                if (model != null)
                {
                    result = new CouponRulesConfig
                    {
                        RulesGUID = model.GetRuleGUID,
                        ValidDays = model.ValidDays,
                        ValidStartDateTime = model.ValidStartDateTime,
                        ValidEndDateTime = model.ValidEndDateTime,
                        Description = model.Description,
                        MinMoney = model.MinMoney,
                        SingleQuantity = model.SingleQuantity,
                        Quantity = model.Quantity,
                        UserType = model.UserType
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public bool RefreshRandomCouponCache(Guid activityId)
        {
            var result = false;

            try
            {
                using (var client = new PromotionClient())
                {
                    var getResult = client.RefreshRandomPromotionCacheByKey(activityId);
                    if (!getResult.Success && getResult.Exception != null)
                        throw getResult.Exception;
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public bool IsExistRandomId(string randomGroupId)
        {
            var result = false;

            try
            {
                result = dbScopeReadManager.Execute(conn => DALNewCoupon.IsExistRandomId(conn, randomGroupId.Trim())) > 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }
         

        #region 日志
        /// <summary>
        /// 插入日志
        /// </summary>
        /// <param name="method"></param>
        /// <param name="objectId"></param>
        /// <param name="remarks"></param>
        /// <param name="msg"></param>
        /// <param name="opera"></param>
        /// <param name="type"></param>
        public static void InsertLog(string method, string objectId, string remarks, string msg, string opera, string type)
        {
            try
            {
                CouponActivityLog info = new CouponActivityLog
                {
                    ObjectId = objectId,
                    Method = method,
                    Message = msg,
                    Remarks = remarks,
                    Operator = opera,
                    Type = type.Trim(),
                    CreatedTime = DateTime.Now,
                    LastUpdateDateTime = DateTime.Now
                };
                LoggerManager.InsertLog("CouponActivityLog", info);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<CouponActivityLog> SelectOperationLog(string objectId)
        {
            List<CouponActivityLog> result = new List<CouponActivityLog>();
            try
            {
                result = DALNewCoupon.SelectOperationLog(objectId, LogType);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
        #endregion
    }
}
