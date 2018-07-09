using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using ThBiz.Common.Entity;
using Tuhu.Component.Framework;
namespace Tuhu.Provisioning.Common
{
    public static class ExportImportUtil
    {
        private static HttpSession httpSession = new HttpSession();
        public static void ExportExcel(HttpContextBase httpContext, string name, MemoryStream streamName)
        {
            httpContext.Response.ContentType = "applicationnd.ms-excel";
            name = HttpUtility.UrlEncode(name, System.Text.Encoding.GetEncoding("UTF-8"));
            httpContext.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", name));
            httpContext.Response.Clear();
            httpContext.Response.BinaryWrite(streamName.ToArray());
            httpContext.Response.End();
        }

        public static void ExportExcel2(HttpContextBase httpContext, string name, MemoryStream streamName)
        {
            httpContext.Response.Clear();
            /*
             * 添加头信息, 为"文件下载/另存为"对话框指定默认文件名
             * 解决了firefox下文件名乱码问题
            */
            string browser = httpContext.Request.Browser.Browser; ;//浏览器名称
            if (browser.ToUpper().IndexOf("IE") >= 0 || browser == "InternetExplorer" || browser.ToLower() == "mozilla")
            {
                name = HttpUtility.UrlEncode(name);
            }
            if (httpContext.Request.UserAgent.ToLower().IndexOf("firefox") > -1)
            {
                httpContext.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + name + "\"");
            }
            else
            {
                httpContext.Response.AddHeader("Content-Disposition", "attachment;filename=" + name);
            }
            //  添加头信息，指定文件大小，让浏览器能够显示下载进度
            //httpContext.Response.AddHeader("Content-Length", file.Length.ToString());
            //  指定返回的是一个不能被客户端读取的流，必须被下载
            httpContext.Response.ContentType = "application/ms-excel";
            //  把文件流发送到客户端
            httpContext.Response.BinaryWrite(streamName.ToArray());
            //  停止页面的执行
            httpContext.Response.End();
        }

        public static void ExportExcel3(HttpContextBase httpContext, string fileName, string[] headText, string[] headValue, DataTable dt)
        {
            string title = "";
            title = fileName + "（" + DateTime.Now.ToString("yyyyMMddHHss") + "）";
            StringWriter sw = new StringWriter();
            string head = "";
            string[] isSpecial = new string[headText.Length];
            int spe = 0;
            foreach (string str in headText)
            {
                isSpecial[spe] = "1";
                if (str.IndexOf('|') >= 0)
                    isSpecial[spe] = str.Split('|')[1];
                head += str.Split('|')[0] + "\t";
                spe += 1;
            }
            sw.WriteLine(head);
            string tmp = "";
            foreach (DataRow dr in dt.Rows)
            {
                tmp = "";
                spe = 0;
                foreach (string tmpStr in headValue)
                {
                    if (tmpStr.ToLower() == "prodtype")
                        tmp += GetType(dr[tmpStr].ToString()) + "\t";
                    else
                    {
                        if (isSpecial[spe] == "1")
                            tmp += "=\"" + dr[tmpStr].ToString() + "\"\t";
                        else
                            tmp += "" + dr[tmpStr].ToString() + "\t";
                    }
                    spe += 1;
                }
                sw.WriteLine(tmp);
            }
            sw.Close();
            httpContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(title, System.Text.Encoding.UTF8) + ".xls");
            httpContext.Response.ContentType = "application/ms-excel";
            httpContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            httpContext.Response.Write(sw);
            httpContext.Response.End();
        }

        public static void ExportExcel3(HttpContextBase httpContext, string fileName, string[] headText, string[] headValue, DataRow[] drs)
        {
            string title = "";
            title = fileName + "（" + DateTime.Now.ToString("yyyyMMddHHss") + "）";
            StringWriter sw = new StringWriter();
            string head = "";
            string[] isSpecial = new string[headText.Length];
            int spe = 0;
            foreach (string str in headText)
            {
                isSpecial[spe] = "1";
                if (str.IndexOf('|') >= 0)
                    isSpecial[spe] = str.Split('|')[1];
                head += str + "\t";
                spe += 1;
            }
            sw.WriteLine(head);
            string tmp = "";
            foreach (DataRow dr in drs)
            {
                tmp = "";
                spe = 0;
                foreach (string tmpStr in headValue)
                {
                    if (tmpStr.ToLower() == "prodtype")
                        tmp += GetType(dr[tmpStr].ToString()) + "\t";
                    else
                    {
                        if (isSpecial[spe] == "1")
                            tmp += "=\"" + dr[tmpStr].ToString() + "\"\t";
                        else
                            tmp += "" + dr[tmpStr].ToString() + "\t";
                    }
                    spe += 1;
                }
                sw.WriteLine(tmp);
            }
            sw.Close();
            httpContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(title, System.Text.Encoding.UTF8) + ".xls");
            httpContext.Response.ContentType = "application/ms-excel";
            httpContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            httpContext.Response.Write(sw);
            httpContext.Response.End();
        }

        public static void ExportExcel4(string path, string[] headText, string data)
        {
            using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("gb2312"));
                string head = "";
                foreach (string headTmp in headText)
                    head += headTmp + "\t";
                sw.WriteLine(head);
                string[] strs = data.Split('|');
                string result = "";
                foreach (string str in strs)
                {
                    result = "";
                    foreach (string tmp in str.Split(';'))
                        result += tmp + "\t";
                    sw.WriteLine(result);
                }
                sw.Close();
            }
        }

        public static void ExportExcel5(string path, string[] headText, string data)
        {
            using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                System.Text.StringBuilder table = new System.Text.StringBuilder();
                table.Append("<table style='border-right:1px solid #000000;border-bottom:1px solid #000000;'><tr>");
                foreach (string head in headText)
                {
                    table.Append("<td style='border-left:1px solid #000000;border-top:1px solid #000000;'>");
                    table.Append(head); //标格的标题  
                    table.Append("</td>");
                }
                table.Append("</tr>");
                string[] strs = data.Split('|');
                foreach (string str in strs)
                {
                    table.Append("<tr>");
                    foreach (string tmp in str.Split(';'))
                    {
                        table.Append("<td style='vnd.ms-excel.numberformat:@;border-left:1px solid #000000;border-top:1px solid #000000;'>");
                        table.Append(tmp);
                        table.Append("</td>");
                    }
                    table.Append("</tr>");
                }
                table.Append("</table>");
                sw.Write(table);
                sw.Close();
            }
        }
        public static void ExportExcel6(string path, string[] headText, List<ShopPayOffModel> data)
        {
            using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                System.Text.StringBuilder table = new System.Text.StringBuilder();
                table.Append("<table style='border-right:1px solid #000000;border-bottom:1px solid #000000;'><tr>");
                foreach (string head in headText)
                {
                    table.Append("<td style='border-left:1px solid #000000;border-top:1px solid #000000;'>");
                    table.Append(head); //标格的标题  
                    table.Append("</td>");
                }
                table.Append("</tr>");
                decimal totalBackmoney = 0;
                decimal totalDeliverFee = 0;
                foreach (ShopPayOffModel str in data)
                {
                    totalBackmoney += string.IsNullOrEmpty(str.BackMoney) ? 0 : decimal.Parse(str.BackMoney);
                    totalDeliverFee += string.IsNullOrEmpty(str.DeliveryFee) ? 0 : decimal.Parse(str.DeliveryFee);
                    table.Append("<tr>");
                    foreach (string tmp in str.GetArray())
                    {
                        table.Append("<td style='vnd.ms-excel.numberformat:@;border-left:1px solid #000000;border-top:1px solid #000000;'>");
                        table.Append(tmp);
                        table.Append("</td>");
                    }
                    table.Append("</tr>");
                }
                string[] lastRow = new string[] { "", "", "", "", "合计：", "行数：" + data.Count, "" + totalBackmoney, "" + totalDeliverFee };
                table.Append("<tr>");
                foreach (string tmp in lastRow)
                {
                    table.Append("<td style='vnd.ms-excel.numberformat:@;border-left:1px solid #000000;border-top:1px solid #000000;'>");
                    table.Append(tmp);
                    table.Append("</td>");
                }
                table.Append("</tr>");
                table.Append("</table>");
                sw.Write(table);
                sw.Close();
            }
        }
        public static void DownloadFile(HttpContextBase httpContext, string filePath)
        {
            string fileName = "VoucherItem_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";//客户端保存的文件名
            FileInfo fileInfo = new FileInfo(filePath);
            httpContext.Response.Clear();
            httpContext.Response.ClearContent();
            httpContext.Response.ClearHeaders();
            httpContext.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
            httpContext.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
            httpContext.Response.AddHeader("Content-Transfer-Encoding", "binary");
            httpContext.Response.ContentType = "application/octet-stream";
            httpContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            httpContext.Response.WriteFile(fileInfo.FullName);
            httpContext.Response.Flush();
            httpContext.Response.End();
        }

        private static string GetType(string prodtype)
        {
            string result = "";
            switch (prodtype)
            {
                case "Maintenance":
                    result = "保养";
                    break;
                case "Others":
                    result = "其他";
                    break;
                case "Tire":
                    result = "轮胎";
                    break;
                case "Beauty":
                    result = "美容";
                    break;
            }
            return result;
        }

        /// <summary>
        /// 获取EXCEL数据
        /// </summary>
        /// <param name="postFile">客户端文件流</param>
        /// <returns>数据集</returns>
        private static DataTable GetFileData(HttpPostedFileBase postFile)
        {
            StreamReader sr = new StreamReader(postFile.InputStream, Encoding.GetEncoding("gb2312"));
            DataTable dt = new DataTable();
            try
            {
                int i = 0;
                while (sr.Peek() >= 0)
                {
                    string strTmp = sr.ReadLine();
                    if (postFile.FileName.ToLower().LastIndexOf(".xls") >= 0 || postFile.FileName.ToLower().LastIndexOf(".xlsx") >= 0)
                        strTmp = strTmp.Replace("\t", ",").TrimEnd(',');
                    int k = 0;
                    DataRow dr = dt.NewRow();
                    foreach (string str in strTmp.Split(','))
                    {
                        if (i == 0)
                            dt.Columns.Add(new DataColumn(str.Replace("\"", "").Replace(" ", "").Replace("　", "").Replace("'", "").Replace("\t", "").Replace("=", "")));
                        else
                        {
                            if (k >= dt.Columns.Count)
                                break;
                            //str.Replace("\"", "");
                            dr[k] = str.Replace("\"", "").Replace(" ", "").Replace("　", "").Replace("'", "").Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace("=", "");
                            k += 1;
                        }
                    }
                    if (i > 0)
                        dt.Rows.Add(dr);
                    i += 1;
                }
            }
            finally
            {
                sr.Close();
            }
            return dt;
        }

        /// <summary>
        /// 缓存EXCEL数据
        /// </summary>
        /// <param name="postFile">客户端文件流</param>
        public static void CouchFileData(HttpPostedFileBase postFile, string key)
        {
            //DataTable dt = new DataTable();
            //var cache = CacheFactory.Create(CacheType.CouchBase);
            //cache.TryGet(string.Format(key), out dt);
            //if (dt == null || dt.Rows.Count == 0)
            //{
            //    dt = GetFileData(postFile);
            //    cache.Upload(string.Format(key), dt);
            //}
            DataTable dt = GetFileData(postFile);
            httpSession.Set(key, dt);
        }

        /// <summary>
        /// 获取缓存的EXCEL数据
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="result">结果</param>
        /// <returns>数据集</returns>
        public static DataTable GetFileData(string key, ref string result)
        {
            //DataTable dt = new DataTable();
            //var cache = CacheFactory.Create(CacheType.CouchBase);
            //cache.TryGet(string.Format(key), out dt);
            //if (dt == null || dt.Rows.Count == 0)
            //    result = "缓存失效";
            //return dt;
            DataTable dt = httpSession.Get<DataTable>(key);
            if (dt == null || dt.Rows.Count == 0)
                result = "缓存失效";
            //httpSession.Remove(key);
            return dt;
        }

        /// <summary>
        /// 删除文件缓存数据
        /// </summary>
        /// <param name="key">键值</param>
        public static void RemoveCouchExcelData(string key)
        {
            //var cache = CacheFactory.Create(CacheType.CouchBase);
            //cache.Remove(string.Format(key));
            httpSession.Remove(key);
        }

        public static void CouchPageData(string userno, string data, string[] headText)
        {
            try
            {
                DataTable dt = new DataTable();
                string key = userno + "receive!@#$";
                foreach (string head in headText)
                {
                    dt.Columns.Add(new DataColumn(head));
                }
                string[] strs = data.Split('|');
                foreach (string str in strs)
                {
                    DataRow dr = dt.NewRow();
                    int i = 0;
                    foreach (string tmp in str.Split(';'))
                    {
                        dr[i] = tmp;
                        i += 1;
                    }
                    dt.Rows.Add(dr);
                }
                httpSession.Set<DataTable>(dt, key);

            }
            catch (Exception ex) { throw ex; }
        }

        public static void ExportExcel3(HttpContextBase httpContext, string fileName, DataTable dt)
        {
            string title = "";
            title = fileName + "（" + DateTime.Now.ToString("yyyyMMddHHss") + "）";
            StringWriter sw = new StringWriter();
            string head = "";
            foreach (DataColumn dc in dt.Columns)
                head += dc.ColumnName + "\t";
            sw.WriteLine(head);
            string tmp = "";
            foreach (DataRow dr in dt.Rows)
            {
                tmp = "";
                foreach (DataColumn dc in dt.Columns)
                    tmp += dr[dc.ColumnName].ToString() + "\t";
                sw.WriteLine(tmp);
            }
            sw.Close();
            httpContext.Response.Clear();
            httpContext.Response.ClearContent();
            httpContext.Response.ClearHeaders();
            httpContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(title, System.Text.Encoding.UTF8) + ".xls");
            httpContext.Response.ContentType = "application/ms-excel";
            httpContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            httpContext.Response.Write(sw);
            httpContext.Response.Flush();
            httpContext.Response.End();
        }

        /// <summary>
        /// 将文件数据转为JSON
        /// </summary>
        /// <param name="postFile">文件流</param>
        /// <returns>JSON字符串</returns>
        public static string GetFileDataToJson(HttpPostedFileBase postFile)
        {
            StreamReader sr = new StreamReader(postFile.InputStream, Encoding.GetEncoding("gb2312"));
            string result = "";
            List<string> head = new List<string>();
            try
            {
                int i = 0;
                while (sr.Peek() >= 0)
                {
                    string strTmp = sr.ReadLine();
                    if (postFile.FileName.ToLower().LastIndexOf(".xls") >= 0 || postFile.FileName.ToLower().LastIndexOf(".xlsx") >= 0)
                        strTmp = strTmp.Replace("\t", ",").TrimEnd(',');
                    int k = 0;
                    string tmpResult = "";
                    foreach (string str in strTmp.Split(','))
                    {
                        if (i == 0)
                        {
                            head.Add(str.Replace("\"", "").Replace(" ", "").Replace("　", "").Replace("'", "").Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace("=", "").Replace("\\", ""));
                            continue;
                        }
                        else
                        {
                            if (k >= head.Count)
                                break;
                            tmpResult += "\\\"" + head[k] + "\\\":" + "\\\"" + str.Replace("\"", "").Replace(" ", "").Replace("　", "").Replace("'", "").Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace("=", "").Replace("\\", "") + "\\\",";
                            k += 1;
                        }
                    }
                    if (i > 0)
                        result += "{" + tmpResult.TrimEnd(',') + "},";
                    i += 1;
                }
                result = "[" + result.TrimEnd(',') + "]";
            }
            finally
            {
                sr.Close();
            }
            return result;
        }

        /// <summary>
        /// Json 字符串 转换为 DataTable数据集合
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this string json)
        {
            DataTable dataTable = new DataTable(); //实例化
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
                ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count == 0)
                        {
                            return dataTable;
                        }
                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                dataTable.Columns.Add(current, dictionary[current].GetType());
                            }
                        }
                        DataRow dataRow = dataTable.NewRow();
                        foreach (string current in dictionary.Keys)
                        {
                            dataRow[current] = dictionary[current];
                        }
                        dataTable.Rows.Add(dataRow); //循环添加行到DataTable中
                    }
                }
            }
            catch
            {
            }
            return dataTable;
        }

        /// <summary>
        /// 将文件数据转为JSON
        /// </summary>
        /// <param name="postFile">文件流</param>
        /// <returns>JSON字符串</returns>
        public static string GetFileDataToJson(HttpPostedFileBase filePost, string encodingStr)
        {
            string result = "";
            List<string> head = new List<string>();
            using (Stream s = filePost.InputStream)
            {
                Encoding encoding = Encoding.GetEncoding("gb2312");
                if (encodingStr.ToLower() == "utf8")
                    encoding = Encoding.UTF8;
                StreamReader sr = new StreamReader(s, encoding);
                try
                {
                    int count = 1;
                    int flag = -1;
                    while (sr.Peek() > 0)
                    {
                        string strTmp = sr.ReadLine();
                        int i = 0;
                        if (strTmp.Split(',').Length >= 2)
                        {
                            if (flag == -1)
                                flag = 1;
                        }
                        else
                        {
                            if (flag == -1)
                                flag = 0;
                        }
                        if (flag == 0)
                            strTmp = strTmp.Replace(" ", ",");
                        string tmpResult = "";
                        foreach (string str in strTmp.Split(','))
                        {
                            if (flag == 0)
                                if (string.IsNullOrWhiteSpace(str))
                                    continue;
                            if (count == 1)
                            {
                                head.Add(str.Replace("\"", "").Replace(" ", "").Replace("　", "").Replace("'", "").Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace("=", "").Replace("\\", ""));
                                continue;
                            }
                            else
                            {
                                if (i >= head.Count)
                                    break;
                                tmpResult += "\"" + head[i] + "\":" + "\"" + str.Replace("\"", "").Replace(" ", "").Replace("　", "").Replace("'", "").Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace("=", "").Replace("\\", "") + "\",";
                                i += 1;
                            }
                        }
                        if (count > 1)
                            result += "{" + tmpResult.TrimEnd(',') + "},";
                        count += 1;
                    }
                    result = "[" + result.TrimEnd(',') + "]";
                }
                finally
                {
                    sr.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// 将长的Json装换成段的字符Lst
        /// </summary>
        /// <param name="fileDataJson">原Json字符串</param>
        /// <param name="num">组内个数</param>
        /// <returns></returns>
        public static List<string> JsonToSplitList(string fileDataJson, int num)
        {
            var cBool = true;
            int d = 0; int c = 0;
            int Count = 0;
            List<string> lstStr = new List<string>();
            string tempStr = "";
            while (cBool)
            {
                if (c >= 0)
                {
                    c = fileDataJson.IndexOf("},{", c);
                    if (c > 0)
                    {
                        tempStr += fileDataJson.Substring(d, c - d + 2);
                        d = c + 2;
                        c = c + 3;
                        Count++;
                        if (Count % num == 0)
                        {
                            tempStr = tempStr.TrimEnd(',') + "]";
                            lstStr.Add(tempStr);
                            tempStr = "[";
                        }
                    }
                }
                else
                {
                    cBool = false;
                    tempStr += fileDataJson.Substring(d, fileDataJson.Length - d);
                    lstStr.Add(tempStr);
                }
            }
            return lstStr;
        }
        /// <summary>  
        /// 将excel导入到datatable  
        /// </summary>  
        /// <param name="filePath">excel路径</param>  
        /// <param name="isColumnName">第一行是否是列名</param>  
        /// <returns>返回datatable</returns>  
        public static DataTable ExcelToDataTable(string filePath, bool isColumnName)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 0;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本  
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本  
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet  
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数  
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行  
                                int cellCount = firstRow.LastCellNum;//列数  

                                //构建datatable的列  
                                if (isColumnName)
                                {
                                    startRow = 1;//如果第一行是列名，则从第二行开始读取  
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            if (cell.StringCellValue != null)
                                            {
                                                column = new DataColumn(cell.StringCellValue);
                                                dataTable.Columns.Add(column);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行  
                                for (int i = startRow; i <= rowCount; ++i)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)  
                                            switch (cell.CellType)
                                            {
                                                case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理  
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
        }
    }
}