using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.C.Monitor;

namespace Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy
{
    /// <summary>
    /// 监控服务
    /// </summary>
    public interface IMonitorService
    {
        /// <summary>
        ///  Metrics计数器 
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OperationResult<bool>> MetricsCounterAsync(MetricsCounterRequest Request, CancellationToken cancellationToken = default(CancellationToken));
    }
}
