using System;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy
{
    /// <summary>
    /// 获取用户信息
    /// </summary>
    public interface IUserAccountService
    {
        /// <summary>
        /// 根据用户编号获取用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OperationResult<User>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
