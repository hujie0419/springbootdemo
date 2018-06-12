using Common.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Enums;
using Tuhu.Service.UserAccount.Models;
using Tuhu.Service.Utility;
using Tuhu.C.YunYing.WinService.JobSchedulerService.DLL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model.Enum;
using Tuhu.C.YunYing.WinService.JobSchedulerService.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BLL
{
    public class VipBaoYangPackageBusiness
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(VipBaoYangPackageBusiness));

        private static readonly string connectionRw = ConfigurationManager.ConnectionStrings["Gungnir"]?.ConnectionString;

        private static string FileDoMain = ConfigurationManager.AppSettings["FileDoMain"];

        public static bool CreateVipBaoYngPackagePromotions(List<UploadDetails> details, UploadFileTask task)
        {
            var result = false;
            try
            {
                var record = DALVipBaoYangPackage.SelectBaoYangPackagePromotionInfoByBatchCode(task.BatchCode);
                if (record != null)
                {
                    if (task.Status == FileStatus.Wait)
                    {
                        var success = DalUploadFileTask.UpdateFileTaskStatus(task, FileStatus.Loaded);
                        if (success)
                        {
                            DALVipBaoYangPackage.BatchBaoYangPakckagePromotion(details, task.BatchCode);
                        }
                    }
                    var data = DALVipBaoYangPackage.SelectNoSuccessPromotionDetailsByBatchCode(task.BatchCode);
                    if (data != null && data.Any())
                    {
                        data.ForEach(item => CreatePromotionToUser(item, record));
                        var validatedData = DALVipBaoYangPackage.SelectNoSuccessPromotionDetailsByBatchCode(task.BatchCode);
                        if (validatedData.Any())
                        {
                            DalUploadFileTask.UpdateFileTaskStatus(task, FileStatus.WaitingForRepair);
                        }
                        else
                        {
                            var success = true;
                            if (string.Equals(record.SettlementMethod, SettlementMethod.PreSettled.ToString(), StringComparison.OrdinalIgnoreCase))
                            {
                                System.Threading.Thread.Sleep(3000);
                                success = VipPromotionOrderBusiness.CreateBaoYangBuyoutOrder(record.BatchCode);
                            }
                            if (success)
                            {
                                DalUploadFileTask.UpdateFileTaskStatus(task, FileStatus.Success);
                            }
                        }
                    }
                    else
                    {
                        var success = true;
                        if (string.Equals(record.SettlementMethod, SettlementMethod.PreSettled.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            success = VipPromotionOrderBusiness.CreateBaoYangBuyoutOrder(record.BatchCode);
                        }
                        if (success)
                        {
                            DalUploadFileTask.UpdateFileTaskStatus(task, FileStatus.Success);
                        }
                    }
                }
                else
                {
                    DalUploadFileTask.UpdateFileTaskStatus(task, FileStatus.Cancel);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CreateVipBaoYngPackagePromotions", ex);
            }

            return result;
        }

        public static bool CreatePromotionToUser(BaoYangPackagePromotionDetail item, BaoYangPackagePromotionRecord record)
        {
            var success = false;
            try
            {
                var userId = GetUserId(item.MobileNumber);
                if (userId != Guid.Empty)
                {
                    using (var dbHelper = DbHelper.CreateDbHelper(connectionRw))
                    {
                        dbHelper.BeginTransaction();
                        DALVipBaoYangPackage.UpdateBaoYangPackagePromotionStatus(dbHelper, record.BatchCode, item.PKID, Status.Running);
                        var createResult = CreatePromotionNew(record.RulesGUID, userId, record.CreateUser, item.StartTime, item.EndTime);
                        if (createResult.IsSuccess && createResult.PromotionId > 0)
                        {
                            if (record.IsSendSms)
                                SendSms(item.MobileNumber, item.Carno);
                            success = DALVipBaoYangPackage.UpdateBaoYangPackagePromotionToSuccess(dbHelper, record.BatchCode,
                                item.PKID, createResult.PromotionId);
                        }
                        else
                        {
                            var message = (createResult?.ErrorMessage?.Contains("已领取过") ?? false) ?
                                "超过优惠券每人限领数量" :
                                createResult?.ErrorMessage;
                            success = DALVipBaoYangPackage.UpdateBaoYangPackagePromotionStatus(dbHelper, record.BatchCode,
                                item.PKID, Status.FAIL, message);
                        }
                        if (!success)
                            dbHelper.Rollback();
                        else
                            dbHelper.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CreatePromotionToUser", ex);
                success = false;
            }
            return success;
        }

        public static Guid GetUserId(string mobile)
        {
            using (var client = new UserAccountClient())
            {
                var serviceResult = client.GetUserByMobile(mobile);
                if (!serviceResult.Success)
                {
                    serviceResult.ThrowIfException(true);
                }
                var user = serviceResult.Result;
                if (user == null || user.UserId == Guid.Empty)
                {
                    serviceResult = client.CreateUserRequest(new CreateUserRequest
                    {
                        MobileNumber = mobile,
                        ChannelIn = nameof(ChannelIn.Import),
                        UserCategoryIn = nameof(UserCategoryIn.Tuhu)
                    });
                    if (!serviceResult.Success)
                    {
                        serviceResult.ThrowIfException(true);
                    }
                    user = serviceResult.Result;
                }
                return user == null ? Guid.Empty : user.UserId;
            }
        }


        public static List<UploadDetails> LoadUploadBaoYangPromotionsFile(byte[] buffer)
        {
            List<UploadDetails> result = new List<UploadDetails>();
            try
            {
                var workBook = new XSSFWorkbook(new MemoryStream(buffer));
                var sheet = workBook.GetSheetAt(0);
                var titleRow = sheet.GetRow(sheet.FirstRowNum);
                var index = titleRow.FirstCellNum;
                Func<ICell, string> getStringValue = cell =>
                {
                    if (cell != null)
                    {
                        if (cell.CellType == CellType.Numeric)
                        {
                            return DateUtil.IsCellDateFormatted(cell) ?
                                cell.DateCellValue.ToString("yyyy-MM-dd HH:mm:ss.fff") :
                                cell.NumericCellValue.ToString();
                        }
                        return cell.StringCellValue;
                    }
                    return null;
                };
                for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;
                    var cellIndex = row.FirstCellNum;
                    var mobile = getStringValue(row.GetCell(cellIndex++))?.Trim();
                    var carNo = getStringValue(row.GetCell(cellIndex++))?.Trim();
                    var quantityStr = getStringValue(row.GetCell(cellIndex++))?.Trim();

                    var startTime = getStringValue(row.GetCell(cellIndex++))?.Trim();
                    var endTime = getStringValue(row.GetCell(cellIndex++))?.Trim();
                    DateTime? _startTime = null;
                    DateTime? _endTime = null;

                    if (!string.IsNullOrWhiteSpace(startTime) && !string.IsNullOrWhiteSpace(endTime))
                    {
                        _startTime = DateTime.Parse(startTime);
                        _endTime = DateTime.Parse(endTime);
                    }

                    int quantity;
                    if (!string.IsNullOrEmpty(mobile) && !string.IsNullOrEmpty(carNo))
                    {
                        if (string.IsNullOrEmpty(quantityStr))
                        {
                            result.Add(new UploadDetails { Carno = carNo, MobileNumber = mobile, StartTime = _startTime, EndTime = _endTime });
                        }
                        else if (int.TryParse(quantityStr, out quantity) && quantity > 0)
                        {
                            result.AddRange(Enumerable.Range(0, quantity).Select(x => new UploadDetails
                            {
                                Carno = carNo,
                                MobileNumber = mobile,
                                StartTime = _startTime,
                                EndTime = _endTime
                            }));
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                Logger.Error("加载大客户保养文件异常", ex);
            }
            return result;
        }

        public static CreatePromotionCodeResult CreatePromotionNew(Guid ruleGuid, Guid userId, string operatorUser, DateTime? startTime, DateTime? endTime)
        {
            CreatePromotionCodeResult result = null;
            try
            {
                CreatePromotionModel promotion = new CreatePromotionModel()
                {
                    Author = operatorUser,
                    Channel = "YunYingJob",
                    GetRuleGUID = ruleGuid,
                    UserID = userId,
                    Operation = "大客户保养套餐塞券",
                    Issuer = operatorUser,
                    IsDiscountFromOut = true
                };
                #region 查询优惠券领取规则并校验
                using (var client = new PromotionClient())
                {
                    var getCouponRule = client.GetCouponRulesByRuleGuids(new[] { ruleGuid });
                    if (!getCouponRule.Success)
                    {
                        return new CreatePromotionCodeResult() { ErrorMessage = getCouponRule.ErrorMessage };
                    }
                    var getCouponRuleModel = getCouponRule.Result.FirstOrDefault();
                    if (getCouponRuleModel == null)
                    {
                        return new CreatePromotionCodeResult() { ErrorMessage = $"{ ruleGuid }领取规则不存在！ " };
                    }
                    if (getCouponRuleModel.Quantity > 0)
                    {
                        if (getCouponRuleModel.GetQuantity + 1 > getCouponRuleModel.Quantity)
                        {
                            return new CreatePromotionCodeResult() { ErrorMessage = $"已抢光！ " };
                        }
                    }
                    //var promotionHistory = client.SelectHistoryPromotionCodeByGetRuleIds(userId,
                    //    new[] { getCouponRuleModel.PKID },
                    //    DateTime.Now.AddMonths(-12),
                    //    DateTime.Now);
                    var hasGet = DALVipBaoYangPackage.GetUserHasGetPromotionCount(userId, getCouponRuleModel.PKID);
                    if (hasGet + 1 > getCouponRuleModel.SingleQuantity)
                    {
                        return new CreatePromotionCodeResult() { ErrorMessage = $"已领取过！ " };
                    }
                    promotion.Discount = getCouponRuleModel.Discount;
                    promotion.MinMoney = getCouponRuleModel.MinMoney;
                    promotion.StartTime = (startTime ?? getCouponRuleModel.ValiStartDate) ?? default(DateTime);
                    promotion.EndTime = (endTime ?? getCouponRuleModel.ValiEndDate) ?? default(DateTime);
                }
                #endregion
                using (var client = new PromotionClient())
                {
                    var getResult = client.CreatePromotionNew(promotion);
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CreatePromotionNew", ex);
            }
            return result ?? new CreatePromotionCodeResult();
        }

        private static bool SendSms(string cellphone, string carNo)
        {
            SmsClient client = null;
            try
            {
                client = new SmsClient();

                var result = client.SendSms(cellphone, 149, carNo, cellphone);

                result.ThrowIfException(true);

                return result.Result > 0;
            }
            catch (Exception ex)
            {
                Logger.Error($"SendSms:{cellphone}/{carNo}", ex);
                return false;
            }
            finally
            {
                client?.Dispose();
            }
        }
    }
}
