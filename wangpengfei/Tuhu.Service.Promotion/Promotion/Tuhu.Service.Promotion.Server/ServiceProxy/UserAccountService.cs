using System.Threading.Tasks;
using Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy;
using System.Threading;
using Microsoft.Extensions.Logging;
using Tuhu.Service.UserAccount.Models;
using Tuhu.Service.UserAccount;
using System;

namespace Tuhu.Service.Promotion.Server.ServiceProxy
{
    /// <summary>
    /// 用户服务
    /// </summary>
    public class UserAccountService : IUserAccountService
    {
        private readonly ILogger _logger;
        private IUserAccountClient _Client;


        public UserAccountService(ILogger<UserAccountService> logger, IUserAccountClient client)
        {
            _logger = logger;
            _Client = client;
        }

        /// <summary>
        /// 根据用户编号获取用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OperationResult<User>> GetUserByIdAsync(Guid userId,CancellationToken cancellationToken = default)
        {
            var result = await _Client.GetUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
            result.ThrowIfException(true);
            if (!result.Success)
            {
                _logger.Error($"UserAccountService GetUserByIdAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage}");
            }
            return result;
        }

    }
}
