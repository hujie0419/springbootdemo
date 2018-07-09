using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalProductCategory
    {
        private const string Proc_SelectAllProductCategories = "Product_SelectAllProductCategories";
        private const string Proc_SelectAllBatteryPID = "Product_SelectAllBatteryPID";

        public static List<SKUProductCategory> SelectAllProductCategories(SqlConnection conn)
        {
            List<SKUProductCategory> result = new List<SKUProductCategory>();

            using (
                var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure,
                    Proc_SelectAllProductCategories))
            {
                while (reader.Read())
                {
                    SKUProductCategory category = new SKUProductCategory();
                    category.Oid = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    category.ParentOid = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                    category.CategoryName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                    category.DisplayName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                    category.Description = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                    category.NodeNo = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                    category.DescendantProductCount = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                    result.Add(category);
                }
            }

            return result;
        }
        /// <summary>
        /// 获取所有蓄电池的PID
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<SKUPBatteryPID> SelectAllBatteryPID(SqlConnection conn)
        {
            List<SKUPBatteryPID> result = new List<SKUPBatteryPID>();

            using (var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure,
                    Proc_SelectAllBatteryPID))
            {
                while (reader.Read())
                {
                    SKUPBatteryPID catagory = new SKUPBatteryPID();
                    catagory.PID = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                    catagory.DisplayName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    result.Add(catagory);

                }
            }
            return result;
        }
    }
}
