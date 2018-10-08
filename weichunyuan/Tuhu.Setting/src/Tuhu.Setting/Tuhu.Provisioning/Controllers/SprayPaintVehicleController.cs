using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business.SprayPaintVehicle;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Shop;
using System.Threading.Tasks;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;

namespace Tuhu.Provisioning.Controllers
{
    public class SprayPaintVehicleController:Controller
    {
        [PowerManage]
        public ActionResult SprayPaintVehicle()
        {
            var manager = new SprayPaintVehicleManager();
            var allConfigs = manager.GetVehicleLevelsForAll();
            var model = allConfigs.Item1;
            var offline = allConfigs.Item2;
            ViewBag.offline = offline;
            return View(model);
        }

        /// <summary>
        /// 修改喷漆车型档次分类
        /// </summary>
        /// <param name="vehicleLevel"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult EditSprayPaintVehicle(string vehicleLevel, string type)
        {
            if (string.IsNullOrWhiteSpace(vehicleLevel))
            {
                vehicleLevel = "A类";
            }
            if (string.IsNullOrWhiteSpace(type))
            {
                type = PaintVehicleLevelTypeEnum.Online.ToString();
            }
            var manager = new SprayPaintVehicleManager();
            ViewBag.VehicleLevel = vehicleLevel;
            ViewBag.ServiceInfo = manager.GetPaintServiceInfo();
            var model = new VehicleLevelModel();
            ViewBag.type = type;
            model = manager.SelectSprayPaintVehicleByLevel(type, vehicleLevel);
            return View(model);
        }

        public JsonResult GetVehicleInfo(string vehicleLevel, string initalWord, string type)
        {
            var manager = new SprayPaintVehicleManager();
            if (string.IsNullOrWhiteSpace(initalWord))
                initalWord = "A";
            if (string.IsNullOrWhiteSpace(type))
            {
                type = PaintVehicleLevelTypeEnum.Online.ToString();
            }
            var vehicleInfo = manager.GetVehicleInfo(type, initalWord, vehicleLevel);
            return Json(new { Status = true, items = vehicleInfo }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdatePaintVehicleLevel(string vehicleLevel, string initalWord, string vehicleIds, string type)
        {
            var manager = new SprayPaintVehicleManager();
            var operateUser = User.Identity.Name;
            if (string.IsNullOrWhiteSpace(vehicleLevel) || string.IsNullOrWhiteSpace(initalWord))
            {
                return Json(new { Status = false });
            }
            var result = manager.UpdatePaintVehicleLevel(vehicleLevel, vehicleIds, initalWord, type,
                operateUser);
            return Json(new { Status = result });
        }

        [HttpPost]
        public JsonResult UpdatePaintService(string vehicleLevel, string paintService)
        {
            var manager = new SprayPaintVehicleManager();            
            var operateUser = User.Identity.Name;
            if (string.IsNullOrWhiteSpace(vehicleLevel))
            {
                return Json(new { Status = false });
            }
            var result = manager.UpdatePaintVehicleService(vehicleLevel,
                paintService, operateUser);
            return Json(new { Status = result });
        }

        [HttpPost]
        public JsonResult UpdatePaintVehicleCache(string type, string data)
        {
            var manager = new SprayPaintVehicleManager();
            var result = manager.UpdatePaintVehicleCache(type, data);
            return Json(new {Status = result.Item1, Msg = result.Item2});
        }


        #region 根据vin码导出数据

        public ActionResult VinSprayPaint()
        {
            return View();
        }

        public FileResult DownloadVinSprayPaintFile()
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var row = sheet.CreateRow(0);
            var cell = null as ICell;
            var cellNum = 0;

            row.CreateCell(cellNum++).SetCellValue("用户手机号");
            row.CreateCell(cellNum++).SetCellValue("VIN码");

            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 14 * 256);
            sheet.SetColumnWidth(cellNum++, 8 * 256);


            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"模板文件 {DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        public async Task<ActionResult> SearchByExcel()
        {
            if (Request.Files.Count <= 0)
            {
                return Json(new { Status = false, Msg = "未选择文件, 请先上传文件" });
            }

            var file = Request.Files[0];
            if (file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return Json(new { Status = false, Msg = "文件格式不正确, 请上传Excel文件" });
            }

            byte[] buffer = null;
            using (var stream = file.InputStream)
            {
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
            }
            var list = ConvertToList(buffer, array => new { Mobile = array[0], Vin = array[1] });
            var vins = list.Select(x => x.Vin);

            var manager = new SprayPaintVehicleManager();
            var result = await manager.GetVinSprayPaintRelationResult(vins);
            result = (from x in list
                      join y in result on x.Vin equals y.Vin into temp
                      from z in temp.DefaultIfEmpty()
                      select new VinSprayPaintRelationship
                      {
                          Mobile = x.Mobile,
                          Vin = x.Vin,
                          SprayPaintLevel = z?.SprayPaintLevel?.OrderBy(x => x.VehicleLevel)?.ToList() ?? new List<VehicleSprayPaintLevel>(),
                      }).ToList();
            return Json(new { Status = true, Data = result });
        }

        private List<T> ConvertToList<T>(byte[] buffer, Func<List<string>, T> func) where T : class
        {
            var list = new List<T>();
            list.Clear();
            if (buffer != null && buffer.Any())
            {
                var workBook = new XSSFWorkbook(new MemoryStream(buffer));
                var sheet = workBook.GetSheetAt(0);
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
                    var array = new List<string>(row.LastCellNum - row.FirstCellNum + 1);
                    for (var cellIndex = row.FirstCellNum; cellIndex <= row.LastCellNum; cellIndex++)
                    {
                        array.Add(getStringValue(row.GetCell(cellIndex))?.Trim());
                    }
                    if (array.Any() && array.Any(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        list.Add(func(array));
                    }
                }
                list = list.Distinct().ToList();
            }
            return list;
        }

        public ActionResult ExportExcel(List<string> data)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var row = sheet.CreateRow(0);
            var cellNum = 0;

            row.CreateCell(cellNum++).SetCellValue("用户手机号");
            row.CreateCell(cellNum++).SetCellValue("VIN码");
            row.CreateCell(cellNum).SetCellValue("车型信息");
            row.CreateCell(cellNum+3).SetCellValue("喷漆喷档");
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, cellNum, cellNum + 2));

            cellNum = 0;
            sheet.SetColumnWidth(cellNum++, 15 * 256);
            sheet.SetColumnWidth(cellNum++, 20 * 256);
            sheet.SetColumnWidth(cellNum++, 20 * 256);
            sheet.SetColumnWidth(cellNum++, 20 * 256);
            sheet.SetColumnWidth(cellNum++, 20 * 256);
            sheet.SetColumnWidth(cellNum++, 20 * 256);

            int rowNum = 0;
            foreach (var str in data)
            {
                var item = JsonConvert.DeserializeObject<VinSprayPaintModel>(str);
                rowNum++;
                var start = rowNum;

                row = sheet.CreateRow(rowNum);
                cellNum = 0;
                row.CreateCell(cellNum++).SetCellValue(item.Mobile);
                row.CreateCell(cellNum++).SetCellValue(item.Vin);
                if (item.VehicleInfo != null && item.VehicleInfo.Any())
                {
                    row.CreateCell(cellNum).SetCellValue(item.VehicleInfo[0]?.VehicleID);
                    row.CreateCell(cellNum + 1).SetCellValue(item.VehicleInfo[0]?.Brand);
                    row.CreateCell(cellNum + 2).SetCellValue(item.VehicleInfo[0]?.Vehicle);
                    row.CreateCell(cellNum + 3).SetCellValue(item.VehicleLevel);
                    foreach (var item2 in item.VehicleInfo.Skip(1))
                    {
                        rowNum++;
                        row = sheet.CreateRow(rowNum);
                        row.CreateCell(cellNum).SetCellValue(item2?.VehicleID);
                        row.CreateCell(cellNum + 1).SetCellValue(item2?.Brand);
                        row.CreateCell(cellNum + 2).SetCellValue(item2?.Vehicle);
                    }
                }
                var end = rowNum;
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(start, end, 0, 0));
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(start, end, 1, 1));
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(start, end, 5, 5));
            }


            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"查询VIN码对应的喷漆等级{DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒")}.xlsx");
        }

        #endregion

    }
}