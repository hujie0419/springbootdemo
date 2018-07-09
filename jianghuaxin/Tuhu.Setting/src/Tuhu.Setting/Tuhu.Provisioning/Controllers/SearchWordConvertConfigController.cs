using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Tuhu.Provisioning.Business.ProductVehicleType;
using Tuhu.Provisioning.Business.SearchWordConvertMapConfig;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class SearchWordConvertConfigController : Controller
    {
        private readonly ISearchWordConvertMgr _iswcmgr = new SearchWordConvertMgr();

        public ActionResult Index(string msg)
        {
            ViewBag.error = msg;
            return View();
        }

        /// <summary>
        /// 上传Excel文件处理
        /// </summary>
        /// <param name="filebase"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FileImport(HttpPostedFileBase filebase)
        {
            HttpPostedFileBase file = Request.Files["files"];

            if (file == null || file.ContentLength <= 0)
            {
                return RedirectToAction("Index", "SearchWordConvertConfig", new { msg = "文件不能为空" });
            }
            var filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
            var ext = System.IO.Path.GetExtension(file.FileName);//获取上传文件的扩展名
            const int maxSize = 6000 * 1024; //定义上传文件的最大空间大小为6M
            const string fileType = ".xls,.xlsx"; //定义上传文件的类型字符串

            if (string.IsNullOrWhiteSpace(ext))
            {
                return RedirectToAction("Index", "SearchWordConvertConfig", new { msg = "文件扩展名不能为空" });
            }
            if (!fileType.Contains(ext))
            {
                return RedirectToAction("Index", "SearchWordConvertConfig", new { msg = "文件类型不对，只能导入xls和xlsx格式的文件" });
            }
            if (filesize >= maxSize)
            {
                return RedirectToAction("Index", "SearchWordConvertConfig", new { msg = "上传文件超过6M，不能上传" });
            }

            var table = new DataTable();

            var bytes = StreamToBytes(file.InputStream);

            using (var memstream = new MemoryStream(bytes))
            {
                //file.InputStream.CopyTo(memstream);
                //memstream.Position = 0; // <-- Add this, to make it work

                switch (ext)
                {
                    case ".xlsx":
                        var wb1 = new XSSFWorkbook(memstream);
                        table = GetExcelData(wb1);
                        break;
                    case ".xls":
                        var wb2 = new HSSFWorkbook(memstream);
                        table = GetExcelData(wb2);
                        break;
                    default:
                        break;
                }
            }

            var existConfigList = _iswcmgr.GetAllSearchWord();
            var deleteList = new List<SearchWordConvertMapDb>();

            var delRow = new List<DataRow>();
            for (var i = table.Rows.Count - 1; i >= 0; i--)
            {
                var targetWord = table.Rows[i][0].ToString();
                var sourceWord = table.Rows[i][1].ToString();
                var canUse = table.Rows[i][2].ToString();//是否删除
                var existEntity = existConfigList.Find(t => t.TargetWord == targetWord && t.SourceWord == sourceWord);

                if (canUse == "0")
                {
                    //被标记删除
                    if (existEntity != null)
                    {
                        deleteList.Add(existEntity);
                    }

                    delRow.Add(table.Rows[i]);
                }
                else
                {
                    //待插入数据
                    if (existEntity != null)
                    {
                        delRow.Add(table.Rows[i]);//避免重复插入
                    }
                }
            }
            foreach (var dr in delRow)
            {
                table.Rows.Remove(dr);
            }
            //执行删除配置操作
            _iswcmgr.DeleteSearchWord(deleteList);
            //执行插入操作
            _iswcmgr.ImportExcelInfoToDb(table);

            return RedirectToAction("Index", "SearchWordConvertConfig", new { msg = "导入成功" });
        }

        /// <summary>
        /// 下载当前最新配置
        /// </summary>
        /// <returns></returns>
        public ActionResult DownLoadFile()
        {
            var sourceList = _iswcmgr.GetAllSearchWord();
            //创建Excel文件的对象
            var book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            var sheet1 = book.CreateSheet("Sheet1");
            //获取list数据
            //给sheet1添加第一行的头部标题
            var row1 = sheet1.CreateRow(0);
            var fileName = "最新配置-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(999) + ".xls";

            row1.CreateCell(0).SetCellValue("关键词");
            row1.CreateCell(1).SetCellValue("同义词");
            row1.CreateCell(2).SetCellValue("是否删除");
            for (var i = 0; i < sourceList.Count; i++)
            {
                var rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(sourceList[i].TargetWord);
                rowtemp.CreateCell(1).SetCellValue(sourceList[i].SourceWord);
                rowtemp.CreateCell(2).SetCellValue("1");
            }

            // 写入到客户端 
            byte[] file;
            using (var ms = new System.IO.MemoryStream())
            {
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                file = ms.ToArray();
            }
            return File(file, "application/vnd.ms-excel", fileName);
        }

        public byte[] StreamToBytes(Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            stream.Position = 0;
            return bytes;
        }

        public DataTable GetExcelData(IWorkbook wb)
        {
            var dt = new DataTable();
            var sheet = wb.GetSheetAt(0);//获取excel第一个sheet
            var headerRow = sheet.GetRow(0);//获取sheet首行

            int cellCount = headerRow.LastCellNum;//获取总列数

            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                var column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                dt.Columns.Add(column);
            }

            for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                var dataRow = dt.NewRow();
                var itemHandle = row.GetCell(cellCount - 1);
                if (itemHandle == null)
                {
                    continue;
                }

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }

            return dt;
        }

    }
}
