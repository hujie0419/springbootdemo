using System;
using System.Data;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.SendEmail.Comm
{
    /// <summary>
    /// Provides exporter or importer between DB and file.
    /// </summary>
    public static class ExportExcelHelper
    {
        public static int MaxRows { get; } = 50000;

        public static byte[] ExportExcel(DataSet source)
        {
            if (source == null || source.Tables.Count == 0)
                return null;

            var workbook = new XSSFWorkbook();//创建Workbook对象

            foreach (DataTable table in source.Tables)
            {
                ExportSheet(workbook, table.TableName, table);
            }

            using (var stream = new MemoryStream())
            {
                workbook.Write(stream);

                return stream.GetBuffer();
            }
        }

        public static byte[] ExportExcel(DataTable source)
        {
            if (source == null)
                return null;

            var workbook = new XSSFWorkbook();//创建Workbook对象

            ExportSheet(workbook, source.TableName, source);

            using (var stream = new MemoryStream())
            {
                workbook.Write(stream);

                return stream.GetBuffer();
            }
        }

        public static void ExportSheet(IWorkbook workbook, string sheetName, DataTable source)
        {
            var totalSheet = (source.Rows.Count + MaxRows - 1) / MaxRows;
            if (totalSheet > 0)
                for (int sheetCount = 0; sheetCount < totalSheet; sheetCount++)
                {
                    var sheet = workbook.CreateSheet(totalSheet > 1 ? sheetName + sheetCount : sheetName);//创建工作表

                    CreateHead(workbook, sheet, source.Columns);

                    for (var index = 0; index < source.Rows.Count - sheetCount * MaxRows; index++)
                    {
                        CreateRow(sheet.CreateRow(index + 1), source.Columns, source.Rows[index + sheetCount * MaxRows]);
                    }
                }
            else
            {
                var sheet = workbook.CreateSheet(sheetName);//创建工作表
                CreateHead(workbook, sheet, source.Columns);
            }
        }

        private static void CreateHead(IWorkbook workbook, ISheet sheet, DataColumnCollection columns)
        {
            //在工作表中添加一行
            var head = sheet.CreateRow(0);
            for (var index = 0; index < columns.Count; index++)
            {
                var cell = head.CreateCell(index);

                var style = workbook.CreateCellStyle();
                //设置单元格的样式：水平对齐居中
                style.Alignment = HorizontalAlignment.Center;
                //新建一个字体样式对象
                var font = workbook.CreateFont();
                //设置字体加粗样式
                font.Boldweight = short.MaxValue;
                //使用SetFont方法将字体样式添加到单元格样式中 
                style.SetFont(font);
                //将新的样式赋给单元格
                cell.CellStyle = style;

                cell.SetCellValue(columns[index].ColumnName);
            }
        }

        private static void CreateRow(IRow row, DataColumnCollection columns, DataRow dataRow)
        {
            for (var index = 0; index < columns.Count; index++)
            {
                var cell = row.CreateCell(index);

                var value = dataRow[columns[index]];
                if (value is bool)
                    cell.SetCellValue(Convert.ToBoolean(value));
                else if (value is DateTime)
                    cell.SetCellValue(Convert.ToDateTime(value).ToString("yyyy/MM/dd HH:mm:ss"));
                else if (value is int)
                    cell.SetCellValue(Convert.ToDouble(value));
                else if (value is string)
                    cell.SetCellValue(value.ToString());
                else
                {
                    double duo;
                    if (double.TryParse(value.ToString(), out duo))
                        cell.SetCellValue(duo);
                    else
                        cell.SetCellValue(value.ToString());
                }
            }
        }

    }
}
