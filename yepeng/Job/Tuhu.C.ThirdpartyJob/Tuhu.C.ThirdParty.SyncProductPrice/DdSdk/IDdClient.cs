using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DdSdk.Api.Request;
using DdSdk.Api.Response;

namespace DdSdk.Api
{
	public interface IDdClient
	{
		/// <summary>
		/// 执行TOP公开API请求。
		/// </summary>
		/// <typeparam name="T">领域对象</typeparam>
		/// <param name="request">具体的TOP API请求</param>
		/// <param name="session">会话ID</param>
		/// <returns>领域对象</returns>
		Task<T> ExecuteAsync<T>(IDdRequest<T> request, string session) where T : DdResponse;

		/// <summary>
		/// 执行TOP隐私API请求。
		/// </summary>
		/// <typeparam name="T">领域对象</typeparam>
		/// <param name="request">具体的TOP API请求</param>
		/// <param name="session">用户会话码</param>
		/// <param name="session">会话ID</param>
		/// <returns>领域对象</returns>
		//T Execute<T>(ITopRequest<T> request, string session, string session) where T : IQplResponse;
	}
}
