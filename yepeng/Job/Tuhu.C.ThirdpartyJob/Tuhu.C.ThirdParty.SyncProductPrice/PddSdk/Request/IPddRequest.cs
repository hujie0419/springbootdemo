using PddSdk.Response;
using System.Net.Http;

namespace PddSdk.Request
{
    /// <summary>
    /// Pdd请求接口参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPddRequest<T> where T : PddResponse
    {
        /// <summary>
        /// 请求方式
        /// </summary>
        HttpMethod Method { get; }
        /// <summary>
        /// 请求链接
        /// </summary>
        string Uri { get; }
        /// <summary>
        /// 请求参数
        /// </summary>
        /// <returns></returns>
        object GetParam();

        /// <summary>
        /// 提前验证参数。
        /// </summary>
        void Validate();
    }
}
