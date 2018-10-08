using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Elasticsearch.Net;
using Nest;
using Quartz;
using Tuhu.WebSite.Component.SystemFramework;
using Tuhu.Yewu.WinService.JobSchedulerService.Models;

namespace Tuhu.Yewu.WinService.JobSchedulerService.Job
{
    [DisallowConcurrentExecution]
    public class GaiZhuangProductUpdateToEsJob : IJob
    {
        private static readonly string IndexName = "gaizhuangproduct";

        protected static readonly ILog Logger = LogManager.GetLogger(typeof(GaiZhuangProductUpdateToEsJob));

        private static readonly string EsHost = ConfigurationManager.AppSettings["ESHost"];


        /// <summary>
        /// 刷新改装产品信息到ES
        /// </summary>
        /// <param name="context"></param>
        public virtual void Execute(IJobExecutionContext context)
        {
            Logger.Info("启动任务");

            TuhuNotification.SendNotification("ProductModify", new { type = "RebuildGaiZhuangIndex" });

            Logger.Info("结束任务");
        }

    }

}
