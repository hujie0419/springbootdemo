using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.DAL
{
    public class dealgroup
    {
        public static List<tmpModel> GetOrderInfo()
        {
            const string sqlStr = @"select S.OrderID as OrderId,
       S.Remark,
       T.UserID as UserId,
       S.PID,
       T.PayStatus
from Gungnir..vw_tbl_Order as T with (nolock)
    left join Gungnir..vw_tbl_OrderList as S with (nolock)
        on T.PKID = S.OrderID
where T.Status = '0NewPingTuan'
      and T.OrderDatetime > '2018-03-29 18:20:00'
      and T.PKID not in (   select OrderId
                            from Activity..tbl_GroupBuyingUserInfo with (nolock)
                            where CreateDateTime > '2018-03-29 18:20:00' )
      and S.Remark is not null
      and T.Status <> '7Canceled';";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteSelect<tmpModel>(true, cmd).ToList();
            }
        }


        public static string GetProductGroupIdByActivityId(Guid ActivityId)
        {
            const string sqlStr = @"select ProductGroupId
from Configuration..GroupBuyingProductGroupConfig with (nolock)
where ActivityId = @activityId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@activityId", ActivityId);
                return DbHelper.ExecuteFetch<tmpModel>(true, cmd)?.ProductGroupId;
            }
        }
        public static Guid? GetGroupBuyingId(int orderId)
        {
            const string sqlStr = @"select GroupId
from Activity..tbl_GroupBuyingUserInfo with (nolock)
where OrderId = @orderId;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);
                return DbHelper.ExecuteFetch<tmpModel>(true, cmd)?.GroupId;
            }
        }
    }



    public class tmpModel
    {
        public int OrderId { get; set; }
        public string Remark { get; set; }
        public string PID { get; set; }
        public Guid UserId { get; set; }
        public Guid ActivityId { get; set; }
        public string PayStatus { get; set; }
        public string ProductGroupId { get; set; }
        public Guid GroupId { get; set; }
    }
}
