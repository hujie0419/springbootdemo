using System.Threading;
using System.Threading.Tasks;

using Tuhu.Service.Utility.Request;

namespace Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy
{
    /// <summary>
    /// 短信服务
    /// </summary>
    public interface ISmsService
    {
        /// <summary>
        /// 发送追踪短信
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OperationResult<bool>> SendBiSmsAsync(BiSmsRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 发送追踪短信 - 批量
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OperationResult<int>> SendBiSmsAsync(BiSmsRequest[] request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 发送普通短信
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<OperationResult<int>> SendSmsAsync(SendTemplateSmsRequest request);

        /// <summary>
        /// 发送普通短信 - 批量
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<OperationResult<int>> SendBatchSmsAsync(SendBatchCellphoneSmsRequest request);

    }
}
