using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using System.Data.SqlClient;
using Tuhu.C.Job.CommentStatisticsJob.Model;
using System.Diagnostics;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.Service.Comment;

namespace Tuhu.C.Job.CommentStatisticsJob.Job
{
    /// <summary>
    /// 评论统计信息初始化（新功能上线时执行，以后不再执行）
    /// </summary>
    [DisallowConcurrentExecution]
    public class ResetOrderCommentStatusJob:IJob
    {
        private static ILog OrderDataLogger = LogManager.GetLogger<ResetOrderCommentStatusJob>();
        public void Execute(IJobExecutionContext context)
        {
            OrderDataLogger.Info($"订单评论数据重置开始");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                OrderDataLogger.Error($"订单评论数据重置异常：{ex}");
            }
            OrderDataLogger.Info($"订单评论数据重置完成");
        }
        public static void DoJob()
        {
            ResetOrderId();
            Stopwatch sw = new Stopwatch();
            int Count = OrderCount();
            var sql = @"SELECT  OrderId
FROM    Tuhu_comment..OrderCommentStatus WITH ( NOLOCK )
WHERE   CreateDateTime > DATEADD(DAY, -40, '2017-08-04')
ORDER BY CreateDateTime
        OFFSET @begin ROWS FETCH NEXT @step ROWS ONLY;";
            int pos = 0;
            int step = 3000;
            int num = 1;
            while (pos < Count)
            {
                try
                {
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.Parameters.AddWithValue("@begin", pos);
                        cmd.Parameters.AddWithValue("@step", step);
                        var val = DbHelper.ExecuteSelect<OrderStatusModel>(true, cmd);
                        if (val.Any())
                        {
                            var data = val.Select(g => g.OrderId).ToList();
                            Done(data);
                        }
                    }
                    OrderDataLogger.Info($"重置订单数据第{num}批,共{Count / step + 1}批");
                    pos += step;
                }
                catch (Exception ex)
                {
                    OrderDataLogger.Warn("重置订单数据第{num}批出现异常", ex);
                }
            }
            sw.Stop();
            OrderDataLogger.Info($"重置订单数据完成,共{Count}条,用时{sw.ElapsedMilliseconds}毫秒");
        }

        private static int OrderCount()
        {
            string sql = @"SELECT  COUNT(1)
FROM    Tuhu_comment..OrderCommentStatus WITH ( NOLOCK )
WHERE   CreateDateTime > DATEADD(DAY, -40, '2017-08-04');";
            using (var cmd = new SqlCommand(sql))
            {
                return Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
            }
        }
        private static bool ResetOrderId()
        {
            const string sql = @"SELECT  o.PKID AS OrderId
FROM    Gungnir..vw_tbl_Order (NOLOCK) o
        LEFT JOIN Tuhu_comment..OrderCommentStatus (NOLOCK) os ON o.PKID = os.OrderId
WHERE   o.OrderDatetime > GETDATE() - 30
        AND os.OrderId IS NULL
        AND ( ( o.InstallStatus = '2Installed'
                AND o.InstallShopID > 0
              )
              OR ( ISNULL(o.InstallShopID, 0) = 0
                   AND o.DeliveryStatus IN ( '3.5Signed' )
                 )
            )";
            using(var cmd=new SqlCommand(sql))
            {
                var val = DbHelper.ExecuteSelect<OrderStatusModel>(true, cmd);
                if(val.Any())
                {
                    var data = val.Select(g => g.OrderId).ToList();
                    try
                    {
                        Done(data);
                        OrderDataLogger.Info($"异常订单数据重置成功,共{data.Count}条数据");
                    }
                    catch(Exception ex)
                    {
                        OrderDataLogger.Warn($"异常订单数据重置失败", ex);
                        return false;
                    }
                }
            }
            return true;
        }

        private static void Done(List<int> data)
        {
            if(data.Any())
            {
                var num = 1;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                foreach (var item in data)
                {                  
                    using (var client = new OrderQueryClient())
                    {
                        var dat = client.GetRelatedSplitOrderIDs(item, SplitQueryType.Full);
                        if (dat.Success && dat.Result.Count() == 1)
                        {
                            using (var client2 = new ShopCommentClient())
                            {
                                var result = client2.SetShopCommentStatus(item);
                                if (result.Success==false || result.Result == false)
                                {
                                    OrderDataLogger.Warn($"重置订单号为{item}的数据失败");
                                }
                            }
                        }
                    }
                    if (num % 20 == 0)
                    {
                        sw.Stop();
                        OrderDataLogger.Info($"重置异常订单第{num - 20}到{num}条,共{data.Count},用时{sw.ElapsedMilliseconds}");
                        sw.Reset();
                        sw.Start();

                    }
                    num += 1;
                }
            }
        }
    }
}
