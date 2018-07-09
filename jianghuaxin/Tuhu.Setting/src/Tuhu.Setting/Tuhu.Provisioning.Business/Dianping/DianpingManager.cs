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
using Tuhu.Provisioning.DataAccess.DAO.Dianping;
using Tuhu.Provisioning.DataAccess.Entity.Dianping;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models;

namespace Tuhu.Provisioning.Business.Dianping
{
    public class DianpingManager
    {
        private DalGroupConfig dalGroupConfig;
        private DalShopConfig dalShopConfig;

        private readonly IConnectionManager thirdpartyConnection;
        private readonly IDBScopeManager dbScopeManager = null;

        private readonly IConnectionManager thirdpartyReadConnection;
        private readonly IDBScopeManager dbScopeReadManager = null;

        private readonly Common.Logging.ILog logger;

        public DianpingManager()
        {
            this.thirdpartyConnection = new ConnectionManager(ConfigurationManager.ConnectionStrings["ThirdParty"].ConnectionString);
            this.dbScopeManager = new DBScopeManager(thirdpartyConnection);
            this.thirdpartyReadConnection = new ConnectionManager(ConfigurationManager.ConnectionStrings["ThirdPartyReadOnly"].ConnectionString);
            this.dbScopeReadManager = new DBScopeManager(thirdpartyReadConnection);
            this.dalGroupConfig = new DalGroupConfig();
            this.dalShopConfig = new DalShopConfig();
            this.logger = LogManager.GetLogger(typeof(DianpingManager));
        }

        #region Group Config

        public List<DianpingGroupConfig> GetGroupConfigs(int pageIndex, int pageSize,
            string dianpingId, string dianpingBrand, string dianpingName, string tuhuProductId, int status)
        {
            var groups = dbScopeReadManager.Execute(conn => dalGroupConfig.SelectGroupConfigs(conn, pageIndex, 
                pageSize, dianpingId, dianpingBrand, dianpingName, tuhuProductId, status));

            if(groups != null && groups.Any())
            {
                var ids = groups.Select(o=>o.TuhuProductId).Distinct();

                Dictionary<string, string> names = new Dictionary<string, string>();
                foreach(var id in ids)
                {
                    var name = GetTuhuProductName(id);
                    if (!string.IsNullOrEmpty(name))
                    {
                        names[id] = name;
                    }
                }

                foreach(var group in groups)
                {
                    if (names.ContainsKey(group.TuhuProductId))
                    {
                        group.TuhuProductName = names[group.TuhuProductId];
                    }
                }
            }

            return groups;
        }

        public int GetGroupConfigsCount(string dianpingId, 
            string dianpingBrand, string dianpingName, string tuhuProductId, int status)
        {
            return dbScopeReadManager.Execute(conn => dalGroupConfig.SelectGroupConfigsCount(conn, 
                dianpingId, dianpingBrand, dianpingName, tuhuProductId, status));
        }

        public string GetTuhuProductName(string tuhuProductId)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(tuhuProductId))
            {
                using (var client = new ShopClient())
                {
                    var serviceResult = client.GetBeautyProductDetailByPid(tuhuProductId);
                    if (serviceResult.Success && serviceResult.Result != null &&
                            !string.IsNullOrEmpty(serviceResult.Result.ProductName))
                    {
                        result = serviceResult.Result.ProductName;
                    }
                }
            }

            return result;
        }

        public int UpdateGroupConfig(DianpingGroupConfig groupConfig, string user)
        {
            int result = 0;

            if (groupConfig != null)
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    int rows = 0;
                    var current = dalGroupConfig.SelectDianpingGroupConfig(conn, groupConfig.DianpingGroupId);
                    if (current != null)
                    {
                        rows = dalGroupConfig.Update(conn, groupConfig);
                        result = rows == 1 ? 1 : 0;
                        if (result == 1)
                        {
                            var log = new
                            {
                                ObjectId = groupConfig.DianpingGroupId,
                                ObjectType = "UpdateGroupConfig",
                                BeforeValue = JsonConvert.SerializeObject(current),
                                AfterValue = JsonConvert.SerializeObject(groupConfig),
                                Remark = "",
                                Creator = user
                            };
                            LoggerManager.InsertLog("CommonConfigLog", log);
                        }
                    }
                    else
                    {
                        result = -1;
                    }
                });
            }

            return result;
        }

        public Tuple<string, int> BulkInsertGroupConfig(List<DianpingGroupConfig> configs, string user)
        {
            Tuple<string, int> result = null;

            List<dynamic> logs = new List<dynamic>();
            dbScopeManager.Execute(conn =>
            {
                var trans = conn.BeginTransaction();
                try
                {
                    foreach (var config in configs)
                    {
                        int perResult = 0;
                        try
                        {
                            var current = dalGroupConfig.SelectDianpingGroupConfig(conn, config.DianpingGroupId, trans);
                            if (current != null)
                            {
                                perResult = -1;
                            }
                            else
                            {
                                dalGroupConfig.Insert(conn, config, trans);
                                perResult = 1;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                            throw ex;
                        }
                        finally
                        {
                            if (perResult != 1)
                            {
                                result = Tuple.Create(config.DianpingGroupId, perResult);
                            }
                        }
                    }

                    if(result == null)
                    {
                        trans.Commit();
                    }
                    else
                    {
                        trans.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
            });

            if(result != null)
            {
                foreach(var config in configs)
                {
                    var log = new
                    {
                        ObjectId = config.DianpingGroupId,
                        ObjectType = "BulkInsertGroupConfig",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(config),
                        Remark = "",
                        Creator = user
                    };
                    LoggerManager.InsertLog("CommonConfigLog", log);
                }
            }

            return result;
        }

        public int InsertGroupConfig(DianpingGroupConfig groupConfig, string user)
        {
            int result = 0;

            if (groupConfig != null)
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    int rows = 0;
                    var current = dalGroupConfig.SelectDianpingGroupConfig(conn, groupConfig.DianpingGroupId);
                    if (current != null)
                    {
                        result = -1;
                    }
                    else
                    {
                        rows = dalGroupConfig.Insert(conn, groupConfig);
                        result = rows == 1 ? 1 : 0; 
                    }
                });
            }

            if (result == 1)
            {
                var log = new
                {
                    ObjectId = groupConfig.DianpingGroupId,
                    ObjectType = "InsertGroupConfig",
                    BeforeValue = "",
                    AfterValue = JsonConvert.SerializeObject(groupConfig),
                    Remark = "",
                    Creator = user
                };
                LoggerManager.InsertLog("CommonConfigLog", log);
            }

            return result;
        }

        public bool UpsertGroupConfig(DianpingGroupConfig groupConfig)
        {
            bool result = false;

            if (groupConfig != null)
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    int rows = 0;
                    var current = dalGroupConfig.SelectDianpingGroupConfig(conn, groupConfig.DianpingGroupId);
                    if (current != null)
                    {
                        rows = dalGroupConfig.Update(conn, groupConfig);
                    }
                    else
                    {
                        rows = dalGroupConfig.Insert(conn, groupConfig);
                    }
                    result = rows > 0;
                });
            }

            return result;
        }

        public bool DeleteGroupConfig(string dianpingId, string user)
        {
            bool result = dbScopeManager.Execute(conn => dalGroupConfig.Delete(conn, dianpingId)) == 1;

            var log = new
            {
                ObjectId = dianpingId,
                ObjectType = "DeleteGroupConfig",
                BeforeValue = "",
                AfterValue = "",
                Remark = "",
                Creator = user
            };
            LoggerManager.InsertLog("CommonConfigLog", log);

            return result;
        }

        #endregion

        #region Shop Config

        public List<DianpingShopConfig> GetShopConfigs(int pageIndex, int pageSize,
            string dianpingId, string dianpingName, string dianpingShopName, string tuhuShopId, 
            int groupStatus, int linkStatus)
        {
            var shops = dbScopeReadManager.Execute(conn => dalShopConfig.SelectShopConfigs(conn, pageIndex, pageSize,
             dianpingId, dianpingName, dianpingShopName, tuhuShopId, groupStatus, linkStatus));

            if (shops != null && shops.Any())
            {
                var ids = shops.Select(o => o.TuhuShopId).Distinct().ToList();

                Dictionary<int, ShopModel> shopdata = new Dictionary<int, ShopModel>();
                foreach (var id in ids)
                {
                    var shop = GetTuhuShop(id);
                    if (shop != null)
                    {
                        shopdata[id] = shop;
                    }
                }

                var sessions = dbScopeReadManager.Execute(conn => dalShopConfig.SelectShopSessions(conn, ids));
                foreach (var shop in shops)
                {
                    if (shopdata.ContainsKey(shop.TuhuShopId))
                    {
                        shop.TuhuShopName = shopdata[shop.TuhuShopId].CarparName;
                        shop.TuhuShopAddress = shopdata[shop.TuhuShopId].Address;
                    }
                    shop.Session = sessions.FirstOrDefault(o => o.TuhuShopId == shop.TuhuShopId);
                }
            }

            return shops;
        }

        public int GetShopConfigsCount(string dianpingId, string dianpingName, 
            string dianpingShopName, string tuhuShopId, int groupStatus, int linkStatus)
        {
            return dbScopeReadManager.Execute(conn => dalShopConfig.SelectShopConfigsCount(conn,
                dianpingId, dianpingName, dianpingShopName, tuhuShopId, groupStatus, linkStatus));
        }

        public string GetTuhuShopName(int tuhuShopId)
        {
            string result = string.Empty;
            if (tuhuShopId > 0)
            {
                var shop = GetTuhuShop(tuhuShopId);
                if (shop != null)
                {
                    result = shop.CarparName;
                }
            }

            return result;
        }

        public ShopModel GetTuhuShop(int tuhuShopId)
        {
            ShopModel result = null;
            if (tuhuShopId > 0)
            {
                using (var client = new ShopClient())
                {
                    var serviceResult = client.FetchShop(tuhuShopId);
                    if (serviceResult.Success && serviceResult.Result != null)
                    {
                        result = serviceResult.Result;
                    }
                }
            }

            return result;
        }

        public int UpdateShopConfig(DianpingShopConfig shopConfig, string user)
        {
            int result = 0;

            if (shopConfig != null)
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    int rows = 0;
                    var configs = dalShopConfig.SelectDianpingShopConfig(conn, shopConfig.DianpingId, shopConfig.TuhuShopId);
                    if (configs != null && configs.Count() == 1)
                    {
                        rows = dalShopConfig.Update(conn, shopConfig);
                        result = rows == 1 ? 1 : 0;
                        if (result == 1)
                        {
                            var log = new
                            {
                                ObjectId = shopConfig.DianpingId,
                                ObjectType = "UpdateShopConfig",
                                BeforeValue = JsonConvert.SerializeObject(configs.First()),
                                AfterValue = JsonConvert.SerializeObject(shopConfig),
                                Remark = "",
                                Creator = user
                            };
                            LoggerManager.InsertLog("CommonConfigLog", log);
                        }
                    }
                    else
                    {
                        result = -1;
                    }
                });
            }

            return result;
        }

        public Tuple<string, int> BulkInsertShopConfig(List<DianpingShopConfig> configs, string user)
        {
            Tuple<string, int> result = null;

            List<dynamic> logs = new List<dynamic>();
            dbScopeManager.Execute(conn =>
            {
                var trans = conn.BeginTransaction();
                try
                {
                    foreach (var config in configs)
                    {
                        int perResult = 0;
                        try
                        {
                            var current = dalShopConfig.SelectDianpingShopConfig(conn, config.DianpingId, config.TuhuShopId, trans);
                            if (current != null && current.Any())
                            {
                                perResult = -1;
                            }
                            else
                            {
                                dalShopConfig.Insert(conn, config, trans);
                                perResult = 1;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                            throw ex;
                        }
                        finally
                        {
                            if (perResult != 1)
                            {
                                result = Tuple.Create(config.DianpingId, perResult);
                            }
                        }
                    }

                    if (result == null)
                    {
                        trans.Commit();
                    }
                    else
                    {
                        trans.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
            });

            if (result != null)
            {
                foreach (var config in configs)
                {
                    var log = new
                    {
                        ObjectId = config.DianpingId,
                        ObjectType = "BulkInsertShopConfig",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(config),
                        Remark = "",
                        Creator = user
                    };
                    LoggerManager.InsertLog("CommonConfigLog", log);
                }
            }

            return result;
        }

        public int InsertShopConfig(DianpingShopConfig shopConfig, string user)
        {
            int result = 0;

            if (shopConfig != null)
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    int rows = 0;
                    var current = dalShopConfig.SelectDianpingShopConfig(conn, 
                        shopConfig.DianpingId, shopConfig.TuhuShopId);
                    if (current != null && current.Any())
                    {
                        result = -1;
                    }
                    else
                    {
                        rows = dalShopConfig.Insert(conn, shopConfig);
                        result = rows == 1 ? 1 : 0;
                    }
                });
            }

            if (result == 1)
            {
                var log = new
                {
                    ObjectId = shopConfig.DianpingId,
                    ObjectType = "InsertShopConfig",
                    BeforeValue = "",
                    AfterValue = JsonConvert.SerializeObject(shopConfig),
                    Remark = "",
                    Creator = user
                };
                LoggerManager.InsertLog("CommonConfigLog", log);
            }

            return result;
        }

        public bool DeleteShopConfig(string dianpingId, string user)
        {
            bool result = dbScopeManager.Execute(conn => dalShopConfig.Delete(conn, dianpingId)) == 1;

            var log = new
            {
                ObjectId = dianpingId,
                ObjectType = "DeleteShopConfig",
                BeforeValue = "",
                AfterValue = "",
                Remark = "",
                Creator = user
            };
            LoggerManager.InsertLog("CommonConfigLog", log);

            return result;
        }

        #endregion
    }
}
