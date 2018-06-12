using PddSdk.Response;
using PddSdk.Request;
using System.Threading.Tasks;

namespace PddSdk
{
    /// <summary>
    /// 拼多多客户端
    /// </summary>
    public interface IPddClient
    {
        /// <summary>
        /// 执行拼多多公开API请求。
        /// </summary>
        /// <typeparam name="T">领域对象</typeparam>
        /// <param name="request">具体的Pdd API请求</param>
        /// <returns>领域对象</returns>
        Task<T> ExecuteAsync<T>(IPddRequest<T> request) where T : PddResponse;
    }
}
