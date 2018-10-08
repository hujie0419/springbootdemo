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
using System.Threading;

namespace Tuhu.C.Job.CommentStatisticsJob.Job
{
    [DisallowConcurrentExecution]
    public class OrderDataCommentJob : IJob
    {
        private static ILog OrderDataLogger = LogManager.GetLogger<OrderDataCommentJob>();
        public void Execute(IJobExecutionContext context)
        {
            OrderDataLogger.Info($"订单评论数据迁移开始");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                OrderDataLogger.Error($"订单评论数据迁移异常：{ex}");
            }
            OrderDataLogger.Info($"订单评论数据迁移完成");
        }
        public static void DoJob()
        {
            Stopwatch sw = new Stopwatch();
            //var step = 3000;
            //var pos = 0;
            //            var Count = GetOrderCount();
            //            var sql = @"
            //INSERT  INTO Tuhu_comment..OrderCommentStatus
            //        ( OrderId ,
            //          ShopId ,
            //          UserId ,
            //          CanComment ,
            //          HasShopReceive ,
            //          CanReply ,
            //          CreateDateTime
            //        )
            //        SELECT  D.*
            //        FROM    ( SELECT    PKID ,
            //                            InstallShopID ,
            //                            UserID ,
            //                            1 AS tag1 ,
            //                            0 AS tag2 ,
            //                            0 AS tag3 ,
            //                            LastUpdateTime
            //                  FROM      Gungnir..tbl_Order AS O WITH ( NOLOCK )
            //                  WHERE     ( ( O.InstallStatus = '2Installed'
            //                                AND O.InstallShopID > 0
            //                              )
            //                              OR ( ISNULL(O.InstallShopID, 0) = 0
            //                                   AND O.DeliveryStatus IN ( '3.5Signed' )
            //                                 )
            //                            )
            //                            AND O.OrderDatetime < DATEADD(MONTH, -1, GETDATE())
            //                            AND O.OrderDatetime > DATEADD(MONTH, -4, GETDATE())
            //                  ORDER BY  O.PKID DESC
            //                            OFFSET @begin ROWS FETCH NEXT @Count ROWS ONLY
            //                ) AS D
            //        WHERE   NOT EXISTS ( SELECT 1
            //                             FROM   Tuhu_comment..OrderCommentStatus AS T WITH ( NOLOCK )
            //                             WHERE  T.OrderId = D.PKID );
            //";
            //            sw.Start();
            //            var num = 1;
            //            while (pos < Count)
            //            {
            //                using (var cmd = new SqlCommand(sql))
            //                {
            //                    cmd.Parameters.AddWithValue("@begin", pos);
            //                    cmd.Parameters.AddWithValue("@Count", step);
            //                    DbHelper.ExecuteNonQuery(cmd);
            //                }
            //                pos += step;
            //                OrderDataLogger.Info($"订单数据初始化,门店第{num}批数据,共{Count / step + 1}批数据");
            //                num += 1;
            //            }
            //            sw.Stop();
            //            OrderDataLogger.Info($"订单数据初始化step1,插入{Count}条数据,用时{sw.ElapsedMilliseconds}毫秒");
            //            sw.Restart();
            var sql2 = @"SELECT  CommentOrderId AS OrderId ,
                    InstallShopID AS ShopId ,
                    CommentType AS Type ,
                    CommentStatus AS Status ,
                    CreateTime ,
                    CommentId
            FROM    Tuhu_comment..tbl_ShopComment WITH ( NOLOCK )
            WHERE   CommentType IN ( 1, 3, 4 )
                    AND CreateTime > DATEADD(MONTH, -4, GETDATE())
                    AND CreateTime < DATEADD(MONTH, -1, GETDATE())
            ORDER BY CreateTime DESC
                    OFFSET @begin ROWS FETCH NEXT @Count ROWS ONLY;";
            var step = 3000;
            var pos = 0;
            var Count = GetOrderCount();
            var val = new List<ShopCommentDataModel>();
            var num = 1;
            while (pos < Count)
            {
                using (var cmd2 = new SqlCommand(sql2))
                {
                    cmd2.Parameters.AddWithValue("@begin", pos);
                    cmd2.Parameters.AddWithValue("@Count", step);
                    pos += step;
                    val = (DbHelper.ExecuteSelect<ShopCommentDataModel>(true, cmd2)).ToList();
                }
                foreach (var item in val)
                {
                    bool CanComment = true, HasShopReceive = false, CanReply = false;
                    DateTime? tm = null;
                    int? CommentId = null;
                    var sql3 = @"SELECT  OrderId ,
                    CanComment ,
                    HasShopReceive ,
                    CanReply ,
                    CreateShopReceiveTime ,
                    CommentId
            FROM    Tuhu_comment..OrderCommentStatus WITH ( NOLOCK )
            WHERE   OrderId = @orderid;";
                    var sql4 = @"UPDATE  Tuhu_comment..OrderCommentStatus
            SET     CanComment = @CanComment ,
                    HasShopReceive = @HasShopReceive ,
                    CanReply = @CanReply ,
                    LastUpdateDateTime = GETDATE() ,
                    CreateShopReceiveTime = @tm ,
                    CommentId = @CommentId
            WHERE   OrderId = @OrderId;";
                    using (var cmd3 = new SqlCommand(sql3))
                    {
                        cmd3.Parameters.AddWithValue("@orderid", item.OrderId);
                        var val2 = (DbHelper.ExecuteSelect<OrderCommentDataModel>(true, cmd3))?.FirstOrDefault();
                        if (val2 != null)
                        {
                            CanComment = val2.CanComment;
                            CanReply = val2.CanReply;
                            HasShopReceive = val2.HasShopReceive;
                            tm = val2.CreateShopReceiveTime;
                            CommentId = val2.CommentId;
                        }
                        if (item.Type == 1)
                        {
                            CanComment = false;
                            CommentId = item.CommentId;
                        }
                        else if (item.Type == 4)
                        {
                            CanReply = false;
                            HasShopReceive = true;
                        }
                        else if (item.Status == 2)
                        {
                            if (HasShopReceive == true && (tm == null || item.CreateTime < tm))
                            {
                                tm = item.CreateTime;
                            }
                            if (HasShopReceive == false)
                            {
                                HasShopReceive = true;
                                CanReply = true;
                                tm = item.CreateTime;
                            }
                        }

                        using (var cmd4 = new SqlCommand(sql4))
                        {
                            cmd4.Parameters.AddWithValue("@CanComment", CanComment);
                            cmd4.Parameters.AddWithValue("@HasShopReceive", HasShopReceive);
                            cmd4.Parameters.AddWithValue("@CanReply", CanReply);
                            cmd4.Parameters.AddWithValue("@OrderId", item.OrderId);
                            cmd4.Parameters.AddWithValue("@tm", tm);
                            cmd4.Parameters.AddWithValue("@CommentId", CommentId);
                            DbHelper.ExecuteNonQuery(cmd4);
                        }
                    }
                }
                OrderDataLogger.Info($"订单数据初始化第{num}批,共{Count/step + 1}");
            }

            sw.Stop();
            OrderDataLogger.Info($"订单数据初始化step2,用时{sw.ElapsedMilliseconds}毫秒");
            sw.Restart();
            //MoveProductdata();
            //sw.Stop();
            //OrderDataLogger.Info($"订单数据初始化step3,用时{sw.ElapsedMilliseconds}毫秒");
        }

        private static int GetOrderCount()
        {
            const string sql = @" SELECT COUNT(1) FROM Tuhu_comment..OrderCommentStatus WITH(NOLOCK)";
            using (var cmd = new SqlCommand(sql))
            {
                return Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
            }
        }
        private static void MoveProductdata()
        {
            Stopwatch sw = new Stopwatch();
            var step = 3000;
            var pos = 0;
            var Count = 1772021;
//            var sql = @"INSERT  INTO Tuhu_comment..ProductCommentStatus
//        ( OrderId ,
//          ProductId ,
//          CanComment ,
//          CanReply ,
//          CreateDateTime
//        )
//SELECT  *
//FROM    ( SELECT    O.OrderID ,
//                    O.PID ,
//                    1 AS tag1 ,
//                    0 AS tag2 ,
//                    O.CreateDate
//          FROM      Tuhu_comment..OrderCommentStatus AS OC WITH ( NOLOCK ) JOIN 
//		  Gungnir..tbl_OrderList AS O WITH ( NOLOCK ) ON O.OrderID = OC.OrderId
//          WHERE     O.Deleted = 0
//                    AND O.ParentID IS NULL
//                    AND O.PID NOT LIKE 'FU-%'
//                    AND O.Price <> 0
//					AND OC.PKID<@max
//					AND OC.PKID>=@min
//        ) AS T
//WHERE   NOT EXISTS ( SELECT 1
//                     FROM   Tuhu_comment..ProductCommentStatus AS TB
//                     WHERE  TB.ProductId = T.PID COLLATE Chinese_PRC_CI_AS
//                            AND TB.OrderId = T.OrderID );";
            //var tmp = 1;
            //while (pos < Count)
            //{
            //    try
            //    {
            //        using (var cmd = new SqlCommand(sql))
            //        {
            //            cmd.Parameters.AddWithValue("@min", pos);
            //            cmd.Parameters.AddWithValue("@max", pos + step);
            //            DbHelper.ExecuteNonQuery(cmd);
            //        }
            //        OrderDataLogger.Info($"产品数据初始化第{tmp}批，共{Count / step + 1}批");
            //        tmp += 1;
            //    }
            //    catch (Exception ex)
            //    {
            //        OrderDataLogger.Warn($"产品数据初始化失败，为{pos},step为{step}", ex);
            //    }
            //    pos += step;
            //}
            //sw.Stop();
            //OrderDataLogger.Info($"产品数据初始化step1,插入{Count}条数据,用时{sw.ElapsedMilliseconds}毫秒");
            var SQL = @"SELECT  MAX(PKID)
FROM    Tuhu_comment..ProductCommentStatus WITH ( NOLOCK );";
            using (var cmd = new SqlCommand(SQL))
            {
                Count = Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
            }
            sw.Restart();
            pos = 0;
            var sql2 = @"
UPDATE  T
SET     T.CanComment = 0 ,
        T.CanReply = 1 ,
        T.LastUpdateDateTime = GETDATE() ,
        T.CreateCommentTime = GTC.CreateTime ,
        T.CommentId = GTC.CommentId
FROM    Tuhu_comment..ProductCommentStatus AS T with (rowlock)
        LEFT JOIN Gungnir..tbl_Comment AS GTC ON T.ProductId = GTC.CommentProductId COLLATE Chinese_PRC_CI_AS
                                                 AND T.OrderId = GTC.CommentOrderId
WHERE   GTC.ParentComment IS NULL
        AND GTC.CommentId IS NOT NULL
        AND T.PKID > @min
        AND T.PKID <= @max;";

            var sql3 = @"UPDATE  T
SET     T.CanComment = 0 ,
        T.CanReply = 0 ,
        T.LastUpdateDateTime = GETDATE()
FROM    Tuhu_comment..ProductCommentStatus AS T with (rowlock)
WHERE   EXISTS ( SELECT 1
                 FROM   Gungnir..tbl_AdditionComment AS TA with (nolock)
                        JOIN Gungnir..tbl_Comment AS TC  with (nolock) ON TA.CommentId = TC.CommentId
                 WHERE  T.ProductId = TC.CommentProductId COLLATE Chinese_PRC_CI_AS
                        AND T.OrderId = TC.CommentOrderId )
        AND T.PKID > @min
        AND T.PKID <= @max;";
            var num = 1;
            var effectComment = 0;
            var effectAdditionalComment = 0;

            while (pos < Count)
            {
                try
                {
                    using (var cmd = new SqlCommand(sql2))
                    {
                        cmd.Parameters.AddWithValue("@min", pos);
                        cmd.Parameters.AddWithValue("@max", pos + step);
                        effectComment = DbHelper.ExecuteNonQuery(cmd);
                    }
                    Thread.Sleep(50);
                    using (var cmd = new SqlCommand(sql3))
                    {
                        cmd.Parameters.AddWithValue("@min", pos);
                        cmd.Parameters.AddWithValue("@max", pos + step);
                        effectAdditionalComment = DbHelper.ExecuteNonQuery(cmd);
                    }
                    pos += step;
                    OrderDataLogger.Info($"产品数据初始化第{num}批，effectComment:{effectComment},effectAdditionalComment:{effectAdditionalComment}");
                    num += 1;
                }
                catch (Exception ex)
                {
                    OrderDataLogger.Warn($"产品数据更新失败，pos为{pos},step为{step}", ex);
                }
            }
            sw.Stop();
            OrderDataLogger.Info($"产品数据初始化step2,用时{sw.ElapsedMilliseconds}毫秒");
        }
        private static int GetProductCount()
        {
            const string sql = @"
SELECT  COUNT(1)
FROM    Gungnir..tbl_OrderList AS O WITH ( NOLOCK )
WHERE   O.Deleted = 0
        AND O.ParentID IS NULL
        AND EXISTS ( SELECT 1
                     FROM   Tuhu_comment..OrderCommentStatus AS OC WITH ( NOLOCK )
                     WHERE  OC.OrderId = O.OrderId );";
            using (var cmd = new SqlCommand(sql))
            {
                return Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
            }
        }
    }
}
