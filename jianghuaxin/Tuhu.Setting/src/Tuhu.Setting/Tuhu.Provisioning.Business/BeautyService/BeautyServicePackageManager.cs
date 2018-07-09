using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Nosql;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.Business.Discovery;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO.BeautyServicePackageDal;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;
using Tuhu.Provisioning.DataAccess.Entity.UnivRedemptionCode;
using Tuhu.Service.KuaiXiu;
using Tuhu.Service.KuaiXiu.Models;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.Provisioning.Business.BeautyService
{
    public class BeautyServicePackageManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BeautyServicePackageManager));

        private static readonly string CacheClientName = "settting";

        public static Tuple<IEnumerable<BeautyServicePackage>, int> GetBeautyServicePackage(int pageIndex, int pageSize, string packageType,
              string packageName, string vipCompanyName, string settlementMethod, int cooperateId)
        {
            Tuple<IEnumerable<BeautyServicePackage>, int> result = null;

            try
            {
                result = BeautyServicePackageDal.SelectBeautyServicePackage(pageIndex, pageSize, packageType,
                    packageName, vipCompanyName, settlementMethod, cooperateId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        public static IEnumerable<string> GetAllVipUserName()
        {
            IEnumerable<string> result = null;

            try
            {
                result = BeautyServicePackageDal.SelectAllVipUserName();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        public static BeautyServicePackage GetBeautyServicePackage(int packageId)
        {
            BeautyServicePackage result = null;

            try
            {
                result = BeautyServicePackageDal.SelectBeautyServicePackage(packageId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        public static BeautyServicePackageDetail GetBeautyServicePackageDetail(int packageDetailId)
        {
            BeautyServicePackageDetail result = null;

            try
            {
                result = BeautyServicePackageDal.SelectBeautyServicePackageDetail(packageDetailId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 根据美容礼包详情id获取礼包详情
        /// </summary>
        /// <param name="packageDetailIds"></param>
        /// <returns></returns>
        public static IEnumerable<BeautyServicePackageDetail> GetBeautyServicePackageDetails(IEnumerable<int> packageDetailIds)
        {
            IEnumerable<BeautyServicePackageDetail> result = null;

            try
            {
                result = BeautyServicePackageDal.SelectBeautyServicePackageDetails(packageDetailIds);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        public static IEnumerable<BeautyServicePackageDetail> GetBeautyServicePackageDetails(int packageId)
        {
            IEnumerable<BeautyServicePackageDetail> result = null;

            try
            {
                result = BeautyServicePackageDal.SelectBeautyServicePackageDetails(packageId);
                if (result != null && result.Any())
                {
                    var cooperateIds = result.Select(s => s.CooperateId).Where(s => s > 0).Distinct();
                    var cooperateUsers = cooperateIds.Select(s => new BankMRManager().FetchMrCooperateUserConfigByPKID(s)).Where(s => s != null);
                    result.ForEach(s => s.CooperateName = cooperateUsers.FirstOrDefault(u => u.PKID == s.CooperateId)?.CooperateName);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 分页查询服务码配置信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isImportUser"></param>
        /// <param name="settlementMethod"></param>
        /// <param name="cooperateId"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<BeautyServicePackageDetail>, int> GetBeautyServicePackageDetails(int pageIndex, int pageSize,
            bool isImportUser, string settlementMethod, int cooperateId, string serviceId)
        {

            Tuple<IEnumerable<BeautyServicePackageDetail>, int> result = null;

            try
            {
                result = BeautyServicePackageDal.SelectBeautyServicePackageDetails(pageIndex, pageSize, isImportUser, settlementMethod,
                    cooperateId, serviceId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        public static IEnumerable<BeautyServiceCodeTypeConfig> GetBeautyServiceCodeTypeConfig()
        {
            IEnumerable<BeautyServiceCodeTypeConfig> result = null;

            try
            {
                result = BeautyServicePackageDal.SelectBeautyServiceCodeTypeConfig();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        public static bool InsertBeautyServicePackage(BeautyServicePackage package)
        {
            bool result = false;

            try
            {
                var insertResult = BeautyServicePackageDal.InsertBeautyServicePackage(package);
                result = insertResult.Item1;
                var log = new BeautyOprLog
                {
                    LogType = "InsertBeautyServicePackage",
                    IdentityID = $"{insertResult.Item2}",
                    OldValue = null,
                    NewValue = JsonConvert.SerializeObject(package),
                    Remarks = $"插入集团客户礼包",
                    OperateUser = package.CreateUser,
                };
                LoggerManager.InsertLog("BeautyOprLog", log);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        public static bool UpdateBeautyServicePackage(BeautyServicePackage package)
        {
            bool result = false;

            try
            {
                var oldValue = GetBeautyServicePackage(package.PKID);
                var writeStr = BeautyServicePackageDal.GetTuhuGrouponWriteConnstr();
                using (var dbhelper = new SqlDbHelper(writeStr))
                {
                    dbhelper.BeginTransaction();
                    var updateBeautyServicePackageResult = BeautyServicePackageDal.UpdateBeautyServicePackage(dbhelper, package);
                    BeautyServicePackageDal.UpdateBeautyServicePackageDetailCooperateIdByPackageId(dbhelper, package.PKID, package.CooperateId);
                    if (!updateBeautyServicePackageResult)
                        throw new Exception("更新兑换码配置失败");
                    if (package.IsPackageCodeGenerated && (package.PackageCodeStartTime != oldValue.PackageCodeStartTime
                        || package.PackageCodeEndTime != oldValue.PackageCodeEndTime))
                    {
                        var updateBeautyServicePackageCodeTimeResult = BeautyServicePackageDal.UpdateBeautyServicePackageCodeTime(dbhelper, package.PKID, package.PackageCodeStartTime, package.PackageCodeEndTime);
                        if (!updateBeautyServicePackageCodeTimeResult)
                            throw new Exception("更新兑换码时间失败");
                    }
                    dbhelper.Commit();
                    result = true;
                    var log = new BeautyOprLog
                    {
                        LogType = "UpdateBeautyServicePackage",
                        IdentityID = $"{package.PKID}",
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = JsonConvert.SerializeObject(package),
                        Remarks = $"修改集团客户礼包,礼包ID为:{package.PKID}",
                        OperateUser = package.UpdateUser,
                    };
                    LoggerManager.InsertLog("BeautyOprLog", log);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        public static bool InsertBeautyServicePackageDetail(BeautyServicePackageDetail packageDetail)
        {
            bool result = false;

            try
            {
                var insertResult = BeautyServicePackageDal.InsertBeautyServicePackageDetail(packageDetail);
                result = insertResult.Item1;
                var log = new BeautyOprLog
                {
                    LogType = "InsertBeautyServicePackageDetail",
                    IdentityID = $"{insertResult.Item2}",
                    OldValue = null,
                    NewValue = JsonConvert.SerializeObject(packageDetail),
                    Remarks = $"插入礼包详情,礼包ID:{packageDetail.PackageId}",
                    OperateUser = packageDetail.CreateUser,
                };
                LoggerManager.InsertLog("BeautyOprLog", log);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        public static Tuple<bool, string> UpdateBeautyServicePackageDetail(BeautyServicePackageDetail packageDetail)
        {
            bool result = false;
            var msg = string.Empty;
            var writeStr = BeautyServicePackageDal.GetTuhuGrouponWriteConnstr();
            using (var dbhelper = new SqlDbHelper(writeStr))
            {
                try
                {
                    var oldValue = GetBeautyServicePackageDetail(packageDetail.PKID);
                    dbhelper.BeginTransaction();
                    var updatePackageDetailResult = BeautyServicePackageDal.UpdateBeautyServicePackageDetail(dbhelper, packageDetail);
                    if (!updatePackageDetailResult)
                        throw new Exception("更新服务码配置失败");
                    if (packageDetail.IsServiceCodeGenerated && (oldValue.ServiceCodeStartTime != packageDetail.ServiceCodeStartTime || oldValue.ServiceCodeEndTime != packageDetail.ServiceCodeEndTime))
                    {
                        if (oldValue.ServiceCodeNum > 10000)
                        {
                            throw new Exception("大于10000暂时不支持修改");
                        }
                        var updateServiceCodeTimeResult = BeautyServicePackageDal.UpdateServiceCodeTime(dbhelper, packageDetail.PKID,
                            packageDetail.ServiceCodeStartTime, packageDetail.ServiceCodeEndTime);
                        if (!updateServiceCodeTimeResult)
                            throw new Exception("更新服务码时间失败");
                        if (oldValue.ServiceCodeEndTime != packageDetail.ServiceCodeEndTime && packageDetail.ServiceCodeEndTime != null)
                        {
                            var serviceCodes = (GetBeautyServicePackageDetailCodesByPackageDetailId(packageDetail.PKID))?.Select(t => t.ServiceCode)?.ToList();
                            if (serviceCodes != null && serviceCodes.Any())
                            {
                                using (var client = new ServiceCodeClient())
                                {
                                    var batchSize = 1000;
                                    var index = 0;
                                    while (index < serviceCodes.Count)
                                    {
                                        var batchCodes = serviceCodes.Skip(index).Take(batchSize).ToList();
                                        var kxChangeCodeTimeModel = new ChangeCodeTimeRequest
                                        {
                                            ServiceCodes = batchCodes,
                                            OverdueTime = Convert.ToDateTime(packageDetail.ServiceCodeEndTime)
                                        };
                                        var kxChangeResult = client.ChangeOverdueTime(kxChangeCodeTimeModel);
                                        if (!kxChangeResult.Success)
                                            throw kxChangeResult.Exception;
                                        if (kxChangeResult.Result != null && kxChangeResult.Result.FailServiceCode != null && kxChangeResult.Result.FailServiceCode.Any())
                                        {
                                            var serviceCodeFailedLog = new BeautyOprLog
                                            {
                                                LogType = "ChangeOverdueTimeFailedCodes",
                                                IdentityID = $"{packageDetail.PKID}",
                                                OldValue = null,
                                                NewValue = JsonConvert.SerializeObject(kxChangeResult.Result.FailServiceCode),
                                                Remarks = $"修改服务码过期时间失败",
                                                OperateUser = packageDetail.UpdateUser,
                                            };
                                            LoggerManager.InsertLog("BeautyOprLog", serviceCodeFailedLog);
                                        }
                                        index += batchSize;
                                    }
                                }
                            }
                        }
                    }
                    dbhelper.Commit();
                    result = true;
                    var log = new BeautyOprLog
                    {
                        LogType = "UpdateBeautyServicePackageDetail",
                        IdentityID = $"{packageDetail.PKID}",
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = JsonConvert.SerializeObject(packageDetail),
                        Remarks = $"修改集团客户礼包详情,详情ID为{packageDetail.PKID}",
                        OperateUser = packageDetail.UpdateUser,
                    };
                    LoggerManager.InsertLog("BeautyOprLog", log);

                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    dbhelper.Rollback();
                    Logger.Error(ex.Message, ex);
                };
            }
            return new Tuple<bool, string>(result, msg);
        }

        public static bool DeleteBeautyServicePackage(int pkid, BeautyServicePackage oldValue, string operateUser)
        {
            bool result = false;

            try
            {
                result = BeautyServicePackageDal.DeleteBeautyServicePackage(pkid);
                var log = new BeautyOprLog
                {
                    LogType = "DeleteBeautyServicePackage",
                    IdentityID = $"{pkid}",
                    OldValue = JsonConvert.SerializeObject(oldValue),
                    NewValue = null,
                    Remarks = $"删除集团客户礼包,礼包ID:{pkid}",
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

        public static bool DeleteBeautyServicePackageDetail(int pkid, BeautyServicePackageDetail oldValue, string operateUser)
        {
            bool result = false;

            try
            {
                result = BeautyServicePackageDal.DeleteBeautyServicePackageDetail(pkid);
                var log = new BeautyOprLog
                {
                    LogType = "DeleteBeautyServicePackageDetail",
                    IdentityID = $"{pkid}",
                    OldValue = JsonConvert.SerializeObject(oldValue),
                    NewValue = null,
                    Remarks = $"删除集团客户礼包详情,详情ID:{pkid}",
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

        public static IEnumerable<BeautyServiceCodeTypeConfig> SelectAllBeautyServiceCodeTypeConfig(int ServerType = 0)
        {
            IEnumerable<BeautyServiceCodeTypeConfig> result = null;

            try
            {
                result = BeautyServicePackageDal.SelectAllBeautyServiceCodeTypeConfig();
                if (ServerType == 1)
                    result = result.Where(w => w.ServerType == 1);
                else if (ServerType == 0)
                {
                    result = result.Where(w => w.ServerType == 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        public static bool InsertBeautyServiceCodeTypeConfig(BeautyServiceCodeTypeConfig config)
        {
            bool result = false;

            try
            {
                result = BeautyServicePackageDal.InsertBeautyServiceCodeTypeConfig(config);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }
        /// <summary>
        /// 更新服务码配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool UpdateBeautyServiceCodeTypeConfig(BeautyServiceCodeTypeConfig config)
        {
            bool result = false;

            try
            {
                result = BeautyServicePackageDal.UpdateBeautyServiceCodeTypeConfig(config);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        public static IEnumerable<BeautyServicePackageCode> GetBeautyServicePackageCodesByPackageId(int packageId)
        {
            IEnumerable<BeautyServicePackageCode> result = null;

            try
            {
                result = BeautyServicePackageDal.SelectBeautyServicePackageCodesByPackageId(packageId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        public static IEnumerable<BeautyServicePackageDetailCode> GetBeautyServicePackageDetailCodesByPackageDetailId(int packageDetailId)
        {
            IEnumerable<BeautyServicePackageDetailCode> result = null;

            try
            {
                result = BeautyServicePackageDal.SelectBeautyServicePackageDetailCodesByPackageDetailId(packageDetailId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        public static async Task<bool> GenerateServiceCodes(int packageDetailId)
        {
            var result = false;
            var hasGenerateServiceCodes = new List<string>();
            var currentProduct = GetBeautyServicePackageDetail(packageDetailId);
            var package = GetBeautyServicePackage(currentProduct.PackageId);
            var cooperateUser = new BankMRManager().FetchMrCooperateUserConfigByPKID(currentProduct.CooperateId);
            if ((string.Equals(package?.PackageType, "serviceCode") || currentProduct.PackageId <= 0) && cooperateUser != null)
            {
                var writeStr = BeautyServicePackageDal.GetTuhuGrouponWriteConnstr();
                using (var dbhelper = new SqlDbHelper(writeStr))
                {
                    dbhelper.BeginTransaction();
                    var key = $"ServiceCodeGenerateingRate/{packageDetailId}";
                    try
                    {
                        var waitingGenerateCount = currentProduct.ServiceCodeNum;
                        var betchSize = 100;
                        while (waitingGenerateCount > 0)
                        {
                            var generateCount = waitingGenerateCount > betchSize ? betchSize : waitingGenerateCount;
                            var serviceCodes = await GenerateServiceCode(currentProduct.PID, generateCount, currentProduct.Name, 0, currentProduct.ServiceCodeEndTime);
                            if (serviceCodes != null && serviceCodes.Count() == generateCount)
                            {
                                hasGenerateServiceCodes.AddRange(serviceCodes);
                                var rows = BeautyServicePackageDal.InsertBeautyServicePackageDetailCodes(dbhelper, currentProduct, cooperateUser.VipUserId, serviceCodes);
                                if (rows == generateCount)
                                {
                                    waitingGenerateCount -= generateCount;
                                }
                                else
                                {
                                    throw new Exception($"服务码插入数据库的没有成功，rows:{rows},generateCount:{generateCount}");
                                }
                                using (var client = CacheHelper.CreateCacheClient(CacheClientName))
                                {
                                    var process = (currentProduct.ServiceCodeNum - waitingGenerateCount) * 1.0f / currentProduct.ServiceCodeNum;
                                    await client.SetAsync(key, process, TimeSpan.FromMinutes(20));
                                }
                            }
                            else
                            {
                                throw new Exception($"快修服务生成的服务码和请求的数量不相等，PID:{currentProduct.PID},name:{currentProduct.Name}," +
                                    $"needCount:{generateCount},realCount：{serviceCodes.Count()}");
                            }
                        }
                        if (string.Equals(currentProduct.SettlementMethod, "PreSettled"))
                        {
                            var createOrderResult = await OrderManager.CreateServiceCodeOrderForVip(cooperateUser, currentProduct);
                            if (createOrderResult.OrderId <= 0)
                            {
                                throw new Exception($"创建大客户兑换码订单失败，packageDetailId：{packageDetailId}，OrderId{createOrderResult.OrderId}");
                            }
                            else
                            {
                                var setOrderIdResult = BeautyServicePackageDal.SetPackageDetailBuyoutOrderId(dbhelper, packageDetailId, createOrderResult.OrderId);
                                if (!setOrderIdResult)
                                {
                                    throw new Exception("设置买断订单Id字段失败");
                                }
                            }
                        }
                        var setResult = BeautyServicePackageDal.SetServiceCodeIsGenerated(dbhelper, packageDetailId);
                        if (!setResult)
                        {
                            throw new Exception("设置是否生成服务码字段失败");
                        }
                        dbhelper.Commit();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        dbhelper.Rollback();
                        using (var client = CacheHelper.CreateCacheClient(CacheClientName))
                        {
                            await client.SetAsync(key, 0, TimeSpan.FromMinutes(20));
                        }
                        await RevertVOLServiceCode(hasGenerateServiceCodes);
                        Logger.Error("批量生成服务码错误", ex);
                    }
                }
            }

            return result;
        }

        public static async Task<bool> GeneratePackageCodes(int packageId)
        {
            var result = false;
            var package = GetBeautyServicePackage(packageId);
            if (package != null && package.PackageCodeNum > 0)
            {
                var writeStr = BeautyServicePackageDal.GetTuhuGrouponWriteConnstr();
                using (var dbhelper = new SqlDbHelper(writeStr))
                {
                    dbhelper.BeginTransaction();
                    var key = $"PacakgeCodeGenerateingRate/{packageId}";
                    try
                    {
                        var waitingGenerateCount = package.PackageCodeNum;
                        var batchSize = 1000;
                        while (waitingGenerateCount > 0)
                        {
                            var generateCount = batchSize < waitingGenerateCount ? batchSize : waitingGenerateCount;
                            var rows = BeautyServicePackageDal.InsertBeautyServicePackageCodes(dbhelper, package, generateCount);
                            if (generateCount == rows)
                            {
                                waitingGenerateCount -= generateCount;
                            }
                            else
                            {
                                throw new Exception($"兑换码插入数据库的没有成功，rows:{rows},generateCount:{generateCount}");
                            }
                            using (var client = CacheHelper.CreateCacheClient(CacheClientName))
                            {
                                var process = (package.PackageCodeNum - waitingGenerateCount) * 1.0f / package.PackageCodeNum;
                                await client.SetAsync(key, process, TimeSpan.FromMinutes(20));
                            }
                        }
                        if (string.Equals(package.SettlementMethod, "PreSettled"))
                        {
                            var pacakgeDetails = GetBeautyServicePackageDetails(packageId);
                            var createOrderResult = await OrderManager.CreatePackageCodeOrderForVipUser(package, pacakgeDetails);
                            if (createOrderResult.OrderId <= 0)
                            {
                                throw new Exception($"创建大客户兑换码订单失败，packageId：{packageId}，OrderId{createOrderResult.OrderId}");
                            }
                            else
                            {
                                var setOrderIdResult = BeautyServicePackageDal.SetPackageBuyoutOrderId(dbhelper, packageId, createOrderResult.OrderId);
                                if (!setOrderIdResult)
                                {
                                    throw new Exception("设置买断订单Id字段失败");
                                }
                            }
                        }
                        var setResult = BeautyServicePackageDal.SetPackageCodeIsGenerated(dbhelper, packageId);
                        if (!setResult)
                        {
                            throw new Exception("设置是否兑换字段失败");
                        }
                        dbhelper.Commit();
                        result = true;

                    }
                    catch (Exception ex)
                    {
                        dbhelper.Rollback();
                        using (var client = CacheHelper.CreateCacheClient(CacheClientName))
                        {
                            await client.SetAsync(key, 0, TimeSpan.FromMinutes(20));
                        }
                        Logger.Error("批量生成兑换码错误", ex);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 根据批次号更新服务码起始时间
        /// </summary>
        /// <param name="user"></param>
        /// <param name="batchCode"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static async Task<Tuple<bool, string>> UpdateBeautyServiceCodeTimeByBatchCode(string user, string batchCode, DateTime startTime, DateTime endTime)
        {
            var result = false;
            var msg = string.Empty;
            try
            {
                var writeStr = BeautyServicePackageDal.GetTuhuGrouponWriteConnstr();
                using (var dbhelper = new SqlDbHelper(writeStr))
                {
                    dbhelper.BeginTransaction();
                    var updateCodeResult = BeautyServicePackageDal.UpdateBeautyServicePackageDetailCodeTimeByBatchCode(dbhelper, batchCode, startTime, endTime);
                    if (!updateCodeResult)
                        throw new Exception("根据批次号更新服务码时间失败,方法UpdateBeautyServicePackageDetailCodeTimeByBatchCode");
                    var serviceCodes = BeautyServicePackageDal.SelectServiceCodesByBatchCode(batchCode);
                    if (serviceCodes != null && serviceCodes.Any())
                    {
                        if (serviceCodes.Count() > 10000)
                            throw new Exception("暂时不支持服务码数量大于10000的修改操作");
                        using (var client = new ServiceCodeClient())
                        {
                            var batchSize = 1000;
                            var index = 0;
                            while (index < serviceCodes.Count())
                            {
                                var batchCodes = serviceCodes.Skip(index).Take(batchSize).ToList();
                                var kxChangeCodeTimeModel = new ChangeCodeTimeRequest
                                {
                                    ServiceCodes = batchCodes,
                                    OverdueTime = endTime
                                };
                                var kxChangeResult = client.ChangeOverdueTime(kxChangeCodeTimeModel);
                                if (!kxChangeResult.Success)
                                    throw kxChangeResult.Exception;
                                if (kxChangeResult.Result != null && kxChangeResult.Result.FailServiceCode != null && kxChangeResult.Result.FailServiceCode.Any())
                                {
                                    var serviceCodeFailedLog = new BeautyOprLog
                                    {
                                        LogType = "ChangeOverdueTimeFailedCodes",
                                        IdentityID = $"{batchCode}",
                                        OldValue = null,
                                        NewValue = JsonConvert.SerializeObject(kxChangeResult.Result.FailServiceCode),
                                        Remarks = $"根据批次号修改服务码过期时间失败,批次:{batchCode},OverdueTime应该改为：{endTime}",
                                        OperateUser = user,
                                    };
                                    LoggerManager.InsertLog("BeautyOprLog", serviceCodeFailedLog);
                                }
                                index += batchSize;
                            }
                            dbhelper.Commit();
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                Logger.Error(ex.Message, ex);
            }
            return new Tuple<bool, string>(result, msg);
        }

        public static async Task<IEnumerable<CreateBeautyCodeTaskModel>> SelectCreateBeautyCodeTaskModels(string batchCode)
        {
            var result = BeautyServicePackageDal.SelectCreateBeautyCodeTaskModels(batchCode);
            return await Task.FromResult(result ?? new List<CreateBeautyCodeTaskModel>());
        }

        public static async Task<decimal> GetServiceCodeGenerateingRate(int packageDetailId)
        {
            using (var client = CacheHelper.CreateCacheClient(CacheClientName))
            {
                var key = $"ServiceCodeGenerateingRate/{packageDetailId}";
                var cacheResult = await client.GetAsync<decimal>(key);
                return cacheResult.Value;
            }
        }

        public static async Task<decimal> GetPacakgeCodeGenerateingRate(int packageId)
        {
            using (var client = CacheHelper.CreateCacheClient(CacheClientName))
            {
                var key = $"PacakgeCodeGenerateingRate/{packageId}";
                var cacheResult = await client.GetAsync<decimal>(key);
                return cacheResult.Value;
            }
        }

        public static async Task<IEnumerable<string>> GenerateServiceCode(string pid, int num, string name, long orderId, DateTime? endTime)
        {
            using (var client = new ServiceCodeClient())
            {
                GenerateServiceCodeRequest request = new GenerateServiceCodeRequest()
                {
                    TuhuOrderId = orderId,
                    ServiceId = pid,
                    ServiceName = name,
                    OverdueTime = endTime,
                    GenerateNum = num
                };
                var serviceResult = await client.GenerateVOLServiceCodeAsync(request);
                serviceResult.ThrowIfException(true);
                return serviceResult.Result;
            }
        }

        public static async Task<bool> RevertVOLServiceCode(List<string> serviceCodes)
        {
            var result = false;
            if (serviceCodes != null && serviceCodes.Any())
            {
                using (var client = new ServiceCodeClient())
                {
                    var request = new RevertVOLServiceCodeRequest()
                    {
                        ServiceCodes = serviceCodes
                    };
                    var serviceResult = await client.RevertVOLServiceCodeAsync(request);
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result;
                }
            }

            return result;
        }

        public static List<EnterpriseUserViewModel> GetEnterpriseUserConfig(int pageIndex, int pageSize, Guid userId, out int total)
        {
            List<EnterpriseUserViewModel> result = null;
            total = 0;
            try
            {
                var userIdList = BeautyServicePackageDal.GetEnterpriseUserUserId(pageIndex, pageSize, userId, out total);
                if (userIdList != null && userIdList != null && userIdList.Any())
                {
                    var userServiceDetails = BeautyServicePackageDal.GetEnterpriseUserServiceConfig(userIdList);
                    var userModuleConfigs = BeautyServicePackageDal.GetEnterpriseUserModuleConfig(userIdList);
                    if (userServiceDetails != null && userServiceDetails.Any())
                    {
                        result = new List<EnterpriseUserViewModel>();
                        foreach (var userService in userServiceDetails)
                        {
                            if (result.Where(x => x.UserId == userService.UserId).Count() <= 0)
                            {
                                //List<CooperateUserConfig> cooperateUserConfig = new List<CooperateUserConfig>();
                                //var cooperateUserServices = BeautyServicePackageDal.GetCooperateUserServices(userServiceDetails.Where(z => z.UserId == userService.UserId).Select(x => x.PackageDetailsId).ToList());
                                //if (cooperateUserServices != null && cooperateUserServices.Any())
                                //{
                                //    foreach (var cooperate in cooperateUserServices)
                                //    {
                                //        if (cooperateUserConfig.Where(x => x.CooperateId == cooperate.CooperateId).Count() <= 0)
                                //        {
                                //            CooperateUserConfig config = new CooperateUserConfig()
                                //            {
                                //                CooperateId = cooperate.CooperateId,
                                //                CooperateName = cooperate.CooperateName,
                                //                CooperateUserServiceDetails = cooperateUserServices?.Where(z => z.CooperateId == cooperate.CooperateId).ToList()
                                //            };
                                //            cooperateUserConfig.Add(config);
                                //        }
                                //    }
                                //}
                                EnterpriseUserViewModel item = new EnterpriseUserViewModel()
                                {
                                    UserId = userService.UserId,
                                    Remark = userService.Remark,
                                    CooperateUserServiceDeails = BeautyServicePackageDal.GetCooperateUserServices(userServiceDetails.Where(z => z.UserId == userService.UserId).Select(x => x.PackageDetailsId).ToList()),
                                    UserModuleDetails = userModuleConfigs?.Where(z => z.UserId == userService.UserId).ToList(),
                                    CreateDateTime = userService.CreateDateTime,
                                    UpdateDateTime = userService.UpdateDateTime
                                };
                                result.Add(item);
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

        public static IEnumerable<CooperateUserService> SelectCooperateUserByCompanyId(int companyId)
        {
            IEnumerable<CooperateUserService> result = null;

            try
            {
                var userDetails = UserAccountService.GetCompanyUsersByCompanyId(companyId);
                if (userDetails != null && userDetails.Any())
                {
                    result = BeautyServicePackageDal.SelectCooperateUserByUserIdList(userDetails.Select(x => x.UserId).ToList());
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            };

            return result;
        }

        public static bool UsertEnterpriseUserConfig(EnterpriseUserViewModel data, string user)
        {
            var result = false;
            try
            {
                using (var dbhelper = new SqlDbHelper(BeautyServicePackageDal.GetTuhuGrouponWriteConnstr()))
                {
                    dbhelper.BeginTransaction();
                    var userConfig = BeautyServicePackageDal.SelectEnterpriseUserConfigByUserId(dbhelper, data.UserId);
                    var moduleConfig = BeautyServicePackageDal.SelectEnterpriseUserModuleConfigByUserId(dbhelper, data.UserId);
                    if (userConfig != null && userConfig.Any())
                        BeautyServicePackageDal.DeleteEnterpriseUserConfigByUserId(dbhelper, data.UserId);
                    if (moduleConfig != null && moduleConfig.Any())
                        BeautyServicePackageDal.DeleteEnterpriseModuleConfigByUserId(dbhelper, data.UserId);
                    if (data.PackageDetailIds != null && data.PackageDetailIds.Any())
                    {
                        foreach (var item in data.PackageDetailIds)
                        {
                            BeautyServicePackageDal.InsertEnterpriseUserConfig(dbhelper, data.UserId, item, data.Remark);
                        }
                    }
                    if (data.UserModuleDetails != null && data.UserModuleDetails.Any())
                    {
                        foreach (var item in data.UserModuleDetails)
                        {
                            BeautyServicePackageDal.InsertEnterpriseUserModuleConfig(dbhelper, data.UserId, item.ModuleType);
                        }
                    }
                    dbhelper.Commit();
                    result = true;
                    var log = new BaoYangOprLog
                    {
                        LogType = "EnterpriseUserConfig",
                        IdentityID = data.UserId.ToString(),
                        OldValue = "[]",
                        NewValue = JsonConvert.SerializeObject(data),
                        Remarks = $"发码服务权限修改 {data.UserMobile}",
                        OperateUser = user,
                    };
                    LoggerManager.InsertLog("BeautyOprLog", log);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }

            return result;
        }
        /// <summary>
        /// 根据兑换码，手机号查询美容兑换码信息
        /// </summary>
        /// <param name="packageCode"></param>
        /// <param name="mobile"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<UnivBeautyRedemptionCodeResult>,int> GetBeautyServicePackageCodes(string packageCode, string mobile, int pageIndex, int pageSize)
        {
            var result = new List<UnivBeautyRedemptionCodeResult>();
            var total = 0;
            try
            {
                if (!string.IsNullOrWhiteSpace(packageCode))
                {
                    var beautyServicePackageCode = BeautyServicePackageDal.FetchBeautyServicePackageCodeByPackageCode(packageCode);
                    if (beautyServicePackageCode != null)
                    {
                        if (beautyServicePackageCode.UserId != null)
                        {
                            var user = UserAccountService.GetUserById(beautyServicePackageCode.UserId.Value);
                            beautyServicePackageCode.Mobile = user?.MobileNumber;
                        }
                        if (!string.IsNullOrWhiteSpace(mobile))
                        {
                            if (beautyServicePackageCode.Mobile == mobile)
                            {
                                result.Add(beautyServicePackageCode);
                            }
                        }
                        else
                        {
                            result.Add(beautyServicePackageCode);
                        }
                        total = result.Count;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(mobile))
                {
                    var user = UserAccountService.GetUserByMobile(mobile);
                    if (user != null && user.UserId != Guid.Empty)
                    {
                        var beautyServicePackageCodes = BeautyServicePackageDal.SelectBeautyServicePackageCodesByUserId(user.UserId, pageIndex, pageSize);
                        beautyServicePackageCodes.Item1.ForEach(i => { i.Mobile = user.MobileNumber; });
                        result.AddRange(beautyServicePackageCodes.Item1);
                        total = beautyServicePackageCodes.Item2;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return new Tuple<IEnumerable<UnivBeautyRedemptionCodeResult>, int>(result, total);
        }
    }
}
