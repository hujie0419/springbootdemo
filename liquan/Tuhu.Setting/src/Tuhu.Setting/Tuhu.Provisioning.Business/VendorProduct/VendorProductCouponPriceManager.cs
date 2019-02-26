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
using Tuhu.Provisioning.Business.ServiceProxy;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.VendorProduct.Request;

namespace Tuhu.Provisioning.Business
{
    public class VendorProductCouponPriceManager : VendorProductCommonManager

    {
        private readonly Common.Logging.ILog Logger;
        private readonly IConnectionManager configurationConn =
     new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private readonly IConnectionManager configurationReadConn =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        private readonly IDBScopeManager dbScopeManagerConfigurationRead;
        private readonly IDBScopeManager dbScopeManagerConfiguration;
        private readonly DalVendorProductCouponPriceConfig _dal;
        private readonly VendorProductService _vendorProductService;

        public VendorProductCouponPriceManager()
        {
            Logger = LogManager.GetLogger(typeof(VendorProductCouponPriceManager));
            dbScopeManagerConfiguration = new DBScopeManager(configurationConn);
            dbScopeManagerConfigurationRead = new DBScopeManager(configurationReadConn);
            _dal = new DalVendorProductCouponPriceConfig();
            _vendorProductService = new VendorProductService();
        }

        /// <summary>
        /// 添加或更新券后价展示配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpSertVendorProductCouponPriceConfig(VendorProductCouponPriceConfigModel model, string user)
        {
            var result = false;
            try
            {
                var oldValue = dbScopeManagerConfigurationRead.Execute(conn =>
                    _dal.GetVendorProductCouponPriceConfig(conn, model.ProductType, model.Pid));
                if (oldValue == null)
                {
                    var pkid = dbScopeManagerConfiguration.Execute
                        (conn => _dal.AddVendorProductCouponPriceConfig(conn, model));
                    result = pkid > 0;
                    if (result)
                    {
                        model.PKID = pkid;
                        model.CreateDateTime = DateTime.Now;
                        model.LastUpdateDateTime = DateTime.Now;
                        var log = new OprVendorProductModel()
                        {
                            LogType = "VendorProductCouponPriceConfig",
                            IdentityId = $"{model.ProductType}_{model.Pid}",
                            OldValue = null,
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"添加{model.ProductType}：{model.Pid}的券后价展示配置",
                            OperateUser = user,
                        };
                        LoggerManager.InsertLog("OprVendorProduct", log);
                    }
                }
                else
                {
                    model.CreateDateTime = oldValue.CreateDateTime;
                    model.LastUpdateDateTime = DateTime.Now;
                    result = dbScopeManagerConfiguration.Execute(conn =>
                    _dal.UpdateVendorProductCouponPriceConfig(conn, model));
                    if (result)
                    {
                        var log = new OprVendorProductModel()
                        {
                            LogType = "VendorProductCouponPriceConfig",
                            IdentityId = $"{model.ProductType}_{model.Pid}",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"更新{model.ProductType}：{model.Pid}的券后价展示配置",
                            OperateUser = user,
                        };
                        LoggerManager.InsertLog("OprVendorProduct", log);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UpSertVendorProductCouponPriceConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 批量添加或更新券后价展示配置
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public bool MultUpSertVendorProductCouponPriceConfig(List<VendorProductCouponPriceConfigModel> models, string user)
        {
            var result = false;
            try
            {
                var logs = new List<OprVendorProductModel>();
                var addList = new List<VendorProductCouponPriceConfigModel>();
                var updateList = new List<VendorProductCouponPriceConfigModel>();
                foreach (var model in models)
                {
                    var oldValue = dbScopeManagerConfigurationRead.Execute(conn =>
                    _dal.GetVendorProductCouponPriceConfig(conn, model.ProductType, model.Pid));
                    if (oldValue != null)
                    {
                        model.CreateDateTime = oldValue.CreateDateTime;
                        model.LastUpdateDateTime = DateTime.Now;
                        updateList.Add(model);
                        logs.Add(new OprVendorProductModel()
                        {
                            LogType = "VendorProductCouponPriceConfig",
                            IdentityId = $"{model.ProductType}_{model.Pid}",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"更新{model.ProductType}：{model.Pid}的券后价展示配置",
                            OperateUser = user,
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
                        var updateResult = _dal.UpdateVendorProductCouponPriceConfig(conn, model);
                        if (!updateResult)
                        {
                            throw new Exception($"UpdateVendorProductCouponPriceConfig失败, " +
                                $"待更新数据{JsonConvert.SerializeObject(model) }");
                        }
                    }
                    foreach (var model in addList)
                    {
                        var pkid = _dal.AddVendorProductCouponPriceConfig(conn, model);
                        var addResult = pkid > 0;
                        if (!addResult)
                        {
                            throw new Exception($"AddVendorProductCouponPriceConfig失败, " +
                               $"待添加数据{JsonConvert.SerializeObject(model) }");
                        }
                        model.PKID = pkid;
                        model.CreateDateTime = DateTime.Now;
                        model.LastUpdateDateTime = DateTime.Now;
                        logs.Add(new OprVendorProductModel
                        {
                            LogType = "VendorProductCouponPriceConfig",
                            IdentityId = $"{model.ProductType}_{model.Pid}",
                            OldValue = null,
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"添加{model.ProductType}：{model.Pid}的券后价展示配置",
                            OperateUser = user,
                        });
                    }
                    result = true;
                });
                if (result && logs.Any())
                {
                    logs.ForEach(s => LoggerManager.InsertLog("OprVendorProduct", s));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UpSertVendorProductCouponPriceConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 查询券后价展示配置
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public async Task<List<VendorProductCouponPriceConfigViewModel>> SelectVendorProductCouponPriceConfig
            (string productType, string brand, string user)
        {
            var results = new List<VendorProductCouponPriceConfigViewModel>();
            try
            {
                var pids = (await GetPidsByBrandFromCache(productType, brand))?.Item1;
                if (pids != null && pids.Any())
                {
                    var configs = dbScopeManagerConfigurationRead.Execute(conn =>
                                   _dal.SelectVendorProductCouponPriceConfig(conn, pids, productType, brand));
                    var products = configs?.Where(s => s.Oid > 0)?.Distinct()
                        ?.Select(s => new Service.Member.Request.ProductModel() { Pid = s.Pid, Oid = s.Oid })?.ToList();
                    var promotionResult = await SelectProductUsefulPromotionAsync(products);//查询券后价和可用券
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
                                        results.Add(new VendorProductCouponPriceConfigViewModel()
                                        {
                                            PKID = config.PKID,
                                            ProductType = string.IsNullOrEmpty(config.ProductType) ? productType : config.ProductType,
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
            }
            catch (Exception ex)
            {
                results = null;
                Logger.Error("SelectVendorProductCouponPriceConfig", ex);
            }
            return results;
        }

        /// <summary>
        /// 券后价展示配置是否重复
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistVendorProductCouponPriceConfig(VendorProductCouponPriceConfigModel model)
        {
            var result = true;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute(conn =>
                  _dal.IsExistVendorProductCouponPriceConfig(conn, model));
            }
            catch (Exception ex)
            {
                Logger.Error("IsExistVendorProductCouponPriceConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        public async Task<bool> RemoveCache(string productType, List<string> pids)
        {
            var result = false;
            try
            {
                var request = new RemoveCacheByTypeRequest()
                {
                    Type = "CouponPriceDisplay",
                    Data = pids.Select(pid => $"{productType}/{pid}").ToList()
                };
                result = await _vendorProductService.RemoveCacheByType(request);
            }
            catch (Exception ex)
            {
                Logger.Error("RemoveCache", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取券后价
        /// </summary>
        /// <param name="oids"></param>
        /// <returns></returns>
        private async Task<List<Service.Member.Models.ProductGetRuleModel>> SelectProductUsefulPromotionAsync
            (List<Service.Member.Request.ProductModel> products)
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
            return result;//防止异步调用导致的多次查询结果不一致
        }
    }
}
