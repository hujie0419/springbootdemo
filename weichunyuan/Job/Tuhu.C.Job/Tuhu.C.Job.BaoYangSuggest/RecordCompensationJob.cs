using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.BaoYangSuggest
{
    [DisallowConcurrentExecution]
    public class RecordCompensationJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(RecordCompensationJob));
        public void Execute(IJobExecutionContext context)
        {
            logger.Info("补偿用户保养档案服务启动");

            try
            {
                var updateBaoYangRecordResult = SuggestBusiness.UpdateBaoYangRecord(true);
                if (updateBaoYangRecordResult)
                {
                    logger.Info("补偿保养档案更新成功");
                }
                else
                {
                    logger.Info("补偿保养档案更新失败");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            logger.Info("补偿用户保养档案服务结束");
        }
    }
}
