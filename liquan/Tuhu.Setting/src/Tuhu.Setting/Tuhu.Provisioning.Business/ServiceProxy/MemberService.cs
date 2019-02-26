using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Common.Logging;

namespace Tuhu.Provisioning.Business.ServiceProxy
{
    public class MemberService
    {
        private readonly ILog _logger;

        public MemberService()
        {
            _logger = LogManager.GetLogger<MemberService>();
        }

        /// <summary>
        /// 获取会员等级列表（异步调用)
        /// </summary>
        /// <returns></returns>
        public async Task<List<MembershipsGradeResponse>> GetMembershipsGradeListAsync()
        {
            using (var memberClient = new MembershipsGradeClient())
            {
                var result = await memberClient.GetMembershipsGradeListAsync(null);
                result.ThrowIfException();
                return result?.Result.ToList();
            }
        }

        /// <summary>
        /// 获取会员等级列表（同步调用）
        /// </summary>
        /// <returns></returns>
        public List<MembershipsGradeResponse> GetMembershipsGradeList()
        {
            using (var memberClient = new MembershipsGradeClient())
            {
                var result =  memberClient.GetMembershipsGradeList(null);
                result.ThrowIfException();
                return result?.Result.ToList();
            }
        }

        #region 会员签到
        /// <summary>
        /// 根据用户ID获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public  async Task<bool?> RefreshUserDailyCheckInConfigCacheAsync()
        {
            try
            {
                using (var client = new UserDailyCheckInClient())
                {
                    var response = await client.RefreshUserDailyCheckInConfigCacheAsync();
                    response.ThrowIfException(true);
                    return response?.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("获取UserDailyCheckInClient==>RefreshUserDailyCheckInConfigCacheAsync接口异常", ex);
                return false;
            }
        }
        #endregion
    }
}
