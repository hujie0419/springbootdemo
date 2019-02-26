using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Common.Logging;

namespace Tuhu.Provisioning.Business.ServiceProxy
{
    /// <summary>
    /// 用户服务
    /// </summary>
    public class UserService
    {
        private readonly ILog _logger;

        public UserService()
        {
            _logger = LogManager.GetLogger<UserService>();
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public async Task<UserObjectModel> FetchUserByUserId(Guid userId)
        {
            try
            {
                using (var userClient = new UserClient())
                {
                    var userResult = await userClient.FetchUserByUserIdAsync(userId.ToString());
                    userResult.ThrowIfException(true);
                    return userResult?.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"获取用户信息失败 {userId}", ex);
                return null;
            }
        }
    }
}
