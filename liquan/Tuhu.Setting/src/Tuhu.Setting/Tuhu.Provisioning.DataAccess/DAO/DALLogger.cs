using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.CommonEnum;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALLogger
    {
        /// <summary>
        /// TireTJ和T-ListAct 类型的数据量较大，暂时保留
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="afterValue"></param>
        /// <returns></returns>
        public static IEnumerable<ConfigHistory> SelectOprLogByObjectTypeAndAftervalue(string objectType, string afterValue)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT  *
                                                   FROM    [Tuhu_Log].[dbo].[tbl_OprLog] WITH ( NOLOCK )
                                                   WHERE   ObjectType = @ObjectType
                                                           AND ( AfterValue = @AfterValue + ','
                                                                 OR AfterValue = @AfterValue
                                                               )
                                                   ORDER BY ChangeDatetime DESC;", CommandType.Text, new SqlParameter[] { new SqlParameter("@ObjectType", objectType), new SqlParameter("@AfterValue", afterValue) }).ConvertTo<ConfigHistory>();
            }
        }

        public static List<ConfigHistory> SelectOprLog(string ObjectType, string startDT, string endDT)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            using (var cmd = new SqlCommand(@"SELECT PKID, ObjectID,ObjectType,Author,IPAddress,HostName,Operation,ChangeDatetime FROM [Tuhu_log].[dbo].[tbl_OprLog] WHERE ObjectType=@ObjectType  AND ChangeDatetime >= @startDT  and ChangeDatetime<= @endDT  order by ChangeDatetime desc "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ObjectType", ObjectType);
                cmd.Parameters.AddWithValue("@startDT", startDT);
                cmd.Parameters.AddWithValue("@endDT", endDT);
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<ConfigHistory>().ToList();
            }
        }

        public static ConfigHistory GetConfigHistory(string PKID)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            using (var cmd = new SqlCommand(@"SELECT PKID, ObjectID,ObjectType,Author,IPAddress,HostName,Operation,ChangeDatetime,AfterValue,BeforeValue FROM [Tuhu_log].[dbo].[tbl_OprLog]  WITH(NOLOCK) WHERE PKID=@PKID    order by ChangeDatetime desc "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<ConfigHistory>().ToList().FirstOrDefault();
            }
        }
        public static FlashSaleProductOprLog GetFlashSaleHistoryByPkid(int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            using (var cmd = new SqlCommand(@"SELECT * FROM [Tuhu_log].[dbo].[FlashSaleProductOprLog]  WITH(NOLOCK) WHERE PKID=@PKID    order by CreateDatetime desc "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<FlashSaleProductOprLog>().ToList().FirstOrDefault();
            }
        }
        public static List<ConfigHistory> SelectConfigHistory(string objectid, string objectType)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            using (var cmd = new SqlCommand(@"SELECT PKID, ObjectID,ObjectType,Author,IPAddress,HostName,Operation,ChangeDatetime,AfterValue,BeforeValue FROM [Tuhu_log].[dbo].[tbl_OprLog]  WITH(NOLOCK) WHERE ObjectId=@ObjectId and ObjectType=@ObjectType    order by ChangeDatetime desc "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ObjectId", objectid);
                cmd.Parameters.AddWithValue("@ObjectType", objectType);
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<ConfigHistory>().ToList();
            }
        }
        public static List<FlashSaleProductOprLog> SelectFlashSaleHistoryByLogId(string logId, string logType)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            using (var cmd = new SqlCommand(@"SELECT * FROM [Tuhu_log].[dbo].[FlashSaleProductOprLog]  WITH(NOLOCK) WHERE LogId=@LogId  and LogType=@LogType order by CreateDatetime desc "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@logId", logId);
                cmd.Parameters.AddWithValue("@LogType", logType);
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<FlashSaleProductOprLog>().ToList();
            }
        }
        public static FlashSaleProductOprLog SelectFlashSaleHistoryDetailByLogId(int pkid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            using (var cmd = new SqlCommand(@"SELECT * FROM [Tuhu_log].[dbo].[FlashSaleProductOprLog]  WITH(NOLOCK) WHERE Pkid=@Pkid"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Pkid", pkid);
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<FlashSaleProductOprLog>().FirstOrDefault();
            }
        }
        #region 产品关联车型配置操作日志
        public static int InsertOpLogForProductVehicleTypeConfig(ProductVehicleTypeConfigOpLog logEntity)
        {
            var conn = ConfigurationManager.ConnectionStrings["SystemLogConnectionString"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            var sql = @"INSERT INTO ProductVehicleTypeConfigOprLog(PID,Operator,OperateContent,OperateTime,CreatedTime) VALUES(@Pid,@Operator,@OperateContent,@OperateTime,@CreatedTime)";
            try
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Pid", logEntity.PID);
                    cmd.Parameters.AddWithValue("@Operator", logEntity.Operator);
                    cmd.Parameters.AddWithValue("@OperateContent", logEntity.OperateContent);
                    cmd.Parameters.AddWithValue("@OperateTime", logEntity.OperateTime);
                    cmd.Parameters.AddWithValue("@CreatedTime", logEntity.CreatedTime);
                    return dbhelper.ExecuteNonQuery(cmd);
                }
            }
            catch (Exception e)
            {
                return -1;
            }


        }

        public static bool BulkSaveOperateLogInfo(DataTable table)
        {
            var conn = ConfigurationManager.ConnectionStrings["SystemLogConnectionString"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }

            using (var bulk = new SqlBulkCopy(conn))
            {

                bulk.BatchSize = table.Rows.Count;
                bulk.DestinationTableName = "ProductVehicleTypeConfigOprLog";

                bulk.ColumnMappings.Add("PID", "PID");
                bulk.ColumnMappings.Add("Operator", "Operator");
                bulk.ColumnMappings.Add("OperateContent", "OperateContent");
                bulk.ColumnMappings.Add("OperateTime", "OperateTime");
                bulk.ColumnMappings.Add("CreatedTime", "CreatedTime");

                bulk.WriteToServer(table);

            }
            return true;

        }

        public static List<ProductVehicleTypeConfigOpLog> GetAllLog()
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            var sql = @"select Id,PID,Operator,OperateContent,OperateTime,CreatedTime from Tuhu_log..ProductVehicleTypeConfigOprLog";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<ProductVehicleTypeConfigOpLog>().ToList();
            }
        }

        public static List<ProductVehicleTypeConfigOpLog> GetAllLogByPid(string pid)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            var sql = @"select Id,PID,Operator,OperateContent,OperateTime,CreatedTime from Tuhu_log..ProductVehicleTypeConfigOprLog AS pvtcol where pvtcol.PID=@Pid";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Pid", pid);
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<ProductVehicleTypeConfigOpLog>().ToList();
            }

        }

        public static List<ProductVehicleTypeConfigOpLog> GetAllLogByTime(string timeS, string timeE)
        {
            var resultList = new List<ProductVehicleTypeConfigOpLog>();
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            var dtStart = new DateTime();
            var dtEnd = new DateTime();
            var sql = "";
            var cmd = new SqlCommand();
            try
            {
                if (!string.IsNullOrEmpty(timeS) && !string.IsNullOrEmpty(timeE))
                {
                    DateTime.TryParse(timeS, out dtStart);
                    DateTime.TryParse(timeE, out dtEnd);
                    sql = @"select Id,PID,Operator,OperateContent,OperateTime,CreatedTime from Tuhu_log..ProductVehicleTypeConfigOprLog AS pvtcol where pvtcol.OperateTime>=@TimeStart and pvtcol.OperateTime<=@TimeEnd ";
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@TimeStart", dtStart);
                    cmd.Parameters.AddWithValue("@TimeEnd", dtEnd);
                    return dbhelper.ExecuteDataTable(cmd).ConvertTo<ProductVehicleTypeConfigOpLog>().ToList();

                }

                if (!string.IsNullOrEmpty(timeS) && string.IsNullOrEmpty(timeE))
                {
                    DateTime.TryParse(timeS, out dtStart);
                    sql = @"select Id,PID,Operator,OperateContent,OperateTime,CreatedTime from Tuhu_log..ProductVehicleTypeConfigOprLog AS pvtcol where pvtcol.OperateTime>=@TimeStart ";
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@TimeStart", dtStart);
                    return dbhelper.ExecuteDataTable(cmd).ConvertTo<ProductVehicleTypeConfigOpLog>().ToList();
                }

                if (string.IsNullOrEmpty(timeS) && !string.IsNullOrEmpty(timeE))
                {
                    DateTime.TryParse(timeE, out dtEnd);
                    sql = @"select Id,PID,Operator,OperateContent,OperateTime,CreatedTime from Tuhu_log..ProductVehicleTypeConfigOprLog AS pvtcol where pvtcol.OperateTime<=@TimeEnd ";
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@TimeEnd", dtEnd);
                    return dbhelper.ExecuteDataTable(cmd).ConvertTo<ProductVehicleTypeConfigOpLog>().ToList();

                }

            }
            catch (Exception e)
            {
            }
            return resultList;
        }
        #endregion

        public static int InsertFlashSaleProductsLog(QiangGouProductModel logEntity, string HashKey)
        {
            try
            {

                var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                var sql = @"INSERT INTO tuhu_log.[dbo].[FlashSaleProductsLog]([ActivityID],[PID],[HashKey],[Position],[Price],[Label],[TotalQuantity]
	,[MaxQuantity],[SaleOutQuantity],[ProductName],[InstallAndPay]
	,[IsUsePCode],[Channel],[FalseOriginalPrice],[IsJoinPlace],[IsShow],[InstallService])VALUES(@ActivityId,@Pid,@HashKey,
	@Position,@Price,@Label,@TotalQuantity,@MaxQuantity,@SaleOutQuantity,@ProductName,
	@InstallAndPay,@IsUsePCode,@Channel,@FalseOriginalPrice,@IsJoinPlace,@IsShow,@InstallService)
";
                var dbhelper = new SqlDbHelper(conn);
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@ActivityId", logEntity.ActivityID);
                    cmd.Parameters.AddWithValue("@Pid", logEntity.PID);
                    cmd.Parameters.AddWithValue("@HashKey", HashKey);
                    cmd.Parameters.AddWithValue("@Position", logEntity.Position);
                    cmd.Parameters.AddWithValue("@Price", logEntity.Price);
                    cmd.Parameters.AddWithValue("@Label", logEntity.Label);
                    cmd.Parameters.AddWithValue("@TotalQuantity", logEntity.TotalQuantity);
                    cmd.Parameters.AddWithValue("@MaxQuantity", logEntity.MaxQuantity);
                    cmd.Parameters.AddWithValue("@SaleOutQuantity", logEntity.SaleOutQuantity);
                    cmd.Parameters.AddWithValue("@ProductName", logEntity.ProductName);
                    cmd.Parameters.AddWithValue("@InstallAndPay", logEntity.InstallAndPay);
                    cmd.Parameters.AddWithValue("@IsUsePCode", logEntity.IsUsePCode);
                    cmd.Parameters.AddWithValue("@Channel", logEntity.Channel);
                    cmd.Parameters.AddWithValue("@FalseOriginalPrice", logEntity.FalseOriginalPrice);
                    cmd.Parameters.AddWithValue("@IsJoinPlace", logEntity.IsJoinPlace);
                    cmd.Parameters.AddWithValue("@IsShow", logEntity.IsShow);
                    cmd.Parameters.AddWithValue("@InstallService", logEntity.InstallService);
                    return dbhelper.ExecuteNonQuery(cmd);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static bool BatchInsertFlashSaleProductsLog(QiangGouModel model, string HashKey)
        {
            try
            {
                var dt = new DataTable("FlashSaleProductsLog");
                DataColumn dc0 = new DataColumn("PKID", typeof(int));
                DataColumn dc1 = new DataColumn("ActivityID", typeof(Guid));
                var dc2 = new DataColumn("PID", typeof(string));
                var dc3 = new DataColumn("HashKey", typeof(string));
                var dc4 = new DataColumn("Position", typeof(int));
                var dc5 = new DataColumn("Price", typeof(decimal));
                var dc6 = new DataColumn("Label", typeof(string));
                var dc7 = new DataColumn("TotalQuantity", typeof(int));
                var dc8 = new DataColumn("MaxQuantity", typeof(int));
                var dc9 = new DataColumn("SaleOutQuantity", typeof(int));
                var dc10 = new DataColumn("ProductName", typeof(string));
                var dc11 = new DataColumn("InstallAndPay", typeof(string));
                var dc12 = new DataColumn("IsUsePCode", typeof(int));
                var dc13 = new DataColumn("Channel", typeof(string));
                var dc14 = new DataColumn("FalseOriginalPrice", typeof(decimal));
                var dc15 = new DataColumn("IsJoinPlace", typeof(int));
                var dc16 = new DataColumn("IsShow", typeof(int));
                var dc17 = new DataColumn("InstallService", typeof(string));


                dt.Columns.Add(dc0);
                dt.Columns.Add(dc1);
                dt.Columns.Add(dc2);
                dt.Columns.Add(dc3);
                dt.Columns.Add(dc4);
                dt.Columns.Add(dc5);
                dt.Columns.Add(dc6);
                dt.Columns.Add(dc7);
                dt.Columns.Add(dc8);
                dt.Columns.Add(dc9);
                dt.Columns.Add(dc10);
                dt.Columns.Add(dc11);
                //dt.Columns.Add(dc0);
                dt.Columns.Add(dc12);
                dt.Columns.Add(dc13);
                dt.Columns.Add(dc14);
                dt.Columns.Add(dc15);
                dt.Columns.Add(dc16);
                dt.Columns.Add(dc17);
                foreach (var p in model.Products)
                {
                    var dr = dt.NewRow();
                    dr["PKID"] = DBNull.Value;
                    dr["ActivityID"] = p.ActivityID;
                    dr["PID"] = p.PID;
                    dr["HashKey"] = HashKey;
                    dr["Position"] = p.Position;
                    dr["Price"] = p.Price;
                    dr["Label"] = p.Label;
                    dr["TotalQuantity"] = p.TotalQuantity;
                    dr["MaxQuantity"] = p.MaxQuantity;
                    dr["SaleOutQuantity"] = p.SaleOutQuantity;
                    dr["ProductName"] = p.ProductName;
                    dr["InstallAndPay"] = p.InstallAndPay;
                    dr["IsUsePCode"] = p.IsUsePCode;
                    dr["Channel"] = p.Channel;
                    dr["FalseOriginalPrice"] = p.FalseOriginalPrice;
                    dr["IsJoinPlace"] = p.IsJoinPlace;
                    dr["IsShow"] = p.IsShow;
                    dr["InstallService"] = p.InstallService;
                    dt.Rows.Add(dr);
                }

                var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(conn))
                {
                    conn = SecurityHelp.DecryptAES(conn);
                }
                using (var cmd = new SqlBulkCopy(conn))
                {
                    cmd.BatchSize = model.Products.Count();
                    cmd.BulkCopyTimeout = 10;
                    cmd.DestinationTableName = "tuhu_log..FlashSaleProductsLog";
                    cmd.WriteToServer(dt);
                    return true;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static List<QiangGouProductModel> SelectFlashSaleProductsLog(string hashKey)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            using (var cmd = new SqlCommand(@"SELECT * FROM [Tuhu_log].[dbo].[FlashSaleProductsLog]  WITH(NOLOCK) WHERE hashKey=@hashKey "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@hashKey", hashKey);
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<QiangGouProductModel>().ToList();
            }
        }


        /// <summary>
        /// 查询城市失效log
        /// </summary>
        /// <param name="logId"></param>
        /// <param name="logType"></param>
        /// <returns></returns>
        public static List<FlashSaleProductOprLog> SelectCityAgingHistoryByLogId(string logId, string logType)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            using (var cmd = new SqlCommand(@"SELECT * FROM [Tuhu_log].[dbo].[CityAgingOprLog]  WITH(NOLOCK) WHERE LogId=@LogId  and LogType=@LogType order by CreateDatetime desc "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@logId", logId);
                cmd.Parameters.AddWithValue("@LogType", logType);
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<FlashSaleProductOprLog>().ToList();
            }
        }



        /// <summary>
        /// 通用日志查询方法 
        /// </summary>
        /// <param name="logId"></param>
        /// <param name="logType"></param>
        /// <returns></returns>
        public static List<CommonOprLog> SelectOpLogHistoryByLogId(string tableName,string logId, string logType)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            var dbhelper = new SqlDbHelper(conn);
            using (var cmd = new SqlCommand($@"SELECT * FROM [Tuhu_log].[dbo].[{tableName}] 
            WITH(NOLOCK) WHERE LogId=@LogId  and LogType=@LogType order by CreateDatetime desc "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@logId", logId);
                cmd.Parameters.AddWithValue("@LogType", logType);
                return dbhelper.ExecuteDataTable(cmd).ConvertTo<CommonOprLog>().ToList();
            }
        }
    }
}
