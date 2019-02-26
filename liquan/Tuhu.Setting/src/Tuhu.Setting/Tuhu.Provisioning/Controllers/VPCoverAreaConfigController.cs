using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.ServiceProxy;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class VPCoverAreaConfigController : Controller
    {
        /// <summary>
        /// 获取所有指定产品类型的PID
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetAllPids(string productType)
        {
            var manager = new VendorProductCommonManager();
            var managerResult = await manager.GetAllPidsFromCache(productType);
            return Json(new { Status = managerResult.Item1 != null, Data = managerResult.Item1, Msg = managerResult.Item2 },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取所有指定产品类型的品牌
        /// </summary>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        public async Task<JsonResult> GetAllBrands(string productType)
        {
            var manager = new VendorProductCommonManager();
            var managerResult = await manager.GetAllBrandsFromCache(productType);
            return Json(new { Status = managerResult.Item1 != null, Data = managerResult.Item1, Msg = managerResult.Item2 },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询覆盖区域--按品牌
        /// </summary>
        /// <param name="brand"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="district"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<JsonResult> SelectCoverArea
            (string productType, string coverType, string brand, string pid, int provinceId, int cityId,
            int districtId, int pageIndex, int pageSize = 20)
        {
            var manager = new VendorProductCoverAreaManager();
            var pager = new PagerModel(pageIndex, pageSize);
            var result = await manager.SelectVendorProductCoverArea(productType, coverType, brand, pid,
                provinceId, cityId, districtId, pager);
            return Json(new { Status = result.Item1 != null, Data = result.Item1, TotalCount = result.Item2 }
                , JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加覆盖区域配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddCoverArea(VendorProductCoverAreaModel model)
        {
            if (model?.CoverRegionId < 1 || string.IsNullOrEmpty(model.ProductType) || string.IsNullOrEmpty(model.CoverType))
            {
                return Json(new { Status = false, Msg = "未知的添加对象" });
            }
            var user = HttpContext.User.Identity.Name;
            var manager = new VendorProductCoverAreaManager();
            var validateResult = manager.ValidateVendorProductConverArea(model);
            if (!validateResult.Item1)
            {
                return Json(new { Status = false, Msg = validateResult.Item2 });
            }
            var result = manager.AddCoverArea(model, user);
            return Json(new { Status = result, Msg = $"添加{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除覆盖区域配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteCoverArea(VendorProductCoverAreaModel model)
        {
            if (model?.PKID < 1)
            {
                return Json(new { Status = false, Msg = "未知的删除对象" });
            }
            bool result = false;
            var user = HttpContext.User.Identity.Name;
            var manager = new VendorProductCoverAreaManager();
            result = manager.DeleteCoverArea(model, user);
            return Json(new { Status = result, Msg = $"删除{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑覆盖区域配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditCoverArea(VendorProductCoverAreaModel model)
        {
            if (model?.CoverRegionId < 1 || string.IsNullOrEmpty(model.ProductType) || string.IsNullOrEmpty(model.CoverType))
            {
                return Json(new { Status = false, Msg = "未知的编辑对象" });
            }
            var user = HttpContext.User.Identity.Name;
            var manager = new VendorProductCoverAreaManager();
            var validateResult = manager.ValidateVendorProductConverArea(model);
            if (!validateResult.Item1)
            {
                return Json(new { Status = false, Msg = validateResult.Item2 });
            }
            var result = manager.EditCoverArea(model, user);
            return Json(new { Status = result, Msg = $"编辑{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 导出Excel模版
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportExcel(string productType)
        {
            var zhName = new VendorProductCommonManager().GetZhNameByProductType(productType);
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            var row = sheet.CreateRow(0);
            var cellNum = 0;
            row.CreateCell(cellNum++).SetCellValue("省份名称");
            row.CreateCell(cellNum++).SetCellValue("城市名称");
            row.CreateCell(cellNum++).SetCellValue("区域名称");
            row.CreateCell(cellNum++).SetCellValue("品牌");
            row.CreateCell(cellNum++).SetCellValue("是否启用TRUE/FALSE");
            row.CreateCell(cellNum).SetCellValue("备注(选填)");
            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 10 * 256);
            sheet.SetColumnWidth(cellNum++, 10 * 256);
            sheet.SetColumnWidth(cellNum++, 10 * 256);
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 20 * 256);
            sheet.SetColumnWidth(cellNum, 28 * 256);
            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"{zhName}覆盖区域模版 {DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UploadExcel(string productType, string coverType)
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
            var fileSize = file.ContentLength;
            if (fileSize > 200 * 1024)
            {
                return Json(new { Status = false, Msg = "文件大小不得超过200KB(不同品牌分批上传)" });
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
                var manager = new VendorProductCoverAreaManager();
                var user = User.Identity.Name;
                var fillResult = await manager.FillConvertCoverArea(convertResult.Item1, productType, coverType);
                if (!string.IsNullOrEmpty(fillResult.Item2))
                {
                    return Json(new { Status = false, Msg = fillResult.Item2 });
                }
                var result = await manager.UploadBatterCoverArea(fillResult.Item1, user);
                return Json(new { Status = result, Msg = $"导入{(result ? "成功" : "失败")}" });
            }
        }

        /// <summary>
        /// 导出蓄电池覆盖区域--按品牌
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public async Task<ActionResult> ExportData(string productType, string brand)
        {
            if (string.IsNullOrEmpty(brand))
            {
                return Json(new { Status = false, Msg = "请选择品牌" }, JsonRequestBehavior.AllowGet);
            }
            var workbook = new XSSFWorkbook();
            var index = 0;
            var sheet = workbook.CreateSheet();
            var row = sheet.CreateRow(index++);
            var cellNum = 0;
            row.CreateCell(cellNum++).SetCellValue("省份名称");
            row.CreateCell(cellNum++).SetCellValue("城市名称");
            row.CreateCell(cellNum++).SetCellValue("区域名称");
            row.CreateCell(cellNum++).SetCellValue("品牌");
            row.CreateCell(cellNum++).SetCellValue("是否启用TRUE/FALSE");
            row.CreateCell(cellNum).SetCellValue("备注(选填)");
            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 10 * 256);
            sheet.SetColumnWidth(cellNum++, 10 * 256);
            sheet.SetColumnWidth(cellNum++, 10 * 256);
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 20 * 256);
            sheet.SetColumnWidth(cellNum, 28 * 256);
            var pager = new PagerModel()
            {
                CurrentPage = 1,
                PageSize = int.MaxValue
            };
            var manager = new VendorProductCoverAreaManager();
            var list = (await manager.SelectVendorProductCoverArea
                    (productType, "Brand", brand, string.Empty, 0, 0, 0, pager))?.Item1;
            if (list != null && list.Any())
            {
                foreach (var item in list)
                {
                    row = sheet.CreateRow(index++);
                    cellNum = 0;
                    row.CreateCell(cellNum++).SetCellValue(item.ProvinceName);
                    row.CreateCell(cellNum++).SetCellValue(item.CityName);
                    row.CreateCell(cellNum++).SetCellValue(item.DistrictName);
                    row.CreateCell(cellNum++).SetCellValue(item.Brand);
                    row.CreateCell(cellNum++).SetCellValue(item.IsEnabled);
                    row.CreateCell(cellNum).SetCellValue(item.Remark);
                }
            }
            var ms = new MemoryStream();
            workbook.Write(ms);
            var zhName = manager.GetZhNameByProductType(productType);
            return File(ms.ToArray(), "application/x-xls", $"{zhName}产品{brand}品牌覆盖区域数据" +
                $"{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> RemoveCache(VendorProductCoverAreaModel model)
        {
            if (string.IsNullOrEmpty(model?.ProductType) || model.CoverRegionId < 1)
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new VendorProductCoverAreaManager();
            var result = await manager.RemoveCache(new List<VendorProductCoverAreaModel>(1) { model });
            return Json(new { Status = result, Msg = $" 清除缓存{(result ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Excel转为List<PaintDiscountConfigModel>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static Tuple<List<VendorProductCoverAreaViewModel>, string> ConvertExcelToList(HttpPostedFileBase file)
        {
            var message = string.Empty;
            var result = null as List<VendorProductCoverAreaViewModel>;
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
                else
                {
                    var repeatData = result.GroupBy(s => new { s.ProvinceName, s.CityName, s.DistrictName, s.Brand })
                        .Where(s => s.Count() > 1).Select(s => $"{s.Key.ProvinceName},{s.Key.CityName},{s.Key.DistrictName},{s.Key.Brand}");
                    if (repeatData != null && repeatData.Any())
                    {
                        message = "Excel中存在重复数据:\n" + string.Join("\n", repeatData);
                    }
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
        private static Tuple<List<VendorProductCoverAreaViewModel>, string> ConvertExcelToList(ISheet sheet)
        {
            var result = new List<VendorProductCoverAreaViewModel>();
            var message = string.Empty;
            //Excel单元格处理
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
                    else if (cell.CellType == CellType.Boolean)
                    {
                        return cell.BooleanCellValue.ToString();
                    }
                    return cell.StringCellValue;
                }
                return null;
            };
            //验证蓄电池覆盖区域数据
            Func<VendorProductCoverAreaViewModel, string> ValidateCoverArea = model =>
            {
                var valiResult = string.Empty;
                if (model == null)
                {
                    valiResult = "未知的对象";
                }
                else if (string.IsNullOrWhiteSpace(model.ProvinceName))
                {
                    valiResult = "缺失省份参数";
                }
                else if (string.IsNullOrWhiteSpace(model.CityName))
                {
                    valiResult = "缺失城市参数";
                }
                else if (string.IsNullOrWhiteSpace(model.DistrictName))
                {
                    valiResult = "缺失区域参数";
                }
                else if (string.IsNullOrWhiteSpace(model.Brand))
                {
                    valiResult = "缺失品牌参数";
                }
                return valiResult;
            };
            var titleRow = sheet.GetRow(sheet.FirstRowNum);
            var index = titleRow.FirstCellNum;
            if (titleRow.GetCell(index++)?.StringCellValue == "省份名称" &&
                titleRow.GetCell(index++)?.StringCellValue == "城市名称" &&
                titleRow.GetCell(index++)?.StringCellValue == "区域名称" &&
                titleRow.GetCell(index++)?.StringCellValue == "品牌" &&
                titleRow.GetCell(index++)?.StringCellValue == "是否启用TRUE/FALSE" &&
                titleRow.GetCell(index)?.StringCellValue == "备注(选填)")
            {
                var nowTime = DateTime.Now;
                for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;
                    var cellIndex = row.FirstCellNum;
                    var provinceName = getStringValue(row.GetCell(cellIndex++));
                    var cityName = getStringValue(row.GetCell(cellIndex++));
                    var districtName = getStringValue(row.GetCell(cellIndex++));
                    var brand = getStringValue(row.GetCell(cellIndex++));
                    var isEnabledStr = getStringValue(row.GetCell(cellIndex++));
                    var serviceRemark = getStringValue(row.GetCell(cellIndex));
                    bool isEnabled = false;
                    if (string.IsNullOrWhiteSpace(isEnabledStr) || !bool.TryParse(isEnabledStr, out isEnabled))
                    {
                        message = $"第{(rowIndex + 1)}行,缺失是否启用参数";
                        break;
                    }
                    var model = new VendorProductCoverAreaViewModel()
                    {
                        ProvinceName = provinceName,
                        CityName = cityName,
                        DistrictName = districtName,
                        Brand = brand,
                        IsEnabled = isEnabled,
                        Remark = serviceRemark
                    };
                    var validatedResult = ValidateCoverArea(model);//初步验证数据
                    if (!string.IsNullOrEmpty(validatedResult))
                    {
                        message = $"第{(rowIndex + 1)}行,{validatedResult}";
                        break;
                    }
                    else
                    {
                        result.Add(model);
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