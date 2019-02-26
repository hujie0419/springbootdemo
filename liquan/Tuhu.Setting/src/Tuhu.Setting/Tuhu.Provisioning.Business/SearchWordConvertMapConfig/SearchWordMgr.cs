using System.IO;
using System.Web;
using System.Data;
using System.Collections.Generic;

using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.SearchWordConvertMapConfig
{
    /// <summary>
    /// 工厂类
    /// </summary>
    public abstract class SearchWordMgr
    {
        /// <summary>
        /// 导入
        /// </summary>
        public abstract int Import(HttpPostedFileBase file, List<SearchWordConvertMapDb> list, out byte[] bytes);

        /// <summary>
        /// 导出
        /// </summary>
        public abstract byte[] Export(List<SearchWordConvertMapDb> list, Dictionary<string, object> extra = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public byte[] StreamToBytes(Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Position = 0;
            return bytes;
        }

        /// <summary>
        /// 文件流转成DataTable
        /// </summary>
        /// <param name="file">文件</param>
        /// <returns></returns>
        public DataTable GetExcelData(HttpPostedFileBase file)
        {
            var ext = Path.GetExtension(file.FileName);
            var bytes = StreamToBytes(file.InputStream);

            ISheet sheet = null;
            using (var memstream = new MemoryStream(bytes))
            {
                switch (ext)
                {
                    case ".xlsx":
                        sheet = new XSSFWorkbook(memstream).GetSheetAt(0);
                        break;
                    case ".xls":
                        sheet = new HSSFWorkbook(memstream).GetSheetAt(0);
                        break;
                }
            }

            var dt = new DataTable();
            if (sheet == null)
            {
                return dt;
            }

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

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }

            return dt;
        }

        public int ValidatorExcel(HttpPostedFileBase file)
        {
            var filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
            var ext = Path.GetExtension(file.FileName);//获取上传文件的扩展名
            const int maxSize = 10000 * 1024; //定义上传文件的最大空间大小为10M
            const string fileType = ".xls,.xlsx"; //定义上传文件的类型字符串

            if (file.ContentLength <= 0)
            {
                // 文件不能为空
                return -100;
            }
            if (string.IsNullOrWhiteSpace(ext))
            {
                // 文件扩展名不能为空
                return -101;
            }
            if (!fileType.Contains(ext))
            {
                // 文件类型不对，只能导入xls和xlsx格式的文件
                return -102;
            }
            if (filesize >= maxSize)
            {
                // 上传文件超过10M，不能上传
                return -103;
            }
            return 0;
        }
    }
}
