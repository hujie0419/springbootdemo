//using log4net;
//using Newtonsoft.Json;
//using Quartz;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Text;
//using Tuhu.Service.Order;
//using Tuhu.Service.Order.Enum;
//using Tuhu.Service.Order.Models;
//using Tuhu.Service.Push;
//using Tuhu.C.Job.Models;


//namespace Tuhu.C.Job.Job
//{
//    [DisallowConcurrentExecution]
//    public class OrderPushMessageInstallJob : IJob
//    {
//        private static readonly ILog Logger = LogManager.GetLogger(typeof(OrderPushMessageInstallJob));

//        private static string IsSendLTX = ConfigurationManager.AppSettings["OrderPushMessageLTX"];

//        private static readonly int MsgId_AnZhuang = 443;
//        private static readonly int MsgId_AnZhuang_No = 447;
//        private static readonly int MsgId_LTX_AnZhuang = 481;
//        private static readonly int MsgId_LTX_AnZhuang_No = 484;

//        public void Execute(IJobExecutionContext context)
//        {
//            Logger.Info("启动任务");

//            //var testUser = "";
//            //using (var cmd = new SqlCommand(@"SELECT  Value TotalDuration,
//            //        Description MsgTitle
//            //FROM    Gungnir..RuntimeSwitch WITH ( NOLOCK )
//            //WHERE   SwitchName = N'OrderPushMessageInstallJob';"))
//            //{
//            //    var swithcConfig = DbHelper.ExecuteSelect<MessagePushConfig>(cmd)?.FirstOrDefault();
//            //    if (swithcConfig?.TotalDuration == null || swithcConfig.TotalDuration == 0)
//            //    {
//            //        Logger.Info("任务关闭");
//            //        return;
//            //    }
//            //    testUser = swithcConfig.MsgTitle;
//            //}

//            #region 处理1
//            using (var cmd = new SqlCommand(@"
//SELECT  PKID ,
//        EnID,
//        MsgTitle,
//        MsgContent,
//        MsgLink,
//        MsgDescription,
//        TotalDuration,
//        AheadHour 
//FROM Configuration.. tbl_MessagePush WITH(NOLOCK) WHERE  EnID in('daodiananzhuangtixing','daodianbuanzhuangtixing','servicetixing')"))
//            {
//                var msgConfig = DbHelper.ExecuteSelect<MessagePushConfig>(cmd)?.ToList();

//                foreach (var i in msgConfig.Where(p => p.EnID != "servicetixing"))
//                {
//                    #region 数据处理
//                    var sql = @"
//SELECT  O.UserID ,
//        O.OrderNo ,
//        UserTel AS Phone ,
//        O.PKID AS OrderID ,
//        O.UserID Device_Tokens,
//        ISNULL(O.OrderProducts, O.OrderNo) AS Products ,
//        S.Address ,
//        S.Telephone ,
//        S.CarparName ,
//        OSC.ServiceCode AS InstallCode,
//        O.OrderDatetime
//FROM    Gungnir..tbl_Order AS O WITH ( NOLOCK )
//        LEFT JOIN Gungnir..Shops AS S WITH ( NOLOCK ) ON S.PKID = O.InstallShopID
//        LEFT JOIN Gungnir..OrderServiceCode AS OSC WITH ( NOLOCK ) ON OSC.OrderId = O.PKID
//                                                              AND OSC.Type = N'新快修超人服务码'
//                                                              AND OSC.Deleted = 0
//WHERE   InstallType = @InstallType
//        AND O.InstallShopID > 0
//        AND O.Status = '2Shipped'
//        AND ( DeliveryStatus = '3Received'
//              OR DeliveryStatus = '3.5Signed'
//            )
//        AND ( InstallStatus IS NULL
//              OR ( InstallStatus <> '2Installed'
//                   AND ( @InstallStatus IS NULL
//                         OR InstallStatus <> '3NoInstall'
//                       )
//                 )
//            )
//        AND ( ( O.InstallShopID != O.WareHouseID
//                OR O.InstallShopID = 0
//                OR O.WareHouseID = 0
//              )
//              OR DeliveryStatus = N'3.5signed'
//            )
//        AND OrderDatetime >= '2016-5-10'
//        AND NOT EXISTS ( SELECT 1
//                         FROM   SystemLog..tbl_OrderPushInfo AS OPI WITH ( NOLOCK )
//                         WHERE  OrderID = O.PKID
//                                AND MsgType = @MsgType )
//ORDER BY O.PKID DESC;";

//                    #endregion

//                    using (var c = new SqlCommand(sql))
//                    {
//                        try
//                        {
//                            var MsgId = 0;
//                            #region daodiananzhuangtixing
//                            if (i.EnID == "daodiananzhuangtixing")
//                            {
//                                c.Parameters.AddWithValue("@InstallType", "1ShopInstall");
//                                c.Parameters.AddWithValue("@InstallStatus", "3NoInstall");
//                                c.Parameters.AddWithValue("@MsgType", "daodiananzhuangtixing");
//                                MsgId = MsgId_AnZhuang;
//                            }

//                            #endregion
//                            #region daodianbuanzhuangtixing
//                            if (i.EnID == "daodianbuanzhuangtixing")
//                            {
//                                c.Parameters.AddWithValue("@InstallType", "3NoInstall");
//                                c.Parameters.AddWithValue("@InstallStatus", DBNull.Value);
//                                c.Parameters.AddWithValue("@MsgType", "daodianbuanzhuangtixing");
//                                MsgId = MsgId_AnZhuang_No;
//                            }
//                            #endregion

//                            var dt = DbHelper.ExecuteSelect<PushInfoModel>(true, c)?.ToList();

//                            //对多设备进行处理，一个订单只写一条消息
//                            using (var client = new PushClient())
//                            using (var orderClinet = new OrderQueryForAppClient())
//                            {
//                                for (var index = 0; index < dt.Count; index++)
//                                {
//                                    var pushKey = MsgId;
//                                    var d = dt[index];

//                                    var productName = d.Products;
//                                    if (!string.IsNullOrWhiteSpace(productName))
//                                    {
//                                        if (productName.Length > 30)
//                                            productName = productName.Substring(0, 20);
//                                    }
//                                    else
//                                        productName = "";

//                                    var msg = msgConfig.Find(p => p.EnID == i.EnID);
//                                    var msgContent = "";
//                                    var replace = new Dictionary<string, string>
//                                    {
//                                        ["{{ios.productname}}"] = productName,
//                                        ["{{ios.orderid}}"] = d.OrderNo,
//                                        ["{{android.productname}}"] = productName,
//                                        ["{{android.orderid}}"] = d.OrderNo,
//                                        ["{{messagebox.productname}}"] = productName,
//                                        ["{{messagebox.orderid}}"] = d.OrderNo,
//                                        ["{{replace.orderid}}"] = d.OrderNo
//                                    };
//                                    try
//                                    {
//                                        #region LTX
//                                        var request = new OrderRequest { Channel = RequestChannel.App }; //渠道
//                                        request.Key = GenerateKey(request.Channel); //秘钥
//                                        Guid userId;
//                                        var orderId = 0;
//                                        var errorMessage = string.Empty;
//                                        var senMsgTitle = msg.MsgTitle;
//                                        var senMsgContent = msgContent;
//                                        if (Guid.TryParse(d.UserID, out userId) && int.TryParse(d.OrderID, out orderId))
//                                        {
//                                            request.UserId = userId; //用户UserID
//                                            request.OrderId = orderId;
//                                            var resultOrder = orderClinet.CheckOrderHasLunTaiXian(request);
//                                            if (resultOrder.Success)
//                                            {
//                                                if (!resultOrder.Result.IsSuccess)
//                                                {
//                                                    errorMessage = resultOrder.Result.ErrorMessage;
//                                                }
//                                            }
//                                            if (!resultOrder.Success)
//                                            {
//                                                errorMessage = "调用服务失败";
//                                                Logger.Error($"消息推送失败=>放弃轮胎险推送逻辑=>" +
//                                                             $"orderid:{d.OrderID};" +
//                                                             $"msg:{resultOrder.Result.ErrorMessage}");
//                                            }

//                                            if (string.IsNullOrEmpty(errorMessage)) //说明订单支持补轮胎
//                                            {
//                                                if (MsgId == MsgId_AnZhuang_No)
//                                                {
//                                                    pushKey = MsgId_LTX_AnZhuang_No;
//                                                }
//                                                if (MsgId == MsgId_AnZhuang)
//                                                {
//                                                    pushKey = MsgId_LTX_AnZhuang;
//                                                }
//                                                if (replace.ContainsKey("{{replace.orderid}}"))
//                                                    replace.Remove("{{replace.orderid}}");
//                                            }
//                                        }
//                                        #endregion
//                                    }
//                                    catch (Exception ex)
//                                    {
//                                        Logger.Error($"消息推送失败=>放弃轮胎险推送逻辑=>" + ex.Message, ex);
//                                    }
//                                    if (pushKey == 443)
//                                    {
//                                        replace["{{first.DATA}}"] = $"您买的{productName}到门店了，赶快去门店享受服务吧！";
//                                        replace["{{OrderSn.DATA}}"] = d.OrderNo;
//                                        replace["{{OrderStatus.DATA}}"] = "到店待安装";
//                                        replace["{{remark.DATA}}"] = "点击查看订单详情";
//                                        replace["{{wxopen.orderid}}"] = d.OrderNo;
//                                    }
//                                    else if (pushKey == 447)
//                                    {
//                                        replace["{{first.DATA}}"] = $"您买的{productName}到门店了，赶快到门店取货吧！";
//                                        replace["{{OrderSn.DATA}}"] = d.OrderNo;
//                                        replace["{{OrderStatus.DATA}}"] = "到店待取货";
//                                        replace["{{remark.DATA}}"] = "点击查看订单详情";
//                                        replace["{{wxopen.orderid}}"] = d.OrderNo;
//                                    }
//                                    else if (pushKey == 481)
//                                    {
//                                        replace["{{first.DATA}}"] = $"您买的{productName}到门店了，赶快去门店享受服务吧！";
//                                        replace["{{OrderSn.DATA}}"] = d.OrderNo;
//                                        replace["{{OrderStatus.DATA}}"] = "到店待安装";
//                                        replace["{{remark.DATA}}"] = "点击查看订单详情";
//                                        replace["{{wxopen.orderid}}"] = d.OrderNo;
//                                    }
//                                    else if (pushKey == 484)
//                                    {
//                                        replace["{{first.DATA}}"] = $"您买的{productName}到门店了，赶快到门店取货吧！";
//                                        replace["{{OrderSn.DATA}}"] = d.OrderNo;
//                                        replace["{{OrderStatus.DATA}}"] = "到店待取货";
//                                        replace["{{remark.DATA}}"] = "点击查看订单详情";
//                                        replace["{{wxopen.orderid}}"] = d.OrderNo;
//                                    }
//                                    using (var pushClient = new TemplatePushClient())
//                                    {
//                                        //var userids = testUser == "all" ? new List<string>() { d.Device_Tokens } : testUser.Split(',').ToList();
//                                        var userids = new List<string>() { d.Device_Tokens };
//                                        var result = pushClient.PushByUserIDAndBatchID(
//                                            userids,
//                                            pushKey,
//                                            new Service.Push.Models.Push.PushTemplateLog()
//                                            {
//                                                Replacement = JsonConvert.SerializeObject(replace),
//                                                DeviceType = Service.Push.Models.Push.DeviceType.iOS
//                                            });
//                                        if (result?.Result == null || !result.Success || !result.Result)
//                                        {
//                                            Logger.Error($"推送失败{result.ErrorCode};{result.ErrorMessage}", result.Exception);
//                                            //continue;
//                                        }
//                                        //if (testUser == "all")
//                                        //{
//                                        InsetPushInfo(d.OrderID, i.EnID, "All");
//                                        //}
//                                        //else
//                                        //{
//                                        //    return;
//                                        //}
//                                    }
//                                }
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Logger.Error(ex.Message, ex);
//                        }
//                    }
//                }
//            }
//            #endregion
//            Logger.Info("结束任务");
//        }

//        private static string GenerateKey(RequestChannel channel)
//        {
//            var time = Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.ToString("yyyyMMdd")));
//            return string.Concat(time, MD5Helper.GetMd5(channel.ToString() + time, Encoding.UTF8));
//        }

//        public int InsetPushInfo(string orderId, string msgType, string channel)
//        {
//            if (string.IsNullOrWhiteSpace(channel))
//                channel = "未知手机号码渠道";
//            using (var cmd = new SqlCommand("INSERT INTO SystemLog..tbl_OrderPushInfo VALUES(@OrderID,@MsgType,GETDATE(),@Channel)"))
//            {
//                cmd.Parameters.AddWithValue("@OrderID", orderId);
//                cmd.Parameters.AddWithValue("@MsgType", msgType);
//                cmd.Parameters.AddWithValue("@Channel", channel);
//                return DbHelper.ExecuteNonQuery(cmd);
//            }
//        }
//        public int InsertMyNews(string userId, string news, string title, string orderId)
//        {
//            using (var cmd = new SqlCommand(@"MERGE Gungnir..tbl_My_Center_News AS A
//                                                USING
//                                                    (
//                                                        SELECT    @UserID AS UserID,
//                                                                @News AS News,
//                                                                @Title AS Title,
//                                                                @HeadImage AS HeadImage,
//                                                                @OrderID AS OrderID
//                                                    ) AS B
//                                                ON A.UserObjectID = B.UserID
//                                                    AND A.News = B.News
//                                                    AND A.Title = B.Title
//                                                    AND A.OrderID = B.OrderID
//                                                    AND A.HeadImage = B.HeadImage
//                                                WHEN NOT MATCHED THEN
//                                                    INSERT(UserObjectID,
//                                                                News,
//                                                                Type,
//                                                                CreateTime,
//                                                                UpdateTime,
//                                                                Title,
//                                                                HeadImage,
//                                                                OrderID,
//                                                                isdelete
//                                                            )
//                                                    VALUES(@UserID,
//                                                                @News,
//                                                                N'4物流',
//                                                                GETDATE(),
//                                                                GETDATE(),
//                                                                @Title,
//                                                                @HeadImage,
//                                                                @OrderID,
//                                                                0
//                                                            ); "))
//            {
//                cmd.Parameters.AddWithValue("@UserID", userId);
//                cmd.Parameters.AddWithValue("@News", news);
//                cmd.Parameters.AddWithValue("@Title", title);
//                cmd.Parameters.AddWithValue("@HeadImage", "http://image.tuhu.cn/news/wuliu.png");
//                cmd.Parameters.AddWithValue("@OrderID", orderId);
//                return DbHelper.ExecuteNonQuery(cmd);
//            }
//        }
//    }
//}
