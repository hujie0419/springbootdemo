using Common.Logging;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Enums;
using Tuhu.Service.UserAccount.Models;
using Tuhu.C.YunYing.WinService.JobSchedulerService.DAL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.DLL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BLL
{
    public class VehiclePartsLiYangBusiness
    {
        private static readonly Common.Logging.ILog Logger = LogManager.GetLogger(typeof(VehiclePartsLiYangBusiness));

        private static readonly string connectionRw = ConfigurationManager.ConnectionStrings["Tuhu_epc"].ConnectionString;

        private static string FileDoMain = ConfigurationManager.AppSettings["FileDoMain"];

        /// <summary>
        /// Excel模版转为DataTable
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static DataTable ConvertExcelToList(byte[] buffer, UploadFileTask task)
        {
            var dt = null as DataTable;
            try
            {
                DalUploadFileTask.UpdateFileTaskStatus(task, FileStatus.Runing);
                using (var reader = ExcelReaderFactory.CreateReader(new MemoryStream(buffer)))
                {
                    var ds = reader.AsDataSet();
                    var temp = ConvertExcelToList(ds, task);
                    reader.Close();
                    dt = temp.Item1;
                    var errorMessage = temp.Item2;
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        DalUploadFileTask.SetErrrorFileStatus(task, FileStatus.Cancel, errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                DalUploadFileTask.SetErrrorFileStatus(task, FileStatus.Cancel, ex.Message);
                Logger.Info("ConvertExcelToList", ex);
                dt = null;
            }
            return dt;
        }

        /// <summary>
        /// Excel模版转为DataTable
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private static Tuple<DataTable, string> ConvertExcelToList(DataSet ds, UploadFileTask task)
        {
            var result = null as DataTable;
            var message = string.Empty;
            var minDateTime = new DateTime(1970, 1, 1, 0, 0, 0);
            try
            {
                var dsRows = ds.Tables[0].Rows;
                if (dsRows.Count < 2)
                {
                    message = "Excel内无数据";
                    return Tuple.Create(result, message);
                }
                var titleRow = dsRows[0];
                var titleCellIndex = 0;
                if (titleRow.ItemArray[titleCellIndex++]?.ToString() == "品牌" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "车系" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "力洋分类" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "*主组" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "*子组" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "*OE号" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "*OE名称" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "OE英文名称" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "图号" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "左右" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "安装开始日期" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "安装结束日期" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "用量" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "参考单价" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "用法/备注" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "力洋ID")
                {
                    #region 初始化DataTable
                    var dt = new DataTable { };
                    dt.Columns.Add(new DataColumn("Brand", typeof(string)));
                    dt.Columns.Add(new DataColumn("Series", typeof(string)));
                    dt.Columns.Add(new DataColumn("Category", typeof(string)));
                    dt.Columns.Add(new DataColumn("MainCategory", typeof(string)));
                    dt.Columns.Add(new DataColumn("SubGroup", typeof(string)));
                    dt.Columns.Add(new DataColumn("OeCode", typeof(string)));
                    dt.Columns.Add(new DataColumn("OeName", typeof(string)));
                    dt.Columns.Add(new DataColumn("OeEnName", typeof(string)));
                    dt.Columns.Add(new DataColumn("ImageNo", typeof(string)));
                    dt.Columns.Add(new DataColumn("Position", typeof(string)));
                    dt.Columns.Add("InstallBeginTime");
                    dt.Columns.Add("InstallEndTime");
                    dt.Columns.Add(new DataColumn("Dosage", typeof(string)));
                    dt.Columns.Add("Price");
                    dt.Columns.Add(new DataColumn("Remarks", typeof(string)));
                    dt.Columns.Add(new DataColumn("LiYangId", typeof(string)));
                    dt.Columns.Add(new DataColumn("BatchCode", typeof(string)));
                    dt.Columns.Add(new DataColumn("AreaKey", typeof(int)));
                    #endregion
                    for (var rowIndex = 1; rowIndex < dsRows.Count; rowIndex++)
                    {
                        var row = dsRows[rowIndex];
                        if (row == null) continue;
                        var cellIndex = 0;
                        var brand = row.ItemArray[cellIndex++].ToString()?.Trim();
                        var series = row.ItemArray[cellIndex++].ToString()?.Trim();
                        var category = row.ItemArray[cellIndex++].ToString()?.Trim();
                        var mainCategory = row.ItemArray[cellIndex++].ToString()?.Trim();
                        var subGroup = row.ItemArray[cellIndex++].ToString()?.Trim();
                        var oeCode = row.ItemArray[cellIndex++].ToString()?.Trim();
                        var oeName = row.ItemArray[cellIndex++].ToString()?.Trim();
                        var oeEnName = row.ItemArray[cellIndex++].ToString()?.Trim();
                        var imageNo = row.ItemArray[cellIndex++].ToString()?.Trim();
                        var position = row.ItemArray[cellIndex++].ToString()?.Trim();
                        DateTime installBeginTime;
                        var installBeginTimeStr = row.ItemArray[cellIndex++].ToString()?.Trim();
                        DateTime installEndTime;
                        var installEndTimeStr = row.ItemArray[cellIndex++].ToString()?.Trim();
                        var dosage = row.ItemArray[cellIndex++].ToString()?.Trim();
                        decimal price;
                        var priceStr = row.ItemArray[cellIndex++].ToString()?.Trim();
                        var remarks = row.ItemArray[cellIndex++].ToString()?.Trim();
                        var liYangId = row.ItemArray[cellIndex].ToString()?.Trim();
                        if (brand.Length > 50)
                        {
                            return Tuple.Create(result, $"第{rowIndex + 1}行，品牌列:长度超出数据库字段限制");
                        }
                        if (series.Length > 50)
                        {
                            return Tuple.Create(result, $"第{rowIndex + 1}行，车系列:长度超出数据库字段限制");
                        }
                        if (category.Length > 100)
                        {
                            return Tuple.Create(result, $"第{rowIndex + 1}行，力洋分类列:长度超出数据库字段限制");
                        }
                        if (mainCategory.Length > 100)
                        {
                            return Tuple.Create(result, $"第{rowIndex + 1}行,主组列:长度超出数据库字段限制");
                        }
                        if (subGroup.Length > 300)
                        {
                            return Tuple.Create(result, $"第{rowIndex + 1}行,子组列:长度超出数据库字段限制");
                        }
                        if (oeCode.Length > 100)
                        {
                            return Tuple.Create(result, $"第{rowIndex + 1}行,OE号列:长度超出数据库字段限制");
                        }
                        if (oeName.Length > 1000)
                        {
                            return Tuple.Create(result, $"第{rowIndex + 1}行,OE名称列:长度超出数据库字段限制");
                        }
                        if (oeEnName.Length > 100)
                        {
                            return Tuple.Create(result, $"第{rowIndex + 1}行,OE英文名称列:长度超出数据库字段限制");
                        }
                        if (imageNo.Length > 20)
                        {
                            return Tuple.Create(result, $"第{rowIndex + 1}行,图号列:长度超出数据库字段限制");
                        }
                        if (position.Length > 20)
                        {
                            return Tuple.Create(result, $"第{rowIndex + 1}行,左右列:长度超出数据库字段限制");
                        }
                        if (dosage.Length > 20)
                        {
                            return Tuple.Create(result, $"第{rowIndex + 1}行,用量列:长度超出数据库字段限制");
                        }
                        if (remarks.Length > 300)
                        {
                            return Tuple.Create(result, $"第{rowIndex + 1}行,用法/备注列:长度超出数据库字段限制");
                        }
                        if (liYangId.Length > 4000)
                        {
                            return Tuple.Create(result, $"第{rowIndex + 1}行,力洋ID列:长度超出数据库字段限制");
                        }
                        #region Excel数据行插入到DataTable
                        var dtRow = dt.NewRow();
                        dtRow["Brand"] = brand ?? string.Empty;
                        dtRow["Series"] = series ?? string.Empty;
                        dtRow["Category"] = category ?? string.Empty;
                        dtRow["MainCategory"] = mainCategory ?? string.Empty;
                        dtRow["SubGroup"] = subGroup ?? string.Empty;
                        dtRow["OeCode"] = oeCode ?? string.Empty;
                        dtRow["OeName"] = oeName ?? string.Empty;
                        dtRow["OeEnName"] = oeEnName ?? string.Empty;
                        dtRow["ImageNo"] = imageNo ?? string.Empty;
                        dtRow["Position"] = position;
                        dtRow["InstallBeginTime"] = (DateTime.TryParse(installBeginTimeStr, out installBeginTime) && (installBeginTime < DateTime.MaxValue && installBeginTime > minDateTime)) ? installBeginTime : (DateTime?)null;
                        dtRow["InstallEndTime"] = (DateTime.TryParse(installEndTimeStr, out installEndTime) && (installEndTime < DateTime.MaxValue && installEndTime > minDateTime)) ? installEndTime : (DateTime?)null;
                        dtRow["Dosage"] = dosage;
                        dtRow["Price"] = Decimal.TryParse(priceStr, out price) ? price : (decimal?)null;
                        dtRow["Remarks"] = remarks;
                        dtRow["LiYangId"] = liYangId;
                        dtRow["BatchCode"] = task.BatchCode;
                        dtRow["AreaKey"] = task.BatchCode.Split('B').LastOrDefault();
                        dt.Rows.Add(dtRow);
                        #endregion
                    }
                    var uniqueDt = dt.AsEnumerable().GroupBy(row => new
                    {
                        Brand = row["Brand"],
                        Series = row["Series"],
                        Category = row["Category"],
                        MainCategory = row["MainCategory"],
                        SubGroup = row["SubGroup"],
                        OeCode = row["OeCode"],
                        OeName = row["OeName"],
                        OeEnName = row["OeEnName"],
                        ImageNo = row["ImageNo"],
                        Position = row["Position"],
                        InstallBeginTime = row["InstallBeginTime"],
                        InstallEndTime = row["InstallEndTime"],
                        Dosage = row["Dosage"],
                        Price = row["Price"],
                        Remarks = row["Remarks"],
                        LiYangId = row["LiYangId"]
                    }).Select(g => g.FirstOrDefault()).CopyToDataTable();
                    result = uniqueDt;
                }
                else
                {
                    message = "与模板不一致，请下载模板之后根据模板填写";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Tuple.Create(result, message);
        }

        /// <summary>
        /// 批量插入全车件数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public static bool BatchAddVehiclePartsLiYang(DataTable dt, UploadFileTask task)
        {
            var result = false;
            try
            {
                if (dt.Rows.Count > 0)
                {
                    using (SqlConnection conn = new SqlConnection(connectionRw))
                    {
                        conn.Open();
                        var trans = conn.BeginTransaction();
                        try
                        {
                            var Series = dt.Rows[0].GetValue<string>("Series");
                            var Brand = dt.Rows[0].GetValue<string>("Brand");
                            int areaKey;
                            if (int.TryParse(task.BatchCode.Split('B').LastOrDefault(), out areaKey) && areaKey > 0)
                            {
                                var brand = DalVehiclePartsLiYang.GetBrandById(conn, trans, areaKey);
                                if (string.IsNullOrEmpty(brand) || string.IsNullOrEmpty(Brand))
                                    throw new Exception("缺少品牌数据");
                                if (!brand.Contains(Brand.Trim()))
                                    throw new Exception("录入品牌数据与所选择品牌不一致");


                                if (DalVehiclePartsLiYang.DeleteVehiclePartsLiYangByBrandandSeries(conn, trans, areaKey, Series))
                                {
                                    DalVehiclePartsLiYang.BatchAddDalVehiclePartsLiYang(conn, trans, dt);
                                    var updateStatus = DalUploadFileTask.UpdateFileTaskStatus(task, FileStatus.Success);
                                    if (updateStatus)
                                    {
                                        trans.Commit();
                                        result = true;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            trans.Rollback();
                            Logger.Info("BatchAddVehiclePartsLiYang", ex);
                            DalUploadFileTask.SetErrrorFileStatus(task, FileStatus.Cancel, ex.Message);
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Info("BatchAddVehiclePartsLiYang", ex);
                DalUploadFileTask.SetErrrorFileStatus(task, FileStatus.Cancel, ex.Message);
            }
            return result;
        }
    }
}
