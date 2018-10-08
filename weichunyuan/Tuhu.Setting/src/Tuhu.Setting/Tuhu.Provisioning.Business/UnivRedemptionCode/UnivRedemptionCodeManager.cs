using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.DataAccess.DAO.BeautyServicePackageDal;
using Tuhu.Provisioning.DataAccess.DAO.UnivRedemptionCode;
using Tuhu.Provisioning.DataAccess.Entity.UnivRedemptionCode;
using Tuhu.Service.GroupBuying.Models.RedemptionCode;

namespace Tuhu.Provisioning.Business.UnivRedemptionCode
{
    public class UnivRedemptionCodeManager
    {
        private readonly IDBScopeManager DbTuhuGrouponScopeManager = null;
        private readonly IDBScopeManager DbTuhuGrouponScopeReadManager = null;
        private readonly IDBScopeManager DbTuhuLogScopeReadManager = null;

        private readonly IDBScopeManager DbTuhuGunginrScopeManager = null;
        private readonly IDBScopeManager DbTuhuGunginrScopeReadManager = null;
        private static Common.Logging.ILog Logger = LogManager.GetLogger(typeof(UnivRedemptionCodeManager));

        #region ctor

        private static readonly IConnectionManager TuhuGrouponConnectionManager =
           new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString);
        private static readonly IConnectionManager TuhuGrouponConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_Groupon_ReadOnly"].ConnectionString);
        private static readonly IConnectionManager TuhuLogConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString);

        private static readonly IConnectionManager TuhuGunginrConnectionManager =
   new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager TuhuGunginrConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_ReadOnly"].ConnectionString);

        public UnivRedemptionCodeManager()
        {
            this.DbTuhuGrouponScopeManager = new DBScopeManager(TuhuGrouponConnectionManager);
            this.DbTuhuGrouponScopeReadManager = new DBScopeManager(TuhuGrouponConnectionReadManager);
            this.DbTuhuLogScopeReadManager = new DBScopeManager(TuhuLogConnectionManager);

            this.DbTuhuGunginrScopeManager = new DBScopeManager(TuhuGunginrConnectionManager);
            this.DbTuhuGunginrScopeReadManager = new DBScopeManager(TuhuGunginrConnectionReadManager);
        }

        #endregion

        #region redemptionCodeConfig

        private const string RedemptionCodeConfigType = "RedemptionCodeConfig";

        /// <summary>
        /// 分页获取兑换码配置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Tuple<int, List<RedemptionConfig>> GetRedemptionCodeConfigs(SearchRedemptionConfigRequest request)
        {
            List<RedemptionConfig> list = null;
            int total = 0;
            try
            {
                var result = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.SelectRedemptionCodeConfigs(conn, request));
                total = result.Item1;
                list = result.Item2;
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(GetRedemptionCodeConfigs), ex);
            }
            return Tuple.Create(total, list ?? new List<RedemptionConfig>());
        }

        public List<RedemptionConfig> GetRedemptionCodeConfigsByGenerateType(string generateType)
        {
            List<RedemptionConfig> list = null;
            int total = 0;
            try
            {
                list = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.SelectRedemptionCodeConfigsByGenerateType(conn, generateType));
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(GetRedemptionCodeConfigsByGenerateType), ex);
            }
            return list;
        }

        /// <summary>
        /// 是否已经存在配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool IsExistsRedemptionConfig(RedemptionConfig config)
        {
            bool result = false;
            try
            {
                result = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.IsExistsRedemptionConfig(conn, config));
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(IsExistsRedemptionConfig), ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool UpdateRedemptionCodeConfig(RedemptionConfig config)
        {
            bool result = false;
            try
            {
                var oldConfig = GetRedemptionCodeConfig(config.ConfigId);
                if (oldConfig != null)
                {
                    result = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.UpdateRedemptionCodeConfig(conn, config));
                }
                if (result)
                {
                    InsertBeautyOprLog(new
                    {
                        LogType = RedemptionCodeConfigType,
                        IdentityID = config.ConfigId,
                        OldValue = JsonConvert.SerializeObject(oldConfig),
                        NewValue = JsonConvert.SerializeObject(config),
                        OperateUser = config.CreateUser,
                        Remarks = "update",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(UpdateRedemptionCodeConfig), ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool AddRedemptionCodeConfig(RedemptionConfig config)
        {
            bool result = false;
            try
            {
                config.ConfigId = Guid.NewGuid();
                config.Auditor = string.Empty;
                config.AuditStatus = "Pending";
                result = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.AddRedemptionCodeConfig(conn, config));
                if (result)
                {
                    InsertBeautyOprLog(new
                    {
                        LogType = RedemptionCodeConfigType,
                        IdentityID = config.ConfigId,
                        OldValue = string.Empty,
                        NewValue = JsonConvert.SerializeObject(config),
                        OperateUser = config.CreateUser,
                        Remarks = "add",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(AddRedemptionCodeConfig), ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 更加ConfigID获取配置
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public RedemptionConfig GetRedemptionCodeConfig(Guid configId)
        {
            RedemptionConfig result = null;
            try
            {
                result = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.GetRedemptionCodeConfig(conn, configId));
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(GetRedemptionCodeConfig), ex);
            }
            return result;
        }
        /// <summary>
        /// 更加groupID获取配置
        /// </summary>GetRedemptionConfigsGroupId
        /// <param name="configId"></param>
        /// <returns></returns>
        public IEnumerable<RedemptionConfig> GetRedemptionConfigsGroupId(Guid groupId)
        {
            IEnumerable<RedemptionConfig> result = null;
            try
            {
                result = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.GetRedemptionConfigsGroupId(conn, groupId));
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(GetRedemptionCodeConfig), ex);
            }
            return result;
        }
        /// <summary>
        /// 配置审核
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="auditStatus"></param>
        /// <returns></returns>
        public bool AuditRedemptionCodeConfig(Guid configId, string auditStatus, string auditor)
        {
            bool result = false;
            try
            {
                result = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.AuditRedemptionCodeConfig(conn, configId, auditStatus, auditor));
                if (result)
                {
                    InsertBeautyOprLog(new
                    {
                        LogType = RedemptionCodeConfigType,
                        IdentityID = configId,
                        NewValue = string.Empty,
                        OldValue = string.Empty,
                        OperateUser = auditor,
                        Remarks = $"审核配置{configId},审核结果:{auditStatus}",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(AuditRedemptionCodeConfig), ex);
                result = false;
            }
            return result;
        }

        public bool DeleteRedemptionCodeConfig(Guid configId, string userName)
        {
            var success = false;
            try
            {
                var oldData = GetRedemptionCodeConfig(configId);
                if (oldData != null)
                {
                    success = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.DeleteRedemptionCodeConfig(conn, configId));
                }
                if (success)
                {
                    InsertBeautyOprLog(new
                    {
                        LogType = RedemptionCodeConfigType,
                        IdentityID = configId,
                        NewValue = string.Empty,
                        OldValue = JsonConvert.SerializeObject(oldData),
                        OperateUser = userName,
                        Remarks = "delete",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(DeleteRedemptionCodeConfig), ex);
            }
            return success;
        }

        #endregion

        #region RedeemMrCodeConfig

        private const string RedeemMrCodeConfigType = "RedeemMrCodeConfig";

        public List<RedeemMrCodeConfig> GetRedeemMrCodeConfigs(Guid configId)
        {
            List<RedeemMrCodeConfig> result = null;
            try
            {
                result = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.SelectRedeemMrCodeConfigs(conn, configId));
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(GetRedeemMrCodeConfigs), ex);
            }
            return result;
        }

        public RedeemMrCodeConfig GetRedeemMrCodeConfig(int mrCodeConfigId)
        {
            RedeemMrCodeConfig result = null;
            try
            {
                result = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.SelectRedeemMrCodeConfig(conn, mrCodeConfigId));
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(GetRedeemMrCodeConfig), ex);
            }
            return result;
        }

        public bool IsExistsRedeemMrCodeConfig(RedeemMrCodeConfig config)
        {
            var result = false;
            try
            {
                result = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.IsExistsRedeemMrCodeConfig(conn, config));
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(IsExistsRedeemMrCodeConfig), ex);
            }
            return result;
        }

        public bool AddRedeemMrCodeConfig(RedeemMrCodeConfig config, string userName)
        {
            var result = false;
            try
            {
                config.PKID = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.AddRedeemMrCodeConfig(conn, config));
                result = config.PKID > 0;
                if (result)
                {
                    InsertBeautyOprLog(new
                    {
                        LogType = RedeemMrCodeConfigType,
                        IdentityID = config.PKID,
                        OldValue = string.Empty,
                        NewValue = JsonConvert.SerializeObject(config),
                        OperateUser = userName,
                        Remarks = "添加美容服务码配置",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(AddRedeemMrCodeConfig), ex);
            }
            return result;
        }

        public bool UpdateRedeemMrCodeConfig(RedeemMrCodeConfig config, string userName)
        {
            var result = false;
            try
            {
                var oldConfig = GetRedeemMrCodeConfig(config.PKID);
                if (config != null)
                {
                    result = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.UpdateRedeemMrCodeConfig(conn, config));
                }
                if (result)
                {

                    InsertBeautyOprLog(new
                    {
                        LogType = RedeemMrCodeConfigType,
                        IdentityID = config.PKID,
                        OldValue = JsonConvert.SerializeObject(oldConfig),
                        NewValue = JsonConvert.SerializeObject(config),
                        OperateUser = userName,
                        Remarks = "更新美容服务码配置",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(UpdateRedeemMrCodeConfig), ex);
            }
            return result;
        }

        public bool DeleteRedeemMrCodeConfig(int id, string userName)
        {
            var result = false;
            try
            {
                var config = GetRedeemMrCodeConfig(id);
                result = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.DeleteRedeemMrCodeConfig(conn, id));
                if (result)
                {
                    InsertBeautyOprLog(new
                    {
                        LogType = RedeemMrCodeConfigType,
                        IdentityID = config.PKID,
                        OldValue = JsonConvert.SerializeObject(config),
                        NewValue = string.Empty,
                        OperateUser = userName,
                        Remarks = "删除美容服务码配置",
                    });
                    DeleteRedeemMrCodeLimitConfig(id, userName);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(DeleteRedeemMrCodeConfig), ex);
            }
            return result;
        }

        #region RedeemMrCodeLimitConfig

        private const string RedeemMrCodeLimitConfigType = "RedeemMrCodeLimitConfig";
        public RedeemMrCodeLimitConfig GetRedeemMrCodeLimitConfig(int mrCodeConfigId)
        {
            RedeemMrCodeLimitConfig result = null;
            try
            {
                result = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.SelectRedeemMrCodeLimitConfig(conn, mrCodeConfigId));
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(GetRedeemMrCodeLimitConfig), ex);
            }
            return result;

        }

        public bool UpdateRedeemMrCodeLimitConfig(RedeemMrCodeLimitConfig config, string userName)
        {
            var result = false;
            try
            {
                var oldValue = GetRedeemMrCodeLimitConfig(config.MrCodeConfigId);
                result = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.UpdateRedeemMrCodeLimitConfig(conn, config));
                if (result)
                {
                    InsertBeautyOprLog(new
                    {
                        LogType = RedeemMrCodeLimitConfigType,
                        IdentityID = config.MrCodeConfigId,
                        OldValue = oldValue == null ? string.Empty : JsonConvert.SerializeObject(oldValue),
                        NewValue = JsonConvert.SerializeObject(config),
                        OperateUser = userName,
                        Remarks = "更新美容服务码限购",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(UpdateRedeemMrCodeLimitConfig), ex);
            }
            return result;
        }

        public bool DeleteRedeemMrCodeLimitConfig(int mrCodeConfigId, string userName)
        {
            var result = false;
            try
            {
                var config = GetRedeemMrCodeLimitConfig(mrCodeConfigId);
                if (config != null)
                {
                    result = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.DeleteRedeemMrCodeLimitConfig(conn, mrCodeConfigId));
                }
                if (result)
                {
                    InsertBeautyOprLog(new
                    {
                        LogType = RedeemMrCodeLimitConfigType,
                        IdentityID = config.MrCodeConfigId,
                        OldValue = JsonConvert.SerializeObject(config),
                        NewValue = string.Empty,
                        OperateUser = userName,
                        Remarks = "删除美容服务码限购配置",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(DeleteRedeemMrCodeLimitConfig), ex);
            }
            return result;
        }

        #endregion

        #endregion

        #region Log

        private bool InsertBeautyOprLog(object log)
        {
            return Business.Logger.LoggerManager.InsertLog("BeautyOprLog", log);
        }

        #endregion

        #region ServiceInterface

        /// <summary>
        /// 根据GetRuleGUIDs获取优惠券信息
        /// </summary>
        /// <param name="getRuleIds"></param>
        /// <returns></returns>
        public static IEnumerable<Service.Member.Models.CouponRuleModel> GetCouponRules(IEnumerable<Guid> getRuleIds)
        {
            using (var client = new Service.Member.PromotionClient())
            {
                var serviceResult = client.GetCouponRules(getRuleIds);
                serviceResult.ThrowIfException(true);
                return serviceResult.Result;
            }
        }

        public static IEnumerable<Service.Shop.Models.Region.Region> GetRegionByRegionIds(IEnumerable<int> regionIds) =>
            regionIds?.Select(x => GetRegionByRegionId(x)) ?? new List<Service.Shop.Models.Region.Region>();

        public static Service.Shop.Models.Region.Region GetRegionByRegionId(int regionId)
        {
            using (var client = new Service.Shop.RegionClient())
            {
                var serviceResult = client.GetRegionByRegionId(regionId);
                serviceResult.ThrowIfException(true);
                return serviceResult.Result;
            }
        }

        public static GenerateRedemptionCodeResult GenerateRedemptionCode(
            int quantity, DateTime startTime, DateTime endTime, string user)
        {
            var result = BaoYang.BaoYangExternalService.GenerateRedemptionCode(new GenerateRedemptionCodeRequest
            {
                CreateUser = user,
                EndTime = endTime,
                StartTime = startTime,
                Quantity = quantity,
                RedemptionCodeType = RedemptionCodeEnum.Type.UniversalServcie,
                Remark = "大客户通用服务兑换码生成",
            });
            return result;
        }

        #endregion

        #region PromotionBusinessTypeConfig

        private const string CouponBusinessType = "CouponBusinessType";

        public bool AddCouponBusinessConfig(PromotionBusinessTypeConfig config)
        {
            var result = false;
            try
            {
                config.PKID = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.AddCouponBusinessConfig(conn, config));
                result = config.PKID > 0;
                if (result)
                {
                    InsertBeautyOprLog(new
                    {
                        LogType = CouponBusinessType,
                        IdentityID = config.PKID,
                        NewValue = JsonConvert.SerializeObject(config),
                        OldValue = string.Empty,
                        OperateUser = config.CreateUser,
                        Remarks = "添加",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(AddCouponBusinessConfig), ex);
            }
            return result;
        }

        public bool RemoveCouponBusinessConfig(int id, string userName)
        {
            var success = false;
            try
            {
                var config = GetCouponBusinessConfig(id);
                if (config != null)
                {
                    success = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.RemoveCouponBusinessConfig(conn, config.PKID));
                    if (success)
                    {
                        InsertBeautyOprLog(new
                        {
                            LogType = CouponBusinessType,
                            IdentityID = id,
                            OldValue = JsonConvert.SerializeObject(config),
                            NewValue = string.Empty,
                            OperateUser = userName,
                            Remarks = "删除",
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(RemoveCouponBusinessConfig), ex);
            }
            return success;
        }

        public List<PromotionBusinessTypeConfig> GetCouponBusinessConfigs(string businessType)
        {
            List<PromotionBusinessTypeConfig> result = null;
            try
            {
                result = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.GetCouponBusinessConfigs(conn, businessType))?.ToList();
                var coupons = GetCouponRules(result?.Select(x => x.GetRuleGuid).Distinct());
                result?.ForEach(x =>
                {
                    var coupon = coupons.FirstOrDefault(item => item.GetRuleGUID == x.GetRuleGuid);
                    if (coupon != null)
                    {
                        x.Description = $"{coupon.PKID}-{coupon.PromotionName}-{coupon.Description}";
                    }
                    else x.Description = string.Empty;
                });
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(GetCouponBusinessConfigs), ex);
            }
            return result ?? new List<PromotionBusinessTypeConfig>();
        }

        public PromotionBusinessTypeConfig GetCouponBusinessConfig(int id)
        {
            PromotionBusinessTypeConfig result = null;
            try
            {
                result = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.SelectCouponBusinessConfig(conn, id));
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(GetCouponBusinessConfig), ex);
            }
            return result;
        }

        public bool IsExistsCouponBusinessConfig(PromotionBusinessTypeConfig config)
        {
            bool result = true;
            try
            {
                result = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.IsExistsCouponBusinessConfig(conn, config));
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(IsExistsCouponBusinessConfig), ex);
            }
            return result;
        }

        #endregion

        #region RedeemPromotionConfig

        private const string RedeemPromotionConfigType = "RedeemPromotionConfigType";

        public RedeemPromotionConfig GetRedeemPromotionConfig(int id)
        {
            RedeemPromotionConfig result = null;
            try
            {
                result = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.SelectRedeemPromotionConfig(conn, id));
                if (result != null)
                {
                    var coupon = GetCouponRules(new[] { result.GetRuleGuid })?.FirstOrDefault();
                    if (coupon != null)
                    {
                        result.CouponDescription = $"{coupon.PKID}-{coupon.PromotionName}-{coupon.Description}";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(GetRedeemPromotionConfig), ex);
            }
            return result;
        }

        public bool IsExistsRedeemPromotionConfig(RedeemPromotionConfig config)
        {
            bool result = true;
            try
            {
                result = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.IsExistsRedeemPromotionConfig(conn, config));
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(IsExistsRedeemPromotionConfig), ex);
            }
            return result;
        }

        public object AddRedeemPromotionConfig(RedeemPromotionConfig config, string userName)
        {
            var result = false;
            try
            {
                config.PKID = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.AddRedeemPromotionConfig(conn, config));
                result = config.PKID > 0;
                if (result)
                {
                    InsertBeautyOprLog(new
                    {
                        LogType = RedeemPromotionConfigType,
                        IdentityID = config.PKID,
                        NewValue = JsonConvert.SerializeObject(config),
                        OldValue = string.Empty,
                        OperateUser = userName,
                        Remarks = "添加",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(AddRedeemPromotionConfig), ex);
            }
            return result;
        }

        public bool DeleteRedeemPromotionConfig(int id, string userName)
        {
            var result = false;
            try
            {
                var config = GetRedeemPromotionConfig(id);
                if (config != null)
                {
                    result = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.DeleteRedeemPromotionConfig(conn, config));
                }
                if (result)
                {
                    InsertBeautyOprLog(new
                    {
                        LogType = RedeemPromotionConfigType,
                        IdentityID = config.PKID,
                        OldValue = JsonConvert.SerializeObject(config),
                        NewValue = string.Empty,
                        OperateUser = userName,
                        Remarks = "删除",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(DeleteRedeemPromotionConfig), ex);
            }
            return result;
        }

        public object UpdateRedeemPromotionConfig(RedeemPromotionConfig config, string userName)
        {
            var result = false;
            try
            {
                var oldConfig = GetRedeemPromotionConfig(config.PKID);
                if (config != null)
                {
                    result = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.UpdateRedeemPromotionConfig(conn, config));
                }
                if (result)
                {
                    InsertBeautyOprLog(new
                    {
                        LogType = RedeemPromotionConfigType,
                        IdentityID = config.PKID,
                        OldValue = JsonConvert.SerializeObject(oldConfig),
                        NewValue = JsonConvert.SerializeObject(config),
                        OperateUser = userName,
                        Remarks = "更新",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(UpdateRedeemPromotionConfig), ex);
            }
            return result;
        }

        public List<RedeemPromotionConfig> GetRedeemPromotionConfigs(Guid id)
        {
            List<RedeemPromotionConfig> result = null;
            try
            {
                result = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.SelectRedeemPromotionConfigs(conn, id));
                //var coupons = GetCouponRules(result.Select(x => x.GetRuleGuid));
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(GetRedeemPromotionConfigs), ex);
            }
            return result;
        }

        #endregion

        #region RedemptionCodeRecord

        private const string RedemptionCodeRecordType = "RedemptionCodeRecord";

        public bool GenerateRedemptionCodeRecords(Guid configId, int days, int num, string userName)
        {
            var success = false;
            var now = DateTime.Now;
            var startTime = new DateTime(now.Year, now.Month, now.Day);
            var endTime = startTime.AddDays(days);
            try
            {
                var result = GenerateRedemptionCode(num, startTime, endTime, userName);
                if (result.Success && result.Codes != null && result.Codes.Any())
                {
                    var createTime = DateTime.Parse(now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    var records = result.Codes.Select(x => new RedemptionCodeRecord
                    {
                        BatchCode = result.BatchCode,
                        CreateTime = createTime,
                        CreateUser = userName,
                        MobileNum = null,
                        OrderId = null,
                        RedemptionCode = x,
                        RedemptionConfigId = configId,
                        UpdateTime = null,
                        UserId = null,
                    }).ToList();
                    success = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.AddRedeemCodeRecords(conn, records));
                    if (success)
                    {
                        InsertBeautyOprLog(new
                        {
                            LogType = RedemptionCodeRecordType,
                            IdentityID = result.BatchCode,
                            NewValue = string.Empty,
                            OldValue = string.Empty,
                            OperateUser = userName,
                            Remarks = $"{userName}生成了批次{result.BatchCode}的兑换码",
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(GenerateRedemptionCodeRecords), ex);
            }
            return success;
        }

        public Tuple<int, List<RedemptionCodeRecordResult>> GetRedemptionCodeRecords(SearchRedemptionConfigRequest request)
        {
            int total = 0;
            List<RedemptionCodeRecordResult> list = null;
            try
            {
                var result = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.SelectRedemptionCodeRecords(conn, request));
                var batchCodes = result.Item2?.Select(t => t.BatchCode);
                if (batchCodes != null && batchCodes.Any())
                {
                    var batchCodeStatus = DbTuhuGrouponScopeReadManager.Execute(conn =>
                    DALUnivRedemptionCode.SelectRedemptionBatchStatus(conn, batchCodes, new { BatchCode = "", Status = "" }));
                    if (batchCodeStatus != null && batchCodeStatus.Any())
                    {
                        result.Item2.ForEach(b =>
                        {
                            b.Status = batchCodeStatus.FirstOrDefault(s => s.BatchCode == b.BatchCode)?.Status;
                        });
                    }
                }
                total = result.Item1;
                list = result.Item2?.ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return Tuple.Create(total, list ?? new List<RedemptionCodeRecordResult>());
        }

        public List<RedemptionCodeRecord> GetRedemptionCodeRecords(string batchCode, string userName)
        {
            List<RedemptionCodeRecord> list = null;
            try
            {
                list = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.SelectRedemptionCodeRecords(conn, batchCode));
                InsertBeautyOprLog(new
                {
                    LogType = "DownloadRedemptionCodeRecords",
                    IdentityID = batchCode,
                    NewValue = string.Empty,
                    OldValue = string.Empty,
                    OperateUser = userName,
                    Remarks = $"{userName}下载了批次{batchCode}的兑换码",
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return list;
        }

        /// <summary>
        /// 根据兑换码，手机号查询通用兑换码信息
        /// </summary>
        /// <param name="redemptionCode"></param>
        /// <param name="mobile"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<UnivBeautyRedemptionCodeResult>, int> GetRedemptionCodeRecordsByCondition(string redemptionCode, string mobile, int pageIndex, int pageSize)
        {
            var result = new List<UnivBeautyRedemptionCodeResult>();
            var total = 0;
            try
            {
                if (!string.IsNullOrWhiteSpace(redemptionCode))
                {
                    var univRedemptionCode = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.FetchRedemptionCodeRecordByRedemptionCode(conn, redemptionCode));
                    if (univRedemptionCode != null)
                    {
                        if (univRedemptionCode.UserId != null)
                        {
                            var user = UserAccountService.GetUserById(univRedemptionCode.UserId.Value);
                            univRedemptionCode.Mobile = user?.MobileNumber;
                        }
                        if (!string.IsNullOrWhiteSpace(mobile))
                        {
                            if (univRedemptionCode.Mobile == mobile)
                            {
                                result.Add(univRedemptionCode);
                            }
                        }
                        else
                        {
                            result.Add(univRedemptionCode);
                        }
                        total = result.Count;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(mobile))
                {
                    var user = UserAccountService.GetUserByMobile(mobile);
                    if (user != null && user.UserId != Guid.Empty)
                    {
                        var redemptionCodes = DbTuhuGrouponScopeReadManager.Execute(conn =>
                            DALUnivRedemptionCode.SelectRedemptionCodeRecordsByUserId(conn, user.UserId, pageIndex, pageSize));
                        redemptionCodes.Item1.ForEach(i =>
                        {
                            i.Mobile = user.MobileNumber;
                        });
                        result.AddRange(redemptionCodes.Item1);
                        total = redemptionCodes.Item2;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return new Tuple<IEnumerable<UnivBeautyRedemptionCodeResult>, int>(GetRedemptionCarNumber(result), total);
        }
        /// <summary>
        /// 获取车牌号
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private  IEnumerable<UnivBeautyRedemptionCodeResult> GetRedemptionCarNumber(IEnumerable<UnivBeautyRedemptionCodeResult> data)
        {
            try
            {
                if (data != null && data.Any())
                {
                    foreach (var item in data)
                    {
                        var promotionConfig = DbTuhuGrouponScopeReadManager.Execute(conn => DALUnivRedemptionCode.SelectRedeemPromotionConfigs(conn, item.ConfigGuid));
                        if (promotionConfig != null && promotionConfig.Any())
                        {
                            switch(promotionConfig.FirstOrDefault().BusinessType)
                            {
                                case "BaoYangPackage":
                                    var baoyangPromotion = BeautyServicePackageDal.GetVipBaoYangGiftPackCouponDetail(item.RedemptionCode);
                                    item.CarNo = baoyangPromotion?.FirstOrDefault()?.CarNo;
                                    break;
                                case "PaintPackage":
                                    var promotions = BeautyServicePackageDal.GetRedeemPromotionRecordByRedemptionCode(item.RedemptionCode);
                                    if (promotions != null && promotions.Any())
                                    {
                                        var paintPromotion = BeautyServicePackageDal.GetVipPaintPackagePromotionDetail(promotions.Select(x => x.PromotionId).ToList());
                                        item.CarNo = paintPromotion?.FirstOrDefault()?.CarNo;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return data.ToList();
        }
        #endregion

        #region GroupSetting
        public List<RedeemGroupSetting> SelectRedeemGroupSetting(out int count, int pageIndex, int pageSize, string keyWord)
        {
            List<RedeemGroupSetting> list = null;
            count = 0;
            try
            {
                var result = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.SelectRedeemGroupSetting(conn, pageIndex, pageSize, keyWord));
                list = result.Item1;
                count = result.Item2;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return list;
        }
        public Tuple<bool, string> SaveRedeemGroupSetting(RedeemGroupSetting model, string userName)
        {
            var result = false;
            if (model == null)
                return Tuple.Create(result, "");
            try
            {

                if (model.GroupId == null || model.GroupId == Guid.Empty)
                {
                    model.GroupId = Guid.NewGuid();
                    result = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.InsertRedeemGroupSetting(conn, model));
                    if (result)
                        InsertBeautyOprLog(new
                        {
                            LogType = "InsertRedeemGroupSetting",
                            IdentityID = model.GroupId,
                            NewValue = JsonConvert.SerializeObject(model),
                            OldValue = string.Empty,
                            OperateUser = userName,
                        });
                }
                else
                {
                    var oldmodel = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.GetRedeemGroupSetting(conn, model.GroupId));

                    if (model.SendCodeType == 1)//自动发码，群组下只能有一个套餐
                    {
                        if (DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.GetRedemptionConfigCountByGroupId(conn, model.GroupId)) > 1)
                        {
                            return Tuple.Create(result, "自动发码时，群组下只能有一个套餐");
                        }
                    }
                    result = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.UpdateRedeemGroupSetting(conn, model));
                    if (result)
                        InsertBeautyOprLog(new
                        {
                            LogType = "UpdateRedeemGroupSetting",
                            IdentityID = model.GroupId,
                            NewValue = JsonConvert.SerializeObject(model),
                            OldValue = JsonConvert.SerializeObject(oldmodel),
                            OperateUser = userName,
                        });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return Tuple.Create(result, "");
        }

        public Tuple<bool, string> DeleteRedeemGroupSetting(RedeemGroupSetting model, string userName)
        {
            var result = false;
            if (model == null)
                return Tuple.Create(result, "");
            try
            {
                if (model.GroupId != null && model.GroupId != Guid.Empty)
                {
                    var oldmodel = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.GetRedeemGroupSetting(conn, model.GroupId));
                    if (DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.GetRedemptionConfigCountByGroupId(conn, model.GroupId)) <= 0)
                    {
                        result = DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.DeleteRedeemGroupSetting(conn, model));
                        if (result)
                            InsertBeautyOprLog(new
                            {
                                LogType = "UpdateRedeemGroupSetting",
                                IdentityID = model.GroupId,
                                NewValue = JsonConvert.SerializeObject(model),
                                OldValue = JsonConvert.SerializeObject(oldmodel),
                                OperateUser = userName,
                            });
                    }
                    else
                    {
                        return Tuple.Create(result, "该分组下存在套餐无法删除！");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return Tuple.Create(result, "");
        }
        public RedeemGroupSetting GetGroupSettingByGroupId(Guid groupId)
        {
            return DbTuhuGrouponScopeManager.Execute(conn => DALUnivRedemptionCode.GetRedeemGroupSetting(conn, groupId));
        }
        public IEnumerable<OpenAppModel> GetAllBigCustomerOpenAppIds(string orderChannel = "大客户")
        {
            return DbTuhuGunginrScopeReadManager.Execute(conn => DALUnivRedemptionCode.GetAllBigCustomerOpenAppIds(conn, orderChannel));
        }
        #endregion

        #region ThirdOpenApp

        public List<OpenAppModel> GetThirdAppSettings(out int count, int pageIndex, int pageSize, string keyWord)
        {
            List<OpenAppModel> list = null;
            count = 0;
            try
            {
                var result = DbTuhuGunginrScopeReadManager.Execute(conn => DALUnivRedemptionCode.GetThirdAppSettings(conn, pageIndex, pageSize, keyWord));
                list = result.Item1;
                count = result.Item2;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return list;
        }
        
        public OpenAppModel GetThirdAppSettingDetail(int id)
        {
            OpenAppModel result = null;
            try
            {
                result = DbTuhuGunginrScopeReadManager.Execute(conn => DALUnivRedemptionCode.GetThirdAppSettingsDetail(conn, id));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result;
        }
        public OpenAppModel GetThirdAppSettingsDetailByGroupId(string  appId)
        {
            OpenAppModel result = null;
            try
            {
                result = DbTuhuGunginrScopeReadManager.Execute(conn => DALUnivRedemptionCode.GetThirdAppSettingsDetailByAppId(conn, appId));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result;
        }
        public bool SaveThirdAppSetting(OpenAppModel model, string userName)
        {
            bool result = false;
            try
            {
                if (model.Id > 0)
                {
                    var oldModel = GetThirdAppSettingDetail(model.Id);
                    result = DbTuhuGunginrScopeReadManager.Execute(conn => DALUnivRedemptionCode.UpdateThirdAppSetting(conn, model));
                    if (result)
                        InsertBeautyOprLog(new
                        {
                            LogType = "UpdateThirdAppSetting",
                            IdentityID = model.AppId,
                            NewValue = JsonConvert.SerializeObject(model),
                            OldValue = JsonConvert.SerializeObject(oldModel),
                            OperateUser = userName,
                        });
                }
                else
                {
                    result = DbTuhuGunginrScopeReadManager.Execute(conn => DALUnivRedemptionCode.InsertThirdAppSetting(conn, model));
                    if (result)
                        InsertBeautyOprLog(new
                        {
                            LogType = "InsertThirdAppSetting",
                            IdentityID = model.AppId,
                            NewValue = JsonConvert.SerializeObject(model),
                            OldValue = string.Empty,
                            OperateUser = userName,
                        });
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result;
        }

        public bool DelThirdAppSetting(int id, string userName)
        {
            var result = DbTuhuGunginrScopeManager.Execute(conn => DALUnivRedemptionCode.DeleteThirdAppSetting(conn, id));
            if (result)
                InsertBeautyOprLog(new
                {
                    LogType = "DeleteThirdAppSetting",
                    IdentityID = id,
                    NewValue = string.Empty,
                    OldValue = string.Empty,
                    OperateUser = userName,
                });
            return result;
        }
        public bool CheckAppIdIsExist(string appId)
        {
            return DbTuhuGunginrScopeManager.Execute(conn => DALUnivRedemptionCode.CheckAppIdIsExist(conn, appId));
        }
        #endregion
    }
}
