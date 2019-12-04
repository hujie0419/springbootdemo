﻿using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.ActivityJob.Bll.WeChatServiceNumber;
using Tuhu.C.ActivityJob.Dal.WeChatServiceNumber;
using Tuhu.C.ActivityJob.ServiceProxy;
using Tuhu.Service.ThirdParty.Models.WeiXin;

namespace Tuhu.C.ActivityJob.Job.WeChatServiceNumber
{
    /// <summary>
    /// 描述：微信服务号 - 拉取每天的微信服务号聊天记录JOB
    /// Job执行时间:每天一次
    /// 创建人：姜华鑫
    /// </summary>
    public class WechatServiceNumberChatLogJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(WechatServiceNumberChatLogJob));

        //private static List<ChatMsg> chatMsgList = new List<ChatMsg>();//聊天记录集合

        public void Execute(IJobExecutionContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            Logger.Info("微信服务号 - 导入微信服务号聊天记录 JOB 开始执行");
            Run();
            stopwatch.Stop();
            Logger.Info($"微信服务号 - 导入微信服务号聊天记录 JOB 结束执行,用时{stopwatch.Elapsed}");
        }

        private void Run()
        {
            try
            {
                List<ChatMsg> chatMsgList = new List<ChatMsg>();//聊天记录集合

                DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                string startTime = (DateTime.Parse(DateTime.Now.AddDays(-1).ToShortDateString()) - time).TotalSeconds.ToString();
                string endTime = (DateTime.Parse(DateTime.Now.AddDays(-1).ToShortDateString() + " 23:59:59") - time).TotalSeconds.ToString();
                WeChatServiceNumberBll.GetWeChatMsgList(new GetWeChatMsgListReqeust
                {
                    PlatForm = 0,
                    StartTime = startTime,
                    EndTime = endTime,
                    Number = 10000,
                    MsgId = 1
                }, ref chatMsgList);

                Logger.Info($"微信服务号 - 导入微信服务号聊天记录 JOB，{DateTime.Now.AddDays(-1).Year}年{DateTime.Now.AddDays(-1).Month}月{DateTime.Now.AddDays(-1).Day}日" +
                    $"微信服务号的聊天记录数量是{chatMsgList.Count}条");

                if (chatMsgList != null && chatMsgList.Any())
                {
                    WeChatServiceNumberDal.ImportWechatServiceNumberChatLog(chatMsgList.Split(500));
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"WechatServiceNumberChatLogJob -> Run -> error ,异常消息：{ex.Message}，堆栈信息：{ex.StackTrace}");
            }
        }

    }
}
