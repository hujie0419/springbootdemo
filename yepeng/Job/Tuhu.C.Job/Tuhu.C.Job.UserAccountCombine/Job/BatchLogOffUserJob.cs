using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using System.Linq;
using System.Collections.Generic;
using Tuhu.C.Job.UserAccountCombine.Model;
using Tuhu.C.Job.UserAccountCombine.BLL;
using Quartz;
using Tuhu.MessageQueue;
using Tuhu;

namespace Tuhu.C.Job.UserAccountCombine.Job
{
    [DisallowConcurrentExecution]
    public class BatchLogOffUserJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(BatchLogOffUserJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Info("BatchLogOffUserJob开始执行：" + DateTime.Now.ToString());
                var result = UACManager.BatchLogOffUser();
                _logger.Info("BatchLogOffUserJob执行结束");
            }
            catch (Exception ex)
            {
                _logger.Info($"BatchLogOffUserJob：运行异常=》{ex}");
            }
        }
    }
}
