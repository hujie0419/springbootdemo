using Common.Logging;
using ExcelDataReader;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Tuhu.C.YunYing.WinService.JobSchedulerService.DAL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.DLL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BLL
{
    public class LiYangId_LevelIdMapBusiness
    {
        private static readonly Common.Logging.ILog Logger = LogManager.GetLogger(typeof(LiYangId_LevelIdMapBusiness));

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
                if (titleRow.ItemArray[titleCellIndex++]?.ToString() == "力洋id" &&
                    titleRow.ItemArray[titleCellIndex++]?.ToString() == "Level ID")
                {
                    #region 初始化DataTable
                    var dt = new DataTable { };
                    dt.Columns.Add(new DataColumn("LiYangId", typeof(string)));
                    dt.Columns.Add(new DataColumn("LiYangLevelId", typeof(string)));
                    dt.Columns.Add(new DataColumn("CreateTime", typeof(DateTime)));
                    dt.Columns.Add(new DataColumn("LastUpdateTime", typeof(DateTime)));
                    #endregion
                    for (var rowIndex = 1; rowIndex < dsRows.Count; rowIndex++)
                    {
                        var row = dsRows[rowIndex];
                        if (row == null) continue;
                        var cellIndex = 0;
                        var liYangIdStr = row.ItemArray[cellIndex++].ToString()?.Trim();
                        var liYangLevelIdStr = row.ItemArray[cellIndex++].ToString()?.Trim();

                        #region Excel数据行插入到DataTable
                        var liYangIds = liYangIdStr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        var levelIds = liYangLevelIdStr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (liYangIds.Count() != levelIds.Count())
                        {
                            message = $"第{rowIndex + 1}行,请确保力洋Id和Level Id一一对应";
                            return Tuple.Create(result, message);
                        }
                        else
                        {
                            for (int i = 0; i < liYangIds.Count(); i++)
                            {
                                var dtRow = dt.NewRow();
                                dtRow["LiYangId"] = liYangIds[i] ?? string.Empty;
                                dtRow["LiYangLevelId"] = levelIds[i] ?? string.Empty;
                                dtRow["CreateTime"] = DateTime.Now;
                                dtRow["LastUpdateTime"] = DateTime.Now;
                                dt.Rows.Add(dtRow);
                            }
                        }
                        #endregion
                    }
                    var uniqueDt = dt.AsEnumerable().GroupBy(row => new
                    {
                        LiYangId = row["LiYangId"],
                        LiYangLevelId = row["LiYangLevelId"],
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
        /// 批量导入LiYangId和LiYangLevelId对应关系
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public static bool BatchAddLiYangId_LevelIdMap(DataTable dt, UploadFileTask task)
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
                            DalLiYangId_LevelIdMap.BatchAddLiYangId_LevelIdMap(conn, trans, dt);
                            var updateStatus = DalUploadFileTask.UpdateFileTaskStatus(task, FileStatus.Success);
                            if (updateStatus)
                            {
                                trans.Commit();
                                result = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            trans.Rollback();
                            Logger.Info("BatchAddLiYangId_LevelIdMap", ex);
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
                result = false;
                Logger.Info("BatchAddLiYangId_LevelIdMap", ex);
                DalUploadFileTask.SetErrrorFileStatus(task, FileStatus.Cancel, ex.Message);
            }
            return result;
        }
    }
}
