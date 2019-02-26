using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Data;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Tuhu.Provisioning.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class ProvisioningExcelHelper : IDisposable
    {
        private IWorkbook _workbook;
        /// <summary>
        /// 初始化Workbook
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="fileName">Excel文件名</param>
        private void InitWorkbook(Stream stream = null, string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName) || fileName.IndexOf(".xlsx") > 0)  //2007及以上版本
                _workbook = stream == null ? new XSSFWorkbook() : new XSSFWorkbook(stream);
            else if (fileName.IndexOf(".xls") > 0) //2003版本
                _workbook = stream == null ? new HSSFWorkbook() : new HSSFWorkbook(stream);

            if (_workbook == null) throw new ArgumentNullException("workbook");
        }

        /// <summary>
        /// DataSet数据导出Excel
        /// </summary>
        /// <param name="ds">数据源</param>
        /// <param name="sheetNames">Sheet名称</param>
        /// <param name="fileName">Excel文件名</param>
        public void DataSetToExcel(DataSet ds, List<string> sheetNames = null, string fileName = null)
        {
            InitWorkbook(fileName: fileName);

            int tableCount = ds.Tables.Count;

            if (tableCount > 0)
            {
                for (int index = 0; index < tableCount; index++)
                {
                    DataTable dt = ds.Tables[index];

                    string sheetName = dt.TableName;
                    if (sheetNames != null && sheetNames.Count > 0) sheetName = sheetNames[index];

                    FillExcel(dt, sheetName);
                }
            }
            else
            {
                FillExcel(new DataTable());
            }

            SaveToExcel(fileName);
        }

        /// <summary>
        /// 清空空行
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DataTable ClearEmputyRow(DataTable dt)
        {
            if (dt == null)
            {
                return dt;
            }
            //清除空行 从上往下
            for (int i = 0; i < dt.Rows.Count;)
            {
                bool IsEmpty = true;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j] != null && dt.Rows[i][j].ToString() != "")
                    {
                        IsEmpty = false;
                        break;
                    }
                }
                if (IsEmpty)
                {
                    dt.Rows[i].Delete();
                }
                else
                {
                    break;
                }
            }
            //清除空行 从下往上
            for (int i = dt.Rows.Count - 1; i > 0;)
            {
                bool IsEmpty = true;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j] != null && dt.Rows[i][j].ToString() != "")
                    {
                        IsEmpty = false;
                        break;
                    }
                }
                if (IsEmpty)
                {
                    dt.Rows[i].Delete();
                    i--;
                }
                else
                {
                    break;
                }
            }
            return dt;
        }

        /// <summary>
        /// DataTable数据导出Excel
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="sheetName">Sheet名称</param>
        /// <param name="fileName">Excel文件名</param>
        public void DataTableToExcel(DataTable dt, string sheetName = null, string fileName = null)
        {
            InitWorkbook(fileName: fileName);

            FillExcel(dt, sheetName);

            SaveToExcel(fileName);
        }

        /// <summary>
        /// 填充Excel
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="sheetName">Sheet名称</param>
        private void FillExcel(DataTable dt, string sheetName = null)
        {
            ISheet sheet = string.IsNullOrWhiteSpace(sheetName) ? _workbook.CreateSheet() : _workbook.CreateSheet(sheetName);

            int startRow = 0;

            IRow headerRow = sheet.CreateRow(startRow);

            startRow++;

            for (int columnIndex = 0; columnIndex < dt.Columns.Count; columnIndex++)
            {
                ICell cell = headerRow.CreateCell(columnIndex);

                cell.SetCellValue(dt.Columns[columnIndex].ColumnName);
            }

            for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
            {
                IRow row = sheet.CreateRow(startRow);

                for (int columnIndex = 0; columnIndex < dt.Columns.Count; columnIndex++)
                {
                    DataColumn column = dt.Columns[columnIndex];
                    
                    ICell cell = row.CreateCell(columnIndex);

                    string cellValue = dt.Rows[rowIndex][columnIndex].ToString();
                    int columnWidth = sheet.GetColumnWidth(rowIndex) / 256;
                    if (columnWidth < cellValue.Length)
                    {
                        columnWidth = cellValue.Length;
                    }
                        sheet.SetColumnWidth(columnIndex, columnWidth * 256);
                    switch (column.DataType.ToString())
                    {
                        case "System.DateTime"://日期类型
                            DateTime dateV;
                            DateTime.TryParse(cellValue, out dateV);
                            cell.SetCellValue(dateV);
                            break;
                        case "System.Int16"://整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(cellValue, out intV);
                            cell.SetCellValue(intV);
                            break;
                        case "System.Decimal"://浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(cellValue, out doubV);
                            cell.SetCellValue(doubV);
                            break;
                        default:
                            cell.SetCellValue(cellValue);
                            break;
                    }
                }

                startRow++;
            }
        }

        /// <summary>
        /// 删除文件夹下所有文件
        /// </summary>
        /// <param name="srcPath"></param>
        public  void DelectDir(string srcPath)
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);          //删除子目录和文件
                }
                else
                {
                    System.IO.File.Delete(i.FullName);      //删除指定文件
                }
            }
        }

        /// <summary>
        /// 导入Excel数据到DataTable
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="fileName">Excel文件名</param>
        /// <returns>DataTable</returns>
        public DataTable ExcelToDataTable(Stream stream, string fileName)
        {
            DataTable dt = new DataTable();

            InitWorkbook(stream, fileName);

            ISheet sheet = _workbook.GetSheetAt(0);

            for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);

                DataRow dr = null;

                if (rowIndex != 0) dr = dt.NewRow();

                for (int cellIndex = 0; cellIndex < row.LastCellNum; cellIndex++)
                {
                    object cellValue = GetCellValue(row, cellIndex);
                    if (rowIndex == 0)
                    {
                        dt.Columns.Add(cellValue.ToString());
                    }
                    else
                    {
                        dr[cellIndex] = cellValue;
                    }
                }

                if (dr != null)
                    dt.Rows.Add(dr);
            }

            return dt;
        }

        private object GetCellValue(IRow row, int cellIndex)
        {
            ICell cell = row.GetCell(cellIndex);

            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                        return cell.NumericCellValue;
                    case CellType.Boolean:
                        return cell.BooleanCellValue;
                    case CellType.Error:
                        return cell.ErrorCellValue;
                    case CellType.Formula:
                        return cell.CellFormula;
                    default:
                        return cell.StringCellValue;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 保存Excel
        /// </summary>
        /// <param name="fileName">Excel文件名</param>
        private void SaveToExcel(string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName)) { fileName = string.Format("{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss")); }

            HttpContext context = HttpContext.Current;

            if (context != null)
            {
                context.Response.Clear();

                context.Response.ContentType = "application/x-excel";

                context.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);

                _workbook.Write(context.Response.OutputStream);

                context.Response.End();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (_workbook != null)
                _workbook = null;
        }
    }
}