using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.PaintDiscountConfig;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class PaintDiscountConfigController : Controller
    {
        // GET: 喷漆打折配置
        public ActionResult PaintDiscountConfig()
        {
            return View();
        }

        /// <summary>
        /// 获取所有服务
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllPaintDiscountService()
        {
            var manager = new PaintDiscountConfigManager();
            var result = manager.GetAllPaintDiscountService();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加喷漆打折配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddPaintConfig(PaintDiscountConfigModel model)
        {
            if (model == null)
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.ServicePid))
            {
                return Json(new { Status = false, Msg = "请选择服务Pid" }, JsonRequestBehavior.AllowGet);
            }
            if (model.SurfaceCount < 1)
            {
                return Json(new { Status = false, Msg = "面数须为正整数" }, JsonRequestBehavior.AllowGet);
            }
            if (!(model.ActivityPrice > 0))
            {
                return Json(new { Status = false, Msg = "活动价格须大于0" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.ActivityName))
            {
                return Json(new { Status = false, Msg = "权益名称不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.ActivityImage))
            {
                return Json(new { Status = false, Msg = "活动图片不能为空" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PaintDiscountConfigManager();
            var user = User.Identity.Name;
            var isExist = manager.IsExistPaintDiscountConfig(model);
            if (isExist)
            {
                return Json(new { Status = false, Msg = "已存在重复的数据，不能重复添加" }, JsonRequestBehavior.AllowGet);
            }
            var result = manager.AddPaintDiscountConfig(model, user);
            return Json(new { Status = result, Msg = $"添加{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除喷漆打折配置
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="surfaceCount"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeletePaintConfig(string servicePid, int surfaceCount)
        {
            if (string.IsNullOrWhiteSpace(servicePid) || surfaceCount < 1)
            {
                return Json(new { Status = false, Msg = "未知的删除对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PaintDiscountConfigManager();
            var user = User.Identity.Name;
            var result = manager.DeletePaintDiscountConfig(servicePid, surfaceCount, user);
            return Json(new { Status = result, Msg = $"删除{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        /// <summary>
        /// 更新喷漆打折配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdatePaintConfig(PaintDiscountConfigModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.ServicePid) || model.SurfaceCount < 1)
            {
                return Json(new { Status = false, Msg = "未知的编辑对象" }, JsonRequestBehavior.AllowGet);
            }
            if (!(model.ActivityPrice > 0))
            {
                return Json(new { Status = false, Msg = "活动价格须大于0" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.ActivityName))
            {
                return Json(new { Status = false, Msg = "权益名称不能为空" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.ActivityImage))
            {
                return Json(new { Status = false, Msg = "活动图片不能为空" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PaintDiscountConfigManager();
            var isExist = manager.IsExistPaintDiscountConfig(model);
            if (isExist)
            {
                return Json(new { Status = false, Msg = "已存在重复的数据，不能重复编辑" }, JsonRequestBehavior.AllowGet);
            }
            var user = User.Identity.Name;
            var result = manager.UpdatePaintDiscountConfig(model, user);
            return Json(new { Status = result, Msg = $"编辑{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        /// <summary>
        /// 批量更新喷漆打折配置
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public ActionResult MultUpdatePaintConfig(List<PaintDiscountConfigModel> models, string activityImage)
        {
            if (models == null || !models.Any())
            {
                return Json(new { Status = false, Msg = "未知的编辑对象" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(activityImage))
            {
                return Json(new { Status = false, Msg = "活动图片不能为空" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new PaintDiscountConfigManager();
            foreach (var model in models)
            {
                if (string.IsNullOrWhiteSpace(model.ServicePid) || model.SurfaceCount < 1)
                {
                    return Json(new { Status = false, Msg = "所有记录的PID不能为空和面数须大于0" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.ActivityImage = activityImage;
                }
            }
            var user = User.Identity.Name;
            var result = manager.MultUpdatePaintConfig(models, user);
            return Json(new { Status = result, Msg = $"编辑{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取喷漆打折配置
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="carNoPrefix"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectPaintConfig(string servicePid, int pageIndex = 1, int pageSize = 20)
        {
            var manager = new PaintDiscountConfigManager();
            var result = manager.SelectPaintDiscountConfig(servicePid, pageIndex, pageSize);
            return Json(new { Status = result != null && result.Item1 != null, Data = result.Item1, TotalCount = result.Item2 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadExcel()
        {
            var files = Request.Files;
            if (files.Count < 1)
            {
                return Json(new { Status = false, Msg = "请选择文件" }, JsonRequestBehavior.AllowGet);
            }
            var file = files[0];
            if (file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return Json(new { Status = false, Msg = "请上传Excel文件" }, JsonRequestBehavior.AllowGet);
            }
            var convertResult = ConvertExcelToList(file);
            if (!string.IsNullOrEmpty(convertResult.Item2))
            {
                return Json(new { Status = false, Msg = convertResult.Item2 }, JsonRequestBehavior.AllowGet);
            }
            else if (convertResult.Item1 == null || !convertResult.Item1.Any())
            {
                return Json(new { Status = false, Msg = "Excel内容为空" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var manager = new PaintDiscountConfigManager();
                var user = User.Identity.Name;
                var result = manager.UploadPaintDiscountConfig(convertResult.Item1, user);
                return Json(new { Status = result, Msg = $"导入{(result ? "成功" : "失败")}" }
                        , JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 导出数据至Excel
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportExcel()
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var row = sheet.CreateRow(0);
            var cell = null as ICell;
            var cellNum = 0;
            row.CreateCell(cellNum++).SetCellValue("PID");
            row.CreateCell(cellNum++).SetCellValue("面数");
            row.CreateCell(cellNum++).SetCellValue("活动价");
            row.CreateCell(cellNum++).SetCellValue("权益名称");
            row.CreateCell(cellNum++).SetCellValue("活动说明");
            row.CreateCell(cellNum++).SetCellValue("活动图片");
            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 8 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 50 * 256);
            sheet.SetColumnWidth(cellNum++, 50 * 256);
            sheet.SetColumnWidth(cellNum++, 28 * 256);
            var manager = new PaintDiscountConfigManager();
            var result = manager.SelectPaintDiscountConfig("", 1, 10000);
            if (result != null && result.Item1 != null && result.Item1.Any())
            {
                int modelRowCount = 1;
                foreach (var model in result.Item1)
                {
                    int modelCol = 0;
                    var modelRow = sheet.CreateRow(modelRowCount);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.ServicePid);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.SurfaceCount);
                    modelRow.CreateCell(modelCol++).SetCellValue((double)model.ActivityPrice);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.ActivityName);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.ActivityExplain);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.ActivityImage);
                    modelRowCount++;
                }
            }
            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"喷漆打折配置 {DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        #region 查看日志
        /// <summary>
        /// 获取喷漆打折操作日志
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="carNoPrefix"></param>
        /// <param name="surfaceCount"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPaintDiscountOprLog(string servicePid, int surfaceCount)
        {
            if (string.IsNullOrEmpty(servicePid) || surfaceCount < 1)
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            var logType = "PaintDiscountConfig";
            var identityID = $"{servicePid}_{surfaceCount}";
            var manager = new PaintDiscountConfigManager();
            var result = manager.SelectPaintDiscountOprLog(logType, identityID);
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 刷新缓存
        /// <summary>
        /// 刷新喷漆打折服务缓存
        /// </summary>
        /// <returns></returns>
        public ActionResult RefreshPaintDiscountPaintCache()
        {
            var manager = new PaintDiscountConfigManager();
            var result = manager.RefreshPaintDiscountConfigCache();
            return Json(new { Status = result, Msg = $"刷新缓存{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// Excel转为List<PaintDiscountConfigModel>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static Tuple<List<PaintDiscountConfigModel>, string> ConvertExcelToList(HttpPostedFileBase file)
        {
            var message = string.Empty;
            var result = null as List<PaintDiscountConfigModel>;
            try
            {
                var stream = file.InputStream;
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
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
        /// sheet=>List<PaintDiscountConfigModel>
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private static Tuple<List<PaintDiscountConfigModel>, string> ConvertExcelToList(ISheet sheet)
        {
            var result = new List<PaintDiscountConfigModel>();
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
            if (titleRow.GetCell(index++)?.StringCellValue == "PID" &&
                titleRow.GetCell(index++)?.StringCellValue == "面数" &&
                titleRow.GetCell(index++)?.StringCellValue == "活动价" &&
                titleRow.GetCell(index++)?.StringCellValue == "权益名称" &&
                titleRow.GetCell(index++)?.StringCellValue == "活动说明" &&
                titleRow.GetCell(index)?.StringCellValue == "活动图片")
            {
                var nowTime = DateTime.Now;
                for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;
                    var cellIndex = row.FirstCellNum;

                    var pid = getStringValue(row.GetCell(cellIndex++));
                    var surfaceCountStr = getStringValue(row.GetCell(cellIndex++));
                    var activityPriceStr = getStringValue(row.GetCell(cellIndex++));
                    var activityName = getStringValue(row.GetCell(cellIndex++));
                    var activityExplain = getStringValue(row.GetCell(cellIndex++));

                    int surfaceCount;
                    surfaceCount = int.TryParse(surfaceCountStr, out surfaceCount) ? surfaceCount : 0;

                    decimal activityPrice;
                    activityPrice = decimal.TryParse(activityPriceStr, out activityPrice) ? activityPrice : 0;

                    if (!string.IsNullOrWhiteSpace(pid) && surfaceCount > 0
                        && activityPrice > 0 && !string.IsNullOrWhiteSpace(activityName))
                    {
                        result.Add(new PaintDiscountConfigModel()
                        {
                            ServicePid = pid,
                            SurfaceCount = surfaceCount,
                            ActivityPrice = activityPrice,
                            ActivityName = activityName,
                            ActivityExplain = activityExplain ?? string.Empty,
                            ActivityImage = string.Empty
                        });
                    }
                    else
                    {
                        message = $"第{rowIndex + 1}行格式不正确, 必须填写PID、面数为正整数、活动价大于0、权益名称不能为空";
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
    }
}