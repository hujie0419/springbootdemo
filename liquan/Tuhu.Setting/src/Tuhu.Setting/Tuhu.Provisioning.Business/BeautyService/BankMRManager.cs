using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Nosql;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO.BankMRActivityDal;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;
using Tuhu.Service.KuaiXiu.Models;
using Tuhu.Service.Shop;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.Provisioning.Business.BeautyService
{
    public class BankMRManager
    {
        private static readonly Common.Logging.ILog Logger = LogManager.GetLogger(typeof(BankMRManager));
        private static readonly IDBScopeManager TuhuGrouponDbScopeManager;

        private static readonly IDBScopeManager TuhuGrouponDbScopeManagerReadOnly;
        private static readonly IDBScopeManager TuhuGroupon_GungnirManagerReadOnly;
        static BankMRManager()
        {
            var connRw = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
            var connRo = ConfigurationManager.ConnectionStrings["Tuhu_Groupon_ReadOnly"].ConnectionString;
            var connGrouponGungnir = ConfigurationManager.ConnectionStrings["Tuhu_Groupon_GungnirReadOnly"].ConnectionString;
            var connManagerRw = new ConnectionManager(SecurityHelp.IsBase64Formatted(connRw) ? SecurityHelp.DecryptAES(connRw) : connRw);
            var connManagerRo = new ConnectionManager(SecurityHelp.IsBase64Formatted(connRo) ? SecurityHelp.DecryptAES(connRo) : connRo);
            var connGrouponGungnirRo = new ConnectionManager(SecurityHelp.IsBase64Formatted(connGrouponGungnir) ? SecurityHelp.DecryptAES(connGrouponGungnir) : connGrouponGungnir);
            TuhuGrouponDbScopeManager = new DBScopeManager(connManagerRw);
            TuhuGrouponDbScopeManagerReadOnly = new DBScopeManager(connManagerRo);
            TuhuGroupon_GungnirManagerReadOnly = new DBScopeManager(connGrouponGungnirRo);
        }
        private BankMRHandler handler = new BankMRHandler();
        /// <summary>
        /// 根据活动ID获取银行活动配置
        /// </summary>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        public BankMRActivityConfig GetBankMRActivityConfigByActivityId(Guid activityId)
        {
            BankMRActivityConfig result = null;

            try
            {
                result = TuhuGrouponDbScopeManagerReadOnly.Execute(conn => BankMRActivityDal.SelectBankMRActivityConfigByActivityId(conn, activityId));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        /// <summary>
        /// 根据活动id查询活动广告位
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public IEnumerable<BankMRActivityAdConfig> GetBankMRActivityAdConfigByActivityId(Guid activityId)
        {
            IEnumerable<BankMRActivityAdConfig> result = null;
            try
            {
                result = TuhuGrouponDbScopeManagerReadOnly.Execute(conn =>
                    BankMRActivityDal.SelectBankMRActivityAdConfigByActivityId(conn, activityId));

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        ///  插入或更新银行活动广告位配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool UpsertBankMRActivityAdConfig(BankMRActivityAdConfig config)
        {
            var result = false;
            try
            {
                result = TuhuGrouponDbScopeManager.Execute(conn =>
                    BankMRActivityDal.UpsertBankMRActivityAdConfig(conn, config));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        /// 分页查询银行美容活动配置
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<BankMRActivityConfig> GetBankMRActivityConfigs(int pageIndex, int pageSize, int cooperateId,
            string activityName, string serviceId, string settleMentMethod, out int count)
        {
            IEnumerable<BankMRActivityConfig> result = null;
            count = 0;
            try
            {
                var temp = TuhuGrouponDbScopeManagerReadOnly.Execute(conn => BankMRActivityDal.SelectBankMRActivityConfigs(conn, pageIndex, pageSize, cooperateId, activityName, serviceId, settleMentMethod));
                result = temp.Item1;
                count = temp.Item2;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 根据活动场次ID查询活动场次
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static BankMRActivityRoundConfig GetBankMRActivityRoundConfigByPKID(int pkid)
        {
            BankMRActivityRoundConfig result = null;

            try
            {

                result = TuhuGrouponDbScopeManagerReadOnly.Execute(conn => BankMRActivityDal.SelectBankMRActivityRoundConfigByPKID(conn, pkid));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        /// <summary>
        /// 插入或更新银行美容活动配置
        /// </summary>
        /// <param name="config">插入活动配置</param>
        /// <param name="adConfig">广告位配置</param>
        /// <param name="operateUser">操作人</param>
        /// <returns></returns>
        public bool UpsertBankMRActivityConfig(BankMRActivityConfig config, IEnumerable<BankMRActivityAdConfig> adConfig, string operateUser)
        {
            var result = false;
            try
            {
                TuhuGrouponDbScopeManager.CreateTransaction(conn =>
                {
                    if (config.ActivityId != null && config.ActivityId != Guid.Empty)
                    {
                        var currentActivity = GetBankMRActivityConfigByActivityId(config.ActivityId);
                        if (!string.Equals(currentActivity.RoundCycleType, config.RoundCycleType) ||
                            currentActivity.StartTime != config.StartTime || currentActivity.EndTime != config.EndTime)//如果活动场次周期类型变了，则需要重新生成活动场次
                        {
                            BankMRActivityDal.SetBankMRActivityRoundDisable(conn, config.ActivityId);
                            var roundResult = handler.GenerateBankMRActivityRound(conn, config);
                            if (!roundResult)
                            {
                                throw new Exception("生成银行美容活动场次失败");
                            }

                        }
                        var updateResult = BankMRActivityDal.UpdateBankMRActivityConfig(conn, config);

                        if (!updateResult)
                        {
                            throw new Exception("更新银行美容活动配置失败");
                        }
                        var log = new BeautyOprLog
                        {
                            LogType = "UpdateBankMRActivityConfig",
                            IdentityID = config.ActivityId.ToString(),
                            OldValue = JsonConvert.SerializeObject(currentActivity),
                            NewValue = JsonConvert.SerializeObject(config),
                            Remarks = $"修改银行美容活动配置,活动ID：{config.ActivityId}",
                            OperateUser = operateUser,
                        };
                        LoggerManager.InsertLog("BeautyOprLog", log);

                    }
                    else
                    {
                        config.ActivityId = Guid.NewGuid();
                        adConfig?.ToList()?.ForEach(s => s.ActivityId = config.ActivityId);
                        var insertResult = BankMRActivityDal.InsertBankMRActivityConfig(conn, config);
                        if (insertResult)
                        {
                            var roundResult = handler.GenerateBankMRActivityRound(conn, config);
                            if (!roundResult)
                            {

                                throw new Exception("生成银行美容活动场次失败");
                            }
                        }
                        else
                        {
                            throw new Exception("插入银行美容活动配置失败");
                        }
                        var log = new BeautyOprLog
                        {
                            LogType = "InsertBankMRActivityConfig",
                            IdentityID = config.ActivityId.ToString(),
                            OldValue = null,
                            NewValue = JsonConvert.SerializeObject(config),
                            Remarks = $"插入银行美容活动配置,活动ID：{config.ActivityId}",
                            OperateUser = operateUser,
                        };
                        LoggerManager.InsertLog("BeautyOprLog", log);
                    }

                    if (adConfig != null && adConfig.Any())
                    {
                        foreach (var ad in adConfig)
                        {
                            if (ad != null && ad.ActivityId != Guid.Empty)
                            {
                                BankMRActivityDal.UpsertBankMRActivityAdConfig(conn, ad);
                            }
                        }
                        var oldAdConfig =
                            BankMRActivityDal.SelectBankMRActivityAdConfigByActivityId(conn, config.ActivityId);
                        var adLog = new BeautyOprLog()
                        {
                            LogType = "UpsertBankMRActivityAdConfig",
                            IdentityID = config.ActivityId.ToString(),
                            OldValue = oldAdConfig != null && oldAdConfig.Any()
                                ? JsonConvert.SerializeObject(oldAdConfig)
                                : string.Empty,
                            NewValue = JsonConvert.SerializeObject(adConfig),
                            Remarks = $"修改银行广告位配置,活动ID：{config.ActivityId}",
                            OperateUser = operateUser,
                        };
                        LoggerManager.InsertLog("BeautyOprLog", adLog);
                    }

                    result = true;
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }

            return result;
        }


        /// <summary>
        /// 根据活动id查询美容活动场次
        /// </summary>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        public IEnumerable<BankMRActivityRoundConfig> GetBankMRActivityRoundConfigsByActivityId(Guid activityId)
        {
            IEnumerable<BankMRActivityRoundConfig> result = null;

            try
            {
                TuhuGrouponDbScopeManagerReadOnly.Execute(conn => result = BankMRActivityDal.SelectBankMRActivityRoundConfigsByActivityId(conn, activityId));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 获取银行活动限购周时间配置
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public IEnumerable<BankMRActivityLimitTimeConfig> SelectBankMRActivityLimitTimeConfigByActivityId(Guid activityId)
        {
            IEnumerable<BankMRActivityLimitTimeConfig> result = null;

            try
            {
                TuhuGrouponDbScopeManagerReadOnly.Execute(conn => result = BankMRActivityDal.SelectBankMRActivityLimitTimeConfigByActivityId(conn, activityId));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        /// <summary>
        /// 更新银行美容活动场次配置
        /// </summary>
        /// <param name="roundConfig">活动场次配置</param>
        /// <returns></returns>
        public bool UpdateBankMRActivityRoundConfig(BankMRActivityRoundConfig roundConfig)
        {
            var result = false;

            try
            {
                TuhuGrouponDbScopeManager.Execute(conn => result = BankMRActivityDal.UpdateBankMRActivityRoundConfig(conn, roundConfig));

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        public bool UpdateBankMRActivityRoundLimitCount(Guid activityId, int limitCount, int userLimitCount)
        {
            var result = false;

            try
            {
                TuhuGrouponDbScopeManager.Execute(conn => result = BankMRActivityDal.UpdateBankMRActivityRoundLimitCount(conn, activityId, limitCount, userLimitCount));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;

        }

        /// <summary>
        /// 根据服务码查询银行美容服务码记录
        /// </summary>
        /// <param name="serviceCodes"></param>
        /// <returns></returns>
        public static IEnumerable<BankMRActivityCodeRecord> GetBankMRActivityCodeRecordByServiceCode(IEnumerable<string> serviceCodes)
        {
            IEnumerable<BankMRActivityCodeRecord> result = null;

            try
            {
                TuhuGrouponDbScopeManagerReadOnly.Execute(conn => result = BankMRActivityDal.SelectBankMRActivityCodeRecordByServiceCode(conn, serviceCodes));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 根据银行美容记录查询服务码详情
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ServiceCodeDetail>> SearchBankMRActivityCodeDetailByRecords(IEnumerable<BankMRActivityCodeRecord> records)
        {
            var result = new List<ServiceCodeDetail>();
            if (records != null)
            {
                records = records.Where(s => !string.IsNullOrEmpty(s.ServiceCode));
                var roundIds = records.Select(t => t.ActivityRoundId).Distinct();
                var rounds = new List<BankMRActivityRoundConfig>();
                foreach (var item in roundIds)
                {
                    var round = GetBankMRActivityRoundConfigByPKID(item);
                    if (round != null)
                    {
                        rounds.Add(round);
                    }
                }
                var activityIds = rounds.Select(t => t.ActivityId).Distinct();
                var activitys = new List<BankMRActivityConfig>();
                foreach (var item in activityIds)
                {
                    var activity = TuhuGrouponDbScopeManagerReadOnly.Execute(conn => BankMRActivityDal.SelectBankMRActivityConfigByActivityId(conn, item));
                    activitys.Add(activity);
                }
                var serviceCodes = records.Select(t => t.ServiceCode).Distinct();
                var serviceCodeDetails = await SearchCodeManager.GetServiceCodeDetailsByCodes(serviceCodes);
                var companyUsers = new List<SYS_CompanyUser>();
                var bankIds = activitys.Select(t => t.BankId).Distinct();
                using (var client = new UserAccountClient())
                {
                    foreach (var item in bankIds)
                    {
                        var serviceResult = await client.SelectCompanyUserInfoAsync(item);
                        serviceResult.ThrowIfException(true);
                        var company = serviceResult.Result;
                        if (company != null)
                        {
                            companyUsers.Add(company);
                        }
                    }
                }
                var products = new List<BeautyProductModel>();
                foreach (var pid in activitys.Select(t => t.ServiceId).Distinct())
                {
                    var product = BeautyProductManager.GetBeautyProductByPid(pid);
                    if (product != null)
                    {
                        products.Add(product);
                    }
                }
                foreach (var re in records)
                {
                    var activityId = (rounds.FirstOrDefault(s => s.PKID == re.ActivityRoundId))?.ActivityId;
                    if (activityId != null)
                    {
                        var activity = activitys.FirstOrDefault(s => s.ActivityId == activityId);
                        if (activity != null && serviceCodeDetails != null)
                        {
                            var serviceCodeDetail = serviceCodeDetails.FirstOrDefault(s => string.Equals(s.Code, re.ServiceCode));
                            var product = products.First(p => string.Equals(p.PID, activity.ServiceId));
                            if (serviceCodeDetail != null)
                            {
                                result.Add(
                               new ServiceCodeDetail()
                               {
                                   BatchCode = activity.ActivityId.ToString(),
                                   ServiceCode = re.ServiceCode,
                                   Status = serviceCodeDetail?.Status.ToString(),
                                   VipCompanyName = companyUsers.FirstOrDefault(t => t.UserId == activity.BankId)?.UserName,
                                   VerifyTime = serviceCodeDetail?.VerifyTime,
                                   StartTime = re.CreateTime,
                                   EndTime = Convert.ToDateTime(serviceCodeDetail.OverdueTime),
                                   VipSettleMentPrice = activity.BankSettlementPrice,
                                   ShopCommission = 0,
                                   PID = activity.ServiceId,
                                   RestrictVehicle = BeautyProductManager.GetVehicleTypeDescription(product.RestrictVehicleType),
                                   Mobile = re.Mobile,
                                   OrderNo = !string.IsNullOrEmpty(serviceCodeDetail?.TuhuOrderId.ToString()) ? $"TH{serviceCodeDetail?.TuhuOrderId}" : null,
                                   VerifyShop = serviceCodeDetail?.InstallShopId.ToString(),
                                   Type = "Bank"
                               });
                            }
                        }
                    }

                }
            }
            return result;
        }

        /// <summary>
        /// 根据userId查询用户服务码
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static IEnumerable<BankMRActivityCodeRecord> GetBankMRActivityCodeRecordByUserId(Guid userId)
        {
            IEnumerable<BankMRActivityCodeRecord> result = null;

            try
            {
                TuhuGrouponDbScopeManagerReadOnly.Execute(conn => result = BankMRActivityDal.SelectBankMRActivityCodeRecordByUserId(conn, userId));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;

        }
        /// <summary>
        /// 分页查询合作用户配置
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<MrCooperateUserConfig>, int> GetMrCooperateUserConfigs(int pageIndex, int pageSize)
        {
            Tuple<IEnumerable<MrCooperateUserConfig>, int> result = null;

            try
            {
                TuhuGrouponDbScopeManagerReadOnly.Execute(conn => result = BankMRActivityDal.SelectMrCooperateUserConfigs(conn, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 获取所有合作用户配置
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MrCooperateUserConfig> GetAllMrCooperateUserConfigs()
        {
            IEnumerable<MrCooperateUserConfig> result = null;

            try
            {
                TuhuGrouponDbScopeManagerReadOnly.Execute(conn => result = BankMRActivityDal.SelectAllMrCooperateUserConfigs(conn));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 根据PKID获取合作用户配置
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public MrCooperateUserConfig FetchMrCooperateUserConfigByPKID(int pkid)
        {
            MrCooperateUserConfig result = null;

            try
            {
                TuhuGrouponDbScopeManagerReadOnly.Execute(conn => result = BankMRActivityDal.FetchMrCooperateUserConfigByPKID(conn, pkid));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 插入合作用户配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool InsertMrCooperateUserConfig(MrCooperateUserConfig config, string user)
        {
            bool result = false;

            try
            {
                TuhuGrouponDbScopeManager.Execute(conn => result = BankMRActivityDal.InsertMrCooperateUserConfig(conn, config));
                var log = new BeautyOprLog
                {
                    LogType = "InsertMrCooperateUserConfig",
                    IdentityID = config.VipUserId.ToString(),
                    OldValue = "",
                    NewValue = JsonConvert.SerializeObject(config),
                    Remarks = $"插入合作用户",
                    OperateUser = user,
                };
                LoggerManager.InsertLog("BeautyOprLog", log);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 更新合作用户配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool UpdateMrCooperateUserConfig(MrCooperateUserConfig config, string user)
        {
            bool result = false;

            try
            {
                var oldValue = FetchMrCooperateUserConfigByPKID(config.PKID);
                TuhuGrouponDbScopeManager.Execute(conn => result = BankMRActivityDal.UpdateMrCooperateUserConfig(conn, config));
                var log = new BeautyOprLog
                {
                    LogType = "UpdateMrCooperateUserConfig",
                    IdentityID = config.PKID.ToString(),
                    OldValue = JsonConvert.SerializeObject(oldValue),
                    NewValue = JsonConvert.SerializeObject(config),
                    Remarks = $"更新合作用户",
                    OperateUser = user,
                };
                LoggerManager.InsertLog("BeautyOprLog", log);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 获取限购配置
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public BankMRActivityLimitConfig FetchBankMRActivityLimitConfig(Guid activityId)
        {
            BankMRActivityLimitConfig result = null;

            try
            {
                TuhuGrouponDbScopeManagerReadOnly.Execute(conn => result = BankMRActivityDal.FetchBankMRActivityLimitConfig(conn, activityId));
                var roundConfig = GetBankMRActivityRoundConfigsByActivityId(activityId);
                var weekLimitTime = SelectBankMRActivityLimitTimeConfigByActivityId(activityId)?.ToArray();
                if (roundConfig != null)
                {
                    if (result == null)
                        result = new BankMRActivityLimitConfig();
                    result.RoundLimit = roundConfig.FirstOrDefault()?.LimitCount ?? 0;
                    result.UserRoundLimit = roundConfig.FirstOrDefault()?.UserLimitCount ?? 0;
                    result.WeekTimeLimit = new BankMRActivityLimitTimeConfig[7];
                    for (int i = 0; i < 7; i++)
                    {
                        result.WeekTimeLimit[i] = new BankMRActivityLimitTimeConfig
                        {
                            DayOfWeek = i + 1,
                            BeginTime = (weekLimitTime?.Any(a => a.DayOfWeek == i + 1) ?? false) ? weekLimitTime.FirstOrDefault(a => a.DayOfWeek == i + 1)?.BeginTime : TimeSpan.FromTicks(0),
                            EndTime = (weekLimitTime?.Any(a => a.DayOfWeek == i + 1) ?? false) ? weekLimitTime.FirstOrDefault(a => a.DayOfWeek == i + 1)?.EndTime : TimeSpan.FromSeconds(86399),
                            Checked = result.DaysOfWeek?.Split(',').Contains((i + 1).ToString()) ?? false,
                        };
                    }
                }
                result.BankLimitConfig = ConvertBankLimitConfig(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }


        private List<BankLimitConfig> ConvertBankLimitConfig(BankMRActivityLimitConfig config)
        {
            List<BankLimitConfig> data = new List<BankLimitConfig>();
            try
            {
                if (!string.IsNullOrEmpty(config.ProvinceIds))
                {
                    var provinceIdList = config.ProvinceIds.Split(',').ToList();
                    var cityIdList = config?.CityIds?.Split(',')?.ToList().Where(_ => !string.IsNullOrEmpty(_)).ToList() ?? new List<string>();
                    var districtIdList = config?.DistrictIds?.Split(',')?.ToList().Where(_=>!string.IsNullOrEmpty(_)).ToList() ?? new List<string>();
                    List<Regions> citys = new List<Regions>();
                    var municipalities = new List<string>() { "上海市", "北京市", "重庆市", "天津市" };
                    foreach (var provinceId in provinceIdList)
                    {
                        var secondRegion = GetRegionByRegionId(Convert.ToInt32(provinceId));
                        if (secondRegion.IsBelongMunicipality)
                        {
                            citys = secondRegion.ChildRegions?.Select(x => new Regions { RegionId = x.DistrictId, RegionName = x.DistrictName }).ToList();
                        }
                        else
                        {
                            citys = secondRegion.ChildRegions?.Select(x => new Regions { RegionId = x.CityId, RegionName = x.CityName }).ToList();
                        }
                        var intersect = citys?.Select(x => x.RegionId.ToString()).ToList().Intersect(cityIdList).ToList();
                        if (cityIdList.Count() > 0)
                        {
                            if (intersect.Count() <= 0)
                            {
                                data.Add(new BankLimitConfig()
                                {
                                    ProvinceId = Convert.ToInt32(provinceId),
                                    Citys = citys,
                                    CityId = -1,
                                    DistrictIds = new List<int>()
                                });
                            }
                            else if (secondRegion.IsBelongMunicipality && intersect.Count() > 0)
                            {
                                intersect.ForEach(x =>
                                {
                                    data.Add(new BankLimitConfig()
                                    {
                                        ProvinceId = Convert.ToInt32(provinceId),
                                        Citys = citys,
                                        CityId = Convert.ToInt32(x),
                                        DistrictIds = new List<int>()
                                    });
                                });
                            }
                            else
                            {
                                foreach (var cityId in cityIdList)
                                {
                                    if (!string.IsNullOrEmpty(cityId))
                                    {
                                        var thirdRegion = GetRegionByRegionId(Convert.ToInt32(cityId));
                                        if (thirdRegion.ProvinceId.ToString() == provinceId)
                                        {
                                            var districts = thirdRegion.ChildRegions?.Select(x => new Regions { RegionId = x.DistrictId, RegionName = x.DistrictName }).ToList();
                                            if (districtIdList.Count() > 0)
                                            {
                                                List<int> districtIds = new List<int>();
                                                foreach (var districtId in districtIdList)
                                                {
                                                    if (!string.IsNullOrEmpty(districtId) && thirdRegion?.ChildRegions?.Where(x => String.Equals(x.DistrictId, Convert.ToInt32(districtId)))?.Count() > 0)
                                                    {
                                                        districtIds.Add(Convert.ToInt32(districtId));
                                                    }
                                                }
                                                data.Add(new BankLimitConfig()
                                                {
                                                    ProvinceId = Convert.ToInt32(provinceId),
                                                    Citys = citys,
                                                    CityId = Convert.ToInt32(cityId),
                                                    Districts = districts,
                                                    DistrictIds = districtIds
                                                });
                                            }
                                            else
                                            {
                                                if (secondRegion?.ChildRegions?.Where(x => String.Equals(x.CityId, Convert.ToInt32(cityId)))?.Count() > 0)
                                                {
                                                    data.Add(new BankLimitConfig()
                                                    {
                                                        ProvinceId = Convert.ToInt32(provinceId),
                                                        Citys = citys,
                                                        CityId = Convert.ToInt32(cityId),
                                                        Districts = districts,
                                                        DistrictIds = new List<int>()
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.Add(new BankLimitConfig()
                            {
                                ProvinceId = Convert.ToInt32(provinceId),
                                Citys = citys,
                                CityId = -1,
                                DistrictIds = new List<int>()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return data;
        }

        public Service.Shop.Models.Region.Region GetRegionByRegionId(int regionId)
        {
            Service.Shop.Models.Region.Region result = null;
            using (var client = new RegionClient())
            {
                var allProvinceResult = client.GetRegionByRegionId(regionId);
                allProvinceResult.ThrowIfException(true);
                result = allProvinceResult.Result;
            }
            return result ?? new Service.Shop.Models.Region.Region(); ;
        }
        /// <summary>
        /// 插入限购配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool InsertBankMRActivityLimitConfig(BankMRActivityLimitConfig config, string operateUser)
        {
            bool result = false;

            try
            {
                TuhuGrouponDbScopeManager.Execute(conn => result = BankMRActivityDal.InsertBankMRActivityLimitConfig(conn, config));
                if (result)
                    TuhuGrouponDbScopeManager.CreateTransaction(conn => result = BankMRActivityDal.InsertBankMRActivityLimitTimeConfig(conn, config.WeekTimeLimit.ToList()));

                UpdateBankMRActivityRoundLimitCount(config.ActivityId, config.RoundLimit, config.UserRoundLimit);
                var log = new BeautyOprLog
                {
                    LogType = "InsertBankMRActivityLimitConfig",
                    IdentityID = config.ActivityId.ToString(),
                    OldValue = null,
                    NewValue = JsonConvert.SerializeObject(config),
                    Remarks = $"插入银行美容限购配置,活动ID：{config.ActivityId}",
                    OperateUser = operateUser,
                };
                LoggerManager.InsertLog("BeautyOprLog", log);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 更新限购配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool UpdateBankMRActivityLimitConfig(BankMRActivityLimitConfig config, string operateUser)
        {
            bool result = false;

            try
            {
                var oldValue = FetchBankMRActivityLimitConfig(config.ActivityId);
                TuhuGrouponDbScopeManager.Execute(conn => result = BankMRActivityDal.UpdateBankMRActivityLimitConfig(conn, config));
                if (result)
                    TuhuGrouponDbScopeManager.CreateTransaction(conn => result = BankMRActivityDal.InsertBankMRActivityLimitTimeConfig(conn, config.WeekTimeLimit.ToList()));
                UpdateBankMRActivityRoundLimitCount(config.ActivityId, config.RoundLimit, config.UserRoundLimit);
                var log = new BeautyOprLog
                {
                    LogType = "UpdateBankMRActivityLimitConfig",
                    IdentityID = config.ActivityId.ToString(),
                    OldValue = JsonConvert.SerializeObject(oldValue),
                    NewValue = JsonConvert.SerializeObject(config),
                    Remarks = $"更新银行美容限购配置,活动ID:{config.ActivityId}",
                    OperateUser = operateUser,
                };
                LoggerManager.InsertLog("BeautyOprLog", log);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        public bool IsExistBankMRActivityUsersByRoundIds(IEnumerable<int> roundIds)
        {
            var isExist = false;
            try
            {
                TuhuGrouponDbScopeManagerReadOnly.Execute(conn => isExist = BankMRActivityDal.IsExistBankMRActivityUsersByRoundIds(conn, roundIds));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }

            return isExist;
        }
        /// <summary>
        /// 批量向活动场次中导入用户
        /// </summary>
        /// <param name="list">用户</param>
        /// <param name="roundIds">场次ID</param>
        /// <param name="operateUser">操作人</param>
        /// <returns></returns>
        public bool BatchImportBankMRActivityUsers(List<BankMRActivityUser> list, List<int> roundIds, string operateUser, string importRoundType, string note)
        {
            var result = false;
            string batchCode = $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}|{importRoundType}";
            try
            {
                if (list != null && list.Any() && roundIds != null && roundIds.Any())
                {
                    TuhuGrouponDbScopeManager.CreateTransaction(conn =>
                    {

                        foreach (var roundId in roundIds)
                        {
                            list.ForEach(r =>
                            {
                                r.ActivityRoundId = roundId;
                                r.BatchCode = batchCode;
                            });
                            var batchSize = 2000;
                            var index = 0;
                            while (index < list.Count)
                            {
                                var batch = list.Skip(index).Take(batchSize);
                                var importResult = handler.BatchImportBankMRActivityUsers(conn, batch);
                                if (!importResult)
                                {
                                    throw new Exception("银行美容第" + index + 1 + "批导入失败");
                                }
                                index += batchSize;
                            }
                            var log = new BeautyOprLog
                            {
                                LogType = "BatchImportBankMRActivityUsers",
                                IdentityID = $"{batchCode}",
                                OldValue = null,
                                NewValue = $"导入用户数量{list.Count()}",
                                Remarks = $"活动场次导入用户规则，场次ID:{roundId},{note}",
                                OperateUser = operateUser,
                            };
                            LoggerManager.InsertLog("BeautyOprLog", log);
                        }
                        result = true;
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("BatchImportBankMRActivityUsers", ex);
            }

            return result;
        }
        /// <summary>
        /// 下载限购上传记录
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public IEnumerable<ImportBankMRActivityRecordModel> GetImportBankMRActivityUsers(string batchCode)
        {
            return TuhuGroupon_GungnirManagerReadOnly.Execute(conn => BankMRActivityDal.GetImportBankMRActivityUsers(conn, batchCode));
        }
        /// <summary>
        /// 获取限购上传记录
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public IEnumerable<ImportBankMRActivityRecordModel> GetImportBankMRActivityUsersRecords(Guid activityId, out int count)
        {
            count = 0;
            var result = TuhuGroupon_GungnirManagerReadOnly.Execute(conn => BankMRActivityDal.GetImportBankMRActivityRecords(conn, activityId));
            count = result.Item1;
            return result.Item2;
        }
        public Tuple<bool, string> DeleteImportBankMRActivityByBatchCode(string batchCode, string operateUser)
        {
            try
            {
                var result = TuhuGrouponDbScopeManager.Execute(conn => BankMRActivityDal.DeleteImportBankMRActivityByBatchCode(conn, batchCode));
                if (result)
                {
                    var log = new BeautyOprLog
                    {
                        LogType = "DeleteImportBankMRActivityUsers",
                        IdentityID = $"{batchCode}",
                        OldValue = null,
                        NewValue = null,
                        Remarks = $"活动场次删除用户规则，batchCode:{batchCode},",
                        OperateUser = operateUser,
                    };
                    LoggerManager.InsertLog("BeautyOprLog", log);
                }
                return Tuple.Create(result, "");
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                return Tuple.Create(false, ex.Message);
            }

        }
        public SignleBankMRActivityUserModel GetSignleBankMRActivityUser(Guid activityId, string mobile, string card, string otherNo)
        {
            try
            {
                var result = TuhuGroupon_GungnirManagerReadOnly.Execute(conn => BankMRActivityDal.GetSignleBankMRActivityUser(conn, activityId, mobile, card, otherNo));

                return result;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                return null;
            }

        }
        /// <summary>
        /// 根据活动Id查询银行美容活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public BankMRActivityConfig FetchBankMRActivityConfigByActivityId(Guid activityId)
        {
            try
            {
                var result = TuhuGrouponDbScopeManagerReadOnly.Execute(conn =>
                    BankMRActivityDal.FetchBankMRActivityConfigByActivityId(conn, activityId));
                return result;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// 插入银行活动组配置
        /// </summary>
        /// <param name="config"></param>
        /// <param name="operateUser"></param>
        /// <returns></returns>
        public bool InsertBankActivityGroupConfig(BankActivityGroupConfig config, string operateUser)
        {
            bool result = false;

            try
            {
                TuhuGrouponDbScopeManager.Execute(conn => result = BankMRActivityDal.InsertBankActivityGroupConfig(conn, config));
                var log = new BeautyOprLog
                {
                    LogType = "InsertBankActivityGroupConfig",
                    IdentityID = config.ActivityId.ToString(),
                    OldValue = null,
                    NewValue = JsonConvert.SerializeObject(config),
                    Remarks = $"插入银行活动组配置,活动ID：{config.ActivityId}",
                    OperateUser = operateUser,
                };
                LoggerManager.InsertLog("BeautyOprLog", log);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 更新银行活动组名
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="groupId"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public bool UpdateBankActivityGroupNameByGroupId(Guid groupId, string groupName, string operateUser)
        {
            bool result = false;
            try
            {
                var oldValue = SelectBankActivityGroupConfigByGroupId(groupId, 1, 10).Item1.FirstOrDefault()?.GroupName;
                TuhuGrouponDbScopeManager.Execute(conn => result = BankMRActivityDal.UpdateBankActivityGroupNameByGroupId(conn, groupId, groupName));
                var log = new BeautyOprLog
                {
                    LogType = "UpdateBankActivityGroupNameByGroupId",
                    IdentityID = groupId.ToString(),
                    OldValue = oldValue,
                    NewValue = groupName,
                    Remarks = $"更新银行活动组名,GroupId:{groupId}",
                    OperateUser = operateUser,
                };
                LoggerManager.InsertLog("BeautyOprLog", log);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result;
        }

        /// <summary>
        /// 根据组名获取银行活动组配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public IEnumerable<BankActivityGroupConfig> SelectBankActivityGroupConfigsByGroupName(string groupName)
        {
            IEnumerable<BankActivityGroupConfig> result = null;
            try
            {
                TuhuGrouponDbScopeManagerReadOnly.Execute(conn => result = BankMRActivityDal.SelectBankActivityGroupConfigsByGroupName(conn, groupName));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 更新银行活动组配置
        /// </summary>
        /// <param name="config"></param>
        /// <param name="operateUser"></param>
        /// <returns></returns>
        public bool UpdateBankActivityGroupConfigByPKID(BankActivityGroupConfig config, string operateUser)
        {
            bool result = false;

            try
            {
                var oldValue = TuhuGrouponDbScopeManagerReadOnly.Execute(conn => BankMRActivityDal.SelectBankActivityGroupConfigByPKID(conn, config.PKID));
                TuhuGrouponDbScopeManager.Execute(conn => result = BankMRActivityDal.UpdateBankActivityGroupConfigByPKID(conn, config));
                var log = new BeautyOprLog
                {
                    LogType = "UpdateBankActivityGroupConfigByPKID",
                    IdentityID = config.PKID.ToString(),
                    OldValue = JsonConvert.SerializeObject(oldValue),
                    NewValue = JsonConvert.SerializeObject(config),
                    Remarks = $"更新银行活动组配置,PKID:{config.PKID}",
                    OperateUser = operateUser,
                };
                LoggerManager.InsertLog("BeautyOprLog", log);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        /// 分页获取银行活动组配置
        /// </summary>
        /// <returns></returns>
        public Tuple<IEnumerable<BankActivityGroupConfig>, int> SelectBankActivityGroupConfigs(int pageIndex, int pageSize)
        {
            Tuple<IEnumerable<BankActivityGroupConfig>, int> result = null;

            try
            {
                TuhuGroupon_GungnirManagerReadOnly.Execute(conn => result = BankMRActivityDal.SelectBankActivityGroupConfigs(conn, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 根据ID删除银行活动组配置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operateUser"></param>
        /// <returns></returns>
        public bool DeleteBankActivityGroupConfigByPKID(int pkid, string operateUser)
        {
            bool result = false;
            try
            {
                var oldValue = TuhuGrouponDbScopeManagerReadOnly.Execute(conn => BankMRActivityDal.SelectBankActivityGroupConfigByPKID(conn, pkid));
                TuhuGrouponDbScopeManager.Execute(conn => result = BankMRActivityDal.DeleteBankActivityGroupConfigByPKID(conn, pkid));
                var log = new BeautyOprLog
                {
                    LogType = "DeleteBankActivityGroupConfigByPKID",
                    IdentityID = pkid.ToString(),
                    OldValue = JsonConvert.SerializeObject(oldValue),
                    NewValue = null,
                    Remarks = $"删除银行活动组配置,{pkid}:{pkid}",
                    OperateUser = operateUser,
                };
                LoggerManager.InsertLog("BeautyOprLog", log);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result;
        }

        /// <summary>
        /// 根据组Id分页获取银行活动组配置
        /// </summary>
        /// <returns></returns>
        public Tuple<IEnumerable<BankActivityGroupConfig>, int> SelectBankActivityGroupConfigByGroupId(Guid groupId, int pageIndex, int pageSize)
        {
            Tuple<IEnumerable<BankActivityGroupConfig>, int> result = null;
            try
            {
                TuhuGroupon_GungnirManagerReadOnly.Execute(conn => result = BankMRActivityDal.SelectBankActivityGroupConfigsByGroupId(conn, groupId, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result;
        }

        /// <summary>
        /// 根据PKID获取银行活动组配置
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public BankActivityGroupConfig SelectBankActivityGroupConfigByPKID(int pkid)
        {
            BankActivityGroupConfig result = null;
            try
            {
                TuhuGrouponDbScopeManagerReadOnly.Execute(conn => result = BankMRActivityDal.SelectBankActivityGroupConfigByPKID(conn, pkid));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 批量导入银行活动白名单
        /// </summary>
        /// <param name="list">白名单</param>
        /// <param name="operateUser">操作人</param>
        /// <returns></returns>
        public bool BatchImportBankActivityWhiteUsers(List<BankActivityWhiteUsers> list, string operateUser)
        {
            var result = false;
            try
            {
                if (list != null && list.Any())
                {
                    TuhuGrouponDbScopeManager.CreateTransaction(conn =>
                    {
                        var batchSize = 2000;
                        var index = 0;
                        while (index < list.Count)
                        {
                            var batch = list.Skip(index).Take(batchSize);
                            var importResult = handler.BatchImportBankActivityWhiteUsers(conn, batch);
                            if (!importResult)
                            {
                                throw new Exception("银行活动白名单第" + index + 1 + "批导入失败");
                            }
                            index += batchSize;
                        }
                        var log = new BeautyOprLog
                        {
                            LogType = "BatchImportBankMRActivityUsers",
                            IdentityID = "",
                            OldValue = null,
                            NewValue = $"导入白名单数量{list.Count()}",
                            Remarks = "批量导入银行活动白名单",
                            OperateUser = operateUser,
                        };
                        LoggerManager.InsertLog("BeautyOprLog", log);
                        result = true;
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("BatchImportBankMRActivityUsers", ex);
            }

            return result;
        }
        /// <summary>
        /// 根据组配置Id获取导入的银行活动白名单
        /// </summary>
        /// <param name="groupConfigId"></param>
        /// <returns></returns>
        public IEnumerable<BankActivityWhiteUsers> SelectImportBankActivityWhiteUsersByGroupConfigId(int groupConfigId)
        {
            IEnumerable<BankActivityWhiteUsers> result = null;
            try
            {
                TuhuGrouponDbScopeManagerReadOnly.Execute(conn => result = BankMRActivityDal.SelectImportBankActivityWhiteUsersByGroupConfigId(conn, groupConfigId));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result;
        }

        /// <summary>
        /// 根据组Id获取已导入银行活动白名单
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IEnumerable<BankActivityWhiteUsers> SelectImportBankActivityWhiteUsersByGroupId(
            Guid groupId)
        {
            IEnumerable<BankActivityWhiteUsers> result = null;
            try
            {
                TuhuGrouponDbScopeManagerReadOnly.Execute(conn => result = BankMRActivityDal.SelectImportBankActivityWhiteUsersByGroupId(conn, groupId));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 分页获取银行美容活动展示配置
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public Tuple<int, IEnumerable<BankMrActivityDisplayConfigEntity>> SelectBankMrActivityDisplayConfigs(
    int pageIndex, int pageSize, int active)
        {
            try
            {
                return TuhuGrouponDbScopeManagerReadOnly.Execute(conn => BankMRActivityDal.SelectBankMrActivityDisplayConfigs(conn, pageIndex, pageSize, active));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return null;
        }
        /// <summary>
        /// 获取银行美容活动展示配置详情
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public BankMrActivityDisplayConfigEntity GetBankMrActivityDisplayConfigsDetail(
    int pkid)
        {
            try
            {
                return TuhuGrouponDbScopeManagerReadOnly.Execute(conn => BankMRActivityDal.GetBankMrActivityDisplayConfigsDetail(conn, pkid));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return null;
        }
        /// <summary>
        /// 新增银行美容活动展示配置
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="regionEntitys"></param>
        /// <returns></returns>
        public bool SaveBankMrActivityDisplayConfig(BankMrActivityDisplayConfigEntity entity, IEnumerable<BankMRActivityDisplayRegionEntity> regionEntitys)
        {
            var result = false;
            try
            {
                result = TuhuGrouponDbScopeManager.Execute(conn => BankMRActivityDal.SaveBankMrActivityDisplayConfig(conn, entity, regionEntitys));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 更新银行美容活动展示配置
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="regionEntitys"></param>
        /// <returns></returns>
        public bool UpdateBankMrActivityDisplayConfig(BankMrActivityDisplayConfigEntity entity, IEnumerable<BankMRActivityDisplayRegionEntity> regionEntitys)
        {
            var result = false;
            try
            {
                result = TuhuGrouponDbScopeManager.Execute(conn => BankMRActivityDal.UpdateBankMrActivityDisplayConfig(conn, entity, regionEntitys));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result;
        }

        #region 银行活动使用记录
        /// <summary>
        /// 获取银行活动使用记录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public List<BankMrUsageRecord> SelectBankMrActivityUsageRecode(string code, string mobile, int pageIndex, int pageSize)
        {
            IEnumerable<BankMRActivityCodeRecord> recordInfo = null;
            IEnumerable<BankMRActivityRoundConfig> roundInfo = null;
            IEnumerable<BankMRActivityConfig> activityInfo = null;
            IEnumerable<BankMRActivityLimitConfig> limitInfo = null;
            //IEnumerable<BankMRActivityUser> userInfo = null;
            IEnumerable<ServiceCode> codeInfo = null;
            try
            {
                TuhuGrouponDbScopeManagerReadOnly.Execute(conn =>
                {
                    recordInfo = BankMRActivityDal.GetBankMrAtivityCodeRecord(conn, code, mobile, pageIndex, pageSize);
                    if (recordInfo != null && recordInfo.Any())
                    {
                        codeInfo = CommonServices.KuaiXiuService.GetServiceCodeDetailsByCodes(recordInfo.Where(_ => !string.IsNullOrEmpty(_.ServiceCode)).Select(x => x.ServiceCode).ToList());
                        var roundIdList = recordInfo.Select(x => x.ActivityRoundId).Distinct().ToList();
                        roundInfo = BankMRActivityDal.GetBankMrAtivityRoundConfig(conn, roundIdList);
                        if (roundInfo != null && roundInfo.Any())
                        {
                            var activityIdList = roundInfo.Select(x => x.ActivityId).Distinct().ToList();
                            activityInfo = BankMRActivityDal.GetBankMrActivityConfig(conn, activityIdList);
                            //limitInfo = BankMRActivityDal.GetBankMrAtivityLimitConfig(conn, activityIdList);
                            //userInfo = BankMRActivityDal.GetBankMrAtivityUser(conn, roundIdList);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return ConvertBankMrUsageRecord(recordInfo, roundInfo, activityInfo, limitInfo,  codeInfo);
        }
        /// <summary>
        /// 信息转换
        /// </summary>
        /// <param name="records"></param>
        /// <param name="rounds"></param>
        /// <param name="activitys"></param>
        /// <param name="limits"></param>
        /// <param name="users"></param>
        /// <param name="codes"></param>
        /// <returns></returns>
        private List<BankMrUsageRecord> ConvertBankMrUsageRecord(IEnumerable<BankMRActivityCodeRecord> records,
            IEnumerable<BankMRActivityRoundConfig> rounds, IEnumerable<BankMRActivityConfig> activitys,
            IEnumerable<BankMRActivityLimitConfig> limits,IEnumerable<ServiceCode> codes)
        {
            List<BankMrUsageRecord> result = new List<BankMrUsageRecord>();
            try
            {
                if (records != null && records.Any())
                {
                    foreach (var record in records)
                    {
                        var code = codes?.Where(x => String.Equals(x.Code, record.ServiceCode))?.FirstOrDefault() ?? new ServiceCode();
                        var round = rounds?.Where(x => x.PKID == record.ActivityRoundId)?.FirstOrDefault() ?? new BankMRActivityRoundConfig();
                        var activity = activitys?.Where(x => x.ActivityId == round.ActivityId)?.FirstOrDefault() ?? new BankMRActivityConfig();
                        //var limit = limits?.Where(x => x.ActivityId == round.ActivityId)?.FirstOrDefault() ?? new BankMRActivityLimitConfig();
                        var user = TuhuGrouponDbScopeManagerReadOnly.Execute(conn => BankMRActivityDal.GetBankMrAtivityUserByRoundIdAndTime(conn, record.ActivityRoundId, record.CreateTime));
                        BankMrUsageRecord usageRecord = new BankMrUsageRecord()
                        {
                            PKID = record.PKID,
                            Mobile = !string.IsNullOrEmpty(record.Mobile) ? record.Mobile.Substring(0, 3) + "****" + record.Mobile.Substring(record.Mobile.Length - 4, 4) : record.Mobile,
                            OtherNum = !string.IsNullOrEmpty(record.OtherNum) ? record.OtherNum.Substring(0, 3) + "****" + record.OtherNum.Substring(record.OtherNum.Length - 4, 4) : record.OtherNum,
                            BankNum = !string.IsNullOrEmpty(record.BankCardNum) ? record.BankCardNum.Substring(0, 3) + "****" + record.BankCardNum.Substring(record.BankCardNum.Length - 4, 4) : record.BankCardNum,
                            ServiceCode = !string.IsNullOrEmpty(code.Code) ? code.Code.Substring(0, 3) + "****" + code.Code.Substring(code.Code.Length - 4, 4) : code.Code,
                            Status = (int)code.Status,
                            TuhuOrderId = code.TuhuOrderId,
                            VerifyTime = code.VerifyTime,
                            ShopId = code.InstallShopId,
                            ActivityId = round.ActivityId,
                            ValidityTime = activity.ValidDays,
                            //MobileDayLimit = user?.DayLimit ?? 0,
                            //BankDayLimit = user?.DayLimit ?? 0,
                            RoundStartTime = round.StartTime,
                            RoundEndTime = round.EndTime,
                            RoundDayLimit = user.LimitCount,
                            DayLimit = user.DayLimit,
                            Total = record.Total
                        };
                        result.Add(usageRecord);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return result;
        }
        /// <summary>
        /// 通过PKID获取信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="type"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public BankMRActivityCodeRecord SelectBankMrAtivityCodeRecordByPKID(int pkid, string type, string user)
        {
            BankMRActivityCodeRecord result = null;
            try
            {
                result = TuhuGrouponDbScopeManagerReadOnly.Execute(conn => BankMRActivityDal.GetBankMrAtivityCodeRecordByPKID(conn, pkid));
                LoggerManager.InsertLog("BeautyOprLog", new BeautyOprLog
                {
                    LogType = "SearchEncryptedMsg",
                    IdentityID = pkid.ToString(),
                    OldValue = null,
                    NewValue = JsonConvert.SerializeObject(result),
                    Remarks = $"{type} Mobile:{result?.Mobile},BankNum:{result?.BankCardNum},OtherNum:{result?.OtherNum},ServiceCode:{result?.ServiceCode}",
                    OperateUser = user,
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return result ?? new BankMRActivityCodeRecord();
        }
        #endregion
    }
}
