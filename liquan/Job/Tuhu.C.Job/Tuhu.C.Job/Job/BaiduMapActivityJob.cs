using Common.Logging;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Models;

namespace Tuhu.C.Job.Job
{


    /// <summary>
    /// 百度地图车主节活动回调job
    /// </summary>
    [DisallowConcurrentExecution]
    public class BaiduMapActivityJob : BaseJob
    {
        public BaiduMapActivityJob()
        {
            logger = LogManager.GetLogger(typeof(BaiduMapActivityJob));
        }

        protected override string JobName
        {
            get { return nameof(BaiduMapActivityJob); }
        }

        public override void Exec()
        {
            logger.Info("开始执行百度地图车主节活动回调job");

            try
            {
                var bags = GetUnNotifiedBMapCodes();
                logger.Info($"共{bags?.Count}条需要回调");
                foreach (var bag in bags)
                {
                    try
                    {
                        if (bag != null && !string.IsNullOrEmpty(bag.Portrait) && !string.IsNullOrEmpty(bag.Mobile) &&
                            !string.IsNullOrEmpty(bag.PromotionCode))
                        {
                            var order = FetchOrderByOrderId(bag.TuhuOrderId);
                            if (order != null && String.Equals(order.PayStatus, "2Paid", StringComparison.CurrentCultureIgnoreCase))
                            {
                                bool notifyResult = NotifyBaidu(bag);

                                bool tuhuUpdateResult = UpdateCodeStatusToNotified(bag.Pkid, notifyResult ? 1 : 2);
                            }
                            else
                            {
                                logger.Info($"车主节活动订单不存在或不是已支付状态 Order:{order?.PayStatus ?? string.Empty},PromotionCode:{bag.PromotionCode}");
                            }
                        }
                    }
                    catch (Exception innEx)
                    {
                        logger.Error($"百度地图车主节活动回调job发生错误, bag:{bag?.PromotionCode}", innEx);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("百度地图车主节活动回调job发生错误", ex);
            }

            logger.Info("百度地图车主节活动回调job执行结束");
        }

        public OrderModel FetchOrderByOrderId(int orderId)
        {
            OrderModel order = null;
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = orderClient.FetchOrderByOrderId(orderId);
                    fetchResult.ThrowIfException(true);
                    order = fetchResult.Success ? fetchResult.Result : null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("查询订单信息接口异常", ex);
            }
            return order;
        }

        private List<BaiduMapRedBag> GetUnNotifiedBMapCodes()
        {
            List<BaiduMapRedBag> result = new List<BaiduMapRedBag>();

            using (var cmd = new SqlCommand())
            {
                cmd.CommandText = @"SELECT  ba.Pkid ,
                                            p.OrderId AS TuhuOrderId ,
                                            ba.DecryptMoblile AS Mobile ,
                                            ba.DecryptPortrait AS Portrait ,
                                            ba.PromotionCode
                                    FROM    Gungnir..tbl_PromotionCode AS p WITH ( NOLOCK )
                                            JOIN Activity..tbl_BaiduMapRedBag AS ba WITH ( NOLOCK ) ON ba.PromotionCode = p.PKID
                                    WHERE   p.UsedTime > DATEADD(HOUR, -12, GETDATE())
                                            AND p.Status > 0
                                            AND p.OrderId IS NOT NULL
                                            AND p.OrderId > 0
                                            AND ba.Staus != 1;";
                cmd.CommandType = CommandType.Text;

                result = DbHelper.ExecuteSelect<BaiduMapRedBag>(true, cmd).ToList();
            }

            return result;
        }

        //private bool IsUsed(string code)
        //{
        //    int status = 0;
        //    using (var cmd = new SqlCommand())
        //    {
        //        cmd.CommandText = @"SELECT Status FROM Gungnir..tbl_PromotionCode WITH(NOLOCK) WHERE Code = @Code";
        //        cmd.CommandType = CommandType.Text;
        //        cmd.Parameters.AddWithValue("@Code", code);

        //        status = (int)DbHelper.ExecuteScalar(true, cmd);
        //    }

        //    return status != 0;
        //}

        private bool NotifyBaidu(BaiduMapRedBag bag)
        {
            const string url = "http://open.api.tuhu.cn/verify/BaiduHongbaoCallback";
            bool result = false;

            try
            {
                string requestUrl = $"{url}?phone={bag.Mobile}&portrait={bag.Portrait}&code={bag.PromotionCode}";
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestUrl);
                req.Timeout = 5000;
                HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
                Stream myStream = rsp.GetResponseStream();
                StreamReader sr = new StreamReader(myStream, Encoding.Default);
                StringBuilder strBuilder = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine());
                }

                var strResult = strBuilder.ToString();
                var jobject = JObject.Parse(strResult);
                var code = jobject.GetValue("errno");
                result = code.Value<int>() == 0;

                if (!result)
                {
                    logger.Info($"调用百度地图核销接口返回核销失败, request:{requestUrl}, errormsg:{jobject.GetValue("message")?.Value<string>()}");
                }
            }
            catch (Exception ex)
            {
                logger.Error("通知百度核销接口失败", ex);
            }

            return result;
        }

        public bool UpdateCodeStatusToNotified(int pkid, int status)
        {
            int result = 0;
            using (var cmd = new SqlCommand())
            {
                cmd.CommandText = @"UPDATE Activity..tbl_BaiduMapRedBag SET Staus = @Status, UpdateTime = GetDate() WHERE Pkid = @PKID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", pkid);
                cmd.Parameters.AddWithValue("@Status", status);
                result = DbHelper.ExecuteNonQuery(cmd);
            }
            return result > 0;
        }

        private class BaiduMapRedBag
        {
            public int Pkid { get; set; }

            public int TuhuOrderId { get; set; }

            public string Mobile { get; set; }

            public string Portrait { get; set; }

            public string PromotionCode { get; set; }
        }
    }
}
