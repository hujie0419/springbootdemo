using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.Dianping;
using Tuhu.Provisioning.DataAccess.Entity.Dianping;

namespace Tuhu.Provisioning.Controllers
{
    public class DianpingController : Controller
    {
        private DianpingManager dianpingManager;

        public DianpingController()
        {
            this.dianpingManager = new DianpingManager();
        }

        #region Group Config

        /// <summary>
        /// 获取团购配置信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="dianpingId"></param>
        /// <param name="dianpingBrand"></param>
        /// <param name="dianpingName"></param>
        /// <param name="tuhuPid"></param>
        /// <param name="tuhuStatus"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        public JsonResult GetGroupConfigs(int pageIndex, int pageSize,
            string dianpingId, string dianpingBrand, string dianpingName, string tuhuPid, int tuhuStatus)
        {
            var data = dianpingManager.GetGroupConfigs(pageIndex, pageSize,
                dianpingId, dianpingBrand, dianpingName, tuhuPid, tuhuStatus);
            var totalCount = dianpingManager.GetGroupConfigsCount(dianpingId, dianpingBrand, dianpingName, tuhuPid, tuhuStatus);
            return Json(new { pageIndex = pageIndex, pageSize = pageSize, totalCount = totalCount, data = data },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取tuhu产品名称
        /// </summary>
        /// <param name="tuhuProductId"></param>
        /// <returns></returns>
        public JsonResult GetTuhuProductName(string tuhuProductId)
        {
            var name = dianpingManager.GetTuhuProductName(tuhuProductId);

            return Json(new { name = name }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 插入或更新团购信息
        /// </summary>
        /// <param name="dianpingGroupId"></param>
        /// <param name="dianpingGroupName"></param>
        /// <param name="dianpingBrand"></param>
        /// <param name="tuhuProductId"></param>
        /// <param name="tuhuProductStatus"></param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage(IwSystem = "OperateSys")]
        public JsonResult UpdateGroupConfig(string dianpingGroupId, string dianpingTuanName, string dianpingBrand,
            string tuhuProductId, int tuhuProductStatus)
        {
            int result = dianpingManager.UpdateGroupConfig(new DataAccess.Entity.Dianping.DianpingGroupConfig()
            {
                DianpingGroupId = dianpingGroupId,
                DianpingBrand = dianpingBrand,
                DianpingTuanName = dianpingTuanName,
                TuhuProductId = tuhuProductId,
                TuhuProductStatus = tuhuProductStatus
            }, User.Identity.Name);

            string message = "更新成功";
            if (result == -1)
            {
                message = "团购不存在";
            }
            else if (result == 0)
            {
                message = "更新失败";
            }

            return Json(new { success = result == 1, msg = message });
        }

        /// <summary>
        /// 插入或更新团购信息
        /// </summary>
        /// <param name="dianpingGroupId"></param>
        /// <param name="dianpingGroupName"></param>
        /// <param name="dianpingBrand"></param>
        /// <param name="tuhuProductId"></param>
        /// <param name="tuhuProductStatus"></param>
        /// <returns></returns>
        [HttpPost]
        [PowerManage(IwSystem = "OperateSys")]
        public JsonResult InsertGroupConfig(string dianpingGroupId, string dianpingTuanName, string dianpingBrand,
            string tuhuProductId, int tuhuProductStatus)
        {
            int result = dianpingManager.InsertGroupConfig(new DataAccess.Entity.Dianping.DianpingGroupConfig()
            {
                DianpingGroupId = dianpingGroupId,
                DianpingBrand = dianpingBrand,
                DianpingTuanName = dianpingTuanName,
                TuhuProductId = tuhuProductId,
                TuhuProductStatus = tuhuProductStatus
            }, User.Identity.Name);

            string message = "新增成功";
            if (result == -1)
            {
                message = "团购已存在";
            }
            else if (result == 0)
            {
                message = "新增失败";
            }
            return Json(new { success = result == 1, msg = message });
        }

        [HttpPost]
        [PowerManage(IwSystem = "OperateSys")]
        public JsonResult DeleteGroupConfig(string dianpingGroupId)
        {
            bool result = dianpingManager.DeleteGroupConfig(dianpingGroupId, User.Identity.Name);

            return Json(new { success = result });
        }

        #endregion

        #region Shop Config

        /// <summary>
        /// 获取门店配置
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="dianpingId"></param>
        /// <param name="dianpingName"></param>
        /// <param name="dianpingShopName"></param>
        /// <param name="tuhuShopId"></param>
        /// <param name="groupStatus"></param>
        /// <returns></returns>
        [PowerManage(IwSystem = "OperateSys")]
        public JsonResult GetShopConfigs(int pageIndex, int pageSize,
            string dianpingId, string dianpingName, string dianpingShopName, string tuhuShopId, 
            int groupStatus, int linkStatus)
        {
            var data = dianpingManager.GetShopConfigs(pageIndex, pageSize,
                dianpingId, dianpingName, dianpingShopName, tuhuShopId, groupStatus, linkStatus);
            var totalCount = dianpingManager.GetShopConfigsCount(dianpingId, dianpingName,
                dianpingShopName, tuhuShopId, groupStatus, linkStatus);
            return Json(new { pageIndex = pageIndex, pageSize = pageSize, totalCount = totalCount, data = data },
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTuhuShopName(int tuhuShopId)
        {
            var name = dianpingManager.GetTuhuShopName(tuhuShopId);

            return Json(new { name = name }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PowerManage(IwSystem = "OperateSys")]
        public JsonResult UpdateShopConfig(string dianpingId, string dianpingName, 
            string dianpingShopName, int tuhuShopId, int groupStatus)
        {
            int result = dianpingManager.UpdateShopConfig(new DianpingShopConfig()
            {
                DianpingId = dianpingId,
                DianpingName = dianpingName,
                DianpingShopName = dianpingShopName,
                TuhuShopId = tuhuShopId,
                GroupStatus = groupStatus
            }, User.Identity.Name);

            string message = "更新成功";
            if (result == -1)
            {
                message = "配置不存在";
            }
            else if (result == 0)
            {
                message = "更新失败";
            }

            return Json(new { success = result == 1, msg = message });
        }
        
        [HttpPost]
        [PowerManage(IwSystem = "OperateSys")]
        public JsonResult InsertShopConfig(string dianpingId, string dianpingName,
            string dianpingShopName, int tuhuShopId, int groupStatus)
        {
            int result = dianpingManager.InsertShopConfig(new DianpingShopConfig()
            {
                DianpingId = dianpingId,
                DianpingName = dianpingName,
                DianpingShopName = dianpingShopName,
                TuhuShopId = tuhuShopId,
                GroupStatus = groupStatus
            }, User.Identity.Name);

            string message = "新增成功";
            if (result == -1)
            {
                message = "点评门店或tuhu门店已存在";
            }
            else if (result == 0)
            {
                message = "新增失败";
            }
            return Json(new { success = result == 1, msg = message });
        }

        [HttpPost]
        [PowerManage(IwSystem = "OperateSys")]
        public JsonResult DeleteShopConfig(string dianpingId)
        {
            bool result = dianpingManager.DeleteShopConfig(dianpingId, User.Identity.Name);

            return Json(new { success = result });
        }

        #endregion

        public FileResult ExportSample(string type)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var row = sheet.CreateRow(0);

            #region title

            if (string.Equals(type, "groupconfig"))
            {
                var cellNum = 0;
                row.CreateCell(cellNum++).SetCellValue("点评团购ID");
                row.CreateCell(cellNum++).SetCellValue("点评品牌名称");
                row.CreateCell(cellNum++).SetCellValue("点评团购名称");
                row.CreateCell(cellNum++).SetCellValue("途虎服务产品ID");
                row.CreateCell(cellNum++).SetCellValue("途虎服务状态");

                cellNum = 0;
                sheet.SetColumnWidth(cellNum++, 20 * 256);
                sheet.SetColumnWidth(cellNum++, 20 * 256);
                sheet.SetColumnWidth(cellNum++, 20 * 256);
                sheet.SetColumnWidth(cellNum++, 20 * 256);
                sheet.SetColumnWidth(cellNum++, 20 * 256);
            }
            else
            {
                var cellNum = 0;
                row.CreateCell(cellNum++).SetCellValue("点评门店ID");
                row.CreateCell(cellNum++).SetCellValue("点评商户名");
                row.CreateCell(cellNum++).SetCellValue("点评分店名");
                row.CreateCell(cellNum++).SetCellValue("途虎门店ID");
                row.CreateCell(cellNum++).SetCellValue("点评团购");

                cellNum = 0;
                sheet.SetColumnWidth(cellNum++, 20 * 256);
                sheet.SetColumnWidth(cellNum++, 20 * 256);
                sheet.SetColumnWidth(cellNum++, 20 * 256);
                sheet.SetColumnWidth(cellNum++, 20 * 256);
                sheet.SetColumnWidth(cellNum++, 20 * 256);
            }

            #endregion

            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/x-xls", $"模板.xlsx");
        }

        [HttpPost]
        [PowerManage(IwSystem = "OperateSys")]
        public JsonResult Upload(string type)
        {
            var files = Request.Files;
            string msg = "上传失败";
            if(files.Count == 1)
            {
                var file = files[0];
                if (file.ContentType != "application/vnd.ms-excel" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    msg = "请上传Excel文件";
                }
                else
                {
                    using (var stream = file.InputStream)
                    {
                        var buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        var workBook = new XSSFWorkbook(new MemoryStream(buffer));
                        var sheet = workBook.GetSheetAt(0);
                        if (string.Equals(type, "groupconfig"))
                        {
                            var convertResult = ConvertSheetToGroupConfig(sheet);
                            if (!string.IsNullOrEmpty(convertResult.Item1))
                            {
                                msg = convertResult.Item1;
                            }
                            else if (convertResult.Item2 == null || !convertResult.Item2.Any())
                            {
                                msg = "文件不能为空";
                            }
                            else if (convertResult.Item2.Distinct().Count() != convertResult.Item2.Count)
                            {
                                msg = "存在重复数据";
                            }
                            else
                            {
                                var result = dianpingManager.BulkInsertGroupConfig(convertResult.Item2, User.Identity.Name);
                                if (result != null)
                                {
                                    string reason = result.Item2 == -1 ? "团购已存在" : "新增失败";
                                    msg = $"点评id：{result.Item1} 导入失败, 原因：{reason}";
                                }
                                else
                                {
                                    msg = string.Empty;
                                }
                            }
                        }else if (string.Equals(type, "shopconfig"))
                        {
                            var convertResult = ConvertSheetToShopConfig(sheet);
                            if (!string.IsNullOrEmpty(convertResult.Item1))
                            {
                                msg = convertResult.Item1;
                            }
                            else if (convertResult.Item2 == null || !convertResult.Item2.Any())
                            {
                                msg = "文件不能为空";
                            }
                            else if (convertResult.Item2.Distinct().Count() != convertResult.Item2.Count)
                            {
                                msg = "存在重复数据";
                            }
                            else
                            {
                                var result = dianpingManager.BulkInsertShopConfig(convertResult.Item2, User.Identity.Name);
                                if (result != null)
                                {
                                    string reason = result.Item2 == -1 ? "团购已存在" : "新增失败";
                                    msg = $"点评id：{result.Item1} 导入失败, 原因：{reason}";
                                }
                                else
                                {
                                    msg = string.Empty;
                                }
                            }
                        }
                    }

                }
            }
            else
            {
                msg = "请先上传文件";
            }

            return Json(new { msg = msg, success = string.IsNullOrEmpty(msg) });
        }

        private Func<ICell, string> getStringValue = cell =>
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

        private Tuple<string, List<DianpingGroupConfig>> ConvertSheetToGroupConfig(ISheet sheet)
        {
            var result = new List<DianpingGroupConfig>();
            var message = string.Empty;

            var titleRow = sheet.GetRow(sheet.FirstRowNum);
            var index = titleRow.FirstCellNum;
            if (string.Equals(titleRow.GetCell(index++)?.StringCellValue, "点评团购ID") &&
                string.Equals(titleRow.GetCell(index++)?.StringCellValue, "点评品牌名称") &&
                string.Equals(titleRow.GetCell(index++)?.StringCellValue, "点评团购名称") &&
                string.Equals(titleRow.GetCell(index++)?.StringCellValue, "途虎服务产品ID") &&
                string.Equals(titleRow.GetCell(index++)?.StringCellValue, "途虎服务状态"))
            {
                for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;
                    var cellIndex = row.FirstCellNum;
                    var item = new DianpingGroupConfig()
                    {
                        DianpingGroupId = getStringValue(row.GetCell(cellIndex++)),
                        DianpingBrand = getStringValue(row.GetCell(cellIndex++)),
                        DianpingTuanName = getStringValue(row.GetCell(cellIndex++)),
                        TuhuProductId = getStringValue(row.GetCell(cellIndex++)),
                        TuhuProductStatus = string.Equals(getStringValue(row.GetCell(cellIndex)), "已上架") ? 1 : 0
                    };
                    if (!string.IsNullOrEmpty(item.DianpingGroupId) && !string.IsNullOrEmpty(item.DianpingBrand) &&
                        !string.IsNullOrEmpty(item.DianpingTuanName) && !string.IsNullOrEmpty(item.TuhuProductId) &&
                        !string.IsNullOrEmpty(dianpingManager.GetTuhuProductName(item.TuhuProductId)))
                    {
                        result.Add(item);
                    }
                    else
                    {
                        message = $"第{rowIndex + 1}行数据错误";
                        break;
                    }
                }
            }
            else
            {
                message = "文件与模板不一致";
            }

            return Tuple.Create(message, result);
        }

        private Tuple<string, List<DianpingShopConfig>> ConvertSheetToShopConfig(ISheet sheet)
        {
            var result = new List<DianpingShopConfig>();
            var message = string.Empty;

            var titleRow = sheet.GetRow(sheet.FirstRowNum);
            var index = titleRow.FirstCellNum;
            if (string.Equals(titleRow.GetCell(index++)?.StringCellValue, "点评门店ID") &&
                string.Equals(titleRow.GetCell(index++)?.StringCellValue, "点评商户名") &&
                string.Equals(titleRow.GetCell(index++)?.StringCellValue, "点评分店名") &&
                string.Equals(titleRow.GetCell(index++)?.StringCellValue, "途虎门店ID") &&
                string.Equals(titleRow.GetCell(index++)?.StringCellValue, "点评团购"))
            {
                for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;
                    var cellIndex = row.FirstCellNum;
                    var dianpingId = getStringValue(row.GetCell(cellIndex++));
                    var dianpingName = getStringValue(row.GetCell(cellIndex++));
                    var dianpingShopName = getStringValue(row.GetCell(cellIndex++));
                    var idStr = getStringValue(row.GetCell(cellIndex++));
                    int tuhuShopId = 0;
                    Int32.TryParse(idStr, out tuhuShopId);
                    var groupStatus = getStringValue(row.GetCell(cellIndex));
                    var item = new DianpingShopConfig()
                    {
                        DianpingId = dianpingId,
                        DianpingName = dianpingName,
                        DianpingShopName = dianpingShopName,
                        TuhuShopId = tuhuShopId,
                        GroupStatus = string.Equals(groupStatus, "已配置") ? 1 : 0
                    };
                    if (!string.IsNullOrEmpty(item.DianpingId) && !string.IsNullOrEmpty(item.DianpingName) &&
                        !string.IsNullOrEmpty(item.DianpingShopName) && item.TuhuShopId > 0 &&
                        !string.IsNullOrEmpty(dianpingManager.GetTuhuShopName(item.TuhuShopId)))
                    {
                        result.Add(item);
                    }
                    else
                    {
                        message = $"第{rowIndex + 1}行数据错误";
                        break;
                    }
                }
            }
            else
            {
                message = "文件与模板不一致";
            }

            return Tuple.Create(message, result);
        }
    }
}