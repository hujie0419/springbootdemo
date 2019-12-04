using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.C.ActivityJob.ServiceProxy;
using Tuhu.Service.ThirdParty.Models.WeiXin;

namespace Tuhu.C.ActivityJob.Bll.WeChatServiceNumber
{
    public class WeChatServiceNumberBll
    {

        /// <summary>
        /// 获取聊天记录
        /// </summary>
        /// <param name="request"></param>
        public static void GetWeChatMsgList(GetWeChatMsgListReqeust request, ref List<ChatMsg> chatMsgList)
        {

            //获取聊天记录的微信接口 的调用频率有限制，60秒内最多可以调用10次,比如我连续调这个接口30次，第4次左右就提示目前不可以调用，过一会才能调，所以每次延迟10秒再调用
            Thread.Sleep(10000);
            var response = ThirdPartyServiceProxy.GetWeChatMsgList(request);
            if (response != null && response.Code != null && response.Code == "000" && response.ChatMsgList != null && response.ChatMsgList.Any())
            {

                //添加聊天记录
                chatMsgList.AddRange(response.ChatMsgList);

                if (response.Number >= request.Number)
                {
                    request.MsgId = response.Msgid;
                    GetWeChatMsgList(request, ref chatMsgList);
                }
            }
        }
    }
}
