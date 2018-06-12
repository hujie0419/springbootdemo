using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Enums;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.C.Job.ThirdPart.Proxy
{
    public class UserAccountServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserAccountServiceProxy));
        /// <summary>
        /// 查询或者创建用户 
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="userName"></param>
        /// <param name="nickName"></param>
        /// <param name="headUrl"></param>
        /// <returns></returns>
        public static async Task<User> GetOrCreateUser(string mobile, string userName = null, string nickName = null, string headUrl = null)
        {
            User user = null;
            try
            {
                if (!string.IsNullOrEmpty(mobile))
                {
                    using (var client = new UserAccountClient())
                    {
                        var result = client.GetUserByMobile(mobile);
                        user = result.Result;
                    }
                    if (user == null)
                    {
                        using (var client = new UserAccountClient())
                        {
                            var createResult = client.CreateUserRequest(
                                new CreateUserRequest
                                {
                                    MobileNumber = mobile,
                                    Profile = new UserProfile
                                    {
                                        UserName = userName,
                                        NickName = nickName,
                                        HeadUrl = headUrl
                                    },
                                    ChannelIn = nameof(ChannelIn.ThirdPartyDiversion),
                                    UserCategoryIn = nameof(UserCategory.Tuhu),
                                });
                            createResult.ThrowIfException(true);
                            user = createResult.Result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("创建用户失败", ex);
            }
           
            return user;
        }
    }
}
