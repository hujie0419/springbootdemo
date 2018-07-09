using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO.ProductVehicleInfoDao;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ProductVehicleType
{
    class ProductVehicleInfoHandler
    {
        private readonly IDBScopeManager dbManager, dbManagerReadOnly;
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(ProductVehicleInfoHandler));

        public ProductVehicleInfoHandler(IDBScopeManager dbMgr, IDBScopeManager dbMgrReadOnly)
        {
            this.dbManager = dbMgr;
            this.dbManagerReadOnly = dbMgrReadOnly;
        }

        public List<ProductInfo> SearchProductInfoByParam(string pidOrDisplayName, int pageIndex, int pageSize, int type)
        {
            List<ProductInfo> result = new List<ProductInfo>();
            try
            {
                dbManagerReadOnly.Execute(conn =>
                {
                    if (type == 1 || type == 2)
                    {
                        result = DalProductVehicleInfo.SelectProductInfo(conn, pidOrDisplayName, pageIndex, pageSize, type);
                    }
                    else
                    {
                        result = DalProductVehicleInfo.SearchProductInfoByParam(conn, pidOrDisplayName, pageIndex, pageSize, type);
                    }

                    if (result != null && result.Any() && type != 2)
                    {
                        var data = DalProductVehicleInfo.SelectProductConfigInfoByPIDs(conn, result.Select(x => x.PID).ToList());
                        if (data != null && data.Any())
                        {
                            result.ForEach(x =>
                            {
                                x.UpdateTime = data.Where(y => y.PID == x.PID).Select(_ => _.UpdateTime).FirstOrDefault() ?? string.Empty;
                            });
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Monitor.ExceptionMonitor.AddNewMonitor("列表页查询产品信息异常", e, "目标检索", MonitorLevel.Critial, MonitorModule.Other);
                return new List<ProductInfo>();
            }
            return result;
        }

        public ProductInfo GetProductInfoByPid(string pid)
        {
            try
            {
                Func<SqlConnection, ProductInfo> action = (connection) => DalProductVehicleInfo.GetProductInfoByPid(connection, pid);
                return dbManagerReadOnly.Execute(action);
            }
            catch (Exception e)
            {
                Monitor.ExceptionMonitor.AddNewMonitor("根据PID检索产品信息异常", e, "检索异常", MonitorLevel.Critial, MonitorModule.Other);
                return new ProductInfo();
            }

        }

        public List<ProductVehicleTypeFileInfoDb> GetExcelInfoByPid(string pid)
        {
            try
            {
                Func<SqlConnection, List<ProductVehicleTypeFileInfoDb>> action = (connection) => DalProductVehicleInfo.GetExcelInfoByPid(connection, pid);
                return dbManagerReadOnly.Execute(action);
            }
            catch (Exception e)
            {
                Monitor.ExceptionMonitor.AddNewMonitor("根据PID检索产品上传的Excel配置异常", e, "检索异常", MonitorLevel.Critial, MonitorModule.Other);
                return new List<ProductVehicleTypeFileInfoDb>();
            }
        }

        public bool SaveProductVehicleExcelInfo(ProductVehicleTypeFileInfoDb entity)
        {
            try
            {
                Func<SqlConnection, bool> action = (connection) => DalProductVehicleInfo.SaveProductVehicleExcelInfo(connection, entity);
                return dbManager.Execute(action);
            }
            catch (Exception e)
            {
                Monitor.ExceptionMonitor.AddNewMonitor("保存上传Excel信息异常", e, "保存数据异常", MonitorLevel.Critial, MonitorModule.Other);
                return false;
            }
        }

        public List<ProductInfo> GetProductInfoByPids(string pids)
        {
            try
            {
                Func<SqlConnection, List<ProductInfo>> action = (connection) => DalProductVehicleInfo.GetProductInfoByPids(connection, pids);
                return dbManagerReadOnly.Execute(action);
            }
            catch (Exception e)
            {
                Monitor.ExceptionMonitor.AddNewMonitor("复制配置页面根据录入PIDs查询产品信息异常", e, "检索异常", MonitorLevel.Critial, MonitorModule.Other);
                return new List<ProductInfo>();
            }
        }

        public bool SaveProductInfoByPid(string pid, string cpremark, bool isAuto,string vehicleLevel)
        {
            try
            {
                int isAutometic = isAuto ? 1 : 0;
                #region 车型信息变更 则删除原有车型信息配置
                bool configNotChange = DalProductVehicleInfo.IsExistProductVehicleTypeConfig(pid, vehicleLevel);
                bool? deleteResult = null;
                if (!configNotChange)
                {
                    Func<SqlConnection, bool> deleteAction = (connection) => DalProductVehicleInfo.DeleteOldProductVehicleTypeConfig(connection, pid);
                    deleteResult=dbManager.Execute(deleteAction);
                    if(deleteResult !=true)
                    {
                        throw new Exception("删除旧车型配置失败");
                    }
                }
                #endregion
                Func<SqlConnection, bool> action = (connection) => DalProductVehicleInfo.SaveProductInfoByPid(connection, pid, cpremark, isAutometic,vehicleLevel);
                return dbManager.Execute(action);
            }
            catch (Exception e)
            {
                Monitor.ExceptionMonitor.AddNewMonitor("编辑页保存车型级别异常", e, "保存数据异常", MonitorLevel.Critial, MonitorModule.Other);
                return false;
            }
        }

        /// <summary>
        /// 添加或更新车型配置信息
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="table"></param>
        /// <param name="fileName"></param>
        /// <param name="cpremark"></param>
        /// <returns></returns>
        public bool InsertOrUpdateVehicleInfoByPID(string pid, DataTable table, string fileName, string cpremark)
        {
            var result = true;
            var vehicleIdList = new List<string>();
            var insertTidList = new List<string>();
            var updateTidList = new List<string>();
            var deleteVehicleIdList = new List<string>();
            var deleteTidList = new List<string>();
            var batchCount = 1000;
            try
            {
                dbManager.CreateTransaction(conn =>
                {
                    var pidInfo = DalProductVehicleInfo.GetAllProductVehicleTypeConfigInfoByPid(conn, pid, fileName, cpremark);
                    VerifyAndConvertData(pidInfo, table, fileName, cpremark, out vehicleIdList, out insertTidList, out updateTidList, out deleteVehicleIdList, out deleteTidList);
                    if (deleteVehicleIdList.Any())//删除的二级车型信息
                    {
                        while (deleteVehicleIdList.Any())
                        {
                            if (deleteVehicleIdList.Count() < batchCount)
                            {
                                batchCount = deleteVehicleIdList.Count();
                            }
                            DalProductVehicleInfo.BatchDeleteSecondVehicleTypeConfig(conn, deleteVehicleIdList.GetRange(0, batchCount), pid);
                            deleteVehicleIdList.RemoveRange(0, batchCount);
                        }
                    }
                    if (deleteTidList.Any())//删除的五级车型信息
                    {
                        while (deleteTidList.Any())
                        {
                            if (deleteTidList.Count() < batchCount)
                            {
                                batchCount = deleteTidList.Count();
                            }
                            DalProductVehicleInfo.BatchDeleteFiveVehicleTypeConfig(conn, deleteTidList.GetRange(0, batchCount), pid);
                            deleteTidList.RemoveRange(0, batchCount);
                        }
                    }
                    if (vehicleIdList.Any())//增加的二级车型信息
                    {
                        while (vehicleIdList.Any())
                        {
                            if (vehicleIdList.Count() < batchCount)
                            {
                                batchCount = vehicleIdList.Count();
                            }
                            DalProductVehicleInfo.BatchInsertSecondVehicleTypeConfig(conn, vehicleIdList.GetRange(0, batchCount), pid);
                            vehicleIdList.RemoveRange(0, batchCount);
                        }
                    }
                    if (insertTidList.Any())//增加的五级车型信息
                    {
                        while (insertTidList.Any())
                        {
                            if (insertTidList.Count() < batchCount)
                            {
                                batchCount = insertTidList.Count();
                            }
                            DalProductVehicleInfo.BatchInsertFiveVehicleTypeConfig(conn, insertTidList.GetRange(0, batchCount), pid);
                            insertTidList.RemoveRange(0, batchCount);
                        }
                    }
                    if (updateTidList.Any())//需要更新的五级车型信息
                    {
                        while (updateTidList.Any())
                        {
                            if (updateTidList.Count() < batchCount)
                            {
                                batchCount = updateTidList.Count();
                            }
                            DalProductVehicleInfo.BatchUpdateFiveVehicleTypeConfig(conn, updateTidList.GetRange(0, batchCount), pid);
                            updateTidList.RemoveRange(0, batchCount);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error(ex);
            }

            return result;
        }


        /// <summary>
        /// 验证及转换数据
        /// </summary>
        /// <param name="pidInfo"></param>
        /// <param name="table"></param>
        /// <param name="fileName"></param>
        /// <param name="cpremark"></param>
        /// <param name="vehicleIdList"></param>
        /// <param name="insertTidList"></param>
        /// <param name="updateTidList"></param>
        /// <param name="deleteVehicleIdList"></param>
        /// <param name="deleteTidList"></param>
        private void VerifyAndConvertData(List<ProductVehicleTypeConfigDb> pidInfo, DataTable table, string fileName, string cpremark,
           out List<string> vehicleIdList, out List<string> insertTidList, out List<string> updateTidList, out List<string> deleteVehicleIdList, out List<string> deleteTidList)
        {
            vehicleIdList = new List<string>();
            insertTidList = new List<string>();
            updateTidList = new List<string>();
            deleteVehicleIdList = new List<string>();
            deleteTidList = new List<string>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (cpremark.Contains("二级"))
                {
                    var vehicleId = table.Rows[i][0].ToString().Trim();
                    var isEnabled = table.Rows[i][3].ToString().Trim();
                    if (isEnabled == "0")
                    {
                        deleteVehicleIdList.Add(vehicleId);
                    }
                    else if (!pidInfo.Exists(item => !string.IsNullOrEmpty(item.VehicleID) && item.VehicleID == vehicleId))
                    {
                        vehicleIdList.Add(vehicleId);
                    }
                }
                else if (cpremark.Contains("五级"))
                {
                    var vehicleId = table.Rows[i][2].ToString().Trim();
                    var tid = table.Rows[i][3].ToString().Trim();
                    var paiLiang = table.Rows[i][4].ToString().Trim();
                    var nian = table.Rows[i][5].ToString().Trim();
                    var salesName = table.Rows[i][8].ToString().Trim();
                    var pidItem = pidInfo.Where(x => !string.IsNullOrEmpty(tid) && x.TID == tid).FirstOrDefault() ?? null;
                    var isEnabled = table.Rows[i][9].ToString().Trim();
                    if (isEnabled == "0")
                    {
                        deleteTidList.Add(tid);
                    }
                    else if (pidItem != null)
                    {
                        //有变化则更新
                        if (pidItem.VehicleID != vehicleId || pidItem.PaiLiang != paiLiang || pidItem.Nian != nian || pidItem.SalesName != salesName)
                        {
                            updateTidList.Add(tid);
                        }
                    }
                    else
                    {
                        insertTidList.Add(tid);
                    }
                }
            }
        }        

        public bool UpdateVehicleInfoByPid(string pid, DataTable table, string level, string vehicleLevel)
        {
            var isSuccess = true;
            try
            {
                dbManagerReadOnly.Execute(conn =>
                {
                    var resultList = DalProductVehicleInfo.GetAllProductVehicleTypeConfigInfoByPid(conn, pid, level, vehicleLevel);
                    if (level.Contains("二级"))
                    {
                        for (int i = table.Rows.Count - 1; i >= 0; i--)
                        {
                            var vehicleId = table.Rows[i][1].ToString();
                            //var nian = table.Rows[i][4].ToString();
                            //var paiLiang = table.Rows[i][5].ToString();
                            if (resultList.Exists(item => item.VehicleID == vehicleId))
                            {
                                table.Rows.Remove(table.Rows[i]);//找到重复的就删掉不更新
                                continue;
                            }
                        }
                    }
                    else if (level.Contains("四级"))
                    {
                        for (int i = table.Rows.Count - 1; i >= 0; i--)
                        {
                            var vehicleId = table.Rows[i][1].ToString();
                            var nian = table.Rows[i][4].ToString();
                            var paiLiang = table.Rows[i][5].ToString();
                            if (resultList.Exists(item => item.VehicleID == vehicleId && item.Nian == nian && item.PaiLiang == paiLiang))
                            {
                                table.Rows.Remove(table.Rows[i]);
                                continue;
                            }

                        }
                    }
                    else if (level.Contains("五级"))
                    {
                        for (int i = table.Rows.Count - 1; i >= 0; i--)
                        {
                            //var tid = table.Rows[i][6].ToString();
                            var vehicleId = table.Rows[i][1].ToString();
                            var paiLiang = table.Rows[i][5].ToString();
                            var nian = table.Rows[i][4].ToString();
                            var salesName = table.Rows[i][7].ToString();
                            if (resultList.Exists(item => item.VehicleID == vehicleId && item.PaiLiang == paiLiang && item.Nian == nian && item.SalesName == salesName))
                            {
                                //如果已存在相同数据则去重
                                table.Rows.Remove(table.Rows[i]);
                                continue;
                            }
                        }
                    }
                    else
                    {

                    }
                    //isSuccess = DalProductVehicleInfo.BulkSaveVehicleInfoByPid(conn, table, level, cpremark);

                });

                Func<SqlConnection, bool> action = (connection) => DalProductVehicleInfo.BulkSaveVehicleInfoByPid(connection, table, level, vehicleLevel);
                dbManager.Execute(action);
            }
            catch (Exception e)
            {
                isSuccess = false;
                Monitor.ExceptionMonitor.AddNewMonitor("编辑页保存车型配置数据异常", e, "保存异常", MonitorLevel.Critial, MonitorModule.Other);
            }

            //try
            //{
            //    //var count = table.Rows.Count;
            //    //先去重，去数据库捞一把防止重复数据插入
            //    if (level.Contains("二级"))
            //    {
            //        Func<SqlConnection, List<ProductVehicleTypeConfigDb>> actionOne = (connection) => DalProductVehicleInfo.GetAllProductVehicleTypeConfigInfoByPid(connection, pid, level, cpremark);
            //        var resultList = dbManager.Execute(actionOne);
            //        try
            //        {
            //            for (int i = table.Rows.Count - 1; i >= 0; i--)
            //            {
            //                var vehicleId = table.Rows[i][1].ToString();
            //                //var nian = table.Rows[i][4].ToString();
            //                //var paiLiang = table.Rows[i][5].ToString();
            //                if (resultList.Exists(item => item.VehicleID == vehicleId))
            //                {
            //                    table.Rows.Remove(table.Rows[i]);//找到重复的就删掉不更新
            //                    //table.Rows[i].Delete();
            //                    continue;
            //                }
            //            }
            //            //table.AcceptChanges();
            //        }
            //        catch (Exception ex)
            //        {
            //            isSuccess = false;
            //        }

            //    }
            //    else if (level.Contains("四级"))
            //    {
            //        Func<SqlConnection, List<ProductVehicleTypeConfigDb>> actionTwo = (connection) => DalProductVehicleInfo.GetAllProductVehicleTypeConfigInfoByPid(connection, pid, level, cpremark);
            //        var resultList = dbManager.Execute(actionTwo);
            //        try
            //        {
            //            for (int i = table.Rows.Count - 1; i >= 0; i--)
            //            {
            //                var vehicleId = table.Rows[i][1].ToString();
            //                var nian = table.Rows[i][4].ToString();
            //                var paiLiang = table.Rows[i][5].ToString();
            //                if (resultList.Exists(item => item.VehicleID == vehicleId && item.Nian == nian && item.PaiLiang == paiLiang))
            //                {
            //                    table.Rows.Remove(table.Rows[i]);
            //                    continue;
            //                }

            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            isSuccess = false;
            //        }
            //    }
            //    else if (level.Contains("五级"))
            //    {
            //        Func<SqlConnection, List<ProductVehicleTypeConfigDb>> actionThree = (connection) => DalProductVehicleInfo.GetAllProductVehicleTypeConfigInfoByPid(connection, pid, level, cpremark);
            //        var resultList = dbManager.Execute(actionThree);
            //        try
            //        {
            //            for (int i = table.Rows.Count - 1; i >= 0; i--)
            //            {
            //                var tid = table.Rows[i][6].ToString();

            //                if (resultList.Exists(item => item.TID == tid))
            //                {
            //                    //如果已存在相同数据则去重
            //                    table.Rows.Remove(table.Rows[i]);
            //                    continue;
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            isSuccess = false;
            //        }
            //    }

            //    Func<SqlConnection, bool> action = (connection) => DalProductVehicleInfo.BulkSaveVehicleInfoByPid(connection, table, level, cpremark);
            //    dbManager.Execute(action);
            //}
            //catch (Exception e)
            //{
            //    isSuccess = false;
            //}

            return isSuccess;
        }

        public bool InsertProductVehicleTypeConfigInfoByPid(string pid, string destPids, string remark,string vehicleLevel)
        {
            try
            {
                Func<SqlConnection, bool> action = (connection) => DalProductVehicleInfo.InsertProductVehicleTypeConfigInfoByPid(connection, pid, destPids, remark, vehicleLevel);
                return dbManager.Execute(action);
            }
            catch (Exception e)
            {
                Monitor.ExceptionMonitor.AddNewMonitor("批量复制产品配置异常", e, "复制数据异常", MonitorLevel.Critial, MonitorModule.Other);
                return false;
            }

        }

        public List<ProductVehicleTypeConfigDb> GetProductVehicleTypeConfigInfoListByPid(string pid)
        {
            try
            {
                Func<SqlConnection, List<ProductVehicleTypeConfigDb>> action = (connection) => DalProductVehicleInfo.GetProductVehicleTypeConfigInfoListByPid(connection, pid);
                return dbManagerReadOnly.Execute(action);
            }
            catch (Exception e)
            {
                Monitor.ExceptionMonitor.AddNewMonitor("根据PID查询产品车型配置信息异常", e, "检索异常", MonitorLevel.Critial, MonitorModule.Other);
                return new List<ProductVehicleTypeConfigDb>();
            }
        }

        public bool DeleteProductVehicleTypeConfigByParams(List<ProductVehicleTypeConfigDb> deleteList)
        {
            try
            {
                Func<SqlConnection, bool> action = (connection) => DalProductVehicleInfo.DeleteProductVehicleTypeConfigByParams(connection, deleteList);
                return dbManager.Execute(action);
            }
            catch (Exception e)
            {
                Monitor.ExceptionMonitor.AddNewMonitor("根据条件删除产品车型配置异常", e, "删除异常", MonitorLevel.Critial, MonitorModule.Other);
                return false;
            }
        }

        public List<ProductInfo> GetAllNoImportProduct(int pageIndex, int pageSize)
        {
            try
            {
                return DalProductVehicleInfo.SelectAllNoImportProduct(pageIndex, pageSize);
            }
            catch (Exception e)
            {
                Monitor.ExceptionMonitor.AddNewMonitor("查询已配置未导入商品异常", e, "目标检索", MonitorLevel.Critial, MonitorModule.Other);
                return new List<ProductInfo>();
            }
        }
    }
}
