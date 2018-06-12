using Common.Logging;
using Quartz;
using System;

namespace Tuhu.C.Job.BaoYangSuggest
{
    [DisallowConcurrentExecution]
    public class UserBaoYangRecordJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(UserBaoYangRecordJob));
        public void Execute(IJobExecutionContext context)
        {
            logger.Info("用户保养档案服务启动");

            try
            {
                var updateBaoYangRecordResult = SuggestBusiness.UpdateBaoYangRecord();
                if (updateBaoYangRecordResult)
                {
                    logger.Info("保养档案更新成功");
                }
                else
                {
                    logger.Info("保养档案更新失败");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            logger.Info("用户保养档案服务结束");
        }
    }
}
