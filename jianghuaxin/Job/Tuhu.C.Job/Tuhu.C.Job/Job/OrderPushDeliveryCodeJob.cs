using log4net;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Service.Order.Models;
using Tuhu.Service.Push;
using Tuhu.C.Job.Models;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    class OrderPushDeliveryCodeJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OrderPushDeliveryCodeJob));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("启动任务");

            //var testUser = "";
            //using (var cmd = new SqlCommand(@"SELECT  Value TotalDuration,
            //        Description MsgTitle
            //FROM    Gungnir..RuntimeSwitch WITH ( NOLOCK )
            //WHERE   SwitchName = N'OrderPushDeliveryCodeJob';"))
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
            using (var cmd = new SqlCommand("SELECT * FROM Configuration.. tbl_MessagePush WITH(NOLOCK) WHERE  EnID='fahuotixing'"))
            {
                try
                {
                    var msgConfig = DbHelper.ExecuteSelect<MessagePushConfig>(cmd)?.First();

                    #region 数据处理
                    using (var c = new SqlCommand(@"
SELECT  *
FROM    ( SELECT    O.OrderNo ,
                    O.PKID AS OrderID ,
                    O.UserID Device_Tokens,
                    O.UserTel AS Phone ,
                    ISNULL(O.OrderProducts, O.OrderNo) AS Products ,
                    LT.DeliveryCode,
                    O.OrderDatetime
          FROM      Gungnir..tbl_Order AS O WITH ( NOLOCK )
                    INNER JOIN WMSSERVER.WMS.dbo.LogisticTask AS LT WITH ( NOLOCK ) ON O.PKID = LT.OrderId
          WHERE     LT.TaskStatus = '3Sent'
                    AND Status = '2Shipped'
                    AND DeliveryStatus = '2Sent'
                    AND O.DeliveryType <> '4NoDelivery'
                    AND InstallType = '3NoInstall'
                    AND O.OrderType <> N'6美容'
                    AND O.OrderChannel != N'r天猫马牌'
                    AND O.OrderDatetime >= '2016-4-24' 
                    --AND LT.DeliveryCode <> ''
                    --AND LT.DeliveryCode IS NOT NULL
        ) AS T
WHERE   NOT EXISTS ( SELECT 1
                     FROM   SystemLog..tbl_OrderPushInfo AS OPI WITH ( NOLOCK )
                     WHERE  OrderID = T.OrderID
                            AND MsgType = 'fahuotixing' )
ORDER BY T.OrderID DESC; "))
                    {
                        try
                        {
                            var dt = DbHelper.ExecuteSelect<PushInfoModel>(true, c)?.ToList();
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
                                string company = "";
                                var DeliveryInfos = SelectOrderDeliveryInfosByOrderId(Convert.ToInt32(d.OrderID), d.UserID);
                                if (DeliveryInfos != null && DeliveryInfos.Any())
                                {
                                    var m = DeliveryInfos.FirstOrDefault(
                                        x => !string.IsNullOrEmpty(x.DeliveryCode) &&
                                             x.DeliveryCode.Contains(d.DeliveryCode));
                                    if (m != null)
                                    {
                                        company = m.DeliveryCompany;
                                    }
                                }
                                using (var client = new TemplatePushClient())
                                {
                                    // var userids = testUser == "all" ? new List<string>() { d.Device_Tokens } : testUser.Split(',').ToList();
                                    var userids = new List<string>() { d.Device_Tokens };
                                    var replacementdict = new Dictionary<string, string>
                                    {
                                        ["{{ios.productname}}"] = productName,
                                        ["{{ios.trackingno}}"] = d.DeliveryCode,
                                        ["{{ios.orderid}}"] = d.OrderNo,
                                        ["{{android.productname}}"] = productName,
                                        ["{{android.trackingno}}"] = d.DeliveryCode,
                                        ["{{android.orderid}}"] = d.OrderNo,
                                        ["{{messagebox.productname}}"] = productName,
                                        ["{{messagebox.trackingno}}"] = d.DeliveryCode,
                                        ["{{messagebox.orderid}}"] = d.OrderNo,
                                        ["{{replace.orderid}}"] = d.OrderNo,
                                        ["{{first.DATA}}"] = $"亲，您购买的宝贝{productName}已经启程了，好想快点来到你身边",
                                        ["{{keyword1.DATA}}"] = d.OrderNo,
                                        ["{{OrderStatus.DATA}}"] = "订单已发货",
                                        ["{{OrderSn.DATA}}"] = d.OrderNo,
                                        ["{{keyword2.DATA}}"] = company,
                                        ["{{keyword3.DATA}}"] = d.DeliveryCode,
                                        ["{{remark.DATA}}"] = $"点击查看订单详情",
                                        ["{{wxopen.orderid}}"] = d.OrderNo,
                                    };


                                    var result = client.PushByUserIDAndBatchID(
                                        userids,
                                        439,
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
                    #endregion
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                }

            }
            #endregion
            Logger.Info("结束任务");
        }

        public IEnumerable<DeliveryInfo> SelectOrderDeliveryInfosByOrderId(int orderid, string userid)
        {
            Guid temp;
            if (Guid.TryParse(userid, out temp))
            {
                try
                {
                    using (var client = new Tuhu.Service.Order.OrderQueryClient())
                    {
                        var result = client.SelectOrderDeliveryInfosByOrderId(
                            new Service.Order.Request.DeliveryInfoRequest()
                            {
                                OrderId = orderid,
                                UserId = temp
                            });
                        Logger.Info(
                            $"查询快递公司信息.orderid:{orderid}.userid:{userid}.result:{(result.Result == null ? "" : JsonConvert.SerializeObject(result.Result))}");
                        if (result.Success && result.Result != null && result.Result.Item2 != null)
                        {
                            return result.Result.Item2;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Warn($"获取快递信息失败.orderid:{orderid}.userid:{userid}.ex:{ex}");
                }
            }
            return new List<DeliveryInfo>();
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
                                                        SELECT    @UserID AS UserID ,
                                                                @News AS News ,
                                                                @Title AS Title ,
                                                                @HeadImage AS HeadImage ,
                                                                @OrderID AS OrderID
                                                    ) AS B
                                                ON A.UserObjectID = B.UserID
                                                    AND A.News = B.News
                                                    AND A.Title = B.Title
                                                    AND A.OrderID = B.OrderID
                                                    AND A.HeadImage = B.HeadImage
                                                WHEN NOT MATCHED THEN
                                                    INSERT ( UserObjectID ,
                                                                News ,
                                                                Type ,
                                                                CreateTime ,
                                                                UpdateTime ,
                                                                Title ,
                                                                HeadImage ,
                                                                OrderID ,
                                                                isdelete
                                                            )
                                                    VALUES ( @UserID ,
                                                                @News ,
                                                                N'4物流' ,
                                                                GETDATE() ,
                                                                GETDATE() ,
                                                                @Title ,
                                                                @HeadImage ,
                                                                @OrderID ,
                                                                0
                                                            );"))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@News", news);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@HeadImage", "http://image.tuhu.cn/news/wuliu.png");
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }
        #endregion
    }
}
