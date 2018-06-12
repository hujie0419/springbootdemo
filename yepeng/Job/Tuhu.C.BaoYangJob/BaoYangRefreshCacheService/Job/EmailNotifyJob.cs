using BaoYangRefreshCacheService.BLL;
using BaoYangRefreshCacheService.Common;
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Job
{
    [DisallowConcurrentExecution]
    public class EmailNotifyJob : BaseJob
    {
        public EmailNotifyJob()
        {
            logger = LogManager.GetLogger(typeof(EmailNotifyJob));
        }

        protected override string JobName
        {
            get { return typeof(EmailNotifyJob).ToString(); }
        }

        public override void Exec()
        {
            var obj = new EmailNotifyBll(logger);
            obj.SendEmails();
        }
    }
}
