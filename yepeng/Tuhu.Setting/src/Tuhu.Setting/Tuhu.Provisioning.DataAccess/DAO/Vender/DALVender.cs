using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.Vender;

namespace Tuhu.Provisioning.DataAccess.DAO.Vender
{
    public static class DALVender
    {
        public static List<VenderModel> SelectVenderInfoByVendorType(SqlConnection conn, string vendorType)
        {
            const string sql = @"select VenderID,VenderShortName ,VenderName from Gungnir..Vender WITH (NOLOCK) where VendorType=@VendorType";
            return conn.Query<VenderModel>(sql, new { VendorType = vendorType }, commandType: CommandType.Text).ToList();
        }
    }
}
