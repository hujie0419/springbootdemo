using System;
using System.Linq;
using Quartz;
using Common.Logging;
using Tuhu.C.Job.CommentStatisticsJob.Dal;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tuhu.C.Job.CommentStatisticsJob.Model;
using System.Diagnostics;
using Tuhu.Service.Comment;
using Tuhu.Service.Comment.Models;

namespace Tuhu.C.Job.CommentStatisticsJob.Job
{
    /// <summary>
    /// 产品追平评论 相关的信息 更新到 ES   （非每天 & 手动触发  全量）
    /// </summary>
    [DisallowConcurrentExecution]
    class ESUpdateProductAdditionCommentJob : IJob
    {
        private static ILog Logger = LogManager.GetLogger<ESUpdateProductAdditionCommentJob>();
        private static string jobName = new System.Diagnostics.StackTrace(true).GetFrame(1).GetMethod().DeclaringType.Name;
        private static int pageSize = 10;
        ///已经更新的 数目
        private static int exeCount = 0;
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info($"更新 {jobName} 执行开始");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Error($"更新  {jobName} 更新异常 结束 ：{ex}");
            }
            Logger.Info($"更新  {jobName} 更新完成 time={watcher.ElapsedMilliseconds}ms count={exeCount}");
            watcher.Stop();
        }

        public static void  DoJob()
        {
            int pageIndex = 1;

            int  totalCount = DalProductStatistics.SelectAdditionCommentCount();
            Logger.Info($"更新   {jobName}  SelectAdditionCommentCount => count={totalCount}");
            int pageTotal = (totalCount - 1) / pageSize + 1;
           
            List<int> pageIndexList = new List<int>();

            for (; pageIndex <= pageTotal; pageIndex++)
            {
                pageIndexList.Add(pageIndex);
            }
            Parallel.ForEach(pageIndexList, new ParallelOptions() { MaxDegreeOfParallelism = 5 }, i =>
             {
                 var sw = new Stopwatch();
                 sw.Start();
                 ExecuteByPage(i, pageSize);
                 Logger.Info($"更新 追评评论 {jobName} 第{i}页  页数={pageSize} 更新完成 time={sw.ElapsedMilliseconds}ms");
                 sw.Stop();
             });
        }

        /// <summary>
        /// 分批执行
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        private static  void ExecuteByPage(int pageIndex,int pageSize)
        {
            #region 批量更新
            List<int> commentIDs = DalProductStatistics.SelectAdditionComment_Page(pageIndex,pageSize).ToList();
            if (commentIDs == null || !commentIDs.Any())
            {
                return;
            }
            using (var client = new ProductCommentClient())
            {
                var result = client.RefreshCommentToESByCommentIds(commentIDs);
                if (result.Success && result.Result)
                {
                    exeCount += commentIDs.Count;
                    Logger.Info($"更新 追评评论 成功  pageIndex ={pageIndex}  & exeCount{exeCount}");
                }
                else
                {
                    Logger.Info($"更新 追评评论 失败  pageIndex ={pageIndex}  异常 message={result.ErrorMessage} & maxid={commentIDs.Max()}&minid={commentIDs.Min()} ");
                }
            }
            #endregion

        }

    }
}
