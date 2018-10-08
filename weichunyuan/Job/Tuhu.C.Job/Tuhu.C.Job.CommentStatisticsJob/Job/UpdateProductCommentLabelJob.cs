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
    /// 产品评论的标签  初始化 （非每天 & 手动触发  全量）
    /// </summary>
    [DisallowConcurrentExecution]
    class UpdateProductCommentLabelJob : IJob
    {
        private static ILog Logger = LogManager.GetLogger<UpdateProductCommentLabelJob>();
        private static int count = 0;
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info($"更新 产品评论的标签 UpdateProductCommentLabelJob 执行开始");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Error($"更新 产品评论的标签 UpdateProductCommentLabelJob 更新异常：{ex}");
            }
            Logger.Info($"更新 产品评论的标签 UpdateProductCommentLabelJob 更新完成 time={watcher.ElapsedMilliseconds}ms count={count}");
            watcher.Stop();
        }

        public static void  DoJob()
        {
            DateTime start = new DateTime(2016, 1, 1) ;
            DateTime end = DateTime.Now;

            int pageIndex = 1;
            int pageSize = 500;
            int maxID = DalProductStatistics.SelectCommentsMaxID();
            int pageTotal = (maxID - 1) / pageSize + 1;
           
            List<int> pageIndexList = new List<int>();

            for (; pageIndex <= pageTotal; pageIndex++)
            {
                pageIndexList.Add(pageIndex);
            }
          
            Parallel.ForEach(pageIndexList, new ParallelOptions() { MaxDegreeOfParallelism = 5 }, f =>
             {
                 var sw = new Stopwatch();
                 sw.Start();
                 BatchUpdateProductCommentLabel(f, pageSize);
                 Logger.Info($"更新门店评论的类型 第{f}页  页数={pageSize} 更新完成 time={sw.ElapsedMilliseconds}ms");
                 sw.Stop();
             });
        }

        private static  void BatchUpdateProductCommentLabel(int pageIndex,int pageSize)
        {
            #region 批量更新
            List<EsProductCommentModel> comments = DalProductStatistics.SelectCommentsByPage(pageIndex,pageSize).Result.ToList();
            count += comments.Count;
            if (comments == null || !comments.Any())
            {
                return;
            }
            using (var client = new ProductCommentClient())
            {
                var result = client.RefreshCommentToESByCommentIds(comments.Select(p => p.Id).ToList());
                if (result.Success && result.Result)
                {
                    Logger.Info($"更新 产品评论的标签 BatchUpdateProductCommentLabel pageIndex ={pageIndex} 有效的 comment 数目 ={comments.Count()} & 总次数count{count}");
                }
                else
                {
                    Logger.Info($"更新 产品评论的标签 更新失败  BatchUpdateProductCommentLabel pageIndex ={pageIndex} 异常 message={result.ErrorMessage} & ={comments.Count()}");
                }
            }
            #endregion

        }

    }
}
