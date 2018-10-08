using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.C.Job.UserAccountCombine.Model;
using Tuhu.C.Job.UserAccountCombine.BLL;
using Quartz;

namespace Tuhu.C.Job.UserAccountCombine.Job
{
    public class BatchImportUserJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(BatchImportUserJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Info("BatchImportUserJob开始执行：" + DateTime.Now.ToString());
                var result = UACManager.BatchImportUser();
                _logger.Info("BatchImportUserJob执行结束");
            }
            catch(Exception ex)
            {
                _logger.Info($"BatchImportUserJob：运行异常=》{ex}");
            }
        }
    }
}
