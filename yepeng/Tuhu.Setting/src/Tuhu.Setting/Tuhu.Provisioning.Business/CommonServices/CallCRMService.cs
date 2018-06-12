using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.CommonServices;
using Tuhu.Service.CallCenter;

namespace Tuhu.Provisioning.Business.CommonServices
{
    public static class CallCRMService
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(CallCRMService));

        public static bool NewAddActivity(string activityName, DateTime startTime, DateTime endTime, string webUrl, string activityRules, string sourceId, CRMSourceType sourceName, string user)
        {
            var result = false;
            try
            {
                using (var client = new CRMClient())
                {
                    var getResult = client.NewAddActivity(new Service.CallCenter.Models.Activity()
                    {
                        ActivityName = activityName,
                        StartTime = startTime,
                        EndTime = endTime,
                        SellingPoint = webUrl,
                        Detail = activityRules,
                        SourceId = sourceId + sourceName.ToString(),
                        SourceName = sourceName.ToString(),
                        CreateUser = user
                    });
                    result = getResult.Result.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public static bool NewUpdateActivity(string activityName, DateTime startTime, DateTime endTime, string webUrl, string activityRules, string sourceId, CRMSourceType sourceName, string user)
        {
            var result = false;
            try
            {
                using (var client = new CRMClient())
                {
                    var getResult = client.NewUpdateActivity(new Service.CallCenter.Models.Activity()
                    {
                        ActivityName = activityName,
                        StartTime = startTime,
                        EndTime = endTime,
                        SellingPoint = webUrl,
                        Detail = activityRules,
                        SourceId = sourceId + sourceName.ToString(),
                        UpdateUser = user
                    });
                    result = getResult.Result.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public static bool NewDeleteActivityBySourceId(string sourceId, CRMSourceType sourceName, string user)
        {
            var result = false;
            try
            {
                using (var client = new CRMClient())
                {
                    var getResult = client.NewDeleteActivityBySourceId(sourceId + sourceName.ToString(), user);
                    result = getResult.Result.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
    }
}
