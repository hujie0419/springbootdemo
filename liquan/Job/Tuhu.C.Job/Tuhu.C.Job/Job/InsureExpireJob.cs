using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models;
using Tuhu.C.Job.Models;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class InsureExpireJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(InsureExpireJob));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("启动任务");
            List<InsureCarModel> Cars = null;
            using (var cmd = new SqlCommand(@"DECLARE @Date DATE= GETDATE();
                                                SELECT  DATEDIFF(DAY , GETDATE() , C_u_InsureExpireDate) AS ExpireDay ,
                                                        CO.UserID ,
                                                        CO.C_u_InsureExpireDate ,
                                                        CO.u_carno ,
                                                        UO.u_mobile_number
                                                FROM    Tuhu_profiles..CarObject AS CO ( NOLOCK )
                                                JOIN    Tuhu_profiles..UserObject AS UO ( NOLOCK )
                                                        ON CO.UserID = UO.UserID
                                                WHERE   C_u_InsureExpireDate IS NOT NULL
                                                        AND CO.C_u_InsureExpireDate >= DATEADD(MONTH , 2 , @Date)
                                                        AND CO.C_u_InsureExpireDate < DATEADD(DAY , 1 ,
                                                                                                DATEADD(MONTH , 2 , @Date)) "))
            {
                Cars = DbHelper.ExecuteSelect<InsureCarModel>(true, cmd).ToList();
                    foreach (var i in Cars)
                    {
                        //插入通知消息
                        using (var pushClient = new PushClient())
                        {
                            var pushMessage = new PushMessageModel()
                            {
                                Type = MessageType.AppNotification,
                                CenterMsgType = "1普通",
                                PhoneNumbers = new List<string> { i.u_mobile_number },
                                Content = "您的爱车" + i.u_carno + "保险还有" + i.ExpireDay + "天就要到期了哦，请及时购买新的保险；不然爱车就要开启裸奔模式了哦",
                                HeadImageUrl = "http://image.tuhu.cn/news/wuliu.png",
                                InsertAppCenterMsg = true,
                                Title = "爱车保险快到期啦",
                                SourceName = "Job.tuhu.cn"
                            };

                            pushMessage.AndriodModel = new AndriodModel
                            {
                                AfterOpen = AfterOpenEnum.GoActivity,
                                AppActivity = "cn.TuHu.Activity.LoveCar.MyLoveCarActivity",
                            };

                            pushMessage.IOSModel = new IOSModel
                            {
                                ExKey1 = "appoperateval",
                                ExValue1 = "Tuhu.THCarDetailsVC",
                                ExKey2 = "keyvaluelenth",
                                ExValue2 = "{\"orderNumber\":\"\"}"
                            };
                            var a = pushClient.PushMessages(pushMessage);
                        }
                    }
            }
            using (var cmd = new SqlCommand(@"DECLARE @Date DATE=GETDATE()
                                                SELECT  DATEDIFF(DAY , GETDATE() , C_u_InsureExpireDate) AS ExpireDay ,
                                                        CO.UserID ,
                                                        CO.C_u_InsureExpireDate ,
                                                        CO.u_carno ,
                                                        UO.u_mobile_number
                                                FROM    Tuhu_profiles..CarObject AS CO ( NOLOCK )
                                                JOIN    Tuhu_profiles..UserObject AS UO ( NOLOCK )
                                                        ON CO.UserID = UO.UserID
                                                WHERE   C_u_InsureExpireDate IS NOT NULL
                                                        AND CO.C_u_InsureExpireDate >= @Date
                                                        AND CO.C_u_InsureExpireDate < DATEADD(DAY , 1 , @Date)"))
            {
                Cars = DbHelper.ExecuteSelect<InsureCarModel>(true, cmd).ToList();
                if (Cars != null)
                    foreach (var i in Cars)
                    {
                        //插入通知消息
                        using (var pushClient = new PushClient())
                        {
                            var pushMessage = new PushMessageModel()
                            {
                                Type = MessageType.AppNotification,
                                CenterMsgType = "1普通",
                                PhoneNumbers = new List<string> { i.u_mobile_number },
                                Content = "您的爱车" + i.u_carno + "保险已过期，目前处于裸奔模式，请及时购买新的保险！",
                                HeadImageUrl = "http://image.tuhu.cn/news/wuliu.png",
                                InsertAppCenterMsg = true,
                                Title = "爱车保险已过期，请及时续保",
                                SourceName = "Job.tuhu.cn"
                            };

                            pushMessage.AndriodModel = new AndriodModel
                            {
                                AfterOpen = AfterOpenEnum.GoActivity,
                                AppActivity = "cn.TuHu.Activity.LoveCar.MyLoveCarActivity",
                            };

                            pushMessage.IOSModel = new IOSModel
                            {
                                ExKey1 = "appoperateval",
                                ExValue1 = "Tuhu.THCarDetailsVC",
                                ExKey2 = "keyvaluelenth",
                                ExValue2 = "{\"orderNumber\":\"\"}"
                            };
                            var a = pushClient.PushMessages(pushMessage);
                        }
                    }
            }
            //这个是每周六提醒块到期的
            if (DateTime.Now.Date.DayOfWeek == DayOfWeek.Saturday)
            {
                using (var cmd = new SqlCommand(@"DECLARE @Date DATE= GETDATE();
                                                    SELECT  DATEDIFF(DAY , GETDATE() , C_u_InsureExpireDate) AS ExpireDay ,
                                                            CO.UserID ,
                                                            CO.C_u_InsureExpireDate ,
                                                            CO.u_carno ,
                                                            UO.u_mobile_number
                                                    FROM    Tuhu_profiles..CarObject AS CO ( NOLOCK )
                                                    JOIN    Tuhu_profiles..UserObject AS UO ( NOLOCK )
                                                            ON CO.UserID = UO.UserID
                                                    WHERE   C_u_InsureExpireDate IS NOT NULL
                                                            AND C_u_InsureExpireDate >= @Date
                                                            AND C_u_InsureExpireDate < DATEADD(MONTH , 2 , GETDATE());"))
                {
                    Cars = DbHelper.ExecuteSelect<InsureCarModel>(true, cmd).ToList();
                }
                if (Cars != null)
                    foreach (var i in Cars)
                    {
                        //插入通知消息
                        using (var pushClient = new PushClient())
                        {
                            var pushMessage = new PushMessageModel()
                            {
                                Type = MessageType.AppNotification,
                                CenterMsgType = "1普通",
                                PhoneNumbers = new List<string> { i.u_mobile_number },
                                Content = "您的爱车" + i.u_carno + "保险还有" + i.ExpireDay + "天就要到期了哦，请及时购买新的保险；不然爱车就要开启裸奔模式了哦",
                                HeadImageUrl = "http://image.tuhu.cn/news/wuliu.png",
                                InsertAppCenterMsg = true,
                                Title = "爱车保险快到期啦",
                                SourceName = "Job.tuhu.cn"
                            };

                            pushMessage.AndriodModel = new AndriodModel
                            {
                                AfterOpen = AfterOpenEnum.GoActivity,
                                AppActivity = "cn.TuHu.Activity.LoveCar.MyLoveCarActivity",
                            };

                            pushMessage.IOSModel = new IOSModel
                            {
                                ExKey1 = "appoperateval",
                                ExValue1 = "Tuhu.THCarDetailsVC",
                                ExKey2 = "keyvaluelenth",
                                ExValue2 = "{\"orderNumber\":\"\"}"
                            };
                            var a = pushClient.PushMessages(pushMessage);
                        }
                    }
                Logger.Info("结束任务");
            }
        }
    }
}
