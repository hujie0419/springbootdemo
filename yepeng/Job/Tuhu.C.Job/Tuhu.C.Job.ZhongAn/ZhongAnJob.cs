using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K.BLL;

namespace Tuhu.Yewu.Job.ZhongAn
{
    [DisallowConcurrentExecution]
    public class ZhongAnJob : IInterruptableJob
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ZhongAnJob));

        bool isStop = false;
        public void Execute(IJobExecutionContext context)
        {
            _logger.Info("众安保险系统服务开始");

            try
            {
                if (!isStop)
                {
                    lock (this)
                    {
                        _logger.Info("开始发送取消承保数据");
                        Business.CancelInsurance(_logger);
                    }
                }

                if (!isStop)
                    lock (this)
                    {
                        _logger.Info("开始发送录入数据");
                        Business.FirstSend(_logger);
                    }

                if (!isStop)
                    lock (this)
                    {
                        _logger.Info("开始发送承保数据");
                        Business.SecondSend(_logger);
                    }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            _logger.Info("众安保险系统服务结束");
        }

        public void Interrupt()
        {
            lock (this)
            {
                isStop = true;
            }
        }
    }
}
