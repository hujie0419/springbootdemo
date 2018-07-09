using log4net;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Service.Push;
using Tuhu.C.Job.Models;

namespace Tuhu.C.Job.Job
{
    /// <summary>Sql代理</summary>
    [DisallowConcurrentExecution]
    public class OrderPushMessageJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OrderPushMessageJob));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("启动任务");

            //var testUser = "";
            //using (var cmd = new SqlCommand(@"SELECT  Value TotalDuration,
            //        Description MsgTitle
            //FROM    Gungnir..RuntimeSwitch WITH ( NOLOCK )
            //WHERE   SwitchName = N'OrderPushMessageJob';"))
            //{
            //    var swithcConfig = DbHelper.ExecuteSelect<MessagePushConfig>(cmd)?.FirstOrDefault();
            //    if (swithcConfig?.TotalDuration == null || swithcConfig.TotalDuration == 0)
            //    {
            //        Logger.Info("任务关闭");
            //        return;
            //    }
            //    testUser = swithcConfig.MsgTitle;
            //}

            using (var command = new SqlCommand(@"
SELECT  PKID ,
        EnID,
        MsgTitle,
        MsgContent,
        MsgLink,
        MsgDescription,
        TotalDuration,
        AheadHour
FROM    Configuration..tbl_MessagePush WITH(NOLOCK)
WHERE   EnID = 'pingjiatixing'; "))
            {
                var msgConfig = DbHelper.ExecuteSelect<MessagePushConfig>(command)?.First();

                using (var cmd = new SqlCommand(@"
SELECT TOP 500
        O.UserID ,
        O.OrderNo ,
        UserTel AS Phone ,
        O.PKID AS OrderID ,
        O.UserID Device_Tokens,
        ISNULL(O.OrderProducts, O.OrderNo) AS Products,
        O.OrderDatetime
FROM    Gungnir..tbl_Order AS O WITH ( NOLOCK )
WHERE   Status <> '7Canceled'
        AND CommentStatus = 1
        AND OrderDatetime >= '2016-4-25'
        AND O.OrderChannel != N'r天猫马牌'
        AND NOT EXISTS ( SELECT 1
                         FROM   SystemLog..tbl_OrderPushInfo AS OPI WITH ( NOLOCK )
                         WHERE  OrderID = O.PKID
                                AND MsgType = 'pingjiatixing' )
ORDER BY O.PKID DESC;"))
                {
                    try
                    {
                        var dt = DbHelper.ExecuteSelect<PushInfoModel>(true, cmd)?.ToList();
                        foreach (var d in dt)
                        {
                            var productName = d.Products;
                            if (!string.IsNullOrWhiteSpace(productName))
                            {
                                if (productName.Length > 30)
                                    productName = productName.Substring(0, 20) + "...";
                            }
                            else
                                productName = "";
                            using (var client = new TemplatePushClient())
                            {
                                //var userids = testUser == "all" ? new List<string>() { d.Device_Tokens } : testUser.Split(',').ToList();
                                var userids = new List<string>() {d.Device_Tokens};
                                var result = client.PushByUserIDAndBatchID(
                                    userids,
                                    450,
                                    new Service.Push.Models.Push.PushTemplateLog()
                                    {
                                        Replacement = JsonConvert.SerializeObject(new Dictionary<string, string>
                                        {
                                            ["{{ios.orderid}}"] = d.OrderNo,
                                            ["{{android.orderid}}"] = d.OrderNo,
                                            ["{{messagebox.orderid}}"] = d.OrderNo,
                                            ["{{replace.orderid}}"] = d.OrderNo,
                                            ["{{first.DATA}}"] = $"您还有订单没有评价哦，评价就有机会获得最高100积分奖励，赶紧来吧！",
                                            ["{{keyword1.DATA}}"] = productName,
                                            ["{{keyword2.DATA}}"] = d.OrderDatetime.ToString("yyyy-MM-dd HH:mm"),

                                            ["{{remark.DATA}}"] = $"点击赢取积分奖励",
                                            ["{{wxopen.orderid}}"] = d.OrderNo,
                                        }),
                                        DeviceType = Service.Push.Models.Push.DeviceType.iOS
                                    });
                                if (result?.Result == null || !result.Success || !result.Result)
                                {
                                    Logger.Error($"推送失败{result.ErrorCode};{result.ErrorMessage}", result.Exception);
                                    //continue;
                                }
                                //if (testUser == "all")
                                    InsetPushInfo(d.OrderID, msgConfig.EnID, "All");
                                //else
                                //{
                                //    return;
                                //}
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message, ex);
                    }
                }
            }
            Logger.Info("结束任务");
        }

        public int InsetPushInfo(string orderId, string msgType, string channel)
        {
            using (var cmd = new SqlCommand("INSERT INTO SystemLog..tbl_OrderPushInfo VALUES(@OrderID,@MsgType,GETDATE(),@Channel)"))
            {
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@MsgType", msgType);
                cmd.Parameters.AddWithValue("@Channel", channel);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
        public int InsertMyNews(string userId, string news, string title, string orderId)
        {
            using (var cmd = new SqlCommand(@"MERGE Gungnir..tbl_My_Center_News AS A
                                                USING
                                                    (
                                                        SELECT    @UserID AS UserID,
                                                                @News AS News,
                                                                @Title AS Title,
                                                                @HeadImage AS HeadImage,
                                                                @OrderID AS OrderID
                                                    ) AS B
                                                ON A.UserObjectID = B.UserID
                                                    AND A.News = B.News
                                                    AND A.Title = B.Title
                                                    AND A.OrderID = B.OrderID
                                                    AND A.HeadImage = B.HeadImage
                                                WHEN NOT MATCHED THEN
                                                    INSERT(UserObjectID,
                                                                News,
                                                                Type,
                                                                CreateTime,
                                                                UpdateTime,
                                                                Title,
                                                                HeadImage,
                                                                OrderID,
                                                                isdelete
                                                            )
                                                    VALUES(@UserID,
                                                                @News,
                                                                N'1普通',
                                                                GETDATE(),
                                                                GETDATE(),
                                                                @Title,
                                                                @HeadImage,
                                                                @OrderID,
                                                                0
                                                            ); "))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@News", news);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@HeadImage", "http://image.tuhu.cn/news/pinglun.png");
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
    }
}
