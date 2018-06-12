using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu;

namespace BaoYangRefreshCacheService.DAL
{
    public static class DalBaoYangPrice
    {
        public static int GetBIAdaptationDataCount()
        {
            using (var cmd = new SqlCommand(@"SELECT count(1)
FROM tuhu_bi..BaoYangAdaptationData WITH(NOLOCK)"))
            {
                cmd.CommandType = CommandType.Text;
                return (int) DbHelper.CreateDbHelper("Tuhu_BI").ExecuteScalar(cmd);
            }
        }

        public static int CleanBIAdaptationData() {
            using (var cmd = new SqlCommand("TRUNCATE TABLE Tuhu_BI..BaoYangAdaptationData"))
            {
                cmd.CommandType = CommandType.Text;
                return DbHelper.CreateDbHelper("Tuhu_BI").ExecuteNonQuery(cmd);
            }
        }

        public static int CleanBaoYangPackageProductPriceTable()
        {
            using (var cmd = new SqlCommand("TRUNCATE TABLE BaoYang..BaoYangPackageProductPrice"))
            {
                cmd.CommandType = CommandType.Text;
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static int CleanBaoYangPackageServicePriceTable()
        {
            using (var cmd = new SqlCommand("TRUNCATE TABLE BaoYang..BaoYangPackageServicePrice"))
            {
                cmd.CommandType = CommandType.Text;
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static void Save(string tableName, DataTable dt, string connection = "BaoYang")
        {
            try
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings[connection].ConnectionString))
                {
                    bulkCopy.BulkCopyTimeout = 10;
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.WriteToServer(dt);
                }
            }catch(Exception ex)
            {

            }
        }
    }
}
