using Common.Logging;
using Quartz;
using System;

namespace Tuhu.C.Job.BaoYangSuggest
{
    [DisallowConcurrentExecution]
    public class MaintainNewDataJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MaintainNewDataJob));
        public void Execute(IJobExecutionContext context)
        {
            logger.Info("维护用户保养档案新数据服务启动");

            try
            {
                SuggestBusiness.MaintainBaoYangRecordNewData();

                logger.Info("维护用户保养档案新数据更新成功");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            logger.Info("维护用户保养档案新数据结束");
        }
    }
}
