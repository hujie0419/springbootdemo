using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalOprLog
    {
        public static void AddOprLog(SqlConnection connection, OprLog oprLog)
        {
            var parameters = new[]
            {
                new SqlParameter("@Author", oprLog.Author?? string.Empty),
                new SqlParameter("@ObjectType", oprLog.ObjectType?? string.Empty),
                new SqlParameter("@ObjectId", oprLog.ObjectID),
                new SqlParameter("@BeforeValue", oprLog.BeforeValue?? string.Empty),
                new SqlParameter("@AfterValue", oprLog.AfterValue?? string.Empty),
                new SqlParameter("@ChangeDatetime", oprLog.ChangeDatetime?? DateTime.Now),
                new SqlParameter("@IpAddress", oprLog.IPAddress?? string.Empty),
                new SqlParameter("@HostName",oprLog.HostName?? string.Empty),
				new SqlParameter("@Operation",oprLog.Operation?? string.Empty)
            };

            SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "procOprLogInsert", parameters);
        }
        public static void AddOprLog(SqlTransaction tran, OprLog oprLog)
        {
            var parameters = new[]
            {
                new SqlParameter("@Author", oprLog.Author?? string.Empty),
                new SqlParameter("@ObjectType", oprLog.ObjectType?? string.Empty),
                new SqlParameter("@ObjectId", oprLog.ObjectID),
                new SqlParameter("@BeforeValue", oprLog.BeforeValue?? string.Empty),
                new SqlParameter("@AfterValue", oprLog.AfterValue?? string.Empty),
                new SqlParameter("@ChangeDatetime", oprLog.ChangeDatetime?? DateTime.Now),
                new SqlParameter("@IpAddress", oprLog.IPAddress?? string.Empty),
                new SqlParameter("@HostName",oprLog.HostName?? string.Empty),
				new SqlParameter("@Operation",oprLog.Operation?? string.Empty)
            };

            SqlHelper.ExecuteNonQuery(tran, CommandType.StoredProcedure, "procOprLogInsert", parameters);
        }
        public static void AddEmailProcess(SqlConnection connection, int OrderId, BizEmailProcess emailProcess)
        {
            var parameters = new[]
            {
                new SqlParameter("@OrderId", OrderId),
                new SqlParameter("@EmailType", emailProcess.Type??string.Empty),
                new SqlParameter("@url", emailProcess.url??string.Empty),
                new SqlParameter("@FromMail", emailProcess.FromMail??string.Empty),
				new SqlParameter("@ToMail", emailProcess.ToMail??string.Empty),
				new SqlParameter("@CC", emailProcess.CC??string.Empty),
                new SqlParameter("@Subject", emailProcess.Subject),
                new SqlParameter("@Status", emailProcess.Status),
                new SqlParameter("@Type", emailProcess.Type),
				new SqlParameter("@Body", emailProcess.Body),
                new SqlParameter("@OrderNo", emailProcess.OrderNo??string.Empty)
            };

            SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "EmailProcess_CheckEmailProcessSending", parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="OrderId">订单号：64137</param>
        /// <param name="EmailType">类型：到店短信/发货短信/确认短信 </param>
        /// <returns></returns>
        public static bool IsExistsEmailSendingMark(SqlConnection connection, int OrderId, string EmailType = "到店短信")
        {
            var list = new List<OprLog>();
            string sql = "SELECT COUNT(1) FROM dbo.tbl_EmailSendingMark WITH ( NOLOCK ) WHERE  OrderId = @OrderId  AND EmailType = @EmailType";
            var parameters = new[]
            {
                new SqlParameter("@OrderId",OrderId),
                new SqlParameter("@EmailType", EmailType)
            };
            object obj = SqlHelper.ExecuteScalar(connection, CommandType.Text, sql, parameters);
            if (obj != null)
            {
                int i = 0;
                int.TryParse(obj.ToString(), out i);
                if (i > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 往EmailProcess表中添加数据
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="emailProcess"></param>
        public static void AddEmailProcess(SqlConnection connection, BizEmailProcess emailProcess)
        {
            string sql = @"INSERT	INTO dbo.tbl_EmailProcess WITH (ROWLOCK)
		                            (   url,
		                                FromMail,
		                                ToMail,
		                                CC,
		                                Subject,
		                                InsertTime,
		                                Status,
		                                Type,
		                                Body,
		                                OrderNo,
		                                IsActive,
		                                LastUpdateTime
		                            )
                            VALUES	(   @url, -- url - nvarchar(200)  
		                                @FromMail, -- FromMail - nvarchar(50)  
		                                @ToMail, -- ToMail - nvarchar(100)  
		                                @CC, -- CC - nvarchar(100)  
		                                @Subject, -- Subject - nvarchar(300)  
		                                GETDATE(), -- InsertTime - smalldatetime  
		                                @Status, -- Status - nvarchar(20)  
		                                @Type, -- Type - nvarchar(30)  
		                                @Body, -- Body - ntext 
		                                @OrderNo,
		                                1, -- IsActive - bit  
		                                GETDATE()  -- LastUpdateTime - smalldatetime  
		                            ) ";
            var parameters = new[]
            {
                new SqlParameter("@url", emailProcess.url??string.Empty),
                new SqlParameter("@FromMail", emailProcess.FromMail??string.Empty),
				new SqlParameter("@ToMail", emailProcess.ToMail??string.Empty),
				new SqlParameter("@CC", emailProcess.CC??string.Empty),
                new SqlParameter("@Subject", emailProcess.Subject),
                new SqlParameter("@Status", emailProcess.Status),
                new SqlParameter("@Type", emailProcess.Type),
				new SqlParameter("@Body", emailProcess.Body),
                new SqlParameter("@OrderNo",emailProcess.OrderNo)
            };

            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, parameters);
        }

        public static List<BizEmailProcess> SelectEmailProcessesByOrderNo(DateTime startDateTime, DateTime endDateTime,
            string orderNo, string userTel)
        {
            var parameters = new[]
            {
                new SqlParameter("@StartDateTime", startDateTime),
                new SqlParameter("@EndDateTime", endDateTime),
                new SqlParameter("@OrderNo", orderNo),
                new SqlParameter("@OrderId", orderNo.Replace("TH"," ")),
                new SqlParameter("@UserTel", userTel)
            };
            return
                DbHelper.ExecuteDataTable("EmailProcess_SelectEmailProcessByOrderNo", CommandType.StoredProcedure,
                    parameters).ConvertTo<BizEmailProcess>().ToList();
        }

        public static bool CheckEmailIsSent(string body)
        {
            var parameters = new[]
            {
                new SqlParameter("@Body", body??string.Empty)
            };
            return Convert.ToBoolean(DbHelper.ExecuteScalar("EmailProcess_CheckEmailIsSentByBodyAndInsertTime", CommandType.StoredProcedure, parameters));
        }
        public static bool AddEmailProcessList(SqlConnection connection, List<BizEmailProcess> emailProcess)
        {
            foreach (BizEmailProcess item in emailProcess)
            {
                var parameters = new[]
                {
                    new SqlParameter("@OrderId", item.OrderID),
                    new SqlParameter("@EmailType", item.Type??string.Empty),
                    new SqlParameter("@url", item.url??string.Empty),
                    new SqlParameter("@FromMail", item.FromMail??string.Empty),
				    new SqlParameter("@ToMail", item.ToMail??string.Empty),
				    new SqlParameter("@CC", item.CC??string.Empty),
                    new SqlParameter("@Subject", item.Subject),
                    new SqlParameter("@Status", item.Status),
                    new SqlParameter("@Type", item.Type),
				    new SqlParameter("@Body", item.Body),
                    new SqlParameter("@OrderNo", item.OrderNo)
                };

                int i = SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "Insert_SendEmailProcess", parameters);
                if (i <= 0)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 订单安装后发送短信
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="orderId"></param>
        public static void InsertOrderInstalledEmailProcess(SqlConnection connection, int orderId)
        {
            var parameters = new[]
                {
                    new SqlParameter("@OrderId", orderId)
                };

            SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "EmailProcess_OrderInstalled_InsertEmailProcess", parameters);
        }

        /// <summary>
        /// 添加步骤时刻记录
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="orderId"></param>
        /// <param name="operatorEmail"></param>
        /// <param name="description"></param>
        public static void AddSystemOperationTimeLog(SqlConnection connection, int orderId, string description)
        {
            var parameters = new[]
                {
                    new SqlParameter("@OrderId", orderId),
                    new SqlParameter("@Description", description)
                };

            SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "SystemLog_tbl_tbl_OperationTimeLog_AddLog", parameters);
        }

        /// <summary>
        /// 添加序列化好的oprlog
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objID"></param>
        /// <param name="otype"></param>
        /// <param name="operation"></param>
        /// <param name="beforeValue"></param>
        /// <param name="afterValue"></param>
        //public static void AddSerializerOprLog(SqlConnection connection, string objType, int objID, Type otype, string operation, object beforeValue = null, object afterValue = null)
        //{
        //    var oprLog = new OprLog();
        //    oprLog.Author = ThreadIdentity.Operator.Name;

        //    DataContractSerializer dcs = new DataContractSerializer(otype);
        //    if (beforeValue != null)
        //    {
        //        if (beforeValue is string)
        //        {
        //            oprLog.BeforeValue = beforeValue.ToString();
        //        }
        //        else
        //        {
        //            StringBuilder builder = new StringBuilder();
        //            foreach (var property in beforeValue.GetType().GetProperties())
        //            {
        //                object value = property.GetValue(beforeValue, null);
        //                if (value != null)
        //                {
        //                    builder.Append(property.Name)
        //                    .Append(" = ")
        //                    .Append((value ?? "null"))
        //                    .AppendLine();
        //                }
        //            }
        //            oprLog.BeforeValue = builder.ToString();
        //        }
        //    }
        //    if (afterValue != null)
        //    {
        //        if (afterValue is string)
        //        {
        //            oprLog.AfterValue = afterValue.ToString();
        //        }
        //        else
        //        {
        //            StringBuilder builder = new StringBuilder();

        //            foreach (var property in afterValue.GetType().GetProperties())
        //            {

        //                object value = property.GetValue(afterValue, null);
        //                if (value != null)
        //                {
        //                    builder.Append(property.Name)
        //                    .Append(" = ")
        //                    .Append((value ?? "null"))
        //                    .AppendLine();
        //                }
        //            }
        //            oprLog.AfterValue = builder.ToString();
        //        }
        //    }

        //    oprLog.ChangeDatetime = DateTime.Now;
        //    oprLog.ObjectID = objID;
        //    oprLog.ObjectType = objType;
        //    oprLog.Operation = operation;
        //    AddOprLog(connection, oprLog);
        //}
        #region private method
        private static OprLog ReadData(SqlDataReader reader)
        {
            //O.[PKID],O.[Author],O.[ObjectType],O.[ObjectID],O.[BeforeValue],O.[AfterValue],
            //O.[ChangeDatetime],O.[IPAddress],O.[HostName],O.[Operation],H.EmployeeName
            OprLog log = new OprLog()
              {
                  PKID = reader.GetInt32(0),
                  Author = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                  ObjectType = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                  ObjectID = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                  BeforeValue = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                  AfterValue = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                  ChangeDatetime = reader.IsDBNull(6) ? null : (DateTime?)reader.GetDateTime(6),
                  IPAddress = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                  HostName = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                  Operation = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                  EmployeeName = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
              };
            return log;
        }
        #endregion
        /// <summary>
        /// 查询历史短信
        /// </summary>
        /// <param name="searchType">查询类型</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页显示行数</param>
        /// <param name="toMail">电话</param>
        /// <param name="orderNo">订单号</param>
        /// <returns></returns>
        public static List<BizEmailProcess> SelectSMSHistoryBySearchVal(string searchType, int pageIndex, int pageSize, string toMail, string orderNo)
        {
            var sqlParameters = new[]
            {
                new SqlParameter("@searchType", searchType),
                new SqlParameter("@pageIndex", pageIndex),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@toMail", string.IsNullOrWhiteSpace(toMail)?DBNull.Value:(object)toMail),
                new SqlParameter("@orderNo", string.IsNullOrWhiteSpace(orderNo)?DBNull.Value:(object)orderNo)
            };
            return
                DbHelper.ExecuteDataTable("proc_OrderSMSHistory", CommandType.StoredProcedure,
                    sqlParameters).ConvertTo<BizEmailProcess>().ToList();
        }
        /// <summary>
        /// 往EmailProcess表中添加数据
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="emailProcess"></param>
        public static void AddMarketingSms(SqlConnection connection, BizEmailProcess emailProcess)
        {

            var parameters = new[]
            {
				new SqlParameter("@PhoneNumber", emailProcess.ToMail??string.Empty),
                new SqlParameter("@MsgSubject", emailProcess.Subject),
				new SqlParameter("@MsgBody", emailProcess.Body),
                new SqlParameter("@RelatedUser", emailProcess.RelatedUser),
                new SqlParameter("@OrderId", emailProcess.OrderID),
                new SqlParameter("@FromMail", emailProcess.FromMail)
            };

            SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "MarketingSms_Insert", parameters);
        }

        public static void AddShopEditOprLog(SqlConnection connection, ShopEditOprLog oprLog)
        {
            var parameters = new[]
            {
				new SqlParameter("@ShopID", oprLog.ShopID),
                new SqlParameter("@Author", oprLog.Author?? string.Empty),
                new SqlParameter("@ObjectType", oprLog.ObjectType?? string.Empty),
                new SqlParameter("@ObjectId", oprLog.ObjectID),
                new SqlParameter("@BeforeValue", oprLog.BeforeValue?? string.Empty),
                new SqlParameter("@AfterValue", oprLog.AfterValue?? string.Empty),
                new SqlParameter("@IpAddress", oprLog.IPAddress?? string.Empty),
				new SqlParameter("@Operation",oprLog.Operation?? string.Empty),
				new SqlParameter("@Remark",oprLog.Remark?? string.Empty)
            };

            SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "Shop_AddShopEditOprLog", parameters);
        }

        public static void AddShopEditOprLog(ShopEditOprLog oprLog)
        {
            var parameters = new[]
            {
				new SqlParameter("@ShopID", oprLog.ShopID),
                new SqlParameter("@Author", oprLog.Author?? string.Empty),
                new SqlParameter("@ObjectType", oprLog.ObjectType?? string.Empty),
                new SqlParameter("@ObjectId", oprLog.ObjectID),
                new SqlParameter("@BeforeValue", oprLog.BeforeValue?? string.Empty),
                new SqlParameter("@AfterValue", oprLog.AfterValue?? string.Empty),
                new SqlParameter("@IpAddress", oprLog.IPAddress?? string.Empty),
				new SqlParameter("@Operation",oprLog.Operation?? string.Empty),
				new SqlParameter("@Remark",oprLog.Remark?? string.Empty)
            };

            DbHelper.ExecuteNonQuery("Shop_AddShopEditOprLog", CommandType.StoredProcedure, parameters);
        }

        public static void AddShopServiceOprLog(SqlConnection connection, ShopServiceLog oprLog)
        {
            var parameters = new[]
            {
				new SqlParameter("@ShopID", oprLog.ShopID),
                new SqlParameter("@Author", oprLog.Author?? string.Empty),
                new SqlParameter("@ProductName", oprLog.ProductName?? string.Empty),
                new SqlParameter("@ProductID", oprLog.ProductID?? string.Empty),
                new SqlParameter("@Value", oprLog.Value?? string.Empty),
                new SqlParameter("@IpAddress", oprLog.IPAddress?? string.Empty),
				new SqlParameter("@Operation",oprLog.Operation?? string.Empty)
            };

            SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "Shop_AddShopServiceOprLog", parameters);
        }

        public static List<ShopServiceLog> SelectShopServiceLog(SqlConnection connection, int shopId, string productID, int pageNum)
        {
            SqlParameter[] sqlparames =
			{
				new SqlParameter("@ShopID",shopId),
				new SqlParameter("@StartIndex",(pageNum-1)*50),
				new SqlParameter("@EndIndex",pageNum*50)
			};
            return SqlHelper.ExecuteDataTable(connection, CommandType.StoredProcedure, "Shop_SelectShopServiceLog", sqlparames).ConvertTo<ShopServiceLog>().ToList();
        }

        public static List<ShopServiceLog> SelectShopServiceOprLogDetail(SqlConnection connection, int shopId, string productId, int pkid)
        {
            string sql = @"SELECT TOP 2
        *
FROM    dbo.ShopServiceLog AS opr WITH ( NOLOCK )
WHERE   opr.ShopID = @ShopID
        AND opr.ProductID = @ProductID
        AND opr.PKID <= @PKID
ORDER BY opr.PKID DESC";
            SqlParameter[] sqlParams =
			{
				new SqlParameter("@ShopId",shopId),
				new SqlParameter("@ProductId",productId),
				new SqlParameter("@PKID",pkid)
			};
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, sqlParams).ConvertTo<ShopServiceLog>().ToList();
        }
    }
}
