using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class LuckyCharmJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<LuckyCharmJob>();
        public void Execute(IJobExecutionContext context)
        {
            Logger.InfoFormat("定时审核服务于{0}时，开始运行", DateTime.Now);
            using (var client = new LuckyCharmClient())
            {
                var condition = new PageLuckyCharmUserRequest() {CheckState=0, AreaName = "上海市闵行区", PageIndex = 0, PageSize = 10 };
                var IsRun = true;
                while (IsRun)
                {
                    try
                    {
                        var users = client.PageLuckyCharmUserAsync(condition);
                        if (!users.Result.Success)
                        {
                            Logger.ErrorFormat("分页获取失败，错误编码：{0}  描述：{1}", users.Result.ErrorCode, users.Result.ErrorMessage);
                        }
                        if (users.Result.Result.Total <= 0)
                        {
                            break;
                        }
                        foreach (var item in users.Result.Result.Users)
                        {
                            //该处可以加入其他的业务逻辑判断，当前的限定上海市闵行区的条件查询只是一个举例
                            //同时可以调用另外的重载client.AuditLuckyCharmUser(AreaName)完成直接的逻辑审核;
                            var isSuccess = client.AuditLuckyCharmUser(item.PKID);
                            if (!isSuccess.Result)
                            {
                                Logger.ErrorFormat("审核失败，错误编码：{0}  描述：{1}", isSuccess.ErrorCode, isSuccess.ErrorMessage);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorFormat("发生不可预测错误：{0}", ex);
                    }
                    finally
                    {
                        IsRun = false;
                    }
                }
            }
            Logger.InfoFormat("定时审核服务于{0}时，结束运行", DateTime.Now);
        }
    }
}
