using System.Threading.Tasks;
using Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy;
using System.Threading;
using Microsoft.Extensions.Logging;
using Tuhu.Service.C.Monitor;

namespace Tuhu.Service.Promotion.Server.ServiceProxy
{
    public class MonitorService : IMonitorService
    {
        private readonly ILogger _logger;
        private IMonitorClient _Client;


        public MonitorService(ILogger<MonitorService> logger, IMonitorClient client)
        {
            _logger = logger;
            _Client = client;
        }

        /// <summary>
        /// 计数器
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> MetricsCounterAsync(MetricsCounterRequest Request, CancellationToken cancellationToken = default)
        {
            var result = await _Client.MetricsCounterAsync(Request, cancellationToken).ConfigureAwait(false);
            if (!result.Success)
            {
                _logger.Warn($"MonitorService MetricsCounterAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage}");
            }
            return result;
        }
    }
}
