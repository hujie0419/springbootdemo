using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Tuhu.Provisioning.Common.Excel
{
    public class ExcelFun
   
    {
        private bool _isWholeTable = false;
        private const int DefaultMaxRow = 65534;
        public IDictionary HtFields { get; set; }

        public DataTable DtExcel { get; set; }

        public bool IsWholeTable
        {
            get { return _isWholeTable; }
            set { _isWholeTable = value; }
        }

        public string errMsg;

        /// <summary>
        /// 无参数的导出方法，导出的数据以及过滤的字段信息来自于对象的属性中
        /// </summary>
        /// <returns></returns>
        public Stream DtToExcel()
        {
            return DtToExcel(DtExcel);
        }
        /// <summary>
        /// 有一个参数的导出方法，过滤字段通过属性传递进入方法
        /// </summary>
        /// <param name="dtExcel">导出的数据源</param>
        /// <returns></returns>
        public Stream DtToExcel(DataTable dtExcel)
        {
            if (_isWholeTable == false && (HtFields == null || HtFields.Count == 0))
            {
                throw new ArgumentException("参数HtFields为空");
            }
            return DtToExcel(dtExcel, HtFields);
        }
        /// <summary>
        /// 通过NPOI组件生成excel
        /// </summary>
        /// <param name="dtExcel">将要导出的数据DataTable</param>
        /// <param name="htFields">需要导出的字段（key）以及对应的字段名称(value)</param>
        /// <returns></returns>
        public Stream DtToExcel(DataTable dtExcel, IDictionary htFields)
        {
            if (ValidateData(dtExcel, htFields) == false)
            {
                return Stream.Null;
            }
            // 构建一个excel工作台，对应一个excel文档
            HSSFWorkbook workbook = new HSSFWorkbook();
            CreateSheet(workbook, dtExcel, htFields, 1, 0);

            MemoryStream ms = new MemoryStream();
            // 将生成的excel工作台写到流中返回
            workbook.Write(ms);
            ms.Position = 0;
            return ms;
        }
        private void CreateSheet(HSSFWorkbook workbook, DataTable dtExcel, IDictionary htFields, int sheetIndex, int nextRowIndex)
        {
            string sheetName = string.Format("sheet{0}", sheetIndex);
            // 构建工作台中的一张表，对应到excel文档中的sheet
            ISheet sheet = workbook.CreateSheet(sheetName);
            #region  构建Sheet的头部
            // 新建一行，对应到excel文件中的一行
            IRow row = sheet.CreateRow(0);
            // 把值写入单元格中，此处生成表头
            // 这个为列索引，从0开始
            int cellIndex = 0;
            if (_isWholeTable)
            {
                for (int i = 0; i < dtExcel.Columns.Count; i++)
                {
                    row.CreateCell(i).SetCellValue(dtExcel.Columns[i].ColumnName);
                }
            }
            else
            {
                foreach (DictionaryEntry dictionaryEntry in htFields)
                {
                    int columnIndex = dtExcel.Columns.IndexOf(dictionaryEntry.Key.ToString());
                    if (columnIndex > -1)
                    {
                        string fieldText = htFields[dictionaryEntry.Key].ToString();
                        ICell cell = row.CreateCell(cellIndex++);
                        cell.SetCellValue(fieldText);
                        // 头部样式设置
                        ICellStyle cellStyle = workbook.CreateCellStyle();
                        cellStyle.BorderBottom = BorderStyle.Thick;
                        cellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.BrightGreen.Index;
                        NPOI.SS.UserModel.IFont headerfont = workbook.CreateFont();
                        headerfont.Boldweight = (short)FontBoldWeight.Bold;
                        cellStyle.SetFont(headerfont);
                        cell.CellStyle = cellStyle;
                    }
                }
            }
            #endregion

            #region  单元格中填写数据
            int totalCount = dtExcel.Rows.Count;
            bool isEndSheet = sheetIndex * DefaultMaxRow >= totalCount;
            int broundRowsCount = sheetIndex * DefaultMaxRow >= totalCount ? totalCount : sheetIndex * DefaultMaxRow;
            for (int i = nextRowIndex; i < broundRowsCount; i++)
            {
                // 列索引置0
                cellIndex = 0;
                // 生成一行，对应到excel文件中的一条数据行
                IRow row1 = sheet.CreateRow(i - nextRowIndex + 1);
                // 把值写入单元格中，此处生成表身
                foreach (DictionaryEntry dictionaryEntry in htFields)
                {
                    row1.CreateCell(cellIndex++).SetCellValue(dtExcel.Rows[i][dictionaryEntry.Key.ToString()].ToString());
                }
            }
            #endregion 
            #region 设置Sheet中列自适应 
            // 列宽度设置为自适应
            for (int i = 0; i < sheet.GetRow(0).LastCellNum; i++)
            {
                sheet.AutoSizeColumn(i);
            }
            #endregion
            if (!isEndSheet)
            {
                CreateSheet(workbook, dtExcel, htFields, sheetIndex + 1, nextRowIndex + broundRowsCount);
            }
        }
        /// <summary>
        /// 导出Excel时进行的参数数据验证
        /// </summary>
        /// <param name="dtExcel"></param>
        /// <param name="htFields"></param>
        /// <returns></returns>
        private bool ValidateData(DataTable dtExcel, IDictionary htFields)
        {
            if (dtExcel == null || dtExcel.Rows.Count == 0)
            {
                return false;
            }
            
            return true;
        }
    }
}