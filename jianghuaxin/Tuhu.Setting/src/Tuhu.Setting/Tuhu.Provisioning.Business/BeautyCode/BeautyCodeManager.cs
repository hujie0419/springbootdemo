using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Nosql;
using Tuhu.Provisioning.Business.BeautyService;
using Tuhu.Provisioning.Business.Sms;
using Tuhu.Provisioning.DataAccess.DAO.BankMRActivityDal;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;

namespace Tuhu.Provisioning.Business.BeautyCode
{
    public class BeautyCodeManager
    {
        #region static

        private static readonly Common.Logging.ILog Logger = LogManager.GetLogger(typeof(BeautyCodeManager));
        private static string _environment = ConfigurationManager.AppSettings["env"];
        private static string _companyMrRelatedUsers = ConfigurationManager.AppSettings["CompanyMrRelatedUsers"];
        private static readonly IDBScopeManager TuhuGrouponDbScopeManager;

        private static readonly IDBScopeManager TuhuGrouponDbScopeManagerReadOnly;

        static BeautyCodeManager()
        {
            var connRw = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
            var connRo = ConfigurationManager.ConnectionStrings["Tuhu_Groupon_ReadOnly"].ConnectionString;
            var connManagerRw = new ConnectionManager(SecurityHelp.IsBase64Formatted(connRw) ? SecurityHelp.DecryptAES(connRw) : connRw);
            var connManagerRo = new ConnectionManager(SecurityHelp.IsBase64Formatted(connRo) ? SecurityHelp.DecryptAES(connRo) : connRo);
            TuhuGrouponDbScopeManager = new DBScopeManager(connManagerRw);
            TuhuGrouponDbScopeManagerReadOnly = new DBScopeManager(connManagerRo);
        }

        #endregion


        private BeautyCodeHandler handler = new BeautyCodeHandler();


        public IEnumerable<BeautyServicePackageSimpleModel> GetPackagesByPackageType(string packageType)
        {
            var packages = null as IEnumerable<BeautyServicePackageSimpleModel>;
            try
            {
                packages = TuhuGrouponDbScopeManagerReadOnly.Execute(conn => handler.GetPackagesByPackageType(conn, packageType));
            }
            catch (Exception ex)
            {
                Logger.Error("GetPackagesByPackageType", ex);
            }
            return packages;
        }

        public IEnumerable<BeautyServicePackageDetailSimpleModel> GetProductsByPackageId(int packageId)
        {
            var products = null as IEnumerable<BeautyServicePackageDetailSimpleModel>;
            try
            {
                products = TuhuGrouponDbScopeManagerReadOnly.Execute(conn => handler.GetProductsByPackageId(conn, packageId));
            }
            catch (Exception ex)
            {
                Logger.Error("GetProductsByPackageId", ex);
            }
            return products;
        }

        public bool BatchAddBeautyCode(List<CreateBeautyCodeTaskModel> list, int mappindId, string type, string userName, string sha1)
        {
            var success = false;
            try
            {
                var batchCode = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                list.ForEach(x =>
                {
                    x.CreateUser = userName;
                    x.Type = "serviceCode";
                    x.MappingId = mappindId;
                    x.BatchCode = batchCode;
                    x.Status = "Created";
                });
                TuhuGrouponDbScopeManager.CreateTransaction(conn =>
                {
                    success = handler.BatchAddBeautyCode(conn, list);
                    if (success)
                    {
                        handler.UpdateBeautyCodeStatus(conn, batchCode, "Preparing");
                        SetUploadedFileSah1Cache(sha1);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error("BatchAddBeautyCode", ex);
            }
            return success;
        }
        /// <summary>
        /// 更新服务码任务状态
        /// </summary>
        /// <param name="batchCode"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateBeautyCodeTaskStatus(string batchCode, string status)
        {
            bool result = false;
            try
            {
                TuhuGrouponDbScopeManager.CreateTransaction(conn =>
                 {
                     handler.UpdateBeautyCodeStatus(conn, batchCode, status);
                 });
                result = true;
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateBeautyCodeTaskStatus", ex);
            }
            return result;
        }

        /// <summary>
        /// 根据服务码配置ids获取导入用户服务码统计信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="mappingIds"></param>
        /// <returns></returns>
        public Tuple<List<BeautyCodeStatistics>, int> GetBeautyCodeStatistics(int pageIndex, int pageSize, IEnumerable<int> mappingIds)
        {
            List<BeautyCodeStatistics> result = null ;
            int totalCount = 0;
            try
            {
                var batchCodesPageResult = TuhuGrouponDbScopeManagerReadOnly.Execute(conn => handler.SelectBatchCodesByMappingIds(conn, pageIndex, pageSize, mappingIds));
                if (batchCodesPageResult != null)
                {
                    var batchCodes = batchCodesPageResult.Item1;
                    totalCount = batchCodesPageResult.Item2;
                    result = TuhuGrouponDbScopeManagerReadOnly.Execute(conn => handler.GetBeautyCodeStatistics(conn, batchCodes));
                    var packageDetails = BeautyServicePackageManager.GetBeautyServicePackageDetails(mappingIds);
                    if (packageDetails != null && result != null)
                    {
                        var cooperateUsers = TuhuGrouponDbScopeManagerReadOnly.Execute(conn => BankMRActivityDal.SelectMrCooperateUserConfigs(conn, 1, 10000));
                        result.ForEach(s =>
                        {
                            var packageDetail = packageDetails.FirstOrDefault(t => t.PKID == s.MappingId);
                            if (packageDetail != null)
                            {
                                var currentCooperateUser = cooperateUsers?.Item1?.FirstOrDefault(u => u.PKID == packageDetail.CooperateId);
                                s.CooperateName = currentCooperateUser?.CooperateName;
                                s.ServiceName = packageDetail.Name;
                                s.SettlementMethod = packageDetail.SettlementMethod;
                                s.VipSettlementPrice = packageDetail.VipSettlementPrice;
                            }
                        });
                    }                   
                }              
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }

            return new Tuple<List<BeautyCodeStatistics>, int>(result, totalCount);
        }
        /// <summary>
        /// 根据批次号查询BeautyCodeStatistics
        /// </summary>
        /// <param name="batchCodes"></param>
        /// <returns></returns>
        public IEnumerable<BeautyCodeStatistics> GetBeautyCodeStatistics(IEnumerable<string> batchCodes)
        {
            IEnumerable<BeautyCodeStatistics> result = null;
			
            try
            {
                result = TuhuGrouponDbScopeManagerReadOnly.Execute(conn => handler.GetBeautyCodeStatistics(conn, batchCodes));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }

            return result;
        }

        private bool SetUploadedFileSah1Cache(string sha1)
        {
            if (!string.IsNullOrEmpty(sha1))
            {
                var key = sha1;
                using (var client = CacheHelper.CreateCacheClient("BeautyCodeClientName"))
                {
                    var cacheResult = client.Set(key, 1, TimeSpan.FromHours(1));
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
                    using (var client = CacheHelper.CreateCacheClient("BeautyCodeClientName"))
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
                Logger.Error(ex.Message, ex);
            }
            return true;
        }

        public async Task<string> SendBatchSms(string userEmail, IEnumerable<string> mobiles, int temmplateId, string[] templateArguments, DateTime? sendTime)
        {
            var result = string.Empty;
            var successResult = new List<int>();
            var failedResult = new List<int>();
            var batchSize = 999;
            for (var index = 0; index < (mobiles.Count() + batchSize - 1) / batchSize; index++)
            {
                var batchItem = mobiles.Skip(index * batchSize).Take(batchSize);
                var sentResult = await SmsManager.SendBatchSmsAsync(batchItem, temmplateId, templateArguments, null, null, null, sendTime);
                if (sentResult.Item1 <= 0)
                {
                    failedResult.Add(sentResult.Item1);
                    var mobilesStr = string.Empty;
                    foreach (var m in batchItem)
                    {
                        mobilesStr += $" {m},";
                    }
                    var emailUser = $"lutong@tuhu.cn;{userEmail}";
                    if (!string.Equals(_environment, "dev"))
                    {
                        emailUser = _companyMrRelatedUsers.Contains(userEmail) ? _companyMrRelatedUsers : $"{_companyMrRelatedUsers};{userEmail}";
                    }
                    TuhuMessage.SendEmail($"用户服务码通知短信发送失败", emailUser, $"{sentResult.Item2},发短信接口返回Code:{sentResult.Item1},失败手机号：{mobilesStr}");
                    ///短信发送失败
                }
                else
                {
                    successResult.Add(sentResult.Item1);
                }
            }

            return successResult.Any() && !failedResult.Any() ? nameof(SmsStatus.Success)
                : successResult.Any() && failedResult.Any() ? nameof(SmsStatus.PartialSuccess)
                : nameof(SmsStatus.Failed);
        }

        public IEnumerable<CompanyUserSmsRecord> GetCompanyUserSmsRecordByBatchCode(string batchCode)
        {
            IEnumerable<CompanyUserSmsRecord> result = null;
            try
            {
                result = TuhuGrouponDbScopeManagerReadOnly.Execute(conn => handler.SelectCompanyUserSmsRecordByBatchCode(conn, batchCode));
            }
            catch (Exception ex)
            {
                Logger.Error("GetProductsByPackageId", ex);
            }

            return result ?? new List<CompanyUserSmsRecord>();
        }

        public bool InsertCompanyUserSmsRecord(CompanyUserSmsRecord record)
        {
            bool result = false;

            try
            {
                result = TuhuGrouponDbScopeManager.Execute(conn => handler.InsertCompanyUserSmsRecord(conn, record));
            }
            catch (Exception ex)
            {
                Logger.Error("GetProductsByPackageId", ex);
            }

            return result;
        }
    }
}
