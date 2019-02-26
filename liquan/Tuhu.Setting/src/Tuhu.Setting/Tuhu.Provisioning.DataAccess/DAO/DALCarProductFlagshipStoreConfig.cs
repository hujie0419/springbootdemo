using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Common;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALCarProductFlagshipStoreConfig
    {
        private static AsyncDbHelper db = null;
        public DALCarProductFlagshipStoreConfig()
        {
            db = DbHelper.CreateDefaultDbHelper();
        }

        public DataTable GetDataTable()
        {
            string sql = "select * from [Configuration].[dbo].[CarProductFlagshipStoreConfig] with (nolock) ";
            return db.ExecuteDataTable(new SqlCommand(sql));
        }

        public DataTable GetBrand()
        {
            string sql = @"SELECT DISTINCT CP_Brand 
                            FROM Tuhu_productcatalog.dbo.vw_Products(NOLOCK)
                            WHERE CP_Brand IS NOT NULL
                                AND not exists
                                    (
                                        SELECT Brand 
                                          FROM [Configuration].[dbo].[CarProductFlagshipStoreConfig] (NOLOCK)
                                        where CP_Brand= Brand
                                    )";
            using (SqlCommand cmd = new SqlCommand(sql))
            {
                return db.ExecuteDataTable(cmd);
            }
        }

        ~DALCarProductFlagshipStoreConfig()
        {
            db.Dispose();
        }
    }
}
