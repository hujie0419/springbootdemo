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
    [DisallowConcurrentExecution]
    class InstallOrderPushJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(InstallOrderPushJob));

        private static readonly int MsgId_YFK = 456;
        private static readonly int MsgId_WFK = 459;

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("启动任务");

            //var testUser = "";
            //using (var cmd = new SqlCommand(@"SELECT Value TotalDuration,
            //        Description MsgTitle
            //FROM    Gungnir..RuntimeSwitch WITH ( NOLOCK )
            //WHERE   SwitchName = N'InstallOrderPushJob';"))
            //{
            //    var swithcConfig = DbHelper.ExecuteSelect<MessagePushConfig>(cmd)?.FirstOrDefault();
            //    if (swithcConfig?.TotalDuration == null || swithcConfig.TotalDuration == 0)
            //    {
            //        Logger.Info("任务关闭");
            //        return;
            //    }
            //    testUser = swithcConfig.MsgTitle;
            //}

            #region 处理1
            using (var cmd = new SqlCommand("SELECT * FROM Configuration.. tbl_MessagePush WITH(NOLOCK) WHERE  EnID in('anzhuangchaoshiyifukuantixing','anzhuangchaoshiweifukuantixing')"))
            {
                var msgConfig = DbHelper.ExecuteSelect<MessagePushConfig>(cmd)?.ToList();

                foreach (var i in msgConfig)
                {
                    #region 数据处理
                    using (var c = new SqlCommand(@"
SELECT  *
FROM    ( SELECT    O.UserID ,
                    O.OrderNo ,
                    UserTel AS Phone ,
                    O.PKID AS OrderID ,
                    O.UserID Device_Tokens,
                    ISNULL(O.OrderProducts, O.OrderNo) AS Products,
                    O.OrderDatetime
          FROM      Gungnir..tbl_Order AS O WITH ( NOLOCK )
                    LEFT JOIN Gungnir.dbo.tbl_OrderDeliveryLog AS LG WITH ( NOLOCK ) ON O.PKID = LG.OrderID
          WHERE     Status <> '7Canceled'
                    AND ( LG.DeliveryStatus = '3Received'
                          OR LG.DeliveryStatus = '3.5Signed'
                        )
                    AND LG.DeliveryDatetime >= @StarTime
                    AND LG.DeliveryDatetime < @EndTime
                    AND PayStatus = @PayStatus
                    AND O.OrderType <> N'6美容'
                    AND O.OrderChannel != N'r天猫马牌'
                    AND InstallType = '1ShopInstall'
                    AND InstallShopID > 0
                    AND InstallStatus <> '2Installed'
                    AND InstallStatus <> '3NoInstall'
                    AND ( O.DeliveryStatus = '3Received'
                          OR O.DeliveryStatus = '3.5Signed'
                        )
        ) AS T
WHERE   NOT EXISTS ( SELECT 1
                     FROM   SystemLog..tbl_OrderPushInfo AS OPI WITH ( NOLOCK )
                     WHERE  OrderID = T.OrderID
                            AND MsgType = @MsgType );"))
                    #endregion
                    {
                        try
                        {
                            var MsgId = 0;
                            c.Parameters.AddWithValue("@StarTime", DateTime.Now.AddHours(-(i.TotalDuration.Value - i.AheadHour.Value)).ToString("yyyy-MM-dd"));
                            c.Parameters.AddWithValue("@EndTime", DateTime.Now.AddHours(-(i.TotalDuration.Value - i.AheadHour.Value)).AddDays(1).ToString("yyyy-MM-dd"));
                            if (i.EnID == "anzhuangchaoshiyifukuantixing")
                            {
                                c.Parameters.AddWithValue("@PayStatus", "2Paid");
                                MsgId = MsgId_YFK;
                            }
                            else
                            {
                                c.Parameters.AddWithValue("@PayStatus", "1Waiting");
                                MsgId = MsgId_WFK;
                            }
                            c.Parameters.AddWithValue("@MsgType", i.EnID);

                            var dt = DbHelper.ExecuteSelect<PushInfoModel>(true, c)?.ToList();
                            foreach (var d in dt)
                            {
                                var productName = d.Products;
                                if (!string.IsNullOrWhiteSpace(productName))
                                {
                                    if (productName.Length > 30)
                                        productName = productName.Substring(0, 20);
                                }
                                else
                                    productName = "";
                                //var userids = testUser == "all" ? new List<string>() { d.Device_Tokens } : testUser.Split(',').ToList();
                                var userids = new List<string>() { d.Device_Tokens };
                                using (var client = new TemplatePushClient())
                                {
                                    var replacementdict = new Dictionary<string, string>
                                    {
                                        ["{{ios.productname}}"] = productName,
                                        ["{{ios.day}}"] = (i.AheadHour / 24)?.ToString(),
                                        ["{{ios.orderid}}"] = d.OrderNo,
                                        ["{{android.productname}}"] = productName,
                                        ["{{android.day}}"] = (i.AheadHour / 24)?.ToString(),
                                        ["{{android.orderid}}"] = d.OrderNo,
                                        ["{{messagebox.productname}}"] = productName,
                                        ["{{messagebox.day}}"] = (i.AheadHour / 24)?.ToString(),
                                        ["{{messagebox.orderid}}"] = d.OrderNo,
                                        ["{{replace.orderid}}"] = d.OrderNo
                                    };
                                    if (MsgId == MsgId_YFK)
                                    {
                                        replacementdict["{{first.DATA}}"] =
                                            $"您预约的【{productName}】未进行安装，再过{(i.AheadHour / 24)?.ToString()}天将会被取消订单，请抓紧去门店享受服务吧！";
                                        replacementdict["{{OrderSn.DATA}}"] = d.OrderNo;
                                        replacementdict["{{OrderStatus.DATA}}"] = "即将到达安装期限";
                                        replacementdict["{{remark.DATA}}"] = "点击查看订单详情";
                                        replacementdict["{{wxopen.orderid}}"] = d.OrderNo;
                                    }
                                    if (MsgId == MsgId_WFK)
                                    {
                                        replacementdict["{{first.DATA}}"] =
                                            $"您预约的{productName}未进行安装，再过{(i.AheadHour / 24)?.ToString()}天将会被取消订单，请抓紧去门店享受服务吧！";
                                        replacementdict["{{OrderSn.DATA}}"] = d.OrderNo;
                                        replacementdict["{{OrderStatus.DATA}}"] = "即将到达安装期限";
                                        replacementdict["{{remark.DATA}}"] = "点击查看订单详情";
                                        replacementdict["{{wxopen.orderid}}"] = d.OrderNo;
                                    }
                                    var result = client.PushByUserIDAndBatchID(
                                        userids,
                                        MsgId,
                                        new Service.Push.Models.Push.PushTemplateLog()
                                        {
                                            Replacement = JsonConvert.SerializeObject(replacementdict),
                                            DeviceType = Service.Push.Models.Push.DeviceType.iOS
                                        });
                                    if (result?.Result == null || !result.Success || !result.Result)
                                    {
                                        Logger.Error($"推送失败{result.ErrorCode};{result.ErrorMessage}", result.Exception);
                                        //continue;
                                    }
                                    //if (testUser == "all")
                                        InsetPushInfo(d.OrderID, i.EnID, "All");
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
            }
            #endregion
            Logger.Info("结束任务");
        }

        #region SQL 操作
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
        #endregion
    }
}
