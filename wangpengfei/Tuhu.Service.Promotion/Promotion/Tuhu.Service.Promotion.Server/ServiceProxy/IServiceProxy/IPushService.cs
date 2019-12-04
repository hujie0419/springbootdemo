using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy
{
    /// <summary>
    /// 推送服务
    /// </summary>
    public interface IPushService
    {
        /// <summary>
        ///  按照batchid和userid推送消息
        /// </summary>
        /// <param name="userids"></param>
        /// <param name="batchid"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        Task<OperationResult<bool>> PushByUserIDAndBatchIDAsync(IEnumerable<string> userids, int batchid, PushTemplateLog log);
    }
}
