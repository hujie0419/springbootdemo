using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models.Activity;
using Tuhu.Service.Activity.DataAccess.Tools;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Requests.Activity;

namespace Tuhu.Service.Activity.DataAccess
{

    public class DalTuboAlliance
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalTuboAlliance));

        /// <summary>
        /// 佣金商品列表查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<List<CommissionProductModel>> GetCommissionProductListDal(GetCommissionProductListRequest request)
        {
            var resultList = new List<CommissionProductModel>();

            try
            {
                using (var dbHelper = DbHelper.CreateDbHelper(true))
                {
                    string sqlGetCommissionProduct = @"SELECT [PKID]
                                                          ,[CpsId]
                                                          ,[PID]
                                                          ,[ProductName]
                                                          ,[CommissionRatio]
                                                          ,[IsEnable]
                                                          ,[CreateTime]
                                                          ,[CreateBy]
                                                          ,[UpdateTime]
                                                          ,[UpdateBy]
                                                          ,[IsDelete]
                                                      FROM [Activity].[dbo].[Cps_ProductList] WITH(NOLOCK) 
                                                      WHERE IsDelete =0 ";

                    if (request.IsEnable != -1)
                    {
                        sqlGetCommissionProduct += "AND IsEnable = @IsEnable";
                    }

                    sqlGetCommissionProduct += @"  ORDER BY CreateTime DESC 
                                                      OFFSET (@PageIndex-1) * @PageSize ROW
                                                      FETCH NEXT @PageSize ROW ONLY";


                    using (var cmd = new SqlCommand(sqlGetCommissionProduct))
                    {
                        cmd.CommandType = CommandType.Text;

                        if (request.IsEnable != -1)
                        {
                            cmd.Parameters.AddWithValue("@IsEnable", request.IsEnable);
                        }

                        cmd.Parameters.AddWithValue("@PageIndex", request.pageIndex);
                        cmd.Parameters.AddWithValue("@PageSize", request.pageSize);

                        var result = await dbHelper.ExecuteSelectAsync<CommissionProductModel>(cmd) ??
                                     new List<CommissionProductModel>();
                        resultList = result.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetCommissionProductListDal佣金商品列表查询接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
            }

            return resultList;

        }

        /// <summary>
        /// 佣金商品详情查询接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<CommissionProductModel> GetCommissionProductDetatilsDal(GetCommissionProductDetatilsRequest request)
        {
            var resultCommissionProduct = new CommissionProductModel();

            try
            {
                using (var dbHelper = DbHelper.CreateDbHelper())
                {
                    string sqlGetCommissionProductDetatils = @"SELECT [PKID]
                                                          ,[CpsId]
                                                          ,[PID]
                                                          ,[ProductName]
                                                          ,[CommissionRatio]
                                                          ,[IsEnable]
                                                          ,[CreateTime]
                                                          ,[CreateBy]
                                                          ,[UpdateTime]
                                                          ,[UpdateBy]
                                                          ,[IsDelete]
                                                      FROM [Activity].[dbo].[Cps_ProductList] WITH(NOLOCK) 
                                                      WHERE IsDelete =0 AND CpsId = @CpsId
                                                      AND PID = @PID";

                    using (var cmd = new SqlCommand(sqlGetCommissionProductDetatils))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@CpsId", request.CpsId);
                        cmd.Parameters.AddWithValue("@PID", request.PID);

                        resultCommissionProduct = await DbHelper.ExecuteFetchAsync<CommissionProductModel>(true, cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetCommissionProductDetatilsDal佣金商品详情查询接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
            }

            return resultCommissionProduct;
        }


        /// <summary>
        /// 下单记录商品验证数据查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<List<CommissionProductModel>> GetCommissionProductByIdsDal(List<string> pidList, List<string> cpsIDList)
        {
            var resultCommissionProductList = new List<CommissionProductModel>();

            try
            {
                using (var dbHelper = DbHelper.CreateDbHelper())
                {
                    string sqlGetCommissionProductDetatils = @"SELECT  [PKID] ,
                                                                        [CpsId] ,
                                                                        [PID] ,
                                                                        [ProductName] ,
                                                                        [CommissionRatio] ,
                                                                        [IsEnable] ,
                                                                        [CreateTime] ,
                                                                        [CreateBy] ,
                                                                        [UpdateTime] ,
                                                                        [UpdateBy] ,
                                                                        [IsDelete]
                                                                FROM    [Activity].[dbo].Cps_ProductList AS A WITH ( NOLOCK )
                                                                        JOIN Activity..SplitString(@PidList, ',', 1)
                                                                        AS B ON A.PID = B.Item
                                                                        JOIN Activity..SplitString(@CpsIdList,
                                                                                                   ',', 1) AS C ON A.CpsId = C.Item
                                                                WHERE   IsDelete = 0
                                                                ORDER BY CreateTime DESC;";

                    using (var cmd = new SqlCommand(sqlGetCommissionProductDetatils))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@PidList", string.Join(",", pidList));
                        cmd.Parameters.AddWithValue("@CpsIdList", string.Join(",", cpsIDList));

                        var result = await dbHelper.ExecuteSelectAsync<CommissionProductModel>(cmd) ??
                                     new List<CommissionProductModel>();
                        resultCommissionProductList = result.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetCommissionProductListDal佣金商品详情查询接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
            }

            return resultCommissionProductList;
        }

        /// <summary>
        /// 佣金订单商品记录创建接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<int> CreateOrderItemRecordsManager(CreateOrderItemRecordRequest request)
        {
            int resultRow = 0;

            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                try
                {

                    if (request.OrderItem?.Count > 0)
                    {
                        dbHelper.BeginTransaction();

                        var orderItemTmp = request.OrderItem.Select(item =>
                        {
                            return new
                            {
                                request.OrderId,
                                item.DarenID,
                                item.CpsId,
                                item.Pid,
                                item.Number,
                                CreateTime = DateTime.Now,
                                IsDelete = 0

                            };
                        });

                        DataTable dtOrderItem = ConvertDataTable.ToDataTable(orderItemTmp);

                        string sqlCreateOrderItemTable = @"CREATE TABLE #orderItemTmp(
	                                         [OrderId] [nvarchar](50) NOT NULL,
	                                         [DarenID] [uniqueidentifier] NULL,
	                                         [CpsId] [uniqueidentifier] NULL,
	                                         [Pid] [nvarchar](50) NULL,
	                                         [Number] [int] NULL,
	                                         [CreateTime] [datetime]  NULL,
	                                         [IsDelete] [bit] NOT NULL);";

                        await dbHelper.ExecuteNonQueryAsync(sqlCreateOrderItemTable);

                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy((SqlConnection)dbHelper.Connection,
                            SqlBulkCopyOptions.KeepIdentity, (SqlTransaction)dbHelper.Transaction))
                        {
                            bulkcopy.BulkCopyTimeout = 660;
                            bulkcopy.DestinationTableName = "#orderItemTmp";
                            bulkcopy.WriteToServer(dtOrderItem);
                            bulkcopy.Close();
                        }


                        var sqlCreateOrderItemRecord = @"INSERT INTO [Activity].[dbo].[Cps_OrderItemRecord]
                                                                                   ([OrderId]
                                                                                   ,[DarenID]
                                                                                   ,[CpsId]
                                                                                   ,[PID]
                                                                                   ,[Number]
                                                                                   ,[CreateTime]
                                                                                   ,[IsDelete])
                                                                           SELECT   [OrderId]
                                                                                   ,[DarenID]
                                                                                   ,[CpsId]
                                                                                   ,[PID]
                                                                                   ,[Number]
                                                                                   ,[CreateTime]
                                                                                   ,[IsDelete]
		                                                                           FROM #orderItemTmp";


                        using (var cmd = new SqlCommand(sqlCreateOrderItemRecord))
                        {
                            cmd.CommandType = CommandType.Text;
                            resultRow = Convert.ToInt32(await dbHelper.ExecuteNonQueryAsync(cmd));
                        }

                        dbHelper.Commit();

                    }
                }

                catch (Exception ex)
                {
                    dbHelper.Rollback();
                    Logger.Error($"CreateOrderItemRecordManager佣金订单下单记录接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
                }
            }

            return resultRow;
        }


        /// <summary>
        /// 拆单商品记录表创建
        /// </summary>
        /// <param name="orderItemRecord"></param>
        /// <returns></returns>
        public static async Task<int> CreateSplitOrderItemRecordManager(CpsSplitOrderItemRecordModel splitOrderItemRecordModel)
        {
            int resultRow = 0;

            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                try
                {

                    var sqlCpsSplitOrderItemRecord = @"INSERT INTO [Activity].[dbo].[Cps_SplitOrderItemRecord]
                                                                   ([OrderId]
                                                                   ,[CpsOrderItemRecordID]
                                                                   ,[PID]
                                                                   ,[Number]
                                                                   ,[CreateTime]
                                                                   ,[IsDelete])
                                                             VALUES
                                                                   (@OrderId
                                                                   ,@CpsOrderItemRecordID
                                                                   ,@PID
                                                                   ,@Number
                                                                   ,GETDATE()
                                                                   ,0)";


                    using (var cmd = new SqlCommand(sqlCpsSplitOrderItemRecord))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@OrderId", splitOrderItemRecordModel.OrderId);
                        cmd.Parameters.AddWithValue("@CpsOrderItemRecordID", splitOrderItemRecordModel.CpsOrderItemRecordID);
                        cmd.Parameters.AddWithValue("@PID", splitOrderItemRecordModel.PID);
                        cmd.Parameters.AddWithValue("@Number", splitOrderItemRecordModel.Number);
                        resultRow = Convert.ToInt32(await dbHelper.ExecuteNonQueryAsync(cmd));
                    }
                }

                catch (Exception ex)
                {
                    Logger.Error($"CreateSplitOrderItemRecordManager拆单商品记录表创建接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
                }
            }

            return resultRow;
        }


        /// <summary>
        /// 查询拆单商品记录
        /// </summary>
        /// <param name="orderItemRecord"></param>
        /// <returns></returns>
        public async static Task<int> GetSplitOrderItemRecordCountManager(CpsSplitOrderItemRecordModel splitOrderItemRecordModel)
        {
            int resultRow = 0;

            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                try
                {

                    var sqlSplitOrderItemRecordCount = @"SELECT  COUNT(0)
                                                            FROM    [Activity].[dbo].[Cps_SplitOrderItemRecord]
                                                            WHERE   IsDelete = 0
                                                                    AND OrderId = @OrderId
                                                                    AND CpsOrderItemRecordID = @CpsOrderItemRecordID
                                                                    AND PID = @PID
                                                                    AND Number = @Number;";


                    using (var cmd = new SqlCommand(sqlSplitOrderItemRecordCount))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@OrderId", splitOrderItemRecordModel.OrderId);
                        cmd.Parameters.AddWithValue("@CpsOrderItemRecordID", splitOrderItemRecordModel.CpsOrderItemRecordID);
                        cmd.Parameters.AddWithValue("@PID", splitOrderItemRecordModel.PID);
                        cmd.Parameters.AddWithValue("@Number", splitOrderItemRecordModel.Number);
                        resultRow = Convert.ToInt32(await dbHelper.ExecuteScalarAsync(cmd));
                    }
                }

                catch (Exception ex)
                {
                    Logger.Error($"GetSplitOrderItemRecordCountManager查询拆单商品记录接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
                }
            }

            return resultRow;
        }


        /// <summary>
        /// CPS支付流水修改状态接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<int> CpsUpdateRunningDal(CpsUpdateRunningRequest request)
        {
            int resultRow = 0;

            try
            {
                using (var dbHelper = DbHelper.CreateDbHelper())
                {

                    var sqlCpsUpdateRunning = @"UPDATE [Activity].[dbo].[Cps_CommissionFlowRecord] WITH(ROWLOCK)
                                                           SET [Status] = @Status
                                                              ,[Reason] = @Reason
                                                              ,[UpdateTime] =GETDATE()";

                    if (!string.IsNullOrWhiteSpace(request.TransactionNo))
                    {
                        sqlCpsUpdateRunning += "  ,[TransactionNo] = @TransactionNo";
                    }


                    sqlCpsUpdateRunning += " WHERE CommissionFlowRecordNo = @CommissionFlowRecordNo AND IsDelete=0";


                    using (var cmd = new SqlCommand(sqlCpsUpdateRunning))
                    {
                        cmd.CommandType = CommandType.Text;

                        if (!string.IsNullOrWhiteSpace(request.TransactionNo))
                        {
                            cmd.Parameters.AddWithValue("@TransactionNo", request.TransactionNo);
                        }
                        cmd.Parameters.AddWithValue("@STATUS", request.Status);
                        cmd.Parameters.AddWithValue("@Reason", request.Reason);
                        cmd.Parameters.AddWithValue("@CommissionFlowRecordNo", request.OutBizNo);
                        resultRow = Convert.ToInt32(await dbHelper.ExecuteNonQueryAsync(cmd));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"CpsUpdateRunningDal佣金流水状态修改接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
            }

            return resultRow;
        }

        /// <summary>
        /// 根据订单ID查询下单商品记录信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static async Task<List<CpsOrderItemRecordModel>> GetOrderItemRecordListDal(string orderId)
        {
            var resultList = new List<CpsOrderItemRecordModel>();

            try
            {
                using (var dbHelper = DbHelper.CreateDbHelper(false))
                {
                    string sqlGetOrderItemRecordList = @"SELECT [PKID]
                                                          ,[OrderId]
                                                          ,[DarenID]
                                                          ,[CpsId]
                                                          ,[PID]
                                                          ,[Number]
                                                          ,[CreateTime]
                                                          ,[UpdateTime]
                                                      FROM [Activity].[dbo].[Cps_OrderItemRecord] AS A  WITH(NOLOCK)
                                                      WHERE IsDelete = 0 AND A.OrderId = @OrderId ORDER BY CreateTime DESC";



                    using (var cmd = new SqlCommand(sqlGetOrderItemRecordList))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        var result = await dbHelper.ExecuteSelectAsync<CpsOrderItemRecordModel>(cmd) ??
                                     new List<CpsOrderItemRecordModel>();
                        resultList = result.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetOrderItemRecordListDal订单记录查询接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
            }

            return resultList;
        }


        /// <summary>
        /// 根据商品配置业务ID查询商品配置信息
        /// </summary>
        /// <param name="cpsId"></param>
        /// <returns></returns>
        public static async Task<CpsProductListModel> GetCpsProductDal(Guid cpsId)
        {
            CpsProductListModel resultModel = null;

            try
            {
                using (var dbHelper = DbHelper.CreateDbHelper(true))
                {
                    string sqlGetCpsProduct = @"SELECT [PKID]
                                                              ,[CpsId]
                                                              ,[PID]
                                                              ,[ProductName]
                                                              ,[CommissionRatio]
                                                              ,[IsEnable]
                                                              ,[CreateTime]
                                                              ,[UpdateTime]
                                                          FROM [Activity].[dbo].[Cps_ProductList] WITH(NOLOCK) WHERE   IsDelete = 0  AND CpsId =@CpsId";



                    using (var cmd = new SqlCommand(sqlGetCpsProduct))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@CpsId", cpsId);

                        resultModel = await dbHelper.ExecuteFetchAsync<CpsProductListModel>(cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetCpsProductDal商品查询接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
            }

            return resultModel;
        }

        /// <summary>
        /// CPS支付流水创建接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<int> CpsCommissionFlowRecordDal(CpsCommissionFlowRecordModel model)
        {
            int resultRow = 0;

            try
            {
                using (var dbHelper = DbHelper.CreateDbHelper())
                {

                    var sqlCpsCommissionFlowRecord = @"INSERT INTO [Activity].[dbo].[Cps_CommissionFlowRecord]
                                                               ([CommissionFlowRecordNo]
                                                               ,[OrderId]
                                                               ,[OrderItemPKID]
                                                               ,[CpsId]
                                                               ,[RedRushOrderId]
                                                               ,[DarenID]
                                                               ,[Pid]
                                                               ,[PayPrice]
                                                               ,[Number]
                                                               ,[CommissionRatio]
                                                               ,[ActutalAmount]
                                                               ,[Type]
                                                               ,[RequestNo]
                                                               ,[TransactionNo]
                                                               ,[Status]
                                                               ,[Reason]
                                                               ,[CreateTime]
                                                               ,[IsDelete])
                                                         VALUES
                                                               (@CommissionFlowRecordNo
                                                               ,@OrderId
                                                               ,@OrderItemPKID
                                                               ,@CpsId
                                                               ,@RedRushOrderId
                                                               ,@DarenID
                                                               ,@Pid
                                                               ,@PayPrice
                                                               ,@Number
                                                               ,@CommissionRatio
                                                               ,@ActutalAmount
                                                               ,@TYPE
                                                               ,@RequestNo
                                                               ,@TransactionNo
                                                               ,@STATUS
                                                               ,@Reason
                                                               ,GETDATE()
                                                               ,0)";


                    using (var cmd = new SqlCommand(sqlCpsCommissionFlowRecord))
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.AddWithValue("@CommissionFlowRecordNo", model.CommissionFlowRecordNo);
                        cmd.Parameters.AddWithValue("@OrderId", model.@OrderId);
                        cmd.Parameters.AddWithValue("@OrderItemPKID", model.OrderItemPKID);
                        cmd.Parameters.AddWithValue("@CpsId", model.CpsId);
                        cmd.Parameters.AddWithValue("@RedRushOrderId", model.RedRushOrderId);
                        cmd.Parameters.AddWithValue("@DarenID", model.DarenID);
                        cmd.Parameters.AddWithValue("@Pid", model.Pid);
                        cmd.Parameters.AddWithValue("@PayPrice", model.PayPrice);
                        cmd.Parameters.AddWithValue("@Number", model.Number);
                        cmd.Parameters.AddWithValue("@CommissionRatio", model.CommissionRatio);
                        cmd.Parameters.AddWithValue("@ActutalAmount", model.ActutalAmount);
                        cmd.Parameters.AddWithValue("@TYPE", model.Type);
                        cmd.Parameters.AddWithValue("@RequestNo", model.RequestNo);
                        cmd.Parameters.AddWithValue("@TransactionNo", model.TransactionNo);
                        cmd.Parameters.AddWithValue("@STATUS", model.Status);
                        cmd.Parameters.AddWithValue("@Reason", model.Reason);
                        resultRow = Convert.ToInt32(await dbHelper.ExecuteNonQueryAsync(cmd));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"CpsCommissionFlowRecordDal佣金流水创建接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
            }

            return resultRow;
        }

        /// <summary>
        /// 查询佣金流水
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="cpsID">佣金比例ID</param>
        /// <param name="darenID">达人ID</param>
        /// <param name="Pid">商品ID</param>
        /// <param name="type">佣金类型</param>
        /// <returns></returns>
        public static async Task<CpsCommissionFlowRecordModel> GetCommissionFlowRecordDetalDal(int orderId, Guid cpsID, Guid darenID, string pid, string runingType)
        {
            var resultModel = new CpsCommissionFlowRecordModel();

            try
            {
                using (var dbHelper = DbHelper.CreateDbHelper(false))
                {
                    string sqlGetRuningListDal = @"SELECT  [PKID] ,
                                                        [CommissionFlowRecordNo] ,
                                                        [OrderItemPKID] ,
                                                        [CpsId] ,
                                                        [OrderId] ,
                                                        [RedRushOrderId] ,
                                                        [DarenID] ,
                                                        [Pid] ,
                                                        [Number] ,
                                                        [CommissionRatio] ,
                                                        [ActutalAmount] ,
                                                        [Type] ,
                                                        [RequestNo] ,
                                                        [TransactionNo] ,
                                                        [Status] ,
                                                        [Reason] ,
                                                        [CreateTime] ,
                                                        [CreateBy] ,
                                                        [UpdateTime] ,
                                                        [UpdateBy] ,
                                                        [IsDelete]
                                                FROM    [Activity].[dbo].[Cps_CommissionFlowRecord] AS A WITH ( NOLOCK )
                                                WHERE   IsDelete = 0
                                                        AND OrderId = @OrderId
                                                        AND CpsId = @CpsId
                                                        AND DarenID = @DarenID
                                                        AND Pid = @PID";

                    if (!string.IsNullOrWhiteSpace(runingType))
                    {
                        sqlGetRuningListDal += " AND [Type] = @Type";
                    }


                    sqlGetRuningListDal += " ORDER BY CreateTime DESC";



                    using (var cmd = new SqlCommand(sqlGetRuningListDal))
                    {
                        cmd.CommandType = CommandType.Text;

                        if (!string.IsNullOrWhiteSpace(runingType))
                        {
                            cmd.Parameters.AddWithValue("@Type", runingType);
                        }

                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.Parameters.AddWithValue("@CpsId", cpsID);
                        cmd.Parameters.AddWithValue("@DarenID", darenID);
                        cmd.Parameters.AddWithValue("@Pid", pid);

                        resultModel = await dbHelper.ExecuteFetchAsync<CpsCommissionFlowRecordModel>(cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetCommissionFlowRecordDetalDal佣金流水查询接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
            }

            return resultModel;
        }




        /// <summary>
        /// 查询佣金流水
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="runingType">类型</param>
        /// <param name="orderItemPKID">订单商品PKID</param>
        /// <param name="redRushOrderId">红冲订单</param>
        /// <returns></returns>
        public static async Task<int> GetCommissionFlowRecordDal(string orderId, string runingType, int orderItemPKID, string redRushOrderId)
        {
            var resultCount = 0;

            try
            {
                using (var dbHelper = DbHelper.CreateDbHelper(false))
                {
                    string sqlGetCommissionFlowRecord = @"SELECT COUNT(0) FROM  [Activity].dbo.Cps_CommissionFlowRecord WITH(NOLOCK) 
                                                      WHERE IsDelete = 0 
                                                      AND OrderItemPKID=@OrderItemPKID ";

                    if (!string.IsNullOrWhiteSpace(runingType))
                    {
                        sqlGetCommissionFlowRecord += " AND [Type] = @Type";
                    }

                    if (!string.IsNullOrWhiteSpace(orderId))
                    {
                        sqlGetCommissionFlowRecord += "  AND OrderId = @OrderId";
                    }


                    if (!string.IsNullOrWhiteSpace(redRushOrderId))
                    {
                        sqlGetCommissionFlowRecord += "  AND RedRushOrderId = @RedRushOrderId";
                    }



                    using (var cmd = new SqlCommand(sqlGetCommissionFlowRecord))
                    {
                        cmd.CommandType = CommandType.Text;

                        if (!string.IsNullOrWhiteSpace(runingType))
                        {
                            cmd.Parameters.AddWithValue("@Type", runingType);
                        }

                        if (!string.IsNullOrWhiteSpace(orderId))
                        {
                            cmd.Parameters.AddWithValue("@OrderId", orderId);
                        }

                        if (!string.IsNullOrWhiteSpace(redRushOrderId))
                        {
                            cmd.Parameters.AddWithValue("@RedRushOrderId", redRushOrderId);
                        }


                        cmd.Parameters.AddWithValue("@OrderItemPKID", orderItemPKID);

                        resultCount = Convert.ToInt32(await dbHelper.ExecuteScalarAsync(cmd));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"CommissionFlowRecord佣金流水查询数量接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
            }

            return resultCount;
        }


        /// <summary>
        /// 佣金流水金额聚合
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static int GetCommissionFlowRecordSumAmountDal(string orderId)
        {
            var resultCount = 0;

            try
            {
                using (var dbHelper = DbHelper.CreateDbHelper(false))
                {
                    string sqlGetCommissionFlowRecordSumAmount = @"SELECT SUM(ActutalAmount)
                                                FROM    [Activity].[dbo].[Cps_CommissionFlowRecord]
                                                WHERE   IsDelete = 0
                                                        AND OrderId = @OrderId";

                    using (var cmd = new SqlCommand(sqlGetCommissionFlowRecordSumAmount))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@OrderId", orderId);

                        resultCount = Convert.ToInt32(dbHelper.ExecuteScalar(cmd));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetCommissionFlowRecordSumAmountDal佣金流水金额聚合查询接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
            }

            return resultCount;
        }

    }
}
