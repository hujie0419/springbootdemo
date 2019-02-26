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
using Common.Logging;

namespace Tuhu.Provisioning.Business
{
    public class VehicleTypeManager
    {
        VehicleTypeHandler handler = null;
        private readonly IConnectionManager connectionManager = null;
        private readonly IDBScopeManager dbScopeManagerGR = null;

        private static readonly Common.Logging.ILog logger = LogManager.GetLogger("VehicleTypeManager");

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
                logger.Error("GetAllVehicleBrands", ex);
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
                logger.Error("GetVehicleSeries", ex);
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
                logger.Error("GetAllBrandCategories", ex);
            }

            return result;
        }

        /// <summary>
        /// 获取排量 
        /// </summary>
        /// <param name="vid"></param>
        /// <returns></returns>
        public IEnumerable<string> GetVehiclePaiLiang(string vid)
        {
            IEnumerable<string> result = null;

            try
            {
                result = dbScopeManagerGR.Execute(conn => DalVehicleType.GetVehiclePaiLiang(conn, vid));
            }
            catch (Exception ex)
            {
                logger.Error("GetVehiclePaiLiang", ex);
            }

            return result;
        }

        /// <summary>
        /// 获取生产年份
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="paiLiang"></param>
        /// <returns></returns>
        public List<string> GetVehicleNian(string vid, string paiLiang)
        {
            List<string> result = null;
            try
            {
                var minAndMax = dbScopeManagerGR.Execute(conn => DalVehicleType.GetVehicleNian(conn, vid, paiLiang));
                if (minAndMax != null)
                {
                    var min = minAndMax.Item1;
                    var max = minAndMax.Item2;
                    if (min != 0 && max != 0)
                    {
                        result = new List<string>();

                        for (int i = min; i <= max; i++)
                        {
                            result.Add(i.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("GetVehicleNian", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取年款信息
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="paiLiang"></param>
        /// <param name="nian"></param>
        /// <returns></returns>
        public IDictionary<string, string> GetVehicleSalesName(string vid, string paiLiang, int nian)
        {
            IDictionary<string, string> result = null;

            try
            {
                result = dbScopeManagerGR.Execute(conn => DalVehicleType.GetVehicleSalesName(conn, vid, paiLiang, nian));
            }
            catch (Exception ex)
            {
                logger.Error("GetVehicleSalesName", ex);
            }

            return result;
        }

        /// <summary>
        /// 获取Tid
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="paiLiang"></param>
        /// <param name="nian"></param>
        /// <returns></returns>
        public IEnumerable<string> GetTids(string vid, string paiLiang, string nian)
        {
            IEnumerable<string> result = null;
            try
            {
                result = dbScopeManagerGR.Execute(conn => DalVehicleType.GetTids(conn, vid, paiLiang, nian));
            }
            catch (Exception ex)
            {
                logger.Error("GetTids", ex);
            }
            return result;
        }
    }
}
