using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;
using Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy;

namespace Tuhu.Service.Promotion.Server.ServiceProxy
{
    /// <summary>
    /// 短信服务
    /// </summary>
    public class SmsService : ISmsService
    {
        private readonly ILogger _logger;
        private ISmsClient _Client;


        public SmsService(ILogger<SmsService> logger, ISmsClient client)
        {
            _logger = logger;
            _Client = client;
        }


        /// <summary>
        ///  发送追踪短信
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> SendBiSmsAsync(BiSmsRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await _Client.SendBiSmsAsync(request, cancellationToken).ConfigureAwait(false);
            result.ThrowIfException(true);
            if (!result.Success)
            {
                _logger.Error($"SmsService SendBiSmsAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage}");
            }
            return result;
        }

        /// <summary>
        ///  发送追踪短信 - 批量
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OperationResult<int>> SendBiSmsAsync(BiSmsRequest[] request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await _Client.SendBiSmsAsync(request, cancellationToken).ConfigureAwait(false);
            result.ThrowIfException(true);
            if (!result.Success)
            {
                _logger.Error($"SmsService SendBiSmsAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage}");
            }
            return result;
        }

        /// <summary>
        /// 发送普通短信
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<int>> SendSmsAsync(SendTemplateSmsRequest request)
        {
            var result = await _Client.SendSmsAsync(request).ConfigureAwait(false);
            result.ThrowIfException(true);
            if (!result.Success)
            {
                _logger.Error($"SmsService SendSmsAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage}");
            }
            return result;
        }

        /// <summary>
        /// 发送普通短信 - 批量
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<int>> SendBatchSmsAsync(SendBatchCellphoneSmsRequest request)
        {
            var result = await _Client.SendBatchSmsAsync(request).ConfigureAwait(false);
            result.ThrowIfException(true);
            if (!result.Success)
            {
                _logger.Error($"SmsService SendBatchSmsAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage}");
            }
            return result;
        }
    }
}
