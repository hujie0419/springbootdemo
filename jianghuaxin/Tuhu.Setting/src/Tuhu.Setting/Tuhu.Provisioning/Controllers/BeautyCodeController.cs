using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Nosql;
using Tuhu.Provisioning.Business.BeautyCode;
using Tuhu.Provisioning.Business.BeautyService;
using Tuhu.Provisioning.Business.Redis;
using Tuhu.Provisioning.Business.Sms;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;
using static Tuhu.Provisioning.Business.Redis.FlashCounter;

namespace Tuhu.Provisioning.Controllers
{
    public class BeautyCodeController : Controller
    {
        private const string _smsUrl = "http://l.tuhu.cn/yvrU";
        private static string _environment = ConfigurationManager.AppSettings["env"];
        private static string _counterClient = "beautyCodeSetting";
        private static readonly Dictionary<int, string> _smsTemplates = new Dictionary<int, string>()
        {
            [214] = $"尊敬的客户，洗车卡券已添加到账户中，使用方法：点击短信链接-点击“登录查询我的服务码”-使用手机号登录-自动生成券码-点击券码旁边“查看门店”到店说明使用途虎并出示服务码即可完成服务。请登录链接 {_smsUrl} 途虎客服：400-111-8868",
            [215] = $"您的洗车券即将过期，请及时使用，使用方法：点击短信链接-点击“登录查询我的服务码”-使用手机号登录-自动生成券码-点击券码旁边“查看门店”到店说明使用途虎并出示服务码即可完成服务。请登录链接 {_smsUrl} 途虎客服：400-111-8868"
        };
        // GET: MDBeautyCode
        public ActionResult BatchImportBeautyCode()
        {
            return View();
        }


        public FileResult ExportBeautyCodeTemplate()
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var row = sheet.CreateRow(0);
            var cell = null as ICell;
            var cellNum = 0;

            row.CreateCell(cellNum++).SetCellValue("用户手机号");
            row.CreateCell(cellNum++).SetCellValue("数量");
            row.CreateCell(cellNum++).SetCellValue("有效期起始时间");
            row.CreateCell(cellNum++).SetCellValue("有效期截止时间");

            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 8 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);

            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"导入模板 {DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }


        public JsonResult GetPackagesByPackageType(string packageType)
        {
            var manager = new BeautyCodeManager();
            var packages = manager.GetPackagesByPackageType(packageType);
            return Json(new { Status = packages != null, Data = packages }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetProductsByPackageId(int packageId)
        {
            var manager = new BeautyCodeManager();
            var products = manager.GetProductsByPackageId(packageId);
            return Json(new { Status = products != null, Data = products }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据合作用户Id获取服务码配置
        /// </summary>
        /// <param name="cooperateId"></param>
        /// <returns></returns>
        public JsonResult GetBeautyServicePackageDetails(int cooperateId)
        {
            var result = BeautyServicePackageManager.GetBeautyServicePackageDetails(1, 1000, true, string.Empty, cooperateId, string.Empty);

            return Json(new { Data = result, Status = result != null }, JsonRequestBehavior.AllowGet);
        }

        public FileResult ExportPackageDetail()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 分页获取导入用户服务码统计信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="cooperateId"></param>
        /// <param name="packageDetailId"></param>
        /// <param name="settleMentMethod"></param>
        /// <returns></returns>
        public JsonResult GetBeautyCodeStatistics(int pageIndex, int pageSize, int cooperateId = 0, int packageDetailId = 0, string settleMentMethod = "")
        {
            var packageDetailIds = new List<int>();
            if (packageDetailId <= 0)
            {
                var packageDetails = BeautyServicePackageManager.GetBeautyServicePackageDetails(1, 10000, true, settleMentMethod, cooperateId, string.Empty);
                packageDetailIds = packageDetails.Item1.Select(t => t.PKID).Distinct().ToList();
            }
            else
            {
                packageDetailIds.Add(packageDetailId);
            }
            var manager = new BeautyCodeManager();
            var result = manager.GetBeautyCodeStatistics(pageIndex, pageSize, packageDetailIds);
            List<BeautyCodeStatistics> data = null;
            var totalCount = 0;
            data = result.Item1;
            totalCount = result.Item2;
            if (result != null && !string.IsNullOrEmpty(settleMentMethod))
            {
                data = data.Where(s => string.Equals(s.SettlementMethod, settleMentMethod)).ToList();
            }
            return Json(new { Status = data != null, Data = data, TotalCount= totalCount }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据批次号更新服务码时间
        /// </summary>
        /// <param name="batchCode"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [PowerManage]
        public async Task<JsonResult> UpdateCodeTimeByBatchCode(string batchCode, DateTime startTime, DateTime endTime)
        {
            var result = false;
            var msg = string.Empty;
            if (!string.IsNullOrEmpty(batchCode) && startTime != null && endTime != null)
            {
                var user = HttpContext.User.Identity.Name;
                var updateResult = await BeautyServicePackageManager.UpdateBeautyServiceCodeTimeByBatchCode(user, batchCode, startTime, endTime);
                result = updateResult.Item1;
                msg = updateResult.Item2;
            }

            return Json(new { Data = result, Status = result, Msg = msg }, JsonRequestBehavior.AllowGet);
        }

        [PowerManage]
        public JsonResult UploadExcel(int id, string type)
        {
            var files = Request.Files;
            var result = ConvertExcelToList(files);
            if (!string.IsNullOrEmpty(result.Item2))
            {
                return Json(new { Status = false, Msg = result.Item2 });
            }
            var list = result.Item1;
            var sha1Value = result.Item3;
            var manager = new BeautyCodeManager();
            var success = manager.BatchAddBeautyCode(list, id, type, User.Identity.Name, sha1Value);
            return Json(new { Status = success });
        }
        /// <summary>
        /// 回滚服务码
        /// </summary>
        /// <param name="batchCode"></param>
        /// <param name="channel"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        [PowerManage]
        public async Task<JsonResult> RevertServiceCode(string batchCode, string channel, string source)
        {
            var serviceCodeManager = new ServiceCodeManager();
            var user = User.Identity.Name;
            var result = await serviceCodeManager.RevertServiceCode(batchCode, channel, source, user);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
      


        public async Task<JsonResult> GetCompanyUserSmsRecords(string batchCode)
        {
            IEnumerable<CompanyUserSmsRecord> result = new List<CompanyUserSmsRecord>();

            if (!string.IsNullOrEmpty(batchCode))
            {
                var manager = new BeautyCodeManager();
                result = manager.GetCompanyUserSmsRecordByBatchCode(batchCode);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取短信模板
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetSmsTemplates()
        {
            var result = _smsTemplates.Select(s => new { Key = s.Key, Value = s.Value });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据批次号给批次下用户发服务码通知短信
        /// </summary>
        /// <param name="batchCode">批次号</param>
        /// <returns></returns>
        [PowerManage]
        [HttpPost]
        public async Task<JsonResult> SendSms(string batchCode, int templateId, DateTime? sendTime=null)
        {
            String sentResult = string.Empty;
            var resultMsg = string.Empty;
            if (!string.IsNullOrWhiteSpace(batchCode) && _smsTemplates.ContainsKey(templateId))
            {
                if (sendTime != null && sendTime < DateTime.Now)
                {
                    sentResult = "TimeNotAvailable";
                    resultMsg = "时间不能早于当前时间";
                }
                else
                {
                    var manager = new BeautyCodeManager();                
                    var key = $"SendSms/{batchCode}";
                    var ts = TimeSpan.FromMinutes(1);
                    long flashCount = await FlashCounter.GetFlashCount(_counterClient, key, ts, OperateType.Increment);
                    if (flashCount > 0)
                    {
                        resultMsg = "操作过于频繁,请等1分钟重试";
                        sentResult = "Frequently";
                    }
                    else
                    {
                        var isLimit = false;
                        if (!string.Equals(_environment, "dev"))//如果是生产环境有一周只能发一次的限制
                        {
                            var smsRecords = manager.GetCompanyUserSmsRecordByBatchCode(batchCode);
                            var latestSmsRecord = smsRecords.Where(s => string.Equals(s.Status, nameof(SmsStatus.Success))
                            || string.Equals(s.Status, nameof(SmsStatus.PartialSuccess))).OrderByDescending(s => s.PKID).FirstOrDefault();
                            if (latestSmsRecord != null)
                            {
                                isLimit = (DateTime.Now - latestSmsRecord.CreatedDateTime).Days < 7;
                            }
                        }
                        if (isLimit)
                        {
                            resultMsg = "一周之内只能发一次短信";
                            sentResult = "BeLimited";
                        }
                        else
                        {
                            var user = HttpContext.User.Identity.Name;
                            var codeTasks = await BeautyServicePackageManager.SelectCreateBeautyCodeTaskModels(batchCode);
                            if (codeTasks.Any())
                            {
                                var mobiles = codeTasks.Select(s => s.MobileNumber).Distinct();
                                if (mobiles.Any())
                                {
                                    string[] arg = new string[1];
                                    arg[0] = _smsUrl;
                                    var msg = _smsTemplates[templateId];
                                    sentResult = await manager.SendBatchSms(user, mobiles, templateId, arg, sendTime);
                                    var needSendTime = sendTime == null ? DateTime.Now : sendTime;                                
                                    manager.InsertCompanyUserSmsRecord(new CompanyUserSmsRecord()
                                    {
                                        Type = "ServiceCode",
                                        BatchCode = batchCode,
                                        SmsTemplateId = templateId,
                                        SmsMsg = msg,
                                        SentTime = (DateTime)needSendTime,
                                        Status = sentResult,
                                        CreatedUser = user
                                    });
                                }
                            }
                        }                       
                    }
                }            
            }
            else
            {
                resultMsg = "参数错误";
            }
            
            return Json(new { result = sentResult,msg= resultMsg });
        }

        #region utils

        private static Tuple<List<CreateBeautyCodeTaskModel>, string, string> ConvertExcelToList(HttpFileCollectionBase files)
        {
            var message = string.Empty;
            var result = null as List<CreateBeautyCodeTaskModel>;
            if (files.Count <= 0)
            {
                return Tuple.Create(result, "请先上传文件", string.Empty);
            }

            var file = files[0];
            if (file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return Tuple.Create(result, "请上传Excel文件", string.Empty);
            }

            var stream = file.InputStream;
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            var sha1Value = GetSha1Value(buffer);
            var manager = new BeautyCodeManager();
            if (manager.IsUploaded(sha1Value))
            {
                return Tuple.Create(result, "文件已经上传过，请不要重复上传", string.Empty);
            }

            var workBook = new XSSFWorkbook(new MemoryStream(buffer));
            var sheet = workBook.GetSheetAt(0);
            var temp = ConvertExcelToList(sheet);
            result = temp.Item1;
            message = temp.Item2;
            if (!string.IsNullOrEmpty(message))
            {
                return Tuple.Create(result, message, string.Empty);
            }
            if (!result.Any())
            {
                return Tuple.Create(result, "Excel内容为空", sha1Value);
            }
            //var repetitionNumber = result.GroupBy(x => x.MobileNumber)
            //    .Where(x => x.Count() > 1)
            //    .Select(x => x.Key).ToList();
            //if (repetitionNumber.Any())
            //{
            //    message = $"{string.Join(",", repetitionNumber)}以上手机号重复, 请确认";
            //}

            return Tuple.Create(result, message, sha1Value);
        }

        private static Tuple<List<CreateBeautyCodeTaskModel>, string> ConvertExcelToList(ISheet sheet)
        {
            var result = new List<CreateBeautyCodeTaskModel>();
            var message = string.Empty;

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

            var titleRow = sheet.GetRow(sheet.FirstRowNum);
            var index = titleRow.FirstCellNum;
            if (titleRow.GetCell(index++)?.StringCellValue == "用户手机号" &&
                titleRow.GetCell(index++)?.StringCellValue == "数量" &&
                titleRow.GetCell(index++)?.StringCellValue == "有效期起始时间" &&
                (titleRow.GetCell(index)?.StringCellValue == "有效期截止时间" || titleRow.GetCell(index)?.StringCellValue == "有效期截至时间"))
            {
                var regex = new Regex("^[0-9]{11}$");
                var minDateTime = new DateTime(1970, 1, 1, 0, 0, 0);
                var nowTime = DateTime.Now;
                for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;
                    var cellIndex = row.FirstCellNum;
                    var item = new CreateBeautyCodeTaskModel { };

                    var mobile = getStringValue(row.GetCell(cellIndex++));
                    var quantityStr = getStringValue(row.GetCell(cellIndex++));
                    var startTimeStr = getStringValue(row.GetCell(cellIndex++));
                    var endTimeStr = getStringValue(row.GetCell(cellIndex++));

                    if (string.IsNullOrWhiteSpace(mobile) && string.IsNullOrWhiteSpace(quantityStr)
                        && string.IsNullOrWhiteSpace(startTimeStr) && string.IsNullOrWhiteSpace(endTimeStr))
                    {
                        continue;
                    }

                    item.MobileNumber = mobile?.Trim();

                    int quantity;
                    item.Quantity = int.TryParse(quantityStr, out quantity) ? quantity : (int?)null;

                    DateTime time;
                    item.StartTime = DateTime.TryParse(startTimeStr, out time) ? time : (DateTime?)null;
                    item.EndTime = DateTime.TryParse(endTimeStr, out time) ? time : (DateTime?)null;

                    time = item.EndTime.GetValueOrDefault();
                    item.EndTime = time.Date.AddDays(1).AddSeconds(-1);

                    time = item.StartTime.GetValueOrDefault();
                    item.StartTime = time.Date;

                    if (item.MobileNumber != null && regex.IsMatch(item.MobileNumber) &&
                        item.Quantity != null && item.Quantity > 0 &&
                        item.StartTime != null && item.StartTime >= minDateTime && item.StartTime <= DateTime.MaxValue &&
                        item.EndTime != null && item.EndTime > item.StartTime && item.EndTime > nowTime && item.EndTime <= DateTime.MaxValue)
                    {
                        result.Add(item);
                    }
                    else
                    {
                        message = $"第{rowIndex + 1}行格式不正确, 必须填写手机号、手机号格式必须正确、数量大于0、时间范围1970-01-01 00:00:00 ~ 9999-12-31 23:59:59, 并且截至时间必须大于当前时间";
                        break;
                    }
                }
            }
            else
            {
                message = "与导入模板不一致，请下载模板之后根据模板填写";
            }
            return Tuple.Create(result, message);
        }

        private static string GetSha1Value(byte[] buffer)
        {
            using (var sha1 = new System.Security.Cryptography.SHA1Managed())
            {
                //return BitConverter.ToString(sha1.ComputeHash(file)).Replace("-", "");
                var hashcode = sha1.ComputeHash(buffer);
                var sb = new System.Text.StringBuilder(2 * hashcode.Length);
                foreach (byte b in hashcode)
                {
                    sb.AppendFormat("{0:X2}", b);
                }
                return sb.ToString();
            }
        }

        #endregion

    }
}