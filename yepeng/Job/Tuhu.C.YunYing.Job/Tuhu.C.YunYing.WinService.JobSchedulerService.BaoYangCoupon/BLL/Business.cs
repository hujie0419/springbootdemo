using Common.Logging;
using System;
using System.Configuration;
using Tuhu.Service.Utility;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.BLL
{
    public static class Business
    {
        private static readonly int PromotionType = Convert.ToInt32(ConfigurationManager.AppSettings["BaoYangCoupon:PromotionType"]);
        private static readonly string CouponDescriptionOfTypeOne = ConfigurationManager.AppSettings["BaoYangCoupon:CouponDescriptionOfTypeOne"];
        private static readonly string CouponDescriptionOfTypeTwo = ConfigurationManager.AppSettings["BaoYangCoupon:CouponDescriptionOfTypeTwo"];
        private static readonly string CouponFreeOrderDescription = ConfigurationManager.AppSettings["BaoYangCoupon:FreeOrderCouponDescription"];
        private static readonly string SMSCouponFreeOrderSubject = ConfigurationManager.AppSettings["BaoYangCoupon:FreeOrderCouponSMSubject"];
        private static readonly string SMSCouponFreeOrderDetail = ConfigurationManager.AppSettings["BaoYangCoupon:FreeOrderCouponSMSMessage"];

        private static readonly int PromotionruleId = Convert.ToInt32(ConfigurationManager.AppSettings["BaoYangCoupon:PromotionruleId"]);
        private static readonly string BaoYangMessageSubject = ConfigurationManager.AppSettings["BaoYangCoupon:BaoYangMessageSubject"];
        private static readonly string BaoyangMessageDetail = ConfigurationManager.AppSettings["BaoYangCoupon:BaoYangMessageDetail"];
        private static readonly string NonBaoYangMessageSubject = ConfigurationManager.AppSettings["BaoYangCoupon:NonBaoYangMessageSubject"];
        private static readonly string NonBaoyangMessageDetail = ConfigurationManager.AppSettings["BaoYangCoupon:NonBaoYangMessageDetail"];
        private static readonly int DiscountOfTypeOne = Convert.ToInt32(ConfigurationManager.AppSettings["BaoYangCoupon:discountOfTypeOne"]);
        private static readonly int DiscountOfTypeTwo = Convert.ToInt32(ConfigurationManager.AppSettings["BaoYangCoupon:discountOfTypeTwo"]);
        private static readonly int MinMoneyOfTypeOne = Convert.ToInt32(ConfigurationManager.AppSettings["BaoYangCoupon:minMoneyOfTypeOne"]);
        private static readonly int MinMoneyOfTypeTwo = Convert.ToInt32(ConfigurationManager.AppSettings["BaoYangCoupon:minMoneyOfTypeTwo"]);
        private static readonly string CodeChannel = ConfigurationManager.AppSettings["BaoYangCoupon:codeChannel"];

        private static readonly string BaoYangAndroidPushTitle = ConfigurationManager.AppSettings["BaoYangCoupon:BaoYangAndroidPushTitle"];
        private static readonly string BaoYangAndroidPushText = ConfigurationManager.AppSettings["BaoYangCoupon:BaoYangAndroidPushText"];
        private static readonly string NonBaoYangAndroidPushTitle = ConfigurationManager.AppSettings["BaoYangCoupon:NonBaoYangAndroidPushTitle"];
        private static readonly string NonBaoYangAndroidPushText = ConfigurationManager.AppSettings["BaoYangCoupon:NonBaoYangAndroidPushText"];
        private static readonly string BaoYangIosPushMessage = ConfigurationManager.AppSettings["BaoYangCoupon:BaoYangIosPushMessage"];
        private static readonly string NonBaoYangIosPushMessage = ConfigurationManager.AppSettings["BaoYangCoupon:NonBaoYangIosPushMessage"];

        private static readonly ILog BaoYangUserCouponJobLog = LogManager.GetLogger(typeof(BaoYangUserCouponJob));
        private static readonly ILog NonBaoYangUserCouponJobLog = LogManager.GetLogger(typeof(NonBaoYangUserCouponJob));

        public static bool InsertBaoYangPromotionData()
        {
            var result = false;
            try
            {
                DalCoupon.InsertBaoYangPromotionData(PromotionType, CouponDescriptionOfTypeOne, CouponDescriptionOfTypeTwo,
                    PromotionruleId, BaoYangMessageSubject,
                    BaoyangMessageDetail, DiscountOfTypeOne, DiscountOfTypeTwo, MinMoneyOfTypeOne, MinMoneyOfTypeTwo,
                    CodeChannel);
                result = true;
            }
            catch (Exception ex)
            {
                BaoYangUserCouponJobLog.Error(ex.Message, ex);
            }

            return result;
        }

        public static bool InsertNonBaoYangPromotionData()
        {
            var result = false;
            try
            {
                DalCoupon.InsertNonBaoYangPromotionData(PromotionType, CouponDescriptionOfTypeOne, CouponDescriptionOfTypeTwo,
                    PromotionruleId,
                    NonBaoYangMessageSubject,
                    NonBaoyangMessageDetail, DiscountOfTypeOne, DiscountOfTypeTwo, MinMoneyOfTypeOne, MinMoneyOfTypeTwo,
                    CodeChannel);
                result = true;
            }
            catch (Exception ex)
            {
                NonBaoYangUserCouponJobLog.Error(ex.Message, ex);
            }

            return result;
        }

        public static void InsertFreeOrderCoupon()
        {
            DalCoupon.InsertFreeOrderCoupon(PromotionType, CouponFreeOrderDescription, CodeChannel);
        }

        /// <summary>感谢您参与{0}活动，活动返券已发至您的账户，可下载手机APP查看 http://t.cn/Ryy1xkt	</summary>
        /// <param name="cellphone">The cellphone.</param>
        /// <param name="activityName">The subject.</param>
        public static bool SendMarketingSms(string cellphone, string activityName) =>
            SendMarketingSms(cellphone, activityName, "活动返券");

        /// <summary>感谢您参与{0}活动，{1}元的{2}已发至您的账户，可下载手机APP查看 http://t.cn/Ryy1xkt	</summary>
        /// <param name="cellphone">手机号.</param>
        /// <param name="activityName">活动名称</param>
        /// <param name="money">金额</param>
        /// <param name="codeName">券名称</param>
        public static bool SendMarketingSms(string cellphone, string activityName, int money, string codeName) =>
            SendMarketingSms(cellphone, activityName, $"{money}元的{codeName}");

        /// <summary>感谢您参与{0}活动，{1}已发至您的账户，可下载手机APP查看 http://t.cn/Ryy1xkt	</summary>
        /// <param name="cellphone">手机号.</param>
        /// <param name="activityName">活动名称</param>
        /// <param name="message">The message.</param>
        private static bool SendMarketingSms(string cellphone, string activityName, string message)
        {
            SmsClient client = null;
            try
            {
                client = new SmsClient();

                var result = client.SendSms(cellphone, 35, activityName, message);

                result.ThrowIfException(true);

                return result.Result > 0;
            }
            catch (Exception ex)
            {
                BaoYangUserCouponJobLog.Error($"SendMarketingSms:{cellphone}/{activityName}", ex);
                return false;
            }
            finally
            {
                client?.Dispose();
            }
        }
    }
}