using System.Collections.Generic;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using System;
using System.Data;
using Newtonsoft.Json;
using Tuhu.Service.ConfigLog;
using Tuhu.Service.OprLog;
using Tuhu.Service.OprLog.Models;
using AutoMapper;
using Tuhu.Service;
using System.Linq;
using Tuhu.Component.Framework;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.CommonEnum;

namespace Tuhu.Provisioning.Business.Logger
{
    public class LoggerManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("LoggerManager");
        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="author">操作人</param>
        /// <param name="objectType">类型  WebAct网站活动</param>
        /// <param name="objectID">活动ID</param>
        /// <param name="operation">操作名称</param>
        /// <returns></returns>
        public static bool InsertOplog(string author, string objectType, int objectID, string operation)
        {
            OprLogManager oprLog = new OprLogManager();
            OprLog model = new OprLog();
            model.Author = author;
            model.ObjectID = objectID;
            model.ObjectType = objectType;
            model.Operation = operation;
            //int i =  DALLogger.InsertOprLog(objectType, objectID, operation, author);
            try
            {
                oprLog.AddOprLog(model);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static IEnumerable<ConfigHistory> SelectOprLogByObjectTypeAndAftervalue(string objectType, string afterValue)
        {
            IEnumerable<ConfigHistory> result = null;
            switch (objectType)
            {
                case "TireTJ":
                case "T-ListAct":
                    result = DALLogger.SelectOprLogByObjectTypeAndAftervalue(objectType, afterValue);
                    break;
                default:
                    using (var client = new OprLogClient())
                    {
                        var oprlist = client.SelectOrderOprLog(0, objectType);
                        if (oprlist != null && oprlist.Result.Count() > 0)
                        {
                            var tempList = oprlist.Result.Where(t => t.AfterValue.Equals(afterValue) || t.AfterValue.Equals(afterValue + ",")).ToList();
                            //AutoMapper初始化配置文件
                            var config = new MapperConfiguration(cfg => cfg.CreateMap<OprLogModel, ConfigHistory>());
                            var mapper = config.CreateMapper();
                            //集合类型转换
                            result = mapper.Map<IEnumerable<ConfigHistory>>(tempList);
                        }
                    }
                    break;
            }
            return result;
        }
        public static bool InsertOplog(ConfigHistory model)
        {
            //AutoMapper初始化配置文件
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ConfigHistory, OprLogModel>());
            var mapper = config.CreateMapper();
            //集合类型转换
            var oprLogModel = mapper.Map<OprLogModel>(model);
            bool flag = false;
            using (var client = new OprLogClient())
            {
                var result = client.AddOprLog(oprLogModel);
                if (result.Result > 0)
                    flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 获取操作日志列表信息
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public List<ConfigHistory> GetList(string objectType, string startDT, string endDT)
        {
            return DALLogger.SelectOprLog(objectType, startDT, endDT);
        }


        public static ConfigHistory GetConfigHistory(string objectID)
        {
            return DALLogger.GetConfigHistory(objectID);
        }
        public static FlashSaleProductOprLog GetFlashSaleHistoryByPkid(int pkid)
        {
            return DALLogger.GetFlashSaleHistoryByPkid(pkid);
        }

        public static List<QiangGouProductModel> SelectFlashSaleProductsLog(string hashKey)
        {
            return DALLogger.SelectFlashSaleProductsLog(hashKey);
        }
        public static IEnumerable<ConfigHistory> SelectOprLogByObjectType(string objectType)
        {
            OperationResult<IEnumerable<OprLogModel>> oprlist = null;
            using (var client = new OprLogClient())
            {
                oprlist = client.SelectOrderOprLog(0, objectType);
            }
            //AutoMapper初始化配置文件
            var config = new MapperConfiguration(cfg => cfg.CreateMap<OprLogModel, ConfigHistory>());
            var mapper = config.CreateMapper();
            //集合类型转换
            return mapper.Map<IEnumerable<ConfigHistory>>(oprlist.Result);
        }

        public static IEnumerable<ConfigHistory> SelectOprLogByParams(string objectType, string objectId)
        {
            OperationResult<IEnumerable<OprLogModel>> oprlist = null;
            using (var client = new OprLogClient())
            {
                oprlist = client.SelectOrderOprLog(int.Parse(objectId), objectType);
            }
            //AutoMapper初始化配置文件
            var config = new MapperConfiguration(cfg => cfg.CreateMap<OprLogModel, ConfigHistory>());
            var mapper = config.CreateMapper();
            //集合类型转换
            return mapper.Map<IEnumerable<ConfigHistory>>(oprlist.Result);
        }
        #region 产品关联车型操作日志
        public static bool InsertOpLogForProductVehicleTypeConfig(ProductVehicleTypeConfigOpLog logEntity)
        {
            if (logEntity != null)
            {
                return InsertLog("PrdVehicleOprLog", logEntity);
            }
            return false;
        }

        public static void BulkSaveOperateLogInfo(List<ProductVehicleTypeConfigOpLog> tbList)
        {
            foreach (var item in tbList)
            {
                InsertLog("PrdVehicleOprLog", item);
            }
        }

        public static List<ProductVehicleTypeConfigOpLog> GetAllLogByTime(string timeS, string timeE)
        {
            return DALLogger.GetAllLogByTime(timeS, timeE);
        }
        public static List<ConfigHistory> SelectConfigHistory(string objectID, string objectType)
        {
            return DALLogger.SelectConfigHistory(objectID, objectType);
        }

        public static List<FlashSaleProductOprLog> SelectFlashSaleHistoryByLogId(string logId, string logType = "FlashSaleLog")
        {
            return DALLogger.SelectFlashSaleHistoryByLogId(logId, logType);
        }
        public static FlashSaleProductOprLog SelectFlashSaleHistoryDetailByLogId(int pkid)
        {
            return DALLogger.SelectFlashSaleHistoryDetailByLogId(pkid);
        }

        /// <summary>
        /// 查询城市失效log
        /// </summary>
        /// <param name="logId"></param>
        /// <param name="logType"></param>
        /// <returns></returns>
        public static List<FlashSaleProductOprLog> SelectCityAgingHistoryByLogId(string logId, string logType)
        {
            return DALLogger.SelectCityAgingHistoryByLogId(logId, logType);
        }

        /// <summary>
        /// 通用日志查询方法 
        /// </summary>
        /// <param name="logId"></param>
        /// <param name="logType"></param>
        /// <returns></returns>
        public static List<CommonOprLog> SelectOpLogHistoryByLogId(string tableName, string logId,
            string logType)
        {
            return DALLogger.SelectOpLogHistoryByLogId(tableName,logId, logType);
        }

        public static List<ProductVehicleTypeConfigOpLog> GetAllLogByPid(string pid)
        {
            return DALLogger.GetAllLogByPid(pid);
        }

        public static List<ProductVehicleTypeConfigOpLog> GetAllLog()
        {
            return DALLogger.GetAllLog();
        }

        public static bool InsertLog(string type, object data)
        {
            var result = true;
            try
            {
                using (var client = new ConfigLogClient())
                {
                    var status = client.InsertDefaultLogQueue(type, JsonConvert.SerializeObject(data));
                    if (!status.Success)
                    {
                        status.ThrowIfException(true);
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"通用日志==》InsertLog执行错误{ex}-{ex.InnerException}-{ex.StackTrace}");
            }
            return result;
        }
        public static async Task<bool> InsertLogAsync(string type, object data)
        {
            var result = true;
            try
            {
                using (var client = new ConfigLogClient())
                {
                    var status = await client.InsertDefaultLogQueueAsync(type, JsonConvert.SerializeObject(data));
                    if (!status.Success)
                    {
                        status.ThrowIfException(true);
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"通用日志==》InsertLog执行错误{ex}-{ex.InnerException}-{ex.StackTrace}");
            }
            return result;
        }

        #endregion

        #region
        public static bool InserUploadFiletLog(string filePath, FileType type, string fileName, string batchCode, string user)
        {
            var result = false;
            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(user))
            {
                var data = new UploadFileTaskLog
                {
                    FilePath = filePath,
                    FileName = fileName,
                    BatchCode = batchCode,
                    Type = type.ToString(),
                    Status = FileStatus.Wait.ToString(),
                    CreateUser = user
                };
                using (var client = new ConfigLogClient())
                {
                    var getResult = client.InsertDefaultLogQueue("UploadFileTaskLog", JsonConvert.SerializeObject(data));
                    getResult.ThrowIfException(true);
                    if (getResult.Success && getResult.Exception == null)
                        result = false;
                }
            }
            return result;
        }
        #endregion

        public static bool InsertFlashSaleLog(QiangGouModel model, string hashkey)
        {
            var result = true;
            try
            {
                //if (model.Products.Count() > 10)
                //{
                //    result = DALLogger.BatchInsertFlashSaleProductsLog(model, hashkey);
                //}
                //else
                //{
                using (var client = new ConfigLogClient())
                {
                    foreach (var item in model.Products)
                    {
                        item.HashKey = hashkey;
                        var status = client.InsertDefaultLogQueue("FlashSaleProductsLog", JsonConvert.SerializeObject(item));
                        result = result && status.Success;
                    }
                }
                //}
            }
            catch (Exception ex)
            {

                Logger.Log(Level.Error, $"InsertFlashSaleLog执行错误{ex}-{ex.InnerException}-{ex.StackTrace}");
                result = false;
                throw;
            }
            return result;

        }
    }
}
