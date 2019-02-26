using System.IO;
using System.Web;
using System.Data;
using System.Collections.Generic;

using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.SearchWordConvertMapConfig
{
    /// <summary>
    /// 新词处理
    /// </summary>
    public class SearchWordNewWordHandler : SearchWordMgr
    {
        private readonly ISearchWordConvertMgr _iswcmgr = new SearchWordConvertMgr();

        /// <summary>
        /// 数据导入
        /// </summary>
        /// <param name="file"></param>
        /// <param name="list"></param>
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
                var key = item.SourceWord;
                if (!string.IsNullOrEmpty(key))
                {
                    dbFindDicts[key] = item;
                }
            }

            var delRow = new List<DataRow>();
            for (var i = table.Rows.Count - 1; i >= 0; i--)
            {
                var canUse = table.Rows[i][1].ToString();
                var sourceWord = table.Rows[i][0].ToString();
                var existEntity = dbFindDicts.ContainsKey(sourceWord) ? dbFindDicts[sourceWord] : null;

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
            _iswcmgr.DeleteSearchWord(deleteList, SearchWordConfigType.NewWord);
            //执行插入操作
            _iswcmgr.ImportExcelInfoToDb(table, SearchWordConfigType.NewWord);

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
            var sheet = wb.CreateSheet("途虎词典配置表");

            //给sheet1添加第一行的头部标题
            var row = sheet.CreateRow(0);

            string[] title = {
                "关键词",
                "是否删除"
            };
            for (var i = 0; i < title.Length; i++)
            {
                var cell = row.CreateCell(i);
                cell.SetCellValue(title[i]);
            }
            for (var i = 0; i < list.Count; i++)
            {
                var rows = sheet.CreateRow(i + 1);
                rows.CreateCell(0).SetCellValue(list[i].SourceWord);
                rows.CreateCell(1).SetCellValue("1");
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
