using System.Threading.Tasks;
using Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy;
using System.Threading;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Tuhu.Service.Push.Models.Push;
using Tuhu.Service.Push;

namespace Tuhu.Service.Promotion.Server.ServiceProxy
{
    public class PushService : IServiceProxy.IPushService
    {

        private readonly ILogger _logger;
        private ITemplatePushClient _Client;


        public PushService(ILogger<PushService> logger, ITemplatePushClient client)
        {
            _logger = logger;
            _Client = client;
        }

        /// <summary>
        ///  按照batchid和userid推送消息
        /// </summary>
        /// <param name="userids"></param>
        /// <param name="batchid"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> PushByUserIDAndBatchIDAsync(IEnumerable<string> userids, int batchid, PushTemplateLog log)
        {
            var result = await _Client.PushByUserIDAndBatchIDAsync(userids, batchid, log).ConfigureAwait(false);
            result.ThrowIfException(true);
            if (!result.Success)
            {
                _logger.Error($"PushService PushByUserIDAndBatchIDAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage}");
            }
            return result;
        }
    }
}
