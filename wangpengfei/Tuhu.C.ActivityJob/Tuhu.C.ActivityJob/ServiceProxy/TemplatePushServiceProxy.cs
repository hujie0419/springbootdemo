using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.C.ActivityJob.ServiceProxy
{
    public class TemplatePushServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TemplatePushServiceProxy));


        /// <summary>
        /// 微信消息模板推送
        /// </summary>
        /// <param name="userids"></param>
        /// <param name="batchid"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static async Task<bool> PushByUserIDAndBatchIDAsync(IEnumerable<string> userids,
            int batchid, PushTemplateLog log)
        {
            try
            {
                using (var client = new TemplatePushClient())
                {
                    var pushResult = await client.PushByUserIDAndBatchIDAsync(userids, batchid, log);
                    if (pushResult.Success && pushResult.Result)
                    {
                        return true;
                    }
                    else
                    {
                        Logger.Warn($"PushByUserIDAndBatchIDAsync 推送失败{pushResult.ErrorCode + pushResult.ErrorMessage}");
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error($"PushByUserIDAndBatchIDAsync 异常,{ex.Message + ex.StackTrace}", ex);
            }

            return false;
        }
    }
}
