using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.CarArchiveService.Jobs
{
    public abstract class BaseJob : IJob
    {
        protected abstract ILog logger { get; }

        protected abstract string JobName { get; }

        public virtual void Execute(IJobExecutionContext context)
        {
            if (context == null)
            {
                logger.Info(this.ToString() + " manully begin");
            }
            else
            {
                logger.Info(this.ToString() + " begin");
            }
            
            try
            {
                Exec();                
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Info(this.ToString() + " end");
        }

        public abstract void Exec();
    }
}
