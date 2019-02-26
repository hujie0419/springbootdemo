using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.CouponActivityConfigV2;
using Tuhu.Provisioning.DataAccess.Request;
using Tuhu.Service.Activity;

namespace Tuhu.Provisioning.Business
{
    public class CouponActivityConfigManagerV2
    {
        private static readonly Common.Logging.ILog Logger;
        private static readonly IConnectionManager configConnString =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private static readonly IConnectionManager configReadConnString =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);

        private static readonly IDBScopeManager dbScopeManagerConfigRead;
        private static readonly IDBScopeManager dbScopeManagerConfig;
        private static readonly IDBScopeManager dbScopeManagerConfigurationRead;
        private static readonly IDBScopeManager dbScopeManagerConfiguration;
        static CouponActivityConfigManagerV2()
        {
            Logger = Common.Logging.LogManager.GetLogger(typeof(CouponActivityConfigManagerV2));
            dbScopeManagerConfig = new DBScopeManager(configConnString);
            dbScopeManagerConfigRead = new DBScopeManager(configReadConnString);
            dbScopeManagerConfiguration = new DBScopeManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
            dbScopeManagerConfigurationRead = new DBScopeManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        }
        /// <summary>
        /// 获取蓄电池/加油卡浮动配置分页列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaoYangResultEntity<Tuple<int, List<CouponActivityConfigV2Model>>> GetCouponActivityConfigs(CouponActivityConfigPageRequestModel request)
        {
            var result = new BaoYangResultEntity<Tuple<int, List<CouponActivityConfigV2Model>>>() { Status = false };
            try
            {
                dbScopeManagerConfigurationRead.Execute(conn =>
                {
                    var tupleConfigs = DalCouponActivityConfigV2.GetCouponActivityConfigs(conn, request);
                    if (tupleConfigs == null || !tupleConfigs.Item2.Any() || tupleConfigs.Item1 <= 0)
                    {
                        result.Status = true;
                        result.Msg = "未查找到数据";
                    }
                    else
                    {
                        var ChannelConfigs = DalCouponActivityConfigV2.GetChannelConfigs(conn, tupleConfigs.Item2.Select(x => x.Id).ToList());
                        var ChannelConfigDic = ChannelConfigs.GroupBy(x => x.ConfigId).Select(x => new { x.Key, x }).ToDictionary(k => k.Key, v => v.x); 
                        foreach (var item in tupleConfigs.Item2)
                        {
                            item.Channels = item.Channel == null ? new List<string>() : item.Channel.Split(',').ToList();
                            if (ChannelConfigDic.ContainsKey(item.Id))
                            {
                                item.ChannelConfigs = ChannelConfigDic[item.Id].ToList();
                            }
                        }
                        result.Status = true;
                        result.Data = tupleConfigs;
                    }
                });
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Msg = "程序遇见错误,请联系管理员";
                Logger.Error("GetCouponActivityConfigs", ex);
            }
            return result;
        }
        /// <summary>
        /// 获取单个浮动设置模型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaoYangResultEntity<CouponActivityConfigV2Model> GetCouponActivityConfig(int id)
        {
            var result = new BaoYangResultEntity<CouponActivityConfigV2Model>() { Status = false };
            try
            {
                dbScopeManagerConfigurationRead.Execute(conn =>
                {
                    var config = DalCouponActivityConfigV2.GetCouponActivityConfigById(conn, id);
                    if (config == null)
                    {
                        result.Status = true;
                        result.Msg = "没有查询到数据";
                    }
                    else
                    {
                        config.ChannelConfigs = DalCouponActivityConfigV2.GetChannelConfigs(conn, config.Id, string.Empty);
                        result.Status = true;
                        result.Data = config;
                    }
                });
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Msg = "程序遇见错误,请联系管理员";
                Logger.Error("GetCouponActivityConfigs", ex);
            }
            return result;
        }
        /// <summary>
        /// 清除蓄电池/加油卡浮动配置缓存
        /// </summary>
        /// <returns></returns>
        public async Task<BaoYangResultEntity<bool>> RemoveCouponActivityConfigCache(int id)
        {
            var result = new BaoYangResultEntity<bool>() { Status = false };
            try
            {
                var model = GetCouponActivityConfig(id).Data;
                if (model != null)
                {
                    using (var cacheCilent = new CacheClient())
                    {
                        var cacheName = "CouponActivityConfig";
                        var cacheKey = $"CouponActivityConfig/{model.ActivityNum}/{model.Type}";
                        var cacheResult = await cacheCilent.RemoveRedisCacheKeyAsync(cacheName, cacheKey);
                        if (cacheResult.Success)
                        {
                            cacheKey = $"CouponActivityChannelConfig/{model.Id}";
                            cacheResult = await cacheCilent.RemoveRedisCacheKeyAsync(cacheName, cacheKey);
                            result.Status = true;
                            result.Data = cacheResult.Success;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Msg = "程序遇见错误,请联系管理员";
                Logger.Error("GetCouponActivityConfigs", ex);
            }

            return result;
        }
        /// <summary>
        /// 根据ID 删除单个模型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BaoYangResultEntity<bool>> DeleteChannelConfigsByConfigId(int id, string name)
        {
            var result = new BaoYangResultEntity<bool>() { Status = false };
            try
            {
                var oldModel = GetCouponActivityConfig(id).Data;
                await RemoveCouponActivityConfigCache(id);
                dbScopeManagerConfiguration.CreateTransaction(conn =>
                {
                    if (DalCouponActivityConfigV2.DeleteCouponActivityConfig(conn, id))
                    {
                        var configIds = new List<int> { id };
                        result.Data = DalCouponActivityConfigV2.DeleteChannelConfigsByConfigId(conn, configIds);
                        result.Status = true;
                        result.Data = true;
                    }
                    else
                    {
                        result.Data = false;
                    }
                });
                if (result.Status && oldModel != null)
                {
                    SaveCouponActivityConfigLog(oldModel, null, name);
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Msg = "程序遇见错误,请联系管理员";
                Logger.Error("GetCouponActivityConfigs", ex);
            }
            return result;
        }
        /// <summary>
        /// 修改蓄电池/加油卡浮动配置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaoYangResultEntity<bool> UpdateCouponActivityConfig(CouponActivityConfigV2Model configModel, string name)
        {
            var result = new BaoYangResultEntity<bool>() { Status = false };
            try
            {
                var oldModel = GetCouponActivityConfig(configModel.Id).Data;
                dbScopeManagerConfiguration.CreateTransaction(conn =>
                {
                    if (DalCouponActivityConfigV2.UpdateCouponActivityConfig(conn, configModel))
                    {
                        if (configModel.ChannelConfigs != null && configModel.ChannelConfigs.Any())
                        {
                            var configIds = new List<int> { configModel.Id };
                            DalCouponActivityConfigV2.DeleteChannelConfigsByConfigId(conn, configIds);
                            foreach (var item in configModel.ChannelConfigs)
                            {
                                DalCouponActivityConfigV2.InsertChannelConfigsById(conn, item);
                            }

                            result.Status = true;
                            result.Data = true;

                        }
                        else
                        {
                            result.Status = true;
                            result.Data = true;
                        }

                    }
                    else
                    {
                        result.Data = false;
                    }
                });
                if (result.Status)
                {
                    SaveCouponActivityConfigLog(oldModel, configModel, name);
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Msg = "程序遇见错误,请联系管理员";
                Logger.Error("GetCouponActivityConfigs", ex);
            }
            return result;
        }
        /// <summary>
        /// 添加蓄电池/加油卡浮动配置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BaoYangResultEntity<bool> InsertCouponActivityConfig(CouponActivityConfigV2Model request, string name)
        {

            var result = new BaoYangResultEntity<bool>() { Status = false };
            try
            {

                dbScopeManagerConfiguration.CreateTransaction(conn =>
                {
                    request.ActivityNum = Guid.NewGuid().ToString();
                    var configId = DalCouponActivityConfigV2.InsertCouponActivityConfig(conn, request);
                    if (configId > 0)
                    {
                        if (request.ChannelConfigs != null && request.ChannelConfigs.Any())
                        {
                            foreach (var item in request.ChannelConfigs)
                            {
                                item.ConfigId = configId;
                                DalCouponActivityConfigV2.InsertChannelConfigsById(conn, item);
                            }
                        }
                        result.Status = true;
                        result.Data = true;
                        request.Id = configId;

                    }
                    else
                    {
                        result.Data = false;
                    }
                });
                if (result.Status)
                {
                    SaveCouponActivityConfigLog(null, request, name);
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Msg = "程序遇见错误,请联系管理员";
                Logger.Error("GetCouponActivityConfigs", ex);
            }

            return result;
        }

        /// <summary>
        /// 验证蓄电池/加油卡浮动配置实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaoYangResultEntity<bool> Validation(CouponActivityConfigV2Model model)
        {
            var result = new BaoYangResultEntity<bool>() { Status = true };
            if (model.ChannelConfigs != null && model.ChannelConfigs.Any())
            {
                foreach (var item in model.ChannelConfigs)
                {
                    if (item.Type == ChannelConfigType.URL.ToString() && string.IsNullOrWhiteSpace(item.Url))
                    {
                        result.Status = false;
                        result.Msg = $"类型{item.Channel}必须输入跳转地址";
                        break;
                    }
                    if (item.Type == ChannelConfigType.Coupon.ToString())
                    {
                        if (item.GetRuleGUID == Guid.Empty)
                        {
                            result.Status = false;
                            result.Msg = $"优惠券码为空或不符合优惠券码规则";
                            break;
                        }
                        //验证优惠券
                        PromotionActivityManager manager = new PromotionActivityManager();
                        SE_GetPromotionActivityCouponInfoConfig Couponresult = manager.GetCouponValidate(item.GetRuleGUID);
                        if (Couponresult == null)
                        {
                            result.Status = false;
                            result.Msg = $"优惠券码{item.GetRuleGUID.ToString()} 验证失败";
                            break;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public BaoYangResultEntity<bool> SaveCouponActivityConfigLog(CouponActivityConfigV2Model oldModel, CouponActivityConfigV2Model newmodel, string name)
        {
            BaoYangResultEntity<bool> result = new BaoYangResultEntity<bool>() { Status = true };
            var log = new BaoYangOprLog()
            {
                CreateTime = DateTime.Now,
                LogType = "BaoYangCouponActivityConfigSetting",
                Remarks = oldModel == null ? "添加" : (newmodel == null ? "删除" : "修改"),
                IdentityID = newmodel == null ? oldModel.Id.ToString() : newmodel.Id.ToString(),
                NewValue = JsonConvert.SerializeObject(newmodel ?? new CouponActivityConfigV2Model()),
                OldValue = JsonConvert.SerializeObject(oldModel ?? new CouponActivityConfigV2Model()),
                OperateUser = name
            };
            LoggerManager.InsertLog("BYOprLog", log);
            return result;
        }

    }
}
