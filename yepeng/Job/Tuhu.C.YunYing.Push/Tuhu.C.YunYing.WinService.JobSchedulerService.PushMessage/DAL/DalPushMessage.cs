using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Model;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.DAL
{
    public static class DalPushMessage
    {
        public static IEnumerable<MessageModel> SelectPushMessage(string channel, DateTime now, bool isSmallLot)
        {
            IEnumerable<MessageModel> result = null;
            using (var cmd = new SqlCommand(@"Gungnir.dbo.Job_SelectPushMessage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;// 设置超时时间为5分钟
                #region AddParameters
                cmd.Parameters.AddWithValue("@Channel", channel);
                cmd.Parameters.AddWithValue("@Now", now);
                cmd.Parameters.AddWithValue("@IsSmallLot", isSmallLot);
                #endregion
                result = DbHelper.ExecuteSelect<MessageModel>(true, cmd);
            }
            return result;
        }

        public static IEnumerable<MessageModel> SelectPushMessageForUnregister(DateTime now)
        {
            IEnumerable<MessageModel> result = null;
            using (var cmd = new SqlCommand(@"Gungnir.dbo.Job_SelectPushMessageForUnregister"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300; // 设置超时时间为5分钟
                #region AddParameters
                cmd.Parameters.AddWithValue("@Now", now);
                #endregion
                result = DbHelper.ExecuteSelect<MessageModel>(true, cmd);
            }
            return result;
        }

        public static IEnumerable<MessageModel> SelectTaskMessage(DateTime now, string taskType)
        {
            IEnumerable<MessageModel> result = null;
            using (var cmd = new SqlCommand(@"Gungnir.dbo.Job_SelectTaskMessage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                #region AddParameters
                cmd.Parameters.AddWithValue("@Now", now);
                #endregion
                result = DbHelper.ExecuteSelect<MessageModel>(true, cmd);
            }
            return result;
        }

        public static IEnumerable<MessageModel> SelectPushMessageForPromotion()
        {
            IEnumerable<MessageModel> result = null;
            string Sql = "SELECT TOP 10000 PromotionSingleTaskUsersHistoryId AS Id,UserCellPhone AS PhoneNumber FROM [Gungnir].[dbo].tbl_PromotionSingleTaskUsersHistory(NOLOCK) WHERE SendState=0";
            using (var cmd = new SqlCommand(Sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 300;// 设置超时时间为5分钟
                result = DbHelper.ExecuteSelect<MessageModel>(true, cmd);
            }
            return result;
        }

        public static int UpdateMessageModel(MessageModel message)
        {
            int count = 0;
            if (message != null)
            {
                using (var cmd = new SqlCommand("Gungnir.dbo.UpdatePushMessage"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    #region AddParameters

                    cmd.Parameters.AddWithValue("@Id", message.Id);
                    cmd.Parameters.AddWithValue("@Status", message.Status);
                    cmd.Parameters.AddWithValue("@ActualSendTime", message.ActualSendTime);
                    cmd.Parameters.AddWithValue("@Note", message.Note);
                    cmd.Parameters.AddWithValue("@UMMessageId", message.UMMessageId);
                    cmd.Parameters.AddWithValue("@TuhuId", message.TuhuId);

                    #endregion

                    count = DbHelper.ExecuteNonQuery(cmd);
                }
            }

            return count;
        }

        public static int UpdateTaskMessageModel(MessageModel message)
        {
            int count = 0;
            if (message != null)
            {
                using (var cmd = new SqlCommand("Gungnir.dbo.Job_UpdatePushTaskMessage"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    #region AddParameters

                    cmd.Parameters.AddWithValue("@Id", message.Id);
                    cmd.Parameters.AddWithValue("@Status", message.Status);
                    cmd.Parameters.AddWithValue("@ActualSendTime", message.ActualSendTime);
                    cmd.Parameters.AddWithValue("@Note", message.Note);
                    cmd.Parameters.AddWithValue("@UMTaskId", message.UMTaskId);
                    cmd.Parameters.AddWithValue("@TuhuId", message.TuhuId);

                    #endregion

                    count = DbHelper.ExecuteNonQuery(cmd);
                }
            }

            return count;
        }
        /// <summary>
        /// 更新消息推送状态为sending
        /// </summary>
        /// <param name="messages"></param>
        public static void UpdateSendingStatusPush(IList<MessageModel> messages)
        {
            if (messages == null || messages.Count == 0) return;
            using (var cmd = new SqlCommand("Gungnir.dbo.UpdatePushMessage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                #region AddParameters
                foreach (var oneMessage in messages)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Id", oneMessage.Id);
                    cmd.Parameters.AddWithValue("@Status", "Sending");
                    cmd.Parameters.AddWithValue("@ActualSendTime", oneMessage.ActualSendTime);
                    cmd.Parameters.AddWithValue("@Note", oneMessage.Note);
                    cmd.Parameters.AddWithValue("@TuhuId", oneMessage.TuhuId);
                    cmd.Parameters.AddWithValue("@UMMessageId", oneMessage.UMMessageId);

                    DbHelper.ExecuteNonQuery(cmd);
                }
                #endregion
            }
        }
        /// <summary>
        /// 更新消息推送状态为sending
        /// </summary>
        /// <param name="messages"></param>
        public static void UpdateSendingStatusTask(IList<MessageModel> messages)
        {
            if (messages == null || messages.Count == 0) return;
            using (var cmd = new SqlCommand("Gungnir.dbo.Job_UpdatePushTaskMessage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                #region AddParameters
                foreach (var oneMessage in messages)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Id", oneMessage.Id);
                    cmd.Parameters.AddWithValue("@Status", "Sending");
                    cmd.Parameters.AddWithValue("@ActualSendTime", oneMessage.ActualSendTime);
                    cmd.Parameters.AddWithValue("@Note", oneMessage.Note);
                    cmd.Parameters.AddWithValue("@TuhuId", oneMessage.TuhuId);
                    cmd.Parameters.AddWithValue("@UMTaskId", oneMessage.UMTaskId);

                    DbHelper.ExecuteNonQuery(cmd);
                }
                #endregion
            }
        }
        /// <summary>
        /// 更新推送的消息状态为1→已发送,2→发送中,3→发送异常
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static void UpdateSendStatePromotion(IList<MessageModel> messages, int state)
        {
            if (messages == null || messages.Count == 0) return;
            //所有数据的id
            string ids = string.Join(",", messages.Select(u => u.Id));
            string script = string.Format("UPDATE [Gungnir].[dbo].tbl_PromotionSingleTaskUsersHistory With(ROWLOCK) SET SendState={0} WHERE PromotionSingleTaskUsersHistoryId IN ({1})", state, ids);
            using (var cmd = new SqlCommand(script))
            {
                cmd.CommandType = CommandType.Text;
                DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static IEnumerable<ZeroActivityInfoModel> SelectUnnotifiedZeroActivityInfo()
        {
            var script = @" SELECT TZA.Period, 
                                   TZA.StartDateTime,
                                   VWP.PID,
                                   VWP.DisplayName 
                            FROM Activity..tbl_ZeroActivity AS TZA WITH(NOLOCK) 
                            LEFT JOIN Tuhu_productcatalog..vw_Products AS VWP WITH(NOLOCK) ON TZA.ProductID + '|' + TZA.VariantID COLLATE Chinese_PRC_CI_AS = VWP.PID 
                            WHERE TZA.HasReminderBeenSent=0 AND GETDATE() >= TZA.StartDateTime 
                            ORDER BY Period";
            using (var cmd = new SqlCommand(script))
            {
                cmd.CommandType = CommandType.Text;
                return DbHelper.ExecuteSelect<ZeroActivityInfoModel>(true, cmd);
            }
        }

        public static Dictionary<int, int> SelectSumsOfApplies(IList<int> periods)
        {
            var script = @" SELECT Period, Count(1) AS SumOfApplies FROM Tuhu_log.dbo.ZeroActivityReminder AS TZA WITH(NOLOCK) JOIN Tuhu_log..SplitString(@Periods, ',', 1) AS PS ON TZA.Period = PS.Item COLLATE Chinese_PRC_CI_AS GROUP BY TZA.Period";

            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(script))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Periods", string.Join(",", periods));
                    return dbHelper.ExecuteQuery(cmd, (dt) =>
                    {
                        Dictionary<int, int> sumsOfApplies = new Dictionary<int, int>();
                        var drs = dt?.Rows?.OfType<DataRow>();
                        if (drs != null && drs.Any())
                        {
                            foreach (var dr in drs)
                            {
                                int period, numOfApplies;
                                if (int.TryParse(dr["Period"]?.ToString(), out period) && int.TryParse(dr["SumOfApplies"]?.ToString(), out numOfApplies))
                                    sumsOfApplies.Add(period, numOfApplies);
                            }
                        }
                        return sumsOfApplies;
                    });
                }
            }
        }

        public static IEnumerable<IEnumerable<string>> SelectUserIdsToNotify(int period, int numofPatchs)
        {
            var result = new List<IEnumerable<string>>();

            var script = string.Format("SELECT UserID FROM Tuhu_log.dbo.ZeroActivityReminder WITH(nolock) WHERE Period={0} ORDER BY CreateTime OFFSET @PageNumber * 100 ROW FETCH NEXT 100 ROW ONLY", period);
            for (int i = 0; i < numofPatchs; ++i)
            {
                using (var dbHelper = DbHelper.CreateLogDbHelper(true))
                {
                    using (var cmd = new SqlCommand(script))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@PageNumber", i);
                        result.Add(dbHelper.ExecuteQuery(cmd, (dt) =>
                        {
                            var userIds = new List<string>();
                            var drs = dt?.Rows?.OfType<DataRow>();
                            if (drs != null && drs.Any())
                            {
                                foreach (var dr in drs)
                                {
                                    var userId = dr["UserID"]?.ToString();
                                    if (!string.IsNullOrWhiteSpace(userId))
                                        userIds.Add(userId);
                                }
                            }
                            return userIds;
                        }));
                    }
                }
            }
            return result;
        }

        public static IEnumerable<string> SelectPhoneNumbersToNotify(IEnumerable<Guid> userIds)
        {
            var script = "SELECT u_mobile_number AS PhoneNumbers FROM Tuhu_profiles..UserObject AS UO WITH(nolock) JOIN Tuhu_profiles..SplitString(@UserIDs, ',', 1) AS PS ON UO.u_user_id = PS.Item COLLATE Chinese_PRC_CI_AS";
            using (var cmd = new SqlCommand(script))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@UserIDs",string.Join(",", userIds.Select(_=>_.ToString("B")).ToList()));
                return DbHelper.ExecuteQuery(cmd, (dt) =>
                {
                    IList<string> phoneNums = new List<string>();
                    var drs = dt?.Rows?.OfType<DataRow>();
                    if (drs != null && drs.Any())
                    {
                        foreach (var dr in drs)
                        {
                            var phoneNum = dr["PhoneNumbers"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(phoneNum))
                                phoneNums.Add(phoneNum);
                        }
                    }
                    return phoneNums;
                });
            }
        }

        public static bool UpdateReminderTransmissionState(int period)
        {
            var script = @" UPDATE Activity..tbl_ZeroActivity WITH(ROWLOCK) SET HasReminderBeenSent=1 WHERE Period=@Period";
            using (var cmd = new SqlCommand(script))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Period", period);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }
    }
}
