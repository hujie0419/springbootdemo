using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Tuhu.Component.Framework;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business.ProductVehicleType;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class VehicleTypeManager 
    {
        VehicleTypeHandler handler = null;
        private readonly IConnectionManager connectionManager = null;
        private readonly IDBScopeManager dbScopeManagerGR = null;

        private static readonly ILog logger = LoggerFactory.GetLogger("VehicleTypeManager");

        public VehicleTypeManager()
        {
            this.connectionManager =
                new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_ReadOnly"].ConnectionString.Decrypt());
            this.dbScopeManagerGR = new DBScopeManager(this.connectionManager);
        }

        public List<string> GetAllVehicleBrands()
        {
            List<string> result = new List<string>();

            try
            {
                result = dbScopeManagerGR.Execute(connection => DalVehicleType.SelectAllVehicleBrands(connection));

                if (result != null)
                {
                    result = result.OrderBy(o => o).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.Message);
            }

            return result;
        }

        public IDictionary<string, string> GetVehicleSeries(string brand)
        {
            IDictionary<string, string> result = null;

            try
            {
                result = dbScopeManagerGR.Execute(connection => DalVehicleType.SelectVehicleSeries(connection, brand));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.Message);
            }

            return result;
        }

        public List<string> GetAllBrandCategories()
        {
            List<string> result = new List<string>();

            try
            {
                result = dbScopeManagerGR.Execute(connection => DalVehicleType.SelectAllBrandCategories(connection));

                if (result != null)
                {
                    result = result.OrderByDescending(o => o).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.Message);
            }

            return result;
        }
    }
}

    