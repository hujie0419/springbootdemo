using System;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Tuhu.C.Job.UserAuthJob.BLL
{
    public class UserWxAuthHelper : UserAuthSendRequest
    {
        #region 微信

        private static readonly string UWeixinAppkey = ConfigurationManager.AppSettings["UWeixinAppkey"];
        private static readonly string UWeixinAppsecret = ConfigurationManager.AppSettings["UWeixinAppsecret"];

        /// <summary>
        /// WX 获取code后，请求以下链接获取access_token
        /// 0:APPID
        /// 1:SECRET
        /// 2:Code
        /// </summary>
        private static string UWeixinGetTokenWithCode =>
            @"https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";

        /// <summary>
        /// 拉取用户信息
        /// 0:ACCESS_TOKEN
        /// 1:OPENID
        /// </summary>
        private static string UWeixinGetUserInfo =>
            @"https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN";

        /// <summary>
        /// 刷新access_token
        /// 0:APPID
        /// 1:REFRESH_TOKEN
        /// </summary>
        private static string UWeixinRefresh =>
            @"https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={0}&grant_type=refresh_token&refresh_token={1}";

        /// <summary>
        /// 检验access是否有效
        /// 0:ACCESS_TOKEN
        /// 1:OPENID
        /// </summary>
        private static string UWeixinCheckAccessToken =>
            @"https://api.weixin.qq.com/sns/auth?access_token={0}&openid={1}";

        #endregion

        /// <summary>
        /// 刷新accessToken
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>
        /// {
        ///  "access_token":"ACCESS_TOKEN",
        ///  "expires_in":7200,
        ///  "refresh_token":"REFRESH_TOKEN",
        ///  "openid":"OPENID",
        ///  "scope":"SCOPE"
        /// }
        /// </returns>
        public async Task<JObject> GetRefreshTokenAsync(string refreshToken)
            => await GetJsonAsync(string.Format(UWeixinRefresh, UWeixinAppkey, refreshToken), Encoding.UTF8);

        /// <summary>
        /// 拉取微信用户信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <returns>
        /// {
        ///  "openid":" OPENID",
        ///  "nickname": NICKNAME,
        ///  "sex":"1",
        ///  "province":"PROVINCE"
        ///  "city":"CITY",
        ///  "country":"COUNTRY",
        ///  "headimgurl": "http://wx.qlogo.cn/mmopen/g3MonUZtNHkdmzicIlibx6iaFqAc56vxLSUfpb6n5WKSYVY0ChQKkiaJSgQ1dZuTOgvLLrhJbERQQ4eMsv84eavHiaiceqxibJxCfHe/46", 
        ///	  "privilege":[
        ///	        "PRIVILEGE1"
        ///	        "PRIVILEGE2"
        ///         ],
        ///   "unionid": "o6_bmasdasdsad6_2sgVt7hMZOPfL"
        /// }
        /// </returns>
        public async Task<JObject> GetUserInfoAsync(string accessToken, string openId)
            => await GetJsonAsync(string.Format(UWeixinGetUserInfo, accessToken, openId), Encoding.UTF8);

        ///// <summary>
        ///// 检查token是否有效
        ///// </summary>
        ///// <param name="accessToken"></param>
        ///// <param name="openId"></param>
        ///// <returns></returns>
        //public async Task<OperationResult<bool>> CheckTokenAsync(string accessToken, string openId)
        //{
        //    var result = await GetJsonAsync(string.Format(UWeixinCheckAccessToken, accessToken, openId), Encoding.UTF8);
        //    if (!result.Success) return OperationResult.FromError<bool>(result.ErrorCode, result.ErrorMessage);

        //    var code = result.Result.Value<string>("errcode");
        //    return code != null && code == "0");
        //}
    }
}
