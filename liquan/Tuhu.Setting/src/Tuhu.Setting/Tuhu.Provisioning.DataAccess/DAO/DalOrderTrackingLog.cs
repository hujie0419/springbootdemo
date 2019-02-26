using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalOrderTrackingLog
    {
        public static void Add(SqlConnection connection, OrderTrackingLogEntity tblOrderTrackingLog)
        {
            var parameters = new[]
            {
                 new SqlParameter("@OrderId", tblOrderTrackingLog.OrderId),
                 new SqlParameter("@OrderStatus", tblOrderTrackingLog.OrderStatus?? string.Empty),
                 new SqlParameter("@DeliveryStatus", tblOrderTrackingLog.DeliveryStatus?? string.Empty),
                 new SqlParameter("@LogisticTaskStatus", tblOrderTrackingLog.LogisticTaskStatus?? string.Empty),
                 new SqlParameter("@CreateTime", tblOrderTrackingLog.CreateTime),
                 new SqlParameter("@Description", tblOrderTrackingLog.Description?? string.Empty),
                 new SqlParameter("@IsOver", tblOrderTrackingLog.IsOver),
                 new SqlParameter("@InstallStatus", tblOrderTrackingLog.InstallStatus?? string.Empty)
            };

            SqlHelper.ExecuteNonQueryV2(connection, CommandType.StoredProcedure, "Order_InsertOrderTrackingLog", parameters);
        }

        public static OrderTrackingLogEntity GetOrderTrackingLog(SqlConnection connection, int pkId)
        {
            OrderTrackingLogEntity tblOrderTrackingLog = null;

            var parameters = new[]
            {
                new SqlParameter("@pkId", pkId)
            };

            using (var dataReader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "存储过程", parameters))
            {
                if (dataReader.Read())
                {
                    tblOrderTrackingLog = new OrderTrackingLogEntity
                    {
                        PkId = dataReader.GetTuhuValue<int>(0),
                        OrderId = dataReader.GetTuhuValue<int>(1),
                        OrderStatus = dataReader.GetTuhuString(2),
                        DeliveryStatus = dataReader.GetTuhuString(3),
                        LogisticTaskStatus = dataReader.GetTuhuString(4),
                        CreateTime = dataReader.GetTuhuValue<DateTime>(5),
                        Description = dataReader.GetTuhuString(6),
                        IsOver = dataReader.GetTuhuValue<bool>(7),
                        InstallStatus = dataReader.GetTuhuString(8)
                    };
                }
            }

            return tblOrderTrackingLog;
        }
        public static List<OrderTrackingLogEntity> SelectOrderTrackingLog(SqlConnection connection, int orderId)
        {
            var listLog = new List<OrderTrackingLogEntity>();

            var parameters = new[]
            {
                new SqlParameter("@orderId", orderId)
            };

            using (var dataReader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "存储过程", parameters))
            {
                while (dataReader.Read())
                {
                    var tblOrderTrackingLog = new OrderTrackingLogEntity
                    {
                        PkId = dataReader.GetTuhuValue<int>(0),
                        OrderId = dataReader.GetTuhuValue<int>(1),
                        OrderStatus = dataReader.GetTuhuString(2),
                        DeliveryStatus = dataReader.GetTuhuString(3),
                        LogisticTaskStatus = dataReader.GetTuhuString(4),
                        CreateTime = dataReader.GetTuhuValue<DateTime>(5),
                        Description = dataReader.GetTuhuString(6),
                        IsOver = dataReader.GetTuhuValue<bool>(7),
                        InstallStatus = dataReader.GetTuhuString(8)
                    };
                    listLog.Add(tblOrderTrackingLog);
                }
            }

            return listLog;
        }
    }
}
