using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Tuhu.Component.Framework;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Request;
using Exception = System.Exception;

namespace Tuhu.Provisioning.Areas.FeedBack.Services
{
    public class OrderTaskService
    {
        private static readonly ILog logger = LoggerFactory.GetLogger("OrderTaskService");
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="remark">反馈内容</param>
        /// <param name="phone">手机号</param>
        /// <param name="currentUser">当前用户</param>
        /// <returns></returns>
        public static async Task<int> CreateTaskNeiBuCommonAsync(string remark,string phone,string currentUser)
        {
            try
            {
                var request = new CreateTaskNeiBuCommonInput
                {
                    Priority = 14,
                    Title = "评论反馈",
                    Remark = remark,
                    Res = phone,
                    Creator = currentUser
                };
                using (var orderTaskClient = new OrderTaskClient())
                {
                    var result=await orderTaskClient.CreateTaskNeiBuCommonAsync(request);
                    return result.Result;
                }
            }
            catch (Exception e)
            {
                logger.Log(Level.Error,e.Message);
                return 0;
            }
        }

        /// <summary>
        /// 插入评论反馈服务接口
        /// </summary>
        /// <returns></returns>
        public static async Task<int> CreateUmengCommentsAsync(int taskId,string content,string phone,string currentUser,string appVersion,string phoneModel)
        {
            try
            {
                var request = new UmengCommentsRequest
                {
                    TaskId = taskId,
                    Content = content,
                    Phone = phone,
                    UserId =currentUser,
                    AppVersion = appVersion,
                    PhoneModel = phoneModel
                };
                using (var orderTaskClient = new OrderTaskClient())
                {
                    var result = await orderTaskClient.CreateUmengCommentsAsync(request);
                    return result.Result;
                }
            }
            catch (Exception e)
            {
                logger.Log(Level.Error, e.Message);
                return 0;
            }
        }
    }
}