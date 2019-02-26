using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Data;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.SearchWordConvertMapConfig
{
    /// <summary>
    /// 车型模块处理类
    /// </summary>
    public class SearchWordVehicleTypeHandler : SearchWordMgr
    {

        private readonly ISearchWordConvertMgr _iswcmgr = new SearchWordConvertMgr();

        /// <summary>
        /// 数据导入
        /// </summary>
        /// <param name="file"></param>
        /// <param name="list"></param>
        /// <param name="bytes"></param>
        public override int Import(HttpPostedFileBase file, List<SearchWordConvertMapDb> list, out byte[] bytes)
        {
            bytes = null;

            var result = ValidatorExcel(file);
            if (result < 0)
                return result;

            var table = GetExcelData(file);

            var deleteList = new List<SearchWordConvertMapDb>();
            var dbFindDicts = new Dictionary<string, SearchWordConvertMapDb>();
            foreach (var item in list)
            {
                var key = item.SourceWord + item.TargetWord + item.VehicleID 
                    + item.Sort + item.TireSize + item.SpecialTireSize + item.VehicleName;

                if (!string.IsNullOrEmpty(key))
                {
                    dbFindDicts[key] = item;
                }
            }

            var delRow = new List<DataRow>();
            for (var i = table.Rows.Count - 1; i >= 0; i--)
            {
                SearchWordConvertMapDb existEntity = null;

                var targetWord = table.Rows[i][0].ToString();
                var sourceWord = table.Rows[i][1].ToString();
                var canUse = table.Rows[i][2].ToString();
                var vehicleId = table.Rows[i][3].ToString();
                var sort = table.Rows[i][4].ToString();
                var tireSize = table.Rows[i][5].ToString();
                var specialTireSize = table.Rows[i][6].ToString();
                var vehicleName = table.Rows[i][7].ToString();
                var key = sourceWord + targetWord + vehicleId + sort + tireSize + specialTireSize + vehicleName;
                if (!string.IsNullOrEmpty(key))
                {
                    existEntity = dbFindDicts.ContainsKey(key) ? dbFindDicts[key] : null;
                }

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
            _iswcmgr.DeleteSearchWord(deleteList, SearchWordConfigType.VehicleType);
            //执行插入操作
            _iswcmgr.ImportExcelInfoToDb(table, SearchWordConfigType.VehicleType);

            result = 1;
            return result;
        }

        /// <summary>
        /// 导出
        /// </summary>
        public override byte[] Export(List<SearchWordConvertMapDb> list, Dictionary<string, object> extra = null)
        {
            //创建Excel文件的对象
            var wb = new HSSFWorkbook();

            //添加一个sheet
            var sheet = wb.CreateSheet("二级车型配置表");

            //给sheet1添加第一行的头部标题
            var row = sheet.CreateRow(0);

            // 设置样式
            var style = wb.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;

            // 设置字体
            var font = wb.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            style.SetFont(font);

            string[] title = {
                "关键词",
                "同义词" ,
                "是否删除",
                "二级车型ID",
                "排序",
                "规格",
                "特殊规格",
                "二级车型名称"
            };
            for (var i = 0; i < title.Length; i++)
            {
                var cell = row.CreateCell(i);
                cell.SetCellValue(title[i]);
            }
            for (var i = 0; i < list.Count; i++)
            {
                var cell = sheet.CreateRow(i + 1);
                cell.CreateCell(0).SetCellValue(list[i].TargetWord);
                cell.CreateCell(1).SetCellValue(list[i].SourceWord);
                cell.CreateCell(2).SetCellValue("1");
                cell.CreateCell(3).SetCellValue(list[i].VehicleID);
                cell.CreateCell(4).SetCellValue(list[i].Sort);
                cell.CreateCell(5).SetCellValue(list[i].TireSize);
                cell.CreateCell(6).SetCellValue(list[i].SpecialTireSize);
                cell.CreateCell(7).SetCellValue(list[i].VehicleName);
            }

            // 写入到客户端 
            byte[] file;
            using (var ms = new MemoryStream())
            {
                wb.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                file = ms.ToArray();
            }
            return file;
        }
    }
}
