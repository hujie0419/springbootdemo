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

        public static List<FlashSaleProductModel> SelectAllSaleActivity()
        {
            using (var cmd = new SqlCommand(@"
SELECT  vvfs.PID 
FROM    Activity.dbo.vw_ValidFlashSale vvfs WITH ( NOLOCK )
WHERE   vvfs.ActiveType = 0
        AND vvfs.IsNewUserFirstOrder = 0
        AND vvfs.StartDateTime <= GETDATE()
        AND vvfs.EndDateTime >= GETDATE()
        AND ActivityID NOT IN (
                N'3464d2b9-cb9f-4f7d-bbb0-196bd8436197',
                N'a03e8ce5-728c-40cc-8598-2659c9ac015f',
                N'590c63bf-2348-4c72-9810-3495d0847dac',
                N'1a448a4a-a44d-4d27-8493-a6286a0de5ca',
                N'b92954b2-6cd4-4388-b735-c57f24749ada',
                N'69792836-cea3-49f3-8433-d00b0516c424',
                N'03c0fc0f-a30f-4930-9704-dea5e4f2e38e',
                N'5547058f-6a1d-465b-a58f-e5121aaf717a',
                N'138746f6-101c-4d72-a8ba-ead201a9d30a',
                N'13f19102-998c-4440-8f1e-f418527926f1',
                N'b93dd5bd-8d89-442a-8d64-16482e2de06a',
                N'95ce2cfb-d4c4-4851-b3a3-68ee61f5d596',
                N'eb5a3a03-4dbc-4079-981d-e025d25d45eb',
                N'2cb2c918-2616-4f79-a743-c41815e1cfcc',
                N'cdb8a5be-2756-427b-8ef7-2e8701f12a3e',
                N'ac9a66e8-6fdf-4a96-a157-c5e166297d52',
                N'8312fe9d-6dce-4cd5-87d5-99ce064336a3',
                N'650c6d05-51f8-46fb-a84f-f136d727a59b',
                N'663543d4-5b62-4133-935d-f0a4c68751f8',
                N'88c467db-622e-4f02-89a5-f80405076599'
        );"))
            {
                cmd.CommandType = CommandType.Text;
                return DbHelper.ExecuteSelect<FlashSaleProductModel>(cmd).ToList();
            }
        }
    }
}
