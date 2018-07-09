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
using Tuhu.Provisioning.DataAccess.Entity.BaoYang;
using Tuhu.Service.BaoYang;
using Tuhu.Service.BaoYang.Config;

namespace Tuhu.Provisioning.Business.BaoYang
{
    public class InstallTypeConfigManager
    {
        private readonly IConnectionManager connectionManager;
        private readonly IConnectionManager connectionManagerRead;
        private readonly IDBScopeManager dbScopeManager;
        private readonly IDBScopeManager dbScopeManagerRead;

        private readonly DalBaoYangInstallTypeConfig dalInstallType;

        private readonly Common.Logging.ILog logger;

        public InstallTypeConfigManager()
        {
            this.connectionManager =
                new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
            this.dbScopeManager = new DBScopeManager(this.connectionManager);

            this.connectionManagerRead =
                new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
            this.dbScopeManagerRead = new DBScopeManager(this.connectionManager);

            this.dalInstallType = new DalBaoYangInstallTypeConfig();


            this.logger = LogManager.GetLogger(typeof(InstallTypeConfigManager));
        }

        public List<InstallTypeConfig> GetInstallTypeConfigs()
        {
            var result = dbScopeManagerRead.Execute(conn => dalInstallType.SelectInstallTypeConfig(conn));

            if (result != null && result.Any())
            {
                IEnumerable<BaoYangPackageDescription> packages = null;
                using (var client = new BaoYangClient())
                {
                    var sericeResult = client.GetBaoYangPackageDescription();
                    if (sericeResult.Success && sericeResult.Result != null)
                    {
                        packages = sericeResult.Result;
                    }
                }

                if (packages != null)
                {
                    foreach (var config in result)
                    {
                        if (packages.Any(o => string.Equals(o.PackageType, config.PackageType)))
                        {
                            var package = packages.First(o => string.Equals(o.PackageType, config.PackageType));
                            config.PackageName = package.ZhName;
                            config.InstallTypes = config.InstallTypes.Where(o => !string.IsNullOrEmpty(o.Name)).ToList();
                            foreach (var installType in config.InstallTypes)
                            {
                                foreach (var type in installType.BaoYangTypeList)
                                {
                                    if (installType.BaoYangTypeNameList == null)
                                    {
                                        installType.BaoYangTypeNameList = new List<string>();
                                    }

                                    if (package.Items.Any(o => string.Equals(o.BaoYangType, type)))
                                    {
                                        var item = package.Items.First(o => string.Equals(o.BaoYangType, type));

                                        installType.BaoYangTypeNameList.Add(item.ZhName);
                                    }
                                }

                                installType.BaoYangTypeNames = string.Join(",", installType.BaoYangTypeNameList);

                                if (!string.IsNullOrEmpty(installType.TextFormat))
                                {
                                    installType.TextFormats = JsonConvert.DeserializeObject<List<InstallTypeText>>(installType.TextFormat);
                                }
                            }
                        }
                    }
                }
            }

            return result.Where(o => o.InstallTypes.Count > 1).ToList();
        }

        public bool UpdateInstallTypeConfig(InstallTypeConfig config, string user)
        {
            bool result = false;

            if (config != null)
            {
                try
                {
                    string packageType = config.PackageType;
                    string imageUrl = config.ImageUrl;

                    var prevData = GetInstallTypeConfigs();

                    dbScopeManager.CreateTransaction(conn =>
                    {
                        dalInstallType.UpdateImage(conn, packageType, imageUrl);
                        foreach (var installType in config.InstallTypes)
                        {
                            installType.TextFormat = JsonConvert.SerializeObject(installType.TextFormats);
                            dalInstallType.Update(conn, packageType, installType.Type, installType.IsDefault, installType.TextFormat);
                        }

                        result = true;
                    });

                    if (result)
                    {
                        var resultData = GetInstallTypeConfigs();
                        var log = new
                        {
                            ObjectId = packageType,
                            ObjectType = "UpdateBaoYangInstallType",
                            BeforeValue = JsonConvert.SerializeObject(prevData.FirstOrDefault(o => string.Equals(o.PackageType, packageType))),
                            AfterValue = JsonConvert.SerializeObject(resultData.FirstOrDefault(o => string.Equals(o.PackageType, packageType))),
                            Remark = "",
                            Creator = user
                        };
                        LoggerManager.InsertLog("CommonConfigLog", log);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }

            return result;
        }

        public List<InstallTypeVehicleConfig> GetInstallTypeVehicleConfig(string packageType, string installType,
            string brand, string series, string vehicleId, string categories, int minPrice, int maxPrice, string brands,
            bool isConfig, int pageIndex, int pageSize)
        {
            List<InstallTypeVehicleConfig> result = new List<InstallTypeVehicleConfig>();

            result = dbScopeManagerRead.Execute(conn => dalInstallType.SelectVehicleConfigs(conn, packageType, installType,
                 brand, series, vehicleId, categories, minPrice, maxPrice, brands,
                 isConfig, pageIndex, pageSize));

            return result;
        }

        public int GetInstallTypeVehicleConfigCount(string packageType, string installType,
            string brand, string series, string vehicleId, string categories, int minPrice, int maxPrice, string brands,
            bool isConfig)
        {
            return dbScopeManagerRead.Execute(conn => dalInstallType.SelectVehicleConfigsCount(conn, packageType, installType,
                 brand, series, vehicleId, categories, minPrice, maxPrice, brands, isConfig));
        }

        public bool DeleteInstallTypeVehicleConfig(string packageType, string installType,
            string vehicleIds, string user)
        {
            bool result = false;

            try
            {
                if (!string.IsNullOrEmpty(packageType) && !string.IsNullOrEmpty(installType) &&
                    !string.IsNullOrEmpty(vehicleIds))
                {
                    dbScopeManager.CreateTransaction(conn =>
                    {
                        var list = vehicleIds.Split(',').ToList();

                        foreach (var vehicleId in list)
                        {
                            dalInstallType.DeleteVehicleConfig(conn, packageType,
                                installType, vehicleId);
                        }
                        result = true;
                    });

                    if (result)
                    {
                        var resultData = GetInstallTypeConfigs();
                        var log = new
                        {
                            ObjectId = installType,
                            ObjectType = "DeleteInstallTypeVehicle",
                            BeforeValue = "",
                            AfterValue = vehicleIds,
                            Remark = "",
                            Creator = user
                        };
                        LoggerManager.InsertLog("CommonConfigLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public bool AddInstallTypeVehicleConfig(string packageType, string installType,
            string vehicleIds, string user)
        {
            bool result = false;

            try
            {
                if (!string.IsNullOrEmpty(packageType) && !string.IsNullOrEmpty(installType) &&
                    !string.IsNullOrEmpty(vehicleIds))
                {
                    dbScopeManager.CreateTransaction(conn =>
                    {
                        var list = vehicleIds.Split(',').ToList();

                        foreach (var vehicleId in list)
                        {
                            var exists = dalInstallType.SelectVehicleConfig(conn, packageType, installType, vehicleId);
                            if (!exists)
                            {
                                dalInstallType.InsertVehicleConfig(conn, packageType, installType, vehicleId);
                            }
                        }

                        result = true;
                    });

                    if (result)
                    {
                        var resultData = GetInstallTypeConfigs();
                        var log = new
                        {
                            ObjectId = installType,
                            ObjectType = "AddInstallTypeVehicle",
                            BeforeValue = "",
                            AfterValue = vehicleIds,
                            Remark = "",
                            Creator = user
                        };
                        LoggerManager.InsertLog("CommonConfigLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }
    }
}
