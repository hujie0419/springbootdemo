using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.ThirdParty;
using Tuhu.Service.ThirdParty.Models.WeiXin;

namespace Tuhu.C.ActivityJob.ServiceProxy
{
    public class ThirdPartyServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ThirdPartyServiceProxy));

        /// <summary>
        /// 获取聊天记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static GetWeChatMsgListResponse GetWeChatMsgList(GetWeChatMsgListReqeust request)
        {
            var response = new GetWeChatMsgListResponse();
            try
            {
                using(var client=new WeiXinServiceClient())
                {
                    var getResult = client.GetWeChatMsgList(request);
                    if (!getResult.Success)
                    {
                        Logger.Warn($"GetWeChatMsgList,获取聊天记录接口失败，message:{getResult.ErrorMessage}");
                    }
                    else
                    {
                        response = getResult.Result ?? new GetWeChatMsgListResponse();
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Error($"GetWeChatMsgList 接口异常，异常信息：{ex.Message}，堆栈信息：{ex.StackTrace}");
            }
            return response;
        }
    }
}
