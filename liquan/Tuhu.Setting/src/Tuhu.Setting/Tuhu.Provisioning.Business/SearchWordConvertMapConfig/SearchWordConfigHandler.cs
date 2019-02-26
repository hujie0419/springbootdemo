using System.IO;
using System.Web;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.SearchWordConvertMapConfig
{
    public class SearchWordConfigHandler : SearchWordMgr
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

            // 用户上传数据
            var table = GetExcelData(file);
            var deleteList = new List<SearchWordConvertMapDb>();

            var sourceDic = new Dictionary<string, string>();
            var opDic = new Dictionary<string, SearchWordConvertMapDb>();
            var dbFindDicts = new Dictionary<string, SearchWordConvertMapDb>();

            // 历史数据
            foreach (var item in list)
            {
                var key = item.SourceWord + item.TargetWord;
                if (string.IsNullOrEmpty(key))
                    continue;

                opDic[key] = item;
                dbFindDicts[key] = item;
                sourceDic[item.SourceWord] = item.TargetWord;
            }

            var redCnt = 0;
            var yellowCnt = 0;
            var repList = new List<SearchWordConvertMapDb>();
            var newList = new List<SearchWordConvertMapDb>();

            /**
             * 遍历用户上传的数据
             */
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var targetWord = table.Rows[i][0].ToString();
                var sourceWord = table.Rows[i][1].ToString();
                var isDelete = table.Rows[i][2].ToString();
                var updateBy = table.Rows[i][4].ToString();
                var pkid = table.Rows[i][5].ToString();

                // 检查过往是否有相同的同义词
                if (sourceDic.ContainsKey(sourceWord))
                {
                    // 目标关键字不同且不是删除状态 添加临时列表中
                    if (!sourceDic[sourceWord].Equals(targetWord) && isDelete == "1" && (pkid == "0" || pkid == ""))
                    {
                        var map = new SearchWordConvertMapDb
                        {
                            TargetWord = targetWord,
                            SourceWord = sourceWord,
                            UpdateBy = updateBy
                        };
                        repList.Add(map);

                        var dbItem = list.FirstOrDefault(p => p.SourceWord == sourceWord);
                        if (!repList.Any(p => p.SourceWord.Equals(dbItem?.SourceWord) && p.TargetWord.Equals(dbItem?.TargetWord)))
                        {
                            repList.Add(dbItem);
                        }
                        redCnt++;
                    }
                }
                else
                {
                    //数据库中不存在，则新增，也需要再次返回
                    newList.Add(new SearchWordConvertMapDb
                    {
                        TargetWord = targetWord,
                        SourceWord = sourceWord,
                        UpdateBy = updateBy
                    });
                    // 新增且不删除 并且不冲锋狙的
                    if ((pkid == "0" || pkid == "") && isDelete == "1" &&
                        newList.Any(p => p.TargetWord == targetWord && p.SourceWord == sourceWord))
                    {
                        repList.Add(new SearchWordConvertMapDb
                        {
                            TargetWord = targetWord,
                            SourceWord = sourceWord,
                            UpdateBy = updateBy
                        });
                        redCnt++;
                    }
                }
            }

            // 如果上传数据已在数据库中，需要单独处理
            if (repList.Any())
            {
                repList = repList?.OrderBy(o => o.PKID).ToList();
                var tmpList = new List<SearchWordConvertMapDb>();
                for (var i = 0; i < list.Count; i++)
                {
                    if (repList.Any(p => p.PKID.Equals(list[i].PKID)))
                    {
                        yellowCnt++;
                    }
                    else
                    {
                        tmpList.Add(list[i]);
                    }
                }
                if (yellowCnt > 0)
                {
                    repList.AddRange(repList.Intersect(newList).ToList());
                }
                repList.AddRange(tmpList);

                var delCnt = 0;
                foreach (var item in repList)
                {
                    var targetWord = item.TargetWord;
                    var sourceWord = item.SourceWord;

                    if (!dbFindDicts.ContainsKey(sourceWord + targetWord))
                        continue;

                    var rows = table.Select($"关键词 = '{targetWord}' AND 同义词 = '{sourceWord}' AND 是否删除 = '0'").ToList();
                    if (rows.Any())
                    {
                        delCnt--;
                    }
                }

                if (redCnt > 1 || yellowCnt + delCnt > 0)
                {
                    var paras = new Dictionary<string, object> { { "RedCnt", redCnt }, { "YellowCnt", yellowCnt } };
                    bytes = Export(repList.ToList(), paras);
                    result = -1;
                    return result;
                }
            }

            var delRow = new List<DataRow>();
            var opList = new List<SearchWordConvertMapDb>();
            for (var i = table.Rows.Count - 1; i >= 0; i--)
            {
                SearchWordConvertMapDb existEntity = null;

                var targetWord = table.Rows[i][0].ToString();
                var sourceWord = table.Rows[i][1].ToString();
                var canUse = table.Rows[i][2].ToString();
                var op = table.Rows[i][4].ToString();
                var key = sourceWord + targetWord;

                if (!string.IsNullOrEmpty(key))
                {
                    existEntity = dbFindDicts.ContainsKey(key) ? dbFindDicts[key] : null;
                }
                if (opDic.ContainsKey(key) && opDic[key].UpdateBy != op)
                {
                    opDic[key].UpdateBy = op;
                    opList.Add(opDic[key]);
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
                var targetWord = dr[0].ToString();
                var sourceWord = dr[1].ToString();
                var tag = dr[3].ToString();
                var key = sourceWord + targetWord;

                if (int.TryParse(tag, out int temp))
                {
                    var existEntity = dbFindDicts.ContainsKey(key) ? dbFindDicts[key] : null;
                    if (existEntity != null && existEntity.Tag != temp)
                    {
                        deleteList.Add(existEntity);
                    }
                    else
                    {
                        table.Rows.Remove(dr);
                    }
                }
                else
                {
                    table.Rows.Remove(dr);
                }
            }

            //执行删除配置操作
            _iswcmgr.DeleteSearchWord(deleteList, SearchWordConfigType.Config);
            //执行插入操作
            _iswcmgr.ImportExcelInfoToDb(table, SearchWordConfigType.Config);
            //批量更新
            _iswcmgr.UpdateSearchWord(opList, SearchWordConfigType.Config);

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
            var sheet = wb.CreateSheet("同义词配置表");

            //给sheet1添加第一行的头部标题
            var row = sheet.CreateRow(0);

            // 设置样式
            var styleRed = wb.CreateCellStyle();
            styleRed.FillPattern = FillPattern.SolidForeground;
            styleRed.FillForegroundColor = HSSFColor.Red.Index;

            var styleYellow = wb.CreateCellStyle();
            styleYellow.FillPattern = FillPattern.SolidForeground;
            styleYellow.FillForegroundColor = HSSFColor.Yellow.Index;

            // 设置字体
            var font = wb.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.Color = HSSFColor.Black.Index;
            font.Boldweight = (short)FontBoldWeight.Bold;
            styleRed.SetFont(font);
            styleYellow.SetFont(font);

            string[] title = {
                "关键词",
                "同义词" ,
                "是否删除",
                "Tag",
                "修改者",
                "PKID"
            };
            var redCnt = 0;
            var yellowCnt = 0;
            if (extra != null && extra.Count > 0)
            {
                redCnt = int.Parse(extra["RedCnt"].ToString());
                yellowCnt = int.Parse(extra["YellowCnt"].ToString());
            }
            for (var i = 0; i < title.Length; i++)
            {
                var cell = row.CreateCell(i);
                cell.SetCellValue(title[i]);
            }

            for (var i = 0; i < list.Count; i++)
            {
                var rows = sheet.CreateRow(i + 1);

                rows.CreateCell(0).SetCellValue(list[i].TargetWord);
                rows.CreateCell(1).SetCellValue(list[i].SourceWord);
                rows.CreateCell(2).SetCellValue("1");
                rows.CreateCell(3).SetCellValue(list[i].Tag.ToString());
                rows.CreateCell(4).SetCellValue(list[i].UpdateBy);
                rows.CreateCell(5).SetCellValue(list[i].PKID.ToString());

                if (i >= redCnt + yellowCnt)
                    continue;

                rows.Cells.ForEach(r =>
                {
                    r.CellStyle = i < redCnt ? styleRed : styleYellow;

                });
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
