using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.CompanyClient;
using Tuhu.Provisioning.Business.ContinentalConfig;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class MaPaiVipController : Controller
    {
        public ActionResult MaPaiVip()
        {
            return View();
        }

        /// <summary>
        /// 马牌券码
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult _MaPaiVipCouponCode(string keyword, int pageIndex = 1, int pageSize = 20)
        {
            MaPaiVipManager manager = new MaPaiVipManager();

            var result = manager.GetContinentalActivityList(keyword.Trim(), pageIndex, pageSize);

            ViewBag.TotalCount = (result != null && result.Any()) ? result.FirstOrDefault().Total : 0;

            return View(result);
        }

        /// <summary>
        /// 增加券码
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public JsonResult InsertCouponCodeConfig(string couponCode)
        {
            MaPaiVipManager manager = new MaPaiVipManager();
            var result = false;
            var msg = "券码已存在";

            var data = manager.SelectContinentalConfigInfoByCouponCode(couponCode.Trim());

            if (data == null)
            {
                result = manager.InsertCouponCodeConfig(couponCode.Trim(), HttpContext.User.Identity.Name);
                msg = result ? "操作成功" : "操作失败";
            }

            if (result)
            {
                return Json(new { status = "success", msg = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail", msg = msg }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        public JsonResult UploadFile()
        {
            var success = new List<MaPaiVipModel>();
            var error = new List<MaPaiVipModel>();
            var msg = string.Empty;
            MaPaiVipManager manager = new MaPaiVipManager();
            var data = new List<MaPaiVipModel>();
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
                    data = ConvertList(dataTable);
                    var verifyData = manager.VerifyData(data);
                    success = verifyData.Item1;
                    error = verifyData.Item2;
                    msg = verifyData.Item3;
                }
                else
                {
                    msg = "文件为空或是文件上传格式不正确";
                }
            }

            return Json(new { SuccessData = success, ErrorData = error, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量操作券码
        /// </summary>
        /// <param name="couponCodeInfo"></param>
        /// <returns></returns>
        public JsonResult BatchOperatorCouponCodeConfig(string couponCodeInfo)
        {
            MaPaiVipManager manager = new MaPaiVipManager();
            var result = false;
            if (!string.IsNullOrEmpty(couponCodeInfo))
            {
                var data = JsonConvert.DeserializeObject<List<MaPaiVipModel>>(couponCodeInfo);
                result = manager.BatchOperatorCouponCodeConfig(data);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量删除券码
        /// </summary>
        /// <param name="pkidStr"></param>
        /// <returns></returns>
        public JsonResult BatchDeleteConfig(string pkidStr)
        {
            MaPaiVipManager manager = new MaPaiVipManager();
            var result = false;
            if (!string.IsNullOrEmpty(pkidStr))
            {
                result = manager.BatchDeleteConfig(pkidStr);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新券码状态
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult UpdateContinentalConfigStatus(long pkid, int status)
        {
            MaPaiVipManager manager = new MaPaiVipManager();
            var result = false;
            result = manager.UpdateContinentalConfigStatus(pkid, status, HttpContext.User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <returns></returns>
        public ActionResult DownLoadFile()
        {
            //创建Excel文件的对象
            HSSFWorkbook book = new HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //获取list数据
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("专属券码");
            row1.CreateCell(1).SetCellValue("添加/删除");
            var fileName = "马牌专属券码配置模板.xls";
            // 写入到客户端 
            MemoryStream ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public JsonResult SelectOperationLog(string objectId)
        {
            CompanyClientManager manager = new CompanyClientManager();
            var result = manager.SelectOperationLog(objectId, "MaPaiVip");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region 私有方法
        private List<MaPaiVipModel> ConvertList(DataTable dataTable)
        {
            var result = new List<MaPaiVipModel>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new MaPaiVipModel()
                    {
                        UniquePrivilegeCode = row["专属券码"].ToString().Trim(),
                        IsDeleted = string.Equals(row["添加/删除"].ToString().Trim(), "删除", 
                        StringComparison.CurrentCultureIgnoreCase) ? true : false,
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
        #endregion
    }
}