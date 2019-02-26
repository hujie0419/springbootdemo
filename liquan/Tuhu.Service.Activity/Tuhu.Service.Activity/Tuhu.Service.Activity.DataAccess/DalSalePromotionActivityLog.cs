using Common.Logging;
using log4net.Repository.Hierarchy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalSalePromotionActivityLog
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalSalePromotionActivityLog));

        #region 查询

        /// <summary>
        /// 查询操作日志记录
        /// </summary>
        /// <param name="referId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<PagedModel<SalePromotionActivityLogModel>> GetOperationLogListAsync(string referId, int pageIndex = 1, int pageSize = 20)
        {
            var pageResult = new PagedModel<SalePromotionActivityLogModel>();
            string sqlSource = @"select log.PKID,
                                        log.ReferId,
                                        log.OperationLogType,
                                        log.CreateDateTime,
                                        log.CreateUserName,
                                        des.OperationLogDescription
                                from   [Activity].[dbo].[SalePromotionActivityLog] log with(nolock)
                             left join [Activity].[dbo].[SalePromotionLogDescription] des with(nolock)
                                       on log.OperationLogType=des.OperationLogType
                               where   log.ReferId=@ReferId
                            order by   log.CreateDateTime desc
                             OFFSET    (@pageIndex -1) * @pageSize ROWS
                           FETCH NEXT   @pageSize ROWS ONLY ";
            string sqlCount = @"select  count(*)
                                 from   [Activity].[dbo].[SalePromotionActivityLog] with(nolock)
                                where   ReferId=@ReferId";
            using (var cmd = new SqlCommand(sqlCount))
            {
                try
                {
                    cmd.Parameters.AddWithValue("@ReferId", referId);
                    int count;
                    int.TryParse((await DbHelper.ExecuteScalarAsync(cmd)).ToString(), out count);
                    pageResult.Pager = new PagerModel() { Total = count };
                    if (count > 0)
                    {
                        cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                        cmd.CommandText = sqlSource;
                        pageResult.Source = await DbHelper.ExecuteSelectAsync<SalePromotionActivityLogModel>(cmd);
                    }
                    else
                    {
                        pageResult.Source = new List<SalePromotionActivityLogModel>();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"GetOperationLogListAsync异常，{ex}");
                }
            }
            return pageResult;
        }

        /// <summary>
        /// 批量获取日志的详情操作数量
        /// </summary>
        /// <param name="PKIDs"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SalePromotionActivityLogModel>> GetDetailCountListAsync(List<string> PKIDs)
        {
            var result = new List<SalePromotionActivityLogModel>();
            string sql = @"Select COUNT(a.PKID) AS DetailCount, cast(a.FPKID as nvarchar(10)) as PKID
                       FROM [Activity].[dbo].[SalePromotionActivityLogDetail] a With (NOLOCK) 
                       JOIN Activity..SplitString(@PKIDs, ',', 1) AS B ON a.FPKID = B.Item
                       GROUP BY a.FPKID";
            using (var cmd = new SqlCommand(sql))
            {
                try
                {
                    cmd.Parameters.AddWithValue("@PKIDs", string.Join(",", PKIDs));
                    result = (await DbHelper.ExecuteSelectAsync<SalePromotionActivityLogModel>(cmd))?.ToList();
                }
                catch (Exception ex)
                {
                    Logger.Error($"GetDetailCountListAsync异常，{ex}");
                }
                return result;
            }
        }

        /// <summary> 
        /// 获取操作日志详情列表
        /// </summary>
        /// <param name="operationId">操作日志id</param>
        /// <returns></returns>
        public static async Task<IEnumerable<SalePromotionActivityLogDetail>> GetOperationLogDetailListAsync(string FPKID)
        {
            string sql = @"select  OperationLogType,Property,OldValue,NewValue
                            from   [Activity].[dbo].[SalePromotionActivityLogDetail]  with(nolock)
                           where   FPKID=@FPKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@FPKID", FPKID);
                return await DbHelper.ExecuteSelectAsync<SalePromotionActivityLogDetail>(cmd);
            }
        }

        #endregion

        /// <summary>
        /// 插入操作日志 包含日志详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<bool> InsertAcitivityLogAndDetailAsync(SalePromotionActivityLogModel model)
        {
            bool result = true;
            int PKID;
            string SqlInsertLog = @"Insert into [Activity].[dbo].[SalePromotionActivityLog](
                                                 [ReferId],[ReferType],OperationLogType,
                                                 [CreateDateTime],[CreateUserName])
                        values(@ReferId,@ReferType,@OperationLogType,getdate(),@CreateUserName);
                        SELECT SCOPE_IDENTITY();";
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = new SqlCommand("", conn, tran))
                    {
                        try
                        {
                            //插入日志
                            cmd.CommandText = SqlInsertLog;
                            cmd.Parameters.Add(new SqlParameter("@ReferId", model.ReferId ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@ReferType", model.ReferType ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@OperationLogType", model.OperationLogType ?? ""));
                            cmd.Parameters.Add(new SqlParameter("@CreateUserName", model.CreateUserName ?? ""));
                            //获取自增主键
                            int.TryParse((await cmd.ExecuteScalarAsync()).ToString(), out PKID);
                            cmd.Parameters.Clear();
                            //插入日志详情
                            if (PKID > 0 && model?.LogDetailList?.Count > 0)
                            {
                                var logTmp = model.LogDetailList.Select(item => new
                                {
                                    FPKID = PKID,
                                    OperationLogType = item.OperationLogType,
                                    Property = item.Property,
                                    OldValue = item.OldValue,
                                    NewValue = item.NewValue
                                });
                                DataTable productDT = ToDataTable(logTmp);
                                cmd.CommandText = @"CREATE TABLE #logTmp([FPKID] [int] NOT NULL,
	                                                                     [OperationLogType] [nvarchar](100) NULL,
	                                                                     [Property] [nvarchar](100) NULL,
	                                                                     [OldValue] [nvarchar](1000) NULL,
	                                                                     [NewValue] [nvarchar](1000) NULL);";
                                await cmd.ExecuteNonQueryAsync();
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, tran))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "#logTmp";
                                    bulkcopy.WriteToServer(productDT);
                                    bulkcopy.Close();
                                }
                                string sqlInsert = @"insert into [Activity].[dbo].[SalePromotionActivityLogDetail](
                                                            [FPKID],[OperationLogType],[Property],[OldValue],[NewValue])
                                                    select  [FPKID],[OperationLogType],[Property],[OldValue],[NewValue]
                                                     from   #logTmp";
                                cmd.CommandText = sqlInsert;
                                await cmd.ExecuteNonQueryAsync();
                            }
                            if (result)
                            {
                                tran.Commit();
                            }
                            else
                            {
                                tran.Rollback();
                            }
                        }
                        catch (Exception e)
                        {
                            result = false;
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 检查operationLogType是否已经存在
        /// </summary>
        /// <param name="operationLogType"></param>
        /// <returns></returns>
        public static async Task<bool> CheckLogTypeIsNoRepeatAsync(string operationLogType)
        {
            bool result = false;
            int count;
            string SqlCheckRepeat = @"select count(*)
                                       from  [Activity].[dbo].[SalePromotionLogDescription] with(nolock)
                                      where  OperationLogType=@OperationLogType";
            using (var cmd = new SqlCommand(SqlCheckRepeat))
            {
                cmd.Parameters.AddWithValue("@OperationLogType", operationLogType);
                int.TryParse((await DbHelper.ExecuteScalarAsync(cmd)).ToString(), out count);
                if (count == 0)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 新增活动日志类型描述
        /// </summary>
        /// <param name="model"></param>
        /// <returns>新增的条数</returns>
        public static Task<int> InsertActivityLogDescriptionAsync(SalePromotionActivityLogDescription model)
        {
            string SqlInsert = @"insert into
                    [Activity].[dbo].[SalePromotionLogDescription]([OperationLogType]
                          ,[OperationLogDescription]
                          ,[Remark]
                          ,[CreateDateTime]
                          ,[CreateUserName])
	                 values([OperationLogType]
                          ,[OperationLogDescription]
                          ,[Remark]
                          ,getdate()
                          ,[CreateUserName])";
            using (var cmd = new SqlCommand(SqlInsert))
            {
                cmd.CommandText = SqlInsert;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@OperationLogType", model.OperationLogType);
                cmd.Parameters.AddWithValue("@OperationLogDescription", model.OperationLogDescription);
                cmd.Parameters.AddWithValue("@Remark", model.Remark);
                cmd.Parameters.AddWithValue("@CreateUserName", model.CreateUserName);
                return DbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        private static DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }
    }
}
