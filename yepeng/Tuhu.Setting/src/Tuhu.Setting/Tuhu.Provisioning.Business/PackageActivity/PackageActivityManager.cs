using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO.PackageActivity;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.PackageActivity;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.BaoYang;

namespace Tuhu.Provisioning.Business.PackageActivity
{
    public class PackageActivityManager
    {
        private static readonly IConnectionManager ConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbReadScopeManager = null;

        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(PackageActivityManager));
        private static string LogType = "PackageActivity";

        public PackageActivityManager()
        {
            this.dbScopeManager = new DBScopeManager(ConnectionManager);
            this.dbReadScopeManager = new DBScopeManager(ConnectionReadManager);
        }

        public List<PackageActivityConfig> SelectPackageActivity(string activityID, string activityName, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            List<PackageActivityConfig> result = new List<PackageActivityConfig>();
            List<OrderLists> orderInfo = new List<OrderLists>();
            var activityId = Guid.Empty;
            try
            {
                dbReadScopeManager.Execute(conn =>
                {
                    if (!string.IsNullOrEmpty(activityID)) { activityId = Guid.Parse(activityID); }
                    result = DALPackageActivity.SelectPackageActivity(conn, activityId, activityName, startTime, endTime, pageIndex, pageSize);
                    if (result != null && result.Any())
                    {
                        result.ForEach(x => x.RoundConfig = DALPackageActivity.SelectPackageActivitySceneConfig(conn, x.ActivityId));
                    }
                });
                if (result != null && result.Any())
                {
                    foreach (var item in result)
                    {
                        if (item.RoundConfig != null && item.RoundConfig.Any())
                        {
                            var flashSaleLog = DALPackageActivity.SelectFlashSalRecordsLog(item.ActivityId);
                            if (flashSaleLog != null && flashSaleLog.Any())
                            {
                                flashSaleLog = flashSaleLog.GroupBy(x => x.OrderId).Select(g => g.FirstOrDefault()).ToList();
                                dbReadScopeManager.Execute(conn =>
                                {
                                     orderInfo = DALPackageActivity.SelectOrderInfo(conn, flashSaleLog.Select(x => x.OrderId).Distinct().ToList());
                                });
                                flashSaleLog = (from fs in flashSaleLog
                                                join or in orderInfo on fs.OrderId equals or.OrderId into temp
                                                from t in temp.DefaultIfEmpty()
                                                select new FlashSaleRecordsLog()
                                                {
                                                    CreateDate = fs.CreateDate,
                                                    OrderStatus = t != null ? t.Status : string.Empty,
                                                    OrderId = t != null ? t.OrderId : 0
                                                }).Where(x => !string.Equals(x.OrderStatus, "7Canceled") && x.OrderId > 0).ToList();
                            }
                            item.RoundConfig.ForEach(_ =>
                            {
                                _.SurplusNum = _.LimitedQuantity - flashSaleLog.Where(x => x.CreateDate >= _.StartTime && x.CreateDate <= _.EndTime).Count();
                            });
                            item.AllSurplusNum = item.MaxSaleQuantity - flashSaleLog.Count();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public PackageActivityConfig SelectPackageActivityConfig(Guid activityId)
        {
            PackageActivityConfig result = new PackageActivityConfig();

            try
            {
                dbReadScopeManager.Execute(conn =>
                {
                    result = DALPackageActivity.SelectPackageActivity(conn, activityId, "", null, null, 1, 99).FirstOrDefault();
                    if (result != null && result.ActivityId != Guid.Empty)
                    {
                        result.ShopConfig = DALPackageActivity.SelectPackageActivityShopConfig(conn, activityId);
                        result.RoundConfig = DALPackageActivity.SelectPackageActivitySceneConfig(conn, activityId);
                        result.ProductConfig = DALPackageActivity.SelectPackageActivityProductConfig(conn, activityId);
                        result.VehicleConfig = DALPackageActivity.SelectPackageActivityVehicleConfig(conn, activityId);
                        result.PriceConfig = DALPackageActivity.SelectPackageActivityPriceConfig(conn, activityId);
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public bool UpsertPaceageActivityConfig(PackageActivityConfig data, string user)
        {
            var result = false;
            string msg = "";

            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    var flag = false;
                    if (data.ActivityId != Guid.Empty)
                    {
                        msg = "更新活动配置";
                        flag = DALPackageActivity.UpdatePackageActivityConfig(conn, data, user);
                    }
                    else
                    {
                        msg = "添加活动配置";
                        data.ActivityId = Guid.NewGuid();
                        flag = DALPackageActivity.InsertPackageActivityConfig(conn, data, user);
                    }

                    if (flag)
                    {
                        DALPackageActivity.DeletePackageActivitySceneConfig(conn, data.ActivityId);
                        DALPackageActivity.DeletePackageActivityShopConfig(conn, data.ActivityId);
                        DALPackageActivity.DeletePackageActivityProductConfig(conn, data.ActivityId);
                        DALPackageActivity.DeletePackageActivityVehicleConfig(conn, data.ActivityId);
                        DALPackageActivity.DeletePackageActivityPriceConfig(conn, data.ActivityId);
                        if (data.RoundConfig != null && data.RoundConfig.Any())
                        {
                            foreach (var item in data.RoundConfig)
                            {
                                item.ActivityId = data.ActivityId;
                                DALPackageActivity.InsertPackageActivitySceneConfig(conn, item);
                            }
                        }
                        if (data.ProductConfig != null && data.ProductConfig.Any())
                        {
                            foreach (var item in data.ProductConfig)
                            {
                                item.ActivityId = data.ActivityId;
                                DALPackageActivity.InsertPackageActivityProductConfig(conn, item);
                            }
                        }
                        if (data.ShopConfig != null && data.ShopConfig.Any())
                        {
                            foreach (var item in data.ShopConfig)
                            {
                                item.ActivityId = data.ActivityId;
                                DALPackageActivity.InsertPackageActivityShopConfig(conn, item);
                            }
                        }
                        if (data.VehicleConfig != null && data.VehicleConfig.Any())
                        {
                            var vehicleIds = data.VehicleConfig.Select(x => x.VehicleID).Distinct();
                            foreach (var vehicleId in vehicleIds)
                            {
                                DALPackageActivity.InsertPackageActivityVehicleConfig(conn, data.ActivityId, vehicleId);
                            }
                        }
                        if (data.PriceConfig != null && data.PriceConfig.Any())
                        {
                            var priceConfigs = data.PriceConfig;
                            foreach (var priceConfig in priceConfigs)
                            {
                                DALPackageActivity.InsertPackageActivityPriceConfig(conn, data.ActivityId, priceConfig);
                            }
                        }
                        result = true;
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            if (result)
            {
                data.ProductConfig = new List<PackageActivityProductConfig>();
                data.RoundConfig = new List<PackageActivityRoundConfig>();
                data.VehicleConfig = new List<PackageActivityVehicleConfig>();
                RecordActivityTypeLog(data.ActivityId, data.StartTime, data.EndTime);
                InsertLog("UpsertPaceageActivityConfig", data.ActivityId.ToString(), JsonConvert.SerializeObject(data), msg, user, LogType);
                Thread.Sleep(2000);
                RefreshPackageBaoYangCache(data.ActivityId);
            }

            return result;
        }

        public bool DeletePackageActivityConfig(Guid activityId, string user)
        {
            var result = false;

            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    DALPackageActivity.DeleteBaoYangPricingConfig(conn, activityId);
                    DALPackageActivity.DeletePackageActivitySceneConfig(conn, activityId);
                    DALPackageActivity.DeletePackageActivityShopConfig(conn, activityId);
                    DALPackageActivity.DeletePackageActivityProductConfig(conn, activityId);
                    DALPackageActivity.DeletePackageActivityPriceConfig(conn, activityId);
                    result = true;
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            if (result)
            {
                InsertLog("DeletePackageActivityConfig", activityId.ToString(), "删除配置", "删除配置", user, LogType);
                Thread.Sleep(2000);
                RefreshPackageBaoYangCache(activityId);
            }

            return result;
        }

        public IEnumerable<Tuple<string, string>> GetBaoYangPacekageType()
        {
            IEnumerable<Tuple<string, string>> result = new List<Tuple<string, string>>();
            try
            {
                using (var client = new BaoYangClient())
                {
                    var data = client.GetBaoYangPackageDescription();
                    if (!data.Success && data.Exception != null)
                        throw data.Exception;
                    if (data.Result != null && data.Result.Any())
                    {
                        result = data.Result.Where(_ => !string.IsNullOrEmpty(_.PackageType) && !string.IsNullOrEmpty(_.ZhName)).Select(x => new Tuple<string, string>(x.PackageType, x.ZhName)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public bool RefreshPackageBaoYangCache(Guid activityId)
        {
            var result = false;
            try
            {
                UpdateFixedActivityCache(activityId);
                UpdateBaoYangActivityConfig(activityId);
                result = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        #region  服务

        public void RecordActivityTypeLog(Guid activityId,DateTime startTime,DateTime endTime)
        {
            try
            {
                ActivityTypeRequest request = new ActivityTypeRequest()
                {
                    ActivityId = activityId,
                    Type = 5,
                    Status = 1,
                    StartDateTime = startTime,
                    EndDateTime = endTime

                };
                using (var client = new ActivityClient())
                {
                    var getResult = client.RecordActivityTypeLog(request);
                    if (!getResult.Success && getResult != null)
                        throw getResult.Exception;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void UpdateBaoYangActivityConfig(Guid activityId)
        {
            try
            {
                using (var client = new ActivityClient())
                {
                    var getResult = client.UpdateBaoYangActivityConfig(activityId);
                    if (!getResult.Success && getResult != null)
                        throw getResult.Exception;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void UpdateFixedActivityCache(Guid activityId)
        {
            try
            {
                using (var client = new Service.BaoYang.CacheClient())
                {
                    var getResult = client.UpdateFixedActivityCache(activityId);
                    if (!getResult.Success && getResult != null)
                        throw getResult.Exception;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }


        #endregion

        #region 日志
        /// <summary>
        /// 插入日志
        /// </summary>
        /// <param name="method"></param>
        /// <param name="objectId"></param>
        /// <param name="remarks"></param>
        /// <param name="msg"></param>
        /// <param name="opera"></param>
        /// <param name="type"></param>
        public static void InsertLog(string method, string objectId, string remarks, string msg, string opera, string type)
        {
            try
            {
                PackageActivityLog info = new PackageActivityLog
                {
                    ObjectId = objectId,
                    Method = method,
                    Message = msg,
                    Remarks = remarks,
                    Operator = opera,
                    Type = type.Trim(),
                    CreatedTime = DateTime.Now,
                    LastUpdateDateTime = DateTime.Now
                };
                LoggerManager.InsertLog("PackageActivityLog", info);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<PackageActivityLog> SelectOperationLog(string objectId)
        {
            List<PackageActivityLog> result = new List<PackageActivityLog>();
            try
            {
                result = DALPackageActivity.SelectOperationLog(objectId, LogType);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
        #endregion

        #region Vehicle

        public List<string> GetAllVehicleBrandCategory()
        {
            List<string> result = null;
            try
            {
                result = dbReadScopeManager.Execute(conn => DALPackageActivity.SelectAllVehicleBrandCategory(conn))?.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return result ?? new List<string>();
        }

        public List<string> GetAllVehicleBrand()
        {
            List<string> result = null;
            try
            {
                result = dbReadScopeManager.Execute(conn => DALPackageActivity.SelectAllVehicleBrand(conn))?.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return result ?? new List<string>();
        }

        /// <summary>
        /// 查询车型
        /// </summary>
        /// <param name="series"></param>
        /// <param name="brands"></param>
        /// <param name="excludeVehicleIds"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <returns></returns>
        public Tuple<int, List<VehicleSimpleInfo>> GetVehicles(IEnumerable<string> series,
            IEnumerable<string> brands, IEnumerable<string> excludeVehicleIds, double minPrice, double maxPrice, int index, int size)
        {
            List<VehicleSimpleInfo> result = null;
            int total = 0;
            try
            {
                var temp = dbReadScopeManager.Execute(conn => DALPackageActivity.SelectVehicles(conn, series,
                    brands, excludeVehicleIds, minPrice, maxPrice, index, size));
                result = temp.Item2?.ToList();
                total = temp.Item1;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            return Tuple.Create(total, result ?? new List<VehicleSimpleInfo>());
        }

        #endregion
    }
}
