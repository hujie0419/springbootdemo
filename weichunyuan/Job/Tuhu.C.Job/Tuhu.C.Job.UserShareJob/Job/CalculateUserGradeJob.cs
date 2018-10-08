using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Tuhu.C.Job.UserShareJob.Dal;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Request;
using System.Threading.Tasks;

namespace Tuhu.C.Job.UserShareJob.Job
{
    /// <summary>
    /// 计算用户会员等级
    /// </summary>
    [DisallowConcurrentExecution]
    public class CalculateUserGradeJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<CalculateUserGradeJob>();

        /// <summary>
        /// 计算逻辑
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            Stopwatch sw = new Stopwatch();
            Logger.Info($"用户会员等级批量计算开始");
            sw.Start();
            try
            {
              var result= CalculateUserGrade();
            }
            catch (Exception ex)
            {
                Logger.Info($"用户会员等级批量计算异常");
                Logger.Info(ex.ToString());
                Logger.Error($"Job计算会员等级异常：", ex);
            }
            sw.Stop();
            Logger.Info($"用户会员等级批量计算完成");
            Logger.Info($"用户会员等级批量计算完成，用时{sw.ElapsedMilliseconds}毫秒");
        }

        public async Task<bool> CalculateUserGrade()
        {
            using (var client = new Tuhu.Service.Member.MembershipsGradeClient())
            {
                var pageSize = 50;
                long minPKID = 0;
                var maxDate = DateTime.Now.AddYears(-1);
                var request = new QueryUserObjectInfoRequest()
                {
                    PageIndex = 1,
                    MinPkId = minPKID,
                    PageSize = pageSize,
                    StartDate = maxDate
                };
                var totalCount = await client.GetNeedCalculateUserGradeCountAsync(request);
                var pageIndex = 1;
                var PageCount = (totalCount.Result - 1) / pageSize + 1;
                while (pageIndex <= PageCount)
                {
                    pageIndex++;
                    request = new QueryUserObjectInfoRequest
                    {
                        MinPkId = minPKID,
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                    };
                    try
                    {
                        var dataList = await client.GetNeedCalculateUserGradeDataAsync(request);
                        if (dataList.Result == null || !dataList.Result.Any())
                        {
                            continue;
                        }
                        minPKID = dataList.Result.Max(t => t.PKID);
                        var calculateRequst = ObjectMapper.ConvertTo<UserObjectInfoResponse, CalculateUserGradeRequest>(dataList.Result);
                        await client.CaculateUserGradeListAsync(calculateRequst.ToList());
                    }
                    catch (Exception e)
                    {
                        Logger.Info($"获取或计算会员任务时出现异常");
                        Logger.Info($"获取或计算会员任务时出现异常：" + e.ToString());
                        continue;
                    }
                }
                return true;
            }
        }
    }
}
