using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.VipBaoYangPackage;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.DAO.VipBaoYangPackage;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.VipBaoYangPackage;
using Tuhu.Service.ConfigLog;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.Service.Member.Request;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Product.Models;
using static Tuhu.Provisioning.DataAccess.Entity.VipPaintPackageModel;

namespace Tuhu.Provisioning.Business.VipPaintPackage
{
    public class VipPaintPackageManager
    {
        private static readonly Common.Logging.ILog Logger;
        private static readonly IConnectionManager configurationConn =
     new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private static readonly IConnectionManager configurationReadConn =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager productcatalogReadConn =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_productcatalog_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager tuhuLogConn =
     new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString);
        private static readonly IDBScopeManager dbScopeManagerConfigurationRead;
        private static readonly IDBScopeManager dbScopeManagerConfiguration;
        private static readonly IDBScopeManager dbScopeManagerProductcatalogRead;
        private static readonly IDBScopeManager dbScopeManagerTuhulog;

        static VipPaintPackageManager()
        {
            Logger = LogManager.GetLogger(typeof(VipPaintPackageManager));
            dbScopeManagerConfiguration = new DBScopeManager(configurationConn);
            dbScopeManagerConfigurationRead = new DBScopeManager(configurationReadConn);
            dbScopeManagerProductcatalogRead = new DBScopeManager(productcatalogReadConn);
            dbScopeManagerTuhulog = new DBScopeManager(tuhuLogConn);
        }

        #region 喷漆大客户套餐配置

        /// <summary>
        /// 添加喷漆大客户套餐配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddPackageConfig(VipPaintPackageConfigModel model)
        {
            var result = false;
            try
            {
                var oldValue = dbScopeManagerConfigurationRead.Execute(conn =>
                DalVipPaintPackage.GetPackageConfig(conn, model.PackageName));
                dbScopeManagerConfiguration.CreateTransaction
                   (conn =>
                   {
                       var addResult = false;
                       if (oldValue == null)
                       {
                           var pkid = DalVipPaintPackage.AddPackageConfig(conn, model);
                           addResult = pkid > 0;
                           model.PKID = addResult ? pkid : 0;
                       }
                       if (addResult)
                       {
                           model.PackagePid = $"FU-PQXB-KAT|{model.PKID.ToString()}";
                           var pidResult = DalVipPaintPackage.UpdatePackagePID(conn, model.PKID, model.PackagePid);
                           if (pidResult)
                           {
                               var packagePid = CreatePaintPackageProduct(model.PKID.ToString(), model.PackageName, model.Operator);
                               if (string.IsNullOrWhiteSpace(packagePid))
                               {
                                   throw new Exception($"CreatePaintPackageProduct失败,待创建产品{JsonConvert.SerializeObject(model)}");
                               }
                               else if (!string.Equals(packagePid, model.PackagePid))
                               {
                                   model.PackagePid = packagePid;
                                   result = DalVipPaintPackage.UpdatePackagePID(conn, model.PKID, model.PackagePid);
                               }
                               else
                               {
                                   result = true;
                               }
                           }
                           else
                           {
                               throw new Exception($"AddVipPackageConfig失败,待插入配置{JsonConvert.SerializeObject(model)}");
                           }
                       }
                   });
                if (result)
                {
                    model.CreateDateTime = DateTime.Now;
                    model.LastUpdateDateTime = DateTime.Now;
                    var log = new
                    {
                        ObjectId = $"PackageConfig_{model.PackageName}",
                        ObjectType = "VipPaintPackage",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(model),
                        Remark = $"添加喷漆大客户套餐:{model.PackageName}的配置",
                        Creator = model.Operator
                    };
                    LoggerManager.InsertLog("CommonConfigLog", log);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("AddVipPackageConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 删除喷漆大客户套餐配置
        /// </summary>
        /// <param name="carNoPrefix"></param>
        /// <param name="surfaceCount"></param>
        /// <param name="servicePid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeletePackageConfig(string packageName, string user)
        {
            var result = true;
            try
            {
                var oldValue = dbScopeManagerConfigurationRead.Execute(conn =>
                DalVipPaintPackage.GetPackageConfig(conn, packageName));
                if (oldValue != null)
                {
                    result = dbScopeManagerConfiguration.Execute(conn => DalVipPaintPackage.DeletePackageConfig(conn, oldValue.PKID, user));
                    if (result)
                    {
                        var log = new
                        {
                            ObjectId = $"PackageConfig_{oldValue.PackageName}",
                            ObjectType = "VipPaintPackage",
                            BeforeValue = JsonConvert.SerializeObject(oldValue),
                            AfterValue = "",
                            Remark = $"删除喷漆大客户套餐:{oldValue.PackageName}的配置",
                            Creator = user,
                        };
                        LoggerManager.InsertLog("CommonConfigLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("DeleteVipPaintPackageConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 查看喷漆大客户套餐配置
        /// </summary>
        /// <param name="packagePid"></param>
        /// <param name="vipUserId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<List<VipPaintPackageConfigModel>, int> SelectPackageConfig
            (string packagePid, Guid vipUserId, int pageIndex, int pageSize)
        {
            var result = null as List<VipPaintPackageConfigModel>;
            var totalCount = 0;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute(conn =>
                 DalVipPaintPackage.SelectPackageConfig(conn, packagePid, vipUserId, pageIndex, pageSize, out totalCount));
                var vipUsers = new VipBaoYangPackageManager().GetAllBaoYangPackageUser();
                if (result != null && result.Any() && vipUsers != null && vipUsers.Any())
                {
                    result.ForEach(s =>
                    {
                        s.VipUserName = vipUsers.FirstOrDefault(v => string.Equals(v.VipUserId, s.VipUserId.ToString()))?.VipUserName;
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SelectVipPaintPackageConfig", ex);
            }
            return Tuple.Create(result, totalCount);
        }

        /// <summary>
        /// 喷漆套餐是否存在
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool IsExistPackageConfig(string packageName, int pkid)
        {
            var result = false;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute(conn =>
                  DalVipPaintPackage.IsExistPackageConfig(conn, packageName, pkid));
            }
            catch (Exception ex)
            {
                Logger.Error("IsExistVipPaintPackageConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 创建套餐对应产品
        /// </summary>
        /// <param name="variantId"></param>
        /// <param name="displayName"></param>
        /// <param name="user"></param>
        /// <returns>套餐Pid</returns>
        private static string CreatePaintPackageProduct(string variantId, string displayName, string user)
        {
            WholeProductInfo info = new WholeProductInfo()
            {
                ProductID = "FU-PQXB-KAT",
                VariantID = variantId,
                TaxRate = 0.006M,
                DefinitionName = "Service",
                UseCategoryPricing = false,
                PrimaryParentCategory = "DKHPQTC",
                Image_filename = "/Images/Products/f7bb/9327/a10280c22b32649423a82b62_w800_h800.png",
                CP_Tire_ROF = "非防爆",
                invoice = "Yes",
                gift = "Full",
                CP_ShuXing3 = "商品安装服务",
                stockout = false,
                IsDaiFa = false,
                DisplayName = displayName,
                Description = "<p> < br /></ p > ",
                CP_Brand = "途虎/Tuhu",
                Name = "喷漆大客户套餐"
            };
            var result = BaoYangExternalService.CreateProductV2(info, user, ChannelType.Tuhu);
            return result;
        }

        #endregion

        #region 给用户塞券

        /// <summary>
        /// 塞券记录是否存在
        /// 存在塞券记录的套餐无法删除
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public bool IsExistPromotionRecord(int packageId)
        {
            var result = true;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute(conn =>
                 DalVipPaintPackage.IsExistPromotionRecord(conn, packageId));
            }
            catch (Exception ex)
            {
                Logger.Error("IsExistPromotionRecord", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取配置了套餐的喷漆大客户
        /// </summary>
        /// <returns></returns>
        public List<VipSimpleUser> GetPaintVipUsers()
        {
            var result = new List<VipSimpleUser>();
            try
            {
                var vipUserIds = dbScopeManagerConfiguration.Execute(conn => DalVipPaintPackage.GetPaintVipUsers(conn));
                var allVipUsers = new VipBaoYangPackageManager().GetAllBaoYangPackageUser();
                if (vipUserIds != null && vipUserIds.Any() && allVipUsers != null && allVipUsers.Any())
                {
                    result = allVipUsers.Where(s => vipUserIds.Contains(s.VipUserId)).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllPaintVipUsers", ex);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 获取喷漆大客户下配置的喷漆套餐
        /// </summary>
        /// <param name="vipUserId"></param>
        /// <returns></returns>
        public List<VipPaintPackageSimpleModel> GetVipPaintPackages(Guid vipUserId)
        {
            var result = null as List<VipPaintPackageSimpleModel>;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute(conn => DalVipPaintPackage.GetVipPaintPackages(conn, vipUserId.ToString()));
            }
            catch (Exception ex)
            {
                Logger.Error("GetVipPaintPackages", ex);
            }
            return result;
        }

        /// <summary>
        /// 文件是否已导入过
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool IsExistVipPaintFile(string fileName)
        {
            var result = true;
            try
            {
                result = dbScopeManagerTuhulog.Execute(conn => DalVipPaintPackage.IsExistVipPaintFile(conn, fileName));
            }
            catch (Exception ex)
            {
                Logger.Error("IsExistVipPaintFile", ex);
            }
            return result;
        }

        /// <summary>
        /// 添加塞券记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertPromotionRecord(VipPaintPackagePromotionRecordModel model)
        {
            var result = false;
            try
            {
                var pkid = dbScopeManagerConfiguration.Execute(conn =>
                DalVipPaintPackage.InsertPromotionRecord(conn, model));
                result = pkid > 0;
                if (pkid > 0)
                {
                    model.PKID = pkid;
                    model.CreateDateTime = DateTime.Now;
                    model.LastUpdateDateTime = DateTime.Now;
                    var log = new
                    {
                        ObjectType = "VipPaintPackage",
                        ObjectId = $"PromotionRecord_{model.BatchCode}",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(model),
                        Remark = $"喷漆大客户套餐塞券,批次号:{model.BatchCode}",
                        Creator = model.CreateUser
                    };
                    LoggerManager.InsertLog("CommonConfigLog", log);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("InsertBaoYangPackagePromotionRecord", ex);
            }
            return result;
        }

        /// <summary>
        /// 验证优惠券
        /// </summary>
        /// <param name="ruleGuid"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        public Tuple<bool, string> ValidatedUploadData(Guid ruleGuid, List<VipPaintPromotionTemplateModel> models)
        {
            if (ruleGuid == Guid.Empty || models == null || !models.Any())
            {
                return Tuple.Create(false, "验证优惠券相关信息不能为空");
            }
            try
            {
                var baoYangManager = new VipBaoYangPackageManager();
                var simplePromotion = baoYangManager.GetPromotionSimpleInfoByGetRuleGuid(ruleGuid);
                if (simplePromotion == null)
                {
                    return Tuple.Create(false, "优惠券有误");
                }
                var existsData = GetImportedPromotionDetail(ruleGuid, models.Select(x => x.MobileNumber));
                var lsit = (from x in models
                            join y in existsData on x.MobileNumber equals y.MobileNumber into temp
                            from z in temp.DefaultIfEmpty()
                            select new
                            {
                                x.MobileNumber,
                                x.PromotionCount,
                                GetQuantity = z?.PromotionCount ?? 0
                            }).ToList();

                //单个用户领取数量限制验证
                var sqValidatedResult = lsit.Where(x => x.PromotionCount + x.GetQuantity > simplePromotion.SingleQuantity);
                if (sqValidatedResult.Any())
                {
                    return Tuple.Create(false, $"{string.Join("\n", sqValidatedResult.Select(x => x.MobileNumber))}超过单个用户优惠券领取数量{simplePromotion.SingleQuantity}, 请先修改券领取数量再进行塞券");
                }

                //总的数量限制验证
                if (simplePromotion.Quantity != null)
                {
                    var quantity = simplePromotion.Quantity.Value;
                    var totalQuantity = models.Sum(x => x.PromotionCount);
                    if (totalQuantity > quantity - simplePromotion.GetQuantity)
                    {
                        return Tuple.Create(false, $"领取数量{totalQuantity}超过剩余取数量{quantity - simplePromotion.GetQuantity}限制(其中总数量为{quantity},已经领取数量为{simplePromotion.GetQuantity}), 请先修改券发行量再进行塞券");
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error("ValidatedUploadData", ex);
                return Tuple.Create(false, "验证优惠券异常");
            }
            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// 之前导入过相同优惠券的记录
        /// 验证优惠券领取限制
        /// </summary>
        /// <param name="ruleGuid"></param>
        /// <param name="mobileNumbers"></param>
        /// <returns></returns>
        private List<VipPaintPromotionTemplateModel> GetImportedPromotionDetail(
            Guid ruleGuid, IEnumerable<string> mobileNumbers)
        {
            var result = new List<VipPaintPromotionTemplateModel>();
            if (ruleGuid != Guid.Empty && mobileNumbers != null && mobileNumbers.Any())
            {
                for (var i = 1; i <= Math.Ceiling(mobileNumbers.Count() * 1.0 / 500); i++)
                {
                    var batchResult = dbScopeManagerConfigurationRead.Execute(conn =>
                    DalVipPaintPackage.SelectImportedPromotionDetail(conn, ruleGuid, mobileNumbers.Skip((i - 1) * 500).Take(500)));
                    result.AddRange(batchResult);
                }
            }
            return result;
        }

        #endregion

        #region 塞券记录
        /// <summary>
        /// 查询塞券记录
        /// </summary>
        /// <param name="batchCode"></param>
        /// <param name="moileNumber"></param>
        /// <param name="packagePid"></param>
        /// <param name="vipUserId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<List<VipPaintPromotionRecordViewModel>, int> SelectPromotionRecord
            (string batchCode, string moileNumber, string packagePid, Guid vipUserId, int pageIndex, int pageSize)
        {
            var result = null as List<VipPaintPromotionRecordViewModel>;
            var totalCount = 0;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute(conn => DalVipPaintPackage.SelectPromotionRecord
                  (conn, batchCode, moileNumber, packagePid, vipUserId, pageIndex, pageSize, out totalCount));
                if (result != null && result.Any())
                {
                    var vipUsers = new VipBaoYangPackageManager().GetAllBaoYangPackageUser();
                    if (vipUsers != null && vipUsers.Any())
                    {
                        result.ForEach(s =>
                        {
                            s.VipUserName = vipUsers.FirstOrDefault(v =>
                                string.Equals(v.VipUserId, s.VipUserId.ToString()))?.VipUserName;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SelectPromotionRecord", ex);
            }
            return Tuple.Create(result, totalCount);
        }

        /// <summary>
        /// 获取当前批次相关信息及对应套餐配置
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public VipPaintPackageConfigForDetail GetPromotionConfigForDetail(string batchCode)
        {
            var result = null as VipPaintPackageConfigForDetail;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute(conn =>
                      DalVipPaintPackage.GetPackageConfigByBatchCode(conn, batchCode));
                if (result != null)
                {
                    using (var client = new PromotionClient())
                    {
                        var clientResult = client.GetCouponRule(result.RuleGUID);
                        clientResult.ThrowIfException(true);
                        if (clientResult.Result != null)
                        {
                            result.RuleId = clientResult.Result.RuleID;
                            result.PromotionName = clientResult.Result.PromotionName;
                            result.Description = clientResult.Result.Description;
                        }
                    }
                    result.VipUserName = new VipBaoYangPackageManager().GetAllBaoYangPackageUser()
                        ?.FirstOrDefault(s => string.Equals(s.VipUserId, result.VipUserId.ToString()))?.VipUserName;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetPromotionConfigForDetail", ex);
            }
            return result;
        }

        /// <summary>
        /// 塞券详情
        /// </summary>
        /// <param name="batchCode"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<List<VipPaintPromotionDetailViewModel>, int> SelectPromotionDetail
            (VipPaintPackagePromotionDetail model, int pageIndex, int pageSize)
        {
            var result = null as List<VipPaintPromotionDetailViewModel>;
            int totalCount = 0;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute(conn =>
                      DalVipPaintPackage.SelectPromotionDetail(conn, model, pageIndex, pageSize, out totalCount));
                result = FillOrderForPromotionDetail(result, model.BatchCode);
            }
            catch (Exception ex)
            {
                Logger.Error("SelectPromotionDetail", ex);
            }
            return Tuple.Create(result, totalCount);
        }

        /// <summary>
        /// 填充订单信息至塞券详情记录
        /// </summary>
        /// <param name="models"></param>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        private List<VipPaintPromotionDetailViewModel> FillOrderForPromotionDetail(List<VipPaintPromotionDetailViewModel> models, string batchCode)
        {
            var package = dbScopeManagerProductcatalogRead.Execute(conn =>
                      DalVipPaintPackage.GetPackageConfigByBatchCode(conn, batchCode));
            var promotions = null as List<PromotionCodeModel>;
            var promotionIds = models?.Where(w => w.PromotionId > 0).Select(s => s.PromotionId).ToList();
            if (promotionIds != null && promotionIds.Any())
            {
                using (var client = new PromotionClient())
                {
                    promotions = client.GetPromotionCodeByIDs(new GetPromotionCodesRequest()
                    {
                        PKIDList = promotionIds
                    }).Result?.ToList();
                }
            }
            if (package != null && string.Equals(package.SettlementMethod, SettlementMethod.ByPeriod.ToString()))
            {
                if (promotions != null && promotions.Any())
                {
                    promotions.ForEach(f =>
                    {
                        var detail = models.FirstOrDefault(s => s.PromotionId == f.Pkid);
                        var status = GetPromotionStatus(f.Status);
                        if (detail != null)
                        {
                            detail.ToCOrder = f.OrderId > 0 ? Convert.ToString(f.OrderId) : string.Empty;
                            if (f.OrderId > 0)
                            {
                                using (var orderClient = new Service.Order.OrderQueryClient())
                                {
                                    var orderResult = orderClient.GetOrderRelationShip(f.OrderId);
                                    if (orderResult.Success && orderResult.Result != null && orderResult.Result.Any())
                                    {
                                        var toBOrder = orderResult.Result.FirstOrDefault().Value?.FirstOrDefault();
                                        detail.ToBOrder = string.IsNullOrEmpty(toBOrder) ? detail.ToBOrder : toBOrder;
                                    }
                                }
                            }
                            detail.Status = string.IsNullOrEmpty(status) ? detail.Status : status;
                        }
                    });
                }
            }
            else if (package != null && string.Equals(package.SettlementMethod, SettlementMethod.PreSettled.ToString()))
            {
                var record = dbScopeManagerConfigurationRead.Execute(conn =>
                  DalVipPaintPackage.GetPromotionRecord(conn, batchCode));
                if (promotions != null && promotions.Any())
                {
                    promotions.ForEach(f =>
                    {
                        var detail = models.FirstOrDefault(s => s.PromotionId == f.Pkid);
                        var status = GetPromotionStatus(f.Status);
                        if (detail != null)
                        {
                            detail.ToCOrder = f.OrderId > 0 ? Convert.ToString(f.OrderId) : string.Empty;
                            detail.Status = string.IsNullOrEmpty(status) ? detail.Status : status;
                        }
                    });
                }
                if (record != null && !string.IsNullOrWhiteSpace(record.ToBOrder))
                {
                    models?.ForEach(s => s.ToBOrder = record.ToBOrder);
                }
            }
            return models;
        }

        /// <summary>
        /// 作废券
        /// </summary>
        /// <param name="promotionIds"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Dictionary<int, bool> InvalidCodes(List<int> promotionIds, string user)
        {
            Dictionary<int, bool> result = new Dictionary<int, bool>();
            try
            {
                List<PromotionCodeModel> promotionCodes = new List<PromotionCodeModel>();
                using (var client = new PromotionClient())
                {
                    var serviceResult = client.GetPromotionCodeByIDs(new GetPromotionCodesRequest()
                    {
                        PKIDList = promotionIds
                    });
                    if (serviceResult.Success)
                    {
                        promotionCodes = serviceResult.Result.ToList();
                    }
                }
                foreach (var code in promotionCodes)
                {
                    using (var client = new PromotionClient())
                    {
                        var serviceResult = client.UpdateUserPromotionCodeStatus(new UpdateUserPromotionCodeStatusRequest()
                        {
                            Status = 3,
                            PromotionCodeId = code.Pkid,
                            OperationAuthor = user,
                            Channel = "setting.tuhu.cn",
                            UserID = Guid.Parse(code.UserId)
                        });
                        if (serviceResult.Exception != null)
                        {
                            Logger.Error(serviceResult.Exception);
                        }
                        result[code.Pkid] = serviceResult.Success && serviceResult.Result;
                    }
                }

                foreach (var item in result)
                {
                    if (item.Value)
                    {
                        var log = new
                        {
                            ObjectId = $"PromotionDetail_{item.Key}",
                            ObjectType = "VipPaintPackage",
                            BeforeValue = JsonConvert.SerializeObject(item),
                            AfterValue = "",
                            Remark = $"喷漆大客户作废券:{user}于{DateTime.Now.ToString("yyyy年MM月dd日 hh:mm")}作废券Id{item.Key}",
                            Creator = user
                        };
                        LoggerManager.InsertLog("CommonConfigLog", log);
                        dbScopeManagerConfiguration.Execute(conn =>
                        DalVipPaintPackage.UpdatePromotionDetailRemark(conn, item.Key, $"{user}于{DateTime.Now.ToString("yyyy年MM月dd日 hh:mm")}作废此券"));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("InvalidCodes", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取优惠券状态
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        private string GetPromotionStatus(int statusId)
        {
            var result = string.Empty;
            switch (statusId)
            {
                case 1: { result = Status.Used.ToString(); break; }
                case 3: { result = Status.Invalid.ToString(); break; }
                default: break;
            }
            return result;
        }

        /// <summary>
        /// 优惠券中文状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public string GetVipPaintPromotionStatus(string status)
        {
            var result = string.Empty;
            switch (status)
            {
                case "SUCCESS": { result = "成功"; break; }
                case "Fail": { result = "失败"; break; }
                case "Invalid": { result = "已作废"; break; }
                case "Used": { result = "已使用"; break; }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取大客户喷漆优惠券
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, Guid, string, string>> SelectCouponGetRules()
        {
            List<Tuple<int, Guid, string, string>> result = new List<Tuple<int, Guid, string, string>>();

            try
            {
                var xml = DalSimpleConfig.SelectConfig("VipPaintPackageConfig");
                if (!string.IsNullOrEmpty(xml))
                {
                    var configInfo = XmlHelper.Deserialize<VipPaintPackageSimpleConfig>(xml);
                    dbScopeManagerConfigurationRead.Execute(conn =>
                    {
                        result = DALVipBaoYangPackage.SelectCouponGetRules(conn, configInfo.ParentCouponId);
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetVipPaintPackageCouponRules", ex);
            }
            return result ?? new List<Tuple<int, Guid, string, string>>();
        }

        /// <summary>
        /// 该批次塞券不成功的记录
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public int? GetNotSuccessPromotionDtailCount(string batchCode)
        {
            var result = null as int?;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute(conn =>
                DalVipPaintPackage.GetNotSuccessPromotionDtailCount(conn, batchCode));
            }
            catch (Exception ex)
            {
                Logger.Error("GetNotSuccessPromotionDtailCount", ex);
            }
            return result;
        }

        /// <summary>
        /// 更新塞券详情记录
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="mobileNumber"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdatePromotionDetail(long pkid, string mobileNumber, string user)
        {
            var result = false;
            try
            {
                var oldValue = dbScopeManagerConfigurationRead.Execute(conn => DalVipPaintPackage.GetPromotionDetail(conn, pkid));
                if (oldValue != null)
                {
                    var remark = $"{user}于{DateTime.Now.ToString("yyyy年MM月dd日 hh:mm")}更新";
                    result = dbScopeManagerConfiguration.Execute(conn => DalVipPaintPackage.UpdatePromotionDetail(conn, pkid, mobileNumber, remark));
                    if (result)
                    {
                        var model = JsonConvert.DeserializeObject<VipPaintPackagePromotionDetail>(JsonConvert.SerializeObject(oldValue));
                        model.MobileNumber = mobileNumber;
                        model.Status = Status.WAIT.ToString();
                        model.Remarks = remark;
                        model.LastUpdateDateTime = DateTime.Now;
                        var log = new
                        {
                            ObjectId = $"PromotionDetail_{pkid}",
                            ObjectType = "VipPaintPackage",
                            BeforeValue = JsonConvert.SerializeObject(oldValue),
                            AfterValue = JsonConvert.SerializeObject(model),
                            Remark = $"喷漆大客户塞券详情:{oldValue.BatchCode}批次中更新PKID为:{oldValue.PKID}的手机号为{mobileNumber}",
                            Creator = user
                        };
                        LoggerManager.InsertLog("CommonConfigLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UpdatePromotionDetail", ex);
            }
            return result;
        }

        /// <summary>
        /// 删除塞券详情记录
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeletePromotionDetail(long pkid, string user)
        {
            var result = false;
            try
            {
                var oldValue = dbScopeManagerConfigurationRead.Execute(conn => DalVipPaintPackage.GetPromotionDetail(conn, pkid));
                if (oldValue != null)
                {
                    result = dbScopeManagerConfiguration.Execute(conn => DalVipPaintPackage.DeletePromotionDetail(conn, pkid));
                    if (result)
                    {
                        var log = new
                        {
                            ObjectId = $"PromotionDetail_{pkid}",
                            ObjectType = "VipPaintPackage",
                            BeforeValue = JsonConvert.SerializeObject(oldValue),
                            AfterValue = "",
                            Remark = $"喷漆大客户塞券详情:{oldValue.BatchCode}批次中删除PKID为:{oldValue.PKID}的记录",
                            Creator = user
                        };
                        LoggerManager.InsertLog("CommonConfigLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("DeletePromotionDetail", ex);
            }
            return result;
        }

        #region 短信配置

        /// <summary>
        /// 查看短信配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<List<VipPaintPackageSmsConfig>, int> SelectPackageSmsConfig
            (VipPaintPackageSmsConfig model, int pageIndex, int pageSize)
        {
            var result = null as List<VipPaintPackageSmsConfig>;
            var totalCount = 0;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute(conn =>
                 DalVipPaintPackage.SelectPackageSmsConfig(conn, model, pageIndex, pageSize, out totalCount));
                var vipUsers = new VipBaoYangPackageManager().GetAllBaoYangPackageUser();
                if (result != null && result.Any() && vipUsers != null && vipUsers.Any())
                {
                    result.ForEach(s =>
                    {
                        s.VipUserName = vipUsers.FirstOrDefault(v => string.Equals(v.VipUserId, s.VipUserId.ToString()))?.VipUserName;
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SelectVipPaintPackageSmsConfig", ex);
            }
            return Tuple.Create(result, totalCount);
        }

        /// <summary>
        /// 添加或更新短信配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateOrInsertPackageSmsPackage(VipPaintPackageSmsConfig model)
        {
            var result = false;
            try
            {
                var oldValue = dbScopeManagerConfigurationRead.Execute(conn => DalVipPaintPackage.GetPackageSendSmsConfig(conn, model.PackageId));
                if (oldValue != null)
                {
                    result = dbScopeManagerConfiguration.Execute(conn => DalVipPaintPackage.UpdatePackageSendSms(conn, model));
                    if (result)
                    {
                        model.PKID = oldValue.PKID;
                        model.CreateDateTime = oldValue.CreateDateTime;
                        model.LastUpdateDateTime = DateTime.Now;
                        var log = new
                        {
                            ObjectId = $"SmsConfig_{model.PackageName}",
                            ObjectType = "VipPaintPackage",
                            BeforeValue = JsonConvert.SerializeObject(oldValue),
                            AfterValue = JsonConvert.SerializeObject(model),
                            Remark = $"喷漆大客户套餐:{model.PackageName}的短信配置由{oldValue.IsSendSms}改为{model.IsSendSms}",
                            Creator = model.Operator
                        };
                        LoggerManager.InsertLog("CommonConfigLog", log);
                    }
                }
                else
                {
                    var pkid = dbScopeManagerConfiguration.Execute(conn => DalVipPaintPackage.InsertPackageSmsConfig(conn, model));
                    result = pkid > 0;
                    if (result)
                    {
                        model.PKID = pkid;
                        model.CreateDateTime = DateTime.Now;
                        model.LastUpdateDateTime = DateTime.Now;
                        var log = new
                        {
                            ObjectId = $"SmsConfig_{model.PackageName}",
                            ObjectType = "VipPaintPackage",
                            BeforeValue = "",
                            AfterValue = JsonConvert.SerializeObject(model),
                            Remark = $"喷漆大客户套餐:{model.PackageName}的短信配置设置为{model.IsSendSms}",
                            Creator = model.Operator
                        };
                        LoggerManager.InsertLog("CommonConfigLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateOrInsertPackageSmsPackage", ex);
            }
            return result;
        }

        /// <summary>
        /// 套餐是否发送短信
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public bool IsPackageSms(int packageId)
        {
            var result = false;//默认不发送短信
            try
            {
                var config = dbScopeManagerProductcatalogRead.Execute(conn =>
                DalVipPaintPackage.GetPackageSendSmsConfig(conn, packageId));
                if (config != null)
                {
                    result = config.IsSendSms;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("IsPackageSms", ex);
            }
            return result;
        }
        #endregion
    }
}
