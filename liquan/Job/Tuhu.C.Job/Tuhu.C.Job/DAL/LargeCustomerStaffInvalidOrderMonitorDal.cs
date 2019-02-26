using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.Models;

namespace Tuhu.C.Job.DAL
{
   public class LargeCustomerStaffInvalidOrderMonitorDal
    {

        public static LargeCustomerInvalidOrderModel GetLargeCustomerStaffInvalidOrder(Guid userId, string activityId,string invalidType)
        {
            const string sqlStr = @" Select   [UserId]
                                             ,[InvalidType]
                                             ,[ActivityId]
                                             ,[OrderIDs]
                                             ,[DetailInfo]
                                             ,[EmailSendCount]
                                      FROM   [Activity].[dbo].[LargeCustomerInvalidOrder] With (NOLOCK)
                                     WHERE   UserId=@UserId
                                             AND IsDelete=0
                                             AND ActivityId=@ActivityId
                                             AND InvalidType=@InvalidType ";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                cmd.Parameters.AddWithValue("@InvalidType", invalidType);
                return DbHelper.ExecuteFetch<LargeCustomerInvalidOrderModel>(cmd);
            }
        }

        /// <summary>
        /// 插入大客户员工异常订单记录 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertLargeCustomerStaffInvalidOrder(LargeCustomerInvalidOrderModel model)
        {
            const string sqlStr = @"INSERT INTO [Activity].[dbo].[LargeCustomerInvalidOrder]
                                                   ([UserId]
                                                   ,[InvalidType]
                                                   ,[ActivityId]
                                                   ,[OrderIDs]
                                                   ,[DetailInfo]
                                                   ,[EmailSendCount]
                                                   ,Phone
                                                   ,IsCouponDeleted
                                                   ,[CreateTime]
                                                   ,[UpdateTime]
                                                   ,[IsDelete])
                                             VALUES
                                                   (@UserId
                                                   ,@InvalidType
                                                   ,@ActivityId
                                                   ,@OrderIDs
                                                   ,@DetailInfo
                                                   ,@EmailSendCount
                                                   ,@Phone
                                                   ,@IsCouponDeleted
                                                   ,GETDATE()
                                                   ,null
                                                   ,0)";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@UserId", model.UserId);
                cmd.Parameters.AddWithValue("@InvalidType", model.InvalidType);
                cmd.Parameters.AddWithValue("@ActivityId", model.ActivityId);
                cmd.Parameters.AddWithValue("@OrderIDs", model.OrderIDs);
                cmd.Parameters.AddWithValue("@DetailInfo", model.DetailInfo);
                cmd.Parameters.AddWithValue("@EmailSendCount", model.EmailSendCount);
                cmd.Parameters.AddWithValue("@Phone", model.Phone);
                cmd.Parameters.AddWithValue("@IsCouponDeleted", model.IsCouponDeleted);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 更新 大客户员工异常订单记录 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int UpdateLargeCustomerStaffInvalidOrder(LargeCustomerInvalidOrderModel model)
        {
            const string sqlStr = @"UPDATE [Activity].[dbo].[LargeCustomerInvalidOrder]  WITH(ROWLOCK)
                                    SET    [OrderIDs] = @OrderIDs,
                                           [DetailInfo] = @DetailInfo,
                                           EmailSendCount = @EmailSendCount,
                                            Phone=@Phone,
                                            IsCouponDeleted=@IsCouponDeleted,
                                           [UpdateTime] = GETDATE()
                                    WHERE  [UserId] = @UserId
                                           AND InvalidType = @InvalidType
                                           AND ActivityId = @ActivityId
                                           AND IsDelete = 0;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@OrderIDs", model.OrderIDs);
                cmd.Parameters.AddWithValue("@DetailInfo", model.DetailInfo);
                cmd.Parameters.AddWithValue("@EmailSendCount", model.EmailSendCount);
                cmd.Parameters.AddWithValue("@UserId", model.UserId);
                cmd.Parameters.AddWithValue("@InvalidType", model.InvalidType);
                cmd.Parameters.AddWithValue("@ActivityId", model.ActivityId);
                cmd.Parameters.AddWithValue("@Phone", model.Phone);
                cmd.Parameters.AddWithValue("@IsCouponDeleted", model.IsCouponDeleted);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
    }
}
