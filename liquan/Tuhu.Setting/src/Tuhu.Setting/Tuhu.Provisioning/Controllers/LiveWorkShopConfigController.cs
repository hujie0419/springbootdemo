using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.LiveWorkShopConfig;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class LiveWorkShopConfigController : Controller
    {
        // GET: ShopLiveWorkShop
        public ActionResult LiveWorkShopConfig()
        {
            return View();
        }

        /// <summary>
        /// 获取透明工场配置类型
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllLiveWorkShopConfigType()
        {
            var manager = new LiveWorkShopConfigManager();
            var result = manager.GetAllLiveWorkShopConfigType();
            return Json(new { Status = result.Any(), Data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 透明工场配置展示
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectLiveWorkShopConfig(string typeName = "", int pageIndex = 1, int pageSize = 20)
        {
            var manager = new LiveWorkShopConfigManager();
            var result = manager.SelectLiveWorkShopConfig(typeName, pageIndex, pageSize);
            if (result.Item1 == null)
            {
                return Json(new { Status = false, Msg = "查询失败" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var totalPage = (result.Item2 % pageSize == 0) ? ((int)result.Item2 / pageSize) : ((int)result.Item2 / pageSize + 1);
                return Json(new { Status = true, Data = result.Item1, TotalCount = result.Item2, TotalPage = totalPage }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 导出Excel模版
        /// </summary>
        /// <returns></returns>
        public FileResult ExportLiveWorkShopConfigTemplate()
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var row = sheet.CreateRow(0);
            var cell = null as ICell;
            var cellNum = 0;
            row.CreateCell(cellNum++).SetCellValue("类型");
            row.CreateCell(cellNum++).SetCellValue("排序");
            row.CreateCell(cellNum++).SetCellValue("文字");
            row.CreateCell(cellNum++).SetCellValue("静态图");
            row.CreateCell(cellNum++).SetCellValue("Gif图");
            row.CreateCell(cellNum++).SetCellValue("H5跳转Url");
            row.CreateCell(cellNum++).SetCellValue("Pc跳转Url");
            row.CreateCell(cellNum++).SetCellValue("门店Id");
            row.CreateCell(cellNum++).SetCellValue("门店设备渠道");
            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 8 * 256);
            sheet.SetColumnWidth(cellNum++, 18 * 256);
            sheet.SetColumnWidth(cellNum++, 50 * 256);
            sheet.SetColumnWidth(cellNum++, 50 * 256);
            sheet.SetColumnWidth(cellNum++, 28 * 256);
            sheet.SetColumnWidth(cellNum++, 28 * 256);
            sheet.SetColumnWidth(cellNum++, 8 * 256);
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            var manager = new LiveWorkShopConfigManager();
            var result = manager.SelectLiveWorkShopConfig("", 1, 10000);
            if (result != null && result.Item1 != null && result.Item1.Any())
            {
                int modelRowCount = 1;
                foreach (var model in result.Item1)
                {
                    int modelCol = 0;
                    var modelRow = sheet.CreateRow(modelRowCount);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.TypeName);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.SortNumber);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.Content);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.Picture);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.Gif);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.H5Url);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.PcUrl);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.ShopId);
                    modelRow.CreateCell(modelCol++).SetCellValue(model.ChannelName);
                    modelRowCount++;
                }
            }
            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"透明工场配置 {DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        /// <summary>
        /// Excel导入
        /// </summary>
        /// <returns></returns>
        public JsonResult ImportLiveWorkShopConfig()
        {
            var files = Request.Files;
            if (files.Count <= 0)
            {
                return Json(new { Status = false, Msg = "请上传Excel文件" }, "text/html");
            }
            var file = files[0];
            if (file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return Json(new { Status = false, Msg = "请上传Excel文件" }, "text/html");
            }
            var result = ConvertExcelToList(file);
            if (!string.IsNullOrEmpty(result.Item2))
            {
                return Json(new { Status = false, Msg = result.Item2 }, "text/html");
            }
            var list = result.Item1 ?? new List<LiveWorkShopConfigModel>();
            var manager = new LiveWorkShopConfigManager();
            var errorList = list.Where(s => string.IsNullOrWhiteSpace(s.TypeName) || s.SortNumber < 0).ToList();
            if (errorList.Any())
            {
                return Json(new { Status = false, Msg = "参数错误" }, "text/html");
            }
            var success = manager.ImportLiveWorkShopConfig(list);
            return Json(new { Status = success, Msg = "导入" + (success ? "成功" : "失败") }, "text/html");
        }

        /// <summary>
        /// 刷新透明工场配置服务缓存
        /// </summary>
        /// <returns></returns>
        public ActionResult RefreshLiveWorkShopConfigCache()
        {
            var manager = new LiveWorkShopConfigManager();
            var result = manager.RefreshLiveWorkShopConfigCache();
            return Json(new { Status = result, Msg = "刷新缓存" + (result ? "成功" : "失败") }, JsonRequestBehavior.AllowGet);
        }

        #region ExcelToList
        /// <summary>
        /// Excel模版转为model List
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static Tuple<List<LiveWorkShopConfigModel>, string> ConvertExcelToList(HttpPostedFileBase file)
        {
            var errorMessage = string.Empty;
            var models = null as List<LiveWorkShopConfigModel>;
            var workBook = new XSSFWorkbook(file.InputStream);
            var sheet = workBook.GetSheetAt(0);
            var temp = ConvertExcelToList(sheet);
            models = temp.Item1;
            errorMessage = temp.Item2;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return Tuple.Create(models, errorMessage);
            }
            if (!models.Any())
            {
                return Tuple.Create(models, "Excel内容为空");
            }
            return Tuple.Create(models, errorMessage);
        }

        /// <summary>
        /// Excel模版转为model List
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private static Tuple<List<LiveWorkShopConfigModel>, string> ConvertExcelToList(ISheet sheet)
        {
            var result = new List<LiveWorkShopConfigModel>();
            var message = string.Empty;
            //所有单元格转为String类型 便于处理
            Func<ICell, string> getStringValue = cell =>
            {
                if (cell != null)
                {
                    if (cell.CellType == CellType.Numeric)
                    {
                        return cell.NumericCellValue.ToString();
                    }
                    return cell.StringCellValue;
                }
                return null;
            };
            var titleRow = sheet.GetRow(sheet.FirstRowNum);
            var index = titleRow.FirstCellNum;
            if (titleRow.GetCell(index++)?.StringCellValue == "类型" &&
                titleRow.GetCell(index++)?.StringCellValue == "排序" &&
                titleRow.GetCell(index++)?.StringCellValue == "文字" &&
                titleRow.GetCell(index++)?.StringCellValue == "静态图" &&
                titleRow.GetCell(index++)?.StringCellValue == "Gif图" &&
                titleRow.GetCell(index++)?.StringCellValue == "H5跳转Url" &&
                titleRow.GetCell(index++)?.StringCellValue == "Pc跳转Url" &&
                titleRow.GetCell(index++)?.StringCellValue == "门店Id" &&
                titleRow.GetCell(index)?.StringCellValue == "门店设备渠道")
            {
                for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;
                    var cellIndex = row.FirstCellNum;
                    var typeName = getStringValue(row.GetCell(cellIndex++));
                    var sortNumberStr = getStringValue(row.GetCell(cellIndex++));
                    int sortNumber = -1;
                    int.TryParse(sortNumberStr, out sortNumber);
                    var content = getStringValue(row.GetCell(cellIndex++));
                    var picture = getStringValue(row.GetCell(cellIndex++));
                    var gif = getStringValue(row.GetCell(cellIndex++));
                    var h5Url = getStringValue(row.GetCell(cellIndex++));
                    var pcUrl = getStringValue(row.GetCell(cellIndex++));
                    var shopIdStr = getStringValue(row.GetCell(cellIndex++));
                    int shopId = 0;
                    int.TryParse(shopIdStr, out shopId);
                    var channelName = getStringValue(row.GetCell(cellIndex));
                    if (string.IsNullOrWhiteSpace(typeName) || sortNumber < 0)
                    {
                        return Tuple.Create(new List<LiveWorkShopConfigModel>(), $"第{(rowIndex + 1)}行错误,请检查类型、图片、排序");
                    }
                    result.Add(new LiveWorkShopConfigModel
                    {
                        TypeName = typeName,
                        SortNumber = sortNumber,
                        Content = content,
                        Picture = picture,
                        Gif = gif,
                        H5Url = h5Url,
                        PcUrl = pcUrl,
                        ShopId = shopId,
                        ChannelName = channelName
                    });
                }
            }
            else
            {
                message = "与模板不一致，请下载模板之后根据模板填写";
            }
            return Tuple.Create(result, message);
        }
        #endregion
    }
}