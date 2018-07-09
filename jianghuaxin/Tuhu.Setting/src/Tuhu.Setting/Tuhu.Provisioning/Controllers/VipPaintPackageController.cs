using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.UploadFile;
using Tuhu.Provisioning.Business.VipBaoYangPackage;
using Tuhu.Provisioning.Business.VipPaintPackage;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.VipBaoYangPackage;
using static Tuhu.Provisioning.DataAccess.Entity.VipPaintPackageModel;

namespace Tuhu.Provisioning.Controllers
{
    public class VipPaintPackageController : Controller
    {
        /// <summary>
        /// 喷漆大客户套餐配置
        /// </summary>
        /// <returns></returns>
        public ActionResult PackageConfig()
        {
            return View();
        }

        /// <summary>
        /// 添加喷漆大客户套餐
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage]
        public ActionResult AddVipPaintPackage(VipPaintPackageConfigModel model)
        {
            #region 数据验证
            if (model == null)
            {
                return Json(new { Status = false, Msg = "未知的添加对象" });
            }
            if (string.IsNullOrWhiteSpace(model.PackageName))
            {
                return Json(new { Status = false, Msg = "套餐名称不能为空" });
            }
            if (model.VipUserId == null || model.VipUserId.Equals(Guid.Empty))
            {
                return Json(new { Status = false, Msg = "请选择套餐所属大客户" });
            }
            if (model.PackagePrice <= 0)
            {
                return Json(new { Status = false, Msg = "请输入套餐价格" });
            }
            if (string.IsNullOrWhiteSpace(model.SettlementMethod))
            {
                return Json(new { Status = false, Msg = "请选择结算方式" });
            }
            var servicePids = model.ServicePids?.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)?.ToList();
            if (servicePids == null || !servicePids.Any())
            {
                return Json(new { Status = false, Msg = "请选择适用Pid" });
            }
            #endregion
            model.PackagePid = string.Empty;
            model.Operator = User.Identity.Name;
            var manager = new VipPaintPackageManager();
            var isExist = manager.IsExistPackageConfig(model.PackageName, model.PKID);
            if (isExist)
            {
                return Json(new { Status = false, Msg = "套餐已存在,不可重复添加" });
            }
            var result = manager.AddPackageConfig(model);
            return Json(new { Status = result, Msg = $"添加喷漆大客户套餐配置{(result ? "成功" : "失败")}" });
        }

        /// <summary>
        /// 删除喷漆大客户套餐
        /// 已塞券的套餐不可删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage]
        public ActionResult DeleteVipPaintPackage(VipPaintPackageConfigModel model)
        {
            if (model == null || model.PKID <= 0 || string.IsNullOrWhiteSpace(model.PackageName))
            {
                return Json(new { Status = false, Msg = "未知的删除对象" });
            }
            var manager = new VipPaintPackageManager();
            var illegal = manager.IsExistPromotionRecord(model.PKID);
            if (illegal)
            {
                return Json(new { Status = false, Msg = "该套餐已给用户塞券，不可删除" });
            }
            var result = manager.DeletePackageConfig(model.PackageName, User.Identity.Name);
            return Json(new { Status = result, Msg = $"删除喷漆大客户套餐配置{(result ? "成功" : "失败")}" });
        }

        /// <summary>
        /// 获取所有大客户
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllPaintPackageUser()
        {
            var manager = new VipBaoYangPackageManager();
            var result = manager.GetAllBaoYangPackageUser();
            return Json(new { Status = result.Any(), Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查看喷漆大客户套餐配置
        /// </summary>
        /// <param name="packagePid"></param>
        /// <param name="vipUserId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectVipPaintPakcage(string packagePid, Guid vipUserId, int pageIndex = 1, int pageSize = 20)
        {
            pageIndex = pageIndex > 0 ? pageIndex : 1;
            var manager = new VipPaintPackageManager();
            var result = manager.SelectPackageConfig(packagePid, vipUserId, pageIndex, pageSize);
            return Json(new { Status = result.Item1 != null, Data = result.Item1, TotalCount = result.Item2 }, JsonRequestBehavior.AllowGet);
        }


        #region 给用户塞券
        public ActionResult PromotionOperation()
        {
            return View();
        }

        /// <summary>
        /// 喷漆大客户优惠券
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCouponRules()
        {
            var manager = new VipPaintPackageManager();
            var result = manager.SelectCouponGetRules();
            return Json(result.Select(x => new
            {
                PKID = x.Item1,
                RuleGUID = x.Item2,
                PromotionName = x.Item3,
                Description = x.Item4
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取配置的喷漆大客户
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPaintVipUsers()
        {
            var manager = new VipPaintPackageManager();
            var result = manager.GetPaintVipUsers();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取喷漆大客户下配置的喷漆套餐
        /// </summary>
        /// <param name="vipUserId"></param>
        /// <returns></returns>
        public ActionResult GetVipPaintPackages(Guid vipUserId)
        {
            if (vipUserId == Guid.Empty)
            {
                return Json(new { Status = false, Msg = "未知的喷漆大客户" });
            }
            else
            {
                var manager = new VipPaintPackageManager();
                var result = manager.GetVipPaintPackages(vipUserId);
                return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 喷漆大客户塞券模版
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportExcelTemplate()
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var row = sheet.CreateRow(0);
            var cell = null as ICell;
            var cellNum = 0;
            row.CreateCell(cellNum++).SetCellValue("手机号");
            row.CreateCell(cellNum++).SetCellValue("车牌号");
            row.CreateCell(cellNum++).SetCellValue("塞券数量");
            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 8 * 256);
            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"喷漆大客户塞券模版 {DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadExcelForView()
        {
            var files = Request.Files;
            if (files.Count < 1)
            {
                return Json(new { Status = false, Msg = "请选择文件" });
            }
            var file = files[0];
            if (file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return Json(new { Status = false, Msg = "请上传Excel文件" });
            }
            var convertResult = ConvertExcelToList(file);
            if (!string.IsNullOrEmpty(convertResult.Item2))
            {
                return Json(new { Status = false, Msg = convertResult.Item2 });
            }
            else if (convertResult.Item1 == null || !convertResult.Item1.Any())
            {
                return Json(new { Status = false, Msg = "Excel内容为空" });
            }
            else
            {
                return Json(new
                {
                    Status = true,
                    Data = convertResult.Item1,
                    TotalCount = convertResult.Item1.Count()
                });
            }
        }

        /// <summary>
        /// 上传文件以给用户塞券
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="ruleGuid"></param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage]
        public ActionResult UploadExcel(int packageId, Guid ruleGuid)
        {
            var result = false;
            if (packageId <= 0 || ruleGuid == Guid.Empty)
            {
                return Json(new { Status = false, Msg = "未知的对象" });
            }
            var files = Request.Files;
            if (files.Count < 1)
            {
                return Json(new { Status = false, Msg = "请选择文件" });
            }
            var file = files[0];
            if (file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return Json(new { Status = false, Msg = "请上传Excel文件" });
            }
            var manager = new VipPaintPackageManager();
            var isExist = manager.IsExistVipPaintFile(file.FileName);
            if (isExist)
            {
                return Json(new { Status = false, Msg = $"{file.FileName}已导入成功，请勿重复导入" }, JsonRequestBehavior.AllowGet);
            }
            var convertResult = ConvertExcelToList(file);
            if (!string.IsNullOrEmpty(convertResult.Item2))
            {
                return Json(new { Status = false, Msg = convertResult.Item2 });
            }
            else if (convertResult.Item1 == null || !convertResult.Item1.Any())
            {
                return Json(new { Status = false, Msg = "Excel内容为空" });
            }
            var validResult = manager.ValidatedUploadData(ruleGuid, convertResult.Item1);
            if (!validResult.Item1)
            {
                return Json(new { Status = false, Msg = string.IsNullOrEmpty(validResult.Item2) ? "优惠券规则验证失败" : validResult.Item2 });
            }
            #region 上传文件到服务器
            var extension = Path.GetExtension(file.FileName);
            var stream = file.InputStream;
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Position = 0;
            var uploadResult = new UploadFileManager().UploadFile(buffer, FileType.VipPaintPackage, extension, file.FileName, User.Identity.Name);
            #endregion
            //上传服务器成功插入一条塞券记录
            if (!string.IsNullOrEmpty(uploadResult.Item1) && !string.IsNullOrEmpty(uploadResult.Item2))
            {
                var isSendSms = manager.IsPackageSms(packageId);
                var model = new VipPaintPackagePromotionRecordModel()
                {
                    PackageId = packageId,
                    BatchCode = uploadResult.Item2,
                    RuleGUID = ruleGuid,
                    IsSendSms = isSendSms,
                    ToBOrder = string.Empty,
                    CreateUser = User.Identity.Name
                };
                result = manager.InsertPromotionRecord(model);
            }
            return Json(new { Status = result, Msg = result ? "上传成功" : "上传失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Excel转为List<VipPaintPromotionTemplateModel>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static Tuple<List<VipPaintPromotionTemplateModel>, string> ConvertExcelToList(HttpPostedFileBase file)
        {
            var message = string.Empty;
            var result = null as List<VipPaintPromotionTemplateModel>;
            try
            {
                var stream = file.InputStream;
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Position = 0;
                var workBook = new XSSFWorkbook(new MemoryStream(buffer));
                var sheet = workBook.GetSheetAt(0);
                var temp = ConvertExcelToList(sheet);
                result = temp.Item1;
                message = temp.Item2;
                if (!string.IsNullOrEmpty(message))
                {
                    return Tuple.Create(result, message);
                }
                if (!result.Any())
                {
                    return Tuple.Create(result, "Excel内容为空");
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Tuple.Create(result, message);
        }

        /// <summary>
        /// sheet=>List<VipPaintPromotionTemplateModel>
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private static Tuple<List<VipPaintPromotionTemplateModel>, string> ConvertExcelToList(ISheet sheet)
        {
            var result = new List<VipPaintPromotionTemplateModel>();
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
            if (titleRow.GetCell(index++)?.StringCellValue == "手机号" &&
                titleRow.GetCell(index++)?.StringCellValue == "车牌号" &&
                titleRow.GetCell(index++)?.StringCellValue == "塞券数量")
            {
                var nowTime = DateTime.Now;
                for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;
                    var cellIndex = row.FirstCellNum;
                    var mobilePhone = getStringValue(row.GetCell(cellIndex++))?.Trim();
                    var carNo = getStringValue(row.GetCell(cellIndex++))?.Trim();
                    var promotionCountStr = getStringValue(row.GetCell(cellIndex++));

                    int promotionCount;
                    promotionCount = int.TryParse(promotionCountStr, out promotionCount) ? promotionCount : 0;

                    if (!string.IsNullOrWhiteSpace(mobilePhone) && mobilePhone.Length < 12
                        && !string.IsNullOrWhiteSpace(carNo) && carNo.Length <= 20 && promotionCount > 0)
                    {
                        result.Add(new VipPaintPromotionTemplateModel()
                        {
                            MobileNumber = mobilePhone,
                            CarNo = carNo,
                            PromotionCount = promotionCount,
                        });
                    }
                    else
                    {
                        message = $"第{rowIndex + 1}行格式不正确, 必须填写11位手机号,车牌号和塞券数量";
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
        #endregion

        #region 塞券记录
        public ActionResult PromotionRecord()
        {
            return View();
        }

        /// <summary>
        /// 查看塞券记录
        /// </summary>
        /// <param name="batchCode"></param>
        /// <param name="mobileNumber"></param>
        /// <param name="packagePid"></param>
        /// <param name="vipUserId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectPromotionRecord
            (string batchCode, string mobileNumber, string packagePid, Guid vipUserId, int pageIndex = 1, int pageSize = 20)
        {
            var manager = new VipPaintPackageManager();
            var result = manager.SelectPromotionRecord(batchCode, mobileNumber, packagePid, vipUserId, pageIndex, pageSize);
            return Json(new { Status = result.Item1 != null, Data = result.Item1, TotalCount = result.Item2 }
                        , JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 下载该批次塞券详情
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        [PowerManage]
        public ActionResult ExportExcel(string batchCode)
        {
            var manager = new VipPaintPackageManager();
            if (string.IsNullOrWhiteSpace(batchCode))
            {
                return Json(new { Status = false, Msg = "未知的批次,无法获取详情" }, JsonRequestBehavior.AllowGet);
            }
            var packageInfo = manager.GetPromotionConfigForDetail(batchCode);
            var details = manager.SelectPromotionDetail(new VipPaintPackagePromotionDetail()
            {
                BatchCode = batchCode,
                MobileNumber = string.Empty,
                PromotionId = 0,
                Status = string.Empty
            }, 1, int.MaxValue);

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var row = sheet.CreateRow(0);
            var cell = null as ICell;
            var cellNum = 0;
            row.CreateCell(cellNum++).SetCellValue("批次号");
            row.CreateCell(cellNum++).SetCellValue("生成的2B订单号");
            row.CreateCell(cellNum++).SetCellValue("生成的2C订单号");
            row.CreateCell(cellNum++).SetCellValue("塞券状态");
            row.CreateCell(cellNum++).SetCellValue("手机号");
            row.CreateCell(cellNum++).SetCellValue("车牌号");
            row.CreateCell(cellNum++).SetCellValue("所属大客户");
            row.CreateCell(cellNum++).SetCellValue("套擦PID");
            row.CreateCell(cellNum++).SetCellValue("套餐名称");
            row.CreateCell(cellNum++).SetCellValue("塞券人");
            row.CreateCell(cellNum++).SetCellValue("塞券时间");
            row.CreateCell(cellNum++).SetCellValue("短信");
            row.CreateCell(cellNum++).SetCellValue("备注");
            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 8 * 256);
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 8 * 256);
            sheet.SetColumnWidth(cellNum++, 50 * 256);

            if (details != null && details.Item1 != null && details.Item1.Any())
            {
                int modelRowCount = 1;
                foreach (var model in details.Item1)
                {
                    int modelCol = 0;
                    var modelRow = sheet.CreateRow(modelRowCount);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.BatchCode);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.ToBOrder);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.ToCOrder);
                    modelRow.CreateCell(modelCol++).SetCellValue(manager.GetVipPaintPromotionStatus(model.Status));
                    modelRow.CreateCell(modelCol++).SetCellValue(model.MobileNumber);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.CarNo);
                    modelRow.CreateCell(modelCol++).SetCellValue(packageInfo.VipUserName);
                    modelRow.CreateCell(modelCol++).SetCellValue(packageInfo.PackagePid);
                    modelRow.CreateCell(modelCol++).SetCellValue(packageInfo.PackageName);
                    modelRow.CreateCell(modelCol++).SetCellValue(packageInfo.CreateUser);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.CreateDateTime.ToString());
                    modelRow.CreateCell(modelCol++).SetCellValue(packageInfo.IsSendSms ? "发送" : "不发送");
                    modelRow.CreateCell(modelCol++).SetCellValue(model.Remarks);
                    modelRowCount++;
                }
            }
            var ms = new MemoryStream();
            workbook.Write(ms);
            #region 下载操作记录日志
            var log = new
            {
                ObjectId = $"PromotionRecord_{batchCode}",
                ObjectType = "VipPaintPackage",
                BeforeValue = "",
                AfterValue = "",
                Remark = $"{User.Identity.Name}下载了批次号为{batchCode}的塞券记录详情",
                Creator = User.Identity.Name
            };
            LoggerManager.InsertLog("CommonConfigLog", log);
            #endregion
            return File(ms.ToArray(), "application/x-xls", $"喷漆大客户塞券批次号为{batchCode}的塞券详情 {DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        #region 塞券详情
        [PowerManage]
        public ActionResult PromotionDetail()
        {
            return View();
        }

        /// <summary>
        /// 获取当前批次相关信息及对应套餐配置
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public ActionResult GetPromotionConfigForDetail(string batchCode)
        {
            if (string.IsNullOrWhiteSpace(batchCode))
            {
                return Json(new { Status = false, Msg = "批次号不能为空" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new VipPaintPackageManager();
            var result = manager.GetPromotionConfigForDetail(batchCode);
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 当前批次号塞券详情
        /// </summary>
        /// <param name="batchCode"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectPromotionDetail
            (string batchCode, string mobileNumber = "", int promotionId = 0, string status = "", int pageIndex = 1, int pageSize = 20)
        {
            pageIndex = pageIndex > 0 ? pageIndex : 1;
            var manager = new VipPaintPackageManager();
            if (string.IsNullOrWhiteSpace(batchCode))
            {
                return Json(new { Status = false, Msg = "未知的批次,无法获取详情" }, JsonRequestBehavior.AllowGet);
            }
            if (!string.IsNullOrWhiteSpace(status) && !Enum.TryParse(status, out Status statusenum))
            {
                status = string.Empty;
            }
            var result = manager.SelectPromotionDetail(new VipPaintPackagePromotionDetail()
            {
                BatchCode = batchCode,
                MobileNumber = mobileNumber,
                PromotionId = promotionId,
                Status = status
            }, pageIndex, pageSize);
            return Json(new { Status = result.Item1 != null, Data = result.Item1, TotalCount = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 该批次下未成功的记录
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetNotSuccessPromotionDtailCount(string batchCode)
        {
            if (string.IsNullOrWhiteSpace(batchCode))
            {
                return Json(new { Status = false, Msg = "未知的批次号" });
            }
            var manager = new VipPaintPackageManager();
            var result = manager.GetNotSuccessPromotionDtailCount(batchCode);
            return Json(new { Status = result.HasValue, Data = result.Value });
        }


        /// <summary>
        /// 作废券
        /// </summary>
        /// <param name="codeStr"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InvalidCodes(List<int> promotionIds)
        {
            bool status = false;
            string message = string.Empty;
            if (promotionIds != null && promotionIds.Any())
            {
                var checkResult = new VipBaoYangPackageManager().IsAllUnUsed(promotionIds);
                if (checkResult)
                {
                    var manager = new VipPaintPackageManager();
                    var invalidResult = manager.InvalidCodes(promotionIds, User.Identity.Name);
                    status = invalidResult.All(o => o.Value);
                    message = $"{string.Join(",", invalidResult.Where(o => o.Value).Select(o => o.Key))}成功，{string.Join(",", invalidResult.Where(o => !o.Value).Select(o => o.Key))}失败";
                }
                else
                {
                    status = false;
                    message = "选中的优惠券中包含已作废的券或者已使用的券，请刷新后重新操作";
                }
            }

            return Json(new { Status = status, Msg = message });
        }

        /// <summary>
        /// 更新塞券任务状态
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateFileTaskStatus(string batchCode)
        {
            var result = new UploadFileManager().UpdateFileTaskStatus(batchCode, FileType.VipPaintPackage, FileStatus.Repaired, FileStatus.WaitingForRepair);
            return Json(new { Status = result, Msg = $"操作{(result ? "成功" : "失败")}" });
        }

        /// <summary>
        /// 获取塞券任务状态
        /// </summary>
        /// <param name="batchcode"></param>
        /// <returns></returns>
        public ActionResult GetFileTaskStatus(string batchcode)
        {
            var result = new UploadFileManager().GetFileTaskStatus(batchcode, FileType.VipPaintPackage);
            return Json(new { Status = !string.IsNullOrWhiteSpace(result), Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除塞券详情记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeletePromotionDetail(long pkid)
        {
            if (pkid <= 0)
            {
                return Json(new { Status = false, Msg = "未知的对象" });
            }
            var manager = new VipPaintPackageManager();
            var result = manager.DeletePromotionDetail(pkid, User.Identity.Name);
            return Json(new { Status = result, Msg = $"删除{(result ? "成功" : "失败")}" });
        }

        /// <summary>
        /// 更新塞券详情
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdatePromotionDetail(long pkid, string mobileNumber)
        {
            if (pkid <= 0)
            {
                return Json(new { Status = false, Msg = "未知的对象" });
            }
            if (string.IsNullOrWhiteSpace(mobileNumber))
            {
                return Json(new { Status = false, Msg = "手机号不能为空" });
            }

            var manager = new VipPaintPackageManager();
            var result = manager.UpdatePromotionDetail(pkid, mobileNumber, User.Identity.Name);
            return Json(new { Status = result, Msg = $"更新{(result ? "成功" : "失败")}" });
        }
        #endregion
        #endregion

        #region 短信配置
        public ActionResult PackageSmsConfig()
        {
            return View();
        }

        /// <summary>
        /// 查看短信配置
        /// </summary>
        /// <param name="vipUserId"></param>
        /// <param name="packageId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectPackageSmsConfig(Guid vipUserId, int packageId, int pageIndex = 1, int pageSize = 20)
        {
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            var manager = new VipPaintPackageManager();
            var result = manager.SelectPackageSmsConfig(new VipPaintPackageSmsConfig()
            {
                PackageId = packageId,
                VipUserId = vipUserId
            }, pageIndex, pageSize);
            return Json(new { Status = result.Item1 != null, Data = result.Item1, TotalCount = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新短信配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage]
        public ActionResult UpdOrInsPackageSmsPackage(VipPaintPackageSmsConfig model)
        {
            if (model == null || model.PackageId < 1)
            {
                return Json(new { Status = false, Msg = "未知的对象" });
            }
            var manager = new VipPaintPackageManager();
            model.Operator = User.Identity.Name;
            var result = manager.UpdateOrInsertPackageSmsPackage(model);
            return Json(new { Status = result, Msg = $"更新短信配置{(result ? "成功" : "失败")}" });
        }
        #endregion
    }
}