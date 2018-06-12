using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Nosql;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.DAO.VipBaoYangPackage;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.VipBaoYangPackage;
using Tuhu.Service.GroupBuying;
using Tuhu.Service.GroupBuying.Models.RedemptionCode;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.Service.Member.Request;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.Service.Order.Models;
using Tuhu.Service.Order.Request;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Product.Models;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Enums;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.Provisioning.Business.VipBaoYangPackage
{
    public class VipBaoYangPackageManager
    {
        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager TuhuLogConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;
        private readonly IDBScopeManager dbTuhuLogScopeReadManager = null;

        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(VipBaoYangPackageManager));
        private static string LogType = "VipBaoYangPackage";
        //private static String VIOLATION_URL = $"{ConfigurationManager.AppSettings["CheXinYiHost"]}gateway.aspx";

        public VipBaoYangPackageManager()
        {
            this.dbScopeManager = new DBScopeManager(ConnectionManager);
            this.dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
            this.dbTuhuLogScopeReadManager = new DBScopeManager(TuhuLogConnectionManager);
        }

        public List<VipBaoYangPackageModel> SelectVipBaoYangPackage(string pid, Guid vipUserId, int pageIndex, int pageSize)
        {
            List<VipBaoYangPackageModel> result = new List<VipBaoYangPackageModel>();

            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    result = DALVipBaoYangPackage.SelectVipBaoYangPackage(conn, pid, vipUserId, pageIndex, pageSize);
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public bool IsExistsPackageName(string packageName, int pkid)
        {
            var result = false;

            try
            {
                result = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.IsExistsPackageName(conn, packageName, pkid)) > 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public List<VipBaoYangPackageModel> GetBaoYangPackageNameByVipUserId(Guid vipUserId)
        {
            List<VipBaoYangPackageModel> result = new List<VipBaoYangPackageModel>();

            try
            {
                result = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.GetBaoYangPackageNameByVipUserId(conn, vipUserId));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        /// <summary>
        /// 根据主键Id获取套餐详情
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public VipBaoYangPackageViewModel SelectVipBaoYangPackageByPkid(int pkid)
        {
            VipBaoYangPackageViewModel result = null;
            try
            {
                var package = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectVipBaoYangPackageByPkid(conn, pkid));
                result = new VipBaoYangPackageViewModel(package);
                var user = BaoYangExternalService.GetCompanyUserInfo(result.VipUserId);
                result.VipUserName = user?.UserName;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 添加保养套餐
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public bool InsertVipBaoYangPackage(VipBaoYangPackageViewModel package)
        {
            var result = false;
            var productId = string.Empty;
            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    var pkid = DALVipBaoYangPackage.InsertVipBaoYangPackage(conn, new VipBaoYangPackageDbModel(package));
                    if (pkid > 0)
                    {
                        productId = $"BY-TUHU-BXGSDCBY|{pkid.ToString()}";
                        result = DALVipBaoYangPackage.UpdateVipBaoYangPackagePID(conn, pkid, productId) > 0;
                    }
                    if (result)
                    {
                        package.PKID = pkid;
                        package.PID = productId;
                        VipBaoYangPackageService.CreateProduct(pkid.ToString(), package.PackageName, package.CreateUser);
                    }
                });

                if (result)
                {
                    InsertLog(productId, null, JsonConvert.SerializeObject(package), "Insert", package.CreateUser);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public bool UpdateVipBaoYangPackage(VipBaoYangPackageViewModel package)
        {
            var success = false;
            try
            {
                var inputPackage = new VipBaoYangPackageDbModel(package);
                var dbPackage = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectVipBaoYangPackageByPkid(conn, package.PKID));
                if (dbPackage != null)
                {
                    var oldData = JsonConvert.SerializeObject(dbPackage);
                    dbPackage.Brands = inputPackage.Brands;

                    success = dbScopeManager.Execute(conn => DALVipBaoYangPackage.UpdateVipBaoYangPackage(conn, dbPackage));
                    if (success)
                    {
                        dbPackage.LastUpdateDateTime = DateTime.Now;
                        var newData = JsonConvert.SerializeObject(dbPackage);
                        InsertLog(dbPackage.PID, oldData, newData, "Update", package.CreateUser);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return success;
        }

        public List<PromotionOperationRecord> SelectPromotionOperationRecord(string pid, Guid vipUserId, int packageId, string batchCode, string mobilePhone, int pageIndex, int pageSize)
        {
            List<PromotionOperationRecord> result = new List<PromotionOperationRecord>();

            try
            {
                result = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectPromotionOperationRecord(conn, pid, vipUserId, packageId, batchCode, mobilePhone, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public bool BatchBaoYangPakckagePromotion(int packageId, Guid ruleGUID, string sha1Value, string batchCode, string operatorUser)
        {
            var result = false;
            try
            {
                dbScopeManager.Execute(conn =>
                {
                    var isSendSms = DALVipBaoYangPackage.IsSendSmsByPackageId(conn, packageId);
                    result = DALVipBaoYangPackage.InsertBaoYangPackagePromotionRecord(conn, packageId, batchCode, ruleGUID, isSendSms, operatorUser) > 0;
                    SetUploadedFileSah1Cache(sha1Value);
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public List<BaoYangPackagePromotionDetail> SelectPromotionDetailsByBatchCode(string batchCode)
        {
            List<BaoYangPackagePromotionDetail> result = new List<BaoYangPackagePromotionDetail>();
            try
            {
                result = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectPromotionDetailsByBatchCode(conn, batchCode));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        private bool SetUploadedFileSah1Cache(string sha1)
        {
            if (!string.IsNullOrEmpty(sha1))
            {
                var key = sha1;
                using (var client = CacheHelper.CreateCacheClient("VipBaoYangPackageClientName"))
                {
                    var cacheResult = client.Set(key, 1, TimeSpan.FromDays(20));
                    return cacheResult.Success;
                }
            }
            return true;
        }

        public bool IsUploaded(string sha1)
        {
            try
            {
                if (!string.IsNullOrEmpty(sha1))
                {
                    var key = sha1;
                    using (var client = CacheHelper.CreateCacheClient("VipBaoYangPackageClientName"))
                    {
                        var cacheResult = client.Get(key);
                        if (!cacheResult.Success && cacheResult.Exception != null)
                        {
                            throw cacheResult.Exception;
                        }

                        if (string.Equals(cacheResult.Message, "Key不存在", StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return true;
        }

        public List<VipSimpleUser> GetBaoYangPackageConfigUser()
        {
            List<VipSimpleUser> result = new List<VipSimpleUser>();

            try
            {
                result = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.GetBaoYangPackageConfigUser(conn));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        /// <summary>
        /// 获取配置了套餐的喷漆大客户
        /// </summary>
        /// <returns></returns>
        public List<VipSimpleUser> GetAllBaoYangPackageUser()
        {
            List<VipSimpleUser> result = new List<VipSimpleUser>();

            try
            {
                result = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.GetAllBaoYangPackageUser(conn));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public List<VipBaoYangPackageSmsConfig> SelectVipSmsConfig(Guid vipUserId, int pageIndex, int pageSize)
        {
            List<VipBaoYangPackageSmsConfig> result = new List<VipBaoYangPackageSmsConfig>();

            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    result = DALVipBaoYangPackage.SelectVipSmsConfig(conn, vipUserId, pageIndex, pageSize);
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public bool UpdateSendSmsStatus(Guid vipUserId, bool isSendSms, string user)
        {
            var result = false;

            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    var data = DALVipBaoYangPackage.IsExistsSmsConfig(conn, vipUserId);
                    if (data > 0)
                    {
                        result = DALVipBaoYangPackage.UpdateSendSmsStatus(conn, vipUserId, isSendSms) > 0;
                    }
                    else
                    {
                        result = DALVipBaoYangPackage.InsertVipSmsCofig(conn, vipUserId, isSendSms, user) > 0;
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public Tuple<bool, string> ValidatedUploadData(Guid ruleGuid, List<BaoYangPackagePromotionDetail> data)
        {
            if (ruleGuid == Guid.Empty || data == null || !data.Any())
            {
                return Tuple.Create(false, "不能为空");
            }
            try
            {
                var simplePromotion = GetPromotionSimpleInfoByGetRuleGuid(ruleGuid);
                if (simplePromotion == null)
                {
                    return Tuple.Create(false, "优惠券有误");
                }
                var existsData = GetImportedPromotionDetail(ruleGuid, data.Select(x => x.MobileNumber));
                var lsit = (from x in data
                            join y in existsData on x.MobileNumber equals y.MobileNumber into temp
                            from z in temp.DefaultIfEmpty()
                            select new
                            {
                                x.MobileNumber,
                                x.Quantity,
                                GetQuantity = z?.Quantity ?? 0
                            }).ToList();

                //单个用户领取数量限制验证
                var sqValidatedResult = lsit.Where(x => x.Quantity + x.GetQuantity > simplePromotion.SingleQuantity);
                if (sqValidatedResult.Any())
                {
                    return Tuple.Create(false, $"{string.Join("\n", sqValidatedResult.Select(x => x.MobileNumber))}超过单个用户优惠券领取数量{simplePromotion.SingleQuantity}, 请先修改券领取数量再进行塞券");
                }

                //总的数量限制验证
                if (simplePromotion.Quantity != null)
                {
                    var quantity = simplePromotion.Quantity.Value;
                    var totalQuantity = data.Sum(x => x.Quantity);
                    if (totalQuantity > quantity - simplePromotion.GetQuantity)
                    {
                        return Tuple.Create(false, $"领取数量{totalQuantity}超过剩余取数量{quantity - simplePromotion.GetQuantity}限制(其中总数量为{quantity},已经领取数量为{simplePromotion.GetQuantity}), 请先修改券发行量再进行塞券");
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Tuple.Create(false, "发生异常");
            }

            return Tuple.Create(true, string.Empty);
        }

        private List<BaoYangPackagePromotionDetail> GetImportedPromotionDetail(
            Guid ruleGuid, IEnumerable<string> mobileNumbers)
        {
            var result = new List<BaoYangPackagePromotionDetail>();
            if (ruleGuid != Guid.Empty && mobileNumbers != null && mobileNumbers.Any())
            {
                for (var i = 1; i <= Math.Ceiling(mobileNumbers.Count() * 1.0 / 500); i++)
                {
                    var batchResult = dbScopeReadManager
                        .Execute(conn => DALVipBaoYangPackage.SelectImportedPromotionDetail(conn, ruleGuid, mobileNumbers.Skip((i - 1) * 500).Take(500)));
                    result.AddRange(batchResult);
                }
            }
            return result;
        }

        public BaoYangPromotionSimpleInfo GetPromotionSimpleInfoByGetRuleGuid(Guid getRuleGuid)
            => dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectPromotionSimpleInfoByGetRuleGuid(conn, getRuleGuid));

        #region PromotionDetail

        public BaoYangPackageConfigSimpleInfo GetPackageConfigSimpleInfo(string batchCode)
        {
            var resulut = null as BaoYangPackageConfigSimpleInfo;
            if (!string.IsNullOrEmpty(batchCode))
            {
                try
                {
                    resulut = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectPackageConfigSimpleInfo(conn, batchCode));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message, ex);
                }
            }
            return resulut;
        }

        public List<BaoYangPackagePromotionDetailSimpleModel> GetPromotionDetailsByBatchCode(string batchCode, int index, int size)
        {
            var result = null as List<BaoYangPackagePromotionDetailSimpleModel>;
            if (!string.IsNullOrEmpty(batchCode))
            {
                try
                {
                    result = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectPromotionDetailsByBatchCode(conn,
                        batchCode, index, size));
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message, ex);
                }
            }

            if (result != null && result.Any())
            {
                GetPromotionCodesRequest request = new GetPromotionCodesRequest()
                {
                    PKIDList = result.Where(o => o.PromotionId.HasValue && o.PromotionId.Value > 0).Select(o => (int)o.PromotionId).ToList()
                };
                using (var client = new PromotionClient())
                {
                    var serviceResult = client.GetPromotionCodeByIDs(request);
                    if (serviceResult.Success && serviceResult.Result != null)
                    {
                        foreach (var detail in result)
                        {
                            if (detail.PromotionId > 0)
                            {
                                var code = serviceResult.Result.FirstOrDefault(o => o.Pkid == detail.PromotionId);
                                detail.PromotionStatus = code != null ? code.Status : 2;
                                if (!string.IsNullOrEmpty(code.UsedTime))
                                {
                                    detail.PromotionUsedTime = DateTime.Parse(code.UsedTime);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public bool UpdatePromotionDetail(long pkid, string mobileNumber, string user)
        {
            var success = false;
            try
            {
                var detail = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectPromotionDetailById(conn, pkid));
                if (detail != null && detail.MobileNumber != mobileNumber)
                {
                    success = dbScopeManager.Execute(conn => DALVipBaoYangPackage.UpdatePromotionDetail(conn, pkid, mobileNumber));
                }

                if (success)
                {
                    InsertPromotionDetailLog(pkid.ToString(), detail.MobileNumber, mobileNumber, "Update", user);
                }
            }
            catch (Exception ex)
            {
                logger.Error(nameof(UpdatePromotionDetail), ex);
            }
            return success;
        }

        public bool IsAllUnUsed(List<int> codes)
        {
            bool result = false;
            GetPromotionCodesRequest request = new GetPromotionCodesRequest()
            {
                PKIDList = codes
            };
            using (var client = new PromotionClient())
            {
                var serviceResult = client.GetPromotionCodeByIDs(request);
                if (serviceResult.Success && serviceResult.Result != null)
                {
                    result = serviceResult.Result.All(o => o.Status == 0);
                }
            }

            return result;
        }

        public Dictionary<int, bool> InvalidCodes(List<int> codes, string user)
        {
            Dictionary<int, bool> result = new Dictionary<int, bool>();
            List<PromotionCodeModel> promotionCodes = new List<PromotionCodeModel>();
            using(var client = new PromotionClient())
            {
                var serviceResult = client.GetPromotionCodeByIDs(new GetPromotionCodesRequest()
                {
                    PKIDList = codes
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
                    if(serviceResult.Exception != null)
                    {
                        logger.Error(serviceResult.Exception);
                    }
                    result[code.Pkid] = serviceResult.Success && serviceResult.Result;
                }
            }

            foreach(var item in result)
            {
                if (item.Value)
                {
                    dbScopeManager.Execute(conn => DALVipBaoYangPackage.UpdatePromotionRemarks(conn, item.Key, $"{user}于{DateTime.Now.ToString("yyyy年MM月dd日 hh:mm")}作废此券"));
                }                
            }

            return result;
        }

       

        #endregion

        #region RedemptionCode

        /// <summary>
        /// 获取单次保养套餐Id和Name
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetVipBaoYangConfigSimpleInfo()
        {
            var dict = null as Dictionary<int, string>;
            try
            {
                dict = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectVipBaoYangConfigSimpleInfo(conn));
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return dict ?? new Dictionary<int, string>();
        }

        /// <summary>
        /// 新增礼包配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool AddVipBaoYangGiftPackConfig(VipBaoYangGiftPackConfig config, string user)
        {
            bool success = false;
            try
            {
                var pkid = dbScopeManager.Execute(conn => DALVipBaoYangPackage.AddVipBaoYangGiftPackConfig(conn, config));
                success = pkid > 0;
                config.PKID = pkid;
                if (success)
                {
                    VipBaoYangPackageService.InsertBaoYangOprLog(new
                    {
                        LogType = "VipBaoYangPackageRedemptionCodeGiftPack",
                        IdentityID = pkid.ToString(),
                        OldValue = string.Empty,
                        NewValue = JsonConvert.SerializeObject(new { config.PackId, config.PackageId, config.IsValid, config.PackName }),
                        Remarks = $"Add",
                        OperateUser = user,
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return success;
        }

        /// <summary>
        /// 修改礼包配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool EditVipBaoYangGiftPackConfig(VipBaoYangGiftPackConfig config)
        {
            bool success = false;
            try
            {
                success = dbScopeManager.Execute(conn => DALVipBaoYangPackage.EditVipBaoYangGiftPackConfig(conn, config));
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return success;
        }

        /// <summary>
        /// 是否存在礼包名称
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool IsExisitsVipBaoYangGiftPackName(VipBaoYangGiftPackConfig config)
        {
            bool exists = false;
            try
            {
                exists = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.IsExisitsVipBaoYangGiftPackName(conn, config));
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return exists;
        }

        /// <summary>
        /// 分页获取配置
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public VipBaoYangPagerModel<VipBaoYangGiftPackConfig> GetVipBaoYangGiftPackConfig(int index, int size)
        {
            var result = null as VipBaoYangPagerModel<VipBaoYangGiftPackConfig>;
            try
            {
                result = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectVipBaoYangGiftPackConfig(conn, index, size));
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return result ?? new VipBaoYangPagerModel<VipBaoYangGiftPackConfig> { Total = 0, Data = new List<VipBaoYangGiftPackConfig>() };
        }

        /// <summary>
        /// 根据礼包Id获取优惠券配置
        /// </summary>
        /// <param name="packId"></param>
        /// <returns></returns>
        public List<GiftPackCouponConfig> GetGiftPackCouponConfig(long packId)
        {
            var result = null as List<GiftPackCouponConfig>;
            try
            {
                var configs = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectGiftPackCouponConfig(conn, packId)).ToList();
                var extraInfo = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectCouponInfos(conn, configs.Select(x => x.GetRuleGUID)));
                result = (from x in configs
                          join y in extraInfo on x.GetRuleGUID equals y.GetRuleGUID into temp
                          from z in temp.DefaultIfEmpty()
                          select new GiftPackCouponConfig
                          {
                              PKID = x.PKID,
                              GetRuleID = z?.GetRuleID ?? 0,
                              GetRuleGUID = x.GetRuleGUID,
                              PackId = x.PackId,
                              Quantity = x.Quantity,
                              Name = z?.Name,
                              Description = z?.Description,
                              PromtionName = z?.PromtionName,
                              RuleID = z?.RuleID ?? 0,
                              Term = z?.Term,
                              ValiEndDate = z?.ValiEndDate,
                              ValiStartDate = z?.ValiStartDate,
                          }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return result ?? new List<GiftPackCouponConfig>();
        }

        /// <summary>
        /// 添加礼包优惠券配置
        /// </summary>
        /// <param name="packId"></param>
        /// <param name="getRuleId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public bool AddGiftPackCouponConfig(long packId, Guid getRuleGUID, int quantity, string user)
        {
            bool succes = false;
            try
            {
                var pkid = dbScopeManager.Execute(conn => DALVipBaoYangPackage.AddGiftPackCouponConfig(conn, packId, getRuleGUID, quantity));
                succes = pkid > 0;
                if (succes)
                {
                    VipBaoYangPackageService.InsertBaoYangOprLog(new
                    {
                        LogType = "VipBaoYangPackageRedemptionCodeCoupon",
                        IdentityID = pkid.ToString(),
                        OldValue = string.Empty,
                        NewValue = JsonConvert.SerializeObject(new { PKID = pkid, PackId = packId, GetRuleGUID = getRuleGUID, Quantity = quantity }),
                        Remarks = $"Add",
                        OperateUser = user,
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return succes;
        }

        /// <summary>
        /// 验证礼包优惠券配置
        /// </summary>
        /// <param name="packId"></param>
        /// <param name="getRuleId"></param>
        /// <returns></returns>
        public Tuple<bool, string> ValidateGiftPackCouponConfig(long packId, Guid getRuleGUID)
        {
            Tuple<bool, string> result = null;
            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    var getRuleValidateResult = DALVipBaoYangPackage.SelectCouponInfos(conn, new[] { getRuleGUID }).FirstOrDefault() != null;
                    if (!getRuleValidateResult)
                    {
                        result = Tuple.Create(false, "优惠券规则不存在");
                        return;
                    }
                    var packValidateResult = DALVipBaoYangPackage.SelectVipBaoYangGiftPackConfigById(conn, packId) != null;

                    if (!packValidateResult)
                    {
                        result = Tuple.Create(false, "礼包不存在");
                        return;
                    }
                    var exist = DALVipBaoYangPackage.IsExistsGiftPackCouponConfig(conn, packId, getRuleGUID);
                    if (exist)
                    {
                        result = Tuple.Create(false, "已存在相同数据");
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                result = Tuple.Create(false, ex.Message);
            }
            return result ?? Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// 生成兑换码
        /// </summary>
        /// <param name="packId"></param>
        /// <param name="quantity"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool GenerateRedemptionCode(long packId, int quantity, DateTime startTime, DateTime endTime, string user)
        {
            bool success = false;
            try
            {
                var result = VipBaoYangPackageService.GenerateRedemptionCode(quantity, startTime, endTime, user);
                if (result.Success && result.Codes != null && result.Codes.Any())
                {
                    var codes = result.Codes.Select(x => new VipBaoYangRedemptionCode
                    {
                        BatchCode = result.BatchCode,
                        CreateUser = user,
                        EndTime = endTime,
                        StartTime = startTime,
                        PackId = packId,
                        RedemptionCode = x,
                        Status = "0New",
                    }).ToList();
                    success = dbScopeManager.Execute(conn => DALVipBaoYangPackage.InsertBaoYangRedemptionCode(conn, codes));
                    if (success)
                    {
                        var packageConfig = GetVipBaoYangPackageByPackId(packId);
                        var package = packageConfig.Item2;
                        var totalCount = packageConfig.Item1 * codes.Count;
                        if(package.SettlementMethod == SettlementMethod.PreSettled.ToString())
                        {
                            Create2BOrder(package.Price, package.PID, package.PackageName, package.VipUserId, totalCount, result.BatchCode);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return success;
        }

        public List<VipBaoYangRedemptionCodeSimpleModel> GetRedemptionCodeSimpleInfo(long packId)
        {
            List<VipBaoYangRedemptionCodeSimpleModel> result = null;
            try
            {
                result = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectRedemptionCodeSimpleInfo(conn, packId));
            }
            catch (Exception ex)
            {
                logger.Error("GetRedemptionCodeSimpleInfo", ex);
            }
            return result ?? new List<VipBaoYangRedemptionCodeSimpleModel>();
        }

        /// <summary>
        /// 查询礼包对应的兑换码简要信息
        /// </summary>
        /// <param name="packId"></param>
        /// <returns></returns>
        public List<VipBaoYangRedemptionCodeSimpleModel> GetRedemptionCodeSimpleAndRecord(long packId)
        {
            var result = null as List<VipBaoYangRedemptionCodeSimpleModel>;
            try
            {
                result = GetRedemptionCodeSimpleInfo(packId);
                var records = GetExportRedemptionCodeRecordDetails(result.Select(x => x.BatchCode));
                result.ForEach(x => x.Records = records.Where(t => t.IdentityID == x.BatchCode).ToList());
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return result;
        }

        /// <summary>
        /// 获取兑换码
        /// </summary>
        /// <param name="batchCode"></param>
        /// <param name="packId"></param>
        /// <returns></returns>
        public List<VipBaoYangRedemptionCode> GetRedemptionCodeDetails(string batchCode, long packId, string user)
        {
            List<VipBaoYangRedemptionCode> result = null;
            try
            {
                result = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectRedemptionCodeDetails(conn, batchCode, packId));
                if (result.Any())
                {
                    InsertDownloadRedemptionCodeRecord(batchCode, user);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return result ?? new List<VipBaoYangRedemptionCode>();
        }

        /// <summary>
        /// 根据礼包Id获取套餐配置
        /// </summary>
        /// <param name="packId"></param>
        /// <returns></returns>
        public Tuple<int, VipBaoYangPackageModel> GetVipBaoYangPackageByPackId(long packId)
        {
            var result = null as Tuple<int, VipBaoYangPackageModel>;
            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    var package = DALVipBaoYangPackage.SelectVipBaoYangPackageByPackId(conn, packId);
                    var coupons = DALVipBaoYangPackage.SelectGiftPackCouponConfig(conn, packId);
                    result = Tuple.Create(coupons.Sum(x => x.Quantity), package);
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return result;
        }

        private bool Create2BOrder(decimal price, string pid, string name, Guid vipUserId,
            int count, string batchCode)
        {
            var success = false;
            try
            {
                var result = VipBaoYangPackageService.CreateOrder(price, pid, name, vipUserId, count, batchCode);
                if (result != null && result.OrderId > 0)
                {
                    VipBaoYangPackageService.ExecuteOrderProcess(new ExecuteOrderProcessRequest()
                    {
                        OrderId = result.OrderId,
                        OrderProcessEnum = OrderProcessEnum.GeneralCompleteToHome,
                        CreateBy = vipUserId.ToString()
                    });
                    logger.Info($"创建买断制2B订单{result.OrderId}成功, 一共{count}个产品数量, 关联批次号:{batchCode}");
                    success = dbScopeManager.Execute(conn => DALVipBaoYangPackage.UpdateRedemptionCode(conn, batchCode, result.OrderId));
                }
                else
                {
                    logger.Error($"创建买断制2B订单失败, 一共{count}个产品数量, 批次号:{batchCode}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return success;
        }

        #endregion

        #region Promotion

        /// <summary>
        /// 获取指定规则的获取优惠券规则
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, Guid, string, string>> GetCouponRules()
        {
            List<Tuple<int, Guid, string, string>> result = new List<Tuple<int, Guid, string, string>>();

            try
            {
                var xml = DalSimpleConfig.SelectConfig("VipBaoYangPackage");
                if (!string.IsNullOrEmpty(xml))
                {
                    var configInfo = XmlHelper.Deserialize<VipBaoYangPackageParentCouponConfig>(xml);
                    dbScopeReadManager.Execute(conn =>
                    {
                        result = DALVipBaoYangPackage.SelectCouponGetRules(conn, configInfo.ParentCouponId);
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result ?? new List<Tuple<int, Guid, string, string>>();
        }

        /// <summary>
        /// 根据RuleId获取'获取优惠券规则'
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public List<Tuple<int, Guid, string, string>> GetCouponGetRules(int ruleId)
        {
            List<Tuple<int, Guid, string, string>> result = null;
            try
            {
                result = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectCouponGetRules(conn, ruleId));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result ?? new List<Tuple<int, Guid, string, string>>();
        }

        /// <summary>
        /// 获取所有优惠券规则
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetAllCouponRules()
        {
            Dictionary<int, string> result = null;
            try
            {
                result = dbScopeReadManager.Execute(conn => DALVipBaoYangPackage.SelectCouponRules(conn));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result ?? new Dictionary<int, string>();
        }

        #endregion

        #region BaoYangOprLog

        public static void InsertLog(string identityId, string oldValue, string newValue, string remarks, string user)
        {
            try
            {
                var data = new
                {
                    LogType = "VipBaoYangPackage",
                    IdentityID = identityId,
                    OldValue = oldValue,
                    NewValue = newValue,
                    Remarks = remarks,
                    OperateUser = user,
                };
                LoggerManager.InsertLog("BYOprLog", data);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public static void InsertPromotionDetailLog(string identityId, string oldValue, string newValue, string remarks, string user)
        {
            try
            {
                var data = new
                {
                    LogType = "VipBaoYangPackagePromotionDetail",
                    IdentityID = identityId,
                    OldValue = oldValue,
                    NewValue = newValue,
                    Remarks = remarks,
                    OperateUser = user,
                };
                LoggerManager.InsertLog("BYOprLog", data);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// 记录导出的日志
        /// </summary>
        /// <param name="batchCode"></param>
        /// <param name="user"></param>
        public static void InsertDownloadRedemptionCodeRecord(string batchCode, string user)
        {
            try
            {
                VipBaoYangPackageService.InsertBaoYangOprLog(new
                {
                    LogType = "VipBaoYangPackageExportRedemptionCode",
                    IdentityID = batchCode,
                    OldValue = string.Empty,
                    NewValue = string.Empty,
                    Remarks = $"{user}下载了批次{batchCode}的兑换码",
                    OperateUser = user,
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private List<BaoYangOprLog> GetExportRedemptionCodeRecordDetails(IEnumerable<string> batchCodes)
            => dbTuhuLogScopeReadManager.Execute(conn =>
                   DALVipBaoYangPackage.SelectExportRedemptionCodeRecordDetails(conn, batchCodes, "VipBaoYangPackageExportRedemptionCode"));


        #endregion
    }
}
