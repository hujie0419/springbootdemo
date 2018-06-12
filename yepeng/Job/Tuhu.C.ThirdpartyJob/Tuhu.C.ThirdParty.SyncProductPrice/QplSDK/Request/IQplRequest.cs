using System.Collections.Generic;
using System.Net.Http;
using Qpl.Api.Response;

namespace Qpl.Api.Request
{
    /// <summary>
    /// TOP请求接口。
    /// </summary>
    public interface IQplRequest<T> where T : QplResponse
    {
        /// <summary>
        /// 获取Jd的API名称。
        /// </summary>
        /// <returns>API名称</returns>
        string ApiName { get; }

        HttpMethod Method { get; }

        string Uri { get; }

        object GetParam();

        /// <summary>
        /// 提前验证参数。
        /// </summary>
        void Validate();

        /// <summary>提前验证参数。</summary>
        bool EncryptParam { get; }
    }
}
