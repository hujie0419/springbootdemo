using Common.Logging;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.Service.Utility;
using Tuhu.C.YunYing.WinService.JobSchedulerService.DAL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.DLL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model.Enum;
using Tuhu.Service.Member.Request;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BLL
{
    public class VipPaintPackageBusiness
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(VipPaintPackageBusiness));

        private static readonly string connectionRw = ConfigurationManager.ConnectionStrings["Gungnir"]?.ConnectionString;

        private static string FileDoMain = ConfigurationManager.AppSettings["FileDoMain"];

        /// <summary>
        /// 大客户喷漆塞券
        /// </summary>
        /// <param name="details"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public static bool CreateVipPaintPackagePromotions(List<UploadDetails> details, UploadFileTask task)
        {
            var result = false;
            try
            {
                var record = DALVipPaintPackage.SelectPaintPackagePromotionInfoByBatchCode(task.BatchCode);
                var package = DALVipPaintPackage.GetVipPaintPackageConfig(task.BatchCode);
                if (record != null && package != null && package.PKID > 0)
                {
                    if (task.Status == FileStatus.Wait)//第一次加载 将券码对照插入表中
                    {
                        DALVipPaintPackage.BatchPaintPakckagePromotion(details, record.BatchCode, package.PKID);
                    }
                    var data = DALVipPaintPackage.SelectNoSuccessPromotionDetailsByBatchCode(record.BatchCode);//待塞券记录
                    if (data != null && data.Any())
                    {
                        data.ForEach(item => CreatePromotionToUser(item, record));
                        var validatedData = DALVipPaintPackage.SelectNoSuccessPromotionDetailsByBatchCode(record.BatchCode);
                        if (validatedData.Any())
                        {
                            DalUploadFileTask.UpdateFileTaskStatus(task, FileStatus.WaitingForRepair);
                        }
                        else
                        {
                            var success = true;
                            #region 买断制套餐全部塞券完成后创建ToB订单
                            if (string.Equals(record.SettlementMethod, SettlementMethod.PreSettled.ToString(), StringComparison.OrdinalIgnoreCase))
                            {
                                System.Threading.Thread.Sleep(3000);//读写库同步延迟
                                success = CreateToBOrder(record);
                            }
                            #endregion
                            if (success)
                            {
                                result = DalUploadFileTask.UpdateFileTaskStatus(task, FileStatus.Success);
                            }
                        }
                    }
                    else
                    {
                        var success = true;
                        #region 买断制套餐全部塞券完成后创建ToB订单
                        if (string.Equals(record.SettlementMethod, SettlementMethod.PreSettled.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            success = CreateToBOrder(record);
                        }
                        #endregion
                        if (success)
                        {
                            result = DalUploadFileTask.UpdateFileTaskStatus(task, FileStatus.Success);
                        }
                    }
                }
                else
                {
                    DalUploadFileTask.SetErrrorFileStatus(task, FileStatus.Cancel, $"找不到批次号{task.BatchCode}对应的套餐");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CreateVipPaintPackagePromotions", ex);
                DalUploadFileTask.SetErrrorFileStatus(task, FileStatus.Cancel, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 给用户塞券
        /// </summary>
        /// <param name="item"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public static bool CreatePromotionToUser(VipPaintPackagePromotionDetail item, VipPaintPackagePromotionRecord record)
        {
            var success = false;
            try
            {
                var userId = VipBaoYangPackageBusiness.GetUserId(item.MobileNumber);
                if (userId != Guid.Empty)
                {
                    using (var dbHelper = DbHelper.CreateDbHelper(connectionRw))
                    {
                        dbHelper.BeginTransaction();
                        DALVipPaintPackage.UpdatePaintPackagePromotionStatus(dbHelper, record.BatchCode, item.PKID, Status.Running);
                        var createResult = CreatePromotionNew(record.RuleGUID, userId, record.CreateUser);
                        if (createResult.IsSuccess && createResult.PromotionId > 0)
                        {
                            item.PromotionId = createResult.PromotionId;
                            success = DALVipPaintPackage.UpdatePaintPackagePromotionToSuccess(dbHelper, item);
                            if (success)
                            {
                                if (record.IsSendSms)
                                {
                                    SendVipPaintPackageSms(item.MobileNumber, item.CarNo);
                                }
                            }
                            else
                            {
                                var invalid = InvalidPromotion(createResult.PromotionId);
                                if (invalid)
                                {
                                    Logger.Info($"创建喷漆大客户优惠券{createResult.PromotionId}成功但更新塞券详情状态失败,因此作废券");
                                }
                                else
                                {
                                    Logger.Info($"更新塞券详情状态失败,作废券{createResult.PromotionId}失败");
                                }
                            }
                        }
                        else
                        {
                            var message = (createResult?.ErrorMessage?.Contains("已领取过") ?? false) ?
                                "超过优惠券每人限领数量" :
                                createResult?.ErrorMessage;
                            success = DALVipPaintPackage.UpdatePaintPackagePromotionStatus(dbHelper, record.BatchCode,
                                item.PKID, Status.FAIL, message);
                        }
                        if (!success)
                        {
                            dbHelper.Rollback();
                        }
                        else
                        {
                            dbHelper.Commit();
                        }
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

        /// <summary>
        /// 解析excel
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static List<UploadDetails> ConvertExcelToList(byte[] buffer, UploadFileTask task)
        {
            List<UploadDetails> result = new List<UploadDetails>();
            try
            {
                DalUploadFileTask.UpdateFileTaskStatus(task, FileStatus.Runing);
                var workBook = WorkbookFactory.Create(new MemoryStream(buffer));
                var sheet = workBook.GetSheetAt(0);
                var titleRow = sheet.GetRow(sheet.FirstRowNum);
                var index = titleRow.FirstCellNum;
                string getStringValue(ICell cell)
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
                }
                for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;
                    var cellIndex = row.FirstCellNum;
                    var mobile = getStringValue(row.GetCell(cellIndex++))?.Trim();
                    var carNo = getStringValue(row.GetCell(cellIndex++))?.Trim();
                    var quantityStr = getStringValue(row.GetCell(cellIndex++))?.Trim();
                    if (!string.IsNullOrEmpty(mobile) && mobile.Length <= 11 && !string.IsNullOrEmpty(carNo) && carNo.Length <= 20)
                    {
                        if (string.IsNullOrEmpty(quantityStr))
                        {
                            result.Add(new UploadDetails { Carno = carNo, MobileNumber = mobile });
                        }
                        else if (int.TryParse(quantityStr, out int quantity) && quantity > 0)
                        {
                            result.AddRange(Enumerable.Range(0, quantity).Select(x => new UploadDetails
                            {
                                Carno = carNo,
                                MobileNumber = mobile
                            }));
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                DalUploadFileTask.SetErrrorFileStatus(task, FileStatus.Cancel, ex.Message);
                Logger.Error($"ConvertExcelToList失败,当前文件{JsonConvert.SerializeObject(task)}", ex);
            }
            return result;
        }

        /// <summary>
        /// 给用户塞券
        /// </summary>
        /// <param name="ruleGuid"></param>
        /// <param name="userId"></param>
        /// <param name="operatorUser"></param>
        /// <returns></returns>
        private static CreatePromotionCodeResult CreatePromotionNew(Guid ruleGuid, Guid userId, string operatorUser)
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
                    Operation = "喷漆大客户套餐塞券",
                    Issuer = operatorUser
                };
                using (var client = new PromotionClient())
                {
                    var getResult = client.CreatePromotionNew(promotion);
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("喷漆大客户套餐给用户塞券失败", ex);
            }
            return result ?? new CreatePromotionCodeResult();
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="cellphone"></param>
        /// <param name="carNo"></param>
        /// <returns></returns>
        private static bool SendVipPaintPackageSms(string cellphone, string carNo)
        {
            try
            {
                using (var client = new SmsClient())
                {
                    var result = client.SendSms(cellphone, 196, carNo, "喷漆优惠", cellphone, "集团客户", "喷漆", "喷漆");
                    result.ThrowIfException(true);
                    return result.Result > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"喷漆大客户套餐发送短信失败:{cellphone}/{carNo}", ex);
                return false;
            }
        }

        /// <summary>
        /// 创建买断制订单
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public static bool CreateToBOrder(VipPaintPackagePromotionRecord model)
        {
            bool success = false;
            var package = DALVipPaintPackage.GetVipPaintPackageConfig(model.BatchCode);
            var total = DALVipPaintPackage.SelectPromotionDetailTotal(model.BatchCode);
            var count = DALVipPaintPackage.SelectPromotionDetailSuccessCount(model.BatchCode);
            if (package != null && count > 0 && total == count)
            {
                var result = VipPromotionOrderBusiness.CreatePaintBuyoutOrder(package, count, model.BatchCode);
                if (result != null && result.OrderId > 0)
                {
                    success = DALVipPaintPackage.UpdatePromotionDetailToBOrder(model.BatchCode, result.OrderId.ToString());
                    if (success)
                    {
                        Logger.Info($"创建买断制2B订单{result.OrderId}成功, 一共{count}个产品数量, 关联批次号:{model.BatchCode}");
                    }
                    else
                    {
                        var cancel = VipPromotionOrderBusiness.CancelOrder(result.OrderId, "关联买断订单到大客户喷漆塞券记录失败");
                        Logger.Info($"更新塞券批次{model.BatchCode}对应买断制2B订单失败," +
                            $"取消订单{result.OrderId}{(cancel ? "成功" : "失败")}");
                    }
                }
                else
                {
                    Logger.Info($"创建买断制2B订单失败, 一共{count}个产品数量, 批次号:{model.BatchCode}");
                }
            }
            return success;
        }

        /// <summary>
        /// 塞券成功但更新塞券状态失败则作废优惠券
        /// </summary>
        /// <param name="promotionId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool InvalidPromotion(int promotionId)
        {
            var result = false;
            using (var client = new PromotionClient())
            {
                var serviceResult = client.FetchPromotionCodeByID(new FetchPromotionCodeRequest()
                {
                    PKID = promotionId
                });
                if (serviceResult.Success)
                {
                    var promotion = serviceResult.Result;
                    var updateResult = client.UpdateUserPromotionCodeStatus(new UpdateUserPromotionCodeStatusRequest()
                    {
                        Status = 3,
                        PromotionCodeId = promotion.Pkid,
                        OperationAuthor = "YunYingJob",
                        Channel = "YunYingJob",
                        UserID = Guid.Parse(promotion.UserId)
                    });
                    if (updateResult.Exception != null)
                    {
                        Logger.Error("InvalidPromotion", updateResult.Exception);
                    }
                    result = updateResult.Success && updateResult.Result;
                }
            }
            return result;
        }
    }
}
