using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.NuomiService;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class NuomiServiceController : Controller
    {
        public ActionResult NuomiServicesConfig()
        {
            ViewBag.UserEmailStr = ConfigurationManager.AppSettings["NuomiUserEmail"].ToString();
            return View();
        }

        public ActionResult _NuomiServiceConfigList(string nuomiTitle, long nuomiId, string serviceId, int isValid, int pageIndex = 1, int pageSize = 20)
        {
            NuomiServiceManager manager = new NuomiServiceManager();

            var result = manager.GetNuomiServicesConfig(nuomiTitle.Trim(), nuomiId, serviceId.Trim(), isValid, pageIndex, pageSize);

            ViewBag.TotalCount = (result != null && result.Any()) ? result.FirstOrDefault().Total : 0;

            return View(result);
        }

        public JsonResult GetServiceNameByServiceId(string serviceId)
        {
            NuomiServiceManager manager = new NuomiServiceManager();

            var result = manager.GetServiceNameByServiceId(serviceId.Trim());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddNuomiServiceConfig(string nuomiTitle, long nuomiId, string serviceId, string userEmail, string remarks)
        {
            NuomiServiceManager manager = new NuomiServiceManager();
            var user = HttpContext.User.Identity.Name;

            var result = manager.AddNuomiServiceConfig(nuomiTitle.Trim(), nuomiId, serviceId.Trim(), userEmail.Trim(), remarks.Trim(), user);

            return Json(new { status = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditNuomiServiceConfig(int pkid, string nuomiTitle, long nuomiId, string serviceId, string userEmail, string remarks)
        {
            NuomiServiceManager manager = new NuomiServiceManager();
            var user = HttpContext.User.Identity.Name;

            var result = manager.EditNuomiServiceConfig(pkid, nuomiTitle.Trim(), nuomiId, serviceId.Trim(), userEmail.Trim(), remarks.Trim(), user);

            if (result)
            {
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DelNuomiServiceConfig(int pkid)
        {
            NuomiServiceManager manager = new NuomiServiceManager();
            var user = HttpContext.User.Identity.Name;

            var result = manager.DelNuomiServiceConfig(pkid, user);

            if (result)
            {
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UploadFile()
        {
            var success = new List<NuomiServicesConfig>();
            var error = new List<NuomiServicesConfig>();
            var msg = string.Empty;
            var userEmail = ConfigurationManager.AppSettings["NuomiUserEmail"].ToString();
            NuomiServiceManager manager = new NuomiServiceManager();
            var data = new List<NuomiServicesConfig>();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                DataTable dataTable = null;
                string fileExtension = Path.GetExtension(file.FileName); // 文件扩展名
                bool isxlsx = fileExtension.Equals(".xlsx");
                bool isxls = fileExtension.Equals(".xls");
                if (file != null && (isxls || isxlsx))
                {
                    using (Stream writer = file.InputStream)
                    {
                        if (isxlsx)
                        {
                            dataTable = RenderDataTableForXLSX(writer, 0);
                        }
                        if (isxls)
                        {
                            dataTable = RenderDataTableForXLS(writer, 0);
                        }
                    }
                    data = ConvertList(dataTable, userEmail);
                    var allServices = NuomiServiceManager.GetAllShopServices();
                    var verifyData = manager.VerifyData(data, allServices);
                    success = verifyData.Item1;
                    error = verifyData.Item2;
                }
                else
                {
                    msg = "文件为空或是文件上传格式不正确";
                }
            }

            return Json(new { SuccessData = success, ErrorData = error, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BatchInsertNuomiConfig(string nuomiInfo)
        {
            var result = false;
            NuomiServiceManager manager = new NuomiServiceManager();

            if (!string.IsNullOrEmpty(nuomiInfo))
            {
                var data = JsonConvert.DeserializeObject<List<NuomiServicesConfig>>(nuomiInfo);

                if (data != null && data.Any())
                {
                    result = manager.BatchInsertNuomiConfig(data, HttpContext.User.Identity.Name);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<NuomiServicesConfig> ConvertList(DataTable dataTable, string email)
        {
            var result = new List<NuomiServicesConfig>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new NuomiServicesConfig()
                    {
                        NuomiTitle = row["糯米团购项目名称"].ToString().Trim(),
                        NuomiId = Convert.ToInt64(row["糯米团购ID"].ToString().Trim()),
                        Email = email,
                        ServiceId = row["途虎服务产品ID"].ToString().Trim(),
                        Remarks = row["团购配置备注"].ToString().Trim()
                    });
                }
            }
            return result;
        }

        private DataTable RenderDataTableForXLSX(Stream excelFileStream, int headerRowIndex)
        {
            DataTable table = new DataTable();

            XSSFWorkbook workbook = new XSSFWorkbook(excelFileStream);
            var sheet = workbook.GetSheetAt(0);
            var headerRow = sheet.GetRow(headerRowIndex);
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue.Trim(), typeof(string));
                table.Columns.Add(column);
            }

            int rowCount = sheet.LastRowNum;
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row != null && row.FirstCellNum >= 0)
                {
                    DataRow dataRow = table.NewRow();
                    bool hasValue = false;

                    for (int j = row.FirstCellNum; j < row.Cells.Count; j++)
                    {
                        if (row.GetCell(j) != null && !string.IsNullOrEmpty(row.GetCell(j).ToString()))
                        {
                            hasValue = true;
                            dataRow[j] = row.GetCell(j).ToString().Trim();
                        }
                    }
                    if (hasValue)
                    {
                        table.Rows.Add(dataRow);
                    }
                }
            }

            workbook = null;
            sheet = null;

            return table;
        }

        private DataTable RenderDataTableForXLS(Stream excelFileStream, int headerRowIndex)
        {
            DataTable table = new DataTable();

            HSSFWorkbook workbook = new HSSFWorkbook(excelFileStream);
            var sheet = workbook.GetSheetAt(0);
            var headerRow = sheet.GetRow(headerRowIndex);
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue.Trim(), typeof(string));
                table.Columns.Add(column);
            }

            int rowCount = sheet.LastRowNum;
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row != null && row.FirstCellNum >= 0)
                {
                    DataRow dataRow = table.NewRow();
                    bool hasValue = false;
                    for (int j = row.FirstCellNum; j < row.Cells.Count; j++)
                    {
                        if (row.GetCell(j) != null && !string.IsNullOrEmpty(row.GetCell(j).ToString()))
                        {
                            hasValue = true;
                            dataRow[j] = row.GetCell(j).ToString().Trim();
                        }
                    }
                    if (hasValue)
                    {
                        table.Rows.Add(dataRow);
                    }
                }
            }

            workbook = null;
            sheet = null;

            return table;
        }

        public JsonResult BatchSoleteConfig(string pkidStr)
        {
            var result = false;
            NuomiServiceManager manager = new NuomiServiceManager();

            if (!string.IsNullOrEmpty(pkidStr))
            {
                result = manager.BatchSoleteConfig(pkidStr, HttpContext.User.Identity.Name);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}