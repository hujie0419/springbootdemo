using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
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

        private static readonly Common.Logging.ILog Logger = LogManager.GetLogger(nameof(SprayPaintVehicleManager));

        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(SprayPaintVehicleManager));

        public SprayPaintVehicleManager()
        {
            dbScopeManagerCF = new DBScopeManager(confConnectionManager);
            CFAlwaysOnReadDbScopeManager = new DBScopeManager(confAlwaysOnReadConnectionManager);
            GRAlwaysOnReadDbScopeManager = new DBScopeManager(grAlwaysOnReadConnectionManager);
        }

        /// <summary>
        /// 当前等级和类型的车型档次配置
        /// </summary>
        /// <param name="type"></param>
        /// <param name="vehicleLevel"></param>
        /// <returns></returns>
        public VehicleLevelModel SelectSprayPaintVehicleByLevel(string type, string vehicleLevel)
        {
            var result = new VehicleLevelModel();
            try
            {
                var configs = GRAlwaysOnReadDbScopeManager.Execute(conn =>
                  DalSprayPaintVehicle.SelectSprayPaintVehicleByLevel(conn, type, vehicleLevel));
                if (configs != null && configs.Any())
                {
                    result = Convert2VehicleLevels(configs).FirstOrDefault(s => s.VehicleLevel.Equals(vehicleLevel));
                    var serviceInfo = GetPaintServiceInfo();
                    var paintService = CFAlwaysOnReadDbScopeManager.Execute(conn => DalSprayPaintVehicle.SelectPaintServiceInfo(conn, result.VehicleLevel));
                    foreach (var service in paintService)
                    {
                        PaintService info = new PaintService
                        {
                            VehicleLevel = result.VehicleLevel,
                            ServiceId = service.ServiceId,
                            DisplayIndex = service.DisplayIndex,
                            ServiceName = serviceInfo.FirstOrDefault(p => p.ServiceId.Equals(service.ServiceId))?.ServersName
                        };
                        result.PaintService.Add(info);
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Error("SelectSprayPaintVehicleByLevel", ex);
            }
            return result;
        }

        /// <summary>
        ///  获取指定类型车型数据 包含不支持喷漆
        /// </summary>
        /// <returns></returns>
        private List<VehicleLevelModel> SelectSprayPaintVehicleByType(List<VehicleLevelViewModel> vehicInfo, string type)
        {
            var vehicleLevels = new List<VehicleLevelModel>();
            if (vehicInfo != null && vehicInfo.Any())
            {
                if (type == PaintVehicleLevelTypeEnum.Offline.ToString())
                { 
                    vehicleLevels = Convert2VehicleLevels(vehicInfo);
                    foreach (var item in vehicleLevels)
                    {
                        item.VehicleInfo = item.VehicleInfo.Where(x =>
                          string.IsNullOrEmpty(x.Type) || x.Type == type).ToList();
                    }
                }
                else
                {
                    vehicleLevels = Convert2VehicleLevels(vehicInfo, PaintVehicleLevelTypeEnum.Offline.ToString());
                }
                var serviceInfo = GetPaintServiceInfo();
                foreach (var item in vehicleLevels)
                {
                    var paintService = CFAlwaysOnReadDbScopeManager.Execute(conn => DalSprayPaintVehicle.SelectPaintServiceInfo(conn, item.VehicleLevel));
                    foreach (var service in paintService)
                    {
                        PaintService info = new PaintService
                        {
                            VehicleLevel = item.VehicleLevel,
                            ServiceId = service.ServiceId,
                            DisplayIndex = service.DisplayIndex,
                            ServiceName = serviceInfo.FirstOrDefault(p => p.ServiceId.Equals(service.ServiceId))?.ServersName
                        };
                        item.PaintService.Add(info);
                    }
                }
            }

            return vehicleLevels;
        }
        /// <summary>
        /// 更新喷漆车型档次分类
        /// </summary>
        /// <param name="vehicleLevel"></param>
        /// <param name="vehicleIds"></param>
        /// <param name="initalWord"></param>
        /// <param name="operateUser"></param>
        /// <returns></returns>
        public bool UpdatePaintVehicleLevel(string vehicleLevel, string vehicleIds, string initalWord, string type, string operateUser)
        {
            var result = false;
            try
            {
                var oldValue = SeletePaintVehicleByInitalWord(vehicleLevel, initalWord, type);
                var isValiad = type == PaintVehicleLevelTypeEnum.Offline.ToString()
                    && dbScopeManagerCF.Execute(conn => DalSprayPaintVehicle.IsRepeatPaintVehicleLevelConfig(conn, vehicleIds, type));
                var isDeleteSuccess = false;
                var isAddSuccess = false;

                if (isValiad)
                {
                    result = false;
                }
                else
                {
                    dbScopeManagerCF.CreateTransaction(conn =>
                    {

                        if (type == PaintVehicleLevelTypeEnum.Online.ToString())
                        {
                            //删除线下占用的车型喷漆
                            DalSprayPaintVehicle.DeletePaintVehicleDataByType(conn, vehicleIds, PaintVehicleLevelTypeEnum.Offline.ToString());
                        }
                        if (oldValue.Any())
                        {
                            isDeleteSuccess = DalSprayPaintVehicle.DeletePaintVehicleDataByInitalWord(conn, vehicleLevel, initalWord, type);
                        }
                        if (!string.IsNullOrWhiteSpace(vehicleIds))
                        {
                            result = DalSprayPaintVehicle.AddPaintVehicleLevel(conn, vehicleLevel, vehicleIds, type);
                            isAddSuccess = result;
                        }
                        else
                        {
                            result = true;
                        }

                    });
                }

                if (isDeleteSuccess)
                {
                    var toValue = new List<VehicleLevelLog>();
                    if (!string.IsNullOrWhiteSpace(vehicleIds))
                    {
                        foreach (var item in oldValue)
                        {
                            if (!vehicleIds.Split(',').Contains(item.VehicleId))
                            {
                                toValue.Add(item);
                            }
                        }
                    }
                    else
                    {
                        toValue = oldValue;
                    }
                    SavePaintVehicleLevelLog(toValue, "Delete", vehicleIds, operateUser);
                }
                if (isAddSuccess)
                {
                    Thread.Sleep(1000);
                    var toValue = new List<VehicleLevelLog>();
                    var newValue = SeletePaintVehicleByInitalWord(vehicleLevel, initalWord, type);
                    foreach (var newData in newValue)
                    {
                        {
                            if (!oldValue.Any(p => p.VehicleId.Equals(newData.VehicleId)))
                            {
                                toValue.Add(newData);
                            }
                        }
                    }
                    SavePaintVehicleLevelLog(newValue, "Add", vehicleIds, operateUser);
                }

            }
            catch (Exception ex)
            {
                Logger.Error("UpdatePaintVehicleLevel", ex);
            }
            return result;
        }
        /// <summary>
        /// 添加车型档次喷漆配置操作日志
        /// </summary>
        /// <param name="vehicleLevel"></param>
        /// <param name="vehicleIds"></param>
        /// <param name="initalWord"></param>
        /// <param name="type"></param>
        /// <param name="operateUser"></param>
        /// <returns></returns>
        private void SavePaintVehicleLevelLog(List<VehicleLevelLog> toValue, string Remarks, string vehicleIds,  string operateUser)
        {  
            if (toValue.Any())
            {
                foreach (var value in toValue)
                {
                    var log = new PaintVehicleOprLog
                    {
                        LogType = "PaintVehicleLevelConfig",
                        VehicleLevel = value.VehicleLevel,
                        VehicleId = value.VehicleId,
                        OldValue = Remarks == "Add" ? null : JsonConvert.SerializeObject(value),
                        NewValue = Remarks == "Delete" ? null : JsonConvert.SerializeObject(value),
                        Remarks = Remarks,
                        OperateUser = operateUser,
                    };
                    InsertLog("PaintVehicle", log);
                }
            }

        }

        /// <summary>
        /// 包装指定类型车型数据
        /// </summary>
        /// <param name="types">需要包含的类型</param>
        /// <param name="type">需要被分进不支持的类型</param>
        /// <returns></returns>
        private List<VehicleLevelModel> Convert2VehicleLevels
            (List<VehicleLevelViewModel> vehicleLevels, string type = "")
        {

            List<VehicleLevelModel> result = new List<VehicleLevelModel>();
            if (vehicleLevels != null)
            {
                foreach (var item in vehicleLevels)
                {
                    var VehicleInfo = new VehicleInfo()
                    {
                        Type = item.Type,
                        VehicleId = item.VehicleId,
                        VehicleSeries = item.VehicleSeries
                    };
                    if (string.IsNullOrEmpty(item.VehicleLevel) || item.Type == type)
                    {
                        if (result.Count(x => x.VehicleLevel == "不支持喷漆") > 0)
                        {
                            result.FirstOrDefault(x => x.VehicleLevel == "不支持喷漆").VehicleInfo.Add(VehicleInfo);
                        }
                        else
                        {
                            var resultItem = new VehicleLevelModel()
                            {
                                PaintService = new List<PaintService>(),
                                VehicleLevel = "不支持喷漆",
                                VehicleInfo = new List<VehicleInfo> { VehicleInfo }
                            };
                            result.Add(resultItem);
                        }
                    }
                    else
                    {
                        if (result.Count(x => x.VehicleLevel == item.VehicleLevel) > 0)
                        {
                            result.FirstOrDefault(x => x.VehicleLevel == item.VehicleLevel).VehicleInfo.Add(VehicleInfo);
                        }
                        else
                        {
                            var resultItem = new VehicleLevelModel()
                            {
                                PaintService = new List<PaintService>(),
                                VehicleLevel = item.VehicleLevel,
                                VehicleInfo = new List<VehicleInfo> { VehicleInfo }
                            };
                            result.Add(resultItem);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取指定首字母开头的车型档次，未指定则获取全部
        /// </summary>
        /// <param name="initalWord"></param>
        /// <returns></returns>
        private List<VehicleLevelViewModel> GetVehicleLevels(string initalWord = "")
            => CFAlwaysOnReadDbScopeManager.Execute(conn => DalSprayPaintVehicle.SelectVehicleLevel(conn, initalWord));
        /// <summary>
        /// 根据等级和品牌开头字母获取车型档次数据
        /// </summary>
        /// <param name="vehicleLevel"></param>
        /// <param name="initalWord"></param>
        /// <returns></returns>
        public List<VehicleLevelLog> SeletePaintVehicleByInitalWord(string vehicleLevel, string initalWord, string type)
        {
            var result = new List<VehicleLevelLog>();
            try
            {
                result = CFAlwaysOnReadDbScopeManager.Execute(conn => DalSprayPaintVehicle.SeletePaintVehicleByInitalWord(conn, vehicleLevel, initalWord, type));
            }
            catch (Exception ex)
            {
                Logger.Error("SeletePaintVehicleByInitalWord", ex);
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
                Logger.Error("GetPaintServiceInfo", ex); 
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
                Logger.Error("UpdatePaintVehicleService", ex); 
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
                Logger.Error("SelectPaintServiceInfo", ex); 
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
                Logger.Error("UpdatePaintVehicleCache", ex); 
                msg = ex.Message;
            }
            return new Tuple<bool, string>(result, msg);
        }
        /// <summary>
        /// 获取所有类型的喷漆车型档次分类
        /// </summary>
        /// <returns></returns>
        public Tuple<List<VehicleLevelModel>, List<VehicleLevelModel>> GetVehicleLevelsForAll()
        {
            Tuple<List<VehicleLevelModel>, List<VehicleLevelModel>> tuple;
            try
            {
                var configs = GetVehicleLevels();
                var online = SelectSprayPaintVehicleByType(configs, PaintVehicleLevelTypeEnum.Online.ToString());
                var offline = SelectSprayPaintVehicleByType(configs, PaintVehicleLevelTypeEnum.Offline.ToString());
                tuple = Tuple.Create(online, offline);
            }
            catch (Exception ex)
            {
                Logger.Error("GetVehicleLevelsForAll", ex); 
                tuple = Tuple.Create(new List<VehicleLevelModel>(), new List<VehicleLevelModel>());
            }
            return tuple;
        }

        /// <summary>
        /// 获取单个喷漆车型档次的选择信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="initalWord"></param>
        /// <param name="vehicleLevel"></param>
        /// <returns></returns>
        public List<TreeItem> GetVehicleInfo(string type, string initalWord, string vehicleLevel)
        {
            List<TreeItem> vehicleInfo;
            try
            {
                var configs = GetVehicleLevels(initalWord);
                vehicleInfo = Convert2TreeItem(configs);
                List<VehicleLevelModel> vehicleLevels = new List<VehicleLevelModel>();
                if (type == PaintVehicleLevelTypeEnum.Offline.ToString())
                {
                    var offlines = SelectSprayPaintVehicleByType(configs, type);
                    vehicleLevels = offlines.Where(t => !t.VehicleLevel.Equals("不支持喷漆")).ToList();
                    var onlines = SelectSprayPaintVehicleByType(configs, PaintVehicleLevelTypeEnum.Online.ToString())
                        .Where(t => !t.VehicleLevel.Equals("不支持喷漆"));
                    //避免与线上的喷漆数据冲突改名字
                    foreach (var item in onlines)
                    {
                        if (item.VehicleLevel == vehicleLevel)
                        {
                            item.VehicleLevel = $"online{item.VehicleLevel}";
                        }
                    }
                    vehicleLevels.AddRange(onlines);
                }
                else
                {
                    vehicleLevels = SelectSprayPaintVehicleByType(configs, type)
                        .Where(t => !t.VehicleLevel.Equals("不支持喷漆")).ToList();
                }
                var currentVehicleLevel = vehicleLevels.FirstOrDefault(p => p.VehicleLevel.Equals(vehicleLevel)) ?? new VehicleLevelModel();
                var CheckedVehicles = currentVehicleLevel.VehicleInfo;
                if (CheckedVehicles != null)
                {
                    foreach (var item in vehicleInfo)
                    {
                        foreach (var vehicle in item.children)
                        {
                            //设置选中状态
                            if (CheckedVehicles.Count(p => p.VehicleId.Equals(vehicle.id)) > 0)
                            {
                                vehicle.check = "true";
                            }
                            foreach (var level in vehicleLevels.Where(p => !p.VehicleLevel.Equals(vehicleLevel)))
                            {
                                //如果在其他车型档次中已经选中该车型，则当前档次中该车型不可选
                                if (level.VehicleInfo.Count(p => p.VehicleId.Equals(vehicle.id)) > 0)
                                {
                                    vehicle.disabled = "true";
                                }
                            }
                        }
                        //如果二级均为选中，则一级也设置为选中
                        if (item.children.Count(p => p.check.Equals("true")) == item.children.Count)
                        {
                            item.check = "true";
                        }
                        //如果二级均为不可选，则一级也设置为不可选
                        if (item.children.Count(p => p.disabled.Equals("true")) == item.children.Count)
                        {
                            item.disabled = "true";
                        }
                    }
                }
                else
                {
                    vehicleInfo = new List<TreeItem>();
                }
            }
            catch (Exception ex)
            {
                vehicleInfo = new List<TreeItem>();
                Logger.Error("GetVehicleInfo", ex); 
            }
            return vehicleInfo;
        }

        /// <summary>
        /// 配置Model转换为车型展示Model
        /// </summary>
        /// <param name="configs"></param>
        /// <returns></returns>
        private List<TreeItem> Convert2TreeItem(List<VehicleLevelViewModel> configs)
        {
            var result = new List<TreeItem>();
            if (configs != null)
            {
                var data = configs.GroupBy(p => p.Brand).ToDictionary(o => o.Key, o => o.Select(t => t));
                foreach (var item in data)
                {
                    var vehicle = new TreeItem
                    {
                        id = item.Key,
                        name = item.Key,
                        check = "false",
                        disabled = string.Empty
                    };
                    foreach (var node in item.Value)
                    {
                        var child = new TreeItem
                        {
                            id = node.VehicleId,
                            name = node.VehicleSeries,
                            check = "false",
                            disabled = string.Empty
                        };
                        vehicle.children.Add(child);
                    }
                    result.Add(vehicle);
                }
            }

            return result;
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
