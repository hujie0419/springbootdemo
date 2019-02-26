using Common.Logging;
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
using Tuhu.Provisioning.DataAccess.Entity.Battery;

namespace Tuhu.Provisioning.Business.BatteryCouponPriceDisplay
{
    public class BatteryCouponPriceDisplayManager
    {
        private readonly Common.Logging.ILog Logger;
        private readonly IConnectionManager configurationConn =
     new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private readonly IConnectionManager configurationReadConn =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        private readonly IDBScopeManager dbScopeManagerConfigurationRead;
        private readonly IDBScopeManager dbScopeManagerConfiguration;
        private readonly string _user;
        private readonly DalBatteryCouponPriceDisplay _dal;

        public BatteryCouponPriceDisplayManager(string user)
        {
            Logger = LogManager.GetLogger(typeof(BatteryCouponPriceDisplayManager));
            dbScopeManagerConfiguration = new DBScopeManager(configurationConn);
            dbScopeManagerConfigurationRead = new DBScopeManager(configurationReadConn);
            _user = user;
            _dal = new DalBatteryCouponPriceDisplay();
        }

        /// <summary>
        /// 添加或更新蓄电池券后价展示配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpSertBatteryCouponPriceDisplay(BatteryCouponPriceDisplayModel model)
        {
            var result = false;
            try
            {
                var oldValue = dbScopeManagerConfigurationRead.Execute(conn =>
                    _dal.GetBatteryCouponPriceDisplay(conn, model.Pid));
                if (oldValue == null)
                {
                    var pkid = dbScopeManagerConfiguration.Execute
                        (conn => _dal.AddBatteryCouponPriceDisplay(conn, model));
                    result = pkid > 0;
                    if (result)
                    {
                        model.PKID = pkid;
                        model.CreateDateTime = DateTime.Now;
                        model.LastUpdateDateTime = DateTime.Now;
                        var log = new BatteryOprLogModel
                        {
                            LogType = "BatteryCouponPriceDisplay",
                            IdentityId = model.Pid,
                            OperationType = "Add",
                            OldValue = null,
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"添加蓄电池：{model.Pid}的券后价展示配置",
                            Operator = _user,
                        };
                        LoggerManager.InsertLog("BatteryOprLog", log);
                    }
                }
                else
                {
                    model.CreateDateTime = oldValue.CreateDateTime;
                    model.LastUpdateDateTime = DateTime.Now;
                    result = dbScopeManagerConfiguration.Execute(conn =>
                    _dal.UpdateBatteryCouponPriceDisplay(conn, model));
                    if (result)
                    {
                        var log = new BatteryOprLogModel
                        {
                            LogType = "BatteryCouponPriceDisplay",
                            IdentityId = model.Pid,
                            OperationType = "Update",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"更新蓄电池：{model.Pid}的券后价展示配置",
                            Operator = _user,
                        };
                        LoggerManager.InsertLog("BatteryOprLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UpSertBatteryCouponPriceDisplay", ex);
            }
            return result;
        }

        /// <summary>
        /// 批量添加或更新蓄电池券后价展示配置
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public bool MultUpSertBatteryCouponPriceDisplay(List<BatteryCouponPriceDisplayModel> models)
        {
            var result = false;
            try
            {
                var logs = new List<BatteryOprLogModel>();
                var addList = new List<BatteryCouponPriceDisplayModel>();
                var updateList = new List<BatteryCouponPriceDisplayModel>();
                foreach (var model in models)
                {
                    var oldValue = dbScopeManagerConfigurationRead.Execute(conn =>
                    _dal.GetBatteryCouponPriceDisplay(conn, model.Pid));
                    if (oldValue != null)
                    {
                        model.CreateDateTime = oldValue.CreateDateTime;
                        model.LastUpdateDateTime = DateTime.Now;
                        updateList.Add(model);
                        logs.Add(new BatteryOprLogModel
                        {
                            LogType = "BatteryCouponPriceDisplay",
                            IdentityId = model.Pid,
                            OperationType = "Update",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"更新蓄电池：{model.Pid}的券后价展示配置",
                            Operator = _user,
                        });
                    }
                    else
                    {
                        addList.Add(model);
                    }
                }
                dbScopeManagerConfiguration.CreateTransaction(conn =>
                {
                    foreach (var model in updateList)
                    {
                        var updateResult = _dal.UpdateBatteryCouponPriceDisplay(conn, model);
                        if (!updateResult)
                        {
                            throw new Exception($"UpdateBatteryCouponPriceDisplay失败, " +
                                $"待更新数据{JsonConvert.SerializeObject(model) }");
                        }
                    }
                    foreach (var model in addList)
                    {
                        var pkid = _dal.AddBatteryCouponPriceDisplay(conn, model);
                        var addResult = pkid > 0;
                        if (!addResult)
                        {
                            throw new Exception($"AddBatteryCouponPriceDisplay失败, " +
                               $"待添加数据{JsonConvert.SerializeObject(model) }");
                        }
                        model.PKID = pkid;
                        model.CreateDateTime = DateTime.Now;
                        model.LastUpdateDateTime = DateTime.Now;
                        logs.Add(new BatteryOprLogModel
                        {
                            LogType = "BatteryCouponPriceDisplay",
                            IdentityId = model.Pid,
                            OperationType = "Add",
                            OldValue = null,
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"添加蓄电池：{model.Pid}的券后价展示配置",
                            Operator = _user,
                        });
                    }
                    result = true;
                });
                if (result && logs.Any())
                {
                    logs.ForEach(s => LoggerManager.InsertLog("BatteryOprLog", s));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UpSertBatteryCouponPriceDisplay", ex);
            }
            return result;
        }

        /// <summary>
        /// 查询蓄电池券后价展示配置
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public async Task<List<BatteryCouponPriceDisplayViewModel>> SelectBatteryCouponPriceDisplay(string brand)
        {
            var results = new List<BatteryCouponPriceDisplayViewModel>();
            try
            {
                var configs = dbScopeManagerConfigurationRead.Execute(conn =>
                                   _dal.SelectBatteryCouponPriceDisplay(conn, brand));
                var batteries = configs?.Where(s => s.Oid > 0)?.Distinct()
                    ?.Select(s => new Service.Member.Request.ProductModel() { Pid = s.Pid, Oid = s.Oid })?.ToList();
                var promotionResult = await SelectProductUsefulPromotionAsync(batteries);//查询券后价和可用券
                if (promotionResult != null && promotionResult.Any())
                {
                    promotionResult.ForEach(promotion => promotion.OIDs.ForEach(oid =>
                    {
                        var config = configs.FirstOrDefault(f => f.Oid == oid);
                        if (config != null)
                        {
                            if (results.Any(r => r.Oid == oid))//一个Oid对应多个券
                            {
                                var result = results.FirstOrDefault(f => f.Oid == oid);
                                if (result.OriginalPrice > promotion.Minmoney)
                                {
                                    result.Coupons.Add(Guid.Parse(promotion.GetRuleGUID));//展示所有可用券
                                    var minPrice = result.OriginalPrice - promotion.Discount;
                                    result.Price = Math.Min(result.Price, minPrice);//展示最低券后价
                                }
                            }
                            else
                            {
                                if (config.OriginalPrice >= promotion.Minmoney)
                                {
                                    results.Add(new BatteryCouponPriceDisplayViewModel()
                                    {
                                        PKID = config.PKID,
                                        Brand = config.Brand,
                                        Pid = config.Pid,
                                        DisplayName = config.DisplayName,
                                        Oid = config.Oid,
                                        IsShow = config.IsShow,
                                        CreateDateTime = config.CreateDateTime,
                                        LastUpdateDateTime = config.LastUpdateDateTime,
                                        Coupons = new List<Guid>(1) { Guid.Parse(promotion.GetRuleGUID) },
                                        OriginalPrice = config.OriginalPrice,
                                        Price = config.OriginalPrice - promotion.Discount
                                    });
                                }
                            }
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SelectBatteryCouponPriceDisplay", ex);
            }
            return results;
        }

        /// <summary>
        /// 蓄电池券后价展示配置是否重复
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistBatteryCouponPriceDisplay(BatteryCouponPriceDisplayModel model)
        {
            var result = true;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute(conn =>
                  _dal.IsExistBatteryCouponPriceDisplay(conn, model));
            }
            catch (Exception ex)
            {
                Logger.Error("IsExistBatteryCouponPriceDisplay", ex);
            }
            return result;
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        public async Task<bool> RemoveCache(List<string> pids)
        {
            var result = false;
            try
            {
                using (var client = new Service.BaoYang.CacheClient())
                {
                    var batchSize = 200;
                    for (var i = 0; i < Math.Ceiling(pids.Count * 1.0 / batchSize); i++) //分批清除缓存 每批次200个
                    {
                        var clientResult = await client.RemoveByTypeAsync
                                ("BatteryCouponPriceDisplay", pids.Skip(i * batchSize).Take(batchSize).ToList());
                        result = clientResult.Success;
                        if (!result && clientResult.Exception != null)
                        {
                            Logger.Warn("调用BaoYang服务RemoveByTypeAsync失败", clientResult.Exception);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("RemoveCache", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取蓄电池券后价
        /// </summary>
        /// <param name="oids"></param>
        /// <returns></returns>
        private async Task<List<Service.Member.Models.ProductGetRuleModel>> SelectProductUsefulPromotionAsync(List<Service.Member.Request.ProductModel> products)
        {
            var result = new List<Service.Member.Models.ProductGetRuleModel>();
            if (products != null && products.Any())
            {
                using (var client = new Service.Member.PromotionClient())
                {
                    var batchSize = 200;
                    for (var i = 0; i < Math.Ceiling(products.Count * 1.0 / 200); i++) ////分批查询 每批次200个
                    {
                        var clientResult = await client.SelectProductUsefulPromotionAsync(
                            new Service.Member.Request.SelectProductUsefulPromotionRequest()
                            {
                                UserId = null,
                                OrderPayMethod = 1,
                                InstallType = 1,
                                EnabledGroupBuy = false,
                                ProductList = products.Skip(i * batchSize).Take(batchSize).ToList()
                            });
                        if (!clientResult.Success && clientResult.Exception != null)
                        {
                            throw new Exception("调用Member服务SelectProductPromotionGetRuleByOids失败", clientResult.Exception);
                        }
                        result.AddRange(clientResult.Result);
                    }
                }
            }
            return result;
        }
    }
}
