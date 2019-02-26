using System;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;

namespace Tuhu.Service.Activity.Server.ServiceProxy
{
    /// <summary>
    ///     优惠券服务代理[old]
    /// </summary>
    public class MemberServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MemberServiceProxy));

        /// <summary>
        ///     创建优惠券
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<CreatePromotionCodeResult>> CreatePromotionNewAsync(
            CreatePromotionModel model)
        {
            using (var client = new PromotionClient())
            {
                var result = await client.CreatePromotionNewAsync(model);
                if (!result.Success)
                {
                    Logger.Error(
                        $"PromotionClient CreatePromotionNewAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage} ");
                }

                return result;
            }
        }

        /// <summary>
        ///     获取用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<OperationResult<UserObjectModel>> FetchUserByUserIdAsync(Guid userId)
        {
            using (var client = new UserClient())
            {
                var result = await client.FetchUserByUserIdAsync(userId.ToString());
                if (!result.Success)
                {
                    Logger.Error(
                        $"MemberServiceProxy -> FetchUserByUserIdAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage} ");
                }

                return result;
            }
        }


        /// <summary>
        ///     获取用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static OperationResult<UserObjectModel> FetchUserByUserId(Guid userId)
        {
            using (var client = new UserClient())
            {
                var result = client.FetchUserByUserId(userId.ToString());
                if (!result.Success)
                {
                    Logger.Error(
                        $"MemberServiceProxy -> FetchUserByUserIdAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage} ");
                }

                return result;
            }
        }
    }
}
