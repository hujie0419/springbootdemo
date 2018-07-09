using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.UploadFile;
using Tuhu.Provisioning.Business.VipBaoYangPackage;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.VipBaoYangPackage;

namespace Tuhu.Provisioning.Controllers
{
    public class VipBaoYangPackageController : Controller
    {
        private VipBaoYangPackageManager manager;
        public VipBaoYangPackageController()
        {
            this.manager = new VipBaoYangPackageManager();
        }

        [PowerManage]
        public ActionResult VipBaoYangPackage()
        {
            return View();
        }

        [PowerManage]
        public ActionResult VipPromotionOperation()
        {
            return View();
        }

        [PowerManage]
        public ActionResult PromotionOperationRecord()
        {
            return View();
        }

        public ActionResult VipBaoYangPackageSmsConfig()
        {
            return View();
        }

        public JsonResult SelectVipSmsConfig(Guid vipUserId, int pageIndex = 1, int pageSize = 5)
        {
            var manager = new VipBaoYangPackageManager();
            var result = manager.SelectVipSmsConfig(vipUserId, pageIndex, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateSendSmsStatus(Guid vipUserId, bool isSendSms)
        {
            var manager = new VipBaoYangPackageManager();
            var result = manager.UpdateSendSmsStatus(vipUserId, isSendSms, HttpContext.User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectVipBaoYangPackage(string pid, Guid vipUserId, int pageIndex, int pageSize)
        {
            var manager = new VipBaoYangPackageManager();
            var result = manager.SelectVipBaoYangPackage(pid, vipUserId, pageIndex, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBaoYangPackageNameByVipUserId(Guid vipUserId)
        {
            var manager = new VipBaoYangPackageManager();
            var result = manager.GetBaoYangPackageNameByVipUserId(vipUserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertVipBaoYangPackage(VipBaoYangPackageViewModel model)
        {
            Func<string> validFunc = () =>
            {
                if (model == null)
                {
                    return "参数不能为空!";
                }
                if (string.IsNullOrWhiteSpace(model.PackageName))
                {
                    return "套餐名称不能为空";
                }
                SettlementMethod method;
                if (!Enum.TryParse(model.SettlementMethod, out method))
                {
                    return "结算方式不存在";
                }
                var gradesTmpl = new List<int> { 1, 2, 3 };
                model.Brands = model.Brands?.Select(brand => new BaoYangPackageOilBrand
                {
                    Brand = brand.Brand,
                    Grades = brand.Grades?.Where(x => gradesTmpl.Contains(x)).ToList() ?? new List<int>(),
                }).Where(brand => brand.Grades.Any()).ToList();
                if (model.Brands == null || !model.Brands.Any())
                {
                    return "品牌不能为空";
                }
                var brands = model.Brands.Select(x => x.Brand).ToList();
                if (brands.Distinct().Count() < brands.Count)
                {
                    return "机油品牌重复";
                }
                model.SettlementMethod = method.ToString();
                model.PackageName = model.PackageName.Trim();
                if (manager.IsExistsPackageName(model.PackageName, model.PKID))
                {
                    return "套餐名称已存在";
                }
                return string.Empty;
            };

            var validResult = validFunc();
            if (!string.IsNullOrEmpty(validResult))
            {
                return Json(new { status = false, msg = validResult });
            }
            model.CreateUser = User.Identity.Name;
            var result = false;
            if (model.PKID <= 0)
            {
                result = manager.InsertVipBaoYangPackage(model);
            }
            else if (User.Identity.Name == "wangminyou@tuhu.cn" ||
                User.Identity.Name == "zhangjianfeng@tuhu.cn" ||
                User.Identity.Name == "xieshuquan@tuhu.cn" ||
                User.Identity.Name == "devteam@tuhu.work" ||
                User.Identity.Name == "testteam@tuhu.work")
            {
                result = manager.UpdateVipBaoYangPackage(model);
            }
            return Json(new { status = result });
        }

        public JsonResult SelectOperationRecord(string batchCode, string pid, Guid vipUserId, string mobilePhone, int pageIndex, int pageSize)
        {
            VipBaoYangPackageManager manager = new VipBaoYangPackageManager();
            var result = manager.SelectPromotionOperationRecord(pid, vipUserId, 0, batchCode, mobilePhone, pageIndex, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectVipBaoYangPackageByPKID(int pkid)
        {
            VipBaoYangPackageManager manager = new VipBaoYangPackageManager();
            var result = manager.SelectVipBaoYangPackageByPkid(pkid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCouponRules()
        {
            VipBaoYangPackageManager manager = new VipBaoYangPackageManager();
            var result = manager.GetCouponRules();
            return Json(result.Select(x => new
            {
                PKID = x.Item1,
                RuleGUID = x.Item2,
                PromotionName = x.Item3,
                Description = x.Item4
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public FileResult ExportVipBaoYangPackageTemplate()
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var row = sheet.CreateRow(0);
            var cell = null as ICell;
            var cellNum = 0;

            row.CreateCell(cellNum++).SetCellValue("手机号");
            row.CreateCell(cellNum++).SetCellValue("车牌号");
            row.CreateCell(cellNum++).SetCellValue("数量");
            row.CreateCell(cellNum++).SetCellValue("有效期起始时间(yyyy/MM/dd)");
            row.CreateCell(cellNum++).SetCellValue("有效期截止时间(yyyy/MM/dd)");

            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);

            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"导入模板 {DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        public JsonResult UploadExcel(int packageId, Guid ruleGUID)
        {
            var files = Request.Files;
            var convertResult = ConvertExcelToList(files);
            var data = new List<BaoYangPackagePromotionDetail>();
            var flag = false;
            if (!string.IsNullOrEmpty(convertResult.Item2))
            {
                return Json(new { status = false, msg = convertResult.Item2 });
            }

            var manager = new VipBaoYangPackageManager();

            var validated = manager.ValidatedUploadData(ruleGUID, convertResult.Item1);
            if (!validated.Item1)
            {
                return Json(new { status = false, msg = validated.Item2 });
            }

            var extension = Path.GetExtension(files[0].FileName);
            var uploadResult = new UploadFileManager().UploadFile(convertResult.Item4, FileType.VipBaoYangPackage, extension, files[0].FileName, HttpContext.User.Identity.Name);
            if (!string.IsNullOrEmpty(uploadResult.Item1) && !string.IsNullOrEmpty(uploadResult.Item2))
            {
                flag = manager.BatchBaoYangPakckagePromotion(packageId, ruleGUID, convertResult.Item3, uploadResult.Item2, HttpContext.User.Identity.Name);
                if (flag)
                    data = convertResult.Item1;
            }

            return Json(new { status = flag, data = data });
        }

        public JsonResult GetBaoYangPackageConfigUser()
        {
            var manager = new VipBaoYangPackageManager();
            var result = manager.GetBaoYangPackageConfigUser();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllBaoYangPackageUser()
        {
            var manager = new VipBaoYangPackageManager();
            var result = manager.GetAllBaoYangPackageUser();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOilBrands()
        {
            BaoYangManager manager = new BaoYangManager();
            var result = manager.SelectAllFuelBrand().Item2;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateFileTaskStatus(string batchCode)
        {
            var manager = new UploadFileManager();
            var result = manager.UpdateFileTaskStatus(batchCode, FileType.VipBaoYangPackage, FileStatus.Repaired, FileStatus.WaitingForRepair);
            return Json(new { status = result });
        }

        public FileResult ExportCreatePromotionRecord(int packageId, string batchCode)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var row = sheet.CreateRow(0);
            var cell = null as ICell;
            var cellNum = 0;

            row.CreateCell(cellNum++).SetCellValue("批次号");
            row.CreateCell(cellNum++).SetCellValue("是否成功");
            row.CreateCell(cellNum++).SetCellValue("手机号");
            row.CreateCell(cellNum++).SetCellValue("车牌号");
            row.CreateCell(cellNum++).SetCellValue("所属大客户");
            row.CreateCell(cellNum++).SetCellValue("套餐PID");
            row.CreateCell(cellNum++).SetCellValue("套餐名称");
            row.CreateCell(cellNum++).SetCellValue("机油升数限制");
            row.CreateCell(cellNum++).SetCellValue("塞券人");
            row.CreateCell(cellNum++).SetCellValue("塞券时间");
            row.CreateCell(cellNum++).SetCellValue("短信配置");
            row.CreateCell(cellNum++).SetCellValue("备注");

            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);

            var manager = new VipBaoYangPackageManager();
            var packageInfo = manager.SelectPromotionOperationRecord(string.Empty, Guid.Empty, packageId, batchCode, string.Empty, 1, 10).FirstOrDefault();
            var promotionRecordInfo = manager.SelectPromotionDetailsByBatchCode(batchCode);
            if (promotionRecordInfo != null && promotionRecordInfo.Any())
            {
                for (var i = 0; i < promotionRecordInfo.Count; i++)
                {
                    cellNum = 0;
                    NPOI.SS.UserModel.IRow rowtemp = sheet.CreateRow(i + 1);
                    rowtemp.CreateCell(cellNum++).SetCellValue(promotionRecordInfo[i].BatchCode);
                    rowtemp.CreateCell(cellNum++).SetCellValue(promotionRecordInfo[i].Status == Status.SUCCESS ? "成功" : "失败");
                    rowtemp.CreateCell(cellNum++).SetCellValue(promotionRecordInfo[i].MobileNumber);
                    rowtemp.CreateCell(cellNum++).SetCellValue(promotionRecordInfo[i].Carno);
                    rowtemp.CreateCell(cellNum++).SetCellValue(packageInfo.VipUserName);
                    rowtemp.CreateCell(cellNum++).SetCellValue(packageInfo.PID);
                    rowtemp.CreateCell(cellNum++).SetCellValue(packageInfo.PackageName);
                    rowtemp.CreateCell(cellNum++).SetCellValue(packageInfo.Volume);
                    rowtemp.CreateCell(cellNum++).SetCellValue(packageInfo.CreateUser);
                    rowtemp.CreateCell(cellNum++).SetCellValue(packageInfo.CreateDateTime.ToString());
                    rowtemp.CreateCell(cellNum++).SetCellValue(packageInfo.IsSendSms ? "发送" : "不发送");
                    rowtemp.CreateCell(cellNum++).SetCellValue(promotionRecordInfo[i].Remarks ?? string.Empty);
                }
            }

            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"{batchCode}{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        private static Tuple<List<BaoYangPackagePromotionDetail>, string, string, byte[]> ConvertExcelToList(HttpFileCollectionBase files)
        {
            var message = string.Empty;
            var result = null as List<BaoYangPackagePromotionDetail>;
            byte[] outBuffer = null;
            if (files.Count <= 0)
            {
                return Tuple.Create(result, "请先上传文件", string.Empty, new byte[0]);
            }

            var file = files[0];
            if (file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return Tuple.Create(result, "请上传Excel文件", string.Empty, new byte[0]);
            }

            var stream = file.InputStream;
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            outBuffer = buffer;
            var sha1Value = GetSha1Value(buffer);
            var manager = new VipBaoYangPackageManager();
            if (manager.IsUploaded(sha1Value))
            {
                return Tuple.Create(result, "文件已经上传过，请不要重复上传", string.Empty, new byte[0]);
            }

            var workBook = new XSSFWorkbook(new MemoryStream(buffer));
            var sheet = workBook.GetSheetAt(0);
            var temp = ConvertExcelToList(sheet);
            result = temp.Item1;
            message = temp.Item2;
            if (!string.IsNullOrEmpty(message))
            {
                return Tuple.Create(result, message, string.Empty, new byte[0]);
            }
            if (!result.Any())
            {
                return Tuple.Create(result, "Excel内容为空", sha1Value, new byte[0]);
            }
            var repetitionNumber = result.GroupBy(x => x.MobileNumber)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key).ToList();
            if (repetitionNumber.Any())
            {
                message = $"{string.Join(",", repetitionNumber)}以上手机号重复, 请确认";
            }

            return Tuple.Create(result, message, sha1Value, outBuffer);
        }

        private static Tuple<List<BaoYangPackagePromotionDetail>, string> ConvertExcelToList(ISheet sheet)
        {
            var result = new List<BaoYangPackagePromotionDetail>();
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
            var last = sheet.LastRowNum;
            if (titleRow.GetCell(index++)?.StringCellValue == "手机号" && titleRow.GetCell(index++)?.StringCellValue == "车牌号")
            {
                if (last < 5000)
                {
                    var phoneRegex = new Regex("^[0-9]{11}$");
                    var carNoRegex = new Regex("^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领A-Z]{1}[A-Z]{1}.{5,6}$");
                    for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        var row = sheet.GetRow(rowIndex);
                        if (row == null) continue;
                        var cellIndex = row.FirstCellNum;
                        var item = new BaoYangPackagePromotionDetail { };

                        var mobile = getStringValue(row.GetCell(cellIndex++));
                        var carNo = getStringValue(row.GetCell(cellIndex++));
                        var quantityStr = getStringValue(row.GetCell(cellIndex++));
                        var startTime = getStringValue(row.GetCell(cellIndex++));
                        var endTime = getStringValue(row.GetCell(cellIndex++));

                        if (string.IsNullOrWhiteSpace(mobile) && string.IsNullOrWhiteSpace(carNo) && string.IsNullOrWhiteSpace(quantityStr))
                        {
                            continue;
                        }
                        int quantity;
                        if (!int.TryParse(quantityStr, out quantity))
                        {
                            message = $"第{rowIndex + 1}行数量格式不正确, 数量必须大于等于1";
                            break;
                        }

                        item.MobileNumber = mobile?.Trim();
                        item.Carno = carNo?.Trim();
                        item.Quantity = quantity;

                        if (!string.IsNullOrWhiteSpace(startTime) || !string.IsNullOrWhiteSpace(endTime))
                        {
                            if (!(!string.IsNullOrWhiteSpace(startTime) && !string.IsNullOrWhiteSpace(endTime)))
                            {
                                message = $"{item.MobileNumber}手机号, 有效期时间填写不完整";
                                break;
                            }
                            DateTime _startTime;
                            if (!DateTime.TryParse(startTime, out _startTime))
                            {
                                item.StartTime = null;
                                message = $"第{rowIndex + 1}行开始日期格式不正确, 请参考模板示例";
                                break;
                            }
                            DateTime _endTime;
                            if (!DateTime.TryParse(endTime, out _endTime))
                            {
                                item.EndTime = null;
                                message = $"第{rowIndex + 1}行截止日期格式不正确, 请参考模板示例";
                                break;
                            }
                            if (_startTime > _endTime)
                            {
                                message = $"第{rowIndex + 1}行截止日期早于开始日期";
                                break;
                            }
                            if (DateTime.Now.Date > _endTime)
                            {
                                message = $"第{rowIndex + 1}行截止日期早于当前日期";
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(item.MobileNumber) && phoneRegex.IsMatch(item.MobileNumber))
                        {
                            if (!string.IsNullOrEmpty(item.Carno) && carNoRegex.IsMatch(item.Carno))
                            {
                                result.Add(item);
                            }
                            else
                            {
                                message = $"第{rowIndex + 1}行车牌号格式不正确, 必须填写车牌号";
                                break;
                            }
                        }
                        else
                        {
                            message = $"第{rowIndex + 1}行手机号格式不正确, 必须填写手机号";
                            break;
                        }

                    }
                }
                else
                {
                    message = "数据不能超过1000条,请重新上传";
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

        #region PromotionDetail

        public ActionResult GetPromotionDetails(string batchcode, int index = 1, int size = 10)
        {
            var manager = new VipBaoYangPackageManager();
            var promotionDetails = manager.GetPromotionDetailsByBatchCode(batchcode, index, size);
            promotionDetails?.ForEach(f =>
            {
                if (f.StartTime.HasValue)
                    f.StartTime = f.StartTime.Value.Date;
                if (f.EndTime.HasValue)
                    f.EndTime = f.EndTime.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            });
            return Json(new
            {
                status = promotionDetails != null,
                data = promotionDetails,
                total = promotionDetails?.FirstOrDefault()?.Total ?? 0
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPackageConfigSimpleInfo(string batchcode)
        {
            var manager = new VipBaoYangPackageManager();
            var packageInfo = manager.GetPackageConfigSimpleInfo(batchcode);
            return Json(new
            {
                status = packageInfo != null,
                data = packageInfo
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PromotionDetail()
        {
            return View();
        }

        public ActionResult UpdatePromotionDetail(long pkid, string mobileNumber)
        {
            if (pkid < 0 || !Regex.IsMatch(mobileNumber, "^[0-9]{11}$"))
            {
                return Json(new { status = false, msg = "修改条目有误或者输入的手机号不正确" });
            }
            var manager = new VipBaoYangPackageManager();
            var result = manager.UpdatePromotionDetail(pkid, mobileNumber, User.Identity.Name);

            return Json(new { status = result });
        }

        [HttpPost]
        public JsonResult InvalidCodes(string codeStr)
        {
            bool status = false;
            string message = string.Empty;
            List<int> codes = JsonConvert.DeserializeObject<List<int>>(codeStr);
            if (codes != null && codes.Any())
            {
                var manager = new VipBaoYangPackageManager();
                var checkResult = manager.IsAllUnUsed(codes);
                if (checkResult)
                {
                    var invalidResult = manager.InvalidCodes(codes, User.Identity.Name);
                    status = invalidResult.All(o => o.Value);
                    message = $"{string.Join(",", invalidResult.Where(o => o.Value).Select(o => o.Key))}成功，{string.Join(",", invalidResult.Where(o => !o.Value).Select(o => o.Key))}失败";
                }
                else
                {
                    status = false;
                    message = "选中的优惠券中包含已作废的券或者已使用的券，请刷新后重新操作";
                }
            }

            return Json(new { status = status, msg = message });
        }

        public ActionResult GetFileTaskStatus(string batchcode)
        {
            var result = new UploadFileManager().GetFileTaskStatus(batchcode, FileType.VipBaoYangPackage);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Exchange code 兑换码

        [PowerManage]
        public ActionResult RedemptionCode()
        {
            return View();
        }

        public ActionResult GetVipBaoYangConfigSimpleInfo()
        {
            var manager = new VipBaoYangPackageManager();
            var result = manager.GetVipBaoYangConfigSimpleInfo();
            return Json(new
            {
                status = true,
                data = result.Select(x => new { PackageId = x.Key, PackageName = x.Value })
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 分页获取礼包配置
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ActionResult GetVipBaoYangGiftPackConfig(int index, int size)
        {
            var manager = new VipBaoYangPackageManager();
            var result = manager.GetVipBaoYangGiftPackConfig(index, size);
            return Json(new
            {
                status = true,
                total = result.Total,
                data = result.Data,
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新或者新增礼包配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public ActionResult AddOrUpdateGiftPackConfig(VipBaoYangGiftPackConfig config)
        {
            if (config == null || string.IsNullOrWhiteSpace(config.PackName) || config.PackageId < 0)
            {
                return Json(new { status = false, msg = "礼包名称、套餐不能为空" });
            }
            config.PackName = config.PackName.Trim();
            var manager = new VipBaoYangPackageManager();
            if (manager.IsExisitsVipBaoYangGiftPackName(config))
            {
                return Json(new { status = false, msg = "礼包名称已存在" });
            }
            var result = config.PackId <= 0 ?
                manager.AddVipBaoYangGiftPackConfig(config, User.Identity.Name) :
                manager.EditVipBaoYangGiftPackConfig(config);
            return Json(new { status = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据礼包Id获取优惠券配置
        /// </summary>
        /// <param name="packId"></param>
        /// <returns></returns>
        public ActionResult GetGiftPackCouponConfig(long packId)
        {
            var manager = new VipBaoYangPackageManager();
            var result = manager.GetGiftPackCouponConfig(packId);
            var list = manager.GetRedemptionCodeSimpleInfo(packId);
            var existsRedemptionCode = list.Any();
            return Json(new
            {
                data = result.Select(x => new
                {
                    x.RuleID,
                    x.Description,
                    x.GetRuleID,
                    x.Name,
                    x.PackId,
                    x.PromtionName,
                    x.Quantity,
                    x.Term,
                    ValiEndDate = x.ValiEndDate?.ToString("yyyy-MM-dd"),
                    ValiStartDate = x.ValiStartDate?.ToString("yyyy-MM-dd"),
                }),
                ExistsRedemptionCode = existsRedemptionCode,
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加礼包优惠券配置
        /// </summary>
        /// <param name="packId"></param>
        /// <param name="getRuleId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public ActionResult AddGiftPackCouponConfig(long packId, int quantity, Guid? getRuleId)
        {
            if (packId < 0 || getRuleId == null || getRuleId.Value == Guid.Empty || quantity < 0)
            {
                return Json(new { status = false, msg = "礼包、优惠券领取Id、优惠券数量必须填写" });
            }
            var manager = new VipBaoYangPackageManager();
            var users = new[] { "wangminyou@tuhu.cn", "xieshuquan@tuhu.cn", "devteam@tuhu.work", "zhangjianfeng@tuhu.cn" };
            if (!users.Contains(User.Identity.Name))
            {
                var list = manager.GetRedemptionCodeSimpleInfo(packId);
                if (list.Any())
                {
                    return Json(new { status = false, msg = "已经生成过兑换码不能再添加优惠券" });
                }
            }
            var validatedResult = manager.ValidateGiftPackCouponConfig(packId, getRuleId.Value);
            if (!validatedResult.Item1)
            {
                return Json(new { status = false, msg = validatedResult.Item2 });
            }
            var result = manager.AddGiftPackCouponConfig(packId, getRuleId.Value, quantity, User.Identity.Name);
            return Json(new { status = result });
        }

        /// <summary>
        /// 生成兑换码
        /// </summary>
        /// <param name="PackId"></param>
        /// <param name="quantity"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult GenerateRedemptionCode(long PackId, int quantity, string startTime, string endTime)
        {

            DateTime startDate, endDate;
            if (!DateTime.TryParse(startTime, out startDate) || !DateTime.TryParse(endTime, out endDate) || quantity < 1)
            {
                return Json(new { status = false, msg = "参数错误" });
            }

            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1).AddSeconds(-1);

            if (startDate > endDate || endDate < DateTime.Now)
            {
                return Json(new { status = false, msg = "开始时间不能大于结束时间,并且结束时间不能小于当前时间" });
            }
            var manager = new VipBaoYangPackageManager();
            var coupons = manager.GetGiftPackCouponConfig(PackId);
            if (coupons == null || !coupons.Any())
            {
                return Json(new { status = false, msg = "您还没有给礼包添加优惠券" });
            }
            var result = manager.GenerateRedemptionCode(PackId, quantity, startDate, endDate, User.Identity.Name);
            return Json(new { status = result });
        }

        /// <summary>
        /// 获取生成兑换码详情
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRedemptionCodeRecord()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 导出一批兑换码
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public ActionResult GetBatchRedemptionCodes(string batchCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取礼包兑换码简要信息
        /// </summary>
        /// <param name="packId"></param>
        /// <returns></returns>
        public ActionResult GetRedemptionCodeSimpleInfo(long packId)
        {
            if (packId < 1)
            {
                return Json(new { status = false, msg = "参数错误" }, JsonRequestBehavior.AllowGet);
            }
            var simpleInfos = new VipBaoYangPackageManager().GetRedemptionCodeSimpleAndRecord(packId);
            return Json(new
            {
                data = simpleInfos.Select(x => new
                {
                    x.BatchCode,
                    CreateDateTime = x.CreateDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    x.CreateUser,
                    Records = x.Records.Select(t => new
                    {
                        DownloadUser = t.OperateUser,
                        DownloadTime = t.CreateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    })
                }),
                status = true
            });
        }

        /// <summary>
        /// 获取兑换码
        /// </summary>
        /// <param name="batchCode"></param>
        /// <param name="packId"></param>
        /// <returns></returns>
        public ActionResult DownloadRedemptionCode(string batchCode, long packId)
        {
            var manager = new VipBaoYangPackageManager();
            var result = manager.GetRedemptionCodeDetails(batchCode, packId, User.Identity.Name);

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var row = sheet.CreateRow(0);
            var cell = null as ICell;
            var cellNum = 0;

            row.CreateCell(cellNum++).SetCellValue("兑换码");
            row.CreateCell(cellNum++).SetCellValue("有效开始时间");
            row.CreateCell(cellNum++).SetCellValue("有效截至时间");

            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 20 * 256);
            sheet.SetColumnWidth(cellNum++, 28 * 256);
            sheet.SetColumnWidth(cellNum++, 28 * 256);
            var rowNumber = 1;
            result.ForEach(x =>
            {
                cellNum = 0;
                row = sheet.CreateRow(rowNumber);

                row.CreateCell(cellNum++).SetCellValue(x.RedemptionCode);
                row.CreateCell(cellNum++).SetCellValue(x.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                row.CreateCell(cellNum++).SetCellValue(x.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));

                rowNumber++;
            });

            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"批次{batchCode}兑换码{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        #endregion

        #region Promotion

        /// <summary>
        /// 根据RuleId获取'获取优惠券规则'
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public ActionResult GetCouponGetRules(int ruleId)
        {
            var manager = new VipBaoYangPackageManager();
            var resut = manager.GetCouponGetRules(ruleId);
            return Json(new
            {
                data = resut.Select(x => new
                {
                    GetRuleID = x.Item1,
                    GetRuleGUID = x.Item2,
                    PromtionName = x.Item3,
                    Description = x.Item4,
                })
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取所有优惠券规则
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllCouponRules()
        {
            var manager = new VipBaoYangPackageManager();
            var result = manager.GetAllCouponRules();
            return Json(new
            {
                data = result.Select(x => new
                {
                    RuleID = x.Key,
                    Name = x.Value
                })
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        
    }
}