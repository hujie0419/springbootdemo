using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Configuration;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalMobileBindUnbind
    {
        public static List<OrderCompletionJudgeCriteria> SelectOrderCompJudgeCriteria(string userId)
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            const string sql = "SELECT InstallShopId, Status, TBO.DeliveryStatus, TBO.DeliveryDatetime, InstallDatetime, TBOD.DeliveryStatus AS DeliveryStatusInLog, TBOD.DeliveryDatetime AS DeliveryDatetimeInLog FROM Gungnir..tbl_Order AS TBO WITH(NOLOCK) LEFT JOIN Gungnir.dbo.tbl_OrderDeliveryLog AS TBOD ON TBO.PKID = TBOD.OrderID WHERE TBO.UserID=@UserID";
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@UserID", userId);
                return dbHelper.ExecuteDataTable(cmd).ConvertTo<OrderCompletionJudgeCriteria>().ToList();
            }
        }
    }
}
