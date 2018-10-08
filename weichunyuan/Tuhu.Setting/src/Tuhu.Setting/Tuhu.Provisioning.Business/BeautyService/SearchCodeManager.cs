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
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO.BeautyServicePackageDal;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;
using Tuhu.Provisioning.DataAccess.Entity.CommonEnum;
using Tuhu.Service.KuaiXiu;
using Tuhu.Service.KuaiXiu.Models;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.Provisioning.Business.BeautyService
{
    public class SearchCodeManager
    {
        private static readonly string CacheClientName = "settting";

        private static readonly Common.Logging.ILog Logger = LogManager.GetLogger(typeof(SearchCodeManager));
        private static string writeStrConfig = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
        private static string writeConnStr = SecurityHelp.IsBase64Formatted(writeStrConfig) ? SecurityHelp.DecryptAES(writeStrConfig) : writeStrConfig;
        /// <summary>
        /// 根据手机号查询服务码详情
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static async Task<Tuple<List<ServiceCodeDetail>, int>> GetServiceCodeDetailByMobile(string mobile, int pageIndex, int pageSize)
        {
            var result = new List<ServiceCodeDetail>();
            var totalCount = 0;
            if (!string.IsNullOrEmpty(mobile))
            {
                Guid userId = Guid.Empty;
                using (var client = new UserAccountClient())
                {
                    var accountResult = await client.GetUserByMobileAsync(mobile);
                    if (accountResult != null && accountResult.Result != null)
                    {
                        userId = accountResult.Result.UserId;
                    }
                }
                if (userId != Guid.Empty)
                {
                    var packageDetailResult = GetBeautyServicePackageDetailCodesByUserId(userId, pageIndex, pageSize);
                    if (packageDetailResult != null)
                    {
                        var packageDetailCodes = packageDetailResult.Item1;
                        totalCount = packageDetailResult.Item2;
                        if (packageDetailCodes != null && packageDetailCodes.Any())
                        {
                            var codeDetail = await GetServiceCodeDetail(packageDetailCodes.Select(t => t.ServiceCode));
                            if (codeDetail != null && codeDetail.Any())
                            {
                                foreach (var item in codeDetail)
                                {
                                    item.Mobile = mobile;
                                    result.Add(item);
                                }
                            }
                        }
                        var bankRecords = BankMRManager.GetBankMRActivityCodeRecordByUserId(userId);
                        var bankServiceCodeDetails = await BankMRManager.SearchBankMRActivityCodeDetailByRecords(bankRecords);
                        if (bankServiceCodeDetails != null && bankServiceCodeDetails.Any())
                        {
                            result.AddRange(bankServiceCodeDetails);
                        }
                    }

                }
            }
            return Tuple.Create(result, totalCount);
        }
        /// <summary>
        /// 根据服务码查询服务码详情，有用户信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static async Task<Tuple<List<ServiceCodeDetail>, int>> GetServiceCodeDetailByCode(string code)
        {
            var result = new List<ServiceCodeDetail>();
            var totalCount = 0;
            if (!string.IsNullOrEmpty(code))
            {
                var packageCodeDetails = GetBeautyServicePackageDetailCodesByCodes(new List<string>() { code });
                var codeDetail = packageCodeDetails?.FirstOrDefault();
                totalCount = codeDetail != null ? 1 : 0;
                if (codeDetail != null)
                {
                    var codeDetails = await GetServiceCodeDetail(new List<string>() { code });
                    if (codeDetails != null && codeDetails.Any())
                    {
                        User user = null;
                        using (var client = new UserAccountClient())
                        {
                            var accountResult = await client.GetUserByIdAsync(codeDetail.UserId);
                            user = accountResult.Result;
                        }
                        foreach (var item in codeDetails)
                        {
                            item.Mobile = user?.MobileNumber;
                            result.Add(item);
                        }
                    }
                }
                if (!result.Any())
                {
                    var bankRecords = BankMRManager.GetBankMRActivityCodeRecordByServiceCode(new List<string>() { code });
                    var bankMrServiceCodeDetails = await BankMRManager.SearchBankMRActivityCodeDetailByRecords(bankRecords);
                    if (bankMrServiceCodeDetails != null && bankMrServiceCodeDetails.Any())
                    {
                        result.AddRange(bankMrServiceCodeDetails);
                    }
                }
            }


            return Tuple.Create(result, totalCount);
        }
        /// <summary>
        /// 根据服务码获取服务码详情信息，没有用户信息
        /// </summary>
        /// <param name="serviceCodes"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ServiceCodeDetail>> GetServiceCodeDetail(IEnumerable<string> serviceCodes)
        {
            IEnumerable<ServiceCodeDetail> result = null;
            if (serviceCodes != null && serviceCodes.Any())
            {
                var packageDetailCodes = GetBeautyServicePackageDetailCodesByCodes(serviceCodes);
                var serviceCodeDetails = await GetServiceCodeDetailsByCodes(serviceCodes);
                if (packageDetailCodes != null && serviceCodeDetails != null && serviceCodeDetails.Any())
                {
                    var packageDetailIds = packageDetailCodes.Select(t => t.PackageDetailId).Distinct();
                    var packageDetails = BeautyServicePackageManager.GetBeautyServicePackageDetails(packageDetailIds);
                    if (packageDetails != null)
                    {
                        var pids = packageDetails.Select(t => t.PID).Distinct();
                        var products = new List<BeautyProductModel>();
                        foreach (var pid in pids)
                        {
                            var product = BeautyProductManager.GetBeautyProductByPid(pid);
                            if (product != null)
                            {
                                products.Add(product);
                            }
                        }

                        result = serviceCodeDetails.Select(t =>
                        {
                            var packageDetailCode = packageDetailCodes.FirstOrDefault(h => string.Equals(h.ServiceCode, t.Code));
                            if (packageDetailCode != null)
                            {
                                var packageDetailId = packageDetailCode.PackageDetailId;
                                var packageDetail = packageDetails.FirstOrDefault(d => d.PKID == packageDetailId);
                                if (packageDetail != null && products.Any())
                                {
                                    var bankManger = new BankMRManager();
                                    var cooperateUser = bankManger.FetchMrCooperateUserConfigByPKID(packageDetail.CooperateId);
                                    var product = products.FirstOrDefault(p => string.Equals(p.PID, packageDetail.PID));
                                    if (product != null)
                                    {
                                        return new ServiceCodeDetail()
                                        {
                                            PackageDetailCodeId = packageDetailCode.PKID,
                                            BatchCode = string.IsNullOrEmpty(packageDetailCode.ImportBatchCode) ? packageDetailCode.PackageDetailId.ToString() :
                                            packageDetailCode.ImportBatchCode,
                                            ServiceCode = t.Code,
                                            Status = string.Equals(t.Source, "VOLRevert") ? "Reverted" : t.Status.ToString(),
                                            VipCompanyName = cooperateUser?.CooperateName,
                                            VerifyTime = t.VerifyTime,
                                            StartTime = packageDetailCode.StartTime,
                                            EndTime = packageDetailCode.EndTime,
                                            VipSettleMentPrice = packageDetail.VipSettlementPrice,
                                            ShopCommission = packageDetail.ShopCommission,
                                            PID = packageDetail.PID,
                                            RestrictVehicle = BeautyProductManager.GetVehicleTypeDescription(product.RestrictVehicleType),
                                            OrderNo = t.TuhuOrderId > 0 ? $"TH{t.TuhuOrderId}" : null,
                                            VerifyShop = t.InstallShopId?.ToString() ,
                                            Type= packageDetail.IsImportUser? "ImportUser": "ServiceCode"
                                        };
                                    }
                                    else
                                    {
                                        return new ServiceCodeDetail();
                                    }
                                }
                                else
                                {
                                    return new ServiceCodeDetail();
                                }
                            }
                            else
                            {
                                return new ServiceCodeDetail();
                            }
                        }).Where(s => !string.IsNullOrWhiteSpace(s.ServiceCode)).ToList();
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 批量根据服务码获取服务码信息
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public static IEnumerable<BeautyServicePackageDetailCode> GetBeautyServicePackageDetailCodesByCodes(IEnumerable<string> codes)
        {
            IEnumerable<BeautyServicePackageDetailCode> result = null;

            try
            {
                result = BeautyServicePackageDal.SelectBeautyServicePackageDetailCodesByCodes(codes);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 根据用户查询服务码信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<BeautyServicePackageDetailCode>, int> GetBeautyServicePackageDetailCodesByUserId(Guid userId, int pageIndex, int pageSize)
        {
            Tuple<IEnumerable<BeautyServicePackageDetailCode>, int> result = null;

            try
            {
                result = BeautyServicePackageDal.SelectBeautyServicePackageDetailCodesByUserId(userId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 根据服务码调快修获取服务码快修维护的信息
        /// </summary>
        /// <param name="serviceCodes"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ServiceCode>> GetServiceCodeDetailsByCodes(IEnumerable<string> serviceCodes)
        {
            using (var client = new ServiceCodeClient())
            {
                var serviceResult = await client.SelectServiceCodeDetailsByCodesAsync(serviceCodes);
                serviceResult.ThrowIfException(true);
                return serviceResult.Result;
            }
        }

        /// <summary>
        /// 修改大客户美容服务码
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pkids"></param>
        /// <returns></returns>
        public static bool UpdateBeautyServicePackageDetailCodes(ServiceCodeDetail model, string user, ServiceCodeDetail oldCode)
        {
            var result = false;
            try
            {
                if (model.StartTime != null && model.EndTime != null)
                {

                    using (var dbhelper = new Tuhu.Component.Common.SqlDbHelper(writeConnStr))
                    {
                        dbhelper.BeginTransaction();
                        var upResult = true;
                        if (model.PackageDetailCodeId > 0)
                        {
                            var oldValue = BeautyServicePackageDal.GetBeautyServicePackageDetailCodeByPKID(model.PackageDetailCodeId);
                            if (oldValue != null)
                            {
                                #region 只更新服务码起止时间 其余数据保持不变
                                var newValue = new BeautyServicePackageDetailCode
                                {
                                    PKID = model.PackageDetailCodeId,
                                    UserId = oldValue.UserId,
                                    PackageDetailId = oldValue.PackageDetailId,
                                    ServiceCode = oldValue.ServiceCode,
                                    IsActive = oldValue.IsActive,
                                    StartTime = model.StartTime,
                                    EndTime = model.EndTime,
                                    PackageCode = oldValue.PackageCode,
                                    VipUserId = oldValue.VipUserId
                                };
                                #endregion
                                upResult = BeautyServicePackageDal.UpdateBeautyServicePackageDetailCodes(dbhelper, newValue);
                            }
                            else
                            {
                                upResult = false;
                            }
                        }
                        if (upResult)
                        {
                            using (var client = new ServiceCodeClient())
                            {

                                var kxChangeCodeTimeModel = new ChangeCodeTimeRequest
                                {
                                    ServiceCodes = new List<string>() { model.ServiceCode },
                                    OverdueTime = model.EndTime
                                };
                                var kxChangeResult = client.ChangeOverdueTime(kxChangeCodeTimeModel);
                                kxChangeResult.ThrowIfException(true);
                                if (kxChangeResult.Success && kxChangeResult.Result != null && kxChangeResult.Result.SuccessServiceCode.Count() > 0
                                    && string.Equals(kxChangeResult.Result.SuccessServiceCode.FirstOrDefault(), model.ServiceCode))
                                {
                                    result = true;
                                    dbhelper.Commit();

                                    var log = new BaoYangOprLog
                                    {
                                        LogType = "修改大客户美容服务码",
                                        IdentityID = oldCode.ServiceCode,
                                        OldValue = JsonConvert.SerializeObject(oldCode),
                                        NewValue = JsonConvert.SerializeObject(model),
                                        Remarks = $"修改服务码有效期:服务码开始时间从{oldCode.StartTime}变更为{model.StartTime},结束时间从{oldCode.EndTime}变更为{model.EndTime}",
                                        OperateUser = user,
                                    };
                                    LoggerManager.InsertLog("BeautyOprLog", log);
                                }
                                else
                                {
                                    throw new Exception($"快修服务修改服务码有效期失败,请求信息:{JsonConvert.SerializeObject(kxChangeCodeTimeModel)},返回信息:{JsonConvert.SerializeObject(kxChangeResult)}");
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 根据门店id获取门店信息
        /// </summary>
        /// <param name="shopIds"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, string>> GetShopInfoByShopIds(IEnumerable<int> shopIds)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (shopIds == null || !shopIds.Any())
                return result;

            using (var client = new Tuhu.Service.Shop.ShopClient())
            {
                var shops = (await client.SelectShopDetailsAsync(shopIds.ToArray())).Result?.ToArray();
                if (shops != null && shops.Any())
                {
                    shops.ForEach(f =>
                    {
                        if (!result.ContainsKey(f.ShopId.ToString()))
                        {
                            result.Add(f.ShopId.ToString(), f.SimpleName);
                        }
                    });
                }
            }
            return result;

        }
    }
}
