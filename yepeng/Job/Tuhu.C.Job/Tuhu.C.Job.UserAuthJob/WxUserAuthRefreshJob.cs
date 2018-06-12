using Common.Logging;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.UserAccount.Enums;
using Tuhu.Service.UserAccount.Models;
using Tuhu.C.Job.UserAuthJob.BLL;
using Tuhu.C.Job.UserAuthJob.DAL;

namespace Tuhu.C.Job.UserAuthJob
{
    [DisallowConcurrentExecution]
    public class WxUserAuthRefreshJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<WxUserAuthRefreshJob>();

        private static readonly int STEP = 1; //请求步长

        private static readonly int WxExpainDay = 23; //微信refreshToken 30天过期,此数值需要小于30

        private static int loggerIndex = 0;

        private static UserWxAuthHelper requestHelper = new UserWxAuthHelper();

        public void Execute(IJobExecutionContext context)
        {
            var index = loggerIndex++;
            var success = 0;
            Logger.Info($"WxRef{index}:开始执行");
            var allAuth = WxUserAuthRefreshDal.GetAllRefreshUserAuth(WxExpainDay);
            if (allAuth != null && allAuth.Any())
            {
                var auths = allAuth.ToList();
                Logger.Info($"WxRef{index}:数据量=>{auths.Count}");

                var updateTask = new List<Task<bool>>(STEP);

                for (var i = 0; i < auths.Count; i += STEP)
                {
                    try
                    {
                        //每批间隔100ms
                        Thread.Sleep(20);
                        updateTask.Clear();
                        for (var j = 0; j < STEP && i + j < auths.Count; j++)
                            updateTask.Add(RefreshOneToken(auths[i + j].RefreshToken, index));

                        if (updateTask.Any())
                        {
                            AsyncHelper.RunSync(() => Task.WhenAll(updateTask));
                            for (int u = 0; u < updateTask.Count; u++)
                            {
                                var error = updateTask[u];
                                if (!error.Result)
                                    Logger.Warn($"WxRef{index}:更新错误=>OldRefreshToken:{auths[i + u].RefreshToken}");
                                else
                                    success++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Info($"WxRef{index}:异常|i={i}|success={success}|{ex.ToString()}");
                    }
                }
                Logger.Info($"WxRef{index}:更新完成,总数:{auths.Count} 成功数:{success}");
            }
            Logger.Info($"WxRef{index}:执行完毕");
        }

        private async Task<bool> RefreshOneToken(string refreshToken, int logIndex)
        {
            var newRefreshToken = await requestHelper.GetRefreshTokenAsync(refreshToken);
            var oauthTokens = new RefreshOAuthTokenKey();
            oauthTokens.oldRefreshTokey = refreshToken;
            if (newRefreshToken==null)
            {
                Logger.Warn($"WxRef{logIndex}:刷新token错误=>返回null");
                return false;
            }

            if (newRefreshToken.Value<string>("errcode") == null)
            {
                oauthTokens.AccessTokey = newRefreshToken.Value<string>("access_token");
                oauthTokens.ExpriesTime = newRefreshToken.Value<string>("expires_in");
                oauthTokens.RefreshTokey = newRefreshToken.Value<string>("refresh_token");
                oauthTokens.Uid = newRefreshToken.Value<string>("openid");

                var userInfo = await requestHelper.GetUserInfoAsync(oauthTokens.AccessTokey, oauthTokens.Uid);
                if (userInfo != null && userInfo.Value<string>("errcode") == null)
                {
                    oauthTokens.MetaData = userInfo.ToString();
                    oauthTokens.RefreshStatus = "Success";
                }
                else
                {
                    if (userInfo == null)
                        oauthTokens.RefreshStatus = "Success-errorMetaData";// Logger.Warn($"WxRef{logIndex}:获取用户信息错误=>返回null");
                    else if (userInfo.Value<string>("errcode") != null)
                        oauthTokens.RefreshStatus = $"Success-errorMetaData-{userInfo.Value<string>("errcode")}";//Logger.Warn($"WxRef{logIndex}:获取用户信息错误=>{userInfo.Value<string>("errcode")}");
                }
                oauthTokens.AuthorizationStatus = AuthStatusEnum.Authorized;
                return await WxUserAuthRefreshDal.UpdateRefreshToken(oauthTokens);
            }
            else
            {
                var errorCode = newRefreshToken.Value<string>("errcode");
                oauthTokens.RefreshStatus = errorCode;
                oauthTokens.AuthorizationStatus = AuthStatusEnum.UnAuthorized;
                //42007 用户修改微信密码，accesstoken和refreshtoken失效，需要重新授权
                //40030 不合法的refresh_token
                //42002 refresh超时
                if (errorCode == "42007" || errorCode == "40030" || errorCode == "42002") //下次不再刷新
                    return await WxUserAuthRefreshDal.UpdateRefreshToken(oauthTokens);

                //42002 refresh_token超时
                Logger.Warn($"WxRef{logIndex}:刷新token错误=>errorCode:{errorCode}");
                return false;
            }
        }
    }

    public class RefreshOAuthTokenKey : OAuthTokenKey
    {
        public string oldRefreshTokey { get; set; }
        public string RefreshStatus { get; set; }

        public string MetaData { get; set; }

        public AuthStatusEnum? AuthorizationStatus { get; set; }

    }
}
