using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalMGMTools
    {
        public static bool CheckExistPinTuanOrder(int orderId)
        {
            var SqlStr = @"SELECT	COUNT(1)
FROM	Activity..tbl_GroupBuyingUserInfo WITH ( NOLOCK )
WHERE	OrderId = @orderid;";
            using (var cmd = new SqlCommand(SqlStr))
            {
                cmd.Parameters.AddWithValue("@orderid", orderId);
                var result = DbHelper.ExecuteScalar(cmd);
                if (Int32.TryParse(result?.ToString(), out int value))
                {
                    return value > 0;
                }
                return false;
            }
        }
        public static GroupBuyingUserInfo FetchUserInfoByOrderId(int orderId)
        {
            var SqlStr = @"SELECT	IIF(T.OwnerId = S.UserId, 1, 0) AS Code ,
		S.GroupId ,
		S.OrderId ,
		S.UserStatus ,
		S.UserId
FROM	Activity..tbl_GroupBuyingUserInfo AS S WITH ( NOLOCK )
		LEFT JOIN Activity..tbl_GroupBuyingInfo AS T WITH ( NOLOCK ) ON S.GroupId = T.GroupId
WHERE	S.OrderId = @orderId;";
            using (var cmd = new SqlCommand(SqlStr))
            {
                cmd.Parameters.AddWithValue("@orderid", orderId);
                var result = DbHelper.ExecuteDataTable(cmd).ConvertTo<GroupBuyingUserInfo>();
                return result.FirstOrDefault() ?? new GroupBuyingUserInfo();
            }
        }
    }
    public class GroupBuyingUserInfo
    {
        public int Code { get; set; }
        public int UserStatus { get; set; }
        public int OrderId { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
    }
}