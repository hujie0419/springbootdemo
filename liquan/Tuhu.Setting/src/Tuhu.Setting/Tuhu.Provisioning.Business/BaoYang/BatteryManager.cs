using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO.BaoYang;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Battery;
using Tuhu.Provisioning.DataAccess.Request;
using Tuhu.Service.BaoYang;
using Tuhu.Service.Shop;

namespace Tuhu.Provisioning.Business.BaoYang
{
    public class BatteryManager
    {
        private static readonly Common.Logging.ILog Logger = LogManager.GetLogger(typeof(BatteryManager));

        private readonly IDBScopeManager dbScopeManagerBaoYang;
        private readonly IDBScopeManager dbScopeManagerBaoYangRead;
        private readonly IDBScopeManager dbScopeManagerProductRead;

        private readonly DalBattery _battery;

        private readonly string _operator;

        public BatteryManager(string @operator)
        {
            _operator = @operator;
            var connectionManagerBaoYang = new ConnectionManager(ConfigurationManager.ConnectionStrings["BaoYang"].ConnectionString);
            var connectionManagerBaoYangRead = new ConnectionManager(ConfigurationManager.ConnectionStrings["BaoYang_AlwaysOnRead"].ConnectionString);
            var connectionManagerProductRead = new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_productcatalog_AlwaysOnRead"].ConnectionString);
            dbScopeManagerBaoYang = new DBScopeManager(connectionManagerBaoYang);
            dbScopeManagerBaoYangRead = new DBScopeManager(connectionManagerBaoYangRead);
            dbScopeManagerProductRead = new DBScopeManager(connectionManagerProductRead);

            _battery = new DalBattery();
        }

        #region Common

        public List<string> GetBatteryBrands()
        {
            List<string> result = null;
            try
            {
                var list = dbScopeManagerBaoYangRead.Execute(conn => _battery.SelectBatteryBrands(conn));
                result = list.ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result ?? new List<string>();
        }

        public Service.Shop.Models.Region.Region GetRegionByRegionId(int regionId)
        {
            using (var client = new RegionClient())
            {
                var serviceResult = client.GetRegionByRegionId(regionId);
                serviceResult.ThrowIfException(true);
                return serviceResult.Result;
            }
        }

        public Service.Shop.Models.Region.Region GetRegionByRegionName(string regionName)
        {
            using (var client = new RegionClient())
            {
                var serviceResult = client.GetRegionByRegionName(regionName);
                serviceResult.ThrowIfException(true);
                return serviceResult.Result;
            }
        }

        public bool RemoveCache(IEnumerable<string> keys)
        {
            using (var client = new Service.BaoYang.CacheClient())
            {
                var serviceResult = client.Remove(keys);
                return serviceResult.Result;
            }
        }

        #endregion

        #region 保养流程蓄电池

        /// <summary>
        /// 这个是保养流程的，还有一个是蓄电池流程
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<BaoYangBatteryCoverArea> GetBaoYangBatteryCoverAreaList(SearchBaoYangBatteryCoverAreaRequest request)
        {
            List<BaoYangBatteryCoverArea> list = null;
            try
            {
                var result = dbScopeManagerBaoYangRead.Execute(conn => _battery.SelectBaoYangBatteryCoverAreas(conn, request));
                list = result.ToList();
                var regions = list.Select(x => x.ProvinceId).Distinct().Select(x => GetRegionByRegionId(x));
                list.ForEach(x =>
                {
                    var region = regions.FirstOrDefault(r => r != null && r.ProvinceId == x.ProvinceId);
                    if (region != null)
                    {
                        //直辖市只到市
                        if (region.IsMunicipality)
                        {
                            if (region.ProvinceId == x.CityId)
                            {
                                x.CityName = region.ProvinceName;
                                x.ProvinceName = region.ProvinceName;
                            }
                        }
                        else
                        {
                            var city = region.ChildRegions?.FirstOrDefault(cr => cr != null && cr.CityId == x.CityId);
                            if (city != null)
                            {
                                x.CityName = city.CityName;
                                x.ProvinceName = city.ProvinceName;
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return list ?? new List<BaoYangBatteryCoverArea>();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddBaoYangBatteryCoverArea(BaoYangBatteryCoverArea item)
        {
            var success = false;
            try
            {
                item.PKID = dbScopeManagerBaoYang.Execute(conn => _battery.AddBaoYangBatteryCoverArea(conn, item));
                if (item.PKID > 0)
                {
                    success = true;
                }
                if (success)
                {
                    var key = $"BaoYangBatteryCoverAreaConfigs/{{0}}/{item.CityId}";
                    RemoveCache(new[] { key });
                    InsertBaoYangOprLog(new BaoYangOprLog
                    {
                        Remarks = "Add",
                        OperateUser = _operator,
                        NewValue = JsonConvert.SerializeObject(item),
                        OldValue = string.Empty,
                        IdentityID = item.PKID.ToString(),
                        LogType = "BaoYangBatteryCoverArea",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return success;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool UpdateBaoYangBatteryCoverArea(BaoYangBatteryCoverArea item)
        {
            var success = false;
            try
            {
                var oldData = dbScopeManagerBaoYangRead.Execute(conn => _battery.SelectBaoYangBatteryCoverAreaById(conn, item.PKID));
                if (oldData != null)
                {
                    item.Brand = oldData.Brand;
                    item.CreateDatetime = DateTime.Now;
                    success = dbScopeManagerBaoYang.Execute(conn => _battery.UpdateBaoYangBatteryCoverArea(conn, item));
                    item.LastUpdateDateTime = DateTime.Now;
                }
                if (success)
                {
                    var key1 = $"BaoYangBatteryCoverAreaConfigs/{{0}}/{item.CityId}";
                    var key2 = $"BaoYangBatteryCoverAreaConfigs/{{0}}/{oldData.CityId}";
                    RemoveCache(new[] { key1, key2 });
                    InsertBaoYangOprLog(new BaoYangOprLog
                    {
                        Remarks = "Update",
                        OperateUser = _operator,
                        NewValue = JsonConvert.SerializeObject(item),
                        OldValue = JsonConvert.SerializeObject(oldData),
                        IdentityID = item.PKID.ToString(),
                        LogType = "BaoYangBatteryCoverArea",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return success;
        }

        /// <summary>
        /// 根据PKID删除覆盖区域
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public bool DeleteBaoYangBatteryCoverArea(long pkid)
        {
            var success = false;
            try
            {
                var oldData = dbScopeManagerBaoYangRead.Execute(conn => _battery.SelectBaoYangBatteryCoverAreaById(conn, pkid));
                if (oldData != null)
                {
                    success = dbScopeManagerBaoYang.Execute(conn => _battery.DeleteBaoYangBatteryCoverArea(conn, pkid));
                }
                if (success)
                {
                    var key = $"BaoYangBatteryCoverAreaConfigs/{{0}}/{oldData.CityId}";
                    RemoveCache(new[] { key });
                    InsertBaoYangOprLog(new BaoYangOprLog
                    {
                        Remarks = "Delete",
                        OperateUser = _operator,
                        NewValue = string.Empty,
                        OldValue = JsonConvert.SerializeObject(oldData),
                        IdentityID = oldData.PKID.ToString(),
                        LogType = "BaoYangBatteryCoverArea",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return success;
        }

        /// <summary>
        /// 是否存在相同数据
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool ExistsBaoYangBatteryCoverArea(BaoYangBatteryCoverArea item)
        {
            var exists = true;
            try
            {
                exists = dbScopeManagerBaoYangRead.Execute(conn => _battery.IsExistsBaoYangBatteryCoverArea(conn, item));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return exists;
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public List<BaoYangBatteryCoverArea> GetAllBaoYangBatteryCoverArea()
        {
            List<BaoYangBatteryCoverArea> result = null;
            try
            {
                result = dbScopeManagerBaoYangRead.Execute(conn => _battery.SelectAllBaoYangBatteryCoverAreas(conn)).ToList();

                var regions = result.Select(x => x.ProvinceId).Distinct().Select(x => GetRegionByRegionId(x));
                result.ForEach(x =>
                {
                    var region = regions.FirstOrDefault(r => r != null && r.ProvinceId == x.ProvinceId);
                    if (region != null)
                    {
                        //直辖市只到市
                        if (region.IsMunicipality)
                        {
                            if (region.ProvinceId == x.CityId)
                            {
                                x.CityName = region.ProvinceName;
                                x.ProvinceName = region.ProvinceName;
                            }
                        }
                        else
                        {
                            var city = region.ChildRegions?.FirstOrDefault(cr => cr != null && cr.CityId == x.CityId);
                            if (city != null)
                            {
                                x.CityName = city.CityName;
                                x.ProvinceName = city.ProvinceName;
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return result ?? new List<BaoYangBatteryCoverArea>();
        }

        public bool BatchUpdateBaoYangBatteryCoverArea(List<BaoYangBatteryCoverArea> list)
        {
            var success = false;
            try
            {
                var exists = dbScopeManagerBaoYangRead.Execute(conn => _battery.SelectAllBaoYangBatteryCoverAreas(conn)).ToList();
                list.ForEach(x =>
                {
                    var existItem = exists.FirstOrDefault(o => o.Brand == x.Brand && o.CityId == x.CityId && o.ProvinceId == x.ProvinceId);
                    //存在就更新，不存在则新增
                    x.PKID = existItem != null ? existItem.PKID : 0;
                });
                success = BatchUpdateBatteryCoverArea(list);
                if (success)
                {
                    var keys = list.Select(x => $"BaoYangBatteryCoverAreaConfigs/{{0}}/{x.CityId}");
                    RemoveCache(keys);
                    var logs = new List<BaoYangOprLog>();
                    foreach (var item in list)
                    {
                        var existItem = exists.FirstOrDefault(o => o.Brand == item.Brand && o.CityId == item.CityId && o.ProvinceId == item.ProvinceId);
                        logs.Add(new BaoYangOprLog
                        {
                            Remarks = existItem == null ? "Add" : "Update",
                            OperateUser = _operator,
                            NewValue = JsonConvert.SerializeObject(item),
                            OldValue = existItem == null ? string.Empty : JsonConvert.SerializeObject(existItem),
                            IdentityID = item.PKID.ToString(),
                            LogType = "BaoYangBatteryCoverArea",
                        });
                        InsertBaoYangOprLogs(logs);
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
            return success;
        }

        private bool BatchUpdateBatteryCoverArea(List<BaoYangBatteryCoverArea> list)
        {
            var success = false;
            dbScopeManagerBaoYang.CreateTransaction(conn =>
            {
                foreach (var item in list)
                {
                    if (item.PKID > 0)
                    {
                        _battery.UpdateBaoYangBatteryCoverArea(conn, item);
                    }
                    else
                    {
                        item.PKID = _battery.AddBaoYangBatteryCoverArea(conn, item);
                    }
                }
                success = true;
            });
            return success;
        }

        private bool InsertBaoYangOprLogs(List<BaoYangOprLog> logs)
        {
            if (logs != null && logs.Any())
            {
                logs.ForEach(log =>
                {
                    InsertBaoYangOprLog(log);
                });
                return true;
            }
            return false;
        }

        private bool InsertBaoYangOprLog(BaoYangOprLog log) => LoggerManager.InsertLog("BYOprLog", log);

        #endregion

    }
}
