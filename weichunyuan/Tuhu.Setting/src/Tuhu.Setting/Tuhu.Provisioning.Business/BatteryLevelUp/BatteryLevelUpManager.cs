using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.Product;
using Tuhu.Provisioning.DataAccess.DAO.BaoYang;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Battery;
using Tuhu.Provisioning.DataAccess.Request;
using Tuhu.Service.BaoYang;
using Tuhu.Service.Product;

namespace Tuhu.Provisioning.Business.BatteryLevelUp
{
    public class BatteryLevelUpManager
    {
        private static readonly Common.Logging.ILog Logger;
        private static readonly IConnectionManager configConnString =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["BaoYang"].ConnectionString);
        private static readonly IConnectionManager configReadConnString =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["BaoYang_AlwaysOnRead"].ConnectionString);

        private static readonly IDBScopeManager dbScopeManagerConfigRead;
        private static readonly IDBScopeManager dbScopeManagerConfig;
        private static readonly IDBScopeManager dbScopeManagerBaoYangRead;
        private static readonly IDBScopeManager dbScopeManagerBaoYang;
        static BatteryLevelUpManager()
        {
            Logger = Common.Logging.LogManager.GetLogger(typeof(BatteryLevelUpManager));
            dbScopeManagerConfig = new DBScopeManager(configConnString);
            dbScopeManagerConfigRead = new DBScopeManager(configReadConnString);
            dbScopeManagerBaoYang = new DBScopeManager(ConfigurationManager.ConnectionStrings["BaoYang"].ConnectionString);
            dbScopeManagerBaoYangRead = new DBScopeManager(ConfigurationManager.ConnectionStrings["BaoYang_AlwaysOnRead"].ConnectionString);
        }
        /// <summary>
        /// 获取配置列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaoYangResultEntity<Tuple<int, List<BatteryLevelUpReslut>>> GetBatteryLeveUpList(BatteryLevelUpRequest model)
        {
            var result = new BaoYangResultEntity<Tuple<int, List<BatteryLevelUpReslut>>>() { Status = false };
            try
            {
                List<BatteryLevelUpReslut> list = new List<BatteryLevelUpReslut>();
                var btaaeryTuple = dbScopeManagerBaoYangRead.Execute(conn =>
                 {
                     var data = DALBatteryLevelUp.GetBatteryLeveUpList(conn, model);
                     return data;
                 });
                var pids = btaaeryTuple.Item2.Select(x => x.NewPID).ToList();
                pids.AddRange(btaaeryTuple.Item2.Select(x => x.OriginalPID).ToList());
                pids = pids.Distinct().ToList();
                var productDic = new ProductManager().SelectProductDetail(pids);
                foreach (var item in btaaeryTuple.Item2)
                {
                    BatteryLevelUpReslut resultModel = new BatteryLevelUpReslut()
                    {
                        Copywriting = item.Copywriting,
                        NewPID = item.NewPID,
                        OriginalPID = item.OriginalPID,
                        CreateDateTime = item.CreateDateTime,
                        IsEnabled = item.IsEnabled,
                        PKID = item.PKID,
                        LsatUpdateDateTime = item.LsatUpdateDateTime
                    };
                    if (productDic.ContainsKey(item.NewPID))
                    {
                        resultModel.NewDisplayName = productDic[item.NewPID].Name;
                    }
                    if (productDic.ContainsKey(item.OriginalPID))
                    {
                        resultModel.OriginalDisplayName = productDic[item.OriginalPID].Name;
                    }
                    list.Add(resultModel);
                }
                result.Data = Tuple.Create(btaaeryTuple.Item1, list);
                result.Status = true;
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                result = new BaoYangResultEntity<Tuple<int, List<BatteryLevelUpReslut>>>() { Status = false, Msg = "获取失败" };

            }
            return result;

        }
        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaoYangResultEntity<bool> InsertBatteryLevelUp(BatteryLevelUpEntity model, string name)
        {
            var result = new BaoYangResultEntity<bool>();
            try
            {
                result = SaveCheckBatteryLevelUp(model);
                if (result.Status)
                {
                    result = dbScopeManagerBaoYang.Execute(conn =>
                     {
                         result.Data = DALBatteryLevelUp.InsertBatteryLevelUp(conn, model);
                         return result;
                     });
                    if (result.Data)
                    {
                        SaveBatteryLevelUpLog(null, model, name);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                result = new BaoYangResultEntity<bool>() { Status = false, Msg = "修改失败" };
            }
            return result;
        }
        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns> 
        public BaoYangResultEntity<BatteryLevelUpEntity> GetBatteryLevelUpEntity(int pkid)
        {
            var result = new BaoYangResultEntity<BatteryLevelUpEntity>();
            try
            {
                result = dbScopeManagerBaoYangRead.Execute(conn =>
                 {
                     return new BaoYangResultEntity<BatteryLevelUpEntity> { Status = true, Data = DALBatteryLevelUp.GetBatteryLevelUpEntity(conn, pkid) };

                 });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                result = new BaoYangResultEntity<BatteryLevelUpEntity>() { Status = false, Msg = "获取失败" };
            }
            return result;
        }
        /// <summary>
        /// 修改配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaoYangResultEntity<bool> UpdateBatteryLevelUp(BatteryLevelUpEntity model, string name)
        {
            var result = new BaoYangResultEntity<bool>();
            try
            {
                result = SaveCheckBatteryLevelUp(model);
                if (result.Status)
                {
                    var oldbatteryLevelUp = GetBatteryLevelUpEntity(model.PKID).Data;
                    if (oldbatteryLevelUp != null)
                    {
                        result = dbScopeManagerBaoYang.Execute(conn =>
                        {
                            result.Data = DALBatteryLevelUp.UpdateBatteryLevelUp(conn, model);
                            return result;
                        });
                    }
                    if (result.Data)
                    {
                        SaveBatteryLevelUpLog(oldbatteryLevelUp, model, name);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                result = new BaoYangResultEntity<bool>() { Status = false, Msg = "修改失败" };
            }
            return result;
        }
        /// <summary>
        /// 保存时检查
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaoYangResultEntity<bool> SaveCheckBatteryLevelUp(BatteryLevelUpEntity model)
        {
            var result = new BaoYangResultEntity<bool>() { Status = true };
            try
            {
                result = dbScopeManagerBaoYangRead.Execute(conn =>
                 {
                     var entity = DALBatteryLevelUp.GetBatteryLevelUpEntityByOriginalPid(conn, model.OriginalPID);
                     //重复原始ID 查询
                     if (entity != null && entity.PKID != model.PKID)
                     {
                         result.Status = false;
                         result.Msg = "该原始产品已经配置了升级购";
                         return result;
                     }
                     entity = DALBatteryLevelUp.GetBatteryLevelUpEntityByNewPID(conn, model.NewPID);
                     if (entity != null && entity.PKID != model.PKID)
                     {
                         result.Status = false;
                         result.Msg = "该升级产品已经配置了升级购";
                         return result;
                     }
                     entity = DALBatteryLevelUp.GetBatteryLevelUpEntity(conn, model.OriginalPID, model.NewPID);
                     if (entity != null)
                     {
                         result.Status = false;
                         result.Msg = "升级链不能循环升级";
                         return result;
                     }
                     return result;
                 });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                result = new BaoYangResultEntity<bool>() { Status = false, Msg = "修改失败" };
            }
            return result;
        }

        /// <summary>
        /// 删除升级购配置
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public async Task<BaoYangResultEntity<bool>> DeleteBatteryLevelUpByPkid(int pkid, string name)
        {
            var result = new BaoYangResultEntity<bool>() { Status = true };
            try
            {
                var batteryLevelUp = GetBatteryLevelUpEntity(pkid).Data;
                if (batteryLevelUp != null)
                {
                    result = dbScopeManagerBaoYang.Execute(conn =>
                    {
                        result.Data = DALBatteryLevelUp.DeleteBatteryByPkid(conn, pkid);
                        return result;
                    });
                }
                if (result.Data)
                {
                    SaveBatteryLevelUpLog(batteryLevelUp, null, name);
                    var cacheResult = await RemoveCacheByTypeAsync("BatteryLevelUpConfig", new List<string> { batteryLevelUp.OriginalPID });
                    if (!cacheResult)
                    {
                        result.Status = false;
                        result.Msg = "删除成功 但删除缓存失败";
                        result.Data = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                result = new BaoYangResultEntity<bool>() { Status = false, Msg = "删除失败" };
            }
            return result;
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <returns></returns>
        public async Task<BaoYangResultEntity<bool>> RemoveBtaaeryLevelUpCache()
        {
            var result = new BaoYangResultEntity<bool>() { Status = true };
            try
            {
                var pids = dbScopeManagerBaoYang.Execute(conn => DALBatteryLevelUp.GetAllOriginalPID(conn));
                var type = "BatteryLevelUpConfig";
                result.Data = await RemoveCacheByTypeAsync(type, pids);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                result = new BaoYangResultEntity<bool>() { Status = true, Msg = "删除失败" };

            }
            return result;
        }
        /// <summary>
        /// 根据type和data清除缓存
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> RemoveCacheByTypeAsync(string type, List<string> data)
        {
            using (var cilent = new Service.BaoYang.CacheClient())
            {
                var cacheResult = await cilent.RemoveByTypeAsync(type, data);
                return cacheResult.Success;
            }
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public BaoYangResultEntity<bool> SaveBatteryLevelUpLog(BatteryLevelUpEntity oldbatteryLevelUp, BatteryLevelUpEntity newbatteryLevelUp, string name)
        {
            BaoYangResultEntity<bool> result = new BaoYangResultEntity<bool>() { Status = true };
            var log = new BaoYangOprLog()
            {
                CreateTime = DateTime.Now,
                LogType = "BaoYangProductBatteryLevelUpSetting",
                Remarks = oldbatteryLevelUp == null ? "添加" : (newbatteryLevelUp == null ? "删除" : "修改"),
                IdentityID = newbatteryLevelUp == null ? oldbatteryLevelUp.OriginalPID : newbatteryLevelUp.OriginalPID,
                NewValue = JsonConvert.SerializeObject(newbatteryLevelUp ?? new BatteryLevelUpEntity()),
                OldValue = JsonConvert.SerializeObject(oldbatteryLevelUp ?? new BatteryLevelUpEntity()),
                OperateUser = name
            };
            LoggerManager.InsertLog("BYOprLog", log);
            return result;
        }

    }
}
