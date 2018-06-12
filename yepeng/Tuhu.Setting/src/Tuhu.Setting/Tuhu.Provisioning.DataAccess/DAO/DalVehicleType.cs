using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using Newtonsoft.Json;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common;

namespace Tuhu.Provisioning.DataAccess
{
    public static class DalVehicleType
    {
        private const string ProcSelectAllBrands = "VehicleType_SelectAllBrands";

        private const string ProcSelectVehicleSeriesByBrand = "VehicleType_SelectVehicleSeriesByBrand";

        public static List<string> SelectAllVehicleBrands(SqlConnection connection)
        {
            var result = new List<string>();

            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, ProcSelectAllBrands))
            {
                while (reader.Read())
                {
                    result.Add(reader.IsDBNull(0) ? string.Empty : reader.GetString(0));
                }
            }

            return result;
        }

        public static List<string> SelectAllBrandCategories(SqlConnection connection)
        {
            var result = new List<string>();

            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text,
                @"SELECT DISTINCT BrandCategory
                FROM Gungnir..tbl_Vehicle_Type WITH ( NOLOCK )
                WHERE BrandCategory IS NOT NULL"))
            {
                while (reader.Read())
                {
                    result.Add(reader.IsDBNull(0) ? string.Empty : reader.GetString(0));
                }
            }

            return result;
        }

        public static IDictionary<string, string> SelectVehicleSeries(SqlConnection connection, string brand)
        {
            var result = new Dictionary<string, string>();

            var parameters = new[]
            {
                new SqlParameter("@Brand", brand)
            };

            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, ProcSelectVehicleSeriesByBrand, parameters))
            {
                while (reader.Read())
                {

                    var key = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                    var value = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                    if (!result.ContainsKey(key))
                    {
                        result.Add(key, value);
                    }
                }
            }

            return result;
        }
    }
}
