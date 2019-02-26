using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Tuhu.Component.Framework;
using Tuhu.Service.ActivityPage;
using Tuhu.Service.ActivityPage.Request.SetRequest;
using Tuhu.Service.ActivityPage.Response.SetResponse;

namespace Tuhu.Provisioning.Services
{
    public class ActivityPageService
    {
        private static readonly ILog logger = LoggerFactory.GetLogger("FeedBackController");
        /// <summary>
        /// 通过活动ID获取活动信息
        /// </summary>
        /// <param name="hashKey">活动ID</param>
        /// <returns></returns>
        public static GetActivityInfoSettingResponse GetActivityPage(string hashKey)
        {
            var activityInfoSettingRequest = new GetActivityInfoSettingRequest
            {
                ActivityId = hashKey
            };

            try
            {
                using (var activityPageClient = new ActivityPageSettingClient())
                {
                    var activityInfoResult = activityPageClient.GetActivityInfo(activityInfoSettingRequest);

                    if (activityInfoResult.Success)
                    {
                        return activityInfoResult.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "GetActivityInfo");
            }
            return new GetActivityInfoSettingResponse();
        }
    }
}