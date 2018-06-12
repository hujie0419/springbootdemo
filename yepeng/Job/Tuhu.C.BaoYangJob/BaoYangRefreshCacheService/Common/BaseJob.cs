using System;
using BaoYangRefreshCacheService.DAL;
using Common.Logging;
using Quartz;

namespace BaoYangRefreshCacheService.Common
{
    public abstract class BaseJob : IJob
    {
        protected  ILog logger;

        protected abstract string JobName { get; }

        //protected BaseJob()
        //{
        //    Log = LogManager.GetLogger(typeof (BaseJob));
        //}

        public virtual void Execute(IJobExecutionContext context)
        {
            try
            {
                if (context == null)
                {
                    logger.Info(this.ToString() + " manully begin");
                }
                else
                {
                    logger.Info(this.ToString() + " begin");
                }

                int id = BaoYangDal.InsertJobHistory(JobName);

                try
                {
                    Exec();
                    BaoYangDal.UpdateJobHistory(id, "Success");
                }
                catch (Exception ex)
                {
                    BaoYangDal.UpdateJobHistory(id, "Fail");
                    logger.Error(ex.Message, ex);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            finally
            {
                logger.Info(this.ToString() + " end");
            }
        }

        public abstract void Exec();
    }
}
