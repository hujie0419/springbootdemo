using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.Activity.Models;

namespace Tuhu.C.Job.Activity.Dal
{
    public class DalFlashSale
    {
        public static IEnumerable<FlashSaleProductModel> SelectFlashSaleProductModel()
        {
            using (var cmd = new SqlCommand(@"SELECT  VFS.ActivityID ,VFS.PID FROM    Activity..vw_ValidFlashSale VFS WITH ( NOLOCK )"))
            {
                cmd.CommandType = CommandType.Text;
                return DbHelper.ExecuteSelect<FlashSaleProductModel>(true, cmd);
            }
        }

        public static int UpdateFlashSaleProducts(FlashSaleProductModel item)
        {
            using (var cmd = new SqlCommand(@" UPDATE  Activity..tbl_FlashSaleProducts WITH ( ROWLOCK )
                                                SET     SaleOutQuantity = SaleOutQuantity + @Num
                                                WHERE   ActivityID = @ActivityID
                                                        AND PID = @PID;"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Num", item.Num);
                cmd.Parameters.AddWithValue("@ActivityID", item.ActivityId);
                cmd.Parameters.AddWithValue("@PID", item.Pid);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
    }
}
