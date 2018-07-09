using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.ConfigLog;
using Tuhu.Service.Paint;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models;
using Tuhu.Service.Vehicle;

namespace Tuhu.Provisioning.Business.SprayPaintVehicle
{
    public class SprayPaintVehicleManager
    {
        private static readonly IConnectionManager confConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);

        private static readonly IConnectionManager confAlwaysOnReadConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);

        private static readonly IConnectionManager grAlwaysOnReadConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private readonly IDBScopeManager dbScopeManagerCF = null;
        private readonly IDBScopeManager CFAlwaysOnReadDbScopeManager = null;
        private readonly IDBScopeManager GRAlwaysOnReadDbScopeManager = null;

        private static readonly ILog Logger = LoggerFactory.GetLogger(nameof(SprayPaintVehicleManager));

        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(SprayPaintVehicleManager));

        public SprayPaintVehicleManager()
        {
            dbScopeManagerCF = new DBScopeManager(confConnectionManager);
            CFAlwaysOnReadDbScopeManager = new DBScopeManager(confAlwaysOnReadConnectionManager);
            GRAlwaysOnReadDbScopeManager = new DBScopeManager(grAlwaysOnReadConnectionManager);
        }

        public List<VehicleLevelModel> SelectSprayPaintVehicle()
        {
            var result = CFAlwaysOnReadDbScopeManager.Execute(conn => DalSprayPaintVehicle.SelectVehicleLevel(conn));
            var serviceInfo = GetPaintServiceInfo();
            foreach (var item in result)
            {
                var paintService = CFAlwaysOnReadDbScopeManager.Execute(conn => DalSprayPaintVehicle.SelectPaintServiceInfo(conn, item.VehicleLevel));
                foreach (var service in paintService)
                {
                    PaintService info = new PaintService
                    {
                        VehicleLevel = item.VehicleLevel,
                        ServiceId = service.ServiceId,
                        DisplayIndex = service.DisplayIndex,
                        ServiceName = serviceInfo.FirstOrDefault(p=>p.ServiceId.Equals(service.ServiceId))?.ServersName
                    };
                    item.PaintService.Add(info);
                }
            }
            return result;
        }

        public List<TreeItem> SelectVehicleInfo(string initialWord)
        {
            return GRAlwaysOnReadDbScopeManager.Execute(conn => DalSprayPaintVehicle.SelectVehicleInfo(conn, initialWord));
        }

        /// <summary>
        /// 更新喷漆车型档次分类
        /// </summary>
        /// <param name="vehicleLevel"></param>
        /// <param name="vehicleIds"></param>
        /// <param name="initalWord"></param>
        /// <param name="operateUser"></param>
        /// <returns></returns>
        public bool UpdatePaintVehicleLevel(string vehicleLevel, string vehicleIds, string initalWord, string operateUser)
        {
            var result = false;
            try
            {
                var oldValue = SeletePaintVehicleByInitalWord(vehicleLevel, initalWord);
                dbScopeManagerCF.CreateTransaction(conn =>
                {
                    if (oldValue.Any())
                    {
                        var isDeleteSuccess = DalSprayPaintVehicle.DeletePaintVehicleDataByInitalWord(conn, vehicleLevel, initalWord);                  
                        if (isDeleteSuccess)
                        {
                            var toDelete = new List<VehicleLevelLog>();
                            if (!string.IsNullOrWhiteSpace(vehicleIds))
                            {
                                foreach (var item in oldValue)
                                {
                                    if (!vehicleIds.Split(',').Contains(item.VehicleId))
                                    {
                                        toDelete.Add(item);
                                    }
                                }
                            }
                            else
                            {
                                toDelete = oldValue;
                            }
                            if (toDelete.Any())
                            {
                                foreach (var value in toDelete)
                                {
                                    var log = new PaintVehicleOprLog
                                    {
                                        LogType = "PaintVehicleLevelConfig",
                                        VehicleLevel = value.VehicleLevel,
                                        VehicleId = value.VehicleId,
                                        OldValue = JsonConvert.SerializeObject(value),
                                        NewValue = null,
                                        Remarks = "Delete",
                                        OperateUser = operateUser,
                                    };
                                    InsertLog("PaintVehicle", log);
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(vehicleIds))
                    {
                        result = DalSprayPaintVehicle.AddPaintVehicleLevel(conn, vehicleLevel, vehicleIds);
                        if (result)
                        {
                            Thread.Sleep(1000);
                            var newValue = SeletePaintVehicleByInitalWord(vehicleLevel, initalWord);
                            var toAdd = new List<VehicleLevelLog>();
                            foreach (var newData in newValue)
                            {
                                if (!oldValue.Any(p => p.VehicleId.Equals(newData.VehicleId)))
                                {
                                    toAdd.Add(newData);
                                }
                            }
                            if (toAdd.Any())
                            {
                                foreach (var value in toAdd)
                                {
                                    var log = new PaintVehicleOprLog
                                    {
                                        LogType = "PaintVehicleLevelConfig",
                                        VehicleLevel = value.VehicleLevel,
                                        VehicleId = value.VehicleId,
                                        OldValue = null,
                                        NewValue = JsonConvert.SerializeObject(value),
                                        Remarks = "Add",
                                        OperateUser = operateUser,
                                    };
                                    InsertLog("PaintVehicle", log);
                                }
                            }
                        }
                    }
                    else
                    {
                        result = true;
                    }
                });               
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "UpdatePaintVehicleLevel");
            }
            return result;
        }

        public List<VehicleLevelLog> SeletePaintVehicleByInitalWord(string vehicleLevel, string initalWord)
        {
            var result = new List<VehicleLevelLog>();
            try
            {
                result = CFAlwaysOnReadDbScopeManager.Execute(conn => DalSprayPaintVehicle.SeletePaintVehicleByInitalWord(conn, vehicleLevel, initalWord));
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "SeletePaintVehicleByInitalWord");
            }
            return result;
        }

        /// <summary>
        /// 获取喷漆服务信息
        /// </summary>
        /// <returns></returns>
        public List<ShopServiceModel> GetPaintServiceInfo()
        {
            var result = new List<ShopServiceModel>();
            try
            {
                using (var client = new ShopClient())
                {
                    var data = client.SelectAllServices();
                    if (data.Success)
                    {
                        if(data.Result != null)
                           result = data.Result.Where(p => p.ServersType.Equals("Paint"))?.ToList();
                    }
                    else
                    {
                        data.ThrowIfException(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetPaintServiceInfo");
            }
            return result;
        }

        /// <summary>
        /// 插入日志
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void InsertLog(string type, object data)
        {
            using (var client = new ConfigLogClient())
            {
                var status = client.InsertDefaultLogQueue(type, JsonConvert.SerializeObject(data));
                if (!status.Success)
                {
                    status.ThrowIfException(true);
                }
            }
        }

        /// <summary>
        /// 更新喷漆车型服务
        /// </summary>
        /// <param name="vehicleLevel"></param>
        /// <param name="paintService"></param>
        /// <param name="operateUser"></param>
        /// <returns></returns>
        public bool UpdatePaintVehicleService(string vehicleLevel, string paintService, string operateUser)
        {
            var result = false;
            var paintServiceModel = new List<PaintService>();
            if (!string.IsNullOrWhiteSpace(paintService))
            {
                paintServiceModel = JsonConvert.DeserializeObject<List<PaintService>>(paintService);
            }
            try
            {
                var oldValue = SelectPaintServiceInfo(vehicleLevel);
                dbScopeManagerCF.CreateTransaction(conn =>
                {
                    if (oldValue.Any())
                    {
                        var isDeleteSuccess = DalSprayPaintVehicle.DeletePaintService(conn, vehicleLevel);
                        var toDelete = new List<PaintServiceLog>();
                        foreach (var oldData in oldValue)
                        {
                            if (!paintServiceModel.Any(p => p.ServiceId.Equals(oldData.ServiceId)))
                            {
                                toDelete.Add(oldData);
                            }
                        }
                        if (isDeleteSuccess)
                        {
                            foreach (var value in toDelete)
                            {
                                var log = new PaintVehicleOprLog
                                {
                                    LogType = "PaintVehicleServiceConfig",
                                    VehicleLevel = value.VehicleLevel,
                                    VehicleId = null,
                                    OldValue = JsonConvert.SerializeObject(value),
                                    NewValue = null,
                                    Remarks = "Delete",
                                    OperateUser = operateUser,
                                };
                                InsertLog("PaintVehicle", log);
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(paintService) && paintServiceModel.Any())
                    {
                        foreach (var service in paintServiceModel)
                        {
                            result = DalSprayPaintVehicle.AddPaintVehicleServiceConfig(conn, service);
                        }
                        if (result)
                        {
                            Thread.Sleep(1000);
                            var newValue = SelectPaintServiceInfo(vehicleLevel);
                            var toAdd = new List<PaintServiceLog>();
                            foreach (var newData in newValue)
                            {
                                if (!oldValue.Any(p => p.ServiceId.Equals(newData.ServiceId)))
                                {
                                    toAdd.Add(newData);
                                }
                            }
                            foreach (var value in toAdd)
                            {
                                var log = new PaintVehicleOprLog
                                {
                                    LogType = "PaintVehicleServiceConfig",
                                    VehicleLevel = value.VehicleLevel,
                                    VehicleId = null,
                                    OldValue = null,
                                    NewValue = JsonConvert.SerializeObject(value),
                                    Remarks = "Add",
                                    OperateUser = operateUser,
                                };
                                InsertLog("PaintVehicle", log);
                            }
                        }
                    }
                    else
                    {
                        result = true;
                    }
                });
                
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "UpdatePaintVehicleService");
            }
            return result;
        }

        public List<PaintServiceLog> SelectPaintServiceInfo(string vehicleLevel)
        {
            var result = new List<PaintServiceLog>();
            try
            {
                result = CFAlwaysOnReadDbScopeManager.Execute(conn => DalSprayPaintVehicle.SelectPaintServiceInfo(conn, vehicleLevel));
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "SelectPaintServiceInfo");
            }
            return result;
        }

        public Tuple<bool,string> UpdatePaintVehicleCache(string type, string data)
        {
            var result = false;
            var msg = string.Empty;
            try
            {
                using (var client = new PaintClient())
                {
                    var update = client.UpdateCache(type, data);
                    if (!update.Success)
                    {
                        update.ThrowIfException(true);
                    }
                    result = update.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "UpdatePaintVehicleCache");
                msg = ex.Message;
            }
            return new Tuple<bool, string>(result, msg);
        }

        #region 根据vin码导出数据

        /// <summary>
        /// 根据vin码查询tids
        /// </summary>
        /// <param name="vin"></param>
        /// <returns></returns>
        private async Task<IEnumerable<string>> GetTidsByVIN(string vin)
        {
            var result = null as IEnumerable<string>;
            vin = vin?.Replace(" ", "");
            if (!string.IsNullOrEmpty(vin))
            {
                using (var client = new VehicleClient())
                {
                    var serviceResult = await client.GetTidsByVINAsync(vin, "setting");
                    serviceResult.ThrowIfException(true);
                    var resultModel = serviceResult.Result;
                    result = resultModel.Data;
                }
            }
            return result ?? new List<string>();
        }

        /// <summary>
        /// 根据Vins码查询tid
        /// </summary>
        /// <param name="vins"></param>
        /// <returns></returns>
        private async Task<List<Tuple<string, string>>> GetTidsByVIN(IEnumerable<string> vins)
        {
            vins = vins?.Distinct();
            var list = new List<Tuple<string, string>>();
            if (vins != null && vins.Any())
            {
                foreach (var vin in vins)
                {
                    var tids = await GetTidsByVIN(vin);
                    list.AddRange(tids.Select(tid => Tuple.Create(vin, tid)));
                }
            }
            return list;
        }

        /// <summary>
        /// 分批次根据Tid获取车型信息
        /// </summary>
        /// <param name="tids"></param>
        /// <returns></returns>
        private List<VehicleInfoDetail> GetVehicleInfoDetailsByTids(IEnumerable<string> tids)
        {
            var result = new List<VehicleInfoDetail>();
            var tidList = (tids ?? new List<string>()).Distinct()
                .Select(x => { int tid; int.TryParse(x, out tid); return tid; })
                .Where(tid => tid > 0).ToList();
            if (tidList.Any())
            {
                const int size = 100;
                for (var index = 0; index < Math.Ceiling(tidList.Count * 1.0 / size); index++)
                {
                    var vehicles = GetVehicleInfoDetailsByTids(tidList.Skip(index * size).Take(size));
                    if (vehicles != null)
                    {
                        result.AddRange(vehicles);
                    }
                    else
                    {
                        _logger.Info("发生异常停止");
                        break;
                    }

                }
            }
            return result;
        }

        /// <summary>
        /// 根据tids查询车型信息
        /// </summary>
        /// <param name="tids"></param>
        /// <returns></returns>
        private IEnumerable<VehicleInfoDetail> GetVehicleInfoDetailsByTids(IEnumerable<int> tids)
        {
            IEnumerable<VehicleInfoDetail> result = null;
            try
            {
                result = GRAlwaysOnReadDbScopeManager.Execute(conn => DalSprayPaintVehicle.SelectVehicleInfoByTids(conn, tids));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }

        /// <summary>
        /// 分批次根据vehicleId获取喷漆等级
        /// </summary>
        /// <param name="vehicleIds"></param>
        /// <returns></returns>
        private IEnumerable<Tuple<string, string>> GetSprayPaintLevelByVehicleIds(IEnumerable<string> vehicleIds)
        {
            IEnumerable<Tuple<string, string>> result = null;
            try
            {
                result = CFAlwaysOnReadDbScopeManager.Execute(conn => DalSprayPaintVehicle.SelectSprayPaintLevelsByVehicleIds(conn, vehicleIds));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }

        /// <summary>
        /// 根据所有vehicleId查询喷漆等级
        /// </summary>
        /// <param name="vehicleIds"></param>
        /// <returns></returns>
        private List<Tuple<string, string>> GetAllSprayPaintLevelByVehicleIds(IEnumerable<string> vehicleIds)
        {
            vehicleIds = vehicleIds?.Distinct();
            var result = new List<Tuple<string, string>>();
            if (vehicleIds != null && vehicleIds.Any())
            {
                const int size = 60;
                for (var i = 0; i < Math.Ceiling(vehicleIds.Count() * 1.0 / size); i++)
                {
                    var items = GetSprayPaintLevelByVehicleIds(vehicleIds.Skip(i * size).Take(size));
                    if (items != null)
                    {
                        result.AddRange(items);
                    }
                    else
                    {
                        _logger.Info("发生异常终止");
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="vins"></param>
        /// <returns></returns>
        public async Task<List<VinSprayPaintRelationship>> GetVinSprayPaintRelationResult(IEnumerable<string> vins)
        {
            var result = null as List<VinSprayPaintRelationship>;
            if (vins != null && vins.Any())
            {
                var list = await GetTidsByVIN(vins);
                var vehicles = GetVehicleInfoDetailsByTids(list.Select(x => x.Item2)).ToList();
                var vehicleLevel = GetAllSprayPaintLevelByVehicleIds(vehicles.Select(x => x.VehicleID));

                vehicles.ForEach(x =>
                {
                    x.VehicleLevel = vehicleLevel.FirstOrDefault(o => x.VehicleID == o.Item1)?.Item2;
                    x.VehicleLevel = !string.IsNullOrWhiteSpace(x.VehicleLevel) ? x.VehicleLevel : "不支持喷漆";
                });

                var result2 = list.Join(vehicles, x => x.Item2, y => y.TID, (x, y) => new
                {
                    Vin = x.Item1,
                    y.Vehicle,
                    y.Brand,
                    y.VehicleID,
                    y.VehicleLevel,
                }).Distinct().ToLookup(x => x.Vin).Select(x => new
                {
                    Vin = x.Key,
                    Detail = x.ToLookup(o => o.VehicleLevel).Select(o => new
                    {
                        VehicleLevel = o.Key,
                        Detail = o.Select(t => new { t.Vehicle, t.VehicleID, t.Brand }).Distinct().ToList()
                    }).ToList()
                }).ToList();

                result = result2.Select(x => new VinSprayPaintRelationship
                {
                    Vin = x.Vin,
                    SprayPaintLevel = x.Detail.Select(y => new VehicleSprayPaintLevel
                    {
                        VehicleLevel = y.VehicleLevel,
                        VehicleInfo = y.Detail.Select(z => new VehicleInfoSimpleInfo
                        {
                            Brand = z.Brand,
                            Vehicle = z.Vehicle,
                            VehicleID = z.VehicleID,
                        }).ToList()
                    }).ToList(),
                }).ToList();
            }
            return result ?? new List<VinSprayPaintRelationship>();
        }

        #endregion
    }
}
