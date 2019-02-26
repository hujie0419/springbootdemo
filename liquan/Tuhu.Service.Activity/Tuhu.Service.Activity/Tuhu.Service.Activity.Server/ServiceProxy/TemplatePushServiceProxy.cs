using Common.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Config;
using Tuhu.Service.Config.Models.Response;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.Service.Activity.Server.ServiceProxy
{
    /// <summary>
    /// 配置服务代理
    /// </summary>
    public class TemplatePushServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TemplatePushServiceProxy));

        /// <summary>
        /// 消息推送
        /// </summary>
        /// <param name="userids"></param>
        /// <param name="batchid"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public async static Task<OperationResult<bool>> PushByUserIDAndBatchIDAsync(IEnumerable<string> userids, int batchid, PushTemplateLog log)
        {
            if (userids != null && userids.Any())
            {
                using (var client = new TemplatePushClient())
                {
                    var result = await client.PushByUserIDAndBatchIDAsync(userids, batchid, log);
                    if (!result.Success)
                    {
                        Logger.Error($" PushByUserIDAndBatchIDAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage} ");
                    }
                    return result;
                }
            }
            else
            {
                return OperationResult.FromResult(true);
            }
        }
    }
}
